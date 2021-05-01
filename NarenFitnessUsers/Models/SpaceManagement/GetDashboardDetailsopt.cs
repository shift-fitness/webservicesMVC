using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.SpaceManagement
{
    public class GetDashboardDetailsopt
    {
        public int IsFreeTrail { get; set; }
        public List<Freetraildetails> Freetraildetail { get; set; }

        public string TextHeader { get; set; }
        public List<TextHeader> TextHeaders { get; set; }
        public string ImageName { get; set; }
        public List<ImageName> PackageImage { get; set; }
        public string BannerName { get; set; }
        public List<BannersName> BannerUrls { get; set; }
    }
}