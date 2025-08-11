<%@ Page Title="New Hire Employment Application" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="NewHirePaperWork.aspx.cs" Inherits="TPASystem2.OnBoarding.NewHirePaperWork" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">


    <style>
        /* ===================================
   NEW HIRE PAPERWORK ENHANCED STYLES
   Add these styles to tpa-common.css
   =================================== */

/* Application Header */
.application-header {
    background: white;
    padding: 2rem;
    border-radius: 16px;
    box-shadow: 0 4px 16px rgba(0,0,0,0.1);
    margin-bottom: 2rem;
    text-align: center;
    border: 1px solid #e5e7eb;
}

.application-title {
    color: #1e293b;
    font-size: 2rem;
    font-weight: 700;
    margin: 0 0 1rem 0;
}

.application-subtitle {
    color: #64748b;
    font-size: 1rem;
    line-height: 1.6;
    margin: 0;
}

/* Enhanced Tab Navigation */
.tab-navigation {
    background: white;
    border-radius: 16px;
    box-shadow: 0 4px 16px rgba(0,0,0,0.1);
    margin-bottom: 2rem;
    overflow: hidden;
    border: 1px solid #e5e7eb;
}

.tab-buttons {
    display: flex;
    flex-wrap: wrap;
    background: #f8f9fa;
}

.tab-button {
    flex: 1;
    min-width: 140px;
    padding: 1.25rem 1rem;
    background: transparent;
    border: none;
    border-right: 1px solid #e5e7eb;
    color: #64748b;
    font-size: 0.9rem;
    font-weight: 500;
    cursor: pointer;
    transition: all 0.3s ease;
    text-decoration: none;
    font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
}

.tab-button:last-child {
    border-right: none;
}

.tab-button:hover {
    background: #e2e8f0;
    color: #334155;
    transform: translateY(-1px);
}

.tab-button.active {
    background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
    color: white;
    position: relative;
    box-shadow: 0 4px 12px rgba(25, 118, 210, 0.3);
}

/* Responsive tab navigation */
@media (max-width: 768px) {
    .tab-buttons {
        flex-direction: column;
    }
    
    .tab-button {
        flex-direction: row;
        justify-content: flex-start;
        padding: 1rem 1.5rem;
        border-right: none;
        border-bottom: 1px solid #e5e7eb;
        min-width: auto;
    }
    
    .tab-button:last-child {
        border-bottom: none;
    }
}

/* Form Container */
.tabbed-form-container {
    background: white;
    border-radius: 16px;
    box-shadow: 0 4px 16px rgba(0,0,0,0.1);
    overflow: hidden;
    border: 1px solid #e5e7eb;
}

/* Tab Content */
.tab-content {
    padding: 2.5rem;
    display: none;
    animation: fadeIn 0.3s ease-in-out;
    background: white;
    color: #1e293b;
}

.tab-content.active {
    display: block;
}

@keyframes fadeIn {
    from { opacity: 0; transform: translateY(10px); }
    to { opacity: 1; transform: translateY(0); }
}

/* Tab Headers */
.tab-header {
    margin-bottom: 2rem;
    padding-bottom: 1.5rem;
    border-bottom: 2px solid #f1f5f9;
}

.tab-header h2 {
    color: #1e293b;
    font-size: 1.75rem;
    font-weight: 700;
    margin: 0 0 0.5rem 0;
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.tab-header .material-icons {
    color: #1976d2;
    font-size: 1.75rem;
}

.tab-header p {
    color: #64748b;
    margin: 0;
    font-size: 1rem;
    line-height: 1.6;
}

/* Form Sections */
.form-section {
    margin-bottom: 2rem;
    padding: 2rem;
    background: white;
    border-radius: 12px;
    border: 1px solid #e5e7eb;
    box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}

.form-section:last-child {
    margin-bottom: 0;
}

.form-section h3 {
    color: #1976d2;
    font-size: 1.25rem;
    font-weight: 600;
    margin: 0 0 1.5rem 0;
    padding-bottom: 0.5rem;
    border-bottom: 2px solid #e3f2fd;
}

/* Form Elements */
.form-row {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 1.5rem;
    margin-bottom: 1.5rem;
}

.form-row:last-child {
    margin-bottom: 0;
}

.form-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.form-group.full-width {
    grid-column: 1 / -1;
}

.form-label {
    display: block;
    font-weight: 600;
    color: #374151;
    margin-bottom: 0.5rem;
    font-size: 0.9rem;
}

.form-label.required::after {
    content: ' *';
    color: #ef4444;
    font-weight: bold;
}

.form-input {
    width: 100%;
    padding: 0.75rem 1rem;
    border: 2px solid #e5e7eb;
    border-radius: 8px;
    font-size: 1rem;
    transition: all 0.3s ease;
    background: white;
    color: #1e293b;
}

.form-input:focus {
    outline: none;
    border-color: #1976d2;
    box-shadow: 0 0 0 3px rgba(25, 118, 210, 0.1);
}

.form-input::placeholder {
    color: #9ca3af;
}

/* Checkbox and Radio Groups */
.checkbox-group {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
    align-items: center;
}

.checkbox-group input[type="checkbox"],
.checkbox-group input[type="radio"] {
    margin-right: 0.5rem;
}

/* Form Tables */
.form-table {
    width: 100%;
    border-collapse: collapse;
    margin: 1rem 0;
    background: white;
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}

.form-table th,
.form-table td {
    padding: 1rem;
    text-align: left;
    border-bottom: 1px solid #e5e7eb;
    vertical-align: top;
}

.form-table th {
    background: #f8f9fa;
    font-weight: 600;
    color: #374151;
    border-bottom: 2px solid #e5e7eb;
}

.form-table tr:last-child td {
    border-bottom: none;
}

/* Education Table Specific */
.education-table .year-checkboxes {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
    align-items: center;
}

.education-table .year-checkboxes label {
    margin: 0;
    font-size: 0.85rem;
    display: flex;
    align-items: center;
    gap: 0.25rem;
}

/* Employment Grid */
.employment-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1.5rem;
    margin-top: 1rem;
}

.employment-full-row {
    grid-column: 1 / -1;
}

/* Criminal History Table */
.criminal-history-table {
    margin: 1rem 0;
}

.criminal-history-table th {
    background: #fef2f2;
    color: #991b1b;
    font-weight: 600;
}

/* Authorization Sections */
.authorization-header {
    background: #f8f9fa;
    padding: 1.5rem;
    border-radius: 8px;
    margin-bottom: 1.5rem;
    border: 1px solid #e5e7eb;
}

.authorization-content {
    background: #fafbfc;
    padding: 1.5rem;
    border-radius: 8px;
    border: 1px solid #e5e7eb;
    line-height: 1.6;
}

.authorization-content p {
    margin-bottom: 1rem;
    color: #374151;
}

/* Final Acknowledgment */
.final-acknowledgment-container {
    display: flex;
    gap: 1rem;
    align-items: flex-start;
    padding: 1.5rem;
    background: #fef7cd;
    border: 2px solid #fbbf24;
    border-radius: 8px;
    margin-bottom: 1rem;
}

.checkbox-label {
    color: #374151;
    line-height: 1.6;
    cursor: pointer;
}

/* Navigation Buttons */
.form-navigation {
    padding: 2rem;
    background: #f8f9fa;
    border-top: 1px solid #e5e7eb;
}

.nav-buttons {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 1rem;
}

.btn {
    padding: 0.75rem 2rem;
    border: none;
    border-radius: 8px;
    font-size: 1rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s ease;
    text-decoration: none;
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    min-width: 120px;
    justify-content: center;
}

.btn-primary {
    background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
    color: white;
}

.btn-primary:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 20px rgba(25, 118, 210, 0.3);
}

.btn-secondary {
    background: #6b7280;
    color: white;
}

.btn-secondary:hover {
    background: #4b5563;
    transform: translateY(-2px);
}

.btn-success {
    background: linear-gradient(135deg, #059669 0%, #10b981 100%);
    color: white;
}

.btn-success:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 20px rgba(5, 150, 105, 0.3);
}

/* Message Panels */
.message-panel {
    padding: 1rem 1.5rem;
    border-radius: 8px;
    margin-bottom: 2rem;
    border: 1px solid;
}

.message-panel.success {
    background: #f0fdf4;
    border-color: #10b981;
    color: #065f46;
}

.message-panel.error {
    background: #fef2f2;
    border-color: #ef4444;
    color: #991b1b;
}

.message-text {
    margin: 0;
    font-weight: 500;
}

/* Legal Notice */
.legal-notice {
    background: #f8f9fa;
    padding: 2rem;
    border-radius: 12px;
    margin-top: 2rem;
    border: 1px solid #e5e7eb;
    text-align: center;
}

.legal-notice h4 {
    color: #1976d2;
    margin: 0 0 1rem 0;
    font-size: 1.25rem;
    font-weight: 700;
}

.legal-notice p {
    color: #374151;
    margin: 0;
    line-height: 1.6;
    font-size: 0.9rem;
}

/* Form Notes */
.form-note {
    font-size: 0.85rem;
    color: #6b7280;
    font-style: italic;
    margin-top: 0.5rem;
}

/* Inline Input for Reference Form */
.inline-input {
    display: inline !important;
    width: auto !important;
    margin: 0 0.5rem !important;
    padding: 0.25rem 0.5rem !important;
    border-bottom: 2px solid #1976d2 !important;
    border-top: none !important;
    border-left: none !important;
    border-right: none !important;
    border-radius: 0 !important;
    background: transparent !important;
    font-weight: 600 !important;
}

/* Text Center Utility */
.text-center {
    text-align: center;
}

/* Validation Errors */
.field-validation-error {
    color: #ef4444;
    font-size: 0.8rem;
    margin-top: 0.5rem;
    display: block;
    padding: 0.5rem 0.75rem;
    background: #fef2f2;
    border: 1px solid #fecaca;
    border-radius: 4px;
    border-left: 3px solid #ef4444;
}

/* Responsive Design */
@media (max-width: 768px) {
    .tab-content {
        padding: 1.5rem;
    }
    
    .form-section {
        padding: 1.5rem;
    }
    
    .form-navigation {
        padding: 1.5rem;
    }
    
    .nav-buttons {
        flex-direction: column;
        gap: 1rem;
    }
    
    .nav-buttons .btn {
        width: 100%;
        min-width: auto;
    }
    
    .employment-grid {
        grid-template-columns: 1fr;
    }
    
    .form-row {
        grid-template-columns: 1fr;
    }
    
    .checkbox-group {
        flex-direction: column;
        align-items: flex-start;
    }
}

@media (max-width: 576px) {
    .application-header {
        padding: 1.5rem;
    }
    
    .application-title {
        font-size: 1.5rem;
    }
    
    .tab-content {
        padding: 1rem;
    }
    
    .form-section {
        padding: 1rem;
    }
    
    .final-acknowledgment-container {
        flex-direction: column;
        gap: 0.5rem;
    }
}

/* Print Styles */
@media print {
    .tab-navigation,
    .form-navigation,
    .btn {
        display: none !important;
    }
    
    .tab-content {
        display: block !important;
        page-break-after: always;
        padding: 0;
        box-shadow: none;
    }
    
    .tab-content:last-child {
        page-break-after: auto;
    }
    
    .form-section {
        page-break-inside: avoid;
        box-shadow: none;
        border: 1px solid #000;
    }
    
    .application-header {
        box-shadow: none;
        border: 1px solid #000;
    }
}

    </style>
    <!-- Page Header -->
    <div class="onboarding-page-header">
        <div class="header-content">
            <div class="header-left">
                <h1>
                    <span class="material-icons">assignment</span>
                    Employment Application
                </h1>
                <p class="page-description">Complete your employment application for TPA, Inc.</p>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnSaveDraft" runat="server" Text="Save Draft" CssClass="btn-tpa btn-secondary" OnClick="btnSaveDraft_Click" />
            </div>
        </div>
    </div>

    <!-- Error/Success Messages -->
    <asp:Panel ID="pnlMessages" runat="server" Visible="false" CssClass="message-panel">
        <asp:Label ID="lblMessage" runat="server" CssClass="message-text"></asp:Label>
    </asp:Panel>

    <!-- Application Header -->
    <div class="application-header">
        <h1 class="application-title">Application for Employment</h1>
        <p class="application-subtitle">
            Application will be kept on file for 90 Days<br />
            Please complete application <strong>completely</strong> to be considered. 
            All information must be entered on application form to be considered for employment – even if resume is attached.
        </p>
    </div>

    <!-- Tab Navigation -->
    <div class="tab-navigation">
        <div class="tab-buttons">
            <asp:Button ID="btnTabPersonal" runat="server" Text="Personal Information" CssClass="tab-button active" OnClick="btnTabPersonal_Click" />
            <asp:Button ID="btnTabPosition" runat="server" Text="Position Information" CssClass="tab-button" OnClick="btnTabPosition_Click" />
            <asp:Button ID="btnTabBackground" runat="server" Text="Background Questions" CssClass="tab-button" OnClick="btnTabBackground_Click" />
            <asp:Button ID="btnTabEducation" runat="server" Text="Education" CssClass="tab-button" OnClick="btnTabEducation_Click" />
            <asp:Button ID="btnTabEmployment" runat="server" Text="Employment History" CssClass="tab-button" OnClick="btnTabEmployment_Click" />
            <asp:Button ID="btnTabReferences" runat="server" Text="References" CssClass="tab-button" OnClick="btnTabReferences_Click" />
            <asp:Button ID="btnTabAuthorization" runat="server" Text="Authorization" CssClass="tab-button" OnClick="btnTabAuthorization_Click" />
        </div>
    </div>

    <!-- Form Container -->
    <div class="tabbed-form-container">
        <asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <!-- Hidden Fields -->
                <asp:HiddenField ID="hfApplicationId" runat="server" />
                <asp:HiddenField ID="hfCurrentTab" runat="server" Value="personal" />

                <!-- Personal Information Tab -->
                <asp:Panel ID="pnlPersonalTab" runat="server" CssClass="tab-content active">
                    <div class="tab-header">
                        <h2><span class="material-icons">person</span>Personal Information</h2>
                        <p>Please provide your personal details and contact information.</p>
                    </div>

                    <div class="form-section">
                        <h3>Basic Information</h3>
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label required">Date</label>
                                <asp:TextBox ID="txtApplicationDate" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label required">Last Name</label>
                                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-input"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" 
                                    ErrorMessage="Last name is required" CssClass="field-validation-error" Display="Dynamic" ValidationGroup="SubmitApplication"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group">
                                <label class="form-label required">First Name</label>
                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-input"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" 
                                    ErrorMessage="First name is required" CssClass="field-validation-error" Display="Dynamic" ValidationGroup="SubmitApplication"></asp:RequiredFieldValidator>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Middle Name</label>
                                <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group full-width">
                                <label class="form-label">Home Address</label>
                                <asp:TextBox ID="txtHomeAddress" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Apt. #</label>
                                <asp:TextBox ID="txtAptNumber" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">City</label>
                                <asp:TextBox ID="txtCity" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">State</label>
                                <asp:TextBox ID="txtState" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Zip Code</label>
                                <asp:TextBox ID="txtZipCode" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Social Security Number</label>
                                <asp:TextBox ID="txtSSN" runat="server" CssClass="form-input" placeholder="___-__-____"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Driver's Lic #</label>
                                <asp:TextBox ID="txtDriversLicense" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">State</label>
                                <asp:TextBox ID="txtDLState" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Phone Number</label>
                                <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-input" placeholder="(___) ____-____"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Cell Number</label>
                                <asp:TextBox ID="txtCellNumber" runat="server" CssClass="form-input" placeholder="(___) ____-____"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="form-section">
                        <h3>Emergency Contact</h3>
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Emergency Contact Person</label>
                                <asp:TextBox ID="txtEmergencyContactName" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Relationship</label>
                                <asp:TextBox ID="txtEmergencyContactRelationship" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group full-width">
                                <label class="form-label">Emergency Contact Address and Phone</label>
                                <asp:TextBox ID="txtEmergencyContactAddress" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Position Information Tab -->
                <asp:Panel ID="pnlPositionTab" runat="server" CssClass="tab-content">
                    <div class="tab-header">
                        <h2><span class="material-icons">work</span>Position Information</h2>
                        <p>Tell us about the position you're applying for and your availability.</p>
                    </div>

                    <div class="form-section">
                        <h3>Position Applied For</h3>
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">1.</label>
                                <asp:TextBox ID="txtPosition1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">2.</label>
                                <asp:TextBox ID="txtPosition2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Salary Desired</label>
                                <asp:TextBox ID="txtSalaryDesired" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Salary Type</label>
                                <div class="checkbox-group">
                                    <asp:RadioButton ID="rbHourly" runat="server" GroupName="SalaryType" Text="Hourly" />
                                    <asp:RadioButton ID="rbYearly" runat="server" GroupName="SalaryType" Text="Yearly" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Available Start Date</label>
                                <asp:TextBox ID="txtAvailableStartDate" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Employment Sought</label>
                                <div class="checkbox-group">
                                    <asp:RadioButton ID="rbFullTime" runat="server" GroupName="EmploymentType" Text="Full Time" />
                                    <asp:RadioButton ID="rbPartTime" runat="server" GroupName="EmploymentType" Text="Part Time" />
                                    <asp:RadioButton ID="rbTemporary" runat="server" GroupName="EmploymentType" Text="Temporary" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="form-section">
                        <h3>Work Location and Schedule Preferences</h3>
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Desired Location to Work</label>
                                <div class="checkbox-group">
                                    <asp:CheckBox ID="chkNashville" runat="server" Text="Nashville" />
                                    <asp:CheckBox ID="chkFranklin" runat="server" Text="Franklin" />
                                    <asp:CheckBox ID="chkShelbyville" runat="server" Text="Shelbyville" />
                                    <asp:CheckBox ID="chkWaynesboro" runat="server" Text="Waynesboro" />
                                    <asp:CheckBox ID="chkOtherLocation" runat="server" Text="Other" />
                                    <asp:TextBox ID="txtOtherLocation" runat="server" CssClass="form-input" Placeholder="Specify other location"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Shift Sought</label>
                                <div class="checkbox-group">
                                    <asp:CheckBox ID="chkFirstShift" runat="server" Text="1st Shift" />
                                    <asp:CheckBox ID="chkSecondShift" runat="server" Text="2nd Shift" />
                                    <asp:CheckBox ID="chkThirdShift" runat="server" Text="3rd Shift" />
                                    <asp:CheckBox ID="chkWeekendsOnly" runat="server" Text="Weekends only" />
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Days Available</label>
                                <div class="checkbox-group">
                                    <asp:CheckBox ID="chkMonday" runat="server" Text="Mon" />
                                    <asp:CheckBox ID="chkTuesday" runat="server" Text="Tues" />
                                    <asp:CheckBox ID="chkWednesday" runat="server" Text="Wed" />
                                    <asp:CheckBox ID="chkThursday" runat="server" Text="Thurs" />
                                    <asp:CheckBox ID="chkFriday" runat="server" Text="Fri" />
                                    <asp:CheckBox ID="chkSaturday" runat="server" Text="Sat" />
                                    <asp:CheckBox ID="chkSunday" runat="server" Text="Sun" />
                                </div>
                            </div>
                        </div>
                        <p class="form-note">*Assignment of days, shifts, and hours are based on company needs without guaranteed permanency</p>
                    </div>
                </asp:Panel>

                <!-- Background Questions Tab -->
                <asp:Panel ID="pnlBackgroundTab" runat="server" CssClass="tab-content">
                    <div class="tab-header">
                        <h2><span class="material-icons">security</span>Background Questions</h2>
                        <p>Please answer all background questions honestly and completely.</p>
                    </div>

                    <div class="form-section">
                        <div class="form-table">
                            <table>
                                <tr>
                                    <td>Have you ever applied for a position with TPA, Inc. before?</td>
                                    <td>
                                        <asp:RadioButton ID="rbAppliedBeforeYes" runat="server" GroupName="AppliedBefore" Text="Yes" />
                                        <asp:RadioButton ID="rbAppliedBeforeNo" runat="server" GroupName="AppliedBefore" Text="No" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>If yes, when?</td>
                                    <td><asp:TextBox ID="txtAppliedBeforeWhen" runat="server" CssClass="form-input"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Have you ever worked for TPA, Inc. before?</td>
                                    <td>
                                        <asp:RadioButton ID="rbWorkedBeforeYes" runat="server" GroupName="WorkedBefore" Text="Yes" />
                                        <asp:RadioButton ID="rbWorkedBeforeNo" runat="server" GroupName="WorkedBefore" Text="No" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>If yes, when?</td>
                                    <td><asp:TextBox ID="txtWorkedBeforeWhen" runat="server" CssClass="form-input"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Do you have any family members employed by TPA, Inc.?</td>
                                    <td>
                                        <asp:RadioButton ID="rbFamilyEmployedYes" runat="server" GroupName="FamilyEmployed" Text="Yes" />
                                        <asp:RadioButton ID="rbFamilyEmployedNo" runat="server" GroupName="FamilyEmployed" Text="No" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>If yes, who?</td>
                                    <td><asp:TextBox ID="txtFamilyEmployedWho" runat="server" CssClass="form-input"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td>Are you a U.S. citizen or Permanent Resident?</td>
                                    <td>
                                        <asp:RadioButton ID="rbUSCitizenYes" runat="server" GroupName="USCitizen" Text="Yes" />
                                        <asp:RadioButton ID="rbUSCitizenNo" runat="server" GroupName="USCitizen" Text="No" />
                                        <span>Alien #</span>
                                        <asp:TextBox ID="txtAlienNumber" runat="server" CssClass="form-input" placeholder="(if no)"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>Or otherwise legally entitled to work in the U.S.A.</td>
                                    <td>
                                        <asp:RadioButton ID="rbLegallyEntitledYes" runat="server" GroupName="LegallyEntitled" Text="Yes" />
                                        <asp:RadioButton ID="rbLegallyEntitledNo" runat="server" GroupName="LegallyEntitled" Text="No" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Are you 18 years or older?</td>
                                    <td>
                                        <asp:RadioButton ID="rbOver18Yes" runat="server" GroupName="Over18" Text="Yes" />
                                        <asp:RadioButton ID="rbOver18No" runat="server" GroupName="Over18" Text="No" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Have you ever served in the U.S. Armed Forces?</td>
                                    <td>
                                        <asp:RadioButton ID="rbArmedForcesYes" runat="server" GroupName="ArmedForces" Text="Yes" />
                                        <asp:RadioButton ID="rbArmedForcesNo" runat="server" GroupName="ArmedForces" Text="No" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Have you ever been convicted of a crime (i.e. misdemeanor or felony)?</td>
                                    <td>
                                        <asp:RadioButton ID="rbConvictedYes" runat="server" GroupName="Convicted" Text="Yes" />
                                        <asp:RadioButton ID="rbConvictedNo" runat="server" GroupName="Convicted" Text="No" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Does your name appear on an abuse registry?</td>
                                    <td>
                                        <asp:RadioButton ID="rbAbuseRegistryYes" runat="server" GroupName="AbuseRegistry" Text="Yes" />
                                        <asp:RadioButton ID="rbAbuseRegistryNo" runat="server" GroupName="AbuseRegistry" Text="No" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Have you ever been found guilty abusing, neglecting, or mistreating individuals?</td>
                                    <td>
                                        <asp:RadioButton ID="rbFoundGuiltyYes" runat="server" GroupName="FoundGuilty" Text="Yes" />
                                        <asp:RadioButton ID="rbFoundGuiltyNo" runat="server" GroupName="FoundGuilty" Text="No" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Has your license and/or certification in any health care profession ever been revoked, suspended, limited, or placed on probation or discipline in any state?</td>
                                    <td>
                                        <asp:RadioButton ID="rbLicenseRevokedYes" runat="server" GroupName="LicenseRevoked" Text="Yes" />
                                        <asp:RadioButton ID="rbLicenseRevokedNo" runat="server" GroupName="LicenseRevoked" Text="No" />
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <!-- Criminal History Details -->
                        <div class="form-row">
                            <div class="form-group full-width">
                                <label class="form-label">If yes, please give details including dates, charges, and dispositions</label>
                                <table class="form-table criminal-history-table">
                                    <thead>
                                        <tr>
                                            <th>DATE</th>
                                            <th>CHARGE</th>
                                            <th>STATUS OR OUTCOME</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td><asp:TextBox ID="txtCriminalDate1" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox></td>
                                            <td><asp:TextBox ID="txtCriminalCharge1" runat="server" CssClass="form-input"></asp:TextBox></td>
                                            <td><asp:TextBox ID="txtCriminalStatus1" runat="server" CssClass="form-input"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:TextBox ID="txtCriminalDate2" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox></td>
                                            <td><asp:TextBox ID="txtCriminalCharge2" runat="server" CssClass="form-input"></asp:TextBox></td>
                                            <td><asp:TextBox ID="txtCriminalStatus2" runat="server" CssClass="form-input"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td><asp:TextBox ID="txtCriminalDate3" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox></td>
                                            <td><asp:TextBox ID="txtCriminalCharge3" runat="server" CssClass="form-input"></asp:TextBox></td>
                                            <td><asp:TextBox ID="txtCriminalStatus3" runat="server" CssClass="form-input"></asp:TextBox></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Education Tab -->
                <asp:Panel ID="pnlEducationTab" runat="server" CssClass="tab-content">
                    <div class="tab-header">
                        <h2><span class="material-icons">school</span>Education</h2>
                        <p>Please provide information about your educational background.</p>
                    </div>

                    <div class="form-section">
                        <h3>Educational Background</h3>
                        <table class="form-table education-table">
                            <thead>
                                <tr>
                                    <th></th>
                                    <th>Elementary School</th>
                                    <th>High School</th>
                                    <th>Undergraduate College/University</th>
                                    <th>Graduate/Professional</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td><strong>School Name and Location</strong></td>
                                    <td><asp:TextBox ID="txtElementarySchool" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtHighSchool" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtUndergraduateSchool" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtGraduateSchool" runat="server" CssClass="form-input"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td><strong>Years Completed (circle)</strong></td>
                                    <td>
                                        <div class="year-checkboxes">
                                            <asp:CheckBox ID="chkElem1" runat="server" Text="1" />
                                            <asp:CheckBox ID="chkElem2" runat="server" Text="2" />
                                            <asp:CheckBox ID="chkElem3" runat="server" Text="3" />
                                            <asp:CheckBox ID="chkElem4" runat="server" Text="4" />
                                            <asp:CheckBox ID="chkElem5" runat="server" Text="5" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="year-checkboxes">
                                            <asp:CheckBox ID="chkHS9" runat="server" Text="9" />
                                            <asp:CheckBox ID="chkHS10" runat="server" Text="10" />
                                            <asp:CheckBox ID="chkHS11" runat="server" Text="11" />
                                            <asp:CheckBox ID="chkHS12" runat="server" Text="12" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="year-checkboxes">
                                            <asp:CheckBox ID="chkUG1" runat="server" Text="1" />
                                            <asp:CheckBox ID="chkUG2" runat="server" Text="2" />
                                            <asp:CheckBox ID="chkUG3" runat="server" Text="3" />
                                            <asp:CheckBox ID="chkUG4" runat="server" Text="4" />
                                            <asp:CheckBox ID="chkUG5" runat="server" Text="5" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="year-checkboxes">
                                            <asp:CheckBox ID="chkGrad1" runat="server" Text="1" />
                                            <asp:CheckBox ID="chkGrad2" runat="server" Text="2" />
                                            <asp:CheckBox ID="chkGrad3" runat="server" Text="3" />
                                            <asp:CheckBox ID="chkGrad4" runat="server" Text="4" />
                                            <asp:CheckBox ID="chkGrad5" runat="server" Text="5" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td><strong>Diploma/Degree</strong></td>
                                    <td>
                                        <asp:RadioButton ID="rbElemDiplomaYes" runat="server" GroupName="ElemDiploma" Text="Yes" />
                                        <asp:RadioButton ID="rbElemDiplomaNo" runat="server" GroupName="ElemDiploma" Text="No" />
                                    </td>
                                    <td>
                                        <asp:RadioButton ID="rbHSDiplomaYes" runat="server" GroupName="HSDiploma" Text="Yes" />
                                        <asp:RadioButton ID="rbHSDiplomaNo" runat="server" GroupName="HSDiploma" Text="No" />
                                    </td>
                                    <td>
                                        <asp:RadioButton ID="rbUGDegreeYes" runat="server" GroupName="UGDegree" Text="Yes" />
                                        <asp:RadioButton ID="rbUGDegreeNo" runat="server" GroupName="UGDegree" Text="No" />
                                    </td>
                                    <td>
                                        <asp:RadioButton ID="rbGradDegreeYes" runat="server" GroupName="GradDegree" Text="Yes" />
                                        <asp:RadioButton ID="rbGradDegreeNo" runat="server" GroupName="GradDegree" Text="No" />
                                    </td>
                                </tr>
                                <tr>
                                    <td><strong>Major/Minor</strong></td>
                                    <td><asp:TextBox ID="txtElemMajor" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtHSMajor" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtUGMajor" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtGradMajor" runat="server" CssClass="form-input"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td><strong>Describe any specialized Training or skills</strong></td>
                                    <td><asp:TextBox ID="txtElemSkills" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtHSSkills" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtUGSkills" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtGradSkills" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2"></asp:TextBox></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>

                    <div class="form-section">
                        <h3>Special Knowledge and Skills</h3>
                        <div class="form-row">
                            <div class="form-group full-width">
                                <label class="form-label">Special knowledge, skills, and abilities you wish considered. Include equipment or machines you operate, computer, languages, laboratory techniques, etc. If applying for secretarial/typist position, indicate typing speed (WPM)</label>
                                <asp:TextBox ID="txtSpecialKnowledge" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="4"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="form-section">
                        <h3>Licenses and Certifications</h3>
                        <table class="form-table">
                            <thead>
                                <tr>
                                    <th>Type of License/Certificate</th>
                                    <th>State</th>
                                    <th>ID Number</th>
                                    <th>Expiration Date</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td><asp:TextBox ID="txtLicenseType1" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtLicenseState1" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtLicenseNumber1" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtLicenseExpiration1" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td><asp:TextBox ID="txtLicenseType2" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtLicenseState2" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtLicenseNumber2" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtLicenseExpiration2" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td><asp:TextBox ID="txtLicenseType3" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtLicenseState3" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtLicenseNumber3" runat="server" CssClass="form-input"></asp:TextBox></td>
                                    <td><asp:TextBox ID="txtLicenseExpiration3" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox></td>
                                </tr>
                            </tbody>
                        </table>

                        <div class="form-row">
                            <div class="form-group full-width">
                                <label class="form-label">List Dept. of Intellectual and Developmental Disabilities (DIDD) training/classes you have:</label>
                                <asp:TextBox ID="txtDIDDTraining" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="3"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Employment History Tab -->
                <asp:Panel ID="pnlEmploymentTab" runat="server" CssClass="tab-content">
                    <div class="tab-header">
                        <h2><span class="material-icons">history</span>Employment Experience</h2>
                        <p>Start with your present or last job. Include any job-related military service assignments and volunteer activities that have given you experience related to your job. Please explain any extended lapses between employments.</p>
                    </div>

                    <!-- Employment History 1 -->
                    <div class="form-section">
                        <h3>Most Recent Employment</h3>
                        <div class="employment-grid">
                            <div class="form-group">
                                <label class="form-label">Employer</label>
                                <asp:TextBox ID="txtEmployer1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Dates Employed From/To</label>
                                <div style="display: flex; gap: 0.5rem;">
                                    <asp:TextBox ID="txtEmploymentFrom1" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                                    <span>to</span>
                                    <asp:TextBox ID="txtEmploymentTo1" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Job Title</label>
                                <asp:TextBox ID="txtJobTitle1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Supervisor</label>
                                <asp:TextBox ID="txtSupervisor1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="employment-full-row">
                                <label class="form-label">Address</label>
                                <asp:TextBox ID="txtEmployerAddress1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">City, State, Zip Code</label>
                                <asp:TextBox ID="txtEmployerCityStateZip1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Telephone Number(s)</label>
                                <asp:TextBox ID="txtEmployerPhone1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Hourly Rate of Pay - Starting</label>
                                <asp:TextBox ID="txtStartingPay1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Final</label>
                                <asp:TextBox ID="txtFinalPay1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="employment-full-row">
                                <label class="form-label">Title/Work Performed</label>
                                <asp:TextBox ID="txtWorkPerformed1" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Are you still employed (Yes or No)</label>
                                <div>
                                    <asp:RadioButton ID="rbStillEmployed1Yes" runat="server" GroupName="StillEmployed1" Text="Yes" />
                                    <asp:RadioButton ID="rbStillEmployed1No" runat="server" GroupName="StillEmployed1" Text="No" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Eligible for Rehire (Yes or No)</label>
                                <div>
                                    <asp:RadioButton ID="rbEligibleRehire1Yes" runat="server" GroupName="EligibleRehire1" Text="Yes" />
                                    <asp:RadioButton ID="rbEligibleRehire1No" runat="server" GroupName="EligibleRehire1" Text="No" />
                                </div>
                            </div>
                            <div class="employment-full-row">
                                <label class="form-label">Reason for Leaving</label>
                                <asp:TextBox ID="txtReasonLeaving1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <!-- Employment History 2 -->
                    <div class="form-section">
                        <h3>Previous Employment</h3>
                        <div class="employment-grid">
                            <div class="form-group">
                                <label class="form-label">Employer</label>
                                <asp:TextBox ID="txtEmployer2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Dates Employed From/To</label>
                                <div style="display: flex; gap: 0.5rem;">
                                    <asp:TextBox ID="txtEmploymentFrom2" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                                    <span>to</span>
                                    <asp:TextBox ID="txtEmploymentTo2" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Job Title</label>
                                <asp:TextBox ID="txtJobTitle2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Supervisor</label>
                                <asp:TextBox ID="txtSupervisor2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="employment-full-row">
                                <label class="form-label">Address</label>
                                <asp:TextBox ID="txtEmployerAddress2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">City, State, Zip Code</label>
                                <asp:TextBox ID="txtEmployerCityStateZip2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Telephone Number(s)</label>
                                <asp:TextBox ID="txtEmployerPhone2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Hourly Rate of Pay - Starting</label>
                                <asp:TextBox ID="txtStartingPay2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Final</label>
                                <asp:TextBox ID="txtFinalPay2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="employment-full-row">
                                <label class="form-label">Title/Work Performed</label>
                                <asp:TextBox ID="txtWorkPerformed2" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Eligible for Rehire (Yes or No)</label>
                                <div>
                                    <asp:RadioButton ID="rbEligibleRehire2Yes" runat="server" GroupName="EligibleRehire2" Text="Yes" />
                                    <asp:RadioButton ID="rbEligibleRehire2No" runat="server" GroupName="EligibleRehire2" Text="No" />
                                </div>
                            </div>
                            <div class="employment-full-row">
                                <label class="form-label">Reason for Leaving</label>
                                <asp:TextBox ID="txtReasonLeaving2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <!-- Employment History 3 -->
                    <div class="form-section">
                        <h3>Additional Previous Employment</h3>
                        <div class="employment-grid">
                            <div class="form-group">
                                <label class="form-label">Employer</label>
                                <asp:TextBox ID="txtEmployer3" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Dates Employed From/To</label>
                                <div style="display: flex; gap: 0.5rem;">
                                    <asp:TextBox ID="txtEmploymentFrom3" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                                    <span>to</span>
                                    <asp:TextBox ID="txtEmploymentTo3" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Job Title</label>
                                <asp:TextBox ID="txtJobTitle3" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Supervisor</label>
                                <asp:TextBox ID="txtSupervisor3" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="employment-full-row">
                                <label class="form-label">Address</label>
                                <asp:TextBox ID="txtEmployerAddress3" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">City, State, Zip Code</label>
                                <asp:TextBox ID="txtEmployerCityStateZip3" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Telephone Number(s)</label>
                                <asp:TextBox ID="txtEmployerPhone3" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Hourly Rate of Pay - Starting</label>
                                <asp:TextBox ID="txtStartingPay3" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Final</label>
                                <asp:TextBox ID="txtFinalPay3" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="employment-full-row">
                                <label class="form-label">Title/Work Performed</label>
                                <asp:TextBox ID="txtWorkPerformed3" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Eligible for Rehire (Yes or No)</label>
                                <div>
                                    <asp:RadioButton ID="rbEligibleRehire3Yes" runat="server" GroupName="EligibleRehire3" Text="Yes" />
                                    <asp:RadioButton ID="rbEligibleRehire3No" runat="server" GroupName="EligibleRehire3" Text="No" />
                                </div>
                            </div>
                            <div class="employment-full-row">
                                <label class="form-label">Reason for Leaving</label>
                                <asp:TextBox ID="txtReasonLeaving3" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- References Tab -->
                <asp:Panel ID="pnlReferencesTab" runat="server" CssClass="tab-content">
                    <div class="tab-header">
                        <h2><span class="material-icons">contacts</span>Request for Professional References</h2>
                        <p>To further process your application, please provide three (3) personal references who can provide professional reference about your character, ability and suitability for the position you have applied for.</p>
                        <p><em>*At least one (1) personal reference must have known you for at least 5 years</em></p>
                    </div>

                    <!-- Reference 1 -->
                    <div class="form-section">
                        <h3>Professional Reference #1</h3>
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">First and last name:</label>
                                <asp:TextBox ID="txtReference1Name" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Phone number:</label>
                                <asp:TextBox ID="txtReference1Phone" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">E-mail address:</label>
                                <asp:TextBox ID="txtReference1Email" runat="server" CssClass="form-input" TextMode="Email"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">How many years have you known personal reference?</label>
                                <asp:TextBox ID="txtReference1Years" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <!-- Reference 2 -->
                    <div class="form-section">
                        <h3>Professional Reference #2</h3>
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">First and last name:</label>
                                <asp:TextBox ID="txtReference2Name" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Phone number:</label>
                                <asp:TextBox ID="txtReference2Phone" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">E-mail address:</label>
                                <asp:TextBox ID="txtReference2Email" runat="server" CssClass="form-input" TextMode="Email"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">How many years have you known personal reference?</label>
                                <asp:TextBox ID="txtReference2Years" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <!-- Reference 3 -->
                    <div class="form-section">
                        <h3>Professional Reference #3</h3>
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">First and last name:</label>
                                <asp:TextBox ID="txtReference3Name" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Phone number:</label>
                                <asp:TextBox ID="txtReference3Phone" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">E-mail address:</label>
                                <asp:TextBox ID="txtReference3Email" runat="server" CssClass="form-input" TextMode="Email"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">How many years have you known personal reference?</label>
                                <asp:TextBox ID="txtReference3Years" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Authorization Tab -->
                <asp:Panel ID="pnlAuthorizationTab" runat="server" CssClass="tab-content">
                    <div class="tab-header">
                        <h2><span class="material-icons">verified_user</span>Background Check Authorization</h2>
                        <p>Please complete the following authorization forms for background investigation.</p>
                    </div>

                    <!-- Background Investigation Form -->
                    <div class="form-section">
                        <div class="authorization-header">
                            <div style="display: flex; justify-content: space-between; margin-bottom: 1rem;">
                                <div>
                                    <strong>Background Investigation Requested By:</strong><br />
                                    Tennessee Personal Assistance, Inc. – Nashville<br />
                                    475 Metroplex<br />
                                    Nashville, TN 37211
                                </div>
                                <div>
                                    <strong>Background Investigation Compiled By:</strong><br />
                                    Powlers' Profile Links, Inc.<br />
                                    P. O. Box 291043<br />
                                    Nashville, TN 37229-1043
                                </div>
                            </div>
                            <h3 class="text-center">Tennessee Personal Assistance, Inc. - Nashville</h3>
                            <h4 class="text-center">DISCLOSURE AND AUTHORIZATION FORM</h4>
                            <h4 class="text-center">(1) BACKGROUND INVESTIGATION QUESTIONNAIRE:</h4>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Name:</label>
                                <div style="display: flex; gap: 1rem;">
                                    <asp:TextBox ID="txtBGLastName" runat="server" CssClass="form-input" placeholder="(Last)"></asp:TextBox>
                                    <asp:TextBox ID="txtBGFirstName" runat="server" CssClass="form-input" placeholder="(First)"></asp:TextBox>
                                    <asp:TextBox ID="txtBGMiddleName" runat="server" CssClass="form-input" placeholder="(Middle Name)"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group full-width">
                                <label class="form-label">Address:</label>
                                <div style="display: flex; gap: 1rem; flex-wrap: wrap;">
                                    <asp:TextBox ID="txtBGStreet" runat="server" CssClass="form-input" placeholder="(Street)" style="flex: 2;"></asp:TextBox>
                                    <asp:TextBox ID="txtBGCity" runat="server" CssClass="form-input" placeholder="(City)" style="flex: 1;"></asp:TextBox>
                                    <asp:TextBox ID="txtBGState" runat="server" CssClass="form-input" placeholder="(State)" style="flex: 0 0 100px;"></asp:TextBox>
                                    <asp:TextBox ID="txtBGZipCode" runat="server" CssClass="form-input" placeholder="(Zip Code)" style="flex: 0 0 120px;"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Social Security Number:</label>
                                <asp:TextBox ID="txtBGSSN" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Telephone Number:</label>
                                <asp:TextBox ID="txtBGPhone" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Other Name (s):</label>
                                <asp:TextBox ID="txtBGOtherName" runat="server" CssClass="form-input" placeholder="(Used Within the Last 7YRS. E.g. Maiden, Other Married Names)"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Year of Name Change</label>
                                <asp:TextBox ID="txtBGNameChangeYear" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Driver's License Number:</label>
                                <asp:TextBox ID="txtBGDriversLicense" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">State</label>
                                <asp:TextBox ID="txtBGDLState" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Date of Birth:</label>
                                <asp:TextBox ID="txtBGDateOfBirth" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group full-width">
                                <label class="form-label">Name on Driver's License:</label>
                                <asp:TextBox ID="txtBGNameOnLicense" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <!-- Previous Addresses -->
                    <div class="form-section">
                        <h3>Previous Residential Addresses (Previous 7 years):</h3>
                        
                        <!-- Former Address 1 -->
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Former Address:</label>
                                <asp:TextBox ID="txtFormerAddress1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Street</label>
                                <asp:TextBox ID="txtFormerStreet1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">City</label>
                                <asp:TextBox ID="txtFormerCity1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">State</label>
                                <asp:TextBox ID="txtFormerState1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Years Resided</label>
                                <asp:TextBox ID="txtFormerYears1" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>

                        <!-- Former Address 2 -->
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Former Address:</label>
                                <asp:TextBox ID="txtFormerAddress2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Street</label>
                                <asp:TextBox ID="txtFormerStreet2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">City</label>
                                <asp:TextBox ID="txtFormerCity2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">State</label>
                                <asp:TextBox ID="txtFormerState2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Years Resided</label>
                                <asp:TextBox ID="txtFormerYears2" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>

                        <!-- Former Address 3 -->
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Former Address:</label>
                                <asp:TextBox ID="txtFormerAddress3" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label class="form-label">Street</label>
                                <asp:TextBox ID="txtFormerStreet3" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">City</label>
                                <asp:TextBox ID="txtFormerCity3" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">State</label>
                                <asp:TextBox ID="txtFormerState3" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Years Resided</label>
                                <asp:TextBox ID="txtFormerYears3" runat="server" CssClass="form-input"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <!-- Additional Background Questions -->
                    <div class="form-section">
                        <h3>Additional Background Questions</h3>
                        <div class="form-table">
                            <table>
                                <tr>
                                    <td>Have you been convicted of any criminal offense, either misdemeanor or felony, other than minor traffic violations in the last 7 years?</td>
                                    <td>
                                        <asp:RadioButton ID="rbConvicted7YearsYes" runat="server" GroupName="Convicted7Years" Text="Yes" />
                                        <asp:RadioButton ID="rbConvicted7YearsNo" runat="server" GroupName="Convicted7Years" Text="No" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>Are you currently charged or under investigation for any violation of the law other than minor traffic violations?</td>
                                    <td>
                                        <asp:RadioButton ID="rbChargedInvestigationYes" runat="server" GroupName="ChargedInvestigation" Text="Yes" />
                                        <asp:RadioButton ID="rbChargedInvestigationNo" runat="server" GroupName="ChargedInvestigation" Text="No" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </div>

                    <!-- DIDD Authorization -->
                    <div class="form-section">
                        <h3>AUTHORIZATION AND GENERAL RELEASE FOR DIDD, BUREAU OF TENNCARE & TPA, INC</h3>
                        <div class="authorization-content">
                            <p><strong>I, the undersigned applicant certify and affirm that, to the best of my knowledge and belief:</strong></p>
                            <div class="form-row">
                                <div class="form-group">
                                    <asp:CheckBox ID="chkDIDDNoAbuse" runat="server" Text="I have NOT had a case of abuse, neglect, mistreatment or exploitation substantiated against me" />
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="form-group">
                                    <asp:CheckBox ID="chkDIDDHadAbuse" runat="server" Text="I have had a case of abuse, neglect, mistreatment or exploitation substantiated against me" />
                                </div>
                            </div>
                            
                            <p>As a condition of submitting this application and in order to verify this affirmation, I further release and authorize Tennessee Personal Assistance, the Tennessee Department of Intellectual and Developmental Disabilities and the Bureau of TennCare to have full and complete access to any and all current or prior personnel or investigative records, from any party, person, business entity or agency, whether governmental or non-governmental, as pertains to any allegations against me of abuse, neglect, mistreatment or exploitation and to consider this information as may be deemed appropriate. This authorization extends to providing any applicable information in personnel or investigative reports concerning my employment with this employer to my future employers who may be Providers of DIDD Services</p>

                            <div class="form-row">
                                <div class="form-group">
                                    <label class="form-label">Full Name (Last, First, Middle):</label>
                                    <asp:TextBox ID="txtDIDDFullName" runat="server" CssClass="form-input"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="form-group">
                                    <label class="form-label">SSN #:</label>
                                    <asp:TextBox ID="txtDIDDSSN" runat="server" CssClass="form-input"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label">Date of Birth:</label>
                                    <asp:TextBox ID="txtDIDDDateOfBirth" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="form-group">
                                    <label class="form-label">Driver License or ID #</label>
                                    <asp:TextBox ID="txtDIDDDriversLicense" runat="server" CssClass="form-input"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label">Witness</label>
                                    <asp:TextBox ID="txtDIDDWitness" runat="server" CssClass="form-input"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Protection from Harm Statement -->
                    <div class="form-section">
                        <h3>PROTECTION FROM HARM STATEMENT</h3>
                        <div class="authorization-content">
                            <p><strong>I certify and affirm that, to the best of my knowledge and belief:</strong></p>
                            <div class="form-row">
                                <div class="form-group">
                                    <asp:CheckBox ID="chkProtectionNoAbuse" runat="server" Text="I have NOT had a case of abuse, neglect, mistreatment or exploitation substantiated against me" />
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="form-group">
                                    <asp:CheckBox ID="chkProtectionHadAbuse" runat="server" Text="I have had a case of abuse, neglect, mistreatment or exploitation substantiated against me" />
                                </div>
                            </div>
                            
                            <p>In order to verify this affirmation, I further release and authorize Tennessee Personal Assistance, the Tennessee Department of Intellectual and Developmental Disabilities and the Bureau of TennCare to have full and complete access to any and all current or prior personnel or investigative records as pertains to substantiated allegations against me of abuse, neglect, mistreatment or exploitation.</p>

                            <div class="form-row">
                                <div class="form-group">
                                    <label class="form-label">Witness:</label>
                                    <asp:TextBox ID="txtProtectionWitness" runat="server" CssClass="form-input"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Pre-Employment Reference Check Authorization -->
                    <div class="form-section">
                        <h3>PRE-EMPLOYMENT REFERENCE CHECK RELEASE & AUTHORIZATION TO CONDUCT REFERENCE CHECKS</h3>
                        <div class="authorization-content">
                            <p>The person named below has applied for a position with our company. Their consideration for employment is largely dependent on this reference form. Below is a signed authorization and consent from the applicant for our company to obtain reference information. Your prompt cooperation, time and attention in completing this reference will be greatly appreciated.</p>

                            <h4>Applicant's Authorization, Release and Request for Reference Information</h4>
                            <p>I <asp:TextBox ID="txtReferenceAuthName" runat="server" CssClass="form-input inline-input" style="width: 300px; display: inline;"></asp:TextBox> have applied for a position with TPA, Inc. I authorize all my current and former employers to provide reference information, including my job performance, my work record and attendance, the reason(s) for my leaving, my eligibility for rehire and my suitability for the position I am now seeking. I encourage my current and former employers to provide complete information to requests for information, which is believed to be true but not documented. I realize some information may be complimentary and some may be critical. I promise I will not bring any legal claims or actions against my current or former employer due to the response to job reference requests. I recognize this is also a State Statute; which provide my employers with certain protection from such claims. I realize no one is required to give a reference, so I make this commitment to encourage the free exchange of reference information.</p>

                            <p>I signed this release voluntarily and was not required to do so as part of the application process.</p>

                            <div class="form-row">
                                <div class="form-group">
                                    <label class="form-label">SSN # XXX-XX-</label>
                                    <asp:TextBox ID="txtSSNLast4" runat="server" CssClass="form-input" MaxLength="4" placeholder="(last 4 digits only)"></asp:TextBox>
                                </div>
                            </div>
                            <p class="form-note">(Please note: For legal compliance and to protect our applicants, a full SSN is not provided. If a full SSN is needed, please call HR @ 615-331-6200)</p>

                            <div class="form-row">
                                <div class="form-group">
                                    <label class="form-label">Applicant's Signature</label>
                                    <asp:TextBox ID="txtApplicantSignature" runat="server" CssClass="form-input"></asp:TextBox>
                                </div>
                                <div class="form-group">
                                    <label class="form-label">Date</label>
                                    <asp:TextBox ID="txtSignatureDate" runat="server" CssClass="form-input" TextMode="Date"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Final Acknowledgment -->
                    <div class="form-section">
                        <h3>Final Acknowledgment</h3>
                        <div class="form-row">
                            <div class="form-group full-width">
                                <div class="final-acknowledgment-container">
                                    <asp:CheckBox ID="chkFinalAcknowledgment" runat="server" CssClass="required-checkbox" />
                                    <label for="<%= chkFinalAcknowledgment.ClientID %>" class="checkbox-label">
                                        <strong>I acknowledge that I have read and understand all sections of this employment application and authorize TPA, Inc. to conduct reference checks and background investigations as described above. I certify that all information provided is true and complete to the best of my knowledge.</strong>
                                    </label>
                                </div>
                                <asp:CustomValidator ID="cvFinalAcknowledgment" runat="server" 
                                    ErrorMessage="You must acknowledge the final statement to submit your application" 
                                    CssClass="field-validation-error" Display="Dynamic" ValidationGroup="SubmitApplication"
                                    ClientValidationFunction="validateFinalAcknowledgment" OnServerValidate="cvFinalAcknowledgment_ServerValidate"></asp:CustomValidator>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Form Navigation -->
                <div class="form-navigation">
                    <div class="nav-buttons">
                        <asp:Button ID="btnPrevious" runat="server" Text="Previous" CssClass="btn btn-secondary" OnClick="btnPrevious_Click" />
                        <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="btn btn-primary" OnClick="btnNext_Click" />
                        <asp:Button ID="btnSubmitApplication" runat="server" Text="Submit Application" CssClass="btn btn-success" OnClick="btnSubmitApplication_Click" ValidationGroup="SubmitApplication" Visible="false" />
                    </div>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <!-- Legal Notice Footer -->
    <div class="legal-notice">
        <h4>TPA, Inc. is an Equal Opportunity Employer</h4>
        <p>Tennessee Law Prohibits Discrimination in Employment: It is illegal to discriminate against any person because of race, color, creed, religion, sex, age, handicap, or national origin in recruitment, training, hiring, discharge, promotion, or any condition, term or privilege of employment.</p>
    </div>

    <script type="text/javascript">
        // Checkbox validation function
        function validateFinalAcknowledgment(sender, args) {
            var checkbox = document.getElementById('<%= chkFinalAcknowledgment.ClientID %>');
            args.IsValid = checkbox.checked;
        }

        // Auto-populate background form fields from personal info
        $(document).ready(function () {
            // Copy personal info to background section when those fields change
            $('#<%= txtFirstName.ClientID %>').on('blur', function () {
                $('#<%= txtBGFirstName.ClientID %>').val($(this).val());
                // Also update reference authorization name
                var fullName = $(this).val() + ' ' + $('#<%= txtLastName.ClientID %>').val();
                $('#<%= txtReferenceAuthName.ClientID %>').val(fullName.trim());
            });

            $('#<%= txtLastName.ClientID %>').on('blur', function () {
                $('#<%= txtBGLastName.ClientID %>').val($(this).val());
                // Also update reference authorization name
                var fullName = $('#<%= txtFirstName.ClientID %>').val() + ' ' + $(this).val();
                $('#<%= txtReferenceAuthName.ClientID %>').val(fullName.trim());
                // Update DIDD full name
                $('#<%= txtDIDDFullName.ClientID %>').val(fullName.trim());
            });

            $('#<%= txtMiddleName.ClientID %>').on('blur', function () {
                $('#<%= txtBGMiddleName.ClientID %>').val($(this).val());
            });

            $('#<%= txtHomeAddress.ClientID %>').on('blur', function () {
                $('#<%= txtBGStreet.ClientID %>').val($(this).val());
            });

            $('#<%= txtCity.ClientID %>').on('blur', function () {
                $('#<%= txtBGCity.ClientID %>').val($(this).val());
            });
            
            $('#<%= txtState.ClientID %>').on('blur', function() {
                $('#<%= txtBGState.ClientID %>').val($(this).val());
            });
            
            $('#<%= txtZipCode.ClientID %>').on('blur', function() {
                $('#<%= txtBGZipCode.ClientID %>').val($(this).val());
            });
            
            $('#<%= txtSSN.ClientID %>').on('blur', function() {
                $('#<%= txtBGSSN.ClientID %>').val($(this).val());
                $('#<%= txtDIDDSSN.ClientID %>').val($(this).val());
                // Auto-fill last 4 digits
                var ssn = $(this).val().replace(/\D/g, '');
                if (ssn.length >= 4) {
                    $('#<%= txtSSNLast4.ClientID %>').val(ssn.slice(-4));
                }
            });
            
            $('#<%= txtPhoneNumber.ClientID %>').on('blur', function() {
                $('#<%= txtBGPhone.ClientID %>').val($(this).val());
            });
            
            $('#<%= txtDriversLicense.ClientID %>').on('blur', function() {
                $('#<%= txtBGDriversLicense.ClientID %>').val($(this).val());
                $('#<%= txtDIDDDriversLicense.ClientID %>').val($(this).val());
            });
            
            $('#<%= txtDLState.ClientID %>').on('blur', function() {
                $('#<%= txtBGDLState.ClientID %>').val($(this).val());
            });

            // Set today's date for application date and signature date
            var today = new Date().toISOString().split('T')[0];
            $('#<%= txtApplicationDate.ClientID %>').val(today);
            $('#<%= txtSignatureDate.ClientID %>').val(today);
        });

        // Save draft functionality
        function saveDraft() {
            __doPostBack('<%= btnSaveDraft.UniqueID %>', '');
        }

        // Auto-save every 5 minutes
        setInterval(saveDraft, 300000);
    </script>
</asp:Content>