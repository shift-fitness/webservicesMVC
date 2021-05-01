using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLineOnlySlots
    {
        public int sNo { get; set; }
        public string slotCode { get; set; }
        public string slotStartTime { get; set; }
        public string slotEndTime { get; set; }
        public Boolean isSlotAvailable { get; set; }

    }
}