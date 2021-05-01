using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLine
{
    public class ImageName
    {
        public string imageId { get; set; }
        public string imageUrl { get; set; }
        public string packageName { get; set; }
        public List<PackageDetails1> packages { get; set; }
    }
}