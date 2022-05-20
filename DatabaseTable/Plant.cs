using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class Plant
    {
        public string Plant_Id { get; set; }
        public string Plant_name { get; set; }
        public string Plant_location { get; set; }
        public string Plant_address { get; set; }
        public string Company_id { get; set; }
        public char? PlantIdentificationChar { get; set; }
    }
}
