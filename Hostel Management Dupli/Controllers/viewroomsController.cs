using Hostel_Management_Dupli.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hostel_Management_Dupli.Controllers
{
    public class viewroomsController : Controller
    {
        private readonly Dbcls _db;

        public viewroomsController(Dbcls db)
        {
            _db = db;
        }

        public IActionResult viewroom_load()
        {
            var rooms = _db.GetAllRooms();
            return View(rooms);
        }
    }
}
