using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class DashboardDisplayOutput
    {
        public string status { get; set; }

        public List<DashboardDisplayResponse> value { get; set; }
    }
}