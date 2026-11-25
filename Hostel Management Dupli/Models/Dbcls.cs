using System.Data;
using Microsoft.Data.SqlClient;

namespace Hostel_Management_Dupli.Models
{
    /// <summary>
    /// Data access service for the Hostel Management System.
    /// Provides parameterized database operations, stored procedure executions, and lifecycle management.
    /// </summary>
    public class Dbcls
    {
        private readonly string _connectionString;
        private const string FallbackConnection = @"Server=localhost\SQLEXPRESS;Database=Hostel dupli;Integrated Security=True;TrustServerCertificate=True;";

        /// <summary>
        /// Initializes a new instance of <see cref="Dbcls"/> via Dependency Injection.
        /// </summary>
        /// <param name="configuration">Application configuration instance to read connection strings.</param>
        public Dbcls(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? FallbackConnection;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Dbcls"/> with the fallback local connection string.
        /// </summary>
        public Dbcls()
        {
            _connectionString = FallbackConnection;
        }

        /// <summary>
        /// Creates a new SQL connection instance for safe, thread-scoped execution.
        /// </summary>
        private SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        /// <summary>
        /// Registers a new administrator record in Admintab and creates authentication credentials in Logintab.
        /// </summary>
        /// <param name="objcls">The administrator model containing name, contact details, username, and password.</param>
        /// <returns>A status message string indicating success or the caught error message.</returns>
        public string AdminInsert(Admincls objcls)
        {
            try
            {
                using var con = CreateConnection();
                con.Open();

                using (var cmd = new SqlCommand("sp_maximum", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    object mxregid = cmd.ExecuteScalar();
                    int regid = (mxregid == null || mxregid == DBNull.Value) ? 1 : Convert.ToInt32(mxregid) + 1;

                    // ADMIN INSERT
                    using (var cmd1 = new SqlCommand("sp_admininsert", con))
                    {
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@id", regid);
                        cmd1.Parameters.AddWithValue("@admin_name", objcls.Name ?? "");
                        cmd1.Parameters.AddWithValue("@admin_phone", objcls.Phone ?? "");
                        cmd1.Parameters.AddWithValue("@admin_email", objcls.Email ?? "");
                        cmd1.ExecuteNonQuery();
                    }

                    // LOGIN INSERT
                    using (var cmd2 = new SqlCommand("sp_insertlogin", con))
                    {
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@rid", regid);
                        cmd2.Parameters.AddWithValue("@una", objcls.Username ?? "");
                        cmd2.Parameters.AddWithValue("@pwd", objcls.Password ?? "");
                        cmd2.Parameters.AddWithValue("@ltype", "Admin");
                        cmd2.ExecuteNonQuery();
                    }
                }

                return "Insertion success!!!";
            }
            catch (Exception ex)
            {
                return "Registration error: " + ex.Message;
            }
        }

        /// <summary>
        /// Registers a new student record in Usertab and creates authentication credentials in Logintab.
        /// </summary>
        /// <param name="clsobj">The student user model containing personal details, username, and password.</param>
        /// <returns>A status message string indicating success or error.</returns>
        public string UserInsert(Usercls clsobj)
        {
            try
            {
                using var con = CreateConnection();
                con.Open();

                using (var cmd = new SqlCommand("sp_maximum", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    object mxregid = cmd.ExecuteScalar();
                    int regid = (mxregid == null || mxregid == DBNull.Value) ? 1 : Convert.ToInt32(mxregid) + 1;

                    // User Insertion
                    using (var cmd1 = new SqlCommand("sp_userinsert", con))
                    {
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@id", regid);
                        cmd1.Parameters.AddWithValue("@name", clsobj.Name ?? "");
                        cmd1.Parameters.AddWithValue("@gen", clsobj.Gender ?? "");
                        cmd1.Parameters.AddWithValue("@age", clsobj.Age);
                        cmd1.Parameters.AddWithValue("@phone", clsobj.Phone ?? "");
                        cmd1.Parameters.AddWithValue("@email", clsobj.Email ?? "");
                        cmd1.Parameters.AddWithValue("@addr", clsobj.Address ?? "");
                        cmd1.Parameters.AddWithValue("@joind", clsobj.JoinDate == default ? DateTime.Now : clsobj.JoinDate);
                        cmd1.ExecuteNonQuery();
                    }

                    // LOGIN INSERT
                    using (var cmd2 = new SqlCommand("sp_insertlogin", con))
                    {
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@rid", regid);
                        cmd2.Parameters.AddWithValue("@una", clsobj.Username ?? "");
                        cmd2.Parameters.AddWithValue("@pwd", clsobj.Password ?? "");
                        cmd2.Parameters.AddWithValue("@ltype", "User");
                        cmd2.ExecuteNonQuery();
                    }
                }

                return "Insertion success!!!";
            }
            catch (Exception ex)
            {
                return "Registration error: " + ex.Message;
            }
        }

        /// <summary>
        /// Authenticates username and password against the Logintab repository.
        /// </summary>
        /// <param name="objcls">Login credentials containing Username and Password.</param>
        /// <returns>A populated <see cref="Logincls"/> with registration id and role type, or regid = 0 if invalid.</returns>
        public Logincls LoginDB(Logincls objcls)
        {
            var result = new Logincls();
            try
            {
                using var con = CreateConnection();
                con.Open();

                using (var cmd1 = new SqlCommand("sp_countid", con))
                {
                    cmd1.CommandType = CommandType.StoredProcedure;
                    cmd1.Parameters.AddWithValue("@uname", objcls.Username ?? "");
                    cmd1.Parameters.AddWithValue("@pwd", objcls.Password ?? "");

                    object scalarCount = cmd1.ExecuteScalar();
                    int cid = scalarCount != null ? Convert.ToInt32(scalarCount) : 0;

                    if (cid >= 1)
                    {
                        using (var cmd2 = new SqlCommand("sp_regid", con))
                        {
                            cmd2.CommandType = CommandType.StoredProcedure;
                            cmd2.Parameters.AddWithValue("@una", objcls.Username ?? "");
                            cmd2.Parameters.AddWithValue("@pwd", objcls.Password ?? "");
                            result.regid = Convert.ToInt32(cmd2.ExecuteScalar() ?? 0);
                        }

                        using (var cmd3 = new SqlCommand("sp_logintype", con))
                        {
                            cmd3.CommandType = CommandType.StoredProcedure;
                            cmd3.Parameters.AddWithValue("@una", objcls.Username ?? "");
                            cmd3.Parameters.AddWithValue("@pwd", objcls.Password ?? "");
                            object logTypeObj = cmd3.ExecuteScalar();
                            result.Logtype = logTypeObj?.ToString() ?? "";
                        }

                        result.Username = objcls.Username;
                        result.Password = objcls.Password;
                    }
                    else
                    {
                        result.regid = 0;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }

        /// <summary>
        /// Adds a new room category into the Roomtab table.
        /// </summary>
        /// <param name="obj">The room details including rent, room type, and initial occupancy.</param>
        /// <returns>Success or error status string.</returns>
        public string AddRoomInsert(Addroommodel obj)
        {
            try
            {
                using var con = CreateConnection();
                con.Open();

                using var cmd = new SqlCommand("sp_rooms", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@rent", obj.Rent ?? "0");
                cmd.Parameters.AddWithValue("@typesharing", obj.Roomtype ?? "Standard");
                cmd.Parameters.AddWithValue("@occupied", obj.Occupied ?? "0");

                cmd.ExecuteNonQuery();
                return "Room Added Successfully!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Retrieves all configured hostel rooms.
        /// </summary>
        /// <returns>A list of <see cref="Addroommodel"/> objects.</returns>
        public List<Addroommodel> GetAllRooms()
        {
            var roomList = new List<Addroommodel>();
            try
            {
                using var con = CreateConnection();
                con.Open();

                using var cmd = new SqlCommand("sp_getAllRooms", con);
                cmd.CommandType = CommandType.StoredProcedure;

                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    roomList.Add(new Addroommodel
                    {
                        Rent = dr["Rent"]?.ToString() ?? "",
                        Roomtype = dr["Typeofsharing"]?.ToString() ?? "",
                        Occupied = dr["Occupied"]?.ToString() ?? "0"
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }

            return roomList;
        }

        /// <summary>
        /// Creates a new student accommodation booking request in Requesttab.
        /// </summary>
        /// <param name="obj">The request model containing user id, room type, rent, and status.</param>
        /// <returns>Status confirmation message.</returns>
        public string InsertRoomRequest(Requstcls obj)
        {
            try
            {
                using var con = CreateConnection();
                con.Open();

                using var cmd = new SqlCommand("sp_insertrequest", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@uid", obj.User_id);
                cmd.Parameters.AddWithValue("@rtype", obj.RoomType ?? "");
                cmd.Parameters.AddWithValue("@rent", obj.Rent ?? "");
                cmd.Parameters.AddWithValue("@status", string.IsNullOrEmpty(obj.Status) ? "Pending" : obj.Status);

                cmd.ExecuteNonQuery();
                return "Request sent successfully!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Retrieves all student room booking requests that are pending admin approval.
        /// </summary>
        /// <returns>A list of pending <see cref="Requstcls"/> items.</returns>
        public List<Requstcls> GetPendingRequests()
        {
            var list = new List<Requstcls>();
            try
            {
                using var con = CreateConnection();
                con.Open();

                using var cmd = new SqlCommand("sp_pendingreq", con);
                cmd.CommandType = CommandType.StoredProcedure;

                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new Requstcls
                    {
                        RequestId = dr["Request_id"] != DBNull.Value ? Convert.ToInt32(dr["Request_id"]) : 0,
                        User_id = dr["user_id"] != DBNull.Value ? Convert.ToInt32(dr["user_id"]) : 0,
                        RoomType = dr["RoomType"]?.ToString() ?? "",
                        Rent = dr["Rent"]?.ToString() ?? "",
                        Status = dr["Status"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }

        /// <summary>
        /// Updates the status of a specific student room request (e.g. 'Approved' or 'Rejected').
        /// If approved, automatically increments the room's occupied bed count.
        /// </summary>
        /// <param name="rid">The unique Request_id.</param>
        /// <param name="status">The new status string.</param>
        public void UpdateRequestStatus(int rid, string status)
        {
            try
            {
                using var con = CreateConnection();
                con.Open();

                string roomType = "";
                using (var getCmd = new SqlCommand("SELECT RoomType FROM Requesttab WHERE Request_id=@rid", con))
                {
                    getCmd.Parameters.AddWithValue("@rid", rid);
                    object r = getCmd.ExecuteScalar();
                    roomType = r?.ToString() ?? "";
                }

                using (var cmd = new SqlCommand("UPDATE Requesttab SET Status=@status WHERE Request_id=@rid", con))
                {
                    cmd.Parameters.AddWithValue("@rid", rid);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.ExecuteNonQuery();
                }

                // If approved and room type found, increment occupied count
                if (string.Equals(status, "Approved", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(roomType))
                {
                    using var incCmd = new SqlCommand(@"
                        UPDATE Roomtab 
                        SET Occupied = CAST(ISNULL(TRY_CAST(Occupied AS INT), 0) + 1 AS VARCHAR(50)) 
                        WHERE Typeofsharing = @roomType", con);
                    incCmd.Parameters.AddWithValue("@roomType", roomType);
                    incCmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Deallocates a student from a room (marks application as Vacated and decrements bed count).
        /// </summary>
        /// <param name="rid">The request id to vacate.</param>
        /// <param name="roomType">The room category.</param>
        public void DeallocateStudent(int rid, string roomType)
        {
            try
            {
                using var con = CreateConnection();
                con.Open();

                using (var cmd = new SqlCommand("UPDATE Requesttab SET Status='Vacated' WHERE Request_id=@rid", con))
                {
                    cmd.Parameters.AddWithValue("@rid", rid);
                    cmd.ExecuteNonQuery();
                }

                if (!string.IsNullOrEmpty(roomType))
                {
                    using var decCmd = new SqlCommand(@"
                        UPDATE Roomtab 
                        SET Occupied = CASE 
                            WHEN ISNULL(TRY_CAST(Occupied AS INT), 0) > 0 
                            THEN CAST(TRY_CAST(Occupied AS INT) - 1 AS VARCHAR(50)) 
                            ELSE '0' 
                        END 
                        WHERE Typeofsharing = @roomType", con);
                    decCmd.Parameters.AddWithValue("@roomType", roomType);
                    decCmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves all current room allocations showing which room is occupied by which student.
        /// </summary>
        /// <returns>A list of <see cref="RoomAllocationModel"/> items.</returns>
        public List<RoomAllocationModel> GetRoomAllocations()
        {
            var list = new List<RoomAllocationModel>();
            try
            {
                using var con = CreateConnection();
                con.Open();

                const string sql = @"
                    SELECT r.Request_id, r.user_id, 
                           ISNULL(u.name, 'Student #' + CAST(r.user_id AS VARCHAR(20))) AS StudentName,
                           ISNULL(u.phone, 'N/A') AS StudentPhone, 
                           ISNULL(u.email, 'N/A') AS StudentEmail,
                           ISNULL(u.gender, 'N/A') AS StudentGender, 
                           ISNULL(u.age, 0) AS StudentAge,
                           r.RoomType, r.Rent, r.Status, 
                           ISNULL(u.join_date, '2000-01-01') AS JoinDate
                    FROM Requesttab r
                    LEFT JOIN Usertab u ON r.user_id = u.id
                    WHERE r.Status = 'Approved'
                    ORDER BY r.RoomType, u.name";

                using var cmd = new SqlCommand(sql, con);
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new RoomAllocationModel
                    {
                        RequestId = Convert.ToInt32(dr["Request_id"]),
                        UserId = Convert.ToInt32(dr["user_id"]),
                        StudentName = dr["StudentName"]?.ToString() ?? "",
                        StudentPhone = dr["StudentPhone"]?.ToString() ?? "",
                        StudentEmail = dr["StudentEmail"]?.ToString() ?? "",
                        StudentGender = dr["StudentGender"]?.ToString() ?? "",
                        StudentAge = Convert.ToInt32(dr["StudentAge"]),
                        RoomType = dr["RoomType"]?.ToString() ?? "",
                        Rent = dr["Rent"]?.ToString() ?? "",
                        Status = dr["Status"]?.ToString() ?? "",
                        JoinDate = Convert.ToDateTime(dr["JoinDate"])
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }

        /// <summary>
        /// Retrieves all room booking applications submitted by a specific student user.
        /// </summary>
        /// <param name="uid">The student user registration id.</param>
        /// <returns>A list of <see cref="Requstcls"/> for the given user.</returns>
        public List<Requstcls> GetUserRequests(int uid)
        {
            var list = new List<Requstcls>();
            try
            {
                using var con = CreateConnection();
                con.Open();

                using var cmd = new SqlCommand("SELECT Request_id, user_id, RoomType, Rent, Status FROM Requesttab WHERE user_id=@uid ORDER BY Request_id DESC", con);
                cmd.Parameters.AddWithValue("@uid", uid);

                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new Requstcls
                    {
                        RequestId = dr["Request_id"] != DBNull.Value ? Convert.ToInt32(dr["Request_id"]) : 0,
                        User_id = dr["user_id"] != DBNull.Value ? Convert.ToInt32(dr["user_id"]) : 0,
                        RoomType = dr["RoomType"]?.ToString() ?? "",
                        Rent = dr["Rent"]?.ToString() ?? "",
                        Status = dr["Status"]?.ToString() ?? ""
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }

        /// <summary>
        /// Updates the occupied bed count for a specific room type category.
        /// </summary>
        /// <param name="roomType">The room sharing type name.</param>
        /// <param name="newOccupied">The updated number of occupied beds.</param>
        /// <returns>A message indicating update completion.</returns>
        public string UpdateRoomOccupied(string roomType, int newOccupied)
        {
            try
            {
                using var con = CreateConnection();
                con.Open();

                using var cmd = new SqlCommand("sp_update", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@roomType", roomType ?? "");
                cmd.Parameters.AddWithValue("@newOccupied", newOccupied);
                cmd.ExecuteNonQuery();
                return "Room updated successfully!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Retrieves the list of all registered student records with their allocated room for administrator management.
        /// </summary>
        /// <returns>A list of <see cref="Usercls"/> entities containing student profiles and room assignments.</returns>
        public List<Usercls> GetRegisteredUsers()
        {
            var list = new List<Usercls>();
            try
            {
                using var con = CreateConnection();
                con.Open();

                const string sql = @"
                    SELECT u.id, u.name, u.gender, u.age, u.phone, u.email, u.address, u.join_date,
                           (SELECT TOP 1 r.RoomType 
                            FROM Requesttab r 
                            WHERE r.user_id = u.id AND r.Status = 'Approved' 
                            ORDER BY r.Request_id DESC) AS AllocatedRoom
                    FROM Usertab u
                    ORDER BY u.id DESC";

                using var cmd = new SqlCommand(sql, con);
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new Usercls
                    {
                        Student_id = dr["id"] != DBNull.Value ? Convert.ToInt32(dr["id"]) : 0,
                        Name = dr["name"]?.ToString() ?? "",
                        Gender = dr["gender"]?.ToString() ?? "",
                        Age = dr["age"] != DBNull.Value ? Convert.ToInt32(dr["age"]) : 0,
                        Phone = dr["phone"]?.ToString() ?? "",
                        Email = dr["email"]?.ToString() ?? "",
                        Address = dr["address"]?.ToString() ?? "",
                        JoinDate = dr["join_date"] != DBNull.Value ? Convert.ToDateTime(dr["join_date"]) : DateTime.MinValue,
                        AllocatedRoom = dr["AllocatedRoom"] != DBNull.Value ? dr["AllocatedRoom"].ToString() : null
                    });
                }
            }
            catch (Exception)
            {
                throw;
            }
            return list;
        }
    }
}
