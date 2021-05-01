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
using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;

namespace NarenFitnessUsers.Controllers
{
    public class ApplicationTypeInfoController : Controller
    {
        // GET: ApplicationTypeInfo
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object ApplicationTypesInfo([FromBody]ApplicationTypesInfo AppTypesInfo)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string AppType_Query = "";

            try
            {
                cnn.Open();

                AppType_Query = "insert into ApplicationTypesInfo(AppTypeId,ImageId,ImageName,ImageURL,ImagePhoto,CreatedBy,CreatedOn,IsDeleted,IsActive) values(" + AppTypesInfo.AppTypeId + "," + AppTypesInfo.ImageId + ",'" + AppTypesInfo.ImageName + "','" + AppTypesInfo.ImageURL + "','" + AppTypesInfo.ImagePhoto + "','" + AppTypesInfo.CreatedBy + "','" + ServerDateTime + "',0,1)  SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(AppType_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                AppTypesInfo.Status = "Success";
                AppTypesInfo.AppTypeInfoId = a;
            }
            catch (Exception ex)
            {
                AppTypesInfo.Status = "Fail";
                AppTypesInfo.AppTypeInfoId = 0;
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(AppTypesInfo);

            return sJSONResponse;
        }
        public Object GetApplicationTypes([FromBody]GetApplicationTypesInfo AppTypes)
        {
            var json = (object)null;
            try
            {

                DataSet ds_custdet1 = new DataSet();
                DataTable dt_ApplicationTypes = new DataTable();
                string query = "select AppTypeId,ImageId,ImageName,ImageURL,ImagePhoto from ApplicationTypesInfo";

                using (SqlDataAdapter da_custdet = new SqlDataAdapter(query, cnn))
                {
                    da_custdet.Fill(ds_custdet1);
                }
                if (ds_custdet1.Tables[0].Rows.Count > 0)
                {
                    AppTypes.AppTypeId = Convert.ToInt32(ds_custdet1.Tables[0].Rows[0]["AppTypeId"].ToString());
                    AppTypes.ImageId = Convert.ToInt32(ds_custdet1.Tables[0].Rows[0]["ImageId"].ToString());
                    AppTypes.ImageName = ds_custdet1.Tables[0].Rows[0]["ImageName"].ToString();
                    AppTypes.ImagePhoto = ds_custdet1.Tables[0].Rows[0]["ImageURL"].ToString();
                    AppTypes.ImageURL = ds_custdet1.Tables[0].Rows[0]["ImagePhoto"].ToString();

                    AppTypes.Status = "Success";
                    json = Json(AppTypes, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    AppTypes.Status = "Fail";
                    json = Json(AppTypes, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ec)
            {

                AppTypes.Status = "Fail";
                json = Json(AppTypes, JsonRequestBehavior.AllowGet);
            }
            return json;
        }
    }
}