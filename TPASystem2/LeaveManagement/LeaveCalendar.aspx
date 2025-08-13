<%@ Page Title="Leave Calendar" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="LeaveCalendar.aspx.cs" Inherits="TPASystem2.LeaveManagement.LeaveCalendar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    <style>
        /* ===============================================
   Leave Calendar Specific Styles
   Add these styles to tpa-common.css
   =============================================== */

/* Calendar Controls */
.calendar-controls {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 20px;
    flex-wrap: wrap;
    gap: 15px;
    padding: 1.5rem;
    background: white;
    border-radius: var(--border-radius-large);
    box-shadow: var(--shadow-light);
}

.calendar-navigation {
    display: flex;
    align-items: center;
    gap: 15px;
}

.current-month-display {
    font-size: 18px;
    font-weight: 600;
    color: var(--text-primary);
    min-width: 200px;
    text-align: center;
    padding: 0 1rem;
}

.calendar-filters {
    display: flex;
    gap: 15px;
    flex-wrap: wrap;
    align-items: center;
}

.filter-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    min-width: 150px;
}

.filter-label {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-weight: 500;
    font-size: 0.9rem;
    color: var(--text-secondary);
}

.filter-label .material-icons {
    font-size: 16px;
}

/* Calendar Container */
.calendar-container {
    display: flex;
    justify-content: center;
    margin: 20px 0;
    padding: 2rem;
    background: white;
    border-radius: var(--border-radius-large);
    box-shadow: var(--shadow-light);
}

/* Leave Calendar Styles */
.leave-calendar {
    width: 100%;
    max-width: 900px;
    border-collapse: collapse;
    font-family: inherit;
    background: white;
    border-radius: var(--border-radius);
    overflow: hidden;
    box-shadow: var(--shadow-light);
}

.leave-calendar td,
.leave-calendar th {
    border: 1px solid var(--border-light);
    padding: 12px 8px;
    text-align: center;
    vertical-align: top;
    height: 80px;
    width: 14.28%;
    position: relative;
}

.calendar-day-header {
    background: linear-gradient(135deg, var(--tpa-primary) 0%, var(--tpa-primary-dark) 100%);
    color: white;
    font-weight: 600;
    height: 50px !important;
    font-size: 0.9rem;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.calendar-day {
    position: relative;
    background: white;
    cursor: pointer;
    transition: all var(--transition-fast);
    border: 1px solid var(--border-light);
}

.calendar-day:hover {
    background: var(--background-light);
    transform: scale(1.02);
    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    z-index: 2;
}

.calendar-other-month {
    background: #f8f9fa;
    color: var(--text-disabled);
    opacity: 0.6;
}

.calendar-selected-day {
    background: var(--tpa-secondary) !important;
    color: white !important;
    font-weight: bold;
    box-shadow: inset 0 0 0 3px var(--tpa-primary) !important;
}

.calendar-weekend {
    background: #fafbfc;
}

.calendar-title {
    background: var(--tpa-primary);
    color: white;
    font-weight: bold;
    padding: 15px;
    font-size: 1.1rem;
}

/* Today indicator */
.calendar-day.today {
    background: linear-gradient(135deg, rgba(255, 152, 0, 0.1) 0%, rgba(245, 124, 0, 0.1) 100%);
    border: 2px solid var(--tpa-primary);
    font-weight: 600;
}

/* Leave indicators on calendar days - Dynamic colors from database */
.leave-indicator {
    position: absolute;
    bottom: 4px;
    left: 4px;
    right: 4px;
    height: 6px;
    border-radius: 3px;
    z-index: 1;
    /* Default background - will be overridden by inline styles from database */
    background: var(--tpa-secondary);
}

/* Fallback colors for leave types (if database color not available) */
.leave-indicator.vacation {
    background: linear-gradient(135deg, #4caf50, #388e3c);
}

.leave-indicator.sick {
    background: linear-gradient(135deg, #f44336, #d32f2f);
}

.leave-indicator.personal {
    background: linear-gradient(135deg, #2196f3, #1976d2);
}

.leave-indicator.maternity,
.leave-indicator.paternity {
    background: linear-gradient(135deg, #9c27b0, #7b1fa2);
}

.leave-indicator.bereavement {
    background: linear-gradient(135deg, #607d8b, #455a64);
}

.leave-indicator.emergency {
    background: linear-gradient(135deg, #ff9800, #f57c00);
}

.leave-indicator.unpaid {
    background: linear-gradient(135deg, #795548, #5d4037);
}

.leave-indicator.multiple {
    background: linear-gradient(90deg, #4caf50 0%, #f44336 25%, #2196f3 50%, #9c27b0 75%, #ff9800 100%);
    background-size: 200% 100%;
    animation: rainbow-slide 3s linear infinite;
}

/* Holiday indicator */
.holiday-indicator {
    position: absolute;
    top: 2px;
    left: 2px;
    width: 12px;
    height: 12px;
    background: #ff5722;
    border-radius: 50%;
    border: 2px solid white;
    z-index: 3;
}

.holiday-indicator::after {
    content: "🎉";
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    font-size: 8px;
    line-height: 1;
}

@keyframes rainbow-slide {
    0% { background-position: 0% 50%; }
    100% { background-position: 200% 50%; }
}

/* Leave count badge */
.leave-count {
    position: absolute;
    top: 2px;
    right: 2px;
    background: var(--tpa-primary);
    color: white;
    border-radius: 50%;
    width: 18px;
    height: 18px;
    font-size: 10px;
    font-weight: 600;
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 2;
    box-shadow: 0 1px 3px rgba(0,0,0,0.3);
}

/* Calendar Legend */
.calendar-legend {
    display: flex;
    flex-wrap: wrap;
    gap: 20px;
    justify-content: center;
    padding: 1rem;
}

.legend-item {
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 14px;
    font-weight: 500;
    color: var(--text-primary);
}

.legend-color {
    width: 20px;
    height: 12px;
    border-radius: 6px;
    border: 1px solid rgba(0,0,0,0.1);
    flex-shrink: 0;
}

.legend-color.vacation {
    background: linear-gradient(135deg, #4caf50, #388e3c);
}

.legend-color.sick {
    background: linear-gradient(135deg, #f44336, #d32f2f);
}

.legend-color.personal {
    background: linear-gradient(135deg, #2196f3, #1976d2);
}

.legend-color.maternity,
.legend-color.paternity {
    background: linear-gradient(135deg, #9c27b0, #7b1fa2);
}

.legend-color.bereavement {
    background: linear-gradient(135deg, #607d8b, #455a64);
}

.legend-color.emergency {
    background: linear-gradient(135deg, #ff9800, #f57c00);
}

.legend-color.multiple {
    background: linear-gradient(90deg, #4caf50 0%, #f44336 25%, #2196f3 50%, #9c27b0 75%, #ff9800 100%);
}

/* Modal Overlay for Day Details */
.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    z-index: 1000;
    display: flex;
    align-items: center;
    justify-content: center;
    animation: fadeIn 0.3s ease;
}

.modal-content {
    background: white;
    border-radius: var(--border-radius-large);
    box-shadow: var(--shadow-heavy);
    width: 90%;
    max-width: 800px;
    max-height: 80vh;
    overflow: hidden;
    display: flex;
    flex-direction: column;
    animation: slideUp 0.3s ease;
}

.modal-header {
    padding: 1.5rem 2rem;
    border-bottom: 1px solid var(--border-light);
    background: linear-gradient(135deg, var(--tpa-primary) 0%, var(--tpa-primary-dark) 100%);
    color: white;
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.modal-title {
    margin: 0;
    font-size: 1.25rem;
    font-weight: 600;
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.modal-close {
    background: none;
    border: none;
    color: white;
    font-size: 1.5rem;
    cursor: pointer;
    padding: 0.5rem;
    border-radius: 50%;
    transition: background-color var(--transition-fast);
    text-decoration: none;
}

.modal-close:hover {
    background: rgba(255, 255, 255, 0.2);
    color: white;
}

.modal-body {
    padding: 2rem;
    overflow-y: auto;
    flex: 1;
}

/* Modern Grid for Day Details */
.modern-grid {
    width: 100%;
    border-collapse: collapse;
    font-size: 0.9rem;
    background: white;
    border-radius: var(--border-radius);
    overflow: hidden;
    box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}

.modern-grid th {
    background: var(--background-light);
    padding: 1rem;
    text-align: left;
    font-weight: 600;
    color: var(--text-primary);
    border-bottom: 2px solid var(--border-light);
    font-size: 0.85rem;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.modern-grid td {
    padding: 1rem;
    border-bottom: 1px solid var(--border-light);
    vertical-align: middle;
}

.modern-grid tr:hover {
    background: var(--background-light);
}

.modern-grid tr:last-child td {
    border-bottom: none;
}

/* Leave Status Badges */
.leave-status-badge {
    display: inline-block;
    padding: 0.25rem 0.75rem;
    border-radius: 20px;
    font-size: 0.75rem;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.status-approved {
    background: #e8f5e8;
    color: #2e7d32;
    border: 1px solid #4caf50;
}

.status-pending {
    background: #fff3e0;
    color: #f57c00;
    border: 1px solid #ff9800;
}

.status-rejected {
    background: #ffebee;
    color: #c62828;
    border: 1px solid #f44336;
}

.status-draft {
    background: #f3e5f5;
    color: #7b1fa2;
    border: 1px solid #9c27b0;
}

/* Leave Progress Indicator */
.leave-progress {
    font-size: 0.8rem;
    color: var(--text-secondary);
    font-style: italic;
}

/* Overview Cards for Statistics */
.overview-cards {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 1.5rem;
    margin-bottom: 2rem;
}

.overview-card {
    background: white;
    padding: 1.5rem;
    border-radius: var(--border-radius-large);
    box-shadow: var(--shadow-light);
    border: 1px solid var(--border-light);
    transition: all var(--transition-fast);
    display: flex;
    align-items: center;
    gap: 1rem;
}

.overview-card:hover {
    transform: translateY(-2px);
    box-shadow: var(--shadow-medium);
}

.card-icon {
    width: 60px;
    height: 60px;
    border-radius: var(--border-radius);
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 1.8rem;
    flex-shrink: 0;
}

.card-icon.events {
    background: linear-gradient(135deg, #4caf50, #388e3c);
    color: white;
}

.card-icon.people {
    background: linear-gradient(135deg, #2196f3, #1976d2);
    color: white;
}

.card-icon.schedule {
    background: linear-gradient(135deg, #ff9800, #f57c00);
    color: white;
}

.card-icon.trending {
    background: linear-gradient(135deg, #9c27b0, #7b1fa2);
    color: white;
}

.card-content {
    flex: 1;
}

.card-number {
    font-size: 2rem;
    font-weight: 700;
    color: var(--text-primary);
    line-height: 1;
    margin-bottom: 0.25rem;
}

.card-label {
    font-size: 0.9rem;
    font-weight: 600;
    color: var(--text-primary);
    margin-bottom: 0.25rem;
}

.card-sublabel {
    font-size: 0.8rem;
    color: var(--text-secondary);
}

/* Animations */
@keyframes fadeIn {
    from { opacity: 0; }
    to { opacity: 1; }
}

@keyframes slideUp {
    from {
        opacity: 0;
        transform: translateY(30px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

/* Responsive Design */
@media (max-width: 768px) {
    .calendar-controls {
        flex-direction: column;
        align-items: stretch;
        gap: 1rem;
    }
    
    .calendar-navigation {
        justify-content: center;
        order: 2;
    }
    
    .calendar-filters {
        justify-content: center;
        order: 1;
        flex-direction: column;
        gap: 1rem;
    }
    
    .filter-group {
        min-width: auto;
        width: 100%;
    }
    
    .leave-calendar td,
    .leave-calendar th {
        height: 60px;
        font-size: 12px;
        padding: 8px 4px;
    }
    
    .current-month-display {
        font-size: 16px;
        min-width: auto;
    }
    
    .calendar-legend {
        gap: 15px;
        flex-direction: column;
        align-items: center;
    }
    
    .legend-item {
        font-size: 12px;
    }
    
    .modal-content {
        width: 95%;
        margin: 1rem;
    }
    
    .modal-header {
        padding: 1rem;
    }
    
    .modal-body {
        padding: 1rem;
    }
    
    .overview-cards {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
    
    .overview-card {
        padding: 1rem;
    }
    
    .card-icon {
        width: 50px;
        height: 50px;
        font-size: 1.5rem;
    }
    
    .card-number {
        font-size: 1.5rem;
    }
}

@media (max-width: 480px) {
    .leave-calendar td,
    .leave-calendar th {
        height: 50px;
        font-size: 11px;
        padding: 4px 2px;
    }
    
    .leave-count {
        width: 16px;
        height: 16px;
        font-size: 9px;
    }
    
    .leave-indicator {
        height: 4px;
        bottom: 2px;
        left: 2px;
        right: 2px;
    }
    
    .calendar-container {
        padding: 1rem;
    }
    
    .modern-grid th,
    .modern-grid td {
        padding: 0.5rem;
        font-size: 0.8rem;
    }
}
    </style>
    <!-- Welcome Header - Matching EmployeeLeavePortal Style -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">date_range</i>
                    Leave Calendar
                </h1>
                <p class="welcome-subtitle">Visual overview of leave schedules and team availability</p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">person</i>
                        <span>
                            <asp:Literal ID="litCurrentUser" runat="server" Text="Current User"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">visibility</i>
                        <span>
                            <asp:Literal ID="litViewPermission" runat="server" Text="View Permission"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">calendar_today</i>
                        <span>
                            <asp:Literal ID="litCurrentMonth" runat="server" Text="Current Month"></asp:Literal>
                        </span>
                    </div>
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnRequestLeave" runat="server" Text="Request Leave" 
                    CssClass="btn btn-outline-light" OnClick="btnRequestLeave_Click" />
                <asp:Button ID="btnBackToManagement" runat="server" Text="Back to Management" 
                    CssClass="btn btn-outline-light" OnClick="btnBackToManagement_Click" />
            </div>
        </div>
    </div>

    <!-- Alert Messages -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert-panel">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Calendar Controls -->
    <div class="time-status-card">
        <div class="status-header">
            <h3>
                <i class="material-icons">tune</i>
                Calendar Settings
            </h3>
        </div>
        
        <div class="calendar-controls">
            <div class="calendar-navigation">
                <asp:Button ID="btnPrevMonth" runat="server" Text="‹ Previous" 
                           CssClass="btn btn-secondary" 
                           OnClick="btnPrevMonth_Click" />
                <div class="current-month-display">
                    <asp:Literal ID="litMonthYear" runat="server"></asp:Literal>
                </div>
                <asp:Button ID="btnNextMonth" runat="server" Text="Next ›" 
                           CssClass="btn btn-secondary" 
                           OnClick="btnNextMonth_Click" />
                <asp:Button ID="btnToday" runat="server" Text="Today" 
                           CssClass="btn btn-primary" 
                           OnClick="btnToday_Click" />
            </div>
            
            <div class="calendar-filters">
                <div class="filter-group">
                    <label class="filter-label">
                        <i class="material-icons">visibility</i>
                        View Type:
                    </label>
                    <asp:DropDownList ID="ddlViewType" runat="server" CssClass="form-control" 
                                      AutoPostBack="true" OnSelectedIndexChanged="ddlViewType_SelectedIndexChanged">
                        <asp:ListItem Value="my" Selected="true">My Leaves</asp:ListItem>
                        <asp:ListItem Value="team">Team Leaves</asp:ListItem>
                        <asp:ListItem Value="department">Department Leaves</asp:ListItem>
                        <asp:ListItem Value="all">All Leaves</asp:ListItem>
                    </asp:DropDownList>
                </div>
                
                <div class="filter-group">
                    <label class="filter-label">
                        <i class="material-icons">business</i>
                        Department:
                    </label>
                    <asp:DropDownList ID="ddlDepartmentFilter" runat="server" CssClass="form-control" 
                                      AutoPostBack="true" OnSelectedIndexChanged="ddlDepartmentFilter_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                
                <div class="filter-group">
                    <label class="filter-label">
                        <i class="material-icons">event</i>
                        Leave Type:
                    </label>
                    <asp:DropDownList ID="ddlLeaveTypeFilter" runat="server" CssClass="form-control" 
                                      AutoPostBack="true" OnSelectedIndexChanged="ddlLeaveTypeFilter_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
            </div>
        </div>
    </div>

    <!-- Calendar Display -->
    <div class="calendar-container">
        <asp:Calendar ID="calLeaveCalendar" runat="server"
                      CssClass="leave-calendar"
                      OnDayRender="calLeaveCalendar_DayRender"
                      OnSelectionChanged="calLeaveCalendar_SelectionChanged"
                      SelectionMode="Day"
                      ShowGridLines="true"
                      DayNameFormat="Short"
                      NextPrevFormat="CustomText"
                      ShowNextPrevMonth="false"
                      ShowTitle="false">
            <DayHeaderStyle CssClass="calendar-day-header" />
            <DayStyle CssClass="calendar-day" />
            <OtherMonthDayStyle CssClass="calendar-other-month" />
            <SelectedDayStyle CssClass="calendar-selected-day" />
            <TitleStyle CssClass="calendar-title" />
            <WeekendDayStyle CssClass="calendar-weekend" />
        </asp:Calendar>
    </div>

    <!-- Calendar Legend -->
    <div class="time-status-card">
        <div class="status-header">
            <h3>
                <i class="material-icons">info</i>
                Legend
            </h3>
        </div>
        
        <div class="calendar-legend">
            <asp:Repeater ID="rptLegend" runat="server">
                <ItemTemplate>
                    <div class="legend-item">
                        <div class="legend-color <%# Eval("TypeName").ToString().ToLower() %>"></div>
                        <span><%# Eval("TypeName") %> Leave</span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            
            <div class="legend-item">
                <div class="legend-color multiple"></div>
                <span>Multiple Leave Types</span>
            </div>
        </div>
    </div>

    <!-- Daily Leave Details Panel -->
    <asp:Panel ID="pnlDayDetails" runat="server" CssClass="modal-overlay" Visible="false">
        <div class="modal-content">
            <div class="modal-header">
                <h4 class="modal-title">
                    <i class="material-icons">event_note</i>
                    Leave Details for <asp:Literal ID="litSelectedDate" runat="server"></asp:Literal>
                </h4>
                <asp:LinkButton ID="btnCloseDayDetails" runat="server" CssClass="modal-close" OnClick="btnCloseDayDetails_Click">
                    <i class="material-icons">close</i>
                </asp:LinkButton>
            </div>
            
            <div class="modal-body">
                <asp:GridView ID="gvDayLeaveDetails" runat="server" 
                    CssClass="modern-grid" 
                    AutoGenerateColumns="false" 
                    EmptyDataText="No leave requests for this date.">
                    
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
                        
                        <asp:TemplateField HeaderText="Progress">
                            <ItemTemplate>
                                <div class="leave-progress">
                                    <%# GetLeaveProgress(Eval("StartDate"), Eval("EndDate")) %>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </asp:Panel>

    <!-- Statistics Panel -->
    <div class="time-status-card">
        <div class="status-header">
            <h3>
                <i class="material-icons">analytics</i>
                Monthly Statistics
            </h3>
        </div>
        
        <div class="overview-cards">
            <div class="overview-card">
                <div class="card-icon">
                    <i class="material-icons">event_available</i>
                </div>
                <div class="card-content">
                    <div class="card-number">
                        <asp:Literal ID="litTotalLeaves" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="card-label">Total Leaves</div>
                    <div class="card-sublabel">This Month</div>
                </div>
            </div>

            <div class="overview-card">
                <div class="card-icon">
                    <i class="material-icons">people</i>
                </div>
                <div class="card-content">
                    <div class="card-number">
                        <asp:Literal ID="litUniqueEmployees" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="card-label">Employees on Leave</div>
                    <div class="card-sublabel">This Month</div>
                </div>
            </div>

            <div class="overview-card">
                <div class="card-icon">
                    <i class="material-icons">schedule</i>
                </div>
                <div class="card-content">
                    <div class="card-number">
                        <asp:Literal ID="litTotalDays" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="card-label">Total Days</div>
                    <div class="card-sublabel">Leave Taken</div>
                </div>
            </div>

            <div class="overview-card">
                <div class="card-icon">
                    <i class="material-icons">trending_up</i>
                </div>
                <div class="card-content">
                    <div class="card-number">
                        <asp:Literal ID="litBusiestDay" runat="server" Text="N/A"></asp:Literal>
                    </div>
                    <div class="card-label">Busiest Day</div>
                    <div class="card-sublabel">Most Leave Requests</div>
                </div>
            </div>
        </div>
    </div>

    <!-- Hidden Fields -->
    <asp:HiddenField ID="hfCurrentDate" runat="server" />
    <asp:HiddenField ID="hfSelectedDate" runat="server" />

</asp:Content>