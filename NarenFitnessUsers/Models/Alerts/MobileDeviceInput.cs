using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Alerts
{
    public class MobileDeviceInput
    {
        public string registeredToken { get; set; }
        public string mobileNo { get; set; }
        public string mobileDeviceId { get; set; }
        public string status { get; set; }

    }
}