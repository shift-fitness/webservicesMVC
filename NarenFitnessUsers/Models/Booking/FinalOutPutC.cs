using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Booking
{
    public class FinalOutPutC
    {
        public string status { get; set; }

        public List<PackageDetailsByInvoice> value { get; set; }
    }
}