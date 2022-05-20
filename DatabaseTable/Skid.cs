using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class Skid
    {
        public int? Skid_config_id { get; set; }
        public int? JigsPerSkid { get; set; }
        public int? PartsPerJigs { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
    }
}
