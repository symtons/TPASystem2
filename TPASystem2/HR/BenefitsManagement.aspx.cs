using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace TPASystem2.HR
{
    public partial class BenefitsManagement : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
                LoadBenefitsStats();
                LoadBenefitsPlans();
                LoadEmployeeEnrollments();
                LoadDropdownData();
                SetActiveTab("Plans");
            }
        }

        private void InitializePage()
        {
            // Set default enrollment date
            txtEnrollmentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtDependentsCount.Text = "0";
        }

        #endregion

        #region Data Loading Methods

        private void LoadBenefitsStats()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load Health Enrollments
                    string healthQuery = @"
                        SELECT COUNT(*) 
                        FROM EmployeeBenefitsEnrollments ebe
                        INNER JOIN BenefitsPlans bp ON ebe.BenefitsPlanId = bp.Id
                        WHERE bp.PlanType = 'HEALTH' AND ebe.Status = 'ACTIVE'";

                    using (SqlCommand cmd = new SqlCommand(healthQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        lblHealthEnrollments.Text = result?.ToString() ?? "0";
                    }

                    // Load Dental Enrollments
                    string dentalQuery = @"
                        SELECT COUNT(*) 
                        FROM EmployeeBenefitsEnrollments ebe
                        INNER JOIN BenefitsPlans bp ON ebe.BenefitsPlanId = bp.Id
                        WHERE bp.PlanType = 'DENTAL' AND ebe.Status = 'ACTIVE'";

                    using (SqlCommand cmd = new SqlCommand(dentalQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        lblDentalEnrollments.Text = result?.ToString() ?? "0";
                    }

                    // Load Vision Enrollments
                    string visionQuery = @"
                        SELECT COUNT(*) 
                        FROM EmployeeBenefitsEnrollments ebe
                        INNER JOIN BenefitsPlans bp ON ebe.BenefitsPlanId = bp.Id
                        WHERE bp.PlanType = 'VISION' AND ebe.Status = 'ACTIVE'";

                    using (SqlCommand cmd = new SqlCommand(visionQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        lblVisionEnrollments.Text = result?.ToString() ?? "0";
                    }

                    // Load Eligible Employees (Full-time employees)
                    string eligibleQuery = @"
                        SELECT COUNT(*) 
                        FROM Employees 
                        WHERE EmployeeType = 'Full-time' AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(eligibleQuery, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        lblEligibleEmployees.Text = result?.ToString() ?? "0";
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading benefits statistics: {ex.Message}", "error");
            }
        }

        private void LoadBenefitsPlans()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT Id as PlanId, PlanName, PlanType, PlanCategory, 
                               MonthlyEmployeeCost, MonthlyEmployerCost, 
                               AnnualDeductible, IsActive, EffectiveDate
                        FROM BenefitsPlans 
                        WHERE 1=1";

                    // Apply filters
                    if (!string.IsNullOrEmpty(ddlPlanType.SelectedValue))
                    {
                        query += " AND PlanType = @PlanType";
                    }
                    if (!string.IsNullOrEmpty(ddlPlanCategory.SelectedValue))
                    {
                        query += " AND PlanCategory = @PlanCategory";
                    }

                    query += " ORDER BY PlanType, PlanName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(ddlPlanType.SelectedValue))
                            cmd.Parameters.AddWithValue("@PlanType", ddlPlanType.SelectedValue);
                        if (!string.IsNullOrEmpty(ddlPlanCategory.SelectedValue))
                            cmd.Parameters.AddWithValue("@PlanCategory", ddlPlanCategory.SelectedValue);

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvBenefitsPlans.DataSource = dt;
                        gvBenefitsPlans.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading benefits plans: {ex.Message}", "error");
            }
        }

        private void LoadEmployeeEnrollments()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT ebe.Id as EnrollmentId, 
                               e.FirstName + ' ' + e.LastName AS EmployeeName,
                               e.EmployeeNumber,
                               bp.PlanName, bp.PlanType,
                               ebe.MonthlyPremium as MonthlyEmployeeCost,
                               ebe.EnrollmentDate,
                               ebe.DependentsCount,
                               ebe.Status as EnrollmentStatus
                        FROM EmployeeBenefitsEnrollments ebe
                        INNER JOIN Employees e ON ebe.EmployeeId = e.Id
                        INNER JOIN BenefitsPlans bp ON ebe.BenefitsPlanId = bp.Id
                        WHERE e.EmployeeType = 'Full-time'";

                    // Apply search filter
                    if (!string.IsNullOrEmpty(txtSearchEmployee.Text.Trim()))
                    {
                        query += " AND (e.FirstName + ' ' + e.LastName LIKE @SearchTerm OR e.EmployeeNumber LIKE @SearchTerm)";
                    }

                    query += " ORDER BY ebe.EnrollmentDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(txtSearchEmployee.Text.Trim()))
                            cmd.Parameters.AddWithValue("@SearchTerm", "%" + txtSearchEmployee.Text.Trim() + "%");

                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gvEmployeeEnrollments.DataSource = dt;
                        gvEmployeeEnrollments.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading employee enrollments: {ex.Message}", "error");
            }
        }

        private void LoadDropdownData()
        {
            try
            {
                LoadEmployeesDropdown();
                LoadBenefitsPlansDropdown();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading dropdown data: {ex.Message}", "error");
            }
        }

        private void LoadEmployeesDropdown()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT Id as EmployeeId, FirstName + ' ' + LastName + ' (' + EmployeeNumber + ')' AS EmployeeName
                    FROM Employees 
                    WHERE EmployeeType = 'Full-time' AND IsActive = 1
                    ORDER BY FirstName, LastName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlEmployee.DataSource = dt;
                    ddlEmployee.DataTextField = "EmployeeName";
                    ddlEmployee.DataValueField = "EmployeeId";
                    ddlEmployee.DataBind();

                    ddlEmployee.Items.Insert(0, new ListItem("Select Employee", ""));
                }
            }
        }

        private void LoadBenefitsPlansDropdown()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT Id as PlanId, PlanName + ' (' + PlanType + ')' AS PlanDisplay
                    FROM BenefitsPlans 
                    WHERE IsActive = 1
                    ORDER BY PlanType, PlanName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlBenefitsPlan.DataSource = dt;
                    ddlBenefitsPlan.DataTextField = "PlanDisplay";
                    ddlBenefitsPlan.DataValueField = "PlanId";
                    ddlBenefitsPlan.DataBind();

                    ddlBenefitsPlan.Items.Insert(0, new ListItem("Select Benefits Plan", ""));
                }
            }
        }

        #endregion

        #region Filter Events

        protected void ddlPlanType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBenefitsPlans();
        }

        protected void ddlPlanCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBenefitsPlans();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadEmployeeEnrollments();
        }

        protected void btnClearFilter_Click(object sender, EventArgs e)
        {
            ddlPlanType.SelectedIndex = 0;
            ddlPlanCategory.SelectedIndex = 0;
            txtSearchEmployee.Text = "";
            LoadBenefitsPlans();
            LoadEmployeeEnrollments();
        }

        #endregion

        #region Button Events

        protected void btnEnrollEmployee_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "showModal", "showEnrollmentModal();", true);
        }

        protected void btnAddPlan_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/AddBenefitsPlan.aspx");
        }

        protected void btnSaveEnrollment_Click(object sender, EventArgs e)
        {
            if (ValidateEnrollmentForm())
            {
                SaveEmployeeEnrollment();
            }
        }

        #endregion

        #region Tab Navigation

        protected void btnTabPlans_Click(object sender, EventArgs e)
        {
            SetActiveTab("Plans");
        }

        protected void btnTabEnrollments_Click(object sender, EventArgs e)
        {
            SetActiveTab("Enrollments");
        }

        protected void btnTabReports_Click(object sender, EventArgs e)
        {
            SetActiveTab("Reports");
        }

        private void SetActiveTab(string tabName)
        {
            // Reset all tab buttons
            btnTabPlans.CssClass = "tab-button";
            btnTabEnrollments.CssClass = "tab-button";
            btnTabReports.CssClass = "tab-button";

            // Hide all tab content
            pnlPlansTab.CssClass = "tab-content";
            pnlEnrollmentsTab.CssClass = "tab-content";
            pnlReportsTab.CssClass = "tab-content";

            // Show selected tab
            switch (tabName)
            {
                case "Plans":
                    btnTabPlans.CssClass = "tab-button active";
                    pnlPlansTab.CssClass = "tab-content active";
                    break;
                case "Enrollments":
                    btnTabEnrollments.CssClass = "tab-button active";
                    pnlEnrollmentsTab.CssClass = "tab-content active";
                    break;
                case "Reports":
                    btnTabReports.CssClass = "tab-button active";
                    pnlReportsTab.CssClass = "tab-content active";
                    break;
            }
        }

        #endregion

        #region GridView Events

        protected void gvBenefitsPlans_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int planId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "EditPlan":
                    Response.Redirect($"~/HR/AddBenefitsPlan.aspx?planId={planId}");
                    break;
                case "DeletePlan":
                    DeleteBenefitsPlan(planId);
                    break;
            }
        }

        protected void gvEmployeeEnrollments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int enrollmentId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ViewDetails":
                    Response.Redirect($"~/HR/EnrollmentDetails.aspx?enrollmentId={enrollmentId}");
                    break;
                case "TerminateEnrollment":
                    TerminateEnrollment(enrollmentId);
                    break;
            }
        }

        #endregion

        #region Report Generation

        protected void btnGenerateSummaryReport_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateBenefitsSummaryReport();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating summary report: {ex.Message}", "error");
            }
        }

        protected void btnGenerateEligibilityReport_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateEligibilityReport();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating eligibility report: {ex.Message}", "error");
            }
        }

        protected void btnGenerateCostReport_Click(object sender, EventArgs e)
        {
            try
            {
                GenerateCostAnalysisReport();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error generating cost report: {ex.Message}", "error");
            }
        }

        #endregion

        #region Helper Methods

        private bool ValidateEnrollmentForm()
        {
            bool isValid = true;

            if (string.IsNullOrEmpty(ddlEmployee.SelectedValue))
            {
                ShowMessage("Please select an employee.", "error");
                isValid = false;
            }

            if (string.IsNullOrEmpty(ddlBenefitsPlan.SelectedValue))
            {
                ShowMessage("Please select a benefits plan.", "error");
                isValid = false;
            }

            if (string.IsNullOrEmpty(txtEnrollmentDate.Text))
            {
                ShowMessage("Please enter an enrollment date.", "error");
                isValid = false;
            }

            return isValid;
        }

        private void SaveEmployeeEnrollment()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_EnrollEmployeeInBenefits", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeId", ddlEmployee.SelectedValue);
                        cmd.Parameters.AddWithValue("@BenefitsPlanId", ddlBenefitsPlan.SelectedValue);
                        cmd.Parameters.AddWithValue("@EffectiveDate", DateTime.Parse(txtEnrollmentDate.Text));
                        cmd.Parameters.AddWithValue("@DependentsCount", int.Parse(txtDependentsCount.Text));
                        cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(txtEnrollmentNotes.Text) ? (object)DBNull.Value : txtEnrollmentNotes.Text);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string status = reader["Status"].ToString();
                                string message = reader["Message"].ToString();

                                if (status == "SUCCESS")
                                {
                                    ShowMessage("Employee successfully enrolled in benefits plan!", "success");
                                    ClearEnrollmentForm();
                                    LoadBenefitsStats();
                                    LoadEmployeeEnrollments();
                                    ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", "closeEnrollmentModal();", true);
                                }
                                else
                                {
                                    ShowMessage(message, "error");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error enrolling employee: {ex.Message}", "error");
            }
        }

        private void ClearEnrollmentForm()
        {
            ddlEmployee.SelectedIndex = 0;
            ddlBenefitsPlan.SelectedIndex = 0;
            txtEnrollmentDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtDependentsCount.Text = "0";
            txtEnrollmentNotes.Text = "";
        }

        private void DeleteBenefitsPlan(int planId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Check if plan has active enrollments - FIXED: Use correct column names
                    string checkQuery = @"
                        SELECT COUNT(*) 
                        FROM EmployeeBenefitsEnrollments 
                        WHERE BenefitsPlanId = @PlanId AND Status = 'ACTIVE'";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@PlanId", planId);
                        conn.Open();
                        int activeEnrollments = (int)checkCmd.ExecuteScalar();

                        if (activeEnrollments > 0)
                        {
                            ShowMessage("Cannot delete plan with active enrollments.", "error");
                            return;
                        }
                    }

                    // Soft delete the plan - FIXED: Use correct column name
                    string deleteQuery = "UPDATE BenefitsPlans SET IsActive = 0 WHERE Id = @PlanId";
                    using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@PlanId", planId);
                        deleteCmd.ExecuteNonQuery();
                    }

                    ShowMessage("Benefits plan deleted successfully!", "success");
                    LoadBenefitsPlans();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting benefits plan: {ex.Message}", "error");
            }
        }

        private void TerminateEnrollment(int enrollmentId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // FIXED: Use correct column names
                    string updateQuery = @"
                        UPDATE EmployeeBenefitsEnrollments 
                        SET Status = 'TERMINATED', 
                            UpdatedAt = GETUTCDATE()
                        WHERE Id = @EnrollmentId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@EnrollmentId", enrollmentId);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    ShowMessage("Enrollment terminated successfully!", "success");
                    LoadBenefitsStats();
                    LoadEmployeeEnrollments();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error terminating enrollment: {ex.Message}", "error");
            }
        }

        private void GenerateBenefitsSummaryReport()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // FIXED: Use correct column names
                string query = @"
                    SELECT 
                        bp.PlanType,
                        bp.PlanName,
                        COUNT(ebe.Id) as TotalEnrollments,
                        SUM(ebe.MonthlyPremium) as TotalEmployeeCosts,
                        SUM(bp.MonthlyEmployerCost) as TotalEmployerCosts,
                        AVG(CAST(ebe.DependentsCount as FLOAT)) as AvgDependents
                    FROM BenefitsPlans bp
                    LEFT JOIN EmployeeBenefitsEnrollments ebe ON bp.Id = ebe.BenefitsPlanId AND ebe.Status = 'ACTIVE'
                    WHERE bp.IsActive = 1
                    GROUP BY bp.PlanType, bp.PlanName, bp.Id
                    ORDER BY bp.PlanType, bp.PlanName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Export to CSV
                    ExportToCSV(dt, "BenefitsSummaryReport");
                }
            }
        }

        private void GenerateEligibilityReport()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // FIXED: Use correct column names and table references
                string query = @"
                    SELECT 
                        e.EmployeeNumber,
                        e.FirstName + ' ' + e.LastName as EmployeeName,
                        e.Email,
                        ISNULL(d.Name, 'No Department') as Department,
                        e.HireDate,
                        CASE 
                            WHEN EXISTS (SELECT 1 FROM EmployeeBenefitsEnrollments ebe 
                                        INNER JOIN BenefitsPlans bp ON ebe.BenefitsPlanId = bp.Id 
                                        WHERE ebe.EmployeeId = e.Id 
                                        AND bp.PlanType = 'HEALTH' 
                                        AND ebe.Status = 'ACTIVE') 
                            THEN 'Yes' ELSE 'No' 
                        END as HasHealthInsurance,
                        CASE 
                            WHEN EXISTS (SELECT 1 FROM EmployeeBenefitsEnrollments ebe 
                                        INNER JOIN BenefitsPlans bp ON ebe.BenefitsPlanId = bp.Id 
                                        WHERE ebe.EmployeeId = e.Id 
                                        AND bp.PlanType = 'DENTAL' 
                                        AND ebe.Status = 'ACTIVE') 
                            THEN 'Yes' ELSE 'No' 
                        END as HasDentalInsurance,
                        CASE 
                            WHEN EXISTS (SELECT 1 FROM EmployeeBenefitsEnrollments ebe 
                                        INNER JOIN BenefitsPlans bp ON ebe.BenefitsPlanId = bp.Id 
                                        WHERE ebe.EmployeeId = e.Id 
                                        AND bp.PlanType = 'VISION' 
                                        AND ebe.Status = 'ACTIVE') 
                            THEN 'Yes' ELSE 'No' 
                        END as HasVisionInsurance
                    FROM Employees e
                    LEFT JOIN Departments d ON e.DepartmentId = d.Id
                    WHERE e.EmployeeType = 'Full-time' AND e.IsActive = 1
                    ORDER BY e.FirstName, e.LastName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Export to CSV
                    ExportToCSV(dt, "EmployeeEligibilityReport");
                }
            }
        }

        private void GenerateCostAnalysisReport()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // FIXED: Use correct column names and table references
                string query = @"
                    SELECT 
                        ISNULL(d.Name, 'No Department') as Department,
                        bp.PlanType,
                        COUNT(ebe.Id) as EnrollmentCount,
                        SUM(ebe.MonthlyPremium) as MonthlyEmployeeCosts,
                        SUM(bp.MonthlyEmployerCost) as MonthlyEmployerCosts,
                        SUM(ebe.MonthlyPremium + bp.MonthlyEmployerCost) as TotalMonthlyCosts,
                        SUM((ebe.MonthlyPremium + bp.MonthlyEmployerCost) * 12) as TotalAnnualCosts
                    FROM EmployeeBenefitsEnrollments ebe
                    INNER JOIN Employees e ON ebe.EmployeeId = e.Id
                    INNER JOIN BenefitsPlans bp ON ebe.BenefitsPlanId = bp.Id
                    LEFT JOIN Departments d ON e.DepartmentId = d.Id
                    WHERE ebe.Status = 'ACTIVE' AND e.EmployeeType = 'Full-time'
                    GROUP BY d.Name, bp.PlanType
                    ORDER BY d.Name, bp.PlanType";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Export to CSV
                    ExportToCSV(dt, "BenefitsCostAnalysisReport");
                }
            }
        }

        private void ExportToCSV(DataTable dt, string fileName)
        {
            try
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", $"attachment;filename={fileName}_{DateTime.Now:yyyyMMdd}.csv");
                Response.Charset = "";
                Response.ContentType = "application/text";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                // Add headers
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sb.Append(dt.Columns[i].ColumnName);
                    if (i < dt.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();

                // Add data rows
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string value = dt.Rows[i][j]?.ToString() ?? "";
                        // Escape commas and quotes in CSV
                        if (value.Contains(",") || value.Contains("\""))
                        {
                            value = "\"" + value.Replace("\"", "\"\"") + "\"";
                        }
                        sb.Append(value);
                        if (j < dt.Columns.Count - 1)
                            sb.Append(",");
                    }
                    sb.AppendLine();
                }

                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting report: {ex.Message}", "error");
            }
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = message;

            // Clear existing CSS classes
            pnlMessage.CssClass = "alert";

            // Add appropriate CSS class based on type
            switch (type.ToLower())
            {
                case "success":
                    pnlMessage.CssClass += " alert-success";
                    break;
                case "error":
                    pnlMessage.CssClass += " alert-error";
                    break;
                case "warning":
                    pnlMessage.CssClass += " alert-warning";
                    break;
                case "info":
                    pnlMessage.CssClass += " alert-info";
                    break;
                default:
                    pnlMessage.CssClass += " alert-info";
                    break;
            }

            // Auto-hide success messages after 5 seconds
            if (type.ToLower() == "success")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "hideMessage",
                    "setTimeout(function() { var alert = document.querySelector('.alert'); if(alert) alert.style.display = 'none'; }, 5000);", true);
            }
        }

        #endregion
    }
}