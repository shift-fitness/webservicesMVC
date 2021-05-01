using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.RazorPay
{
    public class MarchantDetails
    {
        public string MerchantName { get; set; }
        public string MerchantKey { get; set; }
        public string MerchantSecretKey { get; set; }
        public string BranchCode { get; set; }

    }
}