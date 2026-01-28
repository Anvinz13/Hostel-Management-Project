using Microsoft.AspNetCore.Mvc;
using Hostel_Management_Dupli.Models;

namespace Hostel_Management_Dupli.Controllers
{
    public class AddroomController : Controller
    {
        Dbcls obj = new Dbcls();
        public IActionResult roompageload()
        {
            return View();
        }

        [HttpPost]

        public IActionResult AddRoomClick(Addroommodel cls)
        {
            if (ModelState.IsValid)
            {
                string msg = obj.AddRoomInsert(cls); // Call Dbcls method
                TempData["msg"] = msg;             // Show message in view
            }
            return View("roompageload");            // Return same page
        }
    }
}
