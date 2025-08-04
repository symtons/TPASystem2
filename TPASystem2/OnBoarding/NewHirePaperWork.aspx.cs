using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

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
            if (!IsPostBack)
            {
                // Set UnobtrusiveValidationMode to prevent validation errors
                Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

                InitializePage();
                LoadEmployeeData();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    SavePaperworkData();
                    CompleteMandatoryTask();
                    ShowSuccessAndRedirect();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error submitting paperwork: {ex.Message}");
                    ShowErrorMessage("An error occurred while saving your paperwork. Please try again.");
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/OnBoarding/MyOnboarding.aspx");
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

            // Check if this task is still pending
            if (!IsNewHirePaperworkTaskPending())
            {
                Response.Redirect("~/OnBoarding/MyOnboarding.aspx");
                return;
            }
        }

        #endregion

        #region Data Loading Methods

        private void LoadEmployeeData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            e.FirstName,
                            e.LastName,
                            e.Email,
                            e.PhoneNumber,
                            e.Address,
                            e.City,
                            e.State,
                            e.ZipCode,
                            e.DateOfBirth,
                            e.Gender
                        FROM [dbo].[Employees] e
                        WHERE e.Id = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Pre-populate fields with existing data
                                txtFirstName.Text = reader["FirstName"].ToString();
                                txtLastName.Text = reader["LastName"].ToString();
                                txtPersonalEmail.Text = reader["Email"].ToString();
                                txtPhoneNumber.Text = reader["PhoneNumber"].ToString();
                                txtAddress.Text = reader["Address"].ToString();
                                txtCity.Text = reader["City"].ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("State")))
                                    ddlState.SelectedValue = reader["State"].ToString();

                                txtZipCode.Text = reader["ZipCode"].ToString();

                                if (!reader.IsDBNull(reader.GetOrdinal("DateOfBirth")))
                                    txtDateOfBirth.Text = Convert.ToDateTime(reader["DateOfBirth"]).ToString("yyyy-MM-dd");

                                if (!reader.IsDBNull(reader.GetOrdinal("Gender")))
                                    ddlGender.SelectedValue = reader["Gender"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading employee data: {ex.Message}");
                // Don't show error for pre-population failure, just leave fields empty
            }
        }

        #endregion

        #region Data Saving Methods

        private void SavePaperworkData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Update Employee table with personal information
                        UpdateEmployeePersonalInfo(conn, transaction);

                        // Save emergency contact information
                        SaveEmergencyContact(conn, transaction);

                        // Save tax information (W-4 data)
                        SaveTaxInformation(conn, transaction);

                        // Save I-9 information
                        SaveI9Information(conn, transaction);

                        // Save acknowledgments
                        SaveAcknowledgments(conn, transaction);

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private void UpdateEmployeePersonalInfo(SqlConnection conn, SqlTransaction transaction)
        {
            string query = @"
                UPDATE [dbo].[Employees]
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @PersonalEmail,
                    PhoneNumber = @PhoneNumber,
                    Address = @Address,
                    City = @City,
                    State = @State,
                    ZipCode = @ZipCode,
                    DateOfBirth = @DateOfBirth,
                    Gender = @Gender,
                    UpdatedAt = GETUTCDATE()
                WHERE Id = @EmployeeId";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                cmd.Parameters.AddWithValue("@PersonalEmail", txtPersonalEmail.Text.Trim());
                cmd.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text.Trim());
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                cmd.Parameters.AddWithValue("@City", txtCity.Text.Trim());
                cmd.Parameters.AddWithValue("@State", string.IsNullOrEmpty(ddlState.SelectedValue) ? (object)DBNull.Value : ddlState.SelectedValue);
                cmd.Parameters.AddWithValue("@ZipCode", txtZipCode.Text.Trim());

                // Fix for DateTime parsing
                if (string.IsNullOrEmpty(txtDateOfBirth.Text))
                {
                    cmd.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);
                }
                else
                {
                    DateTime dateOfBirth;
                    if (DateTime.TryParse(txtDateOfBirth.Text, out dateOfBirth))
                    {
                        cmd.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);
                    }
                }

                cmd.Parameters.AddWithValue("@Gender", string.IsNullOrEmpty(ddlGender.SelectedValue) ? (object)DBNull.Value : ddlGender.SelectedValue);
                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                cmd.ExecuteNonQuery();
            }
        }

        private void SaveEmergencyContact(SqlConnection conn, SqlTransaction transaction)
        {
            // First, check if emergency contact table exists, if not create it
            string createTableQuery = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmployeeEmergencyContacts' AND xtype='U')
                CREATE TABLE [dbo].[EmployeeEmergencyContacts] (
                    [Id] [int] IDENTITY(1,1) NOT NULL,
                    [EmployeeId] [int] NOT NULL,
                    [ContactName] [nvarchar](100) NOT NULL,
                    [Relationship] [nvarchar](50) NOT NULL,
                    [PhoneNumber] [nvarchar](20) NOT NULL,
                    [Email] [nvarchar](100) NULL,
                    [IsPrimary] [bit] NOT NULL DEFAULT(1),
                    [CreatedAt] [datetime2](7) NOT NULL DEFAULT(GETUTCDATE()),
                    [UpdatedAt] [datetime2](7) NOT NULL DEFAULT(GETUTCDATE()),
                    CONSTRAINT [PK_EmployeeEmergencyContacts] PRIMARY KEY CLUSTERED ([Id] ASC),
                    CONSTRAINT [FK_EmployeeEmergencyContacts_Employees] FOREIGN KEY([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE CASCADE
                )";

            using (SqlCommand createCmd = new SqlCommand(createTableQuery, conn, transaction))
            {
                createCmd.ExecuteNonQuery();
            }

            // Delete existing emergency contacts for this employee
            string deleteQuery = "DELETE FROM [dbo].[EmployeeEmergencyContacts] WHERE EmployeeId = @EmployeeId";
            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                deleteCmd.ExecuteNonQuery();
            }

            // Insert new emergency contact
            string insertQuery = @"
                INSERT INTO [dbo].[EmployeeEmergencyContacts]
                (EmployeeId, ContactName, Relationship, PhoneNumber, Email, IsPrimary, CreatedAt, UpdatedAt)
                VALUES (@EmployeeId, @ContactName, @Relationship, @PhoneNumber, @Email, 1, GETUTCDATE(), GETUTCDATE())";

            using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                cmd.Parameters.AddWithValue("@ContactName", txtEmergencyContactName.Text.Trim());
                cmd.Parameters.AddWithValue("@Relationship", txtEmergencyContactRelationship.Text.Trim());
                cmd.Parameters.AddWithValue("@PhoneNumber", txtEmergencyContactPhone.Text.Trim());
                cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(txtEmergencyContactEmail.Text) ? (object)DBNull.Value : txtEmergencyContactEmail.Text.Trim());

                cmd.ExecuteNonQuery();
            }
        }

        private void SaveTaxInformation(SqlConnection conn, SqlTransaction transaction)
        {
            // Create tax information table if it doesn't exist
            string createTableQuery = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmployeeTaxInformation' AND xtype='U')
                CREATE TABLE [dbo].[EmployeeTaxInformation] (
                    [Id] [int] IDENTITY(1,1) NOT NULL,
                    [EmployeeId] [int] NOT NULL,
                    [FilingStatus] [nvarchar](50) NOT NULL,
                    [Dependents] [int] NOT NULL DEFAULT(0),
                    [AdditionalWithholding] [decimal](10,2) NULL,
                    [TaxExempt] [bit] NOT NULL DEFAULT(0),
                    [TaxYear] [int] NOT NULL,
                    [CreatedAt] [datetime2](7) NOT NULL DEFAULT(GETUTCDATE()),
                    [UpdatedAt] [datetime2](7) NOT NULL DEFAULT(GETUTCDATE()),
                    CONSTRAINT [PK_EmployeeTaxInformation] PRIMARY KEY CLUSTERED ([Id] ASC),
                    CONSTRAINT [FK_EmployeeTaxInformation_Employees] FOREIGN KEY([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE CASCADE
                )";

            using (SqlCommand createCmd = new SqlCommand(createTableQuery, conn, transaction))
            {
                createCmd.ExecuteNonQuery();
            }

            // Delete existing tax info for current year
            int currentYear = DateTime.Now.Year;
            string deleteQuery = "DELETE FROM [dbo].[EmployeeTaxInformation] WHERE EmployeeId = @EmployeeId AND TaxYear = @TaxYear";
            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                deleteCmd.Parameters.AddWithValue("@TaxYear", currentYear);
                deleteCmd.ExecuteNonQuery();
            }

            // Insert new tax information
            string insertQuery = @"
                INSERT INTO [dbo].[EmployeeTaxInformation]
                (EmployeeId, FilingStatus, Dependents, AdditionalWithholding, TaxExempt, TaxYear, CreatedAt, UpdatedAt)
                VALUES (@EmployeeId, @FilingStatus, @Dependents, @AdditionalWithholding, @TaxExempt, @TaxYear, GETUTCDATE(), GETUTCDATE())";

            using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                cmd.Parameters.AddWithValue("@FilingStatus", ddlFilingStatus.SelectedValue);

                // Fix for int parsing
                int dependents = 0;
                if (!string.IsNullOrEmpty(txtDependents.Text) && int.TryParse(txtDependents.Text, out dependents))
                {
                    cmd.Parameters.AddWithValue("@Dependents", dependents);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Dependents", 0);
                }

                // Fix for decimal parsing
                if (string.IsNullOrEmpty(txtAdditionalWithholding.Text))
                {
                    cmd.Parameters.AddWithValue("@AdditionalWithholding", DBNull.Value);
                }
                else
                {
                    decimal additionalWithholding;
                    if (decimal.TryParse(txtAdditionalWithholding.Text, out additionalWithholding))
                    {
                        cmd.Parameters.AddWithValue("@AdditionalWithholding", additionalWithholding);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@AdditionalWithholding", DBNull.Value);
                    }
                }

                cmd.Parameters.AddWithValue("@TaxExempt", chkTaxExempt.Checked);
                cmd.Parameters.AddWithValue("@TaxYear", currentYear);

                cmd.ExecuteNonQuery();
            }
        }

        private void SaveI9Information(SqlConnection conn, SqlTransaction transaction)
        {
            // Create I-9 information table if it doesn't exist
            string createTableQuery = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmployeeI9Information' AND xtype='U')
                CREATE TABLE [dbo].[EmployeeI9Information] (
                    [Id] [int] IDENTITY(1,1) NOT NULL,
                    [EmployeeId] [int] NOT NULL,
                    [WorkAuthorizationStatus] [nvarchar](50) NOT NULL,
                    [AttestationCompleted] [bit] NOT NULL DEFAULT(0),
                    [AttestationDate] [datetime2](7) NOT NULL,
                    [CreatedAt] [datetime2](7) NOT NULL DEFAULT(GETUTCDATE()),
                    [UpdatedAt] [datetime2](7) NOT NULL DEFAULT(GETUTCDATE()),
                    CONSTRAINT [PK_EmployeeI9Information] PRIMARY KEY CLUSTERED ([Id] ASC),
                    CONSTRAINT [FK_EmployeeI9Information_Employees] FOREIGN KEY([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE CASCADE
                )";

            using (SqlCommand createCmd = new SqlCommand(createTableQuery, conn, transaction))
            {
                createCmd.ExecuteNonQuery();
            }

            // Delete existing I-9 info
            string deleteQuery = "DELETE FROM [dbo].[EmployeeI9Information] WHERE EmployeeId = @EmployeeId";
            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                deleteCmd.ExecuteNonQuery();
            }

            // Insert new I-9 information
            string insertQuery = @"
                INSERT INTO [dbo].[EmployeeI9Information]
                (EmployeeId, WorkAuthorizationStatus, AttestationCompleted, AttestationDate, CreatedAt, UpdatedAt)
                VALUES (@EmployeeId, @WorkAuthorizationStatus, @AttestationCompleted, GETUTCDATE(), GETUTCDATE(), GETUTCDATE())";

            using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                cmd.Parameters.AddWithValue("@WorkAuthorizationStatus", ddlWorkAuthorization.SelectedValue);
                cmd.Parameters.AddWithValue("@AttestationCompleted", chkI9Attestation.Checked);

                cmd.ExecuteNonQuery();
            }
        }

        private void SaveAcknowledgments(SqlConnection conn, SqlTransaction transaction)
        {
            // Create acknowledgments table if it doesn't exist
            string createTableQuery = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmployeeAcknowledgments' AND xtype='U')
                CREATE TABLE [dbo].[EmployeeAcknowledgments] (
                    [Id] [int] IDENTITY(1,1) NOT NULL,
                    [EmployeeId] [int] NOT NULL,
                    [AcknowledgmentType] [nvarchar](50) NOT NULL,
                    [Acknowledged] [bit] NOT NULL DEFAULT(0),
                    [AcknowledgedDate] [datetime2](7) NOT NULL,
                    [CreatedAt] [datetime2](7) NOT NULL DEFAULT(GETUTCDATE()),
                    CONSTRAINT [PK_EmployeeAcknowledgments] PRIMARY KEY CLUSTERED ([Id] ASC),
                    CONSTRAINT [FK_EmployeeAcknowledgments_Employees] FOREIGN KEY([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE CASCADE
                )";

            using (SqlCommand createCmd = new SqlCommand(createTableQuery, conn, transaction))
            {
                createCmd.ExecuteNonQuery();
            }

            // Delete existing acknowledgments
            string deleteQuery = "DELETE FROM [dbo].[EmployeeAcknowledgments] WHERE EmployeeId = @EmployeeId";
            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                deleteCmd.ExecuteNonQuery();
            }

            // Insert acknowledgments
            string[] acknowledgmentTypes = { "EmployeeHandbook", "DataAccuracy", "PrivacyConsent" };
            bool[] acknowledgmentValues = { chkEmployeeHandbook.Checked, chkDataAccuracy.Checked, chkPrivacyConsent.Checked };

            for (int i = 0; i < acknowledgmentTypes.Length; i++)
            {
                string insertQuery = @"
                    INSERT INTO [dbo].[EmployeeAcknowledgments]
                    (EmployeeId, AcknowledgmentType, Acknowledged, AcknowledgedDate, CreatedAt)
                    VALUES (@EmployeeId, @AcknowledgmentType, @Acknowledged, GETUTCDATE(), GETUTCDATE())";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                    cmd.Parameters.AddWithValue("@AcknowledgmentType", acknowledgmentTypes[i]);
                    cmd.Parameters.AddWithValue("@Acknowledged", acknowledgmentValues[i]);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void CompleteMandatoryTask()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Find the New Hire Paperwork mandatory task
                string findTaskQuery = @"
                    SELECT Id FROM [dbo].[OnboardingTasks] 
                    WHERE EmployeeId = @EmployeeId 
                        AND IsMandatory = 1 
                        AND Category = 'NEW_HIRE_PAPERWORK' 
                        AND Status != 'COMPLETED'
                        AND IsTemplate = 0";

                int taskId = 0;
                using (SqlCommand findCmd = new SqlCommand(findTaskQuery, conn))
                {
                    findCmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                    var result = findCmd.ExecuteScalar();
                    if (result != null)
                        taskId = Convert.ToInt32(result);
                }

                if (taskId > 0)
                {
                    // Complete the mandatory task
                    using (SqlCommand cmd = new SqlCommand("sp_CompleteMandatoryTask", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@TaskId", taskId);
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        cmd.Parameters.AddWithValue("@CompletedById", CurrentEmployeeId);
                        cmd.Parameters.AddWithValue("@Notes", "New hire paperwork completed through employee portal");

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string status = reader["Status"].ToString();
                                if (status != "SUCCESS")
                                {
                                    throw new Exception("Failed to complete mandatory task: " + reader["Message"].ToString());
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Validation and Helper Methods

        private bool IsNewHirePaperworkTaskPending()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT COUNT(*) FROM [dbo].[OnboardingTasks] 
                        WHERE EmployeeId = @EmployeeId 
                            AND IsMandatory = 1 
                            AND Category = 'NEW_HIRE_PAPERWORK' 
                            AND Status = 'PENDING'
                            AND IsTemplate = 0";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking task status: {ex.Message}");
                return false;
            }
        }

        private int GetCurrentEmployeeId()
        {
            try
            {
                if (Session["UserId"] == null) return 0;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id FROM [dbo].[Employees] WHERE UserId = @UserId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", Session["UserId"]);
                        var result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting current employee ID: {ex.Message}");
                return 0;
            }
        }

        private void ShowSuccessAndRedirect()
        {
            pnlSuccessMessage.Visible = true;

            string script = @"
                setTimeout(function() {
                    window.location.href = '" + ResolveUrl("~/OnBoarding/MyOnboarding.aspx") + @"';
                }, 3000);";

            ClientScript.RegisterStartupScript(this.GetType(), "RedirectAfterSuccess", script, true);
        }

        private void ShowErrorMessage(string message)
        {
            string script = $@"
                alert('Error: {message.Replace("'", "\\'")}');";

            ClientScript.RegisterStartupScript(this.GetType(), "ShowError", script, true);
        }

        #endregion
    }
}