<%@ Page Title="Add New Employee" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="AddEmployee.aspx.cs" Inherits="TPASystem2.HR.AddEmployee" %>

<asp:Content ID="AddEmployeeContent" ContentPlaceHolderID="DashboardContent" runat="server">
    
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/employee-management.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    
    <!-- Page Header -->
    <div class="page-header">
        <div class="page-header-content">
            <h1 class="page-title">Add New Employee</h1>
            <p class="page-subtitle">Create a new employee record and set up onboarding workflow</p>
        </div>
        <div class="page-header-actions">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                        CssClass="btn btn-outline" OnClick="btnCancel_Click" CausesValidation="false" />
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
                                     placeholder="(555) 123-4567" MaxLength="20" TextMode="Phone"></asp:TextBox>
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

            <!-- Address Information Section -->
            <div class="form-section">
                <div class="section-header">
                    <h3><i class="material-icons">home</i> Address Information</h3>
                    <p>Employee residential address</p>
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
                                     placeholder="City name" MaxLength="100"></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= txtState.ClientID %>">State</label>
                        <asp:TextBox ID="txtState" runat="server" CssClass="form-control" 
                                     placeholder="State/Province" MaxLength="50"></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= txtZipCode.ClientID %>">ZIP Code</label>
                        <asp:TextBox ID="txtZipCode" runat="server" CssClass="form-control" 
                                     placeholder="12345" MaxLength="10"></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- Employment Information Section -->
            <div class="form-section">
                <div class="section-header">
                    <h3><i class="material-icons">work</i> Employment Information</h3>
                    <p>Job details and organizational structure</p>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= txtPosition.ClientID %>">Position/Job Title <span class="required">*</span></label>
                        <asp:TextBox ID="txtPosition" runat="server" CssClass="form-control" 
                                     placeholder="e.g., Software Developer, HR Specialist" MaxLength="100" required></asp:TextBox>
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
                            <asp:ListItem Value="Full-time" Selected="True">Full-time</asp:ListItem>
                            <asp:ListItem Value="Part-time">Part-time</asp:ListItem>
                            <asp:ListItem Value="Contract">Contract</asp:ListItem>
                            <asp:ListItem Value="Temporary">Temporary</asp:ListItem>
                            <asp:ListItem Value="Intern">Intern</asp:ListItem>
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
                                     placeholder="50000.00" TextMode="Number" step="0.01"></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= ddlStatus.ClientID %>">Employment Status</label>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                            <asp:ListItem Value="Active" Selected="True">Active</asp:ListItem>
                            <asp:ListItem Value="Inactive">Inactive</asp:ListItem>
                            <asp:ListItem Value="On Leave">On Leave</asp:ListItem>
                            <asp:ListItem Value="Terminated">Terminated</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <!-- System Access Section -->
            <div class="form-section">
                <div class="section-header">
                    <h3><i class="material-icons">security</i> System Access</h3>
                    <p>User account and system access configuration</p>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= txtTemporaryPassword.ClientID %>">Temporary Password</label>
                        <asp:TextBox ID="txtTemporaryPassword" runat="server" CssClass="form-control" 
                                     placeholder="Leave blank for default (TempPass123!)" MaxLength="50"></asp:TextBox>
                        <small class="form-text">If left blank, default password "TempPass123!" will be used</small>
                    </div>
                    
                    <div class="form-group checkbox-group">
                        <asp:CheckBox ID="chkMustChangePassword" runat="server" CssClass="form-check-input" Checked="true" />
                        <label for="<%= chkMustChangePassword.ClientID %>" class="form-check-label">
                            Must change password on first login
                        </label>
                    </div>
                </div>
            </div>

            <!-- Onboarding Preview Section -->
            <div class="form-section">
                <div class="section-header">
                    <h3><i class="material-icons">assignment</i> Onboarding Workflow</h3>
                    <p>Automatic onboarding tasks will be created based on department</p>
                </div>
                
                <div class="onboarding-preview">
                    <div class="preview-card">
                        <div class="preview-icon">
                            <i class="material-icons">checklist</i>
                        </div>
                        <div class="preview-content">
                            <h4>Automated Task Assignment</h4>
                            <p>Upon creation, this employee will automatically be assigned department-specific onboarding tasks. This includes:</p>
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

    <!-- JavaScript for form functionality -->
    <script>
        document.addEventListener('DOMContentLoaded', function () {
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
                showFieldError(field, 'This field is required');
                return false;
            }

            // Email validation
            if (field.type === 'email' && value) {
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailRegex.test(value)) {
                    showFieldError(field, 'Please enter a valid email address');
                    return false;
                }
            }

            // Salary validation
            if (field.getAttribute('id') && field.getAttribute('id').includes('Salary') && value) {
                const salaryValue = parseFloat(value);
                if (salaryValue < 0) {
                    showFieldError(field, 'Salary must be a positive number');
                    return false;
                }
            }

            clearFieldError(field);
            return true;
        }

        function showFieldError(field, message) {
            clearFieldError(field);

            const errorDiv = document.createElement('div');
            errorDiv.className = 'field-error';
            errorDiv.textContent = message;
            errorDiv.style.color = '#dc3545';
            errorDiv.style.fontSize = '0.8rem';
            errorDiv.style.marginTop = '0.25rem';

            field.style.borderColor = '#dc3545';
            field.parentNode.appendChild(errorDiv);
        }

        function clearFieldError(field) {
            const existingError = field.parentNode.querySelector('.field-error');
            if (existingError) {
                existingError.remove();
            }
            field.style.borderColor = '';
        }

        function setDefaultHireDate() {
            const hireDateField = document.querySelector('input[type="date"]');
            if (hireDateField && !hireDateField.value) {
                const today = new Date().toISOString().split('T')[0];
                hireDateField.value = today;
            }
        }

        function initializeDepartmentFeatures() {
            const departmentSelect = document.querySelector('select[id*="Department"]');
            if (departmentSelect) {
                departmentSelect.addEventListener('change', function () {
                    updateOnboardingPreview(this.value);
                });
            }
        }

        function updateOnboardingPreview(departmentId) {
            // This could be enhanced to show department-specific onboarding tasks
            // For now, just update the preview text
            console.log('Department selected:', departmentId);
        }

        // Format phone number as user types
        function formatPhoneNumber(input) {
            let value = input.value.replace(/\D/g, '');
            if (value.length >= 6) {
                value = value.replace(/(\d{3})(\d{3})(\d{4})/, '($1) $2-$3');
            } else if (value.length >= 3) {
                value = value.replace(/(\d{3})(\d{0,3})/, '($1) $2');
            }
            input.value = value;
        }

        // Add phone formatting to phone field
        document.addEventListener('DOMContentLoaded', function () {
            const phoneField = document.querySelector('input[type="tel"], input[id*="Phone"]');
            if (phoneField) {
                phoneField.addEventListener('input', function () {
                    formatPhoneNumber(this);
                });
            }
        });
    </script>
</asp:Content>