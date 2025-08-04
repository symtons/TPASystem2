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
    M.AutoInit();

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
        const isValid = emailRegex.test(input.value.trim());

        if (input.value.trim() && !isValid) {
            input.classList.add('invalid');
            input.classList.remove('valid');
            TPA.Forms.showFieldError(input, 'Please enter a valid email address');
        } else if (input.value.trim()) {
            input.classList.add('valid');
            input.classList.remove('invalid');
            TPA.Forms.hideFieldError(input);
        }

        return isValid;
    },

    validateRequired: function (input) {
        const isValid = input.value.trim() !== '';

        if (!isValid) {
            input.classList.add('invalid');
            input.classList.remove('valid');
            TPA.Forms.showFieldError(input, 'This field is required');
        } else {
            input.classList.add('valid');
            input.classList.remove('invalid');
            TPA.Forms.hideFieldError(input);
        }

        return isValid;
    },

    showFieldError: function (input, message) {
        // Remove existing error
        TPA.Forms.hideFieldError(input);

        // Create error element
        const errorElement = document.createElement('div');
        errorElement.className = 'form-error';
        errorElement.textContent = message;
        errorElement.setAttribute('data-error-for', input.name || input.id);

        // Insert after input
        input.parentNode.insertBefore(errorElement, input.nextSibling);
    },

    hideFieldError: function (input) {
        const errorElement = input.parentNode.querySelector(`[data-error-for="${input.name || input.id}"]`);
        if (errorElement) {
            errorElement.remove();
        }
    },

    setupAutoSave: function (form) {
        const inputs = form.querySelectorAll('input, textarea, select');
        const formId = form.id || 'autosave-form';

        // Load saved data
        TPA.Forms.loadAutoSavedData(form, formId);

        // Save on change
        inputs.forEach(input => {
            input.addEventListener('change', function () {
                TPA.Forms.autoSaveForm(form, formId);
            });
        });
    },

    autoSaveForm: function (form, formId) {
        const formData = new FormData(form);
        const data = {};

        for (let [key, value] of formData.entries()) {
            data[key] = value;
        }

        localStorage.setItem(`tpa-autosave-${formId}`, JSON.stringify(data));
        console.log('📝 Form auto-saved:', formId);
    },

    loadAutoSavedData: function (form, formId) {
        const savedData = localStorage.getItem(`tpa-autosave-${formId}`);
        if (savedData) {
            try {
                const data = JSON.parse(savedData);

                Object.keys(data).forEach(key => {
                    const input = form.querySelector(`[name="${key}"]`);
                    if (input && input.type !== 'password') {
                        input.value = data[key];
                    }
                });

                console.log('💾 Auto-saved data loaded:', formId);
            } catch (e) {
                console.error('Error loading auto-saved data:', e);
            }
        }
    },

    clearAutoSavedData: function (formId) {
        localStorage.removeItem(`tpa-autosave-${formId}`);
    },

    submitAjaxForm: function (form) {
        const formData = new FormData(form);
        const url = form.action || window.location.href;
        const method = form.method || 'POST';

        // Show loading state
        const submitButton = form.querySelector('button[type="submit"], input[type="submit"]');
        const originalText = submitButton.textContent;
        submitButton.textContent = 'Processing...';
        submitButton.disabled = true;

        fetch(url, {
            method: method,
            body: formData
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    TPA.Notifications.show(data.message || 'Operation completed successfully', 'success');

                    // Clear auto-saved data
                    if (form.hasAttribute('data-autosave')) {
                        TPA.Forms.clearAutoSavedData(form.id);
                    }

                    // Redirect if specified
                    if (data.redirect) {
                        setTimeout(() => {
                            window.location.href = data.redirect;
                        }, 1500);
                    }
                } else {
                    TPA.Notifications.show(data.message || 'An error occurred', 'error');
                }
            })
            .catch(error => {
                console.error('Form submission error:', error);
                TPA.Notifications.show('An error occurred while processing your request', 'error');
            })
            .finally(() => {
                // Restore button state
                submitButton.textContent = originalText;
                submitButton.disabled = false;
            });
    }
};






// ===============================================
// Notification System
// ===============================================
TPA.Notifications = {
    init: function () {
        // Auto-hide notifications after delay
        this.setupAutoHide();
    },

    show: function (message, type = 'info', duration = 5000) {
        // Create notification element
        const notification = document.createElement('div');
        notification.className = `global-notification ${type}`;
        notification.innerHTML = `
            <div class="notification-content">
                <i class="material-icons notification-icon">${this.getIcon(type)}</i>
                <span class="notification-text">${message}</span>
                <button type="button" class="notification-close" onclick="TPA.Notifications.hide(this.parentElement.parentElement)">
                    <i class="material-icons">close</i>
                </button>
            </div>
        `;

        // Add to page
        document.body.appendChild(notification);

        // Show with animation
        setTimeout(() => {
            notification.classList.add('show');
        }, 100);

        // Auto-hide
        if (duration > 0) {
            setTimeout(() => {
                TPA.Notifications.hide(notification);
            }, duration);
        }

        // Return notification element for manual control
        return notification;
    },

    hide: function (notification) {
        if (notification && notification.parentElement) {
            notification.classList.remove('show');
            setTimeout(() => {
                notification.remove();
            }, 300);
        }
    },

    getIcon: function (type) {
        const icons = {
            success: 'check_circle',
            error: 'error',
            warning: 'warning',
            info: 'info'
        };
        return icons[type] || icons.info;
    },

    setupAutoHide: function () {
        // Auto-hide existing notifications with timeout
        const existingNotifications = document.querySelectorAll('.global-notification');
        existingNotifications.forEach(notification => {
            setTimeout(() => {
                TPA.Notifications.hide(notification);
            }, 5000);
        });
    }
};

// ===============================================
// Utility Functions
// ===============================================
TPA.Utils = {
    init: function () {
        this.setupGlobalHandlers();
    },

    setupGlobalHandlers: function () {
        // Global click handlers
        document.addEventListener('click', function (e) {
            // Handle notification close buttons
            if (e.target.closest('.notification-close')) {
                const notification = e.target.closest('.global-notification');
                TPA.Notifications.hide(notification);
            }
        });

        // Global keyboard handlers
        document.addEventListener('keydown', function (e) {
            // ESC to close notifications
            if (e.key === 'Escape') {
                const notifications = document.querySelectorAll('.global-notification');
                notifications.forEach(notification => {
                    TPA.Notifications.hide(notification);
                });
            }
        });
    },

    formatTimeAgo: function (date) {
        const now = new Date();
        const diffInSeconds = Math.floor((now - date) / 1000);

        if (diffInSeconds < 60) {
            return 'Just now';
        } else if (diffInSeconds < 3600) {
            const minutes = Math.floor(diffInSeconds / 60);
            return `${minutes}m ago`;
        } else if (diffInSeconds < 86400) {
            const hours = Math.floor(diffInSeconds / 3600);
            return `${hours}h ago`;
        } else {
            const days = Math.floor(diffInSeconds / 86400);
            return `${days}d ago`;
        }
    },

    formatCurrency: function (amount, currency = 'USD') {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: currency
        }).format(amount);
    },

    formatNumber: function (number, decimals = 0) {
        return new Intl.NumberFormat('en-US', {
            minimumFractionDigits: decimals,
            maximumFractionDigits: decimals
        }).format(number);
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
        return function () {
            const args = arguments;
            const context = this;
            if (!inThrottle) {
                func.apply(context, args);
                inThrottle = true;
                setTimeout(() => inThrottle = false, limit);
            }
        };
    },

    copyToClipboard: function (text) {
        if (navigator.clipboard) {
            navigator.clipboard.writeText(text).then(() => {
                TPA.Notifications.show('Copied to clipboard', 'success', 2000);
            }).catch(() => {
                TPA.Notifications.show('Failed to copy to clipboard', 'error');
            });
        } else {
            // Fallback for older browsers
            const textArea = document.createElement('textarea');
            textArea.value = text;
            document.body.appendChild(textArea);
            textArea.select();

            try {
                document.execCommand('copy');
                TPA.Notifications.show('Copied to clipboard', 'success', 2000);
            } catch (err) {
                TPA.Notifications.show('Failed to copy to clipboard', 'error');
            }

            document.body.removeChild(textArea);
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

    isValidEmail: function (email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    },

    isValidPhone: function (phone) {
        const phoneRegex = /^[\+]?[1-9][\d]{0,15}$/;
        return phoneRegex.test(phone.replace(/\s+/g, ''));
    },

    sanitizeHtml: function (html) {
        const div = document.createElement('div');
        div.textContent = html;
        return div.innerHTML;
    }
};

// ===============================================
// API Helper
// ===============================================
TPA.API = {
    baseUrl: '',

    request: async function (endpoint, options = {}) {
        const url = `${this.baseUrl}${endpoint}`;
        const defaultOptions = {
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            }
        };

        const config = { ...defaultOptions, ...options };

        try {
            const response = await fetch(url, config);
            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || `HTTP error! status: ${response.status}`);
            }

            return data;
        } catch (error) {
            console.error('API request failed:', error);
            throw error;
        }
    },

    get: function (endpoint) {
        return this.request(endpoint, { method: 'GET' });
    },

    post: function (endpoint, data) {
        return this.request(endpoint, {
            method: 'POST',
            body: JSON.stringify(data)
        });
    },

    put: function (endpoint, data) {
        return this.request(endpoint, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    },

    delete: function (endpoint) {
        return this.request(endpoint, { method: 'DELETE' });
    }
};

// ===============================================
// Global Helper Functions
// ===============================================

// Global notification function for easy access
function showNotification(message, type = 'info', duration = 5000) {
    return TPA.Notifications.show(message, type, duration);
}

// Global notification hide function
function hideGlobalNotification() {
    const notifications = document.querySelectorAll('.global-notification');
    notifications.forEach(notification => {
        TPA.Notifications.hide(notification);
    });
}

// Confirm dialog wrapper
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

// Session management
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
        showNotification('Your session has expired. Please log in again.', 'warning');
        setTimeout(() => {
            window.location.href = '/login?expired=true';
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

// Initialize session checking (every 5 minutes)
setInterval(() => {
    TPA.Session.checkTimeout();
}, 5 * 60 * 1000);

// Extend session on user activity
let sessionExtendTimer;
document.addEventListener('click', () => {
    clearTimeout(sessionExtendTimer);
    sessionExtendTimer = setTimeout(() => {
        TPA.Session.extend();
    }, 1000);
});

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
// Export for module systems (if needed)
// ===============================================
if (typeof module !== 'undefined' && module.exports) {
    module.exports = TPA;
}

console.log('✅ TPA Common JavaScript loaded successfully');

/* ===============================================
   ENHANCED SUCCESS MODAL JAVASCRIPT - ADD TO tpa-common.js
   =============================================== */

// Enhanced Success Modal with Company Email
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
                        <div class="success-icon">
                            <i class="material-icons">check_circle</i>
                        </div>
                        <h2 class="success-modal-title">Employee Created Successfully!</h2>
                        <p class="success-modal-subtitle">Welcome to the Tennessee Personal Assistance team</p>
                    </div>
                    
                    <div class="success-modal-body">
                        <div class="employee-details">
                            <div class="detail-row">
                                <span class="detail-label">
                                    <i class="material-icons">badge</i>
                                    Employee Number
                                </span>
                                <span class="detail-value">${employeeNumber}</span>
                            </div>
                            
                            <div class="detail-row">
                                <span class="detail-label">
                                    <i class="material-icons">person</i>
                                    Full Name
                                </span>
                                <span class="detail-value">${employeeName}</span>
                            </div>
                            
                            <div class="detail-row">
                                <span class="detail-label">
                                    <i class="material-icons">business</i>
                                    Department
                                </span>
                                <span class="detail-value">${department}</span>
                            </div>
                            
                            <div class="detail-row">
                                <span class="detail-label">
                                    <i class="material-icons">email</i>
                                    Company Email
                                </span>
                                <span class="detail-value">
                                    <span class="company-email-highlight">${companyEmail}</span>
                                </span>
                            </div>
                            
                            <div class="detail-row">
                                <span class="detail-label">
                                    <i class="material-icons">assignment</i>
                                    Onboarding Tasks
                                </span>
                                <span class="detail-value">${taskCount} tasks assigned</span>
                            </div>
                        </div>
                        
                        <div class="next-steps">
                            <h4>
                                <i class="material-icons">info</i>
                                What happens next?
                            </h4>
                            <ul>
                                <li><strong>Welcome Email Sent:</strong> Login credentials sent to their personal email</li>
                                <li><strong>Onboarding Process:</strong> ${taskCount} tasks automatically assigned</li>
                                <li><strong>System Access:</strong> Employee can log in using their company email</li>
                                <li><strong>HR Review:</strong> Monitor onboarding progress in the HR dashboard</li>
                            </ul>
                        </div>
                    </div>
                    
                    <div class="success-modal-footer">
                        <div class="email-sent-notice">
                            <i class="material-icons">mark_email_read</i>
                            Welcome email sent to personal email
                        </div>
                        
                        <div class="modal-actions">
                            <button class="btn-modal btn-modal-secondary" onclick="closeSuccessModal()">
                                Close
                            </button>
                            <a href="/HR/Employees.aspx" class="btn-modal btn-modal-primary">
                                View All Employees
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;

    // Add modal to page
    document.body.insertAdjacentHTML('beforeend', modalHTML);

    // Prevent body scroll
    document.body.style.overflow = 'hidden';

    // Auto-close after 30 seconds
    setTimeout(() => {
        closeSuccessModal();
    }, 30000);
}

// Legacy success modal function (keep for compatibility)
function showSuccessModal(employeeNumber, employeeName, department, taskCount) {
    showSuccessModalWithEmail(employeeNumber, employeeName, department, taskCount, 'Not specified');
}

// Close success modal
function closeSuccessModal(event) {
    // Only close if clicking overlay or close button
    if (event && event.target.closest('.success-modal-content') && !event.target.closest('.btn-modal')) {
        return;
    }

    const modal = document.querySelector('.success-modal-overlay');
    if (modal) {
        modal.style.opacity = '0';
        modal.style.transform = 'scale(0.95)';

        setTimeout(() => {
            modal.remove();
            document.body.style.overflow = '';
        }, 200);
    }
}

// Add keyboard support
document.addEventListener('keydown', function (event) {
    if (event.key === 'Escape') {
        closeSuccessModal();
    }
});

/* ===============================================
   EMAIL VALIDATION HELPER FUNCTIONS
   =============================================== */

// Generate company email preview
function generateEmailPreview() {
    const firstName = document.getElementById('txtFirstName')?.value || '';
    const lastName = document.getElementById('txtLastName')?.value || '';

    if (firstName && lastName) {
        const cleanLastName = lastName.replace(/[^a-zA-Z0-9]/g, '').toLowerCase();
        const firstInitial = firstName.substring(0, 1).replace(/[^a-zA-Z0-9]/g, '').toLowerCase();

        return `${cleanLastName}${firstInitial}@tennesseepersonalassistance.org`;
    }

    return '';
}

// Show email preview in real-time
function updateEmailPreview() {
    const preview = generateEmailPreview();
    const previewElement = document.getElementById('emailPreview');

    if (previewElement && preview) {
        previewElement.textContent = preview;
        previewElement.style.display = 'block';
    } else if (previewElement) {
        previewElement.style.display = 'none';
    }
}

// Add event listeners when page loads
document.addEventListener('DOMContentLoaded', function () {
    const firstNameField = document.getElementById('txtFirstName');
    const lastNameField = document.getElementById('txtLastName');

    if (firstNameField && lastNameField) {
        firstNameField.addEventListener('input', updateEmailPreview);
        lastNameField.addEventListener('input', updateEmailPreview);
    }
});

/* ===============================================
   FORM VALIDATION HELPERS
   =============================================== */

// Validate email format
function validateEmailFormat(email) {
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

/* ===============================================
   SIMPLE SUCCESS MODAL JAVASCRIPT - ADD TO tpa-common.js
   =============================================== */

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
                        <div class="success-icon">
                            <i class="material-icons">check_circle</i>
                        </div>
                        <h2 class="success-modal-title">Employee Created Successfully!</h2>
                    </div>
                    
                    <div class="success-modal-body">
                        <div class="employee-summary">
                            <div class="summary-item">
                                <span class="label">Employee:</span>
                                <span class="value">${employeeName} (${employeeNumber})</span>
                            </div>
                            
                            <div class="summary-item">
                                <span class="label">Department:</span>
                                <span class="value">${department}</span>
                            </div>
                            
                            <div class="summary-item">
                                <span class="label">Company Email:</span>
                                <span class="value highlight">${companyEmail}</span>
                            </div>
                            
                            <div class="summary-item">
                                <span class="label">Default Password:</span>
                                <span class="value highlight">test123</span>
                            </div>
                            
                            <div class="summary-item">
                                <span class="label">Onboarding Tasks:</span>
                                <span class="value">${taskCount} tasks assigned</span>
                            </div>
                        </div>
                        
                        <div class="next-steps">
                            <p><strong>✉️ Welcome email sent</strong> to their personal email with login credentials.</p>
                            <p>The employee can now log in using their company email and the default password.</p>
                        </div>
                    </div>
                    
                    <div class="success-modal-footer">
                        <button class="btn-modal btn-secondary" onclick="closeSuccessModal()">
                            Close
                        </button>
                        <a href="/HR/Employees.aspx" class="btn-modal btn-primary">
                            View All Employees
                        </a>
                    </div>
                </div>
            </div>
        </div>
    `;

    // Add modal to page
    document.body.insertAdjacentHTML('beforeend', modalHTML);

    // Prevent body scroll
    document.body.style.overflow = 'hidden';

    // Auto-close after 30 seconds
    setTimeout(() => {
        closeSuccessModal();
    }, 30000);
}

// Legacy success modal function (keep for compatibility)
function showSuccessModal(employeeNumber, employeeName, department, taskCount) {
    showSuccessModalWithEmail(employeeNumber, employeeName, department, taskCount, 'Not specified');
}

// Close success modal
function closeSuccessModal(event) {
    // Only close if clicking overlay or close button
    if (event && event.target.closest('.success-modal-content') && !event.target.closest('.btn-modal')) {
        return;
    }

    const modal = document.querySelector('.success-modal-overlay');
    if (modal) {
        modal.style.opacity = '0';
        modal.style.transform = 'scale(0.95)';

        setTimeout(() => {
            modal.remove();
            document.body.style.overflow = '';
        }, 200);
    }
}

// Add keyboard support
document.addEventListener('keydown', function (event) {
    if (event.key === 'Escape') {
        closeSuccessModal();
    }
});

/* ===============================================
   EMAIL PREVIEW FUNCTIONS
   =============================================== */

// Generate company email preview
function generateEmailPreview() {
    const firstName = document.getElementById('txtFirstName')?.value || '';
    const lastName = document.getElementById('txtLastName')?.value || '';

    if (firstName && lastName) {
        const cleanLastName = lastName.replace(/[^a-zA-Z0-9]/g, '').toLowerCase();
        const firstInitial = firstName.substring(0, 1).replace(/[^a-zA-Z0-9]/g, '').toLowerCase();

        return `${cleanLastName}${firstInitial}@tennesseepersonalassistance.org`;
    }

    return '';
}

// Show email preview in real-time
function updateEmailPreview() {
    const preview = generateEmailPreview();
    const previewElement = document.getElementById('emailPreview');

    if (previewElement && preview) {
        previewElement.textContent = preview;
        previewElement.style.display = 'block';
    } else if (previewElement) {
        previewElement.style.display = 'none';
    }
}

// Add event listeners when page loads
document.addEventListener('DOMContentLoaded', function () {
    const firstNameField = document.getElementById('txtFirstName');
    const lastNameField = document.getElementById('txtLastName');

    if (firstNameField && lastNameField) {
        firstNameField.addEventListener('input', updateEmailPreview);
        lastNameField.addEventListener('input', updateEmailPreview);
    }
});

