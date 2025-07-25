using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace TPASystem2
{
    public class Global : System.Web.HttpApplication
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Application_Start(object sender, EventArgs e)
        {
            // Register URL routes for clean URLs
            RegisterRoutes(RouteTable.Routes);

            // Application startup logic
            LogApplicationEvent("Application Started", "TPA HR System application has started successfully");

            // Initialize application-level variables
            Application["ApplicationName"] = ConfigurationManager.AppSettings["ApplicationName"] ?? "TPA HR System";
            Application["ApplicationVersion"] = ConfigurationManager.AppSettings["ApplicationVersion"] ?? "1.0.0";
            Application["StartTime"] = DateTime.Now;
            Application["ActiveSessions"] = 0;
        }

        private void RegisterRoutes(RouteCollection routes)
        {
            // Ignore static resources
            routes.RouteExistingFiles = false;

            // Define clean URL routes
            routes.MapPageRoute(
                "Login",
                "login",
                "~/Login.aspx"
            );

            routes.MapPageRoute(
                "TestLogin",
                "test",
                "~/TestLogin.aspx"
            );

            routes.MapPageRoute(
                "Dashboard",
                "dashboard",
                "~/Dashboard.aspx"
            );

            // Fix: Point employees route to HR folder
            routes.MapPageRoute(
                "Employees",
                "employees",
                "~/HR/Employees.aspx"
            );

            routes.MapPageRoute(
                "AddEmployee",
                "employees/add",
                "~/HR/AddEmployee.aspx"
            );

            routes.MapPageRoute(
                "EmployeeDetails",
                "employees/{id}",
                "~/HR/EmployeeDetails.aspx",
                false,
                new RouteValueDictionary { { "id", @"\d+" } }
            );

            routes.MapPageRoute(
                "TimeAttendance",
                "time-attendance",
                "~/TimeAttendance.aspx"
            );

            routes.MapPageRoute(
                "LeaveManagement",
                "leave-management",
                "~/LeaveManagement.aspx"
            );

            routes.MapPageRoute(
                "Reports",
                "reports",
                "~/Reports.aspx"
            );

            routes.MapPageRoute(
                "Profile",
                "profile",
                "~/Profile.aspx"
            );

            routes.MapPageRoute(
                "Settings",
                "settings",
                "~/Settings.aspx"
            );

            routes.MapPageRoute(
                "Error",
                "error",
                "~/Error.aspx"
            );

            routes.MapPageRoute(
                "NotFound",
                "notfound",
                "~/NotFound.aspx"
            );

            routes.MapPageRoute(
                "Help",
                "help",
                "~/Help.aspx"
            );

            // Admin routes
            routes.MapPageRoute(
                "AdminUsers",
                "admin/users",
                "~/Admin/Users.aspx"
            );

            routes.MapPageRoute(
                "AdminReports",
                "admin/reports",
                "~/Admin/Reports.aspx"
            );

            routes.MapPageRoute(
                "AdminSettings",
                "admin/settings",
                "~/Admin/Settings.aspx"
            );
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Session start logic
            Session.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SessionTimeoutMinutes"] ?? "480");

            // Track active sessions
            Application.Lock();
            Application["ActiveSessions"] = (int)Application["ActiveSessions"] + 1;
            Application.UnLock();

            // Log session start
            LogSessionEvent("Session Started", Session.SessionID, GetClientIP());
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Handle URL rewriting for clean URLs
            string url = Request.Url.AbsolutePath.ToLower();

            // Security headers
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            Response.Headers.Add("X-XSS-Protection", "1; mode=block");

            // Handle root requests
            if (url == "/" || url == "/default.aspx")
            {
                Response.Redirect("/login", false);
                Context.ApplicationInstance.CompleteRequest();
            }

            // Force HTTPS in production (uncomment when needed)
            /*
            if (!Request.IsSecureConnection && !Request.IsLocal)
            {
                string redirectURL = Request.Url.ToString().Replace("http:", "https:");
                Response.Redirect(redirectURL, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            */
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // Authentication logic
            if (User != null && User.Identity.IsAuthenticated)
            {
                // User is authenticated - could add role-based logic here
                ValidateUserSession();
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Global error handling
            Exception ex = Server.GetLastError();

            if (ex != null)
            {
                // Log the error
                LogError(ex);

                // Handle specific error types
                if (ex is HttpException httpEx)
                {
                    switch (httpEx.GetHttpCode())
                    {
                        case 404:
                            Server.ClearError();
                            Response.Redirect("/notfound", false);
                            break;
                        case 500:
                            Server.ClearError();
                            Response.Redirect("/error", false);
                            break;
                    }
                }
                else
                {
                    // For other exceptions, redirect to error page
                    Server.ClearError();
                    Response.Redirect("/error", false);
                }
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // Track active sessions
            Application.Lock();
            if ((int)Application["ActiveSessions"] > 0)
            {
                Application["ActiveSessions"] = (int)Application["ActiveSessions"] - 1;
            }
            Application.UnLock();

            // Log session end
            LogSessionEvent("Session Ended", Session.SessionID, GetClientIP());
        }

        protected void Application_End(object sender, EventArgs e)
        {
            // Application shutdown logic
            LogApplicationEvent("Application Ended", "TPA HR System application has shut down");
        }

        #region Helper Methods

        private void LogApplicationEvent(string eventType, string message)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ApplicationLogs (EventType, Message, Timestamp, Source, Level)
                        VALUES (@EventType, @Message, @Timestamp, @Source, @Level)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EventType", eventType);
                        cmd.Parameters.AddWithValue("@Message", message);
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@Source", "Global.asax");
                        cmd.Parameters.AddWithValue("@Level", "Info");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback logging if database is not available
                System.Diagnostics.EventLog.WriteEntry("TPA HR System",
                    $"{eventType}: {message} | Error: {ex.Message}",
                    System.Diagnostics.EventLogEntryType.Information);
            }
        }

        private void LogSessionEvent(string eventType, string sessionId, string clientIP)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO SessionLogs (EventType, SessionId, IPAddress, Timestamp, UserAgent)
                        VALUES (@EventType, @SessionId, @IPAddress, @Timestamp, @UserAgent)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EventType", eventType);
                        cmd.Parameters.AddWithValue("@SessionId", sessionId);
                        cmd.Parameters.AddWithValue("@IPAddress", clientIP);
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@UserAgent", Request.UserAgent ?? "Unknown");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                // Silently fail session logging to prevent application crashes
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
                        INSERT INTO ErrorLogs (ErrorMessage, StackTrace, Source, Timestamp, RequestUrl, UserAgent, IPAddress)
                        VALUES (@ErrorMessage, @StackTrace, @Source, @Timestamp, @RequestUrl, @UserAgent, @IPAddress)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message);
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", ex.Source ?? "Unknown");
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@RequestUrl", Request.Url?.ToString() ?? "");
                        cmd.Parameters.AddWithValue("@UserAgent", Request.UserAgent ?? "");
                        cmd.Parameters.AddWithValue("@IPAddress", GetClientIP());
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                // Silently fail error logging to prevent recursive errors
            }
        }

        private void ValidateUserSession()
        {
            try
            {
                // Add session validation logic here
                if (Session["UserId"] == null && !Request.Url.AbsolutePath.ToLower().Contains("login"))
                {
                    Response.Redirect("/login", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception)
            {
                // Silently handle session validation errors
            }
        }

        private string GetClientIP()
        {
            string ipAddress = "";
            try
            {
                ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown")
                {
                    ipAddress = Request.ServerVariables["REMOTE_ADDR"];
                }
            }
            catch (Exception)
            {
                ipAddress = "Unknown";
            }
            return ipAddress;
        }

        #endregion
    }
}