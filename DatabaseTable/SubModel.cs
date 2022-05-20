using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class SubModel
    {
        public int? Subpart_id { get; set; }
        public string Subpart_Name { get; set; }
        public int? Part_id { get; set; }
        public int? Tag_time { get; set; }
        public string Customer_material_number { get; set; }
        public int? Year { get; set; }
        public int? CountOk { get; set; }
        public int? Month { get; set; }
        public int? PlanID { get; set; }
    }
}
