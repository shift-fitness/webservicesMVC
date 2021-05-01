using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Employee
{
    public class HREmpExperienceDetails
    {
        public int ID { get; set; }
        public string OrganizationName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Role { get; set; }
        public string AwardsRewards { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
    }
}