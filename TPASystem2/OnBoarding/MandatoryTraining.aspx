<%@ Page Title="Mandatory Training" Language="C#" MasterPageFile="~/DashboardMaster.Master" AutoEventWireup="true" CodeBehind="MandatoryTraining.aspx.cs" Inherits="TPASystem2.Training.MandatoryTraining" EnableEventValidation="false" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="DashboardContent" runat="server">
    <link href='<%=ResolveUrl("~/Content/css/tpa-dashboard.css") %>' rel="stylesheet" type="text/css" />
<link href='<%=ResolveUrl("~/Content/css/tpa-common.css") %>' rel="stylesheet" type="text/css" />
<link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">

    <style>
        /* Task-specific styling */
        .mandatory-task-header {
            background: linear-gradient(135deg, #1565c0 0%, #1976d2 100%);
            color: white;
            padding: 2rem;
            border-radius: 12px 12px 0 0;
            margin-bottom: 0;
        }
        
        .task-title {
            display: flex;
            align-items: center;
            gap: 1rem;
            font-size: 2rem;
            font-weight: 600;
            margin: 1rem 0;
        }
        
        .task-subtitle {
            font-size: 1.1rem;
            opacity: 0.9;
            margin: 0;
        }
        
        .mandatory-badge {
            background: #ff5722;
            color: white;
            padding: 0.25rem 0.75rem;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }
        
        .form-container {
            background: white;
            border-radius: 0 0 12px 12px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            overflow: hidden;
        }
        
        .training-section {
            padding: 2rem;
            border-bottom: 1px solid #e0e0e0;
        }
        
        .training-section:last-child {
            border-bottom: none;
        }
        
        .section-header {
            display: flex;
            align-items: center;
            gap: 1rem;
            margin-bottom: 1.5rem;
        }
        
        .section-icon {
            background: #e3f2fd;
            color: #1565c0;
            width: 3rem;
            height: 3rem;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        .section-title {
            font-size: 1.5rem;
            font-weight: 600;
            margin: 0;
            color: #333;
        }
        
        .training-module {
            background: #f8f9fa;
            border: 2px solid #e0e0e0;
            border-radius: 12px;
            padding: 1.5rem;
            margin-bottom: 1.5rem;
            transition: all 0.3s ease;
        }
        
        .training-module:hover {
            border-color: #1565c0;
            box-shadow: 0 4px 12px rgba(21, 101, 192, 0.1);
        }
        
        .module-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1rem;
        }
        
        .module-title {
            font-size: 1.2rem;
            font-weight: 600;
            color: #333;
            margin: 0;
        }
        
        .module-duration {
            background: #1565c0;
            color: white;
            padding: 0.25rem 0.75rem;
            border-radius: 20px;
            font-size: 0.8rem;
            font-weight: 600;
        }
        
        .module-description {
            color: #666;
            line-height: 1.6;
            margin-bottom: 1rem;
        }
        
        .module-content {
            background: white;
            border-radius: 8px;
            padding: 1.5rem;
            margin-bottom: 1rem;
        }
        
        .content-list {
            list-style: none;
            padding: 0;
            margin: 0;
        }
        
        .content-list li {
            padding: 0.5rem 0;
            border-bottom: 1px solid #f0f0f0;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .content-list li:last-child {
            border-bottom: none;
        }
        
        .content-list i {
            color: #4caf50;
            font-size: 1.2rem;
        }
        
        .quiz-section {
            background: #fff3e0;
            border: 2px solid #ff9800;
            border-radius: 8px;
            padding: 1rem;
            margin-top: 1rem;
        }
        
        .quiz-title {
            color: #e65100;
            font-weight: 600;
            margin-bottom: 0.5rem;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .quiz-info {
            color: #bf360c;
            font-size: 0.9rem;
        }
        
        .checkbox-group {
            display: flex;
            align-items: flex-start;
            gap: 0.75rem;
            padding: 1rem;
            background: #f8f9fa;
            border-radius: 8px;
            border: 2px solid #e0e0e0;
            transition: border-color 0.3s ease;
            margin-bottom: 1rem;
        }
        
        .checkbox-group:hover {
            border-color: #1565c0;
        }
        
        .checkbox-label {
            flex: 1;
            line-height: 1.5;
            cursor: pointer;
            margin: 0;
        }
        
        .progress-tracker {
            display: flex;
            justify-content: center;
            margin-bottom: 2rem;
        }
        
        .progress-step {
            display: flex;
            align-items: center;
            gap: 0.5rem;
            padding: 0.5rem 1rem;
            background: #e3f2fd;
            color: #1565c0;
            border-radius: 25px;
            font-weight: 600;
        }
        
        .form-actions {
            display: flex;
            justify-content: space-between;
            gap: 1rem;
            padding: 2rem;
            background: #f8f9fa;
            border-top: 1px solid #e0e0e0;
        }
        
        .btn-primary {
            background: #1565c0;
            color: white;
            border: none;
            padding: 0.75rem 2rem;
            border-radius: 8px;
            font-size: 1rem;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }
        
        .btn-primary:hover {
            background: #0d47a1;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(21, 101, 192, 0.3);
        }
        
        .btn-secondary {
            background: #f5f5f5;
            color: #333;
            border: 2px solid #e0e0e0;
            padding: 0.75rem 2rem;
            border-radius: 8px;
            font-size: 1rem;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
        }
        
        .btn-secondary:hover {
            background: #e0e0e0;
            color: #333;
        }
        
        .success-message {
            background: #e8f5e8;
            color: #2e7d32;
            padding: 1rem;
            border-radius: 8px;
            margin-bottom: 1rem;
            border: 2px solid #4caf50;
        }
        
        .completion-summary {
            background: #e3f2fd;
            border: 2px solid #1565c0;
            border-radius: 12px;
            padding: 2rem;
            text-align: center;
            margin-bottom: 2rem;
        }
        
        .completion-icon {
            font-size: 4rem;
            color: #1565c0;
            margin-bottom: 1rem;
        }
        
        .completion-title {
            font-size: 1.5rem;
            font-weight: 600;
            color: #1565c0;
            margin-bottom: 0.5rem;
        }
        
        .completion-subtitle {
            color: #666;
            margin-bottom: 1.5rem;
        }
        
        @media (max-width: 768px) {
            .module-header {
                flex-direction: column;
                align-items: flex-start;
                gap: 0.5rem;
            }
            
            .form-actions {
                flex-direction: column;
            }
        }
    </style>

    <!-- Page Header -->
    <div class="mandatory-task-header">
        <div class="progress-tracker">
            <div class="progress-step">
                <i class="material-icons">school</i>
                Mandatory Task 3 of 3
            </div>
        </div>
        <h1 class="task-title">
            <i class="material-icons">school</i>
            Mandatory Training
            <span class="mandatory-badge">Mandatory</span>
        </h1>
        <p class="task-subtitle">Complete essential training modules to ensure workplace safety and compliance</p>
    </div>

    <!-- Success Message -->
    <asp:Panel ID="pnlSuccessMessage" runat="server" CssClass="success-message" Visible="false">
        <i class="material-icons" style="vertical-align: middle; margin-right: 0.5rem;">check_circle</i>
        <strong>Training completed successfully!</strong> You will be redirected to your onboarding dashboard.
    </asp:Panel>

    <!-- Form Container -->
    <div class="form-container">
        <!-- Training Completion Summary -->
        <div class="completion-summary">
            <div class="completion-icon">
                <i class="material-icons">school</i>
            </div>
            <h2 class="completion-title">Essential Training Modules</h2>
            <p class="completion-subtitle">Complete all modules below to fulfill your mandatory training requirements</p>
        </div>

        <!-- Company Orientation Module -->
        <div class="training-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">business</i>
                </div>
                <h2 class="section-title">Company Orientation</h2>
            </div>

            <div class="training-module">
                <div class="module-header">
                    <h3 class="module-title">TPA Company Overview & Culture</h3>
                    <span class="module-duration">30 minutes</span>
                </div>
                <p class="module-description">
                    Learn about TPA's history, mission, values, and organizational structure. 
                    Understand our company culture and how you fit into our team.
                </p>
                <div class="module-content">
                    <ul class="content-list">
                        <li><i class="material-icons">check_circle</i> Company history and milestones</li>
                        <li><i class="material-icons">check_circle</i> Mission, vision, and core values</li>
                        <li><i class="material-icons">check_circle</i> Organizational structure and departments</li>
                        <li><i class="material-icons">check_circle</i> Company policies and procedures</li>
                        <li><i class="material-icons">check_circle</i> Communication channels and expectations</li>
                    </ul>
                </div>
                <div class="quiz-section">
                    <div class="quiz-title">
                        <i class="material-icons">quiz</i>
                        Knowledge Check
                    </div>
                    <p class="quiz-info">5 questions • 80% passing score required</p>
                </div>
            </div>
        </div>

        <!-- Workplace Safety Module -->
        <div class="training-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">security</i>
                </div>
                <h2 class="section-title">Workplace Safety</h2>
            </div>

            <div class="training-module">
                <div class="module-header">
                    <h3 class="module-title">Safety Protocols & Emergency Procedures</h3>
                    <span class="module-duration">45 minutes</span>
                </div>
                <p class="module-description">
                    Essential safety training covering workplace hazards, emergency procedures, 
                    and your role in maintaining a safe work environment.
                </p>
                <div class="module-content">
                    <ul class="content-list">
                        <li><i class="material-icons">check_circle</i> Workplace hazard identification</li>
                        <li><i class="material-icons">check_circle</i> Emergency evacuation procedures</li>
                        <li><i class="material-icons">check_circle</i> Fire safety and prevention</li>
                        <li><i class="material-icons">check_circle</i> First aid and medical emergencies</li>
                        <li><i class="material-icons">check_circle</i> Incident reporting procedures</li>
                        <li><i class="material-icons">check_circle</i> Personal protective equipment (PPE)</li>
                    </ul>
                </div>
                <div class="quiz-section">
                    <div class="quiz-title">
                        <i class="material-icons">quiz</i>
                        Safety Assessment
                    </div>
                    <p class="quiz-info">10 questions • 85% passing score required</p>
                </div>
            </div>
        </div>

        <!-- Code of Conduct Module -->
        <div class="training-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">gavel</i>
                </div>
                <h2 class="section-title">Code of Conduct</h2>
            </div>

            <div class="training-module">
                <div class="module-header">
                    <h3 class="module-title">Ethics & Professional Standards</h3>
                    <span class="module-duration">30 minutes</span>
                </div>
                <p class="module-description">
                    Understand TPA's code of conduct, ethical standards, and professional 
                    behavior expectations in the workplace.
                </p>
                <div class="module-content">
                    <ul class="content-list">
                        <li><i class="material-icons">check_circle</i> Professional conduct standards</li>
                        <li><i class="material-icons">check_circle</i> Anti-harassment and discrimination policies</li>
                        <li><i class="material-icons">check_circle</i> Confidentiality and data protection</li>
                        <li><i class="material-icons">check_circle</i> Conflict of interest guidelines</li>
                        <li><i class="material-icons">check_circle</i> Reporting violations and concerns</li>
                    </ul>
                </div>
                <div class="quiz-section">
                    <div class="quiz-title">
                        <i class="material-icons">quiz</i>
                        Ethics Quiz
                    </div>
                    <p class="quiz-info">8 questions • 80% passing score required</p>
                </div>
            </div>
        </div>

        <!-- IT Security Module -->
        <div class="training-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">security</i>
                </div>
                <h2 class="section-title">IT Security Awareness</h2>
            </div>

            <div class="training-module">
                <div class="module-header">
                    <h3 class="module-title">Cybersecurity & Data Protection</h3>
                    <span class="module-duration">15 minutes</span>
                </div>
                <p class="module-description">
                    Learn essential cybersecurity practices to protect company data and systems 
                    from security threats and breaches.
                </p>
                <div class="module-content">
                    <ul class="content-list">
                        <li><i class="material-icons">check_circle</i> Password security best practices</li>
                        <li><i class="material-icons">check_circle</i> Phishing and email security</li>
                        <li><i class="material-icons">check_circle</i> Safe internet browsing</li>
                        <li><i class="material-icons">check_circle</i> Data backup and protection</li>
                        <li><i class="material-icons">check_circle</i> Incident reporting procedures</li>
                    </ul>
                </div>
                <div class="quiz-section">
                    <div class="quiz-title">
                        <i class="material-icons">quiz</i>
                        Security Check
                    </div>
                    <p class="quiz-info">5 questions • 80% passing score required</p>
                </div>
            </div>
        </div>

        <!-- Completion Acknowledgment -->
        <div class="training-section">
            <div class="section-header">
                <div class="section-icon">
                    <i class="material-icons">fact_check</i>
                </div>
                <h2 class="section-title">Training Completion</h2>
            </div>

            <div class="checkbox-group">
                <asp:CheckBox ID="chkTrainingCompletion" runat="server" />
                <label class="checkbox-label" for="<%= chkTrainingCompletion.ClientID %>">
                    <strong>Training Completion Acknowledgment:</strong> I confirm that I have completed all mandatory training modules listed above, 
                    including watching all training materials and passing all required quizzes with the minimum passing scores. 
                    I understand the information presented and agree to follow all policies and procedures outlined in the training.
                </label>
            </div>

            <div class="checkbox-group">
                <asp:CheckBox ID="chkPolicyUnderstanding" runat="server" />
                <label class="checkbox-label" for="<%= chkPolicyUnderstanding.ClientID %>">
                    <strong>Policy Understanding:</strong> I acknowledge that I have read, understood, and agree to comply with 
                    all company policies, safety procedures, and code of conduct requirements as presented in the training modules.
                </label>
            </div>

            <div class="checkbox-group">
                <asp:CheckBox ID="chkContinuousLearning" runat="server" />
                <label class="checkbox-label" for="<%= chkContinuousLearning.ClientID %>">
                    <strong>Continuous Learning Commitment:</strong> I understand that training is an ongoing process and commit to 
                    participating in future training sessions, staying updated on policy changes, and maintaining my knowledge of 
                    workplace safety and security practices.
                </label>
            </div>
        </div>

        <!-- Form Actions -->
        <div class="form-actions">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn-secondary" 
                OnClick="btnCancel_Click" />
            <asp:Button ID="btnCompleteTraining" runat="server" Text="Complete Training" CssClass="btn-primary" 
                OnClick="btnCompleteTraining_Click" />
        </div>
    </div>

    <!-- Loading overlay -->
    <div id="loadingOverlay" style="display: none; position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); z-index: 10000; align-items: center; justify-content: center;">
        <div style="background: white; padding: 2rem; border-radius: 12px; text-align: center;">
            <i class="material-icons" style="font-size: 3rem; color: #1565c0; animation: spin 1s linear infinite;">sync</i>
            <p style="margin: 1rem 0 0 0; font-weight: 600;">Completing your training...</p>
        </div>
    </div>

    <style>
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
    </style>

    <script>
        // Disable all client-side validation
        function Page_ClientValidate() { return true; }
        if (typeof ValidatorOnSubmit === 'function') {
            ValidatorOnSubmit = function() { return true; };
        }

        // Show loading overlay on form submit
        function showLoading() {
            document.getElementById('loadingOverlay').style.display = 'flex';
        }

        // Add form submit handler
        document.addEventListener('DOMContentLoaded', function() {
            try {
                var form = document.forms[0];
                if (form) {
                    form.addEventListener('submit', function(e) {
                        showLoading();
                    });
                }
            } catch (e) {
                console.log('Initialization error:', e);
            }
        });
    </script>
</asp:Content>