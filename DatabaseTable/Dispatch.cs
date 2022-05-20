using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class Dispatch
    {
        public int? ERPNumberID { get; set; }
        public int? Subpart_id { get; set; }
        public string Subpart_Name { get; set; }
        public int? ColorID { get; set; }
        public string ColorcolName { get; set; }
        public int? AssemblyID { get; set; }
        public string AssemblyName { get; set; }
        public int? InStock { get; set; }

    }
    public class DispatchBarcode
    {
        public string BarCode { get; set; }
        public int? DispatchItemsID { get; set; }
    }
    public class DispatchItems
    {
        public int?  DispatchItemsID { get; set; }
        public int? DispatchPlanID { get; set; }
        public int? ERPNo { get; set; }
        public int? Quantity { get; set; }
    }
    public class DispatchPlan
    {
        public int? DispatchPlanID { get; set; }
        public DateTime? DateOfPlan { get; set; }
        public int? AllowNextScan { get; set; }
        public string BPShortCode { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public int Status { get; set; }
        public DateTime? PlanEnteryDate { get; set; }
        public List<DispatchItems> dispatchItems { get; set; }
    }
    public class DispatchDetailsArray
    {
        public int? DispatchPlanID { get; set; }
        public DateTime? PlanEnteryDate { get; set; }
        public DateTime? DateOfPlan { get; set; }
        public int? AllowNextScan { get; set; }
        public int? DispatchItemsID { get; set; }
        public int? Quantity { get; set; }
        public int? ERPNo { get; set; }
        public int? Part_id { get; set; }
        public int? Subpart_Id { get; set; }
        public string Subpart_Name { get; set; }
        public int? ColorID { get; set; }
        public string ColorcolName { get; set; }
        public int? AssemblyID { get; set; }
        public string AssemblyName { get; set; }
        public string BarCode { get; set; }
    }
    public class DispatchDetails
    {
        public int? DispatchPlanID { get; set; }
        public DateTime? PlanEnteryDate { get; set; }
        public DateTime? DateOfPlan { get; set; }
        public int? AllowNextScan { get; set; }
        public List<DispatchItmsDetails> dispatchItmsDetails { get; set; }
    }
    public class DispatchItmsDetails
    {
        public int? DispatchItemsID { get; set; }
        public int? Quantity { get; set; }
        public int? ERPNo { get; set; }
        public int? Subpart_Id { get; set; }
        public string Subpart_Name { get; set; }
        public int? ColorID { get; set; }
        public string ColorcolName { get; set; }
        public int? AssemblyID { get; set; }
        public string AssemblyName { get; set; }
    }
    
}
