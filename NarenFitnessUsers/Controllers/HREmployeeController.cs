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
using NarenFitnessUsers.Models.Employee;
using NarenFitnessUsers.Models.Employee.Performance;
using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using System.Drawing;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web.Configuration;
using System.Collections.Specialized;


namespace NarenFitnessUsers.Controllers
{
    public class HREmployeeController : Controller
    {
        // GET: HREmployee
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
        public string GetPhotoUrl(string Base64String, int Id)
        {
            string urlform = "";
            string endpath = "";
            try
            {

                string base64string = Base64String;
                var bytes = Convert.FromBase64String(base64string);

                endpath = "Icons" + Id + ".png";
                //C:\inetpub\wwwroot\GYMUI\UsersImages
                string filepath = @"C:\inetpub\wwwroot\GYMUI\UsersImages\\" + endpath;
                using (var imageFile = new FileStream(filepath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                //urlform = "http://137.59.201.211/GYMUI/EmployeeImages/" + endpath;
                urlform = "http://202.143.96.72/GYMUI/EmployeeImages/" + endpath;
            }
            catch (Exception ec)
            {
                urlform = "";
            }

            return urlform;
        }
        public string GetPhotoUrl3(string Base64String, int Id)
        {
            string urlform = "";
            string endpath = "";
            try
            {

                string base64string = Base64String;
                var bytes = Convert.FromBase64String(base64string);

                endpath = "Icons" + Id + ".png";
                //C:\inetpub\wwwroot\GYMUI\UsersImages
                string filepath = @"C:\inetpub\wwwroot\GYMUI\CertificateImages\\" + endpath;
                using (var imageFile = new FileStream(filepath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                //urlform = "http://137.59.201.211/GYMUI/EmployeeImages/" + endpath;
                urlform = "http://202.143.96.72/GYMUI/CertificateImages/" + endpath;
            }
            catch (Exception ec)
            {
                urlform = "";
            }

            return urlform;
        }

        public string GetPhotoUrl4(string Base64String, int Id)
        {
            string urlform = "";
            string endpath = "";
            try
            {

                string base64string = Base64String;
                var bytes = Convert.FromBase64String(base64string);

                endpath = "Icons" + Id + ".png";
                //C:\inetpub\wwwroot\GYMUI\UsersImages
                string filepath = @"C:\inetpub\wwwroot\GYMUI\University\\" + endpath;
                using (var imageFile = new FileStream(filepath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                //urlform = "http://137.59.201.211/GYMUI/EmployeeImages/" + endpath;
                urlform = "http://202.143.96.72/GYMUI/University/" + endpath;
            }
            catch (Exception ec)
            {
                urlform = "";
            }

            return urlform;
        }

        public string GetPhotoUrl2(string Base64String, int Id)
        {
            string urlform = "";
            string endpath = "";
            try
            {

                string base64string = Base64String;
                var bytes = Convert.FromBase64String(base64string);

                endpath = "Icons" + Id + ".png";
                //C:\inetpub\wwwroot\GYMUI\UsersImages
                string filepath = @"C:\inetpub\wwwroot\GYMUI\EmployeePhotoes\\" + endpath;
                using (var imageFile = new FileStream(filepath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                //urlform = "http://137.59.201.211/GYMUI/EmployeeImages/" + endpath;
                urlform = "http://202.143.96.72/GYMUI/EmployeePhotoes/" + endpath;
            }
            catch (Exception ec)
            {
                urlform = "";
            }

            return urlform;
        }
        public Object EmployeePhotoEdit([FromBody]EmployeeEdit ee)
        {

            string b = ee.Photo;
            b = b.Substring(b.IndexOf(",") + 1);
            string PhotoUrl = GetPhotoUrl(b, imgname());
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
                    command.CommandText = "update HREmployee set PhotoURL='" + PhotoUrl + "',Photo='" + b + "' where EmployeeCode='" + ee.TrainerCode + "' ";
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

        public Object HREmpCertificatesPost([FromBody]HREEmployeeInput EI)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            // ertifi
            string b = EI.CertificateImage;
            try
            {
                b = b.Substring(b.IndexOf(",") + 1);
            }
            catch (Exception ec)
            {
                b = "0";
            }
            string CertificateUrl = GetPhotoUrl3(b, imgname() + 10001);

            string country_Query = "insert into HREmpCertificates(EmployeeCode,CertificateName,CertificateUrl,CertificateValidityDate,UniversityName,CertificationDate,CertificationDuration,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + EI.EmployeeCode + "','" + EI.CertificateName + "','" + CertificateUrl + "','" + EI.CertificateValidityDate + "','" + EI.UniversityName + "','" + EI.YearOfPassing + "','" + EI.CertificationDuration + "','" + EI.CreatedBy + "','" + ServerDateTime + "',0,1)";
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();

            try
            {

                cnn.Open();
                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                opt.status = "success";

            }
            catch (Exception ex)
            {
                opt.status = "fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);

            return sJSONResponse;
        }
        public Object HREmpCertificatesUpdate([FromBody]HREEmployeeInput EI)
        {
            string sJSONResponse = "";
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            try
            {
                string b = EI.CertificateImage;
                try
                {
                    b = b.Substring(b.IndexOf(",") + 1);
                }
                catch (Exception ec)
                {
                    b = "0";
                }
                string CertificateUrl = GetPhotoUrl3(b, imgname() + 10001);

                cnn.Open();
                SqlTransaction transaction;
                transaction = cnn.BeginTransaction("SampleTransaction");
                command.Connection = cnn;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = "update HREmpCertificates set EmployeeCode='" + EI.EmployeeCode + "',CertificateName='" + EI.CertificateName + "',CertificateUrl='" + CertificateUrl + "',CertificateValidityDate='" + EI.CertificateValidityDate + "',UniversityName='" + EI.UniversityName + "',CertificationDate='" + EI.CertificationDate + "',CertificationDuration='" + EI.CertificationDuration + "'  where ID = " + EI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();

                    opt.status = "success";
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
                opt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);

            return sJSONResponse;
        }
        public Object HREmpCertificatesDelete([FromBody]HREEmployeeInput EI)
        {
            string sJSONResponse = "";
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
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
                    command.CommandText = "delete from HREmpCertificates where ID=" + EI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);

            return sJSONResponse;
        }
        public Object GetHREmpCertificates([FromBody]HREEmployeeInput EI)
        {

            HREmpCertificatesList cl = new HREmpCertificatesList();

            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Certificates = new DataTable();

            List<HREmpCertificates> Certificate = new List<HREmpCertificates>();


            try
            {
                dt_Certificates = getdata(string.Format("select ID,CertificateName,CertificateUrl,CertificateValidityDate,UniversityName,CertificationDate,CertificationDuration from HREmpCertificates where EmployeeCode = '{0}'", EI.EmployeeCode));

                for (int i = 0; i < dt_Certificates.Rows.Count; i++)
                {
                    HREmpCertificates ctf = new HREmpCertificates { ID = Convert.ToInt32(dt_Certificates.Rows[i]["ID"]), CertificateUrl = dt_Certificates.Rows[i]["CertificateUrl"].ToString(), CertificateValidityDate = dt_Certificates.Rows[i]["CertificateValidityDate"].ToString(), CertificationDuration = dt_Certificates.Rows[i]["CertificationDuration"].ToString(), CertificateName = dt_Certificates.Rows[i]["CertificateName"].ToString(), UniversityName = dt_Certificates.Rows[i]["UniversityName"].ToString(), YearOfPassing = dt_Certificates.Rows[i]["CertificationDate"].ToString() };
                    Certificate.Add(ctf);
                }

                cl.status = "success";
                cl.value = Certificate;
                sJSONResponse = JsonConvert.SerializeObject(cl);


            }
            catch (Exception ec)
            {
                cl.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(cl);
            }


            return sJSONResponse;
        }

        public Object HREmpExperienceDetailsPost([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;


            try
            {

                cnn.Open();
                string country_Query = "insert into HREmpExperienceDetails(EmployeeCode,OrganizationName, FromDate, ToDate, Role, AwardsRewards, Notes, CreatedBy, CreatedOn, IsDeleted, IsActive) values('" + EI.EmployeeCode + "','" + EI.OrganizationName + "','" + EI.FromDate + "','" + EI.ToDate + "','" + EI.Role + "','" + EI.AwardsRewards + "','" + EI.Notes + "','" + EI.CreatedBy + "','" + ServerDateTime + "',0,1)";

                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                opt.status = "success";

            }
            catch (Exception ex)
            {
                opt.status = "fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);

            return sJSONResponse;
        }
        public Object HREmpExperienceDetailsUpdate([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";
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
                    command.CommandText = "update HREmpExperienceDetails set OrganizationName = '" + EI.OrganizationName + "', FromDate = '" + EI.FromDate + "', ToDate = '" + EI.ToDate + "', Role='" + EI.Role + "', AwardsRewards = '" + EI.AwardsRewards + "', Notes = '" + EI.Notes + "'  where ID = " + EI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object HREmpExperienceDetailsDelete([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";
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
                    command.CommandText = "delete from HREmpExperienceDetails where ID=" + EI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object GetHREmpExperienceDetails([FromBody]HREEmployeeInput EI)
        {
            HREmpExperienceDetailsList el = new HREmpExperienceDetailsList();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Experience = new DataTable();

            List<HREmpExperienceDetails> Experience = new List<HREmpExperienceDetails>();

            try
            {
                dt_Experience = getdata(string.Format("select ID,OrganizationName, FromDate, ToDate, Role, AwardsRewards, Notes from HREmpExperienceDetails where EmployeeCode = '" + EI.EmployeeCode + "'", ""));

                for (int i = 0; i < dt_Experience.Rows.Count; i++)
                {
                    HREmpExperienceDetails exp = new HREmpExperienceDetails { ID = Convert.ToInt32(dt_Experience.Rows[i]["ID"].ToString()), OrganizationName = dt_Experience.Rows[i]["OrganizationName"].ToString(), FromDate = dt_Experience.Rows[i]["FromDate"].ToString(), ToDate = dt_Experience.Rows[i]["ToDate"].ToString(), Role = dt_Experience.Rows[i]["Role"].ToString(), AwardsRewards = dt_Experience.Rows[i]["AwardsRewards"].ToString(), Notes = dt_Experience.Rows[i]["Notes"].ToString() };
                    Experience.Add(exp);
                }

                el.status = "success";
                el.value = Experience;
                sJSONResponse = JsonConvert.SerializeObject(el);

            }
            catch (Exception ec)
            {
                el.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(el);
            }


            return sJSONResponse;
        }

        public Object HREmpEducationDetailsPost([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string country_Query = "";


            try
            {

                cnn.Open();
                country_Query = "insert into HREmpEducationDetails(EmployeeCode,Stream, UniversityName, PassPercentage, Grade, YearOfPassing, CreatedBy, CreatedOn, IsDeleted, IsActive) values('" + EI.EmployeeCode + "','" + EI.Stream + "','" + EI.UniversityName + "','" + EI.PassPercentage + "','" + EI.Grade + "','" + EI.YearOfPassing + "','" + EI.CreatedBy + "','" + ServerDateTime + "',0,1)";
                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                opt.status = "success";

            }
            catch (Exception ex)
            {
                opt.status = "success";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);

            return sJSONResponse;
        }
        public Object HREmpEducationDetailsUpdate([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            string sJSONResponse = "";
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
                    command.CommandText = "update HREmpEducationDetails set Stream = '" + EI.Stream + "', UniversityName = '" + EI.UniversityName + "', PassPercentage = '" + EI.PassPercentage + "', Grade = '" + EI.Grade + "', YearOfPassing = '" + EI.YearOfPassing + "'  where ID =" + EI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object HREmpEducationDetailsDelete([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            string sJSONResponse = "";
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
                    command.CommandText = "delete from HREmpEducationDetails where ID=" + EI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "success";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object GetHREmpEducationDetails([FromBody]HREEmployeeInput EI)
        {
            HREmpEducationDetailsList El = new HREmpEducationDetailsList();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Education = new DataTable();

            List<HREmpEducationDetails> Education = new List<HREmpEducationDetails>();

            try
            {
                dt_Education = getdata(string.Format("select ID,Stream, UniversityName, PassPercentage, Grade, YearOfPassing from HREmpEducationDetails where EmployeeCode = '{0}'", EI.EmployeeCode));

                for (int i = 0; i < dt_Education.Rows.Count; i++)
                {
                    HREmpEducationDetails edu = new HREmpEducationDetails { ID = Convert.ToInt32(dt_Education.Rows[i]["ID"].ToString()), Stream = dt_Education.Rows[i]["Stream"].ToString(), UniversityName = dt_Education.Rows[i]["UniversityName"].ToString(), PassPercentage = dt_Education.Rows[i]["PassPercentage"].ToString(), Grade = dt_Education.Rows[i]["Grade"].ToString(), YearOfPassing = dt_Education.Rows[i]["YearOfPassing"].ToString() };
                    Education.Add(edu);
                }

                El.status = "success";
                El.value = Education;
                sJSONResponse = JsonConvert.SerializeObject(El);


            }
            catch (Exception ec)
            {
                El.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(El);
            }


            return sJSONResponse;
        }

        public Object HREmpSpecialistsPost([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string country_Query = "";


            try
            {

                cnn.Open();
                country_Query = "insert into HREmpSpecialists(EmployeeCode,SpecializationName,GradeName, CreatedBy, CreatedOn, IsDeleted, IsActive) values('" + EI.EmployeeCode + "','" + EI.SpecializationName + "','"+EI.GradeName+"','" + EI.CreatedBy + "','" + ServerDateTime + "',0,1)";
                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());

                opt.status = "success";

            }
            catch (Exception ex)
            {
                opt.status = "fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);

            return sJSONResponse;
        }
        public Object HREmpSpecialistsUpdate([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";
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
                    command.CommandText = "update HREmpSpecialists set SpecializationName = '" + EI.SpecializationName + "',GradeName='"+EI.GradeName+ "',EmployeeCode='"+EI.EmployeeCode+"'  where ID = " + EI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "success";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object HREmpSpecialistsDelete([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";
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
                    command.CommandText = "delete from HREmpSpecialists where ID=" + EI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object GetHREmpSpecialists([FromBody]HREEmployeeInput EI)
        {
            HREmpSpecialistsList Sl = new HREmpSpecialistsList();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Specilization = new DataTable();

            List<HREmpSpecialists> Specilization = new List<HREmpSpecialists>();

            try
            {
                dt_Specilization = getdata(string.Format("select ID,SpecializationName,GradeName from HREmpSpecialists where EmployeeCode ='{0}' ", EI.EmployeeCode));

                for (int i = 0; i < dt_Specilization.Rows.Count; i++)
                {
                    HREmpSpecialists spl = new HREmpSpecialists { ID = Convert.ToInt32(dt_Specilization.Rows[i]["ID"].ToString()), SpecializationName = dt_Specilization.Rows[i]["SpecializationName"].ToString(), GradeName = dt_Specilization.Rows[i]["GradeName"].ToString() };
                    Specilization.Add(spl);
                }

                Sl.status = "success";
                Sl.value = Specilization;
                sJSONResponse = JsonConvert.SerializeObject(Sl);


            }
            catch (Exception ec)
            {
                Sl.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(Sl);
            }


            return sJSONResponse;
        }

        public Object HREmpUploadFilesPost([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string country_Query = "";


            try
            {

                cnn.Open();
                country_Query = "insert into HREmpUploadFiles(EmployeeCode,FileName, UploadedImage, CreatedBy, CreatedOn, IsDeleted, IsActive) values('" + EI.EmployeeCode + "','" + EI.FileName + "','" + EI.UploadedImage + "','" + EI.CreatedBy + "','" + ServerDateTime + "',0,1)";
                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                opt.status = "success";

            }
            catch (Exception ex)
            {
                opt.status = "fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);

            return sJSONResponse;
        }
        public Object HREmpUploadFilesUpdate([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";
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
                    command.CommandText = "update HREmpUploadFiles set FileName = '" + EI.FileName + "', UploadedImage = '" + EI.UploadedImage + "'  where ID = " + EI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "success";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object HREmpUploadFilesDelete([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";
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
                    command.CommandText = "delete from HREmpUploadFiles where ID=" + EI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "success";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object GetHREmpUploadFiles([FromBody]HREEmployeeInput EI)
        {
            //9912911068 pras
            HREmpUploadFilesList UF = new HREmpUploadFilesList();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_UploadFile = new DataTable();

            List<HREmpUploadFiles> UploadFile = new List<HREmpUploadFiles>();

            try
            {
                dt_UploadFile = getdata(string.Format("select ID,FileName, UploadedImage, CreatedBy, CreatedOn, IsDeleted, IsActive from HREmpUploadFiles where EmployeeCode = '{0}'", EI.EmployeeCode));

                for (int i = 0; i < dt_UploadFile.Rows.Count; i++)
                {
                    HREmpUploadFiles uldfl = new HREmpUploadFiles { ID = Convert.ToInt32(dt_UploadFile.Rows[i]["ID"].ToString()), FileName = dt_UploadFile.Rows[i]["FileName"].ToString(), UploadedImage = dt_UploadFile.Rows[i]["UploadedImage"].ToString() };
                    UploadFile.Add(uldfl);
                }

                UF.status = "success";
                UF.value = UploadFile;
                sJSONResponse = JsonConvert.SerializeObject(UF);


            }
            catch (Exception ec)
            {
                UF.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(UF);
            }


            return sJSONResponse;
        }

        public Object HREmpVideosPost([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string country_Query = "";


            try
            {

                cnn.Open();
                country_Query = "insert into HREmpVideos(EmployeeCode,VideoName, VideoUrl, Video, CreatedBy, CreatedOn, IsDeleted, IsActive) values('" + EI.EmployeeCode + "','" + EI.VideoName + "','" + EI.VideoUrl + "','" + EI.Video + "','" + EI.CreatedBy + "','" + ServerDateTime + "',0,1)";
                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());

                opt.status = "success";
            }
            catch (Exception ex)
            {
                opt.status = "success";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);

            return sJSONResponse;
        }
        public Object HREmpVideosUpdate([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";
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
                    command.CommandText = "update HREmpVideos set VideoName = '" + EI.VideoName + "', VideoUrl = '" + EI.VideoUrl + "', Video = '" + EI.Video + "'  where ID = " + EI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "success";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object HREmpVideosDelete([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";
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
                    command.CommandText = "delete from HREmpVideos where ID=" + EI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "success";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object GetHREmpVideos([FromBody]HREEmployeeInput EI)
        {
            HREmpVideosList UF = new HREmpVideosList();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Upload = new DataTable();
            List<HREmpVideos> video = new List<HREmpVideos>();
            try
            {
                dt_Upload = getdata(string.Format("select ID,VideoName, VideoUrl, Video from HREmpVideos where EmployeeCode = '{0}'", EI.EmployeeCode));

                for (int i = 0; i < dt_Upload.Rows.Count; i++)
                {
                    HREmpVideos Video = new HREmpVideos { ID = Convert.ToInt32(dt_Upload.Rows[i]["ID"].ToString()), Video = dt_Upload.Rows[i]["Video"].ToString(), VideoName = dt_Upload.Rows[i]["VideoName"].ToString(), VideoUrl = dt_Upload.Rows[i]["VideoUrl"].ToString() };
                    video.Add(Video);
                }

                UF.status = "success";
                UF.value = video;
                sJSONResponse = JsonConvert.SerializeObject(UF);


            }
            catch (Exception ec)
            {
                UF.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(UF);
            }


            return sJSONResponse;
        }

        public Object HREmpImagesPost([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;

            string b = EI.Image;
            b = b.Substring(b.IndexOf(",") + 1);
            string PhotoUrl = GetPhotoUrl2(b, imgname() + 10001);

            try
            {

                cnn.Open();
                string country_Query = "insert into HREmpImages(EmployeeCode,ImageName, Image, ImageUrl, CreatedBy, CreatedOn, IsDeleted, IsActive) values('" + EI.EmployeeCode + "','" + EI.ImageName + "','" + EI.Image + "','" + PhotoUrl + "','" + EI.CreatedBy + "','" + ServerDateTime + "',0,1)";
                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                opt.status = "success";

            }
            catch (Exception ex)
            {
                opt.status = "fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);

            return sJSONResponse;
        }
        public Object HREmpImagesUpdate([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            string b = EI.Image;
            b = b.Substring(b.IndexOf(",") + 1);
            string PhotoUrl = GetPhotoUrl2(b, imgname() + 10001);
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
                    command.CommandText = "update HREmpImages set ImageName = '" + EI.ImageName + "', Image = '" + EI.Image + "', ImageUrl = '" + PhotoUrl + "' where ID = " + EI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object HREmpImagesDelete([FromBody]HREEmployeeInput EI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";
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
                    command.CommandText = "delete from HREmpImages where ID=" + EI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object GetHREmpImages([FromBody]HREEmployeeInput EI)
        {
            HREmpImagesList IL = new HREmpImagesList();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Image = new DataTable();

            List<HREmpImages> Image = new List<HREmpImages>();

            try
            {
                dt_Image = getdata(string.Format("select ID,ImageName, Image, ImageUrl from HREmpImages where EmployeeCode = '{0}'", EI.EmployeeCode));

                for (int i = 0; i < dt_Image.Rows.Count; i++)
                {
                    HREmpImages img = new HREmpImages { ID = Convert.ToInt32(dt_Image.Rows[i]["ID"].ToString()), ImageName = dt_Image.Rows[i]["ImageName"].ToString(), Image = dt_Image.Rows[i]["Image"].ToString(), ImageUrl = dt_Image.Rows[i]["ImageUrl"].ToString() };
                    Image.Add(img);
                }

                IL.status = "success";
                IL.value = Image;
                sJSONResponse = JsonConvert.SerializeObject(IL);


            }
            catch (Exception ec)
            {
                IL.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(IL);
            }


            return sJSONResponse;
        }
        public ActionResult UploadImages(List<HttpPostedFileBase> file)
        {
            string filepath = @"C:\inetpub\wwwroot\GYMUI\UsersImages\\";
            foreach (HttpPostedFileBase postedFile in file) //POR CADA ARCHIVO QUE SUBE EL USUARIO...
            {
                if (postedFile != null)
                {
                    string fileName = Path.GetFileName(postedFile.FileName); //Guardo el Filename como String
                    postedFile.SaveAs(filepath + fileName);
                }
            }

            return Content("Im not sure what i have to return");
        }
        public Object FileUpload(Stream Uploading)
        {
            DataSet ds = new DataSet();
            string FileName = Guid.NewGuid().ToString();

            //Path.Combine(Path.GetTempPath(), FileName)
            UploadFile upload = new UploadFile
            {
                FilePath = Path.Combine(@"C:\inetpub\wwwroot\GYMUI\RawAudioVideo", FileName)
            };


            int FileLength = upload.FileLength;

            //string file = HttpContext.Current.Request.Path;

            //MultipartParser parser = new MultipartParser(stream);


            string results = "";


            int length = 0;
            using (FileStream writer = new FileStream(upload.FilePath, FileMode.Create))
            {

                int readCount;
                var buffer = new byte[8192];
                while ((readCount = Uploading.Read(buffer, 0, buffer.Length)) != 0)
                {
                    writer.Write(buffer, 0, readCount);
                    length += readCount;
                }
            }

            upload.FileLength = length;

            //Download File


            string filePath = @"C:\inetpub\wwwroot\GYMUI\RawAudioVideo\" + FileName + "";
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
            // check if exists
            if (!fileInfo.Exists) throw new System.IO.FileNotFoundException("File not found", FileName);

            // open stream
            System.IO.FileStream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            byte[] buffer2 = new byte[stream.Length];
            stream.Read(buffer2, 0, (int)stream.Length);

            string fileInString = Convert.ToBase64String(buffer2);

            byte[] ret = Convert.FromBase64String(fileInString);
            // FileInfo fil = new FileInfo("D://test.mp4");
            string filepath = @"C:\inetpub\wwwroot\GYMUI\Voice\" + FileName + ".mp4";
            //string filepath = @"C:\inetpub\wwwroot\SUCV3.1D\Voice\123.mp4";
            using (var imageFile = new FileStream(filepath, FileMode.Create))
            {
                imageFile.Write(ret, 0, ret.Length);
                imageFile.Close();
            }

            //var content = new MultipartFormDataContent();
            //var file_content = new ByteArrayContent(new StreamContent(Uploading).ReadAsByteArrayAsync().Result);

            string urlform = "http://202.143.96.72/GYMUI/Voice/" + FileName + ".mp4";

            DataTable newTable = new DataTable();
            DataRow row;

            DataColumn col1 = new DataColumn("FileName", typeof(string));
            DataColumn col2 = new DataColumn("FileUrl", typeof(string));

            newTable.Columns.AddRange(new DataColumn[] { col1, col2 });


            row = newTable.NewRow();
            row["FileName"] = FileName;
            row["FileUrl"] = urlform;

            newTable.Rows.Add(row);



            //return results;

            return results;
        }

        // Step 1 : Function Names : Performance Evaluation Standards : (CRUD) 
        // Create : Columns  EvaluationName , Score 
        // FunctioName : Performance Evaluation Names : (CRUD) 
        // Create : PESId, PEName (sub evaluation Name) , score
        // Function Name  : PerformanceEvaluationScore (CRUD)
        // Create : PENId , EmployeeCode , Score 

        // Get standard table object: {ayyay : }


        public Object PerformanceEvaluationPost([FromBody]PEInput PEI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;



            try
            {

                cnn.Open();
                string country_Query = "insert into PerformanceEvaluationStandards(EvaluationName,Score,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + PEI.EvaluationName + "','" + PEI.Score + "','" + PEI.CreatedBy + "','" + ServerDateTime + "',0,1)";
                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                opt.status = "success";

            }
            catch (Exception ex)
            {
                opt.status = "fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);

            return sJSONResponse;
        }
        public Object PerformanceEvaluationUpdate([FromBody]PEInput PEI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";

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
                    command.CommandText = "update PerformanceEvaluationStandards set EvaluationName = '" + PEI.EvaluationName + "', Score = '" + PEI.Score + "' where SNO = " + PEI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object PerformanceEvaluationDelete([FromBody]PEInput PEI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";
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
                    command.CommandText = "delete from  PerformanceEvaluationStandards where SNO='" + PEI.Id + "' ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object GetPerformanceEvaluation([FromBody]PEInput PEI)
        {
            PEStandardOutput PESO = new PEStandardOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_standards = new DataTable();

            List<PEStandard> standards = new List<PEStandard>();

            try
            {
                dt_standards = getdata(string.Format("Select SNO,EvaluationName,Score from PerformanceEvaluationStandards"));

                for (int i = 0; i < dt_standards.Rows.Count; i++)
                {
                    PEStandard standardsdat = new PEStandard { Id = Convert.ToInt32(dt_standards.Rows[i]["SNO"].ToString()), EvaluationName = dt_standards.Rows[i]["EvaluationName"].ToString(), Score = dt_standards.Rows[i]["Score"].ToString() };
                    standards.Add(standardsdat);
                }

                PESO.status = "success";
                PESO.value = standards;
                sJSONResponse = JsonConvert.SerializeObject(PESO);


            }
            catch (Exception ec)
            {
                PESO.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(PESO);
            }


            return sJSONResponse;
        }


        public Object PerformanceEvaluationNamePost([FromBody]PEInput PEI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;

            try
            {

                cnn.Open();
                string country_Query = "insert into PerformanceEvaluationNames(PESubName,PESID,Score,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + PEI.PENames + "','" + PEI.PESID + "','" + PEI.Score + "','" + PEI.CreatedBy + "','" + ServerDateTime + "',0,1)";
                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                opt.status = "success";

            }
            catch (Exception ex)
            {
                opt.status = "fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);

            return sJSONResponse;
        }
        public Object PerformanceEvaluationNameUpdate([FromBody]PEInput PEI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";

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
                    command.CommandText = "update PerformanceEvaluationNames set PESubName = '" + PEI.PESubName + "',Score = '" + PEI.Score + "' where SNO = " + PEI.Id + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object PerformanceEvaluationNameDelete([FromBody]PEInput PEI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";
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
                    command.CommandText = "delete from  PerformanceEvaluationNames where SNO='" + PEI.Id + "' ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object GetPerformanceEvaluationName([FromBody]PEInput PEI)
        {
            PENameOutput PENOP = new PENameOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_standards = new DataTable();

            List<PEName> standards = new List<PEName>();

            try
            {
                dt_standards = getdata(string.Format("Select SNO,PESubName,Score from PerformanceEvaluationNames where PESID='{0}'",PEI.PESID));

                for (int i = 0; i < dt_standards.Rows.Count; i++)
                {
                    PEName standardsdat = new PEName { Id = Convert.ToInt32(dt_standards.Rows[i]["SNO"].ToString()), PENames = dt_standards.Rows[i]["PESubName"].ToString(), Score = dt_standards.Rows[i]["Score"].ToString() };
                    standards.Add(standardsdat);
                }

                PENOP.status = "success";
                PENOP.value = standards;
                sJSONResponse = JsonConvert.SerializeObject(PENOP);


            }
            catch (Exception ec)
            {
                PENOP.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(PENOP);
            }


            return sJSONResponse;
        }


        public Object PerformanceEvaluationScorePost1([FromBody]PEInput PEI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;

            try
            {


                cnn.Open();
                string country_Query = "insert into PerformanceEvaluationScore(EmployeeCode,PESNID,Score,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + PEI.EmployeeCode + "','" + PEI.PESNID + "','" + PEI.Score + "','" + PEI.CreatedBy + "','" + ServerDateTime + "',0,1) ";
                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                opt.status = "success";

            }
            catch (Exception ex)
            {
                opt.status = "fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);

            return sJSONResponse;
        }
        public Object PerformanceEvaluationScorePost([FromBody] List <PEInput> PEI)

        {

            HREEmployeePlanOutput daOP = new HREEmployeePlanOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;


            try
            {
                for (int i = 0; i <= PEI.Count-1; i++)
                {

                    cnn.Open();
                    transaction = cnn.BeginTransaction("SampleTransaction");
                    command.Connection = cnn;
                    command.Transaction = transaction;
                    command.CommandText = "insert into PerformanceEvaluationScore(EmployeeCode,PESNID,Score,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + PEI[i].EmployeeCode + "','" + PEI[i].PESNID + "','" + PEI[i].Score + "','" + PEI[i].CreatedBy + "','',0,1)";
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


        public Object PerformanceEvaluationScoreUpdate([FromBody]PEInput PEI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";

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
                    command.CommandText = "update PerformanceEvaluationScore set EmployeeCode='" + PEI.EmployeeCode + "',PESNID='" + PEI.PESNID + "',Score='" + PEI.Score + "' where SNO='" + PEI.Id + "' ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object PerformanceEvaluationScoreDelete([FromBody]PEInput PEI)
        {
            HREEmployeePlanOutput opt = new HREEmployeePlanOutput();
            SqlCommand command = cnn.CreateCommand();
            string sJSONResponse = "";
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
                    command.CommandText = "delete from  PerformanceEvaluationScore where SNO='" + PEI.Id + "' ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    opt.status = "success";
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
                opt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(opt);
            return sJSONResponse;
        }
        public Object GetPerformanceEvaluationScore([FromBody]PEInput PEI)
        {
            PEScoreOutput PESO = new PEScoreOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_standards = new DataTable();

            List<PEScore> score = new List<PEScore>();

            try
            {
                dt_standards = getdata(string.Format("Select SNO,EmployeeCode,PESNID,Score from PerformanceEvaluationScore"));

                for (int i = 0; i < dt_standards.Rows.Count; i++)
                {
                    PEScore scoredat = new PEScore { Id = Convert.ToInt32(dt_standards.Rows[i]["SNO"].ToString()), EmployeeCode = dt_standards.Rows[i]["EmployeeCode"].ToString(), PESNID = Convert.ToInt32(dt_standards.Rows[i]["PESNID"].ToString()), Score = Convert.ToInt32(dt_standards.Rows[i]["Score"].ToString()) };
                    score.Add(scoredat);
                }

                PESO.status = "success";
                PESO.value = score;
                sJSONResponse = JsonConvert.SerializeObject(PESO);


            }
            catch (Exception ec)
            {
                PESO.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(PESO);
            }


            return sJSONResponse;
        }

        public Object GetPerformancestandardNames([FromBody]PEInput PEI)
        {
            PESNNames PENOP = new PESNNames();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_standards = new DataTable();

            List<PESNOutput> standards = new List<PESNOutput>();

            try
            {
                dt_standards = getdata(string.Format("Select PEN.SNO,PEN.PESubName,PEN.Score,PESID,PES.EvaluationName from PerformanceEvaluationNames PEN,PerformanceEvaluationStandards PES where PEN.PESID=PES.SNO and PEN.PESID=2",PEI.PESID));

                for (int i = 0; i < dt_standards.Rows.Count; i++)
                {
                    PESNOutput standardsdat = new PESNOutput { Id = Convert.ToInt32(dt_standards.Rows[i]["SNO"].ToString()), PeNames = dt_standards.Rows[i]["PESubName"].ToString(), Score = dt_standards.Rows[i]["Score"].ToString(), EvaluationName = dt_standards.Rows[i]["EvaluationName"].ToString() };
                    standards.Add(standardsdat);
                }

                PENOP.status = "success";
                PENOP.value = standards;
                sJSONResponse = JsonConvert.SerializeObject(PENOP);


            }
            catch (Exception ec)
            {
                PENOP.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(PENOP);
            }


            return sJSONResponse;
        }

        public Object GetOverallScore([FromBody]PEInput PEI)
        {
            double x = 0;
            double xvalue = 0;
            double y = 0;
            double initx = 0;
            double inity = 0;

            ScoreDetailsOutput SDO = new ScoreDetailsOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_standards = new DataTable();
            DataTable dt_Evaluation = new DataTable();
            List<PEStandardNestRoot> score = new List<PEStandardNestRoot>();

            try
            {
                dt_standards = getdata(string.Format("Select SNO,EvaluationName,Score from PerformanceEvaluationStandards"));
                for (int i = 0; i < dt_standards.Rows.Count; i++)
                {
                    dt_Evaluation = getdata(string.Format("select sum(PEN.Score) as sumoftotalscore ,sum(PES.Score) as Sumofachivedscore  from PerformanceEvaluationNames PEN,PerformanceEvaluationScore PES where PEN.SNO=PES.PESNID and PEN.PESID='{0}' and PES.EmployeeCode='{1}' ", dt_standards.Rows[i]["SNO"].ToString(), PEI.EmployeeCode));
                    if (dt_Evaluation.Rows[0][0].ToString() != "")
                    {
                        initx = Convert.ToInt32(dt_Evaluation.Rows[0]["Sumofachivedscore"].ToString());
                        inity = Convert.ToInt32(dt_Evaluation.Rows[0]["sumoftotalscore"].ToString());
                        xvalue = (initx / inity);
                        x = xvalue * 100;
                        y = (x * Convert.ToInt32(dt_standards.Rows[i]["Score"].ToString())) / 100;
                    }
                    else
                    {
                        y = 0;
                    }
                    PEStandardNestRoot scoredat = new PEStandardNestRoot { Id = Convert.ToInt32(dt_standards.Rows[i]["SNO"].ToString()), EvaluationName = dt_standards.Rows[i]["EvaluationName"].ToString(), Score = Convert.ToInt32(dt_standards.Rows[i]["Score"].ToString()), AchieveScore = y, ScoreDetails = GetEmployeeScoreDetails(Convert.ToInt32(dt_standards.Rows[i]["SNO"].ToString()), PEI.EmployeeCode, Convert.ToInt32(dt_standards.Rows[i]["Score"].ToString())) };
                    score.Add(scoredat);
                }

                SDO.status = "success";
                SDO.value = score;
                sJSONResponse = JsonConvert.SerializeObject(SDO);
            }
            catch (Exception ec)
            {
                SDO.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(SDO);
            }


            return sJSONResponse;
        }

        public List<PEStandardNest> GetEmployeeScoreDetails(int Id, string EmployeeCode, int Score)
        {

            List<PEStandardNest> ScoreDetails = new List<PEStandardNest>();

            DataTable dt_Scoreetails = getdata(string.Format("select PESubName as Ename,PEN.Score as Totalscore,PES.Score as achivedScore  from PerformanceEvaluationNames PEN,PerformanceEvaluationScore PES where PEN.SNO=PES.PESNID and PEN.PESID='{0}' and PES.EmployeeCode='{1}' ", Id, EmployeeCode));


            for (int i = 0; i <= dt_Scoreetails.Rows.Count - 1; i++)
            {
               
                DataTable dt_Evaluation = new DataTable();
                dt_Evaluation = getdata(string.Format("select PESubName as Ename,PEN.Score as Totalscore,PES.Score as achivedScore  from PerformanceEvaluationNames PEN,PerformanceEvaluationScore PES where PEN.SNO=PES.PESNID and PEN.PESID='{0}' and PES.EmployeeCode='{1}' ", Id, EmployeeCode));

          
                ScoreDetails.Add(new PEStandardNest
                {
                    Ename = dt_Evaluation.Rows[i]["Ename"].ToString()
                                                ,
                    AchivedScore =Convert.ToDouble(dt_Evaluation.Rows[i]["achivedScore"].ToString())
                                                ,
                    Totalscore = Convert.ToDouble(dt_Evaluation.Rows[i]["Totalscore"].ToString())

                });

            }
            return ScoreDetails;

        }

        public Object GetEmployeesbyBranchMapping([FromBody]EmployeeMapping EM)
        {
            EmployeeMappingOutPut EMO = new EmployeeMappingOutPut();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_standards = new DataTable();

            List<EmployeeBranchResult> Empcode = new List<EmployeeBranchResult>();

            try
            {
                dt_standards = getdata(string.Format("select distinct  HRE.EmployeeCode,HRE.EmployeeName from HREmployee HRE,EmployeeBranches EB,UserMapping UM,Roles R  where EB.EmployeeCode=HRE.EmployeeCode and  EB.BranchCode='"+EM.BranchCode+ "' and HRE.EmployeeCode<>1 and UM.UMID=HRE.UMID and UM.RoleCode=R.RoleCode and  HRE.IsActive=1 and EB.IsActive=1 and R.RoleCode between 4 and 5"));

                for (int i = 0; i < dt_standards.Rows.Count; i++)
                {
                    EmployeeBranchResult Emdata = new EmployeeBranchResult { EmployeeCode = dt_standards.Rows[i]["EmployeeCode"].ToString(), EmployeeName = (dt_standards.Rows[i]["EmployeeName"].ToString()) };
                    Empcode.Add(Emdata);
                }

                EMO.status = "success";
                EMO.value = Empcode;
                sJSONResponse = JsonConvert.SerializeObject(EMO);


            }
            catch (Exception ec)
            {
                EMO.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(EMO);
            }


            return sJSONResponse;
        }



    }
}