using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class OnlineSlotTimings
    {
        public string id { get; set; }
        public string createdBy { get; set; }
        public string slotCode { get; set; }
        public string slotName { get; set; }
        public string sessionCode { get; set; }
        public string slotStartTime { get; set; }
        public string slotEndTime { get; set; }
        public string description { get; set; }
    }
}