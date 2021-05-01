using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Booking.Active
{
    public class ActiveBooking
    {
        public string status { get; set; }
        public List<GetBookingActive> value { get; set; }

    }
}