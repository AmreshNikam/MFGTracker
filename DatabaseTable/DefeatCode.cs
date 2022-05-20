using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class DefeatCode
    {
        public int? defect_code_id { get; set; }
        public string DefectbarCode { get; set; }
        public string Message { get; set; }
        public int? From_Station { get; set; }
        public string From_Name { get; set; }
        public int? To_Station { get; set; }
        public string To_Name { get; set; }
        public string Plant_Id { get; set; }
        public string Company_id { get; set; }
    }
}
