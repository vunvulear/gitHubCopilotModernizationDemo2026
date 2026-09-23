using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

[assembly: OwinStartup(typeof(ContosoUniversity.Startup))]

namespace ContosoUniversity
{
    /// <summary>
    /// OWIN startup class that wires up Microsoft Entra ID (formerly Azure AD) authentication
    /// using the OpenID Connect protocol. This replaces the previous Windows Authentication
    /// model so the application can run on Azure hosting without a domain-joined workstation
    /// or server.
    /// </summary>
    public class Startup
    {
        // Entra ID (Azure AD) application registration settings.
        // These are read from Web.config appSettings so they can be externalized to
        // Azure App Configuration / Key Vault in a later modernization task.
        private static readonly string ClientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static readonly string AadInstance = ConfigurationManager.AppSettings["ida:AADInstance"] ?? "https://login.microsoftonline.com/{0}/v2.0";
        private static readonly string TenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static readonly string PostLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        private static readonly string Authority = string.Format(AadInstance, TenantId);

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                // Azure App Service always terminates TLS, so require secure cookies.
                CookieSecure = CookieSecureOption.Always,
                ExpireTimeSpan = TimeSpan.FromHours(8),
                SlidingExpiration = true
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = ClientId,
                Authority = Authority,
                PostLogoutRedirectUri = PostLogoutRedirectUri,
                RedirectUri = PostLogoutRedirectUri,
                ResponseType = "id_token",
                Scope = "openid profile email",
                // Microsoft Entra ID App Roles are surfaced in the "roles" claim. Mapping the
                // role claim type lets ClaimsPrincipal.IsInRole(...) and the MVC
                // [Authorize(Roles = "Admin")] attribute keep working the same way the
                // previous Windows-group based admin checks used to.
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    NameClaimType = "name",
                    RoleClaimType = "roles"
                },
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/Home/Error?message=" + Uri.EscapeDataString(context.Exception.Message));
                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}
