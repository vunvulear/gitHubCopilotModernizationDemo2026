using System.Web.Mvc;

namespace ContosoUniversity
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            // Require every controller/action to be authenticated with Microsoft Entra ID
            // by default. This preserves the previous Windows Authentication behavior,
            // where IIS required a signed-in domain user before any page was served.
            // Individual actions opt out with [AllowAnonymous] (e.g. AccountController,
            // and the Home error/unauthorized pages) and admin-only actions further
            // restrict access with [EntraAuthorize(Roles = "Admin")].
            filters.Add(new EntraAuthorizeAttribute());
        }
    }
}
