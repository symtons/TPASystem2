<%@ Page Title="View Documents" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="ViewDocuments.aspx.cs" Inherits="TPASystem2.Documents.ViewDocuments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">


    <style>
        /* ===============================================
   DOCUMENT VIEWER STYLES - Add to tpa-common.css
   =============================================== */

/* Filter Section */
.filter-section {
    background: white;
    padding: 1.5rem;
    border-radius: 0.5rem;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    margin-bottom: 1.5rem;
}

.filter-row {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 1rem;
    align-items: end;
}

.filter-group {
    display: flex;
    flex-direction: column;
    gap: 0.5rem;
}

.filter-label {
    font-weight: 600;
    color: var(--text-primary);
    font-size: 0.9rem;
}

.search-box {
    display: flex;
    gap: 0.5rem;
}

.search-box .form-input {
    flex: 1;
}

/* Document Grid Enhancements */
.data-table .btn-link {
    color: var(--tpa-primary);
    text-decoration: none;
    display: inline-flex;
    align-items: center;
    gap: 0.25rem;
    padding: 0.25rem 0.5rem;
    border-radius: 0.25rem;
    font-size: 0.85rem;
    margin-right: 0.5rem;
    transition: all 0.2s ease;
}

.data-table .btn-link:hover {
    background: var(--tpa-primary-light);
    color: var(--tpa-primary-dark);
    transform: translateY(-1px);
}

.data-table .btn-link .material-icons {
    font-size: 1rem;
}

/* Empty State */
.empty-state {
    text-align: center;
    padding: 3rem 1rem;
    background: white;
    border-radius: 0.5rem;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.empty-icon {
    font-size: 4rem;
    color: var(--text-muted);
    margin-bottom: 1rem;
}

.empty-icon .material-icons {
    font-size: 4rem;
}

.empty-state h3 {
    color: var(--text-primary);
    margin-bottom: 0.5rem;
    font-size: 1.5rem;
}

.empty-state p {
    color: var(--text-muted);
    margin-bottom: 1.5rem;
    font-size: 1rem;
}

/* Document Modal Styles */
.modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
    padding: 1rem;
}

.modal-content {
    background: white;
    border-radius: 0.75rem;
    box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
    max-width: 90%;
    max-height: 90%;
    display: flex;
    flex-direction: column;
}

.document-modal {
    width: 1000px;
    height: 80vh;
}

.modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 1.5rem;
    border-bottom: 1px solid var(--border-light);
    background: var(--bg-light);
    border-radius: 0.75rem 0.75rem 0 0;
}

.modal-title {
    margin: 0;
    font-size: 1.25rem;
    font-weight: 600;
    color: var(--text-primary);
}

.modal-close {
    background: none;
    border: none;
    color: var(--text-muted);
    cursor: pointer;
    padding: 0.5rem;
    border-radius: 0.25rem;
    display: flex;
    align-items: center;
    justify-content: center;
    transition: all 0.2s ease;
    text-decoration: none;
}

.modal-close:hover {
    background: var(--bg-hover);
    color: var(--text-primary);
}

.modal-close .material-icons {
    font-size: 1.5rem;
}

.modal-body {
    flex: 1;
    padding: 1.5rem;
    overflow-y: auto;
    display: flex;
    flex-direction: column;
    gap: 1rem;
}

/* Document Info Section */
.document-info {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 1rem;
    padding: 1rem;
    background: var(--bg-light);
    border-radius: 0.5rem;
    margin-bottom: 1rem;
}

.info-row {
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.info-label {
    font-weight: 600;
    color: var(--text-secondary);
    font-size: 0.9rem;
}

.info-row > span:not(.info-label) {
    color: var(--text-primary);
    font-weight: 500;
}

/* Document Preview */
.document-preview {
    flex: 1;
    display: flex;
    flex-direction: column;
    min-height: 500px;
}

.document-image-preview {
    max-width: 100%;
    max-height: 600px;
    object-fit: contain;
    border-radius: 0.5rem;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    margin: 0 auto;
}

.unsupported-preview {
    flex: 1;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    text-align: center;
    padding: 2rem;
    background: var(--bg-light);
    border-radius: 0.5rem;
    border: 2px dashed var(--border-light);
}

.unsupported-icon {
    font-size: 3rem;
    color: var(--text-muted);
    margin-bottom: 1rem;
}

.unsupported-icon .material-icons {
    font-size: 3rem;
}

.unsupported-preview h4 {
    color: var(--text-primary);
    margin-bottom: 0.5rem;
    font-size: 1.25rem;
}

.unsupported-preview p {
    color: var(--text-muted);
    margin-bottom: 1.5rem;
    font-size: 1rem;
}

/* File Type Icons */
.file-type-icon {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    width: 2rem;
    height: 2rem;
    border-radius: 0.25rem;
    font-weight: 600;
    font-size: 0.75rem;
    text-transform: uppercase;
    margin-right: 0.5rem;
}

.file-type-pdf {
    background: #dc3545;
    color: white;
}

.file-type-doc,
.file-type-docx {
    background: #2b579a;
    color: white;
}

.file-type-xls,
.file-type-xlsx {
    background: #107c41;
    color: white;
}

.file-type-jpg,
.file-type-jpeg,
.file-type-png,
.file-type-gif {
    background: #6f42c1;
    color: white;
}

.file-type-txt {
    background: #6c757d;
    color: white;
}

.file-type-default {
    background: var(--text-muted);
    color: white;
}

/* Responsive Design */
@media (max-width: 768px) {
    .filter-row {
        grid-template-columns: 1fr;
    }
    
    .document-modal {
        width: 95%;
        height: 90vh;
        margin: 0 auto;
    }
    
    .modal-header {
        padding: 1rem;
    }
    
    .modal-body {
        padding: 1rem;
    }
    
    .document-info {
        grid-template-columns: 1fr;
    }
    
    .data-table .btn-link {
        display: block;
        margin-bottom: 0.25rem;
    }
}

/* Animation for modal */
.modal-overlay {
    animation: fadeIn 0.3s ease-out;
}

.modal-content {
    animation: slideIn 0.3s ease-out;
}

@keyframes fadeIn {
    from {
        opacity: 0;
    }
    to {
        opacity: 1;
    }
}

@keyframes slideIn {
    from {
        transform: scale(0.9) translateY(-20px);
        opacity: 0;
    }
    to {
        transform: scale(1) translateY(0);
        opacity: 1;
    }
}

/* Alert Panel Enhancements for Document Pages */
.alert-panel {
    margin-bottom: 1.5rem;
    padding: 1rem;
    border-radius: 0.5rem;
    display: flex;
    align-items: center;
    gap: 0.75rem;
    font-weight: 500;
}

.alert-success {
    background: #d1e7dd;
    color: #0f5132;
    border: 1px solid #badbcc;
}

.alert-success::before {
    content: "✓";
    font-weight: bold;
    font-size: 1.2rem;
}

.alert-error {
    background: #f8d7da;
    color: #842029;
    border: 1px solid #f5c2c7;
}

.alert-error::before {
    content: "⚠";
    font-weight: bold;
    font-size: 1.2rem;
}

.alert-warning {
    background: #fff3cd;
    color: #664d03;
    border: 1px solid #ffecb5;
}

.alert-warning::before {
    content: "⚠";
    font-weight: bold;
    font-size: 1.2rem;
}

.alert-info {
    background: #d1ecf1;
    color: #055160;
    border: 1px solid #bee5eb;
}

.alert-info::before {
    content: "ℹ";
    font-weight: bold;
    font-size: 1.2rem;
}

/* Grid enhancements for better document display */
.data-table tbody tr:hover {
    background: var(--bg-hover);
    transform: translateY(-1px);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.data-table tbody td {
    vertical-align: middle;
    padding: 1rem 0.75rem;
}

/* Document count styling */
.grid-subtitle {
    font-style: italic;
    color: var(--text-muted);
    margin-bottom: 0;
}
    </style>
    <!-- Welcome Header - Matching EmployeeLeavePortal Style -->
    <div class="onboarding-header">
        <div class="welcome-content">
            <div class="welcome-text">
                <h1 class="welcome-title">
                    <i class="material-icons">folder_open</i>
                    Document Library
                </h1>
                <p class="welcome-subtitle">View and access available documents</p>
                
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
                        <i class="material-icons">today</i>
                        <span>
                            <asp:Literal ID="litCurrentDate" runat="server"></asp:Literal>
                        </span>
                    </div>
                </div>
            </div>
            <div class="welcome-actions">
                <asp:Button ID="btnUploadDocument" runat="server" Text="Upload Document" CssClass="btn-primary" 
                           OnClick="btnUploadDocument_Click" />
            </div>
        </div>
    </div>

    <!-- Error/Success Messages -->
    <asp:Panel ID="pnlMessages" runat="server" Visible="false" CssClass="alert-panel">
        <asp:Label ID="lblMessage" runat="server" CssClass="message-text"></asp:Label>
    </asp:Panel>

    <!-- Filter Section -->
    <div class="grid-container">
        <div class="grid-header">
            <h2 class="grid-title">
                <i class="material-icons">filter_list</i>
                Document Filters
            </h2>
        </div>

        <div class="filter-section">
            <div class="filter-row">
                <div class="filter-group">
                    <label class="filter-label">Category</label>
                    <asp:DropDownList ID="ddlCategoryFilter" runat="server" CssClass="form-input" 
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlCategoryFilter_SelectedIndexChanged">
                        <asp:ListItem Value="">All Categories</asp:ListItem>
                        <asp:ListItem Value="HR_DOCUMENTS">HR Documents</asp:ListItem>
                        <asp:ListItem Value="POLICIES">Policies</asp:ListItem>
                        <asp:ListItem Value="TRAINING">Training Materials</asp:ListItem>
                        <asp:ListItem Value="FORMS">Forms</asp:ListItem>
                        <asp:ListItem Value="ANNOUNCEMENTS">Announcements</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="filter-group">
                    <label class="filter-label">Section</label>
                    <asp:DropDownList ID="ddlSectionFilter" runat="server" CssClass="form-input" 
                                    AutoPostBack="true" OnSelectedIndexChanged="ddlSectionFilter_SelectedIndexChanged">
                        <asp:ListItem Value="">All Sections</asp:ListItem>
                        <asp:ListItem Value="GENERAL">General</asp:ListItem>
                        <asp:ListItem Value="HR_SECTION">HR Section</asp:ListItem>
                        <asp:ListItem Value="MANAGEMENT">Management</asp:ListItem>
                        <asp:ListItem Value="PUBLIC_DOCS">Public Documents</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="filter-group">
                    <label class="filter-label">Search</label>
                    <div class="search-box">
                        <asp:TextBox ID="txtSearch" runat="server" CssClass="form-input" 
                                   placeholder="Search documents..." />
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn-secondary" 
                                  OnClick="btnSearch_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Documents Grid Section -->
    <div class="grid-container">
        <div class="grid-header">
            <h2 class="grid-title">
                <i class="material-icons">description</i>
                Available Documents
            </h2>
            <p class="grid-subtitle">
                <asp:Literal ID="litDocumentCount" runat="server" Text="0 documents found"></asp:Literal>
            </p>
        </div>

        <!-- Documents Grid -->
        <asp:GridView ID="gvDocuments" runat="server" CssClass="data-table" AutoGenerateColumns="false" 
                     EmptyDataText="No documents found matching your criteria." 
                     OnRowCommand="gvDocuments_RowCommand" DataKeyNames="Id">
            <Columns>
                <asp:BoundField DataField="DocumentName" HeaderText="Document Name" SortExpression="DocumentName" />
                <asp:BoundField DataField="Category" HeaderText="Category" SortExpression="Category" />
                <asp:BoundField DataField="Section" HeaderText="Section" SortExpression="Section" />
                <asp:BoundField DataField="FileExtension" HeaderText="Type" SortExpression="FileExtension" />
                <asp:TemplateField HeaderText="Size">
                    <ItemTemplate>
                        <asp:Literal ID="litFileSize" runat="server" 
                                   Text='<%# FormatFileSize(Convert.ToInt64(Eval("FileSize"))) %>'></asp:Literal>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="CreatedDate" HeaderText="Upload Date" DataFormatString="{0:MM/dd/yyyy}" SortExpression="CreatedDate" />
                <asp:TemplateField HeaderText="Actions" ItemStyle-Width="200px">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnView" runat="server" CssClass="btn-link" 
                                      CommandName="ViewDocument" CommandArgument='<%# Eval("Id") %>' 
                                      ToolTip="View Document">
                            <i class="material-icons">visibility</i> View
                        </asp:LinkButton>
                        
                        <asp:LinkButton ID="btnDownload" runat="server" CssClass="btn-link" 
                                      CommandName="DownloadDocument" CommandArgument='<%# Eval("Id") %>' 
                                      ToolTip="Download Document">
                            <i class="material-icons">download</i> Download
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <!-- No Documents Message -->
        <asp:Panel ID="pnlNoDocuments" runat="server" Visible="false" CssClass="empty-state">
            <div class="empty-icon">
                <i class="material-icons">folder_open</i>
            </div>
            <h3>No Documents Found</h3>
            <p>There are no documents available with your current filters.</p>
            <asp:Button ID="btnClearFilters" runat="server" Text="Clear Filters" CssClass="btn-secondary" 
                       OnClick="btnClearFilters_Click" />
        </asp:Panel>
    </div>

    <!-- Document Preview Modal (Hidden by default) -->
    <asp:Panel ID="pnlDocumentModal" runat="server" CssClass="modal-overlay" Style="display: none;">
        <div class="modal-content document-modal">
            <div class="modal-header">
                <h3 class="modal-title">
                    <asp:Literal ID="litModalDocumentName" runat="server" Text="Document Preview"></asp:Literal>
                </h3>
                <asp:LinkButton ID="btnCloseModal" runat="server" CssClass="modal-close" OnClick="btnCloseModal_Click">
                    <i class="material-icons">close</i>
                </asp:LinkButton>
            </div>
            <div class="modal-body">
                <div class="document-info">
                    <div class="info-row">
                        <span class="info-label">Category:</span>
                        <asp:Literal ID="litModalCategory" runat="server"></asp:Literal>
                    </div>
                    <div class="info-row">
                        <span class="info-label">Section:</span>
                        <asp:Literal ID="litModalSection" runat="server"></asp:Literal>
                    </div>
                    <div class="info-row">
                        <span class="info-label">File Size:</span>
                        <asp:Literal ID="litModalFileSize" runat="server"></asp:Literal>
                    </div>
                    <div class="info-row">
                        <span class="info-label">Upload Date:</span>
                        <asp:Literal ID="litModalUploadDate" runat="server"></asp:Literal>
                    </div>
                </div>
                
                <!-- Document Preview Frame -->
                <div class="document-preview">
                    <asp:Panel ID="pnlPDFPreview" runat="server" Visible="false">
                        <iframe id="documentFrame" runat="server" style="width: 100%; height: 600px; border: none;"></iframe>
                    </asp:Panel>
                    
                    <asp:Panel ID="pnlImagePreview" runat="server" Visible="false">
                        <asp:Image ID="imgPreview" runat="server" CssClass="document-image-preview" />
                    </asp:Panel>
                    
                    <asp:Panel ID="pnlUnsupportedPreview" runat="server" Visible="false" CssClass="unsupported-preview">
                        <div class="unsupported-icon">
                            <i class="material-icons">description</i>
                        </div>
                        <h4>Preview Not Available</h4>
                        <p>This file type cannot be previewed in the browser. Please download the file to view it.</p>
                        <asp:Button ID="btnModalDownload" runat="server" Text="Download File" CssClass="btn-primary" 
                                  OnClick="btnModalDownload_Click" />
                    </asp:Panel>
                </div>
            </div>
        </div>
    </asp:Panel>

    <script type="text/javascript">
        // Modal handling
        function showDocumentModal() {
            document.getElementById('<%= pnlDocumentModal.ClientID %>').style.display = 'flex';
            document.body.style.overflow = 'hidden';
        }

        function hideDocumentModal() {
            document.getElementById('<%= pnlDocumentModal.ClientID %>').style.display = 'none';
            document.body.style.overflow = 'auto';
        }

        // Handle modal close on background click
        document.addEventListener('DOMContentLoaded', function () {
            const modal = document.getElementById('<%= pnlDocumentModal.ClientID %>');
            if (modal) {
                modal.addEventListener('click', function (e) {
                    if (e.target === modal) {
                        hideDocumentModal();
                    }
                });
            }
        });
    </script>
</asp:Content>