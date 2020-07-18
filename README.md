# Web Forms Authentication With Blazor Application
This repository is meant to demonstrate how to integrate authentication from a ASP.NET Forms Authentication application with a separate Blazor application. 

## Background
I have an old VB.NET Web Forms application using Forms Authentication using the old aspnetdb SQL Server Database. I have been wanting to do new development using Blazor, but I didn't want my users to have to log on to both applications separately. The solution to the problem, in a nutshell, was to create a cookie which could be shared between the two applications which contains a ClaimsPrincipal object.  

It took me several weeks to figure out how to do this. Surprisingly, I could find almost no documentation on what I assume to be a pretty considerable use case.  The help documentation on the Microsoft website was, at best, a starting point. I have created this repository so that hopefully I can save somebody else this time.  

The branches of this project are the various stages of integration of the two technologies:  

1. Master is a new ASP.NET Web Forms project created by the Visual Studio wizard (note, no authentication added by the wizard).  
2. Step 1 is the modified ASP.NET Web Forms project to include all of the code and configuration changes to work with the Blazor App (prior to any subsequent modifications I have made to the current branch (Step 3)).
3. Step 2 is the new Blazor Server application created by the Visual Studio wizard (note, no authentication added by the wizard).
4. Step 3 is the modified Blazor Server application to run with Forms Authentication from the Web Forms project.

**Several Notes**: 
1.  All authentication is handled by the Web Forms application.  
2.  If you are running this through Visual Studio/IIS Express, you will need to make sure both websites are running.  
3.  The Blazor app may need to be refreshed after logging in or out (something I will work on to avoid).
4.  This solution will not work on a server farm because it uses a file location to manage a shared key between the applications.
If you are interested in a server farm solution, this article provides some guidance:
https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-3.1


## Database Setup

The premise of this application is that you already have an old, Web Forms Forms Authentication application setup,
so you shouldn't need to create a new database.  You can just use your existing one. No changes are made to your database by this application. 
However, if you want to create a test aspnetdb, roll your own or follow the steps below:

1. To configure your Sql Server edition for Forms Authentication, you need to run the Sql Server Registration Tool 
located at %WINDIR%\Microsoft.Net\Framework\<.net version>\aspnet_regsql.exe -W
The "-W" flag causes the command prompt to launch a wizard which will allow you to 
select the Sql Server and the database name.  For this project, I selected aspnetdb_test.
2. Add roles and users to the Forms Authentication db.  I did this manually, which, I know, is a pain.  I will generate a script at a later point.

You will need to modify your web.config file in three spots to use your own "Application Name" if you are using an existing aspnetdb. You will also need to modify the appsettings.json file in the Blazor application accordingly.  
1. In the &lt;AppSettings&gt; Key for Application Name.
2. In the Membership Provider Key for &lt;applicationName&gt;.
3. In the Role Manager Key for &lt;applicationName&gt;.  

## Modifications to Web Forms Application 

### 1. Add logic to Web Forms application to create Claims from Membership User.  

The first step is to create a Claims object for the authenticated user from the existing Membership User.  The Claims object will be used in the next step to create a ClaimsPrincipal which is the Authorization/Authentication format needed by the Blazor application.

```html
        Dim user = Membership.GetUser(UserName)
        Dim claims = New List(Of Claim)()
        claims.Add(New Claim(ClaimTypes.Name, UserName))
        claims.Add(New Claim(ClaimTypes.NameIdentifier, user.ProviderUserKey.ToString))

        If Roles.Enabled Then
            For Each role In Roles.GetRolesForUser(UserName)
                claims.Add(New Claim(ClaimTypes.Role, role))
            Next
        End If
```

### 2. Create a ClaimsPrincipal object
Taking the Claims created in step 1 above, create a new ClaimsPrincipal which can be used by the Blazor App:
```html
       Dim identity = New ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType)
       Dim principal = New ClaimsPrincipal(identity)
```
### 3. Create a DataProtector
Create a DataProtector which will be used by both applications.  You need to set the ApplicationName for both applications to the same value, and you need to use the same values in both applications for the CreateProtector logic.  

```html
        Dim provider = DataProtectionProvider.Create(New System.IO.DirectoryInfo(location),
                Function(Builder) {Builder.SetApplicationName(application_name)})
        Dim protector = provider.CreateProtector(
                "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
                cookie_name,
                "v2")
```

### 4. Create an AuthenticationTicket
Use this DataProtector to create an AuthenticationTicket which can be consumed by the ASP.NET Core (Blazor) application.
```html
        'use data protector to protect ticket
        Dim ticketFormat = New Microsoft.AspNetCore.Authentication.TicketDataFormat(protector)
        Dim tkt = New Microsoft.AspNetCore.Authentication.AuthenticationTicket(principal, "Cookies")
        Dim protectedTkt = ticketFormat.Protect(tkt)
```
### 5. Save the AuthenticationTicket in a Cookie
Save the newly created AuthenticationTicket in an HttpCookie to be shared by both applications.
```html
        Dim ck As HttpCookie = New HttpCookie(cookie_name, protectedTkt) With {
            .Domain = domain_name,
            .Path = "/",
            .Expires = Date.Now.AddMinutes(60),
            .HttpOnly = False
        }

        'Add shared cookie
        ctx.Response.Cookies.Add(ck)
```

### 6. Make Corresponding Changes in the Owin Startup Class

You will need to make changes to the Owin Startup class (in the ConfigAuth subroutine) to add the same functionality as in the LoginHelper.vb class.  
```html
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
                                                                 If HttpContext.Current IsNot Nothing Then
                                                                     Dim userName As String = ""
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

```

## Modifications to Blazor Application

### 1. Add services to Startup

We will need to access the HttpContext for purposes of retrieving the shared cookie value, so we need to add the following code to our Startup services:
```html
            services.AddHttpContextAccessor();
            services.AddScoped<HttpContextAccessor>();
            services.AddHttpClient();
            services.AddScoped<HttpClient>();
```

You will need to add the same DataProtector to your Startup class:
```html
            services.AddDataProtection()
                .ProtectKeysWithDpapi().DisableAutomaticKeyGeneration()
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(shared_key_location))
                .SetApplicationName(application_name);
```
And add Authetication/Authorization as well:
```html
           services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = cookie_name;
                    options.Cookie.Path = "/";
                    options.Cookie.Domain = domain_name;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.HttpOnly = false;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.IsEssential = true;
                }
                );
            services.AddAuthorization();
```
### 2. Add new AuthenticationStateProvider

We need to create a custom AuthenticationStateProvider to override the default AuthenticationStateProvider. I have done this in the CustomAuthStateProviderClass:
```html
        public class CustomAuthStateProvider : AuthenticationStateProvider
```
The logic all occurs in the GetAuthenticationStateAsync function where we retrive to cookie value, unprotect it using the same DataProtector configured the same way, and then convert it into a ClaimsPrincipal which is the return value for the function.

First, you need to get the cookie value.  I struggled with getting the value -- I kept getting the name of the cookie or the name of the object, but not the actual value.  TryGetValue finally worked for me.
```html
        var result = _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(cookie_name, out cookieString);
```
Next, create the DataProtector with the same configuration values as those in the Web Forms application, and use it to Unprotect the cookie value and transform it into an AuthenticationTicket.  
```html
        var dataProtector = provider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", cookie_name, "v2");
        var ticketFormat = new Microsoft.AspNetCore.Authentication.TicketDataFormat(dataProtector);
        AuthenticationTicket ticket = ticketFormat.Unprotect(cookieString);
```
If the ticket has a Principal, return that value from the function.  If not, create a new, blank ClaimsPrincipal and return that.  

### 3. Wrap App.Razor with Cascading Authentication State tag

You will need to add the CascadingAuthenticationState tag around your existing App.Razor markup to trigger the custom AuthenticationStateProvider and access Authentication information.  Here is my whole App.Razor file.
```html
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(Program).Assembly">
        <Found Context="routeData">
            <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
        </Found>
        <NotFound>
            <LayoutView Layout="@typeof(MainLayout)">
                <p>Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
</CascadingAuthenticationState>
```
## Conclusion
With these changes to the Web Forms Application and the Blazor Application, you can now use FormsAuthentication with a server-side Blazor application.  

I really hope this solution is helpful to somebody out there.  I have pushed this out quickly as soon as I figured it out so that I could potentially save somebody else the task of figuring this out.  I will try to refine this code more as I put it into production.  For example, I need to verify if I need the configuration settings in the Owin Startup class -- if we are not really using Owin, we probably don't need it.  

## Sources / References
1. https://docs.microsoft.com/en-us/aspnet/core/security/cookie-sharing?view=aspnetcore-3.1
2. https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-3.1&tabs=aspnetcore2x
3. https://docs.microsoft.com/en-us/aspnet/core/blazor/security/?view=aspnetcore-3.1
My claims.razor class comes from this page. 
4. https://stackoverflow.com/questions/42842511/how-to-manually-decrypt-an-asp-net-core-authentication-cookie
5. http://blazorhelpwebsite.com/Blog/tabid/61/EntryId/4316/A-Demonstration-of-Simple-Server-side-Blazor-Cookie-Authentication.aspx

