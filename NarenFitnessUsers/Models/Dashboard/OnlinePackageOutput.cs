using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class OnlinePackageOutput
    {
        public string status { get; set; }

        public List<OnlinePackageResponse> value { get; set; }
    }
}