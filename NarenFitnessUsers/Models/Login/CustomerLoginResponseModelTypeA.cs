using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Login
{
    public class CustomerLoginResponseModelTypeA
    {
        public int OTP { get; set; }
        public int loginMode { get; set; }

        public string status { get; set; }
        public string errorMessage { get; set; }
        public bool isExist { get; set; }
    }
}