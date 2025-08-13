<%@ Page Title="Leave Management" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="LeaveManagement.aspx.cs" Inherits="TPASystem2.LeaveManagement.LeaveManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    <style>

        /* ===============================================
   COMPLETE LEAVE MANAGEMENT STYLES
   Add these styles to tpa-common.css
   =============================================== */

/* Dashboard Overview Section */
.dashboard-overview {
    background: white;
    border-radius: 16px;
    padding: 2rem;
    margin-bottom: 2rem;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
    border: 1px solid #e5e7eb;
}

.overview-cards {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
    gap: 1.5rem;
    margin-top: 1.5rem;
}

.overview-card {
    background: white;
    border-radius: 12px;
    padding: 1.5rem;
    border: 2px solid #e5e7eb;
    transition: all 0.3s ease;
    display: flex;
    align-items: center;
    gap: 1rem;
    position: relative;
    overflow: hidden;
}

.overview-card::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    width: 4px;
    height: 100%;
    background: #e5e7eb;
    transition: all 0.3s ease;
}

.overview-card:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
}

.overview-card.pending::before {
    background: linear-gradient(135deg, #f59e0b, #d97706);
}

.overview-card.approved::before {
    background: linear-gradient(135deg, #10b981, #059669);
}

.overview-card.active::before {
    background: linear-gradient(135deg, #3b82f6, #2563eb);
}

.overview-card.completed::before {
    background: linear-gradient(135deg, #8b5cf6, #7c3aed);
}

.overview-card.rejected::before {
    background: linear-gradient(135deg, #ef4444, #dc2626);
}

.card-icon {
    width: 60px;
    height: 60px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    background: #f8fafc;
    color: #64748b;
    font-size: 1.5rem;
}

.overview-card.pending .card-icon {
    background: #fef3c7;
    color: #d97706;
}

.overview-card.approved .card-icon {
    background: #dcfce7;
    color: #059669;
}

.overview-card.active .card-icon {
    background: #dbeafe;
    color: #2563eb;
}

.overview-card.completed .card-icon {
    background: #ede9fe;
    color: #7c3aed;
}

.overview-card.rejected .card-icon {
    background: #fee2e2;
    color: #dc2626;
}

.card-content {
    flex: 1;
}

.card-number {
    font-size: 2.5rem;
    font-weight: 700;
    color: #1e293b;
    line-height: 1;
    margin-bottom: 0.25rem;
}

.card-label {
    font-size: 1rem;
    font-weight: 600;
    color: #374151;
    margin-bottom: 0.25rem;
}

.card-sublabel {
    font-size: 0.875rem;
    color: #64748b;
}

/* Leave Management Section */
.leave-management-section,
.leave-balances-section {
    background: white;
    border-radius: 16px;
    margin-bottom: 2rem;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
    border: 1px solid #e5e7eb;
    overflow: hidden;
}

.section-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 2rem 2rem 1rem 2rem;
    border-bottom: 1px solid #e5e7eb;
    background: #f8fafc;
}

.section-title {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    margin: 0;
    color: #1e293b;
    font-size: 1.5rem;
    font-weight: 600;
}

.section-title .material-icons {
    color: #3b82f6;
    font-size: 1.75rem;
}

.request-count {
    font-weight: 400;
    color: #64748b;
    font-size: 1rem;
}

.section-actions {
    display: flex;
    gap: 0.75rem;
}

/* Filter Controls */
.filter-controls,
.balance-search {
    padding: 1.5rem 2rem;
    background: #f8fafc;
    border-bottom: 1px solid #e5e7eb;
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
    align-items: end;
}

.filter-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    min-width: 200px;
}

.filter-label {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-size: 0.875rem;
    font-weight: 500;
    color: #374151;
}

.filter-label .material-icons {
    font-size: 1rem;
    color: #64748b;
}

/* Modern Grid Styles */
.requests-grid,
.balance-grid {
    padding: 0;
}

.modern-grid {
    width: 100%;
    border-collapse: collapse;
    font-size: 0.9rem;
}

.modern-grid th {
    background: #f1f5f9;
    color: #374151;
    font-weight: 600;
    padding: 1rem;
    text-align: left;
    border-bottom: 2px solid #e2e8f0;
    font-size: 0.875rem;
    text-transform: uppercase;
    letter-spacing: 0.025em;
}

.modern-grid td {
    padding: 1rem;
    border-bottom: 1px solid #f1f5f9;
    vertical-align: middle;
}

.modern-grid tbody tr {
    transition: all 0.2s ease;
}

.modern-grid tbody tr:hover {
    background: #f8fafc;
}

.modern-grid tbody tr:nth-child(even) {
    background: #fafbfc;
}

.modern-grid tbody tr:nth-child(even):hover {
    background: #f1f5f9;
}

/* Leave Status Container and Badges */
.leave-status-container {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.leave-status-badge {
    display: inline-flex;
    align-items: center;
    padding: 0.375rem 0.75rem;
    border-radius: 9999px;
    font-size: 0.75rem;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.025em;
}

.status-pending {
    background: #fef3c7;
    color: #92400e;
    border: 1px solid #fbbf24;
}

.status-approved {
    background: #dcfce7;
    color: #166534;
    border: 1px solid #22c55e;
}

.status-approved-upcoming {
    background: #dbeafe;
    color: #1e40af;
    border: 1px solid #3b82f6;
}

.status-active {
    background: #f0f9ff;
    color: #0369a1;
    border: 1px solid #0284c7;
    animation: pulse 2s infinite;
}

.status-completed {
    background: #ede9fe;
    color: #6b21a8;
    border: 1px solid #8b5cf6;
}

.status-rejected {
    background: #fee2e2;
    color: #991b1b;
    border: 1px solid #ef4444;
}

@keyframes pulse {
    0%, 100% {
        opacity: 1;
    }
    50% {
        opacity: 0.7;
    }
}

.leave-progress {
    font-size: 0.75rem;
    color: #64748b;
    font-style: italic;
}

.leave-progress-detail {
    margin-top: 0.5rem;
    font-size: 0.875rem;
    color: #64748b;
}

/* Action Buttons */
.leave-action-buttons {
    display: flex;
    gap: 0.5rem;
    align-items: center;
    flex-wrap: wrap;
}

.btn-sm {
    padding: 0.5rem;
    border-radius: 6px;
    border: none;
    cursor: pointer;
    transition: all 0.2s ease;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    text-decoration: none;
    min-width: 36px;
    height: 36px;
}

.btn-sm .material-icons {
    font-size: 1rem;
}

.btn-sm.btn-info {
    background: #3b82f6;
    color: white;
}

.btn-sm.btn-info:hover {
    background: #2563eb;
    transform: translateY(-1px);
}

.btn-sm.btn-success {
    background: #10b981;
    color: white;
}

.btn-sm.btn-success:hover {
    background: #059669;
    transform: translateY(-1px);
}

.btn-sm.btn-danger {
    background: #ef4444;
    color: white;
}

.btn-sm.btn-danger:hover {
    background: #dc2626;
    transform: translateY(-1px);
}

.btn-sm.btn-secondary {
    background: #6b7280;
    color: white;
}

.btn-sm.btn-secondary:hover {
    background: #4b5563;
    transform: translateY(-1px);
}

.btn-sm.btn-primary {
    background: #3b82f6;
    color: white;
}

.btn-sm.btn-primary:hover {
    background: #2563eb;
    transform: translateY(-1px);
}

.btn-sm.btn-warning {
    background: #f59e0b;
    color: white;
}

.btn-sm.btn-warning:hover {
    background: #d97706;
    transform: translateY(-1px);
}

/* Modal Styles */
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
    padding: 1rem;
}

.modal-content {
    background: white;
    border-radius: 16px;
    width: 100%;
    max-width: 600px;
    max-height: 90vh;
    overflow-y: auto;
    box-shadow: 0 20px 50px rgba(0, 0, 0, 0.3);
    animation: modalSlideIn 0.3s ease;
}

.large-modal {
    max-width: 800px;
}

@keyframes modalSlideIn {
    from {
        opacity: 0;
        transform: translateY(-20px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1.5rem 2rem;
    border-bottom: 1px solid #e5e7eb;
    background: #f8fafc;
    border-radius: 16px 16px 0 0;
}

.modal-title {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    margin: 0;
    color: #1e293b;
    font-size: 1.25rem;
    font-weight: 600;
}

.modal-title .material-icons {
    color: #3b82f6;
    font-size: 1.5rem;
}

.modal-close {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 32px;
    height: 32px;
    border-radius: 50%;
    background: #f1f5f9;
    color: #64748b;
    text-decoration: none;
    transition: all 0.2s ease;
}

.modal-close:hover {
    background: #e2e8f0;
    color: #374151;
    transform: scale(1.1);
}

.modal-body {
    padding: 2rem;
}

/* Leave Detail Grid */
.leave-detail-grid {
    display: grid;
    gap: 1rem;
    margin-bottom: 2rem;
}

.leave-detail-row {
    display: grid;
    grid-template-columns: 140px 1fr;
    gap: 1rem;
    align-items: start;
    padding: 0.75rem 0;
    border-bottom: 1px solid #f1f5f9;
}

.leave-detail-row:last-child {
    border-bottom: none;
}

.leave-detail-label {
    font-weight: 600;
    color: #374151;
    font-size: 0.875rem;
}

.leave-detail-value {
    color: #1e293b;
    font-size: 0.9rem;
}

/* Leave Timeline */
.leave-timeline {
    background: #f8fafc;
    border-radius: 8px;
    padding: 1rem;
    border-left: 4px solid #3b82f6;
}

.timeline-item {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    color: #374151;
    font-size: 0.9rem;
}

.timeline-item .material-icons {
    color: #3b82f6;
    font-size: 1.25rem;
}

.timeline-item small {
    color: #64748b;
    font-size: 0.8rem;
}

/* Approval Actions */
.approval-actions {
    background: #f8fafc;
    border-radius: 12px;
    padding: 1.5rem;
    border: 1px solid #e5e7eb;
}

.approval-actions h5 {
    margin: 0 0 1rem 0;
    color: #1e293b;
    font-size: 1.1rem;
    font-weight: 600;
}

.form-group {
    margin-bottom: 1.5rem;
}

.form-group label {
    display: block;
    margin-bottom: 0.5rem;
    font-weight: 500;
    color: #374151;
    font-size: 0.875rem;
}

.form-control {
    width: 100%;
    padding: 0.75rem;
    border: 1px solid #d1d5db;
    border-radius: 8px;
    font-size: 0.9rem;
    transition: all 0.2s ease;
    background: white;
}

.form-control:focus {
    outline: none;
    border-color: #3b82f6;
    box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.action-buttons {
    display: flex;
    gap: 0.75rem;
    flex-wrap: wrap;
}

/* Alert Panel */
.alert-panel {
    padding: 1rem 1.5rem;
    border-radius: 12px;
    margin-bottom: 1.5rem;
    border: 1px solid;
    display: flex;
    align-items: center;
    gap: 0.75rem;
    font-weight: 500;
}

.alert-panel.success {
    background: #dcfce7;
    color: #166534;
    border-color: #22c55e;
}

.alert-panel.error {
    background: #fee2e2;
    color: #991b1b;
    border-color: #ef4444;
}

.alert-panel.warning {
    background: #fef3c7;
    color: #92400e;
    border-color: #fbbf24;
}

.alert-panel.info {
    background: #dbeafe;
    color: #1e40af;
    border-color: #3b82f6;
}

/* Button Styles */
.btn {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    gap: 0.5rem;
    padding: 0.75rem 1.5rem;
    border: none;
    border-radius: 8px;
    font-size: 0.9rem;
    font-weight: 500;
    text-decoration: none;
    cursor: pointer;
    transition: all 0.2s ease;
    line-height: 1;
}

.btn-primary {
    background: #3b82f6;
    color: white;
}

.btn-primary:hover {
    background: #2563eb;
    transform: translateY(-1px);
}

.btn-secondary {
    background: #6b7280;
    color: white;
}

.btn-secondary:hover {
    background: #4b5563;
    transform: translateY(-1px);
}

.btn-success {
    background: #10b981;
    color: white;
}

.btn-success:hover {
    background: #059669;
    transform: translateY(-1px);
}

.btn-danger {
    background: #ef4444;
    color: white;
}

.btn-danger:hover {
    background: #dc2626;
    transform: translateY(-1px);
}

.btn-warning {
    background: #f59e0b;
    color: white;
}

.btn-warning:hover {
    background: #d97706;
    transform: translateY(-1px);
}

.btn-outline-primary {
    background: transparent;
    color: #3b82f6;
    border: 1px solid #3b82f6;
}

.btn-outline-primary:hover {
    background: #3b82f6;
    color: white;
}

.btn-outline-light {
    background: transparent;
    color: white;
    border: 1px solid rgba(255, 255, 255, 0.3);
}

.btn-outline-light:hover {
    background: rgba(255, 255, 255, 0.1);
    border-color: white;
}

/* Responsive Design */
@media (max-width: 768px) {
    .overview-cards {
        grid-template-columns: 1fr;
    }
    
    .section-header {
        flex-direction: column;
        align-items: stretch;
        gap: 1rem;
    }
    
    .section-actions {
        justify-content: center;
    }
    
    .filter-controls {
        flex-direction: column;
        align-items: stretch;
    }
    
    .filter-group {
        min-width: auto;
    }
    
    .leave-detail-row {
        grid-template-columns: 1fr;
        gap: 0.25rem;
    }
    
    .action-buttons {
        flex-direction: column;
    }
    
    .modern-grid {
        font-size: 0.8rem;
    }
    
    .modern-grid th,
    .modern-grid td {
        padding: 0.75rem 0.5rem;
    }
    
    .leave-action-buttons {
        flex-direction: column;
        align-items: stretch;
    }
    
    .btn-sm {
        min-width: auto;
        height: auto;
        padding: 0.5rem 0.75rem;
        justify-content: flex-start;
        gap: 0.5rem;
    }
    
    .btn-sm .material-icons {
        font-size: 1.125rem;
    }
}

@media (max-width: 480px) {
    .card-number {
        font-size: 2rem;
    }
    
    .modal-content {
        margin: 0.5rem;
        max-height: 95vh;
    }
    
    .modal-header,
    .modal-body {
        padding: 1rem;
    }
    
    .overview-card {
        padding: 1rem;
    }
    
    .card-icon {
        width: 50px;
        height: 50px;
        font-size: 1.25rem;
    }
}
    </style>
    <!-- Welcome Header - Matching EmployeeLeavePortal Style -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">event_available</i>
                    Leave Management
                </h1>
                <p class="welcome-subtitle">Manage all employee leave requests, approvals, and track leave status</p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">person</i>
                        <span>
                            <asp:Literal ID="litManagerName" runat="server" Text="Manager"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">business</i>
                        <span>
                            <asp:Literal ID="litUserRole" runat="server" Text="Leave Manager"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">security</i>
                        <span>Full Leave Management Access</span>
                    </div>
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnViewLeaveCalendar" runat="server" Text="Leave Calendar" 
                    CssClass="btn btn-outline-light" OnClick="btnViewLeaveCalendar_Click" />
                <asp:Button ID="btnCreateLeaveRequest" runat="server" Text="Create Request" 
                    CssClass="btn btn-outline-light" OnClick="btnCreateLeaveRequest_Click" />
            </div>
        </div>
    </div>

    <!-- Alert Messages -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert-panel">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Dashboard Stats Section -->
    <div class="dashboard-overview">
        <h3 class="section-title">
            <i class="material-icons">dashboard</i>
            Leave Overview
        </h3>
        
        <div class="overview-cards">
            <div class="overview-card pending">
                <div class="card-icon">
                    <i class="material-icons">pending</i>
                </div>
                <div class="card-content">
                    <div class="card-number">
                        <asp:Literal ID="litPendingRequests" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="card-label">Pending Approval</div>
                    <div class="card-sublabel">Requires Action</div>
                </div>
            </div>

            <div class="overview-card approved">
                <div class="card-icon">
                    <i class="material-icons">check_circle</i>
                </div>
                <div class="card-content">
                    <div class="card-number">
                        <asp:Literal ID="litApprovedRequests" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="card-label">Approved</div>
                    <div class="card-sublabel">Active Leaves</div>
                </div>
            </div>

            <div class="overview-card active">
                <div class="card-icon">
                    <i class="material-icons">flight_takeoff</i>
                </div>
                <div class="card-content">
                    <div class="card-number">
                        <asp:Literal ID="litActiveLeaves" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="card-label">Currently On Leave</div>
                    <div class="card-sublabel">Today</div>
                </div>
            </div>

            <div class="overview-card completed">
                <div class="card-icon">
                    <i class="material-icons">flight_land</i>
                </div>
                <div class="card-content">
                    <div class="card-number">
                        <asp:Literal ID="litCompletedLeaves" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="card-label">Recently Completed</div>
                    <div class="card-sublabel">This Week</div>
                </div>
            </div>

            <div class="overview-card rejected">
                <div class="card-icon">
                    <i class="material-icons">cancel</i>
                </div>
                <div class="card-content">
                    <div class="card-number">
                        <asp:Literal ID="litRejectedRequests" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="card-label">Rejected</div>
                    <div class="card-sublabel">This Month</div>
                </div>
            </div>
        </div>
    </div>

    <!-- Leave Request Management Section -->
    <div class="leave-management-section">
        <div class="section-header">
            <h3 class="section-title">
                <i class="material-icons">approval</i>
                All Leave Requests
                <span class="request-count">(<asp:Literal ID="litDisplayCount" runat="server" Text="0"></asp:Literal> requests)</span>
            </h3>
            
            <div class="section-actions">
                <asp:Button ID="btnRefreshRequests" runat="server" Text="Refresh" 
                    CssClass="btn btn-secondary" OnClick="btnRefreshRequests_Click" />
                <asp:Button ID="btnExportToExcel" runat="server" Text="Export to Excel" 
                    CssClass="btn btn-outline-primary" OnClick="btnExportToExcel_Click" />
            </div>
        </div>

        <!-- Filter Controls -->
        <div class="filter-controls">
            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">filter_list</i>
                    Status Filter:
                </label>
                <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                    <asp:ListItem Text="All Statuses" Value=""></asp:ListItem>
                    <asp:ListItem Text="Pending" Value="Pending"></asp:ListItem>
                    <asp:ListItem Text="Approved" Value="Approved"></asp:ListItem>
                    <asp:ListItem Text="Rejected" Value="Rejected"></asp:ListItem>
                    <asp:ListItem Text="Currently On Leave" Value="Active"></asp:ListItem>
                    <asp:ListItem Text="Completed" Value="Completed"></asp:ListItem>
                </asp:DropDownList>
            </div>
            
            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">business</i>
                    Department:
                </label>
                <asp:DropDownList ID="ddlDepartmentFilter" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlDepartmentFilter_SelectedIndexChanged">
                </asp:DropDownList>
            </div>

            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">event</i>
                    Leave Type:
                </label>
                <asp:DropDownList ID="ddlLeaveTypeFilter" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlLeaveTypeFilter_SelectedIndexChanged">
                </asp:DropDownList>
            </div>

            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">search</i>
                    Employee Search:
                </label>
                <asp:TextBox ID="txtEmployeeSearch" runat="server" CssClass="form-control" placeholder="Search by name or employee number"></asp:TextBox>
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
            </div>
        </div>

        <!-- Leave Requests Grid -->
        <div class="requests-grid">
            <asp:GridView ID="gvLeaveRequests" runat="server" 
                CssClass="modern-grid" 
                AutoGenerateColumns="false" 
                OnRowCommand="gvLeaveRequests_RowCommand"
                OnRowDataBound="gvLeaveRequests_RowDataBound"
                EmptyDataText="No leave requests found matching your criteria."
                DataKeyNames="Id">
                
                <Columns>
                    <asp:BoundField DataField="EmployeeName" HeaderText="Employee" />
                    <asp:BoundField DataField="EmployeeNumber" HeaderText="Emp #" />
                    <asp:BoundField DataField="Department" HeaderText="Department" />
                    <asp:BoundField DataField="LeaveType" HeaderText="Leave Type" />
                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="EndDate" HeaderText="End Date" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="DaysRequested" HeaderText="Days" />
                    <asp:BoundField DataField="RequestedAt" HeaderText="Requested" DataFormatString="{0:MMM dd, yyyy}" />
                    
                    <asp:TemplateField HeaderText="Status & Progress">
                        <ItemTemplate>
                            <div class="leave-status-container">
                                <span class="leave-status-badge status-<%# GetLeaveStatus(Eval("Status"), Eval("StartDate"), Eval("EndDate")).ToLower() %>">
                                    <%# GetLeaveStatusText(Eval("Status"), Eval("StartDate"), Eval("EndDate")) %>
                                </span>
                                <div class="leave-progress">
                                    <%# GetLeaveProgressText(Eval("Status"), Eval("StartDate"), Eval("EndDate")) %>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Actions" ItemStyle-Width="220px">
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

                                <asp:LinkButton ID="btnEdit" runat="server" 
                                              CommandName="EditRequest" 
                                              CommandArgument='<%# Eval("Id") %>'
                                              CssClass="btn btn-sm btn-secondary"
                                              ToolTip="Edit Request"
                                              Visible='<%# Eval("Status").ToString() == "Pending" %>'>
                                    <i class="material-icons">edit</i>
                                </asp:LinkButton>

                                <asp:LinkButton ID="btnMarkReturned" runat="server" 
                                              CommandName="MarkReturned" 
                                              CommandArgument='<%# Eval("Id") %>'
                                              CssClass="btn btn-sm btn-warning"
                                              ToolTip="Mark as Returned"
                                              OnClientClick="return confirm('Mark this employee as returned from leave?');"
                                              Visible='<%# IsCurrentlyOnLeave(Eval("Status"), Eval("StartDate"), Eval("EndDate")) %>'>
                                    <i class="material-icons">assignment_return</i>
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Employee Leave Balances Section -->
    <div class="leave-balances-section">
        <div class="section-header">
            <h3 class="section-title">
                <i class="material-icons">account_balance_wallet</i>
                Employee Leave Balances
            </h3>
            
            <div class="section-actions">
                <asp:Button ID="btnManageBalances" runat="server" Text="Manage Balances" 
                    CssClass="btn btn-primary" OnClick="btnManageBalances_Click" />
            </div>
        </div>

        <!-- Balance Search -->
        <div class="balance-search">
            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">search</i>
                    Search Employee:
                </label>
                <asp:TextBox ID="txtBalanceSearch" runat="server" CssClass="form-control" placeholder="Search employee for balance info"></asp:TextBox>
                <asp:Button ID="btnSearchBalance" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearchBalance_Click" />
            </div>
        </div>

        <!-- Balance Grid -->
        <div class="balance-grid">
            <asp:GridView ID="gvLeaveBalances" runat="server" 
                CssClass="modern-grid" 
                AutoGenerateColumns="false" 
                OnRowCommand="gvLeaveBalances_RowCommand"
                EmptyDataText="No employee balances found."
                DataKeyNames="EmployeeId">
                
                <Columns>
                    <asp:BoundField DataField="EmployeeName" HeaderText="Employee" />
                    <asp:BoundField DataField="Department" HeaderText="Department" />
                    <asp:BoundField DataField="LeaveType" HeaderText="Leave Type" />
                    <asp:BoundField DataField="AllocatedDays" HeaderText="Allocated" DataFormatString="{0:N1}" />
                    <asp:BoundField DataField="UsedDays" HeaderText="Used" DataFormatString="{0:N1}" />
                    <asp:BoundField DataField="RemainingDays" HeaderText="Remaining" DataFormatString="{0:N1}" />
                    
                    <asp:TemplateField HeaderText="Actions" ItemStyle-Width="120px">
                        <ItemTemplate>
                            <div class="leave-action-buttons">
                                <asp:LinkButton ID="btnEditBalance" runat="server" 
                                              CommandName="EditBalance" 
                                              CommandArgument='<%# Eval("EmployeeId") + "," + Eval("LeaveType") %>'
                                              CssClass="btn btn-sm btn-primary"
                                              ToolTip="Edit Balance">
                                    <i class="material-icons">edit</i>
                                </asp:LinkButton>
                                
                                <asp:LinkButton ID="btnViewHistory" runat="server" 
                                              CommandName="ViewHistory" 
                                              CommandArgument='<%# Eval("EmployeeId") %>'
                                              CssClass="btn btn-sm btn-info"
                                              ToolTip="View History">
                                    <i class="material-icons">history</i>
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Request Details Modal Panel -->
    <asp:Panel ID="pnlRequestDetails" runat="server" CssClass="modal-overlay" Visible="false">
        <div class="modal-content large-modal">
            <div class="modal-header">
                <h4 class="modal-title">
                    <i class="material-icons">event_note</i>
                    Leave Request Details
                </h4>
                <asp:LinkButton ID="btnCloseModal" runat="server" CssClass="modal-close" OnClick="btnCloseModal_Click">
                    <i class="material-icons">close</i>
                </asp:LinkButton>
            </div>
            
            <div class="modal-body">
                <div class="leave-detail-grid">
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
                        <span class="leave-detail-label">Status & Progress:</span>
                        <span class="leave-detail-value">
                            <span class="leave-status-badge" id="spanStatus" runat="server">
                                <asp:Literal ID="litStatus" runat="server"></asp:Literal>
                            </span>
                            <div class="leave-progress-detail">
                                <asp:Literal ID="litProgress" runat="server"></asp:Literal>
                            </div>
                        </span>
                    </div>
                    <div class="leave-detail-row">
                        <span class="leave-detail-label">Requested On:</span>
                        <span class="leave-detail-value"><asp:Literal ID="litRequestedAt" runat="server"></asp:Literal></span>
                    </div>
                    <div class="leave-detail-row">
                        <span class="leave-detail-label">Reason:</span>
                        <span class="leave-detail-value"><asp:Literal ID="litReason" runat="server"></asp:Literal></span>
                    </div>
                    
                    <asp:Panel ID="pnlReviewDetails" runat="server" Visible="false">
                        <div class="leave-detail-row">
                            <span class="leave-detail-label">Reviewed By:</span>
                            <span class="leave-detail-value"><asp:Literal ID="litReviewedBy" runat="server"></asp:Literal></span>
                        </div>
                        <div class="leave-detail-row">
                            <span class="leave-detail-label">Reviewed On:</span>
                            <span class="leave-detail-value"><asp:Literal ID="litReviewedAt" runat="server"></asp:Literal></span>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnlLeaveTimeline" runat="server" Visible="false">
                        <div class="leave-detail-row">
                            <span class="leave-detail-label">Leave Timeline:</span>
                            <span class="leave-detail-value">
                                <div class="leave-timeline">
                                    <asp:Literal ID="litLeaveTimeline" runat="server"></asp:Literal>
                                </div>
                            </span>
                        </div>
                    </asp:Panel>
                </div>

                <!-- Approval Actions -->
                <asp:Panel ID="pnlApprovalActions" runat="server" Visible="false" CssClass="approval-actions">
                    <h5>Take Action on This Request</h5>
                    
                    <div class="form-group">
                        <label for="txtApprovalComments">Comments (Optional):</label>
                        <asp:TextBox ID="txtApprovalComments" runat="server" TextMode="MultiLine" 
                            CssClass="form-control" Rows="3" placeholder="Add any comments for the employee..."></asp:TextBox>
                    </div>
                    
                    <div class="action-buttons">
                        <asp:Button ID="btnApproveRequest" runat="server" Text="Approve Request" 
                            CssClass="btn btn-success" OnClick="btnApproveRequest_Click" 
                            OnClientClick="return confirm('Are you sure you want to approve this leave request?');" />
                        
                        <asp:Button ID="btnRejectRequest" runat="server" Text="Reject Request" 
                            CssClass="btn btn-danger" OnClick="btnRejectRequest_Click" 
                            OnClientClick="return confirm('Are you sure you want to reject this leave request?');" />
                        
                        <asp:Button ID="btnCancelAction" runat="server" Text="Cancel" 
                            CssClass="btn btn-secondary" OnClick="btnCloseModal_Click" />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>

    <!-- Hidden field to store selected request ID -->
    <asp:HiddenField ID="hfSelectedRequestId" runat="server" />

</asp:Content>