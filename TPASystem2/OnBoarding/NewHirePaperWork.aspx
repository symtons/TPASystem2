<%@ Page Title="New Hire Paperwork - Employment Application" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="NewHirePaperWork.aspx.cs" Inherits="TPASystem2.OnBoarding.NewHirePaperwork" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

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

    <!-- Application Header -->
    <div class="application-header">
        <h1 class="application-title">Application for Employment</h1>
        <p class="application-subtitle">
            Application will be kept on file for 90 Days<br />
            Please Print and complete application <strong>completely</strong> to be considered. All information must be entered on 
            application form to be considered for employment – even if resume is attached.
        </p>
    </div>

    <!-- Form Container -->
    <div class="form-container">
        <!-- Application Date -->
        <div class="form-section">
            <div class="form-row">
                <div class="form-col-auto">
                    <div class="form-group">
                        <label class="form-label">Date</label>
                        <asp:TextBox ID="txtApplicationDate" runat="server" CssClass="form-input form-input-date" 
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

            <!-- Name Row -->
            <div class="form-row">
                <div class="form-col-wide">
                    <div class="form-group">
                        <label class="form-label required">Last Name</label>
                        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-input" MaxLength="100" />
                        <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" 
                            ErrorMessage="Last name is required" CssClass="field-validation-error" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col-wide">
                    <div class="form-group">
                        <label class="form-label required">First Name</label>
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-input" MaxLength="100" />
                        <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" 
                            ErrorMessage="First name is required" CssClass="field-validation-error" Display="Dynamic" />
                    </div>
                </div>
                <div class="form-col-medium">
                    <div class="form-group">
                        <label class="form-label">Middle Name</label>
                        <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-input" MaxLength="100" />
                    </div>
                </div>
            </div>

            <!-- Address Row -->
            <div class="form-row">
                <div class="form-col-wide">
                    <div class="form-group">
                        <label class="form-label">Home Address</label>
                        <asp:TextBox ID="txtHomeAddress" runat="server" CssClass="form-input" MaxLength="255" />
                    </div>
                </div>
                <div class="form-col-narrow">
                    <div class="form-group">
                        <label class="form-label">Apt. #</label>
                        <asp:TextBox ID="txtAptNumber" runat="server" CssClass="form-input" MaxLength="20" />
                    </div>
                </div>
            </div>

            <!-- City, State, Zip Row -->
            <div class="form-row">
                <div class="form-col-wide">
                    <div class="form-group">
                        <label class="form-label">City</label>
                        <asp:TextBox ID="txtCity" runat="server" CssClass="form-input" MaxLength="100" />
                    </div>
                </div>
                <div class="form-col-narrow">
                    <div class="form-group">
                        <label class="form-label">State</label>
                        <asp:TextBox ID="txtState" runat="server" CssClass="form-input form-input-state" MaxLength="50" />
                    </div>
                </div>
                <div class="form-col-narrow">
                    <div class="form-group">
                        <label class="form-label">Zip Code</label>
                        <asp:TextBox ID="txtZipCode" runat="server" CssClass="form-input form-input-zip" MaxLength="10" />
                    </div>
                </div>
            </div>

            <!-- SSN and Driver's License Row -->
            <div class="form-row">
                <div class="form-col-medium">
                    <div class="form-group">
                        <label class="form-label">Social Security Number</label>
                        <asp:TextBox ID="txtSSN" runat="server" CssClass="form-input form-input-ssn" MaxLength="11" 
                            placeholder="___-__-____" />
                    </div>
                </div>
                <div class="form-col-medium">
                    <div class="form-group">
                        <label class="form-label">Driver's Lic #</label>
                        <asp:TextBox ID="txtDriversLicense" runat="server" CssClass="form-input" MaxLength="50" />
                    </div>
                </div>
                <div class="form-col-narrow">
                    <div class="form-group">
                        <label class="form-label">State</label>
                        <asp:TextBox ID="txtDLState" runat="server" CssClass="form-input form-input-state" MaxLength="10" />
                    </div>
                </div>
            </div>

            <!-- Phone Numbers Row -->
            <div class="form-row">
                <div class="form-col-medium">
                    <div class="form-group">
                        <label class="form-label">Phone Number</label>
                        <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-input form-input-phone" 
                            MaxLength="20" placeholder="(   )   -    " />
                    </div>
                </div>
                <div class="form-col-medium">
                    <div class="form-group">
                        <label class="form-label">Cell Number</label>
                        <asp:TextBox ID="txtCellNumber" runat="server" CssClass="form-input form-input-phone" 
                            MaxLength="20" placeholder="(   )   -    " />
                    </div>
                </div>
            </div>

            <!-- Emergency Contact -->
            <div class="form-row">
                <div class="form-col-wide">
                    <div class="form-group">
                        <label class="form-label">Emergency Contact Person</label>
                        <asp:TextBox ID="txtEmergencyContactName" runat="server" CssClass="form-input" MaxLength="200" />
                    </div>
                </div>
                <div class="form-col-medium">
                    <div class="form-group">
                        <label class="form-label">Relationship</label>
                        <asp:TextBox ID="txtEmergencyContactRelationship" runat="server" CssClass="form-input" MaxLength="100" />
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <label class="form-label">Emergency Contact Address and phone:</label>
                        <asp:TextBox ID="txtEmergencyContactAddress" runat="server" CssClass="form-input form-input-full" 
                            TextMode="MultiLine" Rows="2" MaxLength="500" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Position Information Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">work</i>
                </div>
                <h2 class="section-title">Position Information</h2>
            </div>

            <!-- Positions Applied For -->
            <div class="form-row">
                <div class="form-col-half">
                    <div class="form-group">
                        <label class="form-label">Position(s) Applied For: 1.</label>
                        <asp:TextBox ID="txtPosition1" runat="server" CssClass="form-input" MaxLength="200" />
                    </div>
                </div>
                <div class="form-col-half">
                    <div class="form-group">
                        <label class="form-label">2.</label>
                        <asp:TextBox ID="txtPosition2" runat="server" CssClass="form-input" MaxLength="200" />
                    </div>
                </div>
            </div>

            <!-- Salary and Employment Type -->
            <div class="form-row">
                <div class="form-col-medium">
                    <div class="form-group">
                        <label class="form-label">Salary Desired:</label>
                        <asp:TextBox ID="txtSalaryDesired" runat="server" CssClass="form-input" MaxLength="20" />
                    </div>
                </div>
                <div class="form-col-medium">
                    <div class="form-group">
                        <div class="radio-group">
                            <div class="radio-option">
                                <asp:RadioButton ID="rbHourly" runat="server" GroupName="SalaryType" />
                                <label for="<%= rbHourly.ClientID %>">Hourly</label>
                            </div>
                            <div class="radio-option">
                                <asp:RadioButton ID="rbYearly" runat="server" GroupName="SalaryType" />
                                <label for="<%= rbYearly.ClientID %>">Yearly</label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="form-col-medium">
                    <div class="form-group">
                        <label class="form-label">Available Start Date:</label>
                        <asp:TextBox ID="txtAvailableStartDate" runat="server" CssClass="form-input form-input-date" TextMode="Date" />
                    </div>
                </div>
            </div>

            <!-- Employment Sought -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <label class="form-label">Employment Sought:</label>
                        <div class="checkbox-group-inline">
                            <div class="checkbox-group">
                                <asp:CheckBox ID="chkFullTime" runat="server" CssClass="checkbox-large" />
                                <label class="checkbox-label" for="<%= chkFullTime.ClientID %>">Full Time</label>
                            </div>
                            <div class="checkbox-group">
                                <asp:CheckBox ID="chkPartTime" runat="server" CssClass="checkbox-large" />
                                <label class="checkbox-label" for="<%= chkPartTime.ClientID %>">Part Time</label>
                            </div>
                            <div class="checkbox-group">
                                <asp:CheckBox ID="chkTemporary" runat="server" CssClass="checkbox-large" />
                                <label class="checkbox-label" for="<%= chkTemporary.ClientID %>">Temporary</label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Desired Location to Work -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <label class="form-label">Desired Location to work:</label>
                        <div class="location-options">
                            <div class="location-option">
                                <asp:CheckBox ID="chkNashville" runat="server" />
                                <label for="<%= chkNashville.ClientID %>">Nashville</label>
                            </div>
                            <div class="location-option">
                                <asp:CheckBox ID="chkFranklin" runat="server" />
                                <label for="<%= chkFranklin.ClientID %>">Franklin</label>
                            </div>
                            <div class="location-option">
                                <asp:CheckBox ID="chkShelbyville" runat="server" />
                                <label for="<%= chkShelbyville.ClientID %>">Shelbyville</label>
                            </div>
                            <div class="location-option">
                                <asp:CheckBox ID="chkWaynesboro" runat="server" />
                                <label for="<%= chkWaynesboro.ClientID %>">Waynesboro</label>
                            </div>
                        </div>
                        <div class="other-location">
                            <asp:CheckBox ID="chkOtherLocation" runat="server" />
                            <label for="<%= chkOtherLocation.ClientID %>">Other</label>
                            <asp:TextBox ID="txtOtherLocation" runat="server" CssClass="conditional-input" MaxLength="200" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- Shift Sought -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <label class="form-label">Shift Sought:</label>
                        <div class="shift-options">
                            <div class="shift-option">
                                <asp:CheckBox ID="chkFirstShift" runat="server" />
                                <label for="<%= chkFirstShift.ClientID %>">1st Shift</label>
                            </div>
                            <div class="shift-option">
                                <asp:CheckBox ID="chkSecondShift" runat="server" />
                                <label for="<%= chkSecondShift.ClientID %>">2nd Shift</label>
                            </div>
                            <div class="shift-option">
                                <asp:CheckBox ID="chkThirdShift" runat="server" />
                                <label for="<%= chkThirdShift.ClientID %>">3rd Shift</label>
                            </div>
                            <div class="shift-option">
                                <asp:CheckBox ID="chkWeekendsOnly" runat="server" />
                                <label for="<%= chkWeekendsOnly.ClientID %>">Weekends only</label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Days Available -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <label class="form-label">Days Available:</label>
                        <div class="days-available">
                            <div class="day-option">
                                <asp:CheckBox ID="chkMonday" runat="server" />
                                <label for="<%= chkMonday.ClientID %>">Mon</label>
                            </div>
                            <div class="day-option">
                                <asp:CheckBox ID="chkTuesday" runat="server" />
                                <label for="<%= chkTuesday.ClientID %>">Tues</label>
                            </div>
                            <div class="day-option">
                                <asp:CheckBox ID="chkWednesday" runat="server" />
                                <label for="<%= chkWednesday.ClientID %>">Wed</label>
                            </div>
                            <div class="day-option">
                                <asp:CheckBox ID="chkThursday" runat="server" />
                                <label for="<%= chkThursday.ClientID %>">Thurs</label>
                            </div>
                            <div class="day-option">
                                <asp:CheckBox ID="chkFriday" runat="server" />
                                <label for="<%= chkFriday.ClientID %>">Fri</label>
                            </div>
                            <div class="day-option">
                                <asp:CheckBox ID="chkSaturday" runat="server" />
                                <label for="<%= chkSaturday.ClientID %>">Sat</label>
                            </div>
                            <div class="day-option">
                                <asp:CheckBox ID="chkSunday" runat="server" />
                                <label for="<%= chkSunday.ClientID %>">Sun</label>
                            </div>
                        </div>
                        <p class="form-note">*Assignment of days, shifts, and hours are based on company needs without guaranteed permanency</p>
                    </div>
                </div>
            </div>
        </div>

        <!-- TPA History Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">history</i>
                </div>
                <h2 class="section-title">TPA History</h2>
            </div>

            <!-- Previous Application -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <div class="conditional-field">
                            <label class="form-label">Have you ever applied for a position with TPA, Inc. before?</label>
                            <div class="radio-group">
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbPreviouslyAppliedYes" runat="server" GroupName="PreviouslyApplied" />
                                    <label for="<%= rbPreviouslyAppliedYes.ClientID %>">Yes</label>
                                </div>
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbPreviouslyAppliedNo" runat="server" GroupName="PreviouslyApplied" />
                                    <label for="<%= rbPreviouslyAppliedNo.ClientID %>">No</label>
                                </div>
                            </div>
                        </div>
                        <div class="conditional-field">
                            <label class="form-label">If yes, when?</label>
                            <asp:TextBox ID="txtPreviousApplicationDate" runat="server" CssClass="conditional-input" MaxLength="100" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- Previous Work -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <div class="conditional-field">
                            <label class="form-label">Have you ever worked for TPA, Inc. before?</label>
                            <div class="radio-group">
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbPreviouslyWorkedYes" runat="server" GroupName="PreviouslyWorked" />
                                    <label for="<%= rbPreviouslyWorkedYes.ClientID %>">Yes</label>
                                </div>
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbPreviouslyWorkedNo" runat="server" GroupName="PreviouslyWorked" />
                                    <label for="<%= rbPreviouslyWorkedNo.ClientID %>">No</label>
                                </div>
                            </div>
                        </div>
                        <div class="conditional-field">
                            <label class="form-label">If yes, when?</label>
                            <asp:TextBox ID="txtPreviousWorkDate" runat="server" CssClass="conditional-input" MaxLength="100" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- Family Members -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <div class="conditional-field">
                            <label class="form-label">Do you have any family members employed by TPA, Inc.</label>
                            <div class="radio-group">
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbFamilyMembersYes" runat="server" GroupName="FamilyMembers" />
                                    <label for="<%= rbFamilyMembersYes.ClientID %>">Yes</label>
                                </div>
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbFamilyMembersNo" runat="server" GroupName="FamilyMembers" />
                                    <label for="<%= rbFamilyMembersNo.ClientID %>">No</label>
                                </div>
                            </div>
                        </div>
                        <div class="conditional-field">
                            <label class="form-label">If yes, who?</label>
                            <asp:TextBox ID="txtFamilyMemberDetails" runat="server" CssClass="conditional-input" MaxLength="500" />
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

            <!-- Citizenship -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <div class="conditional-field">
                            <label class="form-label">Are you a U.S. citizen or Permanent Resident?</label>
                            <div class="radio-group">
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbUSCitizenYes" runat="server" GroupName="USCitizen" />
                                    <label for="<%= rbUSCitizenYes.ClientID %>">Yes</label>
                                </div>
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbUSCitizenNo" runat="server" GroupName="USCitizen" />
                                    <label for="<%= rbUSCitizenNo.ClientID %>">No</label>
                                </div>
                            </div>
                        </div>
                        <div class="conditional-field">
                            <label class="form-label">Alien # (if no)</label>
                            <asp:TextBox ID="txtAlienNumber" runat="server" CssClass="conditional-input" MaxLength="50" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- Work Authorization -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <div class="conditional-field">
                            <label class="form-label">Or otherwise legally entitled to work in the U.S.A.</label>
                            <div class="radio-group">
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbLegallyEntitledYes" runat="server" GroupName="LegallyEntitled" />
                                    <label for="<%= rbLegallyEntitledYes.ClientID %>">Yes</label>
                                </div>
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbLegallyEntitledNo" runat="server" GroupName="LegallyEntitled" />
                                    <label for="<%= rbLegallyEntitledNo.ClientID %>">No</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Age Verification -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <div class="conditional-field">
                            <label class="form-label">Are you 18 years or older?</label>
                            <div class="radio-group">
                                <div class="radio-option">
                                    <asp:RadioButton ID="rb18OrOlderYes" runat="server" GroupName="Age18OrOlder" />
                                    <label for="<%= rb18OrOlderYes.ClientID %>">Yes</label>
                                </div>
                                <div class="radio-option">
                                    <asp:RadioButton ID="rb18OrOlderNo" runat="server" GroupName="Age18OrOlder" />
                                    <label for="<%= rb18OrOlderNo.ClientID %>">No</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Military & Criminal History Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">security</i>
                </div>
                <h2 class="section-title">Military & Criminal History</h2>
            </div>

            <!-- Military Service -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <div class="conditional-field">
                            <label class="form-label">Have you ever served in the U.S. Armed Forces?</label>
                            <div class="radio-group">
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbArmedForcesYes" runat="server" GroupName="ArmedForces" />
                                    <label for="<%= rbArmedForcesYes.ClientID %>">Yes</label>
                                </div>
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbArmedForcesNo" runat="server" GroupName="ArmedForces" />
                                    <label for="<%= rbArmedForcesNo.ClientID %>">No</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Criminal Conviction -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <div class="conditional-field">
                            <label class="form-label">Have you ever been convicted of a crime (i.e. misdemeanor or felony)?</label>
                            <div class="radio-group">
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbConvictedYes" runat="server" GroupName="Convicted" />
                                    <label for="<%= rbConvictedYes.ClientID %>">Yes</label>
                                </div>
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbConvictedNo" runat="server" GroupName="Convicted" />
                                    <label for="<%= rbConvictedNo.ClientID %>">No</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Criminal Details -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <label class="form-label">If yes, please give details including dates, charges, and dispositions</label>
                        <table class="criminal-history-table">
                            <thead>
                                <tr>
                                    <th>DATE</th>
                                    <th>CHARGE</th>
                                    <th>STATUS OR OUTCOME</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td><asp:TextBox ID="txtCriminalDate1" runat="server" CssClass="form-input" MaxLength="100" /></td>
                                    <td><asp:TextBox ID="txtCriminalCharge1" runat="server" CssClass="form-input" MaxLength="500" /></td>
                                    <td><asp:TextBox ID="txtCriminalOutcome1" runat="server" CssClass="form-input" MaxLength="500" /></td>
                                </tr>
                                <tr>
                                    <td><asp:TextBox ID="txtCriminalDate2" runat="server" CssClass="form-input" MaxLength="100" /></td>
                                    <td><asp:TextBox ID="txtCriminalCharge2" runat="server" CssClass="form-input" MaxLength="500" /></td>
                                    <td><asp:TextBox ID="txtCriminalOutcome2" runat="server" CssClass="form-input" MaxLength="500" /></td>
                                </tr>
                                <tr>
                                    <td><asp:TextBox ID="txtCriminalDate3" runat="server" CssClass="form-input" MaxLength="100" /></td>
                                    <td><asp:TextBox ID="txtCriminalCharge3" runat="server" CssClass="form-input" MaxLength="500" /></td>
                                    <td><asp:TextBox ID="txtCriminalOutcome3" runat="server" CssClass="form-input" MaxLength="500" /></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <!-- Abuse Registry -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <div class="conditional-field">
                            <label class="form-label">Does your name appear on an abuse registry?</label>
                            <div class="radio-group">
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbAbuseRegistryYes" runat="server" GroupName="AbuseRegistry" />
                                    <label for="<%= rbAbuseRegistryYes.ClientID %>">Yes</label>
                                </div>
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbAbuseRegistryNo" runat="server" GroupName="AbuseRegistry" />
                                    <label for="<%= rbAbuseRegistryNo.ClientID %>">No</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Found Guilty of Abuse -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <div class="conditional-field">
                            <label class="form-label">Have you ever been found guilty abusing, neglecting, or mistreating individuals?</label>
                            <div class="radio-group">
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbFoundGuiltyYes" runat="server" GroupName="FoundGuilty" />
                                    <label for="<%= rbFoundGuiltyYes.ClientID %>">Yes</label>
                                </div>
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbFoundGuiltyNo" runat="server" GroupName="FoundGuilty" />
                                    <label for="<%= rbFoundGuiltyNo.ClientID %>">No</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- License Revocation -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <div class="conditional-field">
                            <label class="form-label">Has your license and/or certification in any health care profession ever been revoked, suspended, limited, or placed on probation or discipline in any state?</label>
                            <div class="radio-group">
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbLicenseRevokedYes" runat="server" GroupName="LicenseRevoked" />
                                    <label for="<%= rbLicenseRevokedYes.ClientID %>">Yes</label>
                                </div>
                                <div class="radio-option">
                                    <asp:RadioButton ID="rbLicenseRevokedNo" runat="server" GroupName="LicenseRevoked" />
                                    <label for="<%= rbLicenseRevokedNo.ClientID %>">No</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Education Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">school</i>
                </div>
                <h2 class="section-title">Education</h2>
            </div>

            <div class="form-row">
                <div class="form-col-full">
                    <table class="education-table">
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
                                <td><asp:TextBox ID="txtElementarySchool" runat="server" CssClass="form-input" MaxLength="500" /></td>
                                <td><asp:TextBox ID="txtHighSchool" runat="server" CssClass="form-input" MaxLength="500" /></td>
                                <td><asp:TextBox ID="txtUndergraduateSchool" runat="server" CssClass="form-input" MaxLength="500" /></td>
                                <td><asp:TextBox ID="txtGraduateSchool" runat="server" CssClass="form-input" MaxLength="500" /></td>
                            </tr>
                            <tr>
                                <td><strong>Years Completed (circle)</strong></td>
                                <td><asp:TextBox ID="txtElementaryYears" runat="server" CssClass="form-input" MaxLength="20" /></td>
                                <td><asp:TextBox ID="txtHighSchoolYears" runat="server" CssClass="form-input" MaxLength="20" /></td>
                                <td><asp:TextBox ID="txtUndergraduateYears" runat="server" CssClass="form-input" MaxLength="20" /></td>
                                <td><asp:TextBox ID="txtGraduateYears" runat="server" CssClass="form-input" MaxLength="20" /></td>
                            </tr>
                            <tr>
                                <td><strong>Diploma/Degree</strong></td>
                                <td class="checkbox-cell"><asp:CheckBox ID="chkElementaryDiploma" runat="server" /></td>
                                <td class="checkbox-cell"><asp:CheckBox ID="chkHighSchoolDiploma" runat="server" /></td>
                                <td class="checkbox-cell"><asp:CheckBox ID="chkUndergraduateDiploma" runat="server" /></td>
                                <td class="checkbox-cell"><asp:CheckBox ID="chkGraduateDiploma" runat="server" /></td>
                            </tr>
                            <tr>
                                <td><strong>Major/Minor</strong></td>
                                <td><asp:TextBox ID="txtElementaryMajor" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                <td><asp:TextBox ID="txtHighSchoolMajor" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                <td><asp:TextBox ID="txtUndergraduateMajor" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                <td><asp:TextBox ID="txtGraduateMajor" runat="server" CssClass="form-input" MaxLength="200" /></td>
                            </tr>
                            <tr>
                                <td><strong>Describe any specialized Training or skills</strong></td>
                                <td><textarea runat="server" id="txtElementaryTraining" class="form-input" maxlength="1000"></textarea></td>
                                <td><textarea runat="server" id="txtHighSchoolTraining" class="form-input" maxlength="1000"></textarea></td>
                                <td><textarea runat="server" id="txtUndergraduateTraining" class="form-input" maxlength="1000"></textarea></td>
                                <td><textarea runat="server" id="txtGraduateTraining" class="form-input" maxlength="1000"></textarea></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>

            <!-- Special Skills -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <label class="form-label">Special knowledge, skills, and abilities you wish considered. Include equipment or machines you operate, computer, languages, laboratory techniques, etc. If applying for secretarial/typist positions, indicate typing speed (WPM)</label>
                        <asp:TextBox ID="txtSpecialSkills" runat="server" CssClass="form-input form-input-full" 
                            TextMode="MultiLine" Rows="4" MaxLength="2000" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Licenses and Certifications Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">card_membership</i>
                </div>
                <h2 class="section-title">Special Skills, Training, Certifications, and/or Licensures</h2>
            </div>

            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <label class="form-label">List fields of work for which you are licensed, registered, or certified. Please include license numbers, dates, and sources of issuance.</label>
                        <table class="education-table">
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
                                    <td><asp:TextBox ID="txtLicenseType1" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                    <td><asp:TextBox ID="txtLicenseState1" runat="server" CssClass="form-input" MaxLength="10" /></td>
                                    <td><asp:TextBox ID="txtLicenseNumber1" runat="server" CssClass="form-input" MaxLength="100" /></td>
                                    <td><asp:TextBox ID="txtLicenseExpiration1" runat="server" CssClass="form-input" TextMode="Date" /></td>
                                </tr>
                                <tr>
                                    <td><asp:TextBox ID="txtLicenseType2" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                    <td><asp:TextBox ID="txtLicenseState2" runat="server" CssClass="form-input" MaxLength="10" /></td>
                                    <td><asp:TextBox ID="txtLicenseNumber2" runat="server" CssClass="form-input" MaxLength="100" /></td>
                                    <td><asp:TextBox ID="txtLicenseExpiration2" runat="server" CssClass="form-input" TextMode="Date" /></td>
                                </tr>
                                <tr>
                                    <td><asp:TextBox ID="txtLicenseType3" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                    <td><asp:TextBox ID="txtLicenseState3" runat="server" CssClass="form-input" MaxLength="10" /></td>
                                    <td><asp:TextBox ID="txtLicenseNumber3" runat="server" CssClass="form-input" MaxLength="100" /></td>
                                    <td><asp:TextBox ID="txtLicenseExpiration3" runat="server" CssClass="form-input" TextMode="Date" /></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <!-- DIDD Training -->
            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group">
                        <label class="form-label">List Dept. of Intellectual and Developmental Disabilities (DIDD) training/classes you have:</label>
                        <asp:TextBox ID="txtDIDDTraining" runat="server" CssClass="form-input form-input-full" 
                            TextMode="MultiLine" Rows="3" MaxLength="1000" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Employment Experience Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">work_history</i>
                </div>
                <h2 class="section-title">Employment Experience</h2>
            </div>

            <p class="form-note">Start with your present or last job. Include any job-related military service assignments and volunteer activities that have given you experience related to your job. Please explain any extended lapses between employments.</p>

            <!-- Employment History 1 -->
            <div class="employment-section">
                <h4>Most Recent Employment</h4>
                <div class="employment-grid">
                    <div class="form-group">
                        <label class="form-label">Employer</label>
                        <asp:TextBox ID="txtEmployer1" runat="server" CssClass="form-input" MaxLength="500" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Dates Employed From / To</label>
                        <div style="display: flex; gap: 10px;">
                            <asp:TextBox ID="txtEmploymentFrom1" runat="server" CssClass="form-input" TextMode="Date" />
                            <span style="align-self: center;">to</span>
                            <asp:TextBox ID="txtEmploymentTo1" runat="server" CssClass="form-input" TextMode="Date" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Job Title</label>
                        <asp:TextBox ID="txtJobTitle1" runat="server" CssClass="form-input" MaxLength="200" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Supervisor</label>
                        <asp:TextBox ID="txtSupervisor1" runat="server" CssClass="form-input" MaxLength="200" />
                    </div>
                    <div class="form-group employment-full-row">
                        <label class="form-label">Address</label>
                        <asp:TextBox ID="txtEmployerAddress1" runat="server" CssClass="form-input" MaxLength="500" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">City, State, Zip Code</label>
                        <asp:TextBox ID="txtEmployerCityStateZip1" runat="server" CssClass="form-input" MaxLength="200" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Telephone Number(s)</label>
                        <asp:TextBox ID="txtEmployerPhone1" runat="server" CssClass="form-input" MaxLength="100" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Hourly Rate of Pay - Starting</label>
                        <asp:TextBox ID="txtStartingPay1" runat="server" CssClass="form-input" MaxLength="20" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Final</label>
                        <asp:TextBox ID="txtFinalPay1" runat="server" CssClass="form-input" MaxLength="20" />
                    </div>
                    <div class="form-group employment-full-row">
                        <label class="form-label">Title/Work Performed</label>
                        <asp:TextBox ID="txtWorkPerformed1" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2" MaxLength="1000" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Are you still employed (Yes or No)</label>
                        <div class="radio-group">
                            <div class="radio-option">
                                <asp:RadioButton ID="rbStillEmployed1Yes" runat="server" GroupName="StillEmployed1" />
                                <label for="<%= rbStillEmployed1Yes.ClientID %>">Yes</label>
                            </div>
                            <div class="radio-option">
                                <asp:RadioButton ID="rbStillEmployed1No" runat="server" GroupName="StillEmployed1" />
                                <label for="<%= rbStillEmployed1No.ClientID %>">No</label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Eligible for Rehire (Yes or No)</label>
                        <div class="radio-group">
                            <div class="radio-option">
                                <asp:RadioButton ID="rbEligibleRehire1Yes" runat="server" GroupName="EligibleRehire1" />
                                <label for="<%= rbEligibleRehire1Yes.ClientID %>">Yes</label>
                            </div>
                            <div class="radio-option">
                                <asp:RadioButton ID="rbEligibleRehire1No" runat="server" GroupName="EligibleRehire1" />
                                <label for="<%= rbEligibleRehire1No.ClientID %>">No</label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group employment-full-row">
                        <label class="form-label">Reason for Leaving</label>
                        <asp:TextBox ID="txtReasonLeaving1" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2" MaxLength="1000" />
                    </div>
                </div>
            </div>

            <!-- Employment History 2 -->
            <div class="employment-section">
                <h4>Previous Employment</h4>
                <div class="employment-grid">
                    <div class="form-group">
                        <label class="form-label">Employer</label>
                        <asp:TextBox ID="txtEmployer2" runat="server" CssClass="form-input" MaxLength="500" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Dates Employed From / To</label>
                        <div style="display: flex; gap: 10px;">
                            <asp:TextBox ID="txtEmploymentFrom2" runat="server" CssClass="form-input" TextMode="Date" />
                            <span style="align-self: center;">to</span>
                            <asp:TextBox ID="txtEmploymentTo2" runat="server" CssClass="form-input" TextMode="Date" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Job Title</label>
                        <asp:TextBox ID="txtJobTitle2" runat="server" CssClass="form-input" MaxLength="200" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Supervisor</label>
                        <asp:TextBox ID="txtSupervisor2" runat="server" CssClass="form-input" MaxLength="200" />
                    </div>
                    <div class="form-group employment-full-row">
                        <label class="form-label">Address</label>
                        <asp:TextBox ID="txtEmployerAddress2" runat="server" CssClass="form-input" MaxLength="500" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">City, State, Zip Code</label>
                        <asp:TextBox ID="txtEmployerCityStateZip2" runat="server" CssClass="form-input" MaxLength="200" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Telephone Number(s)</label>
                        <asp:TextBox ID="txtEmployerPhone2" runat="server" CssClass="form-input" MaxLength="100" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Hourly Rate of Pay - Starting</label>
                        <asp:TextBox ID="txtStartingPay2" runat="server" CssClass="form-input" MaxLength="20" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Final</label>
                        <asp:TextBox ID="txtFinalPay2" runat="server" CssClass="form-input" MaxLength="20" />
                    </div>
                    <div class="form-group employment-full-row">
                        <label class="form-label">Title/Work Performed</label>
                        <asp:TextBox ID="txtWorkPerformed2" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2" MaxLength="1000" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Eligible for Rehire (Yes or No)</label>
                        <div class="radio-group">
                            <div class="radio-option">
                                <asp:RadioButton ID="rbEligibleRehire2Yes" runat="server" GroupName="EligibleRehire2" />
                                <label for="<%= rbEligibleRehire2Yes.ClientID %>">Yes</label>
                            </div>
                            <div class="radio-option">
                                <asp:RadioButton ID="rbEligibleRehire2No" runat="server" GroupName="EligibleRehire2" />
                                <label for="<%= rbEligibleRehire2No.ClientID %>">No</label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group employment-full-row">
                        <label class="form-label">Reason for Leaving</label>
                        <asp:TextBox ID="txtReasonLeaving2" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2" MaxLength="1000" />
                    </div>
                </div>
            </div>

            <!-- Employment History 3 -->
            <div class="employment-section">
                <h4>Earlier Employment</h4>
                <div class="employment-grid">
                    <div class="form-group">
                        <label class="form-label">Employer</label>
                        <asp:TextBox ID="txtEmployer3" runat="server" CssClass="form-input" MaxLength="500" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Dates Employed From / To</label>
                        <div style="display: flex; gap: 10px;">
                            <asp:TextBox ID="txtEmploymentFrom3" runat="server" CssClass="form-input" TextMode="Date" />
                            <span style="align-self: center;">to</span>
                            <asp:TextBox ID="txtEmploymentTo3" runat="server" CssClass="form-input" TextMode="Date" />
                        </div>
                    </div>
                    <div class="form-group">
                        <label class="form-label">Job Title</label>
                        <asp:TextBox ID="txtJobTitle3" runat="server" CssClass="form-input" MaxLength="200" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Supervisor</label>
                        <asp:TextBox ID="txtSupervisor3" runat="server" CssClass="form-input" MaxLength="200" />
                    </div>
                    <div class="form-group employment-full-row">
                        <label class="form-label">Address</label>
                        <asp:TextBox ID="txtEmployerAddress3" runat="server" CssClass="form-input" MaxLength="500" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">City, State, Zip Code</label>
                        <asp:TextBox ID="txtEmployerCityStateZip3" runat="server" CssClass="form-input" MaxLength="200" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Telephone Number(s)</label>
                        <asp:TextBox ID="txtEmployerPhone3" runat="server" CssClass="form-input" MaxLength="100" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Hourly Rate of Pay - Starting</label>
                        <asp:TextBox ID="txtStartingPay3" runat="server" CssClass="form-input" MaxLength="20" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Final</label>
                        <asp:TextBox ID="txtFinalPay3" runat="server" CssClass="form-input" MaxLength="20" />
                    </div>
                    <div class="form-group employment-full-row">
                        <label class="form-label">Title/Work Performed</label>
                        <asp:TextBox ID="txtWorkPerformed3" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2" MaxLength="1000" />
                    </div>
                    <div class="form-group">
                        <label class="form-label">Eligible for Rehire (Yes or No)</label>
                        <div class="radio-group">
                            <div class="radio-option">
                                <asp:RadioButton ID="rbEligibleRehire3Yes" runat="server" GroupName="EligibleRehire3" />
                                <label for="<%= rbEligibleRehire3Yes.ClientID %>">Yes</label>
                            </div>
                            <div class="radio-option">
                                <asp:RadioButton ID="rbEligibleRehire3No" runat="server" GroupName="EligibleRehire3" />
                                <label for="<%= rbEligibleRehire3No.ClientID %>">No</label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group employment-full-row">
                        <label class="form-label">Reason for Leaving</label>
                        <asp:TextBox ID="txtReasonLeaving3" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2" MaxLength="1000" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Professional References Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">people</i>
                </div>
                <h2 class="section-title">Request for Professional References</h2>
            </div>

            <p class="form-note">
                To further process your application, please provide three (3) personal references who can 
                provide professional reference about your character, ability and suitability for the position 
                you have applied for.<br />
                <strong>*At least one (1) personal reference must have known you for at least 5 years</strong>
            </p>

            <!-- Reference 1 -->
            <div class="reference-section">
                <h4>Professional Reference #1</h4>
                <div class="form-row">
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">First and last name:</label>
                            <asp:TextBox ID="txtReference1Name" runat="server" CssClass="form-input" MaxLength="200" />
                        </div>
                    </div>
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">Phone number:</label>
                            <asp:TextBox ID="txtReference1Phone" runat="server" CssClass="form-input" MaxLength="20" />
                        </div>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">E-mail address:</label>
                            <asp:TextBox ID="txtReference1Email" runat="server" CssClass="form-input" MaxLength="255" />
                        </div>
                    </div>
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">How many years have you known personal reference?</label>
                            <asp:TextBox ID="txtReference1Years" runat="server" CssClass="form-input" MaxLength="10" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- Reference 2 -->
            <div class="reference-section">
                <h4>Professional Reference #2</h4>
                <div class="form-row">
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">First and last name:</label>
                            <asp:TextBox ID="txtReference2Name" runat="server" CssClass="form-input" MaxLength="200" />
                        </div>
                    </div>
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">Phone number:</label>
                            <asp:TextBox ID="txtReference2Phone" runat="server" CssClass="form-input" MaxLength="20" />
                        </div>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">E-mail address:</label>
                            <asp:TextBox ID="txtReference2Email" runat="server" CssClass="form-input" MaxLength="255" />
                        </div>
                    </div>
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">How many years have you known personal reference?</label>
                            <asp:TextBox ID="txtReference2Years" runat="server" CssClass="form-input" MaxLength="10" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- Reference 3 -->
            <div class="reference-section">
                <h4>Professional Reference #3</h4>
                <div class="form-row">
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">First and last name:</label>
                            <asp:TextBox ID="txtReference3Name" runat="server" CssClass="form-input" MaxLength="200" />
                        </div>
                    </div>
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">Phone number:</label>
                            <asp:TextBox ID="txtReference3Phone" runat="server" CssClass="form-input" MaxLength="20" />
                        </div>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">E-mail address:</label>
                            <asp:TextBox ID="txtReference3Email" runat="server" CssClass="form-input" MaxLength="255" />
                        </div>
                    </div>
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">How many years have you known personal reference?</label>
                            <asp:TextBox ID="txtReference3Years" runat="server" CssClass="form-input" MaxLength="10" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Background Investigation Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">search</i>
                </div>
                <h2 class="section-title">Background Investigation</h2>
            </div>

            <div class="authorization-section">
                <h3>Tennessee Personal Assistance, Inc. - Nashville</h3>
                <h3>Disclosure and Authorization Form</h3>
                <h4>(1) Background Investigation Questionnaire:</h4>

                <!-- Personal Information for Background Check -->
                <div class="form-row">
                    <div class="form-col-wide">
                        <div class="form-group">
                            <label class="form-label">Name: (Last) (First) (Middle Name)</label>
                            <asp:TextBox ID="txtBGFullName" runat="server" CssClass="form-input" MaxLength="300" />
                        </div>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-col-full">
                        <div class="form-group">
                            <label class="form-label">Address: (Street) (City) (State) (Zip Code)</label>
                            <asp:TextBox ID="txtBGAddress" runat="server" CssClass="form-input" MaxLength="500" />
                        </div>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">Social Security Number:</label>
                            <asp:TextBox ID="txtBGSSN" runat="server" CssClass="form-input" MaxLength="11" />
                        </div>
                    </div>
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">Telephone Number:</label>
                            <asp:TextBox ID="txtBGPhone" runat="server" CssClass="form-input" MaxLength="20" />
                        </div>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">Other Name (s): (Used Within the Last 7YRS. E.g. Maiden, Other Married Names)</label>
                            <asp:TextBox ID="txtBGOtherNames" runat="server" CssClass="form-input" MaxLength="200" />
                        </div>
                    </div>
                    <div class="form-col-half">
                        <div class="form-group">
                            <label class="form-label">Year of Name Change</label>
                            <asp:TextBox ID="txtBGNameChangeYear" runat="server" CssClass="form-input" MaxLength="4" />
                        </div>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-col-third">
                        <div class="form-group">
                            <label class="form-label">Driver's License Number:</label>
                            <asp:TextBox ID="txtBGDriversLicense" runat="server" CssClass="form-input" MaxLength="50" />
                        </div>
                    </div>
                    <div class="form-col-third">
                        <div class="form-group">
                            <label class="form-label">State</label>
                            <asp:TextBox ID="txtBGDLState" runat="server" CssClass="form-input" MaxLength="10" />
                        </div>
                    </div>
                    <div class="form-col-third">
                        <div class="form-group">
                            <label class="form-label">Date of Birth:</label>
                            <asp:TextBox ID="txtBGDateOfBirth" runat="server" CssClass="form-input" TextMode="Date" />
                        </div>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-col-full">
                        <div class="form-group">
                            <label class="form-label">Name on Driver's License:</label>
                            <asp:TextBox ID="txtBGNameOnLicense" runat="server" CssClass="form-input" MaxLength="200" />
                        </div>
                    </div>
                </div>

                <!-- Previous Addresses -->
                <div class="form-group">
                    <label class="form-label"><strong>Previous Residential Addresses (Previous 7 years):</strong></label>
                    
                    <div class="address-section">
                        <h4>Former Address:</h4>
                        <div class="address-grid">
                            <div class="form-group">
                                <label class="form-label">Street</label>
                                <asp:TextBox ID="txtPrevAddress1Street" runat="server" CssClass="form-input" MaxLength="500" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">City</label>
                                <asp:TextBox ID="txtPrevAddress1City" runat="server" CssClass="form-input" MaxLength="100" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">State</label>
                                <asp:TextBox ID="txtPrevAddress1State" runat="server" CssClass="form-input" MaxLength="50" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Years Resided</label>
                                <asp:TextBox ID="txtPrevAddress1Years" runat="server" CssClass="form-input" MaxLength="10" />
                            </div>
                        </div>
                    </div>

                    <div class="address-section">
                        <h4>Former Address:</h4>
                        <div class="address-grid">
                            <div class="form-group">
                                <label class="form-label">Street</label>
                                <asp:TextBox ID="txtPrevAddress2Street" runat="server" CssClass="form-input" MaxLength="500" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">City</label>
                                <asp:TextBox ID="txtPrevAddress2City" runat="server" CssClass="form-input" MaxLength="100" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">State</label>
                                <asp:TextBox ID="txtPrevAddress2State" runat="server" CssClass="form-input" MaxLength="50" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Years Resided</label>
                                <asp:TextBox ID="txtPrevAddress2Years" runat="server" CssClass="form-input" MaxLength="10" />
                            </div>
                        </div>
                    </div>

                    <div class="address-section">
                        <h4>Former Address:</h4>
                        <div class="address-grid">
                            <div class="form-group">
                                <label class="form-label">Street</label>
                                <asp:TextBox ID="txtPrevAddress3Street" runat="server" CssClass="form-input" MaxLength="500" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">City</label>
                                <asp:TextBox ID="txtPrevAddress3City" runat="server" CssClass="form-input" MaxLength="100" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">State</label>
                                <asp:TextBox ID="txtPrevAddress3State" runat="server" CssClass="form-input" MaxLength="50" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Years Resided</label>
                                <asp:TextBox ID="txtPrevAddress3Years" runat="server" CssClass="form-input" MaxLength="10" />
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Criminal Background Questions -->
                <div class="form-row">
                    <div class="form-col-full">
                        <div class="form-group">
                            <div class="conditional-field">
                                <label class="form-label">Have you been convicted of any criminal offense, either misdemeanor or felony, other than minor traffic violations in the last 7 years?</label>
                                <div class="radio-group">
                                    <div class="radio-option">
                                        <asp:RadioButton ID="rbBGConvicted7YearsYes" runat="server" GroupName="BGConvicted7Years" />
                                        <label for="<%= rbBGConvicted7YearsYes.ClientID %>">Yes</label>
                                    </div>
                                    <div class="radio-option">
                                        <asp:RadioButton ID="rbBGConvicted7YearsNo" runat="server" GroupName="BGConvicted7Years" />
                                        <label for="<%= rbBGConvicted7YearsNo.ClientID %>">No</label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-col-full">
                        <div class="form-group">
                            <div class="conditional-field">
                                <label class="form-label">Are you currently charged or under investigation for any violation of the law other than minor traffic violations?</label>
                                <div class="radio-group">
                                    <div class="radio-option">
                                        <asp:RadioButton ID="rbBGChargedInvestigationYes" runat="server" GroupName="BGChargedInvestigation" />
                                        <label for="<%= rbBGChargedInvestigationYes.ClientID %>">Yes</label>
                                    </div>
                                    <div class="radio-option">
                                        <asp:RadioButton ID="rbBGChargedInvestigationNo" runat="server" GroupName="BGChargedInvestigation" />
                                        <label for="<%= rbBGChargedInvestigationNo.ClientID %>">No</label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Reference Authorization Section -->
        <div class="form-section">
            <div class="authorization-section">
                <h3>Pre-Employment Reference Check</h3>
                <h3>Release & Authorization to Conduct Reference Checks</h3>

                <div class="authorization-content">
                    <p>The person named below has applied for a position with our company. Their consideration for 
                    employment is largely dependent on this reference form. Below is a signed authorization and 
                    consent from the applicant for our company to obtain reference information. Your prompt 
                    cooperation, time and attention in completing this reference will be greatly appreciated.</p>

                    <h4>Applicant's Authorization, Release and Request for Reference Information</h4>

                    <p>I _________________________________ have applied for a position with TPA, Inc. I authorize all my 
                    current and former employers to provide reference information, including my job performance, my work 
                    record and attendance, the reason(s) for my leaving, my eligibility for rehire and my suitability for the 
                    position I am now seeking. I encourage my current and former employers to provide complete responses 
                    to requests for information, which is believed to be true but not documented. I realize some information 
                    may be complimentary and some may be critical. I promise I will not bring any legal claims or actions 
                    against my current or former employer due to the response to job reference requests. I recognize this 
                    is also a State Statute; which provide my employers with certain protection from such claims. I realize 
                    one is required to give a reference, so I make this commitment to encourage the free exchange of 
                    reference information.</p>

                    <p>I signed this release voluntarily and was not required to do so as part of the application process.</p>
                </div>

                <div class="signature-section">
                    <div class="form-row">
                        <div class="form-col-half">
                            <div class="form-group">
                                <label class="form-label">SSN # XXX-XX- (last 4 digits only)</label>
                                <asp:TextBox ID="txtSSNLast4" runat="server" CssClass="form-input" MaxLength="4" />
                            </div>
                        </div>
                    </div>

                    <p class="form-note">(Please note: For legal compliance and to protect our applicants, a full SSN is not provided. If a full SSN is needed, please call HR @ 615-331-6200)</p>

                    <div class="signature-row">
                        <div class="signature-line"></div>
                        <div class="form-group">
                            <label class="form-label">Date</label>
                            <asp:TextBox ID="txtReferenceAuthDate" runat="server" CssClass="form-input" TextMode="Date" />
                        </div>
                    </div>
                    <div class="signature-label">Applicant's Signature</div>
                </div>
            </div>
        </div>

        <!-- DIDD Authorization Section -->
        <div class="form-section">
            <div class="authorization-section">
                <h3>Tennessee Personal Assistance, Inc</h3>
                <h3>Authorization and General Release for DIDD, Bureau of TennCare & TPA, Inc</h3>

                <div class="authorization-content">
                    <p>I, the undersigned applicant certify and affirm that, to the best of my knowledge and belief;</p>

                    <div class="form-row">
                        <div class="form-col-full">
                            <div class="form-group">
                                <div class="checkbox-group">
                                    <asp:CheckBox ID="chkDIDDNoAbuse" runat="server" CssClass="checkbox-large" />
                                    <label class="checkbox-label" for="<%= chkDIDDNoAbuse.ClientID %>">
                                        I have NOT had a case of abuse, neglect, mistreatment or exploitation substantiated against me
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-col-full">
                            <div class="form-group">
                                <div class="checkbox-group">
                                    <asp:CheckBox ID="chkDIDDHadAbuse" runat="server" CssClass="checkbox-large" />
                                    <label class="checkbox-label" for="<%= chkDIDDHadAbuse.ClientID %>">
                                        I have had a case of abuse, neglect, mistreatment or exploitation substantiated against me
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>

                    <p>As a condition of submitting this application and in order to verify this affirmation, I further 
                    release and authorize Tennessee Personal Assistance, the Tennessee Department of Intellectual 
                    and Developmental Disabilities and the Bureau of TennCare to have full and complete access to 
                    any and all current or prior personnel or investigative records, from any party, person, business, 
                    entity or agency, whether governmental or non-governmental, as pertains to any allegations 
                    against me of abuse, neglect, mistreatment or exploitation and to consider this information as 
                    may be deemed appropriate. This authorization extends to providing any applicable information in 
                    personnel or investigative reports concerning my employment with this employer to my future 
                    employers who may be Providers of DIDD Services</p>

                    <div class="form-row">
                        <div class="form-col-full">
                            <div class="form-group">
                                <label class="form-label">Full Name (Last, First, Middle):</label>
                                <asp:TextBox ID="txtDIDDFullName" runat="server" CssClass="form-input" MaxLength="300" />
                            </div>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-col-half">
                            <div class="form-group">
                                <label class="form-label">SSN #:</label>
                                <asp:TextBox ID="txtDIDDSSN" runat="server" CssClass="form-input" MaxLength="11" />
                            </div>
                        </div>
                        <div class="form-col-half">
                            <div class="form-group">
                                <label class="form-label">Date of Birth:</label>
                                <asp:TextBox ID="txtDIDDDateOfBirth" runat="server" CssClass="form-input" TextMode="Date" />
                            </div>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-col-full">
                            <div class="form-group">
                                <label class="form-label">Driver License or ID #</label>
                                <asp:TextBox ID="txtDIDDLicenseID" runat="server" CssClass="form-input" MaxLength="50" />
                            </div>
                        </div>
                    </div>

                    <div class="signature-section">
                        <div class="signature-row">
                            <div class="signature-line"></div>
                            <div style="width: 200px; text-align: center;">
                                <label class="form-label">Witness</label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Protection from Harm Statement -->
        <div class="form-section">
            <div class="authorization-section">
                <h3>Protection from Harm Statement</h3>

                <div class="authorization-content">
                    <p>I _________________________________ certify and affirm that, to the best of my knowledge and belief;</p>

                    <div class="form-row">
                        <div class="form-col-full">
                            <div class="form-group">
                                <div class="checkbox-group">
                                    <asp:CheckBox ID="chkProtectionNoAbuse" runat="server" CssClass="checkbox-large" />
                                    <label class="checkbox-label" for="<%= chkProtectionNoAbuse.ClientID %>">
                                        I have NOT had a case of abuse, neglect, mistreatment or exploitation substantiated against me
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="form-row">
                        <div class="form-col-full">
                            <div class="form-group">
                                <div class="checkbox-group">
                                    <asp:CheckBox ID="chkProtectionHadAbuse" runat="server" CssClass="checkbox-large" />
                                    <label class="checkbox-label" for="<%= chkProtectionHadAbuse.ClientID %>">
                                        I have had a case of abuse, neglect, mistreatment or exploitation substantiated against me
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>

                    <p>In order to verify this affirmation, I further release and authorize Tennessee Personal 
                    Assistance, the Tennessee Department of Intellectual and Developmental Disabilities and the 
                    Bureau of TennCare to have full and complete access to any and all current or prior personnel 
                    or investigative records as it pertains to substantiated allegations against me of abuse, neglect, 
                    mistreatment or exploitation.</p>

                    <div class="signature-section">
                        <div class="signature-row">
                            <div class="signature-line"></div>
                            <div style="width: 200px; text-align: center;">
                                <label class="form-label">Witness:</label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Final Equal Opportunity Statement -->
        <div class="equal-opportunity-footer">
            <h4>TPA, Inc. is an Equal Opportunity Employer</h4>
            <p>
                Tennessee Law Prohibits Discrimination in Employment: It is illegal to discriminate against any person because of race, color, creed, religion, 
                sex, age, handicap, or national origin in recruitment, training, hiring, discharge, promotion, or any condition, term or privilege of employment.
            </p>

            <div class="form-row">
                <div class="form-col-full">
                    <div class="form-group checkbox-group">
                        <asp:CheckBox ID="chkFinalAcknowledgment" runat="server" CssClass="checkbox-large" />
                        <label class="checkbox-label required" for="<%= chkFinalAcknowledgment.ClientID %>">
                            I certify that all information provided in this application is true, complete, and accurate to the best of my knowledge. 
                            I understand that any false information may result in termination of employment. I authorize TPA, Inc. to verify 
                            any information contained in this application and to contact my references and former employers.
                        </label>
                        <asp:RequiredFieldValidator ID="rfvFinalAcknowledgment" runat="server" ControlToValidate="chkFinalAcknowledgment" 
                            ErrorMessage="You must acknowledge the final statement to submit this application" 
                            CssClass="field-validation-error" Display="Dynamic" />
                    </div>
                </div>
            </div>
        </div>

        <!-- Form Actions -->
        <div class="form-actions">
            <asp:Button ID="btnSaveDraft" runat="server" CssClass="btn-secondary" Text="Save as Draft" OnClick="btnSaveDraft_Click" CausesValidation="false" />
            <asp:Button ID="btnSubmit" runat="server" CssClass="btn-tpa" Text="Submit Application" OnClick="btnSubmit_Click" />
        </div>
    </div>

    <script type="text/javascript">
        // Auto-save functionality
        var autoSaveInterval;
        
        function startAutoSave() {
            autoSaveInterval = setInterval(function() {
                // Trigger save draft without validation
                __doPostBack('<%= btnSaveDraft.UniqueID %>', '');
            }, 120000); // Save every 2 minutes
        }
        
        function stopAutoSave() {
            if (autoSaveInterval) {
                clearInterval(autoSaveInterval);
            }
        }
        
        // Start auto-save when page loads
        $(document).ready(function() {
            startAutoSave();
            
            // Format SSN fields
            $('#<%= txtSSN.ClientID %>, #<%= txtBGSSN.ClientID %>, #<%= txtDIDDSSN.ClientID %>').on('input', function () {
                var value = this.value.replace(/\D/g, '');
                var formattedValue = value.replace(/(\d{3})(\d{2})(\d{4})/, '$1-$2-$3');
                if (formattedValue.length <= 11) {
                    this.value = formattedValue;
                }
            });

            // Format phone number fields
            $('input[id*="Phone"], input[id*="phone"]').on('input', function () {
                var value = this.value.replace(/\D/g, '');
                var formattedValue = value.replace(/(\d{3})(\d{3})(\d{4})/, '($1) $2-$3');
                if (formattedValue.length <= 14) {
                    this.value = formattedValue;
                }
            });
        });

        // Stop auto-save when form is submitted
        function onFormSubmit() {
            stopAutoSave();
        }
    </script>
</asp:Content>