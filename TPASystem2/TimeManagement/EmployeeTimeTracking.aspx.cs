using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Globalization;

namespace TPASystem2.TimeManagement
{
    public partial class EmployeeTimeTracking : System.Web.UI.Page
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
                LoadTodaysSchedule();
                LoadCurrentStatus();
                LoadRecentEntries();
                UpdateClockButtons();
                SetActiveSessionData();
            }
        }

        #endregion

        #region Button Events

        protected void btnClockIn_Click(object sender, EventArgs e)
        {
            //try
            //{
                if (HasActiveEntry())
                {
                    ShowMessage("You are already clocked in. Please clock out first.", "error");
                    return;
                }

                int entryId = ClockIn();
                if (entryId > 0)
                {
                    ShowMessage("Successfully clocked in!", "success");
                    LoadCurrentStatus();
                    LoadRecentEntries();
                    UpdateClockButtons();
                    SetActiveSessionData();
                }
                else
                {
                    ShowMessage("Error clocking in. Please try again.", "error");
                }
            //}
            //catch (Exception ex)
            //{
            //    LogError(ex);
            //    ShowMessage("An error occurred while clocking in. Please try again.", "error");
            //}
        }

        protected void btnClockOut_Click(object sender, EventArgs e)
        {
            //try
            //{
                var activeEntry = GetActiveEntry();
                if (activeEntry == null)
                {
                    ShowMessage("No active time entry found to clock out.", "error");
                    return;
                }

                // End any active break first
                if (IsOnBreak())
                {
                    EndBreak(activeEntry.Id);
                }

                if (ClockOut(activeEntry.Id))
                {
                    ShowMessage("Successfully clocked out!", "success");
                    LoadCurrentStatus();
                    LoadRecentEntries();
                    UpdateClockButtons();
                    ClearActiveSessionData();
                }
                else
                {
                    ShowMessage("Error clocking out. Please try again.", "error");
                }
            //}
            //catch (Exception ex)
            //{
            //    LogError(ex);
            //    ShowMessage("An error occurred while clocking out. Please try again.", "error");
            //}
        }

        protected void btnStartBreak_Click(object sender, EventArgs e)
        {
            try
            {
                var activeEntry = GetActiveEntry();
                if (activeEntry == null)
                {
                    ShowMessage("You must be clocked in to start a break.", "error");
                    return;
                }

                if (IsOnBreak())
                {
                    ShowMessage("You are already on break.", "warning");
                    return;
                }

                if (StartBreak(activeEntry.Id))
                {
                    ShowMessage("Break started successfully!", "success");
                    LoadCurrentStatus();
                    UpdateClockButtons();
                    SetBreakData();
                }
                else
                {
                    ShowMessage("Error starting break. Please try again.", "error");
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("An error occurred while starting break.", "error");
            }
        }

        protected void btnEndBreak_Click(object sender, EventArgs e)
        {
            try
            {
                var activeEntry = GetActiveEntry();
                if (activeEntry == null)
                {
                    ShowMessage("No active time entry found.", "error");
                    return;
                }

                if (!IsOnBreak())
                {
                    ShowMessage("You are not currently on break.", "warning");
                    return;
                }

                if (EndBreak(activeEntry.Id))
                {
                    ShowMessage("Break ended successfully!", "success");
                    LoadCurrentStatus();
                    UpdateClockButtons();
                    ClearBreakData();
                }
                else
                {
                    ShowMessage("Error ending break. Please try again.", "error");
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("An error occurred while ending break.", "error");
            }
        }

        protected void btnAddEntry_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateManualEntry())
                {
                    if (AddManualEntry())
                    {
                        ShowMessage("Manual time entry added successfully!", "success");
                        ClearManualEntryForm();
                        LoadCurrentStatus();
                        LoadRecentEntries();
                    }
                    else
                    {
                        ShowMessage("Error adding time entry. Please try again.", "error");
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("An error occurred while adding the time entry.", "error");
            }
        }

        protected void btnViewTimesheets_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
        }

        protected void btnTimeReports_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/TimeManagement/TimeReports.aspx");
        }

        protected void btnViewAllEntries_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
        }

        protected void btnHiddenRefresh_Click(object sender, EventArgs e)
        {
            LoadCurrentStatus();
            SetActiveSessionData();
        }

        protected void FilterChanged(object sender, EventArgs e)
        {
            LoadRecentEntries();
        }

        protected void EntryAction_Command(object sender, CommandEventArgs e)
        {
            try
            {
                if (e.CommandArgument != null && int.TryParse(e.CommandArgument.ToString(), out int entryId))
                {
                    switch (e.CommandName)
                    {
                        case "EditEntry":
                            Response.Redirect($"~/TimeManagement/EditTimeEntry.aspx?id={entryId}");
                            break;
                        case "ClockOutEntry":
                            if (ClockOut(entryId))
                            {
                                ShowMessage("Successfully clocked out!", "success");
                                LoadCurrentStatus();
                                LoadRecentEntries();
                                UpdateClockButtons();
                                ClearActiveSessionData();
                            }
                            break;
                    }
                }
                else
                {
                    ShowMessage("Invalid entry ID.", "error");
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("An error occurred while processing the request.", "error");
            }
        }

        #endregion

        #region Data Loading Methods

        private void LoadEmployeeData()
        {
            //try
            //{
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT e.FirstName, e.LastName, e.EmployeeNumber, e.Position
                        FROM Employees e
                        WHERE e.Id = @EmployeeId AND e.Status='Active'", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litEmployeeName.Text = $"{reader["FirstName"]} {reader["LastName"]}";
                                litEmployeeNumber.Text = reader["EmployeeNumber"]?.ToString() ?? "N/A";
                            }
                            else
                            {
                                ShowMessage("Employee information not found.", "error");
                                Response.Redirect("~/Dashboard.aspx");
                            }
                        }
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    LogError(ex);
            //    ShowMessage("Error loading employee data.", "error");
            //}
        }

        private void LoadTodaysSchedule()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get today's day of week (1=Monday, 7=Sunday)
                    int dayOfWeek = ((int)DateTime.Today.DayOfWeek + 6) % 7 + 1;
                    if (dayOfWeek == 8) dayOfWeek = 1; // Sunday = 1

                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT StartTime, EndTime, 
                               DATEDIFF(HOUR, StartTime, EndTime) as ExpectedHours
                        FROM Schedules 
                        WHERE EmployeeId = @EmployeeId 
                          AND DayOfWeek = @DayOfWeek 
                          AND IsActive = 1
                          AND EffectiveDate <= @Today
                        ORDER BY EffectiveDate DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        cmd.Parameters.AddWithValue("@DayOfWeek", dayOfWeek);
                        cmd.Parameters.AddWithValue("@Today", DateTime.Today);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DateTime startTime = DateTime.Today.Add((TimeSpan)reader["StartTime"]);
                                DateTime endTime = DateTime.Today.Add((TimeSpan)reader["EndTime"]);

                                litScheduledStart.Text = startTime.ToString("h:mm tt");
                                litScheduledEnd.Text = endTime.ToString("h:mm tt");
                                litExpectedHours.Text = reader["ExpectedHours"].ToString();
                            }
                            else
                            {
                                // No schedule found for today
                                pnlTodaysSchedule.Visible = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                pnlTodaysSchedule.Visible = false;
            }
        }

        private void LoadCurrentStatus()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get today's total hours
                    decimal todaysHours = GetTodaysHours(conn);
                    litTodaysHours.Text = todaysHours.ToString("F1");

                    // Get this week's total hours
                    decimal weekHours = GetWeekHours(conn);
                    litWeekTotal.Text = weekHours.ToString("F1");

                    // Get current status and last action
                    var activeEntry = GetActiveEntry();
                    if (activeEntry != null)
                    {
                        bool onBreak = IsOnBreak();

                        if (onBreak)
                        {
                            litCurrentStatus.Text = "On Break";
                            litBreakStatus.Text = "On break";
                            litLastAction.Text = $"Break started at {GetLastBreakStart()?.ToString("h:mm tt") ?? "Unknown"}";
                        }
                        else
                        {
                            litCurrentStatus.Text = "Clocked In";
                            litBreakStatus.Text = "Working";
                            litLastAction.Text = $"Clocked in at {activeEntry.ClockIn.ToString("h:mm tt")}";
                        }
                    }
                    else
                    {
                        litCurrentStatus.Text = "Clocked Out";
                        litBreakStatus.Text = "Not on break";

                        var lastEntry = GetLastTimeEntry();
                        if (lastEntry != null && lastEntry.ClockOut.HasValue)
                        {
                            litLastAction.Text = $"Clocked out at {lastEntry.ClockOut.Value.ToString("h:mm tt")}";
                        }
                        else
                        {
                            litLastAction.Text = "No recent activity";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading current status.", "error");
            }
        }

        private void LoadRecentEntries()
        {
            try
            {
                int days = int.Parse(ddlEntryFilter.SelectedValue);
                DateTime startDate = DateTime.Today.AddDays(-days);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT te.Id, te.ClockIn, te.ClockOut, te.TotalHours, te.Status, 
                               te.Location, te.Notes, ISNULL(te.BreakDuration, 0) as BreakDuration
                        FROM TimeEntries te
                        WHERE te.EmployeeId = @EmployeeId 
                          AND te.ClockIn >= @StartDate
                        ORDER BY te.ClockIn DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        cmd.Parameters.AddWithValue("@StartDate", startDate);

                        DataTable dt = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }

                        if (dt.Rows.Count > 0)
                        {
                            rptRecentEntries.DataSource = dt;
                            rptRecentEntries.DataBind();
                            pnlNoEntries.Visible = false;
                        }
                        else
                        {
                            rptRecentEntries.DataSource = null;
                            rptRecentEntries.DataBind();
                            pnlNoEntries.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading recent entries.", "error");
            }
        }

        #endregion

        #region Time Tracking Methods

        /// <summary>
        /// CORRECTED GetCurrentEmployeeId method
        /// This will return EmployeeId 26 for UserId 33 (Alice Johnson)
        /// </summary>
        private int GetCurrentEmployeeId()
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    return 0;
                }

                int userId = Convert.ToInt32(Session["UserId"]); // This is 33

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Query the Employees table to get the Employee.Id where Employee.UserId = session UserId
                    using (SqlCommand cmd = new SqlCommand(@"
                SELECT Id 
                FROM Employees 
                WHERE UserId = @UserId AND Status = 'Active'", conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            int employeeId = Convert.ToInt32(result);
                            System.Diagnostics.Debug.WriteLine($"GetCurrentEmployeeId: UserId {userId} -> EmployeeId {employeeId}");
                            return employeeId; // This should return 26 for UserId 33
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"GetCurrentEmployeeId: No employee found for UserId {userId}");
                            return 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetCurrentEmployeeId: Error - {ex.Message}");
                LogError(ex);
                return 0;
            }
        }

        /// <summary>
        /// Updated ClockIn method (no changes needed, just for reference)
        /// This should now work correctly with EmployeeId 26
        /// </summary>
        private int ClockIn()
        {
            try
            {
                int employeeId = GetCurrentEmployeeId(); // Should return 26, not 33
                if (employeeId <= 0)
                {
                    ShowMessage("Unable to identify employee. Please contact support.", "error");
                    return 0;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO TimeEntries (EmployeeId, ClockIn, Status, Location, CreatedAt)
                OUTPUT INSERTED.Id
                VALUES (@EmployeeId, @ClockIn, 'Active', @Location, @CreatedAt)", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId); // Now uses 26 instead of 33
                        cmd.Parameters.AddWithValue("@ClockIn", DateTime.Now);
                        cmd.Parameters.AddWithValue("@Location", GetUserLocation());
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error clocking in. Please try again.", "error");
                return 0;
            }
        }
        private bool ClockOut(int entryId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Get entry details for calculation
                        DateTime clockIn;
                        int breakDuration = 0;

                        using (SqlCommand getCmd = new SqlCommand(@"
                            SELECT ClockIn, ISNULL(BreakDuration, 0) as BreakDuration
                            FROM TimeEntries 
                            WHERE Id = @EntryId AND EmployeeId = @EmployeeId", conn, transaction))
                        {
                            getCmd.Parameters.AddWithValue("@EntryId", entryId);
                            getCmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                            using (SqlDataReader reader = getCmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    transaction.Rollback();
                                    return false;
                                }
                                clockIn = (DateTime)reader["ClockIn"];
                                breakDuration = (int)reader["BreakDuration"];
                            }
                        }

                        DateTime clockOut = DateTime.Now;
                        TimeSpan totalTime = clockOut - clockIn;
                        decimal totalHours = (decimal)(totalTime.TotalHours - (breakDuration / 60.0));

                        // Ensure total hours is not negative
                        if (totalHours < 0) totalHours = 0;

                        // Update the time entry
                        using (SqlCommand cmd = new SqlCommand(@"
                            UPDATE TimeEntries 
                            SET ClockOut = @ClockOut, 
                                TotalHours = @TotalHours, 
                                Status = 'Completed'
                            WHERE Id = @EntryId AND EmployeeId = @EmployeeId", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ClockOut", clockOut);
                            cmd.Parameters.AddWithValue("@TotalHours", totalHours);
                            cmd.Parameters.AddWithValue("@EntryId", entryId);
                            cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        private bool StartBreak(int entryId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO BreakEntries (TimeEntryId, BreakStart, CreatedAt)
                    VALUES (@TimeEntryId, @BreakStart, @CreatedAt)", conn))
                {
                    cmd.Parameters.AddWithValue("@TimeEntryId", entryId);
                    cmd.Parameters.AddWithValue("@BreakStart", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private bool EndBreak(int entryId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Get the active break
                        DateTime breakStart;
                        int breakEntryId;

                        using (SqlCommand getBreakCmd = new SqlCommand(@"
                            SELECT Id, BreakStart 
                            FROM BreakEntries 
                            WHERE TimeEntryId = @TimeEntryId AND BreakEnd IS NULL", conn, transaction))
                        {
                            getBreakCmd.Parameters.AddWithValue("@TimeEntryId", entryId);
                            using (SqlDataReader reader = getBreakCmd.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    transaction.Rollback();
                                    return false;
                                }
                                breakEntryId = (int)reader["Id"];
                                breakStart = (DateTime)reader["BreakStart"];
                            }
                        }

                        DateTime breakEnd = DateTime.Now;
                        int breakDurationMinutes = (int)(breakEnd - breakStart).TotalMinutes;

                        // Update break entry
                        using (SqlCommand updateBreakCmd = new SqlCommand(@"
                            UPDATE BreakEntries 
                            SET BreakEnd = @BreakEnd, Duration = @Duration
                            WHERE Id = @BreakEntryId", conn, transaction))
                        {
                            updateBreakCmd.Parameters.AddWithValue("@BreakEnd", breakEnd);
                            updateBreakCmd.Parameters.AddWithValue("@Duration", breakDurationMinutes);
                            updateBreakCmd.Parameters.AddWithValue("@BreakEntryId", breakEntryId);
                            updateBreakCmd.ExecuteNonQuery();
                        }

                        // Update total break duration in time entry
                        using (SqlCommand updateTimeCmd = new SqlCommand(@"
                            UPDATE TimeEntries 
                            SET BreakDuration = ISNULL(BreakDuration, 0) + @BreakDuration
                            WHERE Id = @TimeEntryId", conn, transaction))
                        {
                            updateTimeCmd.Parameters.AddWithValue("@BreakDuration", breakDurationMinutes);
                            updateTimeCmd.Parameters.AddWithValue("@TimeEntryId", entryId);
                            updateTimeCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        private bool AddManualEntry()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        DateTime entryDate = DateTime.Parse(txtEntryDate.Text);
                        DateTime startTime = DateTime.Parse($"{entryDate:yyyy-MM-dd} {txtStartTime.Text}");
                        DateTime endTime = DateTime.Parse($"{entryDate:yyyy-MM-dd} {txtEndTime.Text}");
                        int breakDuration = int.TryParse(txtBreakDuration.Text, out int breakMins) ? breakMins : 0;

                        TimeSpan workTime = endTime - startTime;
                        decimal totalHours = (decimal)(workTime.TotalHours - (breakDuration / 60.0));

                        // Ensure total hours is not negative
                        if (totalHours < 0) totalHours = 0;

                        using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO TimeEntries (EmployeeId, ClockIn, ClockOut, TotalHours, 
                                                    BreakDuration, Status, Location, Notes, CreatedAt, IsManualEntry)
                            VALUES (@EmployeeId, @ClockIn, @ClockOut, @TotalHours, 
                                   @BreakDuration, 'Completed', @Location, @Notes, @CreatedAt, 1)", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                            cmd.Parameters.AddWithValue("@ClockIn", startTime);
                            cmd.Parameters.AddWithValue("@ClockOut", endTime);
                            cmd.Parameters.AddWithValue("@TotalHours", totalHours);
                            cmd.Parameters.AddWithValue("@BreakDuration", breakDuration);
                            cmd.Parameters.AddWithValue("@Location", string.IsNullOrEmpty(txtLocation.Text) ?
                                DBNull.Value : (object)txtLocation.Text);
                            cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtEntryNotes.Text) ?
                                DBNull.Value : (object)txtEntryNotes.Text);
                            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                            int result = cmd.ExecuteNonQuery();
                            if (result > 0)
                            {
                                transaction.Commit();
                                return true;
                            }
                            else
                            {
                                transaction.Rollback();
                                return false;
                            }
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        private bool HasActiveEntry()
        {
            return GetActiveEntry() != null;
        }

        private dynamic GetActiveEntry()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT Id, ClockIn, EmployeeId
                        FROM TimeEntries 
                        WHERE EmployeeId = @EmployeeId 
                          AND Status = 'Active' 
                          AND ClockOut IS NULL", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new
                                {
                                    Id = (int)reader["Id"],
                                    ClockIn = (DateTime)reader["ClockIn"],
                                    EmployeeId = (int)reader["EmployeeId"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return null;
        }

        private dynamic GetLastTimeEntry()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT TOP 1 Id, ClockIn, ClockOut, Status
                        FROM TimeEntries 
                        WHERE EmployeeId = @EmployeeId 
                        ORDER BY ClockIn DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new
                                {
                                    Id = (int)reader["Id"],
                                    ClockIn = (DateTime)reader["ClockIn"],
                                    ClockOut = reader["ClockOut"] as DateTime?,
                                    Status = reader["Status"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return null;
        }

        private bool IsOnBreak()
        {
            try
            {
                var activeEntry = GetActiveEntry();
                if (activeEntry == null) return false;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM BreakEntries 
                        WHERE TimeEntryId = @TimeEntryId AND BreakEnd IS NULL", conn))
                    {
                        cmd.Parameters.AddWithValue("@TimeEntryId", activeEntry.Id);
                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }
        }

        private DateTime? GetLastBreakStart()
        {
            try
            {
                var activeEntry = GetActiveEntry();
                if (activeEntry == null) return null;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT TOP 1 BreakStart 
                        FROM BreakEntries 
                        WHERE TimeEntryId = @TimeEntryId AND BreakEnd IS NULL
                        ORDER BY BreakStart DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@TimeEntryId", activeEntry.Id);
                        object result = cmd.ExecuteScalar();
                        return result as DateTime?;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return null;
            }
        }

        private decimal GetTodaysHours(SqlConnection conn)
        {
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT ISNULL(SUM(
                    CASE 
                        WHEN ClockOut IS NOT NULL THEN TotalHours
                        ELSE DATEDIFF(MINUTE, ClockIn, GETDATE()) / 60.0 - ISNULL(BreakDuration, 0) / 60.0
                    END
                ), 0)
                FROM TimeEntries 
                WHERE EmployeeId = @EmployeeId 
                  AND CAST(ClockIn AS DATE) = CAST(GETDATE() AS DATE)", conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToDecimal(result) : 0m;
            }
        }

        private decimal GetWeekHours(SqlConnection conn)
        {
            DateTime weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);

            using (SqlCommand cmd = new SqlCommand(@"
                SELECT ISNULL(SUM(
                    CASE 
                        WHEN ClockOut IS NOT NULL THEN TotalHours
                        ELSE DATEDIFF(MINUTE, ClockIn, GETDATE()) / 60.0 - ISNULL(BreakDuration, 0) / 60.0
                    END
                ), 0)
                FROM TimeEntries 
                WHERE EmployeeId = @EmployeeId 
                  AND ClockIn >= @WeekStart", conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                cmd.Parameters.AddWithValue("@WeekStart", weekStart);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToDecimal(result) : 0m;
            }
        }

        private void UpdateClockButtons()
        {
            bool hasActiveEntry = HasActiveEntry();
            bool onBreak = IsOnBreak();

            btnClockIn.Enabled = !hasActiveEntry;
            btnClockOut.Enabled = hasActiveEntry;
            btnStartBreak.CssClass = (hasActiveEntry && !onBreak) ?
                "btn btn-warning btn-clock-action" :
                "btn btn-warning btn-clock-action disabled";

            btnEndBreak.CssClass = (hasActiveEntry && onBreak) ?
                "btn btn-info btn-clock-action" :
                "btn btn-info btn-clock-action disabled";
        }

        private bool ValidateManualEntry()
        {
            if (string.IsNullOrEmpty(txtEntryDate.Text))
            {
                ShowMessage("Please select a date for the time entry.", "error");
                return false;
            }

            if (string.IsNullOrEmpty(txtStartTime.Text))
            {
                ShowMessage("Please enter a start time.", "error");
                return false;
            }

            if (string.IsNullOrEmpty(txtEndTime.Text))
            {
                ShowMessage("Please enter an end time.", "error");
                return false;
            }

            DateTime entryDate = DateTime.Parse(txtEntryDate.Text);
            DateTime startTime = DateTime.Parse($"{entryDate:yyyy-MM-dd} {txtStartTime.Text}");
            DateTime endTime = DateTime.Parse($"{entryDate:yyyy-MM-dd} {txtEndTime.Text}");

            if (endTime <= startTime)
            {
                ShowMessage("End time must be after start time.", "error");
                return false;
            }

            if (entryDate > DateTime.Today)
            {
                ShowMessage("Cannot add time entries for future dates.", "error");
                return false;
            }

            // Check for overlapping entries
            if (HasOverlappingEntry(startTime, endTime))
            {
                ShowMessage("This time entry overlaps with an existing entry.", "error");
                return false;
            }

            // Validate break duration
            if (int.TryParse(txtBreakDuration.Text, out int breakDuration))
            {
                TimeSpan workTime = endTime - startTime;
                if (breakDuration >= workTime.TotalMinutes)
                {
                    ShowMessage("Break duration cannot be equal to or greater than total work time.", "error");
                    return false;
                }
            }

            return true;
        }

        private bool HasOverlappingEntry(DateTime startTime, DateTime endTime)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT COUNT(*) 
                        FROM TimeEntries 
                        WHERE EmployeeId = @EmployeeId 
                          AND ((ClockIn <= @StartTime AND ClockOut > @StartTime) 
                            OR (ClockIn < @EndTime AND ClockOut >= @EndTime)
                            OR (ClockIn >= @StartTime AND ClockOut <= @EndTime))", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        cmd.Parameters.AddWithValue("@StartTime", startTime);
                        cmd.Parameters.AddWithValue("@EndTime", endTime);

                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return false;
            }
        }

        private void ClearManualEntryForm()
        {
            txtEntryDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtStartTime.Text = "";
            txtEndTime.Text = "";
            txtBreakDuration.Text = "0";
            txtLocation.Text = "";
            txtEntryNotes.Text = "";
        }

        private void SetActiveSessionData()
        {
            var activeEntry = GetActiveEntry();
            if (activeEntry != null)
            {
                hfActiveEntryId.Value = activeEntry.Id.ToString();
                hfClockInTime.Value = activeEntry.ClockIn.ToString("yyyy-MM-ddTHH:mm:ss");
                hfIsOnBreak.Value = IsOnBreak().ToString().ToLower();

                if (IsOnBreak())
                {
                    var breakStart = GetLastBreakStart();
                    if (breakStart.HasValue)
                    {
                        hfBreakStartTime.Value = breakStart.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                    }
                }
            }
            else
            {
                ClearActiveSessionData();
            }
        }

        private void ClearActiveSessionData()
        {
            hfActiveEntryId.Value = "";
            hfClockInTime.Value = "";
            hfIsOnBreak.Value = "false";
            hfBreakStartTime.Value = "";
        }

        private void SetBreakData()
        {
            hfIsOnBreak.Value = "true";
            var breakStart = GetLastBreakStart();
            if (breakStart.HasValue)
            {
                hfBreakStartTime.Value = breakStart.Value.ToString("yyyy-MM-ddTHH:mm:ss");
            }
        }

        private void ClearBreakData()
        {
            hfIsOnBreak.Value = "false";
            hfBreakStartTime.Value = "";
        }

        private string GetUserLocation()
        {
            // This could be enhanced to capture actual location from client-side geolocation
            // For now, return a default location or get from form if available
            string location = "Office";

            // Check if there's a location preference in session or user profile
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT DefaultLocation FROM Employees 
                        WHERE Id = @EmployeeId", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        object result = cmd.ExecuteScalar();
                        if (result != null && !string.IsNullOrEmpty(result.ToString()))
                        {
                            location = result.ToString();
                        }
                    }
                }
            }
            catch
            {
                // Fall back to default if query fails
            }

            return location;
        }

        private void ValidateUserAccess()
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Additional role-based validation could be added here
            // Check if user has permission to access time tracking
            try
            {
                string userRole = Session["UserRole"]?.ToString();
                if (!string.IsNullOrEmpty(userRole))
                {
                    var allowedRoles = new[] { "EMPLOYEE", "SUPERVISOR", "HRADMIN", "ADMIN" };
                    if (!Array.Exists(allowedRoles, role => role.Equals(userRole, StringComparison.OrdinalIgnoreCase)))
                    {
                        Response.Redirect("~/Unauthorized.aspx");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                // Continue with basic validation
            }
        }

      

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            litMessage.Text = message;

            // Remove existing CSS classes and add the appropriate one
            pnlMessage.CssClass = pnlMessage.CssClass.Replace("alert-success", "")
                                                   .Replace("alert-error", "")
                                                   .Replace("alert-warning", "")
                                                   .Replace("alert-info", "")
                                                   .Trim();

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
        }

        private void LogError(Exception ex)
        {
            // Enhanced error logging
            try
            {
                string errorMessage = $"Error in EmployeeTimeTracking: {ex.Message}\n" +
                                    $"Stack Trace: {ex.StackTrace}\n" +
                                    $"User: {Session["UserId"]}\n" +
                                    $"Employee: {CurrentEmployeeId}\n" +
                                    $"Time: {DateTime.Now}";

                System.Diagnostics.Debug.WriteLine(errorMessage);

                // Log to database if ErrorLogs table exists
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if ErrorLogs table exists
                    using (SqlCommand checkCmd = new SqlCommand(@"
                        SELECT COUNT(*) FROM sys.tables WHERE name = 'ErrorLogs'", conn))
                    {
                        int tableExists = (int)checkCmd.ExecuteScalar();

                        if (tableExists > 0)
                        {
                            using (SqlCommand cmd = new SqlCommand(@"
                                INSERT INTO ErrorLogs (ErrorMessage, StackTrace, UserId, EmployeeId, PageUrl, CreatedAt)
                                VALUES (@ErrorMessage, @StackTrace, @UserId, @EmployeeId, @PageUrl, @CreatedAt)", conn))
                            {
                                cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message);
                                cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                                cmd.Parameters.AddWithValue("@UserId", Session["UserId"] ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId > 0 ? (object)CurrentEmployeeId : DBNull.Value);
                                cmd.Parameters.AddWithValue("@PageUrl", Request.Url?.ToString() ?? "");
                                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch
            {
                // If logging fails, at least write to debug
                System.Diagnostics.Debug.WriteLine($"Failed to log error: {ex.Message}");
            }
        }

        #endregion

        #region Public Helper Methods for ASPX

        /// <summary>
        /// Helper method for ASPX to get entry status CSS class
        /// </summary>
        protected string GetEntryStatusClass(string status)
        {
            switch (status?.ToLower())
            {
                case "active":
                    return "status-active";
                case "completed":
                    return "status-completed";
                case "pending":
                    return "status-pending";
                default:
                    return "status-unknown";
            }
        }

        /// <summary>
        /// Helper method to format time duration
        /// </summary>
        protected string FormatDuration(object minutes)
        {
            if (minutes == null || minutes == DBNull.Value)
                return "0 min";

            int mins = Convert.ToInt32(minutes);
            if (mins < 60)
                return $"{mins} min";

            int hours = mins / 60;
            int remainingMins = mins % 60;

            if (remainingMins == 0)
                return $"{hours}h";

            return $"{hours}h {remainingMins}m";
        }

        /// <summary>
        /// Helper method to check if entry can be edited
        /// </summary>
        protected bool CanEditEntry(object status, object clockIn)
        {
            if (status == null || clockIn == null)
                return false;

            string entryStatus = status.ToString();
            DateTime entryDate = Convert.ToDateTime(clockIn);

            // Can edit if status is Active or if entry is from today and completed
            return entryStatus == "Active" ||
                   (entryStatus == "Completed" && entryDate.Date == DateTime.Today);
        }

        /// <summary>
        /// Helper method to format hours for display
        /// </summary>
        protected string FormatHours(object hours)
        {
            if (hours == null || hours == DBNull.Value)
                return "0.0";

            decimal h = Convert.ToDecimal(hours);
            return h.ToString("F1");
        }

        /// <summary>
        /// Helper method to get display text for location
        /// </summary>
        protected string GetLocationDisplay(object location)
        {
            if (location == null || location == DBNull.Value || string.IsNullOrEmpty(location.ToString()))
                return "Not specified";

            return location.ToString();
        }

        /// <summary>
        /// Helper method to truncate long notes for display
        /// </summary>
        protected string TruncateNotes(object notes, int maxLength = 50)
        {
            if (notes == null || notes == DBNull.Value || string.IsNullOrEmpty(notes.ToString()))
                return "";

            string noteText = notes.ToString();
            if (noteText.Length <= maxLength)
                return noteText;

            return noteText.Substring(0, maxLength) + "...";
        }

        #endregion

        #region Database Schema Requirements

        

        #endregion
    }
}
