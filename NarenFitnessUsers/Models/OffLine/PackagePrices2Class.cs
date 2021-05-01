using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NarenFitnessUsers.Models.Packages;

namespace NarenFitnessUsers.Models.OffLine
{
    public class PackagePrices2Class
    {
        public int isPackages { get; set; }
        public int enquireTypeNo { get; set; }
        public List<PackageDetails1> packageDetails { get; set; }
        public string textHeader { get; set; }
        public List<TextHeader> text { get; set; }
        public string imageName { get; set; }
        public List<ImageName> image { get; set; }
        public string bannerName { get; set; }
        public List<BannersName> banner { get; set; }
        public List<PackagePrices> packages { get; set; }
    }
}