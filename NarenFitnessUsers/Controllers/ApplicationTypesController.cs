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
    public class ApplicationTypesController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object ApplicationTypes([FromBody]ApplicationTypes AppTypes)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string AppType_Query = "";
            ApplicationRequestsPost AppType = new ApplicationRequestsPost();

            try
            {
                cnn.Open();

                AppType_Query = "insert into ApplicationTypes(AppTypeName,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + AppTypes.AppTypeName + "','" + AppTypes.CreatedBy + "','" + ServerDateTime + "',0,1)  SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(AppType_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                AppType.Status = "Success";
                AppType.AppId = a;
            }
            catch (Exception ex)
            {
                AppType.Status = "Fail";
                AppType.AppId = 0;
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(AppType);

            return sJSONResponse;
        }
        public Object GetApplicationTypes([FromBody]GetApplicationTypes AppTypes)
        {
            GetApplicationInfoFail gaif = new GetApplicationInfoFail();

            var json = (object)null;
            try
            {

                DataSet ds_custdet1 = new DataSet();
                DataTable dt_ApplicationTypes = new DataTable();
                string query = "select AppTypeId,AppTypeName from ApplicationTypes";

                using (SqlDataAdapter da_custdet = new SqlDataAdapter(query, cnn))
                {
                    da_custdet.Fill(ds_custdet1);
                }
                if (ds_custdet1.Tables[0].Rows.Count > 0)
                {
                    AppTypes.AppTypeId = Convert.ToInt32(ds_custdet1.Tables[0].Rows[0]["AppTypeId"].ToString());
                    AppTypes.AppTypeName = ds_custdet1.Tables[0].Rows[0]["AppTypeName"].ToString();
                    AppTypes.Status = "Success";
                    json = Json(AppTypes, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    gaif.Status = "Sucess";
                    gaif.Messsage = "No Data";

                    json = Json(gaif, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ec)
            {

                gaif.Status = "Sucess";
                gaif.Messsage = "Internal Server Error";

                json = Json(gaif, JsonRequestBehavior.AllowGet);
            }
            return json;
        }
    }
}