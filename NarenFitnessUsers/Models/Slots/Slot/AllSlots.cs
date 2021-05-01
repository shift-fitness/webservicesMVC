using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Slots.Slot
{
    public class AllSlots
    {
        public string slotID { get; set; }
        public string slotName { get; set; }
        public string slotStartTime { get; set; }
        public string slotEndTime { get; set; }
        public bool isAvailable { get; set; }
    }
}