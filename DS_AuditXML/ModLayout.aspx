<%@ Page Title="Modelo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ModLayout.aspx.cs" Inherits="DS_AuditXML.ModLayout" %>
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

                            <div 
                                ID="div1" runat="server" 
                                style="border: thin ridge #333333; background-color: #CCFF99; height: 200px; width: 95%; padding-left: 1px;">
                                <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
                                <br />
                                <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
                                <br />
                                <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
                                <br />
                            </div>
                        </asp:Panel>
                        <asp:Button 
                            CssClass="myButton" ID="Button1" runat="server" 
                            Text="Gerar (Servidor\Classe Atual)" Width="95%" Visible="True" />
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
                            <div 
                                ID="divMain1" runat="server" 
                                style="border: thin ridge #333333; background-color: #99CCFF; height: 500px; width: 95%;">
                            </div>
                            <br />
                            <br />
                            <div 
                                ID="divMain2" runat="server" 
                                style="border: thin ridge #333333; background-color: #CCFF99; height: 500px; width: 95%;">
                            </div>
                            <br />
                            <br />
                            <div 
                                ID="divMain3" runat="server" 
                                style="border: thin ridge #333333; background-color: #FFCC99; height: 500px; width: 95%;">
                            </div>
                        </asp:Panel>
                        <asp:Button 
                            CssClass="myButton" ID="btn1" runat="server" 
                            Text="Gerar (Servidor\Classe Atual)" Width="48%" Visible="True" />
                        <asp:Button 
                            CssClass="myButton" ID="btn2" runat="server" 
                            Text="Comparar" Width="48%" Visible="True" />
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:Panel>
    <br />
    <asp:Panel ID="panAviso" runat="server" Height="75px" ScrollBars="None" Width="100%" Visible="True">
        <div 
            ID="divMessage" runat="server" 
            style="border: thin ridge #333333; background-color: #FF9999; height: 100%; width: 100%;">
        </div>
    </asp:Panel>
    <br />
</asp:Content>
