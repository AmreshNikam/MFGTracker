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
    public class StoragesController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public StoragesController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<StoragesController>
        
        [HttpGet("{StoreID}")]
        public IActionResult Get(int StoreID)
        {
            List<Storages> storages = new List<Storages>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from Storages where StoreID = " + StoreID;
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Storages storage = new Storages();
                    if (rdr["Storage_id"] != DBNull.Value)
                        storage.Storage_id = Convert.ToInt32(rdr["Storage_id"]);
                    else
                        storage.Storage_id = null;
                    if (rdr["StoreID"] != DBNull.Value)
                        storage.StoreID = Convert.ToInt32(rdr["StoreID"]);
                    else
                        storage.StoreID = null;
                    if (rdr["Section"] != DBNull.Value)
                        storage.Section = rdr["Section"].ToString();
                    else
                        storage.Section = null;
                    if (rdr["Rack"] != DBNull.Value)
                        storage.Rack = rdr["Rack"].ToString();
                    else
                        storage.Rack = null;
                    if (rdr["Shelf"] != DBNull.Value)
                        storage.Shelf = rdr["Shelf"].ToString();
                    else
                        storage.Shelf = null;
                    if (rdr["Bins"] != DBNull.Value)
                        storage.Bins = rdr["Bins"].ToString();
                    else
                        storage.Bins = null;
                    string Store = storage.StoreID.ToString().PadLeft(3, '0');
                    string Section = storage.Section == null ? "000" : storage.Section.ToString().PadLeft(3,'0');
                    string Rack = storage.Rack == null ? "000" : storage.Rack.ToString().PadLeft(3, '0');
                    string Shelf = storage.Shelf == null ? "000" : storage.Shelf.ToString().PadLeft(3, '0');
                    string Bins = storage.Bins == null ? "000" : storage.Bins.ToString().PadLeft(3, '0');

                    storage.Code = Store+ Section+ Rack+ Shelf+ Bins;
                    storages.Add(storage);
                }
                return Ok(storages);
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

        // GET api/<StoragesController>/5
        [HttpGet("[action]/{Storage}")]
        public IActionResult GetStorage(int StorageID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from Storages where Storage_id = '" + StorageID;
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                Storages storage = new Storages();
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    
                    if (rdr["Storage_id"] != DBNull.Value)
                        storage.Storage_id = Convert.ToInt32(rdr["Storage_id"]);
                    else
                        storage.Storage_id = null;
                    if (rdr["StoreID"] != DBNull.Value)
                        storage.StoreID = Convert.ToInt32(rdr["StoreID"]);
                    else
                        storage.StoreID = null;
                    if (rdr["Section"] != DBNull.Value)
                        storage.Section = rdr["Section"].ToString();
                    else
                        storage.Section = null;
                    if (rdr["Rack"] != DBNull.Value)
                        storage.Rack = rdr["Rack"].ToString();
                    else
                        storage.Rack = null;
                    if (rdr["Shelf"] != DBNull.Value)
                        storage.Shelf = rdr["Shelf"].ToString();
                    else
                        storage.Shelf = null;
                    if (rdr["Bins"] != DBNull.Value)
                        storage.Bins = rdr["Bins"].ToString();
                    else
                        storage.Bins = null;
                    string Store = storage.StoreID.ToString().PadLeft(3, '0');
                    string Section = storage.Section == null ? "000" : storage.Section.ToString().PadLeft(3, '0');
                    string Rack = storage.Rack == null ? "000" : storage.Rack.ToString().PadLeft(3, '0');
                    string Shelf = storage.Shelf == null ? "000" : storage.Shelf.ToString().PadLeft(3, '0');
                    string Bins = storage.Bins == null ? "000" : storage.Bins.ToString().PadLeft(3, '0');

                    storage.Code = Store + Section + Rack + Shelf + Bins;
                }
                return Ok(storage);
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

        // POST api/<StoragesController>
        [HttpPost]
        public IActionResult Post([FromBody] Storages storage)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Insert into Storages (StoreID, Section, Rack, Shelf, Bins) values(@StoreID, @Section, @Rack, @Shelf, @Bins)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (storage.StoreID == null)
                    cmd.Parameters.Add(new MySqlParameter("@StoreID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@StoreID", MySqlDbType.Int32)).Value = storage.StoreID;
                if (storage.Section == null)
                    cmd.Parameters.Add(new MySqlParameter("@Section", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Section", MySqlDbType.VarChar)).Value = storage.Section;
                if (storage.Rack == null)
                    cmd.Parameters.Add(new MySqlParameter("@Rack", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Rack", MySqlDbType.VarChar)).Value = storage.Rack;
                if (storage.Shelf == null)
                    cmd.Parameters.Add(new MySqlParameter("@Shelf", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Shelf", MySqlDbType.VarChar)).Value = storage.Shelf;
                if (storage.Bins == null)
                    cmd.Parameters.Add(new MySqlParameter("@Bins", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Bins", MySqlDbType.VarChar)).Value = storage.Bins;
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
        // PUT api/<StoragesController>/5
        [HttpPut("{StorageID}")]
        public IActionResult Put(int StorageID, [FromBody] Storages storage)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Update Storages set Storage_id = @Storage_id, StoreID = @StoreID, Section = @Section, Rack = @Rack, Shelf = @Shelf, Bins = @Bins where Storage_id = " + StorageID;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (storage.Storage_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Storage_id", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Storage_id", MySqlDbType.Int32)).Value = storage.Storage_id;
                if (storage.StoreID == null)
                    cmd.Parameters.Add(new MySqlParameter("@StoreID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@StoreID", MySqlDbType.Int32)).Value = storage.StoreID;
                if (storage.Section == null)
                    cmd.Parameters.Add(new MySqlParameter("@Section", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Section", MySqlDbType.VarChar)).Value = storage.Section;
                if (storage.Rack == null)
                    cmd.Parameters.Add(new MySqlParameter("@Rack", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Rack", MySqlDbType.VarChar)).Value = storage.Rack;
                if (storage.Shelf == null)
                    cmd.Parameters.Add(new MySqlParameter("@Shelf", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Shelf", MySqlDbType.VarChar)).Value = storage.Shelf;
                if (storage.Bins == null)
                    cmd.Parameters.Add(new MySqlParameter("@Bins", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Bins", MySqlDbType.VarChar)).Value = storage.Bins;
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
        [HttpPut]
        public IActionResult Put( [FromQuery] string storage, string barcode)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                int storage_id;
                int stroreID = Convert.ToInt32(storage.Substring(0, 3)); 
                string Section = storage.Substring(3,3) == "000"? "Section is null" : "Section = " + storage.Substring(3, 3);
                string Rack = storage.Substring(6, 3) == "000" ? "Rack is null" : "Rack = " + storage.Substring(6, 3);
                string Shelf = storage.Substring(9, 3) == "000" ? "Shelf is null" : "Shelf = " + storage.Substring(9, 3);
                string Bins = storage.Substring(12, 3) == "000" ? "Bins is null" : "Bins = " + storage.Substring(12, 3);
                string sql;
                sql = "select Storage_id from storages where StoreID = " + stroreID + "  and " + Section + " and " + Rack + " and " + Shelf + " and " + Bins;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                

                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    storage_id = Convert.ToInt32(rdr["Storage_id"]);
                }
                else
                    return StatusCode(700, "Can't find Storage Location");
                rdr.Close();
                sql = "Update barcodemaster set Storage_id = " + storage_id + " where Barcode = '" + barcode + "'";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
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

        // DELETE api/<StoragesController>/5
        [HttpDelete("{StorageID}")]
        public IActionResult Delete(int StorageID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from Storages where Storage_id = " + StorageID;
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
