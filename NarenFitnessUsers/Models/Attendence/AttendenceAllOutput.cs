using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Attendence
{
    public class AttendenceAllOutput
    {
        public string status { get; set; }

        public List<Attendenceobjects> value { get; set; }

    }
}