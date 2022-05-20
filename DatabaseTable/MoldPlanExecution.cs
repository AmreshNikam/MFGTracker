using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class MoldPlanExecution
    {
        public int? MoldPlanExecutionID { get; set; }
        public int? ModelPlanID { get; set; }
        public int? WorkstationID { get; set; }
        public int? ShiftID { get; set; }
        public DateTime? DateOfExecution { get; set; }
        public int? EstimatedInitialQuantity { get; set; }
        public int? EstimatedQuantity { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public string BPShortCode { get; set; }
        public string Notes { get; set; }
        public bool? Status { get; set; }
        public string startTime { get; set; } //this is for short close
        public int ConformCount { get; set; }
    }
    public class MoldPlanExecutionDetails
    {
        public int? MoldPlanExecutionID { get; set; }
        public int? ModelPlanID { get; set; }
        public string Part_name { get; set; }
        public string Subpart_Name { get; set; }
        public int? Tag_time { get; set; }
        public int? WorkstationID { get; set; }
        public string Workstation_name { get; set; }
        public int? ShiftID { get; set; }
        public string Shift_name { get; set; }
        public DateTime? DateOfExecution { get; set; }
        public int? EstimatedInitialQuantity { get; set; }
        public int? EstimatedQuantity { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public string BPShortCode { get; set; }
        public string BPName { get; set; }
        public string Notes { get; set; }
        public bool? Status { get; set; }
        public string company_name { get; set; }
    }
}
