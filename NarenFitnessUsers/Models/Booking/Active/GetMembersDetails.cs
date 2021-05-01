using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Booking.Active
{
    public class GetMembersDetails
    {
      
        public int noOfDays { get; set; }
        public int usedCount { get; set; }
        public int availableCount { get; set; }
        public int mfdid { get; set; }
    }
}