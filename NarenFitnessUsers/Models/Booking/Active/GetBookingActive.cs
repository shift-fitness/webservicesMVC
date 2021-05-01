using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Booking.Active
{
    public class GetBookingActive
    {
        public List<CurrentInfo> currentInfo { get; set; }
        public List<FutureInfo> futureInfo { get; set; }
    
    }
}