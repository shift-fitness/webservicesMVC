using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility.UsedFacilities
{
    public class FacilityUsedOutput
    {
        public string status { get; set; }
        public List<Facilities> value { get; set; }
    }
}