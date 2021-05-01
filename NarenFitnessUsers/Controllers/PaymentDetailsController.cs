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


namespace NarenFitnessUsers.Controllers
{
    public class PaymentDetailsController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
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
                AppType_Query = "insert into PaymentGateway(GatewayProviderName,OrderId,BranchCode,MobileNo,ModeOfPayment,Amount,Currency,receipt,payment_capture,notes,TransactionDate,Signature,CreatedOn,CreatedBy,IsDeleted,IsActive) values('" + Payment.GatewayProviderName + "','" + Payment.OrderId + "','" + Payment.BranchCode + "','" + Payment.MobileNo + "','" + Payment.ModeOfPayment + "','" + Payment.Amount + "','" + Payment.Currency + "','" + Payment.receipt + "','" + Payment.payment_capture + "','" + Payment.notes + "','" + Payment.TransactionDate + "','" + Payment.Signature + "','" + ServerDateTime + "','" + Payment.CreatedBy + "',1,0)  SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(AppType_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                Pdetails.status = "Success";
                Pdetails.transactionId = a;
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
        public Object GetPaymentDetails([FromBody]RazorPaymentcs Payment)
        {
            GetApplicationInfoFail gaif = new GetApplicationInfoFail();

            var json = (object)null;
            try
            {

                DataSet ds_custdet1 = new DataSet();
                DataTable dt_ApplicationTypes = new DataTable();
                string query = "select AppTypeId,AppTypeName from ApplicationTypes";

                using (SqlDataAdapter da_custdet = new SqlDataAdapter(query, cnn))
                {
                    da_custdet.Fill(ds_custdet1);
                }
                if (ds_custdet1.Tables[0].Rows.Count > 0)
                {

                    json = Json(Payment, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    gaif.Status = "Sucess";
                    gaif.Messsage = "No Data";

                    json = Json(gaif, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ec)
            {

                gaif.Status = "Sucess";
                gaif.Messsage = "Internal Server Error";

                json = Json(gaif, JsonRequestBehavior.AllowGet);
            }
            return json;
        }
    }
}