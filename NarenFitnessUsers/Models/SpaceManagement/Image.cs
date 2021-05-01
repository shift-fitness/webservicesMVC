using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.SpaceManagement
{
    public class Image
    {
        public string imageName { get; set; }
        public List<ImageDetails> images { get; set; }
    }
}