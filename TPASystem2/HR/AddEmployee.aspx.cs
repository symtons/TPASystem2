using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using TPASystem2.Helpers;

namespace TPASystem2.HR
{
    public partial class AddEmployee : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Set UnobtrusiveValidationMode to None to avoid jQuery requirement
            Page.UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;

            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                SimpleUrlHelper.RedirectToCleanUrl("login");
                return;
            }

            // Check permissions - only HR and Admin can add employees
            string userRole = Session["UserRole"]?.ToString() ?? "";
            if (!HasEmployeeCreationAccess(userRole))
            {
                Response.Redirect("~/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadFormData();
                SetDefaultValues();
            }
        }

        #region Initialization Methods

        private void LoadFormData()
        {
            LoadDepartments();
            LoadManagers();
        }

        private void LoadDepartments()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT Id, Name 
                        FROM Departments 
                        WHERE IsActive = 1 
                        ORDER BY Name";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlDepartment.Items.Clear();
                            ddlDepartment.Items.Add(new ListItem("Select Department", ""));

                            while (reader.Read())
                            {
                                ddlDepartment.Items.Add(new ListItem(
                                    reader["Name"].ToString(),
                                    reader["Id"].ToString()
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading departments: {ex.Message}", "error");
            }
        }

        private void LoadManagers()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT e.Id, e.FirstName + ' ' + e.LastName as FullName
                        FROM Employees e
                        INNER JOIN Users u ON e.UserId = u.Id
                        WHERE e.Status = 'Active'
                        AND u.IsActive = 1
                        AND (u.Role LIKE '%Manager%' OR u.Role LIKE '%Director%' OR u.Role LIKE '%Admin%')
                        ORDER BY e.FirstName, e.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlManager.Items.Clear();
                            ddlManager.Items.Add(new ListItem("Select Manager", ""));

                            while (reader.Read())
                            {
                                ddlManager.Items.Add(new ListItem(
                                    reader["FullName"].ToString(),
                                    reader["Id"].ToString()
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading managers: {ex.Message}", "error");
            }
        }

        private void SetDefaultValues()
        {
            // Set default hire date to today
            

            // Set default employee type to Full-time
            ddlEmployeeType.SelectedValue = "Full-time";

            // Set default work location
            txtWorkLocation.Text = "Office";

            // Check require password change by default
            chkMustChangePassword.Checked = true;
        }

        #endregion

        #region Event Handlers

        protected void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveEmployee(false);
            }
        }

        protected void btnSaveAndNew_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveEmployee(true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/Employees.aspx");
        }

        protected void btnSaveEmployeeBottom_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveEmployee(false);
            }
        }

        protected void btnCancelBottom_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/Employees.aspx");
        }

        #endregion

        #region Employee Creation Methods

        private void SaveEmployee(bool addAnother)
        {
            try
            {
                int createdByUserId = Convert.ToInt32(Session["UserId"]);

                // Call the stored procedure to create employee with onboarding
                var result = CreateEmployeeWithOnboarding(createdByUserId);

                if (result.Success)
                {
                    LogActivity(createdByUserId, "Employee Created", "Employee",
                               $"Created employee: {txtFirstName.Text} {txtLastName.Text} ({result.EmployeeNumber})",
                               GetClientIP());

                    // Always show success modal
                    ShowSuccessModal(result.EmployeeNumber, result.EmployeeName, result.Department, result.OnboardingTasksCount);

                    if (addAnother)
                    {
                        // Clear form for adding another employee
                        ClearForm();
                    }
                }
                else
                {
                    ShowMessage($"Error creating employee: {result.ErrorMessage}", "error");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error creating employee: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in SaveEmployee: {ex}");
            }
        }

        private dynamic CreateEmployeeWithOnboarding(int createdByUserId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Generate salt and hash password using TPASystem2 method
                    string tempPassword = string.IsNullOrEmpty(txtTemporaryPassword.Text) ? "test123" : txtTemporaryPassword.Text.Trim();
                    string salt = PasswordHelper.GenerateSalt();
                    string passwordHash = PasswordHelper.ComputeHash(tempPassword, salt);

                    using (SqlCommand cmd = new SqlCommand("sp_CreateEmployeeWithOnboarding", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 60; // Increase timeout for complex operations

                        // Add parameters matching the stored procedure
                        cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@Position", txtPosition.Text.Trim());
                        cmd.Parameters.AddWithValue("@DepartmentId", Convert.ToInt32(ddlDepartment.SelectedValue));
                        cmd.Parameters.AddWithValue("@TemporaryPassword", tempPassword);
                        cmd.Parameters.AddWithValue("@PhoneNumber",
                            string.IsNullOrEmpty(txtPhoneNumber.Text) ? (object)DBNull.Value : txtPhoneNumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@EmployeeType", ddlEmployeeType.SelectedValue);
                        cmd.Parameters.AddWithValue("@HireDate", Convert.ToDateTime(DateTime.Now));
                        cmd.Parameters.AddWithValue("@Status", "Active");
                        cmd.Parameters.AddWithValue("@ManagerId",
                            string.IsNullOrEmpty(ddlManager.SelectedValue) || ddlManager.SelectedValue == "0" ?
                            (object)DBNull.Value : Convert.ToInt32(ddlManager.SelectedValue));
                        cmd.Parameters.AddWithValue("@WorkLocation",
                            string.IsNullOrEmpty(txtWorkLocation.Text) ? "Office" : txtWorkLocation.Text.Trim());
                        cmd.Parameters.AddWithValue("@Salary",
                            string.IsNullOrEmpty(txtSalary.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtSalary.Text));
                       
                        cmd.Parameters.AddWithValue("@MustChangePassword", chkMustChangePassword.Checked);
                        cmd.Parameters.AddWithValue("@CreatedByUserId", createdByUserId);

                        // Add the generated salt and password hash
                        cmd.Parameters.AddWithValue("@Salt", salt);
                        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                        // Execute and get the result
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Create anonymous object to return results
                                return new
                                {
                                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                    EmployeeNumber = reader["EmployeeNumber"].ToString(),
                                    EmployeeName = reader["EmployeeName"].ToString(),
                                    OnboardingTasksCount = Convert.ToInt32(reader["OnboardingTasks"]),
                                    Department = reader["Department"].ToString(),
                                    Message = reader["Message"].ToString(),
                                    ErrorMessage = reader["ErrorMessage"].ToString(),
                                    Success = Convert.ToInt32(reader["EmployeeId"]) > 0 && string.IsNullOrEmpty(reader["ErrorMessage"].ToString())
                                };
                            }
                            else
                            {
                                return new
                                {
                                    EmployeeId = 0,
                                    EmployeeNumber = "",
                                    EmployeeName = "",
                                    OnboardingTasksCount = 0,
                                    Department = "",
                                    Message = "",
                                    ErrorMessage = "No result returned from stored procedure",
                                    Success = false
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new
                    {
                        EmployeeId = 0,
                        EmployeeNumber = "",
                        EmployeeName = "",
                        OnboardingTasksCount = 0,
                        Department = "",
                        Message = "",
                        ErrorMessage = ex.Message,
                        Success = false
                    };
                }
            }
        }

        // Method to show the success modal - Alternative approach
        private void ShowSuccessModal(string employeeNumber, string employeeName, string department, int taskCount)
        {
            // Escape single quotes in the data to prevent JavaScript errors
            string safeEmployeeNumber = employeeNumber.Replace("'", "\\'");
            string safeEmployeeName = employeeName.Replace("'", "\\'");
            string safeDepartment = department.Replace("'", "\\'");

            string script = $@"
                setTimeout(function() {{
                    showSuccessModal('{safeEmployeeNumber}', '{safeEmployeeName}', '{safeDepartment}', {taskCount});
                }}, 100);";

            ClientScript.RegisterStartupScript(this.GetType(), "ShowSuccessModal", script, true);
        }

        private void ClearForm()
        {
            // Clear all form fields
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtPhoneNumber.Text = "";
            txtPosition.Text = "";
            txtTemporaryPassword.Text = "";
            txtSalary.Text = "";
            txtWorkLocation.Text = "Office";
            txtAddress.Text = "";
            txtCity.Text = "";
            txtState.Text = "";
            txtZipCode.Text = "";
            

            // Reset dropdowns to default
            if (ddlDepartment.Items.Count > 0)
                ddlDepartment.SelectedIndex = 0;

            if (ddlEmployeeType.Items.Count > 0)
                ddlEmployeeType.SelectedValue = "Full-time";

            if (ddlManager.Items.Count > 0)
                ddlManager.SelectedIndex = 0;

            
            chkMustChangePassword.Checked = true;

            // Clear any existing messages
            pnlMessage.Visible = false;
        }

        #endregion

        #region Helper Methods

        private bool HasEmployeeCreationAccess(string userRole)
        {
            return userRole.Contains("Admin") ||
                   userRole.Contains("HR") ||
                   userRole.Contains("SuperAdmin");
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            litMessage.Text = message;
            pnlMessage.CssClass = $"alert alert-{type}";
        }

        private void LogActivity(int userId, string action, string entityType, string description, string ipAddress)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ApplicationLogs (UserId, Action, EntityType, EntityId, Description, IPAddress, Timestamp)
                        VALUES (@UserId, @Action, @EntityType, NULL, @Description, @IPAddress, GETUTCDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@EntityType", entityType);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging activity: {ex.Message}");
            }
        }

        private string GetClientIP()
        {
            string ip = "";
            try
            {
                ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ip) || ip.ToLower() == "unknown")
                {
                    ip = Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch
            {
                ip = "Unknown";
            }
            return ip;
        }

        #endregion
    }
}