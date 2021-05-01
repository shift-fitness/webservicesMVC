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
using NarenFitnessUsers.Models.Applications;
using NarenFitnessUsers.Models.Slots.Slot;

using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections;

namespace NarenFitnessUsers.Controllers
{
    public class SlotsController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        JsonClass json = new JsonClass();
        // GET: ApplicationTypes
        public int GetOnlinePackageUsed(string MobileNo)
        {
            string query1 = "select count(*) as count from OnlinePackageUsed where  MobileNo='" + MobileNo + "' ";

            int PackageUsedCount = 0;
            cnn.Close();
            cnn.Open();
            SqlCommand cmdExist = new SqlCommand(query1, cnn);
            SqlDataReader dr = cmdExist.ExecuteReader();
            if (dr.Read())
            {
                PackageUsedCount = Convert.ToInt32(dr[0].ToString());
            }


            cnn.Close();


            return PackageUsedCount;
        }

        // GetAllFreeTrialSlots where condition = isactiveActive 
        public Object GetAllSlots([FromBody]SlotsRequest SlotsReq)
        {
            AllSlotsFinalOutPut ASFOP = new AllSlotsFinalOutPut();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";
            string TrainerID = "";
            DataTable dt_Sessions;

            List<GetSessionSlots> Slots = new List<GetSessionSlots>();
            int PackageUsedCount = GetOnlinePackageUsed(SlotsReq.MobileNo);
            try
            {
                if (SlotsReq.EnquiryType == 0 && PackageUsedCount == 0 || SlotsReq.EnquiryType == 2 && PackageUsedCount == 0)
                {
                    dt_Sessions = getdata(string.Format("select distinct CMSSS.SessionCode,CMSSS.SessionName,convert(varchar(10),CMSSS.SessionStartTime, 108) as SessionStartTime,convert(varchar(10),  CMSSS.SessionEndTime, 108) as SessionEndTime  from OnlineSlotTimings olst, CMSSESSIONTIMESETTING CMSSS where olst.SessionCode = CMSSS.SessionCode order by SessionCode asc", ""));

                    for (int i = 0; i < dt_Sessions.Rows.Count; i++)
                    {
                        GetSessionSlots SlotsDetails = new GetSessionSlots { sessionId = dt_Sessions.Rows[i]["SessionCode"].ToString(), sessionName = dt_Sessions.Rows[i]["SessionName"].ToString(), sessionStartTime = dt_Sessions.Rows[i]["SessionStartTime"].ToString(), sessionEndTime = dt_Sessions.Rows[i]["SessionEndTime"].ToString(), slots = GetSlotsDetails(dt_Sessions.Rows[i]["SessionCode"].ToString(), SlotsReq.EnquiryType, SlotsReq.MobileNo, SlotsReq.Date, "", 0) };
                        Slots.Add(SlotsDetails);
                    }
                }
                else if (SlotsReq.EnquiryType == 2 || SlotsReq.EnquiryType == 3)
                {
                    if (PackageUsedCount > 0)
                    {
                        // dt_Sessions = getdata(string.Format("select distinct CMSSS.SessionCode, CMSSS.SessionName,CMSSS.SessionName,convert(varchar(10),CMSSS.SessionStartTime, 108) as SessionStartTime,convert(varchar(10),  CMSSS.SessionEndTime, 108) as SessionEndTime,OLPU.TrainerID from OnlineSlotTimings olst, CMSSESSIONTIMESETTING CMSSS,OnlinePackageUsed  OLPU where olst.SessionCode = CMSSS.SessionCode and OLPU.MobileNo='{0}' order by SessionCode asc", SlotsReq.MobileNo));
                        dt_Sessions = getdata(string.Format("select distinct CMSSS.SessionCode,CMSSS.SessionName,convert(varchar(10),CMSSS.SessionStartTime, 108) as SessionStartTime,convert(varchar(10),  CMSSS.SessionEndTime, 108) as SessionEndTime,OLPU.TrainerID from OnlineSlotTimings olst, CMSSESSIONTIMESETTING CMSSS,OnlinePackageUsed  OLPU where olst.SessionCode = CMSSS.SessionCode and OLPU.MobileNo='{0}' and OLPU.TrainerID=(select Top 1 TrainerID from OnlinePackageUsed where MobileNo='{0}' order by SessionDate desc)  order by SessionCode asc", SlotsReq.MobileNo));

                        if (SlotsReq.TrainerId == null)
                        {
                            TrainerID = dt_Sessions.Rows[0]["TrainerID"].ToString();
                        }
                        else
                        {
                            TrainerID = SlotsReq.TrainerId;
                        }
                        for (int i = 0; i < dt_Sessions.Rows.Count; i++)
                        {
                            GetSessionSlots SlotsDetails = new GetSessionSlots { sessionId = dt_Sessions.Rows[i]["SessionCode"].ToString(), sessionName = dt_Sessions.Rows[i]["SessionName"].ToString(), sessionStartTime = dt_Sessions.Rows[i]["SessionStartTime"].ToString(), sessionEndTime = dt_Sessions.Rows[i]["SessionEndTime"].ToString(), slots = GetSlotsDetails(dt_Sessions.Rows[i]["SessionCode"].ToString(), SlotsReq.EnquiryType, SlotsReq.MobileNo, SlotsReq.Date, TrainerID, 1) };
                            Slots.Add(SlotsDetails);
                        }

                    }
                    else
                    {
                        ASFOP.status = "fail";
                        sJSONResponse = JsonConvert.SerializeObject(ASFOP);
                    }

                }
                else
                {
                }

                ASFOP.status = "success";
                ASFOP.value = Slots;
                sJSONResponse = JsonConvert.SerializeObject(ASFOP);


            }
            catch (Exception ec)
            {
                ASFOP.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(ASFOP);
            }


            return sJSONResponse;
        }
        private DataTable ReturnException(string Status, string Message)
        {
            DataTable dtMsg = new DataTable("MSG");
            dtMsg.Columns.Add("status");
            dtMsg.Columns.Add("value");
            DataRow drMsg = dtMsg.NewRow();
            drMsg["status"] = Status;
            drMsg["value"] = Message;
            dtMsg.Rows.Add(drMsg);
            return dtMsg;
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
        public List<AllSlots> GetSlotsDetails(string SessionCode, int EnquiryType, string MobileNo, DateTime Date, string Trainer, int Count)
        {
            List<AllSlots> slots = new List<AllSlots>();
            DataTable dt = new DataTable();
            DateTime date1 = DateTime.Today;

            var dateNow = DateTime.Now.ToString("HH:mm:ss");
            var time1 = TimeSpan.Parse(dateNow);
            var dateTime1 = DateTime.Today.Add(time1);

            var SlotTime = "";

            if (EnquiryType == 2 && Count == 0)
            {
                if (Date == date1)
                {

                    //Line21
                    dt = SlotAvailable(Date, EnquiryType, Count);
                    //dt = SlotAvailableOld(Date);

                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        SlotTime = dt.Rows[i]["SlotStartTime"].ToString();
                        var time2 = TimeSpan.Parse(SlotTime);
                        var dateTime2 = DateTime.Today.Add(time2).AddMinutes(-45);

                        if (dateTime1 > dateTime2)
                        {
                            try { dt.Rows[i].SetField("IsAvailable", 0); } catch (Exception ec) { }
                            dt.AcceptChanges();
                        }
                    }
                }
                else if (Date > date1)
                {
                    dt = SlotAvailable(Date, EnquiryType, Count);
                }
                else
                {

                }
            }
            else if (EnquiryType == 2 || EnquiryType == 3 && Count > 0)
            {
                if (Date == date1)
                {
                    dt = TrainersSlotAvailable(Trainer, Date, EnquiryType, Count);
                    //dt = SlotAvailableOld(Date);

                    for (int i = 0; i <= dt.Rows.Count - 1; i++)
                    {
                        SlotTime = dt.Rows[i]["SlotStartTime"].ToString();
                        var time2 = TimeSpan.Parse(SlotTime);
                        var dateTime2 = DateTime.Today.Add(time2).AddMinutes(-45);

                        if (dateTime1 > dateTime2)
                        {
                            try { dt.Rows[i].SetField("IsAvailable", 0); } catch (Exception ec) { }
                            dt.AcceptChanges();
                        }
                    }
                }
                else if (Date > date1)
                {
                    dt = TrainersSlotAvailable(Trainer, Date, EnquiryType, Count);
                }
                else
                {

                }
            }



            DataRow[] results = dt.Select("SessionCode = '" + SessionCode + "' ");
            dt = results.CopyToDataTable();

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    slots.Add(new AllSlots
                    {

                        slotID = Convert.ToString(dt.Rows[i]["SlotCode"])
                                               ,
                        slotName = Convert.ToString(dt.Rows[i]["SlotName"])
                                ,
                        slotStartTime = Convert.ToString(dt.Rows[i]["SlotStartTime"])
                          ,
                        slotEndTime = Convert.ToString(dt.Rows[i]["SlotEndTime"])
                        ,
                        isAvailable = Convert.ToBoolean(dt.Rows[i]["IsAvailable"])
                    });

                }
            }
            else
            {
                dt = null;

            }




            return slots;
        }
        public DataTable AbsentTrainer(string SlotDate)
        {
            DataSet ds_AbsTrainers = new DataSet();
            try
            {
                string query1 = "select distinct CreatedBy from TSMLeaveApplication where '" + SlotDate + "' between LeaveAppliedFromDate and LeaveAppliedToDate and StatusID=1";

                SqlDataAdapter da = new SqlDataAdapter(query1, cnn);

                da.Fill(ds_AbsTrainers);

            }
            catch (Exception ec)
            {

            }

            return ds_AbsTrainers.Tables[0];
        }
        public DataTable GetAllAvailableSlots()
        {
            DataTable dt = getdata(string.Format("SELECT  SlotCode,CASE WHEN AvailableSlots > 0 THEN   'true'  ELSE 'false'  END as  AvailableSlots  FROM FinalAvailableSlots Order by  case isnumeric(SlotCode) when 1 then cast(SlotCode as int)  else 999999999999999 end,SlotCode"));
            return dt;
        }
        public Object CancelSlot([FromBody]SlotCancelRequest Users)
        {
            SlotCancelResponse scr = new SlotCancelResponse();
            DataSet ds_CancelSlots = new DataSet();
            string sJSONResponse = "";
            DataSet ds_custdet = new DataSet();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            try
            {


                cnn.Open();
                SqlCommand command = cnn.CreateCommand();
                SqlTransaction transaction;
                transaction = cnn.BeginTransaction("SampleTransaction");
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "update OnlinePackageUsed set IsActive=0,IsDeleted=1 where ID='" + Users.ID + "' ";
                    command.ExecuteNonQuery();

                    transaction.Commit();

                    scr.status = "success";
                    scr.value = "";



                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    scr.status = "fail";
                    scr.value = "";
                }
                finally
                {

                }
            }
            catch (Exception ec)
            {
                scr.status = "fail";
                scr.value = "";
            }

            sJSONResponse = JsonConvert.SerializeObject(scr);

            return sJSONResponse;

        }

        public Object OffLineCancelSlot([FromBody]SlotCancelRequest Users)
        {
            SlotCancelResponse scr = new SlotCancelResponse();
            DataSet ds_CancelSlots = new DataSet();
            string sJSONResponse = "";
            DataSet ds_custdet = new DataSet();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            try
            {


                cnn.Open();
                SqlCommand command = cnn.CreateCommand();
                SqlTransaction transaction;
                transaction = cnn.BeginTransaction("SampleTransaction");
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                  

                    command.CommandText = "update OffLineSlotUsed set IsActive=0,IsDeleted=1 where ID='" + Users.ID + "' ";
                    command.ExecuteNonQuery();
                    command.CommandText = "delete from CCRMMEnquireStatus where ID='" + Users.EnquiryId + "' ";
                    command.ExecuteNonQuery();

                    transaction.Commit();

                    scr.status = "success";
                    scr.value = "";



                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    scr.status = "fail";
                    scr.value = "";
                }
                finally
                {

                }
            }
            catch (Exception ec)
            {
                scr.status = "fail";
                scr.value = "";
            }

            sJSONResponse = JsonConvert.SerializeObject(scr);

            return sJSONResponse;

        }
        public DataTable Absents(string SlotDate)
        {
            DataSet ds_AbsTrainers = new DataSet();
            try
            {
                string query1 = "select TrainerCode,LeaveDate from TSMLeaveApplicationView where StatusID=1";

                SqlDataAdapter da = new SqlDataAdapter(query1, cnn);

                da.Fill(ds_AbsTrainers);

            }
            catch (Exception ec)
            {

            }

            return ds_AbsTrainers.Tables[0];
        }
        public DataTable SlotAvailableOld(DateTime Date)
        {

            DataTable dt_SlotAvailableAB = new DataTable();
            DataTable dt_SlotAvailableCD = new DataTable();
            int TrainerCount = MaxTrainerCount();

            dt_SlotAvailableAB = SlotAvailableAB();
            dt_SlotAvailableCD = SlotAvailableCD(Date);


            string a = "";
            string b = "";

            int SlotCount = 0;
            try
            {
                if (dt_SlotAvailableCD.Rows.Count == 0)
                {
                    for (int i = 0; i <= dt_SlotAvailableAB.Rows.Count - 1; i++)
                    {
                        SlotCount = Convert.ToInt32(dt_SlotAvailableAB.Rows[i]["IsAvailable"].ToString());
                        if (SlotCount < 0)
                        {
                            SlotCount = 0;
                            try { dt_SlotAvailableAB.Rows[i].SetField("IsAvailable", SlotCount); } catch (Exception ec) { }
                        }
                        else if (SlotCount == TrainerCount)
                        {
                            SlotCount = 0;
                            try { dt_SlotAvailableAB.Rows[i].SetField("IsAvailable", SlotCount); } catch (Exception ec) { }
                        }
                        else
                        {
                            SlotCount = 1;
                            try { dt_SlotAvailableAB.Rows[i].SetField("IsAvailable", SlotCount); } catch (Exception ec) { }
                        }

                        dt_SlotAvailableAB.AcceptChanges();
                    }
                }
                else
                {
                    for (int j = 0; j <= dt_SlotAvailableCD.Rows.Count - 1; j++)
                    {
                        for (int i = 0; i <= dt_SlotAvailableAB.Rows.Count - 1; i++)
                        {

                            a = dt_SlotAvailableCD.Rows[j]["SlotCode"].ToString();
                            b = dt_SlotAvailableAB.Rows[i]["SlotCode"].ToString();

                            if (dt_SlotAvailableAB.Rows[i]["SlotCode"].ToString() == dt_SlotAvailableCD.Rows[j]["SlotCode"].ToString())
                            {
                                SlotCount = Convert.ToInt32(dt_SlotAvailableAB.Rows[i]["IsAvailable"].ToString()) - Convert.ToInt32(dt_SlotAvailableCD.Rows[j]["IsAvailable"].ToString());
                                if (SlotCount < 0)
                                    SlotCount = 0;
                                else
                                    SlotCount = 1;
                                try { dt_SlotAvailableAB.Rows[i].SetField("SlotCount", SlotCount); } catch (Exception ec) { }

                            }
                            dt_SlotAvailableAB.AcceptChanges();

                        }
                    }
                }
            }
            catch (Exception ec)
            {

            }

            return dt_SlotAvailableAB;
        }
        public DataTable SlotAvailableAB()
        {
            DataSet ds_SlotAvailableAB = new DataSet();
            try
            {
                string query1 = "select OLST.SessionCode,SAAB.SlotCode,OLST.SlotName,convert(varchar(10), OLST.SlotStartTime, 108) as SlotStartTime,convert(varchar(10), OLST.SlotEndTime, 108) as SlotEndTime,SAAB.SlotCount as IsAvailable from SlotAvailablityAB SAAB,OnlineSlotTimings OLST where SAAB.SlotCode=OLST.SlotCode";
                SqlDataAdapter da = new SqlDataAdapter(query1, cnn);
                da.Fill(ds_SlotAvailableAB);

            }
            catch (Exception ec)
            {

            }

            return ds_SlotAvailableAB.Tables[0];
        }
        public DataTable SlotAvailableCD(DateTime SlotDate)
        {
            string Date = SlotDate.ToString("MM/dd/yyyy");
            DataSet ds_SlotAvailableCD = new DataSet();
            try
            {
                string query1 = " select OLST.SessionCode,OLST.SlotCode,OLST.SlotName,convert(varchar(10), OLST.SlotStartTime, 108) as SlotStartTime,convert(varchar(10), OLST.SlotEndTime, 108) as SlotEndTime,SACD.SlotCount as IsAvailable from SlotAvailablityCD SACD,OnlineSlotTimings OLST where SACD.SlotCode=OLST.SlotCode and  Date between '" + Date + "' and '" + Date + " 23:59:59'";
                SqlDataAdapter da = new SqlDataAdapter(query1, cnn);
                da.Fill(ds_SlotAvailableCD);

            }
            catch (Exception ec)
            {

            }

            return ds_SlotAvailableCD.Tables[0];
        }
        public int MaxTrainerCount()
        {
            DataSet ds_MaxTrainerCount = new DataSet();
            try
            {
                string query1 = "select distinct  TrainerCode from OnlineTrainersSlotMapping";
                SqlDataAdapter da = new SqlDataAdapter(query1, cnn);
                da.Fill(ds_MaxTrainerCount);

            }
            catch (Exception ec)
            {

            }

            return ds_MaxTrainerCount.Tables[0].Rows.Count;
        }

        //
        public Object EditSlotOnline([FromBody]SlotEditRequest Users)
        {
            SlotCancelResponse scr = new SlotCancelResponse();
            DataSet ds_CancelSlots = new DataSet();
            string sJSONResponse = "";
            DataSet ds_custdet = new DataSet();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            try
            {


                cnn.Open();
                SqlCommand command = cnn.CreateCommand();
                SqlTransaction transaction;
                transaction = cnn.BeginTransaction("SampleTransaction");
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "update OnlinePackageUsed set SlotID='" + Users.SlotId + "' where ID='" + Users.Id + "' ";
                    command.ExecuteNonQuery();

                    transaction.Commit();

                    scr.status = "success";
                    scr.value = "";



                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    scr.status = "fail";
                    scr.value = "";
                }
                finally
                {

                }
            }
            catch (Exception ec)
            {
                scr.status = "fail";
                scr.value = "";
            }

            sJSONResponse = JsonConvert.SerializeObject(scr);

            return sJSONResponse;

        }
        public Object EditSlotOffline([FromBody]SlotEditRequest Users)
        {
            SlotCancelResponse scr = new SlotCancelResponse();
            DataSet ds_CancelSlots = new DataSet();
            string sJSONResponse = "";
            DataSet ds_custdet = new DataSet();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            try
            {


                cnn.Open();
                SqlCommand command = cnn.CreateCommand();
                SqlTransaction transaction;
                transaction = cnn.BeginTransaction("SampleTransaction");
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "update OnlinePackageUsed set SlotID='" + Users.SlotId + "' where ID='" + Users.Id + "' ";
                    command.ExecuteNonQuery();
                    transaction.Commit();

                    scr.status = "success";
                    scr.value = "";



                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    scr.status = "fail";
                    scr.value = "";
                }
                finally
                {

                }
            }
            catch (Exception ec)
            {
                scr.status = "fail";
                scr.value = "";
            }

            sJSONResponse = JsonConvert.SerializeObject(scr);

            return sJSONResponse;

        }

        //

        public Object ChangeSlottimings([FromBody]SlotEditRequest Users)
        {
            SlotCancelResponse scr = new SlotCancelResponse();
            DataSet ds_CancelSlots = new DataSet();
            string sJSONResponse = "";
            DataSet ds_custdet = new DataSet();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            try
            {


                cnn.Open();
                SqlCommand command = cnn.CreateCommand();
                SqlTransaction transaction;
                transaction = cnn.BeginTransaction("SampleTransaction");
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "insert into ChangeSlotsDetails(MembershipCode,InvoiceId,SlotCode,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + Users.MembershipCode + "','" + Users.InvoiceId + "','" + Users.SlotId + "','" + Users.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "update CCRMMembership set SlotCode='" + Users.SlotId + "'  where MembershipCode='" + Users.MembershipCode + "' and InvoiceID='" + Users.InvoiceId + "' ";
                    command.ExecuteNonQuery();
                    transaction.Commit();

                    scr.status = "success";
                    scr.value = "";



                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    scr.status = "fail";
                    scr.value = "";
                }
                finally
                {

                }
            }
            catch (Exception ec)
            {
                scr.status = "fail";
                scr.value = "";
            }

            sJSONResponse = JsonConvert.SerializeObject(scr);

            return sJSONResponse;

        }

        // SlotAvailability new
        // select distinct TrainerCode from  OnlineTrainersSlotMapping
        // select distinct SlotCode from OnlineSlotTimings
        // select TrainerCode, LeaveDate from TSMLeaveApplicationView where TrainerCode = '140903' and StatusID = 1 and LeaveDate between '07/29/2020' and '07/29/2020 23:59:59'
        // select TrainerID, SlotID, SessionDate from TrainerSlotsUsedCountTESTING where TrainerID = '140903'  and SessionDate between '07/28/2020' and '07/28/2020 23:59:59'

        public DataTable TrainersAvailable()
        {
            DataSet ds_SlotAvailableAB = new DataSet();
            try
            {
                string query1 = "select distinct TrainerCode from  OnlineTrainersSlotMapping where IsDeleted=0";
                SqlDataAdapter da = new SqlDataAdapter(query1, cnn);
                da.Fill(ds_SlotAvailableAB);

            }
            catch (Exception ec)
            {

            }

            return ds_SlotAvailableAB.Tables[0];
        }
        public DataTable AllSlots()
        {
            DataSet ds_SlotAvailableAB = new DataSet();
            try
            {
                string query1 = "select  SlotCode from OnlineSlotTimings  order by cast(SlotCode as int) asc";
                SqlDataAdapter da = new SqlDataAdapter(query1, cnn);
                da.Fill(ds_SlotAvailableAB);

            }
            catch (Exception ec)
            {

            }

            return ds_SlotAvailableAB.Tables[0];
        }
        public DataTable OnlineSlotTimings()
        {
            DataSet ds_DBOnlineSlotTimings = new DataSet();
            DataTable dt_OnlineSlotTimings = new DataTable();

            try
            {
                string query1 = "select SessionCode,SlotCode,SlotName,convert(varchar(10), SlotStartTime, 108) as SlotStartTime,convert(varchar(10), SlotEndTime, 108) as SlotEndTime from OnlineSlotTimings";
                SqlDataAdapter da = new SqlDataAdapter(query1, cnn);
                da.Fill(ds_DBOnlineSlotTimings);

                DataRow row;
                DataColumn col1 = new DataColumn("SessionCode", typeof(string));
                DataColumn col2 = new DataColumn("SlotCode", typeof(int));
                DataColumn col3 = new DataColumn("SlotName", typeof(string));
                DataColumn col4 = new DataColumn("SlotStartTime", typeof(string));
                DataColumn col5 = new DataColumn("SlotEndTime", typeof(string));
                DataColumn col6 = new DataColumn("IsAvailable", typeof(int));

                dt_OnlineSlotTimings.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6 });

                try
                {

                    for (int i = 0; i <= ds_DBOnlineSlotTimings.Tables[0].Rows.Count - 1; i++)
                    {
                        row = dt_OnlineSlotTimings.NewRow();
                        row["SessionCode"] = ds_DBOnlineSlotTimings.Tables[0].Rows[i]["SessionCode"].ToString();
                        row["SlotCode"] = ds_DBOnlineSlotTimings.Tables[0].Rows[i]["SlotCode"].ToString();
                        row["SlotName"] = ds_DBOnlineSlotTimings.Tables[0].Rows[i]["SlotName"].ToString();
                        row["SlotStartTime"] = ds_DBOnlineSlotTimings.Tables[0].Rows[i]["SlotStartTime"].ToString();
                        row["SlotEndTime"] = ds_DBOnlineSlotTimings.Tables[0].Rows[i]["SlotEndTime"].ToString();
                        dt_OnlineSlotTimings.Rows.Add(row);
                    }
                }
                catch (Exception ec)
                {

                }
            }
            catch (Exception ec)
            {

            }
            return dt_OnlineSlotTimings;
        }
        public DataTable TrainerLeaves(string TrainerCode, DateTime Date)
        {
            DataSet ds_SlotAvailableAB = new DataSet();
            try
            {

                string query1 = "select TrainerCode, LeaveDate from TSMLeaveApplicationView where TrainerCode = '" + TrainerCode + "' and StatusID = 1 and LeaveDate between '" + Date.ToString("MM/dd/yyyy") + "' and '" + Date.ToString("MM/dd/yyyy") + " 23:59:59'";
                SqlDataAdapter da = new SqlDataAdapter(query1, cnn);
                da.Fill(ds_SlotAvailableAB);

            }
            catch (Exception ec)
            {

            }

            return ds_SlotAvailableAB.Tables[0];
        }
        public DataTable SlotsBooked(string TrainerCode, DateTime Date)
        {
            DataSet ds_SlotAvailableAB = new DataSet();
            try
            {
                string query1 = "select TrainerID, SlotID as SlotCode, SessionDate from TrainerSlotsUsedCountTESTING where TrainerID = '" + TrainerCode + "'  and SessionDate between '" + Date.ToString("MM/dd/yyyy") + "' and '" + Date.ToString("MM/dd/yyyy") + " 23:59:59'";
                SqlDataAdapter da = new SqlDataAdapter(query1, cnn);
                da.Fill(ds_SlotAvailableAB);

            }
            catch (Exception ec)
            {

            }

            return ds_SlotAvailableAB.Tables[0];
        }
        public DataTable InActiveSlots(string TrainerCode, DateTime Date, int EnquiryType, int Count)
        {
            DataSet ds_SlotAvailableAB = new DataSet();
            string query1 = "";
            try
            {
                // old query
                // query1 = "select SlotCode from OnlineTrainersSlotMapping where TrainerCode = '" + TrainerCode + "'  and IsActive=0  and  SlotCode not in(select SlotID from TrainerSlotsUsedCountTESTING where TrainerID = '" + TrainerCode + "'  and SessionDate between '" + Date.ToString("MM/dd/yyyy") + "' and '" + Date.ToString("MM/dd/yyyy") + " 23:59:59')";

                if (EnquiryType == 2 && Count == 0)
                {
                    query1 = "select SlotCode from OnlineTrainersSlotMapping where TrainerCode = '" + TrainerCode + "'  and FreeTrialIsActive=0  and  SlotCode not in(select SlotID from TrainerSlotsUsedCountTESTING where TrainerID = '" + TrainerCode + "'  and SessionDate between '" + Date.ToString("MM/dd/yyyy") + "' and '" + Date.ToString("MM/dd/yyyy") + " 23:59:59')";
                }
                else
                {
                    query1 = "select SlotCode from OnlineTrainersSlotMapping where TrainerCode = '" + TrainerCode + "'  and IsActive=0  and  SlotCode not in(select SlotID from TrainerSlotsUsedCountTESTING where TrainerID = '" + TrainerCode + "'  and SessionDate between '" + Date.ToString("MM/dd/yyyy") + "' and '" + Date.ToString("MM/dd/yyyy") + " 23:59:59')";
                }
                SqlDataAdapter da = new SqlDataAdapter(query1, cnn);
                da.Fill(ds_SlotAvailableAB);

            }
            catch (Exception ec)
            {

            }

            return ds_SlotAvailableAB.Tables[0];
        }
        public int IsDeleteTrainers(string TrainerCode)
        {
            int Count = 0;

            try
            {

                string query1 = "select  count(*) as DeleteCount from OnlineTrainersSlotMapping where TrainerCode='" + TrainerCode + "' and IsDeleted=1";
                SqlCommand cmd = new SqlCommand(query1, cnn);
                cnn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    Count = Convert.ToInt32(dr[0].ToString());
                }

            }
            catch (Exception ec)
            {

            }
            finally
            {
                cnn.Close();
            }

            return Count;
        }
        public DataTable SlotAvailable(DateTime Date, int EnquiryType, int Count)
        {
            DataTable dt_SlotAvailable = new DataTable();

            DataRow row;
            DataColumn col1 = new DataColumn("SlotCode", typeof(int));
            DataColumn col2 = new DataColumn("Count", typeof(int));
            DataColumn col3 = new DataColumn("IsAvailable", typeof(string));

            dt_SlotAvailable.Columns.AddRange(new DataColumn[] { col1, col2, col3 });

            DataTable dt_TrainersAvailable = new DataTable();
            DataTable dt_AllSlots = new DataTable();
            DataTable dt_TrainerLeaves = new DataTable();
            DataTable dt_SlotsBooked = new DataTable();
            DataTable dt_OnlineSlotTimings = new DataTable();
            DataTable dt_InActiveSlots = new DataTable();

            dt_OnlineSlotTimings = OnlineSlotTimings();

            dt_TrainersAvailable = TrainersAvailable();
            dt_AllSlots = AllSlots();

            int SlotCount = 0;

            int TrainerCount = dt_TrainersAvailable.Rows.Count;
            int TrainerLoopCount = 0;

            try
            {
                for (int i = 0; i <= dt_TrainersAvailable.Rows.Count - 1; i++)
                {

                    TrainerLoopCount = i + 1;
                    dt_TrainerLeaves = TrainerLeaves(dt_TrainersAvailable.Rows[i]["TrainerCode"].ToString(), Date);
                    dt_SlotsBooked = SlotsBooked(dt_TrainersAvailable.Rows[i]["TrainerCode"].ToString(), Date);
                    int IsDelete = IsDeleteTrainers(dt_TrainersAvailable.Rows[i]["TrainerCode"].ToString());
                    // Leave Check 

                    if (dt_TrainerLeaves.Rows.Count > 0)
                    {
                        if (dt_SlotAvailable.Rows.Count > 0)
                        {
                            for (int j = 0; j <= dt_AllSlots.Rows.Count - 1; j++)
                            {
                                try
                                {
                                    string value = dt_AllSlots.Rows[j]["SlotCode"].ToString();
                                    var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_AllSlots.Rows[j]["SlotCode"].ToString()));
                                    DataRow dr1 = dr[0];

                                    SlotCount = Convert.ToInt32(dr1[1].ToString()) + 1;
                                    try { dt_SlotAvailable.Rows[j].SetField("Count", SlotCount); } catch (Exception ec) { }
                                }
                                catch (Exception ec) { }
                            }
                        }
                        else if (dt_SlotAvailable.Rows.Count == 0)
                        {
                            for (int j = 0; j <= dt_AllSlots.Rows.Count - 1; j++)
                            {

                                try
                                {
                                    row = dt_SlotAvailable.NewRow();
                                    row["SlotCode"] = dt_AllSlots.Rows[j]["SlotCode"].ToString();
                                    row["Count"] = 1;
                                    row["IsAvailable"] = "";
                                    dt_SlotAvailable.Rows.Add(row);
                                }
                                catch (Exception ec) { }
                            }
                        }

                    }
                    else if (IsDelete > 0)
                    {

                        string Trainer = dt_TrainersAvailable.Rows[i]["TrainerCode"].ToString();
                        //if (dt_SlotAvailable.Rows.Count > 0)
                        //{
                        //    for (int j = 0; j <= dt_AllSlots.Rows.Count - 1; j++)
                        //    {
                        //        try
                        //        {
                        //            string value = dt_AllSlots.Rows[j]["SlotCode"].ToString();
                        //            var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_AllSlots.Rows[j]["SlotCode"].ToString()));
                        //            DataRow dr1 = dr[0];

                        //            SlotCount = Convert.ToInt32(dr1[1].ToString()) + 1;
                        //            try { dt_SlotAvailable.Rows[j].SetField("Count", SlotCount); } catch (Exception ec) { }
                        //        }
                        //        catch (Exception ec) { }
                        //    }
                        //}
                        //else if (dt_SlotAvailable.Rows.Count == 0)
                        //{
                        //    for (int j = 0; j <= dt_AllSlots.Rows.Count - 1; j++)
                        //    {

                        //        try
                        //        {
                        //            row = dt_SlotAvailable.NewRow();
                        //            row["SlotCode"] = dt_AllSlots.Rows[j]["SlotCode"].ToString();
                        //            row["Count"] = 1;
                        //            row["IsAvailable"] = "";
                        //            dt_SlotAvailable.Rows.Add(row);
                        //        }
                        //        catch (Exception ec) { }
                        //    }
                        //}

                    }
                    // User Booked Slots

                    else if (dt_SlotsBooked.Rows.Count > 0)
                    {

                        if (dt_SlotAvailable.Rows.Count == 0)
                        {
                            // 1 st Trainer with slots condition ...(need to develop) - 07/29/2020 1:30 PM
                            for (int j = 0; j <= dt_AllSlots.Rows.Count - 1; j++)
                            {

                                try
                                {
                                    row = dt_SlotAvailable.NewRow();
                                    row["SlotCode"] = dt_AllSlots.Rows[j]["SlotCode"].ToString();
                                    row["Count"] = 0;
                                    row["IsAvailable"] = "";
                                    dt_SlotAvailable.Rows.Add(row);
                                }
                                catch (Exception ec)
                                {

                                }
                            }
                        }




                        if (dt_SlotAvailable.Rows.Count > 0)
                        {
                            for (int k = 0; k <= dt_SlotsBooked.Rows.Count - 1; k++)
                            {

                                for (int j = 0; j <= dt_SlotAvailable.Rows.Count - 1; j++)
                                {
                                    string val = dt_SlotsBooked.Rows[k]["SlotCode"].ToString();
                                    string val1 = dt_SlotAvailable.Rows[j]["SlotCode"].ToString();

                                    if (dt_SlotsBooked.Rows[k]["SlotCode"].ToString() == dt_SlotAvailable.Rows[j]["SlotCode"].ToString())
                                    {

                                        try
                                        {
                                            string value = dt_SlotAvailable.Rows[j]["SlotCode"].ToString();
                                            var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[j]["SlotCode"].ToString()));
                                            DataRow dr1 = dr[0];

                                            SlotCount = Convert.ToInt32(dr1[1].ToString()) + 1;
                                            try { dt_SlotAvailable.Rows[j].SetField("Count", SlotCount); } catch (Exception ec) { }

                                        }
                                        catch (Exception ec) { }
                                    }
                                    else
                                    {

                                    }

                                }
                            }


                            // Filter  + 3  and  -3 slots coding
                            // need to check with maximum limit
                            for (int b = 0; b <= dt_SlotsBooked.Rows.Count - 1; b++)
                            {
                                for (int a = 0; a <= dt_SlotAvailable.Rows.Count - 1; a++)
                                {
                                    string vala = dt_SlotsBooked.Rows[b]["SlotCode"].ToString();
                                    string valb = dt_SlotAvailable.Rows[a]["SlotCode"].ToString();

                                    if (dt_SlotsBooked.Rows[b]["SlotCode"].ToString() == dt_SlotAvailable.Rows[a]["SlotCode"].ToString())
                                    {

                                        try
                                        {
                                            var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[a - 3]["SlotCode"].ToString()));
                                            DataRow dr1 = dr[0];


                                            SlotCount = Convert.ToInt32(dr1[1].ToString());

                                            if (SlotCount < TrainerLoopCount)
                                                SlotCount = SlotCount + 1;

                                            dt_SlotAvailable.Rows[a - 3].SetField("Count", SlotCount);
                                        }
                                        catch (Exception ec) { }


                                        try
                                        {
                                            var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[a - 2]["SlotCode"].ToString()));
                                            DataRow dr1 = dr[0];

                                            SlotCount = Convert.ToInt32(dr1[1].ToString());

                                            if (SlotCount < TrainerLoopCount)
                                                SlotCount = SlotCount + 1;

                                            dt_SlotAvailable.Rows[a - 2].SetField("Count", SlotCount);
                                        }
                                        catch (Exception ec) { }


                                        try
                                        {
                                            var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[a - 1]["SlotCode"].ToString()));
                                            DataRow dr1 = dr[0];

                                            SlotCount = Convert.ToInt32(dr1[1].ToString());

                                            if (SlotCount < TrainerLoopCount)
                                                SlotCount = SlotCount + 1;

                                            dt_SlotAvailable.Rows[a - 1].SetField("Count", SlotCount);
                                        }
                                        catch (Exception ec) { }


                                        try
                                        {
                                            var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[a + 1]["SlotCode"].ToString()));
                                            DataRow dr1 = dr[0];

                                            SlotCount = Convert.ToInt32(dr1[1].ToString());

                                            if (SlotCount < TrainerLoopCount)
                                                SlotCount = SlotCount + 1;

                                            dt_SlotAvailable.Rows[a + 1].SetField("Count", SlotCount);
                                            try { if (dt_SlotAvailable.Rows[a + 2].RowError != null) { } else { a = a + 1; } }
                                            catch (Exception ec) { }
                                        }
                                        catch (Exception ec) { }


                                        try
                                        {
                                            var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[a + 2]["SlotCode"].ToString()));
                                            DataRow dr1 = dr[0];

                                            SlotCount = Convert.ToInt32(dr1[1].ToString());

                                            if (SlotCount < TrainerLoopCount)
                                                SlotCount = SlotCount + 1;

                                            dt_SlotAvailable.Rows[a + 2].SetField("Count", SlotCount);
                                            try { if (dt_SlotAvailable.Rows[a + 3].RowError != null) { } else { a = a + 2; } } catch (Exception ec) { }
                                        }
                                        catch (Exception ec) { }


                                        try
                                        {

                                            var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[a + 3]["SlotCode"].ToString()));
                                            DataRow dr1 = dr[0];

                                            SlotCount = Convert.ToInt32(dr1[1].ToString());

                                            if (SlotCount < TrainerLoopCount)
                                                SlotCount = SlotCount + 1;

                                            dt_SlotAvailable.Rows[a + 3].SetField("Count", SlotCount);
                                            try
                                            {
                                                string value = dt_SlotAvailable.Rows[a + 4]["Count"].ToString();
                                                if (dt_SlotAvailable.Rows[a + 4]["Count"].ToString() == "1")
                                                {
                                                    a = a + 3;
                                                }
                                                else if (dt_SlotAvailable.Rows[a + 4].RowError != null)
                                                {
                                                    a = a + 3;
                                                }
                                            }
                                            catch (Exception ec)
                                            {
                                            }
                                        }
                                        catch (Exception ec) { }
                                    }
                                }
                                dt_SlotAvailable.AcceptChanges();
                            }
                        }


                        // Remove All InActive Slots

                        dt_InActiveSlots = InActiveSlots(dt_TrainersAvailable.Rows[i][0].ToString(), Date, EnquiryType, Count);


                        for (int p = 0; p <= dt_InActiveSlots.Rows.Count - 1; p++)
                        {
                            for (int q = 0; q <= dt_SlotAvailable.Rows.Count - 1; q++)
                            {
                                string vala = dt_InActiveSlots.Rows[p]["SlotCode"].ToString();
                                string valb = dt_SlotAvailable.Rows[q]["SlotCode"].ToString();

                                if (dt_InActiveSlots.Rows[p]["SlotCode"].ToString() == dt_SlotAvailable.Rows[q]["SlotCode"].ToString())
                                {
                                    var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[q]["SlotCode"].ToString()));
                                    DataRow dr1 = dr[0];
                                    SlotCount = Convert.ToInt32(dr1[1].ToString());

                                    if (SlotCount < TrainerLoopCount)
                                        SlotCount = SlotCount + 1;


                                    dt_SlotAvailable.Rows[q].SetField("Count", SlotCount);
                                }
                            }

                        }
                    }
                    else
                    {
                        // When there is no Trainer Leave and no Slot Booked for all the Trainers on that day  , Default loop
                        if (dt_SlotAvailable.Rows.Count == 0)
                        {
                            for (int j = 0; j <= dt_AllSlots.Rows.Count - 1; j++)
                            {
                                try
                                {
                                    row = dt_SlotAvailable.NewRow();
                                    row["SlotCode"] = dt_AllSlots.Rows[j]["SlotCode"].ToString();
                                    row["Count"] = 0;
                                    row["IsAvailable"] = "";
                                    dt_SlotAvailable.Rows.Add(row);
                                }
                                catch (Exception ec)
                                {

                                }
                            }
                        }


                        dt_InActiveSlots = InActiveSlots(dt_TrainersAvailable.Rows[i][0].ToString(), Date, EnquiryType, Count);

                        // need to check with maximum limit
                        if (dt_InActiveSlots.Rows.Count > 0 && dt_TrainerLeaves.Rows.Count == 0 && dt_SlotsBooked.Rows.Count == 0)
                        {
                            for (int p = 0; p <= dt_InActiveSlots.Rows.Count - 1; p++)
                            {
                                for (int q = 0; q <= dt_SlotAvailable.Rows.Count - 1; q++)
                                {
                                    string vala = dt_InActiveSlots.Rows[p]["SlotCode"].ToString();
                                    string valb = dt_SlotAvailable.Rows[q]["SlotCode"].ToString();

                                    if (dt_InActiveSlots.Rows[p]["SlotCode"].ToString() == dt_SlotAvailable.Rows[q]["SlotCode"].ToString())
                                    {

                                        var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[q]["SlotCode"].ToString()));
                                        DataRow dr1 = dr[0];
                                        SlotCount = Convert.ToInt32(dr1[1].ToString());

                                        if (SlotCount < TrainerLoopCount)
                                            SlotCount = SlotCount + 1;

                                        dt_SlotAvailable.Rows[q].SetField("Count", SlotCount);
                                    }
                                }
                            }
                        }
                    }




                } // For loop of Trainer List ends

            }
            catch (Exception ec)
            {

            }

            // # Start of SV 10  Final True Falsse based on Trainer Max Count and Trainer Used count
            int AvalTrainerCount = 0;
            int IsAvailable = 0;
            for (int l = 0; l <= dt_OnlineSlotTimings.Rows.Count; l++)
            {

                try
                {
                    string value = dt_SlotAvailable.Rows[l]["SlotCode"].ToString();
                    var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[l]["SlotCode"].ToString()));
                    DataRow dr1 = dr[0];

                    AvalTrainerCount = Convert.ToInt32(dr1[1].ToString());
                    if (AvalTrainerCount >= TrainerCount)
                        IsAvailable = 0;
                    else if (AvalTrainerCount < TrainerCount)
                        IsAvailable = 1;

                    try { dt_OnlineSlotTimings.Rows[l].SetField("IsAvailable", IsAvailable); } catch (Exception ec) { }

                }
                catch (Exception ec)
                {

                }
            }
            // # End of SV 10


            return dt_OnlineSlotTimings;
        }
        public DataTable TrainersSlotAvailable(string TrainerCode, DateTime Date, int EnquiryType, int Count)
        {
            DataTable dt_SlotAvailable = new DataTable();

            DataRow row;
            DataColumn col1 = new DataColumn("SlotCode", typeof(int));
            DataColumn col2 = new DataColumn("Count", typeof(int));
            DataColumn col3 = new DataColumn("IsAvailable", typeof(string));

            dt_SlotAvailable.Columns.AddRange(new DataColumn[] { col1, col2, col3 });

            DataTable dt_TrainersAvailable = new DataTable();
            DataTable dt_AllSlots = new DataTable();
            DataTable dt_TrainerLeaves = new DataTable();
            DataTable dt_SlotsBooked = new DataTable();
            DataTable dt_OnlineSlotTimings = new DataTable();
            DataTable dt_InActiveSlots = new DataTable();

            dt_OnlineSlotTimings = OnlineSlotTimings();

            dt_TrainersAvailable = TrainersAvailable();
            dt_AllSlots = AllSlots();

            int SlotCount = 0;

            int TrainerCount = 1;

            try
            {

                dt_TrainerLeaves = TrainerLeaves(TrainerCode, Date);
                dt_SlotsBooked = SlotsBooked(TrainerCode, Date);
                int IsDelete = IsDeleteTrainers(TrainerCode);
                // Leave Check 

                if (dt_TrainerLeaves.Rows.Count > 0)
                {

                    for (int j = 0; j <= dt_AllSlots.Rows.Count - 1; j++)
                    {

                        try
                        {
                            row = dt_SlotAvailable.NewRow();
                            row["SlotCode"] = dt_AllSlots.Rows[j]["SlotCode"].ToString();
                            row["Count"] = 1;
                            row["IsAvailable"] = "";
                            dt_SlotAvailable.Rows.Add(row);
                        }
                        catch (Exception ec) { }
                    }
                }
                else if (IsDelete > 0)
                {

                    for (int j = 0; j <= dt_AllSlots.Rows.Count - 1; j++)
                    {

                        try
                        {
                            row = dt_SlotAvailable.NewRow();
                            row["SlotCode"] = dt_AllSlots.Rows[j]["SlotCode"].ToString();
                            row["Count"] = 1;
                            row["IsAvailable"] = "";
                            dt_SlotAvailable.Rows.Add(row);
                        }
                        catch (Exception ec) { }
                    }
                }
                // User Booked Slots

                else if (dt_SlotsBooked.Rows.Count > 0)
                {
                    for (int j = 0; j <= dt_AllSlots.Rows.Count - 1; j++)
                    {

                        try
                        {
                            row = dt_SlotAvailable.NewRow();
                            row["SlotCode"] = dt_AllSlots.Rows[j]["SlotCode"].ToString();
                            row["Count"] = 0;
                            row["IsAvailable"] = "";
                            dt_SlotAvailable.Rows.Add(row);
                        }
                        catch (Exception ec) { }
                    }

                    for (int k = 0; k <= dt_SlotsBooked.Rows.Count - 1; k++)
                    {

                        for (int j = 0; j <= dt_SlotAvailable.Rows.Count - 1; j++)
                        {
                            string val = dt_SlotsBooked.Rows[k]["SlotCode"].ToString();
                            string val1 = dt_SlotAvailable.Rows[j]["SlotCode"].ToString();

                            if (dt_SlotsBooked.Rows[k]["SlotCode"].ToString() == dt_SlotAvailable.Rows[j]["SlotCode"].ToString())
                            {

                                try
                                {
                                    string value = dt_SlotAvailable.Rows[j]["SlotCode"].ToString();
                                    var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[j]["SlotCode"].ToString()));
                                    DataRow dr1 = dr[0];

                                    SlotCount = Convert.ToInt32(dr1[1].ToString()) + 1;
                                    try { dt_SlotAvailable.Rows[j].SetField("Count", SlotCount); } catch (Exception ec) { }

                                }
                                catch (Exception ec) { }
                            }
                            else
                            {

                            }

                        }
                    }


                    // Filter  + 3  and  -3 slots coding


                    for (int a = 0; a <= dt_SlotAvailable.Rows.Count - 1; a++)
                    {
                        int starta = a;
                        int value = Convert.ToInt32(dt_SlotAvailable.Rows[a]["Count"].ToString());

                        if (dt_SlotAvailable.Rows[a]["Count"].ToString() == "1")
                        {

                            try
                            {
                                var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[a - 3]["SlotCode"].ToString()));
                                DataRow dr1 = dr[0];
                                SlotCount = Convert.ToInt32(dr1[1].ToString()) + 1;
                                dt_SlotAvailable.Rows[a - 3].SetField("Count", SlotCount);
                            }
                            catch (Exception ec) { }


                            try
                            {
                                var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[a - 2]["SlotCode"].ToString()));
                                DataRow dr1 = dr[0];

                                if (Convert.ToInt32(dr1[1].ToString()) == 0)
                                    SlotCount = Convert.ToInt32(dr1[1].ToString()) + 1;
                                else SlotCount = Convert.ToInt32(dr1[1].ToString());
                                dt_SlotAvailable.Rows[a - 2].SetField("Count", SlotCount);

                            }
                            catch (Exception ec) { }


                            try
                            {
                                var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[a - 1]["SlotCode"].ToString()));
                                DataRow dr1 = dr[0];
                                if (Convert.ToInt32(dr1[1].ToString()) == 0)
                                    SlotCount = Convert.ToInt32(dr1[1].ToString()) + 1;
                                else SlotCount = Convert.ToInt32(dr1[1].ToString());
                                dt_SlotAvailable.Rows[a - 1].SetField("Count", SlotCount);
                            }
                            catch (Exception ec) { }


                            try
                            {
                                var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[a + 1]["SlotCode"].ToString()));
                                DataRow dr1 = dr[0];
                                if (Convert.ToInt32(dr1[1].ToString()) == 0)
                                    SlotCount = Convert.ToInt32(dr1[1].ToString()) + 1;
                                else SlotCount = Convert.ToInt32(dr1[1].ToString());

                                dt_SlotAvailable.Rows[a + 1].SetField("Count", SlotCount);
                                try { if (dt_SlotAvailable.Rows[a + 2].RowError != null) { } else { a = a + 1; } }
                                catch (Exception ec) { }
                            }
                            catch (Exception ec) { }


                            try
                            {
                                var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[a + 2]["SlotCode"].ToString()));
                                DataRow dr1 = dr[0];
                                if (Convert.ToInt32(dr1[1].ToString()) == 0)
                                    SlotCount = Convert.ToInt32(dr1[1].ToString()) + 1;
                                else SlotCount = Convert.ToInt32(dr1[1].ToString());

                                dt_SlotAvailable.Rows[a + 2].SetField("Count", SlotCount);
                                try { if (dt_SlotAvailable.Rows[a + 3].RowError != null) { } else { a = a + 2; } } catch (Exception ec) { }
                            }
                            catch (Exception ec) { }



                            try
                            {

                                var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[a + 3]["SlotCode"].ToString()));
                                DataRow dr1 = dr[0];
                                SlotCount = Convert.ToInt32(dr1[1].ToString()) + 1;

                                dt_SlotAvailable.Rows[a + 3].SetField("Count", SlotCount);
                                try
                                {
                                    string value123 = dt_SlotAvailable.Rows[a + 4]["Count"].ToString();
                                    if (dt_SlotAvailable.Rows[a + 4]["Count"].ToString() == "1")
                                    {
                                        a = a + 3;
                                    }
                                    else if (dt_SlotAvailable.Rows[a + 4].RowError != null)
                                    {
                                        a = a + 3;
                                    }
                                }
                                catch (Exception ec)
                                {
                                }
                            }
                            catch (Exception ec)
                            { }


                            int enda = a;


                        }
                        dt_SlotAvailable.AcceptChanges();
                    }


                    // Remove All InActive Slots

                    dt_InActiveSlots = InActiveSlots(TrainerCode, Date, EnquiryType, Count);


                    for (int p = 0; p <= dt_InActiveSlots.Rows.Count - 1; p++)
                    {
                        for (int q = 0; q <= dt_SlotAvailable.Rows.Count - 1; q++)
                        {
                            string vala = dt_InActiveSlots.Rows[p]["SlotCode"].ToString();
                            string valb = dt_SlotAvailable.Rows[q]["SlotCode"].ToString();

                            if (dt_InActiveSlots.Rows[p]["SlotCode"].ToString() == dt_SlotAvailable.Rows[q]["SlotCode"].ToString())
                            {
                                var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[q]["SlotCode"].ToString()));
                                DataRow dr1 = dr[0];
                                SlotCount = Convert.ToInt32(dr1[1].ToString()) + 1;
                                dt_SlotAvailable.Rows[q].SetField("Count", SlotCount);
                            }
                        }

                    }
                }
                else
                {
                    // When there is no Trainer Leave and no Slot Booked for all the Trainers on that day  , Default loop
                    if (dt_SlotAvailable.Rows.Count == 0)
                    {
                        for (int j = 0; j <= dt_AllSlots.Rows.Count - 1; j++)
                        {
                            try
                            {
                                row = dt_SlotAvailable.NewRow();
                                row["SlotCode"] = dt_AllSlots.Rows[j]["SlotCode"].ToString();
                                row["Count"] = 0;
                                row["IsAvailable"] = "";
                                dt_SlotAvailable.Rows.Add(row);
                            }
                            catch (Exception ec)
                            {

                            }
                        }
                    }

                    dt_InActiveSlots = InActiveSlots(TrainerCode, Date, EnquiryType, Count);


                    for (int p = 0; p <= dt_InActiveSlots.Rows.Count - 1; p++)
                    {
                        for (int q = 0; q <= dt_SlotAvailable.Rows.Count - 1; q++)
                        {
                            string vala = dt_InActiveSlots.Rows[p]["SlotCode"].ToString();
                            string valb = dt_SlotAvailable.Rows[q]["SlotCode"].ToString();

                            if (dt_InActiveSlots.Rows[p]["SlotCode"].ToString() == dt_SlotAvailable.Rows[q]["SlotCode"].ToString())
                            {
                                var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[q]["SlotCode"].ToString()));
                                DataRow dr1 = dr[0];
                                SlotCount = Convert.ToInt32(dr1[1].ToString()) + 1;
                                dt_SlotAvailable.Rows[q].SetField("Count", SlotCount);
                            }
                        }

                    }






                }
            }
            catch (Exception ec)
            {

            }

            // # Start of SV 10  Final True Falsse based on Trainer Max Count and Trainer Used count
            int AvalTrainerCount = 0;

            int IsAvailable = 0;
            for (int l = 0; l <= dt_OnlineSlotTimings.Rows.Count; l++)
            {

                try
                {
                    string value = dt_SlotAvailable.Rows[l]["SlotCode"].ToString();
                    var dr = dt_SlotAvailable.Select("SlotCode = " + Convert.ToInt32(dt_SlotAvailable.Rows[l]["SlotCode"].ToString()));
                    DataRow dr1 = dr[0];

                    AvalTrainerCount = Convert.ToInt32(dr1[1].ToString());
                    if (AvalTrainerCount >= TrainerCount)
                        IsAvailable = 0;
                    else if (AvalTrainerCount < TrainerCount)
                        IsAvailable = 1;

                    try { dt_OnlineSlotTimings.Rows[l].SetField("IsAvailable", IsAvailable); } catch (Exception ec) { }

                }
                catch (Exception ec)
                {

                }
            }
            // # End of SV 10


            return dt_OnlineSlotTimings;
        }


    }
}