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

        private int CurrentEmployeeId
        {
            get
            {
                return GetCurrentEmployeeId();
            }
        }

        private int TimesheetId
        {
            get
            {
                if (int.TryParse(Request.QueryString["id"], out int id))
                    return id;
                return 0;
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (TimesheetId <= 0)
                {
                    ShowMessage("Invalid timesheet ID.", "error");
                    Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
                    return;
                }

                ValidateUserAccess();
                LoadTimesheetData();
                SetupWeekDates();
            }
        }

        #endregion

        #region Button Events

        protected void btnBackToTimesheets_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            try
            {
                if (SaveTimesheet("Draft"))
                {
                    ShowMessage("Timesheet saved successfully as draft.", "success");
                }
                else
                {
                    ShowMessage("Error saving timesheet. Please try again.", "error");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving draft: {ex.Message}");
                ShowMessage("An error occurred while saving the timesheet.", "error");
            }
        }

        protected void btnSubmitTimesheet_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate that there are time entries
                if (!HasTimeEntries())
                {
                    ShowMessage("Please add at least one day of time entries before submitting.", "error");
                    return;
                }

                if (SaveTimesheet("Submitted"))
                {
                    ShowMessage("Timesheet submitted successfully for approval!", "success");
                    Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
                }
                else
                {
                    ShowMessage("Error submitting timesheet. Please try again.", "error");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error submitting timesheet: {ex.Message}");
                ShowMessage("An error occurred while submitting the timesheet.", "error");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (DeleteTimesheet())
                {
                    ShowMessage("Timesheet deleted successfully.", "success");
                    Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
                }
                else
                {
                    ShowMessage("Error deleting timesheet. Only draft timesheets can be deleted.", "error");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting timesheet: {ex.Message}");
                ShowMessage("An error occurred while deleting the timesheet.", "error");
            }
        }

        #endregion

        #region Data Loading Methods

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
                    Response.Redirect("~/Dashboard.aspx");
                    return;
                }

                // Verify the timesheet belongs to this employee
                if (!VerifyTimesheetOwnership())
                {
                    ShowMessage("You do not have permission to edit this timesheet.", "error");
                    Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating user access: {ex.Message}");
                Response.Redirect("~/Login.aspx");
            }
        }

        private bool VerifyTimesheetOwnership()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM TimeSheets 
                        WHERE Id = @TimesheetId AND EmployeeId = @EmployeeId", conn))
                    {
                        cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        int count = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error verifying timesheet ownership: {ex.Message}");
                return false;
            }
        }

        private void LoadTimesheetData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load timesheet header information
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT ts.WeekStartDate, ts.WeekEndDate, ts.Status, ts.Notes,
                               e.FirstName, e.LastName
                        FROM TimeSheets ts
                        INNER JOIN Employees e ON ts.EmployeeId = e.Id
                        WHERE ts.Id = @TimesheetId", conn))
                    {
                        cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DateTime weekStart = Convert.ToDateTime(reader["WeekStartDate"]);
                                DateTime weekEnd = Convert.ToDateTime(reader["WeekEndDate"]);

                                litWeekPeriod.Text = $"{weekStart:MMM dd} - {weekEnd:MMM dd, yyyy}";
                                litEmployeeName.Text = $"{reader["FirstName"]} {reader["LastName"]}";
                                litTimesheetStatus.Text = reader["Status"].ToString();
                                txtTimesheetNotes.Text = reader["Notes"]?.ToString() ?? "";

                                // Disable editing if not in draft or rejected status
                                string status = reader["Status"].ToString();
                                if (status != "Draft" && status != "Rejected")
                                {
                                    DisableEditing();
                                }
                            }
                        }
                    }

                    // Load existing time entries
                    LoadExistingTimeEntries(conn);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading timesheet data: {ex.Message}");
                ShowMessage("Error loading timesheet data.", "error");
            }
        }

        private void LoadExistingTimeEntries(SqlConnection conn)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT te.ClockIn, te.ClockOut, te.Notes, te.TotalHours,
                           DATEPART(WEEKDAY, te.ClockIn) as DayOfWeek
                    FROM TimeEntries te
                    INNER JOIN TimeSheets ts ON DATEPART(week, te.ClockIn) = DATEPART(week, ts.WeekStartDate)
                        AND YEAR(te.ClockIn) = YEAR(ts.WeekStartDate)
                    WHERE ts.Id = @TimesheetId AND te.EmployeeId = @EmployeeId
                    ORDER BY te.ClockIn", conn))
                {
                    cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                    cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime clockIn = Convert.ToDateTime(reader["ClockIn"]);
                            DateTime clockOut = DateTime.MinValue;
                            bool hasClockOut = false;

                            if (reader["ClockOut"] != DBNull.Value)
                            {
                                clockOut = Convert.ToDateTime(reader["ClockOut"]);
                                hasClockOut = true;
                            }

                            string notes = reader["Notes"]?.ToString() ?? "";
                            int dayOfWeek = Convert.ToInt32(reader["DayOfWeek"]);

                            // Map SQL DATEPART weekday (1=Sunday, 2=Monday, etc.) to our day controls
                            PopulateDayControls(dayOfWeek, clockIn, clockOut, hasClockOut, notes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading time entries: {ex.Message}");
            }
        }

        private void PopulateDayControls(int sqlDayOfWeek, DateTime clockIn, DateTime clockOut, bool hasClockOut, string notes)
        {
            // Convert SQL DATEPART weekday to our naming convention
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
                startControl.Text = clockIn.ToString("HH:mm");
                if (hasClockOut)
                {
                    endControl.Text = clockOut.ToString("HH:mm");
                }
                notesControl.Text = notes;

                // Calculate break time if we have both start and end times
                if (hasClockOut)
                {
                    var workDuration = clockOut - clockIn;
                    var expectedBreakMinutes = workDuration.TotalHours > 6 ? 30 : 0; // Assume 30 min break for 6+ hour days
                    breakControl.Text = expectedBreakMinutes.ToString();
                }
            }
        }

        private void SetupWeekDates()
        {
            //try
            //{
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT WeekStartDate FROM TimeSheets WHERE Id = @TimesheetId", conn))
                    {
                        cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            DateTime weekStart = Convert.ToDateTime(result);

                            // Set date labels for each day
                            litMondayDate.Text = weekStart.ToString("MMM dd");
                            litTuesdayDate.Text = weekStart.AddDays(1).ToString("MMM dd");
                            litWednesdayDate.Text = weekStart.AddDays(2).ToString("MMM dd");
                            litThursdayDate.Text = weekStart.AddDays(3).ToString("MMM dd");
                            litFridayDate.Text = weekStart.AddDays(4).ToString("MMM dd");
                            litSaturdayDate.Text = weekStart.AddDays(5).ToString("MMM dd");
                            litSundayDate.Text = weekStart.AddDays(6).ToString("MMM dd");
                        }
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine($"Error setting up week dates: {ex.Message}");
            //}
        }

        private void DisableEditing()
        {
            // Disable all input controls
            var textBoxes = new TextBox[] {
                txtMondayStart, txtMondayEnd, txtMondayBreak, txtMondayNotes,
                txtTuesdayStart, txtTuesdayEnd, txtTuesdayBreak, txtTuesdayNotes,
                txtWednesdayStart, txtWednesdayEnd, txtWednesdayBreak, txtWednesdayNotes,
                txtThursdayStart, txtThursdayEnd, txtThursdayBreak, txtThursdayNotes,
                txtFridayStart, txtFridayEnd, txtFridayBreak, txtFridayNotes,
                txtSaturdayStart, txtSaturdayEnd, txtSaturdayBreak, txtSaturdayNotes,
                txtSundayStart, txtSundayEnd, txtSundayBreak, txtSundayNotes,
                txtTimesheetNotes
            };

            foreach (var textBox in textBoxes)
            {
                textBox.Enabled = false;
            }

            // Disable action buttons except back button
            btnSaveDraft.Visible = false;
            btnSubmitTimesheet.Visible = false;
            btnDelete.Visible = false;
        }

        #endregion

        #region Save and Validation Methods

        private bool SaveTimesheet(string status)
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
                            var totals = CalculateTotals();

                            // Update timesheet header
                            using (SqlCommand cmd = new SqlCommand(@"
                                UPDATE TimeSheets 
                                SET TotalHours = @TotalHours, 
                                    RegularHours = @RegularHours, 
                                    OvertimeHours = @OvertimeHours,
                                    Status = @Status,
                                    Notes = @Notes,
                                    UpdatedAt = GETDATE(),
                                    SubmittedAt = CASE WHEN @Status = 'Submitted' THEN GETDATE() ELSE SubmittedAt END
                                WHERE Id = @TimesheetId AND EmployeeId = @EmployeeId", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@TotalHours", totals.TotalHours);
                                cmd.Parameters.AddWithValue("@RegularHours", totals.RegularHours);
                                cmd.Parameters.AddWithValue("@OvertimeHours", totals.OvertimeHours);
                                cmd.Parameters.AddWithValue("@Status", status);
                                cmd.Parameters.AddWithValue("@Notes", txtTimesheetNotes.Text.Trim());
                                cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                                cmd.ExecuteNonQuery();
                            }

                            // Clear existing time entries for this week and employee
                            ClearExistingTimeEntries(conn, transaction);

                            // Save new time entries
                            SaveTimeEntries(conn, transaction);

                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            System.Diagnostics.Debug.WriteLine($"Error in SaveTimesheet transaction: {ex.Message}");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving timesheet: {ex.Message}");
                return false;
            }
        }

        private void ClearExistingTimeEntries(SqlConnection conn, SqlTransaction transaction)
        {
            using (SqlCommand cmd = new SqlCommand(@"
                DELETE te FROM TimeEntries te
                INNER JOIN TimeSheets ts ON DATEPART(week, te.ClockIn) = DATEPART(week, ts.WeekStartDate)
                    AND YEAR(te.ClockIn) = YEAR(ts.WeekStartDate)
                WHERE ts.Id = @TimesheetId AND te.EmployeeId = @EmployeeId", conn, transaction))
            {
                cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                cmd.ExecuteNonQuery();
            }
        }

        private void SaveTimeEntries(SqlConnection conn, SqlTransaction transaction)
        {
            // Get week start date
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

                // Calculate total hours
                double totalHours = (endTime - startTime).TotalHours - (breakMinutes / 60.0);

                if (totalHours > 0)
                {
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO TimeEntries (EmployeeId, ClockIn, ClockOut, TotalHours, Status, Notes, CreatedAt)
                        VALUES (@EmployeeId, @ClockIn, @ClockOut, @TotalHours, 'Completed', @Notes, GETDATE())", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        cmd.Parameters.AddWithValue("@ClockIn", startTime);
                        cmd.Parameters.AddWithValue("@ClockOut", endTime);
                        cmd.Parameters.AddWithValue("@TotalHours", Math.Round(totalHours, 2));
                        cmd.Parameters.AddWithValue("@Notes", notesControl.Text.Trim());

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving day entry for day {dayOffset}: {ex.Message}");
            }
        }

        private DateTime GetWeekStartDate()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT WeekStartDate FROM TimeSheets WHERE Id = @TimesheetId", conn))
                {
                    cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                    return Convert.ToDateTime(cmd.ExecuteScalar());
                }
            }
        }

        private (decimal TotalHours, decimal RegularHours, decimal OvertimeHours) CalculateTotals()
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

        private bool HasTimeEntries()
        {
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
                    return true;
                }
            }

            return false;
        }

        private bool DeleteTimesheet()
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
                        cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting timesheet: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Helper Methods

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
                        using (SqlCommand cmd = new SqlCommand("SELECT Id FROM Employees WHERE UserId = @UserId AND IsActive = 1", conn))
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

        #endregion
    }
}