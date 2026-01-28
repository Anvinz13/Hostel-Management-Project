using Hostel_Management_Dupli.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hostel_Management_Dupli.Controllers
{
    public class viewroomsController : Controller
    {
        Dbcls db=new Dbcls();
        public IActionResult viewroom_load()
        {
            var rooms = db.GetAllRooms();
            return View(rooms);
        }
    }
}
