using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Support
{
    public class BookingDetails
    {
        public string PackageCode { get; set; }
        public string InvoiceId { get; set; }
        public DateTime StartDate { get; set; }
        public string PackageName { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
    }
}