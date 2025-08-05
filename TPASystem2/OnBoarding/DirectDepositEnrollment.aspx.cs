using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.OnBoarding
{
    public partial class DirectDepositEnrollment : System.Web.UI.Page
    {
        #region Properties and Fields

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private int CurrentEmployeeId
        {
            get
            {
                return GetCurrentEmployeeId();
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
                LoadEmployeeData();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (ManualValidateDirectDeposit())
                {
                    SaveDirectDepositData();
                    CompleteMandatoryTask();
                    ShowSuccessAndRedirect();
                }
                else
                {
                    ShowErrorMessage("Please complete all required fields including bank information and authorizations.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error submitting direct deposit: {ex.Message}");
                ShowErrorMessage("An error occurred while saving your direct deposit information. Please try again.");
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
            try
            {
                string userRole = Session["UserRole"]?.ToString() ?? "";
                if (!userRole.Equals("EMPLOYEE", StringComparison.OrdinalIgnoreCase))
                {
                    Response.Redirect("~/Dashboard.aspx");
                    return;
                }

                if (CurrentEmployeeId <= 0)
                {
                    ShowErrorMessage("Employee record not found. Please contact HR.");
                    return;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in InitializePage: {ex.Message}");
                Response.Redirect("~/Login.aspx");
            }
        }

        private void LoadEmployeeData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT e.FirstName, e.LastName, e.Email
                        FROM Employees e
                        WHERE e.Id = @EmployeeId", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                System.Diagnostics.Debug.WriteLine($"Loaded data for employee: {reader["FirstName"]} {reader["LastName"]}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading employee data: {ex.Message}");
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
                        // First, deactivate any existing direct deposit records for this employee
                        using (SqlCommand deactivateCmd = new SqlCommand(@"
                            UPDATE DirectDeposit 
                            SET IsActive = 0, LastUpdated = GETUTCDATE(), UpdatedById = @EmployeeId
                            WHERE EmployeeId = @EmployeeId AND IsActive = 1", conn, transaction))
                        {
                            deactivateCmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                            deactivateCmd.ExecuteNonQuery();
                        }

                        // Insert new direct deposit record
                        using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO DirectDeposit 
                            (EmployeeId, BankName, AccountType, RoutingNumber, AccountNumber, AccountNumberConfirm,
                             IsDirectDepositAuthorized, IsBankingAccuracyConfirmed, IsBankingPrivacyAccepted,
                             CreatedById, Notes, VerificationStatus)
                            VALUES 
                            (@EmployeeId, @BankName, @AccountType, @RoutingNumber, @AccountNumber, @AccountNumberConfirm,
                             @IsDirectDepositAuthorized, @IsBankingAccuracyConfirmed, @IsBankingPrivacyAccepted,
                             @CreatedById, @Notes, 'PENDING')", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                            cmd.Parameters.AddWithValue("@BankName", txtBankName.Text.Trim());
                            cmd.Parameters.AddWithValue("@AccountType", ddlAccountType.SelectedValue);
                            cmd.Parameters.AddWithValue("@RoutingNumber", txtRoutingNumber.Text.Trim());
                            cmd.Parameters.AddWithValue("@AccountNumber", txtAccountNumber.Text.Trim());
                            cmd.Parameters.AddWithValue("@AccountNumberConfirm", txtConfirmAccountNumber.Text.Trim());
                            cmd.Parameters.AddWithValue("@IsDirectDepositAuthorized", chkDirectDepositAuth.Checked);
                            cmd.Parameters.AddWithValue("@IsBankingAccuracyConfirmed", chkBankingAccuracy.Checked);
                            cmd.Parameters.AddWithValue("@IsBankingPrivacyAccepted", chkBankingPrivacy.Checked);
                            cmd.Parameters.AddWithValue("@CreatedById", CurrentEmployeeId);
                            cmd.Parameters.AddWithValue("@Notes", "Direct deposit enrollment completed via employee portal");

                            cmd.ExecuteNonQuery();
                        }

                        // Update employee record with basic info
                        using (SqlCommand cmd = new SqlCommand(@"
                            UPDATE Employees 
                            SET LastUpdated = GETUTCDATE()
                            WHERE Id = @EmployeeId", conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        System.Diagnostics.Debug.WriteLine("Direct deposit data saved successfully to database");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        System.Diagnostics.Debug.WriteLine($"Error in SaveDirectDepositData: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        private void CompleteMandatoryTask()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Mark the Direct Deposit task as completed
                    using (SqlCommand cmd = new SqlCommand(@"
                        UPDATE OnboardingTasks 
                        SET Status = 'COMPLETED',
                            CompletedDate = GETUTCDATE(),
                            CompletedById = @EmployeeId,
                            Notes = 'Completed through employee portal - Direct deposit enrollment',
                            LastUpdated = GETUTCDATE()
                        WHERE EmployeeId = @EmployeeId 
                          AND Category = 'DIRECT_DEPOSIT' 
                          AND Status = 'PENDING'", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Successfully completed direct deposit task for employee {CurrentEmployeeId}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"No pending direct deposit task found for employee {CurrentEmployeeId}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error completing mandatory task: {ex.Message}");
                // Don't throw error here as the data was saved successfully
            }
        }

        #endregion

        #region Helper Methods

        private System.Web.UI.Control FindControlRecursive(System.Web.UI.Control container, string controlId)
        {
            if (container.ID == controlId)
                return container;

            foreach (System.Web.UI.Control control in container.Controls)
            {
                var found = FindControlRecursive(control, controlId);
                if (found != null)
                    return found;
            }

            return null;
        }

        private bool ManualValidateDirectDeposit()
        {
            bool isValid = true;
            string errorMessage = "";

            try
            {
                // Check basic text fields
                if (string.IsNullOrWhiteSpace(txtBankName?.Text))
                {
                    isValid = false;
                    errorMessage += "Bank name is required. ";
                }

                if (string.IsNullOrEmpty(ddlAccountType?.SelectedValue))
                {
                    isValid = false;
                    errorMessage += "Account type is required. ";
                }

                if (string.IsNullOrWhiteSpace(txtRoutingNumber?.Text))
                {
                    isValid = false;
                    errorMessage += "Routing number is required. ";
                }
                else if (txtRoutingNumber.Text.Length != 9 || !System.Text.RegularExpressions.Regex.IsMatch(txtRoutingNumber.Text, @"^\d{9}$"))
                {
                    isValid = false;
                    errorMessage += "Routing number must be exactly 9 digits. ";
                }

                if (string.IsNullOrWhiteSpace(txtAccountNumber?.Text))
                {
                    isValid = false;
                    errorMessage += "Account number is required. ";
                }

                if (string.IsNullOrWhiteSpace(txtConfirmAccountNumber?.Text))
                {
                    isValid = false;
                    errorMessage += "Account number confirmation is required. ";
                }
                else if (txtAccountNumber?.Text != txtConfirmAccountNumber?.Text)
                {
                    isValid = false;
                    errorMessage += "Account numbers must match. ";
                }

                // Check required checkboxes
                if (!chkDirectDepositAuth?.Checked == true)
                {
                    isValid = false;
                    errorMessage += "Direct deposit authorization is required. ";
                }

                if (!chkBankingAccuracy?.Checked == true)
                {
                    isValid = false;
                    errorMessage += "Banking accuracy confirmation is required. ";
                }

                if (!chkBankingPrivacy?.Checked == true)
                {
                    isValid = false;
                    errorMessage += "Banking privacy acknowledgment is required. ";
                }

                if (!isValid)
                {
                    ShowErrorMessage(errorMessage.Trim());
                }

                return isValid;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ManualValidateDirectDeposit: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Utility Methods

        private int GetCurrentEmployeeId()
        {
            try
            {
                if (Session["UserId"] != null)
                {
                    int userId = Convert.ToInt32(Session["UserId"]);

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("SELECT Id FROM Employees WHERE UserId = @UserId", conn))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            object result = cmd.ExecuteScalar();
                            return result != null ? Convert.ToInt32(result) : 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting current employee ID: {ex.Message}");
            }

            return 0;
        }

        private void ShowErrorMessage(string message)
        {
            try
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert",
                    $"alert('Error: {message.Replace("'", "\\'")}');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing error message: {ex.Message}");
            }
        }

        private void ShowSuccessAndRedirect()
        {
            try
            {
                if (pnlSuccessMessage != null)
                {
                    pnlSuccessMessage.Visible = true;
                }

                ClientScript.RegisterStartupScript(this.GetType(), "redirect",
                    @"
                    alert('Direct deposit enrollment completed successfully!');
                    setTimeout(function() {
                        window.location.href = '" + ResolveUrl("~/OnBoarding/MyOnboarding.aspx") + @"';
                    }, 2000);", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ShowSuccessAndRedirect: {ex.Message}");
                Response.Redirect("~/OnBoarding/MyOnboarding.aspx");
            }
        }

        #endregion
    }
}