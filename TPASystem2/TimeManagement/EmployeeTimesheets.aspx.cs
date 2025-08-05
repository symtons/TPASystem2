using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
            CreateNewTimesheetHandler();
        }

        protected void btnCreateFirstTimesheet_Click(object sender, EventArgs e)
        {
            CreateNewTimesheetHandler();
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
                               COALESCE(pc.FirstName + ' ' + pc.LastName, 'Not Assigned') AS ProgramCoordinator
                        FROM Employees e
                        LEFT JOIN Employees pc ON e.ProgramCoordinatorId = pc.Id
                        WHERE e.Id = @EmployeeId AND e.IsActive = 1", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litEmployeeName.Text = $"{reader["FirstName"]} {reader["LastName"]}";
                                litProgramCoordinator.Text = reader["ProgramCoordinator"]?.ToString() ?? "Not Assigned";
                            }
                            else
                            {
                                litEmployeeName.Text = "Employee Not Found";
                                litProgramCoordinator.Text = "Not Assigned";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading employee data: {ex.Message}");
                litEmployeeName.Text = "Error Loading Data";
                litProgramCoordinator.Text = "Not Assigned";
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
                        SELECT Id, WeekStartDate, WeekEndDate, TotalHours, RegularHours, OvertimeHours, 
                               Status, SubmittedAt, ApprovedAt, Notes, CreatedAt
                        FROM TimeSheets 
                        WHERE EmployeeId = @EmployeeId";

                    // Apply filters
                    string statusFilter = ddlStatusFilter.SelectedValue;
                    if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "all")
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

                        if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "all")
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
                            rptTimesheets.Visible = true;
                            pnlNoTimesheets.Visible = false;
                        }
                        else
                        {
                            rptTimesheets.Visible = false;
                            pnlNoTimesheets.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading timesheets: {ex.Message}");
                rptTimesheets.Visible = false;
                pnlNoTimesheets.Visible = true;
                ShowMessage("Error loading timesheets. Please try again.", "error");
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
                litApprovedTimesheets.Text = "0";
                litTotalHoursThisMonth.Text = "0.0";
                litPendingTimesheets.Text = "0";
            }
        }

        #endregion

        #region Validation and Access

        private void ValidateUserAccess()
        {
            try
            {
                string userRole = Session["UserRole"]?.ToString() ?? "";
                if (userRole!= "EMPLOYEE")
                {
                    Response.Redirect("~/Dashboard.aspx");
                    return;
                }

                if (CurrentEmployeeId <= 0)
                {
                    ShowMessage("Employee record not found. Please contact HR.", "error");
                    Response.Redirect("~/Dashboard.aspx");
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating user access: {ex.Message}");
                //Response.Redirect("~/Login.aspx");
            }
        }

        #endregion

        #region Timesheet Operations

        private void CreateNewTimesheetHandler()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"CreateNewTimesheetHandler: Starting for employee {CurrentEmployeeId}");

                // Check if there's already a timesheet for the current week
                DateTime currentWeekStart = GetCurrentWeekStart();
                System.Diagnostics.Debug.WriteLine($"CreateNewTimesheetHandler: Week start date is {currentWeekStart:yyyy-MM-dd}");

                if (TimesheetExistsForWeek(currentWeekStart))
                {
                    System.Diagnostics.Debug.WriteLine("CreateNewTimesheetHandler: Timesheet already exists for this week");
                    ShowMessage("A timesheet already exists for the current week. Please edit the existing timesheet instead.", "error");
                    return;
                }

                // Create new timesheet
                int timesheetId = CreateNewTimesheet(currentWeekStart);
                System.Diagnostics.Debug.WriteLine($"CreateNewTimesheetHandler: CreateNewTimesheet returned ID {timesheetId}");

                if (timesheetId > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"CreateNewTimesheetHandler: Redirecting to EditTimesheet.aspx?id={timesheetId}");
                    Response.Redirect($"~/TimeManagement/EditTimesheet.aspx?id={timesheetId}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("CreateNewTimesheetHandler: CreateNewTimesheet returned 0 or null");
                    ShowMessage("Error creating timesheet. Please try again.", "error");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CreateNewTimesheetHandler: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ShowMessage("An error occurred while creating the timesheet.", "error");
            }
        }

        private int CreateNewTimesheet(DateTime weekStartDate)
        {
            try
            {
                // Debug: Check if we have a valid employee ID
                System.Diagnostics.Debug.WriteLine($"CreateNewTimesheet: CurrentEmployeeId = {CurrentEmployeeId}");
                System.Diagnostics.Debug.WriteLine($"CreateNewTimesheet: Session UserId = {Session["UserId"]}");
                System.Diagnostics.Debug.WriteLine($"CreateNewTimesheet: Session UserRole = {Session["UserRole"]}");

                if (CurrentEmployeeId <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("CreateNewTimesheet: Invalid EmployeeId - checking session and database");

                    // Additional debugging - check what's in the session
                    if (Session["UserId"] == null)
                    {
                        System.Diagnostics.Debug.WriteLine("CreateNewTimesheet: Session UserId is null");
                        return 0;
                    }

                    // Try to get employee info directly
                    int userId = Convert.ToInt32(Session["UserId"]);
                    using (SqlConnection debugConn = new SqlConnection(connectionString))
                    {
                        debugConn.Open();
                        using (SqlCommand debugCmd = new SqlCommand(@"
                            SELECT e.Id, e.FirstName, e.LastName, e.IsActive, u.Email 
                            FROM Employees e 
                            INNER JOIN Users u ON e.UserId = u.Id 
                            WHERE e.UserId = @UserId", debugConn))
                        {
                            debugCmd.Parameters.AddWithValue("@UserId", userId);
                            using (SqlDataReader reader = debugCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    System.Diagnostics.Debug.WriteLine($"Found Employee: ID={reader["Id"]}, Name={reader["FirstName"]} {reader["LastName"]}, IsActive={reader["IsActive"]}, Email={reader["Email"]}");
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"No Employee found for UserId {userId}");
                                }
                            }
                        }
                    }
                    return 0;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Debug: Verify employee exists and is active
                    using (SqlCommand checkCmd = new SqlCommand(@"
                        SELECT e.Id, e.FirstName, e.LastName, e.IsActive 
                        FROM Employees e 
                        WHERE e.Id = @EmployeeId", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        using (SqlDataReader reader = checkCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                System.Diagnostics.Debug.WriteLine($"Employee verification: ID={reader["Id"]}, Name={reader["FirstName"]} {reader["LastName"]}, IsActive={reader["IsActive"]}");
                                if (!Convert.ToBoolean(reader["IsActive"]))
                                {
                                    System.Diagnostics.Debug.WriteLine($"Employee {CurrentEmployeeId} is not active");
                                    return 0;
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Employee {CurrentEmployeeId} not found in database");
                                return 0;
                            }
                        }
                    }

                    // Calculate week end date (Sunday)
                    DateTime weekEndDate = weekStartDate.AddDays(6);

                    System.Diagnostics.Debug.WriteLine($"Creating timesheet: EmployeeId={CurrentEmployeeId}, WeekStart={weekStartDate:yyyy-MM-dd}, WeekEnd={weekEndDate:yyyy-MM-dd}");

                    // Since the table has defaults, we'll use a minimal INSERT and let defaults handle the rest
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO TimeSheets (EmployeeId, WeekStartDate, WeekEndDate)
                        OUTPUT INSERTED.Id
                        VALUES (@EmployeeId, @WeekStartDate, @WeekEndDate)", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        cmd.Parameters.AddWithValue("@WeekStartDate", weekStartDate);
                        cmd.Parameters.AddWithValue("@WeekEndDate", weekEndDate);

                        object result = cmd.ExecuteScalar();
                        int timesheetId = result != null ? Convert.ToInt32(result) : 0;

                        System.Diagnostics.Debug.WriteLine($"CreateNewTimesheet: Created timesheet ID {timesheetId} for employee {CurrentEmployeeId}");
                        return timesheetId;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating new timesheet: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                return 0;
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
                            UpdatedAt = GETDATE()
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
            // Calculate days to subtract to get to Monday (start of week)
            int daysFromMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            return today.AddDays(-daysFromMonday);
        }

        private int GetCurrentEmployeeId()
        {
              System.Diagnostics.Debug.WriteLine($"GetCurrentEmployeeId: Starting...");

                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("GetCurrentEmployeeId: Session UserId is null");
                    return 0;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                System.Diagnostics.Debug.WriteLine($"GetCurrentEmployeeId: UserId from session = {userId}");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First check if user exists
                    using (SqlCommand userCmd = new SqlCommand("SELECT Email, Role FROM Users WHERE Id = @UserId", conn))
                    {
                        userCmd.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataReader userReader = userCmd.ExecuteReader())
                        {
                            if (userReader.Read())
                            {
                                System.Diagnostics.Debug.WriteLine($"GetCurrentEmployeeId: Found user - Email: {userReader["Email"]}, Role: {userReader["Role"]}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"GetCurrentEmployeeId: User {userId} not found in Users table");
                                return 0;
                            }
                        }
                    }

                    // Now check for employee record
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT e.Id, e.FirstName, e.LastName, e.IsActive 
                        FROM Employees e 
                        WHERE e.UserId = @UserId", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int employeeId = Convert.ToInt32(reader["Id"]);
                                bool isActive = Convert.ToBoolean(reader["IsActive"]);
                                string name = $"{reader["FirstName"]} {reader["LastName"]}";

                                System.Diagnostics.Debug.WriteLine($"GetCurrentEmployeeId: Found employee - ID: {employeeId}, Name: {name}, IsActive: {isActive}");

                                if (isActive)
                                {
                                    return employeeId;
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine($"GetCurrentEmployeeId: Employee {employeeId} is not active");
                                    return 0;
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"GetCurrentEmployeeId: No employee record found for UserId {userId}");
                                return 0;
                            }
                        }
                    }
                }
            
        }

        private void ShowMessage(string message, string type)
        {
            if (pnlMessage != null && litMessage != null)
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
        }

        public string GetStatusClass(string status)
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

        public string GetStatusIcon(string status)
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

        public string FormatDate(object dateValue)
        {
            if (dateValue != null && dateValue != DBNull.Value)
            {
                DateTime date = Convert.ToDateTime(dateValue);
                return date.ToString("MMM dd, yyyy");
            }
            return "";
        }

        public string FormatHours(object hoursValue)
        {
            if (hoursValue != null && hoursValue != DBNull.Value)
            {
                decimal hours = Convert.ToDecimal(hoursValue);
                return hours.ToString("F1");
            }
            return "0.0";
        }

        #endregion
    }
}