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
using NarenFitnessUsers.Models.Attendence;
using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using System.Drawing;
using System.Collections;
using System.Web.Script.Serialization;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace NarenFitnessUsers.Controllers
{
    public class AttendenceController : Controller
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
        public Object DailyAttendencePost([FromBody]AttendenceInput ai)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";
            AttendenceOutput aop = new AttendenceOutput();

            try
            {
                cnn.Open();
                olPackage_Query = "insert into DailyAttendence(SourceId,OrgId,MachineId,UCode,UName,DateOfTransaction,CreatedOn) values('1','" + ai.BranchCode + "','" + ai.MachineId + "','" + ai.MembershipCode + "','" + ai.UserName + "','" + ai.DateOfTransaction + "','" + ServerDateTime + "') SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                aop.status = "Success";
                aop.value = Convert.ToString(a);
            }
            catch (Exception ex)
            {
                aop.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(aop);

            return sJSONResponse;
        }
        public Object GetMemberAttendence([FromBody]AttendenceInput ai)
        {

            AttendenceAllOutput AAOP = new AttendenceAllOutput();
            DataSet ds_SlotBooking = new DataSet();
            string sJSONResponse = "";

            DataTable dt_AttendenceAggregatePresent = new DataTable();

            dt_AttendenceAggregatePresent = getdata(string.Format("SELECT count(*) as NoOfDaysPresent FROM (SELECT ROW_NUMBER() OVER(PARTITION BY UCode,convert(date,DateOfTransaction) ORDER BY UCode,convert(date,DateOfTransaction) DESC) AS StRank, * FROM DailyAttendence) n WHERE StRank IN (1) and  UCode='{0}' and DateOfTransaction between '{1}' and '{2}' ", ai.MembershipCode, ai.StartDate, ai.EndDate));

            double TotalNoDays = ai.EndDate.Subtract(ai.StartDate).Days;
            double NoOfDaysPresent = Convert.ToDouble(dt_AttendenceAggregatePresent.Rows[0]["NoOfDaysPresent"]);
            double CompleteDivided = NoOfDaysPresent / TotalNoDays;
            double PresentPercentage = CompleteDivided * 100;

            try
            {

                List<Attendenceobjects> attReport = new List<Attendenceobjects>();
                Attendenceobjects dashboardlist = new Attendenceobjects { Attendence = GetDateWiseAttendence(ai.MembershipCode, ai.StartDate, ai.EndDate), TotalNoDays = Math.Round(TotalNoDays), NoOfDaysPresent = Math.Round(NoOfDaysPresent), PresentPercentage = Math.Round(PresentPercentage) };
                attReport.Add(dashboardlist);

                AAOP.status = "success";
                AAOP.value = attReport;

                sJSONResponse = JsonConvert.SerializeObject(AAOP);
            }
            catch (Exception ec)
            {
            }


            return sJSONResponse;
        }
        public List<MonthlyAttendence> MonthlyAttendence(string MembershipCode, DateTime StartDate, DateTime EndDate)
        {
            DataSet SelectedSlots = new DataSet();
            DataTable dt_MonthlyAttendence = new DataTable();

            string MonthName = "";
            int Months = 0;
            int Year = 0;

            List<MonthlyAttendence> monthAtt = new List<MonthlyAttendence>();


            dt_MonthlyAttendence = getdata(string.Format("select Month(TransactionDate) as Months,Year(TransactionDate) as Year,datename(month, TransactionDate) as MonthName,count(*) as DaysPresent from FilteredDailyAttendence where UCode='{0}' and TransactionDate between '{1}' and '{2}'  GROUP BY Month(TransactionDate),Year(TransactionDate),datename(month, TransactionDate)", MembershipCode, StartDate, EndDate));


            for (int i = 0; i <= dt_MonthlyAttendence.Rows.Count - 1; i++)
            {

                MonthName = dt_MonthlyAttendence.Rows[i]["MonthName"].ToString();
                Months = Convert.ToInt32(dt_MonthlyAttendence.Rows[i]["Months"].ToString());
                Year = Convert.ToInt32(dt_MonthlyAttendence.Rows[i]["Year"].ToString());



                monthAtt.Add(new MonthlyAttendence { MonthsPresentDates = GetDateWiseAttendence(MembershipCode, StartDate, EndDate) });


            }
            return monthAtt;







        }
        public List<DateWiseAttendence> GetDateWiseAttendence(string MembershipCode, DateTime StartDate, DateTime EndDate)
        {

            DataSet SelectedSlots = new DataSet();
            DataTable dt_DateWiseAtt = new DataTable();
            List<DateWiseAttendence> dawat = new List<DateWiseAttendence>();

            dt_DateWiseAtt = getdata(string.Format("SELECT convert(date,DateOfTransaction) as Date,DATENAME(month, DateOfTransaction) as Month,DATENAME(YEAR, DateOfTransaction) as Year,convert(time,DateOfTransaction) as Time FROM (SELECT ROW_NUMBER() OVER(PARTITION BY UCode,convert(date,DateOfTransaction) ORDER BY UCode,convert(date,DateOfTransaction) DESC) AS StRank, * FROM DailyAttendence) n WHERE StRank IN (1) and  UCode='{0}' and DateOfTransaction between '{1}' and '{2}'", MembershipCode, StartDate, EndDate));

            for (int i = 0; i <= dt_DateWiseAtt.Rows.Count - 1; i++)
            {
                dawat.Add(new DateWiseAttendence
                {
                    PresentDate = Convert.ToDateTime(dt_DateWiseAtt.Rows[i]["Date"])
                    ,
                    Time = Convert.ToString(dt_DateWiseAtt.Rows[i]["Time"])
                    ,
                    Month = Convert.ToString(dt_DateWiseAtt.Rows[i]["Month"])
                    ,
                    Year = Convert.ToString(dt_DateWiseAtt.Rows[i]["Year"])


                });
            }

            return dawat;
        }

    }
}