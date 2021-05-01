using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility.AllFacilities
{
    public class AllFacilityOutput
    {
        public string status { get; set; }
        public List<AllFacilityCombinedList> value { get; set; }
    }
}