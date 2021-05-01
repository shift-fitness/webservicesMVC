using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft;
using Newtonsoft.Json;
using NarenFitnessUsers.Models.C2C;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;
using System.Web.Script.Serialization;


namespace NarenFitnessUsers.Controllers
{
    public class C2CmakecallController : Controller
    {
        JavaScriptSerializer objSerialiser = new JavaScriptSerializer();
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
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
        public Object C2CCall([FromBody]c2cinput c2ci)
        {


            c2coutput mdo = new c2coutput();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string c2ccall_Query = "";

            DataTable dt_C2CcallDetails = getdata(string.Format("select SRNo,CallerId,AgentNo from C2CAPI_Registration where EmployeeCode='{0}' and IsActive=1", c2ci.EmployeeCode));

            string Authorization = "1652dcb2-7e50-4644-b441-38d832e90dc0";
            var apikey = "lF4vZUSwA8Jab0ABWsITtxwM1ZwL6h2jZDdCTX30";

            //C2CData csda = new C2CData();
            //csda.k_number = c2ci.k_number;
            //csda.agent_number = c2ci.agent_number;
            //csda.customer_number = c2ci.CustomerNumber;
            //csda.caller_id = c2ci.caller_id;

            C2CData csda = new C2CData();
            csda.k_number = dt_C2CcallDetails.Rows[0]["SRNo"].ToString();
            csda.agent_number = dt_C2CcallDetails.Rows[0]["AgentNo"].ToString();
            csda.customer_number = c2ci.CustomerNumber;
            csda.caller_id = dt_C2CcallDetails.Rows[0]["CallerId"].ToString();


            string strResult = string.Empty;

            string url = "https://kpi.knowlarity.com/Basic/v1/account/call/makecall";
            ASCIIEncoding encoding = new ASCIIEncoding();
            var serializedObject = Newtonsoft.Json.JsonConvert.SerializeObject(csda);

            byte[] byteArray = Encoding.UTF8.GetBytes(serializedObject);
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "POST";
            webrequest.ContentType = "application/json";
            webrequest.ContentLength = byteArray.Length;
            webrequest.Headers["Authorization"] = Authorization;
            webrequest.Headers["x-api-key"] = apikey;
            // get stream data out of webrequest object
            Stream newStream = webrequest.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();
            HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
            Encoding enc = System.Text.Encoding.UTF8;
            StreamReader loResponseStream = new StreamReader(webresponse.GetResponseStream(), enc);

            strResult = loResponseStream.ReadToEnd();
            loResponseStream.Close();
            webresponse.Close();

            DataTable dtResult = new DataTable();

            string result = strResult.Remove(0, 11);
            int maxlmt = result.Count();
            string result2 = result.Remove(maxlmt - 1);
            string[] result3 = result2.Split(',');

            ArrayList arl = new ArrayList();

            foreach (string item in result3)
            {
                arl.Add(item);
            }

            string message = "";
            string call_id = "";


            try
            {
                message = arl[1].ToString().Replace("message", "");
                message = message.Replace(":", "");
                message = message.Replace(@"\\", "");
                call_id = arl[2].ToString().Replace("call_id", "");
                call_id = call_id.Replace(":", "");
                call_id = call_id.Replace("\\", "");
            }
            catch (Exception ec)
            {
                message = arl[0].ToString().Replace("message", "");
                message = message.Replace(":", "");
                message = message.Replace(@"\\", "");
               
            }



            try
            {
                cnn.Open();
                c2ccall_Query = "insert into C2CAPI_makecall(SRNo,CallerId,AgentNo,EmployeeCode,callid,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + c2ci.k_number + "','" + c2ci.caller_id + "','" + c2ci.agent_number + "','" + c2ci.EmployeeCode + "','" + c2ci.CustomerNumber + "','" + c2ci.CreatedBy + "','" + ServerDateTime + "',0,1)";
                SqlCommand tm_cmd = new SqlCommand(c2ccall_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteNonQuery());
                mdo.status = "Success";
                mdo.value = GetOutputMsg(message, call_id);
            }
            catch (Exception ex)
            {
                mdo.status = "fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(mdo);

            return sJSONResponse;
        }
        public List<C2Coutputmsg> GetOutputMsg(string message, string call_id)
        {

            List<C2Coutputmsg> outmsg = new List<C2Coutputmsg>();


            outmsg.Add(new C2Coutputmsg
            {
                call_id = call_id
                      ,
                message = message

            });




            return outmsg;

        }
        public Object C2CCallPost([FromBody]c2cinput c2ci)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string c2c_Query = "";
            c2coutput c2copt = new c2coutput();

            try
            {

                cnn.Open();
                c2c_Query = "insert into C2CAPI_Registration(SRNo,CallerId,AgentNo,EmployeeCode,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + c2ci.k_number + "','" + c2ci.caller_id + "','" + c2ci.agent_number + "','" + c2ci.EmployeeCode + "','" + c2ci.CreatedBy + "','" + ServerDateTime + "',0,1) SELECT @@IDENTITY;";
                SqlCommand c2c_cmd = new SqlCommand(c2c_Query, cnn);
                a = Convert.ToInt32(c2c_cmd.ExecuteScalar());
                c2copt.status = "Success";

            }
            catch (Exception ex)
            {
                c2copt.status = "fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(c2copt);

            return sJSONResponse;
        }
        public Object C2CCallUpdate([FromBody]c2cinput c2ci)
        {
            c2coutput c2copt = new c2coutput();
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            try
            {
                cnn.Close();
                cnn.Open();
                SqlTransaction transaction;
                transaction = cnn.BeginTransaction("SampleTransaction");
                command.Connection = cnn;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = "update C2CAPI_Registration set SRNo='" + c2ci.k_number + "',CallerId='" + c2ci.caller_id + "',AgentNo='" + c2ci.agent_number + "'  where EmployeeCode='" + c2ci.EmployeeCode + "' ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    c2copt.status = "Success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {

                }

            }
            catch (Exception ec)
            {
                c2copt.status = "fail";
            }
            string sJSONResponse = JsonConvert.SerializeObject(c2copt);

            return sJSONResponse;
        }
        public Object C2CCallDelete([FromBody]c2cinput c2ci)
        {
            c2coutput c2copt = new c2coutput();
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            try
            {

                cnn.Close();
                cnn.Open();
                SqlTransaction transaction;
                transaction = cnn.BeginTransaction("SampleTransaction");
                command.Connection = cnn;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = "delete from C2CAPI_Registration where EmployeeCode='" + c2ci.EmployeeCode + "' ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    c2copt.status = "Success";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {

                }

            }
            catch (Exception ec)
            {
                c2copt.status = "fail";
            }

            string sJSONResponse = JsonConvert.SerializeObject(c2copt);

            return sJSONResponse;
        }
        public Object GetC2CCall([FromBody]c2cinput c2ci)
        {
            c2coutputlist c2copt = new c2coutputlist();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Sessions = new DataTable();

            List<C2CData> c2cdata = new List<C2CData>();

            try
            {
                dt_Sessions = getdata(string.Format("select SRNo,CallerId,AgentNo from C2CAPI_Registration where EmployeeCode='{0}' ", c2ci.EmployeeCode));

                for (int i = 0; i < dt_Sessions.Rows.Count; i++)
                {
                    C2CData c2cData = new C2CData { k_number = dt_Sessions.Rows[i]["SRNo"].ToString(), caller_id = dt_Sessions.Rows[i]["CallerId"].ToString(), agent_number = dt_Sessions.Rows[i]["AgentNo"].ToString() };
                    c2cdata.Add(c2cData);
                }

                c2copt.status = "success";
                c2copt.value = c2cdata;
                sJSONResponse = JsonConvert.SerializeObject(c2copt);


            }
            catch (Exception ec)
            {
                c2copt.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(c2copt);
            }


            return sJSONResponse;
        }



    }
}