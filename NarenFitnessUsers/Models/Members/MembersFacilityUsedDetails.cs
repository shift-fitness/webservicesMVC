using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Members
{
    public class MembersFacilityUsedDetails
    {
        public DateTime FacilityStartDate { get; set; }
        public DateTime FacilityExpireDate { get; set; }
        public string FacilityName { get; set; }
        public DateTime CreatedOn { get; set; }
        public int NoOfDays { get; set; }
    }
}