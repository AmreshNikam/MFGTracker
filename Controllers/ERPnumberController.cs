using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
    public class ERPnumberController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public ERPnumberController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<ERPnumberController>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Select * from erpnumber where ERPNumberID = " + id;
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                ERPnumber eR = new ERPnumber();
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    if (rdr["ERPNumberID"] != DBNull.Value)
                        eR.ERPNumberID = Convert.ToInt32(rdr["ERPNumberID"]);
                    else
                        eR.ERPNumberID = null;
                    if (rdr["Subpart_id"] != DBNull.Value)
                        eR.Subpart_id = Convert.ToInt32(rdr["Subpart_id"]);
                    else
                        eR.Subpart_id = null;
                    if (rdr["ColorID"] != DBNull.Value)
                        eR.ColorID = Convert.ToInt32(rdr["ColorID"]);
                    else
                        eR.ColorID = null;
                    if (rdr["AssemblyID"] != DBNull.Value)
                        eR.AssemblyID = Convert.ToInt32(rdr["AssemblyID"]);
                    else
                        eR.AssemblyID = null;
                    if (rdr["ERPNumber"] != DBNull.Value)
                        eR.ERPNumber = rdr["ERPNumber"].ToString();
                    else
                        eR.ERPNumber = null;
                    if (rdr["SAPMaterialNumber"] != DBNull.Value)
                        eR.SAPMaterialNumber = rdr["SAPMaterialNumber"].ToString();
                    else
                        eR.SAPMaterialNumber = null;
                    eR.Plant_Id = rdr["Plant_Id"].ToString();
                    eR.Company_Id = rdr["company_id"].ToString();
                }
                return Ok(eR);
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

        // GET api/<ERPnumberController>/5
        [HttpGet("{CompanyID}/{PlantID}")]
        public IActionResult Get(string CompanyID, string PlantID, [FromQuery] int ModelID = -1)
        {
            List<ERPnumberDetails> eRPnumbers = new List<ERPnumberDetails>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql;
                if (ModelID == -1)
                    sql = "SELECT ER.ERPNumberID,  MD.Part_id, MD.Part_name, ER.Subpart_id, SM.Subpart_Name, ER.ColorID, " +
                          "CO.ColorcolName, ER.AssemblyID, AM.AssemblyName, ER.ERPNumber,  ER.SAPMaterialNumber, ER.Plant_Id,ER.Company_Id " +
                          "FROM erpnumber as ER " +
                          "LEFT JOIN assembly as AM " +
                          "ON ER.AssemblyID = AM.AssemblyID " +
                          "LEFT JOIN color as CO " +
                          "ON ER.ColorID = CO.ColorID " +
                          "LEFT JOIN sub_model as SM " +
                          "ON ER.Subpart_id = SM.Subpart_id " +
                          "LEFT JOIN model MD " +
                          "ON SM.Part_id = MD.Part_id " +
                          "WHERE ER.Company_Id = '" + CompanyID + "' and ER.Plant_Id = '" + PlantID + "'";
                else
                    sql = "SELECT ER.ERPNumberID,  MD.Part_id, MD.Part_name, ER.Subpart_id, SM.Subpart_Name, ER.ColorID, " +
                          "CO.ColorcolName, ER.AssemblyID, AM.AssemblyName, ER.ERPNumber,  ER.SAPMaterialNumber, ER.Plant_Id,ER.Company_Id " +
                          "FROM erpnumber as ER " +
                          "LEFT JOIN assembly as AM " +
                          "ON ER.AssemblyID = AM.AssemblyID " +
                          "LEFT JOIN color as CO " +
                          "ON ER.ColorID = CO.ColorID " +
                          "LEFT JOIN sub_model as SM " +
                          "ON ER.Subpart_id = SM.Subpart_id " +
                          "LEFT JOIN model MD " +
                          "ON SM.Part_id = MD.Part_id " +
                          "WHERE ER.Company_Id = '" + CompanyID + "' and ER.Plant_Id = '" + PlantID + "' and ER.Subpart_id in (select Subpart_id from sub_model where Part_id = " + ModelID + ")";
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ERPnumberDetails eR = new ERPnumberDetails();
                    if (rdr["ERPNumberID"] != DBNull.Value)
                        eR.ERPNumberID = Convert.ToInt32(rdr["ERPNumberID"]);
                    else
                        eR.ERPNumberID = null;
                    if (rdr["Part_id"] != DBNull.Value)
                        eR.Part_id = Convert.ToInt32(rdr["Part_id"]);
                    else
                        eR.Part_id = null;
                    eR.Part_name = rdr["Part_name"].ToString();
                    if (rdr["Subpart_id"] != DBNull.Value)
                        eR.Subpart_id = Convert.ToInt32(rdr["Subpart_id"]);
                    else
                        eR.Subpart_id = null;
                    eR.Subpart_Name = rdr["Subpart_Name"].ToString();
                    if (rdr["ColorID"] != DBNull.Value)
                        eR.ColorID = Convert.ToInt32(rdr["ColorID"]);
                    else
                        eR.ColorID = null;
                    eR.ColorcolName = rdr["ColorcolName"].ToString();
                    if (rdr["AssemblyID"] != DBNull.Value)
                        eR.AssemblyID = Convert.ToInt32(rdr["AssemblyID"]);
                    else
                        eR.AssemblyID = null;
                    eR.AssemblyName = rdr["AssemblyName"].ToString();
                    if (rdr["ERPNumber"] != DBNull.Value)
                        eR.ERPNumber = rdr["ERPNumber"].ToString();
                    else
                        eR.ERPNumber = null;
                    if (rdr["SAPMaterialNumber"] != DBNull.Value)
                        eR.SAPMaterialNumber = rdr["SAPMaterialNumber"].ToString();
                    else
                        eR.SAPMaterialNumber = null;
                    eR.Plant_Id = rdr["Plant_Id"].ToString();
                    eR.Company_Id = rdr["company_id"].ToString();
                    eRPnumbers.Add(eR);
                }
                return Ok(eRPnumbers);
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
        //Get
        [HttpGet("Template")]
        public IActionResult Get([FromQuery] string PLANTID, string COMPANYID)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            int rowcount = 0;
            try
            {
                connection.Open();
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("ERP Number");
                IRow row = sheet.CreateRow(rowcount);
                XSSFCellStyle cellstyle = (XSSFCellStyle)workbook.CreateCellStyle();
                cellstyle.BorderBottom = BorderStyle.Thin;
                cellstyle.BorderLeft = BorderStyle.Thin;
                cellstyle.BorderRight = BorderStyle.Thin;
                cellstyle.BorderTop = BorderStyle.Thin;
                cellstyle.FillForegroundColor = IndexedColors.Yellow.Index;
                cellstyle.FillPattern = FillPattern.SolidForeground;
                ICell cell = row.CreateCell(0);
                cell.CellStyle = cellstyle;
                cell.SetCellValue("Company Name");
                cell = row.CreateCell(1);
                cell.CellStyle = cellstyle;
                cell.SetCellValue("Model_Code");
                cell = row.CreateCell(2);
                cell.CellStyle = cellstyle;
                cell.SetCellValue("Model_Name");
                cell = row.CreateCell(3);
                cell.CellStyle = cellstyle;
                cell.SetCellValue("SubModel_Code");
                cell = row.CreateCell(4);
                cell.CellStyle = cellstyle;
                cell.SetCellValue("SubModel_Name");
                cell = row.CreateCell(5);
                cell.CellStyle = cellstyle;
                cell.SetCellValue("Color_Code");
                cell = row.CreateCell(6);
                cell.CellStyle = cellstyle;
                cell.SetCellValue("Color_Name");
                cell = row.CreateCell(7);
                cell.CellStyle = cellstyle;
                cell.SetCellValue("Assembly_Code");
                cell = row.CreateCell(8);
                cell.CellStyle = cellstyle;
                cell.SetCellValue("Assembly_Name");
                cell = row.CreateCell(9);
                cell.CellStyle = cellstyle;
                cell.SetCellValue("ERP Number");
                cell = row.CreateCell(10);
                cell.CellStyle = cellstyle;
                cell.SetCellValue("SAP Material Number");
                cell = row.CreateCell(11);
                cell.CellStyle = cellstyle;
                cell.SetCellValue("Plant");
                cell = row.CreateCell(12);
                cell.CellStyle = cellstyle;
                cell.SetCellValue("Company");
                string sql = "Select * from sapmaterialnumber_template where Plant = '" + PLANTID + "' and Company = '" + COMPANYID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                ERPnumber eR = new ERPnumber();
                MySqlDataReader rdr = cmd.ExecuteReader();
                XSSFCellStyle cellstyle2 = (XSSFCellStyle)workbook.CreateCellStyle();
                cellstyle2.BorderBottom = BorderStyle.Thin;
                cellstyle2.BorderLeft = BorderStyle.Thin;
                cellstyle2.BorderRight = BorderStyle.Thin;
                cellstyle2.BorderTop = BorderStyle.Thin;
                while (rdr.Read())
                {
                    rowcount++;
                    row = sheet.CreateRow(rowcount);
                    cell = row.CreateCell(0);
                    cell.CellStyle = cellstyle2;
                    if (rdr["CompanyName"] != DBNull.Value)
                        cell.SetCellValue(rdr["CompanyName"].ToString());
                    cell = row.CreateCell(1);
                    cell.CellStyle = cellstyle2;
                    if (rdr["Model_Code"] != DBNull.Value)
                        cell.SetCellValue(Convert.ToInt32(rdr["Model_Code"]));
                    cell = row.CreateCell(2);
                    cell.CellStyle = cellstyle2;
                    if (rdr["Model_Name"] != DBNull.Value)
                        cell.SetCellValue(rdr["Model_Name"].ToString());
                    cell = row.CreateCell(3);
                    cell.CellStyle = cellstyle2;
                    if (rdr["SubModel_Code"] != DBNull.Value)
                        cell.SetCellValue(Convert.ToInt32(rdr["SubModel_Code"]));
                    cell = row.CreateCell(4);
                    cell.CellStyle = cellstyle2;
                    if (rdr["SubModel_Name"] != DBNull.Value)
                        cell.SetCellValue(rdr["SubModel_Name"].ToString());
                    cell = row.CreateCell(5);
                    cell.CellStyle = cellstyle2;
                    if (rdr["Color_Code"] != DBNull.Value)
                        cell.SetCellValue(Convert.ToInt32(rdr["Color_Code"]));
                    cell = row.CreateCell(6);
                    cell.CellStyle = cellstyle2;
                    if (rdr["Color_Name"] != DBNull.Value)
                        cell.SetCellValue(rdr["Color_Name"].ToString());
                    cell = row.CreateCell(7);
                    cell.CellStyle = cellstyle2;
                    if (rdr["Assembly_Code"] != DBNull.Value)
                        cell.SetCellValue(Convert.ToInt32(rdr["Assembly_Code"]));
                    cell = row.CreateCell(8);
                    cell.CellStyle = cellstyle2;
                    if (rdr["Assembly_Name"] != DBNull.Value)
                        cell.SetCellValue(rdr["Assembly_Name"].ToString());
                    cell = row.CreateCell(9);
                    cell.CellStyle = cellstyle2;
                    cell.SetCellValue("");
                    cell = row.CreateCell(10);
                    cell.CellStyle = cellstyle2;
                    cell.SetCellValue("");
                    cell = row.CreateCell(11);
                    cell.CellStyle = cellstyle2;
                    if (rdr["Plant"] != DBNull.Value)
                        cell.SetCellValue(rdr["Plant"].ToString());
                    cell = row.CreateCell(12);
                    cell.CellStyle = cellstyle2;
                    if (rdr["Company"] != DBNull.Value)
                        cell.SetCellValue(rdr["Company"].ToString());
                }
                for (int i = 0; i < 13; i++)
                    sheet.AutoSizeColumn(i);
                MemoryStream ms = new MemoryStream();
                workbook.Write(ms);
                // Sending the server processed data back to the user computer...
                return File(ms.ToArray(), "application/vnd.ms-excel", "Template.xlsx");
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
        // POST api/<ERPnumberController>
        [HttpPost]
        public IActionResult Post([FromBody] ERPnumber er)
        {
            string sql;
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string Subpart_id, ColorID, AssemblyID;
                if (er.Subpart_id == null)
                    Subpart_id = "Subpart_id is null";
                else
                    Subpart_id = "Subpart_id = " + er.Subpart_id;
                if (er.ColorID == null)
                    ColorID = "ColorID is null";
                else
                    ColorID = "ColorID = " + er.ColorID;
                if (er.AssemblyID == null)
                    AssemblyID = "AssemblyID is null";
                else
                    AssemblyID = "AssemblyID = " + er.AssemblyID;
                sql = "select count(*) as Count from erpnumber where " + Subpart_id + " and " + ColorID + " and " + AssemblyID + " and  Plant_Id = '" + er.Plant_Id + "' and Company_Id = '" + er.Company_Id + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                int count = 0;
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                    count = Convert.ToInt32(rdr["Count"]);
                rdr.Close();
                if(count > 0)
                    return StatusCode(700, "Record already exist");
                sql = "Insert into erpnumber (Subpart_id,ColorID,AssemblyID,ERPNumber,SAPMaterialNumber,Plant_Id,Company_Id) values(@Subpart_id,@ColorID,@AssemblyID,@ERPNumber,@SAPMaterialNumber,@Plant_Id,@Company_Id)";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (er.Subpart_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_id", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_id", MySqlDbType.Int32)).Value = er.Subpart_id;
                if (er.ColorID == null)
                    cmd.Parameters.Add(new MySqlParameter("@ColorID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ColorID", MySqlDbType.Int32)).Value = er.ColorID;
                if (er.AssemblyID == null)
                    cmd.Parameters.Add(new MySqlParameter("@AssemblyID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@AssemblyID", MySqlDbType.Int32)).Value = er.AssemblyID;
                if (er.ERPNumber == null)
                    cmd.Parameters.Add(new MySqlParameter("@ERPNumber", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ERPNumber", MySqlDbType.VarChar)).Value = er.ERPNumber;
                if (er.SAPMaterialNumber == null)
                    cmd.Parameters.Add(new MySqlParameter("@SAPMaterialNumber", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@SAPMaterialNumber", MySqlDbType.VarChar)).Value = er.SAPMaterialNumber;
                if (er.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = er.Plant_Id;
                if (er.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = er.Company_Id;
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

        // PUT api/<ERPnumberController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ERPnumber er)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Update erpnumber set ERPNumberID = @ERPNumberID, Subpart_id = @Subpart_id, ColorID = @ColorID, AssemblyID = @AssemblyID, ERPNumber = @ERPNumber, SAPMaterialNumber = @SAPMaterialNumber, Plant_Id = @Plant_Id, Company_Id = @Company_Id where ERPNumberID =" + id;
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                if (er.ERPNumberID == null)
                    cmd.Parameters.Add(new MySqlParameter("@ERPNumberID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ERPNumberID", MySqlDbType.Int32)).Value = er.ERPNumberID;
                if (er.Subpart_id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_id", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Subpart_id", MySqlDbType.Int32)).Value = er.Subpart_id;
                if (er.ColorID == null)
                    cmd.Parameters.Add(new MySqlParameter("@ColorID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ColorID", MySqlDbType.Int32)).Value = er.ColorID;
                if (er.AssemblyID == null)
                    cmd.Parameters.Add(new MySqlParameter("@AssemblyID", MySqlDbType.Int32)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@AssemblyID", MySqlDbType.Int32)).Value = er.AssemblyID;
                if (er.ERPNumber == null)
                    cmd.Parameters.Add(new MySqlParameter("@ERPNumber", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@ERPNumber", MySqlDbType.VarChar)).Value = er.ERPNumber;
                if (er.SAPMaterialNumber == null)
                    cmd.Parameters.Add(new MySqlParameter("@SAPMaterialNumber", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@SAPMaterialNumber", MySqlDbType.VarChar)).Value = er.SAPMaterialNumber;
                if (er.Plant_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = er.Plant_Id;
                if (er.Company_Id == null)
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = DBNull.Value;
                else
                    cmd.Parameters.Add(new MySqlParameter("@company_id", MySqlDbType.VarChar)).Value = er.Company_Id;
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

        // DELETE api/<ERPnumberController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            
            try
            {
                connection.Open();
                string sql = "Delete from erpnumber where ERPNumberID = "+ id;
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
                DataColumn dcName = new DataColumn("Subpart_id", typeof(int));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("ColorID", typeof(int));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("AssemblyID", typeof(int));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("ERPNumber", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("SAPMaterialNumber", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Plant_Id", typeof(string));
                dt.Columns.Add(dcName);
                dcName = new DataColumn("Company_Id", typeof(string));
                dt.Columns.Add(dcName);

                string Subpart_id;
                string ColorID;
                string AssemblyID;
                string ERPNumber;
                string SAPMaterialNumber;
                string Plant_Id;
                string Company_Id;
                DataFormatter formatter = new DataFormatter();
                for (int row = 0; row <= worksheet.LastRowNum; row++) //Loop the records upto filled row  
                {
                    if (worksheet.GetRow(row) != null) //null is when the row only contains empty cells   
                    {

                        Subpart_id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(3));
                        ColorID = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(5));
                        AssemblyID = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(7));
                        ERPNumber = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(9));
                        SAPMaterialNumber = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(10));
                        Plant_Id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(11));
                        Company_Id = formatter.FormatCellValue(worksheet.GetRow(row).GetCell(12));
                        if (row.Equals(0))
                        {
                            if (Subpart_id.ToLower() != "submodel_code" || ColorID.ToLower() != "color_code" || AssemblyID.ToLower() != "assembly_code" ||
                                ERPNumber.ToLower() != "erp number" || SAPMaterialNumber.ToLower() != "sap material number" || Plant_Id.ToLower() != "plant" || Company_Id.ToLower() != "company")
                                return StatusCode(700, "Template heading mismatch");
                            continue;
                        }
                        DataRow ldr = dt.NewRow();
                        if (string.IsNullOrEmpty(Subpart_id))
                            ldr["Subpart_id"] = DBNull.Value;
                        else
                            ldr["Subpart_id"] = Convert.ToInt32(Subpart_id);
                        if (string.IsNullOrEmpty(ColorID))
                            ldr["ColorID"] = DBNull.Value;
                        else
                            ldr["ColorID"] = Convert.ToInt32(ColorID);
                        if (string.IsNullOrEmpty(AssemblyID))
                            ldr["AssemblyID"] = DBNull.Value;
                        else
                            ldr["AssemblyID"] = Convert.ToInt32(AssemblyID);
                        if (string.IsNullOrEmpty(ERPNumber))
                            ldr["ERPNumber"] = DBNull.Value;
                        else
                            ldr["ERPNumber"] = ERPNumber;
                        if (string.IsNullOrEmpty(SAPMaterialNumber))
                            ldr["ERPNumber"] = DBNull.Value;
                        else
                            ldr["SAPMaterialNumber"] = SAPMaterialNumber;
                        ldr["Plant_Id"] = Plant_Id;
                        ldr["Company_Id"] = Company_Id;
                        dt.Rows.Add(ldr);
                    }
                }
                //using TransactionScope scope = new TransactionScope();
                MySqlCommand cmd = new MySqlCommand("BulkUploadErpnumber", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    Transaction = myTrans,
                    UpdatedRowSource = UpdateRowSource.None
                };
                cmd.Parameters.Add("?Subpart_id", MySqlDbType.Int32).SourceColumn = "Subpart_id";
                cmd.Parameters.Add("?ColorID", MySqlDbType.Int32).SourceColumn = "ColorID";
                cmd.Parameters.Add("?AssemblyID", MySqlDbType.Int32).SourceColumn = "AssemblyID";
                cmd.Parameters.Add("?ERPNumber", MySqlDbType.String).SourceColumn = "ERPNumber";
                cmd.Parameters.Add("?SAPMaterialNumber", MySqlDbType.String).SourceColumn = "SAPMaterialNumber";
                cmd.Parameters.Add("?Plant_Id", MySqlDbType.String).SourceColumn = "Plant_Id";
                cmd.Parameters.Add("?Company_Id", MySqlDbType.String).SourceColumn = "Company_Id";
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
