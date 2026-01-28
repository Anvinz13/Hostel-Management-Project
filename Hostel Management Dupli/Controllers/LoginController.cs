using Hostel_Management_Dupli.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hostel_Management_Dupli.Controllers
{
    public class LoginController : Controller
    {
        Dbcls dbobj=new Dbcls();
        public IActionResult loginpageload()
        {
            return View();
        }
        public IActionResult Login_Click(Logincls clsobj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var getid = dbobj.LoginDB(clsobj);
                    if (getid.regid > 0)
                    {
                        TempData["uid"] = getid.regid;
                        TempData["logtype"] = getid.Logtype;
                        //TempData["message"] = "Login Successful!!";

                        if (getid.Logtype == "Admin")
                        {
                            return RedirectToAction("Adminhomeload", "Adminhome");
                        }
                        else if (getid.Logtype == "User")
                        {
                            return RedirectToAction("Userhomeload", "Userhome");
                        }
                    }
                    else
                    {
                        TempData["message"] = "Invalid Username or Password!!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.Message;
            }
            return View("loginpageload", clsobj);
        }
    }
}
