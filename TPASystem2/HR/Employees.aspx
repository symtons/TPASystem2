<%@ Page Title="Employee Management" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="Employees.aspx.cs" Inherits="TPASystem2.HR.Employees" %>

<asp:Content ID="EmployeeContent" ContentPlaceHolderID="DashboardContent" runat="server">
    <style>
        /* Essential inline styles for immediate functionality */
        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 2rem;
            padding: 1.5rem;
            background: linear-gradient(135deg, #1976d2 0%, #1565c0 100%);
            border-radius: 8px;
            color: white;
        }
        
        .filter-section {
            margin-bottom: 2rem;
            background: white;
            padding: 1rem;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        
        .filter-row {
            display: grid;
            grid-template-columns: 2fr 1fr 1fr 1fr auto;
            gap: 1rem;
            align-items: end;
        }
        
        .filter-group {
            display: flex;
            flex-direction: column;
        }
        
        .filter-group label {
            font-weight: 600;
            margin-bottom: 0.5rem;
            color: #333;
        }
        
        .stats-row {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 1rem;
            margin-bottom: 2rem;
        }
        
        .stat-card {
            background: white;
            border-radius: 8px;
            padding: 1.5rem;
            display: flex;
            align-items: center;
            gap: 1rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        
        .stat-icon {
            width: 50px;
            height: 50px;
            border-radius: 8px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 1.5rem;
        }
        
        .stat-icon.blue { background: #2196f3; color: white; }
        .stat-icon.green { background: #4caf50; color: white; }
        .stat-icon.orange { background: #ff9800; color: white; }
        .stat-icon.purple { background: #9c27b0; color: white; }
        
        .stat-content h3 {
            margin: 0;
            font-size: 2rem;
            font-weight: bold;
            color: #333;
        }
        
        .stat-content p {
            margin: 0;
            color: #666;
        }
        
        .table-container {
            background: white;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        
        .table-header {
            padding: 1rem;
            background: #f5f5f5;
            border-bottom: 1px solid #ddd;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        
        .data-table {
            width: 100%;
            border-collapse: collapse;
        }
        
        .data-table th {
            background: #f8f9fa;
            padding: 1rem;
            text-align: left;
            font-weight: 600;
            border-bottom: 2px solid #dee2e6;
        }
        
        .data-table td {
            padding: 1rem;
            border-bottom: 1px solid #dee2e6;
            vertical-align: top;
        }
        
        .data-table tr:hover {
            background: #f8f9fa;
        }
        
        .btn {
            padding: 0.5rem 1rem;
            border: none;
            border-radius: 4px;
            font-size: 0.9rem;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            margin: 0.2rem;
        }
        
        .btn-primary {
            background: #007bff;
            color: white;
        }
        
        .btn-secondary {
            background: #6c757d;
            color: white;
        }
        
        .btn-outline {
            border: 1px solid #6c757d;
            background: white;
            color: #6c757d;
        }
        
        .btn:hover {
            opacity: 0.8;
        }
        
        .badge {
            padding: 0.25rem 0.5rem;
            border-radius: 0.25rem;
            font-size: 0.75rem;
            font-weight: 600;
        }
        
        .badge-success { background: #d4edda; color: #155724; }
        .badge-danger { background: #f8d7da; color: #721c24; }
        .badge-warning { background: #fff3cd; color: #856404; }
        .badge-secondary { background: #e2e3e5; color: #383d41; }
        
        .form-control {
            padding: 0.5rem;
            border: 1px solid #ced4da;
            border-radius: 0.25rem;
            font-size: 1rem;
            width: 100%;
        }
        
        .alert {
            padding: 1rem;
            margin: 1rem 0;
            border-radius: 0.25rem;
        }
        
        .alert-success {
            background: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
        
        .alert-error {
            background: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }
        
        .empty-state {
            text-align: center;
            padding: 3rem;
            color: #6c757d;
        }
        
        .employee-info h4 {
            margin: 0 0 0.5rem 0;
            color: #333;
        }
        
        .employee-number,
        .employee-email {
            margin: 0.25rem 0;
            font-size: 0.9rem;
            color: #666;
        }
        
        .position-info h5 {
            margin: 0 0 0.5rem 0;
            color: #333;
        }
        
        .department {
            margin: 0.25rem 0;
            font-size: 0.9rem;
            color: #666;
        }
        
        .employee-type {
            background: #e3f2fd;
            color: #1976d2;
            padding: 0.25rem 0.5rem;
            border-radius: 0.25rem;
            font-size: 0.8rem;
        }
        
        .action-buttons {
            display: flex;
            gap: 0.5rem;
            flex-wrap: wrap;
        }
        
        @media (max-width: 768px) {
            .filter-row {
                grid-template-columns: 1fr;
                gap: 1rem;
            }
            
            .stats-row {
                grid-template-columns: 1fr;
            }
            
            .page-header {
                flex-direction: column;
                gap: 1rem;
                text-align: center;
            }
        }
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
        <div class="filter-row">
            <div class="filter-group">
                <label>Search Employees</label>
                <asp:TextBox ID="txtSearch" runat="server" placeholder="Search by name, email, or employee number" 
                             CssClass="form-control" AutoPostBack="true" 
                             OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
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

    <!-- Employee Statistics -->
    <div class="stats-row">
        <div class="stat-card">
            <div class="stat-icon blue">
                👥
            </div>
            <div class="stat-content">
                <h3><asp:Literal ID="litTotalEmployees" runat="server">0</asp:Literal></h3>
                <p>Total Employees</p>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon green">
                ✅
            </div>
            <div class="stat-content">
                <h3><asp:Literal ID="litActiveEmployees" runat="server">0</asp:Literal></h3>
                <p>Active Employees</p>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon orange">
                👤
            </div>
            <div class="stat-content">
                <h3><asp:Literal ID="litNewHires" runat="server">0</asp:Literal></h3>
                <p>New Hires (30 days)</p>
            </div>
        </div>
        <div class="stat-card">
            <div class="stat-icon purple">
                🏢
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

                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge badge-<%# GetStatusClass(Eval("Status").ToString()) %>'>
                            <%# Eval("Status") %>
                        </span>
                        <asp:Panel ID="pnlOnboarding" runat="server" 
                                   Visible='<%# !string.IsNullOrEmpty(Eval("OnboardingStatus")?.ToString()) && Eval("OnboardingStatus").ToString() != "COMPLETED" %>'>
                            <br /><small>📋 <%# Eval("OnboardingStatus") %></small>
                        </asp:Panel>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Hire Date">
                    <ItemTemplate>
                        <div class="date-info">
                            <%# Eval("HireDate", "{0:MMM dd, yyyy}") %>
                            <br /><small><%# GetTenure(Eval("HireDate")) %></small>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <div class="action-buttons">
                            <asp:LinkButton ID="btnView" runat="server" CommandName="ViewEmployee" 
                                          CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-outline"
                                          Text="View" />
                            <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditEmployee" 
                                          CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-primary"
                                          Text="Edit" />
                            <asp:LinkButton ID="btnOnboarding" runat="server" CommandName="ViewOnboarding" 
                                          CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-secondary"
                                          Text="Tasks"
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