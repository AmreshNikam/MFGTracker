using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class PaintPlanning
    {
        public int?  PaintPlanID { get; set; }
        public DateTime? PlanningDate { get; set; }
        public int? SubModel { get; set; }
        public int? Color { get; set; }
        public int? Quantity { get; set; }
        public int? Skid { get; set; }
        public int? Round { get; set; }
        public string BPShortCode { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public string Status { get; set; }
        public DateTime? PlanEnteryDate { get; set; }

    }
    public class PaintPlanningDetails
    {
        public int? PaintPlanID { get; set; }
        public DateTime? PlanningDate { get; set; }
        public int? Part_id { get; set; }
        public string Part_name { get; set; }
        public int? SubModel { get; set; }
        public string Subpart_Name { get; set; }
        public int? Color { get; set; }
        public string ColorcolName { get; set; }
        public int? Quantity { get; set; }
        public int? Skid { get; set; }
        public string Matrix { get; set; }
        public int? JigsPerSkid { get; set; }
        public int? PartsPerJigs { get; set; }
        public int? Round { get; set; }
        public string BPShortCode { get; set; }
        public string Name { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
        public string Status { get; set; }
        public DateTime? PlanEnteryDate { get; set; }
    }
}
