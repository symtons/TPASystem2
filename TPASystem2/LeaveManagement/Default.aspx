<%@ Page Title="Leave Management" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TPASystem2.LeaveManagement.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
       <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
       <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <!-- Page Header -->
    <div class="page-header">
        <div class="page-title-wrapper">
            <h1 class="page-title">
                <i class="material-icons">event_available</i>
                Leave Management
            </h1>
            <p class="page-subtitle">Manage employee leave requests and approvals</p>
        </div>
        <div class="page-actions">
            <asp:Button ID="btnRequestLeave" runat="server" Text="Request Leave" 
                       CssClass="btn waves-effect waves-light" 
                       OnClick="btnRequestLeave_Click" />
            <asp:Button ID="btnLeaveCalendar" runat="server" Text="Leave Calendar" 
                       CssClass="btn btn-outline waves-effect waves-light" 
                       OnClick="btnLeaveCalendar_Click" />
        </div>
    </div>

    <!-- Message Panel -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert-panel">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Dashboard Stats -->
    <div class="stats-grid">
        <div class="stat-card">
            <div class="stat-icon">
                <i class="material-icons">pending_actions</i>
            </div>
            <div class="stat-content">
                <div class="stat-number">
                    <asp:Literal ID="litPendingRequests" runat="server">0</asp:Literal>
                </div>
                <div class="stat-label">Pending Requests</div>
            </div>
        </div>

        <div class="stat-card">
            <div class="stat-icon approved">
                <i class="material-icons">check_circle</i>
            </div>
            <div class="stat-content">
                <div class="stat-number">
                    <asp:Literal ID="litApprovedToday" runat="server">0</asp:Literal>
                </div>
                <div class="stat-label">Approved Today</div>
            </div>
        </div>

        <div class="stat-card">
            <div class="stat-icon warning">
                <i class="material-icons">event_busy</i>
            </div>
            <div class="stat-content">
                <div class="stat-number">
                    <asp:Literal ID="litOnLeaveToday" runat="server">0</asp:Literal>
                </div>
                <div class="stat-label">On Leave Today</div>
            </div>
        </div>

        <div class="stat-card">
            <div class="stat-icon info">
                <i class="material-icons">schedule</i>
            </div>
            <div class="stat-content">
                <div class="stat-number">
                    <asp:Literal ID="litUpcomingLeaves" runat="server">0</asp:Literal>
                </div>
                <div class="stat-label">Upcoming This Week</div>
            </div>
        </div>
    </div>

    <!-- Main Content Grid -->
    <div class="content-grid">
        <!-- Recent Leave Requests -->
        <div class="content-card">
            <div class="card-header">
                <h3>Recent Leave Requests</h3>
                <div class="card-actions">
                    <asp:DropDownList ID="ddlRequestFilter" runat="server" CssClass="form-control small" AutoPostBack="true" OnSelectedIndexChanged="ddlRequestFilter_SelectedIndexChanged">
                        <asp:ListItem Value="">All Requests</asp:ListItem>
                        <asp:ListItem Value="Pending">Pending</asp:ListItem>
                        <asp:ListItem Value="Approved">Approved</asp:ListItem>
                        <asp:ListItem Value="Rejected">Rejected</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
            <div class="card-content">
                <asp:GridView ID="gvRecentRequests" runat="server" CssClass="data-table responsive-table" 
                             AutoGenerateColumns="false" DataKeyNames="Id" 
                             OnRowCommand="gvRecentRequests_RowCommand"
                             OnRowDataBound="gvRecentRequests_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="EmployeeName" HeaderText="Employee" />
                        <asp:BoundField DataField="LeaveType" HeaderText="Type" />
                        <asp:BoundField DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:MMM dd, yyyy}" />
                        <asp:BoundField DataField="EndDate" HeaderText="End Date" DataFormatString="{0:MMM dd, yyyy}" />
                        <asp:BoundField DataField="DaysRequested" HeaderText="Days" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='status-badge status-<%# GetStatusClass(Eval("Status").ToString()) %>'>
                                    <%# Eval("Status") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="RequestedAt" HeaderText="Requested" DataFormatString="{0:MMM dd}" />
                        <asp:TemplateField HeaderText="Actions" ItemStyle-Width="120px">
                            <ItemTemplate>
                                <asp:LinkButton ID="btnView" runat="server" CssClass="btn-icon btn-small" 
                                               CommandName="ViewDetails" CommandArgument='<%# Eval("Id") %>' 
                                               ToolTip="View Details">
                                    <i class="material-icons">visibility</i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnApprove" runat="server" CssClass="btn-icon btn-small green" 
                                               CommandName="Approve" CommandArgument='<%# Eval("Id") %>' 
                                               ToolTip="Approve" Visible='<%# CanApprove(Eval("Status").ToString()) %>'>
                                    <i class="material-icons">check</i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnReject" runat="server" CssClass="btn-icon btn-small red" 
                                               CommandName="Reject" CommandArgument='<%# Eval("Id") %>' 
                                               ToolTip="Reject" Visible='<%# CanApprove(Eval("Status").ToString()) %>'>
                                    <i class="material-icons">close</i>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div class="empty-state">
                            <i class="material-icons">event_available</i>
                            <p>No leave requests found</p>
                        </div>
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </div>

        <!-- Leave Balance Summary -->
        <div class="content-card">
            <div class="card-header">
                <h3>Leave Balance Summary</h3>
                <div class="card-actions">
                    <asp:LinkButton ID="btnViewAllBalances" runat="server" CssClass="btn-link" OnClick="btnViewAllBalances_Click">
                        View All
                    </asp:LinkButton>
                </div>
            </div>
            <div class="card-content">
                <asp:Repeater ID="rptLeaveBalances" runat="server">
                    <HeaderTemplate>
                        <div class="balance-summary">
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="balance-item">
                            <div class="balance-type"><%# Eval("LeaveType") %></div>
                            <div class="balance-info">
                                <div class="balance-available">
                                    <span class="balance-number"><%# Eval("Available") %></span>
                                    <span class="balance-label">Available</span>
                                </div>
                                <div class="balance-used">
                                    <span class="balance-number"><%# Eval("Used") %></span>
                                    <span class="balance-label">Used</span>
                                </div>
                                <div class="balance-total">
                                    <span class="balance-number"><%# Eval("Total") %></span>
                                    <span class="balance-label">Total</span>
                                </div>
                            </div>
                            <div class="balance-progress">
                                <div class="progress-bar">
                                    <div class="progress-fill" style='width: <%# GetUsagePercentage(Eval("Used"), Eval("Total")) %>%'></div>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>

                <asp:Panel ID="pnlNoBalances" runat="server" Visible="false" CssClass="empty-state">
                    <i class="material-icons">account_balance</i>
                    <p>No leave balance information available</p>
                </asp:Panel>
            </div>
        </div>
    </div>

    <!-- Upcoming Leaves Section -->
    <div class="content-card full-width">
        <div class="card-header">
            <h3>Upcoming Leaves</h3>
            <div class="card-actions">
                <asp:DropDownList ID="ddlUpcomingFilter" runat="server" CssClass="form-control small" AutoPostBack="true" OnSelectedIndexChanged="ddlUpcomingFilter_SelectedIndexChanged">
                    <asp:ListItem Value="7">Next 7 Days</asp:ListItem>
                    <asp:ListItem Value="14">Next 14 Days</asp:ListItem>
                    <asp:ListItem Value="30" Selected="true">Next 30 Days</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="card-content">
            <asp:GridView ID="gvUpcomingLeaves" runat="server" CssClass="data-table responsive-table" 
                         AutoGenerateColumns="false" DataKeyNames="Id">
                <Columns>
                    <asp:BoundField DataField="EmployeeName" HeaderText="Employee" />
                    <asp:BoundField DataField="Department" HeaderText="Department" />
                    <asp:BoundField DataField="LeaveType" HeaderText="Type" />
                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="EndDate" HeaderText="End Date" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="DaysRequested" HeaderText="Days" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='status-badge status-<%# GetStatusClass(Eval("Status").ToString()) %>'>
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="empty-state">
                        <i class="material-icons">event</i>
                        <p>No upcoming leaves in the selected period</p>
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>

    <!-- Quick Actions Panel -->
    <div class="quick-actions-panel">
        <h4>Quick Actions</h4>
        <div class="quick-actions-grid">
            <asp:LinkButton ID="btnMyLeaves" runat="server" CssClass="quick-action-item" OnClick="btnMyLeaves_Click">
                <i class="material-icons">person</i>
                <span>My Leaves</span>
            </asp:LinkButton>
            <asp:LinkButton ID="btnApprovalQueue" runat="server" CssClass="quick-action-item" OnClick="btnApprovalQueue_Click">
                <i class="material-icons">approval</i>
                <span>Approval Queue</span>
            </asp:LinkButton>
            <asp:LinkButton ID="btnTeamCalendar" runat="server" CssClass="quick-action-item" OnClick="btnTeamCalendar_Click">
                <i class="material-icons">calendar_today</i>
                <span>Team Calendar</span>
            </asp:LinkButton>
            <asp:LinkButton ID="btnLeaveReports" runat="server" CssClass="quick-action-item" OnClick="btnLeaveReports_Click">
                <i class="material-icons">analytics</i>
                <span>Reports</span>
            </asp:LinkButton>
        </div>
    </div>

    <!-- CSS and JavaScript -->
    <style>
        /* Leave Management Specific Styles */
        .stats-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 1.5rem;
            margin-bottom: 2rem;
        }

        .stat-card {
            background: white;
            border-radius: 8px;
            padding: 1.5rem;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            display: flex;
            align-items: center;
            gap: 1rem;
        }

        .stat-icon {
            width: 60px;
            height: 60px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            background: #2196f3;
            color: white;
        }

        .stat-icon.approved { background: #4caf50; }
        .stat-icon.warning { background: #ff9800; }
        .stat-icon.info { background: #9c27b0; }

        .stat-number {
            font-size: 2rem;
            font-weight: bold;
            color: #212121;
        }

        .stat-label {
            color: #757575;
            font-size: 0.9rem;
        }

        .content-grid {
            display: grid;
            grid-template-columns: 2fr 1fr;
            gap: 2rem;
            margin-bottom: 2rem;
        }

        .content-card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            overflow: hidden;
        }

        .content-card.full-width {
            grid-column: 1 / -1;
        }

        .card-header {
            padding: 1.5rem;
            border-bottom: 1px solid #e0e0e0;
            display: flex;
            justify-content: space-between;
            align-items: center;
            background: #f8f9fa;
        }

        .card-header h3 {
            margin: 0;
            color: #212121;
        }

        .card-content {
            padding: 0;
        }

        .balance-summary {
            padding: 1rem;
        }

        .balance-item {
            border-bottom: 1px solid #f0f0f0;
            padding: 1rem 0;
        }

        .balance-item:last-child {
            border-bottom: none;
        }

        .balance-type {
            font-weight: bold;
            color: #212121;
            margin-bottom: 0.5rem;
        }

        .balance-info {
            display: flex;
            gap: 1.5rem;
            margin-bottom: 0.5rem;
        }

        .balance-available,
        .balance-used,
        .balance-total {
            text-align: center;
        }

        .balance-number {
            display: block;
            font-weight: bold;
            font-size: 1.1rem;
        }

        .balance-label {
            font-size: 0.8rem;
            color: #757575;
        }

        .balance-progress {
            width: 100%;
            background: #f0f0f0;
            height: 4px;
            border-radius: 2px;
            overflow: hidden;
        }

        .progress-fill {
            background: #2196f3;
            height: 100%;
            transition: width 0.3s ease;
        }

        .status-badge {
            padding: 0.25rem 0.5rem;
            border-radius: 12px;
            font-size: 0.8rem;
            font-weight: bold;
            text-transform: uppercase;
        }

        .status-pending {
            background: #fff3cd;
            color: #856404;
        }

        .status-approved {
            background: #d4edda;
            color: #155724;
        }

        .status-rejected {
            background: #f8d7da;
            color: #721c24;
        }

        .quick-actions-panel {
            background: white;
            border-radius: 8px;
            padding: 1.5rem;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }

        .quick-actions-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
            gap: 1rem;
            margin-top: 1rem;
        }

        .quick-action-item {
            display: flex;
            flex-direction: column;
            align-items: center;
            padding: 1rem;
            text-decoration: none;
            color: #424242;
            border: 1px solid #e0e0e0;
            border-radius: 8px;
            transition: all 0.2s ease;
        }

        .quick-action-item:hover {
            background: #f5f5f5;
            border-color: #2196f3;
            color: #2196f3;
        }

        .quick-action-item i {
            font-size: 2rem;
            margin-bottom: 0.5rem;
        }

        @media (max-width: 768px) {
            .content-grid {
                grid-template-columns: 1fr;
            }

            .stats-grid {
                grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            }

            .balance-info {
                flex-direction: column;
                gap: 0.5rem;
            }
        }
    </style>

    <script type="text/javascript">
        // Initialize tooltips
        document.addEventListener('DOMContentLoaded', function() {
            var tooltipElements = document.querySelectorAll('[data-tooltip]');
            if (typeof M !== 'undefined' && M.Tooltip) {
                M.Tooltip.init(tooltipElements);
            }
        });

        // Auto-refresh data every 5 minutes
        setInterval(function() {
            if (typeof __doPostBack !== 'undefined') {
                __doPostBack('<%= Page.ClientID %>', 'Refresh');
            }
        }, 300000);
    </script>
</asp:Content>