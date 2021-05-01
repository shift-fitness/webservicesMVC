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
using NarenFitnessUsers.Models.Login;
using NarenFitnessUsers.Models.Packages;
using NarenFitnessUsers.Models.OffLine;
using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;
using System.Web.Helpers;

namespace NarenFitnessUsers.Controllers
{
    public class OfflineDashboardController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        JsonClass json = new JsonClass();
        public Object DashboardDetails([FromBody]Dashboard Dashboard)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string App_Query = "";

            ResponseMessage rpm = new ResponseMessage();
            string ImageUrl = GetPhotoUrl(Dashboard.Image, Dashboard.AppId + imgname());



            try
            {
                cnn.Open();
                App_Query = "insert into DashboardImages(AppId,DisplayId,Image,ImageURL,CreatedBy,CreatedOn,IsDeleted,IsActive) values(" + Dashboard.AppId + "," + Dashboard.DisplayId + ",'" + Dashboard.Image + "','" + ImageUrl + "','" + Dashboard.CreatedBy + "','" + ServerDateTime + "',0,1)  SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(App_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                rpm.Status = "Success";
                rpm.Id = a;
            }
            catch (Exception ex)
            {
                rpm.Status = "Success";

            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(rpm);

            return sJSONResponse;
        }
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

                //urlform = "http://137.59.201.211/GYMUI/UsersImages/" + endpath;
                urlform = "http://202.143.96.72/GYMUI/UsersImages/" + endpath;
            }
            catch (Exception ec)
            {
                urlform = "";
            }

            return urlform;
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
        public List<Freetraildetails> Freetraildetailslist(string MobileNo)
        {

            // 

            List<Freetraildetails> ftd = new List<Freetraildetails>();
            DataTable dt = getdata(string.Format("select StartDate as SessionDate,convert(varchar(10), CMSSTS.SlotStartTime, 108) as SlotTime ,HRE.EmployeeName as TrainerName from CCRMEnquireFreeTrial CCRMEFT,CMSSLOTTIMESETTING CMSSTS, HREmployee HRE where CCRMEFT.MobileNo = '{0}' and CCRMEFT.SlotCode = CMSSTS.SlotCode and HRE.EmployeeCode = CCRMEFT.TrainerCode ", MobileNo));


            ftd.Add(new Freetraildetails
            {
                sessionDate = Convert.ToString(dt.Rows[0]["SessionDate"])
                               ,
                slotTime = Convert.ToString(dt.Rows[0]["SlotTime"])
                               ,
                trainerName = Convert.ToString(dt.Rows[0]["TrainerName"])

            });


            return ftd;
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
        public List<ImageName> GetPackageImage(int AppID)
        {
            List<ImageName> Imagenamess = new List<ImageName>();

            DataTable dt = getdata(string.Format("select ID as ImageID,ImageURL from DashboardImages where AppId='{0}' and DisplayId=3", AppID));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Imagenamess.Add(new ImageName
                {
                    imageId = Convert.ToString(dt.Rows[i]["imageid"])
                    ,
                    imageUrl = Convert.ToString(dt.Rows[i]["ImageURL"])


                });

            }
            return Imagenamess;
        }

        //public List<BannersName> GetBannerImages(int AppId)
        //{
        //    List<BannersName> Imagenamess = new List<BannersName>();

        //    DataTable dt = getdata(string.Format("select ID as ImageID,ImageURL from DashboardImages where AppId='{0}' and DisplayId=1", AppId));
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        Imagenamess.Add(new BannersName
        //        {
        //            imageid = Convert.ToString(dt.Rows[i]["ImageId"])
        //            ,
        //            imageURL = Convert.ToString(dt.Rows[i]["ImageURL"])


        //        });

        //    }
        //    return Imagenamess;
        //}
        public List<TextHeader> GetText()
        {
            List<TextHeader> textnames = new List<TextHeader>();

            DataTable dt = getdata(string.Format("select ID as TextID,Text as TextName from DashboardText where DisplayID=4", ""));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                textnames.Add(new TextHeader
                {
                    textID = Convert.ToString(dt.Rows[i]["TextID"])
                    ,
                    textName = Convert.ToString(dt.Rows[i]["TextName"])


                });

            }
            return textnames;
        }
        public List<PackageDetails> PackageDetails(string MobileNo)
        {
            List<PackageDetails> ftd = new List<PackageDetails>();
            DataTable dt = getdata(string.Format("select StartDate as SessionDate,convert(varchar(10), CMSSTS.SlotStartTime, 108) as SlotTime ,HRE.EmployeeName as TrainerName from CCRMEnquireFreeTrial CCRMEFT,CMSSLOTTIMESETTING CMSSTS, HREmployee HRE where CCRMEFT.MobileNo = '{0}' and CCRMEFT.SlotCode = CMSSTS.SlotCode and HRE.EmployeeCode = CCRMEFT.TrainerCode ", MobileNo));

            ftd.Add(new PackageDetails
            {
                SessionDate = Convert.ToString(Convert.ToDateTime(dt.Rows[0]["SessionDate"]).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
                                           ,
                SlotTime = Convert.ToString(Convert.ToDateTime(dt.Rows[0]["SlotTime"]).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
                                           ,
                TrainerName = Convert.ToString(dt.Rows[0]["TrainerName"])

            });

            return ftd;
        }
        public List<ImageName2> GetPackages2(string Branchcode, int PackageId, string PackageName, string StartDate, string Enddate)
        {
            List<ImageName2> allpackageall = new List<ImageName2>();
            allpackageall.Add(new ImageName2 { PackageName = PackageName, Packages = PackageDetails2(Branchcode, PackageId, PackageName, StartDate, Enddate) });
            return allpackageall;
        }
        public List<ImageName> GetPackages1(string MobileNo, string PackageName, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        {
            List<ImageName> allpackageall = new List<ImageName>();
            allpackageall.Add(new ImageName { packageName = PackageName, packages = PackageDetails1(MobileNo, PackageName, NoOfSession, NoOfValidity, NoofSlotsUsed) });
            return allpackageall;
        }
        public List<GetFreeTrialsAll> GetImages(int AppId, string ImageName)
        {
            List<GetFreeTrialsAll> allFreetrial = new List<GetFreeTrialsAll>();
            allFreetrial.Add(new GetFreeTrialsAll { ImageName = ImageName, Images = GetFreeTrailImage(AppId) });
            return allFreetrial;
        }
        public List<PackagePrices> GetPackagePriceDetails(int EnquireTypeNo)
        {
            List<PackagePrices> PackagePrice = new List<PackagePrices>();

            DataTable dt = getdata(string.Format("select BranchCode,PackageID,PackageName,PackageCost,DiscountPercentage,NumberOfSession,NumberOfDaysValidity from OnlinePackages where EnquireTypeNo='{0}' ", EnquireTypeNo));
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                PackagePrice.Add(new PackagePrices
                {
                    branchCode = Convert.ToString(dt.Rows[i]["BranchCode"])
                    ,
                    packageId = Convert.ToString(dt.Rows[i]["PackageID"])
                     ,
                    packageName = Convert.ToString(dt.Rows[i]["PackageName"])
                     ,
                    packageCost = Convert.ToString(dt.Rows[i]["PackageCost"])
                     ,
                    discountPercentage = Convert.ToString(dt.Rows[i]["DiscountPercentage"])
                     ,
                    numberOfSession = Convert.ToString(dt.Rows[i]["NumberOfSession"])
                     ,
                    numberOfDaysValidity = Convert.ToString(dt.Rows[i]["NumberOfDaysValidity"])

                });

            }
            return PackagePrice;
        }
        public List<BannerAll> GetBanner(int AppId, string BannerName)
        {
            List<BannerAll> allbanners = new List<BannerAll>();
            allbanners.Add(new BannerAll { bannerName = BannerName, banners = GetBannerImages(AppId) });
            return allbanners;
        }
        public List<BannersName> GetBannerImages(int AppId)
        {
            List<BannersName> Imagenamess = new List<BannersName>();

            DataTable dt = getdata(string.Format("select ID as ImageID,ImageURL from DashboardImages where AppId='{0}' and DisplayId=1 and IsDeleted=0 and ScreenId=2", AppId));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Imagenamess.Add(new BannersName
                {
                    id = Convert.ToInt32(dt.Rows[i]["ImageId"])
                    ,
                    url = Convert.ToString(dt.Rows[i]["ImageURL"])


                });

            }
            return Imagenamess;
        }
        public List<ImageName> GetFreeTrailImage(int AppID)
        {
            List<ImageName> Imagenamess = new List<ImageName>();

            DataTable dt = getdata(string.Format("select ID as ImageID,ImageURL from DashboardImages where AppId='{0}' and DisplayId=2", AppID));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Imagenamess.Add(new ImageName
                {
                    imageId = Convert.ToString(dt.Rows[i]["imageid"])
                    ,
                    imageUrl = Convert.ToString(dt.Rows[i]["ImageURL"])
                });

            }
            return Imagenamess;
        }
        public List<TextHeaderAll> GetHeaders(string HeaderName)
        {
            List<TextHeaderAll> allTextHeader = new List<TextHeaderAll>();
            allTextHeader.Add(new TextHeaderAll { text = HeaderName, headers = GetTextHeaders() });
            return allTextHeader;
        }
        public List<TextHeader> GetTextHeaders()
        {
            List<TextHeader> textnames = new List<TextHeader>();

            DataTable dt = getdata(string.Format("select ID as TextID,Text as TextName from DashboardText where DisplayID=4 and ScreenId=2", ""));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                textnames.Add(new TextHeader
                {
                    textID = Convert.ToString(dt.Rows[i]["TextID"])
                    ,
                    textName = Convert.ToString(dt.Rows[i]["TextName"])


                });

            }
            return textnames;
        }
        public Object GetOffLineDashboard([FromBody]Dashboard Dashboard)
        {
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataTable dt_SAUcount = new DataTable();
            DataTable dt = new DataTable();
            DataTable dt_IsPackage = new DataTable();
            DataTable dt_Active = new DataTable();
            DataTable dt_SessionDateByInvoice = new DataTable();

            int ChangeSlot = 0;
            int PreRenewal = 0;
            int PostRenewal = 0;

            DataSet DashboardFT = new DataSet();
            int S1 = 0;
            int S2 = 0;
            int S = 0;
            int A = 0;
            int U = 0;
            FinalOutPut fout = new FinalOutPut();
            FinalOutPutB foutB = new FinalOutPutB();
            FinalOutPutC foutC = new FinalOutPutC();
            FinalOutPutD foutD = new FinalOutPutD();
            FinalOutPutE foutE = new FinalOutPutE();
            FinalOutPutF foutF = new FinalOutPutF();



            string sJSONResponse = "";
            var dateTimeNow = DateTime.Now; // Return 00/00/0000 00:00:00
            var date = dateTimeNow.ToShortDateString(); //Return 00/00/0000

            string MobileNo = "";
            string MobileDeviceId = "";

            MobileNo = Dashboard.MobileNo;
            MobileDeviceId = Dashboard.MobileDeviceID;

            string status = "";
            string FacilityId = "";
            try
            {
                GetApplicationInfoFail gaif = new GetApplicationInfoFail();

                if (MobileNo == null && MobileDeviceId != "")
                {
                    MobileNo = "";
                }
                else if (MobileNo != "" && MobileDeviceId != "")
                {
                    MobileDeviceId = "";
                }
                else
                {
                    MobileNo = "";
                }

                DataTable dt_SAU = getdata(string.Format("select count(*) as count,value='S' from OffLineSlotUsed where MobileNo='{0}' union all select count(*) as count,value='A' from OffLineSlotUsed where MobileNo='{0}' union all select count(*) as count,value='U' from OffLineSlotUsed OLSU,CMSSLOTTIMESETTING CMSS where OLSU.SlotID=CMSS.SlotCode and MobileNo='{0}' and CONVERT(VARCHAR(101),SessionDate,103) <=CONVERT(VARCHAR(101),GETDATE(),103) and CONVERT(VARCHAR(101),CMSS.SlotEndTime,108) <=CONVERT(VARCHAR(101),GETDATE(),108)", MobileNo));

                if (MobileNo != "")
                {

                    try
                    {
                        dt = getdata(string.Format("select ID,DisplayName from DashboardDisplay", ""));
                        dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT top 1 T1.EnquireTypeNo FROM CCRMMEnquireStatus T1 WHERE  T1.MobileNo = '{0}'  order by T1.ID desc )  SELECT(SELECT top 1 T1.EnquireTypeNo FROM CCRMMEnquireStatus T1 WHERE  T1.MobileNo = '{0}'  order by T1.ID desc) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", MobileNo));
                        //CCRM table quary - 21/10-Dilip
                        // dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM OffLineSlotUsed AS T1   WHERE  T1.MobileNo = '{0}' )  SELECT(SELECT top 1 CCRM.EnquireTypeNo FROM CCRMMembership CCRM, Users U  WHERE CCRM.MembershipCode = U.UCode and  U.MobileNo = '{0}'  order by U.ID desc) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", MobileNo));
                        // dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM OffLineSlotUsed AS T1   WHERE  T1.MobileNo='{0}' )  SELECT (SELECT top 1 T1.EnquireTypeNo FROM CCRMMembership AS T1  WHERE T1.MobileNo='{0}'  order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", MobileNo));
                        dt_Active = getdata(string.Format("IF EXISTS (SELECT T1.MembershipExpireDate FROM CCRMMembership AS T1,Users T2  WHERE T1.StartDate >=GETDATE() and  T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode)   SELECT 1 as IsExist  ELSE SELECT 0 as IsExist", MobileNo));
                        //dt_SessionDateByInvoice = getdata(string.Format("select Top 1 SessionDate from OffLineSlotUsed where InvoiceID=(select Top 1 InvoiceID from CCRMMembership where MobileNo='{0}' and MembershipExpireDate >= GETDATE() order by ID desc) order by ID desc", MobileNo));
                        dt_SessionDateByInvoice = getdata(string.Format("select Top 1 CCRMM.StartDate as  SessionDate,InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and U.MobileNo='{0}' and CCRMM.MembershipExpireDate >= CAST( GETDATE() AS Date ) and CCRMM.StartDate <= CAST( GETDATE() AS Date ) order by CCRMM.ID desc", MobileNo));

                        if (dt_SessionDateByInvoice.Rows.Count <= 0)
                        {
                            dt_SessionDateByInvoice = getdata(string.Format("select Top 1 CCRMM.StartDate as  SessionDate,InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and U.MobileNo='{0}' and CCRMM.MembershipExpireDate >= CAST( GETDATE() AS Date ) and CCRMM.StartDate >= CAST( GETDATE() AS Date ) order by CCRMM.ID desc", MobileNo));

                        }
                        DateTime dtt = Convert.ToDateTime(dt_SessionDateByInvoice.Rows[0]["SessionDate"].ToString());

                        var userdate = Convert.ToDateTime(dt_SessionDateByInvoice.Rows[0]["SessionDate"].ToString());
                        var date2 = userdate.ToShortDateString(); //Return 00/00/0000

                        if (Convert.ToDateTime(date2) >= Convert.ToDateTime(date))
                        {
                            dt_SAUcount = getdata(string.Format("select  Name='NumberOfSession',count=(CMSD.Duration)*30,CCRM.PackageCode as PackageID,CMSP.PackageName,CCRM.BranchCode,CCRM.StartDate as  StartDate,CCRM.MembershipExpireDate as EndDate,NoofSessions=(CMSD.Duration)*30,NumberOfDaysValidity=(CMSD.Duration)*30 from  CCRMMembership CCRM,CMSPACKAGES CMSP,CMSDURATION CMSD,Users U where CMSP.PackageCode=CCRM.PackageCode and U.UCode=CCRM.MembershipCode and  CMSD.DurationID=CCRM.DurationId and  CCRM.MembershipExpireDate >= CAST( GETDATE() AS Date ) and CCRM.StartDate >= CAST( GETDATE() AS Date ) and U.MobileNo='{0}' union all SELECT Name='NumberOfSession',0,'0','0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS ( select  Name='NumberOfSession',count=(CMSD.Duration)*30,CCRM.PackageCode as PackageID,CMSP.PackageName,CCRM.BranchCode,CCRM.StartDate as  StartDate,CCRM.MembershipExpireDate as EndDate,NoofSessions=(CMSD.Duration)*30,NumberOfDaysValidity=(CMSD.Duration)*30 from  CCRMMembership CCRM,CMSPACKAGES CMSP,CMSDURATION CMSD,Users U where CMSP.PackageCode=CCRM.PackageCode and U.UCode=CCRM.MembershipCode and  CMSD.DurationID=CCRM.DurationId and  CCRM.MembershipExpireDate >= CAST( GETDATE() AS Date ) and CCRM.StartDate >= CAST( GETDATE() AS Date ) and U.MobileNo='{0}' ) union all  select Name='Applied',datediff(day,CCRMM.StartDate, getdate()+(select count(*) as Count from OffLineSlotUsed OLSU,CMSSLOTTIMESETTING CMSSS  where CMSSS.SlotCode=OLSU.SlotID and  MobileNo='{0}' and  CONVERT(VARCHAR(101),SessionDate,103)=CONVERT(VARCHAR(101),GETDATE(),103)  and  CONVERT(VARCHAR(101),CMSSS.SlotEndTime,108) > CONVERT(VARCHAR(101),GETDATE(),108))) as count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= CAST( GETDATE() AS Date ) order by CCRMM.ID desc) union all select Name='Applied',0,'0','0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS (select Name='Applied',datediff(day,CCRMM.StartDate, getdate()+(select count(*) as Count from OffLineSlotUsed OLSU,CMSSLOTTIMESETTING CMSSS  where CMSSS.SlotCode=OLSU.SlotID and  MobileNo='{0}' and  CONVERT(VARCHAR(101),SessionDate,103)>CONVERT(VARCHAR(101),GETDATE(),103))) as count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= CAST( GETDATE() AS Date ) order by CCRMM.ID desc)) union all  select Name='Used',(datediff(day,CCRMM.StartDate, getdate())-(select count(*) as Count from OffLineSlotUsed OLSU,CMSSLOTTIMESETTING CMSSS  where CMSSS.SlotCode=OLSU.SlotID and  MobileNo='{0}' and  CONVERT(VARCHAR(101),SessionDate,103)>CONVERT(VARCHAR(101),GETDATE(),103)  and  CONVERT(VARCHAR(101),CMSSS.SlotEndTime,108) > CONVERT(VARCHAR(101),GETDATE(),108))) as Count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= CAST( GETDATE() AS Date ) order by CCRMM.ID desc) union all SELECT Name='Used',0,'0','0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS (select Name='Used',datediff(day,CCRMM.StartDate, getdate()) as Count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= CAST( GETDATE() AS Date ) order by CCRMM.ID desc) )", MobileNo));
                        }
                        else
                        {
                            dt_SAUcount = getdata(string.Format("select  Name='NumberOfSession',count=(CMSD.Duration)*30,CCRM.PackageCode as PackageID,CMSP.PackageName,CCRM.BranchCode,CCRM.StartDate as  StartDate,CCRM.MembershipExpireDate as EndDate,NoofSessions=(CMSD.Duration)*30,NumberOfDaysValidity=(CMSD.Duration)*30 from  CCRMMembership CCRM,CMSPACKAGES CMSP,CMSDURATION CMSD,Users U where CMSP.PackageCode=CCRM.PackageCode and U.UCode=CCRM.MembershipCode and  CMSD.DurationID=CCRM.DurationId and  CCRM.MembershipExpireDate >= GETDATE() and CCRM.StartDate < GETDATE() and U.MobileNo='{0}' union all SELECT Name='NumberOfSession',0,'0','0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS ( select  Name='NumberOfSession',count=(CMSD.Duration)*30,CCRM.PackageCode as PackageID,CMSP.PackageName,CCRM.BranchCode,CCRM.StartDate as  StartDate,CCRM.MembershipExpireDate as EndDate,NoofSessions=(CMSD.Duration)*30,NumberOfDaysValidity=(CMSD.Duration)*30 from  CCRMMembership CCRM,CMSPACKAGES CMSP,CMSDURATION CMSD,Users U where CMSP.PackageCode=CCRM.PackageCode and U.UCode=CCRM.MembershipCode and  CMSD.DurationID=CCRM.DurationId and  CCRM.MembershipExpireDate >= GETDATE() and CCRM.StartDate < GETDATE() and U.MobileNo='{0}' ) union all  select Name='Applied',datediff(day,CCRMM.StartDate, getdate()+(select count(*) as Count from OffLineSlotUsed OLSU,CMSSLOTTIMESETTING CMSSS  where CMSSS.SlotCode=OLSU.SlotID and  MobileNo='{0}' and  CONVERT(VARCHAR(101),SessionDate,103)>CONVERT(VARCHAR(101),GETDATE(),103))) as count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc) union all select Name='Applied',0,'0','0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS (select Name='Applied',datediff(day,CCRMM.StartDate, getdate()+(select count(*) as Count from OffLineSlotUsed OLSU,CMSSLOTTIMESETTING CMSSS  where CMSSS.SlotCode=OLSU.SlotID and  MobileNo='{0}' and  CONVERT(VARCHAR(101),SessionDate,103)>CONVERT(VARCHAR(101),GETDATE(),103))) as count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc)) union all  select Name='Used',datediff(day,CCRMM.StartDate, getdate()) as Count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc) union all SELECT Name='Used',0,'0','0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS (select Name='Used',datediff(day,CCRMM.StartDate, getdate()) as Count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc) )", MobileNo));
                        }
                    }
                    catch (Exception ec)
                    {
                        //dt_SAUcount = getdata(string.Format("with UnionTable as  (select Name='Applied',datediff(day,CCRMM.StartDate, getdate()+(select count(*) as Count from OffLineSlotUsed OLSU,CMSSLOTTIMESETTING CMSSS  where CMSSS.SlotCode=OLSU.SlotID and  MobileNo='{0}' and  CONVERT(VARCHAR(101),SessionDate,103)>CONVERT(VARCHAR(101),GETDATE(),103))) as count,PackageID='',PackageName='',BrancCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc)  union all select Name='Applied',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS (select Name='Applied',datediff(day,CCRMM.StartDate, getdate()+(select count(*) as Count from OffLineSlotUsed OLSU,CMSSLOTTIMESETTING CMSSS  where CMSSS.SlotCode=OLSU.SlotID and  MobileNo='{0}' and  CONVERT(VARCHAR(101),SessionDate,103)>CONVERT(VARCHAR(101),GETDATE(),103))) as count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc)) union all select top 1  Name='Applied',(select count(*) as Count from OffLineSlotUsed OLSU,CMSSLOTTIMESETTING CMSSS  where CMSSS.SlotCode=OLSU.SlotID and  MobileNo='{0}' and  CONVERT(VARCHAR(101),SessionDate,103)>CONVERT(VARCHAR(101),GETDATE(),103)) as Count,PackageID=0,PackageName=0,BranchCode,StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from Users )  SELECT   Name,count,PackageID,PackageName,BrancCode,StartDate,EndDate,NoofSessions,NumberOfDaysValidity FROM UnionTable GROUP BY Name,count,PackageID,PackageName,BrancCode,StartDate,EndDate,NoofSessions,NumberOfDaysValidity having MAX(count) >= 0 union all select  Name='NumberOfSession',count=(CMSD.Duration)*30,CCRM.PackageCode as PackageID,CMSP.PackageName,CCRM.BranchCode,CCRM.StartDate as  StartDate,CCRM.MembershipExpireDate as EndDate,NoofSessions=(CMSD.Duration)*30,NumberOfDaysValidity=(CMSD.Duration)*30 from  CCRMMembership CCRM,CMSPACKAGES CMSP,CMSDURATION CMSD,Users U where CMSP.PackageCode=CCRM.PackageCode and U.UCode=CCRM.MembershipCode and  CMSD.DurationID=CCRM.DurationId and  CCRM.MembershipExpireDate >= GETDATE() and CCRM.StartDate < GETDATE() and U.MobileNo='{0}' union all SELECT Name='NumberOfSession',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS ( select  Name='NumberOfSession',count=(CMSD.Duration)*30,CCRM.PackageCode as PackageID,CMSP.PackageName,CCRM.BranchCode,CCRM.StartDate as  StartDate,CCRM.MembershipExpireDate as EndDate,NoofSessions=(CMSD.Duration)*30,NumberOfDaysValidity=(CMSD.Duration)*30 from  CCRMMembership CCRM,CMSPACKAGES CMSP,CMSDURATION CMSD,Users U where CMSP.PackageCode=CCRM.PackageCode and U.UCode=CCRM.MembershipCode and  CMSD.DurationID=CCRM.DurationId and  CCRM.MembershipExpireDate >= GETDATE() and CCRM.StartDate < GETDATE() and U.MobileNo='{0}' ) union all  select Name='Used',datediff(day,CCRMM.StartDate, getdate()) as Count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc) union all SELECT Name='Used',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS (select Name='Used',datediff(day,CCRMM.StartDate, getdate()) as Count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc))", MobileNo));
                        dt_SAUcount = getdata(string.Format("with UnionTable as  (select Name='Applied',datediff(day,CCRMM.StartDate, getdate()+(select count(*) as Count from OffLineSlotUsed OLSU,CMSSLOTTIMESETTING CMSSS  where  OLSU.IsDeleted=0 and CMSSS.SlotCode=OLSU.SlotID and  MobileNo='{0}' and  convert(varchar(10), CMSSS.SlotEndTime, 108) >=CONVERT(varchar,GETDATE(),8))) as count,PackageID='',PackageName='',BrancCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc)  union all select Name='Applied',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS (select Name='Applied',datediff(day,CCRMM.StartDate, getdate()+(select count(*) as Count from OffLineSlotUsed OLSU,CMSSLOTTIMESETTING CMSSS  where OLSU.IsDeleted=0 and CMSSS.SlotCode=OLSU.SlotID and  MobileNo='{0}' and  convert(varchar(10), CMSSS.SlotEndTime, 108) >=CONVERT(varchar,GETDATE(),8))) as count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc)) union all select top 1  Name='Applied',(select count(*) as Count from OffLineSlotUsed OLSU,CMSSLOTTIMESETTING CMSSS  where OLSU.IsDeleted=0 and CMSSS.SlotCode=OLSU.SlotID and  MobileNo='{0}' ) as Count,PackageID=0,PackageName=0,BranchCode,StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from Users )  SELECT   Name,count,PackageID,PackageName,BrancCode,StartDate,EndDate,NoofSessions,NumberOfDaysValidity FROM UnionTable GROUP BY Name,count,PackageID,PackageName,BrancCode,StartDate,EndDate,NoofSessions,NumberOfDaysValidity having MAX(count) >= 0 union all select  Name='NumberOfSession',count=(CMSD.Duration)*30,CCRM.PackageCode as PackageID,CMSP.PackageName,CCRM.BranchCode,CCRM.StartDate as  StartDate,CCRM.MembershipExpireDate as EndDate,NoofSessions=(CMSD.Duration)*30,NumberOfDaysValidity=(CMSD.Duration)*30 from  CCRMMembership CCRM,CMSPACKAGES CMSP,CMSDURATION CMSD,Users U where CMSP.PackageCode=CCRM.PackageCode and U.UCode=CCRM.MembershipCode and  CMSD.DurationID=CCRM.DurationId and  CCRM.MembershipExpireDate >= GETDATE() and CCRM.StartDate < GETDATE() and U.MobileNo='{0}' union all SELECT Name='NumberOfSession',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS ( select  Name='NumberOfSession',count=(CMSD.Duration)*30,CCRM.PackageCode as PackageID,CMSP.PackageName,CCRM.BranchCode,CCRM.StartDate as  StartDate,CCRM.MembershipExpireDate as EndDate,NoofSessions=(CMSD.Duration)*30,NumberOfDaysValidity=(CMSD.Duration)*30 from  CCRMMembership CCRM,CMSPACKAGES CMSP,CMSDURATION CMSD,Users U where CMSP.PackageCode=CCRM.PackageCode and U.UCode=CCRM.MembershipCode and  CMSD.DurationID=CCRM.DurationId and  CCRM.MembershipExpireDate >= GETDATE() and CCRM.StartDate < GETDATE() and U.MobileNo='{0}' ) union all  select Name='Used',datediff(day,CCRMM.StartDate, getdate()) as Count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc) union all SELECT Name='Used',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS (select Name='Used',datediff(day,CCRMM.StartDate, getdate()) as Count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc))", MobileNo));
                        //dt_SAUcount = getdata(string.Format("select  Name='NumberOfSession',count=(CMSD.Duration)*30,CCRM.PackageCode as PackageID,CMSP.PackageName,CCRM.BranchCode,CCRM.StartDate as  StartDate,CCRM.MembershipExpireDate as EndDate,NoofSessions=(CMSD.Duration)*30,NumberOfDaysValidity=(CMSD.Duration)*30 from  CCRMMembership CCRM,CMSPACKAGES CMSP,CMSDURATION CMSD,Users U where CMSP.PackageCode=CCRM.PackageCode and U.UCode=CCRM.MembershipCode and  CMSD.DurationID=CCRM.DurationId and  CCRM.MembershipExpireDate >= GETDATE() and CCRM.StartDate < GETDATE() and U.MobileNo='{0}' union all SELECT Name='NumberOfSession',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS ( select  Name='NumberOfSession',count=(CMSD.Duration)*30,CCRM.PackageCode as PackageID,CMSP.PackageName,CCRM.BranchCode,CCRM.StartDate as  StartDate,CCRM.MembershipExpireDate as EndDate,NoofSessions=(CMSD.Duration)*30,NumberOfDaysValidity=(CMSD.Duration)*30 from  CCRMMembership CCRM,CMSPACKAGES CMSP,CMSDURATION CMSD,Users U where CMSP.PackageCode=CCRM.PackageCode and U.UCode=CCRM.MembershipCode and  CMSD.DurationID=CCRM.DurationId and  CCRM.MembershipExpireDate >= GETDATE() and CCRM.StartDate < GETDATE() and U.MobileNo='9676600301' ) union all  select Name='Applied',datediff(day,CCRMM.StartDate, getdate()+(select count(*) as Count from OffLineSlotUsed OLSU,CMSSLOTTIMESETTING CMSSS  where CMSSS.SlotCode=OLSU.SlotID and  MobileNo='{0}' and  CONVERT(VARCHAR(101),SessionDate,103)>CONVERT(VARCHAR(101),GETDATE(),103))) as count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc)  union all select Name='Applied',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS (select Name='Applied',datediff(day,CCRMM.StartDate, getdate()+(select count(*) as Count from OffLineSlotUsed OLSU,CMSSLOTTIMESETTING CMSSS  where CMSSS.SlotCode=OLSU.SlotID and  MobileNo='{0}' and  CONVERT(VARCHAR(101),SessionDate,103)>CONVERT(VARCHAR(101),GETDATE(),103))) as count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc))  union all  select Name='Used',datediff(day,CCRMM.StartDate, getdate()) as Count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc) union all SELECT Name='Used',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS (select Name='Used',datediff(day,CCRMM.StartDate, getdate()) as Count,PackageID='',PackageName='',BranchCode='',StartDate='',EndDate='',NoofSessions=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,Users U where  CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}' and CCRMM.InvoiceID=(select Top 1 CCRMM.InvoiceID from CCRMMembership CCRMM,Users U where U.UCode=CCRMM.MembershipCode and  U.MobileNo='{0}'  and CCRMM.MembershipExpireDate >= GETDATE() order by CCRMM.ID desc) )", MobileNo));
                    }

                    //S = Convert.ToInt32(dt_SAUcount.Rows[2][1].ToString());
                    //S = Convert.ToInt32(dt_SAUcount.Rows[0][1].ToString());
                    //A = Convert.ToInt32(dt_SAUcount.Rows[1][1].ToString());
                    //U = Convert.ToInt32(dt_SAUcount.Rows[3][1].ToString());

                    S1 = Convert.ToInt32(dt_SAUcount.Rows[0][1].ToString());
                    S2 = Convert.ToInt32(dt_SAUcount.Rows[1][1].ToString());
                    if (S1 == 1)
                    {
                        S = S1;
                    }
                    else
                    {
                        S = S2;
                    }

                    if (Convert.ToInt32(dt_SAU.Rows[2]["count"].ToString()) > 0 && Convert.ToInt32(dt_Active.Rows[0]["IsExist"].ToString()) == 0)
                    {
                        S = Convert.ToInt32(dt_SAU.Rows[0][0].ToString());
                        A = Convert.ToInt32(dt_SAU.Rows[1][0].ToString());
                        U = Convert.ToInt32(dt_SAU.Rows[2][0].ToString());
                    }
                    else
                    {
                        S = Convert.ToInt32(dt_SAUcount.Rows[0][1].ToString());
                        A = Convert.ToInt32(dt_SAUcount.Rows[1][1].ToString());
                        U = Convert.ToInt32(dt_SAUcount.Rows[2][1].ToString());
                    }
                }
                else
                {
                    dt = getdata(string.Format("select ID,DisplayName from DashboardDisplay", ""));
                    dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM CCRMMembership AS T1,Users T2   WHERE  T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode)  SELECT (SELECT top 1 T1.EnquireTypeNo FROM CCRMMembership AS T1,Users T2  WHERE T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", MobileDeviceId));
                    //try
                    //{

                    //    dt_Active = getdata(string.Format("IF EXISTS (SELECT T1.MembershipExpireDate FROM CCRMMembership AS T1,Users T2  WHERE T1.StartDate >=GETDATE() and  T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode)   SELECT 1 as IsExist  ELSE SELECT 0 as IsExist", MobileNo));
                    //    dt_SessionDateByInvoice = getdata(string.Format("select Top 1 SessionDate from OffLineSlotUsed where InvoiceID=(select Top 1 InvoiceID from CCRMMembership where MobileNo='{0}' and MembershipExpireDate >= GETDATE() order by ID desc) order by ID desc", MobileNo));

                    //    DateTime dtt = Convert.ToDateTime(dt_SessionDateByInvoice.Rows[0]["SessionDate"].ToString());

                    //    var userdate = Convert.ToDateTime(dt_SessionDateByInvoice.Rows[0]["SessionDate"].ToString());
                    //    var date2 = userdate.ToShortDateString(); //Return 00/00/0000

                    //    if (Convert.ToDateTime(date2) >= Convert.ToDateTime(date))
                    //    {
                    //        dt_SAUcount = getdata(string.Format("select Top 1 Name='NumberOfSession',NumberOfSession=0,CCRMM.PackageCode,OLP.PackageName,CCRMM.BranchCode,CCRMM.StartDate,CCRMM.MembershipExpireDate as EndDate,NumberOfSession=0,NumberOfDaysValidity=0 from  CCRMMembership CCRMM,OnlinePackages OLP,SessionsCount SC where CCRMM.PackageCode=OLP.PackageID and  SC.Invoice=CCRMM.InvoiceID and SC.MobileDeviceID='{0}'  union all  select Name='Applied',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp where  Olpu.IsDeleted=0 and Olpp.Invoice=Olpu.InvoiceID and Olpu.MobileDeviceID='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileDeviceID='{0}'  and EndDate >= GETDATE() order by ID desc)  union all  select Name='Used',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp,OnlineSlotTimings OLST where  Olpu.IsDeleted=0 and  OLST.SlotCode=Olpu.SlotID and Olpp.Invoice=Olpu.InvoiceID and olpu.SessionDate <= GETDATE() and convert(varchar(10), OLST.SlotEndTime, 108) < CONVERT(TIME,GETDATE())  and Olpu.MobileDeviceID='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileDeviceID='{0}'  and EndDate >= GETDATE() order by ID desc)", MobileDeviceId));
                    //    }
                    //    else
                    //    {
                    //        dt_SAUcount = getdata(string.Format("select Top 1 Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLP.PackageName,OLPP.BranchCode,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP,SessionsCount SC where OLPP.PackageID=OLP.PackageID and  SC.Invoice=OLPP.Invoice and SC.MobileDeviceID='{0}' union all select Name='Applied',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp where  Olpu.IsDeleted=0 and Olpp.Invoice=Olpu.InvoiceID and Olpu.MobileDeviceID='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileDeviceID='{0}'  and EndDate >= GETDATE() order by ID desc) union all select Name='Used',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp,OnlineSlotTimings OLST where  Olpu.IsDeleted=0 and  OLST.SlotCode=Olpu.SlotID and Olpp.Invoice=Olpu.InvoiceID and olpu.SessionDate <= GETDATE() and Olpu.MobileDeviceID='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileDeviceID='{0}'  and EndDate >= GETDATE() order by ID desc)", MobileDeviceId));
                    //    }
                    //}
                    //catch (Exception ec)
                    //{
                    //    dt_SAUcount = getdata(string.Format("select Top 1 Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLP.PackageName,OLPP.BranchCode,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP,SessionsCount SC where OLPP.PackageID=OLP.PackageID and  SC.Invoice=OLPP.Invoice and SC.MobileDeviceID='{0}' union all select Name='Applied',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp where  Olpu.IsDeleted=0 and Olpp.Invoice=Olpu.InvoiceID and Olpu.MobileDeviceID='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileDeviceID='{0}'  and EndDate >= GETDATE() order by ID desc) union all select Name='Used',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp,OnlineSlotTimings OLST where  Olpu.IsDeleted=0 and  OLST.SlotCode=Olpu.SlotID and Olpp.Invoice=Olpu.InvoiceID and olpu.SessionDate <= GETDATE() and Olpu.MobileDeviceID='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileDeviceID='{0}'  and EndDate >= GETDATE() order by ID desc)", MobileDeviceId));
                    //}
                }

                if (Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 0 || Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 1)
                {

                    List<GetDashboard> dbPackage = new List<GetDashboard>();
                    GetDashboard dashboardlist = new GetDashboard { info = InfoDetails(0, 0), middleSection = GetMiddleSectionImages(Dashboard.AppId, 2, Convert.ToString(dt.Rows[1]["DisplayName"])), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                    dbPackage.Add(dashboardlist);
                    foutC.status = "success";
                    foutC.value = dbPackage;
                    sJSONResponse = JsonConvert.SerializeObject(foutC);

                }
                else if (Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 2)
                {

                    if (dt.Rows[0][0].ToString() == "1")
                    {
                        if (S > A && S > U && A == U)
                        {
                            FacilityId = getfacilityUsed(Dashboard.MobileNo);

                            if (FacilityId == "1")
                            {
                                status = "Freezing";
                            }
                            else
                            {
                                status = "Live";
                            }

                            TimeSpan Ts = Convert.ToDateTime(dt_SessionDateByInvoice.Rows[0][0].ToString()).Subtract(DateTime.Now);
                            TimeSpan Ts_changeslotDays;
                            ChangeSlot = 0;
                            PreRenewal = 0;

                            // logic for change

                            DataTable dt_slotchangebit = new DataTable();
                            dt_slotchangebit = getdata(string.Format("select MembershipCode,SlotCode,CreatedOn from ChangeSlotsDetails where InvoiceId='" + dt_SessionDateByInvoice.Rows[0][1].ToString() + "' "));

                            if (dt_slotchangebit.Rows.Count > 0)
                            {

                                Ts_changeslotDays = DateTime.Now.Subtract(Convert.ToDateTime(dt_slotchangebit.Rows[0]["CreatedOn"].ToString()));
                                if (Ts_changeslotDays.Days >= 15)
                                {
                                    ChangeSlot = 1;
                                }
                                else
                                {
                                    ChangeSlot = 0;
                                }

                            }
                            else
                            {
                                if (Ts.Days >= 15)
                                {
                                    ChangeSlot = 1;
                                }
                                else
                                {
                                    ChangeSlot = 0;
                                }
                            }

                            //if (Ts.Days >= 10)
                            //{
                            //    ChangeSlot = 1;
                            //}


                            if (Ts.Days >= 1)
                            {
                                PreRenewal = 1;
                            }

                            List<GetDashboard1> dbPackage = new List<GetDashboard1>();
                            GetDashboard1 dashboardlistpackage = new GetDashboard1 { info = InfoFacilityDetails(1, 2, status, ChangeSlot, PreRenewal), middleSection = GetMiddleSectionPackages(MobileNo, dt_SAUcount.Rows[0][3].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][7].ToString()), Convert.ToInt32(dt_SAUcount.Rows[0][8].ToString()), U), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            fout.status = "success";
                            fout.value = dbPackage;

                            sJSONResponse = JsonConvert.SerializeObject(fout);
                        }
                        else if (S > A && S > U && A > U)
                        {
                            List<GetSlotDetails> dbPackage = new List<GetSlotDetails>();
                            GetSlotDetails dashboardlistpackage = new GetSlotDetails { info = InfoDetails(2, 2), middleSection = GetMiddleSectionSlots(Dashboard.MobileNo, dt_SAUcount.Rows[0][3].ToString(), dt_SAUcount.Rows[0][2].ToString(), Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString())), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            foutB.status = "success";
                            foutB.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(foutB);
                        }

                        else if (S == A && S > U && A > U)
                        {
                            List<GetSlotDetails> dbPackage = new List<GetSlotDetails>();
                            GetSlotDetails dashboardlistpackage = new GetSlotDetails { info = InfoDetails(2, 2), middleSection = GetMiddleSectionSlots(Dashboard.MobileNo, dt_SAUcount.Rows[0][3].ToString(), dt_SAUcount.Rows[0][2].ToString(), Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString())), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            foutB.status = "success";
                            foutB.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(foutB);
                        }
                        else if (S < A && S == U && A > U)
                        {
                            List<GetSlotDetails> dbPackage = new List<GetSlotDetails>();
                            GetSlotDetails dashboardlistpackage = new GetSlotDetails { info = InfoDetails(2, 2), middleSection = GetMiddleSectionSlots(Dashboard.MobileNo, dt_SAUcount.Rows[0][3].ToString(), dt_SAUcount.Rows[0][2].ToString(), Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString())), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            foutB.status = "success";
                            foutB.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(foutB);
                        }
                        else if (S == A && S == U && A == U)
                        {
                            // changed 1/23/2021
                            FacilityId = getfacilityUsed(Dashboard.MobileNo);

                            status = "Expired";
                            ChangeSlot = 0;
                            PostRenewal = 1;
                            List<GetDashBoard3> dbPackage = new List<GetDashBoard3>();
                            GetDashBoard3 dashboardlistpackage = new GetDashBoard3 { info = InfoFacilityDetailsPost(3, 3, status, ChangeSlot, PostRenewal), middleSection = GetMiddleSectionExpirePackagesTrainers(MobileNo, dt_SAUcount.Rows[0][3].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][7].ToString()), Convert.ToInt32(dt_SAUcount.Rows[0][8].ToString()), U), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            foutE.status = "success";
                            foutE.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(foutE);

                        }
                        else
                        {
                            // ifns==0 handel it
                        }
                    }
                    else
                    {

                        List<GetDashboard> dbPackage = new List<GetDashboard>();
                        GetDashboard dashboardlistpackage = new GetDashboard { info = InfoDetails(3, 2), middleSection = GetMiddleSectionImages(Dashboard.AppId, 3, Convert.ToString(dt.Rows[1]["DisplayName"])), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                        dbPackage.Add(dashboardlistpackage);

                        foutC.status = "success";
                        foutC.value = dbPackage;
                        sJSONResponse = JsonConvert.SerializeObject(foutC);
                    }

                }
                else if (Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) >= 3)
                {
                    if (dt.Rows[0][0].ToString() == "1")
                    {
                        if (S > A && S > U && A == U)
                        {
                            FacilityId = getfacilityUsed(Dashboard.MobileNo);
                            if (FacilityId == "" || FacilityId == "0")
                            {
                                status = "Live";
                            }
                            else if (FacilityId == "1")
                            {
                                status = "Freezing";
                            }
                            else
                            {

                            }

                            TimeSpan Ts = DateTime.Now.Subtract(Convert.ToDateTime(dt_SessionDateByInvoice.Rows[0][0].ToString()));

                            //TimeSpan Ts = Convert.ToDateTime(dt_SessionDateByInvoice.Rows[0][0].ToString()).Subtract(DateTime.Now);
                            ChangeSlot = 0;
                            PreRenewal = 0;

                            //changeslot 0/1
                            //preRenew 0/1

                            if (Ts.Days >= 15)
                            {
                                ChangeSlot = 1;
                            }
                            if (Ts.Days >= 1)
                            {
                                PreRenewal = 1;
                            }

                            List<GetDashboard1Trainer> dbPackage = new List<GetDashboard1Trainer>();
                            GetDashboard1Trainer dashboardlistpackage = new GetDashboard1Trainer { info = InfoFacilityDetails(1, 3, status, ChangeSlot, PreRenewal), middleSection = GetMiddleSectionExpirePackagesDashboard3Trainers(MobileNo, dt_SAUcount.Rows[0][3].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][7].ToString()), Convert.ToInt32(dt_SAUcount.Rows[0][8].ToString()), U), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            foutF.status = "success";
                            foutF.value = dbPackage;

                            sJSONResponse = JsonConvert.SerializeObject(foutF);
                        }
                        else if (S > A && S > U && A > U)
                        {
                            List<GetSlotDetails> dbPackage = new List<GetSlotDetails>();
                            GetSlotDetails dashboardlistpackage = new GetSlotDetails { info = InfoDetails(2, 3), middleSection = GetMiddleSectionSlots(Dashboard.MobileNo, dt_SAUcount.Rows[0][3].ToString(), dt_SAUcount.Rows[0][2].ToString(), Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString())), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            foutB.status = "success";
                            foutB.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(foutB);
                        }

                        else if (S == A && S > U && A > U)
                        {
                            List<GetSlotDetails> dbPackage = new List<GetSlotDetails>();
                            GetSlotDetails dashboardlistpackage = new GetSlotDetails { info = InfoDetails(2, 3), middleSection = GetMiddleSectionSlots(Dashboard.MobileNo, dt_SAUcount.Rows[0][3].ToString(), dt_SAUcount.Rows[0][2].ToString(), Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString())), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            foutB.status = "success";
                            foutB.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(foutB);
                        }
                        else if (S == A && S == U && A == U)
                        {
                            status = "Expired";
                            ChangeSlot = 0;
                            PostRenewal = 1;

                            //

                            List<GetDashBoard3> dbPackage = new List<GetDashBoard3>();
                            GetDashBoard3 dashboardlistpackage = new GetDashBoard3 { info = InfoFacilityDetailsPost(3, 3, status, ChangeSlot, PostRenewal), middleSection = GetMiddleSectionExpirePackagesTrainers(MobileNo, dt_SAUcount.Rows[0][3].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][7].ToString()), Convert.ToInt32(dt_SAUcount.Rows[0][8].ToString()), U), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            foutE.status = "success";
                            foutE.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(foutE);
                        }
                        else
                        {
                            // ifns==0 handel it
                        }
                    }
                    else if (S == A && S > U && A > U)
                    {

                        status = "Expired";
                        ChangeSlot = 0;
                        PostRenewal = 1;

                        List<GetDashBoard3> dbPackage = new List<GetDashBoard3>();
                        GetDashBoard3 dashboardlistpackage = new GetDashBoard3 { info = InfoFacilityDetailsPost(3, 3, status, ChangeSlot, PostRenewal), middleSection = GetMiddleSectionExpirePackagesTrainers(MobileNo, dt_SAUcount.Rows[0][3].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][7].ToString()), Convert.ToInt32(dt_SAUcount.Rows[0][8].ToString()), U), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                        dbPackage.Add(dashboardlistpackage);
                        // foutC
                        foutE.status = "success";
                        foutE.value = dbPackage;
                        sJSONResponse = JsonConvert.SerializeObject(foutE);
                    }
                }
                else
                {
                    foutC.status = "Fail";
                    sJSONResponse = JsonConvert.SerializeObject(foutC);
                }

            }
            catch (Exception ec)
            {
                foutC.status = "Fail";
                sJSONResponse = JsonConvert.SerializeObject(foutC);
            }

            return sJSONResponse;

        }
        public List<MiddleSectionA> GetMiddleSectionPackages(string MobileNo, string PackageName, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        {
            List<MiddleSectionA> midsection = new List<MiddleSectionA>();
            midsection.Add(new MiddleSectionA { packages = GetPackagesA(MobileNo, PackageName, NoOfSession, NoOfValidity, NoofSlotsUsed) });
            return midsection;
        }
        public List<MiddleSectionA> GetMiddleSectionExpirePackages(string MobileNo, string PackageName, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        {


            List<MiddleSectionA> midsection = new List<MiddleSectionA>();
            midsection.Add(new MiddleSectionA { packages = GetExpirePackagesA(MobileNo, PackageName, NoOfSession, NoOfValidity, NoofSlotsUsed) });
            return midsection;
        }
        public List<MiddleSectionATrainerDetails> GetMiddleSectionExpirePackagesTrainers(string MobileNo, string PackageName, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        {
            List<MiddleSectionATrainerDetails> midsection = new List<MiddleSectionATrainerDetails>();
            midsection.Add(new MiddleSectionATrainerDetails { packages = GetExpirePackagesA(MobileNo, PackageName, NoOfSession, NoOfValidity, NoofSlotsUsed), trainerDetails = TrainerDetailsInternal(MobileNo) });
            return midsection;
        }

        public List<MiddleSectionATrainer> GetMiddleSectionExpirePackagesDashboard3Trainers(string MobileNo, string PackageName, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        {
            List<MiddleSectionATrainer> midsection = new List<MiddleSectionATrainer>();
            midsection.Add(new MiddleSectionATrainer { packages = GetPackagesA(MobileNo, PackageName, NoOfSession, NoOfValidity, NoofSlotsUsed), trainerDetails = TrainerDetailsInternal(MobileNo) });
            return midsection;
        }
        public Dictionary<string, object> TrainerDetailsInternal(string MobileNo)
        {

            Dictionary<string, object> TrainerObj = new Dictionary<string, object>();

            DataTable dt = new DataTable();
            dt = getdata(string.Format("Select top 1 OPU.TrainerCode as TrainerID,HRE.EmployeeName,OPU.BranchCode,HRE.PhotoUrl,OPU.PackageCode from CMSPACKAGES OP,CMSSLOTTIMESETTING OLST,CCRMMembership OPU,HREmployee HRE,Users U where  OPU.PackageCode=OP.PackageCode and OLST.SlotCode=OPU.SlotCode and HRE.EmployeeCode=OPU.TrainerCode and U.UCode=OPU.MembershipCode and U.MobileNo='{0}' and OPU.MembershipExpireDate >= GETDATE()  and OPU.StartDate<= GETDATE() Union all Select top 1 OPU.TrainerID,HRE.EmployeeName,HRE.BranchCode,HRE.PhotoUrl,OPU.PackageID as PackageCode from CMSPACKAGES OP,CMSSLOTTIMESETTING OLST,OffLineSlotUsed OPU,HREmployee HRE where  OPU.PackageID=OP.PackageCode and OLST.SlotCode=OPU.SlotID and HRE.EmployeeCode=OPU.TrainerID and OPU.MobileNo='{0}' and OPU.SessionDate < GETDATE()", MobileNo));


            if (dt.Rows.Count > 0)
            {
                TrainerObj.Add("trainerId", dt.Rows[0]["TrainerID"].ToString());
                TrainerObj.Add("employeeName", dt.Rows[0]["EmployeeName"].ToString());
                TrainerObj.Add("branchCode", dt.Rows[0]["BranchCode"].ToString());
                TrainerObj.Add("imageUrl", dt.Rows[0]["PhotoUrl"].ToString());
                TrainerObj.Add("packageCode", dt.Rows[0]["PackageCode"].ToString());
            }
            else
            {
                dt = getdata(string.Format("Select top 1 OPU.TrainerCode as TrainerID,HRE.EmployeeName,OPU.BranchCode,HRE.PhotoUrl,OPU.PackageCode from CMSPACKAGES OP,CMSSLOTTIMESETTING OLST,CCRMMembership OPU,HREmployee HRE,Users U where  OPU.PackageCode=OP.PackageCode and OLST.SlotCode=OPU.SlotCode and HRE.EmployeeCode=OPU.TrainerCode and U.UCode=OPU.MembershipCode and U.MobileNo='{0}' and OPU.MembershipExpireDate <= GETDATE()  and OPU.StartDate<= GETDATE() Union all Select top 1 OPU.TrainerID,HRE.EmployeeName,HRE.BranchCode,HRE.PhotoUrl,OPU.PackageID as PackageCode from CMSPACKAGES OP,CMSSLOTTIMESETTING OLST,OffLineSlotUsed OPU,HREmployee HRE where  OPU.PackageID=OP.PackageCode and OLST.SlotCode=OPU.SlotID and HRE.EmployeeCode=OPU.TrainerID and OPU.MobileNo='{0}' and OPU.SessionDate < GETDATE()", MobileNo));
                if (dt.Rows.Count > 0)
                {
                    TrainerObj.Add("trainerId", dt.Rows[0]["TrainerID"].ToString());
                    TrainerObj.Add("employeeName", dt.Rows[0]["EmployeeName"].ToString());
                    TrainerObj.Add("branchCode", dt.Rows[0]["BranchCode"].ToString());
                    TrainerObj.Add("imageUrl", dt.Rows[0]["PhotoUrl"].ToString());
                    TrainerObj.Add("packageCode", dt.Rows[0]["PackageCode"].ToString());
                }
                else
                {
                    dt = getdata(string.Format("Select top 1 OPU.TrainerCode as TrainerID,HRE.EmployeeName,OPU.BranchCode,HRE.PhotoUrl,OPU.PackageCode from CMSPACKAGES OP,CMSSLOTTIMESETTING OLST,CCRMMembership OPU,HREmployee HRE,Users U where  OPU.PackageCode=OP.PackageCode and OLST.SlotCode=OPU.SlotCode and HRE.EmployeeCode=OPU.TrainerCode and U.UCode=OPU.MembershipCode and U.MobileNo='{0}' and OPU.MembershipExpireDate >= GETDATE()  and OPU.StartDate >= GETDATE() Union all Select top 1 OPU.TrainerID,HRE.EmployeeName,HRE.BranchCode,HRE.PhotoUrl,OPU.PackageID as PackageCode from CMSPACKAGES OP,CMSSLOTTIMESETTING OLST,OffLineSlotUsed OPU,HREmployee HRE where  OPU.PackageID=OP.PackageCode and OLST.SlotCode=OPU.SlotID and HRE.EmployeeCode=OPU.TrainerID and OPU.MobileNo='{0}' and OPU.SessionDate > GETDATE()", MobileNo));

                    {
                        TrainerObj.Add("trainerId", dt.Rows[0]["TrainerID"].ToString());
                        TrainerObj.Add("employeeName", dt.Rows[0]["EmployeeName"].ToString());
                        TrainerObj.Add("branchCode", dt.Rows[0]["BranchCode"].ToString());
                        TrainerObj.Add("imageUrl", dt.Rows[0]["PhotoUrl"].ToString());
                        TrainerObj.Add("packageCode", dt.Rows[0]["PackageCode"].ToString());

                    }
                }
            }

           
            return TrainerObj;
        }
        public List<MiddleSectionA> GetMiddleSectionSlots(string MobileNo, string PackageName, string PackageId, int EnquireTypeNo)
        {
            List<MiddleSectionA> midsection = new List<MiddleSectionA>();
            midsection.Add(new MiddleSectionA { slots = GetSlotsA(MobileNo, PackageName, PackageId, EnquireTypeNo) });
            return midsection;
        }
        public List<MiddleSectionA> GetMiddleSectionImages(int AppId, int DisplayID, string ImageName)
        {
            List<MiddleSectionA> midsection = new List<MiddleSectionA>();
            midsection.Add(new MiddleSectionA { images = GetImageA(1, DisplayID, ImageName) });
            return midsection;
        }
        public List<Package> GetPackagesA(string MobileNo, string PackageName, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        {
            List<Package> packageA = new List<Package>();



            // DataTable dt_SAUcount = getdata(string.Format("select count(*) as count from ccrmmembership CCRMM,Users U  where CCRMM.MembershipCode=U.UCode and U.MobileNo='{0}'", MobileNo));


            packageA.Add(new Package { packageName = PackageName, packages = PackageDetails1(MobileNo, PackageName, NoOfSession, NoOfValidity, NoofSlotsUsed) });
            return packageA;

        }
        public List<Package> GetExpirePackagesA(string MobileNo, string PackageName, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        {
            List<Package> packageA = new List<Package>();
            DataTable dt = getdata(string.Format("select top 1  CMSP.PackageName from CCRMMembership CCRMM,Users U,CMSPACKAGES CMSP,CMSSLOTTIMESETTING CMST,CMSDURATION CMSD where CMSD.DurationID=CCRMM.DurationId and CMSP.PackageCode=CCRMM.PackageCode and CCRMM.SlotCode= CMST.SlotCode and U.UCode=CCRMM.MembershipCode and U.MobileNo='{0}'   union all  select Top 1 CMSP.PackageName from OffLineSlotUsed CCRMM,Users U,CMSPACKAGES CMSP,CMSSLOTTIMESETTING CMST,CMSDURATION CMSD where  CMSP.PackageCode=CCRMM.PackageID   and CCRMM.MobileNo='{0}' ", MobileNo));

            packageA.Add(new Package { packageName = dt.Rows[0]["PackageName"].ToString(), packages = ExpirePackageDetails1(MobileNo, PackageName, NoOfSession, NoOfValidity, NoofSlotsUsed) });
            return packageA;

        }
        public List<Slot> GetSlotsA(string MobileNo, string PackageName, string PackageId, int EnquireTypeNo)
        {
            List<Slot> packageB = new List<Slot>();
            List<Slots> SlotsB = new List<Slots>();

            SlotsB = SlotDetails(MobileNo, PackageName, PackageId, EnquireTypeNo);

            packageB.Add(new Slot { slotName = SlotsB[0].slotName, slots = SlotDetails(MobileNo, PackageName, PackageId, EnquireTypeNo) });
            return packageB;
        }
        public List<Image> GetImageA(int Appid, int DisplalID, string ImageName)
        {
            List<Image> ImageA = new List<Image>();
            ImageA.Add(new Image { imageName = ImageName, images = GetImages(1, DisplalID) });
            return ImageA;
        }
        public List<PackageDetails1> PackageDetails1(string MobileNo, string PackageName, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        {
            int number = NoofSlotsUsed;
            if (number < 0)
            {
                NoofSlotsUsed = 0;
            }
            List<PackageDetails1> ftd = new List<PackageDetails1>();


            DataTable dt = new DataTable();
            dt = getdata(string.Format("select TOP 1 CCRMM.BranchCode,CCRMM.PackageCode,CMSP.PackageName,CCRMM.StartDate,CCRMM.MembershipExpireDate,CMST.SlotName from CCRMMembership CCRMM,Users U,CMSPACKAGES CMSP,CMSSLOTTIMESETTING CMST,CMSDURATION CMSD where CMSD.DurationID=CCRMM.DurationId and CMSP.PackageCode=CCRMM.PackageCode and CCRMM.SlotCode= CMST.SlotCode and U.UCode=CCRMM.MembershipCode and CCRMM.MembershipExpireDate >= CAST( GETDATE() AS Date ) and CCRMM.StartDate <= CAST( GETDATE() AS Date ) and U.MobileNo='{0}' order by CCRMM.ID desc ", MobileNo));
            if (dt.Rows.Count <= 0)
            {
                dt = getdata(string.Format("select TOP 1 CCRMM.BranchCode,CCRMM.PackageCode,CMSP.PackageName,CCRMM.StartDate,CCRMM.MembershipExpireDate,CMST.SlotName from CCRMMembership CCRMM,Users U,CMSPACKAGES CMSP,CMSSLOTTIMESETTING CMST,CMSDURATION CMSD where CMSD.DurationID=CCRMM.DurationId and CMSP.PackageCode=CCRMM.PackageCode and CCRMM.SlotCode= CMST.SlotCode and U.UCode=CCRMM.MembershipCode and CCRMM.MembershipExpireDate >= CAST( GETDATE() AS Date ) and CCRMM.StartDate >= CAST( GETDATE() AS Date ) and U.MobileNo='{0}' order by CCRMM.ID desc ", MobileNo));

            }
            ftd.Add(new PackageDetails1
            {
                branchCode = Convert.ToString(dt.Rows[0]["BranchCode"])
                         ,
                packageID = Convert.ToString(dt.Rows[0]["PackageCode"])
                         ,
                packageName = Convert.ToString(dt.Rows[0]["PackageName"])
                         ,
                slotName = Convert.ToString(dt.Rows[0]["SlotName"])
                         ,
                slotStartDate = Convert.ToString(Convert.ToDateTime(dt.Rows[0]["StartDate"]).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
                         ,
                slotEndDate = Convert.ToString(Convert.ToDateTime(dt.Rows[0]["MembershipExpireDate"]).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
                         ,
                noOfSession = Convert.ToString(NoOfSession)
                         ,
                noOfValidity = Convert.ToString(NoOfValidity)
                ,
                noOfSlotsUsed = Convert.ToString(NoofSlotsUsed)

            });

            return ftd;
        }
        public List<PackageDetails1> ExpirePackageDetails1(string MobileNo, string PackageName, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        {
            int number = NoofSlotsUsed;
            if (number < 0)
            {
                NoofSlotsUsed = 0;
            }
            List<PackageDetails1> ftd = new List<PackageDetails1>();


            DataTable dt = new DataTable();
            dt = getdata(string.Format("select Top 1 CCRMM.ID,CCRMM.BranchCode,CCRMM.PackageCode,CMSP.PackageName,CCRMM.StartDate,CCRMM.MembershipExpireDate,CMST.SlotName,CMSD.Duration,noOfSession=(CMSD.Duration*30),noOfValidity=(CMSD.Duration*30),noOfSlotsUsed=(CMSD.Duration*30) from CCRMMembership CCRMM,Users U,CMSPACKAGES CMSP,CMSSLOTTIMESETTING CMST,CMSDURATION CMSD where CMSD.DurationID=CCRMM.DurationId and CMSP.PackageCode=CCRMM.PackageCode and CCRMM.SlotCode= CMST.SlotCode and U.UCode=CCRMM.MembershipCode and U.MobileNo='{0}'  union all select Top 1 CCRMM.ID,u.BranchCode,CCRMM.PackageID,CMSP.PackageName,CCRMM.SessionDate,CCRMM.SessionDate,CMST.SlotName,CMSD.Duration,noOfSession=(1),noOfValidity=(1),noOfSlotsUsed=(1) from OffLineSlotUsed CCRMM,Users U,CMSPACKAGES CMSP,CMSSLOTTIMESETTING CMST,CMSDURATION CMSD where  CMSP.PackageCode=CCRMM.PackageID and CCRMM.SlotID= CMST.SlotCode  and CCRMM.MobileNo='{0}'  order by CCRMM.ID desc ", MobileNo));
            if (dt.Rows.Count > 0)
            {
                ftd.Add(new PackageDetails1
                {
                    branchCode = Convert.ToString(dt.Rows[0]["BranchCode"])
                             ,
                    packageID = Convert.ToString(dt.Rows[0]["PackageCode"])
                             ,
                    packageName = Convert.ToString(dt.Rows[0]["PackageName"])
                             ,
                    slotName = Convert.ToString(dt.Rows[0]["SlotName"])
                             ,
                    slotStartDate = Convert.ToString(Convert.ToDateTime(dt.Rows[0]["StartDate"]).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
                             ,
                    slotEndDate = Convert.ToString(Convert.ToDateTime(dt.Rows[0]["MembershipExpireDate"]).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
                             ,
                    noOfSession = Convert.ToString(dt.Rows[0]["noOfSession"])
                             ,
                    noOfValidity = Convert.ToString(dt.Rows[0]["noOfValidity"])
                    ,
                    noOfSlotsUsed = Convert.ToString(dt.Rows[0]["noOfSlotsUsed"])

                });
            }
            return ftd;
        }
        public List<Slots> SlotDetails(string MobileNo, string PackageName, string PackageId, int EnquireTypeNo)
        {
            List<Slots> ftd = new List<Slots>();


            DataTable dt = new DataTable();

            if (EnquireTypeNo == 2)
            {
                dt = getdata(string.Format("select CCRMES.ID as EnquiryId,OLPU.ID,OLPU.PackageID,CMSP.PackageName,OLPU.InvoiceID,ST.SessionCode as  SessionID,OLPU.SessionDate,OLPU.SlotID,ST.SlotName,convert(varchar(10), ST.SlotStartTime, 108) as SlotStartTime,convert(varchar(10), ST.SlotEndTime, 108) as SlotEndTime from OffLineSlotUsed OLPU,CMSSLOTTIMESETTING ST,CMSPACKAGES CMSP,CCRMMEnquireStatus CCRMES where CCRMES.MobileNo=OLPU.MobileNo and CCRMES.EnquireTypeNo=OLPU.EnquireTypeNo  and OLPU.IsDeleted=0 and  CMSP.PackageCode=OLPU.PackageID and ST.SlotCode=OLPU.SlotID  and OLPU.MobileNo='{0}' order by OLPU.ID desc ", MobileNo));
            }
            else
            {
                dt = getdata(string.Format("select CCRMES.ID as EnquiryId,OLPU.ID,CCRMM.PackageCode as PackageID,CMSP.PackageName,CCRMM.InvoiceID,ST.SessionCode as  SessionID,OLPU.SessionDate,OLPU.SlotID,ST.SlotName,CONVERT(VARCHAR(5),ST.SlotStartTime, 108) + ' ' + RIGHT(CONVERT(VARCHAR(30),ST.SlotStartTime, 9),2)  as SlotStartTime,CONVERT(VARCHAR(5),ST.SlotEndTime, 108) + ' ' + RIGHT(CONVERT(VARCHAR(30),ST.SlotEndTime, 9),2) as SlotEndTime  from CCRMMembership CCRMM,OffLineSlotUsed OLPU,CMSSLOTTIMESETTING ST,CMSPACKAGES CMSP,CCRMMEnquireStatus CCRMES where CCRMES.MobileNo=OLPU.MobileNo and CCRMES.EnquireTypeNo=OLPU.EnquireTypeNo  and OLPU.IsDeleted=0 and CMSP.PackageCode=OLPU.PackageID and ST.SlotCode=CCRMM.SlotCode and CCRMM.SlotCode=OLPU.SlotID and CCRMM.InvoiceID=OLPU.InvoiceID and CCRMM.PackageCode=OLPU.PackageID and OLPU.MobileNo='{0}' order by OLPU.ID desc ", MobileNo));

            }

            ftd.Add(new Slots
            {
                EnquiryId = Convert.ToInt32(dt.Rows[0]["EnquiryId"])
                 ,
                ID = Convert.ToInt32(dt.Rows[0]["ID"])
                 ,
                packageID = Convert.ToString(dt.Rows[0]["PackageID"])
                 ,
                packageName = Convert.ToString(dt.Rows[0]["PackageName"])
                 ,
                invoiceID = Convert.ToString(dt.Rows[0]["InvoiceID"])
                ,
                sessionID = Convert.ToString(dt.Rows[0]["SessionID"])
                 ,
                sessionDate = Convert.ToString(Convert.ToDateTime(dt.Rows[0]["SessionDate"]).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
                ,
                slotID = Convert.ToString(dt.Rows[0]["SlotID"])
                 ,
                slotName = Convert.ToString(dt.Rows[0]["SlotName"])
                 ,
                slotStartDate = Convert.ToString(Convert.ToDateTime(dt.Rows[0]["SessionDate"]).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
                 ,
                slotEndDate = Convert.ToString(Convert.ToDateTime(dt.Rows[0]["SessionDate"]).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
            });

            return ftd;

        }

        //status : live freez hold : faclities 
        public string getfacilityUsed(string MobileNo)
        {
            string facilityUsed = "";
            DataTable dt = getdata(string.Format("select CCRMMF.MFDID from CCRMMembershipFacility CCRMMF,Users U  where U.UCode=CCRMMF.MemberShipCode and GETDATE()  between CCRMMF.FacilityStartDate and CCRMMF.FacilityExpireDate and U.MobileNo='{0}'  order by CCRMMF.SNO desc ", MobileNo));
            if (dt.Rows.Count > 0)
            {
                facilityUsed = dt.Rows[0]["MFDID"].ToString();
            }
            else
            {
                facilityUsed = "0";
            }

            return facilityUsed;
        }
        public List<Info> InfoDetails(int isPackages, int enquireTypeNo)
        {
            List<Info> ftd = new List<Info>();

            ftd.Add(new Info
            {
                isPackages = isPackages
                     ,
                enquireTypeNo = enquireTypeNo

            });

            return ftd;

        }
        public List<InfoFaclity> InfoFacilityDetails(int isPackages, int enquireTypeNo, string status, int ChangeSlot, int PreRenewal)
        {
            List<InfoFaclity> ftd = new List<InfoFaclity>();

            ftd.Add(new InfoFaclity
            {
                isPackages = isPackages
                     ,
                enquireTypeNo = enquireTypeNo
                ,
                status = status
                ,
                changeSlot = ChangeSlot
                ,
                preRenewal = PreRenewal
            });

            return ftd;

        }
        public List<InfoFacilityPost> InfoFacilityDetailsPost(int isPackages, int enquireTypeNo, string status, int ChangeSlot, int PostRenewal)
        {
            List<InfoFacilityPost> ftd = new List<InfoFacilityPost>();

            ftd.Add(new InfoFacilityPost
            {
                isPackages = isPackages
                     ,
                enquireTypeNo = enquireTypeNo
                ,
                status = status
                ,
                changeSlot = ChangeSlot
                ,
                postRenewal = PostRenewal
            });

            return ftd;

        }
        public List<ImageDetails> GetImages(int AppID, int Display)
        {
            List<ImageDetails> Imagenamess = new List<ImageDetails>();

            DataTable dt = getdata(string.Format("select ID as ImageID,ImageURL from DashboardImages where AppId='{0}' and DisplayId='{1}'  and IsDeleted=0 and ScreenId=2 ", AppID, Display));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Imagenamess.Add(new ImageDetails
                {
                    id = Convert.ToInt32(dt.Rows[i]["imageid"])
                    ,
                    url = Convert.ToString(dt.Rows[i]["ImageURL"])
                });

            }
            return Imagenamess;
        }
        public List<PackageDetails2> PackageDetails2(string Branchcode, int PackageId, string PackageName, string StartDate, string Enddate)
        {
            List<PackageDetails2> ftd = new List<PackageDetails2>();
            ftd.Add(new PackageDetails2
            {
                branchCode = Convert.ToString(Branchcode)
                 ,
                packageID = Convert.ToString(PackageId)
                 ,
                packageName = Convert.ToString(PackageName)
                 ,
                slotStartDate = Convert.ToString(Convert.ToDateTime(StartDate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
                 ,
                slotEndDate = Convert.ToString(Convert.ToDateTime(Enddate).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
            });

            return ftd;
        }


    }
}