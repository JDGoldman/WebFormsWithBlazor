using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;



namespace BlazorAppWithFormsAuthentication
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public CustomAuthStateProvider(IHttpContextAccessor httpContextAccessor, IConfiguration Configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = Configuration;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {

            ClaimsPrincipal user;

            string cookieString = "";
            var cookie_name = _configuration["CookieName"];
            var application_name = _configuration["ApplicationName"];
            
            var result = _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(cookie_name, out cookieString);
            if (result)
            {
                var shared_key_location = _configuration["SharedKeyFileLocation"];
                var provider = DataProtectionProvider.Create(new System.IO.DirectoryInfo(shared_key_location),
                (builder) =>
                {
                    builder.SetApplicationName(application_name);
                    //builder.ProtectKeysWithDpapi(false);
                });

                var dataProtector = provider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", cookie_name, "v2");
                var ticketFormat = new Microsoft.AspNetCore.Authentication.TicketDataFormat(dataProtector);
                AuthenticationTicket ticket = ticketFormat.Unprotect(cookieString);
                if (ticket != null) {
                    user = ticket.Principal;
                } else {
                    var identity = new ClaimsIdentity();
                    user = new ClaimsPrincipal(identity);

                }
            }
            else
            {
                var identity = new ClaimsIdentity();
                user = new ClaimsPrincipal(identity);
            }


            return Task.FromResult(new AuthenticationState(user));
        }
    }
}
