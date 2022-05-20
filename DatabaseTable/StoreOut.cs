using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MFG_Tracker.DatabaseTable
{
    public class StoreOut
    {
        public int? DispatchPlanID { get; set; }
        public string AgainstBarcode { get; set; }
        public string Barcode { get; set; }
        public bool? TrollyBox { get; set; }
        public string TrollyBarcode { get; set; }
        public string Compamy_Id { get; set; }
        public string Plant_Id { get; set; }
        public string BPShortCode { get; set; }
    }
    public class DispatchOutSumarry { 
        public int DispatchItemsID { get; set; }
        public int ERPNo { get; set; }
        public string Subpart_Name { get; set; }
        public string ColorcolName { get; set; }
        public string AssemblyName { get; set; }
        public int Quantity { get; set; }
        public int AllowNextScan { get; set; }
        public int Dispatched { get; set; }
    }
    public class DPlan
    {
        public int ERPNo { get; set; }
        public int Quantity { get; set; }
        public int AllowNextScan { get; set; }
        public int DispatchItemsID { get; set; }
    }
    public class BarcodeList
    {
        public string Barcode { get; set; }        
        public string Subpart_Name { get; set; }
        public string ColorcolName { get; set; }
        public string AssemblyName { get; set; }
    }
    public class BarcodeListWithCount
    {
        public int remeningQty { get; set; }
        public int nextscan { get; set; }
        public List<BarcodeList> BList { get; set; }
    }
    public class BarcodeListPrint
    {
        public int? DispatchItemsID { get; set; }
        public string Barcode { get; set; }
        public string Subpart_Name { get; set; }
        public string ColorcolName { get; set; }
        public string AssemblyName { get; set; }
        public string ERPNumbeer { get; set; }
        public string Trolley { get; set; }
    }
    public class DispatchRept
    {
        public string text { get; set; }
    }
}
