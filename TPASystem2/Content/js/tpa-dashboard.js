// ===============================================
// TPA HR System - Dashboard JavaScript
// File: Content/js/tpa-dashboard.js
// ===============================================

// Dashboard namespace
window.TPA = window.TPA || {};
TPA.Dashboard = TPA.Dashboard || {};

// ===============================================
// Dashboard Initialization
// ===============================================
document.addEventListener('DOMContentLoaded', function () {
    TPA.Dashboard.init();
});

TPA.Dashboard.init = function () {
    console.log('🎛️ Initializing dashboard...');

    // Initialize components
    this.initializeSidebar();
    this.initializeNavigation();
    this.initializeDropdowns();
    this.initializeNotifications();
    this.initializeQuickActions();
    this.initializeDataRefresh();
    this.initializeMobileSupport();

    console.log('✅ Dashboard initialized successfully');
};

// ===============================================
// Sidebar Management
// ===============================================
TPA.Dashboard.initializeSidebar = function () {
    const sidebar = document.getElementById('sidebar');
    if (!sidebar) return;

    // Set active navigation item based on current page
    this.setActiveNavItem();

    // Handle navigation clicks
    const navLinks = sidebar.querySelectorAll('.nav-link');
    navLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            // Don't prevent default for actual navigation
            // Just add visual feedback
            const navItem = this.closest('.nav-item');
            if (navItem) {
                // Remove active from all items
                sidebar.querySelectorAll('.nav-item').forEach(item => {
                    item.classList.remove('active');
                });
                // Add active to clicked item
                navItem.classList.add('active');
            }
        });
    });
};

TPA.Dashboard.setActiveNavItem = function () {
    const currentPath = window.location.pathname.toLowerCase();
    const navItems = document.querySelectorAll('.nav-item');

    navItems.forEach(item => {
        const link = item.querySelector('.nav-link');
        if (link) {
            const linkPath = link.getAttribute('href');
            if (linkPath === currentPath || (currentPath === '/' && linkPath === '/dashboard')) {
                item.classList.add('active');
            } else {
                item.classList.remove('active');
            }
        }
    });
};

// ===============================================
// Navigation & Dropdown Management
// ===============================================
TPA.Dashboard.initializeNavigation = function () {
    // Initialize breadcrumb navigation
    this.initializeBreadcrumbs();
};

TPA.Dashboard.initializeBreadcrumbs = function () {
    const breadcrumbItems = document.querySelectorAll('.breadcrumb-item');
    breadcrumbItems.forEach(item => {
        if (item.tagName === 'A') {
            item.addEventListener('click', function (e) {
                // Add loading state for breadcrumb navigation
                showLoading(this, 'Loading...');
            });
        }
    });
};

TPA.Dashboard.initializeDropdowns = function () {
    // Initialize Materialize dropdowns
    const dropdowns = document.querySelectorAll('.dropdown-trigger');
    if (dropdowns.length > 0) {
        M.Dropdown.init(dropdowns, {
            alignment: 'right',
            constrainWidth: false,
            coverTrigger: false,
            closeOnClick: false,
            hover: false
        });
    }

    // Handle notification dropdown items
    const notificationItems = document.querySelectorAll('.notification-item a');
    notificationItems.forEach(item => {
        item.addEventListener('click', function (e) {
            e.preventDefault();
            // Mark notification as read
            TPA.Dashboard.markNotificationAsRead(this);
        });
    });

    // Handle mark all as read
    const markAllReadBtn = document.querySelector('.mark-all-read');
    if (markAllReadBtn) {
        markAllReadBtn.addEventListener('click', function (e) {
            e.preventDefault();
            TPA.Dashboard.markAllNotificationsAsRead();
        });
    }
};

// ===============================================
// Notification Management
// ===============================================
TPA.Dashboard.initializeNotifications = function () {
    // Check for new notifications periodically
    setInterval(() => {
        this.checkForNewNotifications();
    }, 30000); // Every 30 seconds

    // Handle notification badge clicks
    const notificationBtn = document.querySelector('.notification-btn');
    if (notificationBtn) {
        notificationBtn.addEventListener('click', function () {
            TPA.Dashboard.loadNotifications();
        });
    }
};

TPA.Dashboard.checkForNewNotifications = function () {
    // Only check if user is on the page (not in background)
    if (document.hidden) return;

    fetch('/api/notifications/count', {
        method: 'GET',
        credentials: 'include'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                this.updateNotificationBadge(data.count);
            }
        })
        .catch(error => {
            console.log('Failed to check notifications:', error);
        });
};

TPA.Dashboard.updateNotificationBadge = function (count) {
    const badge = document.querySelector('.notification-badge');
    const badgeText = document.getElementById('litNotificationCount');

    if (badge && badgeText) {
        badgeText.textContent = count;
        badge.style.display = count > 0 ? 'flex' : 'none';
    }
};

TPA.Dashboard.loadNotifications = function () {
    fetch('/api/notifications/recent', {
        method: 'GET',
        credentials: 'include'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                this.updateNotificationDropdown(data.notifications);
            }
        })
        .catch(error => {
            console.log('Failed to load notifications:', error);
        });
};

TPA.Dashboard.updateNotificationDropdown = function (notifications) {
    const container = document.querySelector('#notificationDropdown');
    if (!container) return;

    const notificationsHtml = notifications.map(notification => {
        return `
            <li class="notification-item">
                <a href="#" data-id="${notification.id}">
                    <div class="notification-icon ${notification.type}">
                        <i class="material-icons">${this.getNotificationIcon(notification.type)}</i>
                    </div>
                    <div class="notification-content">
                        <div class="notification-title">${notification.title}</div>
                        <div class="notification-message">${notification.message}</div>
                        <div class="notification-time">${timeAgo(notification.createdAt)}</div>
                    </div>
                </a>
            </li>
        `;
    }).join('');

    // Update the dropdown content (excluding header and footer)
    const existingItems = container.querySelectorAll('.notification-item');
    existingItems.forEach(item => item.remove());

    const divider = container.querySelector('.divider');
    if (divider) {
        divider.insertAdjacentHTML('beforebegin', notificationsHtml);
    }
};

TPA.Dashboard.getNotificationIcon = function (type) {
    const icons = {
        success: 'check_circle',
        warning: 'warning',
        error: 'error',
        info: 'info'
    };
    return icons[type] || icons.info;
};

TPA.Dashboard.markNotificationAsRead = function (notificationElement) {
    const notificationId = notificationElement.getAttribute('data-id');
    if (!notificationId) return;

    fetch(`/api/notifications/${notificationId}/read`, {
        method: 'POST',
        credentials: 'include'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Update UI
                notificationElement.closest('.notification-item').style.opacity = '0.6';
                this.updateNotificationBadge(data.unreadCount || 0);
            }
        })
        .catch(error => {
            console.log('Failed to mark notification as read:', error);
        });
};

TPA.Dashboard.markAllNotificationsAsRead = function () {
    fetch('/api/notifications/mark-all-read', {
        method: 'POST',
        credentials: 'include'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Update UI
                const notificationItems = document.querySelectorAll('.notification-item');
                notificationItems.forEach(item => {
                    item.style.opacity = '0.6';
                });
                this.updateNotificationBadge(0);
                showNotification('All notifications marked as read', 'success', 2000);
            }
        })
        .catch(error => {
            console.log('Failed to mark all notifications as read:', error);
        });
};

// ===============================================
// Quick Actions
// ===============================================
TPA.Dashboard.initializeQuickActions = function () {
    const quickActions = document.querySelectorAll('.quick-action');
    quickActions.forEach(action => {
        action.addEventListener('click', function (e) {
            e.preventDefault();
            const actionKey = this.getAttribute('onclick')?.match(/handleQuickAction\(['"]([^'"]+)['"]\)/)?.[1];
            if (actionKey) {
                TPA.Dashboard.handleQuickAction(actionKey);
            } else {
                // If no onclick, try to get href
                const href = this.getAttribute('href');
                if (href && href !== '#') {
                    window.location.href = href;
                }
            }
        });
    });
};

TPA.Dashboard.handleQuickAction = function (actionKey) {
    console.log('🚀 Quick action triggered:', actionKey);

    switch (actionKey) {
        case 'clock-in':
        case 'clock-out':
            this.handleTimeClockAction(actionKey);
            break;
        case 'submit-leave':
            window.location.href = '/leave-management';
            break;
        case 'view-employees':
            window.location.href = '/employees';
            break;
        case 'generate-report':
            window.location.href = '/reports';
            break;
        case 'view-profile':
            window.location.href = '/profile';
            break;
        case 'system-settings':
            window.location.href = '/settings';
            break;
        case 'get-help':
            window.location.href = '/help';
            break;
        case 'review-requests':
            window.location.href = '/requests';
            break;
        default:
            console.log('Unknown quick action:', actionKey);
            showNotification('Action not implemented yet', 'info');
    }
};

TPA.Dashboard.handleTimeClockAction = function (action) {
    const isClockIn = action === 'clock-in';
    const actionText = isClockIn ? 'Clock In' : 'Clock Out';

    if (confirm(`Are you sure you want to ${actionText.toLowerCase()}?`)) {
        fetch('/api/time-attendance/clock', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            credentials: 'include',
            body: JSON.stringify({
                action: action,
                timestamp: new Date().toISOString(),
                location: 'Web Dashboard'
            })
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    showNotification(`${actionText} successful`, 'success');
                    // Refresh dashboard stats
                    this.refreshDashboardStats();
                } else {
                    showNotification(data.message || `${actionText} failed`, 'error');
                }
            })
            .catch(error => {
                console.error('Time clock error:', error);
                showNotification(`Error during ${actionText.toLowerCase()}`, 'error');
            });
    }
};

// ===============================================
// Data Refresh
// ===============================================
TPA.Dashboard.initializeDataRefresh = function () {
    // Auto-refresh dashboard data every 5 minutes
    setInterval(() => {
        this.refreshDashboardData();
    }, 5 * 60 * 1000);

    // Manual refresh button
    const refreshBtns = document.querySelectorAll('[data-action="refresh"]');
    refreshBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            TPA.Dashboard.refreshDashboardData();
        });
    });
};

TPA.Dashboard.refreshDashboardData = function () {
    console.log('🔄 Refreshing dashboard data...');

    // Only refresh if user is on the page
    if (document.hidden) return;

    Promise.all([
        this.refreshDashboardStats(),
        this.refreshRecentActivities(),
        this.checkForNewNotifications()
    ]).then(() => {
        console.log('✅ Dashboard data refreshed');
        showNotification('Dashboard updated', 'success', 2000);
    }).catch(error => {
        console.error('Failed to refresh dashboard:', error);
    });
};

TPA.Dashboard.refreshDashboardStats = function () {
    return fetch('/api/dashboard/stats/refresh', {
        method: 'GET',
        credentials: 'include'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success && data.stats) {
                this.updateStatsCards(data.stats);
            }
        })
        .catch(error => {
            console.log('Failed to refresh stats:', error);
        });
};

TPA.Dashboard.updateStatsCards = function (stats) {
    stats.forEach(stat => {
        const card = document.querySelector(`[data-stat-key="${stat.key}"]`);
        if (card) {
            const valueElement = card.querySelector('.stat-value');
            const changeElement = card.querySelector('.stat-change');

            if (valueElement) {
                valueElement.textContent = stat.value;
            }

            if (changeElement && stat.change) {
                changeElement.textContent = stat.change;
                changeElement.className = `stat-change ${stat.changeType || 'positive'}`;
            }
        }
    });
};

TPA.Dashboard.refreshRecentActivities = function () {
    return fetch('/api/dashboard/recent-activities/refresh', {
        method: 'GET',
        credentials: 'include'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success && data.activities) {
                this.updateRecentActivities(data.activities);
            }
        })
        .catch(error => {
            console.log('Failed to refresh activities:', error);
        });
};

TPA.Dashboard.updateRecentActivities = function (activities) {
    const container = document.querySelector('.activity-list');
    if (!container) return;

    const activitiesHtml = activities.map(activity => {
        return `
            <li class="activity-item">
                <div class="activity-avatar" style="background: ${activity.color};">
                    ${activity.userInitials}
                </div>
                <div class="activity-content">
                    <div class="activity-header">
                        <span class="activity-user">${activity.user}</span>
                        <span class="activity-action">${activity.action}</span>
                        ${activity.isNew ? '<span class="activity-new">NEW</span>' : ''}
                    </div>
                    <div class="activity-details">${activity.details}</div>
                    <div class="activity-time">${timeAgo(activity.time)}</div>
                </div>
            </li>
        `;
    }).join('');

    container.innerHTML = activitiesHtml;
};

// ===============================================
// Mobile Support
// ===============================================
TPA.Dashboard.initializeMobileSupport = function () {
    // Mobile menu toggle
    window.toggleSidebar = function () {
        const sidebar = document.getElementById('sidebar');
        const overlay = document.querySelector('.mobile-overlay');

        if (sidebar) {
            sidebar.classList.toggle('show');
            if (overlay) {
                overlay.style.display = sidebar.classList.contains('show') ? 'block' : 'none';
            }
        }
    };

    // Close sidebar
    window.closeSidebar = function () {
        const sidebar = document.getElementById('sidebar');
        const overlay = document.querySelector('.mobile-overlay');

        if (sidebar) {
            sidebar.classList.remove('show');
            if (overlay) {
                overlay.style.display = 'none';
            }
        }
    };

    // Handle window resize
    window.addEventListener('resize', TPA.Utils.debounce(function () {
        if (window.innerWidth > 768) {
            closeSidebar();
        }
    }, 250));

    // Handle orientation change
    window.addEventListener('orientationchange', function () {
        setTimeout(closeSidebar, 300);
    });
};

// ===============================================
// Global Dashboard Functions
// ===============================================

// Make refresh function globally available
window.refreshDashboardData = function () {
    TPA.Dashboard.refreshDashboardData();
};

// Handle quick actions globally
window.handleQuickAction = function (actionKey) {
    TPA.Dashboard.handleQuickAction(actionKey);
};

// Toggle user menu (for compatibility)
window.toggleUserMenu = function () {
    const dropdown = M.Dropdown.getInstance(document.querySelector('.user-menu'));
    if (dropdown) {
        dropdown.open();
    }
};

// Page visibility handling
document.addEventListener('visibilitychange', function () {
    if (!document.hidden) {
        // Page became visible, refresh data
        setTimeout(() => {
            TPA.Dashboard.checkForNewNotifications();
        }, 1000);
    }
});

console.log('✅ TPA Dashboard JavaScript loaded successfully');