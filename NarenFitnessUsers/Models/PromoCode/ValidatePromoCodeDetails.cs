using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.PromoCode
{
    public class ValidatePromoCodeDetails
    {
        public int ID { get; set; }
        public string PromoCode { get; set; }
        public string PromoCodeName { get; set; }
        public string PromoCodeDescription { get; set; }
        public string FacilityApplicable { get; set; }
        public int AddDays { get; set; }
        public float DiscountAmount { get; set; }
        public string DiscountPercentage { get; set; }
        public string TermsAndConditions { get; set; }
        public string PromoCodeStartDate { get; set; }
        public string PromoCodeEndDate { get; set; }
       
    }
}