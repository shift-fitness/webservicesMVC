using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility
{
    public class FacilityDetailsOutput
    {
        public string status { get; set; }

        public List<FacilityDetails> value { get; set; }
    }
}