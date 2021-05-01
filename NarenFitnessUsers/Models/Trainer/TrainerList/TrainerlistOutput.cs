using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Trainer.TrainerList
{
    public class TrainerlistOutput
    {
        public string status { get; set; }

        public List<TrainerlistResponse> value { get; set; }
    }
}