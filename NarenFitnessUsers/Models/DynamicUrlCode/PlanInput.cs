using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.DynamicUrlCode
{
    public class PlanInput
    {
        public string MembershipCode { get; set; }
        public string MobileNo { get; set; }
        public string PromoCode { get; set; }
        public string CreatedBy { get; set; }
        public string PID { get; set; }

    }
}