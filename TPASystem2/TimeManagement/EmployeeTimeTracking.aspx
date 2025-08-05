<%@ Page Title="My Time Tracking" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="EmployeeTimeTracking.aspx.cs" Inherits="TPASystem2.TimeManagement.EmployeeTimeTracking" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

    <!-- Welcome Header - Matching MyOnboarding Style -->
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
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnViewTimesheets" runat="server" Text="View Timesheets" 
                    CssClass="btn btn-outline-light" OnClick="btnViewTimesheets_Click" />
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
            <div class="status-indicator">
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
                <span class="status-label">Last Action:</span>
                <span class="status-value">
                    <asp:Literal ID="litLastAction" runat="server" Text="No recent activity"></asp:Literal>
                </span>
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
            </div>
            
            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Start Time</label>
                    <asp:TextBox ID="txtStartTime" runat="server" CssClass="form-control" 
                        TextMode="Time" />
                </div>
                <div class="form-group">
                    <label class="form-label">End Time</label>
                    <asp:TextBox ID="txtEndTime" runat="server" CssClass="form-control" 
                        TextMode="Time" />
                </div>
            </div>
            
            <div class="form-group">
                <label class="form-label">Notes (Optional)</label>
                <asp:TextBox ID="txtEntryNotes" runat="server" CssClass="form-control" 
                    TextMode="MultiLine" Rows="2" placeholder="Add any notes about this time entry..." />
            </div>
            
            <asp:Button ID="btnAddEntry" runat="server" Text="Add Time Entry" 
                CssClass="btn btn-primary" OnClick="btnAddEntry_Click" />
        </div>
    </div>

    <!-- Recent Time Entries -->
    <div class="recent-entries-section">
        <div class="section-header">
            <h3>
                <i class="material-icons">history</i>
                Recent Time Entries
            </h3>
            <asp:Button ID="btnViewAllEntries" runat="server" Text="View All" 
                CssClass="btn btn-outline" OnClick="btnViewAllEntries_Click" />
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
                        </div>
                        
                        <div class="entry-actions">
                            <asp:Button ID="btnEditEntry" runat="server" Text="Edit" 
                                CssClass="btn btn-sm btn-outline" 
                                CommandName="EditEntry" CommandArgument='<%# Eval("Id") %>' 
                                OnCommand="EntryAction_Command" />
                            <%# Eval("Status").ToString() == "Active" ? 
                                $"<button type=\"button\" class=\"btn btn-sm btn-danger\" onclick=\"clockOutEntry({Eval("Id")})\">Clock Out</button>" : "" %>
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

    <!-- JavaScript for real-time updates -->
    <script type="text/javascript">
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

        // Update time every second
        setInterval(updateCurrentTime, 1000);
        updateCurrentTime(); // Initial call

        // Auto-refresh today's hours every 30 seconds
        setInterval(function() {
            var hiddenRefresh = document.getElementById('<%= btnHiddenRefresh.ClientID %>');
            if (hiddenRefresh) {
                hiddenRefresh.click();
            }
        }, 30000);

        // Clock out function for active entries
        function clockOutEntry(entryId) {
            if (confirm('Are you sure you want to clock out this entry?')) {
                __doPostBack('<%= btnClockOut.UniqueID %>', entryId);
            }
        }

        // Set today's date as default for manual entry
        document.addEventListener('DOMContentLoaded', function() {
            var dateInput = document.getElementById('<%= txtEntryDate.ClientID %>');
            if (dateInput && !dateInput.value) {
                var today = new Date();
                var yyyy = today.getFullYear();
                var mm = String(today.getMonth() + 1).padStart(2, '0');
                var dd = String(today.getDate()).padStart(2, '0');
                dateInput.value = yyyy + '-' + mm + '-' + dd;
            }
        });
    </script>

    <!-- Hidden button for auto-refresh -->
    <asp:Button ID="btnHiddenRefresh" runat="server" style="display:none;" 
        OnClick="btnHiddenRefresh_Click" />
</asp:Content>