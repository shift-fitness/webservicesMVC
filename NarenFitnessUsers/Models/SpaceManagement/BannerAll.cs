﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.SpaceManagement
{
    public class BannerAll
    {
        public string bannerName { get; set; }
        public List<BannersName> banners { get; set; }
    }
}