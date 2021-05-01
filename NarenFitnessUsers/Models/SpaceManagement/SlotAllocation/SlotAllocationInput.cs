using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.SpaceManagement.SlotAllocation
{
    public class SlotAllocationInput
    {
        public string ID { get; set; }
        public string TrainerCode { get; set; }
        public string PackageCode { get; set; }
        public string SlotCode { get; set; }
        public string SessionCode { get; set; }
        public int AllocatedCount { get; set; }
        public int FreeTrialAllocatedCount { get; set; }
        public int FilledCount { get; set; }
        public float FilledPercent { get; set; }
        public float RemainingPercent { get; set; }
        public string BranchCode { get; set; }
        public string Duration { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public double SlotPrice { get; set; }
        public string CreatedBy { get; set; }
    }
}