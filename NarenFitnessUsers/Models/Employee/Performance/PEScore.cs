using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Employee.Performance
{
    public class PEScore
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public int PESNID { get; set; }
        public int Score { get; set; }
    }
}