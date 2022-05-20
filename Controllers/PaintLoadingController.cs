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
    public class PaintLoadingController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public PaintLoadingController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<PaintLoadingController>
        [HttpGet("{CompanyID}/{PlantID}")]
        public IActionResult Get(string CompanyID, string PlantID ,[FromQuery] bool LoadUnload = false)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                //in future station is required
                string sql = "Select PaintPlanExecutionID from paint_plan_excecution where Company_ID = '" + CompanyID + "' and Plant_ID = '" + PlantID + "' and Status = " + 1;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                List<int> ppexe = new List<int>();
                while (rdr.Read())
                {
                    ppexe.Add(Convert.ToInt32(rdr["PaintPlanExecutionID"]));
                }
                if(ppexe.Count < 1)
                {
                    return StatusCode(700, "No any active paint plain.");
                }
                rdr.Close();
                string st = string.Join(",", ppexe.ToArray());
                st = "(" + st + ")";
                if (!LoadUnload)
                {
                    sql = "Select PL.Barcode, PL.PaintPlanExecutionID, PL.SkidNumber, PL.Position, " +
                      "BD.BP_ShortCode, BD.Plant_Id, BD.Company_Id, PL.ID, CC.ColorID, CC.ColorcolName, PP.SubModel, SM.Subpart_id, SM.Subpart_Name " +
                      "from PaintLoading as PL " +
                      "left join paint_plan_excecution as PE on PL.PaintPlanExecutionID = PE.PaintPlanExecutionID " +
                      "left join paintplanning as PP on PE.PaintPlanID = PP.PaintPlanID " +
                      "left join color as CC on PP.Color = CC.ColorID " +
                      "left join sub_model as SM on PP.SubModel = SM.Subpart_id " +
                      "left join barcodedetails BD on PL.Barcode = BD.Barcode and PL.LoadingDetails = BD.ID " +
                      "where PL.PaintPlanExecutionID in " + st + " and PL.Barcode is null order by PL.ID ASC";
                }
                else
                {
                    sql = "Select PL.Barcode, PL.PaintPlanExecutionID, PL.SkidNumber, PL.Position, " +
                      "BD.BP_ShortCode, BD.Plant_Id, BD.Company_Id, PL.ID, CC.ColorID, CC.ColorcolName, PP.SubModel, SM.Subpart_id, SM.Subpart_Name " +
                      "from PaintLoading as PL " +
                      "left join paint_plan_excecution as PE on PL.PaintPlanExecutionID = PE.PaintPlanExecutionID " +
                      "left join paintplanning as PP on PE.PaintPlanID = PP.PaintPlanID " +
                      "left join color as CC on PP.Color = CC.ColorID " +
                      "left join sub_model as SM on PP.SubModel = SM.Subpart_id " +
                      "left join barcodedetails BD on PL.Barcode = BD.Barcode and PL.LoadingDetails = BD.ID " +
                      "where PL.PaintPlanExecutionID in " + st + " and PL.Barcode is not null and PL.loadUnload = 0 order by PL.ID ASC";
                }

                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                rdr = cmd.ExecuteReader();
                bool flag = false;
                List<PaintLoading> pls = new List<PaintLoading>();
                while (rdr.Read())
                {
                    flag = true;
                    PaintLoading pl = new PaintLoading();
                    pl.ID = rdr["ID"].ToString();
                    pl.Barcode = rdr["Barcode"].ToString();
                    if (rdr["PaintPlanExecutionID"] != DBNull.Value)
                        pl.PaintPlanExecutionID = Convert.ToInt32(rdr["PaintPlanExecutionID"]);
                    else
                        pl.PaintPlanExecutionID = null;
                    if (rdr["SkidNumber"] != DBNull.Value)
                        pl.SkidNumber = Convert.ToInt32(rdr["SkidNumber"]);
                    else
                        pl.SkidNumber = null;
                    pl.Position = rdr["Position"].ToString();
                    pl.BPShortCode = rdr["BP_ShortCode"].ToString();
                    pl.Plant_Id = rdr["Plant_Id"].ToString();
                    pl.Company_Id = rdr["Company_Id"].ToString();
                    pl.ColorName = rdr["ColorcolName"].ToString();
                    if (rdr["ColorID"] != DBNull.Value)
                        pl.ColorId = Convert.ToInt32(rdr["ColorID"]);
                    else
                        pl.ColorId = null;
                    pl.Subpart_Name = rdr["Subpart_Name"].ToString();
                    pl.SubpartID = Convert.ToInt32(rdr["Subpart_id"]);
                    pls.Add(pl);
                }
                if (!flag)
                {
                    return StatusCode(700, "No more any part remening.");
                }
                return Ok(pls);

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

        // POST api/<PaintLoadingController>
        [HttpPost]
        public IActionResult Post([FromBody] PaintLoading pl)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "insert into paintloading (PaintPlanExecutionID, SkidNumber, Position) values (@PaintPlanExecutionID, @SkidNumber, @Position)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (pl.PaintPlanExecutionID == null)
                    cmd.Parameters.Add(new MySqlParameter("@PaintPlanExecutionID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PaintPlanExecutionID", MySqlDbType.Int32)).Value = pl.PaintPlanExecutionID;
                if (pl.SkidNumber == null)
                    cmd.Parameters.Add(new MySqlParameter("@SkidNumber", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@SkidNumber", MySqlDbType.Int32)).Value = pl.SkidNumber;
                if (pl.Position == null)
                    cmd.Parameters.Add(new MySqlParameter("@Position", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Position", MySqlDbType.Int32)).Value = pl.Position;
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

        // PUT api/<PaintLoadingController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] PaintLoading pl)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            long ID = 0;
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null; 
            try
            {
                connection.Open();
                myTrans = connection.BeginTransaction();
                string sql = string.Empty;
                MySqlCommand cmd = null;
                MySqlDataReader rdr = null;
                //****************************************************************************************
                //                               check the bar code is ok for painting
                //****************************************************************************************
                if (pl.Barcode != null)
                {
                    sql = "select Subpart_id from barcodemaster BM " +
                          "left join erpnumber as ER on BM.ERPNumberID = ER.ERPNumberID " +
                          "where BM.Barcode = '" + pl.Barcode + "'";
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    if(rdr.Read())
                    {
                        int Sid = Convert.ToInt32(rdr["Subpart_id"]);
                        if (Sid != pl.SubpartID)
                            return StatusCode(700, "Invalid Barcode");
                    }
                    else
                        return StatusCode(700, "Unable to find subpart");
                    rdr.Close();
                    sql = "select OkDefectCode, CurrentDefectCode from scanstation, barcodemaster  where ScanstationName = 'Mold Scan' and Barcode = '" + pl.Barcode + "';";
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    int MCode = 0;
                    int CurrentCode = 0;
                    if (rdr.Read())
                    {
                        MCode = Convert.ToInt32(rdr["OkDefectCode"]);
                        CurrentCode = Convert.ToInt32(rdr["CurrentDefectCode"]);
                    }
                    else
                        return StatusCode(700, "Defect code not set for Mold Scan station");
                    rdr.Close();
                    if (MCode != CurrentCode)
                    {
                        sql = "select Workstation_name from workstation where Workstation_Id =(select To_Station from defect_code where defect_code_id = " + CurrentCode + ")";
                        cmd = cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                        rdr = cmd.ExecuteReader();
                        if (rdr.Read())
                        {
                            string wName = rdr["Workstation_name"].ToString();
                            if (wName.ToLower() != "paint" && wName.ToLower() != "repaint")
                                return StatusCode(700, "Invalid barcode");
                        }
                        else
                            return StatusCode(700, "Workstation is not assigned to current defectcode of barcode");
                        rdr.Close();
                    }
                }
                else
                {
                    try { myTrans.Rollback(); } catch { }
                    return StatusCode(700, "Barcode can't be empty");
                }
                //***********************************************************************************************
                //                  update barcode master table
                //**********************************************************************************************
                cmd = new MySqlCommand("PaintLoad", connection)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new MySqlParameter("ScanBarcode", MySqlDbType.VarChar)).Value = pl.Barcode;
                cmd.Parameters.Add(new MySqlParameter("PaintPlanExecutionID", MySqlDbType.VarChar)).Value = pl.PaintPlanExecutionID;
                cmd.Parameters.Add(new MySqlParameter("Color", MySqlDbType.VarChar)).Value = pl.ColorId;
                cmd.Parameters.Add(new MySqlParameter("StorageID", MySqlDbType.Int32)).Value = pl.SoregeID;
                cmd.Parameters.Add(new MySqlParameter("PlantID", MySqlDbType.VarChar)).Value = pl.Plant_Id;
                cmd.Parameters.Add(new MySqlParameter("CompanyID", MySqlDbType.VarChar)).Value = pl.Company_Id;
                cmd.Parameters.Add(new MySqlParameter("DefectCode", MySqlDbType.Int32)).Value = pl.OkDefectCode;
                cmd.Parameters.Add(new MySqlParameter("ReturnCode", MySqlDbType.Int32));

                cmd.Parameters["ReturnCode"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                int result = Convert.ToInt32(cmd.Parameters["ReturnCode"].Value);
                //int DefectCode = Convert.ToInt32(cmd.Parameters["DefectCode"].Value);
                if (result == 0)
                {
                    try { myTrans.Rollback(); } catch { }
                    return StatusCode(700, "Barcode not exist");
                }
                else if (result == 1)
                {
                    try { myTrans.Rollback(); } catch { }
                    return StatusCode(700, "ERP Number is not set");
                }
                //*********** Now update in Barcode Details Table ********************************
                sql = "insert into barcodedetails (Barcode,Workstation_Id,DefectCode,TimeStamp,BP_ShortCode,ExecutionType,ExecutionPlanNo,Plant_Id,Company_Id,Storage_id) values (@Barcode,@Workstation_Id,@DefectCode,@TimeStamp,@BP_ShortCode,@ExecutionType,@ExecutionPlanNo,@Plant_Id,@Company_Id,@Storage_id)";
                cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                cmd.Parameters.Add(new MySqlParameter("@Barcode", MySqlDbType.VarChar)).Value = pl.Barcode;
                cmd.Parameters.Add(new MySqlParameter("@Workstation_Id", MySqlDbType.VarChar)).Value = pl.Workstation_Id;
                // Get defect code from Brcode Master 
                cmd.Parameters.Add(new MySqlParameter("@DefectCode", MySqlDbType.Int32)).Value = pl.OkDefectCode;
                cmd.Parameters.Add(new MySqlParameter("@TimeStamp", MySqlDbType.VarChar)).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                cmd.Parameters.Add(new MySqlParameter("@BP_ShortCode", MySqlDbType.VarChar)).Value = pl.BPShortCode;
                cmd.Parameters.Add(new MySqlParameter("@ExecutionType", MySqlDbType.VarChar)).Value = "P";
                cmd.Parameters.Add(new MySqlParameter("@ExecutionPlanNo", MySqlDbType.Int32)).Value = pl.PaintPlanExecutionID;
                cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = pl.Plant_Id;
                cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = pl.Company_Id;
                cmd.Parameters.Add(new MySqlParameter("@Storage_id", MySqlDbType.Int32)).Value = pl.SoregeID;
                cmd.ExecuteScalar();
                ID = cmd.LastInsertedId;
                //****************************************************************************************
                //                            Update barcode (insert)
                //****************************************************************************************
                sql = "Update paintloading set Barcode = @Barcode, PaintPlanExecutionID = @PaintPlanExecutionID, SkidNumber = @SkidNumber, Position = @Position, LoadUnload = @LoadUnload, LoadingDetails = @LoadingDetails where ID = '" + id + "'";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (pl.Barcode == null)
                    cmd.Parameters.Add(new MySqlParameter("@Barcode", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Barcode", MySqlDbType.VarChar)).Value = pl.Barcode;
                if (pl.PaintPlanExecutionID == null)
                    cmd.Parameters.Add(new MySqlParameter("@PaintPlanExecutionID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PaintPlanExecutionID", MySqlDbType.Int32)).Value = pl.PaintPlanExecutionID;
                if (pl.SkidNumber == null)
                    cmd.Parameters.Add(new MySqlParameter("@SkidNumber", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@SkidNumber", MySqlDbType.Int32)).Value = pl.SkidNumber;
                if (pl.Position == null)
                    cmd.Parameters.Add(new MySqlParameter("@Position", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Position", MySqlDbType.VarChar)).Value = pl.Position;
                //if (pl.BPShortCode == null)
                //    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = DBNull.Value;
                //else
                //    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = pl.BPShortCode;
                //if (pl.Plant_Id == null)
                //    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                //else
                //    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = pl.Plant_Id;
                //if (pl.Company_Id == null)
                //    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                //else
                //    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = pl.Company_Id;
                cmd.Parameters.Add(new MySqlParameter("@LoadUnload", MySqlDbType.Int16)).Value = 0;
                cmd.Parameters.Add(new MySqlParameter("@LoadingDetails", MySqlDbType.Int64)).Value = ID;
                cmd.ExecuteNonQuery();
                //********************************************Master & Details Shifted above************************
                myTrans.Commit();
                return Ok("Record Updated Successfuly");
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


    }
}
