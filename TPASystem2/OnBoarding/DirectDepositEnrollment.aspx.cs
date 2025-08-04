using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace TPASystem2.OnBoarding
{
    public partial class DirectDepositEnrollment : System.Web.UI.Page
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
                LoadExistingDirectDepositData();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    // Validate deposit setup
                    if (!ValidateDepositSetup())
                    {
                        return;
                    }

                    SaveDirectDepositData();
                    CompleteMandatoryTask();
                    ShowSuccessAndRedirect();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error submitting direct deposit: {ex.Message}");
                    ShowErrorMessage("An error occurred while setting up your direct deposit. Please try again.");
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
            if (!IsDirectDepositTaskPending())
            {
                Response.Redirect("~/OnBoarding/MyOnboarding.aspx");
                return;
            }

            // Set default values
            rbFullDeposit.Checked = true;
            ddlPrimaryAmountType.SelectedValue = "remainder";
           // txtDependents.Text = "0";
        }

        #endregion

        #region Data Loading Methods

        private void LoadExistingDirectDepositData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Check if direct deposit data already exists
                    string query = @"
                        SELECT 
                            DepositType,
                            PrimaryBankName,
                            PrimaryAccountType,
                            PrimaryRoutingNumber,
                            PrimaryAccountNumber
                        FROM [dbo].[EmployeeDirectDeposit]
                        WHERE EmployeeId = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Pre-populate fields with existing data
                                string depositType = reader["DepositType"].ToString();
                                if (depositType == "SPLIT")
                                {
                                    rbSplitDeposit.Checked = true;
                                    rbFullDeposit.Checked = false;
                                    pnlPrimaryAmount.Visible = true;
                                    pnlSecondaryAccount.Visible = true;
                                }

                                txtBankName.Text = reader["PrimaryBankName"].ToString();
                                ddlAccountType.SelectedValue = reader["PrimaryAccountType"].ToString();
                                txtRoutingNumber.Text = reader["PrimaryRoutingNumber"].ToString();
                                // Don't pre-populate account numbers for security
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading direct deposit data: {ex.Message}");
                // Don't show error for pre-population failure, just leave fields empty
            }
        }

        #endregion

        #region Data Saving Methods

        private void SaveDirectDepositData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Create direct deposit table if it doesn't exist
                        CreateDirectDepositTable(conn, transaction);

                        // Save direct deposit information
                        SaveDirectDepositInfo(conn, transaction);

                        // Save authorization records
                        SaveDirectDepositAuthorizations(conn, transaction);

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

        private void CreateDirectDepositTable(SqlConnection conn, SqlTransaction transaction)
        {
            string createTableQuery = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmployeeDirectDeposit' AND xtype='U')
                CREATE TABLE [dbo].[EmployeeDirectDeposit] (
                    [Id] [int] IDENTITY(1,1) NOT NULL,
                    [EmployeeId] [int] NOT NULL,
                    [DepositType] [nvarchar](20) NOT NULL,
                    [PrimaryBankName] [nvarchar](100) NOT NULL,
                    [PrimaryAccountType] [nvarchar](20) NOT NULL,
                    [PrimaryRoutingNumber] [nvarchar](9) NOT NULL,
                    [PrimaryAccountNumber] [nvarchar](20) NOT NULL,
                    [PrimaryAmountType] [nvarchar](20) NULL,
                    [PrimaryAmount] [decimal](10,2) NULL,
                    [SecondaryBankName] [nvarchar](100) NULL,
                    [SecondaryAccountType] [nvarchar](20) NULL,
                    [SecondaryRoutingNumber] [nvarchar](9) NULL,
                    [SecondaryAccountNumber] [nvarchar](20) NULL,
                    [SecondaryAmountType] [nvarchar](20) NULL,
                    [SecondaryAmount] [decimal](10,2) NULL,
                    [IsActive] [bit] NOT NULL DEFAULT(1),
                    [CreatedAt] [datetime2](7) NOT NULL DEFAULT(GETUTCDATE()),
                    [UpdatedAt] [datetime2](7) NOT NULL DEFAULT(GETUTCDATE()),
                    CONSTRAINT [PK_EmployeeDirectDeposit] PRIMARY KEY CLUSTERED ([Id] ASC),
                    CONSTRAINT [FK_EmployeeDirectDeposit_Employees] FOREIGN KEY([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE CASCADE
                )";

            using (SqlCommand createCmd = new SqlCommand(createTableQuery, conn, transaction))
            {
                createCmd.ExecuteNonQuery();
            }
        }

        private void SaveDirectDepositInfo(SqlConnection conn, SqlTransaction transaction)
        {
            // Delete existing direct deposit info
            string deleteQuery = "DELETE FROM [dbo].[EmployeeDirectDeposit] WHERE EmployeeId = @EmployeeId";
            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                deleteCmd.ExecuteNonQuery();
            }

            // Determine deposit type
            string depositType = rbFullDeposit.Checked ? "FULL" : "SPLIT";

            // Insert new direct deposit information
            string insertQuery = @"
                INSERT INTO [dbo].[EmployeeDirectDeposit]
                (EmployeeId, DepositType, PrimaryBankName, PrimaryAccountType, PrimaryRoutingNumber, PrimaryAccountNumber,
                 PrimaryAmountType, PrimaryAmount, SecondaryBankName, SecondaryAccountType, SecondaryRoutingNumber, 
                 SecondaryAccountNumber, SecondaryAmountType, SecondaryAmount, IsActive, CreatedAt, UpdatedAt)
                VALUES (@EmployeeId, @DepositType, @PrimaryBankName, @PrimaryAccountType, @PrimaryRoutingNumber, @PrimaryAccountNumber,
                        @PrimaryAmountType, @PrimaryAmount, @SecondaryBankName, @SecondaryAccountType, @SecondaryRoutingNumber,
                        @SecondaryAccountNumber, @SecondaryAmountType, @SecondaryAmount, 1, GETUTCDATE(), GETUTCDATE())";

            using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                cmd.Parameters.AddWithValue("@DepositType", depositType);
                cmd.Parameters.AddWithValue("@PrimaryBankName", txtBankName.Text.Trim());
                cmd.Parameters.AddWithValue("@PrimaryAccountType", ddlAccountType.SelectedValue);
                cmd.Parameters.AddWithValue("@PrimaryRoutingNumber", txtRoutingNumber.Text.Trim());
                cmd.Parameters.AddWithValue("@PrimaryAccountNumber", EncryptAccountNumber(txtAccountNumber.Text.Trim()));

                // Primary amount info (for split deposits)
                if (rbSplitDeposit.Checked)
                {
                    cmd.Parameters.AddWithValue("@PrimaryAmountType", ddlPrimaryAmountType.SelectedValue);

                    if (ddlPrimaryAmountType.SelectedValue != "remainder" && !string.IsNullOrEmpty(txtPrimaryAmount.Text))
                    {
                        decimal primaryAmount;
                        if (decimal.TryParse(txtPrimaryAmount.Text, out primaryAmount))
                        {
                            cmd.Parameters.AddWithValue("@PrimaryAmount", primaryAmount);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@PrimaryAmount", DBNull.Value);
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@PrimaryAmount", DBNull.Value);
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@PrimaryAmountType", "full");
                    cmd.Parameters.AddWithValue("@PrimaryAmount", DBNull.Value);
                }

                // Secondary account info (for split deposits)
                if (rbSplitDeposit.Checked && !string.IsNullOrEmpty(txtSecondaryBankName.Text))
                {
                    cmd.Parameters.AddWithValue("@SecondaryBankName", txtSecondaryBankName.Text.Trim());
                    cmd.Parameters.AddWithValue("@SecondaryAccountType", ddlSecondaryAccountType.SelectedValue);
                    cmd.Parameters.AddWithValue("@SecondaryRoutingNumber", txtSecondaryRoutingNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@SecondaryAccountNumber", EncryptAccountNumber(txtSecondaryAccountNumber.Text.Trim()));
                    cmd.Parameters.AddWithValue("@SecondaryAmountType", ddlSecondaryAmountType.SelectedValue);

                    if (!string.IsNullOrEmpty(txtSecondaryAmount.Text))
                    {
                        decimal secondaryAmount;
                        if (decimal.TryParse(txtSecondaryAmount.Text, out secondaryAmount))
                        {
                            cmd.Parameters.AddWithValue("@SecondaryAmount", secondaryAmount);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@SecondaryAmount", DBNull.Value);
                        }
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@SecondaryAmount", DBNull.Value);
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@SecondaryBankName", DBNull.Value);
                    cmd.Parameters.AddWithValue("@SecondaryAccountType", DBNull.Value);
                    cmd.Parameters.AddWithValue("@SecondaryRoutingNumber", DBNull.Value);
                    cmd.Parameters.AddWithValue("@SecondaryAccountNumber", DBNull.Value);
                    cmd.Parameters.AddWithValue("@SecondaryAmountType", DBNull.Value);
                    cmd.Parameters.AddWithValue("@SecondaryAmount", DBNull.Value);
                }

                cmd.ExecuteNonQuery();
            }
        }

        private void SaveDirectDepositAuthorizations(SqlConnection conn, SqlTransaction transaction)
        {
            // Create authorizations table if it doesn't exist
            string createTableQuery = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EmployeeDirectDepositAuthorizations' AND xtype='U')
                CREATE TABLE [dbo].[EmployeeDirectDepositAuthorizations] (
                    [Id] [int] IDENTITY(1,1) NOT NULL,
                    [EmployeeId] [int] NOT NULL,
                    [AuthorizationType] [nvarchar](50) NOT NULL,
                    [Authorized] [bit] NOT NULL DEFAULT(0),
                    [AuthorizedDate] [datetime2](7) NOT NULL,
                    [CreatedAt] [datetime2](7) NOT NULL DEFAULT(GETUTCDATE()),
                    CONSTRAINT [PK_EmployeeDirectDepositAuthorizations] PRIMARY KEY CLUSTERED ([Id] ASC),
                    CONSTRAINT [FK_EmployeeDirectDepositAuthorizations_Employees] FOREIGN KEY([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE CASCADE
                )";

            using (SqlCommand createCmd = new SqlCommand(createTableQuery, conn, transaction))
            {
                createCmd.ExecuteNonQuery();
            }

            // Delete existing authorizations
            string deleteQuery = "DELETE FROM [dbo].[EmployeeDirectDepositAuthorizations] WHERE EmployeeId = @EmployeeId";
            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
            {
                deleteCmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                deleteCmd.ExecuteNonQuery();
            }

            // Insert authorizations
            string[] authorizationTypes = { "DirectDepositAuthorization", "DataAccuracy", "PrivacyConsent" };
            bool[] authorizationValues = { chkAuthorization.Checked, chkDataAccuracy.Checked, chkPrivacyConsent.Checked };

            for (int i = 0; i < authorizationTypes.Length; i++)
            {
                string insertQuery = @"
                    INSERT INTO [dbo].[EmployeeDirectDepositAuthorizations]
                    (EmployeeId, AuthorizationType, Authorized, AuthorizedDate, CreatedAt)
                    VALUES (@EmployeeId, @AuthorizationType, @Authorized, GETUTCDATE(), GETUTCDATE())";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                    cmd.Parameters.AddWithValue("@AuthorizationType", authorizationTypes[i]);
                    cmd.Parameters.AddWithValue("@Authorized", authorizationValues[i]);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void CompleteMandatoryTask()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Find the Direct Deposit mandatory task
                string findTaskQuery = @"
                    SELECT Id FROM [dbo].[OnboardingTasks] 
                    WHERE EmployeeId = @EmployeeId 
                        AND IsMandatory = 1 
                        AND Category = 'DIRECT_DEPOSIT' 
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
                        cmd.Parameters.AddWithValue("@Notes", "Direct deposit enrollment completed through employee portal");

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

        #region Validation Methods

        private bool ValidateDepositSetup()
        {
            bool isValid = true;

            // Validate primary account information
            if (string.IsNullOrEmpty(txtBankName.Text.Trim()))
            {
                ShowErrorMessage("Bank name is required.");
                return false;
            }

            if (string.IsNullOrEmpty(ddlAccountType.SelectedValue))
            {
                ShowErrorMessage("Account type is required.");
                return false;
            }

            if (string.IsNullOrEmpty(txtRoutingNumber.Text.Trim()) || txtRoutingNumber.Text.Trim().Length != 9)
            {
                ShowErrorMessage("A valid 9-digit routing number is required.");
                return false;
            }

            if (string.IsNullOrEmpty(txtAccountNumber.Text.Trim()))
            {
                ShowErrorMessage("Account number is required.");
                return false;
            }

            if (txtAccountNumber.Text.Trim() != txtConfirmAccountNumber.Text.Trim())
            {
                ShowErrorMessage("Account numbers do not match.");
                return false;
            }

            // Validate split deposit setup if selected
            if (rbSplitDeposit.Checked)
            {
                if (!string.IsNullOrEmpty(txtSecondaryBankName.Text.Trim()))
                {
                    // If secondary bank is provided, validate all secondary fields
                    if (string.IsNullOrEmpty(ddlSecondaryAccountType.SelectedValue) ||
                        string.IsNullOrEmpty(txtSecondaryRoutingNumber.Text.Trim()) ||
                        string.IsNullOrEmpty(txtSecondaryAccountNumber.Text.Trim()))
                    {
                        ShowErrorMessage("If you provide a secondary bank, all secondary account fields are required.");
                        return false;
                    }

                    if (txtSecondaryRoutingNumber.Text.Trim().Length != 9)
                    {
                        ShowErrorMessage("Secondary routing number must be exactly 9 digits.");
                        return false;
                    }

                    // Validate secondary amount
                    if (string.IsNullOrEmpty(txtSecondaryAmount.Text.Trim()))
                    {
                        ShowErrorMessage("Secondary account deposit amount is required.");
                        return false;
                    }

                    decimal secondaryAmount;
                    if (!decimal.TryParse(txtSecondaryAmount.Text.Trim(), out secondaryAmount) || secondaryAmount <= 0)
                    {
                        ShowErrorMessage("Secondary account deposit amount must be a valid positive number.");
                        return false;
                    }

                    // Validate percentage doesn't exceed 100%
                    if (ddlSecondaryAmountType.SelectedValue == "percentage" && secondaryAmount > 100)
                    {
                        ShowErrorMessage("Secondary account percentage cannot exceed 100%.");
                        return false;
                    }
                }
            }

            // Validate authorizations
            if (!chkAuthorization.Checked)
            {
                ShowErrorMessage("You must authorize direct deposit to continue.");
                return false;
            }

            if (!chkDataAccuracy.Checked)
            {
                ShowErrorMessage("You must certify the accuracy of your banking information.");
                return false;
            }

            if (!chkPrivacyConsent.Checked)
            {
                ShowErrorMessage("You must consent to the privacy and security terms.");
                return false;
            }

            return isValid;
        }

        private bool IsDirectDepositTaskPending()
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
                            AND Category = 'DIRECT_DEPOSIT' 
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

        #endregion

        #region Helper Methods

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

        private string EncryptAccountNumber(string accountNumber)
        {
            // For now, we'll store the account number as-is
            // In a production environment, you should implement proper encryption
            // using System.Security.Cryptography or similar
            return accountNumber;
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