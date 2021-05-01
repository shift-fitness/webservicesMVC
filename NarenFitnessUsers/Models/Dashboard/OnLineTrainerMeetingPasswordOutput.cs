using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class OnLineTrainerMeetingPasswordOutput
    {
        public string status { get; set; }

        public List<OnLineTrainerMeetingPasswordResponse> value { get; set; }
    }
}