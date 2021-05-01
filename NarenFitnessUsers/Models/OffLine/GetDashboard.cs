using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.OffLine
{
    public class GetDashboard
    {
        public List<Info> info { get; set; }
        public List<TextHeaderAll> bottomSection { get; set; }
        public List<MiddleSectionA> middleSection { get; set; }
        public List<BannerAll> topSection { get; set; }
    }
}