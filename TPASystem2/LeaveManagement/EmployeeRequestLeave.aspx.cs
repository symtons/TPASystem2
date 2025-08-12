using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace TPASystem2.LeaveManagement
{
    public partial class EmployeeRequestLeave : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int CurrentUserId => Convert.ToInt32(Session["UserId"] ?? "0");
        private int CurrentEmployeeId => Convert.ToInt32(Session["EmployeeId"] ?? "0");
        private bool IsEditMode => !string.IsNullOrEmpty(Request.QueryString["edit"]);
        private int EditRequestId => Convert.ToInt32(Request.QueryString["edit"] ?? "0");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidateUserAccess();
                LoadEmployeeInformation();
                LoadLeaveTypes();
                SetDateValidators();

                if (IsEditMode)
                {
                    LoadRequestForEdit();
                    litFormMode.Text = "Edit Request";
                }
                else
                {
                    litFormMode.Text = "New Request";
                }
            }
        }

        #region User Access and Employee Info

        private void ValidateUserAccess()
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                return;
            }

            // Ensure employee has access to leave management
            try
            {
                string userRole = Session["UserRole"]?.ToString();
                if (!string.IsNullOrEmpty(userRole))
                {
                    var allowedRoles = new[] { "EMPLOYEE", "SUPERVISOR", "PROGRAMDIRECTOR", "HRADMIN", "ADMIN", "SUPERADMIN" };
                    if (!Array.Exists(allowedRoles, role => role.Equals(userRole, StringComparison.OrdinalIgnoreCase)))
                    {
                        Response.Redirect("~/Unauthorized.aspx", false);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void LoadEmployeeInformation()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            e.FirstName + ' ' + e.LastName as FullName,
                            e.EmployeeNumber
                        FROM Employees e
                        WHERE e.UserId = @UserId AND e.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litEmployeeName.Text = reader["FullName"].ToString();
                                litEmployeeNumber.Text = reader["EmployeeNumber"].ToString();

                                // Store employee ID in session for later use
                                if (Session["EmployeeId"] == null)
                                {
                                    Session["EmployeeId"] = GetEmployeeIdFromUserId(CurrentUserId);
                                }
                            }
                            else
                            {
                                ShowMessage("Employee information not found. Please contact HR.", "error");
                                pnlRequestForm.Visible = false;
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading employee information.", "error");
                pnlRequestForm.Visible = false;
            }
        }

        private int GetEmployeeIdFromUserId(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id FROM Employees WHERE UserId = @UserId AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return 0;
            }
        }

        #endregion

        #region Leave Types and Balance Management

        private void LoadLeaveTypes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT Id, TypeName, RequiresApproval, MinAdvanceNotice
                        FROM LeaveTypes 
                        WHERE IsActive = 1 
                        ORDER BY TypeName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlLeaveType.Items.Clear();
                            ddlLeaveType.Items.Add(new ListItem("Select leave type...", ""));

                            while (reader.Read())
                            {
                                string typeName = reader["TypeName"].ToString();
                                ddlLeaveType.Items.Add(new ListItem(typeName, typeName));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading leave types.", "error");
            }
        }

        private void LoadLeaveBalance(string leaveType)
        {
            if (string.IsNullOrEmpty(leaveType))
            {
                txtAvailableBalance.Text = "Select leave type first";
                return;
            }

            try
            {
                int employeeId = CurrentEmployeeId;
                if (employeeId == 0)
                {
                    // Try to get employee ID from database
                    employeeId = GetEmployeeIdFromUserId(CurrentUserId);
                    if (employeeId == 0)
                    {
                        txtAvailableBalance.Text = "Employee not found";
                        return;
                    }
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Updated query to work with your LeaveBalances schema (LeaveType as string)
                    string query = @"
                        SELECT 
                            ISNULL(lb.AllocatedDays, lt.DefaultAllocation) as Total,
                            ISNULL(lb.UsedDays, 0) as Used,
                            (ISNULL(lb.AllocatedDays, lt.DefaultAllocation) - ISNULL(lb.UsedDays, 0)) as Available
                        FROM LeaveTypes lt
                        LEFT JOIN LeaveBalances lb ON lt.TypeName = lb.LeaveType AND lb.EmployeeId = @EmployeeId
                        WHERE lt.TypeName = @LeaveType AND lt.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        cmd.Parameters.AddWithValue("@LeaveType", leaveType);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                decimal available = Convert.ToDecimal(reader["Available"]);
                                txtAvailableBalance.Text = $"{available} days available";
                            }
                            else
                            {
                                txtAvailableBalance.Text = "Leave type not found";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                txtAvailableBalance.Text = "Error loading balance";
            }
        }

        #endregion

        #region Form Setup and Validation

        private void SetDateValidators()
        {
            cvStartDate.ValueToCompare = DateTime.Today.ToString("yyyy-MM-dd");
        }

        private void LoadRequestForEdit()
        {
            if (EditRequestId == 0) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            LeaveType, StartDate, EndDate, DaysRequested, 
                            Reason, Status, WorkflowStatus
                        FROM LeaveRequests 
                        WHERE Id = @RequestId AND EmployeeId = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@RequestId", EditRequestId);
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string status = reader["Status"].ToString();

                                // Only allow editing of pending or draft requests
                                if (status != "Pending" && status != "Draft")
                                {
                                    ShowMessage("Only pending or draft requests can be edited.", "error");
                                    Response.Redirect("EmployeeLeavePortal.aspx", false);
                                    return;
                                }

                                // Populate form fields
                                ddlLeaveType.SelectedValue = reader["LeaveType"].ToString();
                                txtStartDate.Text = Convert.ToDateTime(reader["StartDate"]).ToString("yyyy-MM-dd");
                                txtEndDate.Text = Convert.ToDateTime(reader["EndDate"]).ToString("yyyy-MM-dd");
                                txtReason.Text = reader["Reason"].ToString();

                                // Load balance for selected leave type
                                LoadLeaveBalance(ddlLeaveType.SelectedValue);

                                // Calculate days
                                CalculateDaysRequested();

                                hfEditingRequestId.Value = EditRequestId.ToString();
                            }
                            else
                            {
                                ShowMessage("Leave request not found or access denied.", "error");
                                Response.Redirect("EmployeeLeavePortal.aspx", false);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading request for editing.", "error");
            }
        }

        #endregion

        #region Business Logic

        private void CalculateDaysRequested()
        {
            try
            {
                if (!DateTime.TryParse(txtStartDate.Text, out DateTime startDate) ||
                    !DateTime.TryParse(txtEndDate.Text, out DateTime endDate))
                {
                    txtDaysRequested.Text = "0";
                    return;
                }

                if (startDate > endDate)
                {
                    txtDaysRequested.Text = "0";
                    return;
                }

                decimal businessDays = CalculateBusinessDays(startDate, endDate);

                // Apply half-day calculation if checked
                if (chkHalfDay.Checked && businessDays > 0)
                {
                    businessDays = 0.5m;
                }

                txtDaysRequested.Text = businessDays.ToString("F1");
            }
            catch (Exception ex)
            {
                LogError(ex);
                txtDaysRequested.Text = "0";
            }
        }

        private decimal CalculateBusinessDays(DateTime startDate, DateTime endDate)
        {
            decimal businessDays = 0;
            DateTime current = startDate;

            while (current <= endDate)
            {
                if (current.DayOfWeek != DayOfWeek.Saturday && current.DayOfWeek != DayOfWeek.Sunday)
                {
                    businessDays++;
                }
                current = current.AddDays(1);
            }

            return businessDays;
        }

        private int GetProgramDirectorId(int employeeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First try to get the employee's direct manager
                    string query = @"
                        SELECT ISNULL(e.ManagerId, pd.Id) as DirectorId
                        FROM Employees e
                        CROSS JOIN (
                            SELECT TOP 1 Id FROM Employees 
                            WHERE JobTitle LIKE '%Program Director%' 
                            AND IsActive = 1
                        ) pd
                        WHERE e.Id = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return 0;
            }
        }

        #endregion

        #region Event Handlers

        protected void ddlLeaveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLeaveBalance(ddlLeaveType.SelectedValue);
            CalculateDaysRequested();
        }

        protected void txtStartDate_TextChanged(object sender, EventArgs e)
        {
            CalculateDaysRequested();
        }

        protected void txtEndDate_TextChanged(object sender, EventArgs e)
        {
            CalculateDaysRequested();
        }

        protected void chkHalfDay_CheckedChanged(object sender, EventArgs e)
        {
            CalculateDaysRequested();
        }

        protected void btnSubmitRequest_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && ValidateLeaveRequest())
            {
                if (IsEditMode)
                {
                    UpdateLeaveRequest("Pending");
                }
                else
                {
                    SubmitLeaveRequest("Pending");
                }
            }
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            if (IsEditMode)
            {
                UpdateLeaveRequest("Draft");
            }
            else
            {
                SubmitLeaveRequest("Draft");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("EmployeeLeavePortal.aspx", false);
        }

        protected void btnBackToPortal_Click(object sender, EventArgs e)
        {
            Response.Redirect("EmployeeLeavePortal.aspx", false);
        }

        protected void btnViewMyRequests_Click(object sender, EventArgs e)
        {
            Response.Redirect("MyLeaves.aspx", false);
        }

        protected void btnCreateAnother_Click(object sender, EventArgs e)
        {
            Response.Redirect("EmployeeRequestLeave.aspx", false);
        }

        #endregion

        #region Request Submission and Update

        private bool ValidateLeaveRequest()
        {
            try
            {
                // Validate leave type selection
                if (string.IsNullOrEmpty(ddlLeaveType.SelectedValue))
                {
                    ShowMessage("Please select a leave type.", "error");
                    return false;
                }

                // Validate dates
                if (!DateTime.TryParse(txtStartDate.Text, out DateTime startDate) ||
                    !DateTime.TryParse(txtEndDate.Text, out DateTime endDate))
                {
                    ShowMessage("Please provide valid start and end dates.", "error");
                    return false;
                }

                if (startDate < DateTime.Today)
                {
                    ShowMessage("Start date cannot be in the past.", "error");
                    return false;
                }

                if (endDate < startDate)
                {
                    ShowMessage("End date must be after start date.", "error");
                    return false;
                }

                // Validate reason
                if (string.IsNullOrWhiteSpace(txtReason.Text))
                {
                    ShowMessage("Please provide a reason for your leave request.", "error");
                    return false;
                }

                // Validate balance (only for submitted requests, not drafts)
                if (Page.IsValid)
                {
                    decimal daysRequested = Convert.ToDecimal(txtDaysRequested.Text);
                    if (!CheckLeaveBalance(ddlLeaveType.SelectedValue, daysRequested))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error validating leave request.", "error");
                return false;
            }
        }

        private bool CheckLeaveBalance(string leaveType, decimal daysRequested)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            (ISNULL(lb.AllocatedDays, lt.DefaultAllocation) - ISNULL(lb.UsedDays, 0)) as Available
                        FROM LeaveTypes lt
                        LEFT JOIN LeaveBalances lb ON lt.Id = lb.LeaveTypeId AND lb.EmployeeId = @EmployeeId
                        WHERE lt.TypeName = @LeaveType AND lt.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        cmd.Parameters.AddWithValue("@LeaveType", leaveType);

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            decimal available = Convert.ToDecimal(result);
                            if (daysRequested > available)
                            {
                                ShowMessage($"Insufficient leave balance. You have {available} days available.", "error");
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error checking leave balance.", "error");
                return false;
            }
        }

        private void SubmitLeaveRequest(string status)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Get Program Director ID for approval workflow
                            int programDirectorId = GetProgramDirectorId(CurrentEmployeeId);

                            string insertQuery = @"
                                INSERT INTO LeaveRequests (
                                    EmployeeId, LeaveType, StartDate, EndDate, DaysRequested, 
                                    Reason, Status, RequestedAt, CreatedAt, WorkflowStatus, 
                                    CurrentApprovalStep, ReviewedById
                                ) 
                                OUTPUT INSERTED.Id
                                VALUES (
                                    @EmployeeId, @LeaveType, @StartDate, @EndDate, @DaysRequested, 
                                    @Reason, @Status, @RequestedAt, @CreatedAt, @WorkflowStatus, 
                                    @CurrentApprovalStep, @ReviewedById
                                )";

                            int newRequestId;
                            using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                                cmd.Parameters.AddWithValue("@LeaveType", ddlLeaveType.SelectedValue);
                                cmd.Parameters.AddWithValue("@StartDate", DateTime.Parse(txtStartDate.Text));
                                cmd.Parameters.AddWithValue("@EndDate", DateTime.Parse(txtEndDate.Text));
                                cmd.Parameters.AddWithValue("@DaysRequested", Convert.ToDecimal(txtDaysRequested.Text));
                                cmd.Parameters.AddWithValue("@Reason", txtReason.Text.Trim());
                                cmd.Parameters.AddWithValue("@Status", status);
                                cmd.Parameters.AddWithValue("@RequestedAt", DateTime.UtcNow);
                                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                                cmd.Parameters.AddWithValue("@WorkflowStatus", status == "Pending" ? "PENDING_DIRECTOR_APPROVAL" : "DRAFT");
                                cmd.Parameters.AddWithValue("@CurrentApprovalStep", 1);
                                cmd.Parameters.AddWithValue("@ReviewedById", status == "Pending" ? (object)programDirectorId : DBNull.Value);

                                newRequestId = (int)cmd.ExecuteScalar();
                            }

                            // If submitting (not draft), send notification to Program Director
                            if (status == "Pending" && programDirectorId > 0)
                            {
                                SendApprovalNotification(newRequestId, programDirectorId, conn, transaction);
                            }

                            transaction.Commit();

                            // Show success and redirect
                            ShowSuccessPanel(newRequestId);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error submitting leave request. Please try again.", "error");
            }
        }

        private void UpdateLeaveRequest(string status)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            int requestId = Convert.ToInt32(hfEditingRequestId.Value);
                            int programDirectorId = GetProgramDirectorId(CurrentEmployeeId);

                            string updateQuery = @"
                                UPDATE LeaveRequests 
                                SET LeaveType = @LeaveType,
                                    StartDate = @StartDate,
                                    EndDate = @EndDate,
                                    DaysRequested = @DaysRequested,
                                    Reason = @Reason,
                                    Status = @Status,
                                    WorkflowStatus = @WorkflowStatus,
                                    ReviewedById = @ReviewedById,
                                    RequestedAt = CASE WHEN @Status = 'Pending' THEN @RequestedAt ELSE RequestedAt END
                                WHERE Id = @RequestId AND EmployeeId = @EmployeeId";

                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@RequestId", requestId);
                                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                                cmd.Parameters.AddWithValue("@LeaveType", ddlLeaveType.SelectedValue);
                                cmd.Parameters.AddWithValue("@StartDate", DateTime.Parse(txtStartDate.Text));
                                cmd.Parameters.AddWithValue("@EndDate", DateTime.Parse(txtEndDate.Text));
                                cmd.Parameters.AddWithValue("@DaysRequested", Convert.ToDecimal(txtDaysRequested.Text));
                                cmd.Parameters.AddWithValue("@Reason", txtReason.Text.Trim());
                                cmd.Parameters.AddWithValue("@Status", status);
                                cmd.Parameters.AddWithValue("@WorkflowStatus", status == "Pending" ? "PENDING_DIRECTOR_APPROVAL" : "DRAFT");
                                cmd.Parameters.AddWithValue("@ReviewedById", status == "Pending" ? (object)programDirectorId : DBNull.Value);
                                cmd.Parameters.AddWithValue("@RequestedAt", DateTime.UtcNow);

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    ShowMessage("Request not found or access denied.", "error");
                                    return;
                                }
                            }

                            // If changing from draft to pending, send notification
                            if (status == "Pending" && programDirectorId > 0)
                            {
                                SendApprovalNotification(requestId, programDirectorId, conn, transaction);
                            }

                            transaction.Commit();

                            // Show success and redirect
                            ShowSuccessPanel(requestId);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error updating leave request. Please try again.", "error");
            }
        }

        private void SendApprovalNotification(int requestId, int programDirectorId, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                // Get Program Director email
                string directorQuery = "SELECT Email FROM Employees WHERE Id = @DirectorId";
                string directorEmail = "";

                using (SqlCommand cmd = new SqlCommand(directorQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@DirectorId", programDirectorId);
                    object result = cmd.ExecuteScalar();
                    directorEmail = result?.ToString() ?? "";
                }

                if (!string.IsNullOrEmpty(directorEmail))
                {
                    // Insert notification into email queue
                    string notificationQuery = @"
                        INSERT INTO EmailQueue (
                            ToEmail, Subject, Body, EmailType, 
                            Priority, ScheduledSendTime, CreatedAt
                        ) VALUES (
                            @ToEmail, @Subject, @Body, @EmailType, 
                            @Priority, @ScheduledSendTime, @CreatedAt
                        )";

                    using (SqlCommand cmd = new SqlCommand(notificationQuery, conn, transaction))
                    {
                        string subject = $"Leave Request Approval Required - {litEmployeeName.Text}";
                        string body = CreateApprovalEmailBody(requestId);

                        cmd.Parameters.AddWithValue("@ToEmail", directorEmail);
                        cmd.Parameters.AddWithValue("@Subject", subject);
                        cmd.Parameters.AddWithValue("@Body", body);
                        cmd.Parameters.AddWithValue("@EmailType", "LeaveApproval");
                        cmd.Parameters.AddWithValue("@Priority", "Normal");
                        cmd.Parameters.AddWithValue("@ScheduledSendTime", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                // Don't fail the main transaction for notification errors
            }
        }

        private string CreateApprovalEmailBody(int requestId)
        {
            return $@"
                <h3>Leave Request Approval Required</h3>
                <p>A new leave request has been submitted and requires your approval:</p>
                
                <table style='border-collapse: collapse; width: 100%;'>
                    <tr><td style='padding: 8px; border: 1px solid #ddd;'><strong>Employee:</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{litEmployeeName.Text}</td></tr>
                    <tr><td style='padding: 8px; border: 1px solid #ddd;'><strong>Employee Number:</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{litEmployeeNumber.Text}</td></tr>
                    <tr><td style='padding: 8px; border: 1px solid #ddd;'><strong>Leave Type:</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{ddlLeaveType.SelectedValue}</td></tr>
                    <tr><td style='padding: 8px; border: 1px solid #ddd;'><strong>Start Date:</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{DateTime.Parse(txtStartDate.Text):MMM dd, yyyy}</td></tr>
                    <tr><td style='padding: 8px; border: 1px solid #ddd;'><strong>End Date:</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{DateTime.Parse(txtEndDate.Text):MMM dd, yyyy}</td></tr>
                    <tr><td style='padding: 8px; border: 1px solid #ddd;'><strong>Days Requested:</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{txtDaysRequested.Text}</td></tr>
                    <tr><td style='padding: 8px; border: 1px solid #ddd;'><strong>Reason:</strong></td><td style='padding: 8px; border: 1px solid #ddd;'>{txtReason.Text}</td></tr>
                </table>
                
                <p>Please log in to the HR system to review and approve this request.</p>
                <p><strong>Request ID:</strong> {requestId}</p>
            ";
        }

        #endregion

        #region Success Display

        private void ShowSuccessPanel(int requestId)
        {
            pnlRequestForm.Visible = false;
            pnlSuccess.Visible = true;

            litRequestId.Text = requestId.ToString();
            litSubmittedLeaveType.Text = ddlLeaveType.SelectedValue;
            litSubmittedDates.Text = $"{DateTime.Parse(txtStartDate.Text):MMM dd} - {DateTime.Parse(txtEndDate.Text):MMM dd, yyyy}";
        }

        #endregion

        #region Helper Methods

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            litMessage.Text = message;
            pnlMessage.CssClass = $"alert alert-{type}";
        }

        private void LogError(Exception ex)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ErrorLogs (ErrorMessage, StackTrace, Source, RequestUrl, UserId, CreatedAt)
                        VALUES (@ErrorMessage, @StackTrace, @Source, @RequestUrl, @UserId, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message);
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", ex.Source ?? "EmployeeRequestLeave");
                        cmd.Parameters.AddWithValue("@RequestUrl", Request.Url?.ToString() ?? "");
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Silent fail on logging errors
            }
        }

        #endregion
    }
}