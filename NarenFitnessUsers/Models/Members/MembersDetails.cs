using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Members
{
    public class MembersDetails
    {

      
        public string MemberShipCode { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string MembershipExpireDate { get; set; }
        public string MembershipStartDate { get; set; }
        public string MobileNo { get; set; }
        public string Gender { get; set; }
        public string PhotoUrl { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }

    }
}