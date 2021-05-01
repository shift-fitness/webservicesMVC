using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.RazorPay
{
    public class MarchantDetailsOutputMulti
    {
        public string status { get; set; }
        public List<MarchantDetails> value { get; set; }
    }
}