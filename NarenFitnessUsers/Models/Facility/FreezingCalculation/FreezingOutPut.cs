using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility.FreezingCalculation
{
    public class FreezingOutPut
    {
        public string status { get; set; }
        public List<FreezingValues> value { get; set; }
    }
}