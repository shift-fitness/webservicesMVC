using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility
{
    public class FacilityDescriptionOutput
    {
        public string status { get; set; }
        public List<FacilityDescription> value { get; set; }
    }
}