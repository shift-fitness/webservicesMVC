using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Applications
{
    public class GetApplicationTypes
    {
        public string Status { get; set; }

        public string Message { get; set; }
        public int AppTypeId { get; set; }
        public string AppTypeName { get; set; }
        public string CreatedBy { get; set; }
    }
}