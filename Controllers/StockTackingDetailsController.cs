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
    public class StockTackingDetailsController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public StockTackingDetailsController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        [HttpGet("{Barcode}")]
        public IActionResult Get(string BarCode)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "select BM.Barcode, BM.ERPNumberID, BM.Storage_id, ER.Subpart_id, SM.Subpart_Name, ER.ColorID,ER.AssemblyID, " +
                             "concat(Right(concat('000',CONVERT(SS.StoreID,char)),3), " +
                             "Right(concat('000',CONVERT(ifnull(SS.Section,'0'),char)),3), " +
                             "Right(concat('000',CONVERT(ifnull(SS.Rack,'0'),char)),3), " +
                             "Right(concat('000',CONVERT(ifnull(SS.Shelf,'0'),char)),3), " +
                             "Right(concat('000',CONVERT(ifnull(SS.Bins,'0'),char)),3)) as Code " +
                             "from barcodemaster as BM " +
                             "left join erpnumber as ER on BM.ERPNumberID = ER.ERPNumberID " +
                             "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id " +
                             "left join storages as SS on BM.Storage_id = SS.Storage_id " +
                             "where BM.Barcode = '" + BarCode + "' and  Disatched = 0";
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                StockTacckig st = new StockTacckig();
                if (rdr.Read())
                {

                    if (rdr["ERPNumberID"] != DBNull.Value)
                        st.ERPNumberID = Convert.ToInt32(rdr["ERPNumberID"]);
                    else
                        st.ERPNumberID = null;
                    if (rdr["Storage_id"] != DBNull.Value)
                        st.Storage_id = Convert.ToInt32(rdr["Storage_id"]);
                    else
                        st.Storage_id = null;
                    if (rdr["Subpart_id"] != DBNull.Value)
                        st.Subpart_id = Convert.ToInt32(rdr["Subpart_id"]);
                    else
                        st.Subpart_id = null;
                    if (rdr["ColorID"] != DBNull.Value)
                        st.ColorID = Convert.ToInt32(rdr["ColorID"]);
                    else
                        st.ColorID = null;
                    if (rdr["AssemblyID"] != DBNull.Value)
                        st.AssemblyID = Convert.ToInt32(rdr["AssemblyID"]);
                    else
                        st.AssemblyID = null;
                    st.Barcode = rdr["Barcode"].ToString();
                    st.Code = rdr["Code"].ToString();
                    st.Subpart_Name = rdr["Subpart_Name"].ToString();

                }
                return Ok(st);
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


        

        // POST api/<StockTackingDetailsController>
        [HttpPost]
        public IActionResult Post([FromBody] StockTacckig st, [FromQuery] int Change = 0)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql;
                MySqlCommand cmd;
                int? ERPNo = st.ERPNumberID;
                if(Change >= 2)
                {
                    string Co = st.ColorID == null ? "is null ColorID" : "ColorID = " + st.ColorID;
                    string As = st.AssemblyID == null ? "is null AssemblyID" : "AssemblyID = " + st.AssemblyID;
                    sql = "Select ERPNumberID from erpnumber where Subpart_id = " + st.Subpart_id + " and " + Co + " and " + As;
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        ERPNo = Convert.ToInt32(rdr["ERPNumberID"]);
                    }
                    else
                        return StatusCode(700, "Unable to find ERP Number");
                    rdr.Close();


                }
                int? Storage_id = st.Storage_id;
                if (Change == 1 || Change > 3)
                {
                    string code = st.Code;
                    int StoreID = Convert.ToInt32(code.Substring(0, 3));
                    string Section = code.Substring(3, 3);
                    if (Section == "000")
                        Section = "Section is null";
                    else
                        Section = "Section = '" + Section + "'";
                    string Rack = code.Substring(6, 3);
                    if (Rack == "000")
                        Rack = "Rack is null";
                    else
                        Rack = "Rack = '" + Rack + "'";
                    string Shelf = code.Substring(9, 3);
                    if (Shelf == "000")
                        Shelf = "Shelf is null";
                    else
                        Shelf = "Shelf = '" + Shelf + "'";
                    string Bins = code.Substring(12, 3);
                    if (Bins == "000")
                        Bins = "Bins is null";
                    else
                        Bins = "Bins = '" + Bins + "'";
                    sql = "Select Storage_id from storages where StoreID = " + StoreID + " and " + Section + " and " + Rack + " and " + Shelf + " and " + Bins;
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        Storage_id = Convert.ToInt32(rdr["Storage_id"]);
                    }
                    else
                        return StatusCode(700, "Invalid Location");
                    rdr.Close();
                }

                sql = "Insert into stocktakingdetails (Barcode, StocktakingMasterID, ERPNo, StoregesID, Status) " +
                             "values(@Barcode, @StocktakingMasterID, @ERPNo, @StoregesID, @Status) " +
                             "ON DUPLICATE KEY " +
                             "Update Barcode = @Barcode, StocktakingMasterID = @StocktakingMasterID, ERPNo = @ERPNo, StoregesID = @StoregesID, Status = @Status";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                cmd.Parameters.Add(new MySqlParameter("@Barcode", MySqlDbType.VarChar)).Value = st.Barcode;
                cmd.Parameters.Add(new MySqlParameter("@StocktakingMasterID", MySqlDbType.Int32)).Value = st.StocktakingMasterID;
                cmd.Parameters.Add(new MySqlParameter("@ERPNo", MySqlDbType.Int32)).Value = ERPNo;
                cmd.Parameters.Add(new MySqlParameter("@StoregesID", MySqlDbType.Int32)).Value = Storage_id;
                cmd.Parameters.Add(new MySqlParameter("@Status", MySqlDbType.Int32)).Value = Change;
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

        // PUT api/<StockTackingDetailsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<StockTackingDetailsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
