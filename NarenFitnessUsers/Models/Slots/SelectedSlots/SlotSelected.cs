using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Slots.SelectedSlots
{
    public class SlotSelected
    {
        //public string CurrentSelectionSlots { get; set; }
        //public string PerviousSelectionSlots { get; set; }
        public string SelectedSlotId { get; set; }
        public string MobileNo { get; set; }
        public string TrainerId { get; set; }
        public string Date { get; set; }

        public Boolean ChangeTrainer { get; set; }

        //public string PerviousSelectionTrainerId { get; set; }

    }
}