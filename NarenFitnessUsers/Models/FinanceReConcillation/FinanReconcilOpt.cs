using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.FinanceReConcillation
{
    public class FinanReconcilOpt
    {
        public string status { get; set; }
        public List<FinanceBankReconcilation> value { get; set; }
    }
}