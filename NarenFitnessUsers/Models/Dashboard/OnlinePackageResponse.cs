using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class OnlinePackageResponse
    {
        public string id { get; set; }
        public string packageId { get; set; }
        public string packageName { get; set; }
        public string branchCode { get; set; }
        public string enquireTypeNo { get; set; }
        public string packageCost { get; set; }
        public string discountPercentage { get; set; }
        public string noOfSessions { get; set; }
        public string nooOfDateValidity { get; set; }
        public string description { get; set; }
       
    }
}