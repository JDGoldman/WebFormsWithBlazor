Imports System.Security.Claims
Imports Microsoft.AspNetCore.Authentication
Imports Microsoft.AspNetCore.DataProtection
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Cookies
Public Class LoginHelper
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


        'Add cookie
        ctx.Response.Cookies.Add(ck)

        'Sign in for Owin
        Dim props As New Microsoft.Owin.Security.AuthenticationProperties With {
            .IsPersistent = persistCookie
        }
        ctx.GetOwinContext().Authentication.SignIn(props, identity)

        Return claims

    End Function
End Class
