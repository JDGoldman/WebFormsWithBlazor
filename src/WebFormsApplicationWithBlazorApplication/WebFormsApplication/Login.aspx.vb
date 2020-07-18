Imports System.Security.Claims
Imports Microsoft.AspNetCore.DataProtection
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Cookies
Public Class Login
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request.QueryString("Logout") IsNot Nothing Then
            Logout()
        End If
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        If tbUserName.Text.Length > 0 And tbPassword.Text.Length > 0 Then 'perform other validation logic as needed
            'FormsAuthentication.Authenticate(tbUserName.Text, tbPassword.Text) has been deprecated
            If Membership.ValidateUser(tbUserName.Text, tbPassword.Text) Then
                Logon(tbUserName.Text)
            Else
                lblMessage.Text = "Credentials were not valid."
            End If
        Else
            lblMessage.Text = "You must enter both a user name and password. "
        End If
    End Sub
    Private Sub Logon(UserName As String)
        Dim lh As New LoginHelper
        Dim persistCookie As Boolean = True

        FormsAuthentication.SetAuthCookie(UserName, persistCookie)
        'lh.LoginForms(UserName, persistCookie, Context)
        lh.LoginClaims(UserName, persistCookie, Context)

        Dim strRedirect As String = Request("ReturnURL")
        If strRedirect = "" Then
            strRedirect = "default.aspx"
        End If

        Response.Redirect(strRedirect, True)

    End Sub

    Private Sub Logout()

        Dim lh As New LoginHelper
        lh.Logout(Context)
        Response.Redirect("~/")
    End Sub

End Class