using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Slots.Slot
{
    public class AllSlotsFinalOutPut
    {
        public string status { get; set; }

        public List<GetSessionSlots> value { get; set; }
    }
}