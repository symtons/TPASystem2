using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace TPASystem2
{
    public partial class DashboardMaster : System.Web.UI.MasterPage
    {
        #region Properties and Fields

        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDashboardData();
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }

        #endregion

        #region Data Loading Methods

        private void LoadDashboardData()
        {
            try
            {
                string userEmail = Session["UserEmail"]?.ToString() ?? "";
                string userRole = Session["UserRole"]?.ToString() ?? "";
                string userName = Session["UserName"]?.ToString() ?? userEmail;

                // Set user info across all controls
                SetUserInformation(userName, userEmail, userRole);

                // Load navigation menu based on user role
                LoadNavigationMenu(userRole);

                // Load notifications
                LoadNotifications();

                // Generate breadcrumbs
                GenerateBreadcrumbs();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard master data: {ex.Message}");
            }
        }

        private void SetUserInformation(string userName, string userEmail, string userRole)
        {
            // Get user initials
            string initials = GetUserInitials(userName);

            // Set user info in sidebar
            litSidebarUserName.Text = userName;
            litSidebarUserRole.Text = userRole;

            // Set user info in header
            litHeaderUserInitial.Text = initials;
            litHeaderUserName.Text = userName;

            // Set user info in dropdown
            litDropdownUserInitial.Text = initials;
            litDropdownUserName.Text = userName;
            litDropdownUserEmail.Text = userEmail;
        }

        private void LoadNavigationMenu(string userRole)
        {
            //try
            //{
                var menuHtml = new StringBuilder();
                string currentPage = GetCurrentPageName();

                // Get the application root path dynamically
                string appPath = GetApplicationPath();

                // Dashboard (everyone) - DYNAMIC PATH
                menuHtml.AppendLine(CreateNavItem("Dashboard", $"{appPath}/Dashboard.aspx", "dashboard", currentPage == "dashboard"));

                // Time Management (everyone) - DYNAMIC PATH
               // menuHtml.AppendLine(CreateNavItem("Time Management", $"{appPath}/HR/TimeManagement.aspx", "schedule", currentPage == "time-management"));

                // Role-based menu items
                switch (userRole.ToUpper())
                {
                    case "SUPERADMIN":
                        menuHtml.AppendLine(CreateNavItem("Employee Management", $"{appPath}/HR/Employees.aspx", "people", currentPage == "employees"));
                        menuHtml.AppendLine(CreateNavItem("Leave Management", $"{appPath}/LeaveManagement/LeaveManagement.aspx", "event_available", currentPage == "leave-management"));
                        menuHtml.AppendLine(CreateNavItem("Reports & Analytics", "/reports", "assessment", currentPage == "reports"));
                        menuHtml.AppendLine(CreateNavItem("Onboarding", $"{appPath}/OnBoarding/OnboardingManagement.aspx", "assignment", currentPage == "onboarding"));

                        // Admin Section
                        menuHtml.AppendLine(@"<div class='nav-section-header'>System Administration</div>");
                        menuHtml.AppendLine(CreateNavItem("User Management", "/admin/users", "manage_accounts", currentPage == "users"));
                        menuHtml.AppendLine(CreateNavItem("Role Management", "/admin/roles", "admin_panel_settings", currentPage == "roles"));
                        menuHtml.AppendLine(CreateNavItem("System Settings", "/admin/settings", "settings", currentPage == "settings"));
                        menuHtml.AppendLine(CreateNavItem("System Logs", "/admin/logs", "history", currentPage == "logs"));
                        menuHtml.AppendLine(CreateNavItem("Database Management", "/admin/database", "storage", currentPage == "database"));
                        break;

                    case "ADMIN":
                        menuHtml.AppendLine(CreateNavItem("Employee Management", $"{appPath}/HR/Employees.aspx", "people", currentPage == "employees"));
                        menuHtml.AppendLine(CreateNavItem("Leave Management", $"{appPath}/LeaveManagement/LeaveManagement.aspx", "event_available", currentPage == "leave-management"));
                        menuHtml.AppendLine(CreateNavItem("Reports & Analytics", "/reports", "assessment", currentPage == "reports"));
                        menuHtml.AppendLine(CreateNavItem("Onboarding", $"{appPath}/OnBoarding/OnboardingManagement.aspx", "assignment", currentPage == "onboarding"));

                        // Admin Section
                        menuHtml.AppendLine(@"<div class='nav-section-header'>Administration</div>");
                        menuHtml.AppendLine(CreateNavItem("User Management", "/admin/users", "manage_accounts", currentPage == "users"));
                        menuHtml.AppendLine(CreateNavItem("System Settings", "/admin/settings", "settings", currentPage == "settings"));
                        break;

                    case "HRADMIN":
                        // HR Admin gets HR-focused functionality - DYNAMIC PATHS
                        menuHtml.AppendLine(CreateNavItem("Employees", $"{appPath}/HR/Employees.aspx", "people", currentPage == "employees"));
                        menuHtml.AppendLine(CreateNavItem("Time Management", $"{appPath}/TimeManagement/TimeManagement.aspx", "schedule", currentPage == "time-management"));
                        menuHtml.AppendLine(CreateNavItem("Leave Management", $"{appPath}/LeaveManagement/LeaveManagement.aspx", "event_available", currentPage == "leave-management"));
                        menuHtml.AppendLine(CreateNavItem("HR Reports", "/reports", "assessment", currentPage == "reports"));
                        menuHtml.AppendLine(CreateNavItem("Onboarding", $"{appPath}/OnBoarding/OnboardingManagement.aspx", "assignment", currentPage == "onboarding"));

                        // HR Section
                        menuHtml.AppendLine(@"<div class='nav-section-header'>HR Management</div>");
                        menuHtml.AppendLine(CreateNavItem("Performance Reviews", "/hr/performance", "star", currentPage == "performance"));
                        menuHtml.AppendLine(CreateNavItem("Benefits Management", $"{appPath}/HR/BenefitsManagement.aspx", "favorite", currentPage == "benefits"));
                        menuHtml.AppendLine(CreateNavItem("HR Settings", "/hr/settings", "settings", currentPage == "settings"));
                        break;

                    case "PROGRAMDIRECTOR":
                        menuHtml.AppendLine(CreateNavItem("Program Overview", "/programs", "business", currentPage == "programs"));
                        menuHtml.AppendLine(CreateNavItem("Employees", $"{appPath}/HR/Employees.aspx", "people", currentPage == "employees"));
                    menuHtml.AppendLine(CreateNavItem("Time Management", $"{appPath}/HR/TimeManagement.aspx", "schedule", currentPage == "time-management"));
                    menuHtml.AppendLine(CreateNavItem("Leave Management", "/LeaveManagement/Default.aspx", "event_available", currentPage == "leave-management"));
                        menuHtml.AppendLine(CreateNavItem("Director Reports", "/reports", "assessment", currentPage == "reports"));

                        // Director Section
                        menuHtml.AppendLine(@"<div class='nav-section-header'>Program Management</div>");
                        menuHtml.AppendLine(CreateNavItem("Program Analytics", "/director/analytics", "trending_up", currentPage == "analytics"));
                        menuHtml.AppendLine(CreateNavItem("Budget Overview", "/director/budget", "account_balance", currentPage == "budget"));
                        menuHtml.AppendLine(CreateNavItem("Strategic Planning", "/director/planning", "timeline", currentPage == "planning"));
                        break;

                    case "PROGRAMCOORDINATOR":
                        menuHtml.AppendLine(CreateNavItem("Program Coordination", "/coordination", "group_work", currentPage == "coordination"));
                        menuHtml.AppendLine(CreateNavItem("Team Management", "/team", "groups", currentPage == "team"));
                        menuHtml.AppendLine(CreateNavItem("Leave Requests", "/LeaveManagement/Default.aspx", "event_available", currentPage == "leave-management"));
                        menuHtml.AppendLine(CreateNavItem("Coordinator Reports", "/reports", "assessment", currentPage == "reports"));

                        // Coordinator Section
                        menuHtml.AppendLine(@"<div class='nav-section-header'>Coordination Tools</div>");
                        menuHtml.AppendLine(CreateNavItem("Schedule Management", "/coordinator/schedule", "calendar_today", currentPage == "schedule"));
                        menuHtml.AppendLine(CreateNavItem("Resource Planning", "/coordinator/resources", "inventory", currentPage == "resources"));
                        break;

                    case "EMPLOYEE":
                    default:
                        // Check if employee has onboarding tasks
                        bool hasOnboardingTasks = HasEmployeeOnboardingTasks();

                        if (hasOnboardingTasks)
                        {
                            menuHtml.AppendLine(CreateNavItem("My Onboarding", $"{appPath}/OnBoarding/MyOnboarding.aspx", "assignment_ind", currentPage == "my-onboarding"));
                        }
                        else
                        {
                        menuHtml.AppendLine(CreateNavItem("Time Management", $"{appPath}/TimeManagement/EmployeeTimesheets.aspx", "assignment_ind", currentPage == "time-tracking"));
                        menuHtml.AppendLine(CreateNavItem("My Leave",  $"{appPath}/LeaveManagement/EmployeeLeavePortal.aspx", "event_available", currentPage == "leave-management"));
                            menuHtml.AppendLine(CreateNavItem("My Reports", "/reports/personal", "assessment", currentPage == "reports"));

                            // Employee Section
                            menuHtml.AppendLine(@"<div class='nav-section-header'>My Tools</div>");
                            menuHtml.AppendLine(CreateNavItem("My Tasks", "/tasks", "task", currentPage == "tasks"));
                            menuHtml.AppendLine(CreateNavItem("My Documents", "/documents", "folder", currentPage == "documents"));

                        }
                        break;
                }

                // Profile (everyone)
                menuHtml.AppendLine(@"<div class='nav-section-spacer'></div>");
                menuHtml.AppendLine(CreateNavItem("My Profile", "/profile", "person", currentPage == "profile"));

                // Help (everyone)
                menuHtml.AppendLine(CreateNavItem("Help & Support", "/help", "help", currentPage == "help"));

                litNavigation.Text = menuHtml.ToString();
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine($"Error loading navigation menu: {ex.Message}");
            //    litNavigation.Text = CreateDefaultMenu(userRole);
            //}
        }

        private bool HasEmployeeOnboardingTasks()
        {
            //try
            //{
                int userId = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : 0;

                if (userId == 0) return false;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT COUNT(*) 
                        FROM [dbo].[OnboardingTasks] ot
                        INNER JOIN [dbo].[Employees] e ON ot.EmployeeId = e.Id
                        WHERE e.UserId = @UserId 
                          AND ot.IsTemplate = 0 
                          AND ot.Status != 'COMPLETED'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        int taskCount = Convert.ToInt32(cmd.ExecuteScalar());
                        return taskCount > 0;
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine($"Error checking employee onboarding tasks: {ex.Message}");
            //    return false;
            //}
        }

        // NEW METHOD: Get application path dynamically from IIS
        private string GetApplicationPath()
        {
            string appPath = Request.ApplicationPath;

            // Debug output
            System.Diagnostics.Debug.WriteLine($"Request.ApplicationPath: '{appPath}'");

            // If application is in root, return empty string
            if (appPath == "/")
                return "";

            // Remove trailing slash if present
            if (appPath.EndsWith("/"))
                appPath = appPath.TrimEnd('/');

            return appPath;
        }

        private void LoadNotifications()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT TOP 5 Title, Message, CreatedAt, Type
                        FROM Notifications 
                        WHERE IsRead = 0 
                        ORDER BY CreatedAt DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var notifications = new StringBuilder();
                            int count = 0;

                            while (reader.Read())
                            {
                                string title = reader["Title"].ToString();
                                string message = reader["Message"].ToString();
                                DateTime createdAt = Convert.ToDateTime(reader["CreatedAt"]);
                                string type = reader["Type"].ToString();

                                notifications.AppendLine(CreateNotificationItem(title, message, createdAt, type));
                                count++;
                            }

                            if (count == 0)
                            {
                                notifications.AppendLine(@"
                                    <li class='notification-item no-notifications'>
                                        <div class='notification-content'>
                                            <div class='notification-message'>No new notifications</div>
                                        </div>
                                    </li>");
                            }

                            litNotifications.Text = notifications.ToString();
                            litNotificationCount.Text = count.ToString();
                            pnlNotificationBadge.Visible = count > 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading notifications: {ex.Message}");
                litNotifications.Text = @"
                    <li class='notification-item no-notifications'>
                        <div class='notification-content'>
                            <div class='notification-message'>Unable to load notifications</div>
                        </div>
                    </li>";
                litNotificationCount.Text = "0";
                pnlNotificationBadge.Visible = false;
            }
        }

        #endregion

        #region Helper Methods

        private string GetCurrentPageName()
        {
            try
            {
                string pageName = System.IO.Path.GetFileNameWithoutExtension(Request.Url.AbsolutePath).ToLower();

                // Map specific pages to menu identifiers
                switch (pageName)
                {
                    case "dashboard": return "dashboard";
                    case "employees": return "employees";
                    case "timemanagement": return "time-management";
                    case "default":
                        if (Request.Url.AbsolutePath.Contains("LeaveManagement"))
                            return "leave-management";
                        break;
                    case "onboardingmanagement": return "onboarding";
                    case "myonboarding": return "my-onboarding";
                    case "benefitsmanagement": return "benefits";
                }

                return pageName;
            }
            catch
            {
                return "";
            }
        }

        private string CreateNavItem(string title, string url, string icon, bool isActive = false)
        {
            string activeClass = isActive ? " active" : "";

            // NO URL RESOLUTION - use exactly what's passed in
            return $@"
                <div class='nav-item{activeClass}'>
                    <a href='{url}' class='nav-link'>
                        <i class='material-icons nav-icon'>{icon}</i>
                        <span class='nav-text'>{title}</span>
                    </a>
                </div>";
        }

        // UPDATED CreateDefaultMenu with dynamic paths
        private string CreateDefaultMenu(string userRole)
        {
            var menuHtml = new StringBuilder();
            string appPath = GetApplicationPath();

            // Dashboard (everyone) - DYNAMIC
            menuHtml.AppendLine(CreateNavItem("Dashboard", $"{appPath}/Dashboard.aspx", "dashboard", true));

            // Time Management (everyone) - DYNAMIC
            menuHtml.AppendLine(CreateNavItem("Time Management", $"{appPath}/HR/TimeManagement.aspx", "schedule"));

            // Employees (Admin, HR, Manager only) - DYNAMIC
            if (HasAccess(userRole, "Admin", "HR", "Manager", "SuperAdmin", "HRAdmin", "ProgramDirector"))
            {
                menuHtml.AppendLine(CreateNavItem("Employees", $"{appPath}/HR/Employees.aspx", "people"));
            }

            // Leave Management (everyone)
            menuHtml.AppendLine(CreateNavItem("Leave Management", $"{appPath}/LeaveManagement", "event_available"));

            // Reports (Admin, HR, Manager only)
            if (HasAccess(userRole, "Admin", "HR", "Manager", "SuperAdmin", "HRAdmin", "ProgramDirector"))
            {
                menuHtml.AppendLine(CreateNavItem("Reports", "/reports", "assessment"));
            }

            return menuHtml.ToString();
        }

        private string CreateNotificationItem(string title, string message, DateTime createdAt, string type)
        {
            string timeAgo = FormatTimeAgo(createdAt);
            string iconName = GetNotificationIcon(type);

            return $@"
                <li class='notification-item'>
                    <a href='#'>
                        <div class='notification-icon {type}'>
                            <i class='material-icons'>{iconName}</i>
                        </div>
                        <div class='notification-content'>
                            <div class='notification-title'>{title}</div>
                            <div class='notification-message'>{message}</div>
                            <div class='notification-time'>{timeAgo}</div>
                        </div>
                    </a>
                </li>";
        }

        private string GetNotificationIcon(string type)
        {
            switch (type.ToLower())
            {
                case "success": return "check_circle";
                case "warning": return "warning";
                case "error": return "error";
                case "info":
                default: return "info";
            }
        }

        private bool HasAccess(string userRole, params string[] allowedRoles)
        {
            if (string.IsNullOrEmpty(userRole))
                return false;

            // SuperAdmin always has access to everything
            if (userRole.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase))
                return true;

            foreach (string role in allowedRoles)
            {
                if (userRole.Equals(role, StringComparison.OrdinalIgnoreCase))
                    return true;

                // Handle role hierarchy and aliases
                switch (role.ToLower())
                {
                    case "admin":
                        if (userRole.Equals("Administrator", StringComparison.OrdinalIgnoreCase))
                            return true;
                        break;
                    case "hr":
                        if (userRole.Equals("HRAdmin", StringComparison.OrdinalIgnoreCase) ||
                            userRole.Equals("HRManager", StringComparison.OrdinalIgnoreCase))
                            return true;
                        break;
                    case "manager":
                        if (userRole.Equals("ProgramDirector", StringComparison.OrdinalIgnoreCase) ||
                            userRole.Equals("ProgramCoordinator", StringComparison.OrdinalIgnoreCase))
                            return true;
                        break;
                }
            }

            return false;
        }

        private string GetUserInitials(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return "U";

            if (userName.Contains("@"))
            {
                string username = userName.Split('@')[0];
                if (username.Length >= 2)
                    return (username.Substring(0, 1) + username.Substring(1, 1)).ToUpper();
                else
                    return username.Substring(0, 1).ToUpper();
            }

            var parts = userName.Split(' ');
            if (parts.Length >= 2)
            {
                return (parts[0].Substring(0, 1) + parts[1].Substring(0, 1)).ToUpper();
            }

            return userName.Substring(0, Math.Min(2, userName.Length)).ToUpper();
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

        private void GenerateBreadcrumbs()
        {
            try
            {
                var breadcrumbs = new StringBuilder();
                string currentPage = GetCurrentPageName();
                string appPath = GetApplicationPath();

                // Always start with Dashboard
                breadcrumbs.AppendLine($"<a href='{appPath}/Dashboard.aspx' class='breadcrumb-link'>Dashboard</a>");

                // Add current page breadcrumb based on page
                switch (currentPage)
                {
                    case "employees":
                        breadcrumbs.AppendLine("<span class='breadcrumb-separator'>></span>");
                        breadcrumbs.AppendLine("<span class='breadcrumb-current'>Employees</span>");
                        break;
                    case "time-management":
                        breadcrumbs.AppendLine("<span class='breadcrumb-separator'>></span>");
                        breadcrumbs.AppendLine("<span class='breadcrumb-current'>Time Management</span>");
                        break;
                    case "leave-management":
                        breadcrumbs.AppendLine("<span class='breadcrumb-separator'>></span>");
                        breadcrumbs.AppendLine("<span class='breadcrumb-current'>Leave Management</span>");
                        break;
                    case "onboarding":
                        breadcrumbs.AppendLine("<span class='breadcrumb-separator'>></span>");
                        breadcrumbs.AppendLine("<span class='breadcrumb-current'>Onboarding Management</span>");
                        break;
                    case "my-onboarding":
                        breadcrumbs.AppendLine("<span class='breadcrumb-separator'>></span>");
                        breadcrumbs.AppendLine("<span class='breadcrumb-current'>My Onboarding</span>");
                        break;
                    case "benefits":
                        breadcrumbs.AppendLine("<span class='breadcrumb-separator'>></span>");
                        breadcrumbs.AppendLine("<span class='breadcrumb-current'>Benefits Management</span>");
                        break;
                }

                litBreadcrumbs.Text = breadcrumbs.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating breadcrumbs: {ex.Message}");
                litBreadcrumbs.Text = "<span class='breadcrumb-current'>Dashboard</span>";
            }
        }

        #endregion
    }
}