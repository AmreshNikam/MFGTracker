using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using MFG_Tracker.DatabaseTable;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MFG_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubModelController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public SubModelController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<SubModelController>
        [HttpGet("{PartID}")]
        public IActionResult Get(int PartID)
        {
            List<SubModel> sms = new List<SubModel>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from sub_model where Part_id = " + PartID;
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    SubModel sm = new SubModel();
                    if (rdr["Subpart_id"] != DBNull.Value)
                        sm.Subpart_id = Convert.ToInt32(rdr["Subpart_id"]);
                    else
                        sm.Subpart_id = null;
                    if (rdr["Subpart_Name"] != DBNull.Value)
                        sm.Subpart_Name = rdr["Subpart_Name"].ToString();
                    else
                        sm.Subpart_Name = null;
                    if (rdr["Part_id"] != DBNull.Value)
                        sm.Part_id = Convert.ToInt32(rdr["Part_id"]);
                    else
                        sm.Part_id = null;
                    if (rdr["Tag_time"] != DBNull.Value)
                        sm.Tag_time = Convert.ToInt32(rdr["Tag_time"]);
                    else
                        sm.Tag_time = null;
                    if (rdr["Customer_material_number"] != DBNull.Value)
                        sm.Customer_material_number = rdr["Customer_material_number"].ToString();
                    else
                        sm.Customer_material_number = null;
                    if (rdr["Year"] != DBNull.Value)
                        sm.Year = Convert.ToInt32(rdr["Year"]);
                    else
                        sm.Year = null;
                    if (rdr["CountOk"] != DBNull.Value)
                        sm.CountOk = Convert.ToInt32(rdr["CountOk"]);
                    else
                        sm.CountOk = null;
                    if (rdr["Month"] != DBNull.Value)
                        sm.Month = Convert.ToInt32(rdr["Month"]);
                    else
                        sm.Month = null;
                    if (rdr["PlanID"] != DBNull.Value)
                        sm.PlanID = Convert.ToInt32(rdr["PlanID"]);
                    else
                        sm.PlanID = null;
                    sms.Add(sm);
                }
                return Ok(sms);
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

        // POST api/<SubModelController>
        [HttpPost]
        public IActionResult Post([FromBody] SubModel sm)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Insert into sub_model (Subpart_Name, Part_id, Tag_time, Customer_material_number,Month,Year,CountOk,PlanID) values(@Subpart_Name, @Part_id, @Tag_time, @Customer_material_number,@Month,@Year,@CountOk,@PlanID)"; //, Assy_type, color   , @Assy_type, @color
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (sm.Subpart_Name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_Name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_Name", MySqlDbType.VarChar)).Value = sm.Subpart_Name;
                if (sm.Part_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Part_id", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Part_id", MySqlDbType.Int32)).Value = sm.Part_id;
                if (sm.Tag_time == null)
                    cmd.Parameters.Add(new MySqlParameter("@Tag_time", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Tag_time", MySqlDbType.Int32)).Value = sm.Tag_time;
                if (sm.Customer_material_number == null)
                    cmd.Parameters.Add(new MySqlParameter("@Customer_material_number", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Customer_material_number", MySqlDbType.VarChar)).Value = sm.Customer_material_number;
                cmd.Parameters.Add(new MySqlParameter("@Year", MySqlDbType.Int32)).Value = DateTime.Now.Year;
                cmd.Parameters.Add(new MySqlParameter("@Month", MySqlDbType.Int32)).Value = DateTime.Now.Month;
                cmd.Parameters.Add(new MySqlParameter("@CountOk", MySqlDbType.Int32)).Value = 0;
                cmd.Parameters.Add(new MySqlParameter("@PlanID", MySqlDbType.Int32)).Value = 0;
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

        // PUT api/<SubModelController>/5
        [HttpPut("{SID}")]
        public IActionResult Put(int SID, [FromBody] SubModel sm)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Update sub_model set Subpart_id = @Subpart_id, Subpart_Name = @Subpart_Name,Part_id = @Part_id, Tag_time = @Tag_time, Customer_material_number = @Customer_material_number where Subpart_id = " + SID; //, Assy_type = @Assy_type, color = @color
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (sm.Subpart_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_id", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_id", MySqlDbType.Int32)).Value = sm.Subpart_id;
                if (sm.Subpart_Name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_Name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_Name", MySqlDbType.VarChar)).Value = sm.Subpart_Name;
                if (sm.Part_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Part_id", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Part_id", MySqlDbType.Int32)).Value = sm.Part_id;
                if (sm.Tag_time == null)
                    cmd.Parameters.Add(new MySqlParameter("@Tag_time", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Tag_time", MySqlDbType.Int32)).Value = sm.Tag_time;
                if (sm.Customer_material_number == null)
                    cmd.Parameters.Add(new MySqlParameter("@Customer_material_number", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Customer_material_number", MySqlDbType.VarChar)).Value = sm.Customer_material_number;
                cmd.ExecuteNonQuery();
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

        // DELETE api/<SubModelController>/5
        [HttpDelete("{SID}")]
        public IActionResult Delete(int SID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from sub_model where Subpart_id = " + SID;
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
        [HttpPost("{BulckUpload}")]
        public IActionResult Post(bool BulckUpload)
        {
            if (!BulckUpload)
                return StatusCode(700, "Invalid URL");
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null;
            IFormFile formFile = Request.Form.Files[0];

            if (formFile == null || formFile.Length <= 0)
            {
                return StatusCode(700, "Empty excel file");
            }
            string fileExt = Path.GetExtension(formFile.FileName); //get the extension of uploaded excel file  
            try
            {
                connection.Open();
                myTrans = connection.BeginTransaction();
                using MemoryStream stream = new MemoryStream();
                formFile.CopyTo(stream);
                stream.Position = 0;
                ISheet worksheet = null;
                if (fileExt == ".xls")
                {
                    HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //HSSWorkBook object will read the Excel 97-2000 formats  
                    worksheet = hssfwb.GetSheetAt(0); //get first Excel sheet from workbook  
                }
                else if (fileExt == ".xlsx")
                {
                    XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //XSSFWorkBook will read 2007 Excel format  
                    worksheet = hssfwb.GetSheetAt(0); //get first Excel sheet from workbook   
                }
                else
                {
                    return StatusCode(700, "Not Support file extension");
                }
                DataTable dt = new DataTable();
                DataColumn dcName = new DataColumn("Subpart_Name", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Part_id", typeof(Int32));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Tag_time", typeof(Int32));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Customer_material_number", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Year", typeof(int));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("CountOk", typeof(int));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Month", typeof(int));
                dt.Columns.Add(dcName);
                string Subpart_Name;
                string Part_id;
                string Tag_time;
                string Customer_material_number;
                DataFormatter formatter = new DataFormatter();
                for (int row = 0; row <= worksheet.LastRowNum; row++) //Loop the records upto filled row  
                {
                    if (worksheet.GetRow(row) != null) //null is when the row only contains empty cells   
                    {

                        Subpart_Name = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(0));
                        Part_id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(1));
                        Tag_time = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(2));
                        Customer_material_number = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(3));
                        if (row.Equals(0))
                        {
                            if (Subpart_Name.ToLower() != "sub part name" || Part_id.ToLower() != "main part id" ||
                                Tag_time.ToLower() != "tag time" || Customer_material_number.ToLower() != "customer material number")
                                return StatusCode(400, "Template heading mismatch");
                            continue;
                        }
                        DataRow ldr = dt.NewRow();
                        ldr["Subpart_Name"] = Subpart_Name;
                        ldr["Part_id"] = Convert.ToInt32(Part_id);
                        ldr["Tag_time"] = Convert.ToInt32(Tag_time);
                        ldr["Customer_material_number"] = Customer_material_number;
                        ldr["Year"] = DateTime.Today.Year;
                        ldr["Month"] = DateTime.Today.Month;
                        ldr["CountOk"] = 0;
                        dt.Rows.Add(ldr);
                    }
                }
                //using TransactionScope scope = new TransactionScope();
                MySqlCommand cmd = new MySqlCommand("BulkUploadSubModel", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = myTrans,
                    UpdatedRowSource = UpdateRowSource.None
                };
                cmd.Parameters.Add("?Subpart_Name", MySqlDbType.String).SourceColumn = "Subpart_Name";
                cmd.Parameters.Add("?Part_id", MySqlDbType.Int32).SourceColumn = "Part_id";
                cmd.Parameters.Add("?Tag_time", MySqlDbType.Int32).SourceColumn = "Tag_time";
                cmd.Parameters.Add("?Customer_material_number", MySqlDbType.String).SourceColumn = "Customer_material_number";
                cmd.Parameters.Add("?Year", MySqlDbType.String).SourceColumn = "Year";
                cmd.Parameters.Add("?CountOk", MySqlDbType.Int32).SourceColumn = "CountOk";
                cmd.Parameters.Add("?Month", MySqlDbType.String).SourceColumn = "Month";

                MySqlDataAdapter da = new MySqlDataAdapter()
                { InsertCommand = cmd };
                //da.UpdateBatchSize = 100;
                int records = da.Update(dt);
                //scope.Complete();
                myTrans.Commit();
            }
            catch (MySqlException ex)
            {
                try{ myTrans.Rollback(); } catch { }
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
            return Ok("Record uploaded successfuly");
        }
    }
}
