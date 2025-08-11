<%@ Page Title="My Time Tracking" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="EmployeeTimeTracking.aspx.cs" Inherits="TPASystem2.TimeManagement.EmployeeTimeTracking" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

    <!-- Welcome Header -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">access_time</i>
                    My Time Tracking
                </h1>
                <p class="welcome-subtitle">Track your work hours and manage your time entries</p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">person</i>
                        <span>
                            <asp:Literal ID="litEmployeeName" runat="server" Text="Employee Name"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">badge</i>
                        <span>
                            <asp:Literal ID="litEmployeeNumber" runat="server" Text="EMP001"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">schedule</i>
                        <span id="currentTime">Loading...</span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">location_on</i>
                        <span id="userLocation">Detecting location...</span>
                    </div>
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnViewTimesheets" runat="server" Text="View Timesheets" 
                    CssClass="btn btn-outline-light" OnClick="btnViewTimesheets_Click" />
                <asp:Button ID="btnTimeReports" runat="server" Text="Time Reports" 
                    CssClass="btn btn-outline-light" OnClick="btnTimeReports_Click" />
            </div>
        </div>
    </div>

    <!-- Alert Messages -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <i class="material-icons">info</i>
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Current Status Card -->
    <div class="time-status-card">
        <div class="status-header">
            <h3>
                <i class="material-icons">timeline</i>
                Current Status
            </h3>
            <div class="status-indicator" id="statusIndicator">
                <asp:Literal ID="litCurrentStatus" runat="server" Text="Clocked Out"></asp:Literal>
            </div>
        </div>
        
        <div class="status-details">
            <div class="status-item">
                <span class="status-label">Today's Hours:</span>
                <span class="status-value" id="todaysHours">
                    <asp:Literal ID="litTodaysHours" runat="server" Text="0.0"></asp:Literal> hrs
                </span>
            </div>
            <div class="status-item">
                <span class="status-label">Week Total:</span>
                <span class="status-value">
                    <asp:Literal ID="litWeekTotal" runat="server" Text="0.0"></asp:Literal> hrs
                </span>
            </div>
            <div class="status-item">
                <span class="status-label">Break Status:</span>
                <span class="status-value" id="breakStatus">
                    <asp:Literal ID="litBreakStatus" runat="server" Text="Not on break"></asp:Literal>
                </span>
            </div>
            <div class="status-item">
                <span class="status-label">Last Action:</span>
                <span class="status-value">
                    <asp:Literal ID="litLastAction" runat="server" Text="No recent activity"></asp:Literal>
                </span>
            </div>
        </div>
    </div>

    <!-- Live Timer Display -->
    <div class="live-timer-card" id="liveTimerCard" style="display: none;">
        <div class="timer-header">
            <h4>
                <i class="material-icons pulse">timer</i>
                Active Session
            </h4>
            <div class="session-start">
                Started: <span id="sessionStartTime"></span>
            </div>
        </div>
        <div class="timer-display">
            <div class="elapsed-time" id="elapsedTime">00:00:00</div>
            <div class="timer-controls">
                <button type="button" class="btn btn-warning" id="pauseBtn" onclick="toggleBreak()">
                    <i class="material-icons">pause</i> Break
                </button>
                <button type="button" class="btn btn-danger" onclick="quickClockOut()">
                    <i class="material-icons">stop</i> Clock Out
                </button>
            </div>
        </div>
    </div>

    <!-- Clock In/Out Actions -->
    <div class="clock-actions-container">
        <div class="clock-action-card">
            <div class="action-header">
                <h3>Time Clock</h3>
                <p>Record your work time accurately</p>
            </div>
            
            <div class="clock-buttons">
                <asp:Button ID="btnClockIn" runat="server" Text="Clock In" 
                    CssClass="btn btn-success btn-clock-action" 
                    OnClick="btnClockIn_Click" />
                <asp:Button ID="btnClockOut" runat="server" Text="Clock Out" 
                    CssClass="btn btn-danger btn-clock-action" 
                    OnClick="btnClockOut_Click" />
                <asp:Button ID="btnStartBreak" runat="server" Text="Start Break" 
                    CssClass="btn btn-warning btn-clock-action" 
                    OnClick="btnStartBreak_Click" />
                <asp:Button ID="btnEndBreak" runat="server" Text="End Break" 
                    CssClass="btn btn-info btn-clock-action" 
                    OnClick="btnEndBreak_Click" />
            </div>

            <!-- Break Timer -->
            <div class="break-timer-section" id="breakTimerSection" style="display: none;">
                <div class="break-timer">
                    <i class="material-icons">coffee</i>
                    <span>Break Time: </span>
                    <span id="breakElapsed">00:00:00</span>
                </div>
                <div class="break-controls">
                    <button type="button" class="btn btn-sm btn-success" onclick="endBreakEarly()">
                        End Break
                    </button>
                </div>
            </div>
        </div>

        <!-- Quick Entry Form -->
        <div class="quick-entry-card">
            <div class="action-header">
                <h3>Manual Time Entry</h3>
                <p>Add missed time entries</p>
            </div>
            
            <div class="form-group">
                <label class="form-label">Date</label>
                <asp:TextBox ID="txtEntryDate" runat="server" CssClass="form-control" 
                    TextMode="Date" />
                <asp:RequiredFieldValidator ID="rfvEntryDate" runat="server" 
                    ControlToValidate="txtEntryDate" 
                    ErrorMessage="Date is required" 
                    CssClass="field-validation-error" 
                    ValidationGroup="ManualEntry" />
            </div>
            
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Start Time</label>
                    <asp:TextBox ID="txtStartTime" runat="server" CssClass="form-control" 
                        TextMode="Time" />
                    <asp:RequiredFieldValidator ID="rfvStartTime" runat="server" 
                        ControlToValidate="txtStartTime" 
                        ErrorMessage="Start time is required" 
                        CssClass="field-validation-error" 
                        ValidationGroup="ManualEntry" />
                </div>
                <div class="form-group">
                    <label class="form-label">End Time</label>
                    <asp:TextBox ID="txtEndTime" runat="server" CssClass="form-control" 
                        TextMode="Time" />
                    <asp:RequiredFieldValidator ID="rfvEndTime" runat="server" 
                        ControlToValidate="txtEndTime" 
                        ErrorMessage="End time is required" 
                        CssClass="field-validation-error" 
                        ValidationGroup="ManualEntry" />
                </div>
            </div>

            <div class="form-group">
                <label class="form-label">Break Duration (minutes)</label>
                <asp:TextBox ID="txtBreakDuration" runat="server" CssClass="form-control" 
                    TextMode="Number" Text="0" placeholder="0" />
                <asp:RangeValidator ID="rvBreakDuration" runat="server" 
                    ControlToValidate="txtBreakDuration" 
                    MinimumValue="0" MaximumValue="480" Type="Integer"
                    ErrorMessage="Break duration must be between 0 and 480 minutes" 
                    CssClass="field-validation-error" 
                    ValidationGroup="ManualEntry" />
            </div>
            
            <div class="form-group">
                <label class="form-label">Location</label>
                <asp:TextBox ID="txtLocation" runat="server" CssClass="form-control" 
                    placeholder="Office, Home, Client Site, etc." />
            </div>
            
            <div class="form-group">
                <label class="form-label">Notes (Optional)</label>
                <asp:TextBox ID="txtEntryNotes" runat="server" CssClass="form-control" 
                    TextMode="MultiLine" Rows="2" 
                    placeholder="Add any notes about this time entry..." />
            </div>
            
            <asp:Button ID="btnAddEntry" runat="server" Text="Add Time Entry" 
                CssClass="btn btn-primary" OnClick="btnAddEntry_Click" 
                ValidationGroup="ManualEntry" />
            
            <asp:ValidationSummary ID="vsManualEntry" runat="server" 
                ValidationGroup="ManualEntry" CssClass="validation-summary" 
                HeaderText="Please correct the following errors:" />
        </div>
    </div>

    <!-- Today's Schedule Card -->
    <asp:Panel ID="pnlTodaysSchedule" runat="server" CssClass="schedule-card">
        <div class="schedule-header">
            <h3>
                <i class="material-icons">today</i>
                Today's Schedule
            </h3>
            <span class="schedule-date" id="scheduleDate"></span>
        </div>
        <div class="schedule-content">
            <div class="schedule-item">
                <span class="schedule-label">Scheduled Start:</span>
                <span class="schedule-value">
                    <asp:Literal ID="litScheduledStart" runat="server" Text="9:00 AM"></asp:Literal>
                </span>
            </div>
            <div class="schedule-item">
                <span class="schedule-label">Scheduled End:</span>
                <span class="schedule-value">
                    <asp:Literal ID="litScheduledEnd" runat="server" Text="5:00 PM"></asp:Literal>
                </span>
            </div>
            <div class="schedule-item">
                <span class="schedule-label">Expected Hours:</span>
                <span class="schedule-value">
                    <asp:Literal ID="litExpectedHours" runat="server" Text="8.0"></asp:Literal> hrs
                </span>
            </div>
        </div>
    </asp:Panel>

    <!-- Recent Time Entries -->
    <div class="recent-entries-section">
        <div class="section-header">
            <h3>
                <i class="material-icons">history</i>
                Recent Time Entries
            </h3>
            <div class="header-actions">
                <asp:DropDownList ID="ddlEntryFilter" runat="server" CssClass="form-control form-control-sm" 
                    AutoPostBack="true" OnSelectedIndexChanged="FilterChanged">
                    <asp:ListItem Value="7" Text="Last 7 days" Selected="true" />
                    <asp:ListItem Value="14" Text="Last 14 days" />
                    <asp:ListItem Value="30" Text="Last 30 days" />
                </asp:DropDownList>
                <asp:Button ID="btnViewAllEntries" runat="server" Text="View All" 
                    CssClass="btn btn-outline" OnClick="btnViewAllEntries_Click" />
            </div>
        </div>
        
        <div class="entries-container">
            <asp:Repeater ID="rptRecentEntries" runat="server">
                <ItemTemplate>
                    <div class="entry-card">
                        <div class="entry-header">
                            <div class="entry-date">
                                <i class="material-icons">event</i>
                                <%# Convert.ToDateTime(Eval("ClockIn")).ToString("MMM dd, yyyy") %>
                            </div>
                            <div class="entry-status <%# GetEntryStatusClass(Eval("Status").ToString()) %>">
                                <%# Eval("Status") %>
                            </div>
                        </div>
                        
                        <div class="entry-details">
                            <div class="time-detail">
                                <span class="time-label">Clock In:</span>
                                <span class="time-value"><%# Convert.ToDateTime(Eval("ClockIn")).ToString("h:mm tt") %></span>
                            </div>
                            <%# Eval("ClockOut") != DBNull.Value ? 
                                $"<div class=\"time-detail\"><span class=\"time-label\">Clock Out:</span><span class=\"time-value\">{Convert.ToDateTime(Eval("ClockOut")).ToString("h:mm tt")}</span></div>" : 
                                "<div class=\"time-detail\"><span class=\"time-label\">Clock Out:</span><span class=\"time-value text-muted\">Still clocked in</span></div>" %>
                            
                            <%# Convert.ToInt32(Eval("BreakDuration") ?? 0) > 0 ? 
                                $"<div class=\"time-detail\"><span class=\"time-label\">Break Time:</span><span class=\"time-value\">{Eval("BreakDuration")} min</span></div>" : "" %>
                            
                            <div class="time-detail">
                                <span class="time-label">Total Hours:</span>
                                <span class="time-value total-hours">
                                    <%# Eval("TotalHours") != DBNull.Value ? 
                                        Convert.ToDecimal(Eval("TotalHours")).ToString("F1") + " hrs" : 
                                        "In progress" %>
                                </span>
                            </div>
                            
                            <%# !string.IsNullOrEmpty(Eval("Location").ToString()) ? 
                                $"<div class=\"time-detail\"><span class=\"time-label\">Location:</span><span class=\"time-value\">{Eval("Location")}</span></div>" : "" %>
                            
                            <%# !string.IsNullOrEmpty(Eval("Notes").ToString()) ? 
                                $"<div class=\"time-detail\"><span class=\"time-label\">Notes:</span><span class=\"time-value\">{Eval("Notes")}</span></div>" : "" %>
                        </div>
                        
                        <div class="entry-actions">
                            <%# Eval("Status").ToString() == "Active" ? 
                                $"<button type=\"button\" class=\"btn btn-sm btn-danger\" onclick=\"clockOutEntry({Eval("Id")})\"><i class=\"material-icons\">stop</i> Clock Out</button>" : 
                                $"<asp:Button ID=\"btnEditEntry\" runat=\"server\" Text=\"Edit\" CssClass=\"btn btn-sm btn-outline\" CommandName=\"EditEntry\" CommandArgument=\"{Eval("Id")}\" OnCommand=\"EntryAction_Command\" />" %>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            
            <asp:Panel ID="pnlNoEntries" runat="server" Visible="false" CssClass="empty-state">
                <div class="empty-icon">
                    <i class="material-icons">schedule</i>
                </div>
                <h3>No Time Entries Yet</h3>
                <p>Start tracking your time by clocking in above</p>
            </asp:Panel>
        </div>
    </div>

    <!-- Hidden Controls for AJAX Updates -->
    <asp:Button ID="btnHiddenRefresh" runat="server" OnClick="btnHiddenRefresh_Click" 
        style="display: none;" />
    <asp:HiddenField ID="hfActiveEntryId" runat="server" />
    <asp:HiddenField ID="hfClockInTime" runat="server" />
    <asp:HiddenField ID="hfBreakStartTime" runat="server" />
    <asp:HiddenField ID="hfIsOnBreak" runat="server" />

    <!-- JavaScript for Enhanced Functionality -->
    <script type="text/javascript">
        let timerInterval;
        let breakTimerInterval;
        let sessionStartTime;
        let breakStartTime;
        let isOnBreak = false;

        // Initialize page
        $(document).ready(function() {
            updateCurrentTime();
            updateScheduleDate();
            initializeGeolocation();
            checkActiveSession();
            setDefaultEntryDate();
            
            // Start timers
            setInterval(updateCurrentTime, 1000);
            setInterval(refreshStatus, 30000); // Refresh every 30 seconds
        });

        // Update current time display
        function updateCurrentTime() {
            const now = new Date();
            const timeString = now.toLocaleTimeString('en-US', {
                hour: '2-digit',
                minute: '2-digit',
                second: '2-digit',
                hour12: true
            });
            document.getElementById('currentTime').textContent = timeString;
        }

        // Update schedule date
        function updateScheduleDate() {
            const today = new Date();
            const dateString = today.toLocaleDateString('en-US', {
                weekday: 'long',
                year: 'numeric',
                month: 'long',
                day: 'numeric'
            });
            const scheduleElement = document.getElementById('scheduleDate');
            if (scheduleElement) {
                scheduleElement.textContent = dateString;
            }
        }

        // Set default entry date to today
        function setDefaultEntryDate() {
            const today = new Date();
            const dateString = today.toISOString().split('T')[0];
            const entryDateField = document.getElementById('<%= txtEntryDate.ClientID %>');
            if (entryDateField && !entryDateField.value) {
                entryDateField.value = dateString;
            }
        }

        // Initialize geolocation
        function initializeGeolocation() {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    function(position) {
                        document.getElementById('userLocation').textContent = 
                            `Lat: ${position.coords.latitude.toFixed(2)}, Lng: ${position.coords.longitude.toFixed(2)}`;
                    },
                    function(error) {
                        document.getElementById('userLocation').textContent = 'Location unavailable';
                    }
                );
            } else {
                document.getElementById('userLocation').textContent = 'Location not supported';
            }
        }

        // Check for active session on page load
        function checkActiveSession() {
            const activeEntryId = document.getElementById('<%= hfActiveEntryId.ClientID %>').value;
            const clockInTime = document.getElementById('<%= hfClockInTime.ClientID %>').value;
            const isOnBreakValue = document.getElementById('<%= hfIsOnBreak.ClientID %>').value;

            if (activeEntryId && clockInTime) {
                sessionStartTime = new Date(clockInTime);
                isOnBreak = isOnBreakValue === 'true';
                
                showLiveTimer();
                startTimer();
                
                if (isOnBreak) {
                    const breakStart = document.getElementById('<%= hfBreakStartTime.ClientID %>').value;
                    if (breakStart) {
                        breakStartTime = new Date(breakStart);
                        showBreakTimer();
                        startBreakTimer();
                    }
                }
            }
        }

        // Show live timer
        function showLiveTimer() {
            const timerCard = document.getElementById('liveTimerCard');
            const startTimeElement = document.getElementById('sessionStartTime');
            
            if (timerCard && sessionStartTime) {
                timerCard.style.display = 'block';
                startTimeElement.textContent = sessionStartTime.toLocaleTimeString('en-US', {
                    hour: '2-digit',
                    minute: '2-digit',
                    hour12: true
                });
            }
        }

        // Start main timer
        function startTimer() {
            if (timerInterval) clearInterval(timerInterval);
            
            timerInterval = setInterval(function() {
                if (!isOnBreak && sessionStartTime) {
                    const now = new Date();
                    const elapsed = Math.floor((now - sessionStartTime) / 1000);
                    updateTimerDisplay(elapsed);
                }
            }, 1000);
        }

        // Update timer display
        function updateTimerDisplay(totalSeconds) {
            const hours = Math.floor(totalSeconds / 3600);
            const minutes = Math.floor((totalSeconds % 3600) / 60);
            const seconds = totalSeconds % 60;
            
            const timeString = `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
            
            const elapsedElement = document.getElementById('elapsedTime');
            if (elapsedElement) {
                elapsedElement.textContent = timeString;
            }
        }

        // Show break timer
        function showBreakTimer() {
            const breakSection = document.getElementById('breakTimerSection');
            if (breakSection) {
                breakSection.style.display = 'block';
            }
        }

        // Hide break timer
        function hideBreakTimer() {
            const breakSection = document.getElementById('breakTimerSection');
            if (breakSection) {
                breakSection.style.display = 'none';
            }
        }

        // Start break timer
        function startBreakTimer() {
            if (breakTimerInterval) clearInterval(breakTimerInterval);
            
            breakTimerInterval = setInterval(function() {
                if (breakStartTime) {
                    const now = new Date();
                    const elapsed = Math.floor((now - breakStartTime) / 1000);
                    updateBreakTimerDisplay(elapsed);
                }
            }, 1000);
        }

        // Update break timer display
        function updateBreakTimerDisplay(totalSeconds) {
            const hours = Math.floor(totalSeconds / 3600);
            const minutes = Math.floor((totalSeconds % 3600) / 60);
            const seconds = totalSeconds % 60;
            
            const timeString = `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
            
            const breakElement = document.getElementById('breakElapsed');
            if (breakElement) {
                breakElement.textContent = timeString;
            }
        }

        // Toggle break
        function toggleBreak() {
            if (isOnBreak) {
                endBreakEarly();
            } else {
                startBreak();
            }
        }

        // Start break
        function startBreak() {
            const startBreakBtn = document.getElementById('<%= btnStartBreak.ClientID %>');
            if (startBreakBtn) {
                startBreakBtn.click();
            }
        }

        // End break early
        function endBreakEarly() {
            const endBreakBtn = document.getElementById('<%= btnEndBreak.ClientID %>');
            if (endBreakBtn) {
                endBreakBtn.click();
            }
        }

        // Quick clock out
        function quickClockOut() {
            if (confirm('Are you sure you want to clock out?')) {
                const clockOutBtn = document.getElementById('<%= btnClockOut.ClientID %>');
                if (clockOutBtn) {
                    clockOutBtn.click();
                }
            }
        }

        // Clock out specific entry
        function clockOutEntry(entryId) {
            if (confirm('Are you sure you want to clock out this entry?')) {
                // This would need to be implemented as a server-side method
                __doPostBack('ClockOutEntry', entryId);
            }
        }

        // Refresh status without full page reload
        function refreshStatus() {
            const hiddenRefresh = document.getElementById('<%= btnHiddenRefresh.ClientID %>');
            if (hiddenRefresh) {
                hiddenRefresh.click();
            }
        }

        // Form validation helpers
        function validateTimeEntry() {
            const startTime = document.getElementById('<%= txtStartTime.ClientID %>').value;
            const endTime = document.getElementById('<%= txtEndTime.ClientID %>').value;
            
            if (startTime && endTime) {
                const start = new Date('2000-01-01 ' + startTime);
                const end = new Date('2000-01-01 ' + endTime);
                
                if (end <= start) {
                    alert('End time must be after start time.');
                    return false;
                }
            }
            return true;
        }

        // Add validation to the manual entry form
        document.addEventListener('DOMContentLoaded', function() {
            const addEntryBtn = document.getElementById('<%= btnAddEntry.ClientID %>');
            if (addEntryBtn) {
                addEntryBtn.addEventListener('click', function(e) {
                    if (!validateTimeEntry()) {
                        e.preventDefault();
                    }
                });
            }
        });
    </script>
</asp:Content>