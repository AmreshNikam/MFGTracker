using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MFG_Tracker.DatabaseTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ZXing;
using ZXing.QrCode;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MFG_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScanStationController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public ScanStationController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        //*********************************Scan Station Settings*************************************
        [HttpGet("Settings")]
        public IActionResult Get([FromQuery] string PlantID, string CompanyID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            List<ScanStation> scanStations = new List<ScanStation>();
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select A.ID, A.ScanstationName, A.WorkStationID, B.Workstation_name, " + "A.OkDefectCode,A.StoregeID,E.Stores_id,E.Stores_Name, " +
                             "C.DefectbarCode, A.Plant_Id, A.Company_Id, " +
                             "concat(Right(concat('000',CONVERT(D.StoreID,char)),3), " +
                             "Right(concat('000',CONVERT(ifnull(D.Section,'0'),char)),3), " +
                             "Right(concat('000',CONVERT(ifnull(D.Rack,'0'),char)),3), " +
                             "Right(concat('000',CONVERT(ifnull(D.Shelf,'0'),char)),3), " +
                             "Right(concat('000',CONVERT(ifnull(D.Bins,'0'),char)),3)) as Code  " +
                             "from scanstation as A " +
                             "left join workstation as B on A.WorkStationID = B.Workstation_Id " +
                             "left join defect_code as C on A.OkDefectCode = C.defect_code_id " +
                             "left join storages as D on A.StoregeID = D.Storage_id " +
                             "left join Stores as E on D.StoreID = E.Stores_id where A.Plant_Id = '" + PlantID + "' and A.Company_Id = '" + CompanyID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    ScanStation ss = new ScanStation();
                    ss.ID = Convert.ToInt32(rdr["ID"]);
                    ss.ScanstationName = rdr["ScanstationName"].ToString();
                    if (rdr["WorkStationID"] != DBNull.Value)
                        ss.WorkStationID = Convert.ToInt32(rdr["WorkStationID"]);
                    else
                        ss.WorkStationID = null;
                    ss.Workstation_name = rdr["Workstation_name"].ToString();
                    if (rdr["OkDefectCode"] != DBNull.Value)
                        ss.OkDefectCode = Convert.ToInt32(rdr["OkDefectCode"]);
                    else
                        ss.OkDefectCode = null;
                    ss.DefectbarCode = rdr["DefectbarCode"].ToString();
                    ss.Plant_Id = rdr["Plant_Id"].ToString();
                    ss.Company_Id = rdr["Company_Id"].ToString();
                    if (rdr["StoregeID"] != DBNull.Value)
                        ss.StoregeID = Convert.ToInt32(rdr["StoregeID"]);
                    else
                        ss.StoregeID = null;
                    if (rdr["Stores_id"] != DBNull.Value)
                        ss.Stores_id = Convert.ToInt32(rdr["Stores_id"]);
                    else
                        ss.Stores_id = null;
                    ss.Stores_Name = rdr["Stores_Name"].ToString();
                    ss.Code = rdr["Code"].ToString();
                    scanStations.Add(ss);
                }
                return Ok(scanStations);
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
        [HttpPost("Settings")]
        public IActionResult Post([FromBody] List<ScanStation> scanStations)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null;
            try
            {
                connection.Open();
                myTrans = connection.BeginTransaction();
                foreach (ScanStation ss in scanStations)
                {
                    string sql = "Insert into scanstation (ScanstationName,WorkStationID,OkDefectCode,StoregeID,Plant_ID,Company_ID) values(@ScanstationName@WorkStationID,@OkDefectCode,@StoregeID,@Plant_Id,@Company_Id)";
                    MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    if (ss.Company_Id == null)
                        cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = ss.Company_Id;
                    if (ss.Plant_Id == null)
                        cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = ss.Plant_Id;
                    if (ss.ScanstationName == null)
                        cmd.Parameters.Add(new MySqlParameter("@ScanstationName", MySqlDbType.VarChar)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@ScanstationName", MySqlDbType.VarChar)).Value = ss.ScanstationName;
                    if (ss.WorkStationID == null)
                        cmd.Parameters.Add(new MySqlParameter("@WorkStationID", MySqlDbType.Int32)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@WorkStationID", MySqlDbType.Int32)).Value = ss.WorkStationID;
                    if (ss.OkDefectCode == null)
                        cmd.Parameters.Add(new MySqlParameter("@OkDefectCode", MySqlDbType.Int32)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@OkDefectCode", MySqlDbType.Int32)).Value = ss.OkDefectCode;
                    if (ss.StoregeID == null)
                        cmd.Parameters.Add(new MySqlParameter("@StoregeID", MySqlDbType.Int32)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@StoregeID", MySqlDbType.Int32)).Value = ss.StoregeID;
                    cmd.ExecuteNonQuery();
                }
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
        [HttpPut("Settings")]
        public IActionResult Put([FromBody] List<ScanStation> scanStations)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null;
            try
            {
                connection.Open();
                myTrans = connection.BeginTransaction();
                foreach (ScanStation ss in scanStations)
                {
                    string sql = "update scanstation set ScanstationName = @ScanstationName,WorkStationID = @WorkStationID,OkDefectCode = @OkDefectCode,StoregeID = @StoregeID,Plant_Id = @Plant_Id,Company_Id = @Company_Id where ID = " + ss.ID;
                    MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    if (ss.Company_Id == null)
                        cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = ss.Company_Id;
                    if (ss.Plant_Id == null)
                        cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = ss.Plant_Id;
                    if (ss.ScanstationName == null)
                        cmd.Parameters.Add(new MySqlParameter("@ScanstationName", MySqlDbType.VarChar)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@ScanstationName", MySqlDbType.VarChar)).Value = ss.ScanstationName;
                    if (ss.WorkStationID == null)
                        cmd.Parameters.Add(new MySqlParameter("@WorkStationID", MySqlDbType.Int32)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@WorkStationID", MySqlDbType.Int32)).Value = ss.WorkStationID;
                    if (ss.OkDefectCode == null)
                        cmd.Parameters.Add(new MySqlParameter("@OkDefectCode", MySqlDbType.Int32)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@OkDefectCode", MySqlDbType.Int32)).Value = ss.OkDefectCode;
                    if (ss.StoregeID == null)
                        cmd.Parameters.Add(new MySqlParameter("@StoregeID", MySqlDbType.Int32)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@StoregeID", MySqlDbType.Int32)).Value = ss.StoregeID;
                    cmd.ExecuteNonQuery();
                }
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
        [HttpDelete("Settings/{id}")]
        public IActionResult Delete(int id)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from scanstation where ID = " + id;
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
        //*********************************Mold Scan Station*****************************************
        [HttpPost("MoldScan")]
        public IActionResult Post([FromBody] MoldScan ms)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null;
            string sql;
            MySqlCommand cmd = null;
            try
            {
                connection.Open();
                if (ms.SoregeID == -1)
                {
                    sql = "select Storage_id from storages where StoreID =  " +
                          "(select Stores_id from stores where Stores_Name = " +
                          "(select Workstation_name from workstation where Workstation_Id =  " +
                          "(select To_Station from defect_code where defect_code_id = " + ms.DefectCode + ") " +
                          ") and Company_Id = '" + ms.Company_Id + "' and Plant_Id = '" + ms.Plant_Id + "' )";
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if(rdr.Read())
                    {
                        if (rdr["Storage_id"] != DBNull.Value)
                            ms.SoregeID = Convert.ToInt32(rdr["Storage_id"]);
                        else
                            return StatusCode(700, "Can't find default location please add from master data");
                    }
                    else
                        return StatusCode(700, "Can't find default location please add from master data");
                    rdr.Close();
                }
                myTrans = connection.BeginTransaction();
                cmd = new MySqlCommand("MoldScan", connection)
                { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.Add(new MySqlParameter("ScanBarcode", MySqlDbType.VarChar)).Value = ms.Barcode;
                cmd.Parameters.Add(new MySqlParameter("DefectCode", MySqlDbType.Int32)).Value = ms.DefectCode;
                cmd.Parameters.Add(new MySqlParameter("StorageID", MySqlDbType.Int32)).Value = ms.SoregeID;
                cmd.Parameters.Add(new MySqlParameter("PlantID", MySqlDbType.VarChar)).Value = ms.Plant_Id;
                cmd.Parameters.Add(new MySqlParameter("CompanyID", MySqlDbType.VarChar)).Value = ms.Company_Id;
                cmd.Parameters.Add(new MySqlParameter("OkCode", MySqlDbType.Int32)).Value = ms.OkCode;
                cmd.Parameters.Add(new MySqlParameter("ReturnCode", MySqlDbType.Int32));
                cmd.Parameters["ReturnCode"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add(new MySqlParameter("ExecutionPlan", MySqlDbType.Int32));
                cmd.Parameters["ExecutionPlan"].Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                int result = Convert.ToInt32(cmd.Parameters["ReturnCode"].Value);
                int ExePlan = Convert.ToInt32(cmd.Parameters["ExecutionPlan"].Value);
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
                else if (result == 2)
                {
                    try { myTrans.Rollback(); } catch { }
                    return StatusCode(700, "Already having Ok code");
                }
                //*********** Now update in Barcode Details Table ********************************
                sql = "insert into barcodedetails (Barcode,Workstation_Id,DefectCode,TimeStamp,BP_ShortCode,ExecutionType,ExecutionPlanNo,Plant_Id,Company_Id,Storage_id) values (@Barcode,@Workstation_Id,@DefectCode,@TimeStamp,@BP_ShortCode,@ExecutionType,@ExecutionPlanNo,@Plant_Id,@Company_Id,@Storage_id)";
                cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                cmd.Parameters.Add(new MySqlParameter("@Barcode", MySqlDbType.VarChar)).Value = ms.Barcode;
                cmd.Parameters.Add(new MySqlParameter("@Workstation_Id", MySqlDbType.VarChar)).Value = ms.Workstation_Id;
                cmd.Parameters.Add(new MySqlParameter("@DefectCode", MySqlDbType.Int32)).Value = ms.DefectCode;
                cmd.Parameters.Add(new MySqlParameter("@TimeStamp", MySqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                cmd.Parameters.Add(new MySqlParameter("@BP_ShortCode", MySqlDbType.VarChar)).Value = ms.BP_ShortCode;
                cmd.Parameters.Add(new MySqlParameter("@ExecutionType", MySqlDbType.VarChar)).Value = "M";
                cmd.Parameters.Add(new MySqlParameter("@ExecutionPlanNo", MySqlDbType.Int32)).Value = ExePlan;
                cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = ms.Plant_Id;
                cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = ms.Company_Id;
                cmd.Parameters.Add(new MySqlParameter("@Storage_id", MySqlDbType.Int32)).Value = ms.SoregeID; ;
                cmd.ExecuteNonQuery();
                sql = "update mold_plan_execution set ConformCount = ConformCount + 1 where MoldPlanExecutionID = " + ExePlan;
                cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                cmd.ExecuteNonQuery();
                myTrans.Commit();
                return Ok("OK");
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
        [HttpPost("Unloading")]
        public IActionResult PostUnloading([FromBody] MoldScan ul)
        {
            bool ActiveStatus = false;
            int Quantity = 0, TotalQty = 0;
            long ID = 0;
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null;
            MySqlCommand cmd = null;
            MySqlDataReader rdr = null;
            string sql = string.Empty;
            int PaintPlanID;
            try
            {
                connection.Open();
                if (ul.SoregeID == -1)
                {
                    sql = "select Storage_id from storages where StoreID =  " +
                          "(select Stores_id from stores where Stores_Name = " +
                          "(select Workstation_name from workstation where Workstation_Id =  " +
                          "(select To_Station from defect_code where defect_code_id = " + ul.DefectCode + ") " +
                          ") and Company_Id = '" + ul.Company_Id + "' and Plant_Id = '" + ul.Plant_Id + "' )";
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        if (rdr["Storage_id"] != DBNull.Value)
                            ul.SoregeID = Convert.ToInt32(rdr["Storage_id"]);
                        else
                            return StatusCode(700, "Can't find default location please add from master data");
                    }
                    else
                        return StatusCode(700, "Can't find default location please add from master data");
                    rdr.Close();
                }
                myTrans = connection.BeginTransaction();
                //select single record (part can be repainted)
                sql = "Select PL.Barcode, PL.PaintPlanExecutionID, PL.SkidNumber, PL.Position, " +
                             "PL.LoadUnload, PL.ID, CC.ColorID, CC.ColorcolName, PP.SubModel, SM.Subpart_Name,PP.Round " +
                             "from PaintLoading as PL " +
                             "left join paint_plan_excecution as PE on PL.PaintPlanExecutionID = PE.PaintPlanExecutionID " +
                             "left join paintplanning as PP on PE.PaintPlanID = PP.PaintPlanID " +
                             "left join color as CC on PP.Color = CC.ColorID " +
                             "left join sub_model as SM on PP.SubModel = SM.Subpart_id " +
                             "where PL.Barcode = '" + ul.Barcode + "' order by PL.PaintPlanExecutionID DESC LIMIT 1";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                rdr = cmd.ExecuteReader();
                PaintLoading pl = new PaintLoading();
                if (rdr.Read())
                {
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
                    pl.ColorName = rdr["ColorcolName"].ToString();
                    if (rdr["ColorID"] != DBNull.Value)
                        pl.ColorId = Convert.ToInt32(rdr["ColorID"]);
                    else
                        pl.ColorId = null;
                    pl.Subpart_Name = rdr["Subpart_Name"].ToString();
                    if (rdr["LoadUnload"] != DBNull.Value)
                        pl.LoadUnload = Convert.ToBoolean(rdr["LoadUnload"]);
                    else
                        pl.LoadUnload = false;
                    if (rdr["Round"] != DBNull.Value)
                        pl.Round = Convert.ToInt32(rdr["Round"]);
                    else
                        pl.Round = -1;
                    //****************Check the PaintPlanExecution is Active or not ********************
                    rdr.Close();
                    sql = "Select EstimatedQuantity,Status,PaintPlanID from paint_plan_excecution where PaintPlanExecutionID = " + pl.PaintPlanExecutionID;
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        ActiveStatus = Convert.ToBoolean(rdr["Status"]);
                        Quantity = Convert.ToInt32(rdr["EstimatedQuantity"]);
                        PaintPlanID = Convert.ToInt32(rdr["PaintPlanID"]);
                    }
                    else
                        return StatusCode(700, "Unable to read Paint Ececutin Plan");
                    rdr.Close();
                    //*********** Now update in Barcode Master Table ********************************
                    sql = "select CurrentDefectCode from barcodemaster  where Barcode = '" + ul.Barcode + "';";
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    int CurrentCode = 0;
                    if (rdr.Read())
                    {
                        CurrentCode = Convert.ToInt32(rdr["CurrentDefectCode"]);
                    }
                    else
                        return StatusCode(700, "Invalid Barcode");
                    rdr.Close();
                    if (ul.OkCode == CurrentCode)
                        sql = "update barcodemaster set CurrentDefectCode = @DefectCode, Storage_id = @StorageID where Barcode = '" + ul.Barcode + "'";
                    else
                        sql = "update barcodemaster set CurrentDefectCode = @DefectCode, Storage_id = @StorageID, PRFT = (PRFT + 1) where Barcode = '" + ul.Barcode + "'";
                    cmd = cmd = new MySqlCommand(sql, connection)
                    { CommandType = CommandType.Text };
                    cmd.Parameters.Add(new MySqlParameter("@DefectCode", MySqlDbType.Int32)).Value = ul.DefectCode;
                    cmd.Parameters.Add(new MySqlParameter("@StorageID", MySqlDbType.Int32)).Value = ul.SoregeID;
                    cmd.ExecuteNonQuery();
                    //*********** Now update in Barcode Details Table ********************************
                    sql = "insert into barcodedetails (Barcode,Workstation_Id,DefectCode,TimeStamp,BP_ShortCode,ExecutionType,ExecutionPlanNo,Plant_Id,Company_Id,Storage_id) values (@Barcode,@Workstation_Id,@DefectCode,@TimeStamp,@BP_ShortCode,@ExecutionType,@ExecutionPlanNo,@Plant_Id,@Company_Id,@Storage_id)";
                    cmd = new MySqlCommand(sql, connection)
                    { CommandType = CommandType.Text };
                    cmd.Parameters.Add(new MySqlParameter("@Barcode", MySqlDbType.VarChar)).Value = ul.Barcode;
                    cmd.Parameters.Add(new MySqlParameter("@Workstation_Id", MySqlDbType.VarChar)).Value = ul.Workstation_Id;
                    cmd.Parameters.Add(new MySqlParameter("@DefectCode", MySqlDbType.Int32)).Value = ul.DefectCode;
                    cmd.Parameters.Add(new MySqlParameter("@TimeStamp", MySqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    cmd.Parameters.Add(new MySqlParameter("@BP_ShortCode", MySqlDbType.VarChar)).Value = ul.BP_ShortCode;
                    cmd.Parameters.Add(new MySqlParameter("@ExecutionType", MySqlDbType.VarChar)).Value = "U";
                    cmd.Parameters.Add(new MySqlParameter("@ExecutionPlanNo", MySqlDbType.Int32)).Value = DBNull.Value;     //pl.PaintPlanExecutionID;
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = ul.Plant_Id;
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = ul.Company_Id;
                    cmd.Parameters.Add(new MySqlParameter("@Storage_id", MySqlDbType.Int32)).Value = ul.SoregeID;
                    cmd.ExecuteScalar();
                    ID = cmd.LastInsertedId;


                    if (ActiveStatus && !pl.LoadUnload)
                    {
                        sql = "Select Sum(LoadUnload) as TotalQty from PaintLoading where PaintPlanExecutionID = " + pl.PaintPlanExecutionID;
                        cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                        rdr = cmd.ExecuteReader();
                        if (rdr.Read())
                            TotalQty = Convert.ToInt32(rdr["TotalQty"]);
                        else
                            return StatusCode(700, "Can't find Quantity");
                        rdr.Close();
                        //get paint plan quantity
                        if (Quantity == (TotalQty + 1))
                        {
                            //Close the Paint Plan Execution
                            sql = "update paint_plan_excecution set Status = 0 where PaintPlanExecutionID = " + pl.PaintPlanExecutionID;
                            cmd = cmd = new MySqlCommand(sql, connection)
                            { CommandType = CommandType.Text };
                            cmd.ExecuteNonQuery();

                        }
                        //Unloaded so update LoadUnload (1 means unload)
                        sql = "update paintloading set LoadUnload = 1, PaintPlanExecutionID = " + pl.PaintPlanExecutionID + ",UnloadingDetails = " + ID + " where ID = '" + pl.ID + "'";
                        cmd = cmd = new MySqlCommand(sql, connection)
                        { CommandType = CommandType.Text };
                        cmd.ExecuteNonQuery();
                        //*************************************************************************
                        cmd = new MySqlCommand("IsSkidUnloaded", connection)
                        {
                            CommandType = CommandType.StoredProcedure,
                        };
                        cmd.Parameters.Add("?RID", MySqlDbType.String).Value = pl.ID;
                        cmd.Parameters.Add("?StartSkidNo", MySqlDbType.Int32).Value = ul.StartSkidNo;
                        cmd.Parameters.Add(new MySqlParameter("mRes", MySqlDbType.Int32, 0));
                        cmd.Parameters["mRes"].Direction = ParameterDirection.ReturnValue;
                        cmd.ExecuteNonQuery();
                        int flag = Convert.ToInt32(cmd.Parameters["mRes"].Value);
                        if (flag == 1)
                        {
                            // Update the Round
                            sql = "update general_settings set RoundNo = (RoundNo + 1) where Company_ID = '" + ul.Company_Id + "' and Plant_ID = '" + ul.Plant_Id + "'";
                            cmd = cmd = new MySqlCommand(sql, connection)
                            { CommandType = CommandType.Text };
                            cmd.ExecuteNonQuery();
                        }
                    }
                    //******************Msaster & Details shifted above**************************

                }
                else
                    return StatusCode(700, "No such Barcode in Painting");
                myTrans.Commit();
                return Ok(pl);
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
        //************************************************************************************
        [HttpPost("Assembly")]
        public IActionResult PostAssembly([FromBody] AssemblyScan ms)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null;
            string sql;
            MySqlCommand cmd = null;
            MySqlDataReader rdr = null;
            try
            {
                connection.Open();
                if (ms.SoregeID == -1)
                {
                    sql = "select Storage_id from storages where StoreID =  " +
                          "(select Stores_id from stores where Stores_Name = " +
                          "(select Workstation_name from workstation where Workstation_Id =  " +
                          "(select To_Station from defect_code where defect_code_id = " + ms.DefectCode + ") " +
                          ") and Company_Id = '" + ms.Company_Id + "' and Plant_Id = '" + ms.Plant_Id + "' )";
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        if (rdr["Storage_id"] != DBNull.Value)
                            ms.SoregeID = Convert.ToInt32(rdr["Storage_id"]);
                        else
                            return StatusCode(700, "Can't find default location please add from master data");
                    }
                    else
                        return StatusCode(700, "Can't find default location please add from master data");
                    rdr.Close();
                }
                myTrans = connection.BeginTransaction();
                //Check current barcode's defectcode is for assembly. 
                bool flag = false;

                sql = "select CurrentDefectCode from barcodemaster where Barcode = '" + ms.Barcode + "'";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                rdr = cmd.ExecuteReader();
                int CurrentCode = 0;
                if (rdr.Read())
                    CurrentCode = Convert.ToInt32(rdr["CurrentDefectCode"]);
                else
                    return StatusCode(700, "Invalid Barcode");
                rdr.Close();
                sql = "select OkDefectCode from scanstation where ScanstationName = 'Mold Scan' or ScanstationName = 'Unloading Scan' or ScanstationName = 'Assembly Scan'; ";
                 cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                 rdr = cmd.ExecuteReader();
                int MCode = 0;
                
                while (rdr.Read())
                {
                    MCode = Convert.ToInt32(rdr["OkDefectCode"]);
                    if (MCode == CurrentCode)
                    {
                        flag = true;
                        break;
                    }
                }
                rdr.Close();
                if (!flag)
                    return StatusCode(700, "Invalid Barcode");
                
                               
                //********************************************************************
                //Get ERP Number
                sql = "select Subpart_id, ColorID from erpnumber where ERPNumberID = ( select ERPNumberID from barcodemaster where Barcode = '" + ms.Barcode + "')";
                cmd = cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                rdr = cmd.ExecuteReader();
                int SubModel;
                int? ColorID;
                if (rdr.Read())
                {
                    SubModel = Convert.ToInt32(rdr["Subpart_id"]);
                    if (rdr["ColorID"] == DBNull.Value)
                        ColorID = null;
                    else
                        ColorID = Convert.ToInt32(rdr["ColorID"]);
                }
                else
                    return StatusCode(700, "Can't find barcode");
                rdr.Close();
                if (ColorID == null)
                    sql = "select ERPNumberID from erpnumber where Subpart_id = " + SubModel + " and ColorID is null and AssemblyID = " + ms.AssemblyID + " and Plant_Id = '" + ms.Plant_Id + "' and Company_Id = '" + ms.Company_Id + "'";
                else
                    sql = "select ERPNumberID from erpnumber where Subpart_id = " + SubModel + " and ColorID = " + ColorID + " and AssemblyID = " + ms.AssemblyID + " and Plant_Id = '" + ms.Plant_Id + "' and Company_Id = '" + ms.Company_Id + "'";
                cmd = cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                rdr = cmd.ExecuteReader();
                int ERPNo;
                if (rdr.Read())
                    ERPNo = Convert.ToInt32(rdr["ERPNumberID"]);
                else
                    return StatusCode(700, "Can't find ERP number");
                rdr.Close();
                //Update Barcode Master
                
                if(ms.OkCode == CurrentCode)
                    sql = "update barcodemaster set ERPNumberID = @ERPNo, CurrentDefectCode = @DefectCode, Storage_id = @StorageID where Barcode = '" + ms.Barcode + "'";
                else
                    sql = "update barcodemaster set ERPNumberID = @ERPNo, CurrentDefectCode = @DefectCode, Storage_id = @StorageID, ARFT = (ARFT + 1) where Barcode = '" + ms.Barcode + "'";
                cmd = cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                cmd.Parameters.Add(new MySqlParameter("@DefectCode", MySqlDbType.Int32)).Value = ms.DefectCode;
                cmd.Parameters.Add(new MySqlParameter("@StorageID", MySqlDbType.Int32)).Value = ms.SoregeID;
                cmd.Parameters.Add(new MySqlParameter("@ERPNo", MySqlDbType.VarChar)).Value = ERPNo;
                cmd.ExecuteNonQuery();
                
                //*********** Now update in Barcode Details Table ********************************
                sql = "insert into barcodedetails (Barcode,Workstation_Id,DefectCode,TimeStamp,BP_ShortCode,ExecutionType,ExecutionPlanNo,Plant_Id,Company_Id,Storage_id) values (@Barcode,@Workstation_Id,@DefectCode,@TimeStamp,@BP_ShortCode,@ExecutionType,@ExecutionPlanNo,@Plant_Id,@Company_Id,@Storage_id)";
                cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                cmd.Parameters.Add(new MySqlParameter("@Barcode", MySqlDbType.VarChar)).Value = ms.Barcode;
                cmd.Parameters.Add(new MySqlParameter("@Workstation_Id", MySqlDbType.VarChar)).Value = ms.Workstation_Id;
                cmd.Parameters.Add(new MySqlParameter("@DefectCode", MySqlDbType.Int32)).Value = ms.DefectCode;
                cmd.Parameters.Add(new MySqlParameter("@TimeStamp", MySqlDbType.VarChar)).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                cmd.Parameters.Add(new MySqlParameter("@BP_ShortCode", MySqlDbType.VarChar)).Value = ms.BP_ShortCode;
                cmd.Parameters.Add(new MySqlParameter("@ExecutionType", MySqlDbType.VarChar)).Value = "A";
                cmd.Parameters.Add(new MySqlParameter("@ExecutionPlanNo", MySqlDbType.Int32)).Value = DBNull.Value;
                cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = ms.Plant_Id;
                cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = ms.Company_Id;
                cmd.Parameters.Add(new MySqlParameter("@Storage_id", MySqlDbType.Int32)).Value = ms.SoregeID;
                cmd.ExecuteNonQuery();
                //***********************************************************************************************
                //                             Print barcode
                //***********************************************************************************************
                if (ms.DefectCode == ms.OkCode)
                {
                    //geting part, color and assembly names

                    if(ColorID != null)
                    sql = "select S.Subpart_Name, S.Customer_material_number, C.ColorcolName, A.AssemblyName from sub_model as S, color as C, assembly as A " +
                      "where S.Subpart_id = " + SubModel + " and C.ColorID = " + ColorID + " and A.AssemblyID = " + ms.AssemblyID;
                    else
                        sql = "select S.Subpart_Name, S.Customer_material_number, C.ColorcolName, A.AssemblyName from sub_model as S, color as C, assembly as A " +
                      "where S.Subpart_id = " + SubModel + " and A.AssemblyID = " + ms.AssemblyID;
                    cmd = cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    string PartName = string.Empty;
                    string ColorName = string.Empty;
                    string AssemblyName = string.Empty;
                    string CustMaterial = string.Empty;
                    if (rdr.Read())
                    {
                        PartName = rdr["Subpart_Name"].ToString();
                        ColorName = rdr["ColorcolName"].ToString();
                        AssemblyName = rdr["AssemblyName"].ToString();
                        CustMaterial = rdr["Customer_material_number"].ToString();
                    }
                    else
                        return StatusCode(700, "Unabe to find part, colour and assembly name");
                    rdr.Close();
                    List<string> BCodes = new List<string>();
                    BCodes.Add(ms.Barcode);
                    BCodes.Add(ms.Barcode);
                    sql = "Select * from BarCodeSettings where Plant_Id = '" + ms.Plant_Id + "' and Company_Id = '" + ms.Company_Id + "'";
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    BarCodeSettings bs = new BarCodeSettings();
                    if (rdr.Read())
                    {
                        bs.PrinterType = Convert.ToBoolean(rdr["PrinterType"]);
                        if (bs.PrinterType)
                        {
                            bs.PrinterDPI = Convert.ToInt32(rdr["PrinterDPI"]);
                            bs.StickerWidth = float.Parse(rdr["StickerWidth"].ToString());
                            bs.StickerHeight = float.Parse(rdr["StickerHeight"].ToString());
                            bs.BarOrQR = Convert.ToBoolean(rdr["BarOrQR"]);
                        }
                        else
                        {
                            bs.PageOriantation = Convert.ToBoolean(rdr["PageOriantation"]);
                            if (bs.PageOriantation)
                            {
                                bs.PageWidth = float.Parse(rdr["PageWidth"].ToString()) * 28.3465f;
                                bs.PageHeight = float.Parse(rdr["PageHeight"].ToString()) * 28.3465f;
                                bs.StickerWidth = float.Parse(rdr["StickerWidth"].ToString()) * 28.3465f;
                                bs.StickerHeight = float.Parse(rdr["StickerHeight"].ToString()) * 28.3465f;
                                bs.Rows = Convert.ToInt32(rdr["Rowes"]);
                                bs.Columns = Convert.ToInt32(rdr["Columns"]);
                                bs.CellPadingLeft = float.Parse(rdr["CellPadingLeft"].ToString()) * 28.3465f;
                                bs.CellPadingTop = float.Parse(rdr["CellPadingTop"].ToString()) * 28.3465f;
                                bs.CellPadingRight = float.Parse(rdr["CellPadingRight"].ToString()) * 28.3465f;
                                bs.CellPadingBottom = float.Parse(rdr["CellPadingBottom"].ToString()) * 28.3465f;
                                bs.PageMarginLeft = float.Parse(rdr["PageMarginLeft"].ToString()) * 28.3465f;
                                bs.PageMarginTop = float.Parse(rdr["PageMarginTop"].ToString()) * 28.3465f;
                                bs.PageMarginRight = float.Parse(rdr["PageMarginRight"].ToString()) * 28.3465f;
                                bs.PageMarginBottom = float.Parse(rdr["PageMarginBottom"].ToString()) * 28.3465f;
                                bs.AcrossThenDown = Convert.ToBoolean(rdr["AcrossThenDown"]);
                                bs.VerticalGap = float.Parse(rdr["VerticalGap"].ToString()) * 28.3465f;
                                bs.HorizantalGap = float.Parse(rdr["HorizantalGap"].ToString()) * 28.3465f;
                                bs.Plant_Id = rdr["Plant_Id"].ToString();
                                bs.Company_Id = rdr["Company_Id"].ToString();
                                bs.BarOrQR = Convert.ToBoolean(rdr["BarOrQR"]);
                            }
                            else
                            {
                                bs.PageHeight = float.Parse(rdr["PageWidth"].ToString()) * 28.3465f;
                                bs.PageWidth = float.Parse(rdr["PageHeight"].ToString()) * 28.3465f;
                                bs.StickerHeight = float.Parse(rdr["StickerWidth"].ToString()) * 28.3465f;
                                bs.StickerWidth = float.Parse(rdr["StickerHeight"].ToString()) * 28.3465f;
                                bs.Columns = Convert.ToInt32(rdr["Rows"]);
                                bs.Rows = Convert.ToInt32(rdr["Columns"]);
                                bs.CellPadingBottom = float.Parse(rdr["CellPadingLeft"].ToString()) * 28.3465f;
                                bs.CellPadingLeft = float.Parse(rdr["CellPadingTop"].ToString()) * 28.3465f;
                                bs.CellPadingTop = float.Parse(rdr["CellPadingRight"].ToString()) * 28.3465f;
                                bs.CellPadingRight = float.Parse(rdr["CellPadingBottom"].ToString()) * 28.3465f;
                                bs.PageMarginBottom = float.Parse(rdr["PageMarginLeft"].ToString()) * 28.3465f;
                                bs.PageMarginLeft = float.Parse(rdr["PageMarginTop"].ToString()) * 28.3465f;
                                bs.PageMarginTop = float.Parse(rdr["PageMarginRight"].ToString()) * 28.3465f;
                                bs.PageMarginRight = float.Parse(rdr["PageMarginBottom"].ToString()) * 28.3465f;
                                bs.AcrossThenDown = Convert.ToBoolean(rdr["AcrossThenDown"]);
                                bs.HorizantalGap = float.Parse(rdr["VerticalGap"].ToString()) * 28.3465f;
                                bs.VerticalGap = float.Parse(rdr["HorizantalGap"].ToString()) * 28.3465f;
                                bs.Plant_Id = rdr["Plant_Id"].ToString();
                                bs.Company_Id = rdr["Company_Id"].ToString();
                            }
                        }
                    }
                    else
                        return StatusCode(700, "Unable to read Barcode Settings");
                    rdr.Close();
                    if (!bs.PrinterType)
                    {
                        int totalclos = bs.Columns + bs.Columns - 1;
                        List<float> Gap = new List<float>();
                        for (int i = 0; i < totalclos; i++)
                            if (i % 2 == 0) Gap.Add(bs.StickerWidth); else Gap.Add(bs.VerticalGap);
                        float totalwidth = bs.Columns * bs.StickerWidth + (bs.Columns - 1) * bs.HorizantalGap;
                        float[] gaps = Gap.ToArray();
                        int item = 0;
                        Document doc;
                        iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(bs.PageWidth, bs.PageHeight);
                        using (MemoryStream mst = new MemoryStream())
                        {
                            doc = new Document(rect, bs.PageMarginLeft, bs.PageMarginRight, bs.PageMarginTop, bs.PageMarginBottom);
                            PdfWriter.GetInstance(doc, mst);
                            doc.Open();
                            double Pages = Math.Ceiling((double)((double)BCodes.Count / (double)(bs.Rows * bs.Columns)));
                            for (int i = 0; i < Pages; i++)
                            {
                                if (i > 0)
                                    doc.NewPage();
                                PdfPTable table = new PdfPTable(totalclos)
                                {
                                    //table.WidthPercentage = 100f;
                                    TotalWidth = totalwidth,
                                    LockedWidth = true,
                                    HorizontalAlignment = Element.ALIGN_LEFT

                                };
                                table.SetWidths(gaps);
                                for (int r = 0; r < bs.Rows; r++)
                                {
                                    PdfPCell[] cells = new PdfPCell[totalclos];
                                    for (int c = 0, k = 0; c < totalclos; c++)
                                    {
                                        if (c % 2 == 0)
                                        {
                                            if (bs.AcrossThenDown)
                                                item = r * bs.Columns + k + (i * bs.Rows * bs.Columns);
                                            else
                                                item = c * bs.Rows + r + (i * bs.Rows * bs.Columns);
                                            k++;
                                            iTextSharp.text.Image image128 = null;
                                            if (item < BCodes.Count)
                                            {
                                                if (bs.PageOriantation)
                                                {
                                                    image128 = iTextSharp.text.Image.GetInstance(GetImage(BCodes[item], bs.BarOrQR, bs.StickerWidth, bs.StickerHeight, PartName, ColorName, AssemblyName, CustMaterial), System.Drawing.Imaging.ImageFormat.Bmp);
                                                }
                                                else
                                                {
                                                    System.Drawing.Bitmap img = (Bitmap)GetImage(BCodes[item], bs.BarOrQR, bs.StickerWidth, bs.StickerHeight, PartName, ColorName, AssemblyName, CustMaterial);
                                                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                                    image128 = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Bmp);
                                                }

                                                cells[c] = new PdfPCell(image128, true);
                                            }
                                            else
                                                cells[c] = new PdfPCell();
                                            cells[c].PaddingTop = bs.CellPadingTop;
                                            cells[c].PaddingRight = bs.CellPadingRight;
                                            cells[c].PaddingBottom = bs.CellPadingBottom;
                                            cells[c].PaddingLeft = bs.CellPadingLeft;
                                            cells[c].VerticalAlignment = Element.ALIGN_MIDDLE;
                                            cells[c].FixedHeight = bs.StickerHeight;
                                            table.AddCell(cells[c]);

                                        }
                                        else
                                        {
                                            cells[c] = new PdfPCell()
                                            { FixedHeight = bs.StickerHeight };
                                            table.AddCell(cells[c]);
                                        }
                                    }
                                    table.CompleteRow();
                                    if (bs.Rows > 1 && r < bs.Rows - 1)
                                    {
                                        PdfPCell[] gcells = new PdfPCell[totalclos];
                                        for (int c = 0; c < totalclos; c++)
                                        {
                                            gcells[c] = new PdfPCell()
                                            { FixedHeight = bs.VerticalGap };
                                            table.AddCell(gcells[c]);
                                        }
                                        table.CompleteRow();
                                    }
                                }
                                doc.Add(table);
                            }
                            doc.Close();
                            myTrans.Commit();
                            return File(mst.ToArray(), "APPLICATION/octet-stream", "Barcode.pdf");
                        }
                    }
                    else
                    {
                        string cmds = string.Empty;
                        int? DPI = bs.PrinterDPI;
                        int W = (int)((bs.StickerWidth / 2.54f) * DPI);
                        int H = (int)((bs.StickerHeight / 2.54f) * DPI);
                        //int magnification = 5;
                        //if (DPI <= 150) magnification = 1;
                        //else if (DPI <= 200) magnification = 2;
                        //else if(DPI <= 300) magnification = 3;
                        //else if(DPI <= 600 ) magnification = 4;
                        foreach (string pb in BCodes)
                        {
                            cmds += @"^XA";
                            cmds += @"^LL" + H + "^PW" + W + "^FS";
                            cmds += @"^CF0,15^FS";
                            cmds += @"^FO10,15^FB" + W + @",1,0,L^FH\^FDModel:" + PartName + "^FS";
                            cmds += @"^FO10,15^FB" + (W - 20) + @",1,0,R^FH\^FDType:" + AssemblyName + "^FS";
                            cmds += @"^FO10,32^FB" + W + @",1,0,L^FH\^FDColour:" + ColorName + "^FS";
                            cmds += @"^FO10,32^FB" + (W - 20) + @",1,0,R^FH\^FDPrint Date:" + DateTime.Now.ToString("dd/MM/yyyy") + "^FS";
                            cmds += @"^FO10,49^FB" + W + @",1,0,L^FH\^FDCustomer Material No.:" + CustMaterial + "^FS";
                            cmds += @"^FO10,66^GB" + (W - 20) + @",120,1,B,3^FS";
                            bool newBool = bs.BarOrQR.HasValue ? bs.BarOrQR.Value : false;
                            if (newBool)
                                cmds += @"^FO60,80^BCN,70^FD" + pb + "^FS";
                            else
                                cmds += @"^FO210,66^BQN,2,5^FDQA," + pb + "^FS";
                            cmds += @"^XZ\n";
                        }
                        myTrans.Commit();
                        return Ok(cmds);
                    }
                }
                myTrans.Commit();
                return Ok("Assembly attached");
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

        private System.Drawing.Image GetImage(string v, bool? barOrQR, float stickerWidth, float stickerHeight, string partName, string colorName, string assemblyName, string custMaterial)
        {
            string date = "Print date:" + DateTime.Now.ToString("dd-MMM-yyyy");
            float pixelsPerPoint = 1.333333f;

            int width1 = (int)(stickerWidth * pixelsPerPoint);
            int height1 = (int)(stickerHeight * pixelsPerPoint);

            int width = width1 - 6; // width of the Qr Code
            int height = height1 - 50; // height of the Qr Code
            int margin = 0;
            ZXing.BarcodeFormat iFormat;
            bool newBool = barOrQR.HasValue ? barOrQR.Value : false;
            if (newBool)
                iFormat = ZXing.BarcodeFormat.CODE_128;
            else
                iFormat = ZXing.BarcodeFormat.QR_CODE;

            BarcodeWriter qrCodeWriter = new BarcodeWriter
            {

                Format = iFormat,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin,
                    PureBarcode = false
                }

            };


            Bitmap bitmap = new Bitmap(width1, height1);
            Graphics g = Graphics.FromImage((System.Drawing.Image)bitmap);
            g.FillRectangle(Brushes.White, 0f, 0f, bitmap.Width, bitmap.Height);  // fill the entire bitmap with a red rectangle  

            g.DrawRectangle(new Pen(System.Drawing.Color.Black, 3), new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height));

            //Top Left
            g.DrawString(partName, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point(4, 4));
            //Right Top
            g.DrawString(assemblyName, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Bold), Brushes.Black, new Point(bitmap.Width - ((int)(4.1951219512195121951219512195122f * assemblyName.Length + 14)), 4));
            //Left Top 2
            g.DrawString(colorName, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point(4, 12));
            //Right Top 2
            g.DrawString(date, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point(bitmap.Width - ((int)(4.1951219512195121951219512195122f * date.Length + 14)), 12));
            //Right Top 3
            g.DrawString(custMaterial, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point(3, 20));


            g.DrawImage((System.Drawing.Image)qrCodeWriter.Write(v).Clone(), new System.Drawing.Rectangle((width1 - width) / 2, (height1 - height) / 2 + 2, width, height), new System.Drawing.Rectangle(0, 0, width, height), GraphicsUnit.Pixel);

            return (System.Drawing.Image)(bitmap);
        }
    }
}
