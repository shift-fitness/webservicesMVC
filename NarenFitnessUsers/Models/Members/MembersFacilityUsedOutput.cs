using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Members
{
    public class MembersFacilityUsedOutput
    {

        public string status { get; set; }

        public List<MembersFacilityUsedDetails> value { get; set; }
    }
}