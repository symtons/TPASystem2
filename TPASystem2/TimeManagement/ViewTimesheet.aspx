<%@ Page Title="View Timesheet" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ViewTimesheet.aspx.cs" Inherits="TPASystem2.TimeManagement.ViewTimesheet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

    <!-- Embedded CSS for ViewTimesheet -->
    <style>
        /* =============================================================================
           TIMESHEET VIEW STYLES - Complete CSS for ViewTimesheet
           ============================================================================= */

        /* Alert Panel Styles */
        .alert-panel {
            padding: 1rem;
            margin-bottom: 1.5rem;
            border-radius: 8px;
            border: 1px solid transparent;
            display: flex;
            align-items: center;
        }

        .alert-content {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            width: 100%;
        }

        .alert-icon {
            font-size: 1.2rem;
        }

        .alert-success {
            background-color: #d4edda;
            border-color: #c3e6cb;
            color: #155724;
        }

        .alert-error {
            background-color: #f8d7da;
            border-color: #f5c6cb;
            color: #721c24;
        }

        .alert-warning {
            background-color: #fff3cd;
            border-color: #ffeaa7;
            color: #856404;
        }

        .alert-info {
            background-color: #d1ecf1;
            border-color: #bee5eb;
            color: #0c5460;
        }

        /* Base Button Styles */
        .btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            padding: 0.5rem 1rem;
            font-size: 0.875rem;
            font-weight: 500;
            text-align: center;
            text-decoration: none;
            border: 1px solid transparent;
            border-radius: 6px;
            cursor: pointer;
            transition: all 0.15s ease-in-out;
            gap: 0.25rem;
            min-height: 38px;
        }

        .btn:hover {
            text-decoration: none;
            transform: translateY(-1px);
        }

        .btn:focus {
            outline: 0;
            box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
        }

        .btn:disabled {
            opacity: 0.6;
            cursor: not-allowed;
            transform: none;
        }

        /* Button Variants */
        .btn-primary {
            color: #fff;
            background: linear-gradient(135deg, #1976d2, #42a5f5);
            border-color: #1976d2;
            box-shadow: 0 2px 4px rgba(25, 118, 210, 0.2);
        }

        .btn-primary:hover {
            background: linear-gradient(135deg, #1565c0, #1976d2);
            border-color: #1565c0;
            box-shadow: 0 4px 8px rgba(25, 118, 210, 0.3);
            color: #fff;
        }

        .btn-secondary {
            color: #6c757d;
            background-color: #f8f9fa;
            border-color: #dee2e6;
        }

        .btn-secondary:hover {
            color: #545b62;
            background-color: #e2e6ea;
            border-color: #dae0e5;
        }

        .btn-outline-light {
            color: rgba(255, 255, 255, 0.9);
            background-color: transparent;
            border: 2px solid rgba(255, 255, 255, 0.3);
        }

        .btn-outline-light:hover {
            color: #fff;
            background-color: rgba(255, 255, 255, 0.1);
            border-color: rgba(255, 255, 255, 0.5);
        }

        .btn-outline-secondary {
            color: #6c757d;
            background-color: transparent;
            border-color: #6c757d;
        }

        .btn-outline-secondary:hover {
            color: #fff;
            background-color: #6c757d;
            border-color: #6c757d;
        }

        .btn-outline-primary {
            color: #1976d2;
            background-color: transparent;
            border-color: #1976d2;
        }

        .btn-outline-primary:hover {
            color: #fff;
            background-color: #1976d2;
            border-color: #1976d2;
        }

        /* Button Sizes */
        .btn-sm {
            padding: 0.375rem 0.75rem;
            font-size: 0.8rem;
            border-radius: 4px;
            min-height: 32px;
        }

        .btn-lg {
            padding: 0.75rem 1.5rem;
            font-size: 1rem;
            border-radius: 8px;
            min-height: 48px;
        }

        /* Timesheet-specific onboarding header */
        .timesheet-onboarding-header {
            background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
            color: white;
            padding: 2.5rem;
            border-radius: 16px;
            margin-bottom: 2rem;
            position: relative;
            overflow: hidden;
            box-shadow: 0 8px 32px rgba(25, 118, 210, 0.2);
        }

        .timesheet-onboarding-header::before {
            content: '';
            position: absolute;
            top: -50%;
            right: -10%;
            width: 60%;
            height: 200%;
            background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><circle cx="20" cy="20" r="2" fill="white" opacity="0.1"/><circle cx="80" cy="80" r="1.5" fill="white" opacity="0.1"/><circle cx="40" cy="60" r="1" fill="white" opacity="0.1"/><circle cx="60" cy="30" r="1.2" fill="white" opacity="0.1"/></svg>');
            opacity: 0.3;
        }

        .timesheet-welcome-content {
            display: flex;
            justify-content: space-between;
            align-items: center;
            position: relative;
            z-index: 1;
        }

        .timesheet-welcome-text {
            flex: 1;
        }

        .timesheet-welcome-title {
            font-size: 2.5rem;
            font-weight: 600;
            margin-bottom: 0.5rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .timesheet-welcome-title .material-icons {
            font-size: 2.5rem;
            color: #ffd54f;
        }

        .timesheet-welcome-subtitle {
            font-size: 1.2rem;
            opacity: 0.9;
            margin: 0;
        }

        .timesheet-header-actions {
            display: flex;
            flex-direction: column;
            align-items: flex-end;
            gap: 1rem;
        }

        .timesheet-breadcrumb-nav {
            margin-bottom: 0.5rem;
        }

        .timesheet-status {
            display: flex;
            align-items: center;
        }

        .timesheet-status-badge {
            background: rgba(255, 255, 255, 0.2);
            backdrop-filter: blur(10px);
            color: white;
            padding: 0.5rem 1rem;
            border-radius: 20px;
            font-weight: 600;
            font-size: 0.9rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .timesheet-status-badge .material-icons {
            font-size: 1rem;
        }

        .status-draft {
            background: rgba(251, 191, 36, 0.2);
            color: #fbbf24;
        }

        .status-submitted {
            background: rgba(59, 130, 246, 0.2);
            color: #3b82f6;
        }

        .status-approved {
            background: rgba(34, 197, 94, 0.2);
            color: #22c55e;
        }

        .status-rejected {
            background: rgba(239, 68, 68, 0.2);
            color: #ef4444;
        }

        /* Employee Info Card */
        .timesheet-employee-info-card {
            background: white;
            border-radius: 16px;
            padding: 2rem;
            margin-bottom: 2rem;
            box-shadow: 0 4px 16px rgba(0,0,0,0.1);
            display: flex;
            align-items: center;
            gap: 1.5rem;
        }

        .timesheet-employee-avatar {
            width: 80px;
            height: 80px;
            background: linear-gradient(135deg, #1976d2, #42a5f5);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 2rem;
            flex-shrink: 0;
        }

        .timesheet-employee-details {
            flex: 1;
        }

        .timesheet-employee-details h3 {
            margin: 0 0 0.5rem 0;
            color: #1e293b;
            font-size: 1.5rem;
            font-weight: 700;
        }

        .timesheet-employee-details p {
            margin: 0 0 1rem 0;
            color: #64748b;
            font-size: 1rem;
        }

        .timesheet-info-tags {
            display: flex;
            gap: 1rem;
            flex-wrap: wrap;
        }

        .timesheet-info-tag {
            background: #f1f5f9;
            color: #475569;
            padding: 0.5rem 1rem;
            border-radius: 20px;
            font-size: 0.9rem;
            font-weight: 500;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .timesheet-info-tag .material-icons {
            font-size: 1rem;
        }

        /* Summary Dashboard */
        .timesheet-summary-dashboard {
            background: white;
            border-radius: 16px;
            padding: 2rem;
            margin-bottom: 2rem;
            box-shadow: 0 4px 16px rgba(0,0,0,0.1);
        }

        .timesheet-summary-header {
            margin-bottom: 2rem;
            text-align: center;
        }

        .timesheet-summary-header h3 {
            margin: 0 0 0.5rem 0;
            color: #1e293b;
            font-size: 1.5rem;
            font-weight: 700;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 0.5rem;
        }

        .timesheet-summary-header p {
            margin: 0;
            color: #64748b;
            font-size: 1rem;
        }

        .timesheet-summary-cards {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 1.5rem;
        }

        .timesheet-summary-card {
            background: #f8fafc;
            border-radius: 12px;
            padding: 1.5rem;
            border: 2px solid transparent;
            transition: all 0.3s ease;
            position: relative;
            overflow: hidden;
        }

        .timesheet-summary-card::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 4px;
            background: var(--card-accent, #1976d2);
        }

        .timesheet-summary-card.total-hours {
            --card-accent: #1976d2;
        }

        .timesheet-summary-card.regular-hours {
            --card-accent: #10b981;
        }

        .timesheet-summary-card.overtime-hours {
            --card-accent: #f59e0b;
        }

        .timesheet-summary-card.days-worked {
            --card-accent: #8b5cf6;
        }

        .timesheet-summary-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 25px rgba(0,0,0,0.15);
            border-color: var(--card-accent);
        }

        .timesheet-summary-content {
            display: flex;
            align-items: center;
            gap: 1rem;
        }

        .timesheet-summary-icon {
            width: 50px;
            height: 50px;
            background: var(--card-accent);
            border-radius: 12px;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 1.5rem;
            flex-shrink: 0;
        }

        .timesheet-summary-details {
            flex: 1;
        }

        .timesheet-summary-label {
            font-size: 0.9rem;
            color: #64748b;
            font-weight: 500;
            margin-bottom: 0.5rem;
        }

        .timesheet-summary-value {
            font-size: 2rem;
            font-weight: 700;
            color: #1e293b;
            line-height: 1;
            margin-bottom: 0.5rem;
        }

        .timesheet-summary-value.overtime {
            color: #f59e0b;
        }

        .timesheet-summary-note {
            font-size: 0.8rem;
            color: #64748b;
            margin-top: 0.25rem;
            font-style: italic;
        }

        .timesheet-summary-progress {
            margin-top: 0.75rem;
        }

        .timesheet-progress-bar {
            width: 100%;
            height: 6px;
            background: #e2e8f0;
            border-radius: 3px;
            overflow: hidden;
            margin-bottom: 0.5rem;
        }

        .timesheet-progress-fill {
            height: 100%;
            background: linear-gradient(90deg, #1976d2, #42a5f5);
            border-radius: 3px;
            transition: width 0.5s ease;
        }

        .timesheet-progress-text {
            font-size: 0.8rem;
            color: #64748b;
        }

        /* Daily Entries Section */
        .timesheet-daily-entries-section {
            margin-bottom: 2rem;
        }

        .timesheet-section-header {
            text-align: center;
            margin-bottom: 2rem;
        }

        .timesheet-section-header h2 {
            margin: 0 0 0.5rem 0;
            color: #1e293b;
            font-size: 2rem;
            font-weight: 700;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 0.5rem;
        }

        .timesheet-section-header h3 {
            margin: 0 0 0.5rem 0;
            color: #1e293b;
            font-size: 1.5rem;
            font-weight: 700;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 0.5rem;
        }

        .timesheet-section-header p {
            margin: 0;
            color: #64748b;
            font-size: 1.1rem;
        }

        /* Day Entry Cards */
        .timesheet-day-entry-card.weekend .timesheet-time-value {
            color: #7c3aed;
        }

        /* Animation Enhancements */
        @keyframes fadeInUp {
            from {
                opacity: 0;
                transform: translateY(20px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .timesheet-day-entry-card {
            animation: fadeInUp 0.5s ease forwards;
        }

        .timesheet-day-entry-card:nth-child(1) { animation-delay: 0.1s; }
        .timesheet-day-entry-card:nth-child(2) { animation-delay: 0.15s; }
        .timesheet-day-entry-card:nth-child(3) { animation-delay: 0.2s; }
        .timesheet-day-entry-card:nth-child(4) { animation-delay: 0.25s; }
        .timesheet-day-entry-card:nth-child(5) { animation-delay: 0.3s; }
        .timesheet-day-entry-card:nth-child(6) { animation-delay: 0.35s; }
        .timesheet-day-entry-card:nth-child(7) { animation-delay: 0.4s; }

        /* Animation for Timeline Items */
        @keyframes slideInFromLeft {
            from {
                opacity: 0;
                transform: translateX(-20px);
            }
            to {
                opacity: 1;
                transform: translateX(0);
            }
        }

        .timesheet-timeline-item {
            animation: slideInFromLeft 0.6s ease forwards;
        }

        .timesheet-timeline-item:nth-child(1) { animation-delay: 0.1s; }
        .timesheet-timeline-item:nth-child(2) { animation-delay: 0.3s; }

        /* Responsive Design */
        @media (max-width: 1024px) {
            .timesheet-summary-cards {
                grid-template-columns: repeat(2, 1fr);
            }
            
            .timesheet-time-display-grid {
                grid-template-columns: 1fr;
            }
        }

        @media (max-width: 768px) {
            .timesheet-onboarding-header {
                padding: 1.5rem;
            }
            
            .timesheet-welcome-content {
                flex-direction: column;
                text-align: center;
                gap: 1.5rem;
            }
            
            .timesheet-header-actions {
                align-items: center;
                width: 100%;
            }
            
            .timesheet-employee-info-card {
                flex-direction: column;
                text-align: center;
                padding: 1.5rem;
            }
            
            .timesheet-summary-cards {
                grid-template-columns: 1fr;
            }
            
            .timesheet-day-header {
                flex-direction: column;
                gap: 1rem;
                text-align: center;
            }
            
            .timesheet-day-info {
                justify-content: center;
            }
            
            .timesheet-day-content {
                padding: 1.5rem;
            }
            
            .timesheet-primary-actions, .timesheet-secondary-actions {
                flex-direction: column;
            }
            
            .timesheet-primary-actions .btn, .timesheet-secondary-actions .btn {
                min-width: auto;
                width: 100%;
            }

            .timesheet-hours-breakdown {
                flex-direction: column;
                gap: 0.5rem;
            }
            
            .timesheet-timeline-item {
                gap: 1rem;
            }
            
            .timesheet-timeline-icon {
                width: 40px;
                height: 40px;
                font-size: 1rem;
            }
            
            .timesheet-approval-timeline::before {
                left: 20px;
            }
            
            .timesheet-timeline-content {
                margin-top: 4px;
            }
        }

        @media (max-width: 480px) {
            .timesheet-onboarding-header {
                padding: 1rem;
            }
            
            .timesheet-welcome-title {
                font-size: 1.8rem;
            }
            
            .timesheet-employee-info-card {
                padding: 1rem;
            }
            
            .timesheet-employee-avatar {
                width: 60px;
                height: 60px;
                font-size: 1.5rem;
            }
            
            .timesheet-summary-dashboard, .timesheet-notes-section, .timesheet-actions-section {
                padding: 1.5rem;
            }
            
            .timesheet-day-content {
                padding: 1rem;
            }
            
            .timesheet-info-tags {
                justify-content: center;
            }
            
            .timesheet-time-display-item {
                padding: 0.75rem;
            }
            
            .timesheet-timeline-content {
                padding: 1rem;
            }
            
            .timesheet-timeline-title {
                font-size: 1rem;
            }
        }

        /* Print Optimizations */
        @media print {
            .timesheet-approval-timeline::before {
                background: #000 !important;
            }
            
            .timesheet-timeline-icon {
                background: #000 !important;
                color: white !important;
            }
            
            .timesheet-time-display-grid {
                grid-template-columns: repeat(2, 1fr);
            }
            
            .timesheet-summary-cards {
                grid-template-columns: repeat(2, 1fr);
            }
            
            .timesheet-day-entry-card {
                break-inside: avoid;
                margin-bottom: 1rem;
            }
            
            .timesheet-timeline-item {
                break-inside: avoid;
            }

            .timesheet-actions-section {
                display: none !important;
            }
        }
    </style>

    <!-- Message Panel -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert-panel">
        <div class="alert-content">
            <i class="material-icons alert-icon"></i>
            <asp:Literal ID="litMessage" runat="server"></asp:Literal>
        </div>
    </asp:Panel>

    <!-- Onboarding-Style Header -->
    <div class="timesheet-onboarding-header">
        <div class="timesheet-welcome-content">
            <div class="timesheet-welcome-text">
                <h1 class="timesheet-welcome-title">
                    <i class="material-icons">visibility</i>
                    Timesheet Review
                </h1>
                <p class="timesheet-welcome-subtitle">Viewing timesheet for week of <asp:Literal ID="litWeekRange" runat="server"></asp:Literal></p>
            </div>
            <div class="timesheet-header-actions">
                <div class="timesheet-breadcrumb-nav">
                    <asp:Button ID="btnBackToList" runat="server" Text="← Back to Timesheets" 
                        CssClass="btn btn-outline-light btn-sm" OnClick="btnBackToTimesheets_Click" />
                </div>
                <div class="timesheet-status">
                    <span class="timesheet-status-badge status-<%= GetStatusClass() %>">
                        <i class="material-icons">
                            <%= GetStatusIcon() %>
                        </i>
                        <asp:Literal ID="litTimesheetStatus" runat="server"></asp:Literal>
                    </span>
                </div>
            </div>
        </div>
    </div>

    <!-- Employee Info Card -->
    <div class="timesheet-employee-info-card">
        <div class="timesheet-employee-avatar">
            <i class="material-icons">person</i>
        </div>
        <div class="timesheet-employee-details">
            <h3><asp:Literal ID="litEmployeeName" runat="server"></asp:Literal></h3>
            <p>Employee #<asp:Literal ID="litEmployeeNumber" runat="server"></asp:Literal></p>
            <div class="timesheet-info-tags">
                <span class="timesheet-info-tag">
                    <i class="material-icons">business</i>
                    <asp:Literal ID="litDepartment" runat="server"></asp:Literal>
                </span>
                <span class="timesheet-info-tag">
                    <i class="material-icons">badge</i>
                    <asp:Literal ID="litPosition" runat="server"></asp:Literal>
                </span>
                <span class="timesheet-info-tag">
                    <i class="material-icons">schedule</i>
                    <asp:Literal ID="litSubmissionDate" runat="server"></asp:Literal>
                </span>
            </div>
        </div>
        <div class="timesheet-header-actions">
            <asp:Button ID="btnEditTimesheet" runat="server" Text="✏️ Edit Timesheet" 
                CssClass="btn btn-primary" OnClick="btnEditTimesheet_Click" />
            <asp:Button ID="btnPrintTimesheet" runat="server" Text="🖨️ Print" 
                CssClass="btn btn-outline-secondary" OnClientClick="window.print(); return false;" />
        </div>
    </div>

    <!-- Weekly Summary Dashboard -->
    <div class="timesheet-summary-dashboard">
        <div class="timesheet-summary-header">
            <h3>
                <i class="material-icons">assessment</i>
                Weekly Summary
            </h3>
            <p>Complete breakdown of hours worked during this period</p>
        </div>
        
        <div class="timesheet-summary-cards">
            <div class="timesheet-summary-card total-hours">
                <div class="timesheet-summary-content">
                    <div class="timesheet-summary-icon">
                        <i class="material-icons">schedule</i>
                    </div>
                    <div class="timesheet-summary-details">
                        <div class="timesheet-summary-label">Total Hours</div>
                        <div class="timesheet-summary-value">
                            <asp:Literal ID="litTotalHours" runat="server" Text="0.0"></asp:Literal>h
                        </div>
                        <div class="timesheet-summary-progress">
                            <div class="timesheet-progress-bar">
                                <div class="timesheet-progress-fill" id="totalHoursProgress" style="width: 0%"></div>
                            </div>
                            <span class="timesheet-progress-text">of 40h weekly target</span>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="timesheet-summary-card regular-hours">
                <div class="timesheet-summary-content">
                    <div class="timesheet-summary-icon">
                        <i class="material-icons">work</i>
                    </div>
                    <div class="timesheet-summary-details">
                        <div class="timesheet-summary-label">Regular Hours</div>
                        <div class="timesheet-summary-value">
                            <asp:Literal ID="litRegularHours" runat="server" Text="0.0"></asp:Literal>h
                        </div>
                        <div class="timesheet-summary-note">Standard work hours</div>
                    </div>
                </div>
            </div>
            
            <div class="timesheet-summary-card overtime-hours">
                <div class="timesheet-summary-content">
                    <div class="timesheet-summary-icon">
                        <i class="material-icons">trending_up</i>
                    </div>
                    <div class="timesheet-summary-details">
                        <div class="timesheet-summary-label">Overtime Hours</div>
                        <div class="timesheet-summary-value overtime">
                            <asp:Literal ID="litOvertimeHours" runat="server" Text="0.0"></asp:Literal>h
                        </div>
                        <div class="timesheet-summary-note">Beyond 8h per day</div>
                    </div>
                </div>
            </div>
            
            <div class="timesheet-summary-card days-worked">
                <div class="timesheet-summary-content">
                    <div class="timesheet-summary-icon">
                        <i class="material-icons">event</i>
                    </div>
                    <div class="timesheet-summary-details">
                        <div class="timesheet-summary-label">Days Worked</div>
                        <div class="timesheet-summary-value">
                            <asp:Literal ID="litDaysWorked" runat="server" Text="0"></asp:Literal>
                        </div>
                        <div class="timesheet-summary-note">Days with recorded time</div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Daily Time Entries Section -->
    <div class="timesheet-daily-entries-section view-mode">
        <div class="timesheet-section-header">
            <h2>
                <i class="material-icons">today</i>
                Daily Time Entries
            </h2>
            <p>Detailed breakdown of work hours for each day of the week</p>
        </div>

        <!-- Monday -->
        <div class="timesheet-day-entry-card view-card">
            <div class="timesheet-day-header">
                <div class="timesheet-day-info">
                    <i class="material-icons timesheet-day-icon monday">today</i>
                    <div>
                        <h4>Monday</h4>
                        <span class="timesheet-day-date"><asp:Literal ID="litMondayDate" runat="server"></asp:Literal></span>
                    </div>
                    <div class="timesheet-day-status">
                        <span class="timesheet-status-indicator" id="mondayStatusIndicator">
                            <%= GetDayStatus("Monday") %>
                        </span>
                    </div>
                </div>
                <div class="timesheet-day-total">
                    <span class="timesheet-total-label">Total:</span>
                    <span class="timesheet-total-hours">
                        <asp:Literal ID="litMondayTotal" runat="server" Text="0.0h"></asp:Literal>
                    </span>
                </div>
            </div>
            <div class="timesheet-day-content">
                <div class="timesheet-time-display-grid">
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">play_arrow</i>
                            Start Time
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litMondayStart" runat="server" Text="--:--"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">stop</i>
                            End Time
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litMondayEnd" runat="server" Text="--:--"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">pause</i>
                            Break Duration
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litMondayBreak" runat="server" Text="0 min"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item summary-display">
                        <label>
                            <i class="material-icons">assessment</i>
                            Summary
                        </label>
                        <div class="timesheet-hours-breakdown">
                            <span class="timesheet-breakdown-overtime">OT: <asp:Literal ID="litFridayOvertime" runat="server" Text="0.0h"></asp:Literal></span>
                        </div>
                    </div>
                </div>
                <div class="timesheet-notes-display" id="fridayNotesSection" runat="server" visible="false">
                    <label>
                        <i class="material-icons">note</i>
                        Notes
                    </label>
                    <div class="timesheet-notes-content">
                        <asp:Literal ID="litFridayNotes" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>

        <!-- Saturday -->
        <div class="timesheet-day-entry-card view-card weekend">
            <div class="timesheet-day-header">
                <div class="timesheet-day-info">
                    <i class="material-icons timesheet-day-icon saturday">today</i>
                    <div>
                        <h4>Saturday</h4>
                        <span class="timesheet-day-date"><asp:Literal ID="litSaturdayDate" runat="server"></asp:Literal></span>
                        <span class="timesheet-weekend-badge">Weekend</span>
                    </div>
                    <div class="timesheet-day-status">
                        <span class="timesheet-status-indicator" id="saturdayStatusIndicator">
                            <%= GetDayStatus("Saturday") %>
                        </span>
                    </div>
                </div>
                <div class="timesheet-day-total">
                    <span class="timesheet-total-label">Total:</span>
                    <span class="timesheet-total-hours">
                        <asp:Literal ID="litSaturdayTotal" runat="server" Text="0.0h"></asp:Literal>
                    </span>
                </div>
            </div>
            <div class="timesheet-day-content">
                <div class="timesheet-time-display-grid">
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">play_arrow</i>
                            Start Time
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litSaturdayStart" runat="server" Text="--:--"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">stop</i>
                            End Time
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litSaturdayEnd" runat="server" Text="--:--"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">pause</i>
                            Break Duration
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litSaturdayBreak" runat="server" Text="0 min"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item summary-display">
                        <label>
                            <i class="material-icons">assessment</i>
                            Summary
                        </label>
                        <div class="timesheet-hours-breakdown">
                            <span class="timesheet-breakdown-regular">Regular: <asp:Literal ID="litSaturdayRegular" runat="server" Text="0.0h"></asp:Literal></span>
                            <span class="timesheet-breakdown-overtime">OT: <asp:Literal ID="litSaturdayOvertime" runat="server" Text="0.0h"></asp:Literal></span>
                        </div>
                    </div>
                </div>
                <div class="timesheet-notes-display" id="saturdayNotesSection" runat="server" visible="false">
                    <label>
                        <i class="material-icons">note</i>
                        Notes
                    </label>
                    <div class="timesheet-notes-content">
                        <asp:Literal ID="litSaturdayNotes" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>

        <!-- Sunday -->
        <div class="timesheet-day-entry-card view-card weekend">
            <div class="timesheet-day-header">
                <div class="timesheet-day-info">
                    <i class="material-icons timesheet-day-icon sunday">today</i>
                    <div>
                        <h4>Sunday</h4>
                        <span class="timesheet-day-date"><asp:Literal ID="litSundayDate" runat="server"></asp:Literal></span>
                        <span class="timesheet-weekend-badge">Weekend</span>
                    </div>
                    <div class="timesheet-day-status">
                        <span class="timesheet-status-indicator" id="sundayStatusIndicator">
                            <%= GetDayStatus("Sunday") %>
                        </span>
                    </div>
                </div>
                <div class="timesheet-day-total">
                    <span class="timesheet-total-label">Total:</span>
                    <span class="timesheet-total-hours">
                        <asp:Literal ID="litSundayTotal" runat="server" Text="0.0h"></asp:Literal>
                    </span>
                </div>
            </div>
            <div class="timesheet-day-content">
                <div class="timesheet-time-display-grid">
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">play_arrow</i>
                            Start Time
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litSundayStart" runat="server" Text="--:--"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">stop</i>
                            End Time
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litSundayEnd" runat="server" Text="--:--"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">pause</i>
                            Break Duration
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litSundayBreak" runat="server" Text="0 min"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item summary-display">
                        <label>
                            <i class="material-icons">assessment</i>
                            Summary
                        </label>
                        <div class="timesheet-hours-breakdown">
                            <span class="timesheet-breakdown-regular">Regular: <asp:Literal ID="litSundayRegular" runat="server" Text="0.0h"></asp:Literal></span>
                            <span class="timesheet-breakdown-overtime">OT: <asp:Literal ID="litSundayOvertime" runat="server" Text="0.0h"></asp:Literal></span>
                        </div>
                    </div>
                </div>
                <div class="timesheet-notes-display" id="sundayNotesSection" runat="server" visible="false">
                    <label>
                        <i class="material-icons">note</i>
                        Notes
                    </label>
                    <div class="timesheet-notes-content">
                        <asp:Literal ID="litSundayNotes" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Timesheet Notes Section -->
    <div class="timesheet-notes-section view-mode" id="timesheetNotesSection" runat="server" visible="false">
        <div class="timesheet-section-header">
            <h3>
                <i class="material-icons">note</i>
                Timesheet Notes
            </h3>
            <p>Additional comments and information for this timesheet period</p>
        </div>
        <div class="timesheet-notes-view-card">
            <div class="timesheet-notes-content-display">
                <asp:Literal ID="litTimesheetNotes" runat="server"></asp:Literal>
            </div>
        </div>
    </div>

    <!-- Approval Information Section -->
    <div class="timesheet-approval-info-section" id="approvalInfoSection" runat="server" visible="false">
        <div class="timesheet-section-header">
            <h3>
                <i class="material-icons">fact_check</i>
                Approval Information
            </h3>
            <p>Timeline and details of timesheet approval process</p>
        </div>
        
        <div class="timesheet-approval-timeline">
            <div class="timesheet-timeline-item submitted">
                <div class="timesheet-timeline-icon">
                    <i class="material-icons">send</i>
                </div>
                <div class="timesheet-timeline-content">
                    <div class="timesheet-timeline-title">Timesheet Submitted</div>
                    <div class="timesheet-timeline-date">
                        <i class="material-icons">schedule</i>
                        <asp:Literal ID="litSubmittedAt" runat="server"></asp:Literal>
                    </div>
                    <div class="timesheet-timeline-description">Timesheet submitted for review and approval</div>
                </div>
            </div>
            
            <div class="timesheet-timeline-item approved" id="approvedTimelineItem" runat="server" visible="false">
                <div class="timesheet-timeline-icon">
                    <i class="material-icons">check_circle</i>
                </div>
                <div class="timesheet-timeline-content">
                    <div class="timesheet-timeline-title">Approved</div>
                    <div class="timesheet-timeline-person" id="approvedBySection" runat="server" visible="false">
                        <i class="material-icons">person</i>
                        <span>Approved by: <asp:Literal ID="litApprovedBy" runat="server"></asp:Literal></span>
                    </div>
                    <div class="timesheet-timeline-date" id="approvedAtSection" runat="server" visible="false">
                        <i class="material-icons">schedule</i>
                        <asp:Literal ID="litApprovedAt" runat="server"></asp:Literal>
                    </div>
                    <div class="timesheet-timeline-description">Timesheet has been reviewed and approved</div>
                </div>
            </div>
        </div>
    </div>

    <!-- Actions Section -->
    <div class="timesheet-actions-section">
        <div class="timesheet-actions-card">
            <div class="timesheet-primary-actions">
                <asp:Button ID="btnEditTimesheetBottom" runat="server" Text="✏️ Edit Timesheet" 
                    CssClass="btn btn-primary btn-lg" OnClick="btnEditTimesheet_Click" />
                <asp:Button ID="btnPrintTimesheetBottom" runat="server" Text="🖨️ Print Timesheet" 
                    CssClass="btn btn-outline-secondary btn-lg" OnClientClick="window.print(); return false;" />
            </div>
            <div class="timesheet-secondary-actions">
                <asp:Button ID="btnBackToTimesheetsBottom" runat="server" Text="← Back to Timesheets" 
                    CssClass="btn btn-outline-primary" OnClick="btnBackToTimesheets_Click" />
            </div>
        </div>
    </div>

    <!-- JavaScript for View Enhancement -->
    <script type="text/javascript">
        // Initialize progress bar on page load
        document.addEventListener('DOMContentLoaded', function () {
            updateProgressBar();
            highlightOvertimeHours();
        });

        function updateProgressBar() {
            const totalHoursElement = document.querySelector('[id$="litTotalHours"]');
            const progressBar = document.getElementById('totalHoursProgress');

            if (totalHoursElement && progressBar) {
                const totalHours = parseFloat(totalHoursElement.textContent || '0');
                const progressPercent = Math.min(100, (totalHours / 40) * 100);
                progressBar.style.width = progressPercent + '%';

                // Change color based on progress
                if (progressPercent >= 100) {
                    progressBar.style.background = 'linear-gradient(90deg, #10b981, #059669)';
                } else if (progressPercent >= 80) {
                    progressBar.style.background = 'linear-gradient(90deg, #f59e0b, #d97706)';
                } else {
                    progressBar.style.background = 'linear-gradient(90deg, #1976d2, #42a5f5)';
                }
            }
        }

        function highlightOvertimeHours() {
            // Highlight overtime values if they exist
            const overtimeElements = document.querySelectorAll('.timesheet-breakdown-overtime');
            overtimeElements.forEach(element => {
                const overtimeText = element.textContent;
                const overtimeValue = parseFloat(overtimeText.replace(/[^\d.]/g, ''));

                if (overtimeValue > 0) {
                    element.style.fontWeight = 'bold';
                    element.style.color = '#f59e0b';
                    element.parentElement.style.background = 'linear-gradient(135deg, #fef3c7, #fde68a)';
                    element.parentElement.style.padding = '0.5rem';
                    element.parentElement.style.borderRadius = '6px';
                }
            });
        }

        // Print optimization
        function optimizePrint() {
            // Hide unnecessary elements when printing
            const elementsToHide = [
                '.timesheet-actions-section',
                '.btn'
            ];

            elementsToHide.forEach(selector => {
                const elements = document.querySelectorAll(selector);
                elements.forEach(el => el.style.display = 'none');
            });

            // Optimize layout for print
            const dayCards = document.querySelectorAll('.timesheet-day-entry-card');
            dayCards.forEach(card => {
                card.style.breakInside = 'avoid';
                card.style.marginBottom = '1rem';
            });
        }

        // Enhanced print function
        window.addEventListener('beforeprint', optimizePrint);

        window.addEventListener('afterprint', function () {
            // Restore elements after printing
            location.reload(); // Simple solution to restore all elements
        });
    </script>

</asp:Content>-regular">Regular: <asp:Literal ID="litMondayRegular" runat="server" Text="0.0h"></asp:Literal></span>
                            <span class="timesheet-breakdown-overtime">OT: <asp:Literal ID="litMondayOvertime" runat="server" Text="0.0h"></asp:Literal></span>
                        </div>
                    </div>
                </div>
                <div class="timesheet-notes-display" id="mondayNotesSection" runat="server" visible="false">
                    <label>
                        <i class="material-icons">note</i>
                        Notes
                    </label>
                    <div class="timesheet-notes-content">
                        <asp:Literal ID="litMondayNotes" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>

        <!-- Tuesday -->
        <div class="timesheet-day-entry-card view-card">
            <div class="timesheet-day-header">
                <div class="timesheet-day-info">
                    <i class="material-icons timesheet-day-icon tuesday">today</i>
                    <div>
                        <h4>Tuesday</h4>
                        <span class="timesheet-day-date"><asp:Literal ID="litTuesdayDate" runat="server"></asp:Literal></span>
                    </div>
                    <div class="timesheet-day-status">
                        <span class="timesheet-status-indicator" id="tuesdayStatusIndicator">
                            <%= GetDayStatus("Tuesday") %>
                        </span>
                    </div>
                </div>
                <div class="timesheet-day-total">
                    <span class="timesheet-total-label">Total:</span>
                    <span class="timesheet-total-hours">
                        <asp:Literal ID="litTuesdayTotal" runat="server" Text="0.0h"></asp:Literal>
                    </span>
                </div>
            </div>
            <div class="timesheet-day-content">
                <div class="timesheet-time-display-grid">
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">play_arrow</i>
                            Start Time
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litTuesdayStart" runat="server" Text="--:--"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">stop</i>
                            End Time
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litTuesdayEnd" runat="server" Text="--:--"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">pause</i>
                            Break Duration
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litTuesdayBreak" runat="server" Text="0 min"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item summary-display">
                        <label>
                            <i class="material-icons">assessment</i>
                            Summary
                        </label>
                        <div class="timesheet-hours-breakdown">
                            <span class="timesheet-breakdown-regular">Regular: <asp:Literal ID="litTuesdayRegular" runat="server" Text="0.0h"></asp:Literal></span>
                            <span class="timesheet-breakdown-overtime">OT: <asp:Literal ID="litTuesdayOvertime" runat="server" Text="0.0h"></asp:Literal></span>
                        </div>
                    </div>
                </div>
                <div class="timesheet-notes-display" id="tuesdayNotesSection" runat="server" visible="false">
                    <label>
                        <i class="material-icons">note</i>
                        Notes
                    </label>
                    <div class="timesheet-notes-content">
                        <asp:Literal ID="litTuesdayNotes" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>

        <!-- Wednesday -->
        <div class="timesheet-day-entry-card view-card">
            <div class="timesheet-day-header">
                <div class="timesheet-day-info">
                    <i class="material-icons timesheet-day-icon wednesday">today</i>
                    <div>
                        <h4>Wednesday</h4>
                        <span class="timesheet-day-date"><asp:Literal ID="litWednesdayDate" runat="server"></asp:Literal></span>
                    </div>
                    <div class="timesheet-day-status">
                        <span class="timesheet-status-indicator" id="wednesdayStatusIndicator">
                            <%= GetDayStatus("Wednesday") %>
                        </span>
                    </div>
                </div>
                <div class="timesheet-day-total">
                    <span class="timesheet-total-label">Total:</span>
                    <span class="timesheet-total-hours">
                        <asp:Literal ID="litWednesdayTotal" runat="server" Text="0.0h"></asp:Literal>
                    </span>
                </div>
            </div>
            <div class="timesheet-day-content">
                <div class="timesheet-time-display-grid">
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">play_arrow</i>
                            Start Time
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litWednesdayStart" runat="server" Text="--:--"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">stop</i>
                            End Time
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litWednesdayEnd" runat="server" Text="--:--"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">pause</i>
                            Break Duration
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litWednesdayBreak" runat="server" Text="0 min"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item summary-display">
                        <label>
                            <i class="material-icons">assessment</i>
                            Summary
                        </label>
                        <div class="timesheet-hours-breakdown">
                            <span class="timesheet-breakdown-regular">Regular: <asp:Literal ID="litWednesdayRegular" runat="server" Text="0.0h"></asp:Literal></span>
                            <span class="timesheet-breakdown-overtime">OT: <asp:Literal ID="litWednesdayOvertime" runat="server" Text="0.0h"></asp:Literal></span>
                        </div>
                    </div>
                </div>
                <div class="timesheet-notes-display" id="wednesdayNotesSection" runat="server" visible="false">
                    <label>
                        <i class="material-icons">note</i>
                        Notes
                    </label>
                    <div class="timesheet-notes-content">
                        <asp:Literal ID="litWednesdayNotes" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>

        <!-- Thursday -->
        <div class="timesheet-day-entry-card view-card">
            <div class="timesheet-day-header">
                <div class="timesheet-day-info">
                    <i class="material-icons timesheet-day-icon thursday">today</i>
                    <div>
                        <h4>Thursday</h4>
                        <span class="timesheet-day-date"><asp:Literal ID="litThursdayDate" runat="server"></asp:Literal></span>
                    </div>
                    <div class="timesheet-day-status">
                        <span class="timesheet-status-indicator" id="thursdayStatusIndicator">
                            <%= GetDayStatus("Thursday") %>
                        </span>
                    </div>
                </div>
                <div class="timesheet-day-total">
                    <span class="timesheet-total-label">Total:</span>
                    <span class="timesheet-total-hours">
                        <asp:Literal ID="litThursdayTotal" runat="server" Text="0.0h"></asp:Literal>
                    </span>
                </div>
            </div>
            <div class="timesheet-day-content">
                <div class="timesheet-time-display-grid">
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">play_arrow</i>
                            Start Time
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litThursdayStart" runat="server" Text="--:--"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">stop</i>
                            End Time
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litThursdayEnd" runat="server" Text="--:--"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">pause</i>
                            Break Duration
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litThursdayBreak" runat="server" Text="0 min"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item summary-display">
                        <label>
                            <i class="material-icons">assessment</i>
                            Summary
                        </label>
                        <div class="timesheet-hours-breakdown">
                            <span class="timesheet-breakdown-regular">Regular: <asp:Literal ID="litThursdayRegular" runat="server" Text="0.0h"></asp:Literal></span>
                            <span class="timesheet-breakdown-overtime">OT: <asp:Literal ID="litThursdayOvertime" runat="server" Text="0.0h"></asp:Literal></span>
                        </div>
                    </div>
                </div>
                <div class="timesheet-notes-display" id="thursdayNotesSection" runat="server" visible="false">
                    <label>
                        <i class="material-icons">note</i>
                        Notes
                    </label>
                    <div class="timesheet-notes-content">
                        <asp:Literal ID="litThursdayNotes" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>

        <!-- Friday -->
        <div class="timesheet-day-entry-card view-card">
            <div class="timesheet-day-header">
                <div class="timesheet-day-info">
                    <i class="material-icons timesheet-day-icon friday">today</i>
                    <div>
                        <h4>Friday</h4>
                        <span class="timesheet-day-date"><asp:Literal ID="litFridayDate" runat="server"></asp:Literal></span>
                    </div>
                    <div class="timesheet-day-status">
                        <span class="timesheet-status-indicator" id="fridayStatusIndicator">
                            <%= GetDayStatus("Friday") %>
                        </span>
                    </div>
                </div>
                <div class="timesheet-day-total">
                    <span class="timesheet-total-label">Total:</span>
                    <span class="timesheet-total-hours">
                        <asp:Literal ID="litFridayTotal" runat="server" Text="0.0h"></asp:Literal>
                    </span>
                </div>
            </div>
            <div class="timesheet-day-content">
                <div class="timesheet-time-display-grid">
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">play_arrow</i>
                            Start Time
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litFridayStart" runat="server" Text="--:--"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">stop</i>
                            End Time
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litFridayEnd" runat="server" Text="--:--"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item">
                        <label>
                            <i class="material-icons">pause</i>
                            Break Duration
                        </label>
                        <div class="timesheet-time-value">
                            <asp:Literal ID="litFridayBreak" runat="server" Text="0 min"></asp:Literal>
                        </div>
                    </div>
                    <div class="timesheet-time-display-item summary-display">
                        <label>
                            <i class="material-icons">assessment</i>
                            Summary
                        </label>
                        <div class="timesheet-hours-breakdown">
                            <span class="timesheet-breakdown-regular">Regular: <asp:Literal ID="litFridayRegular" runat="server" Text="0.0h"></asp:Literal></span>
                            <span class="timesheet-breakdownd {
            background: linear-gradient(135deg, #ffffff 0%, #fafbfc 100%);
            border-radius: 16px;
            margin-bottom: 1.5rem;
            box-shadow: 0 4px 16px rgba(0,0,0,0.08);
            border: 2px solid #e2e8f0;
            border-left: 4px solid #1976d2;
            transition: all 0.3s ease;
            overflow: hidden;
        }

        .timesheet-day-entry-card:hover {
            transform: translateY(-1px);
            box-shadow: 0 6px 20px rgba(0,0,0,0.12);
        }

        .timesheet-day-entry-card.weekend {
            border-left: 4px solid #8b5cf6;
        }

        .timesheet-day-header {
            background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
            padding: 1.5rem 2rem;
            display: flex;
            justify-content: space-between;
            align-items: center;
            border-bottom: 1px solid #e2e8f0;
        }

        .timesheet-day-info {
            display: flex;
            align-items: center;
            gap: 1rem;
        }

        .timesheet-day-icon {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.2rem;
            color: white;
        }

        .timesheet-day-icon.monday { background: #1976d2; }
        .timesheet-day-icon.tuesday { background: #dc2626; }
        .timesheet-day-icon.wednesday { background: #059669; }
        .timesheet-day-icon.thursday { background: #7c3aed; }
        .timesheet-day-icon.friday { background: #ea580c; }
        .timesheet-day-icon.saturday { background: #8b5cf6; }
        .timesheet-day-icon.sunday { background: #be185d; }

        .timesheet-day-info h4 {
            margin: 0;
            color: #1e293b;
            font-size: 1.3rem;
            font-weight: 600;
        }

        .timesheet-day-date {
            color: #64748b;
            font-size: 0.9rem;
            font-weight: 500;
        }

        .timesheet-weekend-badge {
            background: linear-gradient(135deg, #8b5cf6, #a855f7);
            color: white;
            padding: 0.25rem 0.75rem;
            border-radius: 12px;
            font-size: 0.8rem;
            font-weight: 600;
            margin-left: 0.5rem;
        }

        .timesheet-day-status {
            margin-left: auto;
        }

        .timesheet-status-indicator {
            padding: 0.5rem 1rem;
            border-radius: 20px;
            font-size: 0.9rem;
            font-weight: 600;
            background: #e2e8f0;
            color: #64748b;
        }

        .timesheet-status-indicator.completed {
            background: linear-gradient(135deg, #dcfce7, #bbf7d0);
            color: #166534;
            border: 1px solid #86efac;
        }

        .timesheet-status-indicator.no-work {
            background: linear-gradient(135deg, #f1f5f9, #e2e8f0);
            color: #475569;
            border: 1px solid #cbd5e1;
        }

        .timesheet-status-indicator.overtime {
            background: linear-gradient(135deg, #fed7aa, #fdba74);
            color: #9a3412;
            border: 1px solid #fb923c;
        }

        .timesheet-day-total {
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .timesheet-total-label {
            color: #64748b;
            font-weight: 500;
        }

        .timesheet-total-hours {
            font-size: 1.5rem;
            font-weight: 700;
            color: #1976d2;
        }

        /* Day Content */
        .timesheet-day-content {
            padding: 2rem;
        }

        .timesheet-time-display-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1.5rem;
            margin-bottom: 1.5rem;
        }

        .timesheet-time-display-item {
            background: #f8fafc;
            border-radius: 8px;
            padding: 1rem;
            border: 1px solid #e2e8f0;
        }

        .timesheet-time-display-item label {
            font-weight: 600;
            color: #374151;
            display: flex;
            align-items: center;
            gap: 0.5rem;
            font-size: 0.9rem;
            margin-bottom: 0.5rem;
        }

        .timesheet-time-display-item label .material-icons {
            font-size: 1rem;
            color: #6b7280;
        }

        .timesheet-time-value {
            font-size: 1.2rem;
            font-weight: 700;
            color: #1976d2;
            text-align: center;
            background: white;
            padding: 0.5rem;
            border-radius: 6px;
            border: 1px solid #e2e8f0;
        }

        .timesheet-time-display-item.summary-display {
            background: linear-gradient(135deg, #f0f9ff, #e0f2fe);
            border-color: #0284c7;
        }

        .timesheet-hours-breakdown {
            display: flex;
            justify-content: space-between;
            gap: 1rem;
            background: white;
            padding: 0.75rem;
            border-radius: 6px;
            border: 1px solid #e2e8f0;
        }

        .timesheet-breakdown-regular,
        .timesheet-breakdown-overtime {
            font-size: 0.9rem;
            font-weight: 600;
            padding: 0.25rem 0.5rem;
            border-radius: 4px;
        }

        .timesheet-breakdown-regular {
            color: #059669;
            background: #ecfdf5;
        }

        .timesheet-breakdown-overtime {
            color: #dc2626;
            background: #fef2f2;
        }

        /* Notes Display */
        .timesheet-notes-display {
            border-top: 1px solid #e2e8f0;
            padding-top: 1.5rem;
            margin-top: 1.5rem;
        }

        .timesheet-notes-display label {
            font-weight: 600;
            color: #374151;
            display: flex;
            align-items: center;
            gap: 0.5rem;
            margin-bottom: 0.75rem;
        }

        .timesheet-notes-content {
            background: #f8fafc;
            border: 1px solid #e2e8f0;
            border-radius: 8px;
            padding: 1rem;
            min-height: 60px;
            font-size: 0.95rem;
            line-height: 1.6;
            color: #374151;
        }

        .timesheet-notes-content:empty::before {
            content: "No notes for this day";
            color: #9ca3af;
            font-style: italic;
        }

        /* Timesheet Notes Section */
        .timesheet-notes-section {
            background: white;
            border-radius: 16px;
            padding: 2rem;
            margin-bottom: 2rem;
            box-shadow: 0 4px 16px rgba(0,0,0,0.08);
        }

        .timesheet-notes-view-card {
            background: #f8fafc;
            border-radius: 12px;
            padding: 1.5rem;
            border: 1px solid #e2e8f0;
        }

        .timesheet-notes-content-display {
            background: white;
            border: 1px solid #e2e8f0;
            border-radius: 8px;
            padding: 1.5rem;
            min-height: 100px;
            font-size: 1rem;
            line-height: 1.6;
            color: #374151;
            white-space: pre-wrap;
        }

        .timesheet-notes-content-display:empty::before {
            content: "No additional notes provided for this timesheet";
            color: #9ca3af;
            font-style: italic;
        }

        /* Approval Information Section */
        .timesheet-approval-info-section {
            background: white;
            border-radius: 16px;
            padding: 2rem;
            margin-bottom: 2rem;
            box-shadow: 0 4px 16px rgba(0,0,0,0.08);
            border-left: 4px solid #10b981;
        }

        .timesheet-approval-timeline {
            position: relative;
            margin-top: 2rem;
        }

        .timesheet-approval-timeline::before {
            content: '';
            position: absolute;
            left: 24px;
            top: 0;
            bottom: 0;
            width: 2px;
            background: linear-gradient(180deg, #10b981, #059669);
        }

        .timesheet-timeline-item {
            position: relative;
            display: flex;
            align-items: flex-start;
            gap: 1.5rem;
            margin-bottom: 2rem;
            padding-left: 0;
        }

        .timesheet-timeline-item:last-child {
            margin-bottom: 0;
        }

        .timesheet-timeline-icon {
            width: 48px;
            height: 48px;
            background: linear-gradient(135deg, #10b981, #059669);
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-size: 1.2rem;
            flex-shrink: 0;
            z-index: 2;
            box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
        }

        .timesheet-timeline-item.submitted .timesheet-timeline-icon {
            background: linear-gradient(135deg, #3b82f6, #1d4ed8);
            box-shadow: 0 4px 12px rgba(59, 130, 246, 0.3);
        }

        .timesheet-timeline-content {
            flex: 1;
            background: #f8fafc;
            border-radius: 12px;
            padding: 1.5rem;
            border: 1px solid #e2e8f0;
            margin-top: 6px;
        }

        .timesheet-timeline-title {
            font-size: 1.2rem;
            font-weight: 700;
            color: #1e293b;
            margin-bottom: 0.5rem;
        }

        .timesheet-timeline-person,
        .timesheet-timeline-date {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            font-size: 0.9rem;
            color: #64748b;
            margin-bottom: 0.5rem;
        }

        .timesheet-timeline-person .material-icons,
        .timesheet-timeline-date .material-icons {
            font-size: 1rem;
        }

        .timesheet-timeline-description {
            color: #475569;
            font-size: 0.95rem;
            line-height: 1.5;
        }

        /* Actions Section */
        .timesheet-actions-section {
            background: white;
            border-radius: 16px;
            padding: 2rem;
            box-shadow: 0 4px 16px rgba(0,0,0,0.08);
            margin-bottom: 2rem;
        }

        .timesheet-actions-card {
            display: flex;
            flex-direction: column;
            gap: 1.5rem;
        }

        .timesheet-primary-actions, .timesheet-secondary-actions {
            display: flex;
            gap: 1rem;
            justify-content: center;
            flex-wrap: wrap;
        }

        .timesheet-primary-actions .btn {
            min-width: 180px;
            font-weight: 600;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 0.5rem;
        }

        /* Hover Effects for Interactive Elements */
        .timesheet-time-display-item:hover {
            transform: translateY(-1px);
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            border-color: #1976d2;
        }

        .timesheet-timeline-content:hover {
            transform: translateY(-1px);
            box-shadow: 0 6px 20px rgba(0,0,0,0.1);
            border-color: #1976d2;
        }

        /* Weekend Styling Enhancements */
        .timesheet-day-entry-card.weekend .timesheet-time-display-item {
            background: linear-gradient(135deg, #faf5ff, #f3e8ff);
            border-color: #c4b5fd;
        }

        </style>