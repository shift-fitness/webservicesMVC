using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Support
{
    public class TicketBookingOutput
    {
        public string status { get; set; }
        public List<TicketBooking> value { get; set; }
    }
}