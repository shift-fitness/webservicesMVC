using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Booking.Active
{
    public class GetBookingrootActive
    {
        public Dictionary<string, Dictionary<string, object>> currentInfo { get; set; }
        public Dictionary<string, object> currentBookings { get; set; }
        public Dictionary<string, Dictionary<string, object>> futureInfo { get; set; }
        public Dictionary<string, object> futureBookings { get; set; }

    }
}