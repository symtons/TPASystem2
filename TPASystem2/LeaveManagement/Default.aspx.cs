using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace TPASystem2.LeaveManagement
{
    public partial class Default : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in
            //if (Session["UserId"] == null)
            //{
            //    Response.Redirect("~/Login.aspx", false);
            //    return;
            //}

            if (!IsPostBack)
            {
                LoadDashboardData();
            }

            // Handle refresh postback
            if (Request.Form["__EVENTTARGET"] == Page.ClientID && Request.Form["__EVENTARGUMENT"] == "Refresh")
            {
                LoadDashboardData();
            }
        }

        #region Data Loading Methods

        private void LoadDashboardData()
        {
            try
            {
                LoadDashboardStats();
                LoadRecentRequests();
                LoadLeaveBalances();
                LoadUpcomingLeaves();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading dashboard data: {ex.Message}", "error");
                LogError(ex);
            }
        }

        private void LoadDashboardStats()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get current user role to determine what stats to show
                    string userRole = Session["UserRole"]?.ToString() ?? "";
                    int currentUserId = Convert.ToInt32(Session["UserId"]);

                    // Base query - modify based on user role
                    string whereClause = "";
                    if (userRole != "SUPERADMIN" && userRole != "HRADMIN")
                    {
                        // For non-admin users, show stats for their department or direct reports
                        whereClause = GetUserAccessFilter(currentUserId);
                    }

                    string statsQuery = $@"
                        SELECT 
                            COUNT(CASE WHEN lr.Status = 'Pending' THEN 1 END) as PendingRequests,
                            COUNT(CASE WHEN lr.Status = 'Approved' AND CAST(lr.ReviewedAt AS DATE) = CAST(GETDATE() AS DATE) THEN 1 END) as ApprovedToday,
                            COUNT(CASE WHEN lr.Status = 'Approved' AND lr.StartDate <= CAST(GETDATE() AS DATE) AND lr.EndDate >= CAST(GETDATE() AS DATE) THEN 1 END) as OnLeaveToday,
                            COUNT(CASE WHEN lr.Status = 'Approved' AND lr.StartDate BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(day, 7, CAST(GETDATE() AS DATE)) THEN 1 END) as UpcomingThisWeek
                        FROM LeaveRequests lr
                        INNER JOIN Employees e ON lr.EmployeeId = e.Id
                        {whereClause}";

                    using (SqlCommand cmd = new SqlCommand(statsQuery, conn))
                    {
                        if (userRole != "SUPERADMIN" && userRole != "HRADMIN")
                        {
                            cmd.Parameters.AddWithValue("@UserId", currentUserId);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litPendingRequests.Text = reader["PendingRequests"].ToString();
                                litApprovedToday.Text = reader["ApprovedToday"].ToString();
                                litOnLeaveToday.Text = reader["OnLeaveToday"].ToString();
                                litUpcomingLeaves.Text = reader["UpcomingThisWeek"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                // Set default values on error
                litPendingRequests.Text = "0";
                litApprovedToday.Text = "0";
                litOnLeaveToday.Text = "0";
                litUpcomingLeaves.Text = "0";
            }
        }

        private void LoadRecentRequests()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string userRole = Session["UserRole"]?.ToString() ?? "";
                    int currentUserId = Convert.ToInt32(Session["UserId"]);
                    string whereClause = GetUserAccessFilter(currentUserId);

                    // Add status filter if selected
                    string statusFilter = ddlRequestFilter.SelectedValue;
                    if (!string.IsNullOrEmpty(statusFilter))
                    {
                        whereClause += (string.IsNullOrEmpty(whereClause) ? " WHERE " : " AND ") + "lr.Status = @StatusFilter";
                    }

                    string query = $@"
                        SELECT TOP 10
                            lr.Id,
                            lr.LeaveType,
                            lr.StartDate,
                            lr.EndDate,
                            lr.DaysRequested,
                            lr.Status,
                            lr.RequestedAt,
                            CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName,
                            d.Name as Department
                        FROM LeaveRequests lr
                        INNER JOIN Employees e ON lr.EmployeeId = e.Id
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        {whereClause}
                        ORDER BY lr.RequestedAt DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (userRole != "SUPERADMIN" && userRole != "HRADMIN")
                        {
                            cmd.Parameters.AddWithValue("@UserId", currentUserId);
                        }

                        if (!string.IsNullOrEmpty(statusFilter))
                        {
                            cmd.Parameters.AddWithValue("@StatusFilter", statusFilter);
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            gvRecentRequests.DataSource = dt;
                            gvRecentRequests.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading recent requests.", "error");
            }
        }

        private void LoadLeaveBalances()
        {
            try
            {
                string userRole = Session["UserRole"]?.ToString() ?? "";
                int currentUserId = Convert.ToInt32(Session["UserId"]);

                // For now, show sample data - in production this would come from leave balance tables
                var balanceData = new List<LeaveBalance>();

                if (userRole == "SUPERADMIN" || userRole == "HRADMIN")
                {
                    // Show company-wide averages for admins
                    balanceData.Add(new LeaveBalance { LeaveType = "Vacation", Available = 120, Used = 45, Total = 165 });
                    balanceData.Add(new LeaveBalance { LeaveType = "Sick", Available = 85, Used = 25, Total = 110 });
                    balanceData.Add(new LeaveBalance { LeaveType = "Personal", Available = 35, Used = 15, Total = 50 });
                }
                else
                {
                    // Show individual balances for regular users
                    balanceData.Add(new LeaveBalance { LeaveType = "Vacation", Available = 12, Used = 8, Total = 20 });
                    balanceData.Add(new LeaveBalance { LeaveType = "Sick", Available = 8, Used = 2, Total = 10 });
                    balanceData.Add(new LeaveBalance { LeaveType = "Personal", Available = 3, Used = 2, Total = 5 });
                }

                if (balanceData.Count > 0)
                {
                    rptLeaveBalances.DataSource = balanceData;
                    rptLeaveBalances.DataBind();
                    pnlNoBalances.Visible = false;
                }
                else
                {
                    pnlNoBalances.Visible = true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                pnlNoBalances.Visible = true;
            }
        }

        private void LoadUpcomingLeaves()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string userRole = Session["UserRole"]?.ToString() ?? "";
                    int currentUserId = Convert.ToInt32(Session["UserId"]);
                    string whereClause = GetUserAccessFilter(currentUserId);

                    int daysAhead = Convert.ToInt32(ddlUpcomingFilter.SelectedValue);

                    string query = $@"
                        SELECT 
                            lr.Id,
                            lr.LeaveType,
                            lr.StartDate,
                            lr.EndDate,
                            lr.DaysRequested,
                            lr.Status,
                            CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName,
                            d.Name as Department
                        FROM LeaveRequests lr
                        INNER JOIN Employees e ON lr.EmployeeId = e.Id
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        {whereClause}
                        {(string.IsNullOrEmpty(whereClause) ? "WHERE" : "AND")} lr.Status = 'Approved' 
                        AND lr.StartDate BETWEEN CAST(GETDATE() AS DATE) AND DATEADD(day, @DaysAhead, CAST(GETDATE() AS DATE))
                        ORDER BY lr.StartDate ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (userRole != "SUPERADMIN" && userRole != "HRADMIN")
                        {
                            cmd.Parameters.AddWithValue("@UserId", currentUserId);
                        }
                        cmd.Parameters.AddWithValue("@DaysAhead", daysAhead);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            gvUpcomingLeaves.DataSource = dt;
                            gvUpcomingLeaves.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading upcoming leaves.", "error");
            }
        }

        private string GetUserAccessFilter(int userId)
        {
            string userRole = Session["UserRole"]?.ToString() ?? "";

            switch (userRole)
            {
                case "SUPERADMIN":
                case "HRADMIN":
                    return ""; // No filter - see all

                case "MANAGER":
                    // Managers see their direct reports
                    return @" WHERE e.ManagerId IN (
                        SELECT Id FROM Employees WHERE UserId = @UserId
                    ) OR lr.EmployeeId IN (
                        SELECT Id FROM Employees WHERE UserId = @UserId
                    )";

                default:
                    // Regular employees see only their own requests
                    return " WHERE lr.EmployeeId IN (SELECT Id FROM Employees WHERE UserId = @UserId)";
            }
        }

        #endregion

        #region Event Handlers

        protected void btnRequestLeave_Click(object sender, EventArgs e)
        {
            Response.Redirect("RequestLeave.aspx", false);
        }

        protected void btnLeaveCalendar_Click(object sender, EventArgs e)
        {
            Response.Redirect("LeaveCalendar.aspx", false);
        }

        protected void btnMyLeaves_Click(object sender, EventArgs e)
        {
            Response.Redirect("MyLeaves.aspx", false);
        }

        protected void btnApprovalQueue_Click(object sender, EventArgs e)
        {
            Response.Redirect("ApproveLeaves.aspx", false);
        }

        protected void btnTeamCalendar_Click(object sender, EventArgs e)
        {
            Response.Redirect("LeaveCalendar.aspx?view=team", false);
        }

        protected void btnLeaveReports_Click(object sender, EventArgs e)
        {
            Response.Redirect("LeaveReports.aspx", false);
        }

        protected void btnViewAllBalances_Click(object sender, EventArgs e)
        {
            Response.Redirect("LeaveBalances.aspx", false);
        }

        protected void ddlRequestFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRecentRequests();
        }

        protected void ddlUpcomingFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadUpcomingLeaves();
        }

        protected void gvRecentRequests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int requestId = Convert.ToInt32(e.CommandArgument);

                switch (e.CommandName)
                {
                    case "ViewDetails":
                        Response.Redirect($"ViewLeaveRequest.aspx?id={requestId}", false);
                        break;

                    case "Approve":
                        ApproveLeaveRequest(requestId);
                        break;

                    case "Reject":
                        Response.Redirect($"RejectLeaveRequest.aspx?id={requestId}", false);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error processing request: {ex.Message}", "error");
                LogError(ex);
            }
        }

        protected void gvRecentRequests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string status = DataBinder.Eval(e.Row.DataItem, "Status").ToString();
                string userRole = Session["UserRole"]?.ToString() ?? "";

                // Hide approve/reject buttons for non-managers or already processed requests
                LinkButton btnApprove = e.Row.FindControl("btnApprove") as LinkButton;
                LinkButton btnReject = e.Row.FindControl("btnReject") as LinkButton;

                if (btnApprove != null && btnReject != null)
                {
                    bool canApprove = (userRole == "SUPERADMIN" || userRole == "HRADMIN" || userRole == "MANAGER")
                                     && status == "Pending";

                    btnApprove.Visible = canApprove;
                    btnReject.Visible = canApprove;
                }
            }
        }

        #endregion

        #region Helper Methods

        protected string GetStatusClass(string status)
        {
            switch (status?.ToLower())
            {
                case "pending": return "pending";
                case "approved": return "approved";
                case "rejected": return "rejected";
                default: return "pending";
            }
        }

        protected bool CanApprove(string status)
        {
            string userRole = Session["UserRole"]?.ToString() ?? "";
            return (userRole == "SUPERADMIN" || userRole == "HRADMIN" || userRole == "MANAGER")
                   && status == "Pending";
        }

        protected string GetUsagePercentage(object used, object total)
        {
            try
            {
                decimal usedDays = Convert.ToDecimal(used);
                decimal totalDays = Convert.ToDecimal(total);

                if (totalDays == 0) return "0";

                decimal percentage = (usedDays / totalDays) * 100;
                return Math.Min(percentage, 100).ToString("F0");
            }
            catch
            {
                return "0";
            }
        }

        private void ApproveLeaveRequest(int requestId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get current user employee ID
                    int currentUserId = Convert.ToInt32(Session["UserId"]);
                    int? reviewerId = GetEmployeeIdByUserId(currentUserId);

                    if (!reviewerId.HasValue)
                    {
                        ShowMessage("Error: Could not identify reviewer.", "error");
                        return;
                    }

                    string updateQuery = @"
                        UPDATE LeaveRequests 
                        SET Status = 'Approved', 
                            ReviewedById = @ReviewerId, 
                            ReviewedAt = GETDATE()
                        WHERE Id = @RequestId AND Status = 'Pending'";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@RequestId", requestId);
                        cmd.Parameters.AddWithValue("@ReviewerId", reviewerId.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ShowMessage("Leave request approved successfully.", "success");

                            // Log the approval
                            LogUserActivity(currentUserId, "Leave Approval", "LeaveRequests",
                                          $"Approved leave request ID: {requestId}", GetClientIP());

                            // Reload data to reflect changes
                            LoadDashboardData();

                            // TODO: Send email notification to employee
                        }
                        else
                        {
                            ShowMessage("Error: Leave request not found or already processed.", "warning");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error approving leave request: {ex.Message}", "error");
                LogError(ex);
            }
        }

        private int? GetEmployeeIdByUserId(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id FROM Employees WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        object result = cmd.ExecuteScalar();
                        return result != null ? (int?)Convert.ToInt32(result) : null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private void ShowMessage(string message, string type)
        {
            string cssClass = type == "error" ? "alert-error" :
                             type == "warning" ? "alert-warning" :
                             type == "success" ? "alert-success" : "alert-info";

            litMessage.Text = $@"
                <div class='alert {cssClass}'>
                    <i class='material-icons'>{GetMessageIcon(type)}</i>
                    {message}
                </div>";

            pnlMessage.Visible = true;
        }

        private string GetMessageIcon(string type)
        {
            switch (type)
            {
                case "error": return "error";
                case "warning": return "warning";
                case "success": return "check_circle";
                default: return "info";
            }
        }

        private void LogError(Exception ex)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ErrorLogs (ErrorMessage, StackTrace, Source, Timestamp, RequestUrl, 
                                             UserAgent, IPAddress, UserId, Severity, IsResolved, CreatedAt)
                        VALUES (@ErrorMessage, @StackTrace, @Source, @Timestamp, @RequestUrl, 
                                @UserAgent, @IPAddress, @UserId, @Severity, 0, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message);
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", ex.Source ?? "LeaveManagement.Default");
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@RequestUrl", Request.Url?.ToString() ?? "");
                        cmd.Parameters.AddWithValue("@UserAgent", Request.UserAgent ?? "");
                        cmd.Parameters.AddWithValue("@IPAddress", GetClientIP());
                        cmd.Parameters.AddWithValue("@UserId", Session["UserId"] ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Severity", "High");
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Fail silently for logging errors
            }
        }

        private void LogUserActivity(int userId, string action, string module, string description, string ipAddress)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ActivityLogs (UserId, Action, Module, Description, IPAddress, Timestamp)
                        VALUES (@UserId, @Action, @Module, @Description, @IPAddress, @Timestamp)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@Module", module);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Fail silently for activity logging
            }
        }

        private string GetClientIP()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip ?? "Unknown";
        }

        #endregion
    }

    #region Data Models

   

    #endregion
}