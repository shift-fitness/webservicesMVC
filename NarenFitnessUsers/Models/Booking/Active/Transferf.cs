using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Booking.Active
{
    public class Transferf
    {
      
        public int MFIDID { get; set; }
        public double LeftOutDays { get; set; }
        public double RemainingAmount { get; set; }
        public int IsTransferUsed { get; set; }

    }
}