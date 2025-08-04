<%@ Page Title="New Hire Paperwork" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="NewHirePaperwork.aspx.cs" Inherits="TPASystem2.OnBoarding.NewHirePaperwork" %>

<asp:Content ID="NewHirePaperworkContent" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    
    <!-- Custom Styles -->
    <style>
        .mandatory-task-header {
            background: linear-gradient(135deg, #4caf50 0%, #66bb6a 100%);
            color: white;
            padding: 2.5rem;
            border-radius: 16px;
            margin-bottom: 2rem;
            position: relative;
            overflow: hidden;
        }
        
        .mandatory-task-header::before {
            content: '📋';
            position: absolute;
            top: 1rem;
            right: 1rem;
            font-size: 3rem;
            opacity: 0.3;
        }
        
        .task-title {
            font-size: 2.2rem;
            font-weight: 700;
            margin: 0 0 0.5rem 0;
            display: flex;
            align-items: center;
            gap: 1rem;
        }
        
        .task-subtitle {
            font-size: 1.1rem;
            opacity: 0.9;
            margin: 0;
        }
        
        .mandatory-badge {
            background: rgba(255, 255, 255, 0.2);
            padding: 0.5rem 1rem;
            border-radius: 25px;
            font-size: 0.9rem;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        
        .form-section {
            background: white;
            border-radius: 16px;
            padding: 2rem;
            margin-bottom: 2rem;
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
            border: 2px solid #e8f5e8;
        }
        
        .section-header {
            display: flex;
            align-items: center;
            gap: 1rem;
            margin-bottom: 2rem;
            padding-bottom: 1rem;
            border-bottom: 2px solid #e8f5e8;
        }
        
        .section-icon {
            background: linear-gradient(135deg, #4caf50 0%, #66bb6a 100%);
            color: white;
            width: 50px;
            height: 50px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.5rem;
        }
        
        .section-title {
            color: #2e7d32;
            font-size: 1.5rem;
            font-weight: 600;
            margin: 0;
        }
        
        .form-row {
            display: flex;
            gap: 1.5rem;
            margin-bottom: 1.5rem;
        }
        
        .form-col {
            flex: 1;
        }
        
        .form-col.col-2 {
            flex: 2;
        }
        
        .form-group {
            margin-bottom: 1.5rem;
        }
        
        .form-label {
            display: block;
            margin-bottom: 0.5rem;
            font-weight: 600;
            color: #2e7d32;
        }
        
        .form-label.required::after {
            content: ' *';
            color: #f44336;
        }
        
        .form-input {
            width: 100%;
            padding: 1rem;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            font-size: 1rem;
            transition: all 0.3s ease;
            background: #fafafa;
        }
        
        .form-input:focus {
            outline: none;
            border-color: #4caf50;
            background: white;
            box-shadow: 0 0 0 3px rgba(76, 175, 80, 0.1);
        }
        
        .form-select {
            width: 100%;
            padding: 1rem;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            font-size: 1rem;
            background: #fafafa;
            cursor: pointer;
        }
        
        .form-select:focus {
            outline: none;
            border-color: #4caf50;
            background: white;
        }
        
        .checkbox-group {
            display: flex;
            align-items: flex-start;
            gap: 1rem;
            padding: 1rem;
            background: #f8f9fa;
            border-radius: 8px;
            border: 2px solid #e0e0e0;
        }
        
        .checkbox-group input[type="checkbox"] {
            width: 20px;
            height: 20px;
            margin: 0;
            accent-color: #4caf50;
        }
        
        .checkbox-label {
            flex: 1;
            line-height: 1.5;
            color: #333;
        }
        
        .form-actions {
            display: flex;
            gap: 1rem;
            justify-content: flex-end;
            margin-top: 3rem;
            padding-top: 2rem;
            border-top: 2px solid #e8f5e8;
        }
        
        .btn-primary {
            background: linear-gradient(135deg, #4caf50 0%, #66bb6a 100%);
            color: white;
            border: none;
            padding: 1rem 3rem;
            border-radius: 25px;
            font-size: 1.1rem;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .btn-primary:hover {
            background: linear-gradient(135deg, #388e3c 0%, #4caf50 100%);
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(76, 175, 80, 0.3);
        }
        
        .btn-secondary {
            background: #f5f5f5;
            color: #666;
            border: 2px solid #e0e0e0;
            padding: 1rem 2rem;
            border-radius: 25px;
            font-size: 1rem;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.3s ease;
        }
        
        .btn-secondary:hover {
            background: #e0e0e0;
            color: #333;
        }
        
        .progress-tracker {
            display: flex;
            justify-content: center;
            margin-bottom: 2rem;
        }
        
        .progress-step {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            padding: 0.5rem 1rem;
            background: #e8f5e8;
            color: #2e7d32;
            border-radius: 25px;
            font-weight: 600;
        }
        
        .form-help {
            font-size: 0.9rem;
            color: #666;
            margin-top: 0.5rem;
            font-style: italic;
        }
        
        .error-message {
            color: #f44336;
            font-size: 0.9rem;
            margin-top: 0.5rem;
        }
        
        .success-message {
            background: #e8f5e8;
            color: #2e7d32;
            padding: 1rem;
            border-radius: 8px;
            margin-bottom: 1rem;
            border: 2px solid #4caf50;
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
            New Hire Paperwork
            <span class="mandatory-badge">Mandatory</span>
        </h1>
        <p class="task-subtitle">Complete your essential employee information and documentation</p>
    </div>

    <!-- Success Message -->
    <asp:Panel ID="pnlSuccessMessage" runat="server" CssClass="success-message" Visible="false">
        <i class="material-icons" style="vertical-align: middle; margin-right: 0.5rem;">check_circle</i>
        <strong>Paperwork completed successfully!</strong> You will be redirected to your onboarding dashboard.
    </asp:Panel>

    <!-- Personal Information Section -->
    <div class="form-section">
        <div class="section-header">
            <div class="section-icon">
                <i class="material-icons">person</i>
            </div>
            <h2 class="section-title">Personal Information</h2>
        </div>

        <!-- Basic Info Row -->
        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="txtFirstName">First Name</label>
                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-input" placeholder="Enter your first name" MaxLength="50" />
                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" 
                        ErrorMessage="First name is required" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label" for="txtMiddleName">Middle Name</label>
                    <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-input" placeholder="Enter your middle name" MaxLength="50" />
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="txtLastName">Last Name</label>
                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-input" placeholder="Enter your last name" MaxLength="50" />
                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" 
                        ErrorMessage="Last name is required" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
        </div>

        <!-- Contact Info Row -->
        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="txtPersonalEmail">Personal Email</label>
                    <asp:TextBox ID="txtPersonalEmail" runat="server" CssClass="form-input" placeholder="your.email@example.com" TextMode="Email" />
                    <asp:RequiredFieldValidator ID="rfvPersonalEmail" runat="server" ControlToValidate="txtPersonalEmail" 
                        ErrorMessage="Personal email is required" CssClass="error-message" Display="Dynamic" />
                    <div class="form-help">This will be used for personal communications</div>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="txtPhoneNumber">Phone Number</label>
                    <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-input" placeholder="(555) 123-4567" />
                    <asp:RequiredFieldValidator ID="rfvPhoneNumber" runat="server" ControlToValidate="txtPhoneNumber" 
                        ErrorMessage="Phone number is required" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
        </div>

        <!-- Address Info -->
        <div class="form-row">
            <div class="form-col col-2">
                <div class="form-group">
                    <label class="form-label required" for="txtAddress">Home Address</label>
                    <asp:TextBox ID="txtAddress" runat="server" CssClass="form-input" placeholder="123 Main Street" MaxLength="200" />
                    <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ControlToValidate="txtAddress" 
                        ErrorMessage="Home address is required" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="txtCity">City</label>
                    <asp:TextBox ID="txtCity" runat="server" CssClass="form-input" placeholder="Enter city" MaxLength="50" />
                    <asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="txtCity" 
                        ErrorMessage="City is required" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="ddlState">State</label>
                    <asp:DropDownList ID="ddlState" runat="server" CssClass="form-select">
                        <asp:ListItem Value="" Text="Select State" />
                        <asp:ListItem Value="TN" Text="Tennessee" />
                        <asp:ListItem Value="AL" Text="Alabama" />
                        <asp:ListItem Value="AR" Text="Arkansas" />
                        <asp:ListItem Value="FL" Text="Florida" />
                        <asp:ListItem Value="GA" Text="Georgia" />
                        <asp:ListItem Value="KY" Text="Kentucky" />
                        <asp:ListItem Value="MS" Text="Mississippi" />
                        <asp:ListItem Value="NC" Text="North Carolina" />
                        <asp:ListItem Value="SC" Text="South Carolina" />
                        <asp:ListItem Value="VA" Text="Virginia" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvState" runat="server" ControlToValidate="ddlState" InitialValue=""
                        ErrorMessage="State is required" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="txtZipCode">ZIP Code</label>
                    <asp:TextBox ID="txtZipCode" runat="server" CssClass="form-input" placeholder="12345" MaxLength="10" />
                    <asp:RequiredFieldValidator ID="rfvZipCode" runat="server" ControlToValidate="txtZipCode" 
                        ErrorMessage="ZIP code is required" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
        </div>

        <!-- Personal Details -->
        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label" for="txtDateOfBirth">Date of Birth</label>
                    <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-input" TextMode="Date" />
                    <div class="form-help">Optional - for benefits and HR records</div>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label" for="ddlGender">Gender</label>
                    <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-select">
                        <asp:ListItem Value="" Text="Prefer not to say" />
                        <asp:ListItem Value="Male" Text="Male" />
                        <asp:ListItem Value="Female" Text="Female" />
                        <asp:ListItem Value="Other" Text="Other" />
                    </asp:DropDownList>
                    <div class="form-help">Optional - for reporting purposes only</div>
                </div>
            </div>
        </div>
    </div>

    <!-- Emergency Contact Section -->
    <div class="form-section">
        <div class="section-header">
            <div class="section-icon">
                <i class="material-icons">contact_emergency</i>
            </div>
            <h2 class="section-title">Emergency Contact Information</h2>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="txtEmergencyContactName">Emergency Contact Name</label>
                    <asp:TextBox ID="txtEmergencyContactName" runat="server" CssClass="form-input" placeholder="Full name" MaxLength="100" />
                    <asp:RequiredFieldValidator ID="rfvEmergencyContactName" runat="server" ControlToValidate="txtEmergencyContactName" 
                        ErrorMessage="Emergency contact name is required" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="txtEmergencyContactRelationship">Relationship</label>
                    <asp:TextBox ID="txtEmergencyContactRelationship" runat="server" CssClass="form-input" placeholder="e.g., Spouse, Parent, Sibling" MaxLength="50" />
                    <asp:RequiredFieldValidator ID="rfvEmergencyContactRelationship" runat="server" ControlToValidate="txtEmergencyContactRelationship" 
                        ErrorMessage="Relationship is required" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="txtEmergencyContactPhone">Emergency Contact Phone</label>
                    <asp:TextBox ID="txtEmergencyContactPhone" runat="server" CssClass="form-input" placeholder="(555) 123-4567" />
                    <asp:RequiredFieldValidator ID="rfvEmergencyContactPhone" runat="server" ControlToValidate="txtEmergencyContactPhone" 
                        ErrorMessage="Emergency contact phone is required" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label" for="txtEmergencyContactEmail">Emergency Contact Email</label>
                    <asp:TextBox ID="txtEmergencyContactEmail" runat="server" CssClass="form-input" placeholder="contact@example.com" TextMode="Email" />
                    <div class="form-help">Optional - backup contact method</div>
                </div>
            </div>
        </div>
    </div>

    <!-- Tax Information Section -->
    <div class="form-section">
        <div class="section-header">
            <div class="section-icon">
                <i class="material-icons">receipt</i>
            </div>
            <h2 class="section-title">Tax Information (W-4)</h2>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="ddlFilingStatus">Filing Status</label>
                    <asp:DropDownList ID="ddlFilingStatus" runat="server" CssClass="form-select">
                        <asp:ListItem Value="" Text="Select Filing Status" />
                        <asp:ListItem Value="Single" Text="Single or Married filing separately" />
                        <asp:ListItem Value="MarriedJointly" Text="Married filing jointly" />
                        <asp:ListItem Value="HeadOfHousehold" Text="Head of household" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvFilingStatus" runat="server" ControlToValidate="ddlFilingStatus" InitialValue=""
                        ErrorMessage="Filing status is required" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label" for="txtDependents">Number of Dependents</label>
                    <asp:TextBox ID="txtDependents" runat="server" CssClass="form-input" TextMode="Number" Text="0" min="0" max="20" />
                    <div class="form-help">Number of qualifying children or dependents</div>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label" for="txtAdditionalWithholding">Additional Federal Withholding</label>
                    <asp:TextBox ID="txtAdditionalWithholding" runat="server" CssClass="form-input" TextMode="Number" step="0.01" placeholder="0.00" />
                    <div class="form-help">Optional - additional amount to withhold per pay period</div>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <div class="checkbox-group">
                        <asp:CheckBox ID="chkTaxExempt" runat="server" />
                        <label class="checkbox-label" for="<%= chkTaxExempt.ClientID %>">
                            I claim exemption from withholding for 2025, and I certify that I meet both of the following conditions for exemption:
                            • Last year I had a right to a refund of all federal income tax withheld because I had no tax liability, and
                            • This year I expect a refund of all federal income tax withheld because I expect to have no tax liability.
                        </label>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- I-9 Employment Eligibility Section -->
    <div class="form-section">
        <div class="section-header">
            <div class="section-icon">
                <i class="material-icons">verified_user</i>
            </div>
            <h2 class="section-title">Employment Eligibility (I-9)</h2>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="ddlWorkAuthorization">Work Authorization Status</label>
                    <asp:DropDownList ID="ddlWorkAuthorization" runat="server" CssClass="form-select">
                        <asp:ListItem Value="" Text="Select Work Authorization" />
                        <asp:ListItem Value="USCitizen" Text="U.S. Citizen" />
                        <asp:ListItem Value="LawfulPermanentResident" Text="Lawful Permanent Resident" />
                        <asp:ListItem Value="AuthorizedToWork" Text="Authorized to work in the U.S." />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvWorkAuthorization" runat="server" ControlToValidate="ddlWorkAuthorization" InitialValue=""
                        ErrorMessage="Work authorization status is required" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
        </div>

        <div class="form-group">
            <div class="checkbox-group">
                <asp:CheckBox ID="chkI9Attestation" runat="server" />
                <label class="checkbox-label" for="<%= chkI9Attestation.ClientID %>">
                    <strong>I attest, under penalty of perjury,</strong> that I am (check one of the following boxes):
                    <br/>• A citizen of the United States
                    <br/>• A noncitizen national of the United States
                    <br/>• A lawful permanent resident
                    <br/>• An alien authorized to work until the expiration date shown (if applicable)
                    <br/><br/>
                    I acknowledge that federal law provides for imprisonment and/or fines for false statements or use of false documents in connection with the completion of this form.
                </label>
            </div>
            <asp:RequiredFieldValidator ID="rfvI9Attestation" runat="server" ControlToValidate="chkI9Attestation" 
                ErrorMessage="You must complete the I-9 attestation" CssClass="error-message" Display="Dynamic" />
        </div>
    </div>

    <!-- Acknowledgments Section -->
    <div class="form-section">
        <div class="section-header">
            <div class="section-icon">
                <i class="material-icons">fact_check</i>
            </div>
            <h2 class="section-title">Acknowledgments & Agreements</h2>
        </div>

        <div class="form-group">
            <div class="checkbox-group">
                <asp:CheckBox ID="chkEmployeeHandbook" runat="server" />
                <label class="checkbox-label" for="<%= chkEmployeeHandbook.ClientID %>">
                    <strong>Employee Handbook Acknowledgment:</strong> I acknowledge that I have received and read the Employee Handbook. I understand that this handbook contains important information about company policies, procedures, and expectations.
                </label>
            </div>
            <asp:RequiredFieldValidator ID="rfvEmployeeHandbook" runat="server" ControlToValidate="chkEmployeeHandbook" 
                ErrorMessage="Employee handbook acknowledgment is required" CssClass="error-message" Display="Dynamic" />
        </div>

        <div class="form-group">
            <div class="checkbox-group">
                <asp:CheckBox ID="chkDataAccuracy" runat="server" />
                <label class="checkbox-label" for="<%= chkDataAccuracy.ClientID %>">
                    <strong>Data Accuracy:</strong> I certify that all information provided in this form is true, complete, and accurate to the best of my knowledge. I understand that any false information may result in termination of employment.
                </label>
            </div>
            <asp:RequiredFieldValidator ID="rfvDataAccuracy" runat="server" ControlToValidate="chkDataAccuracy" 
                ErrorMessage="Data accuracy certification is required" CssClass="error-message" Display="Dynamic" />
        </div>

        <div class="form-group">
            <div class="checkbox-group">
                <asp:CheckBox ID="chkPrivacyConsent" runat="server" />
                <label class="checkbox-label" for="<%= chkPrivacyConsent.ClientID %>">
                    <strong>Privacy Consent:</strong> I consent to the collection, processing, and storage of my personal information as outlined in the company's Privacy Policy for employment purposes, benefits administration, and legal compliance.
                </label>
            </div>
            <asp:RequiredFieldValidator ID="rfvPrivacyConsent" runat="server" ControlToValidate="chkPrivacyConsent" 
                ErrorMessage="Privacy consent is required" CssClass="error-message" Display="Dynamic" />
        </div>
    </div>

    <!-- Form Actions -->
    <div class="form-actions">
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-secondary" 
            OnClick="btnCancel_Click" CausesValidation="false" />
        <asp:Button ID="btnSubmit" runat="server" Text="Complete Paperwork" CssClass="btn-primary" 
            OnClick="btnSubmit_Click" />
    </div>

    <!-- Loading overlay -->
    <div id="loadingOverlay" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); z-index: 10000; align-items: center; justify-content: center;">
        <div style="background: white; padding: 2rem; border-radius: 12px; text-align: center;">
            <i class="material-icons" style="font-size: 3rem; color: #4caf50; animation: spin 1s linear infinite;">sync</i>
            <p style="margin: 1rem 0 0 0; font-weight: 600;">Processing your paperwork...</p>
        </div>
    </div>

    <style>
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
    </style>

    <script>
        // Show loading overlay on form submit
        function showLoading() {
            document.getElementById('loadingOverlay').style.display = 'flex';
        }

        // Add form submit handler
        document.addEventListener('DOMContentLoaded', function() {
            var form = document.forms[0];
            if (form) {
                form.addEventListener('submit', function(e) {
                    if (Page_ClientValidate && Page_ClientValidate()) {
                        showLoading();
                    }
                });
            }
        });
    </script>
</asp:Content>