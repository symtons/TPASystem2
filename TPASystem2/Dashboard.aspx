<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="TPASystem2.Dashboard" %>

<asp:Content ID="DashboardContent" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- Page Header -->
    <div class="page-header">
        <h1 class="page-title">Dashboard</h1>
        <p class="page-subtitle">Welcome back! Here's what's happening at TPA today.</p>
    </div>

    <!-- Dashboard Stats -->
    <div class="stats-grid">
        <asp:Literal ID="litDashboardStats" runat="server"></asp:Literal>
    </div>

    <!-- Main Dashboard Grid -->
    <div class="dashboard-grid">
        <!-- Left Column - Quick Actions -->
        <div class="dashboard-card">
            <div class="card-header">
                <div>
                    <h3 class="card-title">Quick Actions</h3>
                    <p class="card-subtitle">Frequently used tools and shortcuts</p>
                </div>
            </div>
            <div class="card-content">
                <div class="quick-actions-grid">
                    <asp:Literal ID="litQuickActions" runat="server"></asp:Literal>
                </div>
            </div>
        </div>

        <!-- Right Column - Recent Activities -->
        <div class="dashboard-card">
            <div class="card-header">
                <div>
                    <h3 class="card-title">Recent Activities</h3>
                    <p class="card-subtitle">Latest system activities</p>
                </div>
                <div class="card-actions">
                    <a href="/activities" class="view-all-link">View All</a>
                </div>
            </div>
            <div class="card-content">
                <ul class="activity-list">
                    <asp:Literal ID="litRecentActivities" runat="server"></asp:Literal>
                </ul>
            </div>
        </div>
    </div>

    <!-- Hidden user info for JavaScript -->
    <div style="display: none;">
        <asp:Literal ID="litUserName" runat="server"></asp:Literal>
        <asp:Literal ID="litUserInitial" runat="server"></asp:Literal>
        <asp:Literal ID="litHeaderUserInitial" runat="server"></asp:Literal>
        <asp:Literal ID="litUserRole" runat="server"></asp:Literal>
        <asp:Literal ID="litNavigation" runat="server"></asp:Literal>
    </div>

    <!-- Dashboard JavaScript -->
    <script type="text/javascript">
        // Dashboard JavaScript functionality
        function handleQuickAction(actionKey) {
            console.log('Quick action clicked:', actionKey);

            // Handle different action types based on your enhanced role system
            switch (actionKey) {
                // Employee actions
                case 'clock-in':
                    window.location.href = '/time-attendance';
                    break;
                case 'request-leave':
                    window.location.href = '/leave-management';
                    break;
                case 'view-profile':
                    window.location.href = '/profile';
                    break;
                case 'my-documents':
                    window.location.href = '/documents';
                    break;

                // Admin actions
                case 'add-employee':
                    window.location.href = '/employees/add';
                    break;
                case 'generate-report':
                    window.location.href = '/reports';
                    break;
                case 'review-approvals':
                    window.location.href = '/approvals';
                    break;
                case 'system-health':
                    window.location.href = '/admin/health';
                    break;

                // HR Admin actions
                case 'leave-approvals':
                    window.location.href = '/leave/approvals';
                    break;
                case 'hr-reports':
                    window.location.href = '/reports/hr';
                    break;
                case 'performance-reviews':
                    window.location.href = '/hr/performance';
                    break;

                // Program Director actions
                case 'program-overview':
                    window.location.href = '/programs';
                    break;
                case 'strategic-planning':
                    window.location.href = '/director/planning';
                    break;
                case 'budget-review':
                    window.location.href = '/director/budget';
                    break;
                case 'performance-analytics':
                    window.location.href = '/director/analytics';
                    break;

                // Program Coordinator actions
                case 'schedule-meeting':
                    window.location.href = '/coordination/meeting';
                    break;
                case 'task-assignment':
                    window.location.href = '/coordination/tasks';
                    break;
                case 'resource-planning':
                    window.location.href = '/coordinator/resources';
                    break;
                case 'team-reports':
                    window.location.href = '/reports/team';
                    break;

                // Super Admin actions
                case 'system-monitor':
                    window.location.href = '/admin/monitor';
                    break;
                case 'user-management':
                    window.location.href = '/admin/users';
                    break;
                case 'database-backup':
                    window.location.href = '/admin/backup';
                    break;
                case 'system-settings':
                    window.location.href = '/admin/settings';
                    break;

                default:
                    console.log('Unknown action:', actionKey);
                    // Fallback - try to navigate to the action as a URL
                    if (actionKey.startsWith('/')) {
                        window.location.href = actionKey;
                    }
            }
        }

        // Initialize dashboard when page loads
        document.addEventListener('DOMContentLoaded', function () {
            // Animate stat cards on load
            const statCards = document.querySelectorAll('.stat-card');
            statCards.forEach((card, index) => {
                card.style.opacity = '0';
                card.style.transform = 'translateY(20px)';

                setTimeout(() => {
                    card.style.transition = 'all 0.3s ease';
                    card.style.opacity = '1';
                    card.style.transform = 'translateY(0)';
                }, index * 100);
            });

            // Animate quick actions
            const quickActions = document.querySelectorAll('.quick-action');
            quickActions.forEach((action, index) => {
                action.style.opacity = '0';
                action.style.transform = 'translateY(20px)';

                setTimeout(() => {
                    action.style.transition = 'all 0.3s ease';
                    action.style.opacity = '1';
                    action.style.transform = 'translateY(0)';
                }, 300 + (index * 50));
            });

            // Animate activity items
            const activityItems = document.querySelectorAll('.activity-item');
            activityItems.forEach((item, index) => {
                item.style.opacity = '0';
                item.style.transform = 'translateX(20px)';

                setTimeout(() => {
                    item.style.transition = 'all 0.3s ease';
                    item.style.opacity = '1';
                    item.style.transform = 'translateX(0)';
                }, 500 + (index * 50));
            });

            // Add click handlers for stat cards
            statCards.forEach(card => {
                card.addEventListener('click', function () {
                    const label = this.querySelector('.stat-label')?.textContent;
                    if (label) {
                        console.log('Stat card clicked:', label);
                        // Navigate to detailed views based on stat type
                        switch (label.toLowerCase()) {
                            case 'total employees':
                            case 'active employees':
                                window.location.href = '/employees';
                                break;
                            case 'leave requests':
                            case 'pending approvals':
                                window.location.href = '/leave/approvals';
                                break;
                            case 'my hours':
                            case 'active sessions':
                                window.location.href = '/time-attendance';
                                break;
                            case 'reports generated':
                            case 'performance reviews':
                                window.location.href = '/reports';
                                break;
                            default:
                                // Generic fallback
                                break;
                        }
                    }
                });
            });

            console.log('✅ Dashboard initialized successfully');
        });

        // Refresh dashboard data (for future AJAX implementation)
        function refreshDashboard() {
            console.log('🔄 Refreshing dashboard...');
            // For now, just reload the page
            window.location.reload();
        }

        // Auto-refresh dashboard every 5 minutes (optional)
        setInterval(function () {
            console.log('🔄 Auto-refreshing dashboard data...');
            // Implement AJAX refresh here in the future
        }, 300000); // 5 minutes

        // Export functions for global access
        window.handleQuickAction = handleQuickAction;
        window.refreshDashboard = refreshDashboard;

        // Menu navigation functions
        function toggleSubmenu(element) {
            const submenu = element.nextElementSibling;
            const arrow = element.querySelector('.nav-arrow');

            if (submenu.style.display === 'none' || submenu.style.display === '') {
                submenu.style.display = 'block';
                arrow.textContent = 'expand_less';
                element.parentElement.classList.add('expanded');
            } else {
                submenu.style.display = 'none';
                arrow.textContent = 'expand_more';
                element.parentElement.classList.remove('expanded');
            }
        }

        // Export menu functions
        window.toggleSubmenu = toggleSubmenu;
    </script>
</asp:Content>