using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.LeaveManagement
{
    public partial class EditLeaveBalance : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private int EmployeeId
        {
            get
            {
                if (int.TryParse(Request.QueryString["empId"], out int empId))
                    return empId;
                return 0;
            }
        }

        private string LeaveType
        {
            get { return Request.QueryString["type"] ?? ""; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check authorization
                if (!IsAuthorizedUser())
                {
                    Response.Redirect("~/Dashboard.aspx");
                    return;
                }

                // Validate parameters
                if (EmployeeId == 0 || string.IsNullOrEmpty(LeaveType))
                {
                    ShowMessage("Invalid employee or leave type specified.", "error");
                    Response.Redirect("~/LeaveManagement/LeaveManagement.aspx");
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
                return userRole == "HRAdmin" || userRole == "Admin" || userRole == "ProgramDirector";
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
                LoadEmployeeInfo();
                LoadCurrentBalance();
                LoadBalanceHistory();
                SetCurrentYear();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading page data.", "error");
            }
        }

        private void LoadEmployeeInfo()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName,
                            ISNULL(e.EmployeeNumber, 'EMP' + RIGHT('0000' + CAST(e.Id AS varchar(4)), 4)) as EmployeeNumber,
                            ISNULL(d.Name, 'Not Assigned') as Department
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        WHERE e.Id = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litEmployeeName.Text = reader["EmployeeName"].ToString();
                                litEmployeeNumber.Text = reader["EmployeeNumber"].ToString();
                                litDepartment.Text = reader["Department"].ToString();
                                litLeaveType.Text = LeaveType;

                                // Store in hidden fields
                                hfEmployeeId.Value = EmployeeId.ToString();
                                hfLeaveType.Value = LeaveType;
                            }
                            else
                            {
                                ShowMessage("Employee not found.", "error");
                                Response.Redirect("~/LeaveManagement/LeaveManagement.aspx");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading employee information.", "error");
            }
        }

        private void LoadCurrentBalance()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            ISNULL(lb.AllocatedDays, lt.DefaultAllocation) as AllocatedDays,
                            ISNULL(lb.UsedDays, 0) as UsedDays,
                            ISNULL(lb.EffectiveDate, DATEFROMPARTS(YEAR(GETDATE()), 1, 1)) as EffectiveDate,
                            lb.ExpiryDate
                        FROM LeaveTypes lt
                        LEFT JOIN LeaveBalances lb ON lt.TypeName = lb.LeaveType AND lb.EmployeeId = @EmployeeId
                        WHERE lt.TypeName = @LeaveType AND lt.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmd.Parameters.AddWithValue("@LeaveType", LeaveType);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                decimal allocatedDays = Convert.ToDecimal(reader["AllocatedDays"]);
                                decimal usedDays = Convert.ToDecimal(reader["UsedDays"]);
                                decimal remainingDays = allocatedDays - usedDays;

                                // Display current values
                                litCurrentAllocated.Text = allocatedDays.ToString("N1");
                                litCurrentUsed.Text = usedDays.ToString("N1");
                                litCurrentRemaining.Text = remainingDays.ToString("N1");

                                // Populate form fields
                                txtAllocatedDays.Text = allocatedDays.ToString("0.#");
                                txtUsedDays.Text = usedDays.ToString("0.#");
                                txtEffectiveDate.Text = Convert.ToDateTime(reader["EffectiveDate"]).ToString("yyyy-MM-dd");

                                if (reader["ExpiryDate"] != DBNull.Value)
                                {
                                    txtExpiryDate.Text = Convert.ToDateTime(reader["ExpiryDate"]).ToString("yyyy-MM-dd");
                                }

                                // Store original values for comparison
                                hfOriginalAllocated.Value = allocatedDays.ToString();
                                hfOriginalUsed.Value = usedDays.ToString();
                            }
                            else
                            {
                                ShowMessage("Leave type not found or not active.", "error");
                                Response.Redirect("~/LeaveManagement/LeaveManagement.aspx");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading current balance.", "error");
            }
        }

        private void LoadBalanceHistory()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            lbh.ChangeDate,
                            lbh.ChangeType,
                            lbh.PreviousAllocated,
                            lbh.NewAllocated,
                            lbh.PreviousUsed,
                            lbh.NewUsed,
                            lbh.Reason,
                            CONCAT(e.FirstName, ' ', e.LastName) as ChangedBy
                        FROM LeaveBalanceHistory lbh
                        LEFT JOIN Employees e ON lbh.ChangedBy = e.UserId
                        WHERE lbh.EmployeeId = @EmployeeId AND lbh.LeaveType = @LeaveType
                        ORDER BY lbh.ChangeDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmd.Parameters.AddWithValue("@LeaveType", LeaveType);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            gvBalanceHistory.DataSource = dt;
                            gvBalanceHistory.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                // Don't show error for history loading failure
            }
        }

        private void SetCurrentYear()
        {
            litCurrentYear.Text = DateTime.Now.Year.ToString();
        }

        #endregion

        #region Event Handlers

        protected void btnBackToLeaveManagement_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/LeaveManagement/LeaveManagement.aspx");
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveBalanceChanges();
            }
        }

        protected void btnResetToDefault_Click(object sender, EventArgs e)
        {
            ResetBalanceToDefault();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/LeaveManagement/LeaveManagement.aspx");
        }

        protected void cvUsedDays_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (decimal.TryParse(txtAllocatedDays.Text, out decimal allocated) &&
                decimal.TryParse(args.Value, out decimal used))
            {
                args.IsValid = used <= allocated;
            }
            else
            {
                args.IsValid = false;
            }
        }

        #endregion

        #region Balance Management Methods

        private void SaveBalanceChanges()
        {
            try
            {
                decimal newAllocated = Convert.ToDecimal(txtAllocatedDays.Text);
                decimal newUsed = Convert.ToDecimal(txtUsedDays.Text);
                DateTime effectiveDate = Convert.ToDateTime(txtEffectiveDate.Text);
                DateTime? expiryDate = string.IsNullOrEmpty(txtExpiryDate.Text) ? (DateTime?)null : Convert.ToDateTime(txtExpiryDate.Text);
                string reason = txtAdjustmentReason.Text.Trim();

                decimal originalAllocated = Convert.ToDecimal(hfOriginalAllocated.Value);
                decimal originalUsed = Convert.ToDecimal(hfOriginalUsed.Value);

                // Check if there are actually changes
                if (newAllocated == originalAllocated && newUsed == originalUsed)
                {
                    ShowMessage("No changes detected.", "warning");
                    return;
                }

                int currentUserId = Convert.ToInt32(Session["UserId"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Check if balance record exists
                            string checkQuery = "SELECT COUNT(*) FROM LeaveBalances WHERE EmployeeId = @EmployeeId AND LeaveType = @LeaveType";
                            bool balanceExists = false;

                            using (SqlCommand cmd = new SqlCommand(checkQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                                cmd.Parameters.AddWithValue("@LeaveType", LeaveType);
                                balanceExists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                            }

                            if (balanceExists)
                            {
                                // Update existing balance
                                string updateQuery = @"
                                    UPDATE LeaveBalances 
                                    SET AllocatedDays = @AllocatedDays,
                                        UsedDays = @UsedDays,
                                        EffectiveDate = @EffectiveDate,
                                        ExpiryDate = @ExpiryDate
                                    WHERE EmployeeId = @EmployeeId AND LeaveType = @LeaveType";

                                using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                                    cmd.Parameters.AddWithValue("@LeaveType", LeaveType);
                                    cmd.Parameters.AddWithValue("@AllocatedDays", newAllocated);
                                    cmd.Parameters.AddWithValue("@UsedDays", newUsed);
                                    cmd.Parameters.AddWithValue("@EffectiveDate", effectiveDate);
                                    cmd.Parameters.AddWithValue("@ExpiryDate", expiryDate ?? (object)DBNull.Value);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                // Insert new balance record
                                string insertQuery = @"
                                    INSERT INTO LeaveBalances (EmployeeId, LeaveType, AllocatedDays, UsedDays, EffectiveDate, ExpiryDate)
                                    VALUES (@EmployeeId, @LeaveType, @AllocatedDays, @UsedDays, @EffectiveDate, @ExpiryDate)";

                                using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                                    cmd.Parameters.AddWithValue("@LeaveType", LeaveType);
                                    cmd.Parameters.AddWithValue("@AllocatedDays", newAllocated);
                                    cmd.Parameters.AddWithValue("@UsedDays", newUsed);
                                    cmd.Parameters.AddWithValue("@EffectiveDate", effectiveDate);
                                    cmd.Parameters.AddWithValue("@ExpiryDate", expiryDate ?? (object)DBNull.Value);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // Log the balance change
                            LogBalanceChange(EmployeeId, LeaveType, originalAllocated, newAllocated,
                                           originalUsed, newUsed, reason, currentUserId, conn, transaction);

                            // Log application action
                            LogApplicationAction($"Balance updated for employee {EmployeeId}, leave type {LeaveType}. " +
                                               $"Allocated: {originalAllocated} -> {newAllocated}, Used: {originalUsed} -> {newUsed}. " +
                                               $"Reason: {reason}", currentUserId, conn, transaction);

                            transaction.Commit();

                            ShowMessage("Leave balance updated successfully.", "success");

                            // Reload the current balance display
                            LoadCurrentBalance();
                            LoadBalanceHistory();

                            // Clear the reason field
                            txtAdjustmentReason.Text = "";
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
                ShowMessage("Error saving balance changes.", "error");
            }
        }

        private void ResetBalanceToDefault()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get default allocation for this leave type
                    string getDefaultQuery = "SELECT DefaultAllocation FROM LeaveTypes WHERE TypeName = @LeaveType AND IsActive = 1";
                    decimal defaultAllocation = 0;

                    using (SqlCommand cmd = new SqlCommand(getDefaultQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@LeaveType", LeaveType);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            defaultAllocation = Convert.ToDecimal(result);
                        }
                        else
                        {
                            ShowMessage("Default allocation not found for this leave type.", "error");
                            return;
                        }
                    }

                    // Reset form fields
                    txtAllocatedDays.Text = defaultAllocation.ToString("0.#");
                    txtUsedDays.Text = "0";
                    txtEffectiveDate.Text = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy-MM-dd");
                    txtExpiryDate.Text = "";
                    txtAdjustmentReason.Text = $"Reset to default allocation for {LeaveType} leave type";

                    ShowMessage($"Form reset to default allocation ({defaultAllocation} days). Click 'Save Changes' to apply.", "info");
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error resetting to default values.", "error");
            }
        }

        #endregion

        #region Logging Methods

        private void LogBalanceChange(int employeeId, string leaveType, decimal prevAllocated, decimal newAllocated,
                                    decimal prevUsed, decimal newUsed, string reason, int changedBy,
                                    SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                // Create the LeaveBalanceHistory table if it doesn't exist
                string createTableQuery = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LeaveBalanceHistory]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[LeaveBalanceHistory](
                            [Id] [int] IDENTITY(1,1) NOT NULL,
                            [EmployeeId] [int] NOT NULL,
                            [LeaveType] [nvarchar](50) NOT NULL,
                            [ChangeDate] [datetime2](7) NOT NULL DEFAULT GETDATE(),
                            [ChangeType] [nvarchar](20) NOT NULL,
                            [PreviousAllocated] [decimal](5, 2) NULL,
                            [NewAllocated] [decimal](5, 2) NULL,
                            [PreviousUsed] [decimal](5, 2) NULL,
                            [NewUsed] [decimal](5, 2) NULL,
                            [Reason] [nvarchar](1000) NULL,
                            [ChangedBy] [int] NOT NULL,
                            CONSTRAINT [PK_LeaveBalanceHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
                        )
                    END";

                using (SqlCommand cmd = new SqlCommand(createTableQuery, conn, transaction))
                {
                    cmd.ExecuteNonQuery();
                }

                // Insert history record
                string insertHistoryQuery = @"
                    INSERT INTO LeaveBalanceHistory 
                    (EmployeeId, LeaveType, ChangeDate, ChangeType, PreviousAllocated, NewAllocated, 
                     PreviousUsed, NewUsed, Reason, ChangedBy)
                    VALUES 
                    (@EmployeeId, @LeaveType, GETDATE(), @ChangeType, @PreviousAllocated, @NewAllocated,
                     @PreviousUsed, @NewUsed, @Reason, @ChangedBy)";

                using (SqlCommand cmd = new SqlCommand(insertHistoryQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@LeaveType", leaveType);
                    cmd.Parameters.AddWithValue("@ChangeType", "MANUAL_ADJUSTMENT");
                    cmd.Parameters.AddWithValue("@PreviousAllocated", prevAllocated);
                    cmd.Parameters.AddWithValue("@NewAllocated", newAllocated);
                    cmd.Parameters.AddWithValue("@PreviousUsed", prevUsed);
                    cmd.Parameters.AddWithValue("@NewUsed", newUsed);
                    cmd.Parameters.AddWithValue("@Reason", reason);
                    cmd.Parameters.AddWithValue("@ChangedBy", changedBy);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                // Don't throw as this is not critical to the main operation
            }
        }

        private void LogApplicationAction(string action, int userId, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                string logQuery = @"
                    INSERT INTO ApplicationLogs (EventType, Message, Timestamp, Source, Level, UserId)
                    VALUES ('Leave Balance Management', @Message, GETDATE(), 'EditLeaveBalance', 'Info', @UserId)";

                using (SqlCommand cmd = new SqlCommand(logQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@Message", action);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                // Don't throw as this is not critical
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
                        INSERT INTO ErrorLogs (ErrorMessage, StackTrace, Source, Timestamp, UserId, Severity)
                        VALUES (@ErrorMessage, @StackTrace, @Source, GETDATE(), @UserId, 'High')";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message);
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", "EditLeaveBalance.aspx");
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

        #endregion
    }
}