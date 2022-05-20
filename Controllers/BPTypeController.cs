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
    public class BPTypeController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public BPTypeController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<BPTypeController>
        [HttpGet]
        public IActionResult Get()
        {
            List<BPType> bptypes = new List<BPType>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Select * from businesspatnertype";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    BPType bptype = new BPType();
                    if (rdr["ID"] != DBNull.Value)
                        bptype.ID = Convert.ToInt32(rdr["ID"]);
                    else
                        bptype.ID = null;
                    if (rdr["Type"] != DBNull.Value)
                        bptype.Type = rdr["Type"].ToString();
                    else
                        bptype.Type = null;
                    bptypes.Add(bptype);
                }
                return Ok(bptypes);
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

        // GET api/<BPTypeController>/5
        [HttpGet("{ID}")]
        public IActionResult Get(int ID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Select * from businesspatnertype where ID = " + ID;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                BPType bptype = new BPType();
                if (rdr.Read())
                {
                   
                    if (rdr["ID"] != DBNull.Value)
                        bptype.ID = Convert.ToInt32(rdr["ID"]);
                    else
                        bptype.ID = null;
                    if (rdr["Type"] != DBNull.Value)
                        bptype.Type = rdr["Type"].ToString();
                    else
                        bptype.Type = null;
                }
                return Ok(bptype);
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

        // POST api/<BPTypeController>
        [HttpPost]
        public IActionResult Post([FromBody] BPType bptype)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Insert into businesspatnertype (type ) values(@type)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (bptype.Type == null)
                    cmd.Parameters.Add(new MySqlParameter("@type", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@type", MySqlDbType.VarChar)).Value = bptype.Type;
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

        // PUT api/<BPTypeController>/5
        [HttpPut("{ID}")]
        public IActionResult Put(int ID, [FromBody] BPType bptype)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Update businesspatnertype set ID = @ID, type = @type where ID = " + ID;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (bptype.ID == null)
                    cmd.Parameters.Add(new MySqlParameter("@ID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ID", MySqlDbType.Int32)).Value = bptype.ID;
                if (bptype.Type == null)
                    cmd.Parameters.Add(new MySqlParameter("@type", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@type", MySqlDbType.VarChar)).Value = bptype.Type;
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

        // DELETE api/<BPTypeController>/5
        [HttpDelete("{ID}")]
        public IActionResult Delete(int ID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Delete from businesspatnertype where ID = " + ID;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                cmd.ExecuteNonQuery();
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
