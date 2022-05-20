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
    public class PaintPlanEexcecutionController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public PaintPlanEexcecutionController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        // GET api/<PaintPlanEexcecutionController>/1000/1010
        [HttpGet("{COMPANYID}/{PLANTID}")]
        public IActionResult Get(string COMPANYID, string PLANTID, [FromQuery] bool Details = true)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = string.Empty;
                MySqlCommand cmd = null;
                if(!Details)
                {
                    List<PaintPlanEexcecution> paintPlanningExes = new List<PaintPlanEexcecution>();
                    sql = "select * from paint_plan_excecution where status = 1 and Plant_Id = '" + PLANTID + "' and Company_Id = '" + COMPANYID + "'";
                    cmd = new MySqlCommand(sql, connection)
                    { CommandType = CommandType.Text };
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        PaintPlanEexcecution ppe = new PaintPlanEexcecution();
                        if (rdr["PaintPlanExecutionID"] != DBNull.Value)
                            ppe.PaintPlanExecutionID = Convert.ToInt32(rdr["PaintPlanExecutionID"]);
                        else
                            ppe.PaintPlanExecutionID = null;
                        if (rdr["PaintPlanID"] != DBNull.Value)
                            ppe.PaintPlanID = Convert.ToInt32(rdr["PaintPlanID"]);
                        else
                            ppe.PaintPlanID = null;
                        if (rdr["DateOfExecution"] != DBNull.Value)
                            ppe.DateOfExecution = Convert.ToDateTime(rdr["DateOfExecution"]);
                        else
                            ppe.DateOfExecution = null;
                        if (rdr["EstimatedQuantity"] != DBNull.Value)
                            ppe.EstimatedQuantity = Convert.ToInt32(rdr["EstimatedQuantity"]);
                        else
                            ppe.EstimatedQuantity = null;
                        if (rdr["ActualRoundNo"] != DBNull.Value)
                            ppe.ActualRoundNo = Convert.ToInt32(rdr["ActualRoundNo"]);
                        else
                            ppe.ActualRoundNo = null;
                        ppe.Plant_Id = rdr["Plant_Id"].ToString();
                        ppe.Company_Id = rdr["Company_Id"].ToString();
                        ppe.BPShortCode = rdr["BPShortCode"].ToString();
                        paintPlanningExes.Add(ppe);
                    }
                    return Ok(paintPlanningExes);
                }
                else
                {
                    List<PaintPlanEexcecutionDetails> paintPlanningExeDetails = new List<PaintPlanEexcecutionDetails>();
                    sql = "select PE.PaintPlanExecutionID, PE.PaintPlanID, PE.DateOfExecution, SM.Part_id, MO.Part_name, " +
                          "PP.SubModel, SM.Subpart_Name, PP.Color, CO.ColorcolName, PP.Skid, " +
                          "concat(SK.PartsPerJigs, ' X ', SK.JigsPerSkid) as Matrix, PE.EstimatedQuantity, PP.Round, " +
                          "PE.StartSkidNumber, PE.Plant_Id, PE.Company_Id, PE.BPShortCode, BP.Name, PE.ActualRoundNo " +
                          "FROM paint_plan_excecution as PE " +
                          "left join paintplanning as PP on PE.PaintPlanID = PP.PaintPlanID " +
                          "left join sub_model as SM on PP.SubModel = SM.Subpart_id " +
                          "left join model as MO on SM.Part_id = MO.Part_id " +
                          "left join color as CO on PP.Color = CO.ColorID " +
                          "left join skid as SK on PP.Skid = SK.Skid_config_id " +
                          "left join business_partner as BP on PE.BPShortCode = BP.BP_Short_code and PE.company_id = BP.company_id and PE.Plant_Id = BP.Plant_Id " +
                          "where PE.status = 1 and PE.Plant_Id = '" + PLANTID + "' and PE.Company_Id = '" + COMPANYID + "'";
                    cmd = new MySqlCommand(sql, connection)
                    { CommandType = CommandType.Text };
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        PaintPlanEexcecutionDetails ppe = new PaintPlanEexcecutionDetails();
                        if (rdr["PaintPlanExecutionID"] != DBNull.Value)
                            ppe.PaintPlanExecutionID = Convert.ToInt32(rdr["PaintPlanExecutionID"]);
                        else
                            ppe.PaintPlanExecutionID = null;
                        if (rdr["PaintPlanID"] != DBNull.Value)
                            ppe.PaintPlanID = Convert.ToInt32(rdr["PaintPlanID"]);
                        else
                            ppe.PaintPlanID = null;
                        if (rdr["DateOfExecution"] != DBNull.Value)
                            ppe.DateOfExecution = Convert.ToDateTime(rdr["DateOfExecution"]);
                        else
                            ppe.DateOfExecution = null;
                        if (rdr["Part_id"] != DBNull.Value)
                            ppe.Part_id = Convert.ToInt32(rdr["Part_id"]);
                        else
                            ppe.Part_id = null;
                        ppe.Part_name = rdr["Part_name"].ToString();
                        if (rdr["SubModel"] != DBNull.Value)
                            ppe.SubModel = Convert.ToInt32(rdr["SubModel"]);
                        else
                            ppe.SubModel = null;
                        ppe.Subpart_Name = rdr["Subpart_Name"].ToString();
                        if (rdr["Color"] != DBNull.Value)
                            ppe.Color = Convert.ToInt32(rdr["Color"]);
                        else
                            ppe.Color = null;
                        ppe.ColorcolName = rdr["ColorcolName"].ToString();
                        if (rdr["Skid"] != DBNull.Value)
                            ppe.Skid = Convert.ToInt32(rdr["Skid"]);
                        else
                            ppe.Skid = null;
                        ppe.Matrix = rdr["Matrix"].ToString();
                        if (rdr["Round"] != DBNull.Value)
                            ppe.Round = Convert.ToInt32(rdr["Round"]);
                        else
                            ppe.Round = null;
                        if (rdr["EstimatedQuantity"] != DBNull.Value)
                            ppe.EstimatedQuantity = Convert.ToInt32(rdr["EstimatedQuantity"]);
                        else
                            ppe.EstimatedQuantity = null;
                        if (rdr["StartSkidNumber"] != DBNull.Value)
                            ppe.StartSkidNumber = Convert.ToInt32(rdr["StartSkidNumber"]);
                        else
                            ppe.StartSkidNumber = null;
                        if (rdr["ActualRoundNo"] != DBNull.Value)
                            ppe.ActualRoundNo = Convert.ToInt32(rdr["ActualRoundNo"]);
                        else
                            ppe.ActualRoundNo = null;
                        ppe.Plant_Id = rdr["Plant_Id"].ToString();
                        ppe.Company_Id = rdr["Company_Id"].ToString();
                        ppe.BPShortCode = rdr["BPShortCode"].ToString();
                        ppe.BPName = rdr["Name"].ToString();
                        paintPlanningExeDetails.Add(ppe);
                    }
                    return Ok(paintPlanningExeDetails);
                }
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
            // POST api/<PaintPlanEexcecutionController>
        [HttpPost]
        public IActionResult Post([FromBody] PaintPlanEexcecution pp)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null;
            try
            {
                connection.Open();
                myTrans = connection.BeginTransaction();
                string sql = string.Empty;
                MySqlCommand cmd = null;
                MySqlDataReader rdr = null;
                int RoundNo = 0,month = 0;
                sql = "select RoundNo,RoundMonth from general_settings where Plant_ID = '" + pp.Plant_Id + "' and Company_ID = '" + pp.Company_Id + "'";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    month = Convert.ToInt32(rdr["RoundMonth"]);
                    RoundNo = Convert.ToInt32(rdr["RoundNo"]);
                }
                else
                    return StatusCode(700, "Unable to read General Settings");
                rdr.Close();
                if (pp.DateOfExecution.Value.Day == 1 && month != pp.DateOfExecution.Value.Month)
                {
                    //Reset Round number
                    RoundNo = 1;
                    sql = "update general_settings set RoundNo = " + RoundNo + ", StartSkid = " + pp.StartSkidNumber + " where Plant_ID = '" + pp.Plant_Id + "' and Company_ID = '" + pp.Company_Id + "'";
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    //Setting Start skid number for new round.
                    sql = "select count(*) as Total from paint_plan_excecution where status = 1 and ActualRoundNo = " + RoundNo;
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    bool flag = false;
                    if (rdr.Read())
                    {
                        int Total = Convert.ToInt32(rdr["Total"]);
                        rdr.Close();
                        flag = true;
                        if(Total == 0)
                        {
                            sql = "update general_settings set StartSkid = " + pp.StartSkidNumber + " where Plant_ID = '" + pp.Plant_Id + "' and Company_ID = '" + pp.Company_Id + "'";
                            cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                            cmd.ExecuteNonQuery();
                        }
                    }
                    if (!flag)
                        rdr.Close();
                    
                }

                
                //*****************************************************************************************************
                //                            Enter the value in paint loading table
                //*****************************************************************************************************
                
                int? Qty = pp.EstimatedQuantity;
                int? StartSkidNumber = pp.StartSkidNumber;
                int JigsPerSkid;
                int PartsPerJigs;
                int TotalSkids;
                bool LR;
                bool AD;
                bool DA;
                bool AU;
                bool UA;
                sql = "select JigsPerSkid, PartsPerJigs from skid where Skid_config_id = (select Skid from paintplanning where PaintPlanID = " + pp.PaintPlanID + ")";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    JigsPerSkid = Convert.ToInt32(rdr["JigsPerSkid"]);
                    PartsPerJigs = Convert.ToInt32(rdr["PartsPerJigs"]);
                }
                else
                {
                    try { myTrans.Rollback(); } catch { }
                    return StatusCode(700, "Skid data not found");
                }
                rdr.Close();
                sql = "select TotalSkids, LeftToTight, AcrossThenDown, DownThenAcross, AcrossThenUP, UPThenAcross from general_settings where Company_ID = '" + pp.Company_Id + "' and Plant_Id = '" + pp.Plant_Id + "'";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    TotalSkids = Convert.ToInt32(rdr["TotalSkids"]);
                    LR = Convert.ToBoolean(rdr["LeftToTight"]);
                    AD = Convert.ToBoolean(rdr["AcrossThenDown"]);
                    DA = Convert.ToBoolean(rdr["DownThenAcross"]);
                    AU = Convert.ToBoolean(rdr["AcrossThenUP"]);
                    UA = Convert.ToBoolean(rdr["UPThenAcross"]);
                }
                else
                {
                    try { myTrans.Rollback(); } catch { }
                    return StatusCode(700, "General setting data not found");
                }
                rdr.Close();
                // check any skid is vacant for current round 
                int q = (int)Math.Ceiling((double)Qty / ((double)JigsPerSkid * (double)PartsPerJigs)) ;
                int endskidnumber = (int)(StartSkidNumber + q - 1)%TotalSkids;
                if (endskidnumber == 0) endskidnumber = TotalSkids;
                cmd = new MySqlCommand("CheckVacantSkid", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                };
                cmd.Parameters.Add("?DOE", MySqlDbType.Date).Value = pp.DateOfExecution;
                cmd.Parameters.Add("?RoundNo", MySqlDbType.Int32).Value = RoundNo;
                cmd.Parameters.Add("?StartNumber", MySqlDbType.Int32).Value = pp.StartSkidNumber;
                cmd.Parameters.Add("?EndNumber", MySqlDbType.Int32).Value = endskidnumber;
                cmd.Parameters.Add("?CompanyID", MySqlDbType.Int32).Value = pp.Company_Id;
                cmd.Parameters.Add("?PlantID", MySqlDbType.Int32).Value = pp.Plant_Id;
                cmd.Parameters.Add(new MySqlParameter("mRes", MySqlDbType.String, 0));
                cmd.Parameters["mRes"].Direction = ParameterDirection.ReturnValue;
                cmd.ExecuteNonQuery();
                int isvacant = Convert.ToInt16(cmd.Parameters["mRes"].Value);
                if (isvacant == 0)
                    return StatusCode(700, "Select anothe start skid number");
                //***************************************************************************
                sql = "insert into paint_plan_excecution (PaintPlanID, DateOfExecution, EstimatedQuantity, StartSkidNumber, Plant_Id, Company_Id, BPShortCode,Status,ActualRoundNo) values (@PaintPlanID, @DateOfExecution, @EstimatedQuantity, @StartSkidNumber, @Plant_Id, @Company_Id, @BPShortCode,@Status,@ActualRoundNo)";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (pp.PaintPlanID == null)
                    cmd.Parameters.Add(new MySqlParameter("@PaintPlanID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PaintPlanID", MySqlDbType.Int32)).Value = pp.PaintPlanID;
                if (pp.DateOfExecution == null)
                    cmd.Parameters.Add(new MySqlParameter("@DateOfExecution", MySqlDbType.DateTime)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@DateOfExecution", MySqlDbType.DateTime)).Value = pp.DateOfExecution;
                if (pp.EstimatedQuantity == null)
                    cmd.Parameters.Add(new MySqlParameter("@EstimatedQuantity", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@EstimatedQuantity", MySqlDbType.Int32)).Value = pp.EstimatedQuantity;
                if (pp.StartSkidNumber == null)
                    cmd.Parameters.Add(new MySqlParameter("@StartSkidNumber", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@StartSkidNumber", MySqlDbType.Int32)).Value = pp.StartSkidNumber;
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
                cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.Int16)).Value = 1;
                cmd.Parameters.Add(new MySqlParameter("@ActualRoundNo", MySqlDbType.VarChar)).Value = RoundNo;
                cmd.ExecuteNonQuery();
                int ppexeid = (int)cmd.LastInsertedId;
                //***************************************************************************
                DataTable dt = GetSkidMatrix(Qty, StartSkidNumber, ppexeid, JigsPerSkid, PartsPerJigs, TotalSkids, LR, AD, DA, AU, UA);
                cmd = new MySqlCommand("BulkUploadPaintLoading", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = myTrans,
                    UpdatedRowSource = UpdateRowSource.None
                };
                cmd.Parameters.Add("?Barcode", MySqlDbType.String).SourceColumn = "Barcode";
                cmd.Parameters.Add("?PaintPlanExecutionID", MySqlDbType.Int32).SourceColumn = "PaintPlanExecutionID";
                cmd.Parameters.Add("?SkidNumber", MySqlDbType.Int32).SourceColumn = "SkidNumber";
                cmd.Parameters.Add("?Position", MySqlDbType.String).SourceColumn = "Position";
                cmd.Parameters.Add("?ID", MySqlDbType.String).SourceColumn = "ID";
                cmd.Parameters.Add("?LoadUnload", MySqlDbType.String).SourceColumn = "LoadUnload";
                cmd.Parameters.Add("?LoadingDetails", MySqlDbType.String).SourceColumn = "LoadingDetails";
                cmd.Parameters.Add("?UnloadingDetails", MySqlDbType.String).SourceColumn = "UnloadingDetails";

                MySqlDataAdapter da = new MySqlDataAdapter()
                { InsertCommand = cmd };
                int records = da.Update(dt);
                myTrans.Commit();
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

        private DataTable GetSkidMatrix(int? Qty, int? StartSkidNumber, int PaintPlanExecutionID, int JigsPerSkid, int PartsPerJigs, int TotalSkids, bool LR, bool AD, bool DA, bool AU, bool UA)
        {
            int jigs = JigsPerSkid;
            int parts = PartsPerJigs;
            int? Quantty = Qty;
            bool flag = true;
            int qty = 1;
            string Level = string.Empty;
            int totalskd = (int)Math.Ceiling((float)Quantty / (float)(jigs * parts));
            DataTable dt = new DataTable();
            DataColumn dcName = new DataColumn("Barcode", typeof(string));
            dt.Columns.Add(dcName);
            dcName = new DataColumn("PaintPlanExecutionID", typeof(int));
            dt.Columns.Add(dcName);
            dcName = new DataColumn("SkidNumber", typeof(int));
            dt.Columns.Add(dcName);
            dcName = new DataColumn("Position", typeof(string));
            dt.Columns.Add(dcName);
            dcName = new DataColumn("ID", typeof(string));
            dt.Columns.Add(dcName);
            dcName = new DataColumn("LoadUnload", typeof(bool));
            dt.Columns.Add(dcName);
            dcName = new DataColumn("LoadingDetails", typeof(long));
            dt.Columns.Add(dcName);
            dcName = new DataColumn("UnloadingDetails", typeof(long));
            dt.Columns.Add(dcName);
            
            for (int sk = 1; sk <= totalskd && flag; sk++)
            {
                int? sknumber;
                if (StartSkidNumber != 1)
                {
                    sknumber = (StartSkidNumber + sk - 1) % TotalSkids;
                    if (sknumber == 0)
                    { sknumber = TotalSkids; }
                }
                else
                {
                    sknumber = sk;
                }
                if (AD || AU)
                {
                    for (int j = 0; j < jigs; j++)
                    {
                        if (j == 0 && AD) Level = "TOP";
                        if (j == jigs - 1 && AD) Level = "Bottom";
                        if (jigs == 3 && j > 0 && j < jigs - 1 && AD) Level = "Middle";
                        else if (j > 0 && j < jigs - 1 && AD) Level = "Middle_" + (j);
                        if (jigs == 3 && j > 0 && j < jigs - 1 && AU) Level = "Middle";
                        else if (j > 0 && j < jigs - 1 && AU) Level = "Middle_" + (jigs - j - 1);
                        if (j == 0 && AU) Level = "Bottom";
                        if (j == jigs - 1 && AU) Level = "TOP";
                        int k;
                        for (int i = 0; i < parts; i++)
                        {
                            if (LR) k = i + 1; else k = parts - i;
                            DataRow ldr = dt.NewRow();
                            ldr["Barcode"] = null;
                            ldr["PaintPlanExecutionID"] = PaintPlanExecutionID;
                            ldr["SkidNumber"] = sknumber;
                            ldr["Position"] = Level + (k);
                            ldr["ID"] = null;
                            ldr["LoadUnload"] = DBNull.Value;
                            ldr["LoadingDetails"] = DBNull.Value;
                            ldr["UnloadingDetails"] = DBNull.Value; 
                            
                            dt.Rows.Add(ldr);
                            //listBox1.Items.Add(sknumber + " - " + Level + " : " + k);
                            qty++;
                            if (qty > Quantty)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (!flag) break;
                    }
                }
                else
                {
                    for (int j = 0; j < parts; j++)
                    {
                        int k;
                        for (int i = 0; i < jigs; i++)
                        {
                            if (i == 0 && DA) Level = "TOP";
                            if (i == jigs - 1 && DA) Level = "Bottom";
                            if (jigs == 3 && i > 0 && i < jigs - 1 && DA) Level = "Middle";
                            else if (i > 0 && i < jigs - 1 && DA) Level = "Middle_" + (j + 1);
                            if (jigs == 3 && i > 0 && i < jigs - 1 && UA) Level = "Middle";
                            else if (i > 0 && i < jigs - 1 && UA) Level = "Middle_" + (jigs - i - 1);
                            if (i == 0 && UA) Level = "Bottom";
                            if (i == jigs - 1 && UA) Level = "TOP";
                            if (LR) k = j + 1; else k = parts - j;
                            DataRow ldr = dt.NewRow();
                            ldr["Barcode"] = null;
                            ldr["PaintPlanExecutionID"] = PaintPlanExecutionID;
                            ldr["SkidNumber"] = sknumber;
                            ldr["Position"] = Level + (k);
                            ldr["ID"] = null;
                            ldr["LoadUnload"] = DBNull.Value;
                            ldr["LoadingDetails"] = DBNull.Value;
                            ldr["UnloadingDetails"] = DBNull.Value;
                            dt.Rows.Add(ldr);
                            //listBox1.Items.Add(sknumber + " - " + Level + (k));
                            qty++;
                            if (qty > Quantty)
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (!flag) break;
                    }
                }
            }
            return dt;
        }

        // PUT api/<PaintPlanEexcecutionController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] PaintPlanEexcecution pp)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null; ;
            try
            {
                connection.Open();
                myTrans = connection.BeginTransaction();
                string sql;
                MySqlCommand cmd;
                if (!pp.Status.Value )
                {
                    sql = "select count(*) as Total from paintloading where PaintPlanExecutionID = " + pp.PaintPlanExecutionID + " and Barcode is not null";
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        int c = Convert.ToInt32(rdr["Total"]);
                        if (c > 0)
                            return StatusCode(700, "You can't close Execution Plan. First unload");
                    }
                    else
                        return StatusCode(500, "Somting wrong in fetching paintload data");
                    rdr.Close();
                }
                sql = "update paint_plan_excecution set PaintPlanID = @PaintPlanID, DateOfExecution = @DateOfExecution, EstimatedQuantity = @EstimatedQuantity, StartSkidNumber = @StartSkidNumber, Plant_Id = @Plant_Id, Company_Id = @Company_Id, BPShortCode = @BPShortCode, Status = @Status where PaintPlanExecutionID = " + id;
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (pp.PaintPlanID == null)
                    cmd.Parameters.Add(new MySqlParameter("@PaintPlanID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PaintPlanID", MySqlDbType.Int32)).Value = pp.PaintPlanID;
                if (pp.DateOfExecution == null)
                    cmd.Parameters.Add(new MySqlParameter("@DateOfExecution", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@DateOfExecution", MySqlDbType.Date)).Value = pp.DateOfExecution;
                
                if (pp.EstimatedQuantity == null)
                    cmd.Parameters.Add(new MySqlParameter("@EstimatedQuantity", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@EstimatedQuantity", MySqlDbType.Int32)).Value = pp.EstimatedQuantity;
                if (pp.StartSkidNumber == null)
                    cmd.Parameters.Add(new MySqlParameter("@StartSkidNumber", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@StartSkidNumber", MySqlDbType.Int32)).Value = pp.StartSkidNumber;
                if (pp.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = pp.Plant_Id;
                if (pp.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = pp.Company_Id;
                if (pp.BPShortCode == null)
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = pp.BPShortCode;
                if (pp.Status == null)
                    cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.Int16)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.Int16)).Value = Convert.ToInt16(pp.Status);
                if (pp.ActualRoundNo == null)
                    cmd.Parameters.Add(new MySqlParameter("@ActualRoundNo", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ActualRoundNo", MySqlDbType.Int32)).Value = pp.ActualRoundNo;
                cmd.ExecuteNonQuery();
                
                sql = "update paintplanning set Status = 'Closed' where PaintPlanID = " + pp.PaintPlanID;
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                cmd.ExecuteNonQuery();
                myTrans.Commit();
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

        // DELETE api/<PaintPlanEexcecutionController>/5
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
