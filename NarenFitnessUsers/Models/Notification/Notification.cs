using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Notification
{
    public class Notification
    {
        public string RegisteredToken { get; set; }
        public string MobileDeviceId { get; set; }
        public string Status { get; set; }
    }
}