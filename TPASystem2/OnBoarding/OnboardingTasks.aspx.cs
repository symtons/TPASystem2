using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.OnBoarding
{
    public partial class OnboardingTasks : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Disable unobtrusive validation to prevent jQuery errors
            Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                CheckUserAccess();
                LoadDepartments();
                LoadTemplates();
            }
        }

        private void CheckUserAccess()
        {
            // For testing purposes, allow access - comment out the redirect
            // TODO: Uncomment this for production
            /*
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "HR")
            {
                Response.Redirect("~/Dashboard.aspx");
                return;
            }
            */

            // For testing - set a default user session if none exists
            if (Session["UserId"] == null)
            {
                Session["UserId"] = 1; // Default user ID for testing
                Session["UserRole"] = "HR"; // Default role for testing
            }
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
                        SqlDataReader reader = cmd.ExecuteReader();

                        // Load filter dropdown
                        ddlFilterDepartment.Items.Clear();
                        ddlFilterDepartment.Items.Add(new ListItem("All Departments", ""));

                        // Load create form dropdown
                        ddlDepartment.Items.Clear();
                        ddlDepartment.Items.Add(new ListItem("Select Department", ""));

                        while (reader.Read())
                        {
                            string id = reader["Id"].ToString();
                            string name = reader["Name"].ToString();

                            ddlFilterDepartment.Items.Add(new ListItem(name, id));
                            ddlDepartment.Items.Add(new ListItem(name, id));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading departments: " + ex.Message);
            }
        }

        private void LoadTemplates()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            ott.Id,
                            ott.Title,
                            ott.Description,
                            ott.Category,
                            ott.Priority,
                            ott.EstimatedDays,
                            ott.Instructions,
                            ott.CanEmployeeComplete,
                            ott.BlocksSystemAccess,
                            ott.IsActive,
                            ott.CreatedDate,
                            d.Name as DepartmentName
                        FROM OnboardingTaskTemplates ott
                        INNER JOIN Departments d ON ott.DepartmentId = d.Id
                        WHERE 1=1";

                    // Apply filters
                    if (!string.IsNullOrEmpty(ddlFilterDepartment.SelectedValue))
                    {
                        query += " AND ott.DepartmentId = @DepartmentId";
                    }

                    if (!string.IsNullOrEmpty(ddlFilterCategory.SelectedValue))
                    {
                        query += " AND ott.Category = @Category";
                    }

                    if (!string.IsNullOrEmpty(ddlFilterStatus.SelectedValue))
                    {
                        query += " AND ott.IsActive = @IsActive";
                    }

                    query += " ORDER BY ott.CreatedDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(ddlFilterDepartment.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@DepartmentId", ddlFilterDepartment.SelectedValue);
                        }

                        if (!string.IsNullOrEmpty(ddlFilterCategory.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@Category", ddlFilterCategory.SelectedValue);
                        }

                        if (!string.IsNullOrEmpty(ddlFilterStatus.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@IsActive", ddlFilterStatus.SelectedValue == "1");
                        }

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            rptTemplates.DataSource = dt;
                            rptTemplates.DataBind();
                            pnlNoTemplates.Visible = false;
                        }
                        else
                        {
                            rptTemplates.DataSource = null;
                            rptTemplates.DataBind();
                            pnlNoTemplates.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading templates: " + ex.Message);
            }
        }

        protected void ddlFilterDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTemplates();
        }

        protected void ddlFilterCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTemplates();
        }

        protected void ddlFilterStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadTemplates();
        }

        protected void btnTabTemplates_Click(object sender, EventArgs e)
        {
            ShowTemplatesTab();
        }

        protected void btnTabCreate_Click(object sender, EventArgs e)
        {
            ShowCreateTab();
        }

        protected void btnCreateTemplate_Click(object sender, EventArgs e)
        {
            ClearForm();
            ShowCreateTab();
        }

        private void ShowTemplatesTab()
        {
            pnlTemplatesTab.Visible = true;
            pnlCreateTab.Visible = false;
            btnTabTemplates.CssClass = "tab-button active";
            btnTabCreate.CssClass = "tab-button";
            LoadTemplates();
        }

        private void ShowCreateTab()
        {
            pnlTemplatesTab.Visible = false;
            pnlCreateTab.Visible = true;
            btnTabTemplates.CssClass = "tab-button";
            btnTabCreate.CssClass = "tab-button active";
        }

        protected void rptTemplates_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int templateId = Convert.ToInt32(e.CommandArgument);

            try
            {
                switch (e.CommandName)
                {
                    case "Edit":
                        EditTemplate(templateId);
                        break;
                    case "ToggleStatus":
                        ToggleTemplateStatus(templateId);
                        break;
                    case "Delete":
                        DeleteTemplate(templateId);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowError("Error processing command: " + ex.Message);
            }
        }

        private void EditTemplate(int templateId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            Id, DepartmentId, Title, Description, Category, Priority, 
                            EstimatedDays, Instructions, CanEmployeeComplete, 
                            BlocksSystemAccess, RequiresDocuments, RequiredDocumentsList,
                            AcceptedFileTypes, MaxFileSizeMB, IsActive
                        FROM OnboardingTaskTemplates 
                        WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", templateId);
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            hdnTemplateId.Value = reader["Id"].ToString();
                            ddlDepartment.SelectedValue = reader["DepartmentId"].ToString();
                            txtTitle.Text = reader["Title"].ToString();
                            txtDescription.Text = reader["Description"].ToString();
                            ddlCategory.SelectedValue = reader["Category"].ToString();
                            ddlPriority.SelectedValue = reader["Priority"].ToString();
                            txtEstimatedDays.Text = reader["EstimatedDays"].ToString();
                            txtInstructions.Text = reader["Instructions"].ToString();
                            chkCanEmployeeComplete.Checked = Convert.ToBoolean(reader["CanEmployeeComplete"]);
                            chkBlocksSystemAccess.Checked = Convert.ToBoolean(reader["BlocksSystemAccess"]);

                            // Handle document requirements
                            chkRequiresDocuments.Checked = Convert.ToBoolean(reader["RequiresDocuments"]);
                            pnlDocumentRequirements.Visible = chkRequiresDocuments.Checked;

                            if (chkRequiresDocuments.Checked)
                            {
                                txtRequiredDocuments.Text = reader["RequiredDocumentsList"].ToString();
                                if (!string.IsNullOrEmpty(reader["AcceptedFileTypes"].ToString()))
                                {
                                    ddlFileTypes.SelectedValue = reader["AcceptedFileTypes"].ToString();
                                }
                                if (!string.IsNullOrEmpty(reader["MaxFileSizeMB"].ToString()))
                                {
                                    ddlMaxFileSize.SelectedValue = reader["MaxFileSizeMB"].ToString();
                                }
                            }

                            litFormTitle.Text = "Edit Task Template";
                            ShowCreateTab();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error loading template for edit: " + ex.Message);
            }
        }

        private void ToggleTemplateStatus(int templateId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        UPDATE OnboardingTaskTemplates 
                        SET IsActive = CASE WHEN IsActive = 1 THEN 0 ELSE 1 END
                        WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", templateId);
                        cmd.ExecuteNonQuery();

                        ShowSuccess("Template status updated successfully.");
                        LoadTemplates();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error updating template status: " + ex.Message);
            }
        }

        private void DeleteTemplate(int templateId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if template is being used
                    string checkQuery = "SELECT COUNT(*) FROM OnboardingTasks WHERE TemplateId = @TemplateId";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@TemplateId", templateId);
                        int usageCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (usageCount > 0)
                        {
                            ShowError("Cannot delete template. It is currently being used by " + usageCount + " onboarding task(s).");
                            return;
                        }
                    }

                    // Delete template
                    string deleteQuery = "DELETE FROM OnboardingTaskTemplates WHERE Id = @Id";
                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", templateId);
                        cmd.ExecuteNonQuery();

                        ShowSuccess("Template deleted successfully.");
                        LoadTemplates();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error deleting template: " + ex.Message);
            }
        }

        protected void chkRequiresDocuments_CheckedChanged(object sender, EventArgs e)
        {
            pnlDocumentRequirements.Visible = chkRequiresDocuments.Checked;
        }

        protected void btnSaveTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                // Debug: Check if page is valid
                if (!Page.IsValid)
                {
                    ShowError("Please fix validation errors before saving.");
                    return;
                }

                // Debug: Check required fields manually
                if (string.IsNullOrEmpty(txtTitle.Text.Trim()))
                {
                    ShowError("Title is required.");
                    return;
                }

                if (string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                {
                    ShowError("Department is required.");
                    return;
                }

                if (string.IsNullOrEmpty(ddlCategory.SelectedValue))
                {
                    ShowError("Category is required.");
                    return;
                }

                if (string.IsNullOrEmpty(ddlPriority.SelectedValue))
                {
                    ShowError("Priority is required.");
                    return;
                }

                if (string.IsNullOrEmpty(txtEstimatedDays.Text.Trim()))
                {
                    ShowError("Estimated days is required.");
                    return;
                }

                int templateId = Convert.ToInt32(hdnTemplateId.Value);
                int userId = Session["UserId"] != null ? Convert.ToInt32(Session["UserId"]) : 1; // Default to 1 for testing

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query;
                    if (templateId == 0)
                    {
                        // Insert new template
                        query = @"
                            INSERT INTO OnboardingTaskTemplates 
                            (DepartmentId, Title, Description, Category, Priority, EstimatedDays, 
                             Instructions, CanEmployeeComplete, BlocksSystemAccess, RequiresDocuments,
                             RequiredDocumentsList, AcceptedFileTypes, MaxFileSizeMB, IsActive, CreatedDate)
                            VALUES 
                            (@DepartmentId, @Title, @Description, @Category, @Priority, @EstimatedDays,
                             @Instructions, @CanEmployeeComplete, @BlocksSystemAccess, @RequiresDocuments,
                             @RequiredDocumentsList, @AcceptedFileTypes, @MaxFileSizeMB, 1, GETUTCDATE())";
                    }
                    else
                    {
                        // Update existing template
                        query = @"
                            UPDATE OnboardingTaskTemplates 
                            SET DepartmentId = @DepartmentId, Title = @Title, Description = @Description,
                                Category = @Category, Priority = @Priority, EstimatedDays = @EstimatedDays,
                                Instructions = @Instructions, CanEmployeeComplete = @CanEmployeeComplete,
                                BlocksSystemAccess = @BlocksSystemAccess, RequiresDocuments = @RequiresDocuments,
                                RequiredDocumentsList = @RequiredDocumentsList, AcceptedFileTypes = @AcceptedFileTypes,
                                MaxFileSizeMB = @MaxFileSizeMB
                            WHERE Id = @Id";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (templateId > 0)
                        {
                            cmd.Parameters.AddWithValue("@Id", templateId);
                        }

                        cmd.Parameters.AddWithValue("@DepartmentId", ddlDepartment.SelectedValue);
                        cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue);
                        cmd.Parameters.AddWithValue("@Priority", ddlPriority.SelectedValue);
                        cmd.Parameters.AddWithValue("@EstimatedDays", Convert.ToInt32(txtEstimatedDays.Text));
                        cmd.Parameters.AddWithValue("@Instructions", txtInstructions.Text.Trim());
                        cmd.Parameters.AddWithValue("@CanEmployeeComplete", chkCanEmployeeComplete.Checked);
                        cmd.Parameters.AddWithValue("@BlocksSystemAccess", chkBlocksSystemAccess.Checked);

                        // Handle document requirements
                        if (chkRequiresDocuments.Checked)
                        {
                            cmd.Parameters.AddWithValue("@RequiresDocuments", true);
                            cmd.Parameters.AddWithValue("@RequiredDocumentsList", txtRequiredDocuments.Text.Trim());
                            cmd.Parameters.AddWithValue("@AcceptedFileTypes", ddlFileTypes.SelectedValue);
                            cmd.Parameters.AddWithValue("@MaxFileSizeMB", Convert.ToInt32(ddlMaxFileSize.SelectedValue));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@RequiresDocuments", false);
                            cmd.Parameters.AddWithValue("@RequiredDocumentsList", DBNull.Value);
                            cmd.Parameters.AddWithValue("@AcceptedFileTypes", DBNull.Value);
                            cmd.Parameters.AddWithValue("@MaxFileSizeMB", DBNull.Value);
                        }

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            string message = templateId == 0 ? "Template created successfully!" : "Template updated successfully!";
                            ShowSuccess(message);

                            ClearForm();
                            ShowTemplatesTab();
                        }
                        else
                        {
                            ShowError("No rows were affected. Template may not have been saved.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error saving template: " + ex.Message);
                // Log the full exception for debugging
                System.Diagnostics.Debug.WriteLine($"Save Template Error: {ex}");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            ShowTemplatesTab();
        }

        private void ClearForm()
        {
            hdnTemplateId.Value = "0";
            ddlDepartment.SelectedIndex = 0;
            txtTitle.Text = "";
            txtDescription.Text = "";
            ddlCategory.SelectedIndex = 0;
            ddlPriority.SelectedValue = "MEDIUM";
            txtEstimatedDays.Text = "";
            txtInstructions.Text = "";
            chkCanEmployeeComplete.Checked = false;
            chkBlocksSystemAccess.Checked = false;
            chkRequiresDocuments.Checked = false;
            pnlDocumentRequirements.Visible = false;
            txtRequiredDocuments.Text = "";
            ddlFileTypes.SelectedIndex = 0;
            ddlMaxFileSize.SelectedValue = "10";
            litFormTitle.Text = "Create New Task Template";
        }

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
    }
}