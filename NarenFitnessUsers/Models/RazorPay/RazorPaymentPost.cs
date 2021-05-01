using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.RazorPay
{
    public class RazorPaymentPost
    {
        public string invoice { get; set; }
        public string memberShipCode { get; set; }
        public string orderId { get; set; }
    }
}