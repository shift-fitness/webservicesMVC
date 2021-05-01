using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility.AllFacilities.Hold
{
    public class HoldDetails
    {
        public string Name { get; set; }
        public int MFDID { get; set; }
        public DateTime HoldStartDate { get; set; }
        public DateTime HoldEndDate { get; set; }
        public int Freequency { get; set; }
        public int NoOfOptions { get; set; }
        public int NoOfOptionsCurrentlyInUse { get; set; }
        public int RemainingDays { get; set; }
        public Boolean IsHoldUsed { get; set; }
    }
}