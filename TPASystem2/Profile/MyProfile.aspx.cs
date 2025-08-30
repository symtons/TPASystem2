using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.Profile
{
    public partial class MyProfile : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user is logged in
                if (Session["UserId"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                InitializePage();
                LoadProfileData();
            }
        }

        private void InitializePage()
        {
            try
            {
                hfCurrentUserId.Value = Session["UserId"].ToString();

                // Get employee ID
                int employeeId = GetEmployeeIdFromUserId(Convert.ToInt32(Session["UserId"]));
                hfEmployeeId.Value = employeeId.ToString();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error initializing page.", "error");
            }
        }

        private void LoadProfileData()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                int employeeId = GetEmployeeIdFromUserId(userId);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            u.Id as UserId,
                            u.Email,
                            u.Role,
                            u.IsActive,
                            u.CreatedAt as UserCreatedAt,
                            u.LastLogin,
                            u.MustChangePassword,
                            u.PasswordChangedAt,
                            e.Id as EmployeeId,
                            e.FirstName,
                            e.LastName,
                            e.EmployeeNumber,
                            e.Email as EmployeeEmail,
                            e.PhoneNumber,
                            e.JobTitle,
                            e.EmployeeType,
                            e.HireDate,
                            e.Status,
                            e.Address,
                            e.City,
                            e.State,
                            e.ZipCode,
                            e.DateOfBirth,
                            e.Gender,
                            e.WorkLocation,
                            e.EmploymentStatus,
                            d.Name as DepartmentName,
                            mgr.FirstName + ' ' + mgr.LastName as ManagerName
                        FROM Users u
                        LEFT JOIN Employees e ON u.Id = e.UserId
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        LEFT JOIN Employees mgr ON e.ManagerId = mgr.Id
                        WHERE u.Id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                PopulateProfileFields(reader);
                            }
                            else
                            {
                                ShowMessage("Profile data not found.", "error");
                                return;
                            }
                        }
                    }
                }

                // Load additional data
                LoadQuickStats(employeeId);
                LoadRecentActivity(userId);
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading profile data.", "error");
            }
        }

        private void PopulateProfileFields(SqlDataReader reader)
        {
            // Header information
            string firstName = reader["FirstName"]?.ToString() ?? "";
            string lastName = reader["LastName"]?.ToString() ?? "";
            string fullName = $"{firstName} {lastName}".Trim();

            if (string.IsNullOrEmpty(fullName))
            {
                fullName = reader["Email"]?.ToString() ?? "User";
            }

            litFullName.Text = fullName;
            litProfileName.Text = fullName;
            litEmployeeNumber.Text = reader["EmployeeNumber"]?.ToString() ?? "Not assigned";
            litDepartment.Text = reader["DepartmentName"]?.ToString() ?? "Not assigned";
            litJobTitle.Text = reader["JobTitle"]?.ToString() ?? "Not assigned";
            litProfileTitle.Text = reader["JobTitle"]?.ToString() ?? "Employee";

            // Generate avatar initials
            litAvatarInitials.Text = GetInitials(fullName);

            // User role and status
            litUserRole.Text = reader["Role"]?.ToString() ?? "Employee";
            litSystemRole.Text = reader["Role"]?.ToString() ?? "Employee";

            // Personal Information
            litFirstName.Text = firstName;
            litLastName.Text = lastName;

            if (reader["DateOfBirth"] != DBNull.Value)
            {
                DateTime dob = Convert.ToDateTime(reader["DateOfBirth"]);
                litDateOfBirth.Text = dob.ToString("MMMM dd, yyyy");
            }
            else
            {
                litDateOfBirth.Text = "Not provided";
            }

            litGender.Text = reader["Gender"]?.ToString() ?? "Not specified";

            // Employment Details
            litEmpNumber.Text = reader["EmployeeNumber"]?.ToString() ?? "Not assigned";
            litPosition.Text = reader["JobTitle"]?.ToString() ?? "Not assigned";
            litDepartmentName.Text = reader["DepartmentName"]?.ToString() ?? "Not assigned";
            litEmployeeType.Text = reader["EmployeeType"]?.ToString() ?? "Not specified";
            litWorkLocation.Text = reader["WorkLocation"]?.ToString() ?? "Main Office";
            litManagerName.Text = reader["ManagerName"]?.ToString() ?? "Not assigned";
            litEmploymentStatus.Text = reader["EmploymentStatus"]?.ToString() ?? "Active";

            if (reader["HireDate"] != DBNull.Value)
            {
                DateTime hireDate = Convert.ToDateTime(reader["HireDate"]);
                litHireDate.Text = hireDate.ToString("MMMM dd, yyyy");
            }
            else
            {
                litHireDate.Text = "Not available";
            }

            // Contact Information
            litEmail.Text = reader["Email"]?.ToString() ?? reader["EmployeeEmail"]?.ToString() ?? "Not provided";
            litPhoneNumber.Text = reader["PhoneNumber"]?.ToString() ?? "Not provided";
            litAddress.Text = reader["Address"]?.ToString() ?? "Not provided";
            litCity.Text = reader["City"]?.ToString() ?? "Not provided";
            litState.Text = reader["State"]?.ToString() ?? "Not provided";
            litZipCode.Text = reader["ZipCode"]?.ToString() ?? "Not provided";

            // Security Information
            if (reader["LastLogin"] != DBNull.Value)
            {
                DateTime lastLogin = Convert.ToDateTime(reader["LastLogin"]);
                litLastLogin.Text = lastLogin.ToString("MMMM dd, yyyy 'at' hh:mm tt");
            }
            else
            {
                litLastLogin.Text = "Never logged in";
            }

            if (reader["UserCreatedAt"] != DBNull.Value)
            {
                DateTime created = Convert.ToDateTime(reader["UserCreatedAt"]);
                litAccountCreated.Text = created.ToString("MMMM dd, yyyy");
            }
            else
            {
                litAccountCreated.Text = "Unknown";
            }

            if (reader["PasswordChangedAt"] != DBNull.Value)
            {
                DateTime pwdChanged = Convert.ToDateTime(reader["PasswordChangedAt"]);
                litPasswordChanged.Text = pwdChanged.ToString("MMMM dd, yyyy");
            }
            else
            {
                litPasswordChanged.Text = "Unknown";
            }

            bool mustChangePassword = reader["MustChangePassword"] != DBNull.Value &&
                                    Convert.ToBoolean(reader["MustChangePassword"]);
            litPasswordStatus.Text = mustChangePassword ? "Must Change Password" : "Active";

            // Populate edit form fields
            PopulateEditForm(reader);
        }

        private void PopulateEditForm(SqlDataReader reader)
        {
            txtEditFirstName.Text = reader["FirstName"]?.ToString() ?? "";
            txtEditLastName.Text = reader["LastName"]?.ToString() ?? "";
            txtEditPhone.Text = reader["PhoneNumber"]?.ToString() ?? "";
            txtEditAddress.Text = reader["Address"]?.ToString() ?? "";
            txtEditCity.Text = reader["City"]?.ToString() ?? "";
            txtEditState.Text = reader["State"]?.ToString() ?? "";
            txtEditZipCode.Text = reader["ZipCode"]?.ToString() ?? "";

            if (reader["DateOfBirth"] != DBNull.Value)
            {
                DateTime dob = Convert.ToDateTime(reader["DateOfBirth"]);
                txtEditDateOfBirth.Text = dob.ToString("yyyy-MM-dd");
            }
        }

        private void LoadQuickStats(int employeeId)
        {
            try
            {
                if (employeeId == 0)
                {
                    litLeaveBalance.Text = "0";
                    litHoursWorked.Text = "0";
                    litYearsOfService.Text = "0";
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get leave balance
                    string leaveQuery = @"
                        SELECT ISNULL(SUM(BalanceDays), 0) 
                        FROM LeaveBalances 
                        WHERE EmployeeId = @EmployeeId AND LeaveYear = YEAR(GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(leaveQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        object result = cmd.ExecuteScalar();
                        litLeaveBalance.Text = (result ?? 0).ToString();
                    }

                    // Get hours worked this week
                    string hoursQuery = @"
                        SELECT ISNULL(SUM(DATEDIFF(HOUR, ClockInTime, ClockOutTime)), 0)
                        FROM TimeEntries 
                        WHERE EmployeeId = @EmployeeId 
                        AND DATEPART(WEEK, WorkDate) = DATEPART(WEEK, GETDATE())
                        AND YEAR(WorkDate) = YEAR(GETDATE())
                        AND ClockOutTime IS NOT NULL";

                    using (SqlCommand cmd = new SqlCommand(hoursQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        object result = cmd.ExecuteScalar();
                        litHoursWorked.Text = (result ?? 0).ToString();
                    }

                    // Get years of service
                    string serviceQuery = @"
                        SELECT DATEDIFF(YEAR, HireDate, GETDATE()) as YearsOfService
                        FROM Employees 
                        WHERE Id = @EmployeeId AND HireDate IS NOT NULL";

                    using (SqlCommand cmd = new SqlCommand(serviceQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        object result = cmd.ExecuteScalar();
                        litYearsOfService.Text = (result ?? 0).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                litLeaveBalance.Text = "0";
                litHoursWorked.Text = "0";
                litYearsOfService.Text = "0";
            }
        }

        private void LoadRecentActivity(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT TOP 10
                            ActivityType,
                            Description,
                            CreatedAt
                        FROM ActivityLogs 
                        WHERE UserId = @UserId 
                        ORDER BY CreatedAt DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        DataTable dt = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }

                        if (dt.Rows.Count > 0)
                        {
                            rptRecentActivity.DataSource = dt;
                            rptRecentActivity.DataBind();
                            pnlNoActivity.Visible = false;
                        }
                        else
                        {
                            pnlNoActivity.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                pnlNoActivity.Visible = true;
            }
        }

        #region Event Handlers

        protected void btnEditProfile_Click(object sender, EventArgs e)
        {
            pnlEditProfile.Visible = true;
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            pnlChangePassword.Visible = true;
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(hfCurrentUserId.Value);
                int employeeId = GetEmployeeIdFromUserId(userId);

                if (employeeId == 0)
                {
                    ShowMessage("Employee record not found.", "error");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string updateQuery = @"
                        UPDATE Employees 
                        SET FirstName = @FirstName,
                            LastName = @LastName,
                            PhoneNumber = @PhoneNumber,
                            DateOfBirth = @DateOfBirth,
                            Address = @Address,
                            City = @City,
                            State = @State,
                            ZipCode = @ZipCode,
                            UpdatedAt = GETDATE(),
                            LastUpdated = GETDATE()
                        WHERE Id = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", txtEditFirstName.Text.Trim());
                        cmd.Parameters.AddWithValue("@LastName", txtEditLastName.Text.Trim());
                        cmd.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrEmpty(txtEditPhone.Text.Trim()) ? (object)DBNull.Value : txtEditPhone.Text.Trim());
                        cmd.Parameters.AddWithValue("@Address", string.IsNullOrEmpty(txtEditAddress.Text.Trim()) ? (object)DBNull.Value : txtEditAddress.Text.Trim());
                        cmd.Parameters.AddWithValue("@City", string.IsNullOrEmpty(txtEditCity.Text.Trim()) ? (object)DBNull.Value : txtEditCity.Text.Trim());
                        cmd.Parameters.AddWithValue("@State", string.IsNullOrEmpty(txtEditState.Text.Trim()) ? (object)DBNull.Value : txtEditState.Text.Trim());
                        cmd.Parameters.AddWithValue("@ZipCode", string.IsNullOrEmpty(txtEditZipCode.Text.Trim()) ? (object)DBNull.Value : txtEditZipCode.Text.Trim());
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                        if (string.IsNullOrEmpty(txtEditDateOfBirth.Text))
                        {
                            cmd.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@DateOfBirth", DateTime.Parse(txtEditDateOfBirth.Text));
                        }

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Update session name if changed
                            string newName = $"{txtEditFirstName.Text.Trim()} {txtEditLastName.Text.Trim()}".Trim();
                            if (!string.IsNullOrEmpty(newName))
                            {
                                Session["UserName"] = newName;
                            }

                            // Log activity
                            LogActivity(userId, "Profile Updated", "Profile", "User updated their profile information");

                            ShowMessage("Profile updated successfully!", "success");
                            pnlEditProfile.Visible = false;
                            LoadProfileData(); // Refresh the display
                        }
                        else
                        {
                            ShowMessage("No changes were made to your profile.", "info");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error updating profile. Please try again.", "error");
            }
        }

        protected void btnSavePassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtCurrentPassword.Text) ||
                    string.IsNullOrEmpty(txtNewPassword.Text) ||
                    string.IsNullOrEmpty(txtConfirmPassword.Text))
                {
                    ShowMessage("All password fields are required.", "error");
                    return;
                }

                if (txtNewPassword.Text != txtConfirmPassword.Text)
                {
                    ShowMessage("New passwords do not match.", "error");
                    return;
                }

                if (txtNewPassword.Text.Length < 8)
                {
                    ShowMessage("New password must be at least 8 characters long.", "error");
                    return;
                }

                int userId = Convert.ToInt32(hfCurrentUserId.Value);

                // Verify current password
                if (!VerifyCurrentPassword(userId, txtCurrentPassword.Text))
                {
                    ShowMessage("Current password is incorrect.", "error");
                    return;
                }

                // Update password
                string hashedNewPassword = HashPassword(txtNewPassword.Text);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string updateQuery = @"
                        UPDATE Users 
                        SET PasswordHash = @PasswordHash,
                            PasswordChangedAt = GETDATE(),
                            MustChangePassword = 0
                        WHERE Id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@PasswordHash", hashedNewPassword);
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Log activity
                            LogActivity(userId, "Password Changed", "Security", "User changed their password");

                            ShowMessage("Password changed successfully!", "success");
                            pnlChangePassword.Visible = false;
                            ClearPasswordFields();
                            LoadProfileData(); // Refresh security info
                        }
                        else
                        {
                            ShowMessage("Error updating password. Please try again.", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error changing password. Please try again.", "error");
            }
        }

        protected void btnCloseEditModal_Click(object sender, EventArgs e)
        {
            pnlEditProfile.Visible = false;
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            pnlEditProfile.Visible = false;
        }

        protected void btnClosePasswordModal_Click(object sender, EventArgs e)
        {
            pnlChangePassword.Visible = false;
            ClearPasswordFields();
        }

        protected void btnCancelPassword_Click(object sender, EventArgs e)
        {
            pnlChangePassword.Visible = false;
            ClearPasswordFields();
        }

        #endregion

        #region Helper Methods

        private int GetEmployeeIdFromUserId(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT ISNULL(Id, 0) FROM Employees WHERE UserId = @UserId AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        private bool VerifyCurrentPassword(int userId, string currentPassword)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT PasswordHash FROM Users WHERE Id = @UserId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            string storedHash = result.ToString();
                            return VerifyPassword(currentPassword, storedHash);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return false;
        }

        private string HashPassword(string password)
        {
            // Simple SHA256 hash (in production, use BCrypt or similar)
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            string hashOfInput = HashPassword(password);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hash) == 0;
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return "??";

            string[] parts = fullName.Split(' ');
            if (parts.Length == 1)
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();

            return (parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1)).ToUpper();
        }

        protected string GetActivityIcon(string activityType)
        {
            switch (activityType?.ToLower())
            {
                case "login":
                    return "login";
                case "profile updated":
                    return "person";
                case "password changed":
                    return "lock";
                case "leave request":
                    return "event_available";
                case "time entry":
                    return "schedule";
                default:
                    return "info";
            }
        }

        protected string GetRelativeTime(object dateTime)
        {
            if (dateTime == null || dateTime == DBNull.Value)
                return "Unknown time";

            try
            {
                DateTime dt = Convert.ToDateTime(dateTime);
                TimeSpan timeDiff = DateTime.Now - dt;

                if (timeDiff.TotalMinutes < 1)
                    return "Just now";
                else if (timeDiff.TotalMinutes < 60)
                    return $"{(int)timeDiff.TotalMinutes} minutes ago";
                else if (timeDiff.TotalHours < 24)
                    return $"{(int)timeDiff.TotalHours} hours ago";
                else if (timeDiff.TotalDays < 30)
                    return $"{(int)timeDiff.TotalDays} days ago";
                else
                    return dt.ToString("MMM dd, yyyy");
            }
            catch
            {
                return "Unknown time";
            }
        }

        private void ClearPasswordFields()
        {
            txtCurrentPassword.Text = "";
            txtNewPassword.Text = "";
            txtConfirmPassword.Text = "";
        }

        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = $"message-text {type}";
            pnlMessages.CssClass = $"alert-panel alert-{type}";
            pnlMessages.Visible = true;
        }

        private void LogActivity(int userId, string activityType, string entityType, string description)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO ActivityLogs (UserId, ActivityType, EntityType, Description, CreatedAt)
                        VALUES (@UserId, @ActivityType, @EntityType, @Description, GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@ActivityType", activityType);
                        cmd.Parameters.AddWithValue("@EntityType", entityType);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Fail silently
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
                        INSERT INTO ErrorLogs (ErrorMessage, StackTrace, Source, Timestamp, UserId, Severity, CreatedAt)
                        VALUES (@ErrorMessage, @StackTrace, @Source, @Timestamp, @UserId, @Severity, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message);
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", "MyProfile.aspx");
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserId", Session["UserId"] ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Severity", "High");
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Fail silently to avoid recursive errors
            }
        }

        #endregion
    }
}