using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility
{
    public class FacilityObjectTest
    {
        public int SMFMID { get; set; }
        public string FacilityName { get; set; }
        public List<FacilityHeaderALL> header { get; set; }
        public List<FacilityDescriptionALL> description { get; set; }
        public Dictionary<string, object> facilityPrice2 { get; set; }
    }
}