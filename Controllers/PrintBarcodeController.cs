using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
    public class PrintBarcodeController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public PrintBarcodeController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<PrintBarcodeController>
        [HttpGet("{PlanExecutionID}/{CodeType}")]
        public IActionResult Get(int PlanExecutionID,int CodeType,[FromQuery] int Demo, bool BarOrQR, float Width, float Height,string SubModel, string CompanyName, string BPCode, string BPName, string ShiftName)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            string Bcode;
            try
            {
                connection.Open();
                MySqlCommand cmd = null;
                cmd = new MySqlCommand("GenerateBarCode", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                };
                cmd.Parameters.Add("?MPlanExe", MySqlDbType.Int32).Value = PlanExecutionID;
                cmd.Parameters.Add("?CodeType", MySqlDbType.Int32).Value = CodeType;
                cmd.Parameters.Add("?Demo", MySqlDbType.Int32).Value = Demo;
                cmd.Parameters.Add(new MySqlParameter("mRes", MySqlDbType.String, 0));
                cmd.Parameters["mRes"].Direction = ParameterDirection.ReturnValue;
                cmd.ExecuteNonQuery();
                Bcode = cmd.Parameters["mRes"].Value.ToString();
                MemoryStream ms = new MemoryStream();
                GetImage(Bcode, BarOrQR,(int)(Width* 28.3465f* 1.33333f), (int)(Height * 28.3465f* 1.33333f),SubModel, CompanyName, BPCode, BPName, ShiftName).Save(ms, ImageFormat.Bmp);
                byte[] bmpBytes = ms.ToArray();
                return File(bmpBytes, "APPLICATION/octet-stream", "Barcode.bmp");
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
        // GET: api/<PrintBarcodeController>
        [HttpGet]
        public IActionResult Get([FromQuery] string Barcode)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "select Subpart_id from erpnumber where ERPNumberID = (select ERPNumberID from barcodemaster where Barcode = '" + Barcode + "')";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.Read())
                    return Ok(Convert.ToInt32(rdr["Subpart_id"]));
                else
                    return StatusCode(700, "Barcode is not present");
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
        // POST api/<PrintBarcodeController>
        [HttpPost]
        public IActionResult Post([FromBody] BarrCode Code)
        {
            List<PrintBarrCode> BCodes = new List<PrintBarrCode>();
            Document doc ;
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            string sql;
            MySqlConnection connection = new MySqlConnection(connString);
            MySqlTransaction myTrans = null; ;
            try
            {
                connection.Open();
                MySqlCommand cmd = null;
                MySqlDataReader rdr = null;
                if (!Code.Extra)
                {
                    sql = "select EstimatedQuantity, count(BR.MoldPlanExecutionID) as TotalPrinted from mold_plan_execution as ME " +
                          "left join  print_bacode as BR on ME.MoldPlanExecutionID = BR.MoldPlanExecutionID " +
                          "where ME.MoldPlanExecutionID = " + Code.PlanExecutionID;
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    rdr = cmd.ExecuteReader();
                    rdr.Read();
                    int EstimatedQuantity = Convert.ToInt32(rdr["EstimatedQuantity"]);
                    int TotalPrinted = Convert.ToInt32(rdr["TotalPrinted"]);
                    rdr.Close();
                    if (EstimatedQuantity < (TotalPrinted + Code.Quantity))
                        return StatusCode(700, "you can only print " + (EstimatedQuantity - TotalPrinted) + "  Bar/QR Codes");
                    if (EstimatedQuantity == (TotalPrinted + Code.Quantity))
                    {
                        //Close the Mould execution plan
                        sql = "Update mold_plan_execution set Status = 1 where MoldPlanExecutionID = " + Code.PlanExecutionID; ;
                        cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                        cmd.ExecuteNonQuery();
                    }
                }
                myTrans = connection.BeginTransaction();
                for (int i = 0; i < Code.Quantity; i++)
                {
                    cmd = new MySqlCommand("GenerateBarCode", connection)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Transaction = myTrans
                    };
                    cmd.Parameters.Add("?MPlanExe", MySqlDbType.Int32).Value = Code.PlanExecutionID;
                    cmd.Parameters.Add("?CodeType", MySqlDbType.Int32).Value = Code.CodeType;
                    cmd.Parameters.Add("?Demo", MySqlDbType.Int32).Value = 1;
                    cmd.Parameters.Add(new MySqlParameter("mRes", MySqlDbType.String, 0));
                    cmd.Parameters["mRes"].Direction = ParameterDirection.ReturnValue;
                    cmd.ExecuteNonQuery();
                    string Bcode = cmd.Parameters["mRes"].Value.ToString();
                    PrintBarrCode bc = new PrintBarrCode();
                    sql = "insert into print_bacode (BarCode, MoldPlanExecutionID, BP_Short_code,Plant_Id,Company_Id,TimeStamp) values ('" + Bcode + "', " + Code.PlanExecutionID + ",'" + Code.BP_Short_code + "','" + Code.Plant_Id + "','" + Code.Company_Id + "','"+ DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") +"')";
                    cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                    cmd.ExecuteScalar();
                    cmd.LastInsertedId.ToString();
                    bc.BarCode = Bcode;
                    bc.PlanExecutionID = Code.PlanExecutionID;
                    BCodes.Add(bc);
                }
                myTrans.Commit();
                //********************************************************************************************
                sql = "Select * from BarCodeSettings where Plant_Id = '" + Code.Plant_Id + "' and Company_Id = '" + Code.Company_Id + "'";
                cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                rdr = cmd.ExecuteReader();
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
                        double Pages = Math.Ceiling((double)((double)BCodes.Count / (double)(bs.Rows * bs.Columns)));
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
                                        if (item < BCodes.Count)
                                        {
                                            if (bs.PageOriantation)
                                            {
                                                image128 = iTextSharp.text.Image.GetInstance(GetImage(BCodes[item].BarCode, Code.BarOrQR, bs.StickerWidth, bs.StickerHeight, Code.SubModel, Code.CompanyName, Code.BP_Short_code, Code.BPName, Code.ShiftName), System.Drawing.Imaging.ImageFormat.Bmp);
                                            }
                                            else
                                            {
                                                System.Drawing.Bitmap img = (Bitmap)GetImage(BCodes[item].BarCode, Code.BarOrQR, bs.StickerWidth, bs.StickerHeight, Code.SubModel, Code.CompanyName, Code.BP_Short_code, Code.BPName, Code.ShiftName);
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
                    int W =(int) (bs.StickerWidth / 2.54f * DPI);
                    int H = (int)(bs.StickerHeight / 2.54f * DPI);
                    //int magnification = 5;
                    //if (DPI <= 150) magnification = 1;
                    //else if (DPI <= 200) magnification = 2;
                    //else if(DPI <= 300) magnification = 3;
                    //else if(DPI <= 600 ) magnification = 4;
                    foreach (PrintBarrCode pb in BCodes)
                    {
                        cmds += @"^XA";
                        cmds += @"^LL"+ H + "^PW" + W + "^FS";
                        cmds += @"^CF0,15^FS";
                        cmds += @"^FO0,0^FB"+ W + @",1,0,C^FH\^FD"+ Code.CompanyName + "^FS";
                        cmds += @"^FO10,15^FB" + W + @",1,0,L^FH\^FDModel:"+ Code.SubModel+"^FS";
                        cmds += @"^FO10,15^FB"  + (W-20) + @",1,0,R^FH\^FDInscode:"+ Code.BP_Short_code + "^FS";
                        cmds += @"^FO10,35^GB" + (W-10) + @",140,1,B,3^FS";
                        if(Code.BarOrQR)
                            cmds += @"^FO60,60^BCN,80^FD"+ pb.BarCode +"^FS";
                        else
                            cmds += @"^FO210,40^BQN,2,5^FDQA," + pb.BarCode + "^FS";
                        cmds += @"^FO10," + (H-15) + @"^FB" + W + @",1,0,L^FH\^FDPrint Date:"+ DateTime.Now.ToString("dd/MM/yyyy")+"^FS";
                        cmds += @"^FO10," + (H-15) + @"^FB" + W + @",1,0,C^FH\^FDShift:"+ Code.ShiftName + "^FS";
                        cmds += @"^FO10," + (H-15) + @"^FB" + W + @",1,0,R^FH\^FDIns. by:"+ Code.BPName + "^FS";
                        cmds += @"^XZ\n";
                    }
                    return Ok(cmds);
                }
            }

            catch (MySqlException ex)
            {
                try
                { myTrans.Rollback(); }
                catch { }
                return StatusCode(600, ex.Message);
            }
            catch (Exception ex)
            {
                try
                { myTrans.Rollback(); }
                catch { }
                return StatusCode(500, ex.Message);
            }
            finally
            {
                connection.Close();
                //doc.Close();
            }
        }

        private System.Drawing.Image GetImage(string barCode,bool CodeType,float StickerWidth, float StickerHeight,string SubModel,string CompanyName,string BPCode, string BP_Name,string ShiftName)
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
            int height = height1-50; // height of the Qr Code
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
            g.DrawString(BPShortCode, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point(bitmap.Width - ((int)(4.1951219512195121951219512195122f * BPShortCode.Length)+14), 12));
            //Left Bottom
            g.DrawString(date, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point(4, bitmap.Height - 24));
            //Right Bottomr
            g.DrawString(BPName, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point(bitmap.Width - ((int)(4.1951219512195121951219512195122f * BPName.Length)+14), bitmap.Height -  28));
            //Bottom Center
            g.DrawString(Shift, new System.Drawing.Font("Arial Narrow", 8, FontStyle.Regular), Brushes.Black, new Point((bitmap.Width - (int)(4.1951219512195121951219512195122f * Shift.Length)) / 2, bitmap.Height -  16));

            g.DrawImage((System.Drawing.Image)qrCodeWriter.Write(barCode).Clone(), new System.Drawing.Rectangle((width1-width)/2, (height1-height)/2 + 2, width, height), new System.Drawing.Rectangle(0, 0, width, height), GraphicsUnit.Pixel);

            return (System.Drawing.Image)(bitmap);
        }

        
    }
}
