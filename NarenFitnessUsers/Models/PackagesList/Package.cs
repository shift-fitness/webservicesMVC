using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.PackagesList
{
    public class Package
    {
        public string packageName { get; set; }
        public List<PackageDetails1> packages { get; set; }
    }
}