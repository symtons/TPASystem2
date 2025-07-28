using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;

namespace TPASystem2.LeaveManagement
{
    public partial class LeaveCalendar : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DateTime currentDate;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //if (Session["UserId"] == null)
                //{
                //    Response.Redirect("~/Login.aspx", false);
                //    return;
                //}

                InitializePage();
                LoadDepartments();
                SetViewTypeBasedOnRole();
                LoadCalendar();
            }
        }

        #region Page Initialization

        private void InitializePage()
        {
            // Get current date from ViewState or use today's date
            if (ViewState["CurrentDate"] != null)
            {
                currentDate = (DateTime)ViewState["CurrentDate"];
            }
            else
            {
                currentDate = DateTime.Today;
                ViewState["CurrentDate"] = currentDate;
            }

            // Set calendar date and display current month
            calLeaveCalendar.VisibleDate = currentDate;
            calLeaveCalendar.SelectedDate = currentDate;
            litCurrentMonth.Text = currentDate.ToString("MMMM yyyy");
        }

        private void LoadDepartments()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Name FROM Departments WHERE IsActive = 1 ORDER BY Name";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlDepartmentFilter.Items.Clear();
                            ddlDepartmentFilter.Items.Add(new ListItem("All Departments", ""));

                            while (reader.Read())
                            {
                                ddlDepartmentFilter.Items.Add(new ListItem(
                                    reader["Name"].ToString(),
                                    reader["Id"].ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading departments.", "error");
            }
        }

        private void SetViewTypeBasedOnRole()
        {
            string userRole = Session["UserRole"]?.ToString() ?? "";

            // Set default view based on user role
            switch (userRole)
            {
                case "SUPERADMIN":
                case "HRADMIN":
                    ddlViewType.SelectedValue = "all";
                    break;
                case "MANAGER":
                    ddlViewType.SelectedValue = "team";
                    break;
                default:
                    ddlViewType.SelectedValue = "my";
                    break;
            }

            // Restrict view options based on role
            if (userRole != "SUPERADMIN" && userRole != "HRADMIN")
            {
                // Remove "All Leaves" option for non-admin users
                ListItem allLeavesItem = ddlViewType.Items.FindByValue("all");
                if (allLeavesItem != null && userRole != "MANAGER")
                {
                    ddlViewType.Items.Remove(allLeavesItem);
                }
            }
        }

        #endregion

        #region Calendar Management

        private void LoadCalendar()
        {
            try
            {
                // Force calendar to refresh by setting visible date
                calLeaveCalendar.VisibleDate = currentDate;

                // Update month display
                litCurrentMonth.Text = currentDate.ToString("MMMM yyyy");

                // The actual leave data loading happens in DayRender event
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

                // Get leave requests for this date
                var leaveRequests = GetLeaveRequestsForDate(date);

                if (leaveRequests.Count > 0)
                {
                    // Add leave indicator based on leave types
                    var leaveTypes = leaveRequests.Select(lr => lr.LeaveType.ToLower()).Distinct().ToList();

                    string indicatorClass = "leave-indicator ";
                    if (leaveTypes.Count > 1)
                    {
                        indicatorClass += "multiple";
                    }
                    else
                    {
                        indicatorClass += leaveTypes.First();
                    }

                    // Create indicator div
                    var indicator = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                    indicator.Attributes["class"] = indicatorClass;
                    e.Cell.Controls.Add(indicator);

                    // Add tooltip with leave details
                    string tooltip = string.Join(", ", leaveRequests.Select(lr =>
                        $"{lr.EmployeeName} ({lr.LeaveType})"));

                    if (tooltip.Length > 50)
                    {
                        tooltip = tooltip.Substring(0, 47) + "...";
                    }

                    e.Cell.ToolTip = tooltip;

                    // Make the cell clickable for more details
                    e.Cell.Attributes["onclick"] = $"showDayDetails('{date:yyyy-MM-dd}')";
                    e.Cell.Style["cursor"] = "pointer";
                }

                // Highlight weekends
                if (e.Day.IsWeekend)
                {
                    e.Cell.CssClass += " calendar-weekend";
                }

                // Highlight today
                if (e.Day.Date == DateTime.Today)
                {
                    e.Cell.Style["font-weight"] = "bold";
                    e.Cell.Style["border"] = "2px solid #007bff";
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                // Don't show error to user during day render to avoid multiple messages
            }
        }

        protected void calLeaveCalendar_SelectionChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = calLeaveCalendar.SelectedDate;
            ShowDayDetails(selectedDate);
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
                        AddLeaveQueryParameters(cmd, date);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                leaveRequests.Add(new LeaveRequestInfo
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    EmployeeName = reader["EmployeeName"].ToString(),
                                    Department = reader["Department"]?.ToString() ?? "Not Assigned",
                                    LeaveType = reader["LeaveType"].ToString(),
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
                    lr.LeaveType,
                    lr.StartDate,
                    lr.EndDate,
                    lr.DaysRequested,
                    lr.Status,
                    CONCAT(e.FirstName, ' ', e.LastName) as EmployeeName,
                    d.Name as Department
                FROM LeaveRequests lr
                INNER JOIN Employees e ON lr.EmployeeId = e.Id
                LEFT JOIN Departments d ON e.DepartmentId = d.Id
                WHERE lr.Status = 'Approved'
                AND @Date BETWEEN lr.StartDate AND lr.EndDate";

            // Add view type filter
            string viewType = ddlViewType.SelectedValue;
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            switch (viewType)
            {
                case "my":
                    baseQuery += " AND e.UserId = @CurrentUserId";
                    break;
                case "team":
                    baseQuery += @" AND e.ManagerId IN (
                        SELECT Id FROM Employees WHERE UserId = @CurrentUserId
                    ) OR e.UserId = @CurrentUserId";
                    break;
                case "department":
                    baseQuery += @" AND e.DepartmentId IN (
                        SELECT DepartmentId FROM Employees WHERE UserId = @CurrentUserId
                    )";
                    break;
                case "all":
                    // No additional filter for admin users
                    break;
            }

            // Add department filter
            if (!string.IsNullOrEmpty(ddlDepartmentFilter.SelectedValue))
            {
                baseQuery += " AND e.DepartmentId = @DepartmentFilter";
            }

            // Add leave type filter
            if (!string.IsNullOrEmpty(ddlLeaveTypeFilter.SelectedValue))
            {
                baseQuery += " AND lr.LeaveType = @LeaveTypeFilter";
            }

            return baseQuery;
        }

        private void AddLeaveQueryParameters(SqlCommand cmd, DateTime date)
        {
            cmd.Parameters.AddWithValue("@Date", date);

            string viewType = ddlViewType.SelectedValue;
            int currentUserId = Convert.ToInt32(Session["UserId"]);

            if (viewType == "my" || viewType == "team" || viewType == "department")
            {
                cmd.Parameters.AddWithValue("@CurrentUserId", currentUserId);
            }

            if (!string.IsNullOrEmpty(ddlDepartmentFilter.SelectedValue))
            {
                cmd.Parameters.AddWithValue("@DepartmentFilter", Convert.ToInt32(ddlDepartmentFilter.SelectedValue));
            }

            if (!string.IsNullOrEmpty(ddlLeaveTypeFilter.SelectedValue))
            {
                cmd.Parameters.AddWithValue("@LeaveTypeFilter", ddlLeaveTypeFilter.SelectedValue);
            }
        }

        private void ShowDayDetails(DateTime selectedDate)
        {
            try
            {
                var leaveRequests = GetLeaveRequestsForDate(selectedDate);

                litSelectedDate.Text = selectedDate.ToString("MMMM dd, yyyy");

                if (leaveRequests.Count > 0)
                {
                    gvDayLeaves.DataSource = leaveRequests;
                    gvDayLeaves.DataBind();
                }
                else
                {
                    gvDayLeaves.DataSource = null;
                    gvDayLeaves.DataBind();
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

        #region Event Handlers

        protected void btnRequestLeave_Click(object sender, EventArgs e)
        {
            Response.Redirect("RequestLeave.aspx", false);
        }

        protected void btnBackToDashboard_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx", false);
        }

        protected void btnPrevMonth_Click(object sender, EventArgs e)
        {
            currentDate = currentDate.AddMonths(-1);
            ViewState["CurrentDate"] = currentDate;
            LoadCalendar();
            pnlDayDetails.Visible = false;
        }

        protected void btnNextMonth_Click(object sender, EventArgs e)
        {
            currentDate = currentDate.AddMonths(1);
            ViewState["CurrentDate"] = currentDate;
            LoadCalendar();
            pnlDayDetails.Visible = false;
        }

        protected void ddlViewType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCalendar();
            pnlDayDetails.Visible = false;
        }

        protected void ddlDepartmentFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCalendar();
            pnlDayDetails.Visible = false;
        }

        protected void ddlLeaveTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCalendar();
            pnlDayDetails.Visible = false;
        }

        #endregion

        #region Utility Methods

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = $"alert-panel {type}";
            litMessage.Text = message;
        }

        private void LogError(Exception ex)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ErrorLogs (ErrorId, ErrorMessage, StackTrace, Source, Timestamp, RequestUrl, UserAgent, IPAddress, UserId, Severity, CreatedAt)
                        VALUES (@ErrorId, @ErrorMessage, @StackTrace, @Source, @Timestamp, @RequestUrl, @UserAgent, @IPAddress, @UserId, @Severity, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorId", Guid.NewGuid().ToString());
                        cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message ?? "");
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", ex.Source ?? "LeaveManagement.LeaveCalendar");
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@RequestUrl", Request.Url?.ToString() ?? "");
                        cmd.Parameters.AddWithValue("@UserAgent", Request.UserAgent ?? "");
                        cmd.Parameters.AddWithValue("@IPAddress", GetClientIP());
                        cmd.Parameters.AddWithValue("@UserId", Session["UserId"] ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Severity", "High");
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        cmd.ExecuteNonQuery();
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

        #endregion
    }

    #region Data Models

    public class LeaveRequestInfo
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DaysRequested { get; set; }
        public string Status { get; set; }
    }

    #endregion
}