# Modernization Summary: 005-transform-azure-servicebus-notifications

## Objective

Replace the MSMQ-backed admin notification queue with Azure Service Bus, preserving the existing
admin notification workflow (`SendNotification` / `ReceiveNotification` / `MarkAsRead`) and
queue-style buffering behavior, while removing the direct `System.Messaging` dependency and the
broad ("Everyone: Full Control") queue permission that MSMQ required.

## Changes Made

### `Services/NotificationService.cs`
- Removed `System.Messaging` / `MessageQueue` usage entirely.
- Replaced the private MSMQ queue (auto-created with `SetPermissions("Everyone", FullControl)`)
  with an **Azure Service Bus queue**, authenticated via **`DefaultAzureCredential`** (Managed
  Identity in Azure, developer credential locally) - no connection string or shared access key is
  used anywhere.
- `ServiceBusClient` / `ServiceBusSender` / `ServiceBusReceiver` are process-wide, lazily-created
  singletons (mirrors the pattern already used by `AzureBlobStorageService` from task 004), since
  they are thread-safe Azure SDK types and a new `NotificationService` is constructed on every MVC
  request (via `BaseController`).
- `SendNotification` now serializes the notification to JSON and sends it as a
  `ServiceBusMessage` via `Sender.SendMessageAsync(...).GetAwaiter().GetResult()`. No routing key
  is used (single plain queue, no topic/subscription filtering), so the message `Subject` is
  intentionally left unset, per the Azure Service Bus migration guidance.
- `ReceiveNotification` preserves the original **destructive dequeue, single-message-per-call**
  semantics of `MessageQueue.Receive(TimeSpan.FromSeconds(1))`: it waits up to 1 second for a
  pending message (`ReceiveMessageAsync(TimeSpan.FromSeconds(1))`), returns `null` if none is
  available (equivalent of the old `IOTimeout` case, but without relying on catching a
  provider-specific exception), and otherwise explicitly `CompleteMessageAsync`s the message before
  returning the deserialized `Notification` - so the admin dashboard's existing "loop calling
  `ReceiveNotification()` until it returns null, capped at 10" pattern in
  `NotificationsController.GetNotifications()` continues to work unchanged.
- `Dispose()` is now a no-op: the Service Bus client/sender/receiver are shared for the process
  lifetime and must not be torn down per-request/per-controller-instance (unlike the old
  per-instance `MessageQueue`).
- The Service Bus namespace/queue name are read from application settings, never hardcoded, and a
  clear `InvalidOperationException` is thrown if the namespace setting is missing (mirrors the
  `AzureBlobStorageService` pattern for the storage account name).

### `Web.config`
- Replaced the `NotificationQueuePath` (`.\Private$\ContosoUniversityNotifications`) appSetting
  with two new settings: `AzureServiceBus:FullyQualifiedNamespace` (placeholder
  `REPLACE-WITH-SERVICEBUS-NAMESPACE.servicebus.windows.net`, to be replaced once the Service Bus
  namespace is provisioned - see `infra/infra-config.md` when present) and
  `AzureServiceBus:NotificationQueueName` (default `contosouniversity-notifications`). Both are
  non-secret application settings, so they flow through the existing `AzureAppConfig` configuration
  builder from task 002 exactly like the other settings in that section.
- Added assembly binding redirects for the three new assemblies: `Azure.Messaging.ServiceBus`
  (7.19.0.0), `Azure.Core.Amqp` (1.3.1.0), and `Microsoft.Azure.Amqp` (2.6.0.0).

### `ContosoUniversity.csproj` / `packages.config`
- Removed the `System.Messaging` framework assembly reference.
- Added NuGet package references (with `packages\...\lib\...` `HintPath`s, consistent with the
  existing non-SDK-style `packages.config` project format): `Azure.Messaging.ServiceBus` 7.19.0,
  `Azure.Core.Amqp` 1.3.1, `Microsoft.Azure.Amqp` 2.6.9. `Azure.Identity` and `System.ClientModel`
  were already present from prior tasks (002/004) and are reused as-is.

### Documentation
- `NOTIFICATION_SYSTEM_README.md` and `SETUP_TESTING_GUIDE.md` rewritten to describe the Azure
  Service Bus-based notification system (provisioning, Managed Identity/RBAC role requirements,
  troubleshooting) instead of MSMQ/Windows Features setup instructions.
- `README.md` prerequisite line updated from "Microsoft Message Queue (MSMQ) Server enabled" to the
  Azure Service Bus namespace/queue + Managed Identity RBAC requirement.

## Infrastructure Configuration

No `infra/` folder exists yet in this repository (the platform-engineer provisioning task has not
run for this task). The `AzureServiceBus:FullyQualifiedNamespace` value is a placeholder
(`REPLACE-WITH-SERVICEBUS-NAMESPACE.servicebus.windows.net`), documented in `Web.config` with a
comment explaining it must be replaced with the real provisioned namespace once
`infra/infra-config.md` exists, and that the app's Managed Identity must be granted the **Azure
Service Bus Data Sender** and **Azure Service Bus Data Receiver** RBAC roles on the queue - a
strictly least-privilege replacement for the old "Everyone: Full Control" MSMQ ACL.

## Security Improvements

- Eliminated the MSMQ-specific `_queue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl)`
  call - the single biggest security finding driving this task. Access to the new Azure Service Bus
  queue is governed entirely by Azure RBAC role assignments scoped to the app's Managed Identity
  (Data Sender/Receiver), not a broad, unauthenticated local-machine ACL.
- No connection string or shared access key is stored in configuration or code -
  `DefaultAzureCredential` is used exclusively (Managed Identity in Azure, developer credential
  locally), per the `migration-managed-identity` / `migration-azure-servicebus` knowledge bases.

## Validation

- **Consistency check**: performed a manual review against the `validation-check-consistency`
  rubric (the `task` sub-agent tool returned a transient `401 Invalid auto-mode selector` error on
  every retry, so the check was carried out directly against the diff). Result: **zero Critical,
  zero Major issues**. One accepted Minor trade-off is documented below.
  - *Minor (accepted, not fixed)*: `SendNotification`/`ReceiveNotification` block on the async
    Azure Service Bus SDK calls via `.GetAwaiter().GetResult()` to preserve the existing synchronous
    `INotificationService` interface consumed by `BaseController`/`NotificationsController` (classic
    ASP.NET MVC, not MVC Core). The Azure SDK internally uses `ConfigureAwait(false)` throughout, so
    the classic ASP.NET sync-over-async deadlock risk is mitigated, but this remains a known
    blocking-async pattern that could add thread-pool pressure under heavy concurrent load. Fully
    resolving this would require making the notification pipeline (interface + both controllers)
    async, which is out of scope for a queue-technology-swap task; flagged here for visibility.
- **Build**: `ContosoUniversity.csproj` builds successfully (0 errors) with the Visual Studio
  MSBuild toolchain (`msbuild ContosoUniversity.csproj /t:Build`).
- **Unit tests**: The sibling `ContosoUniverisity.Tests` project has a **pre-existing** issue
  (present at the baseline commit `a4a63248b23931aa1eeea472eaaef06c583419fa`, first documented in
  the task 002 summary and unrelated to this task) - its `.csproj` does not reference the
  `ContosoUniversity` project/assembly at all, so it fails to compile (`CS0246: The type or
  namespace 'ContosoUniversity' could not be found`) regardless of any change made here. This was
  re-verified to still be the sole failure cause (no new failures introduced by this task's
  changes) and should be raised separately to the plan owner/integration-tester since it currently
  blocks running the unit test suite at all.

## Old Technology Check

No remaining references to `System.Messaging`, `MessageQueue`, or MSMQ exist in source, project, or
configuration files relevant to this task (`Services/NotificationService.cs`,
`ContosoUniversity.csproj`, `packages.config`, `Web.config`, and the notification-related
documentation files). The only remaining "MessageQueue"/"MSMQ" text is inside historical, frozen
assessment reports under `.github/modernize/assessment/**` and the task's own description text in
`.github/modernize/modernization-plan/.metadata/tasks.json`, both of which are out of scope /
must not be modified.

## Files Modified
- `Services/NotificationService.cs`
- `Web.config`
- `ContosoUniversity.csproj`
- `packages.config`
- `packages/` (new NuGet package folders added: `Azure.Messaging.ServiceBus.7.19.0`,
  `Azure.Core.Amqp.1.3.1`, `Microsoft.Azure.Amqp.2.6.9`, plus transitive
  `System.ClientModel`/`System.Memory.Data`/`System.Text.Json` package folders already present at
  higher versions from prior tasks)
- `NOTIFICATION_SYSTEM_README.md`
- `SETUP_TESTING_GUIDE.md`
- `README.md`
- `.github/modernize/modernization-plan/.metadata/tasks.json` (status marker only)
