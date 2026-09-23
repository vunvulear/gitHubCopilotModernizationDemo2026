# Projects and dependencies analysis

This document provides a comprehensive overview of the projects and their dependencies in the context of upgrading to .NETCoreApp,Version=v10.0.

Detailed findings live alongside this file in `assessment/`. This page is the index: read it first, then open only the documents you need.

## Table of Contents

- [Executive Summary](#executive-summary)
  - [Highlevel Metrics](#highlevel-metrics)
  - [Projects Compatibility](#projects-compatibility)
  - [Package Compatibility](#package-compatibility)
  - [API Compatibility](#api-compatibility)
  - [Binding Redirect Configuration](#binding-redirect-configuration)
- [Top API Migration Challenges](#top-api-migration-challenges)
  - [Technologies and Features](#technologies-and-features)
  - [Most Frequent API Issues](#most-frequent-api-issues)
- [Detailed Reports](#detailed-reports)
  - [Projects Relationship Graph](assessment/project-graph.md)
  - [Aggregate NuGet packages details](assessment/nuget/aggregate-packages.md)
  - [Most Frequent API Issues (complete list)](assessment/api-issues/most-frequent-api-issues.md)
  - [Project Details](#project-details)

## Executive Summary

### Highlevel Metrics

| Metric | Count | Status |
| :--- | :---: | :--- |
| Total Projects | 3 | All require upgrade |
| Total NuGet Packages | 45 | 26 need upgrade |
| Total Code Files | 63 |  |
| Total Code Files with Incidents | 28 |  |
| Total Lines of Code | 3851 |  |
| Total Number of Issues | 649 |  |
| Proposed Target Framework | net10.0 |  |
| Estimated LOC to modify | 572+ | at least 14.9% of codebase |

### Projects Compatibility

| Project | Target Framework | Difficulty | Package Issues | API Issues | Binding Issues | Est. LOC Impact | Description |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| [%USERPROFILE%\source\repos\vunvulear\gitHubCopilotModernizationDemo2026\ContosoUniverisity.Tests.UnitTests\ContosoUniverisity.Tests.UnitTests.csproj](assessment/projects/ContosoUniverisity.Tests.UnitTests.md) | net472 | 🟢 Low | 0 | 0 | 0 |  | ClassLibrary, Sdk Style = True |
| [%USERPROFILE%\source\repos\vunvulear\gitHubCopilotModernizationDemo2026\ContosoUniverisity.Tests\ContosoUniverisity.Tests.csproj](assessment/projects/ContosoUniverisity.Tests.md) | net472 | 🟢 Low | 2 | 0 | 1 |  | ClassicClassLibrary, Sdk Style = False |
| [ContosoUniversity.csproj](assessment/projects/ContosoUniversity.md) | net48 | 🔴 High | 41 | 572 | 12 | 572+ | Wap, Sdk Style = False |

### Package Compatibility

| Status | Count | Percentage |
| :--- | :---: | :---: |
| ✅ Compatible | 19 | 42.2% |
| ⚠️ Incompatible | 2 | 4.4% |
| 🔄 Upgrade Recommended | 24 | 53.3% |
| ***Total NuGet Packages*** | ***45*** | ***100%*** |

### API Compatibility

| Category | Count | Impact |
| :--- | :---: | :--- |
| 🔴 Binary Incompatible | 532 | High - Require code changes |
| 🟡 Source Incompatible | 40 | Medium - Needs re-compilation and potential conflicting API error fixing |
| 🔵 Behavioral change | 0 | Low - Behavioral changes that may require testing at runtime |
| ✅ Compatible | 2297 |  |
| ***Total APIs Analyzed*** | ***2869*** |  |

### Binding Redirect Configuration

| Severity | Count | Description |
| :--- | :---: | :--- |
| 🔴Mandatory | 6 | Must be fixed to avoid runtime failures |
| 🟡Potential | 7 | May cause issues in certain scenarios |
| ***Total Binding Issues*** | ***13*** | ***Across 2 project(s)*** |

## Top API Migration Challenges

### Technologies and Features

| Technology | Issues | Percentage | Migration Path |
| :--- | :---: | :---: | :--- |
| ASP.NET Framework (System.Web) | 498 | 87.1% | Legacy ASP.NET Framework APIs for web applications (System.Web.*) that don't exist in ASP.NET Core due to architectural differences. ASP.NET Core represents a complete redesign of the web framework. Migrate to ASP.NET Core equivalents or consider System.Web.Adapters package for compatibility. |
| MSMQ & Message Queuing | 57 | 10.0% | Microsoft Message Queue (MSMQ) APIs for Windows-based message queuing that are not supported in .NET Core/.NET. MSMQ is a Windows-specific technology. Migrate to RabbitMQ, Azure Service Bus, or other modern message queues. |
| Legacy Configuration System | 16 | 2.8% | Legacy XML-based configuration system (app.config/web.config) that has been replaced by a more flexible configuration model in .NET Core. The old system was rigid and XML-based. Migrate to Microsoft.Extensions.Configuration with JSON/environment variables; use System.Configuration.ConfigurationManager NuGet package as interim bridge if needed. |

### Most Frequent API Issues

| API | Count | Percentage | Category |
| :--- | :---: | :---: | :--- |
| T:System.Web.Mvc.ViewResult | 40 | 7.0% | Binary Incompatible |
| T:System.Web.Mvc.ActionResult | 38 | 6.6% | Binary Incompatible |
| M:System.Web.Mvc.Controller.View(System.Object) | 34 | 5.9% | Binary Incompatible |
| P:System.Web.Mvc.Controller.ModelState | 26 | 4.5% | Binary Incompatible |
| T:System.Web.Mvc.ModelStateDictionary | 26 | 4.5% | Binary Incompatible |
| P:System.Web.Mvc.ControllerBase.ViewBag | 23 | 4.0% | Binary Incompatible |
| M:System.Web.Mvc.ModelStateDictionary.AddModelError(System.String,System.String) | 19 | 3.3% | Binary Incompatible |
| T:System.Messaging.MessageQueue | 18 | 3.1% | Binary Incompatible |
| T:System.Web.Mvc.SelectList | 14 | 2.4% | Binary Incompatible |
| M:System.Web.Mvc.Controller.RedirectToAction(System.String) | 13 | 2.3% | Binary Incompatible |

The table above is the top 10. See [the complete list](assessment/api-issues/most-frequent-api-issues.md) for every affected API.

## Detailed Reports

- [Projects Relationship Graph](assessment/project-graph.md)
- [Aggregate NuGet packages details](assessment/nuget/aggregate-packages.md)
- [Most Frequent API Issues (complete list)](assessment/api-issues/most-frequent-api-issues.md)

### Project Details

- [%USERPROFILE%\source\repos\vunvulear\gitHubCopilotModernizationDemo2026\ContosoUniverisity.Tests.UnitTests\ContosoUniverisity.Tests.UnitTests.csproj](assessment/projects/ContosoUniverisity.Tests.UnitTests.md)
- [%USERPROFILE%\source\repos\vunvulear\gitHubCopilotModernizationDemo2026\ContosoUniverisity.Tests\ContosoUniverisity.Tests.csproj](assessment/projects/ContosoUniverisity.Tests.md)
- [ContosoUniversity.csproj](assessment/projects/ContosoUniversity.md)


