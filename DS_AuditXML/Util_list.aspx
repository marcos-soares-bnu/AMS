<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Util_list.aspx.cs" 
    Inherits="DS_AuditXML.Util_list" enableEventValidation="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <br />
        <asp:Panel ID="panGrids" runat="server" Height="500px" ScrollBars="Auto">
            <asp:Button ID="Button1" runat="server" BackColor="#000000" BorderStyle="None" 
                Font-Bold="True" Font-Names="Verdana" ForeColor="White" OnClick="Button1_Click" 
                Text="Relatório - " Width="100%" />
            <asp:GridView ID="GridView1" runat="server" CellPadding="4" CssClass="grid" 
                Font-Names="Verdana" Font-Size="8pt" ForeColor="#333333" GridLines="None">
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                <EditRowStyle BackColor="#999999" />
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#000000" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <sortedascendingcellstyle backcolor="#E9E7E2" />
                <sortedascendingheaderstyle backcolor="#506C8C" />
                <sorteddescendingcellstyle backcolor="#FFFDF8" />
                <sorteddescendingheaderstyle backcolor="#6F8DAE" />
            </asp:GridView>
            <br />
            <asp:Button ID="Button2" runat="server" BackColor="#1C5E55" BorderStyle="None" 
                Font-Bold="True" Font-Names="Verdana" ForeColor="White" OnClick="Button1_Click" 
                Text="Relatório - " Visible="False" Width="100%" />
            <asp:GridView ID="GridView2" runat="server" CellPadding="4" CssClass="grid" 
                Font-Names="Verdana" Font-Size="8pt" ForeColor="#333333" GridLines="None" 
                Visible="False">
                <AlternatingRowStyle BackColor="White" />
                <EditRowStyle BackColor="#7C6F57" />
                <FooterStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#666666" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#E3EAEB" />
                <SelectedRowStyle BackColor="#C5BBAF" Font-Bold="True" ForeColor="#333333" />
                <sortedascendingcellstyle backcolor="#F8FAFA" />
                <sortedascendingheaderstyle backcolor="#246B61" />
                <sorteddescendingcellstyle backcolor="#D4DFE1" />
                <sorteddescendingheaderstyle backcolor="#15524A" />
            </asp:GridView>
            <br />
        </asp:Panel>
        <br />
        <asp:Panel ID="panAviso" runat="server" Height="100%" ScrollBars="Vertical" Width="100%" Visible="True">
            <div 
                ID="divErro" runat="server" >
            </div>
            <div 
                ID="divMessage" runat="server" >
            </div>
        </asp:Panel>
        <br />
        <asp:Label ID="ehfull" runat="server" 
                    ForeColor="White" Text="0">
        </asp:Label>
        <asp:Label ID="Label1" runat="server" Text="Legenda:"
                    Font-Bold="True" 
                    Font-Names="Verdana" Font-Overline="False" Font-Size="9pt" 
                    Font-Underline="True">
        </asp:Label>
        &nbsp;
        <asp:Label ID="Label2" runat="server" Text="[Diferentes] " 
                    BackColor="#C4E1FF" Font-Names="Verdana" Font-Size="8pt">
        </asp:Label>
        &nbsp;
        <asp:Label ID="Label3" runat="server" Text=" [Equivalentes] " BackColor="#66FF99" 
                    Font-Names="Verdana" Font-Size="8pt">
        </asp:Label>
        &nbsp;
        <asp:Label ID="Label4" runat="server" Text="** Clique nos títulos acima, para trocar entre a visão total e das diferenças." 
                    Font-Names="Verdana" Font-Size="8pt">
        </asp:Label>
    </form>
</body>
</html>
