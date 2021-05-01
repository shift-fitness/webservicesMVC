using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.SpaceManagement
{
    public class MiddleSectionATrainerDetails
    {
        public List<Package> packages { get; set; }
        public Dictionary<string, object> trainerDetails { get; set; }
        public List<Slot> slots { get; set; }
        public List<Image> images { get; set; }

    }
}