using Hostel_Management_Dupli.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hostel_Management_Dupli.Controllers
{
    public class AdminregController : Controller
    {
       
            Dbcls dbobj = new Dbcls();
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
                    string resp = dbobj.AdminInsert(obcls);
                    TempData["msg"] = resp;
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