using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Support
{
    public class MembersTicketDetails
    {
        public string Id { get; set; }
        public string RequestName { get; set; }
        public DateTime RaisedOn { get; set; }
        public DateTime CloserDated { get; set; }
        public DateTime NextUpdate { get; set; }
        
        public string StatusId { get; set; }
        public string StatusName { get; set; }
        public string Comments { get; set; }
    }
}