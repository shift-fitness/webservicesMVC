using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.PromoCode
{
    public class PromoCodePost
    {
        public string DurationID { get; set; }
        public string Status { get; set; }
        public string ID { get; set; }
        public string CreatedBy { get; set; }
        public string AddDays { get; set; }
        public string PromoCodeType { get; set; }
        public string PromoCodeTypeName { get; set; }
        public string PromoCodeName { get; set; }
        public string PromoCodeDescription { get; set; }
        public string FacilityApplicable { get; set; }
        public string TermsAndConditions { get; set; }
        public string PromoCodeStartDate { get; set; }
        public string PromoCodeEndDate { get; set; }
        public string Mode { get; set; }
        public string BranchCode { get; set; }
        public string PromoCode { get; set; }
        public string CodeName { get; set; }
        public string DiscountAmount { get; set; }
        public string DiscountPercentage { get; set; }
        public string PackageCode { get; set; }
        public string SlotCode { get; set; }
        public string MobileNo { get; set; }
        public string Register { get; set; }

    }
}