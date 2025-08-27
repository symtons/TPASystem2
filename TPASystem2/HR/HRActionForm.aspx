<%@ Page Title="Human Resources Action Form" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="HRActionForm.aspx.cs" Inherits="TPASystem2.HR.HRActionForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    <style>
        /* ===============================================
   HR ACTION FORM STYLES
   Add these styles to tpa-common.css
   =============================================== */

/* Form Container */
.form-container {
    background: white;
    border-radius: 16px;
    padding: 2.5rem;
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
    margin-bottom: 2rem;
    border: 1px solid #e5e7eb;
}

/* Form Sections */
.form-section {
    margin-bottom: 3rem;
    padding-bottom: 2rem;
    border-bottom: 1px solid #f0f0f0;
}

.form-section:last-child {
    border-bottom: none;
    margin-bottom: 0;
    padding-bottom: 0;
}

/* Section Headers */
.section-header {
    display: flex;
    align-items: center;
    gap: 1rem;
    margin-bottom: 2rem;
    padding-bottom: 1rem;
    border-bottom: 2px solid #e5e7eb;
    background: linear-gradient(90deg, #f8fafc 0%, #fff 100%);
    padding: 1.5rem;
    border-radius: 12px;
    margin: -0.5rem -0.5rem 2rem -0.5rem;
}

.section-header h4 {
    margin: 0;
    color: #1e293b;
    font-size: 1.3rem;
    font-weight: 600;
    letter-spacing: -0.02em;
}

.section-header .material-icons {
    color: #1976d2;
    font-size: 1.75rem;
    background: rgba(25, 118, 210, 0.1);
    padding: 0.5rem;
    border-radius: 50%;
}

/* Form Grid Layout */
.form-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 2rem;
    margin-bottom: 2rem;
    align-items: start;
}

.form-grid:last-child {
    margin-bottom: 0;
}

/* Form Groups */
.form-group {
    display: flex;
    flex-direction: column;
    gap: 0.75rem;
}

.form-group.full-width {
    grid-column: 1 / -1;
}

/* Form Labels */
.form-label {
    display: block;
    font-weight: 600;
    color: #374151;
    font-size: 0.95rem;
    letter-spacing: -0.01em;
    margin-bottom: 0.5rem;
}

.form-label.required::after {
    content: ' *';
    color: #ef4444;
    font-weight: bold;
}

/* Form Inputs */
.form-input {
    width: 100%;
    padding: 1rem 1.25rem;
    border: 2px solid #e5e7eb;
    border-radius: 12px;
    font-size: 1rem;
    font-weight: 500;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    background: #fafbfc;
    color: #1e293b;
    line-height: 1.5;
}

.form-input:focus {
    outline: none;
    border-color: #1976d2;
    box-shadow: 0 0 0 4px rgba(25, 118, 210, 0.1);
    background: white;
    transform: translateY(-1px);
}

.form-input:hover:not(:focus) {
    border-color: #cbd5e1;
    background: white;
}

.form-input::placeholder {
    color: #9ca3af;
    font-weight: 400;
}

/* Dropdown styling */
.form-input[type="select"], 
select.form-input {
    background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3e%3cpath stroke='%236b7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='M6 8l4 4 4-4'/%3e%3c/svg%3e");
    background-position: right 0.75rem center;
    background-repeat: no-repeat;
    background-size: 1.5em 1.5em;
    padding-right: 3rem;
    cursor: pointer;
}

/* Textarea styling */
textarea.form-input {
    resize: vertical;
    min-height: 100px;
    font-family: inherit;
}

/* Checkbox Groups */
.checkbox-group {
    display: flex;
    flex-wrap: wrap;
    gap: 1.5rem;
    align-items: flex-start;
    padding: 1rem;
    background: #f8fafc;
    border-radius: 12px;
    border: 2px solid #e5e7eb;
    transition: all 0.3s ease;
}

.checkbox-group:hover {
    border-color: #cbd5e1;
}

/* Form Check (Individual Checkbox/Radio) */
.form-check {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    padding: 0.5rem;
    border-radius: 8px;
    transition: all 0.2s ease;
    cursor: pointer;
    user-select: none;
}

.form-check:hover {
    background: rgba(25, 118, 210, 0.05);
}

.form-check input[type="checkbox"],
.form-check input[type="radio"] {
    width: 18px;
    height: 18px;
    margin: 0;
    cursor: pointer;
    accent-color: #1976d2;
    transform: scale(1.2);
}

.form-check label {
    margin: 0;
    cursor: pointer;
    font-weight: 500;
    color: #374151;
    user-select: none;
}

/* Checkbox group items when arranged vertically */
.checkbox-group[style*="column"] {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
}

/* Alert Messages */
.alert {
    padding: 1.25rem 1.5rem;
    border-radius: 12px;
    margin-bottom: 2rem;
    display: flex;
    align-items: flex-start;
    gap: 1rem;
    font-size: 0.95rem;
    line-height: 1.6;
    font-weight: 500;
    border: 1px solid;
}

.alert .material-icons {
    font-size: 1.25rem;
    margin-top: 0.125rem;
    flex-shrink: 0;
}

.alert-success {
    background: linear-gradient(135deg, #ecfdf5 0%, #f0fdf4 100%);
    color: #065f46;
    border-color: #10b981;
}

.alert-success .material-icons {
    color: #10b981;
}

.alert-error {
    background: linear-gradient(135deg, #fef2f2 0%, #fef7f7 100%);
    color: #991b1b;
    border-color: #ef4444;
}

.alert-error .material-icons {
    color: #ef4444;
}

/* Form Footer */
.form-footer {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 2rem 0 0;
    margin-top: 3rem;
    border-top: 2px solid #e5e7eb;
    background: linear-gradient(90deg, #fafbfc 0%, #fff 100%);
    padding: 2rem;
    border-radius: 12px;
    margin-left: -0.5rem;
    margin-right: -0.5rem;
}

.form-footer-left {
    color: #64748b;
    font-size: 0.9rem;
    font-weight: 500;
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.form-footer-left .required {
    color: #ef4444;
    font-weight: bold;
}

.form-footer-right {
    display: flex;
    gap: 1rem;
}

/* Button Styles */
.btn {
    padding: 1rem 2rem;
    border-radius: 12px;
    font-weight: 600;
    font-size: 0.95rem;
    text-decoration: none;
    cursor: pointer;
    transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    border: 2px solid transparent;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    gap: 0.5rem;
    letter-spacing: -0.01em;
    min-width: 140px;
    user-select: none;
}

.btn:focus {
    outline: none;
    box-shadow: 0 0 0 4px rgba(25, 118, 210, 0.2);
}

.btn-tpa {
    background: linear-gradient(135deg, #1976d2 0%, #1565c0 100%);
    color: white;
    box-shadow: 0 4px 16px rgba(25, 118, 210, 0.3);
}

.btn-tpa:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 24px rgba(25, 118, 210, 0.4);
    background: linear-gradient(135deg, #1565c0 0%, #0d47a1 100%);
}

.btn-tpa:active {
    transform: translateY(-1px);
}

.btn-outline {
    background: white;
    color: #1976d2;
    border: 2px solid #1976d2;
    box-shadow: 0 2px 8px rgba(25, 118, 210, 0.1);
}

.btn-outline:hover {
    background: #1976d2;
    color: white;
    transform: translateY(-2px);
    box-shadow: 0 8px 24px rgba(25, 118, 210, 0.3);
}

.btn-outline-light {
    background: rgba(255, 255, 255, 0.2);
    backdrop-filter: blur(10px);
    border: 2px solid rgba(255, 255, 255, 0.3);
    color: white;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
}

.btn-outline-light:hover {
    background: rgba(255, 255, 255, 0.3);
    transform: translateY(-2px);
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
}

/* Responsive Design */
@media (max-width: 768px) {
    .form-container {
        padding: 1.5rem;
        margin: 1rem;
        border-radius: 12px;
    }
    
    .form-grid {
        grid-template-columns: 1fr;
        gap: 1.5rem;
    }
    
    .section-header {
        flex-direction: column;
        text-align: center;
        gap: 1rem;
    }
    
    .section-header h4 {
        font-size: 1.1rem;
    }
    
    .checkbox-group {
        flex-direction: column;
        align-items: flex-start;
        gap: 1rem;
    }
    
    .form-footer {
        flex-direction: column;
        gap: 1.5rem;
        text-align: center;
    }
    
    .form-footer-right {
        flex-direction: column;
        width: 100%;
    }
    
    .btn {
        width: 100%;
        justify-content: center;
    }
}

@media (max-width: 480px) {
    .form-container {
        padding: 1rem;
        margin: 0.5rem;
    }
    
    .form-input {
        padding: 0.875rem 1rem;
        font-size: 0.95rem;
    }
    
    .section-header {
        padding: 1rem;
        margin: -0.25rem -0.25rem 1.5rem -0.25rem;
    }
    
    .section-header .material-icons {
        font-size: 1.5rem;
    }
    
    .btn {
        padding: 0.875rem 1.5rem;
        font-size: 0.9rem;
    }
}

/* Dynamic Section Styling (for JavaScript controlled sections) */
div[id*="Section"] {
    margin-top: 1.5rem;
    padding: 1.5rem;
    background: linear-gradient(135deg, #f8fafc 0%, #fff 100%);
    border: 2px solid #e5e7eb;
    border-radius: 12px;
    transition: all 0.3s ease;
}

div[id*="Section"]:hover {
    border-color: #cbd5e1;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
}

/* Form validation styling */
.form-input:invalid {
    border-color: #ef4444;
    box-shadow: 0 0 0 2px rgba(239, 68, 68, 0.1);
}

.form-input:valid {
    border-color: #10b981;
}

/* Loading state for buttons */
.btn:disabled {
    opacity: 0.6;
    cursor: not-allowed;
    transform: none;
}

.btn:disabled:hover {
    transform: none;
    box-shadow: none;
}

/* Enhanced focus states for accessibility */
.form-input:focus,
.form-check input:focus,
.btn:focus {
    outline: 2px solid #1976d2;
    outline-offset: 2px;
}
 .onboarding-header {
            background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
            color: white;
            padding: 3rem 2.5rem;
            border-radius: 16px;
            margin-bottom: 2rem;
            position: relative;
            overflow: hidden;
            box-shadow: 0 8px 32px rgba(25, 118, 210, 0.3);
        }
        
        .onboarding-header::before {
            content: '';
            position: absolute;
            top: -50%;
            right: -10%;
            width: 60%;
            height: 200%;
            background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><circle cx="20" cy="20" r="2" fill="white" opacity="0.1"/><circle cx="80" cy="80" r="1.5" fill="white" opacity="0.1"/><circle cx="40" cy="60" r="1" fill="white" opacity="0.1"/><circle cx="60" cy="30" r="1.2" fill="white" opacity="0.1"/></svg>');
            opacity: 0.4;
        }
        
        .welcome-content {
            display: flex;
            justify-content: space-between;
            align-items: center;
            position: relative;
            z-index: 1;
        }
        
        .welcome-title {
            font-size: 2.75rem;
            font-weight: 700;
            margin-bottom: 1rem;
            display: flex;
            align-items: center;
            gap: 1rem;
            letter-spacing: -0.02em;
        }
        
        .welcome-title .material-icons {
            font-size: 3rem;
            color: #ffd54f;
            filter: drop-shadow(0 2px 8px rgba(0,0,0,0.2));
        }
        
        .welcome-subtitle {
            font-size: 1.25rem;
            opacity: 0.9;
            margin-bottom: 1.5rem;
            font-weight: 400;
        }
        
        .employee-info {
            display: flex;
            gap: 2rem;
            flex-wrap: wrap;
        }
        
        .employee-detail {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            background: rgba(255, 255, 255, 0.15);
            padding: 0.75rem 1rem;
            border-radius: 12px;
            backdrop-filter: blur(10px);
            font-weight: 500;
        }
        
        .employee-detail .material-icons {
            font-size: 1.25rem;
            opacity: 0.9;
        }
        
        .header-actions {
            display: flex;
            gap: 1rem;
            align-items: center;
        }
        
        @media (max-width: 768px) {
            .welcome-content {
                flex-direction: column;
                gap: 2rem;
                text-align: center;
            }
            
            .welcome-title {
                font-size: 2rem;
            }
            
            .employee-info {
                justify-content: center;
                gap: 1rem;
            }
            
            .header-actions {
                width: 100%;
                justify-content: center;
            }
        }
/* Print styles */
@media print {
    .form-container {
        box-shadow: none;
        border: 1px solid #ccc;
        page-break-inside: avoid;
    }
    
    .btn {
        display: none;
    }
    
    .alert {
        background: transparent !important;
        border: 1px solid #ccc;
    }
}
    </style>
    <!-- Welcome Header - Matching EmployeeLeavePortal Style -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">assignment</i>
                    Human Resources Action Form
                </h1>
                <p class="welcome-subtitle">Submit employee information changes and HR requests for approval</p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">person</i>
                        <span>
                            <asp:Literal ID="litCurrentUser" runat="server" Text="Current User"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">badge</i>
                        <span>
                            <asp:Literal ID="litUserNumber" runat="server" Text="EMP001"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">supervisor_account</i>
                        <span>Requires HR Admin Approval</span>
                    </div>
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnViewExistingRequests" runat="server" Text="View My Requests" 
                    CssClass="btn btn-outline-light" OnClick="btnViewExistingRequests_Click" />
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

    <!-- Form Container -->
    <div class="form-container">
        <!-- Personal Data Section -->
        <div class="form-section">
            <div class="section-header">
                <i class="material-icons">person</i>
                <h4>Personal Data</h4>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Employee</label>
                    <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="form-input" AutoPostBack="true" OnSelectedIndexChanged="ddlEmployee_SelectedIndexChanged">
                        <asp:ListItem Text="Select Employee..." Value=""></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="form-group">
                    <label class="form-label">Employee Name</label>
                    <asp:TextBox ID="txtEmployeeName" runat="server" CssClass="form-input" placeholder="Full employee name"></asp:TextBox>
                </div>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Social Security #</label>
                    <asp:TextBox ID="txtSSN" runat="server" CssClass="form-input" placeholder="XXX-XX-XXXX"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label class="form-label">Classification</label>
                    <div class="checkbox-group">
                        <asp:CheckBox ID="chkFullTime" runat="server" Text="Full Time" />
                        <asp:CheckBox ID="chkPartTime" runat="server" Text="Part Time" />
                        <asp:CheckBox ID="chkPRN" runat="server" Text="PRN" />
                    </div>
                </div>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Shift</label>
                    <asp:TextBox ID="txtShift" runat="server" CssClass="form-input" placeholder="Shift details"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label class="form-label">Total Hours</label>
                    <asp:TextBox ID="txtTotalHours" runat="server" CssClass="form-input" placeholder="40.00" TextMode="Number" step="0.25"></asp:TextBox>
                </div>
            </div>
        </div>

        <!-- Rate Change Section -->
        <div class="form-section">
            <div class="section-header">
                <i class="material-icons">attach_money</i>
                <h4>Rate Change</h4>
            </div>
            
            <div class="form-group">
                <asp:CheckBox ID="chkRateChange" runat="server" Text="This is a rate change" CssClass="form-check" />
            </div>
            
            <div id="rateChangeSection" style="display: none;">
                <div class="form-grid">
                    <div class="form-group">
                        <label class="form-label">Previous Rate/Salary</label>
                        <asp:TextBox ID="txtPreviousRate" runat="server" CssClass="form-input" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Rate Type</label>
                        <div class="checkbox-group">
                            <asp:RadioButton ID="rbSalary" runat="server" Text="Salary" GroupName="RateType" />
                            <asp:RadioButton ID="rbHourly" runat="server" Text="Hourly" GroupName="RateType" />
                        </div>
                    </div>
                </div>
                
                <div class="form-grid">
                    <div class="form-group">
                        <label class="form-label">Amount of Increase</label>
                        <asp:TextBox ID="txtAmountIncrease" runat="server" CssClass="form-input" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Premium/Incentive</label>
                        <asp:TextBox ID="txtPremiumIncentive" runat="server" CssClass="form-input" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                    </div>
                </div>
                
                <div class="form-grid">
                    <div class="form-group">
                        <label class="form-label">New Rate/Salary</label>
                        <asp:TextBox ID="txtNewRate" runat="server" CssClass="form-input" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="form-label">New W-4 Status</label>
                        <asp:TextBox ID="txtNewW4Status" runat="server" CssClass="form-input" placeholder="W-4 status"></asp:TextBox>
                    </div>
                </div>
                
                <div class="form-group">
                    <label class="form-label">Shift Hours</label>
                    <asp:TextBox ID="txtRateChangeShiftHours" runat="server" CssClass="form-input" placeholder="Shift hours"></asp:TextBox>
                </div>
            </div>
        </div>

        <!-- Transfer Section -->
        <div class="form-section">
            <div class="section-header">
                <i class="material-icons">swap_horiz</i>
                <h4>Transfer</h4>
            </div>
            
            <div class="form-group">
                <asp:CheckBox ID="chkTransfer" runat="server" Text="This is a transfer" CssClass="form-check" />
            </div>
            
            <div id="transferSection" style="display: none;">
                <div class="form-grid">
                    <div class="form-group">
                        <label class="form-label">New Location</label>
                        <asp:TextBox ID="txtNewLocation" runat="server" CssClass="form-input" placeholder="New work location"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Leader/Supervisor</label>
                        <asp:TextBox ID="txtLeaderSupervisor" runat="server" CssClass="form-input" placeholder="Supervisor name"></asp:TextBox>
                    </div>
                </div>
                
                <div class="form-grid">
                    <div class="form-group">
                        <label class="form-label">New Class</label>
                        <asp:TextBox ID="txtNewClass" runat="server" CssClass="form-input" placeholder="Employee class"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Shift Hours</label>
                        <asp:TextBox ID="txtTransferShiftHours" runat="server" CssClass="form-input" placeholder="Shift hours"></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>

        <!-- Promotion Section -->
        <div class="form-section">
            <div class="section-header">
                <i class="material-icons">trending_up</i>
                <h4>Promotion</h4>
            </div>
            
            <div class="form-group">
                <asp:CheckBox ID="chkPromotion" runat="server" Text="This is a promotion" CssClass="form-check" />
            </div>
            
            <div id="promotionSection" style="display: none;">
                <div class="form-grid">
                    <div class="form-group">
                        <label class="form-label">Old Job Title</label>
                        <asp:TextBox ID="txtOldJobTitle" runat="server" CssClass="form-input" placeholder="Previous job title"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="form-label">New Job Title</label>
                        <asp:TextBox ID="txtNewJobTitle" runat="server" CssClass="form-input" placeholder="New job title"></asp:TextBox>
                    </div>
                </div>
                
                <div class="form-grid">
                    <div class="form-group">
                        <label class="form-label">New Rate/Salary</label>
                        <asp:TextBox ID="txtPromotionNewRate" runat="server" CssClass="form-input" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Shift Hours</label>
                        <asp:TextBox ID="txtPromotionShiftHours" runat="server" CssClass="form-input" placeholder="Shift hours"></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>

        <!-- Status Change Section -->
        <div class="form-section">
            <div class="section-header">
                <i class="material-icons">update</i>
                <h4>Status Change</h4>
            </div>
            
            <div class="form-group">
                <asp:CheckBox ID="chkStatusChange" runat="server" Text="This is a status change" CssClass="form-check" />
            </div>
            
            <div id="statusChangeSection" style="display: none;">
                <div class="form-grid">
                    <div class="form-group">
                        <label class="form-label">From Status</label>
                        <div class="checkbox-group">
                            <asp:CheckBox ID="chkFromFT" runat="server" Text="FT" />
                            <asp:CheckBox ID="chkFromPT" runat="server" Text="PT" />
                            <asp:CheckBox ID="chkFromPRN" runat="server" Text="PRN" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="form-label">To Status</label>
                        <div class="checkbox-group">
                            <asp:CheckBox ID="chkToFT" runat="server" Text="FT" />
                            <asp:CheckBox ID="chkToPT" runat="server" Text="PT" />
                            <asp:CheckBox ID="chkToPRN" runat="server" Text="PRN" />
                        </div>
                    </div>
                </div>
                
                <div class="form-grid">
                    <div class="form-group">
                        <label class="form-label">From Pay Type</label>
                        <div class="checkbox-group">
                            <asp:CheckBox ID="chkFromSalary" runat="server" Text="Salary" />
                            <asp:CheckBox ID="chkFromHourly" runat="server" Text="Hourly" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="form-label">To Pay Type</label>
                        <div class="checkbox-group">
                            <asp:CheckBox ID="chkToSalary" runat="server" Text="Salary" />
                            <asp:CheckBox ID="chkToHourly" runat="server" Text="Hourly" />
                        </div>
                    </div>
                </div>
                
                <div class="form-group">
                    <label class="form-label">Marital Status</label>
                    <div class="checkbox-group">
                        <asp:RadioButton ID="rbMarried" runat="server" Text="M" GroupName="MaritalStatus" />
                        <asp:RadioButton ID="rbDivorced" runat="server" Text="D" GroupName="MaritalStatus" />
                        <asp:RadioButton ID="rbSingle" runat="server" Text="S" GroupName="MaritalStatus" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Contact Information Changes Section -->
        <div class="form-section">
            <div class="section-header">
                <i class="material-icons">contact_mail</i>
                <h4>Contact Information Changes</h4>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">New Name</label>
                    <asp:TextBox ID="txtNewName" runat="server" CssClass="form-input" placeholder="Updated name"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label class="form-label">New Phone</label>
                    <asp:TextBox ID="txtNewPhone" runat="server" CssClass="form-input" placeholder="(000) 000-0000"></asp:TextBox>
                </div>
            </div>
            
            <div class="form-group full-width">
                <label class="form-label">New Address</label>
                <asp:TextBox ID="txtNewAddress" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="3" placeholder="Full address including city, state, ZIP"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label class="form-label">New Email</label>
                <asp:TextBox ID="txtNewEmail" runat="server" CssClass="form-input" placeholder="email@company.com" TextMode="Email"></asp:TextBox>
            </div>
        </div>

        <!-- Insurance/Benefits Section -->
        <div class="form-section">
            <div class="section-header">
                <i class="material-icons">health_and_safety</i>
                <h4>Insurance/Benefits</h4>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Health Insurance</label>
                    <div class="checkbox-group">
                        <asp:CheckBox ID="chkHealthS" runat="server" Text="S" />
                        <asp:CheckBox ID="chkHealthES" runat="server" Text="E+S" />
                        <asp:CheckBox ID="chkHealthEC" runat="server" Text="E+C" />
                        <asp:CheckBox ID="chkHealthF" runat="server" Text="F" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="form-label">Health Deduct</label>
                    <asp:TextBox ID="txtHealthDeduct" runat="server" CssClass="form-input" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                </div>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Dental Insurance</label>
                    <div class="checkbox-group">
                        <asp:CheckBox ID="chkDentalS" runat="server" Text="S" />
                        <asp:CheckBox ID="chkDentalES" runat="server" Text="E+S" />
                        <asp:CheckBox ID="chkDentalEC" runat="server" Text="E+C" />
                        <asp:CheckBox ID="chkDentalF" runat="server" Text="F" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="form-label">Dental Deduct</label>
                    <asp:TextBox ID="txtDentalDeduct" runat="server" CssClass="form-input" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                </div>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">403(b) Retirement</label>
                    <div class="checkbox-group">
                        <asp:CheckBox ID="chkRetirement403b" runat="server" Text="Yes" />
                        <asp:CheckBox ID="chkNoRetirement" runat="server" Text="No" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="form-label">Retirement Deduct</label>
                    <asp:TextBox ID="txtRetirementDeduct" runat="server" CssClass="form-input" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                </div>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Disability Insurance</label>
                    <div class="checkbox-group">
                        <asp:CheckBox ID="chkDisabilityMe" runat="server" Text="Me" />
                        <asp:CheckBox ID="chkNoDisability" runat="server" Text="No" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="form-label">Effective Date</label>
                    <asp:TextBox ID="txtEffectiveDate" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                </div>
            </div>
        </div>

        <!-- Payroll Deduction Section -->
        <div class="form-section">
            <div class="section-header">
                <i class="material-icons">account_balance</i>
                <h4>Payroll Deduction</h4>
            </div>
            
            <div class="form-group full-width">
                <label class="form-label">Reason for Deduction</label>
                <asp:TextBox ID="txtPayrollReason" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="3" placeholder="Reason for payroll deduction"></asp:TextBox>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Total Amount</label>
                    <asp:TextBox ID="txtPayrollAmount" runat="server" CssClass="form-input" placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                </div>
                <div class="form-group">
                    <asp:CheckBox ID="chkEachPayPeriod" runat="server" Text="Each pay period" CssClass="form-check" />
                </div>
            </div>
        </div>

        <!-- Leave of Absence Section -->
        <div class="form-section">
            <div class="section-header">
                <i class="material-icons">event_busy</i>
                <h4>Leave of Absence</h4>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Type of Leave</label>
                    <div class="checkbox-group" style="flex-direction: column; gap: 0.5rem;">
                        <asp:CheckBox ID="chkVacation" runat="server" Text="Vacation" />
                        <asp:CheckBox ID="chkPaidSickTime" runat="server" Text="Paid Sick Time" />
                        <asp:CheckBox ID="chkUnpaidSickTime" runat="server" Text="Unpaid Sick Time" />
                        <asp:CheckBox ID="chkUnpaidPersonalTime" runat="server" Text="Unpaid Personal Time" />
                        <asp:CheckBox ID="chkFamilyMedical" runat="server" Text="Family/Medical (FMLA)" />
                        <asp:CheckBox ID="chkFuneral" runat="server" Text="Funeral" />
                        <asp:CheckBox ID="chkVoting" runat="server" Text="Voting (max: 3hrs)" />
                        <asp:CheckBox ID="chkPregnancyDisability" runat="server" Text="Pregnancy Disability Leave" />
                        <asp:CheckBox ID="chkMilitary" runat="server" Text="Military" />
                        <asp:CheckBox ID="chkJuryDuty" runat="server" Text="Jury Duty" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="form-label">Funeral Relation to Deceased (if applicable)</label>
                    <asp:TextBox ID="txtFuneralRelation" runat="server" CssClass="form-input" placeholder="Relationship"></asp:TextBox>
                </div>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Start Date</label>
                    <asp:TextBox ID="txtLeaveStartDate" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label class="form-label">End Date</label>
                    <asp:TextBox ID="txtLeaveEndDate" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                </div>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Number of Days</label>
                    <asp:TextBox ID="txtNumberOfDays" runat="server" CssClass="form-input" placeholder="0" TextMode="Number"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label class="form-label">Total Hours</label>
                    <asp:TextBox ID="txtLeaveHours" runat="server" CssClass="form-input" placeholder="0.00" TextMode="Number" step="0.25"></asp:TextBox>
                </div>
            </div>
        </div>

        <!-- Return from Leave Section -->
        <div class="form-section">
            <div class="section-header">
                <i class="material-icons">assignment_return</i>
                <h4>Return from Leave of Absence</h4>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Date Returned to Work</label>
                    <asp:TextBox ID="txtDateReturned" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label class="form-label">Last Day Worked</label>
                    <asp:TextBox ID="txtLastDayWorked" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                </div>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Number of Days Out</label>
                    <asp:TextBox ID="txtDaysOut" runat="server" CssClass="form-input" placeholder="0" TextMode="Number"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label class="form-label">Hours Out</label>
                    <asp:TextBox ID="txtHoursOut" runat="server" CssClass="form-input" placeholder="0.00" TextMode="Number" step="0.25"></asp:TextBox>
                </div>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Absence Excused</label>
                    <div class="checkbox-group">
                        <asp:RadioButton ID="rbAbsenceYes" runat="server" Text="Yes" GroupName="AbsenceExcused" />
                        <asp:RadioButton ID="rbAbsenceNo" runat="server" Text="No" GroupName="AbsenceExcused" />
                    </div>
                </div>
                <div class="form-group">
                    <label class="form-label">Dr's Slip Received</label>
                    <div class="checkbox-group">
                        <asp:RadioButton ID="rbDoctorSlipYes" runat="server" Text="Yes" GroupName="DoctorSlip" />
                        <asp:RadioButton ID="rbDoctorSlipNo" runat="server" Text="No" GroupName="DoctorSlip" />
                    </div>
                </div>
            </div>
            
            <div class="form-grid">
                <div class="form-group">
                    <label class="form-label">Excused Hours</label>
                    <asp:TextBox ID="txtExcusedHours" runat="server" CssClass="form-input" placeholder="0.00" TextMode="Number" step="0.25"></asp:TextBox>
                </div>
                <div class="form-group">
                    <label class="form-label">Accommodation if Needed</label>
                    <asp:TextBox ID="txtAccommodation" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2" placeholder="Any accommodations needed"></asp:TextBox>
                </div>
            </div>
        </div>

        <!-- Additional Comments Section -->
        <div class="form-section">
            <div class="section-header">
                <i class="material-icons">comment</i>
                <h4>Additional Comments</h4>
            </div>
            
            <div class="form-group full-width">
                <label class="form-label">Comments</label>
                <asp:TextBox ID="txtAdditionalComments" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="4" placeholder="Any additional information or comments..."></asp:TextBox>
            </div>
        </div>

        <!-- Form Actions -->
        <div class="form-footer">
            <div class="form-footer-left">
                <span class="required">*</span> Required fields
            </div>
            <div class="form-footer-right">
                <asp:Button ID="btnSaveDraft" runat="server" Text="Save Draft" 
                    CssClass="btn btn-outline" OnClick="btnSaveDraft_Click" />
                <asp:Button ID="btnSubmit" runat="server" Text="Submit for Approval" 
                    CssClass="btn btn-tpa" OnClick="btnSubmit_Click" 
                    OnClientClick="return confirm('Are you sure you want to submit this HR Action for approval? Once submitted, it cannot be edited.');" />
            </div>
        </div>
    </div>

    <!-- JavaScript for Dynamic Sections -->
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            // Rate Change Section Toggle
            const chkRateChange = document.getElementById('<%= chkRateChange.ClientID %>');
            const rateChangeSection = document.getElementById('rateChangeSection');

            if (chkRateChange) {
                chkRateChange.addEventListener('change', function () {
                    rateChangeSection.style.display = this.checked ? 'block' : 'none';
                });
            }

            // Transfer Section Toggle
            const chkTransfer = document.getElementById('<%= chkTransfer.ClientID %>');
            const transferSection = document.getElementById('transferSection');

            if (chkTransfer) {
                chkTransfer.addEventListener('change', function () {
                    transferSection.style.display = this.checked ? 'block' : 'none';
                });
            }

            // Promotion Section Toggle
            const chkPromotion = document.getElementById('<%= chkPromotion.ClientID %>');
            const promotionSection = document.getElementById('promotionSection');

            if (chkPromotion) {
                chkPromotion.addEventListener('change', function () {
                    promotionSection.style.display = this.checked ? 'block' : 'none';
                });
            }

            // Status Change Section Toggle
            const chkStatusChange = document.getElementById('<%= chkStatusChange.ClientID %>');
            const statusChangeSection = document.getElementById('statusChangeSection');

            if (chkStatusChange) {
                chkStatusChange.addEventListener('change', function () {
                    statusChangeSection.style.display = this.checked ? 'block' : 'none';
                });
            }

            // Auto-calculate new rate when previous rate and increase are entered
            const txtPreviousRate = document.getElementById('<%= txtPreviousRate.ClientID %>');
            const txtAmountIncrease = document.getElementById('<%= txtAmountIncrease.ClientID %>');
            const txtNewRate = document.getElementById('<%= txtNewRate.ClientID %>');

            function calculateNewRate() {
                const previous = parseFloat(txtPreviousRate.value) || 0;
                const increase = parseFloat(txtAmountIncrease.value) || 0;
                if (previous > 0 && increase > 0) {
                    txtNewRate.value = (previous + increase).toFixed(2);
                }
            }

            if (txtPreviousRate && txtAmountIncrease && txtNewRate) {
                txtPreviousRate.addEventListener('input', calculateNewRate);
                txtAmountIncrease.addEventListener('input', calculateNewRate);
            }

            // Auto-calculate leave days
            const txtLeaveStartDate = document.getElementById('<%= txtLeaveStartDate.ClientID %>');
            const txtLeaveEndDate = document.getElementById('<%= txtLeaveEndDate.ClientID %>');
            const txtNumberOfDays = document.getElementById('<%= txtNumberOfDays.ClientID %>');

            function calculateLeaveDays() {
                const startDate = new Date(txtLeaveStartDate.value);
                const endDate = new Date(txtLeaveEndDate.value);

                if (startDate && endDate && endDate >= startDate) {
                    const timeDiff = endDate.getTime() - startDate.getTime();
                    const dayDiff = Math.ceil(timeDiff / (1000 * 3600 * 24)) + 1; // +1 to include both start and end days
                    txtNumberOfDays.value = dayDiff;
                }
            }

            if (txtLeaveStartDate && txtLeaveEndDate && txtNumberOfDays) {
                txtLeaveStartDate.addEventListener('change', calculateLeaveDays);
                txtLeaveEndDate.addEventListener('change', calculateLeaveDays);
            }

            // Format SSN input
            const txtSSN = document.getElementById('<%= txtSSN.ClientID %>');
            if (txtSSN) {
                txtSSN.addEventListener('input', function () {
                    let value = this.value.replace(/\D/g, ''); // Remove non-digits
                    if (value.length >= 6) {
                        value = value.substring(0, 3) + '-' + value.substring(3, 5) + '-' + value.substring(5, 9);
                    } else if (value.length >= 3) {
                        value = value.substring(0, 3) + '-' + value.substring(3);
                    }
                    this.value = value;
                });
            }

            // Format phone number input
            const txtNewPhone = document.getElementById('<%= txtNewPhone.ClientID %>');
            if (txtNewPhone) {
                txtNewPhone.addEventListener('input', function () {
                    let value = this.value.replace(/\D/g, ''); // Remove non-digits
                    if (value.length >= 6) {
                        value = '(' + value.substring(0, 3) + ') ' + value.substring(3, 6) + '-' + value.substring(6, 10);
                    } else if (value.length >= 3) {
                        value = '(' + value.substring(0, 3) + ') ' + value.substring(3);
                    }
                    this.value = value;
                });
            }

            // Mutual exclusion for certain checkboxes
            function setupMutualExclusion(checkboxes) {
                checkboxes.forEach(function (checkbox, index) {
                    const element = document.getElementById(checkbox.id);
                    if (element) {
                        element.addEventListener('change', function () {
                            if (this.checked) {
                                checkboxes.forEach(function (otherCheckbox, otherIndex) {
                                    if (index !== otherIndex) {
                                        const otherElement = document.getElementById(otherCheckbox.id);
                                        if (otherElement) {
                                            otherElement.checked = false;
                                        }
                                    }
                                });
                            }
                        });
                    }
                });
            }

            // Setup mutual exclusion for health insurance options
            setupMutualExclusion([
                { id: '<%= chkHealthS.ClientID %>' },
                { id: '<%= chkHealthES.ClientID %>' },
                { id: '<%= chkHealthEC.ClientID %>' },
                { id: '<%= chkHealthF.ClientID %>' }
            ]);
            
            // Setup mutual exclusion for dental insurance options
            setupMutualExclusion([
                { id: '<%= chkDentalS.ClientID %>' },
                { id: '<%= chkDentalES.ClientID %>' },
                { id: '<%= chkDentalEC.ClientID %>' },
                { id: '<%= chkDentalF.ClientID %>' }
            ]);
            
            // Setup mutual exclusion for retirement options
            setupMutualExclusion([
                { id: '<%= chkRetirement403b.ClientID %>' },
                { id: '<%= chkNoRetirement.ClientID %>' }
            ]);
            
            // Setup mutual exclusion for disability insurance options
            setupMutualExclusion([
                { id: '<%= chkDisabilityMe.ClientID %>' },
                { id: '<%= chkNoDisability.ClientID %>' }
            ]);
            
            // Form validation before submit
            const btnSubmit = document.getElementById('<%= btnSubmit.ClientID %>');
            if (btnSubmit) {
                btnSubmit.addEventListener('click', function(e) {
                    const ddlEmployee = document.getElementById('<%= ddlEmployee.ClientID %>');
                    if (!ddlEmployee.value) {
                        alert('Please select an employee.');
                        e.preventDefault();
                        return false;
                    }
                    
                    // Check if at least one action type is selected
                    const actionTypes = [
                        document.getElementById('<%= chkRateChange.ClientID %>'),
                        document.getElementById('<%= chkTransfer.ClientID %>'),
                        document.getElementById('<%= chkPromotion.ClientID %>'),
                        document.getElementById('<%= chkStatusChange.ClientID %>')
                    ];

                    const hasActionSelected = actionTypes.some(function (checkbox) {
                        return checkbox && checkbox.checked;
                    });

                    if (!hasActionSelected) {
                        alert('Please select at least one action type (Rate Change, Transfer, Promotion, or Status Change).');
                        e.preventDefault();
                        return false;
                    }

                    return true;
                });
            }
        });
    </script>
</asp:Content>