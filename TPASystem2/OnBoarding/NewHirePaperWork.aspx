<%@ Page Title="New Hire Paperwork" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="NewHirePaperWork.aspx.cs" Inherits="TPASystem2.OnBoarding.NewHirePaperwork" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
      <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

    <style>
        /* Task-specific styling */
        .mandatory-task-header {
            background: linear-gradient(135deg, #1976d2 0%, #1565c0 100%);
            color: white;
            padding: 2rem;
            border-radius: 12px 12px 0 0;
            margin-bottom: 0;
        }
        
        .task-title {
            display: flex;
            align-items: center;
            gap: 1rem;
            font-size: 2rem;
            font-weight: 600;
            margin: 1rem 0;
        }
        
        .task-subtitle {
            font-size: 1.1rem;
            opacity: 0.9;
            margin: 0;
        }
        
        .mandatory-badge {
            background: #ff5722;
            color: white;
            padding: 0.25rem 0.75rem;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        
        .form-container {
            background: white;
            border-radius: 0 0 12px 12px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            overflow: hidden;
        }
        
        .form-section {
            padding: 2rem;
            border-bottom: 1px solid #e0e0e0;
        }
        
        .form-section:last-child {
            border-bottom: none;
        }
        
        .section-header {
            display: flex;
            align-items: center;
            gap: 1rem;
            margin-bottom: 1.5rem;
        }
        
        .section-icon {
            background: #e3f2fd;
            color: #1976d2;
            width: 3rem;
            height: 3rem;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        .section-title {
            font-size: 1.5rem;
            font-weight: 600;
            margin: 0;
            color: #333;
        }
        
        .form-row {
            display: flex;
            gap: 2rem;
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
            font-weight: 600;
            margin-bottom: 0.5rem;
            color: #333;
        }
        
        .form-label.required::after {
            content: " *";
            color: #f44336;
        }
        
        .form-input, .form-select {
            width: 100%;
            padding: 0.75rem;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            font-size: 1rem;
            transition: border-color 0.3s ease;
            background: white;
        }
        
        .form-input:focus, .form-select:focus {
            outline: none;
            border-color: #1976d2;
            box-shadow: 0 0 0 3px rgba(25, 118, 210, 0.1);
        }
        
        .checkbox-group {
            display: flex;
            align-items: flex-start;
            gap: 0.75rem;
            padding: 1rem;
            background: #f8f9fa;
            border-radius: 8px;
            border: 2px solid #e0e0e0;
            transition: border-color 0.3s ease;
        }
        
        .checkbox-group:hover {
            border-color: #1976d2;
        }
        
        .checkbox-label {
            flex: 1;
            line-height: 1.5;
            cursor: pointer;
            margin: 0;
        }
        
        .form-actions {
            display: flex;
            justify-content: space-between;
            gap: 1rem;
            padding: 2rem;
            background: #f8f9fa;
            border-top: 1px solid #e0e0e0;
        }
        
        .btn-primary {
            background: #1976d2;
            color: white;
            border: none;
            padding: 0.75rem 2rem;
            border-radius: 8px;
            font-size: 1rem;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .btn-primary:hover {
            background: #1565c0;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(25, 118, 210, 0.3);
        }
        
        .btn-secondary {
            background: #f5f5f5;
            color: #333;
            border: 2px solid #e0e0e0;
            padding: 0.75rem 2rem;
            border-radius: 8px;
            font-size: 1rem;
            font-weight: 600;
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

        @media (max-width: 768px) {
            .form-row {
                flex-direction: column;
                gap: 0;
            }
            
            .form-actions {
                flex-direction: column;
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

    <!-- Form Container -->
    <div class="form-container">
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
                        <label class="form-label required" for="txtFirstName">First Name</label>
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-input" placeholder="Enter your first name" MaxLength="50" />
                        <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" 
                            ErrorMessage="First name is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label" for="txtMiddleName">Middle Name</label>
                        <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-input" placeholder="Enter your middle name (optional)" MaxLength="50" />
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

            <!-- Date of Birth and SSN -->
            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required" for="txtDateOfBirth">Date of Birth</label>
                        <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-input" placeholder="MM/DD/YYYY" TextMode="Date" />
                        <asp:RequiredFieldValidator ID="rfvDateOfBirth" runat="server" ControlToValidate="txtDateOfBirth" 
                            ErrorMessage="Date of birth is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required" for="txtSocialSecurityNumber">Social Security Number</label>
                        <asp:TextBox ID="txtSocialSecurityNumber" runat="server" CssClass="form-input" placeholder="XXX-XX-XXXX" MaxLength="11" />
                        <asp:RequiredFieldValidator ID="rfvSocialSecurityNumber" runat="server" ControlToValidate="txtSocialSecurityNumber" 
                            ErrorMessage="Social Security Number is required" CssClass="error-message" Display="Dynamic" />
                        <div class="form-help">Required for tax and benefits processing</div>
                    </div>
                </div>
            </div>

            <!-- Contact Information -->
            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required" for="txtPersonalEmail">Personal Email</label>
                        <asp:TextBox ID="txtPersonalEmail" runat="server" CssClass="form-input" 
                            placeholder="your.email@example.com" TextMode="Email" />
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
                            <asp:ListItem Value="AL" Text="Alabama" />
                            <asp:ListItem Value="AK" Text="Alaska" />
                            <asp:ListItem Value="AZ" Text="Arizona" />
                            <asp:ListItem Value="AR" Text="Arkansas" />
                            <asp:ListItem Value="CA" Text="California" />
                            <asp:ListItem Value="CO" Text="Colorado" />
                            <asp:ListItem Value="CT" Text="Connecticut" />
                            <asp:ListItem Value="DE" Text="Delaware" />
                            <asp:ListItem Value="FL" Text="Florida" />
                            <asp:ListItem Value="GA" Text="Georgia" />
                            <asp:ListItem Value="HI" Text="Hawaii" />
                            <asp:ListItem Value="ID" Text="Idaho" />
                            <asp:ListItem Value="IL" Text="Illinois" />
                            <asp:ListItem Value="IN" Text="Indiana" />
                            <asp:ListItem Value="IA" Text="Iowa" />
                            <asp:ListItem Value="KS" Text="Kansas" />
                            <asp:ListItem Value="KY" Text="Kentucky" />
                            <asp:ListItem Value="LA" Text="Louisiana" />
                            <asp:ListItem Value="ME" Text="Maine" />
                            <asp:ListItem Value="MD" Text="Maryland" />
                            <asp:ListItem Value="MA" Text="Massachusetts" />
                            <asp:ListItem Value="MI" Text="Michigan" />
                            <asp:ListItem Value="MN" Text="Minnesota" />
                            <asp:ListItem Value="MS" Text="Mississippi" />
                            <asp:ListItem Value="MO" Text="Missouri" />
                            <asp:ListItem Value="MT" Text="Montana" />
                            <asp:ListItem Value="NE" Text="Nebraska" />
                            <asp:ListItem Value="NV" Text="Nevada" />
                            <asp:ListItem Value="NH" Text="New Hampshire" />
                            <asp:ListItem Value="NJ" Text="New Jersey" />
                            <asp:ListItem Value="NM" Text="New Mexico" />
                            <asp:ListItem Value="NY" Text="New York" />
                            <asp:ListItem Value="NC" Text="North Carolina" />
                            <asp:ListItem Value="ND" Text="North Dakota" />
                            <asp:ListItem Value="OH" Text="Ohio" />
                            <asp:ListItem Value="OK" Text="Oklahoma" />
                            <asp:ListItem Value="OR" Text="Oregon" />
                            <asp:ListItem Value="PA" Text="Pennsylvania" />
                            <asp:ListItem Value="RI" Text="Rhode Island" />
                            <asp:ListItem Value="SC" Text="South Carolina" />
                            <asp:ListItem Value="SD" Text="South Dakota" />
                            <asp:ListItem Value="TN" Text="Tennessee" />
                            <asp:ListItem Value="TX" Text="Texas" />
                            <asp:ListItem Value="UT" Text="Utah" />
                            <asp:ListItem Value="VT" Text="Vermont" />
                            <asp:ListItem Value="VA" Text="Virginia" />
                            <asp:ListItem Value="WA" Text="Washington" />
                            <asp:ListItem Value="WV" Text="West Virginia" />
                            <asp:ListItem Value="WI" Text="Wisconsin" />
                            <asp:ListItem Value="WY" Text="Wyoming" />
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
        </div>

        <!-- Emergency Contact Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">contact_emergency</i>
                </div>
                <h2 class="section-title">Emergency Contact</h2>
            </div>

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required" for="txtEmergencyContactName">Contact Name</label>
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
                        <label class="form-label required" for="txtEmergencyContactPhone">Phone Number</label>
                        <asp:TextBox ID="txtEmergencyContactPhone" runat="server" CssClass="form-input" placeholder="(555) 123-4567" />
                        <asp:RequiredFieldValidator ID="rfvEmergencyContactPhone" runat="server" ControlToValidate="txtEmergencyContactPhone" 
                            ErrorMessage="Emergency contact phone is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label" for="txtEmergencyContactEmail">Email Address (Optional)</label>
                        <asp:TextBox ID="txtEmergencyContactEmail" runat="server" CssClass="form-input" placeholder="email@example.com" TextMode="Email" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Tax Information Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">receipt_long</i>
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
                            <asp:ListItem Value="MarriedJoint" Text="Married filing jointly" />
                            <asp:ListItem Value="HeadOfHousehold" Text="Head of household" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvFilingStatus" runat="server" ControlToValidate="ddlFilingStatus" InitialValue=""
                            ErrorMessage="Filing status is required" CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label" for="txtDependents">Number of Dependents</label>
                        <asp:TextBox ID="txtDependents" runat="server" CssClass="form-input" placeholder="0" TextMode="Number" />
                        <div class="form-help">Enter 0 if none</div>
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label" for="txtAdditionalWithholding">Additional Withholding</label>
                        <asp:TextBox ID="txtAdditionalWithholding" runat="server" CssClass="form-input" placeholder="0.00" />
                        <div class="form-help">Additional amount to withhold from each paycheck (optional)</div>
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label" for="chkTaxExempt">Tax Exempt Status</label>
                        <div class="checkbox-group">
                            <asp:CheckBox ID="chkTaxExempt" runat="server" />
                            <label class="checkbox-label" for="<%= chkTaxExempt.ClientID %>">
                                I claim exemption from withholding for this tax year
                            </label>
                        </div>
                        <div class="form-help">Check only if you qualify for tax exemption</div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Work Authorization Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">work</i>
                </div>
                <h2 class="section-title">Work Authorization (I-9)</h2>
            </div>

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required" for="ddlWorkAuthorization">Work Authorization Status</label>
                        <asp:DropDownList ID="ddlWorkAuthorization" runat="server" CssClass="form-select">
                            <asp:ListItem Value="" Text="Select Work Authorization" />
                            <asp:ListItem Value="USCitizen" Text="U.S. Citizen" />
                            <asp:ListItem Value="LawfulPermanentResident" Text="Lawful Permanent Resident" />
                            <asp:ListItem Value="AlienAuthorized" Text="Alien authorized to work" />
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
                        <strong>I-9 Attestation:</strong> I attest, under penalty of perjury, that I am (check one of the following boxes):
                        1. A citizen of the United States
                        2. A noncitizen national of the United States
                        3. A lawful permanent resident
                        4. An alien authorized to work until (expiration date, if applicable)
                        I understand that this form is being completed in connection with the federal Form I-9 and that knowingly and willfully making false statements or using false documentation may subject me to criminal or civil penalties under federal law.
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
        document.addEventListener('DOMContentLoaded', function () {
            var form = document.forms[0];
            if (form) {
                form.addEventListener('submit', function (e) {
                    if (Page_ClientValidate && Page_ClientValidate()) {
                        showLoading();
                    }
                });
            }
        });
    </script>
</asp:Content>