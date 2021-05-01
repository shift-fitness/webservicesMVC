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
using NarenFitnessUsers.Models.PackagesList;
using NarenFitnessUsers.Models.OffLinePackageList;
using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;
using System.Web.Helpers;
using System.Collections;


namespace NarenFitnessUsers.Controllers
{
    public class PackagesListOldController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
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

        public Object GetLivePTPackages([FromBody]PackagePriceList PackagePrices)
        {
            string MobileNo = "";
            string MobileDeviceId = "";
            DataTable dt_IsPackage = new DataTable();
            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();
            int EnquiretypeNo = 0;
            PackagePriceFinalOutput ppopt = new PackagePriceFinalOutput();
            try
            {

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


                DataTable dt = getdata(string.Format("select ID,DisplayName from DashboardDisplay", ""));


                if (MobileNo != "")
                {
                    dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM OnlinePackagePurchase AS T1  WHERE T1.MobileNo='{0}')  SELECT (SELECT top 1 EnquireTypeNo FROM OnlinePackagePurchase AS T1  WHERE T1.MobileNo='{0}' order by ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", PackagePrices.mobileNo));
                }
                else
                {
                    dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM OnlinePackagePurchase AS T1  WHERE T1.MobileDeviceID='{0}')  SELECT (SELECT top 1 EnquireTypeNo FROM OnlinePackagePurchase AS T1  WHERE T1.MobileDeviceID='{0}' order by ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", PackagePrices.mobileDeviceId));
                }


                List<PackagePriceList> dbPackage = new List<PackagePriceList>();

                if (Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 0)
                {
                    EnquiretypeNo = 2;
                }
                else if (Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 2 || Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 3)
                {
                    EnquiretypeNo = 3;

                }
                else
                {
                    EnquiretypeNo = 0;
                }

                PackagePriceList dashboardlistpackage = new PackagePriceList { enquiretypeNo = EnquiretypeNo, packagePricesList = GetPackagePriceDetails(EnquiretypeNo), bottomSection = GetHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                dbPackage.Add(dashboardlistpackage);

                ppopt.status = "success";
                ppopt.value = dbPackage;
                sJSONResponse = JsonConvert.SerializeObject(ppopt);


                //dt_Packageresposne.Tables.Clear();
                //dt_Packageresposne.Tables.Add(ReturnException("success", sJSONResponse));
            }
            catch (Exception ec)
            {
                ppopt.status = "success";

                sJSONResponse = JsonConvert.SerializeObject(ppopt);

            }


            // string val = json.DataTableToJSONWithStringBuilder(dt_Packageresposne.Tables[0]);

            return sJSONResponse;

        }
        public List<PackagePrices> GetPackagePriceDetails(int EnquireTypeNo)
        {
            DataTable dt = new DataTable();
            List<PackagePrices> PackagePrice = new List<PackagePrices>();
            if (EnquireTypeNo == 2)
            {
                dt = getdata(string.Format("select BranchCode,PackageID,PackageName,PackageCost,DiscountPercentage,NumberOfSession,NumberOfDaysValidity from OnlinePackages"));
            }
            else
            {
                dt = getdata(string.Format("select BranchCode,PackageID,PackageName,PackageCost,DiscountPercentage,NumberOfSession,NumberOfDaysValidity from OnlinePackages  where PackageID<>1"));
            }




            for (int i = 0; i < dt.Rows.Count; i++)
            {

                PackagePrice.Add(new PackagePrices
                {
                    branchCode = Convert.ToString(dt.Rows[i]["branchCode"])
                    ,
                    packageId = Convert.ToString(dt.Rows[i]["packageID"])
                     ,
                    packageName = Convert.ToString(dt.Rows[i]["packageName"])
                     ,
                    packageCost = Convert.ToString(dt.Rows[i]["packageCost"])
                     ,
                    discountPercentage = Convert.ToString(dt.Rows[i]["discountPercentage"])
                     ,
                    numberOfSession = Convert.ToString(dt.Rows[i]["numberOfSession"])
                     ,
                    numberOfDaysValidity = Convert.ToString(dt.Rows[i]["numberOfDaysValidity"])

                });

            }
            return PackagePrice;
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
        public Object GetOffLineLivePTPackages([FromBody]OffLinePackagePrices PackagePrices)
        {
            string MobileNo = "";
            string MobileDeviceId = "";
            DataTable dt_IsPackage = new DataTable();
            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();
            int EnquiretypeNo = 0;
            OffLinePackagePriceFinalOutput ppopt = new OffLinePackagePriceFinalOutput();
            try
            {

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


                DataTable dt = getdata(string.Format("select ID,DisplayName from DashboardDisplay", ""));


                if (MobileNo != "")
                {
                    dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM OnlinePackagePurchase AS T1  WHERE T1.MobileNo='{0}')  SELECT (SELECT top 1 EnquireTypeNo FROM OnlinePackagePurchase AS T1  WHERE T1.MobileNo='{0}' order by ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", PackagePrices.mobileNo));
                }
                else
                {
                    dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM OnlinePackagePurchase AS T1  WHERE T1.MobileDeviceID='{0}')  SELECT (SELECT top 1 EnquireTypeNo FROM OnlinePackagePurchase AS T1  WHERE T1.MobileDeviceID='{0}' order by ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", PackagePrices.mobileDeviceId));
                }


                List<OffLinePackagePriceList> dbPackage = new List<OffLinePackagePriceList>();

                if (Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 0)
                {
                    EnquiretypeNo = 2;
                }
                else if (Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 2 || Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 3)
                {
                    EnquiretypeNo = 3;

                }
                else
                {
                    EnquiretypeNo = 0;
                }

                OffLinePackagePriceList dashboardlistpackage = new OffLinePackagePriceList { enquiretypeNo = EnquiretypeNo, packagePricesList = GetOffLinePackagePriceDetails(EnquiretypeNo), bottomSection = GetOffLineHeaders(Convert.ToString(dt.Rows[3]["DisplayName"])) };
                dbPackage.Add(dashboardlistpackage);

                ppopt.status = "success";
                ppopt.value = dbPackage;
                sJSONResponse = JsonConvert.SerializeObject(ppopt);


                //dt_Packageresposne.Tables.Clear();
                //dt_Packageresposne.Tables.Add(ReturnException("success", sJSONResponse));
            }
            catch (Exception ec)
            {
                ppopt.status = "success";

                sJSONResponse = JsonConvert.SerializeObject(ppopt);

            }


            // string val = json.DataTableToJSONWithStringBuilder(dt_Packageresposne.Tables[0]);

            return sJSONResponse;

        }
        public List<OffLinePackagePrices> GetOffLinePackagePriceDetails(int EnquireTypeNo)
        {
            DataTable dt = new DataTable();
            List<OffLinePackagePrices> PackagePrice = new List<OffLinePackagePrices>();
            if (EnquireTypeNo == 2)
            {
                dt = getdata(string.Format("select BranchCode,PackageID,PackageName,PackageCost,DiscountPercentage,NumberOfSession,NumberOfDaysValidity from OnlinePackages"));
            }
            else
            {
                dt = getdata(string.Format("select BranchCode,PackageID,PackageName,PackageCost,DiscountPercentage,NumberOfSession,NumberOfDaysValidity from OnlinePackages  where PackageID<>1"));
            }




            for (int i = 0; i < dt.Rows.Count; i++)
            {

                PackagePrice.Add(new OffLinePackagePrices
                {
                    branchCode = Convert.ToString(dt.Rows[i]["branchCode"])
                    ,
                    packageId = Convert.ToString(dt.Rows[i]["packageID"])
                     ,
                    packageName = Convert.ToString(dt.Rows[i]["packageName"])
                     ,
                    packageCost = Convert.ToString(dt.Rows[i]["packageCost"])
                     ,
                    discountPercentage = Convert.ToString(dt.Rows[i]["discountPercentage"])
                     ,
                    numberOfSession = Convert.ToString(dt.Rows[i]["numberOfSession"])
                     ,
                    numberOfDaysValidity = Convert.ToString(dt.Rows[i]["numberOfDaysValidity"])

                });

            }
            return PackagePrice;
        }
        public List<OffLineTextHeaderAll> GetOffLineHeaders(string HeaderName)
        {
            List<OffLineTextHeaderAll> allTextHeader = new List<OffLineTextHeaderAll>();
            allTextHeader.Add(new OffLineTextHeaderAll { text = HeaderName, headers = GetOffLineTextHeaders() });
            return allTextHeader;
        }
        public List<OffLineTextHeader> GetOffLineTextHeaders()
        {
            List<OffLineTextHeader> textnames = new List<OffLineTextHeader>();

            DataTable dt = getdata(string.Format("select ID as TextID,Text as TextName from DashboardText where DisplayID=4", ""));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                textnames.Add(new OffLineTextHeader
                {
                    textID = Convert.ToString(dt.Rows[i]["TextID"])
                    ,
                    textName = Convert.ToString(dt.Rows[i]["TextName"])


                });

            }
            return textnames;
        }
        public ArrayList MobileCheck(string MobileNo)
        {
            DataSet ds_count = new DataSet();
            ArrayList arl_Mcheck = new ArrayList();
            try
            {
                string query1 = "select count(*) as count from CCRMMEnquireForm where MobileNo ='" + MobileNo + "' union all select count(*) as count from Users where MobileNo ='" + MobileNo + "'";
                SqlDataAdapter da = new SqlDataAdapter(query1, cnn);
                da.Fill(ds_count);

                arl_Mcheck.Add(ds_count.Tables[0].Rows[0][0].ToString());
                arl_Mcheck.Add(ds_count.Tables[0].Rows[1][0].ToString());


            }
            catch (Exception ec)
            {

            }

            return arl_Mcheck;
        }

        // Dynamic Pricing
        //28/08/2020: Need to add request Memebershipcode & MobileNo and check the enquirytype and update isfreetrail (0,1)

        public string GetOfflineDynamicPricing([FromBody]OffLineCMSSlotsAvailability PackagePrices)
        {
            string sJSONResponse = "";

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_slots = new DataSet();
            string result = string.Empty;
            //
            int digit1 = 0;
            int MaxNum = 100;
            int AddNum = 0;
            int MinusNum = 0;

            int AvailableSlots = 0;
            int FilledSlots = 0;
            float PlanCost = 0.0f;
            float SlotPrice = 0.0f;
            float MinimumAmount = 0.0f;
            float MaximumAmount = 0.0f;
            float Constant = 0.0f;

            DataTable dt_IsPackage = new DataTable();
            string MobileNo = PackagePrices.MobileNo;
            string MembershipCode = PackagePrices.MembershipCode;

            int EnquiretypeNo = 0;


            DataTable dt_Slotavailable = new DataTable();
            DataTable dt_Slotused = new DataTable();
            DataTable dt_Slot = new DataTable();

            string BranchCode = PackagePrices.BranchCode;
            string PackageCode = PackagePrices.PackageCode;

            ArrayList arl_Mobile = MobileCheck(PackagePrices.MobileNo);
            int EnquireformCount = Convert.ToInt32(arl_Mobile[0].ToString());
            int UserCount = Convert.ToInt32(arl_Mobile[1].ToString());

            string UniqueId = "";

            try
            {

                if (MobileNo != null && MembershipCode != "")
                {
                    UniqueId = MobileNo;
                }
                else if (MobileNo != "" && MembershipCode == "")
                {
                    UniqueId = MobileNo;
                }
                else
                {
                    UniqueId = "";
                }


                DataTable dt = getdata(string.Format("select ID,DisplayName from DashboardDisplay", ""));


                if (UniqueId != "")
                {

                    dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM CCRMMembership AS T1,Users T2   WHERE  T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode)  SELECT (SELECT top 1 T1.EnquireTypeNo FROM CCRMMembership AS T1,Users T2  WHERE T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", PackagePrices.MobileNo));
                    EnquiretypeNo = Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString());
                }
                else
                {
                    // dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM CCRMMembership AS T1,Users T2   WHERE   T1.MembershipCode=T2.UCode)  SELECT (SELECT top 1 T1.EnquireTypeNo FROM CCRMMembership AS T1,Users T2  WHERE T1.MembershipCode=T2.UCode order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", ""));
                    EnquiretypeNo = 0;
                }



                List<OffLinePackagePriceList> dbPackage = new List<OffLinePackagePriceList>();
                try
                {
                    if (EnquiretypeNo == 0)
                    {
                        EnquiretypeNo = 1;
                    }
                    else if (EnquiretypeNo > 0)
                    {
                        EnquiretypeNo = 0;

                    }
                    else
                    {
                        EnquiretypeNo = 1;
                    }
                }
                catch (Exception ec)
                {


                }
            }
            catch (Exception ec)
            {

            }



            dt_Slotused = GetAllSlotsByPackageCodeUsedAUTOPRICE(PackagePrices.BranchCode, PackagePrices.PackageCode, PackagePrices.StartDate);
            if (dt_Slotused.Rows.Count > 0)
            {
                dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICE(PackagePrices.BranchCode, PackagePrices.PackageCode);
                foreach (DataRow rrow in dt_Slotavailable.Rows)
                {
                    foreach (DataRow trow in dt_Slotused.Rows)
                    {
                        if (rrow["SlotCode"].ToString() == trow["SlotCode"].ToString())
                        {
                            //PackageCode
                            if (rrow["PackageCode"].ToString() == trow["PackageCode"].ToString())
                            {
                                string val1 = rrow["SlotCode"].ToString();
                                string val2 = trow["SlotCode"].ToString();

                                if (trow["SNO"].ToString() == "1")
                                {
                                    //  rrow["RegularMembersPerSlot"] = trow["AvailableSlots"].ToString();
                                    // rrow["RegularMembersFilledSlots"] = trow["FilledSlots"].ToString();

                                    //AvailableSlots = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                    AvailableSlots = Convert.ToInt32(rrow["RegularMembersPerSlot"]);
                                    FilledSlots = Convert.ToInt32(trow["FilledSlots"].ToString());
                                    PlanCost = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                    SlotPrice = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                    MinimumAmount = Convert.ToInt32(rrow["MinPlanCost"].ToString());
                                    MaximumAmount = Convert.ToInt32(rrow["MaxPlanCost"].ToString());
                                    Constant = Convert.ToInt32(rrow["SlotPricingConstant"].ToString());
                                    float x = ((Constant / AvailableSlots) / 100);
                                    float y = MinimumAmount * x;
                                    float z = FilledSlots * y;
                                    int A = (int)z;
                                    int B = Convert.ToInt32(MinimumAmount) + A;
                                    int numlength = B.ToString().Length;

                                    if (numlength > 2)
                                    {
                                        //for (int i = 0; i <= numlength - 1; i++)
                                        //{
                                        //    if(i==0)
                                        //    {
                                        //        digit1=Convert.ToInt16(Last2digits(B));
                                        //    }
                                        //}

                                        digit1 = Convert.ToInt16(Last2digits(B));

                                        if (digit1 <= 50)
                                        {
                                            MinusNum = digit1;
                                            B = B - digit1;
                                        }
                                        else if (digit1 > 50)
                                        {
                                            AddNum = MaxNum - digit1;
                                            B = B + AddNum;
                                        }
                                    }
                                    // rrow["SlotPrice"] = MinimumAmount + z;


                                    rrow["SlotPrice"] = B;
                                    float aval = MinimumAmount + z;

                                    if (FilledSlots < AvailableSlots)
                                        rrow["IsRegularSlotAvailable"] = 1;
                                    else if (FilledSlots == AvailableSlots)
                                        rrow["IsRegularSlotAvailable"] = 0;
                                    else
                                        rrow["IsRegularSlotAvailable"] = 0;
                                    //rrow["RegularMembersFilledSlots"] = Convert.ToInt32(rrow["RegularMembersPerSlot"]) - Convert.ToInt32(trow["FilledSlots"].ToString());
                                    rrow["RegularMembersFilledSlots"] = Convert.ToInt32(trow["FilledSlots"].ToString());

                                }
                            }

                            if (rrow["SlotCode"].ToString() == trow["SlotCode"].ToString())
                            {
                                string val1 = rrow["SlotCode"].ToString();
                                string val2 = trow["SlotCode"].ToString();

                                // Trial Slots
                                //if (trow["SNO"].ToString() == "2")
                                //{
                                //    rrow["FreeMembersPerSlot"] = trow["AvailableSlots"].ToString();
                                //    rrow["FreeMembersFilledSlots"] = trow["FilledSlots"].ToString();

                                //    AvailableSlots = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                //    FilledSlots = Convert.ToInt32(trow["FilledSlots"].ToString());
                                //    PlanCost = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                //    SlotPrice = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                //    MinimumAmount = Convert.ToInt32(rrow["MinPlanCost"].ToString());
                                //    MaximumAmount = Convert.ToInt32(rrow["MaxPlanCost"].ToString());
                                //    Constant = Convert.ToInt32(rrow["SlotPricingConstant"].ToString());


                                //    rrow["SlotPrice"] = (MinimumAmount + (FilledSlots) * (MinimumAmount * ((Constant / AvailableSlots) / 100)));

                                //    float aval = (MinimumAmount + (FilledSlots) * (MinimumAmount * ((Constant / AvailableSlots) / 100)));

                                //    if (FilledSlots < AvailableSlots)
                                //        rrow["IsTrialSlotAvailable"] = 1;
                                //    else if (FilledSlots == AvailableSlots)
                                //        rrow["IsTrialSlotAvailable"] = 0;
                                //    else
                                //        rrow["IsTrialSlotAvailable"] = 0;
                                //}
                            }
                        }
                    }
                }
            }
            else
            {
                dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICEDefault(BranchCode, PackageCode);
            }

            // End loop for all days



            int sloutcount = dt_Slotavailable.Rows.Count;
            dt_Slotavailable.DefaultView.Sort = "SlotStartTime ASC";
            DataTable st_newSlotavailable = new DataTable();
            st_newSlotavailable = dt_Slotavailable.DefaultView.ToTable();
            DataTable dt_FilteredSlotsAvailable = new DataTable();

            List<OffLineEnquireInfo> olei = new List<OffLineEnquireInfo>();
            olei.Add(new OffLineEnquireInfo { isFreeTrial = EnquiretypeNo, enquireTypeNo = 2 });

            //DataTable dt_Sessions = getdata(string.Format("select distinct CMSSS.SessionCode,CMSSS.SessionName,convert(varchar(10),CMSSS.SessionStartTime, 108) as SessionStartTime,convert(varchar(10),  CMSSS.SessionEndTime, 108) as SessionEndTime  from  CMSSESSIONTIMESETTING CMSSS  order by SessionCode asc", ""));
            //List<OffLineSessions> OnlineSessions = new List<OffLineSessions>();
            //for (int i = 0; i < dt_Sessions.Rows.Count - 1; i++)
            //{
            //    OffLineSessions SessionsDetails = new OffLineSessions { sessionId = dt_Sessions.Rows[i]["SessionCode"].ToString(), sessionName = dt_Sessions.Rows[i]["SessionName"].ToString(), sessionStartTime = dt_Sessions.Rows[i]["SessionStartTime"].ToString(), sessionEndTime = dt_Sessions.Rows[i]["SessionEndTime"].ToString(), slots = GetSlotsDetails(st_newSlotavailable, dt_Sessions.Rows[i]["SessionCode"].ToString()) };
            //    OnlineSessions.Add(SessionsDetails);
            //}

            List<OffLineOnlySlotsPackageDetails> dt_SlotPackageDopt = new List<OffLineOnlySlotsPackageDetails>();
            OffLineOnlySlotsPackageDetails dashboardlistpackage = new OffLineOnlySlotsPackageDetails { info = olei, slots = GetOnlySlotsDetails(st_newSlotavailable) };
            dt_SlotPackageDopt.Add(dashboardlistpackage);

            OffLineOnlySlotsPackageOutPut olppFOP = new OffLineOnlySlotsPackageOutPut();

            olppFOP.status = "Success";
            olppFOP.value = dt_SlotPackageDopt;
            sJSONResponse = JsonConvert.SerializeObject(olppFOP);


            return sJSONResponse;

        }
        public List<OffLineSlotsPackage> GetOnlySlotsDetails(DataTable dt_SlotPackages)
        {
            List<OffLineSlotsPackage> offLineSlotPackage = new List<OffLineSlotsPackage>();

            try
            {
                for (int i = 0; i < dt_SlotPackages.Rows.Count; i++)
                {

                    offLineSlotPackage.Add(new OffLineSlotsPackage
                    {
                        sNo = Convert.ToInt32(dt_SlotPackages.Rows[i]["SNO"])
                        ,
                        slotCode = Convert.ToString(dt_SlotPackages.Rows[i]["SlotCode"])
                         ,

                        sessionCode = Convert.ToString(dt_SlotPackages.Rows[i]["SessionCode"])

                         ,
                        packageCode = Convert.ToString(dt_SlotPackages.Rows[i]["PackageCode"])

                        ,
                        isSlotAvailable = Convert.ToInt32(dt_SlotPackages.Rows[i]["IsRegularSlotAvailable"])
                         ,
                        planCost = Convert.ToDouble(dt_SlotPackages.Rows[i]["PlanCost"])
                         ,
                        duration = Convert.ToString(dt_SlotPackages.Rows[i]["Duration"])
                         ,
                        noOfDays = Convert.ToInt32(dt_SlotPackages.Rows[i]["NoOfDays"])
                         ,
                        durationCode = Convert.ToString(dt_SlotPackages.Rows[i]["DurationCode"])
                        ,
                        planCode = Convert.ToString(dt_SlotPackages.Rows[i]["PlanCode"])
                         ,
                        planName = Convert.ToString(dt_SlotPackages.Rows[i]["PlanName"])
                         ,
                        trainerCode = Convert.ToString(dt_SlotPackages.Rows[i]["TrainersCode"])
                         ,
                        trainerName = Convert.ToString(dt_SlotPackages.Rows[i]["TrainersName"])
                         ,
                        slotPrice = Convert.ToDouble(dt_SlotPackages.Rows[i]["SlotPrice"])
                        ,
                        slotStartTime = Convert.ToString(dt_SlotPackages.Rows[i]["SlotStartTime"])
                         ,
                        slotEndTime = Convert.ToString(dt_SlotPackages.Rows[i]["SlotEndTime"])


                    });

                }

            }
            catch (Exception ec)
            {

            }



            return offLineSlotPackage;
        }
        public List<OffLineSlotsPackage> GetSlotsDetails(DataTable dt_SlotPackages, string SessionCode)
        {
            List<OffLineSlotsPackage> offLineSlotPackage = new List<OffLineSlotsPackage>();

            try
            {
                DataRow[] results = dt_SlotPackages.Select("SessionCode = '" + SessionCode + "' ");
                dt_SlotPackages = results.CopyToDataTable();


                for (int i = 0; i < dt_SlotPackages.Rows.Count; i++)
                {

                    offLineSlotPackage.Add(new OffLineSlotsPackage
                    {
                        sNo = Convert.ToInt32(dt_SlotPackages.Rows[i]["SNO"])
                        ,
                        slotCode = Convert.ToString(dt_SlotPackages.Rows[i]["SlotCode"])
                        ,
                        sessionCode = Convert.ToString(dt_SlotPackages.Rows[i]["SessionCode"])
                        ,
                        packageCode = Convert.ToString(dt_SlotPackages.Rows[i]["PackageCode"])
                        ,
                        isSlotAvailable = Convert.ToInt32(dt_SlotPackages.Rows[i]["IsRegularSlotAvailable"])
                         ,
                        planCost = Convert.ToDouble(dt_SlotPackages.Rows[i]["PlanCost"])
                         ,
                        duration = Convert.ToString(dt_SlotPackages.Rows[i]["Duration"])
                         ,
                        noOfDays = Convert.ToInt32(dt_SlotPackages.Rows[i]["NoOfDays"])
                         ,
                        durationCode = Convert.ToString(dt_SlotPackages.Rows[i]["DurationCode"])
                        ,
                        planCode = Convert.ToString(dt_SlotPackages.Rows[i]["PlanCode"])
                         ,
                        planName = Convert.ToString(dt_SlotPackages.Rows[i]["PlanName"])
                         ,
                        trainerCode = Convert.ToString(dt_SlotPackages.Rows[i]["TrainersCode"])
                         ,
                        trainerName = Convert.ToString(dt_SlotPackages.Rows[i]["TrainersName"])
                         ,
                        slotPrice = Convert.ToDouble(dt_SlotPackages.Rows[i]["SlotPrice"])
                        ,
                        slotStartTime = Convert.ToString(dt_SlotPackages.Rows[i]["SlotStartTime"])
                         ,
                        slotEndTime = Convert.ToString(dt_SlotPackages.Rows[i]["SlotEndTime"])


                    });

                }

            }
            catch (Exception ec)
            {

            }



            return offLineSlotPackage;
        }
        public DataTable GetAllSlotsByPackageCodeAvailableAUTOPRICEDefault(string BranchCode, string PackageCode)
        {

            //string results = "";
            string result1 = "";
            DataTable dt1 = new DataTable();
            //string status = "";
            int NOD = 0;
            string DurationID = string.Empty;
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet1 = new DataSet();
            DataSet ds_custdet2 = new DataSet();
            DataSet ds_custdet3 = new DataSet();
            DataTable dt_Slotav = new DataTable();
            string result = string.Empty;

            int l = 1;

            DataRow row;
            DataColumn col1 = new DataColumn("SNO", typeof(int));
            DataColumn col2 = new DataColumn("SlotCode", typeof(string));
            DataColumn col3 = new DataColumn("SlotName", typeof(string));
            DataColumn col4 = new DataColumn("SessionCode", typeof(string));
            DataColumn col5 = new DataColumn("SessionName", typeof(string));
            DataColumn col6 = new DataColumn("PackageCode", typeof(string));
            DataColumn col7 = new DataColumn("PackageName", typeof(string));
            DataColumn col8 = new DataColumn("RegularMembersPerSlot", typeof(int));
            DataColumn col9 = new DataColumn("RegularMembersFilledSlots", typeof(int));
            DataColumn col10 = new DataColumn("IsRegularSlotAvailable", typeof(int));
            DataColumn col11 = new DataColumn("FreeMembersPerSlot", typeof(int));
            DataColumn col12 = new DataColumn("FreeMembersFilledSlots", typeof(int));
            DataColumn col13 = new DataColumn("IsTrialSlotAvailable", typeof(int));
            DataColumn col14 = new DataColumn("PlanCost", typeof(double));
            DataColumn col15 = new DataColumn("Duration", typeof(string));
            DataColumn col16 = new DataColumn("NoOfDays", typeof(int));
            DataColumn col17 = new DataColumn("DurationCode", typeof(string));
            DataColumn col18 = new DataColumn("PlanCode", typeof(string));
            DataColumn col19 = new DataColumn("PlanName", typeof(string));
            DataColumn col20 = new DataColumn("TrainersCode", typeof(string));
            DataColumn col21 = new DataColumn("TrainersName", typeof(string));
            DataColumn col22 = new DataColumn("SlotPrice", typeof(string));
            DataColumn col23 = new DataColumn("PlanCostCode", typeof(string));
            DataColumn col24 = new DataColumn("MinPlanCost", typeof(float));
            DataColumn col25 = new DataColumn("MaxPlanCost", typeof(float));
            DataColumn col26 = new DataColumn("SlotPricingConstant", typeof(float));
            DataColumn col27 = new DataColumn("SlotStartTime", typeof(string));
            DataColumn col28 = new DataColumn("SlotEndTime", typeof(string));

            dt_Slotav.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8, col9, col10, col11, col12, col13, col14, col15, col16, col17, col18, col19, col20, col21, col22, col23, col24, col25, col26, col27, col28 });
            string query1 = "";
            if (PackageCode == "P0")
            {
                //changed on 08/11/2018 by dilip point
                //query1 = "select  CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode from CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP,SlotGroups SG,CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS where CMSP.PackageCode=CMSPC.PackageCode and CMSPC.SlotCode=CMSSS.SlotCode and  CMSS.SessionCode = CMSSS.SessionCode and  CMSSS.SLTGID = SG.SLTGID  and CMSSS.BranchCode = '" + BranchCode + "' and CMSP.PackageCode=CMSPC.PackageCode    group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode";
                //query1 = "select  CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice from CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP,SlotGroups SG,CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS where CMSP.PackageCode=CMSPC.PackageCode and CMSPC.SlotCode=CMSSS.SlotCode and  CMSS.SessionCode = CMSSS.SessionCode and  CMSSS.SLTGID = SG.SLTGID  and CMSP.PackageCode=CMSPC.PackageCode and CMSPC.IsActive=1    group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice";
                ////query1 = "select  CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPC.MinSlotPrice,CMSPC.MaxSlotPrice from CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP,SlotGroups SG,CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS where CMSP.BranchCode='" + BranchCode + "' and  CMSP.IsDeleted=0 and CMSP.IsActive=1 and CMSPC.IsActive=1 and CMSPC.IsDeleted=0  and CMSP.PackageCode=CMSPC.PackageCode and CMSPC.SlotCode=CMSSS.SlotCode and  CMSS.SessionCode = CMSSS.SessionCode and  CMSSS.SLTGID = SG.SLTGID  and CMSP.PackageCode=CMSP.PackageCode   group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPC.MinSlotPrice,CMSPC.MaxSlotPrice";
                // latest query1 = "select  CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant from CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP,SlotGroups SG,CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS,CMSPLANCOST CMSPLC where CMSPLC.PackageCode=CMSPC.PackageCode and CMSPLC.PlanCostCode=CMSPC.PlanCostCode and CMSPLC.BranchCode=CMSPC.BranchCode and CMSP.PackageCode=CMSPC.PackageCode and CMSPC.SlotCode=CMSSS.SlotCode and  CMSS.SessionCode = CMSSS.SessionCode and  CMSSS.SLTGID = SG.SLTGID  and CMSP.BranchCode = '1101' and  CMSP.PackageCode=CMSPC.PackageCode and CMSPC.IsActive=1 and CMSP.IsActive=1 and CMSPC.IsDeleted=0 and CMSP.IsDeleted=0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant";
                query1 = "select CMSSS.SlotCode,SG.FreeMembersPerSlot,PG.PackageWiseSlot as RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime  from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP,SlotGroups SG,PackageGroup PG, CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC where  CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode and CMSSS.SLTGID=PG.SLTGID and CMSSS.SLTGID=SG.SLTGID and PG.SLTGID=SG.SLTGID and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode and CMSSS.SLTGID = SG.SLTGID  and CMSP.BranchCode = '" + BranchCode + "'  and PG.PackageCode=CMSPC.PackageCode and PG.PackageCode=CMSP.PackageCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,PG.PackageWiseSlot,CMSSS.SlotStartTime,CMSSS.SlotEndTime ";
            }
            else
            {
                //changed on 08/11/2018 by dilip point
                // query1 = "select  CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode from CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP,SlotGroups SG,CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS where CMSP.PackageCode=CMSPC.PackageCode and CMSPC.SlotCode=CMSSS.SlotCode and  CMSS.SessionCode = CMSSS.SessionCode and  CMSSS.SLTGID = SG.SLTGID  and CMSSS.BranchCode = '" + BranchCode + "' and  CMSP.PackageCode=CMSPC.PackageCode    group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode";
                ////query1 = "select  CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPC.MinSlotPrice,CMSPC.MaxSlotPrice from CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP,SlotGroups SG,CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS where CMSP.PackageCode=CMSPC.PackageCode and CMSPC.SlotCode=CMSSS.SlotCode and  CMSS.SessionCode = CMSSS.SessionCode and  CMSSS.SLTGID = SG.SLTGID  and CMSP.BranchCode = '" + BranchCode + "' and CMSPC.PackageCode = '" + PackageCode + "' and  CMSP.PackageCode=CMSPC.PackageCode and CMSPC.IsActive=1 and CMSP.IsActive=1 and CMSPC.IsDeleted=0 and CMSP.IsDeleted=0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPC.MinSlotPrice,CMSPC.MaxSlotPrice";
                //Latest  query1 = "select CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP,SlotGroups SG, CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC where CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode and CMSSS.SLTGID = SG.SLTGID  and CMSP.BranchCode = '1101' and CMSPC.PackageCode = 'P1' and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant ";
                query1 = "select CMSSS.SlotCode,SG.FreeMembersPerSlot,PG.PackageWiseSlot as RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP,SlotGroups SG,PackageGroup PG, CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC where  CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode and CMSSS.SLTGID=PG.SLTGID and CMSSS.SLTGID=SG.SLTGID and PG.SLTGID=SG.SLTGID and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode and CMSSS.SLTGID = SG.SLTGID  and CMSP.BranchCode = '" + BranchCode + "' and CMSPC.PackageCode = '" + PackageCode + "' and PG.PackageCode=CMSPC.PackageCode and PG.PackageCode=CMSP.PackageCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,PG.PackageWiseSlot,CMSSS.SlotStartTime,CMSSS.SlotEndTime";
            }

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            for (int j = 0; j <= ds_custdet1.Tables[0].Rows.Count - 1; j++)
            {
                DurationID = ds_custdet1.Tables[0].Rows[j][11].ToString();
                char[] strGoal = DurationID.ToCharArray();

                row = dt_Slotav.NewRow();
                if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "W")
                {
                    if (Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString()) == 1)
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " Week";
                    }
                    else
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " Week";
                    }
                    NOD = Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString()) * 7;
                }
                else if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "D")
                {
                    if (Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString()) == 1)
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " Day";
                    }
                    else
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " Days";
                    }
                    NOD = Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString());
                }
                else if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "M")
                {
                    if (Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString()) == 1)
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " Month";
                    }
                    else
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " Months";
                    }
                    NOD = Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString()) * 30;
                }
                else
                {
                    if (Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString()) == 1)
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " ";
                    }
                    else
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " ";
                    }
                    NOD = Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString()) * 30;
                }
                // CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPC.MinSlotPrice,CMSPC.MaxSlotPrice


                row["SNO"] = l;
                row["SlotCode"] = ds_custdet1.Tables[0].Rows[j]["SlotCode"].ToString();
                row["SlotName"] = ds_custdet1.Tables[0].Rows[j]["SlotName"].ToString();
                row["SessionCode"] = ds_custdet1.Tables[0].Rows[j]["SessionCode"].ToString();
                row["SessionName"] = ds_custdet1.Tables[0].Rows[j]["SessionName"].ToString();
                row["PackageCode"] = ds_custdet1.Tables[0].Rows[j]["PackageCode"].ToString();
                row["PackageName"] = ds_custdet1.Tables[0].Rows[j]["PackageName"].ToString();
                row["RegularMembersPerSlot"] = ds_custdet1.Tables[0].Rows[j]["RegularMembersPerSlot"].ToString();
                row["RegularMembersFilledSlots"] = "0";
                row["IsRegularSlotAvailable"] = "1";
                row["FreeMembersPerSlot"] = ds_custdet1.Tables[0].Rows[j]["FreeMembersPerSlot"].ToString();
                row["FreeMembersFilledSlots"] = "0";
                row["IsTrialSlotAvailable"] = "1";
                row["PlanCost"] = Convert.ToDouble(ds_custdet1.Tables[0].Rows[j]["PlanCost"].ToString());
                row["SlotPrice"] = ds_custdet1.Tables[0].Rows[j]["MinPlanCost"].ToString();
                row["MinPlanCost"] = ds_custdet1.Tables[0].Rows[j]["MinPlanCost"].ToString();
                row["MaxPlanCost"] = ds_custdet1.Tables[0].Rows[j]["MaxPlanCost"].ToString();
                row["NoOfDays"] = NOD;
                row["DurationCode"] = ds_custdet1.Tables[0].Rows[j]["DurationId"].ToString();
                row["PlanCode"] = ds_custdet1.Tables[0].Rows[j]["PlanCode"].ToString();
                row["PlanName"] = ds_custdet1.Tables[0].Rows[j]["PlanName"].ToString();
                row["PlanCostCode"] = ds_custdet1.Tables[0].Rows[j]["PlanCostCode"].ToString();
                dt1 = GetTrainersListBySlots(ds_custdet1.Tables[0].Rows[j][0].ToString(), BranchCode, ds_custdet1.Tables[0].Rows[j][9].ToString());
                row["TrainersCode"] = dt1.Rows[0]["TrainerCode"].ToString();
                row["TrainersName"] = dt1.Rows[0]["TrainerName"].ToString();
                row["SlotPricingConstant"] = ds_custdet1.Tables[0].Rows[j]["SlotPricingConstant"].ToString();
                row["SlotStartTime"] = ds_custdet1.Tables[0].Rows[j]["SlotStartTime"].ToString();
                row["SlotEndTime"] = ds_custdet1.Tables[0].Rows[j]["SlotEndTime"].ToString();

                dt_Slotav.Rows.Add(row);
                l = l + 1;
            }

            return dt_Slotav;
        }
        public DataTable GetAllSlotsByPackageCodeAvailableAUTOPRICE(string BranchCode, string PackageCode)
        {

            //string results = "";
            string result1 = "";
            DataTable dt1 = new DataTable();
            //string status = "";
            int NOD = 0;
            string DurationID = string.Empty;
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet1 = new DataSet();
            DataSet ds_custdet2 = new DataSet();
            DataSet ds_custdet3 = new DataSet();
            DataTable dt_Slotav = new DataTable();
            string result = string.Empty;

            int l = 1;

            DataRow row;
            DataColumn col1 = new DataColumn("SNO", typeof(int));
            DataColumn col2 = new DataColumn("SlotCode", typeof(string));
            DataColumn col3 = new DataColumn("SlotName", typeof(string));
            DataColumn col4 = new DataColumn("SessionCode", typeof(string));
            DataColumn col5 = new DataColumn("SessionName", typeof(string));
            DataColumn col6 = new DataColumn("PackageCode", typeof(string));
            DataColumn col7 = new DataColumn("PackageName", typeof(string));
            DataColumn col8 = new DataColumn("RegularMembersPerSlot", typeof(int));
            DataColumn col9 = new DataColumn("RegularMembersFilledSlots", typeof(int));
            DataColumn col10 = new DataColumn("IsRegularSlotAvailable", typeof(int));
            DataColumn col11 = new DataColumn("FreeMembersPerSlot", typeof(int));
            DataColumn col12 = new DataColumn("FreeMembersFilledSlots", typeof(int));
            DataColumn col13 = new DataColumn("IsTrialSlotAvailable", typeof(int));
            DataColumn col14 = new DataColumn("PlanCost", typeof(double));
            DataColumn col15 = new DataColumn("Duration", typeof(string));
            DataColumn col16 = new DataColumn("NoOfDays", typeof(int));
            DataColumn col17 = new DataColumn("DurationCode", typeof(string));
            DataColumn col18 = new DataColumn("PlanCode", typeof(string));
            DataColumn col19 = new DataColumn("PlanName", typeof(string));
            DataColumn col20 = new DataColumn("TrainersCode", typeof(string));
            DataColumn col21 = new DataColumn("TrainersName", typeof(string));
            DataColumn col22 = new DataColumn("SlotPrice", typeof(string));
            DataColumn col23 = new DataColumn("PlanCostCode", typeof(string));
            DataColumn col24 = new DataColumn("MinPlanCost", typeof(float));
            DataColumn col25 = new DataColumn("MaxPlanCost", typeof(float));
            DataColumn col26 = new DataColumn("SlotPricingConstant", typeof(float));
            DataColumn col27 = new DataColumn("SlotStartTime", typeof(string));
            DataColumn col28 = new DataColumn("SlotEndTime", typeof(string));


            dt_Slotav.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8, col9, col10, col11, col12, col13, col14, col15, col16, col17, col18, col19, col20, col21, col22, col23, col24, col25, col26, col27, col28 });
            string query1 = "";
            if (PackageCode == "P0")
            {
                //changed on 08/11/2018 by dilip point
                //query1 = "select  CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode from CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP,SlotGroups SG,CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS where CMSP.PackageCode=CMSPC.PackageCode and CMSPC.SlotCode=CMSSS.SlotCode and  CMSS.SessionCode = CMSSS.SessionCode and  CMSSS.SLTGID = SG.SLTGID  and CMSSS.BranchCode = '" + BranchCode + "' and CMSP.PackageCode=CMSPC.PackageCode    group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode";
                //query1 = "select  CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice from CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP,SlotGroups SG,CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS where CMSP.PackageCode=CMSPC.PackageCode and CMSPC.SlotCode=CMSSS.SlotCode and  CMSS.SessionCode = CMSSS.SessionCode and  CMSSS.SLTGID = SG.SLTGID  and CMSP.PackageCode=CMSPC.PackageCode and CMSPC.IsActive=1    group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice";
                ////query1 = "select  CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPC.MinSlotPrice,CMSPC.MaxSlotPrice from CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP,SlotGroups SG,CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS where CMSP.BranchCode='" + BranchCode + "' and  CMSP.IsDeleted=0 and CMSP.IsActive=1 and CMSPC.IsActive=1 and CMSPC.IsDeleted=0  and CMSP.PackageCode=CMSPC.PackageCode and CMSPC.SlotCode=CMSSS.SlotCode and  CMSS.SessionCode = CMSSS.SessionCode and  CMSSS.SLTGID = SG.SLTGID  and CMSP.PackageCode=CMSP.PackageCode   group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPC.MinSlotPrice,CMSPC.MaxSlotPrice";
                // latest query1 = "select  CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant from CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP,SlotGroups SG,CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS,CMSPLANCOST CMSPLC where CMSPLC.PackageCode=CMSPC.PackageCode and CMSPLC.PlanCostCode=CMSPC.PlanCostCode and CMSPLC.BranchCode=CMSPC.BranchCode and CMSP.PackageCode=CMSPC.PackageCode and CMSPC.SlotCode=CMSSS.SlotCode and  CMSS.SessionCode = CMSSS.SessionCode and  CMSSS.SLTGID = SG.SLTGID  and CMSP.BranchCode = '1101' and  CMSP.PackageCode=CMSPC.PackageCode and CMSPC.IsActive=1 and CMSP.IsActive=1 and CMSPC.IsDeleted=0 and CMSP.IsDeleted=0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant";
                // query1 = "select CMSSS.SlotCode,SG.FreeMembersPerSlot,PG.PackageWiseSlot as RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP,SlotGroups SG,PackageGroup PG, CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC where  CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode and CMSSS.SLTGID=PG.SLTGID and CMSSS.SLTGID=SG.SLTGID and PG.SLTGID=SG.SLTGID and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode and CMSSS.SLTGID = SG.SLTGID  and CMSP.BranchCode = '" + BranchCode + "'  and PG.PackageCode=CMSPC.PackageCode and PG.PackageCode=CMSP.PackageCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,PG.PackageWiseSlot ";
                // Removed BranchCode because unmapping issue in Packagegroup and slotgroup (CMSSLOTTIMESETTING).
                query1 = "select CMSSS.SlotCode,SG.FreeMembersPerSlot,PG.PackageWiseSlot as RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP,SlotGroups SG,PackageGroup PG, CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC,SGSSMapping SGSSM where SG.SLTGID=PG.SLTGID and SGSSM.SLTGID=SG.SLTGID and  SGSSM.SlotCode=CMSSS.SlotCode and  CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode   and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode   and CMSP.BranchCode = '" + BranchCode + "'  and PG.PackageCode=CMSPC.PackageCode and PG.PackageCode=CMSP.PackageCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,PG.PackageWiseSlot,CMSSS.SlotStartTime,CMSSS.SlotEndTime";
            }
            else
            {
                //changed on 08/11/2018 by dilip point
                // query1 = "select  CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode from CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP,SlotGroups SG,CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS where CMSP.PackageCode=CMSPC.PackageCode and CMSPC.SlotCode=CMSSS.SlotCode and  CMSS.SessionCode = CMSSS.SessionCode and  CMSSS.SLTGID = SG.SLTGID  and CMSSS.BranchCode = '" + BranchCode + "' and  CMSP.PackageCode=CMSPC.PackageCode    group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode";
                ////query1 = "select  CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPC.MinSlotPrice,CMSPC.MaxSlotPrice from CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP,SlotGroups SG,CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS where CMSP.PackageCode=CMSPC.PackageCode and CMSPC.SlotCode=CMSSS.SlotCode and  CMSS.SessionCode = CMSSS.SessionCode and  CMSSS.SLTGID = SG.SLTGID  and CMSP.BranchCode = '" + BranchCode + "' and CMSPC.PackageCode = '" + PackageCode + "' and  CMSP.PackageCode=CMSPC.PackageCode and CMSPC.IsActive=1 and CMSP.IsActive=1 and CMSPC.IsDeleted=0 and CMSP.IsDeleted=0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPC.MinSlotPrice,CMSPC.MaxSlotPrice";
                //Latest  query1 = "select CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP,SlotGroups SG, CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC where CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode and CMSSS.SLTGID = SG.SLTGID  and CMSP.BranchCode = '1101' and CMSPC.PackageCode = 'P1' and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant ";
                // Removed BranchCode because unmapping issue in Packagegroup and slotgroup (CMSSLOTTIMESETTING).
                // query1 = "select CMSSS.SlotCode,SG.FreeMembersPerSlot,PG.PackageWiseSlot as RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP,SlotGroups SG,PackageGroup PG, CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC where  CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode and CMSSS.SLTGID=PG.SLTGID and CMSSS.SLTGID=SG.SLTGID and PG.SLTGID=SG.SLTGID and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode and CMSSS.SLTGID = SG.SLTGID  and CMSP.BranchCode = '" + BranchCode + "' and CMSPC.PackageCode = '" + PackageCode + "' and PG.PackageCode=CMSPC.PackageCode and PG.PackageCode=CMSP.PackageCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,PG.PackageWiseSlot";
                query1 = "select CMSSS.SlotCode,SG.FreeMembersPerSlot,PG.PackageWiseSlot as RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP,SlotGroups SG,PackageGroup PG, CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC,SGSSMapping SGSSM where SG.SLTGID=PG.SLTGID and SGSSM.SLTGID=SG.SLTGID and  SGSSM.SlotCode=CMSSS.SlotCode and  CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode  and CMSP.BranchCode='" + BranchCode + "'  and PG.PackageCode = '" + PackageCode + "' and PG.PackageCode=CMSPC.PackageCode and PG.PackageCode=CMSP.PackageCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,PG.PackageWiseSlot,CMSSS.SlotStartTime,CMSSS.SlotEndTime";
            }

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            for (int j = 0; j <= ds_custdet1.Tables[0].Rows.Count - 1; j++)
            {
                DurationID = ds_custdet1.Tables[0].Rows[j][11].ToString();
                char[] strGoal = DurationID.ToCharArray();

                row = dt_Slotav.NewRow();
                if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "W")
                {
                    if (Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString()) == 1)
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " Week";
                    }
                    else
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " Week";
                    }
                    NOD = Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString()) * 7;
                }
                else if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "D")
                {
                    if (Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString()) == 1)
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " Day";
                    }
                    else
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " Days";
                    }
                    NOD = Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString());
                }
                else if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "M")
                {
                    if (Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString()) == 1)
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " Month";
                    }
                    else
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " Months";
                    }
                    NOD = Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString()) * 30;
                }
                else
                {
                    if (Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString()) == 1)
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " ";
                    }
                    else
                    {
                        row["Duration"] = ds_custdet1.Tables[0].Rows[j][6].ToString() + " ";
                    }
                    NOD = Convert.ToInt32(ds_custdet1.Tables[0].Rows[j][6].ToString()) * 30;
                }
                // CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPC.MinSlotPrice,CMSPC.MaxSlotPrice


                row["SNO"] = l;
                row["SlotCode"] = ds_custdet1.Tables[0].Rows[j]["SlotCode"].ToString();
                row["SlotName"] = ds_custdet1.Tables[0].Rows[j]["SlotName"].ToString();
                row["SessionCode"] = ds_custdet1.Tables[0].Rows[j]["SessionCode"].ToString();
                row["SessionName"] = ds_custdet1.Tables[0].Rows[j]["SessionName"].ToString();
                row["PackageCode"] = ds_custdet1.Tables[0].Rows[j]["PackageCode"].ToString();
                row["PackageName"] = ds_custdet1.Tables[0].Rows[j]["PackageName"].ToString();
                row["RegularMembersPerSlot"] = ds_custdet1.Tables[0].Rows[j]["RegularMembersPerSlot"].ToString();
                row["RegularMembersFilledSlots"] = "0";
                row["IsRegularSlotAvailable"] = "1";
                row["FreeMembersPerSlot"] = ds_custdet1.Tables[0].Rows[j]["FreeMembersPerSlot"].ToString();
                row["FreeMembersFilledSlots"] = "0";
                row["IsTrialSlotAvailable"] = "1";
                row["PlanCost"] = Convert.ToDouble(ds_custdet1.Tables[0].Rows[j]["PlanCost"].ToString());
                // row["SlotPrice"] = ds_custdet1.Tables[0].Rows[j]["SlotPrice"].ToString();
                row["SlotPrice"] = ds_custdet1.Tables[0].Rows[j]["MinPlanCost"].ToString();
                row["MinPlanCost"] = ds_custdet1.Tables[0].Rows[j]["MinPlanCost"].ToString();
                row["MaxPlanCost"] = ds_custdet1.Tables[0].Rows[j]["MaxPlanCost"].ToString();
                row["NoOfDays"] = NOD;
                row["DurationCode"] = ds_custdet1.Tables[0].Rows[j]["DurationId"].ToString();
                row["PlanCode"] = ds_custdet1.Tables[0].Rows[j]["PlanCode"].ToString();
                row["PlanName"] = ds_custdet1.Tables[0].Rows[j]["PlanName"].ToString();
                row["PlanCostCode"] = ds_custdet1.Tables[0].Rows[j]["PlanCostCode"].ToString();
                dt1 = GetTrainersListBySlots(ds_custdet1.Tables[0].Rows[j][0].ToString(), BranchCode, ds_custdet1.Tables[0].Rows[j][9].ToString());

                row["TrainersCode"] = dt1.Rows[0]["TrainerCode"].ToString();
                row["TrainersName"] = dt1.Rows[0]["TrainerName"].ToString();


                if (ds_custdet1.Tables[0].Rows[j]["SlotPricingConstant"].ToString() != string.Empty)
                {
                    row["SlotPricingConstant"] = ds_custdet1.Tables[0].Rows[j]["SlotPricingConstant"].ToString();
                }
                else
                {
                    row["SlotPricingConstant"] = 0.0f;
                }
                row["SlotStartTime"] = ds_custdet1.Tables[0].Rows[j]["SlotStartTime"].ToString();
                row["SlotEndTime"] = ds_custdet1.Tables[0].Rows[j]["SlotEndTime"].ToString();

                dt_Slotav.Rows.Add(row);
                l = l + 1;
            }

            return dt_Slotav;
        }
        public DataTable GetAllSlotsByPackageCodeUsedAUTOPRICE(string BranchCode, string PackageCode, string StartDate)
        {
            DataSet ds_custdet1 = new DataSet();
            DataTable dt_Slotav = new DataTable();
            //  Prices Tomorrow efffect
            //string query1 = "select SNO=1,Name='Regular',CCRMM.SlotCode,SG.RegularMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots,CCRMM.PackageCode from CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where CCRMM.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID and CCRMM.BranchCode = '" + BranchCode + "'  and CCRMM.IsActive = 1 and CCRMM.StartDate <= Dateadd(DAY, Datediff(DAY, 1, Getdate()), 0) and  CCRMM.MembershipExpireDate >= Dateadd(DAY, Datediff(DAY, -1, Getdate()), 0) group by CCRMM.SlotCode,SG.RegularMembersPerSlot,CCRMM.PackageCode union all  select SNO=2,Name='Trial',TS.SlotCode,SG.FreeMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots from CCRMEnquireFreeTrial TS,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where TS.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID  and TS.IsActive = 1 group by TS.SlotCode,SG.FreeMembersPerSlot ";
            // Prices Immediate efffect
            //string query1 = "select SNO=1,Name='Regular',CCRMM.SlotCode,SG.RegularMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots,'' as PackageCode from CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where CCRMM.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID and CCRMM.BranchCode = '" + BranchCode + "'  and CCRMM.IsActive = 1 and CCRMM.StartDate <= Dateadd(DAY, Datediff(DAY, -1, Getdate()), 0) and  CCRMM.MembershipExpireDate >= Dateadd(DAY, Datediff(DAY, -1, Getdate()), 0) group by CCRMM.SlotCode,SG.RegularMembersPerSlot union all  select SNO=2,Name='Trial',TS.SlotCode,SG.FreeMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots from CCRMEnquireFreeTrial TS,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where TS.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID  and TS.IsActive = 1 group by TS.SlotCode,SG.FreeMembersPerSlot ";


            // Prices Immediate efffect  latest
            //string query1 = "select SNO=1,Name='Regular',CCRMM.SlotCode,SG.RegularMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots,CCRMM.PackageCode from CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where CCRMM.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID and CCRMM.BranchCode = '" + BranchCode + "'  and CCRMM.IsActive = 1 and CCRMM.StartDate <= Dateadd(DAY, Datediff(DAY, -1, Getdate()), 0) and  CCRMM.MembershipExpireDate >= Dateadd(DAY, Datediff(DAY, -1, Getdate()), 0) group by CCRMM.SlotCode,SG.RegularMembersPerSlot,CCRMM.PackageCode  union all  select SNO=2,Name='Trial',TS.SlotCode,SG.FreeMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots,'' as PackageCode  from CCRMEnquireFreeTrial TS,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where TS.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID  and TS.IsActive = 1 group by TS.SlotCode,SG.FreeMembersPerSlot";
            //  Prices Tomorrow efffect latest
            //string query1 = "select SNO=1,Name='Regular',CCRMM.SlotCode,SG.RegularMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots,CCRMM.PackageCode from CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where CCRMM.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID and CCRMM.BranchCode = '" + BranchCode + "'  and CCRMM.IsActive = 1 and CCRMM.StartDate <= Dateadd(DAY, Datediff(DAY, 1, Getdate()), 0) and  CCRMM.MembershipExpireDate >= Dateadd(DAY, Datediff(DAY, -1, Getdate()), 0) group by CCRMM.SlotCode,SG.RegularMembersPerSlot,CCRMM.PackageCode  union all  select SNO=2,Name='Trial',TS.SlotCode,SG.FreeMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots,'' as PackageCode from CCRMEnquireFreeTrial TS,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where TS.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID  and TS.IsActive = 1 group by TS.SlotCode,SG.FreeMembersPerSlot";

            // Slot detecting for given date but upper queries where only for tomorrow
            // Prices Immediate efffect  latest
            string query1 = "select SNO=1,Name='Regular',CCRMM.SlotCode,SG.RegularMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots,CCRMM.PackageCode from CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where CCRMM.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID and CCRMM.BranchCode = '" + BranchCode + "'  and CCRMM.IsActive = 1 and CCRMM.StartDate <= Dateadd(DAY, Datediff(DAY, -1, '" + StartDate + "'), 0) and  CCRMM.MembershipExpireDate >= Dateadd(DAY, Datediff(DAY, -1,'" + StartDate + "'), 0) group by CCRMM.SlotCode,SG.RegularMembersPerSlot,CCRMM.PackageCode  union all  select SNO=2,Name='Trial',TS.SlotCode,SG.FreeMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots,'' as PackageCode  from CCRMEnquireFreeTrial TS,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where TS.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID  and TS.IsActive = 1 group by TS.SlotCode,SG.FreeMembersPerSlot";
            //  Prices Tomorrow efffect latest
            //string query1 = "select SNO=1,Name='Regular',CCRMM.SlotCode,SG.RegularMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots,CCRMM.PackageCode from CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where CCRMM.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID and CCRMM.BranchCode = '" + BranchCode + "'  and CCRMM.IsActive = 1 and CCRMM.StartDate <= Dateadd(DAY, Datediff(DAY, 1, '" + StartDate + "'), 0) and  CCRMM.MembershipExpireDate >= Dateadd(DAY, Datediff(DAY, -1, '" + StartDate + "'), 0) group by CCRMM.SlotCode,SG.RegularMembersPerSlot,CCRMM.PackageCode  union all  select SNO=2,Name='Trial',TS.SlotCode,SG.FreeMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots,'' as PackageCode from CCRMEnquireFreeTrial TS,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where TS.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID  and TS.IsActive = 1 group by TS.SlotCode,SG.FreeMembersPerSlot";

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            dt_Slotav = ds_custdet1.Tables[0];
            return dt_Slotav;
        }
        public DataTable GetTrainersListBySlots(string SlotCode, string BranchCode, string PackageCode)
        {
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet1 = new DataSet();

            DataTable dt_TrainersList = new DataTable();

            string result = string.Empty;

            DataRow row;
            DataColumn col1 = new DataColumn("TrainerCode", typeof(string));
            DataColumn col2 = new DataColumn("TrainerName", typeof(string));

            dt_TrainersList.Columns.AddRange(new DataColumn[] { col1, col2 });

            string query1 = "select TrainerCode,TrainerName from TrainerSlotSpecializationMapping where SlotCode='" + SlotCode + "' and BranchCode='" + BranchCode + "' and PackageCode='" + PackageCode + "' and IsActive=1 ";

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            if (ds_custdet1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i <= ds_custdet1.Tables[0].Rows.Count - 1; i++)
                {
                    row = dt_TrainersList.NewRow();
                    row["TrainerCode"] = ds_custdet1.Tables[0].Rows[i][0].ToString();
                    row["TrainerName"] = ds_custdet1.Tables[0].Rows[i][1].ToString();

                    dt_TrainersList.Rows.Add(row);
                }
            }
            return dt_TrainersList;

        }
        public string Last2digits(int num)
        {
            string x = Convert.ToString(num);
            for (int i = 0; i < x.Length; ++i)
            {
                if (i == 0)
                    x = x.Substring(x.Length - i - 2, 2).PadRight(i + 2, '0');

            }

            return x;
        }

        // FreeTrial Slots

        public string GetOffLineFreeTrialSlots([FromBody]OffLineCMSSlotsAvailability PackagePrices)
        {
            string sJSONResponse = "";

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_slots = new DataSet();
            string result = string.Empty;
            //
            int digit1 = 0;
            int MaxNum = 100;
            int AddNum = 0;
            int MinusNum = 0;

            int AvailableSlots = 0;
            int FilledSlots = 0;
            float PlanCost = 0.0f;
            float SlotPrice = 0.0f;
            float MinimumAmount = 0.0f;
            float MaximumAmount = 0.0f;
            float Constant = 0.0f;

            DataTable dt_Slotavailable = new DataTable();
            DataTable dt_Slotused = new DataTable();
            DataTable dt_Slot = new DataTable();

            string BranchCode = PackagePrices.BranchCode;
            string PackageCode = PackagePrices.PackageCode;

            ArrayList arl_Mobile = MobileCheck(PackagePrices.MobileNo);
            int EnquireformCount = Convert.ToInt32(arl_Mobile[0].ToString());
            int UserCount = Convert.ToInt32(arl_Mobile[1].ToString());

            DataTable dt_IsPackage = new DataTable();

            string MobileNo = PackagePrices.MobileNo;
            string MembershipCode = PackagePrices.MembershipCode;



            int EnquiretypeNo = 0;

            string UniqueId = "";

            try
            {

                if (MobileNo != null && MembershipCode != "")
                {
                    UniqueId = MobileNo;
                }
                else if (MobileNo != "" && MembershipCode == "")
                {
                    UniqueId = MobileNo;
                }
                else
                {
                    UniqueId = "";
                }


                DataTable dt = getdata(string.Format("select ID,DisplayName from DashboardDisplay", ""));


                if (UniqueId != "")
                {

                    dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM CCRMMembership AS T1,Users T2   WHERE  T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode)  SELECT (SELECT top 1 T1.EnquireTypeNo FROM CCRMMembership AS T1,Users T2  WHERE T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", PackagePrices.MobileNo));
                }
                else
                {
                    dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM CCRMMembership AS T1,Users T2   WHERE  T1.MembershipCode=T2.UCode)  SELECT (SELECT top 1 T1.EnquireTypeNo FROM CCRMMembership AS T1,Users T2  WHERE  T1.MembershipCode=T2.UCode order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", ""));
                }


                List<OffLinePackagePriceList> dbPackage = new List<OffLinePackagePriceList>();

                if (Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) == 0)
                {
                    EnquiretypeNo = 1;
                }
                else if (Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString()) > 0)
                {
                    EnquiretypeNo = 0;

                }
                else
                {
                    EnquiretypeNo = 1;
                }
            }
            catch (Exception ec)
            {

            }

            dt_Slotused = GetAllSlotsByPackageCodeUsedAUTOPRICE(PackagePrices.BranchCode, PackagePrices.PackageCode, PackagePrices.StartDate);
            if (dt_Slotused.Rows.Count > 0)
            {
                dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICE(PackagePrices.BranchCode, PackagePrices.PackageCode);
                foreach (DataRow rrow in dt_Slotavailable.Rows)
                {
                    foreach (DataRow trow in dt_Slotused.Rows)
                    {
                        if (rrow["SlotCode"].ToString() == trow["SlotCode"].ToString())
                        {
                            //PackageCode
                            if (rrow["PackageCode"].ToString() == trow["PackageCode"].ToString())
                            {
                                string val1 = rrow["SlotCode"].ToString();
                                string val2 = trow["SlotCode"].ToString();

                                if (trow["SNO"].ToString() == "1")
                                {
                                    //  rrow["RegularMembersPerSlot"] = trow["AvailableSlots"].ToString();
                                    // rrow["RegularMembersFilledSlots"] = trow["FilledSlots"].ToString();

                                    //AvailableSlots = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                    AvailableSlots = Convert.ToInt32(rrow["RegularMembersPerSlot"]);
                                    FilledSlots = Convert.ToInt32(trow["FilledSlots"].ToString());
                                    PlanCost = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                    SlotPrice = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                    MinimumAmount = Convert.ToInt32(rrow["MinPlanCost"].ToString());
                                    MaximumAmount = Convert.ToInt32(rrow["MaxPlanCost"].ToString());
                                    Constant = Convert.ToInt32(rrow["SlotPricingConstant"].ToString());
                                    float x = ((Constant / AvailableSlots) / 100);
                                    float y = MinimumAmount * x;
                                    float z = FilledSlots * y;
                                    int A = (int)z;
                                    int B = Convert.ToInt32(MinimumAmount) + A;
                                    int numlength = B.ToString().Length;

                                    if (numlength > 2)
                                    {
                                        //for (int i = 0; i <= numlength - 1; i++)
                                        //{
                                        //    if(i==0)
                                        //    {
                                        //        digit1=Convert.ToInt16(Last2digits(B));
                                        //    }
                                        //}

                                        digit1 = Convert.ToInt16(Last2digits(B));

                                        if (digit1 <= 50)
                                        {
                                            MinusNum = digit1;
                                            B = B - digit1;
                                        }
                                        else if (digit1 > 50)
                                        {
                                            AddNum = MaxNum - digit1;
                                            B = B + AddNum;
                                        }
                                    }
                                    // rrow["SlotPrice"] = MinimumAmount + z;


                                    rrow["SlotPrice"] = B;
                                    float aval = MinimumAmount + z;

                                    if (FilledSlots < AvailableSlots)
                                        rrow["IsRegularSlotAvailable"] = 1;
                                    else if (FilledSlots == AvailableSlots)
                                        rrow["IsRegularSlotAvailable"] = 0;
                                    else
                                        rrow["IsRegularSlotAvailable"] = 0;
                                    //rrow["RegularMembersFilledSlots"] = Convert.ToInt32(rrow["RegularMembersPerSlot"]) - Convert.ToInt32(trow["FilledSlots"].ToString());
                                    rrow["RegularMembersFilledSlots"] = Convert.ToInt32(trow["FilledSlots"].ToString());

                                }
                            }

                            if (rrow["SlotCode"].ToString() == trow["SlotCode"].ToString())
                            {
                                string val1 = rrow["SlotCode"].ToString();
                                string val2 = trow["SlotCode"].ToString();

                                // Trial Slots
                                if (trow["SNO"].ToString() == "2")
                                {
                                    rrow["FreeMembersPerSlot"] = trow["AvailableSlots"].ToString();
                                    rrow["FreeMembersFilledSlots"] = trow["FilledSlots"].ToString();

                                    AvailableSlots = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                    FilledSlots = Convert.ToInt32(trow["FilledSlots"].ToString());
                                    PlanCost = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                    SlotPrice = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                    MinimumAmount = Convert.ToInt32(rrow["MinPlanCost"].ToString());
                                    MaximumAmount = Convert.ToInt32(rrow["MaxPlanCost"].ToString());
                                    Constant = Convert.ToInt32(rrow["SlotPricingConstant"].ToString());


                                    //rrow["SlotPrice"] = (MinimumAmount + (FilledSlots) * (MinimumAmount * ((Constant / AvailableSlots) / 100)));
                                    rrow["SlotPrice"] = 0;
                                    float aval = (MinimumAmount + (FilledSlots) * (MinimumAmount * ((Constant / AvailableSlots) / 100)));

                                    if (FilledSlots < AvailableSlots)
                                        rrow["IsTrialSlotAvailable"] = 1;
                                    else if (FilledSlots == AvailableSlots)
                                        rrow["IsTrialSlotAvailable"] = 0;
                                    else
                                        rrow["IsTrialSlotAvailable"] = 0;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICEDefault(BranchCode, PackageCode);
            }

            int sloutcount = dt_Slotavailable.Rows.Count;
            dt_Slotavailable.DefaultView.Sort = "SlotCode ASC";
            DataTable st_newSlotavailable = new DataTable();
            st_newSlotavailable = dt_Slotavailable.DefaultView.ToTable();
            DataTable dt_FilteredSlotsAvailable = new DataTable();

            List<OffLineEnquireInfo> olei = new List<OffLineEnquireInfo>();
            olei.Add(new OffLineEnquireInfo { isFreeTrial = EnquiretypeNo });

            DataTable dt_Sessions = getdata(string.Format("select distinct CMSSS.SessionCode,CMSSS.SessionName,convert(varchar(10),CMSSS.SessionStartTime, 108) as SessionStartTime,convert(varchar(10),  CMSSS.SessionEndTime, 108) as SessionEndTime  from  CMSSESSIONTIMESETTING CMSSS  order by SessionCode asc", ""));
            List<OffLineFreeTrialSessions> OnlineFreeTrialSessions = new List<OffLineFreeTrialSessions>();

            for (int i = 0; i < dt_Sessions.Rows.Count - 1; i++)
            {
                OffLineFreeTrialSessions SessionsDetails = new OffLineFreeTrialSessions { sessionId = dt_Sessions.Rows[i]["SessionCode"].ToString(), sessionName = dt_Sessions.Rows[i]["SessionName"].ToString(), sessionStartTime = dt_Sessions.Rows[i]["SessionStartTime"].ToString(), sessionEndTime = dt_Sessions.Rows[i]["SessionEndTime"].ToString(), slots = GetFreeTrialSlotsDetails(st_newSlotavailable, dt_Sessions.Rows[i]["SessionCode"].ToString()) };
                OnlineFreeTrialSessions.Add(SessionsDetails);
            }

            List<OffLineFreeTrialSlotsPackageDetails> dt_SlotPackageDopt = new List<OffLineFreeTrialSlotsPackageDetails>();
            OffLineFreeTrialSlotsPackageDetails dashboardlistpackage = new OffLineFreeTrialSlotsPackageDetails { info = olei, sessions = OnlineFreeTrialSessions };
            dt_SlotPackageDopt.Add(dashboardlistpackage);

            OffLineFreeTrialSlotsPackageOutPut olppFOP = new OffLineFreeTrialSlotsPackageOutPut();

            olppFOP.status = "Success";
            olppFOP.value = dt_SlotPackageDopt;
            sJSONResponse = JsonConvert.SerializeObject(olppFOP);


            //DataTable uniqueCols = st_newSlotavailable.DefaultView.ToTable(true,"SNO", "SlotCode", "NoOfDays","IsRegularSlotAvailable", "IsTrialSlotAvailable", "FreeMembersPerSlot", "RegularMembersPerSlot", "SessionName", "SlotName", "PlanCost", "Duration", "SessionCode", "PackageCode", "PackageName", "DurationCode", "PlanCode", "PlanName", "SlotPrice", "PlanCostCode", "MinPlanCost", "MaxPlanCost", "SlotPricingConstant");
            //uniqueCols.DefaultView.Sort = "SlotCode ASC";
            //dt_Slotavailable = uniqueCols.DefaultView.ToTable();


            //result = GetJson(st_newSlotavailable);
            //int sloutcount1 = dt_Slotavailable.Rows.Count;
            //ds_slots.Tables.Clear();
            //ds_slots.Tables.Add(ReturnException("Success", result));
            //result = GetJson(ds_slots.Tables[0]);
            return sJSONResponse;

        }
        public List<OffLineFreeTrialSlotsPackage> GetFreeTrialSlotsDetails(DataTable dt_SlotPackages, string SessionCode)
        {
            List<OffLineFreeTrialSlotsPackage> offLineFreeTrialSlotPackage = new List<OffLineFreeTrialSlotsPackage>();

            try
            {
                DataRow[] results = dt_SlotPackages.Select("SessionCode = '" + SessionCode + "' ");
                dt_SlotPackages = results.CopyToDataTable();


                for (int i = 0; i < dt_SlotPackages.Rows.Count; i++)
                {

                    offLineFreeTrialSlotPackage.Add(new OffLineFreeTrialSlotsPackage
                    {
                       //sNo = Convert.ToInt32(dt_SlotPackages.Rows[i]["SNO"])
                       // ,
                       // slotCode = Convert.ToString(dt_SlotPackages.Rows[i]["SlotCode"])
                       //  ,
                       // slotName = Convert.ToString(dt_SlotPackages.Rows[i]["SlotName"])
                       // ,
                       // packageCode = Convert.ToString(dt_SlotPackages.Rows[i]["PackageCode"])
                       // ,
                       // packageName = Convert.ToString(dt_SlotPackages.Rows[i]["PackageName"])
                       // ,
                       // isSlotAvailable = Convert.ToInt32(dt_SlotPackages.Rows[i]["IsRegularSlotAvailable"])
                       //  ,
                       // trainerCode = Convert.ToString(dt_SlotPackages.Rows[i]["TrainersCode"])
                       //  ,
                       // trainerName = Convert.ToString(dt_SlotPackages.Rows[i]["TrainersName"])
                       //  ,
                       // slotStartTime = Convert.ToString(dt_SlotPackages.Rows[i]["SlotStartTime"])
                       //  ,
                       // slotEndTime = Convert.ToString(dt_SlotPackages.Rows[i]["SlotEndTime"])


                    });

                }

            }
            catch (Exception ec)
            {

            }

            return offLineFreeTrialSlotPackage;
        }


        // GetOfflineDynamicPricing (with only slots) :: GetOfflineDynamicPricingWithSessions (with session and slots)
        public string GetOfflineDynamicPricingWithSessions([FromBody]OffLineCMSSlotsAvailability PackagePrices)
        {
            string sJSONResponse = "";

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_slots = new DataSet();
            string result = string.Empty;
            //
            int digit1 = 0;
            int MaxNum = 100;
            int AddNum = 0;
            int MinusNum = 0;

            int AvailableSlots = 0;
            int FilledSlots = 0;
            float PlanCost = 0.0f;
            float SlotPrice = 0.0f;
            float MinimumAmount = 0.0f;
            float MaximumAmount = 0.0f;
            float Constant = 0.0f;

            DataTable dt_IsPackage = new DataTable();
            string MobileNo = PackagePrices.MobileNo;
            string MembershipCode = PackagePrices.MembershipCode;

            int EnquiretypeNo = 0;


            DataTable dt_Slotavailable = new DataTable();
            DataTable dt_Slotused = new DataTable();
            DataTable dt_Slot = new DataTable();

            string BranchCode = PackagePrices.BranchCode;
            string PackageCode = PackagePrices.PackageCode;

            ArrayList arl_Mobile = MobileCheck(PackagePrices.MobileNo);
            int EnquireformCount = Convert.ToInt32(arl_Mobile[0].ToString());
            int UserCount = Convert.ToInt32(arl_Mobile[1].ToString());

            string UniqueId = "";

            try
            {

                if (MobileNo != null && MembershipCode != "")
                {
                    UniqueId = MobileNo;
                }
                else if (MobileNo != "" && MembershipCode == "")
                {
                    UniqueId = MobileNo;
                }
                else
                {
                    UniqueId = "";
                }


                DataTable dt = getdata(string.Format("select ID,DisplayName from DashboardDisplay", ""));


                if (UniqueId != "")
                {

                    dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM CCRMMembership AS T1,Users T2   WHERE  T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode)  SELECT (SELECT top 1 T1.EnquireTypeNo FROM CCRMMembership AS T1,Users T2  WHERE T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", PackagePrices.MobileNo));
                    EnquiretypeNo = Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString());
                }
                else
                {
                    // dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM CCRMMembership AS T1,Users T2   WHERE   T1.MembershipCode=T2.UCode)  SELECT (SELECT top 1 T1.EnquireTypeNo FROM CCRMMembership AS T1,Users T2  WHERE T1.MembershipCode=T2.UCode order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", ""));
                    EnquiretypeNo = 0;
                }



                List<OffLinePackagePriceList> dbPackage = new List<OffLinePackagePriceList>();
                try
                {
                    if (EnquiretypeNo == 0)
                    {
                        EnquiretypeNo = 1;
                    }
                    else if (EnquiretypeNo > 0)
                    {
                        EnquiretypeNo = 0;

                    }
                    else
                    {
                        EnquiretypeNo = 1;
                    }
                }
                catch (Exception ec)
                {


                }
            }
            catch (Exception ec)
            {

            }



            dt_Slotused = GetAllSlotsByPackageCodeUsedAUTOPRICE(PackagePrices.BranchCode, PackagePrices.PackageCode, PackagePrices.StartDate);
            if (dt_Slotused.Rows.Count > 0)
            {
                dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICE(PackagePrices.BranchCode, PackagePrices.PackageCode);
                foreach (DataRow rrow in dt_Slotavailable.Rows)
                {
                    foreach (DataRow trow in dt_Slotused.Rows)
                    {
                        if (rrow["SlotCode"].ToString() == trow["SlotCode"].ToString())
                        {
                            //PackageCode
                            if (rrow["PackageCode"].ToString() == trow["PackageCode"].ToString())
                            {
                                string val1 = rrow["SlotCode"].ToString();
                                string val2 = trow["SlotCode"].ToString();

                                if (trow["SNO"].ToString() == "1")
                                {
                                    //  rrow["RegularMembersPerSlot"] = trow["AvailableSlots"].ToString();
                                    // rrow["RegularMembersFilledSlots"] = trow["FilledSlots"].ToString();

                                    //AvailableSlots = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                    AvailableSlots = Convert.ToInt32(rrow["RegularMembersPerSlot"]);
                                    FilledSlots = Convert.ToInt32(trow["FilledSlots"].ToString());
                                    PlanCost = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                    SlotPrice = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                    MinimumAmount = Convert.ToInt32(rrow["MinPlanCost"].ToString());
                                    MaximumAmount = Convert.ToInt32(rrow["MaxPlanCost"].ToString());
                                    Constant = Convert.ToInt32(rrow["SlotPricingConstant"].ToString());
                                    float x = ((Constant / AvailableSlots) / 100);
                                    float y = MinimumAmount * x;
                                    float z = FilledSlots * y;
                                    int A = (int)z;
                                    int B = Convert.ToInt32(MinimumAmount) + A;
                                    int numlength = B.ToString().Length;

                                    if (numlength > 2)
                                    {
                                        //for (int i = 0; i <= numlength - 1; i++)
                                        //{
                                        //    if(i==0)
                                        //    {
                                        //        digit1=Convert.ToInt16(Last2digits(B));
                                        //    }
                                        //}

                                        digit1 = Convert.ToInt16(Last2digits(B));

                                        if (digit1 <= 50)
                                        {
                                            MinusNum = digit1;
                                            B = B - digit1;
                                        }
                                        else if (digit1 > 50)
                                        {
                                            AddNum = MaxNum - digit1;
                                            B = B + AddNum;
                                        }
                                    }
                                    // rrow["SlotPrice"] = MinimumAmount + z;


                                    rrow["SlotPrice"] = B;
                                    float aval = MinimumAmount + z;

                                    if (FilledSlots < AvailableSlots)
                                        rrow["IsRegularSlotAvailable"] = 1;
                                    else if (FilledSlots == AvailableSlots)
                                        rrow["IsRegularSlotAvailable"] = 0;
                                    else
                                        rrow["IsRegularSlotAvailable"] = 0;
                                    //rrow["RegularMembersFilledSlots"] = Convert.ToInt32(rrow["RegularMembersPerSlot"]) - Convert.ToInt32(trow["FilledSlots"].ToString());
                                    rrow["RegularMembersFilledSlots"] = Convert.ToInt32(trow["FilledSlots"].ToString());

                                }
                            }

                            if (rrow["SlotCode"].ToString() == trow["SlotCode"].ToString())
                            {
                                string val1 = rrow["SlotCode"].ToString();
                                string val2 = trow["SlotCode"].ToString();

                                // Trial Slots
                                //if (trow["SNO"].ToString() == "2")
                                //{
                                //    rrow["FreeMembersPerSlot"] = trow["AvailableSlots"].ToString();
                                //    rrow["FreeMembersFilledSlots"] = trow["FilledSlots"].ToString();

                                //    AvailableSlots = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                //    FilledSlots = Convert.ToInt32(trow["FilledSlots"].ToString());
                                //    PlanCost = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                //    SlotPrice = Convert.ToInt32(trow["AvailableSlots"].ToString());
                                //    MinimumAmount = Convert.ToInt32(rrow["MinPlanCost"].ToString());
                                //    MaximumAmount = Convert.ToInt32(rrow["MaxPlanCost"].ToString());
                                //    Constant = Convert.ToInt32(rrow["SlotPricingConstant"].ToString());


                                //    rrow["SlotPrice"] = (MinimumAmount + (FilledSlots) * (MinimumAmount * ((Constant / AvailableSlots) / 100)));

                                //    float aval = (MinimumAmount + (FilledSlots) * (MinimumAmount * ((Constant / AvailableSlots) / 100)));

                                //    if (FilledSlots < AvailableSlots)
                                //        rrow["IsTrialSlotAvailable"] = 1;
                                //    else if (FilledSlots == AvailableSlots)
                                //        rrow["IsTrialSlotAvailable"] = 0;
                                //    else
                                //        rrow["IsTrialSlotAvailable"] = 0;
                                //}
                            }
                        }
                    }
                }
            }
            else
            {
                dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICEDefault(BranchCode, PackageCode);
            }






            int sloutcount = dt_Slotavailable.Rows.Count;
            dt_Slotavailable.DefaultView.Sort = "SlotCode ASC";
            DataTable st_newSlotavailable = new DataTable();
            st_newSlotavailable = dt_Slotavailable.DefaultView.ToTable();
            DataTable dt_FilteredSlotsAvailable = new DataTable();

            List<OffLineEnquireInfo> olei = new List<OffLineEnquireInfo>();
            olei.Add(new OffLineEnquireInfo { isFreeTrial = EnquiretypeNo, enquireTypeNo = 2 });

            DataTable dt_Sessions = getdata(string.Format("select distinct CMSSS.SessionCode,CMSSS.SessionName,convert(varchar(10),CMSSS.SessionStartTime, 108) as SessionStartTime,convert(varchar(10),  CMSSS.SessionEndTime, 108) as SessionEndTime  from  CMSSESSIONTIMESETTING CMSSS  order by SessionCode asc", ""));
            List<OffLineSessions> OnlineSessions = new List<OffLineSessions>();
            for (int i = 0; i < dt_Sessions.Rows.Count - 1; i++)
            {
                OffLineSessions SessionsDetails = new OffLineSessions { sessionId = dt_Sessions.Rows[i]["SessionCode"].ToString(), sessionName = dt_Sessions.Rows[i]["SessionName"].ToString(), sessionStartTime = dt_Sessions.Rows[i]["SessionStartTime"].ToString(), sessionEndTime = dt_Sessions.Rows[i]["SessionEndTime"].ToString(), slots = GetSlotsDetails(st_newSlotavailable, dt_Sessions.Rows[i]["SessionCode"].ToString()) };
                OnlineSessions.Add(SessionsDetails);
            }

            List<OffLineSlotsPackageDetails> dt_SlotPackageDopt = new List<OffLineSlotsPackageDetails>();
            OffLineSlotsPackageDetails dashboardlistpackage = new OffLineSlotsPackageDetails { info = olei, sessions = OnlineSessions };
            dt_SlotPackageDopt.Add(dashboardlistpackage);

            OffLineSlotsPackageOutPut olppFOP = new OffLineSlotsPackageOutPut();

            olppFOP.status = "Success";
            olppFOP.value = dt_SlotPackageDopt;
            sJSONResponse = JsonConvert.SerializeObject(olppFOP);


            //DataTable uniqueCols = st_newSlotavailable.DefaultView.ToTable(true,"SNO", "SlotCode", "NoOfDays","IsRegularSlotAvailable", "IsTrialSlotAvailable", "FreeMembersPerSlot", "RegularMembersPerSlot", "SessionName", "SlotName", "PlanCost", "Duration", "SessionCode", "PackageCode", "PackageName", "DurationCode", "PlanCode", "PlanName", "SlotPrice", "PlanCostCode", "MinPlanCost", "MaxPlanCost", "SlotPricingConstant");
            //uniqueCols.DefaultView.Sort = "SlotCode ASC";
            //dt_Slotavailable = uniqueCols.DefaultView.ToTable();


            //result = GetJson(st_newSlotavailable);
            //int sloutcount1 = dt_Slotavailable.Rows.Count;
            //ds_slots.Tables.Clear();
            //ds_slots.Tables.Add(ReturnException("Success", result));
            //result = GetJson(ds_slots.Tables[0]);
            return sJSONResponse;

        }
    }
}