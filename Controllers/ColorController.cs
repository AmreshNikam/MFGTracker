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
    public class ColorController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public ColorController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        
        // GET api/<ColorController>/5
        [HttpGet("{SUBPART}")]
        public IActionResult Get(int SUBPART)
        {
            List<Color> colors = new List<Color>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Select * from color where Subpart_id = " + SUBPART;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Color col = new Color();
                    if (rdr["ColorID"] != DBNull.Value)
                        col.ColorID = Convert.ToInt32(rdr["ColorID"]);
                    else
                        col.ColorID = null;
                    if (rdr["ColorcolName"] != DBNull.Value)
                        col.ColorcolName = rdr["ColorcolName"].ToString();
                    else
                        col.ColorcolName = null;
                    if (rdr["Subpart_id"] != DBNull.Value)
                        col.Subpart_id = Convert.ToInt32(rdr["Subpart_id"]);
                    else
                        col.Subpart_id = null;
                    colors.Add(col);
                }
                return Ok(colors);
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

        // POST api/<ColorController>
        [HttpPost]
        public IActionResult Post([FromBody] Color colors)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Insert into color (ColorcolName, Subpart_id) values(@ColorcolName, @Subpart_id)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (colors.ColorcolName == null)
                    cmd.Parameters.Add(new MySqlParameter("@ColorcolName", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ColorcolName", MySqlDbType.VarChar)).Value = colors.ColorcolName;
                if (colors.Subpart_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_id", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_id", MySqlDbType.Int32)).Value = colors.Subpart_id;
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

        // PUT api/<ColorController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id,[FromBody] Color colors)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Update color set ColorID = @ColorID, ColorcolName = @ColorcolName, Subpart_id = @Subpart_id  where ColorID = " + id;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (colors.ColorID == null)
                    cmd.Parameters.Add(new MySqlParameter("@ColorID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ColorID", MySqlDbType.Int32)).Value = colors.ColorID;
                if (colors.ColorcolName == null)
                    cmd.Parameters.Add(new MySqlParameter("@ColorcolName", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ColorcolName", MySqlDbType.VarChar)).Value = colors.ColorcolName;
                if (colors.Subpart_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_id", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_id", MySqlDbType.Int32)).Value = colors.Subpart_id;
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

        // DELETE api/<ColorController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Delete from color where ColorID = " + id;
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
