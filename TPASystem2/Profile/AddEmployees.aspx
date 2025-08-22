<%@ Page Title="Add Employee" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="AddEmployee.aspx.cs" Inherits="TPASystem2.Profile.AddEmployee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    <style>
        /* ===============================================
   ADD EMPLOYEE PAGE STYLES
   Add these styles to tpa-common.css
   =============================================== */

/* Form Grid Layout */
.form-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1.5rem;
    margin-bottom: 1.5rem;
}

.form-grid:last-child {
    margin-bottom: 0;
}

.form-group.full-width {
    grid-column: 1 / -1;
}

/* Section Headers */
.section-header {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    margin-bottom: 1.5rem;
    padding-bottom: 0.75rem;
    border-bottom: 2px solid var(--border-light);
}

.section-header h4 {
    margin: 0;
    color: var(--text-primary);
    font-size: 1.2rem;
    font-weight: 600;
}

.section-header .material-icons {
    color: var(--tpa-primary);
    font-size: 1.5rem;
}

/* Input with Button */
.input-with-button {
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.input-with-button .form-control {
    flex: 1;
}

.btn-generate {
    background: var(--tpa-primary);
    color: white;
    border: none;
    padding: 0.75rem 1rem;
    border-radius: var(--border-radius);
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-size: 0.85rem;
    font-weight: 500;
    cursor: pointer;
    transition: all var(--transition-fast);
    white-space: nowrap;
}

.btn-generate:hover {
    background: var(--tpa-primary-dark);
    transform: translateY(-1px);
}

.btn-generate .material-icons {
    font-size: 1rem;
}

/* Required Field Indicator */
.required {
    color: var(--status-error);
    font-weight: 600;
}

/* Action Buttons */
.action-buttons {
    display: flex;
    gap: 1rem;
    justify-content: flex-end;
}

/* Success Modal Styles */
.success-modal {
    max-width: 600px;
    width: 90%;
}

.modal-header.success {
    background: linear-gradient(135deg, #10b981 0%, #059669 100%);
    color: white;
}

.modal-header.success .material-icons {
    color: white;
}

.success-details {
    margin-bottom: 1.5rem;
}

.detail-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.75rem 0;
    border-bottom: 1px solid var(--border-light);
}

.detail-item:last-child {
    border-bottom: none;
}

.detail-item .label {
    font-weight: 600;
    color: var(--text-muted);
    min-width: 150px;
}

.detail-item .value {
    font-weight: 500;
    color: var(--text-primary);
    flex: 1;
    text-align: right;
}

.password-field {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    background: var(--background-light);
    padding: 0.5rem;
    border-radius: var(--border-radius);
    font-family: 'Courier New', monospace;
    font-size: 0.9rem;
}

.btn-copy {
    background: var(--tpa-primary);
    color: white;
    border: none;
    padding: 0.25rem 0.5rem;
    border-radius: var(--border-radius-small);
    cursor: pointer;
    display: flex;
    align-items: center;
    transition: all var(--transition-fast);
}

.btn-copy:hover {
    background: var(--tpa-primary-dark);
}

.btn-copy .material-icons {
    font-size: 1rem;
}

/* Alert with Content */
.alert-content {
    display: flex;
    align-items: flex-start;
    gap: 0.75rem;
}

.alert-content .material-icons {
    color: inherit;
    font-size: 1.25rem;
    margin-top: 0.125rem;
}

.alert-content > div {
    flex: 1;
}

/* Form Sections */
.form-section {
    margin-bottom: 2.5rem;
    padding: 2rem;
    background: white;
    border-radius: var(--border-radius);
    border: 1px solid var(--border-light);
}

.form-section:last-child {
    margin-bottom: 0;
}

/* Card Footer */
.card-footer {
    background: var(--background-light);
    padding: 1.5rem 2rem;
    border-top: 1px solid var(--border-light);
    border-radius: 0 0 var(--border-radius) var(--border-radius);
}

/* Responsive Design */
@media (max-width: 768px) {
    .form-grid {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
    
    .action-buttons {
        flex-direction: column;
        align-items: stretch;
    }
    
    .input-with-button {
        flex-direction: column;
        align-items: stretch;
    }
    
    .detail-item {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.5rem;
    }
    
    .detail-item .value {
        text-align: left;
    }
    
    .password-field {
        justify-content: space-between;
    }
    
    .form-section {
        padding: 1.5rem;
        margin-bottom: 1.5rem;
    }
    
    .card-footer {
        padding: 1rem 1.5rem;
    }
}

@media (max-width: 480px) {
    .success-modal {
        width: 95%;
        margin: 1rem;
    }
    
    .modal-content {
        max-height: 95vh;
    }
    
    .section-header h4 {
        font-size: 1.1rem;
    }
    
    .form-section {
        padding: 1rem;
    }
    
    .card-footer {
        padding: 1rem;
    }
    .form-group-container {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 1rem;
            grid-column: 1 / -1;
            padding: 1rem;
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            border-radius: 6px;
            margin-bottom: 1rem;
        }
        
        .form-group-container .form-group {
            margin-bottom: 0;
        }

        @media (max-width: 768px) {
            .form-group-container {
                grid-template-columns: 1fr;
            }
        }
}
    </style>
 
   <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">person_add</i>
                    Add New Employee
                </h1>
                <p class="welcome-subtitle">Create a new employee profile - direct system access without onboarding</p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">business</i>
                        <span>Direct System Access</span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">security</i>
                        <span>Temporary Password Generated</span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">notification_important</i>
                        <span>Skips Onboarding Process</span>
                    </div>
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnBackToProfiles" runat="server" Text="Back to Employee Profiles" 
                    CssClass="btn btn-outline-light" OnClick="btnBackToProfiles_Click" />

                <asp:FileUpload runat="server"  CssClass="btn btn-outline-light" Text="Upload File" onclick=""></asp:FileUpload>
            </div>
        </div>
    </div>

    <!-- Alert Messages -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Employee Information Form -->
    <div class="card">
        <div class="card-header">
            <h3>
                <i class="material-icons">assignment_ind</i>
                Employee Information
            </h3>
            <p class="card-subtitle">Complete all required fields to create the employee profile</p>
        </div>

        <div class="card-body">
            <!-- Personal Information Section -->
            <div class="form-section">
                <div class="section-header">
                    <h4>
                        <i class="material-icons">person</i>
                        Personal Information
                    </h4>
                </div>

                <div class="form-grid">
                    <div class="form-group">
                        <label for="<%= txtFirstName.ClientID %>">First Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" MaxLength="100" />
                        <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" 
                            ControlToValidate="txtFirstName" ErrorMessage="First Name is required" 
                            CssClass="field-validation-error" Display="Dynamic" />
                    </div>

                    <div class="form-group">
                        <label for="<%= txtLastName.ClientID %>">Last Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" MaxLength="100" />
                        <asp:RequiredFieldValidator ID="rfvLastName" runat="server" 
                            ControlToValidate="txtLastName" ErrorMessage="Last Name is required" 
                            CssClass="field-validation-error" Display="Dynamic" />
                    </div>

                    <div class="form-group">
                        <label for="<%= txtEmployeeNumber.ClientID %>">Employee Number <span class="required">*</span></label>
                        <div class="input-with-button">
                            <asp:TextBox ID="txtEmployeeNumber" runat="server" CssClass="form-control" MaxLength="20" />
                            <button type="button" class="btn-generate" onclick="generateEmployeeNumber()">
                                <i class="material-icons">refresh</i>
                                Generate
                            </button>
                        </div>
                        <asp:RequiredFieldValidator ID="rfvEmployeeNumber" runat="server" 
                            ControlToValidate="txtEmployeeNumber" ErrorMessage="Employee Number is required" 
                            CssClass="field-validation-error" Display="Dynamic" />
                    </div>

                    <div class="form-group">
                        <label for="<%= ddlGender.ClientID %>">Gender</label>
                        <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Select Gender" Value="" />
                            <asp:ListItem Text="Male" Value="Male" />
                            <asp:ListItem Text="Female" Value="Female" />
                            <asp:ListItem Text="Other" Value="Other" />
                            <asp:ListItem Text="Prefer not to say" Value="PreferNotToSay" />
                        </asp:DropDownList>
                    </div>

                    <div class="form-group">
                        <label for="<%= txtDateOfBirth.ClientID %>">Date of Birth</label>
                        <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>

                    <div class="form-group">
                        <label for="<%= txtSSN.ClientID %>">Social Security Number</label>
                        <asp:TextBox ID="txtSSN" runat="server" CssClass="form-control" MaxLength="11" 
                            placeholder="XXX-XX-XXXX" />
                    </div>

                    <div class="form-group">
                        <label for="<%= txtDriversLicense.ClientID %>">Driver's License</label>
                        <asp:TextBox ID="txtDriversLicense" runat="server" CssClass="form-control" MaxLength="50" />
                    </div>
                </div>
            </div>

            <!-- Contact Information Section -->
            <div class="form-section">
                <div class="section-header">
                    <h4>
                        <i class="material-icons">contact_mail</i>
                        Contact Information
                    </h4>
                </div>

                <div class="form-grid">
                    <div class="form-group">
                        <label for="<%= txtEmail.ClientID %>">Email Address <span class="required">*</span></label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" MaxLength="255" />
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                            ControlToValidate="txtEmail" ErrorMessage="Email is required" 
                            CssClass="field-validation-error" Display="Dynamic" />
                        <asp:RegularExpressionValidator ID="revEmail" runat="server" 
                            ControlToValidate="txtEmail" ErrorMessage="Please enter a valid email address"
                            ValidationExpression="^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
                            CssClass="field-validation-error" Display="Dynamic" />
                    </div>

                    <div class="form-group">
                        <label for="<%= txtPhoneNumber.ClientID %>">Phone Number</label>
                        <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control" MaxLength="20" />
                    </div>

                    <div class="form-group full-width">
                        <label for="<%= txtAddress.ClientID %>">Address</label>
                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" MaxLength="255" />
                    </div>

                    <div class="form-group">
                        <label for="<%= txtCity.ClientID %>">City</label>
                        <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" MaxLength="100" />
                    </div>

                    <div class="form-group">
                        <label for="<%= txtState.ClientID %>">State</label>
                        <asp:TextBox ID="txtState" runat="server" CssClass="form-control" MaxLength="50" />
                    </div>

                    <div class="form-group">
                        <label for="<%= txtZipCode.ClientID %>">Zip Code</label>
                        <asp:TextBox ID="txtZipCode" runat="server" CssClass="form-control" MaxLength="10" />
                    </div>
                </div>
            </div>

            <!-- Employment Information Section -->
            <div class="form-section">
                <div class="section-header">
                    <h4>
                        <i class="material-icons">work</i>
                        Employment Information
                    </h4>
                </div>

                <div class="form-grid">
                    <div class="form-group">
                        <label for="<%= txtJobTitle.ClientID %>">Job Title <span class="required">*</span></label>
                        <asp:TextBox ID="txtJobTitle" runat="server" CssClass="form-control" MaxLength="100" />
                        <asp:RequiredFieldValidator ID="rfvJobTitle" runat="server" 
                            ControlToValidate="txtJobTitle" ErrorMessage="Job Title is required" 
                            CssClass="field-validation-error" Display="Dynamic" />
                    </div>

                    <div class="form-group">
                        <label for="<%= ddlDepartment.ClientID %>">Department <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvDepartment" runat="server" 
                            ControlToValidate="ddlDepartment" InitialValue="0" 
                            ErrorMessage="Department is required" 
                            CssClass="field-validation-error" Display="Dynamic" />
                    </div>

                    <div class="form-group">
                        <label for="<%= ddlEmployeeType.ClientID %>">Employee Type</label>
                        <asp:DropDownList ID="ddlEmployeeType" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Select Type" Value="" />
                            <asp:ListItem Text="Full-time" Value="Full-time" />
                            <asp:ListItem Text="Part-time" Value="Part-time" />
                            <asp:ListItem Text="Contract" Value="Contract" />
                            <asp:ListItem Text="Temporary" Value="Temporary" />
                            <asp:ListItem Text="Intern" Value="Intern" />
                        </asp:DropDownList>
                    </div>

                    <div class="form-group">
                        <label for="<%= ddlManager.ClientID %>">Manager</label>
                        <asp:DropDownList ID="ddlManager" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                    </div>

                    <div class="form-group">
                        <label for="<%= txtHireDate.ClientID %>">Hire Date <span class="required">*</span></label>
                        <asp:TextBox ID="txtHireDate" runat="server" CssClass="form-control" TextMode="Date" />
                        <asp:RequiredFieldValidator ID="rfvHireDate" runat="server" 
                            ControlToValidate="txtHireDate" ErrorMessage="Hire Date is required" 
                            CssClass="field-validation-error" Display="Dynamic" />
                    </div>

                    <div class="form-group">
                        <label for="<%= txtSalary.ClientID %>">Salary</label>
                        <asp:TextBox ID="txtSalary" runat="server" CssClass="form-control" TextMode="Number" 
                            step="0.01" min="0" placeholder="0.00" />
                    </div>

                    <div class="form-group full-width">
                        <label for="<%= txtWorkLocation.ClientID %>">Work Location</label>
                        <asp:TextBox ID="txtWorkLocation" runat="server" CssClass="form-control" MaxLength="100" />
                    </div>
                </div>
            </div>

            <!-- Benefits and Additional Information -->
            <div class="form-section">
                <div class="section-header">
                    <h4>
                        <i class="material-icons">card_giftcard</i>
                        Benefits & Additional Information
                    </h4>
                </div>
                
                <div class="form-grid">
                    <div class="form-group">
                        <label for="<%= ddlDirectDeposit.ClientID %>">Direct Deposit Status</label>
                        <asp:DropDownList ID="ddlDirectDeposit" runat="server" CssClass="form-control" onchange="toggleDirectDepositDetails()">
                            <asp:ListItem Text="Not Set" Value="NotSet" />
                            <asp:ListItem Text="Yes" Value="Yes" />
                            <asp:ListItem Text="No" Value="No" />
                            <asp:ListItem Text="Pending" Value="Pending" />
                        </asp:DropDownList>
                    </div>

                    <div class="form-group-container" id="directDepositDetails" style="display: none;">
                        <div class="form-group">
                            <label for="<%= txtBankName.ClientID %>">Bank Name</label>
                            <asp:TextBox ID="txtBankName" runat="server" CssClass="form-control" 
                                MaxLength="100" placeholder="e.g., Bank of America, Wells Fargo" />
                        </div>
                        <div class="form-group">
                            <label for="<%= txtRoutingNumber.ClientID %>">Routing Number</label>
                            <asp:TextBox ID="txtRoutingNumber" runat="server" CssClass="form-control" 
                                MaxLength="9" placeholder="9-digit routing number" />
                        </div>
                        <div class="form-group">
                            <label for="<%= txtAccountNumber.ClientID %>">Account Number</label>
                            <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-control" 
                                MaxLength="20" placeholder="Bank account number" />
                        </div>
                        <div class="form-group">
                            <label for="<%= ddlAccountType.ClientID %>">Account Type</label>
                            <asp:DropDownList ID="ddlAccountType" runat="server" CssClass="form-control">
                                <asp:ListItem Text="Select Type" Value="" />
                                <asp:ListItem Text="Checking" Value="Checking" />
                                <asp:ListItem Text="Savings" Value="Savings" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group">
                        <label for="<%= txtVacationDays.ClientID %>">Vacation Days</label>
                        <asp:TextBox ID="txtVacationDays" runat="server" CssClass="form-control" 
                            TextMode="Number" min="0" max="365" />
                    </div>

                    <div class="form-group">
                        <label for="<%= txtSickDays.ClientID %>">Sick Days</label>
                        <asp:TextBox ID="txtSickDays" runat="server" CssClass="form-control" 
                            TextMode="Number" min="0" max="365" />
                    </div>

                    <div class="form-group">
                        <label for="<%= ddlInsurance.ClientID %>">Insurance Coverage</label>
                        <asp:DropDownList ID="ddlInsurance" runat="server" CssClass="form-control" onchange="toggleInsuranceDetails()">
                            <asp:ListItem Text="Not Enrolled" Value="NotEnrolled" />
                            <asp:ListItem Text="Medical Only" Value="Medical" />
                            <asp:ListItem Text="Dental Only" Value="Dental" />
                            <asp:ListItem Text="Vision Only" Value="Vision" />
                            <asp:ListItem Text="Medical + Dental" Value="MedicalDental" />
                            <asp:ListItem Text="Medical + Vision" Value="MedicalVision" />
                            <asp:ListItem Text="Dental + Vision" Value="DentalVision" />
                            <asp:ListItem Text="Full Coverage" Value="Full" />
                        </asp:DropDownList>
                    </div>

                    <div class="form-group" id="insuranceDetails" style="display: none;">
                        <label for="<%= txtInsuranceDetails.ClientID %>">Insurance Details</label>
                        <asp:TextBox ID="txtInsuranceDetails" runat="server" CssClass="form-control" 
                            TextMode="MultiLine" Rows="2" MaxLength="500" 
                            placeholder="Plan names, policy numbers, coverage amounts, deductibles, etc." />
                    </div>

                    <div class="form-group">
                        <label for="<%= txtHoursPerWeek.ClientID %>">Hours per Week</label>
                        <asp:TextBox ID="txtHoursPerWeek" runat="server" CssClass="form-control" 
                            TextMode="Number" min="1" max="80" />
                    </div>

                    <div class="form-group">
                        <label for="<%= ddl403b.ClientID %>">403(b) Participation</label>
                        <asp:DropDownList ID="ddl403b" runat="server" CssClass="form-control" onchange="toggle403bDetails()">
                            <asp:ListItem Text="Not Enrolled" Value="NotEnrolled" />
                            <asp:ListItem Text="Enrolled" Value="Enrolled" />
                            <asp:ListItem Text="Pending" Value="Pending" />
                            <asp:ListItem Text="Declined" Value="Declined" />
                        </asp:DropDownList>
                    </div>

                    <div class="form-group" id="participation403bDetails" style="display: none;">
                        <label for="<%= txt403bDetails.ClientID %>">403(b) Details</label>
                        <asp:TextBox ID="txt403bDetails" runat="server" CssClass="form-control" 
                            TextMode="MultiLine" Rows="2" MaxLength="500" 
                            placeholder="Contribution percentage, provider name, account number, beneficiaries, etc." />
                    </div>

                    <div class="form-group">
                        <label for="<%= ddlVehicleInsurance.ClientID %>">Vehicle Insurance</label>
                        <asp:DropDownList ID="ddlVehicleInsurance" runat="server" CssClass="form-control" onchange="toggleVehicleInsuranceDetails()">
                            <asp:ListItem Text="Not Required" Value="NotRequired" />
                            <asp:ListItem Text="On File" Value="OnFile" />
                            <asp:ListItem Text="Pending" Value="Pending" />
                            <asp:ListItem Text="Expired" Value="Expired" />
                        </asp:DropDownList>
                    </div>

                    <div class="form-group" id="vehicleInsuranceDetails" style="display: none;">
                        <label for="<%= txtVehicleInsuranceDetails.ClientID %>">Vehicle Insurance Details</label>
                        <asp:TextBox ID="txtVehicleInsuranceDetails" runat="server" CssClass="form-control" 
                            TextMode="MultiLine" Rows="2" MaxLength="500" 
                            placeholder="Insurance company, policy number, coverage limits, expiration date, etc." />
                    </div>

                    <div class="form-group full-width">
                        <label for="<%= txtComments.ClientID %>">Comments</label>
                        <asp:TextBox ID="txtComments" runat="server" CssClass="form-control" 
                            TextMode="MultiLine" Rows="3" MaxLength="1000" />
                    </div>
                </div>
            </div>

            <!-- Important Notice -->
            <div class="alert alert-info">
                <div class="alert-content">
                    <i class="material-icons">info</i>
                    <div>
                        <strong>Important:</strong> This employee will be created with direct system access. 
                        A temporary password will be generated automatically and displayed after creation. 
                        The onboarding process will be skipped, and the employee will be marked as "Active" immediately.
                    </div>
                </div>
            </div>
        </div>

        <div class="card-footer">
            <div class="action-buttons">
                <asp:Button ID="btnSaveEmployee" runat="server" Text="Create Employee" 
                    CssClass="btn btn-primary" OnClick="btnSaveEmployee_Click" />
                <asp:Button ID="btnSaveAndNew" runat="server" Text="Create & Add Another" 
                    CssClass="btn btn-secondary" OnClick="btnSaveAndNew_Click" />
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                    CssClass="btn btn-outline" OnClick="btnCancel_Click" CausesValidation="false" />
            </div>
        </div>
    </div>

    <!-- Success Modal -->
    <div id="successModal" class="modal-overlay" style="display: none;">
        <div class="modal-content success-modal">
            <div class="modal-header success">
                <h3>
                    <i class="material-icons">check_circle</i>
                    Employee Created Successfully
                </h3>
            </div>
            <div class="modal-body">
                <div class="success-details">
                    <div class="detail-item">
                        <span class="label">Employee Number:</span>
                        <span class="value" id="modalEmployeeNumber"></span>
                    </div>
                    <div class="detail-item">
                        <span class="label">Name:</span>
                        <span class="value" id="modalEmployeeName"></span>
                    </div>
                    <div class="detail-item">
                        <span class="label">Department:</span>
                        <span class="value" id="modalDepartment"></span>
                    </div>
                    <div class="detail-item">
                        <span class="label">Email:</span>
                        <span class="value" id="modalEmail"></span>
                    </div>
                    <div class="detail-item">
                        <span class="label">Temporary Password:</span>
                        <span class="value password-field" id="modalPassword">
                            <span id="passwordText"></span>
                            <button type="button" class="btn-copy" onclick="copyPassword()">
                                <i class="material-icons">content_copy</i>
                            </button>
                        </span>
                    </div>
                </div>
                <div class="alert alert-warning">
                    <i class="material-icons">warning</i>
                    <strong>Important:</strong> Please share this temporary password with the employee securely. 
                    They will be required to change it on their first login.
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-primary" onclick="closeSuccessModal()">Continue</button>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // Generate Employee Number
        function generateEmployeeNumber() {
            const xhr = new XMLHttpRequest();
            xhr.open('POST', '<%= ResolveUrl("~/Profile/AddEmployee.aspx/GenerateEmployeeNumber") %>', true);
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4 && xhr.status === 200) {
                    const response = JSON.parse(xhr.responseText);
                    document.getElementById('<%= txtEmployeeNumber.ClientID %>').value = response.d;
                }
            };
            xhr.send('{}');
        }

        // Success Modal Functions
        function showSuccessModal(employeeNumber, employeeName, department, email, tempPassword) {
            document.getElementById('modalEmployeeNumber').textContent = employeeNumber;
            document.getElementById('modalEmployeeName').textContent = employeeName;
            document.getElementById('modalDepartment').textContent = department;
            document.getElementById('modalEmail').textContent = email;
            document.getElementById('passwordText').textContent = tempPassword;
            document.getElementById('successModal').style.display = 'flex';
        }

        function closeSuccessModal() {
            document.getElementById('successModal').style.display = 'none';
        }

        function copyPassword() {
            const passwordText = document.getElementById('passwordText').textContent;
            navigator.clipboard.writeText(passwordText).then(function() {
                const copyBtn = document.querySelector('.btn-copy');
                const originalHTML = copyBtn.innerHTML;
                copyBtn.innerHTML = '<i class="material-icons">check</i>';
                setTimeout(() => {
                    copyBtn.innerHTML = originalHTML;
                }, 2000);
            });
        }

        // Auto-generate employee number on page load
        document.addEventListener('DOMContentLoaded', function() {
            const empNumberField = document.getElementById('<%= txtEmployeeNumber.ClientID %>');
            if (!empNumberField.value) {
                generateEmployeeNumber();
            }

            // Add SSN formatting
            const ssnField = document.getElementById('<%= txtSSN.ClientID %>');
            if (ssnField) {
                ssnField.addEventListener('input', function(e) {
                    let value = e.target.value.replace(/\D/g, '');
                    if (value.length >= 6) {
                        value = value.substring(0, 3) + '-' + value.substring(3, 5) + '-' + value.substring(5, 9);
                    } else if (value.length >= 4) {
                        value = value.substring(0, 3) + '-' + value.substring(3, 5);
                    }
                    e.target.value = value;
                });
            }
        });

        // Toggle detail fields based on dropdown selections
        function toggleDirectDepositDetails() {
            const dropdown = document.getElementById('<%= ddlDirectDeposit.ClientID %>');
            const detailsDiv = document.getElementById('directDepositDetails');
            
            if (dropdown.value === 'Yes' || dropdown.value === 'Pending') {
                detailsDiv.style.display = 'block';
            } else {
                detailsDiv.style.display = 'none';
                // Clear the bank detail fields when hiding
                document.getElementById('<%= txtBankName.ClientID %>').value = '';
                document.getElementById('<%= txtRoutingNumber.ClientID %>').value = '';
                document.getElementById('<%= txtAccountNumber.ClientID %>').value = '';
                document.getElementById('<%= ddlAccountType.ClientID %>').selectedIndex = 0;
            }
        }

        function toggleInsuranceDetails() {
            const dropdown = document.getElementById('<%= ddlInsurance.ClientID %>');
            const detailsDiv = document.getElementById('insuranceDetails');
            
            if (dropdown.value !== 'NotEnrolled') {
                detailsDiv.style.display = 'block';
            } else {
                detailsDiv.style.display = 'none';
                // Clear the details field when hiding
                document.getElementById('<%= txtInsuranceDetails.ClientID %>').value = '';
            }
        }

        function toggle403bDetails() {
            const dropdown = document.getElementById('<%= ddl403b.ClientID %>');
            const detailsDiv = document.getElementById('participation403bDetails');
            
            if (dropdown.value === 'Enrolled' || dropdown.value === 'Pending') {
                detailsDiv.style.display = 'block';
            } else {
                detailsDiv.style.display = 'none';
                // Clear the details field when hiding
                document.getElementById('<%= txt403bDetails.ClientID %>').value = '';
            }
        }

        function toggleVehicleInsuranceDetails() {
            const dropdown = document.getElementById('<%= ddlVehicleInsurance.ClientID %>');
            const detailsDiv = document.getElementById('vehicleInsuranceDetails');
            
            if (dropdown.value === 'OnFile' || dropdown.value === 'Pending' || dropdown.value === 'Expired') {
                detailsDiv.style.display = 'block';
            } else {
                detailsDiv.style.display = 'none';
                // Clear the details field when hiding
                document.getElementById('<%= txtVehicleInsuranceDetails.ClientID %>').value = '';
            }
        }
    </script>

</asp:Content>