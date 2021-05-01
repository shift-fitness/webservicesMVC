using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.PackagesList
{
    public class PackagePriceFinalOutput
    {
        public string status { get; set; }

        public List<PackagePriceList> value { get; set; }
    }
}