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

        private string BuildAddress(SqlDataReader reader)
        {
            List<string> addressParts = new List<string>();

            string street = reader["HomeAddress"]?.ToString();
            string apt = reader["AptNumber"]?.ToString();

            if (!string.IsNullOrEmpty(street))
            {
                if (!string.IsNullOrEmpty(apt))
                {
                    street += $" Apt {apt}";
                }
                addressParts.Add(street);
            }

            string city = reader["City"]?.ToString();
            string state = reader["State"]?.ToString();
            string zip = reader["Zip"]?.ToString();

            if (!string.IsNullOrEmpty(city) || !string.IsNullOrEmpty(state) || !string.IsNullOrEmpty(zip))
            {
                string cityStateZip = $"{city}, {state} {zip}".Trim().TrimStart(',').Trim();
                addressParts.Add(cityStateZip);
            }

            return addressParts.Count > 0 ? string.Join("<br />", addressParts) : "N/A";
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

        protected void btnSaveStatus_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(hdnSelectedApplicationId.Value))
                {
                    int applicationId = Convert.ToInt32(hdnSelectedApplicationId.Value);
                    string newStatus = ddlNewStatus.SelectedValue;
                    string notes = txtStatusNotes.Text.Trim();

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string sql = @"
                            UPDATE JobApplications 
                            SET Status = @Status, LastModified = @LastModified
                            WHERE ApplicationId = @Id";

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@Status", newStatus);
                            cmd.Parameters.AddWithValue("@LastModified", DateTime.Now);
                            cmd.Parameters.AddWithValue("@Id", applicationId);

                            conn.Open();
                            int affected = cmd.ExecuteNonQuery();

                            if (affected > 0)
                            {
                                ShowMessage($"Application status updated to {GetStatusDisplayText(newStatus)}.", "success");
                                LoadApplications();

                                // Clear form
                                txtStatusNotes.Text = "";

                                // Hide modal
                                ScriptManager.RegisterStartupScript(this, GetType(), "hideStatusModal",
                                    "hideStatusModal();", true);
                            }
                            else
                            {
                                ShowMessage("Failed to update application status.", "error");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error updating status: " + ex.Message, "error");
            }
        }

        #endregion

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
    }
}