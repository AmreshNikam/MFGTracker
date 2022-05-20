using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class Assembly
    {
        public int? AssemblyID { get; set; }
        public string AssemblyName { get; set; }
        public int? Subpart_id { get; set; }
    }
}
