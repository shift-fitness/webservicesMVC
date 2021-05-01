using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Finance
{
    public class FinanceList
    {
        public int SubTypeId { get; set; }
        public string SubTypeName { get; set; }
        public List<Items> Items { get; set; }

    }
}