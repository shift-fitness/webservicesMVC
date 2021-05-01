using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Employee.Performance
{
    public class ScoreDetailsOutput
    {
        public string status { get; set; }
        public List<PEStandardNestRoot> value { get; set; }
    }
}