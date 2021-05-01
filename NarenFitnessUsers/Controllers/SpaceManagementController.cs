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
using NarenFitnessUsers.Models.SpaceManagement.SlotAllocation;

using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;



namespace NarenFitnessUsers.Controllers
{
    public class SpaceManagementController : Controller
    {

        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes
        public Object SlotsCapacityPost([FromBody]SlotAllocationInput sai)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            string a = "";
            string olPackage_Query = "insert into CMSSlotWiseAllocation(TrainerCode,PackageCode,SlotCode,AllocatedCount,FreeTrialAllocatedCount,BranchCode,Duration,MinPrice,MaxPrice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + sai.TrainerCode + "','" + sai.PackageCode + "','" + sai.SlotCode + "','" + sai.AllocatedCount + "','" + sai.FreeTrialAllocatedCount + "','" + sai.BranchCode + "','"+sai.Duration+"','"+sai.MinPrice+"','"+sai.MaxPrice+"','" + sai.CreatedBy + "','" + ServerDateTime + "',0,1)";
            SlotAllocationOutput sapost = new SlotAllocationOutput();

            try
            {
                cnn.Open();

                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                tm_cmd.ExecuteNonQuery();
                sapost.status = "Success";
                sapost.value = a;
            }
            catch (Exception ex)
            {
                sapost.status = "fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(sapost);

            return sJSONResponse;
        }
        public Object SlotsCapacityUpdate([FromBody]SlotAllocationInput sai)
        {
            string sJSONResponse = "";
            SlotAllocationOutput sapost = new SlotAllocationOutput();
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
                    command.CommandText = "update CMSSlotWiseAllocation set TrainerCode='" + sai.TrainerCode + "',PackageCode='" + sai.PackageCode + "',SlotCode='" + sai.SlotCode + "',AllocatedCount='" + sai.AllocatedCount + "',FreeTrialAllocatedCount='" + sai.FreeTrialAllocatedCount + "',Duration='"+sai.Duration+"',MinPrice='"+sai.MinPrice+"',MaxPrice='"+sai.MaxPrice+"' where ID='" + sai.ID + "' ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    sapost.status = "Success";

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
                sapost.status = "fail";

            }

            sJSONResponse = JsonConvert.SerializeObject(sapost);
            return sJSONResponse;
        }
        public Object SlotsCapacityDelete([FromBody]SlotAllocationInput sai)
        {
            string sJSONResponse = "";
            SlotAllocationOutput sapost = new SlotAllocationOutput();
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
                    command.CommandText = "delete from CMSSlotWiseAllocation  where ID='" + sai.ID + "' ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    sapost.status = "Success";
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
                sapost.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(sapost);
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
        public Object GetSSlotsCapacity([FromBody]SlotAllocationInput sai)
        {

            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Facility = new DataTable();

            List<SlotCapacityAllocated> slotallocated = new List<SlotCapacityAllocated>();
            SlotCapacityAllocatedOutput fdo = new SlotCapacityAllocatedOutput();
            try
            {
                dt_Facility = getdata(string.Format("Select CMSSWA.ID,CMSP.PackageName,SlotName,Duration,MaxPrice,MinPrice,AllocatedCount from CMSSlotWiseAllocation CMSSWA, CMSSESSIONTIMESETTING CMSS,CMSSLOTTIMESETTING CMSSS,CMSPACKAGES CMSP where CMSSWA.SlotCode=CMSSS.SlotCode and CMSS.SessionCode=CMSSS.SessionCode and CMSSWA.PackageCode=CMSP.PackageCode and CMSSWA.BranchCode='{0}' and TrainerCode='{1}'  and CMSS.SessionCode='{2}' order by CMSSS.SlotStartTime asc", sai.BranchCode, sai.TrainerCode, sai.SessionCode));

                for (int i = 0; i <= dt_Facility.Rows.Count - 1; i++)
                {
                    slotallocated.Add(new SlotCapacityAllocated
                    {
                        ID = Convert.ToInt32(dt_Facility.Rows[0]["ID"])
                        ,
                        packageName = Convert.ToString(dt_Facility.Rows[0]["PackageName"])
                        ,
                        slotName = Convert.ToString(dt_Facility.Rows[0]["SlotName"])
                        ,
                        duration = Convert.ToString(dt_Facility.Rows[0]["Duration"])
                        ,
                        maxPrice = float.Parse(dt_Facility.Rows[0]["MaxPrice"].ToString())
                        ,
                        minPrice = float.Parse(dt_Facility.Rows[0]["MinPrice"].ToString())
                        ,
                        allocatedCount = Convert.ToString(dt_Facility.Rows[0]["AllocatedCount"])

                    });
                }

                fdo.status = "success";
                fdo.value = slotallocated;
                sJSONResponse = JsonConvert.SerializeObject(fdo);


            }
            catch (Exception ec)
            {

                fdo.status = "success";

                sJSONResponse = JsonConvert.SerializeObject(fdo);
            }


            return sJSONResponse;
        }


    }
}

