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
    public class ShiftController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public ShiftController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<ShiftController>
        [HttpGet("{CompanyID}/{PlantID}")]
        public IActionResult Get(string CompanyID, string PlantID, [FromQuery] int day = -1)
        {
            List<Shift> shifts = new List<Shift>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql;
                if(day == -1)
                    sql = "Select * from shift where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "'";
                else
                    sql = "Select * from shift where Company_ID = '" + CompanyID + "' and Plant_Id = '" + PlantID + "' and Day = " + day;
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Shift shift = new Shift();
                    if (rdr["Shift_id"] != DBNull.Value)
                        shift.Shift_id = Convert.ToInt32(rdr["Shift_id"]);
                    else
                        shift.Shift_id = null;
                    if (rdr["Shift_name"] != DBNull.Value)
                        shift.Shift_name = rdr["Shift_name"].ToString();
                    else
                        shift.Shift_name = null;
                    if (rdr["Start_time"] != DBNull.Value)
                        shift.Start_time = TimeSpan.Parse(rdr["Start_time"].ToString());
                    else
                        shift.Start_time = null;
                    if (rdr["End_time"] != DBNull.Value)
                        shift.End_time = TimeSpan.Parse(rdr["End_time"].ToString());
                    else
                        shift.End_time = null;
                    if (rdr["Day"] != DBNull.Value)
                        shift.Day = Convert.ToInt32(rdr["Day"]);
                    else
                        shift.Day = null;
                    shift.Plant_Id = rdr["Plant_Id"].ToString();
                    shift.Company_id = rdr["company_id"].ToString();
                    shift.DayFirstShift = Convert.ToBoolean(rdr["DayFirstShift"]);
                    shift.DayLastShift = Convert.ToBoolean(rdr["DayLastShift"]);
                    shifts.Add(shift);
                }
                return Ok(shifts);
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

        // GET api/<ShiftController>/5
        [HttpGet("{ID}")]
        public IActionResult Get(int ID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from shift where Shift_id = " + ID;
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                Shift shift = new Shift();
                if (rdr.Read())
                {
                    if (rdr["Shift_id"] != DBNull.Value)
                        shift.Shift_id = Convert.ToInt32(rdr["Shift_id"]);
                    else
                        shift.Shift_id = null;
                    if (rdr["Shift_name"] != DBNull.Value)
                        shift.Shift_name = rdr["Shift_name"].ToString();
                    else
                        shift.Shift_name = null;
                    if (rdr["Start_time"] != DBNull.Value)
                        shift.Start_time = TimeSpan.Parse(rdr["Start_time"].ToString());
                    else
                        shift.Start_time = null;
                    if (rdr["End_time"] != DBNull.Value)
                        shift.End_time = TimeSpan.Parse(rdr["End_time"].ToString());
                    else
                        shift.End_time = null;
                    if (rdr["Day"] != DBNull.Value)
                        shift.Day = Convert.ToInt32(rdr["Day"]);
                    else
                        shift.Day = null;
                    shift.Plant_Id = rdr["Plant_Id"].ToString();
                    shift.Company_id = rdr["company_id"].ToString();
                    shift.DayFirstShift = Convert.ToBoolean(rdr["DayFirstShift"]);
                    shift.DayLastShift = Convert.ToBoolean(rdr["DayLastShift"]);
                }
                return Ok(shift);
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

        // POST api/<ShiftController>
        [HttpPost]
        public IActionResult Post([FromBody] ShiftStrTime shift)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Insert into shift (Shift_name, Start_time, End_time, Day, Plant_Id, Company_id,DayFirstShift,DayLastShift) values(@Shift_name, @Start_time, @End_time, @Day, @Plant_Id, @Company_id,@DayFirstShift,@DayLastShift)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (shift.Shift_name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Shift_name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Shift_name", MySqlDbType.VarChar)).Value = shift.Shift_name;
                if (shift.Start_time == null)
                    cmd.Parameters.Add(new MySqlParameter("@Start_time", MySqlDbType.Time)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Start_time", MySqlDbType.Time)).Value = TimeSpan.Parse(shift.Start_time);
                if (shift.End_time == null)
                    cmd.Parameters.Add(new MySqlParameter("@End_time", MySqlDbType.Time)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@End_time", MySqlDbType.Time)).Value = TimeSpan.Parse(shift.End_time);
                if (shift.Day == null)
                    cmd.Parameters.Add(new MySqlParameter("@Day", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Day", MySqlDbType.Int32)).Value = shift.Day;
                if (shift.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = shift.Plant_Id;
                if (shift.Company_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = shift.Company_id;
                cmd.Parameters.Add(new MySqlParameter("@DayFirstShift", MySqlDbType.Int16)).Value = Convert.ToInt16(shift.DayFirstShift);
                cmd.Parameters.Add(new MySqlParameter("@DayLastShift", MySqlDbType.Int16)).Value = Convert.ToInt16(shift.DayLastShift);
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

        // PUT api/<ShiftController>/5
        [HttpPut("{ID}")]
        public IActionResult Put(int ID, [FromBody] ShiftStrTime shift)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Update shift set Shift_name = @Shift_name, Start_time = @Start_time, End_time = @End_time, Day = @Day, Plant_Id = @Plant_Id, Company_id = @Company_id,DayFirstShift = @DayFirstShift, DayLastShift = @DayLastShift where Shift_id = " + ID;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (shift.Shift_name == null)
                    cmd.Parameters.Add(new MySqlParameter("@Shift_name", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Shift_name", MySqlDbType.VarChar)).Value = shift.Shift_name;
                if (shift.Start_time == null)
                    cmd.Parameters.Add(new MySqlParameter("@Start_time", MySqlDbType.Time)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Start_time", MySqlDbType.Time)).Value = TimeSpan.Parse(shift.Start_time);
                if (shift.End_time == null)
                    cmd.Parameters.Add(new MySqlParameter("@End_time", MySqlDbType.Time)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@End_time", MySqlDbType.Time)).Value = TimeSpan.Parse(shift.End_time);
                if (shift.Day == null)
                    cmd.Parameters.Add(new MySqlParameter("@Day", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Day", MySqlDbType.Int32)).Value = shift.Day;
                if (shift.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = shift.Plant_Id;
                if (shift.Company_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = shift.Company_id;
                cmd.Parameters.Add(new MySqlParameter("@DayFirstShift", MySqlDbType.Int16)).Value = Convert.ToInt16(shift.DayFirstShift);
                cmd.Parameters.Add(new MySqlParameter("@DayLastShift", MySqlDbType.Int16)).Value = Convert.ToInt16(shift.DayLastShift);
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

        // DELETE api/<ShiftController>/5
        [HttpDelete("{ID}")]
        public IActionResult Delete(int ID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Delete from shift where Shift_id = " + ID;
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
                return StatusCode(400, "Invalid URL");
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
                DataColumn dcName = new DataColumn("Shift_name", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Start_time", typeof(TimeSpan));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("End_time", typeof(TimeSpan));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Day", typeof(Int16));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Plant_Id", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Company_id", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("DayFirstShift", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("DayLastShift", typeof(string));
                dt.Columns.Add(dcName);
                string Shift_name;
                string Start_time;
                string End_time;
                string Day;
                string Plant_Id;
                string Company_id;
                string DayFirstShift;
                string DayLastShift;
                DataFormatter formatter = new DataFormatter();
                for (int row = 0; row <= worksheet.LastRowNum; row++) //Loop the records upto filled row  
                {
                    if (worksheet.GetRow(row) != null) //null is when the row only contains empty cells   
                    {

                        Shift_name = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(0));
                        Start_time = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(1));
                        End_time = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(2));
                        Day = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(3));
                        Plant_Id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(4));
                        Company_id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(5));
                        DayFirstShift = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(6));
                        DayLastShift = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(7));
                        if (row.Equals(0))
                        {
                            if (Shift_name.ToLower() != "shift name" || Start_time.ToLower() != "start time" ||
                                End_time.ToLower() != "end time" || Day.ToLower() != "day" || Plant_Id.ToLower() != "plant code" || Company_id.ToLower() != "company code" ||
                                DayFirstShift.ToLower() != "first shift" || DayLastShift.ToLower() != "last shift")
                                return StatusCode(400, "Template heading mismatch");
                            continue;
                        }
                        DataRow ldr = dt.NewRow();
                        ldr["Shift_name"] = Shift_name;
                        ldr["Start_time"] = Start_time;
                        ldr["End_time"] = End_time;
                        ldr["Day"] = Convert.ToInt16(Day);
                        ldr["Plant_Id"] = Plant_Id;
                        ldr["Company_id"] = Company_id;
                        ldr["DayFirstShift"] = Convert.ToInt16(DayFirstShift);
                        ldr["DayLastShift"] = Convert.ToInt16(DayLastShift);
                        dt.Rows.Add(ldr);
                    }
                }
                //using TransactionScope scope = new TransactionScope();
                MySqlCommand cmd = new MySqlCommand("BulkUploadShift", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = myTrans,
                    UpdatedRowSource = UpdateRowSource.None
                };
                cmd.Parameters.Add("?Shift_name", MySqlDbType.String).SourceColumn = "Shift_name";
                cmd.Parameters.Add("?Start_time", MySqlDbType.Time).SourceColumn = "Start_time";
                cmd.Parameters.Add("?End_time", MySqlDbType.Time).SourceColumn = "End_time";
                cmd.Parameters.Add("?Days", MySqlDbType.Int16).SourceColumn = "Day";
                cmd.Parameters.Add("?Plant_Id", MySqlDbType.String).SourceColumn = "Plant_Id";
                cmd.Parameters.Add("?Company_id", MySqlDbType.String).SourceColumn = "Company_id";
                cmd.Parameters.Add("?DayFirstShift", MySqlDbType.Int16).SourceColumn = "DayFirstShift";
                cmd.Parameters.Add("?DayLastShift", MySqlDbType.Int16).SourceColumn = "DayLastShift";

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
