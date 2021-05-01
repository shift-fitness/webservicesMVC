using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Employee.Performance
{
    public class PESNOutput
    {

        public int Id { get; set; }
        public int PesId { get; set; }
        public string PeNames { get; set; }
        public string Score { get; set; }
        public string EvaluationName { get; set; }
    }
}