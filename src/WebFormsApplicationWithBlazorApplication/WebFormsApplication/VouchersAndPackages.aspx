<%@ Page Title="Vouchers and Packages" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="VouchersAndPackages.aspx.vb" Inherits="WebFormsApplication.VouchersAndPackages" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <p>Select a voucher to view its details in the Blazor application.</p>

    <asp:GridView ID="VouchersGrid" runat="server" AutoGenerateColumns="False" CssClass="table table-striped">
        <Columns>
            <asp:TemplateField HeaderText="Voucher">
                <ItemTemplate>
                    <a href='<%# GetVoucherUrl(Convert.ToInt32(Eval("Id"))) %>'><%# Eval("Name") %></a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Code" HeaderText="Code" />
            <asp:BoundField DataField="Amount" HeaderText="Amount" DataFormatString="{0:C}" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right" />
            <asp:BoundField DataField="Status" HeaderText="Status" />
        </Columns>
    </asp:GridView>
</asp:Content>
