﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.PackagesList
{
    public class PackageDescriptionList
    {
        public string packageCode { get; set; }
        public string packageName { get; set; }
        public List<PackageDescriptionDetails> packageDescriptionDetails { get; set; }
    }
}