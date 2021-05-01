using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Attendence
{
    public class Attendenceobjects
    {
        public double TotalNoDays { get; set; }
        public double NoOfDaysPresent { get; set; }
        public double PresentPercentage { get; set; }
        public List<DateWiseAttendence> Attendence { get; set; }
    }
}