using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text;

namespace TPASystem2.OnBoarding
{
    public partial class NewHirePaperWork : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int currentEmployeeId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
                LoadExistingApplication();
                SetActiveTab("Personal");
            }
        }

        private void InitializePage()
        {
            // Get current employee ID from session
            if (Session["UserId"] != null)
            {
                currentEmployeeId = Convert.ToInt32(Session["UserId"]);
            }
            else
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // Set initial dates
            txtApplicationDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtSignatureDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        #region Tab Navigation

        protected void btnTabPersonal_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            SetActiveTab("Personal");
        }

        protected void btnTabPosition_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            SetActiveTab("Position");
        }

        protected void btnTabBackground_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            SetActiveTab("Background");
        }

        protected void btnTabEducation_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            SetActiveTab("Education");
        }

        protected void btnTabEmployment_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            SetActiveTab("Employment");
        }

        protected void btnTabReferences_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            SetActiveTab("References");
        }

        protected void btnTabAuthorization_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();
            SetActiveTab("Authorization");
        }

        private void SetActiveTab(string tabName)
        {
            // Reset all tab buttons
            btnTabPersonal.CssClass = "tab-button";
            btnTabPosition.CssClass = "tab-button";
            btnTabBackground.CssClass = "tab-button";
            btnTabEducation.CssClass = "tab-button";
            btnTabEmployment.CssClass = "tab-button";
            btnTabReferences.CssClass = "tab-button";
            btnTabAuthorization.CssClass = "tab-button";

            // Hide all tab content
            pnlPersonalTab.CssClass = "tab-content";
            pnlPositionTab.CssClass = "tab-content";
            pnlBackgroundTab.CssClass = "tab-content";
            pnlEducationTab.CssClass = "tab-content";
            pnlEmploymentTab.CssClass = "tab-content";
            pnlReferencesTab.CssClass = "tab-content";
            pnlAuthorizationTab.CssClass = "tab-content";

            // Show selected tab and update navigation
            switch (tabName)
            {
                case "Personal":
                    btnTabPersonal.CssClass = "tab-button active";
                    pnlPersonalTab.CssClass = "tab-content active";
                    btnPrevious.Visible = false;
                    btnNext.Visible = true;
                    btnSubmitApplication.Visible = false;
                    break;
                case "Position":
                    btnTabPosition.CssClass = "tab-button active";
                    pnlPositionTab.CssClass = "tab-content active";
                    btnPrevious.Visible = true;
                    btnNext.Visible = true;
                    btnSubmitApplication.Visible = false;
                    break;
                case "Background":
                    btnTabBackground.CssClass = "tab-button active";
                    pnlBackgroundTab.CssClass = "tab-content active";
                    btnPrevious.Visible = true;
                    btnNext.Visible = true;
                    btnSubmitApplication.Visible = false;
                    break;
                case "Education":
                    btnTabEducation.CssClass = "tab-button active";
                    pnlEducationTab.CssClass = "tab-content active";
                    btnPrevious.Visible = true;
                    btnNext.Visible = true;
                    btnSubmitApplication.Visible = false;
                    break;
                case "Employment":
                    btnTabEmployment.CssClass = "tab-button active";
                    pnlEmploymentTab.CssClass = "tab-content active";
                    btnPrevious.Visible = true;
                    btnNext.Visible = true;
                    btnSubmitApplication.Visible = false;
                    break;
                case "References":
                    btnTabReferences.CssClass = "tab-button active";
                    pnlReferencesTab.CssClass = "tab-content active";
                    btnPrevious.Visible = true;
                    btnNext.Visible = true;
                    btnSubmitApplication.Visible = false;
                    break;
                case "Authorization":
                    btnTabAuthorization.CssClass = "tab-button active";
                    pnlAuthorizationTab.CssClass = "tab-content active";
                    btnPrevious.Visible = true;
                    btnNext.Visible = false;
                    btnSubmitApplication.Visible = true;
                    break;
            }

            hfCurrentTab.Value = tabName.ToLower();
            upMain.Update();
        }

        #endregion

        #region Navigation Buttons

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();

            string currentTab = hfCurrentTab.Value;
            switch (currentTab)
            {
                case "position":
                    SetActiveTab("Personal");
                    break;
                case "background":
                    SetActiveTab("Position");
                    break;
                case "education":
                    SetActiveTab("Background");
                    break;
                case "employment":
                    SetActiveTab("Education");
                    break;
                case "references":
                    SetActiveTab("Employment");
                    break;
                case "authorization":
                    SetActiveTab("References");
                    break;
            }
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            SaveCurrentTabData();

            string currentTab = hfCurrentTab.Value;
            switch (currentTab)
            {
                case "personal":
                    SetActiveTab("Position");
                    break;
                case "position":
                    SetActiveTab("Background");
                    break;
                case "background":
                    SetActiveTab("Education");
                    break;
                case "education":
                    SetActiveTab("Employment");
                    break;
                case "employment":
                    SetActiveTab("References");
                    break;
                case "references":
                    SetActiveTab("Authorization");
                    break;
            }
        }

        #endregion

        #region Save and Submit

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            try
            {
                SaveApplicationData(false);
                ShowSuccessMessage("Application draft saved successfully!");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error saving draft: " + ex.Message);
            }
        }

        protected void btnSubmitApplication_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateRequiredFields())
                {
                    ShowErrorMessage("Please complete all required fields and acknowledge the final statement before submitting.");
                    return;
                }

                SaveApplicationData(true);
                ShowSuccessMessage("Application submitted successfully! You will be redirected to your dashboard.");

                // Redirect after a short delay
                ClientScript.RegisterStartupScript(this.GetType(), "redirect",
                    "setTimeout(function(){ window.location.href = '../Dashboard/EmployeeDashboard.aspx'; }, 3000);", true);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error submitting application: " + ex.Message);
            }
        }

        private bool ValidateRequiredFields()
        {
            // Check required personal information
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                return false;
            }

            // Check final acknowledgment
            if (!chkFinalAcknowledgment.Checked)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Data Operations

        private void LoadExistingApplication()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check for existing application
                    string sql = @"
                        SELECT TOP 1 * FROM [EmploymentApplications] 
                        WHERE [EmployeeId] = @EmployeeId 
                        AND [Status] IN ('Draft', 'Pending')
                        ORDER BY [ApplicationDate] DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", currentEmployeeId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                PopulateFormFromDatabase(reader);
                                hfApplicationId.Value = reader["Id"].ToString();
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

            if (GetStringValue(reader, "SalaryType") == "Hourly")
                rbHourly.Checked = true;
            else if (GetStringValue(reader, "SalaryType") == "Yearly")
                rbYearly.Checked = true;

            txtAvailableStartDate.Text = GetDateValue(reader, "AvailableStartDate");

            string employmentType = GetStringValue(reader, "EmploymentSought");
            rbFullTime.Checked = employmentType == "Full Time";
            rbPartTime.Checked = employmentType == "Part Time";
            rbTemporary.Checked = employmentType == "Temporary";

            // Location preferences
            chkNashville.Checked = GetBoolValue(reader, "NashvilleLocation");
            chkFranklin.Checked = GetBoolValue(reader, "FranklinLocation");
            chkShelbyville.Checked = GetBoolValue(reader, "ShelbyvilleLocation");
            chkWaynesboro.Checked = GetBoolValue(reader, "WaynesboroLocation");
            txtOtherLocation.Text = GetStringValue(reader, "OtherLocation");

            // Shift preferences
            chkFirstShift.Checked = GetBoolValue(reader, "FirstShift");
            chkSecondShift.Checked = GetBoolValue(reader, "SecondShift");
            chkThirdShift.Checked = GetBoolValue(reader, "ThirdShift");
            chkWeekendsOnly.Checked = GetBoolValue(reader, "WeekendsOnly");

            // Days available
            chkMonday.Checked = GetBoolValue(reader, "MondayAvailable");
            chkTuesday.Checked = GetBoolValue(reader, "TuesdayAvailable");
            chkWednesday.Checked = GetBoolValue(reader, "WednesdayAvailable");
            chkThursday.Checked = GetBoolValue(reader, "ThursdayAvailable");
            chkFriday.Checked = GetBoolValue(reader, "FridayAvailable");
            chkSaturday.Checked = GetBoolValue(reader, "SaturdayAvailable");
            chkSunday.Checked = GetBoolValue(reader, "SundayAvailable");

            // Background Questions
            rbAppliedBeforeYes.Checked = GetBoolValue(reader, "PreviouslyAppliedToTPA");
            rbAppliedBeforeNo.Checked = !GetBoolValue(reader, "PreviouslyAppliedToTPA");
            txtAppliedBeforeWhen.Text = GetStringValue(reader, "PreviousApplicationDate");

            rbWorkedBeforeYes.Checked = GetBoolValue(reader, "PreviouslyWorkedForTPA");
            rbWorkedBeforeNo.Checked = !GetBoolValue(reader, "PreviouslyWorkedForTPA");
            txtWorkedBeforeWhen.Text = GetStringValue(reader, "PreviousWorkDate");

            rbFamilyEmployedYes.Checked = GetBoolValue(reader, "FamilyMembersEmployedByTPA");
            rbFamilyEmployedNo.Checked = !GetBoolValue(reader, "FamilyMembersEmployedByTPA");
            txtFamilyEmployedWho.Text = GetStringValue(reader, "FamilyMemberDetails");

            rbUSCitizenYes.Checked = GetBoolValue(reader, "USCitizen");
            rbUSCitizenNo.Checked = !GetBoolValue(reader, "USCitizen");
            txtAlienNumber.Text = GetStringValue(reader, "AlienNumber");

            rbLegallyEntitledYes.Checked = GetBoolValue(reader, "LegallyEntitledToWork");
            rbLegallyEntitledNo.Checked = !GetBoolValue(reader, "LegallyEntitledToWork");

            rbOver18Yes.Checked = GetBoolValue(reader, "Is18OrOlder");
            rbOver18No.Checked = !GetBoolValue(reader, "Is18OrOlder");

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

            // Special Skills
            txtSpecialKnowledge.Text = GetStringValue(reader, "SpecialSkills");
            txtDIDDTraining.Text = GetStringValue(reader, "DIDDTraining");

            // Background form fields
            txtSSNLast4.Text = GetStringValue(reader, "SSNLast4");
            txtReferenceAuthName.Text = GetStringValue(reader, "FirstName") + " " + GetStringValue(reader, "LastName");

            // DIDD Authorization fields
            chkDIDDNoAbuse.Checked = GetBoolValue(reader, "DIDDNoAbuse");
            chkDIDDHadAbuse.Checked = GetBoolValue(reader, "DIDDHadAbuse");
            chkProtectionNoAbuse.Checked = GetBoolValue(reader, "ProtectionNoAbuse");
            chkProtectionHadAbuse.Checked = GetBoolValue(reader, "ProtectionHadAbuse");
            chkFinalAcknowledgment.Checked = GetBoolValue(reader, "FinalAcknowledgment");
        }

        private void SaveCurrentTabData()
        {
            try
            {
                SaveApplicationData(false);
            }
            catch (Exception)
            {
                // Silent save - don't show errors for auto-save
            }
        }

        private void SaveApplicationData(bool isSubmission)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        int applicationId;

                        if (!string.IsNullOrEmpty(hfApplicationId.Value))
                        {
                            // Update existing application
                            applicationId = Convert.ToInt32(hfApplicationId.Value);
                            UpdateApplication(conn, transaction, applicationId, isSubmission);
                        }
                        else
                        {
                            // Insert new application
                            applicationId = InsertApplication(conn, transaction, isSubmission);
                            hfApplicationId.Value = applicationId.ToString();
                        }

                        // Save related data
                        SaveEducationData(applicationId, conn, transaction);
                        SaveLicensesData(applicationId, conn, transaction);
                        SaveEmploymentHistoryData(applicationId, conn, transaction);
                        SaveReferencesData(applicationId, conn, transaction);
                        SaveCriminalHistoryData(applicationId, conn, transaction);
                        SaveFormerAddressesData(applicationId, conn, transaction);

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private int InsertApplication(SqlConnection conn, SqlTransaction transaction, bool isSubmission)
        {
            string applicationNumber = GenerateApplicationNumber();
            string status = isSubmission ? "Submitted" : "Draft";

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
                    [SpecialSkills], [DIDDTraining],
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
                    @SpecialSkills, @DIDDTraining,
                    @CreatedAt, @UpdatedAt, @SubmittedAt
                );
                SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                AddApplicationParameters(cmd, applicationNumber, status, isSubmission);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void UpdateApplication(SqlConnection conn, SqlTransaction transaction, int applicationId, bool isSubmission)
        {
            string status = isSubmission ? "Submitted" : "Draft";

            string sql = @"
                UPDATE [EmploymentApplications] SET
                    [Status] = @Status,
                    [FirstName] = @FirstName, [MiddleName] = @MiddleName, [LastName] = @LastName,
                    [HomeAddress] = @HomeAddress, [AptNumber] = @AptNumber,
                    [City] = @City, [State] = @State, [ZipCode] = @ZipCode,
                    [SSN] = @SSN, [DriversLicense] = @DriversLicense, [DLState] = @DLState,
                    [PhoneNumber] = @PhoneNumber, [CellNumber] = @CellNumber,
                    [EmergencyContactName] = @EmergencyContactName, [EmergencyContactRelationship] = @EmergencyContactRelationship,
                    [EmergencyContactAddress] = @EmergencyContactAddress, [Position1] = @Position1, [Position2] = @Position2,
                    [SalaryDesired] = @SalaryDesired, [SalaryType] = @SalaryType, [EmploymentSought] = @EmploymentSought,
                    [AvailableStartDate] = @AvailableStartDate,
                    [NashvilleLocation] = @NashvilleLocation, [FranklinLocation] = @FranklinLocation,
                    [ShelbyvilleLocation] = @ShelbyvilleLocation, [WaynesboroLocation] = @WaynesboroLocation,
                    [OtherLocation] = @OtherLocation, [FirstShift] = @FirstShift, [SecondShift] = @SecondShift,
                    [ThirdShift] = @ThirdShift, [WeekendsOnly] = @WeekendsOnly,
                    [MondayAvailable] = @MondayAvailable, [TuesdayAvailable] = @TuesdayAvailable,
                    [WednesdayAvailable] = @WednesdayAvailable, [ThursdayAvailable] = @ThursdayAvailable,
                    [FridayAvailable] = @FridayAvailable, [SaturdayAvailable] = @SaturdayAvailable,
                    [SundayAvailable] = @SundayAvailable,
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
                    [SpecialSkills] = @SpecialSkills, [DIDDTraining] = @DIDDTraining,
                    [UpdatedAt] = @UpdatedAt" + (isSubmission ? ", [SubmittedAt] = @SubmittedAt" : "") + @"
                WHERE [Id] = @ApplicationId";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                AddApplicationParameters(cmd, null, status, isSubmission);
            }
        }

        private void AddApplicationParameters(SqlCommand cmd, string applicationNumber, string status, bool isSubmission)
        {
            if (!string.IsNullOrEmpty(applicationNumber))
                cmd.Parameters.AddWithValue("@ApplicationNumber", applicationNumber);

            cmd.Parameters.AddWithValue("@ApplicationDate", GetDateOrNull(txtApplicationDate.Text) ?? DateTime.Now);
            cmd.Parameters.AddWithValue("@Status", status);
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

            // Location preferences
            cmd.Parameters.AddWithValue("@NashvilleLocation", chkNashville.Checked);
            cmd.Parameters.AddWithValue("@FranklinLocation", chkFranklin.Checked);
            cmd.Parameters.AddWithValue("@ShelbyvilleLocation", chkShelbyville.Checked);
            cmd.Parameters.AddWithValue("@WaynesboroLocation", chkWaynesboro.Checked);
            cmd.Parameters.AddWithValue("@OtherLocation", GetTextOrNull(txtOtherLocation.Text));

            // Shift preferences
            cmd.Parameters.AddWithValue("@FirstShift", chkFirstShift.Checked);
            cmd.Parameters.AddWithValue("@SecondShift", chkSecondShift.Checked);
            cmd.Parameters.AddWithValue("@ThirdShift", chkThirdShift.Checked);
            cmd.Parameters.AddWithValue("@WeekendsOnly", chkWeekendsOnly.Checked);

            // Days available
            cmd.Parameters.AddWithValue("@MondayAvailable", chkMonday.Checked);
            cmd.Parameters.AddWithValue("@TuesdayAvailable", chkTuesday.Checked);
            cmd.Parameters.AddWithValue("@WednesdayAvailable", chkWednesday.Checked);
            cmd.Parameters.AddWithValue("@ThursdayAvailable", chkThursday.Checked);
            cmd.Parameters.AddWithValue("@FridayAvailable", chkFriday.Checked);
            cmd.Parameters.AddWithValue("@SaturdayAvailable", chkSaturday.Checked);
            cmd.Parameters.AddWithValue("@SundayAvailable", chkSunday.Checked);

            // Background Questions
            cmd.Parameters.AddWithValue("@PreviouslyAppliedToTPA", rbAppliedBeforeYes.Checked);
            cmd.Parameters.AddWithValue("@PreviousApplicationDate", GetTextOrNull(txtAppliedBeforeWhen.Text));
            cmd.Parameters.AddWithValue("@PreviouslyWorkedForTPA", rbWorkedBeforeYes.Checked);
            cmd.Parameters.AddWithValue("@PreviousWorkDate", GetTextOrNull(txtWorkedBeforeWhen.Text));
            cmd.Parameters.AddWithValue("@FamilyMembersEmployedByTPA", rbFamilyEmployedYes.Checked);
            cmd.Parameters.AddWithValue("@FamilyMemberDetails", GetTextOrNull(txtFamilyEmployedWho.Text));
            cmd.Parameters.AddWithValue("@USCitizen", rbUSCitizenYes.Checked);
            cmd.Parameters.AddWithValue("@PermanentResident", rbUSCitizenYes.Checked); // Same as US Citizen for this form
            cmd.Parameters.AddWithValue("@AlienNumber", GetTextOrNull(txtAlienNumber.Text));
            cmd.Parameters.AddWithValue("@LegallyEntitledToWork", rbLegallyEntitledYes.Checked);
            cmd.Parameters.AddWithValue("@Is18OrOlder", rbOver18Yes.Checked);
            cmd.Parameters.AddWithValue("@ServedArmedForces", rbArmedForcesYes.Checked);
            cmd.Parameters.AddWithValue("@ConvictedOfCrime", rbConvictedYes.Checked);
            cmd.Parameters.AddWithValue("@OnAbuseRegistry", rbAbuseRegistryYes.Checked);
            cmd.Parameters.AddWithValue("@FoundGuiltyAbuse", rbFoundGuiltyYes.Checked);
            cmd.Parameters.AddWithValue("@LicenseRevoked", rbLicenseRevokedYes.Checked);

            // Background form fields
            cmd.Parameters.AddWithValue("@SSNLast4", GetTextOrNull(txtSSNLast4.Text));
            cmd.Parameters.AddWithValue("@NameOther", GetTextOrNull(txtBGOtherName.Text));
            cmd.Parameters.AddWithValue("@YearNameChange", GetTextOrNull(txtBGNameChangeYear.Text));
            cmd.Parameters.AddWithValue("@DriversLicenseNumber", GetTextOrNull(txtBGDriversLicense.Text));
            cmd.Parameters.AddWithValue("@DriversLicenseState", GetTextOrNull(txtBGDLState.Text));
            cmd.Parameters.AddWithValue("@DateOfBirth", GetDateOrNull(txtBGDateOfBirth.Text));
            cmd.Parameters.AddWithValue("@NameOnLicense", GetTextOrNull(txtBGNameOnLicense.Text));
            cmd.Parameters.AddWithValue("@ConvictedCriminal7Years", rbConvicted7YearsYes.Checked);
            cmd.Parameters.AddWithValue("@ChargedInvestigation", rbChargedInvestigationYes.Checked);

            // DIDD Authorization
            cmd.Parameters.AddWithValue("@DIDDNoAbuse", chkDIDDNoAbuse.Checked);
            cmd.Parameters.AddWithValue("@DIDDHadAbuse", chkDIDDHadAbuse.Checked);
            cmd.Parameters.AddWithValue("@ProtectionNoAbuse", chkProtectionNoAbuse.Checked);
            cmd.Parameters.AddWithValue("@ProtectionHadAbuse", chkProtectionHadAbuse.Checked);
            cmd.Parameters.AddWithValue("@FinalAcknowledgment", chkFinalAcknowledgment.Checked);

            // Special Skills
            cmd.Parameters.AddWithValue("@SpecialSkills", GetTextOrNull(txtSpecialKnowledge.Text));
            cmd.Parameters.AddWithValue("@DIDDTraining", GetTextOrNull(txtDIDDTraining.Text));

            // Timestamps
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);

            if (isSubmission)
                cmd.Parameters.AddWithValue("@SubmittedAt", DateTime.UtcNow);

            cmd.ExecuteNonQuery();
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
                new { Level = "Elementary", School = txtElementarySchool.Text, Major = txtElemMajor.Text, Skills = txtElemSkills.Text, Diploma = rbElemDiplomaYes.Checked },
                new { Level = "High School", School = txtHighSchool.Text, Major = txtHSMajor.Text, Skills = txtHSSkills.Text, Diploma = rbHSDiplomaYes.Checked },
                new { Level = "Undergraduate", School = txtUndergraduateSchool.Text, Major = txtUGMajor.Text, Skills = txtUGSkills.Text, Diploma = rbUGDegreeYes.Checked },
                new { Level = "Graduate", School = txtGraduateSchool.Text, Major = txtGradMajor.Text, Skills = txtGradSkills.Text, Diploma = rbGradDegreeYes.Checked }
            };

            string insertSql = @"
                INSERT INTO [EmploymentApplicationEducation] 
                ([ApplicationId], [EducationLevel], [SchoolName], [Major], [Skills], [DiplomaReceived])
                VALUES (@ApplicationId, @EducationLevel, @SchoolName, @Major, @Skills, @DiplomaReceived)";

            foreach (var education in educationLevels)
            {
                if (!string.IsNullOrWhiteSpace(education.School))
                {
                    using (SqlCommand cmd = new SqlCommand(insertSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                        cmd.Parameters.AddWithValue("@EducationLevel", education.Level);
                        cmd.Parameters.AddWithValue("@SchoolName", education.School);
                        cmd.Parameters.AddWithValue("@Major", GetTextOrNull(education.Major));
                        cmd.Parameters.AddWithValue("@Skills", GetTextOrNull(education.Skills));
                        cmd.Parameters.AddWithValue("@DiplomaReceived", education.Diploma);
                        cmd.ExecuteNonQuery();
                    }
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
                        cmd.Parameters.AddWithValue("@ExpirationDate", GetTextOrNull(license.Expiration));
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
                ([ApplicationId], [ReferenceName], [ReferencePhone], [ReferenceEmail], [YearsKnown], [ReferenceType])
                VALUES (@ApplicationId, @ReferenceName, @ReferencePhone, @ReferenceEmail, @YearsKnown, @ReferenceType)";

            foreach (var reference in references)
            {
                if (!string.IsNullOrWhiteSpace(reference.Name))
                {
                    using (SqlCommand cmd = new SqlCommand(insertSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                        cmd.Parameters.AddWithValue("@ReferenceName", reference.Name);
                        cmd.Parameters.AddWithValue("@ReferencePhone", GetTextOrNull(reference.Phone));
                        cmd.Parameters.AddWithValue("@ReferenceEmail", GetTextOrNull(reference.Email));
                        cmd.Parameters.AddWithValue("@YearsKnown", GetTextOrNull(reference.Years));
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
                new { Date = txtCriminalDate1.Text, Charge = txtCriminalCharge1.Text, Status = txtCriminalStatus1.Text },
                new { Date = txtCriminalDate2.Text, Charge = txtCriminalCharge2.Text, Status = txtCriminalStatus2.Text },
                new { Date = txtCriminalDate3.Text, Charge = txtCriminalCharge3.Text, Status = txtCriminalStatus3.Text }
            };

            string insertSql = @"
                INSERT INTO [EmploymentApplicationCriminalHistory] 
                ([ApplicationId], [IncidentDate], [Charge], [StatusOutcome])
                VALUES (@ApplicationId, @IncidentDate, @Charge, @StatusOutcome)";

            foreach (var incident in criminalHistory)
            {
                if (!string.IsNullOrWhiteSpace(incident.Charge))
                {
                    using (SqlCommand cmd = new SqlCommand(insertSql, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                        cmd.Parameters.AddWithValue("@IncidentDate", GetDateOrNull(incident.Date));
                        cmd.Parameters.AddWithValue("@Charge", incident.Charge);
                        cmd.Parameters.AddWithValue("@StatusOutcome", GetTextOrNull(incident.Status));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void SaveFormerAddressesData(int applicationId, SqlConnection conn, SqlTransaction transaction)
        {
            // Delete existing former address records
            string deleteSql = "DELETE FROM [EmploymentApplicationFormerAddresses] WHERE [ApplicationId] = @ApplicationId";
            using (SqlCommand deleteCmd = new SqlCommand(deleteSql, conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                deleteCmd.ExecuteNonQuery();
            }

            // Insert former address records
            var formerAddresses = new[]
            {
                new { Street = txtFormerStreet1.Text, City = txtFormerCity1.Text, State = txtFormerState1.Text, Years = txtFormerYears1.Text, Order = 1 },
                new { Street = txtFormerStreet2.Text, City = txtFormerCity2.Text, State = txtFormerState2.Text, Years = txtFormerYears2.Text, Order = 2 },
                new { Street = txtFormerStreet3.Text, City = txtFormerCity3.Text, State = txtFormerState3.Text, Years = txtFormerYears3.Text, Order = 3 }
            };

            string insertSql = @"
                INSERT INTO [EmploymentApplicationFormerAddresses] 
                ([ApplicationId], [Street], [City], [State], [YearsResided], [SequenceOrder])
                VALUES (@ApplicationId, @Street, @City, @State, @YearsResided, @SequenceOrder)";

            foreach (var address in formerAddresses)
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
                        cmd.Parameters.AddWithValue("@SequenceOrder", address.Order);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        protected void cvFinalAcknowledgment_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = chkFinalAcknowledgment.Checked;
        }

        #endregion

        #region Helper Methods

        private string GenerateApplicationNumber()
        {
            string applicationNumber = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("sp_GenerateApplicationNumber", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter outputParam = new SqlParameter("@ApplicationNumber", SqlDbType.NVarChar, 50)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);

                    cmd.ExecuteNonQuery();
                    applicationNumber = outputParam.Value.ToString();
                }
            }

            return applicationNumber;
        }

        private string GetSalaryType()
        {
            if (rbHourly.Checked) return "Hourly";
            if (rbYearly.Checked) return "Yearly";
            return null;
        }

        private string GetEmploymentType()
        {
            if (rbFullTime.Checked) return "Full Time";
            if (rbPartTime.Checked) return "Part Time";
            if (rbTemporary.Checked) return "Temporary";
            return null;
        }

        private object GetTextOrNull(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? (object)DBNull.Value : text.Trim();
        }

        private object GetDateOrNull(string dateText)
        {
            if (string.IsNullOrWhiteSpace(dateText)) return DBNull.Value;
            if (DateTime.TryParse(dateText, out DateTime result))
                return result;
            return DBNull.Value;
        }

        private object GetDecimalOrNull(string decimalText)
        {
            if (string.IsNullOrWhiteSpace(decimalText)) return DBNull.Value;
            if (decimal.TryParse(decimalText, out decimal result))
                return result;
            return DBNull.Value;
        }

        private object GetIntOrNull(string intText)
        {
            if (string.IsNullOrWhiteSpace(intText)) return DBNull.Value;
            if (int.TryParse(intText, out int result))
                return result;
            return DBNull.Value;
        }

        private string GetStringValue(SqlDataReader reader, string columnName)
        {
            try
            {
                var value = reader[columnName];
                return value == DBNull.Value ? "" : value.ToString();
            }
            catch
            {
                return "";
            }
        }

        private bool GetBoolValue(SqlDataReader reader, string columnName)
        {
            try
            {
                var value = reader[columnName];
                if (value == DBNull.Value) return false;
                return Convert.ToBoolean(value);
            }
            catch
            {
                return false;
            }
        }

        private string GetDateValue(SqlDataReader reader, string columnName)
        {
            try
            {
                var value = reader[columnName];
                if (value == DBNull.Value) return "";
                if (DateTime.TryParse(value.ToString(), out DateTime result))
                    return result.ToString("yyyy-MM-dd");
                return "";
            }
            catch
            {
                return "";
            }
        }

        private void ShowSuccessMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "message-text success";
            pnlMessages.Visible = true;
            pnlMessages.CssClass = "message-panel success";
        }

        private void ShowErrorMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "message-text error";
            pnlMessages.Visible = true;
            pnlMessages.CssClass = "message-panel error";
        }

        #endregion
    }
}