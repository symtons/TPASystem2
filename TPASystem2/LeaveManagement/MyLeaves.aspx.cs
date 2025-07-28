using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace TPASystem2.LeaveManagement
{
    public partial class MyLeaves : System.Web.UI.Page
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

                LoadLeaveBalances();
                LoadMyLeaveRequests();

                // Check for success message from RequestLeave page
                if (Request.QueryString["message"] != null)
                {
                    ShowMessage(Request.QueryString["message"], "success");
                }
            }
        }

        #region Data Loading

        private void LoadLeaveBalances()
        {
            try
            {
                // In a real implementation, this would query leave balance tables
                // For now, showing sample data based on employee
                var balanceData = new List<LeaveBalance>
                {
                    new LeaveBalance { LeaveType = "Vacation", Available = 12, Used = 8, Total = 20 },
                    new LeaveBalance { LeaveType = "Sick", Available = 8, Used = 2, Total = 10 },
                    new LeaveBalance { LeaveType = "Personal", Available = 3, Used = 2, Total = 5 },
                    new LeaveBalance { LeaveType = "Emergency", Available = 2, Used = 0, Total = 2 }
                };

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

        private void LoadMyLeaveRequests()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = BuildMyLeaveRequestsQuery();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        AddQueryParameters(cmd, userId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            gvMyLeaves.DataSource = dt;
                            gvMyLeaves.DataBind();

                            litTotalRequests.Text = dt.Rows.Count.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading your leave requests.", "error");
            }
        }

        private string BuildMyLeaveRequestsQuery()
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
                    CONCAT(rev.FirstName, ' ', rev.LastName) as ReviewedBy
                FROM LeaveRequests lr
                INNER JOIN Employees e ON lr.EmployeeId = e.Id
                LEFT JOIN Employees rev ON lr.ReviewedById = rev.Id
                WHERE e.UserId = @UserId";

            var conditions = new List<string>();

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

            // Time period filter
            if (ddlTimePeriod.SelectedValue != "0")
            {
                conditions.Add("lr.RequestedAt >= DATEADD(day, -@DaysBack, GETDATE())");
            }

            if (conditions.Count > 0)
            {
                baseQuery += " AND " + string.Join(" AND ", conditions);
            }

            return baseQuery + " ORDER BY lr.RequestedAt DESC";
        }

        private void AddQueryParameters(SqlCommand cmd, int userId)
        {
            cmd.Parameters.AddWithValue("@UserId", userId);

            if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
            {
                cmd.Parameters.AddWithValue("@StatusFilter", ddlStatusFilter.SelectedValue);
            }

            if (!string.IsNullOrEmpty(ddlLeaveTypeFilter.SelectedValue))
            {
                cmd.Parameters.AddWithValue("@LeaveTypeFilter", ddlLeaveTypeFilter.SelectedValue);
            }

            if (ddlTimePeriod.SelectedValue != "0")
            {
                cmd.Parameters.AddWithValue("@DaysBack", Convert.ToInt32(ddlTimePeriod.SelectedValue));
            }
        }

        #endregion

        #region Event Handlers

        protected void btnRequestNewLeave_Click(object sender, EventArgs e)
        {
            Response.Redirect("RequestLeave.aspx", false);
        }

        protected void btnBackToDashboard_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx", false);
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMyLeaveRequests();
        }

        protected void ddlLeaveTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMyLeaveRequests();
        }

        protected void ddlTimePeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMyLeaveRequests();
        }

        protected void gvMyLeaves_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMyLeaves.PageIndex = e.NewPageIndex;
            LoadMyLeaveRequests();
        }

        protected void gvMyLeaves_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int requestId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ViewDetails":
                    ViewRequestDetails(requestId);
                    break;
                case "EditRequest":
                    EditLeaveRequest(requestId);
                    break;
                case "CancelRequest":
                    CancelLeaveRequest(requestId);
                    break;
            }
        }

        protected void gvMyLeaves_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Additional row formatting can be added here if needed
            }
        }

        #endregion

        #region Modal Handlers

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlRequestDetails.Visible = false;
        }

        protected void btnModalEdit_Click(object sender, EventArgs e)
        {
            if (ViewState["CurrentRequestId"] != null)
            {
                int requestId = Convert.ToInt32(ViewState["CurrentRequestId"]);
                EditLeaveRequest(requestId);
            }
        }

        protected void btnModalCancel_Click(object sender, EventArgs e)
        {
            if (ViewState["CurrentRequestId"] != null)
            {
                int requestId = Convert.ToInt32(ViewState["CurrentRequestId"]);
                CancelLeaveRequest(requestId);
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
                            CONCAT(rev.FirstName, ' ', rev.LastName) as ReviewedBy
                        FROM LeaveRequests lr
                        INNER JOIN Employees e ON lr.EmployeeId = e.Id
                        LEFT JOIN Employees rev ON lr.ReviewedById = rev.Id
                        WHERE lr.Id = @RequestId AND e.UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RequestId", requestId);
                        cmd.Parameters.AddWithValue("@UserId", Convert.ToInt32(Session["UserId"]));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Populate modal fields
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
                                string status = reader["Status"].ToString();
                                bool canEdit = status == "Draft" || status == "Pending";
                                bool canCancel = status == "Pending";

                                pnlModalActions.Visible = canEdit || canCancel;
                                btnModalEdit.Visible = canEdit;
                                btnModalCancel.Visible = canCancel;

                                // Store request ID for modal actions
                                ViewState["CurrentRequestId"] = requestId;

                                // Show modal
                                pnlRequestDetails.Visible = true;
                            }
                            else
                            {
                                ShowMessage("Leave request not found.", "error");
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

        private void EditLeaveRequest(int requestId)
        {
            // Redirect to RequestLeave page with edit mode
            Response.Redirect($"RequestLeave.aspx?edit={requestId}", false);
        }

        private void CancelLeaveRequest(int requestId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Only allow cancellation of pending requests
                    string updateQuery = @"
                        UPDATE LeaveRequests 
                        SET Status = 'Cancelled',
                            WorkflowStatus = 'CANCELLED'
                        WHERE Id = @RequestId 
                        AND EmployeeId IN (SELECT Id FROM Employees WHERE UserId = @UserId)
                        AND Status = 'Pending'";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@RequestId", requestId);
                        cmd.Parameters.AddWithValue("@UserId", Convert.ToInt32(Session["UserId"]));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ShowMessage("Leave request cancelled successfully.", "success");

                            // Log the cancellation
                            LogUserActivity(Convert.ToInt32(Session["UserId"]), "Leave Cancellation", "LeaveRequests",
                                          $"Cancelled leave request ID: {requestId}", GetClientIP());

                            // Reload data to reflect changes
                            LoadMyLeaveRequests();
                        }
                        else
                        {
                            ShowMessage("Error: Leave request not found or cannot be cancelled.", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error cancelling leave request.", "error");
            }
        }

        #endregion

        #region Helper Methods

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
                        cmd.Parameters.AddWithValue("@Source", ex.Source ?? "LeaveManagement.MyLeaves");
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

    public class LeaveBalance
    {
        public string LeaveType { get; set; }
        public int Available { get; set; }
        public int Used { get; set; }
        public int Total { get; set; }
    }

    #endregion
}