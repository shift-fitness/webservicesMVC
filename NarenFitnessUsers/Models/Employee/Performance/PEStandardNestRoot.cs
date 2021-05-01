using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Employee.Performance
{
    public class PEStandardNestRoot
    {
        public int Id { get; set; }
        public string EvaluationName { get; set; }
        public double Score { get; set; }
        public double AchieveScore { get; set; }
        public List<PEStandardNest> ScoreDetails { get; set; }
    }
}