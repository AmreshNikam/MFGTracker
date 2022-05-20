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
    public class SkidController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public SkidController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<SkidController>
        [HttpGet("{CompanyID}/{PlantID}")]
        public IActionResult Get(string CompanyID, string PlantID)
        {
            List<Skid> skids = new List<Skid>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from Skid where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Skid skid = new Skid();
                    if (rdr["Skid_config_id"] != DBNull.Value)
                        skid.Skid_config_id = Convert.ToInt32(rdr["Skid_config_id"]);
                    else
                        skid.Skid_config_id = null;
                    if (rdr["JigsPerSkid"] != DBNull.Value)
                        skid.JigsPerSkid = Convert.ToInt32(rdr["JigsPerSkid"]);
                    else
                        skid.JigsPerSkid = null;
                    if (rdr["PartsPerJigs"] != DBNull.Value)
                        skid.PartsPerJigs = Convert.ToInt32(rdr["PartsPerJigs"]);
                    else
                        skid.PartsPerJigs = null;
                    skid.Plant_Id = rdr["Plant_Id"].ToString();
                    skid.Company_Id = rdr["company_id"].ToString();
                    skids.Add(skid);
                }
                return Ok(skids);
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

        // GET api/<SkidController>/5
        [HttpGet("{ID}")]
        public IActionResult Get(int ID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from skid where Skid_config_id = " + ID;
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                Skid skid = new Skid();
                if (rdr.Read())
                {
                    if (rdr["Skid_config_id"] != DBNull.Value)
                        skid.Skid_config_id = Convert.ToInt32(rdr["Skid_config_id"]);
                    else
                        skid.Skid_config_id = null;
                    if (rdr["JigsPerSkid"] != DBNull.Value)
                        skid.JigsPerSkid = Convert.ToInt32(rdr["JigsPerSkid"]);
                    else
                        skid.JigsPerSkid = null;
                    if (rdr["PartsPerJigs"] != DBNull.Value)
                        skid.PartsPerJigs = Convert.ToInt32(rdr["PartsPerJigs"]);
                    else
                        skid.PartsPerJigs = null;
                    skid.Plant_Id = rdr["Plant_Id"].ToString();
                    skid.Company_Id = rdr["company_id"].ToString();
                }
                return Ok(skid);
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

        // POST api/<SkidController>
        [HttpPost]
        public IActionResult Post([FromBody] Skid skid)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Insert into skid (JigsPerSkid, PartsPerJigs, Plant_Id, Company_Id) values(@JigsPerSkid, @PartsPerJigs, @Plant_Id, @Company_Id)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (skid.JigsPerSkid == null)
                    cmd.Parameters.Add(new MySqlParameter("@JigsPerSkid", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@JigsPerSkid", MySqlDbType.Int32)).Value = skid.JigsPerSkid;
                if (skid.PartsPerJigs == null)
                    cmd.Parameters.Add(new MySqlParameter("@PartsPerJigs", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PartsPerJigs", MySqlDbType.Int32)).Value = skid.PartsPerJigs;
                if (skid.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = skid.Plant_Id;
                if (skid.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = skid.Company_Id;
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

        // PUT api/<SkidController>/5
        [HttpPut("{ID}")]
        public IActionResult Put(int ID, [FromBody] Skid skid)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Update skid set JigsPerSkid = @JigsPerSkid, PartsPerJigs = @PartsPerJigs, Plant_ID = @Plant_ID, Company_ID = @Company_ID where Skid_config_id = " + ID;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (skid.JigsPerSkid == null)
                    cmd.Parameters.Add(new MySqlParameter("@JigsPerSkid", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@JigsPerSkid", MySqlDbType.Int32)).Value = skid.JigsPerSkid;
                if (skid.PartsPerJigs == null)
                    cmd.Parameters.Add(new MySqlParameter("@PartsPerJigs", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PartsPerJigs", MySqlDbType.Int32)).Value = skid.PartsPerJigs;
                if (skid.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = skid.Plant_Id;
                if (skid.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = skid.Company_Id;
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

        // DELETE api/<SkidController>/5
        [HttpDelete("{ID}")]
        public IActionResult Delete(int ID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from skid where Skid_config_id = " + ID;
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
