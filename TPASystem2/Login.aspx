

<%@ Page Title="Login" Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="TPASystem2.Login" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Login - TPA HR System</title>
    
    <!-- Materialize CSS -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/materialize/1.0.0/css/materialize.min.css" rel="stylesheet" />
    <!-- Material Icons -->
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    <!-- Google Fonts -->
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    <!-- Custom CSS -->
    <link href="~/Content/css/tpa-common.css" rel="stylesheet" />
    <link href="~/Content/css/tpa-auth.css" rel="stylesheet" />
</head>

<body class="page-login">
    <form id="form1" runat="server">
        <div class="auth-container">
            <div class="auth-card">
                <!-- Left Side - TPA Branding -->
                <div class="brand-side">
                    <div class="tpa-logo-container">
                        <div class="tpa-logo">
                            <div class="tpa-logo-text">TPA</div>
                            <i class="material-icons tpa-logo-icon">people</i>
                        </div>
                        <div class="tpa-name">TENNESSEE<br>PERSONAL<br>ASSISTANCE<br><small>INC</small></div>
                    </div>
                    
                    <h2 class="tpa-full-name">TPA</h2>
                    <h3 class="tpa-subtitle">Tennessee Personal Assistance</h3>
                    
                    <p class="tpa-tagline">
                        Empowering communities through comprehensive personal assistance services across Tennessee
                    </p>
                    
                    <div class="dots-indicator">
                        <div class="dot active"></div>
                        <div class="dot"></div>
                        <div class="dot"></div>
                    </div>
                </div>
                
                <!-- Right Side - Login Form -->
                <div class="form-side">
                    <div class="welcome-header">
                        <div class="welcome-icon">
                            <i class="material-icons">person</i>
                        </div>
                        <h1 class="welcome-title">Welcome Back</h1>
                    </div>
                    <p class="welcome-subtitle">Sign in to your account</p>
                    
                    <!-- Error Message -->
                    <asp:Panel ID="pnlError" runat="server" CssClass="alert alert-error" Visible="false">
                        <i class="material-icons alert-icon">error</i>
                        <div class="alert-content">
                            <asp:Literal ID="litError" runat="server"></asp:Literal>
                        </div>
                    </asp:Panel>
                    
                    <!-- Email Field -->
                    <div class="form-group">
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-input" 
                                   placeholder="Email Address *" type="email" ClientIDMode="Static" 
                                   autocomplete="email" required="true"></asp:TextBox>
                    </div>
                    
                    <!-- Password Field -->
                    <div class="form-group">
                        <div class="password-container">
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-input" 
                                       placeholder="Password *" TextMode="Password" ClientIDMode="Static"
                                       autocomplete="current-password" required="true"></asp:TextBox>
                            <button type="button" class="password-toggle" onclick="togglePassword('txtPassword')">
                                <i class="material-icons" id="passwordIcon">visibility</i>
                            </button>
                        </div>
                    </div>
                    
                    <!-- Remember Me -->
                    <div class="form-group">
                        <label class="remember-me-label">
                            <asp:CheckBox ID="chkRememberMe" runat="server" />
                            <span>Remember me for 30 days</span>
                        </label>
                    </div>
                    
                    <!-- Login Button -->
                    <asp:Button ID="Button1" runat="server" CssClass="btn-tpa" 
                               Text="Sign In to TPA" OnClick="Button1_Click" ClientIDMode="Static" />
                    
                    <!-- Divider -->
                    <div class="form-divider">
                        <span>OR</span>
                    </div>
                    
                    <!-- Additional Options -->
                    <div class="form-links">
                        <a href="#" class="form-link">Forgot your password?</a>
                        <span class="link-separator">•</span>
                        <a href="#" class="form-link">Need help?</a>
                    </div>
                    
                    <!-- Demo Access Info -->
                    <div class="demo-access">
                        <i class="material-icons">info</i>
                        <div class="demo-content">
                            <div class="demo-title">Demo Access Available</div>
                            <div class="demo-subtitle">Contact your administrator for login credentials</div>
                        </div>
                    </div>
                    
                    <!-- Footer -->
                    <div class="auth-footer">
                        <p class="footer-text">
                            © 2025 Tennessee Personal Assistance. All rights reserved.<br>
                            <span class="footer-version">TPA HR System v1.0</span>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </form>
     <script src="~/Content/js/tpa-common.js"></script>

    <!-- Materialize JavaScript -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/materialize/1.0.0/js/materialize.min.js"></script>
    <!-- Custom JavaScript -->
   
</body>
</html>
