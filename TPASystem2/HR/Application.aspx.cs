using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace TPASystem2.HR
{
    public partial class Application : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializePage();
                LoadApplications();
                LoadPositions();
            }
        }

        #region Page Initialization

        private void InitializePage()
        {
            // Set current user info
            if (Session["UserName"] != null)
            {
                litCurrentUser.Text = Session["UserName"].ToString();
            }

            if (Session["Department"] != null)
            {
                litDepartment.Text = Session["Department"].ToString();
            }

            litCurrentDate.Text = DateTime.Now.ToString("MMMM dd, yyyy");

            // Check user permissions - commented out for testing
            //if (!HasHRPermissions())
            //{
            //    ShowMessage("Access denied. You do not have permission to view this page.", "error");
            //    Response.Redirect("~/Dashboard.aspx");
            //    return;
            //}
        }

        private bool HasHRPermissions()
        {
            if (Session["Role"] != null)
            {
                string role = Session["Role"].ToString();
                return role == "HRAdmin" || role == "Admin";
            }
            return false;
        }

        private void LoadPositions()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"
                        SELECT DISTINCT Position1 as Position
                        FROM JobApplications 
                        WHERE Position1 IS NOT NULL AND Position1 != ''
                        UNION
                        SELECT DISTINCT Position2 as Position
                        FROM JobApplications 
                        WHERE Position2 IS NOT NULL AND Position2 != ''
                        ORDER BY Position";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            ddlPosition.Items.Add(new ListItem(reader["Position"].ToString(), reader["Position"].ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading positions: " + ex.Message, "error");
            }
        }

        #endregion

        #region Data Loading

        private void LoadApplications()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    StringBuilder sql = new StringBuilder(@"
                        SELECT 
                            ApplicationId, ApplicationNumber, Status,
                            FirstName, MiddleName, LastName, 
                            HomePhone, CellPhone, 
                            Position1, Position2, SalaryDesired, SalaryType,
                            EmploymentType, AvailableStartDate,
                            CreatedDate, LastModified, SubmissionDate, IsSubmitted
                        FROM JobApplications
                        WHERE 1=1");

                    List<SqlParameter> parameters = new List<SqlParameter>();

                    // Apply filters
                    ApplyFilters(sql, parameters);

                    sql.Append(" ORDER BY CreatedDate DESC, LastModified DESC");

                    using (SqlCommand cmd = new SqlCommand(sql.ToString(), conn))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        gvApplications.DataSource = dt;
                        gvApplications.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading applications: " + ex.Message, "error");
            }
        }

        private void ApplyFilters(StringBuilder sql, List<SqlParameter> parameters)
        {
            // Search filter
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                sql.Append(@" AND (
                    FirstName LIKE @Search OR 
                    LastName LIKE @Search OR 
                    ApplicationNumber LIKE @Search OR
                    HomePhone LIKE @Search OR
                    CellPhone LIKE @Search
                )");
                parameters.Add(new SqlParameter("@Search", "%" + txtSearch.Text.Trim() + "%"));
            }

            // Status filter
            if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
            {
                sql.Append(" AND Status = @Status");
                parameters.Add(new SqlParameter("@Status", ddlStatus.SelectedValue));
            }

            // Position filter
            if (!string.IsNullOrEmpty(ddlPosition.SelectedValue))
            {
                sql.Append(" AND (Position1 = @Position OR Position2 = @Position)");
                parameters.Add(new SqlParameter("@Position", ddlPosition.SelectedValue));
            }
        }

        #endregion

        #region Grid Events

        protected void gvApplications_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvApplications.PageIndex = e.NewPageIndex;
            LoadApplications();
        }

        protected void gvApplications_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int applicationId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ViewApplication":
                    ViewApplication(applicationId);
                    break;
                case "EditApplication":
                    EditApplicationStatus(applicationId);
                    break;
                case "DeleteApplication":
                    DeleteApplication(applicationId);
                    break;
            }
        }

        protected void gvApplications_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // You can add any row-specific logic here if needed
            }
        }

        #endregion

        #region Application Actions

        private void ViewApplication(int applicationId)
        {
            try
            {
                LoadApplicationDetails(applicationId);
                LoadEmploymentHistory(applicationId);

                hdnSelectedApplicationId.Value = applicationId.ToString();

                // Show modal via JavaScript
                ScriptManager.RegisterStartupScript(this, GetType(), "showApplicationModal",
                    "showApplicationModal();", true);
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading application details: " + ex.Message, "error");
            }
        }

        private void EditApplicationStatus(int applicationId)
        {
            try
            {
                // Load current status
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = "SELECT Status FROM JobApplications WHERE ApplicationId = @Id";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", applicationId);
                        conn.Open();

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            string currentStatus = result.ToString();
                            lblCurrentStatus.Text = GetStatusDisplayText(currentStatus);
                            hdnSelectedApplicationId.Value = applicationId.ToString();

                            // Show status modal
                            ScriptManager.RegisterStartupScript(this, GetType(), "showStatusModal",
                                "showStatusModal();", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error loading application status: " + ex.Message, "error");
            }
        }

        private void DeleteApplication(int applicationId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Delete main application record from JobApplications table
                        string deleteMainSql = "DELETE FROM JobApplications WHERE ApplicationId = @Id";
                        using (SqlCommand cmd = new SqlCommand(deleteMainSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", applicationId);
                            int affected = cmd.ExecuteNonQuery();

                            if (affected > 0)
                            {
                                transaction.Commit();
                                ShowMessage("Application deleted successfully.", "success");
                                LoadApplications();
                            }
                            else
                            {
                                transaction.Rollback();
                                ShowMessage("Application not found or could not be deleted.", "error");
                            }
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error deleting application: " + ex.Message, "error");
            }
        }

        #endregion

        #region Modal Data Loading

        private void LoadApplicationDetails(int applicationId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        FirstName, MiddleName, LastName,
                        HomeAddress, AptNumber, City, State, Zip,
                        HomePhone, CellPhone, SSN,
                        EmergencyContactName, EmergencyContactRelationship, EmergencyContactAddress,
                        Position1, Position2, SalaryDesired, SalaryType, EmploymentType, AvailableStartDate,
                        NashvilleLocation, FranklinLocation, ShelbyvilleLocation, WaynesboroLocation, OtherLocation,
                        FirstShift, SecondShift, ThirdShift, WeekendsOnly,
                        MondayAvailable, TuesdayAvailable, WednesdayAvailable, ThursdayAvailable,
                        FridayAvailable, SaturdayAvailable, SundayAvailable,
                        AppliedBefore, AppliedBeforeWhen, WorkedBefore, WorkedBeforeWhen,
                        FamilyEmployed, FamilyEmployedWho,
                        ConvictedOfCrime, AbuseRegistry
                    FROM JobApplications 
                    WHERE ApplicationId = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", applicationId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Personal Information
                        string fullName = $"{reader["FirstName"]} {reader["MiddleName"]} {reader["LastName"]}".Replace("  ", " ").Trim();
                        litFullName.Text = fullName;

                        string address = BuildAddress(reader);
                        litAddress.Text = address;

                        litPhoneNumber.Text = reader["HomePhone"]?.ToString() ?? "N/A";
                        litCellNumber.Text = reader["CellPhone"]?.ToString() ?? "N/A";

                        string emergencyContact = $"{reader["EmergencyContactName"]} ({reader["EmergencyContactRelationship"]})";
                        if (!string.IsNullOrEmpty(reader["EmergencyContactAddress"]?.ToString()))
                        {
                            emergencyContact += $"<br />{reader["EmergencyContactAddress"]}";
                        }
                        litEmergencyContact.Text = emergencyContact;

                        // Position & Availability
                        litPosition1.Text = reader["Position1"]?.ToString() ?? "N/A";
                        litPosition2.Text = reader["Position2"]?.ToString() ?? "N/A";

                        string salary = reader["SalaryDesired"]?.ToString();
                        if (!string.IsNullOrEmpty(salary))
                        {
                            salary += " " + (reader["SalaryType"]?.ToString() ?? "");
                        }
                        litSalaryDesired.Text = salary ?? "N/A";

                        litStartDate.Text = reader["AvailableStartDate"]?.ToString() ?? "N/A";

                        // Locations
                        List<string> locations = new List<string>();
                        if (reader["NashvilleLocation"] != DBNull.Value && Convert.ToBoolean(reader["NashvilleLocation"])) locations.Add("Nashville");
                        if (reader["FranklinLocation"] != DBNull.Value && Convert.ToBoolean(reader["FranklinLocation"])) locations.Add("Franklin");
                        if (reader["ShelbyvilleLocation"] != DBNull.Value && Convert.ToBoolean(reader["ShelbyvilleLocation"])) locations.Add("Shelbyville");
                        if (reader["WaynesboroLocation"] != DBNull.Value && Convert.ToBoolean(reader["WaynesboroLocation"])) locations.Add("Waynesboro");
                        if (reader["OtherLocation"] != DBNull.Value && Convert.ToBoolean(reader["OtherLocation"])) locations.Add("Other");

                        litLocations.Text = locations.Count > 0 ? string.Join(", ", locations) : "N/A";

                        // Shifts
                        List<string> shifts = new List<string>();
                        if (reader["FirstShift"] != DBNull.Value && Convert.ToBoolean(reader["FirstShift"])) shifts.Add("1st Shift");
                        if (reader["SecondShift"] != DBNull.Value && Convert.ToBoolean(reader["SecondShift"])) shifts.Add("2nd Shift");
                        if (reader["ThirdShift"] != DBNull.Value && Convert.ToBoolean(reader["ThirdShift"])) shifts.Add("3rd Shift");
                        if (reader["WeekendsOnly"] != DBNull.Value && Convert.ToBoolean(reader["WeekendsOnly"])) shifts.Add("Weekends Only");

                        litShifts.Text = shifts.Count > 0 ? string.Join(", ", shifts) : "N/A";

                        // Days Available
                        List<string> days = new List<string>();
                        if (reader["MondayAvailable"] != DBNull.Value && Convert.ToBoolean(reader["MondayAvailable"])) days.Add("Monday");
                        if (reader["TuesdayAvailable"] != DBNull.Value && Convert.ToBoolean(reader["TuesdayAvailable"])) days.Add("Tuesday");
                        if (reader["WednesdayAvailable"] != DBNull.Value && Convert.ToBoolean(reader["WednesdayAvailable"])) days.Add("Wednesday");
                        if (reader["ThursdayAvailable"] != DBNull.Value && Convert.ToBoolean(reader["ThursdayAvailable"])) days.Add("Thursday");
                        if (reader["FridayAvailable"] != DBNull.Value && Convert.ToBoolean(reader["FridayAvailable"])) days.Add("Friday");
                        if (reader["SaturdayAvailable"] != DBNull.Value && Convert.ToBoolean(reader["SaturdayAvailable"])) days.Add("Saturday");
                        if (reader["SundayAvailable"] != DBNull.Value && Convert.ToBoolean(reader["SundayAvailable"])) days.Add("Sunday");

                        litDaysAvailable.Text = days.Count > 0 ? string.Join(", ", days) : "N/A";

                        // Background Information
                        bool previousApp = reader["AppliedBefore"] != DBNull.Value && Convert.ToBoolean(reader["AppliedBefore"]);
                        string prevAppText = previousApp ? "Yes" : "No";
                        if (previousApp && !string.IsNullOrEmpty(reader["AppliedBeforeWhen"]?.ToString()))
                        {
                            prevAppText += $" - {reader["AppliedBeforeWhen"]}";
                        }
                        litPreviousApplication.Text = prevAppText;

                        bool previousWork = reader["WorkedBefore"] != DBNull.Value && Convert.ToBoolean(reader["WorkedBefore"]);
                        string prevWorkText = previousWork ? "Yes" : "No";
                        if (previousWork && !string.IsNullOrEmpty(reader["WorkedBeforeWhen"]?.ToString()))
                        {
                            prevWorkText += $" - {reader["WorkedBeforeWhen"]}";
                        }
                        litPreviousEmployment.Text = prevWorkText;

                        bool familyMembers = reader["FamilyEmployed"] != DBNull.Value && Convert.ToBoolean(reader["FamilyEmployed"]);
                        string familyText = familyMembers ? "Yes" : "No";
                        if (familyMembers && !string.IsNullOrEmpty(reader["FamilyEmployedWho"]?.ToString()))
                        {
                            familyText += $" - {reader["FamilyEmployedWho"]}";
                        }
                        litFamilyMembers.Text = familyText;

                        bool criminal = reader["ConvictedOfCrime"] != DBNull.Value && Convert.ToBoolean(reader["ConvictedOfCrime"]);
                        bool abuse = reader["AbuseRegistry"] != DBNull.Value && Convert.ToBoolean(reader["AbuseRegistry"]);
                        string criminalText = "No issues reported";
                        if (criminal || abuse)
                        {
                            List<string> issues = new List<string>();
                            if (criminal) issues.Add("Convicted of crime");
                            if (abuse) issues.Add("On abuse registry");
                            criminalText = string.Join("; ", issues);
                        }
                        litCriminalBackground.Text = criminalText;
                    }
                }
            }
        }

        private void LoadEmploymentHistory(int applicationId)
        {
            // JobApplications table contains employment history in individual fields (Employer1, Employer2, Employer3)
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        Employer1, EmploymentFrom1, EmploymentTo1, JobTitle1, Supervisor1, WorkPerformed1, ReasonLeaving1,
                        Employer2, EmploymentFrom2, EmploymentTo2, JobTitle2, Supervisor2, WorkPerformed2, ReasonLeaving2,
                        Employer3, EmploymentFrom3, EmploymentTo3, JobTitle3, Supervisor3, WorkPerformed3, ReasonLeaving3
                    FROM JobApplications 
                    WHERE ApplicationId = @ApplicationId";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<object> employmentHistory = new List<object>();

                    if (reader.Read())
                    {
                        // Add Employer 1 if exists
                        if (!string.IsNullOrEmpty(reader["Employer1"]?.ToString()))
                        {
                            employmentHistory.Add(new
                            {
                                Employer = reader["Employer1"],
                                DatesEmployedFrom = reader["EmploymentFrom1"],
                                DatesEmployedTo = reader["EmploymentTo1"],
                                JobTitle = reader["JobTitle1"],
                                Supervisor = reader["Supervisor1"],
                                TitleWorkPerformed = reader["WorkPerformed1"],
                                ReasonForLeaving = reader["ReasonLeaving1"]
                            });
                        }

                        // Add Employer 2 if exists
                        if (!string.IsNullOrEmpty(reader["Employer2"]?.ToString()))
                        {
                            employmentHistory.Add(new
                            {
                                Employer = reader["Employer2"],
                                DatesEmployedFrom = reader["EmploymentFrom2"],
                                DatesEmployedTo = reader["EmploymentTo2"],
                                JobTitle = reader["JobTitle2"],
                                Supervisor = reader["Supervisor2"],
                                TitleWorkPerformed = reader["WorkPerformed2"],
                                ReasonForLeaving = reader["ReasonLeaving2"]
                            });
                        }

                        // Add Employer 3 if exists
                        if (!string.IsNullOrEmpty(reader["Employer3"]?.ToString()))
                        {
                            employmentHistory.Add(new
                            {
                                Employer = reader["Employer3"],
                                DatesEmployedFrom = reader["EmploymentFrom3"],
                                DatesEmployedTo = reader["EmploymentTo3"],
                                JobTitle = reader["JobTitle3"],
                                Supervisor = reader["Supervisor3"],
                                TitleWorkPerformed = reader["WorkPerformed3"],
                                ReasonForLeaving = reader["ReasonLeaving3"]
                            });
                        }
                    }

                    rptEmploymentHistory.DataSource = employmentHistory;
                    rptEmploymentHistory.DataBind();
                }
            }
        }

  

        #endregion

        #region Button Event Handlers

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadApplications();
            ShowMessage("Applications refreshed successfully.", "success");
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            gvApplications.PageIndex = 0; // Reset to first page
            LoadApplications();
        }

        protected void btnClearFilter_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlStatus.SelectedIndex = 0;
            ddlPosition.SelectedIndex = 0;
            gvApplications.PageIndex = 0;
            LoadApplications();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                // Create CSV export
                StringBuilder csv = new StringBuilder();
                csv.AppendLine("Application Number,Name,Phone,Position,Status,Application Date");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sql = @"
                        SELECT 
                            ApplicationNumber, FirstName, LastName, HomePhone, Position1, Status, CreatedDate
                        FROM JobApplications 
                        ORDER BY CreatedDate DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            csv.AppendLine($"{reader["ApplicationNumber"]},{reader["FirstName"]} {reader["LastName"]},{reader["HomePhone"]},{reader["Position1"]},{reader["Status"]},{Convert.ToDateTime(reader["CreatedDate"]):yyyy-MM-dd}");
                        }
                    }
                }

                // Send CSV to browser
                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("Content-Disposition", $"attachment; filename=applications_{DateTime.Now:yyyyMMdd}.csv");
                Response.Write(csv.ToString());
                Response.End();
            }
            catch (Exception ex)
            {
                ShowMessage("Error exporting data: " + ex.Message, "error");
            }
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "hideApplicationModal",
                "hideApplicationModal();", true);
        }

        protected void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(hdnSelectedApplicationId.Value))
            {
                int applicationId = Convert.ToInt32(hdnSelectedApplicationId.Value);
                EditApplicationStatus(applicationId);
            }
        }

        protected void btnCloseStatusModal_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "hideStatusModal",
                "hideStatusModal();", true);
        }

       

        #region Helper Methods

        protected string GetInitials(string firstName, string lastName)
        {
            string first = !string.IsNullOrEmpty(firstName) ? firstName.Substring(0, 1) : "";
            string last = !string.IsNullOrEmpty(lastName) ? lastName.Substring(0, 1) : "";
            return (first + last).ToUpper();
        }

        protected string GetTimeSince(DateTime applicationDate)
        {
            TimeSpan timeSince = DateTime.Now - applicationDate;

            if (timeSince.Days > 0)
            {
                return $"{timeSince.Days} day{(timeSince.Days > 1 ? "s" : "")} ago";
            }
            else if (timeSince.Hours > 0)
            {
                return $"{timeSince.Hours} hour{(timeSince.Hours > 1 ? "s" : "")} ago";
            }
            else
            {
                return "Less than an hour ago";
            }
        }

        protected string GetStatusClass(string status)
        {
            if (string.IsNullOrEmpty(status)) return "draft";

            switch (status.ToUpper())
            {
                case "DRAFT": return "draft";
                case "SUBMITTED": return "submitted";
                case "UNDER_REVIEW": return "under-review";
                case "APPROVED": return "approved";
                case "REJECTED": return "rejected";
                default: return "draft";
            }
        }

        protected string GetStatusIcon(string status)
        {
            if (string.IsNullOrEmpty(status)) return "edit";

            switch (status.ToUpper())
            {
                case "DRAFT": return "edit";
                case "SUBMITTED": return "send";
                case "UNDER_REVIEW": return "hourglass_empty";
                case "APPROVED": return "check_circle";
                case "REJECTED": return "cancel";
                default: return "help";
            }
        }

        protected string GetStatusDisplayText(string status)
        {
            if (string.IsNullOrEmpty(status)) return "Draft";

            switch (status.ToUpper())
            {
                case "DRAFT": return "Draft";
                case "SUBMITTED": return "Submitted";
                case "UNDER_REVIEW": return "Under Review";
                case "APPROVED": return "Approved";
                case "REJECTED": return "Rejected";
                default: return status;
            }
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessages.Visible = true;
            lblMessage.Text = message;

            // Remove existing CSS classes
            pnlMessages.CssClass = "alert-panel";

            // Add appropriate CSS class based on type
            switch (type.ToLower())
            {
                case "success":
                    pnlMessages.CssClass += " success";
                    break;
                case "error":
                    pnlMessages.CssClass += " error";
                    break;
                case "warning":
                    pnlMessages.CssClass += " warning";
                    break;
                case "info":
                    pnlMessages.CssClass += " info";
                    break;
                default:
                    pnlMessages.CssClass += " info";
                    break;
            }
        }

        #endregion

       

    

        private void ProcessApprovalWithOnboarding(int applicationId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Step 1: Get application data
                        var applicationData = GetApplicationData(applicationId, conn, transaction);
                        if (applicationData == null)
                        {
                            ShowMessage("Application not found", "error");
                            return;
                        }

                        // Step 2: Update application status
                        UpdateApplicationStatusInTransaction(applicationId, "Approved", conn, transaction);

                        // Step 3: Create user account
                        int userId = CreateUserAccount(applicationData, conn, transaction);

                        // Step 4: Create employee record
                        int employeeId = CreateEmployeeRecord(applicationData, userId, conn, transaction);

                        // Step 5: Create onboarding tasks
                        CreateOnboardingTasks(employeeId, applicationData.HireDate, conn, transaction);

                        // Step 6: Create progress tracking
                        CreateOnboardingProgress(employeeId, conn, transaction);

                        transaction.Commit();
                        ShowMessage($"Application approved successfully! Employee {applicationData.EmployeeNumber} created and onboarding initiated.", "success");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ShowMessage($"Error processing approval: {ex.Message}", "error");
                    }
                }
            }
        }

        private void UpdateApplicationStatus(int applicationId, string status)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE [dbo].[JobApplications] SET [Status] = @Status, [LastModified] = GETUTCDATE() WHERE [ApplicationId] = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Id", applicationId);

                    int affected = cmd.ExecuteNonQuery();
                    if (affected > 0)
                    {
                        ShowMessage($"Application status updated to {status}.", "success");
                    }
                    else
                    {
                        ShowMessage("Application not found or no changes made.", "warning");
                    }
                }
            }
        }

        private ApplicationData GetApplicationData(int applicationId, SqlConnection conn, SqlTransaction transaction)
        {
            string sql = @"
        SELECT [FirstName], [LastName], [Position1], [CellPhone], [HomePhone], 
               [HomeAddress], [City], [State], [Zip], [AptNumber], [EmploymentType]
        FROM [dbo].[JobApplications] 
        WHERE [ApplicationId] = @ApplicationId";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ApplicationData
                        {
                            ApplicationId = applicationId,
                            FirstName = reader["FirstName"]?.ToString() ?? "",
                            LastName = reader["LastName"]?.ToString() ?? "",
                            Position = reader["Position1"]?.ToString() ?? "General Employee",
                            Phone = reader["CellPhone"]?.ToString() ?? reader["HomePhone"]?.ToString() ?? "Not Provided",
                            Address = BuildAddress(reader),
                            Email = $"{reader["FirstName"]}.{reader["LastName"]}@tpainc.com".ToLower(),
                            EmployeeNumber = GenerateEmployeeNumber(conn, transaction),
                            HireDate = DateTime.UtcNow.AddDays(14),
                            EmploymentType = reader["EmploymentType"]?.ToString() ?? "Full Time"
                        };
                    }
                }
            }
            return null;
        }

        private string BuildAddress(SqlDataReader reader)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(reader["HomeAddress"]?.ToString()))
                parts.Add(reader["HomeAddress"].ToString());

            if (!string.IsNullOrEmpty(reader["AptNumber"]?.ToString()))
                parts.Add($"Apt {reader["AptNumber"]}");

            if (!string.IsNullOrEmpty(reader["City"]?.ToString()))
                parts.Add(reader["City"].ToString());

            if (!string.IsNullOrEmpty(reader["State"]?.ToString()))
                parts.Add(reader["State"].ToString());

            if (!string.IsNullOrEmpty(reader["Zip"]?.ToString()))
                parts.Add(reader["Zip"].ToString());

            return parts.Count > 0 ? string.Join(", ", parts) : "Address Not Provided";
        }

        private string GenerateEmployeeNumber(SqlConnection conn, SqlTransaction transaction)
        {
            string sql = @"
        SELECT ISNULL(MAX(CAST(RIGHT([EmployeeNumber], 4) AS INT)), 0) + 1 
        FROM [dbo].[Employees] 
        WHERE [EmployeeNumber] LIKE 'EMP%' AND LEN([EmployeeNumber]) = 7";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                int nextNumber = Convert.ToInt32(cmd.ExecuteScalar());
                return $"EMP{nextNumber:D4}";
            }
        }

        private void UpdateApplicationStatusInTransaction(int applicationId, string status, SqlConnection conn, SqlTransaction transaction)
        {
            string sql = "UPDATE [dbo].[JobApplications] SET [Status] = @Status, [LastModified] = GETUTCDATE() WHERE [ApplicationId] = @Id";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Id", applicationId);
                cmd.ExecuteNonQuery();
            }
        }

        private int CreateUserAccount(ApplicationData data, SqlConnection conn, SqlTransaction transaction)
        {
            string sql = @"
        INSERT INTO [dbo].[Users] (
            [Email], [PasswordHash], [Salt], [Role], [IsActive], [MustChangePassword], 
            [CreatedAt], [UpdatedAt], [FailedLoginAttempts]
        )
        VALUES (
            @Email, @PasswordHash, @Salt, @Role, @IsActive, @MustChangePassword,
            GETUTCDATE(), GETUTCDATE(), @FailedLoginAttempts
        );
        SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@Email", data.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", "7UqSUHMlJ2oKwgsnJCCh/RdOpcTdJI537HSRDFW4OmY=");
                cmd.Parameters.AddWithValue("@Salt", "testsault");
                cmd.Parameters.AddWithValue("@Role", "EMPLOYEE");
                cmd.Parameters.AddWithValue("@IsActive", true);
                cmd.Parameters.AddWithValue("@MustChangePassword", true);
                cmd.Parameters.AddWithValue("@FailedLoginAttempts", 0);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private int CreateEmployeeRecord(ApplicationData data, int userId, SqlConnection conn, SqlTransaction transaction)
        {
            string sql = @"
        INSERT INTO [dbo].[Employees] (
            [UserId], [EmployeeNumber], [FirstName], [LastName], [Email], [PhoneNumber], 
            [Address], [DepartmentId], [JobTitle], [HireDate], [EmployeeType], [Status], 
            [CreatedAt], [UpdatedAt], [IsActive]
        )
        VALUES (
            @UserId, @EmployeeNumber, @FirstName, @LastName, @Email, @PhoneNumber,
            @Address, @DepartmentId, @JobTitle, @HireDate, @EmployeeType, @Status,
            GETUTCDATE(), GETUTCDATE(), @IsActive
        );
        SELECT SCOPE_IDENTITY();";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@EmployeeNumber", data.EmployeeNumber);
                cmd.Parameters.AddWithValue("@FirstName", data.FirstName);
                cmd.Parameters.AddWithValue("@LastName", data.LastName);
                cmd.Parameters.AddWithValue("@Email", data.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", data.Phone);
                cmd.Parameters.AddWithValue("@Address", data.Address);
                cmd.Parameters.AddWithValue("@DepartmentId", 1); // Default department
                cmd.Parameters.AddWithValue("@JobTitle", data.Position);
                cmd.Parameters.AddWithValue("@HireDate", data.HireDate);
                cmd.Parameters.AddWithValue("@EmployeeType", data.EmploymentType);
                cmd.Parameters.AddWithValue("@Status", "Pending Onboarding");
                cmd.Parameters.AddWithValue("@IsActive", true);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void CreateOnboardingTasks(int employeeId, DateTime hireDate, SqlConnection conn, SqlTransaction transaction)
        {
            string sql = @"
        INSERT INTO [dbo].[OnboardingTasks] (
            [EmployeeId], [Title], [Description], [Category], [Priority], [Status], 
            [DueDate], [Instructions], [CanEmployeeComplete], [BlocksSystemAccess], 
            [IsMandatory], [AssignedById], [CreatedDate]
        )
        VALUES 
        (@EmployeeId, 'Direct Deposit Enrollment', 'Complete direct deposit enrollment for payroll', 
         'SETUP', 'HIGH', 'PENDING', @DepositDueDate, 
         'Employee: Complete direct deposit enrollment form with banking information', 
         1, 0, 1, 1, GETUTCDATE()),
        (@EmployeeId, 'Complete Mandatory Training', 'Complete all required company training modules', 
         'TRAINING', 'HIGH', 'PENDING', @TrainingDueDate, 
         'Employee: Complete all mandatory training courses and assessments', 
         1, 0, 1, 1, GETUTCDATE())";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@DepositDueDate", hireDate.AddDays(7));
                cmd.Parameters.AddWithValue("@TrainingDueDate", hireDate.AddDays(10));
                cmd.ExecuteNonQuery();
            }
        }

        private void CreateOnboardingProgress(int employeeId, SqlConnection conn, SqlTransaction transaction)
        {
            string sql = @"
        INSERT INTO [dbo].[OnboardingProgress] (
            [EmployeeId], [TotalTasks], [CompletedTasks], [PendingTasks], [CompletionPercentage], 
            [StartDate], [Status], [LastUpdated], [MandatoryTasksTotal], [MandatoryTasksCompleted], 
            [MandatoryCompletionPercentage], [AllMandatoryCompleted]
        )
        VALUES (
            @EmployeeId, 2, 0, 2, 0.0, @StartDate, 'IN_PROGRESS', 
            GETUTCDATE(), 2, 0, 0.0, 0
        )";

            using (SqlCommand cmd = new SqlCommand(sql, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                cmd.Parameters.AddWithValue("@StartDate", DateTime.UtcNow.AddDays(14));
                cmd.ExecuteNonQuery();
            }
        }



       

  

        protected void btnSaveStatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(hdnSelectedApplicationId.Value))
                {
                    int applicationId = Convert.ToInt32(hdnSelectedApplicationId.Value);
                    string newStatus = ddlNewStatus.SelectedValue;

                    if (newStatus.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                    {
                        // Handle approval with onboarding creation
                        ApproveApplicationWithOnboarding(applicationId);
                    }
                    else
                    {
                        // Handle simple status update
                        SimpleStatusUpdate(applicationId, newStatus);
                    }

                    // Refresh and close modal
                    LoadApplications();
                    ScriptManager.RegisterStartupScript(this, GetType(), "hideStatusModal", "hideStatusModal();", true);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error processing status update: {ex.Message}", "error");
            }
        }

        private void ApproveApplicationWithOnboarding(int applicationId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Variables for the process
                        string firstName = "", lastName = "", position = "", phone = "", email = "";
                        string employeeNumber = "", address = "";
                        DateTime hireDate = DateTime.UtcNow.AddDays(14);
                        int userId = 0, employeeId = 0;

                        // Step 1: Get application data (properly close reader before next operations)
                        string getAppSql = @"
                    SELECT [FirstName], [LastName], [Position1], 
                           COALESCE([CellPhone], [HomePhone], 'Not Provided') as Phone,
                           COALESCE([HomeAddress] + ', ' + [City] + ', ' + [State] + ' ' + [Zip], 'Address Not Provided') as Address
                    FROM [dbo].[JobApplications] 
                    WHERE [ApplicationId] = @ApplicationId";

                        using (SqlCommand cmd = new SqlCommand(getAppSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    firstName = reader["FirstName"]?.ToString() ?? "";
                                    lastName = reader["LastName"]?.ToString() ?? "";
                                    position = reader["Position1"]?.ToString() ?? "General Employee";
                                    phone = reader["Phone"]?.ToString() ?? "";
                                    address = reader["Address"]?.ToString() ?? "";
                                    
                                    email = $"{firstName}{lastName.Substring(0, 1)}@tennesseepersonalassistance.org".ToLower();
                                }
                                // Reader will be closed automatically by using statement
                            }
                        }

                        // Check if we got the data
                        if (string.IsNullOrEmpty(firstName))
                        {
                            transaction.Rollback();
                            ShowMessage("Application data not found", "error");
                            return;
                        }

                        // Step 2: Generate employee number
                        string empNumSql = @"
                    SELECT 'EMP' + RIGHT('0000' + CAST(
                        ISNULL(MAX(CAST(RIGHT([EmployeeNumber], 4) AS INT)), 0) + 1 
                        AS VARCHAR(4)), 4)
                    FROM [dbo].[Employees] 
                    WHERE [EmployeeNumber] LIKE 'EMP%' AND LEN([EmployeeNumber]) = 7";

                        using (SqlCommand cmd = new SqlCommand(empNumSql, conn, transaction))
                        {
                            employeeNumber = cmd.ExecuteScalar()?.ToString() ?? "EMP0001";
                        }

                        // Step 3: Update application status
                        string updateAppSql = "UPDATE [dbo].[JobApplications] SET [Status] = 'Approved', [LastModified] = GETUTCDATE() WHERE [ApplicationId] = @ApplicationId";
                        using (SqlCommand cmd = new SqlCommand(updateAppSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@ApplicationId", applicationId);
                            cmd.ExecuteNonQuery();
                        }

                        // Step 4: Create user account
                        string createUserSql = @"
                    INSERT INTO [dbo].[Users] (
                        [Email], [PasswordHash], [Salt], [Role], [IsActive], [MustChangePassword], 
                        [CreatedAt], [UpdatedAt], [FailedLoginAttempts]
                    )
                    VALUES (
                        @Email, '7UqSUHMlJ2oKwgsnJCCh/RdOpcTdJI537HSRDFW4OmY=', 'testsalt', 'EMPLOYEE', 1, 1,
                        GETUTCDATE(), GETUTCDATE(), 0
                    );
                    SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmd = new SqlCommand(createUserSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Email", email);
                            userId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Step 5: Create employee record
                        string createEmpSql = @"
                    INSERT INTO [dbo].[Employees] (
                        [UserId], [EmployeeNumber], [FirstName], [LastName], [Email], [PhoneNumber], 
                        [Address], [DepartmentId], [JobTitle], [HireDate], [EmployeeType], [Status], 
                        [CreatedAt], [UpdatedAt], [IsActive]
                    )
                    VALUES (
                        @UserId, @EmployeeNumber, @FirstName, @LastName, @Email, @PhoneNumber,
                        @Address, 1, @JobTitle, @HireDate, 'Full Time', 'Pending Onboarding',
                        GETUTCDATE(), GETUTCDATE(), 1
                    );
                    SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmd = new SqlCommand(createEmpSql, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@UserId", userId);
                            cmd.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);
                            cmd.Parameters.AddWithValue("@FirstName", firstName);
                            cmd.Parameters.AddWithValue("@LastName", lastName);
                            cmd.Parameters.AddWithValue("@Email", email);
                            cmd.Parameters.AddWithValue("@PhoneNumber", phone);
                            cmd.Parameters.AddWithValue("@Address", address);
                            cmd.Parameters.AddWithValue("@JobTitle", position);
                            cmd.Parameters.AddWithValue("@HireDate", hireDate);

                            employeeId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // Step 6: Create 2 onboarding tasks (with TemplateId to satisfy foreign key)
                        string createTasksSql = @"
                    INSERT INTO [dbo].[OnboardingTasks] (
                        [EmployeeId], [TemplateId], [Title], [Description], [Category], [Priority], [Status], 
                        [DueDate], [Instructions], [CanEmployeeComplete], [BlocksSystemAccess], 
                        [IsMandatory], [AssignedById], [CreatedDate]
                    )
                    VALUES 
                    (@EmployeeId, @TemplateId, 'Direct Deposit Enrollment', 'Complete direct deposit enrollment for payroll', 
                     'SETUP', 'HIGH', 'PENDING', @DepositDueDate, 
                     'Employee: Complete direct deposit enrollment form with banking information', 
                     1, 0, 1, 1, GETUTCDATE()),
                    (@EmployeeId, @TemplateId, 'Complete Mandatory Training', 'Complete all required company training modules', 
                     'TRAINING', 'HIGH', 'PENDING', @TrainingDueDate, 
                     'Employee: Complete all mandatory training courses and assessments', 
                     1, 0, 1, 1, GETUTCDATE())";

                        using (SqlCommand cmd = new SqlCommand(createTasksSql, conn, transaction))
                        {
                            // Get a valid TemplateId or use 1 as default
                            int templateId = 1; // Default template ID
                            try
                            {
                                string getTemplateSql = "SELECT TOP 1 Id FROM [dbo].[OnboardingTemplates] WHERE IsActive = 1 ORDER BY Id";
                                using (SqlCommand templateCmd = new SqlCommand(getTemplateSql, conn, transaction))
                                {
                                    var result = templateCmd.ExecuteScalar();
                                    if (result != null)
                                        templateId = Convert.ToInt32(result);
                                }
                            }
                            catch
                            {
                                // Use default templateId = 1
                            }

                            cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                            cmd.Parameters.AddWithValue("@TemplateId", templateId);
                            cmd.Parameters.AddWithValue("@DepositDueDate", hireDate.AddDays(7));
                            cmd.Parameters.AddWithValue("@TrainingDueDate", hireDate.AddDays(10));
                            cmd.ExecuteNonQuery();
                        }

                        // Step 7: Create progress tracking (check if exists first)
                        string checkProgressSql = "SELECT COUNT(*) FROM [dbo].[OnboardingProgress] WHERE [EmployeeId] = @EmployeeId";
                        bool progressExists = false;

                        using (SqlCommand checkCmd = new SqlCommand(checkProgressSql, conn, transaction))
                        {
                            checkCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                            int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                            progressExists = count > 0;
                        }

                        if (!progressExists)
                        {
                            string createProgressSql = @"
                        INSERT INTO [dbo].[OnboardingProgress] (
                            [EmployeeId], [TotalTasks], [CompletedTasks], [PendingTasks], [CompletionPercentage], 
                            [StartDate], [Status], [LastUpdated], [MandatoryTasksTotal], [MandatoryTasksCompleted], 
                            [MandatoryCompletionPercentage], [AllMandatoryCompleted]
                        )
                        VALUES (
                            @EmployeeId, 2, 0, 2, 0.0, @StartDate, 'IN_PROGRESS', 
                            GETUTCDATE(), 2, 0, 0.0, 0
                        )";

                            using (SqlCommand cmd = new SqlCommand(createProgressSql, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                                cmd.Parameters.AddWithValue("@StartDate", hireDate);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        ShowMessage($"Application approved successfully! Employee {employeeNumber} ({firstName} {lastName}) created and onboarding initiated.", "success");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ShowMessage($"Error processing approval: {ex.Message}", "error");
                    }
                }
            }
        }

        private void SimpleStatusUpdate(int applicationId, string status)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE [dbo].[JobApplications] SET [Status] = @Status, [LastModified] = GETUTCDATE() WHERE [ApplicationId] = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Id", applicationId);

                    int affected = cmd.ExecuteNonQuery();
                    if (affected > 0)
                    {
                        ShowMessage($"Application status updated to {status}.", "success");
                    }
                    else
                    {
                        ShowMessage("Application not found or no changes made.", "warning");
                    }
                }
            }
        }
        // Helper class for application data
        public class ApplicationData
        {
            public int ApplicationId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Position { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string Email { get; set; }
            public string EmployeeNumber { get; set; }
            public DateTime HireDate { get; set; }
            public string EmploymentType { get; set; }
        }
    }
}
#endregion