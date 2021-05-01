using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.PackagesList
{
    public class Dashboard
    {
        public int AppId { get; set; }
        public string MobileDeviceID { get; set; }
        public string MobileNo { get; set; }
        public int DisplayId { get; set; }
        public int IsFreeTrial { get; set; }
        public int TextId { get; set; }
        public string Image { get; set; }
        public string ImageURL { get; set; }
        public string CreatedBy { get; set; }
    }
}