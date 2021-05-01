using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Employee.Performance
{
    public class PEInput
    {
        public int Id { get; set; }
        public int PESID { get; set; }
        public int PESNID { get; set; }
        public string PESubName { get; set; }
        public string EvaluationName { get; set; }
        public string PENames { get; set; }
        public string EmployeeName { get; set; }
        public string Score { get; set; }
        public string CreatedBy { get; set; }
        public string EmployeeCode { get; set; }
  

    }
}