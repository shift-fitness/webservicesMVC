using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLineFreeTrialOnlySlotDetails
    {
        public string sessionId { get; set; }
        public string sessionName { get; set; }
        public string sessionStartTime { get; set; }
        public string sessionEndTime { get; set; }
        public List<OffLineEnquireInfo> info { get; set; }
        public List<OffLineFreeTrialOnlySlots> slots { get; set; }
    }
}