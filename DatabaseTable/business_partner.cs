using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class BusinessPartner
    {
        public string BP_Short_code { get; set; }
        public string Name { get; set; }
        public int? BP_Type { get; set; }
        public string Email { get; set; }
        public string Mobile_number { get; set; }
        public string Plant_Id { get; set; }
        public string Company_id { get; set; }
    }
}
