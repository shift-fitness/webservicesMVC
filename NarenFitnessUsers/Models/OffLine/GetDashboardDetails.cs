using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLine
{
    public class GetDashboardDetails
    {
        public int isFreeTrail { get; set; }
        public List<Freetraildetails> freetraildetail { get; set; }
        public string textHeader { get; set; }
        public List<TextHeader> textHeaders { get; set; }
        public string imageName { get; set; }
        public List<ImageName> freeTrailImage { get; set; }
        public string bannerName { get; set; }
        public List<BannersName> bannerUrls { get; set; }
    }
}