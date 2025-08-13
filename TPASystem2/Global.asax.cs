using System;
using System.Web;
using System.Web.UI;

namespace TPASystem2
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // Set validation mode to prevent jQuery errors
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                // Get the last error
                Exception ex = Server.GetLastError();

                if (ex != null && ex is HttpException httpEx)
                {
                    // Handle 404 errors only
                    if (httpEx.GetHttpCode() == 404)
                    {
                        Server.ClearError();
                        Response.Redirect("~/Login.aspx?expired=true", false);
                        Context.ApplicationInstance.CompleteRequest();
                    }
                }
            }
            catch (Exception)
            {
                // Prevent recursive errors - silently fail
            }
        }
    }
}