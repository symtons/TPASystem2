<%@ Page Title="Employee Profile Management" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ManageEmployeeProfiles.aspx.cs" Inherits="TPASystem2.HR.ManageEmployeeProfiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">


    <style>



/* Profile Tabs Container */
.profile-tabs-container {
    background: white;
    border-radius: 16px;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
    border: 1px solid #e5e7eb;
    overflow: hidden;
    margin-bottom: 2rem;
}

.profile-tabs {
    display: flex;
    background: #f8fafc;
    border-bottom: 1px solid #e5e7eb;
    padding: 0;
    margin: 0;
    gap: 0;
}

.tab-button {
    background: transparent;
    border: none;
    padding: 1rem 2rem;
    font-size: 0.95rem;
    font-weight: 500;
    color: #64748b;
    cursor: pointer;
    transition: all 0.3s ease;
    position: relative;
    border-bottom: 3px solid transparent;
    min-width: 150px;
    text-align: center;
}

.tab-button:hover {
    background: #f1f5f9;
    color: #334155;
}

.tab-button.active {
    background: white;
    color: var(--tpa-primary);
    border-bottom-color: var(--tpa-primary);
    font-weight: 600;
}

.tab-button.active::after {
    content: '';
    position: absolute;
    bottom: -1px;
    left: 0;
    right: 0;
    height: 1px;
    background: white;
}

/* Tab Content */
.tab-content {
    display: none;
    padding: 2rem;
}

.tab-content.active {
    display: block;
}

/* Profile Section */
.profile-section {
    margin-bottom: 2rem;
}

.profile-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 1.5rem;
    margin-top: 1rem;
}

.form-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.form-group.full-width {
    grid-column: 1 / -1;
}

.form-group label {
    font-weight: 600;
    color: #374151;
    font-size: 0.9rem;
    display: flex;
    align-items: center;
    gap: 0.25rem;
}

.required {
    color: #ef4444;
    font-weight: bold;
}

.form-control {
    padding: 0.75rem 1rem;
    border: 2px solid #e5e7eb;
    border-radius: 8px;
    font-size: 0.9rem;
    transition: all 0.3s ease;
    background: white;
}

.form-control:focus {
    outline: none;
    border-color: var(--tpa-primary);
    box-shadow: 0 0 0 3px rgba(25, 118, 210, 0.1);
}

.form-control:disabled {
    background: #f9fafb;
    color: #6b7280;
    cursor: not-allowed;
}

/* Checkbox Wrapper */
.checkbox-wrapper {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    margin-top: 0.25rem;
}

.checkbox-wrapper input[type="checkbox"] {
    width: 18px;
    height: 18px;
    accent-color: var(--tpa-primary);
}

/* Filter Controls */
.filter-controls {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 1.5rem;
    margin-bottom: 2rem;
    padding: 1.5rem;
    background: white;
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    border: 1px solid #e5e7eb;
}

.filter-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.filter-label {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-weight: 500;
    font-size: 0.9rem;
    color: var(--text-secondary);
}

.filter-label .material-icons {
    font-size: 18px;
    color: var(--tpa-primary);
}

/* Section Container */
.section-container {
    background: white;
    border-radius: 16px;
    padding: 2rem;
    margin-bottom: 2rem;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
    border: 1px solid #e5e7eb;
}

.section-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    margin-bottom: 1.5rem;
    flex-wrap: wrap;
    gap: 1rem;
}

.section-header h2 {
    margin: 0 0 0.5rem 0;
    font-size: 1.5rem;
    font-weight: 600;
    color: #1f2937;
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.section-header h2 .material-icons {
    font-size: 1.75rem;
    color: var(--tpa-primary);
}

.section-header p {
    margin: 0;
    color: #6b7280;
    font-size: 0.95rem;
}

.section-actions {
    display: flex;
    gap: 0.75rem;
    flex-wrap: wrap;
}

/* Recent Activity Styles */
.activity-list {
    display: flex;
    flex-direction: column;
    gap: 1rem;
}

.activity-item {
    display: flex;
    gap: 1rem;
    padding: 1rem;
    background: #f8fafc;
    border-radius: 8px;
    border: 1px solid #e5e7eb;
    transition: all 0.3s ease;
}

.activity-item:hover {
    background: #f1f5f9;
    border-color: #d1d5db;
}

.activity-icon {
    width: 40px;
    height: 40px;
    border-radius: 50%;
    background: var(--tpa-primary);
    color: white;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
}

.activity-icon .material-icons {
    font-size: 20px;
}

.activity-content {
    flex: 1;
}

.activity-title {
    font-weight: 600;
    color: #1f2937;
    margin-bottom: 0.25rem;
}

.activity-description {
    color: #4b5563;
    font-size: 0.9rem;
    margin-bottom: 0.25rem;
}

.activity-meta {
    color: #6b7280;
    font-size: 0.8rem;
}

/* No Data Panel */
.no-data-panel {
    text-align: center;
    padding: 3rem 2rem;
    background: #f9fafb;
    border-radius: 12px;
    border: 2px dashed #d1d5db;
}

.no-data-content {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 1rem;
}

.no-data-content .material-icons {
    font-size: 3rem;
    color: #9ca3af;
}

.no-data-content h3 {
    margin: 0;
    color: #4b5563;
    font-weight: 600;
}

.no-data-content p {
    margin: 0;
    color: #6b7280;
    max-width: 400px;
}

/* Button Styles Enhancement */
.btn {
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.75rem 1.5rem;
    border: none;
    border-radius: 8px;
    font-size: 0.9rem;
    font-weight: 500;
    text-decoration: none;
    cursor: pointer;
    transition: all 0.3s ease;
    min-height: 44px;
    justify-content: center;
}

/* Add Material Icons to buttons using CSS */
#btnAddEmployee::before {
    content: "person_add";
    font-family: "Material Icons";
    font-size: 18px;
}

#btnExportProfiles::before {
    content: "download";
    font-family: "Material Icons";
    font-size: 18px;
}

#btnRefreshList::before {
    content: "refresh";
    font-family: "Material Icons";
    font-size: 18px;
}

#btnSaveProfile::before {
    content: "save";
    font-family: "Material Icons";
    font-size: 18px;
}

#btnCancelEdit::before {
    content: "cancel";
    font-family: "Material Icons";
    font-size: 18px;
}

#btnDeleteEmployee::before {
    content: "person_off";
    font-family: "Material Icons";
    font-size: 18px;
}

.btn-primary {
    background: linear-gradient(135deg, var(--tpa-primary), var(--tpa-primary-dark));
    color: white;
    box-shadow: 0 2px 8px rgba(25, 118, 210, 0.3);
}

.btn-primary:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(25, 118, 210, 0.4);
}

.btn-outline {
    background: transparent;
    color: var(--tpa-primary);
    border: 2px solid var(--tpa-primary);
}

.btn-outline:hover {
    background: var(--tpa-primary);
    color: white;
}

.btn-outline-light {
    background: transparent;
    color: white;
    border: 2px solid rgba(255, 255, 255, 0.3);
}

.btn-outline-light:hover {
    background: rgba(255, 255, 255, 0.1);
    border-color: white;
}

.btn-danger {
    background: linear-gradient(135deg, #ef4444, #dc2626);
    color: white;
    box-shadow: 0 2px 8px rgba(239, 68, 68, 0.3);
}

.btn-danger:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(239, 68, 68, 0.4);
}

.btn:disabled {
    opacity: 0.6;
    cursor: not-allowed;
    transform: none !important;
}

/* Message Panel Enhancements */
.message-panel {
    margin-bottom: 2rem;
    border-radius: 8px;
    overflow: hidden;
}

.message-success {
    background: #dcfce7;
    color: #166534;
    padding: 1rem 1.5rem;
    display: flex;
    align-items: center;
    gap: 0.75rem;
    border: 1px solid #bbf7d0;
}

.message-error {
    background: #fef2f2;
    color: #dc2626;
    padding: 1rem 1.5rem;
    display: flex;
    align-items: center;
    gap: 0.75rem;
    border: 1px solid #fecaca;
}

.message-warning {
    background: #fefce8;
    color: #ca8a04;
    padding: 1rem 1.5rem;
    display: flex;
    align-items: center;
    gap: 0.75rem;
    border: 1px solid #fef3c7;
}

.message-info {
    background: #eff6ff;
    color: #2563eb;
    padding: 1rem 1.5rem;
    display: flex;
    align-items: center;
    gap: 0.75rem;
    border: 1px solid #dbeafe;
}

.message-success .material-icons,
.message-error .material-icons,
.message-warning .material-icons,
.message-info .material-icons {
    font-size: 20px;
}

/* Responsive Design */
@media (max-width: 768px) {
    .profile-grid {
        grid-template-columns: 1fr;
    }
    
    .filter-controls {
        grid-template-columns: 1fr;
    }
    
    .profile-tabs {
        flex-direction: column;
    }
    
    .tab-button {
        min-width: auto;
        text-align: left;
        border-bottom: none;
        border-left: 3px solid transparent;
    }
    
    .tab-button.active {
        border-left-color: var(--tpa-primary);
        border-bottom: none;
    }
    
    .section-header {
        flex-direction: column;
        align-items: stretch;
    }
    
    .section-actions {
        justify-content: stretch;
    }
    
    .section-actions .btn {
        flex: 1;
    }
    
    .activity-item {
        flex-direction: column;
        text-align: center;
    }
    
    .activity-icon {
        align-self: center;
    }
}

@media (max-width: 480px) {
    .section-container {
        padding: 1rem;
    }
    
    .tab-content {
        padding: 1rem;
    }
    
    .overview-cards {
        grid-template-columns: 1fr;
    }
    
    .profile-tabs-container {
        border-radius: 8px;
    }
}
    </style>
    <!-- Welcome Header - Matching LeaveManagement Style -->
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
                            <asp:Literal ID="litManagerName" runat="server" Text="Manager"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">business</i>
                        <span>
                            <asp:Literal ID="litDepartment" runat="server" Text="Department"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">group</i>
                        <span>
                            <asp:Literal ID="litEmployeeCount" runat="server" Text="0"></asp:Literal> Employees
                        </span>
                    </div>
                </div>
            </div>
            <div class="action-buttons">
                <asp:Button ID="btnAddEmployee" runat="server" Text="Add Employee" 
                    CssClass="btn btn-primary" OnClick="btnAddEmployee_Click" Visible="true" />
                <asp:Button ID="btnExportProfiles" runat="server" Text="Export" 
                    CssClass="btn btn-outline-light" OnClick="btnExportProfiles_Click" />
            </div>
        </div>
    </div>

    <!-- Message Panel -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="message-panel">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Dashboard Overview Section - Matching LeaveManagement Style -->
    <div class="dashboard-overview">
        <div class="section-header">
            <div>
                <h2>
                    <i class="material-icons">analytics</i>
                    Employee Overview
                </h2>
                <p>Quick statistics and management controls</p>
            </div>
        </div>

        <div class="overview-cards">
            <div class="overview-card active">
                <div class="card-icon">
                    <i class="material-icons">people</i>
                </div>
                <div class="card-content">
                    <div class="card-number">
                        <asp:Literal ID="litTotalEmployees" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="card-label">Total Employees</div>
                    <div class="card-subtitle">
                        <asp:Literal ID="litActiveEmployees" runat="server" Text="0"></asp:Literal> Active
                    </div>
                </div>
            </div>

            <div class="overview-card pending">
                <div class="card-icon">
                    <i class="material-icons">person_add</i>
                </div>
                <div class="card-content">
                    <div class="card-number">
                        <asp:Literal ID="litNewHires" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="card-label">New Hires (30 Days)</div>
                    <div class="card-subtitle">Recent additions</div>
                </div>
            </div>

            <div class="overview-card approved">
                <div class="card-icon">
                    <i class="material-icons">assignment_turned_in</i>
                </div>
                <div class="card-content">
                    <div class="card-number">
                        <asp:Literal ID="litCompletedProfiles" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="card-label">Completed Profiles</div>
                    <div class="card-subtitle">Full information</div>
                </div>
            </div>

            <div class="overview-card completed">
                <div class="card-icon">
                    <i class="material-icons">business</i>
                </div>
                <div class="card-content">
                    <div class="card-number">
                        <asp:Literal ID="litDepartmentCount" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="card-label">Departments</div>
                    <div class="card-subtitle">Active departments</div>
                </div>
            </div>
        </div>
    </div>

    <!-- Employee Selection and Filters Section -->
    <div class="section-container">
        <div class="section-header">
            <div>
                <h2>
                    <i class="material-icons">search</i>
                    Select Employee
                </h2>
                <p>Choose an employee to view or edit their profile</p>
            </div>
            <div class="section-actions">
                <asp:Button ID="btnRefreshList" runat="server" Text="Refresh" 
                    CssClass="btn btn-outline" OnClick="btnRefreshList_Click" />
            </div>
        </div>

        <div class="filter-controls">
            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">search</i>
                    Search Employee
                </label>
                <asp:TextBox ID="txtEmployeeSearch" runat="server" CssClass="form-control" 
                    placeholder="Search by name, email, or employee number..." 
                    AutoPostBack="true" OnTextChanged="txtEmployeeSearch_TextChanged"></asp:TextBox>
            </div>

            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">business</i>
                    Department
                </label>
                <asp:DropDownList ID="ddlDepartmentFilter" runat="server" CssClass="form-control" 
                    AutoPostBack="true" OnSelectedIndexChanged="ddlDepartmentFilter_SelectedIndexChanged">
                    <asp:ListItem Value="" Text="All Departments"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">person</i>
                    Status
                </label>
                <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="form-control" 
                    AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                    <asp:ListItem Value="" Text="All Statuses"></asp:ListItem>
                    <asp:ListItem Value="Active" Text="Active"></asp:ListItem>
                    <asp:ListItem Value="Inactive" Text="Inactive"></asp:ListItem>
                    <asp:ListItem Value="Terminated" Text="Terminated"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="filter-group">
                <label class="filter-label">
                    <i class="material-icons">person</i>
                    Select Employee
                </label>
                <asp:DropDownList ID="ddlEmployeeSelect" runat="server" CssClass="form-control" 
                    AutoPostBack="true" OnSelectedIndexChanged="ddlEmployeeSelect_SelectedIndexChanged">
                    <asp:ListItem Value="" Text="Choose an employee..."></asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
    </div>

    <!-- Employee Profile Editor Section -->
    <asp:Panel ID="pnlEmployeeProfile" runat="server" Visible="false" CssClass="section-container">
        <div class="section-header">
            <div>
                <h2>
                    <i class="material-icons">person</i>
                    Employee Profile
                </h2>
                <p>
                    <asp:Literal ID="litSelectedEmployee" runat="server" Text="Selected Employee"></asp:Literal>
                </p>
            </div>
            <div class="section-actions">
                <asp:Button ID="btnSaveProfile" runat="server" Text="Save Changes" 
                    CssClass="btn btn-primary" OnClick="btnSaveProfile_Click" />
                <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" 
                    CssClass="btn btn-outline" OnClick="btnCancelEdit_Click" />
                <asp:Button ID="btnDeleteEmployee" runat="server" Text="Deactivate" 
                    CssClass="btn btn-danger" OnClick="btnDeleteEmployee_Click" 
                    OnClientClick="return confirm('Are you sure you want to deactivate this employee?');" />
            </div>
        </div>

        <!-- Profile Tabs -->
        <div class="profile-tabs-container">
            <div class="profile-tabs">
                <asp:Button ID="btnTabPersonal" runat="server" Text="Personal Information" 
                    CssClass="tab-button active" OnClick="btnTabPersonal_Click" />
                <asp:Button ID="btnTabEmployment" runat="server" Text="Employment Details" 
                    CssClass="tab-button" OnClick="btnTabEmployment_Click" />
                <asp:Button ID="btnTabContact" runat="server" Text="Contact Information" 
                    CssClass="tab-button" OnClick="btnTabContact_Click" />
                <asp:Button ID="btnTabSystemAccess" runat="server" Text="System Access" 
                    CssClass="tab-button" OnClick="btnTabSystemAccess_Click" />
            </div>

            <!-- Personal Information Tab -->
            <asp:Panel ID="pnlPersonalInfo" runat="server" CssClass="tab-content active">
                <div class="profile-section">
                    <div class="profile-grid">
                        <div class="form-group">
                            <label for="txtFirstName">First Name <span class="required">*</span></label>
                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" 
                                placeholder="Enter first name" Required="true"></asp:TextBox>
                        </div>
                        
                        <div class="form-group">
                            <label for="txtLastName">Last Name <span class="required">*</span></label>
                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" 
                                placeholder="Enter last name" Required="true"></asp:TextBox>
                        </div>
                        
                        <div class="form-group">
                            <label for="txtEmployeeNumber">Employee Number <span class="required">*</span></label>
                            <asp:TextBox ID="txtEmployeeNumber" runat="server" CssClass="form-control" 
                                placeholder="Enter employee number" Required="true"></asp:TextBox>
                        </div>
                        
                        <div class="form-group">
                            <label for="txtDateOfBirth">Date of Birth</label>
                            <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control" 
                                TextMode="Date" placeholder="Select date of birth"></asp:TextBox>
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
            </asp:Panel>

            <!-- Employment Details Tab -->
            <asp:Panel ID="pnlEmploymentInfo" runat="server" CssClass="tab-content">
                <div class="profile-section">
                    <div class="profile-grid">
                        <div class="form-group">
                            <label for="txtJobTitle">Job Title <span class="required">*</span></label>
                            <asp:TextBox ID="txtJobTitle" runat="server" CssClass="form-control" 
                                placeholder="Enter job title" Required="true"></asp:TextBox>
                        </div>
                        
                        <div class="form-group">
                            <label for="ddlDepartment">Department <span class="required">*</span></label>
                            <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-control" Required="true">
                                <asp:ListItem Value="" Text="Select Department"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        
                        <div class="form-group">
                            <label for="ddlEmployeeType">Employee Type</label>
                            <asp:DropDownList ID="ddlEmployeeType" runat="server" CssClass="form-control">
                                <asp:ListItem Value="" Text="Select Employee Type"></asp:ListItem>
                                <asp:ListItem Value="Full-Time" Text="Full-Time"></asp:ListItem>
                                <asp:ListItem Value="Part-Time" Text="Part-Time"></asp:ListItem>
                                <asp:ListItem Value="Contract" Text="Contract"></asp:ListItem>
                                <asp:ListItem Value="Temporary" Text="Temporary"></asp:ListItem>
                                <asp:ListItem Value="Intern" Text="Intern"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        
                        <div class="form-group">
                            <label for="txtHireDate">Hire Date <span class="required">*</span></label>
                            <asp:TextBox ID="txtHireDate" runat="server" CssClass="form-control" 
                                TextMode="Date" Required="true"></asp:TextBox>
                        </div>
                        
                        <div class="form-group">
                            <label for="ddlManager">Manager</label>
                            <asp:DropDownList ID="ddlManager" runat="server" CssClass="form-control">
                                <asp:ListItem Value="" Text="Select Manager"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        
                        <div class="form-group">
                            <label for="ddlEmployeeStatus">Status</label>
                            <asp:DropDownList ID="ddlEmployeeStatus" runat="server" CssClass="form-control">
                                <asp:ListItem Value="Active" Text="Active"></asp:ListItem>
                                <asp:ListItem Value="Inactive" Text="Inactive"></asp:ListItem>
                                <asp:ListItem Value="Terminated" Text="Terminated"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        
                        <div class="form-group">
                            <label for="txtSalary">Salary</label>
                            <asp:TextBox ID="txtSalary" runat="server" CssClass="form-control" 
                                placeholder="Enter salary" TextMode="Number" step="0.01"></asp:TextBox>
                        </div>
                        
                        <div class="form-group">
                            <label for="txtWorkLocation">Work Location</label>
                            <asp:TextBox ID="txtWorkLocation" runat="server" CssClass="form-control" 
                                placeholder="Enter work location"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <!-- Contact Information Tab -->
            <asp:Panel ID="pnlContactInfo" runat="server" CssClass="tab-content">
                <div class="profile-section">
                    <div class="profile-grid">
                        <div class="form-group">
                            <label for="txtEmail">Email Address <span class="required">*</span></label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" 
                                TextMode="Email" placeholder="Enter email address" Required="true"></asp:TextBox>
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
            </asp:Panel>

            <!-- System Access Tab -->
            <asp:Panel ID="pnlSystemAccess" runat="server" CssClass="tab-content">
                <div class="profile-section">
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
                        
                        <div class="form-group full-width">
                            <label for="txtSystemNotes">System Notes</label>
                            <asp:TextBox ID="txtSystemNotes" runat="server" CssClass="form-control" 
                                TextMode="MultiLine" Rows="4" 
                                placeholder="Internal notes about system access, permissions, etc."></asp:TextBox>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </asp:Panel>

    <!-- Recent Activity Section -->
    <asp:Panel ID="pnlRecentActivity" runat="server" Visible="false" CssClass="section-container">
        <div class="section-header">
            <div>
                <h2>
                    <i class="material-icons">history</i>
                    Recent Profile Changes
                </h2>
                <p>Audit trail of recent modifications</p>
            </div>
        </div>

        <div class="activity-list">
            <asp:Repeater ID="rptRecentActivity" runat="server">
                <ItemTemplate>
                    <div class="activity-item">
                        <div class="activity-icon">
                            <i class="material-icons">edit</i>
                        </div>
                        <div class="activity-content">
                            <div class="activity-title"><%# Eval("FieldName") %> Updated</div>
                            <div class="activity-description">
                                Changed from "<%# Eval("OldValue") %>" to "<%# Eval("NewValue") %>"
                            </div>
                            <div class="activity-meta">
                                By <%# Eval("ChangedByName") %> on <%# Eval("ChangedDate", "{0:MMM dd, yyyy at h:mm tt}") %>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            
            <asp:Panel ID="pnlNoActivity" runat="server" Visible="false" CssClass="no-data-panel">
                <div class="no-data-content">
                    <i class="material-icons">history</i>
                    <h3>No Recent Activity</h3>
                    <p>No recent changes have been made to this employee's profile.</p>
                </div>
            </asp:Panel>
        </div>
    </asp:Panel>

</asp:Content>