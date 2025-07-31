<%@ Page Title="Onboarding Tasks Management" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="OnboardingTasks.aspx.cs" Inherits="TPASystem2.OnBoarding.OnboardingTasks" %>

<asp:Content ID="OnboardingTasksContent" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    
    <!-- Custom Styles for This Page -->
    <style>
        /* HR Onboarding Management Specific Styles - Matching MyOnboarding Design */
        .onboarding-header {
            background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
            color: white;
            padding: 2.5rem;
            border-radius: 16px;
            margin-bottom: 2rem;
            position: relative;
            overflow: hidden;
        }
        
        .onboarding-header::before {
            content: '';
            position: absolute;
            top: -50%;
            right: -10%;
            width: 60%;
            height: 200%;
            background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><circle cx="20" cy="20" r="2" fill="white" opacity="0.1"/><circle cx="80" cy="80" r="1.5" fill="white" opacity="0.1"/><circle cx="40" cy="60" r="1" fill="white" opacity="0.1"/><circle cx="60" cy="30" r="1.2" fill="white" opacity="0.1"/></svg>');
            opacity: 0.3;
        }
        
        .welcome-content {
            display: flex;
            justify-content: space-between;
            align-items: center;
            position: relative;
            z-index: 1;
        }
        
        .welcome-title {
            font-size: 2.5rem;
            font-weight: 600;
            margin-bottom: 0.5rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .welcome-title .material-icons {
            font-size: 2.5rem;
            color: #ffd54f;
        }
        
        .welcome-subtitle {
            font-size: 1.2rem;
            opacity: 0.9;
            margin: 0;
        }

        /* Filter Controls */
        .filter-controls {
            background: white;
            border-radius: 12px;
            padding: 1.5rem;
            margin-bottom: 2rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            display: flex;
            gap: 1.5rem;
            align-items: end;
            flex-wrap: wrap;
        }
        
        .filter-group {
            display: flex;
            flex-direction: column;
            gap: 0.5rem;
            min-width: 180px;
        }
        
        .filter-group label {
            font-size: 0.9rem;
            font-weight: 600;
            color: #374151;
        }
        
        .filter-control {
            padding: 0.75rem;
            border: 2px solid #e5e7eb;
            border-radius: 8px;
            font-size: 0.9rem;
            background: white;
            transition: border-color 0.2s ease;
        }
        
        .filter-control:focus {
            outline: none;
            border-color: #1976d2;
        }

        /* Tab Navigation */
        .tab-navigation {
            display: flex;
            background: white;
            border-radius: 12px;
            padding: 0.5rem;
            margin-bottom: 2rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        
        .tab-button {
            flex: 1;
            padding: 1rem 1.5rem;
            background: none;
            border: none;
            font-size: 1rem;
            font-weight: 600;
            color: #64748b;
            cursor: pointer;
            transition: all 0.3s ease;
            border-radius: 8px;
            text-align: center;
        }
        
        .tab-button:hover {
            color: #1976d2;
            background: #f0f8ff;
        }
        
        .tab-button.active {
            color: white;
            background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
        }

        /* Template Cards */
        .templates-container {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(400px, 1fr));
            gap: 1.5rem;
        }
        
        .template-card {
            background: white;
            border-radius: 16px;
            overflow: hidden;
            box-shadow: 0 4px 16px rgba(0,0,0,0.1);
            border: 1px solid #e5e7eb;
            transition: all 0.3s ease;
            position: relative;
        }
        
        .template-card:hover {
            transform: translateY(-4px);
            box-shadow: 0 8px 24px rgba(0,0,0,0.15);
        }

        .template-header {
            padding: 1.5rem;
            background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
            border-bottom: 1px solid #e5e7eb;
        }
        
        .template-title {
            font-size: 1.25rem;
            font-weight: 700;
            color: #1e293b;
            margin: 0 0 0.5rem 0;
            line-height: 1.3;
        }
        
        .template-category {
            display: inline-block;
            padding: 0.25rem 0.75rem;
            background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
            color: white;
            font-size: 0.75rem;
            font-weight: 600;
            border-radius: 12px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        
        .template-body {
            padding: 1.5rem;
        }
        
        .template-description {
            color: #64748b;
            line-height: 1.6;
            margin-bottom: 1.5rem;
        }
        
        .template-details {
            display: flex;
            flex-wrap: wrap;
            gap: 1rem;
            margin-bottom: 1.5rem;
        }
        
        .detail-item {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            font-size: 0.9rem;
            color: #64748b;
            background: #f8fafc;
            padding: 0.5rem 0.75rem;
            border-radius: 8px;
        }
        
        .detail-item .material-icons {
            font-size: 1rem;
            color: #1976d2;
        }

        .template-priority {
            position: absolute;
            top: 1rem;
            right: 1rem;
            padding: 0.5rem 0.75rem;
            border-radius: 12px;
            font-size: 0.75rem;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        
        .priority-high { background: #fee2e2; color: #dc2626; }
        .priority-medium { background: #fef3c7; color: #d97706; }
        .priority-low { background: #dcfce7; color: #16a34a; }

        .template-actions {
            padding: 1rem 1.5rem;
            background: #f8fafc;
            border-top: 1px solid #e5e7eb;
            display: flex;
            gap: 0.5rem;
            flex-wrap: wrap;
        }

        /* Form Container */
        .form-container {
            background: white;
            border-radius: 16px;
            box-shadow: 0 4px 16px rgba(0,0,0,0.1);
            overflow: hidden;
        }
        
        .form-header {
            background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
            color: white;
            padding: 2rem;
            position: relative;
            overflow: hidden;
        }
        
        .form-header::before {
            content: '';
            position: absolute;
            top: -50%;
            right: -10%;
            width: 60%;
            height: 200%;
            background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><circle cx="20" cy="20" r="2" fill="white" opacity="0.1"/><circle cx="80" cy="80" r="1.5" fill="white" opacity="0.1"/><circle cx="40" cy="60" r="1" fill="white" opacity="0.1"/><circle cx="60" cy="30" r="1.2" fill="white" opacity="0.1"/></svg>');
            opacity: 0.3;
        }
        
        .form-header h2 {
            display: flex;
            align-items: center;
            gap: 0.75rem;
            margin: 0 0 0.5rem 0;
            font-size: 1.75rem;
            font-weight: 700;
            position: relative;
            z-index: 1;
        }
        
        .form-header h2 .material-icons {
            font-size: 1.75rem;
        }
        
        .form-header p {
            margin: 0;
            opacity: 0.9;
            position: relative;
            z-index: 1;
        }
        
        .form-body {
            padding: 2rem;
        }
        
        .form-section {
            margin-bottom: 2rem;
        }
        
        .form-section h3 {
            margin: 0 0 1.5rem 0;
            font-size: 1.25rem;
            font-weight: 700;
            color: #1e293b;
            padding-bottom: 0.5rem;
            border-bottom: 2px solid #f1f5f9;
        }
        
        .form-row {
            display: flex;
            gap: 1.5rem;
            margin-bottom: 1rem;
        }
        
        .form-group {
            flex: 1;
            margin-bottom: 1rem;
        }
        
        .form-group.col-4 { flex: 0 0 calc(33.333% - 1rem); }
        .form-group.col-6 { flex: 0 0 calc(50% - 0.75rem); }
        .form-group.col-8 { flex: 0 0 calc(66.667% - 0.5rem); }
        
        .form-label {
            display: block;
            margin-bottom: 0.5rem;
            font-weight: 600;
            color: #374151;
            font-size: 0.9rem;
        }
        
        .form-input {
            width: 100%;
            padding: 0.75rem;
            border: 2px solid #e5e7eb;
            border-radius: 8px;
            font-size: 1rem;
            transition: border-color 0.2s ease;
            background: white;
        }
        
        .form-input:focus {
            outline: none;
            border-color: #1976d2;
        }

        .checkbox-group {
            display: flex;
            align-items: flex-start;
            gap: 0.75rem;
            margin-bottom: 1rem;
        }
        
        .checkbox-label {
            display: flex;
            flex-direction: column;
            gap: 0.25rem;
            cursor: pointer;
        }
        
        .checkbox-label strong {
            color: #374151;
            font-weight: 600;
        }
        
        .checkbox-help {
            font-size: 0.8rem;
            color: #64748b;
            line-height: 1.3;
        }

        .document-requirements {
            background: #f8fafc;
            border: 1px solid #e5e7eb;
            border-radius: 12px;
            padding: 1.5rem;
        }

        .form-footer {
            padding: 1.5rem 2rem;
            background: #f8fafc;
            border-top: 1px solid #e5e7eb;
            display: flex;
            justify-content: flex-end;
            gap: 1rem;
        }

        /* Buttons - matching existing style */
        .btn {
            padding: 0.75rem 1.5rem;
            border: none;
            border-radius: 8px;
            font-size: 0.9rem;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s ease;
            text-decoration: none;
            display: inline-flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .btn-primary {
            background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
            color: white;
        }
        
        .btn-primary:hover {
            transform: translateY(-1px);
            box-shadow: 0 4px 12px rgba(25, 118, 210, 0.4);
        }
        
        .btn-outline {
            background: white;
            border: 2px solid #e5e7eb;
            color: #64748b;
        }
        
        .btn-outline:hover {
            border-color: #1976d2;
            color: #1976d2;
        }
        
        .btn-small {
            padding: 0.5rem 1rem;
            font-size: 0.8rem;
        }
        
        .btn-danger {
            background: #ef4444;
            color: white;
        }
        
        .btn-warning {
            background: #f59e0b;
            color: white;
        }
        
        .btn-success {
            background: #10b981;
            color: white;
        }

        /* Alert Messages */
        .alert {
            display: flex;
            align-items: flex-start;
            gap: 0.75rem;
            padding: 1rem;
            border-radius: 12px;
            margin-bottom: 1.5rem;
        }
        
        .alert .material-icons {
            font-size: 1.2rem;
            margin-top: 0.1rem;
        }
        
        .alert-success {
            background: #ecfdf5;
            color: #065f46;
            border: 1px solid #d1fae5;
        }
        
        .alert-error {
            background: #fef2f2;
            color: #991b1b;
            border: 1px solid #fecaca;
        }

        /* Empty State */
        .empty-state {
            text-align: center;
            padding: 3rem 2rem;
            background: white;
            border-radius: 16px;
            box-shadow: 0 4px 16px rgba(0,0,0,0.1);
        }
        
        .empty-icon {
            width: 80px;
            height: 80px;
            margin: 0 auto 1.5rem auto;
            background: #f8fafc;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #64748b;
        }
        
        .empty-icon .material-icons {
            font-size: 2.5rem;
        }
        
        .empty-state h3 {
            margin: 0 0 0.5rem 0;
            color: #1e293b;
            font-size: 1.5rem;
            font-weight: 700;
        }
        
        .empty-state p {
            margin: 0 0 1.5rem 0;
            color: #64748b;
        }

        /* Responsive Design */
        @media (max-width: 768px) {
            .welcome-content {
                flex-direction: column;
                gap: 1.5rem;
                text-align: center;
            }
            
            .welcome-title {
                font-size: 2rem;
            }
            
            .filter-controls {
                flex-direction: column;
                gap: 1rem;
            }
            
            .filter-group {
                min-width: auto;
            }
            
            .templates-container {
                grid-template-columns: 1fr;
            }
            
            .form-row {
                flex-direction: column;
                gap: 0;
            }
            
            .form-footer {
                flex-direction: column;
            }
        }
    </style>
    
    <!-- Welcome Header -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">settings</i>
                    Onboarding Tasks Management
                </h1>
                <p class="welcome-subtitle">
                    Create and manage task templates for employee onboarding workflows
                </p>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnCreateTemplate" runat="server" Text="+ Create Template" 
                            CssClass="btn btn-primary" OnClick="btnCreateTemplate_Click" />
            </div>
        </div>
    </div>

    <!-- Success/Error Messages -->
    <asp:Panel ID="pnlMessages" runat="server" Visible="false">
        <div class="alert alert-success" id="divSuccess" runat="server" visible="false">
            <i class="material-icons">check_circle</i>
            <asp:Literal ID="litSuccessMessage" runat="server"></asp:Literal>
        </div>
        <div class="alert alert-error" id="divError" runat="server" visible="false">
            <i class="material-icons">error</i>
            <asp:Literal ID="litErrorMessage" runat="server"></asp:Literal>
        </div>
    </asp:Panel>

    <!-- Tab Navigation -->
    <div class="tab-navigation">
        <asp:Button ID="btnTabTemplates" runat="server" Text="Task Templates" 
                    CssClass="tab-button active" OnClick="btnTabTemplates_Click" CausesValidation="false" />
        <asp:Button ID="btnTabCreate" runat="server" Text="Create New Template" 
                    CssClass="tab-button" OnClick="btnTabCreate_Click" CausesValidation="false" />
    </div>

    <!-- Templates Tab -->
    <asp:Panel ID="pnlTemplatesTab" runat="server" Visible="true">
        <!-- Filters -->
        <div class="filter-controls">
            <div class="filter-group">
                <label>Department:</label>
                <asp:DropDownList ID="ddlFilterDepartment" runat="server" CssClass="filter-control" 
                                  AutoPostBack="true" OnSelectedIndexChanged="ddlFilterDepartment_SelectedIndexChanged">
                    <asp:ListItem Value="">All Departments</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-group">
                <label>Category:</label>
                <asp:DropDownList ID="ddlFilterCategory" runat="server" CssClass="filter-control" 
                                  AutoPostBack="true" OnSelectedIndexChanged="ddlFilterCategory_SelectedIndexChanged">
                    <asp:ListItem Value="">All Categories</asp:ListItem>
                    <asp:ListItem Value="DOCUMENTATION">Documentation</asp:ListItem>
                    <asp:ListItem Value="SETUP">Setup</asp:ListItem>
                    <asp:ListItem Value="ORIENTATION">Orientation</asp:ListItem>
                    <asp:ListItem Value="TRAINING">Training</asp:ListItem>
                    <asp:ListItem Value="COMPLIANCE">Compliance</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-group">
                <label>Status:</label>
                <asp:DropDownList ID="ddlFilterStatus" runat="server" CssClass="filter-control" 
                                  AutoPostBack="true" OnSelectedIndexChanged="ddlFilterStatus_SelectedIndexChanged">
                    <asp:ListItem Value="">All</asp:ListItem>
                    <asp:ListItem Value="1">Active</asp:ListItem>
                    <asp:ListItem Value="0">Inactive</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>

        <!-- Templates Grid -->
        <div class="templates-container">
            <asp:Repeater ID="rptTemplates" runat="server" OnItemCommand="rptTemplates_ItemCommand">
                <ItemTemplate>
                    <div class="template-card">
                        <div class="template-priority priority-<%# Eval("Priority").ToString().ToLower() %>">
                            <%# Eval("Priority") %>
                        </div>
                        
                        <div class="template-header">
                            <h3 class="template-title"><%# Eval("Title") %></h3>
                            <span class="template-category"><%# Eval("Category") %></span>
                        </div>
                        
                        <div class="template-body">
                            <p class="template-description"><%# Eval("Description") %></p>
                            
                            <div class="template-details">
                                <div class="detail-item">
                                    <i class="material-icons">business</i>
                                    <span><%# Eval("DepartmentName") %></span>
                                </div>
                                <div class="detail-item">
                                    <i class="material-icons">schedule</i>
                                    <span><%# Eval("EstimatedDays") %> days</span>
                                </div>
                                <div class="detail-item">
                                    <i class="material-icons">person</i>
                                    <span><%# Convert.ToBoolean(Eval("CanEmployeeComplete")) ? "Self-Complete" : "HR Managed" %></span>
                                </div>
                            </div>

                            <%# !string.IsNullOrEmpty(Eval("Instructions").ToString()) ? 
                                "<div class='template-instructions'><strong>Instructions:</strong><br/>" + Eval("Instructions") + "</div>" : "" %>
                        </div>

                        <div class="template-actions">
                            <asp:LinkButton ID="btnEdit" runat="server" CssClass="btn btn-outline btn-small" 
                                            CommandName="Edit" CommandArgument='<%# Eval("Id") %>'>
                                <i class="material-icons">edit</i> Edit
                            </asp:LinkButton>
                            
                            <asp:LinkButton ID="btnToggleStatus" runat="server" 
                                            CssClass='<%# Convert.ToBoolean(Eval("IsActive")) ? "btn btn-warning btn-small" : "btn btn-success btn-small" %>'
                                            CommandName="ToggleStatus" CommandArgument='<%# Eval("Id") %>'
                                            OnClientClick='<%# Convert.ToBoolean(Eval("IsActive")) ? "return confirm(\"Deactivate this template?\");" : "return confirm(\"Activate this template?\");" %>'>
                                <i class="material-icons"><%# Convert.ToBoolean(Eval("IsActive")) ? "visibility_off" : "visibility" %></i>
                                <%# Convert.ToBoolean(Eval("IsActive")) ? "Deactivate" : "Activate" %>
                            </asp:LinkButton>
                            
                            <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-danger btn-small" 
                                            CommandName="Delete" CommandArgument='<%# Eval("Id") %>'
                                            OnClientClick="return confirm('Are you sure you want to delete this template?');">
                                <i class="material-icons">delete</i> Delete
                            </asp:LinkButton>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <!-- Empty State -->
            <asp:Panel ID="pnlNoTemplates" runat="server" CssClass="empty-state" Visible="false">
                <div class="empty-icon">
                    <i class="material-icons">assignment</i>
                </div>
                <h3>No Task Templates Found</h3>
                <p>Create your first onboarding task template to get started.</p>
                <asp:Button ID="btnCreateFirst" runat="server" Text="Create Template" 
                            CssClass="btn btn-primary" OnClick="btnCreateTemplate_Click" />
            </asp:Panel>
        </div>
    </asp:Panel>

    <!-- Create/Edit Template Tab -->
    <asp:Panel ID="pnlCreateTab" runat="server" Visible="false">
        <div class="form-container">
            <div class="form-header">
                <h2>
                    <i class="material-icons">add_task</i>
                    <asp:Literal ID="litFormTitle" runat="server" Text="Create New Task Template"></asp:Literal>
                </h2>
                <p>Define a task template for employee onboarding workflows</p>
            </div>

            <div class="form-body">
                <!-- Hidden field for edit mode -->
                <asp:HiddenField ID="hdnTemplateId" runat="server" Value="0" />

                <!-- Basic Information -->
                <div class="form-section">
                    <h3>Basic Information</h3>
                    
                    <div class="form-row">
                        <div class="form-group col-8">
                            <label class="form-label">Task Title *</label>
                            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-input" 
                                         placeholder="Enter task title" MaxLength="200"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle" 
                                                        ErrorMessage="Task title is required" CssClass="field-error" Display="Dynamic" />
                        </div>
                        
                        <div class="form-group col-4">
                            <label class="form-label">Department *</label>
                            <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-input">
                                <asp:ListItem Value="">Select Department</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvDepartment" runat="server" ControlToValidate="ddlDepartment" 
                                                        ErrorMessage="Please select a department" CssClass="field-error" Display="Dynamic" />
                        </div>
                    </div>

                    <div class="form-group">
                        <label class="form-label">Description</label>
                        <asp:TextBox ID="txtDescription" runat="server" CssClass="form-input" TextMode="MultiLine" 
                                     Rows="3" placeholder="Describe what this task involves..." MaxLength="1000"></asp:TextBox>
                    </div>
                </div>

                <!-- Task Configuration -->
                <div class="form-section">
                    <h3>Task Configuration</h3>
                    
                    <div class="form-row">
                        <div class="form-group col-4">
                            <label class="form-label">Category *</label>
                            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-input">
                                <asp:ListItem Value="">Select Category</asp:ListItem>
                                <asp:ListItem Value="DOCUMENTATION">Documentation</asp:ListItem>
                                <asp:ListItem Value="SETUP">Setup</asp:ListItem>
                                <asp:ListItem Value="ORIENTATION">Orientation</asp:ListItem>
                                <asp:ListItem Value="TRAINING">Training</asp:ListItem>
                                <asp:ListItem Value="COMPLIANCE">Compliance</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvCategory" runat="server" ControlToValidate="ddlCategory" 
                                                        ErrorMessage="Please select a category" CssClass="field-error" Display="Dynamic" />
                        </div>
                        
                        <div class="form-group col-4">
                            <label class="form-label">Priority *</label>
                            <asp:DropDownList ID="ddlPriority" runat="server" CssClass="form-input">
                                <asp:ListItem Value="">Select Priority</asp:ListItem>
                                <asp:ListItem Value="LOW">Low</asp:ListItem>
                                <asp:ListItem Value="MEDIUM" Selected="True">Medium</asp:ListItem>
                                <asp:ListItem Value="HIGH">High</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvPriority" runat="server" ControlToValidate="ddlPriority" 
                                                        ErrorMessage="Please select a priority" CssClass="field-error" Display="Dynamic" />
                        </div>
                        
                        <div class="form-group col-4">
                            <label class="form-label">Estimated Days *</label>
                            <asp:TextBox ID="txtEstimatedDays" runat="server" CssClass="form-input" 
                                         placeholder="1" TextMode="Number" min="1" max="365"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvEstimatedDays" runat="server" ControlToValidate="txtEstimatedDays" 
                                                        ErrorMessage="Estimated days is required" CssClass="field-error" Display="Dynamic" />
                            <asp:RangeValidator ID="rvEstimatedDays" runat="server" ControlToValidate="txtEstimatedDays" 
                                                MinimumValue="1" MaximumValue="365" Type="Integer"
                                                ErrorMessage="Please enter a value between 1 and 365" CssClass="field-error" Display="Dynamic" />
                        </div>
                    </div>
                </div>

                <!-- Task Options -->
                <div class="form-section">
                    <h3>Task Options</h3>
                    
                    <div class="form-row">
                        <div class="form-group col-6">
                            <div class="checkbox-group">
                                <asp:CheckBox ID="chkCanEmployeeComplete" runat="server" />
                                <label for="<%= chkCanEmployeeComplete.ClientID %>" class="checkbox-label">
                                    <strong>Employee can self-complete</strong>
                                    <span class="checkbox-help">Allow employees to mark this task as complete themselves</span>
                                </label>
                            </div>
                        </div>
                        
                        <div class="form-group col-6">
                            <div class="checkbox-group">
                                <asp:CheckBox ID="chkBlocksSystemAccess" runat="server" />
                                <label for="<%= chkBlocksSystemAccess.ClientID %>" class="checkbox-label">
                                    <strong>Blocks system access</strong>
                                    <span class="checkbox-help">Prevent system access until this task is completed</span>
                                </label>
                            </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="checkbox-group">
                            <asp:CheckBox ID="chkRequiresDocuments" runat="server" 
                                          OnCheckedChanged="chkRequiresDocuments_CheckedChanged" AutoPostBack="true" />
                            <label for="<%= chkRequiresDocuments.ClientID %>" class="checkbox-label">
                                <strong>Requires document upload</strong>
                                <span class="checkbox-help">This task requires employees to upload documents</span>
                            </label>
                        </div>
                    </div>
                </div>

                <!-- Document Requirements (shown when checkbox is checked) -->
                <asp:Panel ID="pnlDocumentRequirements" runat="server" Visible="false" CssClass="form-section">
                    <h3>Document Requirements</h3>
                    <div class="document-requirements">
                        <div class="form-group">
                            <label class="form-label">Required Documents</label>
                            <asp:TextBox ID="txtRequiredDocuments" runat="server" CssClass="form-input" TextMode="MultiLine" 
                                         Rows="3" placeholder="List the documents that need to be uploaded (one per line)&#13;&#10;Example:&#13;&#10;- Signed offer letter&#13;&#10;- Copy of driver's license&#13;&#10;- Completed tax forms"></asp:TextBox>
                            <small class="form-help">Enter each required document on a new line</small>
                        </div>
                        
                        <div class="form-row">
                            <div class="form-group col-6">
                                <label class="form-label">Accepted File Types</label>
                                <asp:DropDownList ID="ddlFileTypes" runat="server" CssClass="form-input">
                                    <asp:ListItem Value="pdf,doc,docx">Documents (PDF, Word)</asp:ListItem>
                                    <asp:ListItem Value="pdf,doc,docx,jpg,jpeg,png">Documents + Images</asp:ListItem>
                                    <asp:ListItem Value="pdf">PDF Only</asp:ListItem>
                                    <asp:ListItem Value="jpg,jpeg,png">Images Only</asp:ListItem>
                                    <asp:ListItem Value="*">All File Types</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            
                            <div class="form-group col-6">
                                <label class="form-label">Maximum File Size (MB)</label>
                                <asp:DropDownList ID="ddlMaxFileSize" runat="server" CssClass="form-input">
                                    <asp:ListItem Value="5">5 MB</asp:ListItem>
                                    <asp:ListItem Value="10" Selected="True">10 MB</asp:ListItem>
                                    <asp:ListItem Value="25">25 MB</asp:ListItem>
                                    <asp:ListItem Value="50">50 MB</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Instructions -->
                <div class="form-section">
                    <h3>Instructions</h3>
                    <div class="form-group">
                        <label class="form-label">Detailed Instructions</label>
                        <asp:TextBox ID="txtInstructions" runat="server" CssClass="form-input" TextMode="MultiLine" 
                                     Rows="5" placeholder="Provide detailed instructions for completing this task...&#13;&#10;&#13;&#10;Include:&#13;&#10;- Step-by-step process&#13;&#10;- Required resources or contacts&#13;&#10;- Expected timeline&#13;&#10;- Any special considerations" MaxLength="2000"></asp:TextBox>
                        <small class="form-help">These instructions will be shown to employees and HR staff</small>
                    </div>
                </div>
            </div>

            <!-- Form Actions -->
            <div class="form-footer">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-outline" 
                            OnClick="btnCancel_Click" CausesValidation="false" />
                <asp:Button ID="btnSaveTemplate" runat="server" Text="Save Template" CssClass="btn btn-primary" 
                            OnClick="btnSaveTemplate_Click" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>