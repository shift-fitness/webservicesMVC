using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class DashboardTextOutput
    {
        public string status { get; set; }

        public List<DashboardTextResponse> value { get; set; }
    }
}