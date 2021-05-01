using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.PackagesList
{
    public class PackagePriceList
    {
        public int enquiretypeNo { get; set; }
        public string mobileDeviceId { get; set; }
        public string mobileNo { get; set; }
        public List<PackagePrices> packagePricesList { get; set; }
        public List<TextHeaderAll> bottomSection { get; set; }
    }
}