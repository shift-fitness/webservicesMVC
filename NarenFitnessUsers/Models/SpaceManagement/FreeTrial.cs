using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.SpaceManagement
{
    public class FreeTrial
    {
        public string CreatedBy { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string MobileDeviceId { get; set; }
        public string SlotCode { get; set; }
        public string TrainerId { get; set; }
        public string PlanId { get; set; }
        public string SessionDate { get; set; }
        public string UMID { get; set; }
    }
}