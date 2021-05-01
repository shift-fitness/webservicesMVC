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
using NarenFitnessUsers.Models.Dashboard;
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
    public class OnlineSlotTimingsController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object OnlineSlotTimingsPost([FromBody]OnlineSlotTimings ost)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";
            PaymentDetailsPost Pdetails = new PaymentDetailsPost();

            try
            {
                cnn.Open();
                olPackage_Query = "insert into OnlineSlotTimings(SlotCode,SlotName,SessionCode,SlotStartTime,SlotEndTime,Description,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + ost.slotCode + "','" + ost.slotName + "','" + ost.sessionCode + "','" + ost.slotStartTime + "','" + ost.slotEndTime + "','" + ost.description + "','" + ost.createdBy + "','" + ServerDateTime + "',0,1) SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
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
        public Object OnlineSlotTimingsUpdate([FromBody]OnlineSlotTimings ost)
        {
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
                    command.CommandText = "update OnlineSlotTimings set SlotCode='" + ost.slotCode + "',SlotName='" + ost.slotName + "',SlotStartTime='" + ost.slotStartTime + "',SlotEndTime='" + ost.slotEndTime + "' where ID=" + ost.id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
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

            }

            string sJSONResponse = "";
            return sJSONResponse;
        }
        public Object OnlineSlotTimingsDelete([FromBody]OnlineSlotTimings ost)
        {
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
                    command.CommandText = "delete from OnlineSlotTimings where Id='" + ost.id + "' ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
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

            }

            string sJSONResponse = "";
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
        public Object GetOnlineSlotTimings([FromBody]OnlineSlotTimings ost)
        {
            OnlineSlotTimingsOutput daOP = new OnlineSlotTimingsOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Sessions = new DataTable();

            List<OnlineSlotTimingsResponse> dalots = new List<OnlineSlotTimingsResponse>();

            try
            {
                dt_Sessions = getdata(string.Format("select OST.ID,OST.SlotCode,OST.SlotName,OST.SlotStartTime,OST.SlotEndTime,OST.IsActive from OnlineSlotTimings OST,CMSSESSIONTIMESETTING CMSSS where OST.SessionCode=CMSSS.SessionCode and OST.IsDeleted=0 and CMSSS.SessionCode='{0}' ",ost.sessionCode));

                for (int i = 0; i < dt_Sessions.Rows.Count; i++)
                {
                    OnlineSlotTimingsResponse SlotsDetails = new OnlineSlotTimingsResponse { id = Convert.ToInt32( dt_Sessions.Rows[i]["ID"].ToString()), slotCode = dt_Sessions.Rows[i]["SlotCode"].ToString(), slotName = dt_Sessions.Rows[i]["SlotName"].ToString() , slotStartTime = dt_Sessions.Rows[i]["SlotStartTime"].ToString(), slotEndTime = dt_Sessions.Rows[i]["SlotEndTime"].ToString(), isActive =Convert.ToBoolean( dt_Sessions.Rows[i]["IsActive"].ToString()) };
                    dalots.Add(SlotsDetails);
                }

                daOP.status = "success";
                daOP.value = dalots;
                sJSONResponse = JsonConvert.SerializeObject(daOP);


            }
            catch (Exception ec)
            {
                daOP.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(daOP);
            }


            return sJSONResponse;
        }




    }
}