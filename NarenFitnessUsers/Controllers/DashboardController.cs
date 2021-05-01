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
using NarenFitnessUsers.Models.SpaceManagement;
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
    public class DashboardController : Controller
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
                SessionDate = Convert.ToString(dt.Rows[0]["SessionDate"])
                                           ,
                SlotTime = Convert.ToString(dt.Rows[0]["SlotTime"])
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
        public List<ImageName> GetPackages1(string Branchcode, int PackageId, string PackageName, string StartDate, string Enddate, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        {
            List<ImageName> allpackageall = new List<ImageName>();
            allpackageall.Add(new ImageName { packageName = PackageName, packages = PackageDetails1(Branchcode, PackageId, PackageName, StartDate, Enddate, NoOfSession, NoOfValidity, NoofSlotsUsed) });
            return allpackageall;
        }
        public List<GetFreeTrialsAll> GetImages(int AppId, string ImageName)
        {
            List<GetFreeTrialsAll> allFreetrial = new List<GetFreeTrialsAll>();
            allFreetrial.Add(new GetFreeTrialsAll { ImageName = ImageName, Images = GetFreeTrailImage(AppId) });
            return allFreetrial;
        }
        public Object GetLivePTPackages([FromBody]Dashboard Dashboard)
        {
            FinalPackagePriceClassOutPut FPPCOP = new FinalPackagePriceClassOutPut();
            FinalPackagePrice2ClassOutPut FPPC2OP = new FinalPackagePrice2ClassOutPut();
            FinalSlotDetailsClassOutPut FSDCOP = new FinalSlotDetailsClassOutPut();

            string sJSONResponse = "";

            DataSet DashboardFT = new DataSet();
            int S = 0;
            int A = 0;
            int U = 0;

            try
            {

                GetApplicationInfoFail gaif = new GetApplicationInfoFail();
                DataTable dt_SAUcount = getdata(string.Format("select Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLPP.BranchCode,StartDate,EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP where OLPP.PackageID=OLP.PackageID and MobileNo='{0}'  and EndDate >= GETDATE() union all select Name='Applied',COUNT(*) as count,PackageID=0,BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp where Olpp.MobileNo=Olpu.MobileNo and Olpu.MobileNo='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileNo='{0}'  and EndDate >= GETDATE() order by ID asc)  union all select Name='Used',COUNT(*) as count,PackageID=0,BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp where Olpp.MobileNo=Olpu.MobileNo and olpu.SessionDate <= GETDATE() and Olpu.MobileNo='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileNo='{0}'  and EndDate >= GETDATE() order by ID asc)", Dashboard.MobileNo));
                DataTable dt = getdata(string.Format("select ID,DisplayName from DashboardDisplay", ""));
                DataTable dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM OnlinePackagePurchase AS T1  WHERE T1.MobileNo='{0}' )  SELECT (SELECT EnquireTypeNo FROM OnlinePackagePurchase AS T1  WHERE T1.MobileNo='{0}') as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", Dashboard.MobileNo));
                DataTable dt_Active = getdata(string.Format("IF EXISTS (SELECT EndDate FROM OnlinePackagePurchase AS T1  WHERE T1.EndDate >=GETDATE() and  T1.MobileNo='{0}')   SELECT 1 as IsExist  ELSE SELECT 0 as IsExist", Dashboard.MobileNo));

                try
                {
                    S = Convert.ToInt32(dt_SAUcount.Rows[0][1].ToString());
                    A = Convert.ToInt32(dt_SAUcount.Rows[1][1].ToString());
                    U = Convert.ToInt32(dt_SAUcount.Rows[2][1].ToString());
                }
                catch (Exception ec)
                {
                }

                if (Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 0)
                {

                    List<PackagePricesClass> dbfreetrial = new List<PackagePricesClass>();
                    PackagePricesClass dashboardlist = new PackagePricesClass { isPackages = Convert.ToInt32(0), packages = GetPackagePriceDetails(2), enquireTypeNo = 0, imageName = Convert.ToString(dt.Rows[1]["DisplayName"]), image = GetFreeTrailImage(Dashboard.AppId), bannerName = Convert.ToString(dt.Rows[0]["DisplayName"]), banner = GetBannerImages(Dashboard.AppId), textHeader = Convert.ToString(dt.Rows[3]["DisplayName"]), text = GetText() };
                    dbfreetrial.Add(dashboardlist);
                    FPPCOP.status = "success";
                    FPPCOP.value = dbfreetrial;
                    sJSONResponse = JsonConvert.SerializeObject(FPPCOP);

                }
                else if (Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 2)
                {
                    if (dt.Rows[0][0].ToString() == "1")
                    {
                        if (S > A && S > U && A == U)
                        {
                            List<PackagePrices2Class> dbPackage = new List<PackagePrices2Class>();
                            PackagePrices2Class dashboardlistpackage = new PackagePrices2Class { isPackages = Convert.ToInt32(1), packages = GetPackagePriceDetails(3), enquireTypeNo = 2, packageDetails = PackageDetails1(dt_SAUcount.Rows[0][1].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][1].ToString()), dt_SAUcount.Rows[0][1].ToString(), dt_SAUcount.Rows[0][1].ToString(), dt_SAUcount.Rows[0][1].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][1].ToString()), Convert.ToInt32(dt_SAUcount.Rows[0][1].ToString()), U), bannerName = Convert.ToString(dt.Rows[0]["DisplayName"]), banner = GetBannerImages(Dashboard.AppId), textHeader = Convert.ToString(dt.Rows[3]["DisplayName"]), text = GetText() };
                            dbPackage.Add(dashboardlistpackage);


                            FPPC2OP.status = "success";
                            FPPC2OP.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(FPPC2OP);


                        }
                        else if (S > A && S > U && A > U)
                        {
                            List<GetSlotPDetails2> dbPackage = new List<GetSlotPDetails2>();
                            GetSlotPDetails2 dashboardlistpackage = new GetSlotPDetails2 { isPackages = Convert.ToInt32(2), packages = GetPackagePriceDetails(3), enquireTypeNo = 2, slotDetails = PackageDetails2(dt_SAUcount.Rows[0][1].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][1].ToString()), dt_SAUcount.Rows[0][1].ToString(), dt_SAUcount.Rows[0][1].ToString(), dt_SAUcount.Rows[0][1].ToString()), bannerName = Convert.ToString(dt.Rows[0]["DisplayName"]), banner = GetBannerImages(Dashboard.AppId), textHeader = Convert.ToString(dt.Rows[3]["DisplayName"]), text = GetText() };
                            dbPackage.Add(dashboardlistpackage);

                            FSDCOP.status = "success";
                            FSDCOP.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(FSDCOP);


                        }
                        else if (S == A && S == U && A == U)
                        {
                            List<PackagePrices2Class> dbPackage = new List<PackagePrices2Class>();
                            PackagePrices2Class dashboardlistpackage = new PackagePrices2Class { isPackages = Convert.ToInt32(3), packages = GetPackagePriceDetails(3), enquireTypeNo = 2, imageName = Convert.ToString(dt.Rows[1]["DisplayName"]), image = GetPackageImage(Dashboard.AppId), bannerName = Convert.ToString(dt.Rows[0]["DisplayName"]), banner = GetBannerImages(Dashboard.AppId), textHeader = Convert.ToString(dt.Rows[3]["DisplayName"]), text = GetText() };
                            dbPackage.Add(dashboardlistpackage);

                            FPPC2OP.status = "success";
                            FPPC2OP.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(FPPC2OP);


                        }
                    }
                    else
                    {

                        List<PackagePrices2Class> dbPackage = new List<PackagePrices2Class>();
                        PackagePrices2Class dashboardlistpackage = new PackagePrices2Class { isPackages = Convert.ToInt32(3), packages = GetPackagePriceDetails(3), enquireTypeNo = 2, imageName = Convert.ToString(dt.Rows[1]["DisplayName"]), image = GetPackageImage(Dashboard.AppId), bannerName = Convert.ToString(dt.Rows[0]["DisplayName"]), banner = GetBannerImages(Dashboard.AppId), textHeader = Convert.ToString(dt.Rows[3]["DisplayName"]), text = GetText() };
                        dbPackage.Add(dashboardlistpackage);
                        FPPC2OP.status = "success";
                        FPPC2OP.value = dbPackage;
                        sJSONResponse = JsonConvert.SerializeObject(FPPC2OP);


                    }

                }
                else if (Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 3)
                {
                    if (dt.Rows[0][0].ToString() == "1")
                    {
                        if (S > A && S > U && A == U)
                        {
                            List<PackagePrices2Class> dbPackage = new List<PackagePrices2Class>();
                            PackagePrices2Class dashboardlistpackage = new PackagePrices2Class { isPackages = Convert.ToInt32(1), packages = GetPackagePriceDetails(3), enquireTypeNo = 2, packageDetails = PackageDetails1(dt_SAUcount.Rows[0][1].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][1].ToString()), dt_SAUcount.Rows[0][1].ToString(), dt_SAUcount.Rows[0][1].ToString(), dt_SAUcount.Rows[0][1].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][1].ToString()), Convert.ToInt32(dt_SAUcount.Rows[0][1].ToString()), U), bannerName = Convert.ToString(dt.Rows[0]["DisplayName"]), banner = GetBannerImages(Dashboard.AppId), textHeader = Convert.ToString(dt.Rows[3]["DisplayName"]), text = GetText() };
                            dbPackage.Add(dashboardlistpackage);


                            FPPC2OP.status = "success";
                            FPPC2OP.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(FPPC2OP);


                        }
                        else if (S > A && S > U && A > U)
                        {
                            List<GetSlotPDetails2> dbPackage = new List<GetSlotPDetails2>();
                            GetSlotPDetails2 dashboardlistpackage = new GetSlotPDetails2 { isPackages = Convert.ToInt32(2), packages = GetPackagePriceDetails(3), enquireTypeNo = 2, slotDetails = PackageDetails2(dt_SAUcount.Rows[0][1].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][1].ToString()), dt_SAUcount.Rows[0][1].ToString(), dt_SAUcount.Rows[0][1].ToString(), dt_SAUcount.Rows[0][1].ToString()), bannerName = Convert.ToString(dt.Rows[0]["DisplayName"]), banner = GetBannerImages(Dashboard.AppId), textHeader = Convert.ToString(dt.Rows[3]["DisplayName"]), text = GetText() };
                            dbPackage.Add(dashboardlistpackage);

                            FSDCOP.status = "success";
                            FSDCOP.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(FSDCOP);


                        }
                        else if (S == A && S == U && A == U)
                        {
                            List<PackagePrices2Class> dbPackage = new List<PackagePrices2Class>();
                            PackagePrices2Class dashboardlistpackage = new PackagePrices2Class { isPackages = Convert.ToInt32(3), packages = GetPackagePriceDetails(3), enquireTypeNo = 2, imageName = Convert.ToString(dt.Rows[1]["DisplayName"]), image = GetPackageImage(Dashboard.AppId), bannerName = Convert.ToString(dt.Rows[0]["DisplayName"]), banner = GetBannerImages(Dashboard.AppId), textHeader = Convert.ToString(dt.Rows[3]["DisplayName"]), text = GetText() };
                            dbPackage.Add(dashboardlistpackage);

                            FPPC2OP.status = "success";
                            FPPC2OP.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(FPPC2OP);


                        }
                    }
                    else
                    {

                        List<PackagePrices2Class> dbPackage = new List<PackagePrices2Class>();
                        PackagePrices2Class dashboardlistpackage = new PackagePrices2Class { isPackages = Convert.ToInt32(3), packages = GetPackagePriceDetails(3), enquireTypeNo = 2, imageName = Convert.ToString(dt.Rows[1]["DisplayName"]), image = GetPackageImage(Dashboard.AppId), bannerName = Convert.ToString(dt.Rows[0]["DisplayName"]), banner = GetBannerImages(Dashboard.AppId), textHeader = Convert.ToString(dt.Rows[3]["DisplayName"]), text = GetText() };
                        dbPackage.Add(dashboardlistpackage);
                        FPPC2OP.status = "success";
                        FPPC2OP.value = dbPackage;
                        sJSONResponse = JsonConvert.SerializeObject(FPPC2OP);


                    }

                }
                else
                {

                }
            }
            catch (Exception ec)
            {
                FPPC2OP.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(FPPC2OP);
            }
            return sJSONResponse;
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

            DataTable dt = getdata(string.Format("select ID as ImageID,ImageURL from DashboardImages where AppId='{0}' and DisplayId=1 and IsDeleted=0 and ScreenId=1", AppId));
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

            DataTable dt = getdata(string.Format("select ID as TextID,Text as TextName from DashboardText where DisplayID=4 and ScreenId=1", ""));
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

        // 28/08/2020: Need to add Bit Column Pervious Trainer(0,1) in Packages of middle section
        public Object GetDashboard([FromBody]Dashboard Dashboard)
        {
            DataTable dt = new DataTable();
            DataTable dt_IsPackage = new DataTable();
            DataTable dt_Active = new DataTable();
            DataTable dt_SessionDateByInvoice = new DataTable();

            DataSet DashboardFT = new DataSet();
            int S = 0;
            int A = 0;
            int U = 0;
            FinalOutPut fout = new FinalOutPut();
            FinalOutPutB foutB = new FinalOutPutB();
            FinalOutPutC foutC = new FinalOutPutC();
            FinalOutPutCTrainer foutD = new FinalOutPutCTrainer();

            //

            string sJSONResponse = "";
            var dateTimeNow = DateTime.Now; // Return 00/00/0000 00:00:00
            var date = dateTimeNow.ToShortDateString(); //Return 00/00/0000

            string MobileNo = "";
            string MobileDeviceId = "";

            MobileNo = Dashboard.MobileNo;
            MobileDeviceId = Dashboard.MobileDeviceID;

            DataTable dt_SAUcount = new DataTable();
            DataTable dt_Packagename = new DataTable();
            try
            {
                dt = getdata(string.Format("select ID,DisplayName from DashboardDisplay", ""));
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

                if (MobileNo != "")
                {
                    try
                    {

                        dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM OnlinePackagePurchase AS T1  WHERE T1.MobileNo='{0}')  SELECT (SELECT top 1 EnquireTypeNo FROM OnlinePackagePurchase AS T1  WHERE T1.MobileNo='{0}' order by ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", MobileNo));
                        dt_Active = getdata(string.Format("IF EXISTS (SELECT EndDate FROM OnlinePackagePurchase AS T1  WHERE T1.EndDate >=GETDATE() and  T1.MobileNo='{0}')   SELECT 1 as IsExist  ELSE SELECT 0 as IsExist", MobileNo));
                        dt_SessionDateByInvoice = getdata(string.Format("select Top 1 SessionDate from OnlinePackageUsed where InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where MobileNo='{0}' and EndDate >= GETDATE() order by ID desc) order by ID desc", MobileNo));
                        dt_Packagename = getdata(string.Format("Select top 1 OP.PackageName from OnlinePackagePurchase OPP,OnlinePackages OP,OnlineSlotTimings OLST,OnlinePackageUsed OPU,Users U where  OPP.PackageID=OPU.PackageID and  OPP.PackageID=OP.PackageID and OLST.SlotCode=OPU.SlotID and U.UCode=OPP.MembershipCode and U.MobileNo='{0}'  order by StartDate desc", MobileNo));


                        DateTime dtt = Convert.ToDateTime(dt_SessionDateByInvoice.Rows[0]["SessionDate"].ToString());

                        var userdate = Convert.ToDateTime(dt_SessionDateByInvoice.Rows[0]["SessionDate"].ToString());
                        var date2 = userdate.ToShortDateString(); //Return 00/00/0000

                        if (Convert.ToDateTime(date2) >= Convert.ToDateTime(date))
                        {
                            dt_SAUcount = getdata(string.Format("select Top 1 Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLP.PackageName,OLPP.BranchCode,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP,SessionsCount SC where OLPP.PackageID=OLP.PackageID and  SC.Invoice=OLPP.Invoice and SC.MobileNo='{0}' and OLPP.EndDate>=GETDATE() union all SELECT Name='NumberOfSession',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS ( select Top 1 Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLP.PackageName,OLPP.BranchCode,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP,SessionsCount SC where OLPP.PackageID=OLP.PackageID and  SC.Invoice=OLPP.Invoice and SC.MobileNo='{0}' and OLPP.EndDate>=GETDATE())  union all select Name='Applied',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp where  Olpu.IsDeleted=0 and Olpp.Invoice=Olpu.InvoiceID and Olpu.MobileNo='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileNo='{0}'  and EndDate >= GETDATE() order by ID desc) union all select Name='Used',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp,OnlineSlotTimings OLST where  Olpu.IsDeleted=0 and  OLST.SlotCode=Olpu.SlotID and Olpp.Invoice=Olpu.InvoiceID and olpu.SessionDate <= GETDATE() and convert(varchar(10), OLST.SlotEndTime, 108) < CONVERT(TIME,GETDATE())  and Olpu.MobileNo='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileNo='{0}'  and EndDate >= GETDATE() order by ID desc)", MobileNo));
                        }
                        else
                        {
                            dt_SAUcount = getdata(string.Format("select Top 1 Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLP.PackageName,OLPP.BranchCode,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP,SessionsCount SC where OLPP.PackageID=OLP.PackageID and  SC.Invoice=OLPP.Invoice and SC.MobileNo='{0}' and OLPP.EndDate>=GETDATE() union all SELECT Name='NumberOfSession',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS ( select Top 1 Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLP.PackageName,OLPP.BranchCode,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP,SessionsCount SC where OLPP.PackageID=OLP.PackageID and  SC.Invoice=OLPP.Invoice and SC.MobileNo='{0}' and OLPP.EndDate>=GETDATE())  union all select Name='Applied',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp where  Olpu.IsDeleted=0 and Olpp.Invoice=Olpu.InvoiceID and Olpu.MobileNo='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileNo='{0}'  and EndDate >= GETDATE() order by ID desc) union all select Name='Used',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp,OnlineSlotTimings OLST where  Olpu.IsDeleted=0 and  OLST.SlotCode=Olpu.SlotID and Olpp.Invoice=Olpu.InvoiceID and olpu.SessionDate <= GETDATE() and Olpu.MobileNo='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileNo='{0}'  and EndDate >= GETDATE() order by ID desc)", MobileNo));
                        }
                    }
                    catch (Exception ec)
                    {
                        dt_SAUcount = getdata(string.Format("select Top 1 Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLP.PackageName,OLPP.BranchCode,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP,SessionsCount SC where OLPP.PackageID=OLP.PackageID and  SC.Invoice=OLPP.Invoice and SC.MobileNo='{0}' and OLPP.EndDate>=GETDATE() union all SELECT Name='NumberOfSession',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS ( select Top 1 Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLP.PackageName,OLPP.BranchCode,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP,SessionsCount SC where OLPP.PackageID=OLP.PackageID and  SC.Invoice=OLPP.Invoice and SC.MobileNo='{0}' and OLPP.EndDate>=GETDATE()) union all select Name='Applied',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp where  Olpu.IsDeleted=0 and Olpp.Invoice=Olpu.InvoiceID and Olpu.MobileNo='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileNo='{0}'  and EndDate >= GETDATE() order by ID desc) union all select Name='Used',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp,OnlineSlotTimings OLST where  Olpu.IsDeleted=0 and  OLST.SlotCode=Olpu.SlotID and Olpp.Invoice=Olpu.InvoiceID and olpu.SessionDate <= GETDATE() and Olpu.MobileNo='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileNo='{0}'  and EndDate >= GETDATE() order by ID desc)", MobileNo));
                    }
                }
                //else
                //{
                //    try
                //    {
                //        dt = getdata(string.Format("select ID,DisplayName from DashboardDisplay", ""));
                //        dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM OnlinePackagePurchase AS T1  WHERE T1.MobileDeviceID='{0}')  SELECT (SELECT top 1 EnquireTypeNo FROM OnlinePackagePurchase AS T1  WHERE T1.MobileDeviceID='{0}' order by ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", MobileDeviceId));
                //        dt_Active = getdata(string.Format("IF EXISTS (SELECT EndDate FROM OnlinePackagePurchase AS T1  WHERE T1.EndDate >=GETDATE() and  T1.MobileDeviceID='{0}')   SELECT 1 as IsExist  ELSE SELECT 0 as IsExist", MobileDeviceId));
                //        dt_SessionDateByInvoice = getdata(string.Format("select Top 1 SessionDate from OnlinePackageUsed where InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where MobileDeviceID='{0}' and EndDate >= GETDATE() order by ID desc) order by ID desc", MobileDeviceId));

                //        DateTime dtt = Convert.ToDateTime(dt_SessionDateByInvoice.Rows[0]["SessionDate"].ToString());

                //        var userdate = Convert.ToDateTime(dt_SessionDateByInvoice.Rows[0]["SessionDate"].ToString());
                //        var date2 = userdate.ToShortDateString(); //Return 00/00/0000

                //        if (Convert.ToDateTime(date2) >= Convert.ToDateTime(date))
                //        {
                //            dt_SAUcount = getdata(string.Format("select Top 1 Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLP.PackageName,OLPP.BranchCode,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP,SessionsCount SC where OLPP.PackageID=OLP.PackageID and  SC.Invoice=OLPP.Invoice and SC.MobileDeviceID='{0}' union all SELECT Name='NumberOfSession',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS ( select Top 1 Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLP.PackageName,OLPP.BranchCode,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP,SessionsCount SC where OLPP.PackageID=OLP.PackageID and  SC.Invoice=OLPP.Invoice and SC.MobileNo='{0}')   union all  select Name='Applied',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp where  Olpu.IsDeleted=0 and Olpp.Invoice=Olpu.InvoiceID and Olpu.MobileDeviceID='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileDeviceID='{0}'  and EndDate >= GETDATE() order by ID desc)  union all  select Name='Used',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp,OnlineSlotTimings OLST where  Olpu.IsDeleted=0 and  OLST.SlotCode=Olpu.SlotID and Olpp.Invoice=Olpu.InvoiceID and olpu.SessionDate <= GETDATE() and convert(varchar(10), OLST.SlotEndTime, 108) < CONVERT(TIME,GETDATE())  and Olpu.MobileDeviceID='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileDeviceID='{0}'  and EndDate >= GETDATE() order by ID desc)", MobileDeviceId));
                //        }
                //        else
                //        {
                //            dt_SAUcount = getdata(string.Format("select Top 1 Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLP.PackageName,OLPP.BranchCode,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP,SessionsCount SC where OLPP.PackageID=OLP.PackageID and  SC.Invoice=OLPP.Invoice and SC.MobileDeviceID='{0}' union all SELECT Name='NumberOfSession',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS ( select Top 1 Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLP.PackageName,OLPP.BranchCode,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP,SessionsCount SC where OLPP.PackageID=OLP.PackageID and  SC.Invoice=OLPP.Invoice and SC.MobileNo='{0}')  union all select Name='Applied',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp where  Olpu.IsDeleted=0 and Olpp.Invoice=Olpu.InvoiceID and Olpu.MobileDeviceID='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileDeviceID='{0}'  and EndDate >= GETDATE() order by ID desc) union all select Name='Used',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp,OnlineSlotTimings OLST where  Olpu.IsDeleted=0 and  OLST.SlotCode=Olpu.SlotID and Olpp.Invoice=Olpu.InvoiceID and olpu.SessionDate <= GETDATE() and Olpu.MobileDeviceID='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileDeviceID='{0}'  and EndDate >= GETDATE() order by ID desc)", MobileDeviceId));
                //        }
                //    }
                //    catch (Exception ec)
                //    {
                //        dt_SAUcount = getdata(string.Format("select Top 1 Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLP.PackageName,OLPP.BranchCode,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP,SessionsCount SC where OLPP.PackageID=OLP.PackageID and  SC.Invoice=OLPP.Invoice and SC.MobileDeviceID='{0}' union all SELECT Name='NumberOfSession',0,0,'0','0','01/01/1900','01/01/1900',0,0  WHERE NOT EXISTS ( select Top 1 Name='NumberOfSession',OLPP.NumberOfSession as count,OLPP.PackageID,OLP.PackageName,OLPP.BranchCode,OLPP.StartDate,OLPP.EndDate,OLPP.NumberOfSession,OLPP.NumberOfDaysValidity from  OnlinePackagePurchase OLPP,OnlinePackages OLP,SessionsCount SC where OLPP.PackageID=OLP.PackageID and  SC.Invoice=OLPP.Invoice and SC.MobileNo='{0}')  union all select Name ='Applied',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp where  Olpu.IsDeleted=0 and Olpp.Invoice=Olpu.InvoiceID and Olpu.MobileDeviceID='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileDeviceID='{0}'  and EndDate >= GETDATE() order by ID desc) union all select Name='Used',COUNT(*) as count,PackageID=0,PackageName='',BranchCode='',StartDate='',EndDate='',NumberOfSession=0,NumberOfDaysValidity=0 from OnlinePackageUsed Olpu,OnlinePackagePurchase Olpp,OnlineSlotTimings OLST where  Olpu.IsDeleted=0 and  OLST.SlotCode=Olpu.SlotID and Olpp.Invoice=Olpu.InvoiceID and olpu.SessionDate <= GETDATE() and Olpu.MobileDeviceID='{0}'  and Olpu.InvoiceID=(select Top 1 Invoice from OnlinePackagePurchase where  MobileDeviceID='{0}'  and EndDate >= GETDATE() order by ID desc)", MobileDeviceId));
                //    }
                //}
                int EnquireTypeNo = 0;
                try
                {
                    EnquireTypeNo = Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString());
                    S = Convert.ToInt32(dt_SAUcount.Rows[0][1].ToString());
                    A = Convert.ToInt32(dt_SAUcount.Rows[1][1].ToString());
                    U = Convert.ToInt32(dt_SAUcount.Rows[2][1].ToString());
                }
                catch (Exception ec)
                {
                    S = 0;
                    A = 0;
                    U = 0;
                    EnquireTypeNo = 0;
                }

                if (EnquireTypeNo == 0)
                {
                    if (S == A && S == U && A == U)
                    {
                        List<GetDashboard> dbPackage = new List<GetDashboard>();
                        GetDashboard dashboardlist = new GetDashboard { info = InfoDetails(0, 0, "Active"), middleSection = GetMiddleSectionImages(Dashboard.AppId, 2, Convert.ToString(dt.Rows[1]["DisplayName"])), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                        dbPackage.Add(dashboardlist);
                        foutC.status = "success";
                        foutC.value = dbPackage;
                        sJSONResponse = JsonConvert.SerializeObject(foutC);
                    }
                    else if (S < A && S == U && A > U)
                    {
                        List<GetSlotDetails> dbPackage = new List<GetSlotDetails>();
                        GetSlotDetails dashboardlistpackage = new GetSlotDetails { info = InfoDetails(2, 0, "Active"), middleSection = GetMiddleSectionSlots(Dashboard.MobileNo, dt_SAUcount.Rows[0][3].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][2].ToString())), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                        dbPackage.Add(dashboardlistpackage);

                        foutB.status = "success";
                        foutB.value = dbPackage;
                        sJSONResponse = JsonConvert.SerializeObject(foutB);
                    }
                    else
                    {
                        List<GetDashboardTrainerDetails> dbPackage = new List<GetDashboardTrainerDetails>();
                        GetDashboardTrainerDetails dashboardlistpackage = new GetDashboardTrainerDetails { info = InfoDetails(3, 3, "Expired"), middleSection = GetMiddleSectionExpirePackageswithTrainerDetails(MobileNo, dt_SAUcount.Rows[0][3].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][7].ToString()), Convert.ToInt32(dt_SAUcount.Rows[0][8].ToString()), U), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                        dbPackage.Add(dashboardlistpackage);

                        foutD.status = "success";
                        foutD.value = dbPackage;
                        sJSONResponse = JsonConvert.SerializeObject(foutD);
                    }
                }
                else if (Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 2)
                {
                    if (dt.Rows[0][0].ToString() == "1")
                    {
                        if (S > A && S > U && A == U)
                        {
                            List<GetDashboard1> dbPackage = new List<GetDashboard1>();
                            GetDashboard1 dashboardlistpackage = new GetDashboard1 { info = InfoDetails(1, 2, "Active"), middleSection = GetMiddleSectionPackages(dt_SAUcount.Rows[0][4].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][2].ToString()), dt_SAUcount.Rows[0][3].ToString(), dt_SAUcount.Rows[0][5].ToString(), dt_SAUcount.Rows[0][6].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][7].ToString()), Convert.ToInt32(dt_SAUcount.Rows[0][8].ToString()), U, MobileNo), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            fout.status = "success";
                            fout.value = dbPackage;

                            sJSONResponse = JsonConvert.SerializeObject(fout);
                        }
                        else if (S > A && S > U && A > U)
                        {
                            List<GetSlotDetails> dbPackage = new List<GetSlotDetails>();
                            GetSlotDetails dashboardlistpackage = new GetSlotDetails { info = InfoDetails(2, 2, "Active"), middleSection = GetMiddleSectionSlots(Dashboard.MobileNo, dt_SAUcount.Rows[0][3].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][2].ToString())), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            foutB.status = "success";
                            foutB.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(foutB);
                        }

                        else if (S == A && S > U && A > U)
                        {
                            List<GetSlotDetails> dbPackage = new List<GetSlotDetails>();
                            GetSlotDetails dashboardlistpackage = new GetSlotDetails { info = InfoDetails(2, 2, "Active"), middleSection = GetMiddleSectionSlots(Dashboard.MobileNo, dt_SAUcount.Rows[0][3].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][2].ToString())), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            foutB.status = "success";
                            foutB.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(foutB);
                        }
                        else if (S < A && S == U && A > U)
                        {
                            List<GetSlotDetails> dbPackage = new List<GetSlotDetails>();
                            GetSlotDetails dashboardlistpackage = new GetSlotDetails { info = InfoDetails(2, 0, "Active"), middleSection = GetMiddleSectionSlots(Dashboard.MobileNo, dt_SAUcount.Rows[0][3].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][2].ToString())), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            foutB.status = "success";
                            foutB.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(foutB);
                        }
                        else if (S == A && S == U && A == U)
                        {
                            List<GetDashboardTrainerDetails> dbPackage = new List<GetDashboardTrainerDetails>();
                            GetDashboardTrainerDetails dashboardlistpackage = new GetDashboardTrainerDetails { info = InfoDetails(3, 3, "Expired"), middleSection = GetMiddleSectionExpirePackageswithTrainerDetails(MobileNo, dt_SAUcount.Rows[0][3].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][7].ToString()), Convert.ToInt32(dt_SAUcount.Rows[0][8].ToString()), U), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);



                            foutD.status = "success";
                            foutD.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(foutD);
                        }
                        else
                        {
                            // ifns==0 handel it
                        }
                    }
                    else
                    {

                        List<GetDashboard> dbPackage = new List<GetDashboard>();
                        GetDashboard dashboardlistpackage = new GetDashboard { info = InfoDetails(3, 2, "Active"), middleSection = GetMiddleSectionImages(Dashboard.AppId, 3, Convert.ToString(dt.Rows[1]["DisplayName"])), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                        dbPackage.Add(dashboardlistpackage);

                        foutC.status = "success";
                        foutC.value = dbPackage;
                        sJSONResponse = JsonConvert.SerializeObject(foutC);
                    }

                }
                else if (Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 3)
                {
                    if (dt.Rows[0][0].ToString() == "1")
                    {
                        if (S > A && S > U && A == U)
                        {
                            List<GetDashboard1> dbPackage = new List<GetDashboard1>();
                            GetDashboard1 dashboardlistpackage = new GetDashboard1 { info = InfoDetails(1, 3, "Active"), middleSection = GetMiddleSectionPackages(dt_SAUcount.Rows[0][4].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][2].ToString()), dt_SAUcount.Rows[0][3].ToString(), dt_SAUcount.Rows[0][5].ToString(), dt_SAUcount.Rows[0][6].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][7].ToString()), Convert.ToInt32(dt_SAUcount.Rows[0][8].ToString()), U, MobileNo), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            fout.status = "success";
                            fout.value = dbPackage;

                            sJSONResponse = JsonConvert.SerializeObject(fout);
                        }
                        else if (S > A && S > U && A > U)
                        {
                            List<GetSlotDetails> dbPackage = new List<GetSlotDetails>();
                            GetSlotDetails dashboardlistpackage = new GetSlotDetails { info = InfoDetails(2, 3, "Active"), middleSection = GetMiddleSectionSlots(Dashboard.MobileNo, dt_SAUcount.Rows[0][3].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][2].ToString())), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            foutB.status = "success";
                            foutB.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(foutB);
                        }

                        else if (S == A && S > U && A > U)
                        {
                            List<GetSlotDetails> dbPackage = new List<GetSlotDetails>();
                            GetSlotDetails dashboardlistpackage = new GetSlotDetails { info = InfoDetails(2, 3, "Active"), middleSection = GetMiddleSectionSlots(Dashboard.MobileNo, dt_SAUcount.Rows[0][3].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][2].ToString())), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            foutB.status = "success";
                            foutB.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(foutB);
                        }
                        else if (S == A && S == U && A == U)
                        {
                            List<GetDashboard> dbPackage = new List<GetDashboard>();
                            GetDashboard dashboardlistpackage = new GetDashboard { info = InfoDetails(3, 3, "Expired"), middleSection = GetMiddleSectionExpirePackages(MobileNo, dt_Packagename.Rows[0][0].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][7].ToString()), Convert.ToInt32(dt_SAUcount.Rows[0][8].ToString()), U), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                            dbPackage.Add(dashboardlistpackage);

                            foutC.status = "success";
                            foutC.value = dbPackage;
                            sJSONResponse = JsonConvert.SerializeObject(foutC);
                        }
                        else
                        {
                            // ifns==0 handel it
                        }
                    }
                    else if (S == A && S > U && A > U)
                    {

                        List<GetDashboardTrainerDetails> dbPackage = new List<GetDashboardTrainerDetails>();
                        GetDashboardTrainerDetails dashboardlistpackage = new GetDashboardTrainerDetails { info = InfoDetails(3, 3, "Expired"), middleSection = GetMiddleSectionExpirePackageswithTrainerDetails(MobileNo, dt_Packagename.Rows[0][0].ToString(), Convert.ToInt32(dt_SAUcount.Rows[0][7].ToString()), Convert.ToInt32(dt_SAUcount.Rows[0][8].ToString()), U), topSection = GetBanner(Dashboard.AppId, Convert.ToString(dt.Rows[0]["DisplayName"])), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                        dbPackage.Add(dashboardlistpackage);

                        foutD.status = "success";
                        foutD.value = dbPackage;
                        sJSONResponse = JsonConvert.SerializeObject(foutD);
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
        public List<MiddleSectionA> GetMiddleSectionPackages(string Branchcode, int PackageId, string PackageName, string StartDate, string Enddate, int NoOfSession, int NoOfValidity, int NoofSlotsUsed, string MobileNO)
        {
            List<MiddleSectionA> midsection = new List<MiddleSectionA>();
            midsection.Add(new MiddleSectionA { packages = GetPackagesA(Branchcode, PackageId, PackageName, StartDate, Enddate, NoOfSession, NoOfValidity, NoofSlotsUsed, MobileNO) });
            return midsection;
        }
        public List<MiddleSectionA> GetMiddleSectionExpirePackages(string MobileNo, string PackageName, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        {
            List<MiddleSectionA> midsection = new List<MiddleSectionA>();
            midsection.Add(new MiddleSectionA { packages = GetExpirePackagesA(MobileNo, PackageName, NoOfSession, NoOfValidity, NoofSlotsUsed) });
            return midsection;
        }
        public List<MiddleSectionATrainerDetails> GetMiddleSectionExpirePackageswithTrainerDetails(string MobileNo, string PackageName, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        {
            List<MiddleSectionATrainerDetails> midsection = new List<MiddleSectionATrainerDetails>();
            midsection.Add(new MiddleSectionATrainerDetails { packages = GetExpirePackagesA(MobileNo, PackageName, NoOfSession, NoOfValidity, NoofSlotsUsed), trainerDetails = TrainerDetailsInternal(MobileNo)});
            return midsection;
        }

        public Dictionary<string, object> TrainerDetailsInternal(string MobileNo)
        {
            Dictionary<string, object> TrainerObj = new Dictionary<string, object>();
         

            DataTable dt = new DataTable();
            //Select top 1 OPU.TrainerID,HRE.EmployeeName,HRE.BranchCode,HRE.PhotoURL from OnlinePackagePurchase OPP,OnlinePackages OP,OnlineSlotTimings OLST,OnlinePackageUsed OPU,HREmployee HRE where  OPP.PackageID=OPU.PackageID and  OPP.PackageID=OP.PackageID and OLST.SlotCode=OPU.SlotID and HRE.EmployeeCode=OPP.MembershipCode and HRE.EmergencyContactNumber='9676600301' and OPP.EndDate < GETDATE() order by EndDate desc
            dt = getdata(string.Format("Select top 1 OPU.TrainerID,U.UserName,U.BranchCode,U.PhotoUrl from OnlinePackagePurchase OPP,OnlinePackages OP,OnlineSlotTimings OLST,OnlinePackageUsed OPU,Users U where  OPP.PackageID=OPU.PackageID and  OPP.PackageID=OP.PackageID and OLST.SlotCode=OPU.SlotID and U.UCode=OPP.MembershipCode and U.MobileNo='{0}' and OPP.EndDate < GETDATE() order by EndDate desc", MobileNo));
            

            if (dt.Rows.Count > 0)
            {
                TrainerObj.Add("trainerId", dt.Rows[0]["TrainerID"].ToString());
                TrainerObj.Add("employeeName", dt.Rows[0]["UserName"].ToString());
                TrainerObj.Add("branchCode", dt.Rows[0]["BranchCode"].ToString());
                TrainerObj.Add("imageUrl", dt.Rows[0]["PhotoUrl"].ToString());
            }
            return TrainerObj;
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
            dt = getdata(string.Format("Select OPP.BranchCode,OPP.PackageID,OP.PackageName,OPP.StartDate,OPP.EndDate,OLST.SlotName from OnlinePackagePurchase OPP,OnlinePackages OP,OnlineSlotTimings OLST,OnlinePackageUsed OPU,Users U where  OPP.PackageID=OPU.PackageID and  OPP.PackageID=OP.PackageID and OLST.SlotCode=OPU.SlotID and U.UCode=OPP.MembershipCode and U.MobileNo='{0}' ", MobileNo));
            if (dt.Rows.Count == 0)
            {
                dt = getdata(string.Format("Select distinct OPP.BranchCode,OPP.PackageID,OP.PackageName,OPP.StartDate,OPP.EndDate,SlotName='' from OnlinePackagePurchase OPP,OnlinePackages OP where    OPP.PackageID=OP.PackageID and OPP.MobileNo='{0}' ", MobileNo));

                
            }
            DateTime startDate = Convert.ToDateTime(dt.Rows[0]["StartDate"]);
            DateTime endDate = Convert.ToDateTime(dt.Rows[0]["EndDate"]);

            TimeSpan NoOfDays = endDate.Subtract(startDate);
            int days = int.Parse(NoOfDays.Days.ToString());


            if (dt.Rows.Count > 0)
            {
                ftd.Add(new PackageDetails1
                {
                    branchCode = Convert.ToString(dt.Rows[0]["BranchCode"])
                             ,
                    packageID = Convert.ToString(dt.Rows[0]["PackageID"])
                             ,
                    packageName = Convert.ToString(dt.Rows[0]["PackageName"])
                             ,
                    slotStartDate = startDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
                             ,
                    slotEndDate = endDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
                             ,
                    noOfSession = Convert.ToString(days)
                             ,
                    noOfValidity = Convert.ToString(days)
                    ,
                    noOfSlotsUsed = Convert.ToString(days)

                });
            }
            return ftd;
        }
        public List<MiddleSectionA> GetMiddleSectionSlots(string MobileNo, string PackageName, int PackageId)
        {
            List<MiddleSectionA> midsection = new List<MiddleSectionA>();
            midsection.Add(new MiddleSectionA { slots = GetSlotsA(MobileNo, PackageName, PackageId) });
            return midsection;
        }
        public List<MiddleSectionA> GetMiddleSectionImages(int AppId, int DisplayID, string ImageName)
        {
            List<MiddleSectionA> midsection = new List<MiddleSectionA>();
            midsection.Add(new MiddleSectionA { images = GetImageA(1, DisplayID, ImageName) });
            return midsection;
        }
        public List<Package> GetPackagesA(string Branchcode, int PackageId, string PackageName, string StartDate, string Enddate, int NoOfSession, int NoOfValidity, int NoofSlotsUsed, string MobileNo)
        {
            List<Package> packageA = new List<Package>();
            Boolean PreviousTrainer = false;
            // previouus trainer 0,1
            //condition origanal mobile no  .. online package  ..

            //select count(*) from OnlinePackageUsed where MobileNo='7569067335'


            DataTable dt_SAUcount = getdata(string.Format("select count(*) from OnlinePackageUsed where MobileNo = '{0}'", MobileNo));
            if (dt_SAUcount.Rows.Count > 0)
            {
                PreviousTrainer = true;
            }


            packageA.Add(new Package { packageName = PackageName, previousTrainer = PreviousTrainer, packages = PackageDetails1(Branchcode, PackageId, PackageName, StartDate, Enddate, NoOfSession, NoOfValidity, NoofSlotsUsed) });
            return packageA;

        }

        public List<Package> GetExpirePackagesA(string MobileNo, string PackageName, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        {
           

            List<Package> packageA = new List<Package>();
            packageA.Add(new Package { packageName = PackageName, packages = ExpirePackageDetails1(MobileNo, PackageName, NoOfSession, NoOfValidity, NoofSlotsUsed) });
            return packageA;

        }
        public List<Slot> GetSlotsA(string MobileNo, string PackageName, int PackageId)
        {
            List<Slot> packageB = new List<Slot>();
            List<Slots> SlotsB = new List<Slots>();

            SlotsB = SlotDetails(MobileNo, PackageName, PackageId);

            packageB.Add(new Slot { slotName = SlotsB[0].slotName, slots = SlotDetails(MobileNo, PackageName, PackageId) });
            return packageB;
        }
        public List<Image> GetImageA(int Appid, int DisplalID, string ImageName)
        {
            List<Image> ImageA = new List<Image>();
            ImageA.Add(new Image { imageName = ImageName, images = GetImages(1, DisplalID) });
            return ImageA;
        }
        public List<PackageDetails1> PackageDetails1(string Branchcode, int PackageId, string PackageName, string StartDate, string Enddate, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        {
            List<PackageDetails1> ftd = new List<PackageDetails1>();


            ftd.Add(new PackageDetails1
            {
                branchCode = Convert.ToString(Branchcode)
                         ,
                packageID = Convert.ToString(PackageId)
                         ,
                packageName = Convert.ToString(PackageName)
                         ,
                slotStartDate = Convert.ToString(StartDate)
                         ,
                slotEndDate = Convert.ToString(Enddate)
                         ,
                noOfSession = Convert.ToString(NoOfSession)
                         ,
                noOfValidity = Convert.ToString(NoOfValidity)
                ,
                noOfSlotsUsed = Convert.ToString(NoofSlotsUsed)

            });

            return ftd;
        }
        //public List<PackageDetails1> ExpirePackageDetails1(string MobileNo, string PackageName, int NoOfSession, int NoOfValidity, int NoofSlotsUsed)
        //{
        //    int number = NoofSlotsUsed;
        //    if (number < 0)
        //    {
        //        NoofSlotsUsed = 0;
        //    }
        //    List<PackageDetails1> ftd = new List<PackageDetails1>();

        //    DataTable dt = new DataTable();
        //    dt = getdata(string.Format("Select OPP.BranchCode,OPP.PackageID,OP.PackageName,OPP.StartDate,OPP.EndDate,OLST.SlotName from OnlinePackagePurchase OPP,OnlinePackages OP,OnlineSlotTimings OLST,OnlinePackageUsed OPU,Users U where  OPP.PackageID=OPU.PackageID and  OPP.PackageID=OP.PackageID and OLST.SlotCode=OPU.SlotID and U.UCode=OPP.MembershipCode and U.MobileNo='{0}' ", MobileNo));

        //    DateTime startDate = Convert.ToDateTime(dt.Rows[0]["StartDate"]);
        //    DateTime endDate = Convert.ToDateTime(dt.Rows[0]["EndDate"]);

        //    TimeSpan NoOfDays = endDate.Subtract(startDate);
        //    int days = int.Parse(NoOfDays.Days.ToString());


        //    if (dt.Rows.Count > 0)
        //    {
        //        ftd.Add(new PackageDetails1
        //        {
        //            branchCode = Convert.ToString(dt.Rows[0]["BranchCode"])
        //                     ,
        //            packageID = Convert.ToString(dt.Rows[0]["PackageID"])
        //                     ,
        //            packageName = Convert.ToString(dt.Rows[0]["PackageID"])
        //                     ,
        //            slotStartDate = startDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
        //                     ,
        //            slotEndDate = endDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)
        //                     ,
        //            noOfSession = Convert.ToString(days)
        //                     ,
        //            noOfValidity = Convert.ToString(days)
        //            ,
        //            noOfSlotsUsed = Convert.ToString(days)

        //        });
        //    }
        //    return ftd;
        //}
        public List<Slots> SlotDetails(string MobileNo, string PackageName, int PackageId)
        {
            List<Slots> ftd = new List<Slots>();


            DataTable dt = new DataTable();
            dt = getdata(string.Format("select OP.PackageID,OP.PackageName,OLST.SessionCode AS SessionID,OLPU.SessionDate,OLPU.SlotID,OLST.SlotName,convert(varchar(10), OLST.SlotStartTime, 108) as SlotStartTime,convert(varchar(10), OLST.SlotEndTime, 108) as SlotEndTime from OnlinePackagePurchase OLPP,OnlinePackageUsed OLPU,OnlinePackages OP,OnlineSlotTimings OLST where OLST.SlotCode=OLPU.SlotID and OLPP.Invoice=OLPU.InvoiceID and OLPP.PackageID=OP.PackageID and OLPU.MobileNo='{0}' order by OLPU.ID desc ", MobileNo));


            ftd.Add(new Slots
            {
                packageID = PackageId
                 ,
                packageName = Convert.ToString(PackageName)
                 ,
                sessionID = Convert.ToString(dt.Rows[0]["SessionID"])
                 ,
                sessionDate = Convert.ToString(Convert.ToDateTime(dt.Rows[0]["SessionDate"]).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
                ,
                slotID = Convert.ToInt32(dt.Rows[0]["SlotID"])
                 ,
                slotName = Convert.ToString(dt.Rows[0]["SlotName"])
                 ,
                slotStartDate = Convert.ToString(Convert.ToDateTime(dt.Rows[0]["SlotStartTime"]).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
                 ,
                slotEndDate = Convert.ToString(Convert.ToDateTime(dt.Rows[0]["SlotEndTime"]).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
            });

            return ftd;

        }
        public List<Info> InfoDetails(int isPackages, int enquireTypeNo, string status)
        {
            List<Info> ftd = new List<Info>();

            ftd.Add(new Info
            {
                isPackages = isPackages
                     ,
                enquireTypeNo = enquireTypeNo
                     ,
                status = status


            });

            return ftd;

        }
        public List<ImageDetails> GetImages(int AppID, int Display)
        {
            List<ImageDetails> Imagenamess = new List<ImageDetails>();

            DataTable dt = getdata(string.Format("select ID as ImageID,ImageURL from DashboardImages where AppId='{0}' and DisplayId='{1}'  and IsDeleted=0 and ScreenId=1 ", AppID, Display));
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