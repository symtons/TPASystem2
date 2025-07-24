using System;
using System.Web;

namespace TPASystem2.Helpers
{
    /// <summary>
    /// Simple URL helper for Web Forms without MVC dependencies
    /// </summary>
    public static class SimpleUrlHelper
    {
        /// <summary>
        /// Generate a clean URL for a page
        /// </summary>
        /// <param name="pageName">The page name without extension</param>
        /// <returns>Clean URL</returns>
        public static string GetCleanUrl(string pageName)
        {
            if (string.IsNullOrEmpty(pageName))
                return "/";

            return $"/{pageName.ToLower()}";
        }

        /// <summary>
        /// Generate a clean URL with query parameters
        /// </summary>
        /// <param name="pageName">The page name without extension</param>
        /// <param name="queryString">Query string (without ?)</param>
        /// <returns>Clean URL with parameters</returns>
        public static string GetCleanUrl(string pageName, string queryString)
        {
            string url = GetCleanUrl(pageName);

            if (!string.IsNullOrEmpty(queryString))
            {
                if (!queryString.StartsWith("?"))
                    queryString = "?" + queryString;
                url += queryString;
            }

            return url;
        }

        /// <summary>
        /// Generate URL with ID parameter (for details pages)
        /// </summary>
        /// <param name="pageName">Page name</param>
        /// <param name="id">ID parameter</param>
        /// <returns>Clean URL with ID</returns>
        public static string GetDetailsUrl(string pageName, int id)
        {
            return $"/{pageName.ToLower()}/{id}";
        }

        /// <summary>
        /// Redirect to a clean URL
        /// </summary>
        /// <param name="pageName">Page name without extension</param>
        /// <param name="endResponse">Whether to end the response</param>
        public static void RedirectToCleanUrl(string pageName, bool endResponse = true)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                string cleanUrl = GetCleanUrl(pageName);
                context.Response.Redirect(cleanUrl, endResponse);
            }
        }

        /// <summary>
        /// Redirect to a clean URL with query string
        /// </summary>
        /// <param name="pageName">Page name without extension</param>
        /// <param name="queryString">Query string</param>
        /// <param name="endResponse">Whether to end the response</param>
        public static void RedirectToCleanUrl(string pageName, string queryString, bool endResponse = true)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                string cleanUrl = GetCleanUrl(pageName, queryString);
                context.Response.Redirect(cleanUrl, endResponse);
            }
        }

        /// <summary>
        /// Get the current page name without extension
        /// </summary>
        /// <returns>Page name</returns>
        public static string GetCurrentPageName()
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                // Get the actual executing file path (the .aspx file)
                string executionPath = context.Request.CurrentExecutionFilePath;
                string fileName = System.IO.Path.GetFileNameWithoutExtension(executionPath);
                return fileName?.ToLower() ?? "";
            }
            return "";
        }

        /// <summary>
        /// Get the current clean URL path
        /// </summary>
        /// <returns>Clean URL path</returns>
        public static string GetCurrentCleanUrl()
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                // Get the requested path (what user sees in browser)
                string requestPath = context.Request.Url.AbsolutePath;
                return requestPath.ToLower();
            }
            return "/";
        }

        /// <summary>
        /// Check if current request is for a specific page
        /// </summary>
        /// <param name="pageName">Page name to check</param>
        /// <returns>True if current page matches</returns>
        public static bool IsCurrentPage(string pageName)
        {
            return GetCurrentPageName().Equals(pageName.ToLower(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get absolute URL for a page
        /// </summary>
        /// <param name="pageName">Page name</param>
        /// <returns>Absolute URL</returns>
        public static string GetAbsoluteUrl(string pageName)
        {
            var context = HttpContext.Current;
            if (context != null)
            {
                string baseUrl = $"{context.Request.Url.Scheme}://{context.Request.Url.Authority}";
                return baseUrl + GetCleanUrl(pageName);
            }
            return GetCleanUrl(pageName);
        }

        /// <summary>
        /// Generate navigation HTML for common TPA pages
        /// </summary>
        /// <param name="userRole">User role for access control</param>
        /// <returns>Navigation HTML</returns>
        public static string GenerateNavigation(string userRole = "")
        {
            var html = new System.Text.StringBuilder();
            var currentPage = GetCurrentPageName();

            html.AppendLine("<nav class=\"tpa-navigation\">");
            html.AppendLine("  <ul class=\"nav-list\">");

            // Dashboard (everyone)
            AddNavItem(html, "Dashboard", "dashboard", "dashboard", currentPage);

            // Time & Attendance (everyone)
            AddNavItem(html, "Time & Attendance", "time-attendance", "schedule", currentPage);

            // Employees (Admin, HR, Manager only)
            if (HasAccess(userRole, "Admin", "HR", "Manager"))
            {
                AddNavItem(html, "Employees", "employees", "people", currentPage);
            }

            // Leave Management (everyone)
            AddNavItem(html, "Leave Management", "leave-management", "event_available", currentPage);

            // Reports (Admin, HR, Manager only)
            if (HasAccess(userRole, "Admin", "HR", "Manager"))
            {
                AddNavItem(html, "Reports", "reports", "assessment", currentPage);
            }

            // Profile (everyone)
            AddNavItem(html, "Profile", "profile", "person", currentPage);

            // Settings (Admin, HR only)
            if (HasAccess(userRole, "Admin", "HR"))
            {
                AddNavItem(html, "Settings", "settings", "settings", currentPage);
            }

            // Help (everyone)
            AddNavItem(html, "Help", "help", "help", currentPage);

            html.AppendLine("  </ul>");
            html.AppendLine("</nav>");

            return html.ToString();
        }

        /// <summary>
        /// Generate breadcrumbs HTML
        /// </summary>
        /// <param name="items">Breadcrumb items in format "Title|PageName"</param>
        /// <returns>Breadcrumb HTML</returns>
        public static string GenerateBreadcrumbs(params string[] items)
        {
            if (items == null || items.Length == 0)
                return "";

            var html = new System.Text.StringBuilder();

            html.AppendLine("<nav class=\"breadcrumb-nav\">");
            html.AppendLine("  <ol class=\"breadcrumb\">");

            for (int i = 0; i < items.Length; i++)
            {
                string[] parts = items[i].Split('|');
                string title = parts[0];
                string pageName = parts.Length > 1 ? parts[1] : null;
                bool isLast = i == items.Length - 1;

                html.AppendLine("    <li class=\"breadcrumb-item" + (isLast ? " active" : "") + "\">");

                if (isLast || string.IsNullOrEmpty(pageName))
                {
                    html.AppendLine($"      <span>{title}</span>");
                }
                else
                {
                    string url = GetCleanUrl(pageName);
                    html.AppendLine($"      <a href=\"{url}\">{title}</a>");
                }

                html.AppendLine("    </li>");
            }

            html.AppendLine("  </ol>");
            html.AppendLine("</nav>");

            return html.ToString();
        }

        #region Private Helper Methods

        private static void AddNavItem(System.Text.StringBuilder html, string title, string pageName, string icon, string currentPage)
        {
            string activeClass = currentPage == pageName ? " active" : "";
            string url = GetCleanUrl(pageName);

            html.AppendLine($"    <li class=\"nav-item{activeClass}\">");
            html.AppendLine($"      <a href=\"{url}\" class=\"nav-link\">");
            html.AppendLine($"        <i class=\"material-icons\">{icon}</i>");
            html.AppendLine($"        <span>{title}</span>");
            html.AppendLine("      </a>");
            html.AppendLine("    </li>");
        }

        private static bool HasAccess(string userRole, params string[] allowedRoles)
        {
            if (string.IsNullOrEmpty(userRole))
                return false;

            foreach (string role in allowedRoles)
            {
                if (userRole.Equals(role, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        #endregion

        #region Common TPA URLs

        /// <summary>
        /// Common TPA application URLs
        /// </summary>
        public static class TpaUrls
        {
            public static string Login => GetCleanUrl("login");
            public static string Dashboard => GetCleanUrl("dashboard");
            public static string Employees => GetCleanUrl("employees");
            public static string TimeAttendance => GetCleanUrl("time-attendance");
            public static string LeaveManagement => GetCleanUrl("leave-management");
            public static string Reports => GetCleanUrl("reports");
            public static string Profile => GetCleanUrl("profile");
            public static string Settings => GetCleanUrl("settings");
            public static string Help => GetCleanUrl("help");
            public static string Error => GetCleanUrl("error");
            public static string NotFound => GetCleanUrl("notfound");
            public static string Test => GetCleanUrl("test");

            public static string EmployeeDetails(int id) => GetDetailsUrl("employees", id);
        }

        #endregion
    }
}