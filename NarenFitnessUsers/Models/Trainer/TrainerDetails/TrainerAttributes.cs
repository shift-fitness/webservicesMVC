using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Trainer.TrainerDetails
{
    public class TrainerAttributes
    {
        public string trainerCode { get; set; }
        public string trainerName { get; set; }
        public List<certificates> certificates { get; set; }
        public List<Images> images { get; set; }
        public List<Specialist> specialist { get; set; }
        public List<Videos> videos { get; set; }
        public List<TrainerPackages> packages { get; set; }

    }
}