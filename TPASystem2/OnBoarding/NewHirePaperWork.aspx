<%@ Page Title="New Hire Paperwork - Employment Application" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="NewHirePaperWork.aspx.cs" Inherits="TPASystem2.OnBoarding.NewHirePaperWorkTabbed" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <!-- CSS Links -->
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
    <link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
   <style>
       /* ===================================
   NEW HIRE PAPERWORK TABBED FORM STYLES
   Matching MyOnboarding Design Pattern
   Add these styles to tpa-common.css
   =================================== */

/* Tab Navigation Styles - Clean Modern Design */
.tab-navigation {
    background: white;
    border-radius: 16px;
    box-shadow: 0 4px 16px rgba(0,0,0,0.1);
    margin-bottom: 2rem;
    overflow: hidden;
}

.tab-buttons {
    display: flex;
    flex-wrap: wrap;
    background: #f8f9fa;
    border-bottom: 1px solid #e5e7eb;
}

.tab-button {
    flex: 1;
    min-width: 140px;
    padding: 1.25rem 1rem;
    background: transparent;
    border: none;
    border-right: 1px solid #e5e7eb;
    color: #64748b;
    font-size: 0.9rem;
    font-weight: 500;
    cursor: pointer;
    transition: all 0.3s ease;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.5rem;
    text-decoration: none;
    font-family: 'Inter', sans-serif;
}

.tab-button:last-child {
    border-right: none;
}

.tab-button:hover {
    background: #e2e8f0;
    color: #334155;
    transform: translateY(-1px);
}

.tab-button.active {
    background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
    color: white;
    position: relative;
    box-shadow: 0 4px 12px rgba(25, 118, 210, 0.3);
}

.tab-button .material-icons {
    font-size: 1.4rem;
}

.tab-button span {
    font-size: 0.85rem;
    text-align: center;
    line-height: 1.2;
    font-weight: 600;
}

/* Responsive tab navigation */
@media (max-width: 768px) {
    .tab-buttons {
        flex-direction: column;
    }
    
    .tab-button {
        flex-direction: row;
        justify-content: flex-start;
        padding: 1rem 1.5rem;
        border-right: none;
        border-bottom: 1px solid #e5e7eb;
        gap: 0.75rem;
    }
    
    .tab-button:last-child {
        border-bottom: none;
    }

    .tab-button span {
        font-size: 1rem;
    }
}

/* Tabbed Form Container */
.tabbed-form-container {
    background: white;
    border-radius: 16px;
    box-shadow: 0 4px 16px rgba(0,0,0,0.1);
    overflow: hidden;
}

/* Tab Content */
.tab-content {
    padding: 2.5rem;
    display: none;
    animation: fadeIn 0.3s ease-in-out;
}

.tab-content.active {
    display: block;
}

@keyframes fadeIn {
    from { opacity: 0; transform: translateY(10px); }
    to { opacity: 1; transform: translateY(0); }
}

/* Tab Headers */
.tab-header {
    margin-bottom: 2rem;
    padding-bottom: 1.5rem;
    border-bottom: 2px solid #f1f5f9;
}

.tab-header h2 {
    color: #1e293b;
    font-size: 1.75rem;
    font-weight: 700;
    margin: 0 0 0.5rem 0;
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.tab-header .material-icons {
    color: #1976d2;
    font-size: 1.75rem;
}

.tab-header p {
    color: #64748b;
    margin: 0;
    font-size: 1rem;
    line-height: 1.6;
}

/* Form Sections - Matching MyOnboarding Style */
.form-section {
    margin-bottom: 2rem;
    padding: 2rem;
    background: white;
    border-radius: 12px;
    border: 1px solid #e5e7eb;
    box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}

.form-section:last-child {
    margin-bottom: 0;
}

/* Form Groups and Labels */
.tabbed-form-container .form-group {
    margin-bottom: 1.5rem;
}

.tabbed-form-container .form-group:last-child {
    margin-bottom: 0;
}

.tabbed-form-container .form-label {
    display: block;
    font-weight: 600;
    color: #374151;
    margin-bottom: 0.75rem;
    font-size: 0.9rem;
}

.tabbed-form-container .form-label.required::after {
    content: ' *';
    color: #ef4444;
    font-weight: bold;
}

/* Enhanced Form Inputs - MyOnboarding Style */
.tabbed-form-container .form-input {
    width: 100%;
    padding: 0.75rem 1rem;
    border: 2px solid #e5e7eb;
    border-radius: 8px;
    font-size: 1rem;
    transition: all 0.2s ease;
    background: white;
    font-family: 'Inter', sans-serif;
}

.tabbed-form-container .form-input:focus {
    outline: none;
    border-color: #1976d2;
    box-shadow: 0 0 0 3px rgba(25, 118, 210, 0.1);
}

.tabbed-form-container .form-input:disabled {
    background-color: #f8f9fa;
    color: #9ca3af;
    cursor: not-allowed;
}

/* Checkbox and Radio Groups */
.checkbox-group-inline,
.radio-group-inline {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
    align-items: center;
    margin-top: 0.75rem;
}

.checkbox-group,
.radio-group {
    margin-top: 0.75rem;
}

.tabbed-form-container .form-checkbox,
.tabbed-form-container .form-radio {
    margin-right: 0.5rem;
    accent-color: #1976d2;
    width: 18px;
    height: 18px;
}

.checkbox-group label,
.radio-group label,
.checkbox-group-inline label,
.radio-group-inline label {
    font-weight: 500;
    color: #374151;
    cursor: pointer;
    display: flex;
    align-items: center;
    margin-bottom: 0;
    font-size: 0.9rem;
}

/* Form Group Conditional */
.form-group-conditional {
    margin-top: 1rem;
    padding-left: 1.5rem;
    border-left: 3px solid #e5e7eb;
}

.form-group-inline {
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

/* Form Layout Utilities */
.form-row {
    display: flex;
    gap: 1.5rem;
    margin-bottom: 1.5rem;
    flex-wrap: wrap;
}

.form-col-narrow {
    flex: 0 0 120px;
}

.form-col-medium {
    flex: 1;
    min-width: 200px;
}

.form-col-wide {
    flex: 2;
    min-width: 250px;
}

.form-col-full {
    flex: 1 1 100%;
}

.form-col-auto {
    flex: 0 0 auto;
}

@media (max-width: 768px) {
    .form-row {
        flex-direction: column;
        gap: 0;
    }
    
    .form-col-narrow,
    .form-col-medium,
    .form-col-wide {
        flex: 1 1 auto;
        min-width: auto;
    }
}

/* Date Range Inputs */
.date-range {
    display: flex;
    align-items: center;
    gap: 1rem;
    flex-wrap: wrap;
}

.date-range span {
    color: #64748b;
    font-weight: 500;
    flex-shrink: 0;
}

.date-range .form-input {
    flex: 1;
    min-width: 140px;
}

@media (max-width: 576px) {
    .date-range {
        flex-direction: column;
        align-items: stretch;
        gap: 0.75rem;
    }
}

/* Tables - Clean Modern Style */
.form-table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 1rem;
    background: white;
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 2px 8px rgba(0,0,0,0.06);
    border: 1px solid #e5e7eb;
}

.form-table th {
    background: #f8fafc;
    padding: 1rem 0.75rem;
    text-align: left;
    font-weight: 600;
    color: #374151;
    border-bottom: 2px solid #e5e7eb;
    font-size: 0.9rem;
}

.form-table td {
    padding: 0.75rem;
    border-bottom: 1px solid #f1f5f9;
    vertical-align: top;
}

.form-table tr:last-child td {
    border-bottom: none;
}

.form-table .form-input {
    margin: 0;
    font-size: 0.9rem;
    padding: 0.5rem 0.75rem;
}

/* Education Table Specific */
.education-table .year-checkboxes {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
    align-items: center;
}

.education-table .year-checkboxes label {
    margin: 0;
    font-size: 0.85rem;
}

/* Employment Grid */
.employment-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1.5rem;
    margin-top: 1rem;
}

.employment-full-row {
    grid-column: 1 / -1;
}

.employment-section-title {
    color: #1976d2;
    font-size: 1.25rem;
    font-weight: 700;
    margin-bottom: 1.5rem;
    padding-bottom: 0.75rem;
    border-bottom: 2px solid #f1f5f9;
}

@media (max-width: 768px) {
    .employment-grid {
        grid-template-columns: 1fr;
    }
}

/* Reference Grid */
.reference-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1.5rem;
    margin-top: 1rem;
}

.reference-section-title {
    color: #059669;
    font-size: 1.25rem;
    font-weight: 700;
    margin-bottom: 1.5rem;
    padding-bottom: 0.75rem;
    border-bottom: 2px solid #f1f5f9;
}

@media (max-width: 768px) {
    .reference-grid {
        grid-template-columns: 1fr;
    }
}

/* Background Check Specific */
.background-check-header,
.authorization-header {
    text-align: center;
    margin-bottom: 2rem;
    padding: 1.5rem;
    background: #f8fafc;
    border-radius: 8px;
    border: 1px solid #e5e7eb;
}

.background-check-header h3,
.authorization-header h3 {
    color: #1e293b;
    font-size: 1.25rem;
    font-weight: 700;
    margin: 0 0 0.5rem 0;
}

.background-check-header h4,
.authorization-header h4 {
    color: #64748b;
    font-size: 1rem;
    font-weight: 500;
    margin: 0;
}

.authorization-text {
    background: white;
    padding: 1.5rem;
    border-radius: 8px;
    border: 1px solid #e5e7eb;
    margin: 1rem 0;
    box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.authorization-text p {
    margin-bottom: 1rem;
    line-height: 1.6;
    color: #374151;
    text-align: justify;
}

.authorization-content {
    background: #f8fafc;
    padding: 1.5rem;
    border-radius: 8px;
    border-left: 4px solid #1976d2;
    margin: 1rem 0;
}

.authorization-content p {
    margin-bottom: 1rem;
    line-height: 1.6;
    color: #374151;
    text-align: justify;
    font-size: 0.95rem;
}

/* Signature Lines */
.signature-line {
    height: 2px;
    background: #374151;
    margin-top: 2rem;
    position: relative;
}

.signature-line::before {
    content: 'Signature';
    position: absolute;
    top: -1.5rem;
    left: 0;
    font-size: 0.8rem;
    color: #64748b;
}

/* Criminal Details Table */
.criminal-details-table .form-table th {
    background: #fef2f2;
    color: #991b1b;
}

/* License Table */
.license-table .form-table th {
    background: #f0fdf4;
    color: #166534;
}

/* Form Navigation - MyOnboarding Style */
.form-navigation {
    background: #f8fafc;
    padding: 2rem;
    border-top: 1px solid #e5e7eb;
    margin-top: 2rem;
}

.nav-buttons {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 1rem;
    max-width: 600px;
    margin: 0 auto;
}

/* Button Styles - Matching MyOnboarding */
.nav-buttons .btn {
    min-width: 140px;
    padding: 0.75rem 1.5rem;
    font-weight: 600;
    border-radius: 8px;
    transition: all 0.2s ease;
    text-decoration: none;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    border: 2px solid transparent;
    font-family: 'Inter', sans-serif;
    font-size: 0.9rem;
}

.btn-primary {
    background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
    color: white;
    border-color: #1976d2;
}

.btn-primary:hover {
    background: linear-gradient(135deg, #1565c0 0%, #1976d2 100%);
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(25, 118, 210, 0.3);
}

.btn-secondary {
    background-color: #64748b;
    color: white;
    border-color: #64748b;
}

.btn-secondary:hover {
    background-color: #475569;
    border-color: #475569;
    transform: translateY(-1px);
}

.btn-success {
    background: linear-gradient(135deg, #059669 0%, #10b981 100%);
    color: white;
    border-color: #059669;
}

.btn-success:hover {
    background: linear-gradient(135deg, #047857 0%, #059669 100%);
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(5, 150, 105, 0.3);
}

.btn-outline {
    background-color: transparent;
    color: #1976d2;
    border-color: #1976d2;
}

.btn-outline:hover {
    background-color: #1976d2;
    color: white;
    transform: translateY(-1px);
}

/* Form Notes */
.form-note {
    font-size: 0.85rem;
    color: #64748b;
    font-style: italic;
    margin-top: 0.75rem;
    padding: 0.75rem;
    background: #fefce8;
    border-left: 3px solid #eab308;
    border-radius: 4px;
}

/* Application Header */
.application-header {
    text-align: center;
    margin-bottom: 2rem;
    padding: 2rem;
    background: white;
    border-radius: 12px;
    border: 1px solid #e5e7eb;
    box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}

.application-title {
    color: #1e293b;
    font-size: 2rem;
    font-weight: 700;
    margin: 0 0 0.5rem 0;
}

.application-subtitle {
    color: #64748b;
    margin: 0;
    line-height: 1.6;
    max-width: 800px;
    margin: 0 auto;
}

/* Message Panels */
.success-message {
    background: #f0fdf4;
    color: #166534;
    padding: 1rem 1.5rem;
    border-radius: 8px;
    border: 1px solid #bbf7d0;
    margin-bottom: 1.5rem;
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.message-panel {
    padding: 1rem 1.5rem;
    border-radius: 8px;
    margin-bottom: 1.5rem;
}

.message-text.error {
    background: #fef2f2;
    color: #991b1b;
    border: 1px solid #fecaca;
}

/* Mandatory Task Header - MyOnboarding Style */
.mandatory-task-header {
    background: linear-gradient(135deg, #dc2626 0%, #ef4444 100%);
    color: white;
    padding: 2rem;
    border-radius: 12px;
    margin-bottom: 2rem;
    box-shadow: 0 4px 16px rgba(220, 38, 38, 0.2);
}

.progress-tracker {
    margin-bottom: 1rem;
}

.progress-step {
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    background: rgba(255, 255, 255, 0.2);
    padding: 0.5rem 1rem;
    border-radius: 50px;
    font-size: 0.9rem;
    font-weight: 500;
}

.task-title {
    font-size: 1.75rem;
    font-weight: 700;
    margin: 0 0 0.5rem 0;
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.mandatory-badge {
    background: rgba(255, 255, 255, 0.3);
    padding: 0.25rem 0.75rem;
    border-radius: 50px;
    font-size: 0.8rem;
    font-weight: 600;
}

.task-subtitle {
    margin: 0;
    opacity: 0.9;
    font-size: 1rem;
}

/* Validation Error Styling */
.field-validation-error {
    color: #ef4444;
    font-size: 0.8rem;
    margin-top: 0.5rem;
    display: block;
    padding: 0.5rem 0.75rem;
    background: #fef2f2;
    border: 1px solid #fecaca;
    border-radius: 4px;
    border-left: 3px solid #ef4444;
}

/* Responsive Design */
@media (max-width: 768px) {
    .tab-content {
        padding: 1.5rem;
    }
    
    .form-section {
        padding: 1.5rem;
    }
    
    .form-navigation {
        padding: 1.5rem;
    }
    
    .nav-buttons {
        flex-direction: column;
        gap: 1rem;
    }
    
    .nav-buttons .btn {
        width: 100%;
        min-width: auto;
    }
    
    .form-table {
        font-size: 0.85rem;
    }
    
    .form-table th,
    .form-table td {
        padding: 0.5rem;
    }
}

@media (max-width: 576px) {
    .mandatory-task-header {
        padding: 1.5rem;
    }
    
    .task-title {
        font-size: 1.5rem;
        flex-direction: column;
        text-align: center;
        gap: 0.5rem;
    }
    
    .application-header {
        padding: 1.5rem;
    }
    
    .application-title {
        font-size: 1.5rem;
    }
    
    .tab-content {
        padding: 1rem;
    }
    
    .form-section {
        padding: 1rem;
    }
}

/* Print Styles */
@media print {
    .tab-navigation,
    .form-navigation {
        display: none;
    }
    
    .tab-content {
        display: block !important;
        page-break-after: always;
        padding: 0;
    }
    
    .tab-content:last-child {
        page-break-after: auto;
    }
    
    .form-section {
        page-break-inside: avoid;
        box-shadow: none;
        border: 1px solid #ccc;
    }
}

/* ===================================
   NEW HIRE PAPERWORK TABBED FORM STYLES
   Clean White Theme - Matching MyOnboarding
   Add these styles to tpa-common.css
   =================================== */

/* Force Override Dark Theme - Make Everything Light */
.tabbed-form-container,
.tab-navigation,
.form-section,
.tab-content,
.application-header,
.form-table {
    background: white !important;
    color: #1e293b !important;
}

/* Tab Navigation - Clean White Design */
.tab-navigation {
    background: white !important;
    border-radius: 16px;
    box-shadow: 0 4px 16px rgba(0,0,0,0.1);
    margin-bottom: 2rem;
    overflow: hidden;
    border: 1px solid #e5e7eb;
}

.tab-buttons {
    display: flex;
    flex-wrap: wrap;
    background: #f8f9fa !important;
    border-bottom: 1px solid #e5e7eb;
}

.tab-button {
    flex: 1;
    min-width: 140px;
    padding: 1.25rem 1rem;
    background: transparent !important;
    border: none;
    border-right: 1px solid #e5e7eb;
    color: #64748b !important;
    font-size: 0.9rem;
    font-weight: 500;
    cursor: pointer;
    transition: all 0.3s ease;
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 0.5rem;
    text-decoration: none;
    font-family: 'Inter', sans-serif;
}

.tab-button:last-child {
    border-right: none;
}

.tab-button:hover {
    background: #e2e8f0 !important;
    color: #334155 !important;
    transform: translateY(-1px);
}

.tab-button.active {
    background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%) !important;
    color: white !important;
    position: relative;
    box-shadow: 0 4px 12px rgba(25, 118, 210, 0.3);
}

.tab-button .material-icons {
    font-size: 1.4rem;
}

.tab-button span {
    font-size: 0.85rem;
    text-align: center;
    line-height: 1.2;
    font-weight: 600;
}

/* Force White Background for Form Container */
.tabbed-form-container {
    background: white !important;
    border-radius: 16px;
    box-shadow: 0 4px 16px rgba(0,0,0,0.1);
    overflow: hidden;
    border: 1px solid #e5e7eb;
}

/* Tab Content - Force White */
.tab-content {
    padding: 2.5rem;
    display: none;
    animation: fadeIn 0.3s ease-in-out;
    background: white !important;
    color: #1e293b !important;
}

.tab-content.active {
    display: block;
}

@keyframes fadeIn {
    from { opacity: 0; transform: translateY(10px); }
    to { opacity: 1; transform: translateY(0); }
}

/* Tab Headers */
.tab-header {
    margin-bottom: 2rem;
    padding-bottom: 1.5rem;
    border-bottom: 2px solid #f1f5f9;
}

.tab-header h2 {
    color: #1e293b !important;
    font-size: 1.75rem;
    font-weight: 700;
    margin: 0 0 0.5rem 0;
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.tab-header .material-icons {
    color: #1976d2;
    font-size: 1.75rem;
}

.tab-header p {
    color: #64748b !important;
    margin: 0;
    font-size: 1rem;
    line-height: 1.6;
}

/* Form Sections - Force White Background */
.form-section {
    margin-bottom: 2rem;
    padding: 2rem;
    background: white !important;
    border-radius: 12px;
    border: 1px solid #e5e7eb;
    box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}

.form-section:last-child {
    margin-bottom: 0;
}

/* Force White for All Form Elements */
.tabbed-form-container .form-group {
    margin-bottom: 1.5rem;
    background: white !important;
}

.tabbed-form-container .form-label {
    display: block;
    font-weight: 600;
    color: #374151 !important;
    margin-bottom: 0.75rem;
    font-size: 0.9rem;
    background: transparent !important;
}

.tabbed-form-container .form-label.required::after {
    content: ' *';
    color: #ef4444;
    font-weight: bold;
}

/* Form Inputs - Force White Background */
.tabbed-form-container .form-input {
    width: 100%;
    padding: 0.75rem 1rem;
    border: 2px solid #e5e7eb !important;
    border-radius: 8px;
    font-size: 1rem;
    transition: all 0.2s ease;
    background: white !important;
    color: #1e293b !important;
    font-family: 'Inter', sans-serif;
}

.tabbed-form-container .form-input:focus {
    outline: none;
    border-color: #1976d2 !important;
    box-shadow: 0 0 0 3px rgba(25, 118, 210, 0.1) !important;
    background: white !important;
}

.tabbed-form-container .form-input:disabled {
    background-color: #f8f9fa !important;
    color: #9ca3af !important;
    cursor: not-allowed;
}

/* Checkbox and Radio Groups */
.checkbox-group-inline,
.radio-group-inline {
    display: flex;
    flex-wrap: wrap;
    gap: 1rem;
    align-items: center;
    margin-top: 0.75rem;
    background: transparent !important;
}

.checkbox-group,
.radio-group {
    margin-top: 0.75rem;
    background: transparent !important;
}

.tabbed-form-container .form-checkbox,
.tabbed-form-container .form-radio {
    margin-right: 0.5rem;
    accent-color: #1976d2;
    width: 18px;
    height: 18px;
}

.checkbox-group label,
.radio-group label,
.checkbox-group-inline label,
.radio-group-inline label {
    font-weight: 500;
    color: #374151 !important;
    cursor: pointer;
    display: flex;
    align-items: center;
    margin-bottom: 0;
    font-size: 0.9rem;
    background: transparent !important;
}

/* Form Layout */
.form-row {
    display: flex;
    gap: 1.5rem;
    margin-bottom: 1.5rem;
    flex-wrap: wrap;
    background: transparent !important;
}

.form-col-narrow {
    flex: 0 0 120px;
}

.form-col-medium {
    flex: 1;
    min-width: 200px;
}

.form-col-wide {
    flex: 2;
    min-width: 250px;
}

.form-col-full {
    flex: 1 1 100%;
}

.form-col-auto {
    flex: 0 0 auto;
}

/* Date Range */
.date-range {
    display: flex;
    align-items: center;
    gap: 1rem;
    flex-wrap: wrap;
    background: transparent !important;
}

.date-range span {
    color: #64748b !important;
    font-weight: 500;
    flex-shrink: 0;
}

.date-range .form-input {
    flex: 1;
    min-width: 140px;
}

/* Tables - Force White */
.form-table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 1rem;
    background: white !important;
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 2px 8px rgba(0,0,0,0.06);
    border: 1px solid #e5e7eb;
}

.form-table th {
    background: #f8fafc !important;
    padding: 1rem 0.75rem;
    text-align: left;
    font-weight: 600;
    color: #374151 !important;
    border-bottom: 2px solid #e5e7eb;
    font-size: 0.9rem;
}

.form-table td {
    padding: 0.75rem;
    border-bottom: 1px solid #f1f5f9;
    vertical-align: top;
    background: white !important;
    color: #1e293b !important;
}

.form-table tr:last-child td {
    border-bottom: none;
}

.form-table .form-input {
    margin: 0;
    font-size: 0.9rem;
    padding: 0.5rem 0.75rem;
}

/* Employment and Reference Grids */
.employment-grid,
.reference-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1.5rem;
    margin-top: 1rem;
    background: transparent !important;
}

.employment-full-row {
    grid-column: 1 / -1;
}

.employment-section-title,
.reference-section-title {
    color: #1976d2 !important;
    font-size: 1.25rem;
    font-weight: 700;
    margin-bottom: 1.5rem;
    padding-bottom: 0.75rem;
    border-bottom: 2px solid #f1f5f9;
    background: transparent !important;
}

/* Background Check Sections */
.background-check-header,
.authorization-header {
    text-align: center;
    margin-bottom: 2rem;
    padding: 1.5rem;
    background: #f8fafc !important;
    border-radius: 8px;
    border: 1px solid #e5e7eb;
}

.background-check-header h3,
.authorization-header h3 {
    color: #1e293b !important;
    font-size: 1.25rem;
    font-weight: 700;
    margin: 0 0 0.5rem 0;
}

.background-check-header h4,
.authorization-header h4 {
    color: #64748b !important;
    font-size: 1rem;
    font-weight: 500;
    margin: 0;
}

.authorization-text {
    background: white !important;
    padding: 1.5rem;
    border-radius: 8px;
    border: 1px solid #e5e7eb;
    margin: 1rem 0;
    box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.authorization-text p {
    margin-bottom: 1rem;
    line-height: 1.6;
    color: #374151 !important;
    text-align: justify;
}

.authorization-content {
    background: #f8fafc !important;
    padding: 1.5rem;
    border-radius: 8px;
    border-left: 4px solid #1976d2;
    margin: 1rem 0;
}

.authorization-content p {
    margin-bottom: 1rem;
    line-height: 1.6;
    color: #374151 !important;
    text-align: justify;
    font-size: 0.95rem;
}

/* Navigation Buttons */
.form-navigation {
    background: #f8fafc !important;
    padding: 2rem;
    border-top: 1px solid #e5e7eb;
    margin-top: 2rem;
}

.nav-buttons {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 1rem;
    max-width: 600px;
    margin: 0 auto;
}

.nav-buttons .btn {
    min-width: 140px;
    padding: 0.75rem 1.5rem;
    font-weight: 600;
    border-radius: 8px;
    transition: all 0.2s ease;
    text-decoration: none;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    border: 2px solid transparent;
    font-family: 'Inter', sans-serif;
    font-size: 0.9rem;
}

.btn-primary {
    background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%) !important;
    color: white !important;
    border-color: #1976d2;
}

.btn-primary:hover {
    background: linear-gradient(135deg, #1565c0 0%, #1976d2 100%) !important;
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(25, 118, 210, 0.3);
}

.btn-secondary {
    background-color: #64748b !important;
    color: white !important;
    border-color: #64748b;
}

.btn-secondary:hover {
    background-color: #475569 !important;
    border-color: #475569;
    transform: translateY(-1px);
}

.btn-success {
    background: linear-gradient(135deg, #059669 0%, #10b981 100%) !important;
    color: white !important;
    border-color: #059669;
}

.btn-success:hover {
    background: linear-gradient(135deg, #047857 0%, #059669 100%) !important;
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(5, 150, 105, 0.3);
}

.btn-outline {
    background-color: transparent !important;
    color: #1976d2 !important;
    border-color: #1976d2;
}

.btn-outline:hover {
    background-color: #1976d2 !important;
    color: white !important;
    transform: translateY(-1px);
}

/* Application Header */
.application-header {
    text-align: center;
    margin-bottom: 2rem;
    padding: 2rem;
    background: white !important;
    border-radius: 12px;
    border: 1px solid #e5e7eb;
    box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}

.application-title {
    color: #1e293b !important;
    font-size: 2rem;
    font-weight: 700;
    margin: 0 0 0.5rem 0;
}

.application-subtitle {
    color: #64748b !important;
    margin: 0;
    line-height: 1.6;
    max-width: 800px;
    margin: 0 auto;
}

/* Message Panels */
.success-message {
    background: #f0fdf4 !important;
    color: #166534 !important;
    padding: 1rem 1.5rem;
    border-radius: 8px;
    border: 1px solid #bbf7d0;
    margin-bottom: 1.5rem;
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.message-panel {
    padding: 1rem 1.5rem;
    border-radius: 8px;
    margin-bottom: 1.5rem;
}

.message-text.error {
    background: #fef2f2 !important;
    color: #991b1b !important;
    border: 1px solid #fecaca;
}

/* Mandatory Task Header */
.mandatory-task-header {
    background: linear-gradient(135deg, #dc2626 0%, #ef4444 100%) !important;
    color: white !important;
    padding: 2rem;
    border-radius: 12px;
    margin-bottom: 2rem;
    box-shadow: 0 4px 16px rgba(220, 38, 38, 0.2);
}

.progress-tracker {
    margin-bottom: 1rem;
}

.progress-step {
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    background: rgba(255, 255, 255, 0.2);
    padding: 0.5rem 1rem;
    border-radius: 50px;
    font-size: 0.9rem;
    font-weight: 500;
}

.task-title {
    font-size: 1.75rem;
    font-weight: 700;
    margin: 0 0 0.5rem 0;
    display: flex;
    align-items: center;
    gap: 0.75rem;
    color: white !important;
}

.mandatory-badge {
    background: rgba(255, 255, 255, 0.3);
    padding: 0.25rem 0.75rem;
    border-radius: 50px;
    font-size: 0.8rem;
    font-weight: 600;
}

.task-subtitle {
    margin: 0;
    opacity: 0.9;
    font-size: 1rem;
    color: white !important;
}

/* Validation Errors */
.field-validation-error {
    color: #ef4444 !important;
    font-size: 0.8rem;
    margin-top: 0.5rem;
    display: block;
    padding: 0.5rem 0.75rem;
    background: #fef2f2 !important;
    border: 1px solid #fecaca;
    border-radius: 4px;
    border-left: 3px solid #ef4444;
}

/* Responsive Design */
@media (max-width: 768px) {
    .tab-buttons {
        flex-direction: column;
    }
    
    .tab-button {
        flex-direction: row;
        justify-content: flex-start;
        padding: 1rem 1.5rem;
        border-right: none;
        border-bottom: 1px solid #e5e7eb;
        gap: 0.75rem;
    }
    
    .tab-button:last-child {
        border-bottom: none;
    }

    .tab-content {
        padding: 1.5rem;
    }
    
    .form-section {
        padding: 1.5rem;
    }
    
    .form-navigation {
        padding: 1.5rem;
    }
    
    .nav-buttons {
        flex-direction: column;
        gap: 1rem;
    }
    
    .nav-buttons .btn {
        width: 100%;
        min-width: auto;
    }
    
    .form-row {
        flex-direction: column;
        gap: 0;
    }
    
    .form-col-narrow,
    .form-col-medium,
    .form-col-wide {
        flex: 1 1 auto;
        min-width: auto;
    }
    
    .employment-grid,
    .reference-grid {
        grid-template-columns: 1fr;
    }
}

@media (max-width: 576px) {
    .tab-content {
        padding: 1rem;
    }
    
    .form-section {
        padding: 1rem;
    }
    
    .mandatory-task-header {
        padding: 1.5rem;
    }
    
    .task-title {
        font-size: 1.5rem;
        flex-direction: column;
        text-align: center;
        gap: 0.5rem;
    }
    
    .application-header {
        padding: 1.5rem;
    }
    
    .application-title {
        font-size: 1.5rem;
    }
}

/* Additional Force Overrides for Dark Theme Elements */
.tabbed-form-container * {
    background-color: inherit;
}

.tabbed-form-container .form-input,
.tabbed-form-container .form-label,
.tabbed-form-container .form-group,
.tabbed-form-container .checkbox-group,
.tabbed-form-container .radio-group,
.tabbed-form-container .date-range,
.tabbed-form-container .employment-grid,
.tabbed-form-container .reference-grid {
    background: white !important;
    color: #1e293b !important;
}

/* Force Override for Any Dark Backgrounds */
.tab-content *:not(.btn):not(.tab-button):not(.material-icons) {
    background-color: transparent !important;
}

.tab-content input,
.tab-content textarea,
.tab-content select {
    background: white !important;
    color: #1e293b !important;
    border: 2px solid #e5e7eb !important;
}

.tab-content input:focus,
.tab-content textarea:focus,
.tab-content select:focus {
    background: white !important;
    border-color: #1976d2 !important;
    box-shadow: 0 0 0 3px rgba(25, 118, 210, 0.1) !important;
}

/* Force White for Specific Form Elements */
.tabbed-form-container input[type="text"],
.tabbed-form-container input[type="email"],
.tabbed-form-container input[type="tel"],
.tabbed-form-container input[type="date"],
.tabbed-form-container input[type="number"],
.tabbed-form-container textarea,
.tabbed-form-container select {
    background: white !important;
    color: #1e293b !important;
    border: 2px solid #e5e7eb !important;
}

/* Final Override for Any Remaining Dark Elements */
.application-header,
.tab-navigation,
.tabbed-form-container,
.form-section,
.tab-content,
.form-table,
.authorization-text,
.authorization-content {
    background: white !important;
    color: #1e293b !important;
}

.application-header *,
.tab-navigation *,
.tabbed-form-container *,
.form-section *,
.tab-content *,
.form-table *,
.authorization-text *,
.authorization-content * {
    color: inherit !important;
}

/* Ensure Tab Navigation is Always Light */
.tab-buttons,
.tab-button {
    background-color: #f8f9fa !important;
    color: #64748b !important;
}

.tab-button.active {
    background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%) !important;
    color: white !important;
}

/* Print Styles */
@media print {
    .tab-navigation,
    .form-navigation {
        display: none;
    }
    
    .tab-content {
        display: block !important;
        page-break-after: always;
        padding: 0;
        background: white !important;
    }
    
    .tab-content:last-child {
        page-break-after: auto;
    }
    
    .form-section {
        page-break-inside: avoid;
        box-shadow: none;
        border: 1px solid #000;
        background: white !important;
    }
    
    .application-header,
    .mandatory-task-header {
        background: white !important;
        color: black !important;
        box-shadow: none !important;
        border: 1px solid #000 !important;
    }
    
    * {
        background: white !important;
        color: black !important;
    }
}

.form-table tr:last-child td {
    border-bottom: none;
}

.form-table .form-input {
    margin: 0;
    font-size: 0.9rem;
    padding: 0.5rem 0.75rem;
}

/* Education Table Specific */
.education-table .year-checkboxes {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
    align-items: center;
}

.education-table .year-checkboxes label {
    margin: 0;
    font-size: 0.85rem;
}

/* Employment Grid */
.employment-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1.5rem;
    margin-top: 1rem;
}

.employment-full-row {
    grid-column: 1 / -1;
}

.employment-section-title {
    color: #1976d2;
    font-size: 1.25rem;
    font-weight: 700;
    margin-bottom: 1.5rem;
    padding-bottom: 0.75rem;
    border-bottom: 2px solid #f1f5f9;
}

@media (max-width: 768px) {
    .employment-grid {
        grid-template-columns: 1fr;
    }
}

/* Reference Grid */
.reference-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1.5rem;
    margin-top: 1rem;
}

.reference-section-title {
    color: #059669;
    font-size: 1.25rem;
    font-weight: 700;
    margin-bottom: 1.5rem;
    padding-bottom: 0.75rem;
    border-bottom: 2px solid #f1f5f9;
}

@media (max-width: 768px) {
    .reference-grid {
        grid-template-columns: 1fr;
    }
}

/* Background Check Specific */
.background-check-header,
.authorization-header {
    text-align: center;
    margin-bottom: 2rem;
    padding: 1.5rem;
    background: #f8fafc;
    border-radius: 8px;
    border: 1px solid #e5e7eb;
}

.background-check-header h3,
.authorization-header h3 {
    color: #1e293b;
    font-size: 1.25rem;
    font-weight: 700;
    margin: 0 0 0.5rem 0;
}

.background-check-header h4,
.authorization-header h4 {
    color: #64748b;
    font-size: 1rem;
    font-weight: 500;
    margin: 0;
}

.authorization-text {
    background: white;
    padding: 1.5rem;
    border-radius: 8px;
    border: 1px solid #e5e7eb;
    margin: 1rem 0;
    box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.authorization-text p {
    margin-bottom: 1rem;
    line-height: 1.6;
    color: #374151;
    text-align: justify;
}

.authorization-content {
    background: #f8fafc;
    padding: 1.5rem;
    border-radius: 8px;
    border-left: 4px solid #1976d2;
    margin: 1rem 0;
}

.authorization-content p {
    margin-bottom: 1rem;
    line-height: 1.6;
    color: #374151;
    text-align: justify;
    font-size: 0.95rem;
}

/* Signature Lines */
.signature-line {
    height: 2px;
    background: #374151;
    margin-top: 2rem;
    position: relative;
}

.signature-line::before {
    content: 'Signature';
    position: absolute;
    top: -1.5rem;
    left: 0;
    font-size: 0.8rem;
    color: #64748b;
}

/* Criminal Details Table */
.criminal-details-table .form-table th {
    background: #fef2f2;
    color: #991b1b;
}

/* License Table */
.license-table .form-table th {
    background: #f0fdf4;
    color: #166534;
}

/* Form Navigation - MyOnboarding Style */
.form-navigation {
    background: #f8fafc;
    padding: 2rem;
    border-top: 1px solid #e5e7eb;
    margin-top: 2rem;
}

.nav-buttons {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 1rem;
    max-width: 600px;
    margin: 0 auto;
}

/* Button Styles - Matching MyOnboarding */
.nav-buttons .btn {
    min-width: 140px;
    padding: 0.75rem 1.5rem;
    font-weight: 600;
    border-radius: 8px;
    transition: all 0.2s ease;
    text-decoration: none;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    border: 2px solid transparent;
    font-family: 'Inter', sans-serif;
    font-size: 0.9rem;
}

.btn-primary {
    background: linear-gradient(135deg, #1976d2 0%, #42a5f5 100%);
    color: white;
    border-color: #1976d2;
}

.btn-primary:hover {
    background: linear-gradient(135deg, #1565c0 0%, #1976d2 100%);
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(25, 118, 210, 0.3);
}

.btn-secondary {
    background-color: #64748b;
    color: white;
    border-color: #64748b;
}

.btn-secondary:hover {
    background-color: #475569;
    border-color: #475569;
    transform: translateY(-1px);
}

.btn-success {
    background: linear-gradient(135deg, #059669 0%, #10b981 100%);
    color: white;
    border-color: #059669;
}

.btn-success:hover {
    background: linear-gradient(135deg, #047857 0%, #059669 100%);
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(5, 150, 105, 0.3);
}

.btn-outline {
    background-color: transparent;
    color: #1976d2;
    border-color: #1976d2;
}

.btn-outline:hover {
    background-color: #1976d2;
    color: white;
    transform: translateY(-1px);
}

/* Form Notes */
.form-note {
    font-size: 0.85rem;
    color: #64748b;
    font-style: italic;
    margin-top: 0.75rem;
    padding: 0.75rem;
    background: #fefce8;
    border-left: 3px solid #eab308;
    border-radius: 4px;
}

/* Application Header */
.application-header {
    text-align: center;
    margin-bottom: 2rem;
    padding: 2rem;
    background: white;
    border-radius: 12px;
    border: 1px solid #e5e7eb;
    box-shadow: 0 2px 8px rgba(0,0,0,0.06);
}

.application-title {
    color: #1e293b;
    font-size: 2rem;
    font-weight: 700;
    margin: 0 0 0.5rem 0;
}

.application-subtitle {
    color: #64748b;
    margin: 0;
    line-height: 1.6;
    max-width: 800px;
    margin: 0 auto;
}

/* Message Panels */
.success-message {
    background: #f0fdf4;
    color: #166534;
    padding: 1rem 1.5rem;
    border-radius: 8px;
    border: 1px solid #bbf7d0;
    margin-bottom: 1.5rem;
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.message-panel {
    padding: 1rem 1.5rem;
    border-radius: 8px;
    margin-bottom: 1.5rem;
}

.message-text.error {
    background: #fef2f2;
    color: #991b1b;
    border: 1px solid #fecaca;
}

/* Mandatory Task Header - MyOnboarding Style */
.mandatory-task-header {
    background: linear-gradient(135deg, #dc2626 0%, #ef4444 100%);
    color: white;
    padding: 2rem;
    border-radius: 12px;
    margin-bottom: 2rem;
    box-shadow: 0 4px 16px rgba(220, 38, 38, 0.2);
}

.progress-tracker {
    margin-bottom: 1rem;
}

.progress-step {
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    background: rgba(255, 255, 255, 0.2);
    padding: 0.5rem 1rem;
    border-radius: 50px;
    font-size: 0.9rem;
    font-weight: 500;
}

.task-title {
    font-size: 1.75rem;
    font-weight: 700;
    margin: 0 0 0.5rem 0;
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.mandatory-badge {
    background: rgba(255, 255, 255, 0.3);
    padding: 0.25rem 0.75rem;
    border-radius: 50px;
    font-size: 0.8rem;
    font-weight: 600;
}

.task-subtitle {
    margin: 0;
    opacity: 0.9;
    font-size: 1rem;
}

/* Validation Error Styling */
.field-validation-error {
    color: #ef4444;
    font-size: 0.8rem;
    margin-top: 0.5rem;
    display: block;
    padding: 0.5rem 0.75rem;
    background: #fef2f2;
    border: 1px solid #fecaca;
    border-radius: 4px;
    border-left: 3px solid #ef4444;
}

/* Loading States */
.form-loading {
    position: relative;
    pointer-events: none;
    opacity: 0.7;
}

.form-loading::after {
    content: '';
    position: absolute;
    top: 50%;
    left: 50%;
    width: 20px;
    height: 20px;
    margin: -10px 0 0 -10px;
    border: 2px solid transparent;
    border-top: 2px solid #1976d2;
    border-radius: 50%;
    animation: spin 1s linear infinite;
}

@keyframes spin {
    0% { transform: rotate(0deg); }
    100% { transform: rotate(360deg); }
}

/* Enhanced Error States */
.has-error .form-input {
    border-color: #ef4444 !important;
    box-shadow: 0 0 0 2px rgba(239, 68, 68, 0.2) !important;
}

.has-error .form-label {
    color: #ef4444;
}

/* Enhanced Success States */
.has-success .form-input {
    border-color: #10b981 !important;
    box-shadow: 0 0 0 2px rgba(16, 185, 129, 0.2) !important;
}

.has-success .form-label {
    color: #10b981;
}

/* Responsive Design */
@media (max-width: 768px) {
    .tab-content {
        padding: 1.5rem;
    }
    
    .form-section {
        padding: 1.5rem;
    }
    
    .form-navigation {
        padding: 1.5rem;
    }
    
    .nav-buttons {
        flex-direction: column;
        gap: 1rem;
    }
    
    .nav-buttons .btn {
        width: 100%;
        min-width: auto;
    }
    
    .form-table {
        font-size: 0.85rem;
    }
    
    .form-table th,
    .form-table td {
        padding: 0.5rem;
    }
}

@media (max-width: 576px) {
    .mandatory-task-header {
        padding: 1.5rem;
    }
    
    .task-title {
        font-size: 1.5rem;
        flex-direction: column;
        text-align: center;
        gap: 0.5rem;
    }
    
    .application-header {
        padding: 1.5rem;
    }
    
    .application-title {
        font-size: 1.5rem;
    }
    
    .tab-content {
        padding: 1rem;
    }
    
    .form-section {
        padding: 1rem;
    }
}

/* Enhanced Mobile Experience */
@media (max-width: 480px) {
    .form-table th,
    .form-table td {
        padding: 0.5rem 0.25rem;
        font-size: 0.75rem;
    }
    
    .nav-buttons .btn {
        padding: 0.625rem 1rem;
        font-size: 0.85rem;
    }
    
    .employment-section-title,
    .reference-section-title {
        font-size: 1rem;
    }
    
    .background-check-header h3,
    .authorization-header h3 {
        font-size: 1rem;
    }
    
    .authorization-text,
    .authorization-content {
        padding: 1rem;
    }
}

/* Print Styles */
@media print {
    .tab-navigation,
    .form-navigation {
        display: none;
    }
    
    .tab-content {
        display: block !important;
        page-break-after: always;
        padding: 0;
    }
    
    .tab-content:last-child {
        page-break-after: auto;
    }
    
    .form-section {
        page-break-inside: avoid;
        box-shadow: none;
        border: 1px solid #000;
    }
    
    .application-header,
    .mandatory-task-header {
        background: white !important;
        color: black !important;
        box-shadow: none !important;
        border: 1px solid #000 !important;
    }
    
    .signature-line {
        border-bottom: 2px solid #000 !important;
        background: none !important;
    }
}

/* Focus Management */
.tab-button:focus,
.tabbed-form-container .form-input:focus,
.nav-buttons .btn:focus {
    outline: 2px solid #1976d2;
    outline-offset: 2px;
}

.tabbed-form-container .form-checkbox:focus,
.tabbed-form-container .form-radio:focus {
    outline: 2px solid #1976d2;
    outline-offset: 1px;
}

/* High Contrast Mode */
@media (prefers-contrast: high) {
    .form-section {
        border-left-width: 6px;
    }
    
    .tab-button.active {
        background: #000;
        color: #fff;
    }
    
    .tabbed-form-container .form-input:focus {
        border-width: 3px;
    }
}

/* Reduced Motion */
@media (prefers-reduced-motion: reduce) {
    .tab-button,
    .tabbed-form-container .form-input,
    .nav-buttons .btn {
        transition: none;
    }
    
    @keyframes fadeIn {
        from { opacity: 0; }
        to { opacity: 1; }
    }
    
    .form-loading::after {
        animation: none;
        border: 2px solid #1976d2;
    }
};
    border-radius: 8px;
    box-shadow: 0 4px 16px rgba(0,0,0,0.1);
    padding: 1rem;
    border: 1px solid #e5e7eb;
    z-index: 1000;
}

.completion-indicator h4 {
    margin: 0 0 0.5rem 0;
    font-size: 0.9rem;
    color: #374151;
}

.completion-percentage {
    font-size: 1.5rem;
    font-weight: 700;
    color: #1976d2;
    text-align: center;
}

@media (max-width: 1024px) {
    .completion-indicator {
        display: none;
    }
}

/* Smooth Transitions */
.tab-button,
.form-input,
.btn {
    transition: all 0.2s ease;
}

/* Custom Select Styling */
.tabbed-form-container select.form-input {
    appearance: none;
    background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3e%3cpath stroke='%236b7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='M6 8l4 4 4-4'/%3e%3c/svg%3e");
    background-position: right 0.5rem center;
    background-repeat: no-repeat;
    background-size: 1.5em 1.5em;
    padding-right: 2.5rem;
}

/* File Input Styling */
.tabbed-form-container input[type="file"].form-input {
    padding: 0.5rem;
    border: 2px dashed #e5e7eb;
    background: #f8fafc;
}

.tabbed-form-container input[type="file"].form-input:hover {
    border-color: #1976d2;
    background: #f0f9ff;
}

/* Enhanced Table Responsive */
@media (max-width: 768px) {
    .form-table {
        display: block;
        overflow-x: auto;
        white-space: nowrap;
    }
    
    .form-table th,
    .form-table td {
        min-width: 100px;
    }
}

/* Sticky Tab Navigation */
.tab-navigation.sticky {
    position: sticky;
    top: 0;
    z-index: 100;
    margin-bottom: 0;
    border-radius: 0;
}

.tab-navigation.sticky + .tabbed-form-container {
    border-radius: 0 0 16px 16px;
}

/* Animation for Form Validation */
.form-input.shake {
    animation: shake 0.5s;
}

@keyframes shake {
    0%, 100% { transform: translateX(0); }
    25% { transform: translateX(-5px); }
    75% { transform: translateX(5px); }
}

/* Tooltip Styling */
.tooltip-container {
    position: relative;
    display: inline-block;
}

.tooltip-text {
    visibility: hidden;
    width: 200px;
    background-color: #374151;
    color: white;
    text-align: center;
    border-radius: 6px;
    padding: 0.5rem;
    position: absolute;
    z-index: 1001;
    bottom: 125%;
    left: 50%;
    margin-left: -100px;
    opacity: 0;
    transition: opacity 0.3s;
    font-size: 0.8rem;
}

.tooltip-container:hover .tooltip-text {
    visibility: visible;
    opacity: 1;
}

/* Enhanced Print Styles */
@media print {
    .application-header,
    .mandatory-task-header {
        background: white !important;
        color: black !important;
        box-shadow: none !important;
        border: 1px solid #000 !important;
    }
    
    .tab-button.active {
        background: white !important;
        color: black !important;
    }
    
    .form-section {
        background: white !important;
        border: 1px solid #000 !important;
        break-inside: avoid;
    }
    
    .tabbed-form-container .form-input {
        border: 1px solid #000 !important;
        background: white !important;
        box-shadow: none !important;
    }
    
    .signature-line {
        border-bottom: 2px solid #000 !important;
        background: none !important;
    }
}

/* Accessibility Improvements */
.visually-hidden {
    position: absolute !important;
    width: 1px !important;
    height: 1px !important;
    padding: 0 !important;
    margin: -1px !important;
    overflow: hidden !important;
    clip: rect(0, 0, 0, 0) !important;
    white-space: nowrap !important;
    border: 0 !important;
}

/* High Contrast Mode */
@media (prefers-contrast: high) {
    .form-section {
        border-left-width: 6px;
    }
    
    .tab-button.active {
        background: #000;
        color: #fff;
    }
    
    .tabbed-form-container .form-input:focus {
        border-width: 3px;
    }
}

/* Reduced Motion */
@media (prefers-reduced-motion: reduce) {
    .tab-button,
    .tabbed-form-container .form-input,
    .nav-buttons .btn {
        transition: none;
    }
    
    @keyframes fadeIn {
        from { opacity: 0; }
        to { opacity: 1; }
    }
    
    .form-loading::after {
        animation: none;
        border: 2px solid #1976d2;
    }
}form-radio {
    margin-right: 0.5rem;
    accent-color: var(--tpa-primary);
}

.checkbox-group label,
.radio-group label,
.checkbox-group-inline label,
.radio-group-inline label {
    font-weight: normal;
    color: var(--text-secondary);
    cursor: pointer;
    display: flex;
    align-items: center;
    margin-bottom: 0;
    font-size: 0.9rem;
}

/* Form Group Conditional */
.form-group-conditional {
    margin-top: 1rem;
    padding-left: 1.5rem;
    border-left: 3px solid #e9ecef;
}

.form-group-inline {
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

/* Form Layout Utilities */
.form-row {
    display: flex;
    gap: 1rem;
    margin-bottom: 1rem;
    flex-wrap: wrap;
}

.form-col-narrow {
    flex: 0 0 120px;
}

.form-col-medium {
    flex: 1;
    min-width: 200px;
}

.form-col-wide {
    flex: 2;
    min-width: 250px;
}

.form-col-full {
    flex: 1 1 100%;
}

.form-col-auto {
    flex: 0 0 auto;
}

@media (max-width: 768px) {
    .form-row {
        flex-direction: column;
        gap: 0;
    }
    
    .form-col-narrow,
    .form-col-medium,
    .form-col-wide {
        flex: 1 1 auto;
        min-width: auto;
    }
}

/* Date Range Inputs */
.date-range {
    display: flex;
    align-items: center;
    gap: 0.75rem;
    flex-wrap: wrap;
}

.date-range span {
    color: var(--text-secondary);
    font-weight: 500;
    flex-shrink: 0;
}

.date-range .form-input {
    flex: 1;
    min-width: 120px;
}

@media (max-width: 576px) {
    .date-range {
        flex-direction: column;
        align-items: stretch;
        gap: 0.5rem;
    }
}

/* Tables */
.form-table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 1rem;
    background: var(--background-white);
    border-radius: var(--border-radius-small);
    overflow: hidden;
    box-shadow: var(--shadow-light);
    border: 1px solid var(--border-light);
}

.form-table th {
    background: #f8f9fa;
    padding: 0.75rem;
    text-align: left;
    font-weight: 600;
    color: var(--text-primary);
    border-bottom: 2px solid var(--border-light);
    font-size: 0.85rem;
}

.form-table td {
    padding: 0.75rem;
    border-bottom: 1px solid var(--border-light);
    vertical-align: top;
}

.form-table tr:last-child td {
    border-bottom: none;
}

.form-table .form-input {
    margin: 0;
    font-size: 0.85rem;
    padding: 0.5rem;
}

/* Education Table Specific */
.education-table .year-checkboxes {
    display: flex;
    flex-wrap: wrap;
    gap: 0.5rem;
    align-items: center;
}

.education-table .year-checkboxes label {
    margin: 0;
    font-size: 0.8rem;
}

/* Employment Grid */
.employment-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1rem;
    margin-top: 1rem;
}

.employment-full-row {
    grid-column: 1 / -1;
}

.employment-section-title {
    color: var(--tpa-secondary);
    font-size: 1.1rem;
    font-weight: 600;
    margin-bottom: 1rem;
    padding-bottom: 0.5rem;
    border-bottom: 1px solid #e9ecef;
}

@media (max-width: 768px) {
    .employment-grid {
        grid-template-columns: 1fr;
    }
}

/* Reference Grid */
.reference-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 1rem;
    margin-top: 1rem;
}

.reference-section-title {
    color: var(--tpa-success);
    font-size: 1.1rem;
    font-weight: 600;
    margin-bottom: 1rem;
    padding-bottom: 0.5rem;
    border-bottom: 1px solid #e9ecef;
}

@media (max-width: 768px) {
    .reference-grid {
        grid-template-columns: 1fr;
    }
}

/* Background Check Specific */
.background-check-header,
.authorization-header {
    text-align: center;
    margin-bottom: 1.5rem;
    padding: 1rem;
    background: #f8f9fa;
    border-radius: var(--border-radius-small);
    border: 1px solid var(--border-light);
}

.background-check-header h3,
.authorization-header h3 {
    color: var(--text-primary);
    font-size: 1.2rem;
    font-weight: 600;
    margin: 0 0 0.5rem 0;
}

.background-check-header h4,
.authorization-header h4 {
    color: var(--text-secondary);
    font-size: 1rem;
    font-weight: 500;
    margin: 0;
}

.authorization-text {
    background: var(--background-white);
    padding: 1.5rem;
    border-radius: var(--border-radius-small);
    border: 1px solid var(--border-light);
    margin: 1rem 0;
}

.authorization-text p {
    margin-bottom: 1rem;
    line-height: 1.6;
    color: var(--text-primary);
    text-align: justify;
}

.authorization-content {
    background: #f8f9fa;
    padding: 1.5rem;
    border-radius: var(--border-radius-small);
    border-left: 4px solid var(--tpa-primary);
    margin: 1rem 0;
}

.authorization-content p {
    margin-bottom: 1rem;
    line-height: 1.6;
    color: var(--text-primary);
    text-align: justify;
    font-size: 0.9rem;
}

/* Signature Lines */
.signature-line {
    height: 2px;
    background: var(--text-primary);
    margin-top: 2rem;
    position: relative;
}

.signature-line::before {
    content: 'Signature';
    position: absolute;
    top: -1.5rem;
    left: 0;
    font-size: 0.8rem;
    color: var(--text-secondary);
}

/* Criminal Details Table */
.criminal-details-table {
    margin-top: 1rem;
}

.criminal-details-table .form-table th {
    background: var(--tpa-error);
    color: white;
}

/* License Table */
.license-table {
    margin-top: 1rem;
}

.license-table .form-table th {
    background: var(--tpa-success);
    color: white;
}

/* Form Navigation */
.form-navigation {
    background: #f8f9fa;
    padding: 1.5rem 2rem;
    border-top: 1px solid var(--border-light);
    margin-top: 2rem;
}

.nav-buttons {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 1rem;
    max-width: 600px;
    margin: 0 auto;
}

/* Override existing button styles for navigation */
.nav-buttons .btn {
    min-width: 120px;
    padding: 0.75rem 1.5rem;
    font-weight: 500;
    border-radius: var(--border-radius);
    transition: all var(--transition-medium);
    text-decoration: none;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    border: 1px solid transparent;
    font-family: 'Inter', sans-serif;
    font-size: 0.9rem;
    height: auto;
    line-height: 1.4;
}

.btn-primary {
    background: linear-gradient(135deg, var(--tpa-primary) 0%, var(--tpa-primary-dark) 100%);
    color: white;
    border-color: var(--tpa-primary);
}

.btn-primary:hover {
    background: linear-gradient(135deg, var(--tpa-primary-dark) 0%, var(--tpa-primary-darker) 100%);
    transform: translateY(-1px);
    box-shadow: 0 4px 12px rgba(255, 152, 0, 0.3);
}

.btn-secondary {
    background-color: var(--text-secondary);
    color: white;
    border-color: var(--text-secondary);
}

.btn-secondary:hover {
    background-color: var(--text-primary);
    border-color: var(--text-primary);
    transform: translateY(-1px);
}

.btn-success {
    background-color: var(--tpa-success);
    color: white;
    border-color: var(--tpa-success);
}

.btn-success:hover {
    background-color: #45a049;
    border-color: #45a049;
    transform: translateY(-1px);
}

.btn-outline {
    background-color: transparent;
    color: var(--tpa-primary);
    border-color: var(--tpa-primary);
}

.btn-outline:hover {
    background-color: var(--tpa-primary);
    color: white;
    transform: translateY(-1px);
}

/* Form Notes */
.form-note {
    font-size: 0.8rem;
    color: var(--text-secondary);
    font-style: italic;
    margin-top: 0.5rem;
    padding: 0.5rem;
    background: #fff3cd;
    border-left: 3px solid #ffc107;
    border-radius: var(--border-radius-small);
}

/* Responsive Design */
@media (max-width: 992px) {
    .checkbox-group-inline,
    .radio-group-inline {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.5rem;
    }
}

@media (max-width: 768px) {
    .tab-content {
        padding: 1rem;
    }
    
    .form-section {
        padding: 1rem;
    }
    
    .form-navigation {
        padding: 1rem;
    }
    
    .nav-buttons {
        flex-direction: column;
        gap: 0.75rem;
    }
    
    .nav-buttons .btn {
        width: 100%;
        min-width: auto;
    }
    
    .form-table {
        font-size: 0.8rem;
    }
    
    .form-table th,
    .form-table td {
        padding: 0.5rem;
    }
}

@media (max-width: 576px) {
    .tab-header h2 {
        font-size: 1.25rem;
    }
    
    .employment-section-title,
    .reference-section-title {
        font-size: 1rem;
    }
    
    .background-check-header h3,
    .authorization-header h3 {
        font-size: 1.1rem;
    }
    
    .background-check-header h4,
    .authorization-header h4 {
        font-size: 0.9rem;
    }
    
    .authorization-text,
    .authorization-content {
        padding: 1rem;
    }
}

/* Print Styles */
@media print {
    .tab-navigation,
    .form-navigation {
        display: none;
    }
    
    .tab-content {
        display: block !important;
        page-break-after: always;
        padding: 0;
    }
    
    .tab-content:last-child {
        page-break-after: auto;
    }
    
    .form-section {
        page-break-inside: avoid;
        box-shadow: none;
        border: 1px solid #ddd;
    }
    
    .form-table {
        page-break-inside: avoid;
    }
}

/* Accessibility Improvements */
.tab-button:focus,
.tabbed-form-container .form-input:focus,
.nav-buttons .btn:focus {
    outline: 2px solid var(--tpa-primary);
    outline-offset: 2px;
}

.tabbed-form-container .form-checkbox:focus,
.tabbed-form-container .form-radio:focus {
    outline: 2px solid var(--tpa-primary);
    outline-offset: 1px;
}

/* High Contrast Mode Support */
@media (prefers-contrast: high) {
    .form-section {
        border-left-width: 6px;
    }
    
    .tab-button.active {
        background: #000;
        color: #fff;
    }
    
    .tabbed-form-container .form-input:focus {
        border-width: 2px;
    }
}

/* Reduced Motion Support */
@media (prefers-reduced-motion: reduce) {
    .tab-button,
    .tabbed-form-container .form-input,
    .nav-buttons .btn {
        transition: none;
    }
    
    @keyframes fadeIn {
        from { opacity: 0; }
        to { opacity: 1; }
    }
}

/* Additional Application-Specific Styles */
.application-header {
    text-align: center;
    margin-bottom: 2rem;
    padding: 2rem;
    background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
    border-radius: var(--border-radius-large);
    border: 1px solid var(--border-light);
}

.application-title {
    color: var(--text-primary);
    font-size: 2rem;
    font-weight: 700;
    margin: 0 0 0.5rem 0;
}

.application-subtitle {
    color: var(--text-secondary);
    margin: 0;
    line-height: 1.6;
    max-width: 800px;
    margin: 0 auto;
}

/* Message Panels */
.success-message {
    background: #d4edda;
    color: #155724;
    padding: 1rem 1.5rem;
    border-radius: var(--border-radius);
    border: 1px solid #c3e6cb;
    margin-bottom: 1.5rem;
    display: flex;
    align-items: center;
    gap: 0.5rem;
}

.message-panel {
    padding: 1rem 1.5rem;
    border-radius: var(--border-radius);
    margin-bottom: 1.5rem;
}

.message-text.error {
    background: #f8d7da;
    color: #721c24;
    border: 1px solid #f5c6cb;
}

/* Mandatory Task Header Integration */
.mandatory-task-header {
    background: linear-gradient(135deg, #dc3545 0%, #c82333 100%);
    color: white;
    padding: 1.5rem 2rem;
    border-radius: var(--border-radius-large);
    margin-bottom: 2rem;
    box-shadow: var(--shadow-medium);
}

.progress-tracker {
    margin-bottom: 1rem;
}

.progress-step {
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    background: rgba(255, 255, 255, 0.2);
    padding: 0.5rem 1rem;
    border-radius: 50px;
    font-size: 0.9rem;
    font-weight: 500;
}

.task-title {
    font-size: 1.75rem;
    font-weight: 600;
    margin: 0 0 0.5rem 0;
    display: flex;
    align-items: center;
    gap: 0.75rem;
}

.mandatory-badge {
    background: rgba(255, 255, 255, 0.3);
    padding: 0.25rem 0.75rem;
    border-radius: 50px;
    font-size: 0.8rem;
    font-weight: 500;
}

.task-subtitle {
    margin: 0;
    opacity: 0.9;
    font-size: 1.1rem;
}

/* Form Input Date Specific */
.form-input-date {
    max-width: 200px;
}

.form-input-phone {
    max-width: 250px;
}

.form-input-full {
    width: 100%;
}

/* Validation Error Styling */
.field-validation-error {
    color: var(--tpa-error);
    font-size: 0.8rem;
    margin-top: 0.25rem;
    display: block;
    padding: 0.25rem 0.5rem;
    background: #fef2f2;
    border: 1px solid #fecaca;
    border-radius: var(--border-radius-small);
    border-left: 3px solid var(--tpa-error);
}

/* Loading States */
.form-loading {
    position: relative;
    pointer-events: none;
    opacity: 0.7;
}

.form-loading::after {
    content: '';
    position: absolute;
    top: 50%;
    left: 50%;
    width: 20px;
    height: 20px;
    margin: -10px 0 0 -10px;
    border: 2px solid transparent;
    border-top: 2px solid var(--tpa-primary);
    border-radius: 50%;
    animation: spin 1s linear infinite;
}

@keyframes spin {
    0% { transform: rotate(0deg); }
    100% { transform: rotate(360deg); }
}

/* Tab Progress Indicator */
.tab-navigation::before {
    content: '';
    position: absolute;
    top: 0;
    left: 0;
    height: 4px;
    background: var(--tpa-primary);
    transition: width 0.3s ease;
    z-index: 1;
}

/* Focus Management */
.tab-content:focus {
    outline: none;
}

/* Screen Reader Support */
.sr-only {
    position: absolute;
    width: 1px;
    height: 1px;
    padding: 0;
    margin: -1px;
    overflow: hidden;
    clip: rect(0, 0, 0, 0);
    white-space: nowrap;
    border: 0;
}

/* Custom Scrollbar for Tab Content */
.tab-content {
    scrollbar-width: thin;
    scrollbar-color: var(--tpa-primary) #f1f1f1;
}

.tab-content::-webkit-scrollbar {
    width: 8px;
}

.tab-content::-webkit-scrollbar-track {
    background: #f1f1f1;
    border-radius: 4px;
}

.tab-content::-webkit-scrollbar-thumb {
    background: var(--tpa-primary);
    border-radius: 4px;
}

.tab-content::-webkit-scrollbar-thumb:hover {
    background: var(--tpa-primary-dark);
}

/* Enhanced Mobile Experience */
@media (max-width: 480px) {
    .application-header {
        padding: 1.5rem 1rem;
        margin-bottom: 1rem;
    }
    
    .application-title {
        font-size: 1.5rem;
    }
    
    .mandatory-task-header {
        padding: 1rem;
        margin-bottom: 1rem;
    }
    
    .task-title {
        font-size: 1.25rem;
        flex-direction: column;
        text-align: center;
        gap: 0.5rem;
    }
    
    .tab-content {
        padding: 0.75rem;
    }
    
    .form-section {
        padding: 0.75rem;
        margin-bottom: 1rem;
    }
    
    .form-table th,
    .form-table td {
        padding: 0.25rem;
        font-size: 0.7rem;
    }
    
    .nav-buttons .btn {
        padding: 0.625rem 1rem;
        font-size: 0.8rem;
    }
}

/* Dark Mode Support (Future Enhancement) */
@media (prefers-color-scheme: dark) {
    :root {
        --background-white: #1a1a1a;
        --background-light: #2d2d2d;
        --text-primary: #ffffff;
        --text-secondary: #cccccc;
        --border-light: #404040;
        --border-medium: #606060;
    }
    
    .tab-navigation,
    .tabbed-form-container,
    .form-table,
    .authorization-text {
        background: var(--background-white);
        border-color: var(--border-light);
    }
    
    .tab-buttons {
        background: var(--background-light);
    }
    
    .form-section {
        background: var(--background-light);
    }
    
    .tabbed-form-container .form-input {
        background: var(--background-white);
        border-color: var(--border-medium);
        color: var(--text-primary);
    }
    
    .form-table th {
        background: var(--background-light);
    }
}

/* Animation Enhancements */
.tab-button {
    position: relative;
    overflow: hidden;
}

.tab-button::before {
    content: '';
    position: absolute;
    top: 50%;
    left: 50%;
    width: 0;
    height: 0;
    background: rgba(255, 255, 255, 0.1);
    border-radius: 50%;
    transform: translate(-50%, -50%);
    transition: width 0.3s ease, height 0.3s ease;
}

.tab-button:hover::before {
    width: 100%;
    height: 100%;
}

/* Form Completion Progress */
.form-progress {
    background: #f8f9fa;
    height: 4px;
    border-radius: 2px;
    margin-bottom: 2rem;
    overflow: hidden;
}

.form-progress-bar {
    height: 100%;
    background: linear-gradient(90deg, var(--tpa-primary) 0%, var(--tpa-primary-dark) 100%);
    width: 0%;
    transition: width 0.5s ease;
    border-radius: 2px;
}

/* Tooltip Support */
.tooltip {
    position: relative;
    display: inline-block;
}

.tooltip .tooltiptext {
    visibility: hidden;
    width: 200px;
    background-color: var(--text-primary);
    color: white;
    text-align: center;
    border-radius: var(--border-radius-small);
    padding: 0.5rem;
    position: absolute;
    z-index: 1000;
    bottom: 125%;
    left: 50%;
    margin-left: -100px;
    opacity: 0;
    transition: opacity 0.3s;
    font-size: 0.8rem;
}

.tooltip:hover .tooltiptext {
    visibility: visible;
    opacity: 1;
}

/* Print Optimization */
@media print {
    .application-header,
    .mandatory-task-header {
        background: white !important;
        color: black !important;
        box-shadow: none !important;
        border: 1px solid #ccc !important;
    }
    
    .tab-button.active {
        background: white !important;
        color: black !important;
    }
    
    .form-section {
        background: white !important;
        border: 1px solid #ccc !important;
    }
    
    .tabbed-form-container .form-input {
        border: 1px solid #000 !important;
        background: white !important;
    }
}

/* Error State Enhancements */
.has-error .form-input {
    border-color: var(--tpa-error) !important;
    box-shadow: 0 0 0 2px rgba(244, 67, 54, 0.25) !important;
}

.has-error .form-label {
    color: var(--tpa-error);
}

/* Success State Enhancements */
.has-success .form-input {
    border-color: var(--tpa-success) !important;
    box-shadow: 0 0 0 2px rgba(76, 175, 80, 0.25) !important;
}

.has-success .form-label {
    color: var(--tpa-success);
}

/* Form Stepper Integration */
.form-stepper {
    display: flex;
    justify-content: space-between;
    margin-bottom: 2rem;
    padding: 0 2rem;
}

.step {
    flex: 1;
    text-align: center;
    position: relative;
}

.step::after {
    content: '';
    position: absolute;
    top: 15px;
    right: -50%;
    width: 100%;
    height: 2px;
    background: #e0e0e0;
    z-index: 0;
}

.step:last-child::after {
    display: none;
}

.step-number {
    width: 30px;
    height: 30px;
    border-radius: 50%;
    background: #e0e0e0;
    color: #666;
    display: flex;
    align-items: center;
    justify-content: center;
    margin: 0 auto 0.5rem;
    font-weight: 600;
    font-size: 0.9rem;
    position: relative;
    z-index: 1;
}

.step.active .step-number {
    background: var(--tpa-primary);
    color: white;
}

.step.completed .step-number {
    background: var(--tpa-success);
    color: white;
}

.step-title {
    font-size: 0.8rem;
    color: var(--text-secondary);
    margin: 0;
}

.step.active .step-title {
    color: var(--tpa-primary);
    font-weight: 600;
}

@media (max-width: 768px) {
    .form-stepper {
        display: none;
    }
}1fr;
    gap: 1rem;
    margin-top: 1rem;
}

.reference-section-title {
    color: #28a745;
    font-size: 1.1rem;
    font-weight: 600;
    margin-bottom: 1rem;
    padding-bottom: 0.5rem;
    border-bottom: 1px solid #e9ecef;
}

/* Background Check Specific */
.background-check-header,
.authorization-header {
    text-align: center;
    margin-bottom: 1.5rem;
    padding: 1rem;
    background: #f8f9fa;
    border-radius: 4px;
    border: 1px solid #e9ecef;
}

.background-check-header h3,
.authorization-header h3 {
    color: #333;
    font-size: 1.2rem;
    font-weight: 600;
    margin: 0 0 0.5rem 0;
}

.background-check-header h4,
.authorization-header h4 {
    color: #666;
    font-size: 1rem;
    font-weight: 500;
    margin: 0;
}

.authorization-text {
    background: #fff;
    padding: 1.5rem;
    border-radius: 4px;
    border: 1px solid #e9ecef;
    margin: 1rem 0;
}

.authorization-text p {
    margin-bottom: 1rem;
    line-height: 1.6;
    color: #333;
    text-align: justify;
}

.authorization-content {
    background: #f8f9fa;
    padding: 1.5rem;
    border-radius: 4px;
    border-left: 4px solid #007bff;
    margin: 1rem 0;
}

.authorization-content p {
    margin-bottom: 1rem;
    line-height: 1.6;
    color: #333;
    text-align: justify;
    font-size: 0.9rem;
}

/* Signature Lines */
.signature-line {
    height: 2px;
    background: #333;
    margin-top: 2rem;
    position: relative;
}

.signature-line::before {
    content: 'Signature';
    position: absolute;
    top: -1.5rem;
    left: 0;
    font-size: 0.8rem;
    color: #666;
}

/* Criminal Details Table */
.criminal-details-table {
    margin-top: 1rem;
}

.criminal-details-table .form-table th {
    background: #dc3545;
    color: white;
}

/* License Table */
.license-table {
    margin-top: 1rem;
}

.license-table .form-table th {
    background: #28a745;
    color: white;
}

/* Form Navigation */
.form-navigation {
    background: #f8f9fa;
    padding: 1.5rem 2rem;
    border-top: 1px solid #dee2e6;
    margin-top: 2rem;
}

.nav-buttons {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 1rem;
    max-width: 600px;
    margin: 0 auto;
}

.nav-buttons .btn {
    min-width: 120px;
    padding: 0.75rem 1.5rem;
    font-weight: 500;
    border-radius: 4px;
    transition: all 0.3s ease;
    text-decoration: none;
    display: inline-flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    border: 1px solid transparent;
}

.btn-primary {
    background-color: #007bff;
    color: white;
    border-color: #007bff;
}

.btn-primary:hover {
    background-color: #0056b3;
    border-color: #0056b3;
    transform: translateY(-1px);
}

.btn-secondary {
    background-color: #6c757d;
    color: white;
    border-color: #6c757d;
}

.btn-secondary:hover {
    background-color: #545b62;
    border-color: #545b62;
    transform: translateY(-1px);
}

.btn-success {
    background-color: #28a745;
    color: white;
    border-color: #28a745;
}

.btn-success:hover {
    background-color: #1e7e34;
    border-color: #1e7e34;
    transform: translateY(-1px);
}

.btn-outline {
    background-color: transparent;
    color: #007bff;
    border-color: #007bff;
}

.btn-outline:hover {
    background-color: #007bff;
    color: white;
    transform: translateY(-1px);
}

/* Form Notes */
.form-note {
    font-size: 0.8rem;
    color: #666;
    font-style: italic;
    margin-top: 0.5rem;
    padding: 0.5rem;
    background: #fff3cd;
    border-left: 3px solid #ffc107;
    border-radius: 4px;
}

/* Responsive Design */
@media (max-width: 992px) {
    .employment-grid,
    .reference-grid {
        grid-template-columns: 1fr;
    }
    
    .date-range {
        flex-direction: column;
        align-items: stretch;
        gap: 0.5rem;
    }
    
    .checkbox-group-inline,
    .radio-group-inline {
        flex-direction: column;
        align-items: flex-start;
        gap: 0.5rem;
    }
}

@media (max-width: 768px) {
    .tab-content {
        padding: 1rem;
    }
    
    .form-section {
        padding: 1rem;
    }
    
    .form-navigation {
        padding: 1rem;
    }
    
    .nav-buttons {
        flex-direction: column;
        gap: 0.75rem;
    }
    
    .nav-buttons .btn {
        width: 100%;
        min-width: auto;
    }
    
    .form-table {
        font-size: 0.8rem;
    }
    
    .form-table th,
    .form-table td {
        padding: 0.5rem;
    }
}

@media (max-width: 576px) {
    .tab-header h2 {
        font-size: 1.25rem;
    }
    
    .employment-section-title,
    .reference-section-title {
        font-size: 1rem;
    }
    
    .background-check-header h3,
    .authorization-header h3 {
        font-size: 1.1rem;
    }
    
    .background-check-header h4,
    .authorization-header h4 {
        font-size: 0.9rem;
    }
    
    .authorization-text,
    .authorization-content {
        padding: 1rem;
    }
}

/* Print Styles */
@media print {
    .tab-navigation,
    .form-navigation {
        display: none;
    }
    
    .tab-content {
        display: block !important;
        page-break-after: always;
        padding: 0;
    }
    
    .tab-content:last-child {
        page-break-after: auto;
    }
    
    .form-section {
        page-break-inside: avoid;
        box-shadow: none;
        border: 1px solid #ddd;
    }
    
    .form-table {
        page-break-inside: avoid;
    }
}

/* Accessibility Improvements */
.tab-button:focus,
.form-input:focus,
.btn:focus {
    outline: 2px solid #007bff;
    outline-offset: 2px;
}

.form-checkbox:focus,
.form-radio:focus {
    outline: 2px solid #007bff;
    outline-offset: 1px;
}

/* High Contrast Mode Support */
@media (prefers-contrast: high) {
    .form-section {
        border-left-width: 6px;
    }
    
    .tab-button.active {
        background: #000;
        color: #fff;
    }
    
    .form-input:focus {
        border-width: 2px;
    }
}
/* Force Light Sidebar */
.sidebar, .nav-sidebar, .dashboard-nav, .navigation-menu {
    background: #f8f9fa !important;
    color: #1e293b !important;
}

.sidebar a, .nav-sidebar a, .dashboard-nav a {
    color: #374151 !important;
}

.sidebar a:hover, .nav-sidebar a:hover {
    background: #e5e7eb !important;
    color: #1976d2 !important;
}
/* Reduced Motion Support */
@media (prefers-reduced-motion: reduce) {
    .tab-button,
    .form-input,
    .btn {
        transition: none;
    }
    
    @keyframes fadeIn {
        from { opacity: 0; }
        to { opacity: 1; }
    }
}

   </style>
    <!-- Page Header -->
    <div class="mandatory-task-header">
        <div class="progress-tracker">
            <div class="progress-step">
                <i class="material-icons">assignment</i>
                Mandatory Task 1 of 3
            </div>
        </div>
        <h1 class="task-title">
            <i class="material-icons">description</i>
            New Hire Paperwork - Employment Application
            <span class="mandatory-badge">Mandatory</span>
        </h1>
        <p class="task-subtitle">Complete your employment application form with all required information</p>
    </div>

    <!-- Success Message -->
    <asp:Panel ID="pnlSuccessMessage" runat="server" CssClass="success-message" Visible="false">
        <i class="material-icons" style="vertical-align: middle; margin-right: 0.5rem;">check_circle</i>
        <strong>Employment application completed successfully!</strong> You will be redirected to your onboarding dashboard.
    </asp:Panel>

    <!-- Error Messages -->
    <asp:Panel ID="pnlMessages" runat="server" Visible="false" CssClass="message-panel">
        <asp:Label ID="lblMessage" runat="server" CssClass="message-text"></asp:Label>
    </asp:Panel>

    <!-- Application Header -->
    <div class="application-header">
        <h1 class="application-title">Application for Employment</h1>
        <p class="application-subtitle">
            Application will be kept on file for 90 Days<br />
            Please Print and complete application <strong>completely</strong> to be considered. 
            All information must be entered on application form to be considered for employment – even if resume is attached.
        </p>
    </div>

    <!-- Tab Navigation -->
    <div class="tab-navigation">
        <div class="tab-buttons">
            <button type="button" class="tab-button active" data-tab="personal-info">
                <i class="material-icons">person</i>
                <span>Personal Information</span>
            </button>
            <button type="button" class="tab-button" data-tab="position-info">
                <i class="material-icons">work</i>
                <span>Position Information</span>
            </button>
            <button type="button" class="tab-button" data-tab="background">
                <i class="material-icons">security</i>
                <span>Background Questions</span>
            </button>
            <button type="button" class="tab-button" data-tab="education">
                <i class="material-icons">school</i>
                <span>Education</span>
            </button>
            <button type="button" class="tab-button" data-tab="employment">
                <i class="material-icons">history</i>
                <span>Employment History</span>
            </button>
            <button type="button" class="tab-button" data-tab="references">
                <i class="material-icons">contacts</i>
                <span>References</span>
            </button>
            <button type="button" class="tab-button" data-tab="background-check">
                <i class="material-icons">verified_user</i>
                <span>Background Check Authorization</span>
            </button>
        </div>
    </div>

    <!-- Form Container -->
    <div class="tabbed-form-container">
        <asp:UpdatePanel ID="upMain" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <!-- Hidden Fields -->
                <asp:HiddenField ID="hfCurrentTab" runat="server" Value="personal-info" />
                <asp:HiddenField ID="hfApplicationId" runat="server" />

                <!-- Personal Information Tab -->
                <div class="tab-content" id="personal-info" style="display: block;">
                    <div class="tab-header">
                        <h2><i class="material-icons">person</i> Personal Information</h2>
                        <p>Please provide your personal details and contact information.</p>
                    </div>

                    <!-- Application Date -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-auto">
                                <div class="form-group">
                                    <label class="form-label">Date</label>
                                    <asp:TextBox ID="txtApplicationDate" runat="server" CssClass="form-input form-input-date" 
                                        TextMode="Date" Enabled="false" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Name Section -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-wide">
                                <div class="form-group">
                                    <label class="form-label required">Last Name</label>
                                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-input" MaxLength="100" />
                                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" 
                                        ErrorMessage="Last name is required" CssClass="field-validation-error" Display="Dynamic" />
                                </div>
                            </div>
                            <div class="form-col-wide">
                                <div class="form-group">
                                    <label class="form-label required">First Name</label>
                                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-input" MaxLength="100" />
                                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" 
                                        ErrorMessage="First name is required" CssClass="field-validation-error" Display="Dynamic" />
                                </div>
                            </div>
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Middle Name</label>
                                    <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-input" MaxLength="100" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Address Section -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-wide">
                                <div class="form-group">
                                    <label class="form-label">Home Address</label>
                                    <asp:TextBox ID="txtHomeAddress" runat="server" CssClass="form-input" MaxLength="255" />
                                </div>
                            </div>
                            <div class="form-col-narrow">
                                <div class="form-group">
                                    <label class="form-label">Apt. #</label>
                                    <asp:TextBox ID="txtAptNumber" runat="server" CssClass="form-input" MaxLength="20" />
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">City</label>
                                    <asp:TextBox ID="txtCity" runat="server" CssClass="form-input" MaxLength="100" />
                                </div>
                            </div>
                            <div class="form-col-narrow">
                                <div class="form-group">
                                    <label class="form-label">State</label>
                                    <asp:TextBox ID="txtState" runat="server" CssClass="form-input" MaxLength="50" />
                                </div>
                            </div>
                            <div class="form-col-narrow">
                                <div class="form-group">
                                    <label class="form-label">Zip Code</label>
                                    <asp:TextBox ID="txtZipCode" runat="server" CssClass="form-input" MaxLength="20" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- SSN and Driver's License -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Social Security Number</label>
                                    <asp:TextBox ID="txtSSN" runat="server" CssClass="form-input" MaxLength="11" 
                                        placeholder="___-__-____" />
                                </div>
                            </div>
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Driver's Lic #</label>
                                    <asp:TextBox ID="txtDriversLicense" runat="server" CssClass="form-input" MaxLength="50" />
                                </div>
                            </div>
                            <div class="form-col-narrow">
                                <div class="form-group">
                                    <label class="form-label">State</label>
                                    <asp:TextBox ID="txtDLState" runat="server" CssClass="form-input" MaxLength="10" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Phone Numbers -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Phone Number</label>
                                    <asp:TextBox ID="txtPhoneNumber" runat="server" CssClass="form-input form-input-phone" 
                                        MaxLength="20" placeholder="(   )   -    " />
                                </div>
                            </div>
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Cell Number</label>
                                    <asp:TextBox ID="txtCellNumber" runat="server" CssClass="form-input form-input-phone" 
                                        MaxLength="20" placeholder="(   )   -    " />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Emergency Contact -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-wide">
                                <div class="form-group">
                                    <label class="form-label">Emergency Contact Person</label>
                                    <asp:TextBox ID="txtEmergencyContactName" runat="server" CssClass="form-input" MaxLength="200" />
                                </div>
                            </div>
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Relationship</label>
                                    <asp:TextBox ID="txtEmergencyContactRelationship" runat="server" CssClass="form-input" MaxLength="100" />
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Emergency Contact Address and phone:</label>
                                    <asp:TextBox ID="txtEmergencyContactAddress" runat="server" CssClass="form-input form-input-full" 
                                        TextMode="MultiLine" Rows="2" MaxLength="500" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Position Information Tab -->
                <div class="tab-content" id="position-info" style="display: none;">
                    <div class="tab-header">
                        <h2><i class="material-icons">work</i> Position Information</h2>
                        <p>Please specify the position details and your availability.</p>
                    </div>

                    <!-- Position Applied For -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-wide">
                                <div class="form-group">
                                    <label class="form-label">Position(s) Applied For: 1.</label>
                                    <asp:TextBox ID="txtPosition1" runat="server" CssClass="form-input" MaxLength="200" />
                                </div>
                            </div>
                            <div class="form-col-wide">
                                <div class="form-group">
                                    <label class="form-label">2.</label>
                                    <asp:TextBox ID="txtPosition2" runat="server" CssClass="form-input" MaxLength="200" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Salary and Employment Type -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Salary Desired:</label>
                                    <asp:TextBox ID="txtSalaryDesired" runat="server" CssClass="form-input" MaxLength="20" />
                                </div>
                            </div>
                            <div class="form-col-narrow">
                                <div class="form-group">
                                    <label class="form-label">Hourly</label>
                                    <asp:CheckBox ID="chkHourly" runat="server" CssClass="form-checkbox" />
                                </div>
                            </div>
                            <div class="form-col-narrow">
                                <div class="form-group">
                                    <label class="form-label">Yearly</label>
                                    <asp:CheckBox ID="chkYearly" runat="server" CssClass="form-checkbox" />
                                </div>
                            </div>
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Available Start Date:</label>
                                    <asp:TextBox ID="txtAvailableStartDate" runat="server" CssClass="form-input" TextMode="Date" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Employment Sought -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Employment Sought:</label>
                                    <div class="checkbox-group-inline">
                                        <asp:CheckBox ID="chkFullTime" runat="server" Text="Full Time" CssClass="form-checkbox" />
                                        <asp:CheckBox ID="chkPartTime" runat="server" Text="Part Time" CssClass="form-checkbox" />
                                        <asp:CheckBox ID="chkTemporary" runat="server" Text="Temporary" CssClass="form-checkbox" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Desired Location -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Desired Location to work:</label>
                                    <div class="checkbox-group-inline">
                                        <asp:CheckBox ID="chkNashville" runat="server" Text="Nashville" CssClass="form-checkbox" />
                                        <asp:CheckBox ID="chkFranklin" runat="server" Text="Franklin" CssClass="form-checkbox" />
                                        <asp:CheckBox ID="chkShelbyville" runat="server" Text="Shelbyville" CssClass="form-checkbox" />
                                        <asp:CheckBox ID="chkWaynesboro" runat="server" Text="Waynesboro" CssClass="form-checkbox" />
                                        <div class="form-group-inline">
                                            <asp:CheckBox ID="chkOtherLocation" runat="server" Text="Other" CssClass="form-checkbox" />
                                            <asp:TextBox ID="txtOtherLocation" runat="server" CssClass="form-input" MaxLength="100" placeholder="Specify" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Shift Sought -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Shift Sought:</label>
                                    <div class="checkbox-group-inline">
                                        <asp:CheckBox ID="chk1stShift" runat="server" Text="1st Shift" CssClass="form-checkbox" />
                                        <asp:CheckBox ID="chk2ndShift" runat="server" Text="2nd Shift" CssClass="form-checkbox" />
                                        <asp:CheckBox ID="chk3rdShift" runat="server" Text="3rd Shift" CssClass="form-checkbox" />
                                        <asp:CheckBox ID="chkWeekendsOnly" runat="server" Text="Weekends only" CssClass="form-checkbox" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Days Available -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Days Available:</label>
                                    <div class="checkbox-group-inline">
                                        <asp:CheckBox ID="chkMon" runat="server" Text="Mon" CssClass="form-checkbox" />
                                        <asp:CheckBox ID="chkTues" runat="server" Text="Tues" CssClass="form-checkbox" />
                                        <asp:CheckBox ID="chkWed" runat="server" Text="Wed" CssClass="form-checkbox" />
                                        <asp:CheckBox ID="chkThurs" runat="server" Text="Thurs" CssClass="form-checkbox" />
                                        <asp:CheckBox ID="chkFri" runat="server" Text="Fri" CssClass="form-checkbox" />
                                        <asp:CheckBox ID="chkSat" runat="server" Text="Sat" CssClass="form-checkbox" />
                                        <asp:CheckBox ID="chkSun" runat="server" Text="Sun" CssClass="form-checkbox" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-col-full">
                                <p class="form-note">*Assignment of days, shifts, and hours are based on company needs without guaranteed permanency</p>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Background Questions Tab -->
                <div class="tab-content" id="background" style="display: none;">
                    <div class="tab-header">
                        <h2><i class="material-icons">security</i> Background Questions</h2>
                        <p>Please answer all background and eligibility questions honestly.</p>
                    </div>

                    <!-- TPA History Questions -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Have you ever applied for a position with TPA, Inc. before?</label>
                                    <div class="radio-group-inline">
                                        <asp:RadioButton ID="rbAppliedTPAYes" runat="server" GroupName="AppliedTPA" Text="Yes" CssClass="form-radio" />
                                        <asp:RadioButton ID="rbAppliedTPANo" runat="server" GroupName="AppliedTPA" Text="No" CssClass="form-radio" />
                                    </div>
                                    <div class="form-group-conditional">
                                        <label class="form-label">If yes, when?</label>
                                        <asp:TextBox ID="txtAppliedTPADate" runat="server" CssClass="form-input" MaxLength="100" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Have you ever worked for TPA, Inc. before?</label>
                                    <div class="radio-group-inline">
                                        <asp:RadioButton ID="rbWorkedTPAYes" runat="server" GroupName="WorkedTPA" Text="Yes" CssClass="form-radio" />
                                        <asp:RadioButton ID="rbWorkedTPANo" runat="server" GroupName="WorkedTPA" Text="No" CssClass="form-radio" />
                                    </div>
                                    <div class="form-group-conditional">
                                        <label class="form-label">If yes, when?</label>
                                        <asp:TextBox ID="txtWorkedTPADate" runat="server" CssClass="form-input" MaxLength="100" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Do you have any family members employed by TPA, Inc.</label>
                                    <div class="radio-group-inline">
                                        <asp:RadioButton ID="rbFamilyTPAYes" runat="server" GroupName="FamilyTPA" Text="Yes" CssClass="form-radio" />
                                        <asp:RadioButton ID="rbFamilyTPANo" runat="server" GroupName="FamilyTPA" Text="No" CssClass="form-radio" />
                                    </div>
                                    <div class="form-group-conditional">
                                        <label class="form-label">If yes, who?</label>
                                        <asp:TextBox ID="txtFamilyTPADetails" runat="server" CssClass="form-input" MaxLength="200" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Citizenship and Work Authorization -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Are you a U.S. citizen or Permanent Resident?</label>
                                    <div class="radio-group-inline">
                                        <asp:RadioButton ID="rbUSCitizenYes" runat="server" GroupName="USCitizen" Text="Yes" CssClass="form-radio" />
                                        <asp:RadioButton ID="rbUSCitizenNo" runat="server" GroupName="USCitizen" Text="No" CssClass="form-radio" />
                                    </div>
                                    <div class="form-group-conditional">
                                        <label class="form-label">Alien # (if no)</label>
                                        <asp:TextBox ID="txtAlienNumber" runat="server" CssClass="form-input" MaxLength="50" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Or otherwise legally entitled to work in the U.S.A.</label>
                                    <div class="radio-group-inline">
                                        <asp:RadioButton ID="rbLegalWorkYes" runat="server" GroupName="LegalWork" Text="Yes" CssClass="form-radio" />
                                        <asp:RadioButton ID="rbLegalWorkNo" runat="server" GroupName="LegalWork" Text="No" CssClass="form-radio" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Are you 18 years or older?</label>
                                    <div class="radio-group-inline">
                                        <asp:RadioButton ID="rb18OrOlderYes" runat="server" GroupName="Age18" Text="Yes" CssClass="form-radio" />
                                        <asp:RadioButton ID="rb18OrOlderNo" runat="server" GroupName="Age18" Text="No" CssClass="form-radio" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Military and Criminal History -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Have you ever served in the U.S. Armed Forces?</label>
                                    <div class="radio-group-inline">
                                        <asp:RadioButton ID="rbArmedForcesYes" runat="server" GroupName="ArmedForces" Text="Yes" CssClass="form-radio" />
                                        <asp:RadioButton ID="rbArmedForcesNo" runat="server" GroupName="ArmedForces" Text="No" CssClass="form-radio" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Have you ever been convicted of a crime (i.e. misdemeanor or felony)?</label>
                                    <div class="radio-group-inline">
                                        <asp:RadioButton ID="rbConvictedYes" runat="server" GroupName="Convicted" Text="Yes" CssClass="form-radio" />
                                        <asp:RadioButton ID="rbConvictedNo" runat="server" GroupName="Convicted" Text="No" CssClass="form-radio" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Criminal Details Table -->
                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">If yes, please give details including dates, charges, and dispositions</label>
                                    <div class="criminal-details-table">
                                        <table class="form-table">
                                            <thead>
                                                <tr>
                                                    <th>DATE</th>
                                                    <th>CHARGE</th>
                                                    <th>STATUS OR OUTCOME</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td><asp:TextBox ID="txtCriminalDate1" runat="server" CssClass="form-input" TextMode="Date" /></td>
                                                    <td><asp:TextBox ID="txtCriminalCharge1" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                                    <td><asp:TextBox ID="txtCriminalStatus1" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                                </tr>
                                                <tr>
                                                    <td><asp:TextBox ID="txtCriminalDate2" runat="server" CssClass="form-input" TextMode="Date" /></td>
                                                    <td><asp:TextBox ID="txtCriminalCharge2" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                                    <td><asp:TextBox ID="txtCriminalStatus2" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                                </tr>
                                                <tr>
                                                    <td><asp:TextBox ID="txtCriminalDate3" runat="server" CssClass="form-input" TextMode="Date" /></td>
                                                    <td><asp:TextBox ID="txtCriminalCharge3" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                                    <td><asp:TextBox ID="txtCriminalStatus3" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Additional Background Questions -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Does your name appear on an abuse registry?</label>
                                    <div class="radio-group-inline">
                                        <asp:RadioButton ID="rbAbuseRegistryYes" runat="server" GroupName="AbuseRegistry" Text="Yes" CssClass="form-radio" />
                                        <asp:RadioButton ID="rbAbuseRegistryNo" runat="server" GroupName="AbuseRegistry" Text="No" CssClass="form-radio" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Have you ever been found guilty abusing, neglecting, or mistreating individuals?</label>
                                    <div class="radio-group-inline">
                                        <asp:RadioButton ID="rbAbuseFoundGuiltyYes" runat="server" GroupName="AbuseFoundGuilty" Text="Yes" CssClass="form-radio" />
                                        <asp:RadioButton ID="rbAbuseFoundGuiltyNo" runat="server" GroupName="AbuseFoundGuilty" Text="No" CssClass="form-radio" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Has your license and/or certification in any health care profession ever been revoked, suspended, limited, or placed on probation or discipline in any state?</label>
                                    <div class="radio-group-inline">
                                        <asp:RadioButton ID="rbLicenseRevokedYes" runat="server" GroupName="LicenseRevoked" Text="Yes" CssClass="form-radio" />
                                        <asp:RadioButton ID="rbLicenseRevokedNo" runat="server" GroupName="LicenseRevoked" Text="No" CssClass="form-radio" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Education Tab -->
                <div class="tab-content" id="education" style="display: none;">
                    <div class="tab-header">
                        <h2><i class="material-icons">school</i> Education</h2>
                        <p>Please provide your educational background and training.</p>
                    </div>

                    <!-- Education Table -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="education-table">
                                    <table class="form-table">
                                        <thead>
                                            <tr>
                                                <th></th>
                                                <th>Elementary School</th>
                                                <th>High School</th>
                                                <th>Undergraduate College/University</th>
                                                <th>Graduate/Professional</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td><strong>School Name and Location</strong></td>
                                                <td><asp:TextBox ID="txtElementarySchool" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                                <td><asp:TextBox ID="txtHighSchool" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                                <td><asp:TextBox ID="txtUndergraduate" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                                <td><asp:TextBox ID="txtGraduate" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                            </tr>
                                            <tr>
                                                <td><strong>Years Completed (circle)</strong></td>
                                                <td>
                                                    <div class="year-checkboxes">
                                                        <asp:CheckBox ID="chkElem1" runat="server" Text="1" />
                                                        <asp:CheckBox ID="chkElem2" runat="server" Text="2" />
                                                        <asp:CheckBox ID="chkElem3" runat="server" Text="3" />
                                                        <asp:CheckBox ID="chkElem4" runat="server" Text="4" />
                                                        <asp:CheckBox ID="chkElem5" runat="server" Text="5" />
                                                        <asp:CheckBox ID="chkElem6" runat="server" Text="6" />
                                                        <asp:CheckBox ID="chkElem7" runat="server" Text="7" />
                                                        <asp:CheckBox ID="chkElem8" runat="server" Text="8" />
                                                    </div>
                                                </td>
                                                <td>
                                                    <div class="year-checkboxes">
                                                        <asp:CheckBox ID="chkHS9" runat="server" Text="9" />
                                                        <asp:CheckBox ID="chkHS10" runat="server" Text="10" />
                                                        <asp:CheckBox ID="chkHS11" runat="server" Text="11" />
                                                        <asp:CheckBox ID="chkHS12" runat="server" Text="12" />
                                                    </div>
                                                </td>
                                                <td>
                                                    <div class="year-checkboxes">
                                                        <asp:CheckBox ID="chkUG1" runat="server" Text="1" />
                                                        <asp:CheckBox ID="chkUG2" runat="server" Text="2" />
                                                        <asp:CheckBox ID="chkUG3" runat="server" Text="3" />
                                                        <asp:CheckBox ID="chkUG4" runat="server" Text="4" />
                                                        <asp:CheckBox ID="chkUG5" runat="server" Text="5" />
                                                    </div>
                                                </td>
                                                <td>
                                                    <div class="year-checkboxes">
                                                        <asp:CheckBox ID="chkGrad1" runat="server" Text="1" />
                                                        <asp:CheckBox ID="chkGrad2" runat="server" Text="2" />
                                                        <asp:CheckBox ID="chkGrad3" runat="server" Text="3" />
                                                        <asp:CheckBox ID="chkGrad4" runat="server" Text="4" />
                                                        <asp:CheckBox ID="chkGrad5" runat="server" Text="5" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td><strong>Diploma/Degree</strong></td>
                                                <td>
                                                    <div class="radio-group-inline">
                                                        <asp:RadioButton ID="rbElemDiplomaYes" runat="server" GroupName="ElemDiploma" Text="Yes" />
                                                        <asp:RadioButton ID="rbElemDiplomaNo" runat="server" GroupName="ElemDiploma" Text="No" />
                                                    </div>
                                                </td>
                                                <td>
                                                    <div class="radio-group-inline">
                                                        <asp:RadioButton ID="rbHSDiplomaYes" runat="server" GroupName="HSDiploma" Text="Yes" />
                                                        <asp:RadioButton ID="rbHSDiplomaNo" runat="server" GroupName="HSDiploma" Text="No" />
                                                    </div>
                                                </td>
                                                <td>
                                                    <div class="radio-group-inline">
                                                        <asp:RadioButton ID="rbUGDiplomaYes" runat="server" GroupName="UGDiploma" Text="Yes" />
                                                        <asp:RadioButton ID="rbUGDiplomaNo" runat="server" GroupName="UGDiploma" Text="No" />
                                                    </div>
                                                </td>
                                                <td>
                                                    <div class="radio-group-inline">
                                                        <asp:RadioButton ID="rbGradDiplomaYes" runat="server" GroupName="GradDiploma" Text="Yes" />
                                                        <asp:RadioButton ID="rbGradDiplomaNo" runat="server" GroupName="GradDiploma" Text="No" />
                                                    </div>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td><strong>Major/Minor</strong></td>
                                                <td><asp:TextBox ID="txtElemMajor" runat="server" CssClass="form-input" MaxLength="100" /></td>
                                                <td><asp:TextBox ID="txtHSMajor" runat="server" CssClass="form-input" MaxLength="100" /></td>
                                                <td><asp:TextBox ID="txtUGMajor" runat="server" CssClass="form-input" MaxLength="100" /></td>
                                                <td><asp:TextBox ID="txtGradMajor" runat="server" CssClass="form-input" MaxLength="100" /></td>
                                            </tr>
                                            <tr>
                                                <td><strong>Describe any specialized Training or skills</strong></td>
                                                <td><asp:TextBox ID="txtElemSkills" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                                <td><asp:TextBox ID="txtHSSkills" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                                <td><asp:TextBox ID="txtUGSkills" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                                <td><asp:TextBox ID="txtGradSkills" runat="server" CssClass="form-input" MaxLength="200" /></td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Special Knowledge and Skills -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Special knowledge, skills, and abilities you wish considered. Include equipment or machines you operate, computer, languages, laboratory techniques, etc. (if applying for secretarial/typist positions, indicate typing speed (WPM)</label>
                                    <asp:TextBox ID="txtSpecialSkills" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="3" MaxLength="1000" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Licenses and Certifications -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">SPECIAL SKILLS, TRAINING, CERTIFICATIONS, and/or LICENSURES<br />
                                    List fields of work for which you are licensed, registered, or certified. Please include license numbers, dates, and sources of licensure.</label>
                                    <div class="license-table">
                                        <table class="form-table">
                                            <thead>
                                                <tr>
                                                    <th>Type of License/Certificate</th>
                                                    <th>State</th>
                                                    <th>ID Number</th>
                                                    <th>Expiration Date</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr>
                                                    <td><asp:TextBox ID="txtLicenseType1" runat="server" CssClass="form-input" MaxLength="100" /></td>
                                                    <td><asp:TextBox ID="txtLicenseState1" runat="server" CssClass="form-input" MaxLength="20" /></td>
                                                    <td><asp:TextBox ID="txtLicenseNumber1" runat="server" CssClass="form-input" MaxLength="50" /></td>
                                                    <td><asp:TextBox ID="txtLicenseExpiration1" runat="server" CssClass="form-input" TextMode="Date" /></td>
                                                </tr>
                                                <tr>
                                                    <td><asp:TextBox ID="txtLicenseType2" runat="server" CssClass="form-input" MaxLength="100" /></td>
                                                    <td><asp:TextBox ID="txtLicenseState2" runat="server" CssClass="form-input" MaxLength="20" /></td>
                                                    <td><asp:TextBox ID="txtLicenseNumber2" runat="server" CssClass="form-input" MaxLength="50" /></td>
                                                    <td><asp:TextBox ID="txtLicenseExpiration2" runat="server" CssClass="form-input" TextMode="Date" /></td>
                                                </tr>
                                                <tr>
                                                    <td><asp:TextBox ID="txtLicenseType3" runat="server" CssClass="form-input" MaxLength="100" /></td>
                                                    <td><asp:TextBox ID="txtLicenseState3" runat="server" CssClass="form-input" MaxLength="20" /></td>
                                                    <td><asp:TextBox ID="txtLicenseNumber3" runat="server" CssClass="form-input" MaxLength="50" /></td>
                                                    <td><asp:TextBox ID="txtLicenseExpiration3" runat="server" CssClass="form-input" TextMode="Date" /></td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- DIDD Training -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">List Dept. of Intellectual and Developmental Disabilities (DIDD) training/classes you have:</label>
                                    <asp:TextBox ID="txtDIDDTraining" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2" MaxLength="500" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Employment History Tab -->
                <div class="tab-content" id="employment" style="display: none;">
                    <div class="tab-header">
                        <h2><i class="material-icons">history</i> Employment Experience</h2>
                        <p>Start with your present or last job. Include any job-related military service assignments and volunteer activities that have given you experience related to your job. Please explain any extended lapses between employments.</p>
                    </div>

                    <!-- Employment History 1 (Current/Most Recent) -->
                    <div class="form-section">
                        <h3 class="employment-section-title">Current or Most Recent Employment</h3>
                        <div class="employment-grid">
                            <div class="form-group">
                                <label class="form-label">Employer</label>
                                <asp:TextBox ID="txtEmployer1" runat="server" CssClass="form-input" MaxLength="200" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Dates Employed From / To</label>
                                <div class="date-range">
                                    <asp:TextBox ID="txtEmploymentFrom1" runat="server" CssClass="form-input" TextMode="Date" />
                                    <span>to</span>
                                    <asp:TextBox ID="txtEmploymentTo1" runat="server" CssClass="form-input" TextMode="Date" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Job Title</label>
                                <asp:TextBox ID="txtJobTitle1" runat="server" CssClass="form-input" MaxLength="100" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Supervisor</label>
                                <asp:TextBox ID="txtSupervisor1" runat="server" CssClass="form-input" MaxLength="100" />
                            </div>
                            <div class="form-group employment-full-row">
                                <label class="form-label">Address</label>
                                <asp:TextBox ID="txtEmployerAddress1" runat="server" CssClass="form-input" MaxLength="200" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">City, State, Zip Code</label>
                                <asp:TextBox ID="txtEmployerCityStateZip1" runat="server" CssClass="form-input" MaxLength="100" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Telephone Number(s)</label>
                                <asp:TextBox ID="txtEmployerPhone1" runat="server" CssClass="form-input" MaxLength="50" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Hourly Rate of Pay - Starting</label>
                                <asp:TextBox ID="txtStartingPay1" runat="server" CssClass="form-input" MaxLength="20" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Final</label>
                                <asp:TextBox ID="txtFinalPay1" runat="server" CssClass="form-input" MaxLength="20" />
                            </div>
                            <div class="form-group employment-full-row">
                                <label class="form-label">Title/Work Performed</label>
                                <asp:TextBox ID="txtWorkPerformed1" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2" MaxLength="500" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Are you still employed (Yes or No)</label>
                                <div class="radio-group-inline">
                                    <asp:RadioButton ID="rbStillEmployed1Yes" runat="server" GroupName="StillEmployed1" Text="Yes" />
                                    <asp:RadioButton ID="rbStillEmployed1No" runat="server" GroupName="StillEmployed1" Text="No" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Eligible for Rehire (Yes or No)</label>
                                <div class="radio-group-inline">
                                    <asp:RadioButton ID="rbEligibleRehire1Yes" runat="server" GroupName="EligibleRehire1" Text="Yes" />
                                    <asp:RadioButton ID="rbEligibleRehire1No" runat="server" GroupName="EligibleRehire1" Text="No" />
                                </div>
                            </div>
                            <div class="form-group employment-full-row">
                                <label class="form-label">Reason for Leaving</label>
                                <asp:TextBox ID="txtReasonLeaving1" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2" MaxLength="300" />
                            </div>
                        </div>
                    </div>

                    <!-- Employment History 2 -->
                    <div class="form-section">
                        <h3 class="employment-section-title">Previous Employment</h3>
                        <div class="employment-grid">
                            <div class="form-group">
                                <label class="form-label">Employer</label>
                                <asp:TextBox ID="txtEmployer2" runat="server" CssClass="form-input" MaxLength="200" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Dates Employed From / To</label>
                                <div class="date-range">
                                    <asp:TextBox ID="txtEmploymentFrom2" runat="server" CssClass="form-input" TextMode="Date" />
                                    <span>to</span>
                                    <asp:TextBox ID="txtEmploymentTo2" runat="server" CssClass="form-input" TextMode="Date" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Job Title</label>
                                <asp:TextBox ID="txtJobTitle2" runat="server" CssClass="form-input" MaxLength="100" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Supervisor</label>
                                <asp:TextBox ID="txtSupervisor2" runat="server" CssClass="form-input" MaxLength="100" />
                            </div>
                            <div class="form-group employment-full-row">
                                <label class="form-label">Address</label>
                                <asp:TextBox ID="txtEmployerAddress2" runat="server" CssClass="form-input" MaxLength="200" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">City, State, Zip Code</label>
                                <asp:TextBox ID="txtEmployerCityStateZip2" runat="server" CssClass="form-input" MaxLength="100" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Telephone Number(s)</label>
                                <asp:TextBox ID="txtEmployerPhone2" runat="server" CssClass="form-input" MaxLength="50" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Hourly Rate of Pay - Starting</label>
                                <asp:TextBox ID="txtStartingPay2" runat="server" CssClass="form-input" MaxLength="20" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Final</label>
                                <asp:TextBox ID="txtFinalPay2" runat="server" CssClass="form-input" MaxLength="20" />
                            </div>
                            <div class="form-group employment-full-row">
                                <label class="form-label">Title/Work Performed</label>
                                <asp:TextBox ID="txtWorkPerformed2" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2" MaxLength="500" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Eligible for Rehire (Yes or No)</label>
                                <div class="radio-group-inline">
                                    <asp:RadioButton ID="rbEligibleRehire2Yes" runat="server" GroupName="EligibleRehire2" Text="Yes" />
                                    <asp:RadioButton ID="rbEligibleRehire2No" runat="server" GroupName="EligibleRehire2" Text="No" />
                                </div>
                            </div>
                            <div class="form-group employment-full-row">
                                <label class="form-label">Reason for Leaving</label>
                                <asp:TextBox ID="txtReasonLeaving2" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2" MaxLength="300" />
                            </div>
                        </div>
                    </div>

                    <!-- Employment History 3 -->
                    <div class="form-section">
                        <h3 class="employment-section-title">Previous Employment</h3>
                        <div class="employment-grid">
                            <div class="form-group">
                                <label class="form-label">Employer</label>
                                <asp:TextBox ID="txtEmployer3" runat="server" CssClass="form-input" MaxLength="200" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Dates Employed From / To</label>
                                <div class="date-range">
                                    <asp:TextBox ID="txtEmploymentFrom3" runat="server" CssClass="form-input" TextMode="Date" />
                                    <span>to</span>
                                    <asp:TextBox ID="txtEmploymentTo3" runat="server" CssClass="form-input" TextMode="Date" />
                                </div>
                            </div>
                            <div class="form-group">
                                <label class="form-label">Job Title</label>
                                <asp:TextBox ID="txtJobTitle3" runat="server" CssClass="form-input" MaxLength="100" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Supervisor</label>
                                <asp:TextBox ID="txtSupervisor3" runat="server" CssClass="form-input" MaxLength="100" />
                            </div>
                            <div class="form-group employment-full-row">
                                <label class="form-label">Address</label>
                                <asp:TextBox ID="txtEmployerAddress3" runat="server" CssClass="form-input" MaxLength="200" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">City, State, Zip Code</label>
                                <asp:TextBox ID="txtEmployerCityStateZip3" runat="server" CssClass="form-input" MaxLength="100" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Telephone Number(s)</label>
                                <asp:TextBox ID="txtEmployerPhone3" runat="server" CssClass="form-input" MaxLength="50" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Hourly Rate of Pay - Starting</label>
                                <asp:TextBox ID="txtStartingPay3" runat="server" CssClass="form-input" MaxLength="20" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Final</label>
                                <asp:TextBox ID="txtFinalPay3" runat="server" CssClass="form-input" MaxLength="20" />
                            </div>
                            <div class="form-group employment-full-row">
                                <label class="form-label">Title/Work Performed</label>
                                <asp:TextBox ID="txtWorkPerformed3" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2" MaxLength="500" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Eligible for Rehire (Yes or No)</label>
                                <div class="radio-group-inline">
                                    <asp:RadioButton ID="rbEligibleRehire3Yes" runat="server" GroupName="EligibleRehire3" Text="Yes" />
                                    <asp:RadioButton ID="rbEligibleRehire3No" runat="server" GroupName="EligibleRehire3" Text="No" />
                                </div>
                            </div>
                            <div class="form-group employment-full-row">
                                <label class="form-label">Reason for Leaving</label>
                                <asp:TextBox ID="txtReasonLeaving3" runat="server" CssClass="form-input" TextMode="MultiLine" Rows="2" MaxLength="300" />
                            </div>
                        </div>
                    </div>
                </div>

                <!-- References Tab -->
                <div class="tab-content" id="references" style="display: none;">
                    <div class="tab-header">
                        <h2><i class="material-icons">contacts</i> Request for Professional References</h2>
                        <p>To further process your application, please provide three (3) personal references who can provide professional reference about your character, ability and suitability for the position you have applied for.</p>
                        <p><strong>*At least one (1) personal reference must have known you for at least 5 years</strong></p>
                    </div>

                    <!-- Professional Reference #1 -->
                    <div class="form-section">
                        <h3 class="reference-section-title">Professional Reference #1</h3>
                        <div class="reference-grid">
                            <div class="form-group">
                                <label class="form-label">First and last name:</label>
                                <asp:TextBox ID="txtReference1Name" runat="server" CssClass="form-input" MaxLength="200" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Phone number:</label>
                                <asp:TextBox ID="txtReference1Phone" runat="server" CssClass="form-input" MaxLength="50" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">E-mail address:</label>
                                <asp:TextBox ID="txtReference1Email" runat="server" CssClass="form-input" MaxLength="200" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">How many years have you known personal reference?</label>
                                <asp:TextBox ID="txtReference1Years" runat="server" CssClass="form-input" MaxLength="10" />
                            </div>
                        </div>
                    </div>

                    <!-- Professional Reference #2 -->
                    <div class="form-section">
                        <h3 class="reference-section-title">Professional Reference #2</h3>
                        <div class="reference-grid">
                            <div class="form-group">
                                <label class="form-label">First and last name:</label>
                                <asp:TextBox ID="txtReference2Name" runat="server" CssClass="form-input" MaxLength="200" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Phone number:</label>
                                <asp:TextBox ID="txtReference2Phone" runat="server" CssClass="form-input" MaxLength="50" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">E-mail address:</label>
                                <asp:TextBox ID="txtReference2Email" runat="server" CssClass="form-input" MaxLength="200" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">How many years have you known personal reference?</label>
                                <asp:TextBox ID="txtReference2Years" runat="server" CssClass="form-input" MaxLength="10" />
                            </div>
                        </div>
                    </div>

                    <!-- Professional Reference #3 -->
                    <div class="form-section">
                        <h3 class="reference-section-title">Professional Reference #3</h3>
                        <div class="reference-grid">
                            <div class="form-group">
                                <label class="form-label">First and last name:</label>
                                <asp:TextBox ID="txtReference3Name" runat="server" CssClass="form-input" MaxLength="200" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">Phone number:</label>
                                <asp:TextBox ID="txtReference3Phone" runat="server" CssClass="form-input" MaxLength="50" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">E-mail address:</label>
                                <asp:TextBox ID="txtReference3Email" runat="server" CssClass="form-input" MaxLength="200" />
                            </div>
                            <div class="form-group">
                                <label class="form-label">How many years have you known personal reference?</label>
                                <asp:TextBox ID="txtReference3Years" runat="server" CssClass="form-input" MaxLength="10" />
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Background Check Authorization Tab -->
                <div class="tab-content" id="background-check" style="display: none;">
                    <div class="tab-header">
                        <h2><i class="material-icons">verified_user</i> Background Check Authorization</h2>
                        <p>Please complete the background investigation and authorization forms below.</p>
                    </div>

                    <!-- Background Investigation Form -->
                    <div class="form-section">
                        <div class="background-check-header">
                            <h3>Tennessee Personal Assistance, Inc. - Nashville</h3>
                            <h4>DISCLOSURE AND AUTHORIZATION FORM</h4>
                            <h4>(1) BACKGROUND INVESTIGATION QUESTIONNAIRE:</h4>
                        </div>

                        <!-- Personal Information for Background Check -->
                        <div class="form-row">
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Name (Last)</label>
                                    <asp:TextBox ID="txtBGLastName" runat="server" CssClass="form-input" MaxLength="100" />
                                </div>
                            </div>
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">(First)</label>
                                    <asp:TextBox ID="txtBGFirstName" runat="server" CssClass="form-input" MaxLength="100" />
                                </div>
                            </div>
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">(Middle Name)</label>
                                    <asp:TextBox ID="txtBGMiddleName" runat="server" CssClass="form-input" MaxLength="100" />
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-wide">
                                <div class="form-group">
                                    <label class="form-label">Address (Street)</label>
                                    <asp:TextBox ID="txtBGStreet" runat="server" CssClass="form-input" MaxLength="255" />
                                </div>
                            </div>
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">(City)</label>
                                    <asp:TextBox ID="txtBGCity" runat="server" CssClass="form-input" MaxLength="100" />
                                </div>
                            </div>
                            <div class="form-col-narrow">
                                <div class="form-group">
                                    <label class="form-label">(State)</label>
                                    <asp:TextBox ID="txtBGState" runat="server" CssClass="form-input" MaxLength="50" />
                                </div>
                            </div>
                            <div class="form-col-narrow">
                                <div class="form-group">
                                    <label class="form-label">(Zip Code)</label>
                                    <asp:TextBox ID="txtBGZipCode" runat="server" CssClass="form-input" MaxLength="20" />
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Social Security Number:</label>
                                    <asp:TextBox ID="txtBGSSN" runat="server" CssClass="form-input" MaxLength="11" placeholder="___-__-____" />
                                </div>
                            </div>
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Telephone Number:</label>
                                    <asp:TextBox ID="txtBGPhone" runat="server" CssClass="form-input" MaxLength="50" />
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-wide">
                                <div class="form-group">
                                    <label class="form-label">Other Name (s): (Used Within the Last 7YRS. E.g. Maiden, Other Married Names)</label>
                                    <asp:TextBox ID="txtBGOtherNames" runat="server" CssClass="form-input" MaxLength="200" />
                                </div>
                            </div>
                            <div class="form-col-narrow">
                                <div class="form-group">
                                    <label class="form-label">Year of Name Change</label>
                                    <asp:TextBox ID="txtBGNameChangeYear" runat="server" CssClass="form-input" MaxLength="10" />
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Driver's License Number:</label>
                                    <asp:TextBox ID="txtBGDriversLicense" runat="server" CssClass="form-input" MaxLength="50" />
                                </div>
                            </div>
                            <div class="form-col-narrow">
                                <div class="form-group">
                                    <label class="form-label">State</label>
                                    <asp:TextBox ID="txtBGDriversLicenseState" runat="server" CssClass="form-input" MaxLength="10" />
                                </div>
                            </div>
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Date of Birth:</label>
                                    <asp:TextBox ID="txtBGDateOfBirth" runat="server" CssClass="form-input" TextMode="Date" />
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group">
                                    <label class="form-label">Name on Driver's License:</label>
                                    <asp:TextBox ID="txtBGNameOnLicense" runat="server" CssClass="form-input" MaxLength="200" />
                                </div>
                            </div>
                        </div>

                        <!-- Previous Addresses -->
                        <div class="form-section">
                            <h4>Previous Residential Addresses (Previous 7 years):</h4>
                            
                            <!-- Former Address 1 -->
                            <div class="form-row">
                                <div class="form-col-full">
                                    <label class="form-label">Former Address:</label>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="form-col-wide">
                                    <div class="form-group">
                                        <label class="form-label">Street</label>
                                        <asp:TextBox ID="txtBGFormerStreet1" runat="server" CssClass="form-input" MaxLength="255" />
                                    </div>
                                </div>
                                <div class="form-col-medium">
                                    <div class="form-group">
                                        <label class="form-label">City</label>
                                        <asp:TextBox ID="txtBGFormerCity1" runat="server" CssClass="form-input" MaxLength="100" />
                                    </div>
                                </div>
                                <div class="form-col-narrow">
                                    <div class="form-group">
                                        <label class="form-label">State</label>
                                        <asp:TextBox ID="txtBGFormerState1" runat="server" CssClass="form-input" MaxLength="50" />
                                    </div>
                                </div>
                                <div class="form-col-narrow">
                                    <div class="form-group">
                                        <label class="form-label">Years Resided</label>
                                        <asp:TextBox ID="txtBGFormerYears1" runat="server" CssClass="form-input" MaxLength="20" />
                                    </div>
                                </div>
                            </div>

                            <!-- Former Address 2 -->
                            <div class="form-row">
                                <div class="form-col-full">
                                    <label class="form-label">Former Address:</label>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="form-col-wide">
                                    <div class="form-group">
                                        <label class="form-label">Street</label>
                                        <asp:TextBox ID="txtBGFormerStreet2" runat="server" CssClass="form-input" MaxLength="255" />
                                    </div>
                                </div>
                                <div class="form-col-medium">
                                    <div class="form-group">
                                        <label class="form-label">City</label>
                                        <asp:TextBox ID="txtBGFormerCity2" runat="server" CssClass="form-input" MaxLength="100" />
                                    </div>
                                </div>
                                <div class="form-col-narrow">
                                    <div class="form-group">
                                        <label class="form-label">State</label>
                                        <asp:TextBox ID="txtBGFormerState2" runat="server" CssClass="form-input" MaxLength="50" />
                                    </div>
                                </div>
                                <div class="form-col-narrow">
                                    <div class="form-group">
                                        <label class="form-label">Years Resided</label>
                                        <asp:TextBox ID="txtBGFormerYears2" runat="server" CssClass="form-input" MaxLength="20" />
                                    </div>
                                </div>
                            </div>

                            <!-- Former Address 3 -->
                            <div class="form-row">
                                <div class="form-col-full">
                                    <label class="form-label">Former Address:</label>
                                </div>
                            </div>
                            <div class="form-row">
                                <div class="form-col-wide">
                                    <div class="form-group">
                                        <label class="form-label">Street</label>
                                        <asp:TextBox ID="txtBGFormerStreet3" runat="server" CssClass="form-input" MaxLength="255" />
                                    </div>
                                </div>
                                <div class="form-col-medium">
                                    <div class="form-group">
                                        <label class="form-label">City</label>
                                        <asp:TextBox ID="txtBGFormerCity3" runat="server" CssClass="form-input" MaxLength="100" />
                                    </div>
                                </div>
                                <div class="form-col-narrow">
                                    <div class="form-group">
                                        <label class="form-label">State</label>
                                        <asp:TextBox ID="txtBGFormerState3" runat="server" CssClass="form-input" MaxLength="50" />
                                    </div>
                                </div>
                                <div class="form-col-narrow">
                                    <div class="form-group">
                                        <label class="form-label">Years Resided</label>
                                        <asp:TextBox ID="txtBGFormerYears3" runat="server" CssClass="form-input" MaxLength="20" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <!-- Criminal Background Questions -->
                        <div class="form-section">
                            <div class="form-row">
                                <div class="form-col-full">
                                    <div class="form-group">
                                        <label class="form-label">Have you been convicted of any criminal offense, either misdemeanor or felony, other than minor traffic violations in the last 7 years?</label>
                                        <div class="radio-group-inline">
                                            <asp:RadioButton ID="rbBGConvicted7YearsYes" runat="server" GroupName="BGConvicted7Years" Text="Yes" />
                                            <asp:RadioButton ID="rbBGConvicted7YearsNo" runat="server" GroupName="BGConvicted7Years" Text="No" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="form-row">
                                <div class="form-col-full">
                                    <div class="form-group">
                                        <label class="form-label">Are you currently charged or under investigation for any violation of the law other than minor traffic violations?</label>
                                        <div class="radio-group-inline">
                                            <asp:RadioButton ID="rbBGChargedInvestigationYes" runat="server" GroupName="BGChargedInvestigation" Text="Yes" />
                                            <asp:RadioButton ID="rbBGChargedInvestigationNo" runat="server" GroupName="BGChargedInvestigation" Text="No" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Pre-Employment Reference Check Authorization -->
                    <div class="form-section">
                        <div class="authorization-header">
                            <h3>PRE-EMPLOYMENT REFERENCE CHECK</h3>
                            <h4>RELEASE & AUTHORIZATION TO CONDUCT REFERENCE CHECKS</h4>
                        </div>

                        <div class="authorization-text">
                            <p>The person named below has applied for a position with our company. Their consideration for employment is largely dependent on this reference form. Below is a signed authorization and consent from the applicant for our company to obtain reference information. Your prompt cooperation, time and attention in completing this reference will be greatly appreciated.</p>
                        </div>

                        <h4>Applicant's Authorization, Release and Request for Reference Information</h4>
                        
                        <div class="authorization-content">
                            <p>I _________________________________ have applied for a position with TPA, Inc. I authorize all my current and former employers to provide reference information, including my job performance, my work record and attendance, the reason(s) for my leaving, my eligibility for rehire and my suitability for the position I am now seeking. I encourage my current and former employers to provide complete response to requests for information, which is believed to be true but not documented. I realize some information may be complimentary and some may be critical. I promise I will not bring any legal claims or actions against my current or former employer due to the response to job reference requests. I recognize that recognition is also a State Statute; which provide my employers with certain protection from such claims. I realize that one is required to give a reference, so I make this commitment to encourage the free exchange of reference information.</p>
                            
                            <p>I signed this release voluntarily and was not required to do so as part of the application process.</p>
                        </div>

                        <div class="form-row">
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">SSN # (last 4 digits only)</label>
                                    <asp:TextBox ID="txtBGSSNLast4" runat="server" CssClass="form-input" MaxLength="4" placeholder="XXXX" />
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-wide">
                                <div class="form-group">
                                    <label class="form-label">Applicant's Signature</label>
                                    <div class="signature-line"></div>
                                </div>
                            </div>
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Date</label>
                                    <asp:TextBox ID="txtBGSignatureDate" runat="server" CssClass="form-input" TextMode="Date" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- DIDD Authorization Sections -->
                    <div class="form-section">
                        <div class="authorization-header">
                            <h3>TENNESSEE PERSONAL ASSISTANCE, INC</h3>
                            <h4>AUTHORIZATION AND GENERAL RELEASE FOR DIDD, BUREAU OF TENNCARE & TPA, INC</h4>
                        </div>

                        <p>I, the undersigned applicant certify and affirm that, to the best of my knowledge and belief;</p>

                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group checkbox-group">
                                    <asp:CheckBox ID="chkDIDDNoAbuse" runat="server" CssClass="form-checkbox" />
                                    <label class="form-label">I have NOT had a case of abuse, neglect, mistreatment or exploitation substantiated against me</label>
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group checkbox-group">
                                    <asp:CheckBox ID="chkDIDDHadAbuse" runat="server" CssClass="form-checkbox" />
                                    <label class="form-label">I have had a case of abuse, neglect, mistreatment or exploitation substantiated against me</label>
                                </div>
                            </div>
                        </div>

                        <div class="authorization-text">
                            <p>As a condition of submitting this application and in order to verify this affirmation, I further release and authorize Tennessee Personal Assistance, the Tennessee Department of Intellectual and Developmental Disabilities and the Bureau of TennCare to have full and complete access to any and all current or prior personnel or investigative records, from any party, person, business, entity or agency, whether governmental or non-governmental, as pertains to any allegations against me of abuse, neglect, mistreatment or exploitation and to consider this information as may be deemed appropriate. This authorization extends to my providing any applicable information in personnel or investigative reports concerning my employment with this employer to my future employers who may be Providers of DIDD Services</p>
                        </div>

                        <div class="form-row">
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Full Name (Last, First, Middle):</label>
                                    <asp:TextBox ID="txtDIDDFullName" runat="server" CssClass="form-input" MaxLength="200" />
                                </div>
                            </div>
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">SSN #:</label>
                                    <asp:TextBox ID="txtDIDDSSN" runat="server" CssClass="form-input" MaxLength="11" placeholder="___-__-____" />
                                </div>
                            </div>
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Date of Birth:</label>
                                    <asp:TextBox ID="txtDIDDDateOfBirth" runat="server" CssClass="form-input" TextMode="Date" />
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Driver License or ID #</label>
                                    <asp:TextBox ID="txtDIDDDriverLicense" runat="server" CssClass="form-input" MaxLength="50" />
                                </div>
                            </div>
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Witness</label>
                                    <div class="signature-line"></div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Protection from Harm Statement -->
                    <div class="form-section">
                        <div class="authorization-header">
                            <h4>PROTECTION FROM HARM STATEMENT</h4>
                        </div>

                        <p>I, _________________________________ certify and affirm that, to the best of my knowledge and belief;</p>

                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group checkbox-group">
                                    <asp:CheckBox ID="chkProtectionNoAbuse" runat="server" CssClass="form-checkbox" />
                                    <label class="form-label">I have NOT had a case of abuse, neglect, mistreatment or exploitation substantiated against me</label>
                                </div>
                            </div>
                        </div>

                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group checkbox-group">
                                    <asp:CheckBox ID="chkProtectionHadAbuse" runat="server" CssClass="form-checkbox" />
                                    <label class="form-label">I have had a case of abuse, neglect, mistreatment or exploitation substantiated against me</label>
                                </div>
                            </div>
                        </div>

                        <div class="authorization-text">
                            <p>In order to verify this affirmation, I further release and authorize Tennessee Personal Assistance, the Tennessee Department of Intellectual and Developmental Disabilities and the Bureau of TennCare to have full and complete access to any and all current or prior personnel or investigative records as pertains to substantiated allegations against me of abuse, neglect, mistreatment or exploitation.</p>
                        </div>

                        <div class="form-row">
                            <div class="form-col-medium">
                                <div class="form-group">
                                    <label class="form-label">Witness:</label>
                                    <div class="signature-line"></div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Final Acknowledgment -->
                    <div class="form-section">
                        <div class="form-row">
                            <div class="form-col-full">
                                <div class="form-group checkbox-group">
                                    <asp:CheckBox ID="chkFinalAcknowledgment" runat="server" CssClass="form-checkbox" />
                                    <label class="form-label">I acknowledge that all information provided in this application is true and complete to the best of my knowledge. I understand that any false information or omission may disqualify me from further consideration for employment and may result in my dismissal if discovered at a later date.</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Form Navigation Buttons -->
                <div class="form-navigation">
                    <div class="nav-buttons">
                        <asp:Button ID="btnPrevious" runat="server" Text="Previous" CssClass="btn btn-secondary" 
                            OnClientClick="return false;" />
                        <asp:Button ID="btnNext" runat="server" Text="Next" CssClass="btn btn-primary" 
                            OnClientClick="return false;" />
                        <asp:Button ID="btnSaveDraft" runat="server" Text="Save Draft" CssClass="btn btn-outline" 
                            OnClick="btnSaveDraft_Click" />
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit Application" CssClass="btn btn-success" 
                            OnClick="btnSubmit_Click" style="display: none;" />
                    </div>
                </div>

            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <!-- Tab JavaScript -->
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            initializeTabbedForm();
        });

        function initializeTabbedForm() {
            const tabButtons = document.querySelectorAll('.tab-button');
            const tabContents = document.querySelectorAll('.tab-content');
            const btnPrevious = document.getElementById('<%= btnPrevious.ClientID %>');
            const btnNext = document.getElementById('<%= btnNext.ClientID %>');
            const btnSubmit = document.getElementById('<%= btnSubmit.ClientID %>');
            const hfCurrentTab = document.getElementById('<%= hfCurrentTab.ClientID %>');

            let currentTabIndex = 0;
            const totalTabs = tabButtons.length;

            // Initialize first tab
            showTab(0);

            // Tab button click handlers
            tabButtons.forEach((button, index) => {
                button.addEventListener('click', function () {
                    showTab(index);
                });
            });

            // Previous button handler
            btnPrevious.addEventListener('click', function (e) {
                e.preventDefault();
                if (currentTabIndex > 0) {
                    showTab(currentTabIndex - 1);
                }
            });

            // Next button handler
            btnNext.addEventListener('click', function (e) {
                e.preventDefault();
                if (currentTabIndex < totalTabs - 1) {
                    showTab(currentTabIndex + 1);
                }
            });

            function showTab(index) {
                // Hide all tab contents
                tabContents.forEach(content => {
                    content.style.display = 'none';
                });

                // Remove active class from all buttons
                tabButtons.forEach(button => {
                    button.classList.remove('active');
                });

                // Show current tab content
                tabContents[index].style.display = 'block';
                tabButtons[index].classList.add('active');

                // Update current tab index
                currentTabIndex = index;
                hfCurrentTab.value = tabContents[index].id;

                // Update navigation buttons
                btnPrevious.style.display = (index === 0) ? 'none' : 'inline-block';

                if (index === totalTabs - 1) {
                    btnNext.style.display = 'none';
                    btnSubmit.style.display = 'inline-block';
                } else {
                    btnNext.style.display = 'inline-block';
                    btnSubmit.style.display = 'none';
                }

                // Scroll to top of form
                document.querySelector('.tabbed-form-container').scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        }
    </script>

</asp:Content>