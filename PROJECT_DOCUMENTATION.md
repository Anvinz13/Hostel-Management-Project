# Technical Documentation: Sunshine Hostel Management System

**Document Version:** 2.0  
**Framework Target:** ASP.NET Core 10 (`net10.0`)  
**Target Environment:** Microsoft SQL Server 2016+ / Express / LocalDB  

---

## 1. System Architecture

The Sunshine Hostel Management System follows the ASP.NET Core Model-View-Controller (MVC) architectural pattern, utilizing scoped dependency injection, HTTP session management, and thread-safe ADO.NET with `Microsoft.Data.SqlClient`.

```
                    ┌──────────────────────────────────────────┐
                    │               Web Browser                │
                    │   (Responsive Bootstrap 5 UI / CSS)      │
                    └─────────────────────┬────────────────────┘
                                          │ HTTP / HTTPS Requests
                                          ▼
                    ┌──────────────────────────────────────────┐
                    │          ASP.NET Core 10 Host            │
                    │  ├── Middleware (Routing, HSTS, Session) │
                    │  ├── HttpContext.Session                 │
                    │  └── Dependency Injection Container      │
                    └─────────────────────┬────────────────────┘
                                          │
                 ┌────────────────────────┴────────────────────────┐
                 ▼                                                 ▼
    ┌───────────────────────────┐                     ┌───────────────────────────┐
    │    Student Controllers    │                     │     Admin Controllers     │
    │  - LoginController        │                     │  - AdminhomeController    │
    │  - UserhomeController     │                     │  - AddroomController      │
    │  - viewroomsController    │                     │  - RequestController      │
    │  - RequestController      │                     │  - AdminregController     │
    │  - UserregController      │                     └─────────────┬─────────────┘
    └────────────┬──────────────┘                                   │
                 │                                                 │
                 └────────────────────────┬────────────────────────┘
                                          ▼
                    ┌──────────────────────────────────────────┐
                    │           Dbcls Data Service             │
                    │  (Scoped DI, Parameterized Queries,      │
                    │   Connection Pooling, Stored Procedures) │
                    └─────────────────────┬────────────────────┘
                                          │
                                          ▼
                    ┌──────────────────────────────────────────┐
                    │         SQL Server Database              │
                    │  [Admintab], [Usertab], [Logintab],      │
                    │  [Roomtab], [Requesttab]                 │
                    └──────────────────────────────────────────┘
```

---

## 2. Key Architecture Upgrades (.NET 10)

1. **Target Framework Modernization**: Upgraded from `net8.0` to `net10.0` with C# 13 language features and SDK 10.0.302.
2. **Modern SQL Driver**: Migrated from legacy `System.Data.SqlClient` to modern `Microsoft.Data.SqlClient 6.0.1` with patched security dependencies (`System.Security.Cryptography.Pkcs 9.0.23`).
3. **Connection Pooling & Thread Safety**: Eliminated single shared connection instance field in `Dbcls`. Every database operation now creates a scoped connection (`using var con = CreateConnection()`) ensuring thread safety during concurrent requests.
4. **Session State Management**: Registered distributed memory caching and HTTP session state in `Program.cs`. User identity (`uid`), role (`logtype`), and `username` persist across redirects, solving the request-loss bug present in the previous `TempData` approach.
5. **Dependency Injection**: Registered `Dbcls` as a scoped service in the ASP.NET Core service container, enabling constructor injection across all controllers.
6. **Robust Error Handling**: Replaced `throw ex;` antipatterns with clean exception handling and `throw;` to preserve original stack traces.

---

## 3. Database Specification

### 3.1 Tables

#### 1. `Admintab`
- **Purpose**: Stores administrator profile records.
- **Columns**:
  - `id` (INT, PRIMARY KEY): Unique identifier.
  - `admin_name` (VARCHAR(100)): Full name.
  - `admin_phone` (VARCHAR(20)): Contact phone number.
  - `admin_email` (VARCHAR(100)): Official email address.

#### 2. `Usertab`
- **Purpose**: Stores student resident profile records.
- **Columns**:
  - `id` (INT, PRIMARY KEY): Unique student registration identifier.
  - `name` (VARCHAR(100)): Full name.
  - `gender` (VARCHAR(20)): Gender (`Male`, `Female`, `Other`).
  - `age` (INT): Resident age.
  - `phone` (VARCHAR(20)): Contact phone number.
  - `email` (VARCHAR(100)): Email address.
  - `address` (VARCHAR(255)): Permanent residence address.
  - `join_date` (DATETIME): Date of joining the hostel.

#### 3. `Logintab`
- **Purpose**: Centralized authentication store for all users.
- **Columns**:
  - `rid` (INT): Foreign registration reference linking to `Admintab.id` or `Usertab.id`.
  - `username` (VARCHAR(100)): Unique login name.
  - `password` (VARCHAR(100)): User password.
  - `logintype` (VARCHAR(50)): Role discriminator (`Admin` or `User`).

#### 4. `Roomtab`
- **Purpose**: Hostel room inventory and category management.
- **Columns**:
  - `Room_id` (INT, IDENTITY(1,1), PRIMARY KEY): Unique room identifier.
  - `Rent` (VARCHAR(50)): Monthly rent per bed (₹).
  - `Typeofsharing` (VARCHAR(100)): Room category (e.g., `Single Room (AC)`, `2-Sharing (Non-AC)`).
  - `Occupied` (VARCHAR(50)): Current count of occupied beds.

#### 5. `Requesttab`
- **Purpose**: Student accommodation booking applications.
- **Columns**:
  - `Request_id` (INT, IDENTITY(1,1), PRIMARY KEY): Application ID.
  - `user_id` (INT): Foreign reference to `Usertab.id`.
  - `RoomType` (VARCHAR(100)): Requested room category.
  - `Rent` (VARCHAR(50)): Agreed monthly rent.
  - `Status` (VARCHAR(50)): Application state (`Pending`, `Approved`, `Rejected`).

---

### 3.2 Stored Procedures

| Procedure Name | Parameters | Reads / Writes | Purpose |
|----------------|------------|----------------|---------|
| `sp_maximum` | None | Reads `Logintab` | Returns `ISNULL(MAX(rid), 0)` to assign the next sequential ID. |
| `sp_admininsert` | `@id`, `@admin_name`, `@admin_phone`, `@admin_email` | Writes `Admintab` | Inserts administrator profile. |
| `sp_userinsert` | `@id`, `@name`, `@gen`, `@age`, `@phone`, `@email`, `@addr`, `@joind` | Writes `Usertab` | Inserts student resident profile. |
| `sp_insertlogin` | `@rid`, `@una`, `@pwd`, `@ltype` | Writes `Logintab` | Creates login credentials. |
| `sp_countid` | `@uname`, `@pwd` | Reads `Logintab` | Validates credentials matching count. |
| `sp_regid` | `@una`, `@pwd` | Reads `Logintab` | Fetches registration ID for verified user. |
| `sp_logintype` | `@una`, `@pwd` | Reads `Logintab` | Fetches role type (`Admin` or `User`). |
| `sp_rooms` | `@rent`, `@typesharing`, `@occupied` | Writes `Roomtab` | Inserts new room category. |
| `sp_getAllRooms` | None | Reads `Roomtab` | Returns all room records. |
| `sp_insertrequest` | `@uid`, `@rtype`, `@rent`, `@status` | Writes `Requesttab` | Records student room booking request. |
| `sp_pendingreq` | None | Reads `Requesttab` | Returns applications where `Status = 'Pending'`. |
| `sp_update` | `@roomType`, `@newOccupied` | Writes `Roomtab` | Updates bed occupancy headcount. |
| `sp_getAllUsers` | None | Reads `Usertab` | Lists registered student directory. |

---

## 4. UI/UX Design System

The application interface was completely overhauled to provide an intuitive, accessible, and polished user experience:

- **Color Tokens**:
  - Primary: Deep & vibrant blues (`#1e3a8a`, `#2563eb`)
  - Semantic Statuses: Emerald Green (`#10b981`), Amber Warning (`#f59e0b`), Ruby Danger (`#ef4444`)
  - Neutral Backgrounds: Slate (`#f8fafc`, `#ffffff`, `#e2e8f0`)
- **Typography & Layout**:
  - System font stack with optimal line height and scannable visual weights.
  - Responsive flexbox and grid layouts preventing horizontal overflow on mobile screens.
- **Role-Aware Navigation**:
  - Dynamic navbar automatically adapts based on whether the user is an **Admin**, a **Student**, or a **Guest**.
  - Displays user avatar pill with role badge and instant sign-out.
- **In-App Toast & Alerts**:
  - Replaced legacy JavaScript `alert()` popups with styled, dismissible Bootstrap alert banners for feedback.
- **Empty States**:
  - Every table view (Rooms, Pending Requests, My Requests, Students) includes an illustrative empty state when no data is present.

---

## 5. Verification & Testing

The solution includes an automated xUnit test project (`HostelManagement.Tests`):
```bash
dotnet test "Hostel Management Dupli.sln"
```

All 8 tests execute and pass:
1. `GetAllRooms_ReturnsRooms_WhenDatabaseInitialized`
2. `LoginDB_WithValidAdminCredentials_ReturnsAdminRole`
3. `LoginDB_WithValidStudentCredentials_ReturnsUserRole`
4. `LoginDB_WithInvalidCredentials_ReturnsZeroRegId`
5. `GetRegisteredUsers_ReturnsUsersList`
6. `RoomRequest_Workflow_InsertsAndUpdatesStatus`
7. `UpdateRoomOccupied_UpdatesOccupancySuccessfully`
8. `LoginController_Logout_ClearsSessionAndRedirects`
