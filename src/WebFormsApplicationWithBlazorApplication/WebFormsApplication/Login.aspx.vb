Imports System.Security.Claims
Imports Microsoft.AspNetCore.DataProtection
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Cookies
Public Class Login
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'If Not Page.IsPostBack Then
        If Request.QueryString("Logout") IsNot Nothing Then
            Logout()
        End If
        'End If
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        If ValidateUser(tbUserName.Text, tbPassword.Text) Then
            Logon(tbUserName.Text)
        End If
    End Sub
    Private Function ValidateUser(UserName As String, Password As String) As Boolean
        Return True
    End Function
    Private Sub Logon(UserName As String)
        Dim tkt As FormsAuthenticationTicket
        Dim cookiestr As String
        Dim ck As HttpCookie
        Dim persistCookie As Boolean = True

        tkt = New FormsAuthenticationTicket(1, UserName, System.DateTime.Now(), System.DateTime.Now.AddMinutes(30), persistCookie, "", "/")
        cookiestr = FormsAuthentication.Encrypt(tkt)

        ck = New HttpCookie(FormsAuthentication.FormsCookieName(), cookiestr)
        If persistCookie Then
            ck.Expires = tkt.Expiration
            ck.Path = FormsAuthentication.FormsCookiePath()
            Response.Cookies.Add(ck)
            Dim lh As New LoginHelper
            lh.LoginClaims(UserName, persistCookie, Context)

            Dim strRedirect As String = Request("ReturnURL")
            If strRedirect = "" Then
                strRedirect = "default.aspx"
            End If

            Response.Redirect(strRedirect, True)

        End If


    End Sub

    Private Sub Logout()
        Context.GetOwinContext.Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType)
        FormsAuthentication.SignOut()
        Dim cookie_name = ConfigurationManager.AppSettings("CookieName")

        Dim cookie As HttpCookie
        If Response.Cookies(cookie_name) IsNot Nothing Then
            cookie = Response.Cookies(cookie_name)
        ElseIf Request.Cookies(cookie_name) IsNot Nothing Then
            cookie = Request.Cookies(cookie_name)
        Else
            cookie = New HttpCookie(cookie_name)
        End If

        cookie.Expires = DateTime.Now.AddMinutes(-1)
        HttpContext.Current.Response.Cookies.Set(cookie)
        HttpContext.Current.Request.Cookies.Remove(cookie_name)
        HttpContext.Current.Request.Cookies.Clear()


        Response.Redirect("~/")
    End Sub

End Class