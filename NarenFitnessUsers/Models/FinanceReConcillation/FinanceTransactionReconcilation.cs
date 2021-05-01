using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.FinanceReConcillation
{
    public class FinanceTransactionReconcilation
    {

        public string TypeID { get; set; }
        public string SourceID { get; set; }
        public string SourceName { get; set; }
        public string FAPaymentModes { get; set; }
        public string BranchCode { get; set; }
        public string MembershipCode { get; set; }
        public string InvoiceID { get; set; }
        public string Amount { get; set; }
        public string PaymentDate { get; set; }
        public string Receipt { get; set; }
        public string AccountName { get; set; }
        public string Comments { get; set; }
        public string Source { get; set; }
        public string BankAccountId { get; set; }

    }
}