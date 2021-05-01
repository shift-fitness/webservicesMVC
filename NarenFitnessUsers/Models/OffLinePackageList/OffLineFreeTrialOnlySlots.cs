using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLineFreeTrialOnlySlots
    {

       
        public int sNo { get; set; }
        public string slotId { get; set; }
        public string slotStartTime { get; set; }
        public string slotEndTime { get; set; }
        public Boolean isAvailable { get; set; }
    }
}