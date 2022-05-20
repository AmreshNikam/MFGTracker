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
    public class BusinessPartnerController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public BusinessPartnerController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<BusinessPartnerController>
        [HttpGet("{CompanyID}/{PlantID}")]
        public IActionResult Get(string CompanyID, string PlantID)
        {
            List<BusinessPartner> bps = new List<BusinessPartner>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Select * from Business_Partner where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    BusinessPartner bp = new BusinessPartner();
                    if (rdr["BP_Short_code"] != DBNull.Value)
                        bp.BP_Short_code = rdr["BP_Short_code"].ToString();
                    else
                        bp.BP_Short_code = null;
                    if (rdr["Name"] != DBNull.Value)
                        bp.Name = rdr["Name"].ToString();
                    else
                        bp.Name = null;

                    if (rdr["BP_Type"] != DBNull.Value)
                        bp.BP_Type = Convert.ToInt32(rdr["BP_Type"]);
                    else
                        bp.BP_Type = null;
                    if (rdr["Email"] != DBNull.Value)
                        bp.Email = rdr["Email"].ToString();
                    else
                        bp.Email = null;
                    if (rdr["Mobile_number"] != DBNull.Value)
                        bp.Mobile_number = rdr["Mobile_number"].ToString();
                    else
                        bp.Mobile_number = null;
                    bp.Plant_Id = rdr["Plant_Id"].ToString();
                    bp.Company_id = rdr["company_id"].ToString();
                    bps.Add(bp);
                }
                return Ok(bps);
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

        // GET api/<BusinessPartnerController>/5
        [HttpGet("{CompanyID}/{PlantID}/{BPCode}")]
        public IActionResult Get(string CompanyID, string PlantID, string BPCode)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Select * from Business_Partner where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "' and BP_Short_code = '" + BPCode + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                BusinessPartner bp = new BusinessPartner();
                if (rdr.Read())
                {

                    if (rdr["BP_Short_code"] != DBNull.Value)
                        bp.BP_Short_code = rdr["BP_Short_code"].ToString();
                    else
                        bp.BP_Short_code = null;
                    if (rdr["Name"] != DBNull.Value)
                        bp.Name = rdr["Name"].ToString();
                    else
                        bp.Name = null;
                    if (rdr["BP_Type"] != DBNull.Value)
                        bp.BP_Type = Convert.ToInt32(rdr["BP_Type"]);
                    else
                        bp.BP_Type = null;
                    if (rdr["Email"] != DBNull.Value)
                        bp.Email = rdr["Email"].ToString();
                    else
                        bp.Email = null;
                    if (rdr["Mobile_number"] != DBNull.Value)
                        bp.Mobile_number = rdr["Mobile_number"].ToString();
                    else
                        bp.Mobile_number = null;
                    bp.Plant_Id = rdr["Plant_Id"].ToString();
                    bp.Company_id = rdr["company_id"].ToString();

                }
                return Ok(bp);
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

        // POST api/<BusinessPartnerController>
        [HttpPost]
        public IActionResult Post([FromBody] BusinessPartner bp)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Insert into Business_Partner (BP_Short_code, Name, BP_Type, Email, Mobile_number, Plant_Id, company_id) values(@BP_Short_code, @Name, @BP_Type, @Email, @Mobile_number, @Plant_Id, @company_id)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (bp.BP_Short_code == null)
                    cmd.Parameters.Add(new MySqlParameter("@BP_Short_code", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BP_Short_code", MySqlDbType.VarChar)).Value = bp.BP_Short_code;
                if (bp.Name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Name", MySqlDbType.VarChar)).Value = bp.Name;
                if (bp.BP_Type == null)
                    cmd.Parameters.Add(new MySqlParameter("@BP_Type", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BP_Type", MySqlDbType.Int32)).Value = bp.BP_Type;
                if (bp.Email == null)
                    cmd.Parameters.Add(new MySqlParameter("@Email", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Email", MySqlDbType.VarChar)).Value = bp.Email;
                if (bp.Mobile_number == null)
                    cmd.Parameters.Add(new MySqlParameter("@Mobile_number", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Mobile_number", MySqlDbType.VarChar)).Value = bp.Mobile_number;
                if (bp.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = bp.Plant_Id;
                if (bp.Company_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = bp.Company_id;
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

        // PUT api/<BusinessPartnerController>/5
        [HttpPut("{CompanyID}/{PlantID}/{BPCode}")]
        public IActionResult Put(string CompanyID, string PlantID, string BPCode, [FromBody] BusinessPartner bp)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Update Business_Partner set BP_Short_code = @BP_Short_code, Name = @Name, BP_Type = @BP_Type, Email = @Email, Mobile_number = @Mobile_number, Plant_Id = @Plant_Id, company_id = @company_id where Company_ID = '" + CompanyID + "' and Plant_ID = '" + PlantID + "' and BP_Short_code = '" + BPCode + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (bp.BP_Short_code == null)
                    cmd.Parameters.Add(new MySqlParameter("@BP_Short_code", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BP_Short_code", MySqlDbType.VarChar)).Value = bp.BP_Short_code;
                if (bp.Name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Name", MySqlDbType.VarChar)).Value = bp.Name;
                if (bp.BP_Type == null)
                    cmd.Parameters.Add(new MySqlParameter("@BP_Type", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BP_Type", MySqlDbType.Int32)).Value = bp.BP_Type;
                if (bp.Email == null)
                    cmd.Parameters.Add(new MySqlParameter("@Email", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Email", MySqlDbType.VarChar)).Value = bp.Email;
                if (bp.Mobile_number == null)
                    cmd.Parameters.Add(new MySqlParameter("@Mobile_number", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Mobile_number", MySqlDbType.VarChar)).Value = bp.Mobile_number;
                if (bp.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = bp.Plant_Id;
                if (bp.Company_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = bp.Company_id;
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

        // DELETE api/<BusinessPartnerController>/5
        [HttpDelete("{CompanyID}/{PlantID}/{BPCode}")]
        public IActionResult Delete(string CompanyID, string PlantID, string BPCode)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Delete from Business_Partner where Company_ID = '" + CompanyID + "' and Plant_ID = '" + PlantID + "' and BP_Short_code = '" + BPCode + "'";
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
                DataColumn dcName = new DataColumn("BP_Short_code", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Name", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("BP_Type", typeof(int));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Email", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Mobile_number", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Plant_Id", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Company_id", typeof(string));
                dt.Columns.Add(dcName);
                string BP_Short_code;
                string Name;
                string BP_Type;
                string Email;
                string Mobile_number;
                string Plant_Id;
                string Company_id;
                DataFormatter formatter = new DataFormatter();
                for (int row = 0; row <= worksheet.LastRowNum; row++) //Loop the records upto filled row  
                {
                    if (worksheet.GetRow(row) != null) //null is when the row only contains empty cells   
                    {

                        BP_Short_code = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(0));
                        Name = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(1));
                        BP_Type = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(2));
                        Email = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(3));
                        Mobile_number = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(4));
                        Plant_Id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(5));
                        Company_id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(6));
                        if (row.Equals(0))
                        {
                            if (BP_Short_code.ToLower() != "bp short code" || Name.ToLower() != "business patner name" ||
                                BP_Type.ToLower() != "bp type" || Email.ToLower() != "email" || Mobile_number.ToLower() != "mobile number" ||
                                Plant_Id.ToLower() != "plant code" || Company_id.ToLower() != "company code")
                                return StatusCode(700, "Template heading mismatch");
                            continue;
                        }
                        DataRow ldr = dt.NewRow();
                        ldr["BP_Short_code"] = BP_Short_code;
                        ldr["Name"] = Name;
                        ldr["BP_Type"] = Convert.ToInt32(BP_Type);
                        ldr["Email"] = Email;
                        ldr["Mobile_number"] = Mobile_number;
                        ldr["Plant_Id"] = Plant_Id;
                        ldr["Company_id"] = Company_id;
                        dt.Rows.Add(ldr);
                    }
                }
                //using TransactionScope scope = new TransactionScope();
                MySqlCommand cmd = new MySqlCommand("BulkUploadBusinessPartner", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = myTrans,
                    UpdatedRowSource = UpdateRowSource.None
                };
                cmd.Parameters.Add("?BP_Short_code", MySqlDbType.String).SourceColumn = "BP_Short_code";
                cmd.Parameters.Add("?Name", MySqlDbType.String).SourceColumn = "Name";
                cmd.Parameters.Add("?BP_Type", MySqlDbType.String).SourceColumn = "BP_Type";
                cmd.Parameters.Add("?Email", MySqlDbType.String).SourceColumn = "Email";
                cmd.Parameters.Add("?Mobile_number", MySqlDbType.String).SourceColumn = "Mobile_number";
                cmd.Parameters.Add("?Plant_Id", MySqlDbType.String).SourceColumn = "Plant_Id";
                cmd.Parameters.Add("?Company_id", MySqlDbType.String).SourceColumn = "Company_id";

                MySqlDataAdapter da = new MySqlDataAdapter()
                { InsertCommand = cmd };

                //da.UpdateBatchSize = 100;
                int records = da.Update(dt);
                //scope.Complete();
                myTrans.Commit();
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
