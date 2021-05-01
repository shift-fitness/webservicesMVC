using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLineCMSSlotsAvailability
    {
        public string StartDate { get; set; }
        public string BranchCode { get; set; }
        public string PackageCode { get; set; }
        public string SlotCode { get; set; }
        public string TrainerCode { get; set; }
        public string PackageDescCode { get; set; }
        public string PackageDescName { get; set; }
        public string PackageCost { get; set; }
        public string Description { get; set; }
        public string MobileNo { get; set; }
        public string MembershipCode { get; set; }
        public string Register { get; set; }

        public int StartNo { get; set; }
        public int EndNo { get; set; }


    }
}