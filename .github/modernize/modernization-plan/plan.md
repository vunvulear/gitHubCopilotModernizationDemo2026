# Modernization Plan: Contoso University Azure Migration

**Project**: ContosoUniversity

---

## Technical Framework

- **Language**: C# / .NET Framework 4.8
- **Framework**: ASP.NET MVC 5, Entity Framework Core 3.1.32
- **Build Tool**: MSBuild + NuGet packages.config
- **Database**: SQL Server LocalDB
- **Key Dependencies**: Microsoft.AspNet.Mvc 5.2.9, EF Core, Microsoft.Data.SqlClient, Newtonsoft.Json, MSMQ, Bootstrap, jQuery

---

## Overview

This migration moves Contoso University from a machine-bound MVC web app to Azure-managed services. The app currently depends on LocalDB, MSMQ, local file uploads, and Windows authentication. The target state replaces those dependencies with Azure services, centralizes configuration and secrets, and aligns access with Microsoft Entra ID.

The migration follows a staged path:

- remove Azure blockers in identity and configuration
- move stateful dependencies to Azure services
- close security and operational gaps before cutover

---

## Migration Impact Summary

| Application | Original Service | New Azure Service | Authentication | Comments |
|-------------|------------------|-------------------|----------------|----------|
| ContosoUniversity | IIS Express / Windows Auth | Azure App Service | Microsoft Entra ID | Web hosting and sign-in |
| ContosoUniversity | SQL LocalDB | Azure SQL Database | Managed Identity | EF Core data layer |
| ContosoUniversity | MSMQ | Azure Service Bus | Managed Identity | Admin notifications |
| ContosoUniversity | Local disk uploads | Azure Blob Storage | Managed Identity | Teaching materials |
| ContosoUniversity | Web.config settings | Azure App Configuration + Key Vault | Managed Identity | Settings and secrets |

---

## Risks & Blockers

- Windows authentication is not supported on the intended Azure app hosting model as-is.
- MSMQ cannot be carried forward directly; notification semantics need redesign and business approval.
- File uploads currently depend on local paths and `Server.MapPath`, which will not survive cloud hosting unchanged.
- Current configuration keeps connection details in Web.config and needs secure externalization.
- The assessment flagged path traversal, untrusted deserialization, and weak queue permissions around the current implementation.

---

## Missing Automated Tests & Validation Gaps

- No Azure-specific integration coverage exists for SQL, Service Bus, Blob Storage, or Entra ID.
- Existing characterization/unit tests do not validate cloud deployment, cutover, or rollback behavior.
- No automated checks cover upload path normalization, blob lifecycle, or queue failure handling.
- No end-to-end smoke tests verify that admin flows still work after identity changes.

---

## Phased Migration Sequence

1. Confirm the Azure hosting and identity model, then remove Windows-auth dependency assumptions.
2. Externalize configuration and secrets so cloud services can be wired securely.
3. Move the EF Core data layer from LocalDB to Azure SQL Database.
4. Move teaching-material uploads from local disk to Azure Blob Storage.
5. Replace MSMQ notifications with Azure Service Bus.
6. Remediate dependency CVEs and close remaining security findings.
7. Cut over traffic to Azure and retire the local machine dependencies.

---

## Decisions Requiring Human Input

- Which hosting target should be used if Windows-specific behavior must remain available.
- Whether Microsoft Entra ID should fully replace Windows authentication.
- Azure region, backup, retention, and data residency requirements.
- Notification guarantees needed for admin alerts: ordering, retry, and audit retention.
- Whether uploaded teaching materials must remain private by default and how long they should be retained.
- Whether seeded sample data should exist only in dev/test or also in production cutover.

