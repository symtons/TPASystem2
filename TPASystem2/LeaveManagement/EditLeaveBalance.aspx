<%@ Page Title="Edit Leave Balance" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="EditLeaveBalance.aspx.cs" Inherits="TPASystem2.LeaveManagement.EditLeaveBalance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    <style>
        /* ===============================================
   EDIT LEAVE BALANCE STYLES
   Add these styles to tpa-common.css
   =============================================== */

/* Balance Summary Grid */
.balance-summary-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 1.5rem;
    margin-top: 1.5rem;
}

.balance-summary-item {
    background: white;
    border-radius: 12px;
    padding: 1.5rem;
    border: 2px solid #e5e7eb;
    transition: all 0.3s ease;
    display: flex;
    align-items: center;
    gap: 1rem;
    position: relative;
    overflow: hidden;
}

.balance-summary-item::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    width: 4px;
    height: 100%;
    background: #e5e7eb;
    transition: all 0.3s ease;
}

.balance-summary-item:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
}

.balance-summary-item.allocated::before {
    background: linear-gradient(135deg, #10b981, #059669);
}

.balance-summary-item.used::before {
    background: linear-gradient(135deg, #f59e0b, #d97706);
}

.balance-summary-item.remaining::before {
    background: linear-gradient(135deg, #3b82f6, #2563eb);
}

.summary-icon {
    width: 50px;
    height: 50px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    background: #f8fafc;
    color: #64748b;
    font-size: 1.25rem;
}

.balance-summary-item.allocated .summary-icon {
    background: #dcfce7;
    color: #059669;
}

.balance-summary-item.used .summary-icon {
    background: #fef3c7;
    color: #d97706;
}

.balance-summary-item.remaining .summary-icon {
    background: #dbeafe;
    color: #2563eb;
}

.summary-content {
    flex: 1;
}

.summary-number {
    font-size: 2rem;
    font-weight: 700;
    color: #1e293b;
    line-height: 1;
    margin-bottom: 0.25rem;
}

.summary-label {
    font-size: 0.9rem;
    font-weight: 500;
    color: #374151;
}

/* Form Container */
.form-container {
    margin-bottom: 2rem;
}

.form-card {
    background: white;
    border-radius: 16px;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
    border: 1px solid #e5e7eb;
    overflow: hidden;
}

.form-header {
    background: #f8fafc;
    padding: 2rem;
    border-bottom: 1px solid #e5e7eb;
}

.form-header h3 {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    margin: 0 0 0.5rem 0;
    color: #1e293b;
    font-size: 1.5rem;
    font-weight: 600;
}

.form-header h3 .material-icons {
    color: #3b82f6;
    font-size: 1.75rem;
}

.form-header p {
    margin: 0;
    color: #64748b;
    font-size: 0.9rem;
}

.form-content {
    padding: 2rem;
}

.form-row {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 1.5rem;
    margin-bottom: 1.5rem;
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
    display: flex;
    align-items: center;
    gap: 0.5rem;
    font-size: 0.9rem;
    font-weight: 600;
    color: #374151;
}

.form-label.required::after {
    content: '*';
    color: #ef4444;
    margin-left: 0.25rem;
}

.form-label .material-icons {
    font-size: 1rem;
    color: #64748b;
}

.form-control {
    width: 100%;
    padding: 0.75rem;
    border: 2px solid #e5e7eb;
    border-radius: 8px;
    font-size: 0.9rem;
    transition: all 0.2s ease;
    background: white;
    font-family: inherit;
}

.form-control:focus {
    outline: none;
    border-color: #3b82f6;
    box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.form-control:hover {
    border-color: #d1d5db;
}

.form-help {
    font-size: 0.8rem;
    color: #64748b;
    margin-top: 0.25rem;
    font-style: italic;
}

.field-validation-error {
    color: #ef4444;
    font-size: 0.8rem;
    margin-top: 0.25rem;
    display: block;
    font-weight: 500;
}

/* Calculated Values */
.calculated-values {
    background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
    border-radius: 12px;
    padding: 1.5rem;
    border: 1px solid #e5e7eb;
    margin-bottom: 1.5rem;
    position: relative;
}

.calculated-values::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    width: 4px;
    height: 100%;
    background: linear-gradient(135deg, #3b82f6, #2563eb);
    border-radius: 12px 0 0 12px;
}

.calculated-values h4 {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    margin: 0 0 1rem 0;
    color: #1e293b;
    font-size: 1.1rem;
    font-weight: 600;
}

.calculated-values h4 .material-icons {
    color: #3b82f6;
    font-size: 1.25rem;
}

.calculation-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 1rem;
}

.calculation-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.75rem;
    background: white;
    border-radius: 8px;
    border: 1px solid #e5e7eb;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    transition: all 0.2s ease;
}

.calculation-item:hover {
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    transform: translateY(-1px);
}

.calculation-label {
    font-weight: 500;
    color: #374151;
    font-size: 0.9rem;
}

.calculation-value {
    font-weight: 700;
    font-size: 1rem;
    transition: all 0.3s ease;
    border-radius: 4px;
    padding: 0.25rem 0.5rem;
}

.calculation-value.positive {
    color: #059669;
    background: #dcfce7;
}

.calculation-value.warning {
    color: #d97706;
    background: #fef3c7;
}

.calculation-value.negative {
    color: #dc2626;
    background: #fee2e2;
}

.calculation-value.normal-utilization {
    color: #059669;
    background: #dcfce7;
}

.calculation-value.high-utilization {
    color: #d97706;
    background: #fef3c7;
}

.calculation-value.over-utilized {
    color: #dc2626;
    background: #fee2e2;
}

/* Form Actions */
.form-actions {
    display: flex;
    gap: 1rem;
    flex-wrap: wrap;
    padding-top: 1.5rem;
    border-top: 1px solid #e5e7eb;
    justify-content: flex-start;
}

.form-actions .btn {
    min-width: 140px;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    gap: 0.5rem;
    padding: 0.75rem 1.5rem;
    border: none;
    border-radius: 8px;
    font-size: 0.9rem;
    font-weight: 500;
    text-decoration: none;
    cursor: pointer;
    transition: all 0.2s ease;
    line-height: 1;
}

.form-actions .btn:hover {
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.form-actions .btn-primary {
    background: linear-gradient(135deg, #3b82f6, #2563eb);
    color: white;
}

.form-actions .btn-primary:hover {
    background: linear-gradient(135deg, #2563eb, #1d4ed8);
}

.form-actions .btn-secondary {
    background: linear-gradient(135deg, #6b7280, #4b5563);
    color: white;
}

.form-actions .btn-secondary:hover {
    background: linear-gradient(135deg, #4b5563, #374151);
}

.form-actions .btn-outline-secondary {
    background: transparent;
    color: #6b7280;
    border: 2px solid #6b7280;
}

.form-actions .btn-outline-secondary:hover {
    background: #6b7280;
    color: white;
}

/* History Section */
.history-section {
    background: white;
    border-radius: 16px;
    margin-bottom: 2rem;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
    border: 1px solid #e5e7eb;
    overflow: hidden;
}

.history-grid {
    padding: 0;
    overflow-x: auto;
}

.history-section .modern-grid {
    width: 100%;
    border-collapse: collapse;
    font-size: 0.85rem;
}

.history-section .modern-grid th {
    background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
    color: #374151;
    font-weight: 600;
    padding: 1rem 0.75rem;
    text-align: left;
    border-bottom: 2px solid #e2e8f0;
    font-size: 0.8rem;
    text-transform: uppercase;
    letter-spacing: 0.025em;
    white-space: nowrap;
}

.history-section .modern-grid td {
    padding: 0.75rem;
    border-bottom: 1px solid #f1f5f9;
    vertical-align: middle;
    white-space: nowrap;
}

.history-section .modern-grid tbody tr:hover {
    background: #f8fafc;
}

/* Enhanced Alert Panels */
.alert-panel {
    padding: 1rem 1.5rem;
    border-radius: 12px;
    margin-bottom: 1.5rem;
    border: 1px solid;
    display: flex;
    align-items: center;
    gap: 0.75rem;
    font-weight: 500;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.alert-panel.success {
    background: linear-gradient(135deg, #dcfce7 0%, #bbf7d0 100%);
    color: #166534;
    border-color: #22c55e;
}

.alert-panel.error {
    background: linear-gradient(135deg, #fee2e2 0%, #fecaca 100%);
    color: #991b1b;
    border-color: #ef4444;
}

.alert-panel.warning {
    background: linear-gradient(135deg, #fef3c7 0%, #fde68a 100%);
    color: #92400e;
    border-color: #fbbf24;
}

.alert-panel.info {
    background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%);
    color: #1e40af;
    border-color: #3b82f6;
}

/* Responsive Design */
@media (max-width: 768px) {
    .balance-summary-grid {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
    
    .form-row {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
    
    .calculation-grid {
        grid-template-columns: 1fr;
    }
    
    .form-actions {
        flex-direction: column;
    }
    
    .form-actions .btn {
        min-width: auto;
        width: 100%;
    }
    
    .balance-summary-item {
        padding: 1rem;
    }
    
    .summary-number {
        font-size: 1.5rem;
    }
    
    .form-content {
        padding: 1rem;
    }
    
    .form-header {
        padding: 1rem;
    }
    
    .calculated-values {
        padding: 1rem;
    }
    
    .calculation-item {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.5rem;
        text-align: left;
    }
    
    .history-grid {
        overflow-x: scroll;
    }
}

@media (max-width: 480px) {
    .summary-icon {
        width: 40px;
        height: 40px;
        font-size: 1rem;
    }
    
    .summary-number {
        font-size: 1.25rem;
    }
    
    .form-header h3 {
        font-size: 1.25rem;
    }
    
    .form-header h3 .material-icons {
        font-size: 1.5rem;
    }
    
    .form-control {
        padding: 0.625rem;
    }
    
    .alert-panel {
        padding: 0.75rem 1rem;
        font-size: 0.9rem;
    }
}
    </style>
    <!-- Welcome Header - Matching EmployeeLeavePortal Style -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">edit</i>
                    Edit Leave Balance
                </h1>
                <p class="welcome-subtitle">Modify employee leave balance allocation and usage</p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">person</i>
                        <span>
                            <asp:Literal ID="litEmployeeName" runat="server" Text="Employee Name"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">badge</i>
                        <span>
                            <asp:Literal ID="litEmployeeNumber" runat="server" Text="EMP001"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">business</i>
                        <span>
                            <asp:Literal ID="litDepartment" runat="server" Text="Department"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">event</i>
                        <span>
                            <asp:Literal ID="litLeaveType" runat="server" Text="Leave Type"></asp:Literal>
                        </span>
                    </div>
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnBackToLeaveManagement" runat="server" Text="Back to Leave Management" 
                    CssClass="btn btn-outline-light" OnClick="btnBackToLeaveManagement_Click" />
            </div>
        </div>
    </div>

    <!-- Alert Messages -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert-panel">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Current Balance Summary -->
    <div class="time-status-card">
        <div class="status-header">
            <h3>
                <i class="material-icons">account_balance</i>
                Current Balance Summary
            </h3>
            <div class="status-indicator">
                <asp:Literal ID="litCurrentYear" runat="server"></asp:Literal>
            </div>
        </div>
        
        <div class="balance-summary-grid">
            <div class="balance-summary-item allocated">
                <div class="summary-icon">
                    <i class="material-icons">add_circle</i>
                </div>
                <div class="summary-content">
                    <div class="summary-number">
                        <asp:Literal ID="litCurrentAllocated" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="summary-label">Days Allocated</div>
                </div>
            </div>

            <div class="balance-summary-item used">
                <div class="summary-icon">
                    <i class="material-icons">remove_circle</i>
                </div>
                <div class="summary-content">
                    <div class="summary-number">
                        <asp:Literal ID="litCurrentUsed" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="summary-label">Days Used</div>
                </div>
            </div>

            <div class="balance-summary-item remaining">
                <div class="summary-icon">
                    <i class="material-icons">account_balance_wallet</i>
                </div>
                <div class="summary-content">
                    <div class="summary-number">
                        <asp:Literal ID="litCurrentRemaining" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="summary-label">Days Remaining</div>
                </div>
            </div>
        </div>
    </div>

    <!-- Edit Balance Form -->
    <div class="form-container">
        <div class="form-card">
            <div class="form-header">
                <h3>
                    <i class="material-icons">edit</i>
                    Edit Balance Details
                </h3>
                <p>Modify the employee's leave balance allocation and usage. Changes will be logged for audit purposes.</p>
            </div>

            <div class="form-content">
                <div class="form-row">
                    <div class="form-group">
                        <label for="txtAllocatedDays" class="form-label required">
                            <i class="material-icons">add_circle_outline</i>
                            Allocated Days
                        </label>
                        <asp:TextBox ID="txtAllocatedDays" runat="server" CssClass="form-control" 
                            placeholder="Enter allocated days" TextMode="Number" step="0.5" min="0" max="365"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvAllocatedDays" runat="server" 
                            ControlToValidate="txtAllocatedDays" 
                            ErrorMessage="Allocated days is required" 
                            CssClass="field-validation-error" 
                            Display="Dynamic" />
                        <asp:RangeValidator ID="rvAllocatedDays" runat="server" 
                            ControlToValidate="txtAllocatedDays" 
                            MinimumValue="0" MaximumValue="365" Type="Double"
                            ErrorMessage="Allocated days must be between 0 and 365" 
                            CssClass="field-validation-error" 
                            Display="Dynamic" />
                    </div>

                    <div class="form-group">
                        <label for="txtUsedDays" class="form-label required">
                            <i class="material-icons">remove_circle_outline</i>
                            Used Days
                        </label>
                        <asp:TextBox ID="txtUsedDays" runat="server" CssClass="form-control" 
                            placeholder="Enter used days" TextMode="Number" step="0.5" min="0"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvUsedDays" runat="server" 
                            ControlToValidate="txtUsedDays" 
                            ErrorMessage="Used days is required" 
                            CssClass="field-validation-error" 
                            Display="Dynamic" />
                        <asp:RangeValidator ID="rvUsedDays" runat="server" 
                            ControlToValidate="txtUsedDays" 
                            MinimumValue="0" MaximumValue="365" Type="Double"
                            ErrorMessage="Used days must be between 0 and 365" 
                            CssClass="field-validation-error" 
                            Display="Dynamic" />
                        <asp:CustomValidator ID="cvUsedDays" runat="server" 
                            ControlToValidate="txtUsedDays" 
                            OnServerValidate="cvUsedDays_ServerValidate"
                            ErrorMessage="Used days cannot exceed allocated days" 
                            CssClass="field-validation-error" 
                            Display="Dynamic" />
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label for="txtEffectiveDate" class="form-label required">
                            <i class="material-icons">calendar_today</i>
                            Effective Date
                        </label>
                        <asp:TextBox ID="txtEffectiveDate" runat="server" CssClass="form-control" 
                            TextMode="Date"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvEffectiveDate" runat="server" 
                            ControlToValidate="txtEffectiveDate" 
                            ErrorMessage="Effective date is required" 
                            CssClass="field-validation-error" 
                            Display="Dynamic" />
                        <small class="form-help">The date from which this balance becomes effective</small>
                    </div>

                    <div class="form-group">
                        <label for="txtExpiryDate" class="form-label">
                            <i class="material-icons">event_busy</i>
                            Expiry Date (Optional)
                        </label>
                        <asp:TextBox ID="txtExpiryDate" runat="server" CssClass="form-control" 
                            TextMode="Date"></asp:TextBox>
                        <small class="form-help">Leave this blank if the balance doesn't expire</small>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group full-width">
                        <label for="txtAdjustmentReason" class="form-label required">
                            <i class="material-icons">description</i>
                            Reason for Adjustment
                        </label>
                        <asp:TextBox ID="txtAdjustmentReason" runat="server" CssClass="form-control" 
                            TextMode="MultiLine" Rows="3" 
                            placeholder="Please provide a reason for this balance adjustment (required for audit trail)"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvAdjustmentReason" runat="server" 
                            ControlToValidate="txtAdjustmentReason" 
                            ErrorMessage="Adjustment reason is required for audit purposes" 
                            CssClass="field-validation-error" 
                            Display="Dynamic" />
                    </div>
                </div>

                <!-- Calculated Values Display -->
                <div class="calculated-values">
                    <h4>
                        <i class="material-icons">calculate</i>
                        Calculated Values
                    </h4>
                    
                    <div class="calculation-grid">
                        <div class="calculation-item">
                            <div class="calculation-label">Remaining Days:</div>
                            <div class="calculation-value" id="remainingDays">
                                <span id="spanRemainingDays">0</span> days
                            </div>
                        </div>
                        
                        <div class="calculation-item">
                            <div class="calculation-label">Utilization Rate:</div>
                            <div class="calculation-value" id="utilizationRate">
                                <span id="spanUtilizationRate">0</span>%
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Action Buttons -->
                <div class="form-actions">
                    <asp:Button ID="btnSaveChanges" runat="server" Text="Save Changes" 
                        CssClass="btn btn-primary" OnClick="btnSaveChanges_Click" />
                    
                    <asp:Button ID="btnResetToDefault" runat="server" Text="Reset to Default" 
                        CssClass="btn btn-secondary" OnClick="btnResetToDefault_Click" 
                        OnClientClick="return confirm('Are you sure you want to reset this balance to the default allocation for this leave type?');" />
                    
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                        CssClass="btn btn-outline-secondary" OnClick="btnCancel_Click" 
                        CausesValidation="false" />
                </div>
            </div>
        </div>
    </div>

    <!-- Balance History Section -->
    <div class="history-section">
        <div class="section-header">
            <h3 class="section-title">
                <i class="material-icons">history</i>
                Balance History
            </h3>
        </div>

        <div class="history-grid">
            <asp:GridView ID="gvBalanceHistory" runat="server" 
                CssClass="modern-grid" 
                AutoGenerateColumns="false" 
                EmptyDataText="No balance history found for this employee and leave type.">
                
                <Columns>
                    <asp:BoundField DataField="ChangeDate" HeaderText="Date" DataFormatString="{0:MMM dd, yyyy}" />
                    <asp:BoundField DataField="ChangeType" HeaderText="Change Type" />
                    <asp:BoundField DataField="PreviousAllocated" HeaderText="Previous Allocated" DataFormatString="{0:N1}" />
                    <asp:BoundField DataField="NewAllocated" HeaderText="New Allocated" DataFormatString="{0:N1}" />
                    <asp:BoundField DataField="PreviousUsed" HeaderText="Previous Used" DataFormatString="{0:N1}" />
                    <asp:BoundField DataField="NewUsed" HeaderText="New Used" DataFormatString="{0:N1}" />
                    <asp:BoundField DataField="Reason" HeaderText="Reason" />
                    <asp:BoundField DataField="ChangedBy" HeaderText="Changed By" />
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Hidden fields -->
    <asp:HiddenField ID="hfEmployeeId" runat="server" />
    <asp:HiddenField ID="hfLeaveType" runat="server" />
    <asp:HiddenField ID="hfOriginalAllocated" runat="server" />
    <asp:HiddenField ID="hfOriginalUsed" runat="server" />

    <!-- JavaScript for real-time calculations -->
    <script type="text/javascript">
        function calculateValues() {
            var allocated = parseFloat(document.getElementById('<%= txtAllocatedDays.ClientID %>').value) || 0;
            var used = parseFloat(document.getElementById('<%= txtUsedDays.ClientID %>').value) || 0;
            
            var remaining = allocated - used;
            var utilization = allocated > 0 ? Math.round((used / allocated) * 100) : 0;
            
            document.getElementById('spanRemainingDays').textContent = remaining.toFixed(1);
            document.getElementById('spanUtilizationRate').textContent = utilization;
            
            // Update styling based on values
            var remainingElement = document.getElementById('remainingDays');
            var utilizationElement = document.getElementById('utilizationRate');
            
            // Color code remaining days
            remainingElement.className = 'calculation-value';
            if (remaining < 0) {
                remainingElement.classList.add('negative');
            } else if (remaining < 2) {
                remainingElement.classList.add('warning');
            } else {
                remainingElement.classList.add('positive');
            }
            
            // Color code utilization rate
            utilizationElement.className = 'calculation-value';
            if (utilization > 100) {
                utilizationElement.classList.add('over-utilized');
            } else if (utilization > 80) {
                utilizationElement.classList.add('high-utilization');
            } else {
                utilizationElement.classList.add('normal-utilization');
            }
        }
        
        // Attach event listeners when page loads
        document.addEventListener('DOMContentLoaded', function() {
            document.getElementById('<%= txtAllocatedDays.ClientID %>').addEventListener('input', calculateValues);
            document.getElementById('<%= txtUsedDays.ClientID %>').addEventListener('input', calculateValues);
            
            // Calculate initial values
            calculateValues();
        });
    </script>

</asp:Content>