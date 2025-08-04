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
                LoadMandatoryTasks();
                LoadRegularTasks();
                UpdateProgressData();
                CheckSystemAccess();
            }
        }

        protected void rptTasks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                // Debug info
                System.Diagnostics.Debug.WriteLine($"Command Name: {e.CommandName}");
                System.Diagnostics.Debug.WriteLine($"Command Argument: {e.CommandArgument}");

                int taskId = Convert.ToInt32(e.CommandArgument);

                switch (e.CommandName)
                {
                    case "COMPLETE_MANDATORY":
                        System.Diagnostics.Debug.WriteLine($"Completing mandatory task: {taskId}");
                        CompleteMandatoryTask(taskId);
                        break;
                    case "COMPLETE_TASK":
                        System.Diagnostics.Debug.WriteLine($"Completing regular task: {taskId}");
                        CompleteRegularTask(taskId);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine($"Unknown command: {e.CommandName}");
                        ShowNotification($"Unknown command: {e.CommandName}", "error");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in rptTasks_ItemCommand: {ex.Message}");
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
                                string firstName = reader["FirstName"].ToString();
                                string lastName = reader["LastName"].ToString();

                                litEmployeeName.Text = firstName + " " + lastName;
                                litEmployeeNumber.Text = reader["EmployeeNumber"].ToString();
                                litDepartment.Text = reader["DepartmentName"]?.ToString() ?? "Not Assigned";
                                litHireDate.Text = Convert.ToDateTime(reader["HireDate"]).ToString("MMM dd, yyyy");

                                // Set employee initials
                                string initials = "";
                                if (!string.IsNullOrEmpty(firstName)) initials += firstName[0];
                                if (!string.IsNullOrEmpty(lastName)) initials += lastName[0];
                                litEmployeeInitials.Text = initials.ToUpper();
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

        private void LoadMandatoryTasks()
        {
            try
            {
                var mandatoryTasks = GetMandatoryTasks();

                if (mandatoryTasks.Count == 0)
                {
                    pnlMandatoryTasks.Visible = false;
                }
                else
                {
                    rptMandatoryTasks.DataSource = mandatoryTasks;
                    rptMandatoryTasks.DataBind();
                    pnlMandatoryTasks.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading mandatory tasks: {ex.Message}");
                ShowNotification("Error loading mandatory tasks.", "error");
            }
        }

        private void LoadRegularTasks()
        {
            try
            {
                var regularTasks = GetRegularTasks();

                if (regularTasks.Count == 0)
                {
                    pnlEmptyRegularTasks.Visible = true;
                    rptRegularTasks.Visible = false;
                }
                else
                {
                    rptRegularTasks.DataSource = regularTasks;
                    rptRegularTasks.DataBind();
                    pnlEmptyRegularTasks.Visible = false;
                    rptRegularTasks.Visible = true;
                }

                // Check if all tasks are completed
                var allTasks = GetAllTasks();
                if (allTasks.Count == 0 || allTasks.All(t => t.Status == "COMPLETED"))
                {
                    pnlEmptyTasks.Visible = true;
                    pnlMandatoryTasks.Visible = false;
                    pnlEmptyRegularTasks.Visible = false;
                    rptRegularTasks.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading regular tasks: {ex.Message}");
                ShowNotification("Error loading regular tasks.", "error");
            }
        }

        private void UpdateProgressData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Try using stored procedure first, fallback to direct SQL if it doesn't exist
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_GetMandatoryTasksStatus", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                // Skip the first result set (task details)
                                reader.NextResult();

                                // Read the summary statistics
                                if (reader.Read())
                                {
                                    int totalMandatory = Convert.ToInt32(reader["TotalMandatoryTasks"]);
                                    int completedMandatory = Convert.ToInt32(reader["CompletedMandatoryTasks"]);
                                    decimal percentage = Convert.ToDecimal(reader["MandatoryCompletionPercentage"]);

                                    litMandatoryProgress.Text = $"{percentage:F0}%";
                                    litMandatoryProgressText.Text = $"{completedMandatory} of {totalMandatory} mandatory tasks completed";
                                }
                            }
                        }
                    }
                    catch (SqlException)
                    {
                        // Fallback to direct SQL if stored procedure doesn't exist
                        string query = @"
                            SELECT 
                                COUNT(*) as TotalMandatory,
                                COUNT(CASE WHEN Status = 'COMPLETED' THEN 1 END) as CompletedMandatory
                            FROM [dbo].[OnboardingTasks] 
                            WHERE EmployeeId = @EmployeeId 
                                AND IsMandatory = 1 
                                AND IsTemplate = 0";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    int totalMandatory = Convert.ToInt32(reader["TotalMandatory"]);
                                    int completedMandatory = Convert.ToInt32(reader["CompletedMandatory"]);
                                    decimal percentage = totalMandatory > 0 ? (decimal)completedMandatory / totalMandatory * 100 : 0;

                                    litMandatoryProgress.Text = $"{percentage:F0}%";
                                    litMandatoryProgressText.Text = $"{completedMandatory} of {totalMandatory} mandatory tasks completed";
                                }
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

        private void CheckSystemAccess()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Try using stored procedure first, fallback to direct SQL if it doesn't exist
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_GetEmployeeSystemAccessStatus", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string accessStatus = reader["AccessStatus"].ToString();
                                    pnlSystemAccessWarning.Visible = accessStatus == "RESTRICTED_ACCESS";
                                }
                            }
                        }
                    }
                    catch (SqlException)
                    {
                        // Fallback to direct SQL
                        string query = @"
                            SELECT COUNT(*) 
                            FROM [dbo].[OnboardingTasks] 
                            WHERE EmployeeId = @EmployeeId 
                                AND IsMandatory = 1 
                                AND BlocksSystemAccess = 1 
                                AND Status != 'COMPLETED'
                                AND IsTemplate = 0";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                            int blockingTasks = Convert.ToInt32(cmd.ExecuteScalar());
                            pnlSystemAccessWarning.Visible = blockingTasks > 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking system access: {ex.Message}");
            }
        }

        #endregion

        #region Data Retrieval Methods

        private List<OnboardingTaskInfo> GetMandatoryTasks()
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
                            ISNULL(ot.AssignedToRole, 'HR') as AssignedToRole,
                            ISNULL(ot.CanEmployeeComplete, 1) as CanEmployeeComplete,
                            ISNULL(ot.BlocksSystemAccess, 0) as BlocksSystemAccess,
                            ot.SortOrder,
                            CASE 
                                WHEN ot.Status != 'COMPLETED' AND ot.DueDate < GETUTCDATE() THEN 1
                                ELSE 0
                            END AS IsOverdue
                        FROM [dbo].[OnboardingTasks] ot
                        WHERE ot.EmployeeId = @EmployeeId 
                            AND ot.IsMandatory = 1
                            AND ot.IsTemplate = 0
                        ORDER BY ot.SortOrder, ot.DueDate";

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
                                    DueDate = reader["DueDate"] != DBNull.Value ? Convert.ToDateTime(reader["DueDate"]) : DateTime.MinValue,
                                    EstimatedTime = reader["EstimatedTime"].ToString(),
                                    Instructions = reader["Instructions"].ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CompletedDate = reader["CompletedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CompletedDate"]) : (DateTime?)null,
                                    Notes = reader["Notes"].ToString(),
                                    AssignedToRole = reader["AssignedToRole"].ToString(),
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
                System.Diagnostics.Debug.WriteLine($"Error getting mandatory tasks: {ex.Message}");
            }

            return tasks;
        }

        private List<OnboardingTaskInfo> GetRegularTasks()
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
                            ISNULL(ot.AssignedToRole, 'HR') as AssignedToRole,
                            ISNULL(ot.CanEmployeeComplete, 1) as CanEmployeeComplete,
                            ISNULL(ot.BlocksSystemAccess, 0) as BlocksSystemAccess,
                            ot.SortOrder,
                            CASE 
                                WHEN ot.Status != 'COMPLETED' AND ot.DueDate < GETUTCDATE() THEN 1
                                ELSE 0
                            END AS IsOverdue
                        FROM [dbo].[OnboardingTasks] ot
                        WHERE ot.EmployeeId = @EmployeeId 
                            AND ISNULL(ot.IsMandatory, 0) = 0
                            AND ot.IsTemplate = 0
                        ORDER BY ot.SortOrder, ot.DueDate";

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
                                    DueDate = reader["DueDate"] != DBNull.Value ? Convert.ToDateTime(reader["DueDate"]) : DateTime.MinValue,
                                    EstimatedTime = reader["EstimatedTime"].ToString(),
                                    Instructions = reader["Instructions"].ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    CompletedDate = reader["CompletedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CompletedDate"]) : (DateTime?)null,
                                    Notes = reader["Notes"].ToString(),
                                    AssignedToRole = reader["AssignedToRole"].ToString(),
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
                System.Diagnostics.Debug.WriteLine($"Error getting regular tasks: {ex.Message}");
            }

            return tasks;
        }

        private List<OnboardingTaskInfo> GetAllTasks()
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
                            ot.Status
                        FROM [dbo].[OnboardingTasks] ot
                        WHERE ot.EmployeeId = @EmployeeId 
                            AND ot.IsTemplate = 0";

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
                                    Status = reader["Status"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting all tasks: {ex.Message}");
            }

            return tasks;
        }

        #endregion

        #region Task Completion Methods

        private void CompleteMandatoryTask(int taskId)
        {
            try
            {
                // First check if this is a task that requires navigation to a specific page
                string taskCategory = GetTaskCategory(taskId);

                switch (taskCategory)
                {
                    case "NEW_HIRE_PAPERWORK":
                        Response.Redirect("~/OnBoarding/NewHirePaperWork.aspx");
                        return;
                    case "DIRECT_DEPOSIT":
                        Response.Redirect("~/OnBoarding/DirectDepositEnrollment.aspx");
                        return;
                    case "MANDATORY_TRAINING":
                        Response.Redirect("~/OnBoarding/MandatoryTraining.aspx");
                        return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Try using stored procedure first, fallback to direct SQL if it doesn't exist
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_CompleteMandatoryTask", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@TaskId", taskId);
                            cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                            cmd.Parameters.AddWithValue("@CompletedById", CurrentEmployeeId);
                            cmd.Parameters.AddWithValue("@Notes", "Completed by employee through onboarding portal");

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string status = reader["Status"].ToString();
                                    string message = reader["Message"].ToString();

                                    if (status == "SUCCESS")
                                    {
                                        ShowNotification("Mandatory task completed successfully! 🎉", "success");
                                        // Redirect to refresh the page and show updated progress
                                        Response.Redirect(Request.RawUrl);
                                    }
                                    else
                                    {
                                        ShowNotification($"Error: {message}", "error");
                                    }
                                }
                            }
                        }
                    }
                    catch (SqlException)
                    {
                        // Fallback to direct SQL update if stored procedure doesn't exist
                        string updateQuery = @"
                            UPDATE [dbo].[OnboardingTasks] 
                            SET Status = 'COMPLETED', 
                                CompletedDate = GETUTCDATE(),
                                CompletedById = @CompletedById,
                                Notes = @Notes,
                                LastUpdated = GETUTCDATE()
                            WHERE Id = @TaskId AND EmployeeId = @EmployeeId";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@TaskId", taskId);
                            cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                            cmd.Parameters.AddWithValue("@CompletedById", CurrentEmployeeId);
                            cmd.Parameters.AddWithValue("@Notes", "Completed by employee through onboarding portal");

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                ShowNotification("Mandatory task completed successfully! 🎉", "success");
                                Response.Redirect(Request.RawUrl);
                            }
                            else
                            {
                                ShowNotification("Error completing task. Please try again.", "error");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error completing mandatory task: {ex.Message}");
                ShowNotification($"Error completing task: {ex.Message}", "error");
            }
        }

        private void CompleteRegularTask(int taskId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Try using stored procedure first, fallback to direct SQL if it doesn't exist
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_UpdateOnboardingTaskStatus", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@TaskId", taskId);
                            cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                            cmd.Parameters.AddWithValue("@Status", "COMPLETED");
                            cmd.Parameters.AddWithValue("@CompletedById", CurrentEmployeeId);
                            cmd.Parameters.AddWithValue("@Notes", "Completed by employee through onboarding portal");

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string status = reader["Status"].ToString();
                                    string message = reader["Message"].ToString();

                                    if (status == "SUCCESS")
                                    {
                                        ShowNotification("Task completed successfully! 🎉", "success");
                                        // Refresh the page to show updated progress
                                        Response.Redirect(Request.RawUrl);
                                    }
                                    else
                                    {
                                        ShowNotification($"Error: {message}", "error");
                                    }
                                }
                            }
                        }
                    }
                    catch (SqlException)
                    {
                        // Fallback to direct SQL update if stored procedure doesn't exist
                        string updateQuery = @"
                            UPDATE [dbo].[OnboardingTasks] 
                            SET Status = 'COMPLETED', 
                                CompletedDate = GETUTCDATE(),
                                CompletedById = @CompletedById,
                                Notes = @Notes,
                                LastUpdated = GETUTCDATE()
                            WHERE Id = @TaskId AND EmployeeId = @EmployeeId";

                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@TaskId", taskId);
                            cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                            cmd.Parameters.AddWithValue("@CompletedById", CurrentEmployeeId);
                            cmd.Parameters.AddWithValue("@Notes", "Completed by employee through onboarding portal");

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                ShowNotification("Task completed successfully! 🎉", "success");
                                Response.Redirect(Request.RawUrl);
                            }
                            else
                            {
                                ShowNotification("Error completing task. Please try again.", "error");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error completing regular task: {ex.Message}");
                ShowNotification($"Error completing task: {ex.Message}", "error");
            }
        }

        #endregion

        #region Helper Methods

        private string GetTaskCategory(int taskId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Category FROM [dbo].[OnboardingTasks] WHERE Id = @TaskId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TaskId", taskId);
                        return cmd.ExecuteScalar()?.ToString() ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting task category: {ex.Message}");
                return "";
            }
        }

        private int GetCurrentEmployeeId()
        {
            try
            {
                if (Session["UserId"] == null) return 0;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id FROM [dbo].[Employees] WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", Session["UserId"]);
                        var result = cmd.ExecuteScalar();
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

        protected string GetAssignedToDisplay(string assignedToRole)
        {
            switch (assignedToRole?.ToUpper())
            {
                case "HR":
                    return "Human Resources";
                case "IT":
                    return "IT Department";
                case "MANAGER":
                    return "Your Manager";
                case "SECURITY":
                    return "Security Department";
                case "EMPLOYEE":
                    return "You";
                default:
                    return assignedToRole ?? "HR";
            }
        }

        private void ShowNotification(string message, string type)
        {
            string script = $@"
                document.addEventListener('DOMContentLoaded', function() {{
                    var notification = document.createElement('div');
                    notification.className = 'notification {type}';
                    notification.textContent = '{message.Replace("'", "\\'")}';
                    document.body.appendChild(notification);
                    
                    setTimeout(function() {{
                        notification.remove();
                    }}, 5000);
                }});";

            ClientScript.RegisterStartupScript(this.GetType(), "ShowNotification", script, true);
        }

        #endregion

        #region Data Transfer Objects

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

        protected void btnLogout_Click(object sender, EventArgs e)
        {

        }

        protected void GoToDashboard_Click(object sender, EventArgs e)
        {

        }
    }
}