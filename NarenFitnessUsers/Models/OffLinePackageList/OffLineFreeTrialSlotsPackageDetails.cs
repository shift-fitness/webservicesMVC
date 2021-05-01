using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLinePackageList
{
    public class OffLineFreeTrialSlotsPackageDetails
    {
        public List<OffLineEnquireInfo> info { get; set; }
        public List<OffLineFreeTrialSessions> sessions { get; set; }
      
    }
}