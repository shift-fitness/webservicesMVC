using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.SpaceManagement.SlotAllocation
{
    public class SlotAllocationMultiOutput
    {
        public string status { get; set; }

        public List<SlotAllocationDetails> value { get; set; }

    }
}