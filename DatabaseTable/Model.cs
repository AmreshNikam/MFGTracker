using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class Model
    {
        public int? Part_id { get; set; }
        public string Part_name { get; set; }
        public string BP_Short_code { get; set; }
        public string Plant_Id { get; set; }
        public string Company_id { get; set; }
    }
}
