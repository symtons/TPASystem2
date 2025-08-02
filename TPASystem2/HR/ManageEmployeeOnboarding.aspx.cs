using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

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
                if (Request.QueryString["employeeId"] != null && int.TryParse(Request.QueryString["employeeId"], out int id))
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
            // Disable unobtrusive validation
            Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                CheckUserAccess();
                LoadEmployeeInfo();
                LoadAvailableTemplates();
                LoadEmployeeTasks();
                UpdateProgressStats();
            }
        }

        #endregion

        #region Initialization Methods

        private void CheckUserAccess()
        {
            // For testing purposes, allow access
            if (Session["UserId"] == null)
            {
                Session["UserId"] = 1; // Default user ID for testing
                Session["UserRole"] = "HR"; // Default role for testing
            }

            if (EmployeeId <= 0)
            {
                Response.Redirect("~/HR/Employees.aspx");
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
                            e.FirstName + ' ' + e.LastName as FullName,
                            e.EmployeeNumber,
                            e.Position,
                            e.HireDate,
                            d.Name as DepartmentName
                        FROM Employees e
                        INNER JOIN Departments d ON e.DepartmentId = d.Id
                        WHERE e.Id = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litEmployeeName.Text = reader["FullName"].ToString();
                                litEmployeeNumber.Text = reader["EmployeeNumber"].ToString();
                                litHireDate.Text = Convert.ToDateTime(reader["HireDate"]).ToString("MMM dd, yyyy");
                                litDepartment.Text = reader["DepartmentName"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading employee information: " + ex.Message);
            }
        }

        private void LoadAvailableTemplates()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get employee's department first
                    int departmentId = 0;
                    string deptQuery = "SELECT DepartmentId FROM Employees WHERE Id = @EmployeeId";
                    using (SqlCommand deptCmd = new SqlCommand(deptQuery, conn))
                    {
                        deptCmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        object result = deptCmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            departmentId = Convert.ToInt32(result);
                        }
                    }

                    // Load templates for this department and general templates
                    string query = @"
                        SELECT 
                            ott.Id,
                            ott.Title,
                            ott.Description,
                            ott.Category,
                            ott.Priority,
                            ott.EstimatedDays,
                            ott.Instructions,
                            d.Name as DepartmentName,
                            1 as TaskCount
                        FROM OnboardingTaskTemplates ott
                        INNER JOIN Departments d ON ott.DepartmentId = d.Id
                        WHERE ott.IsActive = 1 
                        AND (ott.DepartmentId = @DepartmentId OR ott.DepartmentId IN (
                            SELECT Id FROM Departments WHERE Name = 'General' OR Name = 'All'
                        ))
                        AND ott.Id NOT IN (
                            SELECT DISTINCT TemplateId 
                            FROM OnboardingTasks 
                            WHERE EmployeeId = @EmployeeId AND TemplateId IS NOT NULL
                        )
                        ORDER BY ott.Priority DESC, ott.Title";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        rptAvailableTemplates.DataSource = dt;
                        rptAvailableTemplates.DataBind();

                        // Hide template selection if no templates available
                        pnlTemplateSelection.Visible = dt.Rows.Count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading available templates: " + ex.Message);
            }
        }

        private void LoadEmployeeTasks()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            Id,
                            Title,
                            Description,
                            Category,
                            Status,
                            Priority,
                            DueDate,
                            EstimatedTime,
                            Instructions,
                            CreatedDate,
                            CASE 
                                WHEN Status = 'PENDING' AND DueDate < GETUTCDATE() THEN 'OVERDUE'
                                ELSE Status
                            END as DisplayStatus
                        FROM OnboardingTasks
                        WHERE EmployeeId = @EmployeeId 
                        AND IsTemplate = 0";

                    // Add filter if selected
                    if (!string.IsNullOrEmpty(ddlTaskFilter.SelectedValue))
                    {
                        if (ddlTaskFilter.SelectedValue == "OVERDUE")
                        {
                            query += " AND Status = 'PENDING' AND DueDate < GETUTCDATE()";
                        }
                        else
                        {
                            query += " AND Status = @Status";
                        }
                    }

                    query += " ORDER BY " +
                            "CASE Status " +
                            "WHEN 'IN_PROGRESS' THEN 1 " +
                            "WHEN 'PENDING' THEN 2 " +
                            "WHEN 'COMPLETED' THEN 3 " +
                            "ELSE 4 END, " +
                            "DueDate ASC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        if (!string.IsNullOrEmpty(ddlTaskFilter.SelectedValue) && ddlTaskFilter.SelectedValue != "OVERDUE")
                        {
                            cmd.Parameters.AddWithValue("@Status", ddlTaskFilter.SelectedValue);
                        }

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        rptTasks.DataSource = dt;
                        rptTasks.DataBind();

                        pnlEmptyTasks.Visible = dt.Rows.Count == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading employee tasks: " + ex.Message);
            }
        }

        private void UpdateProgressStats()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            COUNT(*) as TotalTasks,
                            SUM(CASE WHEN Status = 'COMPLETED' THEN 1 ELSE 0 END) as CompletedTasks,
                            SUM(CASE WHEN Status = 'PENDING' THEN 1 ELSE 0 END) as PendingTasks,
                            SUM(CASE WHEN Status = 'IN_PROGRESS' THEN 1 ELSE 0 END) as InProgressTasks
                        FROM OnboardingTasks
                        WHERE EmployeeId = @EmployeeId AND IsTemplate = 0";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int totalTasks = Convert.ToInt32(reader["TotalTasks"]);
                                int completedTasks = Convert.ToInt32(reader["CompletedTasks"]);
                                int pendingTasks = Convert.ToInt32(reader["PendingTasks"]);

                                litTotalTasks.Text = totalTasks.ToString();
                                litCompletedTasks.Text = completedTasks.ToString();
                                litPendingTasks.Text = pendingTasks.ToString();

                                int progressPercent = totalTasks > 0 ? (completedTasks * 100) / totalTasks : 0;
                                litProgressPercent.Text = progressPercent.ToString();
                                litProgressWidth.Text = progressPercent.ToString();
                                litProgressText.Text = $"{progressPercent}% Complete ({completedTasks}/{totalTasks})";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error updating progress stats: " + ex.Message);
            }
        }

        #endregion

        #region Helper Methods

        private void ShowSuccess(string message)
        {
            pnlMessages.Visible = true;
            divSuccess.Visible = true;
            divError.Visible = false;
            litSuccessMessage.Text = message;
        }

        private void ShowError(string message)
        {
            pnlMessages.Visible = true;
            divSuccess.Visible = false;
            divError.Visible = true;
            litErrorMessage.Text = message;
        }

        // Replace the AssignTemplatesToEmployee method in ManageEmployeeOnboarding.aspx.cs with this:

        private void AssignTemplatesToEmployee(List<int> templateIds)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (int templateId in templateIds)
                {
                    try
                    {
                        // First, get template details from OnboardingTaskTemplates
                        string getTemplateQuery = @"
                    SELECT 
                        Title,
                        ISNULL(Description, '') as Description,
                        Category,
                        ISNULL(Priority, 'MEDIUM') as Priority,
                        ISNULL(EstimatedDays, 1) as EstimatedDays,
                        ISNULL(Instructions, '') as Instructions,
                        ISNULL(CanEmployeeComplete, 1) as CanEmployeeComplete,
                        ISNULL(BlocksSystemAccess, 0) as BlocksSystemAccess
                    FROM OnboardingTaskTemplates
                    WHERE Id = @TemplateId AND IsActive = 1";

                        string title = "";
                        string description = "";
                        string category = "";
                        string priority = "MEDIUM";
                        int estimatedDays = 1;
                        string instructions = "";
                        bool canEmployeeComplete = true;
                        bool blocksSystemAccess = false;

                        using (SqlCommand getCmd = new SqlCommand(getTemplateQuery, conn))
                        {
                            getCmd.Parameters.AddWithValue("@TemplateId", templateId);
                            using (SqlDataReader reader = getCmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    title = reader["Title"].ToString();
                                    description = reader["Description"].ToString();
                                    category = reader["Category"].ToString();
                                    priority = reader["Priority"].ToString();
                                    estimatedDays = Convert.ToInt32(reader["EstimatedDays"]);
                                    instructions = reader["Instructions"].ToString();
                                    canEmployeeComplete = Convert.ToBoolean(reader["CanEmployeeComplete"]);
                                    blocksSystemAccess = Convert.ToBoolean(reader["BlocksSystemAccess"]);
                                }
                                else
                                {
                                    throw new Exception($"Template {templateId} not found or inactive");
                                }
                            }
                        }

                        // Now insert the task directly WITHOUT referencing TemplateId foreign key
                        // This avoids the foreign key constraint issue
                        string insertQuery = @"
                    INSERT INTO OnboardingTasks 
                    (EmployeeId, Title, Description, Category, Status, Priority, DueDate, 
                     EstimatedTime, Instructions, CreatedDate, IsTemplate, 
                     AssignedById, LastUpdated, CanEmployeeComplete, BlocksSystemAccess, Notes)
                    VALUES 
                    (@EmployeeId, @Title, @Description, @Category, @Status, @Priority, 
                     @DueDate, @EstimatedTime, @Instructions, GETUTCDATE(), @IsTemplate, 
                     @AssignedById, GETUTCDATE(), @CanEmployeeComplete, @BlocksSystemAccess, @Notes)";

                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                        {
                            insertCmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            insertCmd.Parameters.AddWithValue("@Title", title);
                            insertCmd.Parameters.AddWithValue("@Description", description);
                            insertCmd.Parameters.AddWithValue("@Category", category);
                            insertCmd.Parameters.AddWithValue("@Status", "PENDING");
                            insertCmd.Parameters.AddWithValue("@Priority", priority);
                            insertCmd.Parameters.AddWithValue("@DueDate", DateTime.UtcNow.AddDays(estimatedDays));
                            insertCmd.Parameters.AddWithValue("@EstimatedTime", estimatedDays + " days");
                            insertCmd.Parameters.AddWithValue("@Instructions", instructions);
                            insertCmd.Parameters.AddWithValue("@IsTemplate", 0);
                            insertCmd.Parameters.AddWithValue("@AssignedById", Convert.ToInt32(Session["UserId"] ?? "1"));
                            insertCmd.Parameters.AddWithValue("@CanEmployeeComplete", canEmployeeComplete);
                            insertCmd.Parameters.AddWithValue("@BlocksSystemAccess", blocksSystemAccess);
                            insertCmd.Parameters.AddWithValue("@Notes", $"Created from task template ID: {templateId}");

                            int rowsAffected = insertCmd.ExecuteNonQuery();
                            if (rowsAffected == 0)
                            {
                                throw new Exception($"Failed to create task from template {templateId}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error inserting template {templateId}: {ex.Message}");
                        throw new Exception($"Error creating task from template {templateId}: {ex.Message}");
                    }
                }
            }
        }

        private void UpdateTaskStatus(int taskId, string newStatus)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        UPDATE OnboardingTasks 
                        SET Status = @Status, 
                            LastUpdated = GETUTCDATE(),
                            CompletedDate = CASE WHEN @Status = 'COMPLETED' THEN GETUTCDATE() ELSE NULL END
                        WHERE Id = @TaskId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TaskId", taskId);
                        cmd.Parameters.AddWithValue("@Status", newStatus);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Refresh the tasks and progress
                LoadEmployeeTasks();
                UpdateProgressStats();
            }
            catch (Exception ex)
            {
                ShowError("Error updating task status: " + ex.Message);
            }
        }

        private void DeleteTask(int taskId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM OnboardingTasks WHERE Id = @TaskId";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TaskId", taskId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting task: " + ex.Message);
            }
        }

        #endregion

        #region Event Handlers

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/Employees.aspx");
        }

        protected void btnAssignTemplates_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> selectedTemplateIds = new List<int>();

                // Get selected templates from the repeater
                foreach (RepeaterItem item in rptAvailableTemplates.Items)
                {
                    CheckBox chkSelect = item.FindControl("chkSelectTemplate") as CheckBox;
                    HiddenField hdnTemplateId = item.FindControl("hdnTemplateId") as HiddenField;

                    if (chkSelect != null && chkSelect.Checked && hdnTemplateId != null)
                    {
                        if (int.TryParse(hdnTemplateId.Value, out int templateId))
                        {
                            selectedTemplateIds.Add(templateId);
                        }
                    }
                }

                if (selectedTemplateIds.Count == 0)
                {
                    ShowError("Please select at least one template to assign.");
                    return;
                }

                AssignTemplatesToEmployee(selectedTemplateIds);
                ShowSuccess($"Successfully assigned {selectedTemplateIds.Count} template(s) to employee.");

                // Refresh the page data
                LoadAvailableTemplates();
                LoadEmployeeTasks();
                UpdateProgressStats();
            }
            catch (Exception ex)
            {
                ShowError("Error assigning templates: " + ex.Message);
            }
        }

        protected void ddlTaskFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmployeeTasks();
        }

        protected void btnAddCustomTask_Click(object sender, EventArgs e)
        {
            // Redirect to custom task creation page or show modal
            Response.Redirect($"~/HR/AddCustomTask.aspx?employeeId={EmployeeId}");
        }

        protected void rptTasks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int taskId = Convert.ToInt32(e.CommandArgument);

            try
            {
                switch (e.CommandName)
                {
                    case "StartTask":
                        UpdateTaskStatus(taskId, "IN_PROGRESS");
                        ShowSuccess("Task started successfully.");
                        break;
                    case "CompleteTask":
                        UpdateTaskStatus(taskId, "COMPLETED");
                        ShowSuccess("Task completed successfully.");
                        break;
                    case "EditTask":
                        Response.Redirect($"~/HR/EditTask.aspx?taskId={taskId}&employeeId={EmployeeId}");
                        break;
                    case "DeleteTask":
                        DeleteTask(taskId);
                        ShowSuccess("Task deleted successfully.");
                        LoadEmployeeTasks();
                        UpdateProgressStats();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowError("Error processing task command: " + ex.Message);
            }
        }

        #endregion
    }
}