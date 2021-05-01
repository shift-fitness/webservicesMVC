using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Slots.SelectedSlots
{
    public class SlotBookedPost
    {
        public string MemberShipCode { get; set; }
        public string MobileNo { get; set; }
        public string EnquireTypeNo { get; set; }
        public string PackageId { get; set; }
        public string SessionId { get; set; }
        public string SlotDate { get; set; }
        public string SlotId { get; set; }
        public string TrainerId { get; set; }
        public string ActualPrice { get; set; }
        public string MobileDeviceId { get; set; }
        public string UserId { get; set; }
        public string LoginPassword { get; set; }
        public string MeetingId { get; set; }
        public string MeetingPassword { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string invoiceNo { get; set; }
        public string BranchCode { get; set; }
    }
}