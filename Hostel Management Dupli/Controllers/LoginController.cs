using Hostel_Management_Dupli.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hostel_Management_Dupli.Controllers
{
    public class LoginController : Controller
    {
        private readonly Dbcls _dbobj;

        public LoginController(Dbcls dbobj)
        {
            _dbobj = dbobj;
        }

        public IActionResult loginpageload()
        {
            // If already logged in, redirect to respective home
            var logtype = HttpContext.Session.GetString("logtype");
            if (logtype == "Admin")
            {
                return RedirectToAction("Adminhomeload", "Adminhome");
            }
            else if (logtype == "User")
            {
                return RedirectToAction("Userhomeload", "Userhome");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login_Click(Logincls clsobj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var getid = _dbobj.LoginDB(clsobj);
                    if (getid != null && getid.regid > 0)
                    {
                        HttpContext.Session.SetInt32("uid", getid.regid);
                        HttpContext.Session.SetString("logtype", getid.Logtype ?? "");
                        HttpContext.Session.SetString("username", getid.Username ?? "");

                        TempData["uid"] = getid.regid;
                        TempData["logtype"] = getid.Logtype;
                        TempData["success_msg"] = $"Welcome back, {getid.Username}!";

                        if (string.Equals(getid.Logtype, "Admin", StringComparison.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("Adminhomeload", "Adminhome");
                        }
                        else
                        {
                            return RedirectToAction("Userhomeload", "Userhome");
                        }
                    }
                    else
                    {
                        TempData["error_msg"] = "Invalid username or password. Please try again.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["error_msg"] = "Database connection or login error: " + ex.Message;
            }
            return View("loginpageload", clsobj);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["info_msg"] = "You have been logged out successfully.";
            return RedirectToAction("loginpageload", "Login");
        }
    }
}
