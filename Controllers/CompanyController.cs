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
    public class CompanyController : ControllerBase
    {

        private readonly IConfiguration Configuration;

        public CompanyController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }


        // GET: api/<CompanyController>
        [HttpGet]
        //public IActionResult Get(string CompanyID) => string.IsNullOrEmpty(CompanyID) ? GetAllCompany() : GetCompany(CompanyID);
        public IActionResult GetAllCompany()
        {
            List<Company> company = new List<Company>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Select * from COMPANY";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Company cmp = new Company();
                    if (rdr["Company_ID"] != DBNull.Value)
                        cmp.Company_ID = rdr["Company_ID"].ToString();
                    else
                        cmp.Company_ID = null;
                    if (rdr["Company_Name"] != DBNull.Value)
                        cmp.Company_Name = rdr["Company_Name"].ToString();
                    else
                        cmp.Company_Name = null;
                    if (rdr["Company_Address"] != DBNull.Value)
                        cmp.Company_Address = rdr["Company_Address"].ToString();
                    else
                        cmp.Company_Address = null;
                    if (rdr["LogoFileExtension"] != DBNull.Value)
                        cmp.LogoFileExtension = rdr["LogoFileExtension"].ToString();
                    else
                        cmp.LogoFileExtension = null;
                    if (rdr["LogoFile"] != DBNull.Value)
                        cmp.LogoFile = (byte[])rdr["LogoFile"];
                    else
                        cmp.LogoFile = null;
                    company.Add(cmp);
                }
                return Ok(company);
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
        [HttpGet("{CompanyID}")]
        public IActionResult GetCompany(string CompanyID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Select * from COMPANY where Company_ID = '" + CompanyID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                Company cmp = new Company();
                if (rdr.Read())
                {

                    if (rdr["Company_ID"] != DBNull.Value)
                        cmp.Company_ID = rdr["Company_ID"].ToString();
                    else
                        cmp.Company_ID = null;
                    if (rdr["Company_Name"] != DBNull.Value)
                        cmp.Company_Name = rdr["Company_Name"].ToString();
                    else
                        cmp.Company_Name = null;
                    if (rdr["Company_Address"] != DBNull.Value)
                        cmp.Company_Address = rdr["Company_Address"].ToString();
                    else
                        cmp.Company_Address = null;
                    if (rdr["LogoFileExtension"] != DBNull.Value)
                        cmp.LogoFileExtension = rdr["LogoFileExtension"].ToString();
                    else
                        cmp.LogoFileExtension = null;
                    if (rdr["LogoFile"] != DBNull.Value)
                        cmp.LogoFile = (byte[])rdr["LogoFile"];
                    else
                        cmp.LogoFile = null;
                }
                return Ok(cmp);
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

        // POST api/<CompanyController>
        [HttpPost]
        public IActionResult Post([FromBody] Company company)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Insert into COMPANY (Company_ID, Company_Name, Company_Address, LogoFileExtension, LogoFile) values(@Company_ID, @Company_Name, @Company_Address, @LogoFileExtension, @LogoFile)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (company.Company_ID == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_ID", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_ID", MySqlDbType.VarChar)).Value = company.Company_ID;
                if (company.Company_Name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Name", MySqlDbType.VarChar)).Value = company.Company_Name;
                if (company.Company_Address == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Address", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Address", MySqlDbType.VarChar)).Value = company.Company_Address;
                if (company.LogoFileExtension == null)
                    cmd.Parameters.Add(new MySqlParameter("@LogoFileExtension", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@LogoFileExtension", MySqlDbType.VarChar)).Value = company.LogoFileExtension;
                if (company.LogoFile == null)
                    cmd.Parameters.Add(new MySqlParameter("@LogoFile", MySqlDbType.Blob)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@LogoFile", MySqlDbType.Blob)).Value = company.LogoFile;
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
        // PUT api/<CompanyController>/5
        [HttpPut("{CompanyID}")]
        public IActionResult Put(string CompanyID, [FromBody] Company company)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Update COMPANY set Company_ID = @Company_ID, Company_Name = @Company_Name, Company_Address = @Company_Address, LogoFileExtension = @LogoFileExtension, LogoFile = @LogoFile  where Company_ID = '" + CompanyID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (company.Company_ID == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_ID", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_ID", MySqlDbType.VarChar)).Value = company.Company_ID;
                if (company.Company_Name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Name", MySqlDbType.VarChar)).Value = company.Company_Name;
                if (company.Company_Address == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Address", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Address", MySqlDbType.VarChar)).Value = company.Company_Address;
                if (company.LogoFileExtension == null)
                    cmd.Parameters.Add(new MySqlParameter("@LogoFileExtension", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@LogoFileExtension", MySqlDbType.VarChar)).Value = company.LogoFileExtension;
                if (company.LogoFile == null)
                    cmd.Parameters.Add(new MySqlParameter("@LogoFile", MySqlDbType.Blob)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@LogoFile", MySqlDbType.Blob)).Value = company.LogoFile;
                cmd.ExecuteNonQuery();
                return Ok("Record Updated Successfuly");
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

        // DELETE api/<CompanyController>/5
        [HttpDelete("{CompanyID}")]
        public IActionResult Delete(string CompanyID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Delete from COMPANY where Company_ID = '" + CompanyID + "'";
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
                DataColumn dcName = new DataColumn("Company_ID", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Company_Name", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Company_Address", typeof(string));
                dt.Columns.Add(dcName);
                string CompanyCode;
                string CompanyName;
                string Address;
                DataFormatter formatter = new DataFormatter();
                for (int row = 0; row <= worksheet.LastRowNum; row++) //Loop the records upto filled row  
                {
                    if (worksheet.GetRow(row) != null) //null is when the row only contains empty cells   
                    {

                        CompanyCode = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(0));
                        CompanyName = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(1));
                        Address = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(2));
                        if (row.Equals(0))
                        {
                            if (CompanyCode.ToLower() != "company code" || CompanyName.ToLower() != "company name" || Address.ToLower() != "company address")
                                return StatusCode(700, "Template heading mismatch");
                            continue;
                        }
                        DataRow ldr = dt.NewRow();
                        ldr["Company_ID"] = CompanyCode;
                        ldr["Company_Name"] = CompanyName;
                        ldr["Company_Address"] = Address;
                        dt.Rows.Add(ldr);
                    }
                }
                //using TransactionScope scope = new TransactionScope();
                MySqlCommand cmd = new MySqlCommand("BulkUploadCompany", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = myTrans,
                    UpdatedRowSource = UpdateRowSource.None
                };
                cmd.Parameters.Add("?company_id", MySqlDbType.String).SourceColumn = "Company_ID";
                cmd.Parameters.Add("?company_name", MySqlDbType.String).SourceColumn = "Company_Name";
                cmd.Parameters.Add("?company_address", MySqlDbType.String).SourceColumn = "Company_Address";
                MySqlDataAdapter da = new MySqlDataAdapter()
                { InsertCommand = cmd };
                //da.UpdateBatchSize = 100;
                int records = da.Update(dt);
                //scope.Complete();
                myTrans.Commit();
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
            return Ok("Record uploaded successfuly");
        }
    }
}
