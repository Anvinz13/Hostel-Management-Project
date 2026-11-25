using Microsoft.AspNetCore.Mvc;
using Hostel_Management_Dupli.Models;

namespace Hostel_Management_Dupli.Controllers
{
    public class AddroomController : Controller
    {
        private readonly Dbcls _db;

        public AddroomController(Dbcls db)
        {
            _db = db;
        }

        public IActionResult roompageload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddRoomClick(Addroommodel cls)
        {
            if (ModelState.IsValid)
            {
                string msg = _db.AddRoomInsert(cls);
                TempData["msg"] = msg;
                if (msg.Contains("success", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("UpdateRoom", "Adminhome");
                }
            }
            return View("roompageload", cls);
        }
    }
}
