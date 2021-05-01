using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Login
{
    public class CustomerLoginEmailNonNestedModel
    {
        public Boolean isUserLoggedIn { get; set; }

        public Boolean hasDetailsConfirmed { get; set; }
        public string errorMessage { get; set; }

        //public int loginMode { get; set; }

        public string status { get; set; }

        public bool isExist { get; set; }
    }
}