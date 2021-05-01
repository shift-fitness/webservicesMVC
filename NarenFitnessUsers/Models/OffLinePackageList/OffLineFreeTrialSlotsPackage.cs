using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLineFreeTrialSlotsPackage
    {
        public string slotID { get; set; }
        public string slotName { get; set; }
        public string slotStartTime { get; set; }
        public string slotEndTime { get; set; }
        public string isAvailable { get; set; }
    }
}