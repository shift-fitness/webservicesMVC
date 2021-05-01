using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Employee
{
    public class HREmpUploadFiles
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string UploadedImage { get; set; }
        public string CreatedBy { get; set; }
    }
}