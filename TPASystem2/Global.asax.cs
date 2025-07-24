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
        //    private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //    protected void Application_Start(object sender, EventArgs e)
        //    {
        //        // Register URL routes for clean URLs
        //        RegisterRoutes(RouteTable.Routes);

        //        // Application startup logic
        //        LogApplicationEvent("Application Started", "TPA HR System application has started successfully");

        //        // Initialize application-level variables
        //        Application["ApplicationName"] = ConfigurationManager.AppSettings["ApplicationName"] ?? "TPA HR System";
        //        Application["ApplicationVersion"] = ConfigurationManager.AppSettings["ApplicationVersion"] ?? "1.0.0";
        //        Application["StartTime"] = DateTime.Now;
        //        Application["ActiveSessions"] = 0;
        //    }

        //    private void RegisterRoutes(RouteCollection routes)
        //    {
        //        // Ignore static resources
        //        routes.RouteExistingFiles = false;

        //        // Define clean URL routes
        //        routes.MapPageRoute(
        //            "Login",
        //            "login",
        //            "~/Login.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "TestLogin",
        //            "test",
        //            "~/TestLogin.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "Dashboard",
        //            "dashboard",
        //            "~/Dashboard.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "Employees",
        //            "employees",
        //            "~/Employees.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "EmployeeDetails",
        //            "employees/{id}",
        //            "~/EmployeeDetails.aspx",
        //            false,
        //            new RouteValueDictionary { { "id", @"\d+" } }
        //        );

        //        routes.MapPageRoute(
        //            "TimeAttendance",
        //            "time-attendance",
        //            "~/TimeAttendance.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "LeaveManagement",
        //            "leave-management",
        //            "~/LeaveManagement.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "Reports",
        //            "reports",
        //            "~/Reports.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "Profile",
        //            "profile",
        //            "~/Profile.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "Settings",
        //            "settings",
        //            "~/Settings.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "Error",
        //            "error",
        //            "~/Error.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "NotFound",
        //            "notfound",
        //            "~/NotFound.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "Help",
        //            "help",
        //            "~/Help.aspx"
        //        );

        //        // Admin routes
        //        routes.MapPageRoute(
        //            "AdminUsers",
        //            "admin/users",
        //            "~/Admin/Users.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "AdminReports",
        //            "admin/reports",
        //            "~/Admin/Reports.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "AdminSettings",
        //            "admin/settings",
        //            "~/Admin/Settings.aspx"
        //        );
        //    }

        //    protected void Session_Start(object sender, EventArgs e)
        //    {
        //        // Session start logic
        //        Session.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SessionTimeoutMinutes"] ?? "480");

        //        // Track active sessions
        //        Application.Lock();
        //        Application["ActiveSessions"] = (int)Application["ActiveSessions"] + 1;
        //        Application.UnLock();

        //        // Log session start
        //        LogSessionEvent("Session Started", Session.SessionID, GetClientIP());
        //    }

        //    protected void Application_BeginRequest(object sender, EventArgs e)
        //    {
        //        // Handle URL rewriting for clean URLs
        //        string url = Request.Url.AbsolutePath.ToLower();

        //        // Security headers
        //        Response.Headers.Add("X-Content-Type-Options", "nosniff");
        //        Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
        //        Response.Headers.Add("X-XSS-Protection", "1; mode=block");

        //        // Prevent direct access to .aspx files if URL rewrite is not handling it
        //        if (url.EndsWith(".aspx") && !Request.QueryString.ToString().Contains("direct=true"))
        //        {
        //            string cleanUrl = url.Replace(".aspx", "");
        //            Response.Redirect(cleanUrl, true);
        //        }

        //        // Handle root requests
        //        if (url == "/" || url == "/default.aspx")
        //        {
        //            Response.Redirect("/login", false);
        //            Context.ApplicationInstance.CompleteRequest();
        //        }

        //        // Force HTTPS in production (uncomment when needed)
        //        /*
        //        if (!Request.IsSecureConnection && !Request.IsLocal)
        //        {
        //            string redirectURL = Request.Url.ToString().Replace("http:", "https:");
        //            Response.Redirect(redirectURL, false);
        //            HttpContext.Current.ApplicationInstance.CompleteRequest();
        //        }
        //        */
        //    }

        //    protected void Application_AuthenticateRequest(object sender, EventArgs e)
        //    {
        //        // Authentication logic
        //        if (User != null && User.Identity.IsAuthenticated)
        //        {
        //            // User is authenticated - could add role-based logic here
        //            ValidateUserSession();
        //        }
        //    }

        //    protected void Application_Error(object sender, EventArgs e)
        //    {
        //        // Global error handling
        //        Exception ex = Server.GetLastError();

        //        if (ex != null)
        //        {
        //            // Log the error
        //            LogError(ex);

        //            // Handle specific error types
        //            if (ex is HttpException httpEx)
        //            {
        //                switch (httpEx.GetHttpCode())
        //                {
        //                    case 404:
        //                        Server.ClearError();
        //                        Response.Redirect("/notfound", false);
        //                        break;
        //                    case 500:
        //                        Server.ClearError();
        //                        Response.Redirect("/error", false);
        //                        break;
        //                }
        //            }
        //            else
        //            {
        //                // For other exceptions, redirect to error page
        //                Server.ClearError();
        //                Response.Redirect("/error", false);
        //            }
        //        }
        //    }

        //    protected void Session_End(object sender, EventArgs e)
        //    {
        //        // Session end logic

        //        // Track active sessions
        //        Application.Lock();
        //        int activeSessions = (int)Application["ActiveSessions"];
        //        if (activeSessions > 0)
        //        {
        //            Application["ActiveSessions"] = activeSessions - 1;
        //        }
        //        Application.UnLock();

        //        // Log session end
        //        LogSessionEvent("Session Ended", Session.SessionID, "Session Timeout");

        //        // Clear any session-specific resources
        //        if (Session["UserId"] != null)
        //        {
        //            LogUserActivity((int)Session["UserId"], "Logout", "Session", "Session expired/ended", GetClientIP());
        //        }
        //    }

        //    protected void Application_End(object sender, EventArgs e)
        //    {
        //        // Application shutdown logic
        //        LogApplicationEvent("Application Ended", "TPA HR System application has shut down");
        //    }

        //    #region Helper Methods

        //    private void ValidateUserSession()
        //    {
        //        try
        //        {
        //            // Check if session is still valid
        //            if (Session["UserId"] != null && Session["LoginTime"] != null)
        //            {
        //                DateTime loginTime = (DateTime)Session["LoginTime"];
        //                int sessionTimeoutMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["SessionTimeoutMinutes"] ?? "480");

        //                if (DateTime.Now.Subtract(loginTime).TotalMinutes > sessionTimeoutMinutes)
        //                {
        //                    // Session has expired
        //                    LogUserActivity((int)Session["UserId"], "Session Timeout", "User", "Session expired due to inactivity", GetClientIP());
        //                    Session.Clear();
        //                    FormsAuthentication.SignOut();
        //                    Response.Redirect("/login?expired=true", false);
        //                    HttpContext.Current.ApplicationInstance.CompleteRequest();
        //                }
        //                else
        //                {
        //                    // Update last activity time
        //                    Session["LastActivity"] = DateTime.Now;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Log validation error but don't interrupt the request
        //            LogError(ex, "Session validation error");
        //        }
        //    }

        //    private void LogApplicationEvent(string action, string details)
        //    {
        //        try
        //        {
        //            using (SqlConnection conn = new SqlConnection(connectionString))
        //            {
        //                conn.Open();
        //                string query = @"
        //                    INSERT INTO RecentActivities (UserId, ActivityTypeId, Action, EntityType, Details, IPAddress, CreatedAt)
        //                    VALUES (0, 1, @Action, 'Application', @Details, @IPAddress, @CreatedAt)";

        //                using (SqlCommand cmd = new SqlCommand(query, conn))
        //                {
        //                    cmd.Parameters.AddWithValue("@Action", action);
        //                    cmd.Parameters.AddWithValue("@Details", details);
        //                    cmd.Parameters.AddWithValue("@IPAddress", GetClientIP());
        //                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
        //                    cmd.ExecuteNonQuery();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // If logging fails, write to event log or debug output
        //            System.Diagnostics.Debug.WriteLine($"Failed to log application event: {ex.Message}");
        //        }
        //    }

        //    private void LogSessionEvent(string action, string sessionId, string details)
        //    {
        //        try
        //        {
        //            using (SqlConnection conn = new SqlConnection(connectionString))
        //            {
        //                conn.Open();
        //                string query = @"
        //                    INSERT INTO RecentActivities (UserId, ActivityTypeId, Action, EntityType, Details, IPAddress, CreatedAt)
        //                    VALUES (0, 1, @Action, 'Session', @Details, @IPAddress, @CreatedAt)";

        //                using (SqlCommand cmd = new SqlCommand(query, conn))
        //                {
        //                    cmd.Parameters.AddWithValue("@Action", action);
        //                    cmd.Parameters.AddWithValue("@Details", $"Session {sessionId}: {details}");
        //                    cmd.Parameters.AddWithValue("@IPAddress", GetClientIP());
        //                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
        //                    cmd.ExecuteNonQuery();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Diagnostics.Debug.WriteLine($"Failed to log session event: {ex.Message}");
        //        }
        //    }

        //    private void LogUserActivity(int userId, string action, string entityType, string details, string ipAddress)
        //    {
        //        try
        //        {
        //            using (SqlConnection conn = new SqlConnection(connectionString))
        //            {
        //                conn.Open();
        //                string query = @"
        //                    INSERT INTO RecentActivities (UserId, ActivityTypeId, Action, EntityType, Details, IPAddress, CreatedAt)
        //                    VALUES (@UserId, 1, @Action, @EntityType, @Details, @IPAddress, @CreatedAt)";

        //                using (SqlCommand cmd = new SqlCommand(query, conn))
        //                {
        //                    cmd.Parameters.AddWithValue("@UserId", userId);
        //                    cmd.Parameters.AddWithValue("@Action", action);
        //                    cmd.Parameters.AddWithValue("@EntityType", entityType);
        //                    cmd.Parameters.AddWithValue("@Details", details);
        //                    cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
        //                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
        //                    cmd.ExecuteNonQuery();
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Diagnostics.Debug.WriteLine($"Failed to log user activity: {ex.Message}");
        //        }
        //    }

        //    private void LogError(Exception ex, string context = "")
        //    {
        //        try
        //        {
        //            string errorDetails = $"{context} - Exception: {ex.Message}";
        //            if (ex.InnerException != null)
        //            {
        //                errorDetails += $" | Inner Exception: {ex.InnerException.Message}";
        //            }
        //            errorDetails += $" | Stack Trace: {ex.StackTrace}";

        //            using (SqlConnection conn = new SqlConnection(connectionString))
        //            {
        //                conn.Open();
        //                string query = @"
        //                    INSERT INTO RecentActivities (UserId, ActivityTypeId, Action, EntityType, Details, IPAddress, CreatedAt)
        //                    VALUES (0, 1, 'Error', 'Application', @Details, @IPAddress, @CreatedAt)";

        //                using (SqlCommand cmd = new SqlCommand(query, conn))
        //                {
        //                    cmd.Parameters.AddWithValue("@Details", errorDetails.Length > 1000 ? errorDetails.Substring(0, 1000) : errorDetails);
        //                    cmd.Parameters.AddWithValue("@IPAddress", GetClientIP());
        //                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
        //                    cmd.ExecuteNonQuery();
        //                }
        //            }

        //            // Also log to debug output for development
        //            System.Diagnostics.Debug.WriteLine($"Error logged: {errorDetails}");
        //        }
        //        catch (Exception logEx)
        //        {
        //            // If logging to database fails, at least log to debug output
        //            System.Diagnostics.Debug.WriteLine($"Failed to log error to database: {logEx.Message}");
        //            System.Diagnostics.Debug.WriteLine($"Original error: {ex.Message}");
        //        }
        //    }

        //    private string GetClientIP()
        //    {
        //        try
        //        {
        //            string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //            if (string.IsNullOrEmpty(ipAddress))
        //                ipAddress = Request.ServerVariables["REMOTE_ADDR"];
        //            if (string.IsNullOrEmpty(ipAddress))
        //                ipAddress = Request.UserHostAddress;

        //            return ipAddress ?? "Unknown";
        //        }
        //        catch
        //        {
        //            return "Unknown";
        //        }
        //    }

        //    #endregion

        //    #region Custom Application Methods

        //    /// <summary>
        //    /// Get current application statistics
        //    /// </summary>
        //    public static ApplicationStats GetApplicationStats()
        //    {
        //        try
        //        {
        //            return new ApplicationStats
        //            {
        //                ApplicationName = HttpContext.Current.Application["ApplicationName"]?.ToString() ?? "TPA HR System",
        //                Version = HttpContext.Current.Application["ApplicationVersion"]?.ToString() ?? "1.0.0",
        //                StartTime = (DateTime)(HttpContext.Current.Application["StartTime"] ?? DateTime.Now),
        //                ActiveSessions = (int)(HttpContext.Current.Application["ActiveSessions"] ?? 0),
        //                Uptime = DateTime.Now.Subtract((DateTime)(HttpContext.Current.Application["StartTime"] ?? DateTime.Now))
        //            };
        //        }
        //        catch
        //        {
        //            return new ApplicationStats
        //            {
        //                ApplicationName = "TPA HR System",
        //                Version = "1.0.0",
        //                StartTime = DateTime.Now,
        //                ActiveSessions = 0,
        //                Uptime = TimeSpan.Zero
        //            };
        //        }
        //    }

        //    /// <summary>
        //    /// Force user logout by clearing session
        //    /// </summary>
        //    public static void ForceUserLogout(string reason = "Administrative action")
        //    {
        //        try
        //        {
        //            var context = HttpContext.Current;
        //            if (context?.Session != null)
        //            {
        //                if (context.Session["UserId"] != null)
        //                {
        //                    // Log the forced logout
        //                    var global = new Global();
        //                    global.LogUserActivity(
        //                        (int)context.Session["UserId"],
        //                        "Forced Logout",
        //                        "User",
        //                        $"User session terminated: {reason}",
        //                        global.GetClientIP()
        //                    );
        //                }

        //                context.Session.Clear();
        //                context.Session.Abandon();
        //                FormsAuthentication.SignOut();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            System.Diagnostics.Debug.WriteLine($"Error in ForceUserLogout: {ex.Message}");
        //        }
        //    }

        //    #endregion
        //}

        //#region Support Classes

        ///// <summary>
        ///// Application statistics class
        ///// </summary>
        //public class ApplicationStats
        //{
        //    public string ApplicationName { get; set; }
        //    public string Version { get; set; }
        //    public DateTime StartTime { get; set; }
        //    public int ActiveSessions { get; set; }
        //    public TimeSpan Uptime { get; set; }
        //}

        //#endregion  
    }
}