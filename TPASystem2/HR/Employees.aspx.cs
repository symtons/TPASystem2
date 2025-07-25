using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.IO;

namespace TPASystem2.HR
{
    public partial class Employees : System.Web.UI.Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
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
                            COUNT(CASE WHEN Status = 'Active' THEN 1 END) as ActiveEmployees,
                            COUNT(CASE WHEN HireDate >= DATEADD(DAY, -30, GETDATE()) AND Status = 'Active' THEN 1 END) as NewHires,
                            COUNT(DISTINCT DepartmentId) as DepartmentCount
                        FROM Employees 
                        WHERE IsActive = 1";

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
                ShowMessage($"Error loading employee statistics: {ex.Message}", "error");
            }
        }

        private void LoadEmployees()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    StringBuilder query = new StringBuilder();
                    query.Append(@"
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
                            e.HireDate,
                            e.Status,
                            e.Salary,
                            e.WorkLocation,
                            e.OnboardingStatus,
                            ISNULL(d.Name, 'No Department') as DepartmentName,
                            CASE 
                                WHEN e.ManagerId IS NOT NULL THEN m.FirstName + ' ' + m.LastName 
                                ELSE 'No Manager' 
                            END as ManagerName,
                            DATEDIFF(DAY, e.HireDate, GETDATE()) as DaysEmployed
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id  
                        LEFT JOIN Employees m ON e.ManagerId = m.Id
                        WHERE e.IsActive = 1");

                    // Add search filters
                    if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                    {
                        query.Append(@" AND (
                            e.FirstName LIKE @Search OR 
                            e.LastName LIKE @Search OR 
                            e.Email LIKE @Search OR 
                            e.EmployeeNumber LIKE @Search
                        )");
                    }

                    if (!string.IsNullOrEmpty(ddlDepartment.SelectedValue))
                    {
                        query.Append(" AND e.DepartmentId = @DepartmentId");
                    }

                    if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                    {
                        query.Append(" AND e.Status = @Status");
                    }

                    if (!string.IsNullOrEmpty(ddlEmployeeType.SelectedValue))
                    {
                        query.Append(" AND e.EmployeeType = @EmployeeType");
                    }

                    query.Append(" ORDER BY e.LastName, e.FirstName");

                    using (SqlCommand cmd = new SqlCommand(query.ToString(), conn))
                    {
                        // Add parameters
                        if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                        {
                            cmd.Parameters.AddWithValue("@Search", "%" + txtSearch.Text.Trim() + "%");
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

        #region Helper Methods

        private void ViewEmployee(int employeeId)
        {
            hdnSelectedEmployeeId.Value = employeeId.ToString();
            ShowMessage($"Viewing employee details for ID: {employeeId}", "info");
        }

        private void EditEmployee(int employeeId)
        {
            Response.Redirect($"~/HR/EditEmployee.aspx?id={employeeId}");
        }

        private void ViewOnboarding(int employeeId)
        {
            Response.Redirect($"~/HR/Onboarding.aspx?employeeId={employeeId}");
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
                            e.JobTitle,
                            e.EmployeeType,
                            e.HireDate,
                            e.Status,
                            d.Name as Department,
                            m.FirstName + ' ' + m.LastName as Manager
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        LEFT JOIN Employees m ON e.ManagerId = m.Id
                        WHERE e.IsActive = 1
                        ORDER BY e.LastName, e.FirstName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            StringBuilder csv = new StringBuilder();

                            // Add header row
                            csv.AppendLine("Employee Number,First Name,Last Name,Email,Phone,Position,Job Title,Employee Type,Hire Date,Status,Department,Manager");

                            while (reader.Read())
                            {
                                csv.AppendLine($"{reader["EmployeeNumber"]},{reader["FirstName"]},{reader["LastName"]}," +
                                             $"{reader["Email"]},{reader["PhoneNumber"]},{reader["Position"]}," +
                                             $"{reader["JobTitle"]},{reader["EmployeeType"]},{reader["HireDate"]}," +
                                             $"{reader["Status"]},{reader["Department"]},{reader["Manager"]}");
                            }

                            // Send file to browser
                            Response.Clear();
                            Response.ContentType = "text/csv";
                            Response.AddHeader("Content-Disposition", $"attachment; filename=Employees_{DateTime.Now:yyyyMMdd}.csv");
                            Response.Write(csv.ToString());
                            Response.End();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error exporting employees: {ex.Message}", "error");
            }
        }

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            litMessage.Text = message;
            pnlMessage.CssClass = $"alert alert-{type}";
        }

        #endregion

        #region Helper Methods for GridView Data Binding (PUBLIC - Required for ASPX)

        /// <summary>
        /// Gets status badge CSS class based on employee status
        /// </summary>
        public string GetStatusClass(string status)
        {
            if (string.IsNullOrEmpty(status)) return "badge-secondary";

            switch (status.ToLower())
            {
                case "active": return "badge-success";
                case "inactive": return "badge-secondary";
                case "on leave": return "badge-warning";
                case "terminated": return "badge-danger";
                default: return "badge-secondary";
            }
        }

        /// <summary>
        /// Calculates and returns tenure string based on hire date
        /// </summary>
        public string GetTenure(object hireDate)
        {
            if (hireDate == null || hireDate == DBNull.Value)
                return "N/A";

            try
            {
                DateTime hire = Convert.ToDateTime(hireDate);
                DateTime now = DateTime.Now;

                int years = now.Year - hire.Year;
                int months = now.Month - hire.Month;

                if (months < 0)
                {
                    years--;
                    months += 12;
                }

                if (years > 0)
                {
                    if (months > 0)
                        return $"{years}y {months}m";
                    else
                        return $"{years} year{(years == 1 ? "" : "s")}";
                }
                else if (months > 0)
                {
                    return $"{months} month{(months == 1 ? "" : "s")}";
                }
                else
                {
                    int days = (now - hire).Days;
                    return $"{days} day{(days == 1 ? "" : "s")}";
                }
            }
            catch
            {
                return "N/A";
            }
        }

        /// <summary>
        /// Formats salary for display
        /// </summary>
        public string FormatSalary(object salary)
        {
            if (salary == null || salary == DBNull.Value)
                return "Not Set";

            try
            {
                decimal sal = Convert.ToDecimal(salary);
                return sal.ToString("C0"); // Currency format without decimals
            }
            catch
            {
                return "Not Set";
            }
        }

        /// <summary>
        /// Gets initials for avatar fallback
        /// </summary>
        public string GetInitials(object firstName, object lastName)
        {
            string first = firstName?.ToString()?.Trim() ?? "";
            string last = lastName?.ToString()?.Trim() ?? "";

            string initials = "";
            if (!string.IsNullOrEmpty(first)) initials += first[0];
            if (!string.IsNullOrEmpty(last)) initials += last[0];

            return string.IsNullOrEmpty(initials) ? "?" : initials.ToUpper();
        }

        #endregion
    }
}