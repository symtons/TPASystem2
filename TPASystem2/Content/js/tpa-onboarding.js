/* ===============================================
   TPA HR System - Employee Onboarding JavaScript
   File: Content/js/tpa-onboarding.js
   =============================================== */

// Namespace for onboarding functionality
window.TPAOnboarding = window.TPAOnboarding || {};

(function () {
    'use strict';

    // ===============================================
    // Notification System
    // ===============================================

    function showNotification(message, type = 'info', duration = 5000) {
        // Remove existing notifications
        const existingNotifications = document.querySelectorAll('.notification');
        existingNotifications.forEach(notif => notif.remove());

        // Create notification element
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.innerHTML = `
            <div style="display: flex; align-items: center; gap: 0.5rem;">
                <i class="material-icons">${getNotificationIcon(type)}</i>
                <span>${message}</span>
                <button onclick="this.parentElement.parentElement.remove()" style="background: none; border: none; color: inherit; margin-left: auto; cursor: pointer; padding: 0;">
                    <i class="material-icons" style="font-size: 18px;">close</i>
                </button>
            </div>
        `;

        // Add to page
        document.body.appendChild(notification);

        // Auto-remove after duration
        if (duration > 0) {
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.style.animation = 'slideOutRight 0.3s ease forwards';
                    setTimeout(() => notification.remove(), 300);
                }
            }, duration);
        }
    }

    function getNotificationIcon(type) {
        switch (type) {
            case 'success': return 'check_circle';
            case 'error': return 'error';
            case 'warning': return 'warning';
            case 'info':
            default: return 'info';
        }
    }

    // ===============================================
    // Progress Ring Animation
    // ===============================================

    function animateProgressRing(element, percentage, duration = 1000) {
        const circle = element.querySelector('.progress-ring-fill');
        const percentageText = element.querySelector('.progress-percentage');

        if (!circle || !percentageText) return;

        const radius = 52;
        const circumference = 2 * Math.PI * radius;

        // Set initial state
        circle.style.strokeDasharray = `${circumference} ${circumference}`;
        circle.style.strokeDashoffset = circumference;

        // Animate to target
        let start = null;
        const startPercentage = 0;

        function animate(timestamp) {
            if (!start) start = timestamp;
            const progress = Math.min((timestamp - start) / duration, 1);

            const currentPercentage = startPercentage + (percentage - startPercentage) * easeOutCubic(progress);
            const offset = circumference - (currentPercentage / 100) * circumference;

            circle.style.strokeDashoffset = offset;
            percentageText.textContent = Math.round(currentPercentage) + '%';

            if (progress < 1) {
                requestAnimationFrame(animate);
            }
        }

        requestAnimationFrame(animate);
    }

    function easeOutCubic(t) {
        return 1 - Math.pow(1 - t, 3);
    }

    // ===============================================
    // Task Filtering
    // ===============================================

    function initializeTaskFilters() {
        const filterButtons = document.querySelectorAll('.filter-btn');
        const taskCards = document.querySelectorAll('.task-card');

        filterButtons.forEach(button => {
            button.addEventListener('click', function (e) {
                e.preventDefault();

                const filterType = this.getAttribute('onclick');
                if (!filterType) return;

                // Update active state
                const parentGroup = this.closest('.filter-group');
                if (parentGroup) {
                    parentGroup.querySelectorAll('.filter-btn').forEach(btn =>
                        btn.classList.remove('active'));
                    this.classList.add('active');
                }

                // Apply filter with animation
                applyFilterWithAnimation(taskCards, filterType);
            });
        });
    }

    function applyFilterWithAnimation(cards, filterType) {
        // First, fade out all cards
        cards.forEach(card => {
            card.style.transition = 'opacity 0.2s ease, transform 0.2s ease';
            card.style.opacity = '0';
            card.style.transform = 'translateY(-10px)';
        });

        // After fade out, apply filter and fade in matching cards
        setTimeout(() => {
            cards.forEach(card => {
                const shouldShow = shouldCardShow(card, filterType);

                if (shouldShow) {
                    card.style.display = 'block';
                    setTimeout(() => {
                        card.style.opacity = '1';
                        card.style.transform = 'translateY(0)';
                    }, 50);
                } else {
                    card.style.display = 'none';
                }
            });
        }, 200);
    }

    function shouldCardShow(card, filterType) {
        if (filterType.includes('filterTasks')) {
            const status = filterType.match(/'([^']+)'/)[1];
            if (status === 'all') return true;
            if (status === 'pending') return card.dataset.status !== 'COMPLETED';
            if (status === 'completed') return card.dataset.status === 'COMPLETED';
        } else if (filterType.includes('filterByPriority')) {
            const priority = filterType.match(/'([^']+)'/)[1];
            if (priority === 'all') return true;
            return card.dataset.priority === priority;
        }
        return true;
    }

    // ===============================================
    // Task Completion
    // ===============================================

    function initializeTaskCompletion() {
        const completeButtons = document.querySelectorAll('.btn-task-complete');

        completeButtons.forEach(button => {
            button.addEventListener('click', function (e) {
                e.preventDefault();

                const taskId = this.getAttribute('onclick').match(/\d+/)[0];
                if (taskId) {
                    showTaskCompletionConfirmation(taskId, this);
                }
            });
        });
    }

    function showTaskCompletionConfirmation(taskId, button) {
        const taskCard = button.closest('.task-card');
        const taskTitle = taskCard.querySelector('.task-title').textContent;

        const confirmation = document.createElement('div');
        confirmation.className = 'modal';
        confirmation.innerHTML = `
            <div class="modal-content">
                <div class="modal-header">
                    <h3><i class="material-icons">help_outline</i>Confirm Task Completion</h3>
                </div>
                <div class="modal-body">
                    <p>Are you sure you want to mark "<strong>${taskTitle}</strong>" as complete?</p>
                    <p class="text-muted">This action cannot be undone.</p>
                </div>
                <div class="modal-footer">
                    <button class="btn btn-outline" onclick="this.closest('.modal').remove()">Cancel</button>
                    <button class="btn btn-success" onclick="confirmTaskCompletion(${taskId}, this.closest('.modal'))">
                        <i class="material-icons">check</i>
                        Mark Complete
                    </button>
                </div>
            </div>
        `;

        document.body.appendChild(confirmation);
        confirmation.style.display = 'block';

        // Focus the confirm button
        setTimeout(() => {
            const confirmBtn = confirmation.querySelector('.btn-success');
            if (confirmBtn) confirmBtn.focus();
        }, 100);
    }

    window.confirmTaskCompletion = function (taskId, modal) {
        // Show loading state
        const confirmBtn = modal.querySelector('.btn-success');
        confirmBtn.innerHTML = '<i class="material-icons">hourglass_empty</i>Completing...';
        confirmBtn.disabled = true;

        // Trigger postback
        __doPostBack('ctl00$DashboardContent$rptTasks', 'COMPLETE_TASK:' + taskId);

        // Remove modal
        modal.remove();
    };

    // ===============================================
    // Keyboard Navigation
    // ===============================================

    function initializeKeyboardNavigation() {
        document.addEventListener('keydown', function (e) {
            // ESC key closes modals
            if (e.key === 'Escape') {
                const openModal = document.querySelector('.modal[style*="block"]');
                if (openModal) {
                    openModal.remove();
                }
            }

            // Enter key on task cards
            if (e.key === 'Enter' && e.target.classList.contains('task-card')) {
                const completeBtn = e.target.querySelector('.btn-task-complete');
                if (completeBtn) {
                    completeBtn.click();
                }
            }
        });
    }

    // ===============================================
    // Accessibility Enhancements
    // ===============================================

    function enhanceAccessibility() {
        // Add ARIA labels to progress ring
        const progressRing = document.querySelector('.progress-ring');
        if (progressRing) {
            const percentage = document.querySelector('.progress-percentage').textContent;
            progressRing.setAttribute('role', 'progressbar');
            progressRing.setAttribute('aria-label', `Onboarding progress: ${percentage}`);
            progressRing.setAttribute('aria-valuenow', percentage.replace('%', ''));
            progressRing.setAttribute('aria-valuemin', '0');
            progressRing.setAttribute('aria-valuemax', '100');
        }

        // Add ARIA labels to task cards
        const taskCards = document.querySelectorAll('.task-card');
        taskCards.forEach((card, index) => {
            const title = card.querySelector('.task-title').textContent;
            const status = card.dataset.status;
            card.setAttribute('role', 'article');
            card.setAttribute('aria-label', `Task ${index + 1}: ${title}, Status: ${status}`);
            card.setAttribute('tabindex', '0');
        });

        // Add screen reader announcements for filter changes
        const filterButtons = document.querySelectorAll('.filter-btn');
        filterButtons.forEach(button => {
            button.addEventListener('click', function () {
                const filterType = this.textContent.trim();
                announceToScreenReader(`Filter changed to ${filterType}`);
            });
        });
    }

    function announceToScreenReader(message) {
        const announcement = document.createElement('div');
        announcement.setAttribute('aria-live', 'polite');
        announcement.setAttribute('aria-atomic', 'true');
        announcement.className = 'sr-only';
        announcement.textContent = message;

        document.body.appendChild(announcement);

        setTimeout(() => {
            document.body.removeChild(announcement);
        }, 1000);
    }

    // ===============================================
    // Responsive Enhancements
    // ===============================================

    function initializeResponsiveFeatures() {
        // Mobile-friendly filter toggles
        const filterGroups = document.querySelectorAll('.filter-group');

        if (window.innerWidth <= 768) {
            filterGroups.forEach(group => {
                const label = group.querySelector('.filter-label');
                if (label) {
                    label.addEventListener('click', function () {
                        const buttons = group.querySelector('.filter-buttons');
                        buttons.style.display = buttons.style.display === 'none' ? 'flex' : 'none';
                    });
                }
            });
        }

        // Responsive progress ring sizing
        const progressRing = document.querySelector('.progress-ring');
        if (progressRing && window.innerWidth <= 480) {
            progressRing.style.width = '100px';
            progressRing.style.height = '100px';
        }
    }

    // ===============================================
    // Animation Observers
    // ===============================================

    function initializeAnimationObservers() {
        if ('IntersectionObserver' in window) {
            const observerOptions = {
                threshold: 0.1,
                rootMargin: '0px 0px -50px 0px'
            };

            const observer = new IntersectionObserver((entries) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        entry.target.classList.add('fade-in');

                        // Special handling for progress ring
                        if (entry.target.classList.contains('progress-ring')) {
                            const percentage = parseInt(document.querySelector('.progress-percentage').textContent);
                            setTimeout(() => animateProgressRing(entry.target, percentage), 200);
                        }
                    }
                });
            }, observerOptions);

            // Observe task cards and progress elements
            document.querySelectorAll('.task-card, .progress-ring').forEach(el => {
                observer.observe(el);
            });
        }
    }

    // ===============================================
    // Public API
    // ===============================================

    TPAOnboarding = {
        showNotification: showNotification,
        animateProgressRing: animateProgressRing,
        init: function () {
            // Wait for DOM to be ready
            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', this.init.bind(this));
                return;
            }

            // Initialize all components
            initializeTaskFilters();
            initializeTaskCompletion();
            initializeKeyboardNavigation();
            enhanceAccessibility();
            initializeResponsiveFeatures();
            initializeAnimationObservers();

            console.log('TPA Onboarding system initialized');
        }
    };

    // Auto-initialize
    TPAOnboarding.init();

    // Export for global access
    window.showNotification = showNotification;

})();