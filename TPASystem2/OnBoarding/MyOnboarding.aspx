<%@ Page Title="My Onboarding" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="MyOnboarding.aspx.cs" Inherits="TPASystem2.OnBoarding.MyOnboarding" %>

<asp:Content ID="OnboardingContent" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    
    <!-- Custom Styles for This Page -->
    <style>
        /* Employee Onboarding Specific Styles */
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
            margin-bottom: 1.5rem;
        }
        
        .employee-info {
            display: flex;
            gap: 2rem;
            flex-wrap: wrap;
        }
        
        .info-item {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            font-size: 1rem;
            opacity: 0.95;
        }
        
        .info-item .material-icons {
            font-size: 1.2rem;
        }
        
        .progress-section {
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 1.5rem;
        }
        
        .progress-ring {
            position: relative;
            width: 120px;
            height: 120px;
        }
        
        .progress-ring-svg {
            width: 100%;
            height: 100%;
            transform: rotate(-90deg);
        }
        
        .progress-ring-background {
            fill: none;
            stroke: rgba(255,255,255,0.2);
            stroke-width: 8;
        }
        
        .progress-ring-fill {
            fill: none;
            stroke: white;
            stroke-width: 8;
            stroke-linecap: round;
            transition: stroke-dasharray 0.8s ease;
        }
        
        .progress-text {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            text-align: center;
        }
        
        .progress-percentage {
            display: block;
            font-size: 1.8rem;
            font-weight: bold;
            line-height: 1;
        }
        
        .progress-label {
            display: block;
            font-size: 0.9rem;
            opacity: 0.8;
            margin-top: 0.2rem;
        }
        
        .progress-stats {
            display: flex;
            align-items: center;
            gap: 1rem;
            font-size: 1.1rem;
        }
        
        .stat-item {
            text-align: center;
        }
        
        .stat-number {
            display: block;
            font-size: 1.5rem;
            font-weight: bold;
            line-height: 1;
        }
        
        .stat-label {
            display: block;
            font-size: 0.9rem;
            opacity: 0.8;
            margin-top: 0.2rem;
        }
        
        .stat-divider {
            font-size: 1.8rem;
            opacity: 0.6;
        }
        
        /* Filter Section */
        .filter-section {
            background: white;
            padding: 1.5rem;
            border-radius: 12px;
            box-shadow: 0 2px 12px rgba(0,0,0,0.08);
            margin-bottom: 2rem;
        }
        
        .filter-controls {
            display: flex;
            gap: 3rem;
            align-items: center;
            flex-wrap: wrap;
        }
        
        .filter-group {
            display: flex;
            align-items: center;
            gap: 1rem;
        }
        
        .filter-label {
            font-weight: 600;
            color: #424242;
            font-size: 0.95rem;
        }
        
        .filter-buttons {
            display: flex;
            gap: 0.5rem;
        }
        
        .filter-btn {
            padding: 0.5rem 1rem;
            border: 2px solid #e0e0e0;
            background: white;
            border-radius: 20px;
            font-size: 0.9rem;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.2s ease;
            display: flex;
            align-items: center;
            gap: 0.3rem;
        }
        
        .filter-btn:hover {
            border-color: #1976d2;
            color: #1976d2;
        }
        
        .filter-btn.active {
            background: #1976d2;
            border-color: #1976d2;
            color: white;
        }
        
        .filter-btn.priority-high { border-color: #f44336; color: #f44336; }
        .filter-btn.priority-high.active { background: #f44336; }
        .filter-btn.priority-medium { border-color: #ff9800; color: #ff9800; }
        .filter-btn.priority-medium.active { background: #ff9800; }
        .filter-btn.priority-low { border-color: #4caf50; color: #4caf50; }
        .filter-btn.priority-low.active { background: #4caf50; }
        
        /* Task Cards */
        .tasks-container {
            display: flex;
            flex-direction: column;
            gap: 1.5rem;
        }
        
        .task-card {
            background: white;
            border-radius: 16px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            overflow: hidden;
            transition: all 0.3s ease;
            border-left: 4px solid #e0e0e0;
        }
        
        .task-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 24px rgba(0,0,0,0.15);
        }
        
        .task-card.priority-high { border-left-color: #f44336; }
        .task-card.priority-medium { border-left-color: #ff9800; }
        .task-card.priority-low { border-left-color: #4caf50; }
        .task-card.completed { 
            border-left-color: #4caf50; 
            background: #f8fff8;
            opacity: 0.9;
        }
        .task-card.overdue { border-left-color: #f44336; }
        
        .task-header {
            padding: 1.5rem;
            border-bottom: 1px solid #f0f0f0;
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
        }
        
        .task-title-section {
            display: flex;
            gap: 1rem;
            flex: 1;
        }
        
        .task-status-icon {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            flex-shrink: 0;
        }
        
        .task-title {
            font-size: 1.25rem;
            font-weight: 600;
            color: #212121;
            margin-bottom: 0.5rem;
        }
        
        .task-meta {
            display: flex;
            gap: 1rem;
            flex-wrap: wrap;
            align-items: center;
        }
        
        .task-category, .task-priority, .task-due-date {
            display: flex;
            align-items: center;
            gap: 0.3rem;
            padding: 0.3rem 0.8rem;
            border-radius: 16px;
            font-size: 0.85rem;
            font-weight: 500;
        }
        
        .task-category { background: #e3f2fd; color: #1976d2; }
        .task-category.documentation { background: #fff3e0; color: #f57c00; }
        .task-category.setup { background: #e8f5e8; color: #388e3c; }
        .task-category.orientation { background: #fce4ec; color: #c2185b; }
        .task-category.training { background: #f3e5f5; color: #7b1fa2; }
        
        .task-priority.priority-high { background: #ffebee; color: #d32f2f; }
        .task-priority.priority-medium { background: #fff3e0; color: #f57c00; }
        .task-priority.priority-low { background: #e8f5e8; color: #388e3c; }
        
        .task-due-date { background: #f5f5f5; color: #616161; }
        .task-due-date.overdue { background: #ffebee; color: #d32f2f; }
        
        .task-actions {
            display: flex;
            align-items: center;
            gap: 1rem;
        }
        
        .btn-task-complete {
            background: #4caf50;
            color: white;
            border: none;
            padding: 0.75rem 1.5rem;
            border-radius: 8px;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.2s ease;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .btn-task-complete:hover {
            background: #45a049;
            transform: translateY(-1px);
        }
        
        .completed-badge {
            background: #e8f5e8;
            color: #388e3c;
            padding: 0.5rem 1rem;
            border-radius: 20px;
            font-size: 0.9rem;
            font-weight: 500;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .task-body {
            padding: 1.5rem;
        }
        
        .task-description {
            margin-bottom: 1.5rem;
        }
        
        .task-description p {
            color: #666;
            line-height: 1.6;
        }
        
        .task-instructions {
            background: #f8f9fa;
            padding: 1rem;
            border-radius: 8px;
            margin-bottom: 1.5rem;
            border-left: 4px solid #2196f3;
        }
        
        .task-instructions h4 {
            color: #1976d2;
            font-size: 1rem;
            margin-bottom: 0.5rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .task-instructions p {
            color: #555;
            margin: 0;
        }
        
        .task-details {
            display: flex;
            flex-wrap: wrap;
            gap: 1.5rem;
            margin-bottom: 1rem;
        }
        
        .detail-item {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            color: #666;
            font-size: 0.9rem;
        }
        
        .detail-item.completed-detail {
            color: #388e3c;
        }
        
        .detail-item .material-icons {
            font-size: 1.1rem;
        }
        
        .task-notes {
            background: #fffbf0;
            padding: 1rem;
            border-radius: 8px;
            border-left: 4px solid #ff9800;
        }
        
        .task-notes h4 {
            color: #f57c00;
            font-size: 1rem;
            margin-bottom: 0.5rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .task-notes p {
            color: #555;
            margin: 0;
        }
        
        .task-footer {
            padding: 1rem 1.5rem;
            background: #f8f9fa;
            border-top: 1px solid #e0e0e0;
        }
        
        .help-text {
            color: #666;
            font-size: 0.9rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        /* Empty State */
        .empty-state {
            text-align: center;
            padding: 4rem 2rem;
            background: white;
            border-radius: 16px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
        }
        
        .empty-icon .material-icons {
            font-size: 4rem;
            color: #4caf50;
            margin-bottom: 1rem;
        }
        
        .empty-state h3 {
            color: #212121;
            margin-bottom: 1rem;
        }
        
        .empty-state p {
            color: #666;
            margin-bottom: 2rem;
        }
        
        /* Modal */
        .modal {
            display: none;
            position: fixed;
            z-index: 1000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.5);
        }
        
        .modal-content {
            background-color: white;
            margin: 15% auto;
            padding: 0;
            border-radius: 16px;
            width: 400px;
            max-width: 90%;
            box-shadow: 0 8px 32px rgba(0,0,0,0.2);
        }
        
        .modal-header {
            padding: 2rem 2rem 1rem;
            text-align: center;
        }
        
        .modal-header h3 {
            color: #4caf50;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 0.5rem;
            margin: 0;
        }
        
        .modal-body {
            padding: 0 2rem 1rem;
            text-align: center;
        }
        
        .modal-footer {
            padding: 1rem 2rem 2rem;
            text-align: center;
        }
        
        /* Responsive Design */
        @media (max-width: 768px) {
            .welcome-content {
                flex-direction: column;
                gap: 2rem;
                text-align: center;
            }
            
            .welcome-title {
                font-size: 2rem;
            }
            
            .employee-info {
                justify-content: center;
            }
            
            .filter-controls {
                gap: 1.5rem;
            }
            
            .filter-group {
                flex-direction: column;
                align-items: flex-start;
                gap: 0.5rem;
            }
            
            .task-header {
                flex-direction: column;
                gap: 1rem;
                align-items: flex-start;
            }
            
            .task-actions {
                width: 100%;
                justify-content: flex-start;
            }
        }
    </style>
    
    <!-- Welcome Header with Progress -->
    <div class="onboarding-header">
        <div class="welcome-section">
            <div class="welcome-content">
                <div class="welcome-text">
                    <h1 class="welcome-title">
                        <i class="material-icons">waving_hand</i>
                        Welcome to TPA, <asp:Literal ID="litEmployeeName" runat="server"></asp:Literal>!
                    </h1>
                    <p class="welcome-subtitle">
                        Complete your onboarding tasks to get started. You're doing great!
                    </p>
                    <div class="employee-info">
                        <span class="info-item">
                            <i class="material-icons">badge</i>
                            <asp:Literal ID="litEmployeeNumber" runat="server"></asp:Literal>
                        </span>
                        <span class="info-item">
                            <i class="material-icons">business</i>
                            <asp:Literal ID="litDepartment" runat="server"></asp:Literal>
                        </span>
                        <span class="info-item">
                            <i class="material-icons">event</i>
                            Started: <asp:Literal ID="litHireDate" runat="server"></asp:Literal>
                        </span>
                    </div>
                </div>
                <div class="progress-section">
                    <div class="progress-ring">
                        <svg class="progress-ring-svg">
                            <circle class="progress-ring-background" cx="60" cy="60" r="52"></circle>
                            <circle class="progress-ring-fill" cx="60" cy="60" r="52" 
                                    id="progressCircle" stroke-dasharray="0 327"></circle>
                        </svg>
                        <div class="progress-text">
                            <span class="progress-percentage" id="progressPercentage">0%</span>
                            <span class="progress-label">Complete</span>
                        </div>
                    </div>
                    <div class="progress-stats">
                        <div class="stat-item">
                            <span class="stat-number" id="completedTasks">0</span>
                            <span class="stat-label">Completed</span>
                        </div>
                        <div class="stat-divider">/</div>
                        <div class="stat-item">
                            <span class="stat-number" id="totalTasks">0</span>
                            <span class="stat-label">Total Tasks</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Task Filters -->
    <div class="filter-section">
        <div class="filter-controls">
            <div class="filter-group">
                <label class="filter-label">Show:</label>
                <div class="filter-buttons">
                    <button class="filter-btn active" onclick="filterTasks('all')">
                        <i class="material-icons">list</i>
                        All Tasks
                    </button>
                    <button class="filter-btn" onclick="filterTasks('pending')">
                        <i class="material-icons">pending</i>
                        Pending
                    </button>
                    <button class="filter-btn" onclick="filterTasks('completed')">
                        <i class="material-icons">check_circle</i>
                        Completed
                    </button>
                </div>
            </div>
            <div class="filter-group">
                <label class="filter-label">Priority:</label>
                <div class="filter-buttons">
                    <button class="filter-btn" onclick="filterByPriority('all')">All</button>
                    <button class="filter-btn priority-high" onclick="filterByPriority('HIGH')">High</button>
                    <button class="filter-btn priority-medium" onclick="filterByPriority('MEDIUM')">Medium</button>
                    <button class="filter-btn priority-low" onclick="filterByPriority('LOW')">Low</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Onboarding Tasks -->
    <div class="tasks-container">
        <asp:Repeater ID="rptTasks" runat="server" OnItemCommand="rptTasks_ItemCommand">
            <ItemTemplate>
                <div class="task-card <%# GetTaskCardClass(Eval("Status").ToString(), Eval("Priority").ToString(), Convert.ToBoolean(Eval("IsOverdue"))) %>" 
                     data-status='<%# Eval("Status") %>' data-priority='<%# Eval("Priority") %>'>
                    
                    <!-- Task Header -->
                    <div class="task-header">
                        <div class="task-title-section">
                            <div class="task-status-icon">
                                <%# GetTaskStatusIcon(Eval("Status").ToString()) %>
                            </div>
                            <div class="task-info">
                                <h3 class="task-title"><%# Eval("Title") %></h3>
                                <div class="task-meta">
                                    <span class="task-category <%# GetCategoryClass(Eval("Category").ToString()) %>">
                                        <%# GetCategoryIcon(Eval("Category").ToString()) %>
                                        <%# GetCategoryDisplay(Eval("Category").ToString()) %>
                                    </span>
                                    <span class="task-priority <%# GetPriorityClass(Eval("Priority").ToString()) %>">
                                        <i class="material-icons">flag</i>
                                        <%# Eval("Priority") %> Priority
                                    </span>
                                    <span class="task-due-date <%# Convert.ToBoolean(Eval("IsOverdue")) ? "overdue" : "" %>">
                                        <i class="material-icons">schedule</i>
                                        Due: <%# Eval("DueDate", "{0:MMM dd, yyyy}") %>
                                    </span>
                                </div>
                            </div>
                        </div>
                        <div class="task-actions">
                            <%# Eval("Status").ToString() != "COMPLETED" && Convert.ToBoolean(Eval("CanEmployeeComplete")) ? 
                                "<button class='btn-task-complete' onclick='completeTask(" + Eval("TaskId") + ")'><i class='material-icons'>check</i>Mark Complete</button>" : "" %>
                            
                            <%# Eval("Status").ToString() == "COMPLETED" ? 
                                "<span class='completed-badge'><i class='material-icons'>check_circle</i>Completed</span>" : "" %>
                        </div>
                    </div>

                    <!-- Task Body -->
                    <div class="task-body">
                        <div class="task-description">
                            <p><%# Eval("Description") %></p>
                        </div>
                        
                        <%# !string.IsNullOrEmpty(Eval("Instructions").ToString()) ? 
                            "<div class='task-instructions'><h4><i class='material-icons'>info</i>Instructions:</h4><p>" + Eval("Instructions") + "</p></div>" : "" %>
                        
                        <div class="task-details">
                            <div class="detail-item">
                                <i class="material-icons">schedule</i>
                                <span>Estimated Time: <%# Eval("EstimatedTime") %></span>
                            </div>
                            
                            <%# !string.IsNullOrEmpty(Eval("AssignedToRole").ToString()) ? 
                                "<div class='detail-item'><i class='material-icons'>person</i><span>Assigned to: " + GetAssignedToDisplay(Eval("AssignedToRole").ToString()) + "</span></div>" : "" %>
                            
                            <%# Eval("Status").ToString() == "COMPLETED" && Eval("CompletedDate") != null ? 
                                "<div class='detail-item completed-detail'><i class='material-icons'>check_circle</i><span>Completed on: " + Convert.ToDateTime(Eval("CompletedDate")).ToString("MMM dd, yyyy") + "</span></div>" : "" %>
                        </div>

                        <%# !string.IsNullOrEmpty(Eval("Notes").ToString()) ? 
                            "<div class='task-notes'><h4><i class='material-icons'>notes</i>Notes:</h4><p>" + Eval("Notes") + "</p></div>" : "" %>
                    </div>

                    <!-- Task Footer (for non-self-complete tasks) -->
                    <%# Eval("Status").ToString() != "COMPLETED" && !Convert.ToBoolean(Eval("CanEmployeeComplete")) ? 
                        "<div class='task-footer'><div class='help-text'><i class='material-icons'>help_outline</i>This task will be completed by " + GetAssignedToDisplay(Eval("AssignedToRole").ToString()) + ". You'll be notified when it's done.</div></div>" : "" %>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <!-- Empty State -->
        <asp:Panel ID="pnlEmptyTasks" runat="server" CssClass="empty-state" Visible="false">
            <div class="empty-icon">
                <i class="material-icons">assignment_turned_in</i>
            </div>
            <h3>All Done! 🎉</h3>
            <p>You've completed all your onboarding tasks. Welcome to the team!</p>
            <a href="/Dashboard.aspx" class="btn btn-primary">
                <i class="material-icons">dashboard</i>
                Go to Dashboard
            </a>
        </asp:Panel>
    </div>

    <!-- Success Modal -->
    <div id="successModal" class="modal">
        <div class="modal-content">
            <div class="modal-header">
                <h3><i class="material-icons">check_circle</i>Task Completed!</h3>
            </div>
            <div class="modal-body">
                <p>Great job! You've successfully completed another onboarding task.</p>
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" onclick="closeModal()">Continue</button>
            </div>
        </div>
    </div>

    <!-- Hidden fields for progress data -->
    <asp:HiddenField ID="hidCompletedTasks" runat="server" Value="0" />
    <asp:HiddenField ID="hidTotalTasks" runat="server" Value="0" />
    <asp:HiddenField ID="hidCompletionPercentage" runat="server" Value="0" />

    <script>
        // Page Load
        document.addEventListener('DOMContentLoaded', function() {
            updateProgressRing();
            initializeFilters();
        });
        
        // Update Progress Ring
        function updateProgressRing() {
            const completedTasks = parseInt(document.getElementById('<%= hidCompletedTasks.ClientID %>').value) || 0;
            const totalTasks = parseInt(document.getElementById('<%= hidTotalTasks.ClientID %>').value) || 0;
            const percentage = parseFloat(document.getElementById('<%= hidCompletionPercentage.ClientID %>').value) || 0;

            // Update numbers
            document.getElementById('completedTasks').textContent = completedTasks;
            document.getElementById('totalTasks').textContent = totalTasks;
            document.getElementById('progressPercentage').textContent = Math.round(percentage) + '%';

            // Update progress ring
            const circle = document.getElementById('progressCircle');
            const radius = 52;
            const circumference = 2 * Math.PI * radius;
            const offset = circumference - (percentage / 100) * circumference;

            circle.style.strokeDasharray = circumference + ' ' + circumference;
            circle.style.strokeDashoffset = offset;
        }

        // Filter Functions
        function filterTasks(status) {
            const cards = document.querySelectorAll('.task-card');
            const buttons = document.querySelectorAll('.filter-btn[onclick*="filterTasks"]');

            // Update active button
            buttons.forEach(btn => btn.classList.remove('active'));
            event.target.classList.add('active');

            cards.forEach(card => {
                if (status === 'all') {
                    card.style.display = 'block';
                } else if (status === 'pending') {
                    card.style.display = card.dataset.status !== 'COMPLETED' ? 'block' : 'none';
                } else if (status === 'completed') {
                    card.style.display = card.dataset.status === 'COMPLETED' ? 'block' : 'none';
                }
            });
        }

        function filterByPriority(priority) {
            const cards = document.querySelectorAll('.task-card');
            const buttons = document.querySelectorAll('.filter-btn[onclick*="filterByPriority"]');

            // Update active button
            buttons.forEach(btn => btn.classList.remove('active'));
            event.target.classList.add('active');

            cards.forEach(card => {
                if (priority === 'all') {
                    card.style.display = 'block';
                } else {
                    card.style.display = card.dataset.priority === priority ? 'block' : 'none';
                }
            });
        }

        function initializeFilters() {
            // Set first filter button as active
            const firstBtn = document.querySelector('.filter-btn');
            if (firstBtn) firstBtn.classList.add('active');
        }

        // Complete Task
        function completeTask(taskId) {
            if (confirm('Are you sure you want to mark this task as complete?')) {
                __doPostBack('<%= rptTasks.UniqueID %>', 'COMPLETE_TASK:' + taskId);
            }
        }

        // Modal Functions
        function showModal() {
            document.getElementById('successModal').style.display = 'block';
        }

        function closeModal() {
            document.getElementById('successModal').style.display = 'none';
            location.reload(); // Refresh to show updated progress
        }

        // Close modal on outside click
        window.onclick = function (event) {
            const modal = document.getElementById('successModal');
            if (event.target === modal) {
                closeModal();
            }
        }
    </script>
</asp:Content>