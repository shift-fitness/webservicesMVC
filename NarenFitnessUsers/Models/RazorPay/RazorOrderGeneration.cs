using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.RazorPay
{
    public class RazorOrderGeneration
    {
        public List<Users> user { get; set; }
        public List<PackagePurchase> packetsPurchase { get; set; }
        public List<RazorPaymentcs> payments { get; set; }
        public List<EnquireStatus> enquireStatus { get; set; }
    }
}