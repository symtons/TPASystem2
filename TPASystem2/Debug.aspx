

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Debug.aspx.cs" Inherits="TPASystem2.Debug" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Debug - Routing Test</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 20px; background: #f5f5f5; }
        .container { background: white; padding: 20px; border-radius: 5px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
        .test-item { margin: 10px 0; padding: 10px; background: #e8f4fd; border-radius: 3px; }
        .success { background: #d4edda; color: #155724; }
        .error { background: #f8d7da; color: #721c24; }
        .link { display: inline-block; margin: 5px; padding: 8px 15px; background: #007bff; color: white; text-decoration: none; border-radius: 3px; }
        .link:hover { background: #0056b3; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <h1>🔧 TPA HR System - Debug Page</h1>
            
            <h2>Routing Test</h2>
            <div class="test-item">
                <strong>Current URL:</strong> <asp:Literal ID="litCurrentUrl" runat="server"></asp:Literal>
            </div>
            
            <div class="test-item">
                <strong>Physical Path:</strong> <asp:Literal ID="litPhysicalPath" runat="server"></asp:Literal>
            </div>
            
            <div class="test-item">
                <strong>Application Start Time:</strong> <asp:Literal ID="litAppStartTime" runat="server"></asp:Literal>
            </div>
            
            <div class="test-item">
                <strong>HR Employees File Exists:</strong> <asp:Literal ID="litEmployeesExists" runat="server"></asp:Literal>
            </div>
            
            <h2>Route Testing Links</h2>
            <div>
                <a href="/dashboard" class="link">Dashboard</a>
                <a href="/employees" class="link">Employees (Should work now)</a>
                <a href="/HR/Employees.aspx" class="link">Direct HR/Employees.aspx</a>
                <a href="/login" class="link">Login</a>
                <a href="/nonexistent" class="link">Non-existent (Should go to NotFound)</a>
            </div>
            
            <h2>Route Information</h2>
            <div class="test-item">
                <asp:Literal ID="litRouteInfo" runat="server"></asp:Literal>
            </div>
            
            <h2>Application State</h2>
            <div class="test-item">
                <asp:Literal ID="litAppState" runat="server"></asp:Literal>
            </div>
            
        </div>
    </form>
</body>
</html>