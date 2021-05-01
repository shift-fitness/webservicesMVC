using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Support
{
    public class MembersOpt
    {
        public string status { get; set; }
        public List<MembersTicketDetails> value { get; set; }
    }
}