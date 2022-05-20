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
    public class AssemblyController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public AssemblyController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<AssemblyController>
        [HttpGet("{SUBPART}")]
        public IActionResult Get(int SUBPART)
        {
            List<Assembly> assemblies = new List<Assembly>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Select * from assembly where Subpart_id = " + SUBPART;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Assembly assembly = new Assembly();
                    if (rdr["AssemblyID"] != DBNull.Value)
                        assembly.AssemblyID = Convert.ToInt32(rdr["AssemblyID"]);
                    else
                        assembly.AssemblyID = null;
                    if (rdr["AssemblyName"] != DBNull.Value)
                        assembly.AssemblyName = rdr["AssemblyName"].ToString();
                    else
                        assembly.AssemblyName = null;
                    if (rdr["Subpart_id"] != DBNull.Value)
                        assembly.Subpart_id = Convert.ToInt32(rdr["Subpart_id"]);
                    else
                        assembly.Subpart_id = null;
                    assemblies.Add(assembly);
                }
                return Ok(assemblies);
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

        
        // POST api/<AssemblyController>
        [HttpPost]
        public IActionResult Post([FromBody] Assembly assembly)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Insert into Assembly (AssemblyName, Subpart_id) values(@AssemblyName, @Subpart_id)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (assembly.AssemblyName == null)
                    cmd.Parameters.Add(new MySqlParameter("@AssemblyName", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@AssemblyName", MySqlDbType.VarChar)).Value = assembly.AssemblyName;
                if (assembly.Subpart_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_id", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_id", MySqlDbType.Int32)).Value = assembly.Subpart_id;
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

        // PUT api/<AssemblyController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Assembly assembly)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Update assembly set AssemblyID = @AssemblyID, AssemblyName = @AssemblyName, Subpart_id = @Subpart_id  where AssemblyID = " + id;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (assembly.AssemblyID == null)
                    cmd.Parameters.Add(new MySqlParameter("@AssemblyID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@AssemblyID", MySqlDbType.Int32)).Value = assembly.AssemblyID;
                if (assembly.AssemblyName == null)
                    cmd.Parameters.Add(new MySqlParameter("@AssemblyName", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@AssemblyName", MySqlDbType.VarChar)).Value = assembly.AssemblyName;
                if (assembly.Subpart_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_id", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_id", MySqlDbType.Int32)).Value = assembly.Subpart_id;
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

        // DELETE api/<AssemblyController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Delete from Assembly where AssemblyID = " + id;
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
