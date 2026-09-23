using System.Web;
using System.Web.Mvc;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;

namespace ContosoUniversity.Controllers
{
    /// <summary>
    /// Handles Microsoft Entra ID sign-in and sign-out for the application.
    /// This replaces the implicit Windows Authentication challenge that used to be
    /// performed by IIS before requests ever reached the MVC pipeline.
    /// </summary>
    [AllowAnonymous]
    public class AccountController : Controller
    {
        // GET: /Account/SignIn - challenges the user with the Microsoft Entra ID sign-in page.
        public ActionResult SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new Microsoft.Owin.Security.AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
                return new HttpUnauthorizedResult();
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/SignOut - signs the user out of both the local cookie and Entra ID.
        public ActionResult SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(
                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                CookieAuthenticationDefaults.AuthenticationType);

            return RedirectToAction("Index", "Home");
        }
    }
}
