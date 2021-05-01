using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.FreezingFacility
{
    public class FreezingListOutput
    {
        public string status { get; set; }
        public List<FreezingDetails> FreezingDetails { get; set; }
    }
}