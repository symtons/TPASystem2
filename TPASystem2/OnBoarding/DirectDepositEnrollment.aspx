<%@ Page Title="Direct Deposit Enrollment" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="DirectDepositEnrollment.aspx.cs" Inherits="TPASystem2.OnBoarding.DirectDepositEnrollment" EnableEventValidation="false" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
   <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
<link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
<link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

    <style>
        /* Task-specific styling */
        .mandatory-task-header {
            background: linear-gradient(135deg, #2e7d32 0%, #388e3c 100%);
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
            background: #e8f5e8;
            color: #2e7d32;
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
            border-color: #2e7d32;
            box-shadow: 0 0 0 3px rgba(46, 125, 50, 0.1);
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
            border-color: #2e7d32;
        }
        
        .checkbox-label {
            flex: 1;
            line-height: 1.5;
            cursor: pointer;
            margin: 0;
        }
        
        .bank-preview {
            background: #f8f9fa;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            padding: 1.5rem;
            margin-top: 1rem;
        }
        
        .preview-title {
            font-weight: 600;
            color: #2e7d32;
            margin-bottom: 1rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .preview-item {
            display: flex;
            justify-content: space-between;
            padding: 0.5rem 0;
            border-bottom: 1px solid #e0e0e0;
        }
        
        .preview-item:last-child {
            border-bottom: none;
        }
        
        .preview-label {
            font-weight: 600;
            color: #666;
        }
        
        .preview-value {
            color: #333;
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
            background: #2e7d32;
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
            background: #1b5e20;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(46, 125, 50, 0.3);
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
                <i class="material-icons">account_balance</i>
                Mandatory Task 2 of 3
            </div>
        </div>
        <h1 class="task-title">
            <i class="material-icons">account_balance</i>
            Direct Deposit Enrollment
            <span class="mandatory-badge">Mandatory</span>
        </h1>
        <p class="task-subtitle">Set up direct deposit for your payroll payments</p>
    </div>

    <!-- Success Message -->
    <asp:Panel ID="pnlSuccessMessage" runat="server" CssClass="success-message" Visible="false">
        <i class="material-icons" style="vertical-align: middle; margin-right: 0.5rem;">check_circle</i>
        <strong>Direct deposit enrollment completed successfully!</strong> You will be redirected to your onboarding dashboard.
    </asp:Panel>

    <!-- Form Container -->
    <div class="form-container">
        <!-- Banking Information Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">account_balance</i>
                </div>
                <h2 class="section-title">Banking Information</h2>
            </div>

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required" for="txtBankName">Bank Name</label>
                        <asp:TextBox ID="txtBankName" runat="server" CssClass="form-input" 
                            placeholder="Enter your bank name" MaxLength="100" />
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
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required" for="txtRoutingNumber">Routing Number</label>
                        <asp:TextBox ID="txtRoutingNumber" runat="server" CssClass="form-input" placeholder="9-digit routing number" MaxLength="9" />
                        <div class="form-help">The 9-digit number at the bottom left of your check</div>
                    </div>
                </div>
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required" for="txtAccountNumber">Account Number</label>
                        <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-input" placeholder="Account number" MaxLength="20" />
                        <div class="form-help">Usually 10-12 digits found at the bottom of your check</div>
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="form-col">
                    <div class="form-group">
                        <label class="form-label required" for="txtConfirmAccountNumber">Confirm Account Number</label>
                        <asp:TextBox ID="txtConfirmAccountNumber" runat="server" CssClass="form-input" placeholder="Re-enter account number" MaxLength="20" />
                    </div>
                </div>
                <div class="form-col">
                    <!-- Bank Preview -->
                    <div class="bank-preview">
                        <div class="preview-title">
                            <i class="material-icons">preview</i>
                            Bank Account Preview
                        </div>
                        <div class="preview-item">
                            <span class="preview-label">Bank:</span>
                            <span class="preview-value" id="previewBankName">-</span>
                        </div>
                        <div class="preview-item">
                            <span class="preview-label">Account Type:</span>
                            <span class="preview-value" id="previewAccountType">-</span>
                        </div>
                        <div class="preview-item">
                            <span class="preview-label">Routing Number:</span>
                            <span class="preview-value" id="previewRoutingNumber">-</span>
                        </div>
                        <div class="preview-item">
                            <span class="preview-label">Account Number:</span>
                            <span class="preview-value" id="previewAccountNumber">-</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Authorization Section -->
        <div class="form-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">verified_user</i>
                </div>
                <h2 class="section-title">Authorization & Agreement</h2>
            </div>

            <div class="form-group">
                <div class="checkbox-group">
                    <asp:CheckBox ID="chkDirectDepositAuth" runat="server" />
                    <label class="checkbox-label" for="<%= chkDirectDepositAuth.ClientID %>">
                        <strong>Direct Deposit Authorization:</strong> I authorize TPA to deposit my pay directly into the bank account specified above. 
                        I understand that this authorization will remain in effect until I provide written notice to change or cancel it. 
                        I agree that TPA may reverse any erroneous deposits.
                    </label>
                </div>
            </div>

            <div class="form-group">
                <div class="checkbox-group">
                    <asp:CheckBox ID="chkBankingAccuracy" runat="server" />
                    <label class="checkbox-label" for="<%= chkBankingAccuracy.ClientID %>">
                        <strong>Data Accuracy:</strong> I certify that the banking information provided is accurate and complete. 
                        I understand that incorrect information may result in delayed or misdirected payments, and any associated fees will be my responsibility.
                    </label>
                </div>
            </div>

            <div class="form-group">
                <div class="checkbox-group">
                    <asp:CheckBox ID="chkBankingPrivacy" runat="server" />
                    <label class="checkbox-label" for="<%= chkBankingPrivacy.ClientID %>">
                        <strong>Privacy & Security:</strong> I understand that my banking information will be securely stored and used only for payroll purposes. 
                        TPA will protect this information in accordance with banking regulations and privacy laws.
                    </label>
                </div>
            </div>
        </div>

        <!-- Form Actions -->
        <div class="form-actions">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-secondary" 
                OnClick="btnCancel_Click" />
            <asp:Button ID="btnSubmit" runat="server" Text="Enroll in Direct Deposit" CssClass="btn-primary" 
                OnClick="btnSubmit_Click" />
        </div>
    </div>

    <!-- Loading overlay -->
    <div id="loadingOverlay" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); z-index: 10000; align-items: center; justify-content: center;">
        <div style="background: white; padding: 2rem; border-radius: 12px; text-align: center;">
            <i class="material-icons" style="font-size: 3rem; color: #2e7d32; animation: spin 1s linear infinite;">sync</i>
            <p style="margin: 1rem 0 0 0; font-weight: 600;">Processing your enrollment...</p>
        </div>
    </div>

    <style>
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
    </style>

    <script>
        // Disable all client-side validation
        function Page_ClientValidate() { return true; }
        if (typeof ValidatorOnSubmit === 'function') {
            ValidatorOnSubmit = function () { return true; };
        }

        // Update bank preview as user types
        function updateBankPreview() {
            try {
                var bankName = document.getElementById('<%= txtBankName.ClientID %>').value;
                var accountType = document.getElementById('<%= ddlAccountType.ClientID %>').value;
                var routingNumber = document.getElementById('<%= txtRoutingNumber.ClientID %>').value;
                var accountNumber = document.getElementById('<%= txtAccountNumber.ClientID %>').value;
                
                // Mask account number for preview (show last 4 digits)
                var maskedAccountNumber = accountNumber.length > 4 ? 
                    '****' + accountNumber.slice(-4) : '-';
                
                document.getElementById('previewBankName').textContent = bankName || '-';
                document.getElementById('previewAccountType').textContent = accountType || '-';
                document.getElementById('previewRoutingNumber').textContent = routingNumber || '-';
                document.getElementById('previewAccountNumber').textContent = maskedAccountNumber;
            } catch (e) {
                console.log('Preview update error:', e);
            }
        }

        // Show loading overlay on form submit
        function showLoading() {
            document.getElementById('loadingOverlay').style.display = 'flex';
        }

        // Add event listeners
        document.addEventListener('DOMContentLoaded', function() {
            try {
                // Add input listeners for bank preview
                var inputs = ['<%= txtBankName.ClientID %>', '<%= ddlAccountType.ClientID %>', '<%= txtRoutingNumber.ClientID %>', '<%= txtAccountNumber.ClientID %>'];
                inputs.forEach(function (inputId) {
                    var element = document.getElementById(inputId);
                    if (element) {
                        element.addEventListener('input', updateBankPreview);
                        element.addEventListener('change', updateBankPreview);
                    }
                });

                // Add form submit handler
                var form = document.forms[0];
                if (form) {
                    form.addEventListener('submit', function (e) {
                        showLoading();
                    });
                }

                // Initialize bank preview
                updateBankPreview();
            } catch (e) {
                console.log('Initialization error:', e);
            }
        });
    </script>
</asp:Content>