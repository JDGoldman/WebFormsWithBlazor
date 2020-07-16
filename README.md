# WebFormsWithBlazor
This repository is meant to demonstrate how to integrate a separate Blazor App using Web Forms Authentication. 

Background: I have an old VB.NET Web Forms application using Forms Authentication.  I have been wanting to start all new development
using Blazor, but I didn't want my users to have to log on to both applications.  It took me several weeks to figure out how to use
my legacy Forms Authentication system with Blazor.  The help documentation on the Microsoft website was extremely misleading. 
So, I have created this repository so that hopefully I can save somebody else this time.  

This solution will not work on a server farm because it uses a file location to manage a shared key between the applications.  
If you are interested in a server farm solution, this article provides some guidance.  
https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-3.1
