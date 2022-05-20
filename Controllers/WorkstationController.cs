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
    public class WorkstationController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public WorkstationController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<WorkstationController>
        [HttpGet("{CompanyID}/{PlantID}")]
        public IActionResult Get(string CompanyID, string PlantID)
        {
            List<Workstation> wss = new List<Workstation>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from workstation where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Workstation ws = new Workstation();
                    if (rdr["Workstation_Id"] != DBNull.Value)
                        ws.Workstation_Id = Convert.ToInt32(rdr["Workstation_Id"]);
                    else
                        ws.Workstation_Id = null;
                    if (rdr["Workstation_name"] != DBNull.Value)
                        ws.Workstation_name = rdr["Workstation_name"].ToString();
                    else
                        ws.Workstation_name = null;
                    if (rdr["Description"] != DBNull.Value)
                        ws.Description = rdr["Description"].ToString();
                    else
                        ws.Description = null;
                    if (rdr["Location"] != DBNull.Value)
                        ws.Location = rdr["Location"].ToString();
                    else
                        ws.Location = null;
                    ws.Plant_Id = rdr["Plant_Id"].ToString();
                    ws.Company_Id = rdr["company_Id"].ToString();
                    wss.Add(ws);
                }
                return Ok(wss);
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

        // GET api/<WorkstationController>/5
        [HttpGet("{WID}")]
        public IActionResult Get(int WID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from workstation where Workstation_Id = " + WID;
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                Workstation ws = new Workstation();
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (rdr["Workstation_Id"] != DBNull.Value)
                        ws.Workstation_Id = Convert.ToInt32(rdr["Workstation_Id"]);
                    else
                        ws.Workstation_Id = null;
                    if (rdr["Workstation_name"] != DBNull.Value)
                        ws.Workstation_name = rdr["Workstation_name"].ToString();
                    else
                        ws.Workstation_name = null;
                    if (rdr["Description"] != DBNull.Value)
                        ws.Description = rdr["Description"].ToString();
                    else
                        ws.Description = null;
                    if (rdr["Location"] != DBNull.Value)
                        ws.Location = rdr["Location"].ToString();
                    else
                        ws.Location = null;
                    ws.Plant_Id = rdr["Plant_Id"].ToString();
                    ws.Company_Id = rdr["company_Id"].ToString();
                }
                return Ok(ws);
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

        // POST api/<WorkstationController>
        [HttpPost]
        public IActionResult Post([FromBody] Workstation ws)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Insert into workstation (Workstation_name, Description, Location, Plant_Id, Company_Id) values(@Workstation_name, @Description, @Location, @Plant_Id, @Company_Id)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (ws.Workstation_name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Workstation_name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Workstation_name", MySqlDbType.VarChar)).Value = ws.Workstation_name;
                if (ws.Description == null)
                    cmd.Parameters.Add(new MySqlParameter("@Description", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Description", MySqlDbType.VarChar)).Value = ws.Description;
                if (ws.Location == null)
                    cmd.Parameters.Add(new MySqlParameter("@Location", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Location", MySqlDbType.VarChar)).Value = ws.Location;
                if (ws.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = ws.Plant_Id;
                if (ws.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = ws.Company_Id;
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

        // PUT api/<WorkstationController>/5
        [HttpPut("{WID}")]
        public IActionResult Put(int WID, [FromBody] Workstation ws)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Update Workstation set  Workstation_Id = @Workstation_Id, Workstation_name = @Workstation_name, Description = @Description, Location = @Location, Plant_Id = @Plant_Id, Company_Id = @Company_Id where Workstation_Id = " + WID;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (ws.Workstation_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Workstation_Id", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Workstation_Id", MySqlDbType.Int32)).Value = ws.Workstation_Id;
                if (ws.Workstation_name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Workstation_name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Workstation_name", MySqlDbType.VarChar)).Value = ws.Workstation_name;
                if (ws.Description == null)
                    cmd.Parameters.Add(new MySqlParameter("@Description", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Description", MySqlDbType.VarChar)).Value = ws.Description;
                if (ws.Location == null)
                    cmd.Parameters.Add(new MySqlParameter("@Location", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Location", MySqlDbType.VarChar)).Value = ws.Location;
                if (ws.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = ws.Plant_Id;
                if (ws.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = ws.Company_Id;
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

        // DELETE api/<WorkstationController>/5
        [HttpDelete("{WID}")]
        public IActionResult Delete(int WID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from Workstation where Workstation_Id = " + WID;
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
                    DataColumn dcName = new DataColumn("Workstation_name", typeof(string));
                    dt.Columns.Add(dcName);
                    dcName = new DataColumn("Description", typeof(string));
                    dt.Columns.Add(dcName);
                    dcName = new DataColumn("Location", typeof(string));
                    dt.Columns.Add(dcName);
                    dcName = new DataColumn("Plant_Id", typeof(string));
                    dt.Columns.Add(dcName);
                    dcName = new DataColumn("Company_id", typeof(string));
                    dt.Columns.Add(dcName);
                    string Workstation_name;
                    string Description;
                    string Location;
                    string Plant_Id;
                    string Company_id;
                    DataFormatter formatter = new DataFormatter();
                    for (int row = 0; row <= worksheet.LastRowNum; row++) //Loop the records upto filled row  
                    {
                        if (worksheet.GetRow(row) != null) //null is when the row only contains empty cells   
                        {

                            Workstation_name = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(0));
                            Description = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(1));
                            Location = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(2));
                            Plant_Id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(3));
                            Company_id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(4));
                            if (row.Equals(0))
                            {
                                if (Workstation_name.ToLower() != "workstation name" || Description.ToLower() != "description" ||
                                    Location.ToLower() != "location" || Plant_Id.ToLower() != "plant code" || Company_id.ToLower() != "company code")
                                    return StatusCode(400, "Template heading mismatch");
                                continue;
                            }
                            DataRow ldr = dt.NewRow();
                            ldr["Workstation_name"] = Workstation_name;
                            ldr["Description"] = Description;
                            ldr["Location"] = Location;
                            ldr["Plant_Id"] = Plant_Id;
                            ldr["Company_id"] = Company_id;
                            dt.Rows.Add(ldr);
                        }
                    }
                    //using TransactionScope scope = new TransactionScope() ;
                    MySqlCommand cmd = new MySqlCommand("BulkUploadWorkstation", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Transaction = myTrans,
                        UpdatedRowSource = UpdateRowSource.None
                    };
                    cmd.Parameters.Add("?Workstation_name", MySqlDbType.String).SourceColumn = "Workstation_name";
                    cmd.Parameters.Add("?Description", MySqlDbType.String).SourceColumn = "Description";
                    cmd.Parameters.Add("?Location", MySqlDbType.String).SourceColumn = "Location";
                    cmd.Parameters.Add("?Plant_Id", MySqlDbType.String).SourceColumn = "Plant_Id";
                    cmd.Parameters.Add("?Company_id", MySqlDbType.String).SourceColumn = "Company_id";

                    MySqlDataAdapter da = new MySqlDataAdapter()
                    { InsertCommand = cmd};
                    //da.UpdateBatchSize = 100;
                    int records = da.Update(dt);
                    //scope.Complete();
                    myTrans.Commit();


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
