using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class PaintLoading
    {
        public string Barcode { get; set; }
        public int? PaintPlanExecutionID { get; set; }
        public int? SkidNumber { get; set; }
        public string Position { get; set; }
        public string BPShortCode { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public string ID { get; set; }
        public int? ColorId { get; set; }
        public string ColorName { get; set; }
        public int? SoregeID { get; set; }
        public int? Workstation_Id { get; set; }
        public int? SubpartID { get; set; }
        public string Subpart_Name { get; set; }
        public bool LoadUnload { get; set; }
        public int Round { get; set; }
        public int OkDefectCode { get; set; }
    }
}
