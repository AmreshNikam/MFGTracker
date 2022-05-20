using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class ERPnumber
    {
        public int? ERPNumberID { get; set; }
        public int? Subpart_id { get; set; }
        public int? ColorID { get; set; }
        public int? AssemblyID { get; set; }
        public string ERPNumber { get; set; }
        public string SAPMaterialNumber { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public int? Part_id { get; set; }
    }
    public class ERPnumberDetails
    {
        public int? ERPNumberID { get; set; }
        public int? Part_id { get; set; }
        public string Part_name { get; set; }
        public int? Subpart_id { get; set; }
        public string Subpart_Name { get; set; }
        public int? ColorID { get; set; }
        public string ColorcolName { get; set; }
        public int? AssemblyID { get; set; }
        public string AssemblyName { get; set; }
        public string ERPNumber { get; set; }
        public string SAPMaterialNumber { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }

    }
}
