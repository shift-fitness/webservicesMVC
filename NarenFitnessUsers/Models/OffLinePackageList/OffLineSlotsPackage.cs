using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLineSlotsPackage
    {

        public int sNo { get; set; }
        public string date { get; set; }
        public string slotCode { get; set; }
        public string sessionCode { get; set; }
        public string packageCode { get; set; }
        public int isSlotAvailable { get; set; }
        public double planCost { get; set; }
        public string duration { get; set; }
        public int noOfDays { get; set; }
        public string durationCode { get; set; }
        public string planCode { get; set; }
        public string planName { get; set; }
        public string trainerCode { get; set; }
        public string trainerName { get; set; }
        public double slotPrice { get; set; }
        public string slotStartTime { get; set; }
        public string slotEndTime { get; set; }
        public float discountAmount { get; set; }
        public float discountPercentage { get; set; }
        public float perMonthPrice { get; set; }
        public string planCostCode { get; set; }



    }
}