<%@ Page Title="My Onboarding" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="MyOnboarding.aspx.cs" Inherits="TPASystem2.OnBoarding.MyOnboarding" %>

<asp:Content ID="OnboardingContent" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    
    <!-- Custom Styles for Mandatory Tasks -->
    <style>
        /* Mandatory Tasks Specific Styles */
        .mandatory-section {
            background: linear-gradient(135deg, #e8f5e8 0%, #f1f8e9 100%);
            border: 2px solid #4caf50;
            border-radius: 16px;
            padding: 2rem;
            margin-bottom: 2rem;
            position: relative;
            overflow: hidden;
        }
        
        .mandatory-section::before {
            content: '⚡';
            position: absolute;
            top: 1rem;
            right: 1rem;
            font-size: 2rem;
            opacity: 0.3;
        }
        
        .mandatory-header {
            display: flex;
            align-items: center;
            gap: 1rem;
            margin-bottom: 1.5rem;
        }
        
        .mandatory-icon {
            background: linear-gradient(135deg, #4caf50 0%, #66bb6a 100%);
            color: white;
            width: 60px;
            height: 60px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.8rem;
            box-shadow: 0 4px 12px rgba(76, 175, 80, 0.3);
        }
        
        .mandatory-title {
            color: #2e7d32;
            font-size: 1.8rem;
            font-weight: 700;
            margin: 0;
        }
        
        .mandatory-subtitle {
            color: #388e3c;
            font-size: 1rem;
            margin: 0;
            opacity: 0.9;
        }
        
        .mandatory-progress {
            background: white;
            border-radius: 12px;
            padding: 1.5rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin-bottom: 1.5rem;
        }
        
        .progress-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1rem;
        }
        
        .progress-title {
            font-size: 1.1rem;
            font-weight: 600;
            color: #2e7d32;
        }
        
        .progress-percentage {
            font-size: 1.5rem;
            font-weight: 700;
            color: #4caf50;
        }
        
        .progress-bar-container {
            background: #e8f5e8;
            height: 12px;
            border-radius: 6px;
            overflow: hidden;
            margin-bottom: 0.5rem;
        }
        
        .progress-bar {
            height: 100%;
            background: linear-gradient(135deg, #4caf50 0%, #66bb6a 100%);
            border-radius: 6px;
            transition: width 0.5s ease;
            position: relative;
        }
        
        .progress-bar::after {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: linear-gradient(45deg, transparent 30%, rgba(255,255,255,0.2) 50%, transparent 70%);
            animation: shine 2s infinite;
        }
        
        @keyframes shine {
            0% { transform: translateX(-100%); }
            100% { transform: translateX(100%); }
        }
        
        .progress-text {
            font-size: 0.9rem;
            color: #388e3c;
            text-align: center;
        }
        
        .task-card.mandatory {
            border: 3px solid #4caf50;
            background: linear-gradient(135deg, #ffffff 0%, #f8fff8 100%);
            position: relative;
            transform: scale(1.02);
            box-shadow: 0 8px 24px rgba(76, 175, 80, 0.2);
        }
        
        .task-card.mandatory::before {
            content: 'MANDATORY';
            position: absolute;
            top: -1px;
            right: -1px;
            background: linear-gradient(135deg, #4caf50 0%, #66bb6a 100%);
            color: white;
            padding: 0.3rem 0.8rem;
            font-size: 0.7rem;
            font-weight: 700;
            border-radius: 0 16px 0 12px;
            letter-spacing: 0.5px;
            text-shadow: 0 1px 2px rgba(0,0,0,0.2);
        }
        
        .task-card.mandatory .task-title {
            color: #2e7d32;
            font-weight: 600;
        }
        
        .task-card.mandatory .task-priority {
            background: #4caf50;
        }
        
        .system-access-warning {
            background: linear-gradient(135deg, #fff3e0 0%, #ffe0b3 100%);
            border: 2px solid #ff9800;
            border-radius: 12px;
            padding: 1.5rem;
            margin-bottom: 2rem;
            display: flex;
            align-items: center;
            gap: 1rem;
        }
        
        .warning-icon {
            background: #ff9800;
            color: white;
            width: 50px;
            height: 50px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.5rem;
        }
        
        .warning-content h3 {
            color: #ef6c00;
            margin: 0 0 0.5rem 0;
            font-size: 1.2rem;
        }
        
        .warning-content p {
            color: #f57c00;
            margin: 0;
        }
        
        .regular-tasks-section {
            margin-top: 3rem;
        }
        
        .section-divider {
            display: flex;
            align-items: center;
            margin: 3rem 0 2rem 0;
        }
        
        .section-divider::before,
        .section-divider::after {
            content: '';
            flex: 1;
            height: 2px;
            background: linear-gradient(135deg, #e0e0e0 0%, #f5f5f5 100%);
        }
        
        .section-divider span {
            padding: 0 2rem;
            color: #666;
            font-weight: 600;
            background: white;
        }
        
        /* Rest of existing styles from MyOnboarding.aspx */
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
            background: rgba(255, 255, 255, 0.1);
            padding: 1rem 1.5rem;
            border-radius: 12px;
            backdrop-filter: blur(10px);
        }
        
        .employee-avatar {
            width: 60px;
            height: 60px;
            background: linear-gradient(135deg, #ffd54f 0%, #ffb300 100%);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.5rem;
            color: #1976d2;
            font-weight: 700;
        }
        
        .employee-details h3 {
            margin: 0;
            font-size: 1.3rem;
        }
        
        .employee-details p {
            margin: 0;
            opacity: 0.8;
        }
        
        .task-card {
            background: white;
            border-radius: 16px;
            padding: 2rem;
            margin-bottom: 1.5rem;
            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
            transition: all 0.3s ease;
            border: 2px solid transparent;
        }
        
        .task-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
        }
        
        .task-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 1.5rem;
        }
        
        .task-title {
            font-size: 1.4rem;
            font-weight: 600;
            color: #333;
            margin: 0 0 0.5rem 0;
        }
        
        .task-meta {
            display: flex;
            gap: 1rem;
            align-items: center;
        }
        
        .task-category {
            padding: 0.3rem 0.8rem;
            background: #e3f2fd;
            color: #1976d2;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 500;
        }
        
        .task-priority {
            padding: 0.3rem 0.8rem;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
            text-transform: uppercase;
        }
        
        .priority-high {
            background: #ffebee;
            color: #d32f2f;
        }
        
        .priority-medium {
            background: #fff3e0;
            color: #f57c00;
        }
        
        .priority-low {
            background: #e8f5e8;
            color: #388e3c;
        }
        
        .task-status {
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .status-badge {
            padding: 0.5rem 1rem;
            border-radius: 25px;
            font-size: 0.9rem;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        
        .status-pending {
            background: #fff3e0;
            color: #f57c00;
        }
        
        .status-completed {
            background: #e8f5e8;
            color: #388e3c;
        }
        
        .status-in_progress {
            background: #e3f2fd;
            color: #1976d2;
        }
        
        .task-description {
            color: #666;
            line-height: 1.6;
            margin-bottom: 1.5rem;
        }
        
        .task-due-date {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            color: #666;
            margin-bottom: 1rem;
        }
        
        .task-due-date.overdue {
            color: #d32f2f;
            font-weight: 600;
        }
        
        .task-instructions {
            background: #f8f9fa;
            padding: 1.5rem;
            border-radius: 12px;
            margin-bottom: 1.5rem;
            border-left: 4px solid #1976d2;
        }
        
        .task-instructions h4 {
            margin: 0 0 1rem 0;
            color: #1976d2;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .task-actions {
            display: flex;
            gap: 1rem;
            justify-content: flex-end;
            margin-top: 2rem;
        }
        
        .btn-complete {
            background: linear-gradient(135deg, #4caf50 0%, #66bb6a 100%);
            color: white;
            border: none;
            padding: 0.8rem 2rem;
            border-radius: 25px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .btn-complete:hover {
            background: linear-gradient(135deg, #388e3c 0%, #4caf50 100%);
            transform: translateY(-1px);
            box-shadow: 0 4px 12px rgba(76, 175, 80, 0.3);
        }
        
        .empty-state {
            text-align: center;
            padding: 4rem 2rem;
            color: #666;
        }
        
        .empty-icon {
            font-size: 4rem;
            color: #4caf50;
            margin-bottom: 1rem;
        }
        
        .notification {
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 10000;
            padding: 1rem 1.5rem;
            border-radius: 8px;
            color: white;
            font-weight: 600;
            animation: slideInRight 0.3s ease;
        }
        
        .notification.success {
            background: #4caf50;
        }
        
        .notification.error {
            background: #f44336;
        }
        
        @keyframes slideInRight {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }

        .task-completion-status {
            text-align: center;
            padding: 1rem;
            border-radius: 12px;
            margin-top: 1rem;
        }

        .task-completion-status.completed {
            background: #e8f5e8;
        }

        .task-completion-status.not-employee-task {
            background: #f5f5f5;
        }
    </style>

    <!-- Page Header -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div>
                <h1 class="welcome-title">
                    <i class="material-icons">assignment_ind</i>
                    Welcome to Your Onboarding Journey!
                </h1>
                <p class="welcome-subtitle">Complete your mandatory tasks to get started</p>
            </div>
            <div class="employee-info">
                <div class="employee-avatar">
                    <asp:Literal ID="litEmployeeInitials" runat="server"></asp:Literal>
                </div>
                <div class="employee-details">
                    <h3><asp:Literal ID="litEmployeeName" runat="server"></asp:Literal></h3>
                    <p>Employee #<asp:Literal ID="litEmployeeNumber" runat="server"></asp:Literal></p>
                    <p><asp:Literal ID="litDepartment" runat="server"></asp:Literal> • Hired <asp:Literal ID="litHireDate" runat="server"></asp:Literal></p>
                </div>
            </div>
        </div>
    </div>

    <!-- System Access Warning -->
    <asp:Panel ID="pnlSystemAccessWarning" runat="server" CssClass="system-access-warning" Visible="false">
        <div class="warning-icon">
            <i class="material-icons">warning</i>
        </div>
        <div class="warning-content">
            <h3>System Access Limited</h3>
            <p>Complete your mandatory onboarding tasks to gain full access to all system features.</p>
        </div>
    </asp:Panel>

    <!-- Mandatory Tasks Section -->
    <asp:Panel ID="pnlMandatoryTasks" runat="server" CssClass="mandatory-section">
        <div class="mandatory-header">
            <div class="mandatory-icon">
                <i class="material-icons">priority_high</i>
            </div>
            <div>
                <h2 class="mandatory-title">Mandatory Onboarding Tasks</h2>
                <p class="mandatory-subtitle">These tasks must be completed before accessing all system features</p>
            </div>
        </div>
        
        <div class="mandatory-progress">
            <div class="progress-header">
                <span class="progress-title">Mandatory Tasks Progress</span>
                <span class="progress-percentage">
                    <asp:Literal ID="litMandatoryProgress" runat="server">0%</asp:Literal>
                </span>
            </div>
            <div class="progress-bar-container">
                <div class="progress-bar" style="width: 0%" id="mandatoryProgressBar"></div>
            </div>
            <div class="progress-text">
                <asp:Literal ID="litMandatoryProgressText" runat="server">0 of 3 mandatory tasks completed</asp:Literal>
            </div>
        </div>

        <!-- Mandatory Tasks Repeater -->
        <asp:Repeater ID="rptMandatoryTasks" runat="server" OnItemCommand="rptTasks_ItemCommand">
            <ItemTemplate>
                <div class="task-card mandatory">
                    <div class="task-header">
                        <div class="task-title-section">
                            <h3 class="task-title"><%# Eval("Title") %></h3>
                            <div class="task-meta">
                                <span class="task-category"><%# Eval("Category") %></span>
                                <span class="task-priority priority-<%# Eval("Priority").ToString().ToLower() %>">
                                    <%# Eval("Priority") %>
                                </span>
                                <%# Convert.ToBoolean(Eval("BlocksSystemAccess")) ? "<span class='task-category' style='background: #ffebee; color: #d32f2f;'>Blocks Access</span>" : "" %>
                            </div>
                        </div>
                        <div class="task-status">
                            <span class="status-badge status-<%# Eval("Status").ToString().ToLower() %>">
                                <%# Eval("Status").ToString().Replace("_", " ") %>
                            </span>
                        </div>
                    </div>

                    <div class="task-body">
                        <p class="task-description"><%# Eval("Description") %></p>
                        
                        <div class="task-due-date <%# Convert.ToBoolean(Eval("IsOverdue")) ? "overdue" : "" %>">
                            <i class="material-icons">schedule</i>
                            <span>Due: <%# Convert.ToDateTime(Eval("DueDate")).ToString("MMM dd, yyyy") %></span>
                            <%# Convert.ToBoolean(Eval("IsOverdue")) ? "<strong>(OVERDUE)</strong>" : "" %>
                        </div>
                        
                        <%# !string.IsNullOrEmpty(Eval("Instructions").ToString()) ? 
                            "<div class='task-instructions'><h4><i class='material-icons'>info</i>Instructions:</h4><p>" + Eval("Instructions") + "</p></div>" : "" %>
                    </div>

                    <!-- Task Actions - Fixed to use proper server controls -->
                    <asp:Panel ID="pnlTaskActions" runat="server" CssClass="task-actions" 
                               Visible='<%# Eval("Status").ToString() != "COMPLETED" && Convert.ToBoolean(Eval("CanEmployeeComplete")) %>'>
                        <asp:Button ID="btnCompleteTask" runat="server" Text="Complete This Task" 
                                    CssClass="btn-complete" 
                                    CommandName="COMPLETE_MANDATORY" 
                                    CommandArgument='<%# Eval("TaskId") %>' />
                    </asp:Panel>
                    
                    <!-- Completed Status -->
                    <asp:Panel ID="pnlCompletedStatus" runat="server" CssClass="task-completion-status completed"
                               Visible='<%# Eval("Status").ToString() == "COMPLETED" %>'>
                        <i class="material-icons" style="color: #4caf50; font-size: 2rem;">check_circle</i>
                        <p style="color: #2e7d32; font-weight: 600; margin: 0.5rem 0 0 0;">Task Completed!</p>
                    </asp:Panel>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>

    <!-- Section Divider -->
    <div class="section-divider">
        <span>Additional Onboarding Tasks</span>
    </div>

    <!-- Regular Tasks Section -->
    <div class="regular-tasks-section">
        <asp:Repeater ID="rptRegularTasks" runat="server" OnItemCommand="rptTasks_ItemCommand">
            <ItemTemplate>
                <div class="task-card">
                    <div class="task-header">
                        <div class="task-title-section">
                            <h3 class="task-title"><%# Eval("Title") %></h3>
                            <div class="task-meta">
                                <span class="task-category"><%# Eval("Category") %></span>
                                <span class="task-priority priority-<%# Eval("Priority").ToString().ToLower() %>">
                                    <%# Eval("Priority") %>
                                </span>
                            </div>
                        </div>
                        <div class="task-status">
                            <span class="status-badge status-<%# Eval("Status").ToString().ToLower() %>">
                                <%# Eval("Status").ToString().Replace("_", " ") %>
                            </span>
                        </div>
                    </div>

                    <div class="task-body">
                        <p class="task-description"><%# Eval("Description") %></p>
                        
                        <div class="task-due-date <%# Convert.ToBoolean(Eval("IsOverdue")) ? "overdue" : "" %>">
                            <i class="material-icons">schedule</i>
                            <span>Due: <%# Convert.ToDateTime(Eval("DueDate")).ToString("MMM dd, yyyy") %></span>
                            <%# Convert.ToBoolean(Eval("IsOverdue")) ? "<strong>(OVERDUE)</strong>" : "" %>
                        </div>
                        
                        <%# !string.IsNullOrEmpty(Eval("Instructions").ToString()) ? 
                            "<div class='task-instructions'><h4><i class='material-icons'>info</i>Instructions:</h4><p>" + Eval("Instructions") + "</p></div>" : "" %>
                    </div>

                    <!-- Task Actions for Regular Tasks -->
                    <asp:Panel ID="pnlRegularTaskActions" runat="server" CssClass="task-actions" 
                               Visible='<%# Eval("Status").ToString() != "COMPLETED" && Convert.ToBoolean(Eval("CanEmployeeComplete")) %>'>
                        <asp:Button ID="btnCompleteRegularTask" runat="server" Text="Complete Task" 
                                    CssClass="btn-complete" 
                                    CommandName="COMPLETE_TASK" 
                                    CommandArgument='<%# Eval("TaskId") %>' />
                    </asp:Panel>
                    
                    <!-- Completed Status for Regular Tasks -->
                    <asp:Panel ID="pnlRegularCompletedStatus" runat="server" CssClass="task-completion-status completed"
                               Visible='<%# Eval("Status").ToString() == "COMPLETED" %>'>
                        <i class="material-icons" style="color: #4caf50; font-size: 2rem;">check_circle</i>
                        <p style="color: #2e7d32; font-weight: 600; margin: 0.5rem 0 0 0;">Task Completed!</p>
                    </asp:Panel>
                    
                    <!-- Non-Employee Completable Tasks -->
                    <asp:Panel ID="pnlNonEmployeeTask" runat="server" CssClass="task-completion-status not-employee-task"
                               Visible='<%# Eval("Status").ToString() != "COMPLETED" && !Convert.ToBoolean(Eval("CanEmployeeComplete")) %>'>
                        <i class="material-icons" style="color: #666; font-size: 2rem;">people</i>
                        <p style="color: #666; margin: 0.5rem 0 0 0;">This task will be completed by <%# GetAssignedToDisplay(Eval("AssignedToRole").ToString()) %></p>
                    </asp:Panel>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <!-- Empty State for Regular Tasks -->
        <asp:Panel ID="pnlEmptyRegularTasks" runat="server" CssClass="empty-state" Visible="false">
            <div class="empty-icon">
                <i class="material-icons">assignment_turned_in</i>
            </div>
            <h3>All Additional Tasks Complete! 🎉</h3>
            <p>You've finished all your additional onboarding tasks.</p>
        </asp:Panel>
    </div>

    <!-- Empty State for All Tasks -->
    <asp:Panel ID="pnlEmptyTasks" runat="server" CssClass="empty-state" Visible="false">
        <div class="empty-icon">
            <i class="material-icons">assignment_turned_in</i>
        </div>
        <h3>All Done! Welcome to the Team! 🎉</h3>
        <p>You've completed all your onboarding tasks. Welcome aboard!</p>
         <asp:LinkButton ID="GoToDashboard" runat="server" OnClick="GoToDashboard_Click" CssClass="btn-tpa">
     <i class="material-icons">exit_to_app</i>Go to Dashboard
 </asp:LinkButton>
        
    </asp:Panel>

    <!-- Success Modal -->
    <div id="successModal" class="modal-overlay" style="display: none;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h3><i class="material-icons">check_circle</i>Task Completed!</h3>
                </div>
                <div class="modal-body">
                    <p>Great job! You've successfully completed this task.</p>
                    <div id="modalTaskDetails"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn-tpa" onclick="closeSuccessModal()">Continue</button>
                </div>
            </div>
        </div>
    </div>

    <!-- JavaScript -->
    <script>
        // Update mandatory progress bar
        function updateMandatoryProgress() {
            const progressText = '<%= litMandatoryProgress.Text %>';
            const percentage = parseFloat(progressText.replace('%', ''));
            const progressBar = document.getElementById('mandatoryProgressBar');
            if (progressBar) {
                progressBar.style.width = percentage + '%';
            }
        }

        // Show success modal
        function showSuccessModal(taskTitle) {
            const modal = document.getElementById('successModal');
            const taskDetails = document.getElementById('modalTaskDetails');
            if (taskTitle) {
                taskDetails.innerHTML = '<strong>Task:</strong> ' + taskTitle;
            }
            modal.style.display = 'flex';
        }

        // Close success modal
        function closeSuccessModal() {
            document.getElementById('successModal').style.display = 'none';
            location.reload(); // Refresh to show updated progress
        }

        // Initialize on page load
        document.addEventListener('DOMContentLoaded', function () {
            updateMandatoryProgress();
        });
    </script>
</asp:Content>