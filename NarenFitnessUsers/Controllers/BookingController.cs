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
using NarenFitnessUsers.Models.Slots.Slot;
using NarenFitnessUsers.Models.Booking;
using NarenFitnessUsers.Models.Facility;
using NarenFitnessUsers.Models.Booking.InActive;
using NarenFitnessUsers.Models.Booking.Active;
using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;

namespace NarenFitnessUsers.Controllers
{
    public class BookingController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        JsonClass json = new JsonClass();
        public Object GetSlotBookingByTrainer([FromBody]BookedSlotsRequest SlotsBooking)
        {
            FinalOutPutA FOutputA = new FinalOutPutA();
            DataSet ds_SlotBooking = new DataSet();
            string sJSONResponse = "";

            try
            {

                List<TrainerSlotsBooked> dbslots = new List<TrainerSlotsBooked>();
                TrainerSlotsBooked dashboardlistslots = new TrainerSlotsBooked { trainerSlots = SlotBookingByTrainer(SlotsBooking.TrainerID, SlotsBooking.FromDate, SlotsBooking.ToDate) };
                dbslots.Add(dashboardlistslots);
                FOutputA.status = "success";
                FOutputA.value = dbslots;

                sJSONResponse = JsonConvert.SerializeObject(FOutputA);
            }
            catch (Exception ec)
            {
                FOutputA.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(FOutputA);
            }


            return sJSONResponse;
        }
        public List<SlotBookingByTrainerOP> SlotBookingByTrainer(string TrainerID, string FromDate, string ToDate)
        {
            List<SlotBookingByTrainerOP> ftd = new List<SlotBookingByTrainerOP>();


            DataTable dt = new DataTable();
            dt = getdata(string.Format("select distinct  CCRMMEQ.EnquirePersonFirstName +' '+ CCRMMEQ.EnquirePersonLastName as UserName,OLST.SlotCode as SlotID,convert(varchar(10), OLST.SlotStartTime, 108) as SlotStartTime,convert(varchar(10), OLST.SlotEndTime, 108) as SlotEndTime,OLPU.SessionDate,OLPU.PackageID,OP.PackageName,OLPU.ZoomId as MeetingId,ZoomPassword as MeetingPassword,OLPU.ZoomUrl as MeetingURL,OLPU.MobileNo  from  OnlinePackageUsed OLPU,OnlineSlotTimings OLST,CCRMMEnquireForm CCRMMEQ,OnlinePackages OP where OLPU.PackageID=OP.PackageID and CCRMMEQ.MobileNo=OLPU.MobileNo and  OLPU.SlotID=OLST.SlotCode and  OLPU.IsDeleted=0 and OLPU.TrainerID='{0}' and SessionDate between '{1}' and '{2}'  ", TrainerID, FromDate, ToDate));

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {

                ftd.Add(new SlotBookingByTrainerOP
                {
                    userName = Convert.ToString(dt.Rows[i]["UserName"])
                 ,
                    slotID = Convert.ToString(dt.Rows[i]["SlotID"])
                 ,
                    slotStartTime = Convert.ToString(dt.Rows[i]["SlotStartTime"])
                 ,
                    slotEndTime = Convert.ToString(dt.Rows[i]["SlotEndTime"])
                ,
                    sessionDate = Convert.ToString(dt.Rows[i]["SessionDate"])
                 ,
                    packageID = Convert.ToString(dt.Rows[i]["PackageID"])
                 ,
                    packageName = Convert.ToString(dt.Rows[i]["PackageName"])
                ,
                    meetingId = Convert.ToString(dt.Rows[i]["meetingId"])
                 ,
                    meetingPassword = Convert.ToString(dt.Rows[i]["meetingPassword"])
                 ,
                    meetingUrl = Convert.ToString(dt.Rows[i]["meetingUrl"])
                    ,
                    mobileNumber = Convert.ToString(dt.Rows[i]["MobileNo"])
                });
            }
            return ftd;

        }
        public Object GetBooking([FromBody]BookingRequest Booking)
        {

            FinalOutPutB FOutputA = new FinalOutPutB();
            DataSet ds_SlotBooking = new DataSet();
            string sJSONResponse = "";

            try
            {

                List<GetBookingroot> dbsbookingroot = new List<GetBookingroot>();
                GetBookingroot dashboardlist = new GetBookingroot { bookings = GetBookings(Booking.MembershipCode) };
                dbsbookingroot.Add(dashboardlist);

                FOutputA.status = "success";
                FOutputA.value = dbsbookingroot;

                sJSONResponse = JsonConvert.SerializeObject(FOutputA);
            }
            catch (Exception ec)
            {
                ds_SlotBooking.Tables.Clear();
                ds_SlotBooking.Tables.Add(ReturnException("Fail", ""));
            }


            return sJSONResponse;
        }
        public List<GetBookings> GetBookings(string MemberShipCode)
        {
            List<GetBookings> ftd = new List<GetBookings>();


            DataTable dt = new DataTable();
            dt = getdata(string.Format("select OLP.PackageName,PG.TransactionDate as InvoiceDate,PG.OrderId,PG.Amount as PaidAmount,PG.Invoice as InvoiceID from OnlinePackagePurchase OLPP,PaymentGateway PG,OnlinePackages OLP where  OLPP.Invoice=PG.Invoice and OLP.PackageID=OLPP.PackageID and OLPP.MembershipCode='{0}'  union all  select CMSP.PackageName,FAI.PaymentDate as InvoiceDate,FAI.Receipt as OrderID,(FAI.PayableAmount+FAI.PayableAmount2) as PaidAmount,FAI.InvoiceID from CCRMMembership CCRMM,FAInvoice FAI,CMSPACKAGES CMSP where CCRMM.InvoiceID=FAI.InvoiceID  and CMSP.PackageCode=CCRMM.PackageCode and CCRMM.MembershipCode='{0}' ", MemberShipCode));

            ftd.Add(new GetBookings
            {
                packageName = Convert.ToString(dt.Rows[0]["PackageName"])
                 ,
                invoiceDate = Convert.ToString(dt.Rows[0]["InvoiceDate"])
                 ,
                orderID = Convert.ToString(dt.Rows[0]["OrderId"])
                 ,
                paidAmount = Convert.ToString(dt.Rows[0]["PaidAmount"])
                ,
                invoiceId = Convert.ToString(dt.Rows[0]["InvoiceID"])


            });

            return ftd;

        }
        public Object GetPaymentInfo([FromBody]BookedPackageRequest Booking)
        {
            DataSet BookingsDetails = new DataSet();
            string sJSONResponse = "";
            FinalOutPutC FOutputC = new FinalOutPutC();

            try
            {

                List<PackageDetailsByInvoice> Packages = new List<PackageDetailsByInvoice>();
                PackageDetailsByInvoice PackagesList = new PackageDetailsByInvoice { bookedPackages = GetBookedPackages(Booking.InvoiceId, Booking.Mode) };
                Packages.Add(PackagesList);

                FOutputC.status = "success";
                FOutputC.value = Packages;

                sJSONResponse = JsonConvert.SerializeObject(FOutputC);


            }
            catch (Exception ec)
            {
                FOutputC.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(FOutputC);
            }


            return sJSONResponse;
        }
        public List<GetBookedPackagesByInvoice> GetBookedPackages(string InvoiceId, string Mode)
        {
            List<GetBookedPackagesByInvoice> ftd = new List<GetBookedPackagesByInvoice>();
            DataTable dt_Booking;
            DataTable dt_BookingDue;

            DataTable dt = new DataTable();

            if (Mode == "Online")
            {
                dt_Booking = getdata(string.Format("select PG.Invoice,OP.PackageName,OLPP.StartDate as PackageStartDate,OLPP.EndDate as PackageEndDate,HRE.EmployeeName as TrainerName,PromoCode='0',OLPP.ActualPrice,PayableAmount=0,RemainingAmount=0,AmountDue=0,Wallet=0,DiscountAmount=0.0,PG.Amount as PaidAmount,OLPU.TrainerID as TrainerId,PG.TransactionDate as InvoiceDate,OLPP.PackageID from OnlinePackagePurchase OLPP,PaymentGateway PG,OnlinePackageUsed OLPU,HREmployee HRE,OnlinePackages OP,CCRMMEnquireForm CCRMMEQ where OLPP.Invoice=OLPU.InvoiceID and CCRMMEQ.MobileNo=OLPP.MobileNo and  OLPP.MobileNo=PG.MobileNo  and OLPU.TrainerID=HRE.EmployeeCode and OP.PackageID=OLPP.PackageID and OLPP.Invoice='{0}'", InvoiceId));
            }
            else
            {
                dt_Booking = getdata(string.Format("select distinct  CCRM.InvoiceID,CMSP.PackageName,CCRM.StartDate AS PackageStartDate,CCRM.MembershipExpireDate as PackageEndDate,HRE.EmployeeName as TrainerName,FAI.PromoCode,FAI.PlanCost as ActualPrice,FAI.SlotPrice as PayableAmount,FAI.RemainingAmount,FAI.AmountDue,isnull(FAI.Wallet,0) as Wallet,FAI.DiscountAmount,FAI.PayableAmount+FAI.PayableAmount2 as PaidAmount,CCRM.TrainerCode,FAI.PaymentDate,CCRM.PackageCode as PackageId,FAP.PaymentTypeName  from CCRMMembership CCRM,FAInvoice FAI,HREmployee HRE,CMSPACKAGES CMSP,FAPaymentModes FAP  where CMSP.PackageCode=CCRM.PackageCode and HRE.EmployeeCode=FAI.TrainerCode and CCRM.InvoiceID=FAI.InvoiceID and FAP.PayModeCode=FAI.FAPaymentModes and CCRM.InvoiceID='{0}'", InvoiceId));
                //dt_BookingDue = getdata(string.Format(" ", InvoiceId));

            }

            //dt_Booking = getdata(string.Format("select OLPP.PackageID,OP.PackageName,OLPP.StartDate as PackageStartDate,OLPP.EndDate as PackageEndDate,OLPP.ActualPrice,PG.Amount as PaidAmount,DiscountAmount='0',OLPU.TrainerID as TrainerId,HRE.EmployeeName as TrainerName,PG.Invoice,PG.OrderId,PG.TransactionDate as InvoiceDate,CCRMMEQ.Email from OnlinePackagePurchase OLPP,PaymentGateway PG,OnlinePackageUsed OLPU,HREmployee HRE,OnlinePackages OP,CCRMMEnquireForm CCRMMEQ where OLPP.Invoice=OLPU.InvoiceID and CCRMMEQ.MobileNo=OLPP.MobileNo and  OLPP.MobileNo=PG.MobileNo  and OLPU.TrainerID=HRE.EmployeeCode and OP.PackageID=OLPP.PackageID and OLPP.Invoice='{0}' ", InvoiceId));
            for (int i = 0; i <= dt_Booking.Rows.Count - 1; i++)
            {
                ftd.Add(new GetBookedPackagesByInvoice
                {
                    packageID = Convert.ToString(dt_Booking.Rows[0]["PackageID"])
                     ,
                    packageName = Convert.ToString(dt_Booking.Rows[0]["PackageName"])
                     ,
                    packageStartDate = Convert.ToString(dt_Booking.Rows[0]["PackageStartDate"])
                     ,
                    packageEndDate = Convert.ToString(dt_Booking.Rows[0]["PackageEndDate"])
                    ,
                    actualPrice = Convert.ToDouble(dt_Booking.Rows[0]["ActualPrice"])
                    ,
                    paidAmount = Convert.ToDouble(dt_Booking.Rows[0]["PaidAmount"])
                     ,
                    discountAmount = Convert.ToDouble(dt_Booking.Rows[0]["DiscountAmount"])
                     ,
                    trainerId = Convert.ToString(dt_Booking.Rows[0]["TrainerCode"])
                    ,
                    trainerName = Convert.ToString(dt_Booking.Rows[0]["TrainerName"])
                    ,
                    invoiceID = Convert.ToString(dt_Booking.Rows[0]["InvoiceID"])
                     ,
                    invoiceDate = Convert.ToString(dt_Booking.Rows[0]["PaymentDate"])
                     ,
                    PromoCode = Convert.ToString(dt_Booking.Rows[0]["PromoCode"])
                    ,
                    PayableAmount = Convert.ToDouble(dt_Booking.Rows[0]["PayableAmount"])
                    ,
                    RemainingAmount = Convert.ToDouble(dt_Booking.Rows[0]["RemainingAmount"])
                    ,
                    AmountDue = Convert.ToDouble(dt_Booking.Rows[0]["AmountDue"])
                    ,
                    Wallet = Convert.ToDouble(dt_Booking.Rows[0]["Wallet"])
                    ,
                    MOP = Convert.ToString(dt_Booking.Rows[0]["PaymentTypeName"])


                });
            }
            return ftd;

        }
        private DataTable ReturnException(string Status, string Message)
        {
            DataTable dtMsg = new DataTable("MSG");
            dtMsg.Columns.Add("Status");
            dtMsg.Columns.Add("Value");
            DataRow drMsg = dtMsg.NewRow();
            drMsg["Status"] = Status;
            drMsg["Value"] = Message;
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

        //InActive
        public Object GetBookingInActive([FromBody]BookingRequestInActive Booking)
        {

            FinalOutPutBInActive FOutputB = new FinalOutPutBInActive();
            FinalOutPutABActivePlan FOutputBOffline = new FinalOutPutABActivePlan();
            DataSet ds_SlotBooking = new DataSet();
            string sJSONResponse = "";
            int count = 0;
            try
            {

                List<GetBookingrootInActive> dbsbookingroot = new List<GetBookingrootInActive>();
                GetBookingrootInActive dashboardlist = new GetBookingrootInActive { bookings = GetBookingsInActive(Booking.MembershipCode, Booking.Mode, Booking.MobileNo) };
                dbsbookingroot.Add(dashboardlist);

                count = dashboardlist.bookings.Count;

                if (count > 0)
                {

                    FOutputB.status = "success";
                    FOutputB.value = dbsbookingroot;
                    sJSONResponse = JsonConvert.SerializeObject(FOutputB);
                }
                else
                {
                    FOutputBOffline.status = "success";
                    FOutputBOffline.value = "no bookings found";
                    sJSONResponse = JsonConvert.SerializeObject(FOutputBOffline);
                }
            }
            catch (Exception ec)
            {
                FOutputBOffline.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(FOutputBOffline);
            }


            return sJSONResponse;
        }
        public List<GetBookingsInActive> GetBookingsInActive(string MemberShipCode, string Mode, string MobileNo)
        {
            List<GetBookingsInActive> ftd = new List<GetBookingsInActive>();
            DataTable dt = new DataTable();

          
                if (Mode == "Online")
                {
                    dt = getdata(string.Format("select OLP.PackageName,PG.TransactionDate as InvoiceDate,PG.OrderId,PG.Amount as PaidAmount,PG.Invoice as InvoiceID,OLPP.StartDate,OLPP.EndDate,OLPP.BranchCode from OnlinePackagePurchase OLPP,PaymentGateway PG,OnlinePackages OLP where  OLPP.Invoice=PG.Invoice and OLP.PackageID=OLPP.PackageID and OLPP.MobileNo='{0}' and OLPP.EndDate < GETDATE() and OLPP.StartDate <= GETDATE()", MobileNo));
                    if (dt.Rows.Count == 0)
                    {
                        dt = getdata(string.Format("select Top 1 PackageName='',InvoiceDate='',OrderID=0,PaidAmount=0,InvoiceID=0,StartDate='',MembershipExpireDate='',BranchCode from  OnlinePackagePurchase  where MobileNo='{0}'  ", MobileNo));

                    }


                }
                else
                {
                    dt = getdata(string.Format("select CMSP.PackageName,FAI.PaymentDate as InvoiceDate,FAI.Receipt as OrderID,(FAI.PayableAmount+FAI.PayableAmount2) as PaidAmount,FAI.InvoiceID,CCRMM.StartDate,CCRMM.MembershipExpireDate as EndDate,CCRMM.BranchCode from CCRMMembership CCRMM,FAInvoice FAI,CMSPACKAGES CMSP ,Users U where U.UCode=CCRMM.MembershipCode and CCRMM.InvoiceID=FAI.InvoiceID  and CMSP.PackageCode=CCRMM.PackageCode and U.MobileNo='{0}' and CCRMM.MembershipExpireDate < GETDATE() and CCRMM.StartDate < GETDATE() order by InvoiceDate desc ", MobileNo));

                    if (dt.Rows.Count == 0)
                    {
                        dt = getdata(string.Format("select PackageName = '', CreatedOn as InvoiceDate,OrderID = 0,PaidAmount = 0,InvoiceID,SessionDate as StartDate ,SessionDate as MembershipExpireDate,BranchCode from OffLineSlotUsed where MobileNo = '{0}' ", MobileNo));

                    }
                }

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                ftd.Add(new GetBookingsInActive
                {
                    packageName = Convert.ToString(dt.Rows[i]["PackageName"])
                     ,
                    invoiceDate = Convert.ToString(dt.Rows[i]["InvoiceDate"])
                     ,
                    orderID = Convert.ToString(dt.Rows[i]["OrderId"])
                     ,
                    paidAmount = Convert.ToString(dt.Rows[i]["PaidAmount"])
                    ,
                    invoiceId = Convert.ToString(dt.Rows[i]["InvoiceID"])


                });
            }
            return ftd;

        }

        //Active
        public Object GetActiveBookings([FromBody]BookingRequestActive Booking)
        {
            FinalOutPutBActive FOutputB = new FinalOutPutBActive();
            FinalOutPutBActiveOnline FOutputBOnline = new FinalOutPutBActiveOnline();
            FinalOutPutABActivePlan FOutputBPlan = new FinalOutPutABActivePlan();
            DataSet ds_SlotBooking = new DataSet();
            string sJSONResponse = "";
            int counta = 0;
            int countb = 0;

            try
            {

                if (Booking.Mode == "Online")
                {

                    List<GetBookingrootActiveOnline> dbsbookingroot = new List<GetBookingrootActiveOnline>();
                    GetBookingrootActiveOnline dashboardlist = new GetBookingrootActiveOnline { currentBookings = GetCurrentBookingsActive(Booking.MembershipCode, Booking.Mode, Booking.MobileNo), futureBookings = GetFutureBookingsActive(Booking.MembershipCode, Booking.Mode) };
                    dbsbookingroot.Add(dashboardlist);

                    counta = dashboardlist.currentBookings.Count;
                    countb = dashboardlist.futureBookings.Count;

                    if (counta > 0 || countb > 0)
                    {

                        FOutputBOnline.status = "success";
                        FOutputBOnline.value = dbsbookingroot;
                        sJSONResponse = JsonConvert.SerializeObject(FOutputBOnline);
                    }
                    else
                    {
                        FOutputBPlan.status = "success";
                        FOutputBPlan.value = "no bookings found";
                        sJSONResponse = JsonConvert.SerializeObject(FOutputBPlan);
                    }
                }
                else
                {


                    List<GetBookingrootActive> dbsbookingroot = new List<GetBookingrootActive>();
                    GetBookingrootActive dashboardlist = new GetBookingrootActive { currentInfo = GetCurrentFacilityOptes(Booking.MembershipCode), currentBookings = GetCurrentBookingsActive(Booking.MembershipCode, Booking.Mode, Booking.MobileNo), futureInfo = GetFutureFacilityOptes(Booking.MembershipCode), futureBookings = GetFutureBookingsActive(Booking.MembershipCode, Booking.Mode) };
                    dbsbookingroot.Add(dashboardlist);

                    counta = dashboardlist.currentBookings.Count;
                    countb = dashboardlist.futureBookings.Count;

                    if (counta > 0 || countb > 0)
                    {

                        FOutputB.status = "success";
                        FOutputB.value = dbsbookingroot;
                        sJSONResponse = JsonConvert.SerializeObject(FOutputB);
                    }
                    else
                    {
                        FOutputBPlan.status = "success";
                        FOutputBPlan.value = "no bookings found";
                        sJSONResponse = JsonConvert.SerializeObject(FOutputBPlan);
                    }
                }

            }
            catch (Exception ec)
            {

                FOutputBPlan.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(FOutputBPlan);
            }


            return sJSONResponse;
        }
        public Object GetBookingActive([FromBody]BookingRequestActive Booking)
        {
            ActiveBooking AB = new ActiveBooking();
            DataSet ds_SlotBooking = new DataSet();
            string sJSONResponse = "";

            try
            {

                List<GetBookingActive> ActiveBooking = new List<GetBookingActive>();
                GetBookingActive BookingActive = new GetBookingActive { currentInfo = CurrentInfo(Booking.MembershipCode, Booking.Mode, Booking.MobileNo), futureInfo = FutureInfo(Booking.MembershipCode, Booking.Mode) };
                ActiveBooking.Add(BookingActive);

                AB.status = "success";
                AB.value = ActiveBooking;

                sJSONResponse = JsonConvert.SerializeObject(AB);
            }
            catch (Exception ec)
            {
                ds_SlotBooking.Tables.Clear();
                ds_SlotBooking.Tables.Add(ReturnException("Fail", ""));
            }


            return sJSONResponse;
        }
        public List<GetBookingActive> GetBookingActive(string MemberShipCode)
        {
            List<GetBookingActive> gba = new List<GetBookingActive>();
            try
            {
                gba.Add(new GetBookingActive
                {
                    currentInfo = null
                    ,
                    futureInfo = null
                });
            }
            catch (Exception ec)
            {

            }
            return gba;

        }
        public List<CurrentInfo> CurrentInfo(string MemberShipCode, string Mode, string MobileNo)
        {
            List<CurrentInfo> ci = new List<CurrentInfo>();

            try
            {
                ci.Add(new CurrentInfo
                {
                    CurrentFacilityInfo = GetCurrentFacilityOptes(MemberShipCode)
                    ,
                    CurrentBooking = GetCurrentBookingsActive(MemberShipCode, Mode, MobileNo)
                });
            }
            catch (Exception ec)
            {

            }
            return ci;

        }
        public Dictionary<string, Dictionary<string, object>> GetCurrentFacilityOptes(string MembershipCode)
        {
            Dictionary<string, Dictionary<string, object>> DiscConverter = new Dictionary<string, Dictionary<string, object>>();

            FacilityDetailsOutput fdop = new FacilityDetailsOutput();
            DataSet SelectedSlots = new DataSet();
            DataTable dt_CCRMMembershipFacility = new DataTable();

            DataTable dt_FacilityOpted = new DataTable();
            DataTable dt_Facility = new DataTable();
            DataTable dt_MembersFacilityCount = new DataTable();
            DataTable dt_FacilityFreezingUsed = new DataTable();
            DataTable dt_FacilityFreezingBreaked = new DataTable();


            DataTable dt_Freezing = new DataTable();
            DataTable dt_Upgrade = new DataTable();
            DataTable dt_Change = new DataTable();
            DataTable dt_Transfer = new DataTable();
            DataTable dt_Paused = new DataTable();
            DataTable dt_Convert = new DataTable();

            List<FacilityOptsBooking> foptes = new List<FacilityOptsBooking>();
            DataTable dt_MembersFacility = new DataTable();

            int Facilityused_f = 0;
            int FacilityBreaked_f = 0;

            int Freezing_f = 0;
            int Upgrade_f = 0;
            int Change_f = 0;
            int Transfer_f = 0;
            int Paused_f = 0;
            int Convert_f = 0;

            int Grace_f = 0;
            int Hold_f = 0;
            int Extend_f = 0;


            int AllocatedDays = 0;
            int UsedDays = 0;
            int RemainingDays = 0;
            //

            try
            {

                DataTable dt_FacilityMaster = new DataTable();
                dt_FacilityMaster = getdata(string.Format("select SMFMID,SubSchemName from SubMembersFacilityMaster"));

                DataTable dt_TicketTypes = new DataTable();
                dt_TicketTypes = getdata(string.Format("select distinct ID,RequestType from TicketTypes"));

                DataTable dt_FacilityUsed = new DataTable();
                dt_FacilityUsed = getdata(string.Format("select MFDID=3,COUNT(*) as count from CCRMMembershipFacility where MemberShipCode='{0}' and MFDID=3 and FacilityExpireDate > GETDATE() union all select MFDID=4,COUNT(*) as count from CCRMMembershipFacility where MemberShipCode='{0}' and MFDID=4 and FacilityExpireDate > GETDATE() union all select MFDID=5,COUNT(*) as count from CCRMMembershipFacility where MemberShipCode='{0}' and MFDID=5 and FacilityExpireDate > GETDATE() union all select MFDID=6,COUNT(*) as count from CCRMMembershipFacility where MemberShipCode='{0}' and MFDID=6 and FacilityExpireDate > GETDATE()", MembershipCode));


                DataTable dt_MDetails = new DataTable();
                dt_MDetails = getdata(string.Format("select top 1 CCRMM.MembershipCode,FAI.InvoiceID,CCRMM.BranchCode,CMSSS.SessionCode,CMSS.SessionName,CCRMM.SlotCode,CMSSS.SlotName,CCRMM.PackageCode,CMSPS.PackageName,CCRMM.PlanCode,CMSP.PlanName,CCRMM.PlanCostCode,CCRMM.DurationId  from FAInvoice FAI, CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,CMSSESSIONTIMESETTING CMSS,CMSPLAN CMSP,CMSPACKAGES CMSPS where CCRMM.PlanCode=CMSP.PlanCode and CMSS.SessionCode=CMSSS.SessionCode and CCRMM.SlotCode=CMSSS.SlotCode and CCRMM.PackageCode=CMSPS.PackageCode and FAI.InvoiceID=CCRMM.InvoiceID and FAI.InvoiceID =(select InvoiceID from CCRMMembership where  MembershipCode='"+MembershipCode+"' and GETDATE() > StartDate and GETDATE() <= MembershipExpireDate)   "));

                dt_MembersFacilityCount = getdata(string.Format("select distinct count(*) as count from CCRMMembershipFacility where  FacilityStartDate < GETDATE() and FacilityExpireDate > GETDATE() and InvoiceID=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() > StartDate and GETDATE() <= MembershipExpireDate)", MembershipCode));
                //dt_FacilityOpted = getdata(string.Format("select SMFDID,FacilityName,Opted from FacilityOpted where Invoice=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() > StartDate and GETDATE() <= MembershipExpireDate)", MembershipCode));
                dt_FacilityOpted = getdata(string.Format("select SMFMID as SMFDID ,SubSchemName as FacilityName,Opted=1 from SubMembersFacilityMaster"));

                try
                {
                    if (dt_FacilityOpted != null && dt_FacilityOpted.Rows.Count > 0)
                    //- opt concept is removed 
                    //if (dt_FacilityOpted != null)
                    {
                        if (Convert.ToInt32(dt_MembersFacilityCount.Rows[0]["count"]) > 0)
                        {
                            Freezing_f = 2;
                            if (dt_FacilityOpted != null && dt_FacilityOpted.Rows.Count > 0)
                            {
                                Upgrade_f = Convert.ToInt32(dt_FacilityOpted.Rows[0]["Opted"]);
                                Change_f = Convert.ToInt32(dt_FacilityOpted.Rows[1]["Opted"]);
                                Transfer_f = Convert.ToInt32(dt_FacilityOpted.Rows[3]["Opted"]);
                                Paused_f = 0;
                                Convert_f = 0;
                                Hold_f = 0;

                            }
                            else
                            {
                                Upgrade_f = Convert.ToInt32(dt_FacilityUsed.Rows[0]["count"].ToString());
                                Change_f = Convert.ToInt32(dt_FacilityUsed.Rows[1]["count"].ToString());
                                Transfer_f = Convert.ToInt32(dt_FacilityUsed.Rows[2]["count"].ToString());
                                Paused_f = 0;
                                Convert_f = 0;
                                Hold_f = 0;
                            }
                        }
                        else
                        {
                            //dt_MembersFacility = getdata(string.Format("select CCRMMF.InvoiceID,CCRMMF.PlanCostCode,CMSPC.FreezingID,FF.NoOfDays,SUM(CCRMMF.FreezingDays) as FreezingDays from CMSPLANCOST CMSPC,FreezingFacility FF,CCRMMembershipFacility CCRMMF where FF.FreezingID=CMSPC.FreezingID and CCRMMF.MFDID=1 and CCRMMF.PlanCostCode=CMSPC.PlanCostCode   and CCRMMF.InvoiceID=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() > StartDate and GETDATE() <= MembershipExpireDate) group by CCRMMF.InvoiceID,CCRMMF.PlanCostCode,CMSPC.FreezingID,FF.NoOfDays ", MembershipCode));

                            dt_MembersFacility = getdata(string.Format("select CMSPC.PlanCostCode,CMSPC.FreezingID,FF.NoOfDays from CMSPLANCOST CMSPC,FreezingFacility FF where FF.ID=CMSPC.FreezingID and FF.SMFMID=1  and CMSPC.PlanCostCode=(select PlanCostCode from CCRMMembership where  MembershipCode='{0}' and GETDATE() > StartDate and GETDATE() <= MembershipExpireDate) group by cmspc.PlanCostCode,CMSPC.FreezingID,FF.NoOfDays ", MembershipCode));
                            dt_FacilityFreezingUsed = getdata(string.Format("select SUM(CCRMMF.FreezingDays) as FreezingDays from CMSPLANCOST CMSPC,MembersFacilityFreezing MFF,CCRMMembershipFacility CCRMMF where MFF.FreezingID=CMSPC.FreezingID and CCRMMF.MFDID=1 and CCRMMF.PlanCostCode=CMSPC.PlanCostCode   and CCRMMF.InvoiceID=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() > StartDate and GETDATE() <= MembershipExpireDate) and ISNULL(FacilityExpireDate,0)!=0 group by CCRMMF.InvoiceID", MembershipCode));
                            dt_FacilityFreezingBreaked = getdata(string.Format("select SUM(CCRMMF.FreezingDays) as FreezingDays from CMSPLANCOST CMSPC,MembersFacilityFreezing MFF,CCRMMembershipFacility CCRMMF where MFF.FreezingID=CMSPC.FreezingID and CCRMMF.MFDID=1 and CCRMMF.PlanCostCode=CMSPC.PlanCostCode   and CCRMMF.InvoiceID=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() > StartDate and GETDATE() <= MembershipExpireDate) and ISNULL(FacilityExpireDate,0)=0 group by CCRMMF.InvoiceID", MembershipCode));
                            if (dt_FacilityFreezingUsed != null && dt_FacilityFreezingBreaked != null && dt_FacilityFreezingUsed.Rows.Count > 0 && dt_FacilityFreezingBreaked.Rows.Count > 0)
                            {
                                Facilityused_f = Convert.ToInt32(dt_FacilityFreezingUsed.Rows[0]["FreezingDays"].ToString());
                                FacilityBreaked_f = Convert.ToInt32(dt_FacilityFreezingBreaked.Rows[0]["FreezingDays"].ToString());
                                AllocatedDays = Convert.ToInt32(dt_MembersFacility.Rows[0]["NoOfDays"].ToString());
                                UsedDays = Facilityused_f - FacilityBreaked_f;
                            }
                            else
                            {

                                AllocatedDays = Convert.ToInt32(dt_MembersFacility.Rows[0]["NoOfDays"].ToString());
                                UsedDays = Facilityused_f - FacilityBreaked_f;
                            }

                            RemainingDays = AllocatedDays - UsedDays;

                            if (dt_FacilityOpted != null && dt_FacilityOpted.Rows.Count > 0)
                            {

                                if (RemainingDays > 0)
                                {
                                    Freezing_f = 1;
                                }
                                else
                                {
                                    Freezing_f = 0;
                                }

                                Upgrade_f = Convert.ToInt32(dt_FacilityOpted.Rows[0]["Opted"]);
                                Change_f = Convert.ToInt32(dt_FacilityOpted.Rows[1]["Opted"]);
                                Transfer_f = Convert.ToInt32(dt_FacilityOpted.Rows[3]["Opted"]);
                                Paused_f = 0;
                                Convert_f = 1;
                                Hold_f = 1;
                            }
                            else
                            {

                                if (RemainingDays > 0)
                                {
                                    Freezing_f = 1;
                                }
                                else
                                {
                                    Freezing_f = 0;
                                }

                                Upgrade_f = Convert.ToInt32(dt_FacilityUsed.Rows[0]["count"].ToString());
                                Change_f = Convert.ToInt32(dt_FacilityUsed.Rows[1]["count"].ToString());
                                Transfer_f = Convert.ToInt32(dt_FacilityUsed.Rows[2]["count"].ToString());
                                Paused_f = 0;
                                Convert_f = 1;
                                Hold_f = 1;
                            }
                        }
                    }
                    else
                    {

                    }

                    //Dictionary<string, string> FreezingInfo = new Dictionary<string, string>();
                    //FreezingInfo.Add("MFIDID", "");
                    //FreezingInfo.Add("LeftOutFreezingDays", "");
                    //FreezingInfo.Add("IsFreezingUsed", "");

                    // PaidAmount
                    double FinalAmount = 0.0f;
                    double FinalAmount2 = 0.0f;
                    double IGST = 0.0f;
                    double FinalAmount1 = 0.0f;

                    double Transferfee = 0.0f;
                    double AmountPerDay = 0.0f;
                    double LeftOutDays = 0.0d;
                    double TransferLeftOutDays = 0.0d;
                    double CompleteDays = 0.0d;
                    double TotalDays = 0.0d;
                    double RemainingAmount = 0.0d;

                    ArrayList dates = MembersLatestDatesByInvoice(dt_MDetails.Rows[0]["InvoiceID"].ToString());
                    DateTime MembersStartDate = Convert.ToDateTime(dates[0].ToString());
                    DateTime MembersEndDate = Convert.ToDateTime(dates[1].ToString());
                    DateTime MembersEndDate2 = Convert.ToDateTime(dates[3].ToString());
                    double Duration = Convert.ToInt32(dates[2].ToString());
                    ArrayList Amount = MembersLatestAmountByInvoice(dt_MDetails.Rows[0]["InvoiceID"].ToString());

                    DateTime CurrentDate = DateTime.Now;

                    if (MembersEndDate > CurrentDate)
                    {
                        CompleteDays = (CurrentDate - MembersStartDate).TotalDays;
                        LeftOutDays = (MembersEndDate2 - CurrentDate).TotalDays;
                        TotalDays = Duration * 30;
                        TransferLeftOutDays = (MembersEndDate - CurrentDate).TotalDays;
                    }

                    FinalAmount = Convert.ToDouble(Amount[0].ToString());
                    IGST = Convert.ToDouble(Amount[1].ToString());

                    if (LeftOutDays < 1)
                        LeftOutDays = 0;
                    //if (LeftOutDays > TotalDays)
                    //    LeftOutDays = TotalDays;
                    if (CompleteDays < 1)
                        CompleteDays = 0;

                    if (FinalAmount > 0 && IGST > 0)
                    {

                        FinalAmount1 = Convert.ToDouble(FinalAmount);
                        FinalAmount2 = FinalAmount1 - Transferfee;
                        if (TotalDays > 0)
                        {
                            AmountPerDay = Math.Round(FinalAmount2 / TotalDays);
                            RemainingAmount = Math.Round((AmountPerDay * LeftOutDays));
                        }
                    }


                    Dictionary<string, object> FreezingDic = new Dictionary<string, object>();
                    FreezingDic.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[0][0]));
                    FreezingDic.Add("RequestTypeId", Convert.ToInt32(dt_TicketTypes.Rows[0][0]));
                    FreezingDic.Add("LeftOutFreezingDays", Convert.ToInt32(RemainingDays));
                    FreezingDic.Add("IsFreezingUsed", Freezing_f);

                    Dictionary<string, object> UpgradeDic = new Dictionary<string, object>();
                    UpgradeDic.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[2][0]));
                    UpgradeDic.Add("RequestTypeId", Convert.ToInt32(dt_TicketTypes.Rows[2][0]));
                    UpgradeDic.Add("LeftOutDays", Math.Round(LeftOutDays));
                    UpgradeDic.Add("RemainingAmount", Math.Round(RemainingAmount));
                    UpgradeDic.Add("IsUpgradeUsed", Upgrade_f);

                    Dictionary<string, object> ChangeDis = new Dictionary<string, object>();
                    ChangeDis.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[3][0]));
                    ChangeDis.Add("RequestTypeId", Convert.ToInt32(dt_TicketTypes.Rows[3][0]));
                    ChangeDis.Add("LeftOutDays", Math.Round(LeftOutDays));
                    ChangeDis.Add("RemainingAmount", Math.Round(RemainingAmount));
                    ChangeDis.Add("IsChangeUsed", Change_f);

                    Dictionary<string, object> TransferDis = new Dictionary<string, object>();
                    TransferDis.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[4][0]));
                    TransferDis.Add("RequestTypeId", Convert.ToInt32(dt_TicketTypes.Rows[4][0]));
                    TransferDis.Add("LeftOutDays", Math.Round(LeftOutDays));
                    TransferDis.Add("TransferLeftOutDays", Math.Round(TransferLeftOutDays));
                    TransferDis.Add("RemainingAmount", Math.Round(RemainingAmount));
                    TransferDis.Add("IsTransferUsed", Transfer_f);

                    Dictionary<string, object> HoldDis = new Dictionary<string, object>();
                    HoldDis.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[7][0]));
                    HoldDis.Add("RequestTypeId", Convert.ToInt32(dt_TicketTypes.Rows[7][0]));
                    HoldDis.Add("LeftOutDays", Math.Round(LeftOutDays));
                    HoldDis.Add("RemainingAmount", Math.Round(RemainingAmount));
                    HoldDis.Add("IsHoldUsed", Hold_f);

                    Dictionary<string, object> ConvertDis = new Dictionary<string, object>();
                    ConvertDis.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[8][0]));
                    ConvertDis.Add("RequestTypeId", Convert.ToInt32(dt_TicketTypes.Rows[8][0]));
                    ConvertDis.Add("LeftOutDays", Math.Round(LeftOutDays));
                    ConvertDis.Add("RemainingAmount", Math.Round(RemainingAmount));
                    ConvertDis.Add("IsConvertUsed", Convert_f);



                    //foptes.Add(new FacilityOptsBooking
                    //{

                    //    FreezingInfo = FreezingDic
                    //    ,
                    //    UpgradeInfo = UpgradeDic
                    //    ,
                    //    ChangeInfo = ChangeDis
                    //    ,
                    //    TransferInfo = TransferDis
                    //    ,
                    //    ConvertInfo = ConvertDis


                    //});

                    DiscConverter.Add("FreezingInfo", FreezingDic);
                    DiscConverter.Add("UpgradeInfo", UpgradeDic);
                    DiscConverter.Add("ChangeInfo", ChangeDis);
                    DiscConverter.Add("TransferInfo", TransferDis);
                    DiscConverter.Add("ConvertInfo", ConvertDis);

                    DiscConverter.Add("HoldInfo", HoldDis);

                }
                catch (Exception ec)
                {

                }
            }
            catch (Exception ec)
            {

            }

            return DiscConverter;
        }
        public List<FacilityOptsBooking> GetCurrentFacilityOptesOld(string MembershipCode)
        {
            FacilityDetailsOutput fdop = new FacilityDetailsOutput();
            DataSet SelectedSlots = new DataSet();
            DataTable dt_CCRMMembershipFacility = new DataTable();

            DataTable dt_FacilityOpted = new DataTable();
            DataTable dt_Facility = new DataTable();
            DataTable dt_MembersFacilityCount = new DataTable();
            DataTable dt_FacilityFreezingUsed = new DataTable();
            DataTable dt_FacilityFreezingBreaked = new DataTable();


            DataTable dt_Freezing = new DataTable();
            DataTable dt_Upgrade = new DataTable();
            DataTable dt_Change = new DataTable();
            DataTable dt_Transfer = new DataTable();
            DataTable dt_Paused = new DataTable();
            DataTable dt_Convert = new DataTable();


            List<FacilityOptsBooking> foptes = new List<FacilityOptsBooking>();
            DataTable dt_MembersFacility = new DataTable();

            int Facilityused_f = 0;
            int FacilityBreaked_f = 0;
            int Freezing_f = 0;
            int Upgrade_f = 0;
            int Change_f = 0;
            int Transfer_f = 0;
            int Paused_f = 0;
            int Convert_f = 0;

            int AllocatedDays = 0;
            int UsedDays = 0;
            int RemainingDays = 0;
            //

            try
            {

                DataTable dt_FacilityMaster = new DataTable();
                dt_FacilityMaster = getdata(string.Format("select SMFMID,SubSchemName from SubMembersFacilityMaster"));


                DataTable dt_MDetails = new DataTable();
                dt_MDetails = getdata(string.Format("select top 1 FAI.MembershipCode,FAI.InvoiceID,CCRMM.BranchCode,CMSSS.SessionCode,CMSS.SessionName,CCRMM.SlotCode,CMSSS.SlotName,CCRMM.PackageCode,CMSPAC.PackageName,CCRMM.PlanCode,CMSP.PlanName,CMSPAC.PlanCostCode,CCRMM.DurationId  from FAInvoice FAI, CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,CMSPACKAGESCOST CMSPAC,CMSSESSIONTIMESETTING CMSS,CMSPLAN CMSP where CCRMM.PlanCode=CMSP.PlanCode and CMSS.SessionCode=CMSSS.SessionCode and CCRMM.SlotCode=CMSSS.SlotCode and CCRMM.PackageCode=CMSPAC.PackageCode and  FAI.InvoiceID =(select InvoiceID from CCRMMembership where  MembershipCode='" + MembershipCode + "' and GETDATE() > StartDate and GETDATE() <= MembershipExpireDate)  "));

                dt_MembersFacilityCount = getdata(string.Format("select distinct count(*) as count from CCRMMembershipFacility where  FacilityStartDate < GETDATE() and FacilityExpireDate > GETDATE() and InvoiceID=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() > StartDate and GETDATE() <= MembershipExpireDate)", MembershipCode));
                dt_FacilityOpted = getdata(string.Format("select SMFDID,FacilityName,Opted from FacilityOpted where Invoice=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() > StartDate and GETDATE() <= MembershipExpireDate)", MembershipCode));


                try
                {
                    if (dt_FacilityOpted != null && dt_FacilityOpted.Rows.Count > 0)
                    {
                        if (Convert.ToInt32(dt_MembersFacilityCount.Rows[0]["count"]) > 0)
                        {
                            Freezing_f = 2;
                            if (dt_FacilityOpted != null && dt_FacilityOpted.Rows.Count > 0)
                            {
                                Upgrade_f = Convert.ToInt32(dt_FacilityOpted.Rows[0]["Opted"]);
                                Change_f = Convert.ToInt32(dt_FacilityOpted.Rows[1]["Opted"]);
                                Transfer_f = Convert.ToInt32(dt_FacilityOpted.Rows[3]["Opted"]);
                                Paused_f = Convert.ToInt32(dt_FacilityOpted.Rows[6]["Opted"]);
                                Convert_f = Convert.ToInt32(dt_FacilityOpted.Rows[7]["Opted"]);
                            }
                            else
                            {
                                Upgrade_f = 1;
                                Change_f = 1;
                                Transfer_f = 1;
                                Paused_f = 1;
                                Convert_f = 1;
                            }
                        }
                        else
                        {
                            dt_MembersFacility = getdata(string.Format("select CCRMMF.InvoiceID,CCRMMF.PlanCostCode,CMSPC.FreezingID,FF.NoOfDays,SUM(CCRMMF.FreezingDays) as FreezingDays from CMSPLANCOST CMSPC,FreezingFacility FF,CCRMMembershipFacility CCRMMF where FF.FreezingID=CMSPC.FreezingID and CCRMMF.MFDID=1 and CCRMMF.PlanCostCode=CMSPC.PlanCostCode   and CCRMMF.InvoiceID=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() > StartDate and GETDATE() <= MembershipExpireDate) group by CCRMMF.InvoiceID,CCRMMF.PlanCostCode,CMSPC.FreezingID,FF.NoOfDays ", MembershipCode));
                            dt_FacilityFreezingUsed = getdata(string.Format("select SUM(CCRMMF.FreezingDays) as FreezingDays from CMSPLANCOST CMSPC,MembersFacilityFreezing MFF,CCRMMembershipFacility CCRMMF where MFF.FreezingID=CMSPC.FreezingID and CCRMMF.MFDID=1 and CCRMMF.PlanCostCode=CMSPC.PlanCostCode   and CCRMMF.InvoiceID=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() > StartDate and GETDATE() <= MembershipExpireDate) and ISNULL(FacilityExpireDate,0)!=0 group by CCRMMF.InvoiceID", MembershipCode));
                            dt_FacilityFreezingBreaked = getdata(string.Format("select SUM(CCRMMF.FreezingDays) as FreezingDays from CMSPLANCOST CMSPC,MembersFacilityFreezing MFF,CCRMMembershipFacility CCRMMF where MFF.FreezingID=CMSPC.FreezingID and CCRMMF.MFDID=1 and CCRMMF.PlanCostCode=CMSPC.PlanCostCode   and CCRMMF.InvoiceID=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() > StartDate and GETDATE() <= MembershipExpireDate) and ISNULL(FacilityExpireDate,0)=0 group by CCRMMF.InvoiceID", MembershipCode));
                            if (dt_FacilityFreezingUsed != null && dt_FacilityFreezingBreaked != null && dt_FacilityFreezingUsed.Rows.Count > 0 && dt_FacilityFreezingBreaked.Rows.Count > 0)
                            {
                                Facilityused_f = Convert.ToInt32(dt_FacilityFreezingUsed.Rows[0]["FreezingDays"].ToString());
                                FacilityBreaked_f = Convert.ToInt32(dt_FacilityFreezingBreaked.Rows[0]["FreezingDays"].ToString());
                                AllocatedDays = Convert.ToInt32(dt_MembersFacility.Rows[0]["NoOfDays"].ToString());
                                UsedDays = Facilityused_f - FacilityBreaked_f;
                            }
                            else
                            {

                                AllocatedDays = Convert.ToInt32(dt_MembersFacility.Rows[0]["NoOfDays"].ToString());
                                UsedDays = AllocatedDays;
                            }

                            RemainingDays = AllocatedDays - UsedDays;

                            if (dt_FacilityOpted != null && dt_FacilityOpted.Rows.Count > 0)
                            {

                                if (RemainingDays > 0)
                                {
                                    Freezing_f = 1;
                                }
                                else
                                {
                                    Freezing_f = 0;
                                }

                                Upgrade_f = Convert.ToInt32(dt_FacilityOpted.Rows[0]["Opted"]);
                                Change_f = Convert.ToInt32(dt_FacilityOpted.Rows[1]["Opted"]);
                                Transfer_f = Convert.ToInt32(dt_FacilityOpted.Rows[3]["Opted"]);
                                Paused_f = Convert.ToInt32(dt_FacilityOpted.Rows[6]["Opted"]);
                                Convert_f = Convert.ToInt32(dt_FacilityOpted.Rows[7]["Opted"]);
                            }
                            else
                            {

                                if (RemainingDays > 0)
                                {
                                    Freezing_f = 1;
                                }
                                else
                                {
                                    Freezing_f = 0;
                                }

                                Upgrade_f = 1;
                                Change_f = 1;
                                Transfer_f = 1;
                                Paused_f = 1;
                                Convert_f = 1;
                            }
                        }
                    }
                    else
                    {

                    }



                    // PaidAmount
                    double FinalAmount = 0.0f;
                    double FinalAmount2 = 0.0f;
                    double IGST = 0.0f;
                    double FinalAmount1 = 0.0f;

                    double Transferfee = 0.0f;
                    double AmountPerDay = 0.0f;
                    double LeftOutDays = 0.0d;
                    double CompleteDays = 0.0d;
                    double TotalDays = 0.0d;
                    double RemainingAmount = 0.0d;

                    ArrayList dates = MembersLatestDatesByInvoice(dt_MDetails.Rows[0]["InvoiceID"].ToString());
                    DateTime MembersStartDate = Convert.ToDateTime(dates[0].ToString());
                    DateTime MembersEndDate = Convert.ToDateTime(dates[1].ToString());
                    double Duration = Convert.ToInt32(dates[2].ToString());
                    ArrayList Amount = MembersLatestAmountByInvoice(dt_MDetails.Rows[0]["InvoiceID"].ToString());

                    DateTime CurrentDate = DateTime.Now;

                    if (MembersEndDate > CurrentDate)
                    {
                        CompleteDays = (CurrentDate - MembersStartDate).TotalDays;
                        LeftOutDays = (MembersEndDate - CurrentDate).TotalDays;
                        TotalDays = Duration * 30;
                    }

                    FinalAmount = Convert.ToDouble(Amount[0].ToString());
                    IGST = Convert.ToDouble(Amount[1].ToString());

                    if (LeftOutDays < 1)
                        LeftOutDays = 0;
                    if (LeftOutDays > TotalDays)
                        LeftOutDays = TotalDays;
                    if (CompleteDays < 1)
                        CompleteDays = 0;



                    if (FinalAmount > 0 && IGST > 0)
                    {

                        FinalAmount1 = Convert.ToDouble(FinalAmount);
                        FinalAmount2 = FinalAmount1 - Transferfee;
                        if (TotalDays > 0)
                        {
                            AmountPerDay = Math.Round(FinalAmount2 / TotalDays);
                            RemainingAmount = Math.Round((AmountPerDay * LeftOutDays));
                        }
                    }



                    Dictionary<string, object> FreezingDic = new Dictionary<string, object>();
                    FreezingDic.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[0][0]));
                    FreezingDic.Add("LeftOutFreezingDays", Math.Round(LeftOutDays));
                    FreezingDic.Add("IsFreezingUsed", Freezing_f);

                    Dictionary<string, object> UpgradeDic = new Dictionary<string, object>();
                    UpgradeDic.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[2][0]));
                    UpgradeDic.Add("LeftOutDays", Math.Round(LeftOutDays));
                    UpgradeDic.Add("RemainingAmount", Math.Round(RemainingAmount));
                    UpgradeDic.Add("IsUpgradeUsed", Upgrade_f);

                    Dictionary<string, object> ChangeDis = new Dictionary<string, object>();
                    ChangeDis.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[3][0]));
                    ChangeDis.Add("LeftOutDays", Math.Round(LeftOutDays));
                    ChangeDis.Add("RemainingAmount", Math.Round(RemainingAmount));
                    ChangeDis.Add("IsChangeUsed", Freezing_f);

                    Dictionary<string, object> TransferDis = new Dictionary<string, object>();
                    TransferDis.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[4][0]));
                    TransferDis.Add("LeftOutDays", Math.Round(LeftOutDays));
                    TransferDis.Add("RemainingAmount", Math.Round(RemainingAmount));
                    TransferDis.Add("IsTransferUsed", Freezing_f);

                    Dictionary<string, object> ConvertDis = new Dictionary<string, object>();
                    ConvertDis.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[7][0]));
                    ConvertDis.Add("LeftOutDays", Math.Round(LeftOutDays));
                    ConvertDis.Add("RemainingAmount", Math.Round(RemainingAmount));
                    ConvertDis.Add("IsConvertUsed", Freezing_f);


                    foptes.Add(new FacilityOptsBooking
                    {

                        FreezingInfo = FreezingDic
                        ,
                        UpgradeInfo = UpgradeDic
                        ,
                        ChangeInfo = ChangeDis
                        ,
                        TransferInfo = TransferDis
                        ,
                        ConvertInfo = ConvertDis


                    });

                }
                catch (Exception ec)
                {

                }
            }
            catch (Exception ec)
            {

            }
            return foptes;
        }
        public Dictionary<string, object> GetCurrentBookingsActive(string MemberShipCode, string Mode, string MobileNo)
        {
            List<GetCurrentBookingsActive> ftd = new List<GetCurrentBookingsActive>();
            DataTable dt = new DataTable();
            Dictionary<string, object> CurrentBookingActive = new Dictionary<string, object>();
            //select OLP.PackageName,PG.TransactionDate as InvoiceDate,PG.OrderId,PG.Amount as PaidAmount,PG.Invoice as InvoiceID,OLPP.StartDate,OLPP.EndDate from OnlinePackagePurchase OLPP,PaymentGateway PG,OnlinePackages OLP where  OLPP.Invoice=PG.Invoice and OLP.PackageID=OLPP.PackageID and OLPP.MembershipCode='{0}' and OLPP.EndDate >= GETDATE() and OLPP.StartDate < GETDATE()  union all  select CMSP.PackageName,FAI.PaymentDate as InvoiceDate,FAI.Receipt as OrderID,(FAI.PayableAmount+FAI.PayableAmount2) as PaidAmount,FAI.InvoiceID,CCRMM.StartDate,CCRMM.MembershipExpireDate as EndDate from CCRMMembership CCRMM,FAInvoice FAI,CMSPACKAGES CMSP where CCRMM.InvoiceID=FAI.InvoiceID  and CMSP.PackageCode=CCRMM.PackageCode and CCRMM.MembershipCode='{0}' and CCRMM.MembershipExpireDate >= GETDATE() and CCRMM.StartDate < GETDATE() order by InvoiceDate desc

            try
            {
                if (Mode == "Online")
                {
                    dt = getdata(string.Format("select OLP.PackageName,PG.TransactionDate as InvoiceDate,PG.OrderId,PG.Amount as PaidAmount,PG.Invoice as InvoiceID,OLPP.StartDate,OLPP.EndDate,OLPP.BranchCode from OnlinePackagePurchase OLPP,PaymentGateway PG,OnlinePackages OLP where  OLPP.Invoice=PG.Invoice and OLP.PackageID=OLPP.PackageID and OLPP.MobileNo='{0}' and OLPP.EndDate >= GETDATE() and OLPP.StartDate <= GETDATE()", MobileNo));
                    if (dt.Rows.Count == 0)
                    {
                        dt = getdata(string.Format("select Top 1 PackageName='',InvoiceDate='',OrderID=0,PaidAmount=0,InvoiceID=0,StartDate='',MembershipExpireDate='',BranchCode from  OnlinePackagePurchase  where MobileNo='{0}'  ", MobileNo));

                    }


                }
                else
                {
                    dt = getdata(string.Format("select CMSP.PackageName,FAI.PaymentDate as InvoiceDate,FAI.Receipt as OrderID,(FAI.PayableAmount+FAI.PayableAmount2) as PaidAmount,FAI.InvoiceID,CCRMM.StartDate,CCRMM.MembershipExpireDate as EndDate,CCRMM.BranchCode from CCRMMembership CCRMM,FAInvoice FAI,CMSPACKAGES CMSP where CCRMM.InvoiceID=FAI.InvoiceID  and CMSP.PackageCode=CCRMM.PackageCode and CCRMM.MembershipCode='{0}' and CCRMM.MembershipExpireDate >= GETDATE() and CCRMM.StartDate < GETDATE() order by InvoiceDate desc ", MemberShipCode));

                    if (dt.Rows.Count == 0)
                    {
                        dt = getdata(string.Format("select PackageName = '', CreatedOn as InvoiceDate,OrderID = 0,PaidAmount = 0,InvoiceID,SessionDate as StartDate ,SessionDate as MembershipExpireDate,BranchCode from OffLineSlotUsed where MobileNo = '{0}' ", MobileNo));

                    }
                }



                if (dt.Rows.Count == 0)
                {
                    //CurrentBookingActive.Add("packageName", null);
                    //CurrentBookingActive.Add("invoiceDate", null);
                    //CurrentBookingActive.Add("orderID", null);
                    //CurrentBookingActive.Add("paidAmount", null);
                    //CurrentBookingActive.Add("invoiceId", null);
                    //CurrentBookingActive.Add("startDate", null);
                    //CurrentBookingActive.Add("EndDate", null);

                }
                else
                {
                    CurrentBookingActive.Add("packageName", Convert.ToString(dt.Rows[0]["PackageName"]));
                    CurrentBookingActive.Add("invoiceDate", Convert.ToString(dt.Rows[0]["InvoiceDate"]));
                    CurrentBookingActive.Add("orderID", Convert.ToString(dt.Rows[0]["OrderId"]));
                    CurrentBookingActive.Add("paidAmount", Convert.ToString(dt.Rows[0]["PaidAmount"]));
                    CurrentBookingActive.Add("invoiceId", Convert.ToString(dt.Rows[0]["InvoiceID"]));
                    CurrentBookingActive.Add("startDate", Convert.ToString(dt.Rows[0]["StartDate"]));
                    CurrentBookingActive.Add("EndDate", Convert.ToString(dt.Rows[0]["EndDate"]));
                    CurrentBookingActive.Add("branchCode", Convert.ToString(dt.Rows[0]["branchCode"]));


                }
            }
            catch (Exception ec)
            {

            }



            return CurrentBookingActive;

        }
        public List<FutureInfo> FutureInfo(string MemberShipCode, string Mode)
        {
            List<FutureInfo> fi = new List<FutureInfo>();
            try
            {
                fi.Add(new FutureInfo
                {
                    FutureFacilityInfo = GetFutureFacilityOptes(MemberShipCode)
                    ,
                    FutureBooking = GetFutureBookingsActive(MemberShipCode, Mode)
                });
            }
            catch (Exception ec)
            {

            }
            return fi;

        }
        public Dictionary<string, Dictionary<string, object>> GetFutureFacilityOptes(string MembershipCode)
        {
            Dictionary<string, Dictionary<string, object>> DiscConverter = new Dictionary<string, Dictionary<string, object>>();
            FacilityDetailsOutput fdop = new FacilityDetailsOutput();
            DataSet SelectedSlots = new DataSet();
            DataTable dt_CCRMMembershipFacility = new DataTable();

            DataTable dt_FacilityOpted = new DataTable();
            DataTable dt_Facility = new DataTable();
            DataTable dt_MembersFacilityCount = new DataTable();
            DataTable dt_FacilityFreezingUsed = new DataTable();
            DataTable dt_FacilityFreezingBreaked = new DataTable();


            DataTable dt_Freezing = new DataTable();
            DataTable dt_Upgrade = new DataTable();
            DataTable dt_Change = new DataTable();
            DataTable dt_Transfer = new DataTable();
            DataTable dt_Paused = new DataTable();
            DataTable dt_Convert = new DataTable();

            List<FacilityOptsBooking> foptes = new List<FacilityOptsBooking>();
            DataTable dt_MembersFacility = new DataTable();

            int Facilityused_f = 0;
            int FacilityBreaked_f = 0;
            int Freezing_f = 0;
            int Upgrade_f = 0;
            int Change_f = 0;
            int Transfer_f = 0;
            int Paused_f = 0;
            int Convert_f = 0;
            int Hold_f = 0;
            int AllocatedDays = 0;
            int UsedDays = 0;
            int RemainingDays = 0;

            try
            {

                DataTable dt_FacilityMaster = new DataTable();
                dt_FacilityMaster = getdata(string.Format("select SMFMID,SubSchemName from SubMembersFacilityMaster"));

                DataTable dt_FacilityUsed = new DataTable();
                dt_FacilityUsed = getdata(string.Format("select MFDID=3,COUNT(*) as count from CCRMMembershipFacility where MemberShipCode='{0}' and MFDID=3 and FacilityStartDate < GETDATE()  union all  select MFDID=4,COUNT(*) as count from CCRMMembershipFacility where MemberShipCode='{0}' and MFDID=4 and FacilityStartDate < GETDATE() union all  select MFDID=5,COUNT(*) as count from CCRMMembershipFacility where MemberShipCode='{0}' and MFDID=5 and FacilityStartDate < GETDATE() union all  select MFDID=6,COUNT(*) as count from CCRMMembershipFacility where MemberShipCode='{0}' and MFDID=6 and FacilityStartDate < GETDATE()",MembershipCode));

                DataTable dt_MDetails = new DataTable();
                dt_MDetails = getdata(string.Format("select top 1 FAI.MembershipCode,FAI.InvoiceID,CCRMM.BranchCode,CMSSS.SessionCode,CMSS.SessionName,CCRMM.SlotCode,CMSSS.SlotName,CCRMM.PackageCode,CMSPAC.PackageName,CCRMM.PlanCode,CMSP.PlanName,CMSPAC.PlanCostCode,CCRMM.DurationId  from FAInvoice FAI, CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,CMSPACKAGESCOST CMSPAC,CMSSESSIONTIMESETTING CMSS,CMSPLAN CMSP where CCRMM.PlanCode=CMSP.PlanCode and CMSS.SessionCode=CMSSS.SessionCode and CCRMM.SlotCode=CMSSS.SlotCode and CCRMM.PackageCode=CMSPAC.PackageCode and  FAI.InvoiceID =(select InvoiceID from CCRMMembership where  MembershipCode='" + MembershipCode + "' and GETDATE() < StartDate and GETDATE() <= MembershipExpireDate)  "));

                dt_MembersFacilityCount = getdata(string.Format("select distinct count(*) as count from CCRMMembershipFacility where  FacilityStartDate < GETDATE() and FacilityExpireDate > GETDATE() and InvoiceID=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() < StartDate and GETDATE() <= MembershipExpireDate)", MembershipCode));
                dt_FacilityOpted = getdata(string.Format("select SMFDID,FacilityName,Opted from FacilityOpted where Invoice=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() < StartDate and GETDATE() <= MembershipExpireDate)", MembershipCode));

                if (Convert.ToInt32(dt_MembersFacilityCount.Rows[0]["count"]) > 0)
                {
                    Freezing_f = 2;
                    if (dt_FacilityOpted != null && dt_FacilityOpted.Rows.Count > 0)
                    {
                        Upgrade_f = Convert.ToInt32(dt_FacilityOpted.Rows[0]["Opted"]);
                        Change_f = Convert.ToInt32(dt_FacilityOpted.Rows[1]["Opted"]);
                        Transfer_f = Convert.ToInt32(dt_FacilityOpted.Rows[3]["Opted"]);
                        Paused_f = Convert.ToInt32(dt_FacilityOpted.Rows[6]["Opted"]);
                        Convert_f = Convert.ToInt32(dt_FacilityOpted.Rows[7]["Opted"]);
                        Hold_f = 0;
                    }
                    else
                    {
                        Upgrade_f = Convert.ToInt32(dt_FacilityUsed.Rows[0]["count"].ToString());
                        Change_f = Convert.ToInt32(dt_FacilityUsed.Rows[1]["count"].ToString());
                        Transfer_f = Convert.ToInt32(dt_FacilityUsed.Rows[2]["count"].ToString());
                        Paused_f = 0;
                        Convert_f = 0;
                        Hold_f = 0;
                    }
                }
                else
                {
                    dt_MembersFacility = getdata(string.Format("select CCRMMF.InvoiceID,CCRMMF.PlanCostCode,CMSPC.FreezingID,FF.NoOfDays,SUM(CCRMMF.FreezingDays) as FreezingDays from CMSPLANCOST CMSPC,FreezingFacility FF,CCRMMembershipFacility CCRMMF where FF.ID=CMSPC.FreezingID and CCRMMF.MFDID=1 and CCRMMF.PlanCostCode=CMSPC.PlanCostCode   and CCRMMF.InvoiceID=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() < StartDate and GETDATE() <= MembershipExpireDate) group by CCRMMF.InvoiceID,CCRMMF.PlanCostCode,CMSPC.FreezingID,FF.NoOfDays Union all select CCRM.InvoiceID,CCRM.PlanCostCode,CMSPC.FreezingID,FF.NoOfDays,FreezingDays=0 from CMSPLANCOST CMSPC,FreezingFacility FF,CCRMMembership CCRM where FF.ID=CMSPC.FreezingID and FF.SMFMID=1 and CCRM.PlanCostCode=CMSPC.PlanCostCode   and CCRM.InvoiceID=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() < StartDate and GETDATE() <= MembershipExpireDate) group by CCRM.InvoiceID,CCRM.PlanCostCode,CMSPC.FreezingID,FF.NoOfDays  ", MembershipCode));
                    dt_FacilityFreezingUsed = getdata(string.Format("select SUM(CCRMMF.FreezingDays) as FreezingDays from CMSPLANCOST CMSPC,MembersFacilityFreezing MFF,CCRMMembershipFacility CCRMMF where MFF.FreezingID=CMSPC.FreezingID and CCRMMF.MFDID=1 and CCRMMF.PlanCostCode=CMSPC.PlanCostCode   and CCRMMF.InvoiceID=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() < StartDate and GETDATE() <= MembershipExpireDate) and ISNULL(FacilityExpireDate,0)!=0 group by CCRMMF.InvoiceID", MembershipCode));
                    dt_FacilityFreezingBreaked = getdata(string.Format("select SUM(CCRMMF.FreezingDays) as FreezingDays from CMSPLANCOST CMSPC,MembersFacilityFreezing MFF,CCRMMembershipFacility CCRMMF where MFF.FreezingID=CMSPC.FreezingID and CCRMMF.MFDID=1 and CCRMMF.PlanCostCode=CMSPC.PlanCostCode   and CCRMMF.InvoiceID=(select InvoiceID from CCRMMembership where  MembershipCode='{0}' and GETDATE() < StartDate and GETDATE() <= MembershipExpireDate) and ISNULL(FacilityExpireDate,0)=0 group by CCRMMF.InvoiceID", MembershipCode));
                    //dt_FacilityFreezingUsed != null  && dt_FacilityFreezingUsed.Rows.Count > 0 &&  
                    if (dt_FacilityFreezingBreaked != null && dt_FacilityFreezingBreaked.Rows.Count > 0)
                    {
                        Facilityused_f = Convert.ToInt32(dt_FacilityFreezingUsed.Rows[0]["FreezingDays"].ToString());
                        FacilityBreaked_f = Convert.ToInt32(dt_FacilityFreezingBreaked.Rows[0]["FreezingDays"].ToString());
                        AllocatedDays = Convert.ToInt32(dt_MembersFacility.Rows[0]["NoOfDays"].ToString());
                        UsedDays = Facilityused_f - FacilityBreaked_f;
                    }
                    else
                    {

                        AllocatedDays = Convert.ToInt32(dt_MembersFacility.Rows[0]["NoOfDays"].ToString());
                        UsedDays = 0;
                    }

                    RemainingDays = AllocatedDays - UsedDays;

                    if (dt_FacilityOpted != null && dt_FacilityOpted.Rows.Count > 0)
                    {

                        if (RemainingDays > 0)
                        {
                            Freezing_f = 1;
                        }
                        else
                        {
                            Freezing_f = 0;
                        }

                        Upgrade_f = Convert.ToInt32(dt_FacilityOpted.Rows[0]["Opted"]);
                        Change_f = Convert.ToInt32(dt_FacilityOpted.Rows[1]["Opted"]);
                        Transfer_f = Convert.ToInt32(dt_FacilityOpted.Rows[3]["Opted"]);
                        Paused_f = Convert.ToInt32(dt_FacilityOpted.Rows[6]["Opted"]);
                        Convert_f = Convert.ToInt32(dt_FacilityOpted.Rows[7]["Opted"]);
                    }
                    else
                    {

                        if (RemainingDays > 0)
                        {
                            Freezing_f = 1;
                        }
                        else
                        {
                            Freezing_f = 0;
                        }

                        Upgrade_f = Convert.ToInt32(dt_FacilityUsed.Rows[0]["count"].ToString());
                        Change_f = Convert.ToInt32(dt_FacilityUsed.Rows[1]["count"].ToString());
                        Transfer_f = Convert.ToInt32(dt_FacilityUsed.Rows[2]["count"].ToString());
                        Paused_f = 0;
                        Convert_f = 0;
                        Hold_f = 0;
                    }
                }


                // PaidAmount
                double FinalAmount = 0.0f;
                double FinalAmount2 = 0.0f;
                double IGST = 0.0f;
                double FinalAmount1 = 0.0f;

                double Transferfee = 0.0f;
                double AmountPerDay = 0.0f;
                double LeftOutDays = 0.0d;
                double CompleteDays = 0.0d;
                double TotalDays = 0.0d;
                double RemainingAmount = 0.0d;

                ArrayList dates = MembersFutureLatestDatesByInvoice(dt_MDetails.Rows[0]["InvoiceID"].ToString());
                DateTime MembersStartDate = Convert.ToDateTime(dates[0].ToString());
                DateTime MembersEndDate = Convert.ToDateTime(dates[1].ToString());
                double Duration = Convert.ToInt32(dates[2].ToString());
                ArrayList Amount = MembersLatestAmountByInvoice(dt_MDetails.Rows[0]["InvoiceID"].ToString());

                DateTime CurrentDate = DateTime.Now;

                if (MembersEndDate > CurrentDate)
                {
                    CompleteDays = (CurrentDate - MembersStartDate).TotalDays;
                    LeftOutDays = (MembersEndDate - CurrentDate).TotalDays;
                    TotalDays = Duration * 30;
                }

                FinalAmount = Convert.ToDouble(Amount[0].ToString());
                IGST = Convert.ToDouble(Amount[1].ToString());

                if (LeftOutDays < 1)
                    LeftOutDays = 0;
                if (LeftOutDays > TotalDays)
                    LeftOutDays = TotalDays;
                if (CompleteDays < 1)
                    CompleteDays = 0;



                if (FinalAmount > 0 && IGST > 0)
                {

                    FinalAmount1 = Convert.ToDouble(FinalAmount);
                    FinalAmount2 = FinalAmount1 - Transferfee;
                    if (TotalDays > 0)
                    {
                        AmountPerDay = Math.Round(FinalAmount2 / TotalDays);
                        RemainingAmount = Math.Round((AmountPerDay * LeftOutDays));
                    }
                }

                Dictionary<string, object> FreezingDic = new Dictionary<string, object>();
                FreezingDic.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[0][0]));
                FreezingDic.Add("LeftOutFreezingDays", Math.Round(LeftOutDays));
                FreezingDic.Add("IsFreezingUsed", Freezing_f);

                Dictionary<string, object> UpgradeDic = new Dictionary<string, object>();
                UpgradeDic.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[2][0]));
                UpgradeDic.Add("LeftOutDays", Math.Round(LeftOutDays));
                UpgradeDic.Add("RemainingAmount", Math.Round(RemainingAmount));
                UpgradeDic.Add("IsUpgradeUsed", Upgrade_f);

                Dictionary<string, object> ChangeDis = new Dictionary<string, object>();
                ChangeDis.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[3][0]));
                ChangeDis.Add("LeftOutDays", Math.Round(LeftOutDays));
                ChangeDis.Add("RemainingAmount", Math.Round(RemainingAmount));
                ChangeDis.Add("IsChangeUsed", Change_f);

                Dictionary<string, object> TransferDis = new Dictionary<string, object>();
                TransferDis.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[4][0]));
                TransferDis.Add("LeftOutDays", Math.Round(LeftOutDays));
                TransferDis.Add("RemainingAmount", Math.Round(RemainingAmount));
                TransferDis.Add("IsTransferUsed", Transfer_f);

                Dictionary<string, object> ConvertDis = new Dictionary<string, object>();
                ConvertDis.Add("MFIDID", Convert.ToInt32(dt_FacilityMaster.Rows[7][0]));
                ConvertDis.Add("LeftOutDays", Math.Round(LeftOutDays));
                ConvertDis.Add("RemainingAmount", Math.Round(RemainingAmount));
                ConvertDis.Add("IsConvertUsed", Convert_f);




                //foptes.Add(new FacilityOptsBooking
                //{

                //    FreezingInfo = FreezingDic
                //          ,
                //    UpgradeInfo = UpgradeDic
                //          ,
                //    ChangeInfo = ChangeDis
                //          ,
                //    TransferInfo = TransferDis
                //          ,
                //    ConvertInfo = ConvertDis


                //});

                DiscConverter.Add("FreezingInfo", FreezingDic);
                DiscConverter.Add("UpgradeInfo", UpgradeDic);
                DiscConverter.Add("ChangeInfo", ChangeDis);
                DiscConverter.Add("TransferInfo", TransferDis);
                DiscConverter.Add("ConvertInfo", ConvertDis);

            }
            catch (Exception ecp)
            {

            }
            return DiscConverter;
        }
        public Dictionary<string, object> GetFutureBookingsActive(string MemberShipCode, string Mode)
        {
            List<GetFutureBookingsActive> ftd = new List<GetFutureBookingsActive>();
            DataTable dt = new DataTable();
            Dictionary<string, object> FutureBookingActive = new Dictionary<string, object>();
            //dt = getdata(string.Format("select OLP.PackageName,PG.TransactionDate as InvoiceDate,PG.OrderId,PG.Amount as PaidAmount,PG.Invoice as InvoiceID from OnlinePackagePurchase OLPP,PaymentGateway PG,OnlinePackages OLP where  OLPP.Invoice=PG.Invoice and OLP.PackageID=OLPP.PackageID and OLPP.MembershipCode='{0}'  and OLPP.StartDate > GETDATE()  union all  select CMSP.PackageName,FAI.PaymentDate as InvoiceDate,FAI.Receipt as OrderID,(FAI.PayableAmount+FAI.PayableAmount2) as PaidAmount,FAI.InvoiceID from CCRMMembership CCRMM,FAInvoice FAI,CMSPACKAGES CMSP where CCRMM.InvoiceID=FAI.InvoiceID  and CMSP.PackageCode=CCRMM.PackageCode and CCRMM.MembershipCode='{0}' and  CCRMM.StartDate > GETDATE() order by InvoiceDate desc", MemberShipCode));
            //dt = getdata(string.Format("select OLP.PackageName,PG.TransactionDate as InvoiceDate,PG.OrderId,PG.Amount as PaidAmount,PG.Invoice as InvoiceID,OLPP.StartDate,OLPP.EndDate from OnlinePackagePurchase OLPP,PaymentGateway PG,OnlinePackages OLP where  OLPP.Invoice=PG.Invoice and OLP.PackageID=OLPP.PackageID and OLPP.MembershipCode='{0}' and OLPP.EndDate >= GETDATE() and OLPP.StartDate < GETDATE()  union all   select CMSP.PackageName,FAI.PaymentDate as InvoiceDate,FAI.Receipt as OrderID,(FAI.PayableAmount+FAI.PayableAmount2) as PaidAmount,FAI.InvoiceID,CCRMM.StartDate,CCRMM.MembershipExpireDate as EndDate from CCRMMembership CCRMM,FAInvoice FAI,CMSPACKAGES CMSP where CCRMM.InvoiceID=FAI.InvoiceID  and CMSP.PackageCode=CCRMM.PackageCode and CCRMM.MembershipCode='{0}' and CCRMM.MembershipExpireDate >= GETDATE() and CCRMM.StartDate < GETDATE() order by InvoiceDate desc ", MemberShipCode));

            try
            {
                if (Mode == "Online" || Mode == "OnLine")
                {
                    dt = getdata(string.Format("select OLP.PackageName,PG.TransactionDate as InvoiceDate,PG.OrderId,PG.Amount as PaidAmount,PG.Invoice as InvoiceID,OLPP.StartDate,OLPP.EndDate from OnlinePackagePurchase OLPP,PaymentGateway PG,OnlinePackages OLP where  OLPP.Invoice=PG.Invoice and OLP.PackageID=OLPP.PackageID and OLPP.MembershipCode='{0}' and OLPP.EndDate >= GETDATE() and OLPP.StartDate > GETDATE() ", MemberShipCode));
                }
                else
                {
                    dt = getdata(string.Format("select CMSP.PackageName,FAI.PaymentDate as InvoiceDate,FAI.Receipt as OrderID,(FAI.PayableAmount+FAI.PayableAmount2) as PaidAmount,FAI.InvoiceID,CCRMM.StartDate,CCRMM.MembershipExpireDate as EndDate from CCRMMembership CCRMM,FAInvoice FAI,CMSPACKAGES CMSP where CCRMM.InvoiceID=FAI.InvoiceID  and CMSP.PackageCode=CCRMM.PackageCode and CCRMM.MembershipCode='{0}' and CCRMM.MembershipExpireDate >= GETDATE() and CCRMM.StartDate > GETDATE() order by InvoiceDate desc ", MemberShipCode));
                }

                if (dt.Rows.Count == 0)
                {

                    //FutureBookingActive.Add("packageName", null);
                    //FutureBookingActive.Add("invoiceDate", null);
                    //FutureBookingActive.Add("orderID", null);
                    //FutureBookingActive.Add("paidAmount", null);
                    //FutureBookingActive.Add("invoiceId", null);
                    //FutureBookingActive.Add("startDate", null);
                    //FutureBookingActive.Add("EndDate", null);
                }
                else
                {
                    dt = getdata(string.Format("select OLP.PackageName,PG.TransactionDate as InvoiceDate,PG.OrderId,PG.Amount as PaidAmount,PG.Invoice as InvoiceID,OLPP.StartDate,OLPP.EndDate from OnlinePackagePurchase OLPP,PaymentGateway PG,OnlinePackages OLP where  OLPP.Invoice=PG.Invoice and OLP.PackageID=OLPP.PackageID and OLPP.MembershipCode='{0}'  and OLPP.StartDate > GETDATE()   union all    select CMSP.PackageName,FAI.PaymentDate as InvoiceDate,FAI.Receipt as OrderID,(FAI.PayableAmount+FAI.PayableAmount2) as PaidAmount,FAI.InvoiceID,CCRMM.StartDate,CCRMM.MembershipExpireDate as EndDate from CCRMMembership CCRMM,FAInvoice FAI,CMSPACKAGES CMSP where CCRMM.InvoiceID=FAI.InvoiceID  and CMSP.PackageCode=CCRMM.PackageCode and CCRMM.MembershipCode='{0}' and  CCRMM.StartDate > GETDATE() order by InvoiceDate desc", MemberShipCode));

                    FutureBookingActive.Add("packageName", Convert.ToString(dt.Rows[0]["PackageName"]));
                    FutureBookingActive.Add("invoiceDate", Convert.ToString(dt.Rows[0]["InvoiceDate"]));
                    FutureBookingActive.Add("orderID", Convert.ToString(dt.Rows[0]["OrderId"]));
                    FutureBookingActive.Add("paidAmount", Convert.ToString(dt.Rows[0]["PaidAmount"]));
                    FutureBookingActive.Add("invoiceId", Convert.ToString(dt.Rows[0]["InvoiceID"]));
                    FutureBookingActive.Add("startDate", Convert.ToString(dt.Rows[0]["StartDate"]));
                    FutureBookingActive.Add("EndDate", Convert.ToString(dt.Rows[0]["EndDate"]));
                }
            }
            catch (Exception ec)
            {

            }


            return FutureBookingActive;

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
        public ArrayList MembersLatestDatesByInvoice(string InvoiceId)
        {
            DateTime dt_time = new DateTime();
            ArrayList Dates = new ArrayList();
            DataSet ds_Dates = new DataSet();
            DataSet ds_Facilityopts = new DataSet();
            //string query = "select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.StartDate <= GETDATE() and CCRM.MembershipCode = '" + MembershipCode + "' order by CCRM.ID desc";
            string query = "select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration,Type='Present',CCRM.InvoiceID  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.StartDate <= GETDATE() and CCRM.MembershipExpireDate >=GETDATE() and CCRM.InvoiceID = '" + InvoiceId + "'  union all  select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration,Type='Future',CCRM.InvoiceID  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.StartDate >= GETDATE() and CCRM.InvoiceID = '" + InvoiceId + "' union all  select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration,Type='Past',CCRM.InvoiceID  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.MembershipExpireDate <= GETDATE()  and CCRM.InvoiceID = '" + InvoiceId + "' ";
            using (SqlDataAdapter da_ExpDates = new SqlDataAdapter(query, cnn))
            {
                da_ExpDates.Fill(ds_Dates);
                //Dates.Add(ds_Dates.Tables[0].Rows[0][0].ToString());
                //Dates.Add(ds_Dates.Tables[0].Rows[0][1].ToString());
                //Dates.Add(ds_Dates.Tables[0].Rows[0][3].ToString());
            }

            string query1 = "select Days from CCRMMembershipFacility where InvoiceID='" + ds_Dates.Tables[0].Rows[0]["InvoiceID"].ToString() + "' and MFDID=7  ";
            using (SqlDataAdapter da_mfdid = new SqlDataAdapter(query1, cnn))
            {
                da_mfdid.Fill(ds_Facilityopts);
            }

            try
            {
                if (Convert.ToInt32(ds_Facilityopts.Tables[0].Rows[0]["Days"].ToString()) > 0)
                {
                    dt_time = Convert.ToDateTime(ds_Dates.Tables[0].Rows[0][1].ToString()).AddDays(-Convert.ToInt32(ds_Facilityopts.Tables[0].Rows[0]["Days"].ToString()));
                    Dates.Add(ds_Dates.Tables[0].Rows[0][0].ToString());
                    Dates.Add(dt_time);
                    Dates.Add(ds_Dates.Tables[0].Rows[0][3].ToString());
                    Dates.Add(ds_Dates.Tables[0].Rows[0][1].ToString());
                }
                else
                {
                    Dates.Add(ds_Dates.Tables[0].Rows[0][0].ToString());
                    Dates.Add(ds_Dates.Tables[0].Rows[0][1].ToString());
                    Dates.Add(ds_Dates.Tables[0].Rows[0][3].ToString());
                    Dates.Add(ds_Dates.Tables[0].Rows[0][1].ToString());

                }
            }
            catch(Exception ec)
            {
                Dates.Add(ds_Dates.Tables[0].Rows[0][0].ToString());
                Dates.Add(ds_Dates.Tables[0].Rows[0][1].ToString());
                Dates.Add(ds_Dates.Tables[0].Rows[0][3].ToString());
                Dates.Add(ds_Dates.Tables[0].Rows[0][1].ToString());
            }
            return Dates;
        }
        public ArrayList MembersLatestDatesByInvoiceOld(string InvoiceId)
        {
            ArrayList Dates = new ArrayList();
            DataSet ds_Dates = new DataSet();
            string query = "select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration,Type='Present'  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.StartDate <= GETDATE() and CCRM.MembershipExpireDate >=GETDATE() and CCRM.InvoiceID = '" + InvoiceId + "'   ";

            using (SqlDataAdapter da_ExpDates = new SqlDataAdapter(query, cnn))
            {

                da_ExpDates.Fill(ds_Dates);
                if (ds_Dates.Tables[0].Rows.Count > 0)
                {
                    Dates.Add(ds_Dates.Tables[0].Rows[0][0].ToString());
                    Dates.Add(ds_Dates.Tables[0].Rows[0][1].ToString());
                    Dates.Add(ds_Dates.Tables[0].Rows[0][3].ToString());
                }
                else
                {
                    Dates.Add("10/10/2020");
                    Dates.Add("10/10/2020");
                    Dates.Add("10/10/2020");
                }

            }



            return Dates;
        }
        public ArrayList MembersFutureLatestDatesByInvoice(string InvoiceId)
        {
            ArrayList Dates = new ArrayList();
            DataSet ds_Dates = new DataSet();
            string query = "select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration,Type='Present'  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.StartDate > GETDATE() and CCRM.MembershipExpireDate > GETDATE() and CCRM.InvoiceID = '" + InvoiceId + "'   ";

            using (SqlDataAdapter da_ExpDates = new SqlDataAdapter(query, cnn))
            {

                da_ExpDates.Fill(ds_Dates);
                if (ds_Dates.Tables[0].Rows.Count > 0)
                {
                    Dates.Add(ds_Dates.Tables[0].Rows[0][0].ToString());
                    Dates.Add(ds_Dates.Tables[0].Rows[0][1].ToString());
                    Dates.Add(ds_Dates.Tables[0].Rows[0][3].ToString());
                }
                else
                {
                    Dates.Add("10/10/2020");
                    Dates.Add("10/10/2020");
                    Dates.Add("10/10/2020");
                }

            }



            return Dates;
        }
        public ArrayList MembersLatestAmountByInvoice(string InvoiceId)
        {
            ArrayList Amount = new ArrayList();
            try
            {
                string query = "";

                query = "select (IsNull((select sum(FAD.DuePaidAmount) as FinalAmount from FADueInvoice FAD,FAPaymentModes FAP where FAP.PayModeCode=FAD.FAPaymentModes and InvoiceID='" + InvoiceId + "'  and DuePaidAmount<>0),0)  + ((select sum(PayableAmount+PayableAmount2) as FinalAmount from FAInvoice where InvoiceID='" + InvoiceId + "')))+(select (IsNull((select top 1 RemainingAmount as FinalAmount from FAInvoice where InvoiceID='" + InvoiceId + "' order by ID desc),0)))+(select (IsNull((select top 1 Wallet as FinalAmount from FAInvoice where InvoiceID='" + InvoiceId + "' order by ID desc),0))) as FinalAmount";

                using (SqlCommand cmd_LatestAmount = new SqlCommand(query, cnn))
                {
                    cnn.Open();
                    SqlDataReader LatestAmount_DR = cmd_LatestAmount.ExecuteReader();
                    if (LatestAmount_DR.Read())
                    {
                        Amount.Add(LatestAmount_DR[0].ToString());

                    }
                    cnn.Close();
                }

                string query1 = "select sum(IGST) as IGST from FAInvoice where InvoiceID='" + InvoiceId + "' ";
                using (SqlCommand cmd_LatestAmount1 = new SqlCommand(query1, cnn))
                {
                    cnn.Open();
                    SqlDataReader LatestAmount_DR2 = cmd_LatestAmount1.ExecuteReader();
                    if (LatestAmount_DR2.Read())
                    {
                        Amount.Add(LatestAmount_DR2[0].ToString());

                    }
                    cnn.Close();
                }
            }
            catch (Exception ec)
            {

            }
            finally
            {
                cnn.Close();
            }
            return Amount;
        }

        //public List<FacilityOptsBooking> GetFacilityCurrentOptes(string MembershipCode)
        //{
        //    FacilityDetailsOutput fdop = new FacilityDetailsOutput();
        //    DataSet SelectedSlots = new DataSet();
        //    DataTable dt_CCRMMembershipFacility = new DataTable();
        //    DataTable dt_FacilityOpted = new DataTable();
        //    DataTable dt_Facility = new DataTable();

        //    List<FacilityOptsBooking> foptes = new List<FacilityOptsBooking>();



        //    dt_FacilityOpted = getdata(string.Format("select  CCRMF.MFDID,SMFM.SubSchemName,Opted=1 from CCRMMembershipFacility CCRMF,SubMembersFacilityMaster SMFM where  SMFM.SMFMID=CCRMF.MFDID and  CCRMF.MembershipCode='{0}' and GETDATE() < CCRMF.FacilityStartDate and GETDATE() <= CCRMF.FacilityExpireDate   union all  select SMFDID as MFDID,FacilityName as SubSchemName,Opted  from  FacilityOpted where MembershipCode='{0}' and SMFDID not in (select  CCRMF.MFDID from CCRMMembershipFacility CCRMF,SubMembersFacilityMaster SMFM where  SMFM.SMFMID=CCRMF.MFDID and  CCRMF.MembershipCode='{0}' and GETDATE() < CCRMF.FacilityStartDate and GETDATE() <= CCRMF.FacilityExpireDate)", MembershipCode));


        //    foptes.Add(new FacilityOptsBooking
        //    {
        //        Freezing = Convert.ToInt32(dt_FacilityOpted.Rows[0]["Opted"])
        //        ,

        //        Upgrade = Convert.ToInt32(dt_FacilityOpted.Rows[1]["Opted"])
        //        ,
        //        Change = Convert.ToInt32(dt_FacilityOpted.Rows[2]["Opted"])
        //        ,
        //        Transfer = Convert.ToInt32(dt_FacilityOpted.Rows[3]["Opted"])
        //        ,
        //        Paused = Convert.ToInt32(dt_FacilityOpted.Rows[5]["Opted"])
        //        ,
        //        Convert = Convert.ToInt32(dt_FacilityOpted.Rows[6]["Opted"])
        //    });


        //    return foptes;
        //}
        //public List<FacilityOptsBooking> GetFacilityFutureOptes(string MembershipCode)
        //{
        //    FacilityDetailsOutput fdop = new FacilityDetailsOutput();
        //    DataSet SelectedSlots = new DataSet();
        //    DataTable dt_CCRMMembershipFacility = new DataTable();
        //    DataTable dt_FacilityOpted = new DataTable();
        //    DataTable dt_Facility = new DataTable();

        //    List<FacilityOptsBooking> foptes = new List<FacilityOptsBooking>();



        //    dt_FacilityOpted = getdata(string.Format("select  CCRMF.MFDID,SMFM.SubSchemName,Opted=1 from CCRMMembershipFacility CCRMF,SubMembersFacilityMaster SMFM where  SMFM.SMFMID=CCRMF.MFDID and  CCRMF.MembershipCode='{0}' and GETDATE() >= CCRMF.FacilityStartDate and GETDATE() <= CCRMF.FacilityExpireDate  union all select SMFDID as MFDID,FacilityName as SubSchemName,Opted  from  FacilityOpted where MembershipCode='{0}' and SMFDID not in (select  CCRMF.MFDID from CCRMMembershipFacility CCRMF,SubMembersFacilityMaster SMFM where  SMFM.SMFMID=CCRMF.MFDID and  CCRMF.MembershipCode='{0}' and GETDATE() >= CCRMF.FacilityStartDate and GETDATE() <= CCRMF.FacilityExpireDate)", MembershipCode));


        //    foptes.Add(new FacilityOptsBooking
        //    {
        //        Freezing = Convert.ToInt32(dt_FacilityOpted.Rows[0]["Opted"])
        //        ,

        //        Upgrade = Convert.ToInt32(dt_FacilityOpted.Rows[1]["Opted"])
        //        ,
        //        Change = Convert.ToInt32(dt_FacilityOpted.Rows[2]["Opted"])
        //        ,
        //        Transfer = Convert.ToInt32(dt_FacilityOpted.Rows[3]["Opted"])
        //        ,
        //        Paused = Convert.ToInt32(dt_FacilityOpted.Rows[5]["Opted"])
        //        ,
        //        Convert = Convert.ToInt32(dt_FacilityOpted.Rows[6]["Opted"])
        //    });


        //    return foptes;
        //}
        public List<info> GetInfo(string MemberShipCode)
        {
            List<info> ftd = new List<info>();
            DataTable dt = new DataTable();


            dt = getdata(string.Format("select OLP.PackageName,PG.TransactionDate as InvoiceDate,PG.OrderId,PG.Amount as PaidAmount,PG.Invoice as InvoiceID,OLPP.StartDate,OLPP.EndDate from OnlinePackagePurchase OLPP,PaymentGateway PG,OnlinePackages OLP where  OLPP.Invoice=PG.Invoice and OLP.PackageID=OLPP.PackageID and OLPP.MembershipCode='{0}' and OLPP.EndDate >= GETDATE() and OLPP.StartDate < GETDATE()  union all   select CMSP.PackageName,FAI.PaymentDate as InvoiceDate,FAI.Receipt as OrderID,(FAI.PayableAmount+FAI.PayableAmount2) as PaidAmount,FAI.InvoiceID,CCRMM.StartDate,CCRMM.MembershipExpireDate as EndDate from CCRMMembership CCRMM,FAInvoice FAI,CMSPACKAGES CMSP where CCRMM.InvoiceID=FAI.InvoiceID  and CMSP.PackageCode=CCRMM.PackageCode and CCRMM.MembershipCode='{0}' and CCRMM.MembershipExpireDate >= GETDATE() and CCRMM.StartDate < GETDATE() order by InvoiceDate desc ", MemberShipCode));

            if (dt.Rows.Count == 0)
            {
                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    ftd.Add(new info
                    {
                        changeplan = null
                     ,
                        convert = null
                     ,
                        freezing = null
                     ,
                        paused = null
                    ,
                        transerpackage = null
                        ,
                        upgradeplan = null
                    });
                }
            }
            else
            {

            }



            return ftd;

        }
        public Object GetMembersFreezingDetails([FromBody]FreezingInput fi)
        {
            string sJSONResponse = "";
            string formater = "";
            List<GetMembersDetails> ftd = new List<GetMembersDetails>();
            DataTable dt_UsedCount = new DataTable();
            DataTable dt_noofdays = new DataTable();

            dt_UsedCount = getdata(string.Format("select Top 1  FreezingDays from CCRMMembershipFacility where InvoiceID='{0}' and MFDID=1", fi.Invoice));

            dt_noofdays = getdata(string.Format("select distinct FF.NoOfDays from  FreezingFacility FF,CCRMMembership CCRMM,CMSPLANCOST CMSP  where FF.FreezingID=CMSP.FreezingID and CMSP.PlanCode=CCRMM.PlanCode and CCRMM.DurationId=CMSP.DurationID and CCRMM.InvoiceID='{0}'  ", fi.Invoice));
            if (dt_noofdays.Rows.Count > 0 && dt_UsedCount.Rows.Count > 0)
            {
                string a = dt_noofdays.Rows[0]["NoOfDays"].ToString();
                string b = dt_UsedCount.Rows[0]["FreezingDays"].ToString();

                ftd.Add(new GetMembersDetails
                {
                    availableCount = Convert.ToInt32(dt_noofdays.Rows[0]["NoOfDays"].ToString()) - Convert.ToInt32(dt_UsedCount.Rows[0]["FreezingDays"].ToString())
                           ,
                    mfdid = 1
                           ,
                    noOfDays = Convert.ToInt32(dt_noofdays.Rows[0]["NoOfDays"])
                         ,
                    usedCount = Convert.ToInt32(dt_UsedCount.Rows[0]["FreezingDays"])


                });

                formater = JsonConvert.SerializeObject(ftd);
                FreezingOutput fo = new FreezingOutput();
                fo.status = "success";
                fo.value = formater;

                sJSONResponse = JsonConvert.SerializeObject(fo);
            }
            else
            {

                FreezingOutput fo = new FreezingOutput();
                fo.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(fo);

            }



            return sJSONResponse;

        }
        public DataTable GetMembersUsedFacilitiesFreezingDetails(string MemberShipCode, string BranchCode, string InvoiceID)
        {
            DataSet ds_custdet1 = new DataSet();
            string PromoCodes = "";
            string PromoCode = GetMembersPromoCode(MemberShipCode);
            DataTable dt_MemberShipFacilityDetails = new DataTable();
            DataRow row;
            DataColumn col1 = new DataColumn("SMFMID", typeof(string));
            DataColumn col2 = new DataColumn("SubSchemName", typeof(string));

            dt_MemberShipFacilityDetails.Columns.AddRange(new DataColumn[] { col1, col2 });

            string FacilityID = "";
            string FacilityName = "";
            string query = "select distinct MFDID  from CCRMMembershipFacility where MemberShipCode='" + MemberShipCode + "' and InvoiceID='" + InvoiceID + "'  and MFDID=1 and PlanCode=(select top 1 PlanCode from CCRMMembership where MembershipCode='" + MemberShipCode + "' order by ID desc)";

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            if (ds_custdet1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i <= ds_custdet1.Tables[0].Rows.Count - 1; i++)
                {
                    if (i == 0)
                    {
                        PromoCodes = ds_custdet1.Tables[0].Rows[i][0].ToString();
                    }
                    else
                    {
                        PromoCodes = PromoCodes + "," + ds_custdet1.Tables[0].Rows[i][0].ToString();
                    }
                }
            }

            string[] str = PromoCodes.Split(',');
            for (int i = 0; i <= str.Length - 1; i++)
            {
                if (str[i].ToString() != "" || str[i].ToString() != string.Empty)
                {
                    string query1 = "select distinct SMFMID,SubSchemName from SubMembersFacilityMaster where SMFMID = " + str[i].ToString() + "";
                    using (SqlCommand cmd_FC = new SqlCommand(query1, cnn))
                    {
                        if (cnn.State == ConnectionState.Open)
                        {
                            cnn.Close();
                        }
                        cnn.Open();
                        SqlDataReader FC_DR = cmd_FC.ExecuteReader();
                        if (FC_DR.Read())
                        {
                            FacilityID = FC_DR[0].ToString();
                            FacilityName = FC_DR[1].ToString();
                        }
                        else
                        {
                            FacilityID = "";
                            FacilityName = "";
                        }
                        cnn.Close();
                    }

                    row = dt_MemberShipFacilityDetails.NewRow();
                    row["SMFMID"] = FacilityID;
                    row["SubSchemName"] = FacilityName;
                    dt_MemberShipFacilityDetails.Rows.Add(row);
                }
                else
                {
                    row = dt_MemberShipFacilityDetails.NewRow();
                    row["SMFMID"] = "";
                    row["SubSchemName"] = "";
                    dt_MemberShipFacilityDetails.Rows.Add(row);

                }
            }

            return dt_MemberShipFacilityDetails;
        }
        public string GetMembersPromoCode(string MemberShipCode)
        {
            string PromoCode = "";
            try
            {
                cnn.Close();
                string query = "select  top 1 PromoCode from FAInvoice where MembershipCode='" + MemberShipCode + "' and PromoCode!='' order by ID desc";
                using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
                {
                    cnn.Open();
                    SqlDataReader GA_DR = cmd_SubEXT.ExecuteReader();
                    if (GA_DR.Read())
                    {
                        PromoCode = GA_DR[0].ToString();
                    }
                    cnn.Close();
                }
            }
            catch (Exception ec)
            {

            }
            finally
            {
                cnn.Close();
            }
            return PromoCode;

        }

        //ccrm and invoice
        //paymentgateway and onlinepackageperchase
    }
}