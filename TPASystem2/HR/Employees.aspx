<%@ Page Title="Employee Management" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Employees.aspx.cs" Inherits="TPASystem2.HR.Employees" %>

<asp:Content ID="EmployeeContent" ContentPlaceHolderID="DashboardContent" runat="server">
    
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/employee-management.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    
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
                        <asp:ListItem Value="Temporary">Temporary</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <asp:Button ID="btnClearFilters" runat="server" Text="Clear Filters" 
                                CssClass="btn btn-outline-secondary" OnClick="btnClearFilters_Click" />
                </div>
            </div>
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
                            <div class="employee-avatar">
                                <div class="avatar-fallback">
                                    <%# GetInitials(Eval("FirstName"), Eval("LastName")) %>
                                </div>
                            </div>
                            <div class="employee-details">
                                <h4><%# Eval("FirstName") %> <%# Eval("LastName") %></h4>
                                <p class="employee-number">ID: <%# Eval("EmployeeNumber") %></p>
                                <p class="employee-email"><%# Eval("Email") %></p>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Position & Department">
                    <ItemTemplate>
                        <div class="position-info">
                            <h5><%# Eval("Position") ?? Eval("JobTitle") %></h5>
                            <p class="department"><%# Eval("DepartmentName") %></p>
                            <p class="employee-type"><%# Eval("EmployeeType") %></p>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Status & Tenure">
                    <ItemTemplate>
                        <div class="status-info">
                            <span class="status-badge <%# GetStatusClass(Eval("Status")?.ToString()) %>">
                                <%# Eval("Status") %>
                            </span>
                            <p class="tenure">Tenure: <%# GetTenure(Eval("HireDate")) %></p>
                            <p class="hire-date">Hired: <%# Eval("HireDate", "{0:MM/dd/yyyy}") %></p>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Contact & Location">
                    <ItemTemplate>
                        <div class="contact-info">
                            <p class="phone">📞 <%# Eval("PhoneNumber") ?? "Not provided" %></p>
                            <p class="location">📍 <%# Eval("WorkLocation") ?? "Office" %></p>
                            <p class="salary">💰 <%# FormatSalary(Eval("Salary")) %></p>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <div class="action-buttons">
                            <asp:Button ID="btnView" runat="server" Text="View" 
                                        CssClass="btn btn-sm btn-outline-primary" 
                                        CommandName="ViewEmployee" 
                                        CommandArgument='<%# Eval("Id") %>' />
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" 
                                        CssClass="btn btn-sm btn-outline-secondary" 
                                        CommandName="EditEmployee" 
                                        CommandArgument='<%# Eval("Id") %>' />
                            <asp:Button ID="btnOnboarding" runat="server" Text="Onboarding" 
                                        CssClass="btn btn-sm btn-outline-info" 
                                        CommandName="ViewOnboarding" 
                                        CommandArgument='<%# Eval("Id") %>' 
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
            
            <PagerSettings Mode="NumericFirstLast" PageButtonCount="5" FirstPageText="First" LastPageText="Last" />
            <PagerStyle CssClass="gridview-pager" />
        </asp:GridView>
    </div>

    <!-- Success/Error Messages -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>
    </asp:Panel>

    <!-- Hidden field for selected employee -->
    <asp:HiddenField ID="hdnSelectedEmployeeId" runat="server" />
    
</asp:Content>