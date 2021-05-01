using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Booking.Active
{
    public class GetBookingrootActiveOnline
    {
        public Dictionary<string, object> currentBookings { get; set; }
        public Dictionary<string, object> futureBookings { get; set; }
    }
}