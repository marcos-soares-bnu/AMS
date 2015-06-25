<%@ Page Title="Agendamentos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" 
    EnableEventValidation="false" CodeBehind="Agendamento.aspx.cs" Inherits="DS_AuditXML.Agendamento" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
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
				ID="lbl1" runat="server" 
				Text="Lista dos Comandos:" Font-Bold="True">
			    </asp:Label>
			    <br />
			    <asp:ListBox 
				ID="lstATcmds" runat="server" Height="128px" Width="95%"
				CausesValidation="True" AutoPostBack="True">
			    </asp:ListBox>
			    <br />
			    <br />
			    <asp:Label ID="Label1" runat="server" Text="Parametros Adicionais:"
				Font-Bold="True" 
				Font-Names="Verdana" Font-Overline="False" Font-Size="9pt" 
				Font-Underline="False" ></asp:Label>
			    <asp:LinkButton ID="lnkHelp" 
				runat="server" 
				onclick="lnkHelp_Click" 
				ToolTip="Clique para Ajuda sobre os Comandos DS_AuditXML_cmd!" Text="(Ver)?">(Ver)?
			    </asp:LinkButton>
			    <br />
			    <asp:TextBox 
				ID="txtParams" runat="server" Height="128px" Width="93%"
				TextMode="MultiLine" >
			    </asp:TextBox>
			    <br />
			    <br />
			    <asp:Label ID="Label5" runat="server" Text="Hora Inicio: (HH:mm)" Width="60%"
				Font-Bold="True" 
				Font-Names="Verdana" Font-Overline="False" Font-Size="9pt" 
				Font-Underline="False" ></asp:Label>
			    <asp:Label ID="Label6" runat="server" Text="Repetir?" Width="20%"
				Font-Bold="True" 
				Font-Names="Verdana" Font-Overline="False" Font-Size="9pt" 
				Font-Underline="False" ></asp:Label>
			    <br />
			    <asp:TextBox ID="txtHoraIni" runat="server" Width="22%" Text="000000000000" Visible="False"></asp:TextBox>
			    <asp:TextBox ID="txtHH" runat="server" Width="21%" Text="00" Font-Size="Medium" Font-Bold="True"></asp:TextBox>
			    <asp:Label ID="Label7" runat="server" Width="5%"
				Font-Bold="True" 
				Font-Names="Verdana" Font-Overline="False" Font-Size="Medium" 
				Font-Underline="False" Text=":"></asp:Label>
			    <asp:TextBox ID="txtmm" runat="server" Width="21%" Text="00" Font-Bold="True" Font-Size="Medium"></asp:TextBox>
			    <asp:Label ID="Label2" runat="server" Width="5%"
				Font-Bold="True" 
				Font-Names="Verdana" Font-Overline="False" Font-Size="Medium" 
				Font-Underline="False" Text=""></asp:Label>
			    <asp:CheckBox ID="chkRepetir" runat="server" Checked="True" Width="20%" 
				Font-Bold="True" Font-Names="Verdana" Font-Size="9pt" />
			    <br />
			    <br />
			    <asp:Label ID="Label8" runat="server" Text="Dias da Semana:" 
				Font-Bold="True" 
				Font-Names="Verdana" Font-Overline="False" Font-Size="9pt" 
				Font-Underline="False" ></asp:Label>
			    <br />
			    <asp:CheckBoxList ID="chkSemana" runat="server" Width="75%"
				RepeatDirection="Horizontal" RepeatLayout="Flow" Font-Bold="False" 
				Font-Names="Verdana" Font-Size="9pt">
				<asp:ListItem Value="MON" Selected="True">Seg</asp:ListItem>
				<asp:ListItem Value="TUE" Selected="True">Ter</asp:ListItem>
				<asp:ListItem Value="WED" Selected="True">Qua</asp:ListItem>
				<asp:ListItem Value="THU" Selected="True">Qui</asp:ListItem>
				<asp:ListItem Value="FRI" Selected="True">Sex</asp:ListItem>
				<asp:ListItem Value="SAT">Sab</asp:ListItem>
				<asp:ListItem Value="SUN">Dom</asp:ListItem>
			    </asp:CheckBoxList>
			    <br />
			    <br />
			    <asp:Label ID="Label9" runat="server" Text="Dias do Mes:"
				Font-Bold="True" 
				Font-Names="Verdana" Font-Overline="False" Font-Size="9pt" 
				Font-Underline="False" ></asp:Label>
			    <br />
			    <asp:CheckBoxList ID="chkDiaMes" runat="server" Width="90%"
				RepeatDirection="Horizontal" RepeatLayout="Flow" Font-Bold="False" 
				Font-Names="Verdana" Font-Size="9pt">
				<asp:ListItem Value="1">01</asp:ListItem>
				<asp:ListItem Value="2">02</asp:ListItem>
				<asp:ListItem Value="4">03</asp:ListItem>
				<asp:ListItem Value="8">04</asp:ListItem>
				<asp:ListItem Value="16">05</asp:ListItem>
				<asp:ListItem Value="32">06</asp:ListItem>
				<asp:ListItem Value="64">07</asp:ListItem>
				<asp:ListItem Value="128">08</asp:ListItem>
				<asp:ListItem Value="256">09</asp:ListItem>
				<asp:ListItem Value="512">10</asp:ListItem>
				<asp:ListItem Value="1024">11</asp:ListItem>
				<asp:ListItem Value="2048">12</asp:ListItem>
				<asp:ListItem Value="4096">13</asp:ListItem>
				<asp:ListItem Value="8192">14</asp:ListItem>
				<asp:ListItem Value="16384">15</asp:ListItem>
				<asp:ListItem Value="32768">16</asp:ListItem>
				<asp:ListItem Value="65536">17</asp:ListItem>
				<asp:ListItem Value="131072">18</asp:ListItem>
				<asp:ListItem Value="262144">19</asp:ListItem>
				<asp:ListItem Value="524288">20</asp:ListItem>
				<asp:ListItem Value="1048576">21</asp:ListItem>
				<asp:ListItem Value="2097152">22</asp:ListItem>
				<asp:ListItem Value="4194304">23</asp:ListItem>
				<asp:ListItem Value="8388608">24</asp:ListItem>
				<asp:ListItem Value="16777216">25</asp:ListItem>
				<asp:ListItem Value="33554432">26</asp:ListItem>
				<asp:ListItem Value="67108864">27</asp:ListItem>
				<asp:ListItem Value="134217728">28</asp:ListItem>
				<asp:ListItem Value="268435456">29</asp:ListItem>
				<asp:ListItem Value="536870912">30</asp:ListItem>
				<asp:ListItem Value="1073741824">31</asp:ListItem>
			    </asp:CheckBoxList>
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
                            <asp:ImageButton ID="imbRefresh" runat="server" ImageUrl="~/Images/upd.ico" 
                                Height="24px" Width="24px" 
                                onclick="imbRefresh_Click" Visible="True" 
                                ToolTip="Recarregar a lista de Tarefas!" />
                            <asp:ImageButton ID="imbAddVazio" runat="server" ImageUrl="~/Images/new.ico" 
                                Height="24px" Width="24px" onclick="imbAddVazio_Click" Visible="True" ToolTip="Clique para Adicionar Nova Tarefa!" />
                            <asp:Label ID="dtSel1" runat="server" Text="-" BackColor="Black" ForeColor="White" Font-Bold="True" Font-Size="Small" Width="50%"></asp:Label>
                            <br />
                            <asp:GridView ID="GridView1" runat="server" 
                                CssClass="grid" CellPadding="4" 
                                ForeColor="#333333" GridLines="None" Font-Names="Verdana" 
                                Font-Size="8pt" 
                                onrowcommand="GridView1_RowCommand" 
                                onrowdatabound="GridView1_RowDataBound" 
                                AutoGenerateColumns="False">
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
                                <Columns>
                                    <asp:TemplateField>
                                      <ItemTemplate>
                                        <asp:ImageButton 
                                                ImageUrl="~/Images/new.ico" 
                                                ID="bntNew" 
                                                runat="server" 
                                                CommandName="New" 
                                                CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                                Visible="False"
                                                ToolTip="Clique para Adicionar Nova Tarefa!" Height="24px" Width="24px" />
                                      </ItemTemplate> 
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                      <ItemTemplate>
                                        <asp:ImageButton 
                                                ImageUrl="~/Images/sel.ico" 
                                                ID="bntVer" 
                                                runat="server" 
                                                CommandName="View" 
                                                CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                                ToolTip="Clique para Visualizar o Conteudo da Tarefa!" Height="24px" Width="24px" />
                                      </ItemTemplate> 
                                    </asp:TemplateField>
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
                                    <asp:TemplateField>
                                      <ItemTemplate>
                                        <asp:ImageButton
                                                ImageUrl="~/Images/exe.ico" 
                                                ID="bntRun" 
                                                runat="server" 
                                                CommandName="Run" 
                                                CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                                ToolTip="Clique para Executar a Tarefa!" Height="24px" Width="24px" />
                                      </ItemTemplate> 
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Nome" HeaderText="Nome">
                                    <ItemStyle Width="400px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Status" HeaderText="Status" />
                                    <asp:BoundField DataField="Disparadores" HeaderText="Disparadores">
                                    <ItemStyle Width="600px" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="Proxima Execucao" HeaderText="Proxima Execucao" />
                                    <asp:BoundField DataField="Ultima Execucao" HeaderText="Ultima Execucao" />
                                    <asp:BoundField DataField="Ultimo Resultado" 
                                        HeaderText="Ultimo Resultado" />
                                    <asp:BoundField DataField="Autor" HeaderText="Autor" />
                                </Columns>
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
                                <SelectedNodeStyle  ForeColor="#993300" />
                            </asp:TreeView>
                            <asp:GridView ID="GridView3" runat="server" 
                                CssClass="grid" CellPadding="4" 
                                ForeColor="#333333" GridLines="None" Font-Names="Verdana" 
                                Font-Size="8pt" 
                                onrowcommand="GridView3_RowCommand" 
                                onrowdatabound="GridView3_RowDataBound" 
                                AutoGenerateColumns="False">
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <EditRowStyle BackColor="#999999" />
                                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#CCCCCC" Font-Bold="True" ForeColor="#993300" />
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
                                    <asp:TemplateField>
                                      <ItemTemplate>
                                        <asp:ImageButton
                                                ImageUrl="~/Images/exe.ico" 
                                                ID="bntRun" 
                                                runat="server" 
                                                CommandName="Run" 
                                                CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                                ToolTip="Clique para Executar a Tarefa!" Height="24px" Width="24px" />
                                      </ItemTemplate> 
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ows_ID" HeaderText="spID" />
                                    <asp:BoundField DataField="ows_Title" HeaderText="Usuário de Acesso" />
                                    <asp:BoundField DataField="ows_Privil_x00e9_gio" HeaderText="Privilégio" />
                                    <asp:BoundField DataField="ows_Servidor" HeaderText="Servidor" />
                                    <asp:BoundField DataField="ows_Analista" HeaderText="Analista" />
                                    <asp:BoundField DataField="ows_Body" HeaderText="Motivo do Acesso" />
                                    <asp:BoundField DataField="ows_StartDate" HeaderText="Início do Acesso" />
                                    <asp:BoundField DataField="ows_DueDate" HeaderText="Fim do Acesso (planejado)" />
                                    <asp:BoundField DataField="ows_Fim_x0020_do_x0020_Acesso_x0020_" HeaderText="Fim do Acesso (real)" />
                                    <asp:BoundField DataField="ows_AssignedTo" HeaderText="Responsável Deploy" />
                                    <asp:BoundField DataField="ows_Author" HeaderText="Criado por" />
                                    <asp:BoundField DataField="ows_Editor" HeaderText="Modificado por" />
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                        <asp:Button 
                            CssClass="myButton" ID="Button1" runat="server" 
                            Text="Button1"  onclick="Button1_Click"  Width="48%" Visible="False" />
                        <asp:Button 
                            CssClass="myButton" ID="Button2" runat="server" 
                            Text="Button2" onclick="Button2_Click"  Width="48%" Visible="False" />
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
