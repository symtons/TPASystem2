using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.OnBoarding
{
    public partial class NewHirePaperwork : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int currentEmployeeId;

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    InitializePage();
                    LoadApplicationData();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("An error occurred while loading the page: " + ex.Message);
            }
        }

        #endregion

        #region Initialization

        private void InitializePage()
        {
            // Check if user is logged in
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Get employee ID from the Users->Employees relationship
            currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId <= 0)
            {
                ShowErrorMessage("Employee record not found. Please contact HR to complete your employee profile setup.");
                return;
            }

            // Set default date
            txtApplicationDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            // Hide success message initially
            pnlSuccessMessage.Visible = false;
            pnlMessages.Visible = false;
        }

        private void LoadApplicationData()
        {
            try
            {
                // Check if there's an existing draft application
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                        SELECT TOP 1 * FROM [EmploymentApplications] 
                        WHERE [EmployeeId] = @EmployeeId 
                        AND [Status] IN ('Draft', 'In Progress')
                        ORDER BY [CreatedAt] DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", currentEmployeeId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                PopulateFormFromDatabase(reader);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading existing application data: " + ex.Message);
            }
        }

        private void PopulateFormFromDatabase(SqlDataReader reader)
        {
            // Personal Information
            txtFirstName.Text = GetStringValue(reader, "FirstName");
            txtMiddleName.Text = GetStringValue(reader, "MiddleName");
            txtLastName.Text = GetStringValue(reader, "LastName");
            txtHomeAddress.Text = GetStringValue(reader, "HomeAddress");
            txtAptNumber.Text = GetStringValue(reader, "AptNumber");
            txtCity.Text = GetStringValue(reader, "City");
            txtState.Text = GetStringValue(reader, "State");
            txtZipCode.Text = GetStringValue(reader, "ZipCode");
            txtSSN.Text = GetStringValue(reader, "SSN");
            txtDriversLicense.Text = GetStringValue(reader, "DriversLicense");
            txtDLState.Text = GetStringValue(reader, "DLState");
            txtPhoneNumber.Text = GetStringValue(reader, "PhoneNumber");
            txtCellNumber.Text = GetStringValue(reader, "CellNumber");
            txtEmergencyContactName.Text = GetStringValue(reader, "EmergencyContactName");
            txtEmergencyContactRelationship.Text = GetStringValue(reader, "EmergencyContactRelationship");
            txtEmergencyContactAddress.Text = GetStringValue(reader, "EmergencyContactAddress");

            // Position Information
            txtPosition1.Text = GetStringValue(reader, "Position1");
            txtPosition2.Text = GetStringValue(reader, "Position2");
            txtSalaryDesired.Text = GetStringValue(reader, "SalaryDesired");

            string salaryType = GetStringValue(reader, "SalaryType");
            rbHourly.Checked = salaryType == "Hourly";
            rbYearly.Checked = salaryType == "Yearly";

            if (reader["AvailableStartDate"] != DBNull.Value)
                txtAvailableStartDate.Text = Convert.ToDateTime(reader["AvailableStartDate"]).ToString("yyyy-MM-dd");

            // Employment Type
            string employmentSought = GetStringValue(reader, "EmploymentSought");
            chkFullTime.Checked = employmentSought.Contains("Full Time");
            chkPartTime.Checked = employmentSought.Contains("Part Time");
            chkTemporary.Checked = employmentSought.Contains("Temporary");

            // Location Preferences
            chkNashville.Checked = GetBoolValue(reader, "NashvilleLocation");
            chkFranklin.Checked = GetBoolValue(reader, "FranklinLocation");
            chkShelbyville.Checked = GetBoolValue(reader, "ShelbyvilleLocation");
            chkWaynesboro.Checked = GetBoolValue(reader, "WaynesboroLocation");
            txtOtherLocation.Text = GetStringValue(reader, "OtherLocation");

            // Shift Preferences
            chkFirstShift.Checked = GetBoolValue(reader, "FirstShift");
            chkSecondShift.Checked = GetBoolValue(reader, "SecondShift");
            chkThirdShift.Checked = GetBoolValue(reader, "ThirdShift");
            chkWeekendsOnly.Checked = GetBoolValue(reader, "WeekendsOnly");

            // Days Available
            chkMonday.Checked = GetBoolValue(reader, "MondayAvailable");
            chkTuesday.Checked = GetBoolValue(reader, "TuesdayAvailable");
            chkWednesday.Checked = GetBoolValue(reader, "WednesdayAvailable");
            chkThursday.Checked = GetBoolValue(reader, "ThursdayAvailable");
            chkFriday.Checked = GetBoolValue(reader, "FridayAvailable");
            chkSaturday.Checked = GetBoolValue(reader, "SaturdayAvailable");
            chkSunday.Checked = GetBoolValue(reader, "SundayAvailable");

            // TPA History
            rbPreviouslyAppliedYes.Checked = GetBoolValue(reader, "PreviouslyAppliedToTPA");
            rbPreviouslyAppliedNo.Checked = !GetBoolValue(reader, "PreviouslyAppliedToTPA");
            txtPreviousApplicationDate.Text = GetStringValue(reader, "PreviousApplicationDate");

            rbPreviouslyWorkedYes.Checked = GetBoolValue(reader, "PreviouslyWorkedForTPA");
            rbPreviouslyWorkedNo.Checked = !GetBoolValue(reader, "PreviouslyWorkedForTPA");
            txtPreviousWorkDate.Text = GetStringValue(reader, "PreviousWorkDate");

            rbFamilyMembersYes.Checked = GetBoolValue(reader, "FamilyMembersEmployedByTPA");
            rbFamilyMembersNo.Checked = !GetBoolValue(reader, "FamilyMembersEmployedByTPA");
            txtFamilyMemberDetails.Text = GetStringValue(reader, "FamilyMemberDetails");

            // Legal Status
            rbUSCitizenYes.Checked = GetBoolValue(reader, "USCitizen");
            rbUSCitizenNo.Checked = !GetBoolValue(reader, "USCitizen");
            txtAlienNumber.Text = GetStringValue(reader, "AlienNumber");

            rbLegallyEntitledYes.Checked = GetBoolValue(reader, "LegallyEntitledToWork");
            rbLegallyEntitledNo.Checked = !GetBoolValue(reader, "LegallyEntitledToWork");

            rb18OrOlderYes.Checked = GetBoolValue(reader, "Is18OrOlder");
            rb18OrOlderNo.Checked = !GetBoolValue(reader, "Is18OrOlder");

            // Military & Criminal History
            rbArmedForcesYes.Checked = GetBoolValue(reader, "ServedArmedForces");
            rbArmedForcesNo.Checked = !GetBoolValue(reader, "ServedArmedForces");

            rbConvictedYes.Checked = GetBoolValue(reader, "ConvictedOfCrime");
            rbConvictedNo.Checked = !GetBoolValue(reader, "ConvictedOfCrime");

            rbAbuseRegistryYes.Checked = GetBoolValue(reader, "OnAbuseRegistry");
            rbAbuseRegistryNo.Checked = !GetBoolValue(reader, "OnAbuseRegistry");

            rbFoundGuiltyYes.Checked = GetBoolValue(reader, "FoundGuiltyAbuse");
            rbFoundGuiltyNo.Checked = !GetBoolValue(reader, "FoundGuiltyAbuse");

            rbLicenseRevokedYes.Checked = GetBoolValue(reader, "LicenseRevoked");
            rbLicenseRevokedNo.Checked = !GetBoolValue(reader, "LicenseRevoked");

            // Background Check Information
            txtBGFullName.Text = GetStringValue(reader, "FirstName") + " " + GetStringValue(reader, "LastName");
            txtBGSSN.Text = GetStringValue(reader, "SSN");
            txtBGPhone.Text = GetStringValue(reader, "PhoneNumber");
            txtBGOtherNames.Text = GetStringValue(reader, "NameOther");
            if (reader["YearNameChange"] != DBNull.Value)
                txtBGNameChangeYear.Text = reader["YearNameChange"].ToString();
            txtBGDriversLicense.Text = GetStringValue(reader, "DriversLicenseNumber");
            txtBGDLState.Text = GetStringValue(reader, "DriversLicenseState");
            if (reader["DateOfBirth"] != DBNull.Value)
                txtBGDateOfBirth.Text = Convert.ToDateTime(reader["DateOfBirth"]).ToString("yyyy-MM-dd");
            txtBGNameOnLicense.Text = GetStringValue(reader, "NameOnLicense");

            rbBGConvicted7YearsYes.Checked = GetBoolValue(reader, "ConvictedCriminal7Years");
            rbBGConvicted7YearsNo.Checked = !GetBoolValue(reader, "ConvictedCriminal7Years");

            rbBGChargedInvestigationYes.Checked = GetBoolValue(reader, "ChargedInvestigation");
            rbBGChargedInvestigationNo.Checked = !GetBoolValue(reader, "ChargedInvestigation");

            txtSSNLast4.Text = GetStringValue(reader, "SSNLast4");

            // DIDD Authorization
            chkDIDDNoAbuse.Checked = GetBoolValue(reader, "DIDDNoAbuse");
            chkDIDDHadAbuse.Checked = GetBoolValue(reader, "DIDDHadAbuse");

            // Protection Statement
            chkProtectionNoAbuse.Checked = GetBoolValue(reader, "ProtectionNoAbuse");
            chkProtectionHadAbuse.Checked = GetBoolValue(reader, "ProtectionHadAbuse");

            // Final Acknowledgment
            chkFinalAcknowledgment.Checked = GetBoolValue(reader, "FinalAcknowledgment");
        }

        #endregion

        #region Button Events

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            try
            {
                int applicationId = SaveApplicationToDatabase(false);
                if (applicationId > 0)
                {
                    ShowSuccessMessage("Application saved as draft successfully.");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error saving application: " + ex.Message);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    int applicationId = SaveApplicationToDatabase(true);
                    if (applicationId > 0)
                    {
                        // Mark onboarding task as complete
                        MarkOnboardingTaskComplete();

                        pnlSuccessMessage.Visible = true;

                        // Redirect after 3 seconds
                        ScriptManager.RegisterStartupScript(this, GetType(), "redirect",
                            "setTimeout(function(){ window.location.href = '/OnBoarding/MyOnboarding.aspx'; }, 3000);", true);
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("Error submitting application: " + ex.Message);
                }
            }
        }

        #endregion

        #region Database Operations

        private int SaveApplicationToDatabase(bool isSubmission = false)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int applicationId = SaveMainApplication(conn, transaction, isSubmission);
                        SaveEducationData(applicationId, conn, transaction);
                        SaveLicensesData(applicationId, conn, transaction);
                        SaveEmploymentHistoryData(applicationId, conn, transaction);
                        SaveReferencesData(applicationId, conn, transaction);
                        SaveCriminalHistoryData(applicationId, conn, transaction);
                        SavePreviousAddressesData(applicationId, conn, transaction);

                        transaction.Commit();
                        return applicationId;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private int SaveMainApplication(SqlConnection conn, SqlTransaction transaction, bool isSubmission)
        {
            // Check if application already exists
            int existingId = GetExistingApplicationId(conn, transaction);

            if (existingId > 0)
            {
                return UpdateApplication(conn, transaction, existingId, isSubmission);
            }
            else
            {
                return InsertApplication(conn, transaction, isSubmission);
            }
        }

        private int GetExistingApplicationId(SqlConnection conn, SqlTransaction transaction)
        {
            string sql = @"
                SELECT TOP 1 [Id] FROM [EmploymentApplications] 
                WHERE [EmployeeId] = @EmployeeId 
                AND [Status] IN ('Draft', 'In Progress')
                ORDER BY [CreatedAt] DESC";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", currentEmployeeId);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        private int InsertApplication(SqlConnection conn, SqlTransaction transaction, bool isSubmission)
        {
            string sql = @"
                INSERT INTO [EmploymentApplications] (
                    [ApplicationNumber], [ApplicationDate], [Status], [EmployeeId],
                    [FirstName], [MiddleName], [LastName], [HomeAddress], [AptNumber],
                    [City], [State], [ZipCode], [SSN], [DriversLicense], [DLState],
                    [PhoneNumber], [CellNumber], [EmergencyContactName], [EmergencyContactRelationship],
                    [EmergencyContactAddress], [Position1], [Position2],
                    [SalaryDesired], [SalaryType], [EmploymentSought], [AvailableStartDate],
                    [NashvilleLocation], [FranklinLocation], [ShelbyvilleLocation], [WaynesboroLocation], [OtherLocation],
                    [FirstShift], [SecondShift], [ThirdShift], [WeekendsOnly],
                    [MondayAvailable], [TuesdayAvailable], [WednesdayAvailable], [ThursdayAvailable],
                    [FridayAvailable], [SaturdayAvailable], [SundayAvailable],
                    [PreviouslyAppliedToTPA], [PreviousApplicationDate], [PreviouslyWorkedForTPA], [PreviousWorkDate],
                    [FamilyMembersEmployedByTPA], [FamilyMemberDetails],
                    [USCitizen], [PermanentResident], [AlienNumber], [LegallyEntitledToWork], [Is18OrOlder],
                    [ServedArmedForces], [ConvictedOfCrime], [OnAbuseRegistry], [FoundGuiltyAbuse], [LicenseRevoked],
                    [SSNLast4], [NameOther], [YearNameChange], [DriversLicenseNumber], [DriversLicenseState],
                    [DateOfBirth], [NameOnLicense], [ConvictedCriminal7Years], [ChargedInvestigation],
                    [DIDDNoAbuse], [DIDDHadAbuse], [ProtectionNoAbuse], [ProtectionHadAbuse], [FinalAcknowledgment],
                    [CreatedAt], [UpdatedAt], [SubmittedAt]
                )
                VALUES (
                    @ApplicationNumber, @ApplicationDate, @Status, @EmployeeId,
                    @FirstName, @MiddleName, @LastName, @HomeAddress, @AptNumber,
                    @City, @State, @ZipCode, @SSN, @DriversLicense, @DLState,
                    @PhoneNumber, @CellNumber, @EmergencyContactName, @EmergencyContactRelationship,
                    @EmergencyContactAddress, @Position1, @Position2,
                    @SalaryDesired, @SalaryType, @EmploymentSought, @AvailableStartDate,
                    @NashvilleLocation, @FranklinLocation, @ShelbyvilleLocation, @WaynesboroLocation, @OtherLocation,
                    @FirstShift, @SecondShift, @ThirdShift, @WeekendsOnly,
                    @MondayAvailable, @TuesdayAvailable, @WednesdayAvailable, @ThursdayAvailable,
                    @FridayAvailable, @SaturdayAvailable, @SundayAvailable,
                    @PreviouslyAppliedToTPA, @PreviousApplicationDate, @PreviouslyWorkedForTPA, @PreviousWorkDate,
                    @FamilyMembersEmployedByTPA, @FamilyMemberDetails,
                    @USCitizen, @PermanentResident, @AlienNumber, @LegallyEntitledToWork, @Is18OrOlder,
                    @ServedArmedForces, @ConvictedOfCrime, @OnAbuseRegistry, @FoundGuiltyAbuse, @LicenseRevoked,
                    @SSNLast4, @NameOther, @YearNameChange, @DriversLicenseNumber, @DriversLicenseState,
                    @DateOfBirth, @NameOnLicense, @ConvictedCriminal7Years, @ChargedInvestigation,
                    @DIDDNoAbuse, @DIDDHadAbuse, @ProtectionNoAbuse, @ProtectionHadAbuse, @FinalAcknowledgment,
                    @CreatedAt, @UpdatedAt, @SubmittedAt
                );
                SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                AddApplicationParameters(cmd, isSubmission);
                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        private int UpdateApplication(SqlConnection conn, SqlTransaction transaction, int applicationId, bool isSubmission)
        {
            string sql = @"
                UPDATE [EmploymentApplications] SET
                    [Status] = @Status, [FirstName] = @FirstName, [MiddleName] = @MiddleName, [LastName] = @LastName,
                    [HomeAddress] = @HomeAddress, [AptNumber] = @AptNumber, [City] = @City, [State] = @State,
                    [ZipCode] = @ZipCode, [SSN] = @SSN, [DriversLicense] = @DriversLicense, [DLState] = @DLState,
                    [PhoneNumber] = @PhoneNumber, [CellNumber] = @CellNumber, [EmergencyContactName] = @EmergencyContactName,
                    [EmergencyContactRelationship] = @EmergencyContactRelationship, [EmergencyContactAddress] = @EmergencyContactAddress,
                    [Position1] = @Position1, [Position2] = @Position2, [SalaryDesired] = @SalaryDesired,
                    [SalaryType] = @SalaryType, [EmploymentSought] = @EmploymentSought, [AvailableStartDate] = @AvailableStartDate,
                    [NashvilleLocation] = @NashvilleLocation, [FranklinLocation] = @FranklinLocation,
                    [ShelbyvilleLocation] = @ShelbyvilleLocation, [WaynesboroLocation] = @WaynesboroLocation,
                    [OtherLocation] = @OtherLocation, [FirstShift] = @FirstShift, [SecondShift] = @SecondShift,
                    [ThirdShift] = @ThirdShift, [WeekendsOnly] = @WeekendsOnly, [MondayAvailable] = @MondayAvailable,
                    [TuesdayAvailable] = @TuesdayAvailable, [WednesdayAvailable] = @WednesdayAvailable,
                    [ThursdayAvailable] = @ThursdayAvailable, [FridayAvailable] = @FridayAvailable,
                    [SaturdayAvailable] = @SaturdayAvailable, [SundayAvailable] = @SundayAvailable,
                    [PreviouslyAppliedToTPA] = @PreviouslyAppliedToTPA, [PreviousApplicationDate] = @PreviousApplicationDate,
                    [PreviouslyWorkedForTPA] = @PreviouslyWorkedForTPA, [PreviousWorkDate] = @PreviousWorkDate,
                    [FamilyMembersEmployedByTPA] = @FamilyMembersEmployedByTPA, [FamilyMemberDetails] = @FamilyMemberDetails,
                    [USCitizen] = @USCitizen, [PermanentResident] = @PermanentResident, [AlienNumber] = @AlienNumber,
                    [LegallyEntitledToWork] = @LegallyEntitledToWork, [Is18OrOlder] = @Is18OrOlder,
                    [ServedArmedForces] = @ServedArmedForces, [ConvictedOfCrime] = @ConvictedOfCrime,
                    [OnAbuseRegistry] = @OnAbuseRegistry, [FoundGuiltyAbuse] = @FoundGuiltyAbuse,
                    [LicenseRevoked] = @LicenseRevoked, [SSNLast4] = @SSNLast4, [NameOther] = @NameOther,
                    [YearNameChange] = @YearNameChange, [DriversLicenseNumber] = @DriversLicenseNumber,
                    [DriversLicenseState] = @DriversLicenseState, [DateOfBirth] = @DateOfBirth,
                    [NameOnLicense] = @NameOnLicense, [ConvictedCriminal7Years] = @ConvictedCriminal7Years,
                    [ChargedInvestigation] = @ChargedInvestigation, [DIDDNoAbuse] = @DIDDNoAbuse,
                    [DIDDHadAbuse] = @DIDDHadAbuse, [ProtectionNoAbuse] = @ProtectionNoAbuse,
                    [ProtectionHadAbuse] = @ProtectionHadAbuse, [FinalAcknowledgment] = @FinalAcknowledgment,
                    [UpdatedAt] = @UpdatedAt, [SubmittedAt] = @SubmittedAt
                WHERE [Id] = @ApplicationId";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                AddApplicationParameters(cmd, isSubmission);
                cmd.ExecuteNonQuery();
                return applicationId;
            }
        }

        private void AddApplicationParameters(SqlCommand cmd, bool isSubmission)
        {
            DateTime applicationDate = DateTime.Parse(txtApplicationDate.Text);
            string tempAppNumber = "EMP-" + DateTime.Now.Ticks.ToString().Substring(10);

            // Basic parameters
            cmd.Parameters.AddWithValue("@ApplicationNumber", tempAppNumber);
            cmd.Parameters.AddWithValue("@ApplicationDate", applicationDate);
            cmd.Parameters.AddWithValue("@Status", isSubmission ? "Submitted" : "Draft");
            cmd.Parameters.AddWithValue("@EmployeeId", currentEmployeeId);

            // Personal Information
            cmd.Parameters.AddWithValue("@FirstName", GetTextOrNull(txtFirstName.Text));
            cmd.Parameters.AddWithValue("@MiddleName", GetTextOrNull(txtMiddleName.Text));
            cmd.Parameters.AddWithValue("@LastName", GetTextOrNull(txtLastName.Text));
            cmd.Parameters.AddWithValue("@HomeAddress", GetTextOrNull(txtHomeAddress.Text));
            cmd.Parameters.AddWithValue("@AptNumber", GetTextOrNull(txtAptNumber.Text));
            cmd.Parameters.AddWithValue("@City", GetTextOrNull(txtCity.Text));
            cmd.Parameters.AddWithValue("@State", GetTextOrNull(txtState.Text));
            cmd.Parameters.AddWithValue("@ZipCode", GetTextOrNull(txtZipCode.Text));
            cmd.Parameters.AddWithValue("@SSN", GetTextOrNull(txtSSN.Text));
            cmd.Parameters.AddWithValue("@DriversLicense", GetTextOrNull(txtDriversLicense.Text));
            cmd.Parameters.AddWithValue("@DLState", GetTextOrNull(txtDLState.Text));
            cmd.Parameters.AddWithValue("@PhoneNumber", GetTextOrNull(txtPhoneNumber.Text));
            cmd.Parameters.AddWithValue("@CellNumber", GetTextOrNull(txtCellNumber.Text));
            cmd.Parameters.AddWithValue("@EmergencyContactName", GetTextOrNull(txtEmergencyContactName.Text));
            cmd.Parameters.AddWithValue("@EmergencyContactRelationship", GetTextOrNull(txtEmergencyContactRelationship.Text));
            cmd.Parameters.AddWithValue("@EmergencyContactAddress", GetTextOrNull(txtEmergencyContactAddress.Text));

            // Position Information
            cmd.Parameters.AddWithValue("@Position1", GetTextOrNull(txtPosition1.Text));
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
            cmd.Parameters.AddWithValue("@FirstShift", chkFirstShift.Checked);
            cmd.Parameters.AddWithValue("@SecondShift", chkSecondShift.Checked);
            cmd.Parameters.AddWithValue("@ThirdShift", chkThirdShift.Checked);
            cmd.Parameters.AddWithValue("@WeekendsOnly", chkWeekendsOnly.Checked);

            // Days Available
            cmd.Parameters.AddWithValue("@MondayAvailable", chkMonday.Checked);
            cmd.Parameters.AddWithValue("@TuesdayAvailable", chkTuesday.Checked);
            cmd.Parameters.AddWithValue("@WednesdayAvailable", chkWednesday.Checked);
            cmd.Parameters.AddWithValue("@ThursdayAvailable", chkThursday.Checked);
            cmd.Parameters.AddWithValue("@FridayAvailable", chkFriday.Checked);
            cmd.Parameters.AddWithValue("@SaturdayAvailable", chkSaturday.Checked);
            cmd.Parameters.AddWithValue("@SundayAvailable", chkSunday.Checked);

            // TPA History
            cmd.Parameters.AddWithValue("@PreviouslyAppliedToTPA", rbPreviouslyAppliedYes.Checked);
            cmd.Parameters.AddWithValue("@PreviousApplicationDate", GetTextOrNull(txtPreviousApplicationDate.Text));
            cmd.Parameters.AddWithValue("@PreviouslyWorkedForTPA", rbPreviouslyWorkedYes.Checked);
            cmd.Parameters.AddWithValue("@PreviousWorkDate", GetTextOrNull(txtPreviousWorkDate.Text));
            cmd.Parameters.AddWithValue("@FamilyMembersEmployedByTPA", rbFamilyMembersYes.Checked);
            cmd.Parameters.AddWithValue("@FamilyMemberDetails", GetTextOrNull(txtFamilyMemberDetails.Text));

            // Legal Status
            cmd.Parameters.AddWithValue("@USCitizen", rbUSCitizenYes.Checked);
            cmd.Parameters.AddWithValue("@PermanentResident", rbUSCitizenNo.Checked);
            cmd.Parameters.AddWithValue("@AlienNumber", GetTextOrNull(txtAlienNumber.Text));
            cmd.Parameters.AddWithValue("@LegallyEntitledToWork", rbLegallyEntitledYes.Checked);
            cmd.Parameters.AddWithValue("@Is18OrOlder", rb18OrOlderYes.Checked);

            // Military & Criminal History
            cmd.Parameters.AddWithValue("@ServedArmedForces", rbArmedForcesYes.Checked);
            cmd.Parameters.AddWithValue("@ConvictedOfCrime", rbConvictedYes.Checked);
            cmd.Parameters.AddWithValue("@OnAbuseRegistry", rbAbuseRegistryYes.Checked);
            cmd.Parameters.AddWithValue("@FoundGuiltyAbuse", rbFoundGuiltyYes.Checked);
            cmd.Parameters.AddWithValue("@LicenseRevoked", rbLicenseRevokedYes.Checked);

            // Background Check Information
            cmd.Parameters.AddWithValue("@SSNLast4", GetTextOrNull(txtSSNLast4.Text));
            cmd.Parameters.AddWithValue("@NameOther", GetTextOrNull(txtBGOtherNames.Text));
            cmd.Parameters.AddWithValue("@YearNameChange", GetIntOrNull(txtBGNameChangeYear.Text));
            cmd.Parameters.AddWithValue("@DriversLicenseNumber", GetTextOrNull(txtBGDriversLicense.Text));
            cmd.Parameters.AddWithValue("@DriversLicenseState", GetTextOrNull(txtBGDLState.Text));
            cmd.Parameters.AddWithValue("@DateOfBirth", GetDateOrNull(txtBGDateOfBirth.Text));
            cmd.Parameters.AddWithValue("@NameOnLicense", GetTextOrNull(txtBGNameOnLicense.Text));
            cmd.Parameters.AddWithValue("@ConvictedCriminal7Years", rbBGConvicted7YearsYes.Checked);
            cmd.Parameters.AddWithValue("@ChargedInvestigation", rbBGChargedInvestigationYes.Checked);

            // DIDD Authorization
            cmd.Parameters.AddWithValue("@DIDDNoAbuse", chkDIDDNoAbuse.Checked);
            cmd.Parameters.AddWithValue("@DIDDHadAbuse", chkDIDDHadAbuse.Checked);

            // Protection Statement
            cmd.Parameters.AddWithValue("@ProtectionNoAbuse", chkProtectionNoAbuse.Checked);
            cmd.Parameters.AddWithValue("@ProtectionHadAbuse", chkProtectionHadAbuse.Checked);

            // Final Acknowledgment
            cmd.Parameters.AddWithValue("@FinalAcknowledgment", chkFinalAcknowledgment.Checked);

            // Timestamps
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@SubmittedAt", isSubmission ? (object)DateTime.Now : DBNull.Value);
        }

        private void SaveEducationData(int applicationId, SqlConnection conn, SqlTransaction transaction)
        {
            // Delete existing education records
            string deleteSql = "DELETE FROM [EmploymentApplicationEducation] WHERE [ApplicationId] = @ApplicationId";
            using (SqlCommand deleteCmd = new SqlCommand(deleteSql, conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                deleteCmd.ExecuteNonQuery();
            }

            // Insert education records
            var educationLevels = new[]
            {
                new { Level = "Elementary", School = txtElementarySchool.Text, Years = txtElementaryYears.Text,
                      Diploma = chkElementaryDiploma.Checked, Major = txtElementaryMajor.Text, Training = txtElementaryTraining.Value },
                new { Level = "High School", School = txtHighSchool.Text, Years = txtHighSchoolYears.Text,
                      Diploma = chkHighSchoolDiploma.Checked, Major = txtHighSchoolMajor.Text, Training = txtHighSchoolTraining.Value },
                new { Level = "Undergraduate", School = txtUndergraduateSchool.Text, Years = txtUndergraduateYears.Text,
                      Diploma = chkUndergraduateDiploma.Checked, Major = txtUndergraduateMajor.Text, Training = txtUndergraduateTraining.Value },
                new { Level = "Graduate", School = txtGraduateSchool.Text, Years = txtGraduateYears.Text,
                      Diploma = chkGraduateDiploma.Checked, Major = txtGraduateMajor.Text, Training = txtGraduateTraining.Value }
            };

            string insertSql = @"
                INSERT INTO [EmploymentApplicationEducation] 
                ([ApplicationId], [SchoolLevel], [SchoolNameLocation], [YearsCompleted], [GraduatedDiploma], [MajorMinor], [SpecializedTraining])
                VALUES (@ApplicationId, @SchoolLevel, @SchoolNameLocation, @YearsCompleted, @GraduatedDiploma, @MajorMinor, @SpecializedTraining)";

            foreach (var education in educationLevels)
            {
                if (!string.IsNullOrWhiteSpace(education.School) || !string.IsNullOrWhiteSpace(education.Years) ||
                    education.Diploma || !string.IsNullOrWhiteSpace(education.Major) || !string.IsNullOrWhiteSpace(education.Training))
                {
                    using (SqlCommand cmd = new SqlCommand(insertSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                        cmd.Parameters.AddWithValue("@SchoolLevel", education.Level);
                        cmd.Parameters.AddWithValue("@SchoolNameLocation", GetTextOrNull(education.School));
                        cmd.Parameters.AddWithValue("@YearsCompleted", GetIntOrNull(education.Years));
                        cmd.Parameters.AddWithValue("@GraduatedDiploma", education.Diploma);
                        cmd.Parameters.AddWithValue("@MajorMinor", GetTextOrNull(education.Major));
                        cmd.Parameters.AddWithValue("@SpecializedTraining", GetTextOrNull(education.Training));
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            // Save special skills
            if (!string.IsNullOrWhiteSpace(txtSpecialSkills.Text))
            {
                using (SqlCommand cmd = new SqlCommand(insertSql, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                    cmd.Parameters.AddWithValue("@SchoolLevel", "Special Skills");
                    cmd.Parameters.AddWithValue("@SchoolNameLocation", DBNull.Value);
                    cmd.Parameters.AddWithValue("@YearsCompleted", DBNull.Value);
                    cmd.Parameters.AddWithValue("@GraduatedDiploma", false);
                    cmd.Parameters.AddWithValue("@MajorMinor", DBNull.Value);
                    cmd.Parameters.AddWithValue("@SpecializedTraining", txtSpecialSkills.Text);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void SaveLicensesData(int applicationId, SqlConnection conn, SqlTransaction transaction)
        {
            // Delete existing license records
            string deleteSql = "DELETE FROM [EmploymentApplicationLicenses] WHERE [ApplicationId] = @ApplicationId";
            using (SqlCommand deleteCmd = new SqlCommand(deleteSql, conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                deleteCmd.ExecuteNonQuery();
            }

            // Insert license records
            var licenses = new[]
            {
                new { Type = txtLicenseType1.Text, State = txtLicenseState1.Text, Number = txtLicenseNumber1.Text, Expiration = txtLicenseExpiration1.Text },
                new { Type = txtLicenseType2.Text, State = txtLicenseState2.Text, Number = txtLicenseNumber2.Text, Expiration = txtLicenseExpiration2.Text },
                new { Type = txtLicenseType3.Text, State = txtLicenseState3.Text, Number = txtLicenseNumber3.Text, Expiration = txtLicenseExpiration3.Text }
            };

            string insertSql = @"
                INSERT INTO [EmploymentApplicationLicenses] 
                ([ApplicationId], [LicenseType], [State], [IDNumber], [ExpirationDate])
                VALUES (@ApplicationId, @LicenseType, @State, @IDNumber, @ExpirationDate)";

            foreach (var license in licenses)
            {
                if (!string.IsNullOrWhiteSpace(license.Type))
                {
                    using (SqlCommand cmd = new SqlCommand(insertSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                        cmd.Parameters.AddWithValue("@LicenseType", license.Type);
                        cmd.Parameters.AddWithValue("@State", GetTextOrNull(license.State));
                        cmd.Parameters.AddWithValue("@IDNumber", GetTextOrNull(license.Number));
                        cmd.Parameters.AddWithValue("@ExpirationDate", GetDateOrNull(license.Expiration));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void SaveEmploymentHistoryData(int applicationId, SqlConnection conn, SqlTransaction transaction)
        {
            // Delete existing employment history records
            string deleteSql = "DELETE FROM [EmploymentApplicationHistory] WHERE [ApplicationId] = @ApplicationId";
            using (SqlCommand deleteCmd = new SqlCommand(deleteSql, conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                deleteCmd.ExecuteNonQuery();
            }

            // Insert employment history records
            var employmentHistory = new[]
            {
                new {
                    Employer = txtEmployer1.Text, FromDate = txtEmploymentFrom1.Text, ToDate = txtEmploymentTo1.Text,
                    JobTitle = txtJobTitle1.Text, Supervisor = txtSupervisor1.Text, Address = txtEmployerAddress1.Text,
                    CityStateZip = txtEmployerCityStateZip1.Text, Phone = txtEmployerPhone1.Text,
                    StartingPay = txtStartingPay1.Text, FinalPay = txtFinalPay1.Text, WorkPerformed = txtWorkPerformed1.Text,
                    StillEmployed = rbStillEmployed1Yes.Checked, ReasonLeaving = txtReasonLeaving1.Text,
                    EligibleRehire = rbEligibleRehire1Yes.Checked
                },
                new {
                    Employer = txtEmployer2.Text, FromDate = txtEmploymentFrom2.Text, ToDate = txtEmploymentTo2.Text,
                    JobTitle = txtJobTitle2.Text, Supervisor = txtSupervisor2.Text, Address = txtEmployerAddress2.Text,
                    CityStateZip = txtEmployerCityStateZip2.Text, Phone = txtEmployerPhone2.Text,
                    StartingPay = txtStartingPay2.Text, FinalPay = txtFinalPay2.Text, WorkPerformed = txtWorkPerformed2.Text,
                    StillEmployed = false, ReasonLeaving = txtReasonLeaving2.Text,
                    EligibleRehire = rbEligibleRehire2Yes.Checked
                },
                new {
                    Employer = txtEmployer3.Text, FromDate = txtEmploymentFrom3.Text, ToDate = txtEmploymentTo3.Text,
                    JobTitle = txtJobTitle3.Text, Supervisor = txtSupervisor3.Text, Address = txtEmployerAddress3.Text,
                    CityStateZip = txtEmployerCityStateZip3.Text, Phone = txtEmployerPhone3.Text,
                    StartingPay = txtStartingPay3.Text, FinalPay = txtFinalPay3.Text, WorkPerformed = txtWorkPerformed3.Text,
                    StillEmployed = false, ReasonLeaving = txtReasonLeaving3.Text,
                    EligibleRehire = rbEligibleRehire3Yes.Checked
                }
            };

            string insertSql = @"
                INSERT INTO [EmploymentApplicationHistory] 
                ([ApplicationId], [Employer], [DatesEmployedFrom], [DatesEmployedTo], [JobTitle], [Supervisor],
                 [Address], [CityStateZip], [TelephoneNumbers], [HourlyRateStarting], [HourlyRateFinal],
                 [TitleWorkPerformed], [StillEmployed], [ReasonForLeaving], [EligibleForRehire])
                VALUES (@ApplicationId, @Employer, @DatesEmployedFrom, @DatesEmployedTo, @JobTitle, @Supervisor,
                        @Address, @CityStateZip, @TelephoneNumbers, @HourlyRateStarting, @HourlyRateFinal,
                        @TitleWorkPerformed, @StillEmployed, @ReasonForLeaving, @EligibleForRehire)";

            foreach (var employment in employmentHistory)
            {
                if (!string.IsNullOrWhiteSpace(employment.Employer))
                {
                    using (SqlCommand cmd = new SqlCommand(insertSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                        cmd.Parameters.AddWithValue("@Employer", employment.Employer);
                        cmd.Parameters.AddWithValue("@DatesEmployedFrom", GetDateOrNull(employment.FromDate));
                        cmd.Parameters.AddWithValue("@DatesEmployedTo", GetDateOrNull(employment.ToDate));
                        cmd.Parameters.AddWithValue("@JobTitle", GetTextOrNull(employment.JobTitle));
                        cmd.Parameters.AddWithValue("@Supervisor", GetTextOrNull(employment.Supervisor));
                        cmd.Parameters.AddWithValue("@Address", GetTextOrNull(employment.Address));
                        cmd.Parameters.AddWithValue("@CityStateZip", GetTextOrNull(employment.CityStateZip));
                        cmd.Parameters.AddWithValue("@TelephoneNumbers", GetTextOrNull(employment.Phone));
                        cmd.Parameters.AddWithValue("@HourlyRateStarting", GetDecimalOrNull(employment.StartingPay));
                        cmd.Parameters.AddWithValue("@HourlyRateFinal", GetDecimalOrNull(employment.FinalPay));
                        cmd.Parameters.AddWithValue("@TitleWorkPerformed", GetTextOrNull(employment.WorkPerformed));
                        cmd.Parameters.AddWithValue("@StillEmployed", employment.StillEmployed);
                        cmd.Parameters.AddWithValue("@ReasonForLeaving", GetTextOrNull(employment.ReasonLeaving));
                        cmd.Parameters.AddWithValue("@EligibleForRehire", employment.EligibleRehire);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void SaveReferencesData(int applicationId, SqlConnection conn, SqlTransaction transaction)
        {
            // Delete existing reference records
            string deleteSql = "DELETE FROM [EmploymentApplicationReferences] WHERE [ApplicationId] = @ApplicationId";
            using (SqlCommand deleteCmd = new SqlCommand(deleteSql, conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                deleteCmd.ExecuteNonQuery();
            }

            // Insert reference records
            var references = new[]
            {
                new { Name = txtReference1Name.Text, Phone = txtReference1Phone.Text, Email = txtReference1Email.Text, Years = txtReference1Years.Text, Type = "Professional Reference #1" },
                new { Name = txtReference2Name.Text, Phone = txtReference2Phone.Text, Email = txtReference2Email.Text, Years = txtReference2Years.Text, Type = "Professional Reference #2" },
                new { Name = txtReference3Name.Text, Phone = txtReference3Phone.Text, Email = txtReference3Email.Text, Years = txtReference3Years.Text, Type = "Professional Reference #3" }
            };

            string insertSql = @"
                INSERT INTO [EmploymentApplicationReferences] 
                ([ApplicationId], [FirstLastName], [PhoneNumber], [EmailAddress], [YearsKnown], [ReferenceType])
                VALUES (@ApplicationId, @FirstLastName, @PhoneNumber, @EmailAddress, @YearsKnown, @ReferenceType)";

            foreach (var reference in references)
            {
                if (!string.IsNullOrWhiteSpace(reference.Name))
                {
                    using (SqlCommand cmd = new SqlCommand(insertSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                        cmd.Parameters.AddWithValue("@FirstLastName", reference.Name);
                        cmd.Parameters.AddWithValue("@PhoneNumber", GetTextOrNull(reference.Phone));
                        cmd.Parameters.AddWithValue("@EmailAddress", GetTextOrNull(reference.Email));
                        cmd.Parameters.AddWithValue("@YearsKnown", GetIntOrNull(reference.Years));
                        cmd.Parameters.AddWithValue("@ReferenceType", reference.Type);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void SaveCriminalHistoryData(int applicationId, SqlConnection conn, SqlTransaction transaction)
        {
            // Delete existing criminal history records
            string deleteSql = "DELETE FROM [EmploymentApplicationCriminalHistory] WHERE [ApplicationId] = @ApplicationId";
            using (SqlCommand deleteCmd = new SqlCommand(deleteSql, conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                deleteCmd.ExecuteNonQuery();
            }

            // Insert criminal history records
            var criminalHistory = new[]
            {
                new { Date = txtCriminalDate1.Text, Charge = txtCriminalCharge1.Text, Outcome = txtCriminalOutcome1.Text },
                new { Date = txtCriminalDate2.Text, Charge = txtCriminalCharge2.Text, Outcome = txtCriminalOutcome2.Text },
                new { Date = txtCriminalDate3.Text, Charge = txtCriminalCharge3.Text, Outcome = txtCriminalOutcome3.Text }
            };

            string insertSql = @"
                INSERT INTO [EmploymentApplicationCriminalHistory] 
                ([ApplicationId], [Date], [Charge], [StatusOrOutcome])
                VALUES (@ApplicationId, @Date, @Charge, @StatusOrOutcome)";

            foreach (var criminal in criminalHistory)
            {
                if (!string.IsNullOrWhiteSpace(criminal.Date) || !string.IsNullOrWhiteSpace(criminal.Charge) || !string.IsNullOrWhiteSpace(criminal.Outcome))
                {
                    using (SqlCommand cmd = new SqlCommand(insertSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                        cmd.Parameters.AddWithValue("@Date", GetTextOrNull(criminal.Date));
                        cmd.Parameters.AddWithValue("@Charge", GetTextOrNull(criminal.Charge));
                        cmd.Parameters.AddWithValue("@StatusOrOutcome", GetTextOrNull(criminal.Outcome));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void SavePreviousAddressesData(int applicationId, SqlConnection conn, SqlTransaction transaction)
        {
            // Delete existing address records
            string deleteSql = "DELETE FROM [EmploymentApplicationPreviousAddresses] WHERE [ApplicationId] = @ApplicationId";
            using (SqlCommand deleteCmd = new SqlCommand(deleteSql, conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                deleteCmd.ExecuteNonQuery();
            }

            // Insert previous address records
            var addresses = new[]
            {
                new { Street = txtPrevAddress1Street.Text, City = txtPrevAddress1City.Text, State = txtPrevAddress1State.Text, Years = txtPrevAddress1Years.Text },
                new { Street = txtPrevAddress2Street.Text, City = txtPrevAddress2City.Text, State = txtPrevAddress2State.Text, Years = txtPrevAddress2Years.Text },
                new { Street = txtPrevAddress3Street.Text, City = txtPrevAddress3City.Text, State = txtPrevAddress3State.Text, Years = txtPrevAddress3Years.Text }
            };

            string insertSql = @"
                INSERT INTO [EmploymentApplicationPreviousAddresses] 
                ([ApplicationId], [Street], [City], [State], [YearsResided])
                VALUES (@ApplicationId, @Street, @City, @State, @YearsResided)";

            foreach (var address in addresses)
            {
                if (!string.IsNullOrWhiteSpace(address.Street))
                {
                    using (SqlCommand cmd = new SqlCommand(insertSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                        cmd.Parameters.AddWithValue("@Street", address.Street);
                        cmd.Parameters.AddWithValue("@City", GetTextOrNull(address.City));
                        cmd.Parameters.AddWithValue("@State", GetTextOrNull(address.State));
                        cmd.Parameters.AddWithValue("@YearsResided", GetIntOrNull(address.Years));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void MarkOnboardingTaskComplete()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                        UPDATE [OnboardingTasks] 
                        SET [Status] = 'COMPLETED', [CompletedDate] = @CompletedDate, [UpdatedDate] = @UpdatedDate
                        WHERE [EmployeeId] = @EmployeeId 
                        AND ([Title] LIKE '%Employment Application%' OR [Title] LIKE '%New Hire Paperwork%')
                        AND [Status] IN ('ASSIGNED', 'PENDING')";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", currentEmployeeId);
                        cmd.Parameters.AddWithValue("@CompletedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        System.Diagnostics.Debug.WriteLine($"Marked {rowsAffected} onboarding tasks as completed for employee {currentEmployeeId}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the main operation
                System.Diagnostics.Debug.WriteLine("Error marking onboarding task complete: " + ex.Message);
            }
        }

        #endregion

        #region Helper Methods

        private int GetCurrentEmployeeId()
        {
            try
            {
                if (Session["UserId"] == null)
                    return 0;

                int userId = Convert.ToInt32(Session["UserId"]);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT Id FROM [Employees] WHERE [UserId] = @UserId AND [IsActive] = 1";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        var result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error getting employee ID: " + ex.Message);
                return 0;
            }
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
            return string.IsNullOrWhiteSpace(text) ? DBNull.Value : (object)text.Trim();
        }

        private object GetDateOrNull(string dateText)
        {
            if (string.IsNullOrWhiteSpace(dateText))
                return DBNull.Value;

            if (DateTime.TryParse(dateText, out DateTime date))
                return date;

            return DBNull.Value;
        }

        private object GetDecimalOrNull(string decimalText)
        {
            if (string.IsNullOrWhiteSpace(decimalText))
                return DBNull.Value;

            if (decimal.TryParse(decimalText, out decimal value))
                return value;

            return DBNull.Value;
        }

        private object GetIntOrNull(string intText)
        {
            if (string.IsNullOrWhiteSpace(intText))
                return DBNull.Value;

            if (int.TryParse(intText, out int value))
                return value;

            return DBNull.Value;
        }

        private string GetStringValue(SqlDataReader reader, string columnName)
        {
            return reader[columnName] == DBNull.Value ? string.Empty : reader[columnName].ToString();
        }

        private bool GetBoolValue(SqlDataReader reader, string columnName)
        {
            return reader[columnName] != DBNull.Value && Convert.ToBoolean(reader[columnName]);
        }

        private void ShowSuccessMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "message-text success";
            pnlMessages.CssClass = "message-panel success-message";
            pnlMessages.Visible = true;
        }

        private void ShowErrorMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "message-text error";
            pnlMessages.CssClass = "message-panel error-message";
            pnlMessages.Visible = true;
        }

        #endregion
    }
}