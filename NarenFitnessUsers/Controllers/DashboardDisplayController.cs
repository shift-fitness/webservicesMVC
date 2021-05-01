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
    public class DashboardDisplayController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object DashboardDisplayPost([FromBody]DashboardDisplay dd)
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
                olPackage_Query = "insert into DashboardDisplay(DisplayName,Comments,CreatedBy,CreatedOn,IsDeleted,IsActive,ScreenId) values('" + dd.displayName + "','" + dd.comments + "','" + dd.createdBy + "','" + ServerDateTime + "',0,1,"+dd.screenId+") SELECT @@IDENTITY;";
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
        public Object DashboardDisplayUpdate([FromBody]DashboardDisplay dd)
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
                    command.CommandText = "update DashboardDisplay set DisplayName='" + dd.displayName + "',ScreenId="+dd.screenId+" where ID='" + dd.Id + "'";
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

            string sJSONResponse = "Sucess";
            return sJSONResponse;
        }
        public Object DashboardDisplayDelete([FromBody]DashboardDisplay dd)
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
                    command.CommandText = "delete from DashboardDisplay where Id='" + dd.Id + "' ";
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
        public Object GetDashboardDisplay([FromBody]DashboardDisplay da)
        {
            DashboardDisplayOutput daOP = new DashboardDisplayOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Sessions = new DataTable();

            List<DashboardDisplayResponse> dalots = new List<DashboardDisplayResponse>();

            try
            {
                dt_Sessions = getdata(string.Format("select DD.ID,DD.ScreenId,DisplayName,Comments,D.ScreenName from DashboardDisplay DD,Dashboard D where DD.ScreenId=D.ScreenId and DD.IsDeleted=0 and DD.IsActive=1 order by Id asc", ""));

                for (int i = 0; i < dt_Sessions.Rows.Count; i++)
                {
                    DashboardDisplayResponse SlotsDetails = new DashboardDisplayResponse { id = dt_Sessions.Rows[i]["ID"].ToString(), displayName = dt_Sessions.Rows[i]["DisplayName"].ToString(), comments = dt_Sessions.Rows[i]["Comments"].ToString(), screenId = dt_Sessions.Rows[i]["ScreenId"].ToString(), screenName = dt_Sessions.Rows[i]["ScreenName"].ToString() };
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