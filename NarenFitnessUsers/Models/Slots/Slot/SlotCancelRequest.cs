using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Slots.Slot
{
    public class SlotCancelRequest
    {
        public string ID { get; set; }
        public string EnquiryId { get; set; }
        public string SlotID { get; set; }
        public string MobileNo { get; set; }
        public string TrainerID { get; set; }
    }
}