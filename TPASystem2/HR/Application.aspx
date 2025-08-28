<%@ Page Title="Job Applications Management" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Application.aspx.cs" Inherits="TPASystem2.HR.Application" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

    <style> 

        /* ===============================================
   JOB APPLICATIONS MANAGEMENT STYLES
   Add these styles to tpa-common.css
   =============================================== */

/* Contact Information Display */
.contact-info {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.contact-email,
.contact-phone {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-size: 0.85rem;
    color: #64748b;
}

.contact-email .material-icons,
.contact-phone .material-icons {
    font-size: 1rem;
    color: #94a3b8;
}

/* Position Information Display */
.position-info {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
}

.primary-position {
    font-weight: 600;
    color: #1e293b;
    font-size: 0.9rem;
}

.secondary-position {
    font-size: 0.8rem;
    color: #64748b;
    font-style: italic;
}

/* Date Information Display */
.date-info {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
}

.application-date {
    font-weight: 500;
    color: #374151;
    font-size: 0.9rem;
}

.time-since {
    font-size: 0.8rem;
    color: #64748b;
    font-style: italic;
}

/* Status Badge Enhancements for Applications */
.status-badge.draft {
    background: #f3f4f6;
    color: #6b7280;
}

.status-badge.submitted {
    background: #dbeafe;
    color: #1d4ed8;
}

.status-badge.under-review {
    background: #fef3c7;
    color: #d97706;
}

.status-badge.approved {
    background: #dcfce7;
    color: #166534;
}

.status-badge.rejected {
    background: #fee2e2;
    color: #991b1b;
}

/* Application Detail Modal Specific Styles */
.application-detail-tabs {
    width: 100%;
}

.application-detail-tabs .tab-navigation {
    display: flex;
    border-bottom: 2px solid #e5e7eb;
    margin-bottom: 2rem;
    gap: 0;
}

.application-detail-tabs .tab-button {
    flex: 1;
    padding: 1rem 1.5rem;
    border: none;
    background: transparent;
    color: #64748b;
    font-weight: 500;
    cursor: pointer;
    transition: all 0.3s ease;
    border-bottom: 3px solid transparent;
    font-size: 0.9rem;
}

.application-detail-tabs .tab-button:hover {
    background: #f8fafc;
    color: #374151;
}

.application-detail-tabs .tab-button.active {
    color: var(--tpa-primary);
    border-bottom-color: var(--tpa-primary);
    background: #f8fafc;
}

.application-detail-tabs .tab-content {
    display: none;
    animation: fadeIn 0.3s ease-in-out;
}

.application-detail-tabs .tab-content.active {
    display: block;
}

/* Application Section Styling */
.application-section {
    background: white;
    border-radius: 12px;
    padding: 2rem;
    border: 1px solid #e5e7eb;
}

.application-section .section-title {
    font-size: 1.25rem;
    font-weight: 600;
    color: #1e293b;
    margin-bottom: 1.5rem;
    padding-bottom: 0.75rem;
    border-bottom: 2px solid #f1f5f9;
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

/* Profile Grid for Application Details */
.profile-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 1.5rem;
}

.profile-item {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.profile-item label {
    font-weight: 600;
    color: #374151;
    font-size: 0.9rem;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.profile-value {
    color: #1e293b;
    font-size: 0.95rem;
    line-height: 1.5;
    padding: 0.75rem 1rem;
    background: #f8fafc;
    border-radius: 8px;
    border: 1px solid #e2e8f0;
    min-height: 20px;
}

/* Employment History Specific Styles */
.employment-record {
    background: #f8fafc;
    border-radius: 12px;
    padding: 1.5rem;
    margin-bottom: 1.5rem;
    border: 1px solid #e2e8f0;
    transition: all 0.3s ease;
}

.employment-record:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
}

.employment-record:last-child {
    margin-bottom: 0;
}

.employment-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1rem;
    padding-bottom: 0.75rem;
    border-bottom: 1px solid #e2e8f0;
}

.employment-header h5 {
    font-size: 1.1rem;
    font-weight: 600;
    color: #1e293b;
    margin: 0;
}

.employment-dates {
    font-size: 0.85rem;
    color: #64748b;
    font-weight: 500;
    background: white;
    padding: 0.25rem 0.75rem;
    border-radius: 20px;
    border: 1px solid #e2e8f0;
}

.employment-details {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 0.75rem;
}

.employment-details p {
    margin: 0;
    font-size: 0.9rem;
    color: #475569;
    line-height: 1.4;
}

.employment-details strong {
    color: #374151;
    font-weight: 600;
}

/* Status Update Modal Specific Styles */
.current-status-display {
    font-size: 1.1rem;
    font-weight: 600;
    padding: 0.75rem 1rem;
    background: #f1f5f9;
    border-radius: 8px;
    border: 1px solid #cbd5e1;
    color: #334155;
}

/* Grid Container Enhancements */
.grid-container {
    background: white;
    border-radius: 16px;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
    border: 1px solid #e5e7eb;
    overflow: hidden;
}

.grid-header {
    background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
    padding: 2rem;
    border-bottom: 2px solid #e5e7eb;
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.grid-title {
    font-size: 1.5rem;
    font-weight: 700;
    color: #1e293b;
    margin: 0;
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.grid-title .material-icons {
    color: var(--tpa-primary);
    font-size: 1.75rem;
}

.grid-actions {
    display: flex;
    gap: 1rem;
    align-items: center;
}

/* Filter Grid Specific for Applications */
.filter-grid {
    display: grid;
    grid-template-columns: 2fr 1fr 1fr 1fr;
    gap: 1.5rem;
    align-items: end;
    padding: 2rem;
    background: #f8fafc;
    border-bottom: 1px solid #e5e7eb;
}

.filter-actions {
    display: flex;
    gap: 1rem;
    justify-content: flex-end;
    margin-top: 1rem;
}

/* Data Grid Wrapper */
.data-grid-wrapper {
    padding: 0;
    overflow-x: auto;
}

/* Enhanced Data Table for Applications */
.data-table {
    width: 100%;
    border-collapse: collapse;
    background: white;
    border: none;
}

.data-table th {
    background: #f1f5f9;
    color: #374151;
    font-weight: 600;
    text-align: left;
    padding: 1.25rem 1rem;
    border-bottom: 2px solid #e2e8f0;
    font-size: 0.9rem;
    text-transform: uppercase;
    letter-spacing: 0.5px;
    position: sticky;
    top: 0;
    z-index: 10;
}

.data-table td {
    padding: 1.25rem 1rem;
    border-bottom: 1px solid #f1f5f9;
    vertical-align: middle;
    color: #1e293b;
    font-size: 0.9rem;
    line-height: 1.4;
    transition: all 0.3s ease;
}

.data-table tr:hover {
    background: #f8fafc;
}

.data-table tr:hover td {
    background: #f8fafc;
}

/* Actions Column Specific Styling */
.actions-column {
    width: 120px;
    text-align: center;
}

.action-buttons {
    display: flex;
    gap: 0.5rem;
    justify-content: center;
    align-items: center;
}

.btn-action {
    width: 32px;
    height: 32px;
    border-radius: 50%;
    border: none;
    cursor: pointer;
    transition: all 0.3s ease;
    display: flex;
    align-items: center;
    justify-content: center;
    text-decoration: none;
    color: white;
    font-size: 0;
}

.btn-action .material-icons {
    font-size: 16px;
}

.btn-action:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.btn-view {
    background: linear-gradient(135deg, #3b82f6, #2563eb);
}

.btn-view:hover {
    background: linear-gradient(135deg, #2563eb, #1d4ed8);
}

.btn-edit {
    background: linear-gradient(135deg, #f59e0b, #d97706);
}

.btn-edit:hover {
    background: linear-gradient(135deg, #d97706, #b45309);
}

.btn-delete {
    background: linear-gradient(135deg, #ef4444, #dc2626);
}

.btn-delete:hover {
    background: linear-gradient(135deg, #dc2626, #b91c1c);
}

/* Alert Panel Styling */
.alert-panel {
    padding: 1rem 1.5rem;
    border-radius: 8px;
    margin-bottom: 1.5rem;
    border: 1px solid;
    display: flex;
    align-items: center;
    gap: 0.75rem;
    font-weight: 500;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.alert-panel.success {
    background: linear-gradient(135deg, #dcfce7 0%, #bbf7d0 100%);
    color: #166534;
    border-color: #22c55e;
}

.alert-panel.error {
    background: linear-gradient(135deg, #fee2e2 0%, #fecaca 100%);
    color: #991b1b;
    border-color: #ef4444;
}

.alert-panel.warning {
    background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
    color: #92400e;
    border-color: #fbbf24;
}

.alert-panel.info {
    background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%);
    color: #1e40af;
    border-color: #3b82f6;
}

/* GridView Pager Enhancement */
.gridview-pager {
    padding: 1.5rem 2rem;
    text-align: center;
    background: #f8fafc;
    border-top: 1px solid #e5e7eb;
}

.gridview-pager table {
    margin: 0 auto;
}

.gridview-pager td {
    padding: 0.25rem 0.5rem;
}

.gridview-pager a {
    color: var(--tpa-primary);
    text-decoration: none;
    padding: 0.75rem 1rem;
    border-radius: 8px;
    transition: all 0.3s ease;
    font-weight: 500;
    border: 1px solid transparent;
    background: white;
}

.gridview-pager a:hover {
    background: #f0f9ff;
    border-color: var(--tpa-primary);
    transform: translateY(-1px);
    box-shadow: 0 2px 8px rgba(59, 130, 246, 0.2);
}

.gridview-pager span {
    padding: 0.75rem 1rem;
    background: var(--tpa-primary);
    color: white;
    border-radius: 8px;
    font-weight: 600;
    border: 1px solid var(--tpa-primary);
    box-shadow: 0 2px 8px rgba(59, 130, 246, 0.3);
}

/* Modal Enhancements for Applications */
.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    backdrop-filter: blur(4px);
    z-index: 1000;
    display: none;
    align-items: center;
    justify-content: center;
    padding: 2rem;
}

.modal-content {
    background: white;
    border-radius: 16px;
    box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
    max-height: 90vh;
    overflow-y: auto;
    width: 90%;
    max-width: 1000px;
    position: relative;
    animation: modalSlideIn 0.3s ease-out;
}

.modal-content.large-modal {
    width: 95%;
    max-width: 1200px;
}

.modal-content.medium-modal {
    width: 80%;
    max-width: 800px;
}

@keyframes modalSlideIn {
    from {
        opacity: 0;
        transform: translateY(-50px) scale(0.95);
    }
    to {
        opacity: 1;
        transform: translateY(0) scale(1);
    }
}

.modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 2rem 2rem 1rem 2rem;
    border-bottom: 1px solid #e5e7eb;
}

.modal-title {
    font-size: 1.5rem;
    font-weight: 700;
    color: #1e293b;
    margin: 0;
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.modal-title .material-icons {
    color: var(--tpa-primary);
    font-size: 1.75rem;
}

.modal-close {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background: #f3f4f6;
    border: none;
    cursor: pointer;
    transition: all 0.3s ease;
    display: flex;
    align-items: center;
    justify-content: center;
    color: #6b7280;
    text-decoration: none;
}

.modal-close:hover {
    background: #e5e7eb;
    color: #374151;
    transform: rotate(90deg);
}

.modal-body {
    padding: 2rem;
}

.modal-footer {
    padding: 1.5rem 2rem;
    border-top: 1px solid #e5e7eb;
    background: #f8fafc;
}

.modal-actions {
    display: flex;
    gap: 1rem;
    justify-content: flex-end;
}

/* Form Section in Modals */
.form-section {
    background: white;
    border-radius: 12px;
    padding: 0;
}

.form-group {
    margin-bottom: 1.5rem;
}

.form-label {
    display: block;
    font-weight: 600;
    color: #374151;
    margin-bottom: 0.5rem;
    font-size: 0.9rem;
}

.form-control {
    width: 100%;
    padding: 0.75rem 1rem;
    border: 2px solid #e5e7eb;
    border-radius: 8px;
    font-size: 0.9rem;
    transition: all 0.3s ease;
    background: white;
    box-sizing: border-box;
}

.form-control:focus {
    outline: none;
    border-color: var(--tpa-primary);
    box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

/* Button Icon Styling (since ASP.NET buttons can't contain HTML) */
#btnRefresh::before {
    content: "refresh";
    font-family: 'Material Icons';
    margin-right: 0.5rem;
    font-size: 1rem;
}

#btnExport::before {
    content: "file_download";
    font-family: 'Material Icons';
    margin-right: 0.5rem;
    font-size: 1rem;
}

/* Button Enhancements */
.btn-tpa {
    background: linear-gradient(135deg, var(--tpa-primary), #2563eb);
    color: white;
    border: none;
    padding: 0.75rem 1.5rem;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s ease;
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    text-decoration: none;
    font-size: 0.9rem;
}

.btn-tpa:hover {
    background: linear-gradient(135deg, #2563eb, #1d4ed8);
    transform: translateY(-2px);
    box-shadow: 0 4px 16px rgba(59, 130, 246, 0.3);
}

.btn-secondary {
    background: #f8fafc;
    color: #374151;
    border: 2px solid #e5e7eb;
    padding: 0.75rem 1.5rem;
    border-radius: 8px;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s ease;
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    text-decoration: none;
    font-size: 0.9rem;
}

.btn-secondary:hover {
    background: #f1f5f9;
    border-color: #cbd5e1;
    transform: translateY(-1px);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

/* Responsive Design for Applications */
@media (max-width: 1024px) {
    .filter-grid {
        grid-template-columns: 1fr 1fr;
        gap: 1rem;
    }
    
    .profile-grid {
        grid-template-columns: 1fr;
    }
    
    .employment-details {
        grid-template-columns: 1fr;
    }
    
    .modal-content.large-modal {
        width: 98%;
    }
}

@media (max-width: 768px) {
    .filter-grid {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
    
    .grid-header {
        flex-direction: column;
        align-items: stretch;
        gap: 1rem;
    }
    
    .grid-actions {
        justify-content: center;
    }
    
    .application-detail-tabs .tab-navigation {
        flex-direction: column;
    }
    
    .application-detail-tabs .tab-button {
        border-bottom: 1px solid #e5e7eb;
        border-radius: 0;
    }
    
    .application-detail-tabs .tab-button.active {
        border-bottom-color: var(--tpa-primary);
        border-left: 3px solid var(--tpa-primary);
        border-bottom: 1px solid #e5e7eb;
    }
    
    .contact-info {
        align-items: flex-start;
    }
    
    .employment-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.5rem;
    }
    
    .action-buttons {
        flex-direction: column;
        gap: 0.25rem;
    }
    
    .btn-action {
        width: 28px;
        height: 28px;
    }
    
    .btn-action .material-icons {
        font-size: 14px;
    }
    
    .modal-content {
        margin: 1rem;
        width: calc(100% - 2rem);
    }
    
    .modal-header {
        padding: 1.5rem 1.5rem 1rem 1.5rem;
    }
    
    .modal-body {
        padding: 1.5rem;
    }
    
    .modal-footer {
        padding: 1rem 1.5rem;
    }
    
    .modal-actions {
        flex-direction: column;
        gap: 0.75rem;
    }
    
    .modal-actions .btn-tpa,
    .modal-actions .btn-secondary {
        width: 100%;
        justify-content: center;
    }
}

@media (max-width: 480px) {
    .employee-info-cell {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.5rem;
    }
    
    .employee-avatar {
        width: 32px;
        height: 32px;
        font-size: 0.8rem;
    }
    
    .grid-container {
        border-radius: 8px;
    }
    
    .grid-header {
        padding: 1.5rem;
    }
    
    .filter-grid {
        padding: 1.5rem;
    }
    
    .application-section {
        padding: 1.5rem;
    }
    
    .employment-record {
        padding: 1rem;
    }
    
    .data-table th,
    .data-table td {
        padding: 1rem 0.75rem;
        font-size: 0.85rem;
    }
    
    .gridview-pager {
        padding: 1rem;
    }
    
    .modal-content {
        border-radius: 8px;
    }
}

/* Print Styles for Applications */
@media print {
    .grid-header,
    .filter-grid,
    .action-buttons,
    .modal-overlay,
    .btn-tpa,
    .btn-secondary {
        display: none !important;
    }
    
    .data-table {
        border: 1px solid #000 !important;
    }
    
    .data-table th,
    .data-table td {
        border: 1px solid #000 !important;
        padding: 0.5rem !important;
    }
    
    .status-badge {
        border: 1px solid #000 !important;
        background: #fff !important;
        color: #000 !important;
    }
    
    .employee-avatar {
        background: #f0f0f0 !important;
        color: #000 !important;
    }
}
    </style>

    <!-- Welcome Header - Matching EmployeeLeavePortal Style -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">assignment</i>
                    Job Applications Management
                </h1>
                <p class="welcome-subtitle">Review and manage employment applications</p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">person</i>
                        <span>
                            <asp:Literal ID="litCurrentUser" runat="server" Text="HR Manager"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">business</i>
                        <span>
                            <asp:Literal ID="litDepartment" runat="server" Text="Human Resources"></asp:Literal>
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
                <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CssClass="btn-secondary" OnClick="btnRefresh_Click" />
            </div>
        </div>
    </div>

    <!-- Error/Success Messages -->
    <asp:Panel ID="pnlMessages" runat="server" Visible="false" CssClass="alert-panel">
        <asp:Label ID="lblMessage" runat="server" CssClass="message-text"></asp:Label>
    </asp:Panel>

    <!-- Application Grid Section -->
    <div class="grid-container">
        <div class="grid-header">
            <h2 class="grid-title">
                <i class="material-icons">list_alt</i>
                Employment Applications
            </h2>
            <div class="grid-actions">
                <asp:Button ID="btnExport" runat="server" Text="Export" CssClass="btn-secondary" OnClick="btnExport_Click" />
            </div>
        </div>

        <!-- Filter Section -->
        <div class="filter-grid">
            <div class="form-group">
                <label class="form-label">Search Applications</label>
                <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by name, phone, or application number..."></asp:TextBox>
            </div>
            <div class="form-group">
                <label class="form-label">Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                    <asp:ListItem Text="All Status" Value=""></asp:ListItem>
                    <asp:ListItem Text="Draft" Value="Draft"></asp:ListItem>
                    <asp:ListItem Text="Submitted" Value="Submitted"></asp:ListItem>
                    <asp:ListItem Text="Under Review" Value="Under Review"></asp:ListItem>
                    <asp:ListItem Text="Approved" Value="Approved"></asp:ListItem>
                    <asp:ListItem Text="Rejected" Value="Rejected"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label class="form-label">Position</label>
                <asp:DropDownList ID="ddlPosition" runat="server" CssClass="form-control">
                    <asp:ListItem Text="All Positions" Value=""></asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-actions">
                <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn-tpa" OnClick="btnFilter_Click" />
                <asp:Button ID="btnClearFilter" runat="server" Text="Clear" CssClass="btn-secondary" OnClick="btnClearFilter_Click" />
            </div>
        </div>

        <!-- Data Grid -->
        <div class="data-grid-wrapper">
            <asp:GridView ID="gvApplications" runat="server" 
                AutoGenerateColumns="false" 
                CssClass="data-table" 
                AllowPaging="true" 
                PageSize="20" 
                OnPageIndexChanging="gvApplications_PageIndexChanging"
                OnRowCommand="gvApplications_RowCommand"
                OnRowDataBound="gvApplications_RowDataBound"
                EmptyDataText="No applications found.">
                
                <Columns>
                    <asp:TemplateField HeaderText="Applicant">
                        <ItemTemplate>
                            <div class="employee-info-cell">
                                <div class="employee-avatar">
                                    <%# GetInitials(Eval("FirstName").ToString(), Eval("LastName").ToString()) %>
                                </div>
                                <div class="employee-details">
                                    <div class="employee-name">
                                        <%# Eval("FirstName") %> <%# Eval("LastName") %>
                                    </div>
                                    <div class="employee-number">
                                        App #<%# Eval("ApplicationNumber") %>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Contact">
                        <ItemTemplate>
                            <div class="contact-info">
                                <div class="contact-email">
                                    <i class="material-icons">phone</i>
                                    <%# string.IsNullOrEmpty(Eval("HomePhone")?.ToString()) ? "N/A" : Eval("HomePhone") %>
                                </div>
                                <div class="contact-phone">
                                    <i class="material-icons">phone_android</i>
                                    <%# string.IsNullOrEmpty(Eval("CellPhone")?.ToString()) ? "N/A" : Eval("CellPhone") %>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Position">
                        <ItemTemplate>
                            <div class="position-info">
                                <div class="primary-position">
                                    <%# string.IsNullOrEmpty(Eval("Position1")?.ToString()) ? "N/A" : Eval("Position1") %>
                                </div>
                                <%# !string.IsNullOrEmpty(Eval("Position2")?.ToString()) ? 
                                    "<div class=\"secondary-position\">Alt: " + Eval("Position2") + "</div>" : "" %>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Application Date">
                        <ItemTemplate>
                            <div class="date-info">
                                <div class="application-date">
                                    <%# Convert.ToDateTime(Eval("CreatedDate")).ToString("MMM dd, yyyy") %>
                                </div>
                                <div class="time-since">
                                    <%# GetTimeSince(Convert.ToDateTime(Eval("CreatedDate"))) %>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class="status-badge <%# GetStatusClass(Eval("Status").ToString()) %>">
                                <i class="material-icons"><%# GetStatusIcon(Eval("Status").ToString()) %></i>
                                <%# GetStatusDisplayText(Eval("Status").ToString()) %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Actions" HeaderStyle-Width="120px" ItemStyle-CssClass="actions-column">
                        <ItemTemplate>
                            <div class="action-buttons">
                                <asp:LinkButton ID="btnView" runat="server" 
                                    CommandName="ViewApplication" 
                                    CommandArgument='<%# Eval("ApplicationId") %>' 
                                    CssClass="btn-action btn-view" 
                                    ToolTip="View Application">
                                    <i class="material-icons">visibility</i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnEdit" runat="server" 
                                    CommandName="EditApplication" 
                                    CommandArgument='<%# Eval("ApplicationId") %>' 
                                    CssClass="btn-action btn-edit" 
                                    ToolTip="Edit Status">
                                    <i class="material-icons">edit</i>
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnDelete" runat="server" 
                                    CommandName="DeleteApplication" 
                                    CommandArgument='<%# Eval("ApplicationId") %>' 
                                    CssClass="btn-action btn-delete" 
                                    ToolTip="Delete Application"
                                    OnClientClick='return confirmDelete();'>
                                    <i class="material-icons">delete</i>
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>

                <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" 
                    PageButtonCount="5" Position="Bottom" />
                <PagerStyle CssClass="gridview-pager" />
            </asp:GridView>
        </div>
    </div>

    <!-- Application Detail Modal -->
    <asp:Panel ID="pnlApplicationModal" runat="server" CssClass="modal-overlay" Style="display: none;">
        <div class="modal-content large-modal">
            <div class="modal-header">
                <h3 class="modal-title">
                    <i class="material-icons">assignment</i>
                    Application Details
                </h3>
                <asp:LinkButton ID="btnCloseModal" runat="server" CssClass="modal-close" OnClick="btnCloseModal_Click">
                    <i class="material-icons">close</i>
                </asp:LinkButton>
            </div>
            <div class="modal-body">
                <div class="application-detail-tabs">
                    <div class="tab-navigation">
                        <button type="button" class="tab-button active" onclick="showApplicationTab('personal')">Personal Info</button>
                        <button type="button" class="tab-button" onclick="showApplicationTab('position')">Position & Availability</button>
                        <button type="button" class="tab-button" onclick="showApplicationTab('history')">Employment History</button>
                        <button type="button" class="tab-button" onclick="showApplicationTab('background')">Background Check</button>
                    </div>

                    <!-- Personal Information Tab -->
                    <div id="personalTab" class="tab-content active">
                        <div class="application-section">
                            <h4 class="section-title">Personal Information</h4>
                            <div class="profile-grid">
                                <div class="profile-item">
                                    <label>Full Name</label>
                                    <div class="profile-value">
                                        <asp:Literal ID="litFullName" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="profile-item">
                                    <label>Address</label>
                                    <div class="profile-value">
                                        <asp:Literal ID="litAddress" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="profile-item">
                                    <label>Phone Numbers</label>
                                    <div class="profile-value">
                                        Home: <asp:Literal ID="litPhoneNumber" runat="server"></asp:Literal><br />
                                        Cell: <asp:Literal ID="litCellNumber" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="profile-item">
                                    <label>Emergency Contact</label>
                                    <div class="profile-value">
                                        <asp:Literal ID="litEmergencyContact" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Position & Availability Tab -->
                    <div id="positionTab" class="tab-content">
                        <div class="application-section">
                            <h4 class="section-title">Position & Availability</h4>
                            <div class="profile-grid">
                                <div class="profile-item">
                                    <label>Primary Position</label>
                                    <div class="profile-value">
                                        <asp:Literal ID="litPosition1" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="profile-item">
                                    <label>Secondary Position</label>
                                    <div class="profile-value">
                                        <asp:Literal ID="litPosition2" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="profile-item">
                                    <label>Salary Desired</label>
                                    <div class="profile-value">
                                        <asp:Literal ID="litSalaryDesired" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="profile-item">
                                    <label>Available Start Date</label>
                                    <div class="profile-value">
                                        <asp:Literal ID="litStartDate" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="profile-item">
                                    <label>Preferred Locations</label>
                                    <div class="profile-value">
                                        <asp:Literal ID="litLocations" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="profile-item">
                                    <label>Shift Preferences</label>
                                    <div class="profile-value">
                                        <asp:Literal ID="litShifts" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="profile-item">
                                    <label>Day Availability</label>
                                    <div class="profile-value">
                                        <asp:Literal ID="litDaysAvailable" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Employment History Tab -->
                    <div id="historyTab" class="tab-content">
                        <div class="application-section">
                            <h4 class="section-title">Employment History</h4>
                            <asp:Repeater ID="rptEmploymentHistory" runat="server">
                                <ItemTemplate>
                                    <div class="employment-record">
                                        <div class="employment-header">
                                            <h5><%# Eval("Employer") %></h5>
                                            <span class="employment-dates">
                                                <%# Eval("DatesEmployedFrom") %> - 
                                                <%# Eval("DatesEmployedTo") %>
                                            </span>
                                        </div>
                                        <div class="employment-details">
                                            <p><strong>Position:</strong> <%# Eval("JobTitle") %></p>
                                            <p><strong>Supervisor:</strong> <%# Eval("Supervisor") %></p>
                                            <p><strong>Work Performed:</strong> <%# Eval("TitleWorkPerformed") %></p>
                                            <p><strong>Reason for Leaving:</strong> <%# Eval("ReasonForLeaving") %></p>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                    <!-- Background Check Tab -->
                    <div id="backgroundTab" class="tab-content">
                        <div class="application-section">
                            <h4 class="section-title">Background Information</h4>
                            <div class="profile-grid">
                                <div class="profile-item">
                                    <label>Previous TPA Application</label>
                                    <div class="profile-value">
                                        <asp:Literal ID="litPreviousApplication" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="profile-item">
                                    <label>Previous TPA Employment</label>
                                    <div class="profile-value">
                                        <asp:Literal ID="litPreviousEmployment" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="profile-item">
                                    <label>Family Members at TPA</label>
                                    <div class="profile-value">
                                        <asp:Literal ID="litFamilyMembers" runat="server"></asp:Literal>
                                    </div>
                                </div>
                                <div class="profile-item">
                                    <label>Criminal Background</label>
                                    <div class="profile-value">
                                        <asp:Literal ID="litCriminalBackground" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <div class="modal-actions">
                    <asp:Button ID="btnUpdateStatus" runat="server" Text="Update for OnBoarding" CssClass="btn-tpa" OnClick="btnUpdateStatus_Click" />
                    <asp:Button ID="btnCloseModalFooter" runat="server" Text="Close" CssClass="btn-secondary" OnClick="btnCloseModal_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Status Update Modal -->
    <asp:Panel ID="pnlStatusModal" runat="server" CssClass="modal-overlay" Style="display: none;">
        <div class="modal-content medium-modal">
            <div class="modal-header">
                <h3 class="modal-title">
                    <i class="material-icons">edit</i>
                    Update Application Status
                </h3>
                <asp:LinkButton ID="btnCloseStatusModal" runat="server" CssClass="modal-close" OnClick="btnCloseStatusModal_Click">
                    <i class="material-icons">close</i>
                </asp:LinkButton>
            </div>
            <div class="modal-body">
                <div class="form-section">
                    <div class="form-group">
                        <label class="form-label">Current Status</label>
                        <asp:Label ID="lblCurrentStatus" runat="server" CssClass="current-status-display"></asp:Label>
                    </div>
                    <div class="form-group">
                        <label class="form-label">New Status</label>
                        <asp:DropDownList ID="ddlNewStatus" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Under Review" Value="Under Review"></asp:ListItem>
                            <asp:ListItem Text="Approved" Value="Approved"></asp:ListItem>
                            <asp:ListItem Text="Rejected" Value="Rejected"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Notes (Optional)</label>
                        <asp:TextBox ID="txtStatusNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Add any notes about this status change..."></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <div class="modal-actions">
                    <asp:Button ID="btnSaveStatus" runat="server" Text="Update Status" CssClass="btn-tpa" OnClick="btnSaveStatus_Click" />
                    <asp:Button ID="btnCancelStatus" runat="server" Text="Cancel" CssClass="btn-secondary" OnClick="btnCloseStatusModal_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Hidden fields for modal state -->
    <asp:HiddenField ID="hdnSelectedApplicationId" runat="server" />

    <script type="text/javascript">
        // Application Detail Tab Functions
        function showApplicationTab(tabName) {
            // Hide all tabs
            document.querySelectorAll('.tab-content').forEach(function(tab) {
                tab.classList.remove('active');
            });
            
            // Remove active class from all buttons
            document.querySelectorAll('.tab-button').forEach(function(btn) {
                btn.classList.remove('active');
            });
            
            // Show selected tab
            document.getElementById(tabName + 'Tab').classList.add('active');
            
            // Add active class to clicked button
            event.target.classList.add('active');
        }

        // Modal Functions
        function showApplicationModal() {
            var modal = document.getElementById('<%= pnlApplicationModal.ClientID %>');
            if (modal) {
                modal.style.display = 'flex';
                showApplicationTab('personal');
            }
        }

        function hideApplicationModal() {
            var modal = document.getElementById('<%= pnlApplicationModal.ClientID %>');
            if (modal) {
                modal.style.display = 'none';
            }
        }

        function showStatusModal() {
            var modal = document.getElementById('<%= pnlStatusModal.ClientID %>');
            if (modal) {
                modal.style.display = 'flex';
            }
        }

        function hideStatusModal() {
            var modal = document.getElementById('<%= pnlStatusModal.ClientID %>');
            if (modal) {
                modal.style.display = 'none';
            }
        }

        // Confirmation dialogs
        function confirmDelete() {
            return confirm('Are you sure you want to delete this application? This action cannot be undone.');
        }

        function confirmStatusUpdate() {
            return confirm('Are you sure you want to update the status of this application?');
        }

        // Close modal when clicking outside
        window.onclick = function (event) {
            var applicationModal = document.getElementById('<%= pnlApplicationModal.ClientID %>');
            var statusModal = document.getElementById('<%= pnlStatusModal.ClientID %>');

            if (event.target === applicationModal) {
                hideApplicationModal();
            }
            if (event.target === statusModal) {
                hideStatusModal();
            }
        }
    </script>
</asp:Content>