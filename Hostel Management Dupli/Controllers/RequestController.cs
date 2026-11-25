using Hostel_Management_Dupli.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hostel_Management_Dupli.Controllers
{
    public class RequestController : Controller
    {
        private readonly Dbcls _db;

        public RequestController(Dbcls db)
        {
            _db = db;
        }

        public IActionResult RequestRoom(string type, string rent)
        {
            int uid = HttpContext.Session.GetInt32("uid") ?? Convert.ToInt32(TempData["uid"] ?? 0);

            if (uid <= 0)
            {
                TempData["error_msg"] = "Please log in with your student account before requesting a room.";
                return RedirectToAction("loginpageload", "Login");
            }

            var obj = new Requstcls
            {
                User_id = uid,
                RoomType = type,
                Rent = rent,
                Status = "Pending"
            };

            string result = _db.InsertRoomRequest(obj);
            TempData["msg"] = result.Contains("success", StringComparison.OrdinalIgnoreCase) 
                ? "Room request submitted successfully! The administrator will review your application." 
                : result;

            return RedirectToAction("MyRequests");
        }

        public IActionResult PendingRequests()
        {
            var list = _db.GetPendingRequests();
            return View(list);
        }

        // Admin approve
        public IActionResult Approve(int id)
        {
            _db.UpdateRequestStatus(id, "Approved");
            TempData["msg"] = $"Request #{id} has been approved successfully.";
            return RedirectToAction("PendingRequests");
        }

        // Admin reject
        public IActionResult Reject(int id)
        {
            _db.UpdateRequestStatus(id, "Rejected");
            TempData["msg"] = $"Request #{id} has been rejected.";
            return RedirectToAction("PendingRequests");
        }

        // User: View their own requests
        public IActionResult MyRequests()
        {
            int sid = HttpContext.Session.GetInt32("uid") ?? Convert.ToInt32(TempData["uid"] ?? 0);
            
            if (sid <= 0)
            {
                TempData["error_msg"] = "Please log in to view your room requests.";
                return RedirectToAction("loginpageload", "Login");
            }

            var list = _db.GetUserRequests(sid);
            return View(list);
        }
    }
}
