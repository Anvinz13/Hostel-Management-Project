using Hostel_Management_Dupli.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hostel_Management_Dupli.Controllers
{
    public class UserhomeController : Controller
    {
        private readonly Dbcls _db;

        public UserhomeController(Dbcls db)
        {
            _db = db;
        }

        public IActionResult Userhomeload()
        {
            var username = HttpContext.Session.GetString("username");
            int uid = HttpContext.Session.GetInt32("uid") ?? 0;

            ViewBag.Username = username ?? "Student";
            ViewBag.ActiveRequestsCount = uid > 0 ? _db.GetUserRequests(uid).Count : 0;
            ViewBag.AvailableRoomsCount = _db.GetAllRooms().Count;

            return View();
        }
    }
}
