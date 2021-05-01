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
using NarenFitnessUsers.Models.pdf;

using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;

namespace NarenFitnessUsers.Controllers
{
    public class PdfController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object TermsAndConditionFileUploadPdf([FromBody]pdfInput pdf)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string AppType_Query = "";
            pdfDetails Pdfdetails = new pdfDetails();
            pdfresultoutput pdfo = new pdfresultoutput();
            try
            {
                cnn.Open();
                AppType_Query = "insert into UploadPDF(Name,Type,url,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + pdf.name + "','Pdf','" + pdf.url + "','','" + ServerDateTime + "',0,1)";
                SqlCommand tm_cmd = new SqlCommand(AppType_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                pdfo.status = "Success";
                pdfo.value = Convert.ToString(a);
            }
            catch (Exception ex)
            {
                pdfo.status = "Fail";

            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(Pdfdetails);

            return sJSONResponse;
        }
        public Object GetTermsAndConditionPDFFile([FromBody]pdfInput Booking)
        {
            pdfOutput FOutputA = new pdfOutput();

            string sJSONResponse = "";

            try
            {

                List<pdflist> pdflist = new List<pdflist>();
                pdflist pdf = new pdflist { pdfslist = GetBookedPackages("PT") };
                pdflist.Add(pdf);


                FOutputA.status = "success";
                FOutputA.value = pdflist;

                sJSONResponse = JsonConvert.SerializeObject(FOutputA);
            }
            catch (Exception ec)
            {
                FOutputA.status = "success";
                sJSONResponse = JsonConvert.SerializeObject(FOutputA);
            }


            return sJSONResponse;
        }
        public Object GetPrivacyPolicyPDFFile([FromBody]pdfInput Booking)
        {
            pdfOutput FOutputA = new pdfOutput();

            string sJSONResponse = "";

            try
            {

                List<pdflist> pdflist = new List<pdflist>();
                pdflist pdf = new pdflist { pdfslist = GetBookedPackages("PP") };
                pdflist.Add(pdf);


                FOutputA.status = "success";
                FOutputA.value = pdflist;

                sJSONResponse = JsonConvert.SerializeObject(FOutputA);
            }
            catch (Exception ec)
            {
                FOutputA.status = "success";
                sJSONResponse = JsonConvert.SerializeObject(FOutputA);
            }


            return sJSONResponse;
        }
        public List<pdfDetails> GetBookedPackages(string Type)
        {
            List<pdfDetails> ftd = new List<pdfDetails>();
            DataTable dt_Booking;

            DataTable dt = new DataTable();
            dt_Booking = getdata(string.Format("select  Name,Type,url from UploadPDF where Type='"+Type+"' ", Type));

            ftd.Add(new pdfDetails
            {
                name = Convert.ToString(dt_Booking.Rows[0]["Name"])
                 ,
                type = Convert.ToString(dt_Booking.Rows[0]["Type"])
                 ,
                 url = Convert.ToString(dt_Booking.Rows[0]["url"])

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


    }
}