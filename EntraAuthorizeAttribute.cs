using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ContosoUniversity
{
    /// <summary>
    /// Authorization attribute used across the application in place of Windows-group
    /// checks. Anonymous requests fall through to the OWIN OpenID Connect middleware,
    /// which challenges the user to sign in with Microsoft Entra ID. Authenticated
    /// requests that fail a role check (e.g. non-admins hitting an admin-only action)
    /// are redirected to a friendly "Unauthorized" page instead of a generic 401, or
    /// receive a 403 JSON/status response for AJAX calls.
    /// </summary>
    public class EntraAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var httpContext = filterContext.HttpContext;
            var isAuthenticated = httpContext.User != null && httpContext.User.Identity.IsAuthenticated;

            if (isAuthenticated && httpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Access denied.");
            }
            else if (isAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/Home/Unauthorized");
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}
