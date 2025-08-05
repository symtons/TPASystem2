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

    <!-- Header Section -->
    <div class="timesheet-edit-header">
        <div class="header-content">
            <div class="header-info">
                <div class="breadcrumb">
                    <asp:Button ID="btnBackToTimesheets" runat="server" Text="← Back to Timesheets" 
                        CssClass="btn btn-secondary btn-sm" OnClick="btnBackToTimesheets_Click" />
                </div>
                <h1 class="page-title">
                    <i class="material-icons">edit</i>
                    Edit Timesheet
                </h1>
                <div class="timesheet-info">
                    <div class="info-item">
                        <i class="material-icons">person</i>
                        <span><asp:Literal ID="litEmployeeName" runat="server" Text="Employee Name"></asp:Literal></span>
                    </div>
                    <div class="info-item">
                        <i class="material-icons">date_range</i>
                        <span><asp:Literal ID="litWeekPeriod" runat="server" Text="Week Period"></asp:Literal></span>
                    </div>
                    <div class="info-item status-item">
                        <i class="material-icons">info</i>
                        <span class="status-badge" id="statusBadge" runat="server">
                            <asp:Literal ID="litTimesheetStatus" runat="server" Text="Draft"></asp:Literal>
                        </span>
                    </div>
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnSaveProgress" runat="server" Text="💾 Save Progress" 
                    CssClass="btn btn-outline-secondary" OnClick="btnSaveProgress_Click" />
                <asp:Button ID="btnPreviewTimesheet" runat="server" Text="👁️ Preview" 
                    CssClass="btn btn-outline-primary" OnClick="btnPreviewTimesheet_Click" />
            </div>
        </div>
    </div>

    <!-- Auto-save Indicator -->
    <div id="autoSaveIndicator" class="auto-save-indicator" style="display: none;">
        <i class="material-icons">cloud_done</i>
        <span>Changes saved automatically</span>
    </div>

    <!-- Weekly Summary Section -->
    <div class="timesheet-summary-section">
        <div class="summary-header">
            <h3>
                <i class="material-icons">analytics</i>
                Weekly Summary
            </h3>
            <p class="summary-subtitle">Hours are calculated automatically as you enter your daily times</p>
        </div>
        
        <div class="summary-grid">
            <div class="summary-item">
                <div class="summary-label">Total Hours</div>
                <div class="summary-value total-hours">
                    <asp:Literal ID="litTotalHours" runat="server" Text="0.0"></asp:Literal>h
                </div>
                <div class="summary-progress">
                    <div class="progress-bar">
                        <div class="progress-fill" id="totalHoursProgress" style="width: 0%"></div>
                    </div>
                    <span class="progress-text">of 40h weekly target</span>
                </div>
            </div>
            <div class="summary-item">
                <div class="summary-label">Regular Hours</div>
                <div class="summary-value">
                    <asp:Literal ID="litRegularHours" runat="server" Text="0.0"></asp:Literal>h
                </div>
            </div>
            <div class="summary-item">
                <div class="summary-label">Overtime Hours</div>
                <div class="summary-value overtime">
                    <asp:Literal ID="litOvertimeHours" runat="server" Text="0.0"></asp:Literal>h
                </div>
            </div>
            <div class="summary-item">
                <div class="summary-label">Days Worked</div>
                <div class="summary-value">
                    <asp:Literal ID="litDaysWorked" runat="server" Text="0"></asp:Literal>
                </div>
            </div>
        </div>
    </div>

    <!-- Daily Time Entries Section -->
    <div class="daily-entries-section">
        <div class="section-title">
            <h2>
                <i class="material-icons">schedule</i>
                Daily Time Entries
            </h2>
            <p>Enter your work hours for each day of the week. Times are automatically calculated.</p>
        </div>

        <!-- Quick Fill Options -->
        <div class="quick-fill-section">
            <div class="quick-fill-header">
                <h4><i class="material-icons">flash_on</i> Quick Fill Options</h4>
            </div>
            <div class="quick-fill-buttons">
                <button type="button" class="btn btn-outline-primary btn-sm" onclick="fillStandardWeek()">
                    <i class="material-icons">work</i> Standard Week (9-5)
                </button>
                <button type="button" class="btn btn-outline-primary btn-sm" onclick="fillLastWeek()">
                    <i class="material-icons">history</i> Copy Last Week
                </button>
                <button type="button" class="btn btn-outline-secondary btn-sm" onclick="clearAllTimes()">
                    <i class="material-icons">clear_all</i> Clear All
                </button>
            </div>
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
                <div class="time-inputs">
                    <div class="input-group">
                        <label><i class="material-icons">play_arrow</i> Start Time</label>
                        <asp:TextBox ID="txtMondayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('monday')" placeholder="09:00" />
                    </div>
                    <div class="input-group">
                        <label><i class="material-icons">stop</i> End Time</label>
                        <asp:TextBox ID="txtMondayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('monday')" placeholder="17:00" />
                    </div>
                    <div class="input-group">
                        <label><i class="material-icons">pause</i> Break Duration (minutes)</label>
                        <asp:TextBox ID="txtMondayBreak" runat="server" CssClass="form-control break-input" 
                            TextMode="Number" Text="30" onchange="calculateDayTotal('monday')" 
                            min="0" max="480" step="15" />
                    </div>
                    <div class="input-group time-summary">
                        <label>Daily Summary</label>
                        <div class="daily-hours-display">
                            <span id="mondayHours">0.0 hours</span>
                            <small id="mondayBreakdown">Regular: 0.0h | OT: 0.0h</small>
                        </div>
                    </div>
                </div>
                <div class="notes-section">
                    <label><i class="material-icons">note</i> Notes (optional)</label>
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
                <div class="time-inputs">
                    <div class="input-group">
                        <label><i class="material-icons">play_arrow</i> Start Time</label>
                        <asp:TextBox ID="txtTuesdayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('tuesday')" placeholder="09:00" />
                    </div>
                    <div class="input-group">
                        <label><i class="material-icons">stop</i> End Time</label>
                        <asp:TextBox ID="txtTuesdayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('tuesday')" placeholder="17:00" />
                    </div>
                    <div class="input-group">
                        <label><i class="material-icons">pause</i> Break Duration (minutes)</label>
                        <asp:TextBox ID="txtTuesdayBreak" runat="server" CssClass="form-control break-input" 
                            TextMode="Number" Text="30" onchange="calculateDayTotal('tuesday')" 
                            min="0" max="480" step="15" />
                    </div>
                    <div class="input-group time-summary">
                        <label>Daily Summary</label>
                        <div class="daily-hours-display">
                            <span id="tuesdayHours">0.0 hours</span>
                            <small id="tuesdayBreakdown">Regular: 0.0h | OT: 0.0h</small>
                        </div>
                    </div>
                </div>
                <div class="notes-section">
                    <label><i class="material-icons">note</i> Notes (optional)</label>
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
                <div class="time-inputs">
                    <div class="input-group">
                        <label><i class="material-icons">play_arrow</i> Start Time</label>
                        <asp:TextBox ID="txtWednesdayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('wednesday')" placeholder="09:00" />
                    </div>
                    <div class="input-group">
                        <label><i class="material-icons">stop</i> End Time</label>
                        <asp:TextBox ID="txtWednesdayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('wednesday')" placeholder="17:00" />
                    </div>
                    <div class="input-group">
                        <label><i class="material-icons">pause</i> Break Duration (minutes)</label>
                        <asp:TextBox ID="txtWednesdayBreak" runat="server" CssClass="form-control break-input" 
                            TextMode="Number" Text="30" onchange="calculateDayTotal('wednesday')" 
                            min="0" max="480" step="15" />
                    </div>
                    <div class="input-group time-summary">
                        <label>Daily Summary</label>
                        <div class="daily-hours-display">
                            <span id="wednesdayHours">0.0 hours</span>
                            <small id="wednesdayBreakdown">Regular: 0.0h | OT: 0.0h</small>
                        </div>
                    </div>
                </div>
                <div class="notes-section">
                    <label><i class="material-icons">note</i> Notes (optional)</label>
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
                <div class="time-inputs">
                    <div class="input-group">
                        <label><i class="material-icons">play_arrow</i> Start Time</label>
                        <asp:TextBox ID="txtThursdayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('thursday')" placeholder="09:00" />
                    </div>
                    <div class="input-group">
                        <label><i class="material-icons">stop</i> End Time</label>
                        <asp:TextBox ID="txtThursdayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('thursday')" placeholder="17:00" />
                    </div>
                    <div class="input-group">
                        <label><i class="material-icons">pause</i> Break Duration (minutes)</label>
                        <asp:TextBox ID="txtThursdayBreak" runat="server" CssClass="form-control break-input" 
                            TextMode="Number" Text="30" onchange="calculateDayTotal('thursday')" 
                            min="0" max="480" step="15" />
                    </div>
                    <div class="input-group time-summary">
                        <label>Daily Summary</label>
                        <div class="daily-hours-display">
                            <span id="thursdayHours">0.0 hours</span>
                            <small id="thursdayBreakdown">Regular: 0.0h | OT: 0.0h</small>
                        </div>
                    </div>
                </div>
                <div class="notes-section">
                    <label><i class="material-icons">note</i> Notes (optional)</label>
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
                <div class="time-inputs">
                    <div class="input-group">
                        <label><i class="material-icons">play_arrow</i> Start Time</label>
                        <asp:TextBox ID="txtFridayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('friday')" placeholder="09:00" />
                    </div>
                    <div class="input-group">
                        <label><i class="material-icons">stop</i> End Time</label>
                        <asp:TextBox ID="txtFridayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('friday')" placeholder="17:00" />
                    </div>
                    <div class="input-group">
                        <label><i class="material-icons">pause</i> Break Duration (minutes)</label>
                        <asp:TextBox ID="txtFridayBreak" runat="server" CssClass="form-control break-input" 
                            TextMode="Number" Text="30" onchange="calculateDayTotal('friday')" 
                            min="0" max="480" step="15" />
                    </div>
                    <div class="input-group time-summary">
                        <label>Daily Summary</label>
                        <div class="daily-hours-display">
                            <span id="fridayHours">0.0 hours</span>
                            <small id="fridayBreakdown">Regular: 0.0h | OT: 0.0h</small>
                        </div>
                    </div>
                </div>
                <div class="notes-section">
                    <label><i class="material-icons">note</i> Notes (optional)</label>
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
                <div class="time-inputs">
                    <div class="input-group">
                        <label><i class="material-icons">play_arrow</i> Start Time</label>
                        <asp:TextBox ID="txtSaturdayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('saturday')" placeholder="09:00" />
                    </div>
                    <div class="input-group">
                        <label><i class="material-icons">stop</i> End Time</label>
                        <asp:TextBox ID="txtSaturdayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('saturday')" placeholder="17:00" />
                    </div>
                    <div class="input-group">
                        <label><i class="material-icons">pause</i> Break Duration (minutes)</label>
                        <asp:TextBox ID="txtSaturdayBreak" runat="server" CssClass="form-control break-input" 
                            TextMode="Number" Text="30" onchange="calculateDayTotal('saturday')" 
                            min="0" max="480" step="15" />
                    </div>
                    <div class="input-group time-summary">
                        <label>Daily Summary</label>
                        <div class="daily-hours-display">
                            <span id="saturdayHours">0.0 hours</span>
                            <small id="saturdayBreakdown">Regular: 0.0h | OT: 0.0h</small>
                        </div>
                    </div>
                </div>
                <div class="notes-section">
                    <label><i class="material-icons">note</i> Notes (optional)</label>
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
                <div class="time-inputs">
                    <div class="input-group">
                        <label><i class="material-icons">play_arrow</i> Start Time</label>
                        <asp:TextBox ID="txtSundayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('sunday')" placeholder="09:00" />
                    </div>
                    <div class="input-group">
                        <label><i class="material-icons">stop</i> End Time</label>
                        <asp:TextBox ID="txtSundayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('sunday')" placeholder="17:00" />
                    </div>
                    <div class="input-group">
                        <label><i class="material-icons">pause</i> Break Duration (minutes)</label>
                        <asp:TextBox ID="txtSundayBreak" runat="server" CssClass="form-control break-input" 
                            TextMode="Number" Text="30" onchange="calculateDayTotal('sunday')" 
                            min="0" max="480" step="15" />
                    </div>
                    <div class="input-group time-summary">
                        <label>Daily Summary</label>
                        <div class="daily-hours-display">
                            <span id="sundayHours">0.0 hours</span>
                            <small id="sundayBreakdown">Regular: 0.0h | OT: 0.0h</small>
                        </div>
                    </div>
                </div>
                <div class="notes-section">
                    <label><i class="material-icons">note</i> Notes (optional)</label>
                    <asp:TextBox ID="txtSundayNotes" runat="server" CssClass="form-control notes-input" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>
    </div>

    <!-- Timesheet Notes Section -->
    <div class="timesheet-notes-section">
        <div class="section-title">
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
            updateDayStatus(day, totalHours);
            updateWeeklyTotal();

            // Show auto-save indicator
            showAutoSaveIndicator();
        }

        function updateDayDisplay(day, total, regular, overtime) {
            const totalElement = document.getElementById(day + 'Total');
            const hoursElement = document.getElementById(day + 'Hours');
            const breakdownElement = document.getElementById(day + 'Breakdown');

            if (totalElement) totalElement.textContent = total.toFixed(1) + 'h';
            if (hoursElement) hoursElement.textContent = total.toFixed(1) + ' hours';
            if (breakdownElement) {
                breakdownElement.textContent = `Regular: ${regular.toFixed(1)}h | OT: ${overtime.toFixed(1)}h`;
            }
        }

        function updateDayStatus(day, totalHours) {
            const statusElement = document.getElementById(day + 'Status');
            if (!statusElement) return;

            let status = 'Not Started';
            let className = 'status-not-started';

            if (totalHours > 0) {
                if (totalHours >= 8) {
                    status = 'Full Day';
                    className = 'status-full-day';
                } else if (totalHours >= 4) {
                    status = 'Partial Day';
                    className = 'status-partial-day';
                } else {
                    status = 'Started';
                    className = 'status-started';
                }
            }

            statusElement.textContent = status;
            statusElement.className = 'status-indicator ' + className;
        }

        function updateWeeklyTotal() {
            const days = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday'];
            let totalWeekHours = 0;
            let totalRegularHours = 0;
            let totalOvertimeHours = 0;
            let daysWorked = 0;

            days.forEach(day => {
                const startInput = document.getElementById('txt' + capitalizeFirst(day) + 'Start');
                const endInput = document.getElementById('txt' + capitalizeFirst(day) + 'End');
                const breakInput = document.getElementById('txt' + capitalizeFirst(day) + 'Break');

                if (startInput && endInput && startInput.value && endInput.value) {
                    const start = new Date('2000-01-01 ' + startInput.value);
                    const end = new Date('2000-01-01 ' + endInput.value);
                    const breakMinutes = parseInt(breakInput.value) || 0;

                    if (end < start) end.setDate(end.getDate() + 1);

                    const totalMinutes = (end - start) / (1000 * 60) - breakMinutes;
                    const dayHours = Math.max(0, totalMinutes / 60);

                    if (dayHours > 0) {
                        daysWorked++;
                        totalWeekHours += dayHours;

                        const regularHours = Math.min(8, dayHours);
                        const overtimeHours = Math.max(0, dayHours - 8);

                        totalRegularHours += regularHours;
                        totalOvertimeHours += overtimeHours;
                    }
                }
            });

            // Update server-side controls
            const totalHoursLit = document.querySelector('[id$="litTotalHours"]');
            const regularHoursLit = document.querySelector('[id$="litRegularHours"]');
            const overtimeHoursLit = document.querySelector('[id$="litOvertimeHours"]');
            const daysWorkedLit = document.querySelector('[id$="litDaysWorked"]');

            if (totalHoursLit) totalHoursLit.textContent = totalWeekHours.toFixed(1);
            if (regularHoursLit) regularHoursLit.textContent = totalRegularHours.toFixed(1);
            if (overtimeHoursLit) overtimeHoursLit.textContent = totalOvertimeHours.toFixed(1);
            if (daysWorkedLit) daysWorkedLit.textContent = daysWorked;

            // Update progress bar
            updateProgressBar(totalWeekHours);
        }

        function updateProgressBar(totalHours) {
            const progressBar = document.getElementById('totalHoursProgress');
            if (progressBar) {
                const percentage = Math.min(100, (totalHours / 40) * 100);
                progressBar.style.width = percentage + '%';

                if (percentage >= 100) {
                    progressBar.style.background = 'linear-gradient(90deg, #10b981, #059669)';
                } else if (percentage >= 75) {
                    progressBar.style.background = 'linear-gradient(90deg, #f59e0b, #d97706)';
                } else {
                    progressBar.style.background = 'linear-gradient(90deg, #667eea, #764ba2)';
                }
            }
        }

        function capitalizeFirst(str) {
            return str.charAt(0).toUpperCase() + str.slice(1);
        }

        // Quick fill functions
        function fillStandardWeek() {
            if (!confirm('This will fill Monday-Friday with 9:00 AM - 5:00 PM schedule. Continue?')) return;

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

    <!-- Additional CSS for enhanced features -->
    <style>
        .auto-save-indicator {
            position: fixed;
            top: 20px;
            right: 20px;
            background: linear-gradient(135deg, #10b981, #059669);
            color: white;
            padding: 0.75rem 1.5rem;
            border-radius: 25px;
            display: flex;
            align-items: center;
            gap: 0.5rem;
            font-weight: 600;
            box-shadow: 0 4px 16px rgba(16, 185, 129, 0.3);
            z-index: 1000;
            animation: slideInFromRight 0.3s ease;
        }

        @keyframes slideInFromRight {
            from { transform: translateX(100%); opacity: 0; }
            to { transform: translateX(0); opacity: 1; }
        }

        .quick-fill-section {
            background: linear-gradient(135deg, #f8fafc, #e2e8f0);
            border-radius: 16px;
            padding: 1.5rem;
            margin-bottom: 2rem;
            border: 1px solid #cbd5e1;
        }

        .quick-fill-header h4 {
            margin: 0 0 1rem 0;
            color: #1e293b;
            font-size: 1.2rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

        .quick-fill-buttons {
            display: flex;
            gap: 1rem;
            flex-wrap: wrap;
        }

        .day-status {
            margin-left: auto;
        }

        .status-indicator {
            padding: 0.25rem 0.75rem;
            border-radius: 12px;
            font-size: 0.8rem;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .status-not-started {
            background: #f1f5f9;
            color: #64748b;
        }

        .status-started {
            background: #fef3c7;
            color: #d97706;
        }

        .status-partial-day {
            background: #dbeafe;
            color: #2563eb;
        }

        .status-full-day {
            background: #dcfce7;
            color: #16a34a;
        }

        .weekend-badge {
            background: linear-gradient(135deg, #f093fb, #f5576c);
            color: white;
            padding: 0.25rem 0.5rem;
            border-radius: 8px;
            font-size: 0.7rem;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            margin-left: 0.5rem;
        }

        .day-entry-card.weekend {
            border-left: 4px solid #f093fb;
        }

        .day-icon {
            transition: all 0.3s ease;
        }

        .day-icon.monday { color: #ef4444; }
        .day-icon.tuesday { color: #f97316; }
        .day-icon.wednesday { color: #eab308; }
        .day-icon.thursday { color: #22c55e; }
        .day-icon.friday { color: #3b82f6; }
        .day-icon.saturday { color: #8b5cf6; }
        .day-icon.sunday { color: #f59e0b; }

        .time-summary {
            background: #f8fafc;
            border-radius: 12px;
            padding: 1rem;
            border: 1px solid #e2e8f0;
        }

        .daily-hours-display {
            display: flex;
            flex-direction: column;
            gap: 0.25rem;
        }

        .daily-hours-display span {
            font-size: 1.1rem;
            font-weight: 700;
            color: #1e293b;
        }

        .daily-hours-display small {
            color: #64748b;
            font-size: 0.8rem;
        }

        .progress-bar {
            width: 100%;
            height: 8px;
            background: #e2e8f0;
            border-radius: 4px;
            overflow: hidden;
            margin: 0.5rem 0;
        }

        .progress-fill {
            height: 100%;
            background: linear-gradient(90deg, #667eea, #764ba2);
            border-radius: 4px;
            transition: width 0.3s ease;
        }

        .progress-text {
            font-size: 0.8rem;
            color: #64748b;
        }

        .summary-progress {
            margin-top: 0.75rem;
            text-align: left;
        }

        .summary-subtitle {
            color: #64748b;
            font-size: 1rem;
            margin-top: 0.5rem;
        }
    </style>

</asp:Content>