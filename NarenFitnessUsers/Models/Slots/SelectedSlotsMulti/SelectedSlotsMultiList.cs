using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Slots.SelectedSlotsMulti
{
    public class SelectedSlotsMultiList
    {
        public List<SlotBookedPostMultiple> slotsbooked { get; set; }
        public List<SelectedSlotPaymentDetails> paymentDetails { get; set; }
    }
}