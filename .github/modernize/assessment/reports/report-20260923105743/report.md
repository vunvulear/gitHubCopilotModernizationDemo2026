# ContosoUniverisity.Tests

## Summary

| Metric | Value |
|--------|-------|
| Total Issues | 12 |
| Mandatory Blockers | 1 |
| Potential Issues | 6 |

## Component Information

| Property | Value |
|----------|-------|
| Language | C# |
| Frameworks | .NETFramework,Version=v4.7.2 |
| Build tools | MSBuild |

## Cloud Readiness Issues

| Issue Name | Criticality | Story Points | Occurrences |
|------------|-------------|--------------|-------------|
| Local or network IO operations detected | Potential | 3 | [3](#Local_or_network_IO_operations_detected) |

### Issue Details

<details id="Local_or_network_IO_operations_detected">
<summary><b>Local or network IO operations detected</b> — affected files</summary>

- `Characterization_FullUnitTests.cs (line 62, col 40)`
- `Characterization_FullUnitTests.cs (line 62, col 17)`
- `Characterization_FullUnitTests.cs (line 63, col 28)`

</details>

## DotNET Upgrade Issues [View Details](scenarios/dotnet-version-upgrade/assessment.md)

| Issue Category | Criticality | Story Points |
|----------------|-------------|--------------|
| Binary incompatible for selected .NET version | Mandatory | 1 |
| NuGet package functionality is included with framework reference | Mandatory | 1 |
| System.Web.Optimization bundling and minification is not supported in .NET Core and should be replaced with actual html tags pointing to content files | Mandatory | 1 |
| Manual redirect conflicts with auto-generated version | Mandatory | 1 |
| Project's target framework(s) needs to be changed | Mandatory | 1 |
| Project file needs to be converted to SDK-style | Mandatory | 1 |
| GlobalFilterCollection is not supported in .NET Core and needs to be converted to the corresponding middleware registrations on the application object | Mandatory | 1 |
| Routes registration via RouteCollection is not supported in .NET Core and needs to be converted to the route mappings on the application object | Mandatory | 1 |
| NuGet package is incompatible | Mandatory | 1 |
| Convert application initialization code from Global.asax.cs to .NET Core and clean up Global.asax.cs | Mandatory | 1 |
| Convert System.Messaging to MSMQ in .NET Core | Mandatory | 1 |
| MSMQ & Message Queuing | Mandatory | 2 |
| Legacy Configuration System | Mandatory | 2 |
| ASP.NET Framework (System.Web) | Mandatory | 4 |
| Source incompatible for selected .NET version | Potential | 1 |
| NuGet package upgrade is recommended | Potential | 1 |
| Binding redirect forces version downgrade | Potential | 1 |
| Library-hosted entry point missing GenerateBindingRedirectsOutputType | Potential | 1 |
| NuGet package is deprecated | Optional | 1 |
| NuGet package contains security vulnerability | Optional | 1 |

### Issue Details

<details>
<summary><b>Binary incompatible for selected .NET version</b> — affected files</summary>

- `Services\NotificationService.cs (line 116, col 12)`
- `Services\NotificationService.cs (line 71, col 12)`
- `Services\NotificationService.cs (line 74, col 16)`
- `Services\NotificationService.cs (line 73, col 16)`
- `Services\NotificationService.cs (line 60, col 16)`
- `Services\NotificationService.cs (line 54, col 16)`
- `Services\NotificationService.cs (line 30, col 12)`
- `Services\NotificationService.cs (line 26, col 16)`
- `Services\NotificationService.cs (line 22, col 16)`
- `Services\NotificationService.cs (line 21, col 16)`
- `Services\NotificationService.cs (line 19, col 12)`
- `Services\NotificationService.cs (line 11, col 38)`
- `Controllers\DepartmentsController.cs (line 151, col 8)`
- `Controllers\DepartmentsController.cs (line 152, col 9)`
- `Controllers\DepartmentsController.cs (line 151, col 19)`
- `Controllers\DepartmentsController.cs (line 151, col 9)`
- `Controllers\DepartmentsController.cs (line 163, col 12)`
- `Controllers\DepartmentsController.cs (line 136, col 8)`
- `Controllers\DepartmentsController.cs (line 147, col 12)`
- `Controllers\DepartmentsController.cs (line 145, col 16)`
- `Controllers\DepartmentsController.cs (line 140, col 16)`
- `Controllers\DepartmentsController.cs (line 80, col 34)`
- `Controllers\DepartmentsController.cs (line 78, col 8)`
- `Controllers\DepartmentsController.cs (line 79, col 9)`
- `Controllers\DepartmentsController.cs (line 78, col 9)`
- `Controllers\DepartmentsController.cs (line 132, col 12)`
- `Controllers\DepartmentsController.cs (line 131, col 12)`
- `Controllers\DepartmentsController.cs (line 121, col 20)`
- `Controllers\DepartmentsController.cs (line 118, col 24)`
- `Controllers\DepartmentsController.cs (line 114, col 24)`
- `Controllers\DepartmentsController.cs (line 112, col 24)`
- `Controllers\DepartmentsController.cs (line 110, col 24)`
- `Controllers\DepartmentsController.cs (line 103, col 20)`
- `Controllers\DepartmentsController.cs (line 92, col 20)`
- `Controllers\DepartmentsController.cs (line 84, col 16)`
- `Controllers\DepartmentsController.cs (line 62, col 8)`
- `Controllers\DepartmentsController.cs (line 74, col 12)`
- `Controllers\DepartmentsController.cs (line 73, col 12)`
- `Controllers\DepartmentsController.cs (line 71, col 16)`
- `Controllers\DepartmentsController.cs (line 66, col 16)`
- `Controllers\DepartmentsController.cs (line 44, col 36)`
- `Controllers\DepartmentsController.cs (line 42, col 8)`
- `Controllers\DepartmentsController.cs (line 43, col 9)`
- `Controllers\DepartmentsController.cs (line 42, col 9)`
- `Controllers\DepartmentsController.cs (line 58, col 12)`
- `Controllers\DepartmentsController.cs (line 57, col 12)`
- `Controllers\DepartmentsController.cs (line 54, col 16)`
- `Controllers\DepartmentsController.cs (line 46, col 12)`
- `Controllers\DepartmentsController.cs (line 35, col 8)`
- `Controllers\DepartmentsController.cs (line 38, col 12)`
- `Controllers\DepartmentsController.cs (line 37, col 12)`
- `Controllers\DepartmentsController.cs (line 20, col 8)`
- `Controllers\DepartmentsController.cs (line 31, col 12)`
- `Controllers\DepartmentsController.cs (line 29, col 16)`
- `Controllers\DepartmentsController.cs (line 24, col 16)`
- `Controllers\DepartmentsController.cs (line 13, col 8)`
- `Controllers\DepartmentsController.cs (line 16, col 12)`
- `Controllers\InstructorsController.cs (line 224, col 8)`
- `Controllers\InstructorsController.cs (line 225, col 9)`
- `Controllers\InstructorsController.cs (line 224, col 19)`
- `Controllers\InstructorsController.cs (line 224, col 9)`
- `Controllers\InstructorsController.cs (line 248, col 12)`
- `Controllers\InstructorsController.cs (line 209, col 8)`
- `Controllers\InstructorsController.cs (line 220, col 12)`
- `Controllers\InstructorsController.cs (line 218, col 16)`
- `Controllers\InstructorsController.cs (line 213, col 16)`
- `Controllers\InstructorsController.cs (line 133, col 8)`
- `Controllers\InstructorsController.cs (line 134, col 9)`
- `Controllers\InstructorsController.cs (line 133, col 9)`
- `Controllers\InstructorsController.cs (line 173, col 12)`
- `Controllers\InstructorsController.cs (line 169, col 20)`
- `Controllers\InstructorsController.cs (line 165, col 20)`
- `Controllers\InstructorsController.cs (line 139, col 16)`
- `Controllers\InstructorsController.cs (line 129, col 12)`
- `Controllers\InstructorsController.cs (line 95, col 8)`
- `Controllers\InstructorsController.cs (line 112, col 12)`
- `Controllers\InstructorsController.cs (line 110, col 16)`
- `Controllers\InstructorsController.cs (line 99, col 16)`
- `Controllers\InstructorsController.cs (line 69, col 36)`
- `Controllers\InstructorsController.cs (line 67, col 8)`
- `Controllers\InstructorsController.cs (line 68, col 9)`
- `Controllers\InstructorsController.cs (line 67, col 9)`
- `Controllers\InstructorsController.cs (line 91, col 12)`
- `Controllers\InstructorsController.cs (line 88, col 16)`
- `Controllers\InstructorsController.cs (line 80, col 12)`
- `Controllers\InstructorsController.cs (line 58, col 8)`
- `Controllers\InstructorsController.cs (line 63, col 12)`
- `Controllers\InstructorsController.cs (line 43, col 8)`
- `Controllers\InstructorsController.cs (line 54, col 12)`
- `Controllers\InstructorsController.cs (line 52, col 16)`
- `Controllers\InstructorsController.cs (line 47, col 16)`
- `Controllers\InstructorsController.cs (line 15, col 8)`
- `Controllers\InstructorsController.cs (line 39, col 12)`
- `Controllers\InstructorsController.cs (line 34, col 16)`
- `Controllers\InstructorsController.cs (line 27, col 16)`
- `Controllers\CoursesController.cs (line 218, col 8)`
- `Controllers\CoursesController.cs (line 219, col 9)`
- `Controllers\CoursesController.cs (line 218, col 19)`
- `Controllers\CoursesController.cs (line 218, col 9)`
- `Controllers\CoursesController.cs (line 250, col 12)`
- `Controllers\CoursesController.cs (line 228, col 16)`
- `Controllers\CoursesController.cs (line 203, col 8)`
- `Controllers\CoursesController.cs (line 214, col 12)`
- `Controllers\CoursesController.cs (line 212, col 16)`
- `Controllers\CoursesController.cs (line 207, col 16)`
- `Controllers\CoursesController.cs (line 129, col 34)`
- `Controllers\CoursesController.cs (line 127, col 8)`
- `Controllers\CoursesController.cs (line 128, col 9)`
- `Controllers\CoursesController.cs (line 127, col 9)`
- `Controllers\CoursesController.cs (line 199, col 12)`
- `Controllers\CoursesController.cs (line 198, col 12)`
- `Controllers\CoursesController.cs (line 196, col 16)`
- `Controllers\CoursesController.cs (line 186, col 24)`
- `Controllers\CoursesController.cs (line 185, col 24)`
- `Controllers\CoursesController.cs (line 184, col 24)`
- `Controllers\CoursesController.cs (line 171, col 28)`
- `Controllers\CoursesController.cs (line 158, col 24)`
- `Controllers\CoursesController.cs (line 152, col 24)`
- `Controllers\CoursesController.cs (line 151, col 24)`
- `Controllers\CoursesController.cs (line 150, col 24)`
- `Controllers\CoursesController.cs (line 144, col 24)`
- `Controllers\CoursesController.cs (line 143, col 24)`
- `Controllers\CoursesController.cs (line 142, col 24)`
- `Controllers\CoursesController.cs (line 131, col 12)`
- `Controllers\CoursesController.cs (line 111, col 8)`
- `Controllers\CoursesController.cs (line 123, col 12)`
- `Controllers\CoursesController.cs (line 122, col 12)`
- `Controllers\CoursesController.cs (line 120, col 16)`
- `Controllers\CoursesController.cs (line 115, col 16)`
- `Controllers\CoursesController.cs (line 46, col 36)`
- `Controllers\CoursesController.cs (line 44, col 8)`
- `Controllers\CoursesController.cs (line 45, col 9)`
- `Controllers\CoursesController.cs (line 44, col 9)`
- `Controllers\CoursesController.cs (line 107, col 12)`
- `Controllers\CoursesController.cs (line 106, col 12)`
- `Controllers\CoursesController.cs (line 103, col 16)`
- `Controllers\CoursesController.cs (line 93, col 24)`
- `Controllers\CoursesController.cs (line 92, col 24)`
- `Controllers\CoursesController.cs (line 91, col 24)`
- `Controllers\CoursesController.cs (line 75, col 24)`
- `Controllers\CoursesController.cs (line 69, col 24)`
- `Controllers\CoursesController.cs (line 68, col 24)`
- `Controllers\CoursesController.cs (line 67, col 24)`
- `Controllers\CoursesController.cs (line 61, col 24)`
- `Controllers\CoursesController.cs (line 60, col 24)`
- `Controllers\CoursesController.cs (line 59, col 24)`
- `Controllers\CoursesController.cs (line 48, col 12)`
- `Controllers\CoursesController.cs (line 37, col 8)`
- `Controllers\CoursesController.cs (line 40, col 12)`
- `Controllers\CoursesController.cs (line 39, col 12)`
- `Controllers\CoursesController.cs (line 22, col 8)`
- `Controllers\CoursesController.cs (line 33, col 12)`
- `Controllers\CoursesController.cs (line 31, col 16)`
- `Controllers\CoursesController.cs (line 26, col 16)`
- `Controllers\CoursesController.cs (line 15, col 8)`
- `Controllers\CoursesController.cs (line 18, col 12)`
- `Controllers\StudentsController.cs (line 198, col 8)`
- `Controllers\StudentsController.cs (line 199, col 9)`
- `Controllers\StudentsController.cs (line 198, col 19)`
- `Controllers\StudentsController.cs (line 198, col 9)`
- `Controllers\StudentsController.cs (line 218, col 16)`
- `Controllers\StudentsController.cs (line 217, col 16)`
- `Controllers\StudentsController.cs (line 212, col 16)`
- `Controllers\StudentsController.cs (line 183, col 8)`
- `Controllers\StudentsController.cs (line 194, col 12)`
- `Controllers\StudentsController.cs (line 192, col 16)`
- `Controllers\StudentsController.cs (line 187, col 16)`
- `Controllers\StudentsController.cs (line 146, col 34)`
- `Controllers\StudentsController.cs (line 144, col 8)`
- `Controllers\StudentsController.cs (line 145, col 9)`
- `Controllers\StudentsController.cs (line 144, col 9)`
- `Controllers\StudentsController.cs (line 179, col 12)`
- `Controllers\StudentsController.cs (line 177, col 16)`
- `Controllers\StudentsController.cs (line 171, col 20)`
- `Controllers\StudentsController.cs (line 162, col 16)`
- `Controllers\StudentsController.cs (line 159, col 20)`
- `Controllers\StudentsController.cs (line 153, col 20)`
- `Controllers\StudentsController.cs (line 129, col 8)`
- `Controllers\StudentsController.cs (line 140, col 12)`
- `Controllers\StudentsController.cs (line 138, col 16)`
- `Controllers\StudentsController.cs (line 133, col 16)`
- `Controllers\StudentsController.cs (line 92, col 36)`
- `Controllers\StudentsController.cs (line 90, col 8)`
- `Controllers\StudentsController.cs (line 91, col 9)`
- `Controllers\StudentsController.cs (line 90, col 9)`
- `Controllers\StudentsController.cs (line 125, col 12)`
- `Controllers\StudentsController.cs (line 123, col 16)`
- `Controllers\StudentsController.cs (line 117, col 20)`
- `Controllers\StudentsController.cs (line 108, col 16)`
- `Controllers\StudentsController.cs (line 105, col 20)`
- `Controllers\StudentsController.cs (line 99, col 20)`
- `Controllers\StudentsController.cs (line 80, col 8)`
- `Controllers\StudentsController.cs (line 86, col 12)`
- `Controllers\StudentsController.cs (line 62, col 8)`
- `Controllers\StudentsController.cs (line 76, col 12)`
- `Controllers\StudentsController.cs (line 74, col 16)`
- `Controllers\StudentsController.cs (line 66, col 16)`
- `Controllers\StudentsController.cs (line 14, col 8)`
- `Controllers\StudentsController.cs (line 58, col 12)`
- `Controllers\StudentsController.cs (line 29, col 12)`
- `Controllers\StudentsController.cs (line 18, col 12)`
- `Controllers\StudentsController.cs (line 17, col 12)`
- `Controllers\StudentsController.cs (line 16, col 12)`
- `Controllers\NotificationsController.cs (line 59, col 8)`
- `Controllers\NotificationsController.cs (line 61, col 12)`
- `Controllers\NotificationsController.cs (line 43, col 8)`
- `Controllers\NotificationsController.cs (line 43, col 9)`
- `Controllers\NotificationsController.cs (line 54, col 16)`
- `Controllers\NotificationsController.cs (line 49, col 16)`
- `Controllers\NotificationsController.cs (line 11, col 8)`
- `Controllers\NotificationsController.cs (line 11, col 9)`
- `Controllers\NotificationsController.cs (line 35, col 12)`
- `Controllers\NotificationsController.cs (line 32, col 16)`
- `Controllers\HomeController.cs (line 40, col 8)`
- `Controllers\HomeController.cs (line 43, col 12)`
- `Controllers\HomeController.cs (line 42, col 12)`
- `Controllers\HomeController.cs (line 35, col 8)`
- `Controllers\HomeController.cs (line 37, col 12)`
- `Controllers\HomeController.cs (line 28, col 8)`
- `Controllers\HomeController.cs (line 32, col 12)`
- `Controllers\HomeController.cs (line 30, col 12)`
- `Controllers\HomeController.cs (line 15, col 8)`
- `Controllers\HomeController.cs (line 25, col 12)`
- `Controllers\HomeController.cs (line 10, col 8)`
- `Controllers\HomeController.cs (line 12, col 12)`
- `Controllers\BaseController.cs (line 44, col 12)`
- `Controllers\BaseController.cs (line 13, col 8)`
- `Controllers\BaseController.cs (line 8, col 43)`
- `Global.asax.cs (line 18, col 12)`
- `Global.asax.cs (line 17, col 12)`
- `Global.asax.cs (line 16, col 12)`
- `Global.asax.cs (line 15, col 12)`
- `App_Start\RouteConfig.cs (line 7, col 8)`
- `App_Start\RouteConfig.cs (line 11, col 12)`
- `App_Start\RouteConfig.cs (line 9, col 12)`
- `App_Start\FilterConfig.cs (line 6, col 8)`
- `App_Start\FilterConfig.cs (line 8, col 12)`
- `App_Start\BundleConfig.cs (line 6, col 8)`
- `App_Start\BundleConfig.cs (line 21, col 12)`
- `App_Start\BundleConfig.cs (line 17, col 12)`
- `App_Start\BundleConfig.cs (line 14, col 12)`
- `App_Start\BundleConfig.cs (line 11, col 12)`
- `App_Start\BundleConfig.cs (line 8, col 12)`

</details>

<details>
<summary><b>NuGet package functionality is included with framework reference</b> — affected files</summary>

- `ContosoUniversity.csproj`

</details>

<details>
<summary><b>System.Web.Optimization bundling and minification is not supported in .NET Core and should be replaced with actual html tags pointing to content files</b> — affected files</summary>

- `Views\Courses\Create.cshtml`
- `Views\Courses\Edit.cshtml`
- `Views\Departments\Create.cshtml`
- `Views\Departments\Edit.cshtml`
- `Views\Instructors\Create.cshtml`
- `Views\Instructors\Edit.cshtml`
- `Views\Shared\_Layout.cshtml`
- `Views\Students\Create.cshtml`
- `Views\Students\Edit.cshtml`
- `App_Start\BundleConfig.cs`

</details>

<details>
<summary><b>Manual redirect conflicts with auto-generated version</b> — affected files</summary>

- `Web.config`

</details>

<details>
<summary><b>Project's target framework(s) needs to be changed</b> — affected files</summary>

- `%USERPROFILE%\source\repos\vunvulear\gitHubCopilotModernizationDemo2026\ContosoUniverisity.Tests\ContosoUniverisity.Tests.csproj`
- `%USERPROFILE%\source\repos\vunvulear\gitHubCopilotModernizationDemo2026\ContosoUniverisity.Tests.UnitTests\ContosoUniverisity.Tests.UnitTests.csproj`
- `ContosoUniversity.csproj`

</details>

<details>
<summary><b>Project file needs to be converted to SDK-style</b> — affected files</summary>

- `%USERPROFILE%\source\repos\vunvulear\gitHubCopilotModernizationDemo2026\ContosoUniverisity.Tests\ContosoUniverisity.Tests.csproj`
- `ContosoUniversity.csproj`

</details>

<details>
<summary><b>GlobalFilterCollection is not supported in .NET Core and needs to be converted to the corresponding middleware registrations on the application object</b> — affected files</summary>

- `Global.asax.cs`
- `App_Start\FilterConfig.cs`

</details>

<details>
<summary><b>Routes registration via RouteCollection is not supported in .NET Core and needs to be converted to the route mappings on the application object</b> — affected files</summary>

- `Global.asax.cs`
- `App_Start\RouteConfig.cs`

</details>

<details>
<summary><b>NuGet package is incompatible</b> — affected files</summary>

- `ContosoUniversity.csproj`

</details>

<details>
<summary><b>Convert application initialization code from Global.asax.cs to .NET Core and clean up Global.asax.cs</b> — affected files</summary>

- `Global.asax.cs`

</details>

<details>
<summary><b>Convert System.Messaging to MSMQ in .NET Core</b> — affected files</summary>

- `Services\NotificationService.cs`

</details>

<details>
<summary><b>Source incompatible for selected .NET version</b> — affected files</summary>

- `Services\NotificationService.cs (line 73, col 16)`
- `Services\NotificationService.cs (line 16, col 12)`
- `Services\IFileStorage.cs (line 13, col 8)`
- `Services\FileStorage.cs (line 25, col 8)`
- `Services\FileStorage.cs (line 28, col 12)`
- `Data\SchoolContextFactory.cs (line 9, col 12)`
- `Controllers\CoursesController.cs (line 228, col 16)`
- `Controllers\CoursesController.cs (line 127, col 8)`
- `Controllers\CoursesController.cs (line 179, col 24)`
- `Controllers\CoursesController.cs (line 171, col 28)`
- `Controllers\CoursesController.cs (line 158, col 24)`
- `Controllers\CoursesController.cs (line 148, col 20)`
- `Controllers\CoursesController.cs (line 138, col 20)`
- `Controllers\CoursesController.cs (line 134, col 16)`
- `Controllers\CoursesController.cs (line 44, col 8)`
- `Controllers\CoursesController.cs (line 86, col 24)`
- `Controllers\CoursesController.cs (line 75, col 24)`
- `Controllers\CoursesController.cs (line 65, col 20)`
- `Controllers\CoursesController.cs (line 55, col 20)`
- `Controllers\CoursesController.cs (line 51, col 16)`
- `Global.asax.cs (line 26, col 12)`
- `Global.asax.cs (line 11, col 34)`

</details>

<details>
<summary><b>NuGet package upgrade is recommended</b> — affected files</summary>

- `ContosoUniversity.csproj`

</details>

<details>
<summary><b>Binding redirect forces version downgrade</b> — affected files</summary>

- `Web.config`

</details>

<details>
<summary><b>Library-hosted entry point missing GenerateBindingRedirectsOutputType</b> — affected files</summary>

- `%USERPROFILE%\source\repos\vunvulear\gitHubCopilotModernizationDemo2026\ContosoUniverisity.Tests\ContosoUniverisity.Tests.csproj`

</details>

<details>
<summary><b>NuGet package is deprecated</b> — affected files</summary>

- `%USERPROFILE%\source\repos\vunvulear\gitHubCopilotModernizationDemo2026\ContosoUniverisity.Tests\ContosoUniverisity.Tests.csproj`
- `ContosoUniversity.csproj`

</details>

<details>
<summary><b>NuGet package contains security vulnerability</b> — affected files</summary>

- `ContosoUniversity.csproj`

</details>

## Security Issues

> **Note:** These issues were generated by AI and may contain inaccuracies or incomplete information. Please review carefully.

| Issue Name | Criticality | Story Points | Files |
|------------|-------------|--------------|-------|
| CWE-502: CWE-502: Deserialization of Untrusted Data | Mandatory | 13 | [1](#CWE-502_CWE-502_Deserialization_of_Untrusted_Data) |
| CWE-662: Improper Synchronization | Potential | 8 | [2](#CWE-662_Improper_Synchronization) |
| CWE-820: Missing Synchronization | Potential | 8 | [2](#CWE-820_Missing_Synchronization) |
| CWE-606: Unchecked Input for Loop Condition | Potential | 3 | [1](#CWE-606_Unchecked_Input_for_Loop_Condition) |
| CWE-778: Insufficient Logging | Potential | 3 | [2](#CWE-778_Insufficient_Logging) |
| CWE-99: CWE-99: Improper Control of Resource Identifiers ('Resource Injection') | Potential | 3 | [1](#CWE-99_CWE-99_Improper_Control_of_Resource_Identifiers_Resource_Injection) |
| CWE-22: Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal') | Optional | 8 | [2](#CWE-22_Improper_Limitation_of_a_Pathname_to_a_Restricted_Directory_Path_Traversal) |
| CWE-732: Incorrect Permission Assignment for Critical Resource | Optional | 5 | [1](#CWE-732_Incorrect_Permission_Assignment_for_Critical_Resource) |
| CWE-23: Relative Path Traversal | Optional | 5 | [2](#CWE-23_Relative_Path_Traversal) |
| CWE-36: Absolute Path Traversal | Optional | 5 | [2](#CWE-36_Absolute_Path_Traversal) |
| CWE-570: Expression is Always False | Optional | 1 | [1](#CWE-570_Expression_is_Always_False) |

### Security Issue Details

<details id="CWE-502_CWE-502_Deserialization_of_Untrusted_Data">
<summary><b>CWE-502: CWE-502: Deserialization of Untrusted Data</b> — affected files</summary>

- `Services\NotificationService.cs`

</details>

<details id="CWE-662_Improper_Synchronization">
<summary><b>CWE-662: Improper Synchronization</b> — affected files</summary>

- `Controllers/BaseController.cs`
- `Services/NotificationService.cs`

</details>

<details id="CWE-820_Missing_Synchronization">
<summary><b>CWE-820: Missing Synchronization</b> — affected files</summary>

- `Controllers/BaseController.cs`
- `Services/NotificationService.cs`

</details>

<details id="CWE-606_Unchecked_Input_for_Loop_Condition">
<summary><b>CWE-606: Unchecked Input for Loop Condition</b> — affected files</summary>

- `Controllers/InstructorsController.cs`

</details>

<details id="CWE-778_Insufficient_Logging">
<summary><b>CWE-778: Insufficient Logging</b> — affected files</summary>

- `Controllers/BaseController.cs`
- `Services/NotificationService.cs`

</details>

<details id="CWE-99_CWE-99_Improper_Control_of_Resource_Identifiers_Resource_Injection">
<summary><b>CWE-99: CWE-99: Improper Control of Resource Identifiers ('Resource Injection')</b> — affected files</summary>

- `Controllers\CoursesController.cs`

</details>

<details id="CWE-22_Improper_Limitation_of_a_Pathname_to_a_Restricted_Directory_Path_Traversal">
<summary><b>CWE-22: Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal')</b> — affected files</summary>

- `Controllers\CoursesController.cs`
- `Views\Courses\Edit.cshtml`

</details>

<details id="CWE-732_Incorrect_Permission_Assignment_for_Critical_Resource">
<summary><b>CWE-732: Incorrect Permission Assignment for Critical Resource</b> — affected files</summary>

- `Services/NotificationService.cs`

</details>

<details id="CWE-23_Relative_Path_Traversal">
<summary><b>CWE-23: Relative Path Traversal</b> — affected files</summary>

- `Controllers\CoursesController.cs`
- `Views\Courses\Edit.cshtml`

</details>

<details id="CWE-36_Absolute_Path_Traversal">
<summary><b>CWE-36: Absolute Path Traversal</b> — affected files</summary>

- `Controllers\CoursesController.cs`
- `Views\Courses\Edit.cshtml`

</details>

<details id="CWE-570_Expression_is_Always_False">
<summary><b>CWE-570: Expression is Always False</b> — affected files</summary>

- `Controllers/StudentsController.cs`

</details>

---

## Codebase Insights

> **Note:** These documents are generated by AI and may contain inaccuracies or incomplete information. Please review carefully.

> **Codebase Insights aren't available yet.**
>
> These documents are generated when assessment runs with **Full analysis** coverage. Re-run the assessment and set `analysisCoverage: full` to enable them.

[Share feedback](https://aka.ms/ghcp-appmod/feedback)
