<%@ Page Title="New Hire Paperwork - Employment Application" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="NewHirePaperWork.aspx.cs" Inherits="TPASystem2.OnBoarding.NewHirePaperwork" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">


    <style>

        /* Employment Application Specific Styles - Add to tpa-common.css */

/* Application Header */
.application-header {
    text-align: center;
    margin-bottom: 30px;
    padding: 20px;
    border-bottom: 3px solid #2c3e50;
}

.application-title {
    font-size: 28px;
    font-weight: bold;
    color: #2c3e50;
    margin: 0 0 15px 0;
    text-transform: uppercase;
    letter-spacing: 1px;
}

.application-subtitle {
    font-size: 14px;
    color: #555;
    line-height: 1.6;
    max-width: 800px;
    margin: 0 auto;
}

/* Form Layout Extensions */
.form-col-wide {
    flex: 2;
    min-width: 300px;
}

.form-col-narrow {
    flex: 0.5;
    min-width: 120px;
}

.form-col-auto {
    flex: none;
    width: auto;
}

.form-col-full {
    flex: 1 1 100%;
}

/* Form Input Variants */
.form-input-inline {
    display: inline-block;
    width: auto;
    min-width: 150px;
    margin-left: 10px;
}

.form-input-small {
    width: 80px;
    display: inline-block;
    margin: 0 5px;
}

.form-input-date {
    width: 140px;
    display: inline-block;
}

.form-input-full {
    width: 100%;
    resize: vertical;
}

.form-input-table {
    width: 100%;
    padding: 8px;
    border: 1px solid #ccc;
    font-size: 13px;
}

.date-input {
    width: 150px;
}

/* Specialized Input Groups */
.date-range {
    display: flex;
    align-items: center;
    gap: 10px;
}

.date-separator {
    font-weight: bold;
    color: #666;
}

.pay-range {
    display: flex;
    align-items: center;
    gap: 10px;
    flex-wrap: wrap;
}

.pay-range span {
    font-size: 13px;
    color: #666;
}

/* Radio and Checkbox Enhancements */
.radio-small {
    font-size: 12px;
}

.checkbox-large input {
    transform: scale(1.2);
    margin-right: 8px;
}

/* Specialized Option Layouts */
.location-options {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
    gap: 10px;
}

.days-available {
    display: grid;
    grid-template-columns: repeat(7, 1fr);
    gap: 10px;
}

.other-location {
    margin-top: 10px;
    display: flex;
    align-items: center;
    gap: 10px;
}

.assignment-note {
    margin-top: 10px;
    font-style: italic;
    color: #666;
}

.conditional-field {
    margin-top: 10px;
    display: flex;
    align-items: center;
    gap: 10px;
}

/* Application-Specific Tables */
.criminal-history-table, .education-table, .licenses-table {
    width: 100%;
    border-collapse: collapse;
    margin: 15px 0;
    background: #fff;
}

.criminal-history-table th, .criminal-history-table td,
.education-table th, .education-table td,
.licenses-table th, .licenses-table td {
    border: 1px solid #ddd;
    padding: 10px;
    text-align: left;
    vertical-align: top;
}

.criminal-history-table th, .education-table th, .licenses-table th {
    background-color: #f8f9fa;
    font-weight: bold;
    color: #2c3e50;
}

/* Education Table Specific */
.education-table-container, .licenses-table-container {
    overflow-x: auto;
}

.school-name-cell {
    width: 200px;
    vertical-align: top;
}

.school-input {
    margin-bottom: 10px;
    width: 100%;
}

.years-input {
    width: 60px;
    text-align: center;
}

.years-row {
    background-color: #f0f0f0;
    font-size: 12px;
}

.years-label {
    text-align: center;
    font-style: italic;
    color: #666;
}

.diploma-label, .major-label, .specialized-training-label {
    background-color: #f8f9fa;
    font-weight: bold;
    text-align: center;
}

/* Employment and Reference Blocks */
.employment-block, .reference-block {
    background: #fff;
    border: 1px solid #ddd;
    border-radius: 6px;
    padding: 20px;
    margin-bottom: 25px;
}

.employment-block-title, .reference-title {
    font-size: 16px;
    font-weight: bold;
    color: #2c3e50;
    margin: 0 0 15px 0;
}

.employment-block-title {
    margin-bottom: 20px;
    padding-bottom: 10px;
    border-bottom: 1px solid #eee;
}

.reference-note {
    font-style: italic;
    color: #e74c3c;
    font-size: 14px;
    margin: 10px 0;
}

/* Acknowledgment Section */
.acknowledgment-text {
    background: #f8f9fa;
    border: 1px solid #ddd;
    border-radius: 4px;
    padding: 20px;
    margin-bottom: 25px;
}

.acknowledgment-text p {
    margin: 0 0 15px 0;
    line-height: 1.6;
}

.acknowledgment-text p:last-child {
    margin-bottom: 0;
}

.checkbox-group {
    margin: 20px 0;
}

.checkbox-label {
    display: block;
    margin-left: 30px;
    line-height: 1.6;
    color: #333;
}

/* Footer */
.equal-opportunity-footer {
    background: #f8f9fa;
    border: 1px solid #ddd;
    border-radius: 6px;
    padding: 20px;
    margin-top: 30px;
    text-align: center;
}

.equal-opportunity-footer p {
    margin: 0 0 10px 0;
    font-size: 14px;
    line-height: 1.5;
}

.equal-opportunity-footer p:last-child {
    margin-bottom: 0;
}

/* Responsive Design for Application-Specific Elements */
@media (max-width: 768px) {
    .form-col-wide, .form-col-narrow {
        flex: 1 1 100%;
        min-width: auto;
    }
    
    .location-options {
        grid-template-columns: 1fr;
    }
    
    .days-available {
        grid-template-columns: repeat(3, 1fr);
    }
    
    .date-range, .pay-range {
        flex-direction: column;
        align-items: flex-start;
    }
    
    .conditional-field, .other-location {
        flex-direction: column;
        align-items: flex-start;
        gap: 5px;
    }
    
    .application-title {
        font-size: 22px;
    }
}

@media (max-width: 480px) {
    .application-title {
        font-size: 18px;
    }
}
    </style>

    <!-- Page Header -->
    <div class="mandatory-task-header">
        <div class="progress-tracker">
            <div class="progress-step">
                <i class="material-icons">assignment</i>
                Mandatory Task 1 of 3
            </div>
        </div>
        <h1 class="task-title">
            <i class="material-icons">description</i>
            New Hire Paperwork - Employment Application
            <span class="mandatory-badge">Mandatory</span>
        </h1>
        <p class="task-subtitle">Complete your employment application form with all required information</p>
    </div>

    <!-- Success Message -->
    <asp:Panel ID="pnlSuccessMessage" runat="server" CssClass="success-message" Visible="false">
        <i class="material-icons" style="vertical-align: middle; margin-right: 0.5rem;">check_circle</i>
        <strong>Employment application completed successfully!</strong> You will be redirected to your onboarding dashboard.
    </asp:Panel>

    <!-- Error Messages -->
    <asp:Panel ID="pnlMessages" runat="server" Visible="false" CssClass="message-panel">
        <asp:Label ID="lblMessage" runat="server" CssClass="message-text"></asp:Label>
    </asp:Panel>

    <!-- Form Container -->
    <div class="form-container">
        <!-- Application Date -->
        <div class="form-section">
            <div class="form-row">
                <div class="form-col-auto">
                    <div class="form-group">
                        <label class="form-label">Date</label>
                        <asp:TextBox ID="txtApplicationDate" runat="server" CssClass="form-input date-input" 
                            TextMode="Date" Enabled="false" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Personal Information Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">person</i>
                </div>
                <h2 class="section-title">Personal Information</h2>
            </div>

            <!-- Name Fields -->
            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required">Last Name</label>
                        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-input" MaxLength="50" />
                        <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" 
                            ErrorMessage="Last name is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required">First Name</label>
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-input" MaxLength="50" />
                        <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" 
                            ErrorMessage="First name is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label">Middle Name</label>
                        <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-input" MaxLength="50" />
                    </div>
                </div>
            </div>

            <!-- Address Information -->
            <div class="form-row">
                <div class="form-col-wide">
                    <div class="form-group">
                        <label class="form-label required">Home Address</label>
                        <asp:TextBox ID="txtHomeAddress" runat="server" CssClass="form-input" MaxLength="200" />
                        <asp:RequiredFieldValidator ID="rfvHomeAddress" runat="server" ControlToValidate="txtHomeAddress" 
                            ErrorMessage="Home address is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col-narrow">
                    <div class="form-group">
                        <label class="form-label">Apt. #</label>
                        <asp:TextBox ID="txtAptNumber" runat="server" CssClass="form-input" MaxLength="20" />
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required">City</label>
                        <asp:TextBox ID="txtCity" runat="server" CssClass="form-input" MaxLength="100" />
                        <asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="txtCity" 
                            ErrorMessage="City is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required">State</label>
                        <asp:TextBox ID="txtState" runat="server" CssClass="form-input" MaxLength="50" />
                        <asp:RequiredFieldValidator ID="rfvState" runat="server" ControlToValidate="txtState" 
                            ErrorMessage="State is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required">Zip Code</label>
                        <asp:TextBox ID="txtZipCode" runat="server" CssClass="form-input" MaxLength="10" />
                        <asp:RequiredFieldValidator ID="rfvZipCode" runat="server" ControlToValidate="txtZipCode" 
                            ErrorMessage="Zip code is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
            </div>

            <!-- SSN and Driver's License -->
            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required">Social Security Number</label>
                        <asp:TextBox ID="txtSSN" runat="server" CssClass="form-input" MaxLength="11" placeholder="___-__-____" />
                        <asp:RequiredFieldValidator ID="rfvSSN" runat="server" ControlToValidate="txtSSN" 
                            ErrorMessage="Social Security Number is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label">Driver's Lic #</label>
                        <asp:TextBox ID="txtDriversLicense" runat="server" CssClass="form-input" MaxLength="50" />
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label">State</label>
                        <asp:TextBox ID="txtDLState" runat="server" CssClass="form-input" MaxLength="10" />
                    </div>
                </div>
            </div>

            <!-- Phone Numbers -->
            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required">Phone Number</label>
                        <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-input" MaxLength="20" placeholder="(___) ___-____" />
                        <asp:RequiredFieldValidator ID="rfvPhoneNumber" runat="server" ControlToValidate="txtPhoneNumber" 
                            ErrorMessage="Phone number is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label">Cell Number</label>
                        <asp:TextBox ID="txtCellNumber" runat="server" CssClass="form-input" MaxLength="20" placeholder="(___) ___-____" />
                    </div>
                </div>
            </div>

            <!-- Emergency Contact -->
            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required">Emergency Contact Person</label>
                        <asp:TextBox ID="txtEmergencyContactName" runat="server" CssClass="form-input" MaxLength="100" />
                        <asp:RequiredFieldValidator ID="rfvEmergencyContactName" runat="server" ControlToValidate="txtEmergencyContactName" 
                            ErrorMessage="Emergency contact name is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required">Relationship</label>
                        <asp:TextBox ID="txtEmergencyContactRelationship" runat="server" CssClass="form-input" MaxLength="50" />
                        <asp:RequiredFieldValidator ID="rfvEmergencyContactRelationship" runat="server" ControlToValidate="txtEmergencyContactRelationship" 
                            ErrorMessage="Emergency contact relationship is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="form-col-wide">
                    <div class="form-group">
                        <label class="form-label">Emergency Contact Address and phone:</label>
                        <asp:TextBox ID="txtEmergencyContactAddress" runat="server" CssClass="form-input" MaxLength="300" />
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required">Emergency Contact Phone</label>
                        <asp:TextBox ID="txtEmergencyContactPhone" runat="server" CssClass="form-input" MaxLength="20" />
                        <asp:RequiredFieldValidator ID="rfvEmergencyContactPhone" runat="server" ControlToValidate="txtEmergencyContactPhone" 
                            ErrorMessage="Emergency contact phone is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Position Applied For Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">work</i>
                </div>
                <h2 class="section-title">Position Applied For</h2>
            </div>

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required">1.</label>
                        <asp:TextBox ID="txtPosition1" runat="server" CssClass="form-input" MaxLength="100" />
                        <asp:RequiredFieldValidator ID="rfvPosition1" runat="server" ControlToValidate="txtPosition1" 
                            ErrorMessage="At least one position must be specified" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label">2.</label>
                        <asp:TextBox ID="txtPosition2" runat="server" CssClass="form-input" MaxLength="100" />
                    </div>
                </div>
            </div>

            <!-- Salary and Employment Type -->
            <div class="form-row">
                <div class="form-col-narrow">
                    <div class="form-group">
                        <label class="form-label">Salary Desired:</label>
                        <asp:TextBox ID="txtSalaryDesired" runat="server" CssClass="form-input" MaxLength="10" />
                    </div>
                </div>
                <div class="form-col-narrow">
                    <div class="form-group radio-group">
                        <label class="form-label">Salary Type:</label>
                        <div class="radio-options">
                            <asp:RadioButton ID="rbHourly" runat="server" GroupName="SalaryType" Text="Hourly" CssClass="radio-option" />
                            <asp:RadioButton ID="rbYearly" runat="server" GroupName="SalaryType" Text="Yearly" CssClass="radio-option" />
                        </div>
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label">Available Start Date:</label>
                        <asp:TextBox ID="txtAvailableStartDate" runat="server" CssClass="form-input" TextMode="Date" />
                    </div>
                </div>
            </div>

            <!-- Employment Type -->
            <div class="form-row">
                <div class="form-col">
                    <div class="form-group checkbox-group">
                        <label class="form-label">Employment Sought:</label>
                        <div class="checkbox-options">
                            <asp:CheckBox ID="chkFullTime" runat="server" Text="Full Time" CssClass="checkbox-option" />
                            <asp:CheckBox ID="chkPartTime" runat="server" Text="Part Time" CssClass="checkbox-option" />
                            <asp:CheckBox ID="chkTemporary" runat="server" Text="Temporary" CssClass="checkbox-option" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- Desired Location -->
            <div class="form-row">
                <div class="form-col">
                    <div class="form-group checkbox-group">
                        <label class="form-label">Desired Location to work:</label>
                        <div class="checkbox-options location-options">
                            <asp:CheckBox ID="chkNashville" runat="server" Text="Nashville" CssClass="checkbox-option" />
                            <asp:CheckBox ID="chkFranklin" runat="server" Text="Franklin" CssClass="checkbox-option" />
                            <asp:CheckBox ID="chkShelbyville" runat="server" Text="Shelbyville" CssClass="checkbox-option" />
                            <asp:CheckBox ID="chkWaynesboro" runat="server" Text="Waynesboro" CssClass="checkbox-option" />
                        </div>
                        <div class="other-location">
                            <label>Other:</label>
                            <asp:TextBox ID="txtOtherLocation" runat="server" CssClass="form-input-inline" MaxLength="100" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- Shift Preferences -->
            <div class="form-row">
                <div class="form-col">
                    <div class="form-group checkbox-group">
                        <label class="form-label">Shift Sought:</label>
                        <div class="checkbox-options">
                            <asp:CheckBox ID="chk1stShift" runat="server" Text="1st Shift" CssClass="checkbox-option" />
                            <asp:CheckBox ID="chk2ndShift" runat="server" Text="2nd Shift" CssClass="checkbox-option" />
                            <asp:CheckBox ID="chk3rdShift" runat="server" Text="3rd Shift" CssClass="checkbox-option" />
                            <asp:CheckBox ID="chkWeekendsOnly" runat="server" Text="Weekends only" CssClass="checkbox-option" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- Days Available -->
            <div class="form-row">
                <div class="form-col">
                    <div class="form-group checkbox-group">
                        <label class="form-label">Days Available:</label>
                        <div class="checkbox-options days-available">
                            <asp:CheckBox ID="chkMonday" runat="server" Text="Mon" CssClass="checkbox-option" />
                            <asp:CheckBox ID="chkTuesday" runat="server" Text="Tues" CssClass="checkbox-option" />
                            <asp:CheckBox ID="chkWednesday" runat="server" Text="Wed" CssClass="checkbox-option" />
                            <asp:CheckBox ID="chkThursday" runat="server" Text="Thurs" CssClass="checkbox-option" />
                            <asp:CheckBox ID="chkFriday" runat="server" Text="Fri" CssClass="checkbox-option" />
                            <asp:CheckBox ID="chkSaturday" runat="server" Text="Sat" CssClass="checkbox-option" />
                            <asp:CheckBox ID="chkSunday" runat="server" Text="Sun" CssClass="checkbox-option" />
                        </div>
                        <div class="assignment-note">
                            <small>*Assignment of days, shifts, and hours are based on company needs without guaranteed permanency</small>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Previous TPA Employment Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">history</i>
                </div>
                <h2 class="section-title">Previous TPA Employment</h2>
            </div>

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label">Have you ever applied for a position with TPA, Inc. before?</label>
                        <div class="radio-options">
                            <asp:RadioButton ID="rbAppliedYes" runat="server" GroupName="PreviouslyApplied" Text="Yes" CssClass="radio-option" />
                            <asp:RadioButton ID="rbAppliedNo" runat="server" GroupName="PreviouslyApplied" Text="No" CssClass="radio-option" />
                        </div>
                        <div class="conditional-field">
                            <label>If yes, when?</label>
                            <asp:TextBox ID="txtPreviousApplicationDate" runat="server" CssClass="form-input-inline" MaxLength="100" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label">Have you ever worked for TPA, Inc. before?</label>
                        <div class="radio-options">
                            <asp:RadioButton ID="rbWorkedYes" runat="server" GroupName="PreviouslyWorked" Text="Yes" CssClass="radio-option" />
                            <asp:RadioButton ID="rbWorkedNo" runat="server" GroupName="PreviouslyWorked" Text="No" CssClass="radio-option" />
                        </div>
                        <div class="conditional-field">
                            <label>If yes, when?</label>
                            <asp:TextBox ID="txtPreviousWorkDate" runat="server" CssClass="form-input-inline" MaxLength="100" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label">Do you have any family members employed by TPA, Inc.</label>
                        <div class="radio-options">
                            <asp:RadioButton ID="rbFamilyYes" runat="server" GroupName="FamilyEmployed" Text="Yes" CssClass="radio-option" />
                            <asp:RadioButton ID="rbFamilyNo" runat="server" GroupName="FamilyEmployed" Text="No" CssClass="radio-option" />
                        </div>
                        <div class="conditional-field">
                            <label>If yes, who?</label>
                            <asp:TextBox ID="txtFamilyMemberDetails" runat="server" CssClass="form-input-inline" MaxLength="200" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Legal Status Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">gavel</i>
                </div>
                <h2 class="section-title">Legal Status</h2>
            </div>

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label">Are you a U.S. citizen or Permanent Resident?</label>
                        <div class="radio-options">
                            <asp:RadioButton ID="rbCitizenYes" runat="server" GroupName="USCitizen" Text="Yes" CssClass="radio-option" />
                            <asp:RadioButton ID="rbCitizenNo" runat="server" GroupName="USCitizen" Text="No" CssClass="radio-option" />
                        </div>
                        <div class="conditional-field">
                            <label>Alien # (if no)</label>
                            <asp:TextBox ID="txtAlienNumber" runat="server" CssClass="form-input-inline" MaxLength="50" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label">Or otherwise legally entitled to work in the U.S.A.</label>
                        <div class="radio-options">
                            <asp:RadioButton ID="rbLegallyEntitledYes" runat="server" GroupName="LegallyEntitled" Text="Yes" CssClass="radio-option" />
                            <asp:RadioButton ID="rbLegallyEntitledNo" runat="server" GroupName="LegallyEntitled" Text="No" CssClass="radio-option" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label">Are you 18 years or older?</label>
                        <div class="radio-options">
                            <asp:RadioButton ID="rb18OrOlderYes" runat="server" GroupName="EighteenOrOlder" Text="Yes" CssClass="radio-option" />
                            <asp:RadioButton ID="rb18OrOlderNo" runat="server" GroupName="EighteenOrOlder" Text="No" CssClass="radio-option" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Acknowledgment Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">verified</i>
                </div>
                <h2 class="section-title">Acknowledgment and Certification</h2>
            </div>

            <div class="acknowledgment-text">
                <p><strong>TPA, Inc. is an Equal Opportunity Employer</strong></p>
                <p>
                    Tennessee Law Prohibits Discrimination in Employment: It is illegal to discriminate against any person because of race, color, creed, religion, 
                    sex, age, handicap, or national origin in recruitment, training, hiring, discharge, promotion, or any condition, term or privilege of employment.
                </p>
            </div>

            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group checkbox-group">
                        <asp:CheckBox ID="chkAcknowledgment" runat="server" CssClass="checkbox-large" />
                        <label class="checkbox-label required" for="<%= chkAcknowledgment.ClientID %>">
                            I certify that all information provided in this application is true, complete, and accurate to the best of my knowledge. 
                            I understand that any false information may result in termination of employment. I authorize TPA, Inc. to verify 
                            any information contained in this application and to contact my references and former employers.
                        </label>
                        <asp:CustomValidator ID="cvAcknowledgment" runat="server" 
                            ErrorMessage="You must acknowledge the certification" CssClass="error-message" Display="Dynamic" 
                            ClientValidationFunction="validateAcknowledgment" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Submit Buttons -->
        <div class="form-actions">
            <asp:Button ID="btnSubmit" runat="server" Text="Complete Paperwork" 
                        CssClass="submit-btn" OnClick="btnSubmit_Click" />
            <asp:Button ID="btnSaveProgress" runat="server" Text="Save Progress" 
                        CssClass="submit-btn secondary" OnClick="btnSaveProgress_Click" />
        </div>
    </div>

    <!-- JavaScript for dynamic form behavior -->
    <script type="text/javascript">
        // Custom validation for checkbox
        function validateAcknowledgment(sender, args) {
            var checkbox = document.getElementById('<%= chkAcknowledgment.ClientID %>');
            args.IsValid = checkbox && checkbox.checked;
        }

        document.addEventListener('DOMContentLoaded', function () {
            // Set application date to today
            var appDateField = document.getElementById('<%= txtApplicationDate.ClientID %>');
            if (appDateField && !appDateField.value) {
                var today = new Date().toISOString().split('T')[0];
                appDateField.value = today;
            }

            // Phone number formatting
            function formatPhoneNumber(input) {
                var value = input.value.replace(/\D/g, '');
                var formattedValue = value.replace(/(\d{3})(\d{3})(\d{4})/, '($1) $2-$3');
                if (value.length <= 10) {
                    input.value = formattedValue;
                }
            }

            // SSN formatting
            function formatSSN(input) {
                var value = input.value.replace(/\D/g, '');
                var formattedValue = value.replace(/(\d{3})(\d{2})(\d{4})/, '$1-$2-$3');
                if (value.length <= 9) {
                    input.value = formattedValue;
                }
            }

            // Apply formatting to phone and SSN fields
            var phoneFields = document.querySelectorAll('input[id*="Phone"], input[id*="Cell"]');
            phoneFields.forEach(function (field) {
                field.addEventListener('input', function () { formatPhoneNumber(this); });
            });

            var ssnField = document.getElementById('<%= txtSSN.ClientID %>');
            if (ssnField) {
                ssnField.addEventListener('input', function () { formatSSN(this); });
            }
        });
    </script>
</asp:Content>