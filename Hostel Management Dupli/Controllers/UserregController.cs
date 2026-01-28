using Hostel_Management_Dupli.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hostel_Management_Dupli.Controllers
{
    public class UserregController : Controller
    {
        Dbcls dbobj = new Dbcls();
        public IActionResult Userpageload()
        {
            return View();
        }
        [HttpPost]
        public IActionResult userclick(Usercls obcls)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string resp = dbobj.UserInsert(obcls);
                    TempData["msg"] = resp;
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return View("Userpageload", obcls);
        }
    }
}
