﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NarenFitnessUsers.Models.Trainer.TrainerDetails;

namespace NarenFitnessUsers.Models.Trainer.TrainerDetails
{
    public class GetTrainerDetailsoutput
    {
        public string status { get; set; }
        public List<TrainerAttributes> value { get; set; }
    }
}