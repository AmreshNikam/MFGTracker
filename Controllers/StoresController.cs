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
    public class StoresController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public StoresController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<StoresController>
        [HttpGet("{CompanyID}/{PlantID}")]
        public IActionResult Get(string CompanyID, string PlantID)
        {
            List<Stores> stores = new List<Stores>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from Stores where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Stores store = new Stores();
                    if (rdr["Stores_id"] != DBNull.Value)
                        store.Stores_id = Convert.ToInt32(rdr["Stores_id"]);
                    else
                        store.Stores_id = null;
                    if (rdr["Stores_Name"] != DBNull.Value)
                        store.Stores_Name = rdr["Stores_Name"].ToString();
                    else
                        store.Stores_Name = null;
                    store.Plant_Id = rdr["Plant_Id"].ToString();
                    store.Company_Id = rdr["company_id"].ToString();
                    stores.Add(store);
                }
                return Ok(stores);
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

        // GET api/<StoresController>/5
        [HttpGet("{StoresID}")]
        public IActionResult Get(int StoresID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from Stores where Stores_id = " + StoresID;
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                Stores store = new Stores();
                if (rdr.Read())
                {
                    
                    if (rdr["Stores_id"] != DBNull.Value)
                        store.Stores_id = Convert.ToInt32(rdr["Stores_id"]);
                    else
                        store.Stores_id = null;
                    if (rdr["Stores_Name"] != DBNull.Value)
                        store.Stores_Name = rdr["Stores_Name"].ToString();
                    else
                        store.Stores_Name = null;
                    store.Plant_Id = rdr["Plant_Id"].ToString();
                    store.Company_Id = rdr["company_id"].ToString();
                }
                return Ok(store);
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
        // POST api/<StoresController>
        [HttpPost]
        public IActionResult Post([FromBody] Stores store)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            int? StoreID = null; ;
            try
            {
                connection.Open();
                string sql = "Insert into Stores (Stores_Name, Plant_Id, Company_Id) values(@Stores_Name, @Plant_Id, @Company_Id)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (store.Stores_Name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Stores_Name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Stores_Name", MySqlDbType.VarChar)).Value = store.Stores_Name;
                if (store.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = store.Plant_Id;
                if (store.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = store.Company_Id;
                cmd.ExecuteScalar();
                StoreID = Convert.ToInt32(cmd.LastInsertedId);
                sql = "Insert into Storages (StoreID) values(@StoreID)";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                cmd.Parameters.Add(new MySqlParameter("@StoreID", MySqlDbType.Int32)).Value = StoreID;
                cmd.ExecuteScalar();

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

        // PUT api/<StoresController>/5
        [HttpPut("{StoresID}")]
        public IActionResult Put(int StoresID, [FromBody] Stores store)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Update Stores set Stores_id = @Stores_id, Stores_Name = @Stores_Name, Plant_Id = @Plant_Id, Company_Id = @Company_Id where Stores_id = " + StoresID;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (store.Stores_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Stores_id", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Stores_id", MySqlDbType.Int32)).Value = store.Stores_id;
                if (store.Stores_Name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Stores_Name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Stores_Name", MySqlDbType.VarChar)).Value = store.Stores_Name;
                if (store.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = store.Plant_Id;
                if (store.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = store.Company_Id;
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

        // DELETE api/<StoresController>/5
        [HttpDelete("{StoresID}")]
        public IActionResult Delete(int StoresID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from Stores where Stores_id = " + StoresID;
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
