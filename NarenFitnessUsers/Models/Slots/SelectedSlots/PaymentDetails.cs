using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Slots.SelectedSlots
{
    public class PaymentDetails
    {
        public float ActualPrice { get; set; }
        public float PromotionDiscount { get; set; }
        public float PaidAmount { get; set; }
    }
}