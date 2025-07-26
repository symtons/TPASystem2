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

        private int EmployeeId
        {
            get
            {
                if (ViewState["EmployeeId"] != null)
                    return (int)ViewState["EmployeeId"];

                if (Request.QueryString["employeeId"] != null && int.TryParse(Request.QueryString["employeeId"], out int id))
                {
                    ViewState["EmployeeId"] = id;
                    return id;
                }

                return 0;
            }
            set { ViewState["EmployeeId"] = value; }
        }

        private List<OnboardingTaskInfo> CurrentTaskList
        {
            get { return ViewState["CurrentTaskList"] as List<OnboardingTaskInfo> ?? new List<OnboardingTaskInfo>(); }
            set { ViewState["CurrentTaskList"] = value; }
        }

        private EmployeeOnboardingInfo CurrentEmployee
        {
            get { return ViewState["CurrentEmployee"] as EmployeeOnboardingInfo; }
            set { ViewState["CurrentEmployee"] = value; }
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
            // Check user permissions
            if (Session["UserRole"] == null || Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            string userRole = Session["UserRole"].ToString();
            if (userRole != "HR" && userRole != "ADMIN" && userRole != "HRDIRECTOR")
            {
                Response.Redirect("~/Dashboard.aspx");
                return;
            }

            // Set page title
            Page.Title = "Manage Employee Onboarding - TPA HR System";

            // Set default due date for new tasks (7 days from now)
            txtTaskDueDate.Text = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
        }

        private void LoadEmployeeInfo()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeOnboardingDetails", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var employee = new EmployeeOnboardingInfo
                                {
                                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                    EmployeeNumber = reader["EmployeeNumber"].ToString(),
                                    FirstName = reader["FirstName"].ToString(),
                                    LastName = reader["LastName"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Position = reader["Position"].ToString(),
                                    HireDate = Convert.ToDateTime(reader["HireDate"]),
                                    Status = reader["Status"].ToString(),
                                    DepartmentName = reader["DepartmentName"]?.ToString() ?? "Unknown",
                                    ManagerName = reader["ManagerName"]?.ToString()
                                };

                                CurrentEmployee = employee;
                                UpdateEmployeeDisplay(employee);
                            }
                            else
                            {
                                ShowNotification("Employee not found.", "error");
                                Response.Redirect("~/HR/OnboardingManagement.aspx");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error loading employee information: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in LoadEmployeeInfo: {ex}");
            }
        }

        private void UpdateEmployeeDisplay(EmployeeOnboardingInfo employee)
        {
            litEmployeeName.Text = employee.FullName;
            litEmployeeNumber.Text = employee.EmployeeNumber;
            litHireDate.Text = employee.HireDate.ToString("MMM dd, yyyy");
            litDepartment.Text = employee.DepartmentName;
        }

        #endregion

        #region Data Loading Methods

        private void LoadOnboardingTasks()
        {
            try
            {
                var tasks = GetEmployeeTasks();
                CurrentTaskList = tasks;

                // Apply current filters
                var filteredTasks = ApplyTaskFilters(tasks);

                rptTasks.DataSource = filteredTasks;
                rptTasks.DataBind();

                // Update empty state
                pnlEmptyTasks.Visible = filteredTasks.Count == 0;
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
                    using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeOnboardingDetails", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // Skip employee info (first result set)
                            if (reader.Read()) { }

                            // Move to task details (second result set)
                            if (reader.NextResult())
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetEmployeeTasks: {ex}");
                throw;
            }

            return tasks;
        }

        private List<OnboardingTaskInfo> ApplyTaskFilters(List<OnboardingTaskInfo> tasks)
        {
            var filteredTasks = tasks.AsQueryable();

            // Status filter
            if (!string.IsNullOrEmpty(ddlTaskFilter.SelectedValue))
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
            if (!string.IsNullOrEmpty(ddlCategoryFilter.SelectedValue))
            {
                filteredTasks = filteredTasks.Where(t => t.Category == ddlCategoryFilter.SelectedValue);
            }

            // Priority filter
            if (!string.IsNullOrEmpty(ddlPriorityFilter.SelectedValue))
            {
                filteredTasks = filteredTasks.Where(t => t.Priority == ddlPriorityFilter.SelectedValue);
            }

            return filteredTasks.OrderBy(t => t.SortOrder ?? 999)
                                .ThenBy(t => t.DueDate)
                                .ThenByDescending(t => t.Priority == "HIGH" ? 3 : t.Priority == "MEDIUM" ? 2 : 1)
                                .ToList();
        }

        private void UpdateProgressDisplay()
        {
            try
            {
                var tasks = CurrentTaskList;

                if (tasks.Count == 0)
                {
                    // No tasks assigned yet
                    litTotalTasks.Text = "0";
                    litCompletedTasks.Text = "0";
                    litInProgressTasks.Text = "0";
                    litPendingTasks.Text = "0";
                    litOverdueTasks.Text = "0";
                    litCompletionPercentage.Text = "0";
                    UpdateProgressRing(0);
                    return;
                }

                int totalTasks = tasks.Count;
                int completedTasks = tasks.Count(t => t.Status == "COMPLETED");
                int inProgressTasks = tasks.Count(t => t.Status == "IN_PROGRESS");
                int pendingTasks = tasks.Count(t => t.Status == "PENDING");
                int overdueTasks = tasks.Count(t => t.IsOverdue && t.Status != "COMPLETED");

                decimal completionPercentage = totalTasks > 0 ? (decimal)completedTasks / totalTasks * 100 : 0;

                litTotalTasks.Text = totalTasks.ToString();
                litCompletedTasks.Text = completedTasks.ToString();
                litInProgressTasks.Text = inProgressTasks.ToString();
                litPendingTasks.Text = pendingTasks.ToString();
                litOverdueTasks.Text = overdueTasks.ToString();
                litCompletionPercentage.Text = Math.Round(completionPercentage, 0).ToString();

                UpdateProgressRing(completionPercentage);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UpdateProgressDisplay: {ex}");
            }
        }

        private void UpdateProgressRing(decimal percentage)
        {
            // SVG circle circumference calculation: 2 * π * radius (radius = 52)
            double circumference = 2 * Math.PI * 52;
            double offset = circumference - (double)(percentage / 100) * circumference;

            litProgressCircumference.Text = circumference.ToString("F2");
            litProgressOffset.Text = offset.ToString("F2");
        }

        #endregion

        #region Event Handlers - Navigation

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/OnboardingManagement.aspx");
        }

        #endregion

        #region Event Handlers - Task Management

        protected void btnAddTask_Click(object sender, EventArgs e)
        {
            ClearTaskForm();
            pnlAddTaskModal.Visible = true;
        }

        protected void btnCompleteAll_Click(object sender, EventArgs e)
        {
            CompleteAllPendingTasks();
        }

        protected void rptTasks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int taskId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "EditTask":
                    EditTask(taskId);
                    break;

                case "MarkComplete":
                    UpdateTaskStatus(taskId, "COMPLETED");
                    break;

                case "MarkInProgress":
                    UpdateTaskStatus(taskId, "IN_PROGRESS");
                    break;

                case "DeleteTask":
                    DeleteTask(taskId);
                    break;

                case "ViewDocuments":
                    ViewTaskDocuments(taskId);
                    break;

                case "ViewComments":
                    ViewTaskComments(taskId);
                    break;

                case "TaskDetails":
                    ShowTaskDetails(taskId);
                    break;
            }
        }

        protected void rptTasks_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var task = (OnboardingTaskInfo)e.Item.DataItem;

                // You can add additional data binding logic here if needed
                // For example, conditional visibility based on task properties
            }
        }

        #endregion

        #region Event Handlers - Filters

        protected void ddlTaskFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadOnboardingTasks();
        }

        protected void ddlCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadOnboardingTasks();
        }

        protected void ddlPriorityFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadOnboardingTasks();
        }

        protected void btnClearTaskFilters_Click(object sender, EventArgs e)
        {
            ddlTaskFilter.SelectedIndex = 0;
            ddlCategoryFilter.SelectedIndex = 0;
            ddlPriorityFilter.SelectedIndex = 0;
            LoadOnboardingTasks();
        }

        #endregion

        #region Event Handlers - Modal Actions

        protected void btnSaveTask_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveCustomTask();
            }
        }

        protected void btnCancelAddTask_Click(object sender, EventArgs e)
        {
            pnlAddTaskModal.Visible = false;
        }

        protected void btnCloseAddTaskModal_Click(object sender, EventArgs e)
        {
            pnlAddTaskModal.Visible = false;
        }

        protected void btnCloseTaskDetailsModal_Click(object sender, EventArgs e)
        {
            pnlTaskDetailsModal.Visible = false;
        }

        protected void btnCloseNotification_Click(object sender, EventArgs e)
        {
            pnlNotification.Visible = false;
        }

        #endregion

        #region Event Handlers - Reports

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            GenerateEmployeeOnboardingReport();
        }

        #endregion

        #region Task Management Methods

        private void SaveCustomTask()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_AddCustomOnboardingTask", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmd.Parameters.AddWithValue("@Title", txtTaskTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@Description", txtTaskDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@Category", ddlTaskCategory.SelectedValue);
                        cmd.Parameters.AddWithValue("@Priority", ddlTaskPriority.SelectedValue);
                        cmd.Parameters.AddWithValue("@DueDate", Convert.ToDateTime(txtTaskDueDate.Text));
                        cmd.Parameters.AddWithValue("@EstimatedTime",
                            string.IsNullOrEmpty(txtEstimatedTime.Text) ? "TBD" : txtEstimatedTime.Text.Trim());
                        cmd.Parameters.AddWithValue("@Instructions", txtTaskInstructions.Text.Trim());
                        cmd.Parameters.AddWithValue("@AssignedToRole", ddlAssignedToRole.SelectedValue);
                        cmd.Parameters.AddWithValue("@CanEmployeeComplete", chkCanEmployeeComplete.Checked);
                        cmd.Parameters.AddWithValue("@BlocksSystemAccess", chkBlocksSystemAccess.Checked);
                        cmd.Parameters.AddWithValue("@CreatedById", Convert.ToInt32(Session["UserId"]));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string status = reader["Status"].ToString();
                                string message = reader["Message"].ToString();

                                if (status == "SUCCESS")
                                {
                                    ShowNotification("Custom task added successfully.", "success");
                                    pnlAddTaskModal.Visible = false;
                                    LoadOnboardingTasks();
                                    UpdateProgressDisplay();
                                }
                                else
                                {
                                    ShowNotification($"Error adding task: {message}", "error");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error saving custom task: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in SaveCustomTask: {ex}");
            }
        }

        private void UpdateTaskStatus(int taskId, string newStatus)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateTaskStatus", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TaskId", taskId);
                        cmd.Parameters.AddWithValue("@Status", newStatus);
                        cmd.Parameters.AddWithValue("@CompletedById",
                            newStatus == "COMPLETED" ? Convert.ToInt32(Session["UserId"]) : (object)DBNull.Value);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string status = reader["Status"].ToString();
                                string message = reader["Message"].ToString();

                                if (status == "SUCCESS")
                                {
                                    ShowNotification($"Task status updated to {newStatus.ToLower()}.", "success");
                                    LoadOnboardingTasks();
                                    UpdateProgressDisplay();
                                }
                                else
                                {
                                    ShowNotification($"Error updating task: {message}", "error");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error updating task status: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in UpdateTaskStatus: {ex}");
            }
        }

        private void CompleteAllPendingTasks()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                int completedCount = 0;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        UPDATE OnboardingTasks 
                        SET Status = 'COMPLETED', 
                            CompletedDate = GETUTCDATE(),
                            CompletedById = @UserId,
                            Notes = ISNULL(Notes, '') + ' [Bulk completed by HR on ' + FORMAT(GETUTCDATE(), 'yyyy-MM-dd HH:mm') + ']'
                        WHERE EmployeeId = @EmployeeId 
                        AND Status IN ('PENDING', 'IN_PROGRESS')
                        AND IsTemplate = 0", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        completedCount = cmd.ExecuteNonQuery();
                    }

                    // Update onboarding progress
                    if (completedCount > 0)
                    {
                        using (SqlCommand progressCmd = new SqlCommand("EXEC UpdateOnboardingProgress @EmployeeId", conn))
                        {
                            progressCmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            progressCmd.ExecuteNonQuery();
                        }
                    }
                }

                if (completedCount > 0)
                {
                    ShowNotification($"Successfully completed {completedCount} tasks.", "success");
                    LoadOnboardingTasks();
                    UpdateProgressDisplay();
                }
                else
                {
                    ShowNotification("No pending tasks found to complete.", "info");
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error completing tasks: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in CompleteAllPendingTasks: {ex}");
            }
        }

        private void DeleteTask(int taskId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        DELETE FROM OnboardingTasks 
                        WHERE Id = @TaskId 
                        AND EmployeeId = @EmployeeId 
                        AND IsTemplate = 0", conn))
                    {
                        cmd.Parameters.AddWithValue("@TaskId", taskId);
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                        int deletedRows = cmd.ExecuteNonQuery();

                        if (deletedRows > 0)
                        {
                            // Update onboarding progress
                            using (SqlCommand progressCmd = new SqlCommand("EXEC UpdateOnboardingProgress @EmployeeId", conn))
                            {
                                progressCmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                                progressCmd.ExecuteNonQuery();
                            }

                            ShowNotification("Task deleted successfully.", "success");
                            LoadOnboardingTasks();
                            UpdateProgressDisplay();
                        }
                        else
                        {
                            ShowNotification("Task not found or cannot be deleted.", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error deleting task: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in DeleteTask: {ex}");
            }
        }

        private void EditTask(int taskId)
        {
            // TODO: Implement task editing functionality
            // This would populate the form with existing task data for editing
            ShowNotification("Task editing functionality will be implemented in the next phase.", "info");
        }

        private void ViewTaskDocuments(int taskId)
        {
            // TODO: Implement document viewing functionality
            Response.Redirect($"~/HR/TaskDocuments.aspx?taskId={taskId}&employeeId={EmployeeId}");
        }

        private void ViewTaskComments(int taskId)
        {
            // TODO: Implement comments viewing functionality
            Response.Redirect($"~/HR/TaskComments.aspx?taskId={taskId}&employeeId={EmployeeId}");
        }

        private void ShowTaskDetails(int taskId)
        {
            try
            {
                var task = CurrentTaskList.FirstOrDefault(t => t.TaskId == taskId);
                if (task != null)
                {
                    var detailsHtml = GenerateTaskDetailsHtml(task);
                    litTaskDetailsContent.Text = detailsHtml;
                    pnlTaskDetailsModal.Visible = true;
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error loading task details: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in ShowTaskDetails: {ex}");
            }
        }

        #endregion

        #region Report Generation

        private void GenerateEmployeeOnboardingReport()
        {
            try
            {
                var employee = CurrentEmployee;
                var tasks = CurrentTaskList;

                var html = GenerateOnboardingReportHtml(employee, tasks);

                string fileName = $"OnboardingReport_{employee.EmployeeNumber}_{DateTime.Now:yyyyMMdd}.html";
                Response.ContentType = "text/html";
                Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
                Response.Write(html);
                Response.End();
            }
            catch (Exception ex)
            {
                ShowNotification($"Error generating report: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in GenerateEmployeeOnboardingReport: {ex}");
            }
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
            html.AppendLine(".completed { color: green; }");
            html.AppendLine(".pending { color: orange; }");
            html.AppendLine(".overdue { color: red; }");
            html.AppendLine("</style>");
            html.AppendLine("</head><body>");

            // Employee Header
            html.AppendLine($"<h1>Onboarding Report: {employee.FullName}</h1>");
            html.AppendLine($"<p><strong>Employee Number:</strong> {employee.EmployeeNumber}</p>");
            html.AppendLine($"<p><strong>Position:</strong> {employee.Position}</p>");
            html.AppendLine($"<p><strong>Department:</strong> {employee.DepartmentName}</p>");
            html.AppendLine($"<p><strong>Hire Date:</strong> {employee.HireDate:MMM dd, yyyy}</p>");
            html.AppendLine($"<p><strong>Report Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm}</p>");

            // Progress Summary
            int totalTasks = tasks.Count;
            int completedTasks = tasks.Count(t => t.Status == "COMPLETED");
            decimal completionPercentage = totalTasks > 0 ? (decimal)completedTasks / totalTasks * 100 : 0;

            html.AppendLine("<h2>Progress Summary</h2>");
            html.AppendLine($"<p><strong>Total Tasks:</strong> {totalTasks}</p>");
            html.AppendLine($"<p><strong>Completed Tasks:</strong> {completedTasks}</p>");
            html.AppendLine($"<p><strong>Completion Percentage:</strong> {completionPercentage:F1}%</p>");

            // Task Details Table
            html.AppendLine("<h2>Task Details</h2>");
            html.AppendLine("<table>");
            html.AppendLine("<tr>");
            html.AppendLine("<th>Task</th><th>Category</th><th>Priority</th><th>Status</th><th>Due Date</th><th>Completed Date</th>");
            html.AppendLine("</tr>");

            foreach (var task in tasks.OrderBy(t => t.SortOrder ?? 999).ThenBy(t => t.DueDate))
            {
                string statusClass = task.Status.ToLower().Replace("_", "-");
                if (task.IsOverdue && task.Status != "COMPLETED") statusClass = "overdue";

                html.AppendLine("<tr>");
                html.AppendLine($"<td>{task.Title}</td>");
                html.AppendLine($"<td>{task.Category}</td>");
                html.AppendLine($"<td>{task.Priority}</td>");
                html.AppendLine($"<td class=\"{statusClass}\">{task.Status}</td>");
                html.AppendLine($"<td>{task.DueDate:MMM dd, yyyy}</td>");
                html.AppendLine($"<td>{(task.CompletedDate?.ToString("MMM dd, yyyy") ?? "-")}</td>");
                html.AppendLine("</tr>");
            }

            html.AppendLine("</table>");
            html.AppendLine("</body></html>");

            return html.ToString();
        }

        #endregion

        #region Helper Methods

        private void ClearTaskForm()
        {
            txtTaskTitle.Text = "";
            txtTaskDescription.Text = "";
            ddlTaskCategory.SelectedIndex = 0;
            ddlTaskPriority.SelectedValue = "MEDIUM";
            txtTaskDueDate.Text = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
            txtEstimatedTime.Text = "";
            ddlAssignedToRole.SelectedValue = "HR";
            txtTaskInstructions.Text = "";
            chkCanEmployeeComplete.Checked = true;
            chkBlocksSystemAccess.Checked = false;
        }

        private string GenerateTaskDetailsHtml(OnboardingTaskInfo task)
        {
            var html = new StringBuilder();

            html.AppendLine($"<div class='task-detail-content'>");
            html.AppendLine($"<h5>{task.Title}</h5>");
            html.AppendLine($"<p><strong>Category:</strong> {GetCategoryDisplay(task.Category)}</p>");
            html.AppendLine($"<p><strong>Priority:</strong> {task.Priority}</p>");
            html.AppendLine($"<p><strong>Status:</strong> {GetStatusDisplay(task.Status)}</p>");
            html.AppendLine($"<p><strong>Due Date:</strong> {task.DueDate:MMM dd, yyyy}</p>");
            html.AppendLine($"<p><strong>Estimated Time:</strong> {task.EstimatedTime}</p>");
            html.AppendLine($"<p><strong>Assigned To:</strong> {GetAssignedToDisplay(task.AssignedToRole)}</p>");

            if (!string.IsNullOrEmpty(task.Description))
            {
                html.AppendLine($"<p><strong>Description:</strong></p>");
                html.AppendLine($"<p>{task.Description}</p>");
            }

            if (!string.IsNullOrEmpty(task.Instructions))
            {
                html.AppendLine($"<p><strong>Instructions:</strong></p>");
                html.AppendLine($"<p>{task.Instructions}</p>");
            }

            if (task.Status == "COMPLETED" && task.CompletedDate.HasValue)
            {
                html.AppendLine($"<p><strong>Completed:</strong> {task.CompletedDate:MMM dd, yyyy} by {task.CompletedByName}</p>");
            }

            if (!string.IsNullOrEmpty(task.Notes))
            {
                html.AppendLine($"<p><strong>Notes:</strong></p>");
                html.AppendLine($"<p>{task.Notes}</p>");
            }

            html.AppendLine($"</div>");

            return html.ToString();
        }

        private void ShowNotification(string message, string type)
        {
            litNotificationMessage.Text = message;
            pnlNotification.Visible = true;
            pnlNotification.CssClass = $"notification-panel {type}";
        }

        #endregion

        #region UI Helper Methods

        protected string GetTaskCardClass(string status, object isOverdue)
        {
            if (Convert.ToBoolean(isOverdue) && status != "COMPLETED")
                return "overdue";

            return status.ToLower().Replace("_", "-");
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
                default: return category ?? "Unknown";
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
                default: return status ?? "Unknown";
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

    #region Data Models

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