using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class Shift
    {
        public int? Shift_id { get; set; }
        public string Shift_name { get; set; }
        public TimeSpan? Start_time { get; set; }
        public TimeSpan? End_time { get; set; }
        public int? Day { get; set; }
        public string Plant_Id { get; set; }
        public string Company_id { get; set; }
        public bool DayFirstShift { get; set; }
        public bool DayLastShift { get; set; }
    }
    public class ShiftStrTime
    {
        public int? Shift_id { get; set; }
        public string Shift_name { get; set; }
        public string Start_time { get; set; }
        public string End_time { get; set; }
        public int? Day { get; set; }
        public string Plant_Id { get; set; }
        public string Company_id { get; set; }
        public bool DayFirstShift { get; set; }
        public bool DayLastShift { get; set; }
    }
}
