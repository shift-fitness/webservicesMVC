using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Login
{
    public class CustomerLoginEmailNestedModel
    {
        public string status { get; set; }
        public string errorMessage { get; set; }

        public Boolean isUserLoggedIn { get; set; }
        public Boolean hasDetailsConfirmed { get; set; }

        public bool isExist { get; set; }

        public int OTP { get; set; }

        // public List<PersonalDetails> PersonalDetails { get; set; }

        public Dictionary<string, string> personalDetails { get; set; }
    }
}