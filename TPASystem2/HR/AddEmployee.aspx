<%@ Page Title="Add New Employee" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="AddEmployee.aspx.cs" Inherits="TPASystem2.HR.AddEmployee" %>

<asp:Content ID="AddEmployeeContent" ContentPlaceHolderID="DashboardContent" runat="server">
    
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/employee-management.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    
    <!-- Custom Styles - Matching MyOnboarding Design -->
    <style>
        /* Employee Management Specific Styles - Matching MyOnboarding Design */
        .onboarding-header {
            background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
            color: white;
            padding: 2.5rem;
            border-radius: 16px;
            margin-bottom: 2rem;
            position: relative;
            overflow: hidden;
        }
        
        .onboarding-header::before {
            content: '';
            position: absolute;
            top: -50%;
            right: -10%;
            width: 60%;
            height: 200%;
            background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><circle cx="20" cy="20" r="2" fill="white" opacity="0.1"/><circle cx="80" cy="80" r="1.5" fill="white" opacity="0.1"/><circle cx="40" cy="60" r="1" fill="white" opacity="0.1"/><circle cx="60" cy="30" r="1.2" fill="white" opacity="0.1"/></svg>');
            opacity: 0.3;
        }
        
        .welcome-content {
            display: flex;
            justify-content: space-between;
            align-items: center;
            position: relative;
            z-index: 1;
        }
        
        .welcome-title {
            font-size: 2.5rem;
            font-weight: 600;
            margin-bottom: 0.5rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .welcome-title .material-icons {
            font-size: 2.5rem;
            color: #ffd54f;
        }
        
        .welcome-subtitle {
            font-size: 1.2rem;
            opacity: 0.9;
            margin: 0;
        }

        .header-actions {
            display: flex;
            gap: 1rem;
            align-items: center;
        }

        .header-actions .btn-tpa {
            background: rgba(255,255,255,0.2) !important;
            backdrop-filter: blur(10px) !important;
            border: 2px solid rgba(255,255,255,0.3) !important;
            color: white !important;
            font-weight: 600 !important;
            padding: 0.75rem 1.5rem !important;
            border-radius: 12px !important;
            box-shadow: 0 4px 16px rgba(0,0,0,0.1) !important;
            transition: all 0.3s ease !important;
        }

        .header-actions .btn-tpa:hover {
            background: rgba(255,255,255,0.3) !important;
            transform: translateY(-2px) !important;
            box-shadow: 0 6px 20px rgba(0,0,0,0.15) !important;
        }

        /* Form Container */
        .form-container {
            background: white;
            border-radius: 16px;
            padding: 2rem;
            box-shadow: 0 4px 16px rgba(0,0,0,0.1);
            margin-bottom: 2rem;
        }

        .form-section {
            margin-bottom: 2rem;
        }

        .form-section-title {
            font-size: 1.3rem;
            font-weight: 600;
            color: #1e293b;
            margin-bottom: 1.5rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
            padding-bottom: 0.5rem;
            border-bottom: 2px solid #e2e8f0;
        }

        .form-section-title .material-icons {
            color: var(--tpa-primary);
            font-size: 1.5rem;
        }

        .form-row {
            display: flex;
            gap: 1.5rem;
            margin-bottom: 1.5rem;
        }

        .form-group {
            flex: 1;
            min-width: 200px;
        }

        .form-group.full-width {
            flex: 100%;
        }

        .form-label {
            display: block;
            margin-bottom: 0.5rem;
            font-weight: 500;
            color: #1e293b;
            font-size: 0.9rem;
        }

        .form-control {
            width: 100%;
            padding: 0.75rem 1rem;
            border: 2px solid #e2e8f0;
            border-radius: 8px;
            font-size: 0.9rem;
            transition: all 0.3s ease;
            background: #fafafa;
        }

        .form-control:focus {
            outline: none;
            border-color: var(--tpa-primary);
            background: white;
            box-shadow: 0 0 0 3px rgba(255, 152, 0, 0.1);
        }

        .form-control:hover {
            border-color: #cbd5e1;
            background: white;
        }

        /* Alert Messages */
        .alert {
            padding: 1rem 1.5rem;
            border-radius: 12px;
            margin-bottom: 1.5rem;
            display: flex;
            align-items: flex-start;
            gap: 0.75rem;
            font-size: 0.9rem;
            line-height: 1.5;
        }
        
        .alert .material-icons {
            font-size: 1.2rem;
            margin-top: 0.1rem;
        }
        
        .alert-success {
            background: #ecfdf5;
            color: #065f46;
            border: 1px solid #d1fae5;
        }
        
        .alert-error {
            background: #fef2f2;
            color: #991b1b;
            border: 1px solid #fecaca;
        }

        .alert-warning {
            background: #fffbeb;
            color: #92400e;
            border: 1px solid #fed7aa;
        }

        /* Form Footer */
        .form-footer {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding-top: 2rem;
            border-top: 1px solid #e2e8f0;
            margin-top: 2rem;
        }

        .form-footer-left {
            color: #64748b;
            font-size: 0.9rem;
        }

        .form-footer-right {
            display: flex;
            gap: 1rem;
        }

        /* Button Styles */
        .btn {
            padding: 0.75rem 1.5rem;
            border-radius: 8px;
            font-weight: 500;
            text-decoration: none;
            cursor: pointer;
            transition: all 0.3s ease;
            border: none;
            font-size: 0.9rem;
        }

        .btn-tpa {
            background: linear-gradient(135deg, var(--tpa-primary) 0%, var(--tpa-primary-dark) 100%);
            color: white;
            box-shadow: 0 4px 12px rgba(255, 152, 0, 0.3);
        }

        .btn-tpa:hover {
            transform: translateY(-1px);
            box-shadow: 0 6px 16px rgba(255, 152, 0, 0.4);
        }

        .btn-outline {
            background: transparent;
            border: 2px solid var(--tpa-primary);
            color: var(--tpa-primary);
        }

        .btn-outline:hover {
            background: var(--tpa-primary);
            color: white;
        }

        .btn-secondary {
            background: #f8fafc;
            color: #64748b;
            border: 1px solid #e2e8f0;
        }

        .btn-secondary:hover {
            background: #e2e8f0;
            color: #475569;
        }

        /* Responsive Design */
        @media (max-width: 768px) {
            .welcome-content {
                flex-direction: column;
                gap: 1.5rem;
                text-align: center;
            }
            
            .welcome-title {
                font-size: 2rem;
            }
            
            .header-actions {
                flex-direction: column;
                width: 100%;
            }
            
            .form-row {
                flex-direction: column;
                gap: 0;
            }
            
            .form-footer {
                flex-direction: column;
                gap: 1rem;
            }
            
            .form-footer-right {
                width: 100%;
                justify-content: center;
            }
        }

        /* Required field indicator */
        .required::after {
            content: " *";
            color: #ef4444;
            font-weight: bold;
        }

        /* Checkbox styling */
        .form-check {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            margin: 0;
        }

        .form-check input[type="checkbox"] {
            width: auto;
            margin: 0;
        }

        .form-text {
            font-size: 0.8rem;
            color: #64748b;
            margin-top: 0.25rem;
        }

        /* Validation summary styling */
        .validation-summary {
            background: #fef2f2;
            border: 1px solid #fecaca;
            border-radius: 8px;
            padding: 1rem;
            margin-bottom: 1.5rem;
            color: #991b1b;
        }

        .validation-summary ul {
            margin: 0;
            padding-left: 1.5rem;
        }

        .validation-summary li {
            margin-bottom: 0.25rem;
        }
    </style>
    
    <!-- Welcome Header - Matching MyOnboarding Style -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">person_add</i>
                    Add New Employee
                </h1>
                <p class="welcome-subtitle">Create a new employee record and set up onboarding workflow</p>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                            CssClass="btn btn-outline" OnClick="btnCancel_Click" CausesValidation="false" />
            </div>
        </div>
    </div>
    
    <!-- Message Panel -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>
    
    <!-- Employee Form -->
    <div class="form-container">
        <!-- Personal Information Section -->
        <div class="form-section">
            <h3 class="form-section-title">
                <i class="material-icons">person</i>
                Personal Information
            </h3>
            
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label required">First Name</label>
                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" 
                                 placeholder="Enter first name" MaxLength="50" />
                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" 
                                                ControlToValidate="txtFirstName" 
                                                ErrorMessage="First name is required." 
                                                Display="Dynamic" CssClass="text-danger" />
                </div>
                
                <div class="form-group">
                    <label class="form-label required">Last Name</label>
                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" 
                                 placeholder="Enter last name" MaxLength="50" />
                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server" 
                                                ControlToValidate="txtLastName" 
                                                ErrorMessage="Last name is required." 
                                                Display="Dynamic" CssClass="text-danger" />
                </div>
            </div>
            
            
            
            
        </div>
        
        <!-- Contact Information Section -->
        <div class="form-section">
            <h3 class="form-section-title">
                <i class="material-icons">contact_phone</i>
                Contact Information
            </h3>
            
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label required">Email Address</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" 
                                 TextMode="Email" placeholder="employee@company.com" />
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                                                ControlToValidate="txtEmail" 
                                                ErrorMessage="Email address is required." 
                                                Display="Dynamic" CssClass="text-danger" />
                    <asp:RegularExpressionValidator ID="revEmail" runat="server" 
                                                    ControlToValidate="txtEmail" 
                                                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                                                    ErrorMessage="Please enter a valid email address." 
                                                    Display="Dynamic" CssClass="text-danger" />
                </div>
                
                <div class="form-group">
                    <label class="form-label">Phone Number</label>
                    <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control" 
                                 placeholder="(555) 123-4567" MaxLength="20" />
                </div>
            </div>
            
           
            
           
        </div>
        
        <!-- Employment Information Section -->
        <div class="form-section">
            <h3 class="form-section-title">
                <i class="material-icons">work</i>
                Employment Information
            </h3>
            
            
            
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label required">Department</label>
                    <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvDepartment" runat="server" 
                                                ControlToValidate="ddlDepartment" 
                                                ErrorMessage="Please select a department." 
                                                InitialValue="" Display="Dynamic" CssClass="text-danger" />
                </div>
                
                <div class="form-group">
                    <label class="form-label required">Position</label>
                    <asp:TextBox ID="txtPosition" runat="server" CssClass="form-control" 
                                 placeholder="Job title/position" MaxLength="100" />
                    <asp:RequiredFieldValidator ID="rfvPosition" runat="server" 
                                                ControlToValidate="txtPosition" 
                                                ErrorMessage="Position is required." 
                                                Display="Dynamic" CssClass="text-danger" />
                </div>
            </div>
            
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Manager</label>
                    <asp:DropDownList ID="ddlManager" runat="server" CssClass="form-control">
                    </asp:DropDownList>
                </div>
                
                <div class="form-group">
                    <label class="form-label required">Employee Type</label>
                    <asp:DropDownList ID="ddlEmployeeType" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Select Employee Type" Value="" />
                        <asp:ListItem Text="Full-Time" Value="Full-time" Selected="True" />
                        <asp:ListItem Text="Part-Time" Value="Part-time" />
                        <asp:ListItem Text="Contract" Value="Contract" />
                        <asp:ListItem Text="Temporary" Value="Temporary" />
                        <asp:ListItem Text="Intern" Value="Intern" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvEmployeeType" runat="server" 
                                                ControlToValidate="ddlEmployeeType" 
                                                ErrorMessage="Please select an employee type." 
                                                InitialValue="" Display="Dynamic" CssClass="text-danger" />
                </div>
            </div>
            
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Salary</label>
                    <asp:TextBox ID="txtSalary" runat="server" CssClass="form-control" 
                                 placeholder="Annual salary" TextMode="Number" step="0.01" />
                </div>
                
                <div class="form-group">
                    <label class="form-label">Work Location</label>
                    <asp:TextBox ID="txtWorkLocation" runat="server" CssClass="form-control" 
                                 placeholder="Office location" MaxLength="100" />
                </div>
            </div>
            
            
            
            
        </div>
        
        <!-- Security Information Section -->
        <div class="form-section">
            <h3 class="form-section-title">
                <i class="material-icons">security</i>
                Security & Access
            </h3>
            
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Temporary Password</label>
                    <asp:TextBox ID="txtTemporaryPassword" runat="server" CssClass="form-control" 
                                 placeholder="Auto-generated if left blank" MaxLength="50" TextMode="Password" />
                    <small class="form-text text-muted">Leave blank to auto-generate a secure password</small>
                </div>
                
                <div class="form-group">
                    <label class="form-label">Password Settings</label>
                    <div style="padding-top: 0.5rem;">
                        <asp:CheckBox ID="chkMustChangePassword" runat="server" 
                                      Text="Require password change on first login" 
                                      CssClass="form-check" Checked="true" />
                    </div>
                </div>
            </div>
        </div>
        
        <!-- Form Footer -->
        <div class="form-footer">
            <div class="form-footer-left">
                <i class="material-icons" style="font-size: 1rem; vertical-align: middle; margin-right: 0.25rem;">info</i>
                Fields marked with * are required
            </div>
            <div class="form-footer-right">
                
                <asp:Button ID="btnSaveAndNew" runat="server" Text="Save & Add Another" 
                            CssClass="btn btn-secondary" OnClick="btnSaveAndNew_Click" />
                <asp:Button ID="btnSaveEmployee" runat="server" Text="Create Employee" 
                            CssClass="btn btn-tpa" OnClick="btnSaveEmployee_Click" />
                <asp:Button ID="btnSaveEmployeeBottom" runat="server" Text="Create Employee" 
                            CssClass="btn btn-tpa" OnClick="btnSaveEmployeeBottom_Click" />
            </div>
        </div>
    </div>
    
    <!-- Validation Summary -->
    <asp:ValidationSummary ID="vsEmployee" runat="server" 
                           HeaderText="Please correct the following errors:" 
                           CssClass="validation-summary" 
                           DisplayMode="BulletList" />

</asp:Content>