<%@ Page Title="Relatório Shares" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="Shares.aspx.cs" Inherits="DS_AuditXML.Shares" %>
    
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <br />
    <asp:Table ID="Table1" runat="server" Height="75%" Width="100%" 
        BorderStyle="Ridge" BorderWidth="1px">
        <asp:TableRow ID="TableRow0" runat="server">
            <asp:TableCell ID="lin0_0" runat="server" Width="50%"></asp:TableCell>
            <asp:TableCell ID="lin0_1" runat="server" Width="50%"></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableCell runat="server" Width="50%" ID="lin1_0">
                    <asp:Label ID="Label1" runat="server" Text="Lista dos Servidores Auditados: " Width="50%"></asp:Label>
                    
<asp:DropDownList ID="drpServidores" runat="server" Height="25px" 
                        Width="150px" CssClass="footer" ToolTip="Selecione o Host..." 
                        AutoPostBack="True" ondatabound="drpServidores_DataBound" 
                        ontextchanged="drpServidores_TextChanged">
                    </asp:DropDownList>
            
</asp:TableCell>
            <asp:TableCell runat="server" Width="50%" ID="lin1_1">
                    <asp:Label ID="lblDatas" runat="server" Text="-" ForeColor="White"></asp:Label>
                    
<asp:CheckBox ID="chkCompara" runat="server" 
                        oncheckedchanged="chkCompara_CheckedChanged" Text="Check p/ Comparação!" AutoPostBack="True" />
&nbsp;
                    <asp:Label ID="dtSel1" runat="server" Text="-"></asp:Label>
&nbsp;
                    <asp:Label ID="dtSel2" runat="server" Text="-"></asp:Label>
            
</asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableCell ID="lin2_0" runat="server" Width="50%">
                    <asp:Label ID="Label2" runat="server" Text="Lista das Gerações Auditadas: " Width="50%"></asp:Label>
                    
<asp:DropDownList ID="drpGeracoes" runat="server" Height="25px" 
                        Width="150px" CssClass="footer" ToolTip="Selecione a Geração...">
                    </asp:DropDownList>
            
</asp:TableCell>
            <asp:TableCell ID="lin2_1" runat="server" Width="50%">
                    <asp:Calendar ID="Calendar1" runat="server" Height="152px" 
                        onselectionchanged="Calendar1_SelectionChanged" SelectionMode="DayWeekMonth" 
                        ShowGridLines="True" Width="50%">
                        
<TodayDayStyle BackColor="#CC3300" Font-Bold="True" />
                    
</asp:Calendar>
            
</asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server" ID="TableRow3">
            <asp:TableCell runat="server" Width="50%" ID="lin3_0"></asp:TableCell>
            <asp:TableCell runat="server" Width="50%" ID="lin3_1"></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow runat="server" ID="TableRow4">
            <asp:TableCell runat="server" Width="50%" ID="lin4_0"></asp:TableCell>
            <asp:TableCell runat="server" Width="50%" ID="lin4_1">
                    <asp:Button CssClass="menu" ID="btnVisualiza" runat="server" 
                        Text="Visualizar Informações" BackColor="#506272" ForeColor="#AAAAAE" 
                        onclick="btnVisualiza_Click" Width="50%" />
            
</asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow5" runat="server">
            <asp:TableCell ID="lin5_0" runat="server" Width="50%"></asp:TableCell>
            <asp:TableCell ID="lin5_1" runat="server" Width="50%">
                    <asp:Button CssClass="menu" ID="btnCompara" runat="server" 
                        Text="Comparar Gerações" BackColor="#506272" ForeColor="#AAAAAE" 
                        onclick="btnCompara_Click" Width="50%" />
            
</asp:TableCell>
        </asp:TableRow>
        <asp:TableRow ID="TableRow6" runat="server">
            <asp:TableCell ID="lin6_0" runat="server" Width="50%"></asp:TableCell>
            <asp:TableCell ID="lin6_1" runat="server" Width="50%"></asp:TableCell>
        </asp:TableRow>
    </asp:Table>

</asp:Content>
