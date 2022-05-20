using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class BarcodeDetails
    {
        public string Barcode { get; set; }
        public int? Workstation_Id { get; set; }
        public string DefectCode { get; set; }
        public DateTime? TimeStamp { get; set; }
        public int? BP_ShortCode { get; set; }
        public int? ExecutionType { get; set; }
        public int? ExecutionPlanNo { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
    }
}
