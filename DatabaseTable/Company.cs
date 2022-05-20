using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MFG_Tracker.DatabaseTable
{
    public class Company
    {
        public string Company_ID { get; set; }
        public string Company_Name { get; set; }
        public string Company_Address { get; set; }
        public string LogoFileExtension { get; set; }
        public byte[] LogoFile { get; set; }
    }
}
