using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLineOnlySlotsPackageDetails
    {
        public List<OffLineEnquireInfo> info { get; set; }
        public List<OffLineDurationDateWise> dateWisePrice { get; set; }
        public List<OffLineSlotsPackage> slots { get; set; }
        public List<OffLineSessionPackage> session { get; set; }

    }
}