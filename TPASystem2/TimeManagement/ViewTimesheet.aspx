<%@ Page Title="View Timesheet" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ViewTimesheet.aspx.cs" Inherits="TPASystem2.TimeManagement.ViewTimesheet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

    <!-- Message Panel -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert-panel">
        <div class="alert-content">
            <i class="material-icons alert-icon">info</i>
            <asp:Literal ID="litMessage" runat="server"></asp:Literal>
        </div>
    </asp:Panel>

    <!-- Header Section -->
    <div class="timesheet-view-header">
        <div class="header-content">
            <div class="header-info">
                <div class="breadcrumb">
                    <asp:Button ID="btnBackToTimesheets" runat="server" Text="← Back to Timesheets" 
                        CssClass="btn btn-secondary btn-sm" OnClick="btnBackToTimesheets_Click" />
                </div>
                <h1 class="page-title">
                    <i class="material-icons">assignment</i>
                    View Timesheet
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
                <asp:Button ID="btnEditTimesheet" runat="server" Text="Edit Timesheet" 
                    CssClass="btn btn-primary" OnClick="btnEditTimesheet_Click" />
                <asp:Button ID="btnPrintTimesheet" runat="server" Text="Print" 
                    CssClass="btn btn-outline-secondary" OnClientClick="window.print(); return false;" />
            </div>
        </div>
    </div>

    <!-- Timesheet Summary Section -->
    <div class="timesheet-summary-section">
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

    <!-- Daily Time Entries Section -->
    <div class="daily-entries-section view-mode">
        <div class="section-title">
            <h2>
                <i class="material-icons">today</i>
                Daily Time Entries
            </h2>
            <p>Detailed breakdown of work hours for each day</p>
        </div>

        <!-- Monday -->
        <div class="day-entry-card view-card">
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
                    <span class="total-hours"><asp:Literal ID="litMondayTotal" runat="server" Text="0.0h"></asp:Literal></span>
                </div>
            </div>
            <div class="day-content">
                <div class="time-display">
                    <div class="time-item">
                        <label>Start Time</label>
                        <div class="time-value"><asp:Literal ID="litMondayStart" runat="server" Text="--:--"></asp:Literal></div>
                    </div>
                    <div class="time-item">
                        <label>End Time</label>
                        <div class="time-value"><asp:Literal ID="litMondayEnd" runat="server" Text="--:--"></asp:Literal></div>
                    </div>
                    <div class="time-item">
                        <label>Break Duration</label>
                        <div class="time-value"><asp:Literal ID="litMondayBreak" runat="server" Text="0 min"></asp:Literal></div>
                    </div>
                </div>
                <div class="notes-display" id="mondayNotes" runat="server" visible="false">
                    <label>Notes</label>
                    <div class="notes-content">
                        <asp:Literal ID="litMondayNotes" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>

        <!-- Tuesday -->
        <div class="day-entry-card view-card">
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
                    <span class="total-hours"><asp:Literal ID="litTuesdayTotal" runat="server" Text="0.0h"></asp:Literal></span>
                </div>
            </div>
            <div class="day-content">
                <div class="time-display">
                    <div class="time-item">
                        <label>Start Time</label>
                        <div class="time-value"><asp:Literal ID="litTuesdayStart" runat="server" Text="--:--"></asp:Literal></div>
                    </div>
                    <div class="time-item">
                        <label>End Time</label>
                        <div class="time-value"><asp:Literal ID="litTuesdayEnd" runat="server" Text="--:--"></asp:Literal></div>
                    </div>
                    <div class="time-item">
                        <label>Break Duration</label>
                        <div class="time-value"><asp:Literal ID="litTuesdayBreak" runat="server" Text="0 min"></asp:Literal></div>
                    </div>
                </div>
                <div class="notes-display" id="tuesdayNotes" runat="server" visible="false">
                    <label>Notes</label>
                    <div class="notes-content">
                        <asp:Literal ID="litTuesdayNotes" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>

        <!-- Wednesday -->
        <div class="day-entry-card view-card">
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
                    <span class="total-hours"><asp:Literal ID="litWednesdayTotal" runat="server" Text="0.0h"></asp:Literal></span>
                </div>
            </div>
            <div class="day-content">
                <div class="time-display">
                    <div class="time-item">
                        <label>Start Time</label>
                        <div class="time-value"><asp:Literal ID="litWednesdayStart" runat="server" Text="--:--"></asp:Literal></div>
                    </div>
                    <div class="time-item">
                        <label>End Time</label>
                        <div class="time-value"><asp:Literal ID="litWednesdayEnd" runat="server" Text="--:--"></asp:Literal></div>
                    </div>
                    <div class="time-item">
                        <label>Break Duration</label>
                        <div class="time-value"><asp:Literal ID="litWednesdayBreak" runat="server" Text="0 min"></asp:Literal></div>
                    </div>
                </div>
                <div class="notes-display" id="wednesdayNotes" runat="server" visible="false">
                    <label>Notes</label>
                    <div class="notes-content">
                        <asp:Literal ID="litWednesdayNotes" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>

        <!-- Thursday -->
        <div class="day-entry-card view-card">
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
                    <span class="total-hours"><asp:Literal ID="litThursdayTotal" runat="server" Text="0.0h"></asp:Literal></span>
                </div>
            </div>
            <div class="day-content">
                <div class="time-display">
                    <div class="time-item">
                        <label>Start Time</label>
                        <div class="time-value"><asp:Literal ID="litThursdayStart" runat="server" Text="--:--"></asp:Literal></div>
                    </div>
                    <div class="time-item">
                        <label>End Time</label>
                        <div class="time-value"><asp:Literal ID="litThursdayEnd" runat="server" Text="--:--"></asp:Literal></div>
                    </div>
                    <div class="time-item">
                        <label>Break Duration</label>
                        <div class="time-value"><asp:Literal ID="litThursdayBreak" runat="server" Text="0 min"></asp:Literal></div>
                    </div>
                </div>
                <div class="notes-display" id="thursdayNotes" runat="server" visible="false">
                    <label>Notes</label>
                    <div class="notes-content">
                        <asp:Literal ID="litThursdayNotes" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>

        <!-- Friday -->
        <div class="day-entry-card view-card">
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
                    <span class="total-hours"><asp:Literal ID="litFridayTotal" runat="server" Text="0.0h"></asp:Literal></span>
                </div>
            </div>
            <div class="day-content">
                <div class="time-display">
                    <div class="time-item">
                        <label>Start Time</label>
                        <div class="time-value"><asp:Literal ID="litFridayStart" runat="server" Text="--:--"></asp:Literal></div>
                    </div>
                    <div class="time-item">
                        <label>End Time</label>
                        <div class="time-value"><asp:Literal ID="litFridayEnd" runat="server" Text="--:--"></asp:Literal></div>
                    </div>
                    <div class="time-item">
                        <label>Break Duration</label>
                        <div class="time-value"><asp:Literal ID="litFridayBreak" runat="server" Text="0 min"></asp:Literal></div>
                    </div>
                </div>
                <div class="notes-display" id="fridayNotes" runat="server" visible="false">
                    <label>Notes</label>
                    <div class="notes-content">
                        <asp:Literal ID="litFridayNotes" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>

        <!-- Saturday -->
        <div class="day-entry-card view-card">
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
                    <span class="total-hours"><asp:Literal ID="litSaturdayTotal" runat="server" Text="0.0h"></asp:Literal></span>
                </div>
            </div>
            <div class="day-content">
                <div class="time-display">
                    <div class="time-item">
                        <label>Start Time</label>
                        <div class="time-value"><asp:Literal ID="litSaturdayStart" runat="server" Text="--:--"></asp:Literal></div>
                    </div>
                    <div class="time-item">
                        <label>End Time</label>
                        <div class="time-value"><asp:Literal ID="litSaturdayEnd" runat="server" Text="--:--"></asp:Literal></div>
                    </div>
                    <div class="time-item">
                        <label>Break Duration</label>
                        <div class="time-value"><asp:Literal ID="litSaturdayBreak" runat="server" Text="0 min"></asp:Literal></div>
                    </div>
                </div>
                <div class="notes-display" id="saturdayNotes" runat="server" visible="false">
                    <label>Notes</label>
                    <div class="notes-content">
                        <asp:Literal ID="litSaturdayNotes" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>

        <!-- Sunday -->
        <div class="day-entry-card view-card">
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
                    <span class="total-hours"><asp:Literal ID="litSundayTotal" runat="server" Text="0.0h"></asp:Literal></span>
                </div>
            </div>
            <div class="day-content">
                <div class="time-display">
                    <div class="time-item">
                        <label>Start Time</label>
                        <div class="time-value"><asp:Literal ID="litSundayStart" runat="server" Text="--:--"></asp:Literal></div>
                    </div>
                    <div class="time-item">
                        <label>End Time</label>
                        <div class="time-value"><asp:Literal ID="litSundayEnd" runat="server" Text="--:--"></asp:Literal></div>
                    </div>
                    <div class="time-item">
                        <label>Break Duration</label>
                        <div class="time-value"><asp:Literal ID="litSundayBreak" runat="server" Text="0 min"></asp:Literal></div>
                    </div>
                </div>
                <div class="notes-display" id="sundayNotes" runat="server" visible="false">
                    <label>Notes</label>
                    <div class="notes-content">
                        <asp:Literal ID="litSundayNotes" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Timesheet Notes Section -->
    <div class="timesheet-notes-section view-mode" id="timesheetNotesSection" runat="server" visible="false">
        <div class="section-title">
            <h3>
                <i class="material-icons">note</i>
                Timesheet Notes
            </h3>
        </div>
        <div class="notes-card view-card">
            <div class="notes-content">
                <asp:Literal ID="litTimesheetNotes" runat="server"></asp:Literal>
            </div>
        </div>
    </div>

    <!-- Approval Information Section -->
    <div class="approval-info-section" id="approvalInfoSection" runat="server" visible="false">
        <div class="section-title">
            <h3>
                <i class="material-icons">fact_check</i>
                Approval Information
            </h3>
        </div>
        <div class="approval-card">
            <div class="approval-details">
                <div class="approval-item">
                    <label>Submitted On</label>
                    <div class="approval-value">
                        <i class="material-icons">schedule</i>
                        <asp:Literal ID="litSubmittedAt" runat="server"></asp:Literal>
                    </div>
                </div>
                <div class="approval-item" id="approvedBySection" runat="server" visible="false">
                    <label>Approved By</label>
                    <div class="approval-value">
                        <i class="material-icons">person</i>
                        <asp:Literal ID="litApprovedBy" runat="server"></asp:Literal>
                    </div>
                </div>
                <div class="approval-item" id="approvedAtSection" runat="server" visible="false">
                    <label>Approved On</label>
                    <div class="approval-value">
                        <i class="material-icons">check_circle</i>
                        <asp:Literal ID="litApprovedAt" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Actions Section -->
    <div class="actions-section">
        <div class="actions-card">
            <div class="primary-actions">
                <asp:Button ID="btnBackToList" runat="server" Text="← Back to Timesheets" 
                    CssClass="btn btn-outline-secondary btn-lg" OnClick="btnBackToTimesheets_Click" />
                <asp:Button ID="btnEditTimesheetBottom" runat="server" Text="Edit Timesheet" 
                    CssClass="btn btn-primary btn-lg" OnClick="btnEditTimesheet_Click" />
            </div>
        </div>
    </div>

</asp:Content>