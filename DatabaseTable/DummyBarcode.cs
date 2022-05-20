using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class DummyBarcode
    {
        public string scanstation { get; set; }
        public int SubpartID { get; set; }
        public DateTime? LoadingDate { get; set; }
        public int? ColorId { get; set; }
        public int? RoundNo { get; set; }
        public int? AssemblyId { get; set; }
        public bool AllUnloaded { get; set; }
        public int NoOfPrints { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public string BPShortCode { get; set; }
        public int Quantity { get; set; }

    }
    public class DummyBarcodeDetails
    {
        public string Subpart_Name { get; set; }
        public string ColorcolName { get; set; }
        public string AssemblyName { get; set; }
        public string Customer_material_number { get; set; }
    }
}
