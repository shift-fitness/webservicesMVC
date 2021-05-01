using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.FinanceReConcillation
{
    public class FinanceBankReconcilation
    {
        public string TypeID { get; set; }
        public string SubTypeID { get; set; }
        public string LedgerID { get; set; }
        public string VendorID { get; set; }
        public string SourceId { get; set; }
        public string FAPaymentModes { get; set; }
        public string BranchCode { get; set; }
        public string Amount { get; set; }
        public string Date { get; set; }
        public string InvoiceId { get; set; }
        public string status { get; set; }
        public string ExpenditureTo { get; set; }
        public string BankAccountId { get; set; }
        public string MatchedId { get; set; }
        public string Recouncelled { get; set; }
        public string FATID { get; set; }
        public string Comments { get; set; }

    }
}