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
using NarenFitnessUsers.Models.RazorPay;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;

namespace NarenFitnessUsers.Controllers
{
    public class RazorPayController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object MerchantDetailsPost([FromBody]MarchantDetailsInput mdi)
        {
            MarchantDetailsOutput mdo = new MarchantDetailsOutput();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";

            try
            {
                cnn.Open();
                olPackage_Query = "insert into MarchantDetails(MarchantName,MarchantKey,MarchantSecretKey,BranchCode,IsDeleted,IsActive) values('" + mdi.MarchantName + "','" + mdi.MarchantKey + "','"+mdi.MarchantSecretKey+"','" + mdi.BranchCode + "',0,1)";
                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteNonQuery());
                mdo.status = "Success";
                mdo.value = "";
            }
            catch (Exception ex)
            {
                mdo.status = "fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(mdo);

            return sJSONResponse;
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
        public Object GetMerchantDetails([FromBody]MarchantDetailsInput mts)
        {
            MarchantDetailsOutputMulti daOP = new MarchantDetailsOutputMulti();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_MerchantDetails = new DataTable();

            List<MarchantDetails> dalots = new List<MarchantDetails>();

            try
            {
                dt_MerchantDetails = getdata(string.Format("select MarchantName,MarchantKey,MarchantSecretKey,BranchCode from MarchantDetails", ""));

                for (int i = 0; i < dt_MerchantDetails.Rows.Count; i++)
                {
                    MarchantDetails MarchantData = new MarchantDetails { MerchantName = dt_MerchantDetails.Rows[i]["MarchantName"].ToString(), MerchantKey = dt_MerchantDetails.Rows[i]["MarchantKey"].ToString(), BranchCode = dt_MerchantDetails.Rows[i]["BranchCode"].ToString(),MerchantSecretKey= dt_MerchantDetails.Rows[i]["MarchantKey"].ToString() };
                    dalots.Add(MarchantData);
                }

                daOP.status = "success";
                daOP.value = dalots;
                sJSONResponse = JsonConvert.SerializeObject(daOP);


            }
            catch (Exception ec)
            {
                daOP.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(daOP);
            }


            return sJSONResponse;
        }
    }
}