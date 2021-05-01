using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Employee.Performance
{
    public class PEStandardOutput
    {
        public string status { get; set; }
        public List<PEStandard> value { get; set; }
    }
}