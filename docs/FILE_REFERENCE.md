# GCI Admin Web Portal - File Reference Catalog

This document lists and describes every core file in the GCI Admin Web Portal project (`GCI_Admin`).

---

## 📂 Root Project Configuration Files

- **`Program.cs`**: The main entry point for the web server. Registers dependency injections, sets up Cookie authentication, configures logger instances, database contexts, and maps MVC routing patterns.
- **`appsettings.json` / `appsettings.Development.json`**: JSON config files housing database connection strings, logging policies, JWT settings, and third-party API configurations.
- **`GCI_Admin.csproj`**: MSBuild project file defining target framework version (.NET 8.0) and referencing NuGet packages (e.g. Entity Framework Core, SQL Server, Cookie authentication libraries).

---

## 📂 Controllers Layer (`Controllers/`)
Handles incoming HTTP requests, invokes appropriate services, and returns CSHTML views or API payloads.

- **`AuthController.cs`**: Processes administrative logins, claims generation, cookie creation, and password recovery triggers.
- **`AssembliesController.cs`**: Manages creation, editing, location mappings, and head pastor assignments for local assemblies.
- **`FinanceController.cs`**: Validates collection lists, renders digital signatures, and audits cash reports.
- **`ReportsController.cs`**: Aggregates deacons observations logs and filters reports by date/assembly.
- **`BenevolenceController.cs`**: Handles review, attachment download, and state transitions of welfare applications.
- **`EventController.cs`**: Manages church events rosters, Paid/Free ticket sales records, and attendee registers.
- **`AnnouncementsController.cs`**: Manages home dashboard announcement banners and image file uploads.
- **`MembersController.cs`**: Queries registered church member files.
- **`MinistriesController.cs`**: Handles ministries directories and coordinates assignments of ministry heads.
- **`GrowthCentersController.cs` / `RcpsController.cs`**: Manages cell group registers and Regional Cluster (RCPS) groups.
- **`SystemConfigController.cs`**: Dynamic variables configurations management.

---

## 📂 Services Layer (`Services/`)
Houses business rule interfaces and implementations.

- **`IService/IAssembliesService.cs`**: Declares assembly CRUD operations, status toggles, and leadership assignments.
- **`Service/AssembliesService.cs`**: Implements assembly operations, calling DBContext methods to write changes.
- **`IService/IMinistriesService.cs`**: Declares ministry setup methods and leader enlistments.
- **`Service/MinistriesService.cs`**: Implements ministry operations.
- **`IService/IGECMemberService.cs` / `Service/GECMemberService.cs`**: Manages General Executive Council member records.

---

## 📂 Database Operations Layer (`DBOperations/`)
Manages Entity Framework Core contexts, entities mapping, and repositories.

- **`ApplicationDbContext.cs`**: Core DbContext representing the database session. Maps classes to tables: `Assemblies`, `Members`, `Ministries`, `DeaconReports`, `BenevolenceClaims`, and `ServiceCollections`.
- **`Repositories/LeadershipRepository.cs`**: Abstract queries fetching elder directories and leadership credentials.
- **`Repositories/GECMemberRepository.cs`**: Manages queries for council members.
- **`Migrations/`**: Auto-generated files tracking SQL database schema changes.

---

## 📂 Models & DTOs Layer (`Models/`)
Contains persistent entity definitions and Data Transfer Objects (DTOs) used to parse input forms.

- **`Assembly.cs` / `AssemblyLeader.cs`**: Models for assemblies and their designated leaders.
- **`Ministry.cs` / `MinistryLeader.cs`**: Models for church ministries and leaders.
- **`DeaconReport.cs`**: Observation details for service reports.
- **`BenevolenceClaim.cs`**: Welfare cover application parameters.
- **`DTOs/`**: Classes containing validated validation annotations (`[Required]`, `[EmailAddress]`) used to parse inputs from controllers:
  - `AssemblyDto.cs`
  - `MinistryDto.cs`
  - `EventDto.cs`

---

## 📂 Razor Views Layer (`Views/`)
Markup pages detailing how data is rendered on browser screens.

- **`Views/Shared/_Layout.cshtml`**: Base application master layout template housing CSS styles, script tags, top bar, and sidebar navigation menus.
- **`Views/Home/Index.cshtml`**: The main analytics dashboard. Displays statistical metrics cards, collections diagrams, and warnings grids.
- **`Views/GrowthCenters/Index.cshtml`**: Interface listing all cell groups.
- **`Views/GrowthCenters/_CreateGCLeaderPartial.cshtml`**: Modal form template to quickly assign leaders to a cell group.
- **`Views/Event/Index.cshtml`**: Lists planned church events and bookings buttons.
- **`Views/Event/EventRegistrations.cshtml`**: Tables showing registered event attendees and booking payment statuses.
- **`Views/Leadership/_EldersTable.cshtml`**: Visual component rendering current church board elders.
