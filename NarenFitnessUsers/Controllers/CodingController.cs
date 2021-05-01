using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NarenFitnessUsers.Controllers
{
    public class CodingController : Controller
    {
        //public List<OffLineDurationDateWise> GetSevenDayDuration(string BranchCode, string PackageCode, string MobileNo, string MembershipCode)
        //{

        //    var Percentage = "";
        //    string sJSONResponse = "";

        //    ArrayList arl_stlistA = new ArrayList();
        //    ArrayList arl_stlistB = new ArrayList();

        //    string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        //    DataSet ds_slots = new DataSet();
        //    string result = string.Empty;

        //    DataTable dt_DailyPricePercentage = getdata(string.Format("select Days,DaysDuration,Percentage from DailyPricePercentage", ""));

        //    int AvailableSlots = 0;
        //    int FilledSlots = 0;
        //    float PlanCost = 0.0f;
        //    double SlotPrice = 0.0f;
        //    float MinimumAmount = 0.0f;
        //    float MaximumAmount = 0.0f;
        //    float Constant = 0.0f;
        //    double SlotCapacity = 0.0f;
        //    double capacityPercentage = 0.0f;
        //    DataTable dt_IsPackage = new DataTable();


        //    int EnquiretypeNo = 0;
        //    DataTable dt_Slotavailable = new DataTable();
        //    DataTable dt_Slotused = new DataTable();
        //    DataTable dt_Slot = new DataTable();

        //    ArrayList arl_Mobile = MobileCheck(MobileNo);
        //    int EnquireformCount = Convert.ToInt32(arl_Mobile[0].ToString());
        //    int UserCount = Convert.ToInt32(arl_Mobile[1].ToString());

        //    DateTime currentday = DateTime.Now;
        //    DateTime firstDayThisMonth = new DateTime(currentday.Year, currentday.Month, 1);
        //    DateTime firstDayPlusTwoMonths = firstDayThisMonth.AddMonths(2);
        //    DateTime lastDayNextMonth = firstDayPlusTwoMonths.AddDays(-1);
        //    DateTime endOfLastDayNextMonth = firstDayPlusTwoMonths.AddTicks(-1);

        //    string UniqueId = "";

        //    try
        //    {

        //        if (MobileNo != null && MembershipCode != "")
        //        {
        //            UniqueId = MobileNo;
        //        }
        //        else if (MobileNo != "" && MembershipCode == "")
        //        {
        //            UniqueId = MobileNo;
        //        }
        //        else
        //        {
        //            UniqueId = "";
        //        }

        //        DataTable dt = getdata(string.Format("select ID,DisplayName from DashboardDisplay", ""));


        //        if (UniqueId != "")
        //        {

        //            dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM CCRMMembership AS T1,Users T2   WHERE  T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode)  SELECT (SELECT top 1 T1.EnquireTypeNo FROM CCRMMembership AS T1,Users T2  WHERE T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", MobileNo));
        //            EnquiretypeNo = Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString());
        //        }
        //        else
        //        {
        //            // dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM CCRMMembership AS T1,Users T2   WHERE   T1.MembershipCode=T2.UCode)  SELECT (SELECT top 1 T1.EnquireTypeNo FROM CCRMMembership AS T1,Users T2  WHERE T1.MembershipCode=T2.UCode order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", ""));
        //            EnquiretypeNo = 0;
        //        }

        //        List<OffLinePackagePriceList> dbPackage = new List<OffLinePackagePriceList>();
        //        try
        //        {
        //            if (EnquiretypeNo == 0)
        //            {
        //                EnquiretypeNo = 1;
        //            }
        //            else if (EnquiretypeNo > 0)
        //            {
        //                EnquiretypeNo = 0;

        //            }
        //            else
        //            {
        //                EnquiretypeNo = 1;
        //            }
        //        }
        //        catch (Exception ec)
        //        {
        //        }
        //    }
        //    catch (Exception ec)
        //    {

        //    }

        //    //CreateDatatable
        //    DataTable dt_temptable = new DataTable();

        //    // days calculation
        //    int days = Convert.ToInt32((endOfLastDayNextMonth - currentday).TotalDays);
        //    string date1 = null;
        //    DateTime? date = null;
        //    // days loops start

        //    double x = 0.0f;
        //    double y = 0.0f;
        //    double z = 0.0f;

        //    int count = 0;
        //    int quotient = 0;
        //    int cntd = 0;
        //    try
        //    {

        //        for (int i = 0; i <= 60 - 2; i++)
        //        {
        //            date1 = currentday.AddDays(i).ToShortDateString();
        //            date = System.DateTime.Parse(date1);
        //            cntd = i + 1;
        //            if (i <= 6)
        //            {
        //                DataRow[] results = dt_DailyPricePercentage.Select("Days = " + cntd + " AND DaysDuration= 'Day' ");
        //                foreach (DataRow row in results)
        //                {
        //                    Percentage = row["Percentage"].ToString();
        //                    break;
        //                }
        //            }
        //            else
        //            {
        //                float rmd = i % 7;
        //                if (rmd == 0)
        //                {
        //                    count = i + 7;
        //                    quotient = i / 7;
        //                    DataRow[] results = dt_DailyPricePercentage.Select("Days = " + quotient + " AND DaysDuration= 'Week' ");
        //                    foreach (DataRow row in results)
        //                    {
        //                        Percentage = row["Percentage"].ToString();
        //                        break;
        //                    }


        //                }
        //                else
        //                {

        //                }
        //            }

        //            dt_Slotused = SlotAvailablility(BranchCode, PackageCode, Convert.ToString(date));
        //            foreach (DataRow slurow in dt_Slotused.Rows)
        //            {
        //                string ServerDateTime1 = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        //                slurow["Date"] = date.Value.ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        //                // slurow["Date"] = date.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        //                // date;


        //                AvailableSlots = Convert.ToInt32(slurow["RegularMembersPerSlot"]);
        //                FilledSlots = Convert.ToInt32(slurow["RegularMembersFilledSlots"].ToString());
        //                PlanCost = Convert.ToInt32(slurow["PlanCost"].ToString());
        //                SlotPrice = Convert.ToInt32(slurow["SlotPrice"].ToString());
        //                MinimumAmount = Convert.ToInt32(slurow["MinPlanCost"].ToString());
        //                MaximumAmount = Convert.ToInt32(slurow["MaxPlanCost"].ToString());


        //                try
        //                {
        //                    capacityPercentage = Convert.ToDouble(FilledSlots) / Convert.ToDouble(AvailableSlots);
        //                    SlotCapacity = Math.Round(capacityPercentage * 100);
        //                }
        //                catch (Exception ec)
        //                {

        //                }



        //                float PercentageConverter = float.Parse(Percentage) / 100;

        //                // day wise MinimumAmount calculation

        //                if (SlotCapacity > 0 && SlotCapacity <= 25)
        //                {
        //                    SlotPrice = (MinimumAmount * PercentageConverter) + (MinimumAmount);
        //                }
        //                else if (SlotCapacity > 25 && SlotCapacity <= 50)
        //                {
        //                    x = (MinimumAmount * PercentageConverter) + (MinimumAmount);

        //                    SlotPrice = (x * PercentageConverter) + (MinimumAmount);
        //                }
        //                else if (SlotCapacity > 50 && SlotCapacity <= 75)
        //                {
        //                    x = (MinimumAmount * PercentageConverter) + (MinimumAmount);
        //                    y = (x * PercentageConverter) + (MinimumAmount);
        //                    SlotPrice = (y * PercentageConverter) + (MinimumAmount);
        //                }
        //                else if (SlotCapacity > 75 && SlotCapacity <= 100)
        //                {
        //                    x = (MinimumAmount * PercentageConverter) + (MinimumAmount);
        //                    y = (x * PercentageConverter) + (MinimumAmount);
        //                    z = (y * PercentageConverter) + (MinimumAmount);
        //                    SlotPrice = (z * PercentageConverter) + (MinimumAmount);
        //                }
        //                else
        //                {

        //                    PercentageConverter = float.Parse(Percentage) / 100;
        //                    SlotPrice = (MinimumAmount * PercentageConverter) + (MinimumAmount);
        //                }

        //                try
        //                {
        //                    slurow["SlotPrice"] = Math.Round(SlotPrice);
        //                }
        //                catch (Exception ec)
        //                {
        //                    slurow["SlotPrice"] = 0.0f;
        //                }

        //                if (FilledSlots < AvailableSlots)
        //                    slurow["IsRegularSlotAvailable"] = 1;
        //                else if (FilledSlots == AvailableSlots)
        //                    slurow["IsRegularSlotAvailable"] = 0;
        //                else
        //                    slurow["IsRegularSlotAvailable"] = 0;

        //                int Duration = Convert.ToInt32(slurow["Duration"].ToString());

        //                var da = Math.Round((PlanCost - SlotPrice));
        //                var dp = Math.Round(((da / PlanCost) * 100));
        //                var pmp = Math.Round((SlotPrice / Duration));


        //                slurow["DiscountAmount"] = Math.Round((PlanCost - SlotPrice));
        //                slurow["DiscountPercentage"] = Math.Round(((da / PlanCost) * 100));
        //                slurow["PerMonthPrice"] = Math.Round((SlotPrice / Duration));

        //            }

        //            dt_temptable.Merge(dt_Slotused);
        //        }  // end of days loop
        //    }
        //    catch
        //    {

        //    }

        //    DateTime FromdateWeek = DateTime.Now.AddDays(0);
        //    DateTime ToDateWeek = DateTime.Now.AddDays(7);

        //    DataTable st_newSlotavailable = new DataTable();
        //    DataTable dt_filter = new DataTable();
        //    DataTable dt_datewise = new DataTable();
        //    DataTable dt_finaldata = new DataTable();
        //    dt_filter = dt_temptable;
        //    DataRow[] rows = dt_filter.Select("Date >= #" + FromdateWeek + "# AND Date <= #" + ToDateWeek + "#");

        //    if (rows.Length > 0)
        //    {
        //        dt_datewise = rows.CopyToDataTable();
        //    }

        //    List<OffLineDurationDateWise> DateWiseDurationPrice = new List<OffLineDurationDateWise>();

        //    for (int i = 0; i <= 7 - 1; i++)
        //    {
        //        try
        //        {
        //            date1 = currentday.AddDays(i).ToShortDateString();
        //            date = System.DateTime.Parse(date1);

        //            string Fromdate = date1 + " 00:00:00";
        //            string Todate = date1 + " 23:59:59";

        //            DataRow[] rows2 = dt_datewise.Select("Date >= #" + Fromdate + "# AND Date <= #" + Todate + "#");

        //            if (rows2.Length > 0)
        //            {
        //                dt_finaldata = rows2.CopyToDataTable();
        //            }
        //            dt_finaldata = dt_finaldata.AsEnumerable().GroupBy(r => new { Col1 = r["DurationCode"] }).Select(g => g.OrderBy(r => r["SlotPrice"]).First()).CopyToDataTable();
        //            DateWiseDurationPrice.Add(new OffLineDurationDateWise { date = date.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture), slotPrice = GetSevenDayDurationColumns(dt_finaldata) });
        //        }
        //        catch (Exception ec)
        //        {

        //        }
        //    }

        //    return DateWiseDurationPrice;
        //}
    }
}