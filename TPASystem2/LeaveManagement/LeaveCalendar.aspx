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

.btn-nav {
    background: var(--tpa-primary);
    color: white;
    border: none;
    border-radius: 50%;
    width: 40px;
    height: 40px;
    font-size: 1.5rem;
    font-weight: bold;
    cursor: pointer;
    transition: all 0.3s ease;
    display: flex;
    align-items: center;
    justify-content: center;
}

.btn-nav:hover {
    background: var(--tpa-primary-dark);
    transform: scale(1.1);
    box-shadow: 0 4px 12px rgba(59, 130, 246, 0.3);
}

.btn-small {
    padding: 0.5rem 1rem;
    font-size: 0.9rem;
    min-width: auto;
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

/* Calendar Statistics */
.calendar-stats {
    margin-bottom: 2rem;
}

.stats-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 1.5rem;
    margin-bottom: 2rem;
}

.stat-card {
    background: white;
    border-radius: var(--border-radius-large);
    padding: 1.5rem;
    box-shadow: var(--shadow-light);
    border: 1px solid var(--border-light);
    transition: all 0.3s ease;
    display: flex;
    align-items: center;
    gap: 1rem;
}

.stat-card:hover {
    transform: translateY(-2px);
    box-shadow: var(--shadow-medium);
}

.stat-icon {
    width: 60px;
    height: 60px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    background: linear-gradient(135deg, var(--tpa-primary), var(--tpa-primary-dark));
    color: white;
    flex-shrink: 0;
}

.stat-icon .material-icons {
    font-size: 1.8rem;
}

.stat-content {
    flex: 1;
}

.stat-number {
    font-size: 2rem;
    font-weight: 700;
    color: var(--text-primary);
    line-height: 1;
    margin-bottom: 0.25rem;
}

.stat-label {
    font-size: 1rem;
    font-weight: 600;
    color: var(--text-secondary);
    margin-bottom: 0.25rem;
}

.stat-subtitle {
    font-size: 0.85rem;
    color: var(--text-muted);
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

/* Leave indicators on calendar days */
.leave-indicator {
    position: absolute;
    bottom: 4px;
    left: 4px;
    right: 4px;
    height: 6px;
    border-radius: 3px;
    z-index: 1;
    background: var(--tpa-secondary);
    opacity: 0.8;
}

.leave-count {
    position: absolute;
    top: 4px;
    right: 4px;
    background: var(--tpa-accent);
    color: white;
    font-size: 0.7rem;
    font-weight: bold;
    padding: 2px 4px;
    border-radius: 8px;
    min-width: 16px;
    text-align: center;
    z-index: 2;
}

/* Leave Legend */
.leave-legend {
    background: white;
    border-radius: var(--border-radius-large);
    padding: 1.5rem;
    box-shadow: var(--shadow-light);
    margin-bottom: 2rem;
}

.legend-header {
    margin-bottom: 1rem;
    border-bottom: 1px solid var(--border-light);
    padding-bottom: 1rem;
}

.legend-header h3 {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    margin: 0;
    color: var(--text-primary);
    font-size: 1.1rem;
}

.legend-header .material-icons {
    color: var(--tpa-primary);
}

.legend-items {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
}

.legend-item {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.5rem 1rem;
    background: var(--background-light);
    border-radius: var(--border-radius);
    border: 1px solid var(--border-light);
}

.legend-color {
    width: 16px;
    height: 16px;
    border-radius: 4px;
    flex-shrink: 0;
}

.legend-label {
    font-weight: 500;
    color: var(--text-primary);
}

.legend-count {
    color: var(--text-muted);
    font-size: 0.9rem;
}

/* Day Details Modal */
.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 1000;
    backdrop-filter: blur(4px);
}

.modal-content {
    background: white;
    border-radius: var(--border-radius-large);
    max-width: 600px;
    width: 90%;
    max-height: 80vh;
    overflow: hidden;
    box-shadow: var(--shadow-heavy);
    animation: modalSlideUp 0.3s ease;
}

@keyframes modalSlideUp {
    from {
        transform: translateY(50px);
        opacity: 0;
    }
    to {
        transform: translateY(0);
        opacity: 1;
    }
}

.modal-header {
    background: linear-gradient(135deg, var(--tpa-primary), var(--tpa-primary-dark));
    color: white;
    padding: 1.5rem;
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.modal-header h3 {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    margin: 0;
    font-size: 1.2rem;
}

.btn-close {
    background: transparent;
    border: none;
    color: white;
    font-size: 1.5rem;
    cursor: pointer;
    padding: 0.25rem 0.5rem;
    border-radius: var(--border-radius);
    transition: all 0.3s ease;
}

.btn-close:hover {
    background: rgba(255, 255, 255, 0.2);
}

.modal-body {
    padding: 1.5rem;
    max-height: 60vh;
    overflow-y: auto;
}

/* Leaves List */
.leaves-list {
    display: flex;
    flex-direction: column;
    gap: 1rem;
}

.leave-item {
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 1rem;
    border: 1px solid var(--border-light);
    border-radius: var(--border-radius);
    background: var(--background-light);
    transition: all 0.3s ease;
}

.leave-item:hover {
    background: white;
    box-shadow: var(--shadow-light);
}

.employee-avatar {
    width: 50px;
    height: 50px;
    border-radius: 50%;
    background: var(--tpa-primary);
    color: white;
    display: flex;
    align-items: center;
    justify-content: center;
    font-weight: bold;
    font-size: 1.1rem;
    flex-shrink: 0;
}

.leave-info {
    flex: 1;
}

.employee-name {
    font-weight: 600;
    color: var(--text-primary);
    margin-bottom: 0.25rem;
}

.leave-details {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    margin-bottom: 0.25rem;
}

.leave-type {
    font-weight: 500;
    padding: 0.25rem 0.5rem;
    border-radius: var(--border-radius);
    background: rgba(59, 130, 246, 0.1);
    font-size: 0.85rem;
}

.leave-duration {
    color: var(--text-muted);
    font-size: 0.9rem;
}

.leave-reason {
    color: var(--text-secondary);
    font-size: 0.9rem;
    font-style: italic;
    line-height: 1.4;
}

.leave-status {
    flex-shrink: 0;
}

.status-badge {
    padding: 0.25rem 0.75rem;
    border-radius: 20px;
    font-size: 0.8rem;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.status-approved {
    background: #dcfce7;
    color: #16a34a;
}

/* No Data State */
.no-data {
    text-align: center;
    padding: 2rem;
    color: var(--text-muted);
}

.no-data .material-icons {
    font-size: 3rem;
    color: var(--text-disabled);
    margin-bottom: 1rem;
}

.no-data h4 {
    margin: 0 0 0.5rem 0;
    color: var(--text-secondary);
}

.no-data p {
    margin: 0;
    font-size: 0.9rem;
}

/* Responsive Design */
@media (max-width: 1024px) {
    .calendar-controls {
        flex-direction: column;
        align-items: stretch;
        gap: 1rem;
    }
    
    .calendar-navigation {
        justify-content: center;
    }
    
    .calendar-filters {
        justify-content: center;
    }
    
    .stats-grid {
        grid-template-columns: repeat(2, 1fr);
    }
}

@media (max-width: 768px) {
    .stats-grid {
        grid-template-columns: 1fr;
    }
    
    .calendar-controls {
        padding: 1rem;
    }
    
    .filter-group {
        min-width: 120px;
    }
    
    .current-month-display {
        min-width: 150px;
        font-size: 1rem;
    }
    
    .leave-calendar td,
    .leave-calendar th {
        height: 60px;
        padding: 8px 4px;
    }
    
    .modal-content {
        width: 95%;
        margin: 1rem;
    }
    
    .leave-item {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.75rem;
    }
    
    .leave-details {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.25rem;
    }
}

@media (max-width: 480px) {
    .calendar-navigation {
        flex-wrap: wrap;
        gap: 0.5rem;
    }
    
    .calendar-filters {
        flex-direction: column;
        gap: 0.75rem;
    }
    
    .leave-calendar {
        font-size: 0.8rem;
    }
    
    .leave-calendar td,
    .leave-calendar th {
        height: 50px;
        padding: 4px 2px;
    }
    
    .stat-card {
        flex-direction: column;
        text-align: center;
    }
    
    .stat-icon {
        margin-bottom: 0.5rem;
    }
}
    </style>
    <!-- Welcome Header - Matching EmployeeLeavePortal Style -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">event_available</i>
                    Leave Calendar
                </h1>
                <p class="welcome-subtitle">View team leaves and plan your time off</p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">person</i>
                        <span>
                            <asp:Literal ID="litCurrentUser" runat="server" Text="Current User"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">business</i>
                        <span>
                            <asp:Literal ID="litDepartment" runat="server" Text="Department"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">today</i>
                        <span>
                            <asp:Literal ID="litCurrentDate" runat="server"></asp:Literal>
                        </span>
                    </div>
                </div>
            </div>
            <div class="welcome-actions">
                <asp:Button ID="btnRequestLeave" runat="server" Text="Request Leave" CssClass="btn-tpa" OnClick="btnRequestLeave_Click" />
                <asp:Button ID="btnMyLeaves" runat="server" Text="My Leaves" CssClass="btn-secondary" OnClick="btnMyLeaves_Click" />
            </div>
        </div>
    </div>

    <!-- Error/Success Messages -->
    <asp:Panel ID="pnlMessages" runat="server" Visible="false" CssClass="alert-panel">
        <asp:Label ID="lblMessage" runat="server" CssClass="message-text"></asp:Label>
    </asp:Panel>

    <!-- Calendar Controls -->
    <div class="calendar-controls">
        <div class="calendar-navigation">
            <asp:Button ID="btnPrevMonth" runat="server" Text="‹" CssClass="btn-nav" OnClick="btnPrevMonth_Click" ToolTip="Previous Month" />
            <div class="current-month-display">
                <asp:Literal ID="litCurrentMonth" runat="server"></asp:Literal>
            </div>
            <asp:Button ID="btnNextMonth" runat="server" Text="›" CssClass="btn-nav" OnClick="btnNextMonth_Click" ToolTip="Next Month" />
            <asp:Button ID="btnToday" runat="server" Text="Today" CssClass="btn-secondary btn-small" OnClick="btnToday_Click" />
        </div>
        
        <div class="calendar-filters">
            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">visibility</i>
                    View Type
                </label>
                <asp:DropDownList ID="ddlViewType" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlViewType_SelectedIndexChanged">
                    <asp:ListItem Value="all" Text="All Employees"></asp:ListItem>
                    <asp:ListItem Value="my" Text="My Leaves Only"></asp:ListItem>
                    <asp:ListItem Value="team" Text="My Team"></asp:ListItem>
                    <asp:ListItem Value="department" Text="My Department"></asp:ListItem>
                </asp:DropDownList>
            </div>
            
            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">filter_list</i>
                    Leave Type
                </label>
                <asp:DropDownList ID="ddlLeaveTypeFilter" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlLeaveTypeFilter_SelectedIndexChanged">
                    <asp:ListItem Value="" Text="All Types"></asp:ListItem>
                    <asp:ListItem Value="Vacation" Text="Vacation"></asp:ListItem>
                    <asp:ListItem Value="Sick" Text="Sick Leave"></asp:ListItem>
                    <asp:ListItem Value="Personal" Text="Personal"></asp:ListItem>
                    <asp:ListItem Value="Emergency" Text="Emergency"></asp:ListItem>
                    <asp:ListItem Value="Bereavement" Text="Bereavement"></asp:ListItem>
                    <asp:ListItem Value="Maternity" Text="Maternity"></asp:ListItem>
                    <asp:ListItem Value="Paternity" Text="Paternity"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
    </div>

    <!-- Calendar Statistics -->
    <div class="calendar-stats">
        <div class="stats-grid">
            <div class="stat-card">
                <div class="stat-icon">
                    <i class="material-icons">event</i>
                </div>
                <div class="stat-content">
                    <div class="stat-number">
                        <asp:Literal ID="litTotalLeaves" runat="server">0</asp:Literal>
                    </div>
                    <div class="stat-label">Total Leaves</div>
                    <div class="stat-subtitle">This month</div>
                </div>
            </div>
            
            <div class="stat-card">
                <div class="stat-icon">
                    <i class="material-icons">people</i>
                </div>
                <div class="stat-content">
                    <div class="stat-number">
                        <asp:Literal ID="litUniqueEmployees" runat="server">0</asp:Literal>
                    </div>
                    <div class="stat-label">Employees</div>
                    <div class="stat-subtitle">On leave</div>
                </div>
            </div>
            
            <div class="stat-card">
                <div class="stat-icon">
                    <i class="material-icons">schedule</i>
                </div>
                <div class="stat-content">
                    <div class="stat-number">
                        <asp:Literal ID="litTotalDays" runat="server">0</asp:Literal>
                    </div>
                    <div class="stat-label">Total Days</div>
                    <div class="stat-subtitle">Leave days</div>
                </div>
            </div>
            
            <div class="stat-card">
                <div class="stat-icon">
                    <i class="material-icons">trending_up</i>
                </div>
                <div class="stat-content">
                    <div class="stat-number">
                        <asp:Literal ID="litBusiestDay" runat="server">N/A</asp:Literal>
                    </div>
                    <div class="stat-label">Busiest Day</div>
                    <div class="stat-subtitle">Most leaves</div>
                </div>
            </div>
        </div>
    </div>

    <!-- Calendar Container -->
    <div class="calendar-container">
        <asp:Calendar ID="calLeave" runat="server" 
                     CssClass="leave-calendar" 
                     DayNameFormat="Full"
                     FirstDayOfWeek="Sunday"
                     ShowGridLines="true"
                     OnDayRender="calLeave_DayRender"
                     OnSelectionChanged="calLeave_SelectionChanged"
                     OnVisibleMonthChanged="calLeave_VisibleMonthChanged">
            <DayHeaderStyle CssClass="calendar-day-header" />
            <DayStyle CssClass="calendar-day" />
            <OtherMonthDayStyle CssClass="calendar-other-month" />
            <SelectedDayStyle CssClass="calendar-selected-day" />
            <WeekendDayStyle CssClass="calendar-weekend" />
            <TitleStyle CssClass="calendar-title" />
        </asp:Calendar>
    </div>

    <!-- Leave Legend -->
    <div class="leave-legend">
        <div class="legend-header">
            <h3>
                <i class="material-icons">info</i>
                Leave Types Legend
            </h3>
        </div>
        <div class="legend-items">
            <asp:Repeater ID="rptLeaveLegend" runat="server">
                <ItemTemplate>
                    <div class="legend-item">
                        <div class="legend-color" style='background: <%# Eval("ColorCode") %>;'></div>
                        <span class="legend-label"><%# Eval("LeaveType") %></span>
                        <span class="legend-count">(<%# Eval("Count") %> this month)</span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>

    <!-- Selected Day Details Modal -->
    <asp:Panel ID="pnlDayDetails" runat="server" Visible="false" CssClass="modal-overlay">
        <div class="modal-content">
            <div class="modal-header">
                <h3>
                    <i class="material-icons">event</i>
                    Leaves for <asp:Literal ID="litSelectedDate" runat="server"></asp:Literal>
                </h3>
                <asp:Button ID="btnCloseDayDetails" runat="server" Text="×" CssClass="btn-close" OnClick="btnCloseDayDetails_Click" />
            </div>
            <div class="modal-body">
                <asp:Repeater ID="rptDayLeaves" runat="server">
                    <HeaderTemplate>
                        <div class="leaves-list">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="leave-item">
                            <div class="employee-avatar">
                                <%# GetInitials(Eval("EmployeeName").ToString()) %>
                            </div>
                            <div class="leave-info">
                                <div class="employee-name"><%# Eval("EmployeeName") %></div>
                                <div class="leave-details">
                                    <span class="leave-type" style='color: <%# Eval("ColorCode") %>;'>
                                        <%# Eval("LeaveType") %>
                                    </span>
                                    <span class="leave-duration">
                                        <%# GetLeaveDuration(Eval("StartDate"), Eval("EndDate")) %>
                                    </span>
                                </div>
                                <div class="leave-reason"><%# Eval("Reason") %></div>
                            </div>
                            <div class="leave-status">
                                <span class="status-badge status-approved">Approved</span>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
                
                <asp:Panel ID="pnlNoLeavesForDay" runat="server" Visible="false" CssClass="no-data">
                    <i class="material-icons">event_busy</i>
                    <h4>No leaves scheduled for this day</h4>
                    <p>All employees are available on this date.</p>
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>

    <!-- Hidden Fields -->
    <asp:HiddenField ID="hfCurrentMonth" runat="server" />
    <asp:HiddenField ID="hfCurrentYear" runat="server" />
    <asp:HiddenField ID="hfSelectedDate" runat="server" />

</asp:Content>