using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Trainer.OnlineTrainerList
{
    public class OffLineTrainerObjects
    {
        public List<OffLinePerviousTrainerDetailsObject> previousTrainers { get; set; }
        public List<OffLineTrainerListObject> trainersList { get; set; }


    }
}