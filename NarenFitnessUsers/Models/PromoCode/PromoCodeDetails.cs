using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.PromoCode
{
    public class PromoCodeDetails
    {
     
        public int ID { get; set; }
        public string PromoCode { get; set; }
        public string PromoCodeName { get; set; }
        public string PromoCodeDescription { get; set; }
        public string FacilityApplicable { get; set; }
        public int AddDays { get; set; }
        public float DiscountAmount { get; set; }
        public int DiscountPercentage { get; set; }
        public string TermsAndConditions { get; set; }
        public string PromoCodeStartDate { get; set; }
        public string PromoCodeEndDate { get; set; }
        public int PromoCodeType { get; set; }
        public string PromoCodeTypeName { get; set; }
        public string DurationID { get; set; }
        public Boolean  IsActive { get; set; }







    }
}