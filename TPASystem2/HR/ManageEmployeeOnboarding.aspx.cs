using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;

namespace TPASystem2.HR
{
    public partial class ManageEmployeeOnboarding : System.Web.UI.Page
    {
        #region Properties and Fields

        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // Simple properties without ViewState storage to avoid serialization issues
        private int EmployeeId
        {
            get
            {
                if (Request.QueryString["employeeId"] != null && int.TryParse(Request.QueryString["employeeId"], out int id))
                {
                    return id;
                }
                return 0;
            }
        }

        #endregion

        #region Additional Event Handlers (Missing from .aspx file)

        protected void ddlTaskFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadOnboardingTasks();
            }
            catch (Exception ex)
            {
                ShowNotification($"Error filtering tasks: {ex.Message}", "error");
            }
        }

        protected void ddlCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadOnboardingTasks();
            }
            catch (Exception ex)
            {
                ShowNotification($"Error filtering by category: {ex.Message}", "error");
            }
        }

        protected void ddlPriorityFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadOnboardingTasks();
            }
            catch (Exception ex)
            {
                ShowNotification($"Error filtering by priority: {ex.Message}", "error");
            }
        }

        protected void btnClearTaskFilters_Click(object sender, EventArgs e)
        {
            try
            {
                if (ddlTaskFilter != null) ddlTaskFilter.SelectedIndex = 0;
                if (ddlCategoryFilter != null) ddlCategoryFilter.SelectedIndex = 0;
                if (ddlPriorityFilter != null) ddlPriorityFilter.SelectedIndex = 0;
                LoadOnboardingTasks();
            }
            catch (Exception ex)
            {
                ShowNotification($"Error clearing filters: {ex.Message}", "error");
            }
        }

        protected void rptTasks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                int taskId = Convert.ToInt32(e.CommandArgument);

                switch (e.CommandName)
                {
                    case "CompleteTask":
                        CompleteTask(taskId);
                        LoadOnboardingTasks();
                        UpdateProgressDisplay();
                        break;

                    case "ViewDetails":
                        ShowTaskDetails(taskId);
                        break;

                    case "EditTask":
                        EditTask(taskId);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error processing task command: {ex.Message}", "error");
            }
        }

        protected void rptTasks_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var task = (OnboardingTaskInfo)e.Item.DataItem;

                // Additional binding logic if needed
                // For example, conditional visibility based on task properties
            }
        }

        #endregion

        #region Additional Helper Methods for .aspx Display

        protected string GetTaskCardClass(string status, object isOverdue)
        {
            var overdue = Convert.ToBoolean(isOverdue);
            if (overdue) return "task-card-overdue";

            switch (status?.ToUpper())
            {
                case "COMPLETED": return "task-card-completed";
                case "IN_PROGRESS": return "task-card-in-progress";
                case "PENDING": return "task-card-pending";
                default: return "task-card-pending";
            }
        }

        protected string GetCategoryDisplay(string category)
        {
            switch (category?.ToUpper())
            {
                case "DOCUMENTATION": return "Documentation";
                case "SETUP": return "Setup";
                case "ORIENTATION": return "Orientation";
                case "TRAINING": return "Training";
                case "MEETING": return "Meeting";
                case "EQUIPMENT": return "Equipment";
                default: return category ?? "General";
            }
        }

        private void CompleteTask(int taskId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        UPDATE [dbo].[OnboardingTasks] 
                        SET [Status] = 'COMPLETED',
                            [CompletedDate] = GETUTCDATE(),
                            [Notes] = ISNULL([Notes], '') + ' [COMPLETED]'
                        WHERE [Id] = @TaskId";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@TaskId", taskId);
                        cmd.ExecuteNonQuery();
                    }
                }

                ShowNotification("Task completed successfully!", "success");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to complete task: {ex.Message}");
            }
        }

        private void ShowTaskDetails(int taskId)
        {
            try
            {
                // Implement task details modal if needed
                ShowNotification("Task details functionality to be implemented", "info");
            }
            catch (Exception ex)
            {
                ShowNotification($"Error showing task details: {ex.Message}", "error");
            }
        }

        private void EditTask(int taskId)
        {
            try
            {
                // Implement task editing if needed
                ShowNotification("Task editing functionality to be implemented", "info");
            }
            catch (Exception ex)
            {
                ShowNotification($"Error editing task: {ex.Message}", "error");
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set UnobtrusiveValidationMode to prevent validation errors
                Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

                InitializePage();

                if (EmployeeId > 0)
                {
                    LoadEmployeeInfo();
                    LoadOnboardingTasks();
                    UpdateProgressDisplay();
                }
                else
                {
                    Response.Redirect("~/HR/OnboardingManagement.aspx");
                }
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
        }

        #endregion

        #region Data Loading Methods

        private void LoadEmployeeInfo()
        {
            try
            {
                var employee = GetEmployeeInfo();
                if (employee != null)
                {
                    UpdateEmployeeDisplay(employee);
                }
                else
                {
                    ShowNotification("Employee not found.", "error");
                    Response.Redirect("~/HR/OnboardingManagement.aspx");
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error loading employee information: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in LoadEmployeeInfo: {ex}");
            }
        }

        private EmployeeOnboardingInfo GetEmployeeInfo()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        SELECT 
                            e.[Id] AS EmployeeId,
                            e.[EmployeeNumber],
                            e.[FirstName],
                            e.[LastName],
                            e.[FirstName] + ' ' + e.[LastName] AS FullName,
                            e.[Email],
                            e.[Position],
                            e.[HireDate],
                            ISNULL(e.[Status], 'Active') AS Status,
                            ISNULL(d.[Name], 'Unknown') AS DepartmentName,
                            CASE 
                                WHEN e.[ManagerId] IS NOT NULL THEN m.[FirstName] + ' ' + m.[LastName]
                                ELSE 'No Manager Assigned'
                            END AS ManagerName
                        FROM [dbo].[Employees] e
                        LEFT JOIN [dbo].[Departments] d ON e.[DepartmentId] = d.[Id]
                        LEFT JOIN [dbo].[Employees] m ON e.[ManagerId] = m.[Id]
                        WHERE e.[Id] = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new EmployeeOnboardingInfo
                                {
                                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                    EmployeeNumber = reader["EmployeeNumber"].ToString(),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Position = reader["Position"].ToString(),
                                    HireDate = Convert.ToDateTime(reader["HireDate"]),
                                    Status = reader["Status"]?.ToString() ?? "Unknown",
                                    DepartmentName = reader["DepartmentName"]?.ToString() ?? "Unknown",
                                    ManagerName = reader["ManagerName"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetEmployeeInfo: {ex}");
            }
            return null;
        }

        private void UpdateEmployeeDisplay(EmployeeOnboardingInfo employee)
        {
            try
            {
                if (litEmployeeName != null) litEmployeeName.Text = employee.FullName;
                if (litEmployeeNumber != null) litEmployeeNumber.Text = employee.EmployeeNumber;
                if (litHireDate != null) litHireDate.Text = employee.HireDate.ToString("MMM dd, yyyy");
                if (litDepartment != null) litDepartment.Text = employee.DepartmentName;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UpdateEmployeeDisplay: {ex}");
            }
        }

        private void LoadOnboardingTasks()
        {
            try
            {
                var tasks = GetEmployeeTasks();

                // Apply current filters if needed
                var filteredTasks = ApplyTaskFilters(tasks);

                // Bind to repeater if it exists
                if (rptTasks != null)
                {
                    rptTasks.DataSource = filteredTasks;
                    rptTasks.DataBind();
                }

                // Update empty state if panel exists
                if (pnlEmptyTasks != null)
                {
                    pnlEmptyTasks.Visible = filteredTasks.Count == 0;
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error loading onboarding tasks: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in LoadOnboardingTasks: {ex}");
            }
        }

        private List<OnboardingTaskInfo> GetEmployeeTasks()
        {
            var tasks = new List<OnboardingTaskInfo>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        SELECT 
                            ot.[Id] AS TaskId,
                            ot.[Title],
                            ot.[Description],
                            ot.[Category],
                            ot.[Status],
                            ot.[Priority],
                            ot.[DueDate],
                            ot.[EstimatedTime],
                            ot.[Instructions],
                            ot.[CreatedDate],
                            ot.[CompletedDate],
                            ot.[Notes],
                            ot.[AssignedToRole],
                            ISNULL(ot.[CanEmployeeComplete], 0) AS CanEmployeeComplete,
                            ISNULL(ot.[BlocksSystemAccess], 0) AS BlocksSystemAccess,
                            ot.[SortOrder],
                            CASE 
                                WHEN ot.[CompletedByUserId] IS NOT NULL THEN 
                                    ISNULL(ce.[FirstName] + ' ' + ce.[LastName], 'System')
                                ELSE NULL
                            END AS CompletedByName,
                            0 AS DocumentCount,
                            0 AS CommentCount,
                            CASE 
                                WHEN ot.[Status] != 'COMPLETED' AND ot.[DueDate] < GETUTCDATE() THEN 1
                                ELSE 0
                            END AS IsOverdue
                        FROM [dbo].[OnboardingTasks] ot
                        LEFT JOIN [dbo].[Users] cu ON ot.[CompletedByUserId] = cu.[Id]
                        LEFT JOIN [dbo].[Employees] ce ON cu.[Id] = ce.[UserId]
                        WHERE ot.[EmployeeId] = @EmployeeId
                            AND ot.[IsTemplate] = 0
                        ORDER BY 
                            CASE ot.[Priority]
                                WHEN 'HIGH' THEN 1
                                WHEN 'MEDIUM' THEN 2
                                WHEN 'LOW' THEN 3
                                ELSE 4
                            END,
                            ISNULL(ot.[SortOrder], 999),
                            ot.[DueDate]";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var task = new OnboardingTaskInfo
                                {
                                    TaskId = Convert.ToInt32(reader["TaskId"]),
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"]?.ToString(),
                                    Category = reader["Category"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    Priority = reader["Priority"].ToString(),
                                    DueDate = Convert.ToDateTime(reader["DueDate"]),
                                    EstimatedTime = reader["EstimatedTime"]?.ToString(),
                                    Instructions = reader["Instructions"]?.ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CompletedDate = reader["CompletedDate"] as DateTime?,
                                    Notes = reader["Notes"]?.ToString(),
                                    AssignedToRole = reader["AssignedToRole"]?.ToString(),
                                    CanEmployeeComplete = Convert.ToBoolean(reader["CanEmployeeComplete"]),
                                    BlocksSystemAccess = Convert.ToBoolean(reader["BlocksSystemAccess"]),
                                    SortOrder = reader["SortOrder"] as int?,
                                    CompletedByName = reader["CompletedByName"]?.ToString(),
                                    DocumentCount = Convert.ToInt32(reader["DocumentCount"]),
                                    CommentCount = Convert.ToInt32(reader["CommentCount"]),
                                    IsOverdue = Convert.ToBoolean(reader["IsOverdue"])
                                };

                                tasks.Add(task);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetEmployeeTasks: {ex}");
            }

            return tasks;
        }

        private List<OnboardingTaskInfo> ApplyTaskFilters(List<OnboardingTaskInfo> tasks)
        {
            try
            {
                var filteredTasks = tasks.AsQueryable();

                // Status filter
                if (ddlTaskFilter != null && !string.IsNullOrEmpty(ddlTaskFilter.SelectedValue))
                {
                    if (ddlTaskFilter.SelectedValue == "OVERDUE")
                    {
                        filteredTasks = filteredTasks.Where(t => t.IsOverdue);
                    }
                    else
                    {
                        filteredTasks = filteredTasks.Where(t => t.Status == ddlTaskFilter.SelectedValue);
                    }
                }

                // Category filter
                if (ddlCategoryFilter != null && !string.IsNullOrEmpty(ddlCategoryFilter.SelectedValue))
                {
                    filteredTasks = filteredTasks.Where(t => t.Category == ddlCategoryFilter.SelectedValue);
                }

                // Priority filter  
                if (ddlPriorityFilter != null && !string.IsNullOrEmpty(ddlPriorityFilter.SelectedValue))
                {
                    filteredTasks = filteredTasks.Where(t => t.Priority == ddlPriorityFilter.SelectedValue);
                }

                return filteredTasks.OrderBy(t => t.SortOrder ?? 999)
                                    .ThenBy(t => t.DueDate)
                                    .ThenByDescending(t => t.Priority == "HIGH" ? 3 : t.Priority == "MEDIUM" ? 2 : 1)
                                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ApplyTaskFilters: {ex}");
                return tasks; // Return original list if filtering fails
            }
        }

        private void UpdateProgressDisplay()
        {
            try
            {
                var tasks = GetEmployeeTasks();

                if (tasks.Count > 0)
                {
                    var completedTasks = tasks.Count(t => t.Status == "COMPLETED");
                    var totalTasks = tasks.Count;
                    var progressPercentage = (completedTasks * 100.0) / totalTasks;

                    // Update progress display controls if they exist
                    // if (litProgressPercentage != null) litProgressPercentage.Text = $"{progressPercentage:F1}%";
                    // if (litCompletedTasks != null) litCompletedTasks.Text = completedTasks.ToString();
                    // if (litTotalTasks != null) litTotalTasks.Text = totalTasks.ToString();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UpdateProgressDisplay: {ex}");
            }
        }

        #endregion

        #region Event Handlers

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/OnboardingManagement.aspx");
        }

        protected void btnAddTask_Click(object sender, EventArgs e)
        {
            // Implement add task functionality
            ShowNotification("Add task functionality to be implemented", "info");
        }

        protected void btnCompleteAll_Click(object sender, EventArgs e)
        {
            try
            {
                CompleteAllTasks();
                LoadOnboardingTasks(); // Refresh the task list
                UpdateProgressDisplay(); // Update progress
                ShowNotification("All tasks completed successfully!", "success");
            }
            catch (Exception ex)
            {
                ShowNotification($"Error completing tasks: {ex.Message}", "error");
            }
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateEmployeeOnboardingReport();
            }
            catch (Exception ex)
            {
                ShowNotification($"Error generating report: {ex.Message}", "error");
            }
        }

        protected void btnCloseAddTaskModal_Click(object sender, EventArgs e)
        {
            try
            {
                // Close the add task modal if it exists
                if (pnlAddTaskModal != null)
                {
                    pnlAddTaskModal.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error closing modal: {ex.Message}", "error");
            }
        }

        protected void btnSaveTask_Click(object sender, EventArgs e)
        {
            try
            {
                // Save custom task functionality - placeholder for now
                ShowNotification("Save task functionality to be implemented", "info");

                if (pnlAddTaskModal != null)
                {
                    pnlAddTaskModal.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error saving task: {ex.Message}", "error");
            }
        }

        protected void btnCancelAddTask_Click(object sender, EventArgs e)
        {
            try
            {
                // Cancel add task and close modal
                if (pnlAddTaskModal != null)
                {
                    pnlAddTaskModal.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error canceling task: {ex.Message}", "error");
            }
        }

        protected void btnCloseTaskDetailsModal_Click(object sender, EventArgs e)
        {
            try
            {
                // Close task details modal if it exists
                if (pnlTaskDetailsModal != null)
                {
                    pnlTaskDetailsModal.Visible = false;
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error closing task details: {ex.Message}", "error");
            }
        }

        protected void btnCloseNotification_Click(object sender, EventArgs e)
        {
            try
            {
                if (pnlNotification != null)
                {
                    pnlNotification.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error closing notification: {ex}");
            }
        }

        #endregion

        #region Helper Methods

        private void CompleteAllTasks()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sql = @"
                        UPDATE [dbo].[OnboardingTasks] 
                        SET [Status] = 'COMPLETED',
                            [CompletedDate] = GETUTCDATE(),
                            [Notes] = ISNULL([Notes], '') + ' [BULK COMPLETED BY HR]'
                        WHERE [EmployeeId] = @EmployeeId 
                            AND [Status] != 'COMPLETED' 
                            AND [IsTemplate] = 0";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmd.Parameters.AddWithValue("@CompletedByUserId", Session["UserId"] ?? (object)DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new Exception("No tasks were updated. Tasks may already be completed.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to complete all tasks: {ex.Message}");
            }
        }

        private void GenerateEmployeeOnboardingReport()
        {
            var employee = GetEmployeeInfo();
            var tasks = GetEmployeeTasks();

            if (employee == null)
            {
                throw new Exception("Employee information not found");
            }

            var html = GenerateOnboardingReportHtml(employee, tasks);

            string fileName = $"OnboardingReport_{employee.EmployeeNumber}_{DateTime.Now:yyyyMMdd}.html";
            Response.ContentType = "text/html";
            Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
            Response.Write(html);
            Response.End();
        }

        private string GenerateOnboardingReportHtml(EmployeeOnboardingInfo employee, List<OnboardingTaskInfo> tasks)
        {
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head>");
            html.AppendLine("<title>Employee Onboarding Report</title>");
            html.AppendLine("<style>");
            html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
            html.AppendLine("table { border-collapse: collapse; width: 100%; margin: 20px 0; }");
            html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
            html.AppendLine("th { background-color: #f2f2f2; }");
            html.AppendLine(".completed { background-color: #d4edda; }");
            html.AppendLine(".pending { background-color: #fff3cd; }");
            html.AppendLine(".overdue { background-color: #f8d7da; }");
            html.AppendLine("</style>");
            html.AppendLine("</head><body>");

            // Employee info
            html.AppendLine($"<h1>Onboarding Report - {employee.FullName}</h1>");
            html.AppendLine($"<p><strong>Employee Number:</strong> {employee.EmployeeNumber}</p>");
            html.AppendLine($"<p><strong>Position:</strong> {employee.Position}</p>");
            html.AppendLine($"<p><strong>Department:</strong> {employee.DepartmentName}</p>");
            html.AppendLine($"<p><strong>Hire Date:</strong> {employee.HireDate:yyyy-MM-dd}</p>");
            html.AppendLine($"<p><strong>Report Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm}</p>");

            // Task summary
            var completedTasks = tasks.Count(t => t.Status == "COMPLETED");
            var totalTasks = tasks.Count;
            var progressPercentage = totalTasks > 0 ? (completedTasks * 100.0) / totalTasks : 0;

            html.AppendLine("<h2>Progress Summary</h2>");
            html.AppendLine($"<p><strong>Total Tasks:</strong> {totalTasks}</p>");
            html.AppendLine($"<p><strong>Completed Tasks:</strong> {completedTasks}</p>");
            html.AppendLine($"<p><strong>Progress:</strong> {progressPercentage:F1}%</p>");

            // Task details
            html.AppendLine("<h2>Task Details</h2>");
            html.AppendLine("<table>");
            html.AppendLine("<tr><th>Task</th><th>Category</th><th>Status</th><th>Due Date</th><th>Priority</th></tr>");

            foreach (var task in tasks.OrderBy(t => t.DueDate))
            {
                string rowClass = task.Status.ToUpper() == "COMPLETED" ? "completed" :
                                 task.IsOverdue ? "overdue" : "pending";

                html.AppendLine($"<tr class='{rowClass}'>");
                html.AppendLine($"<td>{task.Title}</td>");
                html.AppendLine($"<td>{task.Category}</td>");
                html.AppendLine($"<td>{task.Status}</td>");
                html.AppendLine($"<td>{task.DueDate:yyyy-MM-dd}</td>");
                html.AppendLine($"<td>{task.Priority}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
            html.AppendLine("</body></html>");

            return html.ToString();
        }

        private void ShowNotification(string message, string type)
        {
            // Implement notification display
            // This would typically update a notification panel or use JavaScript
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

        #region Helper Methods for Page Display

        protected string GetStatusClass(string status)
        {
            switch (status?.ToUpper())
            {
                case "COMPLETED": return "completed";
                case "IN_PROGRESS": return "in-progress";
                case "PENDING": return "pending";
                case "OVERDUE": return "overdue";
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
                default: return "Unknown";
            }
        }

        protected string GetPriorityClass(string priority)
        {
            switch (priority?.ToUpper())
            {
                case "HIGH": return "priority-high";
                case "MEDIUM": return "priority-medium";
                case "LOW": return "priority-low";
                default: return "priority-normal";
            }
        }

        protected string GetAssignedToDisplay(string role)
        {
            switch (role?.ToUpper())
            {
                case "HR": return "HR Department";
                case "MANAGER": return "Direct Manager";
                case "EMPLOYEE": return "Employee (Self)";
                case "IT": return "IT Department";
                case "FACILITIES": return "Facilities";
                case "FINANCE": return "Finance Department";
                default: return role ?? "Unassigned";
            }
        }

        #endregion
    }

    #region Data Models - Serializable

    [Serializable]
    public class EmployeeOnboardingInfo
    {
        public int EmployeeId { get; set; }
        public string EmployeeNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public DateTime HireDate { get; set; }
        public string Status { get; set; }
        public string DepartmentName { get; set; }
        public string ManagerName { get; set; }
    }

    [Serializable]
    public class OnboardingTaskInfo
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime DueDate { get; set; }
        public string EstimatedTime { get; set; }
        public string Instructions { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Notes { get; set; }
        public string AssignedToRole { get; set; }
        public bool CanEmployeeComplete { get; set; }
        public bool BlocksSystemAccess { get; set; }
        public int? SortOrder { get; set; }
        public string CompletedByName { get; set; }
        public int DocumentCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsOverdue { get; set; }
    }

    #endregion
}