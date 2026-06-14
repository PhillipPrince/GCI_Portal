# Gospel Centres International (GCI) Admin Web Portal

This repository contains the **GCI Admin Web Portal**, an ASP.NET Core MVC web application designed for global administrators, pastors, and auditors to manage the Gospel Centres International church network, coordinate ministries, audit financial collections, approve welfare/benevolence applications, and review weekly deacons' reports.

---

## 📖 System Documentation

We have prepared comprehensive guides for the GCI Admin Web Portal located in the [docs](file:///C:/Users/USER/Projects/GCI/GCI_Portal/docs) directory:

- **[Functional Specification](file:///C:/Users/USER/Projects/GCI/GCI_Portal/docs/FUNCTIONAL_SPECIFICATION.md)**: Defines the functional requirements, workflows, authentication mechanisms, and RBAC rules governing the web application.
- **[User Manual](file:///C:/Users/USER/Projects/GCI/GCI_Portal/docs/USER_MANUAL.md)**: A step-by-step user guide for church administrators, pastors, and auditors to execute daily administrative tasks.
- **[Developer Documentation](file:///C:/Users/USER/Projects/GCI/GCI_Portal/docs/DEVELOPER_DOCUMENTATION.md)**: Details system architecture, dependency injection registration, database Entity Framework configurations, cookie/claims security rules, and local setup instructions.
- **[File Reference Catalog](file:///C:/Users/USER/Projects/GCI/GCI_Portal/docs/FILE_REFERENCE.md)**: A comprehensive directory-by-directory catalog documenting every Controller, Service, Repository, and View in the codebase.

---

## ⚡ Quick Start for Developers

### Prerequisites
- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- SQL Relational Database Server (SQL Server, LocalDB, PostgreSQL, etc.)
- IDE (Visual Studio 2022, VS Code, or Rider)

### Setup & Run
1. Restore NuGet dependencies:
   ```powershell
   dotnet restore
   ```
2. Update the connection strings in [appsettings.json](file:///C:/Users/USER/Projects/GCI/GCI_Portal/GCI_Admin/appsettings.json) to point to your local database instance.
3. Apply Entity Framework migrations:
   ```powershell
   dotnet ef database update
   ```
4. Start the application:
   ```powershell
   dotnet run --project GCI_Admin
   ```
5. Open your browser and navigate to `https://localhost:5001` (or the configured HTTPS port).
