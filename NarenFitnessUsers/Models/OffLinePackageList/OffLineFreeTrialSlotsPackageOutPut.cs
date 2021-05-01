using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLineFreeTrialSlotsPackageOutPut
    {
        public string status { get; set; }
        public List<OffLineFreeTrialSlotsPackageDetails> value { get; set; }
    }
}