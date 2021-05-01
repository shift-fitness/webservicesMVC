using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.DynamicUrlCode
{
    public class PlanOutPutUrl
    {
        public string status { get; set; }

        public List<UrlInfo> value { get; set; }
    }
}