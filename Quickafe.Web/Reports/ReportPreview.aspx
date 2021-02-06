<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportPreview.aspx.cs" Inherits="Quickafe.Web.Reports.ReportPreview" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <div style="width: auto;">
        <form id="form1" runat="server" style="width: 100%; height: 100%;">
            <asp:ScriptManager ID="scmScriptManager" runat="server" />
            <div>
                <rsweb:ReportViewer ID="rptReport" runat="server" Font-Names="Verdana" ShowParameterPrompts="false"
                    Font-Size="8pt" ProcessingMode="Remote" SizeToReportContent="true" KeepSessionAlive="true" HyperlinkTarget="_blank"
                    PageCountMode="Actual" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt"
                    InteractivityPostBackMode="AlwaysAsynchronous" AsyncRendering="false"
                    Width="100%" Height="100%">
                </rsweb:ReportViewer>
            </div>
        </form>
    </div>
</body>
</html>
