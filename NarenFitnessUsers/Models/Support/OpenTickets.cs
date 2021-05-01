using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Support
{
    public class OpenTickets
    {

        public string memberShipCode { get; set; }
        public int TicketId { get; set; }
        public int RequestTypeId { get; set; }
        public string RequestType { get; set; }
        public string Comments { get; set; }
        public string PostPoneDate { get; set; }
        public string InvoiceId { get; set; }
        public int StatusId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}