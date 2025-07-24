using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Master page initialization
        }

        /// <summary>
        /// Get CSS class for body based on current page and user
        /// </summary>
        /// <returns></returns>
        protected string GetBodyClass()
        {
            string pageClass = GetCurrentPageClass();
            string userClass = GetUserClass();

            return $"{pageClass} {userClass}".Trim();
        }

        /// <summary>
        /// Get current page CSS class
        /// </summary>
        /// <returns></returns>
        private string GetCurrentPageClass()
        {
            string pageName = System.IO.Path.GetFileNameWithoutExtension(Request.CurrentExecutionFilePath);
            return $"page-{pageName.ToLower()}";
        }

        /// <summary>
        /// Get user role CSS class
        /// </summary>
        /// <returns></returns>
        private string GetUserClass()
        {
            if (Session["UserRole"] != null)
            {
                string role = Session["UserRole"].ToString().ToLower().Replace(" ", "-");
                return $"user-{role}";
            }
            return "user-guest";
        }

        /// <summary>
        /// Show global notification
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public void ShowGlobalNotification(string message, string type = "info")
        {
            litGlobalNotification.Text = message;
            pnlGlobalNotification.CssClass = $"global-notification {type}";
            pnlGlobalNotification.Visible = true;
        }

        /// <summary>
        /// Hide global notification
        /// </summary>
        public void HideGlobalNotification()
        {
            pnlGlobalNotification.Visible = false;
        }

        /// <summary>
        /// Check if user is logged in
        /// </summary>
        /// <returns></returns>
        public bool IsUserLoggedIn()
        {
            return Session["UserId"] != null;
        }

        /// <summary>
        /// Get current user role
        /// </summary>
        /// <returns></returns>
        public string GetUserRole()
        {
            return Session["UserRole"]?.ToString() ?? "";
        }

        /// <summary>
        /// Get current user ID
        /// </summary>
        /// <returns></returns>
        public int GetUserId()
        {
            if (Session["UserId"] != null)
            {
                return Convert.ToInt32(Session["UserId"]);
            }
            return 0;
        }

        /// <summary>
        /// Get current user name
        /// </summary>
        /// <returns></returns>
        public string GetUserName()
        {
            return Session["UserName"]?.ToString() ?? Session["UserEmail"]?.ToString() ?? "";
        }
    }
}