using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.SpaceManagement.SlotAllocation
{
    public class SlotAllocationDetails
    {
        public string TrainerCode { get; set; }
        public string PackageCode { get; set; }
        public string SlotCode { get; set; }
        public string AllocatedCount { get; set; }
        public string FilledCount { get; set; }
        public string FilledPercent { get; set; }
        public string RemainingPercent { get; set; }
        public string BranchCode { get; set; }
        public string SlotPrice { get; set; }

    }
}