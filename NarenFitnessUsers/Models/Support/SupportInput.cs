using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Support
{
    public class SupportInput
    {
        public string ID { get; set; }
        public string RequestType { get; set; }
        public string TicketId { get; set; }
        public int RequestTypeId { get; set; }
        public string BranchCode { get; set; }
        public string MemberShipCode { get; set; }
        public string InvoiceId { get; set; }
        public string Comments { get; set; }
        public string StatusId { get; set; }
        public string StatusName { get; set; }
        public string PostPoneDate { get; set; }
        public string CreatedBy { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

    }
}