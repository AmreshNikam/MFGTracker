using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class MoldPlanning
    {
        public int? MoldPlanningID { get; set; }
        public DateTime? PlanningDate { get; set; }
        public int? WorkstationID { get; set; }
        public int? SubModel { get; set; }
        public int? Quantity { get; set; }
        public string Status { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public string BPShortCode { get; set; }
        public DateTime? PlanEnteryDate { get; set; }


    }
    public class MoldPlanningDetails
    {
        public int? MoldPlanningID { get; set; }
        public DateTime? PlanningDate { get; set; }
        public int? WorkstationID { get; set; }
        public string Workstation_name { get; set; }
        public int? SubModel { get; set; }
        public string Subpart_Name { get; set; }
        public int? tagtime { get; set; }
        public int? Part_id { get; set; }
        public string Part_name { get; set; }
        public int? Quantity { get; set; }
        public int? OK { get; set; }
        public int? Rejected { get; set; }
        public int? Total { get; set; }
        public int? Balance { get; set; }
        public string Status { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public string BPShortCode { get; set; }
        public string BP_Name { get; set; }
        public DateTime? PlanEnteryDate { get; set; }
    }
}
