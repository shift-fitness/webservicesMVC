using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.RazorPay
{
    public class RazorPaymentcs
    {
        public string Mode { get; set; }
        public string MemberShipCode { get; set; }
        public string Invoice { get; set; }
        public string GatewayProviderName { get; set; }
        public string OrderId { get; set; }
        public string BranchCode { get; set; }
        public string MobileNo { get; set; }
        public string ModeOfPayment { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string receipt { get; set; }
        public string payment_capture { get; set; }
        public string notes { get; set; }
        public int TransactionId { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionDate { get; set; }
        public string Signature { get; set; }
        public string CreatedBy { get; set; }
    }
}