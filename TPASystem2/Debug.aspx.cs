using System;
using System.IO;
using System.Web;
using System.Web.Routing;
using System.Web.UI;

namespace TPASystem2
{
    public partial class Debug : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDebugInfo();
            }
        }

        private void LoadDebugInfo()
        {
            try
            {
                // Current URL info
                litCurrentUrl.Text = Request.Url.ToString();
                litPhysicalPath.Text = Request.PhysicalPath;

                // Application start time
                object startTime = Application["StartTime"];
                litAppStartTime.Text = startTime?.ToString() ?? "Not available";

                // Check if HR/Employees.aspx exists
                string employeesPath = Server.MapPath("~/HR/Employees.aspx");
                bool employeesExists = File.Exists(employeesPath);
                litEmployeesExists.Text = employeesExists ?
                    $"<span class='success'>✓ Yes - {employeesPath}</span>" :
                    $"<span class='error'>✗ No - {employeesPath}</span>";

                // Route information
                LoadRouteInfo();

                // Application state
                LoadApplicationState();
            }
            catch (Exception ex)
            {
                litRouteInfo.Text = $"<span class='error'>Error loading debug info: {ex.Message}</span>";
            }
        }

        private void LoadRouteInfo()
        {
            try
            {
                var routeInfo = new System.Text.StringBuilder();
                routeInfo.AppendLine("<strong>Registered Routes:</strong><br/>");

                if (RouteTable.Routes.Count == 0)
                {
                    routeInfo.AppendLine("<span class='error'>⚠️ No routes registered! This is the problem.</span><br/>");
                }
                else
                {
                    routeInfo.AppendLine($"<span class='success'>✓ {RouteTable.Routes.Count} routes registered:</span><br/>");

                    foreach (RouteBase route in RouteTable.Routes)
                    {
                        if (route is Route pageRoute)
                        {
                            routeInfo.AppendLine($"• <code>{pageRoute.Url}</code><br/>");
                        }
                        else if (route.GetType().Name == "PageRouteHandler")
                        {
                            // Try to get route info via reflection for PageRoute
                            try
                            {
                                var urlProperty = route.GetType().GetProperty("Url");
                                string url = urlProperty?.GetValue(route)?.ToString() ?? "Unknown";
                                routeInfo.AppendLine($"• <code>{url}</code><br/>");
                            }
                            catch
                            {
                                routeInfo.AppendLine($"• Route: {route.GetType().Name}<br/>");
                            }
                        }
                    }
                }

                litRouteInfo.Text = routeInfo.ToString();
            }
            catch (Exception ex)
            {
                litRouteInfo.Text = $"<span class='error'>Error getting route info: {ex.Message}</span>";
            }
        }

        private void LoadApplicationState()
        {
            try
            {
                var appState = new System.Text.StringBuilder();

                appState.AppendLine($"<strong>Application Name:</strong> {Application["ApplicationName"]}<br/>");
                appState.AppendLine($"<strong>Version:</strong> {Application["ApplicationVersion"]}<br/>");
                appState.AppendLine($"<strong>Active Sessions:</strong> {Application["ActiveSessions"]}<br/>");

                // Check if Global.asax.cs Application_Start was called
                if (Application["StartTime"] != null)
                {
                    appState.AppendLine("<span class='success'>✓ Application_Start was called</span><br/>");
                }
                else
                {
                    appState.AppendLine("<span class='error'>✗ Application_Start was NOT called</span><br/>");
                }

                litAppState.Text = appState.ToString();
            }
            catch (Exception ex)
            {
                litAppState.Text = $"<span class='error'>Error getting app state: {ex.Message}</span>";
            }
        }
    }
}