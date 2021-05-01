using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
 using NarenFitnessUsers.Models.Facility.AllFacilities.Freezing;
using NarenFitnessUsers.Models.Facility.AllFacilities.Grace;
using NarenFitnessUsers.Models.Facility.AllFacilities.Upgradation;
using NarenFitnessUsers.Models.Facility.AllFacilities.Change;
using NarenFitnessUsers.Models.Facility.AllFacilities.LocationTransfer;
using NarenFitnessUsers.Models.Facility.AllFacilities.PersonTransfer;
using NarenFitnessUsers.Models.Facility.AllFacilities.Extend;
using NarenFitnessUsers.Models.Facility.AllFacilities.Hold;

namespace NarenFitnessUsers.Models.Facility.AllFacilities
{
    public class AllFacilityCombinedList
    {
        public List<FreezingDetails> freezingDetails { get; set; }
        public List<GraceDetails> graceDetails { get; set; }
        public List<UpgradationDetails> upgradationDetails { get; set; }
        public List<ChangeDetails> changeDetails { get; set; }
        public List<LocationTransferDetails> locationTransferDetails { get; set; }
        public List<PersonTransferDetails> personTransferDetails { get; set; }
        public List<ExtendDetails> extendDetails { get; set; }
        public List<HoldDetails> holdDetails { get; set; }


    }
}