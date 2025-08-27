<%@ Page Title="HR Actions Approval" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="HRActionsApproval.aspx.cs" Inherits="TPASystem2.HR.HRActionsApproval" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

    <!-- Welcome Header -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">fact_check</i>
                    HR Actions Approval
                </h1>
                <p class="welcome-subtitle">Review and approve pending HR Action forms from employees</p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">person</i>
                        <span>
                            <asp:Literal ID="litCurrentUser" runat="server" Text="HR Admin"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">pending_actions</i>
                        <span>
                            <asp:Literal ID="litPendingCount" runat="server" Text="0"></asp:Literal> Pending Approvals
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">supervisor_account</i>
                        <span>HR Admin Role</span>
                    </div>
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnRefresh" runat="server" Text="Refresh" 
                    CssClass="btn btn-outline-light" OnClick="btnRefresh_Click" />
            </div>
        </div>
    </div>

    <!-- Alert Messages -->
    <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="alert alert-success">
        <i class="material-icons">check_circle</i>
        <div>
            <strong>Success!</strong>
            <asp:Literal ID="litSuccessMessage" runat="server"></asp:Literal>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="alert alert-error">
        <i class="material-icons">error</i>
        <div>
            <strong>Error!</strong>
            <asp:Literal ID="litErrorMessage" runat="server"></asp:Literal>
        </div>
    </asp:Panel>

    <!-- Filters Section -->
    <div class="form-container">
        <div class="section-header">
            <i class="material-icons">filter_list</i>
            <h4>Filter HR Actions</h4>
        </div>
        
        <div class="form-grid">
            <div class="form-group">
                <label class="form-label">Status</label>
                <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-input" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                    <asp:ListItem Value="" Text="All Statuses"></asp:ListItem>
                    <asp:ListItem Value="PENDING" Text="Pending" Selected="true"></asp:ListItem>
                    <asp:ListItem Value="APPROVED" Text="Approved"></asp:ListItem>
                    <asp:ListItem Value="REJECTED" Text="Rejected"></asp:ListItem>
                    <asp:ListItem Value="DRAFT" Text="Draft"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label class="form-label">Employee</label>
                <asp:DropDownList ID="ddlEmployeeFilter" runat="server" CssClass="form-input" AutoPostBack="true" OnSelectedIndexChanged="ddlEmployeeFilter_SelectedIndexChanged">
                    <asp:ListItem Value="" Text="All Employees"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        
        <div class="form-grid">
            <div class="form-group">
                <label class="form-label">Action Type</label>
                <asp:DropDownList ID="ddlActionTypeFilter" runat="server" CssClass="form-input" AutoPostBack="true" OnSelectedIndexChanged="ddlActionTypeFilter_SelectedIndexChanged">
                    <asp:ListItem Value="" Text="All Action Types"></asp:ListItem>
                    <asp:ListItem Value="RateChange" Text="Rate Change"></asp:ListItem>
                    <asp:ListItem Value="Transfer" Text="Transfer"></asp:ListItem>
                    <asp:ListItem Value="Promotion" Text="Promotion"></asp:ListItem>
                    <asp:ListItem Value="StatusChange" Text="Status Change"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label class="form-label">Date Range</label>
                <div style="display: flex; gap: 1rem;">
                    <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-input" TextMode="Date" placeholder="From Date"></asp:TextBox>
                    <asp:TextBox ID="txtToDate" runat="server" CssClass="form-input" TextMode="Date" placeholder="To Date"></asp:TextBox>
                </div>
            </div>
        </div>
        
        <div class="form-footer">
            <div class="form-footer-left">
                <span>Filter results to find specific HR Actions</span>
            </div>
            <div class="form-footer-right">
                <asp:Button ID="btnApplyFilters" runat="server" Text="Apply Filters" 
                    CssClass="btn btn-tpa" OnClick="btnApplyFilters_Click" />
                <asp:Button ID="btnClearFilters" runat="server" Text="Clear" 
                    CssClass="btn btn-outline" OnClick="btnClearFilters_Click" />
            </div>
        </div>
    </div>

    <!-- HR Actions List -->
    <div class="form-container">
        <div class="section-header">
            <i class="material-icons">list</i>
            <h4>HR Action Requests</h4>
        </div>

        <!-- No Results Panel -->
        <asp:Panel ID="pnlNoResults" runat="server" Visible="false" CssClass="no-data-card">
            <i class="material-icons">assignment_late</i>
            <h4>No HR Actions Found</h4>
            <p>There are no HR Action forms matching your current filters.</p>
            <asp:Button ID="btnViewAll" runat="server" Text="View All Requests" 
                CssClass="btn btn-tpa" OnClick="btnViewAll_Click" />
        </asp:Panel>

        <!-- HR Actions Repeater -->
        <asp:Repeater ID="rptHRActions" runat="server" OnItemCommand="rptHRActions_ItemCommand" OnItemDataBound="rptHRActions_ItemDataBound">
            <ItemTemplate>
                <div class="hr-action-card">
                    <div class="hr-action-header">
                        <div class="hr-action-info">
                            <div class="hr-action-title">
                                <h5>HR Action #<%# Eval("Id") %></h5>
                                <div class="hr-action-meta">
                                    <span class="hr-action-employee">
                                        <i class="material-icons">person</i>
                                        <%# Eval("EmployeeName") %>
                                    </span>
                                    <span class="hr-action-date">
                                        <i class="material-icons">schedule</i>
                                        <%# Convert.ToDateTime(Eval("RequestDate")).ToString("MMM dd, yyyy") %>
                                    </span>
                                </div>
                            </div>
                            <div class="hr-action-status">
                                <span class="status-badge status-<%# Eval("Status").ToString().ToLower() %>">
                                    <%# GetStatusDisplayText(Eval("Status").ToString()) %>
                                </span>
                            </div>
                        </div>
                        
                        <div class="hr-action-types">
                            <asp:Panel ID="pnlActionTypes" runat="server">
                                <!-- Action types will be populated in code-behind -->
                            </asp:Panel>
                        </div>
                    </div>

                    <div class="hr-action-summary">
                        <div class="summary-grid">
                            <div class="summary-item" runat="server" id="divRateChange" visible="false">
                                <div class="summary-label">Rate Change</div>
                                <div class="summary-value">
                                    $<%# Eval("PreviousRateSalary") %> → $<%# Eval("NewRateSalary") %>
                                </div>
                            </div>
                            <div class="summary-item" runat="server" id="divTransfer" visible="false">
                                <div class="summary-label">Transfer</div>
                                <div class="summary-value">
                                    <%# Eval("NewLocation") %>
                                </div>
                            </div>
                            <div class="summary-item" runat="server" id="divPromotion" visible="false">
                                <div class="summary-label">Promotion</div>
                                <div class="summary-value">
                                    <%# Eval("OldJobTitle") %> → <%# Eval("NewJobTitle") %>
                                </div>
                            </div>
                            <div class="summary-item" runat="server" id="divRequestedBy" visible="true">
                                <div class="summary-label">Requested By</div>
                                <div class="summary-value">
                                    <%# Eval("RequestedByName") %>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="hr-action-actions">
                        <asp:Button ID="btnViewDetails" runat="server" Text="View Details" 
                            CssClass="btn btn-outline btn-sm" CommandName="ViewDetails" 
                            CommandArgument='<%# Eval("Id") %>' />
                        
                        <asp:Panel ID="pnlPendingActions" runat="server" Visible='<%# Eval("Status").ToString() == "PENDING" %>'>
                            <asp:Button ID="btnApprove" runat="server" Text="Approve" 
                                CssClass="btn btn-success btn-sm" CommandName="Approve" 
                                CommandArgument='<%# Eval("Id") %>' 
                                OnClientClick="return confirm('Are you sure you want to approve this HR Action? This will update the employee information.');" />
                            
                            <asp:Button ID="btnReject" runat="server" Text="Reject" 
                                CssClass="btn btn-danger btn-sm" CommandName="Reject" 
                                CommandArgument='<%# Eval("Id") %>' 
                                OnClientClick="return confirm('Are you sure you want to reject this HR Action?');" />
                        </asp:Panel>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <!-- HR Action Details Modal -->
    <asp:Panel ID="pnlDetailsModal" runat="server" Visible="false" CssClass="modal-overlay">
        <div class="modal-container">
            <div class="modal-header">
                <h3>
                    <i class="material-icons">assignment</i>
                    HR Action Details #<asp:Literal ID="litModalHRActionId" runat="server"></asp:Literal>
                </h3>
                <asp:Button ID="btnCloseModal" runat="server" Text="×" CssClass="btn-close" OnClick="btnCloseModal_Click" />
            </div>
            
            <div class="modal-body">
                <!-- Employee Information -->
                <div class="detail-section">
                    <h4><i class="material-icons">person</i> Employee Information</h4>
                    <div class="detail-grid">
                        <div class="detail-item">
                            <span class="detail-label">Employee:</span>
                            <span class="detail-value"><asp:Literal ID="litModalEmployee" runat="server"></asp:Literal></span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label">Request Date:</span>
                            <span class="detail-value"><asp:Literal ID="litModalRequestDate" runat="server"></asp:Literal></span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label">Requested By:</span>
                            <span class="detail-value"><asp:Literal ID="litModalRequestedBy" runat="server"></asp:Literal></span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label">Status:</span>
                            <span class="detail-value">
                                <span class="status-badge" id="spanModalStatus" runat="server">
                                    <asp:Literal ID="litModalStatus" runat="server"></asp:Literal>
                                </span>
                            </span>
                        </div>
                    </div>
                </div>

                <!-- Dynamic Action Details -->
                <asp:Panel ID="pnlModalRateChange" runat="server" Visible="false" CssClass="detail-section">
                    <h4><i class="material-icons">attach_money</i> Rate Change Details</h4>
                    <div class="detail-grid">
                        <div class="detail-item">
                            <span class="detail-label">Previous Rate:</span>
                            <span class="detail-value">$<asp:Literal ID="litModalPreviousRate" runat="server"></asp:Literal></span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label">New Rate:</span>
                            <span class="detail-value">$<asp:Literal ID="litModalNewRate" runat="server"></asp:Literal></span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label">Increase Amount:</span>
                            <span class="detail-value">$<asp:Literal ID="litModalIncrease" runat="server"></asp:Literal></span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label">Rate Type:</span>
                            <span class="detail-value"><asp:Literal ID="litModalRateType" runat="server"></asp:Literal></span>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlModalTransfer" runat="server" Visible="false" CssClass="detail-section">
                    <h4><i class="material-icons">swap_horiz</i> Transfer Details</h4>
                    <div class="detail-grid">
                        <div class="detail-item">
                            <span class="detail-label">New Location:</span>
                            <span class="detail-value"><asp:Literal ID="litModalNewLocation" runat="server"></asp:Literal></span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label">Supervisor:</span>
                            <span class="detail-value"><asp:Literal ID="litModalSupervisor" runat="server"></asp:Literal></span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label">New Class:</span>
                            <span class="detail-value"><asp:Literal ID="litModalNewClass" runat="server"></asp:Literal></span>
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlModalPromotion" runat="server" Visible="false" CssClass="detail-section">
                    <h4><i class="material-icons">trending_up</i> Promotion Details</h4>
                    <div class="detail-grid">
                        <div class="detail-item">
                            <span class="detail-label">Old Job Title:</span>
                            <span class="detail-value"><asp:Literal ID="litModalOldJobTitle" runat="server"></asp:Literal></span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label">New Job Title:</span>
                            <span class="detail-value"><asp:Literal ID="litModalNewJobTitle" runat="server"></asp:Literal></span>
                        </div>
                        <div class="detail-item">
                            <span class="detail-label">New Salary:</span>
                            <span class="detail-value">$<asp:Literal ID="litModalPromotionSalary" runat="server"></asp:Literal></span>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Contact Changes -->
                <asp:Panel ID="pnlModalContact" runat="server" Visible="false" CssClass="detail-section">
                    <h4><i class="material-icons">contact_mail</i> Contact Information Changes</h4>
                    <div class="detail-grid">
                        <div class="detail-item" runat="server" id="divModalNewName" visible="false">
                            <span class="detail-label">New Name:</span>
                            <span class="detail-value"><asp:Literal ID="litModalNewName" runat="server"></asp:Literal></span>
                        </div>
                        <div class="detail-item" runat="server" id="divModalNewPhone" visible="false">
                            <span class="detail-label">New Phone:</span>
                            <span class="detail-value"><asp:Literal ID="litModalNewPhone" runat="server"></asp:Literal></span>
                        </div>
                        <div class="detail-item" runat="server" id="divModalNewEmail" visible="false">
                            <span class="detail-label">New Email:</span>
                            <span class="detail-value"><asp:Literal ID="litModalNewEmail" runat="server"></asp:Literal></span>
                        </div>
                        <div class="detail-item" runat="server" id="divModalNewAddress" visible="false">
                            <span class="detail-label">New Address:</span>
                            <span class="detail-value"><asp:Literal ID="litModalNewAddress" runat="server"></asp:Literal></span>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Additional Comments -->
                <asp:Panel ID="pnlModalComments" runat="server" Visible="false" CssClass="detail-section">
                    <h4><i class="material-icons">comment</i> Additional Comments</h4>
                    <div class="comments-display">
                        <asp:Literal ID="litModalComments" runat="server"></asp:Literal>
                    </div>
                </asp:Panel>

                <!-- Approval Actions -->
                <asp:Panel ID="pnlModalApprovalActions" runat="server" Visible="false" CssClass="detail-section approval-section">
                    <h4><i class="material-icons">fact_check</i> Take Action</h4>
                    
                    <div class="form-group">
                        <label class="form-label">Comments (Optional)</label>
                        <asp:TextBox ID="txtModalComments" runat="server" CssClass="form-input" 
                            TextMode="MultiLine" Rows="3" placeholder="Add any comments for this approval..."></asp:TextBox>
                    </div>
                    
                    <div class="approval-actions">
                        <asp:Button ID="btnModalApprove" runat="server" Text="Approve HR Action" 
                            CssClass="btn btn-success" OnClick="btnModalApprove_Click" 
                            OnClientClick="return confirm('Are you sure you want to approve this HR Action? This will update the employee information and cannot be undone.');" />
                        
                        <asp:Button ID="btnModalReject" runat="server" Text="Reject HR Action" 
                            CssClass="btn btn-danger" OnClick="btnModalReject_Click" 
                            OnClientClick="return confirm('Are you sure you want to reject this HR Action?');" />
                    </div>
                </asp:Panel>

                <!-- Approval History -->
                <asp:Panel ID="pnlModalApprovalHistory" runat="server" Visible="false" CssClass="detail-section">
                    <h4><i class="material-icons">history</i> Approval History</h4>
                    <div class="approval-timeline">
                        <div class="timeline-item">
                            <div class="timeline-icon">
                                <i class="material-icons">send</i>
                            </div>
                            <div class="timeline-content">
                                <div class="timeline-title">Request Submitted</div>
                                <div class="timeline-date">
                                    <asp:Literal ID="litModalSubmittedDate" runat="server"></asp:Literal>
                                </div>
                            </div>
                        </div>
                        
                        <asp:Panel ID="pnlModalApprovedTimeline" runat="server" Visible="false">
                            <div class="timeline-item approved">
                                <div class="timeline-icon">
                                    <i class="material-icons">check_circle</i>
                                </div>
                                <div class="timeline-content">
                                    <div class="timeline-title">Approved</div>
                                    <div class="timeline-person">
                                        Approved by: <asp:Literal ID="litModalApprovedBy" runat="server"></asp:Literal>
                                    </div>
                                    <div class="timeline-date">
                                        <asp:Literal ID="litModalApprovedDate" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                        
                        <asp:Panel ID="pnlModalRejectedTimeline" runat="server" Visible="false">
                            <div class="timeline-item rejected">
                                <div class="timeline-icon">
                                    <i class="material-icons">cancel</i>
                                </div>
                                <div class="timeline-content">
                                    <div class="timeline-title">Rejected</div>
                                    <div class="timeline-person">
                                        Rejected by: <asp:Literal ID="litModalRejectedBy" runat="server"></asp:Literal>
                                    </div>
                                    <div class="timeline-date">
                                        <asp:Literal ID="litModalRejectedDate" runat="server"></asp:Literal>
                                    </div>
                                    <div class="timeline-reason">
                                        Reason: <asp:Literal ID="litModalRejectionReason" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                        </asp:Panel>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>

    <!-- Hidden field to store selected HR Action ID -->
    <asp:HiddenField ID="hfSelectedHRActionId" runat="server" />

    <style>
        /* HR Actions Approval Specific Styles */
        .hr-action-card {
            background: white;
            border-radius: 12px;
            border: 2px solid #e5e7eb;
            margin-bottom: 1.5rem;
            transition: all 0.3s ease;
            overflow: hidden;
        }
        
        .hr-action-card:hover {
            border-color: #1976d2;
            box-shadow: 0 8px 24px rgba(25, 118, 210, 0.15);
            transform: translateY(-2px);
        }
        
        .hr-action-header {
            padding: 1.5rem;
            border-bottom: 1px solid #f0f0f0;
            background: linear-gradient(90deg, #fafbfc 0%, #fff 100%);
        }
        
        .hr-action-info {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 1rem;
        }
        
        .hr-action-title h5 {
            margin: 0 0 0.5rem 0;
            color: #1e293b;
            font-size: 1.2rem;
            font-weight: 600;
        }
        
        .hr-action-meta {
            display: flex;
            gap: 1.5rem;
            flex-wrap: wrap;
        }
        
        .hr-action-meta span {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            color: #64748b;
            font-size: 0.9rem;
        }
        
        .hr-action-meta .material-icons {
            font-size: 1rem;
        }
        
        .hr-action-types {
            display: flex;
            gap: 0.75rem;
            flex-wrap: wrap;
        }
        
        .action-type-badge {
            background: #1976d2;
            color: white;
            padding: 0.4rem 0.8rem;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 500;
            display: flex;
            align-items: center;
            gap: 0.25rem;
        }
        
        .action-type-badge.rate-change { background: #10b981; }
        .action-type-badge.transfer { background: #f59e0b; }
        .action-type-badge.promotion { background: #8b5cf6; }
        .action-type-badge.status-change { background: #ef4444; }
        
        .hr-action-summary {
            padding: 1.5rem;
            border-bottom: 1px solid #f0f0f0;
        }
        
        .summary-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1rem;
        }
        
        .summary-item {
            background: #f8fafc;
            padding: 1rem;
            border-radius: 8px;
            border-left: 4px solid #1976d2;
        }
        
        .summary-label {
            font-size: 0.8rem;
            color: #64748b;
            text-transform: uppercase;
            font-weight: 600;
            margin-bottom: 0.25rem;
        }
        
        .summary-value {
            font-size: 1rem;
            color: #1e293b;
            font-weight: 600;
        }
        
        .hr-action-actions {
            padding: 1.5rem;
            display: flex;
            gap: 1rem;
            align-items: center;
            background: #fafbfc;
        }
        
        .btn-sm {
            padding: 0.6rem 1.2rem;
            font-size: 0.85rem;
            min-width: 100px;
        }
        
        /* Modal Styles */
        .modal-overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.7);
            backdrop-filter: blur(8px);
            z-index: 1000;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 2rem;
        }
        
        .modal-container {
            background: white;
            border-radius: 16px;
            width: 100%;
            max-width: 800px;
            max-height: 90vh;
            overflow: hidden;
            box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
        }
        
        .modal-header {
            padding: 2rem;
            border-bottom: 2px solid #e5e7eb;
            background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
            color: white;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        
        .modal-header h3 {
            margin: 0;
            font-size: 1.5rem;
            font-weight: 600;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .btn-close {
            background: rgba(255, 255, 255, 0.2);
            color: white;
            border: none;
            width: 40px;
            height: 40px;
            border-radius: 50%;
            font-size: 1.5rem;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        .btn-close:hover {
            background: rgba(255, 255, 255, 0.3);
        }
        
        .modal-body {
            padding: 2rem;
            max-height: calc(90vh - 120px);
            overflow-y: auto;
        }
        
        .detail-section {
            margin-bottom: 2rem;
            padding-bottom: 1.5rem;
            border-bottom: 1px solid #f0f0f0;
        }
        
        .detail-section:last-child {
            border-bottom: none;
            margin-bottom: 0;
        }
        
        .detail-section h4 {
            margin: 0 0 1rem 0;
            color: #1e293b;
            font-size: 1.1rem;
            font-weight: 600;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .detail-section h4 .material-icons {
            color: #1976d2;
            font-size: 1.25rem;
        }
        
        .detail-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 1rem;
        }
        
        .detail-item {
            display: flex;
            flex-direction: column;
            gap: 0.25rem;
        }
        
        .detail-label {
            font-size: 0.85rem;
            color: #64748b;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        
        .detail-value {
            color: #1e293b;
            font-weight: 500;
            font-size: 1rem;
        }
        
        .comments-display {
            background: #f8fafc;
            padding: 1.5rem;
            border-radius: 8px;
            border-left: 4px solid #1976d2;
            font-style: italic;
            color: #374151;
            line-height: 1.6;
        }
        
        .approval-section {
            background: #fefce8;
            border: 2px solid #fbbf24;
            border-radius: 12px;
            padding: 2rem;
        }
        
        .approval-actions {
            display: flex;
            gap: 1rem;
            justify-content: flex-end;
            margin-top: 1.5rem;
        }
        
        .approval-timeline {
            display: flex;
            flex-direction: column;
            gap: 1rem;
        }
        
        .timeline-item {
            display: flex;
            align-items: flex-start;
            gap: 1rem;
        }
        
        .timeline-icon {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            background: #e5e7eb;
            display: flex;
            align-items: center;
            justify-content: center;
            flex-shrink: 0;
        }
        
        .timeline-item.approved .timeline-icon {
            background: #dcfce7;
            color: #16a34a;
        }
        
        .timeline-item.rejected .timeline-icon {
            background: #fee2e2;
            color: #dc2626;
        }
        
        .timeline-content {
            flex: 1;
            padding-top: 0.25rem;
        }
        
        .timeline-title {
            font-weight: 600;
            color: #1e293b;
            margin-bottom: 0.25rem;
        }
        
        .timeline-person {
            font-size: 0.9rem;
            color: #64748b;
            margin-bottom: 0.25rem;
        }
        
        .timeline-date {
            font-size: 0.85rem;
            color: #9ca3af;
        }
        
        .timeline-reason {
            font-size: 0.9rem;
            color: #dc2626;
            margin-top: 0.5rem;
            font-style: italic;
        }
        
        /* Status badges */
        .status-badge {
            padding: 0.4rem 0.8rem;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            display: inline-flex;
            align-items: center;
            gap: 0.25rem;
        }
        
        .status-badge.status-pending {
            background: #fef3c7;
            color: #d97706;
        }
        
        .status-badge.status-approved {
            background: #dcfce7;
            color: #16a34a;
        }
        
        .status-badge.status-rejected {
            background: #fee2e2;
            color: #dc2626;
        }
        
        .status-badge.status-draft {
            background: #f3f4f6;
            color: #6b7280;
        }
        
        /* No data card */
        .no-data-card {
            text-align: center;
            padding: 3rem 2rem;
            background: white;
            border-radius: 12px;
            border: 2px dashed #e5e7eb;
        }
        
        .no-data-card .material-icons {
            font-size: 4rem;
            color: #9ca3af;
            margin-bottom: 1rem;
        }
        
        .no-data-card h4 {
            color: #6b7280;
            margin-bottom: 0.5rem;
        }
        
        .no-data-card p {
            color: #9ca3af;
            margin-bottom: 2rem;
        }
        
        /* Button variants */
        .btn-success {
            background: linear-gradient(135deg, #16a34a 0%, #15803d 100%);
            color: white;
            border: none;
            box-shadow: 0 4px 12px rgba(34, 197, 94, 0.3);
        }
        
        .btn-success:hover {
            background: linear-gradient(135deg, #15803d 0%, #166534 100%);
            transform: translateY(-1px);
            box-shadow: 0 6px 16px rgba(34, 197, 94, 0.4);
        }
        
        .btn-danger {
            background: linear-gradient(135deg, #dc2626 0%, #b91c1c 100%);
            color: white;
            border: none;
            box-shadow: 0 4px 12px rgba(239, 68, 68, 0.3);
        }
        
        .btn-danger:hover {
            background: linear-gradient(135deg, #b91c1c 0%, #991b1b 100%);
            transform: translateY(-1px);
            box-shadow: 0 6px 16px rgba(239, 68, 68, 0.4);
        }
        
        /* Responsive Design */
        @media (max-width: 768px) {
            .hr-action-info {
                flex-direction: column;
                gap: 1rem;
            }
            
            .hr-action-meta {
                flex-direction: column;
                gap: 0.5rem;
            }
            
            .summary-grid {
                grid-template-columns: 1fr;
            }
            
            .hr-action-actions {
                flex-direction: column;
                align-items: stretch;
            }
            
            .modal-overlay {
                padding: 1rem;
            }
            
            .modal-header {
                padding: 1.5rem;
            }
            
            .modal-body {
                padding: 1.5rem;
            }
            
            .detail-grid {
                grid-template-columns: 1fr;
            }
            
            .approval-actions {
                flex-direction: column;
            }
            
            .btn-sm {
                min-width: auto;
                width: 100%;
            }
        }
    </style>

</asp:Content>