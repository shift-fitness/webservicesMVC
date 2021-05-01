using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Finance
{
    public class FinanceTypes
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public List<FinanceList> SubTypes { get; set; }
    }
}