using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace TPASystem2.TimeManagement
{
    public partial class ViewTimesheet : System.Web.UI.Page
    {
        #region Properties and Fields

        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int CurrentEmployeeId => GetCurrentEmployeeId();

        private int TimesheetId
        {
            get
            {
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out int id))
                {
                    return id;
                }
                return 0;
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                CheckUserAccess();
                LoadTimesheetData();
            }
        }

        #endregion

        #region Initialization Methods

        private void CheckUserAccess()
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (TimesheetId <= 0)
            {
                ShowMessage("Invalid timesheet ID.", "error");
                Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
                return;
            }
        }

        private int GetCurrentEmployeeId()
        {
            if (Session["UserId"] != null)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "SELECT Id FROM Employees WHERE UserId = @UserId AND IsActive = 1";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@UserId", Session["UserId"]);
                            object result = cmd.ExecuteScalar();
                            return result != null ? Convert.ToInt32(result) : 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error getting current employee ID: {ex.Message}");
                    return 0;
                }
            }
            return 0;
        }

        #endregion

        #region Data Loading Methods

        private void LoadTimesheetData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load timesheet details with employee information
                    string query = @"
                        SELECT 
                            ts.Id,
                            ts.EmployeeId,
                            ts.WeekStartDate,
                            ts.WeekEndDate,
                            ts.TotalHours,
                            ts.RegularHours,
                            ts.OvertimeHours,
                            ts.Status,
                            ts.SubmittedAt,
                            ts.ApprovedById,
                            ts.ApprovedAt,
                            ts.Notes,
                            e.FirstName + ' ' + e.LastName AS EmployeeName,
                            approver.FirstName + ' ' + approver.LastName AS ApprovedByName
                        FROM TimeSheets ts
                        INNER JOIN Employees e ON ts.EmployeeId = e.Id
                        LEFT JOIN Employees approver ON ts.ApprovedById = approver.Id
                        WHERE ts.Id = @TimesheetId";

                    // Check if user has access to this timesheet
                    string userRole = Session["UserRole"]?.ToString();
                    if (userRole != "Admin" && userRole != "HR Admin" && userRole != "HR Manager")
                    {
                        query += " AND ts.EmployeeId = @CurrentEmployeeId";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                        if (userRole != "Admin" && userRole != "HR Admin" && userRole != "HR Manager")
                        {
                            cmd.Parameters.AddWithValue("@CurrentEmployeeId", CurrentEmployeeId);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                LoadTimesheetHeader(reader);
                                LoadTimesheetSummary(reader);
                            }
                            else
                            {
                                ShowMessage("Timesheet not found or you don't have permission to view it.", "error");
                                Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
                                return;
                            }
                        }
                    }

                    // Load daily time entries
                    LoadDailyTimeEntries(conn);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading timesheet data: {ex.Message}");
                ShowMessage($"Error loading timesheet: {ex.Message}", "error");
            }
        }

        private void LoadTimesheetHeader(SqlDataReader reader)
        {
            // Employee and period information
            litEmployeeName.Text = reader["EmployeeName"].ToString();

            DateTime startDate = Convert.ToDateTime(reader["WeekStartDate"]);
            DateTime endDate = Convert.ToDateTime(reader["WeekEndDate"]);
            litWeekPeriod.Text = $"{startDate:MMM dd} - {endDate:MMM dd, yyyy}";

            // Status
            string status = reader["Status"].ToString();
            litTimesheetStatus.Text = status;

            // Set status badge CSS class
            string statusClass;
            switch (status.ToLower())
            {
                case "draft":
                    statusClass = "status-draft";
                    break;
                case "submitted":
                    statusClass = "status-submitted";
                    break;
                case "approved":
                    statusClass = "status-approved";
                    break;
                case "rejected":
                    statusClass = "status-rejected";
                    break;
                default:
                    statusClass = "status-draft";
                    break;
            }
            statusBadge.Attributes["class"] = $"status-badge {statusClass}";

            // Show/hide edit button based on status and permissions
            bool canEdit = status == "Draft" || status == "Rejected";
            string userRole = Session["UserRole"]?.ToString();
            if (userRole == "Admin" || userRole == "HR Admin" || userRole == "HR Manager")
            {
                canEdit = true; // Admins can always edit
            }

            btnEditTimesheet.Visible = canEdit;
            btnEditTimesheetBottom.Visible = canEdit;

            // Approval information
            if (reader["SubmittedAt"] != DBNull.Value)
            {
                approvalInfoSection.Visible = true;
                litSubmittedAt.Text = Convert.ToDateTime(reader["SubmittedAt"]).ToString("MMM dd, yyyy hh:mm tt");

                if (reader["ApprovedById"] != DBNull.Value && reader["ApprovedAt"] != DBNull.Value)
                {
                    approvedBySection.Visible = true;
                    approvedAtSection.Visible = true;
                    litApprovedBy.Text = reader["ApprovedByName"].ToString();
                    litApprovedAt.Text = Convert.ToDateTime(reader["ApprovedAt"]).ToString("MMM dd, yyyy hh:mm tt");
                }
            }

            // Timesheet notes
            if (!string.IsNullOrEmpty(reader["Notes"].ToString()))
            {
                timesheetNotesSection.Visible = true;
                litTimesheetNotes.Text = reader["Notes"].ToString().Replace("\n", "<br/>");
            }
        }

        private void LoadTimesheetSummary(SqlDataReader reader)
        {
            decimal totalHours = reader["TotalHours"] != DBNull.Value ? Convert.ToDecimal(reader["TotalHours"]) : 0;
            decimal regularHours = reader["RegularHours"] != DBNull.Value ? Convert.ToDecimal(reader["RegularHours"]) : 0;
            decimal overtimeHours = reader["OvertimeHours"] != DBNull.Value ? Convert.ToDecimal(reader["OvertimeHours"]) : 0;

            litTotalHours.Text = totalHours.ToString("F1");
            litRegularHours.Text = regularHours.ToString("F1");
            litOvertimeHours.Text = overtimeHours.ToString("F1");

            // Calculate days worked (will be updated after loading daily entries)
            litDaysWorked.Text = "0";
        }

        private void LoadDailyTimeEntries(SqlConnection conn)
        {
            try
            {
                string query = @"
                    SELECT 
                        DayOfWeek,
                        StartTime,
                        EndTime,
                        BreakDuration,
                        Notes,
                        TotalHours
                    FROM TimesheetEntries 
                    WHERE TimesheetId = @TimesheetId
                    ORDER BY DayOfWeek";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int daysWorked = 0;
                        DateTime weekStart = GetWeekStartDate();

                        // Initialize all day dates
                        SetDayDates(weekStart);

                        while (reader.Read())
                        {
                            int dayOfWeek = Convert.ToInt32(reader["DayOfWeek"]);
                            string startTime = reader["StartTime"] != DBNull.Value ?
                                Convert.ToDateTime(reader["StartTime"]).ToString("HH:mm") : "--:--";
                            string endTime = reader["EndTime"] != DBNull.Value ?
                                Convert.ToDateTime(reader["EndTime"]).ToString("HH:mm") : "--:--";
                            int breakDuration = reader["BreakDuration"] != DBNull.Value ?
                                Convert.ToInt32(reader["BreakDuration"]) : 0;
                            string notes = reader["Notes"]?.ToString() ?? "";
                            decimal dayTotal = reader["TotalHours"] != DBNull.Value ?
                                Convert.ToDecimal(reader["TotalHours"]) : 0;

                            // Count days worked
                            if (dayTotal > 0)
                            {
                                daysWorked++;
                            }

                            // Set day-specific data
                            switch (dayOfWeek)
                            {
                                case 1: // Monday
                                    SetDayData(litMondayStart, litMondayEnd, litMondayBreak, litMondayNotes,
                                             litMondayTotal, mondayNotes, startTime, endTime, breakDuration, notes, dayTotal);
                                    break;
                                case 2: // Tuesday
                                    SetDayData(litTuesdayStart, litTuesdayEnd, litTuesdayBreak, litTuesdayNotes,
                                             litTuesdayTotal, tuesdayNotes, startTime, endTime, breakDuration, notes, dayTotal);
                                    break;
                                case 3: // Wednesday
                                    SetDayData(litWednesdayStart, litWednesdayEnd, litWednesdayBreak, litWednesdayNotes,
                                             litWednesdayTotal, wednesdayNotes, startTime, endTime, breakDuration, notes, dayTotal);
                                    break;
                                case 4: // Thursday
                                    SetDayData(litThursdayStart, litThursdayEnd, litThursdayBreak, litThursdayNotes,
                                             litThursdayTotal, thursdayNotes, startTime, endTime, breakDuration, notes, dayTotal);
                                    break;
                                case 5: // Friday
                                    SetDayData(litFridayStart, litFridayEnd, litFridayBreak, litFridayNotes,
                                             litFridayTotal, fridayNotes, startTime, endTime, breakDuration, notes, dayTotal);
                                    break;
                                case 6: // Saturday
                                    SetDayData(litSaturdayStart, litSaturdayEnd, litSaturdayBreak, litSaturdayNotes,
                                             litSaturdayTotal, saturdayNotes, startTime, endTime, breakDuration, notes, dayTotal);
                                    break;
                                case 0: // Sunday
                                    SetDayData(litSundayStart, litSundayEnd, litSundayBreak, litSundayNotes,
                                             litSundayTotal, sundayNotes, startTime, endTime, breakDuration, notes, dayTotal);
                                    break;
                            }
                        }

                        // Update days worked count
                        litDaysWorked.Text = daysWorked.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading daily time entries: {ex.Message}");
                ShowMessage("Error loading daily time entries.", "warning");
            }
        }

        private DateTime GetWeekStartDate()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT WeekStartDate FROM TimeSheets WHERE Id = @TimesheetId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToDateTime(result) : DateTime.Today;
                    }
                }
            }
            catch
            {
                return DateTime.Today;
            }
        }

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

        private void SetDayData(System.Web.UI.WebControls.Literal startLit, System.Web.UI.WebControls.Literal endLit,
                               System.Web.UI.WebControls.Literal breakLit, System.Web.UI.WebControls.Literal notesLit,
                               System.Web.UI.WebControls.Literal totalLit, System.Web.UI.HtmlControls.HtmlGenericControl notesDiv,
                               string startTime, string endTime, int breakDuration, string notes, decimal dayTotal)
        {
            startLit.Text = startTime;
            endLit.Text = endTime;
            breakLit.Text = breakDuration > 0 ? $"{breakDuration} min" : "0 min";
            totalLit.Text = dayTotal.ToString("F1") + "h";

            if (!string.IsNullOrEmpty(notes))
            {
                notesLit.Text = notes.Replace("\n", "<br/>");
                notesDiv.Visible = true;
            }
        }

        #endregion

        #region Event Handlers

        protected void btnBackToTimesheets_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
        }

        protected void btnEditTimesheet_Click(object sender, EventArgs e)
        {
            Response.Redirect($"~/TimeManagement/EditTimesheet.aspx?id={TimesheetId}");
        }

        #endregion

        #region Helper Methods

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            litMessage.Text = message;

            // Remove existing alert classes
            pnlMessage.CssClass = "alert-panel";

            // Add type-specific class
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
                default:
                    pnlMessage.CssClass += " alert-info";
                    break;
            }
        }

        #endregion
    }
}