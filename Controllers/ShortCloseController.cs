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
    public class ShortCloseController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public ShortCloseController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        // GET: api/<ShortCloseController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<ShortCloseController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<ShortCloseController>
        [HttpPost("{PLANEXEID}/{SHIFTID}/{STARTSTOP}")]
        public IActionResult Post(int PLANEXEID, int SHIFTID, bool STARTSTOP, [FromBody] ShortClose sc)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            string sql = string.Empty;
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null;
            try
            {
                connection.Open();
                myTrans = connection.BeginTransaction();
                MySqlCommand cmd = null;
                sql = "select * from general_settings where Plant_ID = '" + sc.Plant_ID + "' and Company_ID = '" + sc.Company_ID + "'";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                bool TACK = Convert.ToBoolean(rdr["UseTackTime"]);
                bool Relax = Convert.ToBoolean(rdr["RelaxsessionShiftStart"]);
                rdr.Close();
                if (TACK)
                {
                    TimeSpan actualshiftstarted;
                    sql = "select startTime from mold_plan_execution where MoldPlanExecutionID = " + PLANEXEID;
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    rdr.Read();
                    actualshiftstarted = TimeSpan.Parse(rdr["startTime"].ToString());
                    rdr.Close();
                    sql = "select * from Shift where Shift_Id = " + SHIFTID;
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    TimeSpan st, et,sst,set,scs,sce;
                    scs = sce = TimeSpan.Parse("01:00:00");
                    rdr.Read();
                    if (Relax)
                        st = TimeSpan.Parse(rdr["Start_time"].ToString());
                    else
                        st = actualshiftstarted;
                    et = TimeSpan.Parse(rdr["End_time"].ToString());
                    sst = TimeSpan.Parse("00:00:00");
                    set = et.Subtract(st);
                    rdr.Close();
                    
                    if (st > et)
                    {
                        TimeSpan t23 = TimeSpan.Parse("23:00:00");
                        set = t23.Subtract(st).Add(et).Add(TimeSpan.Parse("01:00:00"));
                    }
                    if (st > TimeSpan.Parse(sc.StartTime))
                    {
                        TimeSpan t23 = TimeSpan.Parse("23:00:00");
                        scs = sce = t23.Subtract(st).Add(TimeSpan.Parse(sc.StartTime)).Add(TimeSpan.Parse("01:00:00"));
                    }
                    else
                    {
                        et = TimeSpan.Parse(sc.EndTime);
                        scs = sce = et.Subtract(st);
                    }

                    if (STARTSTOP)
                    {
                        sql = "Insert into ShortClose (StartTime, EndTime, ExcecutionPlan,BP_Short_code,Plant_Id,Company_Id) values(@StartTime, @EndTime, @ExcecutionPlan,@BP_Short_code,@Plant_Id,@Company_Id)";
                        cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text,Transaction = myTrans };
                        cmd.Parameters.Add(new MySqlParameter("@StartTime", MySqlDbType.Time)).Value = scs;
                        cmd.Parameters.Add(new MySqlParameter("@EndTime", MySqlDbType.Time)).Value = set;
                        cmd.Parameters.Add(new MySqlParameter("@ExcecutionPlan", MySqlDbType.Int32)).Value = PLANEXEID;
                        cmd.Parameters.Add(new MySqlParameter("@BP_Short_code", MySqlDbType.Int32)).Value = sc.BP_Short_code;
                        cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.Int32)).Value = sc.Plant_ID;
                        cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.Int32)).Value = sc.Company_ID;
                        cmd.ExecuteScalar();
                    }
                    else
                    {
                        sql = "SELECT * FROM ShortClose where EndTime = (select max(EndTime) from ShortClose where ExcecutionPlan = " + PLANEXEID + ") and ExcecutionPlan = " + PLANEXEID;
                        cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                        rdr = cmd.ExecuteReader();
                        rdr.Read();
                        int ID = Convert.ToInt32(rdr["ShortCloseID"]);
                        rdr.Close();
                        sql = "Update ShortClose set EndTime = @EndTime, BP_Short_code = @BP_Short_code, Plant_Id = @Plant_Id, Company_Id = @Company_Id where ShortCloseID = " + ID;
                        cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text, Transaction = myTrans };
                        cmd.Parameters.Add(new MySqlParameter("@EndTime", MySqlDbType.Time)).Value = sce;
                        cmd.Parameters.Add(new MySqlParameter("@BP_Short_code", MySqlDbType.VarChar)).Value = sc.BP_Short_code;
                        cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = sc.Plant_ID;
                        cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = sc.Company_ID;
                        cmd.ExecuteScalar();
                    }
                    cmd = new MySqlCommand("UpdateQuantity", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Transaction = myTrans
                    };
                    cmd.Parameters.Add("?planID", MySqlDbType.Int32).Value = PLANEXEID;
                    cmd.Parameters.Add("?TackTime", MySqlDbType.Int32).Value = sc.TackTime;
                    cmd.ExecuteNonQuery();
                }
                //else
                //{
                if (STARTSTOP)
                    {
                        string temp = " Resume at " + DateTime.Now.ToString();
                        sql = "Update mold_plan_execution set Notes = concat(Notes,'" + temp + "'), Status = "+ !STARTSTOP + " where MoldPlanExecutionID = " + PLANEXEID;
                        cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text,Transaction = myTrans };
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string temp = "Stop at " + DateTime.Now.ToString() + " Because " + sc.Note;
                        sql = "Update mold_plan_execution set Notes = concat(Notes,'" + temp + "'), Status = " + !STARTSTOP + "  where MoldPlanExecutionID = " + PLANEXEID;
                        cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text, Transaction = myTrans };
                        cmd.ExecuteNonQuery();
                    }
               // }
                myTrans.Commit();
                return Ok("Done");
            }
            catch (MySqlException ex)
            {
                try { myTrans.Rollback(); } catch { }
                return StatusCode(600, ex.Message);
            }
            catch (Exception ex)
            {
                try { myTrans.Rollback(); } catch { }
                return StatusCode(500, ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        // PUT api/<ShortCloseController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        // DELETE api/<ShortCloseController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
