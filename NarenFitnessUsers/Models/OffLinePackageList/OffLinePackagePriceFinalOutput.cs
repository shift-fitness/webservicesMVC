using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLinePackagePriceFinalOutput
    {
        public string status { get; set; }

        public List<OffLinePackagePriceList> value { get; set; }
    }
}