<%@ Page Title="Edit Timesheet" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="EditTimesheet.aspx.cs" Inherits="TPASystem2.TimeManagement.EditTimesheet" %>

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
                    <i class="material-icons">edit</i>
                    Edit Timesheet
                </h1>
                <p class="welcome-subtitle">
                    Week of <asp:Literal ID="litWeekPeriod" runat="server"></asp:Literal>
                </p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">person</i>
                        <span>
                            <asp:Literal ID="litEmployeeName" runat="server" Text="Employee Name"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">assignment</i>
                        <span>Status: 
                            <asp:Literal ID="litTimesheetStatus" runat="server" Text="Draft"></asp:Literal>
                        </span>
                    </div>
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnBackToTimesheets" runat="server" Text="Back to Timesheets" 
                    CssClass="btn btn-outline-light" OnClick="btnBackToTimesheets_Click" />
            </div>
        </div>
    </div>

    <!-- Alert Messages -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <i class="material-icons">info</i>
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Timesheet Summary -->
    <div class="timesheet-summary-card">
        <div class="summary-header">
            <h3>
                <i class="material-icons">schedule</i>
                Weekly Summary
            </h3>
        </div>
        
        <div class="summary-grid">
            <div class="summary-item">
                <div class="summary-label">Total Hours</div>
                <div class="summary-value total-hours">
                    <asp:Literal ID="litTotalHours" runat="server" Text="0.0"></asp:Literal>h
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

    <!-- Daily Time Entries -->
    <div class="daily-entries-section">
        <div class="section-title">
            <h2>
                <i class="material-icons">today</i>
                Daily Time Entries
            </h2>
            <p>Enter your work hours for each day of the week</p>
        </div>

        <!-- Monday -->
        <div class="day-entry-card">
            <div class="day-header">
                <div class="day-info">
                    <i class="material-icons">today</i>
                    <div>
                        <h4>Monday</h4>
                        <span class="day-date"><asp:Literal ID="litMondayDate" runat="server"></asp:Literal></span>
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
                        <label>Start Time</label>
                        <asp:TextBox ID="txtMondayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('monday')" />
                    </div>
                    <div class="input-group">
                        <label>End Time</label>
                        <asp:TextBox ID="txtMondayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('monday')" />
                    </div>
                    <div class="input-group">
                        <label>Break Duration (minutes)</label>
                        <asp:TextBox ID="txtMondayBreak" runat="server" CssClass="form-control" 
                            TextMode="Number" Text="0" onchange="calculateDayTotal('monday')" />
                    </div>
                </div>
                <div class="notes-section">
                    <label>Notes (optional)</label>
                    <asp:TextBox ID="txtMondayNotes" runat="server" CssClass="form-control" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>

        <!-- Tuesday -->
        <div class="day-entry-card">
            <div class="day-header">
                <div class="day-info">
                    <i class="material-icons">today</i>
                    <div>
                        <h4>Tuesday</h4>
                        <span class="day-date"><asp:Literal ID="litTuesdayDate" runat="server"></asp:Literal></span>
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
                        <label>Start Time</label>
                        <asp:TextBox ID="txtTuesdayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('tuesday')" />
                    </div>
                    <div class="input-group">
                        <label>End Time</label>
                        <asp:TextBox ID="txtTuesdayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('tuesday')" />
                    </div>
                    <div class="input-group">
                        <label>Break Duration (minutes)</label>
                        <asp:TextBox ID="txtTuesdayBreak" runat="server" CssClass="form-control" 
                            TextMode="Number" Text="0" onchange="calculateDayTotal('tuesday')" />
                    </div>
                </div>
                <div class="notes-section">
                    <label>Notes (optional)</label>
                    <asp:TextBox ID="txtTuesdayNotes" runat="server" CssClass="form-control" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>

        <!-- Wednesday -->
        <div class="day-entry-card">
            <div class="day-header">
                <div class="day-info">
                    <i class="material-icons">today</i>
                    <div>
                        <h4>Wednesday</h4>
                        <span class="day-date"><asp:Literal ID="litWednesdayDate" runat="server"></asp:Literal></span>
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
                        <label>Start Time</label>
                        <asp:TextBox ID="txtWednesdayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('wednesday')" />
                    </div>
                    <div class="input-group">
                        <label>End Time</label>
                        <asp:TextBox ID="txtWednesdayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('wednesday')" />
                    </div>
                    <div class="input-group">
                        <label>Break Duration (minutes)</label>
                        <asp:TextBox ID="txtWednesdayBreak" runat="server" CssClass="form-control" 
                            TextMode="Number" Text="0" onchange="calculateDayTotal('wednesday')" />
                    </div>
                </div>
                <div class="notes-section">
                    <label>Notes (optional)</label>
                    <asp:TextBox ID="txtWednesdayNotes" runat="server" CssClass="form-control" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>

        <!-- Thursday -->
        <div class="day-entry-card">
            <div class="day-header">
                <div class="day-info">
                    <i class="material-icons">today</i>
                    <div>
                        <h4>Thursday</h4>
                        <span class="day-date"><asp:Literal ID="litThursdayDate" runat="server"></asp:Literal></span>
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
                        <label>Start Time</label>
                        <asp:TextBox ID="txtThursdayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('thursday')" />
                    </div>
                    <div class="input-group">
                        <label>End Time</label>
                        <asp:TextBox ID="txtThursdayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('thursday')" />
                    </div>
                    <div class="input-group">
                        <label>Break Duration (minutes)</label>
                        <asp:TextBox ID="txtThursdayBreak" runat="server" CssClass="form-control" 
                            TextMode="Number" Text="0" onchange="calculateDayTotal('thursday')" />
                    </div>
                </div>
                <div class="notes-section">
                    <label>Notes (optional)</label>
                    <asp:TextBox ID="txtThursdayNotes" runat="server" CssClass="form-control" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>

        <!-- Friday -->
        <div class="day-entry-card">
            <div class="day-header">
                <div class="day-info">
                    <i class="material-icons">today</i>
                    <div>
                        <h4>Friday</h4>
                        <span class="day-date"><asp:Literal ID="litFridayDate" runat="server"></asp:Literal></span>
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
                        <label>Start Time</label>
                        <asp:TextBox ID="txtFridayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('friday')" />
                    </div>
                    <div class="input-group">
                        <label>End Time</label>
                        <asp:TextBox ID="txtFridayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('friday')" />
                    </div>
                    <div class="input-group">
                        <label>Break Duration (minutes)</label>
                        <asp:TextBox ID="txtFridayBreak" runat="server" CssClass="form-control" 
                            TextMode="Number" Text="0" onchange="calculateDayTotal('friday')" />
                    </div>
                </div>
                <div class="notes-section">
                    <label>Notes (optional)</label>
                    <asp:TextBox ID="txtFridayNotes" runat="server" CssClass="form-control" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>

        <!-- Saturday -->
        <div class="day-entry-card">
            <div class="day-header">
                <div class="day-info">
                    <i class="material-icons">today</i>
                    <div>
                        <h4>Saturday</h4>
                        <span class="day-date"><asp:Literal ID="litSaturdayDate" runat="server"></asp:Literal></span>
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
                        <label>Start Time</label>
                        <asp:TextBox ID="txtSaturdayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('saturday')" />
                    </div>
                    <div class="input-group">
                        <label>End Time</label>
                        <asp:TextBox ID="txtSaturdayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('saturday')" />
                    </div>
                    <div class="input-group">
                        <label>Break Duration (minutes)</label>
                        <asp:TextBox ID="txtSaturdayBreak" runat="server" CssClass="form-control" 
                            TextMode="Number" Text="0" onchange="calculateDayTotal('saturday')" />
                    </div>
                </div>
                <div class="notes-section">
                    <label>Notes (optional)</label>
                    <asp:TextBox ID="txtSaturdayNotes" runat="server" CssClass="form-control" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>

        <!-- Sunday -->
        <div class="day-entry-card">
            <div class="day-header">
                <div class="day-info">
                    <i class="material-icons">today</i>
                    <div>
                        <h4>Sunday</h4>
                        <span class="day-date"><asp:Literal ID="litSundayDate" runat="server"></asp:Literal></span>
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
                        <label>Start Time</label>
                        <asp:TextBox ID="txtSundayStart" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('sunday')" />
                    </div>
                    <div class="input-group">
                        <label>End Time</label>
                        <asp:TextBox ID="txtSundayEnd" runat="server" CssClass="form-control time-input" 
                            TextMode="Time" onchange="calculateDayTotal('sunday')" />
                    </div>
                    <div class="input-group">
                        <label>Break Duration (minutes)</label>
                        <asp:TextBox ID="txtSundayBreak" runat="server" CssClass="form-control" 
                            TextMode="Number" Text="0" onchange="calculateDayTotal('sunday')" />
                    </div>
                </div>
                <div class="notes-section">
                    <label>Notes (optional)</label>
                    <asp:TextBox ID="txtSundayNotes" runat="server" CssClass="form-control" 
                        TextMode="MultiLine" Rows="2" placeholder="Enter any notes about your work day..." />
                </div>
            </div>
        </div>
    </div>

    <!-- Timesheet Notes -->
    <div class="timesheet-notes-section">
        <div class="section-title">
            <h3>
                <i class="material-icons">note</i>
                Timesheet Notes
            </h3>
        </div>
        <div class="notes-card">
            <asp:TextBox ID="txtTimesheetNotes" runat="server" CssClass="form-control" 
                TextMode="MultiLine" Rows="4" 
                placeholder="Add any additional notes or comments about this timesheet..." />
        </div>
    </div>

    <!-- Action Buttons -->
    <div class="actions-section">
        <div class="actions-card">
            <div class="primary-actions">
                <asp:Button ID="btnSaveDraft" runat="server" Text="Save as Draft" 
                    CssClass="btn btn-outline-primary btn-lg" OnClick="btnSaveDraft_Click" />
                <asp:Button ID="btnSubmitTimesheet" runat="server" Text="Submit for Approval" 
                    CssClass="btn btn-success btn-lg" OnClick="btnSubmitTimesheet_Click" 
                    OnClientClick="return confirm('Are you sure you want to submit this timesheet for approval? You will not be able to edit it once submitted.');" />
            </div>
            <div class="secondary-actions">
                <asp:Button ID="btnDelete" runat="server" Text="Delete Timesheet" 
                    CssClass="btn btn-danger" OnClick="btnDelete_Click" 
                    OnClientClick="return confirm('Are you sure you want to delete this timesheet? This action cannot be undone.');" />
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function calculateDayTotal(day) {
            const startTime = document.getElementById('<%= Page.ClientID %>_txt' + capitalizeFirst(day) + 'Start').value;
            const endTime = document.getElementById('<%= Page.ClientID %>_txt' + capitalizeFirst(day) + 'End').value;
            const breakMinutes = parseInt(document.getElementById('<%= Page.ClientID %>_txt' + capitalizeFirst(day) + 'Break').value) || 0;
            
            if (startTime && endTime) {
                const start = new Date('2000-01-01 ' + startTime);
                const end = new Date('2000-01-01 ' + endTime);
                
                if (end > start) {
                    const diffMs = end - start;
                    const diffHours = (diffMs / (1000 * 60 * 60)) - (breakMinutes / 60);
                    
                    if (diffHours > 0) {
                        document.getElementById(day + 'Total').textContent = diffHours.toFixed(1) + 'h';
                        calculateWeeklyTotal();
                        return;
                    }
                }
            }
            
            document.getElementById(day + 'Total').textContent = '0.0h';
            calculateWeeklyTotal();
        }

        function calculateWeeklyTotal() {
            const days = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday'];
            let totalHours = 0;
            let regularHours = 0;
            let overtimeHours = 0;
            let daysWorked = 0;

            days.forEach(day => {
                const dayTotalText = document.getElementById(day + 'Total').textContent;
                const dayHours = parseFloat(dayTotalText.replace('h', '')) || 0;
                
                if (dayHours > 0) {
                    totalHours += dayHours;
                    daysWorked++;
                    
                    if (dayHours > 8) {
                        regularHours += 8;
                        overtimeHours += (dayHours - 8);
                    } else {
                        regularHours += dayHours;
                    }
                }
            });

            // Update summary
            document.getElementById('<%= litTotalHours.ClientID %>').textContent = totalHours.toFixed(1);
            document.getElementById('<%= litRegularHours.ClientID %>').textContent = regularHours.toFixed(1);
            document.getElementById('<%= litOvertimeHours.ClientID %>').textContent = overtimeHours.toFixed(1);
            document.getElementById('<%= litDaysWorked.ClientID %>').textContent = daysWorked.toString();
        }

        function capitalizeFirst(str) {
            return str.charAt(0).toUpperCase() + str.slice(1);
        }

        // Calculate totals on page load
        document.addEventListener('DOMContentLoaded', function() {
            const days = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday'];
            days.forEach(day => calculateDayTotal(day));
        });
    </script>
</asp:Content>