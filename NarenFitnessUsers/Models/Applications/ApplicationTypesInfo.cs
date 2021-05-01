using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Applications
{
    public class ApplicationTypesInfo
    {
        public string Status { get; set; }
        public int AppTypeId { get; set; }
        public int AppTypeInfoId { get; set; }
        public string ImageId { get; set; }
        public string ImageName { get; set; }
        public string ImageURL { get; set; }
        public string ImagePhoto { get; set; }
        public string CreatedBy { get; set; }
    }
}