using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MFG_Tracker.DatabaseTable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MFG_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarCodeSettingsController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public BarCodeSettingsController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        // GET: api/<BarCodeSettingsController>
        [HttpGet("{PLANTID}/{COMPANYID}")]
        public IActionResult Get(string PLANTID, string COMPANYID)
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
                    bs.PrinterDPI = Convert.ToInt32(rdr["PrinterDPI"]);
                    bs.PageWidth = float.Parse(rdr["PageWidth"].ToString());
                    bs.PageHeight = float.Parse(rdr["PageHeight"].ToString());
                    bs.StickerWidth = float.Parse(rdr["StickerWidth"].ToString());
                    bs.StickerHeight = float.Parse(rdr["StickerHeight"].ToString());
                    bs.Rows = Convert.ToInt32(rdr["Rowes"]);
                    bs.Columns = Convert.ToInt32(rdr["Columns"]);
                    bs.CellPadingLeft = float.Parse(rdr["CellPadingLeft"].ToString());
                    bs.CellPadingTop = float.Parse(rdr["CellPadingTop"].ToString());
                    bs.CellPadingRight = float.Parse(rdr["CellPadingRight"].ToString());
                    bs.CellPadingBottom = float.Parse(rdr["CellPadingBottom"].ToString());
                    bs.PageMarginLeft = float.Parse(rdr["PageMarginLeft"].ToString());
                    bs.PageMarginTop = float.Parse(rdr["PageMarginTop"].ToString());
                    bs.PageMarginRight = float.Parse(rdr["PageMarginRight"].ToString());
                    bs.PageMarginBottom = float.Parse(rdr["PageMarginBottom"].ToString());
                    bs.AcrossThenDown = Convert.ToBoolean(rdr["AcrossThenDown"]);
                    bs.VerticalGap = float.Parse(rdr["VerticalGap"].ToString());
                    bs.HorizantalGap = float.Parse(rdr["HorizantalGap"].ToString());
                    bs.PageOriantation = Convert.ToBoolean(rdr["PageOriantation"]);
                    bs.BarOrQR = Convert.ToBoolean(rdr["BarOrQR"]);
                    bs.Plant_Id = rdr["Plant_Id"].ToString();
                    bs.Company_Id = rdr["Company_Id"].ToString();
                }
                return Ok(bs);
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

        // GET api/<BarCodeSettingsController>/5
        

        // POST api/<BarCodeSettingsController>
        [HttpPost]
        public IActionResult Post([FromBody] BarCodeSettings bs)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Insert into BarCodeSettings (PageWidth,PageHeight,StickerWidth,StickerHeight,Rowes,Columns,CellPadingLeft,CellPadingTop,CellPadingRight,CellPadingBottom,PageMarginLeft,PageMarginTop,PageMarginRight,PageMarginBottom,AcrossThenDown,VerticalGap,HorizantalGap,PageOriantation,BarOrQR,Plant_Id,Company_Id,PrinterType,PrinterDPI) values (@PageWidth,@PageHeight,@StickerWidth,@StickerHeight,@Rows,@Columns,@CellPadingLeft,@CellPadingTop,@CellPadingRight,@CellPadingBottom,@PageMarginLeft,@PageMarginTop,@PageMarginRight,@PageMarginBottom,@AcrossThenDown,@VerticalGap,@HorizantalGap,@PageOriantation,@BarOrQR,@Plant_Id,@Company_Id,@PrinterType,@PrinterDPI)";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                cmd.Parameters.Add(new MySqlParameter("@PageWidth", MySqlDbType.Float)).Value = bs.PageWidth;
                cmd.Parameters.Add(new MySqlParameter("@PageHeight", MySqlDbType.Float)).Value = bs.PageHeight;
                cmd.Parameters.Add(new MySqlParameter("@StickerWidth", MySqlDbType.Float)).Value = bs.StickerWidth;
                cmd.Parameters.Add(new MySqlParameter("@StickerHeight", MySqlDbType.Float)).Value = bs.StickerHeight;
                cmd.Parameters.Add(new MySqlParameter("@Rows", MySqlDbType.Int32)).Value = bs.Rows;
                cmd.Parameters.Add(new MySqlParameter("@Columns", MySqlDbType.Int32)).Value = bs.Columns;
                cmd.Parameters.Add(new MySqlParameter("@CellPadingLeft", MySqlDbType.Float)).Value = bs.CellPadingLeft;
                cmd.Parameters.Add(new MySqlParameter("@CellPadingTop", MySqlDbType.Float)).Value = bs.CellPadingTop;
                cmd.Parameters.Add(new MySqlParameter("@CellPadingRight", MySqlDbType.Float)).Value = bs.CellPadingRight;
                cmd.Parameters.Add(new MySqlParameter("@CellPadingBottom", MySqlDbType.Float)).Value = bs.CellPadingBottom;
                cmd.Parameters.Add(new MySqlParameter("@PageMarginLeft", MySqlDbType.Float)).Value = bs.PageMarginLeft;
                cmd.Parameters.Add(new MySqlParameter("@PageMarginTop", MySqlDbType.Float)).Value = bs.PageMarginTop;
                cmd.Parameters.Add(new MySqlParameter("@PageMarginRight", MySqlDbType.Float)).Value = bs.PageMarginRight;
                cmd.Parameters.Add(new MySqlParameter("@PageWidth", MySqlDbType.Float)).Value = bs.PageMarginBottom;
                cmd.Parameters.Add(new MySqlParameter("@PageMarginBottom", MySqlDbType.Float)).Value = Convert.ToInt16(bs.AcrossThenDown);
                cmd.Parameters.Add(new MySqlParameter("@VerticalGap", MySqlDbType.Float)).Value = bs.VerticalGap;
                cmd.Parameters.Add(new MySqlParameter("@HorizantalGap", MySqlDbType.Float)).Value = bs.HorizantalGap;
                cmd.Parameters.Add(new MySqlParameter("@PageOriantation", MySqlDbType.Float)).Value = Convert.ToInt16(bs.PageOriantation);
                cmd.Parameters.Add(new MySqlParameter("@BarOrQR", MySqlDbType.Int16)).Value = Convert.ToInt16(bs.BarOrQR);
                cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.Float)).Value = bs.Plant_Id;
                cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.Float)).Value = bs.Company_Id;
                cmd.Parameters.Add(new MySqlParameter("@PrinterType", MySqlDbType.Int16)).Value = Convert.ToInt16(bs.PrinterType);
                cmd.Parameters.Add(new MySqlParameter("@PrinterDPI", MySqlDbType.Int32)).Value = bs.PrinterDPI;
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

        // PUT api/<BarCodeSettingsController>/5
        [HttpPut("{PLANTID}/{COMPANYID}")]
        public IActionResult Put(string PLANTID, string COMPANYID, [FromBody] BarCodeSettings bs)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            try
            {
                connection.Open();
                string sql = "Update BarCodeSettings set PageWidth = @PageWidth, PageHeight = @PageHeight, StickerWidth = @StickerWidth, StickerHeight = @StickerHeight, Rowes = @Rows, Columns = @Columns, CellPadingLeft = @CellPadingLeft, CellPadingTop = @CellPadingTop, CellPadingRight = @CellPadingRight, CellPadingBottom = @CellPadingBottom, PageMarginLeft = @PageMarginLeft, PageMarginTop = @PageMarginTop, PageMarginRight = @PageMarginRight, PageMarginBottom = @PageMarginBottom, AcrossThenDown = @AcrossThenDown, VerticalGap = @VerticalGap, HorizantalGap = @HorizantalGap, PageOriantation = @PageOriantation,BarOrQR = @BarOrQR, Plant_Id = @Plant_Id, Company_Id = @Company_Id,PrinterType = @PrinterType, PrinterDPI = @PrinterDPI where Plant_Id = '" + PLANTID + "' and Company_Id = '" + COMPANYID + "'"; 
                //string sql = "Update BarCodeSettings set PageWidth = @PageWidth, PageHeight = @PageHeight, StickerWidth = @StickerWidth, StickerHeight = @StickerHeight, Columns = @Columns, CellPadingLeft = @CellPadingLeft, CellPadingTop = @CellPadingTop, CellPadingRight = @CellPadingRight, CellPadingBottom = @CellPadingBottom, PageMarginLeft = @PageMarginLeft, PageMarginTop = @PageMarginTop, PageMarginRight = @PageMarginRight, PageMarginBottom = @PageMarginBottom, AcrossThenDown = @AcrossThenDown, VerticalGap = @VerticalGap, HorizantalGap = @HorizantalGap, PageOriantation = @PageOriantation, Plant_Id = @Plant_Id, Company_Id = @Company_Id where Plant_Id = '" + PLANTID + "' and Company_Id = '" + COMPANYID + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                cmd.Parameters.Add(new MySqlParameter("@PageWidth", MySqlDbType.Float)).Value = bs.PageWidth;
                cmd.Parameters.Add(new MySqlParameter("@PageHeight", MySqlDbType.Float)).Value = bs.PageHeight;
                cmd.Parameters.Add(new MySqlParameter("@StickerWidth", MySqlDbType.Float)).Value = bs.StickerWidth;
                cmd.Parameters.Add(new MySqlParameter("@StickerHeight", MySqlDbType.Float)).Value = bs.StickerHeight;
                cmd.Parameters.Add(new MySqlParameter("@Rows", MySqlDbType.Int32)).Value = bs.Rows;
                cmd.Parameters.Add(new MySqlParameter("@Columns", MySqlDbType.Int32)).Value = bs.Columns;
                cmd.Parameters.Add(new MySqlParameter("@CellPadingLeft", MySqlDbType.Float)).Value = bs.CellPadingLeft;
                cmd.Parameters.Add(new MySqlParameter("@CellPadingTop", MySqlDbType.Float)).Value = bs.CellPadingTop;
                cmd.Parameters.Add(new MySqlParameter("@CellPadingRight", MySqlDbType.Float)).Value = bs.CellPadingRight;
                cmd.Parameters.Add(new MySqlParameter("@CellPadingBottom", MySqlDbType.Float)).Value = bs.CellPadingBottom;
                cmd.Parameters.Add(new MySqlParameter("@PageMarginLeft", MySqlDbType.Float)).Value = bs.PageMarginLeft;
                cmd.Parameters.Add(new MySqlParameter("@PageMarginTop", MySqlDbType.Float)).Value = bs.PageMarginTop;
                cmd.Parameters.Add(new MySqlParameter("@PageMarginRight", MySqlDbType.Float)).Value = bs.PageMarginRight;
                cmd.Parameters.Add(new MySqlParameter("@PageMarginBottom", MySqlDbType.Float)).Value = bs.PageMarginBottom;
                cmd.Parameters.Add(new MySqlParameter("@AcrossThenDown", MySqlDbType.Int16)).Value = Convert.ToInt16(bs.AcrossThenDown);
                cmd.Parameters.Add(new MySqlParameter("@VerticalGap", MySqlDbType.Float)).Value = bs.VerticalGap;
                cmd.Parameters.Add(new MySqlParameter("@HorizantalGap", MySqlDbType.Float)).Value = bs.HorizantalGap;
                cmd.Parameters.Add(new MySqlParameter("@PageOriantation", MySqlDbType.Int16)).Value = Convert.ToInt16(bs.PageOriantation);
                cmd.Parameters.Add(new MySqlParameter("@BarOrQR", MySqlDbType.Int16)).Value = Convert.ToInt16(bs.BarOrQR);
                cmd.Parameters.Add(new MySqlParameter("@Plant_Id", MySqlDbType.VarChar)).Value = bs.Plant_Id;
                cmd.Parameters.Add(new MySqlParameter("@Company_Id", MySqlDbType.VarChar)).Value = bs.Company_Id;
                cmd.Parameters.Add(new MySqlParameter("@PrinterType", MySqlDbType.Int16)).Value = Convert.ToInt16(bs.PrinterType);
                cmd.Parameters.Add(new MySqlParameter("@PrinterDPI", MySqlDbType.Int32)).Value = bs.PrinterDPI;
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

    }
}
