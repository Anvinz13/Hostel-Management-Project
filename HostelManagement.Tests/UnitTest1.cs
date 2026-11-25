using Hostel_Management_Dupli.Controllers;
using Hostel_Management_Dupli.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Xunit;

namespace HostelManagement.Tests
{
    public class HostelManagementTests
    {
        private readonly Dbcls _db;

        public HostelManagementTests()
        {
            // Initializes Dbcls with local database connection
            _db = new Dbcls();
        }

        [Fact]
        public void GetAllRooms_ReturnsRooms_WhenDatabaseInitialized()
        {
            // Act
            var rooms = _db.GetAllRooms();

            // Assert
            Assert.NotNull(rooms);
            Assert.NotEmpty(rooms);
            Assert.Contains(rooms, r => !string.IsNullOrEmpty(r.Roomtype));
        }

        [Fact]
        public void LoginDB_WithValidAdminCredentials_ReturnsAdminRole()
        {
            // Arrange
            var login = new Logincls
            {
                Username = "admin",
                Password = "admin123"
            };

            // Act
            var result = _db.LoginDB(login);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.regid > 0);
            Assert.Equal("Admin", result.Logtype, ignoreCase: true);
        }

        [Fact]
        public void LoginDB_WithValidStudentCredentials_ReturnsUserRole()
        {
            // Arrange
            var login = new Logincls
            {
                Username = "student",
                Password = "user123"
            };

            // Act
            var result = _db.LoginDB(login);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.regid > 0);
            Assert.Equal("User", result.Logtype, ignoreCase: true);
        }

        [Fact]
        public void LoginDB_WithInvalidCredentials_ReturnsZeroRegId()
        {
            // Arrange
            var login = new Logincls
            {
                Username = "nonexistent_user_xyz",
                Password = "wrongpassword"
            };

            // Act
            var result = _db.LoginDB(login);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.regid);
        }

        [Fact]
        public void GetRegisteredUsers_ReturnsUsersList()
        {
            // Act
            var users = _db.GetRegisteredUsers();

            // Assert
            Assert.NotNull(users);
            Assert.NotEmpty(users);
        }

        [Fact]
        public void RoomRequest_Workflow_InsertsAndUpdatesStatus()
        {
            // Arrange: Create a room request
            var testReq = new Requstcls
            {
                User_id = 2, // student John Doe
                RoomType = "Single Room (AC)",
                Rent = "8500",
                Status = "Pending"
            };

            // Act: Insert
            string insertMsg = _db.InsertRoomRequest(testReq);
            Assert.Contains("success", insertMsg, StringComparison.OrdinalIgnoreCase);

            // Verify it is present in pending requests
            var pending = _db.GetPendingRequests();
            Assert.NotNull(pending);
            var myReq = pending.FirstOrDefault(r => r.User_id == 2 && r.RoomType == "Single Room (AC)");
            Assert.NotNull(myReq);

            // Act: Approve the request
            _db.UpdateRequestStatus(myReq.RequestId, "Approved");

            // Verify status in user requests
            var userReqs = _db.GetUserRequests(2);
            var updatedReq = userReqs.FirstOrDefault(r => r.RequestId == myReq.RequestId);
            Assert.NotNull(updatedReq);
            Assert.Equal("Approved", updatedReq.Status);
        }

        [Fact]
        public void GetRoomAllocations_ReturnsAllocatedStudents()
        {
            // Act
            var allocations = _db.GetRoomAllocations();

            // Assert
            Assert.NotNull(allocations);
            // Verify structure has RoomType and StudentName
            foreach (var alloc in allocations)
            {
                Assert.False(string.IsNullOrEmpty(alloc.RoomType));
                Assert.False(string.IsNullOrEmpty(alloc.StudentName));
            }
        }

        [Fact]
        public void DeallocateStudent_VacatesRequestAndFreesBed()
        {
            // Arrange: create an approved allocation
            var req = new Requstcls
            {
                User_id = 2,
                RoomType = "Single Room (AC)",
                Rent = "8500",
                Status = "Approved"
            };
            _db.InsertRoomRequest(req);
            var userReqs = _db.GetUserRequests(2);
            var active = userReqs.First(r => r.Status == "Approved");

            // Act
            _db.DeallocateStudent(active.RequestId, "Single Room (AC)");

            // Assert: verify status became Vacated
            var updatedReqs = _db.GetUserRequests(2);
            var vacated = updatedReqs.First(r => r.RequestId == active.RequestId);
            Assert.Equal("Vacated", vacated.Status);
        }

        [Fact]
        public void UpdateRoomOccupied_UpdatesOccupancySuccessfully()
        {
            // Act
            string result = _db.UpdateRoomOccupied("Single Room (AC)", 1);

            // Assert
            Assert.Contains("success", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void LoginController_Logout_ClearsSessionAndRedirects()
        {
            // Arrange
            var controller = new LoginController(_db);
            var httpContext = new DefaultHttpContext();
            var session = new TestSession();
            session.SetString("username", "admin");
            httpContext.Session = session;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            var tempData = new TempDataDictionary(httpContext, new EmptyTempDataProvider());
            controller.TempData = tempData;

            // Act
            var result = controller.Logout() as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("loginpageload", result.ActionName);
            Assert.Equal("Login", result.ControllerName);
            Assert.DoesNotContain("username", session.Keys);
        }
    }

    // Lightweight in-memory session implementation for unit testing controllers
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _storage = new();

        public bool IsAvailable => true;
        public string Id => Guid.NewGuid().ToString();
        public IEnumerable<string> Keys => _storage.Keys;

        public void Clear() => _storage.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => _storage.Remove(key);

        public void Set(string key, byte[] value) => _storage[key] = value;

        public bool TryGetValue(string key, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out byte[]? value) => _storage.TryGetValue(key, out value);
    }

    public class EmptyTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object?> LoadTempData(HttpContext context) => new Dictionary<string, object?>();
        public void SaveTempData(HttpContext context, IDictionary<string, object?> values) { }
    }
}
