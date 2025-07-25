<%@ Page Title="Add New Employee" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="AddEmployee.aspx.cs" Inherits="TPASystem2.HR.AddEmployee" %>

<asp:Content ID="AddEmployeeContent" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- Page Header -->
    <div class="page-header">
        <div class="page-header-content">
            <h1 class="page-title">Add New Employee</h1>
            <p class="page-subtitle">Create a new employee record and set up onboarding workflow</p>
        </div>
        <div class="page-header-actions">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                        CssClass="btn btn-outline" OnClick="btnCancel_Click" />
            <asp:Button ID="btnSaveEmployee" runat="server" Text="Create Employee" 
                        CssClass="btn btn-primary" OnClick="btnSaveEmployee_Click" />
        </div>
    </div>

    <!-- Form Container -->
    <div class="form-container">
        <div class="form-grid">
            <!-- Personal Information Section -->
            <div class="form-section">
                <div class="section-header">
                    <h3><i class="material-icons">person</i> Personal Information</h3>
                    <p>Basic employee details and contact information</p>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= txtFirstName.ClientID %>">First Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" 
                                     placeholder="Enter first name" MaxLength="100" required></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" 
                                                   ControlToValidate="txtFirstName" 
                                                   ErrorMessage="First name is required" 
                                                   CssClass="error-message" Display="Dynamic" />
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= txtLastName.ClientID %>">Last Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" 
                                     placeholder="Enter last name" MaxLength="100" required></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvLastName" runat="server" 
                                                   ControlToValidate="txtLastName" 
                                                   ErrorMessage="Last name is required" 
                                                   CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= txtEmail.ClientID %>">Email Address <span class="required">*</span></label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" 
                                     TextMode="Email" placeholder="employee@company.com" MaxLength="255" required></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                                                   ControlToValidate="txtEmail" 
                                                   ErrorMessage="Email is required" 
                                                   CssClass="error-message" Display="Dynamic" />
                        <asp:RegularExpressionValidator ID="revEmail" runat="server" 
                                                       ControlToValidate="txtEmail"
                                                       ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
                                                       ErrorMessage="Please enter a valid email address" 
                                                       CssClass="error-message" Display="Dynamic" />
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= txtPhoneNumber.ClientID %>">Phone Number</label>
                        <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control" 
                                     placeholder="(555) 123-4567" MaxLength="20"></asp:TextBox>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= txtDateOfBirth.ClientID %>">Date of Birth</label>
                        <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="form-control" 
                                     TextMode="Date"></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= ddlGender.ClientID %>">Gender</label>
                        <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">Select Gender</asp:ListItem>
                            <asp:ListItem Value="Male">Male</asp:ListItem>
                            <asp:ListItem Value="Female">Female</asp:ListItem>
                            <asp:ListItem Value="Other">Other</asp:ListItem>
                            <asp:ListItem Value="Prefer not to say">Prefer not to say</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <!-- Employment Information Section -->
            <div class="form-section">
                <div class="section-header">
                    <h3><i class="material-icons">work</i> Employment Information</h3>
                    <p>Job title, department, and employment details</p>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= txtPosition.ClientID %>">Position/Job Title <span class="required">*</span></label>
                        <asp:TextBox ID="txtPosition" runat="server" CssClass="form-control" 
                                     placeholder="e.g., Software Engineer, HR Manager" MaxLength="100" required></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPosition" runat="server" 
                                                   ControlToValidate="txtPosition" 
                                                   ErrorMessage="Position is required" 
                                                   CssClass="error-message" Display="Dynamic" />
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= ddlDepartment.ClientID %>">Department <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-control" required>
                            <asp:ListItem Value="">Select Department</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvDepartment" runat="server" 
                                                   ControlToValidate="ddlDepartment" 
                                                   ErrorMessage="Department is required" 
                                                   CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= ddlEmployeeType.ClientID %>">Employee Type <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlEmployeeType" runat="server" CssClass="form-control" required>
                            <asp:ListItem Value="">Select Type</asp:ListItem>
                            <asp:ListItem Value="Full-time">Full-time</asp:ListItem>
                            <asp:ListItem Value="Part-time">Part-time</asp:ListItem>
                            <asp:ListItem Value="Contract">Contract</asp:ListItem>
                            <asp:ListItem Value="Intern">Intern</asp:ListItem>
                            <asp:ListItem Value="Temporary">Temporary</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvEmployeeType" runat="server" 
                                                   ControlToValidate="ddlEmployeeType" 
                                                   ErrorMessage="Employee type is required" 
                                                   CssClass="error-message" Display="Dynamic" />
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= txtHireDate.ClientID %>">Hire Date <span class="required">*</span></label>
                        <asp:TextBox ID="txtHireDate" runat="server" CssClass="form-control" 
                                     TextMode="Date" required></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvHireDate" runat="server" 
                                                   ControlToValidate="txtHireDate" 
                                                   ErrorMessage="Hire date is required" 
                                                   CssClass="error-message" Display="Dynamic" />
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= ddlManager.ClientID %>">Manager</label>
                        <asp:DropDownList ID="ddlManager" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">Select Manager</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= txtWorkLocation.ClientID %>">Work Location</label>
                        <asp:TextBox ID="txtWorkLocation" runat="server" CssClass="form-control" 
                                     placeholder="e.g., Office, Remote, Hybrid" MaxLength="100"></asp:TextBox>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= txtSalary.ClientID %>">Annual Salary</label>
                        <asp:TextBox ID="txtSalary" runat="server" CssClass="form-control" 
                                     placeholder="0.00" TextMode="Number" step="0.01"></asp:TextBox>
                        <small class="form-text">Optional - can be added later</small>
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= ddlStatus.ClientID %>">Initial Status</label>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                            <asp:ListItem Value="Active" Selected="True">Active</asp:ListItem>
                            <asp:ListItem Value="Inactive">Inactive</asp:ListItem>
                            <asp:ListItem Value="On Leave">On Leave</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <!-- Address Information Section -->
            <div class="form-section">
                <div class="section-header">
                    <h3><i class="material-icons">location_on</i> Address Information</h3>
                    <p>Home address and contact details</p>
                </div>
                
                <div class="form-row">
                    <div class="form-group full-width">
                        <label for="<%= txtAddress.ClientID %>">Street Address</label>
                        <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" 
                                     placeholder="123 Main Street" MaxLength="255"></asp:TextBox>
                    </div>
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= txtCity.ClientID %>">City</label>
                        <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" 
                                     placeholder="City" MaxLength="100"></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= txtState.ClientID %>">State</label>
                        <asp:TextBox ID="txtState" runat="server" CssClass="form-control" 
                                     placeholder="State" MaxLength="50"></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= txtZipCode.ClientID %>">ZIP Code</label>
                        <asp:TextBox ID="txtZipCode" runat="server" CssClass="form-control" 
                                     placeholder="12345" MaxLength="10"></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- System Access Section -->
            <div class="form-section">
                <div class="section-header">
                    <h3><i class="material-icons">security</i> System Access</h3>
                    <p>Login credentials and system permissions</p>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= txtTemporaryPassword.ClientID %>">Temporary Password</label>
                        <asp:TextBox ID="txtTemporaryPassword" runat="server" CssClass="form-control" 
                                     placeholder="Leave blank for default" MaxLength="255"></asp:TextBox>
                        <small class="form-text">If blank, will use default: TempPass123!</small>
                    </div>
                    
                    <div class="form-group">
                        <div class="checkbox-group">
                            <asp:CheckBox ID="chkMustChangePassword" runat="server" Checked="true" />
                            <label for="<%= chkMustChangePassword.ClientID %>">Require password change on first login</label>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Onboarding Settings Section -->
            <div class="form-section">
                <div class="section-header">
                    <h3><i class="material-icons">assignment</i> Onboarding Settings</h3>
                    <p>Automatically assign onboarding template based on department</p>
                </div>
                
                <div class="onboarding-info">
                    <div class="info-card">
                        <i class="material-icons">info</i>
                        <div class="info-content">
                            <h4>Automatic Onboarding</h4>
                            <p>The appropriate onboarding template will be automatically assigned based on the selected department. This includes:</p>
                            <ul>
                                <li>Department-specific orientation tasks</li>
                                <li>Required documentation and forms</li>
                                <li>Equipment setup and system access</li>
                                <li>Training modules and meetings</li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Form Actions -->
        <div class="form-actions">
            <asp:Button ID="btnCancelBottom" runat="server" Text="Cancel" 
                        CssClass="btn btn-outline" OnClick="btnCancel_Click" CausesValidation="false" />
            <asp:Button ID="btnSaveAndNew" runat="server" Text="Save & Add Another" 
                        CssClass="btn btn-secondary" OnClick="btnSaveAndNew_Click" />
            <asp:Button ID="btnSaveEmployeeBottom" runat="server" Text="Create Employee" 
                        CssClass="btn btn-primary" OnClick="btnSaveEmployee_Click" />
        </div>
    </div>

    <!-- Success/Error Messages -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>
</asp:Content>

<asp:Content ID="AddEmployeeScripts" ContentPlaceHolderID="ScriptContent" runat="server">
    <script>
        document.addEventListener('DOMContentLoaded', function() {
            // Initialize form validation
            initializeFormValidation();
            
            // Set default hire date to today
            setDefaultHireDate();
            
            // Initialize department-based features
            initializeDepartmentFeatures();
        });

        function initializeFormValidation() {
            const form = document.querySelector('form');
            if (form) {
                // Add real-time validation
                const requiredFields = form.querySelectorAll('input[required], select[required]');
                requiredFields.forEach(field => {
                    field.addEventListener('blur', validateField);
                    field.addEventListener('input', clearFieldError);
                });
            }
        }

        function validateField(event) {
            const field = event.target;
            const value = field.value.trim();
            
            if (field.hasAttribute('required') && !value) {
                field.classList.add('error');
                showFieldError(field, `${getFieldLabel(field)} is required`);
            } else if (field.type === 'email' && value && !isValidEmail(value)) {
                field.classList.add('error');
                showFieldError(field, 'Please enter a valid email address');
            } else {
                field.classList.remove('error');
                clearFieldError(field);
            }
        }

        function clearFieldError(event) {
            const field = event.target;
            field.classList.remove('error');
            const errorElement = field.parentNode.querySelector('.field-error');
            if (errorElement) {
                errorElement.remove();
            }
        }

        function showFieldError(field, message) {
            clearFieldError({ target: field });
            const errorElement = document.createElement('span');
            errorElement.className = 'field-error';
            errorElement.textContent = message;
            field.parentNode.appendChild(errorElement);
        }

        function getFieldLabel(field) {
            const label = field.parentNode.querySelector('label');
            return label ? label.textContent.replace('*', '').trim() : 'Field';
        }

        function isValidEmail(email) {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return emailRegex.test(email);
        }

        function setDefaultHireDate() {
            const hireDateField = document.getElementById('<%= txtHireDate.ClientID %>');
            if (hireDateField && !hireDateField.value) {
                const today = new Date().toISOString().split('T')[0];
                hireDateField.value = today;
            }
        }

        function initializeDepartmentFeatures() {
            const departmentSelect = document.getElementById('<%= ddlDepartment.ClientID %>');
            if (departmentSelect) {
                departmentSelect.addEventListener('change', function() {
                    updateOnboardingInfo(this.value);
                });
            }
        }

        function updateOnboardingInfo(departmentId) {
            // This could be enhanced to show department-specific onboarding information
            console.log('Department selected:', departmentId);
        }

        // Form submission handling
        function validateForm() {
            const requiredFields = document.querySelectorAll('input[required], select[required]');
            let isValid = true;
            
            requiredFields.forEach(field => {
                if (!field.value.trim()) {
                    field.classList.add('error');
                    showFieldError(field, `${getFieldLabel(field)} is required`);
                    isValid = false;
                }
            });
            
            return isValid;
        }

        // Enhance save buttons to show loading state
        document.querySelectorAll('.btn-primary, .btn-secondary').forEach(button => {
            button.addEventListener('click', function() {
                if (validateForm()) {
                    this.disabled = true;
                    this.innerHTML = '<i class="material-icons spin">hourglass_empty</i> Saving...';
                }
            });
        });
    </script>
</asp:Content>