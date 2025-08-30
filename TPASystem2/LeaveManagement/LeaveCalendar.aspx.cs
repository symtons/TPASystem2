using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.LeaveManagement
{
    public partial class LeaveCalendar : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
                LoadCalendarData();
            }
        }

        private void InitializePage()
        {
            try
            {
                // Set current user info
                if (Session["UserName"] != null)
                    litCurrentUser.Text = Session["UserName"].ToString();

                if (Session["Department"] != null)
                    litDepartment.Text = Session["Department"].ToString();

                litCurrentDate.Text = DateTime.Today.ToString("MMM dd, yyyy");

                // Initialize calendar to current month
                DateTime currentDate = DateTime.Today;
                calLeave.VisibleDate = currentDate;
                calLeave.SelectedDate = currentDate;

                hfCurrentMonth.Value = currentDate.Month.ToString();
                hfCurrentYear.Value = currentDate.Year.ToString();

                UpdateMonthDisplay();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error initializing page.", "error");
            }
        }

        private void LoadCalendarData()
        {
            try
            {
                LoadCalendarStatistics();
                LoadLeaveLegend();
                calLeave.DataBind(); // This will trigger DayRender for each day
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading calendar data.", "error");
            }
        }

        #region Calendar Events

        protected void calLeave_DayRender(object sender, DayRenderEventArgs e)
        {
            try
            {
                CalendarDay day = e.Day;
                TableCell cell = e.Cell;
                DateTime date = day.Date;

                // Add today class
                if (date.Date == DateTime.Today)
                {
                    cell.CssClass += " today";
                }

                // Get leaves for this day
                List<LeaveInfo> dayLeaves = GetLeavesForDate(date);

                if (dayLeaves.Count > 0)
                {
                    // Create leave indicators
                    foreach (var leave in dayLeaves.Take(3)) // Show max 3 indicators per day
                    {
                        var indicator = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                        indicator.Attributes["class"] = $"leave-indicator {leave.LeaveType.ToLower().Replace(" ", "")}";
                        indicator.Attributes["style"] = $"background: {leave.ColorCode}; bottom: {4 + (dayLeaves.IndexOf(leave) * 8)}px;";
                        indicator.Attributes["title"] = $"{leave.EmployeeName} - {leave.LeaveType}";
                        cell.Controls.Add(indicator);
                    }

                    // Add count if more than 3 leaves
                    if (dayLeaves.Count > 3)
                    {
                        var countIndicator = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                        countIndicator.Attributes["class"] = "leave-count";
                        countIndicator.InnerText = $"+{dayLeaves.Count - 3}";
                        cell.Controls.Add(countIndicator);
                    }
                }

                // Add click event for day details
                if (!day.IsOtherMonth)
                {
                    cell.Attributes["onclick"] = $"showDayDetails('{date:yyyy-MM-dd}')";
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        protected void calLeave_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime selectedDate = calLeave.SelectedDate;
                hfSelectedDate.Value = selectedDate.ToString("yyyy-MM-dd");
                ShowDayDetails(selectedDate);
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading day details.", "error");
            }
        }

        protected void calLeave_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
        {
            try
            {
                hfCurrentMonth.Value = e.NewDate.Month.ToString();
                hfCurrentYear.Value = e.NewDate.Year.ToString();
                UpdateMonthDisplay();
                LoadCalendarData();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error changing month.", "error");
            }
        }

        #endregion

        #region Navigation Events

        protected void btnPrevMonth_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime currentVisible = calLeave.VisibleDate;
                calLeave.VisibleDate = currentVisible.AddMonths(-1);

                hfCurrentMonth.Value = calLeave.VisibleDate.Month.ToString();
                hfCurrentYear.Value = calLeave.VisibleDate.Year.ToString();

                UpdateMonthDisplay();
                LoadCalendarData();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error navigating to previous month.", "error");
            }
        }

        protected void btnNextMonth_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime currentVisible = calLeave.VisibleDate;
                calLeave.VisibleDate = currentVisible.AddMonths(1);

                hfCurrentMonth.Value = calLeave.VisibleDate.Month.ToString();
                hfCurrentYear.Value = calLeave.VisibleDate.Year.ToString();

                UpdateMonthDisplay();
                LoadCalendarData();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error navigating to next month.", "error");
            }
        }

        protected void btnToday_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime today = DateTime.Today;
                calLeave.VisibleDate = today;
                calLeave.SelectedDate = today;

                hfCurrentMonth.Value = today.Month.ToString();
                hfCurrentYear.Value = today.Year.ToString();

                UpdateMonthDisplay();
                LoadCalendarData();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error navigating to today.", "error");
            }
        }

        #endregion

        #region Filter Events

        protected void ddlViewType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCalendarData();
        }

        protected void ddlLeaveTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCalendarData();
        }

        #endregion

        #region Action Events

        protected void btnRequestLeave_Click(object sender, EventArgs e)
        {
            Response.Redirect("EmployeeRequestLeave.aspx");
        }

        protected void btnMyLeaves_Click(object sender, EventArgs e)
        {
            Response.Redirect("EmployeeLeavePortal.aspx");
        }

        protected void btnCloseDayDetails_Click(object sender, EventArgs e)
        {
            pnlDayDetails.Visible = false;
        }

        #endregion

        #region Data Loading Methods

        private void LoadCalendarStatistics()
        {
            try
            {
                DateTime monthStart = new DateTime(int.Parse(hfCurrentYear.Value), int.Parse(hfCurrentMonth.Value), 1);
                DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = BuildStatisticsQuery();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MonthStart", monthStart);
                        cmd.Parameters.AddWithValue("@MonthEnd", monthEnd);

                        if (Session["UserId"] != null)
                            cmd.Parameters.AddWithValue("@CurrentUserId", Convert.ToInt32(Session["UserId"]));
                        else
                            cmd.Parameters.AddWithValue("@CurrentUserId", 0);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litTotalLeaves.Text = reader["TotalLeaves"].ToString();
                                litUniqueEmployees.Text = reader["UniqueEmployees"].ToString();
                                litTotalDays.Text = reader["TotalDays"].ToString();
                            }
                        }
                    }

                    // Get busiest day
                    string busiestDayQuery = BuildBusiestDayQuery();
                    using (SqlCommand cmd = new SqlCommand(busiestDayQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@MonthStart", monthStart);
                        cmd.Parameters.AddWithValue("@MonthEnd", monthEnd);

                        if (Session["UserId"] != null)
                            cmd.Parameters.AddWithValue("@CurrentUserId", Convert.ToInt32(Session["UserId"]));
                        else
                            cmd.Parameters.AddWithValue("@CurrentUserId", 0);

                        object result = cmd.ExecuteScalar();
                        litBusiestDay.Text = result?.ToString() ?? "N/A";
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                litTotalLeaves.Text = "0";
                litUniqueEmployees.Text = "0";
                litTotalDays.Text = "0";
                litBusiestDay.Text = "N/A";
            }
        }

        private void LoadLeaveLegend()
        {
            try
            {
                DateTime monthStart = new DateTime(int.Parse(hfCurrentYear.Value), int.Parse(hfCurrentMonth.Value), 1);
                DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            lr.LeaveType,
                            COUNT(*) as Count,
                            COALESCE(lt.ColorCode, '#757575') as ColorCode
                        FROM LeaveRequests lr
                        INNER JOIN Employees e ON lr.EmployeeId = e.Id
                        LEFT JOIN LeaveTypes lt ON lr.LeaveType = lt.TypeName
                        WHERE lr.Status = 'Approved'
                        AND (lr.StartDate <= @MonthEnd AND lr.EndDate >= @MonthStart)";

                    query += GetViewTypeFilter();
                    query += GetLeaveTypeFilter();
                    query += " GROUP BY lr.LeaveType, lt.ColorCode ORDER BY Count DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MonthStart", monthStart);
                        cmd.Parameters.AddWithValue("@MonthEnd", monthEnd);

                        if (Session["UserId"] != null)
                            cmd.Parameters.AddWithValue("@CurrentUserId", Convert.ToInt32(Session["UserId"]));
                        else
                            cmd.Parameters.AddWithValue("@CurrentUserId", 0);

                        if (!string.IsNullOrEmpty(ddlLeaveTypeFilter.SelectedValue))
                            cmd.Parameters.AddWithValue("@LeaveType", ddlLeaveTypeFilter.SelectedValue);

                        DataTable dt = new DataTable();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }

                        rptLeaveLegend.DataSource = dt;
                        rptLeaveLegend.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private List<LeaveInfo> GetLeavesForDate(DateTime date)
        {
            List<LeaveInfo> leaves = new List<LeaveInfo>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            lr.Id,
                            CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName,
                            lr.LeaveType,
                            lr.StartDate,
                            lr.EndDate,
                            lr.Reason,
                            COALESCE(lt.ColorCode, '#757575') as ColorCode
                        FROM LeaveRequests lr
                        INNER JOIN Employees e ON lr.EmployeeId = e.Id
                        LEFT JOIN LeaveTypes lt ON lr.LeaveType = lt.TypeName
                        WHERE lr.Status = 'Approved'
                        AND @Date BETWEEN lr.StartDate AND lr.EndDate";

                    query += GetViewTypeFilter();
                    query += GetLeaveTypeFilter();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Date", date);

                        if (Session["UserId"] != null)
                            cmd.Parameters.AddWithValue("@CurrentUserId", Convert.ToInt32(Session["UserId"]));
                        else
                            cmd.Parameters.AddWithValue("@CurrentUserId", 0);

                        if (!string.IsNullOrEmpty(ddlLeaveTypeFilter.SelectedValue))
                            cmd.Parameters.AddWithValue("@LeaveType", ddlLeaveTypeFilter.SelectedValue);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                leaves.Add(new LeaveInfo
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    EmployeeName = reader["EmployeeName"].ToString(),
                                    LeaveType = reader["LeaveType"].ToString(),
                                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                                    EndDate = Convert.ToDateTime(reader["EndDate"]),
                                    Reason = reader["Reason"]?.ToString() ?? "",
                                    ColorCode = reader["ColorCode"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return leaves;
        }

        private void ShowDayDetails(DateTime selectedDate)
        {
            try
            {
                List<LeaveInfo> dayLeaves = GetLeavesForDate(selectedDate);

                litSelectedDate.Text = selectedDate.ToString("MMMM dd, yyyy");

                if (dayLeaves.Count > 0)
                {
                    rptDayLeaves.DataSource = dayLeaves;
                    rptDayLeaves.DataBind();
                    pnlNoLeavesForDay.Visible = false;
                }
                else
                {
                    rptDayLeaves.DataSource = null;
                    rptDayLeaves.DataBind();
                    pnlNoLeavesForDay.Visible = true;
                }

                pnlDayDetails.Visible = true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading day details.", "error");
            }
        }

        #endregion

        #region Query Builders

        private string BuildStatisticsQuery()
        {
            string baseQuery = @"
                SELECT 
                    COUNT(DISTINCT lr.Id) as TotalLeaves,
                    COUNT(DISTINCT lr.EmployeeId) as UniqueEmployees,
                    ISNULL(SUM(lr.DaysRequested), 0) as TotalDays
                FROM LeaveRequests lr
                INNER JOIN Employees e ON lr.EmployeeId = e.Id
                WHERE lr.Status = 'Approved'
                AND (lr.StartDate <= @MonthEnd AND lr.EndDate >= @MonthStart)";

            baseQuery += GetViewTypeFilter();
            baseQuery += GetLeaveTypeFilter();

            return baseQuery;
        }

        private string BuildBusiestDayQuery()
        {
            string baseQuery = @"
                WITH DailyCounts AS (
                    SELECT 
                        CONVERT(date, dates.DateValue) as LeaveDate,
                        COUNT(*) as LeaveCount
                    FROM (
                        SELECT 
                            DATEADD(day, number, lr.StartDate) as DateValue
                        FROM LeaveRequests lr
                        INNER JOIN Employees e ON lr.EmployeeId = e.Id
                        CROSS JOIN (
                            SELECT TOP 32 ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 as number
                            FROM sys.objects
                        ) numbers
                        WHERE lr.Status = 'Approved'
                        AND DATEADD(day, number, lr.StartDate) <= lr.EndDate
                        AND DATEADD(day, number, lr.StartDate) >= @MonthStart
                        AND DATEADD(day, number, lr.StartDate) <= @MonthEnd";

            baseQuery += GetViewTypeFilter().Replace("lr.", "lr.");
            baseQuery += GetLeaveTypeFilter().Replace("lr.", "lr.");

            baseQuery += @"
                    ) dates
                    GROUP BY CONVERT(date, dates.DateValue)
                )
                SELECT TOP 1 
                    DATENAME(weekday, LeaveDate) + ' ' + CAST(DAY(LeaveDate) AS VARCHAR)
                FROM DailyCounts
                ORDER BY LeaveCount DESC, LeaveDate ASC";

            return baseQuery;
        }

        private string GetViewTypeFilter()
        {
            string viewType = ddlViewType.SelectedValue;
            switch (viewType)
            {
                case "my":
                    return " AND e.UserId = @CurrentUserId";
                case "team":
                    return @" AND (e.ManagerId = (SELECT Id FROM Employees WHERE UserId = @CurrentUserId) 
                             OR e.UserId = @CurrentUserId)";
                case "department":
                    return @" AND e.DepartmentId = (SELECT DepartmentId FROM Employees WHERE UserId = @CurrentUserId)";
                default:
                    return "";
            }
        }

        private string GetLeaveTypeFilter()
        {
            if (!string.IsNullOrEmpty(ddlLeaveTypeFilter.SelectedValue))
                return " AND lr.LeaveType = @LeaveType";
            return "";
        }

        #endregion

        #region Helper Methods

        private void UpdateMonthDisplay()
        {
            DateTime displayDate = new DateTime(int.Parse(hfCurrentYear.Value), int.Parse(hfCurrentMonth.Value), 1);
            litCurrentMonth.Text = displayDate.ToString("MMMM yyyy");
        }

        protected string GetInitials(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return "??";

            string[] parts = fullName.Split(' ');
            if (parts.Length == 1)
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();

            return (parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1)).ToUpper();
        }

        protected string GetLeaveDuration(object startDate, object endDate)
        {
            try
            {
                DateTime start = Convert.ToDateTime(startDate);
                DateTime end = Convert.ToDateTime(endDate);

                if (start.Date == end.Date)
                    return "1 day";

                int days = (end.Date - start.Date).Days + 1;
                return $"{days} days ({start:MMM dd} - {end:MMM dd})";
            }
            catch
            {
                return "Duration unknown";
            }
        }

        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = $"message-text {type}";
            pnlMessages.CssClass = $"alert-panel alert-{type}";
            pnlMessages.Visible = true;
        }

        private void LogError(Exception ex)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ErrorLogs (ErrorMessage, StackTrace, Source, Timestamp, UserId, Severity, CreatedAt)
                        VALUES (@ErrorMessage, @StackTrace, @Source, @Timestamp, @UserId, @Severity, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message);
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", "LeaveCalendar.aspx");
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserId", Session["UserId"] ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Severity", "High");
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Fail silently to avoid recursive errors
            }
        }

        #endregion
    }

    #region Data Classes

    public class LeaveInfo
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public string ColorCode { get; set; }
    }

    #endregion
}