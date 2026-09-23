# Modernization Summary: 002-transform-azure-app-configuration-and-key-vault

## Objective

Externalize `Web.config` application settings and secrets so the app can be run on Azure with
centrally managed, Managed-Identity-authenticated configuration, without changing the shape of
the existing configuration consumers.

## Technology Context

ContosoUniversity is a **.NET Framework 4.8** ASP.NET MVC 5 application (not ASP.NET Core), so the
ASP.NET-Core-oriented guidance in the `migration-local-appsettings-azure-app-configuration` skill
(which assumes `WebApplication.CreateBuilder` / `builder.Configuration.AddAzureAppConfiguration`)
does not apply directly. The .NET Framework equivalent — and the approach used here — is
**Configuration Builders** (`Microsoft.Configuration.ConfigurationBuilders.*`), the built-in
.NET Framework 4.7.1+ mechanism for sourcing `<appSettings/>` / `<connectionStrings/>` from
external providers such as Azure App Configuration and Azure Key Vault. This preserves the intent
of the skills (Managed Identity auth, no secrets in code/config, existing consumers unaffected)
while fitting the actual runtime.

## Changes Made

### Web.config
- Added the `<configSections>` entry required to declare `<configBuilders/>`.
- Added a `<configBuilders/>` section with two builders:
  - **AzureAppConfig** (`AzureAppConfigurationBuilder`, mode `Greedy`) — sources non-secret
    application settings (Entra ID app registration values, notification queue path, etc.) from
    Azure App Configuration.
  - **AzureKeyVault** (`AzureKeyVaultConfigBuilder`, mode `Greedy`) — sources secrets / sensitive
    connection material (e.g. the SQL connection string) from Azure Key Vault.
  - Both builders use **Managed Identity** (`DefaultAzureCredential`, the default credential for
    these builders when no connection string / client secret / certificate is configured) — no
    secret material is stored anywhere in the repo.
  - Both are marked `optional="true"` so the app keeps working from the local
    `<appSettings/>` / `<connectionStrings/>` values whenever the store/vault is unreachable
    (local dev, or before the resources are provisioned by the platform-engineer task).
- Applied `configBuilders="AzureAppConfig"` to `<appSettings/>` and
  `configBuilders="AzureKeyVault"` to `<connectionStrings/>`.
- `endpoint`/`vaultName` are currently placeholders (`REPLACE-WITH-APP-CONFIG-STORE` /
  `REPLACE-WITH-KEYVAULT-NAME`) with comments explaining they must be replaced with the real
  resource identifiers once Azure App Configuration / Key Vault are provisioned (see
  `infra/infra-config.md` when available — it did not exist at the time this task ran), or
  overridden per-environment via an Azure App Service Application Setting named
  `AzureAppConfig:endpoint` / `AzureKeyVault:vaultName` (Configuration Builders' built-in
  "load from appSettings" override convention).
- No secret values (client secrets, passwords, keys) were added anywhere.

### Zero changes to configuration consumers
`Startup.cs`, `Services/NotificationService.cs`, `Data/SchoolContextFactory.cs`, and
`Global.asax.cs` all continue to read configuration via
`ConfigurationManager.AppSettings[...]` / `ConfigurationManager.ConnectionStrings[...]` exactly as
before — Configuration Builders transparently intercept these lookups at the framework level, so
no consumer code changes were required (satisfies "keep the current configuration consumers
working with minimal shape changes").

### packages.config / ContosoUniversity.csproj
Added the following packages (and their required supporting assemblies) with explicit
`<Reference>`/`HintPath` entries (this is a `packages.config`-style, non-SDK project):
- `Microsoft.Configuration.ConfigurationBuilders.Base` 3.0.0
- `Microsoft.Configuration.ConfigurationBuilders.AzureAppConfiguration` 3.0.0
- `Microsoft.Configuration.ConfigurationBuilders.Azure` 3.0.0 (provides `AzureKeyVaultConfigBuilder`)
- `Azure.Core` 1.16.0, `Azure.Identity` 1.2.1, `Azure.Security.KeyVault.Secrets` 4.2.0,
  `Azure.Data.AppConfiguration` 1.1.0
- Supporting polyfill/runtime assemblies: `System.Memory.Data`, `System.Text.Json`,
  `System.Text.Encodings.Web`, `System.Security.Cryptography.ProtectedData`,
  `Microsoft.Identity.Client.Extensions.Msal`, `System.Buffers`, `System.Memory`,
  `System.Numerics.Vectors`, `System.Runtime.CompilerServices.Unsafe`, `System.ValueTuple`

### Assembly binding redirects
Added `<dependentAssembly>` binding redirects in `Web.config` for every new assembly (Azure.Core,
Azure.Identity, Azure.Security.KeyVault.Secrets, Azure.Data.AppConfiguration, System.Memory.Data,
System.Text.Json, System.Text.Encodings.Web, System.Security.Cryptography.ProtectedData,
Microsoft.Identity.Client.Extensions.Msal, Microsoft.Identity.Client, System.Buffers,
System.ValueTuple) based on the exact conflicts/redirects MSBuild reported (MSB3247/MSB3277)
during the build.

## Infrastructure Configuration

No `infra/` folder exists yet in this repository (the platform-engineer provisioning task has not
run). No real App Configuration / Key Vault endpoint values were available to wire in. Placeholder
values were used with `optional="true"` so the build and any future test run are not blocked by
unreachable Azure resources. **Follow-up required**: once `infra/infra-config.md` is produced,
replace the `endpoint` / `vaultName` placeholder attributes in `Web.config`'s `<configBuilders/>`
section with the real values (or supply them via the `AzureAppConfig:endpoint` /
`AzureKeyVault:vaultName` Application Settings override at deploy time).

## Validation

- **Consistency check**: zero Critical / Major / Minor issues (see agent run for task 002).
- **Build**: `ContosoUniversity.csproj` builds successfully (0 errors) with MSBuild, both
  incremental and clean rebuild.
- **Unit tests**: The sibling `ContosoUniverisity.Tests` project has a **pre-existing** issue
  (present at the baseline commit `a4a63248b23931aa1eeea472eaaef06c583419fa`, unrelated to this
  task) — its `.csproj` does not reference the `ContosoUniversity` project/assembly at all, so it
  fails to compile (`CS0246: The type or namespace 'ContosoUniversity' could not be found`)
  regardless of any change made here. This is not a regression introduced by task 002; the main
  application project itself builds cleanly. This gap should be raised separately (e.g. to the
  integration-tester / plan owner) since it currently blocks running the unit test suite at all.

## Files Modified
- `Web.config`
- `ContosoUniversity.csproj`
- `packages.config`
- `packages/` (new NuGet package folders added, not tracked previously)

## Old Technology Check
No old-technology references relevant to this task remain: `Web.config` no longer stores
configuration as the sole source of truth — `<appSettings/>` and `<connectionStrings/>` are now
wired to be sourced from Azure App Configuration / Azure Key Vault via Configuration Builders,
with local values retained only as a safe fallback. No secrets or connection strings were added in
plaintext beyond what already existed at baseline (LocalDB dev connection string, which remains as
the pre-provisioning fallback and will be replaced by the Azure SQL Database migration task, 003).
