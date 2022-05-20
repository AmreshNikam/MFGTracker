using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MFG_Tracker.DatabaseTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MFG_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCredentialsController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public UserCredentialsController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<UserCredentialsController>
        [HttpGet("{CompanyID}/{PlantID}")]
        public IActionResult Get(string CompanyID, string PlantID)
        {
            List<UserCredentials> users = new List<UserCredentials>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from user_credentials where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UserCredentials user = new UserCredentials();
                    if (rdr["User_name"] != DBNull.Value)
                        user.User_name = rdr["User_name"].ToString();
                    else
                        user.User_name = null;
                    if (rdr["Password"] != DBNull.Value)
                        user.Password = rdr["Password"].ToString();
                    else
                        user.Password = null;
                    if (rdr["Role"] != DBNull.Value)
                        user.Role = Convert.ToInt32(rdr["Role"]);
                    else
                        user.Role = null;
                    if (rdr["Active_Inactive"] != DBNull.Value)
                        user.Active_Inactive = Convert.ToBoolean(rdr["Active_Inactive"]);
                    else
                        user.Active_Inactive = null;
                    if (rdr["BP_ShortCode"] != DBNull.Value)
                        user.BP_ShortCode = rdr["BP_ShortCode"].ToString();
                    else
                        user.BP_ShortCode = null;
                    if (rdr["Plant_Id"] != DBNull.Value)
                        user.Plant_Id = rdr["Plant_Id"].ToString();
                    else
                        user.Plant_Id = null;
                    if (rdr["Company_Id"] != DBNull.Value)
                        user.Company_Id = rdr["Company_Id"].ToString();
                    else
                        user.Company_Id = null;
                    users.Add(user);
                }
                return Ok(users);
            }
            catch (MySqlException ex)
            {
                return StatusCode(600, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        // GET api/<UserCredentialsController>/5
        [HttpGet("{CompanyID}/{PlantID}/{BPID}")]
        public IActionResult Get(string CompanyID, string PlantID, string BPID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from user_credentials where BP_ShortCode = '" + BPID + "' and Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                UserCredentials user = new UserCredentials();
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    
                    if (rdr["User_name"] != DBNull.Value)
                        user.User_name = rdr["User_name"].ToString();
                    else
                        user.User_name = null;
                    if (rdr["Password"] != DBNull.Value)
                        user.Password = rdr["Password"].ToString();
                    else
                        user.Password = null;
                    if (rdr["Role"] != DBNull.Value)
                        user.Role = Convert.ToInt32(rdr["Role"]);
                    else
                        user.Role = null;
                    if (rdr["Active_Inactive"] != DBNull.Value)
                        user.Active_Inactive = Convert.ToBoolean(rdr["Active_Inactive"]);
                    else
                        user.Active_Inactive = null;
                    if (rdr["BP_ShortCode"] != DBNull.Value)
                        user.BP_ShortCode = rdr["BP_ShortCode"].ToString();
                    else
                        user.BP_ShortCode = null;
                    if (rdr["Plant_Id"] != DBNull.Value)
                        user.Plant_Id = rdr["Plant_Id"].ToString();
                    else
                        user.Plant_Id = null;
                    if (rdr["Company_Id"] != DBNull.Value)
                        user.Company_Id = rdr["Company_Id"].ToString();
                    else
                        user.Company_Id = null;
                }
                return Ok(user);
            }
            catch (MySqlException ex)
            {
                return StatusCode(600, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        //
        [HttpGet]
        public IActionResult Get([FromQuery] string User_name, string Password, string Company_Id, string Plant_Id)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "select UC.User_name, UC.Password, UC.Active_Inactive, UC.BP_ShortCode,BP.Name,RL.Json " +
                             "from user_credentials as UC " +
                             "left join business_partner as BP on UC.BP_ShortCode = BP.BP_Short_code and UC.Company_ID = BP.company_id and UC.Plant_ID = BP.Plant_Id " +
                             "left join role as RL on UC.Role = RL.RoleID and UC.Company_ID = BP.company_id and UC.Plant_ID = BP.Plant_Id " +
                             "where UC.User_name = '" + User_name + "' and UC.Company_ID = '" + Company_Id + "' and UC.Plant_Id = '" + Plant_Id + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                UserCredentials user = new UserCredentials();
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    LoginResonse Lr = new LoginResonse();
                    if (rdr["Password"].ToString() != Password)
                        return StatusCode(700, "Invalid Password");
                    if (Convert.ToBoolean(rdr["Active_Inactive"]) != true)
                        return StatusCode(700, "User is not Active");
                    Lr.LoginName = rdr["User_name"].ToString();
                    Lr.Password = rdr["Password"].ToString();
                    Lr.Active_Inactive = Convert.ToBoolean(rdr["Active_Inactive"]);
                    Lr.UserName = rdr["Name"].ToString();
                    Lr.BPShortCode = rdr["BP_ShortCode"].ToString();
                    Lr.Json = rdr["Json"].ToString();
                    return Ok(Lr);
                }
                else
                    return StatusCode(700, "Invalid User Name");
            }
            catch (MySqlException ex)
            {
                return StatusCode(600, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        // POST api/<UserCredentialsController>
        [HttpPost]
        public IActionResult Post([FromBody] UserCredentials user)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Insert into user_credentials (User_name, Password, Role, Active_Inactive, BP_ShortCode, Plant_ID, Company_ID) values(@User_name, @Password, @Role, @Active_Inactive, @BP_ShortCode, @Plant_Id, @Company_Id)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (user.User_name == null)
                    cmd.Parameters.Add(new MySqlParameter("@User_name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@User_name", MySqlDbType.VarChar)).Value = user.User_name;
                if (user.Password == null)
                    cmd.Parameters.Add(new MySqlParameter("@Password", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Password", MySqlDbType.VarChar)).Value = user.Password;
                if (user.Role == null)
                    cmd.Parameters.Add(new MySqlParameter("@Role", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Role", MySqlDbType.Int32)).Value = user.Role;
                if (user.Active_Inactive == null)
                    cmd.Parameters.Add(new MySqlParameter("@Active_Inactive", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Active_Inactive", MySqlDbType.Int16)).Value = Convert.ToUInt16( user.Active_Inactive);
                if (user.BP_ShortCode == null)
                    cmd.Parameters.Add(new MySqlParameter("@BP_ShortCode", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BP_ShortCode", MySqlDbType.VarChar)).Value = user.BP_ShortCode;
                if (user.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.Int16)).Value = user.Plant_Id;
                if (user.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.Int16)).Value = user.Company_Id;
                cmd.ExecuteNonQuery();
                return Ok("Record Inserted Successfuly");
            }
            catch (MySqlException ex)
            {
                return StatusCode(600, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        // PUT api/<UserCredentialsController>/5
        [HttpPut("{CompanyID}/{PlantID}/{BPID}")]
        public IActionResult Put(string CompanyID, string PlantID, string BPID, [FromBody] UserCredentials user)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Update user_credentials set User_name = @User_name, Password = @Password, Role = @Role, Active_Inactive = @Active_Inactive, BP_ShortCode = @BP_ShortCode, Plant_ID = @Plant_ID, Company_ID = @Company_ID where BP_ShortCode = '" + BPID + "' and Plant_ID = '" + PlantID + "' and Company_ID = '" + CompanyID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (user.User_name == null)
                    cmd.Parameters.Add(new MySqlParameter("@User_name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@User_name", MySqlDbType.VarChar)).Value = user.User_name;
                if (user.Password == null)
                    cmd.Parameters.Add(new MySqlParameter("@Password", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Password", MySqlDbType.VarChar)).Value = user.Password;
                if (user.Role == null)
                    cmd.Parameters.Add(new MySqlParameter("@Role", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Role", MySqlDbType.Int32)).Value = user.Role;
                if (user.Active_Inactive == null)
                    cmd.Parameters.Add(new MySqlParameter("@Active_Inactive", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Active_Inactive", MySqlDbType.Int16)).Value = user.Active_Inactive;
                if (user.BP_ShortCode == null)
                    cmd.Parameters.Add(new MySqlParameter("@BP_ShortCode", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BP_ShortCode", MySqlDbType.VarChar)).Value = user.BP_ShortCode;
                if (user.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_ID", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_ID", MySqlDbType.Int16)).Value = user.Plant_Id;
                if (user.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_ID", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_ID", MySqlDbType.Int16)).Value = user.Company_Id;
                cmd.ExecuteNonQuery();
                return Ok("Record updated Successfuly");
            }
            catch (MySqlException ex)
            {
                return StatusCode(600, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        // DELETE api/<UserCredentialsController>/5
        [HttpDelete("{CompanyID}/{PlantID}/{BPID}")]
        public IActionResult Delete(string CompanyID, string PlantID, string BPID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from user_credentials where BP_ShortCode = '" + BPID + "' and Plant_ID = '" + PlantID + "' and Company_ID = '" + CompanyID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                cmd.ExecuteNonQuery();
                connection.Close();
                return Ok("Successfuly deleted");
            }
            catch (MySqlException ex)
            {
                return StatusCode(600, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
