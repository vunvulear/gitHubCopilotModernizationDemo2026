# Security Assessment Report

**Generated:** 2026-09-23T11:00:11.4374133Z

## Summary

| Metric | Count |
|--------|-------|
| Total Findings | 11 |
| CVE Vulnerabilities | 0 |
| CWE Vulnerabilities | 11 |
| Total Rules Assessed | 59 |
| Rules Passed | 48 |

### By Severity

| Severity | Count |
|----------|-------|
| mandatory | 1 |
| optional | 5 |
| potential | 5 |

## CWE Findings (Code-Level Vulnerabilities)

### CWE-570: Expression is Always False

- **Category:** Code Quality
- **Severity:** optional
- **Story Points:** 1
- **Files:** Controllers/StudentsController.cs

> StudentsController.Details queries the student with Single() at lines 69-72, then checks `if (student == null)` at line 73. Because Single() either returns an entity or throws, the null check is unreachable and always false.

### CWE-606: Unchecked Input for Loop Condition

- **Category:** Code Quality
- **Severity:** potential
- **Story Points:** 3
- **Files:** Controllers/InstructorsController.cs

> InstructorsController.Create iterates directly over the request-bound `selectedCourses` array at lines 72-79 with no cap on the number of items. An oversized submission can drive unbounded loop iterations from untrusted input.

### CWE-662: Improper Synchronization

- **Category:** Concurrency & Synchronization
- **Severity:** potential
- **Story Points:** 8
- **Files:** Controllers/BaseController.cs, Services/NotificationService.cs

> BaseController creates a new NotificationService per controller instance, and NotificationService.NotificationService() (lines 14-32) checks MessageQueue.Exists(_queuePath) and then creates/opens the same queue without any synchronization. Concurrent requests during startup can race on the shared MSMQ resource.

### CWE-820: Missing Synchronization

- **Category:** Concurrency & Synchronization
- **Severity:** potential
- **Story Points:** 8
- **Files:** Controllers/BaseController.cs, Services/NotificationService.cs

> The application uses the MSMQ queue from multiple request paths via BaseController's per-request NotificationService instance, but NotificationService.NotificationService() performs queue existence and creation operations with no synchronization or coordination around the shared queue path.

### CWE-732: Incorrect Permission Assignment for Critical Resource

- **Category:** Credentials & Secrets
- **Severity:** optional
- **Story Points:** 5
- **Files:** Services/NotificationService.cs

> In NotificationService() the queue is created with `_queue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);` (line 23), granting every user full control over the notification queue resource.

### CWE-778: Insufficient Logging

- **Category:** Credentials & Secrets
- **Severity:** potential
- **Story Points:** 3
- **Files:** Controllers/BaseController.cs, Services/NotificationService.cs

> BaseController.SendEntityNotification() hardcodes `userName = "System"` before recording entity create/update/delete notifications, and NotificationService.SendNotification() stores `CreatedBy = userName ?? "System"` (lines 28-29 and 50). The resulting audit record omits the actual actor for security-relevant changes.

### CWE-22: Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal')

- **Category:** File & Path Security
- **Severity:** optional
- **Story Points:** 8
- **Files:** Controllers\CoursesController.cs, Views\Courses\Edit.cshtml

> Controllers.CoursesController.Edit/DeleteConfirmed accept the client-controlled TeachingMaterialImagePath value (bound on the Course model and posted back via the hidden field in Views\Courses\Edit.cshtml:64) and pass it directly to Server.MapPath at Controllers\CoursesController.cs:172 and 229 before deleting the file. No normalization or directory restriction is applied, so crafted path elements can resolve outside the intended upload directory.

### CWE-23: Relative Path Traversal

- **Category:** File & Path Security
- **Severity:** optional
- **Story Points:** 5
- **Files:** Controllers\CoursesController.cs, Views\Courses\Edit.cshtml

> Controllers.CoursesController.Edit/DeleteConfirmed use the posted TeachingMaterialImagePath from the edit form (Views\Courses\Edit.cshtml:64) and resolve it with Server.MapPath at Controllers\CoursesController.cs:172 and 229. Because the value is taken from the client without validation, relative traversal sequences such as .. can be interpreted outside the upload root.

### CWE-36: Absolute Path Traversal

- **Category:** File & Path Security
- **Severity:** optional
- **Story Points:** 5
- **Files:** Controllers\CoursesController.cs, Views\Courses\Edit.cshtml

> Controllers.CoursesController.Edit/DeleteConfirmed take the user-supplied TeachingMaterialImagePath from the bound model and hidden edit-field, then call Server.MapPath on it at Controllers\CoursesController.cs:172 and 229. Absolute virtual-path input is not rejected before resolution, so an attacker can supply a path that maps outside the intended directory.

### CWE-99: CWE-99: Improper Control of Resource Identifiers ('Resource Injection')

- **Category:** Injection Attacks
- **Severity:** potential
- **Story Points:** 3
- **Files:** Controllers\CoursesController.cs

> CoursesController.Edit() binds TeachingMaterialImagePath from the POST body and later passes course.TeachingMaterialImagePath to Server.MapPath() and file deletion logic (around lines 130-181 and 227-235). A crafted value can steer file resolution to an unintended server resource.

### CWE-502: CWE-502: Deserialization of Untrusted Data

- **Category:** Injection Attacks
- **Severity:** mandatory
- **Story Points:** 13
- **Files:** Services\NotificationService.cs

> NotificationService.ReceiveNotification() reads message bodies from MSMQ and directly calls JsonConvert.DeserializeObject<Notification>(jsonContent) (around lines 70-76). The queue is created with broad permissions, so the deserialized payload can be attacker-controlled.

