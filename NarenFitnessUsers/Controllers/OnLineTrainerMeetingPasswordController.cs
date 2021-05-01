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
using NarenFitnessUsers.Models.Login;
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
    public class OnLineTrainerMeetingPasswordController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object OnLineTrainerMeetingPost([FromBody]OnLineTrainerMeetingPassword MeetingPassword)
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
                olPackage_Query = "insert into OnLineTrainerMeetingPassword(ZoomId,ZoomUserName,ZoomPassword,ZoomUrl,GoogleId,GoogleUserName,GooglePassword,GoogleUrl,TrainerCode,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + MeetingPassword.ZoomId + "','" + MeetingPassword.ZoomUserName + "','" + MeetingPassword.GooglePassword + "','" + MeetingPassword.ZoomUrl + "','" + MeetingPassword.GoogleId + "','" + MeetingPassword.GoogleUserName + "','" + MeetingPassword.GooglePassword + "','" + MeetingPassword.GoogleUrl + "','" + MeetingPassword.TrainerCode + "','" + MeetingPassword.CreatedBy + "','" + ServerDateTime + "',0,1) SELECT @@IDENTITY;";
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
        public Object OnLineTrainerMeetingUpdate([FromBody]OnLineTrainerMeetingPassword MeetingPassword)
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
                    command.CommandText = " update OnLineTrainerMeetingPassword set ZoomId='" + MeetingPassword.ZoomId + "',ZoomPassword='" + MeetingPassword.ZoomPassword + "',ZoomUserName='" + MeetingPassword.ZoomUserName + "',ZoomUrl='" + MeetingPassword.ZoomUrl + "',GoogleId='" + MeetingPassword.GoogleId + "',GooglePassword='" + MeetingPassword.GooglePassword + "',GoogleUserName='" + MeetingPassword.GoogleUserName + "',GoogleUrl='" + MeetingPassword.GoogleUrl + "'   where ID='" + MeetingPassword.Id + "'";
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
        public Object OnLineTrainerMeetingDelete([FromBody]OnLineTrainerMeetingPassword MeetingPassword)
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
                    command.CommandText = "delete from OnLineTrainerMeetingPassword where ID='" + MeetingPassword.Id + "' ";
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
        public Object GetOnLineTrainerMeeting([FromBody]OnLineTrainerMeetingPassword da)
        {
            OnLineTrainerMeetingPasswordOutput daOP = new OnLineTrainerMeetingPasswordOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Sessions = new DataTable();

            List<OnLineTrainerMeetingPasswordResponse> dalots = new List<OnLineTrainerMeetingPasswordResponse>();

            try
            {
                dt_Sessions = getdata(string.Format("select OTMP.ID,ZoomId,ZoomUserName,ZoomPassword,ZoomUrl,GoogleId,GoogleUserName,GooglePassword,GoogleUrl,OTMP.TrainerCode,HRE.EmployeeName as TrainerName from OnLineTrainerMeetingPassword OTMP,HREmployee HRE where OTMP.TrainerCode=HRE.EmployeeCode and OTMP.IsActive=1 and OTMP.IsDeleted=0 and OTMP.TrainerCode='{0}'",da.TrainerCode));

                for (int i = 0; i < dt_Sessions.Rows.Count; i++)
                {
                    OnLineTrainerMeetingPasswordResponse SlotsDetails = new OnLineTrainerMeetingPasswordResponse { id = Convert.ToInt32(dt_Sessions.Rows[i]["ID"].ToString()), zoomId = dt_Sessions.Rows[i]["ZoomId"].ToString(), zoomUserName = dt_Sessions.Rows[i]["ZoomUserName"].ToString(), zoomPassword = dt_Sessions.Rows[i]["ZoomPassword"].ToString(), zoomUrl = dt_Sessions.Rows[i]["ZoomUrl"].ToString(), googleId = dt_Sessions.Rows[i]["GoogleId"].ToString(), googleUserName = dt_Sessions.Rows[i]["GoogleUserName"].ToString(), googlePassword = dt_Sessions.Rows[i]["GooglePassword"].ToString(), googleUrl = dt_Sessions.Rows[i]["GoogleUrl"].ToString(), trainerCode = dt_Sessions.Rows[i]["TrainerCode"].ToString(), trainerName = dt_Sessions.Rows[i]["TrainerName"].ToString() };
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