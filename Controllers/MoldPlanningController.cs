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
    public class MoldPlanningController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public MoldPlanningController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<MoldingPlanningController>
        [HttpGet("{CompanyID}/{PlantID}/{Status}")]
        public IActionResult Get(string CompanyID,string PlantID,string Status)
        {
            List<MoldPlanning> modesp = new List<MoldPlanning>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = string.Empty;
                if (Status.ToLower() == "all")
                    sql = "Select * from Mold_Planning where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "'";
                else
                    sql = "Select * from Mold_Planning where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "' and Status = '" + Status + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MoldPlanning mp = new MoldPlanning();
                    if (rdr["MoldPlanningID"] != DBNull.Value)
                        mp.MoldPlanningID = Convert.ToInt32(rdr["MoldPlanningID"]);
                    else
                        mp.MoldPlanningID = null;
                    if (rdr["PlanningDate"] != DBNull.Value)
                        mp.PlanningDate = Convert.ToDateTime(rdr["PlanningDate"]);
                    else
                        mp.PlanningDate = null;
                    if (rdr["WorkstationID"] != DBNull.Value)
                        mp.WorkstationID = Convert.ToInt32(rdr["WorkstationID"]);
                    else
                        mp.WorkstationID = null;
                    if (rdr["SubModel"] != DBNull.Value)
                        mp.SubModel = Convert.ToInt32(rdr["SubModel"]);
                    else
                        mp.SubModel = null;
                    if (rdr["Quantity"] != DBNull.Value)
                        mp.Quantity = Convert.ToInt32(rdr["Quantity"]);
                    else
                        mp.Quantity = null;
                    if (rdr["Status"] != DBNull.Value)
                        mp.Status = rdr["Status"].ToString();
                    else
                        mp.Status = null;
                    if (rdr["BPShortCode"] != DBNull.Value)
                        mp.BPShortCode = rdr["BPShortCode"].ToString();
                    else
                        mp.BPShortCode = null;
                    if (rdr["PlanEnteryDate"] != DBNull.Value)
                        mp.PlanEnteryDate = Convert.ToDateTime(rdr["PlanEnteryDate"]);
                    else
                        mp.PlanEnteryDate = null;
                    mp.Plant_Id = rdr["Plant_Id"].ToString();
                    mp.Company_Id = rdr["company_id"].ToString();
                    modesp.Add(mp);
                }
                return Ok(modesp);
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

        // GET api/<MoldingPlanningController>/5
        [HttpGet("{CompanyID}/{PlantID}/{Status}/{Not}")]
        public IActionResult Get(string CompanyID, string PlantID, string Status, string Not)
        {
            if (string.IsNullOrEmpty(Not) || Not != "Not") return StatusCode(400, "URL is incorrct");
            List<MoldPlanningDetails> modesp = new List<MoldPlanningDetails>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = string.Empty;
                if (Status.ToLower() == "all")
                    //sql = "Select * from Mold_Planning where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "'";
                    sql = "Select M.MoldPlanningID,M.PlanEnteryDate,M.PlanningDate,M.WorkstationID,W.Workstation_name,M.SubModel,SM.Subpart_Name,SM.Tag_time,SM.Part_id, MD.Part_name ,M.Quantity, " +
                          "M.OkCount as OK, M.RegCount as Rejected, (M.OkCount + M.RegCount) as Total, (M.Quantity - (M.OkCount + M.RegCount)) as Balance, " +
                          "M.Status,M.Plant_Id,M.Company_Id,M.BPShortCode,B.Name " +
                          "from Mold_Planning as M " +
                          "left join workstation as W on M.WorkstationID = W.Workstation_Id and M.Company_ID = W.Company_ID and M.Plant_Id = W.Plant_Id " +
                          "left join business_partner as B on M.BPShortCode = B.BP_Short_code and M.Company_ID = B.Company_ID and M.Plant_Id = B.Plant_Id " +
                          "left join sub_model as SM on M.SubModel = SM.Subpart_id,  " +
                          "model as MD " +
                          "where SM.Part_id = MD.Part_id and " +
                          "M.Company_ID = '" + CompanyID + "' and " +
                          "M.Plant_Id = '" + PlantID + "'";
                else
                    //sql = "Select * from Mold_Planning where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "' and Status != '" + Status + "'";
                    sql = "Select M.MoldPlanningID,M.PlanEnteryDate,M.PlanningDate,M.WorkstationID,W.Workstation_name,M.SubModel,SM.Subpart_Name,SM.Tag_time,SM.Part_id, MD.Part_name ,M.Quantity, " +
                          "M.OkCount as OK, M.RegCount as Rejected, (M.OkCount + M.RegCount) as Total, (M.Quantity - (M.OkCount + M.RegCount)) as Balance, " +
                          "M.Status,M.Plant_Id,M.Company_Id,M.BPShortCode,B.Name " +
                          "from Mold_Planning as M " +
                          "left join workstation as W on M.WorkstationID = W.Workstation_Id and M.Company_ID = W.Company_ID and M.Plant_Id = W.Plant_Id " +
                          "left join business_partner as B on M.BPShortCode = B.BP_Short_code and M.Company_ID = B.Company_ID and M.Plant_Id = B.Plant_Id " +
                          "left join sub_model as SM on M.SubModel = SM.Subpart_id,  " +
                          "model as MD " +
                          "where SM.Part_id = MD.Part_id  and " +
                          "M.Company_ID = '" + CompanyID + "' and " +
                          "M.Plant_Id = '" + PlantID + "' and " +
                          "Status != '" + Status + "'";

                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    MoldPlanningDetails mp = new MoldPlanningDetails();
                    if (rdr["MoldPlanningID"] != DBNull.Value)
                        mp.MoldPlanningID = Convert.ToInt32(rdr["MoldPlanningID"]);
                    else
                        mp.MoldPlanningID = null;
                    if (rdr["PlanningDate"] != DBNull.Value)
                        mp.PlanningDate = Convert.ToDateTime(rdr["PlanningDate"]);
                    else
                        mp.PlanningDate = null;
                    if (rdr["WorkstationID"] != DBNull.Value)
                        mp.WorkstationID = Convert.ToInt32(rdr["WorkstationID"]);
                    else
                        mp.WorkstationID = null;
                    if (rdr["Workstation_name"] != DBNull.Value)
                        mp.Workstation_name =rdr["Workstation_name"].ToString();
                    else
                        mp.Workstation_name = null;
                    if (rdr["SubModel"] != DBNull.Value)
                        mp.SubModel = Convert.ToInt32(rdr["SubModel"]);
                    else
                        mp.SubModel = null;
                    if (rdr["Tag_time"] != DBNull.Value)
                        mp.tagtime = Convert.ToInt32(rdr["Tag_time"]);
                    else
                        mp.tagtime = null;
                    if (rdr["Subpart_Name"] != DBNull.Value)
                        mp.Subpart_Name = rdr["Subpart_Name"].ToString();
                    else
                        mp.Subpart_Name = null;
                    if (rdr["Part_id"] != DBNull.Value)
                        mp.Part_id = Convert.ToInt32(rdr["Part_id"]);
                    else
                        mp.Part_id = null;
                    if (rdr["Part_name"] != DBNull.Value)

                        mp.Part_name = rdr["Part_name"].ToString();
                    else
                        mp.Part_name = null;
                    if (rdr["Quantity"] != DBNull.Value)
                        mp.Quantity = Convert.ToInt32(rdr["Quantity"]);
                    else
                        mp.Quantity = null;
                    //*****************************************
                    if (rdr["OK"] != DBNull.Value)
                        mp.OK = Convert.ToInt32(rdr["OK"]);
                    else
                        mp.OK = null;
                    if (rdr["Rejected"] != DBNull.Value)
                        mp.Rejected = Convert.ToInt32(rdr["Rejected"]);
                    else
                        mp.Rejected = null;
                    if (rdr["Total"] != DBNull.Value)
                        mp.Total = Convert.ToInt32(rdr["Total"]);
                    else
                        mp.Total = null;
                    if (rdr["Balance"] != DBNull.Value)
                        mp.Balance = Convert.ToInt32(rdr["Balance"]);
                    else
                        mp.Balance = null;
                    if (rdr["Status"] != DBNull.Value) 
                        mp.Status = rdr["Status"].ToString();
                    else
                        mp.Status = null;
                    if (rdr["BPShortCode"] != DBNull.Value) 
                        mp.BPShortCode = rdr["BPShortCode"].ToString();
                    else
                        mp.BPShortCode = null;
                    if (rdr["Name"] != DBNull.Value)
                        mp.BP_Name = rdr["Name"].ToString();
                    else
                        mp.BP_Name = null;
                    if (rdr["PlanEnteryDate"] != DBNull.Value)
                        mp.PlanEnteryDate = Convert.ToDateTime(rdr["PlanEnteryDate"]);
                    else
                        mp.PlanEnteryDate = null;
                    mp.Plant_Id = rdr["Plant_Id"].ToString();
                    mp.Company_Id = rdr["company_id"].ToString();
                    modesp.Add(mp);
                }
                return Ok(modesp);
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
        // POST api/<MoldingPlanningController>
        [HttpPost]
        public IActionResult Post([FromBody] MoldPlanning mp)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Insert into Mold_Planning (PlanningDate, WorkstationID, SubModel, Quantity, Status, Plant_Id, Company_Id,BPShortCode,PlanEnteryDate) values(@PlanningDate, @WorkstationID, @SubModel, @Quantity, @Status, @Plant_Id, @Company_Id,@BPShortCode,@PlanEnteryDate)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (mp.PlanningDate == null)
                    cmd.Parameters.Add(new MySqlParameter("@PlanningDate", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PlanningDate", MySqlDbType.Date)).Value = mp.PlanningDate;
                if (mp.WorkstationID == null)
                    cmd.Parameters.Add(new MySqlParameter("@WorkstationID", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@WorkstationID", MySqlDbType.VarChar)).Value = mp.WorkstationID;
                if (mp.SubModel == null)
                    cmd.Parameters.Add(new MySqlParameter("@SubModel", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@SubModel", MySqlDbType.VarChar)).Value = mp.SubModel;
                if (mp.Quantity == null)
                    cmd.Parameters.Add(new MySqlParameter("@Quantity", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Quantity", MySqlDbType.Int32)).Value = mp.Quantity;
                if (mp.Status == null)
                    cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.VarChar)).Value = mp.Status;
                if (mp.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = mp.Plant_Id;
                if (mp.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = mp.Company_Id;
                if (mp.BPShortCode == null)
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = mp.BPShortCode;
                if (mp.PlanEnteryDate == null)
                    cmd.Parameters.Add(new MySqlParameter("@PlanEnteryDate", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PlanEnteryDate", MySqlDbType.Date)).Value = mp.PlanEnteryDate;
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

        // PUT api/<MoldingPlanningController>/5
        [HttpPut("{ID}")]
        public IActionResult Put(int ID, [FromBody] MoldPlanning mp)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Update Mold_Planning set MoldPlanningID = @MoldPlanningID, PlanningDate = @PlanningDate, WorkstationID = @WorkstationID, SubModel = @SubModel, Quantity = @Quantity, Status = @Status, Plant_Id = @Plant_Id, Company_Id = @Company_Id, BPShortCode = @BPShortCode, PlanEnteryDate=@PlanEnteryDate  where MoldPlanningID = " + ID;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (mp.MoldPlanningID == null)
                    cmd.Parameters.Add(new MySqlParameter("@MoldPlanningID", MySqlDbType.Int32  )).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@MoldPlanningID", MySqlDbType.Int32)).Value = mp.MoldPlanningID;
                if (mp.PlanningDate == null)
                    cmd.Parameters.Add(new MySqlParameter("@PlanningDate", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PlanningDate", MySqlDbType.Date)).Value = mp.PlanningDate;
                if (mp.WorkstationID == null)
                    cmd.Parameters.Add(new MySqlParameter("@WorkstationID", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@WorkstationID", MySqlDbType.VarChar)).Value = mp.WorkstationID;
                if (mp.SubModel == null)
                    cmd.Parameters.Add(new MySqlParameter("@SubModel", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@SubModel", MySqlDbType.VarChar)).Value = mp.SubModel;
                if (mp.Quantity == null)
                    cmd.Parameters.Add(new MySqlParameter("@Quantity", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Quantity", MySqlDbType.Int32)).Value = mp.Quantity;
                if (mp.Status == null)
                    cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.VarChar)).Value = mp.Status;
                if (mp.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = mp.Plant_Id;
                if (mp.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = mp.Company_Id;
                if (mp.BPShortCode == null)
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = mp.BPShortCode;
                if (mp.PlanEnteryDate == null)
                    cmd.Parameters.Add(new MySqlParameter("@PlanEnteryDate", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PlanEnteryDate", MySqlDbType.Date)).Value = mp.PlanEnteryDate;
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

        // DELETE api/<MoldingPlanningController>/5
        [HttpDelete("{ID}")]
        public IActionResult Delete(int ID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from Mold_Planning where MoldPlanningID = " + ID;
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
