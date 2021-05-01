using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.FinanceReConcillation
{
    public class FinanceTransactionReconcilationopt
    {
        public string status { get; set; }
        public List<FinanceTransactionReconcilation> value { get; set; }
    }
}