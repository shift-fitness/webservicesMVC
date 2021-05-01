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
using NarenFitnessUsers.Models.FreezingFacility;

using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;


namespace NarenFitnessUsers.Controllers
{
    public class FreezingFacilityController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object FreezingFacilityPost([FromBody]FreezingFacilityInput ff)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";
            FreezingFacilityOutput ffopt = new FreezingFacilityOutput();

            try
            {
                cnn.Open();
                olPackage_Query = "insert into FreezingFacility(BranchCode,SMFMID,FreezingName,NoOfDays,FreezingStartDate,FreezingExpireDate,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + ff.BranchCode+"','"+ff.SMFMID+"','"+ff.FreezingName+"','"+ff.NoOfDays+"','"+ff.FreezingStartDate+"','"+ff.FreezingExpireDate+"','"+ff.CreatedBy+"','"+ServerDateTime+"',0,1) SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                ffopt.status = "Success";
                ffopt.value = a;
            }
            catch (Exception ex)
            {
                ffopt.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object FreezingFacilityUpdate([FromBody]FreezingFacilityInput ff)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            FreezingFacilityOutput ffopt = new FreezingFacilityOutput();
                                     
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
                    command.CommandText = "update FreezingFacility set SMFMID='"+ff.SMFMID+"',FreezingName='"+ff.FreezingName+"',NoOfDays='"+ff.NoOfDays+"',FreezingStartDate='"+ff.FreezingStartDate+"',FreezingExpireDate='"+ff.FreezingExpireDate+"'  where ID="+ff.ID+" ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {
                    ffopt.status = "fail";
                    ffopt.value = 1;
                }

            }
            catch (Exception ec)
            {
                ffopt.status = "Success";
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object FreezingFacilityDelete([FromBody]FreezingFacilityInput ff)
        {
            string sJSONResponse = "";
            FreezingFacilityOutput ffopt = new FreezingFacilityOutput();
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
                    command.CommandText = "delete from FreezingFacility where ID="+ff.ID+" ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ffopt.status = "fail";
                    ffopt.value = 1;
                }
                finally
                {
                    ffopt.status = "success";
                    ffopt.value = 1;
                }

            }
            catch (Exception ec)
            {

            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

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
        public Object GetFreezingFacility([FromBody]FreezingFacilityInput mts)
        {
            FreezingListOutput flop = new FreezingListOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Sessions = new DataTable();

            List<FreezingDetails> dalots = new List<FreezingDetails>();

            try
            {
                dt_Sessions = getdata(string.Format("select ID as FreezingID,SMFMID,FreezingName,NoOfDays,FreezingStartDate,FreezingExpireDate from FreezingFacility where BranchCode='{0}' ", mts.BranchCode));

                for (int i = 0; i < dt_Sessions.Rows.Count; i++)
                {
                    FreezingDetails SlotsDetails = new FreezingDetails { FreezingID = Convert.ToInt32(dt_Sessions.Rows[i]["FreezingID"].ToString()), SMFMID = Convert.ToInt32(dt_Sessions.Rows[i]["SMFMID"].ToString()), FreezingName = dt_Sessions.Rows[i]["FreezingName"].ToString(), NoOfDays = Convert.ToInt32(dt_Sessions.Rows[i]["NoOfDays"].ToString()), FreezingStartDate = Convert.ToDateTime(dt_Sessions.Rows[i]["FreezingStartDate"].ToString()), FreezingExpireDate = Convert.ToDateTime(dt_Sessions.Rows[i]["FreezingExpireDate"].ToString()) };
                    dalots.Add(SlotsDetails);
                }

                flop.status = "success";
                flop.FreezingDetails = dalots;
                sJSONResponse = JsonConvert.SerializeObject(flop);


            }
            catch (Exception ec)
            {
                flop.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(flop);
            }


            return sJSONResponse;
        }
    }
}