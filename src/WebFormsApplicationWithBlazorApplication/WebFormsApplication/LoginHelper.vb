﻿Imports System.Security.Claims
Imports Microsoft.AspNetCore.Authentication
Imports Microsoft.AspNetCore.DataProtection
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Cookies
Public Class LoginHelper
    'Public Sub LoginForms(UserName As String, persistCookie As Boolean, ctx As HttpContext)
    '    Dim tkt As FormsAuthenticationTicket
    '    Dim cookiestr As String
    '    Dim ck As HttpCookie

    '    tkt = New FormsAuthenticationTicket(1, UserName, System.DateTime.Now(), System.DateTime.Now.AddMinutes(30), persistCookie, "", "/")
    '    cookiestr = FormsAuthentication.Encrypt(tkt)

    '    ck = New HttpCookie(FormsAuthentication.FormsCookieName(), cookiestr)
    '    If persistCookie Then
    '        ck.Expires = tkt.Expiration
    '    End If

    '    ck.Path = FormsAuthentication.FormsCookiePath()
    '    ctx.Response.Cookies.Add(ck)

    'End Sub
    Public Function LoginClaims(UserName As String, persistCookie As Boolean, ctx As System.Web.HttpContext) As List(Of Claim)
        'create Identity from the Membership User, Roles
        Dim user = Membership.GetUser(UserName)
        Dim claims = New List(Of Claim)()
        claims.Add(New Claim(ClaimTypes.Name, UserName))
        claims.Add(New Claim(ClaimTypes.NameIdentifier, user.ProviderUserKey.ToString))

        If Roles.Enabled Then
            For Each role In Roles.GetRolesForUser(UserName)
                claims.Add(New Claim(ClaimTypes.Role, role))
            Next
        End If

        Dim identity = New ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType)
        Dim principal = New ClaimsPrincipal(identity)

        'get configuration settings
        Dim location = ConfigurationManager.AppSettings("SharedKeyFileLocation")
        Dim application_name = ConfigurationManager.AppSettings("ApplicationName")
        Dim cookie_name = ConfigurationManager.AppSettings("CookieName")
        Dim domain_name = ConfigurationManager.AppSettings("DomainName")

        'create data protector used by both apps
        Dim provider = DataProtectionProvider.Create(New System.IO.DirectoryInfo(location),
                Function(Builder) {Builder.SetApplicationName(application_name)})
        Dim protector = provider.CreateProtector(
                "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
                cookie_name,
                "v2")

        'use data protector to protect ticket
        Dim ticketFormat = New Microsoft.AspNetCore.Authentication.TicketDataFormat(protector)
        Dim tkt = New Microsoft.AspNetCore.Authentication.AuthenticationTicket(principal, "Cookies")
        Dim protectedTkt = ticketFormat.Protect(tkt)


        Dim ck As HttpCookie = New HttpCookie(cookie_name, protectedTkt) With {
            .Domain = domain_name,
            .Path = "/",
            .Expires = Date.Now.AddMinutes(60),
            .HttpOnly = False
        }

        'Add shared cookie
        ctx.Response.Cookies.Add(ck)

        'Sign in for Owin if needed
        Dim props As New Microsoft.Owin.Security.AuthenticationProperties With {
            .IsPersistent = persistCookie
        }
        ctx.GetOwinContext().Authentication.SignIn(props, identity)

        Return claims

    End Function

    Public Sub Logout(ctx As HttpContext)
        ctx.GetOwinContext.Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType)
        FormsAuthentication.SignOut()
        Dim cookie_name = ConfigurationManager.AppSettings("CookieName")

        Dim cookie As HttpCookie
        If ctx.Response.Cookies(cookie_name) IsNot Nothing Then
            cookie = ctx.Response.Cookies(cookie_name)
        ElseIf ctx.Request.Cookies(cookie_name) IsNot Nothing Then
            cookie = ctx.Request.Cookies(cookie_name)
        Else
            cookie = New HttpCookie(cookie_name)
        End If

        cookie.Expires = DateTime.Now.AddMinutes(-1)
        ctx.Response.Cookies.Set(cookie)
        ctx.Request.Cookies.Remove(cookie_name)
        ctx.Request.Cookies.Clear()

    End Sub
End Class
