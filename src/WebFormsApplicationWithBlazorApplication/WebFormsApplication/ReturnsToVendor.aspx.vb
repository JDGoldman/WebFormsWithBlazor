Imports System.Configuration

Public Class ReturnsToVendor
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            RtvsGrid.DataSource = GetRtvs()
            RtvsGrid.DataBind()
        End If
    End Sub

    Protected Function GetRtvUrl(rtvId As Integer) As String
        Dim blazorAppBaseUrl = ConfigurationManager.AppSettings("BlazorAppBaseUrl").TrimEnd("/"c)
        Return $"{blazorAppBaseUrl}/rtv/{rtvId}"
    End Function

    Private Shared Function GetRtvs() As List(Of RtvSummary)
        Return New List(Of RtvSummary) From {
            New RtvSummary With {.Id = 1, .VendorName = "Acme Supplies", .ReferenceNumber = "RTV-2026-001", .Status = "Open", .CreatedDate = New Date(2026, 7, 10)},
            New RtvSummary With {.Id = 2, .VendorName = "Northern Textiles", .ReferenceNumber = "RTV-2026-002", .Status = "Pending Approval", .CreatedDate = New Date(2026, 7, 12)},
            New RtvSummary With {.Id = 3, .VendorName = "City Wholesale", .ReferenceNumber = "RTV-2026-003", .Status = "Closed", .CreatedDate = New Date(2026, 6, 28)}
        }
    End Function

    Private Class RtvSummary
        Public Property Id As Integer
        Public Property VendorName As String
        Public Property ReferenceNumber As String
        Public Property Status As String
        Public Property CreatedDate As Date
    End Class
End Class
