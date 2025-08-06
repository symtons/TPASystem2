<%@ Page Title="Edit Timesheet" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="EditTimesheet.aspx.cs" Inherits="TPASystem2.TimeManagement.EditTimesheet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

    <!-- Message Panel -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert-panel">
        <div class="alert-content">
            <i class="material-icons alert-icon"></i>
            <asp:Literal ID="litMessage" runat="server"></asp:Literal>
        </div>
    </asp:Panel>

    <!-- Onboarding-Style Header -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">schedule</i>
                    Edit Timesheet
                </h1>
                <p class="welcome-subtitle">Enter your work hours for the week of <asp:Literal ID="litWeekRange" runat="server"></asp:Literal></p>
            </div>
            <div class="header-actions">
                <div class="breadcrumb-nav">
                    <asp:Button ID="btnBackToTimesheets" runat="server" Text="← Back to Timesheets" 
                        CssClass="btn btn-outline-light btn-sm" OnClick="btnBackToTimesheets_Click" />
                </div>
                <div class="timesheet-status">
                    <span class="status-badge status-<%= GetStatusClass() %>">
                        <i class="material-icons">schedule</i>
                        <asp:Literal ID="litTimesheetStatus" runat="server"></asp:Literal>
                    </span>
                </div>
            </div>
        </div>
    </div>

    <!-- Employee Info Card -->
    <div class="employee-info-card">
        <div class="employee-avatar">
            <i class="material-icons">person</i>
        </div>
        <div class="employee-details">
            <h3><asp:Literal ID="litEmployeeName" runat="server"></asp:Literal></h3>
            <p>Employee #<asp:Literal ID="litEmployeeNumber" runat="server"></asp:Literal></p>
            <div class="info-tags">
                <span class="info-tag">
                    <i class="material-icons">business</i>
                    <asp:Literal ID="litDepartment" runat="server"></asp:Literal>
                </span>
                <span class="info-tag">
                    <i class="material-icons">badge</i>
                    <asp:Literal ID="litPosition" runat="server"></asp:Literal>
                </span>
            </div>
        </div>
    </div>

    <!-- Auto-save Indicator -->
    <div id="autoSaveIndicator" class="auto-save-indicator" style="display: none;">
        <i class="material-icons">cloud_done</i>
        <span>Auto-saved</span>
    </div>

    <!-- Quick Actions Toolbar -->
    <div class="quick-actions-toolbar">
        <div class="quick-actions-content">
            <div class="action-group">
                <button type="button" class="btn btn-outline-primary btn-small" onclick="fillStandardHours()">
                    <i class="material-icons">schedule</i>
                    Fill Standard Hours
                </button>
                <button type="button" class="btn btn-outline-secondary btn-small" onclick="fillLastWeek()">
                    <i class="material-icons">content_copy</i>
                    Copy Last Week
                </button>
                <button type="button" class="btn btn-outline-danger btn-small" onclick="clearAllTimes()">
                    <i class="material-icons">clear_all</i>
                    Clear All
                </button>
            </div>
            <div class="action-group">
                <asp:Button ID="btnSaveProgress" runat="server" Text="💾 Save Progress" 
                    CssClass="btn btn-outline-primary btn-small" OnClick="btnSaveDraft_Click" />
                <asp:Button ID="btnPreviewTimesheet" runat="server" Text="👁️ Preview" 
                    CssClass="btn btn-outline-secondary btn-small" OnClick="btnPreviewTimesheet_Click" />
            </div>
        </div>
    </div>

    <!-- Weekly Summary Dashboard -->
    <div class="summary-dashboard">
        <div class="summary-header">
            <h3>
                <i class="material-icons">assessment</i>
                Weekly Summary
            </h3>
            <p>Live calculation updates as you enter time</p>
        </div>
        
        <div class="summary-cards">
            <div class="summary-card total-hours">
                <div class="summary-content">
                    <div class="summary-icon">
                        <i class="material-icons">schedule</i>
                    </div>
                    <div class="summary-details">
                        <div class="summary-label">Total Hours</div>
                        <div class="summary-value">
                            <asp:Literal ID="litTotalHours" runat="server" Text="0.0"></asp:Literal>h
                        </div>
                        <div class="summary-progress">
                            <div class="progress-bar">
                                <div class="progress-fill" id="totalHoursProgress" style="width: 0%"></div>
                            </div>
                            <span class="progress-text">of 40h weekly target</span>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="summary-card regular-hours">
                <div class="summary-content">
                    <div class="summary-icon">
                        <i class="material-icons">work</i>
                    </div>
                    <div class="summary-details">
                        <div class="summary-label">Regular Hours</div>
                        <div class="summary-value">
                            <asp:Literal ID="litRegularHours" runat="server" Text="0.0"></asp:Literal>h
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="summary-card overtime-hours">
                <div class="summary-content">
                    <div class="summary-icon">
                        <i class="material-icons">trending_up</i>
                    </div>
                    <div class="summary-details">
                        <div class="summary-label">Overtime Hours</div>
                        <div class="summary-value overtime">
                            <asp:Literal ID="litOvertimeHours" runat="server" Text="0.0"></asp:Literal>h
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="summary-card days-worked">
                <div class="summary-content">
                    <div class="summary-icon">
                        <i class="material-icons">event</i>
                    </div>
                    <div class="summary-details">
                        <div class="summary-label">Days Worked</div>
                        <div class="summary-value">
                            <asp:Literal ID="litDaysWorked" runat="server" Text="0"></asp:Literal>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Daily Time Entries Section -->
    <div class="daily-entries-section">
        <div class="section-header">
            <h2>
                <i class="material-icons">today</i>
                Daily Time Entries
            </h2>
            <p>Enter your work hours for each day of the week. Times are automatically calculated.</p>
        </div>

        <!-- Monday -->
        <div class="day-entry-card edit-mode">
            <div class="day-header">
                <div class="day-info">
                    <i class="material-icons day-icon monday">today</i>
                    <div>
                        <h4>Monday</h4>
                        <span class="day-date"><asp:Literal ID="litMondayDate" runat="server"></asp:Literal></span>
                    </div>
                    <div class="day-status">
                        <span class="status-indicator" id="mondayStatus">Not Started</span>
                    </div>
                </div>
                <div class="day-total">
                    <span class="total-label">Total:</span>
                    <span class="total-hours" id="mondayTotal">0.0h</span>
                </div>
            </div>
            <div class="day-content">
                <div class="time-inputs-grid">
                    <div class="input-group">
                        <label>
                            <i class="material-icons">play_arrow</i>
                            Start Time
                        </label>
                        <asp:TextBox ID="txtMondayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('monday')" placeholder="09:00" />
                    </div>
                    <div class="input-group">
                        <label>
                            <i class="material-icons">stop</i>
                            End Time
                        </label>
                        <asp:TextBox ID="txtMondayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('monday')" placeholder="17:00" />
                    </div>
                    <div class="input-group">
                        <label>
                            <i class="material-icons">pause</i>
                            Break (minutes)
                        </label>
                        <asp:TextBox ID="txtMondayBreak" runat="server" CssClass="form-control break-input" 
                            TextMode="Number" Text="30" onchange="calculateDayTotal('monday')" 
                            min="0" max="480" step="15" />
                    </div>
                    <div class="input-group time-summary">
                        <label>Summary</label>
                        <div class="daily-hours-display">
                            <span id="mondayHours">0.0 hours</span>
                            <small id="mondayBreakdown">Regular: 0.0h | OT: 0.0h</small>
                        </div>
                    </div>
                </div>
                <div class="notes-section">
                    <label>
                        <i class="material-icons">note</i>
                        Notes (optional)
                    </label>
                    <asp:TextBox ID="txtMondayNotes" runat="server" CssClass="form-control notes-input" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>

        <!-- Tuesday -->
        <div class="day-entry-card edit-mode">
            <div class="day-header">
                <div class="day-info">
                    <i class="material-icons day-icon tuesday">today</i>
                    <div>
                        <h4>Tuesday</h4>
                        <span class="day-date"><asp:Literal ID="litTuesdayDate" runat="server"></asp:Literal></span>
                    </div>
                    <div class="day-status">
                        <span class="status-indicator" id="tuesdayStatus">Not Started</span>
                    </div>
                </div>
                <div class="day-total">
                    <span class="total-label">Total:</span>
                    <span class="total-hours" id="tuesdayTotal">0.0h</span>
                </div>
            </div>
            <div class="day-content">
                <div class="time-inputs-grid">
                    <div class="input-group">
                        <label>
                            <i class="material-icons">play_arrow</i>
                            Start Time
                        </label>
                        <asp:TextBox ID="txtTuesdayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('tuesday')" placeholder="09:00" />
                    </div>
                    <div class="input-group">
                        <label>
                            <i class="material-icons">stop</i>
                            End Time
                        </label>
                        <asp:TextBox ID="txtTuesdayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('tuesday')" placeholder="17:00" />
                    </div>
                    <div class="input-group">
                        <label>
                            <i class="material-icons">pause</i>
                            Break (minutes)
                        </label>
                        <asp:TextBox ID="txtTuesdayBreak" runat="server" CssClass="form-control break-input" 
                            TextMode="Number" Text="30" onchange="calculateDayTotal('tuesday')" 
                            min="0" max="480" step="15" />
                    </div>
                    <div class="input-group time-summary">
                        <label>Summary</label>
                        <div class="daily-hours-display">
                            <span id="tuesdayHours">0.0 hours</span>
                            <small id="tuesdayBreakdown">Regular: 0.0h | OT: 0.0h</small>
                        </div>
                    </div>
                </div>
                <div class="notes-section">
                    <label>
                        <i class="material-icons">note</i>
                        Notes (optional)
                    </label>
                    <asp:TextBox ID="txtTuesdayNotes" runat="server" CssClass="form-control notes-input" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>

        <!-- Wednesday -->
        <div class="day-entry-card edit-mode">
            <div class="day-header">
                <div class="day-info">
                    <i class="material-icons day-icon wednesday">today</i>
                    <div>
                        <h4>Wednesday</h4>
                        <span class="day-date"><asp:Literal ID="litWednesdayDate" runat="server"></asp:Literal></span>
                    </div>
                    <div class="day-status">
                        <span class="status-indicator" id="wednesdayStatus">Not Started</span>
                    </div>
                </div>
                <div class="day-total">
                    <span class="total-label">Total:</span>
                    <span class="total-hours" id="wednesdayTotal">0.0h</span>
                </div>
            </div>
            <div class="day-content">
                <div class="time-inputs-grid">
                    <div class="input-group">
                        <label>
                            <i class="material-icons">play_arrow</i>
                            Start Time
                        </label>
                        <asp:TextBox ID="txtWednesdayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('wednesday')" placeholder="09:00" />
                    </div>
                    <div class="input-group">
                        <label>
                            <i class="material-icons">stop</i>
                            End Time
                        </label>
                        <asp:TextBox ID="txtWednesdayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('wednesday')" placeholder="17:00" />
                    </div>
                    <div class="input-group">
                        <label>
                            <i class="material-icons">pause</i>
                            Break (minutes)
                        </label>
                        <asp:TextBox ID="txtWednesdayBreak" runat="server" CssClass="form-control break-input" 
                            TextMode="Number" Text="30" onchange="calculateDayTotal('wednesday')" 
                            min="0" max="480" step="15" />
                    </div>
                    <div class="input-group time-summary">
                        <label>Summary</label>
                        <div class="daily-hours-display">
                            <span id="wednesdayHours">0.0 hours</span>
                            <small id="wednesdayBreakdown">Regular: 0.0h | OT: 0.0h</small>
                        </div>
                    </div>
                </div>
                <div class="notes-section">
                    <label>
                        <i class="material-icons">note</i>
                        Notes (optional)
                    </label>
                    <asp:TextBox ID="txtWednesdayNotes" runat="server" CssClass="form-control notes-input" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>

        <!-- Thursday -->
        <div class="day-entry-card edit-mode">
            <div class="day-header">
                <div class="day-info">
                    <i class="material-icons day-icon thursday">today</i>
                    <div>
                        <h4>Thursday</h4>
                        <span class="day-date"><asp:Literal ID="litThursdayDate" runat="server"></asp:Literal></span>
                    </div>
                    <div class="day-status">
                        <span class="status-indicator" id="thursdayStatus">Not Started</span>
                    </div>
                </div>
                <div class="day-total">
                    <span class="total-label">Total:</span>
                    <span class="total-hours" id="thursdayTotal">0.0h</span>
                </div>
            </div>
            <div class="day-content">
                <div class="time-inputs-grid">
                    <div class="input-group">
                        <label>
                            <i class="material-icons">play_arrow</i>
                            Start Time
                        </label>
                        <asp:TextBox ID="txtThursdayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('thursday')" placeholder="09:00" />
                    </div>
                    <div class="input-group">
                        <label>
                            <i class="material-icons">stop</i>
                            End Time
                        </label>
                        <asp:TextBox ID="txtThursdayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('thursday')" placeholder="17:00" />
                    </div>
                    <div class="input-group">
                        <label>
                            <i class="material-icons">pause</i>
                            Break (minutes)
                        </label>
                        <asp:TextBox ID="txtThursdayBreak" runat="server" CssClass="form-control break-input" 
                            TextMode="Number" Text="30" onchange="calculateDayTotal('thursday')" 
                            min="0" max="480" step="15" />
                    </div>
                    <div class="input-group time-summary">
                        <label>Summary</label>
                        <div class="daily-hours-display">
                            <span id="thursdayHours">0.0 hours</span>
                            <small id="thursdayBreakdown">Regular: 0.0h | OT: 0.0h</small>
                        </div>
                    </div>
                </div>
                <div class="notes-section">
                    <label>
                        <i class="material-icons">note</i>
                        Notes (optional)
                    </label>
                    <asp:TextBox ID="txtThursdayNotes" runat="server" CssClass="form-control notes-input" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>

        <!-- Friday -->
        <div class="day-entry-card edit-mode">
            <div class="day-header">
                <div class="day-info">
                    <i class="material-icons day-icon friday">today</i>
                    <div>
                        <h4>Friday</h4>
                        <span class="day-date"><asp:Literal ID="litFridayDate" runat="server"></asp:Literal></span>
                    </div>
                    <div class="day-status">
                        <span class="status-indicator" id="fridayStatus">Not Started</span>
                    </div>
                </div>
                <div class="day-total">
                    <span class="total-label">Total:</span>
                    <span class="total-hours" id="fridayTotal">0.0h</span>
                </div>
            </div>
            <div class="day-content">
                <div class="time-inputs-grid">
                    <div class="input-group">
                        <label>
                            <i class="material-icons">play_arrow</i>
                            Start Time
                        </label>
                        <asp:TextBox ID="txtFridayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('friday')" placeholder="09:00" />
                    </div>
                    <div class="input-group">
                        <label>
                            <i class="material-icons">stop</i>
                            End Time
                        </label>
                        <asp:TextBox ID="txtFridayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('friday')" placeholder="17:00" />
                    </div>
                    <div class="input-group">
                        <label>
                            <i class="material-icons">pause</i>
                            Break (minutes)
                        </label>
                        <asp:TextBox ID="txtFridayBreak" runat="server" CssClass="form-control break-input" 
                            TextMode="Number" Text="30" onchange="calculateDayTotal('friday')" 
                            min="0" max="480" step="15" />
                    </div>
                    <div class="input-group time-summary">
                        <label>Summary</label>
                        <div class="daily-hours-display">
                            <span id="fridayHours">0.0 hours</span>
                            <small id="fridayBreakdown">Regular: 0.0h | OT: 0.0h</small>
                        </div>
                    </div>
                </div>
                <div class="notes-section">
                    <label>
                        <i class="material-icons">note</i>
                        Notes (optional)
                    </label>
                    <asp:TextBox ID="txtFridayNotes" runat="server" CssClass="form-control notes-input" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>

        <!-- Saturday -->
        <div class="day-entry-card edit-mode weekend">
            <div class="day-header">
                <div class="day-info">
                    <i class="material-icons day-icon saturday">today</i>
                    <div>
                        <h4>Saturday</h4>
                        <span class="day-date"><asp:Literal ID="litSaturdayDate" runat="server"></asp:Literal></span>
                        <span class="weekend-badge">Weekend</span>
                    </div>
                    <div class="day-status">
                        <span class="status-indicator" id="saturdayStatus">Not Started</span>
                    </div>
                </div>
                <div class="day-total">
                    <span class="total-label">Total:</span>
                    <span class="total-hours" id="saturdayTotal">0.0h</span>
                </div>
            </div>
            <div class="day-content">
                <div class="time-inputs-grid">
                    <div class="input-group">
                        <label>
                            <i class="material-icons">play_arrow</i>
                            Start Time
                        </label>
                        <asp:TextBox ID="txtSaturdayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('saturday')" placeholder="09:00" />
                    </div>
                    <div class="input-group">
                        <label>
                            <i class="material-icons">stop</i>
                            End Time
                        </label>
                        <asp:TextBox ID="txtSaturdayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('saturday')" placeholder="17:00" />
                    </div>
                    <div class="input-group">
                        <label>
                            <i class="material-icons">pause</i>
                            Break (minutes)
                        </label>
                        <asp:TextBox ID="txtSaturdayBreak" runat="server" CssClass="form-control break-input" 
                            TextMode="Number" Text="30" onchange="calculateDayTotal('saturday')" 
                            min="0" max="480" step="15" />
                    </div>
                    <div class="input-group time-summary">
                        <label>Summary</label>
                        <div class="daily-hours-display">
                            <span id="saturdayHours">0.0 hours</span>
                            <small id="saturdayBreakdown">Regular: 0.0h | OT: 0.0h</small>
                        </div>
                    </div>
                </div>
                <div class="notes-section">
                    <label>
                        <i class="material-icons">note</i>
                        Notes (optional)
                    </label>
                    <asp:TextBox ID="txtSaturdayNotes" runat="server" CssClass="form-control notes-input" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>

        <!-- Sunday -->
        <div class="day-entry-card edit-mode weekend">
            <div class="day-header">
                <div class="day-info">
                    <i class="material-icons day-icon sunday">today</i>
                    <div>
                        <h4>Sunday</h4>
                        <span class="day-date"><asp:Literal ID="litSundayDate" runat="server"></asp:Literal></span>
                        <span class="weekend-badge">Weekend</span>
                    </div>
                    <div class="day-status">
                        <span class="status-indicator" id="sundayStatus">Not Started</span>
                    </div>
                </div>
                <div class="day-total">
                    <span class="total-label">Total:</span>
                    <span class="total-hours" id="sundayTotal">0.0h</span>
                </div>
            </div>
            <div class="day-content">
                <div class="time-inputs-grid">
                    <div class="input-group">
                        <label>
                            <i class="material-icons">play_arrow</i>
                            Start Time
                        </label>
                        <asp:TextBox ID="txtSundayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('sunday')" placeholder="09:00" />
                    </div>
                    <div class="input-group">
                        <label>
                            <i class="material-icons">stop</i>
                            End Time
                        </label>
                        <asp:TextBox ID="txtSundayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('sunday')" placeholder="17:00" />
                    </div>
                    <div class="input-group">
                        <label>
                            <i class="material-icons">pause</i>
                            Break (minutes)
                        </label>
                        <asp:TextBox ID="txtSundayBreak" runat="server" CssClass="form-control break-input" 
                            TextMode="Number" Text="30" onchange="calculateDayTotal('sunday')" 
                            min="0" max="480" step="15" />
                    </div>
                    <div class="input-group time-summary">
                        <label>Summary</label>
                        <div class="daily-hours-display">
                            <span id="sundayHours">0.0 hours</span>
                            <small id="sundayBreakdown">Regular: 0.0h | OT: 0.0h</small>
                        </div>
                    </div>
                </div>
                <div class="notes-section">
                    <label>
                        <i class="material-icons">note</i>
                        Notes (optional)
                    </label>
                    <asp:TextBox ID="txtSundayNotes" runat="server" CssClass="form-control notes-input" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>
    </div>

    <!-- Timesheet Notes Section -->
    <div class="timesheet-notes-section">
        <div class="section-header">
            <h3>
                <i class="material-icons">note_add</i>
                Additional Notes
            </h3>
            <p>Add any additional comments or information about this timesheet period</p>
        </div>
        <div class="notes-card">
            <asp:TextBox ID="txtTimesheetNotes" runat="server" CssClass="form-control notes-input" 
                TextMode="MultiLine" Rows="4" 
                placeholder="Add any additional notes or comments about this timesheet period..." />
        </div>
    </div>

    <!-- Action Buttons -->
    <div class="actions-section">
        <div class="actions-card">
            <div class="primary-actions">
                <asp:Button ID="btnSaveDraft" runat="server" Text="💾 Save as Draft" 
                    CssClass="btn btn-outline-primary btn-lg" OnClick="btnSaveDraft_Click" />
                <asp:Button ID="btnSubmitTimesheet" runat="server" Text="🚀 Submit for Approval" 
                    CssClass="btn btn-success btn-lg" OnClick="btnSubmitTimesheet_Click" 
                    OnClientClick="return confirmSubmission();" />
            </div>
            <div class="secondary-actions">
                <asp:Button ID="btnDelete" runat="server" Text="🗑️ Delete Timesheet" 
                    CssClass="btn btn-danger" OnClick="btnDelete_Click" 
                    OnClientClick="return confirmDeletion();" />
                <asp:Button ID="btnCancel" runat="server" Text="← Cancel" 
                    CssClass="btn btn-outline-secondary" OnClick="btnBackToTimesheets_Click" />
            </div>
        </div>
    </div>

    <!-- Enhanced JavaScript for calculations and UX -->
    <script type="text/javascript">
        // Time calculation functions
        function calculateDayTotal(day) {
            const startInput = document.getElementById('txt' + capitalizeFirst(day) + 'Start');
            const endInput = document.getElementById('txt' + capitalizeFirst(day) + 'End');
            const breakInput = document.getElementById('txt' + capitalizeFirst(day) + 'Break');

            if (!startInput.value || !endInput.value) {
                updateDayDisplay(day, 0, 0, 0);
                updateWeeklyTotal();
                return;
            }

            const start = new Date('2000-01-01 ' + startInput.value);
            const end = new Date('2000-01-01 ' + endInput.value);
            const breakMinutes = parseInt(breakInput.value) || 0;

            // Handle overnight shifts
            if (end < start) {
                end.setDate(end.getDate() + 1);
            }

            const totalMinutes = (end - start) / (1000 * 60) - breakMinutes;
            const totalHours = Math.max(0, totalMinutes / 60);
            const regularHours = Math.min(8, totalHours);
            const overtimeHours = Math.max(0, totalHours - 8);

            updateDayDisplay(day, totalHours, regularHours, overtimeHours);
            updateWeeklyTotal();
        }

        function updateDayDisplay(day, totalHours, regularHours, overtimeHours) {
            const totalElement = document.getElementById(day + 'Total');
            const hoursElement = document.getElementById(day + 'Hours');
            const breakdownElement = document.getElementById(day + 'Breakdown');
            const statusElement = document.getElementById(day + 'Status');

            if (totalElement) totalElement.textContent = totalHours.toFixed(1) + 'h';
            if (hoursElement) hoursElement.textContent = totalHours.toFixed(1) + ' hours';
            if (breakdownElement) breakdownElement.textContent = `Regular: ${regularHours.toFixed(1)}h | OT: ${overtimeHours.toFixed(1)}h`;

            if (statusElement) {
                if (totalHours > 0) {
                    statusElement.textContent = 'Completed';
                    statusElement.className = 'status-indicator completed';
                } else {
                    statusElement.textContent = 'Not Started';
                    statusElement.className = 'status-indicator not-started';
                }
            }
        }

        function updateWeeklyTotal() {
            const days = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday'];
            let totalWeeklyHours = 0;
            let totalRegularHours = 0;
            let totalOvertimeHours = 0;
            let daysWorked = 0;

            days.forEach(day => {
                const startInput = document.getElementById('txt' + capitalizeFirst(day) + 'Start');
                const endInput = document.getElementById('txt' + capitalizeFirst(day) + 'End');
                const breakInput = document.getElementById('txt' + capitalizeFirst(day) + 'Break');

                if (startInput?.value && endInput?.value) {
                    const start = new Date('2000-01-01 ' + startInput.value);
                    const end = new Date('2000-01-01 ' + endInput.value);
                    const breakMinutes = parseInt(breakInput?.value) || 0;

                    if (end >= start) {
                        const totalMinutes = (end - start) / (1000 * 60) - breakMinutes;
                        const dayHours = Math.max(0, totalMinutes / 60);

                        if (dayHours > 0) {
                            totalWeeklyHours += dayHours;
                            totalRegularHours += Math.min(8, dayHours);
                            totalOvertimeHours += Math.max(0, dayHours - 8);
                            daysWorked++;
                        }
                    }
                }
            });

            // Update summary display
            const totalHoursElement = document.querySelector('[id$="litTotalHours"]');
            const regularHoursElement = document.querySelector('[id$="litRegularHours"]');
            const overtimeHoursElement = document.querySelector('[id$="litOvertimeHours"]');
            const daysWorkedElement = document.querySelector('[id$="litDaysWorked"]');
            const progressBar = document.getElementById('totalHoursProgress');

            if (totalHoursElement) totalHoursElement.textContent = totalWeeklyHours.toFixed(1);
            if (regularHoursElement) regularHoursElement.textContent = totalRegularHours.toFixed(1);
            if (overtimeHoursElement) overtimeHoursElement.textContent = totalOvertimeHours.toFixed(1);
            if (daysWorkedElement) daysWorkedElement.textContent = daysWorked;

            if (progressBar) {
                const progressPercent = Math.min(100, (totalWeeklyHours / 40) * 100);
                progressBar.style.width = progressPercent + '%';
            }
        }

        function capitalizeFirst(str) {
            return str.charAt(0).toUpperCase() + str.slice(1);
        }

        function fillStandardHours() {
            if (!confirm('This will fill Monday-Friday with standard 9 AM - 5 PM hours with 1-hour lunch breaks. Continue?')) return;

            const workdays = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday'];
            workdays.forEach(day => {
                const startInput = document.getElementById('txt' + capitalizeFirst(day) + 'Start');
                const endInput = document.getElementById('txt' + capitalizeFirst(day) + 'End');
                const breakInput = document.getElementById('txt' + capitalizeFirst(day) + 'Break');

                if (startInput) startInput.value = '09:00';
                if (endInput) endInput.value = '17:00';
                if (breakInput) breakInput.value = '60';

                calculateDayTotal(day);
            });

            showAutoSaveIndicator();
        }

        function fillLastWeek() {
            alert('Copy Last Week functionality would connect to server to retrieve previous week data.');
        }

        function clearAllTimes() {
            if (!confirm('This will clear all time entries. Are you sure?')) return;

            const days = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday'];
            days.forEach(day => {
                const startInput = document.getElementById('txt' + capitalizeFirst(day) + 'Start');
                const endInput = document.getElementById('txt' + capitalizeFirst(day) + 'End');
                const breakInput = document.getElementById('txt' + capitalizeFirst(day) + 'Break');
                const notesInput = document.getElementById('txt' + capitalizeFirst(day) + 'Notes');

                if (startInput) startInput.value = '';
                if (endInput) endInput.value = '';
                if (breakInput) breakInput.value = '30';
                if (notesInput) notesInput.value = '';

                calculateDayTotal(day);
            });

            // Clear timesheet notes
            const timesheetNotes = document.querySelector('[id$="txtTimesheetNotes"]');
            if (timesheetNotes) timesheetNotes.value = '';

            showAutoSaveIndicator();
        }

        function showAutoSaveIndicator() {
            const indicator = document.getElementById('autoSaveIndicator');
            if (indicator) {
                indicator.style.display = 'flex';
                setTimeout(() => {
                    indicator.style.display = 'none';
                }, 2000);
            }
        }

        function confirmSubmission() {
            const totalHours = parseFloat(document.querySelector('[id$="litTotalHours"]')?.textContent || '0');

            let message = 'Are you sure you want to submit this timesheet for approval?\n\n';
            message += `Total Hours: ${totalHours.toFixed(1)}h\n`;
            message += 'Once submitted, you will not be able to edit it until it is approved or rejected.';

            if (totalHours === 0) {
                message = 'This timesheet has no hours entered. Are you sure you want to submit it?';
            }

            return confirm(message);
        }

        function confirmDeletion() {
            return confirm('Are you sure you want to delete this timesheet? This action cannot be undone.');
        }

        // Initialize calculations on page load
        document.addEventListener('DOMContentLoaded', function () {
            const days = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday'];
            days.forEach(day => {
                calculateDayTotal(day);
            });

            // Add event listeners for auto-save
            const inputs = document.querySelectorAll('.time-input, .break-input, .notes-input');
            inputs.forEach(input => {
                input.addEventListener('change', () => {
                    setTimeout(showAutoSaveIndicator, 500);
                });
            });
        });

        // Prevent form submission on Enter key in time inputs
        document.addEventListener('keydown', function (e) {
            if (e.key === 'Enter' && (e.target.classList.contains('time-input') || e.target.classList.contains('break-input'))) {
                e.preventDefault();
                e.target.blur();
            }
        });
    </script>

</asp:Content>