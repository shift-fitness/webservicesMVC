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
using NarenFitnessUsers.Models.Applications;
using NarenFitnessUsers.Models.RazorPay;
using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;
using Razorpay.Api;
using System.Runtime.Serialization;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace NarenFitnessUsers.Controllers
{
    public class RazorOrderIdGenerationController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        JsonClass json = new JsonClass();
        Notification nfc = new Notification();
        // GET: ApplicationTypes
        public string orderId;
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
        public Object GetOrderId1([FromBody]RazorPaymentcs Order)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string AppType_Query = "";
            PaymentDetailsGet Pdetails = new PaymentDetailsGet();

            string ReceiptNos = ReceiptNo();

            try
            {

                Dictionary<string, object> input = new Dictionary<string, object>();
                input.Add("amount", Order.Amount); // this amount should be same as transaction amount
                input.Add("currency", Order.Currency);
                input.Add("receipt", ReceiptNos);
                input.Add("payment_capture", 1);



                // string key = "rzp_live_1FlbqBOlw7TqHA";
                //string secret = "6UJyvn78r9xNlbckRZZDvqEZ";

                string key = "rzp_test_51inWTBswRzU37";
                string secret = "u8dI6qRiT8bpvNIdTqvju7bQ";

                RazorpayClient client = new RazorpayClient(key, secret);

                Razorpay.Api.Order order = client.Order.Create(input);
                orderId = order["id"].ToString();

                cnn.Open();
                AppType_Query = "insert into PaymentGateway(GatewayProviderName,OrderId,BranchCode,MobileNo,ModeOfPayment,Amount,Currency,receipt,TransactionDate,CreatedOn,CreatedBy,IsDeleted,IsActive) values('GPN001','" + orderId + "','Brc1101','" + Order.MobileNo + "','OnLine','" + Order.Amount + "','INR','" + ReceiptNos + "','" + Order.TransactionDate + "','" + ServerDateTime + "','" + Order.MobileNo + "',0,1)  SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(AppType_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());

                Pdetails.status = "success";
                Pdetails.orderId = orderId;


            }
            catch (Exception ex)
            {
                Pdetails.status = "Fail";

            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(Pdetails);

            return sJSONResponse;
        }
        public ArrayList GetMarchantKey(string BranchCode)
        {
            ArrayList arl_MerchantKey = new ArrayList();

            string query = "select MarchantKey,MarchantSecretKey from MarchantDetails where BranchCode='" + BranchCode + "' ";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_Login = cmd_SubEXT.ExecuteReader();
                if (DR_Login.Read())
                {
                    arl_MerchantKey.Add(DR_Login[0].ToString());
                    arl_MerchantKey.Add(DR_Login[1].ToString());
                }
                cnn.Close();
            }
            return arl_MerchantKey;
        }

        public Object GetMerchantKeyByBranchCode([FromBody]RazorPaymentcs Order)
        {
            //merchant
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            MerchantDetailsOutput mdopt = new MerchantDetailsOutput();

            List<MerchantDetails> mdetails = new List<MerchantDetails>();
            ArrayList MerchantDetails = GetMarchantKey(Order.BranchCode);

            try
            {


                mdetails.Add(new MerchantDetails { MerchantKey = MerchantDetails[0].ToString(), MerchantSecretKey = MerchantDetails[1].ToString() });

                mdopt.value = mdetails;
                mdopt.status = "success";
            }
            catch (Exception ex)
            {

                mdopt.status = "fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(mdopt);

            return sJSONResponse;
        }

        public Object GetOrderId([FromBody]RazorPaymentcs Order)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string AppType_Query = "";
            string BranchCode = "";
            PaymentDetailsGet Pdetails = new PaymentDetailsGet();
            //string key = "rzp_live_1FlbqBOlw7TqHA";

            ArrayList arl_RazorPay = new ArrayList();

            if (Order.Mode == "Online")
            {
                BranchCode = "1104";
                arl_RazorPay = GetMarchantKey(BranchCode);
            }
            else if (Order.Mode == "Offline")
            {
                BranchCode = Order.BranchCode;
                arl_RazorPay = GetMarchantKey(BranchCode);
            }


            string key = arl_RazorPay[0].ToString();
            string secret = arl_RazorPay[1].ToString();
            string ReceiptNos = ReceiptNo();

            try
            {

                Dictionary<string, object> input = new Dictionary<string, object>();
                input.Add("amount", Order.Amount); // this amount should be same as transaction amount
                input.Add("currency", Order.Currency);
                input.Add("receipt", ReceiptNos);
                input.Add("payment_capture", 1);

                RazorpayClient client = new RazorpayClient(key, secret);

                Razorpay.Api.Order order = client.Order.Create(input);
                orderId = order["id"].ToString();

                cnn.Open();
                AppType_Query = "insert into PaymentGateway(GatewayProviderName,OrderId,BranchCode,MobileNo,ModeOfPayment,Amount,Currency,receipt,TransactionDate,CreatedOn,CreatedBy,IsDeleted,IsActive) values('RAZPAY','" + orderId + "','" + BranchCode + "','" + Order.MobileNo + "','OnLine','" + Order.Amount + "','INR','" + ReceiptNos + "','" + Order.TransactionDate + "','" + ServerDateTime + "','" + Order.MobileNo + "',0,1)  SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(AppType_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());

                Pdetails.status = "success";
                Pdetails.orderId = orderId;


            }
            catch (Exception ex)
            {
                Pdetails.status = "Fail";

            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(Pdetails);

            return sJSONResponse;
        }
        public string ReceiptNo()
        {
            Random random = new Random();
            string r = "";
            try
            {
                for (int i = 1; i < 5; i++)
                {
                    r += random.Next(1, 9).ToString();

                }
            }
            catch (Exception ex)
            {
                return "0";
            }
            return r;

        }
        public Object PaymentDetails([FromBody]RazorPaymentcs Payment)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string AppType_Query = "";
            PaymentDetailsPost Pdetails = new PaymentDetailsPost();

            try
            {
                cnn.Open();
                AppType_Query = "update PaymentGateway set payment_capture='" + Payment.payment_capture + "',TransactionStatus='" + Payment.TransactionStatus + "',receipt='" + Payment.receipt + "',notes='" + Payment.notes + "',Signature='" + Payment.Signature + "' where OrderId='" + Payment.OrderId + "'  SELECT @@IDENTITY; ";
                SqlCommand tm_cmd = new SqlCommand(AppType_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                Pdetails.status = "success";
                Pdetails.transactionId = Payment.TransactionId;
            }
            catch (Exception ex)
            {
                Pdetails.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(Pdetails);

            return sJSONResponse;
        }
        public ArrayList MSerialCheck(string BranchCode)
        {
            int Year = DateTime.Now.Year - 2000;
            int Month = DateTime.Now.Month;


            if (cnn.State == ConnectionState.Open)
            {
                cnn.Close();
            }
            ArrayList al = new ArrayList();
            int SerialNo = 0;
            string SCode = string.Empty;
            string Codification = string.Empty;
            int Count = 0;
            string query1 = "select Max(SerialNo),COUNT(*) as SerialNo from CCRMMembershipCodification where BranchCode='" + BranchCode + "' and Year=" + Year + "  and Month=" + Month + " ";
            try
            {
                using (SqlCommand cmd_RecGen = new SqlCommand(query1, cnn))
                {
                    cnn.Open();
                    SqlDataReader DR_RecGen = cmd_RecGen.ExecuteReader();
                    if (DR_RecGen.Read())
                    {
                        try
                        {
                            SerialNo = Convert.ToInt32(DR_RecGen[0].ToString());
                            Count = Convert.ToInt32(DR_RecGen[0].ToString());
                        }
                        catch (Exception ec)
                        {
                            Count = 0;

                        }
                    }

                    cnn.Close();
                }

                if (Count == 0)
                {
                    SerialNo = 1;
                    SCode = "001";
                }
                else
                {
                    SerialNo = SerialNo + 1;

                    if (SerialNo > 1 && SerialNo < 10)
                        SCode = "00" + Convert.ToString(SerialNo);
                    else if (SerialNo > 9 && SerialNo < 100)
                        SCode = "0" + Convert.ToString(SerialNo);
                    else if (SerialNo > 99 && SerialNo < 1000)
                        SCode = Convert.ToString(SerialNo);
                    else
                        SCode = "00000";
                }

            }
            catch (Exception ec)
            {
                SerialNo = 0;
            }
            finally
            {

                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }

            if (Month < 10)
            {
                Codification = Convert.ToString(Year) + "0" + Convert.ToString(Month) + SCode;
            }
            else
            {
                Codification = Convert.ToString(Year) + Convert.ToString(Month) + SCode;
            }


            al.Add(SCode);
            al.Add(Codification);
            return al;
        }
        public ArrayList MCode(string BranchCode)
        {
            ArrayList al = new ArrayList();
            string MNCode = string.Empty;
            String BCode = BranchCode.Substring(1, BranchCode.Length - 1);
            ArrayList SerialNo = MSerialCheck(BranchCode);
            MNCode = SerialNo[1].ToString() + BCode;
            al.Add(SerialNo[0].ToString());
            al.Add(MNCode);

            return al;
        }
        public string UniqueGeneration2()
        {
            Random random = new Random();
            string r = "";
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    r += random.Next(0, 9).ToString();
                }
            }
            catch (Exception ex)
            {
                return "";
            }
            return r;

        }
        public int UIDNew(string BranchCode)
        {
            int UIDNew = 0;
            string query = "select isnull((Max(UID)+1), 3001) as UID from LoginID where BranchCode='" + BranchCode + "'";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_Login = cmd_SubEXT.ExecuteReader();
                if (DR_Login.Read())
                {
                    UIDNew = Convert.ToInt32(DR_Login[0].ToString());
                }
                cnn.Close();
            }
            return UIDNew;
        }
        public int InvoiceCheck(string Invoice)
        {
            int InvoiceCheck = 0;
            string query = "select COUNT(*) as Count from FAInvoice where InvoiceID='" + Invoice + "' ";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_Login = cmd_SubEXT.ExecuteReader();
                if (DR_Login.Read())
                {
                    InvoiceCheck = Convert.ToInt32(DR_Login[0].ToString());
                }
                cnn.Close();
            }
            return InvoiceCheck;
        }
        public Object PostPaymentDetails([FromBody]CapturePaymentDetails Order)
        {
            string sJSONResponse = "";
            try
            {
                RazorPaymentFinalOutput RPFO = new RazorPaymentFinalOutput();
                string r = "";
                DataSet ds_PaymentDetailsPost = new DataSet();

                DataTable dt_AppType = new DataTable();
                string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                ArrayList arl_pdetails = GetPersonDetailsByMobileNo(Order.MobileNo);
                PaymentDetailsGet Pdetails = new PaymentDetailsGet();

                int Year = DateTime.Now.Year - 2000;
                int Month = DateTime.Now.Month;


                // Clients Unique COde
                ArrayList al = new ArrayList();

                string MembershipCode = GetMembersExistance(Order.MobileNo);


                if (Order.EnquireTypeNo == 2)
                {
                    r = "";
                }
                else
                {
                    if (MembershipCode == null || MembershipCode == "" || MembershipCode == string.Empty)
                    {
                        al = MCode("1104");
                        r = (String)al[1].ToString();
                    }
                    else
                    {
                        r = MembershipCode;
                    }

                }

                // Invoice Generation
                string Invoice = "N" + UniqueGeneration2();
                int UID = UIDNew("1104");

                if (InvoiceCheck(Invoice) != 0)
                {
                    Invoice = "N" + UniqueGeneration2();
                }

                string ReceiptNos = ReceiptNo();

                cnn.Open();
                SqlCommand command = cnn.CreateCommand();
                SqlTransaction transaction;
                // Start a local transaction.
                transaction = cnn.BeginTransaction("SampleTransaction");
                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = cnn;
                command.Transaction = transaction;


                try
                {
                    {

                        if (Order.EnquireTypeNo != 2)
                        {
                            if (MembershipCode == null || MembershipCode == "" || MembershipCode == string.Empty)
                            {
                                command.CommandText = "insert into Users(UCode,BranchCode,UserName,Firstname,Lastname,DateOfBirth,Gender,MaritialStatus,Address,Area,City,State,Country,PinCode,MobileNo,CreatedBy,CreatedOn,IsDeleted,IsActive,Email,Photo,Address2,PhotoUrl) values('" + r + "','1104','" + arl_pdetails[0].ToString() + "','" + arl_pdetails[0].ToString() + "','','" + arl_pdetails[1].ToString() + "','" + arl_pdetails[2].ToString() + "','','','','','','','','" + Order.MobileNo + "','" + Order.MobileNo + "','" + ServerDateTime + "',0,1,'','','','')";
                                command.ExecuteNonQuery();
                                command.CommandText = "insert into CCRMMembershipCodification(SerialNo,MembershipCode,BranchCode,Year,Month) values(" + al[0].ToString() + ",'" + r + "','1104','" + Convert.ToString(Year) + "','" + Convert.ToString(Month) + "')";
                                command.ExecuteNonQuery();
                            }
                            //command.CommandText = "insert into CCRMMEnquireStatus(MobileNo,EnquireTypeNo,MobileDeviceID,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + Order.MobileNo + "','" + Order.EnquireTypeNo + "','" + Order.MobileDeviceID + "','" + Order.MobileNo + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            command.CommandText = "insert into PaymentGateway(GatewayProviderName,OrderId,BranchCode,MemberShipCode,Invoice,MobileNo,ModeOfPayment,payment_capture,Amount,Currency,Discount,receipt,TransactionDate,CreatedOn,CreatedBy,IsDeleted,IsActive) values('GPN001','" + Order.OrderId + "','1104','" + r + "','" + Invoice + "','" + Order.MobileNo + "','" + Order.ModeOfPayment + "','" + Order.PaymentId + "','" + Order.Amount + "','INR','" + Order.DiscountAmount + "','" + ReceiptNos + "','" + Order.TransactionDate + "','" + ServerDateTime + "','" + Order.CreatedBy + "',0,1)";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into OnlinePackagePurchase(BranchCode,MembershipCode,MobileNo,EnquireTypeNo,PackageID,StartDate,EndDate,ActualPrice,NumberOfSession,NumberOfDaysValidity,MobileDeviceID,Invoice,Discount,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('1104','" + r + "','" + Order.MobileNo + "','" + Order.EnquireTypeNo + "','" + Order.PackageID + "','" + Order.StartDate + "','" + Order.EndDate + "','" + Order.ActualPrice + "','" + Order.NumberOfSession + "','" + Order.NumberOfDaysValidity + "','" + Order.MobileDeviceID + "','" + Invoice + "','" + Order.DiscountAmount + "','','" + Order.MobileNo + "','" + ServerDateTime + "',0,1)";
                            command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',1,'FREEZING','" + Order.Freezing + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',3,'UPGRADATION','" + Order.Upgrade + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',4,'CHANGE','" + Order.Change + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',5,'TRANSFER TRANSFER','" + Order.Transfer + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',6,'PERSON TRANSFER','" + Order.Transfer + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',8,'HOLD','" + Order.Transfer + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',9,'Paused','" + Order.Paused + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',10,'Convert','" + Order.Convert + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                        }
                        else
                        {

                            //command.CommandText = "insert into CCRMMEnquireStatus(MobileNo,EnquireTypeNo,MobileDeviceID,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + Order.MobileNo + "','" + Order.EnquireTypeNo + "','" + Order.MobileDeviceID + "','" + Order.MobileNo + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            command.CommandText = "insert into PaymentGateway(GatewayProviderName,OrderId,BranchCode,MemberShipCode,Invoice,MobileNo,ModeOfPayment,payment_capture,Amount,Currency,Discount,receipt,TransactionDate,CreatedOn,CreatedBy,IsDeleted,IsActive) values('GPN001','" + Order.OrderId + "','1104','" + r + "','" + Invoice + "','" + Order.MobileNo + "','" + Order.ModeOfPayment + "','" + Order.PaymentId + "','" + Order.Amount + "','INR','" + Order.DiscountAmount + "','" + ReceiptNos + "','" + Order.TransactionDate + "','" + ServerDateTime + "','" + Order.CreatedBy + "',0,1)";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into OnlinePackagePurchase(BranchCode,MembershipCode,MobileNo,EnquireTypeNo,PackageID,StartDate,EndDate,ActualPrice,NumberOfSession,NumberOfDaysValidity,MobileDeviceID,Invoice,Discount,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('1104','" + r + "','" + Order.MobileNo + "','" + Order.EnquireTypeNo + "','" + Order.PackageID + "','" + Order.StartDate + "','" + Order.EndDate + "','" + Order.ActualPrice + "','" + Order.NumberOfSession + "','" + Order.NumberOfDaysValidity + "','" + Order.MobileDeviceID + "','" + Invoice + "','" + Order.DiscountAmount + "','','" + Order.MobileNo + "','" + ServerDateTime + "',0,1)";
                            command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',1,'FREEZING','" + Order.Freezing + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',3,'UPGRADATION','" + Order.Upgrade + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',4,'CHANGE','" + Order.Change + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',5,'TRANSFER TRANSFER','" + Order.Transfer + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',6,'PERSON TRANSFER','" + Order.Transfer + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',8,'HOLD','" + Order.Transfer + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',9,'Paused','" + Order.Paused + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                            //command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',10,'Convert','" + Order.Convert + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            //command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        cnn.Close();

                        List<RazorPaymentPost> dbPaymentPost = new List<RazorPaymentPost>();
                        RazorPaymentPost dashboardlistpackage = new RazorPaymentPost { invoice = Convert.ToString(Invoice), memberShipCode = Convert.ToString(r), orderId = Convert.ToString(Order.OrderId) };
                        dbPaymentPost.Add(dashboardlistpackage);
                        nfc.PushToMobile("", "Alert From PostPaymentDetails");
                        int val = SendMessage(Order.MobileNo, arl_pdetails[0].ToString());
                        RPFO.status = "success";
                        RPFO.value = dbPaymentPost;
                        sJSONResponse = JsonConvert.SerializeObject(RPFO);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                        RPFO.status = "Fail";
                    }
                    catch (Exception ex2)
                    {
                        RPFO.status = "Fail";
                    }
                }
                sJSONResponse = JsonConvert.SerializeObject(RPFO);
            }
            catch (Exception ec)
            {

            }
            return sJSONResponse;
        }
        public int CheckExistency(string MembershipCode, string PlanCode, string SlotCode, string PackageCode, DateTime StartDate, DateTime EndDate)
        {
            //string query = "select count(*) as Count from CCRMMembership where MembershipCode='" + MembershipCode + "' and PlanCode='" + PlanCode + "' and SlotCode='" + SlotCode + "' and PackageCode='" + PackageCode + "' and DATEADD(dd, DATEDIFF(dd, 0, '" + StartDate + "'), 0)='' and DATEADD(dd, DATEDIFF(dd, 0, '" + EndDate + "'), 0)='' ";
            string query = "select count(*) as Count from CCRMMembership where MembershipCode='" + MembershipCode + "' and PlanCode='" + PlanCode + "' and SlotCode='" + SlotCode + "' and PackageCode='" + PackageCode + "'  ";
            int Exentency = 0;
            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Close();
                cnn.Open();
                SqlDataReader DR_Exist = cmd_SubEXT.ExecuteReader();
                if (DR_Exist.Read())
                {
                    Exentency = Convert.ToInt32(DR_Exist[0].ToString());
                }
                cnn.Close();
            }

            return Exentency;

        }
        public ArrayList GetGSTCode()
        {
            //-- GYMFEES','Trainer Fees','IGSTableAmount','IGST','CGST','SGST
            //IGSTtableamount - IGST
            //payableamount* Personaltrainer%
            //payableamount * Gymfee %
            //IGSTtableamount * IGST
            //IGSTtableamount* CGST
            //IGSTtableamount * SGST

            ArrayList arl_gst = new ArrayList();
            string GSTCode = "";
            int IGSTPercentage = 0;
            int CGSTPercentage = 0;
            int SGSTPercentage = 0;
            int GYMFeePercentage = 0;
            int PersonalTrainerPercentage = 0;

            string query = "select GSTCode,IGSTPercentage,CGSTPercentage,SGSTPercentage,GYMFeePercentage,PersonalTrainerPercentage from FAGST ";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_GST = cmd_SubEXT.ExecuteReader();
                if (DR_GST.Read())
                {
                    GSTCode = DR_GST[0].ToString();
                    IGSTPercentage = Convert.ToInt32(DR_GST[1].ToString());
                    CGSTPercentage = Convert.ToInt32(DR_GST[2].ToString());
                    SGSTPercentage = Convert.ToInt32(DR_GST[3].ToString());
                    GYMFeePercentage = Convert.ToInt32(DR_GST[4].ToString());
                    PersonalTrainerPercentage = Convert.ToInt32(DR_GST[5].ToString());
                }
                cnn.Close();
            }

            arl_gst.Add(GSTCode);
            arl_gst.Add(IGSTPercentage);
            arl_gst.Add(CGSTPercentage);
            arl_gst.Add(SGSTPercentage);
            arl_gst.Add(GYMFeePercentage);
            arl_gst.Add(PersonalTrainerPercentage);

            return arl_gst;

        }
        public Object OffLinePostPaymentDetails([FromBody]OffLineCapturePaymentDetails Order)
        {

            OffLineRazorPaymentFinalOutput RPFO = new OffLineRazorPaymentFinalOutput();
            OffLineRazorPaymentFinalOutputMessage RPFOM = new OffLineRazorPaymentFinalOutputMessage();

            string r = "";
            DataSet ds_PaymentDetailsPost = new DataSet();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            ArrayList arl_pdetails = GetPersonDetailsByMobileNo(Order.MobileNo);
            PaymentDetailsGet Pdetails = new PaymentDetailsGet();

            int Year = DateTime.Now.Year - 2000;
            int Month = DateTime.Now.Month;
            //-- GYMFEES','Trainer Fees','IGSTableAmount','IGST','CGST','SGST
            //IGSTtableamount - IGST
            //payableamount* Personaltrainer%
            //payableamount * Gymfee %
            //IGSTtableamount * IGST
            //IGSTtableamount* CGST
            //IGSTtableamount * SGST

            ArrayList ary_GST = new ArrayList();
            ary_GST = GetGSTCode();
            // Clients Unique COde
            ArrayList al = new ArrayList();
            string GSTCode = "";
            try
            {
                GSTCode = ary_GST[0].ToString();
            }
            catch (Exception ec)
            {
            }

            float IGSTPercentage = float.Parse(ary_GST[1].ToString());
            float CGSTPercentage = float.Parse(ary_GST[2].ToString());
            float SGSTPercentage = float.Parse(ary_GST[3].ToString());
            float GYMFeePercentage = float.Parse(ary_GST[4].ToString());
            float PersonalTrainerPercentage = float.Parse(ary_GST[5].ToString());

            float PayableAmount = Convert.ToInt32(Order.PayableAmount);

            float PersonalTrainer = PayableAmount * (PersonalTrainerPercentage / 100);
            float IGSTableAmount = PayableAmount * (GYMFeePercentage / 100);
            float IGST = PayableAmount * (IGSTPercentage / 100);
            float CGST = PayableAmount * (CGSTPercentage / 100);
            float SGST = PayableAmount * (SGSTPercentage / 100);
            float GYMFEE = IGSTableAmount - IGST;
            int SerialNo = 0;

            string MembershipCode = GetMembersExistance(Order.MobileNo);

            if (MembershipCode == null || MembershipCode == "" || MembershipCode == string.Empty)
            {
                al = MCode(Order.BranchCode);
                r = (String)al[1].ToString();
                SerialNo = Convert.ToInt32(al[0].ToString());
            }
            else
            {
                r = MembershipCode;

            }
            // Invoice Generation
            string Invoice = "N" + UniqueGeneration2();
            int UID = UIDNew(Order.BranchCode);

            if (InvoiceCheck(Invoice) != 0)
            {
                Invoice = "N" + UniqueGeneration2();
            }



            string ReceiptNos = ReceiptNo();
            if (CheckExistency(MembershipCode, Order.PlanCode, Order.SlotCode, Order.PackageCode, Convert.ToDateTime(Order.MembershipStartDate), Convert.ToDateTime(Order.MembershipExpireDate)) == 0)
            {
                cnn.Open();
                SqlCommand command = cnn.CreateCommand();
                SqlTransaction transaction;
                // Start a local transaction.
                transaction = cnn.BeginTransaction("SampleTransaction");
                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    {

                        if (MembershipCode == null || MembershipCode == "" || MembershipCode == string.Empty)
                        {

                            command.CommandText = "insert into Users(UCode,BranchCode,UserName,Firstname,Lastname,DateOfBirth,Gender,MaritialStatus,Address,Area,City,State,Country,PinCode,MobileNo,CreatedBy,CreatedOn,IsDeleted,IsActive,Email,Photo,Address2,PhotoUrl) values('" + r + "','" + Order.BranchCode + "','" + arl_pdetails[0].ToString() + "','" + arl_pdetails[0].ToString() + "','','" + arl_pdetails[1].ToString() + "','" + arl_pdetails[2].ToString() + "','','','','1','1','1','','" + Order.MobileNo + "','" + Order.MobileNo + "','" + ServerDateTime + "',0,1,'','','','')";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into CCRMMembershipCodification(SerialNo,MembershipCode,BranchCode,Year,Month) values(" + al[0].ToString() + ",'" + r + "','" + Order.BranchCode + "','" + Convert.ToString(Year) + "','" + Convert.ToString(Month) + "')";
                            command.ExecuteNonQuery();
                        }
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',1,'FREEZING','" + Order.Freezing + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',3,'UPGRADATION','" + Order.Upgrade + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',4,'CHANGE','" + Order.Change + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',5,'TRANSFER TRANSFER','" + Order.Transfer + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',6,'PERSON TRANSFER','" + Order.Transfer + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',8,'HOLD','" + Order.Transfer + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',9,'Paused','" + Order.Paused + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',10,'Convert','" + Order.Convert + "','" + Order.Invoice + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into CCRMMEnquireStatus(MobileNo,EnquireTypeNo,MobileDeviceID,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + Order.MobileNo + "','" + Order.EnquireTypeNo + "','" + Order.MobileDeviceID + "','" + Order.MobileNo + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into CCRMMembership(MembershipCode,BranchCode,PlanCode,MembershipExpireDate,CreatedBy,CreatedOn,IsDeleted,IsActive,SlotCode,PackageCode,StartDate,TrainerCode,InvoiceID,Receipt,SerialNo,Year,Month,PlanCostCode,DurationId,EnquireTypeNo,EnquireTypeIncentives) values('" + r + "','" + Order.BranchCode + "','" + Order.PlanCode + "','" + Order.MembershipExpireDate + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1,'" + Order.SlotCode + "','" + Order.PackageCode + "','" + Order.MembershipStartDate + "','" + Order.TrainerCode + "','" + Invoice + "','" + ReceiptNo() + "'," + SerialNo + ",'" + Year + "','" + Month + "','" + Order.PlanCostCode + "','" + Order.DurationId + "','" + Order.EnquireTypeNo + "','" + Order.EnquireTypeNo + "')";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FAInvoice(InvoiceID,GSTCode,FAPaymentModes,MembershipCode,PayableAmount,AmountDue,GymFees,TrainerFees,IGSTableAmount,IGST,CGST,SGST,DueDate,CreatedBy,CreatedOn,IsDeleted,IsActive,FinalAmount,FAPaymentModes2,PayableAmount2,PromoCode,ReferenceID,DiscountAmount,TrainerCode,Receipt,EnquireTypeNo,SlotPrice,PlanCost,PaymentDate,RemainingAmount,Wallet,OrderId,Currency) values('" + Invoice + "','" + GSTCode + "','PY6','" + r + "','" + Order.PayableAmount + "','" + Order.AmountDue + "','" + GYMFEE + "','" + PersonalTrainer + "','" + IGSTableAmount + "','" + IGST + "','" + CGST + "','" + SGST + "','" + Order.DueDate + "','" + Order.CreatedBy + "','" + ServerDateTime + "',0,1,'" + Order.FinalAmount + "','0','" + Order.PayableAmount2 + "','" + Order.PromoCode + "','','" + Order.DiscountAmount + "','" + Order.TrainerCode + "','" + ReceiptNo() + "','" + Order.EnquireTypeNo + "','" + Order.SlotPrice + "','" + Order.PlanCost + "','" + Order.TransactionDate + "','0.0','" + Order.Wallet + "','" + Order.OrderId + "','" + Order.Currency + "')";
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        cnn.Close();

                        List<OffLineRazorPaymentPost> dbPaymentPost = new List<OffLineRazorPaymentPost>();
                        OffLineRazorPaymentPost dashboardlistpackage = new OffLineRazorPaymentPost { invoice = Convert.ToString(Invoice), memberShipCode = Convert.ToString(r), orderId = Convert.ToString(Order.OrderId) };
                        dbPaymentPost.Add(dashboardlistpackage);
                        // int val = SendMessage(Order.MobileNo, arl_pdetails[0].ToString());
                        RPFO.status = "success";
                        RPFO.value = dbPaymentPost;
                        sJSONResponse = JsonConvert.SerializeObject(RPFO);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                        RPFO.status = "Fail";
                    }
                    catch (Exception ex2)
                    {
                        RPFO.status = "Fail";
                    }
                }
                sJSONResponse = JsonConvert.SerializeObject(RPFO);
            }
            else
            {

                List<ErrorMessage> ErrorMessage = new List<ErrorMessage>();
                ErrorMessage erm_ErrorMessage = new ErrorMessage { errorMessage = "Plan Already Exist" };
                ErrorMessage.Add(erm_ErrorMessage);

                RPFOM.status = "Fail";
                RPFOM.value = ErrorMessage;
                sJSONResponse = JsonConvert.SerializeObject(RPFOM);
            }


            return sJSONResponse;
        }
        public int SendMessage(string MobileNo, string UserName)
        {

            try
            {
                DataSet ds_custdet = new DataSet();
                DataTable dt_CRM = new DataTable();
                string SendText = string.Empty;


                string httpUrl = string.Empty;
                {
                    SendText = "Hello '" + UserName + "' , Thank you for completing your online payment. Have a great day! NarenFitness.";
                    StreamReader objReader;
                    //httpUrl = "http://sms.smscity.in/httpapi/httpapi?token=8c84bb208c7c7b7b72660ef51a5430b0&sender=NRNGYM&number=0" + MobileNo + "&route=2&type=1&sms='" + SendText + "' ";
                    //httpUrl = "http://api.textlocal.in/send?username=karthikeyaenator@gmail.com&hash=f94d56486adbd53db4d28bcc515c415ef49267fc&numbers=" + MobileNo + "&message=" + SendText + "&sender=SUCARD ";
                    httpUrl = "http://api.textlocal.in/send?username=dilipgoud@narenfitness.com&hash=4a7076b97161c3a3f6c14028abebf7589808c1ec3ed68eaae63c44e0db3f6497&numbers=" + MobileNo + "&message=" + SendText + "&sender=NRNGYM ";

                    System.Net.WebRequest webRequest = System.Net.WebRequest.Create(httpUrl);
                    Stream objstream;
                    objstream = webRequest.GetResponse().GetResponseStream();
                    objReader = new StreamReader(objstream);
                    objReader.Close();
                }



                cnn.Close();

            }
            catch (Exception ex)
            {

            }
            return 1;
        }
        private DataTable ReturnException(string Status, string Message)
        {
            DataTable dtMsg = new DataTable("MSG");
            dtMsg.Columns.Add("status");
            dtMsg.Columns.Add("value");
            DataRow drMsg = dtMsg.NewRow();
            drMsg["status"] = Status;
            drMsg["value"] = Message;
            dtMsg.Rows.Add(drMsg);
            return dtMsg;
        }
        public ArrayList GetPersonDetailsByMobileNo(string MobileNo)
        {
            ArrayList arl_pd = new ArrayList();

            string query = "select EnquirePersonFirstName as UserName,D_O_B,Gender  from CCRMMEnquireForm  where MobileNo='" + MobileNo + "'";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_pdetails = cmd_SubEXT.ExecuteReader();
                if (DR_pdetails.Read())
                {
                    arl_pd.Add(DR_pdetails["UserName"].ToString());
                    arl_pd.Add(DR_pdetails["D_O_B"].ToString());
                    arl_pd.Add(DR_pdetails["Gender"].ToString());
                }
                cnn.Close();
            }
            return arl_pd;
        }
        public string GetMembersExistance(string MobileNo)
        {
            string MembershipCode = "";
            string query = "select top 1 UCode from Users where MobileNo='" + MobileNo + "' and MobileNo<>'' order by ID desc";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_pdetails = cmd_SubEXT.ExecuteReader();
                if (DR_pdetails.Read())
                {
                    MembershipCode = DR_pdetails[0].ToString();

                }
                cnn.Close();
            }
            return MembershipCode;
        }

    }
}