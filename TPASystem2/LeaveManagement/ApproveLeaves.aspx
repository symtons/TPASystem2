<%@ Page Title="Approve Leave Requests" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ApproveLeaves.aspx.cs" Inherits="TPASystem2.LeaveManagement.ApproveLeaves" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    
    <!-- Page Header -->
    <div class="page-header">
        <div class="page-title-wrapper">
            <h1 class="page-title">
                <i class="material-icons">approval</i>
                Approve Leave Requests
            </h1>
            <p class="page-subtitle">Review and approve pending leave requests</p>
        </div>
        <div class="page-actions">
            <asp:Button ID="btnBackToDashboard" runat="server" Text="Back to Dashboard" 
                       CssClass="btn btn-outline waves-effect waves-light" 
                       OnClick="btnBackToDashboard_Click" />
        </div>
    </div>

    <!-- Message Panel -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert-panel">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Filters Section -->
    <div class="content-card">
        <div class="card-header">
            <h3>Filter Requests</h3>
        </div>
        <div class="card-content">
            <div class="leave-filter-row">
                <div class="filter-group">
                    <label>Status</label>
                    <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-control" 
                                      AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                        <asp:ListItem Value="">All Requests</asp:ListItem>
                        <asp:ListItem Value="Pending" Selected="true">Pending</asp:ListItem>
                        <asp:ListItem Value="Approved">Approved</asp:ListItem>
                        <asp:ListItem Value="Rejected">Rejected</asp:ListItem>
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
                    <label>Date Range</label>
                    <asp:DropDownList ID="ddlDateRange" runat="server" CssClass="form-control" 
                                      AutoPostBack="true" OnSelectedIndexChanged="ddlDateRange_SelectedIndexChanged">
                        <asp:ListItem Value="30">Last 30 Days</asp:ListItem>
                        <asp:ListItem Value="60">Last 60 Days</asp:ListItem>
                        <asp:ListItem Value="90" Selected="true">Last 90 Days</asp:ListItem>
                        <asp:ListItem Value="0">All Time</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label>Employee Search</label>
                    <asp:TextBox ID="txtEmployeeSearch" runat="server" CssClass="form-control" 
                                placeholder="Search by employee name..." 
                                AutoPostBack="true" OnTextChanged="txtEmployeeSearch_TextChanged"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>

    <!-- Leave Requests Grid -->
    <div class="content-card">
        <div class="card-header">
            <h3>Leave Requests</h3>
            <div class="card-actions">
                <span class="badge badge-info">
                    <asp:Literal ID="litTotalRequests" runat="server">0</asp:Literal> requests found
                </span>
            </div>
        </div>
        <div class="card-content">
            <asp:GridView ID="gvLeaveRequests" runat="server" CssClass="data-table responsive-table" 
                         AutoGenerateColumns="false" DataKeyNames="Id" 
                         OnRowCommand="gvLeaveRequests_RowCommand"
                         OnRowDataBound="gvLeaveRequests_RowDataBound"
                         AllowPaging="true" PageSize="20"
                         OnPageIndexChanging="gvLeaveRequests_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="EmployeeName" HeaderText="Employee" />
                    <asp:BoundField DataField="Department" HeaderText="Department" />
                    <asp:BoundField DataField="LeaveType" HeaderText="Leave Type" />
                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="EndDate" HeaderText="End Date" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="DaysRequested" HeaderText="Days" />
                    <asp:BoundField DataField="RequestedAt" HeaderText="Requested" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class="leave-status-badge status-<%# Eval("Status").ToString().ToLower() %>">
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Actions" ItemStyle-Width="200px">
                        <ItemTemplate>
                            <div class="leave-action-buttons">
                                <asp:LinkButton ID="btnView" runat="server" 
                                              CommandName="ViewDetails" 
                                              CommandArgument='<%# Eval("Id") %>'
                                              CssClass="btn btn-sm btn-info"
                                              ToolTip="View Details">
                                    <i class="material-icons">visibility</i>
                                </asp:LinkButton>
                                
                                <asp:LinkButton ID="btnApprove" runat="server" 
                                              CommandName="Approve" 
                                              CommandArgument='<%# Eval("Id") %>'
                                              CssClass="btn btn-sm btn-success"
                                              ToolTip="Approve Request"
                                              OnClientClick="return confirm('Are you sure you want to approve this leave request?');"
                                              Visible='<%# Eval("Status").ToString() == "Pending" %>'>
                                    <i class="material-icons">check</i>
                                </asp:LinkButton>
                                
                                <asp:LinkButton ID="btnReject" runat="server" 
                                              CommandName="Reject" 
                                              CommandArgument='<%# Eval("Id") %>'
                                              CssClass="btn btn-sm btn-danger"
                                              ToolTip="Reject Request"
                                              OnClientClick="return confirm('Are you sure you want to reject this leave request?');"
                                              Visible='<%# Eval("Status").ToString() == "Pending" %>'>
                                    <i class="material-icons">close</i>
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div class="leave-empty-state">
                        <i class="material-icons">inbox</i>
                        <h3>No leave requests found</h3>
                        <p>There are no leave requests matching your current filters.</p>
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
                        <span class="leave-detail-label">Employee:</span>
                        <span class="leave-detail-value"><asp:Literal ID="litEmployeeName" runat="server"></asp:Literal></span>
                    </div>
                    <div class="leave-detail-row">
                        <span class="leave-detail-label">Department:</span>
                        <span class="leave-detail-value"><asp:Literal ID="litDepartment" runat="server"></asp:Literal></span>
                    </div>
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
                    <div class="leave-detail-row leave-detail-full-width">
                        <span class="leave-detail-label">Reason:</span>
                        <div class="leave-detail-value leave-reason-text">
                            <asp:Literal ID="litReason" runat="server"></asp:Literal>
                        </div>
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
                </div>
            </div>
            <div class="leave-modal-footer">
                <asp:Panel ID="pnlModalActions" runat="server">
                    <asp:Button ID="btnModalApprove" runat="server" Text="Approve" 
                               CssClass="btn btn-success waves-effect waves-light" 
                               OnClick="btnModalApprove_Click"
                               OnClientClick="return confirm('Are you sure you want to approve this leave request?');" />
                    <asp:Button ID="btnModalReject" runat="server" Text="Reject" 
                               CssClass="btn btn-danger waves-effect waves-light" 
                               OnClick="btnModalReject_Click"
                               OnClientClick="return confirm('Are you sure you want to reject this leave request?');" />
                </asp:Panel>
                <asp:Button ID="btnModalClose" runat="server" Text="Close" 
                           CssClass="btn btn-outline waves-effect waves-light" 
                           OnClick="btnCloseModal_Click" CausesValidation="false" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>