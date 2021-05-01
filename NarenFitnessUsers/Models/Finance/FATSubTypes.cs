using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Finance
{
    public class FATSubTypes
    {
        public int SubTypeId { get; set; }
        public string SubTypeName { get; set; }
        public List<FATLedgers> Ledger { get; set; }
    }
}