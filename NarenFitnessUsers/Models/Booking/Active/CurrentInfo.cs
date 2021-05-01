using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Booking.Active
{
    public class CurrentInfo
    {
        //public List<FacilityOptsBooking> CurrentFacilityInfo { get; set; }

        public Dictionary<string, Dictionary<string, object>> CurrentFacilityInfo { get; set; }
        public Dictionary<string, object> CurrentBooking { get; set; }

    }
}