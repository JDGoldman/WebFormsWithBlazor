# WebFormsWithBlazor
This repository is meant to demonstrate how to integrate authentication into a separate Blazor App project using Web Forms Authentication into an existing Web Forms application.

**Background**: I have an old VB.NET Web Forms application using Forms Authentication. I have been wanting to start all new development using Blazor, but I didn't want my users to have to log on to both applications separately. It took me several weeks to figure out how to use my legacy Forms Authentication system with Blazor. The help documentation on the Microsoft website was extremely misleading. I have created this repository so that hopefully I can save somebody else this time.

The branches of this project are the various stages of integration of the two technologies.  

**Note**: This solution will not work on a server farm because it uses a file location to manage a shared key between the applications.
If you are interested in a server farm solution, this article provides some guidance:
https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-3.1


**Setup**: The premise of this application is that you already have an old, Web Forms Forms Authentication application setup,
so you shouldn't need to create a new db.  You can just use your existing db. No changes are made to your db by this application. 
However, if you want to create a test aspnetdb, please follow these steps:
*1. To configure your Sql Server edition for Forms Authentication, you need to run the Sql Server Registration Tool 
located at %WINDIR%\Microsoft.Net\Framework\<.net version>\aspnet_regsql.exe -W
The "-W" flag causes the command prompt to launch a wizard which will allow you to 
select the Sql Server and the database name.  
For this project, I selected aspnetdb_test.
*2. Add roles and users to the Forms Authentication db.  I did this manually, which, I know, is a pain.  
I will generate a script at a later point.
