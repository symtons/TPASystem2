using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using TPASystem2.Helpers;


namespace TPASystem2
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                SimpleUrlHelper.RedirectToCleanUrl("login");
                return;
            }

            if (!IsPostBack)
            {
                LoadDashboardData();
            }
        }

        private void LoadDashboardData()
        {
            try
            {
                // Get user information from session
                int userId = Convert.ToInt32(Session["UserId"]);
                string userEmail = Session["UserEmail"]?.ToString() ?? "";
                string userRole = Session["UserRole"]?.ToString() ?? "";
                string userName = Session["UserName"]?.ToString() ?? userEmail;

                // Set user info in UI (these controls are in the master page)
                SetUserInformation(userName, userRole);

                // Load dashboard components
                LoadNavigationMenu(userRole);
                LoadDashboardStats(userRole);
                LoadQuickActions(userRole);
                LoadRecentActivities(userId);

                // Log dashboard access
                LogUserActivity(userId, "Dashboard Access", "Dashboard", "User accessed dashboard", GetClientIP());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard: {ex.Message}");
                ShowError("Error loading dashboard data. Please refresh the page.");
            }
        }

        private void SetUserInformation(string userName, string userRole)
        {
            // Try to find controls in master page first, then content page
            var litUserName = FindControlRecursive(Page, "litUserName") as System.Web.UI.WebControls.Literal;
            var litUserInitial = FindControlRecursive(Page, "litUserInitial") as System.Web.UI.WebControls.Literal;
            var litHeaderUserInitial = FindControlRecursive(Page, "litHeaderUserInitial") as System.Web.UI.WebControls.Literal;
            var litUserRole = FindControlRecursive(Page, "litUserRole") as System.Web.UI.WebControls.Literal;

            // Set user name and initials
            if (!string.IsNullOrEmpty(userName))
            {
                string initial = userName.Substring(0, 1).ToUpper();

                if (litUserName != null) litUserName.Text = userName;
                if (litUserInitial != null) litUserInitial.Text = initial;
                if (litHeaderUserInitial != null) litHeaderUserInitial.Text = initial;
            }
            else
            {
                if (litUserName != null) litUserName.Text = "User";
                if (litUserInitial != null) litUserInitial.Text = "U";
                if (litHeaderUserInitial != null) litHeaderUserInitial.Text = "U";
            }

            // Set user role
            if (litUserRole != null) litUserRole.Text = userRole;
        }

        private void LoadNavigationMenu(string userRole)
        {
            try
            {
                var litNavigation = FindControlRecursive(Page, "litNavigation") as System.Web.UI.WebControls.Literal;
                if (litNavigation == null) return;

                var menuHtml = new StringBuilder();

                // Dashboard (everyone)
                menuHtml.AppendLine(CreateNavItem("Dashboard", "/dashboard", "dashboard", true));

                // Time & Attendance (everyone)
                menuHtml.AppendLine(CreateNavItem("Time & Attendance", "/time-attendance", "schedule"));

                // Employees (Admin, HR, Manager only)
                if (HasAccess(userRole, "Admin", "HR", "Manager", "SuperAdmin", "HRAdmin", "ProgramDirector"))
                {
                    menuHtml.AppendLine(CreateNavItem("Employees", "/employees", "people"));
                }

                // Leave Management (everyone)
                menuHtml.AppendLine(CreateNavItem("Leave Management", "/leave-management", "event_available"));

                // Reports (Admin, HR, Manager only)
                if (HasAccess(userRole, "Admin", "HR", "Manager", "SuperAdmin", "HRAdmin", "ProgramDirector"))
                {
                    menuHtml.AppendLine(CreateNavItem("Reports", "/reports", "assessment"));
                }

                // Profile (everyone)
                menuHtml.AppendLine(CreateNavItem("Profile", "/profile", "person"));

                // Settings (Admin, HR only)
                if (HasAccess(userRole, "Admin", "HR", "SuperAdmin", "HRAdmin"))
                {
                    menuHtml.AppendLine(CreateNavItem("Settings", "/settings", "settings"));
                }

                // Help (everyone)
                menuHtml.AppendLine(CreateNavItem("Help", "/help", "help"));

                litNavigation.Text = menuHtml.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading navigation: {ex.Message}");
                var litNavigation = FindControlRecursive(Page, "litNavigation") as System.Web.UI.WebControls.Literal;
                if (litNavigation != null)
                {
                    litNavigation.Text = CreateNavItem("Dashboard", "/dashboard", "dashboard", true);
                }
            }
        }

        private void LoadDashboardStats(string userRole)
        {
            try
            {
                var litDashboardStats = FindControlRecursive(Page, "litDashboardStats") as System.Web.UI.WebControls.Literal;
                if (litDashboardStats == null) return;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get stats from database based on user role
                    string query = @"
                        SELECT StatKey, StatName, StatValue, StatColor, IconName, Subtitle
                        FROM DashboardStats 
                        WHERE IsActive = 1 
                        AND (ApplicableRoles IS NULL OR ApplicableRoles LIKE '%' + @UserRole + '%')
                        ORDER BY SortOrder";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserRole", userRole);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var statsHtml = new StringBuilder();

                            while (reader.Read())
                            {
                                string statKey = reader["StatKey"].ToString();
                                string statName = reader["StatName"].ToString();
                                string statValue = reader["StatValue"].ToString();
                                string statColor = reader["StatColor"].ToString();
                                string iconName = reader["IconName"]?.ToString() ?? "analytics";
                                string subtitle = reader["Subtitle"]?.ToString() ?? "";

                                statsHtml.AppendLine(CreateStatCard(statName, statValue, iconName, statColor, subtitle));
                            }

                            // If no stats from database, show default stats
                            if (statsHtml.Length == 0)
                            {
                                statsHtml.AppendLine(CreateDefaultStats(userRole));
                            }

                            litDashboardStats.Text = statsHtml.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard stats: {ex.Message}");
                var litDashboardStats = FindControlRecursive(Page, "litDashboardStats") as System.Web.UI.WebControls.Literal;
                if (litDashboardStats != null)
                {
                    litDashboardStats.Text = CreateDefaultStats(userRole);
                }
            }
        }

        private void LoadQuickActions(string userRole)
        {
            try
            {
                var litQuickActions = FindControlRecursive(Page, "litQuickActions") as System.Web.UI.WebControls.Literal;
                if (litQuickActions == null) return;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get quick actions from database
                    string query = @"
                        SELECT ActionKey, Title, Description, IconName, Route, Color
                        FROM QuickActions 
                        WHERE IsActive = 1 
                        AND (ApplicableRoles IS NULL OR ApplicableRoles LIKE '%' + @UserRole + '%')
                        ORDER BY SortOrder";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserRole", userRole);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var actionsHtml = new StringBuilder();

                            while (reader.Read())
                            {
                                string actionKey = reader["ActionKey"].ToString();
                                string title = reader["Title"].ToString();
                                string description = reader["Description"]?.ToString() ?? "";
                                string iconName = reader["IconName"]?.ToString() ?? "touch_app";
                                string route = reader["Route"]?.ToString() ?? "#";
                                string color = reader["Color"]?.ToString() ?? "#ff9800";

                                actionsHtml.AppendLine(CreateQuickAction(title, description, iconName, route, actionKey));
                            }

                            // If no actions from database, show default actions
                            if (actionsHtml.Length == 0)
                            {
                                actionsHtml.AppendLine(CreateDefaultQuickActions(userRole));
                            }

                            litQuickActions.Text = actionsHtml.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading quick actions: {ex.Message}");
                var litQuickActions = FindControlRecursive(Page, "litQuickActions") as System.Web.UI.WebControls.Literal;
                if (litQuickActions != null)
                {
                    litQuickActions.Text = CreateDefaultQuickActions(userRole);
                }
            }
        }

        private void LoadRecentActivities(int userId)
        {
            try
            {
                var litRecentActivities = FindControlRecursive(Page, "litRecentActivities") as System.Web.UI.WebControls.Literal;
                if (litRecentActivities == null) return;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get recent activities
                    string query = @"
                        SELECT TOP 10 
                            ra.Action,
                            ra.EntityType,
                            ra.Details,
                            ra.CreatedAt,
                            u.Email as UserEmail,
                            at.Name as ActivityTypeName,
                            at.Color as ActivityColor,
                            at.IconName as ActivityIcon
                        FROM RecentActivities ra
                        LEFT JOIN Users u ON ra.UserId = u.Id
                        LEFT JOIN ActivityTypes at ON ra.ActivityTypeId = at.Id
                        ORDER BY ra.CreatedAt DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var activitiesHtml = new StringBuilder();

                            while (reader.Read())
                            {
                                string action = reader["Action"].ToString();
                                string entityType = reader["EntityType"].ToString();
                                string details = reader["Details"].ToString();
                                DateTime createdAt = Convert.ToDateTime(reader["CreatedAt"]);
                                string userEmail = reader["UserEmail"]?.ToString() ?? "System";
                                string activityColor = reader["ActivityColor"]?.ToString() ?? "#2196f3";

                                // Get user initials
                                string userInitials = GetUserInitials(userEmail);

                                // Format time ago
                                string timeAgo = FormatTimeAgo(createdAt);

                                activitiesHtml.AppendLine(CreateActivityItem(
                                    userInitials,
                                    userEmail,
                                    action,
                                    details,
                                    timeAgo,
                                    activityColor,
                                    createdAt > DateTime.Now.AddHours(-1) // Mark as new if within last hour
                                ));
                            }

                            // If no activities, show default message
                            if (activitiesHtml.Length == 0)
                            {
                                activitiesHtml.AppendLine(@"
                                    <li style='text-align: center; padding: 40px; color: #666;'>
                                        <i class='material-icons' style='font-size: 48px; margin-bottom: 16px; opacity: 0.5;'>timeline</i><br>
                                        No recent activities to display
                                    </li>");
                            }

                            litRecentActivities.Text = activitiesHtml.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading recent activities: {ex.Message}");
                var litRecentActivities = FindControlRecursive(Page, "litRecentActivities") as System.Web.UI.WebControls.Literal;
                if (litRecentActivities != null)
                {
                    litRecentActivities.Text = @"
                        <li style='text-align: center; padding: 40px; color: #666;'>
                            <i class='material-icons' style='font-size: 48px; margin-bottom: 16px; opacity: 0.5;'>error</i><br>
                            Unable to load recent activities
                        </li>";
                }
            }
        }

        #region Helper Methods

        /// <summary>
        /// Recursively finds a control by ID in the page hierarchy
        /// </summary>
        private System.Web.UI.Control FindControlRecursive(System.Web.UI.Control rootControl, string controlId)
        {
            if (rootControl.ID == controlId)
                return rootControl;

            foreach (System.Web.UI.Control control in rootControl.Controls)
            {
                System.Web.UI.Control foundControl = FindControlRecursive(control, controlId);
                if (foundControl != null)
                    return foundControl;
            }

            return null;
        }

        private string CreateNavItem(string title, string url, string icon, bool isActive = false)
        {
            string activeClass = isActive ? " active" : "";
            return $@"
                <div class='nav-item{activeClass}'>
                    <a href='{url}' class='nav-link'>
                        <i class='material-icons nav-icon'>{icon}</i>
                        <span class='nav-text'>{title}</span>
                    </a>
                </div>";
        }

        private string CreateStatCard(string title, string value, string icon, string color, string subtitle = "")
        {
            string subtitleHtml = !string.IsNullOrEmpty(subtitle) ? $"<div class='stat-change positive'>{subtitle}</div>" : "";

            return $@"
                <div class='stat-card'>
                    <div class='stat-header'>
                        <div class='stat-icon' style='background: {color};'>
                            <i class='material-icons'>{icon}</i>
                        </div>
                    </div>
                    <div class='stat-value'>{value}</div>
                    <div class='stat-label'>{title}</div>
                    {subtitleHtml}
                </div>";
        }

        private string CreateQuickAction(string title, string description, string icon, string route, string actionKey)
        {
            return $@"
                <a href='{route}' class='quick-action' onclick='handleQuickAction(""{actionKey}""); return false;'>
                    <div class='quick-action-icon'>
                        <i class='material-icons'>{icon}</i>
                    </div>
                    <div class='quick-action-title'>{title}</div>
                    <div class='quick-action-desc'>{description}</div>
                </a>";
        }

        private string CreateActivityItem(string userInitials, string userEmail, string action, string details, string timeAgo, string color, bool isNew = false)
        {
            string newBadge = isNew ? "<span class='activity-new'>NEW</span>" : "";
            string userName = userEmail.Split('@')[0]; // Get username part of email

            return $@"
                <li class='activity-item'>
                    <div class='activity-avatar' style='background: {color};'>
                        {userInitials}
                    </div>
                    <div class='activity-content'>
                        <div class='activity-header'>
                            <span class='activity-user'>{userName}</span>
                            <span class='activity-action'>{action}</span>
                            {newBadge}
                        </div>
                        <div class='activity-details'>{details}</div>
                        <div class='activity-time'>{timeAgo}</div>
                    </div>
                </li>";
        }

        private string CreateDefaultStats(string userRole)
        {
            var stats = new StringBuilder();

            switch (userRole.ToUpper())
            {
                case "SUPERADMIN":
                    stats.AppendLine(CreateStatCard("Total Users", "127", "people", "#2196f3", "All system users"));
                    stats.AppendLine(CreateStatCard("Active Sessions", "23", "schedule", "#4caf50", "Currently online"));
                    stats.AppendLine(CreateStatCard("System Health", "98%", "security", "#ff9800", "All systems operational"));
                    stats.AppendLine(CreateStatCard("Database Size", "2.1GB", "storage", "#9c27b0", "Total data storage"));
                    break;

                case "ADMIN":
                    stats.AppendLine(CreateStatCard("Total Employees", "98", "people", "#2196f3", "+3 this month"));
                    stats.AppendLine(CreateStatCard("Pending Approvals", "8", "assignment", "#ff9800", "Needs attention"));
                    stats.AppendLine(CreateStatCard("Active Programs", "12", "business", "#4caf50", "Running programs"));
                    stats.AppendLine(CreateStatCard("System Alerts", "2", "warning", "#f44336", "Requires review"));
                    break;

                case "HRADMIN":
                    stats.AppendLine(CreateStatCard("Active Employees", "98", "people", "#2196f3", "Current workforce"));
                    stats.AppendLine(CreateStatCard("Leave Requests", "15", "event_available", "#ff9800", "Pending approval"));
                    stats.AppendLine(CreateStatCard("New Hires", "5", "person_add", "#4caf50", "This month"));
                    stats.AppendLine(CreateStatCard("Performance Reviews", "23", "star", "#9c27b0", "Due this quarter"));
                    break;

                case "PROGRAMDIRECTOR":
                    stats.AppendLine(CreateStatCard("Program Performance", "94%", "trending_up", "#4caf50", "Above target"));
                    stats.AppendLine(CreateStatCard("Budget Utilization", "78%", "account_balance", "#2196f3", "On track"));
                    stats.AppendLine(CreateStatCard("Team Efficiency", "89%", "group_work", "#ff9800", "Good performance"));
                    stats.AppendLine(CreateStatCard("Client Satisfaction", "96%", "thumb_up", "#9c27b0", "Excellent ratings"));
                    break;

                case "PROGRAMCOORDINATOR":
                    stats.AppendLine(CreateStatCard("Assigned Programs", "8", "assignment", "#2196f3", "Active coordination"));
                    stats.AppendLine(CreateStatCard("Team Members", "24", "groups", "#4caf50", "Under coordination"));
                    stats.AppendLine(CreateStatCard("Tasks Completed", "156", "task_alt", "#ff9800", "This month"));
                    stats.AppendLine(CreateStatCard("Upcoming Deadlines", "7", "schedule", "#f44336", "Next 2 weeks"));
                    break;

                case "EMPLOYEE":
                default:
                    stats.AppendLine(CreateStatCard("My Hours", "38.5", "schedule", "#2196f3", "This week"));
                    stats.AppendLine(CreateStatCard("Leave Balance", "12", "event_available", "#4caf50", "Days remaining"));
                    stats.AppendLine(CreateStatCard("My Tasks", "3", "assignment", "#ff9800", "Pending"));
                    stats.AppendLine(CreateStatCard("Completed Projects", "8", "check_circle", "#9c27b0", "This quarter"));
                    break;
            }

            return stats.ToString();
        }

        private string CreateDefaultQuickActions(string userRole)
        {
            var actions = new StringBuilder();

            switch (userRole.ToUpper())
            {
                case "SUPERADMIN":
                    actions.AppendLine(CreateQuickAction("System Monitor", "Check system health", "security", "/admin/monitor", "system-monitor"));
                    actions.AppendLine(CreateQuickAction("User Management", "Manage system users", "manage_accounts", "/admin/users", "user-management"));
                    actions.AppendLine(CreateQuickAction("Database Backup", "Backup system data", "backup", "/admin/backup", "database-backup"));
                    actions.AppendLine(CreateQuickAction("System Settings", "Configure system", "settings", "/admin/settings", "system-settings"));
                    break;

                case "ADMIN":
                    actions.AppendLine(CreateQuickAction("Add Employee", "Register new employee", "person_add", "/employees/add", "add-employee"));
                    actions.AppendLine(CreateQuickAction("Generate Report", "Create system reports", "assessment", "/reports", "generate-report"));
                    actions.AppendLine(CreateQuickAction("Review Approvals", "Check pending requests", "assignment", "/approvals", "review-approvals"));
                    actions.AppendLine(CreateQuickAction("System Health", "View system status", "health_and_safety", "/admin/health", "system-health"));
                    break;

                case "HRADMIN":
                    actions.AppendLine(CreateQuickAction("New Employee", "Add new team member", "person_add", "/employees/add", "add-employee"));
                    actions.AppendLine(CreateQuickAction("Leave Approvals", "Review leave requests", "event_available", "/leave/approvals", "leave-approvals"));
                    actions.AppendLine(CreateQuickAction("HR Reports", "Generate HR analytics", "assessment", "/reports/hr", "hr-reports"));
                    actions.AppendLine(CreateQuickAction("Performance Reviews", "Manage evaluations", "star", "/hr/performance", "performance-reviews"));
                    break;

                case "PROGRAMDIRECTOR":
                    actions.AppendLine(CreateQuickAction("Program Overview", "View all programs", "business", "/programs", "program-overview"));
                    actions.AppendLine(CreateQuickAction("Strategic Planning", "Plan program strategy", "timeline", "/director/planning", "strategic-planning"));
                    actions.AppendLine(CreateQuickAction("Budget Review", "Monitor program budgets", "account_balance", "/director/budget", "budget-review"));
                    actions.AppendLine(CreateQuickAction("Performance Analytics", "View program metrics", "trending_up", "/director/analytics", "performance-analytics"));
                    break;

                case "PROGRAMCOORDINATOR":
                    actions.AppendLine(CreateQuickAction("Schedule Meeting", "Coordinate team meeting", "event", "/coordination/meeting", "schedule-meeting"));
                    actions.AppendLine(CreateQuickAction("Task Assignment", "Assign team tasks", "assignment", "/coordination/tasks", "task-assignment"));
                    actions.AppendLine(CreateQuickAction("Resource Planning", "Plan resource allocation", "inventory", "/coordinator/resources", "resource-planning"));
                    actions.AppendLine(CreateQuickAction("Team Reports", "View team progress", "assessment", "/reports/team", "team-reports"));
                    break;

                case "EMPLOYEE":
                default:
                    actions.AppendLine(CreateQuickAction("Clock In/Out", "Record work time", "schedule", "/time-attendance", "clock-in"));
                    actions.AppendLine(CreateQuickAction("Request Leave", "Submit time off request", "event_available", "/leave-management", "request-leave"));
                    actions.AppendLine(CreateQuickAction("View Profile", "Update personal info", "person", "/profile", "view-profile"));
                    actions.AppendLine(CreateQuickAction("My Documents", "Access my files", "folder", "/documents", "my-documents"));
                    break;
            }

            return actions.ToString();
        }

        private bool HasAccess(string userRole, params string[] allowedRoles)
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

        private string GetUserInitials(string email)
        {
            if (string.IsNullOrEmpty(email))
                return "U";

            if (email.Contains("@"))
            {
                string username = email.Split('@')[0];
                if (username.Length >= 2)
                    return (username.Substring(0, 1) + username.Substring(1, 1)).ToUpper();
                else
                    return username.Substring(0, 1).ToUpper();
            }

            return email.Substring(0, Math.Min(2, email.Length)).ToUpper();
        }

        private string FormatTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan.Days > 0)
                return $"{timeSpan.Days} day{(timeSpan.Days > 1 ? "s" : "")} ago";
            else if (timeSpan.Hours > 0)
                return $"{timeSpan.Hours} hour{(timeSpan.Hours > 1 ? "s" : "")} ago";
            else if (timeSpan.Minutes > 0)
                return $"{timeSpan.Minutes} minute{(timeSpan.Minutes > 1 ? "s" : "")} ago";
            else
                return "Just now";
        }

        private void LogUserActivity(int userId, string action, string entityType, string details, string ipAddress)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO RecentActivities (UserId, ActivityTypeId, Action, EntityType, Details, IPAddress, CreatedAt)
                        VALUES (@UserId, 1, @Action, @EntityType, @Details, @IPAddress, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@EntityType", entityType);
                        cmd.Parameters.AddWithValue("@Details", details);
                        cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging user activity: {ex.Message}");
            }
        }

        private string GetClientIP()
        {
            try
            {
                string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipAddress))
                    ipAddress = Request.ServerVariables["REMOTE_ADDR"];
                if (string.IsNullOrEmpty(ipAddress))
                    ipAddress = Request.UserHostAddress;

                return ipAddress ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        private void ShowError(string message)
        {
            // You can implement error display logic here
            // For now, just log to debug
            System.Diagnostics.Debug.WriteLine($"Dashboard Error: {message}");
        }

        #endregion
    }
}