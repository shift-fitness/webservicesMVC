using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Booking.Active
{
    public class GetFutureBookingsActive
    {
        public string packageName { get; set; }
        public string invoiceDate { get; set; }
        public string orderID { get; set; }
        public string paidAmount { get; set; }
        public string invoiceId { get; set; }
        public string startDate { get; set; }
        public string EndDate { get; set; }
    }
}