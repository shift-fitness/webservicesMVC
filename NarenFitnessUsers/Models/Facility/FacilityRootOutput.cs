using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility
{
    public class FacilityRootOutput
    {
        public string status { get; set; }
        public List<FacilityRoot> value { get; set; }
    }
}