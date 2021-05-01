using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Booking
{
    public class BookedSlotsRequest
    {
        public string TrainerID { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}