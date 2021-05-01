using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.SpaceManagement
{
    public class Slots
    {
        public int packageID { get; set; }
        public string packageName { get; set; }
        public string sessionID { get; set; }
        public string sessionDate { get; set; }
        public int slotID { get; set; }
        public string slotName { get; set; }
        public string slotStartDate { get; set; }
        public string slotEndDate { get; set; }
    }
}