<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NotFound.aspx.cs" Inherits="TPASystem2.NotFound" %>



<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Page Not Found - TPA HR System</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <style>
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            margin: 0;
            padding: 0;
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        .error-container {
            background: white;
            border-radius: 10px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
            padding: 40px;
            text-align: center;
            max-width: 500px;
            width: 90%;
        }
        
        .error-code {
            font-size: 6em;
            font-weight: bold;
            color: #667eea;
            margin: 0;
            line-height: 1;
        }
        
        .error-title {
            font-size: 2em;
            color: #333;
            margin: 20px 0 10px 0;
        }
        
        .error-message {
            color: #666;
            font-size: 1.1em;
            margin-bottom: 30px;
            line-height: 1.5;
        }
        
        .action-buttons {
            display: flex;
            gap: 15px;
            justify-content: center;
            flex-wrap: wrap;
        }
        
        .btn {
            background: #667eea;
            color: white;
            border: none;
            padding: 12px 24px;
            border-radius: 5px;
            text-decoration: none;
            font-size: 1em;
            cursor: pointer;
            transition: background-color 0.3s;
        }
        
        .btn:hover {
            background: #5a67d8;
        }
        
        .btn-secondary {
            background: #718096;
        }
        
        .btn-secondary:hover {
            background: #4a5568;
        }
        
        .search-box {
            margin: 20px 0;
            padding: 0;
        }
        
        .search-input {
            width: 100%;
            padding: 12px;
            border: 2px solid #e2e8f0;
            border-radius: 5px;
            font-size: 1em;
            box-sizing: border-box;
        }
        
        .search-input:focus {
            outline: none;
            border-color: #667eea;
        }
        
        @media (max-width: 600px) {
            .error-code {
                font-size: 4em;
            }
            
            .error-title {
                font-size: 1.5em;
            }
            
            .action-buttons {
                flex-direction: column;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="error-container">
            <div class="error-code">404</div>
            <h1 class="error-title">Page Not Found</h1>
            <p class="error-message">
                The page you're looking for doesn't exist or has been moved.
                <br />Let's get you back on track!
            </p>
            
            <div class="search-box">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" 
                             placeholder="Search for pages or employees..." />
            </div>
            
            <div class="action-buttons">
                <asp:Button ID="btnSearch" runat="server" Text="Search" 
                           CssClass="btn" OnClick="btnSearch_Click" />
                <a href="/dashboard" class="btn">Go to Dashboard</a>
                <a href="/employees" class="btn btn-secondary">View Employees</a>
            </div>
        </div>
    </form>
</body>
</html>