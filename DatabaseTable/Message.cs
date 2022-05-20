using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class Message
    {
        public int? ID { get; set; }
        public string MessageText { get; set; }
        public int? ScanStation { get; set; }
    }
}
