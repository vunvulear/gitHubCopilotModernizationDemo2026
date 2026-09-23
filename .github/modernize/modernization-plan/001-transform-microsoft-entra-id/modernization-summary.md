# Modernization Summary: 001-transform-microsoft-entra-id

## Objective
Replace the implicit Windows Authentication dependency (IIS Express Windows Auth /
domain sign-in) with Microsoft Entra ID authentication, so the app can run on Azure
App Service hosting, while preserving the existing admin/user experience and mapping
"admin-only" behavior (previously only described in code comments, never enforced) to
real role-based authorization.

## Changes Made

### Authentication (OWIN OpenID Connect)
- Added `Startup.cs` (OWIN `[assembly: OwinStartup]`) that configures cookie
  authentication plus OpenID Connect authentication against Microsoft Entra ID,
  reading `ida:ClientId`, `ida:AADInstance`, `ida:TenantId`, `ida:PostLogoutRedirectUri`
  from `Web.config` `appSettings` (placeholder values - to be replaced with the real
  app registration, and to be moved to Azure App Configuration / Key Vault in task
  002-transform-azure-app-configuration-and-key-vault).
- `TokenValidationParameters.RoleClaimType = "roles"` maps Entra ID App Roles into
  `ClaimsPrincipal.IsInRole(...)`, so standard MVC `Roles = "Admin"` checks work the
  same way Windows-group checks used to.
- Cookies are issued with `CookieSecureOption.Always` for Azure App Service (TLS
  terminated at the edge).
- Added `Controllers/AccountController.cs` with `SignIn` (challenges Entra ID) and
  `SignOut` (signs out of both the local cookie and Entra ID) actions.

### Authorization
- Added `EntraAuthorizeAttribute` (custom `AuthorizeAttribute`) that:
  - Lets anonymous requests fall through to the OWIN OIDC challenge (equivalent to the
    old IIS-level Windows Auth challenge).
  - Redirects authenticated-but-unauthorized browser requests to a new
    `Views/Home/Unauthorized.cshtml` page instead of a bare 401.
  - Returns HTTP 403 for AJAX requests instead of redirecting (so `fetch()` calls fail
    cleanly rather than receiving an HTML page as JSON).
- `App_Start/FilterConfig.cs` now registers `EntraAuthorizeAttribute` as a global
  filter, requiring sign-in for every controller/action by default - matching the
  previous "no anonymous access" behavior IIS enforced via Windows Authentication.
- `[EntraAuthorize(Roles = "Admin")]` applied to the actions the code already called
  out as admin-only: `Delete`/`DeleteConfirmed` in `CoursesController`,
  `StudentsController`, `InstructorsController`, `DepartmentsController`, and the
  `NotificationsController` admin dashboard (`Index`, `GetNotifications`,
  `MarkAsRead`).
- `HomeController.Error` / `HomeController.Unauthorized` and the whole
  `AccountController` are `[AllowAnonymous]` so sign-in/error/unauthorized pages remain
  reachable pre-authentication.
- `Views/Shared/_Layout.cshtml` only loads `notifications.js` for users in the `Admin`
  role, avoiding needless 403s for regular users polling an admin-only endpoint.

### UI / UX
- `_Layout.cshtml` nav bar now shows "Signed in as {name}" plus a Sign out link when
  authenticated, or a Sign in link when not.
- Added `Views/Home/Unauthorized.cshtml` friendly access-denied page.
- `Home/Index.cshtml` copy updated to reference Microsoft Entra ID instead of Windows
  Authentication.
- `BaseController.SendEntityNotification` now attributes notifications to the signed-in
  user's name (falls back to "System" only when unauthenticated), replacing the
  previous hardcoded `"System"` value used because there was no real authentication.

### Project / packaging
- `packages.config` / `ContosoUniversity.csproj`: added Microsoft.Owin,
  Microsoft.Owin.Host.SystemWeb, Microsoft.Owin.Security(.Cookies/.OpenIdConnect),
  Owin, and Microsoft.IdentityModel.* / System.IdentityModel.Tokens.Jwt (v8.2.1,
  net472 binaries) NuGet packages and references.
- `Web.config`: added `ida:*` appSettings and assembly binding redirects for the new
  OWIN/IdentityModel assemblies (and a pre-existing `System.Memory` conflict flagged by
  the build).
- `ContosoUniversity.csproj` / `ContosoUniversity.csproj.user`: IIS Express settings
  flipped from `IISExpressWindowsAuthentication=enabled` /
  `IISExpressAnonymousAuthentication=disabled` to the opposite, since the app now
  performs its own authentication via OWIN middleware instead of relying on IIS.

## Consistency Check
Ran `validation-check-consistency` twice:
- First pass: 1 Major (admin notification AJAX endpoints not role-restricted), 1 Minor
  (unused `ClientSecret` field). Both fixed.
- Second pass: 1 Minor (cookie secure option not hardened for production). Fixed by
  setting `CookieSecureOption.Always`.
- Final state: 0 Critical, 0 Major, 0 Minor outstanding.

## Build & Tests
- `msbuild ContosoUniversity.csproj /t:Clean,Build` succeeds with no errors.
- No dedicated unit test project exists inside the `ContosoUniversity` repository
  itself. The characterization/unit test projects (`ContosoUniverisity.Tests`,
  `ContosoUniverisity.Tests.UnitTests`) live in sibling folders outside this
  repository's boundary and are owned by the integration-tester baseline workflow;
  they were left untouched per the "never modify baseline test artifacts" rule. The
  Delete/notification actions that gained `[EntraAuthorize]` are invoked directly
  (not through the MVC filter pipeline) by those characterization tests, so the new
  authorization attributes do not affect their behavior.

## Follow-ups for later tasks
- Replace placeholder `ida:ClientId` / `ida:TenantId` / `ida:Domain` values with the
  real Entra ID app registration, and move them (plus any client secret, if a
  confidential-client flow is later required) to Azure App Configuration / Key Vault
  (task 002).
- Provision Entra ID App Roles ("Admin") and assign them to real users/groups so the
  `[EntraAuthorize(Roles = "Admin")]` checks resolve correctly at runtime.
