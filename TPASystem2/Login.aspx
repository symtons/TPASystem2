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
                        
                            <asp:Image ID="imgLogo" runat="server" ImageUrl="~/Content/logo.png" CssClass="tpa-logo-image" AlternateText="TPA Logo" Width="115px"  />
                       
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
                                <i class="material-icons">visibility</i>
                            </button>
                        </div>
                    </div>
                    
                    <!-- Remember Me & Forgot Password -->
                    <div class="form-options">
                        <label class="remember-me-label">
                            <asp:CheckBox ID="chkRememberMe" runat="server" />
                            Remember me
                        </label>
                        <a href="#" class="form-link">Forgot Password?</a>
                    </div>
                    
                    <!-- Login Button -->
                    <asp:Button ID="btnLogin" runat="server" Text="Sign In" CssClass="btn-tpa" 
                              OnClick="btnLogin_Click" ClientIDMode="Static" />
                    
                    <!-- Additional Options -->
                    <div class="form-footer">
                        <p>Need help? <a href="#" class="form-link">Contact Support</a></p>
                    </div>
                </div>
            </div>
        </div>
    </form>

    <!-- Materialize JavaScript -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/materialize/1.0.0/js/materialize.min.js"></script>
    <!-- Custom JavaScript -->
    <script src="~/Content/js/tpa-common.js"></script>
</body>
</html>