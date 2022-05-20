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
    public class StoreOutController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public StoreOutController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET api/<StoreOutController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id,[FromQuery] string COMPANYID, string PLANTID)
        {
            List<BarcodeList> brs = new List<BarcodeList>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                List<DispatchOutSumarry> DPs = new List<DispatchOutSumarry>();
                BarcodeListWithCount BL = new BarcodeListWithCount();
                string sql = "SELECT SM.Subpart_Name, CO.ColorcolName, AM.AssemblyName, DI.Quantity, DP.AllowNextScan, DI.DispatchItemsID, DI.ERPNo, ifnull(DL.Dispatched,0) as Dispatched  " +
                             "FROM dispatchitems as DI " +
                             "left join dispatchplan as DP on DI.DispatchPlanID = DP.DispatchPlanID " +
                             "left join erpnumber as ER on DI.ERPNo = ER.ERPNumberID " +
                             "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id " +
                             "left join color as CO on ER.ColorID = CO.ColorID " +
                             "left join assembly as AM on ER.AssemblyID = AM.AssemblyID " +
                             "left join(select count(BarCode) as Dispatched, DispatchItemID from dispatchbarcode group by DispatchItemID) as DL on DI.DispatchItemsID = DL.DispatchItemID " +
                             "where DI.DispatchPlanID = " + id;
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DispatchOutSumarry dp = new DispatchOutSumarry();
                    dp.DispatchItemsID = Convert.ToInt32(rdr["DispatchItemsID"]);
                    dp.ERPNo = Convert.ToInt32(rdr["ERPNo"]);
                    dp.Dispatched = Convert.ToInt32(rdr["Dispatched"]);
                    dp.Subpart_Name = rdr["Subpart_Name"].ToString();
                    dp.ColorcolName = rdr["ColorcolName"].ToString();
                    dp.AssemblyName = rdr["AssemblyName"].ToString();
                    if (rdr["Quantity"] == DBNull.Value)
                        dp.Quantity = 0;
                    else
                        dp.Quantity = Convert.ToInt32(rdr["Quantity"]);
                    if (rdr["AllowNextScan"] == DBNull.Value)
                        dp.AllowNextScan = 0;
                    else
                        dp.AllowNextScan = Convert.ToInt32(rdr["AllowNextScan"]);
                    DPs.Add(dp);
                }
                rdr.Close();
                foreach(DispatchOutSumarry dp in DPs)
                {
                    sql = "select A. Barcode from " +
                          "(Select Barcode from BarcodeMaster " +
                          "where ERPNumberID = " + dp.ERPNo + " and substring(Barcode,1,1) = " +
                          "(select PlantIdentificationChar from plant where Plant_Id = '" + PLANTID + "' and company_id = '" + COMPANYID + "') " +
                          "and Disatched = 0 order by substring(Barcode, 2, 2), substring(Barcode, 4, 2),substring(Barcode, 6, 3), substring(Barcode, 12, 5)) as A " +
                          "where A.Barcode not in (select BarCode " +
                          "from dispatchbarcode as DB " +
                          "left join dispatchitems DI on DB.DispatchItemID = DI.DispatchItemsID " +
                          "left join DispatchPlan as DP on DI.DispatchPlanID = DP.DispatchPlanID " +
                          "where DP.Status = 1 union select BarCode from missingbarcode where DispatchPlanID = " + id + ") Limit " + (dp.Quantity + dp.AllowNextScan - dp.Dispatched);


                    cmd = new MySqlCommand(sql, connection)
                    { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    
                    while (rdr.Read())
                    {
                        BarcodeList br = new BarcodeList();
                        br.Barcode = rdr["Barcode"].ToString();
                        br.Subpart_Name = dp.Subpart_Name;
                        br.ColorcolName = dp.ColorcolName;
                        br.AssemblyName = dp.AssemblyName;
                        brs.Add(br);
                    }
                    rdr.Close();
                    
                    BL.BList = brs;
                    BL.nextscan = dp.AllowNextScan;
                    BL.remeningQty = dp.Quantity - dp.Dispatched;
                }
                
                return Ok(BL);
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

        // POST api/<StoreOutController>
        [HttpPost]
        public IActionResult Post([FromBody] StoreOut storeOut, [FromQuery] bool Extra = false)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null;
            try
            {
                connection.Open();
                myTrans = connection.BeginTransaction();
                string sql = "SELECT DI.ERPNo, DI.Quantity,DP.AllowNextScan, DI.DispatchItemsID " +
                             "FROM dispatchitems as DI " +
                             "left join dispatchplan as DP on DI.DispatchPlanID = DP.DispatchPlanID " +
                             "where DI.DispatchPlanID = " + storeOut.DispatchPlanID;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                List<DPlan> DPs = new List<DPlan>();
                while (rdr.Read())
                {
                    DPlan dp = new DPlan();
                    dp.ERPNo = Convert.ToInt32(rdr["ERPNo"]);
                    dp.Quantity = Convert.ToInt32(rdr["Quantity"]);
                    dp.AllowNextScan = Convert.ToInt32(rdr["AllowNextScan"]);
                    dp.DispatchItemsID = Convert.ToInt32(rdr["DispatchItemsID"]);
                    DPs.Add(dp);
                }
                rdr.Close();
                sql = "select ERPNumberID from barcodemaster where Barcode = '" + storeOut.Barcode + "'";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                rdr = cmd.ExecuteReader();
                int erpno;
                if (rdr.Read())
                    erpno = Convert.ToInt32(rdr["ERPNumberID"]);
                else
                    return StatusCode(700, "No such barcode");
                rdr.Close();
                int id = 0, i = 0;
                for (; i < DPs.Count; i++)
                {
                    if (DPs[i].ERPNo == erpno)
                    {
                        id = DPs[i].DispatchItemsID;
                        break;
                    }
                }
                if (i >= DPs.Count)
                    return StatusCode(700, "Invlid Barcode");
                //******************************************************************
                //            Check barcode in fifo
                //******************************************************************AgainstBarcode 4, 2),substring(Barcode, 6, 3), substring(Barcode, 12, 5) " +
                //      "Limit " + (DPs[i].Quantity + DPs[i].AllowNextScan);
                //cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                //rdr = cmd.ExecuteReader();
                //bool flag = false;
                //while (rdr.Read())
                //{
                //    if (storeOut.Barcode == rdr["Barcode"].ToString())
                //    {
                //        flag = true;
                //        break;
                //    }
                //}
                //rdr.Close();
                //if (!flag)
                //    return StatusCode(500, "Selected Barcode is not in FIFO");
                //******************************************************************
                //            insert into dispatch barcode table
                //******************************************************************
                sql = "insert into dispatchbarcode (BarCode, DispatchItemID, TrollyBox, ScanDateTime, BPShortCode, Plant_Id, Company_Id) values (@BarCode, @DispatchItemID, @TrollyBox, @ScanDateTime, @BPShortCode, @Plant_Id, @Company_Id)";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                cmd.Parameters.Add(new MySqlParameter("@BarCode", MySqlDbType.VarChar)).Value = storeOut.Barcode;
                cmd.Parameters.Add(new MySqlParameter("@DispatchItemID", MySqlDbType.VarChar)).Value = id;
                cmd.Parameters.Add(new MySqlParameter("@TrollyBox", MySqlDbType.VarChar)).Value = Convert.ToInt16(storeOut.TrollyBox);
                cmd.Parameters.Add(new MySqlParameter("@ScanDateTime", MySqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                cmd.Parameters.Add(new MySqlParameter("@BPShortCode", MySqlDbType.VarChar)).Value = storeOut.BPShortCode;
                cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = storeOut.Plant_Id;
                cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = storeOut.Compamy_Id;


                cmd.ExecuteNonQuery();
                //****************************************************************
                //               Update barcode master
                //***************************************************************
                int storage_id = 9999; //Box
                if (storeOut.TrollyBox.Value)
                {
                    
                    int stroreID = Convert.ToInt32(storeOut.TrollyBarcode.Substring(0, 3));
                    string Section = storeOut.TrollyBarcode.Substring(3, 3) == "000" ? "Section is null" : "Section = " + storeOut.TrollyBarcode.Substring(3, 3);
                    string Rack = storeOut.TrollyBarcode.Substring(6, 3) == "000" ? "Rack is null" : "Rack = " + storeOut.TrollyBarcode.Substring(6, 3);
                    string Shelf = storeOut.TrollyBarcode.Substring(9, 3) == "000" ? "Shelf is null" : "Shelf = " + storeOut.TrollyBarcode.Substring(9, 3);
                    string Bins = storeOut.TrollyBarcode.Substring(12, 3) == "000" ? "Bins is null" : "Bins = " + storeOut.TrollyBarcode.Substring(12, 3);
                    sql = "select Storage_id from storages where StoreID = " + stroreID + "  and " + Section + " and " + Rack + " and " + Shelf + " and " + Bins;
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        storage_id = Convert.ToInt32(rdr["Storage_id"]);
                    }
                    else
                        return StatusCode(700, "Can't find Storage Location");
                    rdr.Close();
                    

                }
                //Udate Barcode Master
                sql = "Update barcodemaster set Storage_id = " + storage_id + ", Disatched = 2 where Barcode = '" + storeOut.Barcode + "'";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                cmd.ExecuteNonQuery();
                //*****************************************************************
                //      Return updated quantity
                //*****************************************************************

                //************************Close Dispatch plan ******************************
                //if(!flag)
                //{
                //    sql = "Update dispatchplan set Status = 0 where DispatchPlanID = " + id;
                //    cmd = new MySqlCommand(sql, connection){ CommandType = CommandType.Text };
                //    cmd.ExecuteNonQuery();
                //}
                //************************Close Dispatch plan ******************************
                if (Extra)
                {
                    sql = "Update dispatchplan set AllowNextScan = if(AllowNextScan > 0, (AllowNextScan -1), 0) where DispatchPlanID = " + storeOut.DispatchPlanID;
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    cmd.ExecuteNonQuery();
                    //Store Against barcode in Missing Barcode table;
                    sql = "insert into missingbarcode (Barcode,DispatchPlanID) values (@Barcode,@DispatchPlanID)";
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    cmd.Parameters.Add(new MySqlParameter("@BarCode", MySqlDbType.VarChar)).Value = storeOut.AgainstBarcode;
                    cmd.Parameters.Add(new MySqlParameter("@DispatchPlanID", MySqlDbType.VarChar)).Value = storeOut.DispatchPlanID;
                    cmd.ExecuteNonQuery();
                }
                myTrans.Commit();
                return Ok("Barcode successfuly out");
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

        // PUT api/<StoreOutController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id ,[FromQuery] string COMPANYID, string PLANTID, string BPCODE)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                List<BarcodeListPrint> barcode = new List<BarcodeListPrint>();
                string sql = "Select DI.DispatchItemsID, DB.Barcode, SM.Subpart_Name, CO.ColorcolName, AM.AssemblyName, ER.ERPNumber, ST.Stores_Name " +
                             "from dispatchbarcode as DB " +
                             "left join dispatchitems DI on DB.DispatchItemID = DI.DispatchItemsID " +
                             "left join dispatchplan as DP on DI.DispatchPlanID = DP.DispatchPlanID " +
                             "left join erpnumber ER on DI.ERPNo = ER.ERPNumberID " +
                             "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id " +
                             "left join color as CO on ER.ColorID = CO.ColorID " +
                             "left join assembly as AM on ER.AssemblyID = AM.AssemblyID " +
                             "left join barcodemaster as BR on DB.Barcode = BR.BarCode " +
                             "left join storages as SG on BR.Storage_id = SG.Storage_id " +
                             "left join stores as ST on SG.StoreID = ST.Stores_id " +
                             "where DP.DispatchPlanID = " + id + " and ER.Plant_Id = '" + PLANTID + "' and ER.Company_Id = '" + COMPANYID + "' " +
                             "order by SM.Subpart_Name, CO.ColorcolName, AM.AssemblyName"; 
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                
                while (rdr.Read())
                {
                    BarcodeListPrint bp = new BarcodeListPrint();
                    bp.DispatchItemsID = Convert.ToInt32(rdr["DispatchItemsID"]);
                    bp.Barcode = rdr["Barcode"].ToString();
                    bp.Subpart_Name = rdr["Subpart_Name"].ToString();
                    bp.ColorcolName = rdr["ColorcolName"].ToString();
                    bp.AssemblyName = rdr["AssemblyName"].ToString();
                    bp.ERPNumbeer = rdr["ERPNumber"].ToString();
                    bp.Trolley = rdr["Stores_Name"].ToString();
                    barcode.Add(bp);
                }
                rdr.Close();
                //************************** send to printer ************************
                List<List<DispatchRept>> strrept = new List<List<DispatchRept>>();
                var groupedList = barcode.GroupBy(u => new { u.DispatchItemsID });
                int count = 1;
                foreach (var group in groupedList)
                {
                    int count2 = 1;
                    foreach (var info in group)
                    {
                        List<DispatchRept> st = new List<DispatchRept>();
                        DispatchRept rept = new DispatchRept();
                        rept.text = count.ToString();
                        st.Add(rept);
                        rept = new DispatchRept();
                        rept.text = count2.ToString();
                        st.Add(rept);
                        rept = new DispatchRept();
                        rept.text = info.Subpart_Name + " " + info.ColorcolName + " " + info.AssemblyName;
                        st.Add(rept);
                        rept = new DispatchRept();
                        rept.text = info.ERPNumbeer;
                        st.Add(rept);
                        rept = new DispatchRept();
                        rept.text = info.Barcode;
                        st.Add(rept);
                        rept = new DispatchRept();
                        rept.text = info.Trolley;
                        st.Add(rept);
                        strrept.Add(st);
                        count2++;
                        count++;
                        //**********************Update barcode master ***********************
                        //foreach (BarcodeListPrint br in barcode)
                        //{
                        sql = "Update barcodemaster set Disatched = 1 where Barcode = '" + info.Barcode + "'";
                        cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                        cmd.ExecuteNonQuery();
                        //**********************Update barcode details ***********************
                        sql = "insert into barcodedetails (Barcode,Workstation_Id,DefectCode,TimeStamp,BP_ShortCode,ExecutionType,ExecutionPlanNo,Plant_Id,Company_Id) values (@Barcode,@Workstation_Id,@DefectCode,@TimeStamp,@BP_ShortCode,@ExecutionType,@ExecutionPlanNo,@Plant_Id,@Company_Id)";
                        cmd = new MySqlCommand(sql, connection)
                        { CommandType = CommandType.Text };
                        cmd.Parameters.Add(new MySqlParameter("Barcode", MySqlDbType.VarChar)).Value = info.Barcode;
                        cmd.Parameters.Add(new MySqlParameter("Workstation_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                        cmd.Parameters.Add(new MySqlParameter("DefectCode", MySqlDbType.Int32)).Value = DBNull.Value; 
                        cmd.Parameters.Add(new MySqlParameter("TimeStamp", MySqlDbType.DateTime)).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        cmd.Parameters.Add(new MySqlParameter("BP_ShortCode", MySqlDbType.VarChar)).Value = BPCODE;
                        cmd.Parameters.Add(new MySqlParameter("ExecutionType", MySqlDbType.VarChar)).Value = "D";
                        cmd.Parameters.Add(new MySqlParameter("ExecutionPlanNo", MySqlDbType.Int32)).Value = id;
                        cmd.Parameters.Add(new MySqlParameter("Plant_Id", MySqlDbType.VarChar)).Value = PLANTID;
                        cmd.Parameters.Add(new MySqlParameter("Company_Id", MySqlDbType.VarChar)).Value = COMPANYID;
                        cmd.ExecuteNonQuery();
                    }   

                }
                return Ok(strrept);
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

        // DELETE api/<StoreOutController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
