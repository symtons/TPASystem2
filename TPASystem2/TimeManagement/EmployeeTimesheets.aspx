<%@ Page Title="My Timesheets" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="EmployeeTimesheets.aspx.cs" Inherits="TPASystem2.TimeManagement.EmployeeTimesheets" %>

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
                    <i class="material-icons">assignment</i>
                    My Timesheets
                </h1>
                <p class="welcome-subtitle">View, edit, and submit your timesheets for approval</p>
                
                <div class="employee-info">
                    <div class="employee-detail">
                        <i class="material-icons">person</i>
                        <span>
                            <asp:Literal ID="litEmployeeName" runat="server" Text="Employee Name"></asp:Literal>
                        </span>
                    </div>
                    <div class="employee-detail">
                        <i class="material-icons">supervisor_account</i>
                        <span>Program Coordinator: 
                            <asp:Literal ID="litProgramCoordinator" runat="server" Text="Not Assigned"></asp:Literal>
                        </span>
                    </div>
                </div>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnBackToTimeTracking" runat="server" Text="Back to Time Tracking" 
                    CssClass="btn btn-outline-light" OnClick="btnBackToTimeTracking_Click" />
            </div>
        </div>
    </div>

    <!-- Alert Messages -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <i class="material-icons">info</i>
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Quick Stats Summary - Moved to top -->
    <div class="stats-summary-section">
        <div class="section-title">
            <h2>
                <i class="material-icons">dashboard</i>
                Quick Overview
            </h2>
        </div>
        
        <div class="stats-grid">
            <div class="stat-card approved">
                <div class="stat-icon">
                    <i class="material-icons">check_circle</i>
                </div>
                <div class="stat-content">
                    <div class="stat-number"><asp:Literal ID="litApprovedTimesheets" runat="server" Text="0"></asp:Literal></div>
                    <div class="stat-label">Approved Timesheets</div>
                </div>
            </div>
            
            <div class="stat-card hours">
                <div class="stat-icon">
                    <i class="material-icons">schedule</i>
                </div>
                <div class="stat-content">
                    <div class="stat-number"><asp:Literal ID="litTotalHoursThisMonth" runat="server" Text="0.0"></asp:Literal></div>
                    <div class="stat-label">Hours This Month</div>
                </div>
            </div>
            
            <div class="stat-card pending">
                <div class="stat-icon">
                    <i class="material-icons">pending</i>
                </div>
                <div class="stat-content">
                    <div class="stat-number"><asp:Literal ID="litPendingTimesheets" runat="server" Text="0"></asp:Literal></div>
                    <div class="stat-label">Awaiting Approval</div>
                </div>
            </div>
        </div>
    </div>

    <!-- Main Content Area -->
    <div class="main-content-section">
        
        <!-- Filters and Actions Row -->
        <div class="controls-section">
            <div class="filters-container">
                <h3 class="filters-title">
                    <i class="material-icons">tune</i>
                    Filter & Sort
                </h3>
                
                <div class="filters-grid">
                    <div class="filter-item">
                        <label class="filter-label">Status</label>
                        <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="filter-select" 
                            AutoPostBack="true" OnSelectedIndexChanged="FilterChanged">
                            <asp:ListItem Value="" Text="All Statuses" />
                            <asp:ListItem Value="Draft" Text="Draft" />
                            <asp:ListItem Value="Submitted" Text="Submitted" />
                            <asp:ListItem Value="Approved" Text="Approved" />
                            <asp:ListItem Value="Rejected" Text="Rejected" />
                        </asp:DropDownList>
                    </div>
                    
                    <div class="filter-item">
                        <label class="filter-label">Date Range</label>
                        <asp:DropDownList ID="ddlDateFilter" runat="server" CssClass="filter-select" 
                            AutoPostBack="true" OnSelectedIndexChanged="FilterChanged">
                            <asp:ListItem Value="current" Text="Current Week" />
                            <asp:ListItem Value="last" Text="Last Week" />
                            <asp:ListItem Value="month" Text="This Month" />
                            <asp:ListItem Value="all" Text="All Time" />
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            
            <div class="actions-container">
                <asp:Button ID="btnCreateTimesheet" runat="server" Text="Create New Timesheet" 
                    CssClass="btn btn-primary btn-lg create-btn" OnClick="btnCreateTimesheet_Click" />
            </div>
        </div>

        <!-- Timesheets Grid -->
        <div class="timesheets-section">
            <div class="section-title">
                <h2>
                    <i class="material-icons">view_list</i>
                    Your Timesheets
                </h2>
            </div>
            
            <div class="timesheets-grid">
                <asp:Repeater ID="rptTimesheets" runat="server">
                    <ItemTemplate>
                        <div class="timesheet-card-modern">
                            <div class="card-header">
                                <div class="timesheet-period">
                                    <div class="period-badge">
                                        <i class="material-icons">date_range</i>
                                        <span><%# Convert.ToDateTime(Eval("WeekStartDate")).ToString("MMM dd") %> - <%# Convert.ToDateTime(Eval("WeekStartDate")).AddDays(6).ToString("MMM dd") %></span>
                                    </div>
                                    <div class="period-year"><%# Convert.ToDateTime(Eval("WeekStartDate")).ToString("yyyy") %></div>
                                </div>
                                
                                <div class="status-container">
                                    <div class="status-badge <%# GetStatusClass(Eval("Status").ToString()) %>">
                                        <i class="material-icons"><%# GetStatusIcon(Eval("Status").ToString()) %></i>
                                        <span><%# Eval("Status") %></span>
                                    </div>
                                </div>
                            </div>
                            
                            <div class="card-body">
                                <div class="hours-summary">
                                    <div class="hours-main">
                                        <div class="total-hours">
                                            <span class="hours-number"><%# Convert.ToDecimal(Eval("TotalHours")).ToString("F1") %></span>
                                            <span class="hours-label">Total Hours</span>
                                        </div>
                                    </div>
                                    
                                    <div class="hours-breakdown">
                                        <div class="hours-item">
                                            <span class="hours-value"><%# Convert.ToDecimal(Eval("RegularHours")).ToString("F1") %>h</span>
                                            <span class="hours-type">Regular</span>
                                        </div>
                                        <div class="hours-item overtime">
                                            <span class="hours-value"><%# Convert.ToDecimal(Eval("OvertimeHours")).ToString("F1") %>h</span>
                                            <span class="hours-type">Overtime</span>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="timesheet-meta">
                                    <%# Eval("SubmittedAt") != DBNull.Value ? 
                                        $"<div class=\"meta-item\"><i class=\"material-icons\">upload</i><span>Submitted {Convert.ToDateTime(Eval("SubmittedAt")).ToString("MMM dd, yyyy")}</span></div>" : "" %>
                                    <%# Eval("ApprovedAt") != DBNull.Value ? 
                                        $"<div class=\"meta-item\"><i class=\"material-icons\">check_circle</i><span>Approved {Convert.ToDateTime(Eval("ApprovedAt")).ToString("MMM dd, yyyy")}</span></div>" : "" %>
                                </div>
                                
                                <%# !string.IsNullOrEmpty(Eval("Notes").ToString()) ? 
                                    $"<div class=\"timesheet-notes\"><i class=\"material-icons\">note</i><span>{Eval("Notes")}</span></div>" : "" %>
                            </div>
                            
                            <div class="card-actions">
                                <asp:Button ID="btnViewTimesheet" runat="server" Text="View" 
                                    CssClass="btn btn-outline-primary btn-sm" 
                                    CommandName="ViewTimesheet" CommandArgument='<%# Eval("Id") %>' 
                                    OnCommand="TimesheetAction_Command" />
                                
                                <%# Eval("Status").ToString() == "Draft" || Eval("Status").ToString() == "Rejected" ? 
                                    $"<asp:Button ID=\"btnEditTimesheet\" runat=\"server\" Text=\"Edit\" CssClass=\"btn btn-primary btn-sm\" CommandName=\"EditTimesheet\" CommandArgument='{Eval("Id")}' OnCommand=\"TimesheetAction_Command\" />" : "" %>
                                
                                <%# Eval("Status").ToString() == "Draft" ? 
                                    $"<asp:Button ID=\"btnSubmitTimesheet\" runat=\"server\" Text=\"Submit\" CssClass=\"btn btn-success btn-sm\" CommandName=\"SubmitTimesheet\" CommandArgument='{Eval("Id")}' OnCommand=\"TimesheetAction_Command\" OnClientClick=\"return confirm('Submit this timesheet for approval?');\" />" : "" %>
                                
                                <%# Eval("Status").ToString() == "Draft" ? 
                                    $"<asp:Button ID=\"btnDeleteTimesheet\" runat=\"server\" Text=\"Delete\" CssClass=\"btn btn-danger btn-sm\" CommandName=\"DeleteTimesheet\" CommandArgument='{Eval("Id")}' OnCommand=\"TimesheetAction_Command\" OnClientClick=\"return confirm('Delete this timesheet permanently?');\" />" : "" %>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                
                <asp:Panel ID="pnlNoTimesheets" runat="server" Visible="false" CssClass="empty-state-modern">
                    <div class="empty-illustration">
                        <div class="empty-icon">
                            <i class="material-icons">assignment</i>
                        </div>
                        <div class="empty-graphic">
                            <div class="empty-line"></div>
                            <div class="empty-line short"></div>
                            <div class="empty-line"></div>
                        </div>
                    </div>
                    <div class="empty-content">
                        <h3>No Timesheets Found</h3>
                        <p>You haven't created any timesheets yet. Get started by creating your first timesheet.</p>
                        <asp:Button ID="btnCreateFirstTimesheet" runat="server" Text="Create Your First Timesheet" 
                            CssClass="btn btn-primary btn-lg" OnClick="btnCreateTimesheet_Click" />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>

    <!-- Help Section -->
    <div class="help-section">
        <div class="help-container">
            <div class="help-header">
                <h3>
                    <i class="material-icons">help_outline</i>
                    How It Works
                </h3>
            </div>
            
            <div class="help-steps">
                <div class="help-step">
                    <div class="step-number">1</div>
                    <div class="step-content">
                        <h4>Create Timesheet</h4>
                        <p>Create a new timesheet for each work week</p>
                    </div>
                </div>
                
                <div class="help-step">
                    <div class="step-number">2</div>
                    <div class="step-content">
                        <h4>Add Time Entries</h4>
                        <p>Log your daily work hours and activities</p>
                    </div>
                </div>
                
                <div class="help-step">
                    <div class="step-number">3</div>
                    <div class="step-content">
                        <h4>Submit for Approval</h4>
                        <p>Send to your coordinator for review</p>
                    </div>
                </div>
                
                <div class="help-step">
                    <div class="step-number">4</div>
                    <div class="step-content">
                        <h4>Track Status</h4>
                        <p>Monitor approval status and feedback</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>