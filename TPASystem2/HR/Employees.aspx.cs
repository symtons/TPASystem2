using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TPASystem2.Helpers;

namespace TPASystem2.HR
{
    public partial class Employees : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Check if user is logged in
            //if (Session["UserId"] == null)
            //{
            //    SimpleUrlHelper.RedirectToCleanUrl("login");
            //    return;
            //}

            // Check permissions - only HR, Admin, and Managers can access
            string userRole = Session["UserRole"]?.ToString() ?? "";
            if (!HasEmployeeManagementAccess(userRole))
            {
                Response.Redirect("~/Dashboard.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPageData();
            }
        }

        private void LoadPageData()
        {
            LoadDepartments();
            LoadEmployeeStats();
            LoadEmployees();
        }

        #region Data Loading Methods

        private void LoadDepartments()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT Id, Name 
                        FROM Departments 
                        WHERE IsActive = 1 
                        ORDER BY Name";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlDepartment.Items.Clear();
                            ddlDepartment.Items.Add(new ListItem("All Departments", ""));

                            while (reader.Read())
                            {
                                ddlDepartment.Items.Add(new ListItem(
                                    reader["Name"].ToString(),
                                    reader["Id"].ToString()
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading departments: {ex.Message}", "error");
            }
        }

        private void LoadEmployeeStats()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            COUNT(*) as TotalEmployees,
                            COUNT(CASE WHEN e.Status = 'Active' THEN 1 END) as ActiveEmployees,
                            COUNT(CASE WHEN e.HireDate >= DATEADD(DAY, -30, GETDATE()) THEN 1 END) as NewHires,
                            COUNT(DISTINCT e.DepartmentId) as DepartmentCount
                        FROM Employees e
                        WHERE e.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litTotalEmployees.Text = reader["TotalEmployees"].ToString();
                                litActiveEmployees.Text = reader["ActiveEmployees"].ToString();
                                litNewHires.Text = reader["NewHires"].ToString();
                                litDepartmentCount.Text = reader["DepartmentCount"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading stats: {ex.Message}");
            }
        }

        private void LoadEmployees()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Build query with filters
                    StringBuilder queryBuilder = new StringBuilder();
                    queryBuilder.Append(@"
                        SELECT 
                            e.Id,
                            e.EmployeeNumber,
                            e.FirstName,
                            e.LastName,
                            e.Email,
                            e.PhoneNumber,
                            e.Position,
                            e.JobTitle,
                            e.EmployeeType,
                            e.Status,
                            e.HireDate,
                            e.WorkLocation,
                            e.ProfilePictureUrl,
                            e.OnboardingStatus,
                            d.Name as DepartmentName,
                            m.FirstName + ' ' + m.LastName as ManagerName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        LEFT JOIN Employees m ON e.ManagerId = m.Id
                        WHERE e.IsActive = 1");

                    // Apply filters
                    if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                    {
                        queryBuilder.Append(@" AND (
                            e.FirstName LIKE @Search OR 
                            e.LastName LIKE @Search OR 
                            e.Email LIKE @Search OR 
                            e.EmployeeNumber LIKE @Search
                        )");
                    }

                    if (!string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                    {
                        queryBuilder.Append(" AND e.DepartmentId = @DepartmentId");
                    }

                    if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                    {
                        queryBuilder.Append(" AND e.Status = @Status");
                    }

                    if (!string.IsNullOrEmpty(ddlEmployeeType.SelectedValue))
                    {
                        queryBuilder.Append(" AND e.EmployeeType = @EmployeeType");
                    }

                    queryBuilder.Append(" ORDER BY e.FirstName, e.LastName");

                    using (SqlCommand cmd = new SqlCommand(queryBuilder.ToString(), conn))
                    {
                        // Add parameters
                        if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                        {
                            cmd.Parameters.AddWithValue("@Search", $"%{txtSearch.Text.Trim()}%");
                        }
                        if (!string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@DepartmentId", ddlDepartment.SelectedValue);
                        }
                        if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
                        }
                        if (!string.IsNullOrEmpty(ddlEmployeeType.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeType", ddlEmployeeType.SelectedValue);
                        }

                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        gvEmployees.DataSource = dt;
                        gvEmployees.DataBind();

                        // Update record count
                        litRecordCount.Text = dt.Rows.Count.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading employees: {ex.Message}", "error");
            }
        }

        #endregion

        #region Event Handlers

        protected void btnAddEmployee_Click(object sender, EventArgs e)
        {
            // Redirect to Add Employee page in HR folder
            Response.Redirect("~/HR/AddEmployee.aspx");
        }

        protected void btnExportEmployees_Click(object sender, EventArgs e)
        {
            try
            {
                ExportEmployeesToCSV();
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting employees: {ex.Message}", "error");
            }
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        protected void ddlEmployeeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlDepartment.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
            ddlEmployeeType.SelectedIndex = 0;
            LoadEmployees();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvEmployees.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            LoadEmployees();
        }

        protected void gvEmployees_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEmployees.PageIndex = e.NewPageIndex;
            LoadEmployees();
        }

        protected void gvEmployees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int employeeId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ViewEmployee":
                    ViewEmployee(employeeId);
                    break;
                case "EditEmployee":
                    EditEmployee(employeeId);
                    break;
                case "ViewOnboarding":
                    ViewOnboarding(employeeId);
                    break;
            }
        }

        #endregion

        #region Helper Methods for GridView Data Binding (PUBLIC - Required for ASPX)

        /// <summary>
        /// Gets employee avatar URL or returns default avatar
        /// </summary>
        public string GetEmployeeAvatar(object profilePictureUrl)
        {
            if (profilePictureUrl != null && !string.IsNullOrEmpty(profilePictureUrl.ToString()))
            {
                return profilePictureUrl.ToString();
            }
            return "~/Content/images/default-avatar.png";
        }

        /// <summary>
        /// Gets employee initials for avatar fallback
        /// </summary>
        public string GetInitials(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
                return "??";

            string initials = "";
            if (!string.IsNullOrEmpty(firstName))
                initials += firstName.Substring(0, 1).ToUpper();
            if (!string.IsNullOrEmpty(lastName))
                initials += lastName.Substring(0, 1).ToUpper();

            return string.IsNullOrEmpty(initials) ? "??" : initials;
        }

        /// <summary>
        /// Gets CSS class for employee status badge
        /// </summary>
        public string GetStatusClass(string status)
        {
            if (string.IsNullOrEmpty(status))
                return "secondary";

            switch (status.ToLower().Trim())
            {
                case "active":
                    return "success";
                case "inactive":
                    return "danger";
                case "on leave":
                    return "warning";
                case "terminated":
                    return "danger";
                case "pending":
                    return "warning";
                default:
                    return "secondary";
            }
        }

        /// <summary>
        /// Calculates and formats employee tenure
        /// </summary>
        public string GetTenure(object hireDateObj)
        {
            if (hireDateObj == null || hireDateObj == DBNull.Value)
                return "N/A";

            try
            {
                DateTime hireDate = Convert.ToDateTime(hireDateObj);
                TimeSpan tenure = DateTime.Now - hireDate;

                if (tenure.Days < 0)
                    return "Future start";
                else if (tenure.Days == 0)
                    return "Started today";
                else if (tenure.Days < 30)
                    return $"{tenure.Days} day{(tenure.Days != 1 ? "s" : "")}";
                else if (tenure.Days < 365)
                {
                    int months = tenure.Days / 30;
                    return $"{months} month{(months != 1 ? "s" : "")}";
                }
                else
                {
                    int years = tenure.Days / 365;
                    int remainingMonths = (tenure.Days % 365) / 30;
                    if (remainingMonths > 0)
                        return $"{years} year{(years != 1 ? "s" : "")}, {remainingMonths} month{(remainingMonths != 1 ? "s" : "")}";
                    else
                        return $"{years} year{(years != 1 ? "s" : "")}";
                }
            }
            catch (Exception)
            {
                return "Invalid date";
            }
        }

        #endregion

        #region Private Helper Methods

        private bool HasEmployeeManagementAccess(string userRole)
        {
            if (string.IsNullOrEmpty(userRole))
                return false;

            return userRole.Contains("Admin") ||
                   userRole.Contains("HR") ||
                   userRole.Contains("Manager") ||
                   userRole.Contains("SuperAdmin") ||
                   userRole.Contains("ProgramDirector");
        }

        private void ViewEmployee(int employeeId)
        {
            // Redirect to employee detail page in HR folder
            Response.Redirect($"~/HR/EmployeeDetail.aspx?id={employeeId}");
        }

        private void EditEmployee(int employeeId)
        {
            // Redirect to edit employee page in HR folder
            Response.Redirect($"~/HR/EditEmployee.aspx?id={employeeId}");
        }

        private void ViewOnboarding(int employeeId)
        {
            // Redirect to onboarding page
            Response.Redirect($"~/Onboarding/Tasks.aspx?employeeId={employeeId}");
        }

        private void ExportEmployeesToCSV()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            e.EmployeeNumber,
                            e.FirstName,
                            e.LastName,
                            e.Email,
                            e.PhoneNumber,
                            e.Position,
                            e.EmployeeType,
                            e.Status,
                            e.HireDate,
                            e.WorkLocation,
                            d.Name as Department,
                            m.FirstName + ' ' + m.LastName as Manager
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        LEFT JOIN Employees m ON e.ManagerId = m.Id
                        WHERE e.IsActive = 1
                        ORDER BY e.FirstName, e.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        // Create CSV content
                        StringBuilder csv = new StringBuilder();

                        // Add headers
                        csv.AppendLine("Employee Number,First Name,Last Name,Email,Phone,Position,Type,Status,Hire Date,Location,Department,Manager");

                        // Add data rows
                        foreach (DataRow row in dt.Rows)
                        {
                            csv.AppendLine($"\"{row["EmployeeNumber"]}\",\"{row["FirstName"]}\",\"{row["LastName"]}\",\"{row["Email"]}\",\"{row["PhoneNumber"]}\",\"{row["Position"]}\",\"{row["EmployeeType"]}\",\"{row["Status"]}\",\"{row["HireDate"]}\",\"{row["WorkLocation"]}\",\"{row["Department"]}\",\"{row["Manager"]}\"");
                        }

                        // Set response headers for file download
                        Response.Clear();
                        Response.ContentType = "text/csv";
                        Response.AddHeader("Content-Disposition", $"attachment; filename=Employees_{DateTime.Now:yyyyMMdd}.csv");
                        Response.Write(csv.ToString());
                        Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting data: {ex.Message}", "error");
            }
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = $"alert alert-{type}";
            litMessage.Text = message;
        }

        private void LogActivity(int userId, string action, string targetType, string description, string ipAddress)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ActivityLogs (UserId, Action, TargetType, Description, IPAddress, CreatedAt)
                        VALUES (@UserId, @Action, @TargetType, @Description, @IPAddress, GETUTCDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@Action", action);
                        cmd.Parameters.AddWithValue("@TargetType", targetType);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@IPAddress", ipAddress);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error logging activity: {ex.Message}");
            }
        }

        private string GetClientIP()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
                ip = Request.ServerVariables["REMOTE_ADDR"];
            return ip ?? "Unknown";
        }

        #endregion
    }
}