using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.RazorPay
{
    public class OffLineRazorPaymentFinalOutputMessage
    {
        public string status { get; set; }

        public List<ErrorMessage> value { get; set; }
      
    }
}