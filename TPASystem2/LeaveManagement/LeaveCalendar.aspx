<%@ Page Title="Leave Calendar" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="LeaveCalendar.aspx.cs" Inherits="TPASystem2.LeaveManagement.LeaveCalendar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    
    <!-- Page Header -->
    <div class="page-header">
        <div class="page-title-wrapper">
            <h1 class="page-title">
                <i class="material-icons">event</i>
                Leave Calendar
            </h1>
            <p class="page-subtitle">View leave schedules and availability</p>
        </div>
        <div class="page-actions">
            <asp:Button ID="btnRequestLeave" runat="server" Text="Request Leave" 
                       CssClass="btn btn-primary waves-effect waves-light" 
                       OnClick="btnRequestLeave_Click" />
            <asp:Button ID="btnBackToDashboard" runat="server" Text="Back to Dashboard" 
                       CssClass="btn btn-outline waves-effect waves-light" 
                       OnClick="btnBackToDashboard_Click" />
        </div>
    </div>

    <!-- Message Panel -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert-panel">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Calendar Controls -->
    <div class="content-card">
        <div class="card-header">
            <h3>Calendar View</h3>
        </div>
        <div class="card-content">
            <div class="calendar-controls">
                <div class="calendar-navigation">
                    <asp:Button ID="btnPrevMonth" runat="server" Text="‹ Previous" 
                               CssClass="btn btn-outline btn-sm" 
                               OnClick="btnPrevMonth_Click" />
                    <div class="current-month">
                        <asp:Literal ID="litCurrentMonth" runat="server"></asp:Literal>
                    </div>
                    <asp:Button ID="btnNextMonth" runat="server" Text="Next ›" 
                               CssClass="btn btn-outline btn-sm" 
                               OnClick="btnNextMonth_Click" />
                </div>
                
                <div class="calendar-filters">
                    <div class="filter-group">
                        <label>View Type</label>
                        <asp:DropDownList ID="ddlViewType" runat="server" CssClass="form-control" 
                                          AutoPostBack="true" OnSelectedIndexChanged="ddlViewType_SelectedIndexChanged">
                            <asp:ListItem Value="my" Selected="true">My Leaves</asp:ListItem>
                            <asp:ListItem Value="team">Team Leaves</asp:ListItem>
                            <asp:ListItem Value="department">Department Leaves</asp:ListItem>
                            <asp:ListItem Value="all">All Leaves</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    
                    <div class="filter-group">
                        <label>Department</label>
                        <asp:DropDownList ID="ddlDepartmentFilter" runat="server" CssClass="form-control" 
                                          AutoPostBack="true" OnSelectedIndexChanged="ddlDepartmentFilter_SelectedIndexChanged">
                            <asp:ListItem Value="">All Departments</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    
                    <div class="filter-group">
                        <label>Leave Type</label>
                        <asp:DropDownList ID="ddlLeaveTypeFilter" runat="server" CssClass="form-control" 
                                          AutoPostBack="true" OnSelectedIndexChanged="ddlLeaveTypeFilter_SelectedIndexChanged">
                            <asp:ListItem Value="">All Types</asp:ListItem>
                            <asp:ListItem Value="Vacation">Vacation</asp:ListItem>
                            <asp:ListItem Value="Sick">Sick Leave</asp:ListItem>
                            <asp:ListItem Value="Personal">Personal Leave</asp:ListItem>
                            <asp:ListItem Value="Maternity">Maternity Leave</asp:ListItem>
                            <asp:ListItem Value="Paternity">Paternity Leave</asp:ListItem>
                            <asp:ListItem Value="Bereavement">Bereavement Leave</asp:ListItem>
                            <asp:ListItem Value="Emergency">Emergency Leave</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Calendar Display -->
    <div class="content-card">
        <div class="card-content">
            <div class="calendar-container">
                <asp:Calendar ID="calLeaveCalendar" runat="server"
                              CssClass="leave-calendar"
                              OnDayRender="calLeaveCalendar_DayRender"
                              OnSelectionChanged="calLeaveCalendar_SelectionChanged"
                              SelectionMode="Day"
                              ShowGridLines="true"
                              DayNameFormat="Short"
                              NextPrevFormat="CustomText"
                              ShowNextPrevMonth="false">
                    <DayHeaderStyle CssClass="calendar-day-header" />
                    <DayStyle CssClass="calendar-day" />
                    <OtherMonthDayStyle CssClass="calendar-other-month" />
                    <SelectedDayStyle CssClass="calendar-selected-day" />
                    <TitleStyle CssClass="calendar-title" />
                    <WeekendDayStyle CssClass="calendar-weekend" />
                </asp:Calendar>
            </div>
        </div>
    </div>

    <!-- Legend -->
    <div class="content-card">
        <div class="card-header">
            <h3>Legend</h3>
        </div>
        <div class="card-content">
            <div class="calendar-legend">
                <div class="legend-item">
                    <div class="legend-color vacation"></div>
                    <span>Vacation Leave</span>
                </div>
                <div class="legend-item">
                    <div class="legend-color sick"></div>
                    <span>Sick Leave</span>
                </div>
                <div class="legend-item">
                    <div class="legend-color personal"></div>
                    <span>Personal Leave</span>
                </div>
                <div class="legend-item">
                    <div class="legend-color maternity"></div>
                    <span>Maternity/Paternity</span>
                </div>
                <div class="legend-item">
                    <div class="legend-color bereavement"></div>
                    <span>Bereavement Leave</span>
                </div>
                <div class="legend-item">
                    <div class="legend-color emergency"></div>
                    <span>Emergency Leave</span>
                </div>
                <div class="legend-item">
                    <div class="legend-color multiple"></div>
                    <span>Multiple Types</span>
                </div>
            </div>
        </div>
    </div>

    <!-- Day Details Panel -->
    <asp:Panel ID="pnlDayDetails" runat="server" CssClass="content-card" Visible="false">
        <div class="card-header">
            <h3>
                Leave Details for <asp:Literal ID="litSelectedDate" runat="server"></asp:Literal>
            </h3>
        </div>
        <div class="card-content">
            <asp:GridView ID="gvDayLeaves" runat="server" CssClass="data-table responsive-table" 
                         AutoGenerateColumns="false" DataKeyNames="Id">
                <Columns>
                    <asp:BoundField DataField="EmployeeName" HeaderText="Employee" />
                    <asp:BoundField DataField="Department" HeaderText="Department" />
                    <asp:BoundField DataField="LeaveType" HeaderText="Leave Type" />
                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="EndDate" HeaderText="End Date" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="DaysRequested" HeaderText="Days" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class="leave-status-badge status-<%# Eval("Status").ToString().ToLower() %>">
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="leave-empty-state">
                        <i class="material-icons">event_available</i>
                        <p>No leave requests for this date.</p>
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </asp:Panel>

<style>
/* Calendar Specific Styles */
.calendar-controls {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;
    flex-wrap: wrap;
    gap: 15px;
}

.calendar-navigation {
    display: flex;
    align-items: center;
    gap: 15px;
}

.current-month {
    font-size: 18px;
    font-weight: 600;
    color: #333;
    min-width: 200px;
    text-align: center;
}

.calendar-filters {
    display: flex;
    gap: 15px;
    flex-wrap: wrap;
}

.calendar-filters .filter-group {
    min-width: 150px;
}

.calendar-container {
    display: flex;
    justify-content: center;
    margin: 20px 0;
}

.leave-calendar {
    width: 100%;
    max-width: 800px;
    border-collapse: collapse;
    font-family: inherit;
}

.leave-calendar td,
.leave-calendar th {
    border: 1px solid #ddd;
    padding: 8px;
    text-align: center;
    vertical-align: top;
    height: 60px;
    width: 14.28%;
}

.calendar-day-header {
    background: #f8f9fa;
    font-weight: 600;
    color: #333;
    height: 40px !important;
}

.calendar-day {
    position: relative;
    background: white;
    cursor: pointer;
    transition: background-color 0.2s;
}

.calendar-day:hover {
    background: #f8f9fa;
}

.calendar-other-month {
    background: #f5f5f5;
    color: #999;
}

.calendar-selected-day {
    background: #007bff !important;
    color: white !important;
    font-weight: bold;
}

.calendar-weekend {
    background: #f8f9fa;
}

.calendar-title {
    background: #007bff;
    color: white;
    font-weight: bold;
    padding: 10px;
}

/* Leave indicators on calendar days */
.leave-indicator {
    position: absolute;
    bottom: 2px;
    left: 2px;
    right: 2px;
    height: 4px;
    border-radius: 2px;
}

.leave-indicator.vacation {
    background: #28a745;
}

.leave-indicator.sick {
    background: #dc3545;
}

.leave-indicator.personal {
    background: #007bff;
}

.leave-indicator.maternity,
.leave-indicator.paternity {
    background: #6f42c1;
}

.leave-indicator.bereavement {
    background: #6c757d;
}

.leave-indicator.emergency {
    background: #fd7e14;
}

.leave-indicator.multiple {
    background: linear-gradient(45deg, #28a745, #dc3545, #007bff, #6f42c1);
}

/* Calendar Legend */
.calendar-legend {
    display: flex;
    flex-wrap: wrap;
    gap: 20px;
    justify-content: center;
}

.legend-item {
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 14px;
}

.legend-color {
    width: 16px;
    height: 16px;
    border-radius: 3px;
    border: 1px solid #ddd;
}

.legend-color.vacation {
    background: #28a745;
}

.legend-color.sick {
    background: #dc3545;
}

.legend-color.personal {
    background: #007bff;
}

.legend-color.maternity {
    background: #6f42c1;
}

.legend-color.bereavement {
    background: #6c757d;
}

.legend-color.emergency {
    background: #fd7e14;
}

.legend-color.multiple {
    background: linear-gradient(45deg, #28a745, #dc3545, #007bff, #6f42c1);
}

/* Day tooltip */
.day-tooltip {
    position: absolute;
    top: 100%;
    left: 50%;
    transform: translateX(-50%);
    background: #333;
    color: white;
    padding: 8px 12px;
    border-radius: 4px;
    font-size: 12px;
    white-space: nowrap;
    z-index: 1000;
    opacity: 0;
    pointer-events: none;
    transition: opacity 0.2s;
}

.calendar-day:hover .day-tooltip {
    opacity: 1;
}

/* Responsive calendar */
@media (max-width: 768px) {
    .calendar-controls {
        flex-direction: column;
        align-items: stretch;
    }
    
    .calendar-navigation {
        justify-content: center;
        order: 2;
    }
    
    .calendar-filters {
        justify-content: center;
        order: 1;
    }
    
    .calendar-filters .filter-group {
        min-width: 120px;
    }
    
    .leave-calendar td,
    .leave-calendar th {
        height: 50px;
        font-size: 12px;
        padding: 4px;
    }
    
    .calendar-legend {
        gap: 15px;
    }
    
    .legend-item {
        font-size: 12px;
    }
}

@media (max-width: 480px) {
    .calendar-filters {
        flex-direction: column;
        width: 100%;
    }
    
    .calendar-filters .filter-group {
        min-width: auto;
        width: 100%;
    }
    
    .leave-calendar td,
    .leave-calendar th {
        height: 40px;
        font-size: 11px;
        padding: 2px;
    }
    
    .current-month {
        font-size: 16px;
        min-width: auto;
    }
}
</style>
</asp:Content>