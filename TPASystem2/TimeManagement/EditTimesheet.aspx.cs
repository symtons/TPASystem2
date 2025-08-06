using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.TimeManagement
{
    public partial class EditTimesheet : System.Web.UI.Page
    {
        #region Properties and Fields

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        /// <summary>
        /// Gets the current timesheet ID from query string or ViewState
        /// </summary>
        protected int TimesheetId
        {
            get
            {
                if (ViewState["TimesheetId"] != null)
                    return Convert.ToInt32(ViewState["TimesheetId"]);

                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int id))
                {
                    ViewState["TimesheetId"] = id;
                    return id;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the current employee ID from session
        /// </summary>
        protected int CurrentEmployeeId
        {
            get
            {
                if (Session["EmployeeId"] != null)
                    return Convert.ToInt32(Session["EmployeeId"]);

                // Alternative: Get from User.Identity or other authentication mechanism
                // if (User.Identity.IsAuthenticated)
                // {
                //     // Extract employee ID from user claims or database lookup
                // }

                return 0;
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidateAccess();
                LoadTimesheetData();
            }
        }

        #endregion

        #region Validation and Security

        /// <summary>
        /// Validates user access to edit this timesheet
        /// </summary>
        private void ValidateAccess()
        {
            try
            {
                if (CurrentEmployeeId == 0)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                if (TimesheetId == 0)
                {
                    ShowMessage("Invalid timesheet ID.", "error");
                    Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
                    return;
                }

                // Check if user has permission to edit this timesheet
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT Status, EmployeeId 
                        FROM TimeSheets 
                        WHERE Id = @TimesheetId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int timesheetEmployeeId = Convert.ToInt32(reader["EmployeeId"]);
                                string status = reader["Status"].ToString();

                                // Check ownership
                                if (timesheetEmployeeId != CurrentEmployeeId)
                                {
                                    ShowMessage("You do not have permission to edit this timesheet.", "error");
                                    Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
                                    return;
                                }

                                // Check if timesheet is still editable
                                if (status == "APPROVED" || status == "LOCKED")
                                {
                                    ShowMessage("This timesheet has been approved and cannot be edited.", "warning");
                                    Response.Redirect($"~/TimeManagement/ViewTimesheet.aspx?id={TimesheetId}");
                                    return;
                                }
                            }
                            else
                            {
                                ShowMessage("Timesheet not found.", "error");
                                Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error validating access: {ex.Message}", "error");
                Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
            }
        }

        #endregion

        #region Data Loading

        /// <summary>
        /// Loads timesheet data and populates the form controls
        /// </summary>
        private void LoadTimesheetData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT ts.*, e.FirstName, e.LastName, e.EmployeeNumber, 
                               d.Name as DepartmentName, e.Position
                        FROM TimeSheets ts
                        INNER JOIN Employees e ON ts.EmployeeId = e.Id
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        WHERE ts.Id = @TimesheetId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Populate employee information
                                litEmployeeName.Text = $"{reader["FirstName"]} {reader["LastName"]}";
                                litEmployeeNumber.Text = reader["EmployeeNumber"].ToString();
                                litDepartment.Text = reader["DepartmentName"]?.ToString() ?? "N/A";
                                litPosition.Text = reader["Position"]?.ToString() ?? "N/A";

                                // Populate timesheet data
                                DateTime weekStart = Convert.ToDateTime(reader["WeekStartDate"]);
                                DateTime weekEnd = Convert.ToDateTime(reader["WeekEndDate"]);
                                litWeekRange.Text = $"{weekStart:MMM dd} - {weekEnd:MMM dd, yyyy}";

                                string status = reader["Status"].ToString();
                                litTimesheetStatus.Text = status;
                                ViewState["TimesheetStatus"] = status;

                                litTotalHours.Text = Convert.ToDecimal(reader["TotalHours"] ?? 0).ToString("F1");
                                litRegularHours.Text = Convert.ToDecimal(reader["RegularHours"] ?? 0).ToString("F1");
                                litOvertimeHours.Text = Convert.ToDecimal(reader["OvertimeHours"] ?? 0).ToString("F1");

                                txtTimesheetNotes.Text = reader["Notes"]?.ToString() ?? "";

                                // Set date literals for each day
                                SetDayDates(weekStart);
                            }
                        }
                    }

                    // Load individual day entries
                    LoadDayEntries(conn);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading timesheet data: {ex.Message}", "error");
            }
        }

        /// <summary>
        /// Sets the date literals for each day of the week
        /// </summary>
        /// <param name="weekStart">Start date of the week</param>
        private void SetDayDates(DateTime weekStart)
        {
            litMondayDate.Text = weekStart.ToString("MMM dd");
            litTuesdayDate.Text = weekStart.AddDays(1).ToString("MMM dd");
            litWednesdayDate.Text = weekStart.AddDays(2).ToString("MMM dd");
            litThursdayDate.Text = weekStart.AddDays(3).ToString("MMM dd");
            litFridayDate.Text = weekStart.AddDays(4).ToString("MMM dd");
            litSaturdayDate.Text = weekStart.AddDays(5).ToString("MMM dd");
            litSundayDate.Text = weekStart.AddDays(6).ToString("MMM dd");
        }

        /// <summary>
        /// Loads individual day time entries from TimeEntries table
        /// </summary>
        /// <param name="conn">Database connection</param>
        private void LoadDayEntries(SqlConnection conn)
        {
            try
            {
                string query = @"
                    SELECT DATEPART(WEEKDAY, ClockIn) as DayOfWeek,
                           FORMAT(ClockIn, 'HH:mm') as StartTime,
                           FORMAT(ClockOut, 'HH:mm') as EndTime,
                           ISNULL(DATEDIFF(MINUTE, ClockIn, ClockOut), 0) as TotalMinutes,
                           Notes
                    FROM TimeEntries 
                    WHERE EmployeeId = @EmployeeId 
                        AND ClockIn >= (SELECT WeekStartDate FROM TimeSheets WHERE Id = @TimesheetId)
                        AND ClockIn < (SELECT DATEADD(DAY, 7, WeekStartDate) FROM TimeSheets WHERE Id = @TimesheetId)
                    ORDER BY ClockIn";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                    cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int dayOfWeek = Convert.ToInt32(reader["DayOfWeek"]);
                            string startTime = reader["StartTime"]?.ToString() ?? "";
                            string endTime = reader["EndTime"]?.ToString() ?? "";
                            int totalMinutes = Convert.ToInt32(reader["TotalMinutes"] ?? 0);
                            string notes = reader["Notes"]?.ToString() ?? "";

                            // Calculate break time (assume 30 min default, adjust based on your business logic)
                            int workMinutes = totalMinutes;
                            int breakMinutes = 30; // Default break
                            if (totalMinutes > 360) // More than 6 hours
                                breakMinutes = 60; // 1 hour break

                            PopulateDayControls(dayOfWeek, startTime, endTime, breakMinutes, notes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading day entries: {ex.Message}");
            }
        }

        /// <summary>
        /// Populates day controls with time entry data
        /// </summary>
        private void PopulateDayControls(int sqlDayOfWeek, string startTime, string endTime, int breakMinutes, string notes)
        {
            // Convert SQL DATEPART weekday to our controls
            // SQL: 1=Sunday, 2=Monday, 3=Tuesday, 4=Wednesday, 5=Thursday, 6=Friday, 7=Saturday

            TextBox startControl = null, endControl = null, notesControl = null, breakControl = null;

            switch (sqlDayOfWeek)
            {
                case 1: // Sunday
                    startControl = txtSundayStart;
                    endControl = txtSundayEnd;
                    notesControl = txtSundayNotes;
                    breakControl = txtSundayBreak;
                    break;
                case 2: // Monday
                    startControl = txtMondayStart;
                    endControl = txtMondayEnd;
                    notesControl = txtMondayNotes;
                    breakControl = txtMondayBreak;
                    break;
                case 3: // Tuesday
                    startControl = txtTuesdayStart;
                    endControl = txtTuesdayEnd;
                    notesControl = txtTuesdayNotes;
                    breakControl = txtTuesdayBreak;
                    break;
                case 4: // Wednesday
                    startControl = txtWednesdayStart;
                    endControl = txtWednesdayEnd;
                    notesControl = txtWednesdayNotes;
                    breakControl = txtWednesdayBreak;
                    break;
                case 5: // Thursday
                    startControl = txtThursdayStart;
                    endControl = txtThursdayEnd;
                    notesControl = txtThursdayNotes;
                    breakControl = txtThursdayBreak;
                    break;
                case 6: // Friday
                    startControl = txtFridayStart;
                    endControl = txtFridayEnd;
                    notesControl = txtFridayNotes;
                    breakControl = txtFridayBreak;
                    break;
                case 7: // Saturday
                    startControl = txtSaturdayStart;
                    endControl = txtSaturdayEnd;
                    notesControl = txtSaturdayNotes;
                    breakControl = txtSaturdayBreak;
                    break;
            }

            if (startControl != null)
            {
                startControl.Text = startTime;
                endControl.Text = endTime;
                breakControl.Text = breakMinutes.ToString();
                notesControl.Text = notes;
            }
        }

        #endregion

        #region Button Event Handlers

        protected void btnBackToTimesheets_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            SaveTimesheet(false);
        }

        protected void btnSubmitTimesheet_Click(object sender, EventArgs e)
        {
            if (ValidateTimesheet())
            {
                SaveTimesheet(true);
            }
        }

        protected void btnPreviewTimesheet_Click(object sender, EventArgs e)
        {
            try
            {
                // Save current data first
                SaveTimesheet(false);

                // Redirect to view page
                Response.Redirect($"ViewTimesheet.aspx?id={TimesheetId}");
            }
            catch (Exception ex)
            {
                ShowMessage($"Error previewing timesheet: {ex.Message}", "error");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteTimesheet();
        }

        #endregion

        #region Save and Validation Logic

        /// <summary>
        /// Validates timesheet data before submission
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        private bool ValidateTimesheet()
        {
            try
            {
                var (totalHours, _, _) = CalculateTimesheetTotals();

                if (totalHours == 0)
                {
                    ShowMessage("Please enter at least one day of time before submitting.", "warning");
                    return false;
                }

                if (totalHours > 80) // More than 80 hours per week seems excessive
                {
                    ShowMessage("Total hours exceed maximum allowed (80 hours). Please review your entries.", "warning");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error validating timesheet: {ex.Message}", "error");
                return false;
            }
        }

        /// <summary>
        /// Saves timesheet data to database
        /// </summary>
        /// <param name="submitForApproval">Whether to submit for approval or save as draft</param>
        private void SaveTimesheet(bool submitForApproval = false)
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
                            // Calculate totals
                            var (totalHours, regularHours, overtimeHours) = CalculateTimesheetTotals();
                            int daysWorked = CountDaysWorked();

                            // Update timesheet record
                            string status = submitForApproval ? "SUBMITTED" : "DRAFT";
                            DateTime? submittedAt = submitForApproval ? DateTime.UtcNow : (DateTime?)null;

                            string updateQuery = @"
                                UPDATE TimeSheets 
                                SET TotalHours = @TotalHours,
                                    RegularHours = @RegularHours,
                                    OvertimeHours = @OvertimeHours,
                                    Status = @Status,
                                    SubmittedAt = @SubmittedAt,
                                    Notes = @Notes,
                                    UpdatedAt = GETUTCDATE()
                                WHERE Id = @TimesheetId";

                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@TotalHours", totalHours);
                                cmd.Parameters.AddWithValue("@RegularHours", regularHours);
                                cmd.Parameters.AddWithValue("@OvertimeHours", overtimeHours);
                                cmd.Parameters.AddWithValue("@Status", status);
                                cmd.Parameters.AddWithValue("@SubmittedAt", submittedAt ?? (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@Notes", txtTimesheetNotes.Text ?? "");
                                cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);

                                cmd.ExecuteNonQuery();
                            }

                            // Clear existing time entries for this timesheet period
                            ClearExistingTimeEntries(conn, transaction);

                            // Save new time entries
                            SaveTimeEntries(conn, transaction);

                            transaction.Commit();

                            string message = submitForApproval ?
                                "Timesheet submitted for approval successfully!" :
                                "Timesheet saved as draft successfully!";
                            ShowMessage(message, "success");

                            // Update display
                            ViewState["TimesheetStatus"] = status;
                            litTimesheetStatus.Text = status;
                            litTotalHours.Text = totalHours.ToString("F1");
                            litRegularHours.Text = regularHours.ToString("F1");
                            litOvertimeHours.Text = overtimeHours.ToString("F1");
                            litDaysWorked.Text = daysWorked.ToString();

                            if (submitForApproval)
                            {
                                // Redirect to view mode after submission
                                Response.Redirect($"ViewTimesheet.aspx?id={TimesheetId}");
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error saving timesheet: {ex.Message}", "error");
            }
        }

        /// <summary>
        /// Clears existing time entries for this timesheet period
        /// </summary>
        private void ClearExistingTimeEntries(SqlConnection conn, SqlTransaction transaction)
        {
            string deleteQuery = @"
                DELETE FROM TimeEntries 
                WHERE EmployeeId = @EmployeeId 
                    AND ClockIn >= (SELECT WeekStartDate FROM TimeSheets WHERE Id = @TimesheetId)
                    AND ClockIn < (SELECT DATEADD(DAY, 7, WeekStartDate) FROM TimeSheets WHERE Id = @TimesheetId)";

            using (SqlCommand cmd = new SqlCommand(deleteQuery, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Saves time entries for each day that has data
        /// </summary>
        private void SaveTimeEntries(SqlConnection conn, SqlTransaction transaction)
        {
            DateTime weekStart = GetWeekStartDate();

            // Save entries for each day that has data
            SaveDayEntry(conn, transaction, weekStart, 0, txtMondayStart, txtMondayEnd, txtMondayBreak, txtMondayNotes); // Monday
            SaveDayEntry(conn, transaction, weekStart, 1, txtTuesdayStart, txtTuesdayEnd, txtTuesdayBreak, txtTuesdayNotes); // Tuesday
            SaveDayEntry(conn, transaction, weekStart, 2, txtWednesdayStart, txtWednesdayEnd, txtWednesdayBreak, txtWednesdayNotes); // Wednesday
            SaveDayEntry(conn, transaction, weekStart, 3, txtThursdayStart, txtThursdayEnd, txtThursdayBreak, txtThursdayNotes); // Thursday
            SaveDayEntry(conn, transaction, weekStart, 4, txtFridayStart, txtFridayEnd, txtFridayBreak, txtFridayNotes); // Friday
            SaveDayEntry(conn, transaction, weekStart, 5, txtSaturdayStart, txtSaturdayEnd, txtSaturdayBreak, txtSaturdayNotes); // Saturday
            SaveDayEntry(conn, transaction, weekStart, 6, txtSundayStart, txtSundayEnd, txtSundayBreak, txtSundayNotes); // Sunday
        }

        /// <summary>
        /// Saves a single day's time entry
        /// </summary>
        private void SaveDayEntry(SqlConnection conn, SqlTransaction transaction, DateTime weekStart, int dayOffset,
            TextBox startControl, TextBox endControl, TextBox breakControl, TextBox notesControl)
        {
            if (string.IsNullOrEmpty(startControl.Text) || string.IsNullOrEmpty(endControl.Text))
                return;

            try
            {
                DateTime entryDate = weekStart.AddDays(dayOffset);
                DateTime startTime = DateTime.Parse($"{entryDate:yyyy-MM-dd} {startControl.Text}");
                DateTime endTime = DateTime.Parse($"{entryDate:yyyy-MM-dd} {endControl.Text}");
                int breakMinutes = int.TryParse(breakControl.Text, out int breakMins) ? breakMins : 0;

                // Handle overnight shifts
                if (endTime <= startTime)
                {
                    endTime = endTime.AddDays(1);
                }

                // Calculate total hours
                double totalMinutes = (endTime - startTime).TotalMinutes - breakMinutes;
                decimal totalHours = (decimal)(Math.Max(0, totalMinutes) / 60.0);

                string insertQuery = @"
                    INSERT INTO TimeEntries (EmployeeId, ClockIn, ClockOut, TotalHours, Status, Notes, CreatedAt)
                    VALUES (@EmployeeId, @ClockIn, @ClockOut, @TotalHours, 'Completed', @Notes, GETUTCDATE())";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                    cmd.Parameters.AddWithValue("@ClockIn", startTime);
                    cmd.Parameters.AddWithValue("@ClockOut", endTime);
                    cmd.Parameters.AddWithValue("@TotalHours", totalHours);
                    cmd.Parameters.AddWithValue("@Notes", notesControl.Text ?? "");

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving day entry: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets the week start date for this timesheet
        /// </summary>
        private DateTime GetWeekStartDate()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT WeekStartDate FROM TimeSheets WHERE Id = @TimesheetId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToDateTime(result) : DateTime.Today;
                }
            }
        }

        /// <summary>
        /// Deletes the timesheet
        /// </summary>
        private void DeleteTimesheet()
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
                            // First delete related time entries
                            ClearExistingTimeEntries(conn, transaction);

                            // Then delete the timesheet
                            string deleteQuery = "DELETE FROM TimeSheets WHERE Id = @TimesheetId AND EmployeeId = @EmployeeId";
                            using (SqlCommand cmd = new SqlCommand(deleteQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    transaction.Commit();
                                    ShowMessage("Timesheet deleted successfully!", "success");
                                    Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
                                }
                                else
                                {
                                    transaction.Rollback();
                                    ShowMessage("Unable to delete timesheet. Please try again.", "error");
                                }
                            }
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting timesheet: {ex.Message}", "error");
            }
        }

        #endregion

        #region Calculation Methods

        /// <summary>
        /// Calculate timesheet totals from form inputs
        /// </summary>
        /// <returns>Tuple of total, regular, and overtime hours</returns>
        private (decimal totalHours, decimal regularHours, decimal overtimeHours) CalculateTimesheetTotals()
        {
            decimal totalHours = 0;
            decimal regularHours = 0;
            decimal overtimeHours = 0;

            var dayControls = new (TextBox start, TextBox end, TextBox breakTime)[]
            {
                (txtMondayStart, txtMondayEnd, txtMondayBreak),
                (txtTuesdayStart, txtTuesdayEnd, txtTuesdayBreak),
                (txtWednesdayStart, txtWednesdayEnd, txtWednesdayBreak),
                (txtThursdayStart, txtThursdayEnd, txtThursdayBreak),
                (txtFridayStart, txtFridayEnd, txtFridayBreak),
                (txtSaturdayStart, txtSaturdayEnd, txtSaturdayBreak),
                (txtSundayStart, txtSundayEnd, txtSundayBreak)
            };

            foreach (var day in dayControls)
            {
                if (!string.IsNullOrEmpty(day.start.Text) && !string.IsNullOrEmpty(day.end.Text))
                {
                    try
                    {
                        DateTime start = DateTime.Parse($"2000-01-01 {day.start.Text}");
                        DateTime end = DateTime.Parse($"2000-01-01 {day.end.Text}");
                        int breakMinutes = int.TryParse(day.breakTime.Text, out int breakMins) ? breakMins : 0;

                        // Handle overnight shifts
                        if (end <= start)
                        {
                            end = end.AddDays(1);
                        }

                        double dayHours = (end - start).TotalHours - (breakMinutes / 60.0);

                        if (dayHours > 0)
                        {
                            totalHours += (decimal)dayHours;

                            if (dayHours > 8)
                            {
                                regularHours += 8;
                                overtimeHours += (decimal)(dayHours - 8);
                            }
                            else
                            {
                                regularHours += (decimal)dayHours;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error calculating day total: {ex.Message}");
                    }
                }
            }

            return (Math.Round(totalHours, 2), Math.Round(regularHours, 2), Math.Round(overtimeHours, 2));
        }

        /// <summary>
        /// Counts the number of days with time entries
        /// </summary>
        /// <returns>Number of days worked</returns>
        private int CountDaysWorked()
        {
            int daysWorked = 0;

            var dayControls = new (TextBox start, TextBox end)[]
            {
                (txtMondayStart, txtMondayEnd),
                (txtTuesdayStart, txtTuesdayEnd),
                (txtWednesdayStart, txtWednesdayEnd),
                (txtThursdayStart, txtThursdayEnd),
                (txtFridayStart, txtFridayEnd),
                (txtSaturdayStart, txtSaturdayEnd),
                (txtSundayStart, txtSundayEnd)
            };

            foreach (var day in dayControls)
            {
                if (!string.IsNullOrEmpty(day.start.Text) && !string.IsNullOrEmpty(day.end.Text))
                {
                    daysWorked++;
                }
            }

            return daysWorked;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the CSS class for the timesheet status badge
        /// </summary>
        /// <returns>CSS class name for status styling</returns>
        protected string GetStatusClass()
        {
            if (ViewState["TimesheetStatus"] == null) return "draft";

            string status = ViewState["TimesheetStatus"].ToString().ToLower();

            switch (status)
            {
                case "draft":
                    return "draft";
                case "submitted":
                case "pending":
                    return "submitted";
                case "approved":
                    return "approved";
                case "rejected":
                    return "rejected";
                default:
                    return "draft";
            }
        }

        /// <summary>
        /// Shows a message to the user
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="type">Message type (success, error, warning, info)</param>
        protected void ShowMessage(string message, string type)
        {
            try
            {
                if (pnlMessage != null && litMessage != null)
                {
                    pnlMessage.Visible = true;
                    litMessage.Text = message;
                    pnlMessage.CssClass = $"alert-panel {type}";

                    // Auto-hide success messages after 5 seconds
                    if (type == "success")
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "hideMessage",
                            "setTimeout(function() { document.querySelector('.alert-panel').style.display = 'none'; }, 5000);", true);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing message: {ex.Message}");
            }
        }

        #endregion
    }
}