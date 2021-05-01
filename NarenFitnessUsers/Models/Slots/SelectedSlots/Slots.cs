using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Slots.SelectedSlots
{
    public class Slots
    {
        public List<TrainerDetails> trainerDetails { get; set; }
        public List<PackageDetails> slotDetails { get; set; }
    }
}