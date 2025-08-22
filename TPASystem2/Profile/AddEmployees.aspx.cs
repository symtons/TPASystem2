using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Web.Services;

namespace TPASystem2.Profile
{
    public partial class AddEmployee : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        #region Properties

        private int CurrentUserId
        {
            get { return Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : 0; }
        }

        private string CurrentUserRole
        {
            get { return Session["UserRole"]?.ToString() ?? ""; }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user has permission to add employees
                if (!HasEmployeeCreationAccess())
                {
                    Response.Redirect("~/Profile/ManageEmployeeProfiles.aspx");
                    return;
                }

                InitializePage();
            }
        }

        private void InitializePage()
        {
            LoadDepartments();
            LoadManagers();
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            txtHireDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
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
            Response.Redirect("~/Profile/ManageEmployeeProfiles.aspx");
        }

        protected void btnBackToProfiles_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Profile/ManageEmployeeProfiles.aspx");
        }

        #endregion

        #region Employee Creation Methods

        private void SaveEmployee(bool addAnother)
        {
            try
            {
                // Validate unique constraints
                if (IsEmployeeNumberExists(txtEmployeeNumber.Text.Trim()))
                {
                    ShowMessage("Employee Number already exists. Please use a different number.", "error");
                    return;
                }

                if (IsEmployeeEmailExists(txtEmail.Text.Trim()))
                {
                    ShowMessage("Email address already exists. Please use a different email.", "error");
                    return;
                }

                // Validate SSN format if provided
                if (!string.IsNullOrWhiteSpace(txtSSN.Text) && !IsValidSSN(txtSSN.Text.Trim()))
                {
                    ShowMessage("Please enter a valid Social Security Number (XXX-XX-XXXX or 9 digits).", "error");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Generate temporary password
                            string tempPassword = GenerateTemporaryPassword();
                            string salt = GenerateSalt();
                            string hashedPassword = HashPassword(tempPassword, salt);

                            // Create User account first
                            string userQuery = @"
                                INSERT INTO Users (Email, PasswordHash, Salt, Role, IsActive, MustChangePassword, CreatedAt, UpdatedAt, FailedLoginAttempts)
                                VALUES (@Email, @PasswordHash, @Salt, 'EMPLOYEE', 1, 1, GETUTCDATE(), GETUTCDATE(), 0);
                                SELECT SCOPE_IDENTITY();";

                            int userId;
                            using (SqlCommand userCmd = new SqlCommand(userQuery, conn, transaction))
                            {
                                userCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                                userCmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                                userCmd.Parameters.AddWithValue("@Salt", salt);
                                userId = Convert.ToInt32(userCmd.ExecuteScalar());
                            }

                            // Create Employee record - check which columns exist first
                            string employeeQuery = BuildEmployeeInsertQuery();

                            int employeeId;
                            using (SqlCommand empCmd = new SqlCommand(employeeQuery, conn, transaction))
                            {
                                // Basic required parameters
                                empCmd.Parameters.AddWithValue("@UserId", userId);
                                empCmd.Parameters.AddWithValue("@EmployeeNumber", txtEmployeeNumber.Text.Trim());
                                empCmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                                empCmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                                empCmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                                empCmd.Parameters.AddWithValue("@PhoneNumber", (object)txtPhoneNumber.Text.Trim() ?? DBNull.Value);
                                empCmd.Parameters.AddWithValue("@DepartmentId", Convert.ToInt32(ddlDepartment.SelectedValue));
                                empCmd.Parameters.AddWithValue("@JobTitle", txtJobTitle.Text.Trim());
                                empCmd.Parameters.AddWithValue("@EmployeeType", ddlEmployeeType.SelectedValue ?? "");
                                empCmd.Parameters.AddWithValue("@HireDate", DateTime.Parse(txtHireDate.Text));
                                empCmd.Parameters.AddWithValue("@Status", "Active");
                                empCmd.Parameters.AddWithValue("@ManagerId", ddlManager.SelectedValue == "0" ? (object)DBNull.Value : Convert.ToInt32(ddlManager.SelectedValue));
                                empCmd.Parameters.AddWithValue("@Address", (object)txtAddress.Text.Trim() ?? DBNull.Value);
                                empCmd.Parameters.AddWithValue("@City", (object)txtCity.Text.Trim() ?? DBNull.Value);
                                empCmd.Parameters.AddWithValue("@State", (object)txtState.Text.Trim() ?? DBNull.Value);
                                empCmd.Parameters.AddWithValue("@ZipCode", (object)txtZipCode.Text.Trim() ?? DBNull.Value);
                                empCmd.Parameters.AddWithValue("@DateOfBirth", string.IsNullOrWhiteSpace(txtDateOfBirth.Text) ? (object)DBNull.Value : DateTime.Parse(txtDateOfBirth.Text));
                                empCmd.Parameters.AddWithValue("@Gender", (object)ddlGender.SelectedValue ?? DBNull.Value);
                                empCmd.Parameters.AddWithValue("@Position", txtJobTitle.Text.Trim());
                                empCmd.Parameters.AddWithValue("@Salary", string.IsNullOrWhiteSpace(txtSalary.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtSalary.Text));
                                empCmd.Parameters.AddWithValue("@WorkLocation", (object)txtWorkLocation.Text.Trim() ?? DBNull.Value);

                                // Add additional CSV fields if columns exist
                                AddOptionalParameters(empCmd);

                                employeeId = Convert.ToInt32(empCmd.ExecuteScalar());
                            }

                            // Log activity
                            LogActivity(CurrentUserId, "CREATE", "EMPLOYEE",
                                $"Created new employee: {txtFirstName.Text} {txtLastName.Text} ({txtEmployeeNumber.Text})",
                                GetClientIPAddress());

                            transaction.Commit();

                            // Send notification about new employee
                            SendNewEmployeeNotification(employeeId, tempPassword);

                            // Get department name for display
                            string departmentName = ddlDepartment.SelectedItem.Text;

                            // Show success modal
                            ShowSuccessModal(txtEmployeeNumber.Text.Trim(),
                                $"{txtFirstName.Text.Trim()} {txtLastName.Text.Trim()}",
                                departmentName, txtEmail.Text.Trim(), tempPassword);

                            if (addAnother)
                            {
                                ClearForm();
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error creating employee. Please try again.", "error");
            }
        }

        private string BuildEmployeeInsertQuery()
        {
            // Base query with always-present columns
            var columns = new List<string>
            {
                "UserId", "EmployeeNumber", "FirstName", "LastName", "Email", "PhoneNumber",
                "DepartmentId", "JobTitle", "EmployeeType", "HireDate", "Status", "ManagerId",
                "Address", "City", "State", "ZipCode", "DateOfBirth", "Gender", "Position",
                "Salary", "WorkLocation", "CreatedAt", "UpdatedAt", "IsActive", "EmploymentStatus",
                "OnboardingStatus", "IsOnboardingLocked"
            };

            var values = new List<string>
            {
                "@UserId", "@EmployeeNumber", "@FirstName", "@LastName", "@Email", "@PhoneNumber",
                "@DepartmentId", "@JobTitle", "@EmployeeType", "@HireDate", "@Status", "@ManagerId",
                "@Address", "@City", "@State", "@ZipCode", "@DateOfBirth", "@Gender", "@Position",
                "@Salary", "@WorkLocation", "GETUTCDATE()", "GETUTCDATE()", "1", "'Active'",
                "'COMPLETED'", "1"
            };

            // Check for additional columns from CSV and add if they exist
            var additionalColumns = new Dictionary<string, string>
            {
                { "SSN", "@SSN" },
                { "DriversLicense", "@DriversLicense" },
                { "DirectDeposit", "@DirectDeposit" },
                { "VacationDays", "@VacationDays" },
                { "SickDays", "@SickDays" },
                { "Insurance", "@Insurance" },
                { "HoursPerWeek", "@HoursPerWeek" },
                { "Participation403b", "@Participation403b" },
                { "VehicleInsurance", "@VehicleInsurance" },
                { "Comments", "@Comments" }
            };

            foreach (var column in additionalColumns)
            {
                if (ColumnExists("Employees", column.Key))
                {
                    columns.Add(column.Key);
                    values.Add(column.Value);
                }
            }

            return $@"
                INSERT INTO Employees ({string.Join(", ", columns)})
                VALUES ({string.Join(", ", values)});
                SELECT SCOPE_IDENTITY();";
        }

        private void AddOptionalParameters(SqlCommand cmd)
        {
            // Add parameters for additional CSV fields if columns exist
            if (ColumnExists("Employees", "SSN"))
                cmd.Parameters.AddWithValue("@SSN", (object)txtSSN.Text.Trim() ?? DBNull.Value);

            if (ColumnExists("Employees", "DriversLicense"))
                cmd.Parameters.AddWithValue("@DriversLicense", (object)txtDriversLicense.Text.Trim() ?? DBNull.Value);

            if (ColumnExists("Employees", "DirectDeposit"))
            {
                string directDepositValue = ddlDirectDeposit.SelectedValue;
                if (ddlDirectDeposit.SelectedValue == "Yes" || ddlDirectDeposit.SelectedValue == "Pending")
                {
                    var details = new List<string>();

                    if (!string.IsNullOrWhiteSpace(txtBankName.Text))
                        details.Add($"Bank: {txtBankName.Text.Trim()}");

                    if (!string.IsNullOrWhiteSpace(txtRoutingNumber.Text))
                        details.Add($"Routing: {txtRoutingNumber.Text.Trim()}");

                    if (!string.IsNullOrWhiteSpace(txtAccountNumber.Text))
                    {
                        // Mask account number for security (show only last 4 digits)
                        string accountNumber = txtAccountNumber.Text.Trim();
                        string maskedAccount = accountNumber.Length > 4
                            ? new string('*', accountNumber.Length - 4) + accountNumber.Substring(accountNumber.Length - 4)
                            : accountNumber;
                        details.Add($"Account: {maskedAccount}");
                    }

                    if (!string.IsNullOrWhiteSpace(ddlAccountType.SelectedValue))
                        details.Add($"Type: {ddlAccountType.SelectedValue}");

                    if (details.Any())
                        directDepositValue += " | " + string.Join(" | ", details);
                }
                cmd.Parameters.AddWithValue("@DirectDeposit", directDepositValue);
            }

            if (ColumnExists("Employees", "VacationDays"))
                cmd.Parameters.AddWithValue("@VacationDays", string.IsNullOrWhiteSpace(txtVacationDays.Text) ? (object)DBNull.Value : Convert.ToInt32(txtVacationDays.Text));

            if (ColumnExists("Employees", "SickDays"))
                cmd.Parameters.AddWithValue("@SickDays", string.IsNullOrWhiteSpace(txtSickDays.Text) ? (object)DBNull.Value : Convert.ToInt32(txtSickDays.Text));

            if (ColumnExists("Employees", "Insurance"))
            {
                string insuranceValue = ddlInsurance.SelectedValue;
                if (!string.IsNullOrWhiteSpace(txtInsuranceDetails.Text))
                {
                    insuranceValue += ": " + txtInsuranceDetails.Text.Trim();
                }
                cmd.Parameters.AddWithValue("@Insurance", insuranceValue);
            }

            if (ColumnExists("Employees", "HoursPerWeek"))
                cmd.Parameters.AddWithValue("@HoursPerWeek", string.IsNullOrWhiteSpace(txtHoursPerWeek.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtHoursPerWeek.Text));

            if (ColumnExists("Employees", "Participation403b"))
            {
                string participation403bValue = ddl403b.SelectedValue;
                if (!string.IsNullOrWhiteSpace(txt403bDetails.Text))
                {
                    participation403bValue += ": " + txt403bDetails.Text.Trim();
                }
                cmd.Parameters.AddWithValue("@Participation403b", participation403bValue);
            }

            if (ColumnExists("Employees", "VehicleInsurance"))
            {
                string vehicleInsuranceValue = ddlVehicleInsurance.SelectedValue;
                if (!string.IsNullOrWhiteSpace(txtVehicleInsuranceDetails.Text))
                {
                    vehicleInsuranceValue += ": " + txtVehicleInsuranceDetails.Text.Trim();
                }
                cmd.Parameters.AddWithValue("@VehicleInsurance", vehicleInsuranceValue);
            }

            if (ColumnExists("Employees", "Comments"))
                cmd.Parameters.AddWithValue("@Comments", (object)txtComments.Text.Trim() ?? DBNull.Value);
        }

        private bool ColumnExists(string tableName, string columnName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT COUNT(*) 
                        FROM sys.columns 
                        WHERE object_id = OBJECT_ID(@TableName) AND name = @ColumnName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TableName", $"[dbo].[{tableName}]");
                        cmd.Parameters.AddWithValue("@ColumnName", columnName);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Data Loading Methods

        private void LoadDepartments()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Name FROM Departments WHERE IsActive = 1 ORDER BY Name";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlDepartment.Items.Clear();
                            ddlDepartment.Items.Add(new ListItem("Select Department", "0"));

                            while (reader.Read())
                            {
                                ddlDepartment.Items.Add(new ListItem(reader["Name"].ToString(), reader["Id"].ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
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
                        SELECT e.Id, e.FirstName + ' ' + e.LastName + ' (' + e.EmployeeNumber + ')' as FullName
                        FROM Employees e
                        INNER JOIN Users u ON e.UserId = u.Id
                        WHERE e.IsActive = 1 
                        AND (u.Role LIKE '%Manager%' OR u.Role LIKE '%Director%' OR u.Role LIKE '%Admin%' OR u.Role LIKE '%Supervisor%')
                        ORDER BY e.FirstName, e.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlManager.Items.Clear();
                            ddlManager.Items.Add(new ListItem("No Manager", "0"));

                            while (reader.Read())
                            {
                                ddlManager.Items.Add(new ListItem(reader["FullName"].ToString(), reader["Id"].ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        #endregion

        #region Validation Methods

        private bool IsEmployeeNumberExists(string employeeNumber)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Employees WHERE EmployeeNumber = @EmployeeNumber";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return true; // Assume exists to be safe
            }
        }

        private bool IsEmployeeEmailExists(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Employees WHERE Email = @Email";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return true; // Assume exists to be safe
            }
        }

        #endregion

        #region Utility Methods

        private bool HasEmployeeCreationAccess()
        {
            string userRole = CurrentUserRole?.ToUpper() ?? "";
            return userRole.Contains("ADMIN") ||
                   userRole.Contains("HR") ||
                   userRole.Contains("PROGRAMDIRECTOR");
        }

        private bool IsValidSSN(string ssn)
        {
            if (string.IsNullOrWhiteSpace(ssn))
                return true; // SSN is optional

            // Remove any formatting
            string cleanSSN = ssn.Replace("-", "").Replace(" ", "");

            // Check if it's 9 digits
            return cleanSSN.Length == 9 && cleanSSN.All(char.IsDigit);
        }

        private string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789@#$%";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string GenerateSalt()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, 32)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + salt));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private void ClearForm()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmployeeNumber.Text = "";
            txtEmail.Text = "";
            txtPhoneNumber.Text = "";
            txtJobTitle.Text = "";
            txtAddress.Text = "";
            txtCity.Text = "";
            txtState.Text = "";
            txtZipCode.Text = "";
            txtDateOfBirth.Text = "";
            txtSSN.Text = "";
            txtDriversLicense.Text = "";
            txtSalary.Text = "";
            txtWorkLocation.Text = "";
            txtVacationDays.Text = "";
            txtSickDays.Text = "";
            txtHoursPerWeek.Text = "";
            txtComments.Text = "";
            txtBankName.Text = "";
            txtRoutingNumber.Text = "";
            txtAccountNumber.Text = "";
            txtInsuranceDetails.Text = "";
            txt403bDetails.Text = "";
            txtVehicleInsuranceDetails.Text = "";
            txtHireDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

            if (ddlDepartment.Items.Count > 0)
                ddlDepartment.SelectedIndex = 0;

            if (ddlEmployeeType.Items.Count > 0)
                ddlEmployeeType.SelectedIndex = 0;

            if (ddlManager.Items.Count > 0)
                ddlManager.SelectedIndex = 0;

            if (ddlGender.Items.Count > 0)
                ddlGender.SelectedIndex = 0;

            if (ddlDirectDeposit.Items.Count > 0)
                ddlDirectDeposit.SelectedIndex = 0;

            if (ddlAccountType.Items.Count > 0)
                ddlAccountType.SelectedIndex = 0;

            if (ddlInsurance.Items.Count > 0)
                ddlInsurance.SelectedIndex = 0;

            if (ddl403b.Items.Count > 0)
                ddl403b.SelectedIndex = 0;

            if (ddlVehicleInsurance.Items.Count > 0)
                ddlVehicleInsurance.SelectedIndex = 0;

            // Clear any existing messages
            pnlMessage.Visible = false;
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            litMessage.Text = message;
            pnlMessage.CssClass = $"alert alert-{type}";
        }

        private void ShowSuccessModal(string employeeNumber, string employeeName, string department, string email, string tempPassword)
        {
            string safeEmployeeNumber = employeeNumber.Replace("'", "\\'");
            string safeEmployeeName = employeeName.Replace("'", "\\'");
            string safeDepartment = department.Replace("'", "\\'");
            string safeEmail = email.Replace("'", "\\'");
            string safeTempPassword = tempPassword.Replace("'", "\\'");

            string script = $@"
                setTimeout(function() {{
                    showSuccessModal('{safeEmployeeNumber}', '{safeEmployeeName}', '{safeDepartment}', '{safeEmail}', '{safeTempPassword}');
                }}, 100);";

            ClientScript.RegisterStartupScript(this.GetType(), "ShowSuccessModal", script, true);
        }

        private void SendNewEmployeeNotification(int employeeId, string tempPassword)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO EmailQueue (
                            ToEmail, Subject, Body, Priority, Status, CreatedAt, RelatedEmployeeId, CreatedBy
                        )
                        VALUES (
                            @ToEmail, @Subject, @Body, 'NORMAL', 'PENDING', GETUTCDATE(), @EmployeeId, @CreatedBy
                        )";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ToEmail", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@Subject", "Welcome to TPA System - Account Created");
                        cmd.Parameters.AddWithValue("@Body", $@"
                            Welcome to TPA System!
                            
                            Your account has been created with the following details:
                            Employee Number: {txtEmployeeNumber.Text}
                            Name: {txtFirstName.Text} {txtLastName.Text}
                            Email: {txtEmail.Text}
                            Department: {ddlDepartment.SelectedItem.Text}
                            Temporary Password: {tempPassword}
                            
                            Please log in to the system and change your password upon first login.
                            
                            If you have any questions, please contact your supervisor or HR department.
                        ");
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        cmd.Parameters.AddWithValue("@CreatedBy", CurrentUserId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                // Don't fail the entire process if email notification fails
            }
        }

        private void LogActivity(int userId, string action, string entityType, string details, string ipAddress)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO RecentActivities (UserId, Action, EntityType, Details, IPAddress, Timestamp)
                        VALUES (@UserId, @Action, @EntityType, @Details, @IPAddress, GETUTCDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@EntityType", entityType);
                        cmd.Parameters.AddWithValue("@Details", details);
                        cmd.Parameters.AddWithValue("@IPAddress", ipAddress ?? "");

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the main operation
                LogError(ex);
            }
        }

        private void LogError(Exception ex)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ErrorLogs (Message, StackTrace, Source, UserId, Timestamp)
                        VALUES (@Message, @StackTrace, @Source, @UserId, GETUTCDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Message", ex.Message);
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", ex.Source ?? "");
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // If logging fails, we can't do much about it
            }
        }

        private string GetClientIPAddress()
        {
            string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown")
                ipAddress = Request.ServerVariables["REMOTE_ADDR"];

            return ipAddress ?? "";
        }

        #endregion

        #region Web Methods

        [WebMethod]
        public static string GenerateEmployeeNumber()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get the highest existing employee number
                    string query = @"
                        SELECT TOP 1 EmployeeNumber 
                        FROM Employees 
                        WHERE EmployeeNumber LIKE 'EMP%' 
                        ORDER BY CAST(SUBSTRING(EmployeeNumber, 4, LEN(EmployeeNumber)) AS INT) DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            string lastNumber = result.ToString();
                            if (lastNumber.StartsWith("EMP"))
                            {
                                string numberPart = lastNumber.Substring(3);
                                if (int.TryParse(numberPart, out int lastNum))
                                {
                                    return "EMP" + (lastNum + 1).ToString("D4");
                                }
                            }
                        }

                        // If no existing numbers found, start with EMP0001
                        return "EMP0001";
                    }
                }
            }
            catch
            {
                // If there's an error, return a random number
                Random random = new Random();
                return "EMP" + random.Next(1000, 9999).ToString();
            }
        }

        #endregion
    }
}