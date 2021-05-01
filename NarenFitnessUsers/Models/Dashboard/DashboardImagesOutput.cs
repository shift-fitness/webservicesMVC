using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class DashboardImagesOutput
    {
        public string status { get; set; }

        public List<DashboardImagesResponse> value { get; set; }
    }
}