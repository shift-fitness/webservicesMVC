using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Members
{
    public class MembersInput
    {
        public string SearchItem { get; set; }
        public string MemberShipCode { get; set; }
        public string UserName { get; set; }
        public string MobileNo { get; set; }
        public string Invoice { get; set; }

    }
}