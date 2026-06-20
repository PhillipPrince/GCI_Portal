# GCI Admin Web Portal - Functional Specification Document

This document defines the functional specifications, operational business rules, and administrative workflows governing the GCI Admin Web Application (`GCI_Admin`).

---

## 1. Executive Summary & Administrative Goals

The GCI Admin Web Portal is the central management interface for Gospel Centres International (GCI). It serves as the primary system of record for church governance, coordinating the actions of global administrators, pastors, and finance auditors. 

The portal's primary objectives are:
- **Centralized Administration**: Manage assemblies, branches, ministries, and user accounts.
- **Financial Accountability**: Verify and audit weekly service collections against digital signatures captured from compiling deacons.
- **Welfare Coordination**: Process benevolence and welfare claims submitted by members, review supporting files, and track disbursement states.
- **Pastoral Oversight**: Aggregates weekly service reports (Tuesday, Thursday, Friday, Sunday) to flag and action pastoral care requirements.
- **Global Communication**: Push announcements and real-time alerts to the GCI Mobile App user base.

---

## 2. User Roles & Authorization Models

The web portal enforces Role-Based Access Control (RBAC) via claims-based identity validation:

1. **Global Administrator**:
   - Full read/write access to all administrative modules.
   - User account deletion, password resets, and role assignments.
   - Configures global settings, Terms & Conditions, and API parameters.
2. **Finance Auditor**:
   - Access to the Finance Dashboard and Service Collections portal.
   - Validates service cash records against deacons' digital signatures.
   - Approves collection ledger postings.
3. **Assembly Pastor / Leader**:
   - View-only access to global tables; full access to reports and members associated with their home assembly.
   - Analyzes weekly deacon reports and follows up on pastoral care flags.
4. **Ministry/Growth Center Supervisor**:
   - Administers specific ministry rosters, monitors cell group leader allocations, and reviews attendance statistics.

---

## 3. Detailed Functional Modules

### 3.1. User & Account Management (`AuthController`, `DeleteUserController`, `RolesController`)
- **Web Sign-in**: Secure login page enforcing credential checks.
- **Role Assignment**: Admins can assign and update roles of registered church staff.
- **Account Erasure**: Supports soft or hard deletion of user accounts to comply with privacy policies and personal data erasure requests.
- **Terms & Conditions Manager**: Interface to update the Terms and Conditions and Privacy Policy agreements displayed inside the mobile app.

### 3.2. Assemblies & Branches Management (`AssembliesController`, `BranchesController`, `BranchLeadersController`)
- **Assemblies Directory**: CRUD interface to register, update, and manage local church assemblies (e.g. coordinates, physical location, contact address, active status).
- **Assembly Leadership**: Maps Head Pastors and Deacons to specific assemblies.
- **Branch Hierarchies**: Manages regional branches and groups individual assemblies under their corresponding branch nodes.

### 3.3. Members & Ministries Administration (`MembersController`, `MinistriesController`)
- **Global Member Database**: View and search all registered church members. Filter by Home Assembly, age groups, and ministry affiliations.
- **Ministries Setup**: Manage ministerial departments (e.g. Choir, Media/AV, Ushers, Youth Ministry).
- **Roster & Leadership**: Assigns Department Heads (Ministry Leaders) and reviews active member enlistments.

### 3.4. Growth Centers & RCPS (`GrowthCentersController`, `RcpsController`)
- **Cell Group Directory**: Manage Growth Centers, verify locations, and assign Cell Leaders.
- **RCPS Clusters**: Configure Regional Cluster Groups (RCPS), manage cell-to-cluster mapping, and review group sizes.

### 3.5. Events & Feedback Portal (`EventController`)
- **Event Planning**: Create and schedule church events (title, banners, descriptions, dates, and venue coordinates).
- **Ticketing & Bookings**: Setup paid or free ticket classes. Connects with the payment gateway to track booking fees and ticket purchase statistics.
- **Engagement Audits**: Review event feedback submissions and ratings filled out by attendees.

### 3.6. Announcements & Notifications (`AnnouncementsController`, `NotificationsController`)
- **Announcement Banner Builder**: Compose and publish church-wide announcements. Supports uploading image banners that display on the mobile app home dashboard.
- **Push Notification Dispatcher**: Compose custom messages and send push alerts instantly or schedule them to target specific groups (e.g., all members, leaders only, or specific assemblies) via Firebase Cloud Messaging.

### 3.7. Benevolence Claims Processing (`BenevolenceController`)
- **Welfare Review Panel**: Central repository for benevolence cover claims submitted via mobile.
- **Verification Pipeline**:
  - Review claimant details, claim explanations, and download uploaded verification files (medical bills, bereavement documents, etc.).
  - Update claim status: `Pending` -> `Under Review` -> `Approved` -> `Disbursed`.
  - Input review notes. Saving changes triggers an async push notification to the applicant's mobile device.

### 3.8. Finance & Service Audit (`FinanceController`, `ReportsController`)
- **Weekly Deacon Reports**:
  - Consolidates weekly reports submitted by deacons (Tuesday prayers, Thursday Bible studies, Friday kesha vigil, Sunday services).
  - Highlights "Pastoral Flags" (sick members, home visit requests, testimonies) for immediate pastoral assignment.
- **Service Collections Validation**:
  - Review cash collections reported by deacons for each service.
  - Displays the digital signatures drawn by the compiling deacons on-screen to prevent unauthorized edits.
  - Finance auditors review, flag discrepancies, and click **Approve & Finalize** to post audited collections.

### 3.9. System Settings (`SystemConfigController`, `SettingsController`)
- **Dynamic Configs**: Real-time management of system variables (e.g., payment API credentials, SMS endpoints, system maintenance mode).
- **Diagnostics**: Access log viewers and system performance parameters.

---

## 4. Security & Data Integrity

- **Cookie Authentication**: Authenticated sessions are secured using encrypted, HttpOnly session cookies.
- **CSRF Protection**: All form submissions enforce Cross-Site Request Forgery (`[ValidateAntiForgeryToken]`) verification.
- **Data Isolation**: User actions are constrained by assembly permissions to prevent staff from viewing or editing records belonging to other assemblies.
- **Personal Data Security**: Auditing logs track changes to critical fields (e.g., financial ledger entries, user deletions).
