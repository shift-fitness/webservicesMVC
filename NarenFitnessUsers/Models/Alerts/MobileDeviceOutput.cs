using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Alerts
{
    public class MobileDeviceOutput
    {
        public string status { get; set; }

        public List<MobileDeviceDetails> value { get; set; }
    }
}