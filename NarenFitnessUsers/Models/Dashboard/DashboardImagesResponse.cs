using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Dashboard
{
    public class DashboardImagesResponse
    {
        public string id { get; set; }
        public string displayId { get; set; }
        public string displayName { get; set; }
        public string appId { get; set; }

        public string applicationName { get; set; }
       
        public string screenId { get; set; }
        public string screenName { get; set; }

        public string imageURL { get; set; }
        
    }
}