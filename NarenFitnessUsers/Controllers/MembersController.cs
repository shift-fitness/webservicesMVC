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
using NarenFitnessUsers.Models.Members;

using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;

namespace NarenFitnessUsers.Controllers
{
    public class MembersController : Controller
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
        public Object GetMembersSearch([FromBody]MembersInput mi)
        {
            MembersOutput mo = new MembersOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            string MembershipCode = "";
            string MobileNo = "";
            string Name = "";

            DataTable dt_Members = new DataTable();

            List<MembersDetails> dalots = new List<MembersDetails>();

            //mi.MemberShipCode, mi.MobileNo, mi.UserName

            if (mi.MemberShipCode == null && mi.MobileNo == null && mi.UserName == null)
            {
                MobileNo = "0";
                Name = "0";
                MembershipCode = "0";
            }
            else if (mi.MemberShipCode != null)
            {
                MobileNo = "0";
                Name = "0";
                MembershipCode = mi.MemberShipCode;
            }
            else if (mi.MobileNo != null)
            {
                MembershipCode = "0";
                Name = "0";
                MobileNo = mi.MobileNo;
            }
            else if (mi.UserName != null)
            {
                MembershipCode = "0";
                MobileNo = "0";
                Name = mi.UserName;
            }

            try
            {
                dt_Members = getdata(string.Format("select U.UCode as MemberShipCode, U.Firstname as UserName, U.Email, U.DateOfBirth, CCRMS.StartDate, CCRMS.MembershipExpireDate, U.MobileNo, U.Gender,U.PhotoUrl,CCRMS.BranchCode,B.BranchName from Users U, CCRMMembership CCRMS,Branch B where B.BranchCode=CCRMS.BranchCode and U.UCode= CCRMS.MembershipCode and CCRMS.MembershipCode like '{0}%' and CCRMS.MembershipCode<>'' union all  select U.UCode as MemberShipCode, U.Firstname as UserName, U.Email, U.DateOfBirth, CCRMS.StartDate, CCRMS.MembershipExpireDate, U.MobileNo, U.Gender,U.PhotoUrl,CCRMS.BranchCode,B.BranchName from Users U, CCRMMembership CCRMS,Branch B where B.BranchCode=CCRMS.BranchCode and  U.UCode= CCRMS.MembershipCode and U.MobileNo like '{0}%' and U.MobileNo<>'' union all select U.UCode as MemberShipCode, U.Firstname as UserName, U.Email, U.DateOfBirth, CCRMS.StartDate, CCRMS.MembershipExpireDate, U.MobileNo, U.Gender,U.PhotoUrl,CCRMS.BranchCode,B.BranchName from Users U, CCRMMembership CCRMS,Branch B where B.BranchCode=CCRMS.BranchCode and  U.UCode= CCRMS.MembershipCode and U.Firstname like '{0}%' and U.Firstname<>'' ", mi.SearchItem));

                for (int i = 0; i < dt_Members.Rows.Count; i++)
                {
                    MembersDetails MembersDetails = new MembersDetails { MemberShipCode = dt_Members.Rows[i]["MemberShipCode"].ToString(), UserName = dt_Members.Rows[i]["UserName"].ToString(), Email = dt_Members.Rows[i]["Email"].ToString(), DateOfBirth = Convert.ToDateTime(dt_Members.Rows[i]["DateOfBirth"].ToString()), MembershipStartDate = dt_Members.Rows[i]["StartDate"].ToString(), MembershipExpireDate = dt_Members.Rows[i]["MembershipExpireDate"].ToString(), MobileNo = dt_Members.Rows[i]["MobileNo"].ToString(), Gender = dt_Members.Rows[i]["Gender"].ToString(), PhotoUrl = dt_Members.Rows[i]["PhotoUrl"].ToString(), BranchCode = dt_Members.Rows[i]["BranchCode"].ToString(), BranchName = dt_Members.Rows[i]["BranchName"].ToString() };
                    dalots.Add(MembersDetails);
                }

                mo.status = "success";
                mo.value = dalots;
                sJSONResponse = JsonConvert.SerializeObject(mo);


            }
            catch (Exception ec)
            {
                mo.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(mo);
            }


            return sJSONResponse;
        }
        public Object FacilityUsed([FromBody]MembersInput mi)
        {
            MembersFacilityUsedOutput mo = new MembersFacilityUsedOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Members = new DataTable();
            List<MembersFacilityUsedDetails> membersFacility = new List<MembersFacilityUsedDetails>();

            try
            {
                dt_Members = getdata(string.Format("select convert(Date,FacilityStartDate) as FacilityStartDate,convert(Date,FacilityExpireDate) as FacilityExpireDate,SMFM.SubSchemName as FacilityName,CCRMMF.CreatedOn,isnull(CCRMMF.LeftOutDays,0) as NoOfDays from CCRMMembershipFacility CCRMMF,SubMembersFacilityMaster SMFM where CCRMMF.MFDID=SMFM.MFMID and CCRMMF.InvoiceID='{0}' ", mi.Invoice));

                for (int i = 0; i < dt_Members.Rows.Count; i++)
                {
                    MembersFacilityUsedDetails MembersFacilityUsedDetails = new MembersFacilityUsedDetails { FacilityStartDate = Convert.ToDateTime(dt_Members.Rows[i]["FacilityStartDate"].ToString()), FacilityExpireDate = Convert.ToDateTime(dt_Members.Rows[i]["FacilityExpireDate"].ToString()), FacilityName = dt_Members.Rows[i]["FacilityName"].ToString(), NoOfDays = Convert.ToInt32(dt_Members.Rows[i]["NoOfDays"].ToString()), CreatedOn = Convert.ToDateTime(dt_Members.Rows[i]["CreatedOn"].ToString()) };
                    membersFacility.Add(MembersFacilityUsedDetails);
                }

                mo.status = "success";
                mo.value = membersFacility;
                sJSONResponse = JsonConvert.SerializeObject(mo);


            }
            catch (Exception ec)
            {
                mo.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(mo);
            }


            return sJSONResponse;
        }

    }
}