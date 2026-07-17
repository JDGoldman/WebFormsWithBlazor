<%@ Page Title="Returns To Vendor" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ReturnsToVendor.aspx.vb" Inherits="WebFormsApplication.ReturnsToVendor" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %></h2>
    <p>Select a return to vendor record to manage linked files in the Blazor application.</p>

    <asp:GridView ID="RtvsGrid" runat="server" AutoGenerateColumns="False" CssClass="table table-striped">
        <Columns>
            <asp:TemplateField HeaderText="Vendor">
                <ItemTemplate>
                    <a href='<%# GetRtvUrl(Convert.ToInt32(Eval("Id"))) %>'><%# Eval("VendorName") %></a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="ReferenceNumber" HeaderText="Reference" />
            <asp:BoundField DataField="Status" HeaderText="Status" />
            <asp:BoundField DataField="CreatedDate" HeaderText="Created" DataFormatString="{0:d}" />
        </Columns>
    </asp:GridView>
</asp:Content>
