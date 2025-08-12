<%@ Page Title="Request Leave" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="EmployeeRequestLeave.aspx.cs" Inherits="TPASystem2.LeaveManagement.EmployeeRequestLeave" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

    <!-- Welcome Header -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">event_note</i>
                    Request Time Off
                </h1>
                <p class="welcome-subtitle">Submit a new leave request for Program Director approval</p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">person</i>
                        <span><asp:Literal ID="litEmployeeName" runat="server"></asp:Literal></span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">badge</i>
                        <span><asp:Literal ID="litEmployeeNumber" runat="server"></asp:Literal></span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">supervisor_account</i>
                        <span>Requires Program Director Approval</span>
                    </div>
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnBackToPortal" runat="server" Text="Back to Leave Portal" 
                    CssClass="btn btn-outline-light" OnClick="btnBackToPortal_Click" />
            </div>
        </div>
    </div>

    <!-- Alert Messages -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <i class="material-icons">info</i>
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Main Request Form -->
    <div class="time-status-card">
        <div class="status-header">
            <h3>
                <i class="material-icons">assignment</i>
                Leave Request Details
            </h3>
            <div class="status-indicator">
                <asp:Literal ID="litFormMode" runat="server" Text="New Request"></asp:Literal>
            </div>
        </div>

        <asp:Panel ID="pnlRequestForm" runat="server">
            <div class="form-grid">
                <!-- Leave Type Selection -->
                <div class="form-group">
                    <label class="form-label" for="<%= ddlLeaveType.ClientID %>">
                        <i class="material-icons">category</i>
                        Leave Type *
                    </label>
                    <asp:DropDownList ID="ddlLeaveType" runat="server" CssClass="form-control" 
                        AutoPostBack="true" OnSelectedIndexChanged="ddlLeaveType_SelectedIndexChanged">
                        <asp:ListItem Value="" Text="Select leave type..."></asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvLeaveType" runat="server" 
                        ControlToValidate="ddlLeaveType" ErrorMessage="Please select a leave type" 
                        CssClass="field-validation-error" Display="Dynamic" />
                </div>

                <!-- Available Balance Display -->
                <div class="form-group">
                    <label class="form-label">
                        <i class="material-icons">account_balance_wallet</i>
                        Available Balance
                    </label>
                    <div class="balance-display">
                        <asp:TextBox ID="txtAvailableBalance" runat="server" CssClass="form-control balance-readonly" 
                            ReadOnly="true" Text="Select leave type first"></asp:TextBox>
                    </div>
                </div>

                <!-- Start Date -->
                <div class="form-group">
                    <label class="form-label" for="<%= txtStartDate.ClientID %>">
                        <i class="material-icons">event</i>
                        Start Date *
                    </label>
                    <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control date-picker" 
                        TextMode="Date" AutoPostBack="true" OnTextChanged="txtStartDate_TextChanged"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" 
                        ControlToValidate="txtStartDate" ErrorMessage="Start date is required" 
                        CssClass="field-validation-error" Display="Dynamic" />
                    <asp:CompareValidator ID="cvStartDate" runat="server" 
                        ControlToValidate="txtStartDate" Operator="GreaterThanEqual" Type="Date"
                        ErrorMessage="Start date cannot be in the past" 
                        CssClass="field-validation-error" Display="Dynamic" />
                </div>

                <!-- End Date -->
                <div class="form-group">
                    <label class="form-label" for="<%= txtEndDate.ClientID %>">
                        <i class="material-icons">event</i>
                        End Date *
                    </label>
                    <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control date-picker" 
                        TextMode="Date" AutoPostBack="true" OnTextChanged="txtEndDate_TextChanged"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" 
                        ControlToValidate="txtEndDate" ErrorMessage="End date is required" 
                        CssClass="field-validation-error" Display="Dynamic" />
                    <asp:CompareValidator ID="cvEndDate" runat="server" 
                        ControlToValidate="txtEndDate" ControlToCompare="txtStartDate" 
                        Operator="GreaterThanEqual" Type="Date"
                        ErrorMessage="End date must be after start date" 
                        CssClass="field-validation-error" Display="Dynamic" />
                </div>

                <!-- Half Day Option -->
                <div class="form-group checkbox-group">
                    <asp:CheckBox ID="chkHalfDay" runat="server" CssClass="form-check-input" 
                        AutoPostBack="true" OnCheckedChanged="chkHalfDay_CheckedChanged" />
                    <label class="form-check-label" for="<%= chkHalfDay.ClientID %>">
                        <i class="material-icons">schedule</i>
                        Half Day Request
                    </label>
                    <small class="form-text text-muted">Check if this is a half-day leave request</small>
                </div>

                <!-- Days Calculation -->
                <div class="form-group">
                    <label class="form-label">
                        <i class="material-icons">today</i>
                        Total Days Requested
                    </label>
                    <div class="days-calculation">
                        <asp:TextBox ID="txtDaysRequested" runat="server" CssClass="form-control days-readonly" 
                            ReadOnly="true" Text="0"></asp:TextBox>
                        <small class="calculation-note">Business days only (excludes weekends)</small>
                    </div>
                </div>

                <!-- Reason -->
                <div class="form-group full-width">
                    <label class="form-label" for="<%= txtReason.ClientID %>">
                        <i class="material-icons">description</i>
                        Reason for Leave *
                    </label>
                    <asp:TextBox ID="txtReason" runat="server" CssClass="form-control" 
                        TextMode="MultiLine" Rows="4" MaxLength="1000" 
                        placeholder="Please provide a detailed reason for your leave request..."></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvReason" runat="server" 
                        ControlToValidate="txtReason" ErrorMessage="Please provide a reason for your leave" 
                        CssClass="field-validation-error" Display="Dynamic" />
                    <div class="character-count">
                        <small class="text-muted">
                            <span id="reasonCharCount">0</span>/1000 characters
                        </small>
                    </div>
                </div>
            </div>

            <!-- Program Director Authorization Notice -->
            <div class="authorization-info-card">
                <div class="auth-header">
                    <i class="material-icons">verified_user</i>
                    <h4>Authorization Required</h4>
                </div>
                <div class="auth-content">
                    <p><strong>Your request will be sent to your Program Director for approval.</strong></p>
                    <ul>
                        <li>All leave requests require Program Director authorization</li>
                        <li>You will receive email notifications about your request status</li>
                        <li>Emergency leave may require additional documentation</li>
                        <li>Please submit requests at least 48 hours in advance when possible</li>
                    </ul>
                </div>
            </div>

            <!-- Form Actions -->
            <div class="form-actions">
                <asp:Button ID="btnSubmitRequest" runat="server" Text="Submit for Approval" 
                    CssClass="btn btn-primary btn-large" OnClick="btnSubmitRequest_Click" 
                    OnClientClick="return validateForm();" />
                
                <asp:Button ID="btnSaveDraft" runat="server" Text="Save as Draft" 
                    CssClass="btn btn-outline-secondary btn-large" OnClick="btnSaveDraft_Click" 
                    CausesValidation="false" />
                
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                    CssClass="btn btn-outline-danger" OnClick="btnCancel_Click" 
                    CausesValidation="false" />
            </div>
        </asp:Panel>

        <!-- Success Panel -->
        <asp:Panel ID="pnlSuccess" runat="server" Visible="false" CssClass="success-panel">
            <div class="success-content">
                <i class="material-icons success-icon">check_circle</i>
                <h3>Leave Request Submitted Successfully!</h3>
                <p>Your leave request has been submitted for Program Director approval.</p>
                <div class="success-details">
                    <div class="detail-item">
                        <strong>Request ID:</strong> <asp:Literal ID="litRequestId" runat="server"></asp:Literal>
                    </div>
                    <div class="detail-item">
                        <strong>Leave Type:</strong> <asp:Literal ID="litSubmittedLeaveType" runat="server"></asp:Literal>
                    </div>
                    <div class="detail-item">
                        <strong>Dates:</strong> <asp:Literal ID="litSubmittedDates" runat="server"></asp:Literal>
                    </div>
                    <div class="detail-item">
                        <strong>Status:</strong> <span class="status-badge status-pending">Pending Approval</span>
                    </div>
                </div>
                <div class="success-actions">
                    <asp:Button ID="btnViewMyRequests" runat="server" Text="View My Requests" 
                        CssClass="btn btn-primary" OnClick="btnViewMyRequests_Click" />
                    <asp:Button ID="btnCreateAnother" runat="server" Text="Create Another Request" 
                        CssClass="btn btn-outline-primary" OnClick="btnCreateAnother_Click" />
                </div>
            </div>
        </asp:Panel>
    </div>

    <!-- Hidden Fields -->
    <asp:HiddenField ID="hfEditingRequestId" runat="server" />
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Scripts" runat="server">
    <script type="text/javascript">
        $(document).ready(function() {
            // Initialize date pickers with minimum date
            initializeDatePickers();
            
            // Setup character counting
            setupCharacterCounting();
            
            // Initialize form validation
            setupFormValidation();
        });

        function initializeDatePickers() {
            const today = new Date().toISOString().split('T')[0];
            $('.date-picker').attr('min', today);
        }

        function setupCharacterCounting() {
            const reasonTextbox = document.getElementById('<%= txtReason.ClientID %>');
            const charCount = document.getElementById('reasonCharCount');
            
            if (reasonTextbox && charCount) {
                reasonTextbox.addEventListener('input', function() {
                    const count = this.value.length;
                    charCount.textContent = count;
                    
                    if (count > 900) {
                        charCount.style.color = '#dc3545';
                    } else {
                        charCount.style.color = '#6c757d';
                    }
                });
                
                // Initial count
                charCount.textContent = reasonTextbox.value.length;
            }
        }

        function setupFormValidation() {
            // Add real-time validation feedback
            $('.form-control').on('blur', function() {
                validateField(this);
            });
        }

        function validateField(field) {
            const $field = $(field);
            const value = $field.val().trim();
            
            // Remove existing validation classes
            $field.removeClass('is-valid is-invalid');
            
            // Basic validation
            if (value === '' && $field.prop('required')) {
                $field.addClass('is-invalid');
            } else if (value !== '') {
                $field.addClass('is-valid');
            }
        }

        function validateForm() {
            let isValid = true;
            let errorMessage = '';

            // Check leave type
            const leaveType = document.getElementById('<%= ddlLeaveType.ClientID %>').value;
            if (!leaveType) {
                errorMessage = 'Please select a leave type.';
                isValid = false;
            }

            // Check dates
            const startDate = document.getElementById('<%= txtStartDate.ClientID %>').value;
            const endDate = document.getElementById('<%= txtEndDate.ClientID %>').value;
            
            if (!startDate || !endDate) {
                errorMessage = 'Please select both start and end dates.';
                isValid = false;
            } else if (new Date(startDate) > new Date(endDate)) {
                errorMessage = 'End date must be after start date.';
                isValid = false;
            }

            // Check reason
            const reason = document.getElementById('<%= txtReason.ClientID %>').value.trim();
            if (!reason) {
                errorMessage = 'Please provide a reason for your leave request.';
                isValid = false;
            }

            // Check available balance
            const daysRequested = parseFloat(document.getElementById('<%= txtDaysRequested.ClientID %>').value) || 0;
            const availableBalance = document.getElementById('<%= txtAvailableBalance.ClientID %>').value;
            
            if (daysRequested > 0 && availableBalance.includes('days')) {
                const available = parseFloat(availableBalance.split(' ')[0]) || 0;
                if (daysRequested > available) {
                    errorMessage = 'Requested days exceed available balance.';
                    isValid = false;
                }
            }

            if (!isValid) {
                alert(errorMessage);
                return false;
            }

            // Show loading state
            const submitBtn = document.getElementById('<%= btnSubmitRequest.ClientID %>');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.textContent = 'Submitting...';
                
                // Re-enable after 10 seconds to prevent permanent disable
                setTimeout(() => {
                    submitBtn.disabled = false;
                    submitBtn.textContent = 'Submit for Approval';
                }, 10000);
            }

            return true;
        }

        // Auto-calculate business days
        function calculateBusinessDays(startDate, endDate) {
            if (!startDate || !endDate) return 0;
            
            const start = new Date(startDate);
            const end = new Date(endDate);
            let businessDays = 0;
            
            for (let date = new Date(start); date <= end; date.setDate(date.getDate() + 1)) {
                const dayOfWeek = date.getDay();
                if (dayOfWeek !== 0 && dayOfWeek !== 6) { // Not Sunday or Saturday
                    businessDays++;
                }
            }
            
            return businessDays;
        }
    </script>
</asp:Content>