using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class UserCredentials
    {
        public string User_name { get; set; }
        public string Password { get; set; }
        public int? Role { get; set; }
        public bool? Active_Inactive { get; set; }
        public string BP_ShortCode { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
    }
    public class Login
    {
        public string User_name { get; set; }
        public string Password { get; set; }
        public string Plant_Id { get; set; }
        public string Company_Id { get; set; }
    }
    public class LoginResonse
    {
        public string LoginName { get; set; }
        public string Password { get; set; }
        public bool Active_Inactive { get; set; }
        public string BPShortCode { get; set; }
        public string UserName { get; set; }
        public string Json { get; set; }
     }
}
