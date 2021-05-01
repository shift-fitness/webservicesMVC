using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLineOnlySlotsPackageOutPut
    {
        public string status { get; set; }
        public List<OffLineOnlySlotsPackageDetails> value { get; set; }
    }
}