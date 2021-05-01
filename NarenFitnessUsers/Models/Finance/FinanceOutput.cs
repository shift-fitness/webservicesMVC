using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Finance
{
    public class FinanceOutput
    {
        public string status { get; set; }
        public List<FinanceTypes> value { get; set; }
    }
}