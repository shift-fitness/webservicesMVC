using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class OnlineTrainersSlotMappingOutput
    {
        public string status { get; set; }

        public List<OnlineTrainersSlotMappingResponse> value { get; set; }
    }
}