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
        private int CurrentUserId => Convert.ToInt32(Session["UserId"] ?? "0");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidateUserAccess();
                LoadEmployeeInformation();
                LoadLeaveTypes();
                SetDateValidators();
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
                            e.EmployeeNumber,
                            ISNULL(d.Name, 'No Department') as DepartmentName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        WHERE e.UserId = @UserId AND e.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string fullName = reader["FullName"].ToString();
                                string employeeNumber = reader["EmployeeNumber"].ToString();
                                string departmentName = reader["DepartmentName"].ToString();

                                // Set hidden fields for form processing
                                txtEmployeeName.Text = fullName;
                                txtDepartment.Text = departmentName;

                                // Update client-side display using JavaScript
                                string script = $@"
                                    document.getElementById('employeeNameDisplay').textContent = '{fullName}';
                                    document.getElementById('employeeNumberDisplay').textContent = '{employeeNumber}';
                                ";
                                ClientScript.RegisterStartupScript(this.GetType(), "UpdateEmployeeInfo", script, true);
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
                        SELECT TypeName
                        FROM LeaveTypes 
                        WHERE IsActive = 1 
                        ORDER BY TypeName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        ddlLeaveType.Items.Clear();
                        ddlLeaveType.Items.Add(new ListItem("Select leave type...", ""));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
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
                int employeeId = GetEmployeeIdFromUserId(CurrentUserId);
                if (employeeId == 0)
                {
                    txtAvailableBalance.Text = "Employee not found";
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Query to get balance using LeaveType string (matching your schema)
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
                                // If no record found, create default balance
                                CreateDefaultLeaveBalance(employeeId, leaveType);
                                txtAvailableBalance.Text = "Balance created - please refresh";
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

        private void CreateDefaultLeaveBalance(int employeeId, string leaveType)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get default allocation for this leave type
                    string getDefaultQuery = "SELECT DefaultAllocation FROM LeaveTypes WHERE TypeName = @LeaveType";
                    decimal defaultAllocation = 0;

                    using (SqlCommand cmd = new SqlCommand(getDefaultQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@LeaveType", leaveType);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            defaultAllocation = Convert.ToDecimal(result);
                        }
                    }

                    // Create balance record
                    string insertQuery = @"
                        INSERT INTO LeaveBalances (EmployeeId, LeaveType, AllocatedDays, UsedDays, EffectiveDate)
                        VALUES (@EmployeeId, @LeaveType, @AllocatedDays, 0, @EffectiveDate)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        cmd.Parameters.AddWithValue("@LeaveType", leaveType);
                        cmd.Parameters.AddWithValue("@AllocatedDays", defaultAllocation);
                        cmd.Parameters.AddWithValue("@EffectiveDate", DateTime.Today);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
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

        private void SetDateValidators()
        {
            cvStartDate.ValueToCompare = DateTime.Today.ToString("yyyy-MM-dd");
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
            Response.Redirect("~/Dashboard.aspx", false);
        }

        #endregion

        #region Request Submission

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

                // Validate balance
                decimal daysRequested = Convert.ToDecimal(txtDaysRequested.Text);
                if (!CheckLeaveBalance(ddlLeaveType.SelectedValue, daysRequested))
                {
                    return false;
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
                int employeeId = GetEmployeeIdFromUserId(CurrentUserId);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            (ISNULL(lb.AllocatedDays, lt.DefaultAllocation) - ISNULL(lb.UsedDays, 0)) as Available
                        FROM LeaveTypes lt
                        LEFT JOIN LeaveBalances lb ON lt.TypeName = lb.LeaveType AND lb.EmployeeId = @EmployeeId
                        WHERE lt.TypeName = @LeaveType AND lt.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
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
                int employeeId = GetEmployeeIdFromUserId(CurrentUserId);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string insertQuery = @"
                        INSERT INTO LeaveRequests (
                            EmployeeId, LeaveType, StartDate, EndDate, DaysRequested, 
                            Reason, Status, RequestedAt, CreatedAt
                        ) 
                        VALUES (
                            @EmployeeId, @LeaveType, @StartDate, @EndDate, @DaysRequested, 
                            @Reason, @Status, @RequestedAt, @CreatedAt
                        )";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        cmd.Parameters.AddWithValue("@LeaveType", ddlLeaveType.SelectedValue);
                        cmd.Parameters.AddWithValue("@StartDate", DateTime.Parse(txtStartDate.Text));
                        cmd.Parameters.AddWithValue("@EndDate", DateTime.Parse(txtEndDate.Text));
                        cmd.Parameters.AddWithValue("@DaysRequested", Convert.ToDecimal(txtDaysRequested.Text));
                        cmd.Parameters.AddWithValue("@Reason", txtReason.Text.Trim());
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@RequestedAt", DateTime.UtcNow);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            string message = status == "Pending" ?
                                "Leave request submitted successfully! Your Program Director will review it shortly." :
                                "Leave request saved as draft successfully.";

                            ShowMessage(message, "success");
                            ClearForm();
                        }
                        else
                        {
                            ShowMessage("Failed to submit leave request. Please try again.", "error");
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

        private void ClearForm()
        {
            ddlLeaveType.SelectedIndex = 0;
            txtAvailableBalance.Text = "Select leave type first";
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            txtDaysRequested.Text = "0";
            chkHalfDay.Checked = false;
            txtReason.Text = "";
        }

        #endregion

        #region Helper Methods

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
                        cmd.Parameters.AddWithValue("@Source", ex.Source ?? "RequestLeave");
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