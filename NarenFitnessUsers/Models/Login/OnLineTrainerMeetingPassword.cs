using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Login
{
    public class OnLineTrainerMeetingPassword
    {
        public string Id { get; set; }
        public string ZoomId { get; set; }
        public string ZoomUserName { get; set; }
        public string ZoomPassword { get; set; }
        public string ZoomUrl { get; set; }
        public string GoogleId { get; set; }
        public string GoogleUserName { get; set; }
        public string GooglePassword { get; set; }
        public string GoogleUrl { get; set; }
        public string TrainerCode { get; set; }
        public string CreatedBy { get; set; }
    }
}