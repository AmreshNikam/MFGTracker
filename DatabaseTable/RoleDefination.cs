using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class RoleDefination
    {
        public int value { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
        public List<Role> children { get; set; }
    }
    public class quadruplets
    {
        public int ID { get; set; }
        public int? Parent { get; set; }
        public string Node { get; set; }
        public bool value { get; set; }
    }
}
