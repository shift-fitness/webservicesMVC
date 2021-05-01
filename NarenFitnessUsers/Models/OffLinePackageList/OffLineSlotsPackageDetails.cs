using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLineSlotsPackageDetails
    {
        public List<OffLineEnquireInfo> info { get; set; }
        public List<OffLineSessions> sessions { get; set; }
        public List<OffLineSlotsPackage> slots { get; set; }
    }
}