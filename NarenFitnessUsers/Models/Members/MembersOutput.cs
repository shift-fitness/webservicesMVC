using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Members
{
    public class MembersOutput
    {
        public string status { get; set; }

        public List<MembersDetails> value { get; set; }
    }
}