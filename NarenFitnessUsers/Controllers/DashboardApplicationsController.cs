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
    public class DashboardApplicationsController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object DashboardApplications([FromBody]DashboardApplications da)
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
                olPackage_Query = "insert into DashboardApplications(ApplicationName,Comments,CreatedBy,CreatedOn,IsDeleted,IsActive,ScreenId) values('" + da.applicationName + "','" + da.comments + "','" + da.createdBy + "','" + ServerDateTime + "',0,1,"+da.screenId+") SELECT @@IDENTITY;";
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
        public Object DashboardApplicationsUpdate([FromBody]DashboardApplications da)
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
                    command.CommandText = "update DashboardApplications set ApplicationName='" + da.applicationName + "',ScreenId="+da.screenId+" where ID=" + da.Id + " ";
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

            string sJSONResponse = "success";
            return sJSONResponse;
        }
        public Object DashboardApplicationsDelete([FromBody]DashboardApplications da)
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
                    command.CommandText = "delete from DashboardApplications where ID=" + da.Id + " ";
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

            string sJSONResponse = "success";
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
        public Object GetDashboardApplications([FromBody]DashboardApplications da)
        {
            DashboardApplicationOutput daOP = new DashboardApplicationOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Sessions=new DataTable();

            List<DashboardApplicationResponse> dalots = new List<DashboardApplicationResponse>();
          
            try
            {
                dt_Sessions = getdata(string.Format("select DA.ID,DA.ApplicationName,DA.Comments,DA.ScreenId,D.ScreenName from DashboardApplications DA,Dashboard D where DA.ScreenId=D.ScreenId and DA.IsActive=1 and DA.IsDeleted=0", ""));

                for (int i = 0; i < dt_Sessions.Rows.Count; i++)
                {
                    DashboardApplicationResponse SlotsDetails = new DashboardApplicationResponse { id = dt_Sessions.Rows[i]["ID"].ToString(), applicationName = dt_Sessions.Rows[i]["ApplicationName"].ToString(), comments = dt_Sessions.Rows[i]["Comments"].ToString(), screenId = dt_Sessions.Rows[i]["ScreenId"].ToString(), screenName = dt_Sessions.Rows[i]["ScreenName"].ToString() };
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
        public Object GetDashboardScreens([FromBody]DashboardScreens da)
        {
            DashboardScreenOutPut daOP = new DashboardScreenOutPut();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Sessions = new DataTable();

            List<DashboardScreenResponse> dalots = new List<DashboardScreenResponse>();

            try
            {
                dt_Sessions = getdata(string.Format("Select Id,ScreenId,ScreenName from Dashboard where IsActive=1 and IsDeleted=0", ""));

                for (int i = 0; i < dt_Sessions.Rows.Count; i++)
                {
                    DashboardScreenResponse SlotsDetails = new DashboardScreenResponse { id = dt_Sessions.Rows[i]["ID"].ToString(), screenId = dt_Sessions.Rows[i]["ScreenId"].ToString(), screenName = dt_Sessions.Rows[i]["ScreenName"].ToString() };
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

        public Object DashboardScreen([FromBody]DashboardScreens da)
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
                olPackage_Query = "insert into Dashboard(ScreenName,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + da.screenName + "','" + da.createdBy + "','" + ServerDateTime + "',0,1) SELECT @@IDENTITY;";
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

        public Object DashboardScreenUpdate([FromBody]DashboardScreens da)
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
                    command.CommandText = "update Dashboard set ScreenName='"+da.screenName+"',ModifiedBy='"+da.createdBy+"',ModifiedOn='"+ServerDateTime+"' where ScreenId='"+da.Id+"'";
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

            string sJSONResponse = "success";
            return sJSONResponse;
        }

        public Object DashboardScreenDelete([FromBody]DashboardScreens da)
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
                    command.CommandText = "delete from Dashboard where ID=" + da.Id + " ";
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

            string sJSONResponse = "success";
            return sJSONResponse;
        }

    }
}