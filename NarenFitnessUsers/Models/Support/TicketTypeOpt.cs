using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Support
{
    public class TicketTypeOpt
    {
        public string status { get; set; }
        public List<TicketTypes> value { get; set; }
    }
}