using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using System.Collections;
using System.Web.Script.Serialization;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft;
using Newtonsoft.Json;
using NarenFitnessUsers.Models.FinanceReConcillation;
using NarenFitnessUsers.Models.Invoice;
using NarenFitnessUsers.Models.Finance;

namespace NarenFitnessUsers.Controllers
{
    public class FinanceController : Controller
    {

        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        private DataTable getdata(string query)
        {
            SqlCommand cmd = new SqlCommand(query);

            using (SqlDataAdapter sda = new SqlDataAdapter())
            {
                cmd.Connection = cnn;
                sda.SelectCommand = cmd;
                using (DataTable dt = new DataTable())
                {
                    sda.Fill(dt);
                    return dt;
                }
            }
        }
        public Object GetFinanceBankReconcilation([FromBody]FinanceankPlan fp)
        {
            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();

            DataTable dt_FinRecouncillation = getdata(string.Format("select TypeID,SubTypeID,LedgerID,VendorID,SourceId,FAPaymentModes,BranchCode,Amount,Date,InvoiceId,ExpenditureTo,BankAccountId,MatchedId,Recouncelled,FATID,Comments from FinanceBankReconcilation where BranchCode='{0}'  ", fp.BranchCode));
            FinanReconcilOpt fro = new FinanReconcilOpt();

            try
            {
                List<FinanceBankReconcilation> financerec = new List<FinanceBankReconcilation>();

                financerec.Add(new FinanceBankReconcilation
                {
                    TypeID = dt_FinRecouncillation.Rows[0]["TypeID"].ToString()
                    ,
                    SubTypeID = dt_FinRecouncillation.Rows[0]["SubTypeID"].ToString()
                    ,
                    LedgerID = dt_FinRecouncillation.Rows[0]["LedgerID"].ToString()
                    ,
                    VendorID = dt_FinRecouncillation.Rows[0]["VendorID"].ToString()
                    ,
                    SourceId = dt_FinRecouncillation.Rows[0]["SourceId"].ToString()
                    ,
                    FAPaymentModes = dt_FinRecouncillation.Rows[0]["FAPaymentModes"].ToString()
                    ,
                    BranchCode = dt_FinRecouncillation.Rows[0]["BranchCode"].ToString()
                    ,
                    Amount = dt_FinRecouncillation.Rows[0]["Amount"].ToString()
                    ,
                    Date = dt_FinRecouncillation.Rows[0]["Date"].ToString()
                    ,
                    InvoiceId = dt_FinRecouncillation.Rows[0]["InvoiceId"].ToString()
                    ,
                    ExpenditureTo = dt_FinRecouncillation.Rows[0]["ExpenditureTo"].ToString()
                    ,
                    BankAccountId = dt_FinRecouncillation.Rows[0]["BankAccountId"].ToString()
                    ,
                    MatchedId = dt_FinRecouncillation.Rows[0]["MatchedId"].ToString()
                    ,
                    Recouncelled = dt_FinRecouncillation.Rows[0]["Recouncelled"].ToString()
                    ,
                    FATID = dt_FinRecouncillation.Rows[0]["FATID"].ToString()
                    ,
                    Comments = dt_FinRecouncillation.Rows[0]["Comments"].ToString()

                });

                fro.status = "Success";
                fro.value = financerec;
                sJSONResponse = JsonConvert.SerializeObject(fro);

            }
            catch (Exception ec)
            {
                fro.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(fro);

            }
            return sJSONResponse;

        }
        public Object GetFinanceTransactionReconcilation([FromBody]FinanceankPlan fp)
        {

            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();

            DataTable dt_FinanceTransactionRecouncillation = getdata(string.Format("select TypeID,SourceID,SourceName,FAPaymentModes,BranchCode,MembershipCode,InvoiceID,Amount,PaymentDate,Receipt,AccountName,Comments,Source,BankAccountId from FinanceTransactionReconcilation where BranchCode='{0}' ", fp.BranchCode));
            FinanceTransactionReconcilationopt fro = new FinanceTransactionReconcilationopt();

            try
            {
                List<FinanceTransactionReconcilation> financetransactionrecouncill = new List<FinanceTransactionReconcilation>();

                financetransactionrecouncill.Add(new FinanceTransactionReconcilation
                {
                    TypeID = dt_FinanceTransactionRecouncillation.Rows[0]["TypeID"].ToString()
                   ,
                    SourceID = dt_FinanceTransactionRecouncillation.Rows[0]["SourceID"].ToString()
                   ,
                    SourceName = dt_FinanceTransactionRecouncillation.Rows[0]["SourceName"].ToString()
                    ,
                    FAPaymentModes = dt_FinanceTransactionRecouncillation.Rows[0]["FAPaymentModes"].ToString()
                    ,
                    BranchCode = dt_FinanceTransactionRecouncillation.Rows[0]["BranchCode"].ToString()
                    ,
                    MembershipCode = dt_FinanceTransactionRecouncillation.Rows[0]["MembershipCode"].ToString()
                    ,
                    InvoiceID = dt_FinanceTransactionRecouncillation.Rows[0]["InvoiceID"].ToString()
                    ,
                    Amount = dt_FinanceTransactionRecouncillation.Rows[0]["Amount"].ToString()
                    ,
                    PaymentDate = dt_FinanceTransactionRecouncillation.Rows[0]["PaymentDate"].ToString()
                    ,
                    Receipt = dt_FinanceTransactionRecouncillation.Rows[0]["Receipt"].ToString()
                    ,
                    AccountName = dt_FinanceTransactionRecouncillation.Rows[0]["AccountName"].ToString()
                    ,
                    Comments = dt_FinanceTransactionRecouncillation.Rows[0]["Comments"].ToString()
                    ,
                    Source = dt_FinanceTransactionRecouncillation.Rows[0]["Source"].ToString()
                    ,
                    BankAccountId = dt_FinanceTransactionRecouncillation.Rows[0]["BankAccountId"].ToString()



                });

                fro.status = "Success";
                fro.value = financetransactionrecouncill;
                sJSONResponse = JsonConvert.SerializeObject(fro);

            }
            catch (Exception ec)
            {
                fro.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(fro);

            }
            return sJSONResponse;

        }

        // EditPaymentDetails FAINvoice edit with all parementer (Invoiceid).
        public Object Invoice([FromBody]InvoiceInput ii)
        {

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            InvoiceFinalOpt ifo = new InvoiceFinalOpt();

            try
            {

                cnn.Close();
                cnn.Open();
                SqlCommand command = cnn.CreateCommand();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = cnn.BeginTransaction("SampleTransaction");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = cnn;
                command.Transaction = transaction;
                //InvoiceID
                command.CommandText = "update FAInvoice set FAPaymentModes='" + ii.FAPaymentModes + "',PayableAmount='" + ii.PayableAmount + "',AmountDue='" + ii.DueAmount + "',GymFees='" + ii.GymFee + "',TrainerFees='" + ii.PersonalTrainerFee + "',IGSTableAmount='" + ii.IGSTableAmount + "',IGST='" + ii.IGST + "',CGST='" + ii.CGST + "',SGST='" + ii.SGST + "',DueDate='" + ii.DueDate + "',FinalAmount='" + ii.FinalAmount + "',FAPaymentModes2='" + ii.FAPaymentModes2 + "',PayableAmount2='" + ii.PayableAmount2 + "',DiscountAmount='" + ii.DiscountAmount + "',WriteOFF='" + ii.WriteOFF + "',Wallet='" + ii.Wallet + "',PaymentDate='" + ii.PaymentDate + "',SlotPrice='" + ii.SlotPrice + "',PlanCost='',DuePaidAmount='" + ii.DuePaidAmount + "' where   InvoiceID ='" + ii.Invoice + "' ";
                command.ExecuteNonQuery();


                transaction.Commit();
                cnn.Close();


            }
            catch (Exception ece)
            {
            }
            finally
            {
                cnn.Close();
                ifo.status = "Success";
            }

            string sJSONResponse = JsonConvert.SerializeObject(ifo);
            return sJSONResponse;

        }

        // DailyCashFlowReport
        public Object GetFinance([FromBody]FinanceankPlan fp)
        {
            string sJSONResponse = "";
            FinanceOutput fop = new FinanceOutput();
            List<FinanceTypes> ftype = new List<FinanceTypes>();
            for (int i = 1; i <= 2; i++)
            {
                string Type = "";
                if (i == 2)
                { Type = "Expence"; }
                FinanceTypes dashboardlist = new FinanceTypes { TypeId = i, TypeName = "Income", SubTypes = GetSubType(i, fp.FromDate, fp.ToDate, fp.BranchCode) };
                ftype.Add(dashboardlist);
            }

            fop.status = "success";
            fop.value = ftype;
            sJSONResponse = JsonConvert.SerializeObject(fop);

            return sJSONResponse;

        }
        public List<FinanceList> GetSubType(int TypeId, DateTime FromDate, DateTime ToDate, string BranchCode)
        {
            List<FinanceList> subtypes = new List<FinanceList>();
            DataTable dt_SAUcount = getdata(string.Format("select ID as SubTypeId,SubType from FinanceAccountingStandards where TypeID='{0}' ", TypeId));
            for (int i = 1; i <= dt_SAUcount.Rows.Count - 1; i++)
            {
                subtypes.Add(new FinanceList { SubTypeId = Convert.ToInt32(dt_SAUcount.Rows[i]["SubTypeId"].ToString()), SubTypeName = dt_SAUcount.Rows[i]["SubType"].ToString(), Items = GetItems(Convert.ToInt32(dt_SAUcount.Rows[i]["SubTypeId"].ToString()), FromDate, ToDate, BranchCode) });
            }
            return subtypes;

        }
        public List<Items> GetItems(int SubTypeId, DateTime FromDate, DateTime ToDate, string BranchCode)
        {
            List<Items> Items = new List<Items>();
            DataTable dt_SAUcount = getdata(string.Format("select ID as ItemId,ItemName from FinanceAccountingList where SubTypeID='{0}' ", SubTypeId));
            for (int i = 1; i <= dt_SAUcount.Rows.Count - 1; i++)
            {

                Items.Add(new Items { ItemId = Convert.ToInt32(dt_SAUcount.Rows[i]["ItemId"].ToString()), ItemName = dt_SAUcount.Rows[i]["ItemName"].ToString(), Transactions = GetTransactions(Convert.ToInt32(dt_SAUcount.Rows[i]["ItemId"].ToString()), FromDate, ToDate, BranchCode) });
            }
            return Items;

        }
        public List<FinanceAccountingTransactions> GetTransactions(int ItemId, DateTime FromDate, DateTime ToDate, string BranchCode)
        {
            List<FinanceAccountingTransactions> Transactions = new List<FinanceAccountingTransactions>();

            DataTable dt_SAUcount = getdata(string.Format("select FAPaymentModes,InvoiceId,Amount,Date,ExpenditureTo,BankAccountId,SourceType,TransactionDate from FinanceAccountingTransactions where LedgerID='{0}' and Date between '{1}' and '{2}' and BranchCode='{3}' ", ItemId, FromDate, ToDate, BranchCode));
            for (int i = 1; i <= dt_SAUcount.Rows.Count - 1; i++)
            {
                Transactions.Add(new FinanceAccountingTransactions { FAPaymentModes = dt_SAUcount.Rows[i]["FAPaymentModes"].ToString(), InvoiceId = dt_SAUcount.Rows[i]["InvoiceId"].ToString(), Amount = dt_SAUcount.Rows[i]["Amount"].ToString(), Date = dt_SAUcount.Rows[i]["Date"].ToString(), Invoice = dt_SAUcount.Rows[i]["ExpenditureTo"].ToString(), ExpenditureTo = dt_SAUcount.Rows[i]["BankAccountId"].ToString(), BankAccountId = dt_SAUcount.Rows[i]["SourceType"].ToString(), SourceType = dt_SAUcount.Rows[i]["InvoiceId"].ToString() });
            }
            return Transactions;

        }
        public Object FinanceAccountingTransactions([FromBody]FinanceankPlan fp)
        {
            FATopt FOutputA = new FATopt();
            DataSet ds_SlotBooking = new DataSet();
            string sJSONResponse = "";

            try
            {

                List<FATTypes> types = new List<FATTypes>();
                for (int i = 1; i <= 3; i++)
                {
                    if (i == 1)
                        types.Add(new FATTypes { TypeId = 1, TypeName = "CashInFlow", SubType = GetSubTypes(1) });
                    if (i == 2)
                        types.Add(new FATTypes { TypeId = 2, TypeName = "CashOutFlow", SubType = GetSubTypes(2) });
                }

                FOutputA.status = "success";
                FOutputA.value = types;

                sJSONResponse = JsonConvert.SerializeObject(FOutputA);
            }
            catch (Exception ec)
            {
                FOutputA.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(FOutputA);
            }


            return sJSONResponse;
        }
        public List<FATSubTypes> GetSubTypes(int TypeId)
        {
            List<FATSubTypes> subtypes = new List<FATSubTypes>();
            DataTable dt_SAUcount = getdata(string.Format("select ID as SubTypeID,SubType from FinanceAccountingStandards where TypeID='{0}' ", TypeId));
            for (int i = 1; i <= dt_SAUcount.Rows.Count - 1; i++)
            {
                subtypes.Add(new FATSubTypes { SubTypeId = Convert.ToInt32(dt_SAUcount.Rows[i]["SubTypeID"].ToString()), SubTypeName = dt_SAUcount.Rows[i]["SubType"].ToString(), Ledger = GetLedger(Convert.ToInt32(dt_SAUcount.Rows[i]["SubTypeID"].ToString())) });
            }
            return subtypes;

        }
        public List<FATLedgers> GetLedger(int SubTypeId)
        {
            List<FATLedgers> subtypes = new List<FATLedgers>();
            DataTable dt_SAUcount = getdata(string.Format("select LedgerID,sum(Amount) as Amount from FinanceAccountingTransactions where SubTypeID='{0}' group by LedgerID ", SubTypeId));
            for (int i = 1; i <= dt_SAUcount.Rows.Count - 1; i++)
            {
                subtypes.Add(new FATLedgers { LedgerId = Convert.ToInt32(dt_SAUcount.Rows[i]["LedgerID"].ToString()), Amount = float.Parse(dt_SAUcount.Rows[i]["Amount"].ToString()) });
            }
            return subtypes;

        }

    }
}