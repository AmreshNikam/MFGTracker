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
    public class GeneralSettingsController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public GeneralSettingsController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<GeneralSettingsController>
        [HttpGet("{COMPANYID}/{PLANTID}")]
        public IActionResult Get(string COMPANYID, string PLANTID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Select * from general_settings where Plant_ID = '" + PLANTID + "' and Company_ID = '" + COMPANYID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                GeneralSettings gs = new GeneralSettings();
                if (rdr.Read())
                {

                    if (rdr["UseTackTime"] != DBNull.Value)
                        gs.UseTackTime = Convert.ToBoolean(rdr["UseTackTime"]);
                    else
                        gs.UseTackTime = null;
                    if (rdr["RelaxsessionShiftStart"] != DBNull.Value)
                        gs.RelaxsessionShiftStart = Convert.ToBoolean(rdr["RelaxsessionShiftStart"]);
                    else
                        gs.RelaxsessionShiftStart = null;
                    if (rdr["TotalSkids"] != DBNull.Value)
                        gs.TotalSkids = Convert.ToInt32(rdr["TotalSkids"]);
                    else
                        gs.TotalSkids = null;
                    if (rdr["RoundNo"] != DBNull.Value)
                        gs.RoundNo = Convert.ToInt32(rdr["RoundNo"]);
                    else
                        gs.RoundNo = null;
                    //***********************
                    if (rdr["LeftToTight"] != DBNull.Value)
                        gs.LeftToTight = Convert.ToBoolean(rdr["LeftToTight"]);
                    else
                        gs.LeftToTight = null;
                    if (rdr["AcrossThenDown"] != DBNull.Value)
                        gs.AcrossThenDown = Convert.ToBoolean(rdr["AcrossThenDown"]);
                    else
                        gs.AcrossThenDown = null;
                    if (rdr["DownThenAcross"] != DBNull.Value)
                        gs.DownThenAcross = Convert.ToBoolean(rdr["DownThenAcross"]);
                    else
                        gs.DownThenAcross = null;
                    if (rdr["AcrossThenUP"] != DBNull.Value)
                        gs.AcrossThenUP = Convert.ToBoolean(rdr["AcrossThenUP"]);
                    else
                        gs.AcrossThenUP = null;
                    if (rdr["UPThenAcross"] != DBNull.Value)
                        gs.UPThenAcross = Convert.ToBoolean(rdr["UPThenAcross"]);
                    else
                        gs.UPThenAcross = null;
                    if(rdr["CustomerID"] != DBNull.Value)
                        gs.CustomerID = Convert.ToInt32(rdr["CustomerID"]);
                    else
                        gs.CustomerID = null;
                    gs.Company_ID = rdr["Company_ID"].ToString();
                    gs.Plant_ID = rdr["Plant_ID"].ToString();
                }
                connection.Close();
                return Ok(gs);
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

        // POST api/<GeneralSettingsController>
        [HttpPost]
        public IActionResult Post([FromBody] GeneralSettings gs)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Insert into general_settings (UseTackTime,RelaxsessionShiftStart,TotalSkids,RoundNo,LeftToTight, AcrossThenDown, DownThenAcross, AcrossThenUP, UPThenAcross, Plant_ID,Company_ID,CustomerID) values(@UseTackTime,@RelaxsessionShiftStart,@TotalSkids,@RoundNo,@LeftToTight, @AcrossThenDown, @DownThenAcross, @AcrossThenUP, @UPThenAcross,@Plant_ID,@Company_ID,@CustomerID)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (gs.CustomerID == null)
                    cmd.Parameters.Add(new MySqlParameter("@CustomerID", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@CustomerID", MySqlDbType.VarChar)).Value = gs.CustomerID;
                if (gs.Company_ID == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_ID", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_ID", MySqlDbType.VarChar)).Value = gs.Company_ID;
                if (gs.Plant_ID == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_ID", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_ID", MySqlDbType.VarChar)).Value = gs.Plant_ID;
                if (gs.UseTackTime == null)
                    cmd.Parameters.Add(new MySqlParameter("@UseTackTime", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@UseTackTime", MySqlDbType.Int16)).Value = gs.UseTackTime;
                if (gs.RelaxsessionShiftStart == null)
                    cmd.Parameters.Add(new MySqlParameter("@RelaxsessionShiftStart", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@RelaxsessionShiftStart", MySqlDbType.Int16)).Value = gs.RelaxsessionShiftStart;
                if (gs.TotalSkids == null)
                    cmd.Parameters.Add(new MySqlParameter("@TotalSkids", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@TotalSkids", MySqlDbType.Int16)).Value = gs.TotalSkids;
                if (gs.RoundNo == null)
                    cmd.Parameters.Add(new MySqlParameter("@RoundNo", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@RoundNo", MySqlDbType.Int16)).Value = gs.RoundNo;
                //********************************************************************************
                if (gs.LeftToTight == null)
                    cmd.Parameters.Add(new MySqlParameter("@LeftToTight", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@LeftToTight", MySqlDbType.Int16)).Value = gs.LeftToTight;
                if (gs.AcrossThenDown == null)
                    cmd.Parameters.Add(new MySqlParameter("@AcrossThenDown", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@AcrossThenDown", MySqlDbType.Int16)).Value = gs.AcrossThenDown;
                if (gs.DownThenAcross == null)
                    cmd.Parameters.Add(new MySqlParameter("@DownThenAcross", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@DownThenAcross", MySqlDbType.Int16)).Value = gs.DownThenAcross;
                if (gs.AcrossThenUP == null)
                    cmd.Parameters.Add(new MySqlParameter("@AcrossThenUP", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@AcrossThenUP", MySqlDbType.Int16)).Value = gs.AcrossThenUP;
                if (gs.UPThenAcross == null)
                    cmd.Parameters.Add(new MySqlParameter("@UPThenAcross", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@UPThenAcross", MySqlDbType.Int16)).Value = gs.UPThenAcross;
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

        // PUT api/<GeneralSettingsController>/5
        [HttpPut("{COMPANYID}/{PLANTID}")]
        public IActionResult Put(string COMPANYID, string PLANTID, [FromBody] GeneralSettings gs)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Update general_settings set UseTackTime = @UseTackTime, RelaxsessionShiftStart = @RelaxsessionShiftStart,TotalSkids = @TotalSkids,RoundNo = @RoundNo, LeftToTight = @LeftToTight, AcrossThenDown = @AcrossThenDown, DownThenAcross = @DownThenAcross, AcrossThenUP = @AcrossThenUP, UPThenAcross = @UPThenAcross, Company_ID = @Company_ID, Plant_ID = @Plant_ID, CustomerID = @CustomerID where Company_ID = '" + COMPANYID + "' and Plant_ID = '" + PLANTID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (gs.CustomerID == null)
                    cmd.Parameters.Add(new MySqlParameter("@CustomerID", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@CustomerID", MySqlDbType.VarChar)).Value = gs.CustomerID;
                if (gs.Company_ID == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_ID", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_ID", MySqlDbType.VarChar)).Value = gs.Company_ID;
                if (gs.Plant_ID == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_ID", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_ID", MySqlDbType.VarChar)).Value = gs.Plant_ID;
                if (gs.UseTackTime == null)
                    cmd.Parameters.Add(new MySqlParameter("@UseTackTime", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@UseTackTime", MySqlDbType.Int16)).Value = gs.UseTackTime;
                
                if (gs.RelaxsessionShiftStart == null)
                    cmd.Parameters.Add(new MySqlParameter("@RelaxsessionShiftStart", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@RelaxsessionShiftStart", MySqlDbType.Int16)).Value = gs.RelaxsessionShiftStart;
                if (gs.TotalSkids == null)
                    cmd.Parameters.Add(new MySqlParameter("@TotalSkids", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@TotalSkids", MySqlDbType.Int16)).Value = gs.TotalSkids;
                if (gs.RoundNo == null)
                    cmd.Parameters.Add(new MySqlParameter("@RoundNo", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@RoundNo", MySqlDbType.Int16)).Value = gs.RoundNo;
                if (gs.LeftToTight == null)
                    cmd.Parameters.Add(new MySqlParameter("@LeftToTight", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@LeftToTight", MySqlDbType.Int16)).Value = gs.LeftToTight;
                if (gs.AcrossThenDown == null)
                    cmd.Parameters.Add(new MySqlParameter("@AcrossThenDown", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@AcrossThenDown", MySqlDbType.Int16)).Value = gs.AcrossThenDown;
                if (gs.DownThenAcross == null)
                    cmd.Parameters.Add(new MySqlParameter("@DownThenAcross", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@DownThenAcross", MySqlDbType.Int16)).Value = gs.DownThenAcross;
                if (gs.AcrossThenUP == null)
                    cmd.Parameters.Add(new MySqlParameter("@AcrossThenUP", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@AcrossThenUP", MySqlDbType.Int16)).Value = gs.AcrossThenUP;
                if (gs.UPThenAcross == null)
                    cmd.Parameters.Add(new MySqlParameter("@UPThenAcross", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@UPThenAcross", MySqlDbType.Int16)).Value = gs.UPThenAcross;
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
        // DELETE api/<GeneralSettingsController>/5
        [HttpDelete("{COMPANYID}/{PLANTID }")]
        public IActionResult Delete(string COMPANYID, string PLANTID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Delete from general_settings where Company_ID = '" + COMPANYID + "' and Plant_ID = '" + PLANTID + "'";
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
