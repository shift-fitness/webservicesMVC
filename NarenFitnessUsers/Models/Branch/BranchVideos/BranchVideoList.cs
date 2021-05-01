using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Branch.BranchVideos
{
    public class BranchVideoList
    {
        public string status { get; set; }
        public List<BranchVideosVideos> value { get; set; }
    }
}