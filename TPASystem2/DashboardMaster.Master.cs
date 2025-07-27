using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Web.Security;
using System.Web.UI;
using TPASystem2.Helpers;

namespace TPASystem2
{
    public partial class DashboardMaster : System.Web.UI.MasterPage
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in
            //if (Session["UserId"] == null)
            //{
            //    SimpleUrlHelper.RedirectToCleanUrl("login");
            //    return;
            //}

            if (!IsPostBack)
            {
                LoadMasterPageData();
            }
        }

        private void LoadMasterPageData()
        {
            try
            {
                // Get user information from session
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
            //litSidebarUserInitial.Text = initials;
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
            try
            {
                var menuHtml = new StringBuilder();
                string currentPage = GetCurrentPageName();

                // Dashboard (everyone)
                menuHtml.AppendLine(CreateNavItem("Dashboard", "/dashboard", "dashboard", currentPage == "dashboard"));

                // Time & Attendance (everyone)
                menuHtml.AppendLine(CreateNavItem("Time & Attendance", "/time-attendance", "schedule", currentPage == "time-attendance"));

                // Role-based menu items
                switch (userRole.ToUpper())
                {
                    case "SUPERADMIN":
                        // SuperAdmin gets everything
                        menuHtml.AppendLine(CreateNavItem("Employee Management", "/employees", "people", currentPage == "employees"));
                        menuHtml.AppendLine(CreateNavItem("Leave Management", "/leave-management", "event_available", currentPage == "leave-management"));
                        menuHtml.AppendLine(CreateNavItem("Reports & Analytics", "/reports", "assessment", currentPage == "reports"));
                        menuHtml.AppendLine(CreateNavItem("Onboarding", "/onboarding", "assignment", currentPage == "onboarding"));

                        // Admin Section
                        menuHtml.AppendLine(@"<div class='nav-section-header'>System Administration</div>");
                        menuHtml.AppendLine(CreateNavItem("User Management", "/admin/users", "manage_accounts", currentPage == "users"));
                        menuHtml.AppendLine(CreateNavItem("Role Management", "/admin/roles", "admin_panel_settings", currentPage == "roles"));
                        menuHtml.AppendLine(CreateNavItem("System Settings", "/admin/settings", "settings", currentPage == "settings"));
                        menuHtml.AppendLine(CreateNavItem("System Logs", "/admin/logs", "history", currentPage == "logs"));
                        menuHtml.AppendLine(CreateNavItem("Database Management", "/admin/database", "storage", currentPage == "database"));
                        break;

                    case "ADMIN":
                        // Admin gets most functionality
                        menuHtml.AppendLine(CreateNavItem("Employee Management", "/employees", "people", currentPage == "employees"));
                        menuHtml.AppendLine(CreateNavItem("Leave Management", "/leave-management", "event_available", currentPage == "leave-management"));
                        menuHtml.AppendLine(CreateNavItem("Reports & Analytics", "/reports", "assessment", currentPage == "reports"));
                        menuHtml.AppendLine(CreateNavItem("Onboarding", "/onboarding", "assignment", currentPage == "onboarding"));

                        // Admin Section
                        menuHtml.AppendLine(@"<div class='nav-section-header'>Administration</div>");
                        menuHtml.AppendLine(CreateNavItem("User Management", "/admin/users", "manage_accounts", currentPage == "users"));
                        menuHtml.AppendLine(CreateNavItem("System Settings", "/admin/settings", "settings", currentPage == "settings"));
                        break;

                    case "HRADMIN":
                        // HR Admin gets HR-focused functionality
                        menuHtml.AppendLine(CreateNavItem("Employees", "HR/Employees.aspx", "people"));
                        menuHtml.AppendLine(CreateNavItem("Leave Management", "/leave-management", "event_available", currentPage == "leave-management"));
                        menuHtml.AppendLine(CreateNavItem("HR Reports", "/reports", "assessment", currentPage == "reports"));
                        menuHtml.AppendLine(CreateNavItem("Onboarding", "HR/OnboardingManagement.aspx", "assignment"));

                        // HR Section
                        menuHtml.AppendLine(@"<div class='nav-section-header'>HR Management</div>");
                        menuHtml.AppendLine(CreateNavItem("Performance Reviews", "/hr/performance", "star", currentPage == "performance"));
                        menuHtml.AppendLine(CreateNavItem("Benefits Management", "HR/BenefitsManagement.aspx", "favorite"));
                        menuHtml.AppendLine(CreateNavItem("HR Settings", "/hr/settings", "settings", currentPage == "settings"));
                        break;

                    case "PROGRAMDIRECTOR":
                        // Program Director gets oversight functionality
                        menuHtml.AppendLine(CreateNavItem("Program Overview", "/programs", "business", currentPage == "programs"));
                        menuHtml.AppendLine(CreateNavItem("Employees", "/HR/Employees.aspx", "people"));
                        menuHtml.AppendLine(CreateNavItem("Leave Management", "/leave-management", "event_available", currentPage == "leave-management"));
                        menuHtml.AppendLine(CreateNavItem("Director Reports", "/reports", "assessment", currentPage == "reports"));

                        // Director Section
                        menuHtml.AppendLine(@"<div class='nav-section-header'>Program Management</div>");
                        menuHtml.AppendLine(CreateNavItem("Program Analytics", "/director/analytics", "trending_up", currentPage == "analytics"));
                        menuHtml.AppendLine(CreateNavItem("Budget Overview", "/director/budget", "account_balance", currentPage == "budget"));
                        menuHtml.AppendLine(CreateNavItem("Strategic Planning", "/director/planning", "timeline", currentPage == "planning"));
                        break;

                    case "PROGRAMCOORDINATOR":
                        // Program Coordinator gets coordination functionality
                        menuHtml.AppendLine(CreateNavItem("Program Coordination", "/coordination", "group_work", currentPage == "coordination"));
                        menuHtml.AppendLine(CreateNavItem("Team Management", "/team", "groups", currentPage == "team"));
                        menuHtml.AppendLine(CreateNavItem("Leave Requests", "/leave-management", "event_available", currentPage == "leave-management"));
                        menuHtml.AppendLine(CreateNavItem("Coordinator Reports", "/reports", "assessment", currentPage == "reports"));

                        // Coordinator Section
                        menuHtml.AppendLine(@"<div class='nav-section-header'>Coordination Tools</div>");
                        menuHtml.AppendLine(CreateNavItem("Schedule Management", "/coordinator/schedule", "calendar_today", currentPage == "schedule"));
                        menuHtml.AppendLine(CreateNavItem("Resource Planning", "/coordinator/resources", "inventory", currentPage == "resources"));
                        break;

                    case "EMPLOYEE":
                    default:
                        // Regular employees get basic functionality
                        menuHtml.AppendLine(CreateNavItem("My Leave", "/leave-management", "event_available", currentPage == "leave-management"));
                        menuHtml.AppendLine(CreateNavItem("My Reports", "/reports/personal", "assessment", currentPage == "reports"));

                        // Employee Section
                        menuHtml.AppendLine(@"<div class='nav-section-header'>My Tools</div>");
                        menuHtml.AppendLine(CreateNavItem("My Tasks", "/tasks", "task", currentPage == "tasks"));
                        menuHtml.AppendLine(CreateNavItem("My Documents", "/documents", "folder", currentPage == "documents"));
                        break;
                }

                // Profile (everyone)
                menuHtml.AppendLine(@"<div class='nav-section-spacer'></div>");
                menuHtml.AppendLine(CreateNavItem("My Profile", "/profile", "person", currentPage == "profile"));

                // Help (everyone)
                menuHtml.AppendLine(CreateNavItem("Help & Support", "/help", "help", currentPage == "help"));

                litNavigation.Text = menuHtml.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading navigation: {ex.Message}");
                litNavigation.Text = CreateNavItem("Dashboard", "/dashboard", "dashboard", true);
            }
        }

        private void LoadNotifications()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get unread notification count
                    string countQuery = @"
                        SELECT COUNT(*) 
                        FROM Notifications 
                        WHERE UserId = @UserId AND IsRead = 0";

                    using (SqlCommand cmd = new SqlCommand(countQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        int notificationCount = (int)cmd.ExecuteScalar();

                        litNotificationCount.Text = notificationCount.ToString();
                        pnlNotificationBadge.Visible = notificationCount > 0;
                    }

                    // Get recent notifications for dropdown
                    string notificationsQuery = @"
                        SELECT TOP 5 Title, Message, CreatedAt, Type
                        FROM Notifications 
                        WHERE UserId = @UserId 
                        ORDER BY CreatedAt DESC";

                    using (SqlCommand cmd = new SqlCommand(notificationsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var notificationsHtml = new StringBuilder();

                            while (reader.Read())
                            {
                                string title = reader["Title"].ToString();
                                string message = reader["Message"].ToString();
                                DateTime createdAt = Convert.ToDateTime(reader["CreatedAt"]);
                                string type = reader["Type"]?.ToString() ?? "info";

                                notificationsHtml.AppendLine(CreateNotificationItem(title, message, createdAt, type));
                            }

                            if (notificationsHtml.Length == 0)
                            {
                                notificationsHtml.AppendLine(@"
                                    <li class='no-notifications'>
                                        <span>No new notifications</span>
                                    </li>");
                            }

                            litNotifications.Text = notificationsHtml.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading notifications: {ex.Message}");
                litNotificationCount.Text = "0";
                pnlNotificationBadge.Visible = false;
                litNotifications.Text = @"<li class='no-notifications'><span>Unable to load notifications</span></li>";
            }
        }

        private void GenerateBreadcrumbs()
        {
            try
            {
                string currentPage = GetCurrentPageName();
                var breadcrumbsHtml = new StringBuilder();

                // Always start with TPA System
                breadcrumbsHtml.Append(@"<a href='/dashboard' class='breadcrumb-item'>TPA System</a>");
                breadcrumbsHtml.Append(@"<span class='breadcrumb-separator'>/</span>");

                // Add page-specific breadcrumbs
                switch (currentPage.ToLower())
                {
                    case "dashboard":
                        breadcrumbsHtml.Append(@"<span class='breadcrumb-item active'>Dashboard</span>");
                        break;
                    case "employees":
                        breadcrumbsHtml.Append(@"<span class='breadcrumb-item active'>Employees</span>");
                        break;
                    case "time-attendance":
                        breadcrumbsHtml.Append(@"<span class='breadcrumb-item active'>Time & Attendance</span>");
                        break;
                    case "leave-management":
                        breadcrumbsHtml.Append(@"<span class='breadcrumb-item active'>Leave Management</span>");
                        break;
                    case "reports":
                        breadcrumbsHtml.Append(@"<span class='breadcrumb-item active'>Reports</span>");
                        break;
                    case "profile":
                        breadcrumbsHtml.Append(@"<span class='breadcrumb-item active'>Profile</span>");
                        break;
                    case "settings":
                        breadcrumbsHtml.Append(@"<span class='breadcrumb-item active'>Settings</span>");
                        break;
                    default:
                        breadcrumbsHtml.Append($@"<span class='breadcrumb-item active'>{currentPage}</span>");
                        break;
                }

                litBreadcrumbs.Text = breadcrumbsHtml.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating breadcrumbs: {ex.Message}");
                litBreadcrumbs.Text = @"<span class='breadcrumb-item active'>Dashboard</span>";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                // Log the logout activity
                if (Session["UserId"] != null)
                {
                    int userId = Convert.ToInt32(Session["UserId"]);
                    LogUserActivity(userId, "Logout", "User", "User logged out", GetClientIP());
                }

                // Clear session
                Session.Clear();
                Session.Abandon();

                // Clear authentication
                FormsAuthentication.SignOut();

                // Redirect to login
                SimpleUrlHelper.RedirectToCleanUrl("login");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during logout: {ex.Message}");
                SimpleUrlHelper.RedirectToCleanUrl("login");
            }
        }

        #region Helper Methods

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
                        if (userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                            userRole.Equals("HRAdmin", StringComparison.OrdinalIgnoreCase))
                            return true;
                        break;
                    case "hr":
                        if (userRole.Equals("HRAdmin", StringComparison.OrdinalIgnoreCase))
                            return true;
                        break;
                    case "manager":
                        if (userRole.Equals("ProgramDirector", StringComparison.OrdinalIgnoreCase) ||
                            userRole.Equals("ProgramCoordinator", StringComparison.OrdinalIgnoreCase))
                            return true;
                        break;
                    case "director":
                        if (userRole.Equals("ProgramDirector", StringComparison.OrdinalIgnoreCase))
                            return true;
                        break;
                    case "coordinator":
                        if (userRole.Equals("ProgramCoordinator", StringComparison.OrdinalIgnoreCase))
                            return true;
                        break;
                }
            }

            return false;
        }

        private string GetUserInitials(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "U";

            string[] nameParts = name.Split(' ');
            if (nameParts.Length >= 2)
                return (nameParts[0].Substring(0, 1) + nameParts[1].Substring(0, 1)).ToUpper();
            else
                return name.Substring(0, Math.Min(2, name.Length)).ToUpper();
        }

        private string GetCurrentPageName()
        {
            string path = Request.CurrentExecutionFilePath;
            return System.IO.Path.GetFileNameWithoutExtension(path)?.ToLower() ?? "";
        }

        private string FormatTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan.Days > 0)
                return $"{timeSpan.Days}d ago";
            else if (timeSpan.Hours > 0)
                return $"{timeSpan.Hours}h ago";
            else if (timeSpan.Minutes > 0)
                return $"{timeSpan.Minutes}m ago";
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
                System.Diagnostics.Debug.WriteLine($"Error logging activity: {ex.Message}");
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

        #endregion

        #region Public Methods for Child Pages

        /// <summary>
        /// Set page title in breadcrumbs
        /// </summary>
        /// <param name="title"></param>
        public void SetPageTitle(string title)
        {
            var breadcrumbsHtml = new StringBuilder();
            breadcrumbsHtml.Append(@"<a href='/dashboard' class='breadcrumb-item'>TPA System</a>");
            breadcrumbsHtml.Append(@"<span class='breadcrumb-separator'>/</span>");
            breadcrumbsHtml.Append($@"<span class='breadcrumb-item active'>{title}</span>");
            litBreadcrumbs.Text = breadcrumbsHtml.ToString();
        }

        /// <summary>
        /// Set custom breadcrumb trail
        /// </summary>
        /// <param name="breadcrumbs"></param>
        public void SetBreadcrumbs(params BreadcrumbItem[] breadcrumbs)
        {
            var breadcrumbsHtml = new StringBuilder();

            for (int i = 0; i < breadcrumbs.Length; i++)
            {
                var item = breadcrumbs[i];
                bool isLast = i == breadcrumbs.Length - 1;

                if (isLast)
                {
                    breadcrumbsHtml.Append($@"<span class='breadcrumb-item active'>{item.Title}</span>");
                }
                else
                {
                    breadcrumbsHtml.Append($@"<a href='{item.Url}' class='breadcrumb-item'>{item.Title}</a>");
                    breadcrumbsHtml.Append(@"<span class='breadcrumb-separator'>/</span>");
                }
            }

            litBreadcrumbs.Text = breadcrumbsHtml.ToString();
        }

        /// <summary>
        /// Show notification in header
        /// </summary>
        /// <param name="count"></param>
        public void SetNotificationCount(int count)
        {
            litNotificationCount.Text = count.ToString();
            pnlNotificationBadge.Visible = count > 0;
        }

        #endregion
    }

    #region Helper Classes

    /// <summary>
    /// Breadcrumb item class
    /// </summary>
    public class BreadcrumbItem
    {
        public string Title { get; set; }
        public string Url { get; set; }

        public BreadcrumbItem(string title, string url = null)
        {
            Title = title;
            Url = url;
        }
    }

    #endregion
}