<%@ Page Title="Manage Employee Onboarding" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ManageEmployeeOnboarding.aspx.cs" Inherits="TPASystem2.HR.ManageEmployeeOnboarding" %>

<asp:Content ID="ManageOnboardingContent" ContentPlaceHolderID="DashboardContent" runat="server">
    
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/employee-management.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    
    <!-- Page Header -->
    <div class="page-header">
        <div class="page-header-content">
            <h1>
                <asp:Literal ID="litEmployeeName" runat="server"></asp:Literal>
                <span class="page-subtitle">Onboarding Management</span>
            </h1>
            <div class="employee-quick-info">
                <span class="employee-number">
                    <i class="material-icons">badge</i>
                    <asp:Literal ID="litEmployeeNumber" runat="server"></asp:Literal>
                </span>
                <span class="hire-date">
                    <i class="material-icons">event</i>
                    Hired: <asp:Literal ID="litHireDate" runat="server"></asp:Literal>
                </span>
                <span class="department">
                    <i class="material-icons">business</i>
                    <asp:Literal ID="litDepartment" runat="server"></asp:Literal>
                </span>
            </div>
        </div>
        <div class="page-header-actions">
            <asp:Button ID="btnBack" runat="server" Text="← Back to List" 
                        CssClass="btn btn-outline" OnClick="btnBack_Click" CausesValidation="false" />
            <asp:Button ID="btnAddTask" runat="server" Text="+ Add Custom Task" 
                        CssClass="btn btn-primary" OnClick="btnAddTask_Click" />
            <asp:Button ID="btnCompleteAll" runat="server" Text="Complete All Tasks" 
                        CssClass="btn btn-success" OnClick="btnCompleteAll_Click" 
                        OnClientClick="return confirm('Mark all pending tasks as complete?');" />
        </div>
    </div>

    <!-- Progress Overview Card -->
    <div class="progress-overview-card">
        <div class="progress-header">
            <h3>Onboarding Progress</h3>
            <div class="progress-actions">
                <asp:Button ID="btnGenerateReport" runat="server" Text="Generate Report" 
                            CssClass="btn btn-secondary btn-sm" OnClick="btnGenerateReport_Click" />
            </div>
        </div>
        <div class="progress-content">
            <div class="progress-visual">
                <div class="circular-progress">
                    <svg class="progress-ring" width="120" height="120">
                        <circle class="progress-ring-circle" stroke="#e0e0e0" stroke-width="8" 
                                fill="transparent" r="52" cx="60" cy="60"/>
                        <circle class="progress-ring-circle progress-ring-fill" stroke="#4caf50" stroke-width="8" 
                                fill="transparent" r="52" cx="60" cy="60" 
                                stroke-dasharray="<asp:Literal ID="litProgressCircumference" runat="server"></asp:Literal>"
                                stroke-dashoffset="<asp:Literal ID="litProgressOffset" runat="server"></asp:Literal>"/>
                    </svg>
                    <div class="progress-percentage">
                        <asp:Literal ID="litCompletionPercentage" runat="server">0</asp:Literal>%
                    </div>
                </div>
            </div>
            <div class="progress-stats">
                <div class="stat-group">
                    <div class="stat-item">
                        <div class="stat-number">
                            <asp:Literal ID="litTotalTasks" runat="server">0</asp:Literal>
                        </div>
                        <div class="stat-label">Total Tasks</div>
                    </div>
                    <div class="stat-item">
                        <div class="stat-number completed">
                            <asp:Literal ID="litCompletedTasks" runat="server">0</asp:Literal>
                        </div>
                        <div class="stat-label">Completed</div>
                    </div>
                    <div class="stat-item">
                        <div class="stat-number in-progress">
                            <asp:Literal ID="litInProgressTasks" runat="server">0</asp:Literal>
                        </div>
                        <div class="stat-label">In Progress</div>
                    </div>
                    <div class="stat-item">
                        <div class="stat-number pending">
                            <asp:Literal ID="litPendingTasks" runat="server">0</asp:Literal>
                        </div>
                        <div class="stat-label">Pending</div>
                    </div>
                    <div class="stat-item">
                        <div class="stat-number overdue">
                            <asp:Literal ID="litOverdueTasks" runat="server">0</asp:Literal>
                        </div>
                        <div class="stat-label">Overdue</div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Task Management Section -->
    <div class="task-management-section">
        <div class="section-header">
            <h3>
                <i class="material-icons">assignment</i>
                Onboarding Tasks
            </h3>
            <div class="task-filters">
                <asp:DropDownList ID="ddlTaskFilter" runat="server" CssClass="form-control filter-dropdown"
                                  AutoPostBack="true" OnSelectedIndexChanged="ddlTaskFilter_SelectedIndexChanged">
                    <asp:ListItem Value="" Text="All Tasks" />
                    <asp:ListItem Value="PENDING" Text="Pending" />
                    <asp:ListItem Value="IN_PROGRESS" Text="In Progress" />
                    <asp:ListItem Value="COMPLETED" Text="Completed" />
                    <asp:ListItem Value="OVERDUE" Text="Overdue" />
                </asp:DropDownList>
                
                <asp:DropDownList ID="ddlCategoryFilter" runat="server" CssClass="form-control filter-dropdown"
                                  AutoPostBack="true" OnSelectedIndexChanged="ddlCategoryFilter_SelectedIndexChanged">
                    <asp:ListItem Value="" Text="All Categories" />
                    <asp:ListItem Value="DOCUMENTATION" Text="Documentation" />
                    <asp:ListItem Value="SETUP" Text="Setup" />
                    <asp:ListItem Value="ORIENTATION" Text="Orientation" />
                    <asp:ListItem Value="TRAINING" Text="Training" />
                    <asp:ListItem Value="MEETING" Text="Meeting" />
                    <asp:ListItem Value="EQUIPMENT" Text="Equipment" />
                </asp:DropDownList>
                
                <asp:DropDownList ID="ddlPriorityFilter" runat="server" CssClass="form-control filter-dropdown"
                                  AutoPostBack="true" OnSelectedIndexChanged="ddlPriorityFilter_SelectedIndexChanged">
                    <asp:ListItem Value="" Text="All Priorities" />
                    <asp:ListItem Value="HIGH" Text="High Priority" />
                    <asp:ListItem Value="MEDIUM" Text="Medium Priority" />
                    <asp:ListItem Value="LOW" Text="Low Priority" />
                </asp:DropDownList>
            </div>
        </div>
        
        <!-- Task List -->
        <div class="task-list-container">
            <asp:Repeater ID="rptTasks" runat="server" OnItemCommand="rptTasks_ItemCommand" OnItemDataBound="rptTasks_ItemDataBound">
                <ItemTemplate>
                    <div class="task-card <%# GetTaskCardClass(Eval("Status").ToString(), Eval("IsOverdue")) %>" 
                         data-task-id="<%# Eval("TaskId") %>">
                        <div class="task-header">
                            <div class="task-title-section">
                                <h4 class="task-title"><%# Eval("Title") %></h4>
                                <div class="task-meta">
                                    <span class="task-category category-<%# Eval("Category").ToString().ToLower() %>">
                                        <%# GetCategoryDisplay(Eval("Category").ToString()) %>
                                    </span>
                                    <span class="task-priority priority-<%# Eval("Priority").ToString().ToLower() %>">
                                        <%# Eval("Priority") %> Priority
                                    </span>
                                    <span class="task-assigned">
                                        <i class="material-icons">person</i>
                                        <%# GetAssignedToDisplay(Eval("AssignedToRole").ToString()) %>
                                    </span>
                                </div>
                            </div>
                            <div class="task-status-section">
                                <span class="task-status status-<%# Eval("Status").ToString().ToLower().Replace("_", "-") %>">
                                    <%# GetStatusDisplay(Eval("Status").ToString()) %>
                                </span>
                                <div class="task-actions">
                                    <asp:LinkButton ID="btnEditTask" runat="server" 
                                                    CommandName="EditTask" 
                                                    CommandArgument='<%# Eval("TaskId") %>'
                                                    CssClass="btn btn-outline btn-sm" 
                                                    ToolTip="Edit Task">
                                        <i class="material-icons">edit</i>
                                    </asp:LinkButton>
                                    
                                    <asp:LinkButton ID="btnMarkComplete" runat="server" 
                                                    CommandName="MarkComplete" 
                                                    CommandArgument='<%# Eval("TaskId") %>'
                                                    CssClass="btn btn-success btn-sm" 
                                                    ToolTip="Mark Complete"
                                                    Visible='<%# Eval("Status").ToString() != "COMPLETED" %>'
                                                    OnClientClick="return confirm('Mark this task as complete?');">
                                        <i class="material-icons">check</i>
                                    </asp:LinkButton>
                                    
                                    <asp:LinkButton ID="btnMarkInProgress" runat="server" 
                                                    CommandName="MarkInProgress" 
                                                    CommandArgument='<%# Eval("TaskId") %>'
                                                    CssClass="btn btn-warning btn-sm" 
                                                    ToolTip="Mark In Progress"
                                                    Visible='<%# Eval("Status").ToString() == "PENDING" %>'>
                                        <i class="material-icons">play_arrow</i>
                                    </asp:LinkButton>
                                    
                                    <asp:LinkButton ID="btnDeleteTask" runat="server" 
                                                    CommandName="DeleteTask" 
                                                    CommandArgument='<%# Eval("TaskId") %>'
                                                    CssClass="btn btn-danger btn-sm" 
                                                    ToolTip="Delete Task"
                                                    OnClientClick="return confirm('Delete this task? This action cannot be undone.');">
                                        <i class="material-icons">delete</i>
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                        
                        <div class="task-body">
                            <div class="task-description">
                                <%# Eval("Description") %>
                            </div>
                            
                            <asp:Panel ID="pnlTaskInstructions" runat="server" CssClass="task-instructions" 
                                       Visible='<%# !string.IsNullOrEmpty(Eval("Instructions")?.ToString()) %>'>
                                <strong>Instructions:</strong>
                                <div class="instructions-content"><%# Eval("Instructions") %></div>
                            </asp:Panel>
                        </div>
                        
                        <div class="task-footer">
                            <div class="task-timeline">
                                <div class="timeline-item">
                                    <i class="material-icons">event</i>
                                    <span>Due: <%# Eval("DueDate", "{0:MMM dd, yyyy}") %></span>
                                    <asp:Panel ID="pnlOverdueIndicator" runat="server" CssClass="overdue-indicator" 
                                               Visible='<%# Convert.ToBoolean(Eval("IsOverdue")) %>'>
                                        <i class="material-icons">warning</i>
                                        Overdue
                                    </asp:Panel>
                                </div>
                                <div class="timeline-item">
                                    <i class="material-icons">schedule</i>
                                    <span>Est. Time: <%# Eval("EstimatedTime") %></span>
                                </div>
                                <asp:Panel ID="pnlCompletedInfo" runat="server" CssClass="timeline-item completed-info" 
                                           Visible='<%# Eval("Status").ToString() == "COMPLETED" %>'>
                                    <i class="material-icons">check_circle</i>
                                    <span>Completed: <%# Eval("CompletedDate", "{0:MMM dd, yyyy}") %></span>
                                    <span class="completed-by">by <%# Eval("CompletedByName") %></span>
                                </asp:Panel>
                            </div>
                            
                            <div class="task-details-actions">
                                <div class="detail-counts">
                                    <span class="detail-count" title="Documents">
                                        <i class="material-icons">attach_file</i>
                                        <%# Eval("DocumentCount") %>
                                    </span>
                                    <span class="detail-count" title="Comments">
                                        <i class="material-icons">comment</i>
                                        <%# Eval("CommentCount") %>
                                    </span>
                                </div>
                                
                                <div class="action-links">
                                    <asp:LinkButton ID="btnViewDocuments" runat="server" 
                                                    CommandName="ViewDocuments" 
                                                    CommandArgument='<%# Eval("TaskId") %>'
                                                    CssClass="action-link" 
                                                    Text="Documents" />
                                    
                                    <asp:LinkButton ID="btnViewComments" runat="server" 
                                                    CommandName="ViewComments" 
                                                    CommandArgument='<%# Eval("TaskId") %>'
                                                    CssClass="action-link" 
                                                    Text="Comments" />
                                    
                                    <asp:LinkButton ID="btnTaskDetails" runat="server" 
                                                    CommandName="TaskDetails" 
                                                    CommandArgument='<%# Eval("TaskId") %>'
                                                    CssClass="action-link" 
                                                    Text="Details" />
                                </div>
                            </div>
                        </div>
                        
                        <!-- Task Notes Section (if exists) -->
                        <asp:Panel ID="pnlTaskNotes" runat="server" CssClass="task-notes" 
                                   Visible='<%# !string.IsNullOrEmpty(Eval("Notes")?.ToString()) %>'>
                            <h5>Notes:</h5>
                            <div class="notes-content"><%# Eval("Notes") %></div>
                        </asp:Panel>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            
            <!-- Empty State -->
            <asp:Panel ID="pnlEmptyTasks" runat="server" CssClass="empty-state" Visible="false">
                <div class="empty-icon">
                    <i class="material-icons">assignment</i>
                </div>
                <h3>No tasks found</h3>
                <p>No onboarding tasks match your current filter criteria.</p>
                <asp:Button ID="btnClearTaskFilters" runat="server" Text="Clear Filters" 
                            CssClass="btn btn-primary" OnClick="btnClearTaskFilters_Click" />
            </asp:Panel>
        </div>
    </div>

    <!-- Add Custom Task Modal -->
    <asp:Panel ID="pnlAddTaskModal" runat="server" CssClass="modal-overlay" Visible="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h4>Add Custom Onboarding Task</h4>
                    <asp:Button ID="btnCloseAddTaskModal" runat="server" Text="×" 
                                CssClass="btn btn-text modal-close" OnClick="btnCloseAddTaskModal_Click" 
                                CausesValidation="false" />
                </div>
                <div class="modal-body">
                    <div class="form-grid">
                        <div class="form-row">
                            <div class="form-group">
                                <label for="<%= txtTaskTitle.ClientID %>">Task Title <span class="required">*</span></label>
                                <asp:TextBox ID="txtTaskTitle" runat="server" CssClass="form-control" 
                                             placeholder="Enter task title" MaxLength="255" required />
                                <asp:RequiredFieldValidator ID="rfvTaskTitle" runat="server" 
                                                           ControlToValidate="txtTaskTitle" 
                                                           ErrorMessage="Task title is required" 
                                                           CssClass="error-message" Display="Dynamic" />
                            </div>
                            
                            <div class="form-group">
                                <label for="<%= ddlTaskCategory.ClientID %>">Category <span class="required">*</span></label>
                                <asp:DropDownList ID="ddlTaskCategory" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="" Text="Select Category" />
                                    <asp:ListItem Value="DOCUMENTATION" Text="Documentation" />
                                    <asp:ListItem Value="SETUP" Text="Setup" />
                                    <asp:ListItem Value="ORIENTATION" Text="Orientation" />
                                    <asp:ListItem Value="TRAINING" Text="Training" />
                                    <asp:ListItem Value="MEETING" Text="Meeting" />
                                    <asp:ListItem Value="EQUIPMENT" Text="Equipment" />
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvTaskCategory" runat="server" 
                                                           ControlToValidate="ddlTaskCategory" 
                                                           ErrorMessage="Please select a category" 
                                                           CssClass="error-message" Display="Dynamic" />
                            </div>
                        </div>
                        
                        <div class="form-group">
                            <label for="<%= txtTaskDescription.ClientID %>">Description</label>
                            <asp:TextBox ID="txtTaskDescription" runat="server" CssClass="form-control" 
                                         TextMode="MultiLine" Rows="3" 
                                         placeholder="Enter task description" MaxLength="1000" />
                        </div>
                        
                        <div class="form-row">
                            <div class="form-group">
                                <label for="<%= ddlTaskPriority.ClientID %>">Priority <span class="required">*</span></label>
                                <asp:DropDownList ID="ddlTaskPriority" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="HIGH" Text="High Priority" />
                                    <asp:ListItem Value="MEDIUM" Text="Medium Priority" Selected="True" />
                                    <asp:ListItem Value="LOW" Text="Low Priority" />
                                </asp:DropDownList>
                            </div>
                            
                            <div class="form-group">
                                <label for="<%= txtTaskDueDate.ClientID %>">Due Date <span class="required">*</span></label>
                                <asp:TextBox ID="txtTaskDueDate" runat="server" CssClass="form-control" 
                                             TextMode="Date" required />
                                <asp:RequiredFieldValidator ID="rfvTaskDueDate" runat="server" 
                                                           ControlToValidate="txtTaskDueDate" 
                                                           ErrorMessage="Due date is required" 
                                                           CssClass="error-message" Display="Dynamic" />
                            </div>
                        </div>
                        
                        <div class="form-row">
                            <div class="form-group">
                                <label for="<%= txtEstimatedTime.ClientID %>">Estimated Time</label>
                                <asp:TextBox ID="txtEstimatedTime" runat="server" CssClass="form-control" 
                                             placeholder="e.g., 2 hours, 1 day" MaxLength="50" />
                            </div>
                            
                            <div class="form-group">
                                <label for="<%= ddlAssignedToRole.ClientID %>">Assigned To <span class="required">*</span></label>
                                <asp:DropDownList ID="ddlAssignedToRole" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="HR" Text="HR Department" Selected="True" />
                                    <asp:ListItem Value="MANAGER" Text="Direct Manager" />
                                    <asp:ListItem Value="EMPLOYEE" Text="Employee (Self)" />
                                    <asp:ListItem Value="IT" Text="IT Department" />
                                    <asp:ListItem Value="FACILITIES" Text="Facilities" />
                                    <asp:ListItem Value="FINANCE" Text="Finance Department" />
                                </asp:DropDownList>
                            </div>
                        </div>
                        
                        <div class="form-group">
                            <label for="<%= txtTaskInstructions.ClientID %>">Instructions</label>
                            <asp:TextBox ID="txtTaskInstructions" runat="server" CssClass="form-control" 
                                         TextMode="MultiLine" Rows="4" 
                                         placeholder="Enter detailed instructions for completing this task" 
                                         MaxLength="2000" />
                        </div>
                        
                        <div class="form-row">
                            <div class="form-group checkbox-group">
                                <asp:CheckBox ID="chkCanEmployeeComplete" runat="server" CssClass="form-check-input" 
                                              Checked="true" />
                                <label for="<%= chkCanEmployeeComplete.ClientID %>" class="form-check-label">
                                    Employee can complete this task
                                </label>
                            </div>
                            
                            <div class="form-group checkbox-group">
                                <asp:CheckBox ID="chkBlocksSystemAccess" runat="server" CssClass="form-check-input" />
                                <label for="<%= chkBlocksSystemAccess.ClientID %>" class="form-check-label">
                                    Blocks system access until completed
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnCancelAddTask" runat="server" Text="Cancel" 
                                CssClass="btn btn-outline" OnClick="btnCancelAddTask_Click" 
                                CausesValidation="false" />
                    <asp:Button ID="btnSaveTask" runat="server" Text="Add Task" 
                                CssClass="btn btn-primary" OnClick="btnSaveTask_Click" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Task Details Modal -->
    <asp:Panel ID="pnlTaskDetailsModal" runat="server" CssClass="modal-overlay" Visible="false">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h4>Task Details</h4>
                    <asp:Button ID="btnCloseTaskDetailsModal" runat="server" Text="×" 
                                CssClass="btn btn-text modal-close" OnClick="btnCloseTaskDetailsModal_Click" 
                                CausesValidation="false" />
                </div>
                <div class="modal-body">
                    <asp:Literal ID="litTaskDetailsContent" runat="server"></asp:Literal>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnCloseTaskDetails" runat="server" Text="Close" 
                                CssClass="btn btn-primary" OnClick="btnCloseTaskDetailsModal_Click" 
                                CausesValidation="false" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <!-- Notification Panel -->
    <asp:Panel ID="pnlNotification" runat="server" CssClass="notification-panel" Visible="false">
        <div class="notification-content">
            <asp:Literal ID="litNotificationMessage" runat="server"></asp:Literal>
        </div>
        <asp:Button ID="btnCloseNotification" runat="server" Text="×" 
                    CssClass="btn btn-text notification-close" OnClick="btnCloseNotification_Click" />
    </asp:Panel>

    <!-- Additional Styles for Task Management -->
    <style>
        /* Employee Quick Info */
        .employee-quick-info {
            display: flex;
            gap: 1.5rem;
            margin-top: 0.5rem;
            flex-wrap: wrap;
        }

            .employee-quick-info span {
                display: flex;
                align-items: center;
                gap: 0.25rem;
                font-size: 0.9rem;
                color: #757575;
            }

                .employee-quick-info span i {
                    font-size: 1rem;
                    color: #1976d2;
                }

        /* Progress Overview Card */
        .progress-overview-card {
            background: white;
            border-radius: 12px;
            box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
            border: 1px solid #e0e0e0;
            margin-bottom: 2rem;
            overflow: hidden;
        }

        .progress-header {
            padding: 1.5rem;
            border-bottom: 1px solid #e0e0e0;
            display: flex;
            justify-content: space-between;
            align-items: center;
            background: #f8f9fa;
        }

            .progress-header h3 {
                margin: 0;
                color: #212121;
                font-size: 1.25rem;
            }

        .progress-content {
            padding: 2rem;
            display: flex;
            gap: 2rem;
            align-items: center;
        }

        /* Circular Progress */
        .circular-progress {
            position: relative;
            width: 120px;
            height: 120px;
        }

        .progress-ring {
            transform: rotate(-90deg);
        }

        .progress-ring-circle {
            transition: stroke-dashoffset 0.5s ease-in-out;
        }

        .progress-percentage {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            font-size: 1.5rem;
            font-weight: 700;
            color: #4caf50;
        }

        /* Progress Stats */
        .progress-stats {
            flex: 1;
        }

        .stat-group {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(100px, 1fr));
            gap: 1.5rem;
        }

        .stat-item {
            text-align: center;
        }

        .stat-number {
            font-size: 2rem;
            font-weight: 700;
            color: #212121;
            line-height: 1;
        }

            .stat-number.completed {
                color: #4caf50;
            }

            .stat-number.in-progress {
                color: #ff9800;
            }

            .stat-number.pending {
                color: #2196f3;
            }

            .stat-number.overdue {
                color: #f44336;
            }

        .stat-label {
            font-size: 0.85rem;
            color: #757575;
            margin-top: 0.25rem;
        }

        /* Task Management Section */
        .task-management-section {
            margin-bottom: 2rem;
        }

        .task-filters {
            display: flex;
            gap: 1rem;
            align-items: center;
        }

        .filter-dropdown {
            min-width: 150px;
        }

        /* Task Cards */
        .task-list-container {
            display: flex;
            flex-direction: column;
            gap: 1rem;
        }

        .task-card {
            background: white;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            border: 1px solid #e0e0e0;
            transition: transform 0.2s ease, box-shadow 0.2s ease;
            overflow: hidden;
        }

            .task-card:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
            }

            .task-card.completed {
                border-left: 4px solid #4caf50;
                background: #f8fff8;
            }

            .task-card.in-progress {
                border-left: 4px solid #ff9800;
                background: #fff8f0;
            }

            .task-card.pending {
                border-left: 4px solid #2196f3;
                background: #f0f8ff;
            }

            .task-card.overdue {
                border-left: 4px solid #f44336;
                background: #fff0f0;
            }

        .task-header {
            padding: 1.5rem;
            border-bottom: 1px solid #f0f0f0;
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
        }

        .task-title-section {
            flex: 1;
        }

        .task-title {
            margin: 0 0 0.5rem 0;
            font-size: 1.1rem;
            font-weight: 600;
            color: #212121;
            line-height: 1.3;
        }

        .task-meta {
            display: flex;
            gap: 1rem;
            flex-wrap: wrap;
            align-items: center;
        }

        .task-category {
            padding: 0.25rem 0.75rem;
            border-radius: 4px;
            font-size: 0.75rem;
            font-weight: 600;
            text-transform: uppercase;
        }

        .category-documentation {
            background: #e3f2fd;
            color: #1565c0;
        }

        .category-setup {
            background: #f3e5f5;
            color: #7b1fa2;
        }

        .category-orientation {
            background: #e8f5e8;
            color: #2e7d32;
        }

        .category-training {
            background: #fff3e0;
            color: #ef6c00;
        }

        .category-meeting {
            background: #fce4ec;
            color: #c2185b;
        }

        .category-equipment {
            background: #f1f8e9;
            color: #558b2f;
        }

        .task-priority {
            padding: 0.25rem 0.75rem;
            border-radius: 4px;
            font-size: 0.75rem;
            font-weight: 600;
        }

        .priority-high {
            background: #ffebee;
            color: #c62828;
        }

        .priority-medium {
            background: #fff3e0;
            color: #ef6c00;
        }

        .priority-low {
            background: #e8f5e8;
            color: #2e7d32;
        }

        .task-assigned {
            display: flex;
            align-items: center;
            gap: 0.25rem;
            font-size: 0.8rem;
            color: #757575;
        }

        .task-status-section {
            display: flex;
            flex-direction: column;
            align-items: flex-end;
            gap: 0.5rem;
        }

        .task-status {
            padding: 0.5rem 1rem;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
            text-transform: uppercase;
        }

        .status-completed {
            background: #e8f5e8;
            color: #2e7d32;
        }

        .status-in-progress {
            background: #fff3e0;
            color: #ef6c00;
        }

        .status-pending {
            background: #e3f2fd;
            color: #1565c0;
        }

        .status-overdue {
            background: #ffebee;
            color: #c62828;
        }

        .task-actions {
            display: flex;
            gap: 0.5rem;
        }

        .task-body {
            padding: 0 1.5rem 1rem 1.5rem;
        }

        .task-description {
            color: #424242;
            line-height: 1.5;
            margin-bottom: 1rem;
        }

        .task-instructions {
            background: #f8f9fa;
            border-radius: 4px;
            padding: 1rem;
            margin-top: 1rem;
        }

        .instructions-content {
            margin-top: 0.5rem;
            color: #424242;
            line-height: 1.5;
        }

        .task-footer {
            padding: 1rem 1.5rem;
            background: #fafafa;
            border-top: 1px solid #f0f0f0;
        }

        .task-timeline {
            display: flex;
            flex-direction: column;
            gap: 0.5rem;
            margin-bottom: 1rem;
        }

        .timeline-item {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            font-size: 0.85rem;
            color: #757575;
        }

            .timeline-item i {
                font-size: 1rem;
                color: #1976d2;
            }

        .completed-info {
            color: #4caf50 !important;
        }

            .completed-info i {
                color: #4caf50 !important;
            }

        .completed-by {
            font-style: italic;
        }

        .overdue-indicator {
            display: flex;
            align-items: center;
            gap: 0.25rem;
            background: #ffebee;
            color: #c62828;
            padding: 0.25rem 0.5rem;
            border-radius: 4px;
            font-size: 0.75rem;
            font-weight: 600;
            margin-left: 0.5rem;
        }

        .task-details-actions {
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .detail-counts {
            display: flex;
            gap: 1rem;
        }

        .detail-count {
            display: flex;
            align-items: center;
            gap: 0.25rem;
            font-size: 0.8rem;
            color: #757575;
        }

            .detail-count i {
                font-size: 0.9rem;
            }

        .action-links {
            display: flex;
            gap: 1rem;
        }

        .action-link {
            color: #1976d2;
            text-decoration: none;
            font-size: 0.85rem;
            font-weight: 500;
        }

            .action-link:hover {
                text-decoration: underline;
            }

        .task-notes {
            padding: 1rem 1.5rem;
            background: #f8f9fa;
            border-top: 1px solid #f0f0f0;
        }

            .task-notes h5 {
                margin: 0 0 0.5rem 0;
                font-size: 0.9rem;
                font-weight: 600;
                color: #424242;
            }

        .notes-content {
            color: #424242;
            line-height: 1.5;
        }

        /* Modal Styles */
        .modal-overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.5);
            z-index: 1000;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .modal-dialog {
            background: white;
            border-radius: 8px;
            box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
            max-width: 600px;
            width: 90%;
            max-height: 90vh;
            overflow-y: auto;
        }

        .modal-lg {
            max-width: 800px;
        }

        .modal-header {
            padding: 1.5rem;
            border-bottom: 1px solid #e0e0e0;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

            .modal-header h4 {
                margin: 0;
                color: #212121;
            }

        .modal-close {
            width: 30px;
            height: 30px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.2rem;
            color: #757575;
        }

        .modal-body {
            padding: 1.5rem;
        }

        .modal-footer {
            padding: 1rem 1.5rem;
            border-top: 1px solid #e0e0e0;
            display: flex;
            justify-content: flex-end;
            gap: 1rem;
        }

        /* Form Styles in Modal */
        .form-grid .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 1rem;
        }

        .checkbox-group {
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .checkbox-group .form-check-label {
            margin: 0;
            font-size: 0.9rem;
            color: #424242;
        }

        /* Responsive Design */
        @media (max-width: 768px) {
            .employee-quick-info {
                flex-direction: column;
                gap: 0.5rem;
            }

            .progress-content {
                flex-direction: column;
                text-align: center;
            }

            .task-filters {
                flex-direction: column;
                align-items: stretch;
            }

            .task-header {
                flex-direction: column;
                gap: 1rem;
            }

            .task-status-section {
                align-items: flex-start;
            }

            .task-meta {
                flex-direction: column;
                gap: 0.5rem;
                align-items: flex-start;
            }

            .task-details-actions {
                flex-direction: column;
                gap: 1rem;
                align-items: flex-start;
            }

            .modal-dialog {
                width: 95%;
                margin: 1rem;
            }

            .form-grid .form-row {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>