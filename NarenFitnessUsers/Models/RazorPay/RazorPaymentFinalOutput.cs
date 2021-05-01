using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.RazorPay
{
    public class RazorPaymentFinalOutput
    {
        public string status { get; set; }

        public List<RazorPaymentPost> value { get; set; }
    }
}