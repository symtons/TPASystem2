<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="MyProfile.aspx.cs" Inherits="TPASystem2.Profile.MyProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
<style>

    /* ===============================================
   My Profile Page Styles
   Add these styles to tpa-common.css
   =============================================== */

/* Profile Container */
.profile-container {
    padding: 2rem 0;
    max-width: 1200px;
    margin: 0 auto;
}

.profile-layout {
    display: grid;
    grid-template-columns: 1fr 2fr;
    gap: 2rem;
    align-items: start;
}

/* Profile Summary Card - Ensure Light Background */
.profile-summary-card {
    background: white !important;
    border-radius: var(--border-radius-large);
    padding: 2rem;
    box-shadow: var(--shadow-light);
    border: 1px solid var(--border-light);
    position: sticky;
    top: 2rem;
}

.profile-avatar-section {
    display: flex;
    flex-direction: column;
    align-items: center;
    text-align: center;
    margin-bottom: 2rem;
    padding-bottom: 2rem;
    border-bottom: 1px solid var(--border-light);
    background: transparent;
}

.profile-avatar-large {
    width: 120px;
    height: 120px;
    border-radius: 50%;
    background: linear-gradient(135deg, var(--tpa-primary), var(--tpa-primary-dark)) !important;
    color: white;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 2.5rem;
    font-weight: bold;
    margin-bottom: 1rem;
    border: 4px solid white;
    box-shadow: 0 4px 20px rgba(59, 130, 246, 0.3);
}

.profile-avatar-info {
    background: transparent;
}

.profile-avatar-info h3 {
    margin: 0 0 0.5rem 0;
    color: var(--text-primary);
    font-size: 1.5rem;
    font-weight: 600;
    background: transparent;
}

.profile-title {
    color: var(--text-secondary);
    margin: 0 0 1rem 0;
    font-size: 1rem;
    background: transparent;
}

.profile-badges {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
    background: transparent;
}

.profile-badge {
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    padding: 0.5rem 1rem;
    background: var(--background-light) !important;
    border: 1px solid var(--border-light);
    border-radius: 20px;
    font-size: 0.9rem;
    font-weight: 500;
    color: var(--text-secondary);
}

.profile-badge.status-active {
    background: #dcfce7 !important;
    color: #16a34a;
    border-color: #bbf7d0;
}

.profile-badge .material-icons {
    font-size: 1rem;
}

/* Quick Stats - Ensure Light Background */
.profile-quick-stats {
    display: flex;
    flex-direction: column;
    gap: 1rem;
    background: transparent;
}

.quick-stat {
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 1rem;
    background: white !important;
    border-radius: var(--border-radius);
    border: 1px solid var(--border-light);
    transition: all 0.3s ease;
}

.quick-stat:hover {
    background: white !important;
    box-shadow: var(--shadow-light);
    transform: translateY(-1px);
}

.quick-stat .stat-icon {
    width: 50px;
    height: 50px;
    border-radius: 50%;
    background: linear-gradient(135deg, var(--tpa-primary), var(--tpa-primary-dark)) !important;
    color: white;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
}

.quick-stat .stat-icon .material-icons {
    font-size: 1.5rem;
}

.stat-info {
    flex: 1;
    background: transparent;
}

.stat-number {
    font-size: 1.8rem;
    font-weight: 700;
    color: var(--text-primary);
    line-height: 1;
    margin-bottom: 0.25rem;
    background: transparent;
}

.stat-label {
    font-size: 0.9rem;
    color: var(--text-secondary);
    font-weight: 500;
    background: transparent;
}

/* Profile Tabs Container - Light Background */
.profile-tabs-container {
    background: white !important;
    border-radius: var(--border-radius-large);
    box-shadow: var(--shadow-light);
    border: 1px solid var(--border-light);
    overflow: hidden;
}

/* Profile Tabs - Light Background */
.profile-tabs {
    display: flex;
    background: #f8f9fa !important;
    border-bottom: 1px solid var(--border-light);
    overflow-x: auto;
}

.tab-button {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    padding: 1rem 1.5rem;
    background: transparent !important;
    border: none;
    border-bottom: 3px solid transparent;
    cursor: pointer;
    font-size: 0.9rem;
    font-weight: 500;
    color: var(--text-secondary);
    white-space: nowrap;
    transition: all 0.3s ease;
}

.tab-button:hover {
    background: rgba(59, 130, 246, 0.1) !important;
    color: var(--tpa-primary);
}

.tab-button.active {
    background: white !important;
    color: var(--tpa-primary);
    border-bottom-color: var(--tpa-primary);
    font-weight: 600;
}

.tab-button .material-icons {
    font-size: 1.1rem;
}

/* Tab Content - White Background */
.tab-content {
    display: none;
    padding: 2rem;
    background: white !important;
}

.tab-content.active {
    display: block;
    background: white !important;
}

/* Profile Sections */
.profile-section {
    margin-bottom: 2rem;
}

.section-header {
    margin-bottom: 1.5rem;
    padding-bottom: 1rem;
    border-bottom: 1px solid var(--border-light);
}

.section-header h3 {
    display: flex;
    align-items: center;
    gap: 0.5rem;
    margin: 0;
    color: var(--text-primary);
    font-size: 1.3rem;
    font-weight: 600;
}

.section-header .material-icons {
    color: var(--tpa-primary);
    font-size: 1.5rem;
}

/* Profile Grid */
.profile-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 1.5rem;
}

.profile-field {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.profile-field.full-width {
    grid-column: 1 / -1;
}

.profile-field label {
    font-weight: 600;
    color: var(--text-secondary);
    font-size: 0.9rem;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.field-value {
    padding: 1rem;
    background: var(--background-light);
    border: 1px solid var(--border-light);
    border-radius: var(--border-radius);
    color: var(--text-primary);
    font-size: 1rem;
    min-height: 24px;
    display: flex;
    align-items: center;
}

/* Security Grid */
.security-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 1.5rem;
}

.security-card {
    display: flex;
    align-items: center;
    gap: 1rem;
    padding: 1.5rem;
    background: var(--background-light);
    border: 1px solid var(--border-light);
    border-radius: var(--border-radius);
    transition: all 0.3s ease;
    position: relative;
}

.security-card:hover {
    background: white;
    box-shadow: var(--shadow-light);
    transform: translateY(-1px);
}

.security-icon {
    width: 60px;
    height: 60px;
    border-radius: 50%;
    background: linear-gradient(135deg, var(--tpa-primary), var(--tpa-primary-dark));
    color: white;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
}

.security-icon .material-icons {
    font-size: 1.8rem;
}

.security-info {
    flex: 1;
}

.security-info h4 {
    margin: 0 0 0.5rem 0;
    color: var(--text-primary);
    font-size: 1.1rem;
    font-weight: 600;
}

.security-info p {
    margin: 0;
    color: var(--text-secondary);
    font-size: 0.9rem;
    line-height: 1.4;
}

.security-info small {
    color: var(--text-muted);
    font-size: 0.8rem;
}

.security-action {
    flex-shrink: 0;
}

/* Activity List */
.activity-list {
    max-height: 500px;
    overflow-y: auto;
}

.activity-item {
    display: flex;
    align-items: flex-start;
    gap: 1rem;
    padding: 1.5rem;
    border: 1px solid var(--border-light);
    border-radius: var(--border-radius);
    margin-bottom: 1rem;
    background: var(--background-light);
    transition: all 0.3s ease;
}

.activity-item:hover {
    background: white;
    box-shadow: var(--shadow-light);
    transform: translateY(-1px);
}

.activity-item:last-child {
    margin-bottom: 0;
}

.activity-icon {
    width: 45px;
    height: 45px;
    border-radius: 50%;
    background: linear-gradient(135deg, var(--tpa-primary), var(--tpa-primary-dark));
    color: white;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    margin-top: 0.25rem;
}

.activity-icon .material-icons {
    font-size: 1.3rem;
}

.activity-content {
    flex: 1;
}

.activity-title {
    font-weight: 600;
    color: var(--text-primary);
    font-size: 1rem;
    margin-bottom: 0.25rem;
}

.activity-description {
    color: var(--text-secondary);
    font-size: 0.9rem;
    line-height: 1.4;
    margin-bottom: 0.5rem;
}

.activity-time {
    color: var(--text-muted);
    font-size: 0.8rem;
    font-style: italic;
}

/* No Activity State */
.no-activity {
    text-align: center;
    padding: 3rem;
    color: var(--text-muted);
}

.no-activity .material-icons {
    font-size: 4rem;
    color: var(--text-disabled);
    margin-bottom: 1rem;
}

.no-activity h4 {
    margin: 0 0 0.5rem 0;
    color: var(--text-secondary);
    font-size: 1.2rem;
}

.no-activity p {
    margin: 0;
    font-size: 0.9rem;
}

/* Modal Enhancements */
.large-modal {
    max-width: 800px;
    width: 90%;
}

.form-grid {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 1.5rem;
    margin-bottom: 1.5rem;
}

.form-group.full-width {
    grid-column: 1 / -1;
}

.form-hint {
    display: block;
    margin-top: 0.5rem;
    color: var(--text-muted);
    font-size: 0.8rem;
    font-style: italic;
}

.required {
    color: #dc2626;
    font-weight: bold;
}

/* Status Badges */
.status-badge {
    display: inline-flex;
    align-items: center;
    gap: 0.25rem;
    padding: 0.25rem 0.75rem;
    border-radius: 20px;
    font-size: 0.8rem;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.status-active {
    background: #dcfce7;
    color: #16a34a;
}

.status-inactive {
    background: #fee2e2;
    color: #dc2626;
}

.status-pending {
    background: #fef3c7;
    color: #d97706;
}

/* Button Enhancements */
.btn-small {
    padding: 0.5rem 1rem;
    font-size: 0.85rem;
    border-radius: 6px;
}

/* Responsive Design */
@media (max-width: 1024px) {
    .profile-layout {
        grid-template-columns: 1fr;
        gap: 1.5rem;
    }
    
    .profile-summary-card {
        position: static;
    }
    
    .profile-tabs {
        flex-wrap: wrap;
    }
    
    .security-grid {
        grid-template-columns: 1fr;
    }
}

@media (max-width: 768px) {
    .profile-container {
        padding: 1rem;
    }
    
    .profile-summary-card,
    .profile-tabs-container {
        padding: 1.5rem;
    }
    
    .tab-content {
        padding: 1.5rem;
    }
    
    .profile-grid {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
    
    .form-grid {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
    
    .profile-avatar-section {
        flex-direction: column;
    }
    
    .profile-avatar-large {
        width: 100px;
        height: 100px;
        font-size: 2rem;
    }
    
    .quick-stat {
        flex-direction: column;
        text-align: center;
        gap: 0.75rem;
    }
    
    .security-card {
        flex-direction: column;
        text-align: center;
        gap: 1rem;
    }
    
    .activity-item {
        flex-direction: column;
        align-items: center;
        text-align: center;
        gap: 1rem;
    }
    
    .activity-content {
        text-align: center;
    }
}

@media (max-width: 480px) {
    .profile-tabs {
        flex-direction: column;
    }
    
    .tab-button {
        justify-content: center;
        padding: 1rem;
        border-bottom: none;
        border-right: 3px solid transparent;
    }
    
    .tab-button.active {
        border-bottom: none;
        border-right-color: var(--tpa-primary);
    }
    
    .profile-badges {
        width: 100%;
    }
    
    .profile-badge {
        justify-content: center;
    }
    
    .large-modal {
        width: 95%;
        margin: 1rem;
    }
    
    .modal-content {
        max-height: 90vh;
        overflow-y: auto;
    }
}

/* Print Styles */
@media print {
    .profile-container {
        box-shadow: none;
        border: none;
    }
    
    .welcome-actions,
    .tab-button,
    .security-action,
    .btn-tpa,
    .btn-secondary,
    .modal-overlay {
        display: none !important;
    }
    
    .profile-layout {
        grid-template-columns: 1fr;
    }
    
    .tab-content {
        display: block !important;
        page-break-inside: avoid;
    }
    
    .profile-section {
        page-break-inside: avoid;
        margin-bottom: 2rem;
    }
    
    .activity-list {
        max-height: none;
    }
}

/* Accessibility Enhancements */
.tab-button:focus,
.btn-tpa:focus,
.btn-secondary:focus {
    outline: 2px solid var(--tpa-primary);
    outline-offset: 2px;
}

/* Smooth Scrolling for Activity List */
.activity-list {
    scroll-behavior: smooth;
}

/* Custom Scrollbar for Activity List */
.activity-list::-webkit-scrollbar {
    width: 6px;
}

.activity-list::-webkit-scrollbar-track {
    background: var(--background-light);
    border-radius: 3px;
}

.activity-list::-webkit-scrollbar-thumb {
    background: var(--border-light);
    border-radius: 3px;
}

.activity-list::-webkit-scrollbar-thumb:hover {
    background: var(--text-muted);
}

/* Loading States */
.profile-loading {
    display: flex;
    justify-content: center;
    align-items: center;
    padding: 3rem;
    color: var(--text-muted);
}

.profile-loading .material-icons {
    animation: spin 1s linear infinite;
    margin-right: 0.5rem;
}

@keyframes spin {
    from { transform: rotate(0deg); }
    to { transform: rotate(360deg); }
}2, 1fr);
    gap: 1.5rem;
    margin-bottom: 1.5rem;
}

.form-group.full-width {
    grid-column: 1 / -1;
}

.form-hint {
    display: block;
    margin-top: 0.5rem;
    color: var(--text-muted);
    font-size: 0.8rem;
    font-style: italic;
}

.required {
    color: #dc2626;
    font-weight: bold;
}

/* Status Badges */
.status-badge {
    display: inline-flex;
    align-items: center;
    gap: 0.25rem;
    padding: 0.25rem 0.75rem;
    border-radius: 20px;
    font-size: 0.8rem;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.status-active {
    background: #dcfce7;
    color: #16a34a;
}

.status-inactive {
    background: #fee2e2;
    color: #dc2626;
}

.status-pending {
    background: #fef3c7;
    color: #d97706;
}

/* Button Enhancements */
.btn-small {
    padding: 0.5rem 1rem;
    font-size: 0.85rem;
    border-radius: 6px;
}

/* Responsive Design */
@media (max-width: 1024px) {
    .profile-layout {
        grid-template-columns: 1fr;
        gap: 1.5rem;
    }
    
    .profile-summary-card {
        position: static;
    }
    
    .profile-tabs {
        flex-wrap: wrap;
    }
    
    .security-grid {
        grid-template-columns: 1fr;
    }
}

@media (max-width: 768px) {
    .profile-container {
        padding: 1rem;
    }
    
    .profile-summary-card,
    .profile-tabs-container {
        padding: 1.5rem;
    }
    
    .tab-content {
        padding: 1.5rem;
    }
    
    .profile-grid {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
    
    .form-grid {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
    
    .profile-avatar-section {
        flex-direction: column;
    }
    
    .profile-avatar-large {
        width: 100px;
        height: 100px;
        font-size: 2rem;
    }
    
    .quick-stat {
        flex-direction: column;
        text-align: center;
        gap: 0.75rem;
    }
    
    .security-card {
        flex-direction: column;
        text-align: center;
        gap: 1rem;
    }
    
    .activity-item {
        flex-direction: column;
        align-items: center;
        text-align: center;
        gap: 1rem;
    }
    
    .activity-content {
        text-align: center;
    }
}

@media (max-width: 480px) {
    .profile-tabs {
        flex-direction: column;
    }
    
    .tab-button {
        justify-content: center;
        padding: 1rem;
        border-bottom: none;
        border-right: 3px solid transparent;
    }
    
    .tab-button.active {
        border-bottom: none;
        border-right-color: var(--tpa-primary);
    }
    
    .profile-badges {
        width: 100%;
    }
    
    .profile-badge {
        justify-content: center;
    }
    
    .large-modal {
        width: 95%;
        margin: 1rem;
    }
    
    .modal-content {
        max-height: 90vh;
        overflow-y: auto;
    }
}

/* Print Styles */
@media print {
    .profile-container {
        box-shadow: none;
        border: none;
    }
    
    .welcome-actions,
    .tab-button,
    .security-action,
    .btn-tpa,
    .btn-secondary,
    .modal-overlay {
        display: none !important;
    }
    
    .profile-layout {
        grid-template-columns: 1fr;
    }
    
    .tab-content {
        display: block !important;
        page-break-inside: avoid;
    }
    
    .profile-section {
        page-break-inside: avoid;
        margin-bottom: 2rem;
    }
    
    .activity-list {
        max-height: none;
    }
}

/* Accessibility Enhancements */
.tab-button:focus,
.btn-tpa:focus,
.btn-secondary:focus {
    outline: 2px solid var(--tpa-primary);
    outline-offset: 2px;
}

/* Smooth Scrolling for Activity List */
.activity-list {
    scroll-behavior: smooth;
}

/* Custom Scrollbar for Activity List */
.activity-list::-webkit-scrollbar {
    width: 6px;
}

.activity-list::-webkit-scrollbar-track {
    background: var(--background-light);
    border-radius: 3px;
}

.activity-list::-webkit-scrollbar-thumb {
    background: var(--border-light);
    border-radius: 3px;
}

.activity-list::-webkit-scrollbar-thumb:hover {
    background: var(--text-muted);
}

/* Loading States */
.profile-loading {
    display: flex;
    justify-content: center;
    align-items: center;
    padding: 3rem;
    color: var(--text-muted);
}

.profile-loading .material-icons {
    animation: spin 1s linear infinite;
    margin-right: 0.5rem;
}

@keyframes spin {
    from { transform: rotate(0deg); }
    to { transform: rotate(360deg); }
}2, 1fr);
    gap: 1.5rem;
    margin-bottom: 1.5rem;
}

.form-group.full-width {
    grid-column: 1 / -1;
}

.form-hint {
    display: block;
    margin-top: 0.5rem;
    color: var(--text-muted);
    font-size: 0.8rem;
    font-style: italic;
}

.required {
    color: #dc2626;
    font-weight: bold;
}

/* Status Badges */
.status-badge {
    display: inline-flex;
    align-items: center;
    gap: 0.25rem;
    padding: 0.25rem 0.75rem;
    border-radius: 20px;
    font-size: 0.8rem;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.status-active {
    background: #dcfce7;
    color: #16a34a;
}

.status-inactive {
    background: #fee2e2;
    color: #dc2626;
}

.status-pending {
    background: #fef3c7;
    color: #d97706;
}

/* Button Enhancements */
.btn-small {
    padding: 0.5rem 1rem;
    font-size: 0.85rem;
    border-radius: 6px;
}

/* Responsive Design */
@media (max-width: 1024px) {
    .profile-layout {
        grid-template-columns: 1fr;
        gap: 1.5rem;
    }
    
    .profile-summary-card {
        position: static;
    }
    
    .profile-tabs {
        flex-wrap: wrap;
    }
    
    .security-grid {
        grid-template-columns: 1fr;
    }
}

@media (max-width: 768px) {
    .profile-container {
        padding: 1rem;
    }
    
    .profile-summary-card,
    .profile-tabs-container {
        padding: 1.5rem;
    }
    
    .tab-content {
        padding: 1.5rem;
    }
    
    .profile-grid {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
    
    .form-grid {
        grid-template-columns: 1fr;
        gap: 1rem;
    }
    
    .profile-avatar-section {
        flex-direction: column;
    }
    
    .profile-avatar-large {
        width: 100px;
        height: 100px;
        font-size: 2rem;
    }
    
    .quick-stat {
        flex-direction: column;
        text-align: center;
        gap: 0.75rem;
    }
    
    .security-card {
        flex-direction: column;
        text-align: center;
        gap: 1rem;
    }
    
    .activity-item {
        flex-direction: column;
        align-items: center;
        text-align: center;
        gap: 1rem;
    }
    
    .activity-content {
        text-align: center;
    }
}

@media (max-width: 480px) {
    .profile-tabs {
        flex-direction: column;
    }
    
    .tab-button {
        justify-content: center;
        padding: 1rem;
        border-bottom: none;
        border-right: 3px solid transparent;
    }
    
    .tab-button.active {
        border-bottom: none;
        border-right-color: var(--tpa-primary);
    }
    
    .profile-badges {
        width: 100%;
    }
    
    .profile-badge {
        justify-content: center;
    }
    
    .large-modal {
        width: 95%;
        margin: 1rem;
    }
    
    .modal-content {
        max-height: 90vh;
        overflow-y: auto;
    }
}

/* Print Styles */
@media print {
    .profile-container {
        box-shadow: none;
        border: none;
    }
    
    .welcome-actions,
    .tab-button,
    .security-action,
    .btn-tpa,
    .btn-secondary,
    .modal-overlay {
        display: none !important;
    }
    
    .profile-layout {
        grid-template-columns: 1fr;
    }
    
    .tab-content {
        display: block !important;
        page-break-inside: avoid;
    }
    
    .profile-section {
        page-break-inside: avoid;
        margin-bottom: 2rem;
    }
    
    .activity-list {
        max-height: none;
    }
}

/* Dark Mode Support (if implemented) */
@media (prefers-color-scheme: dark) {
    .profile-summary-card,
    .profile-tabs-container {
        background: #1f2937;
        border-color: #374151;
    }
    
    .field-value,
    .security-card,
    .activity-item {
        background: #374151;
        border-color: #4b5563;
        color: #f9fafb;
    }
    
    .profile-tabs {
        background: #374151;
    }
    
    .tab-button.active {
        background: #1f2937;
    }
}

/* Accessibility Enhancements */
.tab-button:focus,
.btn-tpa:focus,
.btn-secondary:focus {
    outline: 2px solid var(--tpa-primary);
    outline-offset: 2px;
}

/* Smooth Scrolling for Activity List */
.activity-list {
    scroll-behavior: smooth;
}

/* Custom Scrollbar for Activity List */
.activity-list::-webkit-scrollbar {
    width: 6px;
}

.activity-list::-webkit-scrollbar-track {
    background: var(--background-light);
    border-radius: 3px;
}

.activity-list::-webkit-scrollbar-thumb {
    background: var(--border-light);
    border-radius: 3px;
}

.activity-list::-webkit-scrollbar-thumb:hover {
    background: var(--text-muted);
}

/* Loading States */
.profile-loading {
    display: flex;
    justify-content: center;
    align-items: center;
    padding: 3rem;
    color: var(--text-muted);
}

.profile-loading .material-icons {
    animation: spin 1s linear infinite;
    margin-right: 0.5rem;
}

@keyframes spin {
    from { transform: rotate(0deg); }
    to { transform: rotate(360deg); }
}
</style>
  <!-- Welcome Header - Matching EmployeeLeavePortal Style -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">account_circle</i>
                    My Profile
                </h1>
                <p class="welcome-subtitle">Manage your personal information, account settings, and preferences</p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">person</i>
                        <span>
                            <asp:Literal ID="litFullName" runat="server" Text="User Name"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">badge</i>
                        <span>
                            <asp:Literal ID="litEmployeeNumber" runat="server" Text="EMP001"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">business</i>
                        <span>
                            <asp:Literal ID="litDepartment" runat="server" Text="Department"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">work</i>
                        <span>
                            <asp:Literal ID="litJobTitle" runat="server" Text="Position"></asp:Literal>
                        </span>
                    </div>
                </div>
            </div>
            <div class="welcome-actions">
                <asp:Button ID="btnEditProfile" runat="server" Text="Edit Profile" CssClass="btn-tpa" OnClick="btnEditProfile_Click" />
                <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" CssClass="btn-secondary" OnClick="btnChangePassword_Click" />
            </div>
        </div>
    </div>

    <!-- Error/Success Messages -->
    <asp:Panel ID="pnlMessages" runat="server" Visible="false" CssClass="alert-panel">
        <asp:Label ID="lblMessage" runat="server" CssClass="message-text"></asp:Label>
    </asp:Panel>

    <!-- Profile Content -->
    <div class="profile-container">
        <div class="profile-layout">
            <!-- Profile Summary Card -->
            <div class="profile-summary-card">
                <div class="profile-avatar-section">
                    <div class="profile-avatar-large">
                        <asp:Literal ID="litAvatarInitials" runat="server"></asp:Literal>
                    </div>
                    <div class="profile-avatar-info">
                        <h3><asp:Literal ID="litProfileName" runat="server"></asp:Literal></h3>
                        <p class="profile-title"><asp:Literal ID="litProfileTitle" runat="server"></asp:Literal></p>
                        <div class="profile-badges">
                            <span class="profile-badge">
                                <i class="material-icons">verified_user</i>
                                <asp:Literal ID="litUserRole" runat="server" Text="Employee"></asp:Literal>
                            </span>
                            <span class="profile-badge status-active">
                                <i class="material-icons">check_circle</i>
                                Active
                            </span>
                        </div>
                    </div>
                </div>
                
                <!-- Quick Stats -->
                <div class="profile-quick-stats">
                    <div class="quick-stat">
                        <div class="stat-icon">
                            <i class="material-icons">event_available</i>
                        </div>
                        <div class="stat-info">
                            <div class="stat-number">
                                <asp:Literal ID="litLeaveBalance" runat="server" Text="0"></asp:Literal>
                            </div>
                            <div class="stat-label">Leave Balance</div>
                        </div>
                    </div>
                    <div class="quick-stat">
                        <div class="stat-icon">
                            <i class="material-icons">schedule</i>
                        </div>
                        <div class="stat-info">
                            <div class="stat-number">
                                <asp:Literal ID="litHoursWorked" runat="server" Text="0"></asp:Literal>
                            </div>
                            <div class="stat-label">Hours This Week</div>
                        </div>
                    </div>
                    <div class="quick-stat">
                        <div class="stat-icon">
                            <i class="material-icons">cake</i>
                        </div>
                        <div class="stat-info">
                            <div class="stat-number">
                                <asp:Literal ID="litYearsOfService" runat="server" Text="0"></asp:Literal>
                            </div>
                            <div class="stat-label">Years of Service</div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Profile Tabs -->
            <div class="profile-tabs-container">
                <div class="profile-tabs">
                    <button type="button" class="tab-button active" onclick="showTab('personal')">
                        <i class="material-icons">person</i>
                        Personal Information
                    </button>
                    <button type="button" class="tab-button" onclick="showTab('employment')">
                        <i class="material-icons">work</i>
                        Employment Details
                    </button>
                    <button type="button" class="tab-button" onclick="showTab('contact')">
                        <i class="material-icons">contact_mail</i>
                        Contact Information
                    </button>
                    <button type="button" class="tab-button" onclick="showTab('security')">
                        <i class="material-icons">security</i>
                        Security & Access
                    </button>
                    <button type="button" class="tab-button" onclick="showTab('activity')">
                        <i class="material-icons">history</i>
                        Recent Activity
                    </button>
                </div>

                <!-- Personal Information Tab -->
                <div id="personalTab" class="tab-content active">
                    <div class="profile-section">
                        <div class="section-header">
                            <h3><i class="material-icons">person</i>Personal Information</h3>
                        </div>
                        <div class="profile-grid">
                            <div class="profile-field">
                                <label>First Name</label>
                                <div class="field-value">
                                    <asp:Literal ID="litFirstName" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field">
                                <label>Last Name</label>
                                <div class="field-value">
                                    <asp:Literal ID="litLastName" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field">
                                <label>Date of Birth</label>
                                <div class="field-value">
                                    <asp:Literal ID="litDateOfBirth" runat="server" Text="Not provided"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field">
                                <label>Gender</label>
                                <div class="field-value">
                                    <asp:Literal ID="litGender" runat="server" Text="Not specified"></asp:Literal>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Employment Details Tab -->
                <div id="employmentTab" class="tab-content">
                    <div class="profile-section">
                        <div class="section-header">
                            <h3><i class="material-icons">work</i>Employment Details</h3>
                        </div>
                        <div class="profile-grid">
                            <div class="profile-field">
                                <label>Employee Number</label>
                                <div class="field-value">
                                    <asp:Literal ID="litEmpNumber" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field">
                                <label>Job Title</label>
                                <div class="field-value">
                                    <asp:Literal ID="litPosition" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field">
                                <label>Department</label>
                                <div class="field-value">
                                    <asp:Literal ID="litDepartmentName" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field">
                                <label>Employee Type</label>
                                <div class="field-value">
                                    <asp:Literal ID="litEmployeeType" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field">
                                <label>Hire Date</label>
                                <div class="field-value">
                                    <asp:Literal ID="litHireDate" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field">
                                <label>Work Location</label>
                                <div class="field-value">
                                    <asp:Literal ID="litWorkLocation" runat="server" Text="Main Office"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field">
                                <label>Manager</label>
                                <div class="field-value">
                                    <asp:Literal ID="litManagerName" runat="server" Text="Not assigned"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field">
                                <label>Employment Status</label>
                                <div class="field-value">
                                    <span class="status-badge status-active">
                                        <asp:Literal ID="litEmploymentStatus" runat="server" Text="Active"></asp:Literal>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Contact Information Tab -->
                <div id="contactTab" class="tab-content">
                    <div class="profile-section">
                        <div class="section-header">
                            <h3><i class="material-icons">contact_mail</i>Contact Information</h3>
                        </div>
                        <div class="profile-grid">
                            <div class="profile-field">
                                <label>Email Address</label>
                                <div class="field-value">
                                    <asp:Literal ID="litEmail" runat="server"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field">
                                <label>Phone Number</label>
                                <div class="field-value">
                                    <asp:Literal ID="litPhoneNumber" runat="server" Text="Not provided"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field full-width">
                                <label>Address</label>
                                <div class="field-value">
                                    <asp:Literal ID="litAddress" runat="server" Text="Not provided"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field">
                                <label>City</label>
                                <div class="field-value">
                                    <asp:Literal ID="litCity" runat="server" Text="Not provided"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field">
                                <label>State</label>
                                <div class="field-value">
                                    <asp:Literal ID="litState" runat="server" Text="Not provided"></asp:Literal>
                                </div>
                            </div>
                            <div class="profile-field">
                                <label>ZIP Code</label>
                                <div class="field-value">
                                    <asp:Literal ID="litZipCode" runat="server" Text="Not provided"></asp:Literal>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Security & Access Tab -->
                <div id="securityTab" class="tab-content">
                    <div class="profile-section">
                        <div class="section-header">
                            <h3><i class="material-icons">security</i>Security & Access</h3>
                        </div>
                        <div class="security-grid">
                            <div class="security-card">
                                <div class="security-icon">
                                    <i class="material-icons">admin_panel_settings</i>
                                </div>
                                <div class="security-info">
                                    <h4>System Role</h4>
                                    <p><asp:Literal ID="litSystemRole" runat="server"></asp:Literal></p>
                                </div>
                            </div>
                            <div class="security-card">
                                <div class="security-icon">
                                    <i class="material-icons">login</i>
                                </div>
                                <div class="security-info">
                                    <h4>Last Login</h4>
                                    <p><asp:Literal ID="litLastLogin" runat="server" Text="Not available"></asp:Literal></p>
                                </div>
                            </div>
                            <div class="security-card">
                                <div class="security-icon">
                                    <i class="material-icons">lock</i>
                                </div>
                                <div class="security-info">
                                    <h4>Password Status</h4>
                                    <p>
                                        <asp:Literal ID="litPasswordStatus" runat="server" Text="Active"></asp:Literal>
                                        <br />
                                        <small>Changed: <asp:Literal ID="litPasswordChanged" runat="server" Text="Unknown"></asp:Literal></small>
                                    </p>
                                </div>
                                <div class="security-action">
                                    <asp:Button ID="btnChangePasswordSec" runat="server" Text="Change Password" CssClass="btn-secondary btn-small" OnClick="btnChangePassword_Click" />
                                </div>
                            </div>
                            <div class="security-card">
                                <div class="security-icon">
                                    <i class="material-icons">verified_user</i>
                                </div>
                                <div class="security-info">
                                    <h4>Account Status</h4>
                                    <p>
                                        <span class="status-badge status-active">Active Account</span>
                                        <br />
                                        <small>Created: <asp:Literal ID="litAccountCreated" runat="server" Text="Unknown"></asp:Literal></small>
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Recent Activity Tab -->
                <div id="activityTab" class="tab-content">
                    <div class="profile-section">
                        <div class="section-header">
                            <h3><i class="material-icons">history</i>Recent Activity</h3>
                        </div>
                        <div class="activity-list">
                            <asp:Repeater ID="rptRecentActivity" runat="server">
                                <ItemTemplate>
                                    <div class="activity-item">
                                        <div class="activity-icon">
                                            <i class="material-icons"><%# GetActivityIcon(Eval("ActivityType").ToString()) %></i>
                                        </div>
                                        <div class="activity-content">
                                            <div class="activity-title"><%# Eval("ActivityType") %></div>
                                            <div class="activity-description"><%# Eval("Description") %></div>
                                            <div class="activity-time"><%# GetRelativeTime(Eval("CreatedAt")) %></div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            
                            <asp:Panel ID="pnlNoActivity" runat="server" Visible="false" CssClass="no-activity">
                                <i class="material-icons">history</i>
                                <h4>No recent activity</h4>
                                <p>Your recent activities will appear here.</p>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Edit Profile Modal -->
    <asp:Panel ID="pnlEditProfile" runat="server" Visible="false" CssClass="modal-overlay">
        <div class="modal-content large-modal">
            <div class="modal-header">
                <h3>
                    <i class="material-icons">edit</i>
                    Edit Profile
                </h3>
                <asp:Button ID="btnCloseEditModal" runat="server" Text="×" CssClass="btn-close" OnClick="btnCloseEditModal_Click" />
            </div>
            <div class="modal-body">
                <div class="form-grid">
                    <div class="form-group">
                        <label>First Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtEditFirstName" runat="server" CssClass="form-control" required></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Last Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtEditLastName" runat="server" CssClass="form-control" required></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Phone Number</label>
                        <asp:TextBox ID="txtEditPhone" runat="server" CssClass="form-control" TextMode="Phone"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Date of Birth</label>
                        <asp:TextBox ID="txtEditDateOfBirth" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="form-group full-width">
                        <label>Address</label>
                        <asp:TextBox ID="txtEditAddress" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>City</label>
                        <asp:TextBox ID="txtEditCity" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>State</label>
                        <asp:TextBox ID="txtEditState" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>ZIP Code</label>
                        <asp:TextBox ID="txtEditZipCode" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnSaveProfile" runat="server" Text="Save Changes" CssClass="btn-tpa" OnClick="btnSaveProfile_Click" />
                <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="btn-secondary" OnClick="btnCancelEdit_Click" />
            </div>
        </div>
    </asp:Panel>

    <!-- Change Password Modal -->
    <asp:Panel ID="pnlChangePassword" runat="server" Visible="false" CssClass="modal-overlay">
        <div class="modal-content">
            <div class="modal-header">
                <h3>
                    <i class="material-icons">lock</i>
                    Change Password
                </h3>
                <asp:Button ID="btnClosePasswordModal" runat="server" Text="×" CssClass="btn-close" OnClick="btnClosePasswordModal_Click" />
            </div>
            <div class="modal-body">
                <div class="form-group">
                    <label>Current Password <span class="required">*</span></label>
                    <asp:TextBox ID="txtCurrentPassword" runat="server" CssClass="form-control" TextMode="Password" required></asp:TextBox>
                </div>
                <div class="form-group">
                    <label>New Password <span class="required">*</span></label>
                    <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" required></asp:TextBox>
                    <small class="form-hint">Password must be at least 8 characters long</small>
                </div>
                <div class="form-group">
                    <label>Confirm New Password <span class="required">*</span></label>
                    <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" required></asp:TextBox>
                </div>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnSavePassword" runat="server" Text="Change Password" CssClass="btn-tpa" OnClick="btnSavePassword_Click" />
                <asp:Button ID="btnCancelPassword" runat="server" Text="Cancel" CssClass="btn-secondary" OnClick="btnCancelPassword_Click" />
            </div>
        </div>
    </asp:Panel>

    <!-- Hidden Fields -->
    <asp:HiddenField ID="hfCurrentUserId" runat="server" />
    <asp:HiddenField ID="hfEmployeeId" runat="server" />

    <!-- JavaScript for Tab Functionality -->
    <script type="text/javascript">
        function showTab(tabName) {
            // Hide all tab contents
            var tabContents = document.querySelectorAll('.tab-content');
            tabContents.forEach(function (content) {
                content.classList.remove('active');
            });

            // Remove active class from all tab buttons
            var tabButtons = document.querySelectorAll('.tab-button');
            tabButtons.forEach(function (button) {
                button.classList.remove('active');
            });

            // Show selected tab content
            var selectedTab = document.getElementById(tabName + 'Tab');
            if (selectedTab) {
                selectedTab.classList.add('active');
            }

            // Add active class to clicked button
            event.target.classList.add('active');
        }

        // Close modals on escape key
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                var modals = document.querySelectorAll('.modal-overlay');
                modals.forEach(function (modal) {
                    if (modal.style.display !== 'none') {
                        modal.style.display = 'none';
                    }
                });
            }
        });

        // Close modal when clicking overlay
        document.addEventListener('click', function (e) {
            if (e.target.classList.contains('modal-overlay')) {
                e.target.style.display = 'none';
            }
        });
    </script>
</asp:Content>