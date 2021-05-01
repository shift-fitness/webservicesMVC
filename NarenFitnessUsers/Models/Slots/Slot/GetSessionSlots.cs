using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Slots.Slot
{
    public class GetSessionSlots
    {
        public string sessionId { get; set; }
        public string sessionName { get; set; }
        public string sessionStartTime { get; set; }
        public string sessionEndTime { get; set; }
        public List<AllSlots> slots { get; set; }
    }
}