using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLine
{
    public class MiddleSectionATrainerDetails
    {
        public List<Package> packages { get; set; }
        public Dictionary<string, object> trainerDetails { get; set; }
      
    }
}