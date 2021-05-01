using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.FreezingFacility
{
    public class FreezingFacilityInput
    {
        public string ID { get; set; }
        public string BranchCode { get; set; }
        public string SMFMID { get; set; }
        public string FreezingName { get; set; }
        public string NoOfDays { get; set; }
        public string FreezingStartDate { get; set; }
        public string FreezingExpireDate { get; set; }
        public string CreatedBy { get; set; }

       
    }
}