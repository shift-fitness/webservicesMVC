using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.RazorPay
{
    public class PackagePurchase
    {
        public string BranchCode { get; set; }
        public string MembershipCode { get; set; }
        public int EnquireTypeNo { get; set; }
        public int PackageID { get; set; }
        public string MobileNo { get; set; }
        public string LastName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public float ActualPrice { get; set; }
        public int NumberOfSession { get; set; }
        public int NumberOfDaysValidity { get; set; }
        public string MobileDeviceID { get; set; }
        public string Invoice { get; set; }
        public string CreatedBy { get; set; }
    }
}