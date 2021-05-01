using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLineFreeTrialOnlySlotOutPut
    {
        public string status { get; set; }
        public List<OffLineFreeTrialOnlySlotDetails> value { get; set; }
    }
}