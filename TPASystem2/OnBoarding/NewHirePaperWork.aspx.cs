using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.OnBoarding
{
    public partial class NewHirePaperWorkTabbed : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int currentEmployeeId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
            }
        }

        private void InitializePage()
        {
            try
            {
                // Get current employee ID from session
                if (Session["UserId"] != null)
                {
                    currentEmployeeId = Convert.ToInt32(Session["UserId"]);
                }
                else
                {
                    ShowErrorMessage("Session expired. Please login again.");
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                // Verify employee exists
                if (!VerifyEmployee())
                {
                    ShowErrorMessage("Invalid employee. Please contact HR to complete your employee profile setup.");
                    return;
                }

                // Set default application date
                txtApplicationDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtBGSignatureDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                // Hide success message initially
                pnlSuccessMessage.Visible = false;
                pnlMessages.Visible = false;

                // Load existing application data if available
                LoadApplicationData();

                // Populate background check fields from personal info
                PopulateBackgroundCheckFields();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error initializing page: " + ex.Message);
            }
        }

        private bool VerifyEmployee()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT COUNT(*) FROM [Employees] WHERE [Id] = @EmployeeId AND [IsActive] = 1";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", currentEmployeeId);
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private void LoadApplicationData()
        {
            try
            {
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
            txtAvailableStartDate.Text = GetDateValue(reader, "AvailableStartDate");

            // Set checkboxes for salary type
            string salaryType = GetStringValue(reader, "SalaryType");
            chkHourly.Checked = salaryType.Contains("Hourly");
            chkYearly.Checked = salaryType.Contains("Yearly");

            // Employment type
            string employmentSought = GetStringValue(reader, "EmploymentSought");
            chkFullTime.Checked = employmentSought.Contains("Full Time");
            chkPartTime.Checked = employmentSought.Contains("Part Time");
            chkTemporary.Checked = employmentSought.Contains("Temporary");

            // Locations
            chkNashville.Checked = GetBoolValue(reader, "NashvilleLocation");
            chkFranklin.Checked = GetBoolValue(reader, "FranklinLocation");
            chkShelbyville.Checked = GetBoolValue(reader, "ShelbyvilleLocation");
            chkWaynesboro.Checked = GetBoolValue(reader, "WaynesboroLocation");
            chkOtherLocation.Checked = !string.IsNullOrEmpty(GetStringValue(reader, "OtherLocation"));
            txtOtherLocation.Text = GetStringValue(reader, "OtherLocation");

            // Shifts
            chk1stShift.Checked = GetBoolValue(reader, "FirstShift");
            chk2ndShift.Checked = GetBoolValue(reader, "SecondShift");
            chk3rdShift.Checked = GetBoolValue(reader, "ThirdShift");
            chkWeekendsOnly.Checked = GetBoolValue(reader, "WeekendsOnly");

            // Days Available
            chkMon.Checked = GetBoolValue(reader, "MondayAvailable");
            chkTues.Checked = GetBoolValue(reader, "TuesdayAvailable");
            chkWed.Checked = GetBoolValue(reader, "WednesdayAvailable");
            chkThurs.Checked = GetBoolValue(reader, "ThursdayAvailable");
            chkFri.Checked = GetBoolValue(reader, "FridayAvailable");
            chkSat.Checked = GetBoolValue(reader, "SaturdayAvailable");
            chkSun.Checked = GetBoolValue(reader, "SundayAvailable");

            // Background Questions
            rbAppliedTPAYes.Checked = GetBoolValue(reader, "PreviouslyAppliedToTPA");
            rbAppliedTPANo.Checked = !GetBoolValue(reader, "PreviouslyAppliedToTPA");
            txtAppliedTPADate.Text = GetStringValue(reader, "PreviousApplicationDate");

            rbWorkedTPAYes.Checked = GetBoolValue(reader, "PreviouslyWorkedForTPA");
            rbWorkedTPANo.Checked = !GetBoolValue(reader, "PreviouslyWorkedForTPA");
            txtWorkedTPADate.Text = GetStringValue(reader, "PreviousWorkDate");

            rbFamilyTPAYes.Checked = GetBoolValue(reader, "FamilyMembersEmployedByTPA");
            rbFamilyTPANo.Checked = !GetBoolValue(reader, "FamilyMembersEmployedByTPA");
            txtFamilyTPADetails.Text = GetStringValue(reader, "FamilyMemberDetails");

            rbUSCitizenYes.Checked = GetBoolValue(reader, "USCitizen");
            rbUSCitizenNo.Checked = !GetBoolValue(reader, "USCitizen");
            txtAlienNumber.Text = GetStringValue(reader, "AlienNumber");

            rbLegalWorkYes.Checked = GetBoolValue(reader, "LegallyEntitledToWork");
            rbLegalWorkNo.Checked = !GetBoolValue(reader, "LegallyEntitledToWork");

            rb18OrOlderYes.Checked = GetBoolValue(reader, "Is18OrOlder");
            rb18OrOlderNo.Checked = !GetBoolValue(reader, "Is18OrOlder");

            rbArmedForcesYes.Checked = GetBoolValue(reader, "ServedArmedForces");
            rbArmedForcesNo.Checked = !GetBoolValue(reader, "ServedArmedForces");

            rbConvictedYes.Checked = GetBoolValue(reader, "ConvictedOfCrime");
            rbConvictedNo.Checked = !GetBoolValue(reader, "ConvictedOfCrime");

            rbAbuseRegistryYes.Checked = GetBoolValue(reader, "OnAbuseRegistry");
            rbAbuseRegistryNo.Checked = !GetBoolValue(reader, "OnAbuseRegistry");

            rbAbuseFoundGuiltyYes.Checked = GetBoolValue(reader, "FoundGuiltyAbuse");
            rbAbuseFoundGuiltyNo.Checked = !GetBoolValue(reader, "FoundGuiltyAbuse");

            rbLicenseRevokedYes.Checked = GetBoolValue(reader, "LicenseRevoked");
            rbLicenseRevokedNo.Checked = !GetBoolValue(reader, "LicenseRevoked");

            // Background Check Information
            txtBGLastName.Text = GetStringValue(reader, "LastName");
            txtBGFirstName.Text = GetStringValue(reader, "FirstName");
            txtBGMiddleName.Text = GetStringValue(reader, "MiddleName");
            txtBGStreet.Text = GetStringValue(reader, "HomeAddress");
            txtBGCity.Text = GetStringValue(reader, "City");
            txtBGState.Text = GetStringValue(reader, "State");
            txtBGZipCode.Text = GetStringValue(reader, "ZipCode");
            txtBGSSN.Text = GetStringValue(reader, "SSN");
            txtBGPhone.Text = GetStringValue(reader, "PhoneNumber");
            txtBGOtherNames.Text = GetStringValue(reader, "NameOther");
            txtBGNameChangeYear.Text = GetStringValue(reader, "YearNameChange");
            txtBGDriversLicense.Text = GetStringValue(reader, "DriversLicenseNumber");
            txtBGDriversLicenseState.Text = GetStringValue(reader, "DriversLicenseState");
            txtBGDateOfBirth.Text = GetDateValue(reader, "DateOfBirth");
            txtBGNameOnLicense.Text = GetStringValue(reader, "NameOnLicense");
            txtBGSSNLast4.Text = GetStringValue(reader, "SSNLast4");

            // Checkboxes for DIDD
            chkDIDDNoAbuse.Checked = GetBoolValue(reader, "DIDDNoAbuse");
            chkDIDDHadAbuse.Checked = GetBoolValue(reader, "DIDDHadAbuse");
            chkProtectionNoAbuse.Checked = GetBoolValue(reader, "ProtectionNoAbuse");
            chkProtectionHadAbuse.Checked = GetBoolValue(reader, "ProtectionHadAbuse");
            chkFinalAcknowledgment.Checked = GetBoolValue(reader, "FinalAcknowledgment");
        }

        private void PopulateBackgroundCheckFields()
        {
            // Auto-populate background check fields from personal information
            if (string.IsNullOrEmpty(txtBGLastName.Text))
                txtBGLastName.Text = txtLastName.Text;
            if (string.IsNullOrEmpty(txtBGFirstName.Text))
                txtBGFirstName.Text = txtFirstName.Text;
            if (string.IsNullOrEmpty(txtBGMiddleName.Text))
                txtBGMiddleName.Text = txtMiddleName.Text;
            if (string.IsNullOrEmpty(txtBGStreet.Text))
                txtBGStreet.Text = txtHomeAddress.Text;
            if (string.IsNullOrEmpty(txtBGCity.Text))
                txtBGCity.Text = txtCity.Text;
            if (string.IsNullOrEmpty(txtBGState.Text))
                txtBGState.Text = txtState.Text;
            if (string.IsNullOrEmpty(txtBGZipCode.Text))
                txtBGZipCode.Text = txtZipCode.Text;
            if (string.IsNullOrEmpty(txtBGSSN.Text))
                txtBGSSN.Text = txtSSN.Text;
            if (string.IsNullOrEmpty(txtBGPhone.Text))
                txtBGPhone.Text = txtPhoneNumber.Text;
            if (string.IsNullOrEmpty(txtBGDriversLicense.Text))
                txtBGDriversLicense.Text = txtDriversLicense.Text;
            if (string.IsNullOrEmpty(txtBGDriversLicenseState.Text))
                txtBGDriversLicenseState.Text = txtDLState.Text;

            // Populate DIDD fields
            if (string.IsNullOrEmpty(txtDIDDFullName.Text))
                txtDIDDFullName.Text = $"{txtLastName.Text}, {txtFirstName.Text} {txtMiddleName.Text}".Trim();
            if (string.IsNullOrEmpty(txtDIDDSSN.Text))
                txtDIDDSSN.Text = txtSSN.Text;
            if (string.IsNullOrEmpty(txtDIDDDriverLicense.Text))
                txtDIDDDriverLicense.Text = txtDriversLicense.Text;
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            try
            {
                SaveApplicationData(false); // Save as draft
                ShowSuccessMessage("Application saved as draft successfully!");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error saving draft: " + ex.Message);
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate required fields before submission
                if (!ValidateRequiredFields())
                {
                    ShowErrorMessage("Please complete all required fields before submitting.");
                    return;
                }

                SaveApplicationData(true); // Save as submitted
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
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
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
                    [ShelbyvilleLocation] = @ShelbyvilleLocation, [WaynesboroLocation] = @WaynesboroLocation, [OtherLocation] = @OtherLocation,
                    [FirstShift] = @FirstShift, [SecondShift] = @SecondShift, [ThirdShift] = @ThirdShift, [WeekendsOnly] = @WeekendsOnly,
                    [MondayAvailable] = @MondayAvailable, [TuesdayAvailable] = @TuesdayAvailable, [WednesdayAvailable] = @WednesdayAvailable, 
                    [ThursdayAvailable] = @ThursdayAvailable, [FridayAvailable] = @FridayAvailable, [SaturdayAvailable] = @SaturdayAvailable, 
                    [SundayAvailable] = @SundayAvailable,
                    [PreviouslyAppliedToTPA] = @PreviouslyAppliedToTPA, [PreviousApplicationDate] = @PreviousApplicationDate, 
                    [PreviouslyWorkedForTPA] = @PreviouslyWorkedForTPA, [PreviousWorkDate] = @PreviousWorkDate,
                    [FamilyMembersEmployedByTPA] = @FamilyMembersEmployedByTPA, [FamilyMemberDetails] = @FamilyMemberDetails,
                    [USCitizen] = @USCitizen, [PermanentResident] = @PermanentResident, [AlienNumber] = @AlienNumber, 
                    [LegallyEntitledToWork] = @LegallyEntitledToWork, [Is18OrOlder] = @Is18OrOlder,
                    [ServedArmedForces] = @ServedArmedForces, [ConvictedOfCrime] = @ConvictedOfCrime, 
                    [OnAbuseRegistry] = @OnAbuseRegistry, [FoundGuiltyAbuse] = @FoundGuiltyAbuse, [LicenseRevoked] = @LicenseRevoked,
                    [SSNLast4] = @SSNLast4, [NameOther] = @NameOther, [YearNameChange] = @YearNameChange, 
                    [DriversLicenseNumber] = @DriversLicenseNumber, [DriversLicenseState] = @DriversLicenseState,
                    [DateOfBirth] = @DateOfBirth, [NameOnLicense] = @NameOnLicense, 
                    [ConvictedCriminal7Years] = @ConvictedCriminal7Years, [ChargedInvestigation] = @ChargedInvestigation,
                    [DIDDNoAbuse] = @DIDDNoAbuse, [DIDDHadAbuse] = @DIDDHadAbuse, 
                    [ProtectionNoAbuse] = @ProtectionNoAbuse, [ProtectionHadAbuse] = @ProtectionHadAbuse, 
                    [FinalAcknowledgment] = @FinalAcknowledgment,
                    [SpecialSkills] = @SpecialSkills, [DIDDTraining] = @DIDDTraining,
                    [UpdatedAt] = @UpdatedAt" +
                    (isSubmission ? ", [SubmittedAt] = @SubmittedAt" : "") + @"
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

            cmd.Parameters.AddWithValue("@ApplicationDate", GetDateOrNull(txtApplicationDate.Text));
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
            cmd.Parameters.AddWithValue("@SalaryDesired", GetTextOrNull(txtSalaryDesired.Text));

            string salaryType = "";
            if (chkHourly.Checked) salaryType += "Hourly ";
            if (chkYearly.Checked) salaryType += "Yearly ";
            cmd.Parameters.AddWithValue("@SalaryType", salaryType.Trim());

            string employmentSought = "";
            if (chkFullTime.Checked) employmentSought += "Full Time ";
            if (chkPartTime.Checked) employmentSought += "Part Time ";
            if (chkTemporary.Checked) employmentSought += "Temporary ";
            cmd.Parameters.AddWithValue("@EmploymentSought", employmentSought.Trim());

            cmd.Parameters.AddWithValue("@AvailableStartDate", GetDateOrNull(txtAvailableStartDate.Text));

            // Locations
            cmd.Parameters.AddWithValue("@NashvilleLocation", chkNashville.Checked);
            cmd.Parameters.AddWithValue("@FranklinLocation", chkFranklin.Checked);
            cmd.Parameters.AddWithValue("@ShelbyvilleLocation", chkShelbyville.Checked);
            cmd.Parameters.AddWithValue("@WaynesboroLocation", chkWaynesboro.Checked);
            cmd.Parameters.AddWithValue("@OtherLocation", GetTextOrNull(txtOtherLocation.Text));

            // Shifts
            cmd.Parameters.AddWithValue("@FirstShift", chk1stShift.Checked);
            cmd.Parameters.AddWithValue("@SecondShift", chk2ndShift.Checked);
            cmd.Parameters.AddWithValue("@ThirdShift", chk3rdShift.Checked);
            cmd.Parameters.AddWithValue("@WeekendsOnly", chkWeekendsOnly.Checked);

            // Days Available
            cmd.Parameters.AddWithValue("@MondayAvailable", chkMon.Checked);
            cmd.Parameters.AddWithValue("@TuesdayAvailable", chkTues.Checked);
            cmd.Parameters.AddWithValue("@WednesdayAvailable", chkWed.Checked);
            cmd.Parameters.AddWithValue("@ThursdayAvailable", chkThurs.Checked);
            cmd.Parameters.AddWithValue("@FridayAvailable", chkFri.Checked);
            cmd.Parameters.AddWithValue("@SaturdayAvailable", chkSat.Checked);
            cmd.Parameters.AddWithValue("@SundayAvailable", chkSun.Checked);

            // Background Questions
            cmd.Parameters.AddWithValue("@PreviouslyAppliedToTPA", rbAppliedTPAYes.Checked);
            cmd.Parameters.AddWithValue("@PreviousApplicationDate", GetTextOrNull(txtAppliedTPADate.Text));
            cmd.Parameters.AddWithValue("@PreviouslyWorkedForTPA", rbWorkedTPAYes.Checked);
            cmd.Parameters.AddWithValue("@PreviousWorkDate", GetTextOrNull(txtWorkedTPADate.Text));
            cmd.Parameters.AddWithValue("@FamilyMembersEmployedByTPA", rbFamilyTPAYes.Checked);
            cmd.Parameters.AddWithValue("@FamilyMemberDetails", GetTextOrNull(txtFamilyTPADetails.Text));

            cmd.Parameters.AddWithValue("@USCitizen", rbUSCitizenYes.Checked);
            cmd.Parameters.AddWithValue("@PermanentResident", rbUSCitizenYes.Checked);
            cmd.Parameters.AddWithValue("@AlienNumber", GetTextOrNull(txtAlienNumber.Text));
            cmd.Parameters.AddWithValue("@LegallyEntitledToWork", rbLegalWorkYes.Checked);
            cmd.Parameters.AddWithValue("@Is18OrOlder", rb18OrOlderYes.Checked);

            cmd.Parameters.AddWithValue("@ServedArmedForces", rbArmedForcesYes.Checked);
            cmd.Parameters.AddWithValue("@ConvictedOfCrime", rbConvictedYes.Checked);
            cmd.Parameters.AddWithValue("@OnAbuseRegistry", rbAbuseRegistryYes.Checked);
            cmd.Parameters.AddWithValue("@FoundGuiltyAbuse", rbAbuseFoundGuiltyYes.Checked);
            cmd.Parameters.AddWithValue("@LicenseRevoked", rbLicenseRevokedYes.Checked);

            // Background Check Information
            cmd.Parameters.AddWithValue("@SSNLast4", GetTextOrNull(txtBGSSNLast4.Text));
            cmd.Parameters.AddWithValue("@NameOther", GetTextOrNull(txtBGOtherNames.Text));
            cmd.Parameters.AddWithValue("@YearNameChange", GetTextOrNull(txtBGNameChangeYear.Text));
            cmd.Parameters.AddWithValue("@DriversLicenseNumber", GetTextOrNull(txtBGDriversLicense.Text));
            cmd.Parameters.AddWithValue("@DriversLicenseState", GetTextOrNull(txtBGDriversLicenseState.Text));
            cmd.Parameters.AddWithValue("@DateOfBirth", GetDateOrNull(txtBGDateOfBirth.Text));
            cmd.Parameters.AddWithValue("@NameOnLicense", GetTextOrNull(txtBGNameOnLicense.Text));

            cmd.Parameters.AddWithValue("@ConvictedCriminal7Years", rbBGConvicted7YearsYes.Checked);
            cmd.Parameters.AddWithValue("@ChargedInvestigation", rbBGChargedInvestigationYes.Checked);

            // DIDD Authorization
            cmd.Parameters.AddWithValue("@DIDDNoAbuse", chkDIDDNoAbuse.Checked);
            cmd.Parameters.AddWithValue("@DIDDHadAbuse", chkDIDDHadAbuse.Checked);
            cmd.Parameters.AddWithValue("@ProtectionNoAbuse", chkProtectionNoAbuse.Checked);
            cmd.Parameters.AddWithValue("@ProtectionHadAbuse", chkProtectionHadAbuse.Checked);
            cmd.Parameters.AddWithValue("@FinalAcknowledgment", chkFinalAcknowledgment.Checked);

            // Additional Fields
            cmd.Parameters.AddWithValue("@SpecialSkills", GetTextOrNull(txtSpecialSkills.Text));
            cmd.Parameters.AddWithValue("@DIDDTraining", GetTextOrNull(txtDIDDTraining.Text));

            // Timestamps
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            if (isSubmission)
            {
                cmd.Parameters.AddWithValue("@SubmittedAt", DateTime.UtcNow);
            }

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
            var educationData = new[]
            {
                new { Level = "Elementary", School = txtElementarySchool.Text, Major = txtElemMajor.Text, Skills = txtElemSkills.Text, Diploma = rbElemDiplomaYes.Checked },
                new { Level = "High School", School = txtHighSchool.Text, Major = txtHSMajor.Text, Skills = txtHSSkills.Text, Diploma = rbHSDiplomaYes.Checked },
                new { Level = "Undergraduate", School = txtUndergraduate.Text, Major = txtUGMajor.Text, Skills = txtUGSkills.Text, Diploma = rbUGDiplomaYes.Checked },
                new { Level = "Graduate", School = txtGraduate.Text, Major = txtGradMajor.Text, Skills = txtGradSkills.Text, Diploma = rbGradDiplomaYes.Checked }
            };

            string insertSql = @"
                INSERT INTO [EmploymentApplicationEducation] 
                ([ApplicationId], [EducationLevel], [SchoolName], [Major], [Skills], [DiplomaReceived])
                VALUES (@ApplicationId, @EducationLevel, @SchoolName, @Major, @Skills, @DiplomaReceived)";

            foreach (var education in educationData)
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
                new { Street = txtBGFormerStreet1.Text, City = txtBGFormerCity1.Text, State = txtBGFormerState1.Text, Years = txtBGFormerYears1.Text },
                new { Street = txtBGFormerStreet2.Text, City = txtBGFormerCity2.Text, State = txtBGFormerState2.Text, Years = txtBGFormerYears2.Text },
                new { Street = txtBGFormerStreet3.Text, City = txtBGFormerCity3.Text, State = txtBGFormerState3.Text, Years = txtBGFormerYears3.Text }
            };

            string insertSql = @"
                INSERT INTO [EmploymentApplicationFormerAddresses] 
                ([ApplicationId], [Street], [City], [State], [YearsResided])
                VALUES (@ApplicationId, @Street, @City, @State, @YearsResided)";

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
                        cmd.Parameters.AddWithValue("@YearsResided", GetTextOrNull(address.Years));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private string GenerateApplicationNumber()
        {
            return "APP" + DateTime.Now.ToString("yyyyMMddHHmmss") + currentEmployeeId.ToString("D4");
        }

        // Helper methods for safe data retrieval and conversion
        private string GetStringValue(SqlDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
            }
            catch
            {
                return string.Empty;
            }
        }

        private bool GetBoolValue(SqlDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return !reader.IsDBNull(ordinal) && reader.GetBoolean(ordinal);
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
                int ordinal = reader.GetOrdinal(columnName);
                if (reader.IsDBNull(ordinal))
                    return string.Empty;

                DateTime date = reader.GetDateTime(ordinal);
                return date.ToString("yyyy-MM-dd");
            }
            catch
            {
                return string.Empty;
            }
        }

        private object GetTextOrNull(string text)
        {
            return string.IsNullOrWhiteSpace(text) ? (object)DBNull.Value : text.Trim();
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

        private void ShowSuccessMessage(string message)
        {
            pnlSuccessMessage.Visible = true;
            pnlMessages.Visible = false;
            // Update success message if needed
        }

        private void ShowErrorMessage(string message)
        {
            pnlMessages.Visible = true;
            pnlSuccessMessage.Visible = false;
            lblMessage.Text = message;
            lblMessage.CssClass = "message-text error";
        }
    }
}