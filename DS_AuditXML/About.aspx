<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="About.aspx.cs" Inherits="DS_AuditXML.About" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <br />
    <h2>
        Contatos - equipe de deploy
    </h2>
    <br />
<asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="List" />
<asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Upd..." />
<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
<br />
<hr />
    <p>
        <asp:Chart ID="Chart1" runat="server">
            <series>
                <asp:Series Name="Series1">
                </asp:Series>
            </series>
            <chartareas>
                <asp:ChartArea Name="ChartArea1">
                </asp:ChartArea>
            </chartareas>
        </asp:Chart>
    </p>
    <p>
        <asp:GridView ID="GridView1" runat="server">
        </asp:GridView>
    </p>
<hr />
<p>
        <strong>E-mail de contato: <a href="mailto:DL_Deploy_AMS" title="Grupo de Email: DL_Deploy_AMS">DL_Deploy_AMS</a>.
        </strong>
    </p>
    <p>
        <strong>Equipe:</strong>
        <br />
            UMPSOAR - Marcos Paulo Soares (634280)
        <br />
            UGBART9 - Guilherme Barth (631282)
        <br />
            UMPALHA - Marcelo Palharim (634214)
        <br />
            UREGAUL - Roger Erivan Gaulke (631223)
        <br />
            UCMKLAN - Carolina Martins Klann (634266)
    </p>
    <p>
        * Grupo Peregrine: <strong>AMS Web Rollout VWdB MAN-LA</strong><br />
        * Para maiores informações, acesse o
        <a href="http://sts237wk8/sites/SDU/Deploy/ProcessoSolicitacaoDeploy.aspx">link</a>!
    </p>
    <br />
</asp:Content>
