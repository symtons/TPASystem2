using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using TPASystem2.Helpers;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.Linq;

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
                        SELECT e.Id, CONCAT(e.FirstName, ' ', e.LastName) as FullName 
                        FROM Employees e
                        INNER JOIN Users u ON e.UserId = u.Id
                        WHERE e.IsActive = 1 
                        AND u.Role IN ('MANAGER', 'HR', 'ADMIN', 'SUPERADMIN')
                        ORDER BY e.FirstName, e.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlManager.Items.Clear();
                            ddlManager.Items.Add(new ListItem("Select Manager (Optional)", ""));

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

        #endregion

        #region Employee Creation Methods

        private void SaveEmployee(bool addAnother)
        {
            try
            {
                int createdByUserId = Convert.ToInt32(Session["UserId"]);

                // Generate company email
                string companyEmail = GenerateCompanyEmail(txtFirstName.Text.Trim(), txtLastName.Text.Trim());

                // Check if company email already exists
                if (IsCompanyEmailTaken(companyEmail))
                {
                    ShowMessage($"The company email '{companyEmail}' is already in use. Please contact IT support for manual assignment.", "error");
                    return;
                }

                // Call the stored procedure to create employee with onboarding
                var result = CreateEmployeeWithOnboarding(createdByUserId, companyEmail);

                if (result.Success)
                {
                    LogActivity(createdByUserId, "Employee Created", "Employee",
                               $"Created employee: {txtFirstName.Text} {txtLastName.Text} ({result.EmployeeNumber})",
                               GetClientIP());

                    // Send welcome email with company credentials to personal email
                    SendWelcomeEmail(txtEmail.Text.Trim(), txtFirstName.Text.Trim(), txtLastName.Text.Trim(),
                                   companyEmail, result.TempPassword);

                    // Always show success modal
                    ShowSuccessModal(result.EmployeeNumber, result.EmployeeName, result.Department, result.OnboardingTasksCount, companyEmail);

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

        private string GenerateCompanyEmail(string firstName, string lastName)
        {
            // Format: lastname + first letter of first name + @tennesseepersonalassistance.org
            string cleanLastName = CleanEmailPart(lastName).ToLower();
            string firstInitial = CleanEmailPart(firstName.Substring(0, 1)).ToLower();

            string baseEmail = $"{cleanLastName}{firstInitial}@tennesseepersonalassistance.org";

            // Check for uniqueness and add numbers if needed
            string finalEmail = baseEmail;
            int counter = 1;

            while (IsCompanyEmailTaken(finalEmail))
            {
                finalEmail = $"{cleanLastName}{firstInitial}{counter}@tennesseepersonalassistance.org";
                counter++;
            }

            return finalEmail;
        }

        private string CleanEmailPart(string input)
        {
            // Remove any non-alphanumeric characters and spaces
            return Regex.Replace(input, @"[^a-zA-Z0-9]", "");
        }

        private bool IsCompanyEmailTaken(string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT COUNT(*) 
                        FROM Users 
                        WHERE LOWER(Email) = LOWER(@Email)
                        UNION ALL
                        SELECT COUNT(*) 
                        FROM Employees 
                        WHERE LOWER(Email) = LOWER(@Email)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int totalCount = 0;
                            while (reader.Read())
                            {
                                totalCount += Convert.ToInt32(reader[0]);
                            }
                            return totalCount > 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking email uniqueness: {ex.Message}");
                return true; // Assume taken if there's an error
            }
        }

        private void SendWelcomeEmail(string personalEmail, string firstName, string lastName, string companyEmail, string tempPassword)
        {
           
                // Create email message
                MailMessage message = new MailMessage();
                message.From = new MailAddress("simbac@tennesseepersonalassistance.org", "TPA Human Resources");
                message.To.Add(personalEmail);
                message.Subject = "Welcome to Tennessee Personal Assistance - Your Account Details";
                message.IsBodyHtml = true;

                // Create email body
                string emailBody = $@"
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .header {{ background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%); color: white; padding: 20px; text-align: center; }}
                            .content {{ padding: 20px; background: #f9f9f9; }}
                            .credentials {{ background: white; padding: 15px; border-left: 4px solid #ff9800; margin: 20px 0; }}
                            .footer {{ background: #333; color: white; padding: 15px; text-align: center; font-size: 12px; }}
                            .highlight {{ color: #ff9800; font-weight: bold; }}
                        </style>
                    </head>
                    <body>
                        <div class='header'>
                            <h1>Welcome to Tennessee Personal Assistance!</h1>
                        </div>
                        
                        <div class='content'>
                            <h2>Dear {firstName} {lastName},</h2>
                            
                            <p>Welcome to the Tennessee Personal Assistance team! We're excited to have you join our organization.</p>
                            
                            <p>Your employee account has been created and your onboarding process has begun. Below are your login credentials for our HR system:</p>
                            
                            <div class='credentials'>
                                <h3>Your Company Account Details:</h3>
                                <p><strong>Company Email:</strong> <span class='highlight'>{companyEmail}</span></p>
                                <p><strong>Temporary Password:</strong> <span class='highlight'>{tempPassword}</span></p>
                                <p><strong>Login URL:</strong> <a href='https://hr.tennesseepersonalassistance.org/login'>https://hr.tennesseepersonalassistance.org/login</a></p>
                            </div>
                            
                            <h3>Important Next Steps:</h3>
                            <ul>
                                <li><strong>First Login:</strong> Use the credentials above to log into the HR system</li>
                                <li><strong>Change Password:</strong> You'll be required to change your password on first login</li>
                                <li><strong>Complete Onboarding:</strong> Review and complete your onboarding tasks in the system</li>
                                <li><strong>Contact Information:</strong> Update your profile with current contact information</li>
                            </ul>
                            
                            <h3>Email Setup:</h3>
                            <p>Your company email address is <strong>{companyEmail}</strong>. This will be your primary email for all company communications. Your personal email ({personalEmail}) will be kept on file as a backup contact method.</p>
                            
                            <p>If you have any questions or need assistance, please don't hesitate to contact our HR department at <a href='mailto:simbac@tennesseepersonalassistance.org'>simbac@tennesseepersonalassistance.org</a> or call us at (615) 555-0123.</p>
                            
                            <p>We look forward to working with you!</p>
                            
                            <p>Best regards,<br/>
                            <strong>Tennessee Personal Assistance<br/>
                            Human Resources Department</strong></p>
                        </div>
                        
                        <div class='footer'>
                            <p>&copy; 2025 Tennessee Personal Assistance. All rights reserved.<br/>
                            This email contains confidential information. Please do not forward.</p>
                        </div>
                    </body>
                    </html>";

                message.Body = emailBody;

                // Configure SMTP client for Office 365
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = "smtp.office365.com";
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("simbac@tennesseepersonalassistance.org", "97072066Sc@2025");

                // Send the email
                smtpClient.Send(message);

                // Log success
                System.Diagnostics.Debug.WriteLine($"Welcome email sent successfully to: {personalEmail}");
                System.Diagnostics.Debug.WriteLine($"From: simbac@tennesseepersonalassistance.org");
                System.Diagnostics.Debug.WriteLine($"Company Email: {companyEmail}");
                System.Diagnostics.Debug.WriteLine($"Temp Password: {tempPassword}");

                message.Dispose();
                smtpClient.Dispose();
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine($"Error sending welcome email: {ex.Message}");
            //    // Log the error but don't throw - employee creation should still succeed
            //    LogEmailError(personalEmail, ex.Message);
            //}
        }

        private void LogEmailError(string recipientEmail, string errorMessage)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ErrorLogs (ErrorMessage, Source, Timestamp, Severity, CreatedAt)
                        VALUES (@ErrorMessage, @Source, @Timestamp, @Severity, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", $"Failed to send welcome email to {recipientEmail}: {errorMessage}");
                        cmd.Parameters.AddWithValue("@Source", "AddEmployee.SendWelcomeEmail");
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@Severity", "Medium");
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception logEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging email error: {logEx.Message}");
            }
        }

        private dynamic CreateEmployeeWithOnboarding(int createdByUserId, string companyEmail)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Use the fixed password and salt values
                    string tempPassword = string.IsNullOrEmpty(txtTemporaryPassword.Text) ? "test123" : txtTemporaryPassword.Text.Trim();
                    string salt = "testsalt";
                    string passwordHash = "7UqSUHMlJ2oKwgsnJCCh/RdOpcTdJI537HSRDFW4OmY=";

                    using (SqlCommand cmd = new SqlCommand("sp_CreateEmployeeWithOnboarding", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 60; // Increase timeout for complex operations

                        // Add ALL parameters that the stored procedure expects
                        cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", companyEmail); // Use company email for login
                        cmd.Parameters.AddWithValue("@Position", txtPosition.Text.Trim());
                        cmd.Parameters.AddWithValue("@DepartmentId", Convert.ToInt32(ddlDepartment.SelectedValue));
                        cmd.Parameters.AddWithValue("@TemporaryPassword", tempPassword);
                        cmd.Parameters.AddWithValue("@PhoneNumber",
                            string.IsNullOrEmpty(txtPhoneNumber.Text) ?
                            (object)DBNull.Value : txtPhoneNumber.Text.Trim());
                        cmd.Parameters.AddWithValue("@EmployeeType", ddlEmployeeType.SelectedValue);
                        cmd.Parameters.AddWithValue("@HireDate", DateTime.Today); // Default to today's date
                        cmd.Parameters.AddWithValue("@Status", "Active"); // Default status
                        cmd.Parameters.AddWithValue("@ManagerId",
                            string.IsNullOrEmpty(ddlManager.SelectedValue) || ddlManager.SelectedValue == "0" ?
                            (object)DBNull.Value : Convert.ToInt32(ddlManager.SelectedValue));
                        cmd.Parameters.AddWithValue("@WorkLocation",
                            string.IsNullOrEmpty(txtWorkLocation.Text) ? "Office" : txtWorkLocation.Text.Trim());
                        cmd.Parameters.AddWithValue("@Salary",
                            string.IsNullOrEmpty(txtSalary.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtSalary.Text));
                        cmd.Parameters.AddWithValue("@Address", (object)DBNull.Value); // Not collected in form
                        cmd.Parameters.AddWithValue("@City", (object)DBNull.Value); // Not collected in form
                        cmd.Parameters.AddWithValue("@State", (object)DBNull.Value); // Not collected in form
                        cmd.Parameters.AddWithValue("@ZipCode", (object)DBNull.Value); // Not collected in form
                        cmd.Parameters.AddWithValue("@DateOfBirth", (object)DBNull.Value); // Not collected in form
                        cmd.Parameters.AddWithValue("@Gender", (object)DBNull.Value); // Not collected in form
                        cmd.Parameters.AddWithValue("@MustChangePassword", chkMustChangePassword.Checked);
                        cmd.Parameters.AddWithValue("@CreatedByUserId", createdByUserId);
                        cmd.Parameters.AddWithValue("@Salt", salt);
                        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new
                                {
                                    Success = !reader["ErrorMessage"].ToString().Contains("Error") && !string.IsNullOrEmpty(reader["EmployeeNumber"].ToString()),
                                    EmployeeNumber = reader["EmployeeNumber"]?.ToString() ?? "",
                                    EmployeeName = reader["EmployeeName"]?.ToString() ?? "",
                                    Department = reader["Department"]?.ToString() ?? "",
                                    OnboardingTasksCount = Convert.ToInt32(reader["OnboardingTasks"] ?? 0),
                                    ErrorMessage = reader["ErrorMessage"]?.ToString() ?? "",
                                    TempPassword = tempPassword
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Database error in CreateEmployeeWithOnboarding: {ex}");
                    return new
                    {
                        Success = false,
                        EmployeeNumber = "",
                        EmployeeName = "",
                        Department = "",
                        OnboardingTasksCount = 0,
                        ErrorMessage = ex.Message,
                        TempPassword = ""
                    };
                }
            }

            return new
            {
                Success = false,
                EmployeeNumber = "",
                EmployeeName = "",
                Department = "",
                OnboardingTasksCount = 0,
                ErrorMessage = "Unknown error occurred",
                TempPassword = ""
            };
        }

        private string GenerateSecurePassword()
        {
            // Generate a secure 12-character password
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void ShowSuccessModal(string employeeNumber, string employeeName, string department, int taskCount, string companyEmail)
        {
            string safeEmployeeNumber = employeeNumber.Replace("'", "\\'");
            string safeEmployeeName = employeeName.Replace("'", "\\'");
            string safeDepartment = department.Replace("'", "\\'");
            string safeCompanyEmail = companyEmail.Replace("'", "\\'");

            string script = $@"
                setTimeout(function() {{
                    if (typeof showSuccessModalWithEmail === 'function') {{
                        showSuccessModalWithEmail('{safeEmployeeNumber}', '{safeEmployeeName}', '{safeDepartment}', {taskCount}, '{safeCompanyEmail}');
                    }} else {{
                        alert('Employee created successfully!\\nEmployee #: {safeEmployeeNumber}\\nName: {safeEmployeeName}\\nDepartment: {safeDepartment}\\nCompany Email: {safeCompanyEmail}\\nOnboarding tasks: {taskCount}');
                    }}
                }}, 100);";

            ClientScript.RegisterStartupScript(this.GetType(), "ShowSuccessModal", script, true);
        }

        private void ClearForm()
        {
            // Clear all form fields - ONLY fields that exist
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmail.Text = "";
            txtPhoneNumber.Text = "";
            txtPosition.Text = "";
            txtTemporaryPassword.Text = "";
            txtSalary.Text = "";
            txtWorkLocation.Text = "Office";

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

        private void LogActivity(int userId, string action, string entityType, string details, string ipAddress)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO RecentActivities (UserId, ActivityTypeId, Action, EntityType, Details, IPAddress, CreatedAt)
                        VALUES (@UserId, 1, @Action, @EntityType, @Details, @IPAddress, GETUTCDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@EntityType", entityType);
                        cmd.Parameters.AddWithValue("@Details", details);
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
            string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            return ipAddress ?? "Unknown";
        }

        #endregion
    }

    // Helper class for URL routing (if it doesn't exist)
    public static class SimpleUrlHelper
    {
        public static void RedirectToCleanUrl(string page)
        {
            System.Web.HttpContext.Current.Response.Redirect($"~/{page}.aspx");
        }
    }
}