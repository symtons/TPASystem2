<%@ Page Title="Manage Employee Onboarding" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ManageEmployeeOnboarding.aspx.cs" Inherits="TPASystem2.HR.ManageEmployeeOnboarding" %>

<asp:Content ID="ManageOnboardingContent" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    
    <!-- Custom Styles - Matching MyOnboarding Design -->
    <style>
        /* Employee Onboarding Management Specific Styles - Matching MyOnboarding Design */
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

        .employee-info {
            display: flex;
            align-items: center;
            gap: 1rem;
            flex-wrap: wrap;
        }

        .employee-detail {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            background: rgba(255,255,255,0.1);
            padding: 0.5rem 1rem;
            border-radius: 20px;
            backdrop-filter: blur(10px);
        }

        .employee-detail .material-icons {
            font-size: 1.2rem;
        }

        .header-actions {
            flex-shrink: 0;
        }

        /* Progress Section */
        .progress-section {
            background: white;
            border-radius: 16px;
            padding: 2rem;
            margin-bottom: 2rem;
            box-shadow: 0 4px 16px rgba(0,0,0,0.1);
        }

        .progress-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1.5rem;
        }

        .progress-stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1.5rem;
            margin-bottom: 2rem;
        }

        .stat-card {
            background: linear-gradient(135deg, #f8faff 0%, #f1f5f9 100%);
            border-radius: 12px;
            padding: 1.5rem;
            text-align: center;
            border: 1px solid #e5e7eb;
        }

        .stat-number {
            font-size: 2rem;
            font-weight: 700;
            color: #1976d2;
            margin-bottom: 0.5rem;
        }

        .stat-label {
            color: #64748b;
            font-size: 0.9rem;
            font-weight: 500;
        }

        .progress-bar-container {
            background: #f1f5f9;
            border-radius: 10px;
            height: 20px;
            overflow: hidden;
            margin-bottom: 0.5rem;
        }

        .progress-bar {
            height: 100%;
            background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
            border-radius: 10px;
            transition: width 0.3s ease;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 0.8rem;
            font-weight: 600;
        }

        /* Template Selection Section */
        .template-selection {
            background: white;
            border-radius: 16px;
            padding: 2rem;
            margin-bottom: 2rem;
            box-shadow: 0 4px 16px rgba(0,0,0,0.1);
        }

        .template-selection h3 {
            margin: 0 0 1.5rem 0;
            color: #1e293b;
            font-size: 1.5rem;
            font-weight: 700;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .template-selection h3 .material-icons {
            color: #1976d2;
        }

        .templates-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
            gap: 1.5rem;
        }

        .template-card {
            background: white;
            border: 2px solid #e5e7eb;
            border-radius: 12px;
            padding: 1.5rem;
            transition: all 0.3s ease;
            cursor: pointer;
            position: relative;
        }

        .template-card:hover {
            border-color: #1976d2;
            transform: translateY(-2px);
            box-shadow: 0 8px 24px rgba(25, 118, 210, 0.15);
        }

        .template-card.selected {
            border-color: #1976d2;
            background: linear-gradient(135deg, #f0f8ff 0%, #e6f3ff 100%);
        }

        .template-header {
            margin-bottom: 1rem;
        }

        .template-title {
            font-size: 1.1rem;
            font-weight: 600;
            color: #1e293b;
            margin: 0 0 0.5rem 0;
        }

        .template-category {
            display: inline-block;
            background: #1976d2;
            color: white;
            padding: 0.25rem 0.75rem;
            border-radius: 12px;
            font-size: 0.75rem;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .template-description {
            color: #64748b;
            font-size: 0.9rem;
            line-height: 1.5;
            margin-bottom: 1rem;
        }

        .template-details {
            display: flex;
            justify-content: space-between;
            align-items: center;
            font-size: 0.8rem;
            color: #64748b;
        }

        .template-tasks-count {
            font-weight: 600;
            color: #1976d2;
        }

        .template-selection-checkbox {
            position: absolute;
            top: 1rem;
            right: 1rem;
            width: 20px;
            height: 20px;
            accent-color: #1976d2;
        }

        /* Tasks Section */
        .tasks-section {
            background: white;
            border-radius: 16px;
            padding: 2rem;
            margin-bottom: 2rem;
            box-shadow: 0 4px 16px rgba(0,0,0,0.1);
        }

        .tasks-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 2rem;
            padding-bottom: 1rem;
            border-bottom: 2px solid #f1f5f9;
        }

        .tasks-header h3 {
            margin: 0;
            color: #1e293b;
            font-size: 1.5rem;
            font-weight: 700;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .tasks-header h3 .material-icons {
            color: #1976d2;
        }

        .task-filters {
            display: flex;
            gap: 1rem;
            align-items: center;
        }

        .filter-select {
            padding: 0.5rem 1rem;
            border: 2px solid #e5e7eb;
            border-radius: 8px;
            font-size: 0.9rem;
            background: white;
        }

        /* Task Cards */
        .task-card {
            background: white;
            border: 1px solid #e5e7eb;
            border-radius: 12px;
            padding: 1.5rem;
            margin-bottom: 1rem;
            transition: all 0.3s ease;
            position: relative;
        }

        .task-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 24px rgba(0,0,0,0.1);
        }

        .task-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 1rem;
        }

        .task-title {
            font-size: 1.1rem;
            font-weight: 600;
            color: #1e293b;
            margin: 0 0 0.5rem 0;
        }

        .task-category {
            display: inline-block;
            background: #f1f5f9;
            color: #64748b;
            padding: 0.25rem 0.75rem;
            border-radius: 12px;
            font-size: 0.75rem;
            font-weight: 600;
            text-transform: uppercase;
        }

        .task-status {
            padding: 0.5rem 1rem;
            border-radius: 20px;
            font-size: 0.75rem;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .status-pending { background: #fef3c7; color: #d97706; }
        .status-in-progress { background: #dbeafe; color: #2563eb; }
        .status-completed { background: #dcfce7; color: #16a34a; }
        .status-overdue { background: #fee2e2; color: #dc2626; }

        .task-description {
            color: #64748b;
            margin-bottom: 1rem;
            line-height: 1.5;
        }

        .task-meta {
            display: flex;
            gap: 1rem;
            align-items: center;
            font-size: 0.9rem;
            color: #64748b;
            margin-bottom: 1rem;
        }

        .task-meta .meta-item {
            display: flex;
            align-items: center;
            gap: 0.25rem;
        }

        .task-meta .material-icons {
            font-size: 1rem;
            color: #1976d2;
        }

        .task-actions {
            display: flex;
            gap: 0.5rem;
            flex-wrap: wrap;
        }

        /* Buttons */
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

        .btn-success {
            background: #10b981;
            color: white;
        }

        .btn-warning {
            background: #f59e0b;
            color: white;
        }

        .btn-danger {
            background: #ef4444;
            color: white;
        }

        .btn-small {
            padding: 0.5rem 1rem;
            font-size: 0.8rem;
        }

        /* Empty State */
        .empty-state {
            text-align: center;
            padding: 3rem 2rem;
            color: #64748b;
        }

        .empty-state .material-icons {
            font-size: 4rem;
            margin-bottom: 1rem;
            opacity: 0.5;
        }

        .empty-state h3 {
            margin: 0 0 0.5rem 0;
            color: #1e293b;
        }

        /* Responsive */
        @media (max-width: 768px) {
            .welcome-content {
                flex-direction: column;
                gap: 1.5rem;
                text-align: center;
            }
            
            .welcome-title {
                font-size: 2rem;
            }
            
            .employee-info {
                justify-content: center;
            }
            
            .templates-grid {
                grid-template-columns: 1fr;
            }
            
            .progress-stats {
                grid-template-columns: repeat(2, 1fr);
            }
            
            .task-filters {
                flex-direction: column;
                gap: 0.5rem;
            }
        }
    </style>
    
    <!-- Employee Header -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">manage_accounts</i>
                    <asp:Literal ID="litEmployeeName" runat="server" Text="Employee Name"></asp:Literal>
                </h1>
                <p class="welcome-subtitle">
                    Onboarding Management & Task Assignment
                </p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">badge</i>
                        <asp:Literal ID="litEmployeeNumber" runat="server" Text="EMP001"></asp:Literal>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">event</i>
                        Hired: <asp:Literal ID="litHireDate" runat="server" Text="Jan 1, 2024"></asp:Literal>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">business</i>
                        <asp:Literal ID="litDepartment" runat="server" Text="Department"></asp:Literal>
                    </div>
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnBack" runat="server" Text="← Back to List" 
                            CssClass="btn btn-outline" OnClick="btnBack_Click" CausesValidation="false" />
            </div>
        </div>
    </div>

    <!-- Progress Section -->
    <div class="progress-section">
        <div class="progress-header">
            <h3>
                <i class="material-icons">trending_up</i>
                Onboarding Progress
            </h3>
        </div>

        <div class="progress-stats">
            <div class="stat-card">
                <div class="stat-number">
                    <asp:Literal ID="litTotalTasks" runat="server" Text="0"></asp:Literal>
                </div>
                <div class="stat-label">Total Tasks</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">
                    <asp:Literal ID="litCompletedTasks" runat="server" Text="0"></asp:Literal>
                </div>
                <div class="stat-label">Completed</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">
                    <asp:Literal ID="litPendingTasks" runat="server" Text="0"></asp:Literal>
                </div>
                <div class="stat-label">Pending</div>
            </div>
            <div class="stat-card">
                <div class="stat-number">
                    <asp:Literal ID="litProgressPercent" runat="server" Text="0"></asp:Literal>%
                </div>
                <div class="stat-label">Progress</div>
            </div>
        </div>

        <div class="progress-bar-container">
            <div class="progress-bar" style="width: <asp:Literal ID="litProgressWidth" runat="server" Text="0"></asp:Literal>%">
                <asp:Literal ID="litProgressText" runat="server" Text="0% Complete"></asp:Literal>
            </div>
        </div>
    </div>

    <!-- Template Selection Section -->
    <asp:Panel ID="pnlTemplateSelection" runat="server" CssClass="template-selection">
        <h3>
            <i class="material-icons">library_add</i>
            Assign Task Templates
        </h3>
        
        <div class="templates-grid">
            <asp:Repeater ID="rptAvailableTemplates" runat="server">
                <ItemTemplate>
                    <div class="template-card" onclick="toggleTemplate(this, <%# Eval("Id") %>)">
                        <asp:CheckBox ID="chkSelectTemplate" runat="server" 
                                      CssClass="template-selection-checkbox" />
                        <asp:HiddenField ID="hdnTemplateId" runat="server" Value='<%# Eval("Id") %>' />
                        
                        <div class="template-header">
                            <h4 class="template-title"><%# Eval("Title") %></h4>
                            <span class="template-category"><%# Eval("Category") %></span>
                        </div>
                        
                        <p class="template-description"><%# Eval("Description") %></p>
                        
                        <div class="template-details">
                            <span class="template-tasks-count"><%# Eval("TaskCount") %> tasks</span>
                            <span><%# Eval("EstimatedDays") %> days</span>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
        
        <div style="margin-top: 2rem; text-align: right;">
            <asp:Button ID="btnAssignTemplates" runat="server" Text="Assign Selected Templates" 
                        CssClass="btn btn-primary" OnClick="btnAssignTemplates_Click" />
        </div>
    </asp:Panel>

    <!-- Current Tasks Section -->
    <div class="tasks-section">
        <div class="tasks-header">
            <h3>
                <i class="material-icons">assignment</i>
                Current Onboarding Tasks
            </h3>
            <div class="task-filters">
                <asp:DropDownList ID="ddlTaskFilter" runat="server" CssClass="filter-select" 
                                  AutoPostBack="true" OnSelectedIndexChanged="ddlTaskFilter_SelectedIndexChanged">
                    <asp:ListItem Value="">All Tasks</asp:ListItem>
                    <asp:ListItem Value="PENDING">Pending</asp:ListItem>
                    <asp:ListItem Value="IN_PROGRESS">In Progress</asp:ListItem>
                    <asp:ListItem Value="COMPLETED">Completed</asp:ListItem>
                    <asp:ListItem Value="OVERDUE">Overdue</asp:ListItem>
                </asp:DropDownList>
                
                <asp:Button ID="btnAddCustomTask" runat="server" Text="+ Add Custom Task" 
                            CssClass="btn btn-primary btn-small" OnClick="btnAddCustomTask_Click" />
            </div>
        </div>

        <asp:Repeater ID="rptTasks" runat="server" OnItemCommand="rptTasks_ItemCommand">
            <ItemTemplate>
                <div class="task-card">
                    <div class="task-header">
                        <div>
                            <h4 class="task-title"><%# Eval("Title") %></h4>
                            <span class="task-category"><%# Eval("Category") %></span>
                        </div>
                        <span class="task-status status-<%# Eval("Status").ToString().ToLower().Replace("_", "-") %>">
                            <%# Eval("Status") %>
                        </span>
                    </div>
                    
                    <p class="task-description"><%# Eval("Description") %></p>
                    
                    <div class="task-meta">
                        <div class="meta-item">
                            <i class="material-icons">schedule</i>
                            Due: <%# Eval("DueDate", "{0:MMM dd, yyyy}") %>
                        </div>
                        <div class="meta-item">
                            <i class="material-icons">flag</i>
                            <%# Eval("Priority") %> Priority
                        </div>
                        <div class="meta-item">
                            <i class="material-icons">timer</i>
                            <%# Eval("EstimatedTime") %>
                        </div>
                    </div>

                    <div class="task-actions">
                        <%# Eval("Status").ToString() == "PENDING" ? 
                            "<asp:LinkButton runat=\"server\" CommandName=\"StartTask\" CommandArgument=\"" + Eval("Id") + "\" CssClass=\"btn btn-primary btn-small\"><i class=\"material-icons\">play_arrow</i>Start Task</asp:LinkButton>" : "" %>
                        
                        <%# Eval("Status").ToString() == "IN_PROGRESS" ? 
                            "<asp:LinkButton runat=\"server\" CommandName=\"CompleteTask\" CommandArgument=\"" + Eval("Id") + "\" CssClass=\"btn btn-success btn-small\"><i class=\"material-icons\">check</i>Complete</asp:LinkButton>" : "" %>
                        
                        <asp:LinkButton runat="server" CommandName="EditTask" CommandArgument='<%# Eval("Id") %>' 
                                        CssClass="btn btn-outline btn-small">
                            <i class="material-icons">edit</i>Edit
                        </asp:LinkButton>
                        
                        <asp:LinkButton runat="server" CommandName="DeleteTask" CommandArgument='<%# Eval("Id") %>' 
                                        CssClass="btn btn-danger btn-small"
                                        OnClientClick="return confirm('Are you sure you want to delete this task?');">
                            <i class="material-icons">delete</i>Delete
                        </asp:LinkButton>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <!-- Empty State -->
        <asp:Panel ID="pnlEmptyTasks" runat="server" CssClass="empty-state" Visible="false">
            <i class="material-icons">assignment_turned_in</i>
            <h3>No Tasks Assigned Yet</h3>
            <p>Assign task templates or create custom tasks to get started with onboarding.</p>
        </asp:Panel>
    </div>

    <!-- JavaScript for Template Selection -->
    <script type="text/javascript">
        function toggleTemplate(card, templateId) {
            const checkbox = card.querySelector('input[type="checkbox"]');
            checkbox.checked = !checkbox.checked;

            if (checkbox.checked) {
                card.classList.add('selected');
            } else {
                card.classList.remove('selected');
            }
        }
    </script>
</asp:Content>