using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Trainer.OnlineTrainerList
{
    public class OnlineTrainerObjects
    {
        public List<PerviousTrainerDetailsObject> previousTrainers { get; set; }
        public List<TrainerListObject> trainersList { get; set; }


    }
}