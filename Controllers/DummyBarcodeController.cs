using iTextSharp.text;
using iTextSharp.text.pdf;
using MFG_Tracker.DatabaseTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MFG_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DummyBarcodeController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public DummyBarcodeController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // POST api/<DummyBarcodeController>
        [HttpPost]
        public IActionResult Post([FromBody] DummyBarcode dummyBarcode)
        {
            string ColorID, AssemblyID;
            if (dummyBarcode.ColorId == null)
                ColorID = "ER.ColorID is null";
            else
                ColorID = "ER.ColorID = " + dummyBarcode.ColorId;
            if (dummyBarcode.AssemblyId == null)
                AssemblyID = "ER.AssemblyID is null";
            else
                AssemblyID = "ER.AssemblyID = " + dummyBarcode.AssemblyId;

            string sql = "select ER.ERPNumberID, SM.Subpart_Name,CM.company_name,BP.Name " +
                         "from erpnumber as ER " +
                         "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id " +
                         "left join company as CM on ER.company_id = CM.company_id, " +
                         "business_partner as BP where  ER.Subpart_id  = " + dummyBarcode.SubpartID + " and " + ColorID + " and " + AssemblyID + " and  ER.Plant_Id = '" + dummyBarcode.Plant_Id + "' and ER.Company_Id = '" + dummyBarcode.Company_Id + "' " +
                         "and BP.company_id = ER.Company_Id and BP.Plant_Id = ER.Plant_Id and BP.BP_Short_code = '" + dummyBarcode.BPShortCode + "'";
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null; ;
            int ERPNo = 0;
            string Subpart_Name;
            string company_name;
            string BPName;
            Document doc;
            List<string> Bcode = new List<string>();
            try
            {
                connection.Open();
                myTrans = connection.BeginTransaction();
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                BarCodeSettings bs = new BarCodeSettings();
                if (rdr.Read())
                {
                    ERPNo = Convert.ToInt32(rdr["ERPNumberID"]);
                    Subpart_Name = rdr["Subpart_Name"].ToString();
                    company_name = rdr["company_name"].ToString();
                    BPName = rdr["Name"].ToString();
                }
                else
                    return StatusCode(700, "Such combination ERP Number doesn't exist in System");
                rdr.Close();
                for (int i = 0; i < dummyBarcode.Quantity; i++) {
                    cmd = new MySqlCommand("GenerateDummyBarCode", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                    };
                    cmd.Parameters.Add("?SubModel", MySqlDbType.Int32).Value = dummyBarcode.SubpartID;
                    cmd.Parameters.Add(new MySqlParameter("mRes", MySqlDbType.String, 0));
                    cmd.Parameters["mRes"].Direction = ParameterDirection.ReturnValue;
                    cmd.ExecuteNonQuery();
                    Bcode.Add(cmd.Parameters["mRes"].Value.ToString());

                    sql = "Insert into print_bacode (BarCode, MoldPlanExecutionID, BP_Short_code, Plant_Id, Company_Id, TimeStamp) values(@BarCode, @MoldPlanExecutionID, @BP_Short_code, @Plant_Id, @Company_Id, @TimeStamp)";
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    cmd.Parameters.Add(new MySqlParameter("@BarCode", MySqlDbType.VarChar)).Value = Bcode[i];
                    cmd.Parameters.Add(new MySqlParameter("@MoldPlanExecutionID", MySqlDbType.Int32)).Value = DBNull.Value;
                    cmd.Parameters.Add(new MySqlParameter("@BP_Short_code", MySqlDbType.VarChar)).Value = dummyBarcode.BPShortCode;
                    cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = dummyBarcode.Plant_Id;
                    cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = dummyBarcode.Company_Id;
                    cmd.Parameters.Add(new MySqlParameter("@TimeStamp", MySqlDbType.VarChar)).Value = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    cmd.ExecuteNonQuery();
                }
                //*********************Insert into Barcode Master**********************************
                sql = "";

                //*********************Insert into Barcode Details*********************************
                sql = "";
                //**********************Print Barcode**********************************************
                sql = "Select * from BarCodeSettings where Plant_Id = '" + dummyBarcode.Plant_Id + "' and Company_Id = '" + dummyBarcode.Company_Id + "'";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                rdr = cmd.ExecuteReader();
                 bs = new BarCodeSettings();
                if (rdr.Read())
                {
                    bs.PrinterType = Convert.ToBoolean(rdr["PrinterType"]);
                    if (bs.PrinterType)
                    {
                        bs.PrinterDPI = Convert.ToInt32(rdr["PrinterDPI"]);
                        bs.StickerWidth = float.Parse(rdr["StickerWidth"].ToString());
                        bs.StickerHeight = float.Parse(rdr["StickerHeight"].ToString());
                        bs.BarOrQR = Convert.ToBoolean(rdr["BarOrQR"]);
                    }
                    else
                    {
                        bs.PageOriantation = Convert.ToBoolean(rdr["PageOriantation"]);
                        if (bs.PageOriantation)
                        {
                            bs.PageWidth = float.Parse(rdr["PageWidth"].ToString()) * 28.3465f;
                            bs.PageHeight = float.Parse(rdr["PageHeight"].ToString()) * 28.3465f;
                            bs.StickerWidth = float.Parse(rdr["StickerWidth"].ToString()) * 28.3465f;
                            bs.StickerHeight = float.Parse(rdr["StickerHeight"].ToString()) * 28.3465f;
                            bs.Rows = Convert.ToInt32(rdr["Rowes"]);
                            bs.Columns = Convert.ToInt32(rdr["Columns"]);
                            bs.CellPadingLeft = float.Parse(rdr["CellPadingLeft"].ToString()) * 28.3465f;
                            bs.CellPadingTop = float.Parse(rdr["CellPadingTop"].ToString()) * 28.3465f;
                            bs.CellPadingRight = float.Parse(rdr["CellPadingRight"].ToString()) * 28.3465f;
                            bs.CellPadingBottom = float.Parse(rdr["CellPadingBottom"].ToString()) * 28.3465f;
                            bs.PageMarginLeft = float.Parse(rdr["PageMarginLeft"].ToString()) * 28.3465f;
                            bs.PageMarginTop = float.Parse(rdr["PageMarginTop"].ToString()) * 28.3465f;
                            bs.PageMarginRight = float.Parse(rdr["PageMarginRight"].ToString()) * 28.3465f;
                            bs.PageMarginBottom = float.Parse(rdr["PageMarginBottom"].ToString()) * 28.3465f;
                            bs.AcrossThenDown = Convert.ToBoolean(rdr["AcrossThenDown"]);
                            bs.VerticalGap = float.Parse(rdr["VerticalGap"].ToString()) * 28.3465f;
                            bs.HorizantalGap = float.Parse(rdr["HorizantalGap"].ToString()) * 28.3465f;
                            bs.Plant_Id = rdr["Plant_Id"].ToString();
                            bs.Company_Id = rdr["Company_Id"].ToString();
                            bs.BarOrQR = Convert.ToBoolean(rdr["BarOrQR"]);
                        }
                        else
                        {
                            bs.PageHeight = float.Parse(rdr["PageWidth"].ToString()) * 28.3465f;
                            bs.PageWidth = float.Parse(rdr["PageHeight"].ToString()) * 28.3465f;
                            bs.StickerHeight = float.Parse(rdr["StickerWidth"].ToString()) * 28.3465f;
                            bs.StickerWidth = float.Parse(rdr["StickerHeight"].ToString()) * 28.3465f;
                            bs.Columns = Convert.ToInt32(rdr["Rowes"]);
                            bs.Rows = Convert.ToInt32(rdr["Columns"]);
                            bs.CellPadingBottom = float.Parse(rdr["CellPadingLeft"].ToString()) * 28.3465f;
                            bs.CellPadingLeft = float.Parse(rdr["CellPadingTop"].ToString()) * 28.3465f;
                            bs.CellPadingTop = float.Parse(rdr["CellPadingRight"].ToString()) * 28.3465f;
                            bs.CellPadingRight = float.Parse(rdr["CellPadingBottom"].ToString()) * 28.3465f;
                            bs.PageMarginBottom = float.Parse(rdr["PageMarginLeft"].ToString()) * 28.3465f;
                            bs.PageMarginLeft = float.Parse(rdr["PageMarginTop"].ToString()) * 28.3465f;
                            bs.PageMarginTop = float.Parse(rdr["PageMarginRight"].ToString()) * 28.3465f;
                            bs.PageMarginRight = float.Parse(rdr["PageMarginBottom"].ToString()) * 28.3465f;
                            bs.AcrossThenDown = Convert.ToBoolean(rdr["AcrossThenDown"]);
                            bs.HorizantalGap = float.Parse(rdr["VerticalGap"].ToString()) * 28.3465f;
                            bs.VerticalGap = float.Parse(rdr["HorizantalGap"].ToString()) * 28.3465f;
                            bs.Plant_Id = rdr["Plant_Id"].ToString();
                            bs.Company_Id = rdr["Company_Id"].ToString();
                        }
                    }
                }
                else
                    return StatusCode(700, "Unable to read Barcode Settings");
                if (!bs.PrinterType)
                {
                    int totalclos = bs.Columns + bs.Columns - 1;
                    List<float> Gap = new List<float>();
                    for (int i = 0; i < totalclos; i++)
                        if (i % 2 == 0) Gap.Add(bs.StickerWidth); else Gap.Add(bs.VerticalGap);
                    float totalwidth = bs.Columns * bs.StickerWidth + (bs.Columns - 1) * bs.HorizantalGap;
                    float[] gaps = Gap.ToArray();
                    int item = 0;

                    iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(bs.PageWidth, bs.PageHeight);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        doc = new Document(rect, bs.PageMarginLeft, bs.PageMarginRight, bs.PageMarginTop, bs.PageMarginBottom);
                        PdfWriter.GetInstance(doc, ms);
                        doc.Open();
                        double Pages = Math.Ceiling((double)((double)Bcode.Count / (double)(bs.Rows * bs.Columns)));
                        for (int i = 0; i < Pages; i++)
                        {
                            if (i > 0)
                                doc.NewPage();
                            PdfPTable table = new PdfPTable(totalclos)
                            {
                                //table.WidthPercentage = 100f;
                                TotalWidth = totalwidth,
                                LockedWidth = true,
                                HorizontalAlignment = Element.ALIGN_LEFT

                            };
                            table.SetWidths(gaps);
                            for (int r = 0; r < bs.Rows; r++)
                            {
                                PdfPCell[] cells = new PdfPCell[totalclos];
                                for (int c = 0, k = 0; c < totalclos; c++)
                                {
                                    if (c % 2 == 0)
                                    {
                                        if (bs.AcrossThenDown)
                                            item = r * bs.Columns + k + (i * bs.Rows * bs.Columns);
                                        else
                                            item = c * bs.Rows + r + (i * bs.Rows * bs.Columns);
                                        k++;
                                        iTextSharp.text.Image image128 = null;
                                        if (item < Bcode.Count)
                                        {
                                            if (bs.PageOriantation)
                                            {
                                                image128 = iTextSharp.text.Image.GetInstance(GetImage(Bcode[item], bs.BarOrQR.Value, bs.StickerWidth, bs.StickerHeight, Subpart_Name, company_name, dummyBarcode.BPShortCode, BPName, "DummyBarcode"), System.Drawing.Imaging.ImageFormat.Bmp);
                                            }
                                            else
                                            {
                                                System.Drawing.Bitmap img = (Bitmap)GetImage(Bcode[item], bs.BarOrQR.Value, bs.StickerWidth, bs.StickerHeight, Subpart_Name, company_name, dummyBarcode.BPShortCode, BPName, "DummyBarcode");
                                                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                                image128 = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Bmp);
                                            }

                                            cells[c] = new PdfPCell(image128, true);
                                        }
                                        else
                                            cells[c] = new PdfPCell();
                                        cells[c].PaddingTop = bs.CellPadingTop;
                                        cells[c].PaddingRight = bs.CellPadingRight;
                                        cells[c].PaddingBottom = bs.CellPadingBottom;
                                        cells[c].PaddingLeft = bs.CellPadingLeft;
                                        cells[c].VerticalAlignment = Element.ALIGN_MIDDLE;
                                        cells[c].FixedHeight = bs.StickerHeight;
                                        table.AddCell(cells[c]);

                                    }
                                    else
                                    {
                                        cells[c] = new PdfPCell()
                                        { FixedHeight = bs.StickerHeight };
                                        table.AddCell(cells[c]);
                                    }
                                }
                                table.CompleteRow();
                                if (bs.Rows > 1 && r < bs.Rows - 1)
                                {
                                    PdfPCell[] gcells = new PdfPCell[totalclos];
                                    for (int c = 0; c < totalclos; c++)
                                    {
                                        gcells[c] = new PdfPCell()
                                        { FixedHeight = bs.VerticalGap };
                                        table.AddCell(gcells[c]);
                                    }
                                    table.CompleteRow();
                                }
                            }
                            doc.Add(table);
                        }
                        doc.Close();
                        return File(ms.ToArray(), "APPLICATION/octet-stream", "Barcode.pdf");
                    }
                }
                else
                {
                    string cmds = string.Empty;
                    int? DPI = bs.PrinterDPI;
                    int W = (int)(bs.StickerWidth / 2.54f * DPI);
                    int H = (int)(bs.StickerHeight / 2.54f * DPI);
                    //int magnification = 5;
                    //if (DPI <= 150) magnification = 1;
                    //else if (DPI <= 200) magnification = 2;
                    //else if(DPI <= 300) magnification = 3;
                    //else if(DPI <= 600 ) magnification = 4;
                    foreach (string pb in Bcode)
                    {
                        cmds += @"^XA";
                        cmds += @"^LL" + H + "^PW" + W + "^FS";
                        cmds += @"^CF0,15^FS";
                        cmds += @"^FO0,0^FB" + W + @",1,0,C^FH\^FD" + company_name + "^FS";
                        cmds += @"^FO10,15^FB" + W + @",1,0,L^FH\^FDModel:" + Subpart_Name + "^FS";
                        cmds += @"^FO10,15^FB" + (W - 20) + @",1,0,R^FH\^FDInscode:" + dummyBarcode.BPShortCode + "^FS";
                        cmds += @"^FO10,35^GB" + (W - 10) + @",140,1,B,3^FS";
                        if (bs.BarOrQR.Value)
                            cmds += @"^FO60,60^BCN,80^FD" + pb + "^FS";
                        else
                            cmds += @"^FO210,40^BQN,2,5^FDQA," + pb + "^FS";
                        cmds += @"^FO10," + (H - 15) + @"^FB" + W + @",1,0,L^FH\^FDPrint Date:" + DateTime.Now.ToString("dd/MM/yyyy") + "^FS";
                        cmds += @"^FO10," + (H - 15) + @"^FB" + W + @",1,0,C^FH\^FDSDummy Barcode^FS";
                        cmds += @"^FO10," + (H - 15) + @"^FB" + W + @",1,0,R^FH\^FDIns. by:" + BPName + "^FS";
                        cmds += @"^XZ\n";
                    }
                    myTrans.Commit();
                    return Ok(cmds);
                }
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
        }
        [HttpPost("{PLANTID}/{COMPANYID}")]
        public IActionResult Post(string PLANTID, string COMPANYID, [FromQuery] string Barcode, string CompanyName)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Select * from BarCodeSettings where Plant_Id = '" + PLANTID + "' and Company_Id = '" + COMPANYID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                BarCodeSettings bs = new BarCodeSettings();
                if (rdr.Read())
                {
                    bs.PrinterType = Convert.ToBoolean(rdr["PrinterType"]);
                    if (bs.PrinterType)
                    {
                        bs.PrinterDPI = Convert.ToInt32(rdr["PrinterDPI"]);
                        bs.StickerWidth = float.Parse(rdr["StickerWidth"].ToString());
                        bs.StickerHeight = float.Parse(rdr["StickerHeight"].ToString());
                        bs.BarOrQR = Convert.ToBoolean(rdr["BarOrQR"]);
                    }
                    else
                    {
                        bs.PageOriantation = Convert.ToBoolean(rdr["PageOriantation"]);
                        if (bs.PageOriantation)
                        {
                            bs.PageWidth = float.Parse(rdr["PageWidth"].ToString()) * 28.3465f;
                            bs.PageHeight = float.Parse(rdr["PageHeight"].ToString()) * 28.3465f;
                            bs.StickerWidth = float.Parse(rdr["StickerWidth"].ToString()) * 28.3465f;
                            bs.StickerHeight = float.Parse(rdr["StickerHeight"].ToString()) * 28.3465f;
                            bs.Rows = Convert.ToInt32(rdr["Rowes"]);
                            bs.Columns = Convert.ToInt32(rdr["Columns"]);
                            bs.CellPadingLeft = float.Parse(rdr["CellPadingLeft"].ToString()) * 28.3465f;
                            bs.CellPadingTop = float.Parse(rdr["CellPadingTop"].ToString()) * 28.3465f;
                            bs.CellPadingRight = float.Parse(rdr["CellPadingRight"].ToString()) * 28.3465f;
                            bs.CellPadingBottom = float.Parse(rdr["CellPadingBottom"].ToString()) * 28.3465f;
                            bs.PageMarginLeft = float.Parse(rdr["PageMarginLeft"].ToString()) * 28.3465f;
                            bs.PageMarginTop = float.Parse(rdr["PageMarginTop"].ToString()) * 28.3465f;
                            bs.PageMarginRight = float.Parse(rdr["PageMarginRight"].ToString()) * 28.3465f;
                            bs.PageMarginBottom = float.Parse(rdr["PageMarginBottom"].ToString()) * 28.3465f;
                            bs.AcrossThenDown = Convert.ToBoolean(rdr["AcrossThenDown"]);
                            bs.VerticalGap = float.Parse(rdr["VerticalGap"].ToString()) * 28.3465f;
                            bs.HorizantalGap = float.Parse(rdr["HorizantalGap"].ToString()) * 28.3465f;
                            bs.Plant_Id = rdr["Plant_Id"].ToString();
                            bs.Company_Id = rdr["Company_Id"].ToString();
                            bs.BarOrQR = Convert.ToBoolean(rdr["BarOrQR"]);
                        }
                        else
                        {
                            bs.PageHeight = float.Parse(rdr["PageWidth"].ToString()) * 28.3465f;
                            bs.PageWidth = float.Parse(rdr["PageHeight"].ToString()) * 28.3465f;
                            bs.StickerHeight = float.Parse(rdr["StickerWidth"].ToString()) * 28.3465f;
                            bs.StickerWidth = float.Parse(rdr["StickerHeight"].ToString()) * 28.3465f;
                            bs.Columns = Convert.ToInt32(rdr["Rows"]);
                            bs.Rows = Convert.ToInt32(rdr["Columns"]);
                            bs.CellPadingBottom = float.Parse(rdr["CellPadingLeft"].ToString()) * 28.3465f;
                            bs.CellPadingLeft = float.Parse(rdr["CellPadingTop"].ToString()) * 28.3465f;
                            bs.CellPadingTop = float.Parse(rdr["CellPadingRight"].ToString()) * 28.3465f;
                            bs.CellPadingRight = float.Parse(rdr["CellPadingBottom"].ToString()) * 28.3465f;
                            bs.PageMarginBottom = float.Parse(rdr["PageMarginLeft"].ToString()) * 28.3465f;
                            bs.PageMarginLeft = float.Parse(rdr["PageMarginTop"].ToString()) * 28.3465f;
                            bs.PageMarginTop = float.Parse(rdr["PageMarginRight"].ToString()) * 28.3465f;
                            bs.PageMarginRight = float.Parse(rdr["PageMarginBottom"].ToString()) * 28.3465f;
                            bs.AcrossThenDown = Convert.ToBoolean(rdr["AcrossThenDown"]);
                            bs.HorizantalGap = float.Parse(rdr["VerticalGap"].ToString()) * 28.3465f;
                            bs.VerticalGap = float.Parse(rdr["HorizantalGap"].ToString()) * 28.3465f;
                            bs.Plant_Id = rdr["Plant_Id"].ToString();
                            bs.Company_Id = rdr["Company_Id"].ToString();
                        }
                    }
                }
                else
                    return StatusCode(700, "Unable to read Barcode Settings");
                rdr.Close();
                sql = "SELECT SM.Subpart_Name, CO.ColorcolName, AM.AssemblyName, SM.Customer_material_number " +
                             "FROM barcodemaster as BM " +
                             "left join erpnumber as ER on BM.ERPNumberID = ER.ERPNumberID " +
                             "left join sub_model as SM on ER.Subpart_id = SM.Subpart_id " +
                             "left join color as CO on ER.ColorID = CO.ColorID " +
                             "left join assembly as AM on ER.AssemblyID = AM.AssemblyID " +
                             "left join print_bacode as PB on BM.Barcode = PB.Barcode " +
                             "left join business_partner BP on PB.BP_Short_code = BP.BP_Short_code and ER.Plant_Id = BP.Plant_Id and ER.company_id = BP.company_id " +
                             "Where BM.Barcode = '" + Barcode + "'";
                DummyBarcodeDetails brdetails = new DummyBarcodeDetails();
                cmd = new MySqlCommand(sql, connection)
                { CommandType = CommandType.Text };
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    brdetails.AssemblyName = rdr["AssemblyName"].ToString();
                    brdetails.Subpart_Name = rdr["Subpart_Name"].ToString();
                    brdetails.ColorcolName = rdr["ColorcolName"].ToString();
                    brdetails.Customer_material_number = rdr["Customer_material_number"].ToString();
                }
                else
                    return StatusCode(700, "Invalid Barcode");
                rdr.Close();
                string Timestamp = "", Shift_name = "", BPName = "", ShortCode = "";
                if (brdetails.AssemblyName == null)
                {
                    sql = "Select PB.TimeStamp, SH.Shift_name, PB.BP_Short_code, BP.Name " +
                              "FROM print_bacode as PB " +
                              "left join mold_plan_execution ME on PB.MoldPlanExecutionID = ME.MoldPlanExecutionID " +
                              "left join shift as SH on ME.ShiftID = SH.Shift_id " +
                              "left join business_partner BP on PB.BP_Short_code = BP.BP_Short_code and PB.Plant_Id = BP.Plant_Id and PB.company_id = BP.company_id " +
                              "Where PB.Barcode = 'S220301300100063'";
                    cmd = new MySqlCommand(sql, connection)
                    { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        Timestamp = rdr["TimeStamp"].ToString();
                        Timestamp = string.Format("dd/MM/yyyy", Timestamp);
                        Shift_name = rdr["Shift_name"].ToString();
                        BPName = rdr["Name"].ToString();
                        ShortCode = rdr["BP_Short_code"].ToString();

                    }
                    else
                        return StatusCode(700, "Invalid Barcode");
                }
                if (!bs.PrinterType)
                {
                    int totalclos = bs.Columns + bs.Columns - 1;
                    List<float> Gap = new List<float>();
                    for (int i = 0; i < totalclos; i++)
                        if (i % 2 == 0) Gap.Add(bs.StickerWidth); else Gap.Add(bs.VerticalGap);
                    float totalwidth = bs.Columns * bs.StickerWidth + (bs.Columns - 1) * bs.HorizantalGap;
                    float[] gaps = Gap.ToArray();
                    int item = 0;
                    Document doc;
                    iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(bs.PageWidth, bs.PageHeight);
                    using (MemoryStream mst = new MemoryStream())
                    {
                        doc = new Document(rect, bs.PageMarginLeft, bs.PageMarginRight, bs.PageMarginTop, bs.PageMarginBottom);
                        PdfWriter.GetInstance(doc, mst);
                        doc.Open();
                        double Pages = Math.Ceiling((double)((double)1 / (double)(bs.Rows * bs.Columns)));
                        for (int i = 0; i < Pages; i++)
                        {
                            if (i > 0)
                                doc.NewPage();
                            PdfPTable table = new PdfPTable(totalclos)
                            {
                                //table.WidthPercentage = 100f;
                                TotalWidth = totalwidth,
                                LockedWidth = true,
                                HorizontalAlignment = Element.ALIGN_LEFT

                            };
                            table.SetWidths(gaps);
                            for (int r = 0; r < bs.Rows; r++)
                            {
                                PdfPCell[] cells = new PdfPCell[totalclos];
                                for (int c = 0, k = 0; c < totalclos; c++)
                                {
                                    if (c % 2 == 0)
                                    {
                                        if (bs.AcrossThenDown)
                                            item = r * bs.Columns + k + (i * bs.Rows * bs.Columns);
                                        else
                                            item = c * bs.Rows + r + (i * bs.Rows * bs.Columns);
                                        k++;
                                        iTextSharp.text.Image image128 = null;
                                        if (item < 1)
                                        {
                                            if (bs.PageOriantation)
                                            {
                                                if (brdetails.AssemblyName == null)
                                                    image128 = iTextSharp.text.Image.GetInstance(GetImage(Barcode, bs.BarOrQR.Value, bs.StickerWidth, bs.StickerHeight, brdetails.Subpart_Name, CompanyName, ShortCode, BPName, Shift_name), System.Drawing.Imaging.ImageFormat.Bmp);
                                                else
                                                    image128 = iTextSharp.text.Image.GetInstance(GetImage(Barcode, bs.BarOrQR, bs.StickerWidth, bs.StickerHeight, brdetails.Subpart_Name, brdetails.ColorcolName, brdetails.AssemblyName, brdetails.Customer_material_number), System.Drawing.Imaging.ImageFormat.Bmp);
                                            }
                                            else
                                            {
                                                System.Drawing.Bitmap img = null;
                                                if (brdetails.AssemblyName == null)
                                                    img = (Bitmap)GetImage(Barcode, bs.BarOrQR.Value, bs.StickerWidth, bs.StickerHeight, brdetails.Subpart_Name, CompanyName, ShortCode, BPName, Shift_name);
                                                else
                                                    img = (Bitmap)GetImage(Barcode, bs.BarOrQR, bs.StickerWidth, bs.StickerHeight, brdetails.Subpart_Name, brdetails.ColorcolName, brdetails.AssemblyName, brdetails.Customer_material_number);
                                                img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                                image128 = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Bmp);
                                            }

                                            cells[c] = new PdfPCell(image128, true);
                                        }
                                        else
                                            cells[c] = new PdfPCell();
                                        cells[c].PaddingTop = bs.CellPadingTop;
                                        cells[c].PaddingRight = bs.CellPadingRight;
                                        cells[c].PaddingBottom = bs.CellPadingBottom;
                                        cells[c].PaddingLeft = bs.CellPadingLeft;
                                        cells[c].VerticalAlignment = Element.ALIGN_MIDDLE;
                                        cells[c].FixedHeight = bs.StickerHeight;
                                        table.AddCell(cells[c]);

                                    }
                                    else
                                    {
                                        cells[c] = new PdfPCell()
                                        { FixedHeight = bs.StickerHeight };
                                        table.AddCell(cells[c]);
                                    }
                                }
                                table.CompleteRow();
                                if (bs.Rows > 1 && r < bs.Rows - 1)
                                {
                                    PdfPCell[] gcells = new PdfPCell[totalclos];
                                    for (int c = 0; c < totalclos; c++)
                                    {
                                        gcells[c] = new PdfPCell()
                                        { FixedHeight = bs.VerticalGap };
                                        table.AddCell(gcells[c]);
                                    }
                                    table.CompleteRow();
                                }
                            }
                            doc.Add(table);
                        }
                        doc.Close();
                        return File(mst.ToArray(), "APPLICATION/octet-stream", "Barcode.pdf");
                    }
                }
                else
                {

                    string cmds = "";
                    float H = bs.StickerHeight;
                    float W = bs.StickerWidth;
                    if (brdetails.AssemblyName != null)
                    {
                        cmds += @"^XA";
                        cmds += @"^LL" + H + "^PW" + W + "^FS";
                        cmds += @"^CF0,15^FS";
                        cmds += @"^FO10,15^FB" + W + @",1,0,L^FH\^FDModel:" + brdetails.Subpart_Name + "^FS";
                        cmds += @"^FO10,15^FB" + (W - 20) + @",1,0,R^FH\^FDType:" + brdetails.AssemblyName + "^FS";
                        cmds += @"^FO10,32^FB" + W + @",1,0,L^FH\^FDColour:" + brdetails.ColorcolName + "^FS";
                        cmds += @"^FO10,32^FB" + (W - 20) + @",1,0,R^FH\^FDRePrint Date:" + DateTime.Now.ToString("dd/MM/yyyy") + "^FS";
                        cmds += @"^FO10,49^FB" + W + @",1,0,L^FH\^FDCustomer Material No.:" + brdetails.Customer_material_number + "^FS";
                        cmds += @"^FO10,66^GB" + (W - 20) + @",120,1,B,3^FS";
                        if (bs.BarOrQR.Value)
                            cmds += @"^FO60,80^BCN,70^FD" + Barcode + "^FS";
                        else
                            cmds += @"^FO210,66^BQN,2,5^FDQA," + Barcode + "^FS";
                        cmds += @"^XZ\n";
                        return Ok(cmds);
                    }
                    else
                    {
                        //Find here BP_Short_code, Name, ShiftName


                        cmds += @"^XA";
                        cmds += @"^LL" + H + "^PW" + W + "^FS";
                        cmds += @"^CF0,15^FS";
                        cmds += @"^FO0,0^FB" + W + @",1,0,C^FH\^FD" + CompanyName + "^FS";
                        cmds += @"^FO10,15^FB" + W + @",1,0,L^FH\^FDModel:" + brdetails.Subpart_Name + "^FS";
                        cmds += @"^FO10,15^FB" + (W - 20) + @",1,0,R^FH\^FDInscode:" + ShortCode + "^FS";
                        cmds += @"^FO10,35^GB" + (W - 10) + @",140,1,B,3^FS";
                        if (bs.BarOrQR.Value)
                            cmds += @"^FO60,60^BCN,80^FD" + Barcode + "^FS";
                        else
                            cmds += @"^FO210,40^BQN,2,5^FDQA," + Barcode + "^FS";
                        cmds += @"^FO10," + (H - 15) + @"^FB" + W + @",1,0,L^FH\^FDPrint Date:" + DateTime.Now.ToString("dd/MM/yyyy") + "^FS";
                        cmds += @"^FO10," + (H - 15) + @"^FB" + W + @",1,0,C^FH\^FDShift:" + Shift_name + "^FS";
                        cmds += @"^FO10," + (H - 15) + @"^FB" + W + @",1,0,R^FH\^FDIns. by:" + BPName + "^FS";
                        cmds += @"^XZ\n";
                        return Ok(cmds);
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
        private System.Drawing.Image GetImage(string v, bool? barOrQR, float stickerWidth, float stickerHeight, string partName, string colorName, string assemblyName, string custMaterial)
        {
            string date = "Print date:" + DateTime.Now.ToString("dd-MMM-yyyy");
            float pixelsPerPoint = 1.333333f;

            int width1 = (int)(stickerWidth * pixelsPerPoint);
            int height1 = (int)(stickerHeight * pixelsPerPoint);

            int width = width1 - 6; // width of the Qr Code
            int height = height1 - 50; // height of the Qr Code
            int margin = 0;
            ZXing.BarcodeFormat iFormat;
            bool newBool = barOrQR.HasValue ? barOrQR.Value : false;
            if (newBool)
                iFormat = ZXing.BarcodeFormat.CODE_128;
            else
                iFormat = ZXing.BarcodeFormat.QR_CODE;

            BarcodeWriter qrCodeWriter = new BarcodeWriter
            {

                Format = iFormat,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin,
                    PureBarcode = false
                }

            };


            Bitmap bitmap = new Bitmap(width1, height1);
            Graphics g = Graphics.FromImage((System.Drawing.Image)bitmap);
            g.FillRectangle(Brushes.White, 0f, 0f, bitmap.Width, bitmap.Height);  // fill the entire bitmap with a red rectangle  

            g.DrawRectangle(new Pen(System.Drawing.Color.Black, 3), new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height));

            //Top Left
            g.DrawString(partName, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point(4, 4));
            //Right Top
            g.DrawString(assemblyName, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Bold), Brushes.Black, new Point(bitmap.Width - ((int)(4.1951219512195121951219512195122f * assemblyName.Length + 14)), 4));
            //Left Top 2
            g.DrawString(colorName, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point(4, 12));
            //Right Top 2
            g.DrawString(date, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point(bitmap.Width - ((int)(4.1951219512195121951219512195122f * date.Length + 14)), 12));
            //Right Top 3
            g.DrawString(custMaterial, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point(3, 20));


            g.DrawImage((System.Drawing.Image)qrCodeWriter.Write(v).Clone(), new System.Drawing.Rectangle((width1 - width) / 2, (height1 - height) / 2 + 2, width, height), new System.Drawing.Rectangle(0, 0, width, height), GraphicsUnit.Pixel);

            return (System.Drawing.Image)(bitmap);
        }
        private System.Drawing.Image GetImage(string barCode, bool CodeType, float StickerWidth, float StickerHeight, string SubModel, string CompanyName, string BPCode, string BP_Name, string ShiftName)
        {
            string model = "Model:" + SubModel;
            string BPShortCode = "Insp code:" + BPCode;
            string BPName = "Insp by: " + BP_Name;
            string date = "Print date:" + DateTime.Now.ToString("dd-MMM-yyyy");
            string Shift = "Shift:" + ShiftName;
            string CName = CompanyName + " - Mold Barcode";
            float pixelsPerPoint = 1.333333f;

            int width1 = (int)(StickerWidth * pixelsPerPoint);
            int height1 = (int)(StickerHeight * pixelsPerPoint);

            int width = width1 - 6; // width of the Qr Code
            int height = height1 - 50; // height of the Qr Code
            int margin = 0;
            ZXing.BarcodeFormat iFormat;
            if (CodeType)
                iFormat = ZXing.BarcodeFormat.CODE_128;
            else
                iFormat = ZXing.BarcodeFormat.QR_CODE;

            BarcodeWriter qrCodeWriter = new BarcodeWriter
            {

                Format = iFormat,
                Options = new QrCodeEncodingOptions
                {
                    Height = height,
                    Width = width,
                    Margin = margin,
                    PureBarcode = false
                }

            };


            Bitmap bitmap = new Bitmap(width1, height1);
            Graphics g = Graphics.FromImage((System.Drawing.Image)bitmap);
            g.FillRectangle(Brushes.White, 0f, 0f, bitmap.Width, bitmap.Height);  // fill the entire bitmap with a red rectangle  

            g.DrawRectangle(new Pen(System.Drawing.Color.Black, 3), new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height));

            //Top Center
            g.DrawString(CName, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point((bitmap.Width - (int)(4.1951219512195121951219512195122f * CName.Length)) / 2, 4));
            //Left Top
            g.DrawString(model, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Bold), Brushes.Black, new Point(4, (int)(12)));
            //Right Top
            g.DrawString(BPShortCode, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point(bitmap.Width - ((int)(4.1951219512195121951219512195122f * BPShortCode.Length) + 14), 12));
            //Left Bottom
            g.DrawString(date, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point(4, bitmap.Height - 24));
            //Right Bottomr
            g.DrawString(BPName, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point(bitmap.Width - ((int)(4.1951219512195121951219512195122f * BPName.Length) + 14), bitmap.Height - 28));
            //Bottom Center
            g.DrawString(Shift, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point((bitmap.Width - (int)(4.1951219512195121951219512195122f * Shift.Length)) / 2, bitmap.Height - 16));

            g.DrawImage((System.Drawing.Image)qrCodeWriter.Write(barCode).Clone(), new System.Drawing.Rectangle((width1 - width) / 2, (height1 - height) / 2 + 2, width, height), new System.Drawing.Rectangle(0, 0, width, height), GraphicsUnit.Pixel);

            return (System.Drawing.Image)(bitmap);
        }
    }

}
