using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.SpaceManagement
{
    public class GetDashboardTrainerDetails
    {
        public List<Info> info { get; set; }
        public List<TextHeaderAll> bottomSection { get; set; }
        public List<MiddleSectionATrainerDetails> middleSection { get; set; }
        public List<BannerAll> topSection { get; set; }

    }
}