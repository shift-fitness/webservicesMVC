using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Trainer.OnlineTrainerDetails
{
    public class OnlineGetTrainerDetailsoutput
    {
        public string status { get; set; }
        public List<OnlineTrainerAttributes> value { get; set; }
    }
}