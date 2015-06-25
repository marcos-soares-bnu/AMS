<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="DS_AuditXML.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


<asp:ListBox ID="listBoxLocation" runat="server" Height="92px" Width="200px" AutoPostBack="True"
OnSelectedIndexChanged="listBoxLocation_SelectedIndexChanged" EnableViewState="True">
</asp:ListBox>


<br />
<br />
<br />
<asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
<br />
<br />
<br />
<br />


</asp:Content>
