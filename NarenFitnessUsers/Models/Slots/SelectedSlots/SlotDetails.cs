using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Slots.SelectedSlots
{
    public class SlotDetails
    {
        public string slotId { get; set; }
        public string slotName { get; set; }
        public string slotStartTime { get; set; }
        public string slotEndTime { get; set; }
        public bool isAvailable { get; set; }
    }
}