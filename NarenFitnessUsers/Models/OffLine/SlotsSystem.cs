using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLine
{
    public class SlotsSystem
    {
        public string sessionCode { get; set; }

        public string sessionName { get; set; }

        public List<Slots> slotsAvailable { get; set; }
    }
}