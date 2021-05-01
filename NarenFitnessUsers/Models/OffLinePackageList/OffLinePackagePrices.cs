using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLinePackagePrices
    {
        public string branchCode { get; set; }
        public string packageId { get; set; }
        public string packageName { get; set; }
        public string packageCost { get; set; }
        public string discountPercentage { get; set; }
        public string numberOfSession { get; set; }
        public string numberOfDaysValidity { get; set; }
        public string mobileDeviceId { get; set; }
        public string mobileNo { get; set; }
    }
}