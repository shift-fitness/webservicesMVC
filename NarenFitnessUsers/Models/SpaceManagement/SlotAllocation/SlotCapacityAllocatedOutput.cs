using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.SpaceManagement.SlotAllocation
{
    public class SlotCapacityAllocatedOutput
    {
        public string status { get; set; }
        public List<SlotCapacityAllocated> value { get; set; }
    }
}