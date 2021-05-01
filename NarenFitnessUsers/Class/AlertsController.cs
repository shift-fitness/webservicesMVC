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
using NarenFitnessUsers.Models.Alerts;
using System.Web.Script.Serialization;

namespace NarenFitnessUsers.Controllers
{
    public class AlertsController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        public string PushNotification([FromBody]Alertoutput Alert)
        {
            string str = "";
            try
            {
                var applicationID = "AAAAZr2c76I:APA91bECB3lqhLqbA2USAvapk4aU5z-jv6Wh5AjjHWAE8KnBjsV7MewC1LpermptoOgkk9MdeGf1IYJzKweyItn32s_UKeMok-0UoCCvcwapcxPG1As460N7SI_M99GMsCcFQWELOCD9";
                var senderId = "441267842978";
                string deviceId = "cQud78e6-uo:APA91bH3xFeSHnEet_xhQ0GtG2wjIW_z7uTgKMEJ6I823D-T8u0nr-aUa30izaZodp_Y4HD3UzTq36buwriO-okZeXh4Ob1LCNOK45vj5Tt9KXmqioAw0PE_HgDYq2H-4GKTgJLuOq_M";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    notification = new
                    {
                        body = "Alert from Server",
                        title = "Notificatipon",
                        icon = "myicon"
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                str = sResponseFromServer;

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                str = ex.Message;
            }
            return str;
        }
        public string GetDeviceDetails([FromBody]MobileDeviceInput Input)
        {
            MobileDeviceOutput mdo = new MobileDeviceOutput();
            List<MobileDeviceDetails> MobileDeviceclass = new List<MobileDeviceDetails>();
            DataTable dt_DeviceToken = new DataTable();
            dt_DeviceToken = getdata(string.Format("select RegisteredToken,MobileDeviceId,Status from MobileDeviceDetails where MobileDeviceId='{0}' and Status={1} ", Input.mobileDeviceId, Input.status));
            string sJSONResponse = "";

            try
            {
                MobileDeviceDetails MobileDevice = new MobileDeviceDetails { registeredToken = dt_DeviceToken.Rows[0]["RegisteredToken"].ToString() };
                MobileDeviceclass.Add(MobileDevice);

                mdo.status = "success";
                mdo.value = MobileDeviceclass;
                sJSONResponse = JsonConvert.SerializeObject(mdo);
            }
            catch (Exception ec)
            {
                mdo.status = "fail";

                sJSONResponse = JsonConvert.SerializeObject(mdo);
            }

            return sJSONResponse;
        }
        public Object DeviceDetails([FromBody]MobileDeviceInput DeviceInput)
        {
            ArrayList arl_MobileExist = new ArrayList();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string AppType_Query = "";

            MobileDeviceplanOutput mdpop = new MobileDeviceplanOutput();
            try
            {
                cnn.Open();

                if (arl_MobileExist[0].ToString() == "0")
                {
                    AppType_Query = "insert into MobileDeviceDetails(RegisteredToken,MobileDeviceId,Status,IsDeleted,IsActive) values('" + DeviceInput.registeredToken + "','" + DeviceInput.mobileDeviceId + "','" + DeviceInput.status + "',0,1) ";
                }
                else if(arl_MobileExist[0].ToString() == "1" && arl_MobileExist[0].ToString() == "0")
                {
                    AppType_Query = "update Users set MobileDeviceId='"+DeviceInput.mobileDeviceId+"' where MobileNo='"+DeviceInput.mobileNo+"'";
                }


                SqlCommand tm_cmd = new SqlCommand(AppType_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                mdpop.status = "success";

            }
            catch (Exception ex)
            {
                mdpop.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(mdpop);

            return sJSONResponse;
        }
        public ArrayList GetMobileExist(string MobileNo, string MobileDeviceId)
        {
            ArrayList arl_userexist = new ArrayList();
            DataTable dt_GetMobileExist = new DataTable();
            dt_GetMobileExist = getdata(string.Format("select count(*) as count from Users where MobileDeviceId='{0}' ", MobileDeviceId));
            arl_userexist.Add(dt_GetMobileExist.Rows[0][0].ToString());
           
            return arl_userexist;
        }
        public Object EditDeviceDetails([FromBody]MobileDeviceInput DeviceInput)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string AppType_Query = "";

            MobileDeviceplanOutput mdpop = new MobileDeviceplanOutput();
            try
            {
                cnn.Open();
                AppType_Query = "update MobileDeviceDetails set Status=" + DeviceInput.status + " where MobileDeviceId='" + DeviceInput.mobileDeviceId + "' ";
                SqlCommand tm_cmd = new SqlCommand(AppType_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                mdpop.status = "success";

            }
            catch (Exception ex)
            {
                mdpop.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(mdpop);

            return sJSONResponse;
        }
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

    }
}