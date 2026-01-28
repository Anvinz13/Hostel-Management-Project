using Microsoft.AspNetCore.Mvc;
using Hostel_Management_Dupli.Models;

namespace Hostel_Management_Dupli.Controllers
{
    public class AdminhomeController : Controller
    {
        Dbcls obj = new Dbcls();

        // Admin home page
        public IActionResult Adminhomeload()
        {
            return View();
        }

        // Redirect to Add Room Page
        public IActionResult AddRoom()
        {
            return RedirectToAction("roompageload", "Addroom");
        }

        // Redirect to Update Room Page (fixed to point to correct action)
        public IActionResult UpdateRoom()
        {
            var rooms = obj.GetAllRooms(); // Get all rooms
            return View("UpdateRoomLoad", rooms); // Use the view directly
        }

        // Redirect to view registered users
        public IActionResult RegisteredUsers()
        {
            return RedirectToAction("UserList", "User"); // Your UserController action
        }

        // Redirect to view pending requests
        public IActionResult PendingRequests()
        {
            return RedirectToAction("PendingRequests", "Request"); // Your RequestController action
        }

        // POST: Update Occupied value for a room
        [HttpPost]
        public IActionResult UpdateRoomOccupied(string roomType, int occupied)
        {
            string resp = obj.UpdateRoomOccupied(roomType, occupied);
            TempData["msg"] = resp;
            return RedirectToAction("UpdateRoom"); // Reload the room list after update
        }
    }
}