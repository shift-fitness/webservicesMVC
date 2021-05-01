using System;
using System.Collections;
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
using NarenFitnessUsers.Models.Login;
using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;
namespace NarenFitnessUsers.Controllers
{
    public class CustomerLoginController : Controller
    {
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        JsonClass json = new JsonClass();
        SqlTransaction transaction;
        [System.Web.Mvc.HttpPost]
        //Array format not to use in Login
        public DataTable LoginMobileCheck(string MobileNo)
        {
            DataSet ds_custdet = new DataSet();
            string SendText = string.Empty;
            DataSet ds_custdet1 = new DataSet();
            DataTable dt_Login = new DataTable();

            string ErrorMessage = "";

            ArrayList arl_Mobile = MobileCheck(MobileNo);
            int EnquireformCount = Convert.ToInt32(arl_Mobile[0].ToString());
            int UserCount = Convert.ToInt32(arl_Mobile[1].ToString());

            string query1 = "";

            try
            {
                if (EnquireformCount > 0 || UserCount > 0)
                {
                    DataRow row;
                    DataColumn col1 = new DataColumn("FirstName", typeof(string));
                    DataColumn col2 = new DataColumn("LastName", typeof(string));
                    DataColumn col3 = new DataColumn("MobileNo", typeof(string));
                    DataColumn col4 = new DataColumn("MobileDeviceID", typeof(string));
                    DataColumn col5 = new DataColumn("Gender", typeof(string));
                    DataColumn col6 = new DataColumn("dob", typeof(string));
                    DataColumn col7 = new DataColumn("Email", typeof(string));
                    DataColumn col8 = new DataColumn("IsUserLoggedIn", typeof(bool));
                    DataColumn col9 = new DataColumn("HasDetailsConfirmed", typeof(bool));
                    DataColumn col10 = new DataColumn("MembershipCode", typeof(string));


                    dt_Login.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8, col9, col10 });

                    if (EnquireformCount > 0 || UserCount == 0)
                    {
                        query1 = "select Top 1 EnquirePersonFirstName as FirstName,EnquirePersonLastName as LastName,Gender,D_O_B,Email,MobileNo,MobileDeviceID,IsUserLoggedIn,HasDetailsConfirmed,UCode='' from  CCRMMEnquireForm where MobileNo='" + MobileNo + "' order by ID desc";
                    }

                    if (UserCount > 0 && EnquireformCount == 0)
                    {
                        query1 = "select Top 1 FirstName,LastName,Gender,DateOfBirth as D_O_B,Email,MobileNo,MobileDeviceID='',IsUserLoggedIn='false',HasDetailsConfirmed='false',UCode from  Users where MobileNo='" + MobileNo + "' order by ID desc";
                    }

                    if (UserCount > 0 && EnquireformCount > 0)
                    {
                        query1 = "select Top 1 U.FirstName,U.LastName,U.Gender,CCRMMEF.D_O_B,CCRMMEF.Email,CCRMMEF.MobileNo,CCRMMEF.MobileDeviceID,CCRMMEF.IsUserLoggedIn,CCRMMEF.HasDetailsConfirmed,U.UCode from  Users U, CCRMMEnquireForm CCRMMEF where CCRMMEF.MobileNo=U.MobileNo and CCRMMEF.MobileNo='" + MobileNo + "' order by U.ID desc";
                    }

                    using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
                    {
                        da_custdet.Fill(ds_custdet1);
                    }
                    if (ds_custdet1.Tables[0].Rows.Count > 0)
                    {

                        row = dt_Login.NewRow();
                        row["FirstName"] = ds_custdet1.Tables[0].Rows[0]["FirstName"].ToString();
                        row["LastName"] = ds_custdet1.Tables[0].Rows[0]["LastName"].ToString();
                        row["MobileNo"] = ds_custdet1.Tables[0].Rows[0]["MobileNo"].ToString();
                        row["MobileDeviceID"] = ds_custdet1.Tables[0].Rows[0]["MobileDeviceID"].ToString();
                        row["Gender"] = ds_custdet1.Tables[0].Rows[0]["Gender"].ToString();

                        if (ds_custdet1.Tables[0].Rows[0]["D_O_B"].ToString() == "1/1/1900 12:00:00 AM")
                        {
                            row["dob"] = "";
                        }
                        else
                        {
                            row["dob"] = ds_custdet1.Tables[0].Rows[0]["D_O_B"].ToString();
                        }
                        row["Email"] = ds_custdet1.Tables[0].Rows[0]["Email"].ToString();
                        row["IsUserLoggedIn"] = Convert.ToBoolean(ds_custdet1.Tables[0].Rows[0]["IsUserLoggedIn"].ToString());
                        row["HasDetailsConfirmed"] = Convert.ToBoolean(ds_custdet1.Tables[0].Rows[0]["HasDetailsConfirmed"].ToString());
                        row["MembershipCode"] = ds_custdet1.Tables[0].Rows[0]["UCode"].ToString();
                        dt_Login.Rows.Add(row);

                    }
                    else
                    {
                        ErrorMessage = "MobileNo or Pin is Wrong";
                    }
                }
                else
                {
                    ErrorMessage = "Users Mobile already exists";
                }

            }
            catch (Exception ec)
            {
                ErrorMessage = ec.Message;
            }
            return dt_Login;
        }
        public ArrayList MobileCheck(string MobileNo)
        {
            DataSet ds_count = new DataSet();
            ArrayList arl_Mcheck = new ArrayList();
            try
            {
                string query1 = "select count(*) as count from CCRMMEnquireForm where MobileNo ='" + MobileNo + "' union all select count(*) as count from Users where MobileNo ='" + MobileNo + "'";
                SqlDataAdapter da = new SqlDataAdapter(query1, cnn);
                da.Fill(ds_count);

                arl_Mcheck.Add(ds_count.Tables[0].Rows[0][0].ToString());
                arl_Mcheck.Add(ds_count.Tables[0].Rows[1][0].ToString());


            }
            catch (Exception ec)
            {

            }

            return arl_Mcheck;
        }
        public int OTP(string MobileNo)
        {
            int OTP1 = OTPGENERATION();
            try
            {
                DataSet ds_custdet = new DataSet();
                DataTable dt_CRM = new DataTable();
                string SendText = string.Empty;


                string httpUrl = string.Empty;
                {
                    SendText = "Mobile Number Verification OTP is :" + OTP1;
                    StreamReader objReader;
                    //httpUrl = "http://sms.smscity.in/httpapi/httpapi?token=8c84bb208c7c7b7b72660ef51a5430b0&sender=NRNGYM&number=0" + MobileNo + "&route=2&type=1&sms='" + SendText + "' ";
                    //httpUrl = "http://api.textlocal.in/send?username=karthikeyaenator@gmail.com&hash=f94d56486adbd53db4d28bcc515c415ef49267fc&numbers=" + MobileNo + "&message=" + SendText + "&sender=SUCARD ";
                    // httpUrl = "http://api.textlocal.in/send?username=dilipgoud@narenfitness.com&hash=4a7076b97161c3a3f6c14028abebf7589808c1ec3ed68eaae63c44e0db3f6497&numbers=" + MobileNo + "&message=" + SendText + "&sender=NRNGYM";

                    System.Net.WebRequest webRequest = System.Net.WebRequest.Create(httpUrl);
                    Stream objstream;
                    objstream = webRequest.GetResponse().GetResponseStream();
                    objReader = new StreamReader(objstream);
                    objReader.Close();
                }



                cnn.Close();

            }
            catch (Exception ex)
            {

            }
            return OTP1;
        }
        public Object ValidateLogin([FromBody]CustomerLogin Login)
        {
            var json = (object)null;
            try
            {

                DataSet ds_custdet1 = new DataSet();
                DataTable dt_Login = new DataTable();
                string query = "select Top 1 MobileNo,Password,EnquirePersonFirstName +' '+ EnquirePersonLastName as UserName,Address,RoleId,Lat,Long,Gender,Address,Email from  CCRMMEnquireForm where MobileNo='" + Login.MobileNo + "' order by ID desc ";

                using (SqlDataAdapter da_custdet = new SqlDataAdapter(query, cnn))
                {
                    da_custdet.Fill(ds_custdet1);
                }
                if (ds_custdet1.Tables[0].Rows.Count > 0)
                {
                    Login.MobileNo = ds_custdet1.Tables[0].Rows[0]["MobileNo"].ToString();
                    Login.Password = ds_custdet1.Tables[0].Rows[0]["Password"].ToString();
                    Login.UserName = ds_custdet1.Tables[0].Rows[0]["UserName"].ToString();
                    Login.Address = ds_custdet1.Tables[0].Rows[0]["Address"].ToString();
                    Login.RoleCode = Convert.ToInt32(ds_custdet1.Tables[0].Rows[0]["RoleId"].ToString());
                    Login.Lat = ds_custdet1.Tables[0].Rows[0]["Lat"].ToString();
                    Login.Long = ds_custdet1.Tables[0].Rows[0]["Long"].ToString();
                    Login.Gender = ds_custdet1.Tables[0].Rows[0]["Long"].ToString();
                    Login.Address = ds_custdet1.Tables[0].Rows[0]["Address"].ToString();
                    Login.EmailId = ds_custdet1.Tables[0].Rows[0]["Email"].ToString();

                    json = Json(Login, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    json = "User not found";
                }
            }
            catch (Exception ec)
            {
                json = "Internal Server Error";
            }
            return json;
        }
        public Object MobMatchOTP([FromBody]CustomerLogin Login)
        {
            var json = (object)null;
            int MobileExist = 0;
            try
            {
                cnn.Close();
                string query1 = "select Top 1 OTP from OTPVerification where MobileNo='" + Login.MobileNo + "' order by Id desc";
                SqlCommand cmdExist = new SqlCommand(query1, cnn);
                cnn.Open();
                SqlDataReader drExist = cmdExist.ExecuteReader();
                if (drExist.Read())
                {
                    MobileExist = Convert.ToInt32(drExist[0].ToString());
                }
                cnn.Close();
                json = "OTP is " + MobileExist;
            }
            catch (Exception ec)
            {
                json = "User not found";
            }

            return json;
        }
        public Object OTP([FromBody]CustomerLogin OTPM)
        {
            var json = (object)null;
            try
            {
                DataSet ds_custdet = new DataSet();
                DataTable dt_CRM = new DataTable();
                string SendText = string.Empty;
                int OTP = OTPGENERATION();
                try
                {
                    string httpUrl = string.Empty;
                    {
                        SendText = "Mobile Number Verification OTP is :" + OTP;
                        StreamReader objReader;
                        //httpUrl = "http://sms.smscity.in/httpapi/httpapi?token=8c84bb208c7c7b7b72660ef51a5430b0&sender=NRNGYM&number=0" + OTPM.MobileNo + "&route=2&type=1&sms='" + SendText + "' ";
                        //httpUrl = "http://api.textlocal.in/send?username=karthikeyaenator@gmail.com&hash=f94d56486adbd53db4d28bcc515c415ef49267fc&numbers=" + OTPM.MobileNo + "&message=" + SendText + "&sender=SUCARD ";
                        System.Net.WebRequest webRequest = System.Net.WebRequest.Create(httpUrl);
                        Stream objstream;
                        objstream = webRequest.GetResponse().GetResponseStream();
                        objReader = new StreamReader(objstream);
                        objReader.Close();
                    }


                }
                catch (Exception ex)
                {
                    json = "Internal Server Error" + ex.ToString();

                }
                finally
                {
                    json = Json(OTPM, JsonRequestBehavior.AllowGet);
                }
                cnn.Close();

            }
            catch (Exception ex)
            {
                json = "Internal Server Error" + ex.ToString();
            }
            return json;
        }
        public int OTPGENERATION()
        {
            Random random = new Random();
            string r = "";
            try
            {
                for (int i = 1; i < 5; i++)
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
        public DataTable LoginEmailCheck(string EmailId)
        {
            string ErrorMessage = "";
            DataSet ds_custdet = new DataSet();

            string SendText = string.Empty;

            DataSet ds_custdet1 = new DataSet();
            DataTable dt_Login = new DataTable();
            try
            {
                if (EmailCheck(EmailId) > 0)
                {
                    DataRow row;
                    DataColumn col1 = new DataColumn("FirstName", typeof(string));
                    DataColumn col2 = new DataColumn("LastName", typeof(string));
                    DataColumn col3 = new DataColumn("MobileNo", typeof(string));
                    DataColumn col4 = new DataColumn("Gender", typeof(string));
                    DataColumn col5 = new DataColumn("dob", typeof(string));
                    DataColumn col6 = new DataColumn("Email", typeof(string));
                    DataColumn col7 = new DataColumn("HasDetailsConfirmed", typeof(bool));
                    DataColumn col8 = new DataColumn("IsUserLoggedIn", typeof(bool));
                    DataColumn col9 = new DataColumn("MobileDeviceID", typeof(string));
                    DataColumn col10 = new DataColumn("MembershipCode", typeof(string));

                    dt_Login.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8, col9, col10 });
                    string query1 = "select Top 1 EnquirePersonFirstName as FirstName,EnquirePersonLastName as LastName,MobileNo,Gender,D_O_B,Email,HasDetailsConfirmed,IsUserLoggedIn,MobileDeviceID,MembershhipCode='' from CCRMMEnquireForm where Email='" + EmailId + "' order by ID desc ";

                    using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
                    {
                        da_custdet.Fill(ds_custdet1);
                    }
                    if (ds_custdet1.Tables[0].Rows.Count > 0)
                    {
                        row = dt_Login.NewRow();
                        row["FirstName"] = ds_custdet1.Tables[0].Rows[0]["FirstName"].ToString();
                        row["LastName"] = ds_custdet1.Tables[0].Rows[0]["LastName"].ToString();
                        row["MobileNo"] = ds_custdet1.Tables[0].Rows[0]["MobileNo"].ToString();
                        row["Gender"] = ds_custdet1.Tables[0].Rows[0]["Gender"].ToString();
                        row["dob"] = ds_custdet1.Tables[0].Rows[0]["D_O_B"].ToString();
                        row["Email"] = ds_custdet1.Tables[0].Rows[0]["Email"].ToString();
                        row["HasDetailsConfirmed"] = Convert.ToBoolean(ds_custdet1.Tables[0].Rows[0]["HasDetailsConfirmed"].ToString());
                        row["IsUserLoggedIn"] = Convert.ToBoolean(ds_custdet1.Tables[0].Rows[0]["IsUserLoggedIn"].ToString());
                        row["MobileDeviceID"] = ds_custdet1.Tables[0].Rows[0]["MobileDeviceID"].ToString();
                        row["MembershipCode"] = "";
                        dt_Login.Rows.Add(row);


                    }
                    else
                    {
                        ErrorMessage = "Email is Wrong";
                    }
                }
                else
                {
                    ErrorMessage = "";

                }

            }
            catch (Exception ec)
            {
                ErrorMessage = ec.Message;
            }
            return dt_Login;
        }
        public int EmailCheck(string EmailId)
        {
            int EmailExist = 0;
            try
            {
                cnn.Close();
                string query1 = "select count(*) as UserAvailability from CCRMMEnquireForm where Email='" + EmailId + "' ";
                SqlCommand cmdExist = new SqlCommand(query1, cnn);
                cnn.Open();
                SqlDataReader drExist = cmdExist.ExecuteReader();
                if (drExist.Read())
                {
                    EmailExist = Convert.ToInt32(drExist[0].ToString());
                }
                cnn.Close();
            }
            catch (Exception ec)
            {
                EmailExist = 0;
            }

            return EmailExist;
        }
        public DataTable LoginEmailPasswordCheck(string Email, string Password)
        {

            DataSet ds_custdet = new DataSet();

            string SendText = string.Empty;

            DataSet ds_custdet1 = new DataSet();
            DataTable dt_Login = new DataTable();

            try
            {
                if (EmailCheck(Email) > 0)
                {
                    DataRow row;
                    DataColumn col1 = new DataColumn("UserName", typeof(string));
                    DataColumn col2 = new DataColumn("UserName", typeof(string));
                    DataColumn col3 = new DataColumn("MobileNo", typeof(string));
                    DataColumn col4 = new DataColumn("Gender", typeof(string));
                    DataColumn col5 = new DataColumn("Address", typeof(string));
                    DataColumn col6 = new DataColumn("Email", typeof(string));

                    dt_Login.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6 });
                    string query1 = "select Top 1 EnquirePersonFirstName as FirstName,EnquirePersonLastName as LastName, MobileNo,Gender,Address,Password,RoleId,Address,Email,Lat,Long from  CCRMMEnquireForm where Email='" + Email + "' and Password='" + Password + "' order by ID desc ";

                    using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
                    {
                        da_custdet.Fill(ds_custdet1);
                    }
                    if (ds_custdet1.Tables[0].Rows.Count > 0)
                    {
                        row = dt_Login.NewRow();
                        row["FirstName"] = ds_custdet1.Tables[0].Rows[0]["FirstName"].ToString();
                        row["LastName"] = ds_custdet1.Tables[0].Rows[0]["LastName"].ToString();
                        row["MobileNo"] = ds_custdet1.Tables[0].Rows[0]["MobileNo"].ToString();
                        row["Gender"] = ds_custdet1.Tables[0].Rows[0]["Gender"].ToString();
                        row["Address"] = ds_custdet1.Tables[0].Rows[0]["Address"].ToString();
                        row["Email"] = ds_custdet1.Tables[0].Rows[0]["Email"].ToString();
                        dt_Login.Rows.Add(row);

                    }
                    else
                    {

                    }
                }
                else
                {

                }

            }
            catch (Exception ec)
            {

            }
            return dt_Login;
        }
        public Object Login([FromBody]CustomerLogin Login)
        {

            string sJSONResponse = "";
            DataTable dt_Login = new DataTable();
            var json = (Object)null;
            try
            {
                if (Login.MobileNo != null && Login.EmailId == null || Login.MobileNo != null && Login.EmailId != null)
                {

                    dt_Login = LoginMobileCheck(Login.MobileNo);

                    if (dt_Login.Rows.Count > 0)
                    {

                        List<CustomerLoginNestedModel> CustomerLogin = new List<CustomerLoginNestedModel>();
                        CustomerLoginNestedModel CustomerLoginNeste = new CustomerLoginNestedModel();

                        CustomerLoginNeste.status = "Success";
                        CustomerLoginNeste.hasDetailsConfirmed = Convert.ToBoolean(dt_Login.Rows[0]["HasDetailsConfirmed"].ToString());
                        CustomerLoginNeste.errorMessage = "Mobile Number Exist In Our Records";
                        CustomerLoginNeste.isExist = true;
                        CustomerLoginNeste.personalDetails = PersonalDetailsEmail(dt_Login);
                        CustomerLoginNeste.isUserLoggedIn = Convert.ToBoolean(dt_Login.Rows[0]["isUserLoggedIn"].ToString());

                        if (Convert.ToBoolean(dt_Login.Rows[0]["isUserLoggedIn"].ToString()) == false)
                        {
                            CustomerLoginNeste.OTP = OTP(Login.MobileNo);
                        }

                        UpdateMobileDeviceId(Login.MobileNo, Login.MobileDeviceID, 1);
                        sJSONResponse = JsonConvert.SerializeObject(CustomerLoginNeste);

                    }
                    else
                    {

                        List<CustomerLoginNonNestedModel> CustomerLogin = new List<CustomerLoginNonNestedModel>();
                        CustomerLoginNonNestedModel CustomerLoginNonNeste = new CustomerLoginNonNestedModel();

                        CustomerLoginNonNeste.status = "Success";
                        CustomerLoginNonNeste.hasDetailsConfirmed = false;
                        CustomerLoginNonNeste.errorMessage = "";
                        CustomerLoginNonNeste.OTP = OTP(Login.MobileNo);
                        CustomerLoginNonNeste.isExist = false;
                        CustomerLoginNonNeste.isUserLoggedIn = false;
                        sJSONResponse = JsonConvert.SerializeObject(CustomerLoginNonNeste);
                    }

                }
                else if (Login.MobileNo == null && Login.EmailId != null)
                {
                    dt_Login = LoginEmailCheck(Login.EmailId);

                    if (dt_Login.Rows.Count > 0)
                    {

                        List<CustomerLoginEmailNestedModel> CustomerLogin = new List<CustomerLoginEmailNestedModel>();
                        CustomerLoginEmailNestedModel CustomerLoginNeste = new CustomerLoginEmailNestedModel();

                        CustomerLoginNeste.status = "Success";
                        CustomerLoginNeste.hasDetailsConfirmed = Convert.ToBoolean(dt_Login.Rows[0]["HasDetailsConfirmed"].ToString());
                        CustomerLoginNeste.isUserLoggedIn = Convert.ToBoolean(dt_Login.Rows[0]["IsUserLoggedIn"].ToString());
                        CustomerLoginNeste.isExist = true;
                        if (Convert.ToBoolean(dt_Login.Rows[0]["IsUserLoggedIn"].ToString()) == false)
                        {
                            CustomerLoginNeste.OTP = OTP(dt_Login.Rows[0]["MobileNo"].ToString());
                        }


                        CustomerLoginNeste.errorMessage = "EmailID Exist In Our Records";
                        CustomerLoginNeste.personalDetails = PersonalDetailsMobileNo(dt_Login);
                        UpdateMobileDeviceId(Login.MobileNo, Login.MobileDeviceID, 2);
                        sJSONResponse = JsonConvert.SerializeObject(CustomerLoginNeste);

                    }
                    else
                    {

                        List<CustomerLoginEmailNonNestedModel> CustomerLogin = new List<CustomerLoginEmailNonNestedModel>();
                        CustomerLoginEmailNonNestedModel CustomerLoginNeste = new CustomerLoginEmailNonNestedModel();

                        CustomerLoginNeste.status = "Success";
                        CustomerLoginNeste.hasDetailsConfirmed = false;
                        CustomerLoginNeste.isUserLoggedIn = false;
                        CustomerLoginNeste.errorMessage = "";
                        CustomerLoginNeste.isExist = false;

                        sJSONResponse = JsonConvert.SerializeObject(CustomerLoginNeste);
                    }
                }

            }
            catch (Exception ec)
            {
                json = ec.Message;
            }
            return sJSONResponse;
        }
        public List<PersonalDetails> PersonalDetailsClass(DataTable dt_PD)
        {
            List<PersonalDetails> Pdetails = new List<PersonalDetails>();

            Pdetails.Add(new PersonalDetails
            {
                FirstName = dt_PD.Rows[0]["FirstName"].ToString()
                ,
                LastName = dt_PD.Rows[0]["LastName"].ToString()
                ,
                MobileNo = dt_PD.Rows[0]["MobileNo"].ToString()
                ,
                Gender = dt_PD.Rows[0]["Gender"].ToString()
                ,
                Address = dt_PD.Rows[0]["Address"].ToString()
                ,
                Email = dt_PD.Rows[0]["Email"].ToString()
                ,
                MobileDeviceID = dt_PD.Rows[0]["MobileDeviceID"].ToString()
            });

            return Pdetails;
        }
        public Dictionary<string, string> PersonalDetailsMobileNo(DataTable dt_PD)
        {
            Dictionary<string, string> PersonalDetails = new Dictionary<string, string>();
            PersonalDetails.Add("FirstName", dt_PD.Rows[0]["FirstName"].ToString());
            PersonalDetails.Add("LastName", dt_PD.Rows[0]["LastName"].ToString());
            PersonalDetails.Add("MobileNo", dt_PD.Rows[0]["MobileNo"].ToString());
            PersonalDetails.Add("Gender", dt_PD.Rows[0]["Gender"].ToString());
            PersonalDetails.Add("MembershipCode", dt_PD.Rows[0]["MembershipCode"].ToString());

            DateTime dt = Convert.ToDateTime(dt_PD.Rows[0]["dob"].ToString());

            PersonalDetails.Add("dob", dt.ToString("MM/dd/yyyy"));

            return PersonalDetails;
        }
        public Dictionary<string, string> PersonalDetailsEmail(DataTable dt_PD)
        {
            Dictionary<string, string> PersonalDetails = new Dictionary<string, string>();
            PersonalDetails.Add("FirstName", dt_PD.Rows[0]["FirstName"].ToString());
            PersonalDetails.Add("LastName", dt_PD.Rows[0]["LastName"].ToString());
            PersonalDetails.Add("Gender", dt_PD.Rows[0]["Gender"].ToString());
            PersonalDetails.Add("MobileNo", dt_PD.Rows[0]["MobileNo"].ToString());
            PersonalDetails.Add("MembershipCode", dt_PD.Rows[0]["MembershipCode"].ToString());
            if (dt_PD.Rows[0]["dob"].ToString() != "")
            {
                DateTime dt = Convert.ToDateTime(dt_PD.Rows[0]["dob"].ToString());
                PersonalDetails.Add("dob", dt.ToString("MM/dd/yyyy"));
            }
            else
            {
                PersonalDetails.Add("dob", "");
            }


            //PersonalDetails.Add("dob", dt_PD.Rows[0]["dob"].ToString());
            PersonalDetails.Add("Email", dt_PD.Rows[0]["Email"].ToString());
            return PersonalDetails;
        }
        public string DataTableToJSONWithStringBuilder(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }
        public Object PersonalDetails([FromBody]CustomerLogin Users)
        {
            CustomerLoginResponseModelTypeA CustomerLoginNeste = new CustomerLoginResponseModelTypeA();
            SqlCommand command = cnn.CreateCommand();

            ArrayList arl_count = new ArrayList();
            DataSet ds_custdet = new DataSet();

            arl_count = CheckExist(Users.MobileDeviceID);

            ArrayList arl_Mobile = MobileCheck(Users.MobileNo);
            int EnquireformCount = Convert.ToInt32(arl_Mobile[0].ToString());
            int UserCount = Convert.ToInt32(arl_Mobile[1].ToString());

            string MobileNo = "";
            string MobileLst4 = "";

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            if (Users.isExist != true)
            {
                try
                {
                    if (EnquireformCount == 0)
                    {
                        try
                        {
                            if (EmailCheck(Users.EmailId) == 0)
                            {



                                cnn.Close();
                                cnn.Open();
                                SqlTransaction transaction;
                                transaction = cnn.BeginTransaction("SampleTransaction");
                                command.Connection = cnn;
                                command.Transaction = transaction;
                                try
                                {
                                    // User,EnquireForm
                                    if (Users.MobileNo != null && Users.EmailId == null || Users.MobileNo != null && Users.EmailId != null)
                                    {


                                        command.CommandText = "insert into CCRMMEnquireForm(BranchCode,MobileNo,EnquirePersonFirstName,EnquirePersonLastName,Gender,Email,D_O_B,OTP,Address,UniqueUsed,HasDetailsConfirmed,IsUserLoggedIn,MobileDeviceID,CreatedOn,RoleId,IsDeleted,IsActive) values('" + 1104 + "','" + Users.MobileNo + "','" + Users.FirstName + "','" + Users.LastName + "','" + Users.Gender + "','" + Users.EmailId + "','" + Users.DateOfBirth + "','','" + Users.Address + "',1,'" + Users.hasDetailsConfirmed + "','" + Users.isUserLoggedIn + "','" + Users.MobileDeviceID + "','" + ServerDateTime + "','6',0,1)";
                                        command.ExecuteNonQuery();
                                        //command.CommandText = "insert into  CCRMMEnquireStatus(MobileDeviceID,MobileNo,EnquireTypeNo,Status,CreatedOn,IsDeleted,IsActive) values('" + Users.MobileDeviceID + "','" + Users.MobileNo + "',1,'True','" + ServerDateTime + "',0,1)";
                                        //command.ExecuteNonQuery();


                                    }
                                    else if (Users.MobileNo == null && Users.EmailId != null)
                                    {
                                        command.CommandText = "insert into CCRMMEnquireForm(BranchCode,MobileNo,EnquirePersonFirstName,EnquirePersonLastName,Gender,Email,D_O_B,OTP,Address,UniqueUsed,HasDetailsConfirmed,IsUserLoggedIn,MobileDeviceID,CreatedOn,RoleId,IsDeleted,IsActive) values('" + 1104 + "','" + Users.MobileNo + "','" + Users.FirstName + "','" + Users.LastName + "','" + Users.Gender + "','" + Users.EmailId + "','" + Users.DateOfBirth + "','','" + Users.Address + "',2,'" + Users.hasDetailsConfirmed + "','" + Users.isUserLoggedIn + "','" + Users.MobileDeviceID + "','" + ServerDateTime + "','6',0,1)";
                                        command.ExecuteNonQuery();
                                        //command.CommandText = "insert into  CCRMMEnquireStatus(MobileDeviceID,MobileNo,EnquireTypeNo,Status,CreatedOn,IsDeleted,IsActive) values('" + Users.MobileDeviceID + "','" + Users.MobileNo + "',1,'True','" + ServerDateTime + "',0,1)";
                                        //command.ExecuteNonQuery();
                                    }

                                    transaction.Commit();

                                    CustomerLoginNeste.status = "Success";
                                    CustomerLoginNeste.errorMessage = "";
                                    CustomerLoginNeste.isExist = false;
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                }
                                finally
                                {

                                }

                            }
                            else
                            {
                                MobileNo = GetMobileidByEmailid(Users.Email);
                                MobileLst4 = MobileNo.Substring(MobileNo.Length - 4);

                                CustomerLoginNeste.status = "Fail";
                                CustomerLoginNeste.errorMessage = "EmailID Exist In Our Records with mobile number ending with  +91-XXXXXX" + MobileLst4;
                                CustomerLoginNeste.isExist = false;
                            }

                        }
                        catch (Exception ec)
                        {

                        }
                    }
                    else
                    {
                        CustomerLoginNeste.status = "success";
                        CustomerLoginNeste.errorMessage = "Mobile Exist";
                        CustomerLoginNeste.isExist = false;
                    }
                }
                catch (Exception ec)
                {
                }

            }
            else
            {
                try
                {
                    if (EnquireformCount > 0)
                    {
                        if (Convert.ToInt32(arl_count[1].ToString()) == 1)
                        {
                            cnn.Open();
                            transaction = cnn.BeginTransaction("SampleTransaction");
                            command.Connection = cnn;
                            command.Transaction = transaction;
                            command.CommandText = "update CCRMMEnquireForm set BranchCode='1104',EnquirePersonFirstName='" + Users.FirstName + "',EnquirePersonLastName='" + Users.LastName + "',Gender='" + Users.Gender + "',Email='" + Users.EmailId + "',Password='" + Users.Password + "',RoleId='" + Users.RoleCode + "',D_O_B='" + Users.DateOfBirth + "',Height='" + Users.Height + "',weight='" + Users.weight + "',HeightMeasurement='" + Users.HeightMeasurement + "',WeightMeasurement='" + Users.WeightMeasurement + "',BMI=" + Users.BMI + ",HowDidYouKnowAboutUs='" + Users.HowDidYouKnowAboutUs + "',PreferredCallBackTime='" + Users.PreferredCallBackTime + "',OTP='',Description='" + Users.Description + "',CreatedBy='" + Users.CreatedBy + "',IsUserLoggedIn='" + Users.isUserLoggedIn + "',HasDetailsConfirmed='" + Users.hasDetailsConfirmed + "' where MobileNo='" + Users.MobileNo + "' ";
                            command.ExecuteNonQuery();
                            command.CommandText = "update Users set Firstname='" + Users.FirstName + "',Lastname='" + Users.LastName + "',UserName='" + Users.FirstName +" "+ Users.LastName + "',Gender='" + Users.Gender + "',DateOfBirth='" + Users.DateOfBirth + "',Email='" + Users.Email + "' where MobileNo='" + Users.MobileNo + "' ";
                            command.ExecuteNonQuery();


                            //command.CommandText = "update CCRMMEnquireStatus set Status='1' where MobileNo='" + Users.MobileNo + "'";
                            //command.ExecuteNonQuery();

                            transaction.Commit();
                            CustomerLoginNeste.status = "Success";
                            CustomerLoginNeste.loginMode = 1;
                            CustomerLoginNeste.errorMessage = "";
                            CustomerLoginNeste.isExist = true;

                        }
                        else if (Convert.ToInt32(arl_count[1].ToString()) == 2)
                        {
                            cnn.Open();
                            transaction = cnn.BeginTransaction("SampleTransaction");
                            command.Connection = cnn;
                            command.Transaction = transaction;
                            command.CommandText = "update CCRMMEnquireForm set BranchCode='1104',EnquirePersonFirstName='" + Users.FirstName + "',EnquirePersonLastName='" + Users.LastName + "',Gender='" + Users.Gender + "',Email='" + Users.EmailId + "',Password='" + Users.Password + "',RoleId='" + Users.RoleCode + "',D_O_B='" + Users.DateOfBirth + "',Height='" + Users.Height + "',weight='" + Users.weight + "',HeightMeasurement='" + Users.HeightMeasurement + "',WeightMeasurement='" + Users.WeightMeasurement + "',BMI=" + Users.BMI + ",HowDidYouKnowAboutUs='" + Users.HowDidYouKnowAboutUs + "',PreferredCallBackTime='" + Users.PreferredCallBackTime + "',OTP='',Description='" + Users.Description + "',CreatedBy='" + Users.CreatedBy + "',MobileNo='" + Users.MobileNo + "',IsUserLoggedIn='" + Users.isUserLoggedIn + "',HasDetailsConfirmed='" + Users.hasDetailsConfirmed + "' where Email='" + Users.Email + "' ";
                            command.ExecuteNonQuery();
                            command.CommandText = "update Users set Firstname='" + Users.FirstName + "',Lastname='" + Users.LastName + "',UserName='" + Users.FirstName + " " + Users.LastName + "',Gender='" + Users.Gender + "',DateOfBirth='" + Users.DateOfBirth + "',Email='" + Users.Email + "' where MobileNo='" + Users.MobileNo + "' ";
                            command.ExecuteNonQuery();
                            //command.CommandText = "update CCRMMEnquireStatus set Status='1' where Email='" + Users.Email + "' ";
                            //command.ExecuteNonQuery();
                            transaction.Commit();

                            CustomerLoginNeste.status = "Success";
                            CustomerLoginNeste.OTP = OTP(Users.MobileNo);
                            CustomerLoginNeste.errorMessage = "";
                            CustomerLoginNeste.loginMode = 2;
                            CustomerLoginNeste.isExist = false;
                        }
                    }
                    else if (EnquireformCount == 0)
                    {

                        try
                        {
                            if (EmailCheck(Users.EmailId) == 0)
                            {
                                cnn.Close();
                                cnn.Open();
                                SqlTransaction transaction;
                                transaction = cnn.BeginTransaction("SampleTransaction");
                                command.Connection = cnn;
                                command.Transaction = transaction;
                                try
                                {
                                    if (Users.MobileNo != null && Users.EmailId == null || Users.MobileNo != null && Users.EmailId != null)
                                    {
                                        command.CommandText = "insert into CCRMMEnquireForm(BranchCode,MobileNo,EnquirePersonFirstName,EnquirePersonLastName,Gender,Email,D_O_B,OTP,Address,UniqueUsed,HasDetailsConfirmed,IsUserLoggedIn,MobileDeviceID,CreatedOn,RoleId,IsDeleted,IsActive) values('" + 1104 + "','" + Users.MobileNo + "','" + Users.FirstName + "','" + Users.LastName + "','" + Users.Gender + "','" + Users.EmailId + "','" + Users.DateOfBirth + "','','" + Users.Address + "',1,'" + Users.hasDetailsConfirmed + "','" + Users.isUserLoggedIn + "','" + Users.MobileDeviceID + "','" + ServerDateTime + "','6',0,1)";
                                        command.ExecuteNonQuery();
                                        //command.CommandText = "insert into  CCRMMEnquireStatus(MobileDeviceID,MobileNo,EnquireTypeNo,Status,CreatedOn,IsDeleted,IsActive) values('" + Users.MobileDeviceID + "','" + Users.MobileNo + "',1,'True','" + ServerDateTime + "',0,1)";
                                        //command.ExecuteNonQuery();


                                    }
                                    else if (Users.MobileNo == null && Users.EmailId != null)
                                    {
                                        command.CommandText = "insert into CCRMMEnquireForm(BranchCode,MobileNo,EnquirePersonFirstName,EnquirePersonLastName,Gender,Email,D_O_B,OTP,Address,UniqueUsed,HasDetailsConfirmed,IsUserLoggedIn,MobileDeviceID,CreatedOn,RoleId,IsDeleted,IsActive) values('" + 1104 + "','" + Users.MobileNo + "','" + Users.FirstName + "','" + Users.LastName + "','" + Users.Gender + "','" + Users.EmailId + "','" + Users.DateOfBirth + "','','" + Users.Address + "',2,'" + Users.hasDetailsConfirmed + "','" + Users.isUserLoggedIn + "','" + Users.MobileDeviceID + "','" + ServerDateTime + "','6',0,1)";
                                        command.ExecuteNonQuery();
                                        //command.CommandText = "insert into  CCRMMEnquireStatus(MobileDeviceID,MobileNo,EnquireTypeNo,Status,CreatedOn,IsDeleted,IsActive) values('" + Users.MobileDeviceID + "','" + Users.MobileNo + "',1,'True','" + ServerDateTime + "',0,1)";
                                        //command.ExecuteNonQuery();
                                    }

                                    transaction.Commit();

                                    CustomerLoginNeste.status = "Success";
                                    CustomerLoginNeste.errorMessage = "";
                                    CustomerLoginNeste.isExist = false;
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                }
                                finally
                                {

                                }

                            }
                            else
                            {
                                MobileNo = GetMobileidByEmailid(Users.Email);
                                MobileLst4 = MobileNo.Substring(MobileNo.Length - 4);

                                CustomerLoginNeste.status = "Fail";
                                CustomerLoginNeste.errorMessage = "EmailID Exist In Our Records with mobile number ending with  +91-XXXXXX" + MobileLst4;
                                CustomerLoginNeste.isExist = false;
                            }

                        }
                        catch (Exception ec)
                        {

                        }
                    }
                    else
                    {
                        CustomerLoginNeste.status = "Fail";
                        CustomerLoginNeste.errorMessage = "";
                        CustomerLoginNeste.isExist = true;

                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    CustomerLoginNeste.status = "Fail";
                    CustomerLoginNeste.errorMessage = "";
                    CustomerLoginNeste.isExist = false;
                }
                finally
                {
                    cnn.Close();
                }

            }

            string sJSONResponse = JsonConvert.SerializeObject(CustomerLoginNeste);
            return sJSONResponse;
        }
        public string GetMobileidByEmailid(string EmailId)
        {

            string MobileNo = "";
            try
            {
                cnn.Close();
                string query1 = "select MobileNo  from CCRMMEnquireForm where Email='" + EmailId + "' ";
                SqlCommand cmdExist = new SqlCommand(query1, cnn);
                cnn.Open();
                SqlDataReader drExist = cmdExist.ExecuteReader();
                if (drExist.Read())
                {
                    MobileNo = drExist[0].ToString();
                }
                cnn.Close();
            }
            catch (Exception ec)
            {

            }

            return MobileNo;
        }
        public ArrayList CheckExist(string MobileDeviceId)
        {

            ArrayList arl_Login = new ArrayList();
            try
            {
                cnn.Close();
                string query1 = "select count(*) as count,UniqueUsed from CCRMMEnquireForm where MobileDeviceID='" + MobileDeviceId + "' and UniqueUsed<>'' group by UniqueUsed  ";
                SqlCommand cmdExist = new SqlCommand(query1, cnn);
                cnn.Open();
                SqlDataReader drExist = cmdExist.ExecuteReader();
                if (drExist.Read())
                {
                    arl_Login.Add(Convert.ToInt32(drExist["count"].ToString()));
                    arl_Login.Add(Convert.ToInt32(drExist["UniqueUsed"].ToString()));
                }
                cnn.Close();
            }
            catch (Exception ec)
            {

            }

            return arl_Login;
        }
        public int UpdateMobileDeviceId(string MobileNo, string MobileDeviceID, int LoginMode)
        {
            int Updated = 0;
            try
            {
                cnn.Close();
                string query1 = "update CCRMMEnquireForm set MobileDeviceID='" + MobileDeviceID + "',UniqueUsed=" + LoginMode + " where MobileNo='" + MobileNo + "' ";
                SqlCommand cmdExist = new SqlCommand(query1, cnn);
                cnn.Open();
                cmdExist.ExecuteReader();
                cnn.Close();
            }
            catch (Exception ec)
            {
                Updated = 0;
            }

            return Updated;
        }
        public string tempmno()
        {
            Random random = new Random();
            string r = "";
            try
            {
                for (int i = 1; i < 5; i++)
                {
                    r += random.Next(1, 9).ToString();

                }
            }
            catch (Exception ex)
            {
                return "0";
            }
            return r;

        }
        private DataTable ReturnException(string Status, string Message)
        {
            DataTable dtMsg = new DataTable("MSG");
            dtMsg.Columns.Add("Status");
            dtMsg.Columns.Add("Value");
            DataRow drMsg = dtMsg.NewRow();
            drMsg["Status"] = Status;
            drMsg["Value"] = Message;
            dtMsg.Rows.Add(drMsg);
            return dtMsg;
        }
        public Object Logout([FromBody]CustomerLogout Logout)
        {
            LogoutFinalOutPut LOOP = new LogoutFinalOutPut();
            DataSet ds_CancelSlots = new DataSet();
            string sJSONResponse = "";
            DataSet ds_custdet = new DataSet();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            try
            {
                cnn.Open();
                SqlCommand command = cnn.CreateCommand();
                SqlTransaction transaction;
                transaction = cnn.BeginTransaction("SampleTransaction");
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "update CCRMMEnquireForm set IsUserLoggedIn=" + Logout.IsUserLoggedIn + " where MobileNo='" + Logout.MobileNo + "' ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    LOOP.status = "success";
                    LOOP.value = "";

                    sJSONResponse = JsonConvert.SerializeObject(LOOP);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    LOOP.status = "fail";
                    LOOP.value = "";

                    sJSONResponse = JsonConvert.SerializeObject(LOOP);
                }
                finally
                {

                }
            }
            catch (Exception ec)
            {
                LOOP.status = "fail";
                LOOP.value = "";

                sJSONResponse = JsonConvert.SerializeObject(LOOP);
            }
            return sJSONResponse;


        }


    }
}