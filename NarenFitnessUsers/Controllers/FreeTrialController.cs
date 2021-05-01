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
using NarenFitnessUsers.Models.SpaceManagement;
using NarenFitnessUsers.Models.Applications;
using NarenFitnessUsers.Models.RazorPay;
using NarenFitnessUsers.Models.Login;
using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;

namespace NarenFitnessUsers.Controllers
{
    public class FreeTrialController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object FreeTrial([FromBody]FreeTrial FTrial)
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
                AppType_Query = "insert into UIFreeTrial(MobileNo,EmailId,MobileDeviceId,SlotCode,TrainerId,PlanId,SessionDate,UMID,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + FTrial.MobileNo + "','" + FTrial.EmailId + "','" + FTrial.MobileDeviceId + "','" + FTrial.SlotCode + "','" + FTrial.TrainerId + "','" + FTrial.PlanId + "','" + FTrial.SessionDate + "','" + FTrial.UMID + "','" + FTrial.CreatedBy + "','" + ServerDateTime + "',0,1)  SELECT @@IDENTITY;";
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