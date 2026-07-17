Imports System.Configuration

Public Class VouchersAndPackages
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            VouchersGrid.DataSource = GetVouchers()
            VouchersGrid.DataBind()
        End If
    End Sub

    Protected Function GetVoucherUrl(voucherId As Integer) As String
        Dim blazorAppBaseUrl = ConfigurationManager.AppSettings("BlazorAppBaseUrl").TrimEnd("/"c)
        Return $"{blazorAppBaseUrl}/voucher/{voucherId}"
    End Function

    Private Shared Function GetVouchers() As List(Of VoucherSummary)
        Return New List(Of VoucherSummary) From {
            New VoucherSummary With {.Id = 1, .Name = "Spring Promotion", .Code = "SPRING2026", .Amount = 50D, .Status = "Active"},
            New VoucherSummary With {.Id = 2, .Name = "Loyalty Reward", .Code = "LOYAL100", .Amount = 100D, .Status = "Active"},
            New VoucherSummary With {.Id = 3, .Name = "Holiday Package", .Code = "HOLIDAY25", .Amount = 25D, .Status = "Redeemed"}
        }
    End Function

    Private Class VoucherSummary
        Public Property Id As Integer
        Public Property Name As String
        Public Property Code As String
        Public Property Amount As Decimal
        Public Property Status As String
    End Class
End Class
