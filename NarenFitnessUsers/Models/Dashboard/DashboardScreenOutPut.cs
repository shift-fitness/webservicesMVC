using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class DashboardScreenOutPut
    {
        public string status { get; set; }

        public List<DashboardScreenResponse> value { get; set; }
    }
}