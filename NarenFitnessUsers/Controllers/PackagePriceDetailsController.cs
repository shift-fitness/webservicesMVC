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
using NarenFitnessUsers.Models.RazorPay;
using NarenFitnessUsers.Class;
using NarenFitnessUsers.Models.Packages;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;

namespace NarenFitnessUsers.Controllers
{
    public class PackagePriceDetailsController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        JsonClass json = new JsonClass();
        public Object GetPaymentDetails([FromBody]PackagePriceList PackagePrices)
        {
            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();
            int EnquiretypeNo = 0;
            int isFreeTrial = 0;
            PackagePriceFinalOutput ppopt = new PackagePriceFinalOutput();
            try
            {

                List<PackagePriceList> dbPackage = new List<PackagePriceList>();

                if (PackagePrices.enquiretypeNo == 0)
                {
                    EnquiretypeNo = 2;
                    isFreeTrial = 1;
                }
                else if (PackagePrices.enquiretypeNo == 2 || PackagePrices.enquiretypeNo == 3)
                {
                    EnquiretypeNo = 3;
                    isFreeTrial = 0;
                }
                else
                {
                    EnquiretypeNo = 0;
                    isFreeTrial = 0;
                }

                PackagePriceList dashboardlistpackage = new PackagePriceList { enquiretypeNo = EnquiretypeNo,isFreeTrial=isFreeTrial, packagePricesList = GetPackagePriceDetails(PackagePrices.enquiretypeNo) };
                dbPackage.Add(dashboardlistpackage);

                ppopt.status = "success";
                ppopt.value = dbPackage;
                sJSONResponse = JsonConvert.SerializeObject(ppopt);


                //dt_Packageresposne.Tables.Clear();
                //dt_Packageresposne.Tables.Add(ReturnException("success", sJSONResponse));
            }
            catch (Exception ec)
            {
                ppopt.status = "success";

                sJSONResponse = JsonConvert.SerializeObject(ppopt);

            }


            // string val = json.DataTableToJSONWithStringBuilder(dt_Packageresposne.Tables[0]);

            return sJSONResponse;

        }
        private DataTable ReturnException(string Status, string Message)
        {
            DataTable dtMsg = new DataTable("MSG");
            dtMsg.Columns.Add("status");
            dtMsg.Columns.Add("value");
            DataRow drMsg = dtMsg.NewRow();
            drMsg["status"] = Status;
            drMsg["value"] = Message;
            dtMsg.Rows.Add(drMsg);
            return dtMsg;
        }
        public List<PackagePrices> GetPackagePriceDetails(int EnquireTypeNo)
        {
            DataTable dt = new DataTable();



            List<PackagePrices> PackagePrice = new List<PackagePrices>();

            if (EnquireTypeNo == 0)
            {

                dt = getdata(string.Format("select BranchCode,PackageID,PackageName,PackageCost,DiscountPercentage,NumberOfSession,NumberOfDaysValidity from OnlinePackages where IsDeleted=0 and IsActive=1 "));

            }
            else if (EnquireTypeNo == 2 || EnquireTypeNo == 3)
            {

                dt = getdata(string.Format("select BranchCode,PackageID,PackageName,PackageCost,DiscountPercentage,NumberOfSession,NumberOfDaysValidity from OnlinePackages where IsDeleted=0 and IsActive=1 and PackageID <> 1"));

            }


            for (int i = 0; i < dt.Rows.Count; i++)
            {

                PackagePrice.Add(new PackagePrices
                {
                    branchCode = Convert.ToString(dt.Rows[i]["branchCode"])
                    ,
                    packageId = Convert.ToString(dt.Rows[i]["packageID"])
                     ,
                    packageName = Convert.ToString(dt.Rows[i]["packageName"])
                     ,
                    packageCost = Convert.ToString(dt.Rows[i]["packageCost"])
                     ,
                    discountPercentage = Convert.ToString(dt.Rows[i]["discountPercentage"])
                     ,
                    numberOfSession = Convert.ToString(dt.Rows[i]["numberOfSession"])
                     ,
                    numberOfDaysValidity = Convert.ToString(dt.Rows[i]["numberOfDaysValidity"])

                });

            }
            return PackagePrice;
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