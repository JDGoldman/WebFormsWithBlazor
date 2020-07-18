# Web Forms Authentication With Blazor Application
This repository is meant to demonstrate how to integrate authentication from a ASP.NET Forms Authentication application with a separate Blazor application. 

## Background
I have an old VB.NET Web Forms application using Forms Authentication. I have been wanting to start all new development using Blazor, but I didn't want my users to have to log on to both applications separately. It took me several weeks to figure out how to use my legacy Forms Authentication system with Blazor. The help documentation on the Microsoft website was extremely misleading. I have created this repository so that hopefully I can save somebody else this time.  The solution to the problem, in a nutshell, was to create a cookie which could be shared between the two applications which contains a ClaimsPrincipal object.  Actually figuring out how to implement this took a ton of time.

The branches of this project are the various stages of integration of the two technologies:  

1. Master is a new ASP.NET Web Forms project created by the Visual Studio wizard (note, no authentication added).  
2. Step 1 is the modified ASP.NET Web Forms project to include all of the code and configuration changes to work with the Blazor App.
3. Step 2 is the new Blazor Server application created by the Visual Studio wizard (note, no authentication added).
4. Step 3 is the modified Blazor Server application to run with the Web Forms Authentication from the Web Forms project.

**Several Notes**: 
1.  All authentication is handled by the Web Forms application.  
2.  If you are running this through Visual Studio/IIS Express, you will need to make sure both websites are running.  
3.  The Blazor app may need to be refreshed after logging in or out.   
4.  This solution will not work on a server farm because it uses a file location to manage a shared key between the applications.
If you are interested in a server farm solution, this article provides some guidance:
https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-3.1


## Project Setup

The premise of this application is that you already have an old, Web Forms Forms Authentication application setup,
so you shouldn't need to create a new db.  You can just use your existing db. No changes are made to your db by this application. 
However, if you want to create a test aspnetdb, please follow these steps:

1. To configure your Sql Server edition for Forms Authentication, you need to run the Sql Server Registration Tool 
located at %WINDIR%\Microsoft.Net\Framework\<.net version>\aspnet_regsql.exe -W
The "-W" flag causes the command prompt to launch a wizard which will allow you to 
select the Sql Server and the database name.  For this project, I selected aspnetdb_test.
2. Add roles and users to the Forms Authentication db.  I did this manually, which, I know, is a pain.  
I will generate a script at a later point.

You will need to modify your web.config file in three spots to use your own "Application Name" if you are using an existing aspnetdb. You will also need to modify the appsettings.json file accordingly.  
1. In the App Settings Key for Application Name.
2. In the Membership Provider Key for applicationName.
3. In the Role Manager Key for applicationName.  

## Authorization Changes

### 1. Add logic to Web Forms application to create Claims from Membership User.  

First step is to create a Claims for the authenticated user from the existing Membership User:
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
And then, taking these Claims, create a new ClaimsPrincipal which can be used by the Blazor App:
```html
       Dim identity = New ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType)
       Dim principal = New ClaimsPrincipal(identity)
```

Next, create a DataProtector which will be used by both applications:

```html
        Dim provider = DataProtectionProvider.Create(New System.IO.DirectoryInfo(location),
                Function(Builder) {Builder.SetApplicationName(application_name)})
        Dim protector = provider.CreateProtector(
                "Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware",
                cookie_name,
                "v2")
```

Use this DataProtector to create an AuthenticationTicket which can be consumed by the ASP.NET Core (Blazor) application.
```html
        'use data protector to protect ticket
        Dim ticketFormat = New Microsoft.AspNetCore.Authentication.TicketDataFormat(protector)
        Dim tkt = New Microsoft.AspNetCore.Authentication.AuthenticationTicket(principal, "Cookies")
        Dim protectedTkt = ticketFormat.Protect(tkt)
```
Finally, set the HttpCookie to be shared by both applications:
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


