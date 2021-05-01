using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Booking.Active
{
    public class FutureInfo
    {
        public Dictionary<string, Dictionary<string, object>> FutureFacilityInfo { get; set; }
        public Dictionary<string, object> FutureBooking { get; set; }
    }
}