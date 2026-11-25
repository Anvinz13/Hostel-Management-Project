using Hostel_Management_Dupli.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hostel_Management_Dupli.Controllers
{
    public class UserregController : Controller
    {
        private readonly Dbcls _db;

        public UserregController(Dbcls db)
        {
            _db = db;
        }

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
                    string resp = _db.UserInsert(obcls);
                    if (resp.Contains("success", StringComparison.OrdinalIgnoreCase))
                    {
                        TempData["success_msg"] = "Student registration completed successfully! Please sign in with your credentials.";
                        return RedirectToAction("loginpageload", "Login");
                    }
                    else
                    {
                        TempData["msg"] = resp;
                    }
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
