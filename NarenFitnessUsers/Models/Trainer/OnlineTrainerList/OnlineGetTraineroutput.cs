using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Trainer.OnlineTrainerList
{
    public class OnlineGetTraineroutput
    {
        public string status { get; set; }
        public List<OnlineTrainerObjects> value { get; set; }
    }
}