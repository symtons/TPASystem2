using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.TimeManagement
{
    public partial class TimeManagement : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
                LoadDepartments();
                LoadTimeManagementData();
                SetActiveTab("Current");
            }
        }

        private void InitializePage()
        {
            // Set default date range to current week
            DateTime today = DateTime.Today;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1); // Monday
            DateTime endOfWeek = startOfWeek.AddDays(6); // Sunday

            txtStartDate.Text = startOfWeek.ToString("yyyy-MM-dd");
            txtEndDate.Text = endOfWeek.ToString("yyyy-MM-dd");

            // Set last updated timestamp
            lblLastUpdated.Text = DateTime.Now.ToString("h:mm:ss tt");
        }

        #endregion

        #region Data Loading Methods

        private void LoadTimeManagementData()
        {
            LoadStatusOverview();
            LoadTimeStats();
            LoadCurrentStatusData();
            LoadTimesheetData();
            LoadAttendanceData();
            LoadScheduleData();
        }

        private void LoadDepartments()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT Id, Name FROM Departments WHERE IsActive = 1 ORDER BY Name";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        ddlDepartment.DataSource = dt;
                        ddlDepartment.DataTextField = "Name";
                        ddlDepartment.DataValueField = "Id";
                        ddlDepartment.DataBind();

                        ddlDepartment.Items.Insert(0, new ListItem("All Departments", ""));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading departments: {ex.Message}", "error");
            }
        }

        private void LoadStatusOverview()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get currently clocked in employees
                    string clockedInQuery = @"
                        SELECT COUNT(*) 
                        FROM TimeEntries te
                        INNER JOIN Employees e ON te.EmployeeId = e.Id
                        WHERE CAST(te.ClockIn AS DATE) = CAST(GETDATE() AS DATE)
                        AND te.ClockOut IS NULL 
                        AND te.Status = 'Active'
                        AND e.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(clockedInQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        lblClockedIn.Text = result?.ToString() ?? "0";
                    }

                    // Get total active employees for clocked out calculation
                    string totalActiveQuery = @"
                        SELECT COUNT(*) 
                        FROM Employees 
                        WHERE IsActive = 1 AND EmployeeType = 'Full-time'";

                    using (SqlCommand cmd = new SqlCommand(totalActiveQuery, conn))
                    {
                        int totalActive = (int)(cmd.ExecuteScalar() ?? 0);
                        int clockedIn = int.Parse(lblClockedIn.Text);
                        lblClockedOut.Text = (totalActive - clockedIn).ToString();
                    }

                    // For now, set break and late arrivals to 0 (can be enhanced later)
                    lblOnBreak.Text = "0";

                    // Get late arrivals today
                    string lateArrivalsQuery = @"
                        SELECT COUNT(DISTINCT te.EmployeeId)
                        FROM TimeEntries te
                        INNER JOIN Employees e ON te.EmployeeId = e.Id
                        INNER JOIN Schedules s ON e.Id = s.EmployeeId 
                        WHERE CAST(te.ClockIn AS DATE) = CAST(GETDATE() AS DATE)
                        AND s.DayOfWeek = DATEPART(WEEKDAY, GETDATE()) - 1
                        AND s.IsActive = 1
                        AND CAST(te.ClockIn AS TIME) > s.StartTime
                        AND e.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(lateArrivalsQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        lblLateArrivals.Text = result?.ToString() ?? "0";
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading status overview: {ex.Message}", "error");
            }
        }

        private void LoadTimeStats()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Attendance rate this month
                    string attendanceQuery = @"
                        SELECT 
                            CASE 
                                WHEN COUNT(DISTINCT e.Id) = 0 THEN 0
                                ELSE CAST(COUNT(DISTINCT te.EmployeeId) AS FLOAT) / COUNT(DISTINCT e.Id) * 100
                            END as AttendanceRate
                        FROM Employees e
                        LEFT JOIN TimeEntries te ON e.Id = te.EmployeeId 
                            AND MONTH(te.ClockIn) = MONTH(GETDATE()) 
                            AND YEAR(te.ClockIn) = YEAR(GETDATE())
                        WHERE e.IsActive = 1 AND e.EmployeeType = 'Full-time'";

                    using (SqlCommand cmd = new SqlCommand(attendanceQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        double rate = Convert.ToDouble(result ?? 0);
                        lblAttendanceRate.Text = rate.ToString("F0") + "%";
                    }

                    // Overtime hours this week
                    string overtimeQuery = @"
                        SELECT ISNULL(SUM(ts.OvertimeHours), 0)
                        FROM TimeSheets ts
                        INNER JOIN Employees e ON ts.EmployeeId = e.Id
                        WHERE ts.WeekStartDate >= DATEADD(wk, DATEDIFF(wk, 0, GETDATE()), 0)
                        AND ts.WeekStartDate < DATEADD(wk, DATEDIFF(wk, 0, GETDATE()) + 1, 0)
                        AND e.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(overtimeQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        decimal overtime = Convert.ToDecimal(result ?? 0);
                        lblOvertimeHours.Text = overtime.ToString("F1");
                    }

                    // Pending timesheets
                    string pendingQuery = @"
                        SELECT COUNT(*)
                        FROM TimeSheets ts
                        INNER JOIN Employees e ON ts.EmployeeId = e.Id
                        WHERE ts.Status = 'Submitted'
                        AND e.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(pendingQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        lblPendingTimesheets.Text = result?.ToString() ?? "0";
                    }

                    // Total hours this week
                    string totalHoursQuery = @"
                        SELECT ISNULL(SUM(ts.TotalHours), 0)
                        FROM TimeSheets ts
                        INNER JOIN Employees e ON ts.EmployeeId = e.Id
                        WHERE ts.WeekStartDate >= DATEADD(wk, DATEDIFF(wk, 0, GETDATE()), 0)
                        AND ts.WeekStartDate < DATEADD(wk, DATEDIFF(wk, 0, GETDATE()) + 1, 0)
                        AND e.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(totalHoursQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        decimal totalHours = Convert.ToDecimal(result ?? 0);
                        lblTotalHours.Text = totalHours.ToString("F0");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading time statistics: {ex.Message}", "error");
            }
        }

        private void LoadCurrentStatusData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            e.Id as EmployeeId,
                            e.FirstName + ' ' + e.LastName AS EmployeeName,
                            e.EmployeeNumber,
                            ISNULL(d.Name, 'No Department') AS Department,
                            CASE 
                                WHEN te.ClockOut IS NULL AND CAST(te.ClockIn AS DATE) = CAST(GETDATE() AS DATE) THEN 'clocked-in'
                                ELSE 'clocked-out'
                            END AS Status,
                            te.ClockIn AS ClockInTime,
                            CASE 
                                WHEN te.ClockOut IS NULL AND CAST(te.ClockIn AS DATE) = CAST(GETDATE() AS DATE) 
                                THEN DATEDIFF(MINUTE, te.ClockIn, GETDATE()) / 60.0
                                ELSE 0
                            END AS HoursWorked,
                            ISNULL(te.Location, 'Not Specified') AS Location,
                            CASE 
                                WHEN s.StartTime IS NOT NULL AND s.EndTime IS NOT NULL 
                                THEN DATEDIFF(MINUTE, s.StartTime, s.EndTime) / 60.0
                                ELSE 8.0
                            END AS ScheduledHours
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        LEFT JOIN TimeEntries te ON e.Id = te.EmployeeId 
                            AND CAST(te.ClockIn AS DATE) = CAST(GETDATE() AS DATE)
                            AND te.Id = (
                                SELECT TOP 1 Id 
                                FROM TimeEntries 
                                WHERE EmployeeId = e.Id 
                                AND CAST(ClockIn AS DATE) = CAST(GETDATE() AS DATE)
                                ORDER BY ClockIn DESC
                            )
                        LEFT JOIN Schedules s ON e.Id = s.EmployeeId 
                            AND s.DayOfWeek = DATEPART(WEEKDAY, GETDATE()) - 1
                            AND s.IsActive = 1
                        WHERE e.IsActive = 1 AND e.EmployeeType = 'Full-time'";

                    // Apply department filter
                    if (!string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                    {
                        query += " AND e.DepartmentId = @DepartmentId";
                    }

                    query += " ORDER BY e.FirstName, e.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                            cmd.Parameters.AddWithValue("@DepartmentId", ddlDepartment.SelectedValue);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvCurrentStatus.DataSource = dt;
                        gvCurrentStatus.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading current status data: {ex.Message}", "error");
            }
        }

        private void LoadTimesheetData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            ts.Id,
                            e.FirstName + ' ' + e.LastName AS EmployeeName,
                            e.EmployeeNumber,
                            CONVERT(VARCHAR, ts.WeekStartDate, 101) + ' - ' + CONVERT(VARCHAR, ts.WeekEndDate, 101) AS WeekPeriod,
                            ts.TotalHours,
                            ts.RegularHours,
                            ts.OvertimeHours,
                            ts.SubmittedAt,
                            ts.Status
                        FROM TimeSheets ts
                        INNER JOIN Employees e ON ts.EmployeeId = e.Id
                        WHERE e.IsActive = 1";

                    // Apply filters
                    if (!string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                    {
                        query += " AND e.DepartmentId = @DepartmentId";
                    }

                    // Apply date range filter
                    DateTime startDate = DateTime.Parse(txtStartDate.Text);
                    DateTime endDate = DateTime.Parse(txtEndDate.Text);
                    query += " AND ts.WeekStartDate >= @StartDate AND ts.WeekEndDate <= @EndDate";

                    query += " ORDER BY ts.SubmittedAt DESC, e.FirstName, e.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                            cmd.Parameters.AddWithValue("@DepartmentId", ddlDepartment.SelectedValue);

                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvTimesheets.DataSource = dt;
                        gvTimesheets.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading timesheet data: {ex.Message}", "error");
            }
        }

        private void LoadAttendanceData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    DateTime startDate = DateTime.Parse(txtStartDate.Text);
                    DateTime endDate = DateTime.Parse(txtEndDate.Text);

                    string query = @"
                        SELECT 
                            e.FirstName + ' ' + e.LastName AS EmployeeName,
                            e.EmployeeNumber,
                            ISNULL(d.Name, 'No Department') AS Department,
                            COUNT(DISTINCT CAST(te.ClockIn AS DATE)) AS DaysPresent,
                            DATEDIFF(DAY, @StartDate, @EndDate) + 1 - COUNT(DISTINCT CAST(te.ClockIn AS DATE)) AS DaysAbsent,
                            CASE 
                                WHEN DATEDIFF(DAY, @StartDate, @EndDate) + 1 = 0 THEN 0
                                ELSE CAST(COUNT(DISTINCT CAST(te.ClockIn AS DATE)) AS FLOAT) / (DATEDIFF(DAY, @StartDate, @EndDate) + 1)
                            END AS AttendanceRate,
                            SUM(CASE 
                                WHEN s.StartTime IS NOT NULL AND CAST(te.ClockIn AS TIME) > s.StartTime THEN 1 
                                ELSE 0 
                            END) AS LateArrivals,
                            SUM(CASE 
                                WHEN s.EndTime IS NOT NULL AND te.ClockOut IS NOT NULL AND CAST(te.ClockOut AS TIME) < s.EndTime THEN 1 
                                ELSE 0 
                            END) AS EarlyDepartures,
                            ISNULL(SUM(te.TotalHours), 0) AS TotalHours
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        LEFT JOIN TimeEntries te ON e.Id = te.EmployeeId 
                            AND CAST(te.ClockIn AS DATE) BETWEEN @StartDate AND @EndDate
                        LEFT JOIN Schedules s ON e.Id = s.EmployeeId 
                            AND s.DayOfWeek = DATEPART(WEEKDAY, te.ClockIn) - 1
                            AND s.IsActive = 1
                        WHERE e.IsActive = 1 AND e.EmployeeType = 'Full-time'";

                    if (!string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                    {
                        query += " AND e.DepartmentId = @DepartmentId";
                    }

                    query += @"
                        GROUP BY e.Id, e.FirstName, e.LastName, e.EmployeeNumber, d.Name
                        ORDER BY e.FirstName, e.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);

                        if (!string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                            cmd.Parameters.AddWithValue("@DepartmentId", ddlDepartment.SelectedValue);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvAttendance.DataSource = dt;
                        gvAttendance.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading attendance data: {ex.Message}", "error");
            }
        }

        private void LoadScheduleData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            e.FirstName + ' ' + e.LastName AS EmployeeName,
                            e.EmployeeNumber,
                            ISNULL(d.Name, 'No Department') AS Department,
                            MAX(CASE WHEN s.DayOfWeek = 1 THEN FORMAT(s.StartTime, 'h:mm tt') + ' - ' + FORMAT(s.EndTime, 'h:mm tt') ELSE 'Off' END) AS Monday,
                            MAX(CASE WHEN s.DayOfWeek = 2 THEN FORMAT(s.StartTime, 'h:mm tt') + ' - ' + FORMAT(s.EndTime, 'h:mm tt') ELSE 'Off' END) AS Tuesday,
                            MAX(CASE WHEN s.DayOfWeek = 3 THEN FORMAT(s.StartTime, 'h:mm tt') + ' - ' + FORMAT(s.EndTime, 'h:mm tt') ELSE 'Off' END) AS Wednesday,
                            MAX(CASE WHEN s.DayOfWeek = 4 THEN FORMAT(s.StartTime, 'h:mm tt') + ' - ' + FORMAT(s.EndTime, 'h:mm tt') ELSE 'Off' END) AS Thursday,
                            MAX(CASE WHEN s.DayOfWeek = 5 THEN FORMAT(s.StartTime, 'h:mm tt') + ' - ' + FORMAT(s.EndTime, 'h:mm tt') ELSE 'Off' END) AS Friday,
                            SUM(CASE 
                                WHEN s.DayOfWeek BETWEEN 1 AND 5 THEN DATEDIFF(MINUTE, s.StartTime, s.EndTime) / 60.0 
                                ELSE 0 
                            END) AS WeeklyHours
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        LEFT JOIN Schedules s ON e.Id = s.EmployeeId AND s.IsActive = 1
                        WHERE e.IsActive = 1 AND e.EmployeeType = 'Full-time'";

                    if (!string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                    {
                        query += " AND e.DepartmentId = @DepartmentId";
                    }

                    query += @"
                        GROUP BY e.Id, e.FirstName, e.LastName, e.EmployeeNumber, d.Name
                        ORDER BY e.FirstName, e.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                            cmd.Parameters.AddWithValue("@DepartmentId", ddlDepartment.SelectedValue);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvSchedules.DataSource = dt;
                        gvSchedules.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading schedule data: {ex.Message}", "error");
            }
        }

        #endregion

        #region Filter Events

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTimeManagementData();
        }

        protected void ddlTimeFrame_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDateRangeFromSelection();
            LoadTimeManagementData();
        }

        protected void btnApplyFilter_Click(object sender, EventArgs e)
        {
            LoadTimeManagementData();
        }

        protected void btnClearFilter_Click(object sender, EventArgs e)
        {
            ddlDepartment.SelectedIndex = 0;
            ddlTimeFrame.SelectedValue = "week";
            SetDateRangeFromSelection();
            LoadTimeManagementData();
        }

        private void SetDateRangeFromSelection()
        {
            DateTime today = DateTime.Today;
            DateTime startDate, endDate;

            switch (ddlTimeFrame.SelectedValue)
            {
                case "today":
                    startDate = today;
                    endDate = today;
                    break;
                case "week":
                    startDate = today.AddDays(-(int)today.DayOfWeek + 1); // Monday
                    endDate = startDate.AddDays(6); // Sunday
                    break;
                case "month":
                    startDate = new DateTime(today.Year, today.Month, 1);
                    endDate = startDate.AddMonths(1).AddDays(-1);
                    break;
                default: // custom
                    return; // Don't change dates for custom selection
            }

            txtStartDate.Text = startDate.ToString("yyyy-MM-dd");
            txtEndDate.Text = endDate.ToString("yyyy-MM-dd");
        }

        #endregion

        #region Button Events

        protected void btnViewReports_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/TimeReports.aspx");
        }

        protected void btnExportData_Click(object sender, EventArgs e)
        {
            try
            {
                ExportCurrentTabData();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting data: {ex.Message}", "error");
            }
        }

        protected void btnRefreshStatus_Click(object sender, EventArgs e)
        {
            LoadStatusOverview();
            LoadCurrentStatusData();
            lblLastUpdated.Text = DateTime.Now.ToString("h:mm:ss tt");
            ShowMessage("Status refreshed successfully!", "success");
        }

        protected void btnBulkApprove_Click(object sender, EventArgs e)
        {
            try
            {
                BulkApproveTimesheets();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error in bulk approval: {ex.Message}", "error");
            }
        }

        protected void btnManageSchedules_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/ScheduleManagement.aspx");
        }

        #endregion

        #region Tab Navigation

        protected void btnTabCurrent_Click(object sender, EventArgs e)
        {
            SetActiveTab("Current");
        }

        protected void btnTabTimesheets_Click(object sender, EventArgs e)
        {
            SetActiveTab("Timesheets");
        }

        protected void btnTabAttendance_Click(object sender, EventArgs e)
        {
            SetActiveTab("Attendance");
        }

        protected void btnTabSchedules_Click(object sender, EventArgs e)
        {
            SetActiveTab("Schedules");
        }

        private void SetActiveTab(string tabName)
        {
            // Reset all tab buttons
            btnTabCurrent.CssClass = "tab-button";
            btnTabTimesheets.CssClass = "tab-button";
            btnTabAttendance.CssClass = "tab-button";
            btnTabSchedules.CssClass = "tab-button";

            // Hide all tab content
            pnlCurrentTab.CssClass = "tab-content";
            pnlTimesheetsTab.CssClass = "tab-content";
            pnlAttendanceTab.CssClass = "tab-content";
            pnlSchedulesTab.CssClass = "tab-content";

            // Show selected tab
            switch (tabName)
            {
                case "Current":
                    btnTabCurrent.CssClass = "tab-button active";
                    pnlCurrentTab.CssClass = "tab-content active";
                    break;
                case "Timesheets":
                    btnTabTimesheets.CssClass = "tab-button active";
                    pnlTimesheetsTab.CssClass = "tab-content active";
                    break;
                case "Attendance":
                    btnTabAttendance.CssClass = "tab-button active";
                    pnlAttendanceTab.CssClass = "tab-content active";
                    break;
                case "Schedules":
                    btnTabSchedules.CssClass = "tab-button active";
                    pnlSchedulesTab.CssClass = "tab-content active";
                    break;
            }
        }

        #endregion

        #region GridView Events

        protected void gvTimesheets_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int timesheetId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ViewTimesheet":
                    Response.Redirect($"~/HR/TimesheetDetails.aspx?id={timesheetId}");
                    break;
                case "ApproveTimesheet":
                    ApproveTimesheet(timesheetId);
                    break;
                case "RejectTimesheet":
                    RejectTimesheet(timesheetId);
                    break;
            }
        }

        #endregion

        #region Helper Methods

        private void ApproveTimesheet(int timesheetId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string updateQuery = @"
                        UPDATE TimeSheets 
                        SET Status = 'Approved', 
                            ApprovedById = @ApprovedById,
                            ApprovedAt = GETUTCDATE(),
                            UpdatedAt = GETUTCDATE()
                        WHERE Id = @TimesheetId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@TimesheetId", timesheetId);
                        cmd.Parameters.AddWithValue("@ApprovedById", GetCurrentUserId());

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    ShowMessage("Timesheet approved successfully!", "success");
                    LoadTimesheetData();
                    LoadTimeStats(); // Refresh stats
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error approving timesheet: {ex.Message}", "error");
            }
        }

        private void RejectTimesheet(int timesheetId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string updateQuery = @"
                        UPDATE TimeSheets 
                        SET Status = 'Rejected', 
                            ApprovedById = @ApprovedById,
                            ApprovedAt = GETUTCDATE(),
                            UpdatedAt = GETUTCDATE(),
                            Notes = ISNULL(Notes, '') + CHAR(13) + CHAR(10) + 'Rejected on ' + CONVERT(VARCHAR, GETDATE(), 101)
                        WHERE Id = @TimesheetId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@TimesheetId", timesheetId);
                        cmd.Parameters.AddWithValue("@ApprovedById", GetCurrentUserId());

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    ShowMessage("Timesheet rejected successfully!", "success");
                    LoadTimesheetData();
                    LoadTimeStats(); // Refresh stats
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error rejecting timesheet: {ex.Message}", "error");
            }
        }

        private void BulkApproveTimesheets()
        {
            int approvedCount = 0;

            foreach (GridViewRow row in gvTimesheets.Rows)
            {
                CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                if (chkSelect != null && chkSelect.Checked)
                {
                    // Get the timesheet ID from the GridView
                    int timesheetId = Convert.ToInt32(gvTimesheets.DataKeys[row.RowIndex].Value);

                    try
                    {
                        ApproveTimesheet(timesheetId);
                        approvedCount++;
                    }
                    catch (Exception ex)
                    {
                        ShowMessage($"Error approving timesheet ID {timesheetId}: {ex.Message}", "error");
                    }
                }
            }

            if (approvedCount > 0)
            {
                ShowMessage($"{approvedCount} timesheet(s) approved successfully!", "success");
                LoadTimesheetData();
                LoadTimeStats();
            }
            else
            {
                ShowMessage("No timesheets selected for approval.", "warning");
            }
        }

        private void ExportCurrentTabData()
        {
            string activeTab = GetActiveTabName();
            DataTable dataToExport = null;
            string fileName = "";

            switch (activeTab)
            {
                case "Current":
                    dataToExport = GetCurrentStatusDataTable();
                    fileName = "CurrentStatus";
                    break;
                case "Timesheets":
                    dataToExport = GetTimesheetDataTable();
                    fileName = "Timesheets";
                    break;
                case "Attendance":
                    dataToExport = GetAttendanceDataTable();
                    fileName = "Attendance";
                    break;
                case "Schedules":
                    dataToExport = GetScheduleDataTable();
                    fileName = "Schedules";
                    break;
            }

            if (dataToExport != null)
            {
                ExportToCSV(dataToExport, fileName);
            }
        }

        private string GetActiveTabName()
        {
            if (btnTabCurrent.CssClass.Contains("active")) return "Current";
            if (btnTabTimesheets.CssClass.Contains("active")) return "Timesheets";
            if (btnTabAttendance.CssClass.Contains("active")) return "Attendance";
            if (btnTabSchedules.CssClass.Contains("active")) return "Schedules";
            return "Current"; // Default
        }

        private DataTable GetCurrentStatusDataTable()
        {
            // Return the same data used for current status grid
            return ((DataTable)gvCurrentStatus.DataSource);
        }

        private DataTable GetTimesheetDataTable()
        {
            // Return the same data used for timesheet grid
            return ((DataTable)gvTimesheets.DataSource);
        }

        private DataTable GetAttendanceDataTable()
        {
            // Return the same data used for attendance grid
            return ((DataTable)gvAttendance.DataSource);
        }

        private DataTable GetScheduleDataTable()
        {
            // Return the same data used for schedule grid
            return ((DataTable)gvSchedules.DataSource);
        }

        private void ExportToCSV(DataTable dt, string fileName)
        {
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename={fileName}_{DateTime.Now:yyyyMMdd}.csv");
                Response.Charset = "";
                Response.ContentType = "application/text";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                // Add headers
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb.Append(dt.Columns[i].ColumnName);
                    if (i < dt.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();

                // Add data rows
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string value = dt.Rows[i][j]?.ToString() ?? "";
                        // Escape commas and quotes in CSV
                        if (value.Contains(",") || value.Contains("\""))
                        {
                            value = "\"" + value.Replace("\"", "\"\"") + "\"";
                        }
                        sb.Append(value);
                        if (j < dt.Columns.Count - 1)
                            sb.Append(",");
                    }
                    sb.AppendLine();
                }

                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting to CSV: {ex.Message}", "error");
            }
        }

        private int GetCurrentUserId()
        {
            // Get current user ID from session or implement your authentication logic
            //if (Session["UserId"] != null)
            //{
            //    return Convert.ToInt32(Session["UserId"]);
            //}
            return 1; // Default fallback - should be replaced with actual user management
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = message;

            // Clear existing CSS classes
            pnlMessage.CssClass = "alert";

            // Add appropriate CSS class based on type
            switch (type.ToLower())
            {
                case "success":
                    pnlMessage.CssClass += " alert-success";
                    break;
                case "error":
                    pnlMessage.CssClass += " alert-error";
                    break;
                case "warning":
                    pnlMessage.CssClass += " alert-warning";
                    break;
                case "info":
                    pnlMessage.CssClass += " alert-info";
                    break;
                default:
                    pnlMessage.CssClass += " alert-info";
                    break;
            }

            // Auto-hide success messages after 5 seconds
            if (type.ToLower() == "success")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "hideMessage",
                    "setTimeout(function() { var alert = document.querySelector('.alert'); if(alert) alert.style.display = 'none'; }, 5000);", true);
            }
        }

        #endregion

        #region Public Methods for ASPX

        public string GetStatusClass(string status)
        {
            switch (status?.ToLower())
            {
                case "clocked-in":
                    return "status-clocked-in";
                case "on-break":
                    return "status-on-break";
                case "late":
                    return "status-late";
                case "clocked-out":
                default:
                    return "status-clocked-out";
            }
        }

        public string GetStatusText(string status)
        {
            switch (status?.ToLower())
            {
                case "clocked-in":
                    return "Clocked In";
                case "on-break":
                    return "On Break";
                case "late":
                    return "Late";
                case "clocked-out":
                default:
                    return "Clocked Out";
            }
        }

        public string GetTimesheetStatusClass(string status)
        {
            switch (status?.ToLower())
            {
                case "draft":
                    return "timesheet-status-draft";
                case "submitted":
                    return "timesheet-status-submitted";
                case "approved":
                    return "timesheet-status-approved";
                case "rejected":
                    return "timesheet-status-rejected";
                default:
                    return "timesheet-status-draft";
            }
        }

        #endregion
    }
}