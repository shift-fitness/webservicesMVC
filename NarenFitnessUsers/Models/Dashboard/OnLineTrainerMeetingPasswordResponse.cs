using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class OnLineTrainerMeetingPasswordResponse
    {
        public int id { get; set; }
        public string zoomId { get; set; }
        public string zoomUserName { get; set; }
        
        public string zoomPassword { get; set; }
        public string zoomUrl { get; set; }
        public string googleId { get; set; }
        public string googleUserName { get; set; }
        public string googlePassword { get; set; }
        public string googleUrl { get; set; }
        public string trainerCode { get; set; }
        public string trainerName { get; set; }
    }
}