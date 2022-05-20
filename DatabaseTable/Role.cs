using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string Json { get; set; }
        public string  Company_Id { get; set; }
        public string Plant_Id { get; set; }
    }
   
}
