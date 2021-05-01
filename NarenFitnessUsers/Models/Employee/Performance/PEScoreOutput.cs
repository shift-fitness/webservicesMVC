using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Employee.Performance
{
    public class PEScoreOutput
    {
        public string status { get; set; }
        public List<PEScore> value { get; set; }
    }
}