using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility.UsedFacilities
{
    public class Facilities
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public List<FacilityUsedMembersDetails> FacilityUsed { get; set; }
    }
}