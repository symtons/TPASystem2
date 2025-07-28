using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.LeaveManagement
{
    public partial class ApproveLeaves : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //if (Session["UserId"] == null)
                //{
                //    Response.Redirect("~/Login.aspx", false);
                //    return;
                //}

                //if (!HasApprovalPermissions())
                //{
                //    Response.Redirect("Default.aspx?error=Access denied", false);
                //    return;
                //}

                LoadLeaveRequests();
            }
        }

        #region Access Control

        private bool HasApprovalPermissions()
        {
            string userRole = Session["UserRole"]?.ToString() ?? "";
            return userRole == "SUPERADMIN" || userRole == "HRADMIN" || userRole == "MANAGER" || userRole == "ADMIN";
        }

        #endregion

        #region Data Loading

        private void LoadLeaveRequests()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = BuildLeaveRequestsQuery();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        AddQueryParameters(cmd);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            gvLeaveRequests.DataSource = dt;
                            gvLeaveRequests.DataBind();

                            litTotalRequests.Text = dt.Rows.Count.ToString();
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

        private string BuildLeaveRequestsQuery()
        {
            string baseQuery = @"
                SELECT 
                    lr.Id,
                    lr.LeaveType,
                    lr.StartDate,
                    lr.EndDate,
                    lr.DaysRequested,
                    lr.Reason,
                    lr.Status,
                    lr.RequestedAt,
                    lr.ReviewedAt,
                    CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName,
                    d.Name as Department,
                    CONCAT(rev.FirstName, ' ', rev.LastName) as ReviewedBy
                FROM LeaveRequests lr
                INNER JOIN Employees e ON lr.EmployeeId = e.Id
                LEFT JOIN Departments d ON e.DepartmentId = d.Id
                LEFT JOIN Employees rev ON lr.ReviewedById = rev.Id";

            string whereClause = BuildWhereClause();
            string orderClause = " ORDER BY lr.RequestedAt DESC";

            return baseQuery + whereClause + orderClause;
        }

        private string BuildWhereClause()
        {
            var conditions = new System.Collections.Generic.List<string>();

            // Add user access filter based on role
            string userRole = Session["UserRole"]?.ToString() ?? "";
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            if (userRole == "MANAGER")
            {
                // Managers see their direct reports only
                conditions.Add(@"e.ManagerId IN (SELECT Id FROM Employees WHERE UserId = @CurrentUserId)");
            }
            // SUPERADMIN and HRADMIN see all requests (no additional filter needed)

            // Status filter
            if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
            {
                conditions.Add("lr.Status = @StatusFilter");
            }

            // Leave type filter
            if (!string.IsNullOrEmpty(ddlLeaveTypeFilter.SelectedValue))
            {
                conditions.Add("lr.LeaveType = @LeaveTypeFilter");
            }

            // Date range filter
            if (ddlDateRange.SelectedValue != "0")
            {
                conditions.Add("lr.RequestedAt >= DATEADD(day, -@DaysBack, GETDATE())");
            }

            // Employee search filter
            if (!string.IsNullOrEmpty(txtEmployeeSearch.Text.Trim()))
            {
                conditions.Add("(e.FirstName LIKE @EmployeeSearch OR e.LastName LIKE @EmployeeSearch OR CONCAT(e.FirstName, ' ', e.LastName) LIKE @EmployeeSearch)");
            }

            return conditions.Count > 0 ? " WHERE " + string.Join(" AND ", conditions) : "";
        }

        private void AddQueryParameters(SqlCommand cmd)
        {
            string userRole = Session["UserRole"]?.ToString() ?? "";
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            if (userRole == "MANAGER")
            {
                cmd.Parameters.AddWithValue("@CurrentUserId", currentUserId);
            }

            if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
            {
                cmd.Parameters.AddWithValue("@StatusFilter", ddlStatusFilter.SelectedValue);
            }

            if (!string.IsNullOrEmpty(ddlLeaveTypeFilter.SelectedValue))
            {
                cmd.Parameters.AddWithValue("@LeaveTypeFilter", ddlLeaveTypeFilter.SelectedValue);
            }

            if (ddlDateRange.SelectedValue != "0")
            {
                cmd.Parameters.AddWithValue("@DaysBack", Convert.ToInt32(ddlDateRange.SelectedValue));
            }

            if (!string.IsNullOrEmpty(txtEmployeeSearch.Text.Trim()))
            {
                cmd.Parameters.AddWithValue("@EmployeeSearch", "%" + txtEmployeeSearch.Text.Trim() + "%");
            }
        }

        #endregion

        #region Event Handlers

        protected void btnBackToDashboard_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx", false);
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLeaveRequests();
        }

        protected void ddlLeaveTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLeaveRequests();
        }

        protected void ddlDateRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLeaveRequests();
        }

        protected void txtEmployeeSearch_TextChanged(object sender, EventArgs e)
        {
            LoadLeaveRequests();
        }

        protected void gvLeaveRequests_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvLeaveRequests.PageIndex = e.NewPageIndex;
            LoadLeaveRequests();
        }

        protected void gvLeaveRequests_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int requestId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ViewDetails":
                    ViewRequestDetails(requestId);
                    break;
                case "Approve":
                    ApproveLeaveRequest(requestId);
                    break;
                case "Reject":
                    RejectLeaveRequest(requestId);
                    break;
            }
        }

        protected void gvLeaveRequests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // You can add additional row formatting here if needed
            }
        }

        #endregion

        #region Modal Handlers

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlRequestDetails.Visible = false;
        }

        protected void btnModalApprove_Click(object sender, EventArgs e)
        {
            if (ViewState["CurrentRequestId"] != null)
            {
                int requestId = Convert.ToInt32(ViewState["CurrentRequestId"]);
                ApproveLeaveRequest(requestId);
                pnlRequestDetails.Visible = false;
            }
        }

        protected void btnModalReject_Click(object sender, EventArgs e)
        {
            if (ViewState["CurrentRequestId"] != null)
            {
                int requestId = Convert.ToInt32(ViewState["CurrentRequestId"]);
                RejectLeaveRequest(requestId);
                pnlRequestDetails.Visible = false;
            }
        }

        #endregion

        #region Request Management

        private void ViewRequestDetails(int requestId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            lr.*,
                            CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName,
                            d.Name as Department,
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
                                litDepartment.Text = reader["Department"]?.ToString() ?? "Not Assigned";
                                litLeaveType.Text = reader["LeaveType"].ToString();
                                litStartDate.Text = Convert.ToDateTime(reader["StartDate"]).ToString("MMM dd, yyyy");
                                litEndDate.Text = Convert.ToDateTime(reader["EndDate"]).ToString("MMM dd, yyyy");
                                litDaysRequested.Text = reader["DaysRequested"].ToString();
                                litStatus.Text = reader["Status"].ToString();
                                litRequestedAt.Text = Convert.ToDateTime(reader["RequestedAt"]).ToString("MMM dd, yyyy hh:mm tt");
                                litReason.Text = reader["Reason"].ToString();

                                // Set status badge class
                                spanStatus.Attributes["class"] = "leave-status-badge status-" + reader["Status"].ToString().ToLower();

                                // Show review information if available
                                if (reader["ReviewedAt"] != DBNull.Value)
                                {
                                    pnlReviewInfo.Visible = true;
                                    litReviewedBy.Text = reader["ReviewedBy"]?.ToString() ?? "Unknown";
                                    litReviewedAt.Text = Convert.ToDateTime(reader["ReviewedAt"]).ToString("MMM dd, yyyy hh:mm tt");
                                }
                                else
                                {
                                    pnlReviewInfo.Visible = false;
                                }

                                // Show/hide action buttons based on status
                                bool isPending = reader["Status"].ToString() == "Pending";
                                pnlModalActions.Visible = isPending;

                                // Store request ID for modal actions
                                ViewState["CurrentRequestId"] = requestId;

                                // Show modal
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
                            ReviewedAt = GETDATE(),
                            WorkflowStatus = 'APPROVED'
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
                            LoadLeaveRequests();

                            // TODO: Send email notification to employee
                            // TODO: Create notification record
                        }
                        else
                        {
                            ShowMessage("Error: Leave request not found or already processed.", "error");
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

        private void RejectLeaveRequest(int requestId)
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
                        SET Status = 'Rejected', 
                            ReviewedById = @ReviewerId, 
                            ReviewedAt = GETDATE(),
                            WorkflowStatus = 'REJECTED'
                        WHERE Id = @RequestId AND Status = 'Pending'";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@RequestId", requestId);
                        cmd.Parameters.AddWithValue("@ReviewerId", reviewerId.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ShowMessage("Leave request rejected.", "warning");

                            // Log the rejection
                            LogUserActivity(currentUserId, "Leave Rejection", "LeaveRequests",
                                          $"Rejected leave request ID: {requestId}", GetClientIP());

                            // Reload data to reflect changes
                            LoadLeaveRequests();

                            // TODO: Send email notification to employee
                            // TODO: Create notification record
                        }
                        else
                        {
                            ShowMessage("Error: Leave request not found or already processed.", "error");
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
            catch (Exception ex)
            {
                LogError(ex);
                return null;
            }
        }

        #endregion

        #region Utility Methods

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
                        INSERT INTO ErrorLogs (ErrorId, ErrorMessage, StackTrace, Source, Timestamp, RequestUrl, UserAgent, IPAddress, UserId, Severity, CreatedAt)
                        VALUES (@ErrorId, @ErrorMessage, @StackTrace, @Source, @Timestamp, @RequestUrl, @UserAgent, @IPAddress, @UserId, @Severity, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorId", Guid.NewGuid().ToString());
                        cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message ?? "");
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", ex.Source ?? "LeaveManagement.ApproveLeaves");
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
}