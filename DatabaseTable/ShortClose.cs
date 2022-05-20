using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class ShortClose
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public Int32? ExcecutionPlan { get; set; }
        public Int32? TackTime { get; set; }
        public string BP_Short_code { get; set; }
        public string Plant_ID { get; set; }
        public string Company_ID { get; set; }
        public string Note { get; set; }
    }
}
