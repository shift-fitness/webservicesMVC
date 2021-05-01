using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class OnlineTrainersSlotMapping
    {
        public string Id { get; set; }
        public string branchCode { get; set; }
        public string trainerCode { get; set; }
        public string trainerName { get; set; }
        public string sessionCode { get; set; }
        public string sessionName { get; set; }
        public string slotCode { get; set; }
        public string slotName { get; set; }
        public string packageCode { get; set; }
        public string packageName { get; set; }
        public string description { get; set; }

        public string createdBy { get; set; }
    }
}