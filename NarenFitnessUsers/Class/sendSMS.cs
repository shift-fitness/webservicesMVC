using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Net;
using System.Web.Configuration;
using System.Collections.Specialized;

namespace NarenFitnessUsers.Class
{
    public class sendSMS
    {
        public string SendSMS(string MobileNo, string SendText)
        {

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            string result = "";
            string httpUrl = "";

            try
            {


                StreamReader objReader;
                httpUrl = "http://sms.smscity.in/httpapi/httpapi?token=8c84bb208c7c7b7b72660ef51a5430b0&sender=NRNGYM&number=0" + MobileNo + "&route=2&type=1&sms='" + SendText + "' ";
                System.Net.WebRequest webRequest = System.Net.WebRequest.Create(httpUrl);
                Stream objstream;
                objstream = webRequest.GetResponse().GetResponseStream();
                objReader = new StreamReader(objstream);


            }
            catch (Exception ex)
            {

            }
            finally
            {

            }
            return result;

        }
    }
}