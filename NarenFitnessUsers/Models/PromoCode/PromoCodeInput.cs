using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.PromoCode
{
    public class PromoCodeInput
    {
        public string ExpireDate { get; set; }
        public string DurationId { get; set; }
        public string Register { get; set; }
        public string PackageCode { get; set; }
        public string SlotCode { get; set; }
        public string MobileNo { get; set; }
        public string PromoCode { get; set; }
        public string BranchCode { get; set; }
    }
}