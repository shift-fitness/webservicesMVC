using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Booking
{
    public class SlotBookingByTrainerOP
    {
        public string userName { get; set; }
        public string slotID { get; set; }
        public string slotStartTime { get; set; }
        public string slotEndTime { get; set; }
        public string sessionDate { get; set; }
        public string packageID { get; set; }
        public string packageName { get; set; }
        public string meetingId { get; set; }
        public string meetingPassword { get; set; }
        public string meetingUrl { get; set; }
        public string mobileNumber { get; set; }
    }
}