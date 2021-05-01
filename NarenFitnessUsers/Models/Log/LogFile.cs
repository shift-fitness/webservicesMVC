using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Log
{
    public class LogFile
    {

        public string CallUUid { get; set; }
        public string CallData { get; set; }
        public DateTime CallTime { get; set; }
        public string AgentList { get; set; }
        public string AgentNumber { get; set; }
        public int CallTransferStatus { get; set; }
        public string CallRecordingUrl { get; set; }
        public int ConversationDuration { get; set; }
        public int TotalCallDuration { get; set; }
        public string CreatedBy { get; set; }
    }
}