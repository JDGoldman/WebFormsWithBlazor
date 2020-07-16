# WebFormsWithBlazor
This repository is meant to demonstrate how to integrate authentication into a separate Blazor App project using ASP.NET Web Forms Authentication into an existing Web Forms application.

**Background**: I have an old VB.NET Web Forms application using Forms Authentication. I have been wanting to start all new development using Blazor, but I didn't want my users to have to log on to both applications separately. It took me several weeks to figure out how to use my legacy Forms Authentication system with Blazor. The help documentation on the Microsoft website was extremely misleading. I have created this repository so that hopefully I can save somebody else this time.

The branches of this project are the various stages of integration of the two technologies.  

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


**Setup**: The premise of this application is that you already have an old, Web Forms Forms Authentication application setup,
so you shouldn't need to create a new db.  You can just use your existing db. No changes are made to your db by this application. 
However, if you want to create a test aspnetdb, please follow these steps:

1. To configure your Sql Server edition for Forms Authentication, you need to run the Sql Server Registration Tool 
located at %WINDIR%\Microsoft.Net\Framework\<.net version>\aspnet_regsql.exe -W
The "-W" flag causes the command prompt to launch a wizard which will allow you to 
select the Sql Server and the database name.  For this project, I selected aspnetdb_test.
2. Add roles and users to the Forms Authentication db.  I did this manually, which, I know, is a pain.  
I will generate a script at a later point.
3. You will need to modify your web.config file in three spots to use your own "Application Name" if you are using an existing aspnetdb.  You will also need to modify the appsettings.json file accordingly.  
