using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Branch.BranchDetails
{
    public class GetBranchFields
    {
        public string aboutBranch { get; set; }
        public string branchTiming { get; set; }
        public string branchAddress { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public List<GetBranchImages> branchImages { get; set; }
        public List<GetBranchVideos> branchVideos { get; set; }

    }
}