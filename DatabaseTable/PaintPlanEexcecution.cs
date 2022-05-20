using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class PaintPlanEexcecution
    {
        public int? PaintPlanExecutionID { get; set; }
        public int? PaintPlanID { get; set; }
        public DateTime? DateOfExecution { get; set; }
        public int? EstimatedQuantity { get; set; }
        public int? StartSkidNumber { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public string BPShortCode { get; set; }
        public bool? Status { get; set; }
        public int? ActualRoundNo { get; set; }
    }
    public class PaintPlanEexcecutionDetails
    {
        public int? PaintPlanExecutionID { get; set; }
        public int? PaintPlanID { get; set; }
        public DateTime? DateOfExecution { get; set; }
        public int? Part_id { get; set; }
        public string Part_name { get; set; }
        public int? SubModel { get; set; }
        public string Subpart_Name { get; set; }
        public int? Color { get; set; }
        public string ColorcolName { get; set; }
        public int? Skid { get; set; }
        public string Matrix { get; set; }
        public int? Round { get; set; }
        public int? ActualRoundNo { get; set; }
        public int? EstimatedQuantity { get; set; }
        public int? StartSkidNumber { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public string BPShortCode { get; set; }
        public string BPName { get; set; }
    }
}
