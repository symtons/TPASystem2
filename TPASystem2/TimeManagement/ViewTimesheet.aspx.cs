using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Collections.Generic;

namespace TPASystem2.TimeManagement
{
    public partial class ViewTimesheet : System.Web.UI.Page
    {
        #region Properties and Fields

        private string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString; }
        }

        private int TimesheetId
        {
            get
            {
                if (ViewState["TimesheetId"] == null)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["id"]) && int.TryParse(Request.QueryString["id"], out int id))
                    {
                        ViewState["TimesheetId"] = id;
                    }
                    else
                    {
                        ViewState["TimesheetId"] = 0;
                    }
                }
                return (int)ViewState["TimesheetId"];
            }
        }

        private int CurrentEmployeeId
        {
            get
            {
                if (Session["EmployeeId"] != null && int.TryParse(Session["EmployeeId"].ToString(), out int empId))
                {
                    return empId;
                }
                return 0;
            }
        }

        // Store timesheet data for use in helper methods
        private Dictionary<string, object> timesheetData = new Dictionary<string, object>();
        private Dictionary<string, Dictionary<string, object>> dailyData = new Dictionary<string, Dictionary<string, object>>();

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (TimesheetId == 0)
                {
                    ShowMessage("Invalid timesheet ID.", "error");
                    Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
                    return;
                }

                LoadTimesheetData();
            }
        }

        #endregion

        #region Data Loading Methods

        private void LoadTimesheetData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    // Load timesheet basic information
                    LoadTimesheetInfo(conn);

                    // Load daily time entries
                    LoadDailyTimeEntries(conn);

                    // Calculate and display summary
                    CalculateAndDisplaySummary();

                    // Set approval information
                    SetApprovalInformation();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading timesheet data: {ex.Message}");
                ShowMessage("Error loading timesheet data. Please try again.", "error");
            }
        }

        private void LoadTimesheetInfo(SqlConnection conn)
        {
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT ts.Id, ts.EmployeeId, ts.WeekStartDate, ts.WeekEndDate, ts.TotalHours, 
                       ts.RegularHours, ts.OvertimeHours, ts.Status, ts.SubmittedAt, 
                       ts.ApprovedById, ts.ApprovedAt, ts.Notes,
                       e.FirstName, e.LastName, e.EmployeeNumber, e.Department, e.Position,
                       approver.FirstName as ApproverFirstName, approver.LastName as ApproverLastName
                FROM TimeSheets ts
                INNER JOIN Employees e ON ts.EmployeeId = e.Id
                LEFT JOIN Employees approver ON ts.ApprovedById = approver.Id
                WHERE ts.Id = @TimesheetId", conn))
            {
                cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Store data for use in helper methods
                        foreach (var field in new[] { "Id", "EmployeeId", "WeekStartDate", "WeekEndDate", "TotalHours",
                                                     "RegularHours", "OvertimeHours", "Status", "SubmittedAt",
                                                     "ApprovedById", "ApprovedAt", "Notes", "FirstName", "LastName",
                                                     "EmployeeNumber", "Department", "Position", "ApproverFirstName", "ApproverLastName" })
                        {
                            timesheetData[field] = reader[field] != DBNull.Value ? reader[field] : null;
                        }

                        // Set basic information
                        string employeeName = $"{reader["FirstName"]} {reader["LastName"]}";
                        litEmployeeName.Text = employeeName;
                        litEmployeeNumber.Text = reader["EmployeeNumber"]?.ToString() ?? "";
                        litDepartment.Text = reader["Department"]?.ToString() ?? "";
                        litPosition.Text = reader["Position"]?.ToString() ?? "";

                        // Set week information
                        DateTime weekStart = Convert.ToDateTime(reader["WeekStartDate"]);
                        DateTime weekEnd = Convert.ToDateTime(reader["WeekEndDate"]);
                        litWeekRange.Text = $"{weekStart:MMM dd} - {weekEnd:MMM dd, yyyy}";

                        // Set status
                        string status = reader["Status"]?.ToString() ?? "Draft";
                        litTimesheetStatus.Text = status;

                        // Set submission date
                        if (reader["SubmittedAt"] != DBNull.Value)
                        {
                            DateTime submittedAt = Convert.ToDateTime(reader["SubmittedAt"]);
                            litSubmissionDate.Text = $"Submitted: {submittedAt:MMM dd, yyyy}";
                        }
                        else
                        {
                            litSubmissionDate.Text = "Not submitted";
                        }

                        // Set date headers for each day
                        SetDayDates(weekStart);

                        // Set timesheet notes if any
                        if (!string.IsNullOrEmpty(reader["Notes"]?.ToString()))
                        {
                            litTimesheetNotes.Text = reader["Notes"].ToString().Replace("\n", "<br/>");
                            timesheetNotesSection.Visible = true;
                        }
                    }
                    else
                    {
                        ShowMessage("Timesheet not found.", "error");
                        Response.Redirect("~/TimeManagement/EmployeeTimesheets.aspx");
                    }
                }
            }
        }

        private void LoadDailyTimeEntries(SqlConnection conn)
        {
            // Initialize daily data
            var days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            foreach (var day in days)
            {
                dailyData[day] = new Dictionary<string, object>
                {
                    ["StartTime"] = null,
                    ["EndTime"] = null,
                    ["BreakDuration"] = 0,
                    ["TotalHours"] = 0m,
                    ["RegularHours"] = 0m,
                    ["OvertimeHours"] = 0m,
                    ["Notes"] = ""
                };
            }

            using (SqlCommand cmd = new SqlCommand(@"
                SELECT te.ClockIn, te.ClockOut, te.TotalHours, te.Notes,
                       DATEPART(WEEKDAY, te.ClockIn) as DayOfWeek,
                       DATEDIFF(MINUTE, te.ClockIn, te.ClockOut) as TotalMinutes
                FROM TimeEntries te
                INNER JOIN TimeSheets ts ON DATEPART(week, te.ClockIn) = DATEPART(week, ts.WeekStartDate)
                    AND YEAR(te.ClockIn) = YEAR(ts.WeekStartDate)
                WHERE ts.Id = @TimesheetId AND te.EmployeeId = @EmployeeId
                ORDER BY te.ClockIn", conn))
            {
                cmd.Parameters.AddWithValue("@TimesheetId", TimesheetId);
                cmd.Parameters.AddWithValue("@EmployeeId", timesheetData["EmployeeId"]);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime clockIn = Convert.ToDateTime(reader["ClockIn"]);
                        string startTime = clockIn.ToString("HH:mm");
                        string endTime = "--:--";
                        decimal totalHours = 0m;
                        int breakDuration = 0;

                        if (reader["ClockOut"] != DBNull.Value)
                        {
                            DateTime clockOut = Convert.ToDateTime(reader["ClockOut"]);
                            endTime = clockOut.ToString("HH:mm");

                            if (reader["TotalHours"] != DBNull.Value)
                            {
                                totalHours = Convert.ToDecimal(reader["TotalHours"]);
                            }
                            else
                            {
                                // Calculate total hours
                                double totalMinutes = (clockOut - clockIn).TotalMinutes;
                                totalHours = (decimal)(totalMinutes / 60.0);
                            }

                            // Calculate break duration (assume 30 min break for 8+ hour days)
                            if (totalHours >= 8)
                            {
                                breakDuration = 30;
                            }
                        }

                        string notes = reader["Notes"]?.ToString() ?? "";
                        int sqlDayOfWeek = Convert.ToInt32(reader["DayOfWeek"]);

                        // Map SQL DATEPART weekday to day name
                        string dayName = GetDayNameFromSqlWeekday(sqlDayOfWeek);

                        // Store daily data
                        if (dailyData.ContainsKey(dayName))
                        {
                            dailyData[dayName]["StartTime"] = startTime;
                            dailyData[dayName]["EndTime"] = endTime;
                            dailyData[dayName]["BreakDuration"] = breakDuration;
                            dailyData[dayName]["TotalHours"] = totalHours;
                            dailyData[dayName]["Notes"] = notes;

                            // Calculate regular and overtime hours
                            if (totalHours > 8)
                            {
                                dailyData[dayName]["RegularHours"] = 8m;
                                dailyData[dayName]["OvertimeHours"] = totalHours - 8m;
                            }
                            else
                            {
                                dailyData[dayName]["RegularHours"] = totalHours;
                                dailyData[dayName]["OvertimeHours"] = 0m;
                            }
                        }

                        // Set day data in UI
                        SetDayDataInUI(dayName, startTime, endTime, breakDuration, notes, totalHours);
                    }
                }
            }
        }

        private void SetDayDataInUI(string dayName, string startTime, string endTime, int breakDuration, string notes, decimal totalHours)
        {
            switch (dayName.ToLower())
            {
                case "monday":
                    SetDayData(litMondayStart, litMondayEnd, litMondayBreak, litMondayNotes, litMondayTotal, litMondayRegular, litMondayOvertime,
                              mondayNotesSection, startTime, endTime, breakDuration, notes, totalHours);
                    break;
                case "tuesday":
                    SetDayData(litTuesdayStart, litTuesdayEnd, litTuesdayBreak, litTuesdayNotes, litTuesdayTotal, litTuesdayRegular, litTuesdayOvertime,
                              tuesdayNotesSection, startTime, endTime, breakDuration, notes, totalHours);
                    break;
                case "wednesday":
                    SetDayData(litWednesdayStart, litWednesdayEnd, litWednesdayBreak, litWednesdayNotes, litWednesdayTotal, litWednesdayRegular, litWednesdayOvertime,
                              wednesdayNotesSection, startTime, endTime, breakDuration, notes, totalHours);
                    break;
                case "thursday":
                    SetDayData(litThursdayStart, litThursdayEnd, litThursdayBreak, litThursdayNotes, litThursdayTotal, litThursdayRegular, litThursdayOvertime,
                              thursdayNotesSection, startTime, endTime, breakDuration, notes, totalHours);
                    break;
                case "friday":
                    SetDayData(litFridayStart, litFridayEnd, litFridayBreak, litFridayNotes, litFridayTotal, litFridayRegular, litFridayOvertime,
                              fridayNotesSection, startTime, endTime, breakDuration, notes, totalHours);
                    break;
                case "saturday":
                    SetDayData(litSaturdayStart, litSaturdayEnd, litSaturdayBreak, litSaturdayNotes, litSaturdayTotal, litSaturdayRegular, litSaturdayOvertime,
                              saturdayNotesSection, startTime, endTime, breakDuration, notes, totalHours);
                    break;
                case "sunday":
                    SetDayData(litSundayStart, litSundayEnd, litSundayBreak, litSundayNotes, litSundayTotal, litSundayRegular, litSundayOvertime,
                              sundayNotesSection, startTime, endTime, breakDuration, notes, totalHours);
                    break;
            }
        }

        private void CalculateAndDisplaySummary()
        {
            decimal totalHours = 0m;
            decimal regularHours = 0m;
            decimal overtimeHours = 0m;
            int daysWorked = 0;

            foreach (var dayData in dailyData.Values)
            {
                decimal dayTotal = (decimal)dayData["TotalHours"];
                if (dayTotal > 0)
                {
                    daysWorked++;
                    totalHours += dayTotal;
                    regularHours += (decimal)dayData["RegularHours"];
                    overtimeHours += (decimal)dayData["OvertimeHours"];
                }
            }

            // Display summary
            litTotalHours.Text = totalHours.ToString("F1");
            litRegularHours.Text = regularHours.ToString("F1");
            litOvertimeHours.Text = overtimeHours.ToString("F1");
            litDaysWorked.Text = daysWorked.ToString();
        }

        private void SetApprovalInformation()
        {
            string status = timesheetData["Status"]?.ToString() ?? "Draft";

            if (status != "Draft")
            {
                approvalInfoSection.Visible = true;

                // Set submitted information
                if (timesheetData["SubmittedAt"] != null)
                {
                    DateTime submittedAt = Convert.ToDateTime(timesheetData["SubmittedAt"]);
                    litSubmittedAt.Text = submittedAt.ToString("MMM dd, yyyy 'at' hh:mm tt");
                }

                // Set approval information if approved
                if (status == "Approved" && timesheetData["ApprovedAt"] != null)
                {
                    approvedTimelineItem.Visible = true;

                    DateTime approvedAt = Convert.ToDateTime(timesheetData["ApprovedAt"]);
                    litApprovedAt.Text = approvedAt.ToString("MMM dd, yyyy 'at' hh:mm tt");
                    approvedAtSection.Visible = true;

                    if (timesheetData["ApproverFirstName"] != null && timesheetData["ApproverLastName"] != null)
                    {
                        string approverName = $"{timesheetData["ApproverFirstName"]} {timesheetData["ApproverLastName"]}";
                        litApprovedBy.Text = approverName;
                        approvedBySection.Visible = true;
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        private string GetDayNameFromSqlWeekday(int sqlWeekday)
        {
            // SQL DATEPART weekday: 1=Sunday, 2=Monday, 3=Tuesday, 4=Wednesday, 5=Thursday, 6=Friday, 7=Saturday
            switch (sqlWeekday)
            {
                case 1: return "Sunday";
                case 2: return "Monday";
                case 3: return "Tuesday";
                case 4: return "Wednesday";
                case 5: return "Thursday";
                case 6: return "Friday";
                case 7: return "Saturday";
                default: return "Monday";
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
                               System.Web.UI.WebControls.Literal totalLit, System.Web.UI.WebControls.Literal regularLit,
                               System.Web.UI.WebControls.Literal overtimeLit, System.Web.UI.HtmlControls.HtmlGenericControl notesDiv,
                               string startTime, string endTime, int breakDuration, string notes, decimal dayTotal)
        {
            startLit.Text = startTime;
            endLit.Text = endTime;
            breakLit.Text = breakDuration > 0 ? $"{breakDuration} min" : "0 min";
            totalLit.Text = dayTotal.ToString("F1") + "h";

            // Calculate regular and overtime
            decimal regular = dayTotal > 8 ? 8m : dayTotal;
            decimal overtime = dayTotal > 8 ? dayTotal - 8m : 0m;

            regularLit.Text = regular.ToString("F1") + "h";
            overtimeLit.Text = overtime.ToString("F1") + "h";

            if (!string.IsNullOrEmpty(notes))
            {
                notesLit.Text = notes.Replace("\n", "<br/>");
                notesDiv.Visible = true;
            }
        }

        // Helper methods for status display (used in ASPX inline code)
        protected string GetStatusClass()
        {
            string status = timesheetData.ContainsKey("Status") ? timesheetData["Status"]?.ToString() ?? "draft" : "draft";
            return status.ToLower();
        }

        protected string GetStatusIcon()
        {
            string status = timesheetData.ContainsKey("Status") ? timesheetData["Status"]?.ToString() ?? "Draft" : "Draft";
            switch (status.ToLower())
            {
                case "draft": return "edit";
                case "submitted": return "send";
                case "approved": return "check_circle";
                case "rejected": return "cancel";
                default: return "help";
            }
        }

        protected string GetDayStatus(string dayName)
        {
            if (dailyData.ContainsKey(dayName))
            {
                decimal totalHours = (decimal)dailyData[dayName]["TotalHours"];
                if (totalHours == 0)
                {
                    return "No Work";
                }
                else if (totalHours > 8)
                {
                    return "Overtime";
                }
                else
                {
                    return "Completed";
                }
            }
            return "No Work";
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