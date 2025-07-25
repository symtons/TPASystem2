<%@ Page Title="Employee Management" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Employees.aspx.cs" Inherits="TPASystem2.HR.Employees" %>

<%-- REMOVED: The HeadContent section that was causing the error --%>

<asp:Content ID="EmployeeContent" ContentPlaceHolderID="DashboardContent" runat="server">
    
    <%-- Add the CSS directly in the page since HeadContent doesn't exist --%>
    <style type="text/css">
        @import url("../../Content/css/employee-management.css");
    </style>
    
    <!-- Page Header -->
    <div class="page-header">
        <div class="page-header-content">
            <h1>Employee Management</h1>
            <p>Manage employee records and view all staff information</p>
        </div>
        <div class="page-header-actions">
            <asp:Button ID="btnAddEmployee" runat="server" Text="Add New Employee" 
                        CssClass="btn btn-primary" OnClick="btnAddEmployee_Click" />
            <asp:Button ID="btnExportEmployees" runat="server" Text="Export CSV" 
                        CssClass="btn btn-secondary" OnClick="btnExportEmployees_Click" />
        </div>
    </div>

    <!-- Search and Filter Section -->
    <div class="filter-section">
        <div class="filter-card">
            <div class="filter-row">
                <div class="filter-group">
                    <label>Search Employees</label>
                    <div class="search-input">
                        <asp:TextBox ID="txtSearch" runat="server" placeholder="Search by name, email, or employee number" 
                                     CssClass="form-control" AutoPostBack="true" 
                                     OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                    </div>
                </div>
                <div class="filter-group">
                    <label>Department</label>
                    <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-control" 
                                      AutoPostBack="true" OnSelectedIndexChanged="ddlDepartment_SelectedIndexChanged">
                        <asp:ListItem Value="">All Departments</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label>Status</label>
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" 
                                      AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                        <asp:ListItem Value="">All Status</asp:ListItem>
                        <asp:ListItem Value="Active">Active</asp:ListItem>
                        <asp:ListItem Value="Inactive">Inactive</asp:ListItem>
                        <asp:ListItem Value="On Leave">On Leave</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label>Employee Type</label>
                    <asp:DropDownList ID="ddlEmployeeType" runat="server" CssClass="form-control" 
                                      AutoPostBack="true" OnSelectedIndexChanged="ddlEmployeeType_SelectedIndexChanged">
                        <asp:ListItem Value="">All Types</asp:ListItem>
                        <asp:ListItem Value="Full-time">Full-time</asp:ListItem>
                        <asp:ListItem Value="Part-time">Part-time</asp:ListItem>
                        <asp:ListItem Value="Contract">Contract</asp:ListItem>
                        <asp:ListItem Value="Intern">Intern</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-actions">
                    <label>&nbsp;</label>
                    <asp:Button ID="btnClearFilters" runat="server" Text="Clear Filters" 
                                CssClass="btn btn-outline" OnClick="btnClearFilters_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- Employee Statistics -->
    <div class="stats-row">
        <div class="stat-card">
            <div class="stat-icon blue">
                <i class="material-icons">people</i>
            </div>
            <div class="stat-content">
                <h3><asp:Literal ID="litTotalEmployees" runat="server">0</asp:Literal></h3>
                <p>Total Employees</p>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon green">
                <i class="material-icons">check_circle</i>
            </div>
            <div class="stat-content">
                <h3><asp:Literal ID="litActiveEmployees" runat="server">0</asp:Literal></h3>
                <p>Active Employees</p>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon orange">
                <i class="material-icons">person_add</i>
            </div>
            <div class="stat-content">
                <h3><asp:Literal ID="litNewHires" runat="server">0</asp:Literal></h3>
                <p>New Hires (30 days)</p>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon purple">
                <i class="material-icons">business</i>
            </div>
            <div class="stat-content">
                <h3><asp:Literal ID="litDepartmentCount" runat="server">0</asp:Literal></h3>
                <p>Departments</p>
            </div>
        </div>
    </div>

    <!-- Employee Table -->
    <div class="table-container">
        <div class="table-header">
            <div class="table-title">
                <h3>Employee Directory</h3>
                <span>Showing <asp:Literal ID="litRecordCount" runat="server">0</asp:Literal> employees</span>
            </div>
            <div class="table-actions">
                <asp:DropDownList ID="ddlPageSize" runat="server" CssClass="form-control" style="width: auto;" 
                                  AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                    <asp:ListItem Value="10">10 per page</asp:ListItem>
                    <asp:ListItem Value="25" Selected="True">25 per page</asp:ListItem>
                    <asp:ListItem Value="50">50 per page</asp:ListItem>
                    <asp:ListItem Value="100">100 per page</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>

        <asp:GridView ID="gvEmployees" runat="server" CssClass="data-table" 
                      AutoGenerateColumns="false" AllowPaging="true" PageSize="25"
                      OnPageIndexChanging="gvEmployees_PageIndexChanging"
                      OnRowCommand="gvEmployees_RowCommand"
                      EmptyDataText="No employees found."
                      GridLines="None">
            <Columns>
                <asp:TemplateField HeaderText="Employee">
                    <ItemTemplate>
                        <div class="employee-info">
                            <h4><%# Eval("FirstName") %> <%# Eval("LastName") %></h4>
                            <p class="employee-number">ID: <%# Eval("EmployeeNumber") %></p>
                            <p class="employee-email"><%# Eval("Email") %></p>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Position & Department">
                    <ItemTemplate>
                        <div class="position-info">
                            <h5><%# Eval("Position") ?? Eval("JobTitle") %></h5>
                            <p class="department"><%# Eval("DepartmentName") %></p>
                            <span class="employee-type"><%# Eval("EmployeeType") %></span>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Status & Hire Date">
                    <ItemTemplate>
                        <div class="status-info">
                            <span class='badge <%# GetStatusClass(Eval("Status")?.ToString()) %>'>
                                <%# Eval("Status") %>
                            </span>
                            <p class="hire-date">Hired: <%# Eval("HireDate", "{0:MMM dd, yyyy}") %></p>
                            <p class="tenure"><%# GetTenure(Eval("HireDate")) %></p>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <div class="action-buttons">
                            <asp:Button ID="btnView" runat="server" 
                                        CommandName="ViewEmployee" 
                                        CommandArgument='<%# Eval("Id") %>' 
                                        Text="View" 
                                        CssClass="btn btn-primary btn-sm" />
                            
                            <asp:Button ID="btnEdit" runat="server" 
                                        CommandName="EditEmployee" 
                                        CommandArgument='<%# Eval("Id") %>' 
                                        Text="Edit" 
                                        CssClass="btn btn-secondary btn-sm" />
                            
                            <asp:Button ID="btnTasks" runat="server" 
                                        CommandName="ViewOnboarding" 
                                        CommandArgument='<%# Eval("Id") %>' 
                                        Text="Tasks"
                                        CssClass="btn btn-outline btn-sm"
                                        Visible='<%# !string.IsNullOrEmpty(Eval("OnboardingStatus")?.ToString()) %>' />
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            
            <EmptyDataTemplate>
                <div class="empty-state">
                    <h3>🔍 No Employees Found</h3>
                    <p>Try adjusting your search criteria or add a new employee to get started.</p>
                    <asp:Button ID="btnAddFirstEmployee" runat="server" Text="Add First Employee" 
                                CssClass="btn btn-primary" OnClick="btnAddEmployee_Click" />
                </div>
            </EmptyDataTemplate>
        </asp:GridView>
    </div>

    <!-- Success/Error Messages -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Hidden field for selected employee -->
    <asp:HiddenField ID="hdnSelectedEmployeeId" runat="server" />
    
</asp:Content>