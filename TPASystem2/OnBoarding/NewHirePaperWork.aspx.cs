using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.OnBoarding
{
    public partial class NewHirePaperwork : System.Web.UI.Page
    {
        #region Properties and Fields

        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int CurrentEmployeeId => GetCurrentEmployeeId();

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            // Disable validation for employment application form
            Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                InitializePage();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && ValidateForm())
            {
                try
                {
                    int applicationId = SaveApplicationToDatabase();
                    if (applicationId > 0)
                    {
                        // Save additional data
                        SaveEducationData(applicationId);
                        SaveLicensesData(applicationId);
                        SaveEmploymentHistoryData(applicationId);
                        SaveReferencesData(applicationId);
                        SaveCriminalHistoryData(applicationId);

                        ShowSuccessMessage("Your employment application has been completed successfully! Application #: " + GenerateApplicationNumber(applicationId));
                        ClearForm();

                        // Redirect back to onboarding dashboard after 3 seconds
                        ClientScript.RegisterStartupScript(this.GetType(), "redirect",
                            "setTimeout(function(){ window.location.href = 'MyOnboarding.aspx'; }, 3000);", true);
                    }
                    else
                    {
                        ShowErrorMessage("An error occurred while submitting your application. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("An error occurred: " + ex.Message);
                }
            }
        }

        protected void btnSaveProgress_Click(object sender, EventArgs e)
        {
            // Implementation for saving progress without full validation
            try
            {
                int applicationId = SaveApplicationToDatabase(false);
                if (applicationId > 0)
                {
                    ShowSuccessMessage("Your progress has been saved. You can continue later.");
                }
                else
                {
                    ShowErrorMessage("An error occurred while saving your progress. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("An error occurred: " + ex.Message);
            }
        }

        #endregion

        #region Validation Methods

        private bool ValidateForm()
        {
            bool isValid = true;
            string errorMessage = "";

            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtFirstName?.Text))
            {
                isValid = false;
                errorMessage += "First name is required. ";
            }

            if (string.IsNullOrWhiteSpace(txtLastName?.Text))
            {
                isValid = false;
                errorMessage += "Last name is required. ";
            }

            if (string.IsNullOrWhiteSpace(txtHomeAddress?.Text))
            {
                isValid = false;
                errorMessage += "Home address is required. ";
            }

            if (string.IsNullOrWhiteSpace(txtCity?.Text))
            {
                isValid = false;
                errorMessage += "City is required. ";
            }

            if (string.IsNullOrWhiteSpace(txtState?.Text))
            {
                isValid = false;
                errorMessage += "State is required. ";
            }

            if (string.IsNullOrWhiteSpace(txtZipCode?.Text))
            {
                isValid = false;
                errorMessage += "Zip code is required. ";
            }

            if (string.IsNullOrWhiteSpace(txtSSN?.Text))
            {
                isValid = false;
                errorMessage += "Social Security Number is required. ";
            }

            if (string.IsNullOrWhiteSpace(txtPhoneNumber?.Text))
            {
                isValid = false;
                errorMessage += "Phone number is required. ";
            }

            if (string.IsNullOrWhiteSpace(txtPosition1?.Text))
            {
                isValid = false;
                errorMessage += "At least one position must be specified. ";
            }

            if (!chkAcknowledgment.Checked)
            {
                isValid = false;
                errorMessage += "You must acknowledge the certification. ";
            }

            if (!isValid)
            {
                ShowErrorMessage(errorMessage);
            }

            return isValid;
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
                ShowErrorMessage("Employee record not found. Please contact HR.");
                return;
            }

            // Set application date to today
            txtApplicationDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            // Hide messages panel initially
            pnlMessages.Visible = false;
            pnlSuccessMessage.Visible = false;
        }

        private int GetCurrentEmployeeId()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT Id FROM Employees WHERE UserId = @UserId AND IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        var result = cmd.ExecuteScalar();
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

        #region Database Operations

        private int SaveApplicationToDatabase(bool isSubmission = true)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string sql = @"
                            INSERT INTO [EmploymentApplications] (
                                [ApplicationNumber], [ApplicationDate], [Status], [EmployeeId],
                                [FirstName], [MiddleName], [LastName], [HomeAddress], [AptNumber],
                                [City], [State], [ZipCode], [SSN], [DriversLicense], [DLState],
                                [PhoneNumber], [CellNumber], [EmergencyContactName], [EmergencyContactRelationship],
                                [EmergencyContactAddress], [EmergencyContactPhone], [Position1], [Position2],
                                [SalaryDesired], [SalaryType], [EmploymentSought], [AvailableStartDate],
                                [NashvilleLocation], [FranklinLocation], [ShelbyvilleLocation], [WaynesboroLocation], [OtherLocation],
                                [FirstShift], [SecondShift], [ThirdShift], [WeekendsOnly],
                                [MondayAvailable], [TuesdayAvailable], [WednesdayAvailable], [ThursdayAvailable],
                                [FridayAvailable], [SaturdayAvailable], [SundayAvailable],
                                [PreviouslyAppliedToTPA], [PreviousApplicationDate], [PreviouslyWorkedForTPA], [PreviousWorkDate],
                                [FamilyMembersEmployedByTPA], [FamilyMemberDetails],
                                [USCitizen], [PermanentResident], [AlienNumber], [LegallyEntitledToWork], [Is18OrOlder],
                                [CreatedAt], [UpdatedAt]
                            )
                            VALUES (
                                @ApplicationNumber, @ApplicationDate, @Status, @EmployeeId,
                                @FirstName, @MiddleName, @LastName, @HomeAddress, @AptNumber,
                                @City, @State, @ZipCode, @SSN, @DriversLicense, @DLState,
                                @PhoneNumber, @CellNumber, @EmergencyContactName, @EmergencyContactRelationship,
                                @EmergencyContactAddress, @EmergencyContactPhone, @Position1, @Position2,
                                @SalaryDesired, @SalaryType, @EmploymentSought, @AvailableStartDate,
                                @NashvilleLocation, @FranklinLocation, @ShelbyvilleLocation, @WaynesboroLocation, @OtherLocation,
                                @FirstShift, @SecondShift, @ThirdShift, @WeekendsOnly,
                                @MondayAvailable, @TuesdayAvailable, @WednesdayAvailable, @ThursdayAvailable,
                                @FridayAvailable, @SaturdayAvailable, @SundayAvailable,
                                @PreviouslyAppliedToTPA, @PreviousApplicationDate, @PreviouslyWorkedForTPA, @PreviousWorkDate,
                                @FamilyMembersEmployedByTPA, @FamilyMemberDetails,
                                @USCitizen, @PermanentResident, @AlienNumber, @LegallyEntitledToWork, @Is18OrOlder,
                                @CreatedAt, @UpdatedAt
                            );
                            SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
                        {
                            DateTime applicationDate = DateTime.Parse(txtApplicationDate.Text);
                            string tempAppNumber = "EMP-" + DateTime.Now.Ticks.ToString().Substring(10);

                            // Add parameters
                            cmd.Parameters.AddWithValue("@ApplicationNumber", tempAppNumber);
                            cmd.Parameters.AddWithValue("@ApplicationDate", applicationDate);
                            cmd.Parameters.AddWithValue("@Status", isSubmission ? "SUBMITTED" : "DRAFT");
                            cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                            // Personal Information
                            cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@MiddleName", GetTextOrNull(txtMiddleName.Text));
                            cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                            cmd.Parameters.AddWithValue("@HomeAddress", txtHomeAddress.Text.Trim());
                            cmd.Parameters.AddWithValue("@AptNumber", GetTextOrNull(txtAptNumber.Text));
                            cmd.Parameters.AddWithValue("@City", txtCity.Text.Trim());
                            cmd.Parameters.AddWithValue("@State", txtState.Text.Trim());
                            cmd.Parameters.AddWithValue("@ZipCode", txtZipCode.Text.Trim());
                            cmd.Parameters.AddWithValue("@SSN", txtSSN.Text.Trim());
                            cmd.Parameters.AddWithValue("@DriversLicense", GetTextOrNull(txtDriversLicense.Text));
                            cmd.Parameters.AddWithValue("@DLState", GetTextOrNull(txtDLState.Text));
                            cmd.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text.Trim());
                            cmd.Parameters.AddWithValue("@CellNumber", GetTextOrNull(txtCellNumber.Text));

                            // Emergency Contact
                            cmd.Parameters.AddWithValue("@EmergencyContactName", txtEmergencyContactName.Text.Trim());
                            cmd.Parameters.AddWithValue("@EmergencyContactRelationship", txtEmergencyContactRelationship.Text.Trim());
                            cmd.Parameters.AddWithValue("@EmergencyContactAddress", GetTextOrNull(txtEmergencyContactAddress.Text));
                            cmd.Parameters.AddWithValue("@EmergencyContactPhone", txtEmergencyContactPhone.Text.Trim());

                            // Position Information
                            cmd.Parameters.AddWithValue("@Position1", txtPosition1.Text.Trim());
                            cmd.Parameters.AddWithValue("@Position2", GetTextOrNull(txtPosition2.Text));
                            cmd.Parameters.AddWithValue("@SalaryDesired", GetDecimalOrNull(txtSalaryDesired.Text));
                            cmd.Parameters.AddWithValue("@SalaryType", GetSalaryType());
                            cmd.Parameters.AddWithValue("@EmploymentSought", GetEmploymentType());
                            cmd.Parameters.AddWithValue("@AvailableStartDate", GetDateOrNull(txtAvailableStartDate.Text));

                            // Location Preferences
                            cmd.Parameters.AddWithValue("@NashvilleLocation", chkNashville.Checked);
                            cmd.Parameters.AddWithValue("@FranklinLocation", chkFranklin.Checked);
                            cmd.Parameters.AddWithValue("@ShelbyvilleLocation", chkShelbyville.Checked);
                            cmd.Parameters.AddWithValue("@WaynesboroLocation", chkWaynesboro.Checked);
                            cmd.Parameters.AddWithValue("@OtherLocation", GetTextOrNull(txtOtherLocation.Text));

                            // Shift Preferences
                            cmd.Parameters.AddWithValue("@FirstShift", chk1stShift.Checked);
                            cmd.Parameters.AddWithValue("@SecondShift", chk2ndShift.Checked);
                            cmd.Parameters.AddWithValue("@ThirdShift", chk3rdShift.Checked);
                            cmd.Parameters.AddWithValue("@WeekendsOnly", chkWeekendsOnly.Checked);

                            // Days Available
                            cmd.Parameters.AddWithValue("@MondayAvailable", chkMonday.Checked);
                            cmd.Parameters.AddWithValue("@TuesdayAvailable", chkTuesday.Checked);
                            cmd.Parameters.AddWithValue("@WednesdayAvailable", chkWednesday.Checked);
                            cmd.Parameters.AddWithValue("@ThursdayAvailable", chkThursday.Checked);
                            cmd.Parameters.AddWithValue("@FridayAvailable", chkFriday.Checked);
                            cmd.Parameters.AddWithValue("@SaturdayAvailable", chkSaturday.Checked);
                            cmd.Parameters.AddWithValue("@SundayAvailable", chkSunday.Checked);

                            // Previous TPA Employment
                            cmd.Parameters.AddWithValue("@PreviouslyAppliedToTPA", rbAppliedYes.Checked);
                            cmd.Parameters.AddWithValue("@PreviousApplicationDate", GetTextOrNull(txtPreviousApplicationDate.Text));
                            cmd.Parameters.AddWithValue("@PreviouslyWorkedForTPA", rbWorkedYes.Checked);
                            cmd.Parameters.AddWithValue("@PreviousWorkDate", GetTextOrNull(txtPreviousWorkDate.Text));

                            // Family Employment
                            cmd.Parameters.AddWithValue("@FamilyMembersEmployedByTPA", rbFamilyYes.Checked);
                            cmd.Parameters.AddWithValue("@FamilyMemberDetails", GetTextOrNull(txtFamilyMemberDetails.Text));

                            // Legal Status
                            cmd.Parameters.AddWithValue("@USCitizen", rbCitizenYes.Checked);
                            cmd.Parameters.AddWithValue("@PermanentResident", rbCitizenNo.Checked);
                            cmd.Parameters.AddWithValue("@AlienNumber", GetTextOrNull(txtAlienNumber.Text));
                            cmd.Parameters.AddWithValue("@LegallyEntitledToWork", rbLegallyEntitledYes.Checked);
                            cmd.Parameters.AddWithValue("@Is18OrOlder", rb18OrOlderYes.Checked);

                            // Metadata
                            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

                            var result = cmd.ExecuteScalar();
                            int applicationId = Convert.ToInt32(result);

                            // Update with proper application number
                            if (applicationId > 0)
                            {
                                string properAppNumber = GenerateApplicationNumber(applicationId);
                                string updateSql = "UPDATE [EmploymentApplications] SET [ApplicationNumber] = @ProperAppNumber WHERE [Id] = @ApplicationId";
                                using (SqlCommand updateCmd = new SqlCommand(updateSql, conn, transaction))
                                {
                                    updateCmd.Parameters.AddWithValue("@ProperAppNumber", properAppNumber);
                                    updateCmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                                    updateCmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            return applicationId;
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Placeholder methods for additional data saving (these controls don't exist in the simplified form)
        private void SaveEducationData(int applicationId)
        {
            // Implementation for education data if form had education fields
            // Currently this form only has basic employment application info
        }

        private void SaveLicensesData(int applicationId)
        {
            // Implementation for licenses data if form had license fields
        }

        private void SaveEmploymentHistoryData(int applicationId)
        {
            // Implementation for employment history if form had employment history fields
        }

        private void SaveReferencesData(int applicationId)
        {
            // Implementation for references if form had reference fields
        }

        private void SaveCriminalHistoryData(int applicationId)
        {
            // Implementation for criminal history if form had criminal history fields
        }

        #endregion

        #region Helper Methods

        private string GenerateApplicationNumber(int applicationId)
        {
            return "EMP-" + DateTime.Now.Year + "-" + applicationId.ToString("0000");
        }

        private string GetSalaryType()
        {
            if (rbHourly.Checked) return "Hourly";
            if (rbYearly.Checked) return "Yearly";
            return null;
        }

        private string GetEmploymentType()
        {
            var types = new List<string>();
            if (chkFullTime.Checked) types.Add("Full Time");
            if (chkPartTime.Checked) types.Add("Part Time");
            if (chkTemporary.Checked) types.Add("Temporary");
            return types.Count > 0 ? string.Join(", ", types) : null;
        }

        private object GetTextOrNull(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? (object)DBNull.Value : text.Trim();
        }

        private object GetDateOrNull(string dateText)
        {
            if (string.IsNullOrWhiteSpace(dateText)) return DBNull.Value;

            if (DateTime.TryParse(dateText, out DateTime date))
                return date;

            return DBNull.Value;
        }

        private object GetDecimalOrNull(string decimalText)
        {
            if (string.IsNullOrWhiteSpace(decimalText)) return DBNull.Value;

            if (decimal.TryParse(decimalText, out decimal value))
                return value;

            return DBNull.Value;
        }

        private object GetIntOrNull(string intText)
        {
            if (string.IsNullOrWhiteSpace(intText)) return DBNull.Value;

            if (int.TryParse(intText, out int value))
                return value;

            return DBNull.Value;
        }

        private void ShowSuccessMessage(string message)
        {
            pnlSuccessMessage.Visible = true;
            pnlMessages.Visible = false;
            // Update the success message text
            foreach (Control control in pnlSuccessMessage.Controls)
            {
                if (control is System.Web.UI.LiteralControl)
                {
                    // Replace the existing message
                    pnlSuccessMessage.Controls.Clear();
                    pnlSuccessMessage.Controls.Add(new System.Web.UI.LiteralControl(
                        "<i class=\"material-icons\" style=\"vertical-align: middle; margin-right: 0.5rem;\">check_circle</i>" +
                        "<strong>" + message + "</strong>"));
                    break;
                }
            }
        }

        private void ShowErrorMessage(string message)
        {
            pnlMessages.Visible = true;
            pnlMessages.CssClass = "message-panel error";
            lblMessage.Text = message;
            lblMessage.CssClass = "message-text error";
            pnlSuccessMessage.Visible = false;
        }

        private void ClearForm()
        {
            // Clear all form fields
            foreach (Control control in this.Controls)
            {
                ClearControls(control);
            }

            // Reset application date
            txtApplicationDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void ClearControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox textBox && textBox.ID != "txtApplicationDate")
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
                else if (control is DropDownList dropDown)
                {
                    dropDown.SelectedIndex = -1;
                }
                else if (control.HasControls())
                {
                    ClearControls(control);
                }
            }
        }

        #endregion
    }
}