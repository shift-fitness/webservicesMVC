using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility
{
    public class FacilityObject
    {
        public int SMFMID { get; set; }
        public string FacilityName { get; set; }
        public double FacilityPrice { get; set; }
        public List<FacilityHeaderALL> header { get; set; }
        public List<FacilityDescriptionALL> description { get; set; }


    }
}