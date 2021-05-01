using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility.AllFacilities.Freezing
{
    public class FreezingDetails
    {
        public string Name { get; set; }
        public int MFDID { get; set; }
        public DateTime FreezingStartDate { get; set; }
        public DateTime FreezingEndDate { get; set; }
        public int Freequency { get; set; }
        public int NoOfOptions { get; set; }
        public int NoOfOptionsCurrentlyInUse { get; set; }
        public int RemainingDays { get; set; }
        public Boolean IsFreezingUsed { get; set; }

    }


}