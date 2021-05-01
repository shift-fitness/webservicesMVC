using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.SpaceManagement.SlotAllocation
{
    public class SlotCapacityAllocated
    {
        public int ID { get; set; }
        public string packageName { get; set; }
        public string slotName { get; set; }
        public string duration { get; set; }
        public float maxPrice { get; set; }
        public float minPrice { get; set; }
        public string allocatedCount { get; set; }
    }
}