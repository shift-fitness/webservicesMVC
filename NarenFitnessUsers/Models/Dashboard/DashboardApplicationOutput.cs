using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class DashboardApplicationOutput
    {
        public string status { get; set; }

        public List<DashboardApplicationResponse> value { get; set; }
    }
}