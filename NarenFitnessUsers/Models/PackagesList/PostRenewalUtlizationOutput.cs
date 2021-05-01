using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.PackagesList
{
    public class PostRenewalUtlizationOutput
    {
        public string status { get; set; }
        public List<PostRenewalUtlizationValueOutput> value { get; set; }
    }
}