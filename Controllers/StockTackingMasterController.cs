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
    public class StockTackingMasterController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public StockTackingMasterController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<StockTackingMasterController>
        [HttpGet("{CompanyID}/{PlantID}")]
        public IActionResult Get(string CompanyID,string PlantID, [FromQuery] DateTime? Today = null)
        {
            List<StockTackingMaster> stockTackings = new List<StockTackingMaster>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql;
                if(Today == null)
                    sql = "Select * from stocktakingmaster where Company_Id = '" + CompanyID + "' and Plant_Id = '" + PlantID + "'";
                else
                    sql = "Select * from stocktakingmaster where Company_Id = '" + CompanyID + "' and Plant_Id = '" + PlantID + "' and Date = '" + Today.Value.ToString("yyyy-MM-dd") + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    StockTackingMaster st = new StockTackingMaster();
                    if (rdr["StocktakingMasterID"] != DBNull.Value)
                        st.StocktakingMasterID = Convert.ToInt32(rdr["StocktakingMasterID"]);
                    else
                        st.StocktakingMasterID = null;
                    if (rdr["Date"] != DBNull.Value)
                        st.Date = Convert.ToDateTime(rdr["Date"]);
                    else
                        st.Date = null;
                    if (rdr["Location"] != DBNull.Value)
                        st.Location = Convert.ToInt32(rdr["Location"]);
                    else
                        st.Location = null;
                    st.Plant_Id = rdr["Plant_Id"].ToString();
                    st.Company_Id = rdr["Company_Id"].ToString();
                    st.BPShortCode = rdr["BPShortCode"].ToString();
                    stockTackings.Add(st);
                }
                return Ok(stockTackings);
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

        // GET api/<StockTackingMasterController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from stocktakingmaster where StocktakingMasterID = " + id;
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                StockTackingMaster st = new StockTackingMaster();
                if (rdr.Read())
                {
                    
                    if (rdr["StocktakingMasterID"] != DBNull.Value)
                        st.StocktakingMasterID = Convert.ToInt32(rdr["StocktakingMasterID"]);
                    else
                        st.StocktakingMasterID = null;
                    if (rdr["Date"] != DBNull.Value)
                        st.Date = Convert.ToDateTime(rdr["Date"]);
                    else
                        st.Date = null;
                    if (rdr["Location"] != DBNull.Value)
                        st.Location = Convert.ToInt32(rdr["Location"]);
                    else
                        st.Location = null;
                    st.Plant_Id = rdr["Plant_Id"].ToString();
                    st.Company_Id = rdr["Company_Id"].ToString();
                    st.BPShortCode = rdr["BPShortCode"].ToString();
                }
                return Ok(st);
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

        // POST api/<StockTackingMasterController>
        [HttpPost]
        public IActionResult Post([FromBody] StockTackingMaster st)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Insert into stocktakingmaster (Date, Location, Plant_Id, Company_Id, BPShortCode) values(@Date, @Location, @Plant_Id, @Company_Id, @BPShortCode)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (st.Date == null)
                    cmd.Parameters.Add(new MySqlParameter("@Date", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Date", MySqlDbType.Date)).Value = st.Date;
                if (st.Location == null)
                    cmd.Parameters.Add(new MySqlParameter("@Location", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Location", MySqlDbType.Int32)).Value = st.Location;
                cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = st.Plant_Id;
                cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = st.Company_Id;
                cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = st.BPShortCode;
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

        // PUT api/<StockTackingMasterController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] StockTackingMaster st)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Update stocktakingmaster set StocktakingMasterID = @StocktakingMasterID, Date = @Date, Location = @Location, Plant_Id = @Plant_Id, Company_Id = @Company_Id, BPShortCode = @BPShortCode where StocktakingMasterID = " + id;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (st.StocktakingMasterID == null)
                    cmd.Parameters.Add(new MySqlParameter("@StocktakingMasterID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@StocktakingMasterID", MySqlDbType.Int32)).Value = st.StocktakingMasterID;
                if (st.Date == null)
                    cmd.Parameters.Add(new MySqlParameter("@Date", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Date", MySqlDbType.Date)).Value = st.Date;
                if (st.Location == null)
                    cmd.Parameters.Add(new MySqlParameter("@Location", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Location", MySqlDbType.Int32)).Value = st.Location;
                cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = st.Plant_Id;
                cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = st.Company_Id;
                cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = st.BPShortCode;
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

        // DELETE api/<StockTackingMasterController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from stocktakingmaster where StocktakingMasterID = " + id;
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
