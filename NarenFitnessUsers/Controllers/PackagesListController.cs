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
    public class PackagesListController : Controller
    {
        //

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
            DataColumn col3 = new DataColumn("SessionCode", typeof(string));
            DataColumn col4 = new DataColumn("PackageCode", typeof(string));
            DataColumn col5 = new DataColumn("RegularMembersPerSlot", typeof(int));
            DataColumn col6 = new DataColumn("RegularMembersFilledSlots", typeof(int));
            DataColumn col7 = new DataColumn("IsRegularSlotAvailable", typeof(int));
            DataColumn col8 = new DataColumn("FreeMembersPerSlot", typeof(int));//take as count
            DataColumn col9 = new DataColumn("FreeMembersFilledSlots", typeof(int));//take as slot
            DataColumn col10 = new DataColumn("IsTrialSlotAvailable", typeof(int));
            DataColumn col11 = new DataColumn("PlanCost", typeof(double));
            DataColumn col12 = new DataColumn("Duration", typeof(string));
            DataColumn col13 = new DataColumn("NoOfDays", typeof(int));
            DataColumn col14 = new DataColumn("DurationCode", typeof(string));
            DataColumn col15 = new DataColumn("PlanCode", typeof(string));
            DataColumn col16 = new DataColumn("PlanName", typeof(string));
            DataColumn col17 = new DataColumn("TrainersCode", typeof(string));
            DataColumn col18 = new DataColumn("TrainersName", typeof(string));
            DataColumn col19 = new DataColumn("SlotPrice", typeof(string));
            DataColumn col20 = new DataColumn("PlanCostCode", typeof(string));
            DataColumn col21 = new DataColumn("MinPlanCost", typeof(float));
            DataColumn col22 = new DataColumn("MaxPlanCost", typeof(float));
            DataColumn col23 = new DataColumn("SlotPricingConstant", typeof(float));
            DataColumn col24 = new DataColumn("SlotStartTime", typeof(string));
            DataColumn col25 = new DataColumn("SlotEndTime", typeof(string));
            DataColumn col26 = new DataColumn("Date", typeof(DateTime));
            DataColumn col27 = new DataColumn("DiscountAmount", typeof(float));//remove
            DataColumn col28 = new DataColumn("DiscountPercentage", typeof(float));//remove
            DataColumn col29 = new DataColumn("PerMonthPrice", typeof(float));

            dt_Slotav.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8, col9, col10, col11, col12, col13, col14, col15, col16, col17, col18, col19, col20, col21, col22, col23, col24, col25, col26, col27, col28, col29 });
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
                query1 = "select CMSSS.SlotCode,FreeMembersPerSlot=1,CMSSWA.AllocatedCount as RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP,CMSSlotWiseAllocation CMSSWA , CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC,SGSSMapping SGSSM where CMSSWA.PackageCode=CMSPC.PackageCode and CMSSWA.Duration=CMSPC.DurationId and  CMSSWA.PackageCode = '" + PackageCode + "' and CMSSWA.PackageCode=CMSPC.PackageCode and CMSSWA.PackageCode=CMSP.PackageCode and  SGSSM.SlotCode=CMSSS.SlotCode and  CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode  and CMSP.BranchCode='" + BranchCode + "'   and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,CMSSWA.AllocatedCount,CMSSS.SlotStartTime,CMSSS.SlotEndTime";
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
                row["SessionCode"] = ds_custdet1.Tables[0].Rows[j]["SessionCode"].ToString();
                row["PackageCode"] = ds_custdet1.Tables[0].Rows[j]["PackageCode"].ToString();
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
                try
                {
                    row["TrainersCode"] = dt1.Rows[0]["TrainerCode"].ToString();
                    row["TrainersName"] = dt1.Rows[0]["TrainerName"].ToString();
                }
                catch (Exception ec)
                {
                    row["TrainersCode"] = "";
                    row["TrainersName"] = "";
                }
                row["SlotPricingConstant"] = 0.0f;
                row["SlotStartTime"] = 0.0f;
                row["SlotEndTime"] = 0.0f;

                dt_Slotav.Rows.Add(row);
                l = l + 1;
            }

            return dt_Slotav;
        }
        public DataTable GetAllSlotsByPackageCodeAvailableAUTOPRICE(string BranchCode, string PackageCode)
        {
            DataTable dt1 = new DataTable();

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
            DataColumn col3 = new DataColumn("SessionCode", typeof(string));
            DataColumn col4 = new DataColumn("PackageCode", typeof(string));
            DataColumn col5 = new DataColumn("RegularMembersPerSlot", typeof(int));
            DataColumn col6 = new DataColumn("RegularMembersFilledSlots", typeof(int));
            DataColumn col7 = new DataColumn("IsRegularSlotAvailable", typeof(int));
            DataColumn col8 = new DataColumn("FreeMembersPerSlot", typeof(int));
            DataColumn col9 = new DataColumn("FreeMembersFilledSlots", typeof(int));
            DataColumn col10 = new DataColumn("IsTrialSlotAvailable", typeof(int));
            DataColumn col11 = new DataColumn("PlanCost", typeof(double));
            DataColumn col12 = new DataColumn("Duration", typeof(string));
            DataColumn col13 = new DataColumn("NoOfDays", typeof(int));
            DataColumn col14 = new DataColumn("DurationCode", typeof(string));
            DataColumn col15 = new DataColumn("PlanCode", typeof(string));
            DataColumn col16 = new DataColumn("PlanName", typeof(string));
            DataColumn col17 = new DataColumn("TrainersCode", typeof(string));
            DataColumn col18 = new DataColumn("TrainersName", typeof(string));//remove
            DataColumn col19 = new DataColumn("SlotPrice", typeof(double));
            DataColumn col20 = new DataColumn("PlanCostCode", typeof(string));
            DataColumn col21 = new DataColumn("MinPlanCost", typeof(float));
            DataColumn col22 = new DataColumn("MaxPlanCost", typeof(float));
            DataColumn col23 = new DataColumn("SlotPricingConstant", typeof(float));
            DataColumn col24 = new DataColumn("SlotStartTime", typeof(string));//remove
            DataColumn col25 = new DataColumn("SlotEndTime", typeof(string));//remove
            DataColumn col26 = new DataColumn("Date", typeof(DateTime));
            DataColumn col27 = new DataColumn("DiscountAmount", typeof(float));//remove
            DataColumn col28 = new DataColumn("DiscountPercentage", typeof(float));//remove
            DataColumn col29 = new DataColumn("PerMonthPrice", typeof(float));
            DataColumn col30 = new DataColumn("DurationNo", typeof(int));


            dt_Slotav.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8, col9, col10, col11, col12, col13, col14, col15, col16, col17, col18, col19, col20, col21, col22, col23, col24, col25, col26, col27, col28, col29, col30 });
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
                // query1 = "select CMSSS.SlotCode,SG.FreeMembersPerSlot,PG.PackageWiseSlot as RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP,SlotGroups SG,PackageGroup PG, CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC,SGSSMapping SGSSM where SG.SLTGID=PG.SLTGID and SGSSM.SLTGID=SG.SLTGID and  SGSSM.SlotCode=CMSSS.SlotCode and  CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode   and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode   and CMSP.BranchCode = '" + BranchCode + "'  and PG.PackageCode=CMSPC.PackageCode and PG.PackageCode=CMSP.PackageCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,PG.PackageWiseSlot,CMSSS.SlotStartTime,CMSSS.SlotEndTime";
                query1 = "select CMSSS.SlotCode,SG.FreeMembersPerSlot,PG.AllocatedCount as RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration / Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,convert(varchar(10), CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10), CMSSS.SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP, SlotGroups SG,CMSSlotWiseAllocation PG, CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC,SGSSMapping SGSSM where PG.SlotCode = SGSSM.SlotCode and PG.PackageCode = CMSP.PackageCode  and SGSSM.SLTGID = SG.SLTGID and SGSSM.SlotCode = CMSSS.SlotCode and CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode   and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode   and CMSP.BranchCode = '" + BranchCode + "'  and PG.PackageCode = CMSPC.PackageCode and PG.PackageCode = CMSP.PackageCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration / Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,PG.AllocatedCount,CMSSS.SlotStartTime,CMSSS.SlotEndTime";
            }
            else
            {
                //changed on 08/11/2018 by dilip point
                // query1 = "select  CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode from CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP,SlotGroups SG,CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS where CMSP.PackageCode=CMSPC.PackageCode and CMSPC.SlotCode=CMSSS.SlotCode and  CMSS.SessionCode = CMSSS.SessionCode and  CMSSS.SLTGID = SG.SLTGID  and CMSSS.BranchCode = '" + BranchCode + "' and  CMSP.PackageCode=CMSPC.PackageCode    group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode";
                ////query1 = "select  CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPC.MinSlotPrice,CMSPC.MaxSlotPrice from CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP,SlotGroups SG,CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS where CMSP.PackageCode=CMSPC.PackageCode and CMSPC.SlotCode=CMSSS.SlotCode and  CMSS.SessionCode = CMSSS.SessionCode and  CMSSS.SLTGID = SG.SLTGID  and CMSP.BranchCode = '" + BranchCode + "' and CMSPC.PackageCode = '" + PackageCode + "' and  CMSP.PackageCode=CMSPC.PackageCode and CMSPC.IsActive=1 and CMSP.IsActive=1 and CMSPC.IsDeleted=0 and CMSP.IsDeleted=0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPC.MinSlotPrice,CMSPC.MaxSlotPrice";
                //Latest  query1 = "select CMSSS.SlotCode,SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP,SlotGroups SG, CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC where CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode and CMSSS.SLTGID = SG.SLTGID  and CMSP.BranchCode = '1101' and CMSPC.PackageCode = 'P1' and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant ";
                // Removed BranchCode because unmapping issue in Packagegroup and slotgroup (CMSSLOTTIMESETTING).
                // query1 = "select CMSSS.SlotCode,SG.FreeMembersPerSlot,PG.PackageWiseSlot as RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP,SlotGroups SG,PackageGroup PG, CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC where  CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode and CMSSS.SLTGID=PG.SLTGID and CMSSS.SLTGID=SG.SLTGID and PG.SLTGID=SG.SLTGID and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode and CMSSS.SLTGID = SG.SLTGID  and CMSP.BranchCode = '" + BranchCode + "' and CMSPC.PackageCode = '" + PackageCode + "' and PG.PackageCode=CMSPC.PackageCode and PG.PackageCode=CMSP.PackageCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,PG.PackageWiseSlot";
                //query1 = "select CMSSS.SlotCode,SG.FreeMembersPerSlot,PG.PackageWiseSlot as RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP,SlotGroups SG,PackageGroup PG, CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC,SGSSMapping SGSSM where SG.SLTGID=PG.SLTGID and SGSSM.SLTGID=SG.SLTGID and  SGSSM.SlotCode=CMSSS.SlotCode and  CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode  and CMSP.BranchCode='" + BranchCode + "'  and PG.PackageCode = '" + PackageCode + "' and PG.PackageCode=CMSPC.PackageCode and PG.PackageCode=CMSP.PackageCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,PG.PackageWiseSlot,CMSSS.SlotStartTime,CMSSS.SlotEndTime";
                //query1 = "select CMSSS.SlotCode,FreeMembersPerSlot=1,CMSSWA.AllocatedCount as RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP,CMSSlotWiseAllocation CMSSWA , CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC,SGSSMapping SGSSM where CMSSWA.PackageCode=CMSPC.PackageCode and CMSSWA.Duration=CMSPC.DurationId and  CMSSWA.PackageCode = '" + PackageCode + "' and CMSSWA.PackageCode=CMSPC.PackageCode and CMSSWA.PackageCode=CMSP.PackageCode and  SGSSM.SlotCode=CMSSS.SlotCode and  CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode  and CMSP.BranchCode='" + BranchCode + "'   and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,CMSSWA.AllocatedCount,CMSSS.SlotStartTime,CMSSS.SlotEndTime";
                query1 = "select CMSSS.SlotCode,FreeMembersPerSlot = 1,CMSSWA.AllocatedCount as RegularMembersPerSlot,CMSS.SessionName,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly] as Duration,CMSS.SessionCode,CMSS.SessionName,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSSWA.MinPrice as MinPlanCost,CMSSWA.MaxPrice as MaxPlanCost,CMSPLC.SlotPricingConstant,convert(varchar(10), CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10), CMSSS.SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING CMSSS, CMSPACKAGES CMSP, CMSSlotWiseAllocation CMSSWA , CMSPACKAGESCOST CMSPC, CMSSESSIONTIMESETTING CMSS, CMSPLANCOST CMSPLC, SGSSMapping SGSSM where CMSSWA.PackageCode = CMSPC.PackageCode and CMSSWA.Duration = CMSPC.DurationId and CMSSWA.SlotCode = CMSPC.SlotCode and CMSSWA.PackageCode = '" + PackageCode + "' and CMSSWA.PackageCode = CMSPC.PackageCode and CMSSWA.PackageCode = CMSP.PackageCode and SGSSM.SlotCode = CMSSS.SlotCode and CMSPLC.PackageCode = CMSPC.PackageCode and CMSPLC.PlanCostCode = CMSPC.PlanCostCode and CMSPLC.BranchCode = CMSPC.BranchCode and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.SlotCode = CMSSS.SlotCode and CMSS.SessionCode = CMSSS.SessionCode  and CMSP.BranchCode = '" + BranchCode + "'   and CMSP.PackageCode = CMSPC.PackageCode and CMSPC.IsActive = 1 and CMSP.IsActive = 1 and CMSPC.IsDeleted = 0 and CMSP.IsDeleted = 0  group by CMSSWA.MinPrice,CMSSWA.MaxPrice,CMSSS.SlotName,CMSPC.PlanCost,CMSPC.[PlanDuration/Monthly],CMSS.SessionName,CMSS.SessionCode,CMSP.PackageCode,CMSP.PackageName,CMSPC.DurationId,CMSSS.SlotCode,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,CMSSWA.AllocatedCount,CMSSS.SlotStartTime,CMSSS.SlotEndTime";
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
                row["SessionCode"] = ds_custdet1.Tables[0].Rows[j]["SessionCode"].ToString();
                row["PackageCode"] = ds_custdet1.Tables[0].Rows[j]["PackageCode"].ToString();
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

                try
                {
                    row["TrainersCode"] = dt1.Rows[0]["TrainerCode"].ToString();
                    row["TrainersName"] = dt1.Rows[0]["TrainerName"].ToString();
                }
                catch (Exception ec)
                {

                }

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
                row["Date"] = DBNull.Value;
                row["DiscountAmount"] = 0.0f;
                row["DiscountPercentage"] = 0.0f;
                row["PerMonthPrice"] = 0.0f;
                row["DurationNo"] = ds_custdet1.Tables[0].Rows[j][6].ToString();


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
            string query1 = "select SNO=1,Name='Regular',CCRMM.SlotCode,SG.RegularMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots,CCRMM.PackageCode from CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where CCRMM.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID and CCRMM.BranchCode = '" + BranchCode + "'  and CCRMM.IsActive = 1 and CCRMM.StartDate <= Dateadd(DAY, Datediff(DAY, -1, '" + StartDate + "'), 0) and  CCRMM.MembershipExpireDate >= Dateadd(DAY, Datediff(DAY, -1,'" + StartDate + "'), 0) group by CCRMM.SlotCode,SG.RegularMembersPerSlot,CCRMM.PackageCode ";
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

        //
        // Function : GetOffLineFreeTrialSlots  same like GetAllSlots in SlotController
        // FreeTrial Slots

        public DataTable GetAllFreeTrialSlotsByPackageCodeAvailableAUTOPRICEDefault(string BranchCode, string PackageCode)
        {
            DataSet ds_custdet1 = new DataSet();
            DataTable dt_Slotav = new DataTable();
            string result = string.Empty;

            int l = 1;

            DataRow row;
            DataColumn col1 = new DataColumn("SNO", typeof(int));
            DataColumn col2 = new DataColumn("SessionCode", typeof(string));
            DataColumn col3 = new DataColumn("SlotCode", typeof(string));
            DataColumn col4 = new DataColumn("SlotStartTime", typeof(string));
            DataColumn col5 = new DataColumn("SlotEndTime", typeof(string));
            DataColumn col6 = new DataColumn("FreeTrialMembersPerSlot", typeof(int));
            DataColumn col7 = new DataColumn("IsFreeTrialSlotAvailable", typeof(int));
            DataColumn col8 = new DataColumn("Date", typeof(DateTime));



            dt_Slotav.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8 });
            string query1 = "";
            if (PackageCode == "P0")
            {
                query1 = "select CMSSS.SlotCode,CMSSS.SessionCode,SG.FreeMembersPerSlot,PG.PackageWiseSlot as RegularMembersPerSlot,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime  from CMSSLOTTIMESETTING CMSSS,SlotGroups SG,PackageGroup PG  where   CMSSS.SLTGID=PG.SLTGID and CMSSS.SLTGID=SG.SLTGID and PG.SLTGID=SG.SLTGID   and CMSSS.SLTGID = SG.SLTGID  and SG.BranchCode = '" + BranchCode + "'  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSSS.SlotCode,PG.PackageWiseSlot,CMSSS.SlotStartTime,CMSSS.SlotEndTime,CMSSS.SessionCode";
            }
            else
            {
                query1 = "select CMSSS.SlotCode,CMSSS.SessionCode,FreeMembersPerSlot=1,CMSSWA.FreeTrialAllocatedCount as  FreeTriaInUsePerSlot ,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING CMSSS,CMSSlotWiseAllocation CMSSWA,CCRMEnquireFreeTrial CCRMEFT where CMSSS.SlotCode=CCRMEFT.SlotCode and  CCRMEFT.PackageCode ='" + PackageCode + "'   and CMSSWA.SlotCode=CCRMEFT.SlotCode and CMSSWA.BranchCode='" + BranchCode + "'   group by CMSSS.SlotName,CMSSS.SlotCode,CMSSWA.FreeTrialAllocatedCount,CMSSS.SlotStartTime,CMSSS.SlotEndTime,CMSSS.SessionCode  union all  select distinct SlotCode,SessionCode,FreeMembersPerSlot=0,FreeTriaInUsePerSlot=1,convert(varchar(10),SlotStartTime, 108) as SlotStartTime,convert(varchar(10),SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING  where SlotCode not in(select distinct CMSSS.SlotCode from CMSSLOTTIMESETTING CMSSS,CMSSlotWiseAllocation CMSSWA,CCRMEnquireFreeTrial CCRMEFT where CMSSS.SlotCode=CCRMEFT.SlotCode and  CCRMEFT.PackageCode ='" + PackageCode + "'   and CMSSWA.SlotCode=CCRMEFT.SlotCode and CMSSWA.BranchCode='" + BranchCode + "')";
            }

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            for (int j = 0; j <= ds_custdet1.Tables[0].Rows.Count - 1; j++)
            {
                row = dt_Slotav.NewRow();
                row["SNO"] = l;
                row["SlotCode"] = ds_custdet1.Tables[0].Rows[j]["SlotCode"].ToString();
                row["SessionCode"] = ds_custdet1.Tables[0].Rows[j]["SessionCode"].ToString();
                row["SlotStartTime"] = 0.0f;
                row["SlotEndTime"] = 0.0f;
                row["FreeTrialMembersPerSlot"] = ds_custdet1.Tables[0].Rows[j]["FreeTriaInUsePerSlot"].ToString();
                row["IsFreeTrialSlotAvailable"] = "0";
                row["Date"] = DBNull.Value;
                dt_Slotav.Rows.Add(row);
                l = l + 1;
            }

            return dt_Slotav;
        }
        public DataTable GetAllFreeTrialSlotsByPackageCodeAvailableAUTOPRICE(string BranchCode, string PackageCode,DateTime StartDate)
        {
            DataTable dt1 = new DataTable();
            DataSet ds_custdet1 = new DataSet();
            DataTable dt_Slotav = new DataTable();
            string result = string.Empty;
            var dateNow = DateTime.Now.ToString("HH:mm:ss");
            var time1 = TimeSpan.Parse(dateNow);
            var dateTime1 = DateTime.Today.Add(time1);
            var SlotTime = "";

            int l = 1;
            DataRow row;
            DataColumn col1 = new DataColumn("SNO", typeof(int));
            DataColumn col2 = new DataColumn("SlotCode", typeof(string));
            DataColumn col3 = new DataColumn("SessionCode", typeof(string));
            DataColumn col4 = new DataColumn("SlotStartTime", typeof(string));
            DataColumn col5 = new DataColumn("SlotEndTime", typeof(string));
            DataColumn col6 = new DataColumn("FreeTrialMembersPerSlot", typeof(int));
            DataColumn col7 = new DataColumn("IsFreeTrialSlotAvailable", typeof(int));
            DataColumn col8 = new DataColumn("Date", typeof(DateTime));

            dt_Slotav.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8 });
            string query1 = "";
            if (PackageCode == "P0")
            {

                query1 = "select CMSSS.SlotCode,CMSSS.SessionCode,SG.FreeMembersPerSlot,PG.PackageWiseSlot as RegularMembersPerSlot,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime  from CMSSLOTTIMESETTING CMSSS,SlotGroups SG,PackageGroup PG  where   CMSSS.SLTGID=PG.SLTGID and CMSSS.SLTGID=SG.SLTGID and PG.SLTGID=SG.SLTGID   and CMSSS.SLTGID = SG.SLTGID  and SG.BranchCode = '" + BranchCode + "'  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSSS.SlotCode,PG.PackageWiseSlot,CMSSS.SlotStartTime,CMSSS.SlotEndTime,CMSSS.SessionCode";
            }
            else
            {

                query1 = "select CMSSS.SlotCode,CMSSS.SessionCode,FreeMembersPerSlot=1,CMSSWA.FreeTrialAllocatedCount as  FreeTriaInUsePerSlot ,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING CMSSS,CMSSlotWiseAllocation CMSSWA,CCRMEnquireFreeTrial CCRMEFT where CMSSS.SlotCode=CCRMEFT.SlotCode and  CCRMEFT.PackageCode ='" + PackageCode + "'   and CMSSWA.SlotCode=CCRMEFT.SlotCode and CMSSWA.BranchCode='" + BranchCode + "'   group by CMSSS.SlotName,CMSSS.SlotCode,CMSSWA.FreeTrialAllocatedCount,CMSSS.SlotStartTime,CMSSS.SlotEndTime,CMSSS.SessionCode  union all  select distinct SlotCode,SessionCode,FreeMembersPerSlot=0,FreeTriaInUsePerSlot=1,convert(varchar(10),SlotStartTime, 108) as SlotStartTime,convert(varchar(10),SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING  where SlotCode not in(select distinct CMSSS.SlotCode from CMSSLOTTIMESETTING CMSSS,CMSSlotWiseAllocation CMSSWA,CCRMEnquireFreeTrial CCRMEFT where CMSSS.SlotCode=CCRMEFT.SlotCode and  CCRMEFT.PackageCode ='" + PackageCode + "'   and CMSSWA.SlotCode=CCRMEFT.SlotCode and CMSSWA.BranchCode='" + BranchCode + "')";
            }

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            for (int j = 0; j <= ds_custdet1.Tables[0].Rows.Count - 1; j++)
            {

                row = dt_Slotav.NewRow();

                row["SNO"] = l;
                row["SlotCode"] = ds_custdet1.Tables[0].Rows[j]["SlotCode"].ToString();
                row["SessionCode"] = ds_custdet1.Tables[0].Rows[j]["SessionCode"].ToString();
                row["SlotStartTime"] = ds_custdet1.Tables[0].Rows[j]["SlotStartTime"].ToString();
                row["SlotEndTime"] = ds_custdet1.Tables[0].Rows[j]["SlotEndTime"].ToString();
                row["FreeTrialMembersPerSlot"] = ds_custdet1.Tables[0].Rows[j]["FreeTriaInUsePerSlot"].ToString();
                row["Date"] = DBNull.Value;

                SlotTime = ds_custdet1.Tables[0].Rows[j]["SlotStartTime"].ToString();
                var time2 = TimeSpan.Parse(SlotTime);
                // var dateTime2 = DateTime.Today.Add(time2);
                var dateTime2 = StartDate.Add(time2);

                if (dateTime1 > dateTime2)
                {
                    row["IsFreeTrialSlotAvailable"] = "0";
                    //try { ds_custdet1.Tables[0].Rows[j].SetField("IsAvailable", 0); } catch (Exception ec) { }
                    //ds_custdet1.Tables[0].AcceptChanges();
                }
                else
                {
                    row["IsFreeTrialSlotAvailable"] = "1";
                }


                //if (Convert.ToInt32(ds_custdet1.Tables[0].Rows[j]["FreeMembersPerSlot"].ToString()) == 0)
                //    row["IsFreeTrialSlotAvailable"] = "1";
                //else
                //    row["IsFreeTrialSlotAvailable"] = "0";

                dt_Slotav.Rows.Add(row);
                l = l + 1;
            }

            return dt_Slotav;
        }
        public DataTable GetAllFreeTrialSlotsByPackageCodeUsedAUTOPRICE(string BranchCode, string PackageCode, string StartDate)
        {
            DataSet ds_custdet1 = new DataSet();
            DataTable dt_Slotav = new DataTable();

            string query1 = "select SNO=1,Name='Regular',CCRMM.SlotCode,SG.RegularMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots,CCRMM.PackageCode from CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where CCRMM.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID and CCRMM.BranchCode = '" + BranchCode + "'  and CCRMM.IsActive = 1 and CCRMM.StartDate <= Dateadd(DAY, Datediff(DAY, -1, '" + StartDate + "'), 0) and  CCRMM.MembershipExpireDate >= Dateadd(DAY, Datediff(DAY, -1,'" + StartDate + "'), 0) group by CCRMM.SlotCode,SG.RegularMembersPerSlot,CCRMM.PackageCode ";


            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            dt_Slotav = ds_custdet1.Tables[0];
            return dt_Slotav;
        }
        public string GetOffLineFreeTrialSlots([FromBody]OffLineCMSSlotsAvailability PackagePrices)
        {
            string sJSONResponse = "";

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_slots = new DataSet();
            string result = string.Empty;

            int AvailableSlots = 0;
            int FilledSlots = 0;
            int EnquiretypeNo = 0;
            string UniqueId = "";

            DataTable dt_IsPackage = new DataTable();
            string MobileNo = PackagePrices.MobileNo;
            string MembershipCode = PackagePrices.MembershipCode;

            DataTable dt_DailyPricePercentage = getdata(string.Format("select Days,DaysDuration,Percentage from DailyPricePercentage", ""));
            DataTable dt_Slotavailable = new DataTable();
            DataTable dt_Slotused = new DataTable();
            DataTable dt_Slot = new DataTable();

            string BranchCode = PackagePrices.BranchCode;
            string PackageCode = PackagePrices.PackageCode;

            ArrayList arl_Mobile = MobileCheck(PackagePrices.MobileNo);
            int EnquireformCount = Convert.ToInt32(arl_Mobile[0].ToString());
            int UserCount = Convert.ToInt32(arl_Mobile[1].ToString());

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

            //CreateDatatable
            DataTable dt_temptable = new DataTable();

            // days calculation

            DateTime currentday = DateTime.Now;
            DateTime firstDayThisMonth = new DateTime(currentday.Year, currentday.Month, 1);
            DateTime firstDayPlusTwoMonths = firstDayThisMonth.AddMonths(2);
            DateTime lastDayNextMonth = firstDayPlusTwoMonths.AddDays(-1);
            DateTime endOfLastDayNextMonth = firstDayPlusTwoMonths.AddTicks(-1);

            dt_Slotavailable = GetAllFreeTrialSlotsByPackageCodeAvailableAUTOPRICE(PackagePrices.BranchCode, PackagePrices.PackageCode,Convert.ToDateTime(PackagePrices.StartDate));

            //foreach (DataRow rrow in dt_Slotavailable.Rows)
            //{

            //    rrow["Date"] = PackagePrices.StartDate;
            //    AvailableSlots = Convert.ToInt32(rrow["FreeTrialMembersPerSlot"]);
            //    FilledSlots = Convert.ToInt32(rrow["IsFreeTrialSlotAvailable"].ToString());

            //    if (FilledSlots < AvailableSlots)
            //        rrow["IsFreeTrialSlotAvailable"] = 1;
            //    else if (FilledSlots == AvailableSlots)
            //        rrow["IsFreeTrialSlotAvailable"] = 0;
            //    else
            //        rrow["IsFreeTrialSlotAvailable"] = 0;
            //}


            dt_temptable.Merge(dt_Slotavailable);

            int sloutcount = dt_temptable.Rows.Count;

            DataTable st_newSlotavailable = new DataTable();
            st_newSlotavailable = dt_temptable.DefaultView.ToTable();
            DataTable dt_FilteredSlotsAvailable = new DataTable();

            List<OffLineEnquireInfo> olei = new List<OffLineEnquireInfo>();
            olei.Add(new OffLineEnquireInfo { isFreeTrial = EnquiretypeNo, enquireTypeNo = 2 });

            DataTable dt_Sessions = getdata(string.Format("select distinct CMSSS.SessionCode,CMSSS.SessionName,convert(varchar(10),CMSSS.SessionStartTime, 108) as SessionStartTime,convert(varchar(10),  CMSSS.SessionEndTime, 108) as SessionEndTime  from  CMSSESSIONTIMESETTING CMSSS  order by SessionCode asc", ""));
            List<OffLineFreeTrialSessions> OnlineFreeTrialSessions = new List<OffLineFreeTrialSessions>();

            List<OffLineFreeTrialOnlySlotDetails> dt_SlotPackageDopt = new List<OffLineFreeTrialOnlySlotDetails>();

            for (int i = 0; i < dt_Sessions.Rows.Count - 1; i++)
            {

                OffLineFreeTrialOnlySlotDetails dashboardlistpackage = new OffLineFreeTrialOnlySlotDetails { sessionId = dt_Sessions.Rows[i]["SessionCode"].ToString(), sessionName = dt_Sessions.Rows[i]["SessionName"].ToString(), sessionStartTime = dt_Sessions.Rows[i]["SessionStartTime"].ToString(), sessionEndTime = dt_Sessions.Rows[i]["SessionEndTime"].ToString(), info = olei, slots = GetOnlyFreeTrialSlotsDetails(st_newSlotavailable, PackagePrices.StartDate, BranchCode, dt_Sessions.Rows[i]["SessionCode"].ToString()) };
                dt_SlotPackageDopt.Add(dashboardlistpackage);

            }

            OffLineFreeTrialOnlySlotOutPut olppFOP = new OffLineFreeTrialOnlySlotOutPut();

            olppFOP.status = "Success";
            olppFOP.value = dt_SlotPackageDopt;

            sJSONResponse = JsonConvert.SerializeObject(olppFOP);


            return sJSONResponse;

        }
        // slotCode to slotId  :: isSlotAvailable to isAvailable Boolan
        public List<OffLineFreeTrialOnlySlots> GetOnlyFreeTrialSlotsDetails(DataTable dt_Slot, string Date, string BranchCode, string SessionCode)
        {
            DataTable dt_temptable = new DataTable();
            DataTable dt_filter = new DataTable();

            DataRow[] rows = dt_Slot.Select("SessionCode = '" + SessionCode + "' ");
            List<OffLineFreeTrialOnlySlots> offLineSlot = new List<OffLineFreeTrialOnlySlots>();

            try
            {
                dt_filter = rows.CopyToDataTable();

                for (int i = 0; i < dt_filter.Rows.Count; i++)
                {
                    offLineSlot.Add(new OffLineFreeTrialOnlySlots
                    {
                        sNo = Convert.ToInt32(dt_filter.Rows[i]["SNO"])
                                           ,
                        slotId = Convert.ToString(dt_filter.Rows[i]["SlotCode"])
                                            ,
                        isAvailable = Convert.ToBoolean(dt_filter.Rows[i]["IsFreeTrialSlotAvailable"])
                                            ,
                        slotStartTime = Convert.ToString(dt_filter.Rows[i]["SlotStartTime"])
                                            ,
                        slotEndTime = Convert.ToString(dt_filter.Rows[i]["SlotEndTime"])
                    });
                }
            }
            catch (Exception ec)
            {

            }
            return offLineSlot;
        }
        public List<OffLineFreeTrialSlotsPackage> GetFreeTrialSlotsDetails(DataTable dt_SlotPackages, string SessionCode, string BranchCode)
        {
            List<OffLineFreeTrialSlotsPackage> offLineFreeTrialSlotPackage = new List<OffLineFreeTrialSlotsPackage>();
            DataTable temp = new DataTable();
            try
            {
                DataTable dt_Slot = getdata(string.Format("select distinct SlotCode,SlotName from TrainerSlotSpecializationMapping where BranchCode='{0}' and SessionCode='{1}'", BranchCode, SessionCode));
                DataRow[] results = dt_SlotPackages.Select("SessionCode = '" + SessionCode + "' ");
                dt_SlotPackages = results.CopyToDataTable();

                for (int i = 0; i < dt_SlotPackages.Rows.Count; i++)
                {
                    DataRow[] slotname = dt_Slot.Select("SlotCode = '" + dt_SlotPackages.Rows[i]["SlotCode"].ToString() + "' ");
                    temp = slotname.CopyToDataTable();
                    offLineFreeTrialSlotPackage.Add(new OffLineFreeTrialSlotsPackage
                    {
                        slotID = Convert.ToString(dt_SlotPackages.Rows[i]["SlotCode"])
                         ,
                        slotName = Convert.ToString(temp.Rows[0]["SlotName"].ToString())
                        ,
                        isAvailable = Convert.ToString(dt_SlotPackages.Rows[i]["IsFreeTrialSlotAvailable"])
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

            return offLineFreeTrialSlotPackage;
        }

        // Dynamic Pricing OfflineDynamicPricingWithSessions (old) #3
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


                                    //new calculation Logics 10/21/2020
                                    //SlotCapacity = (FilledSlots/AvailableSlots)*100;
                                    //if(either 25/50/75/100)
                                    //formala 
                                    //SlotPrice= minimumprice+(minimumprice*(perventage get by DailyPricePercentage/100))



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
        public Object GetPackageDescription()
        {

            DataTable dt_IsPackage = new DataTable();
            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();

            PackageDescriptionOutput ppopt = new PackageDescriptionOutput();
            try
            {

                DataTable dt_packages = getdata(string.Format("select distinct CMSPD.PackageCode,CMSP.PackageName from CMSPACKAGESDESCRIPTION CMSPD, CMSPACKAGES CMSP where CMSPD.BranchCode = '1102'and CMSPD.PackageCode = CMSP.PackageCode  and CMSPD.IsDeleted = 0", ""));
                DataTable dt_packageDescriptionDetails = getdata(string.Format("select CMSPD.ID,CMSPD.PackageDescCode,CMSPD.PackageCode,CMSP.PackageName,CMSPD.PackageDescription  from CMSPACKAGESDESCRIPTION CMSPD,CMSPACKAGES CMSP where CMSPD.BranchCode='1102'and CMSPD.PackageCode=CMSP.PackageCode  and CMSPD.IsDeleted=0 order by ID asc", ""));
                List<PackageDescriptionList> packageDescriptionLists = new List<PackageDescriptionList>();

                for (int i = 0; i <= dt_packages.Rows.Count - 1; i++)
                {
                    PackageDescriptionList packageDescriptionList = new PackageDescriptionList { packageCode = dt_packages.Rows[i]["PackageCode"].ToString(), packageName = dt_packages.Rows[i]["PackageName"].ToString(), packageDescriptionDetails = GetPackageDescriptionDetails(dt_packageDescriptionDetails, dt_packages.Rows[i]["PackageCode"].ToString()) };
                    packageDescriptionLists.Add(packageDescriptionList);
                }

                ppopt.status = "success";
                ppopt.value = packageDescriptionLists;
                sJSONResponse = JsonConvert.SerializeObject(ppopt);

            }
            catch (Exception ec)
            {
                ppopt.status = "success";
                sJSONResponse = JsonConvert.SerializeObject(ppopt);

            }

            return sJSONResponse;

        }
        public List<PackageDescriptionDetails> GetPackageDescriptionDetails(DataTable PackageDescriptionDetails, string PackageCode)
        {
            List<PackageDescriptionDetails> PackageDescriptionDetail = new List<PackageDescriptionDetails>();
            DataRow[] rows = PackageDescriptionDetails.Select("PackageCode = '" + PackageCode + "' ");

            if (rows.Length > 0)
            {
                PackageDescriptionDetails = rows.CopyToDataTable();
            }

            for (int i = 0; i <= PackageDescriptionDetails.Rows.Count - 1; i++)
            {
                PackageDescriptionDetail.Add(new PackageDescriptionDetails { PackageDescCode = PackageDescriptionDetails.Rows[i]["PackageDescCode"].ToString(), PackageDescription = PackageDescriptionDetails.Rows[i]["PackageDescription"].ToString() });
            }

            return PackageDescriptionDetail;
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
        public int ReceiptGeneration()
        {
            if (cnn.State == ConnectionState.Open)
            {
                cnn.Close();
            }

            int status = 0;
            string query1 = "select MAX(Receipt) from FAInvoice";
            try
            {
                using (SqlCommand cmd_RecGen = new SqlCommand(query1, cnn))
                {
                    cnn.Open();
                    SqlDataReader DR_RecGen = cmd_RecGen.ExecuteReader();
                    if (DR_RecGen.Read())
                    {
                        status = Convert.ToInt32(DR_RecGen[0].ToString());
                    }
                    cnn.Close();
                }

                if (status == 0)
                {
                    status = 1;
                }
                else
                {
                    status = status + 1;
                }

            }
            catch (Exception ec)
            {
                status = 1;
            }
            finally
            {

                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }
            return status;
        }
        public int SendMessage(string MobileNo, string UserName)
        {

            try
            {
                DataSet ds_custdet = new DataSet();
                DataTable dt_CRM = new DataTable();
                string SendText = string.Empty;


                string httpUrl = string.Empty;
                {
                    SendText = "Hello '" + UserName + "' , Thank you for completing your online payment. Have a great day! NarenFitness.";
                    StreamReader objReader;
                    //httpUrl = "http://sms.smscity.in/httpapi/httpapi?token=8c84bb208c7c7b7b72660ef51a5430b0&sender=NRNGYM&number=0" + MobileNo + "&route=2&type=1&sms='" + SendText + "' ";
                    //httpUrl = "http://api.textlocal.in/send?username=karthikeyaenator@gmail.com&hash=f94d56486adbd53db4d28bcc515c415ef49267fc&numbers=" + MobileNo + "&message=" + SendText + "&sender=SUCARD ";
                    httpUrl = "http://api.textlocal.in/send?username=dilipgoud@narenfitness.com&hash=4a7076b97161c3a3f6c14028abebf7589808c1ec3ed68eaae63c44e0db3f6497&numbers=" + MobileNo + "&message=" + SendText + "&sender=NRNGYM ";

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
        public ArrayList GetGSTCode()
        {
            //-- GYMFEES','Trainer Fees','IGSTableAmount','IGST','CGST','SGST
            //IGSTtableamount - IGST
            //payableamount* Personaltrainer%
            //payableamount * Gymfee %
            //IGSTtableamount * IGST
            //IGSTtableamount* CGST
            //IGSTtableamount * SGST

            ArrayList arl_gst = new ArrayList();
            string GSTCode = "";
            int IGSTPercentage = 0;
            int CGSTPercentage = 0;
            int SGSTPercentage = 0;
            int GYMFeePercentage = 0;
            int PersonalTrainerPercentage = 0;

            string query = "select GSTCode,IGSTPercentage,CGSTPercentage,SGSTPercentage,GYMFeePercentage,PersonalTrainerPercentage from FAGST ";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_GST = cmd_SubEXT.ExecuteReader();
                if (DR_GST.Read())
                {
                    GSTCode = DR_GST[0].ToString();
                    IGSTPercentage = Convert.ToInt32(DR_GST[1].ToString());
                    CGSTPercentage = Convert.ToInt32(DR_GST[2].ToString());
                    SGSTPercentage = Convert.ToInt32(DR_GST[3].ToString());
                    GYMFeePercentage = Convert.ToInt32(DR_GST[4].ToString());
                    PersonalTrainerPercentage = Convert.ToInt32(DR_GST[5].ToString());
                }
                cnn.Close();
            }

            arl_gst.Add(GSTCode);
            arl_gst.Add(IGSTPercentage);
            arl_gst.Add(CGSTPercentage);
            arl_gst.Add(SGSTPercentage);
            arl_gst.Add(GYMFeePercentage);
            arl_gst.Add(PersonalTrainerPercentage);

            return arl_gst;

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
        public ArrayList MSerialCheck(string BranchCode)
        {
            int Year = DateTime.Now.Year - 2000;
            int Month = DateTime.Now.Month;


            if (cnn.State == ConnectionState.Open)
            {
                cnn.Close();
            }
            ArrayList al = new ArrayList();
            int SerialNo = 0;
            string SCode = string.Empty;
            string Codification = string.Empty;
            int Count = 0;
            string query1 = "select Max(SerialNo),COUNT(*) as SerialNo from CCRMMembershipCodification where BranchCode='" + BranchCode + "' and Year=" + Year + "  and Month=" + Month + " ";
            try
            {
                using (SqlCommand cmd_RecGen = new SqlCommand(query1, cnn))
                {
                    cnn.Open();
                    SqlDataReader DR_RecGen = cmd_RecGen.ExecuteReader();
                    if (DR_RecGen.Read())
                    {
                        try
                        {
                            SerialNo = Convert.ToInt32(DR_RecGen[0].ToString());
                            Count = Convert.ToInt32(DR_RecGen[0].ToString());
                        }
                        catch (Exception ec)
                        {
                            Count = 0;

                        }
                    }

                    cnn.Close();
                }

                if (Count == 0)
                {
                    SerialNo = 1;
                    SCode = "001";
                }
                else
                {
                    SerialNo = SerialNo + 1;

                    if (SerialNo > 1 && SerialNo < 10)
                        SCode = "00" + Convert.ToString(SerialNo);
                    else if (SerialNo > 9 && SerialNo < 100)
                        SCode = "0" + Convert.ToString(SerialNo);
                    else if (SerialNo > 99 && SerialNo < 1000)
                        SCode = Convert.ToString(SerialNo);
                    else
                        SCode = "00000";
                }

            }
            catch (Exception ec)
            {
                SerialNo = 0;
            }
            finally
            {

                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }

            if (Month < 10)
            {
                Codification = Convert.ToString(Year) + "0" + Convert.ToString(Month) + SCode;
            }
            else
            {
                Codification = Convert.ToString(Year) + Convert.ToString(Month) + SCode;
            }


            al.Add(SCode);
            al.Add(Codification);
            return al;
        }
        public ArrayList MCode(string BranchCode)
        {
            ArrayList al = new ArrayList();
            string MNCode = string.Empty;
            String BCode = BranchCode.Substring(1, BranchCode.Length - 1);
            ArrayList SerialNo = MSerialCheck(BranchCode);
            MNCode = SerialNo[1].ToString() + BCode;
            al.Add(SerialNo[0].ToString());
            al.Add(MNCode);

            return al;
        }

        // add opts

        //7
        public Object Renewal([FromBody]RenewalPostInput Renewal)
        {
            string r = "";
            DataSet ds_PaymentDetailsPost = new DataSet();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            ArrayList arl_pdetails = GetPersonDetailsByMobileNo(Renewal.MobileNo);

            int Year = DateTime.Now.Year - 2000;
            int Month = DateTime.Now.Month;
            //-- GYMFEES','Trainer Fees','IGSTableAmount','IGST','CGST','SGST
            //IGSTtableamount - IGST
            //payableamount* Personaltrainer%
            //payableamount * Gymfee %
            //IGSTtableamount * IGST
            //IGSTtableamount* CGST
            //IGSTtableamount * SGST

            ArrayList ary_GST = new ArrayList();
            ary_GST = GetGSTCode();
            // Clients Unique COde
            ArrayList al = new ArrayList();
            string GSTCode = "";
            try
            {
                GSTCode = ary_GST[0].ToString();
            }
            catch (Exception ec)
            {
            }

            int IGSTPercentage = Convert.ToInt32(ary_GST[1].ToString());
            int CGSTPercentage = Convert.ToInt32(ary_GST[2].ToString());
            int SGSTPercentage = Convert.ToInt32(ary_GST[3].ToString());
            int GYMFeePercentage = Convert.ToInt32(ary_GST[4].ToString());
            int PersonalTrainerPercentage = Convert.ToInt32(ary_GST[5].ToString());

            float PayableAmount = Convert.ToInt32(Renewal.PayableAmount);

            double PersonalTrainer = PayableAmount * (PersonalTrainerPercentage / 100);
            double IGSTableAmount = PayableAmount * (GYMFeePercentage / 100);
            double IGST = PayableAmount * (IGSTPercentage / 100);
            double CGST = PayableAmount * (CGSTPercentage / 100);
            double SGST = PayableAmount * (SGSTPercentage / 100);
            double GYMFEE = IGSTableAmount - IGST;

            List<PostRenewalUtlizationValueOutput> otpdetails = new List<PostRenewalUtlizationValueOutput>();
            PostRenewalUtlizationOutput ppuo = new PostRenewalUtlizationOutput();

            string MembershipCode = GetMembersExistance(Renewal.MobileNo);

            if (MembershipCode == null || MembershipCode == "" || MembershipCode == string.Empty)
            {
                al = MCode(Renewal.BranchCode);
                r = (String)al[1].ToString();
            }
            else
            {
                r = MembershipCode;
            }
            // Invoice Generation
            string Invoice = "N" + UniqueGeneration2();
            int UID = UIDNew(Renewal.BranchCode);

            if (InvoiceCheck(Invoice) != 0)
            {
                Invoice = "N" + UniqueGeneration2();
            }

            int receipt = ReceiptGeneration();
            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            // Start a local transaction.
            transaction = cnn.BeginTransaction("SampleTransaction");
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = cnn;
            command.Transaction = transaction;


            try
            {

                command.CommandText = "insert into CCRMMembership(MembershipCode,BranchCode,PlanCode,MembershipExpireDate,CreatedBy,CreatedOn,IsDeleted,IsActive,SlotCode,PackageCode,StartDate,TrainerCode,InvoiceID,Receipt,SerialNo,Year,Month,PlanCostCode,DurationId,EnquireTypeNo,EnquireTypeIncentives) values('" + Renewal.MembershipCode + "','" + Renewal.BranchCode + "','" + Renewal.PlanCode + "','" + Renewal.MembershipExpireDate + "','" + Renewal.CreatedBy + "','" + ServerDateTime + "',0,1,'" + Renewal.SlotCode + "','" + Renewal.PackageCode + "','" + Renewal.MembershipStartDate + "','" + Renewal.TrainerCode + "','" + Renewal.Invoice + "','" + Renewal.receipt + "',''," + Month + "," + Year + ",'" + Renewal.EnquireTypeNo + "','" + Renewal.DurationId + "','" + Renewal.EnquireTypeNo + "','')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FAInvoice(InvoiceID,GSTCode,FAPaymentModes,MembershipCode,PayableAmount,AmountDue,GymFees,TrainerFees,IGSTableAmount,IGST,CGST,SGST,DueDate,CreatedBy,CreatedOn,IsDeleted,IsActive,FinalAmount,FAPaymentModes2,PayableAmount2,PromoCode,DiscountAmount,TrainerCode,Receipt,EnquireTypeNo,SlotPrice,PlanCost,PaymentDate,RemainingAmount,Wallet,Recouncelled,Matchedid) values('" + Invoice + "','" + GSTCode + "','" + Renewal.ModeOfPayment + "','" + Renewal.MembershipCode + "','" + PayableAmount + "','" + Renewal.AmountDue + "','" + GYMFEE + "','" + PersonalTrainer + "','" + IGSTableAmount + "','" + IGST + "','" + CGST + "','" + SGST + "','" + Renewal.DueDate + "','" + Renewal.CreatedBy + "','" + ServerDateTime + "',0,1,'" + Renewal.FinalAmount + "','','','" + Renewal.PromoCode + "','" + Renewal.DiscountAmount + "','" + Renewal.TrainerCode + "','" + receipt + "','" + Renewal.EnquireTypeNo + "','" + Renewal.SlotPrice + "','" + Renewal.PlanCost + "','" + Renewal.PaymentDate + "','" + Renewal.RemainingAmount + "','" + Renewal.Wallet + "','','')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMEnquireStatus(MobileNo,EnquireTypeNo,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + Renewal.MobileNo + "','" + Renewal.EnquireTypeNo + "','" + Renewal.CreatedBy + "','" + ServerDateTime + "',1,0)";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',1,'FREEZING','" + Renewal.Freezing + "','" + Renewal.Invoice + "','" + Renewal.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',3,'UPGRADATION','" + Renewal.Upgrade + "','" + Renewal.Invoice + "','" + Renewal.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',4,'CHANGE','" + Renewal.Change + "','" + Renewal.Invoice + "','" + Renewal.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',5,'TRANSFER TRANSFER','" + Renewal.Transfer + "','" + Renewal.Invoice + "','" + Renewal.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',6,'PERSON TRANSFER','" + Renewal.Transfer + "','" + Renewal.Invoice + "','" + Renewal.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',8,'HOLD','" + Renewal.Transfer + "','" + Renewal.Invoice + "','" + Renewal.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',9,'Paused','" + Renewal.Paused + "','" + Renewal.Invoice + "','" + Renewal.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',10,'Convert','" + Renewal.Convert + "','" + Renewal.Invoice + "','" + Renewal.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();

                if (Renewal.AmountDue != 0)
                {
                    command.CommandText = "insert into Remainders(MembershipCode,RemainderCode,RemainderName,Amount,Date,CreatedBy,CreatedOn,FAInvoice) values('" + Renewal.MembershipCode + "','1001','DUEEXPIREDATE','" + Renewal.Amount + "','" + ServerDateTime + "','" + Renewal.CreatedBy + "','" + ServerDateTime + "','" + Renewal.Invoice + "')";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into Assign(MemberShipCode,AssignCode,MobileNo,EnquireTypeNo,AssignedBy,AssignedTo,NextFollowDate,Description,CreatedBy,CreatedOn,IsDeleted,IsActive,Invoice) values('" + Renewal.MembershipCode + "','2004','" + Renewal.MobileNo + "','" + Renewal.EnquireTypeNo + "','','" + Renewal.CreatedBy + "','" + Renewal.DueDate + "','','" + Renewal.CreatedBy + "','" + ServerDateTime + "',0,1,'" + Renewal.Invoice + "')";
                    command.ExecuteNonQuery();

                }
                if (Renewal.Comment != "")
                {
                    command.CommandText = "insert into CommentsTracker(UCode,MasterID,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + Renewal.MembershipCode + "',17,'" + Renewal.Comment + "','" + Renewal.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                }
                if (Renewal.Wallet != "")
                {
                    command.CommandText = "insert into WalletTransactions(MembershipCode,NewMembershipCode,InvoiceId,TransactionId,TransactionName,MobileNo,Credit,Debit,CreatedOn,CreatedBy,IsDeleted,IsActive) values('" + Renewal.MembershipCode + "','" + Renewal.MembershipCode + "','" + Renewal.Invoice + "'," + Renewal.TransactionId + ",'" + Renewal.TransactionStatus + "','" + Renewal.MobileNo + "','','','" + ServerDateTime + "','" + Renewal.CreatedBy + "',0,1) ";
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                cnn.Close();



                otpdetails.Add(new PostRenewalUtlizationValueOutput { userId = UID, receiptNo = receipt, invoiceNo = Invoice });


                ppuo.status = "success";
                ppuo.value = otpdetails;
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

                ppuo.status = "fail";
                ppuo.value = otpdetails;
            }




            sJSONResponse = JsonConvert.SerializeObject(ppuo);



            return sJSONResponse;
        }

        //add opts

        //6
        public string PreRenewal([FromBody]PreRenewalUtlizationInput PreRenewal)
        {

            string r = "";
            DataSet ds_PaymentDetailsPost = new DataSet();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            ArrayList arl_pdetails = GetPersonDetailsByMobileNo(PreRenewal.MobileNo);

            int Year = DateTime.Now.Year - 2000;
            int Month = DateTime.Now.Month;
            //-- GYMFEES','Trainer Fees','IGSTableAmount','IGST','CGST','SGST
            //IGSTtableamount - IGST
            //payableamount* Personaltrainer%
            //payableamount * Gymfee %
            //IGSTtableamount * IGST
            //IGSTtableamount* CGST
            //IGSTtableamount * SGST

            ArrayList ary_GST = new ArrayList();
            ary_GST = GetGSTCode();
            // Clients Unique COde
            ArrayList al = new ArrayList();
            string GSTCode = "";
            try
            {
                GSTCode = ary_GST[0].ToString();
            }
            catch (Exception ec)
            {
            }

            int IGSTPercentage = Convert.ToInt32(ary_GST[1].ToString());
            int CGSTPercentage = Convert.ToInt32(ary_GST[2].ToString());
            int SGSTPercentage = Convert.ToInt32(ary_GST[3].ToString());
            int GYMFeePercentage = Convert.ToInt32(ary_GST[4].ToString());
            int PersonalTrainerPercentage = Convert.ToInt32(ary_GST[5].ToString());

            float PayableAmount = Convert.ToInt32(PreRenewal.PayableAmount);

            double PersonalTrainer = PayableAmount * (PersonalTrainerPercentage / 100);
            double IGSTableAmount = PayableAmount * (GYMFeePercentage / 100);
            double IGST = PayableAmount * (IGSTPercentage / 100);
            double CGST = PayableAmount * (CGSTPercentage / 100);
            double SGST = PayableAmount * (SGSTPercentage / 100);
            double GYMFEE = IGSTableAmount - IGST;



            string MembershipCode = GetMembersExistance(PreRenewal.MobileNo);

            if (MembershipCode == null || MembershipCode == "" || MembershipCode == string.Empty)
            {
                al = MCode(PreRenewal.BranchCode);
                r = (String)al[1].ToString();
            }
            else
            {
                r = MembershipCode;
            }
            // Invoice Generation
            string Invoice = "N" + UniqueGeneration2();
            int UID = UIDNew(PreRenewal.BranchCode);

            if (InvoiceCheck(Invoice) != 0)
            {
                Invoice = "N" + UniqueGeneration2();
            }

            int receipt = ReceiptGeneration();
            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            // Start a local transaction.
            transaction = cnn.BeginTransaction("SampleTransaction");
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = cnn;
            command.Transaction = transaction;


            try
            {

                command.CommandText = "insert into CCRMMembership(MembershipCode,BranchCode,PlanCode,MembershipExpireDate,CreatedBy,CreatedOn,IsDeleted,IsActive,SlotCode,PackageCode,StartDate,TrainerCode,InvoiceID,Receipt,SerialNo,Year,Month,PlanCostCode,DurationId,EnquireTypeNo,EnquireTypeIncentives) values('" + PreRenewal.MemberShipCode + "','" + PreRenewal.BranchCode + "','" + PreRenewal.PlanCode + "','" + PreRenewal.MembershipExpireDate + "','" + PreRenewal.CreatedBy + "','" + ServerDateTime + "',0,1,'" + PreRenewal.SlotCode + "','" + PreRenewal.PackageCode + "','" + PreRenewal.MembershipStartDate + "','" + PreRenewal.TrainerCode + "','" + PreRenewal.Invoice + "','" + PreRenewal.receipt + "','','','','" + PreRenewal.EnquireTypeNo + "','" + PreRenewal.DurationId + "','" + PreRenewal.EnquireTypeNo + "','')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FAInvoice(InvoiceID,GSTCode,FAPaymentModes,MembershipCode,PayableAmount,AmountDue,GymFees,TrainerFees,IGSTableAmount,IGST,CGST,SGST,DueDate,CreatedBy,CreatedOn,IsDeleted,IsActive,FinalAmount,FAPaymentModes2,PayableAmount2,PromoCode,DiscountAmount,TrainerCode,Receipt,EnquireTypeNo,SlotPrice,PlanCost,PaymentDate,RemainingAmount,Wallet,Recouncelled,Matchedid) values('" + Invoice + "','" + GSTCode + "','" + PreRenewal.ModeOfPayment + "','" + PreRenewal.MemberShipCode + "','" + PayableAmount + "','" + PreRenewal.AmountDue + "','" + GYMFEE + "','" + PersonalTrainer + "','" + IGSTableAmount + "','" + IGST + "','" + CGST + "','" + SGST + "','" + PreRenewal.DueDate + "','" + PreRenewal.CreatedBy + "','" + ServerDateTime + "',0,1,'" + PreRenewal.FinalAmount + "','','','" + PreRenewal.PromoCode + "','" + PreRenewal.DiscountAmount + "','" + PreRenewal.TrainerCode + "','" + receipt + "','" + PreRenewal.EnquireTypeNo + "','" + PreRenewal.SlotPrice + "','" + PreRenewal.PlanCost + "','" + ServerDateTime + "','','','','')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMEnquireStatus(MobileNo,EnquireTypeNo,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + PreRenewal.MobileNo + "','" + PreRenewal.EnquireTypeNo + "','" + PreRenewal.CreatedBy + "','" + ServerDateTime + "',1,0)";
                command.ExecuteNonQuery();
                command.ExecuteNonQuery();
                command.CommandText = "insert into FacilityOpted(MembershipCode,Invoice,Freezing,Change,Upgrade,Transfer,Paused,[Convert],CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + PreRenewal.MembershipCode + "','" + PreRenewal.Invoice + "','" + PreRenewal.Freezing + "','" + PreRenewal.Change + "','" + PreRenewal.Upgrade + "','" + PreRenewal.Transfer + "','" + PreRenewal.Paused + "','" + PreRenewal.Convert + "','" + PreRenewal.CreatedBy + "','" + ServerDateTime + "',0,1)";
                if (PreRenewal.AmountDue != 0)
                {
                    command.CommandText = "insert into Remainders(MembershipCode,RemainderCode,RemainderName,Amount,Date,CreatedBy,CreatedOn,FAInvoice) values('" + PreRenewal.MemberShipCode + "','1001','DUEEXPIREDATE','" + PreRenewal.Amount + "','" + ServerDateTime + "','" + PreRenewal.CreatedBy + "','" + ServerDateTime + "','" + PreRenewal.Invoice + "')";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into Assign(MemberShipCode,AssignCode,MobileNo,EnquireTypeNo,AssignedBy,AssignedTo,NextFollowDate,Description,CreatedBy,CreatedOn,IsDeleted,IsActive,Invoice) values('" + PreRenewal.MemberShipCode + "','2004','" + PreRenewal.MobileNo + "','" + PreRenewal.EnquireTypeNo + "','','','','','" + PreRenewal.CreatedBy + "','" + ServerDateTime + "',0,1,'" + PreRenewal.Invoice + "')";
                    command.ExecuteNonQuery();

                }
                if (PreRenewal.Comment != "")
                {
                    command.CommandText = "insert into CommentsTracker(UCode,MasterID,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + PreRenewal.MemberShipCode + "',17,'" + PreRenewal.Comment + "','" + PreRenewal.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                }
                if (PreRenewal.Wallet != "")
                {
                    command.CommandText = "insert into WalletTransactions(MembershipCode,NewMembershipCode,InvoiceId,TransactionId,TransactionName,MobileNo,Credit,Debit,CreatedOn,CreatedBy,IsDeleted,IsActive) values('" + PreRenewal.MemberShipCode + "','" + PreRenewal.MemberShipCode + "','" + PreRenewal.Invoice + "'," + PreRenewal.TransactionId + ",'" + PreRenewal.TransactionStatus + "','" + PreRenewal.MobileNo + "','','','" + ServerDateTime + "','" + PreRenewal.CreatedBy + "',0,1) ";
                    command.ExecuteNonQuery();
                }


                transaction.Commit();
                cnn.Close();

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


            List<PostRenewalUtlizationValueOutput> otpdetails = new List<PostRenewalUtlizationValueOutput>();
            otpdetails.Add(new PostRenewalUtlizationValueOutput { userId = UID, receiptNo = receipt, invoiceNo = Invoice });

            PostRenewalUtlizationOutput ppuo = new PostRenewalUtlizationOutput();
            ppuo.status = "success";
            ppuo.value = otpdetails;

            sJSONResponse = JsonConvert.SerializeObject(ppuo);



            return sJSONResponse;
        }

        // Package Details

        public Object GetPackages([FromBody]PackageDetailsInput Packages)
        {

            DataTable dt_IsPackage = new DataTable();
            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();

            PackageDetailsOutput ppopt = new PackageDetailsOutput();
            try
            {

                dt_IsPackage = getdata(string.Format("select PackageCode,PackageName from CMSPACKAGES where BranchCode='{0}'and IsDeleted=0 and IsActive=1 ", Packages.BranchCode));

                List<PackageDetails> dbPackage = new List<PackageDetails>();
                for (int i = 0; i <= dt_IsPackage.Rows.Count - 1; i++)
                {
                    PackageDetails dashboardlistpackage = new PackageDetails { packageCode = dt_IsPackage.Rows[i][0].ToString(), packageName = dt_IsPackage.Rows[i][1].ToString() };
                    dbPackage.Add(dashboardlistpackage);
                }


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

        //101
        // Dynamic Pricing
        //28/08/2020: Need to add request Memebershipcode & MobileNo and check the enquirytype and update isfreetrail (0,1)

        // Dynamic Pricing OfflineDynamicPricing #1
        public string GetOfflineDynamicPricing([FromBody]OffLineCMSSlotsAvailability PackagePrices)
        {
            string sJSONResponse = "";
            var Percentage = "";
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_slots = new DataSet();
            string result = string.Empty;


            double x = 0.0f;
            double y = 0.0f;
            double z = 0.0f;

            int AvailableSlots = 0;
            int FilledSlots = 0;
            float PlanCost = 0.0f;
            double SlotPrice = 0.0f;
            double SlotCapacity = 0.0f;
            float MinimumAmount = 0.0f;
            float MaximumAmount = 0.0f;
            float Constant = 0.0f;
            double capacityPercentage = 0.0f;



            DataTable dt_IsPackage = new DataTable();
            string MobileNo = PackagePrices.MobileNo;
            string MembershipCode = PackagePrices.MembershipCode;

            int EnquiretypeNo = 0;

            DataTable dt_DailyPricePercentage = getdata(string.Format("select Days,DaysDuration,Percentage from DailyPricePercentage", ""));

            DataTable dt_Slotavailable = new DataTable();
            DataTable dt_Slotused = new DataTable();
            DataTable dt_Slot = new DataTable();

            string BranchCode = PackagePrices.BranchCode;
            string PackageCode = PackagePrices.PackageCode;

            ArrayList arl_Mobile = MobileCheck(PackagePrices.MobileNo);
            int EnquireformCount = Convert.ToInt32(arl_Mobile[0].ToString());
            int UserCount = Convert.ToInt32(arl_Mobile[1].ToString());

            ArrayList arl_stlistA = new ArrayList();
            ArrayList arl_stlistB = new ArrayList();

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

            //CreateDatatable
            DataTable dt_temptable = new DataTable();

            // days calculation

            DateTime currentday = DateTime.Now;
            DateTime firstDayThisMonth = new DateTime(currentday.Year, currentday.Month, 1);
            DateTime firstDayPlusTwoMonths = firstDayThisMonth.AddMonths(2);
            DateTime lastDayNextMonth = firstDayPlusTwoMonths.AddDays(-1);
            DateTime endOfLastDayNextMonth = firstDayPlusTwoMonths.AddTicks(-1);

            int days = Convert.ToInt32((endOfLastDayNextMonth - currentday).TotalDays);
            string date1 = null;
            DateTime? date = null;
            // days loops start

            for (int i = 0; i <= days; i++)
            {
                date1 = currentday.AddDays(i).ToShortDateString();
                date = System.DateTime.Parse(date1);

                dt_Slotused = GetAllSlotsByPackageCodeUsedAUTOPRICE(PackagePrices.BranchCode, PackagePrices.PackageCode, Convert.ToString(date));

                if (dt_Slotused.Rows.Count > 0)
                {
                    dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICE(PackagePrices.BranchCode, PackagePrices.PackageCode);
                    foreach (DataRow rrow in dt_Slotavailable.Rows)
                    {
                        foreach (DataRow trow in dt_Slotused.Rows)
                        {
                            rrow["Date"] = date;

                            AvailableSlots = Convert.ToInt32(rrow["RegularMembersPerSlot"]);
                            FilledSlots = Convert.ToInt32(trow["FilledSlots"].ToString());

                            if (rrow["SlotCode"].ToString() == trow["SlotCode"].ToString())
                            {
                                //PackageCode
                                if (rrow["PackageCode"].ToString() == trow["PackageCode"].ToString())
                                {
                                    string val1 = rrow["SlotCode"].ToString();
                                    string val2 = trow["SlotCode"].ToString();

                                    if (trow["SNO"].ToString() == "1")
                                    {

                                        arl_stlistA.Add(rrow["SlotCode"].ToString());
                                        PlanCost = Convert.ToInt32(rrow["PlanCost"].ToString());
                                        SlotPrice = Convert.ToInt32(rrow["SlotPrice"].ToString());
                                        MinimumAmount = Convert.ToInt32(rrow["MinPlanCost"].ToString());
                                        MaximumAmount = Convert.ToInt32(rrow["MaxPlanCost"].ToString());


                                        //new calculation Logics 10/21/2020
                                        //SlotCapacity = (FilledSlots/AvailableSlots)*100;
                                        //if(either 25/50/75/100)
                                        //formala 
                                        //SlotPrice= minimumprice+(minimumprice*(perventage get by DailyPricePercentage/100))

                                        try
                                        {
                                            capacityPercentage = Convert.ToDouble(FilledSlots) / Convert.ToDouble(AvailableSlots);
                                            SlotCapacity = Math.Round(capacityPercentage * 100);
                                        }
                                        catch (Exception ec)
                                        {

                                        }

                                        int count = i + 1;
                                        DataRow[] results = dt_DailyPricePercentage.Select("Days = " + count + " ");
                                        foreach (DataRow row in results)
                                        {
                                            Percentage = row["Percentage"].ToString();
                                            break;
                                        }

                                        float PercentageConverter = float.Parse(Percentage) / 100;

                                        // day wise MinimumAmount calculation

                                        if (SlotCapacity > 0 && SlotCapacity <= 25)
                                        {
                                            SlotPrice = (MinimumAmount * PercentageConverter) + (MinimumAmount);
                                        }
                                        else if (SlotCapacity > 25 && SlotCapacity <= 50)
                                        {
                                            x = (MinimumAmount * PercentageConverter) + (MinimumAmount);

                                            SlotPrice = (x * PercentageConverter) + (MinimumAmount);
                                        }
                                        else if (SlotCapacity > 50 && SlotCapacity <= 75)
                                        {
                                            x = (MinimumAmount * PercentageConverter) + (MinimumAmount);
                                            y = (x * PercentageConverter) + (MinimumAmount);
                                            SlotPrice = (y * PercentageConverter) + (MinimumAmount);
                                        }
                                        else if (SlotCapacity > 75 && SlotCapacity <= 100)
                                        {
                                            x = (MinimumAmount * PercentageConverter) + (MinimumAmount);
                                            y = (x * PercentageConverter) + (MinimumAmount);
                                            z = (y * PercentageConverter) + (MinimumAmount);
                                            SlotPrice = (z * PercentageConverter) + (MinimumAmount);
                                        }
                                        else
                                        {
                                            SlotPrice = 0;
                                        }

                                        try
                                        {
                                            rrow["SlotPrice"] = Math.Round(SlotPrice);
                                        }
                                        catch (Exception ec)
                                        {
                                            rrow["SlotPrice"] = 0.0f;
                                        }

                                        if (FilledSlots < AvailableSlots)
                                            rrow["IsRegularSlotAvailable"] = 1;
                                        else if (FilledSlots == AvailableSlots)
                                            rrow["IsRegularSlotAvailable"] = 0;
                                        else
                                            rrow["IsRegularSlotAvailable"] = 0;


                                        int Duration = Convert.ToInt32(rrow["DurationNo"].ToString());

                                        var da = Math.Round((PlanCost - SlotPrice));
                                        var dp = Math.Round(((da / PlanCost) * 100));
                                        var pmp = Math.Round((SlotPrice / Duration));

                                        rrow["RegularMembersFilledSlots"] = Convert.ToInt32(trow["FilledSlots"].ToString());
                                        rrow["DiscountAmount"] = Math.Round((PlanCost - SlotPrice));
                                        rrow["DiscountPercentage"] = Math.Round(((da / PlanCost) * 100));
                                        rrow["PerMonthPrice"] = Math.Round((SlotPrice / Duration));
                                    }
                                }
                            }
                            else
                            {
                                arl_stlistB.Add(rrow["SlotCode"].ToString());
                                PlanCost = Convert.ToInt32(rrow["PlanCost"].ToString());
                                SlotPrice = Convert.ToInt32(rrow["SlotPrice"].ToString());
                                MinimumAmount = Convert.ToInt32(rrow["MinPlanCost"].ToString());
                                MaximumAmount = Convert.ToInt32(rrow["MaxPlanCost"].ToString());
                                Constant = Convert.ToInt32(rrow["SlotPricingConstant"].ToString());

                                int count = i + 1;
                                DataRow[] results = dt_DailyPricePercentage.Select("Days = " + count + " ");
                                foreach (DataRow row in results)
                                {
                                    Percentage = row["Percentage"].ToString();
                                    break;
                                }

                                float PercentageConverter = float.Parse(Percentage) / 100;
                                SlotPrice = (MinimumAmount * PercentageConverter) + (MinimumAmount);

                                try
                                {
                                    rrow["SlotPrice"] = Math.Round(SlotPrice);
                                }
                                catch (Exception ec)
                                {
                                    rrow["SlotPrice"] = 0.0f;
                                }

                                int Duration = Convert.ToInt32(rrow["DurationNo"].ToString());

                                var da = Math.Round((PlanCost - SlotPrice));
                                var dp = Math.Round(((da / PlanCost) * 100));
                                var pmp = Math.Round((SlotPrice / Duration));

                                rrow["DiscountAmount"] = Math.Round((PlanCost - SlotPrice));
                                rrow["DiscountPercentage"] = Math.Round(((da / PlanCost) * 100));
                                rrow["PerMonthPrice"] = Math.Round((SlotPrice / Duration));

                            }
                        }
                    }
                }
                else
                {
                    dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICEDefault(BranchCode, PackageCode);
                }

                dt_temptable.Merge(dt_Slotavailable);

            }  // end of days loop

            int sloutcount = dt_temptable.Rows.Count;
            dt_temptable.DefaultView.Sort = "SlotStartTime ASC";
            DataTable st_newSlotavailable = new DataTable();
            st_newSlotavailable = dt_temptable.DefaultView.ToTable();
            DataTable dt_FilteredSlotsAvailable = new DataTable();

            List<OffLineEnquireInfo> olei = new List<OffLineEnquireInfo>();
            olei.Add(new OffLineEnquireInfo { isFreeTrial = EnquiretypeNo, enquireTypeNo = 2 });

            List<OffLineOnlySlotsPackageDetails> dt_SlotPackageDopt = new List<OffLineOnlySlotsPackageDetails>();
            OffLineOnlySlotsPackageDetails dashboardlistpackage = new OffLineOnlySlotsPackageDetails { info = olei, dateWisePrice = GetSevenDaysSlotPriceDurationDateWise(st_newSlotavailable), slots = GetOnlySlotsDetails(st_newSlotavailable, PackagePrices.StartDate, PackagePrices.BranchCode, PackagePrices.PackageCode), session = GetSessionDetails(PackagePrices.BranchCode, PackagePrices.PackageCode) };
            dt_SlotPackageDopt.Add(dashboardlistpackage);

            OffLineOnlySlotsPackageOutPut olppFOP = new OffLineOnlySlotsPackageOutPut();

            olppFOP.status = "Success";
            olppFOP.value = dt_SlotPackageDopt;
            sJSONResponse = JsonConvert.SerializeObject(olppFOP);


            return sJSONResponse;

        }

        // logic for splitting datewisepackage amount.
        public List<OffLineDurationDateWise> GetSevenDaysSlotPriceDurationDateWise(DataTable st_newSlotavailable)
        {

            DateTime currentday = DateTime.Now;
            DateTime firstDayThisMonth = new DateTime(currentday.Year, currentday.Month, 1);
            DateTime firstDayPlusTwoMonths = firstDayThisMonth.AddMonths(2);
            DateTime lastDayNextMonth = firstDayPlusTwoMonths.AddDays(-1);
            DateTime endOfLastDayNextMonth = firstDayPlusTwoMonths.AddTicks(-1);

            DataTable dt_filter = new DataTable();
            DataTable dt_datewise = new DataTable();

            int days = Convert.ToInt32((endOfLastDayNextMonth - currentday).TotalDays);
            string date1 = null;
            DateTime? date = null;

            List<OffLineDurationDateWise> DateWiseDurationPrice = new List<OffLineDurationDateWise>();
            try
            {
                for (int i = 0; i <= 6 - 1; i++)
                {
                    date1 = currentday.AddDays(i).ToShortDateString();
                    date = System.DateTime.Parse(date1);

                    string Fromdate = date1 + " 00:00:00";
                    string Todate = date1 + " 23:59:59";

                    DataRow[] rows = st_newSlotavailable.Select("Date >= #" + Fromdate + "# AND Date <= #" + Todate + "#");

                    if (rows.Length > 0)
                    {
                        dt_datewise = rows.CopyToDataTable();
                    }

                    dt_filter = dt_datewise.AsEnumerable().GroupBy(r => new { Col1 = r["DurationCode"] }).Select(g => g.OrderBy(r => r["SlotPrice"]).First()).CopyToDataTable();
                    DateWiseDurationPrice.Add(new OffLineDurationDateWise { date = date.ToString(), slotPrice = GetSevenDaysDurationColumns(dt_filter) });
                }
            }
            catch (Exception ec)
            {

            }
            return DateWiseDurationPrice;
        }
        public List<OffLineSlotsPackage> GetOnlySlotsDetails(DataTable dt_SlotPackages, string Date, string BranchCode, string PackageCode)
        {
            double x = 0.0f;
            double y = 0.0f;
            double z = 0.0f;

            var Percentage = "";
            int AvailableSlots = 0;
            int FilledSlots = 0;
            float PlanCost = 0.0f;
            double SlotPrice = 0.0f;
            double SlotCapacity = 0.0f;
            float MinimumAmount = 0.0f;
            float MaximumAmount = 0.0f;
            float Constant = 0.0f;
            double capacityPercentage = 0.0f;

            DataTable dt_temptable = new DataTable();

            DataTable dt_DailyPricePercentage = getdata(string.Format("select Days,DaysDuration,Percentage from DailyPricePercentage", ""));

            DataTable dt_Slotavailable = new DataTable();
            DataTable dt_Slotused = new DataTable();
            DataTable dt_Slot = new DataTable();

            ArrayList arl_stlistA = new ArrayList();
            ArrayList arl_stlistB = new ArrayList();

            {



                dt_Slotused = GetAllSlotsByPackageCodeUsedAUTOPRICE(BranchCode, PackageCode, Convert.ToString(Date));

                if (dt_Slotused.Rows.Count > 0)
                {
                    dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICE(BranchCode, PackageCode);
                    foreach (DataRow rrow in dt_Slotavailable.Rows)
                    {

                        foreach (DataRow trow in dt_Slotused.Rows)
                        {
                            rrow["Date"] = Date;

                            AvailableSlots = Convert.ToInt32(rrow["RegularMembersPerSlot"]);
                            FilledSlots = Convert.ToInt32(trow["FilledSlots"].ToString());

                            if (rrow["SlotCode"].ToString() == trow["SlotCode"].ToString())
                            {
                                //PackageCode
                                if (rrow["PackageCode"].ToString() == trow["PackageCode"].ToString())
                                {
                                    string val1 = rrow["SlotCode"].ToString();
                                    string val2 = trow["SlotCode"].ToString();

                                    if (trow["SNO"].ToString() == "1")
                                    {

                                        arl_stlistA.Add(rrow["SlotCode"].ToString());
                                        PlanCost = Convert.ToInt32(rrow["PlanCost"].ToString());
                                        SlotPrice = Convert.ToInt32(rrow["SlotPrice"].ToString());
                                        MinimumAmount = Convert.ToInt32(rrow["MinPlanCost"].ToString());
                                        MaximumAmount = Convert.ToInt32(rrow["MaxPlanCost"].ToString());


                                        //new calculation Logics 10/21/2020
                                        //SlotCapacity = (FilledSlots/AvailableSlots)*100;
                                        //if(either 25/50/75/100)
                                        //formala 
                                        //SlotPrice= minimumprice+(minimumprice*(perventage get by DailyPricePercentage/100))

                                        try
                                        {
                                            capacityPercentage = Convert.ToDouble(FilledSlots) / Convert.ToDouble(AvailableSlots);
                                            SlotCapacity = Math.Round(capacityPercentage * 100);
                                        }
                                        catch (Exception ec)
                                        {

                                        }


                                        DataRow[] results = dt_DailyPricePercentage.Select("Days = " + 1 + " ");
                                        foreach (DataRow row in results)
                                        {
                                            Percentage = row["Percentage"].ToString();
                                            break;
                                        }

                                        float PercentageConverter = float.Parse(Percentage) / 100;

                                        // day wise MinimumAmount calculation

                                        if (SlotCapacity > 0 && SlotCapacity <= 25)
                                        {
                                            SlotPrice = (MinimumAmount * PercentageConverter) + (MinimumAmount);
                                        }
                                        else if (SlotCapacity > 25 && SlotCapacity <= 50)
                                        {
                                            x = (MinimumAmount * PercentageConverter) + (MinimumAmount);

                                            SlotPrice = (x * PercentageConverter) + (MinimumAmount);
                                        }
                                        else if (SlotCapacity > 50 && SlotCapacity <= 75)
                                        {
                                            x = (MinimumAmount * PercentageConverter) + (MinimumAmount);
                                            y = (x * PercentageConverter) + (MinimumAmount);
                                            SlotPrice = (y * PercentageConverter) + (MinimumAmount);
                                        }
                                        else if (SlotCapacity > 75 && SlotCapacity <= 100)
                                        {
                                            x = (MinimumAmount * PercentageConverter) + (MinimumAmount);
                                            y = (x * PercentageConverter) + (MinimumAmount);
                                            z = (y * PercentageConverter) + (MinimumAmount);
                                            SlotPrice = (z * PercentageConverter) + (MinimumAmount);
                                        }
                                        else
                                        {
                                            SlotPrice = 0;
                                        }

                                        try
                                        {
                                            rrow["SlotPrice"] = Math.Round(SlotPrice);
                                        }
                                        catch (Exception ec)
                                        {
                                            rrow["SlotPrice"] = 0.0f;
                                        }

                                        if (FilledSlots < AvailableSlots)
                                            rrow["IsRegularSlotAvailable"] = 1;
                                        else if (FilledSlots == AvailableSlots)
                                            rrow["IsRegularSlotAvailable"] = 0;
                                        else
                                            rrow["IsRegularSlotAvailable"] = 0;


                                        int Duration = Convert.ToInt32(rrow["DurationNo"].ToString());

                                        var da = Math.Round((PlanCost - SlotPrice));
                                        var dp = Math.Round(((da / PlanCost) * 100));
                                        var pmp = Math.Round((SlotPrice / Duration));

                                        rrow["RegularMembersFilledSlots"] = Convert.ToInt32(trow["FilledSlots"].ToString());
                                        rrow["DiscountAmount"] = Math.Round((PlanCost - SlotPrice));
                                        rrow["DiscountPercentage"] = Math.Round(((da / PlanCost) * 100));
                                        rrow["PerMonthPrice"] = Math.Round((SlotPrice / Duration));
                                    }


                                }
                            }
                            else
                            {
                                arl_stlistB.Add(rrow["SlotCode"].ToString());
                                PlanCost = Convert.ToInt32(rrow["PlanCost"].ToString());
                                SlotPrice = Convert.ToInt32(rrow["SlotPrice"].ToString());
                                MinimumAmount = Convert.ToInt32(rrow["MinPlanCost"].ToString());
                                MaximumAmount = Convert.ToInt32(rrow["MaxPlanCost"].ToString());
                                Constant = Convert.ToInt32(rrow["SlotPricingConstant"].ToString());


                                DataRow[] results = dt_DailyPricePercentage.Select("Days = " + 1 + " ");
                                foreach (DataRow row in results)
                                {
                                    Percentage = row["Percentage"].ToString();
                                    break;
                                }

                                float PercentageConverter = float.Parse(Percentage) / 100;
                                SlotPrice = (MinimumAmount * PercentageConverter) + (MinimumAmount);

                                try
                                {
                                    rrow["SlotPrice"] = Math.Round(SlotPrice);
                                }
                                catch (Exception ec)
                                {
                                    rrow["SlotPrice"] = 0.0f;
                                }

                                int Duration = Convert.ToInt32(rrow["DurationNo"].ToString());

                                var da = Math.Round((PlanCost - SlotPrice));
                                var dp = Math.Round(((da / PlanCost) * 100));
                                var pmp = Math.Round((SlotPrice / Duration));

                                rrow["DiscountAmount"] = Math.Round((PlanCost - SlotPrice));
                                rrow["DiscountPercentage"] = Math.Round(((da / PlanCost) * 100));
                                rrow["PerMonthPrice"] = Math.Round((SlotPrice / Duration));

                            }

                        }
                    }   // 1 st loop  for each
                }
                else
                {
                    dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICEDefault(BranchCode, PackageCode);
                }

                dt_temptable.Merge(dt_Slotavailable);
            }

            DataTable dt_filter = new DataTable();

            string date1 = null;
            DateTime? date = null;
            DateTime StartDate = Convert.ToDateTime(Date);
            date1 = StartDate.AddDays(0).ToShortDateString();
            date = System.DateTime.Parse(date1);

            string Fromdate = date1 + " 00:00:00";
            string Todate = date1 + " 23:59:59";

            DataRow[] rows = dt_temptable.Select("Date >= #" + Fromdate + "# AND Date <= #" + Todate + "#");

            List<OffLineSlotsPackage> offLineSlotPackage = new List<OffLineSlotsPackage>();

            try
            {
                dt_filter = rows.CopyToDataTable();

                for (int i = 0; i < dt_filter.Rows.Count; i++)
                {

                    offLineSlotPackage.Add(new OffLineSlotsPackage
                    {
                        sNo = Convert.ToInt32(dt_filter.Rows[i]["SNO"])
                        ,
                        slotCode = Convert.ToString(dt_filter.Rows[i]["SlotCode"])
                         ,
                        sessionCode = Convert.ToString(dt_filter.Rows[i]["SessionCode"])
                         ,
                        packageCode = Convert.ToString(dt_filter.Rows[i]["PackageCode"])
                        ,
                        isSlotAvailable = Convert.ToInt32(dt_filter.Rows[i]["IsRegularSlotAvailable"])
                         ,
                        planCost = Convert.ToDouble(dt_filter.Rows[i]["PlanCost"])
                         ,
                        duration = Convert.ToString(dt_filter.Rows[i]["Duration"])
                         ,
                        noOfDays = Convert.ToInt32(dt_filter.Rows[i]["NoOfDays"])
                         ,
                        durationCode = Convert.ToString(dt_filter.Rows[i]["DurationCode"])
                        ,
                        planCode = Convert.ToString(dt_filter.Rows[i]["PlanCode"])
                         ,
                        planName = Convert.ToString(dt_filter.Rows[i]["PlanName"])
                         ,
                        trainerCode = Convert.ToString(dt_filter.Rows[i]["TrainersCode"])
                         ,
                        trainerName = Convert.ToString(dt_filter.Rows[i]["TrainersName"])
                         ,
                        slotPrice = Convert.ToDouble(dt_filter.Rows[i]["SlotPrice"])
                        ,
                        slotStartTime = Convert.ToString(dt_filter.Rows[i]["SlotStartTime"])
                         ,
                        slotEndTime = Convert.ToString(dt_filter.Rows[i]["SlotEndTime"])
                        ,
                        discountAmount = float.Parse(dt_filter.Rows[i]["DiscountAmount"].ToString())
                        ,
                        discountPercentage = float.Parse(dt_filter.Rows[i]["DiscountPercentage"].ToString())
                        ,
                        perMonthPrice = float.Parse(dt_filter.Rows[i]["PerMonthPrice"].ToString())

                    });

                }

            }
            catch (Exception ec)
            {

            }



            return offLineSlotPackage;
        }

        public List<OffLineSessionPackage> GetSessionDetails(string BranchCode, string PackageCode)
        {

            List<OffLineSessionPackage> sessions = new List<OffLineSessionPackage>();
            DataTable dt_PackageSessions = getdata(string.Format("select SessionCode,SessionName,convert(varchar(10), SessionStartTime, 108) as SessionStartTime,convert(varchar(10), SessionEndTime, 108) as SessionEndTime from CMSSESSIONTIMESETTING", ""));

            DataTable dt_Sessions = GetAllSlotsByPackageCodeAvailableAUTOPRICE(BranchCode, PackageCode);

            DataView view = new DataView(dt_Sessions);
            dt_Sessions = view.ToTable(true, "SessionCode");


            //DataTable dt_filter = dt_Sessions.AsEnumerable().GroupBy(r => new { Col1 = r["SessionCode"] }).Select(g => g.OrderBy(r => r["SessionCode"]).First()).CopyToDataTable();

            for (int i = 0; i <= dt_PackageSessions.Rows.Count - 1; i++)
            {
                DataRow[] SessionSearch = dt_Sessions.Select("SessionCode = '" + dt_PackageSessions.Rows[i]["SessionCode"].ToString() + "'");
                if (SessionSearch.Length != 0)
                {
                    sessions.Add(new OffLineSessionPackage
                    {
                        sessionId = dt_PackageSessions.Rows[i]["SessionCode"].ToString()
                                    ,
                        sessionName = dt_PackageSessions.Rows[i]["SessionName"].ToString()
                                    ,
                        sessionStartTime = dt_PackageSessions.Rows[i]["SessionStartTime"].ToString()
                                   ,
                        sessionEndTime = dt_PackageSessions.Rows[i]["SessionEndTime"].ToString()

                    });
                }


            }
            return sessions;

        }

        // records per slot data
        public List<OffLineDurationWiseColums> GetSevenDaysDurationColumns(DataTable st_newSlotavailable)
        {

            //        dt.AsEnumerable()
            //.GroupBy(r => new { Col1 = r["Col1"], Col2 = r["Col2"] })
            //.Select(g =>
            //{
            //    var row = dt.NewRow();

            //    row["PK"] = g.Min(r => r.Field<int>("PK"));
            //    row["Col1"] = g.Key.Col1;
            //    row["Col2"] = g.Key.Col2;

            //    return row;

            //})
            //.CopyToDataTable();

            // get minimum price / duration : Logic
            DataTable dt_duratuonfilter = new DataTable();

            // dt_duratuonfilter = st_newSlotavailable.AsEnumerable().GroupBy(r => new { Col1 = r["SlotCode"], Col2 = r["DurationCode"] }).Select(g => g.OrderBy(r => r["SlotPrice"]).First()).CopyToDataTable();



            st_newSlotavailable = st_newSlotavailable.AsEnumerable().GroupBy(r => new { Col1 = r["DurationCode"] }).Select(g => { var row = st_newSlotavailable.NewRow(); row["SlotPrice"] = g.Min(r => r.Field<double>("SlotPrice")); row["DurationCode"] = g.Key.Col1; return row; }).CopyToDataTable();



            // var GroupBy = st_newSlotavailable.AsEnumerable().GroupBy(e => e.Field<string>("DurationCode")).Select(d => new { d.Key, Count = d.Count() });
            //DataTable dtTemp = GroupBy.cop
            //DataTable dtObj2 = new DataTable("tableName2"); // Set table name    
            //                                                //merging first data table into second data table      
            //dtObj2.Merge(dtObj1);

            List<OffLineDurationWiseColums> dc = new List<OffLineDurationWiseColums>();


            for (int i = 0; i <= st_newSlotavailable.Rows.Count - 1; i++)
            {
                dc.Add(new OffLineDurationWiseColums
                {
                    sno = i + 1
                                    ,

                    durationCode = st_newSlotavailable.Rows[i]["DurationCode"].ToString()
                                    ,
                    slotPrice = Convert.ToDouble(st_newSlotavailable.Rows[i]["SlotPrice"].ToString())

                });
            }


            return dc;

        }

        // Dynamic Pricing
        public string GetDateWiseDynamicPricing([FromBody]OffLineCMSSlotsAvailability PackagePrices)
        {

            var Percentage = "";
            string sJSONResponse = "";

            ArrayList arl_stlistA = new ArrayList();
            ArrayList arl_stlistB = new ArrayList();

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_slots = new DataSet();
            string result = string.Empty;

            DataTable dt_DailyPricePercentage = getdata(string.Format("select Days,DaysDuration,Percentage from DailyPricePercentage", ""));

            int AvailableSlots = 0;
            int FilledSlots = 0;
            float PlanCost = 0.0f;
            double SlotPrice = 0.0f;
            float MinimumAmount = 0.0f;
            float MaximumAmount = 0.0f;
            float Constant = 0.0f;
            double SlotCapacity = 0.0f;
            double capacityPercentage = 0.0f;
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

            //CreateDatatable
            DataTable dt_temptable = new DataTable();

            // days calculation

            DateTime currentday = DateTime.Now;
            DateTime firstDayThisMonth = new DateTime(currentday.Year, currentday.Month, 1);
            DateTime firstDayPlusTwoMonths = firstDayThisMonth.AddMonths(2);
            DateTime lastDayNextMonth = firstDayPlusTwoMonths.AddDays(-1);
            DateTime endOfLastDayNextMonth = firstDayPlusTwoMonths.AddTicks(-1);

            int days = Convert.ToInt32((endOfLastDayNextMonth - currentday).TotalDays);
            string date1 = null;
            DateTime? date = null;
            // days loops start
            dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICE(PackagePrices.BranchCode, PackagePrices.PackageCode);

            int count = 0;
            int quotient = 0;
            int cntd = 0;
            try
            {

                for (int i = 0; i <= days - 2; i++)
                {
                    date1 = currentday.AddDays(i).ToShortDateString();
                    date = System.DateTime.Parse(date1);
                    cntd = i + 1;
                    if (i <= 6)
                    {
                        DataRow[] results = dt_DailyPricePercentage.Select("Days = " + cntd + " AND DaysDuration= 'Day' ");
                        foreach (DataRow row in results)
                        {
                            Percentage = row["Percentage"].ToString();
                            break;
                        }
                    }
                    else
                    {
                        float rmd = i % 7;
                        if (rmd == 0)
                        {
                            count = i + 7;
                            quotient = i / 7;
                            DataRow[] results = dt_DailyPricePercentage.Select("Days = " + quotient + " AND DaysDuration= 'Week' ");
                            foreach (DataRow row in results)
                            {
                                Percentage = row["Percentage"].ToString();
                                break;
                            }


                        }
                        else
                        {

                        }
                    }

                    dt_Slotused = GetAllSlotsByPackageCodeUsedAUTOPRICE(PackagePrices.BranchCode, PackagePrices.PackageCode, Convert.ToString(date));
                    if (dt_Slotused.Rows.Count > 0)
                    {
                        dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICE(PackagePrices.BranchCode, PackagePrices.PackageCode);
                        foreach (DataRow rrow in dt_Slotavailable.Rows)
                        {
                            foreach (DataRow trow in dt_Slotused.Rows)
                            {
                                rrow["Date"] = date;

                                AvailableSlots = Convert.ToInt32(rrow["RegularMembersPerSlot"]);
                                FilledSlots = Convert.ToInt32(trow["FilledSlots"].ToString());

                                if (rrow["SlotCode"].ToString() == trow["SlotCode"].ToString())
                                {
                                    //PackageCode
                                    if (rrow["PackageCode"].ToString() == trow["PackageCode"].ToString())
                                    {
                                        string val1 = rrow["SlotCode"].ToString();
                                        string val2 = trow["SlotCode"].ToString();

                                        if (trow["SNO"].ToString() == "1")
                                        {
                                            arl_stlistA.Add(rrow["SlotCode"].ToString());
                                            PlanCost = Convert.ToInt32(rrow["PlanCost"].ToString());
                                            SlotPrice = Convert.ToInt32(rrow["SlotPrice"].ToString());
                                            MinimumAmount = Convert.ToInt32(rrow["MinPlanCost"].ToString());
                                            MaximumAmount = Convert.ToInt32(rrow["MaxPlanCost"].ToString());


                                            //new calculation Logics 10/21/2020
                                            //SlotCapacity = (FilledSlots/AvailableSlots)*100;
                                            //if(either 25/50/75/100)
                                            //formala 
                                            //SlotPrice= minimumprice+(minimumprice*(perventage get by DailyPricePercentage/100))

                                            try
                                            {
                                                capacityPercentage = Convert.ToDouble(FilledSlots) / Convert.ToDouble(AvailableSlots);
                                                SlotCapacity = Math.Round(capacityPercentage * 100);
                                            }
                                            catch (Exception ec)
                                            {

                                            }

                                            rrow["SlotPrice"] = Math.Round(CalculateSlotPrice(capacityPercentage, SlotCapacity, MinimumAmount, MaximumAmount, AvailableSlots, FilledSlots, Percentage));





                                            if (FilledSlots < AvailableSlots)
                                                rrow["IsRegularSlotAvailable"] = 1;
                                            else if (FilledSlots == AvailableSlots)
                                                rrow["IsRegularSlotAvailable"] = 0;
                                            else
                                                rrow["IsRegularSlotAvailable"] = 0;


                                            int Duration = Convert.ToInt32(rrow["DurationNo"].ToString());

                                            var da = Math.Round((PlanCost - SlotPrice));
                                            var dp = Math.Round(((da / PlanCost) * 100));
                                            var pmp = Math.Round((SlotPrice / Duration));

                                            rrow["RegularMembersFilledSlots"] = Convert.ToInt32(trow["FilledSlots"].ToString());
                                            rrow["DiscountAmount"] = Math.Round((PlanCost - SlotPrice));
                                            rrow["DiscountPercentage"] = Math.Round(((da / PlanCost) * 100));
                                            rrow["PerMonthPrice"] = Math.Round((SlotPrice / Duration));
                                        }
                                    }
                                }
                                else
                                {
                                    arl_stlistB.Add(rrow["SlotCode"].ToString());
                                    PlanCost = Convert.ToInt32(rrow["PlanCost"].ToString());
                                    SlotPrice = Convert.ToInt32(rrow["SlotPrice"].ToString());
                                    MinimumAmount = Convert.ToInt32(rrow["MinPlanCost"].ToString());
                                    MaximumAmount = Convert.ToInt32(rrow["MaxPlanCost"].ToString());
                                    Constant = Convert.ToInt32(rrow["SlotPricingConstant"].ToString());

                                    rrow["SlotPrice"] = Math.Round(CalculateSlotPrice2(MinimumAmount, Percentage));




                                    int Duration = Convert.ToInt32(rrow["DurationNo"].ToString());

                                    var da = Math.Round((PlanCost - SlotPrice));
                                    var dp = Math.Round(((da / PlanCost) * 100));
                                    var pmp = Math.Round((SlotPrice / Duration));

                                    rrow["DiscountAmount"] = Math.Round((PlanCost - SlotPrice));
                                    rrow["DiscountPercentage"] = Math.Round(((da / PlanCost) * 100));
                                    rrow["PerMonthPrice"] = Math.Round((SlotPrice / Duration));

                                }

                            }
                        }   // 1 st loop  for each
                    }
                    else
                    {
                        dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICEDefault(BranchCode, PackageCode);
                    }

                    dt_temptable.Merge(dt_Slotavailable);

                }  // end of days loop

            }
            catch
            {

            }


            int sloutcount = dt_Slotavailable.Rows.Count;
            dt_Slotavailable.DefaultView.Sort = "SlotStartTime ASC";
            DataTable st_newSlotavailable = new DataTable();
            st_newSlotavailable = dt_Slotavailable.DefaultView.ToTable();
            DataTable dt_FilteredSlotsAvailable = new DataTable();

            List<OffLineEnquireInfo> olei = new List<OffLineEnquireInfo>();
            olei.Add(new OffLineEnquireInfo { isFreeTrial = EnquiretypeNo, enquireTypeNo = 2 });

            List<OffLineDurationDateDetails> dt_SlotPackageDopt = new List<OffLineDurationDateDetails>();
            OffLineDurationDateDetails dashboardlistpackage = new OffLineDurationDateDetails { dateWisePrice = GetSlotPriceDurationDateWise(dt_temptable) };
            dt_SlotPackageDopt.Add(dashboardlistpackage);

            OffLineDurationDateOutPut olppFOP = new OffLineDurationDateOutPut();

            olppFOP.status = "Success";
            olppFOP.value = dt_SlotPackageDopt;
            sJSONResponse = JsonConvert.SerializeObject(olppFOP);


            return sJSONResponse;

        }
        public double CalculateSlotPrice(double capacityPercentage, double SlotCapacity, float MinimumAmount, float MaximumAmount, int AvailableSlots, int FilledSlots, string Percentage)
        {
            double SlotPrice = 0.0f;

            float x = 0.0f;
            float y = 0.0f;
            float z = 0.0f;

            try
            {
                capacityPercentage = Convert.ToDouble(FilledSlots) / Convert.ToDouble(AvailableSlots);
                SlotCapacity = Math.Round(capacityPercentage * 100);
            }
            catch (Exception ec)
            {
            }

            float PercentageConverter = float.Parse(Percentage) / 100;

            // day wise MinimumAmount calculation

            if (SlotCapacity > 0 && SlotCapacity <= 25)
            {
                SlotPrice = (MinimumAmount * PercentageConverter) + (MinimumAmount);
            }
            else if (SlotCapacity > 25 && SlotCapacity <= 50)
            {
                x = (MinimumAmount * PercentageConverter) + (MinimumAmount);

                SlotPrice = (x * PercentageConverter) + (MinimumAmount);
            }
            else if (SlotCapacity > 50 && SlotCapacity <= 75)
            {
                x = (MinimumAmount * PercentageConverter) + (MinimumAmount);
                y = (x * PercentageConverter) + (MinimumAmount);
                SlotPrice = (y * PercentageConverter) + (MinimumAmount);
            }
            else if (SlotCapacity > 75 && SlotCapacity <= 100)
            {
                x = (MinimumAmount * PercentageConverter) + (MinimumAmount);
                y = (x * PercentageConverter) + (MinimumAmount);
                z = (y * PercentageConverter) + (MinimumAmount);
                SlotPrice = (z * PercentageConverter) + (MinimumAmount);
            }
            else
            {
                SlotPrice = 0;
            }

            return SlotPrice;
        }

        public double CalculateSlotPrice2(float MinimumAmount, string Percentage)
        {
            double SlotPrice = 0.0f;

            float PercentageConverter = float.Parse(Percentage) / 100;
            SlotPrice = (MinimumAmount * PercentageConverter) + (MinimumAmount);



            return SlotPrice;
        }
        public List<OffLineDurationDateWise> GetSlotPriceDurationDateWise(DataTable st_newSlotavailable)
        {

            DateTime currentday = DateTime.Now;
            DateTime firstDayThisMonth = new DateTime(currentday.Year, currentday.Month, 1);
            DateTime firstDayPlusTwoMonths = firstDayThisMonth.AddMonths(2);
            DateTime lastDayNextMonth = firstDayPlusTwoMonths.AddDays(-1);
            DateTime endOfLastDayNextMonth = firstDayPlusTwoMonths.AddTicks(-1);

            int days = Convert.ToInt32((endOfLastDayNextMonth - currentday).TotalDays);
            string date1 = null;
            DateTime? date = null;

            DataTable dt_filter = new DataTable();
            DataTable dt_datewise = new DataTable();

            List<OffLineDurationDateWise> DateWiseDurationPrice = new List<OffLineDurationDateWise>();
            try
            {
                for (int i = 0; i <= days - 1; i++)
                {
                    date1 = currentday.AddDays(i).ToShortDateString();
                    date = System.DateTime.Parse(date1);

                    string Fromdate = date1 + " 00:00:00";
                    string Todate = date1 + " 23:59:59";

                    DataRow[] rows = st_newSlotavailable.Select("Date >= #" + Fromdate + "# AND Date <= #" + Todate + "#");

                    if (rows.Length > 0)
                    {
                        dt_datewise = rows.CopyToDataTable();
                    }
                    dt_filter = dt_datewise.AsEnumerable().GroupBy(r => new { Col1 = r["DurationCode"] }).Select(g => g.OrderBy(r => r["SlotPrice"]).First()).CopyToDataTable();
                    DateWiseDurationPrice.Add(new OffLineDurationDateWise { date = date.ToString(), slotPrice = GetDurationColumns(dt_filter) });
                }
            }
            catch (Exception ec)
            {

            }
            return DateWiseDurationPrice;
        }
        // records per slot data
        public List<OffLineDurationWiseColums> GetDurationColumns(DataTable st_newSlotavailable)
        {

            //        dt.AsEnumerable()
            //.GroupBy(r => new { Col1 = r["Col1"], Col2 = r["Col2"] })
            //.Select(g =>
            //{
            //    var row = dt.NewRow();

            //    row["PK"] = g.Min(r => r.Field<int>("PK"));
            //    row["Col1"] = g.Key.Col1;
            //    row["Col2"] = g.Key.Col2;

            //    return row;

            //})
            //.CopyToDataTable();

            // get minimum price / duration : Logic
            DataTable dt_duratuonfilter = new DataTable();

            // dt_duratuonfilter = st_newSlotavailable.AsEnumerable().GroupBy(r => new { Col1 = r["SlotCode"], Col2 = r["DurationCode"] }).Select(g => g.OrderBy(r => r["SlotPrice"]).First()).CopyToDataTable();



            st_newSlotavailable = st_newSlotavailable.AsEnumerable().GroupBy(r => new { Col1 = r["DurationCode"], Col2 = r["Duration"] }).Select(g => { var row = st_newSlotavailable.NewRow(); row["SlotPrice"] = g.Min(r => r.Field<double>("SlotPrice")); row["DurationCode"] = g.Key.Col1; row["Duration"] = g.Key.Col2; return row; }).CopyToDataTable();



            // var GroupBy = st_newSlotavailable.AsEnumerable().GroupBy(e => e.Field<string>("DurationCode")).Select(d => new { d.Key, Count = d.Count() });
            //DataTable dtTemp = GroupBy.cop
            //DataTable dtObj2 = new DataTable("tableName2"); // Set table name    
            //                                                //merging first data table into second data table      
            //dtObj2.Merge(dtObj1);

            List<OffLineDurationWiseColums> dc = new List<OffLineDurationWiseColums>();


            for (int i = 0; i <= st_newSlotavailable.Rows.Count - 1; i++)
            {
                dc.Add(new OffLineDurationWiseColums
                {
                    sno = i + 1

                                    ,
                    durationCode = st_newSlotavailable.Rows[i]["DurationCode"].ToString()
                                    ,
                    slotPrice = Convert.ToDouble(st_newSlotavailable.Rows[i]["SlotPrice"].ToString())

                });
            }


            return dc;

        }

    }
}