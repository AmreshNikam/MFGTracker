using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class ScanStation
    {
        public int? ID { get; set; }
        public string ScanstationName { get; set; }
        public int? WorkStationID { get; set; }
        public string Workstation_name { get; set; }
        public int? OkDefectCode { get; set; }
        public string DefectbarCode { get; set; }
        public int? StoregeID { get; set; }
        public int? Stores_id { get; set; }
        public string Stores_Name { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public string Code { get; set; }
    }
    public class MoldScan
    {
        public string Barcode { get; set; }
        public int? Workstation_Id { get; set; }
        public int? SoregeID { get; set; }
        public int DefectCode { get; set; }
        public int? StartSkidNo { get; set; }
        public string BP_ShortCode { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public int OkCode { get; set; }

    }
    public class AssemblyScan
    {
        public string Barcode { get; set; }
        public int? Workstation_Id { get; set; }
        public int? SoregeID { get; set; }
        public int DefectCode { get; set; }
        public int AssemblyID { get; set; }
        public string BP_ShortCode { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public int OkCode { get; set; }

    }

}
