using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.PromoCode
{
    public class ValidatePromoCodeOutPut
    {
        public string status { get; set; }

        public List<ValidatePromoCodeDetails> value { get; set; }
    }
}