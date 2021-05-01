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
    public class OnlineTrainersSlotMappingController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object OnlineTrainersSlotMappingPost([FromBody]OnlineTrainersSlotMapping mts)
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
                olPackage_Query = "insert into OnlineTrainersSlotMapping(BranchCode,TrainerCode,TrainerName,SessionCode,SessionName,SlotCode,SlotName,PackageCode,PackageName,Description,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + mts.branchCode + "','" + mts.trainerCode + "','" + mts.trainerName + "','" + mts.sessionCode + "','" + mts.sessionName + "','" + mts.slotCode + "','" + mts.slotName + "','" + mts.packageCode + "','" + mts.packageName + "','" + mts.description + "','" + mts.createdBy + "','" + ServerDateTime + "',0,1) SELECT @@IDENTITY;";
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
        public Object OnlineTrainersSlotMappingUpdate([FromBody]OnlineTrainersSlotMapping mts)
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
                    command.CommandText = "update OnlineTrainersSlotMapping set TrainerCode='" + mts.trainerCode + "',TrainerName='" + mts.trainerName + "',SessionCode='" + mts.sessionCode + "',SessionName='" + mts.sessionName + "',SlotCode='" + mts.slotCode + "',SlotName='" + mts.slotName + "',PackageCode='" + mts.packageCode + "',PackageName='" + mts.packageName + "' where ID=" + mts.Id + " ";
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
        public Object OnlineTrainersSlotMappingDelete([FromBody]OnlineTrainersSlotMapping mts)
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
                    command.CommandText = "delete from OnlineTrainersSlotMapping where Id='" + mts.Id + "' ";
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
        public Object GetOnlineTrainersSlotMapping([FromBody]OnlineTrainersSlotMapping mts)
        {
            OnlineTrainersSlotMappingOutput daOP = new OnlineTrainersSlotMappingOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Sessions = new DataTable();

            List<OnlineTrainersSlotMappingResponse> dalots = new List<OnlineTrainersSlotMappingResponse>();

            try
            {
                dt_Sessions = getdata(string.Format("select OTSM.ID,OTSM.PackageCode,OTSM.PackageName,OTSM.SessionCode,OTSM.SessionName,OTSM.SlotCode,OTSM.SlotName,OTSM.IsActive,OTSM.FreeTrialIsActive,OTSM.Description from OnlineTrainersSlotMapping OTSM where OTSM.IsDeleted=0 and OTSM.BranchCode='{0}' and OTSM.TrainerCode='{1}' and OTSM.SessionCode='{2}' ", mts.branchCode,mts.trainerCode,mts.sessionCode));

                for (int i = 0; i < dt_Sessions.Rows.Count; i++)
                {
                    OnlineTrainersSlotMappingResponse SlotsDetails = new OnlineTrainersSlotMappingResponse { id = Convert.ToInt32(dt_Sessions.Rows[i]["ID"].ToString()), packageCode = dt_Sessions.Rows[i]["PackageCode"].ToString(), packageName = dt_Sessions.Rows[i]["PackageName"].ToString(), sessionCode = dt_Sessions.Rows[i]["SessionCode"].ToString(), sessionName = dt_Sessions.Rows[i]["SessionName"].ToString(), slotCode = dt_Sessions.Rows[i]["SlotCode"].ToString(), slotName = dt_Sessions.Rows[i]["SlotName"].ToString(), isActive =Convert.ToBoolean( dt_Sessions.Rows[i]["IsActive"].ToString()), description = dt_Sessions.Rows[i]["Description"].ToString(), freeTrialIsActive = Convert.ToBoolean(dt_Sessions.Rows[i]["FreeTrialIsActive"].ToString()) };
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