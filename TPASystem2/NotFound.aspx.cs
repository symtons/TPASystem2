using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;

namespace TPASystem2
{
    public partial class NotFound : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Log the 404 error
                LogPageNotFound();

                // Set response code
                Response.StatusCode = 404;
                Response.TrySkipIisCustomErrors = true;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Redirect to search results or dashboard with search term
                Response.Redirect($"/dashboard?search={HttpUtility.UrlEncode(searchTerm)}", false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }

        private void LogPageNotFound()
        {
            try
            {
                string requestedUrl = Request.Url?.ToString() ?? "";
                string referrer = Request.UrlReferrer?.ToString() ?? "";
                string userAgent = Request.UserAgent ?? "";
                string ipAddress = GetClientIP();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ErrorLogs (ErrorMessage, Source, Timestamp, RequestUrl, UserAgent, IPAddress, Referrer)
                        VALUES (@ErrorMessage, @Source, @Timestamp, @RequestUrl, @UserAgent, @IPAddress, @Referrer)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", "404 - Page Not Found");
                        cmd.Parameters.AddWithValue("@Source", "NotFound.aspx");
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@RequestUrl", requestedUrl);
                        cmd.Parameters.AddWithValue("@UserAgent", userAgent);
                        cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                        cmd.Parameters.AddWithValue("@Referrer", referrer);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                // Silently fail logging to prevent recursive errors
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
    }
}