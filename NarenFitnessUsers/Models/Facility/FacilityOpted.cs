using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility
{
    public class FacilityOpted
    {
        public string MembershipCode { get; set; }
        public string Invoice { get; set; }
        public int Freezing { get; set; }
        public int Upgrade { get; set; }
        public int Change { get; set; }
        public int Transfer { get; set; }
        public int CreatedBy { get; set; }
        public int Paused { get; set; }
        public int Convert { get; set; }
    }
}