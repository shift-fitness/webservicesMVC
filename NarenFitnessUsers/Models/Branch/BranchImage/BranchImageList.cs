using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Branch.BranchImage
{
    public class BranchImageList
    {
        public string status { get; set; }
        public List<BranchImageDetails2> value { get; set; }
    }
}