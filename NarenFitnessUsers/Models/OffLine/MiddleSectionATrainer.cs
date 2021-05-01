using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLine
{
    public class MiddleSectionATrainer
    {
        public List<Package> packages { get; set; }
        public List<Slot> slots { get; set; }
        public List<Image> images { get; set; }

        public Dictionary<string, object> trainerDetails { get; set; }
    }
}