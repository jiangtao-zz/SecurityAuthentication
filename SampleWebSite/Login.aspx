<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="SampleWebSite.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    Login Page with SecurityAuthentication embeded.
    </div>
    
    <iframe src="~/LoginAuthentication.axd?mode=widget_embed"  scrolling="no" 
        frameBorder="no" style="width:400px;height:240px; vertical-align: middle;" 
        align="center"></iframe>
    </form>
</body>
</html>
