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
using NarenFitnessUsers.Models.Notification;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;

namespace NarenFitnessUsers.Controllers
{
    public class NotificationController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object NotificationDetailsPost([FromBody]Notification not)
        {
            notificationoutput nop = new notificationoutput();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";
           

            try
            {
                cnn.Open();
                olPackage_Query = "insert into MobileDeviceDetails(RegisteredToken,MobileDeviceId,Status,IsDeleted,IsActive) values('"+not.RegisteredToken+"','"+not.MobileDeviceId+"','"+not.Status+"',0,1)";
                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteNonQuery());
                nop.status = "Success";
                nop.value = "";
            }
            catch (Exception ex)
            {
                nop.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(nop);

            return sJSONResponse;
        }
    }
}