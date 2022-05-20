using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MFG_Tracker.DatabaseTable;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.Transactions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MFG_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoldPlanExecutionController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public MoldPlanExecutionController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<MoldPlanExecutionController>
        [HttpGet("{COMPANYID}/{PLANTID}")]
        public IActionResult Get(string COMPANYID,string PLANTID, [FromQuery]DateTime date, int ShiftID = 0, bool Details=true)
        {
            if (!Details)
            {
                List<MoldPlanExecution> mdes = new List<MoldPlanExecution>();
                string connString = this.Configuration.GetConnectionString("MFG_Tracker");
                MySqlConnection connection = new MySqlConnection(connString);
                try
                {
                    connection.Open();
                    string sql = "Select * from mold_plan_execution where DateOfExecution = '" + date.ToString("yyyy/MM/dd") + "' and Plant_ID = '" + PLANTID + "' and Company_ID = '" + COMPANYID + "'";
                    MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        MoldPlanExecution mde = new MoldPlanExecution();
                        if (rdr["MoldPlanExecutionID"] != DBNull.Value)
                            mde.MoldPlanExecutionID = Convert.ToInt32(rdr["MoldPlanExecutionID"]);
                        else
                            mde.MoldPlanExecutionID = null;
                        if (rdr["ModelPlanID"] != DBNull.Value)
                            mde.ModelPlanID = Convert.ToInt32(rdr["ModelPlanID"]);
                        else
                            mde.ModelPlanID = null;
                        if (rdr["WorkstationID"] != DBNull.Value)
                            mde.WorkstationID = Convert.ToInt32(rdr["WorkstationID"]);
                        else
                            mde.WorkstationID = null;

                        if (rdr["ShiftID"] != DBNull.Value)
                            mde.ShiftID = Convert.ToInt32(rdr["ShiftID"]);
                        else
                            mde.ShiftID = null;
                        if (rdr["DateOfExecution"] != DBNull.Value)
                            mde.DateOfExecution = Convert.ToDateTime(rdr["DateOfExecution"]);
                        else
                            mde.DateOfExecution = null;
                        if (rdr["EstimatedInitialQuantity"] != DBNull.Value)
                            mde.EstimatedInitialQuantity = Convert.ToInt32(rdr["EstimatedInitialQuantity"]);
                        else
                            mde.EstimatedInitialQuantity = null;
                        if (rdr["EstimatedQuantity"] != DBNull.Value)
                            mde.EstimatedQuantity = Convert.ToInt32(rdr["EstimatedQuantity"]);
                        else
                            mde.EstimatedQuantity = null;
                        if (rdr["Plant_Id"] != DBNull.Value)
                            mde.Plant_Id = rdr["Plant_Id"].ToString();
                        else
                            mde.Plant_Id = null;
                        if (rdr["Company_Id"] != DBNull.Value)
                            mde.Company_Id = rdr["Company_Id"].ToString();
                        else
                            mde.Company_Id = null;
                        if (rdr["BPShortCode"] != DBNull.Value)
                            mde.BPShortCode = rdr["BPShortCode"].ToString();
                        else
                            mde.BPShortCode = null;
                        if (rdr["Notes"] != DBNull.Value)
                            mde.BPShortCode = rdr["Notes"].ToString();
                        else
                            mde.Notes = null;
                        if (rdr["Status"] != DBNull.Value)
                            mde.Status = Convert.ToBoolean(rdr["Status"]);
                        else
                            mde.Status = null;
                        
                        mdes.Add(mde);
                    }
                    connection.Close();
                    return Ok(mdes);
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
            else
            {
                List<MoldPlanExecutionDetails> mdes = new List<MoldPlanExecutionDetails>();
                string connString = this.Configuration.GetConnectionString("MFG_Tracker");
                MySqlConnection connection = new MySqlConnection(connString);
                try
                {
                    connection.Open();
                    string sql;
                    if(ShiftID == 0)
                           sql = "select ME.MoldPlanExecutionID, ME.ModelPlanID, MD.Part_name, SM.Subpart_Name,SM.Tag_time, ME.WorkstationID, WS.Workstation_name, ME.ShiftID, SH.Shift_name, ME.DateOfExecution, ME.EstimatedInitialQuantity, ME.EstimatedQuantity, ME.Notes, ME.Status, ME.Plant_Id, ME.Company_Id,CM.Name as Company_Name, ME.BPShortCode, BP.Name " +
                                 "from mold_plan_execution as ME " +
                                 "left join workstation as WS on ME.WorkstationID = WS.Workstation_Id " +
                                 "left join shift as SH on ME.ShiftID = SH.Shift_id " +
                                 "left join mold_planning as MP on ME.ModelPlanID = MP.MoldPlanningID " +
                                 "left join sub_model SM on MP.SubModel = SM.Subpart_id " +
                                 "left join model as MD on SM.Part_id = MD.Part_id " +
                                 "left join business_partner as BP on ME.BPShortCode = BP.BP_Short_code and ME.Plant_ID = BP.Plant_Id and ME.Company_ID = BP.company_id " +
                                 "left join business_partner as CM on MD.BP_Short_code = CM.BP_Short_code and MD.Plant_ID = CM.Plant_Id and MD.Company_ID = CM.company_id " +
                                 "where ME.DateOfExecution = '" + date.ToString("yyyy/MM/dd") + "' and ME.Plant_ID = '" + PLANTID + "' and ME.Company_ID = '" + COMPANYID + "'";
                    else
                        sql = "select ME.MoldPlanExecutionID, ME.ModelPlanID, MD.Part_name, SM.Subpart_Name,SM.Tag_time, ME.WorkstationID, WS.Workstation_name, ME.ShiftID, SH.Shift_name, ME.DateOfExecution, ME.EstimatedInitialQuantity, ME.EstimatedQuantity, ME.Notes, ME.Status, ME.Plant_Id, ME.Company_Id,CM.Name as Company_Name, ME.BPShortCode, BP.Name " +
                                 "from mold_plan_execution as ME " +
                                 "left join workstation as WS on ME.WorkstationID = WS.Workstation_Id " +
                                 "left join shift as SH on ME.ShiftID = SH.Shift_id " +
                                 "left join mold_planning as MP on ME.ModelPlanID = MP.MoldPlanningID " +
                                 "left join sub_model SM on MP.SubModel = SM.Subpart_id " +
                                 "left join model as MD on SM.Part_id = MD.Part_id " +
                                 "left join business_partner as BP on ME.BPShortCode = BP.BP_Short_code and ME.Plant_ID = BP.Plant_Id and ME.Company_ID = BP.company_id " +
                                 "left join business_partner as CM on MD.BP_Short_code = CM.BP_Short_code and MD.Plant_ID = CM.Plant_Id and MD.Company_ID = CM.company_id " +
                                 "where ME.DateOfExecution = '" + date.ToString("yyyy/MM/dd") + "' and ME.Plant_ID = '" + PLANTID + "' and ME.Company_ID = '" + COMPANYID + "' and ME.ShiftID =" + ShiftID;
                    MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        MoldPlanExecutionDetails mde = new MoldPlanExecutionDetails();
                        if (rdr["MoldPlanExecutionID"] != DBNull.Value)
                            mde.MoldPlanExecutionID = Convert.ToInt32(rdr["MoldPlanExecutionID"]);
                        else
                            mde.MoldPlanExecutionID = null;
                        if (rdr["ModelPlanID"] != DBNull.Value)
                            mde.ModelPlanID = Convert.ToInt32(rdr["ModelPlanID"]);
                        else
                            mde.ModelPlanID = null;
                        if (rdr["Part_name"] != DBNull.Value)
                            mde.Part_name = rdr["Part_name"].ToString();
                        else
                            mde.Part_name = null;
                        if (rdr["Subpart_Name"] != DBNull.Value)
                            mde.Subpart_Name = rdr["Subpart_Name"].ToString();
                        else
                            mde.Subpart_Name = null;
                        if (rdr["Tag_time"] != DBNull.Value)
                            mde.Tag_time = Convert.ToInt32(rdr["Tag_time"]);
                        else
                            mde.Tag_time = null;
                        if (rdr["WorkstationID"] != DBNull.Value)
                            mde.WorkstationID = Convert.ToInt32(rdr["WorkstationID"]);
                        else
                            mde.WorkstationID = null;
                        if (rdr["Workstation_name"] != DBNull.Value)
                            mde.Workstation_name = rdr["Workstation_name"].ToString();
                        else
                            mde.Workstation_name = null;
                        if (rdr["ShiftID"] != DBNull.Value)
                            mde.ShiftID = Convert.ToInt32(rdr["ShiftID"]);
                        else
                            mde.ShiftID = null;
                        if (rdr["Shift_name"] != DBNull.Value)
                            mde.Shift_name = rdr["Shift_name"].ToString();
                        else
                            mde.Shift_name = null;
                        if (rdr["DateOfExecution"] != DBNull.Value)
                            mde.DateOfExecution = Convert.ToDateTime(rdr["DateOfExecution"]);
                        else
                            mde.DateOfExecution = null;
                        if (rdr["EstimatedInitialQuantity"] != DBNull.Value)
                            mde.EstimatedInitialQuantity = Convert.ToInt32(rdr["EstimatedInitialQuantity"]);
                        else
                            mde.EstimatedInitialQuantity = null;
                        if (rdr["EstimatedQuantity"] != DBNull.Value)
                            mde.EstimatedQuantity = Convert.ToInt32(rdr["EstimatedQuantity"]);
                        else
                            mde.EstimatedQuantity = null;
                        if (rdr["Plant_Id"] != DBNull.Value)
                            mde.Plant_Id = rdr["Plant_Id"].ToString();
                        else
                            mde.Plant_Id = null;
                        if (rdr["Company_Id"] != DBNull.Value)
                            mde.Company_Id = rdr["Company_Id"].ToString();
                        else
                            mde.Company_Id = null;
                        if (rdr["BPShortCode"] != DBNull.Value)
                            mde.BPShortCode = rdr["BPShortCode"].ToString();
                        else
                            mde.BPShortCode = null;
                        if (rdr["Notes"] != DBNull.Value)
                            mde.BPShortCode = rdr["Notes"].ToString();
                        else
                            mde.Notes = null;
                        if (rdr["Status"] != DBNull.Value)
                            mde.Status = Convert.ToBoolean(rdr["Status"]);
                        else
                            mde.Status = null;
                        if (rdr["Name"] != DBNull.Value)
                            mde.BPName = rdr["Name"].ToString();
                        else
                            mde.BPName = null;
                        if (rdr["company_name"] != DBNull.Value)
                            mde.company_name = rdr["company_name"].ToString();
                        else
                            mde.company_name = null;
                        mdes.Add(mde);
                    }
                    connection.Close();
                    return Ok(mdes);
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
        

        // POST api/<MoldPlanExecutionController>
        [HttpPost]
        public IActionResult Post([FromBody] MoldPlanExecution mpe)
        {
            
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            string sql;
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null;
            try
            {
                //using TransactionScope scope = new TransactionScope();
                connection.Open();
                myTrans = connection.BeginTransaction();
                sql = "select * from general_settings where Plant_ID = '" + mpe.Plant_Id + "' and Company_ID = '" + mpe.Company_Id + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                bool UPDATE = false;
                bool Relax = true;
                if (rdr.Read())
                {
                    UPDATE = Convert.ToBoolean(rdr["UseTackTime"]);
                    Relax = Convert.ToBoolean(rdr["RelaxsessionShiftStart"]);
                }
                    rdr.Close();

                sql = "Insert into mold_plan_execution (ModelPlanID, WorkstationID, ShiftID, DateOfExecution, EstimatedInitialQuantity, EstimatedQuantity, Plant_ID, Company_Id, BPShortCode, Notes, Status,startTime) values(@ModelPlanID, @WorkstationID, @ShiftID, @DateOfExecution, @EstimatedInitialQuantity, @EstimatedQuantity, @Plant_ID, @Company_Id, @BPShortCode, @Notes, @Status,@startTime)";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text, Transaction = myTrans };
                if (mpe.ModelPlanID == null)
                    cmd.Parameters.Add(new MySqlParameter("@ModelPlanID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ModelPlanID", MySqlDbType.Int32)).Value = mpe.ModelPlanID;
                if (mpe.WorkstationID == null)
                    cmd.Parameters.Add(new MySqlParameter("@WorkstationID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@WorkstationID", MySqlDbType.Int32)).Value = mpe.WorkstationID;
                if (mpe.ShiftID == null)
                    cmd.Parameters.Add(new MySqlParameter("@ShiftID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ShiftID", MySqlDbType.Int32)).Value = mpe.ShiftID;
                if (mpe.DateOfExecution == null)
                    cmd.Parameters.Add(new MySqlParameter("@DateOfExecution", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@DateOfExecution", MySqlDbType.Date)).Value = mpe.DateOfExecution;
                if (mpe.EstimatedInitialQuantity == null)
                    cmd.Parameters.Add(new MySqlParameter("@EstimatedInitialQuantity", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@EstimatedInitialQuantity", MySqlDbType.Int32)).Value = mpe.EstimatedInitialQuantity;
                if (mpe.EstimatedQuantity == null)
                    cmd.Parameters.Add(new MySqlParameter("@EstimatedQuantity", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@EstimatedQuantity", MySqlDbType.Int32)).Value = mpe.EstimatedQuantity;
                if (mpe.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_ID", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_ID", MySqlDbType.VarChar)).Value = mpe.Plant_Id;
                if (mpe.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = mpe.Company_Id;
                if (mpe.BPShortCode == null)
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = mpe.BPShortCode;
                if (mpe.Notes == null)
                    cmd.Parameters.Add(new MySqlParameter("@Notes", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Notes", MySqlDbType.VarChar)).Value = mpe.Notes;
                if (mpe.Status == null)
                    cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.Int16)).Value = mpe.Status;
                if (mpe.startTime == null)
                    cmd.Parameters.Add(new MySqlParameter("@startTime", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@startTime", MySqlDbType.VarChar)).Value = mpe.startTime;
                cmd.ExecuteScalar();
                ShortClose sc = new ShortClose()
                { ExcecutionPlan = Convert.ToInt32(cmd.LastInsertedId) };
                TimeSpan s, e;
                if (UPDATE)
                {
                    sql = "Select * from Shift where Shift_id = " + mpe.ShiftID;
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        if (Relax)
                            sc.StartTime = rdr["Start_time"].ToString(); // 0
                        else
                            sc.StartTime = mpe.startTime;
                        sc.EndTime = rdr["End_time"].ToString(); // difference between End_time & Start_time
                        s =  TimeSpan.Parse(sc.StartTime);
                        e =  TimeSpan.Parse(sc.EndTime);
                        if (s > e)
                        {
                            TimeSpan t23 = TimeSpan.Parse("23:00:00");
                            e = t23.Subtract(s).Add(e).Add(TimeSpan.Parse("01:00:00"));
                        }
                        else
                            e = e.Subtract(s);
                        s = TimeSpan.Parse("00:00:00");
                    }
                    else
                        return StatusCode(700, "Shift ID is not proper");
                    rdr.Close();
                    sql = "Insert into ShortClose (StartTime, EndTime, ExcecutionPlan) values(@StartTime, @EndTime, @ExcecutionPlan)";
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text, Transaction = myTrans };
                    cmd.Parameters.Add(new MySqlParameter("@StartTime", MySqlDbType.Time)).Value = s;
                    cmd.Parameters.Add(new MySqlParameter("@EndTime", MySqlDbType.Time)).Value = e;
                    cmd.Parameters.Add(new MySqlParameter("@ExcecutionPlan", MySqlDbType.Int32)).Value = sc.ExcecutionPlan;
                    cmd.ExecuteScalar();
                }
                //scope.Complete();
                myTrans.Commit();
                connection.Close();
                return Ok("Record Inserted Successfuly");
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

        // PUT api/<MoldPlanExecutionController>/5
        [HttpPut("{ID}")]
        public IActionResult Put(int ID, [FromBody] MoldPlanExecution mpe)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Update mold_plan_execution set MoldPlanExecutionID = @MoldPlanExecutionID, ModelPlanID = @ModelPlanID, WorkstationID = @WorkstationID, ShiftID = @ShiftID, DateOfExecution = @DateOfExecution, EstimatedInitialQuantity = @EstimatedInitialQuantity, EstimatedQuantity = @EstimatedQuantity, Plant_Id = @Plant_Id, Company_Id = @Company_Id, BPShortCode = @BPShortCode, Notes = @Notes where MoldPlanExecutionID = " + ID;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (mpe.MoldPlanExecutionID == null)
                    cmd.Parameters.Add(new MySqlParameter("@MoldPlanExecutionID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@MoldPlanExecutionID", MySqlDbType.Int32)).Value = mpe.MoldPlanExecutionID;
                if (mpe.ModelPlanID == null)
                    cmd.Parameters.Add(new MySqlParameter("@ModelPlanID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ModelPlanID", MySqlDbType.Int32)).Value = mpe.ModelPlanID;
                if (mpe.WorkstationID == null)
                    cmd.Parameters.Add(new MySqlParameter("@WorkstationID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@WorkstationID", MySqlDbType.Int32)).Value = mpe.WorkstationID;
                if (mpe.ShiftID == null)
                    cmd.Parameters.Add(new MySqlParameter("@ShiftID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ShiftID", MySqlDbType.Int32)).Value = mpe.ShiftID;
                if (mpe.DateOfExecution == null)
                    cmd.Parameters.Add(new MySqlParameter("@DateOfExecution", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@DateOfExecution", MySqlDbType.Date)).Value = mpe.DateOfExecution;
                if (mpe.EstimatedInitialQuantity == null)
                    cmd.Parameters.Add(new MySqlParameter("@EstimatedInitialQuantity", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@EstimatedInitialQuantity", MySqlDbType.Int32)).Value = mpe.EstimatedInitialQuantity;
                if (mpe.EstimatedQuantity == null)
                    cmd.Parameters.Add(new MySqlParameter("@EstimatedQuantity", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@EstimatedQuantity", MySqlDbType.Int32)).Value = mpe.EstimatedQuantity;
                if (mpe.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = mpe.Plant_Id;
                if (mpe.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = mpe.Company_Id;
                if (mpe.BPShortCode == null)
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = mpe.BPShortCode;
                if (mpe.Notes == null)
                    cmd.Parameters.Add(new MySqlParameter("@Notes", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Notes", MySqlDbType.VarChar)).Value = mpe.Notes;
                if (mpe.Status == null)
                    cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.Int16)).Value = mpe.Status;
                cmd.ExecuteNonQuery();
                connection.Close();
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
        

        // DELETE api/<MoldPlanExecutionController>/5
        [HttpDelete("{ID}")]
        public IActionResult Delete(int ID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from mold_plan_execution where MoldPlanExecutionID = " + ID;
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
