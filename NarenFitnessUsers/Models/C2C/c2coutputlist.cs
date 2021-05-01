using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.C2C
{
    public class c2coutputlist
    {
        public string status { get; set; }

        public List<C2CData> value { get; set; }
    }
}