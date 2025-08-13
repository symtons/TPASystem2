using System;
using System.Web;

namespace TPASystem2.Helpers
{
    /// <summary>
    /// Helper class for managing clean URLs and navigation in the TPA System
    /// </summary>
    public static class SimpleUrlHelper
    {
        /// <summary>
        /// Redirects to a clean URL without .aspx extension
        /// </summary>
        /// <param name="page">The page name without extension</param>
        public static void RedirectToCleanUrl(string page)
        {
            if (HttpContext.Current?.Response != null)
            {
                string cleanUrl = $"/{page}";
                HttpContext.Current.Response.Redirect(cleanUrl, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }

        /// <summary>
        /// Redirects to a clean URL with parameters
        /// </summary>
        /// <param name="page">The page name without extension</param>
        /// <param name="parameters">Query string parameters</param>
        public static void RedirectToCleanUrl(string page, string parameters)
        {
            if (HttpContext.Current?.Response != null)
            {
                string cleanUrl = $"/{page}";
                if (!string.IsNullOrEmpty(parameters))
                {
                    cleanUrl += $"?{parameters}";
                }
                HttpContext.Current.Response.Redirect(cleanUrl, false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }

        /// <summary>
        /// Gets a clean URL for a page
        /// </summary>
        /// <param name="page">The page name without extension</param>
        /// <returns>Clean URL</returns>
        public static string GetCleanUrl(string page)
        {
            return $"/{page}";
        }

        /// <summary>
        /// Gets a clean URL for a page with parameters
        /// </summary>
        /// <param name="page">The page name without extension</param>
        /// <param name="parameters">Query string parameters</param>
        /// <returns>Clean URL with parameters</returns>
        public static string GetCleanUrl(string page, string parameters)
        {
            string cleanUrl = $"/{page}";
            if (!string.IsNullOrEmpty(parameters))
            {
                cleanUrl += $"?{parameters}";
            }
            return cleanUrl;
        }

        /// <summary>
        /// Builds a clean URL for navigation links
        /// </summary>
        /// <param name="page">The page name</param>
        /// <param name="id">Optional ID parameter</param>
        /// <returns>Clean URL</returns>
        public static string BuildUrl(string page, int? id = null)
        {
            string url = $"/{page}";
            if (id.HasValue)
            {
                url += $"/{id.Value}";
            }
            return url;
        }

        /// <summary>
        /// Gets the current page name from the request
        /// </summary>
        /// <returns>Current page name</returns>
        public static string GetCurrentPageName()
        {
            if (HttpContext.Current?.Request?.Url != null)
            {
                string path = HttpContext.Current.Request.Url.AbsolutePath;
                string pageName = path.TrimStart('/').Split('/')[0];

                if (string.IsNullOrEmpty(pageName))
                {
                    return "dashboard";
                }

                return pageName.ToLower();
            }

            return "dashboard";
        }

        /// <summary>
        /// Checks if the current request is for a specific page
        /// </summary>
        /// <param name="pageName">Page name to check</param>
        /// <returns>True if current page matches</returns>
        public static bool IsCurrentPage(string pageName)
        {
            string currentPage = GetCurrentPageName();
            return currentPage.Equals(pageName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Safely redirects to login page
        /// </summary>
        public static void RedirectToLogin()
        {
            RedirectToCleanUrl("login.aspx");
        }

        /// <summary>
        /// Safely redirects to dashboard
        /// </summary>
        public static void RedirectToDashboard()
        {
            RedirectToCleanUrl("dashboard");
        }

        /// <summary>
        /// Redirects to unauthorized page
        /// </summary>
        public static void RedirectToUnauthorized()
        {
            RedirectToCleanUrl("unauthorized");
        }

        /// <summary>
        /// Builds navigation URLs for specific sections
        /// </summary>
        /// <param name="section">Section name (employees, reports, etc.)</param>
        /// <param name="action">Optional action (add, edit, view)</param>
        /// <param name="id">Optional ID parameter</param>
        /// <returns>Complete navigation URL</returns>
        public static string BuildNavigationUrl(string section, string action = null, int? id = null)
        {
            string url = $"/{section}";

            if (!string.IsNullOrEmpty(action))
            {
                url += $"/{action}";
            }

            if (id.HasValue)
            {
                url += $"/{id.Value}";
            }

            return url;
        }

        /// <summary>
        /// Determines if a URL is external
        /// </summary>
        /// <param name="url">URL to check</param>
        /// <returns>True if external URL</returns>
        public static bool IsExternalUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                   url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                   url.StartsWith("//", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the return URL from query string or provides default
        /// </summary>
        /// <param name="defaultUrl">Default URL if no return URL specified</param>
        /// <returns>Safe return URL</returns>
        public static string GetReturnUrl(string defaultUrl = "/dashboard")
        {
            if (HttpContext.Current?.Request != null)
            {
                string returnUrl = HttpContext.Current.Request.QueryString["returnUrl"];

                if (!string.IsNullOrEmpty(returnUrl) && !IsExternalUrl(returnUrl))
                {
                    return returnUrl;
                }
            }

            return defaultUrl;
        }
    }
}