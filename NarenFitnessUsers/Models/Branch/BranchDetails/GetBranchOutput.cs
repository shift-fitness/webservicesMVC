using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Branch.BranchDetails
{
    public class GetBranchOutput
    {
        public string status { get; set; }
        public List<GetBranchFields> value { get; set; }
    }
}