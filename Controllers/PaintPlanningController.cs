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
    public class PaintPlanningController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public PaintPlanningController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<PaintPlanningController>
        [HttpGet("{CompanyID}/{PlantID}/{Status}")]
        public IActionResult Get(string CompanyID, string PlantID, string Status = "ALL")
        {
            List<PaintPlanning> paintPlannings  = new List<PaintPlanning>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = string.Empty;
                if (Status.ToLower() == "all")
                    sql = "Select * from paintplanning where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "'";
                else
                    sql = "Select * from paintplanning where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "' and Status = '" + Status + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    PaintPlanning paintPlanning = new PaintPlanning();
                    if (rdr["PaintPlanID"] != DBNull.Value)
                        paintPlanning.PaintPlanID = Convert.ToInt32(rdr["PaintPlanID"]);
                    else
                        paintPlanning.PaintPlanID = null;
                    if (rdr["PlanningDate"] != DBNull.Value)
                        paintPlanning.PlanningDate = Convert.ToDateTime(rdr["PlanningDate"]);
                    else
                        paintPlanning.PlanningDate = null;
                    if (rdr["SubModel"] != DBNull.Value)
                        paintPlanning.SubModel = Convert.ToInt32(rdr["SubModel"]);
                    else
                        paintPlanning.SubModel = null;
                    if (rdr["Color"] != DBNull.Value)
                        paintPlanning.Color = Convert.ToInt32(rdr["Color"]);
                    else
                        paintPlanning.Color = null;
                    if (rdr["Quantity"] != DBNull.Value)
                        paintPlanning.Quantity = Convert.ToInt32(rdr["Quantity"]);
                    else
                        paintPlanning.Quantity = null;
                    if (rdr["Skid"] != DBNull.Value)
                        paintPlanning.Skid = Convert.ToInt32(rdr["Skid"]);
                    else
                        paintPlanning.Skid = null;
                    if (rdr["Round"] != DBNull.Value)
                        paintPlanning.Round = Convert.ToInt32(rdr["Round"]);
                    else
                        paintPlanning.Round = null;
                    if (rdr["Status"] != DBNull.Value)
                        paintPlanning.Status = rdr["Status"].ToString();
                    else
                        paintPlanning.Status = null;
                    if (rdr["BPShortCode"] != DBNull.Value)
                        paintPlanning.BPShortCode = rdr["BPShortCode"].ToString();
                    else
                        paintPlanning.BPShortCode = null; 
                    if (rdr["PlanEnteryDate"] != DBNull.Value)
                        paintPlanning.PlanEnteryDate = Convert.ToDateTime(rdr["PlanEnteryDate"]);
                    else
                        paintPlanning.PlanEnteryDate = null; 
                    paintPlanning.Plant_Id = rdr["Plant_Id"].ToString();
                    paintPlanning.Company_Id = rdr["company_id"].ToString();
                    paintPlannings.Add(paintPlanning);
                }
                return Ok(paintPlannings);
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

        // GET api/<PaintPlanningController>/5
        [HttpGet("{CompanyID}/{PlantID}/{Status}/{Not}")]
        public IActionResult Get(string CompanyID, string PlantID, string Status, string Not)
        {
            if (string.IsNullOrEmpty(Not) || Not != "Not") return StatusCode(400, "URL is incorrct");
            List<PaintPlanningDetails> planningDetails = new List<PaintPlanningDetails>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = string.Empty;
                if (Status.ToLower() == "all")
                    sql = sql = "SELECT PP.PaintPlanID, PP.PlanEnteryDate,PP.PlanningDate, MO.Part_id, MO.Part_name, PP.SubModel, SM.Subpart_Name, PP.Color, CO.ColorcolName, PP.Quantity, PP.Skid, concat(SK.JigsPerSkid, ' X ',PartsPerJigs) as Matrix,SK.JigsPerSkid,SK.PartsPerJigs, PP.Round, PP.BPShortCode, BP.Name, PP.Plant_Id, PP.Company_Id, Status " +
                          "FROM paintplanning as PP " +
                          "left join sub_model as SM on PP.SubModel = SM.Subpart_id " +
                          "left join color as CO on PP.Color = CO.ColorID " +
                          "left join model as MO on SM.Part_id = MO.Part_id " +
                          "left join skid as SK on PP.Skid = SK.Skid_config_id " +
                          "left join business_partner as BP on PP.BPShortCode = BP.BP_Short_code and PP.Plant_Id = BP.Plant_Id and PP.Company_ID = BP.Company_id " +
                          "Where PP.Company_ID = '" + CompanyID + "' and " +
                          "PP.Plant_Id = '" + PlantID + "'";
                else
                    sql = "SELECT PP.PaintPlanID, PP.PlanEnteryDate,PP.PlanningDate, MO.Part_id, MO.Part_name, PP.SubModel, SM.Subpart_Name, PP.Color, CO.ColorcolName, PP.Quantity, PP.Skid, concat(SK.JigsPerSkid, ' X ',PartsPerJigs) as Matrix,SK.JigsPerSkid,SK.PartsPerJigs, PP.Round, PP.BPShortCode, BP.Name, PP.Plant_Id, PP.Company_Id, Status " +
                          "FROM paintplanning as PP " +
                          "left join sub_model as SM on PP.SubModel = SM.Subpart_id " +
                          "left join color as CO on PP.Color = CO.ColorID " +
                          "left join model as MO on SM.Part_id = MO.Part_id " +
                          "left join skid as SK on PP.Skid = SK.Skid_config_id " +
                          "left join business_partner as BP on PP.BPShortCode = BP.BP_Short_code and PP.Plant_Id = BP.Plant_Id and PP.Company_ID = BP.Company_id " +
                          "Where PP.Company_ID = '" + CompanyID + "' and " +
                          "PP.Plant_Id = '" + PlantID + "' and " +
                          "PP.Status != '" + Status + "'";

                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    PaintPlanningDetails paintPlanning = new PaintPlanningDetails();
                    if (rdr["PaintPlanID"] != DBNull.Value)
                        paintPlanning.PaintPlanID = Convert.ToInt32(rdr["PaintPlanID"]);
                    else
                        paintPlanning.PaintPlanID = null;
                    if (rdr["PlanningDate"] != DBNull.Value)
                        paintPlanning.PlanningDate = Convert.ToDateTime(rdr["PlanningDate"]);
                    else
                        paintPlanning.PlanningDate = null;
                    if (rdr["Part_id"] != DBNull.Value)
                        paintPlanning.Part_id = Convert.ToInt32(rdr["Part_id"]);
                    else
                        paintPlanning.Part_id = null;
                    paintPlanning.Part_name = rdr["Part_name"].ToString();
                    if (rdr["SubModel"] != DBNull.Value)
                        paintPlanning.SubModel = Convert.ToInt32(rdr["SubModel"]);
                    else
                        paintPlanning.SubModel = null;
                    paintPlanning.Subpart_Name = rdr["Subpart_Name"].ToString();
                    if (rdr["Color"] != DBNull.Value)
                        paintPlanning.Color = Convert.ToInt32(rdr["Color"]);
                    else
                        paintPlanning.Color = null;
                    paintPlanning.ColorcolName = rdr["ColorcolName"].ToString();
                    if (rdr["Quantity"] != DBNull.Value)
                        paintPlanning.Quantity = Convert.ToInt32(rdr["Quantity"]);
                    else
                        paintPlanning.Quantity = null;
                    if (rdr["Skid"] != DBNull.Value)
                        paintPlanning.Skid = Convert.ToInt32(rdr["Skid"]);
                    else
                        paintPlanning.Skid = null;
                    paintPlanning.Matrix = rdr["Matrix"].ToString();
                    if (rdr["JigsPerSkid"] != DBNull.Value)
                        paintPlanning.JigsPerSkid = Convert.ToInt32(rdr["JigsPerSkid"]);
                    else
                        paintPlanning.JigsPerSkid = null;
                    if (rdr["PartsPerJigs"] != DBNull.Value)
                        paintPlanning.PartsPerJigs = Convert.ToInt32(rdr["PartsPerJigs"]);
                    else
                        paintPlanning.PartsPerJigs = null;
                    if (rdr["Round"] != DBNull.Value)
                        paintPlanning.Round = Convert.ToInt32(rdr["Round"]);
                    else
                        paintPlanning.Round = null;
                    if (rdr["Status"] != DBNull.Value)
                        paintPlanning.Status = rdr["Status"].ToString();
                    else
                        paintPlanning.Status = null;
                    if (rdr["BPShortCode"] != DBNull.Value)
                        paintPlanning.BPShortCode = rdr["BPShortCode"].ToString();
                    else
                        paintPlanning.BPShortCode = null;
                    if (rdr["PlanEnteryDate"] != DBNull.Value)
                        paintPlanning.PlanEnteryDate = Convert.ToDateTime(rdr["PlanEnteryDate"]);
                    else
                        paintPlanning.PlanEnteryDate = null;
                    paintPlanning.Name = rdr["Name"].ToString();
                    paintPlanning.Plant_Id = rdr["Plant_Id"].ToString();
                    paintPlanning.Company_Id = rdr["company_id"].ToString();
                    planningDetails.Add(paintPlanning);
                }
                return Ok(planningDetails);
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

        // POST api/<PaintPlanningController>
        [HttpPost]
        public IActionResult Post([FromBody] PaintPlanning  pp)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "insert into paintplanning (PaintPlanID,PlanningDate,SubModel,Color,Quantity,Skid,Round,BPShortCode,Plant_Id,Company_Id,Status,PlanEnteryDate) values (@PaintPlanID,@PlanningDate,@SubModel,@Color,@Quantity,@Skid,@Round,@BPShortCode,@Plant_Id,@Company_Id,@Status,@PlanEnteryDate)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (pp.PaintPlanID == null)
                    cmd.Parameters.Add(new MySqlParameter("@PaintPlanID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PaintPlanID", MySqlDbType.Int32)).Value = pp.PaintPlanID;
                if (pp.PlanningDate == null)
                    cmd.Parameters.Add(new MySqlParameter("@PlanningDate", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PlanningDate", MySqlDbType.Date)).Value = pp.PlanningDate;
                if (pp.SubModel == null)
                    cmd.Parameters.Add(new MySqlParameter("@SubModel", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@SubModel", MySqlDbType.Int32)).Value = pp.SubModel;
                if (pp.Color == null)
                    cmd.Parameters.Add(new MySqlParameter("@Color", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Color", MySqlDbType.Int32)).Value = pp.Color;
                if (pp.Quantity == null)
                    cmd.Parameters.Add(new MySqlParameter("@Quantity", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Quantity", MySqlDbType.Int32)).Value = pp.Quantity;
                if (pp.Skid == null)
                    cmd.Parameters.Add(new MySqlParameter("@Skid", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Skid", MySqlDbType.Int32)).Value = pp.Skid;
                if (pp.Round == null)
                    cmd.Parameters.Add(new MySqlParameter("@Round", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Round", MySqlDbType.Int32)).Value = pp.Round;
                if (pp.BPShortCode == null)
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = pp.BPShortCode;
                if (pp.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = pp.Plant_Id;
                if (pp.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = pp.Company_Id;
                if (pp.Status == null)
                    cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.VarChar)).Value = pp.Status;
                if (pp.PlanEnteryDate == null)
                    cmd.Parameters.Add(new MySqlParameter("@PlanEnteryDate", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PlanEnteryDate", MySqlDbType.Date)).Value = pp.PlanEnteryDate;
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

        // PUT api/<PaintPlanningController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] PaintPlanning pp)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "update paintplanning set PaintPlanID = @PaintPlanID, PlanningDate = @PlanningDate, SubModel = @SubModel, Color = @Color, Quantity = @Quantity, Skid = @Skid, Round = @Round, BPShortCode = @BPShortCode, Plant_Id = @Plant_Id, Company_Id = @Company_Id, Status = @Status, PlanEnteryDate = @PlanEnteryDate where PaintPlanID = " + id;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (pp.PaintPlanID == null)
                    cmd.Parameters.Add(new MySqlParameter("@PaintPlanID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PaintPlanID", MySqlDbType.Int32)).Value = pp.PaintPlanID;
                if (pp.PlanningDate == null)
                    cmd.Parameters.Add(new MySqlParameter("@PlanningDate", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PlanningDate", MySqlDbType.Date)).Value = pp.PlanningDate;
                if (pp.SubModel == null)
                    cmd.Parameters.Add(new MySqlParameter("@SubModel", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@SubModel", MySqlDbType.Int32)).Value = pp.SubModel;
                if (pp.Color == null)
                    cmd.Parameters.Add(new MySqlParameter("@Color", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Color", MySqlDbType.Int32)).Value = pp.Color;
                if (pp.Quantity == null)
                    cmd.Parameters.Add(new MySqlParameter("@Quantity", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Quantity", MySqlDbType.Int32)).Value = pp.Quantity;
                if (pp.Skid == null)
                    cmd.Parameters.Add(new MySqlParameter("@Skid", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Skid", MySqlDbType.Int32)).Value = pp.Skid;
                if (pp.Round == null)
                    cmd.Parameters.Add(new MySqlParameter("@Round", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Round", MySqlDbType.Int32)).Value = pp.Round;
                if (pp.BPShortCode == null)
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = pp.BPShortCode;
                if (pp.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = pp.Plant_Id;
                if (pp.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = pp.Company_Id;
                if (pp.Status == null)
                    cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.VarChar)).Value = pp.Status;
                if (pp.PlanEnteryDate == null)
                    cmd.Parameters.Add(new MySqlParameter("@PlanEnteryDate", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PlanEnteryDate", MySqlDbType.Date)).Value = pp.PlanEnteryDate;
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

        // DELETE api/<PaintPlanningController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from paintplanning where PaintPlanID = " + id;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                cmd.ExecuteNonQuery();
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
