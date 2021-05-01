using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class OnlineSlotTimingsResponse
    {

        public int id { get; set; }
        public string slotCode { get; set; }
        public string slotName { get; set; }
        public string slotStartTime { get; set; }
        public string slotEndTime { get; set; }
        public Boolean isActive { get; set; }
    }
}