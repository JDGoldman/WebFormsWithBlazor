Imports Owin
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNetCore.DataProtection
Imports Microsoft.Owin
Imports Microsoft.Owin.Security.Interop
Imports Microsoft.Owin.Security.Cookies
Imports System.Threading.Tasks
Imports ChunkingCookieManager = Microsoft.Owin.Security.Interop.ChunkingCookieManager

Partial Public Class Startup
    Public Sub ConfigureAuth(app As IAppBuilder)
        Dim location = ConfigurationManager.AppSettings("SharedKeyFileLocation")
        Dim cookie_name = ConfigurationManager.AppSettings("CookieName")
        Dim domain_name = ConfigurationManager.AppSettings("DomainName")
        Dim application_name = ConfigurationManager.AppSettings("ApplicationName")
        app.UseCookieAuthentication(New CookieAuthenticationOptions() With {
            .AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            .AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
            .CookieDomain = domain_name,
            .CookieHttpOnly = False,
            .CookieName = cookie_name,
            .CookieSecure = CookieSecureOption.SameAsRequest,
            .LoginPath = New PathString("/Login.aspx"),
            .Provider = New CookieAuthenticationProvider() With {
                .OnValidateIdentity = Function(ctx)
                                          Dim ret = Task.Run(Function()
                                                                 Dim userName As String = ""
                                                                 If HttpContext.Current IsNot Nothing Then
                                                                     userName = System.Web.HttpContext.Current.User.Identity.Name
                                                                     Dim context As System.Web.HttpContext = HttpContext.Current
                                                                     Dim lh As New LoginHelper
                                                                     Dim claims = lh.LoginClaims(userName, True, context)
                                                                     Return claims
                                                                 Else
                                                                     Return Task.FromResult(0)
                                                                 End If
                                                             End Function)
                                          Return ret
                                      End Function
               },
        .TicketDataFormat = New AspNetTicketDataFormat(
             New DataProtectorShim(
                DataProtectionProvider.Create(New System.IO.DirectoryInfo(location),
                Function(Builder) {Builder.SetApplicationName(application_name)}).CreateProtector(
                "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
                cookie_name,
                "v2"))),
                .CookieManager = New ChunkingCookieManager()
            })

        System.Web.Helpers.AntiForgeryConfig.UniqueClaimTypeIdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"


    End Sub

End Class
