﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.SpaceManagement
{
    public class Slot
    {
        public string slotName { get; set; }
        public List<Slots> slots { get; set; }
    }
}