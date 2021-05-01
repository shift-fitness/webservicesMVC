using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Finance
{
    public class FinanceAccountingTransactions
    {
        public string FAPaymentModes { get; set; }
        public string InvoiceId { get; set; }
        public string RemainingAmount { get; set; }
        public string Amount { get; set; }
        public string Date { get; set; }
        public string Invoice { get; set; }
        public string ExpenditureTo { get; set; }
        public string BankAccountId { get; set; }
        public string SourceType { get; set; }
    }
}