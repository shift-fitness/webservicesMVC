using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLinePackagePriceList
    {
        public int enquiretypeNo { get; set; }
        public string mobileDeviceId { get; set; }
        public string mobileNo { get; set; }
        public List<OffLinePackagePrices> packagePricesList { get; set; }
        public List<OffLineTextHeaderAll> bottomSection { get; set; }
    }
}