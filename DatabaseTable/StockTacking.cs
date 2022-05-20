using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class StockTackingMaster
    {
        public int? StocktakingMasterID { get; set; }
        public DateTime? Date { get; set; }
        public int? Location { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public string BPShortCode { get; set; }
    }
    public class StockTackingDetails
    {
        public int? StocktakingMasterID { get; set; }
        public string BarCode { get; set; }
        public int? ERPNo { get; set; }
        public int? StoregesID { get; set; }
    }
    public class StockTacckig
    {
        public string Barcode { get; set; }
        public int? ERPNumberID { get; set; }
        public int? Storage_id { get; set; }
        public int? Subpart_id { get; set; }
        public string Subpart_Name { get; set; }
        public int? ColorID { get; set; }
        public int? AssemblyID { get; set; }
        public int? StocktakingMasterID { get; set; }
        public string Code { get; set; }
    }
}
