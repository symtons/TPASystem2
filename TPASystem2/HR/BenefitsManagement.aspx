<%@ Page Title="Benefits Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BenefitsManagement.aspx.cs" Inherits="TPASystem2.HR.BenefitsManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    <link href="~/Content/css/tpa-common.css" rel="stylesheet">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <div class="benefits-container">
        <!-- Page Header -->
        <div class="page-header">
            <div class="header-content">
                <h1><i class="material-icons">favorite</i>Benefits Management</h1>
                <p>Manage employee benefits, plans, and enrollments for full-time employees</p>
            </div>
            <div class="header-actions">
                <asp:Button ID="btnEnrollEmployee" runat="server" Text="Enroll Employee" 
                    CssClass="btn btn-primary" OnClick="btnEnrollEmployee_Click" />
                <asp:Button ID="btnAddPlan" runat="server" Text="Add Plan" 
                    CssClass="btn btn-secondary" OnClick="btnAddPlan_Click" />
            </div>
        </div>

        <!-- Alert Messages -->
        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </asp:Panel>

        <!-- Benefits Overview Stats -->
        <div class="stats-grid">
            <div class="stat-card">
                <div class="stat-icon health">
                    <i class="material-icons">local_hospital</i>
                </div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblHealthEnrollments" runat="server">0</asp:Label></h3>
                    <p>Health Plan Enrollments</p>
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-icon dental">
                    <i class="material-icons">sentiment_satisfied</i>
                </div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblDentalEnrollments" runat="server">0</asp:Label></h3>
                    <p>Dental Plan Enrollments</p>
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-icon vision">
                    <i class="material-icons">visibility</i>
                </div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblVisionEnrollments" runat="server">0</asp:Label></h3>
                    <p>Vision Plan Enrollments</p>
                </div>
            </div>
            <div class="stat-card">
                <div class="stat-icon eligible">
                    <i class="material-icons">people</i>
                </div>
                <div class="stat-content">
                    <h3><asp:Label ID="lblEligibleEmployees" runat="server">0</asp:Label></h3>
                    <p>Eligible Full-Time Employees</p>
                </div>
            </div>
        </div>

        <!-- Filter and Search -->
        <div class="filter-section">
            <div class="filter-row">
                <div class="filter-group">
                    <label for="ddlPlanType">Plan Type:</label>
                    <asp:DropDownList ID="ddlPlanType" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlPlanType_SelectedIndexChanged">
                        <asp:ListItem Value="" Text="All Plans"></asp:ListItem>
                        <asp:ListItem Value="HEALTH" Text="Health"></asp:ListItem>
                        <asp:ListItem Value="DENTAL" Text="Dental"></asp:ListItem>
                        <asp:ListItem Value="VISION" Text="Vision"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label for="ddlPlanCategory">Category:</label>
                    <asp:DropDownList ID="ddlPlanCategory" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlPlanCategory_SelectedIndexChanged">
                        <asp:ListItem Value="" Text="All Categories"></asp:ListItem>
                        <asp:ListItem Value="BASIC" Text="Basic"></asp:ListItem>
                        <asp:ListItem Value="PREMIUM" Text="Premium"></asp:ListItem>
                        <asp:ListItem Value="FAMILY" Text="Family"></asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="filter-group">
                    <label for="txtSearchEmployee">Search Employee:</label>
                    <asp:TextBox ID="txtSearchEmployee" runat="server" CssClass="form-control" placeholder="Enter employee name..."></asp:TextBox>
                </div>
                <div class="filter-group">
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                    <asp:Button ID="btnClearFilter" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClearFilter_Click" />
                </div>
            </div>
        </div>

        <!-- Benefits Plans Tab Navigation -->
        <div class="tab-navigation">
            <asp:Button ID="btnTabPlans" runat="server" Text="Benefits Plans" CssClass="tab-button active" OnClick="btnTabPlans_Click" />
            <asp:Button ID="btnTabEnrollments" runat="server" Text="Employee Enrollments" CssClass="tab-button" OnClick="btnTabEnrollments_Click" />
            <asp:Button ID="btnTabReports" runat="server" Text="Reports" CssClass="tab-button" OnClick="btnTabReports_Click" />
        </div>

        <!-- Benefits Plans Tab -->
        <asp:Panel ID="pnlPlansTab" runat="server" CssClass="tab-content active">
            <div class="table-container">
                <div class="table-header">
                    <div class="table-title">
                        <h3>Available Benefits Plans</h3>
                        <span>Manage and configure benefits plans for full-time employees</span>
                    </div>
                </div>
                <asp:GridView ID="gvBenefitsPlans" runat="server" CssClass="data-table" AutoGenerateColumns="False" 
                    EmptyDataText="No benefits plans found" OnRowCommand="gvBenefitsPlans_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="PlanName" HeaderText="Plan Name" />
                        <asp:BoundField DataField="PlanType" HeaderText="Type" />
                        <asp:BoundField DataField="PlanCategory" HeaderText="Category" />
                        <asp:BoundField DataField="MonthlyEmployeeCost" HeaderText="Employee Cost" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="MonthlyEmployerCost" HeaderText="Employer Cost" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="AnnualDeductible" HeaderText="Deductible" DataFormatString="{0:C}" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='<%# Convert.ToBoolean(Eval("IsActive")) ? "badge badge-success" : "badge badge-danger" %>'>
                                    <%# Convert.ToBoolean(Eval("IsActive")) ? "Active" : "Inactive" %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <div class="action-buttons">
                                    <asp:LinkButton ID="btnEdit" runat="server" CommandName="EditPlan" CommandArgument='<%# Eval("Id") %>' 
                                        CssClass="btn btn-sm btn-outline-primary" Text="Edit" />
                                    <asp:LinkButton ID="btnViewDetails" runat="server" CommandName="ViewDetails" CommandArgument='<%# Eval("Id") %>' 
                                        CssClass="btn btn-sm btn-outline-secondary" Text="Details" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <!-- Employee Enrollments Tab -->
        <asp:Panel ID="pnlEnrollmentsTab" runat="server" CssClass="tab-content">
            <div class="table-container">
                <div class="table-header">
                    <div class="table-title">
                        <h3>Employee Benefits Enrollments</h3>
                        <span>View and manage benefits enrollments for full-time employees</span>
                    </div>
                </div>
                <asp:GridView ID="gvEmployeeEnrollments" runat="server" CssClass="data-table" AutoGenerateColumns="False" 
                    EmptyDataText="No enrollments found" OnRowCommand="gvEmployeeEnrollments_RowCommand">
                    <Columns>
                        <asp:TemplateField HeaderText="Employee">
                            <ItemTemplate>
                                <div class="employee-info">
                                    <div class="employee-avatar">
                                        <div class="avatar-fallback">
                                            <%# GetEmployeeInitials(Eval("EmployeeName").ToString()) %>
                                        </div>
                                    </div>
                                    <div class="employee-details">
                                        <h4><%# Eval("EmployeeName") %></h4>
                                        <p class="employee-number"><%# Eval("EmployeeNumber") %></p>
                                        <p class="employee-type">Full-time</p>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="PlanName" HeaderText="Plan Name" />
                        <asp:BoundField DataField="PlanType" HeaderText="Type" />
                        <asp:BoundField DataField="EffectiveDate" HeaderText="Effective Date" DataFormatString="{0:MM/dd/yyyy}" />
                        <asp:BoundField DataField="MonthlyPremium" HeaderText="Monthly Premium" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="DependentsCount" HeaderText="Dependents" />
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <span class='<%# GetStatusCssClass(Eval("Status").ToString()) %>'>
                                    <%# Eval("Status") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Actions">
                            <ItemTemplate>
                                <div class="action-buttons">
                                    <asp:LinkButton ID="btnEditEnrollment" runat="server" CommandName="EditEnrollment" 
                                        CommandArgument='<%# Eval("EnrollmentId") %>' 
                                        CssClass="btn btn-sm btn-outline-primary" Text="Edit" />
                                    <asp:LinkButton ID="btnCancelEnrollment" runat="server" CommandName="CancelEnrollment" 
                                        CommandArgument='<%# Eval("EnrollmentId") %>' 
                                        CssClass="btn btn-sm btn-outline-danger" Text="Cancel" 
                                        OnClientClick="return confirm('Are you sure you want to cancel this enrollment?');" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <!-- Reports Tab -->
        <asp:Panel ID="pnlReportsTab" runat="server" CssClass="tab-content">
            <div class="reports-grid">
                <div class="report-card">
                    <div class="report-header">
                        <h3>Benefits Summary Report</h3>
                        <p>Overview of all benefits enrollments and costs for full-time employees</p>
                    </div>
                    <div class="report-actions">
                        <asp:Button ID="btnGenerateSummaryReport" runat="server" Text="Generate Report" 
                            CssClass="btn btn-primary" OnClick="btnGenerateSummaryReport_Click" />
                    </div>
                </div>
                <div class="report-card">
                    <div class="report-header">
                        <h3>Employee Eligibility Report</h3>
                        <p>View eligible full-time employees not enrolled in benefits</p>
                    </div>
                    <div class="report-actions">
                        <asp:Button ID="btnGenerateEligibilityReport" runat="server" Text="Generate Report" 
                            CssClass="btn btn-primary" OnClick="btnGenerateEligibilityReport_Click" />
                    </div>
                </div>
                <div class="report-card">
                    <div class="report-header">
                        <h3>Cost Analysis Report</h3>
                        <p>Analyze benefits costs and trends for the organization</p>
                    </div>
                    <div class="report-actions">
                        <asp:Button ID="btnGenerateCostReport" runat="server" Text="Generate Report" 
                            CssClass="btn btn-primary" OnClick="btnGenerateCostReport_Click" />
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>

    <!-- Enrollment Modal -->
    <div id="enrollmentModal" class="modal" style="display: none;">
        <div class="modal-content">
            <div class="modal-header">
                <h3>Enroll Full-Time Employee in Benefits</h3>
                <span class="close" onclick="closeModal()">&times;</span>
            </div>
            <div class="modal-body">
                <asp:UpdatePanel ID="upEnrollment" runat="server">
                    <ContentTemplate>
                        <div class="form-group">
                            <label for="ddlEmployeeSelect">Select Full-Time Employee:</label>
                            <asp:DropDownList ID="ddlEmployeeSelect" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="ddlBenefitsPlan">Select Benefits Plan:</label>
                            <asp:DropDownList ID="ddlBenefitsPlan" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label for="txtEffectiveDate">Effective Date:</label>
                            <asp:TextBox ID="txtEffectiveDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtDependentsCount">Number of Dependents:</label>
                            <asp:TextBox ID="txtDependentsCount" runat="server" CssClass="form-control" TextMode="Number" Text="0"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label for="txtEnrollmentNotes">Notes:</label>
                            <asp:TextBox ID="txtEnrollmentNotes" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnSaveEnrollment" runat="server" Text="Enroll Employee" 
                    CssClass="btn btn-primary" OnClick="btnSaveEnrollment_Click" />
                <button type="button" class="btn btn-secondary" onclick="closeModal()">Cancel</button>
            </div>
        </div>
    </div>

    <script>
        function showEnrollmentModal() {
            document.getElementById('enrollmentModal').style.display = 'block';
        }

        function closeModal() {
            document.getElementById('enrollmentModal').style.display = 'none';
        }

        // Close modal when clicking outside of it
        window.onclick = function(event) {
            var modal = document.getElementById('enrollmentModal');
            if (event.target == modal) {
                modal.style.display = "none";
            }
        }
    </script>
</asp:Content>