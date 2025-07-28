using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.LeaveManagement
{
    public partial class RequestLeave : System.Web.UI.Page
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

                InitializePage();
                LoadEmployeeInformation();
                SetDateValidators();
            }
        }

        #region Page Initialization

        private void InitializePage()
        {
            cvStartDate.ValueToCompare = DateTime.Today.ToString("yyyy-MM-dd");
        }

        private void LoadEmployeeInformation()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            e.Id,
                            CONCAT(e.FirstName, ' ', e.LastName) as FullName,
                            d.Name as DepartmentName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        WHERE e.UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtEmployeeName.Text = reader["FullName"].ToString();
                                txtDepartment.Text = reader["DepartmentName"]?.ToString() ?? "Not Assigned";
                                ViewState["EmployeeId"] = reader["Id"];
                            }
                            else
                            {
                                ShowMessage("Employee information not found. Please contact HR.", "error");
                                pnlRequestForm.Visible = false;
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

        private void SetDateValidators()
        {
            cvStartDate.ValueToCompare = DateTime.Today.ToString("yyyy-MM-dd");
        }

        #endregion

        #region Leave Balance Management

        private void LoadLeaveBalance(string leaveType)
        {
            if (string.IsNullOrEmpty(leaveType))
            {
                txtAvailableBalance.Text = "--";
                return;
            }

            try
            {
                // In a real implementation, this would query leave balance tables
                // For now, showing sample balances based on leave type
                int availableBalance = GetAvailableBalance(leaveType);
                txtAvailableBalance.Text = availableBalance + " days";
            }
            catch (Exception ex)
            {
                LogError(ex);
                txtAvailableBalance.Text = "Error loading balance";
            }
        }

        private int GetAvailableBalance(string leaveType)
        {
            // Sample balance logic - replace with actual database query in production
            switch (leaveType.ToLower())
            {
                case "vacation":
                    return 15;
                case "sick":
                    return 10;
                case "personal":
                    return 5;
                case "maternity":
                case "paternity":
                    return 90;
                case "bereavement":
                    return 3;
                case "emergency":
                    return 2;
                default:
                    return 0;
            }
        }

        #endregion

        #region Date Calculations

        private void CalculateDaysRequested()
        {
            if (!string.IsNullOrEmpty(txtStartDate.Text) && !string.IsNullOrEmpty(txtEndDate.Text))
            {
                try
                {
                    DateTime startDate = DateTime.Parse(txtStartDate.Text);
                    DateTime endDate = DateTime.Parse(txtEndDate.Text);

                    if (endDate >= startDate)
                    {
                        int businessDays = CalculateBusinessDays(startDate, endDate);

                        if (chkHalfDay.Checked && businessDays == 1)
                        {
                            txtDaysRequested.Text = "0.5";
                        }
                        else
                        {
                            txtDaysRequested.Text = businessDays.ToString();
                        }
                    }
                    else
                    {
                        txtDaysRequested.Text = "0";
                    }
                }
                catch
                {
                    txtDaysRequested.Text = "0";
                }
            }
            else
            {
                txtDaysRequested.Text = "0";
            }
        }

        private int CalculateBusinessDays(DateTime startDate, DateTime endDate)
        {
            int businessDays = 0;
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
            if (Page.IsValid)
            {
                SubmitLeaveRequest("Pending");
            }
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            SubmitLeaveRequest("Draft");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx", false);
        }

        protected void btnBackToDashboard_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx", false);
        }

        #endregion

        #region Leave Request Submission

        private void SubmitLeaveRequest(string status)
        {
            try
            {
                if (!ValidateRequest())
                    return;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO LeaveRequests 
                        (EmployeeId, LeaveType, StartDate, EndDate, DaysRequested, Reason, Status, RequestedAt, CreatedAt, WorkflowStatus, CurrentApprovalStep)
                        VALUES 
                        (@EmployeeId, @LeaveType, @StartDate, @EndDate, @DaysRequested, @Reason, @Status, GETDATE(), GETDATE(), @WorkflowStatus, @CurrentApprovalStep)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", ViewState["EmployeeId"]);
                        cmd.Parameters.AddWithValue("@LeaveType", ddlLeaveType.SelectedValue);
                        cmd.Parameters.AddWithValue("@StartDate", DateTime.Parse(txtStartDate.Text));
                        cmd.Parameters.AddWithValue("@EndDate", DateTime.Parse(txtEndDate.Text));

                        // Handle half-day requests
                        if (chkHalfDay.Checked)
                        {
                            cmd.Parameters.AddWithValue("@DaysRequested", 0.5);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@DaysRequested", CalculateBusinessDays(
                                DateTime.Parse(txtStartDate.Text),
                                DateTime.Parse(txtEndDate.Text)));
                        }

                        cmd.Parameters.AddWithValue("@Reason", txtReason.Text.Trim());
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@WorkflowStatus", status == "Draft" ? "DRAFT" : "PENDING");
                        cmd.Parameters.AddWithValue("@CurrentApprovalStep", 1);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            // Log the activity
                            LogUserActivity(Convert.ToInt32(Session["UserId"]),
                                          status == "Draft" ? "Leave Draft Saved" : "Leave Request Submitted",
                                          "LeaveRequests",
                                          $"Leave request for {ddlLeaveType.SelectedValue} from {txtStartDate.Text} to {txtEndDate.Text}",
                                          GetClientIP());

                            if (status == "Draft")
                            {
                                ShowMessage("Leave request saved as draft successfully.", "success");
                                ClearForm();
                            }
                            else
                            {
                                ShowMessage("Leave request submitted successfully. You will be notified once it's reviewed.", "success");

                                // TODO: Send email notification to manager
                                // TODO: Create notification record

                                // Redirect to dashboard after successful submission
                                Response.Redirect("Default.aspx?message=Leave request submitted successfully", false);
                            }
                        }
                        else
                        {
                            ShowMessage("Error submitting leave request. Please try again.", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("An error occurred while processing your request. Please try again.", "error");
            }
        }

        private bool ValidateRequest()
        {
            bool isValid = true;

            // Check if dates overlap with existing approved leave
            if (HasOverlappingLeave())
            {
                ShowMessage("You already have approved leave during this period.", "error");
                isValid = false;
            }

            // Check available balance
            decimal requestedDays = decimal.Parse(txtDaysRequested.Text);
            int availableBalance = GetAvailableBalance(ddlLeaveType.SelectedValue);

            if (requestedDays > availableBalance)
            {
                ShowMessage($"Insufficient leave balance. You have {availableBalance} days available.", "error");
                isValid = false;
            }

            // Check minimum notice period (except for sick and emergency leave)
            if (ddlLeaveType.SelectedValue == "Vacation" || ddlLeaveType.SelectedValue == "Personal")
            {
                DateTime startDate = DateTime.Parse(txtStartDate.Text);
                int daysNotice = (startDate - DateTime.Today).Days;
                int minimumNotice = ddlLeaveType.SelectedValue == "Vacation" ? 14 : 7;

                if (daysNotice < minimumNotice)
                {
                    ShowMessage($"{ddlLeaveType.SelectedValue} leave requires at least {minimumNotice} days advance notice.", "warning");
                    // Don't block submission, but warn user
                }
            }

            return isValid;
        }

        private bool HasOverlappingLeave()
        {
            try
            {
                DateTime startDate = DateTime.Parse(txtStartDate.Text);
                DateTime endDate = DateTime.Parse(txtEndDate.Text);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT COUNT(*) 
                        FROM LeaveRequests 
                        WHERE EmployeeId = @EmployeeId 
                        AND Status = 'Approved'
                        AND (
                            (@StartDate BETWEEN StartDate AND EndDate) OR
                            (@EndDate BETWEEN StartDate AND EndDate) OR
                            (StartDate BETWEEN @StartDate AND @EndDate) OR
                            (EndDate BETWEEN @StartDate AND @EndDate)
                        )";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", ViewState["EmployeeId"]);
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);

                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }
        }

        private void ClearForm()
        {
            ddlLeaveType.SelectedIndex = 0;
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            txtDaysRequested.Text = "0";
            txtReason.Text = "";
            txtAvailableBalance.Text = "--";
            chkHalfDay.Checked = false;
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
                        cmd.Parameters.AddWithValue("@Source", ex.Source ?? "LeaveManagement.RequestLeave");
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