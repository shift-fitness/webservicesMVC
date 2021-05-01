using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Attendence
{
    public class AttendencePercentage
    {
        public int NoOfDaysCompleted { get; set; }
        public int NoOfDaysPresent { get; set; }
        public float PresentPercentage { get; set; }
      
    }
}