using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.PackagesList
{
    public class PackageDetailsOutput
    {
        public string status { get; set; }

        public List<PackageDetails> value { get; set; }


    }
}