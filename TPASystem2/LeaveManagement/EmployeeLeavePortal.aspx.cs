using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace TPASystem2.LeaveManagement
{
    public partial class EmployeeLeavePortal : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private int CurrentUserId => Convert.ToInt32(Session["UserId"] ?? 0);
        private int CurrentEmployeeId => Convert.ToInt32(Session["EmployeeId"] ?? 0);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidateUserAccess();
                LoadEmployeeInformation();
                LoadLeaveBalances();
                LoadRecentLeaveRequests();

                // Check for success message from other pages
                if (Request.QueryString["message"] != null)
                {
                    ShowMessage(Request.QueryString["message"], "success");
                }
            }
        }

        #region Employee Information Loading

        private void ValidateUserAccess()
        {
            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                return;
            }

            // Validate employee has access to leave management
            try
            {
                string userRole = Session["UserRole"]?.ToString();
                if (!string.IsNullOrEmpty(userRole))
                {
                    var allowedRoles = new[] { "EMPLOYEE", "SUPERVISOR", "PROGRAMDIRECTOR", "HRADMIN", "ADMIN", "SUPERADMIN" };
                    if (!Array.Exists(allowedRoles, role => role.Equals(userRole, StringComparison.OrdinalIgnoreCase)))
                    {
                        Response.Redirect("~/Unauthorized.aspx", false);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }

        private void LoadEmployeeInformation()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            e.FirstName + ' ' + e.LastName as FullName,
                            e.EmployeeNumber,
                            d.Name as DepartmentName,
                            ISNULL(mgr.FirstName + ' ' + mgr.LastName, 'Program Director') as SupervisorName
                        FROM Employees e
                        LEFT JOIN Departments d ON e.DepartmentId = d.Id
                        LEFT JOIN Employees mgr ON e.ManagerId = mgr.Id
                        WHERE e.UserId = @UserId AND e.IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                litEmployeeName.Text = reader["FullName"].ToString();
                                litEmployeeNumber.Text = reader["EmployeeNumber"].ToString();
                                litDepartment.Text = reader["DepartmentName"].ToString();
                                litSupervisor.Text = reader["SupervisorName"].ToString();

                                // Store employee ID in session for later use
                                if (Session["EmployeeId"] == null)
                                {
                                    Session["EmployeeId"] = GetEmployeeIdFromUserId(CurrentUserId);
                                }
                            }
                            else
                            {
                                ShowMessage("Employee information not found. Please contact HR.", "error");
                                return;
                            }
                        }
                    }
                }

                // Set leave year
                litLeaveYear.Text = DateTime.Now.Year.ToString();
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error loading employee information.", "error");
            }
        }

        private int GetEmployeeIdFromUserId(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id FROM Employees WHERE UserId = @UserId AND IsActive = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        object result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                return 0;
            }
        }

        #endregion

        #region Leave Balance Management

        private void LoadLeaveBalances()
        {
            try
            {
                int employeeId = CurrentEmployeeId;
                if (employeeId == 0)
                {
                    pnlNoBalances.Visible = true;
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            lt.TypeName as LeaveType,
                            ISNULL(lb.AllocatedDays, lt.DefaultAllocation) as Total,
                            ISNULL(lb.UsedDays, 0) as Used,
                            (ISNULL(lb.AllocatedDays, lt.DefaultAllocation) - ISNULL(lb.UsedDays, 0)) as Available
                        FROM LeaveTypes lt
                        LEFT JOIN LeaveBalances lb ON lt.Id = lb.LeaveTypeId AND lb.EmployeeId = @EmployeeId
                        WHERE lt.IsActive = 1
                        ORDER BY lt.TypeName";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                rptLeaveBalances.DataSource = dt;
                                rptLeaveBalances.DataBind();
                                pnlNoBalances.Visible = false;
                            }
                            else
                            {
                                pnlNoBalances.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                pnlNoBalances.Visible = true;
                ShowMessage("Error loading leave balances.", "error");
            }
        }

        protected string GetBalancePercentage(object available, object total)
        {
            try
            {
                decimal availableValue = Convert.ToDecimal(available ?? 0);
                decimal totalValue = Convert.ToDecimal(total ?? 0);

                if (totalValue == 0) return "0";

                decimal percentage = (availableValue / totalValue) * 100;
                return Math.Max(0, Math.Min(100, percentage)).ToString("F0");
            }
            catch
            {
                return "0";
            }
        }

        #endregion

        #region Recent Leave Requests

        private void LoadRecentLeaveRequests()
        {
            try
            {
                int employeeId = CurrentEmployeeId;
                if (employeeId == 0)
                {
                    pnlNoRequests.Visible = true;
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = BuildLeaveRequestsQuery();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                        if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
                        {
                            cmd.Parameters.AddWithValue("@Status", ddlStatusFilter.SelectedValue);
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                rptRecentRequests.DataSource = dt;
                                rptRecentRequests.DataBind();
                                pnlNoRequests.Visible = false;
                            }
                            else
                            {
                                pnlNoRequests.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                pnlNoRequests.Visible = true;
                ShowMessage("Error loading leave requests.", "error");
            }
        }

        private string BuildLeaveRequestsQuery()
        {
            string baseQuery = @"
                SELECT TOP 10
                    lr.Id,
                    lr.LeaveType,
                    lr.StartDate,
                    lr.EndDate,
                    lr.DaysRequested,
                    lr.Reason,
                    lr.Status,
                    lr.RequestedAt,
                    lr.ReviewedAt,
                    lr.WorkflowStatus
                FROM LeaveRequests lr
                WHERE lr.EmployeeId = @EmployeeId";

            if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
            {
                baseQuery += " AND lr.Status = @Status";
            }

            baseQuery += " ORDER BY lr.RequestedAt DESC";

            return baseQuery;
        }

        #endregion

        #region Event Handlers

        protected void btnRequestLeave_Click(object sender, EventArgs e)
        {
            Response.Redirect("RequestLeave.aspx", false);
        }

        protected void btnViewMyLeaves_Click(object sender, EventArgs e)
        {
            Response.Redirect("MyLeaves.aspx", false);
        }

        protected void btnViewLeaveCalendar_Click(object sender, EventArgs e)
        {
            Response.Redirect("LeaveCalendar.aspx", false);
        }

        protected void btnViewAllRequests_Click(object sender, EventArgs e)
        {
            Response.Redirect("MyLeaves.aspx", false);
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRecentLeaveRequests();
        }

        protected void rptRecentRequests_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                int requestId = Convert.ToInt32(e.CommandArgument);

                switch (e.CommandName)
                {
                    case "ViewDetails":
                        Response.Redirect($"ViewLeaveRequest.aspx?id={requestId}", false);
                        break;

                    case "Edit":
                        Response.Redirect($"RequestLeave.aspx?edit={requestId}", false);
                        break;

                    case "Cancel":
                        CancelLeaveRequest(requestId);
                        break;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error processing request.", "error");
            }
        }

        #endregion

        #region Leave Request Actions

        private void CancelLeaveRequest(int requestId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Verify ownership and status
                    string checkQuery = @"
                        SELECT Status, LeaveType 
                        FROM LeaveRequests 
                        WHERE Id = @RequestId AND EmployeeId = @EmployeeId";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@RequestId", requestId);
                        checkCmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        using (SqlDataReader reader = checkCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string status = reader["Status"].ToString();
                                string leaveType = reader["LeaveType"].ToString();

                                if (status != "Pending" && status != "Draft")
                                {
                                    ShowMessage("Only pending or draft requests can be cancelled.", "error");
                                    return;
                                }
                            }
                            else
                            {
                                ShowMessage("Leave request not found or access denied.", "error");
                                return;
                            }
                        }
                    }

                    // Update request status to cancelled
                    string updateQuery = @"
                        UPDATE LeaveRequests 
                        SET Status = 'Cancelled', 
                            WorkflowStatus = 'CANCELLED'
                        WHERE Id = @RequestId AND EmployeeId = @EmployeeId";

                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@RequestId", requestId);
                        updateCmd.Parameters.AddWithValue("@EmployeeId", CurrentEmployeeId);

                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ShowMessage("Leave request cancelled successfully.", "success");
                            LoadRecentLeaveRequests(); // Refresh the list
                        }
                        else
                        {
                            ShowMessage("Failed to cancel leave request.", "error");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                ShowMessage("Error cancelling leave request.", "error");
            }
        }

        #endregion

        #region Helper Methods

        private void ShowMessage(string message, string type)
        {
            pnlMessage.Visible = true;
            litMessage.Text = message;

            pnlMessage.CssClass = $"alert alert-{type}";
        }

        private void LogError(Exception ex)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = @"
                        INSERT INTO ErrorLogs (ErrorMessage, StackTrace, Source, RequestUrl, UserId, CreatedAt)
                        VALUES (@ErrorMessage, @StackTrace, @Source, @RequestUrl, @UserId, @CreatedAt)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ErrorMessage", ex.Message);
                        cmd.Parameters.AddWithValue("@StackTrace", ex.StackTrace ?? "");
                        cmd.Parameters.AddWithValue("@Source", ex.Source ?? "EmployeeLeavePortal");
                        cmd.Parameters.AddWithValue("@RequestUrl", Request.Url?.ToString() ?? "");
                        cmd.Parameters.AddWithValue("@UserId", CurrentUserId);
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Silent fail on logging errors
            }
        }

        #endregion
    }
}