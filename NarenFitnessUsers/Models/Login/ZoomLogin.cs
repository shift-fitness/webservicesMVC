using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Login
{
    public class ZoomLogin
    {
        public string meetingId { get; set; }
        public string meetingPassword { get; set; }
        public string meetingUserName { get; set; }
        public string meetingUrl { get; set; }
        public string id { get; set; }


        public string trainerId { get; set; }

        public string slotStartTime { get; set; }
        public string slotEndTime { get; set; }
    }
}