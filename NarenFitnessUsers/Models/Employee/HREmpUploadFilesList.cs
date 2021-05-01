using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Employee
{
    public class HREmpUploadFilesList
    {
        public string status { get; set; }
        public List<HREmpUploadFiles> value { get; set; }
    }
}