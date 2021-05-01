using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Slots.Slot
{
    public class SlotsRequest
    {
        public int EnquiryType { get; set; }
        public string MobileNo { get; set; }
        public string MemberShipCode { get; set; }
        public string SessionDate { get; set; }
        public string TrainerId { get; set; }
        public DateTime Date { get; set; }
    }
}