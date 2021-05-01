using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility.FreezingCalculation
{
    public class FreezingValues
    {
        public int TotalFreezingDays { get; set; }
        public int FreezedDays { get; set; }
        public int LeftOutFreezingDays { get; set; }
        public string Note { get; set; }

    }
}