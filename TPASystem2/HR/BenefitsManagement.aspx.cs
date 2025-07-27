using System;
using System.Collections.Generic;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBenefitsOverview();
                LoadBenefitsPlans();
                LoadEmployeeEnrollments();
                LoadEmployeeDropdown();
                LoadBenefitsPlansDropdown();
                SetDefaultEffectiveDate();

                // Set default active tab
                SetActiveTab("Plans");
            }
        }

        #region Data Loading Methods

        private void LoadBenefitsOverview()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get benefits enrollment counts by type for full-time employees only
                    string query = @"
                        SELECT 
                            bp.PlanType,
                            COUNT(ebe.Id) as EnrollmentCount
                        FROM BenefitsPlans bp
                        LEFT JOIN EmployeeBenefitsEnrollments ebe ON bp.Id = ebe.BenefitsPlanId AND ebe.Status = 'ACTIVE'
                        LEFT JOIN Employees e ON ebe.EmployeeId = e.Id AND e.EmployeeType = 'Full-time'
                        WHERE bp.IsActive = 1
                        GROUP BY bp.PlanType";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string planType = reader["PlanType"].ToString();
                                int count = Convert.ToInt32(reader["EnrollmentCount"]);

                                switch (planType.ToUpper())
                                {
                                    case "HEALTH":
                                        lblHealthEnrollments.Text = count.ToString();
                                        break;
                                    case "DENTAL":
                                        lblDentalEnrollments.Text = count.ToString();
                                        break;
                                    case "VISION":
                                        lblVisionEnrollments.Text = count.ToString();
                                        break;
                                }
                            }
                        }
                    }

                    // Get eligible employees count (full-time only)
                    string eligibleQuery = "SELECT COUNT(*) FROM Employees WHERE EmployeeType = 'Full-time' AND Status = 'Active'";
                    using (SqlCommand cmd = new SqlCommand(eligibleQuery, conn))
                    {
                        int eligibleCount = Convert.ToInt32(cmd.ExecuteScalar());
                        lblEligibleEmployees.Text = eligibleCount.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading benefits overview: " + ex.Message, "error");
            }
        }

        private void LoadBenefitsPlans()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            Id, PlanName, PlanType, PlanCategory, Description,
                            MonthlyEmployeeCost, MonthlyEmployerCost, AnnualDeductible,
                            CoPayOfficeVisit, CoPaySpecialist, CoPayEmergency,
                            OutOfPocketMax, EffectiveDate, EndDate, IsActive
                        FROM BenefitsPlans 
                        WHERE IsActive = 1";

                    // Apply filters
                    if (!string.IsNullOrEmpty(ddlPlanType.SelectedValue))
                    {
                        query += " AND PlanType = @PlanType";
                    }

                    if (!string.IsNullOrEmpty(ddlPlanCategory.SelectedValue))
                    {
                        query += " AND PlanCategory = @PlanCategory";
                    }

                    query += " ORDER BY PlanType, PlanCategory, PlanName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(ddlPlanType.SelectedValue))
                            cmd.Parameters.AddWithValue("@PlanType", ddlPlanType.SelectedValue);

                        if (!string.IsNullOrEmpty(ddlPlanCategory.SelectedValue))
                            cmd.Parameters.AddWithValue("@PlanCategory", ddlPlanCategory.SelectedValue);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            gvBenefitsPlans.DataSource = dt;
                            gvBenefitsPlans.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading benefits plans: " + ex.Message, "error");
            }
        }

        private void LoadEmployeeEnrollments()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            ebe.Id as EnrollmentId,
                            ebe.EmployeeId,
                            e.EmployeeNumber,
                            e.FirstName + ' ' + e.LastName as EmployeeName,
                            bp.PlanName,
                            bp.PlanType,
                            bp.PlanCategory,
                            ebe.EnrollmentDate,
                            ebe.EffectiveDate,
                            ebe.EndDate,
                            ebe.Status,
                            ebe.DependentsCount,
                            ebe.MonthlyPremium,
                            ebe.PayrollDeduction,
                            ebe.Notes
                        FROM EmployeeBenefitsEnrollments ebe
                        INNER JOIN Employees e ON ebe.EmployeeId = e.Id
                        INNER JOIN BenefitsPlans bp ON ebe.BenefitsPlanId = bp.Id
                        WHERE ebe.Status = 'ACTIVE' AND e.EmployeeType = 'Full-time'";

                    // Apply search filter
                    if (!string.IsNullOrEmpty(txtSearchEmployee.Text.Trim()))
                    {
                        query += " AND (e.FirstName + ' ' + e.LastName LIKE @SearchTerm OR e.EmployeeNumber LIKE @SearchTerm)";
                    }

                    // Apply plan type filter
                    if (!string.IsNullOrEmpty(ddlPlanType.SelectedValue))
                    {
                        query += " AND bp.PlanType = @PlanType";
                    }

                    query += " ORDER BY e.LastName, e.FirstName, bp.PlanType";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(txtSearchEmployee.Text.Trim()))
                            cmd.Parameters.AddWithValue("@SearchTerm", "%" + txtSearchEmployee.Text.Trim() + "%");

                        if (!string.IsNullOrEmpty(ddlPlanType.SelectedValue))
                            cmd.Parameters.AddWithValue("@PlanType", ddlPlanType.SelectedValue);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            gvEmployeeEnrollments.DataSource = dt;
                            gvEmployeeEnrollments.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading employee enrollments: " + ex.Message, "error");
            }
        }

        private void LoadEmployeeDropdown()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT Id, EmployeeNumber, FirstName + ' ' + LastName + ' (' + EmployeeNumber + ')' as DisplayName
                        FROM Employees 
                        WHERE Status = 'Active' AND EmployeeType = 'Full-time'
                        ORDER BY LastName, FirstName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        ddlEmployeeSelect.Items.Clear();
                        ddlEmployeeSelect.Items.Add(new ListItem("Select Full-Time Employee", ""));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ddlEmployeeSelect.Items.Add(new ListItem(reader["DisplayName"].ToString(), reader["Id"].ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading employees: " + ex.Message, "error");
            }
        }

        private void LoadBenefitsPlansDropdown()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT Id, PlanName + ' (' + PlanType + ' - ' + PlanCategory + ')' as DisplayName, PlanType
                        FROM BenefitsPlans 
                        WHERE IsActive = 1 AND (EndDate IS NULL OR EndDate > GETUTCDATE())
                        ORDER BY PlanType, PlanCategory, PlanName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        ddlBenefitsPlan.Items.Clear();
                        ddlBenefitsPlan.Items.Add(new ListItem("Select Benefits Plan", ""));

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ddlBenefitsPlan.Items.Add(new ListItem(reader["DisplayName"].ToString(), reader["Id"].ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading benefits plans: " + ex.Message, "error");
            }
        }

        private void SetDefaultEffectiveDate()
        {
            txtEffectiveDate.Text = DateTime.Now.Date.ToString("yyyy-MM-dd");
        }

        #endregion

        #region Event Handlers

        protected void btnEnrollEmployee_Click(object sender, EventArgs e)
        {
            LoadEmployeeDropdown();
            LoadBenefitsPlansDropdown();
            SetDefaultEffectiveDate();

            // Show modal using JavaScript
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowModal", "showEnrollmentModal();", true);
        }

        protected void btnAddPlan_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/AddBenefitsPlan.aspx");
        }

        protected void btnSaveEnrollment_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlEmployeeSelect.SelectedValue) || string.IsNullOrEmpty(ddlBenefitsPlan.SelectedValue))
                {
                    ShowMessage("Please select both an employee and a benefits plan.", "error");
                    return;
                }

                int employeeId = Convert.ToInt32(ddlEmployeeSelect.SelectedValue);
                int benefitsPlanId = Convert.ToInt32(ddlBenefitsPlan.SelectedValue);
                DateTime effectiveDate = Convert.ToDateTime(txtEffectiveDate.Text);
                int dependentsCount = string.IsNullOrEmpty(txtDependentsCount.Text) ? 0 : Convert.ToInt32(txtDependentsCount.Text);
                string notes = txtEnrollmentNotes.Text.Trim();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("sp_EnrollEmployeeInBenefits", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        cmd.Parameters.AddWithValue("@BenefitsPlanId", benefitsPlanId);
                        cmd.Parameters.AddWithValue("@EffectiveDate", effectiveDate);
                        cmd.Parameters.AddWithValue("@DependentsCount", dependentsCount);
                        cmd.Parameters.AddWithValue("@Notes", string.IsNullOrEmpty(notes) ? (object)DBNull.Value : notes);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string status = reader["Status"].ToString();
                                string message = reader["Message"].ToString();

                                if (status == "SUCCESS")
                                {
                                    ShowMessage("Employee successfully enrolled in benefits plan.", "success");
                                    LoadBenefitsOverview();
                                    LoadEmployeeEnrollments();

                                    // Clear form
                                    ddlEmployeeSelect.SelectedIndex = 0;
                                    ddlBenefitsPlan.SelectedIndex = 0;
                                    txtDependentsCount.Text = "0";
                                    txtEnrollmentNotes.Text = "";
                                    SetDefaultEffectiveDate();

                                    // Close modal
                                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseModal", "closeModal();", true);
                                }
                                else
                                {
                                    ShowMessage("Error: " + message, "error");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error enrolling employee: " + ex.Message, "error");
            }
        }

        protected void ddlPlanType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBenefitsPlans();
            LoadEmployeeEnrollments();
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
            if (e.CommandName == "EditPlan")
            {
                int planId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"~/HR/EditBenefitsPlan.aspx?id={planId}");
            }
            else if (e.CommandName == "ViewDetails")
            {
                int planId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"~/HR/BenefitsPlanDetails.aspx?id={planId}");
            }
        }

        protected void gvEmployeeEnrollments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditEnrollment")
            {
                int enrollmentId = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"~/HR/EditBenefitsEnrollment.aspx?id={enrollmentId}");
            }
            else if (e.CommandName == "CancelEnrollment")
            {
                try
                {
                    int enrollmentId = Convert.ToInt32(e.CommandArgument);

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string query = @"
                            UPDATE EmployeeBenefitsEnrollments 
                            SET Status = 'CANCELLED', EndDate = CAST(GETUTCDATE() AS DATE), UpdatedAt = GETUTCDATE()
                            WHERE Id = @EnrollmentId";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@EnrollmentId", enrollmentId);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                ShowMessage("Benefits enrollment cancelled successfully.", "success");
                                LoadBenefitsOverview();
                                LoadEmployeeEnrollments();
                            }
                            else
                            {
                                ShowMessage("Error cancelling enrollment.", "error");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error cancelling enrollment: " + ex.Message, "error");
                }
            }
        }

        #endregion

        #region Report Generation

        protected void btnGenerateSummaryReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/Reports/BenefitsSummaryReport.aspx");
        }

        protected void btnGenerateEligibilityReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/Reports/BenefitsEligibilityReport.aspx");
        }

        protected void btnGenerateCostReport_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/Reports/BenefitsCostReport.aspx");
        }

        #endregion

        #region Helper Methods

        protected string GetEmployeeInitials(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return "??";

            string[] names = fullName.Split(' ');
            if (names.Length >= 2)
                return (names[0].Substring(0, 1) + names[1].Substring(0, 1)).ToUpper();
            else if (names.Length == 1)
                return names[0].Substring(0, Math.Min(2, names[0].Length)).ToUpper();
            else
                return "??";
        }

        protected string GetStatusCssClass(string status)
        {
            switch (status?.ToUpper())
            {
                case "ACTIVE":
                    return "badge badge-success";
                case "CANCELLED":
                    return "badge badge-danger";
                case "INACTIVE":
                    return "badge badge-secondary";
                default:
                    return "badge badge-secondary";
            }
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = message;

            string cssClass = "alert ";
            switch (type.ToLower())
            {
                case "success":
                    cssClass += "alert-success";
                    break;
                case "error":
                    cssClass += "alert-error";
                    break;
                case "warning":
                    cssClass += "alert-warning";
                    break;
                default:
                    cssClass += "alert-info";
                    break;
            }

            pnlMessage.CssClass = cssClass;
        }

        #endregion
    }
}