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
using NarenFitnessUsers.Models.DynamicUrlCode;
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
    public class DynamicUrlGenerationController : Controller
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
        public Object CreateCodeUrl([FromBody]PlanInput pi)
        {
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            PlanOutPutUrl popt = new PlanOutPutUrl();
            DataSet SelectedSlots = new DataSet();
          
            string sJSONResponse = "";
       
            DataTable dt_Promocode = new DataTable();
            List<UrlCodeParameters> urlcode = new List<UrlCodeParameters>();

            Random ran = new Random();
            String b = "ACDEFGHI0123456789JKLMNOPQRSTUVWXYZ";

            String randomurlcode = "";
            for (int i = 0; i < 9; i++)
            {
                int a = ran.Next(b.Length); //string.Lenght gets the size of string
                randomurlcode = randomurlcode + b.ElementAt(a);
            }

            string url = "http://play.google.com/store/apps/details?id=" + randomurlcode;
            string alternateUrl = "http://app.naren.fit/" + randomurlcode;
            string Text = "Invite your friends and family to Naren Fitness and get Rs.1000 Off when they purchase the package";

            DataTable dt_CodeUrl = new DataTable();
            dt_CodeUrl = getdata(string.Format("select Top 1 Url,GenertedCode from UrlCode where MobileNo='{0}'  and Used=0 order by SNO desc", pi.MobileNo));

            DataTable dt_PromoText = new DataTable();
            dt_PromoText = getdata(string.Format("select top 1 PID,PromoCodeDescription from PromoCodeMaster where PromoCode=''  and IsDeleted=0"));


            try
            {
                dt_Promocode = getdata(string.Format("select count(*) as count from UrlCode where MobileNo='{0}' and Used=0", pi.MobileNo));
                if (Convert.ToInt32(dt_Promocode.Rows[0][0].ToString()) == 0)
                {
                    cnn.Open();
                    SqlCommand command = cnn.CreateCommand();
                    SqlTransaction transaction;
                    transaction = cnn.BeginTransaction("SampleTransaction");
                    command.Connection = cnn;
                    command.Transaction = transaction;

                    try
                    {
                        command.CommandText = "insert into UrlCode(MembershipCode,MobileNo,GenertedCode,Url,Used,CreatedBy,CreatedOn) values('" + pi.MembershipCode + "','" + pi.MobileNo + "','" + randomurlcode + "','" + url + "',0,'" + pi.CreatedBy + "','" + ServerDateTime + "')";
                        command.ExecuteNonQuery();
                        command.CommandText = "update PromoCodeMaster set PromoCode='" + randomurlcode + "' where PID='" + dt_PromoText.Rows[0]["PID"].ToString() + "' ";
                        command.ExecuteNonQuery();
                        transaction.Commit();
                        {

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


                    try
                    {
                        List<UrlInfo> UrlInfo = new List<UrlInfo>();
                        UrlInfo UrlInfoDet = new UrlInfo { Url = alternateUrl,OriginalUrl= url, Code = randomurlcode, Text = Text };
                        UrlInfo.Add(UrlInfoDet);
                        popt.status = "success";
                        popt.value = UrlInfo;

                    }
                    catch (Exception ec)
                    {
                        popt.status = "fail";
                    }

                    sJSONResponse = JsonConvert.SerializeObject(popt);
                }
                else
                {
                    try
                    {
                        List<UrlInfo> UrlInfo = new List<UrlInfo>();
                        UrlInfo UrlInfoDet = new UrlInfo { Url = dt_CodeUrl.Rows[0]["Url"].ToString(), OriginalUrl = url, Code = dt_CodeUrl.Rows[0]["GenertedCode"].ToString(), Text = Text };
                        UrlInfo.Add(UrlInfoDet);
                        popt.status = "success";
                        popt.value = UrlInfo;

                    }
                    catch (Exception ec)
                    {
                        popt.status = "fail";
                    }

                    sJSONResponse = JsonConvert.SerializeObject(popt);
                }
            }
            catch (Exception ex)
            {
                popt.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(popt);
            }
            finally
            {
                cnn.Close();
            }
            return sJSONResponse;
        }

    }
}