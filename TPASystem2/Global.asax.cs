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
        //private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //protected void Application_Start(object sender, EventArgs e)
        //{
        //    try
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
        //    catch (Exception ex)
        //    {
        //        // Log startup error
        //        System.Diagnostics.EventLog.WriteEntry("TPA HR System",
        //            $"Application Start Error: {ex.Message}",
        //            System.Diagnostics.EventLogEntryType.Error);
        //    }
        //}

        //private void RegisterRoutes(RouteCollection routes)
        //{
        //    try
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
        //            "Dashboard",
        //            "dashboard",
        //            "~/Dashboard.aspx"
        //        );

        //        // Employee Management Routes - Point directly to HR folder
        //        routes.MapPageRoute(
        //            "Employees",
        //            "employees",
        //            "~/HR/Employees.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "AddEmployee",
        //            "employees/add",
        //            "~/HR/AddEmployee.aspx"
        //        );

        //        routes.MapPageRoute(
        //            "EmployeeDetails",
        //            "employees/{id}",
        //            "~/HR/EmployeeDetails.aspx",
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
        //    catch (Exception ex)
        //    {
        //        // Log routing error
        //        System.Diagnostics.EventLog.WriteEntry("TPA HR System",
        //            $"Routing Registration Error: {ex.Message}",
        //            System.Diagnostics.EventLogEntryType.Error);
        //    }
        //}

        //protected void Session_Start(object sender, EventArgs e)
        //{
        //    // Session start logic
        //    Session.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SessionTimeoutMinutes"] ?? "480");

        //    // Track active sessions
        //    Application.Lock();
        //    Application["ActiveSessions"] = (int)Application["ActiveSessions"] + 1;
        //    Application.UnLock();

        //    // Log session start
        //    LogSessionEvent("Session Started", Session.SessionID, GetClientIP());
        //}

        //protected void Application_BeginRequest(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // Handle URL rewriting for clean URLs
        //        string url = Request.Url.AbsolutePath.ToLower();

        //        // Security headers
        //        Response.Headers.Add("X-Content-Type-Options", "nosniff");
        //        Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
        //        Response.Headers.Add("X-XSS-Protection", "1; mode=block");

        //        // Handle root requests
        //        if (url == "/" || url == "/default.aspx")
        //        {
        //            Response.Redirect("/login", false);
        //            Context.ApplicationInstance.CompleteRequest();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log request handling error
        //        System.Diagnostics.EventLog.WriteEntry("TPA HR System",
        //            $"Request Handling Error: {ex.Message}",
        //            System.Diagnostics.EventLogEntryType.Warning);
        //    }
        //}

        //protected void Application_AuthenticateRequest(object sender, EventArgs e)
        //{
        //    // Authentication logic
        //    if (User != null && User.Identity.IsAuthenticated)
        //    {
        //        // User is authenticated - could add role-based logic here
        //        ValidateUserSession();
        //    }
        //}

        //protected void Application_Error(object sender, EventArgs e)
        //{
        //    try
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
        //    catch (Exception)
        //    {
        //        // Prevent recursive errors
        //    }
        //}

        //protected void Session_End(object sender, EventArgs e)
        //{
        //    // Track active sessions
        //    Application.Lock();
        //    if ((int)Application["ActiveSessions"] > 0)
        //    {
        //        Application["ActiveSessions"] = (int)Application["ActiveSessions"] - 1;
        //    }
        //    Application.UnLock();

        //    // Log session end
        //    LogSessionEvent("Session Ended", Session.SessionID, GetClientIP());
        //}

        //protected void Application_End(object sender, EventArgs e)
        //{
        //    // Application shutdown logic
        //    LogApplicationEvent("Application Ended", "TPA HR System application has shut down");
        //}

        //#region Helper Methods

        //private void LogApplicationEvent(string eventType, string message)
        //{
        //    try
        //    {
        //        // Simple event log - avoid database dependency during startup
        //        System.Diagnostics.EventLog.WriteEntry("TPA HR System",
        //            $"{eventType}: {message}",
        //            System.Diagnostics.EventLogEntryType.Information);
        //    }
        //    catch (Exception)
        //    {
        //        // Silently fail logging to prevent application startup issues
        //    }
        //}

        //private void LogSessionEvent(string eventType, string sessionId, string clientIP)
        //{
        //    try
        //    {
        //        // Simple logging - enhance with database logging later if needed
        //        System.Diagnostics.EventLog.WriteEntry("TPA HR System",
        //            $"{eventType}: Session {sessionId} from {clientIP}",
        //            System.Diagnostics.EventLogEntryType.Information);
        //    }
        //    catch (Exception)
        //    {
        //        // Silently fail session logging to prevent application crashes
        //    }
        //}

        //private void LogError(Exception ex)
        //{
        //    try
        //    {
        //        string errorDetails = $"Error: {ex.Message}\nURL: {Request.Url}\nStack: {ex.StackTrace}";
        //        System.Diagnostics.EventLog.WriteEntry("TPA HR System",
        //            errorDetails,
        //            System.Diagnostics.EventLogEntryType.Error);
        //    }
        //    catch (Exception)
        //    {
        //        // Silently fail error logging to prevent recursive errors
        //    }
        //}

        //private void ValidateUserSession()
        //{
        //    try
        //    {
        //        // Add session validation logic here if needed
        //        // For now, just ensure basic session handling
        //    }
        //    catch (Exception)
        //    {
        //        // Silently handle session validation errors
        //    }
        //}

        //private string GetClientIP()
        //{
        //    string ipAddress = "";
        //    try
        //    {
        //        ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        //        if (string.IsNullOrEmpty(ipAddress) || ipAddress.ToLower() == "unknown")
        //        {
        //            ipAddress = Request.ServerVariables["REMOTE_ADDR"];
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        ipAddress = "Unknown";
        //    }
        //    return ipAddress;
        //}

       // #endregion
    }
}