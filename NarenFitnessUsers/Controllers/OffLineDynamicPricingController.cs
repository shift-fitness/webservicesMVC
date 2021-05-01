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
using NarenFitnessUsers.Models.DynamicPricing;
using NarenFitnessUsers.Models.PackagesList;
using NarenFitnessUsers.Models.OffLinePackageList;
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
    public class OffLineDynamicPricingController : Controller
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
        public DataTable AvailableSlots(string BranchCode, string PackageCode)
        {
            DataSet ds_custdet1 = new DataSet();
            DataTable dt_Slotav = new DataTable();
            string result = string.Empty;


            string query1 = "";
            if (PackageCode == "P0")
            {
                query1 = "select distinct SlotCode,PackageCode,AllocatedCount from CMSSlotWiseAllocation where BranchCode='" + BranchCode + "' group by PackageCode,SlotCode,AllocatedCount";
            }
            else
            {
                query1 = "select distinct SlotCode,PackageCode,AllocatedCount from CMSSlotWiseAllocation where BranchCode='" + BranchCode + "' and PackageCode='" + PackageCode + "'  group by PackageCode,SlotCode,AllocatedCount";
            }

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }


            return ds_custdet1.Tables[0];
        }
        public DataTable UsedSlots(string BranchCode, string PackageCode, string StartDate)
        {
            DataSet ds_custdet1 = new DataSet();
            DataTable dt_Slotav = new DataTable();

            string query1 = "select SlotCode,PackageCode,count(*) as UsedCount from CCRMMembership where '" + StartDate + "' < MembershipExpireDate and PackageCode='" + PackageCode + "' and BranchCode='" + BranchCode + "'  group by SlotCode,PackageCode";


            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            dt_Slotav = ds_custdet1.Tables[0];
            return dt_Slotav;
        }
        public DataTable SlotAvailablility(string BranchCode, string PackageCode, string StartDate)
        {
            DataSet ds_custdet1 = new DataSet();
            DataTable dt_Slotav = new DataTable();
            string TrainerCode = "";
            string TrainerName = "";
            string result = string.Empty;

            DataTable dt_Trainer = getdata(string.Format("select distinct EmployeeCode as TrainerCode,EmployeeName as TrainerName from HREmployee where  BranchCode='" + BranchCode + "'  and IsActive=1 ", ""));

            string query1 = "";
            if (PackageCode == "P0")
            {
                query1 = "select distinct CMSSWA.SlotCode,CMSPC.SlotName,CMSSWA.PackageCode,CMSPC.PackageName,CMSPC.SessionCode,CMSPC.SessionName,CMSPC.DurationId,CMSPC.[PlanDuration/Monthly] as Duration,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.PlanCost,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,CMSSWA.AllocatedCount as RegularAllocatedCount,RegularUsedCount=0,CMSSWA.FreeTrialAllocatedCount,FreeTrialUsedCount=0,CMSSWA.TrainerCode from CMSSlotWiseAllocation CMSSWA,CMSPACKAGESCOST CMSPC,CMSPLANCOST CMSPLC where CMSPLC.PlanCostCode=CMSPC.PlanCostCode and  CMSPLC.PlanCode=CMSPC.PlanCode and CMSPC.BranchCode=CMSPLC.BranchCode and CMSPC.PackageCode=CMSPLC.PackageCode and CMSPC.BranchCode=CMSPLC.BranchCode and CMSPC.PackageCode=CMSSWA.PackageCode and CMSPC.SlotCode=CMSSWA.SlotCode  and   CMSSWA.BranchCode='" + BranchCode + "'  and CMSSWA.SlotCode not in(select CCRMM.SlotCode from CCRMMembership CCRMM,CMSSlotWiseAllocation CMSSWA where CMSSWA.BranchCode=CCRMM.BranchCode and  CMSSWA.PackageCode=CCRMM.PackageCode and CMSSWA.SlotCode=CCRMM.SlotCode and  '" + StartDate + "' < CCRMM.MembershipExpireDate and CCRMM.BranchCode='" + BranchCode + "'  group by CCRMM.SlotCode,CCRMM.PackageCode,CMSSWA.AllocatedCount)  group by CMSSWA.SlotCode,CMSPC.SlotName,CMSSWA.PackageCode,CMSPC.PackageName,CMSPC.SessionCode,CMSPC.SessionName,CMSPC.DurationId,CMSPC.[PlanDuration/Monthly],CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSSWA.AllocatedCount,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,CMSSWA.AllocatedCount,CMSSWA.FreeTrialAllocatedCount,CMSSWA.TrainerCode,CMSPC.PlanCost  union all  select CCRMM.SlotCode,CMSPC.SlotName,CCRMM.PackageCode,CMSPC.PackageName,CMSPC.SessionCode,CMSPC.SessionName,CMSPC.DurationId,CMSPC.[PlanDuration/Monthly] as Duration,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.PlanCost,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,CMSSWA.AllocatedCount as RegularAllocatedCount,count(*) as RegularUsedCount,CMSSWA.FreeTrialAllocatedCount,FreeTrialUsedCount=0,CMSSWA.TrainerCode from CCRMMembership CCRMM,CMSSlotWiseAllocation CMSSWA,CMSPACKAGESCOST CMSPC,CMSPLANCOST CMSPLC where CMSPLC.PlanCostCode=CMSPC.PlanCostCode and  CMSPLC.PlanCode=CMSPC.PlanCode and CMSPC.BranchCode=CMSSWA.BranchCode and CMSPC.PackageCode=CMSSWA.PackageCode and CMSPC.SlotCode=CMSSWA.SlotCode and CMSSWA.BranchCode=CCRMM.BranchCode and  CMSSWA.PackageCode=CCRMM.PackageCode and CMSSWA.SlotCode=CCRMM.SlotCode and  '" + StartDate + "' < CCRMM.MembershipExpireDate and CCRMM.BranchCode='" + BranchCode + "'  group by CCRMM.SlotCode,CMSPC.SlotName,CCRMM.PackageCode,CMSPC.PackageName,CMSPC.SessionCode,CMSPC.SessionName,CMSPC.DurationId,CMSPC.[PlanDuration/Monthly],CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSSWA.AllocatedCount,CMSPLC.MinPlanCost,CMSPLC.MaxPlanCost,CMSPLC.SlotPricingConstant,CMSSWA.AllocatedCount,CMSSWA.FreeTrialAllocatedCount,CMSSWA.TrainerCode,CMSPC.PlanCost";
            }
            else
            {
                query1 = "select * from (select  CMSSWA.SlotCode,CMSPC.SlotName,convert(varchar(10),CMSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSS.SlotEndTime, 108) as SlotEndTime,CMSSWA.PackageCode,CMSPC.PackageName,CMSPC.SessionCode,CMSPC.SessionName,CMSSWA.Duration as DurationId,CMSD.Duration,CMSPC.PlanCode,CMSPC.PlanName,CMSPLC.PlanCost,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSSWA.MinPrice as MinPlanCost,CMSSWA.MaxPrice as MaxPlanCost,CMSSWA.AllocatedCount as RegularAllocatedCount,RegularUsedCount=0,CMSSWA.FreeTrialAllocatedCount,FreeTrialUsedCount=0,CMSSWA.TrainerCode from CMSSlotWiseAllocation CMSSWA,CMSPACKAGESCOST CMSPC,CMSPLANCOST CMSPLC,CMSDURATION CMSD,CMSSLOTTIMESETTING CMSS where CMSS.SlotCode=CMSSWA.SlotCode and  CMSSWA.Duration=CMSPLC.DurationID and CMSD.DurationID=CMSSWA.Duration and CMSPLC.PlanCostCode=CMSPC.PlanCostCode and  CMSPLC.PlanCode=CMSPC.PlanCode and CMSPC.BranchCode=CMSPLC.BranchCode and CMSPC.PackageCode=CMSPLC.PackageCode and CMSPC.BranchCode=CMSPLC.BranchCode and CMSPC.PackageCode=CMSSWA.PackageCode and CMSPC.SlotCode=CMSSWA.SlotCode  and   CMSSWA.BranchCode='" + BranchCode + "' and CMSSWA.PackageCode='" + PackageCode + "' and CMSSWA.SlotCode not in(select CCRMM.SlotCode from CCRMMembership CCRMM,CMSSlotWiseAllocation CMSSWA where CMSSWA.BranchCode=CCRMM.BranchCode and  CMSSWA.PackageCode=CCRMM.PackageCode and CMSSWA.SlotCode=CCRMM.SlotCode and  '" + StartDate + "' < CCRMM.MembershipExpireDate and CCRMM.PackageCode='" + PackageCode + "' and CCRMM.BranchCode='" + BranchCode + "'  group by CCRMM.SlotCode,CCRMM.PackageCode,CMSSWA.AllocatedCount)  group by CMSSWA.SlotCode,CMSPC.SlotName,CMSSWA.PackageCode,CMSPC.PackageName,CMSPC.SessionCode,CMSPC.SessionName,CMSSWA.Duration,CMSD.Duration,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSSWA.AllocatedCount,CMSSWA.MinPrice,CMSSWA.MaxPrice,CMSSWA.AllocatedCount,CMSSWA.FreeTrialAllocatedCount,CMSSWA.TrainerCode,CMSPLC.PlanCost,CMSS.SlotStartTime,CMSS.SlotEndTime  union all  select CCRMM.SlotCode,CMSPC.SlotName,convert(varchar(10),CMSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSS.SlotEndTime, 108) as SlotEndTime,CCRMM.PackageCode,CMSPC.PackageName,CMSPC.SessionCode,CMSPC.SessionName,CMSSWA.Duration as DurationId,CMSD.Duration,CMSPC.PlanCode,CMSPC.PlanName,CMSPLC.PlanCost,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSSWA.MinPrice as MinPlanCost,CMSSWA.MaxPrice as MaxPlanCost,CMSSWA.AllocatedCount as RegularAllocatedCount,count(*) as RegularUsedCount,CMSSWA.FreeTrialAllocatedCount,FreeTrialUsedCount=0,CMSSWA.TrainerCode  from CCRMMembership CCRMM,CMSSlotWiseAllocation CMSSWA,CMSPACKAGESCOST CMSPC,CMSPLANCOST CMSPLC,CMSDURATION CMSD,CMSSLOTTIMESETTING CMSS where CMSS.SlotCode=CMSSWA.SlotCode and  CMSSWA.Duration=CMSPLC.DurationID and CMSD.DurationID=CMSSWA.Duration and CMSPLC.PlanCostCode=CMSPC.PlanCostCode and  CMSPLC.PlanCode=CMSPC.PlanCode and CMSPC.BranchCode=CMSSWA.BranchCode and CMSPC.PackageCode=CMSSWA.PackageCode and CMSPC.SlotCode=CMSSWA.SlotCode and CMSSWA.BranchCode=CCRMM.BranchCode and  CMSSWA.PackageCode=CCRMM.PackageCode and CMSSWA.SlotCode=CCRMM.SlotCode and  '" + StartDate + "' < CCRMM.MembershipExpireDate and CCRMM.PackageCode='" + PackageCode + "' and CCRMM.BranchCode='" + BranchCode + "'  group by CCRMM.SlotCode,CMSPC.SlotName,CCRMM.PackageCode,CMSPC.PackageName,CMSPC.SessionCode,CMSPC.SessionName,CMSSWA.Duration,CMSD.Duration,CMSPC.PlanCode,CMSPC.PlanName,CMSPC.SlotPrice,CMSPC.PlanCostCode,CMSSWA.AllocatedCount,CMSSWA.MinPrice,CMSSWA.MaxPrice,CMSSWA.AllocatedCount,CMSSWA.FreeTrialAllocatedCount,CMSSWA.TrainerCode,CMSPLC.PlanCost,CMSS.SlotStartTime,CMSS.SlotEndTime) varilable   ORDER BY LEN(Duration),Duration, SlotCode asc";
            }

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }


            DataRow row;
            DataColumn col1 = new DataColumn("SNO", typeof(int));
            DataColumn col2 = new DataColumn("SlotCode", typeof(string));
            DataColumn col3 = new DataColumn("SlotStartTime", typeof(string));
            DataColumn col4 = new DataColumn("SlotEndTime", typeof(string));
            DataColumn col5 = new DataColumn("SessionCode", typeof(string));
            DataColumn col6 = new DataColumn("PackageCode", typeof(string));
            DataColumn col7 = new DataColumn("RegularMembersPerSlot", typeof(int));
            DataColumn col8 = new DataColumn("RegularMembersFilledSlots", typeof(int));
            DataColumn col9 = new DataColumn("IsRegularSlotAvailable", typeof(int));
            DataColumn col10 = new DataColumn("FreeMembersPerSlot", typeof(int));
            DataColumn col11 = new DataColumn("FreeMembersFilledSlots", typeof(int));
            DataColumn col12 = new DataColumn("IsTrialSlotAvailable", typeof(int));
            DataColumn col13 = new DataColumn("PlanCost", typeof(double));
            DataColumn col14 = new DataColumn("Duration", typeof(string));
            DataColumn col15 = new DataColumn("NoOfDays", typeof(int));
            DataColumn col16 = new DataColumn("DurationCode", typeof(string));
            DataColumn col17 = new DataColumn("PlanCode", typeof(string));
            DataColumn col18 = new DataColumn("PlanName", typeof(string));
            DataColumn col19 = new DataColumn("TrainersCode", typeof(string));
            DataColumn col20 = new DataColumn("TrainersName", typeof(string));
            DataColumn col21 = new DataColumn("SlotPrice", typeof(double));
            DataColumn col22 = new DataColumn("PlanCostCode", typeof(string));
            DataColumn col23 = new DataColumn("MinPlanCost", typeof(float));
            DataColumn col24 = new DataColumn("MaxPlanCost", typeof(float));
            DataColumn col25 = new DataColumn("Date", typeof(string));
            DataColumn col26 = new DataColumn("DiscountAmount", typeof(float));
            DataColumn col27 = new DataColumn("DiscountPercentage", typeof(float));
            DataColumn col28 = new DataColumn("PerMonthPrice", typeof(float));


            dt_Slotav.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8, col9, col10, col11, col12, col13, col14, col15, col16, col17, col18, col19, col20, col21, col22, col23, col24, col25, col26, col27, col28 });

            for (int j = 0; j <= ds_custdet1.Tables[0].Rows.Count; j++)
            {
                try
                {
                    row = dt_Slotav.NewRow();
                    row["SNO"] = j + 1;

                    row["SlotCode"] = ds_custdet1.Tables[0].Rows[j]["SlotCode"].ToString();
                    row["SessionCode"] = ds_custdet1.Tables[0].Rows[j]["SessionCode"].ToString();
                    row["PackageCode"] = ds_custdet1.Tables[0].Rows[j]["PackageCode"].ToString();
                    row["RegularMembersPerSlot"] = ds_custdet1.Tables[0].Rows[j]["RegularAllocatedCount"].ToString();
                    row["RegularMembersFilledSlots"] = ds_custdet1.Tables[0].Rows[j]["RegularUsedCount"].ToString();
                    row["IsRegularSlotAvailable"] = "0";
                    row["FreeMembersPerSlot"] = ds_custdet1.Tables[0].Rows[j]["FreeTrialAllocatedCount"].ToString();
                    row["FreeMembersFilledSlots"] = ds_custdet1.Tables[0].Rows[j]["FreeTrialUsedCount"].ToString();
                    row["IsTrialSlotAvailable"] = "1";
                    row["PlanCost"] = ds_custdet1.Tables[0].Rows[j]["PlanCost"].ToString();

                    row["SlotPrice"] = ds_custdet1.Tables[0].Rows[j]["MinPlanCost"].ToString();
                    row["MinPlanCost"] = ds_custdet1.Tables[0].Rows[j]["MinPlanCost"].ToString();
                    row["MaxPlanCost"] = ds_custdet1.Tables[0].Rows[j]["MaxPlanCost"].ToString();
                    row["Duration"] = ds_custdet1.Tables[0].Rows[j]["Duration"].ToString();
                    row["NoOfDays"] = Convert.ToInt32(ds_custdet1.Tables[0].Rows[j]["Duration"].ToString()) * 30;
                    row["DurationCode"] = ds_custdet1.Tables[0].Rows[j]["DurationId"].ToString();
                    row["PlanCode"] = ds_custdet1.Tables[0].Rows[j]["PlanCode"].ToString();
                    row["PlanName"] = ds_custdet1.Tables[0].Rows[j]["PlanName"].ToString();
                    row["PlanCostCode"] = ds_custdet1.Tables[0].Rows[j]["PlanCostCode"].ToString();
                    TrainerCode = ds_custdet1.Tables[0].Rows[j]["TrainerCode"].ToString();
                    DataRow[] results = dt_Trainer.Select("TrainerCode =" + ds_custdet1.Tables[0].Rows[j]["TrainerCode"].ToString() + " ");
                    foreach (DataRow row1 in results)
                    {
                        TrainerCode = row1["TrainerCode"].ToString();
                        TrainerName = row1["TrainerName"].ToString();
                        break;
                    }
                    try
                    {
                        row["TrainersCode"] = TrainerCode;
                        row["TrainersName"] = TrainerName;
                    }
                    catch (Exception ec)
                    {

                    }


                    row["SlotStartTime"] = ds_custdet1.Tables[0].Rows[j]["SlotStartTime"].ToString();
                    row["SlotEndTime"] = ds_custdet1.Tables[0].Rows[j]["SlotEndTime"].ToString();

                    //row["SlotStartTime"] = "";
                    //row["SlotEndTime"] = "";
                    //row["Date"] = DBNull.Value;
                    row["DiscountAmount"] = 0.0f;
                    row["DiscountPercentage"] = 0.0f;
                    row["PerMonthPrice"] = 0.0f;


                    //  row.AcceptChanges();
                    dt_Slotav.Rows.Add(row);
                }
                catch (Exception ec)
                {

                }

            }

            return dt_Slotav;
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

        /// <summary>
        /// 0-13 : 14-27 : 28 - 41  : 42 - 55 : 56 - 70 : 71 - 84
        /// </summary>
        /// <param name="PackagePrices"></param>
        /// <returns></returns>
        public string OffLineDynamicPriceSplit([FromBody]OffLineCMSSlotsAvailability PackagePrices)
        {

            string sJSONResponse = "";
            var Percentage = "";
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_slots = new DataSet();
            string result = string.Empty;


            double x = 0.0f;
            double y = 0.0f;
            double z = 0.0f;

            int count = 0;
            int quotient = 0;
            int cntd = 0;

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

                    dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM OffLineSlotUsed   WHERE  MobileNo='{0}')  SELECT (SELECT top 1 T1.EnquireTypeNo FROM OffLineSlotUsed AS T1,Users T2  WHERE T2.MobileNo='{0}'  order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", PackagePrices.MobileNo));
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

            for (int i = PackagePrices.StartNo; i <= PackagePrices.EndNo; i++)
            {
                //date1 = currentday.AddDays(i).ToShortDateString();
                //date = System.DateTime.Parse(date1);

                date1 = PackagePrices.StartDate;
                date = System.DateTime.Parse(date1).AddDays(i);

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
                        //count = i + 7;
                        //quotient = i / 7;
                        //DataRow[] results = dt_DailyPricePercentage.Select("Days = " + quotient + " AND DaysDuration= 'Week' ");
                        //foreach (DataRow row in results)
                        //{
                        //    Percentage = row["Percentage"].ToString();
                        //    break;
                        //}

                    }
                }


                dt_Slotused = SlotAvailablility(PackagePrices.BranchCode, PackagePrices.PackageCode, Convert.ToString(date));

                foreach (DataRow slurow in dt_Slotused.Rows)
                {
                    string val = date.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    slurow["Date"] = date.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    //slurow["Date"] = date;


                    AvailableSlots = Convert.ToInt32(slurow["RegularMembersPerSlot"]);
                    FilledSlots = Convert.ToInt32(slurow["RegularMembersFilledSlots"].ToString());
                    PlanCost = Convert.ToInt32(slurow["PlanCost"].ToString());
                    SlotPrice = Convert.ToInt32(slurow["SlotPrice"].ToString());
                    MinimumAmount = Convert.ToInt32(slurow["MinPlanCost"].ToString());
                    MaximumAmount = Convert.ToInt32(slurow["MaxPlanCost"].ToString());


                    try
                    {
                        capacityPercentage = Convert.ToDouble(FilledSlots) / Convert.ToDouble(AvailableSlots);
                        SlotCapacity = Math.Round(capacityPercentage * 100);
                    }
                    catch (Exception ec)
                    {

                    }

                    count = i + 1;
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

                        PercentageConverter = float.Parse(Percentage) / 100;
                        SlotPrice = (MinimumAmount * PercentageConverter) + (MinimumAmount);
                    }

                    try
                    {
                        slurow["SlotPrice"] = Math.Round(SlotPrice);
                    }
                    catch (Exception ec)
                    {
                        slurow["SlotPrice"] = 0.0f;
                    }

                    if (FilledSlots < AvailableSlots)
                        slurow["IsRegularSlotAvailable"] = 1;
                    else if (FilledSlots == AvailableSlots)
                        slurow["IsRegularSlotAvailable"] = 0;
                    else
                        slurow["IsRegularSlotAvailable"] = 0;

                    string DurationId = slurow["DurationCode"].ToString();
                    int Duration = Convert.ToInt32(slurow["Duration"].ToString());
                    char[] strGoal = DurationId.ToCharArray();


                    if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "W")
                    {
                        if (Duration == 1)
                        {
                            slurow["Duration"] = Duration + " Week";
                        }
                        else
                        {
                            slurow["Duration"] = Duration + " Week";
                        }

                    }
                    else if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "D")
                    {
                        if (Duration == 1)
                        {
                            slurow["Duration"] = Duration + " Day";
                        }
                        else
                        {
                            slurow["Duration"] = Duration + " Days";
                        }

                    }
                    else if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "M")
                    {
                        if (Duration == 1)
                        {
                            slurow["Duration"] = Duration + " Month";
                        }
                        else
                        {
                            slurow["Duration"] = Duration + " Months";
                        }

                    }
                    else if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "M")
                    {
                        if (Duration == 3)
                        {
                            slurow["Duration"] = Duration + " Month";
                        }
                        else
                        {
                            slurow["Duration"] = Duration + " Months";
                        }

                    }
                    else if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "M")
                    {
                        if (Duration == 6)
                        {
                            slurow["Duration"] = Duration + " Month";
                        }
                        else
                        {
                            slurow["Duration"] = Duration + " Months";
                        }

                    }
                    else if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "M")
                    {
                        if (Duration == 12)
                        {
                            slurow["Duration"] = Duration + " Month";
                        }
                        else
                        {
                            slurow["Duration"] = Duration + " Months";
                        }

                    }
                    else
                    {
                        if (Duration == 1)
                        {
                            slurow["Duration"] = Duration + " ";
                        }
                        else
                        {
                            slurow["Duration"] = Duration + " ";
                        }

                    }

                    var da = Math.Round((PlanCost - SlotPrice));
                    var dp = Math.Round(((da / PlanCost) * 100));
                    var pmp = Math.Round((SlotPrice / Duration));

                    slurow["DiscountAmount"] = Math.Round((PlanCost - SlotPrice));
                    slurow["DiscountPercentage"] = Math.Round(((da / PlanCost) * 100));
                    slurow["PerMonthPrice"] = Math.Round((SlotPrice / Duration));

                }

                dt_temptable.Merge(dt_Slotused);
            }

            int sloutcount = dt_temptable.Rows.Count;
            // dt_temptable.DefaultView.Sort = "SlotStartTime ASC";
            DataTable st_newSlotavailable = new DataTable();
            DataTable st_newSlotavailableAll = new DataTable();
            st_newSlotavailable = dt_temptable.DefaultView.ToTable();
            st_newSlotavailableAll = dt_temptable.DefaultView.ToTable();
            DataTable dt_FilteredSlotsAvailable = new DataTable();
            int count2 = st_newSlotavailable.Rows.Count;
            int count3 = 0;
            //
            DataTable dt_filter = new DataTable();
            DataTable dt_datewise = new DataTable();
            string date2 = "";

            date1 = currentday.AddDays(-7).ToShortDateString();
            date2 = currentday.AddDays(0).ToShortDateString();
            date = System.DateTime.Parse(date1);

            string Fromdate = date1 + " 00:00:00";
            string Todate = date2 + " 23:59:59";

            DataRow[] rows = st_newSlotavailable.Select("Date >= #" + Fromdate + "# AND Date <= #" + Todate + "#");

            if (rows.Length > 0)
            {
                dt_datewise = rows.CopyToDataTable();
            }

            count3 = dt_datewise.Rows.Count;
            //
            List<OffLineEnquireInfo> olei = new List<OffLineEnquireInfo>();
            //Register='Enroll'
            olei.Add(new OffLineEnquireInfo { isFreeTrial = EnquiretypeNo, enquireTypeNo = 2, Register = "Enroll" });

            List<OffLineOnlySlotsPackageDetails> dt_SlotPackageDopt = new List<OffLineOnlySlotsPackageDetails>();
            OffLineOnlySlotsPackageDetails dashboardlistpackage = new OffLineOnlySlotsPackageDetails { info = olei, dateWisePrice = GetSevenDayDuration(st_newSlotavailableAll, PackagePrices.BranchCode, PackagePrices.PackageCode, PackagePrices.MobileNo, PackagePrices.MembershipCode,PackagePrices.StartDate), slots = GetOnlySlotsDetails(st_newSlotavailableAll, PackagePrices.StartDate, PackagePrices.BranchCode, PackagePrices.PackageCode), session = GetSessionDetails(st_newSlotavailable, PackagePrices.BranchCode, PackagePrices.PackageCode) };

            // OffLineOnlySlotsPackageDetails dashboardlistpackage = new OffLineOnlySlotsPackageDetails { info = olei, dateWisePrice = GetSevenDayDuration(PackagePrices.BranchCode, PackagePrices.PackageCode, PackagePrices.MobileNo, PackagePrices.MembershipCode)};
            dt_SlotPackageDopt.Add(dashboardlistpackage);

            OffLineOnlySlotsPackageOutPut olppFOP = new OffLineOnlySlotsPackageOutPut();

            olppFOP.status = "Success";
            olppFOP.value = dt_SlotPackageDopt;
            sJSONResponse = JsonConvert.SerializeObject(olppFOP);


            return sJSONResponse;

        }
        //{"BranchCode":"1101","PackageCode":"P24","StartDate":"03\/30\/2021","MemberShipCode":"","Register":0,"MobileNo":"8686378737"}
        public string OffLineDynamicPrice([FromBody]OffLineCMSSlotsAvailability PackagePrices)
        {
            string sJSONResponse = "";
            var Percentage = "";
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_slots = new DataSet();
            string result = string.Empty;
            DataTable dt_DailyPricePercentage = new DataTable();

            double x = 0.0f;
            double y = 0.0f;
            double z = 0.0f;

            int count = 0;
            int quotient = 0;
            int cntd = 0;

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
            //,
            if (PackagePrices.Register == "0")
            { dt_DailyPricePercentage = getdata(string.Format("select Days,DaysDuration,Percentage as Percentage from DailyPricePercentage", "")); }
            else if (PackagePrices.Register == "1")
            { dt_DailyPricePercentage = getdata(string.Format("select Days,DaysDuration,PrePercentage as Percentage from DailyPricePercentage", "")); }
            else if (PackagePrices.Register == "2")
            { dt_DailyPricePercentage = getdata(string.Format("select Days,DaysDuration,PostPercentage as Percentage from DailyPricePercentage", "")); }
            else
            { }


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

                    dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM OffLineSlotUsed   WHERE  MobileNo='{0}')  SELECT (SELECT top 1 T1.EnquireTypeNo FROM OffLineSlotUsed AS T1,Users T2  WHERE T2.MobileNo='{0}'  order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", PackagePrices.MobileNo));
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
            // current date changed to given date
            DateTime currentday = DateTime.Now;

            // DateTime currentday = Convert.ToDateTime(PackagePrices.StartDate).AddDays(-1);
            DateTime firstDayThisMonth = new DateTime(currentday.Year, currentday.Month, 1);
            DateTime firstDayPlusTwoMonths = firstDayThisMonth.AddMonths(2);
            DateTime lastDayNextMonth = firstDayPlusTwoMonths.AddDays(-1);
            DateTime endOfLastDayNextMonth = firstDayPlusTwoMonths.AddTicks(-1);

            int days = Convert.ToInt32((endOfLastDayNextMonth - currentday).TotalDays);
            string date1 = null;
            DateTime? date = null;
            // days loops start

            for (int i = 0; i <= 60; i++)
            {
                //date1 = currentday.AddDays(i).ToShortDateString();
                //date = System.DateTime.Parse(date1);

                date1 = PackagePrices.StartDate;
                date = System.DateTime.Parse(date1).AddDays(i);

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

                dt_Slotused = SlotAvailablility(PackagePrices.BranchCode, PackagePrices.PackageCode, Convert.ToString(date));

                foreach (DataRow slurow in dt_Slotused.Rows)
                {
                    string val = date.Value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                    slurow["Date"] = date.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                    //slurow["Date"] = date;


                    AvailableSlots = Convert.ToInt32(slurow["RegularMembersPerSlot"]);
                    FilledSlots = Convert.ToInt32(slurow["RegularMembersFilledSlots"].ToString());
                    PlanCost = Convert.ToInt32(slurow["PlanCost"].ToString());
                    SlotPrice = Convert.ToInt32(slurow["SlotPrice"].ToString());
                    MinimumAmount = Convert.ToInt32(slurow["MinPlanCost"].ToString());
                    MaximumAmount = Convert.ToInt32(slurow["MaxPlanCost"].ToString());


                    try
                    {
                        capacityPercentage = Convert.ToDouble(FilledSlots) / Convert.ToDouble(AvailableSlots);
                        SlotCapacity = Math.Round(capacityPercentage * 100);
                    }
                    catch (Exception ec)
                    {

                    }

                    count = i + 1;
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

                        PercentageConverter = float.Parse(Percentage) / 100;
                        SlotPrice = (MinimumAmount * PercentageConverter) + (MinimumAmount);
                    }

                    try
                    {
                        slurow["SlotPrice"] = Math.Round(SlotPrice);
                    }
                    catch (Exception ec)
                    {
                        slurow["SlotPrice"] = 0.0f;
                    }

                    if (FilledSlots < AvailableSlots)
                        slurow["IsRegularSlotAvailable"] = 1;
                    else if (FilledSlots == AvailableSlots)
                        slurow["IsRegularSlotAvailable"] = 0;
                    else
                        slurow["IsRegularSlotAvailable"] = 0;

                    string DurationId = slurow["DurationCode"].ToString();
                    int Duration = Convert.ToInt32(slurow["Duration"].ToString());
                    char[] strGoal = DurationId.ToCharArray();


                    if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "W")
                    {
                        if (Duration == 1)
                        {
                            slurow["Duration"] = Duration + " Week";
                        }
                        else
                        {
                            slurow["Duration"] = Duration + " Week";
                        }

                    }
                    else if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "D")
                    {
                        if (Duration == 1)
                        {
                            slurow["Duration"] = Duration + " Day";
                        }
                        else
                        {
                            slurow["Duration"] = Duration + " Days";
                        }

                    }
                    else if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "M")
                    {
                        if (Duration == 1)
                        {
                            slurow["Duration"] = Duration + " Month";
                        }
                        else
                        {
                            slurow["Duration"] = Duration + " Months";
                        }

                    }
                    else if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "M")
                    {
                        if (Duration == 3)
                        {
                            slurow["Duration"] = Duration + " Month";
                        }
                        else
                        {
                            slurow["Duration"] = Duration + " Months";
                        }

                    }
                    else if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "M")
                    {
                        if (Duration == 6)
                        {
                            slurow["Duration"] = Duration + " Month";
                        }
                        else
                        {
                            slurow["Duration"] = Duration + " Months";
                        }

                    }
                    else if (strGoal[0].ToString() == "D" && strGoal[1].ToString() == "M")
                    {
                        if (Duration == 12)
                        {
                            slurow["Duration"] = Duration + " Month";
                        }
                        else
                        {
                            slurow["Duration"] = Duration + " Months";
                        }

                    }
                    else
                    {
                        if (Duration == 1)
                        {
                            slurow["Duration"] = Duration + " ";
                        }
                        else
                        {
                            slurow["Duration"] = Duration + " ";
                        }

                    }

                    var da = Math.Round((PlanCost - SlotPrice));
                    var dp = Math.Round(((da / PlanCost) * 100));
                    var pmp = Math.Round((SlotPrice / Duration));

                    slurow["DiscountAmount"] = Math.Round((PlanCost - SlotPrice));
                    slurow["DiscountPercentage"] = Math.Round(((da / PlanCost) * 100));
                    slurow["PerMonthPrice"] = Math.Round((SlotPrice / Duration));

                }

                dt_temptable.Merge(dt_Slotused);
            }

            int sloutcount = dt_temptable.Rows.Count;
            // dt_temptable.DefaultView.Sort = "SlotStartTime ASC";
            DataTable st_newSlotavailable = new DataTable();
            DataTable st_newSlotavailableAll = new DataTable();
            st_newSlotavailable = dt_temptable.DefaultView.ToTable();
            st_newSlotavailableAll = dt_temptable.DefaultView.ToTable();
            DataTable dt_FilteredSlotsAvailable = new DataTable();
            int count2 = st_newSlotavailable.Rows.Count;
            int count3 = 0;
            //
            DataTable dt_filter = new DataTable();
            DataTable dt_datewise = new DataTable();
            string date2 = "";

            date1 = currentday.AddDays(-7).ToShortDateString();
            date2 = currentday.AddDays(0).ToShortDateString();
            date = System.DateTime.Parse(date1);

            string Fromdate = date1 + " 00:00:00";
            string Todate = date2 + " 23:59:59";

            DataRow[] rows = st_newSlotavailable.Select("Date >= #" + Fromdate + "# AND Date <= #" + Todate + "#");

            if (rows.Length > 0)
            {
                dt_datewise = rows.CopyToDataTable();
            }

            count3 = dt_datewise.Rows.Count;
            //
            List<OffLineEnquireInfo> olei = new List<OffLineEnquireInfo>();
            //Register='Enroll'
            olei.Add(new OffLineEnquireInfo { isFreeTrial = EnquiretypeNo, enquireTypeNo = 2, Register = "Enroll" });

            List<OffLineOnlySlotsPackageDetails> dt_SlotPackageDopt = new List<OffLineOnlySlotsPackageDetails>();
            OffLineOnlySlotsPackageDetails dashboardlistpackage = new OffLineOnlySlotsPackageDetails { info = olei, dateWisePrice = GetSevenDayDuration(st_newSlotavailableAll, PackagePrices.BranchCode, PackagePrices.PackageCode, PackagePrices.MobileNo, PackagePrices.MembershipCode, PackagePrices.StartDate), slots = GetOnlySlotsDetails(st_newSlotavailableAll, PackagePrices.StartDate, PackagePrices.BranchCode, PackagePrices.PackageCode), session = GetSessionDetails(st_newSlotavailable, PackagePrices.BranchCode, PackagePrices.PackageCode) };

            // OffLineOnlySlotsPackageDetails dashboardlistpackage = new OffLineOnlySlotsPackageDetails { info = olei, dateWisePrice = GetSevenDayDuration(PackagePrices.BranchCode, PackagePrices.PackageCode, PackagePrices.MobileNo, PackagePrices.MembershipCode)};
            dt_SlotPackageDopt.Add(dashboardlistpackage);

            OffLineOnlySlotsPackageOutPut olppFOP = new OffLineOnlySlotsPackageOutPut();

            olppFOP.status = "Success";
            olppFOP.value = dt_SlotPackageDopt;
            sJSONResponse = JsonConvert.SerializeObject(olppFOP);


            return sJSONResponse;

        }
        public string GetSevenDayDuration2([FromBody]OffLineCMSSlotsAvailability PackagePrices)
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

            double x = 0.0f;
            double y = 0.0f;
            double z = 0.0f;

            int count = 0;
            int quotient = 0;
            int cntd = 0;
            try
            {

                for (int i = 0; i <= 60 - 2; i++)
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
                    dt_Slotused = SlotAvailablility(PackagePrices.BranchCode, PackagePrices.PackageCode, Convert.ToString(date));
                    foreach (DataRow slurow in dt_Slotused.Rows)
                    {
                        slurow["Date"] = date;


                        AvailableSlots = Convert.ToInt32(slurow["RegularMembersPerSlot"]);
                        FilledSlots = Convert.ToInt32(slurow["RegularMembersFilledSlots"].ToString());
                        PlanCost = Convert.ToInt32(slurow["PlanCost"].ToString());
                        SlotPrice = Convert.ToInt32(slurow["SlotPrice"].ToString());
                        MinimumAmount = Convert.ToInt32(slurow["MinPlanCost"].ToString());
                        MaximumAmount = Convert.ToInt32(slurow["MaxPlanCost"].ToString());


                        try
                        {
                            capacityPercentage = Convert.ToDouble(FilledSlots) / Convert.ToDouble(AvailableSlots);
                            SlotCapacity = Math.Round(capacityPercentage * 100);
                        }
                        catch (Exception ec)
                        {

                        }


                        // DataRow[] results = dt_DailyPricePercentage.Select("Days = " + count + " ");
                        //foreach (DataRow row in results)
                        //{
                        //    Percentage = row["Percentage"].ToString();
                        //    break;
                        //}

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

                            PercentageConverter = float.Parse(Percentage) / 100;
                            SlotPrice = (MinimumAmount * PercentageConverter) + (MinimumAmount);
                        }

                        try
                        {
                            slurow["SlotPrice"] = Math.Round(SlotPrice);
                        }
                        catch (Exception ec)
                        {
                            slurow["SlotPrice"] = 0.0f;
                        }

                        if (FilledSlots < AvailableSlots)
                            slurow["IsRegularSlotAvailable"] = 1;
                        else if (FilledSlots == AvailableSlots)
                            slurow["IsRegularSlotAvailable"] = 0;
                        else
                            slurow["IsRegularSlotAvailable"] = 0;

                        int Duration = Convert.ToInt32(slurow["Duration"].ToString());

                        var da = Math.Round((PlanCost - SlotPrice));
                        var dp = Math.Round(((da / PlanCost) * 100));
                        var pmp = Math.Round((SlotPrice / Duration));


                        slurow["DiscountAmount"] = Math.Round((PlanCost - SlotPrice));
                        slurow["DiscountPercentage"] = Math.Round(((da / PlanCost) * 100));
                        slurow["PerMonthPrice"] = Math.Round((SlotPrice / Duration));

                    }

                    dt_temptable.Merge(dt_Slotused);

                }  // end of days loop

            }
            catch
            {

            }


            int sloutcount = dt_temptable.Rows.Count;
            //dt_temptable.DefaultView.Sort = "SlotStartTime ASC";
            DataTable st_newSlotavailable = new DataTable();
            st_newSlotavailable = dt_temptable.DefaultView.ToTable();
            DataTable dt_FilteredSlotsAvailable = new DataTable();

            List<OffLineEnquireInfo> olei = new List<OffLineEnquireInfo>();
            olei.Add(new OffLineEnquireInfo { isFreeTrial = EnquiretypeNo, enquireTypeNo = 2 });

            List<OffLineDurationDateDetails> dt_SlotPackageDopt = new List<OffLineDurationDateDetails>();
            OffLineDurationDateDetails dashboardlistpackage = new OffLineDurationDateDetails { dateWisePrice = GetSlotPriceDurationDateWise(st_newSlotavailable) };
            dt_SlotPackageDopt.Add(dashboardlistpackage);

            OffLineDurationDateOutPut olppFOP = new OffLineDurationDateOutPut();

            olppFOP.status = "Success";
            olppFOP.value = dt_SlotPackageDopt;
            sJSONResponse = JsonConvert.SerializeObject(olppFOP);


            return sJSONResponse;

        }
        public List<OffLineDurationDateWise> GetSevenDayDuration(DataTable dt, string BranchCode, string PackageCode, string MobileNo, string MembershipCode, string Datetime)
        {

            var Percentage = "";
            string sJSONResponse = "";

            //ArrayList arl_stlistA = new ArrayList();
            //ArrayList arl_stlistB = new ArrayList();

            //string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            //DataSet ds_slots = new DataSet();
            //string result = string.Empty;

            //DataTable dt_DailyPricePercentage = getdata(string.Format("select Days,DaysDuration,Percentage from DailyPricePercentage", ""));

            //int AvailableSlots = 0;
            //int FilledSlots = 0;
            //float PlanCost = 0.0f;
            //double SlotPrice = 0.0f;
            //float MinimumAmount = 0.0f;
            //float MaximumAmount = 0.0f;
            //float Constant = 0.0f;
            //double SlotCapacity = 0.0f;
            //double capacityPercentage = 0.0f;
            //DataTable dt_IsPackage = new DataTable();


            //int EnquiretypeNo = 0;
            //DataTable dt_Slotavailable = new DataTable();
            //DataTable dt_Slotused = new DataTable();
            //DataTable dt_Slot = new DataTable();

            //ArrayList arl_Mobile = MobileCheck(MobileNo);
            //int EnquireformCount = Convert.ToInt32(arl_Mobile[0].ToString());
            //int UserCount = Convert.ToInt32(arl_Mobile[1].ToString());

            //DateTime currentday = DateTime.Now;
            //DateTime firstDayThisMonth = new DateTime(currentday.Year, currentday.Month, 1);
            //DateTime firstDayPlusTwoMonths = firstDayThisMonth.AddMonths(2);
            //DateTime lastDayNextMonth = firstDayPlusTwoMonths.AddDays(-1);
            //DateTime endOfLastDayNextMonth = firstDayPlusTwoMonths.AddTicks(-1);

            //string UniqueId = "";

            //try
            //{

            //    if (MobileNo != null && MembershipCode != "")
            //    {
            //        UniqueId = MobileNo;
            //    }
            //    else if (MobileNo != "" && MembershipCode == "")
            //    {
            //        UniqueId = MobileNo;
            //    }
            //    else
            //    {
            //        UniqueId = "";
            //    }

            //    DataTable dt = getdata(string.Format("select ID,DisplayName from DashboardDisplay", ""));


            //    if (UniqueId != "")
            //    {

            //        dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM CCRMMembership AS T1,Users T2   WHERE  T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode)  SELECT (SELECT top 1 T1.EnquireTypeNo FROM CCRMMembership AS T1,Users T2  WHERE T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", MobileNo));
            //        EnquiretypeNo = Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString());
            //    }
            //    else
            //    {
            //        // dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM CCRMMembership AS T1,Users T2   WHERE   T1.MembershipCode=T2.UCode)  SELECT (SELECT top 1 T1.EnquireTypeNo FROM CCRMMembership AS T1,Users T2  WHERE T1.MembershipCode=T2.UCode order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", ""));
            //        EnquiretypeNo = 0;
            //    }

            //    List<OffLinePackagePriceList> dbPackage = new List<OffLinePackagePriceList>();
            //    try
            //    {
            //        if (EnquiretypeNo == 0)
            //        {
            //            EnquiretypeNo = 1;
            //        }
            //        else if (EnquiretypeNo > 0)
            //        {
            //            EnquiretypeNo = 0;

            //        }
            //        else
            //        {
            //            EnquiretypeNo = 1;
            //        }
            //    }
            //    catch (Exception ec)
            //    {
            //    }
            //}
            //catch (Exception ec)
            //{

            //}

            ////CreateDatatable
            //DataTable dt_temptable = new DataTable();

            //// days calculation
            //int days = Convert.ToInt32((endOfLastDayNextMonth - currentday).TotalDays);
            //string date1 = null;
            //DateTime? date = null;
            //// days loops start

            //double x = 0.0f;
            //double y = 0.0f;
            //double z = 0.0f;

            //int count = 0;
            //int quotient = 0;
            //int cntd = 0;
            //try
            //{

            //    for (int i = 0; i <= 60 - 2; i++)
            //    {
            //        date1 = currentday.AddDays(i).ToShortDateString();
            //        date = System.DateTime.Parse(date1);
            //        cntd = i + 1;
            //        if (i <= 6)
            //        {
            //            DataRow[] results = dt_DailyPricePercentage.Select("Days = " + cntd + " AND DaysDuration= 'Day' ");
            //            foreach (DataRow row in results)
            //            {
            //                Percentage = row["Percentage"].ToString();
            //                break;
            //            }
            //        }
            //        else
            //        {
            //            float rmd = i % 7;
            //            if (rmd == 0)
            //            {
            //                count = i + 7;
            //                quotient = i / 7;
            //                DataRow[] results = dt_DailyPricePercentage.Select("Days = " + quotient + " AND DaysDuration= 'Week' ");
            //                foreach (DataRow row in results)
            //                {
            //                    Percentage = row["Percentage"].ToString();
            //                    break;
            //                }


            //            }
            //            else
            //            {

            //            }
            //        }

            //        dt_Slotused = SlotAvailablility(BranchCode, PackageCode, Convert.ToString(date));
            //        foreach (DataRow slurow in dt_Slotused.Rows)
            //        {
            //            string ServerDateTime1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            //            slurow["Date"] = date.Value.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            //           // slurow["Date"] = date.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            //               // date;


            //            AvailableSlots = Convert.ToInt32(slurow["RegularMembersPerSlot"]);
            //            FilledSlots = Convert.ToInt32(slurow["RegularMembersFilledSlots"].ToString());
            //            PlanCost = Convert.ToInt32(slurow["PlanCost"].ToString());
            //            SlotPrice = Convert.ToInt32(slurow["SlotPrice"].ToString());
            //            MinimumAmount = Convert.ToInt32(slurow["MinPlanCost"].ToString());
            //            MaximumAmount = Convert.ToInt32(slurow["MaxPlanCost"].ToString());


            //            try
            //            {
            //                capacityPercentage = Convert.ToDouble(FilledSlots) / Convert.ToDouble(AvailableSlots);
            //                SlotCapacity = Math.Round(capacityPercentage * 100);
            //            }
            //            catch (Exception ec)
            //            {

            //            }



            //            float PercentageConverter = float.Parse(Percentage) / 100;

            //            // day wise MinimumAmount calculation

            //            if (SlotCapacity > 0 && SlotCapacity <= 25)
            //            {
            //                SlotPrice = (MinimumAmount * PercentageConverter) + (MinimumAmount);
            //            }
            //            else if (SlotCapacity > 25 && SlotCapacity <= 50)
            //            {
            //                x = (MinimumAmount * PercentageConverter) + (MinimumAmount);

            //                SlotPrice = (x * PercentageConverter) + (MinimumAmount);
            //            }
            //            else if (SlotCapacity > 50 && SlotCapacity <= 75)
            //            {
            //                x = (MinimumAmount * PercentageConverter) + (MinimumAmount);
            //                y = (x * PercentageConverter) + (MinimumAmount);
            //                SlotPrice = (y * PercentageConverter) + (MinimumAmount);
            //            }
            //            else if (SlotCapacity > 75 && SlotCapacity <= 100)
            //            {
            //                x = (MinimumAmount * PercentageConverter) + (MinimumAmount);
            //                y = (x * PercentageConverter) + (MinimumAmount);
            //                z = (y * PercentageConverter) + (MinimumAmount);
            //                SlotPrice = (z * PercentageConverter) + (MinimumAmount);
            //            }
            //            else
            //            {

            //                PercentageConverter = float.Parse(Percentage) / 100;
            //                SlotPrice = (MinimumAmount * PercentageConverter) + (MinimumAmount);
            //            }

            //            try
            //            {
            //                slurow["SlotPrice"] = Math.Round(SlotPrice);
            //            }
            //            catch (Exception ec)
            //            {
            //                slurow["SlotPrice"] = 0.0f;
            //            }

            //            if (FilledSlots < AvailableSlots)
            //                slurow["IsRegularSlotAvailable"] = 1;
            //            else if (FilledSlots == AvailableSlots)
            //                slurow["IsRegularSlotAvailable"] = 0;
            //            else
            //                slurow["IsRegularSlotAvailable"] = 0;

            //            int Duration = Convert.ToInt32(slurow["Duration"].ToString());

            //            var da = Math.Round((PlanCost - SlotPrice));
            //            var dp = Math.Round(((da / PlanCost) * 100));
            //            var pmp = Math.Round((SlotPrice / Duration));


            //            slurow["DiscountAmount"] = Math.Round((PlanCost - SlotPrice));
            //            slurow["DiscountPercentage"] = Math.Round(((da / PlanCost) * 100));
            //            slurow["PerMonthPrice"] = Math.Round((SlotPrice / Duration));

            //        }

            //        dt_temptable.Merge(dt_Slotused);
            //    }  // end of days loop
            //}
            //catch
            //{

            //}

            string date1 = null;
            DateTime? date = null;
            DateTime currentday = DateTime.Now;

            //DateTime FromdateWeek = DateTime.Now.AddDays(-1);
            //DateTime ToDateWeek = DateTime.Now.AddDays(7);

            DateTime FromdateWeek = Convert.ToDateTime(Datetime).AddDays(1);
            DateTime ToDateWeek = Convert.ToDateTime(Datetime).AddDays(7);

            DataTable st_newSlotavailable = new DataTable();
            DataTable dt_filter = new DataTable();
            DataTable dt_datewise = new DataTable();
            DataTable dt_finaldata = new DataTable();
            dt_filter = dt;
            DataRow[] rows = dt_filter.Select("Date >= #" + FromdateWeek + "# AND Date <= #" + ToDateWeek + "#");

            if (rows.Length > 0)
            {
                dt_datewise = rows.CopyToDataTable();
            }

            List<OffLineDurationDateWise> DateWiseDurationPrice = new List<OffLineDurationDateWise>();

            for (int i = 0; i <= 7 - 1; i++)
            {
                try
                {
                    //date1 = currentday.AddDays(i).ToShortDateString();
                    //date = System.DateTime.Parse(date1);

                    date1 = Convert.ToDateTime(Datetime).AddDays(i).ToShortDateString();
                    date = System.DateTime.Parse(date1);

                    string Fromdate = date1 + " 00:00:00";
                    string Todate = date1 + " 23:59:59";

                    DataRow[] rows2 = dt_datewise.Select("Date >= #" + Fromdate + "# AND Date <= #" + Todate + "#");

                    if (rows2.Length > 0)
                    {
                        dt_finaldata = rows2.CopyToDataTable();
                    }
                    dt_finaldata = dt_finaldata.AsEnumerable().GroupBy(r => new { Col1 = r["DurationCode"] }).Select(g => g.OrderBy(r => r["SlotPrice"]).First()).CopyToDataTable();
                    DateWiseDurationPrice.Add(new OffLineDurationDateWise { date = date.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), slotPrice = GetSevenDayDurationColumns(dt_finaldata) });
                }
                catch (Exception ec)
                {

                }
            }

            return DateWiseDurationPrice;
        }
        public List<OffLineDurationWiseColums> GetSevenDayDurationColumns(DataTable st_newSlotavailable)
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
        public List<OffLineSessionPackage> GetSessionDetails(DataTable st_newSlotavailable, string BranchCode, string PackageCode)
        {

            List<OffLineSessionPackage> sessions = new List<OffLineSessionPackage>();
            DataTable dt_PackageSessions = getdata(string.Format("select SessionCode,SessionName,convert(varchar(10), SessionStartTime, 108) as SessionStartTime,convert(varchar(10), SessionEndTime, 108) as SessionEndTime from CMSSESSIONTIMESETTING", ""));

            DataTable dt_Sessions = st_newSlotavailable;
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
        public List<OffLineDurationWiseColums> GetSevenDaysDurationColumns(DataTable st_newSlotavailable)
        {

            DataTable dt_duratuonfilter = new DataTable();
            st_newSlotavailable = st_newSlotavailable.AsEnumerable().GroupBy(r => new { Col1 = r["DurationCode"] }).Select(g => { var row = st_newSlotavailable.NewRow(); row["SlotPrice"] = g.Min(r => r.Field<double>("SlotPrice")); row["DurationCode"] = g.Key.Col1; return row; }).CopyToDataTable();
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
        public List<OffLineSlotsPackage> GetOnlySlotsDetails(DataTable dt_SlotPackages, string Date, string BranchCode, string PackageCode)
        {
            DataTable dt_temptable = new DataTable();
            DataTable dt_DailyPricePercentage = getdata(string.Format("select Days,DaysDuration,Percentage from DailyPricePercentage", ""));
            DataTable dt_filter = new DataTable();
            string Startdatecombin = "";
            string Enddatecombin = "";
            string date1 = null;
            DateTime? date = null;
            DateTime StartDate = Convert.ToDateTime(Date);
            date1 = StartDate.AddDays(0).ToShortDateString();
            date = System.DateTime.Parse(date1);

            //string Fromdate = date1 + " 00:00:00";
            //string Todate = date1 + " 23:59:59";
            //DataRow[] rows = dt_SlotPackages.Select("Date >= #" + Fromdate + "# AND Date <= #" + Todate + "#");

            List<OffLineSlotsPackage> offLineSlotPackage = new List<OffLineSlotsPackage>();

            try
            {
                //  dt_SlotPackages = rows.CopyToDataTable();

                for (int i = 0; i < dt_SlotPackages.Rows.Count; i++)
                {
                    Startdatecombin = Convert.ToString(Convert.ToDateTime(dt_SlotPackages.Rows[i]["Date"].ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)) + " " + Convert.ToString(Convert.ToDateTime(dt_SlotPackages.Rows[i]["SlotStartTime"].ToString()).ToString("HH:mm:ss", CultureInfo.InvariantCulture));
                    Enddatecombin = Convert.ToString(Convert.ToDateTime(dt_SlotPackages.Rows[i]["Date"].ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)) + " " + Convert.ToString(Convert.ToDateTime(dt_SlotPackages.Rows[i]["SlotEndTime"].ToString()).ToString("HH:mm:ss", CultureInfo.InvariantCulture));


                    offLineSlotPackage.Add(new OffLineSlotsPackage
                    {
                        //add plancostcode


                        sNo = Convert.ToInt32(dt_SlotPackages.Rows[i]["SNO"])
                                          ,
                        date = Convert.ToString(Convert.ToDateTime(dt_SlotPackages.Rows[i]["Date"].ToString()).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture))
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
                        slotStartTime = Startdatecombin
                                           ,
                        slotEndTime = Enddatecombin

                                          //slotStartTime = Convert.ToString(Convert.ToDateTime(dt_SlotPackages.Rows[i]["SlotStartTime"]).ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture))
                                          //                   ,
                                          //slotEndTime = Convert.ToString(Convert.ToDateTime(dt_SlotPackages.Rows[i]["SlotEndTime"]).ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture))
                                          ,
                        discountAmount = float.Parse(dt_SlotPackages.Rows[i]["DiscountAmount"].ToString())
                                          ,
                        discountPercentage = float.Parse(dt_SlotPackages.Rows[i]["DiscountPercentage"].ToString())
                                          ,
                        perMonthPrice = float.Parse(dt_SlotPackages.Rows[i]["PerMonthPrice"].ToString())
                        ,
                        planCostCode = Convert.ToString(dt_SlotPackages.Rows[i]["PlanCostCode"].ToString())

                    });

                }

            }
            catch (Exception ec)
            {

            }



            return offLineSlotPackage;
        }
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

            double x = 0.0f;
            double y = 0.0f;
            double z = 0.0f;

            int count = 0;
            int quotient = 0;
            int cntd = 0;
            try
            {

                for (int i = 0; i <= 60 - 2; i++)
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
                    dt_Slotused = SlotAvailablility(PackagePrices.BranchCode, PackagePrices.PackageCode, Convert.ToString(date));
                    foreach (DataRow slurow in dt_Slotused.Rows)
                    {
                        slurow["Date"] = date.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);


                        AvailableSlots = Convert.ToInt32(slurow["RegularMembersPerSlot"]);
                        FilledSlots = Convert.ToInt32(slurow["RegularMembersFilledSlots"].ToString());
                        PlanCost = Convert.ToInt32(slurow["PlanCost"].ToString());
                        SlotPrice = Convert.ToInt32(slurow["SlotPrice"].ToString());
                        MinimumAmount = Convert.ToInt32(slurow["MinPlanCost"].ToString());
                        MaximumAmount = Convert.ToInt32(slurow["MaxPlanCost"].ToString());


                        try
                        {
                            capacityPercentage = Convert.ToDouble(FilledSlots) / Convert.ToDouble(AvailableSlots);
                            SlotCapacity = Math.Round(capacityPercentage * 100);
                        }
                        catch (Exception ec)
                        {

                        }


                        // DataRow[] results = dt_DailyPricePercentage.Select("Days = " + count + " ");
                        //foreach (DataRow row in results)
                        //{
                        //    Percentage = row["Percentage"].ToString();
                        //    break;
                        //}

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

                            PercentageConverter = float.Parse(Percentage) / 100;
                            SlotPrice = (MinimumAmount * PercentageConverter) + (MinimumAmount);
                        }

                        try
                        {
                            slurow["SlotPrice"] = Math.Round(SlotPrice);
                        }
                        catch (Exception ec)
                        {
                            slurow["SlotPrice"] = 0.0f;
                        }

                        if (FilledSlots < AvailableSlots)
                            slurow["IsRegularSlotAvailable"] = 1;
                        else if (FilledSlots == AvailableSlots)
                            slurow["IsRegularSlotAvailable"] = 0;
                        else
                            slurow["IsRegularSlotAvailable"] = 0;

                        int Duration = Convert.ToInt32(slurow["Duration"].ToString());

                        var da = Math.Round((PlanCost - SlotPrice));
                        var dp = Math.Round(((da / PlanCost) * 100));
                        var pmp = Math.Round((SlotPrice / Duration));


                        slurow["DiscountAmount"] = Math.Round((PlanCost - SlotPrice));
                        slurow["DiscountPercentage"] = Math.Round(((da / PlanCost) * 100));
                        slurow["PerMonthPrice"] = Math.Round((SlotPrice / Duration));

                    }

                    dt_temptable.Merge(dt_Slotused);

                }  // end of days loop

            }
            catch
            {

            }


            int sloutcount = dt_temptable.Rows.Count;
            //dt_temptable.DefaultView.Sort = "SlotStartTime ASC";
            DataTable st_newSlotavailable = new DataTable();
            st_newSlotavailable = dt_temptable.DefaultView.ToTable();
            DataTable dt_FilteredSlotsAvailable = new DataTable();

            List<OffLineEnquireInfo> olei = new List<OffLineEnquireInfo>();
            olei.Add(new OffLineEnquireInfo { isFreeTrial = EnquiretypeNo, enquireTypeNo = 2 });

            List<OffLineDurationDateDetails> dt_SlotPackageDopt = new List<OffLineDurationDateDetails>();
            OffLineDurationDateDetails dashboardlistpackage = new OffLineDurationDateDetails { dateWisePrice = GetSlotPriceDurationDateWise(st_newSlotavailable) };
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
                    date1 = currentday.AddDays(i+1).ToShortDateString();
                    date = System.DateTime.Parse(date1);

                    string Fromdate = date1 + " 00:00:00";
                    string Todate = date1 + " 23:59:59";

                    DataRow[] rows = st_newSlotavailable.Select("Date >= #" + Fromdate + "# AND Date <= #" + Todate + "#");

                    if (rows.Length > 0)
                    {
                        dt_datewise = rows.CopyToDataTable();
                    }
                    dt_filter = dt_datewise.AsEnumerable().GroupBy(r => new { Col1 = r["DurationCode"] }).Select(g => g.OrderBy(r => r["SlotPrice"]).First()).CopyToDataTable();
                    DateWiseDurationPrice.Add(new OffLineDurationDateWise { date = date.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), slotPrice = GetDurationColumns(dt_filter) });
                }
            }
            catch (Exception ec)
            {

            }
            return DateWiseDurationPrice;
        }
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