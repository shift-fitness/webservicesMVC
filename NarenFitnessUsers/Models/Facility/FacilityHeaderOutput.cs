﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Facility
{
    public class FacilityHeaderOutput
    {
        public string status { get; set; }
        public List<FacilityHeader> value { get; set; }
    }
}