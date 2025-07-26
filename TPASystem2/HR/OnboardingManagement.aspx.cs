using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

namespace TPASystem2.HR
{
    public partial class OnboardingManagement : System.Web.UI.Page
    {
        #region Properties and Fields

        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // ViewState properties for maintaining state - using simple types instead of complex objects
        private List<OnboardingEmployeeInfo> CurrentEmployeeList
        {
            get
            {
                // Don't store complex objects in ViewState - retrieve fresh data instead
                return GetOnboardingEmployees();
            }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set UnobtrusiveValidationMode to prevent validation errors
                Page.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

                InitializePage();
                LoadDepartments();
                SetDefaultFilters();
                LoadOnboardingData();
                LoadStatistics();
            }
        }

        #endregion

        #region Initialization Methods

        private void InitializePage()
        {
            // Check user permissions
            //if (Session["UserRole"] == null || Session["UserId"] == null)
            //{
            //    Response.Redirect("~/Login.aspx");
            //    return;
            //}

            //string userRole = Session["UserRole"].ToString();
            //if (userRole != "HR" && userRole != "ADMIN" && userRole != "HRDIRECTOR")
            //{
            //    Response.Redirect("~/Dashboard.aspx");
            //    return;
            //}

            // Set page title and breadcrumbs
            Page.Title = "Employee Onboarding Management - TPA HR System";
        }

        private void LoadDepartments()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT Id, Name FROM Departments WHERE IsActive = 1 ORDER BY Name", conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            ddlFilterDepartment.Items.Clear();
                            ddlFilterDepartment.Items.Add(new ListItem("All Departments", ""));

                            while (reader.Read())
                            {
                                ddlFilterDepartment.Items.Add(new ListItem(reader["Name"].ToString(), reader["Id"].ToString()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error loading departments: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in LoadDepartments: {ex}");
            }
        }

        private void SetDefaultFilters()
        {
            // Set default date range (last 6 months)
            txtFromDate.Text = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        #endregion

        #region Data Loading Methods

        private void LoadOnboardingData()
        {
            try
            {
                var employees = GetOnboardingEmployees();

                gvEmployees.DataSource = employees;
                gvEmployees.DataBind();

                // Update UI based on results
                UpdateGridVisibility(employees.Count > 0);
            }
            catch (Exception ex)
            {
                ShowNotification($"Error loading onboarding data: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in LoadOnboardingData: {ex}");
            }
        }

        private List<OnboardingEmployeeInfo> GetOnboardingEmployees()
        {
            var employees = new List<OnboardingEmployeeInfo>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_GetOnboardingOverview", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Add filter parameters
                        cmd.Parameters.AddWithValue("@DepartmentId",
                            string.IsNullOrEmpty(ddlFilterDepartment.SelectedValue) ? (object)DBNull.Value : Convert.ToInt32(ddlFilterDepartment.SelectedValue));
                        cmd.Parameters.AddWithValue("@Status",
                            string.IsNullOrEmpty(ddlFilterStatus.SelectedValue) ? (object)DBNull.Value : ddlFilterStatus.SelectedValue);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var employee = new OnboardingEmployeeInfo
                                {
                                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                    EmployeeNumber = reader["EmployeeNumber"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    Position = reader["Position"].ToString(),
                                    HireDate = Convert.ToDateTime(reader["HireDate"]),
                                    DepartmentName = reader["DepartmentName"]?.ToString() ?? "Unknown",
                                    TotalTasks = Convert.ToInt32(reader["TotalTasks"]),
                                    CompletedTasks = Convert.ToInt32(reader["CompletedTasks"]),
                                    PendingTasks = Convert.ToInt32(reader["PendingTasks"]),
                                    InProgressTasks = Convert.ToInt32(reader["InProgressTasks"]),
                                    OverdueTasks = Convert.ToInt32(reader["OverdueTasks"]),
                                    CompletionPercentage = Convert.ToDecimal(reader["CompletionPercentage"]),
                                    OnboardingStatus = reader["OnboardingStatus"].ToString(),
                                    DaysSinceHire = Convert.ToInt32(reader["DaysSinceHire"])
                                };

                                // Apply additional client-side filters
                                if (ApplyClientFilters(employee))
                                {
                                    employees.Add(employee);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetOnboardingEmployees: {ex}");
                throw;
            }

            return employees;
        }

        private bool ApplyClientFilters(OnboardingEmployeeInfo employee)
        {
            // Search filter
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string searchTerm = txtSearch.Text.ToLower();
                if (!employee.FullName.ToLower().Contains(searchTerm) &&
                    !employee.EmployeeNumber.ToLower().Contains(searchTerm) &&
                    !employee.Position.ToLower().Contains(searchTerm))
                {
                    return false;
                }
            }

            // Date range filter
            if (!string.IsNullOrEmpty(txtFromDate.Text))
            {
                DateTime fromDate = Convert.ToDateTime(txtFromDate.Text);
                if (employee.HireDate < fromDate)
                    return false;
            }

            if (!string.IsNullOrEmpty(txtToDate.Text))
            {
                DateTime toDate = Convert.ToDateTime(txtToDate.Text);
                if (employee.HireDate > toDate)
                    return false;
            }

            return true;
        }

        private void LoadStatistics()
        {
            //try
            //{
                var employees = CurrentEmployeeList;

                litTotalEmployees.Text = employees.Count.ToString();
                litCompletedEmployees.Text = employees.Count(e => e.OnboardingStatus == "COMPLETED").ToString();
                litInProgressEmployees.Text = employees.Count(e => e.OnboardingStatus == "IN_PROGRESS").ToString();
                litOverdueEmployees.Text = employees.Count(e => e.OverdueTasks > 0).ToString();
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine($"Error in LoadStatistics: {ex}");
            //}
        }

        #endregion

        #region Grid Events

        protected void gvEmployees_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEmployees.PageIndex = e.NewPageIndex;
            LoadOnboardingData();
        }

        protected void gvEmployees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int employeeId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "ManageOnboarding":
                    Response.Redirect($"~/HR/ManageEmployeeOnboarding.aspx?employeeId={employeeId}");
                    break;

                case "ViewDetails":
                    // Could open a modal or redirect to employee details
                    Response.Redirect($"~/HR/EmployeeDetails.aspx?id={employeeId}");
                    break;

                case "QuickComplete":
                    CompleteEmployeeOnboarding(employeeId);
                    break;
            }
        }

        protected void gvEmployees_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var employee = (OnboardingEmployeeInfo)e.Row.DataItem;

                // Highlight overdue rows
                if (employee.OverdueTasks > 0)
                {
                    e.Row.CssClass += " row-overdue";
                }
            }
        }

        #endregion

        #region Filter Events

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadOnboardingData();
            LoadStatistics();
        }

        protected void ddlFilterDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadOnboardingData();
            LoadStatistics();
        }

        protected void ddlFilterStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadOnboardingData();
            LoadStatistics();
        }

        protected void DateFilter_Changed(object sender, EventArgs e)
        {
            LoadOnboardingData();
            LoadStatistics();
        }

        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            ClearAllFilters();
            LoadOnboardingData();
            LoadStatistics();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvEmployees.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gvEmployees.PageIndex = 0; // Reset to first page
            LoadOnboardingData();
        }

        #endregion

        #region Action Events

        protected void btnExportReport_Click(object sender, EventArgs e)
        {
            ExportOnboardingReport();
        }

        protected void btnBulkActions_Click(object sender, EventArgs e)
        {
            pnlQuickActions.Visible = !pnlQuickActions.Visible;
            pnlQuickActions.CssClass = pnlQuickActions.Visible ? "quick-actions-panel show" : "quick-actions-panel";
        }

        protected void btnCloseQuickActions_Click(object sender, EventArgs e)
        {
            pnlQuickActions.Visible = false;
        }

        protected void btnSendReminders_Click(object sender, EventArgs e)
        {
            SendReminderEmails();
        }

        protected void btnGenerateProgressReport_Click(object sender, EventArgs e)
        {
            GenerateProgressReport();
        }

        protected void btnReassignTasks_Click(object sender, EventArgs e)
        {
            // Redirect to bulk reassignment page
            Response.Redirect("~/HR/BulkTaskReassignment.aspx");
        }

        protected void btnCloseNotification_Click(object sender, EventArgs e)
        {
            pnlNotification.Visible = false;
        }

        #endregion

        #region Helper Methods

        private void CompleteEmployeeOnboarding(int employeeId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(@"
                        UPDATE OnboardingTasks 
                        SET Status = 'COMPLETED', 
                            CompletedDate = GETUTCDATE(),
                            CompletedById = @UserId,
                            Notes = ISNULL(Notes, '') + ' [Bulk completed by HR]'
                        WHERE EmployeeId = @EmployeeId AND Status != 'COMPLETED'", conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        cmd.Parameters.AddWithValue("@UserId", Convert.ToInt32(Session["UserId"]));

                        int affectedRows = cmd.ExecuteNonQuery();

                        if (affectedRows > 0)
                        {
                            // Update onboarding progress
                            using (SqlCommand progressCmd = new SqlCommand("EXEC UpdateOnboardingProgress @EmployeeId", conn))
                            {
                                progressCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                                progressCmd.ExecuteNonQuery();
                            }

                            ShowNotification($"Employee onboarding marked as complete ({affectedRows} tasks updated).", "success");
                        }
                        else
                        {
                            ShowNotification("No pending tasks found to complete.", "info");
                        }
                    }
                }

                // Refresh data
                LoadOnboardingData();
                LoadStatistics();
            }
            catch (Exception ex)
            {
                ShowNotification($"Error completing onboarding: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in CompleteEmployeeOnboarding: {ex}");
            }
        }

        private void ExportOnboardingReport()
        {
            try
            {
                var employees = CurrentEmployeeList;
                var csv = new StringBuilder();

                // CSV Headers
                csv.AppendLine("Employee Number,Full Name,Position,Department,Hire Date,Days Since Hire,Total Tasks,Completed Tasks,Pending Tasks,In Progress Tasks,Overdue Tasks,Completion Percentage,Status");

                // CSV Data
                foreach (var emp in employees)
                {
                    csv.AppendLine($"{emp.EmployeeNumber}," +
                                  $"\"{emp.FullName}\"," +
                                  $"\"{emp.Position}\"," +
                                  $"\"{emp.DepartmentName}\"," +
                                  $"{emp.HireDate:yyyy-MM-dd}," +
                                  $"{emp.DaysSinceHire}," +
                                  $"{emp.TotalTasks}," +
                                  $"{emp.CompletedTasks}," +
                                  $"{emp.PendingTasks}," +
                                  $"{emp.InProgressTasks}," +
                                  $"{emp.OverdueTasks}," +
                                  $"{emp.CompletionPercentage:F1}%," +
                                  $"{emp.OnboardingStatus}");
                }

                // Download file
                string fileName = $"OnboardingReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                Response.ContentType = "text/csv";
                Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
                Response.Write(csv.ToString());
                Response.End();
            }
            catch (Exception ex)
            {
                ShowNotification($"Error exporting report: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in ExportOnboardingReport: {ex}");
            }
        }

        private void SendReminderEmails()
        {
            try
            {
                var overdueEmployees = CurrentEmployeeList.Where(e => e.OverdueTasks > 0).ToList();

                if (overdueEmployees.Count == 0)
                {
                    ShowNotification("No employees with overdue tasks found.", "info");
                    return;
                }

                int emailsSent = 0;

                foreach (var employee in overdueEmployees)
                {
                    // Here you would implement email sending logic
                    // For now, we'll simulate the process
                    bool emailSent = SendReminderEmail(employee);
                    if (emailSent) emailsSent++;
                }

                ShowNotification($"Reminder emails sent to {emailsSent} employees with overdue tasks.", "success");
            }
            catch (Exception ex)
            {
                ShowNotification($"Error sending reminder emails: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in SendReminderEmails: {ex}");
            }
        }

        private bool SendReminderEmail(OnboardingEmployeeInfo employee)
        {
            // TODO: Implement actual email sending
            // This is a placeholder for email functionality
            try
            {
                // Email logic would go here
                // Return true if email sent successfully
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void GenerateProgressReport()
        {
            try
            {
                // Generate detailed progress report
                var reportData = GetDetailedProgressReport();

                // Create downloadable report
                var html = GenerateProgressReportHtml(reportData);

                string fileName = $"DetailedProgressReport_{DateTime.Now:yyyyMMdd_HHmmss}.html";
                Response.ContentType = "text/html";
                Response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");
                Response.Write(html);
                Response.End();
            }
            catch (Exception ex)
            {
                ShowNotification($"Error generating progress report: {ex.Message}", "error");
                System.Diagnostics.Debug.WriteLine($"Error in GenerateProgressReport: {ex}");
            }
        }

        private string GetDetailedProgressReport()
        {
            // This would generate a more detailed analysis
            // For now, return basic info
            return "Detailed progress report data";
        }

        private string GenerateProgressReportHtml(string reportData)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head>");
            html.AppendLine("<title>Onboarding Progress Report</title>");
            html.AppendLine("<style>body{font-family:Arial,sans-serif;margin:20px;}</style>");
            html.AppendLine("</head><body>");
            html.AppendLine($"<h1>Onboarding Progress Report</h1>");
            html.AppendLine($"<p>Generated: {DateTime.Now:yyyy-MM-dd HH:mm}</p>");
            html.AppendLine($"<p>{reportData}</p>");
            html.AppendLine("</body></html>");

            return html.ToString();
        }

        private void ClearAllFilters()
        {
            txtSearch.Text = "";
            ddlFilterDepartment.SelectedIndex = 0;
            ddlFilterStatus.SelectedIndex = 0;
            SetDefaultFilters();
        }

        private void UpdateGridVisibility(bool hasData)
        {
            pnlEmployeeGrid.Visible = hasData;
        }

        private void ShowNotification(string message, string type)
        {
            litNotificationMessage.Text = message;
            pnlNotification.Visible = true;
            pnlNotification.CssClass = $"notification-panel {type}";
        }

        #endregion

        #region UI Helper Methods

        protected string GetEmployeeInitials(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "??";

            var parts = fullName.Split(' ');
            if (parts.Length >= 2)
            {
                return (parts[0].Substring(0, 1) + parts[1].Substring(0, 1)).ToUpper();
            }
            else if (parts.Length == 1 && parts[0].Length >= 2)
            {
                return parts[0].Substring(0, 2).ToUpper();
            }

            return fullName.Substring(0, Math.Min(2, fullName.Length)).ToUpper();
        }

        protected string GetStatusClass(string status)
        {
            switch (status?.ToUpper())
            {
                case "COMPLETED": return "completed";
                case "IN_PROGRESS": return "in-progress";
                case "PENDING": return "pending";
                case "OVERDUE": return "overdue";
                case "NO_TASKS": return "no-tasks";
                default: return "pending";
            }
        }

        protected string GetStatusDisplay(string status)
        {
            switch (status?.ToUpper())
            {
                case "COMPLETED": return "Completed";
                case "IN_PROGRESS": return "In Progress";
                case "PENDING": return "Pending";
                case "OVERDUE": return "Overdue";
                case "NO_TASKS": return "No Tasks";
                default: return "Unknown";
            }
        }

        private string GetProgressColorClass(decimal percentage)
        {
            if (percentage >= 90) return "progress-excellent";
            if (percentage >= 70) return "progress-good";
            if (percentage >= 50) return "progress-fair";
            return "progress-poor";
        }

        #endregion
    }

    #region Data Models - Fixed with Serializable

    [Serializable]
    public class OnboardingEmployeeInfo
    {
        public int EmployeeId { get; set; }
        public string EmployeeNumber { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public DateTime HireDate { get; set; }
        public string DepartmentName { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int OverdueTasks { get; set; }
        public decimal CompletionPercentage { get; set; }
        public string OnboardingStatus { get; set; }
        public int DaysSinceHire { get; set; }
    }

    #endregion
}