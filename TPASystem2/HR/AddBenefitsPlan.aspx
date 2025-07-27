<%@ Page Title="Add Benefits Plan" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="AddBenefitsPlan.aspx.cs" Inherits="TPASystem2.HR.AddBenefitsPlan" %>

<asp:Content ID="Content2" ContentPlaceHolderID="DashboardContent" runat="server">
    
    <!-- CSS Links -->
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    <link href="Content/css/tpa-dashboard.css" rel="stylesheet">
    
    <div class="benefits-container">
        <!-- Page Header -->
        <div class="page-header">
            <div class="header-content">
                <h1><i class="material-icons">add_circle</i>Add Benefits Plan</h1>
                <p>Create a new benefits plan for full-time employees</p>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                    CssClass="btn btn-secondary" OnClick="btnCancel_Click" CausesValidation="false" />
            </div>
        </div>

        <!-- Alert Messages -->
        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Benefits Plan Form -->
        <div class="form-container">
            <div class="form-header">
                <h3>Benefits Plan Information</h3>
                <p>Enter the details for the new benefits plan available to full-time employees</p>
            </div>
            
            <div class="form-content">
                <div class="form-grid">
                    <!-- Basic Information -->
                    <div class="form-section">
                        <h4>Basic Information</h4>
                        
                        <div class="form-group">
                            <label for="txtPlanName">Plan Name <span class="required">*</span></label>
                            <asp:TextBox ID="txtPlanName" runat="server" CssClass="form-control" 
                                placeholder="Enter plan name..." MaxLength="200"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvPlanName" runat="server" 
                                ControlToValidate="txtPlanName" ErrorMessage="Plan name is required" 
                                CssClass="field-validation-error" Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="ddlPlanType">Plan Type <span class="required">*</span></label>
                                <asp:DropDownList ID="ddlPlanType" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="" Text="Select Plan Type"></asp:ListItem>
                                    <asp:ListItem Value="HEALTH" Text="Health"></asp:ListItem>
                                    <asp:ListItem Value="DENTAL" Text="Dental"></asp:ListItem>
                                    <asp:ListItem Value="VISION" Text="Vision"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvPlanType" runat="server" 
                                    ControlToValidate="ddlPlanType" ErrorMessage="Plan type is required" 
                                    CssClass="field-validation-error" Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>

                            <div class="form-group">
                                <label for="ddlPlanCategory">Plan Category <span class="required">*</span></label>
                                <asp:DropDownList ID="ddlPlanCategory" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="" Text="Select Category"></asp:ListItem>
                                    <asp:ListItem Value="BASIC" Text="Basic"></asp:ListItem>
                                    <asp:ListItem Value="PREMIUM" Text="Premium"></asp:ListItem>
                                    <asp:ListItem Value="FAMILY" Text="Family"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvPlanCategory" runat="server" 
                                    ControlToValidate="ddlPlanCategory" ErrorMessage="Plan category is required" 
                                    CssClass="field-validation-error" Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>
                        </div>

                        <div class="form-group">
                            <label for="txtDescription">Description</label>
                            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" 
                                TextMode="MultiLine" Rows="4" 
                                placeholder="Enter plan description..."></asp:TextBox>
                        </div>
                    </div>

                    <!-- Cost Information -->
                    <div class="form-section">
                        <h4>Cost Information</h4>
                        
                        <div class="form-row">
                            <div class="form-group">
                                <label for="txtMonthlyEmployeeCost">Monthly Employee Cost <span class="required">*</span></label>
                                <div class="input-group">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox ID="txtMonthlyEmployeeCost" runat="server" CssClass="form-control" 
                                        placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                                </div>
                                <asp:RequiredFieldValidator ID="rfvEmployeeCost" runat="server" 
                                    ControlToValidate="txtMonthlyEmployeeCost" ErrorMessage="Employee cost is required" 
                                    CssClass="field-validation-error" Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:RangeValidator ID="rvEmployeeCost" runat="server" 
                                    ControlToValidate="txtMonthlyEmployeeCost" Type="Double" MinimumValue="0" MaximumValue="9999"
                                    ErrorMessage="Employee cost must be between $0 and $9,999" 
                                    CssClass="field-validation-error" Display="Dynamic"></asp:RangeValidator>
                            </div>

                            <div class="form-group">
                                <label for="txtMonthlyEmployerCost">Monthly Employer Cost <span class="required">*</span></label>
                                <div class="input-group">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox ID="txtMonthlyEmployerCost" runat="server" CssClass="form-control" 
                                        placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                                </div>
                                <asp:RequiredFieldValidator ID="rfvEmployerCost" runat="server" 
                                    ControlToValidate="txtMonthlyEmployerCost" ErrorMessage="Employer cost is required" 
                                    CssClass="field-validation-error" Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:RangeValidator ID="rvEmployerCost" runat="server" 
                                    ControlToValidate="txtMonthlyEmployerCost" Type="Double" MinimumValue="0" MaximumValue="9999"
                                    ErrorMessage="Employer cost must be between $0 and $9,999" 
                                    CssClass="field-validation-error" Display="Dynamic"></asp:RangeValidator>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="txtAnnualDeductible">Annual Deductible</label>
                                <div class="input-group">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox ID="txtAnnualDeductible" runat="server" CssClass="form-control" 
                                        placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtOutOfPocketMax">Out-of-Pocket Maximum</label>
                                <div class="input-group">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox ID="txtOutOfPocketMax" runat="server" CssClass="form-control" 
                                        placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Coverage Details (for Health plans) -->
                    <div class="form-section" id="healthCoverageSection" style="display: none;">
                        <h4>Health Coverage Details</h4>
                        
                        <div class="form-row">
                            <div class="form-group">
                                <label for="txtCoPayOfficeVisit">Office Visit Co-Pay</label>
                                <div class="input-group">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox ID="txtCoPayOfficeVisit" runat="server" CssClass="form-control" 
                                        placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtCoPaySpecialist">Specialist Co-Pay</label>
                                <div class="input-group">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox ID="txtCoPaySpecialist" runat="server" CssClass="form-control" 
                                        placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                                </div>
                            </div>

                            <div class="form-group">
                                <label for="txtCoPayEmergency">Emergency Co-Pay</label>
                                <div class="input-group">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox ID="txtCoPayEmergency" runat="server" CssClass="form-control" 
                                        placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Additional Coverage Details -->
                    <div class="form-section">
                        <h4>Additional Details</h4>
                        
                        <div class="form-group">
                            <label for="txtCoverageDetails">Coverage Details</label>
                            <asp:TextBox ID="txtCoverageDetails" runat="server" CssClass="form-control" 
                                TextMode="MultiLine" Rows="4" 
                                placeholder="Enter detailed coverage information..."></asp:TextBox>
                        </div>

                        <div class="form-row">
                            <div class="form-group">
                                <label for="txtEffectiveDate">Effective Date <span class="required">*</span></label>
                                <asp:TextBox ID="txtEffectiveDate" runat="server" CssClass="form-control" 
                                    TextMode="Date"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEffectiveDate" runat="server" 
                                    ControlToValidate="txtEffectiveDate" ErrorMessage="Effective date is required" 
                                    CssClass="field-validation-error" Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>

                            <div class="form-group">
                                <label for="txtEndDate">End Date (Optional)</label>
                                <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" 
                                    TextMode="Date"></asp:TextBox>
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="checkbox-group">
                                <asp:CheckBox ID="chkIsActive" runat="server" Checked="true" />
                                <label for="chkIsActive">Active Plan (Available for enrollment)</label>
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="checkbox-group">
                                <asp:CheckBox ID="chkAutoEnroll" runat="server" Checked="true" />
                                <label for="chkAutoEnroll">Auto-enroll eligible full-time employees in Basic plans</label>
                            </div>
                            <small class="form-text">If checked and this is a Basic plan, all eligible full-time employees will be automatically enrolled.</small>
                        </div>
                    </div>
                </div>

                <!-- Form Actions -->
                <div class="form-actions">
                    <asp:Button ID="btnSave" runat="server" Text="Save Benefits Plan" 
                        CssClass="btn btn-primary" OnClick="btnSave_Click" />
                    <asp:Button ID="btnSaveAndAdd" runat="server" Text="Save & Add Another" 
                        CssClass="btn btn-secondary" OnClick="btnSaveAndAdd_Click" />
                    <asp:Button ID="btnCancelForm" runat="server" Text="Cancel" 
                        CssClass="btn btn-outline-secondary" OnClick="btnCancel_Click" 
                        CausesValidation="false" />
                </div>
            </div>
        </div>
    </div>

    <script>
        // Show/hide health coverage section based on plan type
        document.addEventListener('DOMContentLoaded', function () {
            var planTypeDropdown = document.getElementById('<%= ddlPlanType.ClientID %>');
            var healthSection = document.getElementById('healthCoverageSection');
            
            function toggleHealthSection() {
                if (planTypeDropdown.value === 'HEALTH') {
                    healthSection.style.display = 'block';
                } else {
                    healthSection.style.display = 'none';
                }
            }
            
            planTypeDropdown.addEventListener('change', toggleHealthSection);
            toggleHealthSection(); // Initial check
        });
        
        // Set default effective date to today
        document.addEventListener('DOMContentLoaded', function() {
            var effectiveDateInput = document.getElementById('<%= txtEffectiveDate.ClientID %>');
            if (!effectiveDateInput.value) {
                var today = new Date();
                var formattedDate = today.toISOString().split('T')[0];
                effectiveDateInput.value = formattedDate;
            }
        });
    </script>
</asp:Content>