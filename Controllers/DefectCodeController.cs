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
    public class DefectCodeController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public DefectCodeController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<DefeatCodeController>
        [HttpGet("{CompanyID}/{PlantID}/{StationID}")]
        public IActionResult Get(string CompanyID, string PlantID, int StationID)
        {
            List<DefeatCode> dcodes = new List<DefeatCode>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Select DC.DefectbarCode, DC.Message, DC.From_Station, WS1.Workstation_name as From_Name, DC.To_Station, WS2.Workstation_name as To_Name, DC.Plant_Id, DC.company_id, DC.defect_code_id " +
                             "from defect_code as DC " +
                             "left join  workstation as WS1 on DC.From_Station = WS1.Workstation_Id and DC.company_id = WS1.Company_Id and DC.Plant_Id = WS1.Plant_Id " +
                             "left join  workstation as WS2 on DC.To_Station = WS2.Workstation_Id and DC.company_id = WS2.Company_Id and DC.Plant_Id = WS2.Plant_Id " +
                             "where DC.Company_ID = '" + CompanyID + "' and DC.Plant_ID = '" + PlantID + "' and DC.From_Station =" + StationID;
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DefeatCode dc = new DefeatCode();
                    if (rdr["defect_code_id"] != DBNull.Value)
                        dc.defect_code_id = Convert.ToInt32(rdr["defect_code_id"]);
                    else
                        dc.defect_code_id = null;
                    if (rdr["DefectbarCode"] != DBNull.Value)
                        dc.DefectbarCode = rdr["DefectbarCode"].ToString();
                    else
                        dc.DefectbarCode = null;
                    if (rdr["Message"] != DBNull.Value)
                        dc.Message = rdr["Message"].ToString();
                    else
                        dc.Message = null;
                    if (rdr["From_Station"] != DBNull.Value)
                        dc.From_Station = Convert.ToInt32(rdr["From_Station"]);
                    else
                        dc.From_Station = null;
                    if (rdr["To_Station"] != DBNull.Value)
                        dc.To_Station = Convert.ToInt32(rdr["To_Station"]);
                    else
                        dc.To_Station = null;
                    dc.From_Name = rdr["From_Name"].ToString();
                    dc.To_Name = rdr["To_Name"].ToString();
                    dc.Plant_Id = rdr["Plant_Id"].ToString();
                    dc.Company_id = rdr["company_id"].ToString();
                    dcodes.Add(dc);
                }
                return Ok(dcodes);
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

        // GET api/<DefeatCodeController>/5
        [HttpGet("{Dcode}")]
        public IActionResult Get(int DCode)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Select * from defect_code where defeat_code_id = " + DCode;
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                DefeatCode dc = new DefeatCode();
                if (rdr.Read())
                {
                    if (rdr["defect_code_id"] != DBNull.Value)
                        dc.defect_code_id = Convert.ToInt32(rdr["defect_code_id"]);
                    else
                        dc.defect_code_id = null;
                    if (rdr["DefectbarCode"] != DBNull.Value)
                        dc.DefectbarCode = rdr["DefectbarCode"].ToString();
                    else
                        dc.DefectbarCode = null;
                    if (rdr["Message"] != DBNull.Value)
                        dc.Message = rdr["Message"].ToString();
                    else
                        dc.Message = null;
                    if (rdr["From_Station"] != DBNull.Value)
                        dc.From_Station = Convert.ToInt32(rdr["From_Station"]);
                    else
                        dc.From_Station = null;
                    if (rdr["To_Station"] != DBNull.Value)
                        dc.To_Station = Convert.ToInt32(rdr["To_Station"]);
                    else
                        dc.To_Station = null;
                    dc.Plant_Id = rdr["Plant_Id"].ToString();
                    dc.Company_id = rdr["company_id"].ToString();
                }
                return Ok(dc);
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

        // POST api/<DefeatCodeController>
        [HttpPost]
        public IActionResult Post([FromBody] DefeatCode dc)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Insert into defect_code (DefectbarCode, Message, From_Station, To_Station, Plant_Id, Company_id) values(@DefectbarCode, @Message, @From_Station, @To_Station, @Plant_Id, @Company_id)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (dc.DefectbarCode == null)
                    cmd.Parameters.Add(new MySqlParameter("@DefectbarCode", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@DefectbarCode", MySqlDbType.VarChar)).Value = dc.DefectbarCode;
                if (dc.Message == null)
                    cmd.Parameters.Add(new MySqlParameter("@Message", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Message", MySqlDbType.VarChar)).Value = dc.Message;
                if (dc.From_Station == null)
                    cmd.Parameters.Add(new MySqlParameter("@From_Station", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@From_Station", MySqlDbType.VarChar)).Value = dc.From_Station;
                if (dc.To_Station == null)
                    cmd.Parameters.Add(new MySqlParameter("@To_Station", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@To_Station", MySqlDbType.VarChar)).Value = dc.To_Station;
                if (dc.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = dc.Plant_Id;
                if (dc.Company_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = dc.Company_id;
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

        // PUT api/<DefeatCodeController>/5
        [HttpPut("{Dcode}")]
        public IActionResult Put(int DCode, [FromBody] DefeatCode dc)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Update defect_code set DefectbarCode = @DefectbarCode, Message = @Message, From_Station = @From_Station, To_Station = @To_Station, Plant_Id = @Plant_Id, Company_id = @Company_id where defect_code_id = " + DCode;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (dc.defect_code_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@defect_code_id", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@defect_code_id", MySqlDbType.Int32)).Value = dc.defect_code_id;
                if (dc.DefectbarCode == null)
                    cmd.Parameters.Add(new MySqlParameter("@DefectbarCode", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@DefectbarCode", MySqlDbType.VarChar)).Value = dc.DefectbarCode;
                if (dc.Message == null)
                    cmd.Parameters.Add(new MySqlParameter("@Message", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Message", MySqlDbType.VarChar)).Value = dc.Message;
                if (dc.From_Station == null)
                    cmd.Parameters.Add(new MySqlParameter("@From_Station", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@From_Station", MySqlDbType.VarChar)).Value = dc.From_Station;
                if (dc.To_Station == null)
                    cmd.Parameters.Add(new MySqlParameter("@To_Station", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@To_Station", MySqlDbType.VarChar)).Value = dc.To_Station;
                if (dc.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = dc.Plant_Id;
                if (dc.Company_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = dc.Company_id;
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

        // DELETE api/<DefeatCodeController>/5
        [HttpDelete("{Dcode}")]
        public IActionResult Delete(int Dcode)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Delete from defect_code where defect_code_id = " + Dcode;
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
                using (MemoryStream stream = new MemoryStream())
                {
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
                    DataColumn dcName = new DataColumn("DefectbarCode", typeof(string));
                    dt.Columns.Add(dcName);
                    dcName = new DataColumn("Message", typeof(string));
                    dt.Columns.Add(dcName);
                    dcName = new DataColumn("From_Station", typeof(Int32));
                    dt.Columns.Add(dcName);
                    dcName = new DataColumn("To_Station", typeof(Int32));
                    dt.Columns.Add(dcName);
                    dcName = new DataColumn("Plant_Id", typeof(string));
                    dt.Columns.Add(dcName);
                    dcName = new DataColumn("Company_id", typeof(string));
                    dt.Columns.Add(dcName);
                    string DefectbarCode;
                    string Message;
                    string From_Station;
                    string To_Station;
                    string Plant_Id;
                    string Company_id;
                    DataFormatter formatter = new DataFormatter();
                    for (int row = 0; row <= worksheet.LastRowNum; row++) //Loop the records upto filled row  
                    {
                        if (worksheet.GetRow(row) != null) //null is when the row only contains empty cells   
                        {

                            DefectbarCode = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(0));
                            Message = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(1));
                            From_Station = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(2));
                            To_Station = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(3));
                            Plant_Id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(4));
                            Company_id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(5));
                            if (row.Equals(0))
                            {
                                if (DefectbarCode.ToLower() != "bar code" || Message.ToLower() != "message" || From_Station.ToLower() != "from station id" ||
                                    To_Station.ToLower() != "to station id" || Plant_Id.ToLower() != "plant code" || Company_id.ToLower() != "company code")
                                    return StatusCode(700, "Template heading mismatch");
                                continue;
                            }
                            DataRow ldr = dt.NewRow();
                            ldr["DefectbarCode"] = DefectbarCode;
                            ldr["Message"] = Message;
                            ldr["From_Station"] = Convert.ToInt32(From_Station);
                            ldr["To_Station"] = Convert.ToInt32(To_Station);
                            ldr["Plant_Id"] = Plant_Id;
                            ldr["Company_id"] = Company_id;
                            dt.Rows.Add(ldr);
                        }
                    }
                    //using (TransactionScope scope = new TransactionScope())
                    //{
                    MySqlCommand cmd = new MySqlCommand("BulkUploadDefectCode", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Transaction = myTrans,
                        UpdatedRowSource = UpdateRowSource.None
                    };
                        cmd.Parameters.Add("?DefectbarCode", MySqlDbType.String).SourceColumn = "DefectbarCode";
                    cmd.Parameters.Add("?Message", MySqlDbType.String).SourceColumn = "Message";
                    cmd.Parameters.Add("?From_Station", MySqlDbType.Int32).SourceColumn = "From_Station";
                    cmd.Parameters.Add("?To_Station", MySqlDbType.Int32).SourceColumn = "To_Station";
                    cmd.Parameters.Add("?Plant_Id", MySqlDbType.String).SourceColumn = "Plant_Id";
                    cmd.Parameters.Add("?Company_id", MySqlDbType.String).SourceColumn = "Company_id";

                    MySqlDataAdapter da = new MySqlDataAdapter() { InsertCommand = cmd };
                    //da.UpdateBatchSize = 100;
                    int records = da.Update(dt);
                    //scope.Complete();
                    myTrans.Commit();
                    //}

                }
            }
            catch (MySqlException ex)
            {
                myTrans.Rollback();
                return StatusCode(600, ex.Message);
            }
            catch (Exception ex)
            {
                myTrans.Rollback();
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
