using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using MFG_Tracker.DatabaseTable;
using Microsoft.AspNetCore.Http;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using System.Transactions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MFG_Tracker.DatabaseTable
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public PlantController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<PlantController>
        [HttpGet("{CompanyID}")]
        public IActionResult Get(string CompanyID)
        {
            List<Plant> plants = new List<Plant>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from Plant where Company_ID = '" + CompanyID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Plant plant = new Plant();
                    if (rdr["Plant_Id"] != DBNull.Value)
                        plant.Plant_Id = rdr["Plant_Id"].ToString();
                    else
                        plant.Plant_Id = null;
                    if (rdr["Plant_name"] != DBNull.Value)
                        plant.Plant_name = rdr["Plant_name"].ToString();
                    else
                        plant.Plant_name = null;
                    if (rdr["Plant_location"] != DBNull.Value)
                        plant.Plant_location = rdr["Plant_location"].ToString();
                    else
                        plant.Plant_location = null;
                    if (rdr["Plant_address"] != DBNull.Value)
                        plant.Plant_address = rdr["Plant_address"].ToString();
                    else
                        plant.Plant_address = null;
                    if (rdr["PlantIdentificationChar"] != DBNull.Value)
                        plant.PlantIdentificationChar = Convert.ToChar(rdr["PlantIdentificationChar"]);
                    else
                        plant.PlantIdentificationChar = null;
                    plant.Company_id = rdr["company_id"].ToString(); 
                    plants.Add(plant);
                }
                return Ok(plants);
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

        // GET api/<PlantController>/5
        [HttpGet("{CompanyID}/{PlantID}")]
        public IActionResult Get(string CompanyID, string PlantID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from Plant where Company_ID = '" + CompanyID + "' and Plant_Id ='" + PlantID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                Plant plant = new Plant();
                if (rdr.Read())
                {
                    if (rdr["Plant_Id"] != DBNull.Value)
                        plant.Plant_Id = rdr["Plant_Id"].ToString();
                    else
                        plant.Plant_Id = null;
                    if (rdr["Plant_name"] != DBNull.Value)
                        plant.Plant_name = rdr["Plant_name"].ToString();
                    else
                        plant.Plant_name = null;
                    if (rdr["Plant_location"] != DBNull.Value)
                        plant.Plant_location = rdr["Plant_location"].ToString();
                    else
                        plant.Plant_location = null;
                    if (rdr["Plant_address"] != DBNull.Value)
                        plant.Plant_address = rdr["Plant_address"].ToString();
                    else
                        plant.Plant_address = null;
                    if (rdr["PlantIdentificationChar"] != DBNull.Value)
                        plant.PlantIdentificationChar = Convert.ToChar(rdr["PlantIdentificationChar"]);
                    else
                        plant.PlantIdentificationChar = null;
                    plant.Company_id = rdr["company_id"].ToString();
                }
                return Ok(plant);
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

        // POST api/<PlantController>
        [HttpPost]
        public IActionResult Post([FromBody] Plant plant)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Insert into Plant (Plant_Id, Plant_name, Plant_location,Plant_address,company_id,PlantIdentificationChar) values(@Plant_Id, @Plant_name, @Plant_location, @Plant_address, @company_id,@PlantIdentificationChar)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (plant.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = plant.Plant_Id;
                if (plant.Plant_name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_name", MySqlDbType.VarChar)).Value = plant.Plant_name;
                if (plant.Plant_location == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_location", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_location", MySqlDbType.VarChar)).Value = plant.Plant_location;
                if (plant.Plant_address == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_address", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_address", MySqlDbType.VarChar)).Value = plant.Plant_address;
                if (plant.Company_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = plant.Company_id; 
                if (plant.PlantIdentificationChar == null)
                    cmd.Parameters.Add(new MySqlParameter("@PlantIdentificationChar", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PlantIdentificationChar", MySqlDbType.VarChar)).Value = plant.PlantIdentificationChar;
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

        // PUT api/<PlantController>/5
        [HttpPut("{CompanyID}/{PlantID}")]
        public IActionResult Put(string CompanyID, string PlantID, [FromBody] Plant plant)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Update Plant set Plant_Id = @Plant_Id, Plant_name = @Plant_name, Plant_location = @Plant_location, Plant_address = @Plant_address, company_id = @company_id, PlantIdentificationChar = @PlantIdentificationChar where Company_ID = '" + CompanyID + "' and Plant_ID = '" + PlantID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (plant.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = plant.Plant_Id;
                if (plant.Plant_name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_name", MySqlDbType.VarChar)).Value = plant.Plant_name;
                if (plant.Plant_location == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_location", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_location", MySqlDbType.VarChar)).Value = plant.Plant_location;
                if (plant.Plant_address == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_address", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_address", MySqlDbType.VarChar)).Value = plant.Plant_address;
                if (plant.Company_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = plant.Company_id;
                if (plant.PlantIdentificationChar == null)
                    cmd.Parameters.Add(new MySqlParameter("@PlantIdentificationChar", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@PlantIdentificationChar", MySqlDbType.VarChar)).Value = plant.PlantIdentificationChar;
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

        // DELETE api/<PlantController>/5
        [HttpDelete("{CompanyID}/{PlantID}")]
        public IActionResult Delete(string CompanyID, string PlantID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from Plant where Company_ID = '" + CompanyID + "' and Plant_ID = '" + PlantID + "'";
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
                DataColumn dcName = new DataColumn("Plant_Id", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Plant_name", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Plant_location", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Plant_address", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Company_id", typeof(string)); 
                dt.Columns.Add(dcName);
                dcName = new DataColumn("PlantIdentificationChar", typeof(char)); 
                dt.Columns.Add(dcName);
                string Plant_Id;
                string Plant_name;
                string Plant_location;
                string Plant_address;
                string Company_id;
                string PlantIdentificationChar;
                DataFormatter formatter = new DataFormatter();
                for (int row = 0; row <= worksheet.LastRowNum; row++) //Loop the records upto filled row  
                {

                    if (worksheet.GetRow(row) != null) //null is when the row only contains empty cells   
                    {

                        Plant_Id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(0));
                        Plant_name = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(1));
                        Plant_location = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(2));
                        Plant_address = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(3));
                        Company_id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(4));
                        PlantIdentificationChar = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(5));
                        if (row.Equals(0))
                        {
                            if (Plant_Id.ToLower() != "plant code" || Plant_name.ToLower() != "plant name" ||
                                Plant_location.ToLower() != "location" || Plant_address.ToLower() != "address" || Company_id.ToLower() != "company code" || PlantIdentificationChar.ToLower() != "plant identification char")
                                return StatusCode(400, "Template heading mismatch");
                            continue;
                        }
                        DataRow ldr = dt.NewRow();
                        ldr["Plant_Id"] = Plant_Id;
                        ldr["Plant_name"] = Plant_name;
                        ldr["Plant_location"] = Plant_location;
                        ldr["Plant_address"] = Plant_address;
                        ldr["Company_id"] = Company_id;
                        ldr["PlantIdentificationChar"] = char.Parse(PlantIdentificationChar);
                        dt.Rows.Add(ldr);
                    }
                }
                //using TransactionScope scope = new TransactionScope();
                MySqlCommand cmd = new MySqlCommand("BulkUploadPlant", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = myTrans,
                    UpdatedRowSource = UpdateRowSource.None
                };
                cmd.Parameters.Add("?Plant_Id", MySqlDbType.String).SourceColumn = "Plant_Id";
                cmd.Parameters.Add("?Plant_name", MySqlDbType.String).SourceColumn = "Plant_name";
                cmd.Parameters.Add("?Plant_location", MySqlDbType.String).SourceColumn = "Plant_location";
                cmd.Parameters.Add("?Plant_address", MySqlDbType.String).SourceColumn = "Plant_address";
                cmd.Parameters.Add("?Company_id", MySqlDbType.String).SourceColumn = "Company_id";
                cmd.Parameters.Add("?PlantIdentificationChar", MySqlDbType.VarChar).SourceColumn = "PlantIdentificationChar";

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
