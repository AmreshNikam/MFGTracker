using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class Workstation
    {
        public int? Workstation_Id { get; set; }
        public string Workstation_name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
    }
}
