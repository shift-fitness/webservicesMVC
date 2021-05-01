using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Employee
{
    public class HREmpEducationDetails
    {
        public int ID { get; set; }
        public string Stream { get; set; }
        public string UniversityName { get; set; }
        public string PassPercentage { get; set; }
        public string Grade { get; set; }
        public string YearOfPassing { get; set; }
        public string CreatedBy { get; set; }

    }
}