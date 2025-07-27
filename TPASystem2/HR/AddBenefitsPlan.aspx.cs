using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Configuration;

namespace TPASystem2.HR
{
    public partial class AddBenefitsPlan : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetDefaultValues();
            }
        }

        private void SetDefaultValues()
        {
            // Set default effective date to today
            txtEffectiveDate.Text = DateTime.Now.Date.ToString("yyyy-MM-dd");

            // Set default checkbox states
            chkIsActive.Checked = true;
            chkAutoEnroll.Checked = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (SaveBenefitsPlan())
                {
                    ShowMessage("Benefits plan saved successfully!", "success");
                    Response.Redirect("~/HR/BenefitsManagement.aspx");
                }
            }
        }

        protected void btnSaveAndAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (SaveBenefitsPlan())
                {
                    ShowMessage("Benefits plan saved successfully! You can add another plan.", "success");
                    ClearForm();
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/BenefitsManagement.aspx");
        }

        private bool SaveBenefitsPlan()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First, insert the benefits plan
                    string insertQuery = @"
                        INSERT INTO BenefitsPlans 
                        (PlanName, PlanType, PlanCategory, Description, MonthlyEmployeeCost, 
                         MonthlyEmployerCost, AnnualDeductible, CoPayOfficeVisit, CoPaySpecialist, 
                         CoPayEmergency, OutOfPocketMax, CoverageDetails, EffectiveDate, EndDate, 
                         IsActive, CreatedAt, UpdatedAt)
                        OUTPUT INSERTED.Id
                        VALUES 
                        (@PlanName, @PlanType, @PlanCategory, @Description, @MonthlyEmployeeCost, 
                         @MonthlyEmployerCost, @AnnualDeductible, @CoPayOfficeVisit, @CoPaySpecialist, 
                         @CoPayEmergency, @OutOfPocketMax, @CoverageDetails, @EffectiveDate, @EndDate, 
                         @IsActive, GETUTCDATE(), GETUTCDATE())";

                    int newPlanId = 0;
                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        // Required fields
                        cmd.Parameters.AddWithValue("@PlanName", txtPlanName.Text.Trim());
                        cmd.Parameters.AddWithValue("@PlanType", ddlPlanType.SelectedValue);
                        cmd.Parameters.AddWithValue("@PlanCategory", ddlPlanCategory.SelectedValue);
                        cmd.Parameters.AddWithValue("@MonthlyEmployeeCost", Convert.ToDecimal(txtMonthlyEmployeeCost.Text));
                        cmd.Parameters.AddWithValue("@MonthlyEmployerCost", Convert.ToDecimal(txtMonthlyEmployerCost.Text));
                        cmd.Parameters.AddWithValue("@EffectiveDate", Convert.ToDateTime(txtEffectiveDate.Text));
                        cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked);

                        // Optional fields
                        cmd.Parameters.AddWithValue("@Description",
                            string.IsNullOrEmpty(txtDescription.Text.Trim()) ? (object)DBNull.Value : txtDescription.Text.Trim());

                        cmd.Parameters.AddWithValue("@AnnualDeductible",
                            string.IsNullOrEmpty(txtAnnualDeductible.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtAnnualDeductible.Text));

                        cmd.Parameters.AddWithValue("@CoPayOfficeVisit",
                            string.IsNullOrEmpty(txtCoPayOfficeVisit.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtCoPayOfficeVisit.Text));

                        cmd.Parameters.AddWithValue("@CoPaySpecialist",
                            string.IsNullOrEmpty(txtCoPaySpecialist.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtCoPaySpecialist.Text));

                        cmd.Parameters.AddWithValue("@CoPayEmergency",
                            string.IsNullOrEmpty(txtCoPayEmergency.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtCoPayEmergency.Text));

                        cmd.Parameters.AddWithValue("@OutOfPocketMax",
                            string.IsNullOrEmpty(txtOutOfPocketMax.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtOutOfPocketMax.Text));

                        cmd.Parameters.AddWithValue("@CoverageDetails",
                            string.IsNullOrEmpty(txtCoverageDetails.Text.Trim()) ? (object)DBNull.Value : txtCoverageDetails.Text.Trim());

                        cmd.Parameters.AddWithValue("@EndDate",
                            string.IsNullOrEmpty(txtEndDate.Text) ? (object)DBNull.Value : Convert.ToDateTime(txtEndDate.Text));

                        newPlanId = (int)cmd.ExecuteScalar();
                    }

                    // Auto-enroll eligible employees if this is a basic plan and auto-enrollment is enabled
                    if (newPlanId > 0 && ddlPlanCategory.SelectedValue == "BASIC" && chkIsActive.Checked && chkAutoEnroll.Checked)
                    {
                        AutoEnrollEligibleEmployees(conn, newPlanId);
                    }

                    return newPlanId > 0;
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error saving benefits plan: " + ex.Message, "error");
                return false;
            }
        }

        private void AutoEnrollEligibleEmployees(SqlConnection conn, int planId)
        {
            try
            {
                // Auto-enroll full-time employees who don't have this type of benefit
                string autoEnrollQuery = @"
                    INSERT INTO EmployeeBenefitsEnrollments 
                    (EmployeeId, BenefitsPlanId, EnrollmentDate, EffectiveDate, Status, 
                     DependentsCount, MonthlyPremium, PayrollDeduction, Notes, CreatedAt, UpdatedAt)
                    SELECT 
                        e.Id,
                        @PlanId,
                        GETUTCDATE(),
                        @EffectiveDate,
                        'ACTIVE',
                        0,
                        @MonthlyEmployeeCost,
                        @MonthlyEmployeeCost,
                        'Auto-enrolled in basic benefits plan',
                        GETUTCDATE(),
                        GETUTCDATE()
                    FROM Employees e
                    WHERE e.EmployeeType = 'Full-time' 
                        AND e.Status = 'Active'
                        AND NOT EXISTS (
                            SELECT 1 FROM EmployeeBenefitsEnrollments ebe 
                            INNER JOIN BenefitsPlans bp ON ebe.BenefitsPlanId = bp.Id
                            WHERE ebe.EmployeeId = e.Id 
                                AND bp.PlanType = @PlanType 
                                AND ebe.Status = 'ACTIVE'
                        )";

                using (SqlCommand cmd = new SqlCommand(autoEnrollQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@PlanId", planId);
                    cmd.Parameters.AddWithValue("@EffectiveDate", Convert.ToDateTime(txtEffectiveDate.Text));
                    cmd.Parameters.AddWithValue("@MonthlyEmployeeCost", Convert.ToDecimal(txtMonthlyEmployeeCost.Text));
                    cmd.Parameters.AddWithValue("@PlanType", ddlPlanType.SelectedValue);

                    int enrolledCount = cmd.ExecuteNonQuery();

                    if (enrolledCount > 0)
                    {
                        ShowMessage($"Benefits plan created and {enrolledCount} eligible full-time employees auto-enrolled.", "success");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't fail the main operation
                ShowMessage("Benefits plan created successfully, but auto-enrollment encountered an error: " + ex.Message, "warning");
            }
        }

        private void ClearForm()
        {
            // Clear all form fields
            txtPlanName.Text = "";
            ddlPlanType.SelectedIndex = 0;
            ddlPlanCategory.SelectedIndex = 0;
            txtDescription.Text = "";
            txtMonthlyEmployeeCost.Text = "";
            txtMonthlyEmployerCost.Text = "";
            txtAnnualDeductible.Text = "";
            txtCoPayOfficeVisit.Text = "";
            txtCoPaySpecialist.Text = "";
            txtCoPayEmergency.Text = "";
            txtOutOfPocketMax.Text = "";
            txtCoverageDetails.Text = "";
            txtEndDate.Text = "";

            // Reset defaults
            SetDefaultValues();
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
    }
}