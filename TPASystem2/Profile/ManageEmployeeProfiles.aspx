<%@ Page Title="Employee Profile Management" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ManageEmployeeProfiles.aspx.cs" Inherits="TPASystem2.HR.ManageEmployeeProfiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

  <style>

  

/* Employee Grid Specific Styles */
.employee-info-cell {
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 0.5rem 0;
}

.employee-avatar {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background: linear-gradient(135deg, var(--tpa-primary), var(--tpa-secondary));
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    font-weight: 700;
    font-size: 0.9rem;
    flex-shrink: 0;
}

.employee-details {
    flex: 1;
}

.employee-name {
    font-weight: 600;
    color: #1e293b;
    margin-bottom: 0.25rem;
}

.employee-number {
    font-size: 0.8rem;
    color: #64748b;
    font-style: italic;
}

/* System Access Info */
.system-access-info {
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
}

.access-role {
    font-weight: 500;
    color: #374151;
    font-size: 0.9rem;
}

.access-status {
    font-size: 0.8rem;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.access-status.access-active {
    color: #10b981;
}

.access-status.access-inactive {
    color: #ef4444;
}

/* Status Badges */
.status-badge {
    display: inline-flex;
    align-items: center;
    gap: 0.25rem;
    padding: 0.25rem 0.75rem;
    border-radius: 20px;
    font-size: 0.8rem;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.status-badge.active {
    background: #dcfce7;
    color: #166534;
}

.status-badge.inactive {
    background: #fee2e2;
    color: #991b1b;
}

.status-badge.terminated {
    background: #fef3c7;
    color: #92400e;
}

.status-badge.on-leave {
    background: #dbeafe;
    color: #1d4ed8;
}

.status-badge.unknown {
    background: #f3f4f6;
    color: #6b7280;
}

/* Action Buttons */
.actions-column {
    width: 180px;
    text-align: center;
}

.action-buttons {
    display: flex;
    gap: 0.5rem;
    justify-content: center;
    flex-wrap: wrap;
}

.btn-action {
    width: 32px;
    height: 32px;
    border-radius: 6px;
    border: none;
    cursor: pointer;
    transition: all 0.3s ease;
    display: flex;
    align-items: center;
    justify-content: center;
    text-decoration: none;
    color: white;
}

.btn-action .material-icons {
    font-size: 16px;
}

.btn-action.btn-primary {
    background: linear-gradient(135deg, #3b82f6, #2563eb);
}

.btn-action.btn-primary:hover {
    background: linear-gradient(135deg, #2563eb, #1d4ed8);
    transform: translateY(-1px);
    box-shadow: 0 4px 8px rgba(59, 130, 246, 0.3);
}

.btn-action.btn-info {
    background: linear-gradient(135deg, #06b6d4, #0891b2);
}

.btn-action.btn-info:hover {
    background: linear-gradient(135deg, #0891b2, #0e7490);
    transform: translateY(-1px);
    box-shadow: 0 4px 8px rgba(6, 182, 212, 0.3);
}

.btn-action.btn-warning {
    background: linear-gradient(135deg, #f59e0b, #d97706);
}

.btn-action.btn-warning:hover {
    background: linear-gradient(135deg, #d97706, #b45309);
    transform: translateY(-1px);
    box-shadow: 0 4px 8px rgba(245, 158, 11, 0.3);
}

.btn-action.btn-success {
    background: linear-gradient(135deg, #10b981, #059669);
}

.btn-action.btn-success:hover {
    background: linear-gradient(135deg, #059669, #047857);
    transform: translateY(-1px);
    box-shadow: 0 4px 8px rgba(16, 185, 129, 0.3);
}

.btn-action.btn-danger {
    background: linear-gradient(135deg, #ef4444, #dc2626);
}

.btn-action.btn-danger:hover {
    background: linear-gradient(135deg, #dc2626, #b91c1c);
    transform: translateY(-1px);
    box-shadow: 0 4px 8px rgba(239, 68, 68, 0.3);
}

/* Modal Enhancements */
.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.6);
    z-index: 10000;
    display: flex;
    align-items: center;
    justify-content: center;
    backdrop-filter: blur(4px);
}

.modal-content {
    background: white;
    border-radius: 16px;
    box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
    max-height: 90vh;
    overflow-y: auto;
    animation: modalSlideIn 0.3s ease;
    position: relative;
}

.modal-content.large-modal {
    width: 90%;
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
    padding: 0;
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

/* Welcome Actions Enhancement */
.welcome-actions {
    display: flex;
    gap: 1rem;
    align-items: center;
}

.welcome-actions .btn-tpa {
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.welcome-actions .btn-secondary {
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

/* Filter Grid Enhancement */
.filter-grid {
    display: grid;
    grid-template-columns: 2fr 1fr 1fr 1fr;
    gap: 1.5rem;
    align-items: end;
    margin-bottom: 1.5rem;
}

.filter-actions {
    display: flex;
    gap: 1rem;
    justify-content: flex-end;
    margin-top: 1rem;
    padding-top: 1rem;
    border-top: 1px solid #e5e7eb;
}

/* Badge Enhancements */
.badge {
    display: inline-flex;
    align-items: center;
    gap: 0.25rem;
    padding: 0.5rem 1rem;
    border-radius: 20px;
    font-size: 0.85rem;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.badge.badge-info {
    background: #dbeafe;
    color: #1d4ed8;
}

.badge.badge-success {
    background: #dcfce7;
    color: #166534;
}

.badge.badge-warning {
    background: #fef3c7;
    color: #92400e;
}

.badge.badge-error {
    background: #fee2e2;
    color: #991b1b;
}

/* GridView Pager Enhancements */
.gridview-pager {
    padding: 1.5rem;
    text-align: center;
    background: #f8fafc;
    border-top: 1px solid #e5e7eb;
    border-radius: 0 0 12px 12px;
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
}

.gridview-pager a:hover {
    background: #fff7ed;
    border-color: var(--tpa-primary);
    transform: translateY(-1px);
}

.gridview-pager span {
    padding: 0.75rem 1rem;
    background: var(--tpa-primary);
    color: white;
    border-radius: 8px;
    font-weight: 600;
    border: 1px solid var(--tpa-primary);
}

/* Empty Data State */
.data-table .empty-data {
    text-align: center;
    padding: 3rem 2rem;
    color: #6b7280;
    font-style: italic;
}

/* Responsive Enhancements */
@media (max-width: 1024px) {
    .filter-grid {
        grid-template-columns: 1fr 1fr;
        gap: 1rem;
    }
    
    .action-buttons {
        flex-direction: column;
        gap: 0.25rem;
    }
    
    .btn-action {
        width: 28px;
        height: 28px;
    }
    
    .modal-content.large-modal {
        width: 95%;
    }
}

@media (max-width: 768px) {
    .filter-grid {
        grid-template-columns: 1fr;
    }
    
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
    
    .actions-column {
        width: auto;
    }
    
    .action-buttons {
        flex-direction: row;
        gap: 0.25rem;
        justify-content: flex-start;
    }
    
    .modal-header {
        padding: 1.5rem 1.5rem 1rem 1.5rem;
    }
    
    .modal-footer {
        padding: 1rem 1.5rem;
    }
    
    .modal-actions {
        flex-direction: column;
        gap: 0.75rem;
    }
    
    .btn-profile-save,
    .btn-secondary {
        width: 100%;
        justify-content: center;
    }
    
    .welcome-actions {
        flex-direction: column;
        width: 100%;
        gap: 0.75rem;
    }
    
    .welcome-actions .btn-tpa,
    .welcome-actions .btn-secondary {
        width: 100%;
        justify-content: center;
    }
}

@media (max-width: 480px) {
    .employee-avatar {
        width: 28px;
        height: 28px;
        font-size: 0.7rem;
    }
    
    .employee-name {
        font-size: 0.9rem;
    }
    
    .employee-number {
        font-size: 0.75rem;
    }
    
    .btn-action {
        width: 24px;
        height: 24px;
    }
    
    .btn-action .material-icons {
        font-size: 14px;
    }
    
    .modal-content {
        margin: 1rem;
        width: calc(100% - 2rem);
    }
    
    .modal-header {
        padding: 1rem;
    }
    
    .modal-title {
        font-size: 1.25rem;
    }
    
    .modal-footer {
        padding: 1rem;
    }
    
    .profile-grid {
        grid-template-columns: 1fr;
    }
}

/* Print Styles for Employee Grid */
@media print {
    .welcome-actions,
    .filter-grid,
    .filter-actions,
    .action-buttons,
    .modal-overlay,
    .btn-tpa,
    .btn-secondary,
    .btn-action {
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
    
    .employee-avatar {
        background: #f0f0f0 !important;
        color: #000 !important;
    }
    
    .status-badge {
        border: 1px solid #000 !important;
        background: #fff !important;
        color: #000 !important;
    }
}

/* Loading States */
.loading-grid {
    position: relative;
    min-height: 200px;
}

.loading-grid::after {
    content: '';
    position: absolute;
    top: 50%;
    left: 50%;
    width: 40px;
    height: 40px;
    margin: -20px 0 0 -20px;
    border: 4px solid #e5e7eb;
    border-top: 4px solid var(--tpa-primary);
    border-radius: 50%;
    animation: spin 1s linear infinite;
}

/* Enhanced Data Table */
.data-table {
    width: 100%;
    border-collapse: collapse;
    background: white;
    border-radius: 12px;
    overflow: hidden;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
    border: 1px solid #e5e7eb;
}

.data-table th {
    background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
    color: #374151;
    font-weight: 600;
    text-align: left;
    padding: 1rem;
    border-bottom: 2px solid #e5e7eb;
    font-size: 0.9rem;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.data-table td {
    padding: 1rem;
    border-bottom: 1px solid #f3f4f6;
    vertical-align: middle;
    font-size: 0.9rem;
}

.data-table tr:hover {
    background: #f8fafc;
}

.data-table tr:last-child td {
    border-bottom: none;
}

/* Content Card Enhancements */
.content-card {
    background: white;
    border-radius: 16px;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
    border: 1px solid #e5e7eb;
    overflow: hidden;
    margin-bottom: 2rem;
}

.card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1.5rem 2rem;
    border-bottom: 1px solid #e5e7eb;
    background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
}

.card-header h3 {
    margin: 0;
    font-size: 1.25rem;
    font-weight: 600;
    color: #1e293b;
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.card-header h3 .material-icons {
    color: var(--tpa-primary);
    font-size: 1.5rem;
}

.card-actions {
    display: flex;
    gap: 1rem;
    align-items: center;
}

.card-content {
    padding: 0;
}

/* Management Controls Enhancement */
.management-controls {
    background: white;
    border-radius: 16px;
    padding: 2rem;
    margin-bottom: 2rem;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
    border: 1px solid #e5e7eb;
}

.controls-header {
    margin-bottom: 1.5rem;
    padding-bottom: 1rem;
    border-bottom: 1px solid #e5e7eb;
}

.controls-header h3 {
    margin: 0;
    font-size: 1.25rem;
    font-weight: 600;
    color: #1e293b;
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.controls-header h3 .material-icons {
    color: var(--tpa-primary);
    font-size: 1.5rem;
}

/* Form Control Enhancements for Grid */
.form-control:focus {
    outline: none;
    border-color: var(--tpa-primary);
    box-shadow: 0 0 0 3px rgba(255, 152, 0, 0.1);
    transform: translateY(-1px);
}

.form-select {
    appearance: none;
    background-image: url("data:image/svg+xml;charset=utf-8,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3E%3Cpath stroke='%236B7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='M6 8l4 4 4-4'/%3E%3C/svg%3E");
    background-position: right 0.75rem center;
    background-repeat: no-repeat;
    background-size: 16px 12px;
    padding-right: 2.5rem;
}

.form-select:focus {
    background-image: url("data:image/svg+xml;charset=utf-8,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3E%3Cpath stroke='%23FF9800' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='M6 8l4 4 4-4'/%3E%3C/svg%3E");
}

/* Accessibility Enhancements */
.btn-action:focus,
.modal-close:focus {
    outline: 2px solid var(--tpa-primary);
    outline-offset: 2px;
}

/* High Contrast Mode Support */
@media (prefers-contrast: high) {
    .data-table th,
    .data-table td {
        border-color: #000;
        border-width: 2px;
    }
    
    .employee-avatar {
        border: 2px solid #000;
    }
    
    .btn-action {
        border: 2px solid #000;
    }
    
    .status-badge {
        border: 2px solid #000;
    }
}

/* Reduced Motion Support */
@media (prefers-reduced-motion: reduce) {
    .btn-action,
    .modal-close,
    .data-table tr {
        transition: none;
    }
    
    .modal-content {
        animation: none;
    }
    
    .loading-grid::after {
        animation: none;
    }
}
  </style>
    <!-- Welcome Header -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">people</i>
                    Employee Profile Management
                </h1>
                <p class="welcome-subtitle">Manage employee profiles, personal information, and employment details</p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">person</i>
                        <span>
                            <asp:Literal ID="litManagerName" runat="server" Text="System Administrator"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">business</i>
                        <span>
                            <asp:Literal ID="litDepartmentName" runat="server" Text="Administration"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">admin_panel_settings</i>
                        <span>
                            <asp:Literal ID="litUserRole" runat="server" Text="System Administrator"></asp:Literal>
                        </span>
                    </div>
                </div>
            </div>
            
            <div class="welcome-actions">
               <!-- Fixed Add Employee Button -->
<asp:Button ID="btnAddEmployee" runat="server" Text="Add Employee" 
    CssClass="btn-tpa" OnClick="btnAddEmployee_Click"  Visible="false"/>
      <asp:Button ID="Button1" runat="server" Text="Add Employee" OnClick="Button1_Click" />
<!-- Fixed Export Button -->
<asp:Button ID="btnExportEmployees" runat="server" Text="Export" 
    CssClass="btn-secondary" OnClick="btnExportEmployees_Click" />
            </div>
        </div>
    </div>

    <!-- Message Panel -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Dashboard Overview -->
    

    <!-- Filters Section -->
    <div class="management-controls">
        <div class="controls-header">
            <h3>
                <i class="material-icons">filter_list</i>
                Employee Filters
            </h3>
        </div>
        
        <div class="filter-grid">
            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">search</i>
                    Employee Search:
                </label>
                <asp:TextBox ID="txtEmployeeSearch" runat="server" CssClass="form-control" 
                    placeholder="Search by name, email, or employee number" AutoPostBack="true" 
                    OnTextChanged="txtEmployeeSearch_TextChanged"></asp:TextBox>
            </div>
            
            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">business</i>
                    Department:
                </label>
                <asp:DropDownList ID="ddlDepartmentFilter" runat="server" CssClass="form-control" 
                    AutoPostBack="true" OnSelectedIndexChanged="ddlDepartmentFilter_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
            
            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">work</i>
                    Employee Type:
                </label>
                <asp:DropDownList ID="ddlEmployeeTypeFilter" runat="server" CssClass="form-control" 
                    AutoPostBack="true" OnSelectedIndexChanged="ddlEmployeeTypeFilter_SelectedIndexChanged">
                    <asp:ListItem Text="All Types" Value=""></asp:ListItem>
                    <asp:ListItem Text="Full-Time" Value="Full-Time"></asp:ListItem>
                    <asp:ListItem Text="Part-Time" Value="Part-Time"></asp:ListItem>
                    <asp:ListItem Text="Contract" Value="Contract"></asp:ListItem>
                    <asp:ListItem Text="Temporary" Value="Temporary"></asp:ListItem>
                </asp:DropDownList>
            </div>
            
            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">toggle_on</i>
                    Status:
                </label>
                <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-control" 
                    AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                    <asp:ListItem Text="All Statuses" Value=""></asp:ListItem>
                    <asp:ListItem Text="Active" Value="ACTIVE"></asp:ListItem>
                    <asp:ListItem Text="Inactive" Value="INACTIVE"></asp:ListItem>
                    <asp:ListItem Text="Terminated" Value="TERMINATED"></asp:ListItem>
                    <asp:ListItem Text="On Leave" Value="ON_LEAVE"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        
        <div class="filter-actions">
            <asp:Button ID="btnClearFilters" runat="server" Text="Clear Filters" 
                CssClass="btn-outline" OnClick="btnClearFilters_Click" />
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" 
                CssClass="btn-secondary" OnClick="btnRefresh_Click" />
        </div>
    </div>

    <!-- Employee Grid -->
    <div class="content-card">
        <div class="card-header">
            <h3>
                <i class="material-icons">people</i>
                Employee Directory
            </h3>
            <div class="card-actions">
                <span class="badge badge-info">
                    <asp:Literal ID="litEmployeeCount" runat="server">0</asp:Literal> employees found
                </span>
            </div>
        </div>
        
        <div class="card-content">
            <asp:GridView ID="gvEmployees" runat="server" CssClass="data-table responsive-table" 
                         AutoGenerateColumns="false" DataKeyNames="Id" 
                         OnRowCommand="gvEmployees_RowCommand"
                         OnRowDataBound="gvEmployees_RowDataBound"
                         AllowPaging="true" PageSize="25"
                         OnPageIndexChanging="gvEmployees_PageIndexChanging"
                         EmptyDataText="No employees found matching your criteria.">
                <Columns>
                    <asp:TemplateField HeaderText="Employee">
                        <ItemTemplate>
                            <div class="employee-info-cell">
                                <div class="employee-avatar">
                                    <%# GetEmployeeInitials(Eval("FirstName"), Eval("LastName")) %>
                                </div>
                                <div class="employee-details">
                                    <div class="employee-name"><%# Eval("FirstName") %> <%# Eval("LastName") %></div>
                                    <div class="employee-number"><%# Eval("EmployeeNumber") %></div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:BoundField DataField="Email" HeaderText="Email" />
                    <asp:BoundField DataField="DepartmentName" HeaderText="Department" />
                    <asp:BoundField DataField="JobTitle" HeaderText="Job Title" />
                    <asp:BoundField DataField="EmployeeType" HeaderText="Type" />
                    <asp:BoundField DataField="HireDate" HeaderText="Hire Date" DataFormatString="{0:MMM dd, yyyy}" />
                    
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class="status-badge <%# GetStatusClass(Eval("Status")) %>">
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="System Access">
                        <ItemTemplate>
                            <div class="system-access-info">
                                <span class="access-role"><%# Eval("UserRole") ?? "No Access" %></span>
                                <span class="access-status <%# GetAccessStatusClass(Eval("UserIsActive")) %>">
                                    <%# Convert.ToBoolean(Eval("UserIsActive") ?? false) ? "Active" : "Inactive" %>
                                </span>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Actions" ItemStyle-CssClass="actions-column">
                        <ItemTemplate>
                            <div class="action-buttons">
                                <asp:LinkButton ID="btnViewProfile" runat="server" 
                                    CommandName="ViewProfile" CommandArgument='<%# Eval("Id") %>'
                                    CssClass="btn-action btn-primary" ToolTip="View/Edit Profile">
                                    <i class="material-icons">edit</i>
                                </asp:LinkButton>
                                
                                <asp:LinkButton ID="btnViewActivity" runat="server" 
                                    CommandName="ViewActivity" CommandArgument='<%# Eval("Id") %>'
                                    CssClass="btn-action btn-info" ToolTip="View Activity">
                                    <i class="material-icons">history</i>
                                </asp:LinkButton>
                                
                                <asp:LinkButton ID="btnToggleStatus" runat="server" 
                                    CommandName="ToggleStatus" CommandArgument='<%# Eval("Id") %>'
                                    CssClass='<%# "btn-action " + (Eval("Status").ToString() == "ACTIVE" ? "btn-warning" : "btn-success") %>' 
                                    ToolTip='<%# Eval("Status").ToString() == "ACTIVE" ? "Deactivate" : "Activate" %>'
                                    OnClientClick="return confirm('Are you sure you want to change this employee\'s status?');">
                                    <i class="material-icons"><%# Eval("Status").ToString() == "ACTIVE" ? "pause" : "play_arrow" %></i>
                                </asp:LinkButton>
                                
                                <asp:LinkButton ID="btnDeleteEmployee" runat="server" 
                                    CommandName="DeleteEmployee" CommandArgument='<%# Eval("Id") %>'
                                    CssClass="btn-action btn-danger" ToolTip="Delete Employee"
                                    OnClientClick="return confirm('Are you sure you want to delete this employee? This action cannot be undone.');"
                                    Visible='<%# IsAdmin() %>'>
                                    <i class="material-icons">delete</i>
                                </asp:LinkButton>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                
                <PagerStyle CssClass="gridview-pager" />
            </asp:GridView>
        </div>
    </div>

    <!-- Employee Profile Modal Panel -->
    <!-- REPLACE the modal panels in your ASPX with these corrected versions that match the existing system: -->

<!-- Employee Profile Modal Panel - Fixed to match existing pattern -->
<asp:Panel ID="pnlEmployeeModal" runat="server" CssClass="modal-overlay" Visible="false" style="display: none;">
    <div class="modal-content large-modal">
        <div class="modal-header">
            <h4 class="modal-title">
                <i class="material-icons">person</i>
                Employee Profile: <asp:Literal ID="litSelectedEmployee" runat="server"></asp:Literal>
            </h4>
            <asp:LinkButton ID="btnCloseModal" runat="server" CssClass="modal-close" OnClick="btnCloseModal_Click">
                <i class="material-icons">close</i>
            </asp:LinkButton>
        </div>
        
        <div class="modal-body">
            <!-- Profile Tabs -->
            <div class="profile-tabs-container">
                <div class="profile-tabs">
                    <button type="button" class="tab-button active" onclick="showTab('personal'); return false;">
                        <i class="material-icons">person</i>
                        Personal Info
                    </button>
                    <button type="button" class="tab-button" onclick="showTab('employment'); return false;">
                        <i class="material-icons">work</i>
                        Employment
                    </button>
                    <button type="button" class="tab-button" onclick="showTab('contact'); return false;">
                        <i class="material-icons">contact_mail</i>
                        Contact
                    </button>
                    <button type="button" class="tab-button" onclick="showTab('system'); return false;">
                        <i class="material-icons">security</i>
                        System Access
                    </button>
                </div>

                <!-- Personal Information Tab -->
                <div id="personalTab" class="tab-content active">
                    <div class="profile-section">
                        <h3><i class="material-icons">person</i>Personal Information</h3>
                        <div class="profile-grid">
                            <div class="form-group">
                                <label for="txtFirstName">First Name <span class="required">*</span></label>
                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" 
                                    placeholder="Enter first name" required></asp:TextBox>
                            </div>
                            
                            <div class="form-group">
                                <label for="txtLastName">Last Name <span class="required">*</span></label>
                                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" 
                                    placeholder="Enter last name" required></asp:TextBox>
                            </div>
                            
                            <div class="form-group">
                                <label for="txtEmployeeNumber">Employee Number <span class="required">*</span></label>
                                <asp:TextBox ID="txtEmployeeNumber" runat="server" CssClass="form-control" 
                                    placeholder="Enter employee number" required></asp:TextBox>
                            </div>
                            
                            <div class="form-group">
                                <label for="txtDateOfBirth">Date of Birth</label>
                                <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control" 
                                    TextMode="Date"></asp:TextBox>
                            </div>
                            
                            <div class="form-group">
                                <label for="ddlGender">Gender</label>
                                <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="" Text="Select Gender"></asp:ListItem>
                                    <asp:ListItem Value="Male" Text="Male"></asp:ListItem>
                                    <asp:ListItem Value="Female" Text="Female"></asp:ListItem>
                                    <asp:ListItem Value="Other" Text="Other"></asp:ListItem>
                                    <asp:ListItem Value="Prefer not to say" Text="Prefer not to say"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Employment Information Tab -->
                <div id="employmentTab" class="tab-content">
                    <div class="profile-section">
                        <h3><i class="material-icons">work</i>Employment Information</h3>
                        <div class="profile-grid">
                            <div class="form-group">
                                <label for="txtJobTitle">Job Title <span class="required">*</span></label>
                                <asp:TextBox ID="txtJobTitle" runat="server" CssClass="form-control" 
                                    placeholder="Enter job title" required></asp:TextBox>
                            </div>
                            
                            <div class="form-group">
                                <label for="ddlDepartment">Department <span class="required">*</span></label>
                                <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-control" required>
                                </asp:DropDownList>
                            </div>
                            
                            <div class="form-group">
                                <label for="ddlEmployeeType">Employee Type</label>
                                <asp:DropDownList ID="ddlEmployeeType" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="" Text="Select Type"></asp:ListItem>
                                    <asp:ListItem Value="Full-Time" Text="Full-Time"></asp:ListItem>
                                    <asp:ListItem Value="Part-Time" Text="Part-Time"></asp:ListItem>
                                    <asp:ListItem Value="Contract" Text="Contract"></asp:ListItem>
                                    <asp:ListItem Value="Temporary" Text="Temporary"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            
                            <div class="form-group">
                                <label for="txtHireDate">Hire Date <span class="required">*</span></label>
                                <asp:TextBox ID="txtHireDate" runat="server" CssClass="form-control" 
                                    TextMode="Date" required></asp:TextBox>
                            </div>
                            
                            <div class="form-group">
                                <label for="ddlManager">Manager</label>
                                <asp:DropDownList ID="ddlManager" runat="server" CssClass="form-control">
                                </asp:DropDownList>
                            </div>
                            
                            <div class="form-group">
                                <label for="ddlEmployeeStatus">Employment Status</label>
                                <asp:DropDownList ID="ddlEmployeeStatus" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="ACTIVE" Text="Active"></asp:ListItem>
                                    <asp:ListItem Value="INACTIVE" Text="Inactive"></asp:ListItem>
                                    <asp:ListItem Value="TERMINATED" Text="Terminated"></asp:ListItem>
                                    <asp:ListItem Value="ON_LEAVE" Text="On Leave"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            
                            <div class="form-group">
                                <label for="txtSalary">Salary</label>
                                <asp:TextBox ID="txtSalary" runat="server" CssClass="form-control" 
                                    placeholder="Enter salary" TextMode="Number"></asp:TextBox>
                            </div>
                            
                            <div class="form-group">
                                <label for="txtWorkLocation">Work Location</label>
                                <asp:TextBox ID="txtWorkLocation" runat="server" CssClass="form-control" 
                                    placeholder="Enter work location"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Contact Information Tab -->
                <div id="contactTab" class="tab-content">
                    <div class="profile-section">
                        <h3><i class="material-icons">contact_mail</i>Contact Information</h3>
                        <div class="profile-grid">
                            <div class="form-group">
                                <label for="txtEmail">Email Address <span class="required">*</span></label>
                                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" 
                                    placeholder="Enter email address" TextMode="Email" required></asp:TextBox>
                            </div>
                            
                            <div class="form-group">
                                <label for="txtPhoneNumber">Phone Number</label>
                                <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control" 
                                    placeholder="Enter phone number"></asp:TextBox>
                            </div>
                            
                            <div class="form-group full-width">
                                <label for="txtAddress">Address</label>
                                <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" 
                                    placeholder="Enter address"></asp:TextBox>
                            </div>
                            
                            <div class="form-group">
                                <label for="txtCity">City</label>
                                <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" 
                                    placeholder="Enter city"></asp:TextBox>
                            </div>
                            
                            <div class="form-group">
                                <label for="txtState">State</label>
                                <asp:TextBox ID="txtState" runat="server" CssClass="form-control" 
                                    placeholder="Enter state"></asp:TextBox>
                            </div>
                            
                            <div class="form-group">
                                <label for="txtZipCode">ZIP Code</label>
                                <asp:TextBox ID="txtZipCode" runat="server" CssClass="form-control" 
                                    placeholder="Enter ZIP code"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- System Access Tab -->
                <div id="systemTab" class="tab-content">
                    <div class="profile-section">
                        <h3><i class="material-icons">security</i>System Access</h3>
                        <div class="profile-grid">
                            <div class="form-group">
                                <label for="ddlUserRole">User Role</label>
                                <asp:DropDownList ID="ddlUserRole" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="" Text="No System Access"></asp:ListItem>
                                    <asp:ListItem Value="EMPLOYEE" Text="Employee"></asp:ListItem>
                                    <asp:ListItem Value="SUPERVISOR" Text="Supervisor"></asp:ListItem>
                                    <asp:ListItem Value="PROGRAMDIRECTOR" Text="Program Director"></asp:ListItem>
                                    <asp:ListItem Value="HRADMIN" Text="HR Admin"></asp:ListItem>
                                    <asp:ListItem Value="ADMIN" Text="Admin"></asp:ListItem>
                                    <asp:ListItem Value="SUPERADMIN" Text="Super Admin"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            
                            <div class="form-group">
                                <label for="chkIsActive">System Access Active</label>
                                <div class="checkbox-wrapper">
                                    <asp:CheckBox ID="chkIsActive" runat="server" Text="Allow system access" />
                                </div>
                            </div>
                            
                            <div class="form-group">
                                <label for="chkMustChangePassword">Force Password Change</label>
                                <div class="checkbox-wrapper">
                                    <asp:CheckBox ID="chkMustChangePassword" runat="server" Text="User must change password on next login" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        
        <div class="modal-footer">
            <asp:Button ID="btnSaveProfile" runat="server" Text="Save Changes" 
                CssClass="btn btn-primary" OnClick="btnSaveProfile_Click" />
            <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" 
                CssClass="btn btn-outline" OnClick="btnCancelEdit_Click" />
        </div>
    </div>
</asp:Panel>

<!-- Activity Modal Panel - Fixed -->
<asp:Panel ID="pnlActivityModal" runat="server" CssClass="modal-overlay" Visible="false" style="display: none;">
    <div class="modal-content">
        <div class="modal-header">
            <h4 class="modal-title">
                <i class="material-icons">history</i>
                Recent Activity: <asp:Literal ID="litActivityEmployee" runat="server"></asp:Literal>
            </h4>
            <asp:LinkButton ID="btnCloseActivityModal" runat="server" CssClass="modal-close" OnClick="btnCloseActivityModal_Click">
                <i class="material-icons">close</i>
            </asp:LinkButton>
        </div>
        
        <div class="modal-body">
            <div class="activity-list">
                <asp:Literal ID="litRecentActivity" runat="server"></asp:Literal>
            </div>
        </div>
    </div>
</asp:Panel>

  <script type="text/javascript">
      // Tab switching functionality
      function showTab(tabName) {
          // Hide all tab contents
          var tabContents = document.querySelectorAll('.tab-content');
          tabContents.forEach(function (content) {
              content.classList.remove('active');
          });

          // Remove active class from all tab buttons
          var tabButtons = document.querySelectorAll('.tab-button');
          tabButtons.forEach(function (button) {
              button.classList.remove('active');
          });

          // Show selected tab content using static IDs
          var targetTab = document.getElementById(tabName + 'Tab');
          if (targetTab) {
              targetTab.classList.add('active');
          }

          // Add active class to clicked button
          if (event && event.target) {
              var clickedButton = event.target.closest('.tab-button');
              if (clickedButton) {
                  clickedButton.classList.add('active');
              }
          }

          return false; // Prevent form submission
      }

      // Modal display functions
      function showModal() {
          var modal = document.getElementById('<%= pnlEmployeeModal.ClientID %>');
          if (modal) {
              modal.style.display = 'flex';
              showTab('personal');
          }
      }

      function hideModal() {
          var modal = document.getElementById('<%= pnlEmployeeModal.ClientID %>');
          if (modal) {
              modal.style.display = 'none';
          }
      }

      function showActivityModal() {
          var modal = document.getElementById('<%= pnlActivityModal.ClientID %>');
          if (modal) {
              modal.style.display = 'flex';
          }
      }

      function hideActivityModal() {
          var modal = document.getElementById('<%= pnlActivityModal.ClientID %>');
          if (modal) {
              modal.style.display = 'none';
          }
      }

      // Close modal functions
      function closeModal() {
          hideModal();
      }

      function closeActivityModal() {
          hideActivityModal();
      }

      // Form validation
      function validateProfileForm() {
          var isValid = true;
          var errorMessage = '';

          // Required fields validation
          var firstName = document.getElementById('<%= txtFirstName.ClientID %>');
            var lastName = document.getElementById('<%= txtLastName.ClientID %>');
            var employeeNumber = document.getElementById('<%= txtEmployeeNumber.ClientID %>');
            var email = document.getElementById('<%= txtEmail.ClientID %>');

          if (firstName && !firstName.value.trim()) {
              errorMessage += 'First Name is required.\n';
              isValid = false;
          }

          if (lastName && !lastName.value.trim()) {
              errorMessage += 'Last Name is required.\n';
              isValid = false;
          }

          if (employeeNumber && !employeeNumber.value.trim()) {
              errorMessage += 'Employee Number is required.\n';
              isValid = false;
          }

          if (email && !email.value.trim()) {
              errorMessage += 'Email is required.\n';
              isValid = false;
          } else if (email && email.value.trim() && !isValidEmail(email.value.trim())) {
              errorMessage += 'Please enter a valid email address.\n';
              isValid = false;
          }

          if (!isValid) {
              alert('Please correct the following errors:\n\n' + errorMessage);
          }

          return isValid;
      }

      function isValidEmail(email) {
          var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
          return emailRegex.test(email);
      }

      // Grid functionality
      function confirmDelete(employeeName) {
          return confirm('Are you sure you want to delete ' + employeeName + '? This action cannot be undone.');
      }

      function confirmStatusChange(employeeName, currentStatus) {
          var newStatus = currentStatus === 'ACTIVE' ? 'INACTIVE' : 'ACTIVE';
          return confirm('Are you sure you want to change ' + employeeName + '\'s status to ' + newStatus + '?');
      }

      // Search enhancement
      function highlightSearchTerm() {
          var searchInput = document.getElementById('<%= txtEmployeeSearch.ClientID %>');
            if (searchInput) {
                var searchTerm = searchInput.value.toLowerCase();
                if (searchTerm.length > 0) {
                    var gridId = '<%= gvEmployees.ClientID %>';
                  var grid = document.getElementById(gridId);
                  if (grid) {
                      var rows = grid.querySelectorAll('tr');
                      rows.forEach(function (row) {
                          var cells = row.querySelectorAll('td');
                          cells.forEach(function (cell) {
                              var text = cell.textContent.toLowerCase();
                              if (text.includes(searchTerm)) {
                                  cell.style.backgroundColor = '#fff3cd';
                              }
                          });
                      });
                  }
              }
          }
      }

      // Auto-save form data to prevent loss
      function autoSaveFormData() {
          if (typeof (Storage) !== "undefined") {
              var formData = {
                  firstName: document.getElementById('<%= txtFirstName.ClientID %>') ? document.getElementById('<%= txtFirstName.ClientID %>').value : '',
                    lastName: document.getElementById('<%= txtLastName.ClientID %>') ? document.getElementById('<%= txtLastName.ClientID %>').value : '',
                    email: document.getElementById('<%= txtEmail.ClientID %>') ? document.getElementById('<%= txtEmail.ClientID %>').value : ''
                };
                localStorage.setItem('employeeFormData', JSON.stringify(formData));
            }
        }

        function restoreFormData() {
            if (typeof(Storage) !== "undefined") {
                var savedData = localStorage.getItem('employeeFormData');
                if (savedData) {
                    var formData = JSON.parse(savedData);
                    var firstNameField = document.getElementById('<%= txtFirstName.ClientID %>');
                    var lastNameField = document.getElementById('<%= txtLastName.ClientID %>');
                    var emailField = document.getElementById('<%= txtEmail.ClientID %>');
                    
                    if (firstNameField && !firstNameField.value) firstNameField.value = formData.firstName || '';
                    if (lastNameField && !lastNameField.value) lastNameField.value = formData.lastName || '';
                    if (emailField && !emailField.value) emailField.value = formData.email || '';
                }
            }
        }

        function clearAutoSavedData() {
            if (typeof(Storage) !== "undefined") {
                localStorage.removeItem('employeeFormData');
            }
        }

        // Initialize page functionality
        function initializePage() {
            // Auto-hide success messages
            setTimeout(function() {
                var successAlerts = document.querySelectorAll('.alert-success');
                successAlerts.forEach(function(alert) {
                    if (alert) {
                        alert.style.display = 'none';
                    }
                });
            }, 5000);

            // Close modal when clicking outside
            var modals = document.querySelectorAll('.modal-overlay');
            modals.forEach(function(modal) {
                modal.addEventListener('click', function(e) {
                    if (e.target === modal) {
                        modal.style.display = 'none';
                        clearAutoSavedData();
                    }
                });
            });

            // Prevent modal content clicks from closing modal
            var modalContents = document.querySelectorAll('.modal-content');
            modalContents.forEach(function(content) {
                content.addEventListener('click', function(e) {
                    e.stopPropagation();
                });
            });

            // Initialize first tab as active
            var firstTab = document.getElementById('personalTab');
            if (firstTab) {
                firstTab.classList.add('active');
            }
            
            var firstButton = document.querySelector('.tab-button');
            if (firstButton) {
                firstButton.classList.add('active');
            }

            // Add form change listeners for auto-save
            var formFields = document.querySelectorAll('input, select, textarea');
            formFields.forEach(function(field) {
                field.addEventListener('change', autoSaveFormData);
                field.addEventListener('input', autoSaveFormData);
            });

            // Restore any auto-saved data
            restoreFormData();

            // Initialize search highlighting
            var searchBox = document.getElementById('<%= txtEmployeeSearch.ClientID %>');
            if (searchBox) {
                searchBox.addEventListener('input', function() {
                    setTimeout(highlightSearchTerm, 100);
                });
            }

            // Add keyboard shortcuts
            document.addEventListener('keydown', function(e) {
                // ESC key closes modals
                if (e.key === 'Escape') {
                    var visibleModals = document.querySelectorAll('.modal-overlay[style*="flex"]');
                    visibleModals.forEach(function(modal) {
                        modal.style.display = 'none';
                    });
                }
                
                // Ctrl+S saves form (prevent default save)
                if (e.ctrlKey && e.key === 's') {
                    e.preventDefault();
                    var saveButton = document.getElementById('<%= btnSaveProfile.ClientID %>');
                    if (saveButton && saveButton.style.display !== 'none') {
                        if (validateProfileForm()) {
                            saveButton.click();
                        }
                    }
                }
            });
        }

        // Document ready
        document.addEventListener('DOMContentLoaded', initializePage);

        // Handle postback events for UpdatePanel compatibility
        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            if (prm) {
                prm.add_endRequest(function (sender, e) {
                    // Re-initialize functionality after postback
                    initializePage();
                    
                    // Check if we need to show a modal after postback
                    var modalToShow = '<%= Session["ShowModal"] %>';
                    if (modalToShow === 'Employee') {
                        showModal();
                        '<% Session["ShowModal"] = null; %>';
                    } else if (modalToShow === 'Activity') {
                        showActivityModal();
                        '<% Session["ShowModal"] = null; %>';
                    }
                    
                    // Re-apply search highlighting
                    setTimeout(highlightSearchTerm, 200);
                });
                
                prm.add_beginRequest(function(sender, e) {
                    // Show loading indicator
                    var grid = document.getElementById('<%= gvEmployees.ClientID %>');
                    if (grid) {
                        grid.style.opacity = '0.7';
                        grid.style.pointerEvents = 'none';
                    }
                });
          }
      }

      // Global error handler
      window.addEventListener('error', function (e) {
          console.error('JavaScript Error:', e.error);
          // Don't show errors to users in production, just log them
      });

      // Performance monitoring
      window.addEventListener('load', function () {
          // Log page load time for performance monitoring
          if (window.performance && window.performance.timing) {
              var loadTime = window.performance.timing.loadEventEnd - window.performance.timing.navigationStart;
              console.log('Page load time:', loadTime + 'ms');
          }
      });

      // Accessibility enhancements
      function enhanceAccessibility() {
          // Add ARIA labels to action buttons
          var actionButtons = document.querySelectorAll('.btn-action');
          actionButtons.forEach(function (button) {
              var icon = button.querySelector('.material-icons');
              if (icon) {
                  var action = '';
                  switch (icon.textContent.trim()) {
                      case 'edit': action = 'Edit employee profile'; break;
                      case 'history': action = 'View employee activity'; break;
                      case 'pause': action = 'Deactivate employee'; break;
                      case 'play_arrow': action = 'Activate employee'; break;
                      case 'delete': action = 'Delete employee'; break;
                  }
                  if (action) {
                      button.setAttribute('aria-label', action);
                  }
              }
          });

          // Enhance modal accessibility
          var modals = document.querySelectorAll('.modal-overlay');
          modals.forEach(function (modal) {
              modal.setAttribute('role', 'dialog');
              modal.setAttribute('aria-modal', 'true');

              var title = modal.querySelector('.modal-title');
              if (title) {
                  var titleId = 'modal-title-' + Math.random().toString(36).substr(2, 9);
                  title.id = titleId;
                  modal.setAttribute('aria-labelledby', titleId);
              }
          });
      }

      // Call accessibility enhancements after page load
      document.addEventListener('DOMContentLoaded', function () {
          setTimeout(enhanceAccessibility, 100);
      });

    </script>
</asp:Content>