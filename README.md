# 🏨 Sunshine Hostel Management System

A modern, responsive ASP.NET Core 10 web application designed to automate hostel administration, room allocation, student accommodation applications, and resident records.

---

## 📌 Overview

The **Sunshine Hostel Management System** digitizes residential hostel operations. It provides role-based portals for **Students** (browse rooms, submit booking requests, track application statuses) and **Administrators** (configure room categories, monitor occupancy, review pending applications, and manage student directories).

---

## ✨ Features

- **Authentication & Role-Based Access**:
  - Secure login with session tracking (`Admin` and `User/Student`).
  - Student registration with personal details, contact information, and joining dates.
  - Administrator signup and dashboard protection.
- **Room Management (Admin)**:
  - Add room categories (Single AC/Non-AC, 2-Sharing, 3-Sharing, 4-Sharing) with rental rates.
  - Live occupancy management with fast increment/decrement controls.
- **Accommodation Booking (Student)**:
  - Responsive room catalog with pricing and amenities highlights.
  - One-click room booking application submission.
  - Real-time application status tracker (`Pending`, `Approved`, `Rejected`).
- **Application Review Workflow (Admin)**:
  - Approval queue for pending student applications.
  - One-click "Approve" and "Reject" workflow with instant status updates.
- **Student Directory**:
  - Centralized directory of registered hostel residents with full contact information.
- **Modern UI / UX**:
  - Built with Bootstrap 5, Bootstrap Icons, responsive CSS design system, contextual navigation, and dismissible notification alerts (replacing browser alerts).

---

## 💻 Tech Stack & Requirements

| Component | Technology | Version |
|-----------|------------|---------|
| Framework | ASP.NET Core | **.NET 10.0** (`net10.0`) |
| Language | C# | 13+ |
| Database Client | Microsoft.Data.SqlClient | 6.0.1 (with Pkcs 9.0.23 security patch) |
| Database Engine | Microsoft SQL Server / Express / LocalDB | 2016 or newer |
| Frontend | HTML5, Modern CSS3, Bootstrap 5, Bootstrap Icons | Latest |
| Testing | xUnit, ASP.NET Core Test Framework | Latest |

---

## 🚀 Setup & Installation

Follow these numbered steps to run the application on a fresh machine:

### 1. Prerequisites
Ensure you have installed:
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or verify with `dotnet --version` showing `10.x`)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (Express, Developer, or Standard edition)

### 2. Clone the Repository
```bash
git clone https://github.com/Anvinz13/Hostel-Management-Project.git
cd Hostel-Management-Project
```

### 3. Initialize the Database
Open SQL Server Management Studio (SSMS), Azure Data Studio, or PowerShell, and execute the included initialization script:
```powershell
# Using sqlcmd or PowerShell
sqlcmd -S "localhost\SQLEXPRESS" -i "Database_Schema.sql"
```
*Note: The script creates the `[Hostel dupli]` database, all tables (`Admintab`, `Usertab`, `Logintab`, `Roomtab`, `Requesttab`), stored procedures, and seeds demo accounts and room categories.*

### 4. Configure Connection String
Check `Hostel Management Dupli/appsettings.json` and adjust the connection string to match your SQL Server instance if needed:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=Hostel dupli;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

### 5. Build and Run the Application
```bash
dotnet restore "Hostel Management Dupli/Hostel Management Dupli.csproj"
dotnet build "Hostel Management Dupli/Hostel Management Dupli.csproj"
dotnet run --project "Hostel Management Dupli/Hostel Management Dupli.csproj"
```

The web application will launch at:
- `http://localhost:5000` or `https://localhost:5001` (or the ports indicated in your console output).

---

## 🔑 Default Credentials

The database script seeds the following pre-configured accounts:

| Role | Username | Password | Purpose |
|------|----------|----------|---------|
| **Admin** | `admin` | `admin123` | Full administrative control (rooms, requests, students) |
| **Student** | `student` | `user123` | Standard resident account (browse rooms, view requests) |

---

## 📖 How to Use

### Student Workflow
1. Navigate to `/Login/loginpageload` and sign in with `student` / `user123`, or click **Register as Student** to create a new profile.
2. Click **Browse Rooms** in the top navigation to view available room types and rental rates.
3. Click **Request This Room** to submit a booking application.
4. Visit **My Requests** to monitor approval status in real time.

### Administrator Workflow
1. Sign in with `admin` / `admin123`.
2. Access the **Administrator Dashboard** to view total rooms, pending applications, and student counts.
3. Click **Pending Requests** to review and approve or reject incoming room requests.
4. Click **Manage Rooms** to adjust live bed occupancy headcounts.
5. Click **Add Room** to configure new room categories and rents.
6. Click **Students** to view registered resident profiles and contact details.

---

## 📁 Project Structure

```
Hostel-Management-Project/
├── Database_Schema.sql                     # Full SQL database creation and seed script
├── README.md                               # Project guide and overview
├── PROJECT_DOCUMENTATION.md                # Comprehensive technical documentation
├── Hostel Management Dupli.sln             # Visual Studio solution file
│
├── Hostel Management Dupli/                # Main ASP.NET Core 10 Web Project
│   ├── Controllers/                        # MVC Controllers
│   │   ├── AddroomController.cs            # Adding room categories
│   │   ├── AdminhomeController.cs          # Admin dashboard & occupancy updates
│   │   ├── AdminregController.cs           # Admin registration
│   │   ├── HomeController.cs               # Landing & privacy pages
│   │   ├── LoginController.cs              # Authentication & session management
│   │   ├── RequestController.cs            # Student room booking & review workflow
│   │   ├── UserhomeController.cs           # Student dashboard
│   │   ├── UserregController.cs            # Student registration
│   │   └── viewroomsController.cs          # Room browsing
│   │
│   ├── Models/                             # Domain Models & Data Access
│   │   ├── Dbcls.cs                        # Scoped ADO.NET data access service
│   │   ├── Admincls.cs                     # Admin profile model
│   │   ├── Usercls.cs                      # Student profile model
│   │   ├── Logincls.cs                     # Authentication credential model
│   │   ├── Addroommodel.cs                 # Room definition model
│   │   ├── Requstcls.cs                    # Room request model
│   │   └── ErrorViewModel.cs               # Error view model
│   │
│   ├── Views/                              # Razor MVC Views
│   │   ├── Shared/                         # _Layout, _ViewImports, _ViewStart
│   │   ├── Home/                           # Index (landing), Privacy
│   │   ├── Login/                          # Sign-in portal
│   │   ├── Adminhome/                      # Admin dashboard, manage rooms, student directory
│   │   ├── Addroom/                        # Add room form
│   │   ├── Userhome/                       # Student home dashboard
│   │   ├── Userreg/                        # Student registration form
│   │   ├── Adminreg/                       # Admin registration form
│   │   ├── viewrooms/                      # Room catalog view
│   │   └── Request/                        # Pending requests (admin) & My requests (student)
│   │
│   ├── wwwroot/                            # Static Web Assets
│   │   ├── css/site.css                    # Custom CSS design system & tokens
│   │   └── js/site.js                      # Client-side scripts
│   │
│   ├── appsettings.json                    # Configuration & connection string
│   └── Program.cs                          # ASP.NET Core 10 pipeline & DI configuration
│
└── HostelManagement.Tests/                 # Automated xUnit Test Suite
    ├── UnitTest1.cs                        # Comprehensive unit & integration tests
    └── HostelManagement.Tests.csproj       # Test project targeting net10.0
```

---

## 🧪 Automated Testing

Execute the test suite to verify database operations, authentication, and workflow logic:
```bash
dotnet test "Hostel Management Dupli.sln"
```

All 8 test scenarios will execute and validate:
- ✅ Room catalog querying
- ✅ Admin and student authentication
- ✅ Invalid credential handling
- ✅ Student registration retrieval
- ✅ End-to-end room request insertion, review, approval, and rejection
- ✅ Room occupancy updates
- ✅ Session clearing and logout redirection

---

## 🛠️ Common Problems & Troubleshooting

| Problem | Cause | Solution |
|---------|-------|----------|
| **Cannot open database "Hostel dupli"** | Database not created or SQL Server not running. | Execute `Database_Schema.sql` in SSMS or PowerShell and ensure SQL Server service is running. |
| **Login fails with invalid credentials** | Default accounts missing. | Execute the seed section of `Database_Schema.sql` to populate `admin` and `student` accounts. |
| **NullReferenceException during room booking** | Session expired or student not logged in. | Sign in as a student at `/Login/loginpageload` before submitting a room request. |
| **Vulnerability warning for Pkcs** | Transitive package vulnerability. | Project references `System.Security.Cryptography.Pkcs` version 9.0.23 / 10.0 to remediate GHSA advisories. |

---

## 👥 Contact & Owner

- **Project Lead**: Anvin
- **Repository**: [https://github.com/Anvinz13/Hostel-Management-Project](https://github.com/Anvinz13/Hostel-Management-Project)
- **Hostel Facility**: Sunshine Hostel, VSNL Road, Kakkanad, Kochi, Kerala
