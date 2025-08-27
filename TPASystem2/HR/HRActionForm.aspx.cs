using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;

namespace TPASystem2.HR
{
    public partial class HRActionForm : System.Web.UI.Page
    {
        #region Properties and Fields

        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int CurrentUserId => Convert.ToInt32(Session["UserId"] ?? 0);
        private string CurrentUserRole => Session["UserRole"]?.ToString() ?? "";
        private string CurrentUserEmail => Session["UserEmail"]?.ToString() ?? "";
        private string CurrentUserName => Session["UserName"]?.ToString() ?? "";

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CheckUserAuthentication();
                InitializePage();
                LoadEmployeeData();
                LoadUserInfo();
            }
        }

        #endregion

        #region Initialization Methods

        private void CheckUserAuthentication()
        {
            if (CurrentUserId == 0)
            {
                Response.Redirect("~/Login.aspx", false);
                return;
            }

            // Check if user has permission to submit HR actions
            if (!CanSubmitHRAction())
            {
                ShowError("You do not have permission to submit HR Action forms.");
                btnSubmit.Enabled = false;
                btnSaveDraft.Enabled = false;
            }
        }

        private bool CanSubmitHRAction()
        {
            // HR admins can submit for any employee, others can only submit for themselves
            return CurrentUserRole.ToUpper().Contains("HRADMIN") || CurrentUserRole.ToUpper().Contains("ADMIN") || CurrentUserRole.ToUpper().Contains("SUPERADMIN");
        }

        private void InitializePage()
        {
            // Set today's date for effective date by default
            txtEffectiveDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

            // Hide form sections by default (JavaScript will handle showing them)
            Page.ClientScript.RegisterStartupScript(this.GetType(), "InitializeSections",
                "document.getElementById('rateChangeSection').style.display = 'none';" +
                "document.getElementById('transferSection').style.display = 'none';" +
                "document.getElementById('promotionSection').style.display = 'none';" +
                "document.getElementById('statusChangeSection').style.display = 'none';", true);
        }

        private void LoadEmployeeData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT e.Id, e.EmployeeNumber, 
                               e.FirstName + ' ' + e.LastName AS FullName,
                               d.Name AS DepartmentName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        WHERE e.IsActive = 1
                        ORDER BY e.FirstName, e.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        ddlEmployee.Items.Clear();
                        ddlEmployee.Items.Add(new ListItem("Select Employee...", ""));

                        while (reader.Read())
                        {
                            string displayText = $"{reader["FullName"]} ({reader["EmployeeNumber"]}) - {reader["DepartmentName"]}";
                            ddlEmployee.Items.Add(new ListItem(displayText, reader["Id"].ToString()));
                        }
                    }
                }

                // If not HR admin, pre-select current user
                if (!CanSubmitHRAction())
                {
                    ddlEmployee.SelectedValue = GetCurrentUserEmployeeId().ToString();
                    ddlEmployee.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowError("Error loading employee data. Please try again.");
            }
        }

        private void LoadUserInfo()
        {
            try
            {
                litCurrentUser.Text = CurrentUserName;

                // Get user's employee number
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT e.EmployeeNumber 
                        FROM Users u 
                        INNER JOIN Employees e ON u.Id = e.UserId 
                        WHERE u.Id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        object result = cmd.ExecuteScalar();
                        litUserNumber.Text = result?.ToString() ?? "N/A";
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private int GetCurrentUserEmployeeId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT e.Id FROM Users u INNER JOIN Employees e ON u.Id = e.UserId WHERE u.Id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #region Event Handlers

        protected void ddlEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlEmployee.SelectedValue))
            {
                LoadSelectedEmployeeInfo(Convert.ToInt32(ddlEmployee.SelectedValue));
            }
            else
            {
                ClearEmployeeInfo();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                SubmitHRAction("PENDING");
            }
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            if (ValidateBasicFields())
            {
                SubmitHRAction("DRAFT");
            }
        }

        protected void btnViewExistingRequests_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/HRActionsList.aspx", false);
        }

        #endregion

        #region Employee Data Loading

        private void LoadSelectedEmployeeInfo(int employeeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT e.FirstName + ' ' + e.LastName AS FullName,
                               e.Email, e.PhoneNumber, e.Address, e.City, e.State, e.ZipCode,
                               e.JobTitle, e.Salary, e.WorkLocation, e.EmployeeType
                        FROM Employees e
                        WHERE e.Id = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtEmployeeName.Text = reader["FullName"].ToString();
                                txtNewEmail.Text = reader["Email"].ToString();
                                txtNewPhone.Text = reader["PhoneNumber"].ToString();

                                // Construct full address
                                var addressParts = new StringBuilder();
                                if (!string.IsNullOrEmpty(reader["Address"].ToString()))
                                    addressParts.Append(reader["Address"].ToString());
                                if (!string.IsNullOrEmpty(reader["City"].ToString()))
                                    addressParts.Append($", {reader["City"]}");
                                if (!string.IsNullOrEmpty(reader["State"].ToString()))
                                    addressParts.Append($", {reader["State"]}");
                                if (!string.IsNullOrEmpty(reader["ZipCode"].ToString()))
                                    addressParts.Append($" {reader["ZipCode"]}");

                                txtNewAddress.Text = addressParts.ToString();
                                txtOldJobTitle.Text = reader["JobTitle"].ToString();
                                txtPreviousRate.Text = reader["Salary"].ToString();
                                txtNewLocation.Text = reader["WorkLocation"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowError("Error loading employee information. Please try again.");
            }
        }

        private void ClearEmployeeInfo()
        {
            txtEmployeeName.Text = "";
            txtNewEmail.Text = "";
            txtNewPhone.Text = "";
            txtNewAddress.Text = "";
            txtOldJobTitle.Text = "";
            txtPreviousRate.Text = "";
            txtNewLocation.Text = "";
        }

        #endregion

        #region Form Validation

        private bool ValidateBasicFields()
        {
            if (string.IsNullOrEmpty(ddlEmployee.SelectedValue))
            {
                ShowError("Please select an employee.");
                return false;
            }

            return true;
        }

        private bool ValidateForm()
        {
            if (!ValidateBasicFields())
                return false;

            // Check if at least one action type is selected
            if (!chkRateChange.Checked && !chkTransfer.Checked &&
                !chkPromotion.Checked && !chkStatusChange.Checked)
            {
                ShowError("Please select at least one action type (Rate Change, Transfer, Promotion, or Status Change).");
                return false;
            }

            // Validate specific sections based on what's checked
            if (chkRateChange.Checked)
            {
                if (!ValidateRateChangeSection())
                    return false;
            }

            if (chkTransfer.Checked)
            {
                if (!ValidateTransferSection())
                    return false;
            }

            if (chkPromotion.Checked)
            {
                if (!ValidatePromotionSection())
                    return false;
            }

            // Validate dates if provided
            if (!string.IsNullOrEmpty(txtLeaveStartDate.Text) && !string.IsNullOrEmpty(txtLeaveEndDate.Text))
            {
                if (DateTime.Parse(txtLeaveEndDate.Text) < DateTime.Parse(txtLeaveStartDate.Text))
                {
                    ShowError("Leave end date must be after start date.");
                    return false;
                }
            }

            return true;
        }

        private bool ValidateRateChangeSection()
        {
            decimal previousRate, newRate;

            if (string.IsNullOrEmpty(txtPreviousRate.Text) || !decimal.TryParse(txtPreviousRate.Text, out previousRate))
            {
                ShowError("Please enter a valid previous rate/salary for rate change.");
                return false;
            }

            if (string.IsNullOrEmpty(txtNewRate.Text) || !decimal.TryParse(txtNewRate.Text, out newRate))
            {
                ShowError("Please enter a valid new rate/salary for rate change.");
                return false;
            }

            if (!rbSalary.Checked && !rbHourly.Checked)
            {
                ShowError("Please select rate type (Salary or Hourly) for rate change.");
                return false;
            }

            return true;
        }

        private bool ValidateTransferSection()
        {
            if (string.IsNullOrEmpty(txtNewLocation.Text))
            {
                ShowError("Please enter the new location for transfer.");
                return false;
            }

            return true;
        }

        private bool ValidatePromotionSection()
        {
            if (string.IsNullOrEmpty(txtNewJobTitle.Text))
            {
                ShowError("Please enter the new job title for promotion.");
                return false;
            }

            return true;
        }

        #endregion

        #region Form Submission

        private void SubmitHRAction(string status)
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
                            string insertQuery = @"
                                INSERT INTO HRActions (
                                    EmployeeId, RequestedById, EmployeeName, SocialSecurityNumber, 
                                    Classification, EmployeeType, Shift, TotalHours,
                                    IsRateChange, PreviousRateSalary, RateType, AmountOfIncrease, 
                                    PremiumIncentive, NewRateSalary, NewW4Status, ShiftHours,
                                    IsTransfer, NewLocation, LeaderSupervisor, NewClass, TransferShiftHours,
                                    IsPromotion, OldJobTitle, NewJobTitle, PromotionNewRateSalary, PromotionShiftHours,
                                    IsStatusChange, StatusFromFT, StatusFromPT, StatusFromPRN,
                                    StatusToFT, StatusToPT, StatusToPRN, StatusFromSalary, StatusFromHourly,
                                    StatusToSalary, StatusToHourly, MaritalStatus,
                                    NewName, NewPhone, NewAddress, NewEmail,
                                    HealthInsurance, HealthDeduct, DentalInsurance, DentalDeduct,
                                    Retirement403b, RetirementDeduct, DisabilityInsurance, EffectiveDate,
                                    PayrollDeductionReason, PayrollDeductionAmount, PayrollDeductionEachPayPeriod,
                                    LeaveType, IsVacation, IsPaidSickTime, IsUnpaidSickTime, IsUnpaidPersonalTime,
                                    IsFamilyMedicalFMLA, IsFuneral, FuneralRelation, IsVoting, IsPregnancyDisability,
                                    IsMilitary, IsJuryDuty, LeaveStartDate, LeaveEndDate, NumberOfDays, TotalLeaveHours,
                                    DateReturnedToWork, LastDayWorked, NumberOfDaysOut, HoursOut,
                                    AbsenceExcused, DoctorSlipReceived, ExcusedHours, AccommodationNeeded,
                                    AdditionalComments, Status, RequestDate
                                ) VALUES (
                                    @EmployeeId, @RequestedById, @EmployeeName, @SSN,
                                    @Classification, @EmployeeType, @Shift, @TotalHours,
                                    @IsRateChange, @PreviousRate, @RateType, @AmountIncrease,
                                    @PremiumIncentive, @NewRate, @NewW4Status, @ShiftHours,
                                    @IsTransfer, @NewLocation, @LeaderSupervisor, @NewClass, @TransferShiftHours,
                                    @IsPromotion, @OldJobTitle, @NewJobTitle, @PromotionNewRate, @PromotionShiftHours,
                                    @IsStatusChange, @StatusFromFT, @StatusFromPT, @StatusFromPRN,
                                    @StatusToFT, @StatusToPT, @StatusToPRN, @StatusFromSalary, @StatusFromHourly,
                                    @StatusToSalary, @StatusToHourly, @MaritalStatus,
                                    @NewName, @NewPhone, @NewAddress, @NewEmail,
                                    @HealthInsurance, @HealthDeduct, @DentalInsurance, @DentalDeduct,
                                    @Retirement403b, @RetirementDeduct, @DisabilityInsurance, @EffectiveDate,
                                    @PayrollReason, @PayrollAmount, @EachPayPeriod,
                                    @LeaveType, @IsVacation, @IsPaidSickTime, @IsUnpaidSickTime, @IsUnpaidPersonalTime,
                                    @IsFamilyMedical, @IsFuneral, @FuneralRelation, @IsVoting, @IsPregnancyDisability,
                                    @IsMilitary, @IsJuryDuty, @LeaveStartDate, @LeaveEndDate, @NumberOfDays, @LeaveHours,
                                    @DateReturned, @LastDayWorked, @DaysOut, @HoursOut,
                                    @AbsenceExcused, @DoctorSlipReceived, @ExcusedHours, @Accommodation,
                                    @AdditionalComments, @Status, @RequestDate
                                ); SELECT SCOPE_IDENTITY();";

                            using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                            {
                                AddHRActionParameters(cmd, status);

                                int newHRActionId = Convert.ToInt32(cmd.ExecuteScalar());

                                // If submitting (not draft), create workflow approval record
                                if (status == "PENDING")
                                {
                                    CreateWorkflowApproval(newHRActionId, conn, transaction);
                                    SendNotificationToHRAdmin(newHRActionId, conn, transaction);
                                }

                                transaction.Commit();

                                // Show success message
                                string message = status == "PENDING"
                                    ? $"HR Action form submitted successfully! Reference ID: {newHRActionId}. HR Admin will review your request."
                                    : $"HR Action form saved as draft successfully! Reference ID: {newHRActionId}.";

                                ShowSuccess(message);

                                // Clear form after successful submission
                                if (status == "PENDING")
                                {
                                    ClearForm();
                                }

                                // Log activity
                                LogUserActivity("HR Action " + status, $"HR Action {status.ToLower()} for employee ID: {ddlEmployee.SelectedValue}");
                            }
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
                LogError(ex);
                ShowError("Error submitting HR Action form. Please try again.");
            }
        }

        private void AddHRActionParameters(SqlCommand cmd, string status)
        {
            // Basic information
            cmd.Parameters.AddWithValue("@EmployeeId", Convert.ToInt32(ddlEmployee.SelectedValue));
            cmd.Parameters.AddWithValue("@RequestedById", GetCurrentUserEmployeeId());
            cmd.Parameters.AddWithValue("@EmployeeName", txtEmployeeName.Text.Trim());
            cmd.Parameters.AddWithValue("@SSN", txtSSN.Text.Trim());
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@RequestDate", DateTime.Now);

            // Classification
            var classification = new StringBuilder();
            if (chkFullTime.Checked) classification.Append("FT,");
            if (chkPartTime.Checked) classification.Append("PT,");
            if (chkPRN.Checked) classification.Append("PRN,");
            cmd.Parameters.AddWithValue("@Classification", classification.ToString().TrimEnd(','));

            cmd.Parameters.AddWithValue("@EmployeeType", txtEmployeeName.Text.Trim()); // You might want to add this field
            cmd.Parameters.AddWithValue("@Shift", txtShift.Text.Trim());
            cmd.Parameters.AddWithValue("@TotalHours", string.IsNullOrEmpty(txtTotalHours.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtTotalHours.Text));

            // Rate Change
            cmd.Parameters.AddWithValue("@IsRateChange", chkRateChange.Checked);
            cmd.Parameters.AddWithValue("@PreviousRate", string.IsNullOrEmpty(txtPreviousRate.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtPreviousRate.Text));
            cmd.Parameters.AddWithValue("@RateType", rbSalary.Checked ? "Salary" : rbHourly.Checked ? "Hourly" : (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@AmountIncrease", string.IsNullOrEmpty(txtAmountIncrease.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtAmountIncrease.Text));
            cmd.Parameters.AddWithValue("@PremiumIncentive", string.IsNullOrEmpty(txtPremiumIncentive.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtPremiumIncentive.Text));
            cmd.Parameters.AddWithValue("@NewRate", string.IsNullOrEmpty(txtNewRate.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtNewRate.Text));
            cmd.Parameters.AddWithValue("@NewW4Status", txtNewW4Status.Text.Trim());
            cmd.Parameters.AddWithValue("@ShiftHours", txtRateChangeShiftHours.Text.Trim());

            // Transfer
            cmd.Parameters.AddWithValue("@IsTransfer", chkTransfer.Checked);
            cmd.Parameters.AddWithValue("@NewLocation", txtNewLocation.Text.Trim());
            cmd.Parameters.AddWithValue("@LeaderSupervisor", txtLeaderSupervisor.Text.Trim());
            cmd.Parameters.AddWithValue("@NewClass", txtNewClass.Text.Trim());
            cmd.Parameters.AddWithValue("@TransferShiftHours", txtTransferShiftHours.Text.Trim());

            // Promotion
            cmd.Parameters.AddWithValue("@IsPromotion", chkPromotion.Checked);
            cmd.Parameters.AddWithValue("@OldJobTitle", txtOldJobTitle.Text.Trim());
            cmd.Parameters.AddWithValue("@NewJobTitle", txtNewJobTitle.Text.Trim());
            cmd.Parameters.AddWithValue("@PromotionNewRate", string.IsNullOrEmpty(txtPromotionNewRate.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtPromotionNewRate.Text));
            cmd.Parameters.AddWithValue("@PromotionShiftHours", txtPromotionShiftHours.Text.Trim());

            // Status Change
            cmd.Parameters.AddWithValue("@IsStatusChange", chkStatusChange.Checked);
            cmd.Parameters.AddWithValue("@StatusFromFT", chkFromFT.Checked);
            cmd.Parameters.AddWithValue("@StatusFromPT", chkFromPT.Checked);
            cmd.Parameters.AddWithValue("@StatusFromPRN", chkFromPRN.Checked);
            cmd.Parameters.AddWithValue("@StatusToFT", chkToFT.Checked);
            cmd.Parameters.AddWithValue("@StatusToPT", chkToPT.Checked);
            cmd.Parameters.AddWithValue("@StatusToPRN", chkToPRN.Checked);
            cmd.Parameters.AddWithValue("@StatusFromSalary", chkFromSalary.Checked);
            cmd.Parameters.AddWithValue("@StatusFromHourly", chkFromHourly.Checked);
            cmd.Parameters.AddWithValue("@StatusToSalary", chkToSalary.Checked);
            cmd.Parameters.AddWithValue("@StatusToHourly", chkToHourly.Checked);
            cmd.Parameters.AddWithValue("@MaritalStatus", rbMarried.Checked ? "M" : rbDivorced.Checked ? "D" : rbSingle.Checked ? "S" : (object)DBNull.Value);

            // Contact Information
            cmd.Parameters.AddWithValue("@NewName", txtNewName.Text.Trim());
            cmd.Parameters.AddWithValue("@NewPhone", txtNewPhone.Text.Trim());
            cmd.Parameters.AddWithValue("@NewAddress", txtNewAddress.Text.Trim());
            cmd.Parameters.AddWithValue("@NewEmail", txtNewEmail.Text.Trim());

            // Insurance/Benefits
            var healthInsurance = new StringBuilder();
            if (chkHealthS.Checked) healthInsurance.Append("S,");
            if (chkHealthES.Checked) healthInsurance.Append("E+S,");
            if (chkHealthEC.Checked) healthInsurance.Append("E+C,");
            if (chkHealthF.Checked) healthInsurance.Append("F,");
            cmd.Parameters.AddWithValue("@HealthInsurance", healthInsurance.ToString().TrimEnd(','));

            cmd.Parameters.AddWithValue("@HealthDeduct", string.IsNullOrEmpty(txtHealthDeduct.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtHealthDeduct.Text));

            var dentalInsurance = new StringBuilder();
            if (chkDentalS.Checked) dentalInsurance.Append("S,");
            if (chkDentalES.Checked) dentalInsurance.Append("E+S,");
            if (chkDentalEC.Checked) dentalInsurance.Append("E+C,");
            if (chkDentalF.Checked) dentalInsurance.Append("F,");
            cmd.Parameters.AddWithValue("@DentalInsurance", dentalInsurance.ToString().TrimEnd(','));

            cmd.Parameters.AddWithValue("@DentalDeduct", string.IsNullOrEmpty(txtDentalDeduct.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtDentalDeduct.Text));
            cmd.Parameters.AddWithValue("@Retirement403b", chkRetirement403b.Checked);
            cmd.Parameters.AddWithValue("@RetirementDeduct", string.IsNullOrEmpty(txtRetirementDeduct.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtRetirementDeduct.Text));
            cmd.Parameters.AddWithValue("@DisabilityInsurance", chkDisabilityMe.Checked);
            cmd.Parameters.AddWithValue("@EffectiveDate", string.IsNullOrEmpty(txtEffectiveDate.Text) ? (object)DBNull.Value : DateTime.Parse(txtEffectiveDate.Text));

            // Payroll Deduction
            cmd.Parameters.AddWithValue("@PayrollReason", txtPayrollReason.Text.Trim());
            cmd.Parameters.AddWithValue("@PayrollAmount", string.IsNullOrEmpty(txtPayrollAmount.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtPayrollAmount.Text));
            cmd.Parameters.AddWithValue("@EachPayPeriod", chkEachPayPeriod.Checked);

            // Leave of Absence
            var leaveTypes = new StringBuilder();
            if (chkVacation.Checked) leaveTypes.Append("Vacation,");
            if (chkPaidSickTime.Checked) leaveTypes.Append("Paid Sick Time,");
            if (chkUnpaidSickTime.Checked) leaveTypes.Append("Unpaid Sick Time,");
            if (chkUnpaidPersonalTime.Checked) leaveTypes.Append("Unpaid Personal Time,");
            if (chkFamilyMedical.Checked) leaveTypes.Append("Family/Medical (FMLA),");
            if (chkFuneral.Checked) leaveTypes.Append("Funeral,");
            if (chkVoting.Checked) leaveTypes.Append("Voting,");
            if (chkPregnancyDisability.Checked) leaveTypes.Append("Pregnancy Disability,");
            if (chkMilitary.Checked) leaveTypes.Append("Military,");
            if (chkJuryDuty.Checked) leaveTypes.Append("Jury Duty,");
            cmd.Parameters.AddWithValue("@LeaveType", leaveTypes.ToString().TrimEnd(','));

            cmd.Parameters.AddWithValue("@IsVacation", chkVacation.Checked);
            cmd.Parameters.AddWithValue("@IsPaidSickTime", chkPaidSickTime.Checked);
            cmd.Parameters.AddWithValue("@IsUnpaidSickTime", chkUnpaidSickTime.Checked);
            cmd.Parameters.AddWithValue("@IsUnpaidPersonalTime", chkUnpaidPersonalTime.Checked);
            cmd.Parameters.AddWithValue("@IsFamilyMedical", chkFamilyMedical.Checked);
            cmd.Parameters.AddWithValue("@IsFuneral", chkFuneral.Checked);
            cmd.Parameters.AddWithValue("@FuneralRelation", txtFuneralRelation.Text.Trim());
            cmd.Parameters.AddWithValue("@IsVoting", chkVoting.Checked);
            cmd.Parameters.AddWithValue("@IsPregnancyDisability", chkPregnancyDisability.Checked);
            cmd.Parameters.AddWithValue("@IsMilitary", chkMilitary.Checked);
            cmd.Parameters.AddWithValue("@IsJuryDuty", chkJuryDuty.Checked);

            cmd.Parameters.AddWithValue("@LeaveStartDate", string.IsNullOrEmpty(txtLeaveStartDate.Text) ? (object)DBNull.Value : DateTime.Parse(txtLeaveStartDate.Text));
            cmd.Parameters.AddWithValue("@LeaveEndDate", string.IsNullOrEmpty(txtLeaveEndDate.Text) ? (object)DBNull.Value : DateTime.Parse(txtLeaveEndDate.Text));
            cmd.Parameters.AddWithValue("@NumberOfDays", string.IsNullOrEmpty(txtNumberOfDays.Text) ? (object)DBNull.Value : Convert.ToInt32(txtNumberOfDays.Text));
            cmd.Parameters.AddWithValue("@LeaveHours", string.IsNullOrEmpty(txtLeaveHours.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtLeaveHours.Text));

            // Return from Leave
            cmd.Parameters.AddWithValue("@DateReturned", string.IsNullOrEmpty(txtDateReturned.Text) ? (object)DBNull.Value : DateTime.Parse(txtDateReturned.Text));
            cmd.Parameters.AddWithValue("@LastDayWorked", string.IsNullOrEmpty(txtLastDayWorked.Text) ? (object)DBNull.Value : DateTime.Parse(txtLastDayWorked.Text));
            cmd.Parameters.AddWithValue("@DaysOut", string.IsNullOrEmpty(txtDaysOut.Text) ? (object)DBNull.Value : Convert.ToInt32(txtDaysOut.Text));
            cmd.Parameters.AddWithValue("@HoursOut", string.IsNullOrEmpty(txtHoursOut.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtHoursOut.Text));
            cmd.Parameters.AddWithValue("@AbsenceExcused", rbAbsenceYes.Checked ? true : rbAbsenceNo.Checked ? false : (bool?)null);
            cmd.Parameters.AddWithValue("@DoctorSlipReceived", rbDoctorSlipYes.Checked ? true : rbDoctorSlipNo.Checked ? false : (bool?)null);
            cmd.Parameters.AddWithValue("@ExcusedHours", string.IsNullOrEmpty(txtExcusedHours.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtExcusedHours.Text));
            cmd.Parameters.AddWithValue("@Accommodation", txtAccommodation.Text.Trim());

            // Additional Comments
            cmd.Parameters.AddWithValue("@AdditionalComments", txtAdditionalComments.Text.Trim());
        }

        private void CreateWorkflowApproval(int hrActionId, SqlConnection conn, SqlTransaction transaction)
        {
            // Get HR Admin workflow step
            string workflowQuery = @"
                SELECT Id FROM ApprovalWorkflows 
                WHERE EntityType = 'HRAction' AND StepNumber = 1 AND IsActive = 1";

            using (SqlCommand cmd = new SqlCommand(workflowQuery, conn, transaction))
            {
                int workflowStepId = Convert.ToInt32(cmd.ExecuteScalar());

                // Get HR Admin user ID
                int hrAdminId = GetHRAdminForApproval(conn, transaction);

                if (hrAdminId > 0)
                {
                    string insertApprovalQuery = @"
                        INSERT INTO WorkflowApprovals 
                        (EntityType, EntityId, WorkflowStepId, AssignedToId, Status, DueDate, CreatedAt, UpdatedAt)
                        VALUES 
                        ('HRAction', @EntityId, @WorkflowStepId, @AssignedToId, 'PENDING', @DueDate, @CreatedAt, @UpdatedAt)";

                    using (SqlCommand approvalCmd = new SqlCommand(insertApprovalQuery, conn, transaction))
                    {
                        approvalCmd.Parameters.AddWithValue("@EntityId", hrActionId);
                        approvalCmd.Parameters.AddWithValue("@WorkflowStepId", workflowStepId);
                        approvalCmd.Parameters.AddWithValue("@AssignedToId", hrAdminId);
                        approvalCmd.Parameters.AddWithValue("@DueDate", DateTime.Now.AddDays(3)); // 3 day deadline
                        approvalCmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                        approvalCmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                        approvalCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private int GetHRAdminForApproval(SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"
                SELECT TOP 1 e.Id 
                FROM Users u 
                INNER JOIN Employees e ON u.Id = e.UserId 
                WHERE u.Role LIKE '%HRADMIN%' AND u.IsActive = 1 AND e.IsActive = 1
                ORDER BY e.HireDate"; // Assign to most senior HR Admin

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        #endregion

        #region Notification Methods

        private void SendNotificationToHRAdmin(int hrActionId, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                // Get HR Admin email
                string emailQuery = @"
                    SELECT u.Email, u.FirstName + ' ' + u.LastName as FullName
                    FROM Users u 
                    INNER JOIN Employees e ON u.Id = e.UserId 
                    WHERE u.Role LIKE '%HRADMIN%' AND u.IsActive = 1 AND e.IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(emailQuery, conn, transaction))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string hrAdminEmail = reader["Email"].ToString();
                        string hrAdminName = reader["FullName"].ToString();

                        if (!string.IsNullOrEmpty(hrAdminEmail))
                        {
                            SendApprovalEmail(hrAdminEmail, hrAdminName, hrActionId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log email error but don't fail the main transaction
                LogError(new Exception("Email notification failed", ex));
            }
        }

        private void SendApprovalEmail(string hrAdminEmail, string hrAdminName, int hrActionId)
        {
            try
            {
                string subject = $"HR Action Approval Required - Request #{hrActionId}";
                string body = CreateApprovalEmailBody(hrActionId, hrAdminName);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["FromEmail"] ?? "noreply@company.com");
                    mail.To.Add(hrAdminEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        // SMTP settings should be in web.config
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(new Exception("Failed to send approval email", ex));
            }
        }

        private string CreateApprovalEmailBody(int hrActionId, string hrAdminName)
        {
            StringBuilder emailBody = new StringBuilder();
            emailBody.AppendLine("<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>");
            emailBody.AppendLine($"<h2 style='color: #1976d2;'>HR Action Approval Required</h2>");
            emailBody.AppendLine($"<p>Dear {hrAdminName},</p>");
            emailBody.AppendLine($"<p>A new HR Action form has been submitted and requires your review and approval.</p>");

            emailBody.AppendLine("<div style='background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>");
            emailBody.AppendLine("<h3 style='margin-top: 0; color: #333;'>Request Details:</h3>");
            emailBody.AppendLine("<table style='width: 100%; border-collapse: collapse;'>");
            emailBody.AppendLine($"<tr><td style='padding: 8px; border-bottom: 1px solid #ddd;'><strong>Request ID:</strong></td><td style='padding: 8px; border-bottom: 1px solid #ddd;'>{hrActionId}</td></tr>");
            emailBody.AppendLine($"<tr><td style='padding: 8px; border-bottom: 1px solid #ddd;'><strong>Submitted By:</strong></td><td style='padding: 8px; border-bottom: 1px solid #ddd;'>{CurrentUserName}</td></tr>");
            emailBody.AppendLine($"<tr><td style='padding: 8px; border-bottom: 1px solid #ddd;'><strong>Employee:</strong></td><td style='padding: 8px; border-bottom: 1px solid #ddd;'>{txtEmployeeName.Text}</td></tr>");
            emailBody.AppendLine($"<tr><td style='padding: 8px; border-bottom: 1px solid #ddd;'><strong>Submission Date:</strong></td><td style='padding: 8px; border-bottom: 1px solid #ddd;'>{DateTime.Now:MMM dd, yyyy}</td></tr>");

            // Add action types
            StringBuilder actionTypes = new StringBuilder();
            if (chkRateChange.Checked) actionTypes.Append("Rate Change, ");
            if (chkTransfer.Checked) actionTypes.Append("Transfer, ");
            if (chkPromotion.Checked) actionTypes.Append("Promotion, ");
            if (chkStatusChange.Checked) actionTypes.Append("Status Change, ");

            emailBody.AppendLine($"<tr><td style='padding: 8px; border-bottom: 1px solid #ddd;'><strong>Action Types:</strong></td><td style='padding: 8px; border-bottom: 1px solid #ddd;'>{actionTypes.ToString().TrimEnd(',', ' ')}</td></tr>");
            emailBody.AppendLine("</table>");
            emailBody.AppendLine("</div>");

            emailBody.AppendLine("<p>Please log into the HR system to review and approve this request.</p>");

            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/");
            emailBody.AppendLine($"<p><a href='{baseUrl}HR/HRActionsList.aspx' style='background: #1976d2; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>Review Request</a></p>");

            emailBody.AppendLine("<p>Thank you,<br>TPA HR System</p>");
            emailBody.AppendLine("</div>");

            return emailBody.ToString();
        }

        #endregion

        #region Helper Methods

        private void ClearForm()
        {
            // Clear all form fields
            ddlEmployee.SelectedIndex = 0;
            txtEmployeeName.Text = "";
            txtSSN.Text = "";

            // Clear checkboxes
            chkFullTime.Checked = false;
            chkPartTime.Checked = false;
            chkPRN.Checked = false;

            chkRateChange.Checked = false;
            chkTransfer.Checked = false;
            chkPromotion.Checked = false;
            chkStatusChange.Checked = false;

            // Clear all text boxes
            foreach (Control control in Page.Controls)
            {
                ClearControlsRecursive(control);
            }

            // Reset effective date to today
            txtEffectiveDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
        }

        private void ClearControlsRecursive(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox textBox && textBox.ID != "txtEffectiveDate")
                {
                    textBox.Text = "";
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.Checked = false;
                }
                else if (control is RadioButton radioButton)
                {
                    radioButton.Checked = false;
                }
                else if (control.HasControls())
                {
                    ClearControlsRecursive(control);
                }
            }
        }

        private void ShowSuccess(string message)
        {
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
            litSuccessMessage.Text = message;
        }

        private void ShowError(string message)
        {
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
            litErrorMessage.Text = message;
        }

        private void LogError(Exception ex)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ErrorLogs (ErrorMessage, StackTrace, Source, Timestamp, RequestUrl, 
                                             UserAgent, IPAddress, UserId, Severity, CreatedAt)
                        VALUES (@ErrorMessage, @StackTrace, @Source, @Timestamp, @RequestUrl,
                                @UserAgent, @IPAddress, @UserId, @Severity, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message);
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", ex.Source ?? "HRActionForm");
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@RequestUrl", Request.Url?.ToString() ?? "");
                        cmd.Parameters.AddWithValue("@UserAgent", Request.UserAgent ?? "");
                        cmd.Parameters.AddWithValue("@IPAddress", GetClientIP());
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        cmd.Parameters.AddWithValue("@Severity", "High");
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Fail silently for logging errors
            }
        }

        private void LogUserActivity(string action, string description)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ActivityLogs (UserId, Action, Module, Description, IPAddress, Timestamp)
                        VALUES (@UserId, @Action, @Module, @Description, @IPAddress, @Timestamp)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@Module", "HR Action Form");
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@IPAddress", GetClientIP());
                        cmd.Parameters.AddWithValue("@Timestamp", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Fail silently for activity logging
            }
        }

        private string GetClientIP()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip ?? "Unknown";
        }

        #endregion
    }
}