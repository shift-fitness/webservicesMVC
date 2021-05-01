using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.RazorPay
{
    public class OffLineRazorPaymentFinalOutput
    {
        public string status { get; set; }

        public List<OffLineRazorPaymentPost> value { get; set; }
    }
}