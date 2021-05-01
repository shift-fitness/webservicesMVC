using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Login
{
    public class ZoomResponse
    {
        public string status { get; set; }

        public List<zoombody> value { get; set; }
    }
}