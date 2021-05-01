using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Booking
{
    public class GetBookedPackagesByInvoice
    {
        public string packageID { get; set; }
        public double actualPrice { get; set; }
        public string packageStartDate { get; set; }
        public string packageEndDate { get; set; }
        public string packageName { get; set; }
        public double paidAmount { get; set; }
        public double discountAmount { get; set; }
        public string trainerId { get; set; }
        public string trainerName { get; set; }
        public double dueAmount { get; set; }
        public string invoiceID { get; set; }
        public string MOP { get; set; }
        public string invoiceDate { get; set; }
        public string PromoCode { get; set; }
        public double PayableAmount { get; set; }
        public double RemainingAmount { get; set; }
        public double AmountDue { get; set; }
        public double Wallet { get; set; }


    }
}