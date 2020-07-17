<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Login.aspx.vb" Inherits="WebFormsApplication.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    User name: <asp:TextBox ID="tbUserName" runat="server"></asp:TextBox><br />
    Password: <asp:TextBox ID="tbPassword" runat="server" TextMode="Password"></asp:TextBox>
    <asp:Button ID="btnSubmit" runat="server" Text="Submit" /><br />
    <asp:Label ID="lblMessage" runat="server"></asp:Label>
    <p>It ain't pretty, but it works.</p>
</asp:Content>
