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
                LoadEmployeeList();
                SetActiveTab("personal");

                // Check for success message
                if (Request.QueryString["message"] != null)
                {
                    ShowMessage(Request.QueryString["message"], "success");
                }
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

            // Configure field access based on role
            if (isProgramDirector && !isAdmin)
            {
                // Program Directors cannot change departments or certain sensitive fields
                ddlDepartment.Enabled = false;
                txtSalary.Enabled = false;
                ddlUserRole.Enabled = false;
                btnDeleteEmployee.Visible = false;
            }
        }

        private bool IsAdmin()
        {
            string currentRole = CurrentUserRole;
            bool isAdmin = currentRole == "ADMIN" || currentRole == "HRADMIN" || currentRole == "SUPERADMIN";

            // Debug logging - you can remove this after testing
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string debugQuery = @"
                        INSERT INTO ErrorLogs (ErrorMessage, StackTrace, Source, Timestamp, UserId, Severity)
                        VALUES (@ErrorMessage, @StackTrace, @Source, GETDATE(), @UserId, @Severity)";

                    using (SqlCommand cmd = new SqlCommand(debugQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", $"Role Check - CurrentRole: {currentRole}, IsAdmin: {isAdmin}");
                        cmd.Parameters.AddWithValue("@StackTrace", "Role Debug");
                        cmd.Parameters.AddWithValue("@Source", "ManageEmployeeProfiles-RoleCheck");
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        cmd.Parameters.AddWithValue("@Severity", "Info");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { } // Don't let debug logging break the app

            return isAdmin;
        }

        private bool IsProgramDirector()
        {
            return CurrentUserRole == "PROGRAMDIRECTOR";
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
                        return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
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
            // Debug: Log the role check
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
                // For safety, admins should still get access even if there's an error
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
                                litManagerName.Text = reader["FullName"]?.ToString() ?? "Manager";
                                litDepartment.Text = reader["DepartmentName"]?.ToString() ?? "Department";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void LoadDashboardOverview()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Build department filter for Program Directors
                    string departmentFilter = "";
                    if (IsProgramDirector() && !IsAdmin())
                    {
                        departmentFilter = "AND DepartmentId = @UserDepartmentId";
                    }

                    // Total Employees
                    string totalQuery = $"SELECT COUNT(*) FROM Employees WHERE IsActive = 1 {departmentFilter}";
                    using (SqlCommand cmd = new SqlCommand(totalQuery, conn))
                    {
                        if (IsProgramDirector() && !IsAdmin())
                            cmd.Parameters.AddWithValue("@UserDepartmentId", GetUserDepartmentId());
                        litTotalEmployees.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Active Employees
                    string activeQuery = $"SELECT COUNT(*) FROM Employees WHERE IsActive = 1 AND Status = 'Active' {departmentFilter}";
                    using (SqlCommand cmd = new SqlCommand(activeQuery, conn))
                    {
                        if (IsProgramDirector() && !IsAdmin())
                            cmd.Parameters.AddWithValue("@UserDepartmentId", GetUserDepartmentId());
                        litActiveEmployees.Text = cmd.ExecuteScalar().ToString();
                    }

                    // New Hires (30 days)
                    string newHiresQuery = $@"
                        SELECT COUNT(*) FROM Employees 
                        WHERE IsActive = 1 AND HireDate >= DATEADD(day, -30, GETDATE()) {departmentFilter}";
                    using (SqlCommand cmd = new SqlCommand(newHiresQuery, conn))
                    {
                        if (IsProgramDirector() && !IsAdmin())
                            cmd.Parameters.AddWithValue("@UserDepartmentId", GetUserDepartmentId());
                        litNewHires.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Completed Profiles
                    string completedQuery = $@"
                        SELECT COUNT(*) FROM Employees 
                        WHERE IsActive = 1 
                        AND FirstName IS NOT NULL 
                        AND LastName IS NOT NULL 
                        AND Email IS NOT NULL 
                        AND JobTitle IS NOT NULL 
                        AND DepartmentId IS NOT NULL {departmentFilter}";
                    using (SqlCommand cmd = new SqlCommand(completedQuery, conn))
                    {
                        if (IsProgramDirector() && !IsAdmin())
                            cmd.Parameters.AddWithValue("@UserDepartmentId", GetUserDepartmentId());
                        litCompletedProfiles.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Department Count
                    if (IsAdmin())
                    {
                        string deptQuery = "SELECT COUNT(*) FROM Departments WHERE IsActive = 1";
                        using (SqlCommand cmd = new SqlCommand(deptQuery, conn))
                        {
                            litDepartmentCount.Text = cmd.ExecuteScalar().ToString();
                        }
                    }
                    else
                    {
                        litDepartmentCount.Text = "1"; // Program Director sees only their department
                    }

                    // Employee count for header
                    litEmployeeCount.Text = litTotalEmployees.Text;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading dashboard overview.", "error");
            }
        }

        private void LoadDropdownData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Load Departments
                    LoadDepartments(conn);

                    // Load Managers
                    LoadManagers(conn);

                    // Load Department Filter
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

            // Program Directors can only assign to their department
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

            // Program Directors see managers within their department
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

            // Program Directors see only their department in filter
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

        private void LoadEmployeeList()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    StringBuilder query = new StringBuilder(@"
                        SELECT DISTINCT
                            e.Id,
                            e.FirstName + ' ' + e.LastName + ' (' + e.EmployeeNumber + ')' as DisplayName,
                            e.FirstName,
                            e.LastName,
                            e.EmployeeNumber,
                            e.Email,
                            d.Name as DepartmentName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
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

                        if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@StatusFilter", ddlStatusFilter.SelectedValue);
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlEmployeeSelect.Items.Clear();
                            ddlEmployeeSelect.Items.Add(new ListItem("Choose an employee...", ""));

                            while (reader.Read())
                            {
                                ddlEmployeeSelect.Items.Add(new ListItem(
                                    reader["DisplayName"].ToString(),
                                    reader["Id"].ToString()
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading employee list.", "error");
            }
        }

        private void LoadEmployeeProfile(int employeeId)
        {
            // Debug the access check
            string debugInfo = $"LoadEmployeeProfile called - EmployeeId: {employeeId}, CurrentRole: {CurrentUserRole}, IsAdmin: {IsAdmin()}";

            // Check access permissions
            if (!CanAccessEmployee(employeeId))
            {
                // Enhanced error message with debug info
                ShowMessage($"Access denied. You can only view employees in your department. Debug: {debugInfo}", "error");
                return;
            }

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

                                // Show profile panel
                                pnlEmployeeProfile.Visible = true;
                                pnlRecentActivity.Visible = true;

                                // Load recent activity
                                LoadRecentActivity(employeeId);

                                ShowMessage($"Profile loaded successfully for {reader["FirstName"]} {reader["LastName"]}", "success");
                            }
                            else
                            {
                                ShowMessage("Employee not found or access denied.", "error");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage($"Error loading employee profile: {ex.Message}", "error");
            }
        }

        private void LoadRecentActivity(int employeeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT TOP 10
                            epa.FieldName,
                            epa.OldValue,
                            epa.NewValue,
                            epa.ChangedDate,
                            e.FirstName + ' ' + e.LastName as ChangedByName
                        FROM EmployeeProfileAudit epa
                        LEFT JOIN Employees e ON epa.ChangedBy = e.Id
                        WHERE epa.EmployeeId = @EmployeeId
                        ORDER BY epa.ChangedDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                rptRecentActivity.DataSource = dt;
                                rptRecentActivity.DataBind();
                                pnlNoActivity.Visible = false;
                            }
                            else
                            {
                                pnlNoActivity.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                pnlNoActivity.Visible = true;
            }
        }

        #endregion

        #region Event Handlers

        protected void btnAddEmployee_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/HR/AddEmployee.aspx");
        }

        protected void btnExportProfiles_Click(object sender, EventArgs e)
        {
            // Export functionality can be implemented here
            ShowMessage("Export functionality will be implemented.", "info");
        }

        protected void btnRefreshList_Click(object sender, EventArgs e)
        {
            LoadEmployeeList();
            ShowMessage("Employee list refreshed.", "success");
        }

        protected void txtEmployeeSearch_TextChanged(object sender, EventArgs e)
        {
            LoadEmployeeList();
        }

        protected void ddlDepartmentFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmployeeList();
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadEmployeeList();
        }

        protected void ddlEmployeeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlEmployeeSelect.SelectedValue))
            {
                int employeeId = Convert.ToInt32(ddlEmployeeSelect.SelectedValue);
                LoadEmployeeProfile(employeeId);
            }
            else
            {
                pnlEmployeeProfile.Visible = false;
                pnlRecentActivity.Visible = false;
            }
        }

        protected void btnSaveProfile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlEmployeeSelect.SelectedValue))
            {
                int employeeId = Convert.ToInt32(ddlEmployeeSelect.SelectedValue);
                SaveEmployeeProfile(employeeId);
            }
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlEmployeeSelect.SelectedValue))
            {
                int employeeId = Convert.ToInt32(ddlEmployeeSelect.SelectedValue);
                LoadEmployeeProfile(employeeId); // Reload original data
                ShowMessage("Changes cancelled.", "info");
            }
        }

        protected void btnDeleteEmployee_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlEmployeeSelect.SelectedValue))
            {
                int employeeId = Convert.ToInt32(ddlEmployeeSelect.SelectedValue);
                DeactivateEmployee(employeeId);
            }
        }

        #endregion

        #region Tab Management

        private void SetActiveTab(string tabName)
        {
            // Reset all tabs
            btnTabPersonal.CssClass = "tab-button";
            btnTabEmployment.CssClass = "tab-button";
            btnTabContact.CssClass = "tab-button";
            btnTabSystemAccess.CssClass = "tab-button";

            // Hide all tab content
            pnlPersonalInfo.CssClass = "tab-content";
            pnlEmploymentInfo.CssClass = "tab-content";
            pnlContactInfo.CssClass = "tab-content";
            pnlSystemAccess.CssClass = "tab-content";

            // Set active tab and show content
            switch (tabName.ToLower())
            {
                case "personal":
                    btnTabPersonal.CssClass = "tab-button active";
                    pnlPersonalInfo.CssClass = "tab-content active";
                    break;
                case "employment":
                    btnTabEmployment.CssClass = "tab-button active";
                    pnlEmploymentInfo.CssClass = "tab-content active";
                    break;
                case "contact":
                    btnTabContact.CssClass = "tab-button active";
                    pnlContactInfo.CssClass = "tab-content active";
                    break;
                case "system":
                    btnTabSystemAccess.CssClass = "tab-button active";
                    pnlSystemAccess.CssClass = "tab-content active";
                    break;
            }
        }

        protected void btnTabPersonal_Click(object sender, EventArgs e)
        {
            SetActiveTab("personal");
        }

        protected void btnTabEmployment_Click(object sender, EventArgs e)
        {
            SetActiveTab("employment");
        }

        protected void btnTabContact_Click(object sender, EventArgs e)
        {
            SetActiveTab("contact");
        }

        protected void btnTabSystemAccess_Click(object sender, EventArgs e)
        {
            SetActiveTab("system");
        }

        #endregion

        #region Database Operations

        private void SaveEmployeeProfile(int employeeId)
        {
            if (!CanAccessEmployee(employeeId))
            {
                ShowMessage("Access denied. You can only edit employees in your department.", "error");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Update employee record
                            string updateQuery = @"
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

                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                            {
                                // Add parameters
                                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                                cmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text.Trim());
                                cmd.Parameters.AddWithValue("@LastName", txtLastName.Text.Trim());
                                cmd.Parameters.AddWithValue("@EmployeeNumber", txtEmployeeNumber.Text.Trim());
                                cmd.Parameters.AddWithValue("@DateOfBirth",
                                    string.IsNullOrEmpty(txtDateOfBirth.Text) ? (object)DBNull.Value : DateTime.Parse(txtDateOfBirth.Text));
                                cmd.Parameters.AddWithValue("@Gender",
                                    string.IsNullOrEmpty(ddlGender.SelectedValue) ? (object)DBNull.Value : ddlGender.SelectedValue);
                                cmd.Parameters.AddWithValue("@JobTitle", txtJobTitle.Text.Trim());
                                cmd.Parameters.AddWithValue("@DepartmentId",
                                    string.IsNullOrEmpty(ddlDepartment.SelectedValue) ? (object)DBNull.Value : Convert.ToInt32(ddlDepartment.SelectedValue));
                                cmd.Parameters.AddWithValue("@EmployeeType",
                                    string.IsNullOrEmpty(ddlEmployeeType.SelectedValue) ? (object)DBNull.Value : ddlEmployeeType.SelectedValue);
                                cmd.Parameters.AddWithValue("@HireDate",
                                    string.IsNullOrEmpty(txtHireDate.Text) ? (object)DBNull.Value : DateTime.Parse(txtHireDate.Text));
                                cmd.Parameters.AddWithValue("@ManagerId",
                                    string.IsNullOrEmpty(ddlManager.SelectedValue) ? (object)DBNull.Value : Convert.ToInt32(ddlManager.SelectedValue));
                                cmd.Parameters.AddWithValue("@Status", ddlEmployeeStatus.SelectedValue);
                                cmd.Parameters.AddWithValue("@Salary",
                                    string.IsNullOrEmpty(txtSalary.Text) ? (object)DBNull.Value : Convert.ToDecimal(txtSalary.Text));
                                cmd.Parameters.AddWithValue("@WorkLocation",
                                    string.IsNullOrEmpty(txtWorkLocation.Text) ? (object)DBNull.Value : txtWorkLocation.Text.Trim());
                                cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                                cmd.Parameters.AddWithValue("@PhoneNumber",
                                    string.IsNullOrEmpty(txtPhoneNumber.Text) ? (object)DBNull.Value : txtPhoneNumber.Text.Trim());
                                cmd.Parameters.AddWithValue("@Address",
                                    string.IsNullOrEmpty(txtAddress.Text) ? (object)DBNull.Value : txtAddress.Text.Trim());
                                cmd.Parameters.AddWithValue("@City",
                                    string.IsNullOrEmpty(txtCity.Text) ? (object)DBNull.Value : txtCity.Text.Trim());
                                cmd.Parameters.AddWithValue("@State",
                                    string.IsNullOrEmpty(txtState.Text) ? (object)DBNull.Value : txtState.Text.Trim());
                                cmd.Parameters.AddWithValue("@ZipCode",
                                    string.IsNullOrEmpty(txtZipCode.Text) ? (object)DBNull.Value : txtZipCode.Text.Trim());

                                cmd.ExecuteNonQuery();
                            }

                            // Update user account if exists and user has permission
                            if (IsAdmin() && !string.IsNullOrEmpty(ddlUserRole.SelectedValue))
                            {
                                UpdateUserAccount(employeeId, conn, transaction);
                            }

                            // Log the changes
                            LogProfileChanges(employeeId, conn, transaction);

                            transaction.Commit();
                            ShowMessage("Employee profile updated successfully.", "success");

                            // Refresh the display
                            LoadEmployeeProfile(employeeId);
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
                ShowMessage("Error saving employee profile. Please try again.", "error");
            }
        }

        private void UpdateUserAccount(int employeeId, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                // Check if user account exists
                string checkUserQuery = "SELECT UserId FROM Employees WHERE Id = @EmployeeId";
                int? userId = null;

                using (SqlCommand cmd = new SqlCommand(checkUserQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        userId = Convert.ToInt32(result);
                    }
                }

                if (userId.HasValue)
                {
                    // Update existing user
                    string updateUserQuery = @"
                        UPDATE Users SET
                            Role = @Role,
                            IsActive = @IsActive,
                            MustChangePassword = @MustChangePassword,
                            UpdatedAt = GETDATE()
                        WHERE Id = @UserId";

                    using (SqlCommand cmd = new SqlCommand(updateUserQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId.Value);
                        cmd.Parameters.AddWithValue("@Role", ddlUserRole.SelectedValue);
                        cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked);
                        cmd.Parameters.AddWithValue("@MustChangePassword", chkMustChangePassword.Checked);
                        cmd.ExecuteNonQuery();
                    }
                }
                else if (!string.IsNullOrEmpty(ddlUserRole.SelectedValue))
                {
                    // Create new user account
                    CreateUserAccount(employeeId, conn, transaction);
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        private void CreateUserAccount(int employeeId, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                // Get employee email for user account
                string getEmailQuery = "SELECT Email FROM Employees WHERE Id = @EmployeeId";
                string employeeEmail = "";

                using (SqlCommand cmd = new SqlCommand(getEmailQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    employeeEmail = cmd.ExecuteScalar()?.ToString() ?? "";
                }

                if (!string.IsNullOrEmpty(employeeEmail))
                {
                    // Generate default password (should be changed on first login)
                    string defaultPassword = "TempPass123!";
                    string salt = Guid.NewGuid().ToString();
                    string hashedPassword = HashPassword(defaultPassword, salt);

                    // Create user account
                    string createUserQuery = @"
                        INSERT INTO Users (Email, PasswordHash, Salt, Role, IsActive, MustChangePassword, CreatedAt, UpdatedAt)
                        VALUES (@Email, @PasswordHash, @Salt, @Role, @IsActive, 1, GETDATE(), GETDATE());
                        SELECT SCOPE_IDENTITY();";

                    int newUserId = 0;
                    using (SqlCommand cmd = new SqlCommand(createUserQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@Email", employeeEmail);
                        cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);
                        cmd.Parameters.AddWithValue("@Salt", salt);
                        cmd.Parameters.AddWithValue("@Role", ddlUserRole.SelectedValue);
                        cmd.Parameters.AddWithValue("@IsActive", chkIsActive.Checked);

                        newUserId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Update employee record with new UserId
                    string updateEmployeeQuery = "UPDATE Employees SET UserId = @UserId WHERE Id = @EmployeeId";
                    using (SqlCommand cmd = new SqlCommand(updateEmployeeQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@UserId", newUserId);
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        cmd.ExecuteNonQuery();
                    }

                    ShowMessage($"User account created successfully. Default password: {defaultPassword} (must be changed on first login)", "info");
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        private string HashPassword(string password, string salt)
        {
            // Simple password hashing - in production, use stronger hashing like bcrypt
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + salt));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private void LogProfileChanges(int employeeId, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                // For now, we'll log a general "Profile Updated" entry
                // In a full implementation, you would compare old vs new values for each field
                string insertAuditQuery = @"
                    INSERT INTO EmployeeProfileAudit (EmployeeId, FieldName, OldValue, NewValue, ChangedBy, ChangedDate)
                    VALUES (@EmployeeId, @FieldName, @OldValue, @NewValue, @ChangedBy, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(insertAuditQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                    cmd.Parameters.AddWithValue("@FieldName", "Profile");
                    cmd.Parameters.AddWithValue("@OldValue", "Previous Values");
                    cmd.Parameters.AddWithValue("@NewValue", "Updated Values");
                    cmd.Parameters.AddWithValue("@ChangedBy", CurrentEmployeeId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                // Don't throw - audit logging failure shouldn't stop the main operation
            }
        }

        private void DeactivateEmployee(int employeeId)
        {
            if (!CanAccessEmployee(employeeId))
            {
                ShowMessage("Access denied. You can only deactivate employees in your department.", "error");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string updateQuery = @"
                        UPDATE Employees SET
                            Status = 'Inactive',
                            IsActive = 0,
                            UpdatedAt = GETDATE()
                        WHERE Id = @EmployeeId";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ShowMessage("Employee has been deactivated.", "success");

                            // Reset the form
                            pnlEmployeeProfile.Visible = false;
                            pnlRecentActivity.Visible = false;
                            ddlEmployeeSelect.SelectedIndex = 0;

                            // Refresh the employee list and dashboard
                            LoadEmployeeList();
                            LoadDashboardOverview();
                        }
                        else
                        {
                            ShowMessage("Error deactivating employee. Please try again.", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error deactivating employee. Please try again.", "error");
            }
        }

        #endregion

        #region Utility Methods

        private void ShowMessage(string message, string messageType)
        {
            string cssClass = "";
            string icon = "";

            switch (messageType.ToLower())
            {
                case "success":
                    cssClass = "message-success";
                    icon = "check_circle";
                    break;
                case "error":
                    cssClass = "message-error";
                    icon = "error";
                    break;
                case "warning":
                    cssClass = "message-warning";
                    icon = "warning";
                    break;
                case "info":
                default:
                    cssClass = "message-info";
                    icon = "info";
                    break;
            }

            litMessage.Text = $@"
                <div class='{cssClass}'>
                    <i class='material-icons'>{icon}</i>
                    <span>{message}</span>
                </div>";

            pnlMessage.Visible = true;

            // Auto-hide success messages after 5 seconds
            if (messageType.ToLower() == "success")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "hideMessage",
                    "setTimeout(function() { document.querySelector('.message-panel').style.display = 'none'; }, 5000);", true);
            }
        }

        private void LogError(Exception ex)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string insertQuery = @"
                        INSERT INTO ErrorLogs (ErrorMessage, StackTrace, Source, Timestamp, UserId, Severity)
                        VALUES (@ErrorMessage, @StackTrace, @Source, GETDATE(), @UserId, @Severity)";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message);
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", ex.Source ?? "ManageEmployeeProfiles");
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        cmd.Parameters.AddWithValue("@Severity", "High");

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // If logging fails, we don't want to throw another exception
                // In production, you might want to log to a file or other system
            }
        }

        #endregion
    }
}