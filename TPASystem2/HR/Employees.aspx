<%@ Page Title="Employee Management" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Employees.aspx.cs" Inherits="TPASystem2.HR.Employees" %>

<%-- Add the employee-management.css to the head section --%>
<asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="~/employee-management.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="EmployeeContent" ContentPlaceHolderID="DashboardContent" runat="server">
    
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

    <!-- Statistics Row -->
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
                <i class="material-icons">pending</i>
            </div>
            <div class="stat-content">
                <h3><asp:Literal ID="litOnboardingEmployees" runat="server">0</asp:Literal></h3>
                <p>In Onboarding</p>
            </div>
        </div>
        
        <div class="stat-card">
            <div class="stat-icon purple">
                <i class="material-icons">calendar_today</i>
            </div>
            <div class="stat-content">
                <h3><asp:Literal ID="litNewHires" runat="server">0</asp:Literal></h3>
                <p>New Hires (30 days)</p>
            </div>
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
                        <asp:ListItem Value="Terminated">Terminated</asp:ListItem>
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
                        <asp:ListItem Value="Temporary">Temporary</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <asp:Button ID="btnClearFilters" runat="server" Text="Clear" 
                                CssClass="btn btn-outline" OnClick="btnClearFilters_Click" />
                </div>
            </div>
        </div>
    </div>

    <!-- Employee Table -->
    <div class="table-container">
        <div class="table-header">
            <h3>Employee List</h3>
            <!-- REMOVED: Bulk Actions button that was causing the error -->
        </div>
        
        <asp:GridView ID="gvEmployees" runat="server" 
                      CssClass="data-table" 
                      AutoGenerateColumns="false"
                      DataKeyNames="Id"
                      OnRowCommand="gvEmployees_RowCommand"
                      OnRowDataBound="gvEmployees_RowDataBound"
                      AllowPaging="true" 
                      PageSize="25"
                      OnPageIndexChanging="gvEmployees_PageIndexChanging"
                      PagerStyle-CssClass="pagination-container">
            
            <Columns>
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelect" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="Employee" SortExpression="LastName">
                    <ItemTemplate>
                        <div class="employee-info">
                            <h4><%# Eval("FirstName") %> <%# Eval("LastName") %></h4>
                            <div class="employee-number">ID: <%# Eval("EmployeeNumber") %></div>
                            <div class="employee-email"><%# Eval("Email") %></div>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="Position" SortExpression="Position">
                    <ItemTemplate>
                        <div class="position-info">
                            <h5><%# Eval("Position") ?? Eval("JobTitle") %></h5>
                            <div class="department"><%# Eval("DepartmentName") %></div>
                            <span class="employee-type"><%# Eval("EmployeeType") %></span>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="Status" SortExpression="Status">
                    <ItemTemplate>
                        <span class='badge <%# GetStatusClass(Eval("Status")?.ToString()) %>'>
                            <%# Eval("Status") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="Hire Date" SortExpression="HireDate">
                    <ItemTemplate>
                        <%# Eval("HireDate", "{0:MMM dd, yyyy}") %>
                        <div style="font-size: 0.8rem; color: #666;">
                            <%# GetTenure(Eval("HireDate")) %>
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