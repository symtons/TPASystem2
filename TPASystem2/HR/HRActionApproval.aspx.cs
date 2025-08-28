using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Linq;

namespace TPASystem2.HR
{
    public partial class HRActionsApproval : System.Web.UI.Page
    {
        #region Properties and Fields

        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int CurrentUserId => Convert.ToInt32(Session["UserId"] ?? 0);
        private string CurrentUserRole => Session["UserRole"]?.ToString() ?? "";
        private string CurrentUserEmail => Session["UserEmail"]?.ToString() ?? "";
        private string CurrentUserName => Session["UserName"]?.ToString() ?? "";

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CheckUserPermissions();
                InitializePage();
                LoadFilters();
                LoadHRActions();
            }
        }

        #endregion

        #region Initialization Methods

        private void CheckUserPermissions()
        {
            //if (CurrentUserId == 0)
            //{
            //    Response.Redirect("~/Login.aspx", false);
            //    return;
            //}

            //// Check if user has HR Admin permissions
            //if (!CanApproveHRActions())
            //{
            //    ShowError("You do not have permission to approve HR Actions.");
            //    rptHRActions.Visible = false;
            //    return;
            //}
        }

        private bool CanApproveHRActions()
        {
            return CurrentUserRole.ToUpper().Contains("HRADMIN") ||
                   CurrentUserRole.ToUpper().Contains("SUPERADMIN") ||
                   CurrentUserRole.ToUpper().Contains("ADMIN");
        }

        private void InitializePage()
        {
            litCurrentUser.Text = CurrentUserName;

            // Set default date range (last 30 days)
            txtFromDate.Text = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
            txtToDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
        }

        private void LoadFilters()
        {
            try
            {
                // Load employees for filter
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT DISTINCT e.Id, e.FirstName + ' ' + e.LastName AS FullName
                        FROM Employees e
                        INNER JOIN HRActions hr ON e.Id = hr.EmployeeId
                        WHERE e.IsActive = 1
                        ORDER BY FullName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        ddlEmployeeFilter.Items.Clear();
                        ddlEmployeeFilter.Items.Add(new ListItem("All Employees", ""));

                        while (reader.Read())
                        {
                            ddlEmployeeFilter.Items.Add(new ListItem(reader["FullName"].ToString(), reader["Id"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowError("Error loading filter options.");
            }
        }

        #endregion

        #region Data Loading Methods

        private void LoadHRActions()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = BuildHRActionsQuery();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        AddFilterParameters(cmd);

                        DataTable dt = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }

                        if (dt.Rows.Count > 0)
                        {
                            rptHRActions.DataSource = dt;
                            rptHRActions.DataBind();
                            pnlNoResults.Visible = false;

                            // Update pending count
                            int pendingCount = dt.AsEnumerable().Count(row => row.Field<string>("Status") == "PENDING");
                            litPendingCount.Text = pendingCount.ToString();
                        }
                        else
                        {
                            rptHRActions.DataSource = null;
                            rptHRActions.DataBind();
                            pnlNoResults.Visible = true;
                            litPendingCount.Text = "0";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowError("Error loading HR Actions. Please try again.");
            }
        }

        private string BuildHRActionsQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(@"
                SELECT hr.Id, hr.EmployeeId, hr.Status, hr.RequestDate, hr.ApprovedDate, hr.RejectedDate,
                       hr.EmployeeName, hr.IsRateChange, hr.IsTransfer, hr.IsPromotion, hr.IsStatusChange,
                       hr.PreviousRateSalary, hr.NewRateSalary, hr.AmountOfIncrease, hr.RateType,
                       hr.NewLocation, hr.LeaderSupervisor, hr.NewClass,
                       hr.OldJobTitle, hr.NewJobTitle, hr.PromotionNewRateSalary,
                       hr.NewName, hr.NewPhone, hr.NewEmail, hr.NewAddress,
                       hr.AdditionalComments, hr.CreatedAt, hr.UpdatedAt,
                       reqBy.FirstName + ' ' + reqBy.LastName AS RequestedByName,
                       appBy.FirstName + ' ' + appBy.LastName AS ApprovedByName,
                       d.Name AS DepartmentName
                FROM HRActions hr
                LEFT JOIN Employees reqByEmp ON hr.RequestedById = reqByEmp.Id
                LEFT JOIN Employees reqBy ON reqByEmp.Id = reqBy.Id
                LEFT JOIN Employees appByEmp ON hr.ApprovedById = appByEmp.Id
                LEFT JOIN Employees appBy ON appByEmp.Id = appBy.Id
                LEFT JOIN Employees e ON hr.EmployeeId = e.Id
                LEFT JOIN Departments d ON e.DepartmentId = d.Id
                WHERE 1=1");

            // Add filters
            if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
                query.AppendLine("AND hr.Status = @Status");

            if (!string.IsNullOrEmpty(ddlEmployeeFilter.SelectedValue))
                query.AppendLine("AND hr.EmployeeId = @EmployeeId");

            if (!string.IsNullOrEmpty(ddlActionTypeFilter.SelectedValue))
            {
                switch (ddlActionTypeFilter.SelectedValue)
                {
                    case "RateChange":
                        query.AppendLine("AND hr.IsRateChange = 1");
                        break;
                    case "Transfer":
                        query.AppendLine("AND hr.IsTransfer = 1");
                        break;
                    case "Promotion":
                        query.AppendLine("AND hr.IsPromotion = 1");
                        break;
                    case "StatusChange":
                        query.AppendLine("AND hr.IsStatusChange = 1");
                        break;
                }
            }

            if (!string.IsNullOrEmpty(txtFromDate.Text))
                query.AppendLine("AND CAST(hr.RequestDate AS DATE) >= @FromDate");

            if (!string.IsNullOrEmpty(txtToDate.Text))
                query.AppendLine("AND CAST(hr.RequestDate AS DATE) <= @ToDate");

            query.AppendLine("ORDER BY hr.RequestDate DESC, hr.Id DESC");

            return query.ToString();
        }

        private void AddFilterParameters(SqlCommand cmd)
        {
            if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
                cmd.Parameters.AddWithValue("@Status", ddlStatusFilter.SelectedValue);

            if (!string.IsNullOrEmpty(ddlEmployeeFilter.SelectedValue))
                cmd.Parameters.AddWithValue("@EmployeeId", Convert.ToInt32(ddlEmployeeFilter.SelectedValue));

            if (!string.IsNullOrEmpty(txtFromDate.Text))
                cmd.Parameters.AddWithValue("@FromDate", DateTime.Parse(txtFromDate.Text));

            if (!string.IsNullOrEmpty(txtToDate.Text))
                cmd.Parameters.AddWithValue("@ToDate", DateTime.Parse(txtToDate.Text));
        }

        #endregion

        #region Event Handlers

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadHRActions();
        }

        protected void ddlEmployeeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadHRActions();
        }

        protected void ddlActionTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadHRActions();
        }

        protected void btnApplyFilters_Click(object sender, EventArgs e)
        {
            LoadHRActions();
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            ddlStatusFilter.SelectedIndex = 1; // Default to Pending
            ddlEmployeeFilter.SelectedIndex = 0;
            ddlActionTypeFilter.SelectedIndex = 0;
            txtFromDate.Text = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
            txtToDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            LoadHRActions();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadHRActions();
            ShowSuccess("HR Actions list refreshed successfully.");
        }

        protected void btnViewAll_Click(object sender, EventArgs e)
        {
            ddlStatusFilter.SelectedIndex = 0; // All statuses
            LoadHRActions();
        }

        protected void rptHRActions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                int hrActionId = Convert.ToInt32(e.CommandArgument);
                hfSelectedHRActionId.Value = hrActionId.ToString();

                switch (e.CommandName)
                {
                    case "ViewDetails":
                        LoadHRActionDetails(hrActionId);
                        pnlDetailsModal.Visible = true;
                        break;

                    case "Approve":
                        ApproveHRAction(hrActionId, "");
                        break;

                    case "Reject":
                        RejectHRAction(hrActionId, "Request rejected by HR Admin");
                        break;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowError("Error processing HR Action. Please try again.");
            }
        }

        protected void rptHRActions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                Panel pnlActionTypes = (Panel)e.Item.FindControl("pnlActionTypes");

                // Build action type badges
                StringBuilder actionTypes = new StringBuilder();

                if (Convert.ToBoolean(row["IsRateChange"]))
                    actionTypes.Append("<span class='action-type-badge rate-change'><i class='material-icons'>attach_money</i> Rate Change</span>");

                if (Convert.ToBoolean(row["IsTransfer"]))
                    actionTypes.Append("<span class='action-type-badge transfer'><i class='material-icons'>swap_horiz</i> Transfer</span>");

                if (Convert.ToBoolean(row["IsPromotion"]))
                    actionTypes.Append("<span class='action-type-badge promotion'><i class='material-icons'>trending_up</i> Promotion</span>");

                if (Convert.ToBoolean(row["IsStatusChange"]))
                    actionTypes.Append("<span class='action-type-badge status-change'><i class='material-icons'>update</i> Status Change</span>");

                pnlActionTypes.Controls.Add(new LiteralControl(actionTypes.ToString()));
            }
        }

        protected void btnModalApprove_Click(object sender, EventArgs e)
        {
            int hrActionId = Convert.ToInt32(hfSelectedHRActionId.Value);
            string comments = txtModalComments.Text.Trim();
            ApproveHRAction(hrActionId, comments);
        }

        protected void btnModalReject_Click(object sender, EventArgs e)
        {
            int hrActionId = Convert.ToInt32(hfSelectedHRActionId.Value);
            string comments = txtModalComments.Text.Trim();
            if (string.IsNullOrEmpty(comments))
                comments = "Request rejected by HR Admin";
            RejectHRAction(hrActionId, comments);
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlDetailsModal.Visible = false;
            hfSelectedHRActionId.Value = "";
            txtModalComments.Text = "";
        }

        #endregion

        #region HR Action Details Modal

        private void LoadHRActionDetails(int hrActionId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT hr.*, 
                               e.FirstName + ' ' + e.LastName AS EmployeeFullName,
                               reqBy.FirstName + ' ' + reqBy.LastName AS RequestedByName,
                               appBy.FirstName + ' ' + appBy.LastName AS ApprovedByName
                        FROM HRActions hr
                        INNER JOIN Employees e ON hr.EmployeeId = e.Id
                        LEFT JOIN Employees reqBy ON hr.RequestedById = reqBy.Id
                        LEFT JOIN Employees appBy ON hr.ApprovedById = appBy.Id
                        WHERE hr.Id = @HRActionId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@HRActionId", hrActionId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                PopulateModalDetails(reader);
                            }
                            else
                            {
                                ShowError("HR Action not found.");
                                pnlDetailsModal.Visible = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowError("Error loading HR Action details.");
                pnlDetailsModal.Visible = false;
            }
        }

        private void PopulateModalDetails(SqlDataReader reader)
        {
            // Basic information
            litModalHRActionId.Text = reader["Id"].ToString();
            litModalEmployee.Text = reader["EmployeeFullName"].ToString();
            litModalRequestDate.Text = Convert.ToDateTime(reader["RequestDate"]).ToString("MMM dd, yyyy 'at' h:mm tt");
            litModalRequestedBy.Text = reader["RequestedByName"]?.ToString() ?? "N/A";

            string status = reader["Status"].ToString();
            litModalStatus.Text = GetStatusDisplayText(status);
            spanModalStatus.Attributes["class"] = $"status-badge status-{status.ToLower()}";

            // Reset all panels
            pnlModalRateChange.Visible = false;
            pnlModalTransfer.Visible = false;
            pnlModalPromotion.Visible = false;
            pnlModalContact.Visible = false;
            pnlModalComments.Visible = false;
            pnlModalApprovalActions.Visible = false;
            pnlModalApprovalHistory.Visible = true;
            pnlModalApprovedTimeline.Visible = false;
            pnlModalRejectedTimeline.Visible = false;

            // Rate Change Details
            if (Convert.ToBoolean(reader["IsRateChange"]))
            {
                pnlModalRateChange.Visible = true;
                litModalPreviousRate.Text = reader["PreviousRateSalary"]?.ToString() ?? "0";
                litModalNewRate.Text = reader["NewRateSalary"]?.ToString() ?? "0";
                litModalIncrease.Text = reader["AmountOfIncrease"]?.ToString() ?? "0";
                litModalRateType.Text = reader["RateType"]?.ToString() ?? "N/A";
            }

            // Transfer Details
            if (Convert.ToBoolean(reader["IsTransfer"]))
            {
                pnlModalTransfer.Visible = true;
                litModalNewLocation.Text = reader["NewLocation"]?.ToString() ?? "N/A";
                litModalSupervisor.Text = reader["LeaderSupervisor"]?.ToString() ?? "N/A";
                litModalNewClass.Text = reader["NewClass"]?.ToString() ?? "N/A";
            }

            // Promotion Details
            if (Convert.ToBoolean(reader["IsPromotion"]))
            {
                pnlModalPromotion.Visible = true;
                litModalOldJobTitle.Text = reader["OldJobTitle"]?.ToString() ?? "N/A";
                litModalNewJobTitle.Text = reader["NewJobTitle"]?.ToString() ?? "N/A";
                litModalPromotionSalary.Text = reader["PromotionNewRateSalary"]?.ToString() ?? "0";
            }

            // Contact Information Changes
            bool hasContactChanges = !string.IsNullOrEmpty(reader["NewName"]?.ToString()) ||
                                   !string.IsNullOrEmpty(reader["NewPhone"]?.ToString()) ||
                                   !string.IsNullOrEmpty(reader["NewEmail"]?.ToString()) ||
                                   !string.IsNullOrEmpty(reader["NewAddress"]?.ToString());

            if (hasContactChanges)
            {
                pnlModalContact.Visible = true;

                if (!string.IsNullOrEmpty(reader["NewName"]?.ToString()))
                {
                    divModalNewName.Visible = true;
                    litModalNewName.Text = reader["NewName"].ToString();
                }

                if (!string.IsNullOrEmpty(reader["NewPhone"]?.ToString()))
                {
                    divModalNewPhone.Visible = true;
                    litModalNewPhone.Text = reader["NewPhone"].ToString();
                }

                if (!string.IsNullOrEmpty(reader["NewEmail"]?.ToString()))
                {
                    divModalNewEmail.Visible = true;
                    litModalNewEmail.Text = reader["NewEmail"].ToString();
                }

                if (!string.IsNullOrEmpty(reader["NewAddress"]?.ToString()))
                {
                    divModalNewAddress.Visible = true;
                    litModalNewAddress.Text = reader["NewAddress"].ToString();
                }
            }

            // Additional Comments
            if (!string.IsNullOrEmpty(reader["AdditionalComments"]?.ToString()))
            {
                pnlModalComments.Visible = true;
                litModalComments.Text = reader["AdditionalComments"].ToString();
            }

            // Approval Actions (only show if pending)
            pnlModalApprovalActions.Visible = status == "PENDING";

            // Approval History
            litModalSubmittedDate.Text = Convert.ToDateTime(reader["RequestDate"]).ToString("MMM dd, yyyy 'at' h:mm tt");

            if (status == "APPROVED" && reader["ApprovedDate"] != DBNull.Value)
            {
                pnlModalApprovedTimeline.Visible = true;
                litModalApprovedBy.Text = reader["ApprovedByName"]?.ToString() ?? "System";
                litModalApprovedDate.Text = Convert.ToDateTime(reader["ApprovedDate"]).ToString("MMM dd, yyyy 'at' h:mm tt");
            }
            else if (status == "REJECTED" && reader["RejectedDate"] != DBNull.Value)
            {
                pnlModalRejectedTimeline.Visible = true;
                litModalRejectedDate.Text = Convert.ToDateTime(reader["RejectedDate"]).ToString("MMM dd, yyyy 'at' h:mm tt");
                litModalRejectionReason.Text = reader["RejectionReason"]?.ToString() ?? "No reason provided";
            }
        }

        #endregion

        #region Approval Methods

        private void ApproveHRAction(int hrActionId, string comments)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Use the stored procedure
                    using (SqlCommand cmd = new SqlCommand("sp_ApproveHRAction", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@HRActionId", hrActionId);
                        cmd.Parameters.AddWithValue("@ApprovedById", GetCurrentUserEmployeeId());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string result = reader["Result"].ToString();
                                string message = reader["Message"].ToString();

                                if (result == "SUCCESS")
                                {
                                    ShowSuccess($"HR Action approved successfully! {message}");

                                    // Log activity
                                    LogUserActivity("HR Action Approved", $"Approved HR Action ID: {hrActionId}");

                                    // Close modal and refresh data
                                    pnlDetailsModal.Visible = false;
                                    LoadHRActions();
                                }
                                else
                                {
                                    ShowError($"Error approving HR Action: {message}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowError("Error approving HR Action. Please try again.");
            }
        }

        private void RejectHRAction(int hrActionId, string rejectionReason)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Use the stored procedure
                    using (SqlCommand cmd = new SqlCommand("sp_RejectHRAction", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@HRActionId", hrActionId);
                        cmd.Parameters.AddWithValue("@RejectedById", GetCurrentUserEmployeeId());
                        cmd.Parameters.AddWithValue("@RejectionReason", rejectionReason);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string result = reader["Result"].ToString();
                                string message = reader["Message"].ToString();

                                if (result == "SUCCESS")
                                {
                                    ShowSuccess($"HR Action rejected successfully. {message}");

                                    // Log activity
                                    LogUserActivity("HR Action Rejected", $"Rejected HR Action ID: {hrActionId}. Reason: {rejectionReason}");

                                    // Close modal and refresh data
                                    pnlDetailsModal.Visible = false;
                                    LoadHRActions();
                                }
                                else
                                {
                                    ShowError($"Error rejecting HR Action: {message}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowError("Error rejecting HR Action. Please try again.");
            }
        }

        #endregion

        #region Helper Methods

        public string GetStatusDisplayText(string status)
        {
            switch (status?.ToUpper())
            {
                case "PENDING": return "Pending Review";
                case "APPROVED": return "Approved";
                case "REJECTED": return "Rejected";
                case "DRAFT": return "Draft";
                default: return status ?? "Unknown";
            }
        }

        private int GetCurrentUserEmployeeId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT e.Id FROM Users u INNER JOIN Employees e ON u.Id = e.UserId WHERE u.Id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
            litSuccessMessage.Text = message;
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
            litErrorMessage.Text = message;
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
                                             UserAgent, IPAddress, UserId, Severity, CreatedAt)
                        VALUES (@ErrorMessage, @StackTrace, @Source, @Timestamp, @RequestUrl,
                                @UserAgent, @IPAddress, @UserId, @Severity, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message);
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", ex.Source ?? "HRActionsApproval");
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@RequestUrl", Request.Url?.ToString() ?? "");
                        cmd.Parameters.AddWithValue("@UserAgent", Request.UserAgent ?? "");
                        cmd.Parameters.AddWithValue("@IPAddress", GetClientIP());
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
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

        private void LogUserActivity(string action, string description)
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
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@Module", "HR Actions Approval");
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@IPAddress", GetClientIP());
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