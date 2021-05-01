using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLineDurationDateWise
    {
        public string date { get; set; }
        public List<OffLineDurationWiseColums> slotPrice { get; set; }
    }
}