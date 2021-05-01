using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Employee
{
    public class HREmpCertificatesList
    {
        public string status { get; set; }
        public List<HREmpCertificates> value { get; set; }
    }
}