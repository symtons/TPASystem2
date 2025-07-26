<%@ Page Title="Employee Onboarding Management" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="OnboardingManagement.aspx.cs" Inherits="TPASystem2.HR.OnboardingManagement" %>

<asp:Content ID="OnboardingContent" ContentPlaceHolderID="DashboardContent" runat="server">
    
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/employee-management.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    
    <!-- Page Header -->
    <div class="page-header">
        <div class="page-header-content">
            <h1>Employee Onboarding Management</h1>
            <p>Manage and track employee onboarding progress across all departments</p>
        </div>
        <div class="page-header-actions">
            <asp:Button ID="btnExportReport" runat="server" Text="Export Report" 
                        CssClass="btn btn-secondary" OnClick="btnExportReport_Click" />
            <asp:Button ID="btnBulkActions" runat="server" Text="Bulk Actions" 
                        CssClass="btn btn-outline" OnClick="btnBulkActions_Click" />
        </div>
    </div>

    <!-- Statistics Cards -->
    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-icon">
                <i class="material-icons">people</i>
            </div>
            <div class="stat-content">
                <div class="stat-number">
                    <asp:Literal ID="litTotalEmployees" runat="server">0</asp:Literal>
                </div>
                <div class="stat-label">Total in Onboarding</div>
            </div>
        </div>
        
        <div class="stat-card success">
            <div class="stat-icon">
                <i class="material-icons">check_circle</i>
            </div>
            <div class="stat-content">
                <div class="stat-number">
                    <asp:Literal ID="litCompletedEmployees" runat="server">0</asp:Literal>
                </div>
                <div class="stat-label">Completed</div>
            </div>
        </div>
        
        <div class="stat-card warning">
            <div class="stat-icon">
                <i class="material-icons">access_time</i>
            </div>
            <div class="stat-content">
                <div class="stat-number">
                    <asp:Literal ID="litInProgressEmployees" runat="server">0</asp:Literal>
                </div>
                <div class="stat-label">In Progress</div>
            </div>
        </div>
        
        <div class="stat-card danger">
            <div class="stat-icon">
                <i class="material-icons">warning</i>
            </div>
            <div class="stat-content">
                <div class="stat-number">
                    <asp:Literal ID="litOverdueEmployees" runat="server">0</asp:Literal>
                </div>
                <div class="stat-label">Overdue Tasks</div>
            </div>
        </div>
    </div>

    <!-- Filter Section -->
    <div class="filter-section">
        <div class="filter-card">
            <div class="filter-row">
                <div class="filter-group">
                    <label>Search Employees</label>
                    <div class="search-input">
                        <asp:TextBox ID="txtSearch" runat="server" placeholder="Search by name, employee number, or position" 
                                     CssClass="form-control search-box" AutoPostBack="true" OnTextChanged="txtSearch_TextChanged" />
                        <i class="material-icons search-icon">search</i>
                    </div>
                </div>
                
                <div class="filter-group">
                    <label>Department</label>
                    <asp:DropDownList ID="ddlFilterDepartment" runat="server" CssClass="form-control filter-dropdown"
                                      AutoPostBack="true" OnSelectedIndexChanged="ddlFilterDepartment_SelectedIndexChanged">
                        <asp:ListItem Value="" Text="All Departments" />
                    </asp:DropDownList>
                </div>
                
                <div class="filter-group">
                    <label>Status</label>
                    <asp:DropDownList ID="ddlFilterStatus" runat="server" CssClass="form-control filter-dropdown"
                                      AutoPostBack="true" OnSelectedIndexChanged="ddlFilterStatus_SelectedIndexChanged">
                        <asp:ListItem Value="" Text="All Statuses" />
                        <asp:ListItem Value="PENDING" Text="Pending" />
                        <asp:ListItem Value="IN_PROGRESS" Text="In Progress" />
                        <asp:ListItem Value="COMPLETED" Text="Completed" />
                        <asp:ListItem Value="OVERDUE" Text="Overdue" />
                    </asp:DropDownList>
                </div>
                
                <div class="filter-group">
                    <label>Hire Date Range</label>
                    <div class="date-range">
                        <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control date-input" 
                                     TextMode="Date" AutoPostBack="true" OnTextChanged="DateFilter_Changed" />
                        <span class="date-separator">to</span>
                        <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control date-input" 
                                     TextMode="Date" AutoPostBack="true" OnTextChanged="DateFilter_Changed" />
                    </div>
                </div>
                
                <div class="filter-actions">
                    <asp:Button ID="btnClearFilters" runat="server" Text="Clear" 
                                CssClass="btn btn-outline" OnClick="btnClearFilters_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- Employee Onboarding List -->
    <div class="content-section">
        <div class="section-header">
            <h3>
                <i class="material-icons">assignment</i>
                Employee Onboarding Progress
            </h3>
            <div class="section-actions">
                <asp:DropDownList ID="ddlPageSize" runat="server" CssClass="form-control page-size-selector"
                                  AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                    <asp:ListItem Value="10" Text="10 per page" />
                    <asp:ListItem Value="25" Text="25 per page" Selected="True" />
                    <asp:ListItem Value="50" Text="50 per page" />
                    <asp:ListItem Value="100" Text="100 per page" />
                </asp:DropDownList>
            </div>
        </div>
        
        <!-- Employee Grid -->
        <asp:Panel ID="pnlEmployeeGrid" runat="server" CssClass="grid-container">
            <asp:GridView ID="gvEmployees" runat="server" AutoGenerateColumns="false" 
                          CssClass="employee-grid" AllowPaging="true" PageSize="25"
                          OnPageIndexChanging="gvEmployees_PageIndexChanging"
                          OnRowCommand="gvEmployees_RowCommand"
                          OnRowDataBound="gvEmployees_RowDataBound"
                          EmptyDataText="No employees found with the selected criteria.">
                <Columns>
                    <asp:TemplateField HeaderText="Employee" SortExpression="FullName">
                        <ItemTemplate>
                            <div class="employee-info">
                                <div class="employee-avatar">
                                    <span><%# GetEmployeeInitials(Eval("FullName").ToString()) %></span>
                                </div>
                                <div class="employee-details">
                                    <div class="employee-name"><%# Eval("FullName") %></div>
                                    <div class="employee-meta">
                                        <span class="employee-number"><%# Eval("EmployeeNumber") %></span>
                                        <span class="employee-position"><%# Eval("Position") %></span>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Department" SortExpression="DepartmentName">
                        <ItemTemplate>
                            <div class="department-info">
                                <span class="department-name"><%# Eval("DepartmentName") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Hire Date" SortExpression="HireDate">
                        <ItemTemplate>
                            <div class="hire-date-info">
                                <div class="hire-date"><%# Eval("HireDate", "{0:MMM dd, yyyy}") %></div>
                                <div class="days-since"><%# Eval("DaysSinceHire") %> days ago</div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Progress" SortExpression="CompletionPercentage">
                        <ItemTemplate>
                            <div class="progress-info">
                                <div class="progress-bar-container">
                                    <div class="progress-bar">
                                        <div class="progress-fill" style='width: <%# Eval("CompletionPercentage") %>%'></div>
                                    </div>
                                    <span class="progress-percentage"><%# Eval("CompletionPercentage", "{0:F0}") %>%</span>
                                </div>
                                <div class="task-summary">
                                    <%# Eval("CompletedTasks") %>/<%# Eval("TotalTasks") %> tasks completed
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Status" SortExpression="OnboardingStatus">
                        <ItemTemplate>
                            <span class='status-badge status-<%# GetStatusClass(Eval("OnboardingStatus").ToString()) %>'>
                                <%# GetStatusDisplay(Eval("OnboardingStatus").ToString()) %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Issues" SortExpression="OverdueTasks">
                        <ItemTemplate>
                            <div class="issues-info">
                                <asp:Panel ID="pnlOverdueTasks" runat="server" CssClass="issue-indicator overdue" 
                                           Visible='<%# Convert.ToInt32(Eval("OverdueTasks")) > 0 %>'>
                                    <i class="material-icons">warning</i>
                                    <span><%# Eval("OverdueTasks") %> overdue</span>
                                </asp:Panel>
                                <asp:Panel ID="pnlPendingTasks" runat="server" CssClass="issue-indicator pending" 
                                           Visible='<%# Convert.ToInt32(Eval("PendingTasks")) > 0 && Convert.ToInt32(Eval("OverdueTasks")) == 0 %>'>
                                    <i class="material-icons">schedule</i>
                                    <span><%# Eval("PendingTasks") %> pending</span>
                                </asp:Panel>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <div class="action-buttons">
                                <asp:LinkButton ID="btnManageOnboarding" runat="server" 
                                                CommandName="ManageOnboarding" 
                                                CommandArgument='<%# Eval("EmployeeId") %>'
                                                CssClass="btn btn-primary btn-sm" 
                                                ToolTip="Manage Onboarding">
                                    <i class="material-icons">edit</i>
                                    Manage
                                </asp:LinkButton>
                                
                                <asp:LinkButton ID="btnViewDetails" runat="server" 
                                                CommandName="ViewDetails" 
                                                CommandArgument='<%# Eval("EmployeeId") %>'
                                                CssClass="btn btn-outline btn-sm" 
                                                ToolTip="View Details">
                                    <i class="material-icons">visibility</i>
                                </asp:LinkButton>
                                
                                <asp:LinkButton ID="btnQuickComplete" runat="server" 
                                                CommandName="QuickComplete" 
                                                CommandArgument='<%# Eval("EmployeeId") %>'
                                                CssClass="btn btn-success btn-sm" 
                                                ToolTip="Mark as Complete"
                                                Visible='<%# Eval("OnboardingStatus").ToString() != "COMPLETED" %>'
                                                OnClientClick="return confirm('Mark this employee onboarding as complete?');">
                                    <i class="material-icons">check</i>
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                
                <PagerStyle CssClass="gridview-pager" />
                <EmptyDataTemplate>
                    <div class="empty-state">
                        <h3>No onboarding records found</h3>
                        <p>No employees match your current filter criteria.</p>
                        <asp:Button ID="btnClearEmptyFilters" runat="server" Text="Clear Filters" 
                                    CssClass="btn btn-primary" OnClick="btnClearFilters_Click" />
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </asp:Panel>
    </div>

    <!-- Quick Actions Panel -->
    <asp:Panel ID="pnlQuickActions" runat="server" CssClass="quick-actions-panel" Visible="false">
        <div class="quick-actions-header">
            <h4>Bulk Actions</h4>
            <asp:Button ID="btnCloseQuickActions" runat="server" Text="×" 
                        CssClass="btn btn-text close-btn" OnClick="btnCloseQuickActions_Click" />
        </div>
        <div class="quick-actions-content">
            <div class="action-group">
                <label>Send Reminder Emails</label>
                <asp:Button ID="btnSendReminders" runat="server" Text="Send to Overdue" 
                            CssClass="btn btn-warning" OnClick="btnSendReminders_Click" />
            </div>
            <div class="action-group">
                <label>Generate Reports</label>
                <asp:Button ID="btnGenerateProgressReport" runat="server" Text="Progress Report" 
                            CssClass="btn btn-info" OnClick="btnGenerateProgressReport_Click" />
            </div>
            <div class="action-group">
                <label>Reassign Tasks</label>
                <asp:Button ID="btnReassignTasks" runat="server" Text="Bulk Reassign" 
                            CssClass="btn btn-secondary" OnClick="btnReassignTasks_Click" />
            </div>
        </div>
    </asp:Panel>

    <!-- Notification Panel -->
    <asp:Panel ID="pnlNotification" runat="server" CssClass="notification-panel" Visible="false">
        <div class="notification-content">
            <asp:Literal ID="litNotificationMessage" runat="server"></asp:Literal>
        </div>
        <asp:Button ID="btnCloseNotification" runat="server" Text="×" 
                    CssClass="btn btn-text notification-close" OnClick="btnCloseNotification_Click" />
    </asp:Panel>

    <!-- Onboarding Management Specific Styles -->
    <style>
        /* Statistics Grid */
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 1.5rem;
            margin-bottom: 2rem;
        }

        .stat-card {
            background: white;
            border-radius: 12px;
            padding: 1.5rem;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            border: 1px solid #e0e0e0;
            display: flex;
            align-items: center;
            gap: 1rem;
            transition: transform 0.2s ease, box-shadow 0.2s ease;
        }

            .stat-card:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
            }

            .stat-card.success {
                border-left: 4px solid #4caf50;
            }

            .stat-card.warning {
                border-left: 4px solid #ff9800;
            }

            .stat-card.danger {
                border-left: 4px solid #f44336;
            }

        .stat-icon {
            background: #f5f5f5;
            border-radius: 50%;
            width: 60px;
            height: 60px;
            display: flex;
            align-items: center;
            justify-content: center;
        }

            .stat-card.success .stat-icon {
                background: #e8f5e8;
                color: #4caf50;
            }

            .stat-card.warning .stat-icon {
                background: #fff3e0;
                color: #ff9800;
            }

            .stat-card.danger .stat-icon {
                background: #ffebee;
                color: #f44336;
            }

        .stat-content {
            flex: 1;
        }

        .stat-number {
            font-size: 2rem;
            font-weight: 700;
            color: #212121;
            line-height: 1;
        }

        .stat-label {
            font-size: 0.9rem;
            color: #757575;
            margin-top: 0.25rem;
        }

        /* Employee Grid Enhanced Styles */
        .employee-grid {
            width: 100%;
            border-collapse: collapse;
            background: white;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
        }

            .employee-grid th {
                background: #f8f9fa;
                padding: 1rem;
                text-align: left;
                font-weight: 600;
                color: #424242;
                border-bottom: 2px solid #e0e0e0;
            }

            .employee-grid td {
                padding: 1rem;
                border-bottom: 1px solid #f0f0f0;
                vertical-align: middle;
            }

            .employee-grid tr:hover {
                background: #f8f9fa;
            }

        /* Employee Info */
        .employee-info {
            display: flex;
            align-items: center;
            gap: 0.75rem;
        }

        .employee-avatar {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            background: #1976d2;
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: 600;
            font-size: 0.9rem;
            flex-shrink: 0;
        }

        .employee-details {
            flex: 1;
        }

        .employee-name {
            font-weight: 600;
            color: #212121;
            margin-bottom: 0.25rem;
        }

        .employee-meta {
            display: flex;
            gap: 0.75rem;
            font-size: 0.85rem;
            color: #757575;
        }

        .employee-number {
            font-family: 'Courier New', monospace;
            background: #f5f5f5;
            padding: 0.1rem 0.3rem;
            border-radius: 3px;
        }

        .employee-position {
            font-style: italic;
        }

        /* Department Info */
        .department-info {
            text-align: left;
        }

        .department-name {
            font-weight: 500;
            color: #424242;
            padding: 0.5rem 0.75rem;
            background: #f8f9fa;
            border-radius: 4px;
            border-left: 3px solid #1976d2;
        }

        /* Hire Date Info */
        .hire-date-info {
            text-align: left;
        }

        .hire-date {
            font-weight: 500;
            color: #424242;
            margin-bottom: 0.25rem;
        }

        .days-since {
            font-size: 0.8rem;
            color: #757575;
        }

        /* Progress Bar */
        .progress-info {
            min-width: 200px;
        }

        .progress-bar-container {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            margin-bottom: 0.25rem;
        }

        .progress-bar {
            flex: 1;
            height: 8px;
            background: #e0e0e0;
            border-radius: 4px;
            overflow: hidden;
        }

        .progress-fill {
            height: 100%;
            background: linear-gradient(90deg, #4caf50, #66bb6a);
            transition: width 0.3s ease;
        }

        .progress-percentage {
            font-weight: 600;
            color: #424242;
            font-size: 0.85rem;
            min-width: 35px;
        }

        .task-summary {
            font-size: 0.8rem;
            color: #757575;
        }

        /* Status Badges */
        .status-badge {
            padding: 0.25rem 0.75rem;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
            text-transform: uppercase;
            white-space: nowrap;
        }

        .status-completed {
            background: #e8f5e8;
            color: #2e7d32;
        }

        .status-in-progress {
            background: #fff3e0;
            color: #ef6c00;
        }

        .status-pending {
            background: #e3f2fd;
            color: #1565c0;
        }

        .status-overdue {
            background: #ffebee;
            color: #c62828;
        }

        .status-no-tasks {
            background: #f5f5f5;
            color: #757575;
        }

        /* Issue Indicators */
        .issues-info {
            display: flex;
            flex-direction: column;
            gap: 0.25rem;
        }

        .issue-indicator {
            display: flex;
            align-items: center;
            gap: 0.25rem;
            font-size: 0.8rem;
            padding: 0.25rem 0.5rem;
            border-radius: 4px;
            white-space: nowrap;
        }

            .issue-indicator.overdue {
                background: #ffebee;
                color: #c62828;
            }

            .issue-indicator.pending {
                background: #e3f2fd;
                color: #1565c0;
            }

            .issue-indicator i {
                font-size: 0.9rem;
            }

        /* Action Buttons */
        .action-buttons {
            display: flex;
            gap: 0.5rem;
            align-items: center;
            flex-wrap: wrap;
        }

        .btn-sm {
            padding: 0.5rem 0.75rem;
            font-size: 0.8rem;
            display: flex;
            align-items: center;
            gap: 0.25rem;
        }

            .btn-sm i {
                font-size: 1rem;
            }

        /* Quick Actions Panel */
        .quick-actions-panel {
            position: fixed;
            top: 0;
            right: 0;
            width: 350px;
            height: 100vh;
            background: white;
            box-shadow: -4px 0 20px rgba(0, 0, 0, 0.15);
            z-index: 1000;
            transform: translateX(100%);
            transition: transform 0.3s ease;
        }

            .quick-actions-panel.show {
                transform: translateX(0);
            }

        .quick-actions-header {
            padding: 1.5rem;
            border-bottom: 1px solid #e0e0e0;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

            .quick-actions-header h4 {
                margin: 0;
                color: #212121;
            }

        .close-btn {
            width: 30px;
            height: 30px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.2rem;
            color: #757575;
        }

        .quick-actions-content {
            padding: 1.5rem;
        }

        .action-group {
            margin-bottom: 2rem;
        }

            .action-group label {
                display: block;
                font-weight: 600;
                color: #424242;
                margin-bottom: 0.5rem;
            }

        /* Date Range Filter */
        .date-range {
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .date-separator {
            color: #757575;
            font-size: 0.9rem;
        }

        .date-input {
            flex: 1;
        }

        /* Notification Panel */
        .notification-panel {
            position: fixed;
            top: 20px;
            right: 20px;
            background: white;
            border-radius: 8px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
            z-index: 1001;
            max-width: 400px;
            display: flex;
            align-items: center;
            gap: 1rem;
            padding: 1rem 1.5rem;
            border-left: 4px solid #4caf50;
        }

        .notification-content {
            flex: 1;
        }

        .notification-close {
            width: 24px;
            height: 24px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1rem;
            color: #757575;
        }

        /* Page Size Selector */
        .page-size-selector {
            min-width: 140px;
            font-size: 0.9rem;
        }

        /* Responsive Design */
        @media (max-width: 768px) {
            .stats-grid {
                grid-template-columns: 1fr;
            }

            .filter-row {
                flex-direction: column;
                gap: 1rem;
            }

            .date-range {
                flex-direction: column;
                align-items: stretch;
            }

            .employee-grid {
                font-size: 0.9rem;
            }

            .employee-meta {
                flex-direction: column;
                gap: 0.25rem;
                align-items: flex-start;
            }

            .action-buttons {
                flex-direction: column;
                align-items: stretch;
            }

            .btn-sm {
                justify-content: center;
            }

            .quick-actions-panel {
                width: 100%;
            }

            .progress-info {
                min-width: auto;
            }
        }
    </style>
</asp:Content>