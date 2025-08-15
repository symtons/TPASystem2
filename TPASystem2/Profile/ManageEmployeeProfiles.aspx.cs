using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPASystem2.HR
{
    public partial class ManageEmployeeProfiles : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int CurrentUserId => Convert.ToInt32(Session["UserId"] ?? 0);
        private int CurrentEmployeeId => Convert.ToInt32(Session["EmployeeId"] ?? 0);
        private string CurrentUserRole => Session["UserRole"]?.ToString() ?? "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidateUserAccess();
                LoadUserInformation();
                LoadDashboardOverview();
                LoadDropdownData();
                LoadEmployeeGrid();
            }
        }

        #region Security and Access Control

        private void ValidateUserAccess()
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                return;
            }

            try
            {
                string userRole = CurrentUserRole;
                var allowedRoles = new[] { "ADMIN", "HRADMIN", "SUPERADMIN", "PROGRAMDIRECTOR" };

                if (!Array.Exists(allowedRoles, role => role.Equals(userRole, StringComparison.OrdinalIgnoreCase)))
                {
                    Response.Redirect("~/Unauthorized.aspx", false);
                    return;
                }

                // Configure interface based on role
                ConfigureRoleBasedInterface();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Access validation failed. Please contact support.", "error");
            }
        }

        private void ConfigureRoleBasedInterface()
        {
            bool isAdmin = IsAdmin();
            bool isProgramDirector = IsProgramDirector();

            // Show/hide Add Employee button based on role
            btnAddEmployee.Visible = isAdmin || isProgramDirector;

            // Set user role display
            litUserRole.Text = CurrentUserRole;
        }

        public bool IsAdmin()
        {
            string currentRole = CurrentUserRole?.ToUpper() ?? "";
            return currentRole == "ADMIN" ||
                   currentRole == "HRADMIN" ||
                   currentRole == "SUPERADMIN" ||
                   currentRole == "SYSTEMADMINISTRATOR";
        }

        public bool IsProgramDirector()
        {
            return CurrentUserRole?.ToUpper() == "PROGRAMDIRECTOR";
        }

        private int GetUserDepartmentId()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT DepartmentId FROM Employees WHERE UserId = @UserId AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        object result = cmd.ExecuteScalar();
                        return result != null && result != DBNull.Value ?
                            Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return 0;
            }
        }

        private bool CanAccessEmployee(int employeeId)
        {
            try
            {
                bool isAdmin = IsAdmin();
                bool isProgramDirector = IsProgramDirector();

                // Admins should always have access
                if (isAdmin)
                {
                    return true;
                }

                // Non-program directors should not have access
                if (!isProgramDirector)
                {
                    return false;
                }

                // Program Directors can only access employees in their department
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        SELECT COUNT(*) 
                        FROM Employees 
                        WHERE Id = @EmployeeId 
                        AND DepartmentId = @UserDepartmentId 
                        AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        cmd.Parameters.AddWithValue("@UserDepartmentId", GetUserDepartmentId());
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return IsAdmin();
            }
        }

        #endregion

        #region Data Loading Methods

        private void LoadUserInformation()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            e.FirstName + ' ' + e.LastName as FullName,
                            d.Name as DepartmentName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        WHERE e.UserId = @UserId AND e.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litManagerName.Text = reader["FullName"]?.ToString() ?? "System Administrator";
                                litDepartmentName.Text = reader["DepartmentName"]?.ToString() ?? "Administration";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                litManagerName.Text = "System Administrator";
                litDepartmentName.Text = "Administration";
            }
        }

        private void LoadDashboardOverview()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get total employees count
                    string totalQuery = "SELECT COUNT(*) FROM Employees WHERE IsActive = 1";
                    if (IsProgramDirector() && !IsAdmin())
                    {
                        totalQuery = @"
                            SELECT COUNT(*) 
                            FROM Employees 
                            WHERE IsActive = 1 AND DepartmentId = @UserDepartmentId";
                    }

                    using (SqlCommand cmd = new SqlCommand(totalQuery, conn))
                    {
                        if (IsProgramDirector() && !IsAdmin())
                        {
                            cmd.Parameters.AddWithValue("@UserDepartmentId", GetUserDepartmentId());
                        }

                        int totalEmployees = Convert.ToInt32(cmd.ExecuteScalar());
                        litTotalEmployees.Text = totalEmployees.ToString();
                    }

                    // Get active onboarding count
                    string onboardingQuery = @"
                        SELECT COUNT(DISTINCT e.Id) 
                        FROM Employees e 
                        WHERE e.IsActive = 1 
                        AND (e.OnboardingStatus IS NULL OR e.OnboardingStatus != 'COMPLETED')";

                    if (IsProgramDirector() && !IsAdmin())
                    {
                        onboardingQuery += " AND e.DepartmentId = @UserDepartmentId";
                    }

                    using (SqlCommand cmd = new SqlCommand(onboardingQuery, conn))
                    {
                        if (IsProgramDirector() && !IsAdmin())
                        {
                            cmd.Parameters.AddWithValue("@UserDepartmentId", GetUserDepartmentId());
                        }

                        int activeOnboarding = Convert.ToInt32(cmd.ExecuteScalar());
                        litActiveOnboarding.Text = activeOnboarding.ToString();
                    }

                    // Get new hires this month
                    string newHiresQuery = @"
                        SELECT COUNT(*) 
                        FROM Employees 
                        WHERE IsActive = 1 
                        AND HireDate >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)
                        AND HireDate < DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) + 1, 0)";

                    if (IsProgramDirector() && !IsAdmin())
                    {
                        newHiresQuery += " AND DepartmentId = @UserDepartmentId";
                    }

                    using (SqlCommand cmd = new SqlCommand(newHiresQuery, conn))
                    {
                        if (IsProgramDirector() && !IsAdmin())
                        {
                            cmd.Parameters.AddWithValue("@UserDepartmentId", GetUserDepartmentId());
                        }

                        int newHires = Convert.ToInt32(cmd.ExecuteScalar());
                        litNewHires.Text = newHires.ToString();
                    }

                    // Get department count
                    string deptQuery = "SELECT COUNT(*) FROM Departments WHERE IsActive = 1";
                    using (SqlCommand cmd = new SqlCommand(deptQuery, conn))
                    {
                        int deptCount = Convert.ToInt32(cmd.ExecuteScalar());
                        litDepartmentCount.Text = deptCount.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                litTotalEmployees.Text = "0";
                litActiveOnboarding.Text = "0";
                litNewHires.Text = "0";
                litDepartmentCount.Text = "0";
            }
        }

        private void LoadDropdownData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    LoadDepartments(conn);
                    LoadManagers(conn);
                    LoadDepartmentFilter(conn);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading dropdown data.", "error");
            }
        }

        private void LoadDepartments(SqlConnection conn)
        {
            string query = "SELECT Id, Name FROM Departments WHERE IsActive = 1 ORDER BY Name";

            if (IsProgramDirector() && !IsAdmin())
            {
                query = @"
                    SELECT Id, Name FROM Departments 
                    WHERE IsActive = 1 AND Id = @UserDepartmentId 
                    ORDER BY Name";
            }

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (IsProgramDirector() && !IsAdmin())
                {
                    cmd.Parameters.AddWithValue("@UserDepartmentId", GetUserDepartmentId());
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    ddlDepartment.Items.Clear();
                    ddlDepartment.Items.Add(new ListItem("Select Department", ""));

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

        private void LoadManagers(SqlConnection conn)
        {
            string query = @"
                SELECT Id, FirstName + ' ' + LastName as FullName 
                FROM Employees 
                WHERE IsActive = 1 
                AND (JobTitle LIKE '%Manager%' OR JobTitle LIKE '%Director%' OR JobTitle LIKE '%Supervisor%')
                ORDER BY FullName";

            if (IsProgramDirector() && !IsAdmin())
            {
                query = @"
                    SELECT Id, FirstName + ' ' + LastName as FullName 
                    FROM Employees 
                    WHERE IsActive = 1 
                    AND DepartmentId = @UserDepartmentId
                    AND (JobTitle LIKE '%Manager%' OR JobTitle LIKE '%Director%' OR JobTitle LIKE '%Supervisor%')
                    ORDER BY FullName";
            }

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (IsProgramDirector() && !IsAdmin())
                {
                    cmd.Parameters.AddWithValue("@UserDepartmentId", GetUserDepartmentId());
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    ddlManager.Items.Clear();
                    ddlManager.Items.Add(new ListItem("Select Manager", ""));

                    while (reader.Read())
                    {
                        ddlManager.Items.Add(new ListItem(
                            reader["FullName"].ToString(),
                            reader["Id"].ToString()
                        ));
                    }
                }
            }
        }

        private void LoadDepartmentFilter(SqlConnection conn)
        {
            string query = "SELECT Id, Name FROM Departments WHERE IsActive = 1 ORDER BY Name";

            if (IsProgramDirector() && !IsAdmin())
            {
                query = @"
                    SELECT Id, Name FROM Departments 
                    WHERE IsActive = 1 AND Id = @UserDepartmentId 
                    ORDER BY Name";
            }

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                if (IsProgramDirector() && !IsAdmin())
                {
                    cmd.Parameters.AddWithValue("@UserDepartmentId", GetUserDepartmentId());
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    ddlDepartmentFilter.Items.Clear();
                    ddlDepartmentFilter.Items.Add(new ListItem("All Departments", ""));

                    while (reader.Read())
                    {
                        ddlDepartmentFilter.Items.Add(new ListItem(
                            reader["Name"].ToString(),
                            reader["Id"].ToString()
                        ));
                    }
                }
            }
        }

        private void LoadEmployeeGrid()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    StringBuilder query = new StringBuilder(@"
                        SELECT 
                            e.Id,
                            e.FirstName,
                            e.LastName,
                            e.EmployeeNumber,
                            e.Email,
                            e.JobTitle,
                            e.EmployeeType,
                            e.HireDate,
                            e.Status,
                            d.Name as DepartmentName,
                            u.Role as UserRole,
                            u.IsActive as UserIsActive
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        LEFT JOIN Users u ON e.UserId = u.Id
                        WHERE e.IsActive = 1");

                    // Apply role-based filtering
                    if (IsProgramDirector() && !IsAdmin())
                    {
                        query.Append(" AND e.DepartmentId = @UserDepartmentId");
                    }

                    // Apply search filter
                    if (!string.IsNullOrEmpty(txtEmployeeSearch.Text.Trim()))
                    {
                        query.Append(@" AND (
                            e.FirstName LIKE @SearchTerm OR 
                            e.LastName LIKE @SearchTerm OR 
                            e.EmployeeNumber LIKE @SearchTerm OR 
                            e.Email LIKE @SearchTerm
                        )");
                    }

                    // Apply department filter
                    if (!string.IsNullOrEmpty(ddlDepartmentFilter.SelectedValue))
                    {
                        query.Append(" AND e.DepartmentId = @DepartmentFilter");
                    }

                    // Apply employee type filter
                    if (!string.IsNullOrEmpty(ddlEmployeeTypeFilter.SelectedValue))
                    {
                        query.Append(" AND e.EmployeeType = @EmployeeTypeFilter");
                    }

                    // Apply status filter
                    if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
                    {
                        query.Append(" AND e.Status = @StatusFilter");
                    }

                    query.Append(" ORDER BY e.FirstName, e.LastName");

                    using (SqlCommand cmd = new SqlCommand(query.ToString(), conn))
                    {
                        // Add parameters
                        if (IsProgramDirector() && !IsAdmin())
                        {
                            cmd.Parameters.AddWithValue("@UserDepartmentId", GetUserDepartmentId());
                        }

                        if (!string.IsNullOrEmpty(txtEmployeeSearch.Text.Trim()))
                        {
                            cmd.Parameters.AddWithValue("@SearchTerm", "%" + txtEmployeeSearch.Text.Trim() + "%");
                        }

                        if (!string.IsNullOrEmpty(ddlDepartmentFilter.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@DepartmentFilter", ddlDepartmentFilter.SelectedValue);
                        }

                        if (!string.IsNullOrEmpty(ddlEmployeeTypeFilter.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeTypeFilter", ddlEmployeeTypeFilter.SelectedValue);
                        }

                        if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@StatusFilter", ddlStatusFilter.SelectedValue);
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            gvEmployees.DataSource = dt;
                            gvEmployees.DataBind();

                            litEmployeeCount.Text = dt.Rows.Count.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading employee data.", "error");
            }
        }

        #endregion

        #region Grid Event Handlers

        

        

    

        #endregion

        #region Filter Event Handlers

        protected void txtEmployeeSearch_TextChanged(object sender, EventArgs e)
        {
            LoadEmployeeGrid();
        }

        protected void ddlDepartmentFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmployeeGrid();
        }

        protected void ddlEmployeeTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmployeeGrid();
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmployeeGrid();
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtEmployeeSearch.Text = "";
            ddlDepartmentFilter.SelectedIndex = 0;
            ddlEmployeeTypeFilter.SelectedIndex = 0;
            ddlStatusFilter.SelectedIndex = 0;
            LoadEmployeeGrid();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadEmployeeGrid();
            LoadDashboardOverview();
        }

        #endregion

        #region Employee Profile Management

        private void LoadEmployeeProfile(int employeeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            e.*,
                            d.Name as DepartmentName,
                            mgr.FirstName + ' ' + mgr.LastName as ManagerName,
                            u.Role as UserRole,
                            u.IsActive as UserIsActive,
                            u.MustChangePassword
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        LEFT JOIN Employees mgr ON e.ManagerId = mgr.Id
                        LEFT JOIN Users u ON e.UserId = u.Id
                        WHERE e.Id = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Store employee ID in ViewState for saving
                                ViewState["EditingEmployeeId"] = employeeId;

                                // Header information
                                litSelectedEmployee.Text = $"{reader["FirstName"]} {reader["LastName"]} - {reader["EmployeeNumber"]}";

                                // Personal Information
                                txtFirstName.Text = reader["FirstName"]?.ToString() ?? "";
                                txtLastName.Text = reader["LastName"]?.ToString() ?? "";
                                txtEmployeeNumber.Text = reader["EmployeeNumber"]?.ToString() ?? "";
                                txtDateOfBirth.Text = reader["DateOfBirth"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["DateOfBirth"]).ToString("yyyy-MM-dd") : "";
                                ddlGender.SelectedValue = reader["Gender"]?.ToString() ?? "";

                                // Employment Information
                                txtJobTitle.Text = reader["JobTitle"]?.ToString() ?? "";
                                ddlDepartment.SelectedValue = reader["DepartmentId"]?.ToString() ?? "";
                                ddlEmployeeType.SelectedValue = reader["EmployeeType"]?.ToString() ?? "";
                                txtHireDate.Text = reader["HireDate"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["HireDate"]).ToString("yyyy-MM-dd") : "";
                                ddlManager.SelectedValue = reader["ManagerId"]?.ToString() ?? "";
                                ddlEmployeeStatus.SelectedValue = reader["Status"]?.ToString() ?? "";
                                txtSalary.Text = reader["Salary"] != DBNull.Value ?
                                    reader["Salary"].ToString() : "";
                                txtWorkLocation.Text = reader["WorkLocation"]?.ToString() ?? "";

                                // Contact Information
                                txtEmail.Text = reader["Email"]?.ToString() ?? "";
                                txtPhoneNumber.Text = reader["PhoneNumber"]?.ToString() ?? "";
                                txtAddress.Text = reader["Address"]?.ToString() ?? "";
                                txtCity.Text = reader["City"]?.ToString() ?? "";
                                txtState.Text = reader["State"]?.ToString() ?? "";
                                txtZipCode.Text = reader["ZipCode"]?.ToString() ?? "";

                                // System Access
                                ddlUserRole.SelectedValue = reader["UserRole"]?.ToString() ?? "";
                                chkIsActive.Checked = reader["UserIsActive"] != DBNull.Value ?
                                    Convert.ToBoolean(reader["UserIsActive"]) : false;
                                chkMustChangePassword.Checked = reader["MustChangePassword"] != DBNull.Value ?
                                    Convert.ToBoolean(reader["MustChangePassword"]) : false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading employee profile.", "error");
            }
        }

        private void LoadEmployeeActivity(int employeeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Get employee name first
                    string nameQuery = "SELECT FirstName + ' ' + LastName as FullName FROM Employees WHERE Id = @EmployeeId";
                    using (SqlCommand nameCmd = new SqlCommand(nameQuery, conn))
                    {
                        nameCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        object result = nameCmd.ExecuteScalar();
                        litActivityEmployee.Text = result?.ToString() ?? "Unknown Employee";
                    }

                    string query = @"
                        SELECT TOP 20
                            ra.Action,
                            ra.EntityType,
                            ra.Details,
                            ra.CreatedAt,
                            u.Role
                        FROM RecentActivities ra
                        LEFT JOIN Users u ON ra.UserId = u.Id
                        WHERE ra.EmployeeId = @EmployeeId OR ra.UserId = (
                            SELECT UserId FROM Employees WHERE Id = @EmployeeId
                        )
                        ORDER BY ra.CreatedAt DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            StringBuilder activityHtml = new StringBuilder();

                            if (!reader.HasRows)
                            {
                                activityHtml.Append("<p class='no-activity'>No recent activity found.</p>");
                            }
                            else
                            {
                                while (reader.Read())
                                {
                                    DateTime createdAt = Convert.ToDateTime(reader["CreatedAt"]);
                                    string timeAgo = GetTimeAgo(createdAt);

                                    activityHtml.AppendLine($@"
                                        <div class='activity-item'>
                                            <div class='activity-icon'>
                                                <i class='material-icons'>history</i>
                                            </div>
                                            <div class='activity-content'>
                                                <div class='activity-title'>{reader["Action"]}</div>
                                                <div class='activity-details'>{reader["Details"]}</div>
                                                <div class='activity-time'>{timeAgo}</div>
                                            </div>
                                        </div>");
                                }
                            }

                            litRecentActivity.Text = activityHtml.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                litRecentActivity.Text = "<p class='error'>Unable to load activity history.</p>";
            }
        }

        private void ToggleEmployeeStatus(int employeeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Get current status
                            string getCurrentStatusQuery = "SELECT Status, FirstName, LastName FROM Employees WHERE Id = @EmployeeId";
                            string currentStatus = "";
                            string employeeName = "";

                            using (SqlCommand getCmd = new SqlCommand(getCurrentStatusQuery, conn, transaction))
                            {
                                getCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                                using (SqlDataReader reader = getCmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        currentStatus = reader["Status"].ToString();
                                        employeeName = $"{reader["FirstName"]} {reader["LastName"]}";
                                    }
                                }
                            }

                            // Toggle status
                            string newStatus = currentStatus == "ACTIVE" ? "INACTIVE" : "ACTIVE";

                            string updateQuery = @"
                                UPDATE Employees 
                                SET Status = @NewStatus, UpdatedAt = GETDATE()
                                WHERE Id = @EmployeeId";

                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@NewStatus", newStatus);
                                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                                cmd.ExecuteNonQuery();
                            }

                            // Log the activity
                            LogActivity("Employee Status Changed", "Employee",
                                $"Changed status of {employeeName} from {currentStatus} to {newStatus}",
                                employeeId, conn, transaction);

                            transaction.Commit();
                            ShowMessage($"Employee status changed to {newStatus} successfully!", "success");

                            // Reload grid
                            LoadEmployeeGrid();
                            LoadDashboardOverview();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error changing employee status.", "error");
            }
        }

        private void DeleteEmployee(int employeeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Get employee name for logging
                            string getNameQuery = "SELECT FirstName, LastName FROM Employees WHERE Id = @EmployeeId";
                            string employeeName = "";
                            using (SqlCommand nameCmd = new SqlCommand(getNameQuery, conn, transaction))
                            {
                                nameCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                                using (SqlDataReader reader = nameCmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        employeeName = $"{reader["FirstName"]} {reader["LastName"]}";
                                    }
                                }
                            }

                            // Soft delete - set IsActive to false instead of actual deletion
                            string deleteQuery = @"
                                UPDATE Employees 
                                SET IsActive = 0, 
                                    Status = 'TERMINATED',
                                    TerminationDate = GETDATE(),
                                    UpdatedAt = GETDATE()
                                WHERE Id = @EmployeeId";

                            using (SqlCommand cmd = new SqlCommand(deleteQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                                cmd.ExecuteNonQuery();
                            }

                            // Deactivate associated user account
                            string deactivateUserQuery = @"
                                UPDATE Users 
                                SET IsActive = 0, UpdatedAt = GETDATE()
                                WHERE Id = (SELECT UserId FROM Employees WHERE Id = @EmployeeId)";

                            using (SqlCommand userCmd = new SqlCommand(deactivateUserQuery, conn, transaction))
                            {
                                userCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                                userCmd.ExecuteNonQuery();
                            }

                            // Log the activity
                            LogActivity("Employee Deleted", "Employee",
                                $"Deleted employee: {employeeName} (ID: {employeeId})", employeeId, conn, transaction);

                            transaction.Commit();
                            ShowMessage($"Employee {employeeName} has been deleted successfully.", "success");

                            // Reload grid and overview
                            LoadEmployeeGrid();
                            LoadDashboardOverview();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error deleting employee. Please try again.", "error");
            }
        }

        #endregion

        #region Modal Event Handlers

       
        #endregion

        #region Action Button Handlers

        protected void btnAddEmployee_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/AddEmployee.aspx");
        }

        protected void btnExportEmployees_Click(object sender, EventArgs e)
        {
            try
            {
                // Create CSV export
                StringBuilder csv = new StringBuilder();
                csv.AppendLine("Employee Number,First Name,Last Name,Email,Department,Job Title,Employee Type,Hire Date,Status");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            e.EmployeeNumber,
                            e.FirstName,
                            e.LastName,
                            e.Email,
                            d.Name as DepartmentName,
                            e.JobTitle,
                            e.EmployeeType,
                            e.HireDate,
                            e.Status
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        WHERE e.IsActive = 1";

                    if (IsProgramDirector() && !IsAdmin())
                    {
                        query += " AND e.DepartmentId = @UserDepartmentId";
                    }

                    query += " ORDER BY e.FirstName, e.LastName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (IsProgramDirector() && !IsAdmin())
                        {
                            cmd.Parameters.AddWithValue("@UserDepartmentId", GetUserDepartmentId());
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                csv.AppendLine($"\"{reader["EmployeeNumber"]}\",\"{reader["FirstName"]}\",\"{reader["LastName"]}\",\"{reader["Email"]}\",\"{reader["DepartmentName"]}\",\"{reader["JobTitle"]}\",\"{reader["EmployeeType"]}\",\"{(reader["HireDate"] != DBNull.Value ? Convert.ToDateTime(reader["HireDate"]).ToString("yyyy-MM-dd") : "")}\",\"{reader["Status"]}\"");
                            }
                        }
                    }
                }

                // Download CSV
                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("content-disposition", $"attachment;filename=Employees_{DateTime.Now:yyyyMMdd}.csv");
                Response.Write(csv.ToString());
                Response.End();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error exporting employee data.", "error");
            }
        }

        #endregion

        #region Save Methods

        private void SaveEmployeeProfile(int employeeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Update Employee record
                            string updateEmployeeQuery = @"
                                UPDATE Employees SET
                                    FirstName = @FirstName,
                                    LastName = @LastName,
                                    EmployeeNumber = @EmployeeNumber,
                                    DateOfBirth = @DateOfBirth,
                                    Gender = @Gender,
                                    JobTitle = @JobTitle,
                                    DepartmentId = @DepartmentId,
                                    EmployeeType = @EmployeeType,
                                    HireDate = @HireDate,
                                    ManagerId = @ManagerId,
                                    Status = @Status,
                                    Salary = @Salary,
                                    WorkLocation = @WorkLocation,
                                    Email = @Email,
                                    PhoneNumber = @PhoneNumber,
                                    Address = @Address,
                                    City = @City,
                                    State = @State,
                                    ZipCode = @ZipCode,
                                    UpdatedAt = GETDATE()
                                WHERE Id = @EmployeeId";

                            using (SqlCommand cmd = new SqlCommand(updateEmployeeQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                                cmd.Parameters.AddWithValue("@EmployeeNumber", txtEmployeeNumber.Text.Trim());
                                cmd.Parameters.AddWithValue("@DateOfBirth",
                                    string.IsNullOrEmpty(txtDateOfBirth.Text) ? (object)DBNull.Value : DateTime.Parse(txtDateOfBirth.Text));
                                cmd.Parameters.AddWithValue("@Gender",
                                    string.IsNullOrEmpty(ddlGender.SelectedValue) ? (object)DBNull.Value : ddlGender.SelectedValue);
                                cmd.Parameters.AddWithValue("@JobTitle", txtJobTitle.Text.Trim());
                                cmd.Parameters.AddWithValue("@DepartmentId",
                                    string.IsNullOrEmpty(ddlDepartment.SelectedValue) ? (object)DBNull.Value : int.Parse(ddlDepartment.SelectedValue));
                                cmd.Parameters.AddWithValue("@EmployeeType",
                                    string.IsNullOrEmpty(ddlEmployeeType.SelectedValue) ? (object)DBNull.Value : ddlEmployeeType.SelectedValue);
                                cmd.Parameters.AddWithValue("@HireDate",
                                    string.IsNullOrEmpty(txtHireDate.Text) ? (object)DBNull.Value : DateTime.Parse(txtHireDate.Text));
                                cmd.Parameters.AddWithValue("@ManagerId",
                                    string.IsNullOrEmpty(ddlManager.SelectedValue) ? (object)DBNull.Value : int.Parse(ddlManager.SelectedValue));
                                cmd.Parameters.AddWithValue("@Status",
                                    string.IsNullOrEmpty(ddlEmployeeStatus.SelectedValue) ? "ACTIVE" : ddlEmployeeStatus.SelectedValue);
                                cmd.Parameters.AddWithValue("@Salary",
                                    string.IsNullOrEmpty(txtSalary.Text) ? (object)DBNull.Value : decimal.Parse(txtSalary.Text));
                                cmd.Parameters.AddWithValue("@WorkLocation", txtWorkLocation.Text.Trim());
                                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                                cmd.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text.Trim());
                                cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
                                cmd.Parameters.AddWithValue("@City", txtCity.Text.Trim());
                                cmd.Parameters.AddWithValue("@State", txtState.Text.Trim());
                                cmd.Parameters.AddWithValue("@ZipCode", txtZipCode.Text.Trim());
                                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                                cmd.ExecuteNonQuery();
                            }

                            // Update User record if exists and user has permission
                            if (IsAdmin() && !string.IsNullOrEmpty(ddlUserRole.SelectedValue))
                            {
                                // Check if user record exists
                                string checkUserQuery = "SELECT UserId FROM Employees WHERE Id = @EmployeeId";
                                using (SqlCommand checkCmd = new SqlCommand(checkUserQuery, conn, transaction))
                                {
                                    checkCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                                    object userId = checkCmd.ExecuteScalar();

                                    if (userId != null && userId != DBNull.Value)
                                    {
                                        // Update existing user
                                        string updateUserQuery = @"
                                            UPDATE Users SET
                                                Role = @Role,
                                                IsActive = @IsActive,
                                                MustChangePassword = @MustChangePassword,
                                                UpdatedAt = GETDATE()
                                            WHERE Id = @UserId";

                                        using (SqlCommand userCmd = new SqlCommand(updateUserQuery, conn, transaction))
                                        {
                                            userCmd.Parameters.AddWithValue("@Role", ddlUserRole.SelectedValue);
                                            userCmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked);
                                            userCmd.Parameters.AddWithValue("@MustChangePassword", chkMustChangePassword.Checked);
                                            userCmd.Parameters.AddWithValue("@UserId", userId);
                                            userCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }

                            // Log the activity
                            LogActivity("Employee Profile Updated", "Employee",
                                $"Updated profile for {txtFirstName.Text} {txtLastName.Text} (ID: {employeeId})", employeeId, conn, transaction);

                            transaction.Commit();
                            ShowMessage("Employee profile updated successfully!", "success");

                            // Close modal and reload grid
                            pnlEmployeeModal.Visible = false;
                            ViewState["EditingEmployeeId"] = null;
                            LoadEmployeeGrid();
                            LoadDashboardOverview();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error saving employee profile. Please check all fields and try again.", "error");
            }
        }

        #endregion
        // Add these methods to your ManageEmployeeProfiles.aspx.cs file:

        #region Modal Event Handlers

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            if (ViewState["EditingEmployeeId"] != null)
            {
                int employeeId = Convert.ToInt32(ViewState["EditingEmployeeId"]);

                if (!CanAccessEmployee(employeeId))
                {
                    ShowMessage("Access denied. You cannot modify this employee.", "error");
                    return;
                }

                SaveEmployeeProfile(employeeId);
            }
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            pnlEmployeeModal.Visible = false;
            ViewState["EditingEmployeeId"] = null;
            ClearModalFields();
        }

        protected void btnCloseModal_Click(object sender, EventArgs e)
        {
            pnlEmployeeModal.Visible = false;
            ViewState["EditingEmployeeId"] = null;
            ClearModalFields();
        }

        protected void btnCloseActivityModal_Click(object sender, EventArgs e)
        {
            pnlActivityModal.Visible = false;
        }

        private void ClearModalFields()
        {
            // Clear all form fields
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtEmployeeNumber.Text = "";
            txtDateOfBirth.Text = "";
            ddlGender.SelectedIndex = 0;
            txtJobTitle.Text = "";
            ddlDepartment.SelectedIndex = 0;
            ddlEmployeeType.SelectedIndex = 0;
            txtHireDate.Text = "";
            ddlManager.SelectedIndex = 0;
            ddlEmployeeStatus.SelectedIndex = 0;
            txtSalary.Text = "";
            txtWorkLocation.Text = "";
            txtEmail.Text = "";
            txtPhoneNumber.Text = "";
            txtAddress.Text = "";
            txtCity.Text = "";
            txtState.Text = "";
            txtZipCode.Text = "";
            ddlUserRole.SelectedIndex = 0;
            chkIsActive.Checked = false;
            chkMustChangePassword.Checked = false;
        }

        #endregion

        #region Grid Event Handlers

        protected void gvEmployees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int employeeId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ViewProfile":
                    if (CanAccessEmployee(employeeId))
                    {
                        LoadEmployeeProfile(employeeId);
                        pnlEmployeeModal.Visible = true;

                        // Set the first tab as active using client script
                        ScriptManager.RegisterStartupScript(this, GetType(), "showFirstTab",
                            "setTimeout(function() { showTab('personal'); }, 100);", true);
                    }
                    else
                    {
                        ShowMessage("Access denied. You can only view employees in your department.", "error");
                    }
                    break;

                case "ViewActivity":
                    if (CanAccessEmployee(employeeId))
                    {
                        LoadEmployeeActivity(employeeId);
                        pnlActivityModal.Visible = true;
                    }
                    else
                    {
                        ShowMessage("Access denied.", "error");
                    }
                    break;

                case "ToggleStatus":
                    if (CanAccessEmployee(employeeId))
                    {
                        ToggleEmployeeStatus(employeeId);
                    }
                    else
                    {
                        ShowMessage("Access denied.", "error");
                    }
                    break;

                case "DeleteEmployee":
                    if (IsAdmin() && CanAccessEmployee(employeeId))
                    {
                        DeleteEmployee(employeeId);
                    }
                    else
                    {
                        ShowMessage("Access denied. Only administrators can delete employees.", "error");
                    }
                    break;
            }
        }

        protected void gvEmployees_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Hide delete button for non-admins
                LinkButton btnDelete = e.Row.FindControl("btnDeleteEmployee") as LinkButton;
                if (btnDelete != null)
                {
                    btnDelete.Visible = IsAdmin();
                }
            }
        }

        protected void gvEmployees_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEmployees.PageIndex = e.NewPageIndex;
            LoadEmployeeGrid();
        }

        #endregion
        #region Helper Methods

        //protected string GetEmployeeInitials(object firstName, object lastName)
        //{
        //    string first = firstName?.ToString() ?? "";
        //    string last = lastName?.ToString() ?? "";

        //    string firstInitial = !string.IsNullOrEmpty(first) ? first.Substring(0, 1).ToUpper() : "";
        //    string lastInitial = !string.IsNullOrEmpty(last) ? last.Substring(0, 1).ToUpper() : "";

        //    return firstInitial + lastInitial;
        //}

        //protected string GetStatusClass(object status)
        //{
        //    string statusValue = status?.ToString().ToUpper() ?? "";

        //    switch (statusValue)
        //    {
        //        case "ACTIVE":
        //            return "active";
        //        case "INACTIVE":
        //            return "inactive";
        //        case "TERMINATED":
        //            return "terminated";
        //        case "ON_LEAVE":
        //            return "on-leave";
        //        default:
        //            return "unknown";
        //    }
        //}



        private string GetTimeAgo(DateTime dateTime)
        {
            TimeSpan timeDifference = DateTime.Now - dateTime;

            if (timeDifference.TotalMinutes < 1)
                return "Just now";
            else if (timeDifference.TotalMinutes < 60)
                return $"{(int)timeDifference.TotalMinutes} minute{((int)timeDifference.TotalMinutes != 1 ? "s" : "")} ago";
            else if (timeDifference.TotalHours < 24)
                return $"{(int)timeDifference.TotalHours} hour{((int)timeDifference.TotalHours != 1 ? "s" : "")} ago";
            else if (timeDifference.TotalDays < 30)
                return $"{(int)timeDifference.TotalDays} day{((int)timeDifference.TotalDays != 1 ? "s" : "")} ago";
            else
                return dateTime.ToString("MMM dd, yyyy");
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            pnlMessage.CssClass = $"alert alert-{type}";
            litMessage.Text = message;

            // Auto-hide success messages after 5 seconds
            if (type == "success")
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "hideMessage",
                    "setTimeout(function() { var alert = document.querySelector('.alert'); if(alert) alert.style.display = 'none'; }, 5000);", true);
            }
        }

        private void LogError(Exception ex)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ErrorLogs (ErrorMessage, StackTrace, Source, Timestamp, UserId, Severity)
                        VALUES (@ErrorMessage, @StackTrace, @Source, GETDATE(), @UserId, @Severity)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message);
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", "ManageEmployeeProfiles");
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        cmd.Parameters.AddWithValue("@Severity", "Error");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // If logging fails, don't throw another exception
            }
        }

        private void LogActivity(string action, string entityType, string details, int? employeeId, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                string query = @"
                    INSERT INTO RecentActivities (UserId, EmployeeId, ActivityTypeId, Action, EntityType, Details, IPAddress, CreatedAt)
                    VALUES (@UserId, @EmployeeId, 1, @Action, @EntityType, @Details, @IPAddress, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId.HasValue ? (object)employeeId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@EntityType", entityType);
                    cmd.Parameters.AddWithValue("@Details", details);
                    cmd.Parameters.AddWithValue("@IPAddress", Request.UserHostAddress ?? "Unknown");
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // If activity logging fails, don't break the main operation
            }
        }
        protected string GetEmployeeInitials(object firstName, object lastName)
        {
            string first = firstName?.ToString() ?? "";
            string last = lastName?.ToString() ?? "";

            string firstInitial = !string.IsNullOrEmpty(first) ? first.Substring(0, 1).ToUpper() : "";
            string lastInitial = !string.IsNullOrEmpty(last) ? last.Substring(0, 1).ToUpper() : "";

            return firstInitial + lastInitial;
        }

        protected string GetStatusClass(object status)
        {
            string statusValue = status?.ToString().ToUpper() ?? "";

            switch (statusValue)
            {
                case "ACTIVE":
                    return "active";
                case "INACTIVE":
                    return "inactive";
                case "TERMINATED":
                    return "terminated";
                case "ON_LEAVE":
                    return "on-leave";
                default:
                    return "unknown";
            }
        }

        protected string GetAccessStatusClass(object isActive)
        {
            bool active = Convert.ToBoolean(isActive ?? false);
            return active ? "access-active" : "access-inactive";
        }
        #endregion
    }
}