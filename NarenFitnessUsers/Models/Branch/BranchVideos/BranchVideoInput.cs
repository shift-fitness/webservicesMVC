﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Branch.BranchVideos
{
    public class BranchVideoInput
    {
        
        public string BranchCode { get; set; }

        public string CreatedBy { get; set; }
        
        public string Videoname { get; set; }
        public string VideoUrl { get; set; }
        public string Video { get; set; }
    }
}