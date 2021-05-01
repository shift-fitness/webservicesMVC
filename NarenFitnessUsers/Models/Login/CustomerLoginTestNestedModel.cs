using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Login
{
    public class CustomerLoginTestNestedModel
    {
        public Boolean isUserLoggedIn { get; set; }
        public Boolean hasDetailsConfirmed { get; set; }
        public string errorMessgae { get; set; }
        public string status { get; set; }
        public int OTP { get; set; }
        public bool isExist { get; set; }
        public Dictionary<string, string> personalDetails { get; set; }
    }
}