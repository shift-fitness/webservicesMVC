using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Trainer.OnlineTrainerList
{
    public class OffLineGetTraineroutput
    {
        public string status { get; set; }
        public List<OffLineTrainerObjects> value { get; set; }
    }
}