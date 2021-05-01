using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Support
{
    public class TicketTransaction
    {
        public int ID { get; set; }
        public string TicketId { get; set; }
        public int RequestTypeId { get; set; }
        public string BranchCode { get; set; }
        public string MemberShipCode { get; set; }
        public string MemberName { get; set; }
        public string Comments { get; set; }
        public string StatusId { get; set; }
        public string StatusName { get; set; }
        public string PostPoneDate { get; set; }
    }
}