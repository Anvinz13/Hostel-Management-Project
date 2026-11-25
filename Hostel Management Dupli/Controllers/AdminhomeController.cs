using Microsoft.AspNetCore.Mvc;
using Hostel_Management_Dupli.Models;

namespace Hostel_Management_Dupli.Controllers
{
    public class AdminhomeController : Controller
    {
        private readonly Dbcls _db;

        public AdminhomeController(Dbcls db)
        {
            _db = db;
        }

        // Admin dashboard page
        public IActionResult Adminhomeload()
        {
            // Verify admin session if present
            var logtype = HttpContext.Session.GetString("logtype");
            if (!string.IsNullOrEmpty(logtype) && logtype != "Admin")
            {
                TempData["error_msg"] = "Access denied: Administrator privileges required.";
                return RedirectToAction("loginpageload", "Login");
            }

            try
            {
                ViewBag.RoomsCount = _db.GetAllRooms().Count;
                ViewBag.PendingRequestsCount = _db.GetPendingRequests().Count;
                ViewBag.UsersCount = _db.GetRegisteredUsers().Count;
                ViewBag.AllocationsCount = _db.GetRoomAllocations().Count;
            }
            catch
            {
                ViewBag.RoomsCount = 0;
                ViewBag.PendingRequestsCount = 0;
                ViewBag.UsersCount = 0;
                ViewBag.AllocationsCount = 0;
            }

            return View();
        }

        // View which room is occupied by which student
        public IActionResult Allocations()
        {
            var allocations = _db.GetRoomAllocations();
            ViewBag.Rooms = _db.GetAllRooms();
            return View(allocations);
        }

        // Deallocate a resident from a room (vacate)
        public IActionResult Deallocate(int id, string roomType)
        {
            _db.DeallocateStudent(id, roomType);
            TempData["success_msg"] = $"Resident successfully deallocated from {roomType}. Room bed released.";
            return RedirectToAction("Allocations");
        }

        // Redirect to Add Room Page
        public IActionResult AddRoom()
        {
            return RedirectToAction("roompageload", "Addroom");
        }

        // Redirect to Update Room Page
        public IActionResult UpdateRoom()
        {
            var rooms = _db.GetAllRooms();
            return View("UpdateRoomLoad", rooms);
        }

        // View registered student users (Fixes the previously broken action)
        public IActionResult RegisteredUsers()
        {
            var users = _db.GetRegisteredUsers();
            return View(users);
        }

        // Redirect to view pending requests
        public IActionResult PendingRequests()
        {
            return RedirectToAction("PendingRequests", "Request");
        }

        // POST: Update Occupied value for a room
        [HttpPost]
        public IActionResult UpdateRoomOccupied(string roomType, int occupied)
        {
            string resp = _db.UpdateRoomOccupied(roomType, occupied);
            TempData["msg"] = resp;
            return RedirectToAction("UpdateRoom");
        }
    }
}
