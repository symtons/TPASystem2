<%@ Page Title="Request Leave" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="RequestLeave.aspx.cs" Inherits="TPASystem2.LeaveManagement.RequestLeave" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    
    <!-- Page Header -->
    <div class="page-header">
        <div class="page-title-wrapper">
            <h1 class="page-title">
                <i class="material-icons">event_note</i>
                Request Leave
            </h1>
            <p class="page-subtitle">Submit a new leave request</p>
        </div>
        <div class="page-actions">
            <asp:Button ID="btnBackToDashboard" runat="server" Text="Back to Dashboard" 
                       CssClass="btn btn-outline waves-effect waves-light" 
                       OnClick="btnBackToDashboard_Click" />
        </div>
    </div>

    <!-- Message Panel -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert-panel">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Leave Request Form -->
    <div class="content-card">
        <div class="card-header">
            <h3>Leave Request Details</h3>
        </div>
        <div class="card-content">
            <asp:Panel ID="pnlRequestForm" runat="server">
                <div class="form-grid">
                    <!-- Employee Information (Read Only) -->
                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">Employee Name</label>
                            <asp:TextBox ID="txtEmployeeName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Department</label>
                            <asp:TextBox ID="txtDepartment" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>

                    <!-- Leave Type -->
                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label required">Leave Type <span class="required-asterisk">*</span></label>
                            <asp:DropDownList ID="ddlLeaveType" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlLeaveType_SelectedIndexChanged">
                                <asp:ListItem Value="">Select Leave Type</asp:ListItem>
                                <asp:ListItem Value="Vacation">Vacation</asp:ListItem>
                                <asp:ListItem Value="Sick">Sick Leave</asp:ListItem>
                                <asp:ListItem Value="Personal">Personal Leave</asp:ListItem>
                                <asp:ListItem Value="Maternity">Maternity Leave</asp:ListItem>
                                <asp:ListItem Value="Paternity">Paternity Leave</asp:ListItem>
                                <asp:ListItem Value="Bereavement">Bereavement Leave</asp:ListItem>
                                <asp:ListItem Value="Emergency">Emergency Leave</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvLeaveType" runat="server" 
                                ControlToValidate="ddlLeaveType" 
                                ErrorMessage="Please select a leave type" 
                                CssClass="field-validation-error" 
                                Display="Dynamic" />
                        </div>
                        <div class="form-group">
                            <label class="form-label">Available Balance</label>
                            <asp:TextBox ID="txtAvailableBalance" runat="server" CssClass="form-control" ReadOnly="true" Text="--"></asp:TextBox>
                        </div>
                    </div>

                    <!-- Date Range -->
                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label required">Start Date <span class="required-asterisk">*</span></label>
                            <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control datepicker" 
                                        TextMode="Date" AutoPostBack="true" OnTextChanged="txtStartDate_TextChanged"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" 
                                ControlToValidate="txtStartDate" 
                                ErrorMessage="Please select a start date" 
                                CssClass="field-validation-error" 
                                Display="Dynamic" />
                            <asp:CompareValidator ID="cvStartDate" runat="server" 
                                ControlToValidate="txtStartDate" 
                                Type="Date" 
                                Operator="GreaterThanEqual" 
                                ErrorMessage="Start date cannot be in the past" 
                                CssClass="field-validation-error" 
                                Display="Dynamic" />
                        </div>
                        <div class="form-group">
                            <label class="form-label required">End Date <span class="required-asterisk">*</span></label>
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control datepicker" 
                                        TextMode="Date" AutoPostBack="true" OnTextChanged="txtEndDate_TextChanged"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" 
                                ControlToValidate="txtEndDate" 
                                ErrorMessage="Please select an end date" 
                                CssClass="field-validation-error" 
                                Display="Dynamic" />
                            <asp:CompareValidator ID="cvEndDate" runat="server" 
                                ControlToValidate="txtEndDate" 
                                ControlToCompare="txtStartDate" 
                                Type="Date" 
                                Operator="GreaterThanEqual" 
                                ErrorMessage="End date must be on or after start date" 
                                CssClass="field-validation-error" 
                                Display="Dynamic" />
                        </div>
                    </div>

                    <!-- Days Calculation -->
                    <div class="form-row">
                        <div class="form-group">
                            <label class="form-label">Total Days Requested</label>
                            <asp:TextBox ID="txtDaysRequested" runat="server" CssClass="form-control" ReadOnly="true" Text="0"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Half Day Option</label>
                            <asp:CheckBox ID="chkHalfDay" runat="server" Text="This is a half-day request" AutoPostBack="true" OnCheckedChanged="chkHalfDay_CheckedChanged" />
                        </div>
                    </div>

                    <!-- Reason -->
                    <div class="form-row full-width">
                        <div class="form-group">
                            <label class="form-label required">Reason <span class="required-asterisk">*</span></label>
                            <asp:TextBox ID="txtReason" runat="server" CssClass="form-control" 
                                        TextMode="MultiLine" Rows="4" 
                                        placeholder="Please provide a reason for your leave request..."></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvReason" runat="server" 
                                ControlToValidate="txtReason" 
                                ErrorMessage="Please provide a reason for your leave request" 
                                CssClass="field-validation-error" 
                                Display="Dynamic" />
                        </div>
                    </div>

                    <!-- Form Actions -->
                    <div class="form-actions">
                        <asp:Button ID="btnSubmitRequest" runat="server" Text="Submit Request" 
                                   CssClass="btn btn-primary waves-effect waves-light" 
                                   OnClick="btnSubmitRequest_Click" />
                        <asp:Button ID="btnSaveDraft" runat="server" Text="Save as Draft" 
                                   CssClass="btn btn-secondary waves-effect waves-light" 
                                   OnClick="btnSaveDraft_Click" CausesValidation="false" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                                   CssClass="btn btn-outline waves-effect waves-light" 
                                   OnClick="btnCancel_Click" CausesValidation="false" />
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>

    <!-- Leave Policy Information -->
    <div class="content-card">
        <div class="card-header">
            <h3>Leave Policy Guidelines</h3>
        </div>
        <div class="card-content">
            <div class="leave-policy-grid">
                <div class="policy-item">
                    <h4>Vacation Leave</h4>
                    <p>Must be requested at least 2 weeks in advance. Maximum 3 consecutive weeks.</p>
                </div>
                <div class="policy-item">
                    <h4>Sick Leave</h4>
                    <p>Can be requested on short notice. Medical certificate required for 3+ days.</p>
                </div>
                <div class="policy-item">
                    <h4>Personal Leave</h4>
                    <p>Should be requested at least 1 week in advance when possible.</p>
                </div>
                <div class="policy-item">
                    <h4>Emergency Leave</h4>
                    <p>For unexpected situations. Manager approval required within 24 hours.</p>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // Set minimum date to today
        document.addEventListener('DOMContentLoaded', function() {
            var today = new Date().toISOString().split('T')[0];
            var startDate = document.getElementById('<%= txtStartDate.ClientID %>');
            var endDate = document.getElementById('<%= txtEndDate.ClientID %>');

            if (startDate) {
                startDate.setAttribute('min', today);
            }
            if (endDate) {
                endDate.setAttribute('min', today);
            }
        });
    </script>
</asp:Content>