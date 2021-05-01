using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.RazorPay
{
    public class PaymentDetailsGet
    {
        public string status { get; set; }
        public string orderId { get; set; }
    }
}