using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MFG_Tracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarcodeMasterController : ControllerBase
    {
        private readonly IConfiguration Configuration;

        public BarcodeMasterController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        
        [HttpPut("{BarCode}")]
        public IActionResult Put(string Barcode, [FromQuery] int Storage)
        {
            string connString = this.Configuration.GetConnectionString("MFG_Tracker");
            MySqlConnection connection = new MySqlConnection(connString);
            connection.Open();
            try
            {
                string sql = "Update BarcodeMaster set Storage_id = " + Storage + " where Barcode = '" + Barcode + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection) { CommandType = CommandType.Text };
                
                cmd.ExecuteNonQuery();
                return Ok("Storage Location updated Successfuly");
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
