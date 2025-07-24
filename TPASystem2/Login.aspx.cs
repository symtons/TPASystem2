using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2
{
    public partial class Login : System.Web.UI.Page
    {

        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Clear any existing session
                Session.Clear();

                // Add client-side attributes for better UX
                txtEmail.Attributes.Add("autocomplete", "email");
                txtPassword.Attributes.Add("autocomplete", "current-password");

                // Set focus to email field
                Page.SetFocus(txtEmail);
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear previous error messages
                pnlError.Visible = false;
                litError.Text = "";

                // Get form data
                string email = txtEmail.Text.Trim().ToLower();
                string password = txtPassword.Text.Trim();

                // Basic server-side validation
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ShowError("Please enter both email and password.");
                    return;
                }

                if (!IsValidEmail(email))
                {
                    ShowError("Please enter a valid email address.");
                    return;
                }

                // Authenticate user directly with database
                UserInfo user = AuthenticateUser(email, password);

                if (user != null)
                {
                    // Authentication successful - Log the login
                    LogActivity(user.Id, "Login", "User", $"User {email} logged in successfully", GetClientIP());

                    // Set session variables
                    Session["UserId"] = user.Id;
                    Session["UserEmail"] = user.Email;
                    Session["UserRole"] = user.Role;
                    Session["UserName"] = user.Name;
                    Session["IsActive"] = user.IsActive;
                    Session["LoginTime"] = DateTime.Now;

                    // Update last login in database
                    UpdateLastLogin(user.Id);

                    // Redirect to dashboard
                    Response.Redirect("Dashboard.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    // Authentication failed - Log the attempt
                    LogFailedLogin(email, GetClientIP());
                    ShowError("Invalid email or password. Please check your credentials and try again.");

                    // Clear password field for security
                    txtPassword.Text = "";
                    Page.SetFocus(txtPassword);
                }
            }
            catch (Exception ex)
            {
                // Log error (in production, use proper logging like Serilog)
                System.Diagnostics.Debug.WriteLine($"Login Error: {ex.Message}");
                ShowError("An error occurred during login. Please try again later.");
            }
        }

        private UserInfo AuthenticateUser(string email, string password)
        {
            UserInfo user = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First check if user is locked out
                    if (IsUserLockedOut(email, conn))
                    {
                        ShowError("Account is temporarily locked due to multiple failed login attempts. Please try again later.");
                        return null;
                    }

                    // Get user credentials and info
                    string query = @"
                        SELECT u.Id, u.Email, u.PasswordHash, u.Salt, u.Role, u.IsActive, u.FailedLoginAttempts,
                               ISNULL(e.FirstName, '') + ' ' + ISNULL(e.LastName, '') as FullName
                        FROM Users u
                        LEFT JOIN Employees e ON u.Id = e.UserId
                        WHERE u.Email = @Email AND u.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedHash = reader["PasswordHash"].ToString();
                                string salt = reader["Salt"].ToString();

                                // Verify password using the same method as the API
                                if (VerifyPassword(password, salt, storedHash))
                                {
                                    user = new UserInfo
                                    {
                                        Id = Convert.ToInt32(reader["Id"]),
                                        Email = reader["Email"].ToString(),
                                        Role = reader["Role"].ToString(),
                                        Name = reader["FullName"].ToString().Trim(),
                                        IsActive = Convert.ToBoolean(reader["IsActive"])
                                    };

                                    // Reset failed login attempts on successful login
                                    ResetFailedLoginAttempts(user.Id, conn);
                                }
                                else
                                {
                                    // Increment failed login attempts
                                    IncrementFailedLoginAttempts(email, conn);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database Error in AuthenticateUser: {ex.Message}");
                throw;
            }

            return user;
        }

        private bool VerifyPassword(string password, string salt, string storedHash)
        {
            try
            {
                // Use SHA256 with salt (matching the API implementation)
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    string saltedPassword = password + salt;
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                    string computedHash = Convert.ToBase64String(bytes);
                    return computedHash == storedHash;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool IsUserLockedOut(string email, SqlConnection conn)
        {
            try
            {
                string query = @"
                    SELECT LockoutEnd, FailedLoginAttempts 
                    FROM Users 
                    WHERE Email = @Email";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var lockoutEnd = reader["LockoutEnd"] as DateTime?;
                            int failedAttempts = Convert.ToInt32(reader["FailedLoginAttempts"]);

                            // Check if user is currently locked out
                            if (lockoutEnd.HasValue && lockoutEnd.Value > DateTime.UtcNow)
                            {
                                return true;
                            }

                            // Auto-lockout after 5 failed attempts
                            if (failedAttempts >= 5)
                            {
                                // Set lockout for 15 minutes
                                SetUserLockout(email, DateTime.UtcNow.AddMinutes(15), conn);
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking lockout: {ex.Message}");
            }

            return false;
        }

        private void SetUserLockout(string email, DateTime lockoutEnd, SqlConnection conn)
        {
            try
            {
                string query = @"
                    UPDATE Users 
                    SET LockoutEnd = @LockoutEnd, UpdatedAt = @UpdatedAt 
                    WHERE Email = @Email";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@LockoutEnd", lockoutEnd);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting lockout: {ex.Message}");
            }
        }

        private void IncrementFailedLoginAttempts(string email, SqlConnection conn)
        {
            try
            {
                string query = @"
                    UPDATE Users 
                    SET FailedLoginAttempts = FailedLoginAttempts + 1, UpdatedAt = @UpdatedAt 
                    WHERE Email = @Email";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error incrementing failed attempts: {ex.Message}");
            }
        }

        private void ResetFailedLoginAttempts(int userId, SqlConnection conn)
        {
            try
            {
                string query = @"
                    UPDATE Users 
                    SET FailedLoginAttempts = 0, LockoutEnd = NULL, UpdatedAt = @UpdatedAt 
                    WHERE Id = @UserId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resetting failed attempts: {ex.Message}");
            }
        }

        private void UpdateLastLogin(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        UPDATE Users 
                        SET LastLogin = @LastLogin, UpdatedAt = @UpdatedAt 
                        WHERE Id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@LastLogin", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating last login: {ex.Message}");
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
                        INSERT INTO RecentActivities (UserId, ActivityTypeId, Action, EntityType, Details, IPAddress, CreatedAt)
                        VALUES (@UserId, 1, @Action, @EntityType, @Details, @IPAddress, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@EntityType", entityType);
                        cmd.Parameters.AddWithValue("@Details", details);
                        cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging activity: {ex.Message}");
            }
        }

        private void LogFailedLogin(string email, string ipAddress)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO RecentActivities (UserId, ActivityTypeId, Action, EntityType, Details, IPAddress, CreatedAt)
                        VALUES (0, 1, 'Failed Login', 'User', @Details, @IPAddress, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Details", $"Failed login attempt for email: {email}");
                        cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging failed login: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            litError.Text = message;
            pnlError.Visible = true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private string GetClientIP()
        {
            string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(ipAddress))
                ipAddress = Request.UserHostAddress;

            return ipAddress ?? "Unknown";
        }

       

        protected void Button1_Click(object sender, EventArgs e)
        {
            //try
            //{
                // Clear previous error messages
                pnlError.Visible = false;
                litError.Text = "";

                // Get form data
                string email = txtEmail.Text.Trim().ToLower();
                string password = txtPassword.Text.Trim();

                // Basic server-side validation
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ShowError("Please enter both email and password.");
                    return;
                }

                if (!IsValidEmail(email))
                {
                    ShowError("Please enter a valid email address.");
                    return;
                }

                // Authenticate user directly with database
                UserInfo user = AuthenticateUser(email, password);

                if (user != null)
                {
                    // Authentication successful - Log the login
                    LogActivity(user.Id, "Login", "User", $"User {email} logged in successfully", GetClientIP());

                    // Set session variables
                    Session["UserId"] = user.Id;
                    Session["UserEmail"] = user.Email;
                    Session["UserRole"] = user.Role;
                    Session["UserName"] = user.Name;
                    Session["IsActive"] = user.IsActive;
                    Session["LoginTime"] = DateTime.Now;

                    // Update last login in database
                    UpdateLastLogin(user.Id);

                    // Redirect to dashboard
                    Response.Redirect("~/Dashboard.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    // Authentication failed - Log the attempt
                    LogFailedLogin(email, GetClientIP());
                    ShowError("Invalid email or password. Please check your credentials and try again.");

                    // Clear password field for security
                    txtPassword.Text = "";
                    Page.SetFocus(txtPassword);
                }
            //}
            //catch (Exception ex)
            //{
            //    // Log error (in production, use proper logging like Serilog)
            //    System.Diagnostics.Debug.WriteLine($"Login Error: {ex.Message}");
            //    ShowError("An error occurred during login. Please try again later.");
            //}
        }
    }

    // User Info class
    public class UserInfo
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
