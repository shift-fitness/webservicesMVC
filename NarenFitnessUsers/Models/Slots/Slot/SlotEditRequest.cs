using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Slots.Slot
{
    public class SlotEditRequest
    {
        public int Id { get; set; }
        public string SlotId { get; set; }
        public int MembershipCode { get; set; }
        public string InvoiceId { get; set; }
        public string CreatedBy { get; set; }
       
    }
}