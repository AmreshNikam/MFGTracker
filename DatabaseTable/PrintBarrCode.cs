using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class PrintBarrCode
    {
        public string BarCode { get; set; }
        public int? PlanExecutionID { get; set; }
        public string BP_Short_code { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
    }
    public class BarrCode
    {
        public int? Quantity { get; set; }
        public int? PlanExecutionID { get; set; }
        public int? CodeType { get; set; }
        public string SubModel { get; set; }
        public string BP_Short_code { get; set; }
        public string BPName { get; set; }
        public string ShiftName { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public string CompanyName { get; set; }
        public bool BarOrQR { get; set; }
        public bool Extra { get; set; }
    }
    
}
