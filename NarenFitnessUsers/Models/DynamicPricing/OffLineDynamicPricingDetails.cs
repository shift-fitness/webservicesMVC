using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.DynamicPricing
{
    public class OffLineDynamicPricingDetails
    {
        public string slotCode { get; set; }
        public string sessionCode { get; set; }
        public string packageCode { get; set; }
        public string isSlotAvailable { get; set; }
        
        public string duration { get; set; }
        public string noOfDays { get; set; }
        public string durationCode { get; set; }
        public string planCode { get; set; }

        public string planCost { get; set; }
        public string planName { get; set; }
        public string trainerCode { get; set; }
        public string trainerName { get; set; }
        public string slotPrice { get; set; }
        public string slotStartTime { get; set; }
        public string slotEndTime { get; set; }
        public string DiscountAmount { get; set; }
        public string DiscountPercentage { get; set; }
        public string PerMonthPrice { get; set; }


    }
}