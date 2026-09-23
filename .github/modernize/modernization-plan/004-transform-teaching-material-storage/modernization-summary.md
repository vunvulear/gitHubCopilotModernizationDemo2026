# Task 004 — Transform Teaching Material Storage to Azure Blob Storage

## Objective

Move course teaching-material image uploads/deletions off local disk (`~/Uploads/TeachingMaterials/`)
onto Azure Blob Storage, authenticated with Managed Identity, while preserving upload validation and
the stored reference consumed by the UI/database.

## Changes Made

### New files
- **`Services/IBlobStorageService.cs`** — storage abstraction with `UploadTeachingMaterial` and
  `DeleteTeachingMaterial`.
- **`Services/AzureBlobStorageService.cs`** — `Azure.Storage.Blobs` implementation:
  - Authenticates with `DefaultAzureCredential` (Managed Identity in Azure, developer credential
    locally) — no account keys or connection strings.
  - Container/service client built once and cached in a `static readonly Lazy<BlobContainerClient>`
    (Azure SDK clients are thread-safe and singleton-safe; a fresh client per request/controller
    instance would re-acquire an AAD token on every call).
  - Container created idempotently (`CreateIfNotExists(PublicAccessType.None)` — private container,
    no anonymous access).
  - Upload writes a GUID-suffixed blob name (`course_{CourseID}_{GUID}{ext}`) with the posted file's
    content type set via `BlobUploadOptions.HttpHeaders`; returns the blob's absolute HTTPS URL, which
    is what gets persisted as `Course.TeachingMaterialImagePath` (Views already render this field with
    `@Url.Content(...)`, which passes absolute URLs through unchanged, so no view changes were needed).
  - Delete accepts the stored reference (the absolute blob URL) and removes it via `DeleteIfExists()`;
    falls back to treating the trailing segment as a blob name for any legacy rows that still contain
    a local virtual path (`~/Uploads/TeachingMaterials/...`) from before this migration, so existing
    data doesn't hard-fail.

### Modified files
- **`Controllers/CoursesController.cs`** — `Create`, `Edit`, and `DeleteConfirmed` no longer call
  `Server.MapPath`, `Directory.CreateDirectory`, or `HttpPostedFileBase.SaveAs`. All three now call
  `IBlobStorageService` (field default-constructed the same way `BaseController.notificationService`
  is, so `new CoursesController()` keeps working for both MVC activation and the existing unit tests).
  File-type/size validation logic is unchanged. On `Edit`, the new image is uploaded before the old
  blob is deleted (safer than the original order, which deleted the old file before saving the new
  one) — this only matters on I/O failure and doesn't change any tested behavior.
- **`ContosoUniversity.csproj`** / **`packages.config`** — added `Azure.Storage.Blobs` and
  `Azure.Storage.Common` (+ their `System.IO.Hashing` dependency). Removed the unused
  `Services/FileStorage.cs` / `IFileStorage.cs` local-disk abstraction (dead code, superseded).
  Removed the `Uploads/TeachingMaterials/*` `Content` includes.
- **`Web.config`** — added `AzureStorage:AccountName` (placeholder
  `REPLACE-WITH-STORAGE-ACCOUNT-NAME`, sourced from Azure App Configuration via the existing
  `AzureAppConfig` configuration builder from task 002, same pattern as the `ida:*` settings) and
  `AzureStorage:ContainerName` (`teaching-materials`). No `infra/` folder existed at the time this
  task ran, so the account name is a placeholder to be filled in once the platform-engineer task
  provisions the storage account (see `infra/infra-config.md` when it exists).
- **`TEACHING_MATERIAL_UPLOAD.md`** — rewritten to describe the Blob Storage-backed design,
  configuration keys, and troubleshooting steps instead of the old local-disk instructions.
- Deleted the physical `Uploads/TeachingMaterials/` directory (including `.gitkeep`) — no longer
  needed since nothing is written to local disk.

### Dependency/version note
The storage-blob skill's reference SDK versions (`Azure.Storage.Blobs 12.28.0`) require
`Azure.Core >= 1.51.1`, which in turn requires bumping `System.ClientModel`, `System.Memory.Data`,
`System.Text.Json`, `Microsoft.Bcl.AsyncInterfaces`, etc. to very recent (`10.x`) versions — a large,
high-risk cascade across packages shared by the already-completed Entra ID / App Configuration / Key
Vault / SQL tasks (001–003). To avoid destabilizing that work, this task instead used
**`Azure.Storage.Blobs 12.24.0`** / **`Azure.Storage.Common 12.23.0`**, which only required bumping
`Azure.Core` to **1.44.1** (still floor-compatible with the already-referenced `Azure.Identity 1.11.4`,
whose only constraint is `Azure.Core >= 1.38.0`) plus minor bumps to `System.ClientModel` (1.0.0 →
1.1.0), `System.Memory.Data` (1.0.2 → 6.0.1), and `System.Text.Json` (6.0.10 → 6.0.11). All bumps are
floor-only (no upper-bound conflicts) and the full solution's main project builds clean afterward.
Binding redirects in `Web.config` were updated accordingly.

## Behavioral fidelity notes
- Blob name pattern (`course_{CourseID}_{GUID}.{ext}`) mirrors the original local filename pattern.
- File type/size validation (jpg/jpeg/png/gif/bmp, 5MB max) is untouched — validation still runs
  before any storage call, so upload/storage errors are only ever reported for actual storage
  failures, matching original behavior.
- Container is private (no anonymous public access); the app's Managed Identity needs the
  `Storage Blob Data Contributor` role on the target storage account/container (RBAC, configured via
  the platform-engineer task).
- Upload uses the unconditional-overwrite `BlobUploadOptions` (no `Conditions`) — each upload targets
  a brand-new GUID-suffixed name, so overwrite never actually triggers, but the intent is documented
  with a `// MIGRATION NOTE:` per the storage-blob skill's overwrite-semantics rule.

## Validation

- **Consistency check** (`validation-check-consistency` via the `task` agent): **0 issues** across all
  10 changed/added/removed files.
- **Completeness**: no remaining local-disk upload code (`Server.MapPath`, `SaveAs`,
  `Uploads/TeachingMaterials` path handling) outside of explanatory comments describing the legacy
  path format still handled for backward compatibility in `DeleteTeachingMaterial`.
- **Build**: `ContosoUniversity.csproj` — clean rebuild, 0 errors.
- **Unit tests**: The sibling `ContosoUniverisity.Tests` / `ContosoUniverisity.Tests.UnitTests`
  projects (outside this git repository) have a **pre-existing** missing `ProjectReference` to
  `ContosoUniversity.csproj` (already called out in the task 002 summary as a baseline gap, unrelated
  to this repo). This was true before this task's changes and is unrelated to teaching-material
  storage; it is not something this task introduced or could fix within the `ContosoUniversity`
  repository boundary. The two characterization tests that exercise `CoursesController.Create`
  (`Courses_Create_InvalidExtension_ShouldReturnModelError`,
  `Courses_Create_OversizeFile_ShouldReturnModelError`) only cover the validation-failure path, which
  runs identically before and after this change (rejection happens before any storage call).

## Success criteria status

| Criterion | Status | Notes |
| --- | --- | --- |
| `passBuild` | ✅ true | `ContosoUniversity.csproj` builds clean (Debug, rebuild) |
| `passUnitTests` | ⚠️ false (pre-existing baseline gap) | Same pre-existing test-project reference gap already flagged in tasks 002/003; not introduced or fixable within this repo/task |
