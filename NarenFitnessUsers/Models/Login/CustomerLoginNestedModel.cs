using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Login
{
    public class CustomerLoginNestedModel
    {
        public Boolean isUserLoggedIn { get; set; }
        public Boolean hasDetailsConfirmed { get; set; }
        public string errorMessage { get; set; }
        public string status { get; set; }
        public int OTP { get; set; }
        public bool isExist { get; set; }
        //public string MembershipCode { get; set; }
        public Dictionary<string, string> personalDetails { get; set; }
    }
}