<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="TPASystem2.Dashboard" %>

<asp:Content ID="DashboardContent" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- Page Header -->

    <style>

      

/* Dashboard Container */
.dashboard-container {
    padding: 20px;
    max-width: 1400px;
    margin: 0 auto;
}

/* Dashboard Header */
.dashboard-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 30px;
    padding-bottom: 15px;
    border-bottom: 1px solid #e0e0e0;
}

.dashboard-title {
    font-size: 28px;
    font-weight: 600;
    color: #2c3e50;
    margin: 0;
}

.dashboard-refresh {
    display: flex;
    align-items: center;
    gap: 10px;
}

.refresh-btn {
    background: #3498db;
    color: white;
    border: none;
    padding: 8px 16px;
    border-radius: 6px;
    cursor: pointer;
    display: flex;
    align-items: center;
    gap: 5px;
    font-size: 14px;
    transition: all 0.3s ease;
}

.refresh-btn:hover {
    background: #2980b9;
    transform: translateY(-1px);
}

.refresh-btn.loading {
    opacity: 0.7;
    cursor: not-allowed;
}

.refresh-btn.loading i {
    animation: spin 1s linear infinite;
}

@keyframes spin {
    from { transform: rotate(0deg); }
    to { transform: rotate(360deg); }
}

/* Stats Grid - Exactly 4 stats per row */
.dashboard-stats {
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 20px;
    margin-bottom: 30px;
}

/* Responsive grid for smaller screens */
@media (max-width: 1200px) {
    .dashboard-stats {
        grid-template-columns: repeat(2, 1fr);
    }
}

@media (max-width: 768px) {
    .dashboard-stats {
        grid-template-columns: 1fr;
    }
}

/* Individual Stat Card - Enhanced Design */
.stat-card {
    background: linear-gradient(145deg, #ffffff, #f8f9fa);
    border-radius: 16px;
    padding: 28px 24px;
    box-shadow: 
        0 4px 20px rgba(0, 0, 0, 0.08),
        0 2px 6px rgba(0, 0, 0, 0.04);
    display: flex;
    align-items: center;
    gap: 20px;
    transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    border: 1px solid rgba(255, 255, 255, 0.8);
    position: relative;
    overflow: hidden;
    backdrop-filter: blur(10px);
}

.stat-card:hover {
    transform: translateY(-8px) scale(1.02);
    box-shadow: 
        0 12px 40px rgba(0, 0, 0, 0.12),
        0 8px 16px rgba(0, 0, 0, 0.08);
    border-color: var(--stat-color, #3498db);
}

.stat-card::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    height: 5px;
    background: linear-gradient(90deg, var(--stat-color, #3498db), var(--stat-color-light, #5dade2));
    border-radius: 16px 16px 0 0;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.stat-card::after {
    content: '';
    position: absolute;
    top: -50%;
    right: -50%;
    width: 100%;
    height: 100%;
    background: radial-gradient(circle, var(--stat-color, #3498db)10, transparent 70%);
    opacity: 0.03;
    pointer-events: none;
}

/* Stat Icon - Enhanced with Multiple Styles */
.stat-icon {
    width: 64px;
    height: 64px;
    border-radius: 16px;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    position: relative;
    overflow: hidden;
    box-shadow: 
        0 8px 24px rgba(0, 0, 0, 0.12),
        inset 0 1px 0 rgba(255, 255, 255, 0.2);
}

.stat-icon::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: linear-gradient(145deg, rgba(255, 255, 255, 0.1), rgba(255, 255, 255, 0));
    border-radius: 16px;
}

.stat-icon i {
    font-size: 32px;
    color: white;
    position: relative;
    z-index: 1;
    text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

/* Enhanced Stat Icon Colors with Gradients */
.stat-primary {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    --stat-color: #667eea;
    --stat-color-light: #764ba2;
}

.stat-success {
    background: linear-gradient(135deg, #56ab2f 0%, #a8e6cf 100%);
    --stat-color: #56ab2f;
    --stat-color-light: #a8e6cf;
}

.stat-warning {
    background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
    --stat-color: #f093fb;
    --stat-color-light: #f5576c;
}

.stat-danger {
    background: linear-gradient(135deg, #fc4a1a 0%, #f7b733 100%);
    --stat-color: #fc4a1a;
    --stat-color-light: #f7b733;
}

.stat-info {
    background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
    --stat-color: #4facfe;
    --stat-color-light: #00f2fe;
}

.stat-secondary {
    background: linear-gradient(135deg, #a8edea 0%, #fed6e3 100%);
    --stat-color: #a8edea;
    --stat-color-light: #fed6e3;
}

/* Stat Content - Enhanced Typography */
.stat-content {
    flex: 1;
    min-width: 0;
    position: relative;
}

.stat-value {
    font-size: 36px;
    font-weight: 800;
    color: #2c3e50;
    line-height: 1;
    margin-bottom: 8px;
    transition: all 0.3s ease;
    background: linear-gradient(135deg, #2c3e50, #34495e);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
    text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.stat-title {
    font-size: 17px;
    font-weight: 700;
    color: #34495e;
    margin-bottom: 4px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    letter-spacing: 0.5px;
    text-transform: uppercase;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.stat-subtitle {
    font-size: 14px;
    color: #7f8c8d;
    opacity: 0.9;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    font-weight: 500;
    position: relative;
}

.stat-subtitle::before {
    content: '•';
    color: var(--stat-color, #3498db);
    margin-right: 6px;
    font-weight: 900;
}

/* Real-time Update Animation - Enhanced */
.stat-card.updating {
    animation: updatePulse 0.6s ease-in-out;
    box-shadow: 
        0 12px 40px rgba(0, 0, 0, 0.15),
        0 0 0 3px var(--stat-color, #3498db)20;
}

.stat-card.updating .stat-value {
    animation: valueUpdate 0.6s ease-in-out;
    transform: scale(1.1);
}

@keyframes updatePulse {
    0% { transform: translateY(-8px) scale(1.02); }
    50% { transform: translateY(-12px) scale(1.05); }
    100% { transform: translateY(-8px) scale(1.02); }
}

@keyframes valueUpdate {
    0% { transform: scale(1); opacity: 1; }
    50% { transform: scale(1.15); opacity: 0.8; }
    100% { transform: scale(1.1); opacity: 1; }
}

/* Loading State - Enhanced */
.stat-card.loading {
    opacity: 0.8;
    pointer-events: none;
}

.stat-card.loading .stat-icon {
    animation: iconPulse 1.5s ease-in-out infinite;
}

.stat-card.loading .stat-value {
    background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
    background-size: 200% 100%;
    animation: loadingShimmer 1.5s infinite;
    color: transparent;
    border-radius: 6px;
    -webkit-background-clip: initial;
    -webkit-text-fill-color: transparent;
}

@keyframes iconPulse {
    0%, 100% { opacity: 0.6; transform: scale(0.95); }
    50% { opacity: 1; transform: scale(1); }
}

@keyframes loadingShimmer {
    0% { background-position: 200% 0; }
    100% { background-position: -200% 0; }
}

/* Enhanced Real-time Update Badges */
.stat-update-badge {
    position: absolute;
    top: 12px;
    right: 12px;
    width: 12px;
    height: 12px;
    background: radial-gradient(circle, #2ecc71, #27ae60);
    border-radius: 50%;
    opacity: 0;
    animation: updateBadge 2.5s ease-in-out;
    box-shadow: 
        0 0 0 3px rgba(46, 204, 113, 0.3),
        0 2px 4px rgba(0, 0, 0, 0.2);
}

@keyframes updateBadge {
    0% { opacity: 0; transform: scale(0.3); }
    20% { opacity: 1; transform: scale(1.2); }
    40% { opacity: 1; transform: scale(1); }
    100% { opacity: 0; transform: scale(0.8); }
}

/* Success/Error States for Stats */
.stat-card.success {
    border-color: #2ecc71;
    background: linear-gradient(145deg, #ffffff, #f1f9f4);
}

.stat-card.success::before {
    background: linear-gradient(90deg, #2ecc71, #27ae60);
}

.stat-card.error {
    border-color: #e74c3c;
    background: linear-gradient(145deg, #ffffff, #fdf2f2);
}

.stat-card.error::before {
    background: linear-gradient(90deg, #e74c3c, #c0392b);
}

.stat-card.error .stat-value {
    color: #e74c3c;
    background: linear-gradient(135deg, #e74c3c, #c0392b);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
}

.stat-card.error::after {
    content: '⚠';
    position: absolute;
    top: 12px;
    right: 12px;
    color: #e74c3c;
    font-size: 18px;
    animation: errorPulse 2s ease-in-out infinite;
}

@keyframes errorPulse {
    0%, 100% { opacity: 0.7; }
    50% { opacity: 1; }
}

/* Responsive Enhancements */
@media (max-width: 480px) {
    .stat-card {
        padding: 20px 16px;
        flex-direction: column;
        text-align: center;
        gap: 16px;
    }
    
    .stat-icon {
        width: 56px;
        height: 56px;
    }
    
    .stat-icon i {
        font-size: 28px;
    }
    
    .stat-value {
        font-size: 32px;
    }
    
    .stat-title {
        font-size: 16px;
        white-space: normal;
        text-align: center;
    }
    
    .stat-subtitle {
        white-space: normal;
        text-align: center;
    }
}

/* Dark Mode Support */
@media (prefers-color-scheme: dark) {
    .stat-card {
        background: linear-gradient(145deg, #2c3e50, #34495e);
        border-color: #34495e;
        color: #ecf0f1;
    }
    
    .stat-value {
        background: linear-gradient(135deg, #ecf0f1, #bdc3c7);
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
    }
    
    .stat-title {
        color: #ecf0f1;
    }
    
    .stat-subtitle {
        color: #95a5a6;
    }
    
    .stat-card:hover {
        background: linear-gradient(145deg, #34495e, #2c3e50);
    }
}

/* Premium Glass Effect (Optional) */
.stat-card.glass-effect {
    background: rgba(255, 255, 255, 0.15);
    backdrop-filter: blur(10px);
    border: 1px solid rgba(255, 255, 255, 0.2);
    box-shadow: 
        0 8px 32px rgba(0, 0, 0, 0.1),
        inset 0 1px 0 rgba(255, 255, 255, 0.2);
}

.stat-card.glass-effect:hover {
    background: rgba(255, 255, 255, 0.25);
    border-color: var(--stat-color, #3498db);
}

/* Micro-interactions */
.stat-card .stat-icon {
    transition: all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

.stat-card:hover .stat-icon {
    transform: scale(1.1) rotate(5deg);
}

.stat-card:active {
    transform: translateY(-4px) scale(0.98);
}

/* Number Counter Animation */
.stat-value.counting {
    animation: countUp 0.8s ease-out;
}

@keyframes countUp {
    0% { 
        opacity: 0.5;
        transform: translateY(20px) scale(0.8);
    }
    50% {
        opacity: 0.8;
        transform: translateY(-5px) scale(1.1);
    }
    100% {
        opacity: 1;
        transform: translateY(0) scale(1);
    }
}

/* Dashboard Content Grid */
.dashboard-content {
    display: grid;
    grid-template-columns: 2fr 1fr;
    gap: 30px;
    margin-top: 20px;
}

@media (max-width: 1024px) {
    .dashboard-content {
        grid-template-columns: 1fr;
    }
}

/* Quick Actions Section */
.quick-actions-section {
    background: white;
    border-radius: 12px;
    padding: 24px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    border: 1px solid #f0f0f0;
}

.section-title {
    font-size: 20px;
    font-weight: 600;
    color: #2c3e50;
    margin-bottom: 20px;
    display: flex;
    align-items: center;
    gap: 8px;
}

.section-title i {
    color: #3498db;
}

.quick-actions-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 16px;
}

.quick-action {
    background: #f8f9fa;
    border: 1px solid #e9ecef;
    border-radius: 8px;
    padding: 16px;
    cursor: pointer;
    transition: all 0.3s ease;
    display: flex;
    align-items: center;
    gap: 12px;
    text-decoration: none;
    color: inherit;
}

.quick-action:hover {
    background: #e9ecef;
    transform: translateY(-1px);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    text-decoration: none;
    color: inherit;
}

.action-icon {
    width: 40px;
    height: 40px;
    background: #3498db;
    border-radius: 8px;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
}

.action-icon i {
    color: white;
    font-size: 20px;
}

.action-content {
    flex: 1;
    min-width: 0;
}

.action-title {
    font-size: 14px;
    font-weight: 600;
    color: #2c3e50;
    margin-bottom: 2px;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.action-description {
    font-size: 12px;
    color: #7f8c8d;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

/* Recent Activities Section */
.recent-activities-section {
    background: white;
    border-radius: 12px;
    padding: 24px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    border: 1px solid #f0f0f0;
}

.activities-list {
    list-style: none;
    padding: 0;
    margin: 0;
    max-height: 400px;
    overflow-y: auto;
}

.activity-item {
    display: flex;
    align-items: flex-start;
    gap: 12px;
    padding: 12px 0;
    border-bottom: 1px solid #f8f9fa;
}

.activity-item:last-child {
    border-bottom: none;
}

.activity-avatar {
    width: 36px;
    height: 36px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    font-size: 14px;
    font-weight: 600;
    flex-shrink: 0;
}

.activity-content {
    flex: 1;
    min-width: 0;
}

.activity-header {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-bottom: 4px;
}

.activity-user {
    font-weight: 600;
    color: #2c3e50;
    font-size: 14px;
}

.activity-action {
    color: #7f8c8d;
    font-size: 14px;
}

.activity-new {
    background: #e74c3c;
    color: white;
    font-size: 10px;
    padding: 2px 6px;
    border-radius: 10px;
    font-weight: 600;
}

.activity-details {
    color: #34495e;
    font-size: 13px;
    margin-bottom: 4px;
    line-height: 1.4;
}

.activity-time {
    color: #95a5a6;
    font-size: 12px;
}

/* Auto-refresh Indicator */
.auto-refresh-indicator {
    position: fixed;
    top: 20px;
    right: 20px;
    background: #2ecc71;
    color: white;
    padding: 8px 16px;
    border-radius: 6px;
    font-size: 12px;
    z-index: 1000;
    opacity: 0;
    transform: translateY(-20px);
    transition: all 0.3s ease;
}

.auto-refresh-indicator.show {
    opacity: 1;
    transform: translateY(0);
}

/* Notification Toast */
.notification-toast {
    position: fixed;
    bottom: 20px;
    right: 20px;
    background: #2c3e50;
    color: white;
    padding: 12px 20px;
    border-radius: 8px;
    font-size: 14px;
    z-index: 1000;
    opacity: 0;
    transform: translateY(20px);
    transition: all 0.3s ease;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.notification-toast.show {
    opacity: 1;
    transform: translateY(0);
}

.notification-toast.success {
    background: #2ecc71;
}

.notification-toast.error {
    background: #e74c3c;
}

.notification-toast.warning {
    background: #f39c12;
}

.notification-toast.info {
    background: #3498db;
}

/* Real-time Update Badges */
.stat-update-badge {
    position: absolute;
    top: 8px;
    right: 8px;
    width: 8px;
    height: 8px;
    background: #2ecc71;
    border-radius: 50%;
    opacity: 0;
    animation: fadeInOut 2s ease-in-out;
}

@keyframes fadeInOut {
    0% { opacity: 0; transform: scale(0.5); }
    50% { opacity: 1; transform: scale(1); }
    100% { opacity: 0; transform: scale(0.5); }
}

/* Dashboard Skeleton Loading */
.skeleton-stat-card {
    background: white;
    border-radius: 12px;
    padding: 24px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    display: flex;
    align-items: center;
    gap: 16px;
    border: 1px solid #f0f0f0;
}

.skeleton-icon {
    width: 56px;
    height: 56px;
    border-radius: 12px;
    background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
    background-size: 200% 100%;
    animation: loading 1.5s infinite;
}

.skeleton-content {
    flex: 1;
}

.skeleton-value {
    height: 32px;
    width: 80px;
    background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
    background-size: 200% 100%;
    animation: loading 1.5s infinite;
    border-radius: 4px;
    margin-bottom: 8px;
}

.skeleton-title {
    height: 16px;
    width: 120px;
    background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
    background-size: 200% 100%;
    animation: loading 1.5s infinite;
    border-radius: 4px;
    margin-bottom: 4px;
}

.skeleton-subtitle {
    height: 12px;
    width: 100px;
    background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
    background-size: 200% 100%;
    animation: loading 1.5s infinite;
    border-radius: 4px;
}

/* Error State */
.stat-card.error {
    border-color: #e74c3c;
    background: #fdf2f2;
}

.stat-card.error .stat-value {
    color: #e74c3c;
}

.stat-card.error::after {
    content: '⚠';
    position: absolute;
    top: 8px;
    right: 8px;
    color: #e74c3c;
    font-size: 16px;
}

/* Connection Status Indicator */
.connection-status {
    position: fixed;
    top: 10px;
    left: 50%;
    transform: translateX(-50%);
    background: #e74c3c;
    color: white;
    padding: 8px 16px;
    border-radius: 20px;
    font-size: 12px;
    font-weight: 600;
    z-index: 1001;
    opacity: 0;
    transform: translateX(-50%) translateY(-30px);
    transition: all 0.3s ease;
}

.connection-status.show {
    opacity: 1;
    transform: translateX(-50%) translateY(0);
}

.connection-status.connected {
    background: #2ecc71;
}

.connection-status.disconnected {
    background: #e74c3c;
}

/* Mobile Optimizations */
@media (max-width: 480px) {
    .dashboard-container {
        padding: 15px;
    }
    
    .dashboard-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 15px;
    }
    
    .dashboard-title {
        font-size: 24px;
    }
    
    .stat-card {
        padding: 16px;
        flex-direction: column;
        text-align: center;
    }
    
    .stat-icon {
        width: 48px;
        height: 48px;
    }
    
    .stat-icon i {
        font-size: 24px;
    }
    
    .stat-value {
        font-size: 28px;
    }
    
    .quick-actions-grid {
        grid-template-columns: 1fr;
    }
    
    .quick-action {
        flex-direction: column;
        text-align: center;
        padding: 20px;
    }
    
    .action-title,
    .action-description {
        white-space: normal;
    }
    
    .notification-toast {
        left: 15px;
        right: 15px;
        bottom: 15px;
    }
}

/* Print Styles */
@media print {
    .dashboard-refresh,
    .quick-actions-section,
    .recent-activities-section {
        display: none;
    }
    
    .dashboard-stats {
        grid-template-columns: repeat(2, 1fr);
        gap: 15px;
    }
    
    .stat-card {
        box-shadow: none;
        border: 1px solid #ddd;
        page-break-inside: avoid;
    }
    
    .stat-card::before {
        display: none;
    }
}

/* Accessibility Improvements */
.stat-card:focus-within {
    outline: 2px solid #3498db;
    outline-offset: 2px;
}

.quick-action:focus {
    outline: 2px solid #3498db;
    outline-offset: 2px;
}

/* Reduced Motion */
@media (prefers-reduced-motion: reduce) {
    .stat-card,
    .quick-action,
    .notification-toast,
    .auto-refresh-indicator,
    .connection-status {
        transition: none;
    }
    
    .refresh-btn.loading i,
    .loading,
    .skeleton-icon,
    .skeleton-value,
    .skeleton-title,
    .skeleton-subtitle {
        animation: none;
    }
    
    .stat-card.updating .stat-value {
        animation: none;
    }
}

/* High Contrast Mode */
@media (prefers-contrast: high) {
    .stat-card {
        border: 2px solid #000;
    }
    
    .stat-value {
        color: #000;
        font-weight: 900;
    }
    
    .stat-title {
        color: #000;
        font-weight: 700;
    }
    
    .quick-action {
        border: 2px solid #000;
    }
}
    </style>
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