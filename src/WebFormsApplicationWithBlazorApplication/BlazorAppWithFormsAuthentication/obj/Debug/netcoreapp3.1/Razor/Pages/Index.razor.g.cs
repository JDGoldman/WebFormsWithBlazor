#pragma checksum "D:\Data\Development\Development_Other\GitHubRepos\WebFormsWithBlazor\src\WebFormsApplicationWithBlazorApplication\BlazorAppWithFormsAuthentication\Pages\Index.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "522dec54f1a5bc3adc1534cfdad1d121248f2ad7"
// <auto-generated/>
#pragma warning disable 1591
namespace BlazorAppWithFormsAuthentication.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "D:\Data\Development\Development_Other\GitHubRepos\WebFormsWithBlazor\src\WebFormsApplicationWithBlazorApplication\BlazorAppWithFormsAuthentication\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Data\Development\Development_Other\GitHubRepos\WebFormsWithBlazor\src\WebFormsApplicationWithBlazorApplication\BlazorAppWithFormsAuthentication\_Imports.razor"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Data\Development\Development_Other\GitHubRepos\WebFormsWithBlazor\src\WebFormsApplicationWithBlazorApplication\BlazorAppWithFormsAuthentication\_Imports.razor"
using Microsoft.AspNetCore.Components.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Data\Development\Development_Other\GitHubRepos\WebFormsWithBlazor\src\WebFormsApplicationWithBlazorApplication\BlazorAppWithFormsAuthentication\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\Data\Development\Development_Other\GitHubRepos\WebFormsWithBlazor\src\WebFormsApplicationWithBlazorApplication\BlazorAppWithFormsAuthentication\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "D:\Data\Development\Development_Other\GitHubRepos\WebFormsWithBlazor\src\WebFormsApplicationWithBlazorApplication\BlazorAppWithFormsAuthentication\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\Data\Development\Development_Other\GitHubRepos\WebFormsWithBlazor\src\WebFormsApplicationWithBlazorApplication\BlazorAppWithFormsAuthentication\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "D:\Data\Development\Development_Other\GitHubRepos\WebFormsWithBlazor\src\WebFormsApplicationWithBlazorApplication\BlazorAppWithFormsAuthentication\_Imports.razor"
using BlazorAppWithFormsAuthentication;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "D:\Data\Development\Development_Other\GitHubRepos\WebFormsWithBlazor\src\WebFormsApplicationWithBlazorApplication\BlazorAppWithFormsAuthentication\_Imports.razor"
using BlazorAppWithFormsAuthentication.Shared;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/")]
    public partial class Index : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
            __builder.AddMarkupContent(0, "<h1>Hello, world!</h1>\r\n\r\nWelcome to your new app.\r\n\r\n");
            __builder.OpenComponent<BlazorAppWithFormsAuthentication.Shared.SurveyPrompt>(1);
            __builder.AddAttribute(2, "Title", "How is Blazor working for you?");
            __builder.CloseComponent();
            __builder.AddMarkupContent(3, "\r\n");
            __builder.AddMarkupContent(4, "<p>Authorize view below:</p>\r\n");
            __builder.OpenComponent<Microsoft.AspNetCore.Components.Authorization.AuthorizeView>(5);
            __builder.AddAttribute(6, "Authorized", (Microsoft.AspNetCore.Components.RenderFragment<Microsoft.AspNetCore.Components.Authorization.AuthenticationState>)((context) => (__builder2) => {
                __builder2.AddContent(7, "You are authorized.");
            }
            ));
            __builder.AddAttribute(8, "NotAuthorized", (Microsoft.AspNetCore.Components.RenderFragment<Microsoft.AspNetCore.Components.Authorization.AuthenticationState>)((context) => (__builder2) => {
                __builder2.AddContent(9, "You are not authorized.");
            }
            ));
            __builder.CloseComponent();
        }
        #pragma warning restore 1998
    }
}
#pragma warning restore 1591