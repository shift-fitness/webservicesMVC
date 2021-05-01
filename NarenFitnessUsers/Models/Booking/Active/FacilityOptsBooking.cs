using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Booking.Active
{
    public class FacilityOptsBooking
    {
        public Dictionary<string, object> FreezingInfo { get; set; }
        public Dictionary<string, object> ChangeInfo { get; set; }
        public Dictionary<string, object> UpgradeInfo { get; set; }
        public Dictionary<string, object> TransferInfo { get; set; }
        public Dictionary<string, object> ConvertInfo { get; set; }



    }
}