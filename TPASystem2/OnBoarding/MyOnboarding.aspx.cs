using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.OnBoarding
{
    public partial class MyOnboarding : System.Web.UI.Page
    {
        #region Properties and Fields

        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int CurrentEmployeeId => GetCurrentEmployeeId();

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set UnobtrusiveValidationMode to prevent validation errors
                Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

                InitializePage();
                LoadEmployeeInfo();
                LoadOnboardingTasks();
                UpdateProgressData();
            }
        }

        protected void rptTasks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                if (e.CommandName.StartsWith("COMPLETE_TASK:"))
                {
                    int taskId = Convert.ToInt32(e.CommandName.Split(':')[1]);
                    CompleteTask(taskId);
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error processing task: {ex.Message}", "error");
            }
        }

        #endregion

        #region Initialization Methods

        private void InitializePage()
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Check if user is an employee
            string userRole = Session["UserRole"]?.ToString() ?? "";
            if (!userRole.Equals("EMPLOYEE", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Dashboard.aspx");
                return;
            }

            // Check if employee exists
            if (CurrentEmployeeId <= 0)
            {
                ShowNotification("Employee record not found. Please contact HR.", "error");
                return;
            }
        }

        #endregion

        #region Data Loading Methods

        private void LoadEmployeeInfo()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            e.Id,
                            e.FirstName,
                            e.LastName,
                            e.EmployeeNumber,
                            e.HireDate,
                            d.Name as DepartmentName
                        FROM [dbo].[Employees] e
                        LEFT JOIN [dbo].[Departments] d ON e.DepartmentId = d.Id
                        WHERE e.UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", Session["UserId"]);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litEmployeeName.Text = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
                                litEmployeeNumber.Text = reader["EmployeeNumber"].ToString();
                                litDepartment.Text = reader["DepartmentName"]?.ToString() ?? "Not Assigned";
                                litHireDate.Text = Convert.ToDateTime(reader["HireDate"]).ToString("MMM dd, yyyy");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading employee info: {ex.Message}");
                ShowNotification("Error loading employee information.", "error");
            }
        }

        private void LoadOnboardingTasks()
        {
            try
            {
                var tasks = GetEmployeeTasks();

                if (tasks.Count == 0)
                {
                    pnlEmptyTasks.Visible = true;
                    rptTasks.Visible = false;
                }
                else
                {
                    rptTasks.DataSource = tasks;
                    rptTasks.DataBind();
                    pnlEmptyTasks.Visible = false;
                    rptTasks.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading onboarding tasks: {ex.Message}");
                ShowNotification("Error loading onboarding tasks.", "error");
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

                    string query = @"
                        SELECT 
                            ot.Id AS TaskId,
                            ot.Title,
                            ot.Description,
                            ot.Category,
                            ot.Status,
                            ot.Priority,
                            ot.DueDate,
                            ot.EstimatedTime,
                            ot.Instructions,
                            ot.CreatedDate,
                            ot.CompletedDate,
                            ot.Notes,
                            ot.AssignedToRole,
                            ISNULL(ot.CanEmployeeComplete, 0) AS CanEmployeeComplete,
                            ISNULL(ot.BlocksSystemAccess, 0) AS BlocksSystemAccess,
                            CASE 
                                WHEN ot.Status != 'COMPLETED' AND ot.DueDate < GETUTCDATE() THEN 1
                                ELSE 0
                            END AS IsOverdue
                        FROM [dbo].[OnboardingTasks] ot
                        WHERE ot.EmployeeId = @EmployeeId
                          AND ot.IsTemplate = 0
                        ORDER BY 
                            CASE ot.Priority
                                WHEN 'HIGH' THEN 1
                                WHEN 'MEDIUM' THEN 2
                                WHEN 'LOW' THEN 3
                                ELSE 4
                            END,
                            ot.DueDate,
                            ot.CreatedDate";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tasks.Add(new OnboardingTaskInfo
                                {
                                    TaskId = Convert.ToInt32(reader["TaskId"]),
                                    Title = reader["Title"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Category = reader["Category"].ToString(),
                                    Status = reader["Status"].ToString(),
                                    Priority = reader["Priority"].ToString(),
                                    DueDate = Convert.ToDateTime(reader["DueDate"]),
                                    EstimatedTime = reader["EstimatedTime"].ToString(),
                                    Instructions = reader["Instructions"]?.ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CompletedDate = reader["CompletedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CompletedDate"]),
                                    Notes = reader["Notes"]?.ToString(),
                                    AssignedToRole = reader["AssignedToRole"]?.ToString(),
                                    CanEmployeeComplete = Convert.ToBoolean(reader["CanEmployeeComplete"]),
                                    BlocksSystemAccess = Convert.ToBoolean(reader["BlocksSystemAccess"]),
                                    IsOverdue = Convert.ToBoolean(reader["IsOverdue"])
                                });
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

        private void UpdateProgressData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            TotalTasks,
                            CompletedTasks,
                            CompletionPercentage
                        FROM [dbo].[OnboardingProgress]
                        WHERE EmployeeId = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hidTotalTasks.Value = reader["TotalTasks"].ToString();
                                hidCompletedTasks.Value = reader["CompletedTasks"].ToString();
                                hidCompletionPercentage.Value = reader["CompletionPercentage"].ToString();
                            }
                            else
                            {
                                // Initialize progress if it doesn't exist
                                InitializeProgress();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating progress data: {ex.Message}");
            }
        }

        #endregion

        #region Task Management Methods

        private void CompleteTask(int taskId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Update task status
                            string updateTaskQuery = @"
                                UPDATE [dbo].[OnboardingTasks] 
                                SET Status = 'COMPLETED',
                                    CompletedDate = GETUTCDATE(),
                                    CompletedByUserId = @UserId,
                                    LastUpdated = GETUTCDATE()
                                WHERE Id = @TaskId 
                                  AND EmployeeId = @EmployeeId 
                                  AND CanEmployeeComplete = 1 
                                  AND Status != 'COMPLETED'";

                            using (SqlCommand cmd = new SqlCommand(updateTaskQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@TaskId", taskId);
                                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                                cmd.Parameters.AddWithValue("@UserId", Session["UserId"]);

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected == 0)
                                {
                                    throw new Exception("Task not found or cannot be completed by employee.");
                                }
                            }

                            // Update progress
                            UpdateOnboardingProgress(conn, transaction);

                            transaction.Commit();

                            // Show success and refresh page
                            ClientScript.RegisterStartupScript(this.GetType(), "showSuccess", "showModal();", true);
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error completing task: {ex.Message}");
                ShowNotification($"Error completing task: {ex.Message}", "error");
            }
        }

        private void UpdateOnboardingProgress(SqlConnection conn, SqlTransaction transaction)
        {
            string updateProgressQuery = @"
                UPDATE [dbo].[OnboardingProgress]
                SET 
                    CompletedTasks = (SELECT COUNT(*) FROM [dbo].[OnboardingTasks] WHERE EmployeeId = @EmployeeId AND IsTemplate = 0 AND Status = 'COMPLETED'),
                    PendingTasks = (SELECT COUNT(*) FROM [dbo].[OnboardingTasks] WHERE EmployeeId = @EmployeeId AND IsTemplate = 0 AND Status != 'COMPLETED'),
                    OverdueTasks = (SELECT COUNT(*) FROM [dbo].[OnboardingTasks] WHERE EmployeeId = @EmployeeId AND IsTemplate = 0 AND (Status = 'OVERDUE' OR (Status != 'COMPLETED' AND DueDate < GETUTCDATE()))),
                    CompletionPercentage = CASE 
                        WHEN TotalTasks > 0 
                        THEN CAST(((SELECT COUNT(*) FROM [dbo].[OnboardingTasks] WHERE EmployeeId = @EmployeeId AND IsTemplate = 0 AND Status = 'COMPLETED') * 100.0 / TotalTasks) as DECIMAL(5,2))
                        ELSE 0
                    END,
                    Status = CASE 
                        WHEN (SELECT COUNT(*) FROM [dbo].[OnboardingTasks] WHERE EmployeeId = @EmployeeId AND IsTemplate = 0 AND Status = 'COMPLETED') = TotalTasks THEN 'COMPLETED'
                        WHEN (SELECT COUNT(*) FROM [dbo].[OnboardingTasks] WHERE EmployeeId = @EmployeeId AND IsTemplate = 0 AND Status = 'COMPLETED') > 0 THEN 'IN_PROGRESS'
                        ELSE 'NOT_STARTED'
                    END,
                    LastUpdated = GETUTCDATE(),
                    CompletionDate = CASE 
                        WHEN (SELECT COUNT(*) FROM [dbo].[OnboardingTasks] WHERE EmployeeId = @EmployeeId AND IsTemplate = 0 AND Status = 'COMPLETED') = TotalTasks 
                        THEN GETUTCDATE()
                        ELSE CompletionDate
                    END
                WHERE EmployeeId = @EmployeeId";

            using (SqlCommand cmd = new SqlCommand(updateProgressQuery, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                cmd.ExecuteNonQuery();
            }
        }

        private void InitializeProgress()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO [dbo].[OnboardingProgress] 
                        (EmployeeId, TotalTasks, CompletedTasks, PendingTasks, OverdueTasks, CompletionPercentage, StartDate, Status, LastUpdated)
                        SELECT 
                            @EmployeeId,
                            COUNT(*) as TotalTasks,
                            0 as CompletedTasks,
                            COUNT(*) as PendingTasks,
                            0 as OverdueTasks,
                            0 as CompletionPercentage,
                            GETUTCDATE() as StartDate,
                            'NOT_STARTED' as Status,
                            GETUTCDATE() as LastUpdated
                        FROM [dbo].[OnboardingTasks]
                        WHERE EmployeeId = @EmployeeId AND IsTemplate = 0";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Reload progress data
                UpdateProgressData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing progress: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        private int GetCurrentEmployeeId()
        {
            try
            {
                int userId = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : 0;

                if (userId == 0) return 0;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT Id FROM [dbo].[Employees] WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting employee ID: {ex.Message}");
                return 0;
            }
        }

        private void ShowNotification(string message, string type)
        {
            string script = $"showNotification('{message}', '{type}');";
            ClientScript.RegisterStartupScript(this.GetType(), "showNotification", script, true);
        }

        #endregion

        #region Template Helper Methods

        public string GetTaskCardClass(string status, string priority, bool isOverdue)
        {
            var classes = new List<string>();

            if (status == "COMPLETED")
                classes.Add("completed");

            if (isOverdue && status != "COMPLETED")
                classes.Add("overdue");

            classes.Add($"priority-{priority.ToLower()}");

            return string.Join(" ", classes);
        }

        public string GetTaskStatusIcon(string status)
        {
            switch (status?.ToUpper())
            {
                case "COMPLETED":
                    return "<i class='material-icons' style='color: #4caf50; background: #e8f5e8;'>check_circle</i>";
                case "IN_PROGRESS":
                    return "<i class='material-icons' style='color: #ff9800; background: #fff3e0;'>access_time</i>";
                case "OVERDUE":
                    return "<i class='material-icons' style='color: #f44336; background: #ffebee;'>warning</i>";
                case "PENDING":
                default:
                    return "<i class='material-icons' style='color: #2196f3; background: #e3f2fd;'>pending</i>";
            }
        }

        public string GetCategoryIcon(string category)
        {
            switch (category?.ToUpper())
            {
                case "DOCUMENTATION":
                    return "<i class='material-icons'>description</i>";
                case "SETUP":
                    return "<i class='material-icons'>settings</i>";
                case "ORIENTATION":
                    return "<i class='material-icons'>tour</i>";
                case "TRAINING":
                    return "<i class='material-icons'>school</i>";
                default:
                    return "<i class='material-icons'>assignment</i>";
            }
        }

        public string GetCategoryDisplay(string category)
        {
            switch (category?.ToUpper())
            {
                case "DOCUMENTATION":
                    return "Documentation";
                case "SETUP":
                    return "Setup";
                case "ORIENTATION":
                    return "Orientation";
                case "TRAINING":
                    return "Training";
                default:
                    return category ?? "General";
            }
        }

        public string GetCategoryClass(string category)
        {
            return $"category-{category?.ToLower() ?? "general"}";
        }

        public string GetPriorityClass(string priority)
        {
            return $"priority-{priority?.ToLower() ?? "medium"}";
        }

        public string GetAssignedToDisplay(string role)
        {
            switch (role?.ToUpper())
            {
                case "HR":
                    return "HR Department";
                case "MANAGER":
                    return "Direct Manager";
                case "EMPLOYEE":
                    return "You";
                case "IT":
                    return "IT Department";
                case "FACILITIES":
                    return "Facilities";
                case "FINANCE":
                    return "Finance Department";
                default:
                    return role ?? "Unassigned";
            }
        }

        #endregion
    }

    #region Data Models

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
        public bool IsOverdue { get; set; }
    }

    #endregion
}