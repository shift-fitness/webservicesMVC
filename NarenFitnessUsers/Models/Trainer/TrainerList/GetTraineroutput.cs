using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Trainer.TrainerList
{
    public class GetTraineroutput
    {
        public string status { get; set; }
        public List<TrainersFields> value { get; set; }

    }
}