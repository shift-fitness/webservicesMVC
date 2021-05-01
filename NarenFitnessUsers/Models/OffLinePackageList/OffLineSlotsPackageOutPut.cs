using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLineSlotsPackageOutPut
    {
        public string status { get; set; }
        public List<OffLineSlotsPackageDetails> value { get; set; }

    }
}