using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using MFG_Tracker.DatabaseTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ZXing;
using ZXing.QrCode;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MFG_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public ReportsController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<ReportsController>
        [HttpGet("Master/ModelList")]
        //*************************************Master***************************
        public IActionResult GetModelList([FromQuery] string COMPANYID, string PLANTID)
        {
            List<ModelList> modelLists = new List<ModelList>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "select BP.Name, M.Part_name, SM.Customer_material_number,  SM.Subpart_Name, SM.Tag_time " +
                             "from model as M " +
                             "left join business_partner as BP on M.BP_Short_code = BP.BP_Short_code and M.Plant_Id = BP.Plant_Id and M.Company_id = BP.Company_id " +
                             "inner join sub_model as SM on M.Part_id = SM.Part_id " +
                             "where M.company_id = '" + COMPANYID + "' and M.Plant_Id = '" + PLANTID + "' " +
                             "order by BP.Name";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ModelList modelList = new ModelList();
                    modelList.Name = rdr["Name"].ToString();
                    modelList.Part_name = rdr["Part_name"].ToString();
                    modelList.Customer_material_number = rdr["Customer_material_number"].ToString();
                    modelList.Subpart_Name = rdr["Subpart_Name"].ToString();
                    if (rdr["Tag_time"] == DBNull.Value)
                        modelList.Tag_time = null;
                    else
                        modelList.Tag_time = Convert.ToInt32(rdr["Tag_time"]);
                    
                    modelLists.Add(modelList);
                }
                return Ok(modelLists);
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
        [HttpGet("Master/InspectionList")]

        public IActionResult GetInspectionList([FromQuery] string COMPANYID, string PLANTID,bool BRORQR = true, bool PDF = false)
        {
            List<InspectionList> inspectionLists = new List<InspectionList>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "select DC.DefectbarCode, DC.Message, WS1.Workstation_name as From_Station, WS2.Workstation_name as To_Station " +
                             "from defect_code as DC " +
                             "left join workstation as WS1 on DC.From_Station = WS1.Workstation_Id " +
                             "left join workstation as WS2 on DC.To_Station = WS2.Workstation_Id " +
                             "where DC.company_id = '" + COMPANYID + "' and DC.Plant_Id = '" + PLANTID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    InspectionList inspectionList = new InspectionList();
                    inspectionList.DefectbarCode = rdr["DefectbarCode"].ToString();
                    inspectionList.Message = rdr["Message"].ToString();
                    inspectionList.From_Station = rdr["From_Station"].ToString();
                    inspectionList.To_Station = rdr["To_Station"].ToString();
                    inspectionLists.Add(inspectionList);
                    ZXing.BarcodeFormat iFormat;
                    if (BRORQR)
                        iFormat = ZXing.BarcodeFormat.CODE_128;
                    else
                        iFormat = ZXing.BarcodeFormat.QR_CODE;

                    BarcodeWriter qrCodeWriter = new BarcodeWriter
                    {

                        Format = iFormat,
                        Options = new QrCodeEncodingOptions
                        {
                            Height = 50,
                            Width = 250,
                            Margin = 0,
                            PureBarcode = false
                        }

                    };
                    var btmap = qrCodeWriter.Write(inspectionList.DefectbarCode);
                    MemoryStream ms = new MemoryStream();
                    btmap.Save(ms, ImageFormat.Jpeg);
                    byte[] byteImage = ms.ToArray();
                    var SigBase64 = Convert.ToBase64String(byteImage);
                    inspectionList.Image = "data:image/jpeg;base64," + SigBase64;
                }
                if(!PDF)
                    return Ok(inspectionLists);
                else
                {
                    List<byte[]> files = new List<byte[]>();
                    List<string> filename = new List<string>();
                    List<string> folder = new List<string>();
                    var groupedList = inspectionLists.GroupBy(u => new { u.From_Station, u.To_Station });
                    foreach (var group in groupedList)
                    {

                        HeaderFooterDefectCode HF = new HeaderFooterDefectCode();
                        HF.FromStation = group.Key.From_Station;
                        HF.ToStation = group.Key.To_Station;
                        iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
                        using (MemoryStream ms = new MemoryStream())
                        using (Document doc = new Document(rec, 36, 36, 36, 36))
                        using (PdfWriter pdfWriter = PdfWriter.GetInstance(doc, ms))
                        {
                            pdfWriter.PageEvent = HF;
                            doc.Open();
                            PdfPTable table = new PdfPTable(2);
                            //Console.WriteLine("Users from " + group.Key.From_Station + " at the age of " + group.Key.To_Station + ":");
                            foreach (var info in group)
                            {
                                iTextSharp.text.Image bm = null;
                                if (BRORQR)
                                {
                                    Barcode128 bc = new Barcode128();
                                    bc.TextAlignment = Element.ALIGN_CENTER;
                                    bc.Code = info.DefectbarCode;
                                    bc.StartStopText = false;
                                    bc.CodeType = Barcode128.CODE128;
                                    bc.Extended = true;
                                    bm = bc.CreateImageWithBarcode(pdfWriter.DirectContent, iTextSharp.text.BaseColor.BLACK, iTextSharp.text.BaseColor.BLACK);
                                    bm.ScaleToFit(250, 50);
                                }
                                else
                                {
                                    BarcodeQRCode qrcode = new BarcodeQRCode(info.DefectbarCode, 1, 1, null);
                                    bm = qrcode.GetImage();
                                    bm.ScalePercent(200);
                                }
                                bm.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                                PdfPCell cell = new PdfPCell();
                                cell.PaddingTop = 10;
                                cell.PaddingRight = 10;
                                cell.PaddingBottom = 10;
                                cell.PaddingLeft = 10;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.AddElement(bm);
                                Paragraph paragraph = new Paragraph(new Chunk(info.Message));
                                paragraph.Alignment = Element.ALIGN_CENTER;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                            }
                            doc.Add(table);
                            doc.Close();
                            //return File(ms.ToArray(), "APPLICATION/octet-stream", group.Key.From_Station + ".pdf");
                            files.Add(ms.ToArray());
                            filename.Add(group.Key.From_Station + " to " + group.Key.To_Station + ".pdf");
                            folder.Add(group.Key.From_Station);
                        }
                    }
                    using(MemoryStream ms2 = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(ms2, ZipArchiveMode.Create, true))
                        {
                            for (int i = 0; i < files.Count; i++)
                            {
                                var zipEntry = archive.CreateEntry(folder[i] + "/" + filename[i], CompressionLevel.Optimal);
                                using (var zipEntryStream = zipEntry.Open())
                                {
                                    zipEntryStream.Write(files[i], 0, files[i].Length);
                                    zipEntryStream.Close();
                                }
                            }
                            
                        }
                        ms2.Position = 0;
                        return File(ms2.ToArray(), MediaTypeNames.Application.Zip, "InsectionCode.zip");
                    }

                }
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
        [HttpGet("Master/UseManagement")]

        public IActionResult GetUseManagement([FromQuery] string COMPANYID, string PLANTID)
        {
            List<UseManagement> useManagements = new List<UseManagement>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "select BP.Name, BP.Email,BP.Mobile_number,UC.Active_Inactive,RO.RoleName,RO.json " +
                             "from user_credentials as UC " +
                             "left join business_partner as BP on UC.BP_ShortCode = BP.BP_Short_code and UC.Plant_ID = BP.Plant_Id and UC.Company_ID = BP.company_id " +
                             "left join role as RO on UC.Role = RO.RoleID and UC.Plant_ID = RO.Plant_Id and UC.Company_ID = RO.company_id " +
                             "where UC.Company_ID = '" + COMPANYID + "' and UC.Plant_ID = '" + PLANTID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    UseManagement useManagement = new UseManagement();
                    useManagement.Name = rdr["Name"].ToString();
                    useManagement.Email = rdr["Email"].ToString();
                    useManagement.Mobile_number = rdr["Mobile_number"].ToString();
                    useManagement.RoleName = rdr["RoleName"].ToString();
                    useManagement.Active_Inactive = rdr["Active_Inactive"].ToString();
                    string st = rdr["json"].ToString();
                    st = st.Remove(0, 1);
                    st = st.Remove(st.Length - 1, 1);
                    st = st.Replace(@"\", "");
                    Root rt = JsonSerializer.Deserialize<Root>(st);
                    st = string.Empty;
                    if (rt.@checked)
                        st = rt.text;
                    else
                        st = GetCheckedNode(rt.children).Remove(0,2);
                    useManagement.json = st;
                    useManagements.Add(useManagement);
                }
                return Ok(useManagements);
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

        private string GetCheckedNode(List<Child> children)
        {
            string st = string.Empty;
            if (children == null)
                return st;
            foreach(Child ch in children)
            {
                if (ch.@checked)
                    st = st + ", " + ch.text;
                else
                    st = st +  GetCheckedNode(ch.children);
            }
            return st;
        }

        [HttpGet("Master/Storages")]

        public IActionResult GetStorages([FromQuery] string COMPANYID, string PLANTID, bool BRORQR = true, bool PDF = false)
        {
            List<StoragesReport> storages = new List<StoragesReport>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "select ST.Stores_Name, ifnull(SG.Section,'-') as Section, ifnull(SG.Rack,'-') as Rack, ifnull(SG.Shelf,'-') as Shelf, ifnull(SG.Bins,'-') as Bins, " +
                             "concat(Right(concat('000',CONVERT(SG.StoreID,char)),3), " +
                             "Right(concat('000',CONVERT(ifnull(SG.Section,'0'),char)),3), " +
                             "Right(concat('000',CONVERT(ifnull(SG.Rack,'0'),char)),3), " +
                             "Right(concat('000',CONVERT(ifnull(SG.Shelf,'0'),char)),3), " +
                             "Right(concat('000',CONVERT(ifnull(SG.Bins,'0'),char)),3)) as Code " +
                             "from storages as SG " +
                             "left join stores as ST on SG.StoreID = ST.Stores_id " +
                             "where ST.Plant_Id = '" + PLANTID + "' and Company_Id = '" + COMPANYID + "' " +
                             "order by ST.Stores_id";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    StoragesReport st = new StoragesReport();
                    st.Stores_Name = rdr["Stores_Name"].ToString();
                    st.Section = rdr["Section"].ToString();
                    st.Rack = rdr["Rack"].ToString();
                    st.Shelf = rdr["Shelf"].ToString();
                    st.Bins = rdr["Bins"].ToString();
                    st.Code = rdr["Code"].ToString();
                    ZXing.BarcodeFormat iFormat;
                    if (BRORQR)
                        iFormat = ZXing.BarcodeFormat.CODE_128;
                    else
                        iFormat = ZXing.BarcodeFormat.QR_CODE;

                    BarcodeWriter qrCodeWriter = new BarcodeWriter
                    {

                        Format = iFormat,
                        Options = new QrCodeEncodingOptions
                        {
                            Height = 50,
                            Width = 250,
                            Margin = 0,
                            PureBarcode = false
                        }

                    };
                    var btmap = qrCodeWriter.Write(st.Code);
                    MemoryStream ms = new MemoryStream();
                    btmap.Save(ms, ImageFormat.Jpeg);
                    byte[] byteImage = ms.ToArray();
                    var SigBase64 = Convert.ToBase64String(byteImage);
                    st.Image = "data:image/jpeg;base64," + SigBase64;
                    storages.Add(st);
                }
                if(!PDF)
                    return Ok(storages);
                else
                {
                    List<byte[]> files = new List<byte[]>();
                    List<string> filename = new List<string>();
                    List<string> folder = new List<string>();
                    var groupedList = storages.GroupBy(u => new { u.Stores_Name});
                    foreach (var group in groupedList)
                    {

                        HeaderFooterStorages HF = new HeaderFooterStorages();
                        HF.StoreName = group.Key.Stores_Name;
                        iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(PageSize.A4);
                        using (MemoryStream ms = new MemoryStream())
                        using (Document doc = new Document(rec, 36, 36, 36, 36))
                        using (PdfWriter pdfWriter = PdfWriter.GetInstance(doc, ms))
                        {
                            pdfWriter.PageEvent = HF;
                            doc.Open();
                            float[] w = { 1, 1, 1, 1, 3 };
                            PdfPTable table = new PdfPTable(w);
                            foreach (var info in group)
                            {
                                iTextSharp.text.Image bm = null;
                                if (BRORQR)
                                {
                                    Barcode128 bc = new Barcode128();
                                    bc.TextAlignment = Element.ALIGN_CENTER;
                                    bc.Code = info.Code;
                                    bc.StartStopText = false;
                                    bc.CodeType = Barcode128.CODE128;
                                    bc.Extended = true;
                                    bm = bc.CreateImageWithBarcode(pdfWriter.DirectContent, iTextSharp.text.BaseColor.BLACK, iTextSharp.text.BaseColor.BLACK);
                                    bm.ScaleToFit(250, 50);
                                }
                                else
                                {
                                    BarcodeQRCode qrcode = new BarcodeQRCode(info.Code, 1, 1, null);
                                    bm = qrcode.GetImage();
                                    bm.ScalePercent(200);
                                }
                                bm.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                                PdfPCell cell = new PdfPCell();
                                Paragraph paragraph = new Paragraph(new Chunk(info.Section));
                                paragraph.Alignment = Element.ALIGN_CENTER;
                                cell.AddElement(paragraph);
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                table.AddCell(cell);
                                cell = new PdfPCell();
                                paragraph = new Paragraph(new Chunk(info.Rack));
                                paragraph.Alignment = Element.ALIGN_CENTER;
                                cell.AddElement(paragraph);
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                table.AddCell(cell);
                                cell = new PdfPCell();
                                paragraph = new Paragraph(new Chunk(info.Shelf));
                                paragraph.Alignment = Element.ALIGN_CENTER;
                                cell.AddElement(paragraph);
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                table.AddCell(cell);
                                cell = new PdfPCell();
                                paragraph = new Paragraph(new Chunk(info.Bins));
                                paragraph.Alignment = Element.ALIGN_CENTER;
                                cell.AddElement(paragraph);
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                table.AddCell(cell);
                                cell = new PdfPCell();
                                cell.PaddingTop = 10;
                                cell.PaddingRight = 10;
                                cell.PaddingBottom = 10;
                                cell.PaddingLeft = 10;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.AddElement(bm);
                                table.AddCell(cell);
                            }
                            doc.Add(table);
                            doc.Close();
                            //return File(ms.ToArray(), "APPLICATION/octet-stream", group.Key.From_Station + ".pdf");
                            files.Add(ms.ToArray());
                            filename.Add(group.Key.Stores_Name + ".pdf");
                            folder.Add(group.Key.Stores_Name);
                        }
                    }
                    using (MemoryStream ms2 = new MemoryStream())
                    {
                        using (var archive = new ZipArchive(ms2, ZipArchiveMode.Create, true))
                        {
                            for (int i = 0; i < files.Count; i++)
                            {
                                var zipEntry = archive.CreateEntry(folder[i] + "/" + filename[i], CompressionLevel.Optimal);
                                using (var zipEntryStream = zipEntry.Open())
                                {
                                    zipEntryStream.Write(files[i], 0, files[i].Length);
                                    zipEntryStream.Close();
                                }
                            }

                        }
                        ms2.Position = 0;
                        return File(ms2.ToArray(), MediaTypeNames.Application.Zip, "Storeges.zip");
                    }
                }
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
        //********************************Production************************************
        [HttpGet("Production/LoadingUnloading")]
        public IActionResult GetLoadingUnloading([FromQuery] DateTime StartDate, DateTime EndDate, string CompanyId, string PlantID ,int? Round = null)
        {
            string roundwise = string.Empty;
            if (Round != null)
                roundwise = "Date(BD1.TimeStamp) = '" + StartDate.ToString("yyyy-MM-dd") + "' and PP.Round = " + Round;
            else
                roundwise = "BD1.TimeStamp >= '" + StartDate.ToString("yyyy-MM-dd") + "' and BD1.TimeStamp <= '" + EndDate.ToString("yyyy-MM-dd") + "'";
            List<LoadingUnloadingDayWise> loadingUnloadings = new List<LoadingUnloadingDayWise>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "select PE.ActualRoundNo, PL.SkidNumber, PL.Position, BP1.Name as Loading_By, BD1.TimeStamp as Loading_Date_Time, GetShift(BD1.TimeStamp,BD1.Company_Id,BD1.Plant_Id) as Loading_Shift, " +
                             "BP3.name as Customer_Name, SM.Subpart_Name, CO.ColorcolName, PL.Barcode, BD2.TimeStamp as Unloading_Date_Time, " +
                             "BP2.Name as Unloading_By, GetShift(BD2.TimeStamp,BD2.Company_Id,BD2.Plant_Id) as UnLoading_Shift, WS.Workstation_name as Send_Location, DC.Message as Reason, ER.ERPNumber " +
                             "from paintloading as PL " +
                             "left join barcodedetails as BD1 on PL.LoadingDetails = BD1.ID " +
                             "left join barcodedetails as BD2 on PL.UnloadingDetails = BD2.ID " +
                             "Left join business_partner as BP1 on BD1.BP_ShortCode = BP1.BP_Short_code and BD1.Company_Id = BP1.company_id and BD1.Plant_Id = BP1.Plant_Id " +
                             "Left join business_partner as BP2 on BD2.BP_ShortCode = BP2.BP_Short_code and BD2.Company_Id = BP2.company_id and BD2.Plant_Id = BP2.Plant_Id " +
                             "Left join paint_plan_excecution as PE on PL.PaintPlanExecutionID = PE.PaintPlanExecutionID " +
                             //"left join paintplanning PP on PE.PaintPlanID = PP.PaintPlanID and PE.Company_Id = PP.company_id and PE.Plant_Id = PP.Plant_Id " +
                             "left join barcodemaster as BM on PL.barcode = BM.barcode " +
                             "left join erpnumber as ER on BM.ERPNumberID = ER.ERPNumberID and BD1.Company_Id = ER.company_id and BD1.Plant_Id = ER.Plant_Id " +
                             "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id " +
                             "left join color as CO on ER.ColorID = CO.ColorID " +
                             "left join model as MD on SM.Part_id = MD.Part_id " +
                             "left join business_partner as BP3 on MD.BP_Short_code = BP3.BP_Short_code and MD.Company_Id = BP3.company_id and MD.Plant_Id = BP3.Plant_Id " +
                             "left join defect_code as DC on BD2.DefectCode = DC.defect_code_id and BD2.Company_Id = DC.company_id and BD2.Plant_Id = DC.Plant_Id " +
                             "left join workstation as WS on DC.To_Station = WS.Workstation_Id and DC.company_id = WS.Company_Id and DC.Plant_id = WS.Plant_Id " +
                             "left join paintplanning as PP on PE.PaintPlanID = PP.PaintPlanID " +
                             "Where " + roundwise + " and BD1.Company_Id = '" + CompanyId + "' and BD1.Plant_Id = '" + PlantID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    LoadingUnloadingDayWise loadingUnloading = new LoadingUnloadingDayWise();
                    if (rdr["ActualRoundNo"] == DBNull.Value)
                        loadingUnloading.Round = null;
                    else
                        loadingUnloading.Round = Convert.ToInt32(rdr["ActualRoundNo"]);
                    if (rdr["SkidNumber"] == DBNull.Value)
                        loadingUnloading.SkidNumber = null;
                    else
                        loadingUnloading.SkidNumber = Convert.ToInt32(rdr["SkidNumber"]);
                    loadingUnloading.Position = rdr["Position"].ToString();
                    loadingUnloading.Loading_By = rdr["Loading_By"].ToString();
                    if (rdr["Loading_Date_Time"] == DBNull.Value)
                        loadingUnloading.Loading_Date_Time = null;
                    else
                        loadingUnloading.Loading_Date_Time = Convert.ToDateTime(rdr["Loading_Date_Time"]);
                    loadingUnloading.Loading_Shift = rdr["Loading_Shift"].ToString();
                    loadingUnloading.Customer_Name = rdr["Customer_Name"].ToString();
                    loadingUnloading.Subpart_Name = rdr["Subpart_Name"].ToString();
                    loadingUnloading.ColorcolName = rdr["ColorcolName"].ToString();
                    loadingUnloading.Barcode = rdr["Barcode"].ToString();
                    if (rdr["Unloading_Date_Time"] == DBNull.Value)
                        loadingUnloading.Unloading_Date_Time = null;
                    else
                        loadingUnloading.Unloading_Date_Time = Convert.ToDateTime(rdr["Unloading_Date_Time"]);
                    loadingUnloading.Unloading_By = rdr["Unloading_By"].ToString();
                    loadingUnloading.UnLoading_Shift = rdr["UnLoading_Shift"].ToString();
                    loadingUnloading.Send_Location = rdr["Send_Location"].ToString();
                    loadingUnloading.Reason = rdr["Reason"].ToString();
                    loadingUnloading.ERPNumber = rdr["ERPNumber"].ToString();
                    loadingUnloadings.Add(loadingUnloading);
                }
                return Ok(loadingUnloadings);
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
        [HttpGet("Production/LoadingColorwise")]

        public IActionResult GetLoadingColorwise([FromQuery] DateTime StartDate, DateTime EndDate, string CompanyId, string PlantID)
        {
            List<LoadingColorwiseSummary> loadingUnloadings = new List<LoadingColorwiseSummary>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "select A.Date, A.Loading_Shift, A.Round, A.Quantity, A.From_Skid,  " +
                             "if(mod((A.From_Skid+ceil(A.Quantity / A.total)-1), B.TotalSkids) = 0, B.TotalSkids, mod((A.From_Skid+ceil(A.Quantity / A.total)-1), B.TotalSkids)) as To_Skid, " +
                             "ceil(A.Quantity / A.total) as Total_Skid, ceil(A.Quantity / A.total) / B.TotalSkids Average_Round, " +
                             "A.ColorcolName " +
                             "from ( " +
                             "SELECT Date(PE.DateOfExecution) as Date, GetShift(PE.DateOfExecution,'" + CompanyId + "','" + PlantID + "') as Loading_Shift, " +
                             "PE.ActualRoundNo as Round, PE.StartSkidNumber as From_Skid, PE.EstimatedQuantity as Quantity, " +
                             "CO.ColorcolName, (SK.JigsPerSkid * SK.PartsPerJigs) as total " +
                             "FROM paint_plan_excecution as PE " +
                             "left join paintplanning PP on PE.PaintPlanID = PP.PaintPlanID " +
                             "left join color as CO on PP.color = CO.ColorID " +
                             "left join skid as SK on PP.skid = SK.Skid_config_id " +
                             "where Date(DateOfExecution) BETWEEN  '" + StartDate.ToString("yyyy-MM-dd") + "' and '" + EndDate.ToString("yyyy-MM-dd") + "') as A, general_settings as B " +
                             "where B.Company_ID = '" + CompanyId + "' and B.Plant_id = '" + PlantID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    LoadingColorwiseSummary loading = new LoadingColorwiseSummary();
                    if (rdr["Date"] == DBNull.Value)
                        loading.Date = null;
                    else
                        loading.Date = Convert.ToDateTime(rdr["Date"]);
                    loading.Loading_Shift = rdr["Loading_Shift"].ToString();
                    if (rdr["Round"] == DBNull.Value)
                        loading.Round = null;
                    else
                        loading.Round = Convert.ToInt32(rdr["Round"]);
                    if (rdr["Quantity"] == DBNull.Value)
                        loading.Quantity = null;
                    else
                        loading.Quantity = Convert.ToInt32(rdr["Quantity"]);
                    if (rdr["From_Skid"] == DBNull.Value)
                        loading.From_Skid = null;
                    else
                        loading.From_Skid = Convert.ToInt32(rdr["From_Skid"]);
                    if (rdr["To_Skid"] == DBNull.Value)
                        loading.To_Skid = null;
                    else
                        loading.To_Skid = Convert.ToInt32(rdr["To_Skid"]);
                    if (rdr["Total_Skid"] == DBNull.Value)
                        loading.Total_Skid = null;
                    else
                        loading.Total_Skid = Convert.ToInt32(rdr["Total_Skid"]);
                    if (rdr["Average_Round"] == DBNull.Value)
                        loading.Average_Round = null;
                    else
                        loading.Average_Round = Convert.ToDouble(rdr["Average_Round"]);
                    loading.ColorName = rdr["ColorcolName"].ToString();
                    loadingUnloadings.Add(loading);
                }
                return Ok(loadingUnloadings);
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
        [HttpGet("Production/LoadingSkidSummary")]
        public IActionResult GetLoadingSkidSummary([FromQuery] DateTime StartDate, DateTime EndDate, string CompanyId, string PlantID)
        {
            List<LoadingSkidSummary> loadingSkids = new List<LoadingSkidSummary>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "WITH temp AS ( " +
                             "select A.DT, A.Loading_Shift, A.Round, A.Quantity, A.From_Skid,  " +
                             "if(mod((A.From_Skid+ceil(A.Quantity / A.total)-1), B.TotalSkids) = 0, B.TotalSkids, mod((A.From_Skid+ceil(A.Quantity / A.total)-1), B.TotalSkids)) as To_Skid, " +
                             "ceil(A.Quantity / A.total) as Total_Skid, ceil(A.Quantity / A.total) / B.TotalSkids Average_Round " +
                             "from ( " +
                             "SELECT PE.DateOfExecution as DT, GetShift(PE.DateOfExecution,'" + CompanyId + "','" + PlantID + "') as Loading_Shift, " +
                             "PE.ActualRoundNo as Round, PE.StartSkidNumber as From_Skid, PE.EstimatedQuantity as Quantity, " +
                             "(SK.JigsPerSkid * SK.PartsPerJigs) as total " +
                             "FROM paint_plan_excecution as PE " +
                             "left join paintplanning PP on PE.PaintPlanID = PP.PaintPlanID " +
                             "left join skid as SK on PP.skid = SK.Skid_config_id " +
                             "where Date(DateOfExecution) BETWEEN  '" + StartDate.ToString("yyyy-MM-dd") + "' and '" + EndDate.ToString("yyyy-MM-dd") + "') as A, general_settings as B " +
                             "where B.Company_ID = '" + CompanyId + "' and B.Plant_id = '" + PlantID + "') " +
                             "select A.LoadingDate, A.Loading_Shift, A.Quantity, A.Average_Round, B.From_Round, B.From_Skid, C.To_Round, C.To_Skid, A.Total_Skid " +
                             "from " +
                             "(select Date(DT) as LoadingDate ,Loading_Shift, sum(Quantity) as Quantity, sum(Average_Round) as Average_Round, sum(Total_Skid) as Total_Skid from  temp " +
                             "group by Date(DT),Loading_Shift) as A " +
                             "left join " +
                             "(select Date(DT) as LoadingDate,Loading_Shift, Round as From_Round, From_Skid from  temp " +
                             "WHERE time(DT) IN (SELECT MIN(time(DT)) FROM temp GROUP BY Date(DT),Loading_Shift)) as B " +
                             "on  A.LoadingDate = B.LoadingDate and A.Loading_Shift = B.Loading_Shift " +
                             "left join " +
                             "(select Date(DT) as LoadingDate,Loading_Shift, Round as To_Round, To_Skid from  temp " +
                             "WHERE time(DT) IN (SELECT MAX(time(DT)) FROM temp GROUP BY Date(DT),Loading_Shift)) as C " +
                             "on A.LoadingDate = C.LoadingDate and A.Loading_Shift = C.Loading_Shift";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    LoadingSkidSummary loading = new LoadingSkidSummary();
                    if (rdr["LoadingDate"] == DBNull.Value)
                        loading.LoadingDate = null;
                    else
                        loading.LoadingDate = Convert.ToDateTime(rdr["LoadingDate"]);
                    loading.Loading_Shift = rdr["Loading_Shift"].ToString();
                    if (rdr["From_Round"] == DBNull.Value)
                        loading.From_Round = null;
                    else
                        loading.From_Round = Convert.ToInt32(rdr["From_Round"]);
                    if (rdr["To_Round"] == DBNull.Value)
                        loading.To_Round = null;
                    else
                        loading.To_Round = Convert.ToInt32(rdr["To_Round"]);
                    if (rdr["Quantity"] == DBNull.Value)
                        loading.Quantity = null;
                    else
                        loading.Quantity = Convert.ToInt32(rdr["Quantity"]);
                    if (rdr["From_Skid"] == DBNull.Value)
                        loading.From_Skid = null;
                    else
                        loading.From_Skid = Convert.ToInt32(rdr["From_Skid"]);
                    if (rdr["To_Skid"] == DBNull.Value)
                        loading.To_Skid = null;
                    else
                        loading.To_Skid = Convert.ToInt32(rdr["To_Skid"]);
                    if (rdr["Total_Skid"] == DBNull.Value)
                        loading.Total_Skid = null;
                    else
                        loading.Total_Skid = Convert.ToInt32(rdr["Total_Skid"]);
                    if (rdr["Average_Round"] == DBNull.Value)
                        loading.Average_Round = null;
                    else
                        loading.Average_Round = Convert.ToDouble(rdr["Average_Round"]);
                    loadingSkids.Add(loading);
                }
                return Ok(loadingSkids);
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
        //**************************************Dispatch*********************************
        [HttpGet("Dispatch/DispatchDetailBarcodewise")]
        public IActionResult DispatchDetailBarcodewise([FromQuery] DateTime StartDate, DateTime EndDate, string CompanyId, string PlantID)
        {
            List<DispatchDetailBarcodewise> dispatchDetailBarcodewises = new List<DispatchDetailBarcodewise>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "select " +
                             "DP.DisatchID, DP.PlanEnteryDate, DP.DateOfPlan as Plan_Execution_Date, BP1.Name as Plan_Added_by , " +
                             "DB.BarCode, BP2.Name as Customer_Name, MD.Part_name,SM.Subpart_Name , CO.ColorcolName, AM.AssemblyName, if(DB.TrollyBox = 1 , ST.Stores_Name,  null) as Ship_In,  " +
                             "if(DB.TrollyBox = 1 , 'Trolly' , 'Box') as Shipment_Type, Getshift(DB.ScanDateTime ,DB.company_id, DB.plant_id) as Shift, " +
                             "DB.ScanDateTime, BP3.Name as Scan_By " +
                             "from dispatchbarcode as DB " +
                             "left join dispatchitems as DI on DB.DispatchItemID = DI.DispatchItemsID " +
                             "left join dispatchplan as DP on DI.DispatchPlanID = DP.DispatchPlanID " +
                             "left join business_partner as BP1 on  DP.BPShortCode = BP1.BP_Short_code and DP.Plant_Id = BP1.Plant_Id and DP.Company_Id = BP1.company_id " +
                             "left join barcodemaster as BM on DB.Barcode = BM.Barcode " +
                             "left join erpnumber as ER on BM.ERPNumberID = ER.ERPNumberID " +
                             "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id " +
                             "left join Model as MD on SM.Part_id = MD.Part_id " +
                             "left join business_partner as BP2 on MD.BP_Short_code = BP2.BP_Short_code and MD.Plant_Id = BP2.Plant_Id and MD.Company_Id = BP2.company_id " +
                             "left join color as CO on ER.ColorID = CO.ColorID " +
                             "left join assembly as AM on ER.AssemblyID = AM.AssemblyID " +
                             "left join storages as SG on BM.Storage_id = SG.Storage_id " +
                             "left join stores as ST on SG.StoreID = ST.Stores_id " +
                             "left join business_partner as BP3 on  DB.BPShortCode = BP3.BP_Short_code and DB.Plant_Id = BP3.Plant_Id and DB.Company_Id = BP3.company_id " +
                             "where DP.Status = 0 and DP.DateOfPlan >= '" + StartDate.ToString("yyyy-MM-dd") + "' and DP.DateOfPlan <= '" + EndDate.ToString("yyyy-MM-dd") + "' and DP.Plant_Id = '" + PlantID + "' and DP.Company_Id = '" + CompanyId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DispatchDetailBarcodewise dispatch = new DispatchDetailBarcodewise();
                    dispatch.DispatchPlanID = rdr["DisatchID"].ToString();
                    dispatch.PlanEnteryDate = Convert.ToDateTime(rdr["PlanEnteryDate"]);
                    dispatch.Plan_Execution_Date = Convert.ToDateTime(rdr["Plan_Execution_Date"]);
                    dispatch.Plan_Added_by = rdr["Plan_Added_by"].ToString();
                    dispatch.BarCode = rdr["BarCode"].ToString();
                    dispatch.Customer_Name = rdr["Customer_Name"].ToString();
                    dispatch.Part_name = rdr["Part_name"].ToString();
                    dispatch.Subpart_Name = rdr["Subpart_Name"].ToString();
                    dispatch.ColorcolName = rdr["ColorcolName"].ToString();
                    dispatch.AssemblyName = rdr["AssemblyName"].ToString();
                    dispatch.Ship_In = rdr["Ship_In"].ToString();
                    dispatch.Shipment_Type = rdr["Shipment_Type"].ToString();
                    dispatch.Shift = rdr["Shift"].ToString();
                    dispatch.ScanDateTime = Convert.ToDateTime(rdr["ScanDateTime"]);
                    dispatch.Scan_By = rdr["Scan_By"].ToString();
                    dispatchDetailBarcodewises.Add(dispatch);
                }
                return Ok(dispatchDetailBarcodewises);
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
        [HttpGet("Dispatch/DispatchDetailModelwise")]
        public IActionResult DispatchDetailModelwise([FromQuery] DateTime StartDate, DateTime EndDate, string CompanyId, string PlantID)
        {
            List<DispatchDetailModelwise> dispatchDetails = new List<DispatchDetailModelwise>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "select DP.DisatchID, DP.DateOfPlan, BP.Name as Customer_Name, MD.Part_name, " +
                             "SM.Subpart_Name, CO.ColorcolName, AM.AssemblyName,DI.Quantity " +
                             "from dispatchitems as DI " +
                             "left join dispatchplan as DP on DI.DispatchPlanID = DP.DispatchPlanID " +
                             "left join erpnumber as ER on DI.ERPNo = ER.ERPNumberID " +
                             "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id " +
                             "left join Model as MD on SM.Part_id = MD.Part_id " +
                             "left join business_partner as BP on MD.BP_Short_code = BP.BP_Short_code and MD.Plant_Id = BP.Plant_Id and MD.Company_Id = BP.company_id " +
                             "left join color as CO on ER.ColorID = CO.ColorID " +
                             "left join assembly as AM on ER.AssemblyID = AM.AssemblyID " +
                             "where DP.Status = 0 and DP.DateOfPlan >= '" + StartDate.ToString("yyyy-MM-dd") + "' and DP.DateOfPlan <= '" + EndDate.ToString("yyyy-MM-dd") + "' and DP.Plant_Id = '" + PlantID + "' and DP.Company_Id = '" + CompanyId + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DispatchDetailModelwise dispatch = new DispatchDetailModelwise();
                    dispatch.DisatchID = rdr["DisatchID"].ToString();
                    dispatch.DateOfPlan = Convert.ToDateTime(rdr["DateOfPlan"]);
                    dispatch.Customer_Name = rdr["Customer_Name"].ToString();
                    dispatch.Part_name = rdr["Part_name"].ToString();
                    dispatch.Customer_Name = rdr["Customer_Name"].ToString();
                    dispatch.Part_name = rdr["Part_name"].ToString();
                    dispatch.Subpart_Name = rdr["Subpart_Name"].ToString();
                    dispatch.ColorcolName = rdr["ColorcolName"].ToString();
                    dispatch.AssemblyName = rdr["AssemblyName"].ToString();
                    dispatch.Quantity = Convert.ToInt32 (rdr["Quantity"]);
                    dispatchDetails.Add(dispatch);
                }
                return Ok(dispatchDetails);
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
        [HttpGet("Dispatch/DisatchSummary")]
        public IActionResult DisatchSummary([FromQuery] DateTime StartDate, DateTime EndDate, string CompanyId, string PlantID)
        {
            List<DisatchSummary> dispatchDetails = new List<DisatchSummary>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "select DP.DisatchID, DP.DateOfPlan, BP.Name as Customer_Name, sum(DI.Quantity) as Quantity " +
                             "from dispatchitems as DI " +
                             "left join dispatchplan as DP on DI.DispatchPlanID = DP.DispatchPlanID " +
                             "left join erpnumber as ER on DI.ERPNo = ER.ERPNumberID " +
                             "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id " +
                             "left join Model as MD on SM.Part_id = MD.Part_id " +
                             "left join business_partner as BP on MD.BP_Short_code = BP.BP_Short_code and MD.Plant_Id = BP.Plant_Id and MD.Company_Id = BP.company_id " +
                             "where DP.Status = 0 and DP.DateOfPlan >= '" + StartDate.ToString("yyyy-MM-dd") + "' and DP.DateOfPlan <= '" + EndDate.ToString("yyyy-MM-dd") + "' and DP.Plant_Id = '" + PlantID + "' and DP.Company_Id = '" + CompanyId + "'" +
                             "group by Customer_Name, DisatchID";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    DisatchSummary dispatch = new DisatchSummary();
                    dispatch.DisatchID = rdr["DisatchID"].ToString();
                    dispatch.DateOfPlan = Convert.ToDateTime(rdr["DateOfPlan"]);
                    dispatch.Customer_Name = rdr["Customer_Name"].ToString();
                    dispatch.Quantity = Convert.ToInt32(rdr["Quantity"]);
                    dispatchDetails.Add(dispatch);
                }
                return Ok(dispatchDetails);
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
        //***************************************Tracking********************************
        [HttpGet("Tracking/BarCodeTrack")]
        public IActionResult BarCodeTrack([FromQuery] string Barcode)
        {
            string CustomConditions = string.Empty;
            List<BarcodeTrack> barcodeTracks = new List<BarcodeTrack>();
            List<BarcodeTrackerQuery> barcodeTrackers = new List<BarcodeTrackerQuery>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            CustomConditions = "A.Barcode = '" + Barcode + "'";
            try
            {
                connection.Open();
                string sql = "select A.Barcode, if(BM.Disatched = 0, GetLocaton(BM.Storage_id),'Dispatched') as CurrentLocation, BP.name as Customer_Name, MD.Part_name, SM.Subpart_Name, " +
                             "B.MouldPlanEnteryDate, B.MouldPlanningDate, B.MouldPlanAddedBy, B.MouldPlanDateOfExecution, B.MouldPlanExecutedBy, B.Shift, " +
                             "A.Workstation_name, A.DefectCode, Message, A.FromStation, A.ToStation, A.TimeStamp, A.Store, A.Name, A.ExecutionType, " +
                             "C.BarcodePrintDateTime, C.PrintedBy, " +
                             "D.PaintPlanEnteryDate, D.PaintPlanningDate, D.PaintPlanAddedBy, D.Round, D.PaintPlanDateOfExecution, D.ActualRoundNo, D.PaintPlanExecutedBy, " +
                             "PL.SkidNumber, PL.Position, " +
                             "E.DisatchID, E.DispatchPlanEnteryDate, E.DispatchPlanningDate, E.DispatchPlanAddedBy, E.ScanDateTime, E.DispatchScanBy " +
                             "from " +
                             "(select Barcode, WK.Workstation_name, BD.DefectCode, DC.Message, WK1.Workstation_name as FromStation, WK2.Workstation_name as ToStation, TimeStamp, GetLocaton(BD.Storage_id) as Store, " +
                             "BP.Name, ExecutionType, ExecutionPlanNo, BD.Plant_Id, BD.Company_Id, BD.Sequence " +
                             "from barcodedetails as BD " +
                             "left join business_partner as BP on  BD.BP_ShortCode = BP.BP_Short_code and BD.Plant_Id = BP.Plant_Id and BD.Company_Id = BP.company_id " +
                             "left join workstation as WK on BD.Workstation_Id = WK.Workstation_Id and BD.Plant_Id = WK.Plant_Id and BD.Company_Id = WK.company_id " +
                             "left join defect_code as DC on BD.DefectCode = DC.defect_code_id and BD.Plant_Id = DC.Plant_Id and BD.Company_Id = DC.company_id " +
                             "left join workstation as WK1 on DC.From_Station = WK1.Workstation_Id and DC.Plant_Id = WK1.Plant_Id and DC.Company_Id = WK1.company_id " +
                             "left join workstation as WK2 on DC.To_Station = WK2.Workstation_Id and DC.Plant_Id = WK2.Plant_Id and DC.Company_Id = WK2.company_id " +
                             ") as A " +
                             // MOULD 
                             "left join (select ME.MoldPlanExecutionID, MP.PlanEnteryDate as MouldPlanEnteryDate, MP.PlanningDate as MouldPlanningDate, BP1.Name as MouldPlanAddedBy, " +
                             "ME.DateOfExecution as MouldPlanDateOfExecution, BP2.Name as MouldPlanExecutedBy, SH.Shift_name as Shift " +
                             "from mold_plan_execution as ME " +
                             "left join mold_planning as MP on ME.ModelPlanID = MP.MoldPlanningID " +
                             "left join business_partner as BP1 on MP.BPShortCode = BP1.BP_Short_code and ME.Plant_Id = BP1.Plant_Id and ME.Company_Id = BP1.company_id " +
                             "left join business_partner as BP2 on ME.BPShortCode = BP2.BP_Short_code and ME.Plant_Id = BP2.Plant_Id and ME.Company_Id = BP2.company_id " +
                             "left join shift as SH on ME.ShiftID = SH.Shift_id and ME.Plant_Id = SH.Plant_Id and ME.Company_Id = SH.company_id) as B on A.ExecutionType = 'M' and A.ExecutionPlanNo = B.MoldPlanExecutionID " +
                             // Print Barcode 
                             "left join (select PB.BarCode, PB.Plant_Id, PB.Company_Id, PB.TimeStamp as BarcodePrintDateTime, BP.Name as PrintedBy " +
                             "from print_bacode as PB " +
                             "left join business_partner as BP on PB.BP_Short_code = BP.BP_Short_code and BP.Plant_Id = BP.Plant_Id and PB.Company_Id = BP.company_id) as C on A.Barcode = C.Barcode and A.Plant_Id = C.Plant_Id and A.Company_Id = C.Company_Id " +
                             // Paint
                             "left join (select PE.PaintPlanExecutionID, PP.PlanEnteryDate as PaintPlanEnteryDate, PP.PlanningDate as PaintPlanningDate, BP1.Name as PaintPlanAddedBy, PP.Round, " +
                             "PE.DateOfExecution as PaintPlanDateOfExecution, PE.ActualRoundNo, BP2.Name as PaintPlanExecutedBy " +
                             "from paint_plan_excecution as PE " +
                             "left join paintplanning as PP on PE.PaintPlanID = PP.PaintPlanID and PE.Plant_Id = PP.Plant_Id and PE.Company_Id = PP.Company_Id " +
                             "left join business_partner as BP1 on PP.BPShortCode = BP1.BP_Short_code and PP.Plant_Id = BP1.Plant_Id and PP.Company_Id = BP1.company_id " +
                             "left join business_partner as BP2 on PE.BPShortCode = BP2.BP_Short_code and PE.Plant_Id = BP2.Plant_Id and PE.Company_Id = BP2.company_id) as D on A.ExecutionType = 'P' and A.ExecutionPlanNo = D.PaintPlanExecutionID " +
                             //paintloading 
                             "left join paintloading as PL on A.ExecutionType  = 'P' and A.Barcode = PL.Barcode " +
                             // Dispatch 
                             "left join (select DB.BarCode, DP.DisatchID, DP.PlanEnteryDate as DispatchPlanEnteryDate, DP.DateOfPlan as DispatchPlanningDate, BP2.Name as DispatchPlanAddedBy, DB.ScanDateTime, BP1.Name as DispatchScanBy " +
                             "from dispatchbarcode as DB " +
                             "left join dispatchitems as DI on DB.DispatchItemID = DI.DispatchItemsID " +
                             "left join  dispatchplan as DP on DI.DispatchPlanID = DP.DispatchPlanID " +
                             "left join business_partner as BP1 on DB.BPShortCode = BP1.BP_Short_code and DB.Plant_Id = BP1.Plant_Id and DB.Company_Id = BP1.company_id " +
                             "left join business_partner as BP2 on DP.BPShortCode = BP2.BP_Short_code and DP.Plant_Id = BP2.Plant_Id and DP.Company_Id = BP2.company_id) as E on A.ExecutionType = 'D' and A.Barcode = E.Barcode " +
                             "left join barcodemaster as BM on A.Barcode = BM.Barcode " +
                             //
                             "left join erpnumber as ER on BM.ERPNumberID = ER.ERPNumberID " +
                             "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id " +
                             "left join model as MD on SM.Part_id = MD.Part_id " +
                             "left join business_partner as BP on MD.BP_Short_code = BP.BP_Short_code and MD.Company_Id = BP.company_id and MD.Plant_Id = BP.Plant_Id " +
                              "where " + CustomConditions +
                             "order by A.Barcode, A.Sequence ";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    BarcodeTrackerQuery trackerQuery = new BarcodeTrackerQuery();
                    trackerQuery.Barcode = rdr["Barcode"].ToString();
                    trackerQuery.CurrentLocation = rdr["CurrentLocation"].ToString();
                    trackerQuery.Customer_Name = rdr["Customer_Name"].ToString();
                    trackerQuery.Part_name = rdr["Part_name"].ToString();
                    trackerQuery.Subpart_Name = rdr["Subpart_Name"].ToString();
                    if (rdr["MouldPlanEnteryDate"] == DBNull.Value)
                        trackerQuery.MouldPlanEnteryDate = null;
                    else
                        trackerQuery.MouldPlanEnteryDate = Convert.ToDateTime(rdr["MouldPlanEnteryDate"]);
                    if (rdr["MouldPlanningDate"] == DBNull.Value)
                        trackerQuery.MouldPlanningDate = null;
                    else
                        trackerQuery.MouldPlanningDate = Convert.ToDateTime(rdr["MouldPlanningDate"]);
                    trackerQuery.MouldPlanAddedBy = rdr["MouldPlanAddedBy"].ToString();
                    if (rdr["MouldPlanDateOfExecution"] == DBNull.Value)
                        trackerQuery.MouldPlanDateOfExecution = null;
                    else
                        trackerQuery.MouldPlanDateOfExecution = Convert.ToDateTime(rdr["MouldPlanDateOfExecution"]);
                    trackerQuery.MouldPlanExecutedBy = rdr["MouldPlanExecutedBy"].ToString();
                    trackerQuery.Shift = rdr["Shift"].ToString();
                    trackerQuery.Workstation_name = rdr["Workstation_name"].ToString();
                    trackerQuery.DefectCode = rdr["DefectCode"].ToString();
                    trackerQuery.Message = rdr["Message"].ToString();
                    trackerQuery.FromStation = rdr["FromStation"].ToString();
                    trackerQuery.ToStation = rdr["ToStation"].ToString();
                    if (rdr["TimeStamp"] == DBNull.Value)
                        trackerQuery.TimeStamp = null;
                    else
                        trackerQuery.TimeStamp = Convert.ToDateTime(rdr["TimeStamp"]);
                    trackerQuery.Store = rdr["Store"].ToString();
                    trackerQuery.Name = rdr["Name"].ToString();
                    trackerQuery.ExecutionType = rdr["ExecutionType"].ToString();
                    if (rdr["BarcodePrintDateTime"] == DBNull.Value)
                        trackerQuery.BarcodePrintDateTime = null;
                    else
                        trackerQuery.BarcodePrintDateTime = Convert.ToDateTime(rdr["BarcodePrintDateTime"]);
                    trackerQuery.PrintedBy = rdr["PrintedBy"].ToString();
                    if (rdr["PaintPlanEnteryDate"] == DBNull.Value)
                        trackerQuery.PaintPlanEnteryDate = null;
                    else
                        trackerQuery.PaintPlanEnteryDate = Convert.ToDateTime(rdr["PaintPlanEnteryDate"]);
                    if (rdr["PaintPlanningDate"] == DBNull.Value)
                        trackerQuery.PaintPlanningDate = null;
                    else
                        trackerQuery.PaintPlanningDate = Convert.ToDateTime(rdr["PaintPlanningDate"]);
                    trackerQuery.PaintPlanAddedBy = rdr["PaintPlanAddedBy"].ToString();
                    if (rdr["Round"] == DBNull.Value)
                        trackerQuery.Round = null;
                    else
                        trackerQuery.Round = Convert.ToInt32(rdr["Round"]);
                    if (rdr["PaintPlanDateOfExecution"] == DBNull.Value)
                        trackerQuery.PaintPlanDateOfExecution = null;
                    else
                        trackerQuery.PaintPlanDateOfExecution = Convert.ToDateTime(rdr["PaintPlanDateOfExecution"]);
                    if (rdr["ActualRoundNo"] == DBNull.Value)
                        trackerQuery.ActualRoundNo = null;
                    else
                        trackerQuery.ActualRoundNo = Convert.ToInt32(rdr["ActualRoundNo"]);
                    trackerQuery.PaintPlanExecutedBy = rdr["PaintPlanExecutedBy"].ToString();
                    if (rdr["SkidNumber"] == DBNull.Value)
                        trackerQuery.SkidNumber = null;
                    else
                        trackerQuery.SkidNumber = Convert.ToInt32(rdr["SkidNumber"]);
                    trackerQuery.Position = rdr["Position"].ToString();
                    trackerQuery.DisatchID = rdr["DisatchID"].ToString();
                    if (rdr["DispatchPlanEnteryDate"] == DBNull.Value)
                        trackerQuery.DispatchPlanEnteryDate = null;
                    else
                        trackerQuery.DispatchPlanEnteryDate = Convert.ToDateTime(rdr["DispatchPlanEnteryDate"]);
                    if (rdr["DispatchPlanningDate"] == DBNull.Value)
                        trackerQuery.DispatchPlanningDate = null;
                    else
                        trackerQuery.DispatchPlanningDate = Convert.ToDateTime(rdr["DispatchPlanningDate"]);
                    trackerQuery.DispatchPlanAddedBy = rdr["DispatchPlanAddedBy"].ToString();
                    if (rdr["ScanDateTime"] == DBNull.Value)
                        trackerQuery.ScanDateTime = null;
                    else
                        trackerQuery.ScanDateTime = Convert.ToDateTime(rdr["ScanDateTime"]);
                    trackerQuery.DispatchScanBy = rdr["DispatchScanBy"].ToString();
                    barcodeTrackers.Add(trackerQuery);
                }
                var groupedList = barcodeTrackers.GroupBy(u => new { u.Barcode });
                int currentRec = 0;
                foreach (var group in groupedList)
                {
                    int subM = currentRec, subP = currentRec, subU = currentRec, subA = currentRec, subD = currentRec;
                    int totalGroupRec = (int)Math.Ceiling((double)group.Count() / 5.0);
                    for(int i = 0; i < totalGroupRec; i++, currentRec++)
                        barcodeTracks.Add(new BarcodeTrack());
                    
                    foreach (var info in group)
                    {
                        if (info.ExecutionType == "M")
                        {
                            if(subM >= currentRec)
                            {
                                currentRec++;
                                barcodeTracks.Add(new BarcodeTrack());
                            }
                            barcodeTracks[subM].Barcode = info.Barcode;
                            barcodeTracks[subM].CurrentLocation = info.CurrentLocation;
                            barcodeTracks[subM].Customer_Name = info.Customer_Name;
                            barcodeTracks[subM].Part_name = info.Part_name;
                            barcodeTracks[subM].Subpart_Name = info.Subpart_Name;
                            barcodeTracks[subM].MouldPlanEnteryDate = info.MouldPlanEnteryDate;
                            barcodeTracks[subM].MouldPlanningDate = info.MouldPlanningDate;
                            barcodeTracks[subM].MouldPlanAddedBy = info.MouldPlanAddedBy;
                            barcodeTracks[subM].MouldPlanDateOfExecution = info.MouldPlanDateOfExecution;
                            barcodeTracks[subM].MouldPlanExecutedBy = info.MouldPlanExecutedBy;
                            barcodeTracks[subM].Shift = info.Shift;
                            barcodeTracks[subM].BarcodePrintDateTime = info.BarcodePrintDateTime;
                            barcodeTracks[subM].PrintedBy = info.PrintedBy;
                            barcodeTracks[subM].BarcodeScanDate = info.TimeStamp;
                            barcodeTracks[subM].MouldWorkstation_name = info.Workstation_name;
                            barcodeTracks[subM].MouldDefectCode = info.DefectCode;
                            barcodeTracks[subM].MouldDefectMessage = info.Message;
                            barcodeTracks[subM].MouldFromStation = info.FromStation;
                            barcodeTracks[subM].MouldToStation = info.ToStation;
                            barcodeTracks[subM].BacodeScanBy = info.Name;
                            barcodeTracks[subM].MouldStorelocation = info.Store;
                            subM++;
                        }
                        if (info.ExecutionType == "P")
                        {
                            if (subP >= currentRec)
                            {
                                currentRec++;
                                barcodeTracks.Add(new BarcodeTrack());
                            }
                            barcodeTracks[subP].PaintPlanEnteryDate = info.PaintPlanEnteryDate;
                            barcodeTracks[subP].PaintPlanningDate = info.PaintPlanningDate;
                            barcodeTracks[subP].PaintPlanAddedBy = info.PaintPlanAddedBy;
                            barcodeTracks[subP].Round = info.Round;
                            barcodeTracks[subP].PaintPlanDateOfExecution = info.PaintPlanDateOfExecution;
                            barcodeTracks[subP].ActualRoundNo = info.ActualRoundNo;
                            barcodeTracks[subP].PaintPlanExecutedBy = info.PaintPlanExecutedBy;
                            barcodeTracks[subP].LoadingScanDate = info.TimeStamp;
                            barcodeTracks[subP].PaintWorkstation_name = info.Workstation_name;
                            barcodeTracks[subP].PaintDefectCode = info.DefectCode;
                            barcodeTracks[subP].PaintMessage = info.Message;
                            barcodeTracks[subP].PaintFromStation = info.FromStation;
                            barcodeTracks[subP].PaintToStation = info.ToStation;
                            barcodeTracks[subP].LoadingBy = info.Name;
                            barcodeTracks[subP].PaintStorelocation = info.Store;
                            barcodeTracks[subP].SkidNumber = info.SkidNumber;
                            barcodeTracks[subP].Position = info.Position;
                            subP++;
                        }
                        if (info.ExecutionType == "U")
                        {
                            if (subU >= currentRec)
                            {
                                currentRec++;
                                barcodeTracks.Add(new BarcodeTrack());
                            }
                            barcodeTracks[subU].UnloadScanDate = info.TimeStamp;
                            barcodeTracks[subU].UnloadWorkstation_name = info.Workstation_name;
                            barcodeTracks[subU].UnloadDefectCode = info.DefectCode;
                            barcodeTracks[subU].UnloadDefectMessage = info.Message;
                            barcodeTracks[subU].UnloadFromStation = info.FromStation;
                            barcodeTracks[subU].UnloadToStation = info.ToStation;
                            barcodeTracks[subU].UnloadedBy = info.Name;
                            barcodeTracks[subU].UnloadStorelocation = info.Store;
                            subU++;
                        }
                        if (info.ExecutionType == "A")
                        {
                            if (subA >= currentRec)
                            {
                                currentRec++;
                                barcodeTracks.Add(new BarcodeTrack());
                            }
                            barcodeTracks[subA].AsseblyScanDate = info.TimeStamp;
                            barcodeTracks[subA].AsseblyWorkstation_name = info.Workstation_name;
                            barcodeTracks[subA].AsseblyDefectCode = info.DefectCode;
                            barcodeTracks[subA].AsseblyMessage = info.Message;
                            barcodeTracks[subA].AsseblyFromStation = info.FromStation;
                            barcodeTracks[subA].AsseblyToStation = info.ToStation;
                            barcodeTracks[subA].AsseblyScanBy = info.Name;
                            barcodeTracks[subA].AsseblyStorelocation = info.Store;
                            subA++;
                        }
                        if (info.ExecutionType == "D")
                        {
                            if (subD >= currentRec)
                            {
                                currentRec++;
                                barcodeTracks.Add(new BarcodeTrack());
                            }
                            barcodeTracks[subD].DisatchID = info.DisatchID;
                            barcodeTracks[subD].DispatchPlanEnteryDate = info.DispatchPlanEnteryDate;
                            barcodeTracks[subD].DispatchPlanningDate = info.DispatchPlanningDate;
                            barcodeTracks[subD].DispatchPlanAddedBy = info.DispatchPlanAddedBy;
                            barcodeTracks[subD].DispatchScanDateTime = info.ScanDateTime;
                            barcodeTracks[subD].DispatchScanBy = info.DispatchScanBy;
                            subD++;
                        }
                    }
                }
                return Ok(barcodeTracks);
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
        //********************************Quality***************************************
        [HttpGet("Quality/AssemblyDetails")]
        public IActionResult AssemblyDetails([FromQuery] DateTime StartDate, DateTime EndDate, string CompanyId, string PlantID, bool Summary = true)
        {
            List<AssemblyDetails> assemblyDetails = new List<AssemblyDetails>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql;
                if (Summary)
                {
                    sql = "with MyTable as " +
                                 "( " +
                                 "select Date(BD.TimeStamp) as Date, GROUP_CONCAT(BD.Barcode) as Barcodes, GetShift(BD.TimeStamp,BD.Company_Id,BD.Plant_Id) as Shift, grouping(Workstation_name) as Total, WS.Workstation_name, count(*) as SubTotal, " +
                                 "BP.name as Customer_Name, MD.Part_name, SM.Subpart_Name, CO.ColorcolName " +
                                 "from barcodedetails as BD " +
                                 "left join defect_code as DC on BD.DefectCode = DC.defect_code_id " +
                                 "left join workstation as WS on DC.To_Station = WS.Workstation_Id " +
                                 "left join barcodemaster as BM on BD.Barcode = BM.Barcode " +
                                 "left join erpnumber as ER on BM.ERPNumberID = ER.ERPNumberID and BD.Company_Id = ER.company_id and BD.Plant_Id = ER.Plant_Id " +
                                 "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id " +
                                 "left join color as CO on ER.ColorID = CO.ColorID " +
                                 "left join model as MD on SM.Part_id = MD.Part_id " +
                                 "left join business_partner as BP on MD.BP_Short_code = BP.BP_Short_code and MD.Company_Id = BP.company_id and MD.Plant_Id = BP.Plant_Id " +
                                 "where BD.ExecutionType = 'A' and BD.TimeStamp between '" + StartDate.ToString("yyyy-MM-dd") + "' and '" + EndDate.ToString("yyyy-MM-dd") + "' and BD.Sequence = " +
                                 "(select  max(Sequence) from barcodedetails as X where X.Barcode = BD.Barcode) and BD.Company_Id = '" + CompanyId + "' and BD.Plant_Id = '" + PlantID + "' " +
                                 "group by Date,Shift ,Workstation_name WITH ROLLUP " +
                                 ")  " +
                                 "select A.Date, A.Shift, A.Customer_Name, A.Part_name, A.Subpart_Name, A.ColorcolName, A.Barcodes, A.Workstation_name, A.SubTotal, (100*A.SubTotal/B.SubTotal) as Percentage " +
                                 "from ( " +
                                 "select Date, Barcodes, Shift, Workstation_name, SubTotal, Customer_Name, Part_name, Subpart_Name, ColorcolName from MyTable where Date is not null and Shift is not null and Total = 0 " +
                                 ") as A " +
                                 "left join (select Date, Shift, SubTotal from MyTable where Date is not null and Shift is not null and Total = 1 ) as B " +
                                 "on A.Date = B.Date and A.Shift = B.Shift ";
                }
                else
                {
                    sql = "with MyTable as " +
                          "( " +
                          "select Date(BD.TimeStamp) as Date, GetShift(BD.TimeStamp,BD.Company_Id,BD.Plant_Id) as Shift, grouping(Workstation_name) as Total, WS.Workstation_name, count(*) as SubTotal " +
                          "from barcodedetails as BD " +
                          "left join defect_code as DC on BD.DefectCode = DC.defect_code_id " +
                          "left join workstation as WS on DC.To_Station = WS.Workstation_Id " +
                          "where BD.ExecutionType = 'A' and BD.TimeStamp between '" + StartDate.ToString("yyyy-MM-dd") + "' and '" + EndDate.ToString("yyyy-MM-dd") + "' and BD.Sequence = " +
                           "(select  max(Sequence) from barcodedetails as X where X.Barcode = BD.Barcode) and BD.Company_Id = '" + CompanyId + "' and BD.Plant_Id = '" + PlantID + "' " +
                          "group by Date, Shift, Workstation_name WITH ROLLUP " +
                          ")  " +
                          "select A.Date, A.Shift, A.Workstation_name, A.SubTotal, (100*A.SubTotal/B.SubTotal) as Percentage " +
                          "from ( " +
                          "select Date, Shift, Workstation_name, SubTotal  from MyTable where Date is not null and Shift is not null and Total = 0 " +
                          ") as A " +
                          "left join (select Date, Shift, SubTotal from MyTable where Date is not null and Shift is not null and Total = 1 ) as B " +
                          "on A.Date = B.Date and A.Shift = B.Shift";
                }
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    AssemblyDetails details = new AssemblyDetails();
                    if (Summary)
                    {
                        details.Customer_Name = rdr["Customer_Name"].ToString();
                        details.Part_name = rdr["Part_name"].ToString();
                        details.Subpart_Name = rdr["Subpart_Name"].ToString();
                        details.ColorcolName = rdr["ColorcolName"].ToString();
                        details.Barcodes = rdr["Barcodes"].ToString();
                    }
                    details.Date = Convert.ToDateTime(rdr["Date"]);
                    details.Shift = rdr["Shift"].ToString();
                    details.Workstation_name = rdr["Workstation_name"].ToString();
                    details.SubTotal = Convert.ToInt32(rdr["SubTotal"]);
                    details.Percentage = Convert.ToDouble(rdr["Percentage"]);
                    assemblyDetails.Add(details);
                }
                rdr.Close();
                sql = "select  distinct WS.Workstation_name from defect_code as DC " +
                      "left join workstation as WS on DC.To_Station = WS.Workstation_Id and DC.company_id = WS.Company_Id and DC.Plant_Id = WS.Plant_Id " +
                      "where DC.From_Station = (select Workstation_Id from workstation where Workstation_name = 'Assembly')";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                rdr = cmd.ExecuteReader();
                List<string> fields = new List<string>();
                while (rdr.Read())
                {
                    fields.Add(rdr["Workstation_name"].ToString());
                }
                var groupedList = assemblyDetails.GroupBy(u => new { u.Date,u.Shift });
                string json = "[";
                foreach(var group in groupedList)
                {
                    json = json + "{";
                    json = json + "\"Date\":\"" + group.Key.Date + "\",";
                    json = json + "\"Shift\":\"" + group.Key.Shift + "\",";
                    bool flag = true;
                    List<string> f = new List<string>();
                    string bcodes = string.Empty;
                    int ?total = 0;
                    foreach (var info in group)
                    {
                        if(flag && Summary)
                        {
                            json = json + "\"Customer_Name\":\"" + info.Customer_Name + "\",";
                            json = json + "\"Part_name\":\"" + info.Part_name + "\",";
                            json = json + "\"Subpart_Name\":\"" + info.Subpart_Name + "\",";
                            json = json + "\"ColorcolName\":\"" + info.ColorcolName + "\",";
                            flag = false;
                        }
                        foreach(string st in fields)
                        {
                            if(info.Workstation_name == st)
                            {
                                json = json + "\"" + info.Workstation_name + "\":" + info.SubTotal + ",";
                                json = json + "\"" + info.Workstation_name + "_Percentage\":" + info.Percentage + ",";
                                bcodes = bcodes + info.Barcodes + ",";
                                total += info.SubTotal;
                                f.Add(st);
                            }
                            
                        }
                    }
                    foreach (string st in fields)
                    {
                        if(!f.Contains(st))
                        {
                            json = json + "\"" + st + "\":" + 0 + ",";
                            json = json + "\"" + st + "_Percentage\":" + 0 + ",";
                        }
                    }
                    f.Clear();
                    if (Summary)
                    {
                        json = json + "\"Total\":" + total + ",";
                        bcodes = bcodes.Remove(bcodes.Length - 1, 1);
                        json = json + "\"Barcodes\":\"" + bcodes + "\"";
                    }
                    else
                        json = json + "\"Total\":" + total;
                    //json = json.Remove(json.Length - 1, 1);
                    json += "},";
                }
                if(json.Substring(json.Length-1,1) == ",")
                    json = json.Remove(json.Length - 1, 1);
                json = json + "]";
                return Ok(json);
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
        //*********************************LiveStockDetails******************************
        [HttpGet("LiveStockDetails/StoreWise")]
        public IActionResult StoreWise([FromQuery] string CompanyId, string PlantID)
        {
            List<StockStorewise> storewises = new List<StockStorewise>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "select BP.name as Customer_Name, MD.Part_name, SM.Subpart_Name, CO.ColorcolName, AM.AssemblyName, ST.Stores_Name, count(*) as Instock, group_concat(BM.Barcode) as Barcodes " +
                             "from barcodemaster as BM " +
                             "left join erpnumber as ER on BM.ERPNumberID = ER.ERPNumberID " +
                             "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id  " +
                             "left join color as CO on ER.ColorID = CO.ColorID " +
                             "left join assembly as AM on ER.AssemblyID = AM.AssemblyID " +
                             "left join model as MD on SM.Part_id = MD.Part_id " +
                             "left join business_partner as BP on MD.BP_Short_code = BP.BP_Short_code and MD.Company_Id = BP.company_id and MD.Plant_Id = BP.Plant_Id " +
                             "left join storages as SG on BM.Storage_id = SG.Storage_id " +
                             "left join stores as ST on SG.StoreID = ST.Stores_id " +
                             "where BM.Disatched != 1 " +
                             "group by Customer_Name, Part_name, Subpart_Name, ColorcolName, AssemblyName, Stores_Name";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    StockStorewise stock = new StockStorewise();
                    stock.Customer_Name = rdr["Customer_Name"].ToString();
                    stock.Part_name = rdr["Part_name"].ToString();
                    stock.Subpart_Name = rdr["Subpart_Name"].ToString();
                    stock.ColorcolName = rdr["ColorcolName"].ToString();
                    stock.AssemblyName = rdr["AssemblyName"].ToString();
                    stock.Stores_Name = rdr["Stores_Name"].ToString();
                    stock.Instock = Convert.ToInt32(rdr["Instock"]);
                    stock.Barcodes = rdr["Barcodes"].ToString();
                    storewises.Add(stock);
                }
                return Ok(storewises);
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
        //*********************************Stock Take******************************
        [HttpGet("StockTake/Details")]
        public IActionResult StockTake([FromQuery]DateTime STDate, int Loc, string CompanyId, string PlantId, bool Summary = false)
        {
            List<StockTakeDetails> stockTakeDetails = new List<StockTakeDetails>();
            List<StockTakeSummary> stockTakeSummaries = new List<StockTakeSummary>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "select STD.Barcode, SM1.Subpart_Name, " +
                             "if(CO2.ColorcolName is null, '-',CO2.ColorcolName) as Actual_Color, if(AM2.AssemblyName is null, '-', AM2.AssemblyName) as Actual_Assembly , " +
                             "ST2.Stores_Name as Actual_Location, if(SG2.Section is null, '-', SG2.Section) as Actual_Section, if(SG2.Rack is null, '-', SG2.Rack) as Actual_Rack, if(SG2.Shelf is null, '-', SG2.Shelf) as Actual_Shelf, if(SG2.Bins is null, '-', SG2.Bins) as Actual_Bins, " +
                             "if(CO1.ColorcolName is null, '-', CO1.ColorcolName) as AfterStockTaking_Color, if(AM1.AssemblyName is null, '-', AM1.AssemblyName) as AfterStockTaking_Assembly , " +
                             "ST1.Stores_Name as AfterStockTaking_Location, if(SG1.Section is null, '-', SG1.Section) as AfterStockTaking_Section, if(SG1.Rack is null, '-', SG1.Rack) as AfterStockTaking_Rack, if(SG1.Shelf is null, '-', SG1.Shelf) as AfterStockTaking_Shelf, if(SG1.Bins is null, '-', SG1.Bins) as AfterStockTaking_Bins, SS.StatusMessage as Remark " +
                             "from stocktakingdetails as STD " +
                             "left join erpnumber as ER1 on STD.ERPNo = ER1.ERPNumberID " +
                             "left join sub_model as SM1 on ER1.Subpart_id = SM1.Subpart_id " +
                             "left join color as CO1 on ER1.ColorID = CO1.ColorID  " +
                             "left join assembly as AM1 on ER1.AssemblyID = AM1.AssemblyID " +
                             "left join storages as SG1 on STD.StoregesID = SG1.Storage_id " +
                             "left join stores as ST1 on SG1.StoreID = ST1.Stores_id " +
                             "left join stocktakngstatus as SS on STD.Status = SS.ID " +
                             "left join barcodemaster as BM on STD.Barcode = BM.Barcode " +
                             "left join erpnumber as ER2 on BM.ERPNumberID = ER2.ERPNumberID " +
                             "left join color as CO2 on ER2.ColorID = CO2.ColorID  " +
                             "left join assembly as AM2 on ER2.AssemblyID = AM2.AssemblyID " +
                             "left join storages as SG2 on BM.Storage_id = SG2.Storage_id " +
                             "left join stores as ST2 on SG2.StoreID = ST2.Stores_id " +
                             "where STD.StocktakingMasterID =  " +
                             "( select StocktakingMasterID from stocktakingmaster where Date = '" + STDate.ToString("yyyy-MM-dd") + "' and Location = " + Loc + " and Company_Id = '" + CompanyId + "' and Plant_Id = '" + PlantId + "') " +
                             "union " +
                             "select BM.Barcode, SM.Subpart_Name, if(CO.ColorcolName is null, '-', CO.ColorcolName) as Actual_Color, if(AM.AssemblyName is null, '-', AM.AssemblyName) as Actual_Assembly, " +
                             "ST.Stores_Name as Actual_Location, if(SG.Section is null, '-', SG.Section) as Actual_Section, if(SG.Rack is null, '-', SG.Rack) as Actual_Rack, if(SG.Shelf is null, '-', SG.Shelf) as Actual_Shelf, if(SG.Bins is null, '-', SG.Bins) as Actual_Bins, " +
                             "'-' as AfterStockTaking_Color, '-' as AfterStockTaking_Assembly, '-' as AfterStockTaking_Location, '-' as AfterStockTaking_Section, '-' as AfterStockTaking_Rack, '-' as AfterStockTaking_Shelf, '-' as AfterStockTaking_Bins, " +
                             "'Not Found' as Remark " +
                             "from barcodemaster as BM " +
                             "left join erpnumber as ER on BM.ERPNumberID = ER.ERPNumberID " +
                             "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id " +
                             "left join color as CO on ER.ColorID = CO.ColorID " +
                             "left join assembly as AM on ER.AssemblyID = AM.AssemblyID " +
                             "left join storages as SG on BM.Storage_id = SG.Storage_id " +
                             "left join stores as ST on SG.StoreID = ST.Stores_id " +
                             "where BM.Storage_id in " +
                             "( select Storage_id from storages where StoreID = 8) and  " +
                             "BM.Barcode not in (Select Barcode from stocktakingdetails where StocktakingMasterID = " +
                             "( select StocktakingMasterID from stocktakingmaster where Date = '" + STDate.ToString("yyyy-MM-dd") + "' and Location = " + Loc + " and Company_Id = '" + CompanyId + "' and Plant_Id = '" + PlantId + "')) and BM.Disatched = 0  " +
                             "and ER.Company_Id = '" + CompanyId + "' and ER.Plant_Id = '" + PlantId + "'";
                if(Summary)
                {
                    sql = "select A.Subpart_Name, A.Actual_Color, A.Actual_Assembly, A.Actual_Location, A.Remark, count(*) as Count " +
                          "from( " + sql + ") as A group by A.Subpart_Name, A.Actual_Color, A.Actual_Assembly, A.Actual_Location, A.Remark"; 
                }
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (Summary)
                    {
                        StockTakeSummary stockTake = new StockTakeSummary();
                        stockTake.Subpart_Name = rdr["Subpart_Name"].ToString();
                        stockTake.Actual_Color = rdr["Actual_Color"].ToString();
                        stockTake.Actual_Assembly = rdr["Actual_Assembly"].ToString();
                        stockTake.Actual_Location = rdr["Actual_Location"].ToString();
                        stockTake.Remark = rdr["Remark"].ToString();
                        stockTake.Count = Convert.ToInt32(rdr["Count"]);
                        stockTakeSummaries.Add(stockTake);
                    }
                    else
                    {
                        StockTakeDetails stockTake = new StockTakeDetails();
                        stockTake.Barcode = rdr["Barcode"].ToString();
                        stockTake.Subpart_Name = rdr["Subpart_Name"].ToString();
                        stockTake.Actual_Color = rdr["Actual_Color"].ToString();
                        stockTake.Actual_Assembly = rdr["Actual_Assembly"].ToString();
                        stockTake.Actual_Location = rdr["Actual_Location"].ToString();
                        stockTake.Actual_Section = rdr["Actual_Section"].ToString();
                        stockTake.Actual_Rack = rdr["Actual_Rack"].ToString();
                        stockTake.Actual_Shelf = rdr["Actual_Shelf"].ToString();
                        stockTake.Actual_Bins = rdr["Actual_Bins"].ToString();
                        stockTake.AfterStockTaking_Color = rdr["AfterStockTaking_Color"].ToString();
                        stockTake.AfterStockTaking_Assembly = rdr["AfterStockTaking_Assembly"].ToString();
                        stockTake.AfterStockTaking_Location = rdr["AfterStockTaking_Location"].ToString();
                        stockTake.AfterStockTaking_Section = rdr["AfterStockTaking_Section"].ToString();
                        stockTake.AfterStockTaking_Rack = rdr["AfterStockTaking_Rack"].ToString();
                        stockTake.AfterStockTaking_Shelf = rdr["AfterStockTaking_Shelf"].ToString();
                        stockTake.AfterStockTaking_Bins = rdr["AfterStockTaking_Bins"].ToString();
                        stockTake.Remark = rdr["Remark"].ToString();
                        stockTakeDetails.Add(stockTake);
                    }
                }
                if( Summary)
                    return Ok(stockTakeSummaries);
                else
                    return Ok(stockTakeDetails);
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
        //*********************************Management******************************
        [HttpGet("Management/Ageing")]
        public IActionResult Ageing([FromQuery] string CompanyId, string PlantId)
        {
            List<Ageing> ageings = new List<Ageing>();
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "with mytable as ( " +
                             "SELECT Barcode, TimeStamp, Sequence as Sequence, ST.Stores_Name,time_to_sec(timediff(now(), BD.TimeStamp )) / 3600 as age " +
                             "FROM barcodedetails  as BD " +
                             "left join storages SG on BD.Storage_id = SG.Storage_id " +
                             "left join stores as ST on SG.StoreID = ST.Stores_id  " +
                             "where BD.ID in " +
                             "(select max(ID)  from barcodedetails where Company_Id = '" + CompanyId + "' and Plant_Id = '"+ PlantId + "' group by Barcode) " +
                             ") " +
                             "select ST.Stores_Name, if(A.Count is null, 0, A.count) as 'Age_limit_in_8_Hrs', if(B.Count is null, 0, B.count) as 'Age_limit_in_16_Hrs', if(C.Count is null, 0, C.count) as 'Age_limit_in_24_Hrs', if(D.Count is null, 0, D.count) as 'More_than_1_day', if(E.Count is null, 0, E.count) as 'More_than_2_days' " +
                             "from stores as ST " +
                             "left join (select Stores_Name, count(Barcode) as count from mytable where age <= 8 group by Stores_Name) as A on ST.Stores_Name = A.Stores_Name " +
                             "left join (select Stores_Name, count(Barcode) as count from mytable where age > 16 and age <= 24 group by Stores_Name) as C on ST.Stores_Name = C.Stores_Name " +
                             "left join (select Stores_Name, count(Barcode) as count from mytable where age > 8 and age <= 16 group by Stores_Name) as B on ST.Stores_Name = B.Stores_Name " +
                             "left join (select Stores_Name, count(Barcode) as count from mytable where age > 24 and age <= 48 group by Stores_Name) as D on ST.Stores_Name = D.Stores_Name " +
                             "left join (select Stores_Name, count(Barcode) as count from mytable where age > 48 group by Stores_Name) as E on ST.Stores_Name = E.Stores_Name " +
                             "where ST.Stores_id != 9999"; 
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Ageing ageing = new Ageing();
                    ageing.Stores_Name = rdr["Stores_Name"].ToString();
                    ageing.Age_limit_in_8_Hrs = Convert.ToInt32(rdr["Age_limit_in_8_Hrs"]);
                    ageing.Age_limit_in_16_Hrs = Convert.ToInt32(rdr["Age_limit_in_16_Hrs"]);
                    ageing.Age_limit_in_24_Hrs = Convert.ToInt32(rdr["Age_limit_in_24_Hrs"]);
                    ageing.More_than_1_day = Convert.ToInt32(rdr["More_than_1_day"]);
                    ageing.More_than_2_days = Convert.ToInt32(rdr["More_than_2_days"]);
                    ageings.Add(ageing);
                }
                return Ok(ageings);
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
    }
    public class HeaderFooterDefectCode : PdfPageEventHelper
    {
        public string FromStation;
        public string ToStation;
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            //base.OnStartPage(writer, document);
            iTextSharp.text.Font fontHeaderFooter = FontFactory.GetFont("arial", 14f);
            LineSeparator ls = new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -1);
            
            Chunk c1 = new Chunk("Inspection Barcode Scanning Sheet\n", fontHeaderFooter);
            Chunk c2 = new Chunk(ls);
            Chunk c3 = new Chunk(ls);
            Chunk c4 = new Chunk("From Station: " + FromStation, fontHeaderFooter);
            Chunk c5 = new Chunk("To Station: " + ToStation, fontHeaderFooter);
            Paragraph paragraph = new Paragraph(c1);
            paragraph.Alignment = Element.ALIGN_CENTER;
            
            document.Add(paragraph);

            paragraph = new Paragraph(c2);
            paragraph.SetLeading(0.5F, 0.5F);
            document.Add(paragraph);
            paragraph = new Paragraph(c3);
            paragraph.SetLeading(0.5F, 0.5F);
            document.Add(paragraph);
            document.Add(new Paragraph(" "));
            //********************************************************
            PdfPTable table = new PdfPTable(2);
            table.SpacingBefore = 0;
            table.SpacingAfter = 10;
            //********************************************************
            paragraph = new Paragraph(c4);
            paragraph.Alignment = Element.ALIGN_CENTER;
            PdfPCell cell = new PdfPCell();
            cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.AddElement(paragraph);
            table.AddCell(cell);
            //*******************************************************
            paragraph = new Paragraph(c5);
            paragraph.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.AddElement(paragraph);
            table.AddCell(cell);
            document.Add(table);
        }
    }
    public class HeaderFooterStorages : PdfPageEventHelper
    {
        public string StoreName;
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            //base.OnStartPage(writer, document);
            iTextSharp.text.Font fontHeaderFooter = FontFactory.GetFont("arial", 14f);
            LineSeparator ls = new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_CENTER, -1);

            Chunk c1 = new Chunk("Storages Barcode/QR Scanning Sheet\n", fontHeaderFooter);
            Chunk c2 = new Chunk(ls);
            Chunk c3 = new Chunk(ls);
            Chunk c4 = new Chunk("From Station: " + StoreName, fontHeaderFooter);
           
            Paragraph paragraph = new Paragraph(c1);
            paragraph.Alignment = Element.ALIGN_CENTER;
            document.Add(paragraph);
            paragraph = new Paragraph(c2);
            paragraph.SetLeading(0.5F, 0.5F);
            document.Add(paragraph);
            paragraph = new Paragraph(c3);
            paragraph.SetLeading(0.5F, 0.5F);
            document.Add(paragraph);
            paragraph = new Paragraph(c4);
            paragraph.Alignment = Element.ALIGN_CENTER;
            document.Add(paragraph);
            document.Add(new Paragraph(" "));
            float[] w = { 1, 1, 1, 1, 3 };
            PdfPTable table = new PdfPTable(w);
            PdfPCell cell = new PdfPCell();
            paragraph = new Paragraph(new Chunk("Section"));
            paragraph.Alignment = Element.ALIGN_CENTER;
            cell.AddElement(paragraph);
            table.AddCell(cell);
            cell = new PdfPCell();
            paragraph = new Paragraph(new Chunk("Rack"));
            paragraph.Alignment = Element.ALIGN_CENTER;
            cell.AddElement(paragraph);
            table.AddCell(cell);
            cell = new PdfPCell();
            paragraph = new Paragraph(new Chunk("Shelf"));
            paragraph.Alignment = Element.ALIGN_CENTER;
            cell.AddElement(paragraph);
            table.AddCell(cell);
            cell = new PdfPCell();
            paragraph = new Paragraph(new Chunk("Bins"));
            paragraph.Alignment = Element.ALIGN_CENTER;
            cell.AddElement(paragraph);
            table.AddCell(cell);
            cell = new PdfPCell();
            paragraph = new Paragraph(new Chunk("Code"));
            paragraph.Alignment = Element.ALIGN_CENTER;
            cell.AddElement(paragraph);
            table.AddCell(cell);
            document.Add(table);
        }

    }
}
