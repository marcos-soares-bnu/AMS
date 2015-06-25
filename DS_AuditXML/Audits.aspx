<%@ Page Title="Auditorias" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="Audits.aspx.cs" Inherits="DS_AuditXML.Audits" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <br />
    <asp:Panel 
        ID="panExterno" 
        runat="server" 
        Height="75%" 
        ScrollBars="None" 
        Width="100%">
        <asp:Table 
            ID="tabInterno" 
            runat="server" 
            BorderStyle="Ridge" 
            BorderWidth="1px"
            Height="100%" 
            Width="100%">
            <asp:TableRow ID="TableRow0" runat="server">
                <asp:TableCell ID="c1l1" runat="server" Width="30%">
                    <asp:Panel 
                        ID="panMenuInt" 
                        runat="server" 
                        Height="640px" 
                        ScrollBars="None" 
                        Width="300px">
                        <asp:Panel 
                            ID="Panel2" 
                            runat="server" 
                            Height="97%" 
                            ScrollBars="Vertical" 
                            Width="100%">
                            <br />
			    <asp:Label 
				ID="Label1" runat="server" 
				Text="Lista dos Servidores:" Font-Bold="True">
			    </asp:Label>
			    <br />
			    <asp:ListBox 
				ID="lstHost" runat="server" Height="128px" Width="95%" 
				AutoPostBack="True" OnSelectedIndexChanged="lstHost_SelectedIndexChanged" EnableViewState="True">
			    </asp:ListBox>
			    <br />
			    <br />
			    <asp:Label 
				ID="Label2" runat="server" 
				Text="Lista das Classes:" Font-Bold="True">
			    </asp:Label>
			    <a href="http://msdn.microsoft.com/en-us/library/aa394084(v=vs.85).aspx" target="_blank">(?)</a>
			    <br />
			    <asp:ListBox 
				ID="lstTipoClasse" runat="server" Height="128px" Width="95%" 
				AutoPostBack="True" OnSelectedIndexChanged="lstTipoClasse_SelectedIndexChanged" EnableViewState="True">
			    </asp:ListBox>
			    <br />
			    <br />
			    <asp:CheckBox ID="chkCompara" runat="server" 
				oncheckedchanged="chkCompara_CheckedChanged" Text="Comparação [Inicio-Fim]: " AutoPostBack="True" Font-Bold="True" />
			    <asp:Label ID="lblDatas" runat="server" Text="-" ForeColor="White"></asp:Label>
			    <br />
			    <asp:Calendar 
				ID="Calendar1" runat="server" Height="60px" Width="95%"
				onselectionchanged="Calendar1_SelectionChanged" SelectionMode="DayWeekMonth" ShowGridLines="True" >
				<TodayDayStyle BackColor="#CC3300" Font-Bold="True" />
			    </asp:Calendar>
			    <br />
			    <asp:Label ID="Label3" runat="server" Text="Lista das Auditorias:" Font-Bold="True"></asp:Label>
			    <br />
			    <asp:DropDownList ID="drpGeracoes" runat="server" Width="95%" 
				CssClass="footer" ToolTip="Selecione a Geração..." Height="25px">
			    </asp:DropDownList>
                        </asp:Panel>
                    </asp:Panel>
                </asp:TableCell>
                <asp:TableCell ID="c2l1" runat="server" Width="1%">
                    <asp:Panel 
                        ID="panInterno" 
                        runat="server" 
                        Height="640px" 
                        ScrollBars="None" 
                        Width="600px">
                        <asp:Panel 
                            ID="panGrids" 
                            runat="server" 
                            Height="97%" 
                            ScrollBars="Both" 
                            Width="100%">
                            <br />
                            <asp:Label ID="dtSel1" runat="server" Text="-" BackColor="Black" ForeColor="White" Font-Bold="True" Font-Size="Small"></asp:Label>
                            <br />
                            <asp:GridView ID="GridView1" runat="server" CssClass="grid" CellPadding="4" 
                                ForeColor="#333333" GridLines="None" Font-Names="Verdana" Font-Size="8pt" >
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <EditRowStyle BackColor="#999999" />
                                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#000000" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                <SelectedRowStyle BackColor="#E2DED6" ForeColor="#333333" Font-Bold="True" />
                                <sortedascendingcellstyle backcolor="#E9E7E2" />
                                <sortedascendingheaderstyle backcolor="#506C8C" />
                                <sorteddescendingcellstyle backcolor="#FFFDF8" />
                                <sorteddescendingheaderstyle backcolor="#6F8DAE" />
                            </asp:GridView>
                            <br />
                            <asp:Label ID="dtSel2" runat="server" Text="-" BackColor="#1C5E55" ForeColor="White" Font-Bold="True" Font-Size="Small"></asp:Label>
                            <br />
                            <asp:GridView ID="GridView2" runat="server" CssClass="grid" CellPadding="4" 
                                ForeColor="#333333" GridLines="None" Font-Names="Verdana" Font-Size="8pt" >
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <EditRowStyle BackColor="#999999" />
                                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#1C5E55" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                <SelectedRowStyle BackColor="#E2DED6" ForeColor="#333333" Font-Bold="True" />
                                <sortedascendingcellstyle backcolor="#E9E7E2" />
                                <sortedascendingheaderstyle backcolor="#506C8C" />
                                <sorteddescendingcellstyle backcolor="#FFFDF8" />
                                <sorteddescendingheaderstyle backcolor="#6F8DAE" />
                            </asp:GridView>
                            <br />
                            <asp:Label ID="dtSel3" runat="server" Text="-" BackColor="#CCCCCC" ForeColor="#993300" Font-Bold="True" Font-Size="Small"></asp:Label>
                            <asp:TreeView ID="TreeView1" runat="server" 
                                                    Font-Size="Small"
                                                    Font-Names="Courier New" Font-Bold="True">
                                <NodeStyle          ForeColor="#993300"></NodeStyle>
                                <ParentNodeStyle    ForeColor="#993300" />
                                <RootNodeStyle      ForeColor="#993300" BackColor="#CCCCCC"></RootNodeStyle>
                                <SelectedNodeStyle  ForeColor="#993300" BackColor="#CCCCCC"/>
                            </asp:TreeView>
                        </asp:Panel>
                        <asp:Button 
                            CssClass="myButton" ID="btnGeraClasse" runat="server" 
                            Text="Gerar (Servidor\Classe Atual)" onclick="btnGeraClasse_Click" Width="48%" Visible="True" />
                        <asp:Button 
                            CssClass="myButton" ID="btnCompara" runat="server" 
                            Text="Comparar" onclick="btnCompara_Click" Width="48%" Visible="True" />
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <br />
    <asp:Panel ID="panAviso" runat="server" Height="75px" ScrollBars="None" Width="100%" Visible="True">
        <div 
            ID="divMessage" runat="server" >
        </div>
    </asp:Panel>
    <br />
</asp:Content>
