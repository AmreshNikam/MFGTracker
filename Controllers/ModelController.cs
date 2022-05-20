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
    public class ModelController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public ModelController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<ModelController>
        [HttpGet("{CompanyID}/{PlantID}")]
        public IActionResult Get(string CompanyID, string PlantID)
        {
            List<Model> models = new List<Model>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from model where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Model model = new Model();
                    if (rdr["Part_id"] != DBNull.Value)
                        model.Part_id = Convert.ToInt32(rdr["Part_id"]);
                    else
                        model.Part_id = null;
                    if (rdr["Part_name"] != DBNull.Value)
                        model.Part_name = rdr["Part_name"].ToString();
                    else
                        model.Part_name = null;
                    if (rdr["BP_Short_code"] != DBNull.Value)
                        model.BP_Short_code = rdr["BP_Short_code"].ToString();
                    else
                        model.BP_Short_code = null;
                    model.Plant_Id = rdr["Plant_Id"].ToString();
                    model.Company_id = rdr["company_id"].ToString();
                    models.Add(model);
                }
                return Ok(models);
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

        // GET api/<ModelController>/5
        [HttpGet("{ID}")]
        public IActionResult Get(int ID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from model where Part_id = " + ID;
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                Model model = new Model();
                if (rdr.Read())
                {
                    if (rdr["Part_id"] != DBNull.Value)
                        model.Part_id = Convert.ToInt32(rdr["Part_id"]);
                    else
                        model.Part_id = null;
                    if (rdr["Part_name"] != DBNull.Value)
                        model.Part_name = rdr["Part_name"].ToString();
                    else
                        model.Part_name = null;
                    if (rdr["BP_Short_code"] != DBNull.Value)
                        model.BP_Short_code = rdr["BP_Short_code"].ToString();
                    else
                        model.BP_Short_code = null;
                    model.Plant_Id = rdr["Plant_Id"].ToString();
                    model.Company_id = rdr["company_id"].ToString();
                }
                return Ok(model);
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

        // POST api/<ModelController>
        [HttpPost]
        public IActionResult Post([FromBody] Model model)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Insert into model (Part_name, BP_Short_code, Plant_Id, Company_id) values(@Part_name, @BP_Short_code, @Plant_Id, @Company_id)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (model.Part_name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Part_name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Part_name", MySqlDbType.VarChar)).Value = model.Part_name;
                if (model.BP_Short_code == null)
                    cmd.Parameters.Add(new MySqlParameter("@BP_Short_code", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BP_Short_code", MySqlDbType.VarChar)).Value = model.BP_Short_code;
                if (model.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = model.Plant_Id;
                if (model.Company_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = model.Company_id;
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

        // PUT api/<ModelController>/5
        [HttpPut("{ID}")]
        public IActionResult Put(int ID, [FromBody] Model model)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Update model set Part_name = @Part_name, BP_Short_code = @BP_Short_code, Plant_Id = @Plant_Id, Company_id = @Company_id where Part_id = " + ID;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (model.Part_name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Part_name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Part_name", MySqlDbType.VarChar)).Value = model.Part_name;
                if (model.BP_Short_code == null)
                    cmd.Parameters.Add(new MySqlParameter("@BP_Short_code", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@BP_Short_code", MySqlDbType.VarChar)).Value = model.BP_Short_code;
                if (model.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = model.Plant_Id;
                if (model.Company_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = model.Company_id;
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

        // DELETE api/<ModelController>/5
        [HttpDelete("{ID}")]
        public IActionResult Delete(int ID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from model where Part_id = " + ID;
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
                DataColumn dcName = new DataColumn("Part_name", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("BP_Short_code", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Plant_Id", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Company_id", typeof(string));
                dt.Columns.Add(dcName);
                string Part_name;
                string BP_Short_code;
                string Plant_Id;
                string Company_id;
                DataFormatter formatter = new DataFormatter();
                for (int row = 0; row <= worksheet.LastRowNum; row++) //Loop the records upto filled row  
                {
                    if (worksheet.GetRow(row) != null) //null is when the row only contains empty cells   
                    {

                        Part_name = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(0));
                        BP_Short_code = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(1));
                        Plant_Id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(2));
                        Company_id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(3));
                        if (row.Equals(0))
                        {
                            if (Part_name.ToLower() != "part name" || BP_Short_code.ToLower() != "bp shor code" ||
                                Plant_Id.ToLower() != "plant code" || Company_id.ToLower() != "company code")
                                return StatusCode(700, "Template heading mismatch");
                            continue;
                        }
                        DataRow ldr = dt.NewRow();
                        ldr["Part_name"] = Part_name;
                        ldr["BP_Short_code"] = BP_Short_code;
                        ldr["Plant_Id"] = Plant_Id;
                        ldr["Company_id"] = Company_id;
                        dt.Rows.Add(ldr);
                    }
                }
                //using TransactionScope scope = new TransactionScope();
                MySqlCommand cmd = new MySqlCommand("BulkUploadModel", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = myTrans,
                    UpdatedRowSource = UpdateRowSource.None
                };
                cmd.Parameters.Add("?Part_Name", MySqlDbType.String).SourceColumn = "Part_name";
                cmd.Parameters.Add("?BP_Short_code", MySqlDbType.String).SourceColumn = "BP_Short_code";
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
