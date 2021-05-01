using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.pdf
{
    public class pdfOutput
    {
        public string status { get; set; }
        public List<pdflist> value { get; set; }
    }
}