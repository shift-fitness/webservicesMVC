using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Attendence
{
    public class AttendenceInput
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string SourceId { get; set; }
        public string BranchCode { get; set; }
        public string MachineId { get; set; }
        public string MembershipCode { get; set; }
        public string UserName { get; set; }
        public string DateOfTransaction { get; set; }
        public string CreatedOn { get; set; }
    }
}