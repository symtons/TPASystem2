using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
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
                        WHERE e.IsActive = 1 
                        AND e.Status = 'Active'
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
            txtHireDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

            // Set default status to Active
            ddlStatus.SelectedValue = "Active";

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

                    if (addAnother)
                    {
                        ShowMessage($"Employee {result.EmployeeNumber} created successfully! You can add another employee below.", "success");
                        ClearForm();
                    }
                    else
                    {
                        // Redirect to the employee list with success message
                        Session["SuccessMessage"] = $"Employee {result.EmployeeNumber} created successfully with {result.OnboardingTasksCount} onboarding tasks assigned.";
                        Response.Redirect("~/HR/Employees.aspx");
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

        private EmployeeCreationResult CreateEmployeeWithOnboarding(int createdByUserId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("sp_CreateEmployeeWithOnboarding", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters matching the stored procedure
                    cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                    cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Position", txtPosition.Text.Trim());
                    cmd.Parameters.AddWithValue("@DepartmentId", Convert.ToInt32(ddlDepartment.SelectedValue));
                    cmd.Parameters.AddWithValue("@TemporaryPassword",
                        string.IsNullOrEmpty(txtTemporaryPassword.Text) ? "TempPass123!" : txtTemporaryPassword.Text);
                    cmd.Parameters.AddWithValue("@PhoneNumber",
                        string.IsNullOrEmpty(txtPhoneNumber.Text) ? (object)DBNull.Value : txtPhoneNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@EmployeeType", ddlEmployeeType.SelectedValue);
                    cmd.Parameters.AddWithValue("@HireDate", Convert.ToDateTime(txtHireDate.Text));
                    cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@ManagerId",
                        string.IsNullOrEmpty(ddlManager.SelectedValue) ? (object)DBNull.Value : Convert.ToInt32(ddlManager.SelectedValue));
                    cmd.Parameters.AddWithValue("@WorkLocation",
                        string.IsNullOrEmpty(txtWorkLocation.Text) ? "Office" : txtWorkLocation.Text.Trim());
                    cmd.Parameters.AddWithValue("@Salary",
                        string.IsNullOrEmpty(txtSalary.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtSalary.Text));
                    cmd.Parameters.AddWithValue("@Address",
                        string.IsNullOrEmpty(txtAddress.Text) ? (object)DBNull.Value : txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@City",
                        string.IsNullOrEmpty(txtCity.Text) ? (object)DBNull.Value : txtCity.Text.Trim());
                    cmd.Parameters.AddWithValue("@State",
                        string.IsNullOrEmpty(txtState.Text) ? (object)DBNull.Value : txtState.Text.Trim());
                    cmd.Parameters.AddWithValue("@ZipCode",
                        string.IsNullOrEmpty(txtZipCode.Text) ? (object)DBNull.Value : txtZipCode.Text.Trim());
                    cmd.Parameters.AddWithValue("@DateOfBirth",
                        string.IsNullOrEmpty(txtDateOfBirth.Text) ? (object)DBNull.Value : Convert.ToDateTime(txtDateOfBirth.Text));
                    cmd.Parameters.AddWithValue("@Gender",
                        string.IsNullOrEmpty(ddlGender.SelectedValue) ? (object)DBNull.Value : ddlGender.SelectedValue);
                    cmd.Parameters.AddWithValue("@MustChangePassword", chkMustChangePassword.Checked);
                    cmd.Parameters.AddWithValue("@CreatedByUserId", createdByUserId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Check if creation was successful
                            if (reader["ErrorMessage"] != DBNull.Value)
                            {
                                return new EmployeeCreationResult
                                {
                                    Success = false,
                                    ErrorMessage = reader["ErrorMessage"].ToString()
                                };
                            }

                            return new EmployeeCreationResult
                            {
                                Success = true,
                                EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                EmployeeNumber = reader["EmployeeNumber"].ToString(),
                                EmployeeName = reader["EmployeeName"].ToString(),
                                OnboardingTasksCount = Convert.ToInt32(reader["OnboardingTasks"]),
                                Department = reader["Department"].ToString(),
                                Message = reader["Message"].ToString()
                            };
                        }
                        else
                        {
                            return new EmployeeCreationResult
                            {
                                Success = false,
                                ErrorMessage = "No result returned from stored procedure"
                            };
                        }
                    }
                }
            }
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
            txtDateOfBirth.Text = "";

            // Reset dropdowns to default
            ddlDepartment.SelectedIndex = 0;
            ddlEmployeeType.SelectedValue = "Full-time";
            ddlManager.SelectedIndex = 0;
            ddlStatus.SelectedValue = "Active";
            ddlGender.SelectedIndex = 0;

            // Reset date and checkbox
            txtHireDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            chkMustChangePassword.Checked = true;

            // Focus on first field
            txtFirstName.Focus();
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
                        INSERT INTO ActivityLogs (UserId, Action, EntityType, Description, IPAddress, Timestamp)
                        VALUES (@UserId, @Action, @EntityType, @Description, @IPAddress, @Timestamp)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@EntityType", entityType);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow);
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
            string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            return ipAddress ?? "Unknown";
        }

        #endregion
    }

    #region Helper Classes

    public class EmployeeCreationResult
    {
        public bool Success { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeNumber { get; set; }
        public string EmployeeName { get; set; }
        public int OnboardingTasksCount { get; set; }
        public string Department { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
    }

    #endregion
}