using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class BarcodeMaster
    {
        public string Barcode { get; set; }
        public int? ERPNumberID { get; set; }
        public string CurrentDefectCode { get; set; }
        public int? Storage_id { get; set; }
    }
}
