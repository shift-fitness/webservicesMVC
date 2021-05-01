using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class DashboardDisplayResponse
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string comments { get; set; }
        public string screenId { get; set; }
        public string screenName { get; set; }
    }
}