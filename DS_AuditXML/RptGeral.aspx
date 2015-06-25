<%@ Page Title="Relatórios" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    CodeBehind="RptGeral.aspx.cs" Inherits="DS_AuditXML.RptGeral" %>
<asp:Content ID="HeaderContent" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
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
				            ID="Label3" runat="server" 
				            Text="Lista dos Servidores: " Font-Bold="True">
			                </asp:Label>
			                <asp:CheckBox 
				            ID="chkSrv" runat="server" Text="Ativos" 
				            AutoPostBack="True" oncheckedchanged="chkSrv_CheckedChanged" />
			                <br />
			                <asp:ListBox 
				            ID="lstHost" runat="server" Height="128px" Width="95%" 
				            EnableViewState="True" AutoPostBack="True" SelectionMode="Multiple" 
				            OnSelectedIndexChanged="lstHost_SelectedIndexChanged" >
			                </asp:ListBox>
			                <br />
			                <br />
			                <asp:Label 
				            ID="Label4" runat="server" 
				            Text="Lista das Classes: " Font-Bold="True">
			                </asp:Label>
			                <asp:CheckBox ID="chkCls" runat="server" Text="Ativos" 
				            AutoPostBack="True" oncheckedchanged="chkCls_CheckedChanged" />
			                <br />
			                <asp:ListBox 
				            ID="lstTipoClasse" runat="server" Height="128px" Width="95%" 
				            EnableViewState="True" AutoPostBack="True" SelectionMode="Multiple"
				            OnSelectedIndexChanged="lstTipoClasse_SelectedIndexChanged" >
			                </asp:ListBox>
			                <br />
			                <br />
			                <asp:CheckBox ID="chkCompara" runat="server" 
				            oncheckedchanged="chkCompara_CheckedChanged" Text="Intervalo [Inicio-Fim]: " AutoPostBack="True" Font-Bold="True" Checked="True" Enabled="True" />
			                <asp:Label ID="lblDatas" runat="server" Text="-" ForeColor="White"></asp:Label>
			                <br />
			                <asp:Calendar 
				            ID="Calendar1" runat="server" Height="60px" Width="95%"
				            onselectionchanged="Calendar1_SelectionChanged" SelectionMode="DayWeekMonth" ShowGridLines="True" >
				            <TodayDayStyle BackColor="#CC3300" Font-Bold="True" />
			                </asp:Calendar>
			                <br />
			                <br />
			                <asp:CheckBox ID="chkSemanal" runat="server" 
				            oncheckedchanged="chkSemanal_CheckedChanged" Text="Geração Semanal!" Font-Bold="True" AutoPostBack="True" Enabled="False" />
                            <br />
                            <asp:Label ID="lbl_txtDtIni" runat="server" Text="" Visible="True"></asp:Label>
                            <br />
                            <asp:Label ID="lbl_txtDtFim" runat="server" Text="" Visible="True"></asp:Label>
                        </asp:Panel>
		    	        <asp:Button CssClass="myButton" ID="btnFiltrar" runat="server" 
				        Text="Filtrar" onclick="btnFiltrar_Click" Width="88%" />
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
                            <asp:Label ID="dtSel1" runat="server" Text="-" BackColor="Black" ForeColor="White" Font-Bold="True" Font-Size="Small" Width="50%"></asp:Label>
                            <br />
                            <asp:GridView ID="grdRptGeral" runat="server"
                                CssClass="grid" CellPadding="4" 
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
                            <br />
                            <asp:Label ID="dtSel2" runat="server" Text="-" BackColor="#1C5E55" ForeColor="White" Font-Bold="True" Font-Size="Small"></asp:Label>
                            <br />
                            <asp:GridView ID="GridView2" runat="server" 
                                CssClass="grid" CellPadding="4" 
                                ForeColor="#333333" GridLines="None" Font-Names="Verdana" 
                                Font-Size="8pt" 
                                onrowcommand="grdRptGeral_RowCommand" 
                                onrowdatabound="grdRptGeral_RowDataBound" 
                                AutoGenerateColumns="False">
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
                                <Columns>
                                    <asp:TemplateField>
                                      <ItemTemplate>
                                        <asp:ImageButton
                                                ImageUrl="~/Images/del.ico" 
                                                ID="bntDel" 
                                                runat="server" 
                                                CommandName="Del" 
                                                CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                                ToolTip="Clique para Deletar a Tarefa!" Height="24px" Width="24px" />
                                      </ItemTemplate> 
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ows_ID" HeaderText="spID" />
                                    <asp:BoundField DataField="ows_Title" HeaderText="Auditoria" />
                                    <asp:BoundField DataField="ows_Semana" HeaderText="Intervalo" />
                                    <asp:BoundField DataField="ows_Servidor" HeaderText="Servidor" />
                                    <asp:BoundField DataField="ows_Compara_Result" HeaderText="Comparação" />
                                    <asp:BoundField DataField="ows_Acao" HeaderText="Ação" />
                                    <asp:BoundField DataField="ows_Created" HeaderText="Criado" />
                                    <asp:BoundField DataField="ows_Modified" HeaderText="Modificado" />
                                    <asp:BoundField DataField="ows_Author" HeaderText="Criado por" />
                                    <asp:BoundField DataField="ows_Editor" HeaderText="Modificado por" />
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                        <asp:Button 
                            CssClass="myButton" ID="btnGerar" runat="server" 
                            Text="Gerar Relatório" onclick="btnGerar_Click" Width="32%" Visible="True" />
                        <asp:Button 
                            CssClass="myButton" ID="btnExportarXLS" runat="server" 
                            Text="Visualizar Relatório" onclick="btnExportarXLS_Click" Width="32%" Visible="True" />
                        <asp:Button 
                            CssClass="myButton" ID="btnExportarSP" runat="server" 
                            Text="Salvar Portal SDC" onclick="btnExportarSP_Click" Width="32%" Visible="True" />
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
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
</asp:Content>
