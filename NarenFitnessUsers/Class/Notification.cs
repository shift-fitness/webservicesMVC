using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Net;
using System.Web.Script.Serialization;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace NarenFitnessUsers.Class
{
    public class Notification
    {

        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        public string PushToMobile(string MobileDeviceId, string Message)
        {
            string res = string.Empty;
            try
            {
                string RegisteredToken = GetMobileDeviceId(MobileDeviceId);

                string GoogleAppID = "AAAAZr2c76I:APA91bECB3lqhLqbA2USAvapk4aU5z-jv6Wh5AjjHWAE8KnBjsV7MewC1LpermptoOgkk9MdeGf1IYJzKweyItn32s_UKeMok-0UoCCvcwapcxPG1As460N7SI_M99GMsCcFQWELOCD9";
                var SENDER_ID = "213791179584";

                System.Net.WebRequest tRequest;
                tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

                string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message="
                                    + Message + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" +
                                      RegisteredToken + "";

                Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse tResponse = tRequest.GetResponse();
                dataStream = tResponse.GetResponseStream();
                StreamReader tReader = new StreamReader(dataStream);
                String sResponseFromServer = tReader.ReadToEnd();
                res = sResponseFromServer;
                tReader.Close();
                dataStream.Close();
                tResponse.Close();
                return res;
            }
            catch (Exception ex)
            {
                string value = ex.Message;

                return value;
            }

        }
        public string GetMobileDeviceId(string MobiledeviceId)
        {
            string MobileDeviceId = string.Empty;

            try
            {
                string query = "select RegisteredToken from MobileDeviceDetails where MobileDeviceId='"+ MobiledeviceId + "'";
                using (SqlCommand cmd_ps = new SqlCommand(query, cnn))
                {
                    cnn.Open();
                    SqlDataReader dr = cmd_ps.ExecuteReader();
                    if (dr.Read())
                    {
                        MobileDeviceId = dr["RegisteredToken"].ToString();
                    }

                    cnn.Close();
                }
            }
            catch (Exception ec)
            {

            }
            return MobileDeviceId;

        }




    }
}