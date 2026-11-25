namespace Hostel_Management_Dupli.Models
{
    /// <summary>
    /// Represents a student allocated to a specific hostel room.
    /// Combines room request details with student profile information.
    /// </summary>
    public class RoomAllocationModel
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public string StudentName { get; set; } = "";
        public string StudentPhone { get; set; } = "";
        public string StudentEmail { get; set; } = "";
        public string StudentGender { get; set; } = "";
        public int StudentAge { get; set; }
        public string RoomType { get; set; } = "";
        public string Rent { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime JoinDate { get; set; }
    }
}
