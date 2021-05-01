using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Finance
{
    public class FATTypes
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public List<FATSubTypes> SubType { get; set; }
    }
}