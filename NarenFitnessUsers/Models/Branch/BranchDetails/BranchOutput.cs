using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Branch.BranchDetails
{
    public class BranchOutput
    {
        public string status { get; set; }
        public List<BranchFields> value { get; set; }
    }
}