using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.PromoCode
{
    public class PromoCodeOutput
    {
        public string status { get; set; }

        public List<PromoCodeDetails> value { get; set; }
    }
}