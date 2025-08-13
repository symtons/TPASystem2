using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
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
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Handle AJAX refresh requests FIRST
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                string eventArgument = Request.Form["__EVENTARGUMENT"];
                if (eventArgument == "RefreshStats")
                {
                    RefreshDashboardStatsOnly();
                    return;
                }
                else if (eventArgument == "RefreshActivities")
                {
                    RefreshActivitiesOnly();
                    return;
                }
            }

            // Normal page load
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

                // Set user info in UI
                SetUserInformation(userName, userRole);

                // Load dashboard components
                LoadNavigationMenu(userRole);
                LoadRealTimeDashboardStats(userRole, userId);
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

        #region Real-time Dashboard Stats with Inline Queries

        /// <summary>
        /// Load real-time dashboard statistics using inline queries based on user role
        /// Each user gets exactly 4 stats relevant to their role
        /// </summary>
        private void LoadRealTimeDashboardStats(string userRole, int userId)
        {
            try
            {
                var litDashboardStats = FindControlRecursive(Page, "litDashboardStats") as System.Web.UI.WebControls.Literal;
                if (litDashboardStats == null) return;

                List<DashboardStat> stats = GetRealTimeStatsByRole(userRole, userId);

                // Ensure exactly 4 stats
                if (stats.Count == 4)
                {
                    var statsHtml = new StringBuilder();
                    foreach (var stat in stats)
                    {
                        statsHtml.AppendLine(CreateStatCard(stat.Name, stat.Value, stat.Icon, stat.Color, stat.Subtitle, stat.Key));
                    }
                    litDashboardStats.Text = statsHtml.ToString();
                }
                else
                {
                    // Fallback if we don't get exactly 4 stats
                    litDashboardStats.Text = CreateFallbackStats(userRole);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading real-time dashboard stats: {ex.Message}");
                var litDashboardStats = FindControlRecursive(Page, "litDashboardStats") as System.Web.UI.WebControls.Literal;
                if (litDashboardStats != null)
                {
                    litDashboardStats.Text = CreateFallbackStats(userRole);
                }
            }
        }

        /// <summary>
        /// Get real-time statistics based on user role using inline database queries
        /// </summary>
        private List<DashboardStat> GetRealTimeStatsByRole(string userRole, int userId)
        {
            var stats = new List<DashboardStat>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                switch (userRole.ToUpper())
                {
                    case "SUPERADMIN":
                        stats = GetSuperAdminStats(conn);
                        break;
                    case "ADMIN":
                        stats = GetAdminStats(conn);
                        break;
                    case "HRADMIN":
                        stats = GetHRAdminStats(conn);
                        break;
                    case "PROGRAMDIRECTOR":
                        stats = GetProgramDirectorStats(conn, userId);
                        break;
                    case "PROGRAMCOORDINATOR":
                        stats = GetProgramCoordinatorStats(conn, userId);
                        break;
                    case "EMPLOYEE":
                    default:
                        stats = GetEmployeeStats(conn, userId);
                        break;
                }
            }

            return stats;
        }

        private List<DashboardStat> GetSuperAdminStats(SqlConnection conn)
        {
            var stats = new List<DashboardStat>();

            // 1. Total Users
            string totalUsersQuery = "SELECT COUNT(*) FROM Users WHERE IsActive = 1";
            var totalUsers = ExecuteScalarQuery(conn, totalUsersQuery);
            stats.Add(new DashboardStat("total_users", "Total Users", totalUsers.ToString(), "primary", "people", "Active system users"));

            // 2. Active Sessions (last 24 hours)
            string activeSessionsQuery = @"
                SELECT COUNT(*) FROM UserSessions 
                WHERE IsActive = 1 AND CreatedAt >= DATEADD(HOUR, -24, GETUTCDATE())";
            var activeSessions = ExecuteScalarQuery(conn, activeSessionsQuery);
            stats.Add(new DashboardStat("active_sessions", "Active Sessions", activeSessions.ToString(), "success", "schedule", "Last 24 hours"));

            // 3. Total Departments
            string totalDepartmentsQuery = "SELECT COUNT(*) FROM Departments WHERE IsActive = 1";
            var totalDepartments = ExecuteScalarQuery(conn, totalDepartmentsQuery);
            stats.Add(new DashboardStat("total_departments", "Total Departments", totalDepartments.ToString(), "info", "analytics", "Active departments"));

            // 4. Logins Today
            string loginsQuery = "SELECT COUNT(*) FROM Users WHERE CAST(LastLogin AS DATE) = CAST(GETDATE() AS DATE)";
            var loginsToday = ExecuteScalarQuery(conn, loginsQuery);
            stats.Add(new DashboardStat("recent_logins", "Logins Today", loginsToday.ToString(), "warning", "login", "User activity today"));

            return stats;
        }

        private List<DashboardStat> GetAdminStats(SqlConnection conn)
        {
            var stats = new List<DashboardStat>();

            // 1. Total Employees
            string totalEmployeesQuery = @"
                SELECT COUNT(*) FROM Employees e 
                INNER JOIN Users u ON e.UserId = u.Id 
                WHERE u.IsActive = 1";
            var totalEmployees = ExecuteScalarQuery(conn, totalEmployeesQuery);
            stats.Add(new DashboardStat("total_employees", "Total Employees", totalEmployees.ToString(), "primary", "people", "Active employees"));

            // 2. Pending Approvals
            string pendingApprovalsQuery = @"
                SELECT 
                (SELECT COUNT(*) FROM LeaveRequests WHERE Status = 'Pending') +
                (SELECT COUNT(*) FROM WorkflowApprovals WHERE Status = 'PENDING')";
            var pendingApprovals = ExecuteScalarQuery(conn, pendingApprovalsQuery);
            stats.Add(new DashboardStat("pending_approvals", "Pending Approvals", pendingApprovals.ToString(), "warning", "assignment", "Needs attention"));

            // 3. Active Shifts Today
            string activeShiftsQuery = @"
                SELECT COUNT(*) FROM TimeEntries 
                WHERE Status = 'Active' AND CAST(CreatedAt AS DATE) = CAST(GETDATE() AS DATE)";
            var activeShifts = ExecuteScalarQuery(conn, activeShiftsQuery);
            stats.Add(new DashboardStat("active_shifts", "Active Shifts", activeShifts.ToString(), "info", "schedule", "Currently running"));

            // 4. System Health
            string systemHealthQuery = @"
                SELECT COUNT(*) FROM ErrorLogs 
                WHERE CreatedAt >= DATEADD(HOUR, -1, GETUTCDATE()) AND IsResolved = 0";
            var errorCount = ExecuteScalarQuery(conn, systemHealthQuery);
            string healthValue = errorCount == 0 ? "100%" : "98%";
            string healthColor = errorCount == 0 ? "success" : "warning";
            stats.Add(new DashboardStat("system_health", "System Health", healthValue, healthColor, "security", "System operational"));

            return stats;
        }

        private List<DashboardStat> GetHRAdminStats(SqlConnection conn)
        {
            var stats = new List<DashboardStat>();

            // 1. Total Employees
            string totalEmployeesQuery = @"
                SELECT COUNT(*) FROM Employees e 
                INNER JOIN Users u ON e.UserId = u.Id 
                WHERE u.IsActive = 1";
            var totalEmployees = ExecuteScalarQuery(conn, totalEmployeesQuery);
            stats.Add(new DashboardStat("hr_total_employees", "Total Employees", totalEmployees.ToString(), "primary", "people", "All departments"));

            // 2. Pending Leave Requests
            string pendingLeaveQuery = "SELECT COUNT(*) FROM LeaveRequests WHERE Status = 'Pending'";
            var pendingLeave = ExecuteScalarQuery(conn, pendingLeaveQuery);
            stats.Add(new DashboardStat("hr_pending_leave", "Pending Leave Requests", pendingLeave.ToString(), "warning", "warning", "Awaiting review"));

            // 3. New Hires This Month
            string newHiresQuery = @"
                SELECT COUNT(*) FROM Employees e 
                INNER JOIN Users u ON e.UserId = u.Id 
                WHERE u.IsActive = 1 AND e.HireDate >= DATEADD(MONTH, -1, GETDATE())";
            var newHires = ExecuteScalarQuery(conn, newHiresQuery);
            stats.Add(new DashboardStat("hr_new_hires", "New Hires This Month", newHires.ToString(), "success", "trending_up", "Recent additions"));

            // 4. Onboarding Tasks
            string onboardingTasksQuery = @"
                SELECT COUNT(*) FROM OnboardingTasks ot 
                INNER JOIN OnboardingChecklists oc ON ot.ChecklistId = oc.Id 
                WHERE ot.Status = 'PENDING' AND ot.IsMandatory = 1";
            var onboardingTasks = ExecuteScalarQuery(conn, onboardingTasksQuery);
            stats.Add(new DashboardStat("hr_onboarding_tasks", "Onboarding Tasks", onboardingTasks.ToString(), "info", "assignment", "Active checklist items"));

            return stats;
        }

        private List<DashboardStat> GetProgramDirectorStats(SqlConnection conn, int userId)
        {
            var stats = new List<DashboardStat>();

            // Get employee ID for the user
            int employeeId = GetEmployeeIdByUserId(conn, userId);

            // 1. Direct Reports
            string directReportsQuery = @"
                SELECT COUNT(*) FROM Employees e 
                INNER JOIN Users u ON e.UserId = u.Id 
                WHERE e.ManagerId = @EmployeeId AND u.IsActive = 1";
            var directReports = ExecuteScalarQuery(conn, directReportsQuery, new SqlParameter("@EmployeeId", employeeId));
            stats.Add(new DashboardStat("mgr_direct_reports", "Direct Reports", directReports.ToString(), "primary", "people", "Team members"));

            // 2. Team Leave Requests
            string teamLeaveQuery = @"
                SELECT COUNT(*) FROM LeaveRequests lr 
                INNER JOIN Employees e ON lr.EmployeeId = e.Id 
                WHERE e.ManagerId = @EmployeeId AND lr.Status = 'Pending'";
            var teamLeave = ExecuteScalarQuery(conn, teamLeaveQuery, new SqlParameter("@EmployeeId", employeeId));
            stats.Add(new DashboardStat("mgr_team_leave_requests", "Team Leave Requests", teamLeave.ToString(), "warning", "warning", "Pending approval"));

            // 3. Team Attendance Today
            string attendanceQuery = @"
                SELECT 
                    CASE 
                        WHEN COUNT(DISTINCT e.Id) = 0 THEN 100
                        ELSE (COUNT(DISTINCT te.EmployeeId) * 100 / COUNT(DISTINCT e.Id))
                    END
                FROM Employees e 
                INNER JOIN Users u ON e.UserId = u.Id 
                LEFT JOIN TimeEntries te ON e.Id = te.EmployeeId AND CAST(te.CreatedAt AS DATE) = CAST(GETDATE() AS DATE)
                WHERE e.ManagerId = @EmployeeId AND u.IsActive = 1";
            var attendance = ExecuteScalarQuery(conn, attendanceQuery, new SqlParameter("@EmployeeId", employeeId));
            stats.Add(new DashboardStat("mgr_team_attendance", "Team Attendance", $"{attendance}%", "success", "check_circle", "Today"));

            // 4. Completed Tasks (Last 30 days)
            string completedTasksQuery = @"
                SELECT COUNT(*) FROM OnboardingTasks ot 
                INNER JOIN OnboardingChecklists oc ON ot.ChecklistId = oc.Id 
                INNER JOIN Employees e ON oc.EmployeeId = e.Id 
                WHERE e.ManagerId = @EmployeeId 
                    AND ot.Status = 'COMPLETED' 
                    AND ot.CompletedAt >= DATEADD(MONTH, -1, GETDATE())";
            var completedTasks = ExecuteScalarQuery(conn, completedTasksQuery, new SqlParameter("@EmployeeId", employeeId));
            stats.Add(new DashboardStat("mgr_completed_tasks", "Completed Tasks", completedTasks.ToString(), "success", "check_circle", "Last 30 days"));

            return stats;
        }

        private List<DashboardStat> GetProgramCoordinatorStats(SqlConnection conn, int userId)
        {
            var stats = new List<DashboardStat>();
            int employeeId = GetEmployeeIdByUserId(conn, userId);

            // 1. My Tasks
            string myTasksQuery = @"
                SELECT COUNT(*) FROM OnboardingTasks ot 
                INNER JOIN OnboardingChecklists oc ON ot.ChecklistId = oc.Id 
                WHERE ot.AssignedToId = @EmployeeId 
                    AND ot.Status IN ('PENDING', 'IN_PROGRESS')";
            var myTasks = ExecuteScalarQuery(conn, myTasksQuery, new SqlParameter("@EmployeeId", employeeId));
            stats.Add(new DashboardStat("coord_my_tasks", "My Tasks", myTasks.ToString(), "warning", "assignment", "Active assignments"));

            // 2. Completed Today
            string completedTodayQuery = @"
                SELECT COUNT(*) FROM OnboardingTasks ot 
                INNER JOIN OnboardingChecklists oc ON ot.ChecklistId = oc.Id 
                WHERE ot.AssignedToId = @EmployeeId 
                    AND ot.Status = 'COMPLETED' 
                    AND CAST(ot.CompletedAt AS DATE) = CAST(GETDATE() AS DATE)";
            var completedToday = ExecuteScalarQuery(conn, completedTodayQuery, new SqlParameter("@EmployeeId", employeeId));
            stats.Add(new DashboardStat("coord_completed_today", "Completed Today", completedToday.ToString(), "success", "check_circle", "Tasks finished"));

            // 3. Overdue Tasks
            string overdueTasksQuery = @"
                SELECT COUNT(*) FROM OnboardingTasks ot 
                INNER JOIN OnboardingChecklists oc ON ot.ChecklistId = oc.Id 
                WHERE ot.AssignedToId = @EmployeeId 
                    AND ot.Status IN ('PENDING', 'IN_PROGRESS') 
                    AND ot.DueDate < CAST(GETDATE() AS DATE)";
            var overdueTasks = ExecuteScalarQuery(conn, overdueTasksQuery, new SqlParameter("@EmployeeId", employeeId));
            stats.Add(new DashboardStat("coord_overdue_tasks", "Overdue Tasks", overdueTasks.ToString(), "danger", "warning", "Past due date"));

            // 4. Hours This Month
            string hoursQuery = @"
                SELECT ISNULL(SUM(ISNULL(HoursWorked, 0)), 0) 
                FROM TimeEntries 
                WHERE EmployeeId = @EmployeeId 
                    AND MONTH(CreatedAt) = MONTH(GETDATE()) 
                    AND YEAR(CreatedAt) = YEAR(GETDATE())";
            var hours = ExecuteScalarQuery(conn, hoursQuery, new SqlParameter("@EmployeeId", employeeId));
            stats.Add(new DashboardStat("coord_hours_month", "Hours This Month", hours.ToString(), "info", "access_time", "Total logged"));

            return stats;
        }

        private List<DashboardStat> GetEmployeeStats(SqlConnection conn, int userId)
        {
            var stats = new List<DashboardStat>();
            int employeeId = GetEmployeeIdByUserId(conn, userId);

            // 1. PTO Balance
            string ptoBalanceQuery = @"
                SELECT ISNULL(SUM(AvailableDays), 0) 
                FROM LeaveBalances 
                WHERE EmployeeId = @EmployeeId";
            var ptoBalance = ExecuteScalarQuery(conn, ptoBalanceQuery, new SqlParameter("@EmployeeId", employeeId));
            stats.Add(new DashboardStat("emp_pto_balance", "PTO Balance", $"{ptoBalance} days", "success", "schedule", "Available this year"));

            // 2. Hours This Week
            string hoursWeekQuery = @"
                SELECT ISNULL(SUM(ISNULL(HoursWorked, 0)), 0) 
                FROM TimeEntries 
                WHERE EmployeeId = @EmployeeId 
                    AND CreatedAt >= DATEADD(day, -(DATEPART(weekday, GETDATE()) - 1), CAST(GETDATE() AS DATE))";
            var hoursWeek = ExecuteScalarQuery(conn, hoursWeekQuery, new SqlParameter("@EmployeeId", employeeId));
            stats.Add(new DashboardStat("emp_hours_week", "Hours This Week", hoursWeek.ToString(), "info", "access_time", "Current week total"));

            // 3. My Pending Requests
            string pendingRequestsQuery = @"
                SELECT COUNT(*) FROM LeaveRequests 
                WHERE EmployeeId = @EmployeeId AND Status = 'Pending'";
            var pendingRequests = ExecuteScalarQuery(conn, pendingRequestsQuery, new SqlParameter("@EmployeeId", employeeId));
            stats.Add(new DashboardStat("emp_pending_requests", "My Pending Requests", pendingRequests.ToString(), "warning", "warning", "Awaiting approval"));

            // 4. Completed Tasks (Last 30 days)
            string completedTasksQuery = @"
                SELECT COUNT(*) FROM OnboardingTasks ot 
                INNER JOIN OnboardingChecklists oc ON ot.ChecklistId = oc.Id 
                WHERE oc.EmployeeId = @EmployeeId 
                    AND ot.Status = 'COMPLETED' 
                    AND ot.CompletedAt >= DATEADD(MONTH, -1, GETDATE())";
            var completedTasks = ExecuteScalarQuery(conn, completedTasksQuery, new SqlParameter("@EmployeeId", employeeId));
            stats.Add(new DashboardStat("emp_completed_tasks", "Completed Tasks", completedTasks.ToString(), "primary", "check_circle", "Last 30 days"));

            return stats;
        }

        #endregion

        #region AJAX Refresh Methods

        /// <summary>
        /// Handle AJAX request to refresh only dashboard stats
        /// </summary>
        private void RefreshDashboardStatsOnly()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                string userRole = Session["UserRole"]?.ToString() ?? "";

                // Get fresh stats
                List<DashboardStat> stats = GetRealTimeStatsByRole(userRole, userId);

                Response.Clear();
                Response.ContentType = "text/html";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                // Generate HTML for stats
                var statsHtml = new StringBuilder();
                if (stats.Count == 4)
                {
                    foreach (var stat in stats)
                    {
                        statsHtml.AppendLine(CreateStatCard(stat.Name, stat.Value, stat.Icon, stat.Color, stat.Subtitle, stat.Key));
                    }
                }
                else
                {
                    // Fallback if we don't get exactly 4 stats
                    statsHtml.AppendLine(CreateFallbackStats(userRole));
                }

                // Wrap in container for easy replacement
                Response.Write($"<div id=\"dashboard-stats-content\">{statsHtml.ToString()}</div>");
                Response.End();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing stats: {ex.Message}");
                Response.StatusCode = 500;
                Response.Write("Error refreshing statistics");
                Response.End();
            }
        }

        /// <summary>
        /// Handle AJAX request to refresh only recent activities
        /// </summary>
        private void RefreshActivitiesOnly()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);

                // Get fresh activities HTML
                var activitiesHtml = GetRecentActivitiesHtml();

                Response.Clear();
                Response.ContentType = "text/html";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                Response.Write($"<div id=\"recent-activities-content\">{activitiesHtml}</div>");
                Response.End();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing activities: {ex.Message}");
                Response.StatusCode = 500;
                Response.Write("Error refreshing activities");
                Response.End();
            }
        }

        /// <summary>
        /// Get recent activities HTML
        /// </summary>
        private string GetRecentActivitiesHtml()
        {
            try
            {
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
                            u.Email,
                            u.Role
                        FROM RecentActivities ra
                        INNER JOIN Users u ON ra.UserId = u.Id
                        WHERE u.IsActive = 1
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
                                string userEmail = reader["Email"].ToString();
                                string userRole = reader["Role"].ToString();

                                activitiesHtml.AppendLine(CreateActivityItem(action, entityType, details, createdAt, userEmail, userRole));
                            }

                            // If no activities, show default message
                            if (activitiesHtml.Length == 0)
                            {
                                activitiesHtml.AppendLine("<li class='activity-item'>No recent activities found.</li>");
                            }

                            return activitiesHtml.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting activities HTML: {ex.Message}");
                return "<li class='activity-item'>Unable to load recent activities.</li>";
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Execute a scalar query and return the result as integer
        /// </summary>
        private int ExecuteScalarQuery(SqlConnection conn, string query, params SqlParameter[] parameters)
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                object result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
        }

        /// <summary>
        /// Get employee ID for a user
        /// </summary>
        private int GetEmployeeIdByUserId(SqlConnection conn, int userId)
        {
            string query = "SELECT ISNULL(Id, 0) FROM Employees WHERE UserId = @UserId";
            return ExecuteScalarQuery(conn, query, new SqlParameter("@UserId", userId));
        }

        /// <summary>
        /// Dashboard stat data structure
        /// </summary>
        public class DashboardStat
        {
            public string Key { get; set; }
            public string Name { get; set; }
            public string Value { get; set; }
            public string Color { get; set; }
            public string Icon { get; set; }
            public string Subtitle { get; set; }

            public DashboardStat(string key, string name, string value, string color, string icon, string subtitle)
            {
                Key = key;
                Name = name;
                Value = value;
                Color = color;
                Icon = icon;
                Subtitle = subtitle;
            }
        }

        /// <summary>
        /// Create fallback stats if database queries fail
        /// </summary>
        private string CreateFallbackStats(string userRole)
        {
            var stats = new StringBuilder();

            switch (userRole.ToUpper())
            {
                case "SUPERADMIN":
                    stats.AppendLine(CreateStatCard("Total Users", "Loading...", "people", "primary", "Active system users", "total_users"));
                    stats.AppendLine(CreateStatCard("Active Sessions", "Loading...", "schedule", "success", "Last 24 hours", "active_sessions"));
                    stats.AppendLine(CreateStatCard("Total Departments", "Loading...", "analytics", "info", "Active departments", "total_departments"));
                    stats.AppendLine(CreateStatCard("Logins Today", "Loading...", "login", "warning", "User activity today", "recent_logins"));
                    break;

                case "ADMIN":
                    stats.AppendLine(CreateStatCard("Total Employees", "Loading...", "people", "primary", "Active employees", "total_employees"));
                    stats.AppendLine(CreateStatCard("Pending Approvals", "Loading...", "assignment", "warning", "Needs attention", "pending_approvals"));
                    stats.AppendLine(CreateStatCard("Active Shifts", "Loading...", "schedule", "info", "Currently running", "active_shifts"));
                    stats.AppendLine(CreateStatCard("System Health", "Loading...", "security", "success", "System operational", "system_health"));
                    break;

                case "HRADMIN":
                    stats.AppendLine(CreateStatCard("Total Employees", "Loading...", "people", "primary", "All departments", "hr_total_employees"));
                    stats.AppendLine(CreateStatCard("Pending Leave Requests", "Loading...", "warning", "warning", "Awaiting review", "hr_pending_leave"));
                    stats.AppendLine(CreateStatCard("New Hires This Month", "Loading...", "trending_up", "success", "Recent additions", "hr_new_hires"));
                    stats.AppendLine(CreateStatCard("Onboarding Tasks", "Loading...", "assignment", "info", "Active checklist items", "hr_onboarding_tasks"));
                    break;

                case "PROGRAMDIRECTOR":
                    stats.AppendLine(CreateStatCard("Direct Reports", "Loading...", "people", "primary", "Team members", "mgr_direct_reports"));
                    stats.AppendLine(CreateStatCard("Team Leave Requests", "Loading...", "warning", "warning", "Pending approval", "mgr_team_leave_requests"));
                    stats.AppendLine(CreateStatCard("Team Attendance", "Loading...", "check_circle", "success", "Today", "mgr_team_attendance"));
                    stats.AppendLine(CreateStatCard("Completed Tasks", "Loading...", "check_circle", "success", "Last 30 days", "mgr_completed_tasks"));
                    break;

                case "PROGRAMCOORDINATOR":
                    stats.AppendLine(CreateStatCard("My Tasks", "Loading...", "assignment", "warning", "Active assignments", "coord_my_tasks"));
                    stats.AppendLine(CreateStatCard("Completed Today", "Loading...", "check_circle", "success", "Tasks finished", "coord_completed_today"));
                    stats.AppendLine(CreateStatCard("Overdue Tasks", "Loading...", "warning", "danger", "Past due date", "coord_overdue_tasks"));
                    stats.AppendLine(CreateStatCard("Hours This Month", "Loading...", "access_time", "info", "Total logged", "coord_hours_month"));
                    break;

                case "EMPLOYEE":
                default:
                    stats.AppendLine(CreateStatCard("PTO Balance", "Loading...", "schedule", "success", "Available this year", "emp_pto_balance"));
                    stats.AppendLine(CreateStatCard("Hours This Week", "Loading...", "access_time", "info", "Current week total", "emp_hours_week"));
                    stats.AppendLine(CreateStatCard("My Pending Requests", "Loading...", "warning", "warning", "Awaiting approval", "emp_pending_requests"));
                    stats.AppendLine(CreateStatCard("Completed Tasks", "Loading...", "check_circle", "primary", "Last 30 days", "emp_completed_tasks"));
                    break;
            }

            return stats.ToString();
        }

        /// <summary>
        /// Create a beautiful stat card with enhanced styling and data-attributes for real-time updates
        /// </summary>
        private string CreateStatCard(string title, string value, string icon, string color, string subtitle, string statKey = "")
        {
            string colorClass = GetColorClass(color);
            string dataAttribute = !string.IsNullOrEmpty(statKey) ? $"data-stat-key=\"{statKey}\"" : "";

            // Add number formatting for better display
            string formattedValue = FormatStatValue(value);

            // Determine if this is a trend value (contains % or trend indicators)
            bool isTrend = value.Contains("%") || value.Contains("↑") || value.Contains("↓");
            string trendClass = isTrend ? "trend-value" : "";

            return $@"
                <div class='stat-card {trendClass}' {dataAttribute}>
                    <div class='stat-icon {colorClass}'>
                        <i class='material-icons'>{icon}</i>
                    </div>
                    <div class='stat-content'>
                        <div class='stat-value {(value == "Loading..." ? "loading-text" : "")}'>{formattedValue}</div>
                        <div class='stat-title'>{title}</div>
                        <div class='stat-subtitle'>{subtitle}</div>
                    </div>
                    {(value != "Loading..." ? "<div class='stat-update-indicator'></div>" : "")}
                </div>";
        }

        /// <summary>
        /// Format stat values for better display
        /// </summary>
        private string FormatStatValue(string value)
        {
            // If it's loading, return as-is
            if (value == "Loading..." || string.IsNullOrEmpty(value))
                return value;

            // If it contains non-numeric characters (like %), return as-is
            if (value.Contains("%") || value.Contains("days") || !char.IsDigit(value[0]))
                return value;

            // Try to parse as number for formatting
            if (int.TryParse(value, out int numValue))
            {
                if (numValue >= 1000000)
                    return $"{(numValue / 1000000.0):F1}M";
                else if (numValue >= 1000)
                    return $"{(numValue / 1000.0):F1}K";
                else
                    return numValue.ToString();
            }

            return value;
        }

        #endregion

        #region Existing Methods (Quick Actions, Recent Activities, etc.)

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
                                string color = reader["Color"]?.ToString() ?? "primary";

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
                            u.Email,
                            u.Role
                        FROM RecentActivities ra
                        INNER JOIN Users u ON ra.UserId = u.Id
                        WHERE u.IsActive = 1
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
                                string userEmail = reader["Email"].ToString();
                                string userRole = reader["Role"].ToString();

                                activitiesHtml.AppendLine(CreateActivityItem(action, entityType, details, createdAt, userEmail, userRole));
                            }

                            // If no activities, show default message
                            if (activitiesHtml.Length == 0)
                            {
                                activitiesHtml.AppendLine("<li class='activity-item'>No recent activities found.</li>");
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
                    litRecentActivities.Text = "<li class='activity-item'>Unable to load recent activities.</li>";
                }
            }
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
                    actions.AppendLine(CreateQuickAction("Leave Requests", "Review pending requests", "event_available", "/leave/requests", "leave-requests"));
                    actions.AppendLine(CreateQuickAction("Employee Reports", "Generate HR reports", "assessment", "/reports/hr", "hr-reports"));
                    actions.AppendLine(CreateQuickAction("Onboarding", "Manage new hire tasks", "assignment", "/onboarding", "onboarding"));
                    break;

                case "PROGRAMDIRECTOR":
                    actions.AppendLine(CreateQuickAction("Team Overview", "View team status", "people", "/team/overview", "team-overview"));
                    actions.AppendLine(CreateQuickAction("Approve Requests", "Review team requests", "assignment", "/approvals/team", "team-approvals"));
                    actions.AppendLine(CreateQuickAction("Schedule Meeting", "Plan team meeting", "event", "/meetings/schedule", "schedule-meeting"));
                    actions.AppendLine(CreateQuickAction("Performance Review", "Evaluate team members", "assessment", "/performance", "performance-review"));
                    break;

                case "PROGRAMCOORDINATOR":
                    actions.AppendLine(CreateQuickAction("My Tasks", "View assigned tasks", "assignment", "/tasks/my", "my-tasks"));
                    actions.AppendLine(CreateQuickAction("Time Entry", "Log work hours", "access_time", "/time/entry", "time-entry"));
                    actions.AppendLine(CreateQuickAction("Submit Report", "Daily activity report", "description", "/reports/daily", "daily-report"));
                    actions.AppendLine(CreateQuickAction("Request Leave", "Submit time off request", "event_available", "/leave/request", "request-leave"));
                    break;

                case "EMPLOYEE":
                default:
                    actions.AppendLine(CreateQuickAction("Time Entry", "Log work hours", "access_time", "/time/entry", "time-entry"));
                    actions.AppendLine(CreateQuickAction("Request Leave", "Submit time off request", "event_available", "/leave/request", "request-leave"));
                    actions.AppendLine(CreateQuickAction("My Profile", "Update personal info", "person", "/profile", "my-profile"));
                    actions.AppendLine(CreateQuickAction("Submit Report", "Daily activity report", "description", "/reports/daily", "daily-report"));
                    break;
            }

            return actions.ToString();
        }

        private string CreateQuickAction(string title, string description, string icon, string route, string actionKey)
        {
            return $@"
                <div class='quick-action' data-action-key='{actionKey}' onclick='window.location.href=""{route}""'>
                    <div class='action-icon'>
                        <i class='material-icons'>{icon}</i>
                    </div>
                    <div class='action-content'>
                        <div class='action-title'>{title}</div>
                        <div class='action-description'>{description}</div>
                    </div>
                </div>";
        }

        private string CreateActivityItem(string action, string entityType, string details, DateTime createdAt, string userEmail, string userRole)
        {
            string timeAgo = GetTimeAgo(createdAt);
            string userInitials = GetUserInitials(userEmail);
            string color = GetRoleColor(userRole);
            bool isNew = createdAt > DateTime.UtcNow.AddMinutes(-30);
            string newBadge = isNew ? "<span class='activity-new'>NEW</span>" : "";
            string userName = userEmail.Split('@')[0];

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

        private string GetColorClass(string color)
        {
            switch (color.ToLower())
            {
                case "primary": return "stat-primary";
                case "success": return "stat-success";
                case "warning": return "stat-warning";
                case "danger": return "stat-danger";
                case "info": return "stat-info";
                case "secondary": return "stat-secondary";
                default: return "stat-primary";
            }
        }

        private string GetRoleColor(string role)
        {
            switch (role.ToUpper())
            {
                case "SUPERADMIN": return "#e91e63";
                case "ADMIN": return "#2196f3";
                case "HRADMIN": return "#ff9800";
                case "PROGRAMDIRECTOR": return "#4caf50";
                case "PROGRAMCOORDINATOR": return "#9c27b0";
                case "EMPLOYEE": return "#607d8b";
                default: return "#757575";
            }
        }

        private string GetUserInitials(string email)
        {
            if (string.IsNullOrEmpty(email)) return "??";

            var parts = email.Split('@')[0].Split('.');
            if (parts.Length >= 2)
            {
                return (parts[0].Substring(0, 1) + parts[1].Substring(0, 1)).ToUpper();
            }
            else
            {
                return email.Substring(0, Math.Min(2, email.Length)).ToUpper();
            }
        }

        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Just now";
            else if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes}m ago";
            else if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours}h ago";
            else if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays}d ago";
            else
                return dateTime.ToString("MMM dd");
        }

        private void LoadNavigationMenu(string userRole)
        {
            var litNavigation = FindControlRecursive(Page, "litNavigation") as System.Web.UI.WebControls.Literal;
            if (litNavigation == null) return;

            var menuHtml = new StringBuilder();

            // Dashboard (everyone)
            menuHtml.AppendLine(CreateNavItem("Dashboard", "/Dashboard.aspx", "dashboard"));

            // Employees Management
            if (HasAccess(userRole, "Admin", "HR", "SuperAdmin", "HRAdmin", "ProgramDirector"))
            {
                menuHtml.AppendLine(CreateNavItem("Employees", "/employees", "people"));
            }

            // Time Management
            if (HasAccess(userRole, "Admin", "HR", "SuperAdmin", "HRAdmin", "ProgramDirector", "ProgramCoordinator", "Employee"))
            {
                menuHtml.AppendLine(CreateNavItem("Time Tracking", "/time", "access_time"));
            }

            // Leave Management
            if (HasAccess(userRole, "Admin", "HR", "SuperAdmin", "HRAdmin", "ProgramDirector", "ProgramCoordinator", "Employee"))
            {
                menuHtml.AppendLine(CreateNavItem("Leave Management", "/leave", "event_available"));
            }

            // Reports
            if (HasAccess(userRole, "Admin", "HR", "SuperAdmin", "HRAdmin", "ProgramDirector"))
            {
                menuHtml.AppendLine(CreateNavItem("Reports", "/reports", "assessment"));
            }

            // Administration
            if (HasAccess(userRole, "Admin", "SuperAdmin"))
            {
                menuHtml.AppendLine(CreateNavItem("Administration", "/admin", "settings"));
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

        private string CreateNavItem(string title, string route, string icon)
        {
            string activeClass = IsCurrentPage(route) ? "nav-active" : "";
            return $@"
                <li class='nav-item {activeClass}'>
                    <a href='{route}' class='nav-link'>
                        <i class='material-icons'>{icon}</i>
                        <span>{title}</span>
                    </a>
                </li>";
        }

        private bool IsCurrentPage(string route)
        {
            if (string.IsNullOrEmpty(route)) return false;

            try
            {
                string currentPath = Request.Url.AbsolutePath.ToLower();
                string cleanRoute = route.TrimStart('/').ToLower();

                return currentPath.Contains(cleanRoute) || currentPath.EndsWith($"/{cleanRoute}");
            }
            catch
            {
                return false;
            }
        }

        private bool HasAccess(string userRole, params string[] allowedRoles)
        {
            return allowedRoles.Any(role => string.Equals(role, userRole, StringComparison.OrdinalIgnoreCase));
        }

        private void SetUserInformation(string userName, string userRole)
        {
            var litUserName = FindControlRecursive(Page, "litUserName") as System.Web.UI.WebControls.Literal;
            var litUserRole = FindControlRecursive(Page, "litUserRole") as System.Web.UI.WebControls.Literal;
            var litUserInitial = FindControlRecursive(Page, "litUserInitial") as System.Web.UI.WebControls.Literal;
            var litHeaderUserInitial = FindControlRecursive(Page, "litHeaderUserInitial") as System.Web.UI.WebControls.Literal;

            if (litUserName != null) litUserName.Text = userName;
            if (litUserRole != null) litUserRole.Text = userRole;

            string initials = GetUserInitials(userName);
            if (litUserInitial != null) litUserInitial.Text = initials;
            if (litHeaderUserInitial != null) litHeaderUserInitial.Text = initials;
        }

        private void LogUserActivity(int userId, string action, string entityType, string details, string ipAddress)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO RecentActivities (UserId, Action, EntityType, Details, IPAddress, CreatedAt, ActivityTypeId)
                        VALUES (@UserId, @Action, @EntityType, @Details, @IPAddress, @CreatedAt, 1)";

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
            string ip = Request.Headers["X-Forwarded-For"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.Headers["X-Real-IP"];
            }
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.UserHostAddress;
            }
            return ip ?? "Unknown";
        }

        private void ShowError(string message)
        {
            System.Diagnostics.Debug.WriteLine($"Dashboard Error: {message}");
        }

        private Control FindControlRecursive(Control root, string id)
        {
            if (root.ID == id)
                return root;

            foreach (Control control in root.Controls)
            {
                Control found = FindControlRecursive(control, id);
                if (found != null)
                    return found;
            }

            return null;
        }

        #endregion
    }
}