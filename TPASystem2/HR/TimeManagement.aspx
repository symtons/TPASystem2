<%@ Page Title="Time Management" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="TimeManagement.aspx.cs" Inherits="TPASystem2.HR.TimeManagement" %>

<asp:Content ID="Content2" ContentPlaceHolderID="DashboardContent" runat="server">
    
    <!-- CSS Links -->
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    
    <div class="time-management-container">
        <!-- Page Header -->
        <div class="page-header">
            <div class="header-content">
                <h1><i class="material-icons">schedule</i>Time Management</h1>
                <p>Monitor employee time tracking, attendance, and timesheet approvals</p>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnViewReports" runat="server" Text="View Reports" 
                    CssClass="btn btn-secondary" OnClick="btnViewReports_Click" />
                <asp:Button ID="btnExportData" runat="server" Text="Export Data" 
                    CssClass="btn btn-primary" OnClick="btnExportData_Click" />
            </div>
        </div>

        <!-- Alert Messages -->
        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Real-time Status Overview -->
        <div class="status-overview">
            <div class="status-header">
                <h3><i class="material-icons">live_tv</i>Live Status Overview</h3>
                <span class="last-updated">Last updated: <asp:Label ID="lblLastUpdated" runat="server"></asp:Label></span>
            </div>
            <div class="status-grid">
                <div class="status-card clocked-in">
                    <div class="status-icon">
                        <i class="material-icons">work</i>
                    </div>
                    <div class="status-content">
                        <h4><asp:Label ID="lblClockedIn" runat="server">0</asp:Label></h4>
                        <p>Currently Clocked In</p>
                    </div>
                </div>
                <div class="status-card clocked-out">
                    <div class="status-icon">
                        <i class="material-icons">home</i>
                    </div>
                    <div class="status-content">
                        <h4><asp:Label ID="lblClockedOut" runat="server">0</asp:Label></h4>
                        <p>Clocked Out</p>
                    </div>
                </div>
                <div class="status-card on-break">
                    <div class="status-icon">
                        <i class="material-icons">coffee</i>
                    </div>
                    <div class="status-content">
                        <h4><asp:Label ID="lblOnBreak" runat="server">0</asp:Label></h4>
                        <p>On Break</p>
                    </div>
                </div>
                <div class="status-card late-arrivals">
                    <div class="status-icon">
                        <i class="material-icons">warning</i>
                    </div>
                    <div class="status-content">
                        <h4><asp:Label ID="lblLateArrivals" runat="server">0</asp:Label></h4>
                        <p>Late Arrivals Today</p>
                    </div>
                </div>
            </div>
        </div>

        <!-- Time Stats Grid -->
        <div class="stats-grid">
            <div class="stat-card">
                <div class="stat-icon attendance">
                    <i class="material-icons">check_circle</i>
                </div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblAttendanceRate" runat="server">0%</asp:Label></h3>
                    <p>Attendance Rate (This Month)</p>
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-icon overtime">
                    <i class="material-icons">access_time</i>
                </div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblOvertimeHours" runat="server">0</asp:Label></h3>
                    <p>Overtime Hours (This Week)</p>
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-icon timesheets">
                    <i class="material-icons">assignment</i>
                </div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblPendingTimesheets" runat="server">0</asp:Label></h3>
                    <p>Pending Timesheets</p>
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-icon hours">
                    <i class="material-icons">schedule</i>
                </div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblTotalHours" runat="server">0</asp:Label></h3>
                    <p>Total Hours (This Week)</p>
                </div>
            </div>
        </div>

        <!-- Filter Section -->
        <div class="filter-section">
            <div class="filter-row">
                <div class="filter-group">
                    <label for="ddlDepartment">Department:</label>
                    <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged">
                        <asp:ListItem Value="" Text="All Departments"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label for="ddlTimeFrame">Time Frame:</label>
                    <asp:DropDownList ID="ddlTimeFrame" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlTimeFrame_SelectedIndexChanged">
                        <asp:ListItem Value="today" Text="Today"></asp:ListItem>
                        <asp:ListItem Value="week" Text="This Week" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="month" Text="This Month"></asp:ListItem>
                        <asp:ListItem Value="custom" Text="Custom Range"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label for="txtStartDate">Start Date:</label>
                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                </div>
                <div class="filter-group">
                    <label for="txtEndDate">End Date:</label>
                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                </div>
            </div>
            <div class="filter-actions">
                <asp:Button ID="btnApplyFilter" runat="server" Text="Apply Filter" CssClass="btn btn-primary" OnClick="btnApplyFilter_Click" />
                <asp:Button ID="btnClearFilter" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClearFilter_Click" />
            </div>
        </div>

        <!-- Tab Navigation -->
        <div class="tab-navigation">
            <asp:Button ID="btnTabCurrent" runat="server" Text="Current Status" CssClass="tab-button active" OnClick="btnTabCurrent_Click" />
            <asp:Button ID="btnTabTimesheets" runat="server" Text="Timesheets" CssClass="tab-button" OnClick="btnTabTimesheets_Click" />
            <asp:Button ID="btnTabAttendance" runat="server" Text="Attendance" CssClass="tab-button" OnClick="btnTabAttendance_Click" />
            <asp:Button ID="btnTabSchedules" runat="server" Text="Schedules" CssClass="tab-button" OnClick="btnTabSchedules_Click" />
        </div>

        <!-- Current Status Tab -->
        <asp:Panel ID="pnlCurrentTab" runat="server" CssClass="tab-content active">
            <div class="table-container">
                <div class="table-header">
                    <div class="table-title">
                        <h3>Current Employee Status</h3>
                        <span>Real-time view of employee clock status</span>
                    </div>
                    <div class="table-actions">
                        <asp:Button ID="btnRefreshStatus" runat="server" Text="Refresh" CssClass="btn btn-sm btn-secondary" OnClick="btnRefreshStatus_Click" />
                    </div>
                </div>
                <asp:GridView ID="gvCurrentStatus" runat="server" CssClass="data-table" AutoGenerateColumns="False" 
                    EmptyDataText="No employees found">
                    <Columns>
                        <asp:TemplateField HeaderText="Employee">
                            <ItemTemplate>
                                <div class="employee-info">
                                    <div class="employee-avatar">
                                        <i class="material-icons">person</i>
                                    </div>
                                    <div class="employee-details">
                                        <div class="employee-name"><%# Eval("EmployeeName") %></div>
                                        <div class="employee-number"><%# Eval("EmployeeNumber") %></div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Department" HeaderText="Department" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <div class="time-status">
                                    <span class='<%# GetStatusClass(Eval("Status").ToString()) %>'>
                                        <%# GetStatusText(Eval("Status").ToString()) %>
                                    </span>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="ClockInTime" HeaderText="Clock In" DataFormatString="{0:h:mm tt}" />
                        <asp:BoundField DataField="HoursWorked" HeaderText="Hours Today" DataFormatString="{0:F1}" />
                        <asp:BoundField DataField="Location" HeaderText="Location" />
                        <asp:BoundField DataField="ScheduledHours" HeaderText="Scheduled" DataFormatString="{0:F1}h" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <!-- Timesheets Tab -->
        <asp:Panel ID="pnlTimesheetsTab" runat="server" CssClass="tab-content">
            <div class="table-container">
                <div class="table-header">
                    <div class="table-title">
                        <h3>Timesheet Management</h3>
                        <span>Review and approve employee timesheets</span>
                    </div>
                    <div class="table-actions">
                        <asp:Button ID="btnBulkApprove" runat="server" Text="Bulk Approve" CssClass="btn btn-sm btn-success" OnClick="btnBulkApprove_Click" />
                    </div>
                </div>
                <asp:GridView ID="gvTimesheets" runat="server" CssClass="data-table" AutoGenerateColumns="False" 
                    EmptyDataText="No timesheets found" OnRowCommand="gvTimesheets_RowCommand">
                    <Columns>
                        <asp:TemplateField HeaderText="Select">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelect" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Employee">
                            <ItemTemplate>
                                <div class="employee-info">
                                    <div class="employee-avatar">
                                        <i class="material-icons">person</i>
                                    </div>
                                    <div class="employee-details">
                                        <div class="employee-name"><%# Eval("EmployeeName") %></div>
                                        <div class="employee-number"><%# Eval("EmployeeNumber") %></div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="WeekPeriod" HeaderText="Week Period" />
                        <asp:BoundField DataField="TotalHours" HeaderText="Total Hours" DataFormatString="{0:F1}" />
                        <asp:BoundField DataField="RegularHours" HeaderText="Regular" DataFormatString="{0:F1}" />
                        <asp:BoundField DataField="OvertimeHours" HeaderText="Overtime" DataFormatString="{0:F1}" />
                        <asp:BoundField DataField="SubmittedAt" HeaderText="Submitted" DataFormatString="{0:MM/dd/yyyy}" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='<%# GetTimesheetStatusClass(Eval("Status").ToString()) %>'>
                                    <%# Eval("Status") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <div class="action-buttons">
                                    <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn btn-sm btn-outline-primary" 
                                        CommandName="ViewTimesheet" CommandArgument='<%# Eval("Id") %>' />
                                    <asp:Button ID="btnApprove" runat="server" Text="Approve" CssClass="btn btn-sm btn-outline-success" 
                                        CommandName="ApproveTimesheet" CommandArgument='<%# Eval("Id") %>' 
                                        Visible='<%# Eval("Status").ToString() == "Submitted" %>' />
                                    <asp:Button ID="btnReject" runat="server" Text="Reject" CssClass="btn btn-sm btn-outline-danger" 
                                        CommandName="RejectTimesheet" CommandArgument='<%# Eval("Id") %>' 
                                        Visible='<%# Eval("Status").ToString() == "Submitted" %>' />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <!-- Attendance Tab -->
        <asp:Panel ID="pnlAttendanceTab" runat="server" CssClass="tab-content">
            <div class="table-container">
                <div class="table-header">
                    <div class="table-title">
                        <h3>Attendance Overview</h3>
                        <span>Track employee attendance patterns and issues</span>
                    </div>
                </div>
                <asp:GridView ID="gvAttendance" runat="server" CssClass="data-table" AutoGenerateColumns="False" 
                    EmptyDataText="No attendance data found">
                    <Columns>
                        <asp:TemplateField HeaderText="Employee">
                            <ItemTemplate>
                                <div class="employee-info">
                                    <div class="employee-avatar">
                                        <i class="material-icons">person</i>
                                    </div>
                                    <div class="employee-details">
                                        <div class="employee-name"><%# Eval("EmployeeName") %></div>
                                        <div class="employee-number"><%# Eval("EmployeeNumber") %></div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Department" HeaderText="Department" />
                        <asp:BoundField DataField="DaysPresent" HeaderText="Days Present" />
                        <asp:BoundField DataField="DaysAbsent" HeaderText="Days Absent" />
                        <asp:BoundField DataField="AttendanceRate" HeaderText="Attendance Rate" DataFormatString="{0:P1}" />
                        <asp:BoundField DataField="LateArrivals" HeaderText="Late Arrivals" />
                        <asp:BoundField DataField="EarlyDepartures" HeaderText="Early Departures" />
                        <asp:BoundField DataField="TotalHours" HeaderText="Total Hours" DataFormatString="{0:F1}" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <!-- Schedules Tab -->
        <asp:Panel ID="pnlSchedulesTab" runat="server" CssClass="tab-content">
            <div class="table-container">
                <div class="table-header">
                    <div class="table-title">
                        <h3>Employee Schedules</h3>
                        <span>View and manage employee work schedules</span>
                    </div>
                    <div class="table-actions">
                        <asp:Button ID="btnManageSchedules" runat="server" Text="Manage Schedules" CssClass="btn btn-sm btn-primary" OnClick="btnManageSchedules_Click" />
                    </div>
                </div>
                <asp:GridView ID="gvSchedules" runat="server" CssClass="data-table" AutoGenerateColumns="False" 
                    EmptyDataText="No schedules found">
                    <Columns>
                        <asp:TemplateField HeaderText="Employee">
                            <ItemTemplate>
                                <div class="employee-info">
                                    <div class="employee-avatar">
                                        <i class="material-icons">person</i>
                                    </div>
                                    <div class="employee-details">
                                        <div class="employee-name"><%# Eval("EmployeeName") %></div>
                                        <div class="employee-number"><%# Eval("EmployeeNumber") %></div>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Department" HeaderText="Department" />
                        <asp:BoundField DataField="Monday" HeaderText="Monday" />
                        <asp:BoundField DataField="Tuesday" HeaderText="Tuesday" />
                        <asp:BoundField DataField="Wednesday" HeaderText="Wednesday" />
                        <asp:BoundField DataField="Thursday" HeaderText="Thursday" />
                        <asp:BoundField DataField="Friday" HeaderText="Friday" />
                        <asp:BoundField DataField="WeeklyHours" HeaderText="Weekly Hours" DataFormatString="{0:F1}" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>
    </div>

    <script type="text/javascript">
        // Auto-refresh current status every 30 seconds
        setInterval(function() {
            if (document.querySelector('.tab-button.active').textContent.trim() === 'Current Status') {
                __doPostBack('<%= btnRefreshStatus.UniqueID %>', '');
            }
        }, 30000);

        // Update last updated timestamp
        function updateLastUpdated() {
            var now = new Date();
            var timeString = now.toLocaleTimeString();
            var element = document.querySelector('.last-updated');
            if (element) {
                element.innerHTML = 'Last updated: ' + timeString;
            }
        }

        // Call on page load
        document.addEventListener('DOMContentLoaded', function() {
            updateLastUpdated();
        });
    </script>

</asp:Content>