using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLine
{
    public class Slots
    {
        public int EnquiryId { get; set; }
        public int ID { get; set; }
        public string packageID { get; set; }
        public string packageName { get; set; }
        public string invoiceID { get; set; }
        public string sessionID { get; set; }
        public string sessionDate { get; set; }
        public string slotID { get; set; }
        public string slotName { get; set; }
        public string slotStartDate { get; set; }
        public string slotEndDate { get; set; }
    }
}