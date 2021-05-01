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
using NarenFitnessUsers.Models.PromoCode;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;

namespace NarenFitnessUsers.Controllers
{
    public class PromoCodeController : Controller
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
        public Object PromoCode([FromBody]List<PromoCodePost> pcp)
        {

            PromoCodeOutput daOP = new PromoCodeOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;


            try
            {
                for (int i = 0; i <= pcp.Count-1; i++)
                {

                    cnn.Open();
                    transaction = cnn.BeginTransaction("SampleTransaction");
                    command.Connection = cnn;
                    command.Transaction = transaction;
                    command.CommandText = "insert into PromoCodeMaster(BranchCode,PromoCodeType,PromoCodeTypeName,PromoCode,PromoCodeName,PromoCodeDescription,FacilityApplicable,AddDays,DiscountAmount,DiscountPercentage,TermsAndConditions,PromoCodeStartDate,PromoCodeEndDate,PackageCode,SlotCode,MobileNo,Register,DurationID,IsDeleted,IsActive,CreatedBy,CreatedOn) values('" + pcp[i].BranchCode + "','" + pcp[i].PromoCodeType + "','" + pcp[i].PromoCodeTypeName + "','" + pcp[i].PromoCode + "','" + pcp[i].PromoCodeName + "','" + pcp[i].PromoCodeDescription + "','" + pcp[i].FacilityApplicable + "','" + pcp[i].AddDays + "','" + pcp[i].DiscountAmount + "','" + pcp[i].DiscountPercentage + "','" + pcp[i].TermsAndConditions + "','" + pcp[i].PromoCodeStartDate + "','" + pcp[i].PromoCodeEndDate + "','" + pcp[i].PackageCode + "','" + pcp[i].SlotCode + "','" + pcp[i].MobileNo + "','" + pcp[i].Register + "','" + pcp[i].DurationID + "',0,1,'" + pcp[i].CreatedBy + "','')";
                    command.ExecuteNonQuery();
                    transaction.Commit();

                    cnn.Close();
                }
                daOP.status = "success";

                sJSONResponse = JsonConvert.SerializeObject(daOP);
            }
            catch (Exception ex)
            {
                try
                {
                    transaction = cnn.BeginTransaction("SampleTransaction");
                    transaction.Rollback();
                    daOP.status = "fail";

                    sJSONResponse = JsonConvert.SerializeObject(daOP);

                }
                catch (Exception ex2)
                {
                    daOP.status = "fail";

                    sJSONResponse = JsonConvert.SerializeObject(daOP);
                }
            }

            return sJSONResponse;
        }
        public Object GetPromoCode([FromBody]PromoCodeInput pci)
        {
            PromoCodeOutput daOP = new PromoCodeOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Promocode = new DataTable();

            List<PromoCodeDetails> proDetails = new List<PromoCodeDetails>();

            try
            {
                dt_Promocode = getdata(string.Format("select PID as ID,PromoCode,PromoCodeName,PromoCodeDescription,FacilityApplicable,AddDays,DiscountAmount,DiscountPercentage,TermsAndConditions,PromoCodeStartDate,PromoCodeEndDate,PromoCodeType,PromoCodeTypeName,PID,IsActive,DurationID from PromoCodeMaster where BranchCode='{0}' and Register='{1}'  and IsDeleted=0 and IsActive=1 and PromoCodeType=1 ", pci.BranchCode, pci.Register));

                for (int i = 0; i < dt_Promocode.Rows.Count; i++)
                {
                    PromoCodeDetails PromocodeDetails = new PromoCodeDetails { ID = Convert.ToInt32(dt_Promocode.Rows[i]["ID"].ToString()), PromoCode = dt_Promocode.Rows[i]["PromoCode"].ToString(), PromoCodeName = dt_Promocode.Rows[i]["PromoCodeName"].ToString(), PromoCodeDescription = dt_Promocode.Rows[i]["PromoCodeDescription"].ToString(), FacilityApplicable = dt_Promocode.Rows[i]["FacilityApplicable"].ToString(), AddDays = Convert.ToInt32(dt_Promocode.Rows[i]["AddDays"].ToString()), DiscountAmount = float.Parse(dt_Promocode.Rows[i]["DiscountAmount"].ToString()), IsActive = Convert.ToBoolean(dt_Promocode.Rows[i]["IsActive"].ToString()), DiscountPercentage = Convert.ToInt32(dt_Promocode.Rows[i]["DiscountPercentage"].ToString()), TermsAndConditions = Convert.ToString(dt_Promocode.Rows[i]["TermsAndConditions"].ToString()), PromoCodeStartDate = Convert.ToString(dt_Promocode.Rows[i]["PromoCodeStartDate"].ToString()), PromoCodeEndDate = Convert.ToString(dt_Promocode.Rows[i]["PromoCodeEndDate"].ToString()), PromoCodeType = Convert.ToInt32(dt_Promocode.Rows[i]["PromoCodeType"].ToString()), PromoCodeTypeName = Convert.ToString(dt_Promocode.Rows[i]["PromoCodeTypeName"].ToString()), DurationID = Convert.ToString(dt_Promocode.Rows[i]["DurationID"].ToString()) };
                    proDetails.Add(PromocodeDetails);
                }

                daOP.status = "success";
                daOP.value = proDetails;
                sJSONResponse = JsonConvert.SerializeObject(daOP);


            }
            catch (Exception ec)
            {
                daOP.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(daOP);
            }


            return sJSONResponse;
        }
        public Object ValidatePromocode([FromBody]PromoCodeInput pci)
        {


            Class.SearchEngin se = new Class.SearchEngin();
            ValidatePromoCodeOutPut daOP = new ValidatePromoCodeOutPut();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";
            DataTable dt_Promocode = new DataTable();
            string Query = "";

            Dictionary<string, string> PromocodeRequirement = new Dictionary<string, string>();

            try
            {
                PromocodeRequirement.Add("BranchCode", pci.BranchCode);
                PromocodeRequirement.Add("PromoCode", pci.PromoCode);
                PromocodeRequirement.Add("DurationId", pci.DurationId);
                PromocodeRequirement.Add("PackageCode", pci.PackageCode);
                PromocodeRequirement.Add("SlotCode", pci.SlotCode);
                PromocodeRequirement.Add("MobileNo", pci.MobileNo);
                Query = se.PromocodeQueryBuilder(PromocodeRequirement);
            }
            catch (Exception ec)
            {

            }

            List<ValidatePromoCodeDetails> ValidateproDetails = new List<ValidatePromoCodeDetails>();

            try
            {
                dt_Promocode = getdata(Query);

                for (int i = 0; i < dt_Promocode.Rows.Count; i++)
                {
                    ValidatePromoCodeDetails PromocodeDetails = new ValidatePromoCodeDetails { ID = Convert.ToInt32(dt_Promocode.Rows[i]["PID"].ToString()), PromoCode = dt_Promocode.Rows[i]["PromoCode"].ToString(), PromoCodeName = dt_Promocode.Rows[i]["PromoCodeName"].ToString(), PromoCodeDescription = dt_Promocode.Rows[i]["PromoCodeDescription"].ToString(), FacilityApplicable = dt_Promocode.Rows[i]["FacilityApplicable"].ToString(), AddDays = Convert.ToInt32(dt_Promocode.Rows[i]["AddDays"].ToString()), DiscountAmount = float.Parse(dt_Promocode.Rows[i]["DiscountAmount"].ToString()), DiscountPercentage = dt_Promocode.Rows[i]["DiscountPercentage"].ToString(), TermsAndConditions = Convert.ToString(dt_Promocode.Rows[i]["TermsAndConditions"].ToString()), PromoCodeStartDate = Convert.ToString(dt_Promocode.Rows[i]["PromoCodeStartDate"].ToString()), PromoCodeEndDate = Convert.ToString(dt_Promocode.Rows[i]["PromoCodeEndDate"].ToString()) };
                    ValidateproDetails.Add(PromocodeDetails);
                }

                daOP.status = "success";
                daOP.value = ValidateproDetails;
                sJSONResponse = JsonConvert.SerializeObject(daOP);


            }
            catch (Exception ec)
            {
                daOP.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(daOP);
            }


            return sJSONResponse;
        }

        //Promocode Multiple post
        //CRUD --  Update,delete,create,Get
        public Object CreatePromoCode([FromBody]PromoCodePost pcp)
        {

            PromoCodeOutput daOP = new PromoCodeOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            transaction = cnn.BeginTransaction("SampleTransaction");
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                command.CommandText = "insert into PromoCode(PromoCode,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + pcp.PromoCode + "','" + pcp.CreatedBy + "','',0,1)";
                command.ExecuteNonQuery();
                transaction.Commit();

                daOP.status = "success";

                sJSONResponse = JsonConvert.SerializeObject(daOP);
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                    daOP.status = "fail";

                    sJSONResponse = JsonConvert.SerializeObject(daOP);

                }
                catch (Exception ex2)
                {

                }
            }

            return sJSONResponse;
        }
        public Object GetPromoCodeMain([FromBody]PromoCodeInput pci)
        {
            PromoCodeDetailsoutput daOP = new PromoCodeDetailsoutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Promocode = new DataTable();

            List<PromoDetails> proDetails = new List<PromoDetails>();

            try
            {
                dt_Promocode = getdata(string.Format("select ID,PromoCode from PromoCode"));

                for (int i = 0; i < dt_Promocode.Rows.Count; i++)
                {
                    PromoDetails PromocodeDetails = new PromoDetails { ID = Convert.ToInt32(dt_Promocode.Rows[i]["ID"].ToString()), PromoCode = dt_Promocode.Rows[i]["PromoCode"].ToString() };
                    proDetails.Add(PromocodeDetails);
                }

                daOP.status = "success";
                daOP.value = proDetails;
                sJSONResponse = JsonConvert.SerializeObject(daOP);


            }
            catch (Exception ec)
            {
                daOP.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(daOP);
            }


            return sJSONResponse;
        }
        public Object UpdatePromoCode([FromBody]PromoCodePost pcp)
        {

            PromoCodeOutput daOP = new PromoCodeOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            transaction = cnn.BeginTransaction("SampleTransaction");
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                command.CommandText = "update PromoCode set PromoCode='" + pcp.PromoCode + "' where ID='" + pcp.ID + "' ";
                command.ExecuteNonQuery();
                transaction.Commit();

                daOP.status = "success";

                sJSONResponse = JsonConvert.SerializeObject(daOP);
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                    daOP.status = "fail";

                    sJSONResponse = JsonConvert.SerializeObject(daOP);

                }
                catch (Exception ex2)
                {

                }
            }

            return sJSONResponse;
        }
        public Object DeletePromoCode([FromBody]PromoCodePost pcp)
        {

            PromoCodeOutput daOP = new PromoCodeOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            transaction = cnn.BeginTransaction("SampleTransaction");
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                command.CommandText = "delete from PromoCode where PromoCode='" + pcp.PromoCode + "' ";
                command.ExecuteNonQuery();
                transaction.Commit();

                daOP.status = "success";

                sJSONResponse = JsonConvert.SerializeObject(daOP);
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                    daOP.status = "fail";

                    sJSONResponse = JsonConvert.SerializeObject(daOP);

                }
                catch (Exception ex2)
                {

                }
            }

            return sJSONResponse;
        }

    }
}