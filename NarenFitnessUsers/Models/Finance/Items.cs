using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NarenFitnessUsers.Models.Finance
{
    public class Items
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public List<FinanceAccountingTransactions> Transactions { get; set; }
    }
}