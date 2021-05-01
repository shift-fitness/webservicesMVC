using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.FreezingFacility
{
    public class FreezingDetails
    {

        public int FreezingID { get; set; }
        public int SMFMID { get; set; }
        public string FreezingName { get; set; }
        public int NoOfDays { get; set; }
        public DateTime FreezingStartDate { get; set; }
        public DateTime FreezingExpireDate { get; set; }
    }
}