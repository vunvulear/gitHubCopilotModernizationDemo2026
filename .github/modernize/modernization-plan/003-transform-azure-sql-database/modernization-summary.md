# Modernization Summary: 003-transform-azure-sql-database

## Objective
Migrate the EF Core persistence layer from LocalDB (integrated security) to Azure SQL
Database, authenticating with Azure Managed Identity instead of a user/password or
Windows-integrated-security connection string, while preserving the existing schema, seed
data, and CRUD behavior.

## Knowledge Bases Applied
- `migration-azure-sql-database` (builtin) — connection string shape
  (`Authentication=Active Directory Default`), `Microsoft.Data.SqlClient` as the required
  ADO.NET provider.
- `migration-managed-identity` (builtin) — `DefaultAzureCredential`-based Managed Identity
  pattern for Azure SQL Database, no secrets in code/config.

## Infrastructure Configuration
No `infra/` folder exists in the repository yet (the platform-engineer provisioning task has
not run), so no real Azure SQL Server/database name was available. A placeholder
(`REPLACE-WITH-SQL-SERVER-NAME`) is used in the connection string, consistent with the
pattern already established by task 002 for the App Configuration/Key Vault endpoints. Once
`infra/infra-config.md` is produced by the platform-engineer, the placeholder should be
replaced with the real Azure SQL logical server name (or supplied via the `AzureKeyVault`
configuration builder / an App Service connection-string override at deploy time — no code
change required either way, since `SchoolContextFactory` reads the connection string
indirectly through `ConfigurationManager.ConnectionStrings`).

## Changes Made

### Web.config
- Replaced the LocalDB connection string
  (`Data Source=(LocalDb)\MSSQLLocalDB;...;Integrated Security=True;...`) with an Azure SQL
  Database connection string using Managed Identity:
  `Server=tcp:REPLACE-WITH-SQL-SERVER-NAME.database.windows.net,1433;Database=ContosoUniversityNoAuthEFCore;Authentication=Active Directory Default;TrustServerCertificate=False;Encrypt=True;`
  - `Authentication=Active Directory Default` lets `Microsoft.Data.SqlClient` resolve
    credentials via `DefaultAzureCredential` under the hood (System/User-Assigned Managed
    Identity in Azure, developer credentials such as Azure CLI/Visual Studio locally) — no
    user name or password anywhere.
  - Kept the connection string under the existing `configBuilders="AzureKeyVault"` builder
    (added in task 002), so the real endpoint can also be sourced from Key Vault once
    provisioned/seeded, without further code changes.
- Updated/added assembly `<bindingRedirect>` entries in `<runtime>` for every package that was
  bumped (see below), based on the exact conflicts MSBuild reported (MSB3247/MSB3277) after
  the package updates, verified against the actual on-disk assembly file versions.

### packages.config / ContosoUniversity.csproj
Upgraded `Microsoft.Data.SqlClient` from `2.1.4` (has a known high-severity CVE,
GHSA-98g6-xh36-x2p7) to `6.0.2`, replacing the old `Microsoft.Data.SqlClient.SNI.runtime`
native-binary package (and its hand-rolled `Copy` post-build target) with the officially
supported `Microsoft.Data.SqlClient.SNI` 6.0.2 package, whose own `.targets` file is now
imported to place `x86/x64/arm64` native SNI binaries in the output directory. This is a
runtime-only ADO.NET provider swap; `Microsoft.Data.SqlClient` maintains full API
compatibility with `System.Data.SqlClient`/its own prior versions, so no consumer code
changes were required.

`Microsoft.Data.SqlClient 6.0.2`'s own dependency graph (per its official .NET Framework
release notes) required a coordinated bump of the following packages, each resolved to the
minimum version that satisfies every consumer in the graph (Azure.Identity, Azure.Core, MSAL,
and the `Microsoft.Extensions.*` family are also relied on by the task-002 Azure App
Configuration/Key Vault Configuration Builders and by EF Core 3.1.32 respectively, so a single
unified version was chosen for each shared assembly and reconciled with binding redirects):

| Package | Before | After |
|---|---|---|
| Microsoft.Data.SqlClient | 2.1.4 | 6.0.2 |
| Microsoft.Data.SqlClient.SNI.runtime → Microsoft.Data.SqlClient.SNI | 2.1.1 | 6.0.2 |
| Azure.Identity | 1.2.1 | 1.11.4 |
| Azure.Core | 1.16.0 | 1.38.0 |
| Microsoft.Identity.Client | 4.21.1 | 4.61.3 |
| Microsoft.Identity.Client.Extensions.Msal | 2.12.0 | 4.61.3 |
| System.Diagnostics.DiagnosticSource | 4.7.1 | 6.0.1 |
| System.Text.Json | 4.6.0 | 6.0.10 |
| System.Text.Encodings.Web | 4.7.2 | 6.0.0 |
| System.Security.Cryptography.ProtectedData | 4.5.0 | 4.7.0 |
| Microsoft.Bcl.AsyncInterfaces | 1.1.1 | 6.0.0 |
| System.Runtime.CompilerServices.Unsafe | 4.5.3 | 6.0.0 |
| Microsoft.Extensions.Caching.Abstractions | 3.1.32 | 8.0.0 |
| Microsoft.Extensions.Caching.Memory | 3.1.32 | 8.0.1 |
| Microsoft.Extensions.DependencyInjection.Abstractions | 3.1.32 | 8.0.2 |
| Microsoft.Extensions.Logging.Abstractions | 3.1.32 | 8.0.2 |
| Microsoft.Extensions.Options | 3.1.32 | 8.0.2 |
| Microsoft.Extensions.Primitives | 3.1.32 | 8.0.0 |

New direct dependencies added (required by `Microsoft.Data.SqlClient` 6.0.2 / its
dependents on .NET Framework): `Microsoft.Bcl.Cryptography` 8.0.0,
`System.Security.Cryptography.Pkcs` 8.0.1, `System.ClientModel` 1.0.0,
`System.IO.FileSystem.AccessControl` 5.0.0.

Packages intentionally left unchanged because they already satisfied every consumer's
minimum version requirement: `System.Buffers` 4.5.1, `System.Memory` 4.5.4,
`System.Numerics.Vectors` 4.5.0, `System.Threading.Tasks.Extensions` 4.5.4,
`System.ValueTuple` 4.5.0, `Microsoft.IdentityModel.*` 8.2.1 family,
`System.Memory.Data` 1.0.2. `Microsoft.Extensions.Configuration.*`,
`Microsoft.Extensions.DependencyInjection` (concrete), and `Microsoft.Extensions.Logging`
(concrete) were kept at EF Core's own `3.1.32` since `Microsoft.Data.SqlClient` does not
depend on them; their `*.Abstractions` counterparts were bumped and reconciled via binding
redirects, which is the standard mechanism NuGet/MSBuild use for this exact scenario.

### Data/SchoolContextFactory.cs / Data/SchoolContext.cs
No changes required — both already used `Microsoft.EntityFrameworkCore.SqlServer`'s
`UseSqlServer(connectionString)` reading the connection string from
`ConfigurationManager.ConnectionStrings["DefaultConnection"]`, which transparently picks up
the new Azure SQL Database + Managed Identity connection string.

## Validation
- **Consistency check** (performed directly per the `validation-check-consistency` skill,
  since the `task`/`general-purpose` sub-agents were unavailable in this session — CAPIError
  401 "Invalid auto-mode selector" on every retry): reviewed the full diff of
  `ContosoUniversity.csproj`, `Web.config`, and `packages.config` hunk-by-hunk. All changes are
  version-bump/HintPath/binding-redirect pairs that are internally consistent (every bumped
  assembly's binding redirect matches its actual on-disk file version, verified via
  `[System.Reflection.AssemblyName]::GetAssemblyName`), plus the one intentional connection
  string change matching the `migration-azure-sql-database` skill's documented format exactly.
  **Zero Critical, zero Major, zero Minor issues identified.**
- **Build**: `ContosoUniversity.csproj` builds cleanly (0 errors) via `msbuild /t:Rebuild`,
  with **zero remaining MSB3247 binding-redirect conflict warnings** (all resolved).
  Native SNI binaries (`Microsoft.Data.SqlClient.SNI.{x64,x86,arm64}.dll`) are correctly
  copied to `bin\`.
- **Unit tests**: `ContosoUniverisity.Tests.csproj` (the actual test-case project) fails to
  compile with the exact same pre-existing errors as at the baseline commit
  `a4a63248b23931aa1eeea472eaaef06c583419fa` (confirmed by rebuilding the full solution both
  before and after this task's changes) — its `.csproj` has **no `ProjectReference` to
  `ContosoUniversity.csproj`** at all, so every reference to `ContosoUniversity.*` types
  (`SchoolContext`, `Notification`, etc.) fails with `CS0246`. This is unrelated to and
  predates this task (already flagged in task 002's summary as a gap requiring a separate,
  out-of-scope fix to `ContosoUniverisity.Tests.csproj`'s references). **No new test
  failures were introduced by this task** — the main `ContosoUniversity` project itself
  builds and its data-access surface (`SchoolContext`/`SchoolContextFactory`) is unchanged
  apart from the connection string and provider package version.

## Old Technology Check
- No `System.Data.SqlClient` references remain anywhere in the codebase (the project already
  used `Microsoft.Data.SqlClient`; it has now been upgraded to a current, CVE-free version).
- No LocalDB / Windows-integrated-security connection strings remain in `Web.config` (only a
  descriptive code comment mentions "LocalDB" to explain what was migrated away from).
- No secrets, passwords, or connection-string credentials were introduced anywhere.

## Files Modified
- `Web.config`
- `ContosoUniversity.csproj`
- `packages.config`
- `packages/` (new/updated NuGet package folders, not tracked in git — see `.gitignore`)

## Follow-ups for Later Tasks / Platform Engineer
- Replace `REPLACE-WITH-SQL-SERVER-NAME` in `Web.config`'s `DefaultConnection` connection
  string with the real Azure SQL logical server name once `infra/infra-config.md` is
  available, or supply it via the `AzureKeyVault` configuration builder / an App Service
  connection-string application setting at deploy time.
- The Managed Identity used at runtime (System- or User-Assigned) must be granted an
  appropriate Azure SQL Database role (e.g. `db_datareader`/`db_datawriter` or higher) on the
  target database via a Microsoft Entra-authenticated `CREATE USER ... FROM EXTERNAL
  PROVIDER` statement — this is an infrastructure/DBA action outside the scope of this
  code-migration task.
- The pre-existing `ContosoUniverisity.Tests.csproj` missing `ProjectReference` to
  `ContosoUniversity.csproj` should be fixed in a separate task so the test suite can
  actually compile and run.
