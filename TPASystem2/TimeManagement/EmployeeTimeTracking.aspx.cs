using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

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
                LoadCurrentStatus();
                LoadRecentEntries();
                UpdateClockButtons();
            }
        }

        #endregion

        #region Button Events

        protected void btnClockIn_Click(object sender, EventArgs e)
        {
            try
            {
                if (HasActiveEntry())
                {
                    ShowMessage("You are already clocked in. Please clock out first.", "error");
                    return;
                }

                ClockIn();
                ShowMessage("Successfully clocked in!", "success");
                LoadCurrentStatus();
                LoadRecentEntries();
                UpdateClockButtons();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clocking in: {ex.Message}");
                ShowMessage("An error occurred while clocking in. Please try again.", "error");
            }
        }

        protected void btnClockOut_Click(object sender, EventArgs e)
        {
            try
            {
                var activeEntry = GetActiveEntry();
                if (activeEntry == null)
                {
                    ShowMessage("No active time entry found to clock out.", "error");
                    return;
                }

                ClockOut(activeEntry.Id);
                ShowMessage("Successfully clocked out!", "success");
                LoadCurrentStatus();
                LoadRecentEntries();
                UpdateClockButtons();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clocking out: {ex.Message}");
                ShowMessage("An error occurred while clocking out. Please try again.", "error");
            }
        }

        protected void btnStartBreak_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implement break functionality
                ShowMessage("Break functionality will be implemented soon.", "info");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error starting break: {ex.Message}");
                ShowMessage("An error occurred while starting break.", "error");
            }
        }

        protected void btnEndBreak_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implement break functionality
                ShowMessage("Break functionality will be implemented soon.", "info");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error ending break: {ex.Message}");
                ShowMessage("An error occurred while ending break.", "error");
            }
        }

        protected void btnAddEntry_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateManualEntry())
                {
                    AddManualEntry();
                    ShowMessage("Manual time entry added successfully!", "success");
                    ClearManualEntryForm();
                    LoadCurrentStatus();
                    LoadRecentEntries();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding manual entry: {ex.Message}");
                ShowMessage("An error occurred while adding the time entry.", "error");
            }
        }

        protected void btnViewTimesheets_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
        }

        protected void btnViewAllEntries_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
        }

        protected void btnHiddenRefresh_Click(object sender, EventArgs e)
        {
            LoadCurrentStatus();
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
                    }
                }
                else
                {
                    ShowMessage("Invalid entry ID.", "error");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in EntryAction_Command: {ex.Message}");
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
                        SELECT e.FirstName, e.LastName, e.EmployeeNumber
                        FROM Employees e
                        WHERE e.Id = @EmployeeId", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litEmployeeName.Text = $"{reader["FirstName"]} {reader["LastName"]}";
                                litEmployeeNumber.Text = reader["EmployeeNumber"].ToString();
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

        private void LoadCurrentStatus()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if currently clocked in
                    var activeEntry = GetActiveEntry();
                    if (activeEntry != null)
                    {
                        litCurrentStatus.Text = "Clocked In";
                        litLastAction.Text = $"Clocked in at {activeEntry.ClockIn.ToString("h:mm tt")}";
                    }
                    else
                    {
                        litCurrentStatus.Text = "Clocked Out";

                        // Get last completed entry
                        using (SqlCommand cmd = new SqlCommand(@"
                            SELECT TOP 1 ClockOut 
                            FROM TimeEntries 
                            WHERE EmployeeId = @EmployeeId AND ClockOut IS NOT NULL
                            ORDER BY ClockOut DESC", conn))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                            var lastClockOut = cmd.ExecuteScalar();

                            if (lastClockOut != null)
                            {
                                litLastAction.Text = $"Clocked out at {Convert.ToDateTime(lastClockOut).ToString("h:mm tt")}";
                            }
                            else
                            {
                                litLastAction.Text = "No recent activity";
                            }
                        }
                    }

                    // Calculate today's hours
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT ISNULL(SUM(TotalHours), 0) as TodaysHours
                        FROM TimeEntries 
                        WHERE EmployeeId = @EmployeeId 
                          AND CAST(ClockIn AS DATE) = CAST(GETDATE() AS DATE)
                          AND TotalHours IS NOT NULL", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        var todaysHours = cmd.ExecuteScalar();
                        litTodaysHours.Text = Convert.ToDecimal(todaysHours ?? 0).ToString("F1");
                    }

                    // Calculate week total
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT ISNULL(SUM(TotalHours), 0) as WeekHours
                        FROM TimeEntries 
                        WHERE EmployeeId = @EmployeeId 
                          AND ClockIn >= DATEADD(week, DATEDIFF(week, 0, GETDATE()), 0)
                          AND TotalHours IS NOT NULL", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        var weekHours = cmd.ExecuteScalar();
                        litWeekTotal.Text = Convert.ToDecimal(weekHours ?? 0).ToString("F1");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading current status: {ex.Message}");
            }
        }

        private void LoadRecentEntries()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT TOP 5 Id, ClockIn, ClockOut, TotalHours, Status, Location
                        FROM TimeEntries 
                        WHERE EmployeeId = @EmployeeId
                        ORDER BY ClockIn DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            rptRecentEntries.DataSource = dt;
                            rptRecentEntries.DataBind();
                            pnlNoEntries.Visible = false;
                        }
                        else
                        {
                            pnlNoEntries.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading recent entries: {ex.Message}");
                pnlNoEntries.Visible = true;
            }
        }

        #endregion

        #region Time Clock Operations

        private void ClockIn()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO TimeEntries (EmployeeId, ClockIn, Status, Location)
                    VALUES (@EmployeeId, GETDATE(), 'Active', @Location)", conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                    cmd.Parameters.AddWithValue("@Location", "Web Portal");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void ClockOut(int entryId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE TimeEntries 
                    SET ClockOut = GETDATE(),
                        TotalHours = DATEDIFF(MINUTE, ClockIn, GETDATE()) / 60.0,
                        Status = 'Completed'
                    WHERE Id = @EntryId AND EmployeeId = @EmployeeId", conn))
                {
                    cmd.Parameters.AddWithValue("@EntryId", entryId);
                    cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void AddManualEntry()
        {
            DateTime entryDate = Convert.ToDateTime(txtEntryDate.Text);
            TimeSpan startTime = TimeSpan.Parse(txtStartTime.Text);
            TimeSpan endTime = TimeSpan.Parse(txtEndTime.Text);

            DateTime clockIn = entryDate.Date.Add(startTime);
            DateTime clockOut = entryDate.Date.Add(endTime);

            // Handle next day clock out
            if (endTime < startTime)
            {
                clockOut = clockOut.AddDays(1);
            }

            decimal totalHours = (decimal)(clockOut - clockIn).TotalHours;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO TimeEntries (EmployeeId, ClockIn, ClockOut, TotalHours, Status, Location, Notes)
                    VALUES (@EmployeeId, @ClockIn, @ClockOut, @TotalHours, 'Pending', 'Manual Entry', @Notes)", conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                    cmd.Parameters.AddWithValue("@ClockIn", clockIn);
                    cmd.Parameters.AddWithValue("@ClockOut", clockOut);
                    cmd.Parameters.AddWithValue("@TotalHours", totalHours);
                    cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtEntryNotes.Text) ? DBNull.Value : (object)txtEntryNotes.Text);
                    cmd.ExecuteNonQuery();
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
                        SELECT Id, ClockIn 
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
                                    ClockIn = (DateTime)reader["ClockIn"]
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting active entry: {ex.Message}");
            }
            return null;
        }

        private void UpdateClockButtons()
        {
            bool hasActiveEntry = HasActiveEntry();

            btnClockIn.Enabled = !hasActiveEntry;
            btnClockOut.Enabled = hasActiveEntry;
            btnStartBreak.Enabled = hasActiveEntry;
            btnEndBreak.Enabled = false; // TODO: Implement break logic

            if (hasActiveEntry)
            {
                btnClockIn.CssClass = "btn btn-success btn-clock-action disabled";
                btnClockOut.CssClass = "btn btn-danger btn-clock-action";
            }
            else
            {
                btnClockIn.CssClass = "btn btn-success btn-clock-action";
                btnClockOut.CssClass = "btn btn-danger btn-clock-action disabled";
            }
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

            DateTime entryDate = Convert.ToDateTime(txtEntryDate.Text);
            if (entryDate > DateTime.Now.Date)
            {
                ShowMessage("Cannot create time entries for future dates.", "error");
                return false;
            }

            return true;
        }

        private void ClearManualEntryForm()
        {
            txtEntryDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtStartTime.Text = "";
            txtEndTime.Text = "";
            txtEntryNotes.Text = "";
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

        #endregion
    }
}