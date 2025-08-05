using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.TimeManagement
{
    public partial class EmployeeTimesheets : System.Web.UI.Page
    {
        #region Properties and Fields

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private int CurrentEmployeeId
        {
            get
            {
                return GetCurrentEmployeeId();
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidateUserAccess();
                LoadEmployeeData();
                LoadTimesheets();
                LoadQuickStats();
            }
        }

        #endregion

        #region Button Events

        protected void btnBackToTimeTracking_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/TimeManagement/EmployeeTimeTracking.aspx");
        }

        protected void btnCreateTimesheet_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if there's already a timesheet for the current week
                DateTime currentWeekStart = GetCurrentWeekStart();

                if (TimesheetExistsForWeek(currentWeekStart))
                {
                    ShowMessage("A timesheet already exists for the current week. Please edit the existing timesheet instead.", "error");
                    return;
                }

                // Create new timesheet
                int timesheetId = CreateNewTimesheet(currentWeekStart);

                if (timesheetId > 0)
                {
                    Response.Redirect($"~/TimeManagement/EditTimesheet.aspx?id={timesheetId}");
                }
                else
                {
                    ShowMessage("Error creating timesheet. Please try again.", "error");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating timesheet: {ex.Message}");
                ShowMessage("An error occurred while creating the timesheet.", "error");
            }
        }

        protected void FilterChanged(object sender, EventArgs e)
        {
            LoadTimesheets();
        }

        protected void TimesheetAction_Command(object sender, CommandEventArgs e)
        {
            try
            {
                int timesheetId = Convert.ToInt32(e.CommandArgument);

                switch (e.CommandName)
                {
                    case "ViewTimesheet":
                        Response.Redirect($"~/TimeManagement/ViewTimesheet.aspx?id={timesheetId}");
                        break;

                    case "EditTimesheet":
                        Response.Redirect($"~/TimeManagement/EditTimesheet.aspx?id={timesheetId}");
                        break;

                    case "SubmitTimesheet":
                        SubmitTimesheet(timesheetId);
                        break;

                    case "DeleteTimesheet":
                        DeleteTimesheet(timesheetId);
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in TimesheetAction_Command: {ex.Message}");
                ShowMessage("An error occurred while processing the request.", "error");
            }
        }

        #endregion

        #region Validation and Access

        private void ValidateUserAccess()
        {
            try
            {
                string userRole = Session["UserRole"]?.ToString() ?? "";
                if (!userRole.Equals("EMPLOYEE", StringComparison.OrdinalIgnoreCase))
                {
                    Response.Redirect("~/Dashboard.aspx");
                    return;
                }

                if (CurrentEmployeeId <= 0)
                {
                    ShowMessage("Employee record not found. Please contact HR.", "error");
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating user access: {ex.Message}");
                Response.Redirect("~/Login.aspx");
            }
        }

        #endregion

        #region Data Loading Methods

        private void LoadEmployeeData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT e.FirstName, e.LastName, e.EmployeeNumber,
                               pc.FirstName + ' ' + pc.LastName AS ProgramCoordinatorName
                        FROM Employees e
                        LEFT JOIN Employees pc ON e.ManagerId = pc.Id
                        WHERE e.Id = @EmployeeId", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litEmployeeName.Text = $"{reader["FirstName"]} {reader["LastName"]}";
                                litProgramCoordinator.Text = reader["ProgramCoordinatorName"]?.ToString() ?? "Not Assigned";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading employee data: {ex.Message}");
            }
        }

        private void LoadTimesheets()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT Id, WeekStartDate, TotalHours, RegularHours, OvertimeHours, 
                               Status, SubmittedAt, ApprovedAt, Notes
                        FROM TimeSheets 
                        WHERE EmployeeId = @EmployeeId";

                    // Apply filters
                    string statusFilter = ddlStatusFilter.SelectedValue;
                    if (!string.IsNullOrEmpty(statusFilter))
                    {
                        query += " AND Status = @StatusFilter";
                    }

                    string dateFilter = ddlDateFilter.SelectedValue;
                    switch (dateFilter)
                    {
                        case "current":
                            query += " AND WeekStartDate = @CurrentWeekStart";
                            break;
                        case "last":
                            query += " AND WeekStartDate = @LastWeekStart";
                            break;
                        case "month":
                            query += " AND WeekStartDate >= @MonthStart";
                            break;
                            // "all" - no additional filter
                    }

                    query += " ORDER BY WeekStartDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        if (!string.IsNullOrEmpty(statusFilter))
                        {
                            cmd.Parameters.AddWithValue("@StatusFilter", statusFilter);
                        }

                        if (dateFilter == "current")
                        {
                            cmd.Parameters.AddWithValue("@CurrentWeekStart", GetCurrentWeekStart());
                        }
                        else if (dateFilter == "last")
                        {
                            cmd.Parameters.AddWithValue("@LastWeekStart", GetCurrentWeekStart().AddDays(-7));
                        }
                        else if (dateFilter == "month")
                        {
                            cmd.Parameters.AddWithValue("@MonthStart", new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                        }

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            rptTimesheets.DataSource = dt;
                            rptTimesheets.DataBind();
                            pnlNoTimesheets.Visible = false;
                        }
                        else
                        {
                            pnlNoTimesheets.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading timesheets: {ex.Message}");
                pnlNoTimesheets.Visible = true;
            }
        }

        private void LoadQuickStats()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Approved timesheets count
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM TimeSheets 
                        WHERE EmployeeId = @EmployeeId AND Status = 'Approved'", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        litApprovedTimesheets.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                    }

                    // Total hours this month
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT ISNULL(SUM(TotalHours), 0) 
                        FROM TimeSheets 
                        WHERE EmployeeId = @EmployeeId 
                          AND WeekStartDate >= @MonthStart
                          AND Status = 'Approved'", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        cmd.Parameters.AddWithValue("@MonthStart", new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                        decimal totalHours = Convert.ToDecimal(cmd.ExecuteScalar() ?? 0);
                        litTotalHoursThisMonth.Text = totalHours.ToString("F1");
                    }

                    // Pending timesheets count
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM TimeSheets 
                        WHERE EmployeeId = @EmployeeId AND Status = 'Submitted'", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        litPendingTimesheets.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading quick stats: {ex.Message}");
            }
        }

        #endregion

        #region Timesheet Operations

        private int CreateNewTimesheet(DateTime weekStartDate)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO TimeSheets (EmployeeId, WeekStartDate, Status)
                    OUTPUT INSERTED.Id
                    VALUES (@EmployeeId, @WeekStartDate, 'Draft')", conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                    cmd.Parameters.AddWithValue("@WeekStartDate", weekStartDate);

                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        private void SubmitTimesheet(int timesheetId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First, validate that the timesheet has time entries
                    using (SqlCommand validateCmd = new SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM TimeEntries te
                        INNER JOIN TimeSheets ts ON DATEPART(week, te.ClockIn) = DATEPART(week, ts.WeekStartDate)
                            AND YEAR(te.ClockIn) = YEAR(ts.WeekStartDate)
                        WHERE ts.Id = @TimesheetId AND te.EmployeeId = @EmployeeId", conn))
                    {
                        validateCmd.Parameters.AddWithValue("@TimesheetId", timesheetId);
                        validateCmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        int entryCount = Convert.ToInt32(validateCmd.ExecuteScalar() ?? 0);
                        if (entryCount == 0)
                        {
                            ShowMessage("Cannot submit timesheet without any time entries. Please add time entries first.", "error");
                            return;
                        }
                    }

                    // Update timesheet status to submitted
                    using (SqlCommand cmd = new SqlCommand(@"
                        UPDATE TimeSheets 
                        SET Status = 'Submitted', 
                            SubmittedAt = GETDATE(),
                            LastUpdated = GETDATE()
                        WHERE Id = @TimesheetId AND EmployeeId = @EmployeeId AND Status = 'Draft'", conn))
                    {
                        cmd.Parameters.AddWithValue("@TimesheetId", timesheetId);
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ShowMessage("Timesheet submitted successfully for approval!", "success");
                            LoadTimesheets();
                            LoadQuickStats();
                        }
                        else
                        {
                            ShowMessage("Error submitting timesheet. Please ensure it's in draft status.", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error submitting timesheet: {ex.Message}");
                ShowMessage("An error occurred while submitting the timesheet.", "error");
            }
        }

        private void DeleteTimesheet(int timesheetId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(@"
                        DELETE FROM TimeSheets 
                        WHERE Id = @TimesheetId AND EmployeeId = @EmployeeId AND Status = 'Draft'", conn))
                    {
                        cmd.Parameters.AddWithValue("@TimesheetId", timesheetId);
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ShowMessage("Timesheet deleted successfully!", "success");
                            LoadTimesheets();
                            LoadQuickStats();
                        }
                        else
                        {
                            ShowMessage("Error deleting timesheet. Only draft timesheets can be deleted.", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting timesheet: {ex.Message}");
                ShowMessage("An error occurred while deleting the timesheet.", "error");
            }
        }

        #endregion

        #region Helper Methods

        private bool TimesheetExistsForWeek(DateTime weekStartDate)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM TimeSheets 
                        WHERE EmployeeId = @EmployeeId AND WeekStartDate = @WeekStartDate", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        cmd.Parameters.AddWithValue("@WeekStartDate", weekStartDate);

                        int count = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking timesheet existence: {ex.Message}");
                return false;
            }
        }

        private DateTime GetCurrentWeekStart()
        {
            DateTime today = DateTime.Today;
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
            return today.AddDays(-daysUntilMonday);
        }

        private int GetCurrentEmployeeId()
        {
            try
            {
                if (Session["UserId"] != null)
                {
                    int userId = Convert.ToInt32(Session["UserId"]);

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SELECT Id FROM Employees WHERE UserId = @UserId", conn))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            object result = cmd.ExecuteScalar();
                            return result != null ? Convert.ToInt32(result) : 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting current employee ID: {ex.Message}");
            }

            return 0;
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            litMessage.Text = message;

            switch (type.ToLower())
            {
                case "success":
                    pnlMessage.CssClass = "alert alert-success";
                    break;
                case "error":
                    pnlMessage.CssClass = "alert alert-error";
                    break;
                case "info":
                    pnlMessage.CssClass = "alert alert-info";
                    break;
                default:
                    pnlMessage.CssClass = "alert";
                    break;
            }
        }

        protected string GetStatusClass(string status)
        {
            switch (status?.ToLower())
            {
                case "draft":
                    return "status-draft";
                case "submitted":
                    return "status-submitted";
                case "approved":
                    return "status-approved";
                case "rejected":
                    return "status-rejected";
                default:
                    return "status-unknown";
            }
        }

        protected string GetStatusIcon(string status)
        {
            switch (status?.ToLower())
            {
                case "draft":
                    return "edit";
                case "submitted":
                    return "upload";
                case "approved":
                    return "check_circle";
                case "rejected":
                    return "cancel";
                default:
                    return "info";
            }
        }

        #endregion
    }
}
cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
cmd.Parameters.AddWithValue("@WeekStartDate", weekStartDate);

int count = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking timesheet existence: {ex.Message}");
return false;
            }
        }

        private DateTime GetCurrentWeekStart()
{
    DateTime today = DateTime.Today;
    int daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
    return today.AddDays(-daysUntilMonday);
}

private int GetCurrentEmployeeId()
{
    try
    {
        if (Session["UserId"] != null)
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Id FROM Employees WHERE UserId = @UserId", conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error getting current employee ID: {ex.Message}");
    }

    return 0;
}

private void ShowMessage(string message, string type)
{
    pnlMessage.Visible = true;
    litMessage.Text = message;

    switch (type.ToLower())
    {
        case "success":
            pnlMessage.CssClass = "alert alert-success";
            break;
        case "error":
            pnlMessage.CssClass = "alert alert-error";
            break;
        case "info":
            pnlMessage.CssClass = "alert alert-info";
            break;
        default:
            pnlMessage.CssClass = "alert";
            break;
    }
}

protected string GetStatusClass(string status)
{
    switch (status?.ToLower())
    {
        case "draft":
            return "status-draft";
        case "submitted":
            return "status-submitted";
        case "approved":
            return "status-approved";
        case "rejected":
            return "status-rejected";
        default:
            return "status-unknown";
    }
}

        #endregion
    }
}