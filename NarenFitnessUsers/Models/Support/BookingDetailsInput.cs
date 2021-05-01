using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Support
{
    public class BookingDetailsInput
    {
        public string Mode { get; set; }
        public string InvoiceId { get; set; }
        public string MobileNo { get; set; }
        public string BranchCode { get; set; }

    }
}