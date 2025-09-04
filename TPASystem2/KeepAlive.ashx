using System;
using System.Web;
using System.Web.SessionState;

namespace TPASystem2
{
    /// <summary>
    /// Keep Alive Handler - Maintains session activity
    /// This handler is called by the session manager to keep the server session alive
    /// </summary>
    public class KeepAlive : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                // Set response headers
                context.Response.ContentType = "application/json";
                context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                context.Response.Cache.SetNoStore();
                context.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));

                // Check if session exists and user is logged in
                if (context.Session != null && context.Session["UserId"] != null)
                {
                    // Session is valid - update last activity
                    context.Session["LastActivity"] = DateTime.UtcNow;
                    
                    // Log keep-alive activity (optional)
                    string userId = context.Session["UserId"].ToString();
                    string userEmail = context.Session["UserEmail"]?.ToString() ?? "Unknown";
                    
                    System.Diagnostics.Debug.WriteLine($"Keep-alive ping from user {userEmail} (ID: {userId}) at {DateTime.UtcNow}");
                    
                    // Return success response
                    context.Response.Write(@"{
                        ""success"": true,
                        ""message"": ""Session kept alive"",
                        ""timestamp"": """ + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + @""",
                        ""sessionTimeoutMinutes"": 10
                    }");
                }
                else
                {
                    // Session is invalid or expired
                    context.Response.StatusCode = 401; // Unauthorized
                    context.Response.Write(@"{
                        ""success"": false,
                        ""message"": ""Session expired or invalid"",
                        ""timestamp"": """ + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + @"""
                    }");
                }
            }
            catch (Exception ex)
            {
                // Handle errors
                context.Response.StatusCode = 500; // Internal Server Error
                context.Response.Write(@"{
                    ""success"": false,
                    ""message"": ""Server error occurred"",
                    ""error"": """ + ex.Message.Replace("\"", "\\\"") + @""",
                    ""timestamp"": """ + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + @"""
                }");
                
                // Log the error (in production, use proper logging)
                System.Diagnostics.Debug.WriteLine($"KeepAlive Handler Error: {ex.Message}");
            }
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}