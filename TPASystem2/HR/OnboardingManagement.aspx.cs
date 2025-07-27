using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

namespace TPASystem2.HR
{
    public partial class OnboardingManagement : System.Web.UI.Page
    {
        #region Properties and Fields

        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set UnobtrusiveValidationMode to prevent validation errors
                Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

                InitializePage();
                LoadDepartments();
                SetDefaultFilters();
                LoadOnboardingData();
                LoadStatistics();
            }
        }

        #endregion

        #region Initialization Methods

        private void InitializePage()
        {
            // Check user permissions (commented out for testing)
            //if (Session["UserRole"] == null || Session["UserId"] == null)
            //{
            //    Response.Redirect("~/Login.aspx");
            //    return;
            //}

            // Initialize dropdowns
            InitializeFilterDropdowns();
            InitializeBulkActionDropdown();
        }

        private void InitializeFilterDropdowns()
        {
            try
            {
                // Status dropdown
                ddlFilterStatus.Items.Clear();
                ddlFilterStatus.Items.Add(new ListItem("All Statuses", ""));
                ddlFilterStatus.Items.Add(new ListItem("Completed", "COMPLETED"));
                ddlFilterStatus.Items.Add(new ListItem("In Progress", "IN_PROGRESS"));
                ddlFilterStatus.Items.Add(new ListItem("Pending", "PENDING"));
                ddlFilterStatus.Items.Add(new ListItem("Overdue", "OVERDUE"));
                ddlFilterStatus.Items.Add(new ListItem("No Tasks", "NO_TASKS"));

                // Page size dropdown
                ddlPageSize.Items.Clear();
                ddlPageSize.Items.Add(new ListItem("10", "10"));
                ddlPageSize.Items.Add(new ListItem("25", "25"));
                ddlPageSize.Items.Add(new ListItem("50", "50"));
                ddlPageSize.Items.Add(new ListItem("100", "100"));
                ddlPageSize.SelectedValue = "25";
            }
            catch (Exception ex)
            {
                ShowNotification($"Error initializing dropdowns: {ex.Message}", "error");
            }
        }

        private void InitializeBulkActionDropdown()
        {
            try
            {
                ddlBulkAction.Items.Clear();
                ddlBulkAction.Items.Add(new ListItem("Select Action", ""));
                ddlBulkAction.Items.Add(new ListItem("Complete Onboarding", "COMPLETE"));
                ddlBulkAction.Items.Add(new ListItem("Send Reminder", "REMIND"));
                ddlBulkAction.Items.Add(new ListItem("Export Details", "EXPORT"));
            }
            catch (Exception ex)
            {
                ShowNotification($"Error initializing bulk actions: {ex.Message}", "error");
            }
        }

        private void LoadDepartments()
        {
            try
            {
                ddlFilterDepartment.Items.Clear();
                ddlFilterDepartment.Items.Add(new ListItem("All Departments", ""));

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Use simple query instead of stored procedure for now
                    string sql = "SELECT Id, Name FROM Departments WHERE IsActive = 1 ORDER BY Name";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ddlFilterDepartment.Items.Add(new ListItem(
                                    reader["Name"].ToString(),
                                    reader["Id"].ToString()
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error loading departments: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in LoadDepartments: {ex}");
            }
        }

        private void SetDefaultFilters()
        {
            // Set default date range (last 6 months)
            txtFromDate.Text = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        #endregion

        #region Data Loading Methods

        private void LoadOnboardingData()
        {
            try
            {
                var employees = GetOnboardingEmployees();

                gvEmployees.DataSource = employees;
                gvEmployees.DataBind();

                // Update UI based on results
                UpdateGridVisibility(employees.Count > 0);
            }
            catch (Exception ex)
            {
                ShowNotification($"Error loading onboarding data: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in LoadOnboardingData: {ex}");
            }
        }

        private List<OnboardingEmployeeInfo> GetOnboardingEmployees()
        {
            var employees = new List<OnboardingEmployeeInfo>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Use simple SQL query for now (can replace with stored procedure later)
                    string sql = @"
                        SELECT TOP 50
                            e.[Id] AS EmployeeId,
                            e.[EmployeeNumber],
                            e.[FirstName] + ' ' + e.[LastName] AS FullName,
                            e.[Position],
                            e.[HireDate],
                            ISNULL(d.[Name], 'Unknown') AS DepartmentName,
                            COUNT(ot.[Id]) AS TotalTasks,
                            SUM(CASE WHEN ot.[Status] = 'COMPLETED' THEN 1 ELSE 0 END) AS CompletedTasks,
                            SUM(CASE WHEN ot.[Status] = 'PENDING' THEN 1 ELSE 0 END) AS PendingTasks,
                            SUM(CASE WHEN ot.[Status] = 'IN_PROGRESS' THEN 1 ELSE 0 END) AS InProgressTasks,
                            SUM(CASE WHEN ot.[Status] != 'COMPLETED' AND ot.[DueDate] < GETUTCDATE() THEN 1 ELSE 0 END) AS OverdueTasks,
                            CASE 
                                WHEN COUNT(ot.[Id]) > 0 THEN 
                                    CAST((SUM(CASE WHEN ot.[Status] = 'COMPLETED' THEN 1 ELSE 0 END) * 100.0) / COUNT(ot.[Id]) AS DECIMAL(5,2))
                                ELSE 0 
                            END AS CompletionPercentage,
                            CASE 
                                WHEN COUNT(ot.[Id]) = 0 THEN 'NO_TASKS'
                                WHEN SUM(CASE WHEN ot.[Status] = 'COMPLETED' THEN 1 ELSE 0 END) = COUNT(ot.[Id]) THEN 'COMPLETED'
                                WHEN SUM(CASE WHEN ot.[Status] != 'COMPLETED' AND ot.[DueDate] < GETUTCDATE() THEN 1 ELSE 0 END) > 0 THEN 'OVERDUE'
                                WHEN SUM(CASE WHEN ot.[Status] = 'COMPLETED' THEN 1 ELSE 0 END) > 0 THEN 'IN_PROGRESS'
                                ELSE 'PENDING'
                            END AS OnboardingStatus,
                            DATEDIFF(day, e.[HireDate], GETUTCDATE()) AS DaysSinceHire
                        FROM [dbo].[Employees] e
                        LEFT JOIN [dbo].[Departments] d ON e.[DepartmentId] = d.[Id]
                        LEFT JOIN [dbo].[OnboardingTasks] ot ON e.[Id] = ot.[EmployeeId] AND ot.[IsTemplate] = 0
                        WHERE e.[HireDate] >= DATEADD(year, -1, GETUTCDATE())
                            AND (e.[Status] IS NULL OR e.[Status] != 'Terminated')
                        GROUP BY e.[Id], e.[EmployeeNumber], e.[FirstName], e.[LastName], e.[Position], e.[HireDate], d.[Name]
                        ORDER BY e.[HireDate] DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var employee = new OnboardingEmployeeInfo
                                {
                                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                    EmployeeNumber = reader["EmployeeNumber"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    Position = reader["Position"].ToString(),
                                    HireDate = Convert.ToDateTime(reader["HireDate"]),
                                    DepartmentName = reader["DepartmentName"]?.ToString() ?? "Unknown",
                                    TotalTasks = Convert.ToInt32(reader["TotalTasks"]),
                                    CompletedTasks = Convert.ToInt32(reader["CompletedTasks"]),
                                    PendingTasks = Convert.ToInt32(reader["PendingTasks"]),
                                    InProgressTasks = Convert.ToInt32(reader["InProgressTasks"]),
                                    OverdueTasks = Convert.ToInt32(reader["OverdueTasks"]),
                                    CompletionPercentage = Convert.ToDecimal(reader["CompletionPercentage"]),
                                    OnboardingStatus = reader["OnboardingStatus"].ToString(),
                                    DaysSinceHire = Convert.ToInt32(reader["DaysSinceHire"])
                                };

                                employees.Add(employee);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetOnboardingEmployees: {ex}");
                ShowNotification($"Error loading employee data: {ex.Message}", "error");
            }

            return employees;
        }

        private void LoadStatistics()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        WITH EmployeeStats AS (
                            SELECT 
                                e.[Id],
                                COUNT(ot.[Id]) AS TotalTasks,
                                SUM(CASE WHEN ot.[Status] = 'COMPLETED' THEN 1 ELSE 0 END) AS CompletedTasks,
                                SUM(CASE WHEN ot.[Status] != 'COMPLETED' AND ot.[DueDate] < GETUTCDATE() THEN 1 ELSE 0 END) AS OverdueTasks,
                                CASE 
                                    WHEN COUNT(ot.[Id]) = 0 THEN 'NO_TASKS'
                                    WHEN SUM(CASE WHEN ot.[Status] = 'COMPLETED' THEN 1 ELSE 0 END) = COUNT(ot.[Id]) THEN 'COMPLETED'
                                    WHEN SUM(CASE WHEN ot.[Status] != 'COMPLETED' AND ot.[DueDate] < GETUTCDATE() THEN 1 ELSE 0 END) > 0 THEN 'OVERDUE'
                                    WHEN SUM(CASE WHEN ot.[Status] = 'COMPLETED' THEN 1 ELSE 0 END) > 0 THEN 'IN_PROGRESS'
                                    ELSE 'PENDING'
                                END AS OnboardingStatus
                            FROM [dbo].[Employees] e
                            LEFT JOIN [dbo].[OnboardingTasks] ot ON e.[Id] = ot.[EmployeeId] AND ot.[IsTemplate] = 0
                            WHERE e.[HireDate] >= DATEADD(year, -1, GETUTCDATE())
                                AND (e.[Status] IS NULL OR e.[Status] != 'Terminated')
                            GROUP BY e.[Id]
                        )
                        SELECT 
                            COUNT(*) AS TotalEmployees,
                            SUM(CASE WHEN OnboardingStatus = 'COMPLETED' THEN 1 ELSE 0 END) AS CompletedEmployees,
                            SUM(CASE WHEN OnboardingStatus = 'IN_PROGRESS' THEN 1 ELSE 0 END) AS InProgressEmployees,
                            SUM(CASE WHEN OnboardingStatus = 'OVERDUE' THEN 1 ELSE 0 END) AS OverdueEmployees
                        FROM EmployeeStats";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litTotalEmployees.Text = reader["TotalEmployees"].ToString();
                                litCompletedEmployees.Text = reader["CompletedEmployees"].ToString();
                                litInProgressEmployees.Text = reader["InProgressEmployees"].ToString();
                                litOverdueEmployees.Text = reader["OverdueEmployees"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadStatistics: {ex}");
                ShowNotification($"Error loading statistics: {ex.Message}", "error");
            }
        }

        private void UpdateGridVisibility(bool hasData)
        {
            if (pnlEmployeeGrid != null)
            {
                pnlEmployeeGrid.Visible = hasData;
            }
        }

        #endregion

        #region Grid Events

        protected void gvEmployees_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEmployees.PageIndex = e.NewPageIndex;
            LoadOnboardingData();
        }

        protected void gvEmployees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int employeeId = Convert.ToInt32(e.CommandArgument);

                switch (e.CommandName)
                {
                    case "ManageOnboarding":
                        Response.Redirect($"~/HR/ManageEmployeeOnboarding.aspx?employeeId={employeeId}");
                        break;

                    case "ViewDetails":
                        Response.Redirect($"~/HR/EmployeeDetails.aspx?id={employeeId}");
                        break;

                    case "QuickComplete":
                        CompleteEmployeeOnboarding(employeeId);
                        LoadOnboardingData(); // Refresh grid
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error processing command: {ex.Message}", "error");
            }
        }

        protected void gvEmployees_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var employee = (OnboardingEmployeeInfo)e.Row.DataItem;

                // Highlight overdue rows
                if (employee.OverdueTasks > 0)
                {
                    e.Row.CssClass += " row-overdue";
                }
            }
        }

        #endregion

        #region Button Events

        protected void btnExportReport_Click(object sender, EventArgs e)
        {
            try
            {
                ExportOnboardingReport();
            }
            catch (Exception ex)
            {
                ShowNotification($"Error exporting report: {ex.Message}", "error");
            }
        }

        protected void btnBulkActions_Click(object sender, EventArgs e)
        {
            try
            {
                if (pnlQuickActions != null)
                {
                    pnlQuickActions.Visible = !pnlQuickActions.Visible;
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error showing bulk actions: {ex.Message}", "error");
            }
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            try
            {
                // Reset all filters
                txtSearch.Text = "";
                ddlFilterDepartment.SelectedIndex = 0;
                ddlFilterStatus.SelectedIndex = 0;
                SetDefaultFilters();

                // Reload data
                LoadOnboardingData();

                ShowNotification("Filters cleared", "info");
            }
            catch (Exception ex)
            {
                ShowNotification($"Error clearing filters: {ex.Message}", "error");
            }
        }

        protected void btnExecuteBulkAction_Click(object sender, EventArgs e)
        {
            try
            {
                ExecuteBulkAction();
            }
            catch (Exception ex)
            {
                ShowNotification($"Error executing bulk action: {ex.Message}", "error");
            }
        }

        protected void btnCloseBulkActions_Click(object sender, EventArgs e)
        {
            if (pnlQuickActions != null)
            {
                pnlQuickActions.Visible = false;
            }
        }

        protected void btnCloseNotification_Click(object sender, EventArgs e)
        {
            if (pnlNotification != null)
            {
                pnlNotification.Visible = false;
            }
        }

        #endregion

        #region Helper Methods

        private void CompleteEmployeeOnboarding(int employeeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Simple update query (can replace with stored procedure later)
                    string sql = @"
                        UPDATE [dbo].[OnboardingTasks] 
                        SET [Status] = 'COMPLETED',
                            [CompletedDate] = GETUTCDATE(),
                            [Notes] = ISNULL([Notes], '') + ' [COMPLETED BY HR]'
                        WHERE [EmployeeId] = @EmployeeId 
                            AND [Status] != 'COMPLETED' 
                            AND [IsTemplate] = 0";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        ShowNotification($"Completed {rowsAffected} tasks for employee", "success");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to complete onboarding: {ex.Message}");
            }
        }

        private void ExportOnboardingReport()
        {
            var employees = GetOnboardingEmployees();

            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head><title>Onboarding Report</title>");
            html.AppendLine("<style>table { border-collapse: collapse; width: 100%; } th, td { border: 1px solid #ddd; padding: 8px; }</style>");
            html.AppendLine("</head><body>");
            html.AppendLine("<h1>Employee Onboarding Report</h1>");
            html.AppendLine($"<p>Generated: {DateTime.Now:yyyy-MM-dd HH:mm}</p>");

            html.AppendLine("<table>");
            html.AppendLine("<tr><th>Employee #</th><th>Name</th><th>Position</th><th>Hire Date</th><th>Department</th><th>Progress</th><th>Status</th></tr>");

            foreach (var emp in employees)
            {
                html.AppendLine("<tr>");
                html.AppendLine($"<td>{emp.EmployeeNumber}</td>");
                html.AppendLine($"<td>{emp.FullName}</td>");
                html.AppendLine($"<td>{emp.Position}</td>");
                html.AppendLine($"<td>{emp.HireDate:yyyy-MM-dd}</td>");
                html.AppendLine($"<td>{emp.DepartmentName}</td>");
                html.AppendLine($"<td>{emp.CompletionPercentage:F1}%</td>");
                html.AppendLine($"<td>{emp.OnboardingStatus}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table></body></html>");

            string fileName = $"OnboardingReport_{DateTime.Now:yyyyMMdd}.html";
            Response.ContentType = "text/html";
            Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
            Response.Write(html.ToString());
            Response.End();
        }

        private void ExecuteBulkAction()
        {
            string action = ddlBulkAction.SelectedValue;
            if (string.IsNullOrEmpty(action))
            {
                ShowNotification("Please select an action", "warning");
                return;
            }

            // Get selected employees (this would need checkboxes in the grid)
            var selectedEmployees = new List<int>();

            // For now, just show a message
            ShowNotification($"Bulk action '{action}' would be executed for selected employees", "info");
        }

        private void ShowNotification(string message, string type)
        {
            try
            {
                if (pnlNotification != null && litNotificationMessage != null)
                {
                    pnlNotification.Visible = true;
                    litNotificationMessage.Text = message;
                    pnlNotification.CssClass = $"notification {type}";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing notification: {ex}");
            }
        }

        #endregion

        #region Helper Methods for Display (Referenced in ASPX)

        protected string GetStatusClass(string status)
        {
            switch (status?.ToUpper())
            {
                case "COMPLETED": return "completed";
                case "IN_PROGRESS": return "in-progress";
                case "PENDING": return "pending";
                case "OVERDUE": return "overdue";
                case "NO_TASKS": return "no-tasks";
                default: return "pending";
            }
        }

        protected string GetStatusDisplay(string status)
        {
            switch (status?.ToUpper())
            {
                case "COMPLETED": return "Completed";
                case "IN_PROGRESS": return "In Progress";
                case "PENDING": return "Pending";
                case "OVERDUE": return "Overdue";
                case "NO_TASKS": return "No Tasks";
                default: return "Unknown";
            }
        }

        #endregion
    }

    #region Data Models - Serializable for ViewState

    [Serializable]
    public class OnboardingEmployeeInfo
    {
        public int EmployeeId { get; set; }
        public string EmployeeNumber { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public DateTime HireDate { get; set; }
        public string DepartmentName { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int OverdueTasks { get; set; }
        public decimal CompletionPercentage { get; set; }
        public string OnboardingStatus { get; set; }
        public int DaysSinceHire { get; set; }
    }

    #endregion
}