using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.RazorPay
{
    public class PaymentDetailsPost
    {
        public string status { get; set; }
        public int transactionId { get; set; }
    }
}