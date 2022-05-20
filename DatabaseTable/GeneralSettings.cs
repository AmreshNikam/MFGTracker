using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class GeneralSettings
    {
        public bool? UseTackTime { get; set; }
        public bool? RelaxsessionShiftStart { get; set; }
        public int? TotalSkids { get; set; }
        public int? RoundNo { get; set; }
        public bool? LeftToTight { get; set; }
        public bool? AcrossThenDown { get; set; }
        public bool? DownThenAcross { get; set; }
        public bool? AcrossThenUP { get; set; }
        public bool? UPThenAcross { get; set; }
        public int? CustomerID { get; set; }
        public string Plant_ID { get; set; }
        public string Company_ID { get; set; }
    }
}
