using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility
{
    public class FacilityHeader
    {
      
        public int HeaderId { get; set; }
        public string FacilityName { get; set; }
        public string HeaderName { get; set; }
        public int SMFDID { get; set; }
        public string CreatedBy { get; set; }
    }
}