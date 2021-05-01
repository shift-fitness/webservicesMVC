using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class OnlineSlotTimingsOutput
    {
        public string status { get; set; }

        public List<OnlineSlotTimingsResponse> value { get; set; }
    }
}