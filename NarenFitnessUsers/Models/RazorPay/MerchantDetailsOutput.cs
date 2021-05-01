using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.RazorPay
{
    public class MerchantDetailsOutput
    {
        public string status { get; set; }
        public List<MerchantDetails> value { get; set; }
    }
}