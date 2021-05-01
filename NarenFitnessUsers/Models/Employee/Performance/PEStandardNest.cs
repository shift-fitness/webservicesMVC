using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Employee.Performance
{
    public class PEStandardNest
    {
        //PESubName as Ename,PEN.Score as Totalscore,PES.Score as achivedScore
        public string Ename { get; set; }
        public double Totalscore { get; set; }
        public double AchivedScore { get; set; }

    }
}