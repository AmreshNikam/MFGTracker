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
    public class DispatchController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public DispatchController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        [HttpGet("ERPStock/{COMPANYID}/{PLANTID}")]
        public IActionResult Get(string COMPANYID, string PLANTID, [FromQuery] int SubPart)
        {
            List<Dispatch> dispatches = new List<Dispatch>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "select ER.ERPNumberID, ER.Subpart_id, SM.Subpart_Name, ER.ColorID, CO.ColorcolName, " +
                             "ER.AssemblyID, AM.AssemblyName, (ifnull(ST.InStock,0) - ifnull(IP.InPlan,0)) as InStock " +
                             "from erpnumber as ER " +
                             "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id " +
                             "left join color as CO on ER.ColorID = CO.ColorID " +
                             "left join assembly as AM on ER.AssemblyID = AM.AssemblyID " +
                             "left join (select count(*) as InStock, ERPNumberID  from barcodemaster where Disatched = 0 and CurrentDefectCode in " +
                             "(select OkDefectCode from  scanstation where ScanstationName in ('Mold Scan','Unloading Scan','Assembly Scan')) " +
                             "group by ERPNumberID) as ST on ER.ERPNumberID = ST.ERPNumberID " +
                             "left join (SELECT sum(Quantity) as InPlan, ERPNo FROM dispatchitems where DispatchPlanID in (select DispatchPlanID from dispatchplan where Status = 1) group by ERPNo) as IP " +
                             "on  ER.ERPNumberID = IP.ERPNo " +
                             "where  ER.Subpart_id = " + SubPart + " and ER.Plant_Id = '" + PLANTID + "' and ER.Company_Id = '" + COMPANYID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Dispatch dispatch = new Dispatch();
                    if (rdr["ERPNumberID"] != DBNull.Value)
                        dispatch.ERPNumberID = Convert.ToInt32(rdr["ERPNumberID"]);
                    else
                        dispatch.ERPNumberID = null;
                    if (rdr["Subpart_id"] != DBNull.Value)
                        dispatch.Subpart_id = Convert.ToInt32(rdr["Subpart_id"]);
                    else
                        dispatch.Subpart_id = null;
                    dispatch.Subpart_Name = rdr["Subpart_Name"].ToString();
                    if (rdr["ColorID"] != DBNull.Value)
                        dispatch.ColorID = Convert.ToInt32(rdr["ColorID"]);
                    else
                        dispatch.ColorID = null;
                    dispatch.ColorcolName = rdr["ColorcolName"].ToString();
                    if (rdr["AssemblyID"] != DBNull.Value)
                        dispatch.AssemblyID = Convert.ToInt32(rdr["AssemblyID"]);
                    else
                        dispatch.AssemblyID = null;
                    dispatch.AssemblyName = rdr["AssemblyName"].ToString();
                    if (rdr["InStock"] != DBNull.Value)
                        dispatch.InStock = Convert.ToInt32(rdr["InStock"]);
                    else
                        dispatch.InStock = null;

                    dispatches.Add(dispatch);
                }
                return Ok(dispatches);
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

        [HttpGet("{COMPANYID}/{PLANTID}")]
        public IActionResult GetDispatchlanDetails(string COMPANYID, string PLANTID, [FromQuery] bool Status)
        {
            List<DispatchDetailsArray> dispatches = new List<DispatchDetailsArray>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "SELECT DP.DispatchPlanID, DP.PlanEnteryDate, DP.DateOfPlan, DP.AllowNextScan,DI.DispatchItemsID, DI.Quantity,DI.ERPNo, " +
                             "SM.Part_id, SM.Subpart_Id, SM.Subpart_Name, CO.ColorID, CO.ColorcolName, AM.AssemblyID, AM.AssemblyName " +
                             "from dispatchplan as DP " +
                             "left join dispatchitems as DI on DP.DispatchPlanID = DI.DispatchPlanID " +
                             "left join erpnumber as ER on DI.ERPNo = ER.ERPNumberID " +
                             "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id " +
                             "left join color as CO on ER.ColorID = CO.ColorID " +
                             "left join assembly as AM on ER.AssemblyID = AM.AssemblyID " +
                             "where DP.Status = " + Convert.ToInt16(Status) + " and DP.Plant_Id = '" + PLANTID + "' and DP.Company_Id = '" + COMPANYID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DispatchDetailsArray dispatch = new DispatchDetailsArray();
                    if (rdr["DispatchPlanID"] != DBNull.Value)
                        dispatch.DispatchPlanID = Convert.ToInt32(rdr["DispatchPlanID"]);
                    else
                        dispatch.DispatchPlanID = null;
                    if (rdr["PlanEnteryDate"] != DBNull.Value)
                        dispatch.PlanEnteryDate = Convert.ToDateTime(rdr["PlanEnteryDate"]);
                    else
                        dispatch.PlanEnteryDate = null;
                    if (rdr["DateOfPlan"] != DBNull.Value)
                        dispatch.DateOfPlan = Convert.ToDateTime(rdr["DateOfPlan"]);
                    else
                        dispatch.DateOfPlan = null;
                    if (rdr["AllowNextScan"] != DBNull.Value)
                        dispatch.AllowNextScan = Convert.ToInt32(rdr["AllowNextScan"]);
                    else
                        dispatch.AllowNextScan = null;
                    if (rdr["DispatchItemsID"] != DBNull.Value)
                        dispatch.DispatchItemsID = Convert.ToInt32(rdr["DispatchItemsID"]);
                    else
                        dispatch.DispatchItemsID = null;
                    if (rdr["Quantity"] != DBNull.Value)
                        dispatch.Quantity = Convert.ToInt32(rdr["Quantity"]);
                    else
                        dispatch.Quantity = null;
                    if (rdr["ERPNo"] != DBNull.Value)
                        dispatch.ERPNo = Convert.ToInt32(rdr["ERPNo"]);
                    else
                        dispatch.ERPNo = null;
                    if (rdr["Part_id"] != DBNull.Value)
                        dispatch.Part_id = Convert.ToInt32(rdr["Part_id"]);
                    else
                        dispatch.Part_id = null;
                    if (rdr["Subpart_Id"] != DBNull.Value)
                        dispatch.Subpart_Id = Convert.ToInt32(rdr["Subpart_Id"]);
                    else
                        dispatch.Subpart_Id = null;
                    dispatch.Subpart_Name = rdr["Subpart_Name"].ToString();
                    if (rdr["ColorID"] != DBNull.Value)
                        dispatch.ColorID = Convert.ToInt32(rdr["ColorID"]);
                    else
                        dispatch.ColorID = null;
                    dispatch.ColorcolName = rdr["ColorcolName"].ToString();
                    if (rdr["AssemblyID"] != DBNull.Value)
                        dispatch.AssemblyID = Convert.ToInt32(rdr["AssemblyID"]);
                    else
                        dispatch.AssemblyID = null;
                    dispatch.AssemblyName = rdr["AssemblyName"].ToString();
                    dispatches.Add(dispatch);
                }
                //List<DispatchDetails> dispatchDetails = new List<DispatchDetails>();
                //var groupedList = dispatches.GroupBy(u => new { u.DisatchPlanID, u.PlanEnteryDate,u.DateOfPlan,u.AllowNextScan });
                //foreach (var group in groupedList)
                //{
                //    DispatchDetails dp = new DispatchDetails();
                //    dp.DisatchPlanID = group.Key.DisatchPlanID;
                //    dp.PlanEnteryDate = group.Key.PlanEnteryDate;
                //    dp.DateOfPlan = group.Key.DateOfPlan;
                //    dp.AllowNextScan = group.Key.AllowNextScan;
                //    List<DispatchItmsDetails> dispatchItmsDetails = new List<DispatchItmsDetails>();
                //    foreach (var info in group)
                //    {
                //        DispatchItmsDetails di = new DispatchItmsDetails();
                //        di.DispatchItemsID = info.DispatchItemsID;
                //        di.Quantity = info.Quantity;
                //        di.ERPNo = info.ERPNo;
                //        di.AssemblyID = info.AssemblyID;
                //        di.AssemblyName = info.AssemblyName;
                //        di.ColorID = info.ColorID;
                //        di.ColorcolName = info.ColorcolName;
                //        di.Subpart_Id = info.Subpart_Id;
                //        di.Subpart_Name = info.Subpart_Name;
                //        dispatchItmsDetails.Add(di);
                //    }
                //    dp.dispatchItmsDetails = dispatchItmsDetails;
                //    dispatchDetails.Add(dp);
                //}
                return Ok(dispatches);
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

        // POST api/<DispatchController>
        [HttpPost]
        public IActionResult Post([FromBody] DispatchPlan dispatch)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null;

            try
            {
                connection.Open();
                myTrans = connection.BeginTransaction();
                //**********************************************************************************************************
                //               insert into disatchplan table
                //**********************************************************************************************************
                string sql = "Insert into dispatchplan (DateOfPlan, AllowNextScan, BPShortCode, Plant_Id, Company_Id,PlanEnteryDate) values(@DateOfPlan, @AllowNextScan, @BPShortCode, @Plant_Id, @Company_Id,@PlanEnteryDate)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (dispatch.DateOfPlan == null)
                    cmd.Parameters.Add(new MySqlParameter("@DateOfPlan", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@DateOfPlan", MySqlDbType.Date)).Value = dispatch.DateOfPlan;
                if (dispatch.AllowNextScan == null)
                    cmd.Parameters.Add(new MySqlParameter("@AllowNextScan", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@AllowNextScan", MySqlDbType.Int32)).Value = dispatch.AllowNextScan;
                if (dispatch.BPShortCode == null)
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = dispatch.BPShortCode;
                if (dispatch.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = dispatch.Plant_Id;
                if (dispatch.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.Blob)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.Blob)).Value = dispatch.Company_Id;
                if (dispatch.PlanEnteryDate == null)
                    cmd.Parameters.Add(new MySqlParameter("@PlanEnteryDate", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PlanEnteryDate", MySqlDbType.Date)).Value = dispatch.PlanEnteryDate;
                cmd.ExecuteNonQuery();
                long ID = cmd.LastInsertedId;
                //**********************************************************************************************************
                //               insert into dispatchitem table
                //**********************************************************************************************************
                foreach (DispatchItems di in dispatch.dispatchItems)
                {
                    sql = "Insert into dispatchitems (DispatchPlanID, ERPNo, Quantity) values(@DispatchPlanID, @ERPNo, @Quantity)";
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    cmd.Parameters.Add(new MySqlParameter("@DispatchPlanID", MySqlDbType.Int32)).Value = ID;
                    if (di.ERPNo == null)
                        cmd.Parameters.Add(new MySqlParameter("@ERPNo", MySqlDbType.Int32)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@ERPNo", MySqlDbType.Int32)).Value = di.ERPNo;
                    if (di.Quantity == null)
                        cmd.Parameters.Add(new MySqlParameter("@Quantity", MySqlDbType.Int32)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@Quantity", MySqlDbType.Int32)).Value = di.Quantity;
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

        // PUT api/<DispatchController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] DispatchPlan dispatch)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null;

            try
            {
                connection.Open();
                myTrans = connection.BeginTransaction();
                //**********************************************************************************************************
                //               update disatchplan table
                //**********************************************************************************************************
                string sql = "Update dispatchplan set DateOfPlan = @DateOfPlan, AllowNextScan = @AllowNextScan, BPShortCode = @BPShortCode, Plant_Id = @Plant_Id, Company_Id = @Company_Id where DispatchPlanID = " + id;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (dispatch.DateOfPlan == null)
                    cmd.Parameters.Add(new MySqlParameter("@DateOfPlan", MySqlDbType.Date)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@DateOfPlan", MySqlDbType.Date)).Value = dispatch.DateOfPlan;
                if (dispatch.AllowNextScan == null)
                    cmd.Parameters.Add(new MySqlParameter("@AllowNextScan", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@AllowNextScan", MySqlDbType.Int32)).Value = dispatch.AllowNextScan;
                if (dispatch.BPShortCode == null)
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = dispatch.BPShortCode;
                if (dispatch.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = dispatch.Plant_Id;
                if (dispatch.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = dispatch.Company_Id;
                cmd.ExecuteNonQuery();

                //**********************************************************************************************************
                //               update dispatchitem table
                //**********************************************************************************************************
                foreach (DispatchItems di in dispatch.dispatchItems)
                {
                    sql = "Update dispatchitems set DispatchPlanID = @DispatchPlanID, ERPNo = @ERPNo, Quantity = @Quantity where DispatchItemsID = " + di.DispatchItemsID;
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    cmd.Parameters.Add(new MySqlParameter("@DispatchPlanID", MySqlDbType.Int32)).Value = di.DispatchPlanID;
                    if (di.ERPNo == null)
                        cmd.Parameters.Add(new MySqlParameter("@ERPNo", MySqlDbType.Int32)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@ERPNo", MySqlDbType.Int32)).Value = di.ERPNo;
                    if (di.Quantity == null)
                        cmd.Parameters.Add(new MySqlParameter("@Quantity", MySqlDbType.Int32)).Value = DBNull.Value;
                    else
                        cmd.Parameters.Add(new MySqlParameter("@Quantity", MySqlDbType.Int32)).Value = di.Quantity;
                    cmd.ExecuteNonQuery();

                }
                myTrans.Commit();
                return Ok("Record updated Successfuly");
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

        // DELETE api/<DispatchController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Delete from dispatchplan where DispatchPlanID = " + id;
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
