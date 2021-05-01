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
using NarenFitnessUsers.Models.Applications;
using NarenFitnessUsers.Models.Slots.SelectedSlots;
using NarenFitnessUsers.Models.Slots.SelectedSlotsMulti;
using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;
using System.Net.Mail;


namespace NarenFitnessUsers.Controllers
{
    public class SelectSlotController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        JsonClass json = new JsonClass();
        // GET: ApplicationTypes

        public int imgname()
        {
            Random random = new Random();
            string r = "";
            try
            {
                for (int i = 1; i < 4; i++)
                {
                    r += random.Next(1, 9).ToString();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return Convert.ToInt32(r);

        }
        public string GetPhotoUrl(string Base64String, int Id)
        {
            string urlform = "";
            string endpath = "";
            try
            {

                string base64string = Base64String;
                var bytes = Convert.FromBase64String(base64string);

                endpath = "Icons" + Id + ".png";
                //C:\inetpub\wwwroot\GYMUI\UsersImages
                string filepath = @"C:\inetpub\wwwroot\GYMUI\UsersImages\\" + endpath;
                using (var imageFile = new FileStream(filepath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }


                urlform = "http://202.143.96.72/GYMUI/UsersImages/" + endpath;
            }
            catch (Exception ec)
            {
                urlform = "";
            }

            return urlform;
        }
        public Object GetSelectedSlotsDetails([FromBody]SlotSelected SlotsSelected)
        {
            SelectedSlotFinalOutPut SSFOP = new SelectedSlotFinalOutPut();
            SelectSlotFinalOutPutMessage SSFOPM = new SelectSlotFinalOutPutMessage();

            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            try
            {
                List<Slots> Slots = new List<Slots>();
                Slots SlotsDetails = new Slots { trainerDetails = GetTrainerDetails(Convert.ToInt32(SlotsSelected.SelectedSlotId), SlotsSelected.TrainerId, SlotsSelected.Date), slotDetails = GetPackageDetails(SlotsSelected.MobileNo) };


                // Change Trainer Concept
                //Slots SlotsDetails = new Slots { trainerDetails = GetTrainerDetailsLatest(Convert.ToInt32(SlotsSelected.SelectedSlotId), SlotsSelected.TrainerId, SlotsSelected.Date, SlotsSelected.ChangeTrainer, SlotsSelected.MobileNo), slotDetails = GetPackageDetails(SlotsSelected.MobileNo) };


                Slots.Add(SlotsDetails);

                if (Slots[0].trainerDetails.Count == 0)
                {
                    SSFOPM.status = "fail";
                    SSFOPM.value = "Opps you have just missed this booking slot! Please try another slot";
                    sJSONResponse = JsonConvert.SerializeObject(SSFOPM);
                }
                else
                {
                    SSFOP.status = "success";
                    SSFOP.value = Slots;
                    sJSONResponse = JsonConvert.SerializeObject(SSFOP);
                }




            }
            catch (Exception ec)
            {
                SSFOPM.status = "fail";
                SSFOPM.value = "Internal Server Error";
                sJSONResponse = JsonConvert.SerializeObject(SSFOPM);

            }

            return sJSONResponse;
        }
        private DataTable ReturnException(string Status, string Message)
        {
            DataTable dtMsg = new DataTable("MSG");
            dtMsg.Columns.Add("STATUS");
            dtMsg.Columns.Add("VALUE");
            DataRow drMsg = dtMsg.NewRow();
            drMsg["STATUS"] = Status;
            drMsg["VALUE"] = Message;
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
        public List<TrainerDetails> GetTrainerDetails(int SlotId, string TrainerID, string Date)
        {
            List<TrainerDetails> Trainers = new List<TrainerDetails>();
            DataTable dt = new DataTable();
            DataTable st_absenttrainerlist = AbsentTrainer(Date);

            if (TrainerID == null)
            {
                dt = getdata(string.Format("select OLTS.TrainerCode,OLTS.TrainerName,HRE.PhotoURL from OnlineTrainersSlotMapping OLTS,HREmployee HRE  where HRE.EmployeeCode=OLTS.TrainerCode  and OLTS.IsActive=1 and OLTS.SlotCode='{0}'  and    OLTS.TrainerCode not in (  select TrainerID as TrainerCode from OnlinePackageUsed where  IsDeleted=0 and SessionDate between '" + Date + "' and '" + Date + " 23:59:59' and SlotID='{0}') ORDER  by NEWID() ", SlotId));


                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    for (int m = 0; m < st_absenttrainerlist.Rows.Count; m++)
                    {
                        try
                        {
                            if (dt.Rows[m]["TrainerCode"].ToString() == st_absenttrainerlist.Rows[j]["CreatedBy"].ToString())
                            {
                                dt.Rows.RemoveAt(m);
                            }
                        }
                        catch (Exception ec)
                        {

                        }
                    }
                    dt.AcceptChanges();
                }




                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {

                    if (SlotAvailabilityByTrainer(Convert.ToString(SlotId), Date, Convert.ToString(dt.Rows[i]["TrainerCode"])) == 0)
                    {
                        Trainers.Add(new TrainerDetails
                        {
                            trainerId = Convert.ToString(dt.Rows[i]["TrainerCode"])
                                                       ,
                            trainerName = Convert.ToString(dt.Rows[i]["TrainerName"])
                                                        ,
                            PhotoURL = Convert.ToString(dt.Rows[i]["PhotoURL"])
                        });

                        i = dt.Rows.Count - 1;

                    }
                }

            }
            else
            {
                if (SlotAvailabilityByTrainer(Convert.ToString(SlotId), Date, TrainerID) == 0)
                {
                    dt = getdata(string.Format("select OLTS.TrainerCode,OLTS.TrainerName,HRE.PhotoURL from OnlineTrainersSlotMapping OLTS,HREmployee HRE  where HRE.EmployeeCode=OLTS.TrainerCode  and OLTS.IsActive=1 and OLTS.SlotCode='{0}' and OLTS.TrainerCode='{1}'  and  OLTS.TrainerCode not in (select TrainerID as TrainerCode from OnlinePackageUsed where SessionDate between  '" + Date + "' and '" + Date + " 23:59:59' and SlotID='{0}' and IsDeleted=0) order by OLTS.ID asc ", SlotId, TrainerID));

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        for (int m = 0; m < st_absenttrainerlist.Rows.Count; m++)
                        {
                            if (dt.Rows[m]["TrainerCode"].ToString() == st_absenttrainerlist.Rows[j]["CreatedBy"].ToString())
                            {
                                dt.Rows.RemoveAt(m);
                            }
                        }
                        dt.AcceptChanges();
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Trainers.Add(new TrainerDetails
                        {
                            trainerId = Convert.ToString(dt.Rows[i]["TrainerCode"])
                            ,
                            trainerName = Convert.ToString(dt.Rows[i]["TrainerName"])
                             ,
                            PhotoURL = Convert.ToString(dt.Rows[i]["PhotoURL"])
                        });

                    }
                }
                else
                {


                }
            }



            return Trainers;
        }
        public List<TrainerDetails> GetTrainerDetailsLatest(int SlotId, string TrainerID, string Date, Boolean ChangeTrainer, string MobileNo)
        {
            List<TrainerDetails> Trainers = new List<TrainerDetails>();
            DataTable dt = new DataTable();
            DataTable st_absenttrainerlist = AbsentTrainer(Date);

            if (TrainerID == null && ChangeTrainer == false)
            {
                // Previous used trainer
                dt = getdata(string.Format("select Top 1 OLPU.TrainerID as TrainerCode,OLTS.TrainerName,HRE.PhotoURL from OnlinePackageUsed OLPU,OnlineTrainersSlotMapping OLTS,HREmployee HRE where HRE.EmployeeCode=OLTS.TrainerCode and OLTS.TrainerCode=OLPU.TrainerID and  OLPU.IsDeleted=0 and OLPU.MobileNo='" + MobileNo + "' order by OLPU.ID desc ", MobileNo));


            }
            else if (TrainerID == null && ChangeTrainer == true)
            {
                //Get trainerid by randon Query
                dt = getdata(string.Format("select Top 1 OLTS.TrainerCode,OLTS.TrainerName,HRE.PhotoURL from OnlineTrainersSlotMapping OLTS,HREmployee HRE  where HRE.EmployeeCode=OLTS.TrainerCode and OLTS.SlotCode='{0}'  and    OLTS.TrainerCode not in (  select TrainerID as TrainerCode from OnlinePackageUsed where  IsDeleted=0 and SessionDate between '" + Date + "' and '" + Date + " 23:59:59' and SlotID='{0}') ORDER  by NEWID() ", SlotId));

            }
            else if (TrainerID != null && ChangeTrainer == false || TrainerID != null && ChangeTrainer == true)
            {

                dt = getdata(string.Format("select Top 1 TrainerCode,TrainerName,PhotoURL='' from OnlineTrainersSlotMapping  where SlotCode='{0}'  and TrainerCode='{1}'  and  TrainerCode not in (  select TrainerID as TrainerCode from OnlinePackageUsed where SessionDate between  '" + Date + "' and '" + Date + " 23:59:59' and SlotID='{0}' and IsDeleted=0) order by ID asc ", SlotId, TrainerID));

            }

            for (int j = 0; j < dt.Rows.Count; j++)
            {
                for (int m = 0; m < st_absenttrainerlist.Rows.Count; m++)
                {
                    try
                    {
                        if (dt.Rows[m]["TrainerCode"].ToString() == st_absenttrainerlist.Rows[j]["CreatedBy"].ToString())
                        {
                            dt.Rows.RemoveAt(m);
                        }
                    }
                    catch (Exception ec)
                    {

                    }
                }
                dt.AcceptChanges();
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (SlotAvailabilityByTrainer(Convert.ToString(SlotId), Date, Convert.ToString(dt.Rows[i]["TrainerCode"])) == 0)
                {
                    Trainers.Add(new TrainerDetails
                    {
                        trainerId = Convert.ToString(dt.Rows[i]["TrainerCode"])
                    ,
                        trainerName = Convert.ToString(dt.Rows[i]["TrainerName"])
                     ,
                        PhotoURL = Convert.ToString(dt.Rows[i]["PhotoURL"])
                    });
                }
            }

            return Trainers;
        }
        public List<SlotDetails> GetSlotsDetails(int SlotId)
        {
            List<SlotDetails> slots = new List<SlotDetails>();

            DataTable dt = getdata(string.Format("select SlotCode,SlotName,convert(varchar(10), SlotStartTime, 108) as SlotStartTime,convert(varchar(10), SlotEndTime, 108) as SlotEndTime,IsActive as IsAvailable from OnlineSlotTimings", SlotId));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                slots.Add(new SlotDetails
                {
                    slotId = Convert.ToString(dt.Rows[i]["SlotCode"])
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
            return slots;
        }
        public List<PackageDetails> GetPackageDetails(string MobileNo)
        {
            List<PackageDetails> Packages = new List<PackageDetails>();

            // Latest package invoice details

            //
            //DataTable dt = getdata(string.Format("select OLPP.PackageID,OLP.PackageName,convert(varchar(10), OLPP.StartDate, 108) as PackageStartTime,convert(varchar(10), OLPP.EndDate, 108) as PackageEndTime,OLPP.NumberOfSession,OLPP.Invoice,OLPP.MembershipCode from OnlinePackagePurchase OLPP,OnlinePackages OLP where OLPP.PackageID=OLP.PackageID and MobileNo='{0}' and StartDate < GETDATE()  and EndDate > GETDATE()", MobileNo));

            DataTable dt = getdata(string.Format("select  OLPP.PackageID,OLP.PackageName,convert(varchar(10), OLPP.StartDate, 108) as PackageStartTime,convert(varchar(10), OLPP.EndDate, 108) as PackageEndTime,OLPP.NumberOfSession,OLPP.Invoice,OLPP.MembershipCode,SC.UsedSlots from SessionsCount SC,OnlinePackagePurchase OLPP,OnlinePackages OLP where OLP.PackageID=OLPP.PackageID  and  SC.Invoice=OLPP.Invoice and  SC.MobileNo='{0}'   group by OLPP.PackageID,OLP.PackageName,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.Invoice,OLPP.MembershipCode,SC.UsedSlots", MobileNo));

            DataRow[] dr = dt.Select("UsedSlots = MAX(UsedSlots)");



            Packages.Add(new PackageDetails
            {

                packageId = Convert.ToString(dr[0][0].ToString())
                                       ,
                packageName = Convert.ToString(dr[0][1].ToString())
                                       ,
                invoiceId = Convert.ToString(dr[0][5].ToString())
                ,
                membershipCode = Convert.ToString(dr[0][6].ToString())

            });


            return Packages;
        }
        public List<PaymentDetails> GetPaymentDetails()
        {
            List<PaymentDetails> Payments = new List<PaymentDetails>();

            DataTable dt = getdata(string.Format("select Top 5 SlotPrice as ActualPrice,PlanCost as PromotionDiscount,SlotPrice as PaidAmount from CMSPACKAGESCOST"));
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                Payments.Add(new PaymentDetails
                {
                    ActualPrice = float.Parse(dt.Rows[i]["ActualPrice"].ToString())
                            ,
                    PromotionDiscount = float.Parse(dt.Rows[i]["PromotionDiscount"].ToString())
                    ,
                    PaidAmount = float.Parse(dt.Rows[i]["PaidAmount"].ToString())


                });

            }
            return Payments;
        }
        public int ActivateSlots(string[] Slots, string TrainerID)
        {
            string query1 = "";
            int SlotActivate = 0;

            for (int i = 0; i <= Slots.Length - 1; i++)
            {
                cnn.Close();
                query1 = "update OnlineTrainersSlotMapping set IsActive = 0 where SlotCode = '" + Slots[i].ToString() + "'  and TrainerCode='" + TrainerID + "' ";
                SqlCommand cmdExist = new SqlCommand(query1, cnn);
                cnn.Open();
                cmdExist.ExecuteReader();
                cnn.Close();
                SlotActivate = 1;
            }


            return SlotActivate;
        }
        public int DeActivateSlots(string[] Slots, string TrainerID)
        {
            string query1 = "";
            int SlotActivate = 0;

            for (int i = 0; i <= Slots.Length - 1; i++)
            {
                cnn.Close();
                query1 = "update OnlineTrainersSlotMapping set IsActive = 1 where SlotCode = '" + Slots[i].ToString() + "'  and TrainerCode='" + TrainerID + "' ";
                SqlCommand cmdExist = new SqlCommand(query1, cnn);
                cnn.Open();
                cmdExist.ExecuteReader();
                cnn.Close();
                SlotActivate = 1;
            }


            return SlotActivate;
        }
        public string GetTrainer(int SlotId)
        {
            string query1 = "select Top 1 TrainerCode from OnlineTrainersSlotMapping  where  IsActive=1 and SlotCode=" + SlotId + "  and    SlotCode not in (select slotid as SlotCode from OnlinePackageUsed where SessionDate between  CAST(GETDATE() as date) and DATEADD(DAY,1,CAST(GETDATE() as date))) ORDER BY ID asc";

            string TrainerID = "";
            cnn.Close();
            cnn.Open();
            SqlCommand cmdExist = new SqlCommand(query1, cnn);
            SqlDataReader dr = cmdExist.ExecuteReader();
            if (dr.Read())
            {
                TrainerID = dr[0].ToString();
            }


            cnn.Close();


            return TrainerID;
        }
        public ArrayList GetmeetingDetails(string TrainerCode)
        {
            ArrayList arl = new ArrayList();

            string query1 = "select Top 1 ZoomId,ZoomUserName,ZoomPassword,ZoomUrl  from OnLineTrainerMeetingPassword where TrainerCode='" + TrainerCode + "' order by ID desc";

            try
            {

                cnn.Close();
                cnn.Open();
                SqlCommand cmdExist = new SqlCommand(query1, cnn);
                SqlDataReader dr = cmdExist.ExecuteReader();
                if (dr.Read())
                {

                    arl.Add(dr["ZoomId"].ToString());
                    arl.Add(dr["ZoomUserName"].ToString());
                    arl.Add(dr["ZoomPassword"].ToString());
                    arl.Add(dr["ZoomUrl"].ToString());
                    //arl.Add(dr["GoogleId"].ToString());
                    //arl.Add(dr["GoogleUserName"].ToString());
                    //arl.Add(dr["GooglePassword"].ToString());
                    //arl.Add(dr["GoogleUrl"].ToString());

                }
            }
            catch (Exception ec)
            {

            }
            finally
            { cnn.Close(); }



            return arl;
        }
        public ArrayList GetTrainersDetails(string TrainerId)
        {
            ArrayList arl_Tname = new ArrayList();
            try
            {
                cnn.Close();
                string query1 = "select MobileNo,Firstname +' '+Lastname as UserName from Users where UCode='" + TrainerId + "' ";
                SqlCommand cmdExist = new SqlCommand(query1, cnn);
                cnn.Open();
                SqlDataReader drExist = cmdExist.ExecuteReader();
                if (drExist.Read())
                {
                    arl_Tname.Add(drExist["MobileNo"].ToString());
                    arl_Tname.Add(drExist["UserName"].ToString());
                }
                cnn.Close();
            }
            catch (Exception ec)
            {

            }

            return arl_Tname;
        }
        public ArrayList GetUserDetails(string MembershipCode)
        {
            ArrayList arl_Tname = new ArrayList();
            try
            {
                cnn.Close();
                string query1 = "select MobileNo,Firstname +' '+Lastname as UserName from Users where UCode='" + MembershipCode + "' ";
                SqlCommand cmdExist = new SqlCommand(query1, cnn);
                cnn.Open();
                SqlDataReader drExist = cmdExist.ExecuteReader();
                if (drExist.Read())
                {
                    arl_Tname.Add(drExist["MobileNo"].ToString());
                    arl_Tname.Add(drExist["UserName"].ToString());
                }
                cnn.Close();
            }
            catch (Exception ec)
            {

            }

            return arl_Tname;
        }
        public string GetSlotTime(string SlotId, DateTime SlotDate)
        {
            string SlotTime = "";
            try
            {
                cnn.Close();
                string query1 = "select LEFT(DATENAME(DAY,'" + SlotDate + "'),3) + ' ' + LEFT(DATENAME(MONTH,'" + SlotDate + "'),3) + ' ' + RIGHT('00' + CAST(YEAR('" + SlotDate + "') AS VARCHAR),2) +' '+SUBSTRING(CONVERT(VARCHAR, SlotStartTime, 100),13,2) + ':' + SUBSTRING(CONVERT(VARCHAR, SlotStartTime, 100),16,2) + '  '+ SUBSTRING(CONVERT(VARCHAR, SlotStartTime, 100),18,2) AS T   from OnlineSlotTimings where SlotCode='" + SlotId + "'";
                SqlCommand cmdExist = new SqlCommand(query1, cnn);
                cnn.Open();
                SqlDataReader drExist = cmdExist.ExecuteReader();
                if (drExist.Read())
                {
                    SlotTime = drExist[0].ToString();
                }
                cnn.Close();
            }
            catch (Exception ec)
            {

            }

            return SlotTime;
        }
        public Object OffLineSlotBooked([FromBody]SlotBookedPost Users)
        {
            ArrayList arl = new ArrayList();
            SlotBookedFinalOutPut SBFOP = new SlotBookedFinalOutPut();
            DataSet ds_Slotbooked = new DataSet();
            string sJSONResponse = "";
            DataSet ds_slotbkd = new DataSet();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);


            ArrayList arl_TrainerDetails = new ArrayList();
            ArrayList arl_UserDetails = new ArrayList();
            string SlotTime = "";

            try
            {

                if (OffLineSlotAvailabilityByTrainer(Users.SlotId, Users.SlotDate, Users.MemberShipCode) == 0)
                {
                    arl = GetmeetingDetails(Users.TrainerId);
                    cnn.Open();
                    SqlCommand command = cnn.CreateCommand();
                    SqlTransaction transaction;
                    transaction = cnn.BeginTransaction("SampleTransaction");
                    command.Connection = cnn;
                    command.Transaction = transaction;

                    try
                    {
                        command.CommandText = "insert into OffLineSlotUsed(MemberShipCode,BranchCode,MobileNo,EnquireTypeNo,PackageID,SessionID,SlotID,InvoiceID,TrainerID,SessionDate,ActualPrice,MobileDeviceID,IsDeleted,IsActive,CreatedOn) values('" + Users.MemberShipCode + "','"+Users.BranchCode+"','"+Users+"','" + Users.MobileNo + "','" + Users.EnquireTypeNo + "','" + Users.PackageId + "','" + Users.SessionId + "','" + Users.SlotId + "','" + Users.invoiceNo + "','" + Users.TrainerId + "','" + Users.SlotDate + "','" + Users.ActualPrice + "','" + Users.MobileDeviceId + "',0,1,'" + ServerDateTime + "') ";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into CCRMMEnquireStatus(MobileNo,EnquireTypeNo,MobileDeviceID,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + Users.MobileNo + "','" + Users.EnquireTypeNo + "','" + Users.MobileDeviceId + "','" + Users.MobileNo + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        transaction.Commit();

                        arl_TrainerDetails = GetTrainersDetails(Users.TrainerId);
                        arl_UserDetails = GetUserDetails(Users.MemberShipCode);
                        SlotTime = GetSlotTime(Users.SlotId, Convert.ToDateTime(Users.SlotDate));
                        try
                        {
                            SendMessage(arl_TrainerDetails, 1, SlotTime);
                            SendMessage(arl_UserDetails, 2, SlotTime);
                        }
                        catch (Exception ec)
                        {

                        }

                        SBFOP.status = "success";
                        sJSONResponse = JsonConvert.SerializeObject(SBFOP);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        SBFOP.status = "fail";
                        sJSONResponse = JsonConvert.SerializeObject(SBFOP);
                    }
                    finally
                    {

                    }

                }
                else
                {
                    SBFOP.status = "fail";
                    SBFOP.value = "Opps you have just missed this booking slot!Please try another slot";
                    sJSONResponse = JsonConvert.SerializeObject(SBFOP);

                }

            }
            catch (Exception ec)
            {
                SBFOP.status = "fail";
                SBFOP.value = "Internal Server Error";
                sJSONResponse = JsonConvert.SerializeObject(SBFOP);
            }



            return sJSONResponse;


        }
        public int SendMessage(ArrayList arl_Details, int Type, string slottime)
        {

            string SendText = "";
            if (Type == 1)
            {
                SendText = "Dear " + arl_Details[1].ToString() + ", live slot booking for Dilip  (" + slottime + ") is confirmed. Please start the session 10mins before the scheduled time.";
            }
            else
            {
                SendText = "Dear " + arl_Details[1].ToString() + " , we confirm your slot booking (" + slottime + "). Please join the session 5mins before the scheduled time.";
            }






            try
            {
                DataSet ds_custdet = new DataSet();
                DataTable dt_CRM = new DataTable();



                string httpUrl = string.Empty;
                {

                    StreamReader objReader;
                    //httpUrl = "http://sms.smscity.in/httpapi/httpapi?token=8c84bb208c7c7b7b72660ef51a5430b0&sender=NRNGYM&number=0" + arl_Details[0].ToString() + "&route=2&type=1&sms=" + SendText + " ";
                    //httpUrl = "http://api.textlocal.in/send?username=karthikeyaenator@gmail.com&hash=f94d56486adbd53db4d28bcc515c415ef49267fc&numbers=" + arl_Details[0].ToString() + "&message=" + SendText + "&sender=SUCARD ";
                    httpUrl = "http://api.textlocal.in/send?username=dilipgoud@narenfitness.com&hash=4a7076b97161c3a3f6c14028abebf7589808c1ec3ed68eaae63c44e0db3f6497&numbers=" + arl_Details[0].ToString() + "&message=" + SendText + "&sender=NRNGYM ";
                    //http://api.textlocal.in/send?username=dilipgoud@narenfitness.com&hash=4a7076b97161c3a3f6c14028abebf7589808c1ec3ed68eaae63c44e0db3f6497&numbers=
                    System.Net.WebRequest webRequest = System.Net.WebRequest.Create(httpUrl);
                    Stream objstream;
                    objstream = webRequest.GetResponse().GetResponseStream();
                    objReader = new StreamReader(objstream);
                    objReader.Close();
                }



                cnn.Close();

            }
            catch (Exception ex)
            {

            }
            return 1;
        }
        public Object sendMail([FromBody]SlotBookedPost Users)
        {

            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("info@narenfitness.com");
                mail.To.Add("manneyashok@gmail.com");
                mail.Subject = "Test Mail..!!!!";
                mail.Body = "mail";



                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.EnableSsl = true;

                SmtpServer.Credentials = new System.Net.NetworkCredential("info@narenfitness.com", "Fitness@1");

                SmtpServer.Send(mail);




            }
            catch (Exception ex)
            {

            }
            return 1;
        }
        public int SlotAvailabilityByTrainer(string SlotId, string SlotDate, string TrainerId)
        {

            int Exist = 0;
            try
            {
                cnn.Close();
                string query1 = "select Top 1 slot1=(SlotID-3),slot2=(SlotID-2),slot3=(SlotID-1),slot4=SlotID,slot5=(SlotID+1),slot6=(SlotID+2),slot7=(SlotID+3) from OnlinePackageUsed where IsDeleted=0 and  SlotID between (5-3) and (5+3) and  TrainerID='" + TrainerId + "' and SessionDate between '" + SlotDate + "' and '" + SlotDate + " 23:01:01' order by SessionDate desc";
                SqlCommand cmdExist = new SqlCommand(query1, cnn);
                cnn.Open();
                SqlDataReader drExist = cmdExist.ExecuteReader();
                if (drExist.Read())
                {
                    Exist = Convert.ToInt32(drExist[0].ToString());
                }
                else
                {
                    Exist = 0;
                }
                cnn.Close();
            }
            catch (Exception ec)
            {
                Exist = 0;
            }

            return Exist;
        }
        public int OffLineSlotAvailabilityByTrainer(string SlotId, string SlotDate, string MembershipCode)
        {
            int Exist = 0;
            try
            {
                cnn.Close();
                string query1 = "select COUNT(*) from OffLineSlotUsed where IsDeleted=0 and  SlotID='" + SlotId + "' and SessionDate='" + SlotDate + "' and MemberShipCode='" + MembershipCode + "'";
                SqlCommand cmdExist = new SqlCommand(query1, cnn);
                cnn.Open();
                SqlDataReader drExist = cmdExist.ExecuteReader();
                if (drExist.Read())
                {
                    Exist = Convert.ToInt32(drExist[0].ToString());
                }
                else
                {
                    Exist = 0;
                }
                cnn.Close();
            }
            catch (Exception ec)
            {
                Exist = 0;
            }

            return Exist;
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
        // 08/29/2020 : Tasks inside
        public Object SlotBooked([FromBody]SelectedSlotsMultiList Users)
        {
            ArrayList arl = new ArrayList();
            SlotBookedFinalOutPut SBFOP = new SlotBookedFinalOutPut();
            DataSet ds_Slotbooked = new DataSet();
            string sJSONResponse = "";
            DataSet ds_slotbkd = new DataSet();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            List<SlotBookedPostMultiple> Booked = new List<SlotBookedPostMultiple>();
            Booked = Users.slotsbooked;

            List<SelectedSlotPaymentDetails> PaymentDetails = new List<SelectedSlotPaymentDetails>();
            // PaymentDetails = Users.paymentDetails;
            //each and every post should not go to postpaymentdetails only enquirytype=2 

            ArrayList arl_SelectMultier = new ArrayList();
            ArrayList arl_TrainerDetails = new ArrayList();
            ArrayList arl_UserDetails = new ArrayList();

            string SlotTime = "";

            try
            {

                for (int i = 0; i <= Booked.Count - 1; i++)
                {
                    if (Booked[i].EnquireTypeNo != "2")
                    {

                        if (SlotAvailabilityByTrainer(Booked[i].SlotId, Booked[i].SlotDate, Booked[i].TrainerId) == 0)
                        {
                            arl = GetmeetingDetails(Booked[i].TrainerId);
                            cnn.Open();
                            SqlCommand command = cnn.CreateCommand();
                            SqlTransaction transaction;
                            transaction = cnn.BeginTransaction("SampleTransaction");
                            command.Connection = cnn;
                            command.Transaction = transaction;
                            //when invoice is Empty and enquirytype=2 we have to insert which was created in postpaymentdetails.
                            try
                            {
                                command.CommandText = "insert into OnlinePackageUsed(MemberShipCode,MobileNo,EnquireTypeNo,PackageID,SessionID,SlotID,InvoiceID,TrainerID,SessionDate,ActualPrice,MobileDeviceID,ZoomId,ZoomUserName,ZoomPassword,ZoomUrl,GoogleId,GoogleUserName,GooglePassword,GoogleUrl,IsDeleted,IsActive,CreatedOn) values('" + Booked[i].MemberShipCode + "','" + Booked[i].MobileNo + "','" + Booked[i].EnquireTypeNo + "','" + Booked[i].PackageId + "','" + Booked[i].SessionId + "','" + Booked[i].SlotId + "','" + Booked[i].invoiceNo + "','" + Booked[i].TrainerId + "','" + Booked[i].SlotDate + "','" + Booked[i].ActualPrice + "','" + Booked[i].MobileDeviceId + "','" + arl[0].ToString() + "','" + arl[1].ToString() + "','" + arl[2].ToString() + "','" + arl[3].ToString() + "','','','','',0,1,'" + ServerDateTime + "') ";
                                command.ExecuteNonQuery();
                                transaction.Commit();

                                arl_TrainerDetails = GetTrainersDetails(Booked[i].TrainerId);
                                arl_UserDetails = GetUserDetails(Booked[i].MemberShipCode);
                                SlotTime = GetSlotTime(Booked[i].SlotId, Convert.ToDateTime(Booked[i].SlotDate));

                                //SendMessage(arl_TrainerDetails, 1, SlotTime);
                                //SendMessage(arl_UserDetails, 2, SlotTime);

                                SBFOP.status = "success";
                                sJSONResponse = JsonConvert.SerializeObject(SBFOP);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                SBFOP.status = "fail";
                                sJSONResponse = JsonConvert.SerializeObject(SBFOP);
                            }
                            finally
                            {

                            }

                        }
                        else
                        {
                            SBFOP.status = "fail";
                            SBFOP.value = "Opps you have just missed this booking slot!Please try another slot";
                            sJSONResponse = JsonConvert.SerializeObject(SBFOP);

                        }
                    }
                    else
                    {
                        if (SlotAvailabilityByTrainer(Booked[i].SlotId, Booked[i].SlotDate, Booked[i].TrainerId) == 0)
                        {
                            string Invoice = "";

                            if (Booked[i].invoiceNo == null)
                            {
                                Invoice = "N" + UniqueGeneration2();
                                int UID = UIDNew("1104");

                                if (InvoiceCheck(Invoice) != 0)
                                {
                                    Invoice = "N" + UniqueGeneration2();
                                }
                            }
                            else
                            {
                                Invoice = Booked[i].invoiceNo;
                            }


                            PostPaymentDetails(Users.slotsbooked, (Invoice));
                            arl = GetmeetingDetails(Booked[i].TrainerId);
                            cnn.Open();
                            SqlCommand command = cnn.CreateCommand();
                            SqlTransaction transaction;
                            transaction = cnn.BeginTransaction("SampleTransaction");
                            command.Connection = cnn;
                            command.Transaction = transaction;
                            //when invoice is Empty and enquirytype=2 we have to insert which was created in postpaymentdetails.
                            try
                            {

                                // PostPaymentDetails(Users.slotsbooked);
                                command.CommandText = "insert into OnlinePackageUsed(MemberShipCode,MobileNo,EnquireTypeNo,PackageID,SessionID,SlotID,InvoiceID,TrainerID,SessionDate,ActualPrice,MobileDeviceID,ZoomId,ZoomUserName,ZoomPassword,ZoomUrl,GoogleId,GoogleUserName,GooglePassword,GoogleUrl,IsDeleted,IsActive,CreatedOn) values('" + Booked[i].MemberShipCode + "','" + Booked[i].MobileNo + "','" + Booked[i].EnquireTypeNo + "','" + Booked[i].PackageId + "','" + Booked[i].SessionId + "','" + Booked[i].SlotId + "','" + Invoice + "','" + Booked[i].TrainerId + "','" + Booked[i].SlotDate + "','" + Booked[i].ActualPrice + "','" + Booked[i].MobileDeviceId + "','" + arl[0].ToString() + "','" + arl[1].ToString() + "','" + arl[2].ToString() + "','" + arl[3].ToString() + "','','','','',0,1,'" + ServerDateTime + "') ";
                                command.ExecuteNonQuery();
                                transaction.Commit();

                                arl_TrainerDetails = GetTrainersDetails(Booked[i].TrainerId);
                                arl_UserDetails = GetUserDetails(Booked[i].MemberShipCode);
                                SlotTime = GetSlotTime(Booked[i].SlotId, Convert.ToDateTime(Booked[i].SlotDate));

                                //SendMessage(arl_TrainerDetails, 1, SlotTime);
                                //SendMessage(arl_UserDetails, 2, SlotTime);

                                SBFOP.status = "success";
                                sJSONResponse = JsonConvert.SerializeObject(SBFOP);
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                SBFOP.status = "fail";
                                sJSONResponse = JsonConvert.SerializeObject(SBFOP);
                            }
                            finally
                            {

                            }



                        }
                        else
                        {
                            SBFOP.status = "fail";
                            SBFOP.value = "Opps you have just missed this booking slot!Please try another slot";
                            sJSONResponse = JsonConvert.SerializeObject(SBFOP);

                        }
                    }
                }
            }
            catch (Exception ec)
            {
                SBFOP.status = "fail";
                SBFOP.value = "Internal Server Error";
                sJSONResponse = JsonConvert.SerializeObject(SBFOP);
            }
            return sJSONResponse;
        }
        public Object OffLineSlotBookedMultple([FromBody]OffLineSelectedSlotsMultiList Users)
        {
            ArrayList arl = new ArrayList();
            SlotBookedFinalOutPut SBFOP = new SlotBookedFinalOutPut();
            DataSet ds_Slotbooked = new DataSet();
            string sJSONResponse = "";
            DataSet ds_slotbkd = new DataSet();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            List<OffLineSlotBookedPostMultiple> Booked = new List<OffLineSlotBookedPostMultiple>();
            Booked = Users.slotsbooked;

            ArrayList arl_TrainerDetails = new ArrayList();
            ArrayList arl_UserDetails = new ArrayList();
            string SlotTime = "";

            try
            {
                for (int i = 0; i <= Booked.Count - 1; i++)
                {
                    if (OffLineSlotAvailabilityByTrainer(Booked[i].SlotId, Booked[i].SlotDate, Booked[i].MemberShipCode) == 0)
                    {
                        arl = GetmeetingDetails(Booked[i].TrainerId);
                        cnn.Open();
                        SqlCommand command = cnn.CreateCommand();
                        SqlTransaction transaction;
                        transaction = cnn.BeginTransaction("SampleTransaction");
                        command.Connection = cnn;
                        command.Transaction = transaction;

                        try
                        {
                            command.CommandText = "insert into OffLineSlotUsed(MemberShipCode,MobileNo,EnquireTypeNo,PackageID,SessionID,SlotID,InvoiceID,TrainerID,SessionDate,ActualPrice,MobileDeviceID,IsDeleted,IsActive,CreatedOn) values('" + Booked[i].MemberShipCode + "','" + Booked[i].MobileNo + "','" + Booked[i].EnquireTypeNo + "','" + Booked[i].PackageId + "','" + Booked[i].SessionId + "','" + Booked[i].SlotId + "','" + Booked[i].invoiceNo + "','" + Booked[i].TrainerId + "','" + Booked[i].SlotDate + "','" + Booked[i].ActualPrice + "','" + Booked[i].MobileDeviceId + "',0,1,'" + ServerDateTime + "') ";
                            command.ExecuteNonQuery();
                            transaction.Commit();

                            arl_TrainerDetails = GetTrainersDetails(Booked[i].TrainerId);
                            arl_UserDetails = GetUserDetails(Booked[i].MemberShipCode);
                            SlotTime = GetSlotTime(Booked[i].SlotId, Convert.ToDateTime(Booked[i].SlotDate));
                            try
                            {
                                SendMessage(arl_TrainerDetails, 1, SlotTime);
                                SendMessage(arl_UserDetails, 2, SlotTime);
                            }
                            catch (Exception ec)
                            {

                            }

                            SBFOP.status = "success";
                            sJSONResponse = JsonConvert.SerializeObject(SBFOP);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            SBFOP.status = "fail";
                            sJSONResponse = JsonConvert.SerializeObject(SBFOP);
                        }
                        finally
                        {

                        }

                    }
                    else
                    {
                        SBFOP.status = "fail";
                        SBFOP.value = "Opps you have just missed this booking slot!Please try another slot";
                        sJSONResponse = JsonConvert.SerializeObject(SBFOP);

                    }
                }
            }
            catch (Exception ec)
            {
                SBFOP.status = "fail";
                SBFOP.value = "Internal Server Error";
                sJSONResponse = JsonConvert.SerializeObject(SBFOP);
            }



            return sJSONResponse;


        }
        public string ReceiptNo()
        {
            Random random = new Random();
            string r = "";
            try
            {
                for (int i = 1; i < 5; i++)
                {
                    r += random.Next(1, 9).ToString();

                }
            }
            catch (Exception ex)
            {
                return "0";
            }
            return r;

        }
        public string UniqueGeneration2()
        {
            Random random = new Random();
            string r = "";
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    r += random.Next(0, 9).ToString();
                }
            }
            catch (Exception ex)
            {
                return "";
            }
            return r;

        }
        public ArrayList GetPersonDetailsByMobileNo(string MobileNo)
        {
            ArrayList arl_pd = new ArrayList();

            string query = "select EnquirePersonFirstName as UserName,D_O_B,Gender  from CCRMMEnquireForm  where MobileNo='" + MobileNo + "'";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_pdetails = cmd_SubEXT.ExecuteReader();
                if (DR_pdetails.Read())
                {
                    arl_pd.Add(DR_pdetails["UserName"].ToString());
                    arl_pd.Add(DR_pdetails["D_O_B"].ToString());
                    arl_pd.Add(DR_pdetails["Gender"].ToString());
                }
                cnn.Close();
            }
            return arl_pd;
        }
        public int UIDNew(string BranchCode)
        {
            int UIDNew = 0;
            string query = "select isnull((Max(UID)+1), 3001) as UID from LoginID where BranchCode='" + BranchCode + "'";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_Login = cmd_SubEXT.ExecuteReader();
                if (DR_Login.Read())
                {
                    UIDNew = Convert.ToInt32(DR_Login[0].ToString());
                }
                cnn.Close();
            }
            return UIDNew;
        }
        public int InvoiceCheck(string Invoice)
        {
            int InvoiceCheck = 0;
            string query = "select COUNT(*) as Count from FAInvoice where InvoiceID='" + Invoice + "' ";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_Login = cmd_SubEXT.ExecuteReader();
                if (DR_Login.Read())
                {
                    InvoiceCheck = Convert.ToInt32(DR_Login[0].ToString());
                }
                cnn.Close();
            }
            return InvoiceCheck;
        }
        public string GetMembersExistance(string MobileNo)
        {
            string MembershipCode = "";
            string query = "select top 1 UCode from Users where MobileNo='" + MobileNo + "' and MobileNo<>'' order by ID desc";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_pdetails = cmd_SubEXT.ExecuteReader();
                if (DR_pdetails.Read())
                {
                    MembershipCode = DR_pdetails[0].ToString();

                }
                cnn.Close();
            }
            return MembershipCode;
        }
        public string PostPaymentDetails(List<SlotBookedPostMultiple> Booked, string Invoice)
        {

            string r = "";
            DataSet ds_PaymentDetailsPost = new DataSet();

            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            //ArrayList arl_pdetails = GetPersonDetailsByMobileNo(paymentDetails[0].MobileNo);


            int Year = DateTime.Now.Year - 2000;
            int Month = DateTime.Now.Month;


            // Clients Unique COde
            ArrayList al = new ArrayList();

            //string MembershipCode = GetMembersExistance(arl_PostDetails[0].ToString());


            r = "";


            string ReceiptNos = ReceiptNo();

            //start
            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            // Start a local transaction.
            transaction = cnn.BeginTransaction("SampleTransaction");
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = cnn;
            command.Transaction = transaction;
            //MobileDeviceID,ModeofPayment,paymentid,amount,currency,discount,transactiondate,enddate,actualprice,numberofsession,numberofdays,discount,comments - These keys data should be static we will not receive.

            try
            {
                {
                    command.CommandText = "insert into CCRMMEnquireStatus(MobileNo,EnquireTypeNo,MobileDeviceID,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + Booked[0].MobileNo + "','" + Booked[0].EnquireTypeNo + "','" + Booked[0].MobileDeviceId + "','" + Booked[0].MobileNo + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into PaymentGateway(GatewayProviderName,OrderId,BranchCode,MemberShipCode,Invoice,MobileNo,ModeOfPayment,payment_capture,Amount,Currency,Discount,receipt,TransactionDate,CreatedOn,CreatedBy,IsDeleted,IsActive) values('GPN001','" + ReceiptNo() + "','1104','" + r + "','" + Invoice + "','" + Booked[0].MobileNo + "','INR','','" + ReceiptNo() + "','INR','0','" + ReceiptNos + "','" + Booked[0].SlotDate + "','" + ServerDateTime + "','" + Booked[0].CreatedBy + "',0,1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into OnlinePackagePurchase(BranchCode,MembershipCode,MobileNo,EnquireTypeNo,PackageID,StartDate,EndDate,ActualPrice,NumberOfSession,NumberOfDaysValidity,MobileDeviceID,Invoice,Discount,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('1104','" + r + "','" + Booked[0].MobileNo + "','" + Booked[0].EnquireTypeNo + "','" + Booked[0].PackageId + "','" + Booked[0].SlotDate + "','" + Booked[0].SlotDate + "','" + Booked[0].ActualPrice + "','1','1','" + Booked[0].MobileDeviceId + "','" + Invoice + "','" + Booked[0].ActualPrice + "','','" + Booked[0].MobileNo + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();


                    transaction.Commit();
                    cnn.Close();


                }
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (Exception ex2)
                {

                }
            }

            return r;
            //end
        }
        public Object GetOfflineSelectedSlotsDetails([FromBody]SlotSelected SlotsSelected)
        {

            SelectedSlotFinalOutPut SSFOP = new SelectedSlotFinalOutPut();
            SelectSlotFinalOutPutMessage SSFOPM = new SelectSlotFinalOutPutMessage();

            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            try
            {
                List<Slots> Slots = new List<Slots>();
                Slots SlotsDetails = new Slots { trainerDetails = GetOfflineTrainerDetails(SlotsSelected.SelectedSlotId, SlotsSelected.TrainerId, SlotsSelected.Date), slotDetails = GetOfflinePackageDetails(SlotsSelected.SelectedSlotId, SlotsSelected.MobileNo) };

                Slots.Add(SlotsDetails);

                if (Slots[0].trainerDetails.Count == 0)
                {
                    SSFOPM.status = "fail";
                    SSFOPM.value = "Opps you have just missed this booking slot! Please try another slot";
                    sJSONResponse = JsonConvert.SerializeObject(SSFOPM);
                }
                else
                {
                    SSFOP.status = "success";
                    SSFOP.value = Slots;
                    sJSONResponse = JsonConvert.SerializeObject(SSFOP);
                }

            }
            catch (Exception ec)
            {
                SSFOPM.status = "fail";
                SSFOPM.value = "Internal Server Error";
                sJSONResponse = JsonConvert.SerializeObject(SSFOPM);

            }

            return sJSONResponse;
        }
        public List<TrainerDetails> GetOfflineTrainerDetails(string SlotId, string TrainerID, string Date)
        {
            List<TrainerDetails> Trainers = new List<TrainerDetails>();
            DataTable dt = new DataTable();
            DataTable st_absenttrainerlist = AbsentTrainer(Date);

            if (TrainerID == null)
            {
                dt = getdata(string.Format("select top 1 CMSSWA.TrainerCode,HRE.EmployeeName as TrainerName,HRE.PhotoURL from CMSSlotWiseAllocation CMSSWA,HREmployee HRE  where HRE.EmployeeCode=CMSSWA.TrainerCode  and CMSSWA.IsActive=1 and CMSSWA.TrainerCode='{0}' and CMSSWA.SlotCode='{1}' order by CMSSWA.ID desc ", TrainerID, SlotId));

                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {

                    if (SlotAvailabilityByTrainer(Convert.ToString(SlotId), Date, Convert.ToString(dt.Rows[i]["TrainerCode"])) == 0)
                    {
                        Trainers.Add(new TrainerDetails
                        {
                            trainerId = Convert.ToString(dt.Rows[i]["TrainerCode"])
                                                       ,
                            trainerName = Convert.ToString(dt.Rows[i]["TrainerName"])
                                                        ,
                            PhotoURL = Convert.ToString(dt.Rows[i]["PhotoURL"])
                        });

                        i = dt.Rows.Count - 1;

                    }
                }

            }
            else
            {
                if (SlotAvailabilityByTrainer(Convert.ToString(SlotId), Date, TrainerID) == 0)
                {
                    dt = getdata(string.Format("select Distinct CMSSWA.TrainerCode,HRE.EmployeeName as TrainerName,HRE.PhotoURL from CMSSlotWiseAllocation CMSSWA,HREmployee HRE  where HRE.EmployeeCode=CMSSWA.TrainerCode  and CMSSWA.IsActive=1 and CMSSWA.TrainerCode='{0}' and CMSSWA.SlotCode='{1}' ", TrainerID, SlotId));

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        for (int m = 0; m < st_absenttrainerlist.Rows.Count; m++)
                        {
                            if (dt.Rows[m]["TrainerCode"].ToString() == st_absenttrainerlist.Rows[j]["CreatedBy"].ToString())
                            {
                                dt.Rows.RemoveAt(m);
                            }
                        }
                        dt.AcceptChanges();
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Trainers.Add(new TrainerDetails
                        {
                            trainerId = Convert.ToString(dt.Rows[i]["TrainerCode"])
                            ,
                            trainerName = Convert.ToString(dt.Rows[i]["TrainerName"])
                             ,
                            PhotoURL = Convert.ToString(dt.Rows[i]["PhotoURL"])
                        });

                    }
                }
                else
                {
                }
            }

            return Trainers;
        }
        public List<PackageDetails> GetOfflinePackageDetails(string SlotId, string MobileNo)
        {
            List<PackageDetails> Packages = new List<PackageDetails>();


            //DataTable dt = getdata(string.Format("select OLPP.PackageID,OLP.PackageName,convert(varchar(10), OLPP.StartDate, 108) as PackageStartTime,convert(varchar(10), OLPP.EndDate, 108) as PackageEndTime,OLPP.NumberOfSession,OLPP.Invoice,OLPP.MembershipCode from OnlinePackagePurchase OLPP,OnlinePackages OLP where OLPP.PackageID=OLP.PackageID and MobileNo='{0}' and StartDate < GETDATE()  and EndDate > GETDATE()", MobileNo));

            DataTable dt = getdata(string.Format("select OSLU.PackageID,CMSP.PackageName,OSLU.SessionDate as PackageStartTime,OSLU.SessionDate as PackageEndTime,NumberOfSession=0,OSLU.InvoiceID,OSLU.MemberShipCode,UsedSlots=1 from OffLineSlotUsed OSLU,CMSPACKAGES CMSP where OSLU.PackageID=CMSP.PackageCode and OSLU.SlotID='{0}' and OSLU.MobileNo='{1}' ", SlotId, MobileNo));

            Packages.Add(new PackageDetails
            {

                packageId = Convert.ToString(dt.Rows[0][0].ToString())
                                                ,
                packageName = Convert.ToString(dt.Rows[0][1].ToString())
                                                ,
                invoiceId = Convert.ToString(dt.Rows[0][5].ToString())
                         ,
                membershipCode = Convert.ToString(dt.Rows[0][6].ToString())

            });

            return Packages;
        }

    }
}