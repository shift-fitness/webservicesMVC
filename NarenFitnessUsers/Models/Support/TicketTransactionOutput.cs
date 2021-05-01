using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Support
{
    public class TicketTransactionOutput
    {
        public string status { get; set; }
        public List<TicketTransaction> value { get; set; }
    }
}