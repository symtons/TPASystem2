<%@ Page Title="My Leave Requests" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="MyLeaves.aspx.cs" Inherits="TPASystem2.LeaveManagement.MyLeaves" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    
    <!-- Page Header -->
    <div class="page-header">
        <div class="page-title-wrapper">
            <h1 class="page-title">
                <i class="material-icons">person_pin_circle</i>
                My Leave Requests
            </h1>
            <p class="page-subtitle">View and manage your leave requests</p>
        </div>
        <div class="page-actions">
            <asp:Button ID="btnRequestNewLeave" runat="server" Text="Request New Leave" 
                       CssClass="btn btn-primary waves-effect waves-light" 
                       OnClick="btnRequestNewLeave_Click" />
            <asp:Button ID="btnBackToDashboard" runat="server" Text="Back to Dashboard" 
                       CssClass="btn btn-outline waves-effect waves-light" 
                       OnClick="btnBackToDashboard_Click" />
        </div>
    </div>

    <!-- Message Panel -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert-panel">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Leave Balance Summary -->
    <div class="content-card">
        <div class="card-header">
            <h3>Leave Balance Summary</h3>
        </div>
        <div class="card-content">
            <div class="leave-balance-grid">
                <asp:Repeater ID="rptLeaveBalances" runat="server">
                    <ItemTemplate>
                        <div class="leave-balance-item">
                            <div class="balance-type"><%# Eval("LeaveType") %></div>
                            <div class="balance-details">
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
                                <div class="progress-bar" style="width: <%# GetUsagePercentage(Eval("Used"), Eval("Total")) %>%"></div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                
                <asp:Panel ID="pnlNoBalances" runat="server" Visible="false" CssClass="leave-empty-state">
                    <i class="material-icons">info</i>
                    <p>No leave balance information available.</p>
                </asp:Panel>
            </div>
        </div>
    </div>

    <!-- Filter Section -->
    <div class="content-card">
        <div class="card-header">
            <h3>Filter My Requests</h3>
        </div>
        <div class="card-content">
            <div class="leave-filter-row">
                <div class="filter-group">
                    <label>Status</label>
                    <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-control" 
                                      AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                        <asp:ListItem Value="">All Requests</asp:ListItem>
                        <asp:ListItem Value="Pending">Pending</asp:ListItem>
                        <asp:ListItem Value="Approved">Approved</asp:ListItem>
                        <asp:ListItem Value="Rejected">Rejected</asp:ListItem>
                        <asp:ListItem Value="Draft">Draft</asp:ListItem>
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
                <div class="filter-group">
                    <label>Time Period</label>
                    <asp:DropDownList ID="ddlTimePeriod" runat="server" CssClass="form-control" 
                                      AutoPostBack="true" OnSelectedIndexChanged="ddlTimePeriod_SelectedIndexChanged">
                        <asp:ListItem Value="30">Last 30 Days</asp:ListItem>
                        <asp:ListItem Value="90" Selected="true">Last 3 Months</asp:ListItem>
                        <asp:ListItem Value="180">Last 6 Months</asp:ListItem>
                        <asp:ListItem Value="365">Last Year</asp:ListItem>
                        <asp:ListItem Value="0">All Time</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
        </div>
    </div>

    <!-- My Leave Requests -->
    <div class="content-card">
        <div class="card-header">
            <h3>My Leave Requests</h3>
            <div class="card-actions">
                <span class="badge badge-info">
                    <asp:Literal ID="litTotalRequests" runat="server">0</asp:Literal> requests found
                </span>
            </div>
        </div>
        <div class="card-content">
            <asp:GridView ID="gvMyLeaves" runat="server" CssClass="data-table responsive-table" 
                         AutoGenerateColumns="false" DataKeyNames="Id" 
                         OnRowCommand="gvMyLeaves_RowCommand"
                         OnRowDataBound="gvMyLeaves_RowDataBound"
                         AllowPaging="true" PageSize="15"
                         OnPageIndexChanging="gvMyLeaves_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="LeaveType" HeaderText="Leave Type" />
                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="EndDate" HeaderText="End Date" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="DaysRequested" HeaderText="Days" />
                    <asp:BoundField DataField="RequestedAt" HeaderText="Requested On" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class="leave-status-badge status-<%# Eval("Status").ToString().ToLower() %>">
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Reviewed By">
                        <ItemTemplate>
                            <%# Eval("ReviewedBy") ?? "--" %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Actions" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <div class="leave-action-buttons">
                                <asp:LinkButton ID="btnView" runat="server" 
                                              CommandName="ViewDetails" 
                                              CommandArgument='<%# Eval("Id") %>'
                                              CssClass="btn btn-sm btn-info"
                                              ToolTip="View Details">
                                    <i class="material-icons">visibility</i>
                                </asp:LinkButton>
                                
                                <asp:LinkButton ID="btnEdit" runat="server" 
                                              CommandName="EditRequest" 
                                              CommandArgument='<%# Eval("Id") %>'
                                              CssClass="btn btn-sm btn-warning"
                                              ToolTip="Edit Request"
                                              Visible='<%# Eval("Status").ToString() == "Draft" || Eval("Status").ToString() == "Pending" %>'>
                                    <i class="material-icons">edit</i>
                                </asp:LinkButton>
                                
                                <asp:LinkButton ID="btnCancel" runat="server" 
                                              CommandName="CancelRequest" 
                                              CommandArgument='<%# Eval("Id") %>'
                                              CssClass="btn btn-sm btn-danger"
                                              ToolTip="Cancel Request"
                                              OnClientClick="return confirm('Are you sure you want to cancel this leave request?');"
                                              Visible='<%# Eval("Status").ToString() == "Pending" %>'>
                                    <i class="material-icons">cancel</i>
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="leave-empty-state">
                        <i class="material-icons">event_busy</i>
                        <h3>No leave requests found</h3>
                        <p>You haven't submitted any leave requests yet.</p>
                        <asp:Button ID="btnCreateFirstRequest" runat="server" Text="Create Your First Request" 
                                   CssClass="btn btn-primary" OnClick="btnRequestNewLeave_Click" />
                    </div>
                </EmptyDataTemplate>
                <PagerStyle CssClass="pager" />
            </asp:GridView>
        </div>
    </div>

    <!-- Leave Request Details Modal -->
    <asp:Panel ID="pnlRequestDetails" runat="server" CssClass="leave-modal-overlay" Visible="false">
        <div class="leave-modal-content">
            <div class="leave-modal-header">
                <h3>Leave Request Details</h3>
                <asp:LinkButton ID="btnCloseModal" runat="server" CssClass="leave-modal-close" OnClick="btnCloseModal_Click">
                    <i class="material-icons">close</i>
                </asp:LinkButton>
            </div>
            <div class="leave-modal-body">
                <div class="leave-details-grid">
                    <div class="leave-detail-row">
                        <span class="leave-detail-label">Leave Type:</span>
                        <span class="leave-detail-value"><asp:Literal ID="litLeaveType" runat="server"></asp:Literal></span>
                    </div>
                    <div class="leave-detail-row">
                        <span class="leave-detail-label">Start Date:</span>
                        <span class="leave-detail-value"><asp:Literal ID="litStartDate" runat="server"></asp:Literal></span>
                    </div>
                    <div class="leave-detail-row">
                        <span class="leave-detail-label">End Date:</span>
                        <span class="leave-detail-value"><asp:Literal ID="litEndDate" runat="server"></asp:Literal></span>
                    </div>
                    <div class="leave-detail-row">
                        <span class="leave-detail-label">Days Requested:</span>
                        <span class="leave-detail-value"><asp:Literal ID="litDaysRequested" runat="server"></asp:Literal></span>
                    </div>
                    <div class="leave-detail-row">
                        <span class="leave-detail-label">Status:</span>
                        <span class="leave-detail-value">
                            <span class="leave-status-badge" id="spanStatus" runat="server">
                                <asp:Literal ID="litStatus" runat="server"></asp:Literal>
                            </span>
                        </span>
                    </div>
                    <div class="leave-detail-row">
                        <span class="leave-detail-label">Requested On:</span>
                        <span class="leave-detail-value"><asp:Literal ID="litRequestedAt" runat="server"></asp:Literal></span>
                    </div>
                    <asp:Panel ID="pnlReviewInfo" runat="server" Visible="false">
                        <div class="leave-detail-row">
                            <span class="leave-detail-label">Reviewed By:</span>
                            <span class="leave-detail-value"><asp:Literal ID="litReviewedBy" runat="server"></asp:Literal></span>
                        </div>
                        <div class="leave-detail-row">
                            <span class="leave-detail-label">Reviewed On:</span>
                            <span class="leave-detail-value"><asp:Literal ID="litReviewedAt" runat="server"></asp:Literal></span>
                        </div>
                    </asp:Panel>
                    <div class="leave-detail-row leave-detail-full-width">
                        <span class="leave-detail-label">Reason:</span>
                        <div class="leave-detail-value leave-reason-text">
                            <asp:Literal ID="litReason" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>
            </div>
            <div class="leave-modal-footer">
                <asp:Panel ID="pnlModalActions" runat="server">
                    <asp:Button ID="btnModalEdit" runat="server" Text="Edit Request" 
                               CssClass="btn btn-warning waves-effect waves-light" 
                               OnClick="btnModalEdit_Click" />
                    <asp:Button ID="btnModalCancel" runat="server" Text="Cancel Request" 
                               CssClass="btn btn-danger waves-effect waves-light" 
                               OnClick="btnModalCancel_Click"
                               OnClientClick="return confirm('Are you sure you want to cancel this leave request?');" />
                </asp:Panel>
                <asp:Button ID="btnModalClose" runat="server" Text="Close" 
                           CssClass="btn btn-outline waves-effect waves-light" 
                           OnClick="btnCloseModal_Click" CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>