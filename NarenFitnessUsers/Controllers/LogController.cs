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
using NarenFitnessUsers.Models.Log;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;


namespace NarenFitnessUsers.Controllers
{
    public class LogController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object LogData([FromBody]LogFile log)
        {
            LogOutput lop = new LogOutput();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";


            try
            {
                cnn.Open();
                olPackage_Query = "insert into AgentCallTrack(CallUUid,CallData,CallTime,AgentList,AgentNumber,CallTransferStatus,CallRecordingUrl,ConversationDuration,TotalCallDuration,CreatedBy,CreatedOn,IsDeleted,IsActive) values('"+log.CallUUid+"','"+log.CallData+"','"+log.CallTime+"','"+log.AgentList+"','"+log.AgentNumber+"','"+log.CallTransferStatus+"','"+log.CallRecordingUrl+"','"+log.ConversationDuration+"','"+log.TotalCallDuration+"','"+log.CreatedBy+"','"+ServerDateTime+"',0,1)";
                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteNonQuery());
                lop.status = "Success";
                lop.value = "";
            }
            catch (Exception ex)
            {
                lop.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(lop);

            return sJSONResponse;
        }


    }
}