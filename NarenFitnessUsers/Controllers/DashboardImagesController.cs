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
using NarenFitnessUsers.Models.Dashboard;
using NarenFitnessUsers.Models.RazorPay;
using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;

namespace NarenFitnessUsers.Controllers
{
    public class DashboardImagesController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes

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
        public Object DashboardImage([FromBody]DashboardImages di)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";
            PaymentDetailsPost Pdetails = new PaymentDetailsPost();

            string b = di.imageUrl;
            b = b.Substring(b.IndexOf(",") + 1);

            string ImageUrl = GetPhotoUrl(b, di.displayId+Convert.ToString(imgname()));
            try
            {
               
                cnn.Open();
                olPackage_Query = "insert into DashboardImages(AppId,DisplayId,Image,ImageURL,CreatedBy,CreatedOn,IsDeleted,IsActive,ScreenId) values('" + di.appId + "','" + di.displayId + "','" + b + "','" + ImageUrl + "','" + di.createdBy + "','" + ServerDateTime + "',0,1,"+di.screenId+") SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                Pdetails.status = "Success";
                Pdetails.transactionId = a;
            }
            catch (Exception ex)
            {
                Pdetails.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(Pdetails);

            return sJSONResponse;
        }

        public string GetPhotoUrl(string Base64String, string Id)
        {
            string urlform = "";
            string endpath = "";
            try
            {

                string base64string = Base64String;

              

                var bytes = Convert.FromBase64String(base64string);

                endpath = "Icons" + Id + ".png";
                //C:\inetpub\wwwroot\GYMUI\UsersImages
                string filepath = @"C:\inetpub\wwwroot\GYMUI\DashBoardImages\\" + endpath;
                using (var imageFile = new FileStream(filepath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                //urlform = "http://137.59.201.211/GYMUI/DashBoardImages/" + endpath;
                urlform = "http://202.143.96.72/GYMUI/DashBoardImages/" + endpath;
            }
            catch (Exception ec)
            {
                urlform = "";
            }

            return urlform;
        }
        public Object DashboardImageUpdate([FromBody]DashboardImages di)
        {
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            try
            {
                string b = di.image;
                b = b.Substring(b.IndexOf(",") + 1);
                string ImageUrl = GetPhotoUrl(b, di.displayId + Convert.ToString(imgname()));
               
                cnn.Close();
                cnn.Open();
                SqlTransaction transaction;
                transaction = cnn.BeginTransaction("SampleTransaction");
                command.Connection = cnn;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = "update DashboardImages set Image='" + b + "',ImageURL='"+ImageUrl+"',ScreenId=" + di.screenId+",AppId="+di.appId+",DisplayId="+di.displayId+" where Id='" + di.id + "'";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {

                }

            }
            catch (Exception ec)
            {

            }

            string sJSONResponse = "";
            return sJSONResponse;
        }
        public Object DashboardImageDelete([FromBody]DashboardImages di)
        {
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            try
            {

                cnn.Close();
                cnn.Open();
                SqlTransaction transaction;
                transaction = cnn.BeginTransaction("SampleTransaction");
                command.Connection = cnn;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = "delete from DashboardImages where Id='" + di.id + "' ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {

                }

            }
            catch (Exception ec)
            {

            }

            string sJSONResponse = "";
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
        public Object GetDashboardImage([FromBody]DashboardImages da)
        {
            DashboardImagesOutput daOP = new DashboardImagesOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Sessions = new DataTable();

            List<DashboardImagesResponse> dalots = new List<DashboardImagesResponse>();

            try
            {
                dt_Sessions = getdata(string.Format("select DI.Id,DI.AppId,DA.ApplicationName,DI.DisplayId,DD.DisplayName,DI.ScreenId,D.ScreenName,DI.ImageURL from DashboardImages DI,Dashboard D,DashboardDisplay DD,DashboardApplications DA where DI.ScreenId=D.ScreenId and DD.ID=DI.DisplayId and DA.ID=DI.AppId and DI.IsActive=1 and DI.IsDeleted=0", ""));

                for (int i = 0; i < dt_Sessions.Rows.Count; i++)
                {
                    DashboardImagesResponse SlotsDetails = new DashboardImagesResponse { id = dt_Sessions.Rows[i]["Id"].ToString(), appId = dt_Sessions.Rows[i]["AppId"].ToString(), applicationName = dt_Sessions.Rows[i]["ApplicationName"].ToString(), displayId = dt_Sessions.Rows[i]["DisplayId"].ToString(), displayName = dt_Sessions.Rows[i]["DisplayName"].ToString(), screenId = dt_Sessions.Rows[i]["ScreenId"].ToString(), screenName = dt_Sessions.Rows[i]["ScreenName"].ToString(), imageURL = dt_Sessions.Rows[i]["ImageURL"].ToString() };
                    dalots.Add(SlotsDetails);
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