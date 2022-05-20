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
    public class RoleDefinationController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public RoleDefinationController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<RoleDefinationController>
        [HttpGet]
        public IActionResult Get()
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from roledefination";
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                List<quadruplets> dict = new List<quadruplets>();
                while (rdr.Read())
                {
                    quadruplets t = new quadruplets();
                    t.ID = Convert.ToInt32(rdr["ID"]);
                    if (rdr["Parent"] != DBNull.Value)
                        t.Parent = Convert.ToInt32(rdr["Parent"]);
                    else
                        t.Parent = null;
                    t.Node = rdr["Node"].ToString();
                    t.value = Convert.ToBoolean(rdr["value"]);
                    dict.Add(t);
                }
                string json = string.Empty;
                json = Create_TreeJson(dict, null);
                json = json.Remove(json.Length - 1, 1);
                return Ok(json);
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

        private string Create_TreeJson(List<quadruplets> dict, int? p)
        {
            List<int> index = getChildes(dict, p);
            string json = string.Empty;
            foreach (int n in index)
            {
                json = json + "{";
                json = json + "\"text\":\"" + dict[n].Node + "\",";
                json = json + "\"value\":\"" + dict[n].ID + "\",";
                json = json + "\"checked\":" + dict[n].value.ToString().ToLower();
                string st = Create_TreeJson(dict, dict[n].ID);
                if (!string.IsNullOrEmpty(st))
                {
                    if (st.EndsWith("},"))
                        st = st.Remove(st.Length - 1, 1);
                    json = json + ",\"children\": [" + st + "]";
                }
                json = json + "},";

            }
            return json;
        }

        private List<int> getChildes(List<quadruplets> dict, int? p)
        {
            List<int> ind = new List<int>();
            int count = 0;
            foreach (quadruplets t in dict)
            {
                if (t.Parent == p)
                    ind.Add(count);
                count++;
            }
            return ind;
        }

        // POST api/<RoleDefinationController>
        [HttpPost]
        public IActionResult Post([FromBody] quadruplets qp)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "insert into roledefination (Parent,Node,Value) values (@Parent,@Node,@Value)";
                MySqlCommand cmd = new MySqlCommand(sql, connection){ CommandType = CommandType.Text };
                cmd.Parameters.Add(new MySqlParameter("@Parent", MySqlDbType.VarChar)).Value = qp.Parent;
                cmd.Parameters.Add(new MySqlParameter("@Node", MySqlDbType.VarChar)).Value = qp.Node;
                cmd.Parameters.Add(new MySqlParameter("@Value", MySqlDbType.Int16)).Value = Convert.ToUInt16(qp.value);
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

        // PUT api/<RoleDefinationController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] quadruplets qp)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "update roledefination set Parent = @Parent ,Node = @Node, Value = @Value where ID =" + id;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                cmd.Parameters.Add(new MySqlParameter("@Parent", MySqlDbType.VarChar)).Value = qp.Parent;
                cmd.Parameters.Add(new MySqlParameter("@Node", MySqlDbType.VarChar)).Value = qp.Node;
                cmd.Parameters.Add(new MySqlParameter("@Value", MySqlDbType.Int16)).Value = Convert.ToUInt16(qp.value);
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

        // DELETE api/<RoleDefinationController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from roledefination where ID = " + id;
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
