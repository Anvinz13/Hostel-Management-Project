using Hostel_Management_Dupli.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hostel_Management_Dupli.Controllers
{
    public class AdminregController : Controller
    {
        private readonly Dbcls _db;

        public AdminregController(Dbcls db)
        {
            _db = db;
        }

        public IActionResult Admin_pageload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult adminclick(Admincls obcls)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string resp = _db.AdminInsert(obcls);
                    if (resp.Contains("success", StringComparison.OrdinalIgnoreCase))
                    {
                        TempData["success_msg"] = "Administrator account created successfully! Please sign in.";
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
            return View("Admin_pageload", obcls);
        }
    }
}
