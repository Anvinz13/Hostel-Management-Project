using Hostel_Management_Dupli.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hostel_Management_Dupli.Controllers
{
    public class RequestController : Controller
    {
        Dbcls db = new Dbcls();
        public IActionResult RequestRoom(string type, string rent)

        {
            int uid = Convert.ToInt32(TempData["uid"] ?? 0);

            Requstcls obj = new Requstcls
            {
                User_id = uid,
                RoomType = type,
                Rent = rent,
                Status = "Pending"  
            };

            db.InsertRoomRequest(obj);

            TempData["msg"] = "Room request sent!";
            return RedirectToAction("viewroom_load", "viewrooms");
        }

        public IActionResult PendingRequests()
        {
            var list = db.GetPendingRequests();
            return View(list);
        }

        // Admin approve
        public IActionResult Approve(int id)
        {
            db.UpdateRequestStatus(id, "Approved");
            return RedirectToAction("PendingRequests");
        }

        public IActionResult Reject(int id)
        {
            db.UpdateRequestStatus(id, "Rejected");
            return RedirectToAction("PendingRequests");
        }

        // User: View their own requests
        public IActionResult MyRequests()
        {
            int sid = Convert.ToInt32(TempData["uid"] ?? 0);
            var list = db.GetUserRequests(sid);
            return View(list);
        }
    }
}
