// ===============================================
// TPA HR System - Common JavaScript
// File: Content/js/tpa-common.js
// ===============================================

// Global TPA namespace
window.TPA = window.TPA || {};

// ===============================================
// Document Ready
// ===============================================
document.addEventListener('DOMContentLoaded', function () {
    // Initialize Materialize components
    if (typeof M !== 'undefined') {
        M.AutoInit();
    }

    // Initialize TPA components
    TPA.init();

    console.log('🚀 TPA HR System initialized');
});

// ===============================================
// TPA Core Functions
// ===============================================
TPA.init = function () {
    // Initialize all TPA modules
    TPA.Forms.init();
    TPA.Notifications.init();
    TPA.Utils.init();
};

// ===============================================
// Form Utilities
// ===============================================
TPA.Forms = {
    init: function () {
        this.setupValidation();
        this.setupFormEnhancements();
    },

    setupValidation: function () {
        // Email validation
        const emailInputs = document.querySelectorAll('input[type="email"]');
        emailInputs.forEach(input => {
            input.addEventListener('blur', function () {
                TPA.Forms.validateEmail(this);
            });
        });

        // Required field validation
        const requiredInputs = document.querySelectorAll('input[required], textarea[required], select[required]');
        requiredInputs.forEach(input => {
            input.addEventListener('blur', function () {
                TPA.Forms.validateRequired(this);
            });
        });
    },

    setupFormEnhancements: function () {
        // Auto-save form data to localStorage (if enabled)
        const forms = document.querySelectorAll('form[data-autosave]');
        forms.forEach(form => {
            TPA.Forms.setupAutoSave(form);
        });

        // Form submission handling
        const ajaxForms = document.querySelectorAll('form[data-ajax]');
        ajaxForms.forEach(form => {
            form.addEventListener('submit', function (e) {
                e.preventDefault();
                TPA.Forms.submitAjaxForm(this);
            });
        });
    },

    validateEmail: function (input) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        const isValid = emailRegex.test(input.value);

        if (!isValid && input.value.length > 0) {
            this.showFieldError(input, 'Please enter a valid email address');
            return false;
        } else {
            this.clearFieldError(input);
            return true;
        }
    },

    validateRequired: function (input) {
        const isValid = input.value.trim().length > 0;

        if (!isValid) {
            this.showFieldError(input, 'This field is required');
            return false;
        } else {
            this.clearFieldError(input);
            return true;
        }
    },

    showFieldError: function (input, message) {
        input.classList.add('error');
        this.showValidationMessage(input, message, 'error');
    },

    clearFieldError: function (input) {
        input.classList.remove('error');
        this.clearValidationMessage(input);
    },

    showValidationMessage: function (input, message, type) {
        // Remove existing message
        this.clearValidationMessage(input);

        // Create new message
        const messageElement = document.createElement('div');
        messageElement.className = `validation-message validation-${type}`;
        messageElement.textContent = message;

        // Insert after input
        input.parentNode.insertBefore(messageElement, input.nextSibling);
    },

    clearValidationMessage: function (input) {
        const existingMessage = input.parentNode.querySelector('.validation-message');
        if (existingMessage) {
            existingMessage.remove();
        }
    },

    setupAutoSave: function (form) {
        const formId = form.id || `form_${Date.now()}`;
        const inputs = form.querySelectorAll('input, textarea, select');

        inputs.forEach(input => {
            input.addEventListener('input', function () {
                const data = new FormData(form);
                const formData = Object.fromEntries(data.entries());
                localStorage.setItem(`autosave_${formId}`, JSON.stringify(formData));
            });
        });

        // Restore data on page load
        this.restoreAutoSaveData(form, formId);
    },

    restoreAutoSaveData: function (form, formId) {
        try {
            const savedData = localStorage.getItem(`autosave_${formId}`);
            if (savedData) {
                const data = JSON.parse(savedData);
                Object.entries(data).forEach(([name, value]) => {
                    const input = form.querySelector(`[name="${name}"]`);
                    if (input) {
                        input.value = value;
                    }
                });
            }
        } catch (e) {
            console.warn('Failed to restore auto-save data:', e);
        }
    },

    submitAjaxForm: function (form) {
        const url = form.action || window.location.href;
        const method = form.method || 'POST';
        const formData = new FormData(form);

        // Show loading state
        const submitButton = form.querySelector('button[type="submit"], input[type="submit"]');
        if (submitButton) {
            showLoading(submitButton);
        }

        fetch(url, {
            method: method,
            body: formData,
            credentials: 'same-origin'
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    showNotification(data.message || 'Form submitted successfully', 'success');
                    if (data.redirect) {
                        window.location.href = data.redirect;
                    }
                } else {
                    showNotification(data.message || 'Form submission failed', 'error');
                }
            })
            .catch(error => {
                console.error('Form submission error:', error);
                showNotification('An error occurred while submitting the form', 'error');
            })
            .finally(() => {
                if (submitButton) {
                    hideLoading(submitButton);
                }
            });
    }
};

// ===============================================
// Notifications
// ===============================================
TPA.Notifications = {
    init: function () {
        this.createContainer();
    },

    createContainer: function () {
        if (!document.getElementById('tpa-notifications')) {
            const container = document.createElement('div');
            container.id = 'tpa-notifications';
            container.className = 'notification-container';
            document.body.appendChild(container);
        }
    },

    show: function (message, type = 'info', duration = 5000) {
        const notification = document.createElement('div');
        notification.className = `notification notification-${type}`;
        notification.innerHTML = `
            <div class="notification-content">
                <i class="notification-icon">${this.getIcon(type)}</i>
                <span class="notification-message">${message}</span>
                <button class="notification-close" onclick="this.parentElement.parentElement.remove()">×</button>
            </div>
        `;

        const container = document.getElementById('tpa-notifications');
        container.appendChild(notification);

        // Auto-remove after duration
        if (duration > 0) {
            setTimeout(() => {
                if (notification.parentElement) {
                    notification.remove();
                }
            }, duration);
        }

        return notification;
    },

    getIcon: function (type) {
        const icons = {
            success: '✓',
            error: '✗',
            warning: '⚠',
            info: 'ℹ'
        };
        return icons[type] || icons.info;
    }
};

// ===============================================
// Utilities
// ===============================================
TPA.Utils = {
    init: function () {
        // Initialize utility functions
    },

    formatNumber: function (number, decimals = 0) {
        return new Intl.NumberFormat('en-US', {
            minimumFractionDigits: decimals,
            maximumFractionDigits: decimals
        }).format(number);
    },

    formatCurrency: function (amount, currency = 'USD') {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: currency
        }).format(amount);
    },

    formatDate: function (date, options = {}) {
        const defaultOptions = {
            year: 'numeric',
            month: 'short',
            day: 'numeric'
        };
        return new Intl.DateTimeFormat('en-US', { ...defaultOptions, ...options }).format(new Date(date));
    },

    formatTimeAgo: function (date) {
        const now = new Date();
        const diffInSeconds = Math.floor((now - new Date(date)) / 1000);

        if (diffInSeconds < 60) return 'Just now';
        if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)} minutes ago`;
        if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)} hours ago`;
        return `${Math.floor(diffInSeconds / 86400)} days ago`;
    },

    copyToClipboard: function (text) {
        if (navigator.clipboard) {
            navigator.clipboard.writeText(text).then(() => {
                showNotification('Copied to clipboard', 'success', 2000);
            });
        } else {
            // Fallback for older browsers
            const textArea = document.createElement('textarea');
            textArea.value = text;
            document.body.appendChild(textArea);
            textArea.select();
            document.execCommand('copy');
            document.body.removeChild(textArea);
            showNotification('Copied to clipboard', 'success', 2000);
        }
    },

    downloadFile: function (url, filename) {
        const link = document.createElement('a');
        link.href = url;
        link.download = filename || 'download';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    },

    debounce: function (func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    },

    throttle: function (func, limit) {
        let inThrottle;
        return function (...args) {
            if (!inThrottle) {
                func.apply(this, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    }
};

// ===============================================
// Global Helper Functions
// ===============================================

// Show notification (backwards compatibility)
function showNotification(message, type = 'info', duration = 5000) {
    return TPA.Notifications.show(message, type, duration);
}

// Confirm dialog with callback
function confirmAction(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

// Loading state helpers
function showLoading(element, text = 'Loading...') {
    if (element) {
        element.classList.add('loading');
        element.disabled = true;

        if (element.tagName === 'BUTTON') {
            element.setAttribute('data-original-text', element.textContent);
            element.innerHTML = `<div class="spinner"></div> ${text}`;
        }
    }
}

function hideLoading(element) {
    if (element) {
        element.classList.remove('loading');
        element.disabled = false;

        if (element.tagName === 'BUTTON' && element.hasAttribute('data-original-text')) {
            element.textContent = element.getAttribute('data-original-text');
            element.removeAttribute('data-original-text');
        }
    }
}

// Form validation helper
function validateForm(form) {
    let isValid = true;
    const requiredFields = form.querySelectorAll('[required]');

    requiredFields.forEach(field => {
        if (!TPA.Forms.validateRequired(field)) {
            isValid = false;
        }

        if (field.type === 'email' && !TPA.Forms.validateEmail(field)) {
            isValid = false;
        }
    });

    return isValid;
}

// Format numbers for display
function formatNumber(number, decimals = 0) {
    return TPA.Utils.formatNumber(number, decimals);
}

// Format currency for display
function formatCurrency(amount, currency = 'USD') {
    return TPA.Utils.formatCurrency(amount, currency);
}

// Format time ago
function timeAgo(date) {
    return TPA.Utils.formatTimeAgo(new Date(date));
}

// Refresh page data
function refreshPage() {
    if (typeof window.refreshDashboardData === 'function') {
        window.refreshDashboardData();
    } else {
        window.location.reload();
    }
}

// Copy text to clipboard
function copyToClipboard(text) {
    TPA.Utils.copyToClipboard(text);
}

// Download file
function downloadFile(url, filename) {
    TPA.Utils.downloadFile(url, filename);
}

// ===============================================
// Session Management - FIXED VERSION
// ===============================================
TPA.Session = {
    checkTimeout: function () {
        // Check if session is still valid
        fetch('/api/session/check', {
            method: 'GET',
            credentials: 'include'
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Session expired');
                }
                return response.json();
            })
            .then(data => {
                if (!data.valid) {
                    this.handleSessionExpired();
                }
            })
            .catch(() => {
                this.handleSessionExpired();
            });
    },

    handleSessionExpired: function () {
        // Show notification and redirect to actual Login.aspx file
        showNotification('Your session has expired. Please log in again.', 'warning');

        // Clear any local storage or session storage
        if (typeof (Storage) !== "undefined") {
            localStorage.clear();
            sessionStorage.clear();
        }

        setTimeout(() => {
            // FIX: Redirect to the actual Login.aspx file instead of clean URL
            // This prevents 404 errors when routing is not configured
            window.location.href = window.location.origin + '/Login.aspx?expired=true';
        }, 2000);
    },

    extend: function () {
        // Extend session timeout
        fetch('/api/session/extend', {
            method: 'POST',
            credentials: 'include'
        })
            .catch(() => {
                console.log('Failed to extend session');
            });
    }
};

// Updated: Check session on every page load but with error handling
document.addEventListener('DOMContentLoaded', function () {
    // Only check session on pages that require authentication
    const currentPage = window.location.pathname.toLowerCase();
    const publicPages = ['/login.aspx', '/error.aspx', '/notfound.aspx', '/debug.aspx'];

    // Don't check session on public pages
    if (publicPages.some(page => currentPage.includes(page))) {
        return;
    }

    // Check if user session exists in a simple way
    // This avoids complex API calls that might not be available
    const checkSessionSimple = function () {
        // Make a simple request to check if we're still logged in
        fetch(window.location.pathname, {
            method: 'HEAD',
            credentials: 'same-origin',
            cache: 'no-cache'
        })
            .then(response => {
                // If we get redirected to login page, session has expired
                if (response.url && response.url.includes('Login.aspx')) {
                    TPA.Session.handleSessionExpired();
                }
            })
            .catch(error => {
                // If there's a network error, don't force logout
                console.log('Session check failed:', error);
            });
    };

    // Initial session check
    checkSessionSimple();

    // Set up periodic session checking (every 5 minutes)
    setInterval(checkSessionSimple, 5 * 60 * 1000);
});

// Extend session on user activity
let sessionExtendTimer;
document.addEventListener('click', () => {
    clearTimeout(sessionExtendTimer);
    sessionExtendTimer = setTimeout(() => {
        TPA.Session.extend();
    }, 1000);
});

// Also extend session on keyboard activity
document.addEventListener('keypress', () => {
    clearTimeout(sessionExtendTimer);
    sessionExtendTimer = setTimeout(() => {
        TPA.Session.extend();
    }, 1000);
});

// Handle page visibility changes (user switching tabs)
document.addEventListener('visibilitychange', function () {
    if (!document.hidden) {
        // Page became visible again, but don't auto-check session
        // This prevents unnecessary requests
        console.log('Page became visible');
    }
});

// Fallback: If user tries to access a page and gets 404, redirect to login
window.addEventListener('error', function (e) {
    // Check if this might be a session-related error
    if (e.message && e.message.includes('404')) {
        console.log('Possible session timeout detected via 404 error');
        // Redirect to login as a fallback
        setTimeout(() => {
            window.location.href = '/Login.aspx?expired=true';
        }, 1000);
    }
});

// Alternative method: Check for specific error patterns that indicate session timeout
const originalFetch = window.fetch;
window.fetch = function (...args) {
    return originalFetch.apply(this, args)
        .then(response => {
            // If we get a 404 on a page that should exist, it might be session timeout
            if (response.status === 404 && !isStaticResource(args[0])) {
                console.log('404 detected on dynamic page, checking if session expired');
                // Check if we're being redirected to login
                if (response.url && response.url.includes('Login.aspx')) {
                    TPA.Session.handleSessionExpired();
                }
            }
            return response;
        })
        .catch(error => {
            console.log('Fetch error:', error);
            throw error;
        });
};

// Helper function to determine if a URL is for a static resource
function isStaticResource(url) {
    if (typeof url !== 'string') return false;

    const staticExtensions = ['.css', '.js', '.png', '.jpg', '.jpeg', '.gif', '.ico', '.svg', '.woff', '.woff2', '.ttf'];
    return staticExtensions.some(ext => url.toLowerCase().includes(ext));
}

// ===============================================
// Error Handling
// ===============================================
window.addEventListener('error', function (e) {
    console.error('Global error:', e.error);

    // Don't show notifications for minor errors
    if (e.error && e.error.message && !e.error.message.includes('Script error')) {
        showNotification('An unexpected error occurred', 'error');
    }
});

// Handle unhandled promise rejections
window.addEventListener('unhandledrejection', function (e) {
    console.error('Unhandled promise rejection:', e.reason);
    showNotification('An error occurred while processing your request', 'error');
});

// ===============================================
// Performance Monitoring
// ===============================================
TPA.Performance = {
    init: function () {
        if ('performance' in window) {
            // Log page load time
            window.addEventListener('load', () => {
                setTimeout(() => {
                    const perfData = performance.getEntriesByType('navigation')[0];
                    if (perfData) {
                        console.log(`📊 Page load time: ${Math.round(perfData.loadEventEnd - perfData.fetchStart)}ms`);
                    }
                }, 0);
            });
        }
    },

    measureFunction: function (fn, name) {
        return function (...args) {
            const start = performance.now();
            const result = fn.apply(this, args);
            const end = performance.now();
            console.log(`⏱️ ${name}: ${Math.round(end - start)}ms`);
            return result;
        };
    }
};

// Initialize performance monitoring
TPA.Performance.init();

// ===============================================
// Browser Compatibility
// ===============================================
TPA.Compatibility = {
    init: function () {
        this.checkFeatures();
        this.addPolyfills();
    },

    checkFeatures: function () {
        const required = [
            'fetch',
            'Promise',
            'localStorage',
            'sessionStorage'
        ];

        const missing = required.filter(feature => !(feature in window));

        if (missing.length > 0) {
            console.warn('Missing browser features:', missing);
            showNotification('Your browser may not support all features. Please update your browser.', 'warning');
        }
    },

    addPolyfills: function () {
        // Simple forEach polyfill for NodeList
        if (window.NodeList && !NodeList.prototype.forEach) {
            NodeList.prototype.forEach = Array.prototype.forEach;
        }

        // Simple closest polyfill
        if (!Element.prototype.closest) {
            Element.prototype.closest = function (s) {
                var el = this;
                do {
                    if (el.matches(s)) return el;
                    el = el.parentElement || el.parentNode;
                } while (el !== null && el.nodeType === 1);
                return null;
            };
        }

        // Simple matches polyfill
        if (!Element.prototype.matches) {
            Element.prototype.matches = Element.prototype.msMatchesSelector ||
                Element.prototype.webkitMatchesSelector;
        }
    }
};

// Initialize compatibility checking
TPA.Compatibility.init();

// ===============================================
// Email Validation Helper
// ===============================================
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

// Show validation message
function showValidationMessage(fieldId, message, type = 'error') {
    const field = document.getElementById(fieldId);
    if (!field) return;

    // Remove existing validation message
    const existingMessage = field.parentNode.querySelector('.validation-message');
    if (existingMessage) {
        existingMessage.remove();
    }

    // Add new validation message
    if (message) {
        const messageElement = document.createElement('div');
        messageElement.className = `validation-message text-${type}`;
        messageElement.textContent = message;
        messageElement.style.fontSize = '0.8rem';
        messageElement.style.marginTop = '0.25rem';

        field.parentNode.appendChild(messageElement);
    }
}

// Clear validation messages
function clearValidationMessages() {
    const messages = document.querySelectorAll('.validation-message');
    messages.forEach(message => message.remove());
}

// ===============================================
// SUCCESS MODAL FUNCTIONALITY
// ===============================================

// Simple Success Modal with Company Email
function showSuccessModalWithEmail(employeeNumber, employeeName, department, taskCount, companyEmail) {
    // Remove any existing modal
    const existingModal = document.querySelector('.success-modal-overlay');
    if (existingModal) {
        existingModal.remove();
    }

    // Create modal HTML
    const modalHTML = `
        <div class="success-modal-overlay" onclick="closeSuccessModal(event)">
            <div class="success-modal-dialog">
                <div class="success-modal-content">
                    <div class="success-modal-header">
                        <h2>🎉 Welcome to Tennessee Personal Assistance!</h2>
                        <button type="button" class="success-modal-close" onclick="closeSuccessModal()">&times;</button>
                    </div>
                    <div class="success-modal-body">
                        <div class="success-info">
                            <p><strong>Employee #:</strong> ${employeeNumber}</p>
                            <p><strong>Name:</strong> ${employeeName}</p>
                            <p><strong>Department:</strong> ${department}</p>
                            <p><strong>Onboarding Tasks:</strong> ${taskCount} tasks assigned</p>
                        </div>
                        <div class="email-section">
                            <h4>📧 Company Email Created</h4>
                            <div class="email-display">
                                <input type="text" value="${companyEmail}" readonly onclick="this.select()">
                                <button onclick="copyToClipboard('${companyEmail}')">Copy</button>
                            </div>
                            <p class="email-note">
                                Please save this email address. You will receive login instructions shortly.
                            </p>
                        </div>
                    </div>
                    <div class="success-modal-footer">
                        <button type="button" class="btn-primary" onclick="closeSuccessModal()">Continue</button>
                    </div>
                </div>
            </div>
        </div>
    `;

    // Add modal to page
    document.body.insertAdjacentHTML('beforeend', modalHTML);

    // Add styles if not already present
    if (!document.getElementById('success-modal-styles')) {
        const styles = document.createElement('style');
        styles.id = 'success-modal-styles';
        styles.textContent = `
            .success-modal-overlay {
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(0, 0, 0, 0.5);
                display: flex;
                justify-content: center;
                align-items: center;
                z-index: 10000;
            }
            .success-modal-dialog {
                background: white;
                border-radius: 10px;
                max-width: 500px;
                width: 90%;
                max-height: 90vh;
                overflow-y: auto;
                box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
            }
            .success-modal-header {
                background: #4CAF50;
                color: white;
                padding: 20px;
                border-radius: 10px 10px 0 0;
                position: relative;
            }
            .success-modal-header h2 {
                margin: 0;
                font-size: 1.5rem;
            }
            .success-modal-close {
                position: absolute;
                top: 15px;
                right: 20px;
                background: none;
                border: none;
                font-size: 2rem;
                color: white;
                cursor: pointer;
            }
            .success-modal-body {
                padding: 20px;
            }
            .success-info p {
                margin: 10px 0;
                font-size: 1.1rem;
            }
            .email-section {
                margin-top: 20px;
                padding: 15px;
                background: #f0f8ff;
                border-radius: 5px;
            }
            .email-section h4 {
                margin: 0 0 10px 0;
                color: #2196F3;
            }
            .email-display {
                display: flex;
                gap: 10px;
                margin: 10px 0;
            }
            .email-display input {
                flex: 1;
                padding: 10px;
                border: 1px solid #ddd;
                border-radius: 4px;
                font-family: monospace;
                background: white;
            }
            .email-display button {
                padding: 10px 15px;
                background: #2196F3;
                color: white;
                border: none;
                border-radius: 4px;
                cursor: pointer;
            }
            .email-note {
                font-size: 0.9rem;
                color: #666;
                margin: 10px 0 0 0;
            }
            .success-modal-footer {
                padding: 20px;
                text-align: center;
                border-top: 1px solid #eee;
            }
            .btn-primary {
                background: #4CAF50;
                color: white;
                border: none;
                padding: 12px 30px;
                border-radius: 5px;
                cursor: pointer;
                font-size: 1rem;
            }
        `;
        document.head.appendChild(styles);
    }
}

// Close success modal
function closeSuccessModal(event) {
    if (event && event.target !== event.currentTarget) {
        return; // Don't close if clicking inside modal
    }

    const modal = document.querySelector('.success-modal-overlay');
    if (modal) {
        modal.remove();
    }
}

// ===============================================
// Export for module systems (if needed)
// ===============================================
if (typeof module !== 'undefined' && module.exports) {
    module.exports = TPA;
}

console.log('✅ TPA Common JavaScript loaded successfully');