using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Support
{
    public class BookingDetailsOpt
    {
        public string status { get; set; }
        public List<BookingDetails> value { get; set; }
    }
}