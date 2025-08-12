<%@ Page Title="My Leave Management" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="EmployeeLeavePortal.aspx.cs" Inherits="TPASystem2.LeaveManagement.EmployeeLeavePortal" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

    <!-- Welcome Header - Matching Time Tracking Style -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">event_available</i>
                    My Leave Management
                </h1>
                <p class="welcome-subtitle">Request time off and manage your leave requests</p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">person</i>
                        <span>
                            <asp:Literal ID="litEmployeeName" runat="server" Text="Employee Name"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">badge</i>
                        <span>
                            <asp:Literal ID="litEmployeeNumber" runat="server" Text="EMP001"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">business</i>
                        <span>
                            <asp:Literal ID="litDepartment" runat="server" Text="Department"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">supervisor_account</i>
                        <span>
                            <asp:Literal ID="litSupervisor" runat="server" Text="Program Director"></asp:Literal>
                        </span>
                    </div>
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnViewLeaveCalendar" runat="server" Text="View Leave Calendar" 
                    CssClass="btn btn-outline-light" OnClick="btnViewLeaveCalendar_Click" />
            </div>
        </div>
    </div>

    <!-- Alert Messages -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <i class="material-icons">info</i>
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Leave Balance Summary Card -->
    <div class="time-status-card">
        <div class="status-header">
            <h3>
                <i class="material-icons">account_balance</i>
                Leave Balance Summary
            </h3>
            <div class="status-indicator">
                <asp:Literal ID="litLeaveYear" runat="server" Text="2025"></asp:Literal>
            </div>
        </div>
        
        <div class="leave-balance-grid">
            <asp:Repeater ID="rptLeaveBalances" runat="server">
                <ItemTemplate>
                    <div class="balance-item">
                        <div class="balance-header">
                            <span class="balance-type"><%# Eval("LeaveType") %></span>
                            <span class="balance-available"><%# Eval("Available") %> days</span>
                        </div>
                        <div class="balance-bar">
                            <div class="balance-progress" style="width: <%# GetBalancePercentage(Eval("Available"), Eval("Total")) %>%"></div>
                        </div>
                        <div class="balance-details">
                            <small>Used: <%# Eval("Used") %> | Total: <%# Eval("Total") %></small>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            
            <asp:Panel ID="pnlNoBalances" runat="server" Visible="false" CssClass="no-data-message">
                <i class="material-icons">info</i>
                <span>No leave balances configured for your account.</span>
            </asp:Panel>
        </div>
    </div>

    <!-- Quick Actions -->
    <div class="clock-actions-container">
        <div class="clock-action-card">
            <div class="action-header">
                <h3>
                    <i class="material-icons">add_circle</i>
                    Request New Leave
                </h3>
                <p>Submit a new leave request for approval by your Program Director</p>
            </div>
            <div class="action-content">
                <asp:Button ID="btnRequestLeave" runat="server" Text="New Leave Request" 
                    CssClass="btn btn-primary btn-large" OnClick="btnRequestLeave_Click" />
            </div>
        </div>
        
        <div class="clock-action-card">
            <div class="action-header">
                <h3>
                    <i class="material-icons">list_alt</i>
                    My Leave Requests
                </h3>
                <p>View status and manage your submitted leave requests</p>
            </div>
            <div class="action-content">
                <asp:Button ID="btnViewMyLeaves" runat="server" Text="View My Requests" 
                    CssClass="btn btn-outline-primary btn-large" OnClick="btnViewMyLeaves_Click" />
            </div>
        </div>
    </div>

    <!-- Recent Leave Requests -->
    <div class="recent-entries-section">
        <div class="section-header">
            <h3>
                <i class="material-icons">history</i>
                Recent Leave Requests
            </h3>
            <div class="filters-controls">
                <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                    <asp:ListItem Value="" Text="All Status"></asp:ListItem>
                    <asp:ListItem Value="Pending" Text="Pending"></asp:ListItem>
                    <asp:ListItem Value="Approved" Text="Approved"></asp:ListItem>
                    <asp:ListItem Value="Rejected" Text="Rejected"></asp:ListItem>
                    <asp:ListItem Value="Draft" Text="Draft"></asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="btnViewAllRequests" runat="server" Text="View All" 
                    CssClass="btn btn-outline-secondary" OnClick="btnViewAllRequests_Click" />
            </div>
        </div>

        <div class="entries-container">
            <asp:Repeater ID="rptRecentRequests" runat="server" OnItemCommand="rptRecentRequests_ItemCommand">
                <ItemTemplate>
                    <div class="entry-card">
                        <div class="entry-header">
                            <div class="entry-title">
                                <span class="leave-type-badge leave-type-<%# Eval("LeaveType").ToString().ToLower() %>">
                                    <%# Eval("LeaveType") %>
                                </span>
                                <span class="entry-date"><%# Convert.ToDateTime(Eval("StartDate")).ToString("MMM dd") %> - <%# Convert.ToDateTime(Eval("EndDate")).ToString("MMM dd, yyyy") %></span>
                            </div>
                            <div class="entry-status">
                                <span class="status-badge status-<%# Eval("Status").ToString().ToLower() %>">
                                    <%# Eval("Status") %>
                                </span>
                            </div>
                        </div>
                        <div class="entry-details">
                            <div class="detail-item">
                                <i class="material-icons">schedule</i>
                                <span><%# Eval("DaysRequested") %> day(s)</span>
                            </div>
                            <div class="detail-item">
                                <i class="material-icons">access_time</i>
                                <span>Requested on <%# Convert.ToDateTime(Eval("RequestedAt")).ToString("MMM dd, yyyy") %></span>
                            </div>
                            <%# !string.IsNullOrEmpty(Eval("ReviewedAt")?.ToString()) ? 
                                "<div class=\"detail-item\"><i class=\"material-icons\">check_circle</i><span>Reviewed on " + 
                                Convert.ToDateTime(Eval("ReviewedAt")).ToString("MMM dd, yyyy") + "</span></div>" : "" %>
                        </div>
                        <%# !string.IsNullOrEmpty(Eval("Reason")?.ToString()) ? 
                            "<div class=\"entry-reason\"><strong>Reason:</strong> " + Eval("Reason") + "</div>" : "" %>
                        
                        <div class="entry-actions">
                            <asp:LinkButton ID="btnViewDetails" runat="server" 
                                CssClass="btn btn-outline-primary btn-sm" 
                                CommandName="ViewDetails" 
                                CommandArgument='<%# Eval("Id") %>'
                                Text="View Details" />
                            
                            <%# Eval("Status").ToString() == "Pending" || Eval("Status").ToString() == "Draft" ? 
                                "<asp:LinkButton ID=\"btnEdit\" runat=\"server\" CssClass=\"btn btn-outline-secondary btn-sm\" CommandName=\"Edit\" CommandArgument=\"" + Eval("Id") + "\" Text=\"Edit\" />" : "" %>
                            
                            <%# Eval("Status").ToString() == "Pending" ? 
                                "<asp:LinkButton ID=\"btnCancel\" runat=\"server\" CssClass=\"btn btn-outline-danger btn-sm\" CommandName=\"Cancel\" CommandArgument=\"" + Eval("Id") + "\" Text=\"Cancel\" OnClientClick=\"return confirm('Are you sure you want to cancel this request?');\" />" : "" %>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlNoRequests" runat="server" Visible="false" CssClass="no-data-card">
                <i class="material-icons">event_busy</i>
                <h4>No Leave Requests Found</h4>
                <p>You haven't submitted any leave requests yet. Click "New Leave Request" to get started.</p>
                <asp:Button ID="btnCreateFirstRequest" runat="server" Text="Create Your First Request" 
                    CssClass="btn btn-primary" OnClick="btnRequestLeave_Click" />
            </asp:Panel>
        </div>
    </div>

    <!-- Authorization Notice -->
    <div class="authorization-notice">
        <div class="notice-content">
            <i class="material-icons">info</i>
            <div class="notice-text">
                <strong>Authorization Process:</strong>
                All leave requests require approval from your Program Director before being finalized. 
                You will receive email notifications when your request is reviewed.
            </div>
        </div>
    </div>

    <!-- Hidden Fields for Client-Side Operations -->
    <asp:HiddenField ID="hfSelectedRequestId" runat="server" />
    
    <script type="text/javascript">
        $(document).ready(function () {
            // Initialize tooltips
            initializeTooltips();

            // Add loading states to buttons
            setupButtonStates();

            // Initialize balance progress animations
            animateBalanceProgressBars();
        });

        function initializeTooltips() {
            $('[data-toggle="tooltip"]').tooltip();
        }

        function setupButtonStates() {
            $('.btn').on('click', function () {
                const btn = $(this);
                if (!btn.hasClass('btn-no-loading')) {
                    btn.addClass('loading');
                    setTimeout(() => btn.removeClass('loading'), 3000);
                }
            });
        }

        function animateBalanceProgressBars() {
            $('.balance-progress').each(function () {
                const width = $(this).css('width');
                $(this).css('width', '0');
                $(this).animate({ width: width }, 1000);
            });
        }

        // Show confirmation for cancellation
        function confirmCancellation(requestId, leaveType) {
            return confirm(`Are you sure you want to cancel your ${leaveType} request? This action cannot be undone.`);
        }
    </script>
    
</asp:Content>