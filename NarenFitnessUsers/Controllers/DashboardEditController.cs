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
    public class DashboardEditController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object OnlinePackagePost([FromBody]OnlinePackagePost OnlinePackage)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";
            PaymentDetailsPost Pdetails = new PaymentDetailsPost();

            try
            {
                cnn.Open();
                olPackage_Query = "insert into OnlinePackages(BranchCode,EnquireTypeNo,PackageID,PackageName,PackageCost,DiscountPercentage,NumberOfSession,NumberOfDaysValidity,Description,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + OnlinePackage.branchCode + "','" + OnlinePackage.enquireTypeNo + "','" + OnlinePackage.packageId + "','" + OnlinePackage.packageName + "','" + OnlinePackage.packageCost + "','" + OnlinePackage.discountPercentage + "','" + OnlinePackage.noOFSessions + "','" + OnlinePackage.nooOfDateValidity + "','" + OnlinePackage.description + "','','" + ServerDateTime + "',0,1) SELECT @@IDENTITY;";
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
        public Object OnlinePackageUpdate([FromBody]OnlinePackagePost OnlinePackage)
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
                    command.CommandText = "update OnlinePackages set PackageName='" + OnlinePackage.packageName + "',PackageCost='" + OnlinePackage.packageCost + "',DiscountPercentage='" + OnlinePackage.discountPercentage + "',NumberOfSession='" + OnlinePackage.noOFSessions + "',NumberOfDaysValidity='" + OnlinePackage.nooOfDateValidity + "' where ID=" + OnlinePackage.Id + " ";
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
        public Object OnlinePackageDelete([FromBody]OnlinePackagePost OnlinePackage)
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
                    command.CommandText = "update OnlinePackages set PackageName='" + OnlinePackage.packageName + "',PackageCost='" + OnlinePackage.packageCost + "',DiscountPercentage='" + OnlinePackage.discountPercentage + "',NumberOfSession='" + OnlinePackage.noOFSessions + "',NumberOfDaysValidity='" + OnlinePackage.nooOfDateValidity + "' where ID=" + OnlinePackage.Id + " ";
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
    }
}