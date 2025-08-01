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
                            e.Id,
                            e.FirstName,
                            e.LastName,
                            e.EmployeeNumber,
                            e.HireDate,
                            e.Position,
                            d.Name as DepartmentName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        WHERE e.Id = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            litEmployeeName.Text = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
                            litEmployeeNumber.Text = reader["EmployeeNumber"].ToString();
                            litHireDate.Text = Convert.ToDateTime(reader["HireDate"]).ToString("MMM dd, yyyy");
                            litDepartment.Text = reader["DepartmentName"].ToString();
                        }
                        else
                        {
                            Response.Redirect("~/HR/Employees.aspx");
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

                    // Get employee's department for relevant templates
                    string deptQuery = "SELECT DepartmentId FROM Employees WHERE Id = @EmployeeId";
                    int departmentId = 0;

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
                            ot.Id,
                            ot.Title,
                            ot.Description,
                            ot.Category,
                            ot.Status,
                            ot.Priority,
                            ot.DueDate,
                            ot.EstimatedTime,
                            ot.Instructions,
                            ot.CreatedDate,
                            ot.CompletedDate
                        FROM OnboardingTasks ot
                        WHERE ot.EmployeeId = @EmployeeId
                        AND ot.IsTemplate = 0";

                    // Apply filter if selected
                    if (!string.IsNullOrEmpty(ddlTaskFilter.SelectedValue))
                    {
                        query += " AND ot.Status = @Status";
                    }

                    query += " ORDER BY ot.Priority DESC, ot.DueDate ASC, ot.CreatedDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        if (!string.IsNullOrEmpty(ddlTaskFilter.SelectedValue))
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
                            SUM(CASE WHEN Status = 'IN_PROGRESS' THEN 1 ELSE 0 END) as InProgressTasks,
                            SUM(CASE WHEN Status = 'OVERDUE' OR (Status = 'PENDING' AND DueDate < GETUTCDATE()) THEN 1 ELSE 0 END) as OverdueTasks
                        FROM OnboardingTasks 
                        WHERE EmployeeId = @EmployeeId AND IsTemplate = 0";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            int totalTasks = Convert.ToInt32(reader["TotalTasks"]);
                            int completedTasks = Convert.ToInt32(reader["CompletedTasks"]);
                            int pendingTasks = Convert.ToInt32(reader["PendingTasks"]);

                            litTotalTasks.Text = totalTasks.ToString();
                            litCompletedTasks.Text = completedTasks.ToString();
                            litPendingTasks.Text = pendingTasks.ToString();

                            // Calculate progress percentage
                            int progressPercent = totalTasks > 0 ? (completedTasks * 100) / totalTasks : 0;
                            litProgressPercent.Text = progressPercent.ToString();
                            litProgressWidth.Text = progressPercent.ToString();
                            litProgressText.Text = $"{progressPercent}% Complete ({completedTasks}/{totalTasks})";
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

        #region Event Handlers

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/Employees.aspx");
        }

        protected void btnAssignTemplates_Click(object sender, EventArgs e)
        {
            try
            {
                // First, let's debug what we're working with
                ShowError($"Debug: EmployeeId = {EmployeeId}, Session UserId = {Session["UserId"]}");

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

                ShowError($"Debug: Found {selectedTemplateIds.Count} selected templates: {string.Join(",", selectedTemplateIds)}");

                if (selectedTemplateIds.Count == 0)
                {
                    ShowError("Please select at least one template to assign.");
                    return;
                }

                // Try the database connection first
                TestDatabaseConnection();

                AssignTemplatesToEmployee(selectedTemplateIds);
                ShowSuccess($"Successfully assigned {selectedTemplateIds.Count} template(s) to employee.");

                // Refresh the page data
                LoadAvailableTemplates();
                LoadEmployeeTasks();
                UpdateProgressStats();
            }
            catch (Exception ex)
            {
                ShowError("Error in btnAssignTemplates_Click: " + ex.Message + " | Stack: " + ex.StackTrace);
                System.Diagnostics.Debug.WriteLine($"Full error: {ex}");
            }
        }

        private void TestDatabaseConnection()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Test if we can read from OnboardingTasks
                string testQuery = "SELECT COUNT(*) FROM OnboardingTasks";
                using (SqlCommand cmd = new SqlCommand(testQuery, conn))
                {
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    ShowError($"Debug: OnboardingTasks table has {count} records");
                }

                // Test if we can read from OnboardingTaskTemplates
                string testQuery2 = "SELECT COUNT(*) FROM OnboardingTaskTemplates";
                using (SqlCommand cmd2 = new SqlCommand(testQuery2, conn))
                {
                    int count2 = Convert.ToInt32(cmd2.ExecuteScalar());
                    ShowError($"Debug: OnboardingTaskTemplates table has {count2} records");
                }
            }
        }

        private void AssignTemplatesToEmployee(List<int> templateIds)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                foreach (int templateId in templateIds)
                {
                    try
                    {
                        // Use stored procedure approach
                        using (SqlCommand cmd = new SqlCommand("sp_CreateOnboardingTaskFromTemplate", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                            cmd.Parameters.AddWithValue("@TemplateId", templateId);
                            cmd.Parameters.AddWithValue("@CreatedByUserId", Convert.ToInt32(Session["UserId"]));

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    bool success = Convert.ToBoolean(reader["Success"]);
                                    string message = reader["Message"].ToString();

                                    if (!success)
                                    {
                                        throw new Exception($"Stored procedure failed: {message}");
                                    }
                                }
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
                        break;
                }

                LoadEmployeeTasks();
                UpdateProgressStats();
            }
            catch (Exception ex)
            {
                ShowError("Error processing task: " + ex.Message);
            }
        }

        private void UpdateTaskStatus(int taskId, string status)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
                    UPDATE OnboardingTasks 
                    SET Status = @Status, 
                        LastUpdated = GETUTCDATE()";

                if (status == "COMPLETED")
                {
                    query += ", CompletedDate = GETUTCDATE()";
                }

                query += " WHERE Id = @TaskId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void DeleteTask(int taskId)
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

        #endregion

        #region Helper Methods

        private void ShowSuccess(string message)
        {
            // You can implement a success notification system here
            // For now, we'll use a simple alert via JavaScript
            ScriptManager.RegisterStartupScript(this, GetType(), "showSuccess",
                $"alert('Success: {message}');", true);
        }

        private void ShowError(string message)
        {
            // You can implement an error notification system here
            // For now, we'll use a simple alert via JavaScript
            ScriptManager.RegisterStartupScript(this, GetType(), "showError",
                $"alert('Error: {message}');", true);
        }

        #endregion
    }
}