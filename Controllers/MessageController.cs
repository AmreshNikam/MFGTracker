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
    public class MessageController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public MessageController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }


        // GET api/<MessageController>/5
        [HttpGet("{ScanStationId}")]
        public IActionResult GetCompany(int ScanStationId)
        {
            List<Message> msgs = new List<Message>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Select * from Message where ScanStation = " + ScanStationId;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                
                while (rdr.Read())
                {

                    Message msg = new Message();
                    if (rdr["ID"] != DBNull.Value)
                        msg.ID = Convert.ToInt32(rdr["ID"]);
                    else
                        msg.ID = null;
                    if (rdr["MessageText"] != DBNull.Value)
                        msg.MessageText = rdr["MessageText"].ToString();
                    else
                        msg.MessageText = null;
                    if (rdr["ScanStation"] != DBNull.Value)
                        msg.ScanStation = Convert.ToInt32(rdr["ScanStation"]);
                    else
                        msg.ScanStation = null;
                    msgs.Add(msg);
                }
                return Ok(msgs);
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

        // POST api/<MessageController>
        [HttpPost]
        public IActionResult Post([FromBody] Message msg)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Insert into Message (MessageText, ScanStation) values(@MessageText, @ScanStation)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (msg.MessageText == null)
                    cmd.Parameters.Add(new MySqlParameter("@MessageText", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@MessageText", MySqlDbType.VarChar)).Value = msg.MessageText;
                if (msg.ScanStation == null)
                    cmd.Parameters.Add(new MySqlParameter("@ScanStation", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ScanStation", MySqlDbType.Int32)).Value = msg.ScanStation;
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

        // PUT api/<MessageController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Message msg)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Update Message set ID = @ID, MessageText = @MessageText, ScanStation = @ScanStation  where ID = " + id;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (msg.ID == null)
                    cmd.Parameters.Add(new MySqlParameter("@ID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ID", MySqlDbType.Int32)).Value = msg.ID;
                if (msg.MessageText == null)
                    cmd.Parameters.Add(new MySqlParameter("@MessageText", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@MessageText", MySqlDbType.VarChar)).Value = msg.MessageText;
                if (msg.ScanStation == null)
                    cmd.Parameters.Add(new MySqlParameter("@ScanStation", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ScanStation", MySqlDbType.Int32)).Value = msg.ScanStation;
                cmd.ExecuteNonQuery();
                return Ok("Record Updated Successfuly");
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

        // DELETE api/<MessageController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Delete from Message where ID = " + id;
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
