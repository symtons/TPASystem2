using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

namespace TPASystem2.LeaveManagement
{
    public partial class LeaveManagement : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user has appropriate permissions
                if (!IsAuthorizedUser())
                {
                    Response.Redirect("~/Dashboard.aspx");
                    return;
                }

                LoadPageData();
            }
        }

        #region Page Loading Methods

        private bool IsAuthorizedUser()
        {
            try
            {
                string userRole = Session["UserRole"]?.ToString();
                return userRole == "HRAdmin" || userRole == "Admin" || userRole == "ProgramDirector" || userRole == "Manager";
            }
            catch
            {
                return false;
            }
        }

        private void LoadPageData()
        {
            try
            {
                LoadManagerInfo();
                LoadDashboardStats();
                LoadFilterDropdowns();
                LoadLeaveRequests();
                LoadLeaveBalances();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading page data.", "error");
            }
        }

        private void LoadManagerInfo()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                string userRole = Session["UserRole"]?.ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT CONCAT(FirstName, ' ', LastName) as FullName
                        FROM Employees 
                        WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            litManagerName.Text = result.ToString();
                        }

                        // Set role display
                        switch (userRole)
                        {
                            case "HRAdmin":
                                litUserRole.Text = "HR Administrator";
                                break;
                            case "ProgramDirector":
                                litUserRole.Text = "Program Director";
                                break;
                            case "Manager":
                                litUserRole.Text = "Manager";
                                break;
                            default:
                                litUserRole.Text = "Leave Manager";
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                litManagerName.Text = "Leave Manager";
                litUserRole.Text = Session["UserRole"]?.ToString() ?? "Manager";
            }
        }

        private void LoadDashboardStats()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Pending requests
                    string pendingQuery = "SELECT COUNT(*) FROM LeaveRequests WHERE Status = 'Pending'";
                    using (SqlCommand cmd = new SqlCommand(pendingQuery, conn))
                    {
                        litPendingRequests.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                    }

                    // Approved requests
                    string approvedQuery = "SELECT COUNT(*) FROM LeaveRequests WHERE Status = 'Approved'";
                    using (SqlCommand cmd = new SqlCommand(approvedQuery, conn))
                    {
                        litApprovedRequests.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                    }

                    // Currently on leave (approved and within date range)
                    string activeQuery = @"
                        SELECT COUNT(*) 
                        FROM LeaveRequests 
                        WHERE Status = 'Approved' 
                        AND CAST(GETDATE() AS DATE) BETWEEN StartDate AND EndDate";
                    using (SqlCommand cmd = new SqlCommand(activeQuery, conn))
                    {
                        litActiveLeaves.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                    }

                    // Completed leaves (approved and end date has passed)
                    string completedQuery = @"
                        SELECT COUNT(*) 
                        FROM LeaveRequests 
                        WHERE Status = 'Approved' 
                        AND EndDate < CAST(GETDATE() AS DATE)
                        AND EndDate >= DATEADD(week, -1, CAST(GETDATE() AS DATE))";
                    using (SqlCommand cmd = new SqlCommand(completedQuery, conn))
                    {
                        litCompletedLeaves.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                    }

                    // Rejected requests this month
                    string rejectedQuery = @"
                        SELECT COUNT(*) 
                        FROM LeaveRequests 
                        WHERE Status = 'Rejected' 
                        AND YEAR(RequestedAt) = YEAR(GETDATE()) 
                        AND MONTH(RequestedAt) = MONTH(GETDATE())";
                    using (SqlCommand cmd = new SqlCommand(rejectedQuery, conn))
                    {
                        litRejectedRequests.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void LoadFilterDropdowns()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load Departments
                    string deptQuery = "SELECT Id, Name FROM Departments WHERE IsActive = 1 ORDER BY Name";
                    using (SqlCommand cmd = new SqlCommand(deptQuery, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlDepartmentFilter.Items.Clear();
                            ddlDepartmentFilter.Items.Add(new ListItem("All Departments", ""));

                            while (reader.Read())
                            {
                                ddlDepartmentFilter.Items.Add(new ListItem(
                                    reader["Name"].ToString(),
                                    reader["Id"].ToString()
                                ));
                            }
                        }
                    }

                    // Load Leave Types
                    string leaveTypeQuery = "SELECT TypeName FROM LeaveTypes WHERE IsActive = 1 ORDER BY TypeName";
                    using (SqlCommand cmd = new SqlCommand(leaveTypeQuery, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlLeaveTypeFilter.Items.Clear();
                            ddlLeaveTypeFilter.Items.Add(new ListItem("All Leave Types", ""));

                            while (reader.Read())
                            {
                                string typeName = reader["TypeName"].ToString();
                                ddlLeaveTypeFilter.Items.Add(new ListItem(typeName, typeName));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void LoadLeaveRequests()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = BuildLeaveRequestQuery();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        AddLeaveRequestParameters(cmd);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            gvLeaveRequests.DataSource = dt;
                            gvLeaveRequests.DataBind();

                            litDisplayCount.Text = dt.Rows.Count.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading leave requests.", "error");
            }
        }

        private void LoadLeaveBalances()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = BuildLeaveBalanceQuery();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        AddLeaveBalanceParameters(cmd);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            gvLeaveBalances.DataSource = dt;
                            gvLeaveBalances.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading leave balances.", "error");
            }
        }

        #endregion

        #region Query Building Methods

        private string BuildLeaveRequestQuery()
        {
            string baseQuery = @"
                SELECT 
                    lr.Id,
                    CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName,
                    ISNULL(e.EmployeeNumber, 'EMP' + RIGHT('0000' + CAST(e.Id AS varchar(4)), 4)) as EmployeeNumber,
                    ISNULL(d.Name, 'Not Assigned') as Department,
                    lr.LeaveType,
                    lr.StartDate,
                    lr.EndDate,
                    lr.DaysRequested,
                    lr.Reason,
                    lr.Status,
                    lr.RequestedAt,
                    lr.ReviewedAt,
                    CONCAT(rev.FirstName, ' ', rev.LastName) as ReviewedBy
                FROM LeaveRequests lr
                INNER JOIN Employees e ON lr.EmployeeId = e.Id
                LEFT JOIN Departments d ON e.DepartmentId = d.Id
                LEFT JOIN Employees rev ON lr.ReviewedById = rev.Id
                WHERE 1=1";

            var conditions = new List<string>();

            // Status filter - including special logic for active/completed leaves
            if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
            {
                switch (ddlStatusFilter.SelectedValue)
                {
                    case "Active":
                        conditions.Add("lr.Status = 'Approved' AND CAST(GETDATE() AS DATE) BETWEEN lr.StartDate AND lr.EndDate");
                        break;
                    case "Completed":
                        conditions.Add("lr.Status = 'Approved' AND lr.EndDate < CAST(GETDATE() AS DATE)");
                        break;
                    default:
                        conditions.Add("lr.Status = @StatusFilter");
                        break;
                }
            }

            // Department filter
            if (!string.IsNullOrEmpty(ddlDepartmentFilter.SelectedValue))
            {
                conditions.Add("e.DepartmentId = @DepartmentFilter");
            }

            // Leave type filter
            if (!string.IsNullOrEmpty(ddlLeaveTypeFilter.SelectedValue))
            {
                conditions.Add("lr.LeaveType = @LeaveTypeFilter");
            }

            // Employee search
            if (!string.IsNullOrEmpty(txtEmployeeSearch.Text.Trim()))
            {
                conditions.Add("(CONCAT(e.FirstName, ' ', e.LastName) LIKE @EmployeeSearch OR ISNULL(e.EmployeeNumber, 'EMP' + RIGHT('0000' + CAST(e.Id AS varchar(4)), 4)) LIKE @EmployeeSearch)");
            }

            if (conditions.Count > 0)
            {
                baseQuery += " AND " + string.Join(" AND ", conditions);
            }

            return baseQuery + " ORDER BY lr.RequestedAt DESC";
        }

        private string BuildLeaveBalanceQuery()
        {
            string baseQuery = @"
                SELECT 
                    e.Id as EmployeeId,
                    CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName,
                    ISNULL(d.Name, 'Not Assigned') as Department,
                    lb.LeaveType,
                    lb.AllocatedDays,
                    ISNULL(lb.UsedDays, 0) as UsedDays,
                    (lb.AllocatedDays - ISNULL(lb.UsedDays, 0)) as RemainingDays
                FROM Employees e
                INNER JOIN LeaveBalances lb ON e.Id = lb.EmployeeId
                LEFT JOIN Departments d ON e.DepartmentId = d.Id
                WHERE e.Status = 'Active'";

            // Employee search for balances
            if (!string.IsNullOrEmpty(txtBalanceSearch.Text.Trim()))
            {
                baseQuery += " AND (CONCAT(e.FirstName, ' ', e.LastName) LIKE @BalanceSearch OR ISNULL(e.EmployeeNumber, 'EMP' + RIGHT('0000' + CAST(e.Id AS varchar(4)), 4)) LIKE @BalanceSearch)";
            }

            return baseQuery + " ORDER BY e.FirstName, e.LastName, lb.LeaveType";
        }

        private void AddLeaveRequestParameters(SqlCommand cmd)
        {
            if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue) &&
                ddlStatusFilter.SelectedValue != "Active" &&
                ddlStatusFilter.SelectedValue != "Completed")
            {
                cmd.Parameters.AddWithValue("@StatusFilter", ddlStatusFilter.SelectedValue);
            }

            if (!string.IsNullOrEmpty(ddlDepartmentFilter.SelectedValue))
            {
                cmd.Parameters.AddWithValue("@DepartmentFilter", ddlDepartmentFilter.SelectedValue);
            }

            if (!string.IsNullOrEmpty(ddlLeaveTypeFilter.SelectedValue))
            {
                cmd.Parameters.AddWithValue("@LeaveTypeFilter", ddlLeaveTypeFilter.SelectedValue);
            }

            if (!string.IsNullOrEmpty(txtEmployeeSearch.Text.Trim()))
            {
                cmd.Parameters.AddWithValue("@EmployeeSearch", "%" + txtEmployeeSearch.Text.Trim() + "%");
            }
        }

        private void AddLeaveBalanceParameters(SqlCommand cmd)
        {
            if (!string.IsNullOrEmpty(txtBalanceSearch.Text.Trim()))
            {
                cmd.Parameters.AddWithValue("@BalanceSearch", "%" + txtBalanceSearch.Text.Trim() + "%");
            }
        }

        #endregion

        #region Helper Methods for Leave Status

        protected string GetLeaveStatus(object status, object startDate, object endDate)
        {
            try
            {
                string statusStr = status?.ToString() ?? "";

                if (statusStr != "Approved")
                    return statusStr.ToLower();

                DateTime start = Convert.ToDateTime(startDate);
                DateTime end = Convert.ToDateTime(endDate);
                DateTime today = DateTime.Today;

                if (today < start)
                    return "approved-upcoming";
                else if (today >= start && today <= end)
                    return "active";
                else
                    return "completed";
            }
            catch
            {
                return status?.ToString()?.ToLower() ?? "unknown";
            }
        }

        protected string GetLeaveStatusText(object status, object startDate, object endDate)
        {
            try
            {
                string statusStr = status?.ToString() ?? "";

                if (statusStr != "Approved")
                    return statusStr;

                DateTime start = Convert.ToDateTime(startDate);
                DateTime end = Convert.ToDateTime(endDate);
                DateTime today = DateTime.Today;

                if (today < start)
                    return "Approved - Upcoming";
                else if (today >= start && today <= end)
                    return "Currently On Leave";
                else
                    return "Leave Completed";
            }
            catch
            {
                return status?.ToString() ?? "Unknown";
            }
        }

        protected string GetLeaveProgressText(object status, object startDate, object endDate)
        {
            try
            {
                string statusStr = status?.ToString() ?? "";

                if (statusStr != "Approved")
                    return "";

                DateTime start = Convert.ToDateTime(startDate);
                DateTime end = Convert.ToDateTime(endDate);
                DateTime today = DateTime.Today;

                if (today < start)
                {
                    int daysUntil = (start - today).Days;
                    return $"Starts in {daysUntil} day{(daysUntil != 1 ? "s" : "")}";
                }
                else if (today >= start && today <= end)
                {
                    int daysRemaining = (end - today).Days;
                    if (daysRemaining == 0)
                        return "Returns tomorrow";
                    else
                        return $"{daysRemaining} day{(daysRemaining != 1 ? "s" : "")} remaining";
                }
                else
                {
                    int daysAgo = (today - end).Days;
                    return $"Completed {daysAgo} day{(daysAgo != 1 ? "s" : "")} ago";
                }
            }
            catch
            {
                return "";
            }
        }

        protected bool IsCurrentlyOnLeave(object status, object startDate, object endDate)
        {
            try
            {
                string statusStr = status?.ToString() ?? "";

                if (statusStr != "Approved")
                    return false;

                DateTime start = Convert.ToDateTime(startDate);
                DateTime end = Convert.ToDateTime(endDate);
                DateTime today = DateTime.Today;

                return today >= start && today <= end;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Event Handlers

        protected void btnViewLeaveCalendar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/LeaveManagement/LeaveCalendar.aspx");
        }

        protected void btnCreateLeaveRequest_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/LeaveManagement/EmployeeRequestLeave.aspx");
        }

        protected void btnRefreshRequests_Click(object sender, EventArgs e)
        {
            LoadLeaveRequests();
            LoadDashboardStats();
            ShowMessage("Leave requests refreshed successfully.", "success");
        }

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                ExportLeaveRequestsToExcel();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error exporting to Excel.", "error");
            }
        }

        protected void btnManageBalances_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/LeaveManagement/ManageLeaveBalances.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadLeaveRequests();
        }

        protected void btnSearchBalance_Click(object sender, EventArgs e)
        {
            LoadLeaveBalances();
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLeaveRequests();
        }

        protected void ddlDepartmentFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLeaveRequests();
        }

        protected void ddlLeaveTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLeaveRequests();
        }

        protected void gvLeaveRequests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int requestId = Convert.ToInt32(e.CommandArgument);

            try
            {
                switch (e.CommandName)
                {
                    case "ViewDetails":
                        ShowRequestDetails(requestId);
                        break;
                    case "Approve":
                        ApproveLeaveRequest(requestId);
                        break;
                    case "Reject":
                        RejectLeaveRequest(requestId);
                        break;
                    case "EditRequest":
                        Response.Redirect($"~/LeaveManagement/EditLeaveRequest.aspx?id={requestId}");
                        break;
                    case "MarkReturned":
                        MarkEmployeeReturned(requestId);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error processing request.", "error");
            }
        }

        protected void gvLeaveRequests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Additional row formatting can be added here if needed
            }
        }

        protected void gvLeaveBalances_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                switch (e.CommandName)
                {
                    case "EditBalance":
                        string[] args = e.CommandArgument.ToString().Split(',');
                        int employeeId = Convert.ToInt32(args[0]);
                        string leaveType = args[1];
                        Response.Redirect($"~/LeaveManagement/EditLeaveBalance.aspx?empId={employeeId}&type={leaveType}");
                        break;
                    case "ViewHistory":
                        int empId = Convert.ToInt32(e.CommandArgument);
                        Response.Redirect($"~/LeaveManagement/EmployeeLeaveHistory.aspx?empId={empId}");
                        break;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error processing balance request.", "error");
            }
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlRequestDetails.Visible = false;
        }

        protected void btnApproveRequest_Click(object sender, EventArgs e)
        {
            try
            {
                int requestId = Convert.ToInt32(hfSelectedRequestId.Value);
                ApproveLeaveRequest(requestId, txtApprovalComments.Text.Trim());
                pnlRequestDetails.Visible = false;
                LoadLeaveRequests();
                LoadDashboardStats();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error approving request.", "error");
            }
        }

        protected void btnRejectRequest_Click(object sender, EventArgs e)
        {
            try
            {
                int requestId = Convert.ToInt32(hfSelectedRequestId.Value);
                RejectLeaveRequest(requestId, txtApprovalComments.Text.Trim());
                pnlRequestDetails.Visible = false;
                LoadLeaveRequests();
                LoadDashboardStats();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error rejecting request.", "error");
            }
        }

        #endregion

        #region Leave Request Management Methods

        private void ShowRequestDetails(int requestId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            lr.Id,
                            CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName,
                            ISNULL(d.Name, 'Not Assigned') as Department,
                            lr.LeaveType,
                            lr.StartDate,
                            lr.EndDate,
                            lr.DaysRequested,
                            lr.Reason,
                            lr.Status,
                            lr.RequestedAt,
                            lr.ReviewedAt,
                            CONCAT(rev.FirstName, ' ', rev.LastName) as ReviewedBy
                        FROM LeaveRequests lr
                        INNER JOIN Employees e ON lr.EmployeeId = e.Id
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        LEFT JOIN Employees rev ON lr.ReviewedById = rev.Id
                        WHERE lr.Id = @RequestId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RequestId", requestId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Populate modal fields
                                litEmployeeName.Text = reader["EmployeeName"].ToString();
                                litDepartment.Text = reader["Department"].ToString();
                                litLeaveType.Text = reader["LeaveType"].ToString();

                                DateTime startDate = Convert.ToDateTime(reader["StartDate"]);
                                DateTime endDate = Convert.ToDateTime(reader["EndDate"]);

                                litStartDate.Text = startDate.ToString("MMM dd, yyyy");
                                litEndDate.Text = endDate.ToString("MMM dd, yyyy");
                                litDaysRequested.Text = reader["DaysRequested"].ToString();
                                litReason.Text = reader["Reason"]?.ToString() ?? "No reason provided";

                                string status = reader["Status"].ToString();
                                litStatus.Text = GetLeaveStatusText(status, startDate, endDate);
                                litProgress.Text = GetLeaveProgressText(status, startDate, endDate);

                                // Set status badge class
                                spanStatus.Attributes["class"] = $"leave-status-badge status-{GetLeaveStatus(status, startDate, endDate)}";

                                litRequestedAt.Text = Convert.ToDateTime(reader["RequestedAt"]).ToString("MMM dd, yyyy 'at' h:mm tt");

                                // Show review details if reviewed
                                if (reader["ReviewedAt"] != DBNull.Value)
                                {
                                    pnlReviewDetails.Visible = true;
                                    litReviewedBy.Text = reader["ReviewedBy"]?.ToString() ?? "System";
                                    litReviewedAt.Text = Convert.ToDateTime(reader["ReviewedAt"]).ToString("MMM dd, yyyy 'at' h:mm tt");
                                }
                                else
                                {
                                    pnlReviewDetails.Visible = false;
                                }

                                // Show timeline for approved leaves
                                if (status == "Approved")
                                {
                                    pnlLeaveTimeline.Visible = true;
                                    litLeaveTimeline.Text = GenerateLeaveTimeline(startDate, endDate);
                                }
                                else
                                {
                                    pnlLeaveTimeline.Visible = false;
                                }

                                // Show approval actions if pending
                                pnlApprovalActions.Visible = status == "Pending";
                                hfSelectedRequestId.Value = requestId.ToString();

                                // Clear previous comments
                                txtApprovalComments.Text = "";

                                pnlRequestDetails.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading request details.", "error");
            }
        }

        private string GenerateLeaveTimeline(DateTime startDate, DateTime endDate)
        {
            try
            {
                DateTime today = DateTime.Today;
                StringBuilder timeline = new StringBuilder();

                timeline.Append("<div class='timeline-item'>");

                if (today < startDate)
                {
                    int daysUntil = (startDate - today).Days;
                    timeline.Append($"<i class='material-icons'>schedule</i> Leave starts in {daysUntil} day{(daysUntil != 1 ? "s" : "")} ({startDate:MMM dd})");
                }
                else if (today >= startDate && today <= endDate)
                {
                    int totalDays = (endDate - startDate).Days + 1;
                    int daysPassed = (today - startDate).Days + 1;
                    int daysRemaining = (endDate - today).Days;

                    timeline.Append($"<i class='material-icons'>flight_takeoff</i> Currently on leave (Day {daysPassed} of {totalDays})");

                    if (daysRemaining > 0)
                        timeline.Append($"<br><small>{daysRemaining} day{(daysRemaining != 1 ? "s" : "")} remaining</small>");
                    else
                        timeline.Append("<br><small>Returns tomorrow</small>");
                }
                else
                {
                    int daysAgo = (today - endDate).Days;
                    timeline.Append($"<i class='material-icons'>flight_land</i> Leave completed {daysAgo} day{(daysAgo != 1 ? "s" : "")} ago ({endDate:MMM dd})");
                }

                timeline.Append("</div>");

                return timeline.ToString();
            }
            catch
            {
                return "Timeline unavailable";
            }
        }

        private void ApproveLeaveRequest(int requestId, string comments = "")
        {
            try
            {
                int currentUserId = Convert.ToInt32(Session["UserId"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get request details first
                    string getRequestQuery = @"
                        SELECT EmployeeId, LeaveType, DaysRequested, StartDate, EndDate
                        FROM LeaveRequests 
                        WHERE Id = @RequestId AND Status = 'Pending'";

                    int employeeId;
                    string leaveType;
                    decimal daysRequested;
                    DateTime startDate, endDate;

                    using (SqlCommand cmd = new SqlCommand(getRequestQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@RequestId", requestId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                ShowMessage("Request not found or already processed.", "error");
                                return;
                            }

                            employeeId = Convert.ToInt32(reader["EmployeeId"]);
                            leaveType = reader["LeaveType"].ToString();
                            daysRequested = Convert.ToDecimal(reader["DaysRequested"]);
                            startDate = Convert.ToDateTime(reader["StartDate"]);
                            endDate = Convert.ToDateTime(reader["EndDate"]);
                        }
                    }

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Update request status
                            string updateQuery = @"
                                UPDATE LeaveRequests 
                                SET Status = 'Approved', 
                                    ReviewedById = @ReviewedById, 
                                    ReviewedAt = GETDATE(),
                                    WorkflowStatus = 'APPROVED'
                                WHERE Id = @RequestId";

                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@RequestId", requestId);
                                cmd.Parameters.AddWithValue("@ReviewedById", currentUserId);
                                cmd.ExecuteNonQuery();
                            }

                            // Update leave balance
                            string updateBalanceQuery = @"
                                UPDATE LeaveBalances 
                                SET UsedDays = ISNULL(UsedDays, 0) + @DaysUsed
                                WHERE EmployeeId = @EmployeeId AND LeaveType = @LeaveType";

                            using (SqlCommand cmd = new SqlCommand(updateBalanceQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                                cmd.Parameters.AddWithValue("@LeaveType", leaveType);
                                cmd.Parameters.AddWithValue("@DaysUsed", daysRequested);
                                cmd.ExecuteNonQuery();
                            }

                            // Add approval comment if provided
                            if (!string.IsNullOrEmpty(comments))
                            {
                                AddLeaveComment(requestId, currentUserId, comments, "APPROVAL", conn, transaction);
                            }

                            // Log approval action
                            LogLeaveAction(requestId, "APPROVED", "Pending", "Approved", currentUserId, comments, conn, transaction);

                            transaction.Commit();

                            // Send approval notification email
                            SendApprovalNotification(requestId, employeeId, true, comments);

                            ShowMessage("Leave request approved successfully.", "success");
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error approving leave request.", "error");
            }
        }

        private void RejectLeaveRequest(int requestId, string comments = "")
        {
            try
            {
                int currentUserId = Convert.ToInt32(Session["UserId"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get employee ID for notification
                    string getEmployeeQuery = "SELECT EmployeeId FROM LeaveRequests WHERE Id = @RequestId";
                    int employeeId;

                    using (SqlCommand cmd = new SqlCommand(getEmployeeQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@RequestId", requestId);
                        object result = cmd.ExecuteScalar();
                        if (result == null)
                        {
                            ShowMessage("Request not found.", "error");
                            return;
                        }
                        employeeId = Convert.ToInt32(result);
                    }

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Update request status
                            string updateQuery = @"
                                UPDATE LeaveRequests 
                                SET Status = 'Rejected', 
                                    ReviewedById = @ReviewedById, 
                                    ReviewedAt = GETDATE(),
                                    WorkflowStatus = 'REJECTED'
                                WHERE Id = @RequestId";

                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@RequestId", requestId);
                                cmd.Parameters.AddWithValue("@ReviewedById", currentUserId);
                                cmd.ExecuteNonQuery();
                            }

                            // Add rejection comment if provided
                            if (!string.IsNullOrEmpty(comments))
                            {
                                AddLeaveComment(requestId, currentUserId, comments, "REJECTION", conn, transaction);
                            }

                            // Log rejection action
                            LogLeaveAction(requestId, "REJECTED", "Pending", "Rejected", currentUserId, comments, conn, transaction);

                            transaction.Commit();

                            // Send rejection notification email
                            SendApprovalNotification(requestId, employeeId, false, comments);

                            ShowMessage("Leave request rejected.", "success");
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error rejecting leave request.", "error");
            }
        }

        private void MarkEmployeeReturned(int requestId)
        {
            try
            {
                int currentUserId = Convert.ToInt32(Session["UserId"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Add a comment noting early return
                            string comment = $"Employee marked as returned early by {Session["UserName"] ?? "Administrator"} on {DateTime.Now:MMM dd, yyyy 'at' h:mm tt}";
                            AddLeaveComment(requestId, currentUserId, comment, "EARLY_RETURN", conn, transaction);

                            // Log the action
                            LogLeaveAction(requestId, "EARLY_RETURN", "Approved", "Approved", currentUserId, comment, conn, transaction);

                            transaction.Commit();

                            ShowMessage("Employee marked as returned from leave.", "success");
                            LoadLeaveRequests();
                            LoadDashboardStats();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error marking employee as returned.", "error");
            }
        }

        #endregion

        #region Helper Methods

        private void AddLeaveComment(int requestId, int userId, string comment, string commentType, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                string commentQuery = @"
                    INSERT INTO LeaveRequestComments (RequestId, UserId, Comment, CommentType, CreatedAt)
                    VALUES (@RequestId, @UserId, @Comment, @CommentType, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(commentQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@RequestId", requestId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Comment", comment);
                    cmd.Parameters.AddWithValue("@CommentType", commentType);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                // Don't throw as this is not critical
            }
        }

        private void LogLeaveAction(int requestId, string actionType, string previousStatus, string newStatus, int actionBy, string comments, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                string historyQuery = @"
                    INSERT INTO LeaveRequestHistory (RequestId, ActionType, PreviousStatus, NewStatus, ActionBy, ActionDate, Comments)
                    VALUES (@RequestId, @ActionType, @PreviousStatus, @NewStatus, @ActionBy, GETDATE(), @Comments)";

                using (SqlCommand cmd = new SqlCommand(historyQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@RequestId", requestId);
                    cmd.Parameters.AddWithValue("@ActionType", actionType);
                    cmd.Parameters.AddWithValue("@PreviousStatus", previousStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NewStatus", newStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ActionBy", actionBy);
                    cmd.Parameters.AddWithValue("@Comments", comments ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                // Don't throw as this is not critical
            }
        }

        private void ExportLeaveRequestsToExcel()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = BuildLeaveRequestQuery();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        AddLeaveRequestParameters(cmd);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            // Generate Excel content
                            StringBuilder sb = new StringBuilder();

                            // Add headers
                            sb.AppendLine("Employee Name\tEmployee Number\tDepartment\tLeave Type\tStart Date\tEnd Date\tDays Requested\tStatus\tProgress\tRequested Date\tReason");

                            // Add data rows
                            foreach (DataRow row in dt.Rows)
                            {
                                DateTime startDate = Convert.ToDateTime(row["StartDate"]);
                                DateTime endDate = Convert.ToDateTime(row["EndDate"]);
                                string status = row["Status"].ToString();

                                sb.AppendLine($"{row["EmployeeName"]}\t{row["EmployeeNumber"]}\t{row["Department"]}\t{row["LeaveType"]}\t" +
                                            $"{startDate:yyyy-MM-dd}\t{endDate:yyyy-MM-dd}\t" +
                                            $"{row["DaysRequested"]}\t{GetLeaveStatusText(status, startDate, endDate)}\t" +
                                            $"{GetLeaveProgressText(status, startDate, endDate)}\t" +
                                            $"{Convert.ToDateTime(row["RequestedAt"]):yyyy-MM-dd}\t" +
                                            $"{row["Reason"]?.ToString().Replace('\t', ' ').Replace('\n', ' ')}");
                            }

                            // Set response headers for Excel download
                            Response.Clear();
                            Response.ContentType = "application/vnd.ms-excel";
                            Response.AddHeader("content-disposition", $"attachment; filename=LeaveManagement_{DateTime.Now:yyyyMMdd}.xls");
                            Response.Write(sb.ToString());
                            Response.End();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        private void SendApprovalNotification(int requestId, int employeeId, bool approved, string comments)
        {
            try
            {
                // Implementation would depend on your email system
                // This is a placeholder for email notification functionality

                string status = approved ? "Approved" : "Rejected";
                string subject = $"Leave Request {status}";

                // In a real implementation, you would:
                // 1. Get employee email from database
                // 2. Create email content
                // 3. Send email using your email service

                // For now, we'll log this action
                LogAction($"Leave request {requestId} {status.ToLower()} for employee {employeeId}. Comments: {comments}");
            }
            catch (Exception ex)
            {
                LogError(ex);
                // Don't throw here as this is not critical to the approval process
            }
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = $"alert-panel {type}";
            litMessage.Text = message;
        }

        private void LogError(Exception ex)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ErrorLogs (ErrorMessage, StackTrace, Source, Timestamp, UserId, Severity)
                        VALUES (@ErrorMessage, @StackTrace, @Source, GETDATE(), @UserId, 'High')";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message);
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", "LeaveManagement.aspx");
                        cmd.Parameters.AddWithValue("@UserId", Session["UserId"] ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Ignore logging errors to prevent infinite loops
            }
        }

        private void LogAction(string action)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ApplicationLogs (EventType, Message, Timestamp, Source, Level, UserId)
                        VALUES ('Leave Management Action', @Message, GETDATE(), 'LeaveManagement', 'Info', @UserId)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Message", action);
                        cmd.Parameters.AddWithValue("@UserId", Session["UserId"] ?? (object)DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Ignore logging errors
            }
        }

        #endregion
    }
}