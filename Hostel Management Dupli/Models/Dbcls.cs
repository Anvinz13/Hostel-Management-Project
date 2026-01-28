using System.Data;
using System.Data.SqlClient;

namespace Hostel_Management_Dupli.Models
{
    public class Dbcls
    {

        SqlConnection con = new SqlConnection(@"server=LAPTOP-6U7QMFB9\SQLEXPRESS;database=Hostel dupli;Integrated security=true");

        public string AdminInsert(Admincls objcls)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_maximum", con);
                cmd.CommandType = CommandType.StoredProcedure;

                object mxregid = cmd.ExecuteScalar();
                int regid = 0;
                if (mxregid == null || mxregid == DBNull.Value)
                {
                    regid = 1;
                }
                else
                {
                    int orgregid = Convert.ToInt32(mxregid);
                    regid = orgregid + 1;
                }




                //ADMIN INSERT

                SqlCommand cmd1 = new SqlCommand("sp_admininsert", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@id", regid);
                cmd1.Parameters.AddWithValue("@admin_name", objcls.Name);
                cmd1.Parameters.AddWithValue("@admin_phone", objcls.Phone);
                cmd1.Parameters.AddWithValue("@admin_email", objcls.Email);

                cmd1.ExecuteNonQuery();

                //LOGIN INSERT


                SqlCommand cmd2 = new SqlCommand("sp_insertlogin", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.AddWithValue("@rid", regid);
                cmd2.Parameters.AddWithValue("@una", objcls.Username);
                cmd2.Parameters.AddWithValue("@pwd", objcls.Password);
                cmd2.Parameters.AddWithValue("@ltype", "Admin");
                cmd2.ExecuteNonQuery();
                return ("Insertion success!!!");
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public string UserInsert(Usercls clsobj)
        {

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_maximum", con);
                cmd.CommandType = CommandType.StoredProcedure;

                object mxregid = cmd.ExecuteScalar();
                int regid = 0;
                if (mxregid == null || mxregid == DBNull.Value)
                {
                    regid = 1;
                }
                else
                {
                    int orgregid = Convert.ToInt32(mxregid);
                    regid = orgregid + 1;
                }




                //UserInsertion

                SqlCommand cmd1 = new SqlCommand("sp_userinsert", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@id", regid);
                cmd1.Parameters.AddWithValue("@name", clsobj.Name);
                cmd1.Parameters.AddWithValue("@gen", clsobj.Gender);
                cmd1.Parameters.AddWithValue("@age", clsobj.Age);
                cmd1.Parameters.AddWithValue("@phone", clsobj.Phone);
                cmd1.Parameters.AddWithValue("@email", clsobj.Email);
                cmd1.Parameters.AddWithValue("@addr", clsobj.Address);
                cmd1.Parameters.AddWithValue("@joind", clsobj.JoinDate);
                cmd1.ExecuteNonQuery();

                //LOGIN INSERT


                SqlCommand cmd2 = new SqlCommand("sp_insertlogin", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.AddWithValue("@rid", regid);
                cmd2.Parameters.AddWithValue("@una", clsobj.Username);
                cmd2.Parameters.AddWithValue("@pwd", clsobj.Password);
                cmd2.Parameters.AddWithValue("@ltype", "User");
                cmd2.ExecuteNonQuery();
                return ("Insertion success!!!");
            }
            catch (Exception ex)
            {

                return ex.Message.ToString();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public Logincls LoginDB(Logincls objcls)
        {
            var result = new Logincls();
            try
            {
                SqlCommand cmd1 = new SqlCommand("sp_countid", con);
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.Parameters.AddWithValue("@uname", objcls.Username);
                cmd1.Parameters.AddWithValue("@pwd", objcls.Password);
                con.Open();
                int cid = Convert.ToInt32(cmd1.ExecuteScalar().ToString());
                con.Close();


                if (cid == 1)
                {
                    SqlCommand cmd2 = new SqlCommand("sp_regid", con);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@una", objcls.Username);
                    cmd2.Parameters.AddWithValue("@pwd", objcls.Password);
                    con.Open();
                    result.regid = Convert.ToInt32(cmd2.ExecuteScalar());
                    con.Close();

                    SqlCommand cmd3 = new SqlCommand("sp_logintype", con);
                    cmd3.CommandType = CommandType.StoredProcedure;
                    cmd3.Parameters.AddWithValue("@una", objcls.Username);
                    cmd3.Parameters.AddWithValue("@pwd", objcls.Password);
                    con.Open();
                    result.Logtype = cmd3.ExecuteScalar().ToString();
                    con.Close();

                    result.Username = objcls.Username;
                    result.Password = objcls.Password;
                }
                else
                {
                    result.regid = 0;
                }
            }
            catch (Exception ex)
            {
                con.Close();
                throw ex;
            }
            return result;



        }
        public string AddRoomInsert(Addroommodel obj)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("sp_rooms", con);
                cmd.CommandType = CommandType.StoredProcedure;

                // Match your SP parameters exactly
                cmd.Parameters.AddWithValue("@rent", obj.Rent);
                cmd.Parameters.AddWithValue("@typesharing", obj.Roomtype);
                cmd.Parameters.AddWithValue("@occupied", obj.Occupied);

                cmd.ExecuteNonQuery();

                return "Room Added Successfully!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        public List<Addroommodel> GetAllRooms()
        {
            List<Addroommodel> roomList = new List<Addroommodel>();

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_getAllRooms", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Addroommodel room = new Addroommodel();
                    room.Rent = dr["Rent"].ToString();
                    room.Roomtype = dr["Typeofsharing"].ToString();
                    room.Occupied = dr["Occupied"].ToString();

                    roomList.Add(room);
                }

                dr.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return roomList;
        }

        public string InsertRoomRequest(Requstcls obj)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_insertrequest", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@uid", obj.User_id);
                cmd.Parameters.AddWithValue("@rtype", obj.RoomType);
                cmd.Parameters.AddWithValue("@rent", obj.Rent);
                cmd.Parameters.AddWithValue("@status", obj.Status);

                cmd.ExecuteNonQuery();
                return "Request sent successfully!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                con.Close();
            }
        }

        public List<Requstcls> GetPendingRequests()
        {
            List<Requstcls> list = new List<Requstcls>();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_pendingreq", con);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new Requstcls
                    {
                        RequestId = Convert.ToInt32(dr["Request_id"]),
                        User_id = Convert.ToInt32(dr["user_id"]),
                        RoomType = dr["RoomType"].ToString(),
                        Rent = dr["Rent"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
                dr.Close();
            }
            finally
            {
                con.Close();
            }
            return list;
        }

        public void UpdateRequestStatus(int rid, string status)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Requesttab SET Status=@status WHERE Request_id=@rid", con);
                cmd.Parameters.AddWithValue("@rid", rid);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                con.Close();
            }
        }

        public List<Requstcls> GetUserRequests(int uid)
        {
            List<Requstcls> list = new List<Requstcls>();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Requesttab WHERE user_id=@uid", con);
                cmd.Parameters.AddWithValue("@uid", uid);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new Requstcls
                    {
                        RequestId = Convert.ToInt32(dr["Request_id"]),
                        User_id = Convert.ToInt32(dr["user_id"]),
                        RoomType = dr["RoomType"].ToString(),
                        Rent = dr["Rent"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
                dr.Close();
            }
            finally
            {
                con.Close();
            }
            return list;
        }

        public string UpdateRoomOccupied(string roomType, int newOccupied)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_update", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@roomType", roomType);
                cmd.Parameters.AddWithValue("@newOccupied", newOccupied);
                cmd.ExecuteNonQuery();
                return "Room updated successfully!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

    }
}
