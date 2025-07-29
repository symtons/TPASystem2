<%@ Page Title="" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="OnboardingManagement.aspx.cs" Inherits="TPASystem2.OnBoarding.OnboardingManagement" %>

<asp:Content ID="OnboardingContent" ContentPlaceHolderID="DashboardContent" runat="server">

     <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/employee-management.css") %>' rel="stylesheet" type="text/css" />
    
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
            <div class="stat-content">
                <div class="stat-number">
                    <asp:Literal ID="litTotalEmployees" runat="server">0</asp:Literal>
                </div>
                <div class="stat-label">Total in Onboarding</div>
            </div>
        </div>
        
        <div class="stat-card success">
            <div class="stat-content">
                <div class="stat-number">
                    <asp:Literal ID="litCompletedEmployees" runat="server">0</asp:Literal>
                </div>
                <div class="stat-label">Completed</div>
            </div>
        </div>
        
        <div class="stat-card warning">
            <div class="stat-content">
                <div class="stat-number">
                    <asp:Literal ID="litInProgressEmployees" runat="server">0</asp:Literal>
                </div>
                <div class="stat-label">In Progress</div>
            </div>
        </div>
        
        <div class="stat-card danger">
            <div class="stat-content">
                <div class="stat-number">
                    <asp:Literal ID="litOverdueEmployees" runat="server">0</asp:Literal>
                </div>
                <div class="stat-label">Overdue</div>
            </div>
        </div>
    </div>

    <!-- Filters Section -->
    <div class="filters-section">
        <div class="filter-row">
            <div class="filter-group">
                <label>Search:</label>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search employees..."></asp:TextBox>
            </div>
            
            <div class="filter-group">
                <label>Department:</label>
                <asp:DropDownList ID="ddlFilterDepartment" runat="server" CssClass="form-control">
                </asp:DropDownList>
            </div>
            
            <div class="filter-group">
                <label>Status:</label>
                <asp:DropDownList ID="ddlFilterStatus" runat="server" CssClass="form-control">
                </asp:DropDownList>
            </div>
            
            <div class="filter-group">
                <label>From Date:</label>
                <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            
            <div class="filter-group">
                <label>To Date:</label>
                <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            
            <div class="filter-group">
                <asp:Button ID="btnClearFilters" runat="server" Text="Clear Filters" 
                           CssClass="btn btn-secondary" OnClick="btnClearFilters_Click" />
            </div>
        </div>
        
        <div class="filter-row">
            <div class="filter-group">
                <label>Page Size:</label>
                <asp:DropDownList ID="ddlPageSize" runat="server" CssClass="form-control">
                </asp:DropDownList>
            </div>
        </div>
    </div>

    <!-- Employee Grid -->
    <asp:Panel ID="pnlEmployeeGrid" runat="server" CssClass="grid-container">
        <asp:GridView ID="gvEmployees" runat="server" 
                      AutoGenerateColumns="false" 
                      CssClass="data-grid" 
                      AllowPaging="true" 
                      PageSize="25"
                      OnPageIndexChanging="gvEmployees_PageIndexChanging"
                      OnRowCommand="gvEmployees_RowCommand"
                      OnRowDataBound="gvEmployees_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="Employee">
                    <ItemTemplate>
                        <div class="employee-info">
                            <div class="employee-details">
                                <div class="employee-name"><%# Eval("FullName") %></div>
                                <div class="employee-meta">
                                    <span><%# Eval("EmployeeNumber") %></span> - 
                                    <span><%# Eval("Position") %></span>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:BoundField DataField="DepartmentName" HeaderText="Department" />
                
                <asp:TemplateField HeaderText="Hire Date">
                    <ItemTemplate>
                        <%# Eval("HireDate", "{0:yyyy-MM-dd}") %>
                        <small>(<%# Eval("DaysSinceHire") %> days ago)</small>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="Progress">
                    <ItemTemplate>
                        <div class="progress-info">
                            <div class="progress-text">
                                <%# Eval("CompletedTasks") %>/<%# Eval("TotalTasks") %> tasks
                                (<%# Eval("CompletionPercentage", "{0:F1}") %>%)
                            </div>
                            <div class="progress-bar">
                                <div class="progress-fill" style='width: <%# Eval("CompletionPercentage") %>%'></div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class="status-badge <%# GetStatusClass(Eval("OnboardingStatus").ToString()) %>">
                            <%# GetStatusDisplay(Eval("OnboardingStatus").ToString()) %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:Button ID="btnManage" runat="server" 
                                   Text="Manage" 
                                   CssClass="btn btn-sm btn-primary"
                                   CommandName="ManageOnboarding"
                                   CommandArgument='<%# Eval("EmployeeId") %>' />
                        <asp:Button ID="btnComplete" runat="server" 
                                   Text="Complete" 
                                   CssClass="btn btn-sm btn-success"
                                   CommandName="QuickComplete"
                                   CommandArgument='<%# Eval("EmployeeId") %>'
                                   OnClientClick="return confirm('Complete all onboarding tasks for this employee?');" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            
            <PagerStyle CssClass="pager" />
            <HeaderStyle CssClass="grid-header" />
            <RowStyle CssClass="grid-row" />
            <AlternatingRowStyle CssClass="grid-row-alt" />
        </asp:GridView>
    </asp:Panel>

    <!-- Bulk Actions Panel -->
    <asp:Panel ID="pnlQuickActions" runat="server" CssClass="bulk-actions-panel" Visible="false">
        <div class="bulk-actions-header">
            <h3>Bulk Actions</h3>
            <asp:Button ID="btnCloseBulkActions" runat="server" Text="×" 
                       CssClass="btn btn-close" OnClick="btnCloseBulkActions_Click" />
        </div>
        <div class="bulk-actions-content">
            <div class="form-group">
                <label>Select Action:</label>
                <asp:DropDownList ID="ddlBulkAction" runat="server" CssClass="form-control">
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <asp:Button ID="btnExecuteBulkAction" runat="server" Text="Execute" 
                           CssClass="btn btn-primary" OnClick="btnExecuteBulkAction_Click" />
            </div>
        </div>
    </asp:Panel>

    <!-- Notification Panel -->
    <asp:Panel ID="pnlNotification" runat="server" CssClass="notification" Visible="false">
        <asp:Literal ID="litNotificationMessage" runat="server"></asp:Literal>
        <asp:Button ID="btnCloseNotification" runat="server" Text="×" 
                   CssClass="btn btn-close" OnClick="btnCloseNotification_Click" />
    </asp:Panel>

    <!-- Basic Styles -->
    <style>
        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
            padding-bottom: 15px;
            border-bottom: 1px solid #ddd;
        }
        
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }
        
        .stat-card {
            background: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            border-left: 4px solid #007bff;
        }
        
        .stat-card.success { border-left-color: #28a745; }
        .stat-card.warning { border-left-color: #ffc107; }
        .stat-card.danger { border-left-color: #dc3545; }
        
        .stat-number {
            font-size: 2em;
            font-weight: bold;
            color: #333;
        }
        
        .stat-label {
            color: #666;
            margin-top: 5px;
        }
        
        .filters-section {
            background: white;
            padding: 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        
        .filter-row {
            display: flex;
            gap: 15px;
            align-items: end;
            margin-bottom: 15px;
        }
        
        .filter-group {
            display: flex;
            flex-direction: column;
            min-width: 150px;
        }
        
        .filter-group label {
            font-weight: bold;
            margin-bottom: 5px;
            color: #333;
        }
        
        .form-control {
            padding: 8px;
            border: 1px solid #ddd;
            border-radius: 4px;
            font-size: 14px;
        }
        
        .btn {
            padding: 8px 16px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
            text-decoration: none;
            display: inline-block;
        }
        
        .btn-primary { background: #007bff; color: white; }
        .btn-secondary { background: #6c757d; color: white; }
        .btn-success { background: #28a745; color: white; }
        .btn-outline { background: transparent; border: 1px solid #007bff; color: #007bff; }
        .btn-sm { padding: 4px 8px; font-size: 12px; }
        .btn-close { background: transparent; color: #999; padding: 4px 8px; }
        
        .grid-container {
            background: white;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        
        .data-grid {
            width: 100%;
            border-collapse: collapse;
        }
        
        .grid-header {
            background: #f8f9fa;
            font-weight: bold;
            padding: 12px;
            border-bottom: 2px solid #ddd;
        }
        
        .grid-row, .grid-row-alt {
            padding: 12px;
            border-bottom: 1px solid #eee;
        }
        
        .grid-row-alt {
            background: #f9f9f9;
        }
        
        .employee-info {
            display: flex;
            align-items: center;
        }
        
        .employee-name {
            font-weight: bold;
            color: #333;
        }
        
        .employee-meta {
            font-size: 12px;
            color: #666;
            margin-top: 2px;
        }
        
        .progress-info {
            min-width: 120px;
        }
        
        .progress-text {
            font-size: 12px;
            margin-bottom: 4px;
        }
        
        .progress-bar {
            width: 100%;
            height: 8px;
            background: #eee;
            border-radius: 4px;
            overflow: hidden;
        }
        
        .progress-fill {
            height: 100%;
            background: #28a745;
            transition: width 0.3s ease;
        }
        
        .status-badge {
            padding: 4px 8px;
            border-radius: 12px;
            font-size: 11px;
            font-weight: bold;
            text-transform: uppercase;
        }
        
        .status-badge.completed { background: #d4edda; color: #155724; }
        .status-badge.in-progress { background: #d1ecf1; color: #0c5460; }
        .status-badge.pending { background: #fff3cd; color: #856404; }
        .status-badge.overdue { background: #f8d7da; color: #721c24; }
        .status-badge.no-tasks { background: #e2e3e5; color: #383d41; }
        
        .bulk-actions-panel {
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            background: white;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.3);
            z-index: 1000;
            min-width: 300px;
        }
        
        .bulk-actions-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 1px solid #ddd;
        }
        
        .notification {
            position: fixed;
            top: 20px;
            right: 20px;
            padding: 15px 20px;
            background: #d4edda;
            color: #155724;
            border-radius: 4px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.2);
            z-index: 1000;
            max-width: 300px;
        }
        
        .notification.error {
            background: #f8d7da;
            color: #721c24;
        }
        
        .notification.warning {
            background: #fff3cd;
            color: #856404;
        }
        
        .row-overdue {
            background-color: #fff5f5 !important;
        }
        
        .pager {
            padding: 10px;
            text-align: center;
            background: #f8f9fa;
        }
    </style>
</asp:Content>