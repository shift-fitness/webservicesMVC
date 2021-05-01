using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.SpaceManagement
{
    public class ImageName2
    {
        public string imageid { get; set; }
        public string imageURL { get; set; }
        public string PackageName { get; set; }
        public List<PackageDetails2> Packages { get; set; }
    }
}