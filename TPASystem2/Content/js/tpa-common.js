// TPA Common JavaScript Functions
// Utility functions without session management

// Utility Functions
const TPAUtils = {
    // Format currency
    formatCurrency: function (amount) {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: 'USD'
        }).format(amount);
    },

    // Format date
    formatDate: function (date, format = 'MM/dd/yyyy') {
        if (!date) return '';
        const d = new Date(date);
        if (isNaN(d.getTime())) return '';

        const month = String(d.getMonth() + 1).padStart(2, '0');
        const day = String(d.getDate()).padStart(2, '0');
        const year = d.getFullYear();

        switch (format) {
            case 'MM/dd/yyyy':
                return `${month}/${day}/${year}`;
            case 'yyyy-MM-dd':
                return `${year}-${month}-${day}`;
            case 'MMM dd, yyyy':
                return d.toLocaleDateString('en-US', {
                    month: 'short',
                    day: 'numeric',
                    year: 'numeric'
                });
            default:
                return d.toLocaleDateString();
        }
    },

    // Validate email
    validateEmail: function (email) {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    },

    // Show loading state
    showLoading: function (element, text = 'Loading...') {
        if (element) {
            element.disabled = true;
            element.innerHTML = `<i class="material-icons spin">autorenew</i> ${text}`;
        }
    },

    // Hide loading state
    hideLoading: function (element, originalText) {
        if (element) {
            element.disabled = false;
            element.innerHTML = originalText;
        }
    },

    // Debounce function
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

    // Generate random ID
    generateId: function (prefix = 'tpa') {
        return `${prefix}_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
    }
};

// Alert System
function showAlert(message, type = 'info', duration = 5000) {
    // Create alert container if it doesn't exist
    let container = document.getElementById('alert-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'alert-container';
        container.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 9999;
            max-width: 400px;
        `;
        document.body.appendChild(container);
    }

    // Create alert element
    const alert = document.createElement('div');
    alert.className = `alert alert-${type}`;
    alert.style.cssText = `
        padding: 15px 20px;
        margin-bottom: 10px;
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        background: white;
        border-left: 4px solid ${getAlertColor(type)};
        animation: slideIn 0.3s ease-out;
        position: relative;
        word-wrap: break-word;
    `;

    alert.innerHTML = `
        <div style="display: flex; align-items: center; gap: 10px;">
            <i class="material-icons" style="color: ${getAlertColor(type)}; font-size: 20px;">
                ${getAlertIcon(type)}
            </i>
            <span style="flex: 1; color: #374151; font-weight: 500;">${message}</span>
            <button onclick="this.parentElement.parentElement.remove()" 
                    style="background: none; border: none; color: #9ca3af; cursor: pointer; padding: 0; margin-left: 10px;">
                <i class="material-icons" style="font-size: 18px;">close</i>
            </button>
        </div>
    `;

    container.appendChild(alert);

    // Auto remove after duration
    if (duration > 0) {
        setTimeout(() => {
            if (alert.parentNode) {
                alert.style.animation = 'slideOut 0.3s ease-in';
                setTimeout(() => alert.remove(), 300);
            }
        }, duration);
    }
}

function getAlertColor(type) {
    const colors = {
        'success': '#10b981',
        'error': '#ef4444',
        'warning': '#f59e0b',
        'info': '#3b82f6'
    };
    return colors[type] || colors.info;
}

function getAlertIcon(type) {
    const icons = {
        'success': 'check_circle',
        'error': 'error',
        'warning': 'warning',
        'info': 'info'
    };
    return icons[type] || icons.info;
}

// Add CSS animations
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOut {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(100%);
            opacity: 0;
        }
    }
    
    .spin {
        animation: spin 1s linear infinite;
    }
    
    @keyframes spin {
        from { transform: rotate(0deg); }
        to { transform: rotate(360deg); }
    }
`;
document.head.appendChild(style);

// Form Validation Helpers
const FormValidator = {
    required: function (value, fieldName) {
        if (!value || value.trim() === '') {
            return `${fieldName} is required.`;
        }
        return null;
    },

    email: function (value) {
        if (value && !TPAUtils.validateEmail(value)) {
            return 'Please enter a valid email address.';
        }
        return null;
    },

    minLength: function (value, minLength, fieldName) {
        if (value && value.length < minLength) {
            return `${fieldName} must be at least ${minLength} characters long.`;
        }
        return null;
    },

    validateForm: function (formData, rules) {
        const errors = [];

        for (const field in rules) {
            const value = formData[field];
            const fieldRules = rules[field];

            for (const rule of fieldRules) {
                const error = this[rule.type](value, rule.param, rule.fieldName || field);
                if (error) {
                    errors.push(error);
                    break; // Stop at first error for this field
                }
            }
        }

        return errors;
    }
};

// Page Enhancement Functions
const PageEnhancer = {
    // Initialize Material Design components
    initMaterialComponents: function () {
        // Initialize tooltips if Materialize is available
        if (typeof M !== 'undefined' && M.Tooltip) {
            M.Tooltip.init(document.querySelectorAll('.tooltipped'));
        }

        // Initialize dropdowns if Materialize is available
        if (typeof M !== 'undefined' && M.Dropdown) {
            M.Dropdown.init(document.querySelectorAll('.dropdown-trigger'));
        }

        // Initialize modals if Materialize is available
        if (typeof M !== 'undefined' && M.Modal) {
            M.Modal.init(document.querySelectorAll('.modal'));
        }
    },

    // Add smooth scrolling to anchor links
    initSmoothScrolling: function () {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                e.preventDefault();
                const targetId = this.getAttribute('href').substring(1);
                const targetElement = document.getElementById(targetId);
                if (targetElement) {
                    targetElement.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    },

    // Add loading states to forms
    initFormLoadingStates: function () {
        document.querySelectorAll('form').forEach(form => {
            form.addEventListener('submit', function () {
                const submitButton = form.querySelector('button[type="submit"], input[type="submit"]');
                if (submitButton) {
                    TPAUtils.showLoading(submitButton, 'Processing...');
                }
            });
        });
    },

    // Initialize all page enhancements
    init: function () {
        this.initMaterialComponents();
        this.initSmoothScrolling();
        this.initFormLoadingStates();
    }
};

// Table Enhancement Functions
const TableEnhancer = {
    // Make tables responsive
    makeTablesResponsive: function () {
        document.querySelectorAll('table:not(.no-responsive)').forEach(table => {
            if (!table.parentElement.classList.contains('table-responsive')) {
                const wrapper = document.createElement('div');
                wrapper.className = 'table-responsive';
                table.parentNode.insertBefore(wrapper, table);
                wrapper.appendChild(table);
            }
        });
    },

    // Add sorting to table headers
    initTableSorting: function () {
        document.querySelectorAll('table.sortable th[data-sort]').forEach(header => {
            header.style.cursor = 'pointer';
            header.addEventListener('click', function () {
                const table = this.closest('table');
                const column = this.dataset.sort;
                const sortOrder = this.dataset.sortOrder === 'asc' ? 'desc' : 'asc';

                // Update all headers
                table.querySelectorAll('th[data-sort]').forEach(th => {
                    th.dataset.sortOrder = '';
                    th.classList.remove('sort-asc', 'sort-desc');
                });

                // Update current header
                this.dataset.sortOrder = sortOrder;
                this.classList.add(`sort-${sortOrder}`);

                // Sort table rows
                TableEnhancer.sortTable(table, column, sortOrder);
            });
        });
    },

    sortTable: function (table, column, order) {
        const tbody = table.querySelector('tbody');
        const rows = Array.from(tbody.querySelectorAll('tr'));

        rows.sort((a, b) => {
            const aValue = a.querySelector(`[data-sort-value="${column}"]`)?.textContent ||
                a.children[parseInt(column)]?.textContent || '';
            const bValue = b.querySelector(`[data-sort-value="${column}"]`)?.textContent ||
                b.children[parseInt(column)]?.textContent || '';

            const comparison = aValue.localeCompare(bValue, undefined, { numeric: true });
            return order === 'asc' ? comparison : -comparison;
        });

        rows.forEach(row => tbody.appendChild(row));
    },

    init: function () {
        this.makeTablesResponsive();
        this.initTableSorting();
    }
};

// Page Load Handler
document.addEventListener('DOMContentLoaded', function () {
    // Initialize page enhancements
    PageEnhancer.init();
    TableEnhancer.init();

    // Add any additional initialization here
    console.log('TPA Common JavaScript loaded successfully');
});

// Export for global use
window.TPAUtils = TPAUtils;
window.FormValidator = FormValidator;
window.PageEnhancer = PageEnhancer;
window.TableEnhancer = TableEnhancer;