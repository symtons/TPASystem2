/**
 * TPA Session Manager - Auto logout with warning popup
 * Tracks user activity and automatically logs out after 10 minutes of inactivity
 * Shows warning popup before logout
 */

// Initialize TPA namespace if it doesn't exist
if (typeof TPA === 'undefined') {
    window.TPA = {};
}

TPA.SessionManager = (function () {
    'use strict';

    // Configuration
    const CONFIG = {
        SESSION_TIMEOUT: 10 * 60 * 1000,     // 10 minutes in milliseconds
        WARNING_TIME: 2 * 60 * 1000,        // Show warning 2 minutes before timeout
        CHECK_INTERVAL: 30 * 1000,          // Check every 30 seconds
        PING_INTERVAL: 5 * 60 * 1000,       // Ping server every 5 minutes to keep session alive
        LOGOUT_URL: 'Login.aspx',            // Where to redirect on logout
        KEEP_ALIVE_URL: 'KeepAlive.ashx'     // Server endpoint to keep session alive
    };

    // State variables
    let lastActivity = Date.now();
    let warningShown = false;
    let warningModal = null;
    let checkTimer = null;
    let countdownTimer = null;
    let pingTimer = null;
    let isWarningActive = false;

    // Activity events to track
    const ACTIVITY_EVENTS = [
        'mousedown', 'mousemove', 'keypress', 'scroll', 'touchstart', 'click'
    ];

    /**
     * Initialize session management
     */
    function init() {
        console.log('🔐 Initializing TPA Session Manager...');

        // Only run on authenticated pages (skip login page)
        if (window.location.pathname.toLowerCase().indexOf('login') !== -1) {
            console.log('📄 Login page detected - skipping session manager');
            return;
        }

        setupActivityTracking();
        startSessionMonitoring();
        setupKeepAlive();
        createWarningModal();

        console.log('✅ Session Manager initialized - Auto logout after 10 minutes of inactivity');
    }

    /**
     * Setup activity event listeners
     */
    function setupActivityTracking() {
        ACTIVITY_EVENTS.forEach(event => {
            document.addEventListener(event, trackActivity, true);
        });

        // Track form submissions
        document.addEventListener('submit', trackActivity, true);

        // Track AJAX requests (if using jQuery)
        if (window.$ && $.ajaxSetup) {
            $(document).ajaxComplete(trackActivity);
        }
    }

    /**
     * Track user activity
     */
    function trackActivity() {
        lastActivity = Date.now();

        // Hide warning if user becomes active
        if (isWarningActive) {
            hideWarning();
        }

        // Reset warning flag
        warningShown = false;
    }

    /**
     * Start session monitoring
     */
    function startSessionMonitoring() {
        checkTimer = setInterval(checkSession, CONFIG.CHECK_INTERVAL);
    }

    /**
     * Setup periodic server ping to keep session alive
     */
    function setupKeepAlive() {
        pingTimer = setInterval(pingServer, CONFIG.PING_INTERVAL);
    }

    /**
     * Ping server to keep session alive
     */
    function pingServer() {
        // Only ping if user has been active recently
        const timeSinceActivity = Date.now() - lastActivity;
        if (timeSinceActivity < CONFIG.PING_INTERVAL) {
            fetch(window.location.pathname, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: '__EVENTTARGET=KeepAlive&__EVENTARGUMENT=ping'
            }).catch(error => {
                console.warn('⚠️ Keep-alive ping failed:', error);
            });
        }
    }

    /**
     * Check session timeout
     */
    function checkSession() {
        const now = Date.now();
        const timeSinceActivity = now - lastActivity;
        const timeUntilTimeout = CONFIG.SESSION_TIMEOUT - timeSinceActivity;

        // Debug logging
        console.log(`⏱️ Session check - Time since activity: ${Math.round(timeSinceActivity / 1000)}s, Time until timeout: ${Math.round(timeUntilTimeout / 1000)}s`);

        // Show warning if approaching timeout
        if (timeUntilTimeout <= CONFIG.WARNING_TIME && !warningShown) {
            showWarning(Math.round(timeUntilTimeout / 1000));
            warningShown = true;
        }

        // Auto logout if timeout reached
        if (timeUntilTimeout <= 0) {
            performLogout();
        }
    }

    /**
     * Create warning modal HTML
     */
    function createWarningModal() {
        const modalHTML = `
            <div id="sessionWarningModal" class="session-warning-modal" style="display: none;">
                <div class="session-warning-overlay"></div>
                <div class="session-warning-content">
                    <div class="warning-header">
                        <i class="warning-icon">⚠️</i>
                        <h3>Session About to Expire</h3>
                    </div>
                    <div class="warning-body">
                        <p>Your session will expire in <strong><span id="countdownDisplay">2:00</span></strong> due to inactivity.</p>
                        <p>You will be automatically logged out to protect your data.</p>
                    </div>
                    <div class="warning-actions">
                        <button type="button" id="stayLoggedInBtn" class="btn btn-primary">
                            <i class="material-icons">refresh</i>
                            Stay Logged In
                        </button>
                        <button type="button" id="logoutNowBtn" class="btn btn-secondary">
                            <i class="material-icons">exit_to_app</i>
                            Logout Now
                        </button>
                    </div>
                </div>
            </div>
        `;

        document.body.insertAdjacentHTML('beforeend', modalHTML);

        // Add event listeners
        document.getElementById('stayLoggedInBtn').addEventListener('click', stayLoggedIn);
        document.getElementById('logoutNowBtn').addEventListener('click', performLogout);

        // Store modal reference
        warningModal = document.getElementById('sessionWarningModal');
    }

    /**
     * Show session warning
     */
    function showWarning(secondsLeft) {
        console.log('⚠️ Showing session warning - ' + secondsLeft + ' seconds remaining');

        isWarningActive = true;
        warningModal.style.display = 'block';

        // Disable page interaction
        document.body.style.pointerEvents = 'none';
        warningModal.style.pointerEvents = 'all';

        // Start countdown
        startCountdown(secondsLeft);

        // Play warning sound if available
        playWarningSound();
    }

    /**
     * Hide session warning
     */
    function hideWarning() {
        if (warningModal && isWarningActive) {
            console.log('✅ Hiding session warning - user is active');

            isWarningActive = false;
            warningModal.style.display = 'none';

            // Re-enable page interaction
            document.body.style.pointerEvents = 'all';

            // Clear countdown
            if (countdownTimer) {
                clearInterval(countdownTimer);
                countdownTimer = null;
            }
        }
    }

    /**
     * Start countdown display
     */
    function startCountdown(initialSeconds) {
        let secondsLeft = initialSeconds;
        const countdownDisplay = document.getElementById('countdownDisplay');

        // Update display immediately
        updateCountdownDisplay(secondsLeft);

        countdownTimer = setInterval(() => {
            secondsLeft--;
            updateCountdownDisplay(secondsLeft);

            if (secondsLeft <= 0) {
                clearInterval(countdownTimer);
                performLogout();
            }
        }, 1000);
    }

    /**
     * Update countdown display
     */
    function updateCountdownDisplay(seconds) {
        const minutes = Math.floor(seconds / 60);
        const secs = seconds % 60;
        const display = `${minutes}:${secs.toString().padStart(2, '0')}`;

        const countdownElement = document.getElementById('countdownDisplay');
        if (countdownElement) {
            countdownElement.textContent = display;

            // Add urgency styling for last 30 seconds
            if (seconds <= 30) {
                countdownElement.style.color = '#e74c3c';
                countdownElement.style.fontWeight = 'bold';
            }
        }
    }

    /**
     * Stay logged in - extend session
     */
    function stayLoggedIn() {
        console.log('🔄 User chose to stay logged in - extending session');

        // Reset activity time
        trackActivity();

        // Hide warning
        hideWarning();

        // Ping server to refresh session
        pingServer();

        // Show confirmation
        showNotification('Session extended successfully', 'success');
    }

    /**
     * Perform logout
     */
    function performLogout() {
        console.log('🚪 Performing automatic logout due to session timeout');

        // Clear all timers
        cleanup();

        // Show logout message
        showNotification('You have been logged out due to inactivity', 'warning', 0);

        // Redirect to logout/login page after short delay
        setTimeout(() => {
            window.location.href = CONFIG.LOGOUT_URL;
        }, 2000);
    }

    /**
     * Play warning sound
     */
    function playWarningSound() {
        try {
            // Create audio context for a simple beep
            if (window.AudioContext || window.webkitAudioContext) {
                const audioContext = new (window.AudioContext || window.webkitAudioContext)();
                const oscillator = audioContext.createOscillator();
                const gainNode = audioContext.createGain();

                oscillator.connect(gainNode);
                gainNode.connect(audioContext.destination);

                oscillator.frequency.value = 800; // Frequency in Hz
                gainNode.gain.setValueAtTime(0.3, audioContext.currentTime);
                gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.5);

                oscillator.start();
                oscillator.stop(audioContext.currentTime + 0.5);
            }
        } catch (error) {
            // Ignore audio errors
            console.warn('Could not play warning sound:', error);
        }
    }

    /**
     * Show notification
     */
    function showNotification(message, type = 'info', duration = 5000) {
        // Try to use existing notification system first
        if (window.TPA && window.TPA.Dashboard && window.TPA.Dashboard.showNotification) {
            window.TPA.Dashboard.showNotification(message, type, duration);
            return;
        }

        // Fallback: create simple notification
        const notification = document.createElement('div');
        notification.className = `session-notification ${type}`;
        notification.innerHTML = `
            <div class="notification-content">
                <span class="notification-message">${message}</span>
                <button class="notification-close">&times;</button>
            </div>
        `;

        document.body.appendChild(notification);

        // Auto-remove notification
        if (duration > 0) {
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.remove();
                }
            }, duration);
        }

        // Close button
        notification.querySelector('.notification-close').addEventListener('click', () => {
            notification.remove();
        });
    }

    /**
     * Cleanup timers and events
     */
    function cleanup() {
        if (checkTimer) {
            clearInterval(checkTimer);
            checkTimer = null;
        }

        if (countdownTimer) {
            clearInterval(countdownTimer);
            countdownTimer = null;
        }

        if (pingTimer) {
            clearInterval(pingTimer);
            pingTimer = null;
        }

        // Remove event listeners
        ACTIVITY_EVENTS.forEach(event => {
            document.removeEventListener(event, trackActivity, true);
        });
    }

    /**
     * Public API
     */
    return {
        init: init,
        trackActivity: trackActivity,
        performLogout: performLogout,
        cleanup: cleanup,

        // For testing/debugging
        getTimeUntilTimeout: function () {
            const timeSinceActivity = Date.now() - lastActivity;
            return CONFIG.SESSION_TIMEOUT - timeSinceActivity;
        },

        showWarning: function () {
            showWarning(120); // Show 2-minute warning for testing
        }
    };
})();

// Auto-initialize when DOM is ready
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', TPA.SessionManager.init);
} else {
    TPA.SessionManager.init();
}

// Make available globally for debugging
window.TPASessionManager = TPA.SessionManager;