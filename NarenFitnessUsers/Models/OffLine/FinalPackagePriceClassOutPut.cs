using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLine
{
    public class FinalPackagePriceClassOutPut
    {
        public string status { get; set; }

        public List<PackagePricesClass> value { get; set; }
    }
}