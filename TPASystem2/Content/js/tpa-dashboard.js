//// =============================================================================
//// TPA Dashboard - Complete Enhanced Real-time Dashboard System
//// File: tpa-dashboard.js
//// =============================================================================

//// Initialize TPA namespace
//var TPA = TPA || {};
//TPA.Dashboard = TPA.Dashboard || {};

//// Dashboard configuration
//TPA.Dashboard.config = {
//    refreshInterval: 30000, // 30 seconds
//    isAutoRefreshEnabled: true,
//    isVisible: true,
//    connectionRetries: 0,
//    maxRetries: 3,
//    animationDuration: 600,
//    enableSounds: false,
//    theme: 'auto' // auto, light, dark
//};

//// =============================================================================
//// INITIALIZATION
//// =============================================================================

//// Initialize enhanced dashboard functionality
//TPA.Dashboard.init = function () {
//    console.log('🚀 Initializing TPA Dashboard System...');

//    // Check if we're on the dashboard page
//    if (!this.isDashboardPage()) {
//        console.log('Not on dashboard page, skipping initialization');
//        return;
//    }

//    try {
//        // Add enhanced styles
//        this.addEnhancedStyles();

//        // Set up core functionality
//        this.setupVisibilityDetection();
//        this.setupRefreshControls();
//        this.setupKeyboardShortcuts();
//        this.setupThemeDetection();

//        // Initialize stat cards
//        this.initializeStatCards();

//        // Setup auto-refresh
//        if (this.config.isAutoRefreshEnabled) {
//            this.startAutoRefresh();
//        }

//        // Initial load
//        this.refreshDashboardData();

//        // Setup periodic cleanup
//        this.setupPeriodicCleanup();

//        console.log('✅ TPA Dashboard initialized successfully');
//        this.showNotification('Dashboard ready', 'success', 2000);

//    } catch (error) {
//        console.error('❌ Dashboard initialization failed:', error);
//        this.showNotification('Dashboard initialization failed', 'error', 5000);
//    }
//};

//// Check if current page is dashboard
//TPA.Dashboard.isDashboardPage = function () {
//    return document.querySelector('.dashboard-container, [data-page="dashboard"], .stat-card') !== null;
//};

//// =============================================================================
//// CORE FUNCTIONALITY
//// =============================================================================

//// Setup page visibility detection
//TPA.Dashboard.setupVisibilityDetection = function () {
//    document.addEventListener('visibilitychange', () => {
//        this.config.isVisible = !document.hidden;

//        if (this.config.isVisible) {
//            console.log('👁 Dashboard visible - resuming updates');
//            this.refreshDashboardData();
//            if (this.config.isAutoRefreshEnabled) {
//                this.startAutoRefresh();
//            }
//        } else {
//            console.log('🙈 Dashboard hidden - pausing updates');
//            this.stopAutoRefresh();
//        }
//    });
//};

//// Setup refresh controls
//TPA.Dashboard.setupRefreshControls = function () {
//    const refreshBtns = document.querySelectorAll('[data-action="refresh"], .refresh-btn, .btn-refresh');
//    refreshBtns.forEach(btn => {
//        btn.addEventListener('click', (e) => {
//            e.preventDefault();
//            this.manualRefresh();
//        });
//    });

//    // Auto-refresh toggle buttons
//    const toggleBtns = document.querySelectorAll('[data-action="toggle-refresh"]');
//    toggleBtns.forEach(btn => {
//        btn.addEventListener('click', (e) => {
//            e.preventDefault();
//            this.toggleAutoRefresh();
//        });
//    });
//};

//// Setup keyboard shortcuts
//TPA.Dashboard.setupKeyboardShortcuts = function () {
//    document.addEventListener('keydown', (e) => {
//        // Only handle shortcuts when not in input fields
//        if (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA') {
//            return;
//        }

//        switch (true) {
//            // Ctrl+R or F5 for refresh
//            case (e.ctrlKey && e.key === 'r') || e.key === 'F5':
//                e.preventDefault();
//                this.manualRefresh();
//                break;

//            // Ctrl+D for dashboard focus
//            case e.ctrlKey && e.key === 'd':
//                e.preventDefault();
//                this.focusDashboard();
//                break;

//            // Space to toggle auto-refresh
//            case e.code === 'Space' && e.target === document.body:
//                e.preventDefault();
//                this.toggleAutoRefresh();
//                break;

//            // ESC to clear notifications
//            case e.key === 'Escape':
//                this.clearAllNotifications();
//                break;
//        }
//    });
//};

//// Setup theme detection
//TPA.Dashboard.setupThemeDetection = function () {
//    if (this.config.theme === 'auto') {
//        // Listen for system theme changes
//        const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
//        mediaQuery.addListener((e) => {
//            this.applyTheme(e.matches ? 'dark' : 'light');
//        });

//        // Apply initial theme
//        this.applyTheme(mediaQuery.matches ? 'dark' : 'light');
//    }
//};

//// Apply theme
//TPA.Dashboard.applyTheme = function (theme) {
//    document.body.classList.remove('tpa-theme-light', 'tpa-theme-dark');
//    document.body.classList.add(`tpa-theme-${theme}`);
//    console.log(`🎨 Applied theme: ${theme}`);
//};

//// =============================================================================
//// STAT CARDS MANAGEMENT
//// =============================================================================

//// Initialize stat cards with enhanced interactions
//TPA.Dashboard.initializeStatCards = function () {
//    const statCards = document.querySelectorAll('.stat-card');

//    statCards.forEach((card, index) => {
//        // Add accessibility
//        card.setAttribute('tabindex', '0');
//        card.setAttribute('role', 'button');

//        // Add click handlers
//        card.addEventListener('click', (e) => {
//            this.handleStatCardClick(e, card);
//        });

//        // Add keyboard support
//        card.addEventListener('keydown', (e) => {
//            if (e.key === 'Enter' || e.key === ' ') {
//                e.preventDefault();
//                this.handleStatCardClick(e, card);
//            }
//        });

//        // Add entrance animation with stagger
//        this.animateCardEntrance(card, index);
//    });

//    console.log(`📊 Initialized ${statCards.length} stat cards`);
//};

//// Handle stat card click
//TPA.Dashboard.handleStatCardClick = function (event, card) {
//    // Create ripple effect
//    this.createClickRipple(event, card);

//    // Get stat key and trigger action
//    const statKey = card.getAttribute('data-stat-key');
//    if (statKey) {
//        this.handleStatAction(statKey, card);
//    }
//};

//// Handle stat-specific actions
//TPA.Dashboard.handleStatAction = function (statKey, card) {
//    console.log(`📈 Stat card clicked: ${statKey}`);

//    // Define actions for different stat types
//    const actions = {
//        'total_users': () => window.location.href = '/admin/users',
//        'pending_approvals': () => window.location.href = '/approvals',
//        'hr_pending_leave': () => window.location.href = '/leave/requests',
//        'emp_pending_requests': () => window.location.href = '/leave/my-requests',
//        'mgr_direct_reports': () => window.location.href = '/team/overview',
//        // Add more actions as needed
//    };

//    const action = actions[statKey];
//    if (action) {
//        action();
//    } else {
//        // Default action - show detail modal or navigate
//        this.showStatDetails(statKey, card);
//    }
//};

//// Show stat details (placeholder for future enhancement)
//TPA.Dashboard.showStatDetails = function (statKey, card) {
//    const statTitle = card.querySelector('.stat-title')?.textContent || 'Statistic';
//    const statValue = card.querySelector('.stat-value')?.textContent || '0';

//    this.showNotification(`${statTitle}: ${statValue}`, 'info', 3000);
//};

//// Animate card entrance
//TPA.Dashboard.animateCardEntrance = function (card, index) {
//    card.style.opacity = '0';
//    card.style.transform = 'translateY(30px) scale(0.9)';

//    setTimeout(() => {
//        card.style.transition = 'all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275)';
//        card.style.opacity = '1';
//        card.style.transform = 'translateY(0) scale(1)';
//    }, index * 100 + 200); // Stagger animations
//};

//// Create click ripple effect
//TPA.Dashboard.createClickRipple = function (event, element) {
//    const ripple = document.createElement('div');
//    const rect = element.getBoundingClientRect();
//    const size = Math.max(rect.width, rect.height);
//    const x = event.clientX - rect.left - size / 2;
//    const y = event.clientY - rect.top - size / 2;

//    ripple.className = 'click-ripple';
//    ripple.style.cssText = `
//        position: absolute;
//        width: ${size}px;
//        height: ${size}px;
//        left: ${x}px;
//        top: ${y}px;
//        background: var(--stat-color, #3498db);
//        border-radius: 50%;
//        opacity: 0.2;
//        transform: scale(0);
//        animation: clickRipple 0.6s ease-out;
//        pointer-events: none;
//        z-index: 1;
//    `;

//    element.style.position = 'relative';
//    element.appendChild(ripple);

//    setTimeout(() => {
//        if (ripple.parentNode) {
//            ripple.parentNode.removeChild(ripple);
//        }
//    }, 600);
//};

//// =============================================================================
//// REFRESH AND UPDATE FUNCTIONALITY
//// =============================================================================

//// Manual refresh triggered by user
//TPA.Dashboard.manualRefresh = function () {
//    console.log('🔄 Manual refresh triggered');
//    this.setRefreshState(true);

//    this.performanceMonitor.start();

//    Promise.all([
//        this.refreshDashboardStats(),
//        this.refreshRecentActivities()
//    ]).then(() => {
//        this.setRefreshState(false);
//        this.performanceMonitor.end('Manual refresh');
//        this.showNotification('Dashboard refreshed', 'success', 2000);
//    }).catch(error => {
//        this.setRefreshState(false);
//        this.performanceMonitor.end('Manual refresh (failed)');
//        this.showNotification('Refresh failed', 'error', 3000);
//        console.error('Manual refresh failed:', error);
//    });
//};

//// Refresh all dashboard data
//TPA.Dashboard.refreshDashboardData = function () {
//    if (!this.config.isVisible) {
//        console.log('Page not visible, skipping refresh');
//        return Promise.resolve();
//    }

//    console.log('⏰ Refreshing dashboard data...');

//    return Promise.all([
//        this.refreshDashboardStats(),
//        this.refreshRecentActivities(),
//        this.checkForNewNotifications()
//    ]).then(() => {
//        console.log('✅ Dashboard data refreshed successfully');
//    }).catch(error => {
//        console.error('❌ Dashboard refresh failed:', error);
//        this.handleConnectionError();
//    });
//};

//// Refresh dashboard statistics
//TPA.Dashboard.refreshDashboardStats = function () {
//    return new Promise((resolve, reject) => {
//        this.setStatsLoadingState(true);

//        fetch(window.location.pathname, {
//            method: 'POST',
//            headers: {
//                'Content-Type': 'application/x-www-form-urlencoded',
//                'X-Requested-With': 'XMLHttpRequest'
//            },
//            body: '__EVENTTARGET=' + encodeURIComponent(document.forms[0]?.id || 'form1') + '&__EVENTARGUMENT=RefreshStats',
//            credentials: 'same-origin'
//        })
//            .then(response => {
//                if (!response.ok) {
//                    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
//                }
//                return response.text();
//            })
//            .then(html => {
//                this.updateStatsFromHtml(html);
//                this.setStatsLoadingState(false);
//                this.config.connectionRetries = 0;
//                this.hideConnectionError();
//                resolve();
//            })
//            .catch(error => {
//                console.error('Failed to refresh stats:', error);
//                this.setStatsLoadingState(false);
//                this.handleConnectionError();
//                reject(error);
//            });
//    });
//};

//// Update stats from HTML response
//TPA.Dashboard.updateStatsFromHtml = function (html) {
//    try {
//        const tempDiv = document.createElement('div');
//        tempDiv.innerHTML = html;

//        // Look for the dashboard stats content
//        const newStatsContainer = tempDiv.querySelector('#dashboard-stats-content');
//        const currentStatsContainer = document.querySelector('#litDashboardStats, [data-dashboard-stats]');

//        if (newStatsContainer && currentStatsContainer) {
//            // Replace entire stats container content
//            currentStatsContainer.innerHTML = newStatsContainer.innerHTML;

//            // Re-initialize the new stat cards
//            this.initializeStatCards();

//            console.log('📊 Dashboard stats updated successfully');
//        } else {
//            // Fallback: try to update individual cards
//            const newStatCards = tempDiv.querySelectorAll('.stat-card');

//            newStatCards.forEach(newCard => {
//                const statKey = newCard.getAttribute('data-stat-key');
//                if (statKey) {
//                    const currentCard = document.querySelector(`[data-stat-key="${statKey}"]`);
//                    if (currentCard) {
//                        this.updateStatCard(currentCard, newCard);
//                    }
//                }
//            });

//            console.log(`📊 Updated ${newStatCards.length} stat cards`);
//        }
//    } catch (error) {
//        console.error('Error updating stats from HTML:', error);
//        this.showNotification('Failed to update dashboard stats', 'error', 3000);
//    }
//};

//// Update activities from HTML response
//TPA.Dashboard.updateActivitiesFromHtml = function (html) {
//    try {
//        const tempDiv = document.createElement('div');
//        tempDiv.innerHTML = html;

//        const newActivitiesContainer = tempDiv.querySelector('#recent-activities-content');
//        const currentActivitiesContainer = document.querySelector('#litRecentActivities, [data-recent-activities]');

//        if (newActivitiesContainer && currentActivitiesContainer) {
//            const newContent = newActivitiesContainer.innerHTML;
//            const currentContent = currentActivitiesContainer.innerHTML;

//            if (newContent !== currentContent) {
//                // Fade out, update, fade in
//                currentActivitiesContainer.style.opacity = '0.5';
//                setTimeout(() => {
//                    currentActivitiesContainer.innerHTML = newContent;
//                    currentActivitiesContainer.style.opacity = '1';
//                }, 200);

//                console.log('📝 Updated recent activities');
//            }
//        }
//    } catch (error) {
//        console.error('Error updating activities from HTML:', error);
//        this.showNotification('Failed to update activities', 'error', 3000);
//    }
//};

//// Improved refresh dashboard statistics with better error handling
//TPA.Dashboard.refreshDashboardStats = function () {
//    return new Promise((resolve, reject) => {
//        // Don't try to refresh if no form exists
//        const form = document.forms[0];
//        if (!form) {
//            console.warn('No form found for AJAX request, skipping stats refresh');
//            resolve();
//            return;
//        }

//        this.setStatsLoadingState(true);

//        // Create form data for the request
//        const formData = new FormData();
//        formData.append('__EVENTTARGET', form.id || 'form1');
//        formData.append('__EVENTARGUMENT', 'RefreshStats');

//        // Add any necessary viewstate data
//        const viewState = document.querySelector('input[name="__VIEWSTATE"]');
//        if (viewState) {
//            formData.append('__VIEWSTATE', viewState.value);
//        }

//        const viewStateGenerator = document.querySelector('input[name="__VIEWSTATEGENERATOR"]');
//        if (viewStateGenerator) {
//            formData.append('__VIEWSTATEGENERATOR', viewStateGenerator.value);
//        }

//        const eventValidation = document.querySelector('input[name="__EVENTVALIDATION"]');
//        if (eventValidation) {
//            formData.append('__EVENTVALIDATION', eventValidation.value);
//        }

//        fetch(window.location.pathname, {
//            method: 'POST',
//            headers: {
//                'X-Requested-With': 'XMLHttpRequest'
//            },
//            body: formData,
//            credentials: 'same-origin'
//        })
//            .then(response => {
//                if (!response.ok) {
//                    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
//                }
//                return response.text();
//            })
//            .then(html => {
//                this.updateStatsFromHtml(html);
//                this.setStatsLoadingState(false);
//                this.config.connectionRetries = 0;
//                this.hideConnectionError();
//                resolve();
//            })
//            .catch(error => {
//                console.error('Failed to refresh stats:', error);
//                this.setStatsLoadingState(false);
//                this.handleConnectionError();
//                reject(error);
//            });
//    });
//};

//// Update individual stat card with enhanced animation
//TPA.Dashboard.updateStatCard = function (currentCard, newCard) {
//    const currentValue = currentCard.querySelector('.stat-value');
//    const newValue = newCard.querySelector('.stat-value');

//    if (currentValue && newValue && currentValue.textContent !== newValue.textContent) {
//        const oldValue = currentValue.textContent;
//        const newValueText = newValue.textContent;

//        // Add update animation classes
//        currentCard.classList.add('updating');

//        // Show update indicator
//        this.showUpdateIndicator(currentCard);

//        // Animate value change
//        if (this.isNumericValue(oldValue) && this.isNumericValue(newValueText)) {
//            this.animateCounterValue(currentValue, oldValue, newValueText);
//        } else {
//            // Non-numeric values use fade transition
//            currentValue.style.opacity = '0';
//            setTimeout(() => {
//                currentValue.textContent = newValueText;
//                currentValue.style.opacity = '1';
//                currentValue.classList.add('counting');
//            }, 200);
//        }

//        // Remove animation classes
//        setTimeout(() => {
//            currentCard.classList.remove('updating');
//            currentValue.classList.remove('counting');
//        }, this.config.animationDuration);

//        console.log(`📈 Updated stat: ${currentCard.getAttribute('data-stat-key')} = ${newValueText}`);
//    }
//};

//// Check if value is numeric
//TPA.Dashboard.isNumericValue = function (value) {
//    if (!value || value === 'Loading...' || value === 'N/A') return false;

//    const cleanValue = value.replace(/[KMB%,\s]/g, '').replace(/days?/g, '');
//    return !isNaN(parseFloat(cleanValue)) && isFinite(cleanValue);
//};

//// Animate counter values
//TPA.Dashboard.animateCounterValue = function (element, startValue, endValue) {
//    const start = this.parseNumericValue(startValue);
//    const end = this.parseNumericValue(endValue);
//    const suffix = this.extractSuffix(endValue);

//    if (start === end) return;

//    const duration = Math.min(this.config.animationDuration, Math.abs(end - start) * 50);
//    const steps = Math.min(30, Math.abs(end - start));
//    const stepDuration = duration / steps;
//    const increment = (end - start) / steps;

//    let current = start;
//    let step = 0;

//    element.classList.add('counting');

//    const timer = setInterval(() => {
//        step++;
//        current += increment;

//        if (step >= steps) {
//            current = end;
//            clearInterval(timer);
//        }

//        const displayValue = this.formatDisplayValue(Math.round(current), suffix);
//        element.textContent = displayValue;

//    }, stepDuration);
//};

//// Parse numeric value from formatted string
//TPA.Dashboard.parseNumericValue = function (value) {
//    if (!value) return 0;

//    const cleanValue = value.replace(/[^\d.,KMB]/g, '');

//    if (cleanValue.includes('K')) {
//        return parseFloat(cleanValue.replace('K', '')) * 1000;
//    } else if (cleanValue.includes('M')) {
//        return parseFloat(cleanValue.replace('M', '')) * 1000000;
//    } else if (cleanValue.includes('B')) {
//        return parseFloat(cleanValue.replace('B', '')) * 1000000000;
//    }

//    return parseFloat(cleanValue) || 0;
//};

//// Extract suffix from value
//TPA.Dashboard.extractSuffix = function (value) {
//    if (!value) return '';

//    if (value.includes('days')) return ' days';
//    if (value.includes('day')) return ' day';
//    if (value.includes('%')) return '%';

//    return '';
//};

//// Format display value
//TPA.Dashboard.formatDisplayValue = function (value, suffix) {
//    if (suffix === ' days' || suffix === ' day') {
//        return value + (value === 1 ? ' day' : ' days');
//    }

//    if (value >= 1000000) {
//        return (value / 1000000).toFixed(1) + 'M' + (suffix === '%' ? suffix : '');
//    } else if (value >= 1000) {
//        return (value / 1000).toFixed(1) + 'K' + (suffix === '%' ? suffix : '');
//    }

//    return value + suffix;
//};

//// Show update indicator
//TPA.Dashboard.showUpdateIndicator = function (card) {
//    const existingIndicators = card.querySelectorAll('.stat-update-badge, .update-ripple');
//    existingIndicators.forEach(indicator => indicator.remove());

//    // Add update badge
//    const badge = document.createElement('div');
//    badge.className = 'stat-update-badge';
//    card.appendChild(badge);

//    // Add ripple effect
//    const ripple = document.createElement('div');
//    ripple.className = 'update-ripple';
//    ripple.style.cssText = `
//        position: absolute;
//        top: 50%;
//        left: 50%;
//        width: 0;
//        height: 0;
//        background: var(--stat-color, #3498db);
//        border-radius: 50%;
//        opacity: 0.2;
//        transform: translate(-50%, -50%);
//        animation: rippleEffect 0.8s ease-out;
//        pointer-events: none;
//        z-index: 0;
//    `;

//    card.appendChild(ripple);

//    // Cleanup
//    setTimeout(() => {
//        badge.remove();
//        ripple.remove();
//    }, 2500);
//};

//// =============================================================================
//// AUTO-REFRESH FUNCTIONALITY
//// =============================================================================

//// Start auto-refresh
//TPA.Dashboard.startAutoRefresh = function () {
//    this.stopAutoRefresh(); // Clear any existing interval

//    this.refreshInterval = setInterval(() => {
//        if (this.config.isVisible) {
//            console.log('⏰ Auto-refresh triggered');
//            this.refreshDashboardData();
//        }
//    }, this.config.refreshInterval);

//    console.log(`⏱ Auto-refresh started (${this.config.refreshInterval / 1000}s interval)`);
//};

//// Stop auto-refresh
//TPA.Dashboard.stopAutoRefresh = function () {
//    if (this.refreshInterval) {
//        clearInterval(this.refreshInterval);
//        this.refreshInterval = null;
//        console.log('⏱ Auto-refresh stopped');
//    }
//};

//// Toggle auto-refresh
//TPA.Dashboard.toggleAutoRefresh = function () {
//    this.config.isAutoRefreshEnabled = !this.config.isAutoRefreshEnabled;

//    if (this.config.isAutoRefreshEnabled) {
//        this.startAutoRefresh();
//        this.showNotification('Auto-refresh enabled', 'success', 2000);
//    } else {
//        this.stopAutoRefresh();
//        this.showNotification('Auto-refresh disabled', 'warning', 2000);
//    }

//    // Update toggle buttons
//    const toggleBtns = document.querySelectorAll('[data-action="toggle-refresh"]');
//    toggleBtns.forEach(btn => {
//        btn.textContent = this.config.isAutoRefreshEnabled ? 'Disable Auto-refresh' : 'Enable Auto-refresh';
//        btn.classList.toggle('active', this.config.isAutoRefreshEnabled);
//    });
//};

//// =============================================================================
//// LOADING STATES AND UI FEEDBACK
//// =============================================================================

//// Set refresh button loading state
//TPA.Dashboard.setRefreshState = function (isLoading) {
//    const refreshBtns = document.querySelectorAll('.refresh-btn, [data-action="refresh"]');
//    refreshBtns.forEach(btn => {
//        if (isLoading) {
//            btn.classList.add('loading');
//            btn.disabled = true;
//            const icon = btn.querySelector('i');
//            if (icon) {
//                icon.style.animation = 'spin 1s linear infinite';
//            }
//        } else {
//            btn.classList.remove('loading');
//            btn.disabled = false;
//            const icon = btn.querySelector('i');
//            if (icon) {
//                icon.style.animation = '';
//            }
//        }
//    });
//};

//// Set loading state for stat cards
//TPA.Dashboard.setStatsLoadingState = function (isLoading) {
//    const statCards = document.querySelectorAll('.stat-card');
//    statCards.forEach(card => {
//        if (isLoading) {
//            card.classList.add('loading');
//        } else {
//            card.classList.remove('loading');
//        }
//    });
//};

//// =============================================================================
//// RECENT ACTIVITIES
//// =============================================================================

//// Refresh recent activities
//TPA.Dashboard.refreshRecentActivities = function () {
//    return new Promise((resolve, reject) => {
//        fetch(window.location.pathname, {
//            method: 'POST',
//            headers: {
//                'Content-Type': 'application/x-www-form-urlencoded',
//                'X-Requested-With': 'XMLHttpRequest'
//            },
//            body: '__EVENTTARGET=' + encodeURIComponent(document.forms[0]?.id || 'form1') + '&__EVENTARGUMENT=RefreshActivities',
//            credentials: 'same-origin'
//        })
//            .then(response => {
//                if (!response.ok) {
//                    throw new Error(`HTTP ${response.status}: ${response.statusText}`);
//                }
//                return response.text();
//            })
//            .then(html => {
//                this.updateActivitiesFromHtml(html);
//                resolve();
//            })
//            .catch(error => {
//                console.error('Failed to refresh activities:', error);
//                reject(error);
//            });
//    });
//};

//// Update activities from HTML
//TPA.Dashboard.updateActivitiesFromHtml = function (html) {
//    const tempDiv = document.createElement('div');
//    tempDiv.innerHTML = html;

//    const newActivitiesContainer = tempDiv.querySelector('#litRecentActivities, [data-recent-activities]');
//    const currentActivitiesContainer = document.querySelector('#litRecentActivities, [data-recent-activities]');

//    if (newActivitiesContainer && currentActivitiesContainer) {
//        const newContent = newActivitiesContainer.innerHTML;
//        const currentContent = currentActivitiesContainer.innerHTML;

//        if (newContent !== currentContent) {
//            // Fade out, update, fade in
//            currentActivitiesContainer.style.opacity = '0.5';
//            setTimeout(() => {
//                currentActivitiesContainer.innerHTML = newContent;
//                currentActivitiesContainer.style.opacity = '1';
//            }, 200);

//            console.log('📝 Updated recent activities');
//        }
//    }
//};

//// =============================================================================
//// NOTIFICATIONS SYSTEM
//// =============================================================================

//// Enhanced notification system
//TPA.Dashboard.showNotification = function (message, type = 'info', duration = 3000) {
//    // Remove existing notifications
//    this.clearAllNotifications();

//    // Create notification element
//    const toast = document.createElement('div');
//    toast.className = `tpa-notification tpa-notification-${type}`;
//    toast.setAttribute('role', 'alert');
//    toast.setAttribute('aria-live', 'polite');

//    // Add icon and content
//    const icon = this.getNotificationIcon(type);
//    toast.innerHTML = `
//        <div class="notification-content">
//            <i class="material-icons notification-icon">${icon}</i>
//            <span class="notification-message">${message}</span>
//            <button class="notification-close" aria-label="Close notification">
//                <i class="material-icons">close</i>
//            </button>
//        </div>
//    `;

//    // Add close handler
//    const closeBtn = toast.querySelector('.notification-close');
//    closeBtn.addEventListener('click', () => {
//        this.hideNotification(toast);
//    });

//    // Add to page
//    document.body.appendChild(toast);

//    // Animate in
//    setTimeout(() => {
//        toast.classList.add('show');
//    }, 10);

//    // Auto-hide
//    if (duration > 0) {
//        setTimeout(() => {
//            this.hideNotification(toast);
//        }, duration);
//    }

//    return toast;
//};

//// Hide notification
//TPA.Dashboard.hideNotification = function (toast) {
//    toast.classList.remove('show');
//    setTimeout(() => {
//        if (toast.parentNode) {
//            toast.parentNode.removeChild(toast);
//        }
//    }, 300);
//};

//// Clear all notifications
//TPA.Dashboard.clearAllNotifications = function () {
//    const notifications = document.querySelectorAll('.tpa-notification');
//    notifications.forEach(notification => {
//        this.hideNotification(notification);
//    });
//};

//// Get notification icon
//TPA.Dashboard.getNotificationIcon = function (type) {
//    const icons = {
//        'success': 'check_circle',
//        'error': 'error',
//        'warning': 'warning',
//        'info': 'info'
//    };
//    return icons[type] || 'notifications';
//};

//// =============================================================================
//// CONNECTION AND ERROR HANDLING
//// =============================================================================

//// Handle connection errors
//TPA.Dashboard.handleConnectionError = function () {
//    this.config.connectionRetries++;

//    if (this.config.connectionRetries >= this.config.maxRetries) {
//        this.showConnectionError();
//        this.stopAutoRefresh();
//        this.showNotification('Connection lost. Auto-refresh disabled.', 'error', 0);
//    } else {
//        console.warn(`⚠ Connection error (retry ${this.config.connectionRetries}/${this.config.maxRetries})`);

//        // Exponential backoff
//        const retryDelay = Math.pow(2, this.config.connectionRetries) * 1000;
//        setTimeout(() => {
//            if (this.config.isVisible) {
//                this.refreshDashboardData();
//            }
//        }, retryDelay);
//    }
//};

//// Show connection error
//TPA.Dashboard.showConnectionError = function () {
//    let indicator = document.querySelector('.connection-status');
//    if (!indicator) {
//        indicator = document.createElement('div');
//        indicator.className = 'connection-status disconnected';
//        indicator.innerHTML = `
//            <i class="material-icons">wifi_off</i>
//            <span>Connection lost</span>
//        `;
//        document.body.appendChild(indicator);
//    }

//    indicator.classList.add('show');
//};

//// Hide connection error
//TPA.Dashboard.hideConnectionError = function () {
//    const indicator = document.querySelector('.connection-status');
//    if (indicator) {
//        indicator.classList.remove('show');
//        setTimeout(() => {
//            if (indicator.parentNode) {
//                indicator.parentNode.removeChild(indicator);
//            }
//        }, 300);
//    }
//};

//// =============================================================================
//// UTILITY FUNCTIONS
//// =============================================================================

//// Focus dashboard
//TPA.Dashboard.focusDashboard = function () {
//    const firstStatCard = document.querySelector('.stat-card');
//    if (firstStatCard) {
//        firstStatCard.focus();
//        this.showNotification('Dashboard focused', 'info', 1500);
//    }
//};

//// Check for new notifications (placeholder)
//TPA.Dashboard.checkForNewNotifications = function () {
//    return Promise.resolve(); // Implement as needed
//};

//// Performance monitor
//TPA.Dashboard.performanceMonitor = {
//    startTime: null,

//    start: function () {
//        this.startTime = performance.now();
//    },

//    end: function (operation) {
//        if (this.startTime) {
//            const duration = performance.now() - this.startTime;
//            console.log(`⚡ ${operation} completed in ${duration.toFixed(2)}ms`);
//            this.startTime = null;

//            if (duration > 2000) {
//                TPA.Dashboard.showNotification(`Slow operation: ${operation}`, 'warning', 3000);
//            }
//        }
//    }
//};

//// Setup periodic cleanup
//TPA.Dashboard.setupPeriodicCleanup = function () {
//    // Clean up old notifications and effects every 5 minutes
//    setInterval(() => {
//        this.cleanup();
//    }, 5 * 60 * 1000);
//};

//// Cleanup function
//TPA.Dashboard.cleanup = function () {
//    // Remove orphaned elements
//    const orphans = document.querySelectorAll('.click-ripple, .update-ripple, .stat-update-badge');
//    orphans.forEach(element => {
//        if (element.parentNode) {
//            element.parentNode.removeChild(element);
//        }
//    });

//    console.log('🧹 Dashboard cleanup completed');
//};

//// =============================================================================
//// ENHANCED STYLES
//// =============================================================================

//// Add enhanced styles dynamically
//TPA.Dashboard.addEnhancedStyles = function () {
//    if (document.querySelector('#tpa-dashboard-styles')) return;

//    const style = document.createElement('style');
//    style.id = 'tpa-dashboard-styles';
//    style.textContent = `
//        /* Click Ripple Animation */
//        @keyframes clickRipple {
//            0% {
//                transform: scale(0);
//                opacity: 0.2;
//            }
//            100% {
//                transform: scale(1);
//                opacity: 0;
//            }
//        }
        
//        /* Ripple Effect Animation */
//        @keyframes rippleEffect {
//            0% {
//                width: 0;
//                height: 0;
//                opacity: 0.3;
//            }
//            50% {
//                width: 100px;
//                height: 100px;
//                opacity: 0.1;
//            }
//            100% {
//                width: 200px;
//                height: 200px;
//                opacity: 0;
//            }
//        }
        
//        /* Spin Animation for Loading Icons */
//        @keyframes spin {
//            from { transform: rotate(0deg); }
//            to { transform: rotate(360deg); }
//        }
        
//        /* Stat Card Enhancements */
//        .stat-card {
//            cursor: pointer;
//            user-select: none;
//            position: relative;
//            overflow: hidden;
//        }
        
//        .stat-card:focus {
//            outline: 2px solid var(--stat-color, #3498db);
//            outline-offset: 2px;
//        }
        
//        .stat-card.trend-value .stat-value::after {
//            content: '';
//            position: absolute;
//            bottom: -2px;
//            left: 0;
//            right: 0;
//            height: 2px;
//            background: linear-gradient(90deg, var(--stat-color, #3498db), transparent);
//            border-radius: 1px;
//        }
        
//        /* Loading Text Animation */
//        .loading-text {
//            background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%) !important;
//            background-size: 200% 100% !important;
//            animation: loadingShimmer 1.5s infinite !important;
//            -webkit-background-clip: text !important;
//            -webkit-text-fill-color: transparent !important;
//            background-clip: text !important;
//        }
        
//        @keyframes loadingShimmer {
//            0% { background-position: 200% 0; }
//            100% { background-position: -200% 0; }
//        }
        
//        /* Enhanced Notifications */
//        .tpa-notification {
//            position: fixed;
//            bottom: 20px;
//            right: 20px;
//            background: linear-gradient(135deg, #34495e, #2c3e50);
//            color: white;
//            padding: 16px 20px;
//            border-radius: 12px;
//            font-size: 14px;
//            font-weight: 500;
//            z-index: 10000;
//            opacity: 0;
//            transform: translateX(400px);
//            transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
//            box-shadow: 0 8px 32px rgba(0, 0, 0, 0.15);
//            backdrop-filter: blur(10px);
//            max-width: 350px;
//            min-width: 280px;
//        }
        
//        .tpa-notification.show {
//            opacity: 1;
//            transform: translateX(0);
//        }
        
//        .tpa-notification-success {
//            background: linear-gradient(135deg, #2ecc71, #27ae60);
//        }
        
//        .tpa-notification-error {
//            background: linear-gradient(135deg, #e74c3c, #c0392b);
//        }
        
//        .tpa-notification-warning {
//            background: linear-gradient(135deg, #f39c12, #d68910);
//        }
        
//        .tpa-notification-info {
//            background: linear-gradient(135deg, #3498db, #2980b9);
//        }
        
//        .notification-content {
//            display: flex;
//            align-items: center;
//            gap: 12px;
//        }
        
//        .notification-icon {
//            font-size: 20px;
//            flex-shrink: 0;
//        }
        
//        .notification-message {
//            flex: 1;
//            line-height: 1.4;
//        }
        
//        .notification-close {
//            background: none;
//            border: none;
//            color: inherit;
//            cursor: pointer;
//            padding: 4px;
//            border-radius: 4px;
//            transition: background-color 0.2s;
//            flex-shrink: 0;
//        }
        
//        .notification-close:hover {
//            background-color: rgba(255, 255, 255, 0.1);
//        }
        
//        .notification-close i {
//            font-size: 18px;
//        }
        
//        /* Connection Status Indicator */
//        .connection-status {
//            position: fixed;
//            top: 20px;
//            left: 50%;
//            transform: translateX(-50%);
//            background: linear-gradient(135deg, #e74c3c, #c0392b);
//            color: white;
//            padding: 12px 20px;
//            border-radius: 25px;
//            font-size: 13px;
//            font-weight: 600;
//            z-index: 10001;
//            opacity: 0;
//            transform: translateX(-50%) translateY(-40px);
//            transition: all 0.3s ease;
//            display: flex;
//            align-items: center;
//            gap: 8px;
//            box-shadow: 0 4px 16px rgba(0, 0, 0, 0.2);
//        }
        
//        .connection-status.show {
//            opacity: 1;
//            transform: translateX(-50%) translateY(0);
//        }
        
//        .connection-status.connected {
//            background: linear-gradient(135deg, #2ecc71, #27ae60);
//        }
        
//        .connection-status i {
//            font-size: 16px;
//        }
        
//        /* Refresh Button Enhancements */
//        .refresh-btn.loading {
//            opacity: 0.7;
//            cursor: not-allowed;
//        }
        
//        .refresh-btn.loading i {
//            animation: spin 1s linear infinite;
//        }
        
//        /* Theme Support */
//        .tpa-theme-dark .stat-card {
//            background: linear-gradient(145deg, #2c3e50, #34495e);
//            border-color: #34495e;
//            color: #ecf0f1;
//        }
        
//        .tpa-theme-dark .stat-value {
//            background: linear-gradient(135deg, #ecf0f1, #bdc3c7);
//            -webkit-background-clip: text;
//            -webkit-text-fill-color: transparent;
//        }
        
//        .tpa-theme-dark .stat-title {
//            color: #ecf0f1;
//        }
        
//        .tpa-theme-dark .stat-subtitle {
//            color: #95a5a6;
//        }
        
//        .tpa-theme-dark .stat-card:hover {
//            background: linear-gradient(145deg, #34495e, #2c3e50);
//        }
        
//        .tpa-theme-dark .loading-text {
//            background: linear-gradient(90deg, #34495e 25%, #2c3e50 50%, #34495e 75%) !important;
//        }
        
//        /* Accessibility Improvements */
//        .stat-card:focus-visible {
//            outline: 3px solid var(--stat-color, #3498db);
//            outline-offset: 2px;
//        }
        
//        @media (prefers-reduced-motion: reduce) {
//            .stat-card,
//            .tpa-notification,
//            .connection-status,
//            .refresh-btn i {
//                transition: none !important;
//                animation: none !important;
//            }
//        }
        
//        /* Mobile Optimizations */
//        @media (max-width: 768px) {
//            .tpa-notification {
//                left: 10px;
//                right: 10px;
//                bottom: 10px;
//                max-width: none;
//                min-width: auto;
//            }
            
//            .connection-status {
//                left: 10px;
//                right: 10px;
//                top: 10px;
//                transform: none;
//            }
            
//            .connection-status.show {
//                transform: none;
//            }
//        }
        
//        /* High Contrast Mode */
//        @media (prefers-contrast: high) {
//            .stat-card {
//                border: 2px solid #000;
//            }
            
//            .stat-value {
//                color: #000 !important;
//                font-weight: 900 !important;
//                -webkit-text-fill-color: #000 !important;
//            }
            
//            .stat-title {
//                color: #000 !important;
//                font-weight: 700 !important;
//            }
//        }
        
//        /* Print Styles */
//        @media print {
//            .tpa-notification,
//            .connection-status,
//            .refresh-btn,
//            .click-ripple,
//            .update-ripple,
//            .stat-update-badge {
//                display: none !important;
//            }
            
//            .stat-card {
//                box-shadow: none !important;
//                border: 1px solid #000 !important;
//                background: white !important;
//            }
//        }
//    `;

//    document.head.appendChild(style);
//};

//// =============================================================================
//// EVENT HANDLERS AND INITIALIZATION
//// =============================================================================

//// Handle page unload
//TPA.Dashboard.handleUnload = function () {
//    this.stopAutoRefresh();
//    this.clearAllNotifications();
//    console.log('👋 Dashboard cleanup on unload');
//};

//// Handle window resize
//TPA.Dashboard.handleResize = function () {
//    // Debounce resize events
//    clearTimeout(this.resizeTimeout);
//    this.resizeTimeout = setTimeout(() => {
//        console.log('📱 Dashboard responsive adjustment');
//        // Add any responsive adjustments here
//    }, 250);
//};

//// Handle network status changes
//TPA.Dashboard.handleNetworkChange = function () {
//    if (navigator.onLine) {
//        console.log('🌐 Network connection restored');
//        this.config.connectionRetries = 0;
//        this.hideConnectionError();
//        this.refreshDashboardData();
//        this.showNotification('Connection restored', 'success', 2000);

//        if (this.config.isAutoRefreshEnabled) {
//            this.startAutoRefresh();
//        }
//    } else {
//        console.log('📡 Network connection lost');
//        this.stopAutoRefresh();
//        this.showConnectionError();
//        this.showNotification('Network connection lost', 'error', 0);
//    }
//};

//// Setup event listeners
//TPA.Dashboard.setupEventListeners = function () {
//    // Page lifecycle events
//    window.addEventListener('beforeunload', () => this.handleUnload());
//    window.addEventListener('resize', () => this.handleResize());

//    // Network status events
//    window.addEventListener('online', () => this.handleNetworkChange());
//    window.addEventListener('offline', () => this.handleNetworkChange());

//    // Page focus events
//    window.addEventListener('focus', () => {
//        if (this.config.isAutoRefreshEnabled) {
//            this.refreshDashboardData();
//        }
//    });
//};

//// =============================================================================
//// PUBLIC API
//// =============================================================================

//// Expose public methods
//TPA.Dashboard.publicAPI = {
//    // Manual refresh
//    refresh: function () {
//        return TPA.Dashboard.manualRefresh();
//    },

//    // Toggle auto-refresh
//    toggleAutoRefresh: function () {
//        return TPA.Dashboard.toggleAutoRefresh();
//    },

//    // Show notification
//    notify: function (message, type, duration) {
//        return TPA.Dashboard.showNotification(message, type, duration);
//    },

//    // Get configuration
//    getConfig: function () {
//        return { ...TPA.Dashboard.config };
//    },

//    // Update configuration
//    updateConfig: function (newConfig) {
//        TPA.Dashboard.config = { ...TPA.Dashboard.config, ...newConfig };

//        // Apply changes that need immediate effect
//        if (newConfig.refreshInterval !== undefined) {
//            if (TPA.Dashboard.config.isAutoRefreshEnabled) {
//                TPA.Dashboard.startAutoRefresh();
//            }
//        }

//        if (newConfig.theme !== undefined) {
//            TPA.Dashboard.applyTheme(newConfig.theme);
//        }
//    },

//    // Get stats
//    getStats: function () {
//        const statCards = document.querySelectorAll('.stat-card');
//        const stats = {};

//        statCards.forEach(card => {
//            const key = card.getAttribute('data-stat-key');
//            const value = card.querySelector('.stat-value')?.textContent;
//            const title = card.querySelector('.stat-title')?.textContent;

//            if (key) {
//                stats[key] = { title, value };
//            }
//        });

//        return stats;
//    }
//};

//// =============================================================================
//// INITIALIZATION AND STARTUP
//// =============================================================================

//// Enhanced initialization
//TPA.Dashboard.initialize = function () {
//    console.log('🚀 TPA Dashboard System Starting...');

//    // Wait for DOM to be ready
//    if (document.readyState === 'loading') {
//        document.addEventListener('DOMContentLoaded', () => this.init());
//    } else {
//        this.init();
//    }

//    // Setup global event listeners
//    this.setupEventListeners();

//    // Expose public API
//    window.TPADashboard = this.publicAPI;
//};

//// =============================================================================
//// AUTO-INITIALIZATION
//// =============================================================================

//// Initialize when script loads
//TPA.Dashboard.initialize();

//// Export for module systems
//if (typeof module !== 'undefined' && module.exports) {
//    module.exports = TPA.Dashboard;
//}

//// AMD support
//if (typeof define === 'function' && define.amd) {
//    define('tpa-dashboard', [], function () {
//        return TPA.Dashboard;
//    });
//}

//// Global fallback
//window.TPA = window.TPA || {};
//window.TPA.Dashboard = TPA.Dashboard;

//console.log('📊 TPA Dashboard System Loaded Successfully');

//// =============================================================================
//// DEBUG AND DEVELOPMENT HELPERS
//// =============================================================================

//// Development mode helpers
//if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
//    // Add debug helpers
//    TPA.Dashboard.debug = {
//        // Force refresh
//        forceRefresh: () => TPA.Dashboard.manualRefresh(),

//        // Simulate connection error
//        simulateError: () => TPA.Dashboard.handleConnectionError(),

//        // Show test notification
//        testNotification: (type = 'info') => {
//            const messages = {
//                success: 'Test success notification',
//                error: 'Test error notification',
//                warning: 'Test warning notification',
//                info: 'Test info notification'
//            };
//            TPA.Dashboard.showNotification(messages[type] || messages.info, type, 3000);
//        },

//        // Get current state
//        getState: () => ({
//            config: TPA.Dashboard.config,
//            isRefreshing: !!TPA.Dashboard.refreshInterval,
//            performance: TPA.Dashboard.performanceMonitor
//        }),

//        // Clear all data
//        reset: () => {
//            TPA.Dashboard.stopAutoRefresh();
//            TPA.Dashboard.clearAllNotifications();
//            TPA.Dashboard.hideConnectionError();
//            TPA.Dashboard.cleanup();
//        }
//    };

//    // Make debug available globally
//    window.TPADebug = TPA.Dashboard.debug;

//    console.log('🛠 Debug mode enabled. Use TPADebug.* for testing.');
//}

///*
//=============================================================================
//TPA DASHBOARD USAGE EXAMPLES:
//=============================================================================

//// Manual refresh
//TPADashboard.refresh();

//// Show custom notification
//TPADashboard.notify('Custom message', 'success', 5000);

//// Toggle auto-refresh
//TPADashboard.toggleAutoRefresh();

//// Update configuration
//TPADashboard.updateConfig({
//    refreshInterval: 60000, // 1 minute
//    theme: 'dark'
//});

//// Get current stats
//const stats = TPADashboard.getStats();
//console.log(stats);

//// In development mode:
//TPADebug.testNotification('success');
//TPADebug.simulateError();
//TPADebug.getState();

//=============================================================================
//*/