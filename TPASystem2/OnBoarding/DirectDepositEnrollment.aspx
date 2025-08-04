<%@ Page Title="Direct Deposit Enrollment" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="DirectDepositEnrollment.aspx.cs" Inherits="TPASystem2.OnBoarding.DirectDepositEnrollment" %>

<asp:Content ID="DirectDepositContent" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    
    <!-- Custom Styles -->
    <style>
        .mandatory-task-header {
            background: linear-gradient(135deg, #2196f3 0%, #42a5f5 100%);
            color: white;
            padding: 2.5rem;
            border-radius: 16px;
            margin-bottom: 2rem;
            position: relative;
            overflow: hidden;
        }
        
        .mandatory-task-header::before {
            content: '💳';
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
        
        .info-section {
            background: linear-gradient(135deg, #e3f2fd 0%, #f3e5f5 100%);
            border: 2px solid #2196f3;
            border-radius: 16px;
            padding: 2rem;
            margin-bottom: 2rem;
        }
        
        .info-header {
            display: flex;
            align-items: center;
            gap: 1rem;
            margin-bottom: 1.5rem;
        }
        
        .info-icon {
            background: #2196f3;
            color: white;
            width: 50px;
            height: 50px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.5rem;
        }
        
        .info-title {
            color: #1565c0;
            font-size: 1.4rem;
            font-weight: 600;
            margin: 0;
        }
        
        .info-content {
            color: #1976d2;
            line-height: 1.6;
        }
        
        .info-list {
            list-style: none;
            padding: 0;
            margin: 1rem 0;
        }
        
        .info-list li {
            padding: 0.5rem 0;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .info-list li::before {
            content: '✓';
            color: #4caf50;
            font-weight: bold;
            width: 20px;
            text-align: center;
        }
        
        .form-section {
            background: white;
            border-radius: 16px;
            padding: 2rem;
            margin-bottom: 2rem;
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
            border: 2px solid #e3f2fd;
        }
        
        .section-header {
            display: flex;
            align-items: center;
            gap: 1rem;
            margin-bottom: 2rem;
            padding-bottom: 1rem;
            border-bottom: 2px solid #e3f2fd;
        }
        
        .section-icon {
            background: linear-gradient(135deg, #2196f3 0%, #42a5f5 100%);
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
            color: #1565c0;
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
            color: #1565c0;
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
            border-color: #2196f3;
            background: white;
            box-shadow: 0 0 0 3px rgba(33, 150, 243, 0.1);
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
            border-color: #2196f3;
            background: white;
        }
        
        .form-help {
            font-size: 0.9rem;
            color: #666;
            margin-top: 0.5rem;
            font-style: italic;
        }
        
        .form-help.security {
            color: #4caf50;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .radio-group {
            display: flex;
            flex-direction: column;
            gap: 1rem;
        }
        
        .radio-option {
            display: flex;
            align-items: center;
            gap: 1rem;
            padding: 1rem;
            background: #f8f9fa;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            cursor: pointer;
            transition: all 0.3s ease;
        }
        
        .radio-option:hover {
            background: #e3f2fd;
            border-color: #2196f3;
        }
        
        .radio-option.selected {
            background: #e3f2fd;
            border-color: #2196f3;
        }
        
        .radio-option input[type="radio"] {
            width: 20px;
            height: 20px;
            margin: 0;
            accent-color: #2196f3;
        }
        
        .radio-label {
            flex: 1;
        }
        
        .radio-title {
            font-weight: 600;
            color: #1565c0;
            margin-bottom: 0.5rem;
        }
        
        .radio-description {
            color: #666;
            font-size: 0.9rem;
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
            accent-color: #2196f3;
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
            border-top: 2px solid #e3f2fd;
        }
        
        .btn-primary {
            background: linear-gradient(135deg, #2196f3 0%, #42a5f5 100%);
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
            background: linear-gradient(135deg, #1976d2 0%, #2196f3 100%);
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(33, 150, 243, 0.3);
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
            background: #e3f2fd;
            color: #1565c0;
            border-radius: 25px;
            font-weight: 600;
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
        
        .bank-preview {
            background: #f0f7ff;
            border: 2px solid #2196f3;
            border-radius: 12px;
            padding: 1.5rem;
            margin-top: 1rem;
        }
        
        .bank-preview h4 {
            color: #1565c0;
            margin: 0 0 1rem 0;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .bank-info {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 1rem;
        }
        
        .bank-detail {
            display: flex;
            flex-direction: column;
        }
        
        .bank-detail-label {
            font-size: 0.9rem;
            color: #666;
            margin-bottom: 0.25rem;
        }
        
        .bank-detail-value {
            font-weight: 600;
            color: #1565c0;
        }
    </style>

    <!-- Page Header -->
    <div class="mandatory-task-header">
        <div class="progress-tracker">
            <div class="progress-step">
                <i class="material-icons">account_balance</i>
                Mandatory Task 2 of 3
            </div>
        </div>
        <h1 class="task-title">
            <i class="material-icons">account_balance_wallet</i>
            Direct Deposit Enrollment
            <span class="mandatory-badge">Mandatory</span>
        </h1>
        <p class="task-subtitle">Set up your bank account for direct deposit payroll</p>
    </div>

    <!-- Success Message -->
    <asp:Panel ID="pnlSuccessMessage" runat="server" CssClass="success-message" Visible="false">
        <i class="material-icons" style="vertical-align: middle; margin-right: 0.5rem;">check_circle</i>
        <strong>Direct deposit enrollment completed successfully!</strong> You will be redirected to your onboarding dashboard.
    </asp:Panel>

    <!-- Information Section -->
    <div class="info-section">
        <div class="info-header">
            <div class="info-icon">
                <i class="material-icons">info</i>
            </div>
            <h2 class="info-title">Why Direct Deposit?</h2>
        </div>
        <div class="info-content">
            <p>Direct deposit is the fastest, safest, and most convenient way to receive your paycheck. Your earnings are automatically deposited into your bank account on payday.</p>
            <ul class="info-list">
                <li>No more waiting for checks to clear</li>
                <li>Secure and encrypted transactions</li>
                <li>Access to your money immediately on payday</li>
                <li>Environmentally friendly - no paper checks</li>
                <li>Never lose or misplace a paycheck</li>
            </ul>
            <p><strong>What you'll need:</strong> Your bank's routing number and your account number. You can find these on a check or by logging into your online banking.</p>
        </div>
    </div>

    <!-- Deposit Type Selection -->
    <div class="form-section">
        <div class="section-header">
            <div class="section-icon">
                <i class="material-icons">settings</i>
            </div>
            <h2 class="section-title">Deposit Options</h2>
        </div>

        <div class="form-group">
            <label class="form-label required">How would you like to receive your paycheck?</label>
            <div class="radio-group">
                <div class="radio-option" onclick="selectDepositType('full')">
                    <asp:RadioButton ID="rbFullDeposit" runat="server" GroupName="depositType" />
                    <div class="radio-label">
                        <div class="radio-title">Full Direct Deposit</div>
                        <div class="radio-description">Deposit your entire paycheck into one account (recommended)</div>
                    </div>
                </div>
                <div class="radio-option" onclick="selectDepositType('split')">
                    <asp:RadioButton ID="rbSplitDeposit" runat="server" GroupName="depositType" />
                    <div class="radio-label">
                        <div class="radio-title">Split Deposit</div>
                        <div class="radio-description">Divide your paycheck between multiple accounts (checking/savings)</div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Primary Account Section -->
    <div class="form-section">
        <div class="section-header">
            <div class="section-icon">
                <i class="material-icons">account_balance</i>
            </div>
            <h2 class="section-title">Primary Bank Account</h2>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="txtBankName">Bank Name</label>
                    <asp:TextBox ID="txtBankName" runat="server" CssClass="form-input" placeholder="e.g., Chase Bank, Wells Fargo" MaxLength="100" />
                    <asp:RequiredFieldValidator ID="rfvBankName" runat="server" ControlToValidate="txtBankName" 
                        ErrorMessage="Bank name is required" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="ddlAccountType">Account Type</label>
                    <asp:DropDownList ID="ddlAccountType" runat="server" CssClass="form-select">
                        <asp:ListItem Value="" Text="Select Account Type" />
                        <asp:ListItem Value="Checking" Text="Checking Account" />
                        <asp:ListItem Value="Savings" Text="Savings Account" />
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvAccountType" runat="server" ControlToValidate="ddlAccountType" InitialValue=""
                        ErrorMessage="Account type is required" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="txtRoutingNumber">Routing Number</label>
                    <asp:TextBox ID="txtRoutingNumber" runat="server" CssClass="form-input" placeholder="9-digit routing number" MaxLength="9" />
                    <asp:RequiredFieldValidator ID="rfvRoutingNumber" runat="server" ControlToValidate="txtRoutingNumber" 
                        ErrorMessage="Routing number is required" CssClass="error-message" Display="Dynamic" />
                    <asp:RegularExpressionValidator ID="revRoutingNumber" runat="server" ControlToValidate="txtRoutingNumber"
                        ValidationExpression="^\d{9}$" ErrorMessage="Routing number must be exactly 9 digits" CssClass="error-message" Display="Dynamic" />
                    <div class="form-help">The 9-digit number at the bottom left of your check</div>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="txtAccountNumber">Account Number</label>
                    <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-input" placeholder="Your account number" MaxLength="20" />
                    <asp:RequiredFieldValidator ID="rfvAccountNumber" runat="server" ControlToValidate="txtAccountNumber" 
                        ErrorMessage="Account number is required" CssClass="error-message" Display="Dynamic" />
                    <div class="form-help">Usually 8-12 digits, found on your check or bank statement</div>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label required" for="txtConfirmAccountNumber">Confirm Account Number</label>
                    <asp:TextBox ID="txtConfirmAccountNumber" runat="server" CssClass="form-input" placeholder="Re-enter your account number" MaxLength="20" />
                    <asp:RequiredFieldValidator ID="rfvConfirmAccountNumber" runat="server" ControlToValidate="txtConfirmAccountNumber" 
                        ErrorMessage="Please confirm your account number" CssClass="error-message" Display="Dynamic" />
                    <asp:CompareValidator ID="cvAccountNumber" runat="server" ControlToValidate="txtConfirmAccountNumber" 
                        ControlToCompare="txtAccountNumber" ErrorMessage="Account numbers do not match" CssClass="error-message" Display="Dynamic" />
                    <div class="form-help security">
                        <i class="material-icons">security</i>
                        For your security, please confirm your account number
                    </div>
                </div>
            </div>
        </div>

        <!-- Deposit Amount for Primary Account (Split Deposit) -->
        <asp:Panel ID="pnlPrimaryAmount" runat="server" Visible="false">
            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label" for="ddlPrimaryAmountType">Deposit Amount</label>
                        <asp:DropDownList ID="ddlPrimaryAmountType" runat="server" CssClass="form-select" onchange="togglePrimaryAmount()">
                            <asp:ListItem Value="remainder" Text="Remainder (after other deductions)" Selected="True" />
                            <asp:ListItem Value="fixed" Text="Fixed Dollar Amount" />
                            <asp:ListItem Value="percentage" Text="Percentage of Net Pay" />
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label" for="txtPrimaryAmount">Amount</label>
                        <asp:TextBox ID="txtPrimaryAmount" runat="server" CssClass="form-input" TextMode="Number" step="0.01" Enabled="false" />
                        <div class="form-help">Leave blank for remainder option</div>
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Bank Account Preview -->
        <asp:Panel ID="pnlBankPreview" runat="server" CssClass="bank-preview">
            <h4>
                <i class="material-icons">preview</i>
                Account Summary
            </h4>
            <div class="bank-info">
                <div class="bank-detail">
                    <span class="bank-detail-label">Bank Name</span>
                    <span class="bank-detail-value" id="previewBankName">-</span>
                </div>
                <div class="bank-detail">
                    <span class="bank-detail-label">Account Type</span>
                    <span class="bank-detail-value" id="previewAccountType">-</span>
                </div>
                <div class="bank-detail">
                    <span class="bank-detail-label">Routing Number</span>
                    <span class="bank-detail-value" id="previewRoutingNumber">-</span>
                </div>
                <div class="bank-detail">
                    <span class="bank-detail-label">Account Number</span>
                    <span class="bank-detail-value" id="previewAccountNumber">-</span>
                </div>
            </div>
        </asp:Panel>
    </div>

    <!-- Secondary Account Section (Split Deposit Only) -->
    <asp:Panel ID="pnlSecondaryAccount" runat="server" CssClass="form-section" Visible="false">
        <div class="section-header">
            <div class="section-icon">
                <i class="material-icons">savings</i>
            </div>
            <h2 class="section-title">Secondary Account (Optional)</h2>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label" for="txtSecondaryBankName">Bank Name</label>
                    <asp:TextBox ID="txtSecondaryBankName" runat="server" CssClass="form-input" placeholder="e.g., Credit Union, Savings Bank" MaxLength="100" />
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label" for="ddlSecondaryAccountType">Account Type</label>
                    <asp:DropDownList ID="ddlSecondaryAccountType" runat="server" CssClass="form-select">
                        <asp:ListItem Value="" Text="Select Account Type" />
                        <asp:ListItem Value="Checking" Text="Checking Account" />
                        <asp:ListItem Value="Savings" Text="Savings Account" />
                    </asp:DropDownList>
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label" for="txtSecondaryRoutingNumber">Routing Number</label>
                    <asp:TextBox ID="txtSecondaryRoutingNumber" runat="server" CssClass="form-input" placeholder="9-digit routing number" MaxLength="9" />
                    <asp:RegularExpressionValidator ID="revSecondaryRoutingNumber" runat="server" ControlToValidate="txtSecondaryRoutingNumber"
                        ValidationExpression="^\d{9}$" ErrorMessage="Routing number must be exactly 9 digits" CssClass="error-message" Display="Dynamic" />
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label" for="txtSecondaryAccountNumber">Account Number</label>
                    <asp:TextBox ID="txtSecondaryAccountNumber" runat="server" CssClass="form-input" placeholder="Your account number" MaxLength="20" />
                </div>
            </div>
        </div>

        <div class="form-row">
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label" for="ddlSecondaryAmountType">Deposit Amount</label>
                    <asp:DropDownList ID="ddlSecondaryAmountType" runat="server" CssClass="form-select" onchange="toggleSecondaryAmount()">
                        <asp:ListItem Value="fixed" Text="Fixed Dollar Amount" Selected="True" />
                        <asp:ListItem Value="percentage" Text="Percentage of Net Pay" />
                    </asp:DropDownList>
                </div>
            </div>
            <div class="form-col">
                <div class="form-group">
                    <label class="form-label" for="txtSecondaryAmount">Amount</label>
                    <asp:TextBox ID="txtSecondaryAmount" runat="server" CssClass="form-input" TextMode="Number" step="0.01" placeholder="0.00" />
                    <div class="form-help">Amount to deposit in secondary account</div>
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Acknowledgments Section -->
    <div class="form-section">
        <div class="section-header">
            <div class="section-icon">
                <i class="material-icons">fact_check</i>
            </div>
            <h2 class="section-title">Authorization & Agreement</h2>
        </div>

        <div class="form-group">
            <div class="checkbox-group">
                <asp:CheckBox ID="chkAuthorization" runat="server" />
                <label class="checkbox-label" for="<%= chkAuthorization.ClientID %>">
                    <strong>Direct Deposit Authorization:</strong> I authorize TPA to deposit my pay directly into the account(s) specified above. I understand that this authorization will remain in effect until I provide written notice to change or cancel it. I agree that TPA may reverse any erroneous deposits.
                </label>
            </div>
            <asp:RequiredFieldValidator ID="rfvAuthorization" runat="server" ControlToValidate="chkAuthorization" 
                ErrorMessage="You must authorize direct deposit to continue" CssClass="error-message" Display="Dynamic" />
        </div>

        <div class="form-group">
            <div class="checkbox-group">
                <asp:CheckBox ID="chkDataAccuracy" runat="server" />
                <label class="checkbox-label" for="<%= chkDataAccuracy.ClientID %>">
                    <strong>Data Accuracy:</strong> I certify that the banking information provided is accurate and complete. I understand that incorrect information may result in delayed or misdirected payments, and any associated fees will be my responsibility.
                </label>
            </div>
            <asp:RequiredFieldValidator ID="rfvDataAccuracy" runat="server" ControlToValidate="chkDataAccuracy" 
                ErrorMessage="Data accuracy certification is required" CssClass="error-message" Display="Dynamic" />
        </div>

        <div class="form-group">
            <div class="checkbox-group">
                <asp:CheckBox ID="chkPrivacyConsent" runat="server" />
                <label class="checkbox-label" for="<%= chkPrivacyConsent.ClientID %>">
                    <strong>Privacy & Security:</strong> I understand that my banking information will be securely stored and used only for payroll purposes. TPA will protect this information in accordance with banking regulations and privacy laws.
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
        <asp:Button ID="btnSubmit" runat="server" Text="Enroll in Direct Deposit" CssClass="btn-primary" 
            OnClick="btnSubmit_Click" />
    </div>

    <!-- Loading overlay -->
    <div id="loadingOverlay" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); z-index: 10000; align-items: center; justify-content: center;">
        <div style="background: white; padding: 2rem; border-radius: 12px; text-align: center;">
            <i class="material-icons" style="font-size: 3rem; color: #2196f3; animation: spin 1s linear infinite;">sync</i>
            <p style="margin: 1rem 0 0 0; font-weight: 600;">Setting up your direct deposit...</p>
        </div>
    </div>

    <style>
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
    </style>

    <script>
        // Deposit type selection
        function selectDepositType(type) {
            var fullRadio = document.getElementById('<%= rbFullDeposit.ClientID %>');
            var splitRadio = document.getElementById('<%= rbSplitDeposit.ClientID %>');
            var primaryAmountPanel = document.getElementById('<%= pnlPrimaryAmount.ClientID %>');
            var secondaryAccountPanel = document.getElementById('<%= pnlSecondaryAccount.ClientID %>');
            
            if (type === 'full') {
                fullRadio.checked = true;
                splitRadio.checked = false;
                primaryAmountPanel.style.display = 'none';
                secondaryAccountPanel.style.display = 'none';
            } else {
                fullRadio.checked = false;
                splitRadio.checked = true;
                primaryAmountPanel.style.display = 'block';
                secondaryAccountPanel.style.display = 'block';
            }
            
            // Update radio option styling
            var radioOptions = document.querySelectorAll('.radio-option');
            radioOptions.forEach(function(option) {
                option.classList.remove('selected');
            });
            event.currentTarget.classList.add('selected');
        }

        // Toggle primary amount input
        function togglePrimaryAmount() {
            var dropdown = document.getElementById('<%= ddlPrimaryAmountType.ClientID %>');
            var amountInput = document.getElementById('<%= txtPrimaryAmount.ClientID %>');
            
            if (dropdown.value === 'remainder') {
                amountInput.disabled = true;
                amountInput.value = '';
            } else {
                amountInput.disabled = false;
            }
        }

        // Toggle secondary amount input
        function toggleSecondaryAmount() {
            var dropdown = document.getElementById('<%= ddlSecondaryAmountType.ClientID %>');
            var amountInput = document.getElementById('<%= txtSecondaryAmount.ClientID %>');
            
            if (dropdown.value === 'percentage') {
                amountInput.placeholder = '0-100';
                amountInput.max = '100';
            } else {
                amountInput.placeholder = '0.00';
                amountInput.removeAttribute('max');
            }
        }

        // Update bank preview
        function updateBankPreview() {
            var bankName = document.getElementById('<%= txtBankName.ClientID %>').value || '-';
            var accountType = document.getElementById('<%= ddlAccountType.ClientID %>').value || '-';
            var routingNumber = document.getElementById('<%= txtRoutingNumber.ClientID %>').value || '-';
            var accountNumber = document.getElementById('<%= txtAccountNumber.ClientID %>').value;
            
            // Mask account number for security
            var maskedAccountNumber = accountNumber ? 
                '****' + accountNumber.slice(-4) : '-';
            
            document.getElementById('previewBankName').textContent = bankName;
            document.getElementById('previewAccountType').textContent = accountType;
            document.getElementById('previewRoutingNumber').textContent = routingNumber;
            document.getElementById('previewAccountNumber').textContent = maskedAccountNumber;
        }

        // Show loading overlay on form submit
        function showLoading() {
            document.getElementById('loadingOverlay').style.display = 'flex';
        }

        // Add event listeners
        document.addEventListener('DOMContentLoaded', function() {
            // Add input listeners for bank preview
            var inputs = ['<%= txtBankName.ClientID %>', '<%= ddlAccountType.ClientID %>', '<%= txtRoutingNumber.ClientID %>', '<%= txtAccountNumber.ClientID %>'];
            inputs.forEach(function(inputId) {
                var element = document.getElementById(inputId);
                if (element) {
                    element.addEventListener('input', updateBankPreview);
                    element.addEventListener('change', updateBankPreview);
                }
            });

            // Add form submit handler
            var form = document.forms[0];
            if (form) {
                form.addEventListener('submit', function(e) {
                    if (Page_ClientValidate && Page_ClientValidate()) {
                        showLoading();
                    }
                });
            }

            // Initialize bank preview
            updateBankPreview();
        });
    </script>
</asp:Content>