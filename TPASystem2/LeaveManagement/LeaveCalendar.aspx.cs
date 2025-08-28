using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace TPASystem2.LeaveManagement
{
    public partial class LeaveCalendar : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private DateTime CurrentDate
        {
            get
            {
                if (DateTime.TryParse(hfCurrentDate.Value, out DateTime date))
                    return date;
                return DateTime.Today;
            }
            set
            {
                hfCurrentDate.Value = value.ToString("yyyy-MM-dd");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                Session["UserId"] = 2; // Use existing employee ID
                Session["UserRole"] = "Employee";
                Session["Username"] = "demo.user";
                //if (!IsAuthorizedUser())
                //{
                //    Response.Redirect("~/Dashboard.aspx");
                //    return;
                //}

                InitializePage();
            }
        }

        #region Page Initialization

        private bool IsAuthorizedUser()
        {
            return Session["UserId"] != null;
        }

        private void InitializePage()
        {
            //try
            //{
                CurrentDate = DateTime.Today;
                LoadUserInfo();
                LoadFilterDropdowns();
                SetupViewType();
                LoadCalendar();
                LoadLegend();
                LoadMonthlyStatistics();
            //}
            //catch (Exception ex)
            //{
            //    LogError(ex);
            //    ShowMessage("Error loading calendar.", "error");
            //}
        }

        private void LoadUserInfo()
        {
            //try
            //{
                int userId = Convert.ToInt32(Session["UserId"]);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT CONCAT(FirstName, ' ', LastName) as FullName
                        FROM Employees 
                        WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            litCurrentUser.Text = result.ToString();
                        }
                        else
                        {
                            litCurrentUser.Text = "User";
                        }
                    }
                }

                // Set permission based on session role (fallback)
                string userRole = Session["UserRole"]?.ToString() ?? "";
                switch (userRole.ToUpper())
                {
                    case "HRADMIN":
                    case "ADMIN":
                        litViewPermission.Text = "Full Company Access";
                        break;
                    case "MANAGER":
                    case "SUPERVISOR":
                        litViewPermission.Text = "Team Management Access";
                        break;
                    default:
                        litViewPermission.Text = "Personal View Access";
                        break;
                }

                litCurrentMonth.Text = CurrentDate.ToString("MMMM yyyy");
            //}
            //catch (Exception ex)
            //{
            //    LogError(ex);
            //    litCurrentUser.Text = "User";
            //    litViewPermission.Text = "Standard Access";
            //}
        }

        private void LoadFilterDropdowns()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load Departments
                    string deptQuery = "SELECT Id, Name FROM Departments ORDER BY Name";
                    using (SqlCommand cmd = new SqlCommand(deptQuery, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlDepartmentFilter.Items.Clear();
                            ddlDepartmentFilter.Items.Add(new ListItem("All Departments", ""));

                            while (reader.Read())
                            {
                                ddlDepartmentFilter.Items.Add(new ListItem(
                                    reader["Name"].ToString(),
                                    reader["Id"].ToString()
                                ));
                            }
                        }
                    }

                    // Load Leave Types - check if LeaveTypes table exists
                    string checkLeaveTypesQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = 'LeaveTypes'";

                    using (SqlCommand checkCmd = new SqlCommand(checkLeaveTypesQuery, conn))
                    {
                        int tableExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        ddlLeaveTypeFilter.Items.Clear();
                        ddlLeaveTypeFilter.Items.Add(new ListItem("All Leave Types", ""));

                        if (tableExists > 0)
                        {
                            string leaveTypeQuery = "SELECT DISTINCT TypeName FROM LeaveTypes ORDER BY TypeName";
                            using (SqlCommand cmd = new SqlCommand(leaveTypeQuery, conn))
                            {
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string typeName = reader["TypeName"].ToString();
                                        ddlLeaveTypeFilter.Items.Add(new ListItem(typeName, typeName));
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Fallback: get distinct leave types from LeaveRequests
                            string fallbackQuery = "SELECT DISTINCT LeaveType FROM LeaveRequests WHERE LeaveType IS NOT NULL ORDER BY LeaveType";
                            using (SqlCommand cmd = new SqlCommand(fallbackQuery, conn))
                            {
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        string typeName = reader["LeaveType"].ToString();
                                        ddlLeaveTypeFilter.Items.Add(new ListItem(typeName, typeName));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void SetupViewType()
        {
            //try
            //{
                string userRole = Session["UserRole"]?.ToString() ?? "";

                ddlViewType.Items.Clear();
                ddlViewType.Items.Add(new ListItem("My Leaves", "my"));

                // Add options based on session role
                switch (userRole.ToUpper())
                {
                    case "HRADMIN":
                    case "ADMIN":
                        ddlViewType.Items.Add(new ListItem("Team Leaves", "team"));
                        ddlViewType.Items.Add(new ListItem("Department Leaves", "department"));
                        ddlViewType.Items.Add(new ListItem("All Leaves", "all"));
                        ddlViewType.SelectedValue = "all";
                        break;
                    case "MANAGER":
                    case "SUPERVISOR":
                        ddlViewType.Items.Add(new ListItem("Team Leaves", "team"));
                        ddlViewType.Items.Add(new ListItem("Department Leaves", "department"));
                        ddlViewType.SelectedValue = "team";
                        break;
                    default:
                        ddlViewType.SelectedValue = "my";
                        break;
                }
            //}
            //catch (Exception ex)
            //{
            //    LogError(ex);
            //    ddlViewType.Items.Clear();
            //    ddlViewType.Items.Add(new ListItem("My Leaves", "my"));
            //    ddlViewType.SelectedValue = "my";
            //}
        }

        private void LoadLegend()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if LeaveTypes table exists
                    string checkQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = 'LeaveTypes'";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        int tableExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (tableExists > 0)
                        {
                            string query = "SELECT TypeName FROM LeaveTypes ORDER BY TypeName";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                                {
                                    DataTable dt = new DataTable();
                                    adapter.Fill(dt);
                                    rptLegend.DataSource = dt;
                                    rptLegend.DataBind();
                                }
                            }
                        }
                        else
                        {
                            // Fallback: create legend from existing leave requests
                            string fallbackQuery = "SELECT DISTINCT LeaveType as TypeName FROM LeaveRequests WHERE LeaveType IS NOT NULL ORDER BY LeaveType";
                            using (SqlCommand cmd = new SqlCommand(fallbackQuery, conn))
                            {
                                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                                {
                                    DataTable dt = new DataTable();
                                    adapter.Fill(dt);
                                    rptLegend.DataSource = dt;
                                    rptLegend.DataBind();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        #endregion

        #region Calendar Management

        private void LoadCalendar()
        {
            try
            {
                calLeaveCalendar.VisibleDate = CurrentDate;
                calLeaveCalendar.TodaysDate = DateTime.Today;
                litMonthYear.Text = CurrentDate.ToString("MMMM yyyy");
                litCurrentMonth.Text = CurrentDate.ToString("MMMM yyyy");
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading calendar.", "error");
            }
        }

        protected void calLeaveCalendar_DayRender(object sender, DayRenderEventArgs e)
        {
            try
            {
                DateTime date = e.Day.Date;
                var leaveRequests = GetLeaveRequestsForDate(date);

                if (leaveRequests.Count > 0)
                {
                    // Add leave indicator
                    var leaveTypes = leaveRequests.Select(lr => lr.LeaveType.ToLower().Replace(" ", "")).Distinct().ToList();

                    string indicatorClass = "leave-indicator ";
                    if (leaveTypes.Count > 1)
                    {
                        indicatorClass += "multiple";
                    }
                    else
                    {
                        indicatorClass += leaveTypes.First();
                    }

                    var indicator = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                    indicator.Attributes["class"] = indicatorClass;
                    indicator.Attributes["title"] = BuildTooltip(leaveRequests);

                    // Apply color if available
                    if (leaveRequests.Count == 1 && !string.IsNullOrEmpty(leaveRequests.First().ColorCode))
                    {
                        indicator.Style["background"] = leaveRequests.First().ColorCode;
                    }

                    e.Cell.Controls.Add(indicator);

                    // Add count if multiple people
                    if (leaveRequests.Count > 1)
                    {
                        var countLabel = new System.Web.UI.HtmlControls.HtmlGenericControl("span");
                        countLabel.Attributes["class"] = "leave-count";
                        countLabel.InnerText = leaveRequests.Count.ToString();
                        e.Cell.Controls.Add(countLabel);
                    }
                }

                // Highlight today
                if (date.Date == DateTime.Today)
                {
                    e.Cell.CssClass += " today";
                }

                // Make cell clickable
                e.Cell.Attributes["onclick"] = $"__doPostBack('{calLeaveCalendar.UniqueID}', 'SELECT${date:yyyy-MM-dd}');";
                e.Cell.Style["cursor"] = "pointer";
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private string BuildTooltip(List<LeaveRequestInfo> requests)
        {
            if (requests.Count == 1)
            {
                return $"{requests.First().EmployeeName} - {requests.First().LeaveType}";
            }
            return $"{requests.Count} employee(s) on leave";
        }

        private List<LeaveRequestInfo> GetLeaveRequestsForDate(DateTime date)
        {
            var leaveRequests = new List<LeaveRequestInfo>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = BuildLeaveQuery();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        AddQueryParameters(cmd, date);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                leaveRequests.Add(new LeaveRequestInfo
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    EmployeeName = reader["EmployeeName"]?.ToString() ?? "Unknown",
                                    Department = reader["Department"]?.ToString() ?? "Not Assigned",
                                    LeaveType = reader["LeaveType"].ToString(),
                                    ColorCode = reader["ColorCode"].ToString(),
                                    //ColorCode = reader.IsDBNull("ColorCode") ? "" : reader["ColorCode"].ToString(),
                                    StartDate = Convert.ToDateTime(reader["StartDate"]),
                                    EndDate = Convert.ToDateTime(reader["EndDate"]),
                                    DaysRequested = Convert.ToDecimal(reader["DaysRequested"]),
                                    Status = reader["Status"].ToString()
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

            return leaveRequests;
        }

        private string BuildLeaveQuery()
        {
            string baseQuery = @"
                SELECT 
                    lr.Id,
                    CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName,
                    ISNULL(d.Name, 'Not Assigned') as Department,
                    lr.LeaveType,
                    lr.StartDate,
                    lr.EndDate,
                    lr.DaysRequested,
                    lr.Status";

            // Check if LeaveTypes table exists and has ColorCode column
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string checkQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.COLUMNS 
                        WHERE TABLE_NAME = 'LeaveTypes' AND COLUMN_NAME = 'ColorCode'";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        int columnExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (columnExists > 0)
                        {
                            baseQuery += ", ISNULL(lt.ColorCode, '#757575') as ColorCode";
                        }
                        else
                        {
                            baseQuery += ", '#757575' as ColorCode";
                        }
                    }
                }
            }
            catch
            {
                baseQuery += ", '#757575' as ColorCode";
            }

            baseQuery += @"
                FROM LeaveRequests lr
                INNER JOIN Employees e ON lr.EmployeeId = e.Id
                LEFT JOIN Departments d ON e.DepartmentId = d.Id";

            // Add LeaveTypes join if table exists
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string checkQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = 'LeaveTypes'";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        int tableExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (tableExists > 0)
                        {
                            baseQuery += " LEFT JOIN LeaveTypes lt ON lr.LeaveType = lt.TypeName";
                        }
                    }
                }
            }
            catch
            {
                // Continue without join
            }

            baseQuery += @"
                WHERE lr.Status = 'Approved'
                AND @Date BETWEEN lr.StartDate AND lr.EndDate";

            // Add view filters
            string viewType = ddlViewType.SelectedValue;
            switch (viewType)
            {
                case "my":
                    baseQuery += " AND e.UserId = @CurrentUserId";
                    break;
                case "team":
                    baseQuery += @" AND (e.ManagerId = (SELECT Id FROM Employees WHERE UserId = @CurrentUserId) 
                                   OR e.UserId = @CurrentUserId)";
                    break;
                case "department":
                    baseQuery += @" AND e.DepartmentId = (SELECT DepartmentId FROM Employees WHERE UserId = @CurrentUserId)";
                    break;
                    // "all" case - no additional filter
            }

            // Add optional filters
            if (!string.IsNullOrEmpty(ddlDepartmentFilter.SelectedValue))
            {
                baseQuery += " AND e.DepartmentId = @DepartmentFilter";
            }

            if (!string.IsNullOrEmpty(ddlLeaveTypeFilter.SelectedValue))
            {
                baseQuery += " AND lr.LeaveType = @LeaveTypeFilter";
            }

            return baseQuery;
        }

        private void AddQueryParameters(SqlCommand cmd, DateTime date)
        {
            cmd.Parameters.AddWithValue("@Date", date);

            string viewType = ddlViewType.SelectedValue;
            if (viewType != "all")
            {
                // Safe conversion
                if (Session["UserId"] != null && int.TryParse(Session["UserId"].ToString(), out int userId))
                {
                    cmd.Parameters.AddWithValue("@CurrentUserId", userId);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@CurrentUserId", 2); // Fallback
                }
            }

            // Same pattern for other parameters
            if (!string.IsNullOrEmpty(ddlDepartmentFilter.SelectedValue))
            {
                if (int.TryParse(ddlDepartmentFilter.SelectedValue, out int deptId))
                {
                    cmd.Parameters.AddWithValue("@DepartmentFilter", deptId);
                }
            }

            if (!string.IsNullOrEmpty(ddlLeaveTypeFilter.SelectedValue))
            {
                cmd.Parameters.AddWithValue("@LeaveTypeFilter", ddlLeaveTypeFilter.SelectedValue);
            }
        }

        #endregion

        #region Event Handlers

       

        protected void btnNextMonth_Click(object sender, EventArgs e)
        {
            CurrentDate = CurrentDate.AddMonths(1);
            LoadCalendar();
            LoadMonthlyStatistics();
        }

        protected void btnToday_Click(object sender, EventArgs e)
        {
            CurrentDate = DateTime.Today;
            LoadCalendar();
            LoadMonthlyStatistics();
        }

        protected void btnRequestLeave_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/LeaveManagement/EmployeeRequestLeave.aspx");
        }

        protected void btnBackToManagement_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/LeaveManagement/EmployeeLeavePortal.aspx");
        }

        protected void ddlViewType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCalendar();
            LoadMonthlyStatistics();
        }

        protected void ddlDepartmentFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCalendar();
            LoadMonthlyStatistics();
        }

        protected void ddlLeaveTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCalendar();
            LoadMonthlyStatistics();
        }

        protected void calLeaveCalendar_SelectionChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = calLeaveCalendar.SelectedDate;
            ShowDayDetails(selectedDate);
        }

        protected void btnCloseDayDetails_Click(object sender, EventArgs e)
        {
            pnlDayDetails.Visible = false;
        }

        #endregion

        #region Day Details

        private void ShowDayDetails(DateTime date)
        {
            try
            {
                hfSelectedDate.Value = date.ToString("yyyy-MM-dd");
                litSelectedDate.Text = date.ToString("MMMM dd, yyyy");

                var leaveRequests = GetLeaveRequestsForDate(date);
                gvDayLeaveDetails.DataSource = leaveRequests;
                gvDayLeaveDetails.DataBind();

                pnlDayDetails.Visible = true;
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading day details.", "error");
            }
        }

        protected string GetLeaveProgress(object startDate, object endDate)
        {
            try
            {
                DateTime start = Convert.ToDateTime(startDate);
                DateTime end = Convert.ToDateTime(endDate);
                DateTime today = DateTime.Today;

                if (today < start)
                {
                    int daysUntil = (start - today).Days;
                    return $"Starts in {daysUntil} day{(daysUntil != 1 ? "s" : "")}";
                }
                else if (today >= start && today <= end)
                {
                    int totalDays = (end - start).Days + 1;
                    int daysPassed = (today - start).Days + 1;
                    return $"Day {daysPassed} of {totalDays}";
                }
                else
                {
                    return "Completed";
                }
            }
            catch
            {
                return "Unknown";
            }
        }

        #endregion

        #region Monthly Statistics

        private void LoadMonthlyStatistics()
        {
            try
            {
                DateTime monthStart = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
                DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string statsQuery = BuildStatisticsQuery();
                    using (SqlCommand cmd = new SqlCommand(statsQuery, conn))
                    {
                        AddStatisticsParameters(cmd, monthStart, monthEnd);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litTotalLeaves.Text = reader["TotalLeaves"]?.ToString() ?? "0";
                                litUniqueEmployees.Text = reader["UniqueEmployees"]?.ToString() ?? "0";
                                litTotalDays.Text = reader["TotalDays"]?.ToString() ?? "0";
                            }
                            else
                            {
                                litTotalLeaves.Text = "0";
                                litUniqueEmployees.Text = "0";
                                litTotalDays.Text = "0";
                            }
                        }
                    }

                    // Get busiest day (simplified)
                    string busiestDayQuery = @"
                        SELECT TOP 1 CAST(lr.StartDate AS DATE) as BusiestDate
                        FROM LeaveRequests lr
                        INNER JOIN Employees e ON lr.EmployeeId = e.Id
                        WHERE lr.Status = 'Approved'
                        AND lr.StartDate BETWEEN @MonthStart AND @MonthEnd
                        GROUP BY CAST(lr.StartDate AS DATE)
                        ORDER BY COUNT(*) DESC";

                    using (SqlCommand cmd = new SqlCommand(busiestDayQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@MonthStart", monthStart);
                        cmd.Parameters.AddWithValue("@MonthEnd", monthEnd);

                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            DateTime busiestDay = Convert.ToDateTime(result);
                            litBusiestDay.Text = busiestDay.ToString("MMM dd");
                        }
                        else
                        {
                            litBusiestDay.Text = "N/A";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                // Set defaults
                litTotalLeaves.Text = "0";
                litUniqueEmployees.Text = "0";
                litTotalDays.Text = "0";
                litBusiestDay.Text = "N/A";
            }
        }

        private string BuildStatisticsQuery()
        {
            string baseQuery = @"
                SELECT 
                    COUNT(DISTINCT lr.Id) as TotalLeaves,
                    COUNT(DISTINCT lr.EmployeeId) as UniqueEmployees,
                    ISNULL(SUM(lr.DaysRequested), 0) as TotalDays
                FROM LeaveRequests lr
                INNER JOIN Employees e ON lr.EmployeeId = e.Id
                LEFT JOIN Departments d ON e.DepartmentId = d.Id
                WHERE lr.Status = 'Approved'
                AND (lr.StartDate <= @MonthEnd AND lr.EndDate >= @MonthStart)";

            // Add view type filter
            string viewType = ddlViewType.SelectedValue;
            switch (viewType)
            {
                case "my":
                    baseQuery += " AND e.UserId = @CurrentUserId";
                    break;
                case "team":
                    baseQuery += @" AND (e.ManagerId = (SELECT Id FROM Employees WHERE UserId = @CurrentUserId) 
                                   OR e.UserId = @CurrentUserId)";
                    break;
                case "department":
                    baseQuery += @" AND e.DepartmentId = (SELECT DepartmentId FROM Employees WHERE UserId = @CurrentUserId)";
                    break;
            }

            // Add filters
            if (!string.IsNullOrEmpty(ddlDepartmentFilter.SelectedValue))
            {
                baseQuery += " AND e.DepartmentId = @DepartmentFilter";
            }

            if (!string.IsNullOrEmpty(ddlLeaveTypeFilter.SelectedValue))
            {
                baseQuery += " AND lr.LeaveType = @LeaveTypeFilter";
            }

            return baseQuery;
        }

        private void AddStatisticsParameters(SqlCommand cmd, DateTime monthStart, DateTime monthEnd)
        {
            cmd.Parameters.AddWithValue("@MonthStart", monthStart);
            cmd.Parameters.AddWithValue("@MonthEnd", monthEnd);

            string viewType = ddlViewType.SelectedValue;
            if (viewType != "all")
            {
                // Safe conversion
                if (Session["UserId"] != null && int.TryParse(Session["UserId"].ToString(), out int userId))
                {
                    cmd.Parameters.AddWithValue("@CurrentUserId", userId);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@CurrentUserId", 2); // Fallback
                }
            }

            // Same pattern for department filter
            if (!string.IsNullOrEmpty(ddlDepartmentFilter.SelectedValue))
            {
                if (int.TryParse(ddlDepartmentFilter.SelectedValue, out int deptId))
                {
                    cmd.Parameters.AddWithValue("@DepartmentFilter", deptId);
                }
            }

            if (!string.IsNullOrEmpty(ddlLeaveTypeFilter.SelectedValue))
            {
                cmd.Parameters.AddWithValue("@LeaveTypeFilter", ddlLeaveTypeFilter.SelectedValue);
            }
        }

        #endregion

        #region Utility Methods

        private void ShowMessage(string message, string type)
        {
            try
            {
                string cssClass = type == "error" ? "alert-danger" : "alert-success";
                litMessage.Text = $"<div class='alert {cssClass}' role='alert'>" +
                                 $"<i class='material-icons'>{(type == "error" ? "error" : "check_circle")}</i>" +
                                 $"<span>{message}</span>" +
                                 $"</div>";
                pnlMessage.Visible = true;
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void LogError(Exception ex)
        {
            try
            {
                // Check if ErrorLogs table exists
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string checkQuery = @"
                        SELECT COUNT(*) 
                        FROM INFORMATION_SCHEMA.TABLES 
                        WHERE TABLE_NAME = 'ErrorLogs'";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        int tableExists = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (tableExists > 0)
                        {
                            string query = @"
                                INSERT INTO ErrorLogs (ErrorMessage, Source, Timestamp, RequestUrl, IPAddress, UserId)
                                VALUES (@ErrorMessage, @Source, @Timestamp, @RequestUrl, @IPAddress, @UserId)";

                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message ?? "");
                                cmd.Parameters.AddWithValue("@Source", "LeaveManagement.LeaveCalendar");
                                cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                                cmd.Parameters.AddWithValue("@RequestUrl", Request.Url?.ToString() ?? "");
                                cmd.Parameters.AddWithValue("@IPAddress", GetClientIP());
                                cmd.Parameters.AddWithValue("@UserId", Session["UserId"] ?? (object)DBNull.Value);

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch
            {
                // Fail silently for logging errors
            }
        }

        private string GetClientIP()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip ?? "Unknown";
        }

        protected void btnPrevMonth_Click(object sender, EventArgs e)
        {
            // Ensure session exists
            if (Session["UserId"] == null)
            {
                Session["UserId"] = 2;
                Session["UserRole"] = "Employee";
            }

            CurrentDate = CurrentDate.AddMonths(-1);
            LoadCalendar();
            LoadMonthlyStatistics();
        }

        #endregion
    }

    #region Data Models

    public class LeaveRequestInfo
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string LeaveType { get; set; }
        public string ColorCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DaysRequested { get; set; }
        public string Status { get; set; }
    }

    #endregion
}