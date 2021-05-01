using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility
{
    public class FacilityDescription
    {
        public string ID { get; set; }
        public string SMFDID { get; set; }
        public string FacilityName { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
    }
}