using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class OnlineTrainersSlotMappingResponse
    {
      

        public int id { get; set; }

        public string sessionCode { get; set; }
        public string sessionName { get; set; }
        public string packageCode { get; set; }
        public string packageName { get; set; }
        public string slotCode { get; set; }
        public string slotName { get; set; }
        public Boolean isActive { get; set; }
        public Boolean freeTrialIsActive { get; set; }
        public string description { get; set; }
        
    }
}