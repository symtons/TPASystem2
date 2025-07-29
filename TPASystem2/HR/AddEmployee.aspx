<%@ Page Title="Add New Employee" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="AddEmployee.aspx.cs" Inherits="TPASystem2.HR.AddEmployee" %>

<asp:Content ID="AddEmployeeContent" ContentPlaceHolderID="DashboardContent" runat="server">
    
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/employee-management.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    
    <!-- Message Panel -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>
    
    <!-- Page Header -->
    <div class="page-header">
        <div class="page-header-content">
            <h1 class="page-title">Add New Employee</h1>
            <p class="page-subtitle">Create a new employee record and set up onboarding workflow</p>
        </div>
        <div class="page-header-actions">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                        CssClass="btn btn-outline" OnClick="btnCancel_Click" CausesValidation="false" />
            <asp:Button ID="btnSaveAndNew" runat="server" Text="Save & Add Another" 
                        CssClass="btn btn-secondary" OnClick="btnSaveAndNew_Click" />
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
                                     placeholder="Enter email address" TextMode="Email" MaxLength="255" required></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" 
                                                   ControlToValidate="txtEmail" 
                                                   ErrorMessage="Email is required" 
                                                   CssClass="error-message" Display="Dynamic" />
                        <asp:RegularExpressionValidator ID="revEmail" runat="server" 
                                                       ControlToValidate="txtEmail" 
                                                       ErrorMessage="Please enter a valid email address" 
                                                       ValidationExpression="^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$" 
                                                       CssClass="error-message" Display="Dynamic" />
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= txtPhoneNumber.ClientID %>">Phone Number</label>
                        <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-control" 
                                     placeholder="Enter phone number" MaxLength="20"></asp:TextBox>
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
                            <asp:ListItem Text="Select Gender" Value=""></asp:ListItem>
                            <asp:ListItem Text="Male" Value="Male"></asp:ListItem>
                            <asp:ListItem Text="Female" Value="Female"></asp:ListItem>
                            <asp:ListItem Text="Other" Value="Other"></asp:ListItem>
                            <asp:ListItem Text="Prefer not to say" Value="NotSpecified"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <!-- Address Information Section -->
            <div class="form-section">
                <div class="section-header">
                    <h3><i class="material-icons">location_on</i> Address Information</h3>
                    <p>Employee residential address details</p>
                </div>
                
                <div class="form-group">
                    <label for="<%= txtAddress.ClientID %>">Street Address</label>
                    <asp:TextBox ID="txtAddress" runat="server" CssClass="form-control" 
                                 placeholder="Enter street address" MaxLength="255"></asp:TextBox>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= txtCity.ClientID %>">City</label>
                        <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" 
                                     placeholder="Enter city" MaxLength="100"></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= txtState.ClientID %>">State</label>
                        <asp:TextBox ID="txtState" runat="server" CssClass="form-control" 
                                     placeholder="Enter state" MaxLength="50"></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= txtZipCode.ClientID %>">ZIP Code</label>
                        <asp:TextBox ID="txtZipCode" runat="server" CssClass="form-control" 
                                     placeholder="Enter ZIP code" MaxLength="10"></asp:TextBox>
                    </div>
                </div>
            </div>

            <!-- Employment Information Section -->
            <div class="form-section">
                <div class="section-header">
                    <h3><i class="material-icons">work</i> Employment Information</h3>
                    <p>Job position, department, and employment details</p>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= txtPosition.ClientID %>">Position/Job Title <span class="required">*</span></label>
                        <asp:TextBox ID="txtPosition" runat="server" CssClass="form-control" 
                                     placeholder="Enter job position" MaxLength="100" required></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvPosition" runat="server" 
                                                   ControlToValidate="txtPosition" 
                                                   ErrorMessage="Position is required" 
                                                   CssClass="error-message" Display="Dynamic" />
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= ddlDepartment.ClientID %>">Department <span class="required">*</span></label>
                        <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-control" required>
                            <asp:ListItem Text="Select Department" Value=""></asp:ListItem>
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
                            <asp:ListItem Text="Full-time" Value="Full-time"></asp:ListItem>
                            <asp:ListItem Text="Part-time" Value="Part-time"></asp:ListItem>
                            <asp:ListItem Text="Contract" Value="Contract"></asp:ListItem>
                            <asp:ListItem Text="Temporary" Value="Temporary"></asp:ListItem>
                            <asp:ListItem Text="Intern" Value="Intern"></asp:ListItem>
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
                            <asp:ListItem Text="Select Manager" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= txtWorkLocation.ClientID %>">Work Location</label>
                        <asp:TextBox ID="txtWorkLocation" runat="server" CssClass="form-control" 
                                     placeholder="Enter work location" MaxLength="100"></asp:TextBox>
                    </div>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= txtSalary.ClientID %>">Salary</label>
                        <asp:TextBox ID="txtSalary" runat="server" CssClass="form-control" 
                                     placeholder="Enter salary amount" TextMode="Number" step="0.01"></asp:TextBox>
                    </div>
                    
                    <div class="form-group">
                        <label for="<%= ddlStatus.ClientID %>">Status</label>
                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Active" Value="Active"></asp:ListItem>
                            <asp:ListItem Text="Inactive" Value="Inactive"></asp:ListItem>
                            <asp:ListItem Text="Pending" Value="Pending"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>

            <!-- Account Setup Section -->
            <div class="form-section">
                <div class="section-header">
                    <h3><i class="material-icons">security</i> Account Setup</h3>
                    <p>Login credentials and account security settings</p>
                </div>
                
                <div class="form-row">
                    <div class="form-group">
                        <label for="<%= txtTemporaryPassword.ClientID %>">Temporary Password</label>
                        <asp:TextBox ID="txtTemporaryPassword" runat="server" CssClass="form-control" 
                                     placeholder="Leave blank for default password" MaxLength="255"></asp:TextBox>
                        <small class="form-text">If left blank, default password "TempPass123!" will be used</small>
                    </div>
                    
                    <div class="form-group">
                        <div class="checkbox-group">
                            <asp:CheckBox ID="chkMustChangePassword" runat="server" />
                            <label for="<%= chkMustChangePassword.ClientID %>">Require password change on first login</label>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Form Actions -->
        <div class="form-actions">
            <asp:Button ID="btnCancelBottom" runat="server" Text="Cancel" 
                        CssClass="btn btn-outline" OnClick="btnCancel_Click" CausesValidation="false" />
            <asp:Button ID="btnSaveEmployeeBottom" runat="server" Text="Create Employee" 
                        CssClass="btn btn-primary" OnClick="btnSaveEmployee_Click" />
        </div>
    </div>

    <!-- Success Modal -->
    <div id="successModal" class="modal-overlay" style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header success-header">
                    <div class="success-icon">
                        <i class="material-icons">check_circle</i>
                    </div>
                    <h4 class="modal-title">Employee Created Successfully!</h4>
                </div>
                <div class="modal-body">
                    <div class="success-details">
                        <p><strong>Employee Number:</strong> <span id="successEmployeeNumber"></span></p>
                        <p><strong>Name:</strong> <span id="successEmployeeName"></span></p>
                        <p><strong>Department:</strong> <span id="successDepartment"></span></p>
                        <p><strong>Onboarding Tasks:</strong> <span id="successTaskCount"></span> tasks assigned</p>
                    </div>
                    <div class="success-message">
                        <p>The employee has been successfully created with onboarding tasks assigned. The employee will receive login credentials via email.</p>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" onclick="redirectToEmployeeList()">
                        <i class="material-icons">list</i> View Employee List
                    </button>
                    <button type="button" class="btn btn-outline" onclick="addAnotherEmployee()">
                        <i class="material-icons">person_add</i> Add Another Employee
                    </button>
                </div>
            </div>
        </div>
    </div>

    <!-- JavaScript for Modal Functionality -->
    <script type="text/javascript">
        function showSuccessModal(employeeNumber, employeeName, department, taskCount) {
            // Update modal content
            document.getElementById('successEmployeeNumber').textContent = employeeNumber;
            document.getElementById('successEmployeeName').textContent = employeeName;
            document.getElementById('successDepartment').textContent = department;
            document.getElementById('successTaskCount').textContent = taskCount;
            
            // Show modal
            document.getElementById('successModal').style.display = 'flex';
            
            // Add body class to prevent scrolling
            document.body.style.overflow = 'hidden';
            
            // Auto-hide modal after 10 seconds (optional)
            setTimeout(function() {
                if (document.getElementById('successModal').style.display === 'flex') {
                    redirectToEmployeeList();
                }
            }, 10000);
        }

        function hideSuccessModal() {
            document.getElementById('successModal').style.display = 'none';
            document.body.style.overflow = 'auto';
        }

        function redirectToEmployeeList() {
            hideSuccessModal();
            window.location.href = '<%= ResolveUrl("~/HR/Employees.aspx") %>';
        }

        function addAnotherEmployee() {
            hideSuccessModal();
            // Simply reload the page to clear the form
            window.location.href = '<%= ResolveUrl("~/HR/AddEmployee.aspx") %>';
        }

        // Close modal when clicking outside
        document.addEventListener('click', function (event) {
            var modal = document.getElementById('successModal');
            if (event.target === modal) {
                hideSuccessModal();
            }
        });

        // Close modal with ESC key
        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') {
                hideSuccessModal();
            }
        });
    </script>

</asp:Content>