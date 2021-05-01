using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Trainer.OnlineTrainerDetails
{
    public class OnlineTrainerAttributes
    {
        public string trainerCode { get; set; }
        public string trainerName { get; set; }
        public string packageCode { get; set; }
        public string packageName { get; set; }
        public List<Onlinecertificates> certificates { get; set; }
        public List<OnlineImages> images { get; set; }
        public List<OnlineSpecialist> specialist { get; set; }
        public List<OnlineVideos> videos { get; set; }
    }
}