using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.Serialization;
using System.Collections;
using System.Web.Script.Serialization;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
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
using NarenFitnessUsers.Models.Facility;
using NarenFitnessUsers.Models.Facility.UsedFacilities;
using NarenFitnessUsers.Models.Facility.AllFacilities;
using NarenFitnessUsers.Class;
using NarenFitnessUsers.Models.Facility.AllFacilities.Freezing;
using NarenFitnessUsers.Models.Facility.AllFacilities.Grace;
using NarenFitnessUsers.Models.Facility.AllFacilities.Upgradation;
using NarenFitnessUsers.Models.Facility.AllFacilities.Change;
using NarenFitnessUsers.Models.Facility.AllFacilities.LocationTransfer;
using NarenFitnessUsers.Models.Facility.AllFacilities.PersonTransfer;
using NarenFitnessUsers.Models.Facility.AllFacilities.Extend;
using NarenFitnessUsers.Models.Facility.AllFacilities.Hold;
using NarenFitnessUsers.Models.Facility.AllFacilities.Convert;
using NarenFitnessUsers.Models.Facility.FreezingCalculation;
using NarenFitnessUsers.Models.Applications;
using NarenFitnessUsers.Models.PackagesList;
using NarenFitnessUsers.Models.OffLinePackageList;



namespace NarenFitnessUsers.Controllers
{
    public class FacilityController : Controller
    {
        //work
        SqlConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["NarenFitnessUsers"].ToString());
        // GET: ApplicationTypes

        public Object FacilityPausedUpdate([FromBody]FacilityInput fi)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";
            FacilityOutput ffopt = new FacilityOutput();

            try
            {
                cnn.Open();
                olPackage_Query = "update  CCRMMembership set Paused=1 where InvoiceID='" + fi.InvoiceID + "' ";
                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                ffopt.status = "Success";
                ffopt.value = a;
            }
            catch (Exception ex)
            {
                ffopt.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object FacilityPost([FromBody]FacilityInput fi)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";
            FacilityOutput ffopt = new FacilityOutput();

            try
            {
                cnn.Open();
                olPackage_Query = "insert  into CCRMMembershipFacility(MemberShipCode,MFDID,SMFMID,FacilityExpireDate,FacilityStartDate,LeftOutDays,FreezingDays,BranchCode,InvoiceID,CreatedBy,CreatedOn) values('" + fi.MemberShipCode + "','" + fi.MFDID + "','" + fi.SMFMID + "','" + fi.FacilityExpireDate + "','" + fi.FacilityStartDate + "','" + fi.LeftOutDays + "','" + fi.FreezingDays + "','" + fi.BranchCode + "','" + fi.InvoiceID + "','" + fi.CreatedBy + "','" + ServerDateTime + "') SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                ffopt.status = "Success";
                ffopt.value = a;
            }
            catch (Exception ex)
            {
                ffopt.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object FacilityUpdate([FromBody]FacilityInput fi)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            FacilityOutput ffopt = new FacilityOutput();

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
                    command.CommandText = "";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {
                    ffopt.status = "fail";
                    ffopt.value = 1;
                }

            }
            catch (Exception ec)
            {
                ffopt.status = "Success";
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object FacilityDelete([FromBody]FacilityInput fi)
        {
            string sJSONResponse = "";
            FacilityOutput ffopt = new FacilityOutput();
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
                    command.CommandText = " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {
                    ffopt.status = "fail";
                    ffopt.value = 1;
                }

            }
            catch (Exception ec)
            {

            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

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
        public Object GetFreezingFacility([FromBody]FacilityInput fi)
        {
            FacilityDetailsOutput fdop = new FacilityDetailsOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Facility = new DataTable();

            List<FacilityDetails> fddetails = new List<FacilityDetails>();

            try
            {
                dt_Facility = getdata(string.Format("select FreezingID,BranchCode,SMFMID,FreezingName,NoOfDays,FreezingStartDate,FreezingExpireDate from FreezingFacility"));

                for (int i = 0; i < dt_Facility.Rows.Count; i++)
                {
                    FacilityDetails fd = new FacilityDetails { total = 1, noofDays = 2, leftoutCount = 3, usedCount = 4 };
                    fddetails.Add(fd);
                }

                fdop.status = "success";
                fdop.value = fddetails;
                sJSONResponse = JsonConvert.SerializeObject(fdop);


            }
            catch (Exception ec)
            {
                fdop.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(fdop);
            }


            return sJSONResponse;
        }

        public Object GetFacility()
        {
            FacilityRootOutput fropt = new FacilityRootOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Facility = new DataTable();

            List<FacilityRoot> froot = new List<FacilityRoot>();

            try
            {
                dt_Facility = getdata(string.Format("select SMFMID,SubSchemName from SubMembersFacilityMaster"));

                for (int i = 0; i < dt_Facility.Rows.Count; i++)
                {
                    FacilityRoot ft = new FacilityRoot { SMFMID = Convert.ToInt32(dt_Facility.Rows[i]["SMFMID"].ToString()), SubSchemName = dt_Facility.Rows[i]["SubSchemName"].ToString() };
                    froot.Add(ft);
                }

                fropt.status = "success";
                fropt.value = froot;
                sJSONResponse = JsonConvert.SerializeObject(fropt);


            }
            catch (Exception ec)
            {
                fropt.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(fropt);
            }


            return sJSONResponse;
        }
        public Object FacilityHeaderPost([FromBody]FacilityHeader fh)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";
            FacilityOutput ffopt = new FacilityOutput();

            try
            {
                cnn.Open();
                olPackage_Query = "insert into FacilityHeader(SMFDID,HeaderName,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fh.SMFDID + "','" + fh.HeaderName + "','" + fh.CreatedBy + "','" + ServerDateTime + "',0,1) SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                ffopt.status = "Success";
                ffopt.value = a;
            }
            catch (Exception ex)
            {
                ffopt.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object FacilityHeaderUpdate([FromBody]FacilityInput fi)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            FacilityOutput ffopt = new FacilityOutput();

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
                    command.CommandText = "update FacilityHeader set HeaderName='" + fi.HeaderName + "' where SMFDID=" + fi.SMFMID + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();

                    ffopt.status = "success";
                    ffopt.value = 1;
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
                ffopt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object FacilityHeaderDelete([FromBody]FacilityInput fi)
        {
            string sJSONResponse = "";
            FacilityOutput ffopt = new FacilityOutput();
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
                    command.CommandText = "delete from FacilityHeader where SNO=" + fi.SMFMID + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    ffopt.status = "success";
                    ffopt.value = 1;
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
                ffopt.status = "fail";

            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }

        public Object GetFacilityHeader([FromBody]FacilityHeader fh)
        {
            FacilityDetailsOutput fdop = new FacilityDetailsOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Facility = new DataTable();

            List<FacilityHeader> fheader = new List<FacilityHeader>();
            FacilityHeaderOutput fho = new FacilityHeaderOutput();
            try
            {
                dt_Facility = getdata(string.Format("select FH.SNO,SMFM.MFMID,SMFM.SubSchemName,FH.HeaderName,FH.CreatedBy from SubMembersFacilityMaster SMFM,FacilityHeader FH where SMFM.SMFMID=FH.SMFDID"));

                for (int i = 0; i <= dt_Facility.Rows.Count - 1; i++)
                {
                    fheader.Add(new FacilityHeader
                    {
                        HeaderId = Convert.ToInt32(dt_Facility.Rows[i]["SNO"])
                        ,
                        SMFDID = Convert.ToInt32(dt_Facility.Rows[i]["MFMID"])
                        ,
                        FacilityName = Convert.ToString(dt_Facility.Rows[i]["SubSchemName"])
                     ,
                        HeaderName = Convert.ToString(dt_Facility.Rows[i]["HeaderName"])
                        ,
                        CreatedBy = Convert.ToString(dt_Facility.Rows[i]["CreatedBy"])

                    });
                }



                fho.status = "success";
                fho.value = fheader;
                sJSONResponse = JsonConvert.SerializeObject(fho);


            }
            catch (Exception ec)
            {
                fdop.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(fdop);
            }


            return sJSONResponse;
        }

        public Object FacilityDescriptionPost([FromBody]FacilityDescriptionInput fd)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string olPackage_Query = "";
            FacilityOutput ffopt = new FacilityOutput();

            try
            {
                cnn.Open();
                olPackage_Query = "insert into FacilityDescription(HeaderID,SMFDID,Description,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fd.HeaderId + "','" + fd.SMFDID + "','" + fd.Description + "','" + fd.CreatedBy + "','" + ServerDateTime + "',0,1) SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                ffopt.status = "Success";
                ffopt.value = a;
            }
            catch (Exception ex)
            {
                ffopt.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object FacilityDescriptionUpdate([FromBody]FacilityInput fi)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            FacilityOutput ffopt = new FacilityOutput();

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
                    command.CommandText = "update FacilityDescription set Description='" + fi.Description + "' where SNO=" + fi.SNO + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    ffopt.status = "success";
                    ffopt.value = 1;
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
                ffopt.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object FacilityDescriptionDelete([FromBody]FacilityInput fi)
        {
            string sJSONResponse = "";
            FacilityOutput ffopt = new FacilityOutput();
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
                    command.CommandText = "delete from FacilityDescription where SNO=" + fi.SNO + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    ffopt.status = "success";
                    ffopt.value = 1;
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
                ffopt.status = "fail";

            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }

        public Object GetFacilityDescription([FromBody]FacilityHeader fh)
        {
            FacilityDetailsOutput fdop = new FacilityDetailsOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Facility = new DataTable();

            List<FacilityDescription> fdescription = new List<FacilityDescription>();
            FacilityDescriptionOutput fdo = new FacilityDescriptionOutput();
            try
            {
                dt_Facility = getdata(string.Format("select FD.SNO as ID,FD.SMFDID,SMFM.SubSchemName as FacilityName,FD.Description,FD.CreatedBy from FacilityDescription FD,SubMembersFacilityMaster SMFM where SMFM.SMFMID=FD.SMFDID and  HeaderID='{0}' ", fh.HeaderId));

                for (int i = 0; i <= dt_Facility.Rows.Count - 1; i++)
                {
                    fdescription.Add(new FacilityDescription
                    {
                        ID = Convert.ToString(dt_Facility.Rows[i]["ID"])
                        ,
                        FacilityName = Convert.ToString(dt_Facility.Rows[i]["FacilityName"])
                        ,
                        Description = Convert.ToString(dt_Facility.Rows[i]["Description"])
                        ,
                        SMFDID = Convert.ToString(dt_Facility.Rows[i]["SMFDID"])
                        ,
                        CreatedBy = Convert.ToString(dt_Facility.Rows[i]["CreatedBy"])
                    });
                }



                fdo.status = "success";
                fdo.value = fdescription;
                sJSONResponse = JsonConvert.SerializeObject(fdo);


            }
            catch (Exception ec)
            {
                fdop.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(fdop);
            }


            return sJSONResponse;
        }

        public Object FacilityOptedPost([FromBody]FacilityOpted fo)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;

            FacilityOutput ffopt = new FacilityOutput();


            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            // Start a local transaction.
            transaction = cnn.BeginTransaction("SampleTransaction");
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                {
                    command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fo.MembershipCode + "',1,'FREEZING','" + fo.Freezing + "','" + fo.Invoice + "','" + fo.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fo.MembershipCode + "',2,'GRACE','" + fo.Freezing + "','" + fo.Invoice + "','" + fo.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fo.MembershipCode + "',3,'UPGRADATION','" + fo.Upgrade + "','" + fo.Invoice + "','" + fo.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fo.MembershipCode + "',4,'CHANGE','" + fo.Change + "','" + fo.Invoice + "','" + fo.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fo.MembershipCode + "',5,'TRANSFER TRANSFER','" + fo.Transfer + "','" + fo.Invoice + "','" + fo.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fo.MembershipCode + "',6,'PERSON TRANSFER','" + fo.Transfer + "','" + fo.Invoice + "','" + fo.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fo.MembershipCode + "',7,'EXTEND','" + fo.Transfer + "','" + fo.Invoice + "','" + fo.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fo.MembershipCode + "',8,'HOLD','" + fo.Transfer + "','" + fo.Invoice + "','" + fo.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fo.MembershipCode + "',9,'Paused','" + fo.Paused + "','" + fo.Invoice + "','" + fo.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fo.MembershipCode + "',10,'Convert','" + fo.Convert + "','" + fo.Invoice + "','" + fo.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    cnn.Close();



                }
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (Exception ex2)
                {

                }
            }








            //try
            //{
            //    cnn.Open();
            //    olPackage_Query = "insert into FacilityOpted(MembershipCode,Invoice,Freezing,Change,Upgrade,Transfer,Paused,[Convert],CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fo.MembershipCode + "','" + fo.Invoice + "','" + fo.Freezing + "','" + fo.Change + "','" + fo.Upgrade + "','" + fo.Transfer + "','" + fo.Paused + "','" + fo.Convert + "','" + fo.CreatedBy + "','" + ServerDateTime + "',0,1) SELECT @@IDENTITY;";
            //    SqlCommand tm_cmd = new SqlCommand(olPackage_Query, cnn);
            //    a = Convert.ToInt32(tm_cmd.ExecuteScalar());
            //    ffopt.status = "Success";
            //    ffopt.value = a;
            //}
            //catch (Exception ex)
            //{
            //    ffopt.status = "Fail";
            //}
            //finally
            //{
            //    cnn.Close();
            //}

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object FacilityOptedUpdate([FromBody]FacilityInput fi)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            FacilityOutput ffopt = new FacilityOutput();

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
                    command.CommandText = "";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {
                    ffopt.status = "fail";
                    ffopt.value = 1;
                }

            }
            catch (Exception ec)
            {
                ffopt.status = "Success";
            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object FacilityOptedDelete([FromBody]FacilityInput fi)
        {
            string sJSONResponse = "";
            FacilityOutput ffopt = new FacilityOutput();
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
                    command.CommandText = " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
                finally
                {
                    ffopt.status = "fail";
                    ffopt.value = 1;
                }

            }
            catch (Exception ec)
            {

            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }

        public Object GetFacilityOptes([FromBody]FacilityInput fi)
        {
            FacilityDetailsOutput fdop = new FacilityDetailsOutput();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Facility = new DataTable();

            List<FacilityOpted> foptes = new List<FacilityOpted>();
            FacilityOptesOutput foo = new FacilityOptesOutput();

            try
            {
                dt_Facility = getdata(string.Format("select top 1 Freezing,Change,Upgrade,Transfer,Paused,[Convert] from FacilityOpted where MembershipCode='33456' order by SNO"));

                for (int i = 0; i <= dt_Facility.Rows.Count - 1; i++)
                {
                    foptes.Add(new FacilityOpted
                    {
                        Freezing = Convert.ToInt32(dt_Facility.Rows[0]["Freezing"])
                        ,
                        Change = Convert.ToInt32(dt_Facility.Rows[0]["Change"])
                        ,
                        Upgrade = Convert.ToInt32(dt_Facility.Rows[0]["Upgrade"])
                        ,
                        Transfer = Convert.ToInt32(dt_Facility.Rows[0]["Transfer"])
                        ,
                        Paused = Convert.ToInt32(dt_Facility.Rows[0]["Paused"])
                        ,
                        Convert = Convert.ToInt32(dt_Facility.Rows[0]["Convert"])


                    });
                }



                foo.status = "success";
                foo.value = foptes;
                sJSONResponse = JsonConvert.SerializeObject(foo);


            }
            catch (Exception ec)
            {
                fdop.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(fdop);
            }


            return sJSONResponse;
        }
        public Object GetFacilityDetails([FromBody]FacilityInput fi)
        {
            DataTable dt_FacilityPrice = new DataTable();
            DataTable dt_FacilityWisePrice = new DataTable();
            double Percentage = 0.0f;
            double noofdays = 0.0f;
            double PerDay = 0.0f;
            double UnUsedDays = 0.0f;
            double FacilityPrice = 0.0f;
            double FacilityPriceA = 0.0f;
            double FacilityPriceB = 0.0f;

            FacilityAll fa = new FacilityAll();
            DataSet ds_SlotBooking = new DataSet();
            string sJSONResponse = "";
            DataTable dt_facility = new DataTable();

            try
            {

                List<FacilityObject> facilityobject = new List<FacilityObject>();

                if (fi.DurationId == "DM1")
                {
                    dt_facility = getdata(string.Format("select SMFMID,SubSchemName from  SubMembersFacilityMaster where SMFMID in (3,4,9)"));
                }
                else
                {
                    dt_facility = getdata(string.Format("select SMFMID,SubSchemName from  SubMembersFacilityMaster where SMFMID not in(2,7,8,10)"));
                }
                for (int i = 0; i <= dt_facility.Rows.Count - 1; i++)
                {
                    FacilityPrice = 0.0f;
                    try
                    {
                        dt_FacilityPrice = getdata(string.Format("select FacilityId,DurationID,Duration,Percentage from FacilityPrice where DurationID='{0}' and FacilityId='{1}' ", fi.DurationId, Convert.ToInt32(dt_facility.Rows[i]["SMFMID"].ToString())));

                        //noofdays        : noofdays = 30 * duration
                        //slotprice (ip)  : PerDay   =  SlotPrice/NoOfDays
                        //UnUsed Days     : Get valued from freezing (MembersFacilityFreezing)  for freezing  (Frequency * NoOfDays) others put 1 in output
                        //FacilityPrice   : (UnUsedDays * PerDay) * (Percentage/100)

                        noofdays = 30 * Convert.ToInt32(dt_FacilityPrice.Rows[0]["FacilityId"].ToString());
                        PerDay = Convert.ToDouble(fi.SlotPrice) / noofdays;
                        UnUsedDays = Convert.ToInt32(dt_FacilityPrice.Rows[0]["FacilityId"].ToString()) * Convert.ToInt32(dt_FacilityPrice.Rows[0]["Duration"].ToString());
                        Percentage = Convert.ToInt32(dt_FacilityPrice.Rows[0]["Percentage"].ToString());

                        FacilityPriceA = (UnUsedDays * PerDay);
                        FacilityPriceB = Percentage / 100;
                        FacilityPrice = FacilityPriceA * FacilityPriceB;


                    }
                    catch (Exception ec)
                    {

                    }
                    //GetFacilityPrice(int FacilityId, string Duration, double SlotPrice)
                    //facilityPrice = GetFacilityPrice(Convert.ToInt32(dt_facility.Rows[i]["SMFMID"].ToString()), fi.DurationId, fi.SlotPrice)

                    FacilityObject dashboardlist = new FacilityObject { SMFMID = Convert.ToInt32(dt_facility.Rows[i]["SMFMID"].ToString()), FacilityName = dt_facility.Rows[i]["SubSchemName"].ToString(), FacilityPrice = Math.Round(FacilityPrice), header = GetHeaderFacility(Convert.ToInt32(dt_facility.Rows[i]["SMFMID"].ToString())), description = GetDescriptionFacility(Convert.ToInt32(dt_facility.Rows[i]["SMFMID"].ToString())) };
                    facilityobject.Add(dashboardlist);
                }

                fa.status = "success";
                fa.value = facilityobject;

                sJSONResponse = JsonConvert.SerializeObject(fa);

            }
            catch (Exception ec)
            {

            }


            return sJSONResponse;
        }


        public Dictionary<string, object> GetFacilityPrice2(int FacilityId, string Duration, double SlotPrice)
        {
            Dictionary<string, object> facilityPrice = new Dictionary<string, object>();

            DataTable dt_FacilityPrice = new DataTable();
            DataTable dt_FacilityWisePrice = new DataTable();
            double Percentage = 0.0f;
            double noofdays = 0.0f;
            double PerDay = 0.0f;
            double UnUsedDays = 0.0f;
            double FacilityPrice = 0.0f;
            double FacilityPriceA = 0.0f;
            double FacilityPriceB = 0.0f;
            try
            {
                dt_FacilityPrice = getdata(string.Format("select FacilityId,DurationID,Duration,Percentage from FacilityPrice where DurationID='{0}' and FacilityId='{1}' ", Duration, FacilityId));


                //noofdays        : noofdays = 30 * duration
                //slotprice (ip)  : PerDay   =  SlotPrice/NoOfDays
                //UnUsed Days     : Get valued from freezing (MembersFacilityFreezing)  for freezing  (Frequency * NoOfDays) others put 1 in output
                //FacilityPrice   : (UnUsedDays * PerDay) * (Percentage/100)

                noofdays = 30 * Convert.ToInt32(dt_FacilityPrice.Rows[0]["FacilityId"].ToString());
                PerDay = SlotPrice / noofdays;
                UnUsedDays = Convert.ToInt32(dt_FacilityPrice.Rows[0]["FacilityId"].ToString()) * Convert.ToInt32(dt_FacilityPrice.Rows[0]["Duration"].ToString());
                Percentage = Convert.ToInt32(dt_FacilityPrice.Rows[0]["Percentage"].ToString());

                FacilityPriceA = (UnUsedDays * PerDay);
                FacilityPriceB = Percentage / 100;
                FacilityPrice = FacilityPriceA * FacilityPriceB;

                facilityPrice.Add("1", Convert.ToString(dt_FacilityPrice.Rows[0]["FacilityId"].ToString()));
                facilityPrice.Add("2", Convert.ToString(dt_FacilityPrice.Rows[0]["DurationId"].ToString()));
                facilityPrice.Add("3", Math.Round(FacilityPrice));
            }
            catch (Exception ex)
            {

            }
            return facilityPrice;
        }
        public List<FacilityHeaderALL> GetHeaderFacility(int smfdid)
        {

            DataTable dt_Facility = new DataTable();
            List<FacilityHeaderALL> foptes = new List<FacilityHeaderALL>();


            dt_Facility = getdata(string.Format("select SNO as Id,HeaderName from FacilityHeader where SMFDID='{0}' ", smfdid));

            for (int i = 0; i <= dt_Facility.Rows.Count - 1; i++)
            {
                foptes.Add(new FacilityHeaderALL
                {
                    Id = Convert.ToString(dt_Facility.Rows[i]["Id"])
                    ,
                    HeaderName = Convert.ToString(dt_Facility.Rows[i]["HeaderName"])
                    // Price
                });
            }

            return foptes;
        }
        public List<FacilityDescriptionALL> GetDescriptionFacility(int smfdid)
        {

            DataTable dt_Facility = new DataTable();
            List<FacilityDescriptionALL> foptes = new List<FacilityDescriptionALL>();


            dt_Facility = getdata(string.Format("select SNO as Id,HeaderID,Description from FacilityDescription where SMFDID='{0}' ", smfdid));

            for (int i = 0; i <= dt_Facility.Rows.Count - 1; i++)
            {
                foptes.Add(new FacilityDescriptionALL
                {
                    Id = Convert.ToString(dt_Facility.Rows[i]["Id"])
                    ,

                    Description = Convert.ToString(dt_Facility.Rows[i]["Description"])

                });
            }

            return foptes;
        }

        // Hold Start

        public DataTable GetFAInvoiceLatestRecord(string MemberShipCode)
        {
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet1 = new DataSet();

            DataTable dt_MemberShipDetails = new DataTable();
            string result = string.Empty;

            string query1 = "select * from FAInvoice where ID=(select Top 1 FAI.ID from FAInvoice FAI,CCRMMembership CCRM where  FAI.MembershipCode='" + MemberShipCode + "' and CCRM.StartDate < GETDATE()  and FAI.InvoiceID=CCRM.InvoiceID order by FAI.ID desc)";

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }
            return ds_custdet1.Tables[0];
        }

        public DataTable GetWalletCalculation(string MemberShipCode)
        {
            double FinalAmount = 0.0f;
            double FinalAmount2 = 0.0f;
            double IGST = 0.0f;
            double FinalAmount1 = 0.0f;
            double TransferfeePercentage = 0.0f;
            double Transferfee = 0.0f;
            double AmountPerDay = 0.0f;
            double LeftOutDays = 0.0d;
            double CompleteDays = 0.0d;
            double TotalDays = 0.0d;
            double RemainingAmount = 0.0d;
            int IsTransferUsed = 0;

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet1 = new DataSet();

            DataTable dt_Transfer = new DataTable();
            try
            {
                string result = string.Empty;
                try
                {
                    ArrayList dates = MembersLatestDates(MemberShipCode);
                    ArrayList Amount = MembersLatestAmount(MemberShipCode, DateTime.Now);

                    DateTime MembersStartDate = Convert.ToDateTime(dates[0].ToString());
                    DateTime MembersEndDate = Convert.ToDateTime(dates[1].ToString());
                    double Duration = Convert.ToInt32(dates[2].ToString());
                    DateTime CurrentDate = DateTime.Now;

                    if (MembersEndDate > CurrentDate)
                    {
                        CompleteDays = (CurrentDate - MembersStartDate).TotalDays;
                        LeftOutDays = (MembersEndDate - CurrentDate).TotalDays;
                        TotalDays = Duration * 30;
                    }

                    FinalAmount = Convert.ToDouble(Amount[0].ToString());
                    IGST = Convert.ToDouble(Amount[1].ToString());

                    if (LeftOutDays < 1)
                        LeftOutDays = 0;
                    if (CompleteDays < 1)
                        CompleteDays = 0;

                    if (FinalAmount > 0 && IGST > 0)
                    {
                        //FinalAmount1 = Convert.ToDouble(FinalAmount) - Convert.ToDouble(IGST);
                        FinalAmount1 = Convert.ToDouble(FinalAmount);
                        TransferfeePercentage = TransferPresentPercentage(MemberShipCode, CompleteDays);
                        Transferfee = Convert.ToDouble(FinalAmount) * (TransferfeePercentage / 100);
                        // FinalAmount2 = FinalAmount1 - Transferfee;
                        FinalAmount2 = FinalAmount1;
                        if (TotalDays > 0)
                        {
                            AmountPerDay = FinalAmount2 / TotalDays;
                            RemainingAmount = (AmountPerDay * LeftOutDays);
                        }
                    }

                    DataRow row;
                    DataColumn col1 = new DataColumn("RemainingAmount", typeof(float));
                    DataColumn col2 = new DataColumn("LeftOutDays", typeof(int));
                    DataColumn col3 = new DataColumn("FinalAmount", typeof(float));
                    DataColumn col4 = new DataColumn("TotalDays", typeof(int));
                    DataColumn col5 = new DataColumn("CompletedDays", typeof(int));
                    dt_Transfer.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5 });
                    if (RemainingAmount > 0)
                    {
                        row = dt_Transfer.NewRow();

                        row["RemainingAmount"] = RemainingAmount;
                        row["LeftOutDays"] = LeftOutDays;
                        row["FinalAmount"] = FinalAmount;
                        row["TotalDays"] = TotalDays;

                        if (CompleteDays < 0)
                            row["CompletedDays"] = 0.0f;
                        else
                            row["CompletedDays"] = CompleteDays;

                        dt_Transfer.Rows.Add(row);
                    }
                    else
                    {
                        row = dt_Transfer.NewRow();

                        row["RemainingAmount"] = "0";
                        row["LeftOutDays"] = "0";
                        row["FinalAmount"] = "0";
                        row["TotalDays"] = "0";
                        row["CompletedDays"] = "0";

                        dt_Transfer.Rows.Add(row);
                    }


                }
                catch (Exception ec)
                {
                }
            }
            catch (Exception ec)
            {


            }
            return dt_Transfer;
        }

        public ArrayList GetMembersBasicPlanDetails(string MemberShipCode, string StartDate)
        {
            ArrayList arl = new ArrayList();
            try
            {
                cnn.Close();
                //string query = "select CCRM.ID,CCRM.StartDate,CCRM.MembershipExpireDate,CCRM.PlanCostCode,CCRM.SlotCode,CCRM.DurationID,CCRM.EnquireTypeNo  from CCRMMembership CCRM,CMSPLANCOST CMSPC  where CCRM.MembershipCode='" + MemberShipCode + "' and CMSPC.PlanCostCode=CCRM.PlanCostCode and  CCRM.ID=(select max(ID) from CCRMMembership where MembershipCode='" + MemberShipCode + "' and StartDate <= GETDATE())";
                string query = "select CCRM.ID,CCRM.StartDate,CCRM.MembershipExpireDate,CCRM.PlanCostCode,CCRM.SlotCode,CCRM.DurationID,CCRM.EnquireTypeNo  from CCRMMembership CCRM,CMSPLANCOST CMSPC  where CCRM.MembershipCode='" + MemberShipCode + "' and CMSPC.PlanCostCode=CCRM.PlanCostCode and  CCRM.ID=(select max(ID) from CCRMMembership where MembershipCode='" + MemberShipCode + "' and StartDate between '" + StartDate + "' and '" + StartDate + " 23:59:59')";

                //string query = "select CCRM.PlanCostCode,CCRM.SlotCode,CCRM.DurationID,CCRM.EnquireTypeNo  from CCRMMembership CCRM,CMSPLANCOST CMSPC  where CCRM.MembershipCode='" + MemberShipCode + "' and CMSPC.PlanCostCode=CCRM.PlanCostCode and  CCRM.ID=(select MAX(ID) from CCRMMembership where MembershipCode='" + MemberShipCode + "')";
                //string query = "select CCRM.PlanCostCode,CCRM.SlotCode,CCRM.DurationID,CCRM.EnquireTypeNo  from CCRMMembership CCRM,CMSPLANCOST CMSPC  where CCRM.MembershipCode='"+ MemberShipCode + "' and CMSPC.PlanCostCode=CCRM.PlanCostCode and  CCRM.ID=(select MAX(ID) from CCRMMembership where MembershipCode='"+ MemberShipCode + "' and StartDate <= GETDATE() and MembershipExpireDate >=GETDATE())";
                using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
                {
                    cnn.Open();
                    SqlDataReader GA_DR = cmd_SubEXT.ExecuteReader();
                    if (GA_DR.Read())
                    {
                        arl.Add(GA_DR["PlanCostCode"].ToString());
                        arl.Add(GA_DR["SlotCode"].ToString());
                        arl.Add(GA_DR["DurationID"].ToString());
                        arl.Add(GA_DR["EnquireTypeNo"].ToString());
                    }

                    cnn.Close();
                }
            }
            catch (Exception ec)
            {


            }
            finally
            {
                cnn.Close();
            }
            return arl;

        }
        public DataTable GetMembersPlanDetails(string MemberShipCode, string BranchCode, ArrayList ALD)
        {
            DataSet ds_custdet1 = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                //string query = "select DurationId,'DurationName' as DurationName,PlanCode,PlanCost,PlanName,PackageCode,PackageName,SessionCode,SessionName,SlotCode,SlotName,SlotPrice,PlanCostCode from CMSPACKAGESCOST where BranchCode='" + BranchCode + "' and DurationId='" + ALD[2].ToString() + "' and SlotCode='ST76' and PlanCostCode='" + ALD[0].ToString() + "'";
                string query = "select CMSPAC.DurationID,CMSD.DurationName,CMSPAC.PlanCode,CMSPAC.PlanCost,CMSPAC.PlanName,CMSPAC.PackageCode,CMSPAC.PackageName,CMSPAC.SessionCode, CMSPAC.SessionName,CMSPAC.SlotCode,CMSPAC.SlotName,CMSPAC.SlotPrice,CMSPAC.PlanCostCode from CMSPACKAGESCOST CMSPAC,CMSDURATION CMSD where CMSPAC.DurationId=CMSD.DurationID and CMSPAC.PlanCostCode='" + ALD[0].ToString() + "' and CMSPAC.DurationId='" + ALD[2].ToString() + "' and CMSPAC.SlotCode='" + ALD[1].ToString() + "' and CMSPAC.BranchCode='" + BranchCode + "'";
                using (SqlDataAdapter da_custdet = new SqlDataAdapter(query, cnn))
                {
                    da_custdet.Fill(dt);
                }
            }
            catch (Exception ec)
            {

            }
            finally
            {

            }
            return dt;
        }

        public DataTable GetMembersPersonalDetails(string MemberShipCode, string StartDate)
        {
            DataSet ds_custdet1 = new DataSet();
            DataTable dt = new DataTable();
            try
            {

                //string query = "select CCRM.MembershipCode,U.Firstname,U.Lastname,U.MobileNo,U.DateOfBirth,U.Gender,U.Address,U.Email,CCRM.StartDate as MembershipStartDate,CCRM.MembershipExpireDate,CCRM.TrainerCode,HR.EmployeeName,U.Address2,CCRM.ID from CCRMMembership CCRM, Users U,HREmployee HR where CCRM.MembershipCode = U.UCode and CCRM.TrainerCode = HR.EmployeeCode and U.UCode = '" + MemberShipCode + "' and CCRM.ID = (select MAX(ID) from CCRMMembership where MembershipCode='" + MemberShipCode + "' and StartDate <= GETDATE())";
                string query = "select CCRM.MembershipCode,U.Firstname,U.Lastname,U.MobileNo,U.DateOfBirth,U.Gender,U.Address,U.Email,CCRM.StartDate as MembershipStartDate,CCRM.MembershipExpireDate,CCRM.TrainerCode,HR.EmployeeName,U.Address2,CCRM.ID from CCRMMembership CCRM, Users U,HREmployee HR where CCRM.MembershipCode = U.UCode and CCRM.TrainerCode = HR.EmployeeCode and U.UCode = '" + MemberShipCode + "' and CCRM.ID = (select MAX(ID) from CCRMMembership where MembershipCode='" + MemberShipCode + "'  and StartDate between '" + StartDate + "' and '" + StartDate + " 23:59:59') ";



                using (SqlDataAdapter da_custdet = new SqlDataAdapter(query, cnn))
                {
                    da_custdet.Fill(dt);
                }
            }
            catch (Exception ec)
            {

            }
            finally
            {

            }
            return dt;
        }

        public int GetFacilityLatest(string MembershipCode)
        {
            int SNO = 0;

            string query = "select top 1 SNO from CCRMMembershipFacility  where MemberShipCode='" + MembershipCode + "' order by SNO desc";

            using (SqlCommand cmd_ExpDates = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader Exp_DR = cmd_ExpDates.ExecuteReader();
                if (Exp_DR.Read())
                {
                    SNO = Convert.ToInt32(Exp_DR[0].ToString());
                }
                cnn.Close();
            }

            return SNO;
        }


        //Hold End


        // Freezing

        public ArrayList GetMembersExpireDate(string MembershipCode)
        {
            ArrayList arl = new ArrayList();
            DateTime dt = DateTime.Now;

            try
            {
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();

                }
                string query = "select top 1 MembershipExpireDate,BranchCode from CCRMMembership where MembershipCode='" + MembershipCode + "' order by ID desc";
                cnn.Close();
                using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
                {
                    cnn.Open();
                    SqlDataReader GA_DR = cmd_SubEXT.ExecuteReader();
                    if (GA_DR.Read())
                    {
                        arl.Add(Convert.ToDateTime(GA_DR[0].ToString()));
                        arl.Add(GA_DR[1].ToString());

                    }
                    cnn.Close();
                }
            }
            catch (Exception ec)
            {

            }
            return arl;
        }
        public string GetMembersInvoiceNumberForFacilityPosts(string MemberShipCode)
        {

            string r = "";
            string Invoicenumber = "";
            string BranchCode = "";
            if (cnn.State == ConnectionState.Open)
            {
                cnn.Close();

            }
            try
            {
                string query = "select top 1 FAI.InvoiceID,CCRMM.BranchCode from FAInvoice FAI, CCRMMembership CCRMM where FAI.MembershipCode = '" + MemberShipCode + "' and FAI.InvoiceID = CCRMM.InvoiceID and FAI.ID = (select Max(ID) as ID from FAInvoice where MembershipCode = '" + MemberShipCode + "')";
                //string query = "select top 1 FAI.InvoiceID,CCRMM.BranchCode from FAInvoice FAI,CCRMMembership CCRMM  where FAI.MembershipCode='" + MemberShipCode + "' and FAI.InvoiceID=CCRMM.InvoiceID and CCRMM.StartDate <= GETDATE() order by FAI.ID desc";
                //string query = "select top 1 FAI.InvoiceID from FAInvoice FAI,CCRMMembership CCRMM  where FAI.MembershipCode='" + MemberShipCode + "' and FAI.InvoiceID=CCRMM.InvoiceID  and StartDate between '" + StartDate + "' and '" + StartDate + " 23:59:59' order by FAI.ID desc";

                using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
                {
                    cnn.Open();
                    SqlDataReader GA_DR = cmd_SubEXT.ExecuteReader();
                    if (GA_DR.Read())
                    {
                        Invoicenumber = GA_DR[0].ToString();
                        BranchCode = GA_DR[1].ToString();
                    }
                    cnn.Close();
                }


            }
            catch (Exception ex)
            {
                return "";
            }
            finally
            {
                cnn.Close();
            }
            return Invoicenumber;
        }
        public int GetFreezingLatest(string MembershipCode)
        {
            int SNO = 0;

            string query = "select top 1 SNO from CCRMMembershipFacility  where MemberShipCode='" + MembershipCode + "' order by SNO desc";

            using (SqlCommand cmd_ExpDates = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader Exp_DR = cmd_ExpDates.ExecuteReader();
                if (Exp_DR.Read())
                {
                    SNO = Convert.ToInt32(Exp_DR[0].ToString());
                }
                cnn.Close();
            }

            return SNO;
        }
        public Object Freezing([FromBody]FreezingAllInput fi)
        {

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            string result = "";
            DataSet ds_custdet = new DataSet();

            ArrayList PED = GetMembersExpireDate(fi.MemberShipCode);
            DateTime MSED = Convert.ToDateTime(PED[0].ToString()).AddDays(fi.FreezingDays);
            FacilityOutputStatus ffopt = new FacilityOutputStatus();



            string Comments = "";

            try
            {
                Comments = fi.Comments;
            }
            catch
            {
                Comments = "";
            }
            int SNO = 0;
            try
            {
                {
                    if (fi.Mode == "0")
                    {
                        try
                        {


                            cnn.Close();
                            cnn.Open();
                            SqlCommand command = cnn.CreateCommand();
                            SqlTransaction transaction;

                            // Start a local transaction.
                            transaction = cnn.BeginTransaction("SampleTransaction");

                            // Must assign both transaction object and connection
                            // to Command object for a pending local transaction
                            command.Connection = cnn;
                            command.Transaction = transaction;
                            //InvoiceID
                            command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,FacilityStartDate,FacilityExpireDate,CreatedBy,CreatedOn,IsDeleted,IsActive,FreezingDays,PlanCostCode,BranchCode,InvoiceID,PreviousExpiryDate) values('" + fi.MemberShipCode + "','1','" + fi.FreezingStartDate + "','" + fi.FreezingEndDate + "','" + fi.CreatedBy + "','" + ServerDateTime + "',0,1," + fi.FreezingDays + ",'" + fi.PlanCostCode + "','" + PED[1].ToString() + "','" + fi.InvoiceId + "','" + PED[0].ToString() + "')";
                            command.ExecuteNonQuery();
                            command.CommandText = "update CCRMMembership set MembershipExpireDate='" + MSED + "',SMFMID=1 where ID=(select Top 1 ID from CCRMMembership where MembershipCode='" + fi.MemberShipCode + "' order by ID desc)";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into Remainders(MembershipCode,RemainderCode,RemainderName,Amount,Date,CreatedBy,CreatedOn) values('" + fi.MemberShipCode + "','1002','EXPIREDATE','0','" + MSED + "','" + fi.CreatedBy + "','" + ServerDateTime + "')";
                            command.ExecuteNonQuery();
                            if (fi.Comments != "")
                            {
                                command.CommandText = "insert into CommentsTracker(UCode,MasterID,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fi.MemberShipCode + "',21,'" + Comments + "','" + fi.CreatedBy + "','" + ServerDateTime + "',0,1)";
                                command.ExecuteNonQuery();
                            }
                            if (fi.FreezingDays > 5)
                            {
                                command.CommandText = "update CCRMMembership set Paused=1 where InvoiceID='" + fi.InvoiceId + "'";
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            cnn.Close();


                        }
                        catch (Exception ece)
                        {
                        }
                        finally
                        {
                            cnn.Close();
                            ffopt.status = "Success";
                        }

                    }
                    else if (fi.Mode == "1")
                    {
                        SNO = GetFreezingLatest(fi.MemberShipCode);
                        try
                        {

                            if (cnn.State == ConnectionState.Open)
                            {
                                cnn.Close();
                            }
                            else
                            {

                            }

                            ArrayList FacilityArray = new ArrayList();
                            FacilityArray = MembersFacilityLatestRecordByMFDID(fi.InvoiceId, fi.MFDID);

                            string FreezingStartDate = FacilityArray[0].ToString();
                            DateTime MembersExpireDate = Convert.ToDateTime(FacilityArray[1].ToString());
                            int FreezingAppliedDays = Convert.ToInt32(FacilityArray[2].ToString());
                            double FreezingUsedDays = 0.0d;
                            DateTime FreezingStartDays = Convert.ToDateTime(FreezingStartDate);

                            if (DateTime.Now > FreezingStartDays)
                            {
                                FreezingUsedDays = (DateTime.Now - FreezingStartDays).TotalDays;
                            }
                            double FreezDays = FreezingAppliedDays - FreezingUsedDays;
                            DateTime OriginalDate = MSED.AddDays(-FreezingAppliedDays);
                            MSED = OriginalDate.AddDays(FreezingUsedDays);


                            cnn.Close();
                            cnn.Open();
                            SqlCommand command = cnn.CreateCommand();
                            SqlTransaction transaction;
                            // Start a local transaction.
                            transaction = cnn.BeginTransaction("SampleTransaction");

                            // Must assign both transaction object and connection
                            // to Command object for a pending local transaction
                            command.Connection = cnn;
                            command.Transaction = transaction;

                            command.Transaction = transaction;
                            //InvoiceID
                            command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,CreatedBy,CreatedOn,IsDeleted,IsActive,FreezingDays,PlanCostCode,BranchCode,InvoiceID,PreviousExpiryDate) values('" + fi.MemberShipCode + "','" + fi.MFDID + "','" + fi.CreatedBy + "','" + ServerDateTime + "',0,1," + FreezDays + ",'" + fi.PlanCostCode + "','" + PED[0].ToString() + "','" + fi.InvoiceId + "','" + PED[0].ToString() + "')";
                            command.ExecuteNonQuery();
                            command.CommandText = "update CCRMMembership set MembershipExpireDate='" + MSED + "',SMFMID=1 where ID=(select Top 1 ID from CCRMMembership where MembershipCode='" + fi.MemberShipCode + "' order by ID desc) ";
                            command.ExecuteNonQuery();
                            command.CommandText = "update CCRMMembershipFacility set FacilityExpireDate='" + ServerDateTime + "',SMFMID=1 where SNO='" + SNO + "'";
                            command.ExecuteNonQuery();

                            if (fi.Comments != "")
                            {
                                command.CommandText = "insert into CommentsTracker(UCode,MasterID,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fi.MemberShipCode + "',21,'" + fi.Comments + "','" + fi.CreatedBy + "','" + ServerDateTime + "',0,1)";
                                command.ExecuteNonQuery();
                            }
                            transaction.Commit();
                            cnn.Close();

                            ffopt.status = "Success";
                        }
                        catch (Exception ece)
                        {

                            try
                            {

                            }
                            catch (Exception ex2)
                            {
                                result = "0";
                            }
                        }
                        finally
                        {
                            cnn.Close();

                        }
                    }
                    else if (fi.Mode == "2")
                    {
                        try
                        {

                            cnn.Close();
                            cnn.Open();
                            SqlCommand command = cnn.CreateCommand();
                            SqlTransaction transaction;

                            // Start a local transaction.
                            transaction = cnn.BeginTransaction("SampleTransaction");

                            // Must assign both transaction object and connection
                            // to Command object for a pending local transaction
                            command.Connection = cnn;
                            command.Transaction = transaction;
                            command.CommandText = "update CCRMMembershipFacility set FacilityStartDate='" + fi.FreezingStartDate + "',FacilityExpireDate='" + fi.FreezingEndDate + "',FreezingDays='" + fi.FreezingDays + "',PreviousExpiryDate='" + PED + "',ModifiedOn='" + ServerDateTime + "',ModifiedBy='" + fi.CreatedBy + "' where InvoiceID='" + fi.InvoiceId + "' and SNO=" + fi.Sno + " ";
                            command.ExecuteNonQuery();
                            command.CommandText = "update CCRMMembership set MembershipExpireDate='" + MSED + "'  where InvoiceID='" + fi.InvoiceId + "' ";
                            command.ExecuteNonQuery();

                            transaction.Commit();
                            cnn.Close();


                        }
                        catch (Exception ece)
                        {
                        }
                        finally
                        {
                            cnn.Close();
                            ffopt.status = "Success";
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                cnn.Close();
            }

            string sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;


        }
        public ArrayList MembersFacilityLatestRecordByMFDID(string InvoiceID, int MFDID)
        {
            ArrayList FreezingDates = new ArrayList();
            ArrayList FacilityArray = new ArrayList();
            try
            {
                if (MFDID == 1)
                {
                    if (cnn.State == ConnectionState.Open)
                    {
                        cnn.Close();
                    }
                    string query = "select top 1 FacilityStartDate,FacilityExpireDate,FreezingDays from CCRMMembershipFacility where InvoiceID = '" + InvoiceID + "' and MFDID=" + MFDID + " order by SNO desc";
                    using (SqlCommand cmd_ExpDates = new SqlCommand(query, cnn))
                    {
                        cnn.Open();
                        SqlDataReader Exp_DR = cmd_ExpDates.ExecuteReader();
                        if (Exp_DR.Read())
                        {
                            FreezingDates.Add(Exp_DR[0].ToString());
                            FreezingDates.Add(Exp_DR[1].ToString());
                            FreezingDates.Add(Exp_DR[2].ToString());
                        }
                        cnn.Close();
                        FacilityArray = FreezingDates;
                    }
                }
            }
            catch (Exception ec)
            {

            }
            finally
            {

                cnn.Close();
            }
            return FacilityArray;
        }


        // FacilityPrice new fn -- Request : DurationId,SlotPrice    Response : Price output (Multiple)

        public List<FacilityPriceDetails> GetFacilityPrice(int FacilityId, string Duration, double SlotPrice)
        {
            List<FacilityPriceDetails> fpd = new List<FacilityPriceDetails>();
            DataTable dt_FacilityPrice = new DataTable();
            DataTable dt_FacilityWisePrice = new DataTable();
            double Percentage = 0.0f;
            double noofdays = 0.0f;
            double PerDay = 0.0f;
            double UnUsedDays = 0.0f;
            double FacilityPrice = 0.0f;
            double FacilityPriceA = 0.0f;
            double FacilityPriceB = 0.0f;


            dt_FacilityPrice = getdata(string.Format("select FacilityId,DurationID,Duration,Percentage from FacilityPrice where DurationID='{0}' and FacilityId='{1}' ", Duration, FacilityId));



            //noofdays        : noofdays = 30 * duration
            //slotprice (ip)  : PerDay   =  SlotPrice/NoOfDays
            //UnUsed Days     : Get valued from freezing (MembersFacilityFreezing)  for freezing  (Frequency * NoOfDays) others put 1 in output
            //FacilityPrice   : (UnUsedDays * PerDay) * (Percentage/100)

            noofdays = 30 * Convert.ToInt32(dt_FacilityPrice.Rows[0]["FacilityId"].ToString());
            PerDay = SlotPrice / noofdays;
            UnUsedDays = Convert.ToInt32(dt_FacilityPrice.Rows[0]["FacilityId"].ToString()) * Convert.ToInt32(dt_FacilityPrice.Rows[0]["Duration"].ToString());
            Percentage = Convert.ToInt32(dt_FacilityPrice.Rows[0]["Percentage"].ToString());

            FacilityPriceA = (UnUsedDays * PerDay);
            FacilityPriceB = Percentage / 100;
            FacilityPrice = FacilityPriceA * FacilityPriceB;

            fpd.Add(new FacilityPriceDetails
            {
                FacilityId = Convert.ToInt32(dt_FacilityPrice.Rows[0]["FacilityId"])
           ,
                DurationId = Convert.ToString(dt_FacilityPrice.Rows[0]["DurationId"])
                ,
                FacilityPrice = Math.Round(FacilityPrice)

            });



            return fpd;
        }

        //

        //change

        public string UniqueGeneration2()
        {
            Random random = new Random();
            string r = "";
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    r += random.Next(0, 9).ToString();
                }
            }
            catch (Exception ex)
            {
                return "";
            }
            return r;

        }

        public int InvoiceCheck(string Invoice)
        {
            int InvoiceCheck = 0;
            string query = "select COUNT(*) as Count from FAInvoice where InvoiceID='" + Invoice + "' ";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_Login = cmd_SubEXT.ExecuteReader();
                if (DR_Login.Read())
                {
                    InvoiceCheck = Convert.ToInt32(DR_Login[0].ToString());
                }
                cnn.Close();
            }
            return InvoiceCheck;
        }
        public Object ChangePackage([FromBody]FacilityChangeInput fci)
        {
            FacilityOutputStatus ffopt = new FacilityOutputStatus();

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet = new DataSet();

            string Invoice2 = GetMembersInvoiceNumberForFacilityPosts(fci.MemberShipCode);
            string Invoice = "N" + UniqueGeneration2();
            if (InvoiceCheck(Invoice) != 0)
            {
                Invoice = "N" + UniqueGeneration2();
            }

            float WalletUsed = 0.0f;

            try
            {
                WalletUsed = float.Parse(fci.WalletAmountUsed);
            }
            catch
            {
                WalletUsed = 0.0f;
            }

            string Comments = "";
            try
            {
                Comments = fci.Comments;
            }
            catch
            {
                Comments = "";
            }
            float AmountDue = 0.0f;
            int receptGeneration = ReceiptGeneration();


            string SendText = string.Empty;
            string result = "";

            cnn.Close();

            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            // Start a local transaction.
            transaction = cnn.BeginTransaction("SampleTransaction");
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = cnn;
            command.Transaction = transaction;


            try
            {

                command.CommandText = "insert into CCRMMembership(MembershipCode,BranchCode,PlanCode,MembershipExpireDate,CreatedBy,CreatedOn,IsDeleted,IsActive,SlotCode,PackageCode,StartDate,TrainerCode,InvoiceID,Receipt,SerialNo,Year,Month,PlanCostCode,DurationId,SMFMID,EnquireTypeNo,EnquireTypeIncentives) values('" + fci.MemberShipCode + "','" + fci.BranchCode + "','" + fci.PlanCode + "','" + fci.ChangeEndDate + "','" + fci.CreatedBy + "','" + ServerDateTime + "',0,1,'" + fci.SlotCode + "','" + fci.PackageCode + "','" + fci.ChangeStartDate + "','" + fci.TrainerCode + "','" + fci.InvoiceID + "','" + receptGeneration + "',100,'0','0','" + fci.PlanCostCode + "','" + fci.DurationCode + "','" + fci.SMFMID + "','" + fci.EnquireTypeNo + "','')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FAInvoice(InvoiceID,GSTCode,FAPaymentModes,MembershipCode,PayableAmount,AmountDue,GymFees,TrainerFees,IGSTableAmount,IGST,CGST,SGST,DueDate,CreatedBy,CreatedOn,IsDeleted,IsActive,FinalAmount,FAPaymentModes2,PayableAmount2,PromoCode,DiscountAmount,TrainerCode,Receipt,EnquireTypeNo,SlotPrice,PlanCost,PaymentDate,RemainingAmount,Wallet,Recouncelled,Matchedid) values('" + Invoice + "','" + fci.GSTCodes + "','" + fci.FAPaymentModes + "','" + fci.MemberShipCode + "','" + fci.PayableAmount + "','" + fci.DueAmount + "','" + fci.GymFee + "','" + fci.TrainerFees + "','" + fci.IGSTableAmount + "','" + fci.IGST + "','" + fci.CGST + "','" + fci.SGST + "','" + fci.DueDate + "','" + fci.CreatedBy + "','" + ServerDateTime + "',0,1,'" + fci.FinalAmount + "','" + fci.FAPaymentModes2 + "','" + fci.PayableAmount2 + "','" + fci.PromoCode + "','" + fci.DiscountAmount + "','" + fci.TrainerCode + "','" + receptGeneration + "','" + fci.EnquireTypeNo + "','" + fci.SlotPrice + "','" + fci.PlanCost + "','" + fci.PaymentDate + "','" + fci.RemainingAmount + "','" + fci.WalletAmountUsed + "','','')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,PackageCode,PackageName,SessionCode,SessionName,PlanCode,PlanName,PlanAmount,SlotCode,SlotName,DurationCode,DurationName,LeftOutDays,LeftOutAmount,CreatedBy,CreatedOn,IsDeleted,IsActive,BranchCode,IsChange,InvoiceID,RemainingAmount,NewInvoiceID) values('" + fci.MemberShipCode + "','" + fci.MFDID + "','" + fci.PackageCode + "','" + fci.PackageName + "','" + fci.SessionCode + "','" + fci.SessionName + "','" + fci.PlanCode + "','" + fci.PlanName + "','" + fci.PlanAmount + "','" + fci.SlotCode + "','" + fci.SlotName + "','" + fci.DurationCode + "','" + fci.DurationName + "','" + fci.LeftOutDays + "','" + fci.LeftOutAmount + "','" + fci.CreatedBy + "','" + ServerDateTime + "',0,1,'" + fci.BranchCode + "','" + fci.IsChange + "','" + Invoice + "','" + fci.RemainingAmount + "','" + Invoice2 + "')";
                command.ExecuteNonQuery();
                command.CommandText = "update CCRMMembership set MembershipExpireDate='" + ServerDateTime + "' where ID=(select MAX(ID) as ID from CCRMMembership where MembershipCode='" + fci.MemberShipCode + "'  and InvoiceID='" + Invoice2 + "')";
                command.ExecuteNonQuery();
                if (fci.Comments != "")
                {
                    command.CommandText = "insert into CommentsTracker(UCode,MasterID,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + fci.MemberShipCode + "',0,'" + fci.Comments + "','" + fci.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                }
                if (WalletUsed != 0.0f)
                {
                    command.CommandText = "insert into WalletTransactions(MembershipCode,NewMembershipCode,InvoiceId,TransactionId,TransactionName,MobileNo,Credit,Debit,CreatedOn,CreatedBy,IsDeleted,IsActive) values('" + fci.MemberShipCode + "','','" + Invoice + "',1,'','','','','" + ServerDateTime + "','" + fci.CreatedBy + "',0,1) ";
                    command.ExecuteNonQuery();
                }

                if (AmountDue != 0.0f)
                {
                    command.CommandText = "insert into Remainders(MembershipCode,RemainderCode,RemainderName,FAInvoice,Amount,Date,CreatedBy,CreatedOn) values('" + fci.MemberShipCode + "','','','" + Invoice + "','" + fci.FinalAmount + "','" + fci.DueDate + "','" + fci.CreatedBy + "','" + ServerDateTime + "')";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into Assign(MemberShipCode,AssignCode,MobileNo,EnquireTypeNo,AssignedBy,AssignedTo,NextFollowDate,Description,CreatedBy,CreatedOn,IsDeleted,IsActive,Invoice) values('" + fci.MemberShipCode + "','AssignCode','989876567','" + fci.EnquireTypeNo + "','AssignedBy','AssignedBy.To','" + ServerDateTime + "','" + fci.Description + "','" + fci.CreatedBy + "','" + ServerDateTime + "',0,1,'" + fci.InvoiceID + "')";
                    command.ExecuteNonQuery();

                }

                transaction.Commit();
                cnn.Close();

                ffopt.status = "success";


            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (Exception ex2)
                {

                }
            }

            string sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;



        }

        // SelfTransfer
        public int TransferBit(string MembershipCode, int MFDID)
        {
            int val = 0;
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            DataSet ds_custdet = new DataSet();
            cnn.Close();
            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            transaction = cnn.BeginTransaction("SampleTransaction");
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                {
                    command.CommandText = "update  CCRMMembershipFacility set IsTransfer=1 where MemberShipCode='" + MembershipCode + "' and MFDID=" + MFDID + "  ";
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    cnn.Close();
                    val = 1;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    val = 0;
                }
            }
            finally
            {
                cnn.Close();
            }

            return val;
        }
        public Object RegisteredSelfTransfer([FromBody]FacilitySelfTransfer fst)
        {

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet = new DataSet();
            DataTable dtMsg = new DataTable("MSG");
            dtMsg.Columns.Add("UserID");

            string SendText = string.Empty;
            string result = "";

            string Invoice = "N" + UniqueGeneration2();

            if (InvoiceCheck(Invoice) != 0)
            {
                Invoice = "N" + UniqueGeneration2();
            }

            string Invoice2 = GetMembersInvoiceNumberForFacilityPosts(fst.MemberShipCode);


            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            // Start a local transaction.
            transaction = cnn.BeginTransaction("SampleTransaction");
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = cnn;
            command.Transaction = transaction;


            try
            {
                command.CommandText = "insert into CCRMMembership(MembershipCode,BranchCode,PlanCode,MembershipExpireDate,CreatedBy,CreatedOn,IsDeleted,IsActive,SlotCode,PackageCode,StartDate,TrainerCode,InvoiceID,Receipt,PlanCostCode,DurationId) values('" + fst.MemberShipCode + "','" + fst.BranchCode + "','" + fst.PlanCode + "','" + fst.MembershipExpireDate + "','" + fst.CreatedBy + "','" + ServerDateTime + "',0,1,,'" + fst.SlotCode + "','" + fst.PackageCode + "','" + fst.MembershipStartDate + "','" + fst.TrainerCode + "','" + Invoice + "','" + ReceiptGeneration() + "','" + fst.PlanCostCode + "','" + fst.DurationCode + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,FacilityStartDate,FacilityExpireDate,PackageCode,PackageName,SessionCode,SessionName,PlanCode,PlanName,PlanAmount,SlotCode,SlotName,LeftOutDays,LeftOutAmount,CreatedBy,CreatedOn,IsDeleted,IsActive,BranchCode,IsTransfer,NewMembersCode,InvoiceID,NewInvoiceID) values('" + fst.MemberShipCode + "','" + fst.MFDID + "','','','" + fst.PackageCode + "','" + fst.PackageName + "','" + fst.SessionCode + "','" + fst.SessionName + "','" + fst.PlanCode + "','" + fst.PlanName + "','" + fst.PlanAmount + "','" + fst.SlotCode + "','" + fst.SlotName + "','" + fst.LeftOutDays + "','" + fst.FromLeftOutAmount + "','" + fst.CreatedBy + "','" + ServerDateTime + "',0,1,'" + fst.BranchCode + "','" + fst.IsTransferUsed + "','','" + Invoice + "','" + Invoice2 + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FAInvoice(InvoiceID,GSTCode,FAPaymentModes,MembershipCode,PayableAmount,AmountDue,GymFees,TrainerFees,IGSTableAmount,IGST,CGST,SGST,DueDate,CreatedBy,CreatedOn,IsDeleted,IsActive,FinalAmount,FAPaymentModes2,PayableAmount2,PromoCode,DiscountAmount,TrainerCode,Receipt,EnquireTypeNo,SlotPrice,PlanCost,PaymentDate,RemainingAmount,Wallet,Recouncelled,Matchedid) values('" + Invoice + "','" + fst.GSTCodes + "','" + fst.FAPaymentModes + "','" + fst.MemberShipCode + "','" + fst.PayableAmount + "','" + fst.AmountDue + "','" + fst.GymFee + "','" + fst.TrainerFees + "','" + fst.IGSTableAmount + "','" + fst.IGST + "','" + fst.CGST + "','" + fst.SGST + "','" + fst.DueDate + "','" + fst.CreatedBy + "','" + ServerDateTime + "',0,1,'" + fst.FinalAmount + "','" + fst.FAPaymentModes2 + "','" + fst.PromoCode + "','" + fst.DiscountAmount + "','" + fst.TrainerCode + "','" + ReceiptGeneration() + "','" + fst.EnquireTypeNo + "','" + fst.SlotPrice + "','" + fst.PlanCost + "','" + fst.PaymentDate + "','" + fst.RemainingAmount + "','" + fst.WalletAmountUsed + "','','')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into Remainders(MembershipCode,RemainderCode,RemainderName,FAInvoice,Amount,Date,CreatedBy,CreatedOn) values('" + fst.MemberShipCode + "','','','" + Invoice + "','" + fst.FinalAmount + "','" + fst.DueDate + "','" + fst.CreatedBy + "','" + ServerDateTime + "')";
                command.ExecuteNonQuery();
                command.CommandText = "update CCRMMembership set MembershipExpireDate='" + ServerDateTime + "' where ID=(select MAX(ID) as ID from CCRMMembership where MembershipCode='" + fst.MemberShipCode + "'  and InvoiceID='" + Invoice2 + "')";
                command.ExecuteNonQuery();

                if (fst.Comments != "")
                {
                    command.CommandText = "insert into CommentsTracker(UCode,MasterID,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values(,'" + fst.MemberShipCode + "',0,'" + fst.Comments + "','" + fst.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                }
                //if (fst.wa WalletUsed != 0.0f)
                //{
                command.CommandText = "insert into WalletTransactions(MembershipCode,NewMembershipCode,InvoiceId,TransactionId,TransactionName,MobileNo,Credit,Debit,CreatedOn,CreatedBy,IsDeleted,IsActive) values('" + fst.MemberShipCode + "','','" + Invoice + "',1,'','','','','" + ServerDateTime + "','" + fst.CreatedBy + "',0,1) ";
                command.ExecuteNonQuery();
                //}

                transaction.Commit();
                cnn.Close();

            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (Exception ex2)
                {

                }
            }

            if (result == "1")
            {
                TransferBit(fst.MemberShipCode, Convert.ToInt32(fst.MFDID));
            }
            else
            {

            }
            return result;
        }

        // PersonToPersonTransfer
        public int ReceiptGeneration()
        {
            if (cnn.State == ConnectionState.Open)
            {
                cnn.Close();
            }

            int status = 0;
            string query1 = "select MAX(Receipt) from FAInvoice";
            try
            {
                using (SqlCommand cmd_RecGen = new SqlCommand(query1, cnn))
                {
                    cnn.Open();
                    SqlDataReader DR_RecGen = cmd_RecGen.ExecuteReader();
                    if (DR_RecGen.Read())
                    {
                        status = Convert.ToInt32(DR_RecGen[0].ToString());
                    }
                    cnn.Close();
                }

                if (status == 0)
                {
                    status = 1;
                }
                else
                {
                    status = status + 1;
                }

            }
            catch (Exception ec)
            {
                status = 1;
            }
            finally
            {

                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }
            return status;
        }
        public Object RegisteredPersonToPersonTransfer([FromBody] TransferInput fi)
        {

            string r = "";

            float WalletUsed = 0.0f;

            try
            {
                WalletUsed = float.Parse(fi.WalletAmountUsed);
            }
            catch
            {
                WalletUsed = 0.0f;
            }


            string Comments = "";
            try
            {
                Comments = fi.Comments;
            }
            catch
            {
                Comments = "";
            }

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet = new DataSet();
            DataTable dtMsg = new DataTable("MSG");
            dtMsg.Columns.Add("UserID");

            string SendText = string.Empty;
            string result = "";


            string Invoice = "N" + UniqueGeneration2();

            if (InvoiceCheck(Invoice) != 0)
            {
                Invoice = "N" + UniqueGeneration2();
            }



            string Invoice2 = GetMembersInvoiceNumberForFacilityPosts(fi.FromMemberShipCode);

            //Receipt
            string ReceiptNo = Convert.ToString(ReceiptGeneration());



            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            // Start a local transaction.
            transaction = cnn.BeginTransaction("SampleTransaction");
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                command.CommandText = "insert into CCRMMembership(MembershipCode,BranchCode,PlanCode,MembershipExpireDate,CreatedBy,CreatedOn,IsDeleted,IsActive,SlotCode,PackageCode,StartDate,TrainerCode,InvoiceID,Receipt,PlanCostCode,DurationId) values('" + fi.FromMemberShipCode + "','" + fi.FromBranchCode + "','" + fi.FromPlanCode + "','" + fi.FromMembershipExpireDate + "','" + fi.CreatedBy + "','" + ServerDateTime + "',0,1,,'" + fi.FromSlotCode + "','" + fi.FromPackageCode + "','" + fi.FromMembershipStartDate + "','" + fi.ToTrainerCode + "','" + Invoice + "','" + ReceiptGeneration() + "','" + fi.ToPlanCostCode + "','" + fi.ToDurationCode + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,FacilityStartDate,FacilityExpireDate,PackageCode,PackageName,SessionCode,SessionName,PlanCode,PlanName,PlanAmount,SlotCode,SlotName,LeftOutDays,LeftOutAmount,CreatedBy,CreatedOn,IsDeleted,IsActive,BranchCode,IsTransfer,NewMembersCode,InvoiceID,NewInvoiceID) values('" + fi.FromMemberShipCode + "','" + fi.FromMFDID + "','','','" + fi.FromPackageCode + "','" + fi.FromPackageName + "','" + fi.FromSessionCode + "','" + fi.FromSessionName + "','" + fi.FromPlanCode + "','" + fi.FromPlanName + "','" + fi.FromPlanAmount + "','" + fi.FromSlotCode + "','" + fi.FromSlotName + "','" + fi.FromLeftOutDays + "','" + fi.FromLeftOutAmount + "','" + fi.CreatedBy + "','" + ServerDateTime + "',0,1,'" + fi.FromBranchCode + "','" + fi.IsTransferUsed + "','','" + Invoice + "','" + Invoice2 + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FAInvoice(InvoiceID,GSTCode,FAPaymentModes,MembershipCode,PayableAmount,AmountDue,GymFees,TrainerFees,IGSTableAmount,IGST,CGST,SGST,DueDate,CreatedBy,CreatedOn,IsDeleted,IsActive,FinalAmount,FAPaymentModes2,PayableAmount2,PromoCode,DiscountAmount,TrainerCode,Receipt,EnquireTypeNo,SlotPrice,PlanCost,PaymentDate,RemainingAmount,Wallet,Recouncelled,Matchedid) values('" + Invoice + "','" + fi.ToGSTCodes + "','" + fi.ToFAPaymentModes + "','" + fi.FromMemberShipCode + "','" + fi.ToPayableAmount + "','" + fi.ToAmountDue + "','" + fi.ToGymFee + "','" + fi.ToTrainerFees + "','" + fi.ToIGSTableAmount + "','" + fi.ToIGST + "','" + fi.ToCGST + "','" + fi.ToSGST + "','" + fi.ToDueDate + "','" + fi.CreatedBy + "','" + ServerDateTime + "',0,1,'" + fi.ToFinalAmount + "','" + fi.ToFAPaymentModes2 + "','" + fi.ToPromoCode + "','" + fi.ToDiscountAmount + "','" + fi.ToTrainerCode + "','" + ReceiptGeneration() + "','" + fi.EnquireTypeNo + "','" + fi.SlotPrice + "','" + fi.PlanCost + "','" + fi.PaymentDate + "','" + fi.RemainingAmount + "','" + fi.WalletAmountUsed + "','','')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into Remainders(MembershipCode,RemainderCode,RemainderName,FAInvoice,Amount,Date,CreatedBy,CreatedOn) values('" + fi.FromMemberShipCode + "','','','" + Invoice + "','" + fi.ToFinalAmount + "','" + fi.ToDueDate + "','" + fi.CreatedBy + "','" + ServerDateTime + "')";
                command.ExecuteNonQuery();
                command.CommandText = "update CCRMMembership set MembershipExpireDate='" + ServerDateTime + "' where ID=(select MAX(ID) as ID from CCRMMembership where MembershipCode='" + fi.FromMemberShipCode + "'  and InvoiceID='" + Invoice2 + "')";
                command.ExecuteNonQuery();

                if (fi.Comments != "")
                {
                    command.CommandText = "insert into CommentsTracker(UCode,MasterID,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values()";
                    command.ExecuteNonQuery();
                }
                if (WalletUsed != 0.0f)
                {
                    command.CommandText = "insert into WalletTransactions(MembershipCode,NewMembershipCode,InvoiceId,TransactionId,TransactionName,MobileNo,Credit,Debit,CreatedOn,CreatedBy,IsDeleted,IsActive) values('" + fi.FromMemberShipCode + "','','" + Invoice + "',1,'','','','','" + ServerDateTime + "','" + fi.CreatedBy + "',0,1) ";
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                cnn.Close();
                result = "1";
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                    result = "0";
                }
                catch (Exception ex2)
                {
                    result = "0";
                }
            }



            if (result == "1")
            {
                TransferBit(fi.FromMemberShipCode, Convert.ToInt32(fi.FromMFDID));
                DataRow drMsg = dtMsg.NewRow();
                drMsg["UserID"] = r;
                dtMsg.Rows.Add(drMsg);

            }
            else
            {

                ds_custdet.Tables.Clear();

            }
            return result;
        }
        public Object RegisteredLocationToLocationTransfer([FromBody] TransferInput fi)
        {


            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet = new DataSet();
            DataTable dtMsg = new DataTable("MSG");
            dtMsg.Columns.Add("UserID");

            string SendText = string.Empty;
            string result = "";

            string Invoice = "N" + UniqueGeneration2();
            if (InvoiceCheck(Invoice) != 0)
            {
                Invoice = "N" + UniqueGeneration2();
            }

            // DataTable Invoice2 = GetFAInvoiceLatestRecord(UserInfo[0].FromMemberShipCode);
            string Invoice2 = GetMembersInvoiceNumberForFacilityPosts(fi.FromMemberShipCode);


            //Receipt
            string ReceiptNo = Convert.ToString(ReceiptGeneration());

            float WalletUsed = 0.0f;

            try
            {
                WalletUsed = float.Parse(fi.WalletAmountUsed);
            }
            catch
            {
                WalletUsed = 0.0f;
            }



            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            // Start a local transaction.
            transaction = cnn.BeginTransaction("SampleTransaction");
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = cnn;
            command.Transaction = transaction;


            try
            {
                command.CommandText = "insert into CCRMMembership(MembershipCode,BranchCode,PlanCode,MembershipExpireDate,CreatedBy,CreatedOn,IsDeleted,IsActive,SlotCode,PackageCode,StartDate,TrainerCode,InvoiceID,Receipt,PlanCostCode,DurationId) values('" + fi.FromMemberShipCode + "','" + fi.FromBranchCode + "','" + fi.FromPlanCode + "','" + fi.FromMembershipExpireDate + "','" + fi.CreatedBy + "','" + ServerDateTime + "',0,1,,'" + fi.FromSlotCode + "','" + fi.FromPackageCode + "','" + fi.FromMembershipStartDate + "','" + fi.ToTrainerCode + "','" + Invoice + "','" + ReceiptGeneration() + "','" + fi.ToPlanCostCode + "','" + fi.ToDurationCode + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,FacilityStartDate,FacilityExpireDate,PackageCode,PackageName,SessionCode,SessionName,PlanCode,PlanName,PlanAmount,SlotCode,SlotName,LeftOutDays,LeftOutAmount,CreatedBy,CreatedOn,IsDeleted,IsActive,BranchCode,IsTransfer,NewMembersCode,InvoiceID,NewInvoiceID) values('" + fi.FromMemberShipCode + "','" + fi.FromMFDID + "','','','" + fi.FromPackageCode + "','" + fi.FromPackageName + "','" + fi.FromSessionCode + "','" + fi.FromSessionName + "','" + fi.FromPlanCode + "','" + fi.FromPlanName + "','" + fi.FromPlanAmount + "','" + fi.FromSlotCode + "','" + fi.FromSlotName + "','" + fi.FromLeftOutDays + "','" + fi.FromLeftOutAmount + "','" + fi.CreatedBy + "','" + ServerDateTime + "',0,1,'" + fi.FromBranchCode + "','" + fi.IsTransferUsed + "','','" + Invoice + "','" + Invoice2 + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FAInvoice(InvoiceID,GSTCode,FAPaymentModes,MembershipCode,PayableAmount,AmountDue,GymFees,TrainerFees,IGSTableAmount,IGST,CGST,SGST,DueDate,CreatedBy,CreatedOn,IsDeleted,IsActive,FinalAmount,FAPaymentModes2,PayableAmount2,PromoCode,DiscountAmount,TrainerCode,Receipt,EnquireTypeNo,SlotPrice,PlanCost,PaymentDate,RemainingAmount,Wallet,Recouncelled,Matchedid) values('" + Invoice + "','" + fi.ToGSTCodes + "','" + fi.ToFAPaymentModes + "','" + fi.FromMemberShipCode + "','" + fi.ToPayableAmount + "','" + fi.ToAmountDue + "','" + fi.ToGymFee + "','" + fi.ToTrainerFees + "','" + fi.ToIGSTableAmount + "','" + fi.ToIGST + "','" + fi.ToCGST + "','" + fi.ToSGST + "','" + fi.ToDueDate + "','" + fi.CreatedBy + "','" + ServerDateTime + "',0,1,'" + fi.ToFinalAmount + "','" + fi.ToFAPaymentModes2 + "','" + fi.ToPromoCode + "','" + fi.ToDiscountAmount + "','" + fi.ToTrainerCode + "','" + ReceiptGeneration() + "','" + fi.EnquireTypeNo + "','" + fi.SlotPrice + "','" + fi.PlanCost + "','" + fi.PaymentDate + "','" + fi.RemainingAmount + "','" + fi.WalletAmountUsed + "','','')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into Remainders(MembershipCode,RemainderCode,RemainderName,FAInvoice,Amount,Date,CreatedBy,CreatedOn) values('" + fi.FromMemberShipCode + "','','','" + Invoice + "','" + fi.ToFinalAmount + "','" + fi.ToDueDate + "','" + fi.CreatedBy + "','" + ServerDateTime + "')";
                command.ExecuteNonQuery();

                command.CommandText = "update CCRMMembership set MembershipExpireDate='" + ServerDateTime + "' where ID=(select MAX(ID) as ID from CCRMMembership where MembershipCode='" + fi.FromMemberShipCode + "'  and InvoiceID='" + Invoice2 + "')";
                command.ExecuteNonQuery();

                if (fi.Comments != "")
                {
                    command.CommandText = "insert into CommentsTracker(UCode,MasterID,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values()";
                    command.ExecuteNonQuery();
                }
                if (WalletUsed != 0.0f)
                {
                    command.CommandText = "insert into WalletTransactions(MembershipCode,NewMembershipCode,InvoiceId,TransactionId,TransactionName,MobileNo,Credit,Debit,CreatedOn,CreatedBy,IsDeleted,IsActive) values('" + fi.FromMemberShipCode + "','','" + Invoice + "',1,'','','','','" + ServerDateTime + "','" + fi.CreatedBy + "',0,1) ";
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                cnn.Close();
                result = "1";
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                    result = "0";
                }
                catch (Exception ex2)
                {
                    result = "0";
                }
            }


            if (result == "1")
            {
                TransferBit(fi.FromMemberShipCode, Convert.ToInt32(fi.FromMFDID));

            }
            else
            {
                result = "Fail";
            }
            return result;
        }
        public ArrayList MSerialCheck(string BranchCode)
        {
            int Year = DateTime.Now.Year - 2000;
            int Month = DateTime.Now.Month;


            if (cnn.State == ConnectionState.Open)
            {
                cnn.Close();
            }
            ArrayList al = new ArrayList();
            int SerialNo = 0;
            string SCode = string.Empty;
            string Codification = string.Empty;
            int Count = 0;
            string query1 = "select Max(SerialNo),COUNT(*) as SerialNo from CCRMMembershipCodification where BranchCode='" + BranchCode + "' and Year=" + Year + "  and Month=" + Month + " ";
            try
            {
                using (SqlCommand cmd_RecGen = new SqlCommand(query1, cnn))
                {
                    cnn.Open();
                    SqlDataReader DR_RecGen = cmd_RecGen.ExecuteReader();
                    if (DR_RecGen.Read())
                    {
                        try
                        {
                            SerialNo = Convert.ToInt32(DR_RecGen[0].ToString());
                            Count = Convert.ToInt32(DR_RecGen[0].ToString());
                        }
                        catch (Exception ec)
                        {
                            Count = 0;

                        }
                    }

                    cnn.Close();
                }

                if (Count == 0)
                {
                    SerialNo = 1;
                    SCode = "001";
                }
                else
                {
                    SerialNo = SerialNo + 1;

                    if (SerialNo > 1 && SerialNo < 10)
                        SCode = "00" + Convert.ToString(SerialNo);
                    else if (SerialNo > 9 && SerialNo < 100)
                        SCode = "0" + Convert.ToString(SerialNo);
                    else if (SerialNo > 99 && SerialNo < 1000)
                        SCode = Convert.ToString(SerialNo);
                    else
                        SCode = "00000";
                }

            }
            catch (Exception ec)
            {
                SerialNo = 0;
            }
            finally
            {

                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
            }

            if (Month < 10)
            {
                Codification = Convert.ToString(Year) + "0" + Convert.ToString(Month) + SCode;
            }
            else
            {
                Codification = Convert.ToString(Year) + Convert.ToString(Month) + SCode;
            }


            al.Add(SCode);
            al.Add(Codification);
            return al;
        }
        public ArrayList MCode(string BranchCode)
        {
            ArrayList al = new ArrayList();
            string MNCode = string.Empty;
            String BCode = BranchCode.Substring(1, BranchCode.Length - 1);
            ArrayList SerialNo = MSerialCheck(BranchCode);
            MNCode = SerialNo[1].ToString() + BCode;
            al.Add(SerialNo[0].ToString());
            al.Add(MNCode);

            return al;
        }
        public string GetPhotoUrl(string Base64String, string Id)
        {
            string urlform = "";
            string endpath = "";
            try
            {

                string base64string = Base64String;
                var bytes = Convert.FromBase64String(base64string);

                endpath = "MembersPhotoUrl" + Id + ".png";

                string filepath = @"C:\inetpub\wwwroot\GYMUI\PhotoUrls\\" + endpath;
                using (var imageFile = new FileStream(filepath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }
                urlform = "http://137.59.201.211/GYMUI/PhotoUrls/" + endpath;
            }
            catch (Exception ec)
            {
                urlform = "";
            }

            return urlform;
        }
        public Object NewPersonToPersonTransfer([FromBody]NewMemberTransfer nmt)
        {
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            int Year = DateTime.Now.Year - 2000;
            int Month = DateTime.Now.Month;
            string r = "";
            // Clients Unique COde
            ArrayList al = new ArrayList();
            al = MCode(nmt.ToBranchCode);
            r = (String)al[1].ToString();




            string SendText = string.Empty;
            string result = "";

            string Invoice = "N" + UniqueGeneration2();

            if (InvoiceCheck(Invoice) != 0)
            {
                Invoice = "N" + UniqueGeneration2();
            }

            //DataTable Invoice2 = GetFAInvoiceLatestRecord(UserInfo[0].FromMemberShipCode);
            string Invoice2 = GetMembersInvoiceNumberForFacilityPosts(nmt.FromMemberShipCode);


            //Receipt
            string ReceiptNo = Convert.ToString(ReceiptGeneration());

            int generation = ReceiptGeneration();

            string PhotoUrl = GetPhotoUrl(nmt.Photo, r);

            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            // Start a local transaction.
            transaction = cnn.BeginTransaction("SampleTransaction");
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = cnn;
            command.Transaction = transaction;


            try
            {
                command.CommandText = "insert into Users(UCode,BranchCode,UserName,Firstname,Lastname,DateOfBirth,Gender,MaritialStatus,Address,Area,City,State,Country,PinCode,MobileNo,CreatedBy,CreatedOn,IsDeleted,IsActive,Photo,Email,PhotoUrl) values('" + r + "','" + nmt.BranchCode + "','" + nmt.UserName + "','" + nmt.Firstname + "','" + nmt.Lastname + "','" + nmt.DateOfBirth + "','" + nmt.Gender + "','" + nmt.MaritialStatus + "','" + nmt.Address + "','" + nmt.Area + "','" + nmt.City + "','" + nmt.State + "','" + nmt.Country + "','" + nmt.PinCode + "','" + nmt.MobileNo + "','" + nmt.CreatedBy + "','" + ServerDateTime + "',0,1,'" + nmt.Photo + "','" + nmt.Email + "','" + nmt.Photo + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into Login(UCode,UserName,PaswordSalt,PasswordHash,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + nmt.MembershipCode + "','" + nmt.UserName + "','" + nmt.PasswordSalt + "','" + nmt.PasswordHash + "','" + nmt.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMembershipCodification(SerialNo,MembershipCode,BranchCode,Year,Month) values('" + al[0].ToString() + "','" + nmt.MembershipCode + "','" + nmt.BranchCode + "','" + Year + "','" + Month + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMembership(MembershipCode,BranchCode,PlanCode,MembershipExpireDate,CreatedBy,CreatedOn,IsDeleted,IsActive,SlotCode,PackageCode,StartDate,TrainerCode,InvoiceID,Receipt,PlanCostCode,DurationId) values('" + r + "','" + nmt.ToBranchCode + "','" + nmt.ToPlanCode + "','" + nmt.ToMembershipExpireDate + "','" + nmt.CreatedBy + "','" + ServerDateTime + "',0,1,'" + nmt.FromSlotCode + "','" + nmt.FromPackageCode + "','" + nmt.FromMembershipStartDate + "','" + nmt.ToTrainerCode + "','" + Invoice + "','" + generation + "','" + nmt.ToPlanCostCode + "','" + nmt.ToDurationCode + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,FacilityStartDate,FacilityExpireDate,PackageCode,PackageName,SessionCode,SessionName,PlanCode,PlanName,PlanAmount,SlotCode,SlotName,LeftOutDays,LeftOutAmount,CreatedBy,CreatedOn,IsDeleted,IsActive,BranchCode,IsTransfer,NewMembersCode,InvoiceID,NewInvoiceID) values('" + nmt.FromMemberShipCode + "','" + nmt.FromMFDID + "','','','" + nmt.FromPackageCode + "','" + nmt.FromPackageName + "','" + nmt.FromSessionCode + "','" + nmt.FromSessionName + "','" + nmt.FromPlanCode + "','" + nmt.FromPlanName + "','" + nmt.FromPlanAmount + "','" + nmt.FromSlotCode + "','" + nmt.FromSlotName + "','" + nmt.FromLeftOutDays + "','" + nmt.FromLeftOutAmount + "','" + nmt.CreatedBy + "','" + ServerDateTime + "',0,1,'" + nmt.FromBranchCode + "','" + nmt.IsTransferUsed + "','','" + Invoice + "','" + Invoice2 + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FAInvoice(InvoiceID,GSTCode,FAPaymentModes,MembershipCode,PayableAmount,AmountDue,GymFees,TrainerFees,IGSTableAmount,IGST,CGST,SGST,DueDate,CreatedBy,CreatedOn,IsDeleted,IsActive,FinalAmount,FAPaymentModes2,PayableAmount2,PromoCode,DiscountAmount,TrainerCode,Receipt,EnquireTypeNo,SlotPrice,PlanCost,PaymentDate,RemainingAmount,Wallet,Recouncelled,Matchedid) values('" + Invoice + "','" + nmt.ToGSTCodes + "','" + nmt.ToFAPaymentModes + "','" + nmt.FromMemberShipCode + "','" + nmt.ToPayableAmount + "','" + nmt.ToDueAmount + "','" + nmt.ToGymFee + "','" + nmt.ToTrainerFees + "','" + nmt.ToIGSTableAmount + "','" + nmt.ToIGST + "','" + nmt.ToCGST + "','" + nmt.ToSGST + "','" + nmt.ToDueDate + "','" + nmt.CreatedBy + "','" + ServerDateTime + "',0,1,'" + nmt.ToFinalAmount + "','" + nmt.ToFAPaymentModes + "','" + nmt.ToFAPaymentModes2 + "','" + nmt.ToPromoCode + "','" + nmt.ToDiscountAmount + "','" + nmt.ToTrainerCode + "','" + generation + "','" + nmt.EnquireTypeNo + "','" + nmt.SlotPrice + "','" + nmt.PlanCost + "','" + nmt.PaymentDate + "','" + nmt.RemainingAmount + "','" + nmt.WalletAmountUsed + "','','')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into Remainders(MembershipCode,RemainderCode,RemainderName,FAInvoice,Amount,Date,CreatedBy,CreatedOn) values('" + nmt.FromMemberShipCode + "','','','" + Invoice + "','" + nmt.ToFinalAmount + "','" + nmt.ToDueDate + "','" + nmt.CreatedBy + "','" + ServerDateTime + "')";
                command.ExecuteNonQuery();

                //server date time
                command.CommandText = "update CCRMMembership set MembershipExpireDate='' where ID=(select MAX(ID) as ID from CCRMMembership where MembershipCode='" + nmt.FromMemberShipCode + "'  and InvoiceID='" + Invoice2 + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMEnquireStatus(MobileNo,EnquireTypeNo,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + nmt.MobileNo + "','" + nmt.EnquireTypeNo + "','" + nmt.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                if (nmt.Comments != "")
                {
                    command.CommandText = "insert into CommentsTracker(UCode,MasterID,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + nmt.MembershipCode + "',0,'" + nmt.Comments + "','" + nmt.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                }
                if (nmt.WalletAmountUsed != "")
                {
                    command.CommandText = "insert into WalletTransactions(MembershipCode,NewMembershipCode,InvoiceId,TransactionId,TransactionName,MobileNo,Credit,Debit,CreatedOn,CreatedBy,IsDeleted,IsActive) values('" + nmt.FromMemberShipCode + "','','" + Invoice + "',1,'','','','','" + ServerDateTime + "','" + nmt.CreatedBy + "',0,1) ";
                    command.ExecuteNonQuery();
                }


                transaction.Commit();
                cnn.Close();
                result = "1";
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                    result = "0";
                }
                catch (Exception ex2)
                {
                    result = "0";
                }
            }

            DataSet ds_custdet = new DataSet();
            DataTable dtMsg = new DataTable("MSG");
            dtMsg.Columns.Add("UserID");
            dtMsg.Columns.Add("Invoice No: ");
            dtMsg.Columns.Add("Receipt No: ");



            return result;
        }

        // FromDetails from db server
        public Object NewLocationToLocationTransfer([FromBody]NewMemberTransfer nmt)
        {
            string result = "";
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int Year = DateTime.Now.Year - 2000;
            int Month = DateTime.Now.Month;
            string r = "";
            // Clients Unique COde
            ArrayList al = new ArrayList();
            al = MCode(nmt.ToBranchCode);
            r = (String)al[1].ToString();
            r = r.Substring(0, 10);

            TransferBit(nmt.FromMemberShipCode, Convert.ToInt32(nmt.FromMFDID));

            string Invoice = "N" + UniqueGeneration2();

            if (InvoiceCheck(Invoice) != 0)
            {
                Invoice = "N" + UniqueGeneration2();
            }





            DataTable dt_PEDetails = new DataTable();
            dt_PEDetails = getdata(string.Format("select BranchCode,EnquirePersonFirstName as FirstName,EnquirePersonLastName as LastName,Gender,Email,D_O_B,Address from CCRMMEnquireForm where MobileNo = '{0}'", nmt.MobileNo));

            DataTable dt_MDetails = new DataTable();
            dt_MDetails = getdata(string.Format("select top 1 FAI.MembershipCode,FAI.InvoiceID,CCRMM.BranchCode,CMSSS.SessionCode,CMSS.SessionName,CCRMM.SlotCode,CMSSS.SlotName,CCRMM.PackageCode,CMSPAC.PackageName,CCRMM.PlanCode,CMSP.PlanName,CMSPAC.PlanCostCode,CCRMM.DurationId,  from FAInvoice FAI, CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,CMSPACKAGESCOST CMSPAC,CMSSESSIONTIMESETTING CMSS,CMSPLAN CMSP where CCRMM.PlanCode=CMSP.PlanCode and CMSS.SessionCode=CMSSS.SessionCode and CCRMM.SlotCode=CMSSS.SlotCode and CCRMM.PackageCode=CMSPAC.PackageCode and  FAI.InvoiceID ='{0}' ", nmt.InvoiceID));


            DataTable dt_Invoice = new DataTable();
            dt_Invoice = getdata(string.Format("select Top 1 GSTCode,FAPaymentModes,FAPaymentModes2,PayableAmount,PayableAmount2,GSTCode,IGST,CGST,SGST,GymFees,FinalAmount,DiscountAmount,TrainerCode,Wallet,PaymentDate,AmountDue,TrainerFees,IGSTableAmount,PromoCode,SlotPrice,PlanCost from  FAInvoice where InvoiceID='{0}' order by ID desc", nmt.InvoiceID));

            int generation = ReceiptGeneration();
            string Invoice2 = GetMembersInvoiceNumberForFacilityPosts(dt_MDetails.Rows[0]["MembershipCode"].ToString());
            string ReceiptNo = Convert.ToString(ReceiptGeneration());
            string PhotoUrl = GetPhotoUrl(nmt.Photo, r);

            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            // Start a local transaction.
            transaction = cnn.BeginTransaction("SampleTransaction");
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = cnn;
            command.Transaction = transaction;


            try
            {
                command.CommandText = "insert into Users(UCode,BranchCode,UserName,Firstname,Lastname,DateOfBirth,Gender,MaritialStatus,Address,Area,City,State,Country,PinCode,MobileNo,CreatedBy,CreatedOn,IsDeleted,IsActive,Photo,Email,PhotoUrl) values('" + r + "','" + nmt.BranchCode + "','" + nmt.UserName + "','" + nmt.Firstname + "','" + nmt.Lastname + "','" + nmt.DateOfBirth + "','" + nmt.Gender + "','" + nmt.MaritialStatus + "','" + nmt.Address + "','" + nmt.Area + "','" + nmt.City + "','" + nmt.State + "','" + nmt.Country + "','" + nmt.PinCode + "','" + nmt.MobileNo + "','" + nmt.CreatedBy + "','" + ServerDateTime + "',0,1,'" + nmt.Photo + "','" + nmt.Email + "','" + nmt.Photo + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into Login(UCode,UserName,PaswordSalt,PasswordHash,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "','" + nmt.UserName + "','" + nmt.PasswordSalt + "','" + nmt.PasswordHash + "','" + nmt.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMembershipCodification(SerialNo,MembershipCode,BranchCode,Year,Month) values('" + al[0].ToString() + "','" + r + "','" + nmt.BranchCode + "','" + Year + "','" + Month + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMembership(MembershipCode,BranchCode,PlanCode,MembershipExpireDate,CreatedBy,CreatedOn,IsDeleted,IsActive,SlotCode,PackageCode,StartDate,TrainerCode,InvoiceID,Receipt,PlanCostCode,DurationId) values('" + r + "','" + nmt.ToBranchCode + "','" + nmt.ToPlanCode + "','" + nmt.ToMembershipExpireDate + "','" + nmt.CreatedBy + "','" + ServerDateTime + "',0,1,'" + dt_MDetails.Rows[0]["SlotCode"].ToString() + "','" + dt_MDetails.Rows[0]["PackageCode"].ToString() + "','" + dt_MDetails.Rows[0]["StartDate"].ToString() + "','" + nmt.ToTrainerCode + "','" + Invoice + "','" + generation + "','" + nmt.ToPlanCostCode + "','" + nmt.ToDurationCode + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,FacilityStartDate,FacilityExpireDate,PackageCode,PackageName,SessionCode,SessionName,PlanCode,PlanName,PlanAmount,SlotCode,SlotName,LeftOutDays,LeftOutAmount,CreatedBy,CreatedOn,IsDeleted,IsActive,BranchCode,IsTransfer,NewMembersCode,InvoiceID,NewInvoiceID) values('" + dt_MDetails.Rows[0]["MembershipCode"].ToString() + "','" + nmt.FromMFDID + "','','','" + dt_MDetails.Rows[0]["PackageCode"].ToString() + "','" + dt_MDetails.Rows[0]["PackageName"].ToString() + "','" + dt_MDetails.Rows[0]["SessionCode"].ToString() + "','" + dt_MDetails.Rows[0]["SessionName"].ToString() + "','" + dt_MDetails.Rows[0]["PlanCode"].ToString() + "','" + dt_MDetails.Rows[0]["PlanName"].ToString() + "','" + dt_Invoice.Rows[0]["PlanCost"].ToString() + "','" + dt_MDetails.Rows[0]["SlotCode"].ToString() + "','" + dt_MDetails.Rows[0]["SlotName"].ToString() + "','" + nmt.FromLeftOutDays + "','" + nmt.FromLeftOutAmount + "','" + nmt.CreatedBy + "','" + ServerDateTime + "',0,1,'" + dt_MDetails.Rows[0]["BranchCode"].ToString() + "','" + nmt.IsTransferUsed + "','','" + Invoice + "','" + Invoice2 + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into FAInvoice(InvoiceID,GSTCode,FAPaymentModes,MembershipCode,PayableAmount,AmountDue,GymFees,TrainerFees,IGSTableAmount,IGST,CGST,SGST,DueDate,CreatedBy,CreatedOn,IsDeleted,IsActive,FinalAmount,FAPaymentModes2,PayableAmount2,PromoCode,DiscountAmount,TrainerCode,Receipt,EnquireTypeNo,SlotPrice,PlanCost,PaymentDate,RemainingAmount,Wallet,Recouncelled,Matchedid) values('" + Invoice + "','" + nmt.ToGSTCodes + "','" + nmt.ToFAPaymentModes + "','" + nmt.FromMemberShipCode + "','" + nmt.ToPayableAmount + "','" + nmt.ToDueAmount + "','" + nmt.ToGymFee + "','" + nmt.ToTrainerFees + "','" + nmt.ToIGSTableAmount + "','" + nmt.ToIGST + "','" + nmt.ToCGST + "','" + nmt.ToSGST + "','" + nmt.ToDueDate + "','" + nmt.CreatedBy + "','" + ServerDateTime + "',0,1,'" + nmt.ToFinalAmount + "','" + nmt.ToFAPaymentModes + "','" + nmt.ToFAPaymentModes2 + "','" + nmt.ToPromoCode + "','" + nmt.ToDiscountAmount + "','" + nmt.ToTrainerCode + "','" + generation + "','" + nmt.EnquireTypeNo + "','" + nmt.SlotPrice + "','" + nmt.PlanCost + "','" + nmt.PaymentDate + "','" + nmt.RemainingAmount + "','" + nmt.WalletAmountUsed + "','','')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into Remainders(MembershipCode,RemainderCode,RemainderName,FAInvoice,Amount,Date,CreatedBy,CreatedOn) values('" + nmt.FromMemberShipCode + "','','','" + Invoice + "','" + nmt.ToFinalAmount + "','" + nmt.ToDueDate + "','" + nmt.CreatedBy + "','" + ServerDateTime + "')";
                command.ExecuteNonQuery();
                command.CommandText = "update CCRMMembership set MembershipExpireDate='' where ID=(select MAX(ID) as ID from CCRMMembership where MembershipCode='" + nmt.MembershipCode + "'  and InvoiceID='" + Invoice2 + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMEnquireStatus(MobileNo,EnquireTypeNo,CreatedBy,CreatedOn,IsDeleted,IsActive)  values('" + r + "','" + nmt.EnquireTypeNo + "','" + nmt.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                if (nmt.Comments != "")
                {
                    command.CommandText = "insert into CommentsTracker(UCode,MasterID,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',0,'" + nmt.Comments + "','" + nmt.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                }


            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (Exception ex2)
                {

                }
            }



            DataSet ds_custdet = new DataSet();
            DataTable dtMsg = new DataTable("MSG");

            dtMsg.Columns.Add("UserID");
            dtMsg.Columns.Add("Invoice No: ");
            dtMsg.Columns.Add("Receipt No: ");



            return result;
        }

        // FacilityInfo(Function) : // Facilities (Objects)

        public Object GetAllFacilities([FromBody]AllFacilityInput fci)
        {

            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();

            AllFacilityOutput ppopt = new AllFacilityOutput();
            try
            {
                List<AllFacilityCombinedList> allFacility = new List<AllFacilityCombinedList>();

                AllFacilityCombinedList allFacilityDetails = new AllFacilityCombinedList { freezingDetails = FreezingFacility(fci.MembershipCode, fci.StartDate), graceDetails = GraceFacility(), upgradationDetails = UpgradationFacility(fci.MembershipCode, fci.StartDate), changeDetails = ChangeFacility(fci.MembershipCode, fci.StartDate), locationTransferDetails = LocationTransferFacility(fci.MembershipCode, fci.StartDate), personTransferDetails = PersonTransferFacility(fci.MembershipCode, fci.StartDate), extendDetails = ExtendFacility(), holdDetails = HoldFacility(fci.MembershipCode, fci.StartDate) };
                allFacility.Add(allFacilityDetails);

                ppopt.status = "success";
                ppopt.value = allFacility;
                sJSONResponse = JsonConvert.SerializeObject(ppopt);

            }
            catch (Exception ec)
            {
                ppopt.status = "success";
                sJSONResponse = JsonConvert.SerializeObject(ppopt);

            }
            return sJSONResponse;

        }

        public string GetMembersInvoiceNumber(string MemberShipCode, string StartDate)
        {
            string r = "";
            string Invoicenumber = "";
            if (cnn.State == ConnectionState.Open)
            {
                cnn.Close();

            }
            try
            {
                string query = "select top 1 FAI.InvoiceID from FAInvoice FAI,CCRMMembership CCRMM  where FAI.MembershipCode='" + MemberShipCode + "' and FAI.InvoiceID=CCRMM.InvoiceID  and StartDate between '" + StartDate + "' and '" + StartDate + " 23:59:59' order by FAI.ID desc";

                using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
                {
                    cnn.Open();
                    SqlDataReader GA_DR = cmd_SubEXT.ExecuteReader();
                    if (GA_DR.Read())
                    {
                        Invoicenumber = GA_DR[0].ToString();
                    }
                    cnn.Close();
                }
            }
            catch (Exception ex)
            {
                return "";
            }
            finally
            {
                cnn.Close();
            }
            return Invoicenumber;
        }

        // Freezing Start
        // Freezing Related Functions
        public int GetMembersLatestPlansFreezingSchems(string MemberShipCode)
        {
            int LatestPlansFreezing = 0;
            try
            {
                if (cnn.State == ConnectionState.Open)
                {
                    cnn.Close();
                }
                string query = "select distinct CPC.FreezingID  from CCRMMembership CCRM,CMSPLANCOST CPC  where CCRM.MemberShipCode='" + MemberShipCode + "' and CCRM.ID=(select top 1 ID from CCRMMembership where MembershipCode='" + MemberShipCode + "' and StartDate <= GETDATE() order by ID desc) and CCRM.PlanCostCode=CPC.PlanCostCode";
                using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
                {
                    cnn.Open();
                    SqlDataReader GA_DR = cmd_SubEXT.ExecuteReader();
                    if (GA_DR.Read())
                    {
                        LatestPlansFreezing = Convert.ToInt32(GA_DR[0].ToString());
                    }
                    else
                    {
                        LatestPlansFreezing = 0;
                    }

                    cnn.Close();
                }
            }
            catch (Exception ec)
            {
                LatestPlansFreezing = 0;

            }
            return LatestPlansFreezing;

        }
        public int GetFacilitysFreezingUtlized(string MemberShipCode, string Invoice)
        {
            int count = 0;

            string query = "select count(*) as UtlizedCount from CCRMMembershipFacility  where MemberShipCode='" + MemberShipCode + "' and InvoiceID='" + Invoice + "' and MFDID=1";
            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader GA_DR = cmd_SubEXT.ExecuteReader();
                if (GA_DR.Read())
                {
                    count = Convert.ToInt32(GA_DR[0].ToString());

                }
                cnn.Close();
            }
            return count;

        }
        public DataTable GetMembersLatestFreezing(string MemberShipCode)
        {
            // Note MFDID Freezing is 1 
            string FreezingStartDate = string.Empty;
            string FreezingEndDate = string.Empty;
            int NoOfOptions = 0;
            int OptionsUsed = 0;
            int NoOfOptionCurrentlyInUse = 0;


            string query = "select top 1 FacilityStartDate,FacilityExpireDate,NoOfOptions,OptionsUsed,NoOfOptionCurrentlyInUse from CCRMMembershipFacility where MemberShipCode='" + MemberShipCode + "' and MFDID=1 order by SNO desc";
            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader GA_DR = cmd_SubEXT.ExecuteReader();
                if (GA_DR.Read())
                {
                    FreezingStartDate = GA_DR[0].ToString();
                    FreezingEndDate = GA_DR[1].ToString();
                    NoOfOptions = Convert.ToInt32(GA_DR[2].ToString());
                    OptionsUsed = Convert.ToInt32(GA_DR[3].ToString());
                    NoOfOptionCurrentlyInUse = Convert.ToInt32(GA_DR[4].ToString());
                }

                cnn.Close();
            }

            DataTable dtFreezeDate = new DataTable();
            DataRow row;
            DataColumn col1 = new DataColumn("FreezingStartDate", typeof(DateTime));
            DataColumn col2 = new DataColumn("FreezingEndDate", typeof(DateTime));
            DataColumn col3 = new DataColumn("NoOfOptions", typeof(string));
            DataColumn col4 = new DataColumn("OptionsUsed", typeof(string));
            DataColumn col5 = new DataColumn("NoOfOptionCurrentlyInUse", typeof(string));

            dtFreezeDate.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5 });

            if (FreezingStartDate != null && FreezingEndDate != null)
            {
                if (FreezingStartDate != "" && FreezingEndDate != "" && NoOfOptions != 0)
                {
                    row = dtFreezeDate.NewRow();
                    row["FreezingStartDate"] = FreezingStartDate;
                    row["FreezingEndDate"] = FreezingEndDate;
                    row["NoOfOptions"] = NoOfOptions;
                    row["OptionsUsed"] = OptionsUsed;
                    row["NoOfOptionCurrentlyInUse"] = NoOfOptionCurrentlyInUse;
                    dtFreezeDate.Rows.Add(row);
                }
                else
                {
                    row = dtFreezeDate.NewRow();
                    row["FreezingStartDate"] = DBNull.Value;
                    row["FreezingEndDate"] = DBNull.Value;
                    row["NoOfOptions"] = "";
                    row["OptionsUsed"] = "";
                    row["NoOfOptionCurrentlyInUse"] = "";
                    dtFreezeDate.Rows.Add(row);
                }

            }
            else
            {
                row = dtFreezeDate.NewRow();
                row["FreezingStartDate"] = DBNull.Value;
                row["FreezingEndDate"] = DBNull.Value;
                row["NoOfOptions"] = "";
                row["OptionsUsed"] = "";
                row["NoOfOptionCurrentlyInUse"] = "";
                dtFreezeDate.Rows.Add(row);

            }
            return dtFreezeDate;

        }
        public List<FreezingDetails> FreezingFacility(string MemberShipCode, string StartDate)
        {
            List<FreezingDetails> freezing = new List<FreezingDetails>();

            int FreezeCount = 0;
            int LatestPlansFreezing = GetMembersLatestPlansFreezingSchems(MemberShipCode);
            string InvoicNumber = GetMembersInvoiceNumber(MemberShipCode, StartDate);
            FreezeCount = GetFacilitysFreezingUtlized(MemberShipCode, InvoicNumber);
            DataTable FreezingDates = GetMembersLatestFreezing(MemberShipCode);
            ArrayList arl = GetMembersExpireDate(MemberShipCode);

            DateTime ExpireDate = Convert.ToDateTime(arl[0].ToString());

            DataTable dtFreezing = new DataTable();
            DataTable dtFreezing1 = new DataTable();
            DataTable dtFreezing2 = new DataTable();

            string sd = FreezingDates.Rows[0][0].ToString();
            string ed = FreezingDates.Rows[0][1].ToString();

            if (sd != "" && ed != "" && sd != "1/1/1900 12:00:00 AM" && ed != "1/1/1900 12:00:00 AM")
            {
                dtFreezing1 = getdata(string.Format("select FF.NoOfDays,CCRMF.OptionsUsed as DaysUsed,CCRMF.NoOfOptions,CCRMF.FacilityStartDate,CCRMF.FacilityExpireDate from CCRMMembershipFacility CCRMF,CMSPLANCOST CMSPC,FreezingFacility FF where CCRMF.MFDID=1 and CMSPC.PlanCostCode=CCRMF.PlanCostCode and CMSPC.FreezingID=FF.FreezingID and CCRMF.MemberShipCode='{0}' and CCRMF.InvoiceID='{1}' and FacilityExpireDate between GETDATE() and '{2}' order by CCRMF.SNO desc", MemberShipCode, InvoicNumber, ExpireDate));
                if (dtFreezing1.Rows.Count > 0)
                {
                    dtFreezing = dtFreezing1;
                }
                else
                {
                    dtFreezing2 = getdata(string.Format("select CCRMF.SMFMID,FF.FreezingName as Name,Frequency=0,CCRMF.NoOfOptions,FF.NoOfDays,CCRMF.OptionsUsed as DaysUsed,CCRMF.FacilityStartDate,CCRMF.FacilityExpireDate from CCRMMembershipFacility CCRMF,CMSPLANCOST CMSPC,FreezingFacility FF where CCRMF.MFDID=1 and CMSPC.PlanCostCode=CCRMF.PlanCostCode and CMSPC.FreezingID=FF.FreezingID and CCRMF.MemberShipCode='{0}' and CCRMF.InvoiceID='{1}' and FacilityExpireDate between GETDATE() and '10/07/2020' order by CCRMF.SNO desc", MemberShipCode, InvoicNumber, ExpireDate));
                    dtFreezing = dtFreezing2;
                }

            }
            // if datess empty
            else
            {

                dtFreezing1 = getdata(string.Format("select top 1  CCRMF.SMFMID,Name='',CCRMF.OptionsUsed,MFF.Frequency,CCRMF.NoOfOptions,CCRMF.FacilityStartDate,CCRMF.FacilityExpireDate,CCRMF.NoOfOptionCurrentlyInUse from CCRMMembershipFacility CCRMF,CMSPLANCOST CMSPC,MembersFacilityFreezing MFF where CCRMF.MFDID=1 and CMSPC.PlanCostCode=CCRMF.PlanCostCode and CMSPC.FreezingID=MFF.FreezingID and CCRMF.MemberShipCode='{0}' and FacilityStartDate='' and FacilityExpireDate='' order by CCRMF.SNO desc", MemberShipCode));

                if (dtFreezing1.Rows.Count > 0)
                {
                    dtFreezing = dtFreezing1;
                }
                else
                {
                    dtFreezing2 = getdata(string.Format("select top 1 MFF.SMFMID as MFDID,SMFM.SubSchemName as Name,Frequency,NoOfOptions,OptionsUsed=0,FacilityStartDate='01/01/1900',FacilityExpireDate='01/01/1900' from MembersFacilityFreezing MFF,SubMembersFacilityMaster SMFM where MFF.FreezingID={0} and MFF.SMFMID=SMFM.SMFMID", LatestPlansFreezing));
                    dtFreezing = dtFreezing2;
                }
            }

            for (int i = 0; i < dtFreezing.Rows.Count; i++)
            {
                freezing.Add(new FreezingDetails
                {
                    Name = Convert.ToString(dtFreezing.Rows[i]["Name"])
                      ,
                    MFDID = Convert.ToInt32(dtFreezing.Rows[i]["MFDID"])
                    ,
                    FreezingStartDate = Convert.ToDateTime(dtFreezing.Rows[i]["FacilityStartDate"])
                      ,
                    FreezingEndDate = Convert.ToDateTime(dtFreezing.Rows[i]["FacilityExpireDate"])
                    ,
                    Freequency = Convert.ToInt32(dtFreezing.Rows[i]["Frequency"])
                      ,
                    NoOfOptions = Convert.ToInt32(dtFreezing.Rows[i]["NoOfOptions"])
                    ,
                    NoOfOptionsCurrentlyInUse = Convert.ToInt32(dtFreezing.Rows[i]["OptionsUsed"])
                      ,
                    RemainingDays = 10
                    ,
                    IsFreezingUsed = false
                    ,

                });

            }
            return freezing;
        }

        // Freezing End

        public DataTable GetMembersUsedFacilitiesUpgradationDetails(string MemberShipCode, string BranchCode, string InvoiceID)
        {
            DataSet ds_custdet1 = new DataSet();
            string PromoCodes = "";
            string PromoCode = GetMembersPromoCode(MemberShipCode);
            DataTable dt_MemberShipFacilityDetails = new DataTable();
            DataRow row;
            DataColumn col1 = new DataColumn("SMFMID", typeof(string));
            DataColumn col2 = new DataColumn("SubSchemName", typeof(string));

            dt_MemberShipFacilityDetails.Columns.AddRange(new DataColumn[] { col1, col2 });

            string FacilityID = "";
            string FacilityName = "";
            string query = "select distinct MFDID  from CCRMMembershipFacility where MemberShipCode='" + MemberShipCode + "' and InvoiceID='" + InvoiceID + "'  and MFDID=3 and PlanCode=(select top 1 PlanCode from CCRMMembership where MembershipCode='" + MemberShipCode + "' order by ID desc)";

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            if (ds_custdet1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i <= ds_custdet1.Tables[0].Rows.Count - 1; i++)
                {
                    if (i == 0)
                    {
                        PromoCodes = ds_custdet1.Tables[0].Rows[i][0].ToString();
                    }
                    else
                    {
                        PromoCodes = PromoCodes + "," + ds_custdet1.Tables[0].Rows[i][0].ToString();
                    }
                }
            }

            string[] str = PromoCodes.Split(',');
            for (int i = 0; i <= str.Length - 1; i++)
            {
                if (str[i].ToString() != "" || str[i].ToString() != string.Empty)
                {
                    string query1 = "select distinct SMFMID,SubSchemName from SubMembersFacilityMaster where SMFMID = " + str[i].ToString() + "";
                    using (SqlCommand cmd_FC = new SqlCommand(query1, cnn))
                    {
                        if (cnn.State == ConnectionState.Open)
                        {
                            cnn.Close();
                        }
                        cnn.Open();
                        SqlDataReader FC_DR = cmd_FC.ExecuteReader();
                        if (FC_DR.Read())
                        {
                            FacilityID = FC_DR[0].ToString();
                            FacilityName = FC_DR[1].ToString();
                        }
                        else
                        {
                            FacilityID = "";
                            FacilityName = "";
                        }
                        cnn.Close();
                    }

                    row = dt_MemberShipFacilityDetails.NewRow();
                    row["SMFMID"] = FacilityID;
                    row["SubSchemName"] = FacilityName;
                    dt_MemberShipFacilityDetails.Rows.Add(row);
                }
                else
                {
                    row = dt_MemberShipFacilityDetails.NewRow();
                    row["SMFMID"] = "";
                    row["SubSchemName"] = "";
                    dt_MemberShipFacilityDetails.Rows.Add(row);

                }
            }

            return dt_MemberShipFacilityDetails;
        }
        public List<UpgradationDetails> UpgradationFacility(string MemberShipCode, string StartDate)
        {
            DataTable dt_MemberShipDetails = new DataTable();
            string result = string.Empty;

            DataRow row;
            DataColumn col1 = new DataColumn("MFDID", typeof(int));
            DataColumn col2 = new DataColumn("Name", typeof(string));
            DataColumn col3 = new DataColumn("IsUpgradeUsed", typeof(bool));

            dt_MemberShipDetails.Columns.AddRange(new DataColumn[] { col1, col2, col3 });

            string InvoicNumber = GetMembersInvoiceNumber(MemberShipCode, StartDate);

            try
            {
                string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                DataSet ds_custdet1 = new DataSet();
                DataSet ds_custdet2 = new DataSet();
                DataSet ds_custdet3 = new DataSet();


                int MFDID = 0;
                string Name = string.Empty;
                int Upgrade = 0;

                DataTable dt_UsedFacility = GetMembersUsedFacilitiesUpgradationDetails(MemberShipCode, "", InvoicNumber);

                for (int i = 0; i <= dt_UsedFacility.Rows.Count - 1; i++)
                {
                    try
                    {
                        if (Convert.ToInt32(dt_UsedFacility.Rows[i][0].ToString()) == 3)
                        {
                            Upgrade = 1;

                        }

                        else if (Convert.ToInt32(dt_UsedFacility.Rows[i][0].ToString()) == 5)
                        {
                            Upgrade = 1;
                        }
                        else if (Convert.ToInt32(dt_UsedFacility.Rows[i][0].ToString()) == 6)
                        {
                            Upgrade = 1;
                        }
                        else
                        {
                            Upgrade = 0;
                        }
                    }
                    catch (Exception ec)
                    {
                        Upgrade = 0;
                    }
                }

                string query = "select SMFMID as MFDID,SubSchemName as Name from SubMembersFacilityMaster MFD where  SMFMID=3";
                using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
                {
                    cnn.Open();
                    SqlDataReader GA_DR = cmd_SubEXT.ExecuteReader();
                    if (GA_DR.Read())
                    {
                        MFDID = Convert.ToInt32(GA_DR[0].ToString());
                        Name = GA_DR[1].ToString();
                    }
                    cnn.Close();
                }

                row = dt_MemberShipDetails.NewRow();
                row["MFDID"] = MFDID;
                row["Name"] = Name;
                row["IsUpgradeUsed"] = Upgrade;
                dt_MemberShipDetails.Rows.Add(row);
            }
            catch (Exception ec)
            {

                row = dt_MemberShipDetails.NewRow();
                row["MFDID"] = "3";
                row["Name"] = "UPGRADATION";
                row["IsUpgradeUsed"] = 0;
                dt_MemberShipDetails.Rows.Add(row);
            }



            List<UpgradationDetails> upgradation = new List<UpgradationDetails>();
            upgradation.Add(new UpgradationDetails { MFDID = dt_MemberShipDetails.Rows[0]["MFDID"].ToString(), Name = dt_MemberShipDetails.Rows[0]["Name"].ToString(), IsUpgradationUsed = "0" });
            return upgradation;
        }

        // Change Start

        public DataTable GetMembersUsedFacilitiesChangeDetails(string MemberShipCode, string BranchCode, string InvoiceID)
        {
            DataSet ds_custdet1 = new DataSet();
            string PromoCodes = "";
            string PromoCode = GetMembersPromoCode(MemberShipCode);
            DataTable dt_MemberShipFacilityDetails = new DataTable();
            DataRow row;
            DataColumn col1 = new DataColumn("SMFMID", typeof(string));
            DataColumn col2 = new DataColumn("SubSchemName", typeof(string));

            dt_MemberShipFacilityDetails.Columns.AddRange(new DataColumn[] { col1, col2 });

            string FacilityID = "";
            string FacilityName = "";
            string query = "select distinct MFDID  from CCRMMembershipFacility where MemberShipCode='" + MemberShipCode + "' and InvoiceID='" + InvoiceID + "'  and MFDID=4 and PlanCode=(select top 1 PlanCode from CCRMMembership where MembershipCode='" + MemberShipCode + "' order by ID desc)";

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            if (ds_custdet1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i <= ds_custdet1.Tables[0].Rows.Count - 1; i++)
                {
                    if (i == 0)
                    {
                        PromoCodes = ds_custdet1.Tables[0].Rows[i][0].ToString();
                    }
                    else
                    {
                        PromoCodes = PromoCodes + "," + ds_custdet1.Tables[0].Rows[i][0].ToString();
                    }
                }
            }

            string[] str = PromoCodes.Split(',');
            for (int i = 0; i <= str.Length - 1; i++)
            {
                if (str[i].ToString() != "" || str[i].ToString() != string.Empty)
                {
                    string query1 = "select distinct SMFMID,SubSchemName from SubMembersFacilityMaster where SMFMID = " + str[i].ToString() + "";
                    using (SqlCommand cmd_FC = new SqlCommand(query1, cnn))
                    {
                        if (cnn.State == ConnectionState.Open)
                        {
                            cnn.Close();
                        }
                        cnn.Open();
                        SqlDataReader FC_DR = cmd_FC.ExecuteReader();
                        if (FC_DR.Read())
                        {
                            FacilityID = FC_DR[0].ToString();
                            FacilityName = FC_DR[1].ToString();
                        }
                        else
                        {
                            FacilityID = "";
                            FacilityName = "";
                        }
                        cnn.Close();
                    }

                    row = dt_MemberShipFacilityDetails.NewRow();
                    row["SMFMID"] = FacilityID;
                    row["SubSchemName"] = FacilityName;
                    dt_MemberShipFacilityDetails.Rows.Add(row);
                }
                else
                {
                    row = dt_MemberShipFacilityDetails.NewRow();
                    row["SMFMID"] = "4";
                    row["SubSchemName"] = "PLAN CHANGE";
                    dt_MemberShipFacilityDetails.Rows.Add(row);

                }
            }

            return dt_MemberShipFacilityDetails;
        }

        //100
        public List<ChangeDetails> ChangeFacility(string MemberShipCode, string StartDate)
        {
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet1 = new DataSet();

            DataTable dt_Transfer = new DataTable();

            double FinalAmount = 0.0f;
            double FinalAmount2 = 0.0f;
            double IGST = 0.0f;
            double FinalAmount1 = 0.0f;
            double TransferfeePercentage = 0.0f;
            double Transferfee = 0.0f;
            double AmountPerDay = 0.0f;
            double LeftOutDays = 0.0d;
            double CompleteDays = 0.0d;
            double TotalDays = 0.0d;
            double RemainingAmount = 0.0d;
            int IsChangePackageUsed = 0;

            FacilityOutputStatus ffopt = new FacilityOutputStatus();

            string result = string.Empty;

            string InvoicNumber = GetMembersInvoiceNumber(MemberShipCode, StartDate);

            try
            {
                DataTable dt_UsedFacility = GetMembersUsedFacilitiesChangeDetails(MemberShipCode, "", InvoicNumber);

                if (dt_UsedFacility.Rows.Count > 0)
                {

                    for (int i = 0; i <= dt_UsedFacility.Rows.Count - 1; i++)
                    {
                        if (Convert.ToInt32(dt_UsedFacility.Rows[i][0].ToString()) == 4)
                        {
                            IsChangePackageUsed = 1;

                        }

                        else if (Convert.ToInt32(dt_UsedFacility.Rows[i][0].ToString()) == 5)
                        {
                            IsChangePackageUsed = 1;
                        }
                        else if (Convert.ToInt32(dt_UsedFacility.Rows[i][0].ToString()) == 6)
                        {
                            IsChangePackageUsed = 1;
                        }
                        else
                        {
                            IsChangePackageUsed = 0;
                        }

                    }

                }
                else
                {
                    IsChangePackageUsed = 0;

                }

                ArrayList dates = MembersLatestDates(MemberShipCode);
                ArrayList Amount = MembersLatestAmount(MemberShipCode, Convert.ToDateTime(StartDate));
                DateTime MembersStartDate = Convert.ToDateTime(dates[0].ToString());
                DateTime MembersEndDate = Convert.ToDateTime(dates[1].ToString());
                double Duration = Convert.ToInt32(dates[2].ToString());
                DateTime CurrentDate = DateTime.Now;

                if (MembersEndDate > CurrentDate)
                {
                    CompleteDays = (CurrentDate - MembersStartDate).TotalDays;
                    LeftOutDays = (MembersEndDate - CurrentDate).TotalDays;
                    TotalDays = Duration * 30;
                }

                FinalAmount = Convert.ToDouble(Amount[0].ToString());
                IGST = Convert.ToDouble(Amount[1].ToString());

                if (LeftOutDays < 1)
                    LeftOutDays = 0;
                if (LeftOutDays > TotalDays)
                    LeftOutDays = TotalDays;
                if (CompleteDays < 1)
                    CompleteDays = 0;

                if (FinalAmount > 0 && IGST > 0)
                {
                    //10.5.2019
                    //FinalAmount1 = Convert.ToDouble(FinalAmount) - Convert.ToDouble(IGST);
                    FinalAmount1 = Convert.ToDouble(FinalAmount);
                    TransferfeePercentage = TransferPresentPercentage(MemberShipCode, CompleteDays);
                    //Transferfee = Convert.ToDouble(FinalAmount) * (TransferfeePercentage / 100);
                    Transferfee = 0;
                    FinalAmount2 = FinalAmount1 - Transferfee;
                    if (TotalDays > 0)
                    {
                        AmountPerDay = FinalAmount2 / TotalDays;
                        RemainingAmount = (AmountPerDay * LeftOutDays);
                    }
                }

                DataRow row;
                DataColumn col1 = new DataColumn("MFDID", typeof(int));
                DataColumn col2 = new DataColumn("Name", typeof(string));
                DataColumn col3 = new DataColumn("RemainingAmount", typeof(float));
                DataColumn col4 = new DataColumn("LeftOutDays", typeof(int));
                DataColumn col5 = new DataColumn("FinalAmount", typeof(float));
                DataColumn col6 = new DataColumn("TotalDays", typeof(int));
                DataColumn col7 = new DataColumn("CompletedDays", typeof(int));
                DataColumn col8 = new DataColumn("IsChangePackageUsed", typeof(bool));

                dt_Transfer.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8 });

                row = dt_Transfer.NewRow();
                row["MFDID"] = "4";
                row["Name"] = "Change Package";
                row["RemainingAmount"] = RemainingAmount;
                row["LeftOutDays"] = LeftOutDays;
                row["FinalAmount"] = FinalAmount;
                row["TotalDays"] = TotalDays;
                row["CompletedDays"] = CompleteDays;
                row["IsChangePackageUsed"] = IsChangePackageUsed;
                dt_Transfer.Rows.Add(row);

            }
            catch (Exception ec)
            {
                DataRow row;
                DataColumn col1 = new DataColumn("MFDID", typeof(int));
                DataColumn col2 = new DataColumn("Name", typeof(string));
                DataColumn col3 = new DataColumn("RemainingAmount", typeof(float));
                DataColumn col4 = new DataColumn("LeftOutDays", typeof(int));
                DataColumn col5 = new DataColumn("FinalAmount", typeof(float));
                DataColumn col6 = new DataColumn("TotalDays", typeof(int));
                DataColumn col7 = new DataColumn("CompletedDays", typeof(int));
                DataColumn col8 = new DataColumn("IsChangePackageUsed", typeof(bool));

                dt_Transfer.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8 });

                row = dt_Transfer.NewRow();
                row["MFDID"] = "4";
                row["Name"] = "Change Package";
                row["RemainingAmount"] = 0.0f;
                row["LeftOutDays"] = 0;
                row["FinalAmount"] = 0;
                row["TotalDays"] = 0;
                row["CompletedDays"] = 0;
                row["IsChangePackageUsed"] = IsChangePackageUsed;
                dt_Transfer.Rows.Add(row);


            }

            List<ChangeDetails> change = new List<ChangeDetails>();
            change.Add(new ChangeDetails { MFDID = Convert.ToInt32(dt_Transfer.Rows[0]["MFDID"].ToString()), Name = dt_Transfer.Rows[0]["Name"].ToString(), RemainingAmount = float.Parse(dt_Transfer.Rows[0]["RemainingAmount"].ToString()), LeftOutDays = Convert.ToInt32(dt_Transfer.Rows[0]["LeftOutDays"].ToString()), FinalAmount = float.Parse(dt_Transfer.Rows[0]["FinalAmount"].ToString()), TotalDays = Convert.ToInt32(dt_Transfer.Rows[0]["TotalDays"].ToString()), CompleteDays = Convert.ToInt32(dt_Transfer.Rows[0]["CompletedDays"].ToString()), IsChangeUsed = false });
            return change;
        }

        //Location Transfer Start
        // LocationTransfer Functions
        public double TransferPresentPercentage(string MembershipCode, double CompleteDays)
        {
            // int PresentDays = MembersAttendenceFromStartDate(MembershipCode, Dates);
            double TransferFee = 0.0f;
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet1 = new DataSet();

            DataTable dt_TransferPercentage = new DataTable();
            string result = string.Empty;

            string query1 = "select Days,Discount from MembersTransferFacilityDescription where IsActive=1";

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }
            if (ds_custdet1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i <= ds_custdet1.Tables[0].Rows.Count - 1; i++)
                {
                    if (Convert.ToInt32(ds_custdet1.Tables[0].Rows[i][0].ToString()) <= 10 && CompleteDays <= 10)
                    {
                        TransferFee = float.Parse(ds_custdet1.Tables[0].Rows[i][1].ToString());
                    }
                    else if (Convert.ToInt32(ds_custdet1.Tables[0].Rows[i][0].ToString()) >= 10 && Convert.ToInt32(ds_custdet1.Tables[0].Rows[i][0].ToString()) <= 20 && CompleteDays >= 10 && CompleteDays <= 20)
                    {

                        TransferFee = float.Parse(ds_custdet1.Tables[0].Rows[i][1].ToString());

                    }
                    else if (Convert.ToInt32(ds_custdet1.Tables[0].Rows[i][0].ToString()) >= 20 && Convert.ToInt32(ds_custdet1.Tables[0].Rows[i][0].ToString()) <= 30 && CompleteDays >= 20 && CompleteDays <= 30)
                    {

                        TransferFee = float.Parse(ds_custdet1.Tables[0].Rows[i][1].ToString());

                    }
                    else if (Convert.ToInt32(ds_custdet1.Tables[0].Rows[i][0].ToString()) < 30 && CompleteDays < 30)
                    {

                        TransferFee = float.Parse(ds_custdet1.Tables[0].Rows[i][1].ToString());

                    }
                    else
                    {
                        TransferFee = float.Parse(ds_custdet1.Tables[0].Rows[i][1].ToString());
                    }
                }
            }
            return TransferFee;
        }
        public ArrayList MembersLatestDates(string MembershipCode)
        {
            ArrayList Dates = new ArrayList();
            DataSet ds_Dates = new DataSet();
            //string query = "select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.StartDate <= GETDATE() and CCRM.MembershipCode = '" + MembershipCode + "' order by CCRM.ID desc";
            string query = "select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration,Type='Present'  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.StartDate <= GETDATE() and CCRM.MembershipExpireDate >=GETDATE() and CCRM.MembershipCode = '" + MembershipCode + "'  union all  select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration,Type='Future'  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.StartDate >= GETDATE() and CCRM.MembershipCode = '" + MembershipCode + "' union all  select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration,Type='Past'  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.MembershipExpireDate <= GETDATE()  and CCRM.MembershipCode = '" + MembershipCode + "' ";

            using (SqlDataAdapter da_ExpDates = new SqlDataAdapter(query, cnn))
            {

                da_ExpDates.Fill(ds_Dates);
                if (ds_Dates.Tables[0].Rows.Count > 0)
                {
                    Dates.Add(ds_Dates.Tables[0].Rows[0][0].ToString());
                    Dates.Add(ds_Dates.Tables[0].Rows[0][1].ToString());
                    Dates.Add(ds_Dates.Tables[0].Rows[0][3].ToString());
                }
                else
                {
                    Dates.Add("10/10/2020");
                    Dates.Add("10/10/2020");
                    Dates.Add("10/10/2020");
                }

            }



            return Dates;
        }
        public ArrayList MembersLatestAmount(string MembershipCode, DateTime StartDate)
        {
            ArrayList Amount = new ArrayList();
            try
            {

                //
                // string query = "select top 1 FinalAmount,IGST from FAInvoice where MembershipCode='" + MembershipCode + "'  order by ID desc";
                //string query = "select sum(PayableAmount+PayableAmount2) as FinalAmount from FAInvoice where MembershipCode='" + MembershipCode + "' and InvoiceID=(select top 1 InvoiceID from FAInvoice where MembershipCode='" + MembershipCode + "' order by ID desc)";
                //string query = "select (IsNull((select sum(FA.DuePaidAmount) as FinalAmount from FAInvoice FA,FAPaymentModes FAP where FAP.PayModeCode=FA.FAPaymentModes and InvoiceID=(select InvoiceID from FAInvoice where ID=(select MAX(ID) from FAInvoice where MembershipCode='" + MembershipCode + "'))  and DuePaidAmount<>0),0)  + (select sum(PayableAmount+PayableAmount2) as FinalAmount from FAInvoice where MembershipCode='" + MembershipCode + "' and InvoiceID=(select top 1 InvoiceID from FAInvoice where MembershipCode='" + MembershipCode + "' order by ID desc))) as FinalAmount";
                //Old
                //string query = "select (IsNull((select sum(FA.DuePaidAmount) as FinalAmount from FAInvoice FA,FAPaymentModes FAP where FAP.PayModeCode=FA.FAPaymentModes and InvoiceID=(select InvoiceID from FAInvoice where ID=(select MAX(ID) from FAInvoice where MembershipCode='" + MembershipCode + "'))  and DuePaidAmount<>0),0)  + ((select sum(PayableAmount+PayableAmount2) as FinalAmount from FAInvoice where MembershipCode='" + MembershipCode + "' and InvoiceID=(select top 1 InvoiceID from FAInvoice where MembershipCode='" + MembershipCode + "' order by ID desc)))) + (select (IsNull((select top 1 RemainingAmount as FinalAmount from CCRMMembershipFacility where MemberShipCode='" + MembershipCode + "' order by SNO desc),0)))  as FinalAmount";
                //new
                //string query = "select (IsNull((select sum(FA.DuePaidAmount) as FinalAmount from FAInvoice FA,FAPaymentModes FAP where FAP.PayModeCode=FA.FAPaymentModes and InvoiceID=(select InvoiceID from FAInvoice where InvoiceID=(select InvoiceID from CCRMMembership where MembershipCode='"+MembershipCode+"' and StartDate <= GETDATE() and MembershipExpireDate >=GETDATE()))  and DuePaidAmount<>0),0)  + ((select sum(PayableAmount+PayableAmount2) as FinalAmount from FAInvoice where MembershipCode='"+MembershipCode+"' and InvoiceID=(select InvoiceID from FAInvoice where InvoiceID=(select InvoiceID from CCRMMembership where MembershipCode='"+MembershipCode+"' and StartDate <= GETDATE() and MembershipExpireDate >=GETDATE()))))) + (select (IsNull((select top 1 RemainingAmount as FinalAmount from CCRMMembershipFacility where MemberShipCode='"+MembershipCode+"' order by SNO desc),0)))  as FinalAmount";
                //using Latest 10/10/2019  latest final
                //string query = "select (IsNull((select sum(FA.DuePaidAmount) as FinalAmount from FAInvoice FA,FAPaymentModes FAP where FAP.PayModeCode=FA.FAPaymentModes and InvoiceID=(select Top 1 InvoiceID from FAInvoice where InvoiceID=(select Top 1 InvoiceID from CCRMMembership where MembershipCode='" + MembershipCode + "' and StartDate <= GETDATE() order by ID desc ))  and DuePaidAmount<>0),0)  + ((select sum(PayableAmount+PayableAmount2) as FinalAmount from FAInvoice where MembershipCode='" + MembershipCode + "' and InvoiceID=(select Top 1 InvoiceID from FAInvoice where InvoiceID=(select top 1 InvoiceID from CCRMMembership where MembershipCode='" + MembershipCode + "' and StartDate <= GETDATE() order by ID desc ))))) + (select (IsNull((select top 1 RemainingAmount as FinalAmount from CCRMMembershipFacility where MemberShipCode='" + MembershipCode + "' order by SNO desc),0)))  as FinalAmount";
                //string query = "select (IsNull((select sum(FAD.DuePaidAmount) as FinalAmount from FADueInvoice FAD,FAPaymentModes FAP where FAP.PayModeCode=FAD.FAPaymentModes and InvoiceID=(select Top 1 InvoiceID from FAInvoice where InvoiceID=(select Top 1 InvoiceID from CCRMMembership where MembershipCode='" + MembershipCode + "' and  StartDate between '"+StartDate+"' and '"+StartDate+" 23:59:59' order by ID desc ))  and DuePaidAmount<>0),0)  + ((select sum(PayableAmount+PayableAmount2) as FinalAmount from FAInvoice where MembershipCode='" + MembershipCode + "' and InvoiceID=(select Top 1 InvoiceID from FAInvoice where InvoiceID=(select top 1 InvoiceID from CCRMMembership where MembershipCode='" + MembershipCode + "' and StartDate between '"+StartDate+"' and '"+StartDate+" 23:59:59' order by ID desc ))))) + (select (IsNull((select top 1 RemainingAmount as FinalAmount from CCRMMembershipFacility where MemberShipCode='" + MembershipCode + "' order by SNO desc),0)))  as FinalAmount";
                string query = "";
                if (StartDate > DateTime.Now)
                {
                    query = "select (IsNull((select sum(FAD.DuePaidAmount) as FinalAmount from FADueInvoice FAD,FAPaymentModes FAP where FAP.PayModeCode=FAD.FAPaymentModes and InvoiceID=(select Top 1 InvoiceID from FAInvoice where InvoiceID=(select Top 1 InvoiceID from CCRMMembership where MembershipCode='" + MembershipCode + "' and StartDate >= DATEADD(dd, 0, DATEDIFF(dd, -1, GETDATE())) order by ID desc ))  and DuePaidAmount<>0),0)  + ((select sum(PayableAmount+PayableAmount2) as FinalAmount from FAInvoice where MembershipCode='" + MembershipCode + "' and InvoiceID=(select Top 1 InvoiceID from FAInvoice where InvoiceID=(select top 1 InvoiceID from CCRMMembership where MembershipCode='" + MembershipCode + "' and StartDate >= DATEADD(dd, 0, DATEDIFF(dd, -1, GETDATE())) order by ID desc ))))) + (select (IsNull((select top 1 RemainingAmount as FinalAmount from FAInvoice where MemberShipCode='" + MembershipCode + "' order by ID desc),0))) + (select (IsNull((select top 1 Wallet as FinalAmount from FAInvoice where MemberShipCode='" + MembershipCode + "' order by ID desc),0))) as FinalAmount";
                }
                else if (StartDate < DateTime.Now)
                {
                    query = "select (IsNull((select sum(FAD.DuePaidAmount) as FinalAmount from FADueInvoice FAD,FAPaymentModes FAP where FAP.PayModeCode=FAD.FAPaymentModes and InvoiceID=(select Top 1 InvoiceID from FAInvoice where InvoiceID=(select Top 1 InvoiceID from CCRMMembership where MembershipCode='" + MembershipCode + "' and StartDate <= DATEADD(dd, 0, DATEDIFF(dd, -1, GETDATE())) order by ID desc ))  and DuePaidAmount<>0),0)  + ((select sum(PayableAmount+PayableAmount2) as FinalAmount from FAInvoice where MembershipCode='" + MembershipCode + "' and InvoiceID=(select Top 1 InvoiceID from FAInvoice where InvoiceID=(select top 1 InvoiceID from CCRMMembership where MembershipCode='" + MembershipCode + "' and StartDate <= DATEADD(dd, 0, DATEDIFF(dd, -1, GETDATE())) order by ID desc ))))) + (select (IsNull((select top 1 RemainingAmount as FinalAmount from FAInvoice where MemberShipCode='" + MembershipCode + "' order by ID desc),0))) + (select (IsNull((select top 1 Wallet as FinalAmount from FAInvoice where MemberShipCode='" + MembershipCode + "' order by ID desc),0))) as FinalAmount";

                }
                else
                {

                }

                using (SqlCommand cmd_LatestAmount = new SqlCommand(query, cnn))
                {
                    cnn.Open();
                    SqlDataReader LatestAmount_DR = cmd_LatestAmount.ExecuteReader();
                    if (LatestAmount_DR.Read())
                    {
                        Amount.Add(LatestAmount_DR[0].ToString());

                    }
                    cnn.Close();
                }

                string query1 = "select sum(IGST) as IGST from FAInvoice where MembershipCode='" + MembershipCode + "' and InvoiceID=(select top 1 InvoiceID from CCRMMembership where MembershipCode='" + MembershipCode + "' order by ID desc)";
                using (SqlCommand cmd_LatestAmount1 = new SqlCommand(query1, cnn))
                {
                    cnn.Open();
                    SqlDataReader LatestAmount_DR2 = cmd_LatestAmount1.ExecuteReader();
                    if (LatestAmount_DR2.Read())
                    {
                        Amount.Add(LatestAmount_DR2[0].ToString());

                    }
                    cnn.Close();
                }
            }
            catch (Exception ec)
            {

            }
            finally
            {
                cnn.Close();
            }
            return Amount;
        }
        public string GetMembersPromoCode(string MemberShipCode)
        {
            string PromoCode = "";
            try
            {
                cnn.Close();
                string query = "select  top 1 PromoCode from FAInvoice where MembershipCode='" + MemberShipCode + "' and PromoCode!='' order by ID desc";
                using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
                {
                    cnn.Open();
                    SqlDataReader GA_DR = cmd_SubEXT.ExecuteReader();
                    if (GA_DR.Read())
                    {
                        PromoCode = GA_DR[0].ToString();
                    }
                    cnn.Close();
                }
            }
            catch (Exception ec)
            {

            }
            finally
            {
                cnn.Close();
            }
            return PromoCode;

        }
        public DataTable GetMembersUsedFacilitiesDetails(string MemberShipCode, string BranchCode, string InvoiceID)
        {
            DataSet ds_custdet1 = new DataSet();
            string PromoCodes = "";
            string PromoCode = GetMembersPromoCode(MemberShipCode);
            DataTable dt_MemberShipFacilityDetails = new DataTable();
            DataRow row;
            DataColumn col1 = new DataColumn("SMFMID", typeof(string));
            DataColumn col2 = new DataColumn("SubSchemName", typeof(string));

            dt_MemberShipFacilityDetails.Columns.AddRange(new DataColumn[] { col1, col2 });

            string FacilityID = "";
            string FacilityName = "";
            string query = "select distinct MFDID  from CCRMMembershipFacility where MembershipCode='" + MemberShipCode + "' and InvoiceID='" + InvoiceID + "' and PlanCode=(select top 1 PlanCode from CCRMMembership where MembershipCode='" + MemberShipCode + "' order by ID desc) union all select distinct MFDID  from CCRMMembershipFacility where NewMembersCode='" + MemberShipCode + "' and InvoiceID='" + InvoiceID + "' and PlanCode=(select top 1 PlanCode from CCRMMembership where NewMembersCode='" + MemberShipCode + "' order by ID desc)";

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            if (ds_custdet1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i <= ds_custdet1.Tables[0].Rows.Count - 1; i++)
                {
                    if (i == 0)
                    {
                        PromoCodes = ds_custdet1.Tables[0].Rows[i][0].ToString();
                    }
                    else
                    {
                        PromoCodes = PromoCodes + "," + ds_custdet1.Tables[0].Rows[i][0].ToString();
                    }

                }
            }

            string[] str = PromoCodes.Split(',');

            for (int i = 0; i <= str.Length - 1; i++)
            {

                if (str[i].ToString() != "" || str[i].ToString() != string.Empty)
                {
                    string query1 = "select distinct SMFMID,SubSchemName from SubMembersFacilityMaster where SMFMID = " + str[i].ToString() + "";
                    using (SqlCommand cmd_FC = new SqlCommand(query1, cnn))
                    {
                        if (cnn.State == ConnectionState.Open)
                        {
                            cnn.Close();
                        }
                        cnn.Open();
                        SqlDataReader FC_DR = cmd_FC.ExecuteReader();
                        if (FC_DR.Read())
                        {
                            FacilityID = FC_DR[0].ToString();
                            FacilityName = FC_DR[1].ToString();
                        }
                        else
                        {
                            FacilityID = "";
                            FacilityName = "";
                        }
                        cnn.Close();
                    }

                    row = dt_MemberShipFacilityDetails.NewRow();
                    row["SMFMID"] = FacilityID;
                    row["SubSchemName"] = FacilityName;
                    dt_MemberShipFacilityDetails.Rows.Add(row);
                }
                else
                {
                    //FacilityID = "";
                    //FacilityName = "";
                    //row = dt_MemberShipFacilityDetails.NewRow();
                    //row["SMFMID"] = FacilityID;
                    //row["SubSchemName"] = FacilityName;
                    //dt_MemberShipFacilityDetails.Rows.Add(row);

                }
            }

            return dt_MemberShipFacilityDetails;
        }
        public List<LocationTransferDetails> LocationTransferFacility(string MemberShipCode, string StartDate)
        {
            double FinalAmount = 0.0f;
            double FinalAmount2 = 0.0f;
            double IGST = 0.0f;
            double FinalAmount1 = 0.0f;
            double TransferfeePercentage = 0.0f;
            double Transferfee = 0.0f;
            double AmountPerDay = 0.0f;
            double LeftOutDays = 0.0d;
            double CompleteDays = 0.0d;
            double TotalDays = 0.0d;
            double RemainingAmount = 0.0d;
            int IsTransferUsed = 0;

            string InvoicNumber = GetMembersInvoiceNumber(MemberShipCode, StartDate);

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet1 = new DataSet();

            DataTable dt_Transfer = new DataTable();

            string result = string.Empty;

            DataTable dt_UsedFacility = GetMembersUsedFacilitiesDetails(MemberShipCode, "", InvoicNumber);

            for (int i = 0; i <= dt_UsedFacility.Rows.Count - 1; i++)
            {
                try
                {
                    if (Convert.ToInt32(dt_UsedFacility.Rows[i][0].ToString()) == 5)
                    {
                        IsTransferUsed = 1;

                    }
                    else if (Convert.ToInt32(dt_UsedFacility.Rows[i][0].ToString()) == 6)
                    {
                        IsTransferUsed = 1;
                    }

                    else
                    {
                        IsTransferUsed = 0;
                    }
                }
                catch (Exception ec)
                {
                    IsTransferUsed = 0;
                }
            }

            ArrayList dates = MembersLatestDates(MemberShipCode);
            ArrayList Amount = MembersLatestAmount(MemberShipCode, Convert.ToDateTime(StartDate));

            DateTime MembersStartDate = Convert.ToDateTime(dates[0].ToString());
            DateTime MembersEndDate = Convert.ToDateTime(dates[1].ToString());
            double Duration = Convert.ToInt32(dates[2].ToString());
            DateTime CurrentDate = DateTime.Now;

            if (MembersEndDate > CurrentDate)
            {
                CompleteDays = (CurrentDate - MembersStartDate).TotalDays;
                LeftOutDays = (MembersEndDate - CurrentDate).TotalDays;
                TotalDays = Duration * 30;
            }

            FinalAmount = Convert.ToDouble(Amount[0].ToString());
            IGST = Convert.ToDouble(Amount[1].ToString());

            if (LeftOutDays < 1)
                LeftOutDays = 0;
            if (LeftOutDays > TotalDays)
                LeftOutDays = TotalDays;
            if (CompleteDays < 1)
                CompleteDays = 0;

            if (FinalAmount > 0 && IGST > 0)
            {
                //10.5.2019
                //FinalAmount1 = Convert.ToDouble(FinalAmount) - Convert.ToDouble(IGST);
                FinalAmount1 = Convert.ToDouble(FinalAmount);

                TransferfeePercentage = TransferPresentPercentage(MemberShipCode, CompleteDays);
                //Transferfee = Convert.ToDouble(FinalAmount) * (TransferfeePercentage / 100);
                Transferfee = 0;
                FinalAmount2 = FinalAmount1 - Transferfee;
                if (TotalDays > 0)
                {
                    AmountPerDay = FinalAmount2 / TotalDays;
                    RemainingAmount = (AmountPerDay * LeftOutDays);
                }
            }



            double cmpdays = 0;

            if (CompleteDays < 0)
                cmpdays = 0.0f;
            else
                cmpdays = CompleteDays;


            List<LocationTransferDetails> locationTransfer = new List<LocationTransferDetails>();
            locationTransfer.Add(new LocationTransferDetails
            {
                Name = "Location Transfer"
                      ,
                MFDID = 5
                    ,
                TransferFees = Transferfee
                      ,
                CompleteDays = cmpdays
                    ,
                LeftOutDays = LeftOutDays
                      ,
                TotalDays = TotalDays
                    ,
                FinalAmount = FinalAmount
                      ,
                RemainingAmount = RemainingAmount
                    ,
                IsTransferUsed = false
                    ,

            });

            return locationTransfer;
        }
        //Location Transfer End

        // PersonTransfer Functions
        public List<PersonTransferDetails> PersonTransferFacility(string MemberShipCode, string StartDate)
        {

            double FinalAmount = 0.0f;
            double IGST = 0.0f;
            double FinalAmount2 = 0.0f;
            double FinalAmount1 = 0.0f;
            double TransferfeePercentage = 0.0f;
            double Transferfee = 0.0f;
            double AmountPerDay = 0.0f;
            double LeftOutDays = 0.0d;
            double CompleteDays = 0.0d;
            double TotalDays = 0.0d;
            double RemainingAmount = 0.0d;
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet1 = new DataSet();
            int IsTransferUsed = 0;
            DataTable dt_Transfer = new DataTable();
            string result = string.Empty;

            string InvoicNumber = GetMembersInvoiceNumber(MemberShipCode, StartDate);

            try
            {

                DataTable dt_UsedFacility = GetMembersUsedFacilitiesDetails(MemberShipCode, "", InvoicNumber);

                for (int i = 0; i <= dt_UsedFacility.Rows.Count - 1; i++)
                {
                    try
                    {
                        if (Convert.ToInt32(dt_UsedFacility.Rows[i][0].ToString()) == 5)
                        {
                            IsTransferUsed = 1;

                        }
                        else if (Convert.ToInt32(dt_UsedFacility.Rows[i][0].ToString()) == 6)
                        {
                            IsTransferUsed = 1;
                        }

                        else
                        {
                            IsTransferUsed = 0;
                        }
                    }
                    catch (Exception ec)
                    {
                        IsTransferUsed = 0;
                    }
                }


                ArrayList dates = MembersLatestDates(MemberShipCode);
                ArrayList Amount = MembersLatestAmount(MemberShipCode, Convert.ToDateTime(StartDate));
                DateTime MembersStartDate = Convert.ToDateTime(dates[0].ToString());
                DateTime MembersEndDate = Convert.ToDateTime(dates[1].ToString());
                double Duration = Convert.ToInt32(dates[2].ToString());
                DateTime CurrentDate = DateTime.Now;

                if (MembersEndDate > CurrentDate)
                {
                    CompleteDays = (CurrentDate - MembersStartDate).TotalDays;
                    LeftOutDays = (MembersEndDate - CurrentDate).TotalDays;
                    TotalDays = Duration * 30;
                }

                FinalAmount = Convert.ToDouble(Amount[0].ToString());
                IGST = Convert.ToDouble(Amount[1].ToString());

                if (LeftOutDays < 1)
                    LeftOutDays = 0;
                if (LeftOutDays > TotalDays)
                    LeftOutDays = TotalDays;
                if (CompleteDays < 1)
                    CompleteDays = 0;



                if (FinalAmount > 0 && IGST > 0)
                {
                    //10.5.2019
                    //FinalAmount1 = Convert.ToDouble(FinalAmount) - Convert.ToDouble(IGST);
                    FinalAmount1 = Convert.ToDouble(FinalAmount);
                    TransferfeePercentage = TransferPresentPercentage(MemberShipCode, CompleteDays);
                    //Transferfee = Convert.ToDouble(FinalAmount) * (TransferfeePercentage / 100);
                    Transferfee = 0;
                    FinalAmount2 = FinalAmount1 - Transferfee;
                    if (TotalDays > 0)
                    {
                        AmountPerDay = FinalAmount2 / TotalDays;
                        RemainingAmount = (AmountPerDay * LeftOutDays);
                    }

                }

                DataRow row;
                DataColumn col1 = new DataColumn("MFDID", typeof(int));
                DataColumn col2 = new DataColumn("Name", typeof(string));
                DataColumn col3 = new DataColumn("TransferFees", typeof(float));
                DataColumn col4 = new DataColumn("RemainingAmount", typeof(float));
                DataColumn col5 = new DataColumn("LeftOutDays", typeof(int));
                DataColumn col6 = new DataColumn("FinalAmount", typeof(float));
                DataColumn col7 = new DataColumn("TotalDays", typeof(int));
                DataColumn col8 = new DataColumn("CompletedDays", typeof(int));
                DataColumn col9 = new DataColumn("IsTransferUsed", typeof(bool));

                dt_Transfer.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8, col9 });

                row = dt_Transfer.NewRow();
                row["MFDID"] = "6";
                row["Name"] = "Person Transfer";
                row["TransferFees"] = Transferfee;
                row["RemainingAmount"] = RemainingAmount;
                row["LeftOutDays"] = LeftOutDays;
                row["FinalAmount"] = FinalAmount;
                row["TotalDays"] = TotalDays;
                if (CompleteDays < 0)
                    row["CompletedDays"] = 0;
                else
                    row["CompletedDays"] = CompleteDays;
                row["IsTransferUsed"] = IsTransferUsed;
                dt_Transfer.Rows.Add(row);


            }
            catch (Exception ec)
            {
                DataRow row;
                DataColumn col1 = new DataColumn("MFDID", typeof(int));
                DataColumn col2 = new DataColumn("Name", typeof(string));
                DataColumn col3 = new DataColumn("TransferFees", typeof(float));
                DataColumn col4 = new DataColumn("RemainingAmount", typeof(float));
                DataColumn col5 = new DataColumn("LeftOutDays", typeof(int));
                DataColumn col6 = new DataColumn("FinalAmount", typeof(float));
                DataColumn col7 = new DataColumn("TotalDays", typeof(int));
                DataColumn col8 = new DataColumn("CompletedDays", typeof(int));
                DataColumn col9 = new DataColumn("IsTransferUsed", typeof(bool));

                dt_Transfer.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8, col9 });

                row = dt_Transfer.NewRow();
                row["MFDID"] = "6";
                row["Name"] = "Person Transfer";
                row["TransferFees"] = 0.0f;
                row["RemainingAmount"] = 0;
                row["LeftOutDays"] = 0;
                row["FinalAmount"] = 0;
                row["TotalDays"] = 0;
                row["CompletedDays"] = 0;
                row["IsTransferUsed"] = IsTransferUsed;
                dt_Transfer.Rows.Add(row);

            }

            List<PersonTransferDetails> PersonTransferDetails = new List<PersonTransferDetails>();
            PersonTransferDetails.Add(new PersonTransferDetails { MFDID = Convert.ToInt32(dt_Transfer.Rows[0]["MFDID"].ToString()), Name = dt_Transfer.Rows[0]["Name"].ToString(), TransferFees = float.Parse(dt_Transfer.Rows[0]["TransferFees"].ToString()), RemainingAmount = float.Parse(dt_Transfer.Rows[0]["RemainingAmount"].ToString()), LeftOutDays = Convert.ToInt32(dt_Transfer.Rows[0]["LeftOutDays"].ToString()), FinalAmount = float.Parse(dt_Transfer.Rows[0]["FinalAmount"].ToString()), TotalDays = Convert.ToInt32(dt_Transfer.Rows[0]["TotalDays"].ToString()), IsTransferUsed = false });
            return PersonTransferDetails;
        }

        // new logic to be implemented
        public List<GraceDetails> GraceFacility()
        {
            List<GraceDetails> grace = new List<GraceDetails>();
            grace.Add(new GraceDetails { });
            return grace;
        }
        public List<ExtendDetails> ExtendFacility()
        {
            List<ExtendDetails> Extend = new List<ExtendDetails>();
            Extend.Add(new ExtendDetails { });
            return Extend;
        }

        //Hold Start
        public int GetFacilitysHoldUtlized(string MemberShipCode, string Invoice)
        {
            int count = 0;

            string query = "select count(*) as UtlizedCount from CCRMMembershipFacility  where MemberShipCode='" + MemberShipCode + "' and InvoiceID='" + Invoice + "' and MFDID=8";
            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader GA_DR = cmd_SubEXT.ExecuteReader();
                if (GA_DR.Read())
                {
                    count = Convert.ToInt32(GA_DR[0].ToString());

                }
                cnn.Close();
            }
            return count;

        }
        public DataTable GetMembersLatestHold(string MemberShipCode)
        {
            // Note MFDID Freezing is 1 
            string HoldStartDate = string.Empty;
            string HoldEndDate = string.Empty;
            int NoOfOptions = 0;
            int OptionsUsed = 0;
            int NoOfOptionCurrentlyInUse = 0;


            string query = "select top 1 FacilityStartDate,FacilityExpireDate,NoOfOptions,OptionsUsed,NoOfOptionCurrentlyInUse from CCRMMembershipFacility where MemberShipCode='" + MemberShipCode + "' and MFDID=8 order by SNO desc";
            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader GA_DR = cmd_SubEXT.ExecuteReader();
                if (GA_DR.Read())
                {
                    HoldStartDate = GA_DR[0].ToString();
                    HoldEndDate = GA_DR[1].ToString();
                    NoOfOptions = Convert.ToInt32(GA_DR[2].ToString());
                    OptionsUsed = Convert.ToInt32(GA_DR[3].ToString());
                    NoOfOptionCurrentlyInUse = Convert.ToInt32(GA_DR[4].ToString());
                }

                cnn.Close();
            }

            DataTable dtFreezeDate = new DataTable();
            DataRow row;
            DataColumn col1 = new DataColumn("HoldStartDate", typeof(DateTime));
            DataColumn col2 = new DataColumn("HoldEndDate", typeof(DateTime));
            DataColumn col3 = new DataColumn("NoOfOptions", typeof(string));
            DataColumn col4 = new DataColumn("OptionsUsed", typeof(string));
            DataColumn col5 = new DataColumn("NoOfOptionCurrentlyInUse", typeof(string));

            dtFreezeDate.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5 });

            if (HoldStartDate != null && HoldEndDate != null)
            {
                if (HoldStartDate != "" && HoldEndDate != "" && NoOfOptions != 0)
                {
                    row = dtFreezeDate.NewRow();
                    row["HoldStartDate"] = HoldStartDate;
                    row["HoldEndDate"] = HoldEndDate;
                    row["NoOfOptions"] = NoOfOptions;
                    row["OptionsUsed"] = OptionsUsed;
                    row["NoOfOptionCurrentlyInUse"] = NoOfOptionCurrentlyInUse;
                    dtFreezeDate.Rows.Add(row);
                }
                else
                {
                    row = dtFreezeDate.NewRow();
                    row["HoldStartDate"] = DBNull.Value;
                    row["HoldEndDate"] = DBNull.Value;
                    row["NoOfOptions"] = "";
                    row["OptionsUsed"] = "";
                    row["NoOfOptionCurrentlyInUse"] = "";
                    dtFreezeDate.Rows.Add(row);
                }

            }
            else
            {
                row = dtFreezeDate.NewRow();
                row["HoldStartDate"] = DBNull.Value;
                row["HoldEndDate"] = DBNull.Value;
                row["NoOfOptions"] = "";
                row["OptionsUsed"] = "";
                row["NoOfOptionCurrentlyInUse"] = "";
                dtFreezeDate.Rows.Add(row);

            }
            return dtFreezeDate;

        }
        public List<HoldDetails> HoldFacility(string MemberShipCode, string StartDate)
        {
            List<HoldDetails> hold = new List<HoldDetails>();

            int FreezeCount = 0;
            int LatestPlansHold = GetMembersLatestPlansFreezingSchems(MemberShipCode);
            string InvoicNumber = GetMembersInvoiceNumber(MemberShipCode, StartDate);
            FreezeCount = GetFacilitysHoldUtlized(MemberShipCode, InvoicNumber);
            DataTable FreezingDates = GetMembersLatestHold(MemberShipCode);
            ArrayList arl = GetMembersExpireDate(MemberShipCode);
            DateTime ExpireDate = Convert.ToDateTime(arl[0].ToString());

            DataTable dtHold = new DataTable();
            DataTable dtHold1 = new DataTable();
            DataTable dtHold2 = new DataTable();

            string sd = FreezingDates.Rows[0][0].ToString();
            string ed = FreezingDates.Rows[0][1].ToString();

            if (sd != "" && ed != "" && sd != "1/1/1900 12:00:00 AM" && ed != "1/1/1900 12:00:00 AM")
            {
                dtHold1 = getdata(string.Format("select FF.NoOfDays,CCRMF.OptionsUsed as DaysUsed,CCRMF.NoOfOptions,CCRMF.FacilityStartDate,CCRMF.FacilityExpireDate from CCRMMembershipFacility CCRMF,CMSPLANCOST CMSPC,FreezingFacility FF where CCRMF.MFDID=1 and CMSPC.PlanCostCode=CCRMF.PlanCostCode and CMSPC.FreezingID=FF.FreezingID and CCRMF.MemberShipCode='{0}' and CCRMF.InvoiceID='{1}' and FacilityExpireDate between GETDATE() and '{2}' order by CCRMF.SNO desc", MemberShipCode, InvoicNumber, ExpireDate));
                if (dtHold1.Rows.Count > 0)
                {
                    dtHold = dtHold1;
                }
                else
                {
                    dtHold2 = getdata(string.Format("select CCRMF.SMFMID,FF.FreezingName as Name,Frequency=0,CCRMF.NoOfOptions,FF.NoOfDays,CCRMF.OptionsUsed as DaysUsed,CCRMF.FacilityStartDate,CCRMF.FacilityExpireDate from CCRMMembershipFacility CCRMF,CMSPLANCOST CMSPC,FreezingFacility FF where CCRMF.MFDID=1 and CMSPC.PlanCostCode=CCRMF.PlanCostCode and CMSPC.FreezingID=FF.FreezingID and CCRMF.MemberShipCode='{0}' and CCRMF.InvoiceID='{1}' and FacilityExpireDate between GETDATE() and '10/07/2020' order by CCRMF.SNO desc", MemberShipCode, InvoicNumber, ExpireDate));
                    dtHold = dtHold2;
                }

            }
            // if datess empty
            else
            {

                dtHold1 = getdata(string.Format("select top 1  CCRMF.SMFMID,Name='',CCRMF.OptionsUsed,MFF.Frequency,CCRMF.NoOfOptions,CCRMF.FacilityStartDate,CCRMF.FacilityExpireDate,CCRMF.NoOfOptionCurrentlyInUse from CCRMMembershipFacility CCRMF,CMSPLANCOST CMSPC,MembersFacilityFreezing MFF where CCRMF.MFDID=1 and CMSPC.PlanCostCode=CCRMF.PlanCostCode and CMSPC.FreezingID=MFF.FreezingID and CCRMF.MemberShipCode='{0}' and FacilityStartDate='' and FacilityExpireDate='' order by CCRMF.SNO desc", MemberShipCode));

                if (dtHold1.Rows.Count > 0)
                {
                    dtHold = dtHold1;
                }
                else
                {
                    dtHold2 = getdata(string.Format("select top 1 MFF.SMFMID as MFDID,SMFM.SubSchemName as Name,Frequency,NoOfOptions,OptionsUsed=0,FacilityStartDate='01/01/1900',FacilityExpireDate='01/01/1900' from MembersFacilityFreezing MFF,SubMembersFacilityMaster SMFM where MFF.FreezingID={0} and MFF.SMFMID=SMFM.SMFMID", LatestPlansHold));
                    dtHold = dtHold2;
                }
            }

            for (int i = 0; i < dtHold.Rows.Count; i++)
            {
                hold.Add(new HoldDetails
                {
                    Name = "Hold"
                      ,
                    MFDID = 8
                    ,
                    HoldStartDate = Convert.ToDateTime(dtHold.Rows[i]["FacilityStartDate"])
                      ,
                    HoldEndDate = Convert.ToDateTime(dtHold.Rows[i]["FacilityExpireDate"])
                    ,
                    Freequency = Convert.ToInt32(dtHold.Rows[i]["Frequency"])
                      ,
                    NoOfOptions = Convert.ToInt32(dtHold.Rows[i]["NoOfOptions"])
                    ,
                    NoOfOptionsCurrentlyInUse = Convert.ToInt32(dtHold.Rows[i]["OptionsUsed"])
                      ,
                    RemainingDays = 10
                    ,
                    IsHoldUsed = false
                    ,

                });

            }

            return hold;
        }

        //Hold End
        /// <summary>
        /// AllTransferTogether
        /// </summary>
        /// <param name="nmt"></param>
        /// <returns></returns>

        public string GetMembersExistance(string MobileNo)
        {
            string MembershipCode = "";
            string query = "select top 1 UCode from Users where MobileNo='" + MobileNo + "' and MobileNo<>'' order by ID desc";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_pdetails = cmd_SubEXT.ExecuteReader();
                if (DR_pdetails.Read())
                {
                    MembershipCode = DR_pdetails[0].ToString();

                }
                cnn.Close();
            }
            return MembershipCode;
        }

        public ArrayList MembersLatestDatesByInvoice(string InvoiceId)
        {
            DateTime dt_time = new DateTime();
            ArrayList Dates = new ArrayList();
            DataSet ds_Dates = new DataSet();
            DataSet ds_Facilityopts = new DataSet();
            //string query = "select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.StartDate <= GETDATE() and CCRM.MembershipCode = '" + MembershipCode + "' order by CCRM.ID desc";
            string query = "select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration,Type='Present',CCRM.InvoiceID  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.StartDate <= GETDATE() and CCRM.MembershipExpireDate >=GETDATE() and CCRM.InvoiceID = '" + InvoiceId + "'  union all  select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration,Type='Future',CCRM.InvoiceID  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.StartDate >= GETDATE() and CCRM.InvoiceID = '" + InvoiceId + "' union all  select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration,Type='Past',CCRM.InvoiceID  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.MembershipExpireDate <= GETDATE()  and CCRM.InvoiceID = '" + InvoiceId + "' ";
            using (SqlDataAdapter da_ExpDates = new SqlDataAdapter(query, cnn))
            {
                da_ExpDates.Fill(ds_Dates);
                //Dates.Add(ds_Dates.Tables[0].Rows[0][0].ToString());
                //Dates.Add(ds_Dates.Tables[0].Rows[0][1].ToString());
                //Dates.Add(ds_Dates.Tables[0].Rows[0][3].ToString());
            }

            try
            {
                string query1 = "select Days from CCRMMembershipFacility where InvoiceID='" + ds_Dates.Tables[0].Rows[0]["InvoiceID"].ToString() + "' and MFDID=7  ";
                using (SqlDataAdapter da_mfdid = new SqlDataAdapter(query1, cnn))
                {
                    da_mfdid.Fill(ds_Facilityopts);
                }


                if (Convert.ToInt32(ds_Facilityopts.Tables[0].Rows[0]["Days"].ToString()) > 0)
                {
                    dt_time = Convert.ToDateTime(ds_Dates.Tables[0].Rows[0][1].ToString()).AddDays(-Convert.ToInt32(ds_Facilityopts.Tables[0].Rows[0]["Days"].ToString()));
                    Dates.Add(ds_Dates.Tables[0].Rows[0][0].ToString());
                    Dates.Add(dt_time);
                    Dates.Add(ds_Dates.Tables[0].Rows[0][3].ToString());
                }
                else
                {
                    Dates.Add(ds_Dates.Tables[0].Rows[0][0].ToString());
                    Dates.Add(ds_Dates.Tables[0].Rows[0][1].ToString());
                    Dates.Add(ds_Dates.Tables[0].Rows[0][3].ToString());
                }
            }
            catch (Exception ecp)
            {
                Dates.Add(ds_Dates.Tables[0].Rows[0][0].ToString());
                Dates.Add(ds_Dates.Tables[0].Rows[0][1].ToString());
                Dates.Add(ds_Dates.Tables[0].Rows[0][3].ToString());
            }

            return Dates;
        }
        public ArrayList MembersLatestDatesByInvoice123(string InvoiceId)
        {
            ArrayList Dates = new ArrayList();
            DataSet ds_Dates = new DataSet();
            string query = "select top 1 CONVERT(date, CCRM.StartDate) as StartDate,CONVERT(date, CCRM.MembershipExpireDate) as MembershipExpireDate,CCRM.DurationId,CMSD.Duration,Type='Present'  from CCRMMembership CCRM,CMSDURATION CMSD where CCRM.DurationId=CMSD.DurationID and  CCRM.StartDate <= GETDATE() and CCRM.MembershipExpireDate >=GETDATE() and CCRM.InvoiceID = '" + InvoiceId + "'   ";

            using (SqlDataAdapter da_ExpDates = new SqlDataAdapter(query, cnn))
            {

                da_ExpDates.Fill(ds_Dates);
                if (ds_Dates.Tables[0].Rows.Count > 0)
                {
                    Dates.Add(ds_Dates.Tables[0].Rows[0][0].ToString());
                    Dates.Add(ds_Dates.Tables[0].Rows[0][1].ToString());
                    Dates.Add(ds_Dates.Tables[0].Rows[0][3].ToString());
                }
                else
                {
                    Dates.Add("10/10/2020");
                    Dates.Add("10/10/2020");
                    Dates.Add("10/10/2020");
                }

            }



            return Dates;
        }
        public ArrayList MembersLatestAmountByInvoice(string InvoiceId)
        {
            ArrayList Amount = new ArrayList();
            try
            {
                string query = "";
                //if (StartDate > DateTime.Now)
                //{
                //query = "select (IsNull((select sum(FAD.DuePaidAmount) as FinalAmount from FADueInvoice FAD,FAPaymentModes FAP where FAP.PayModeCode=FAD.FAPaymentModes and InvoiceID=(select Top 1 InvoiceID from FAInvoice where InvoiceID=(select Top 1 InvoiceID from CCRMMembership where MembershipCode='" + MembershipCode + "' and StartDate >= DATEADD(dd, 0, DATEDIFF(dd, -1, GETDATE())) order by ID desc ))  and DuePaidAmount<>0),0)  + ((select sum(PayableAmount+PayableAmount2) as FinalAmount from FAInvoice where MembershipCode='" + MembershipCode + "' and InvoiceID=(select Top 1 InvoiceID from FAInvoice where InvoiceID=(select top 1 InvoiceID from CCRMMembership where MembershipCode='" + MembershipCode + "' and StartDate >= DATEADD(dd, 0, DATEDIFF(dd, -1, GETDATE())) order by ID desc ))))) + (select (IsNull((select top 1 RemainingAmount as FinalAmount from FAInvoice where MemberShipCode='" + MembershipCode + "' order by ID desc),0))) + (select (IsNull((select top 1 Wallet as FinalAmount from FAInvoice where MemberShipCode='" + MembershipCode + "' order by ID desc),0))) as FinalAmount";
                query = "select (IsNull((select sum(FAD.DuePaidAmount) as FinalAmount from FADueInvoice FAD,FAPaymentModes FAP where FAP.PayModeCode=FAD.FAPaymentModes and InvoiceID='" + InvoiceId + "'  and DuePaidAmount<>0),0)  + ((select sum(PayableAmount+PayableAmount2) as FinalAmount from FAInvoice where InvoiceID='" + InvoiceId + "')))+(select (IsNull((select top 1 RemainingAmount as FinalAmount from FAInvoice where InvoiceID='" + InvoiceId + "' order by ID desc),0)))+(select (IsNull((select top 1 Wallet as FinalAmount from FAInvoice where InvoiceID='" + InvoiceId + "' order by ID desc),0))) as FinalAmount";
                //}
                //else if (StartDate < DateTime.Now)
                //{
                //    // query = "select (IsNull((select sum(FAD.DuePaidAmount) as FinalAmount from FADueInvoice FAD,FAPaymentModes FAP where FAP.PayModeCode=FAD.FAPaymentModes and InvoiceID=(select Top 1 InvoiceID from FAInvoice where InvoiceID=(select Top 1 InvoiceID from CCRMMembership where MembershipCode='" + MembershipCode + "' and StartDate <= DATEADD(dd, 0, DATEDIFF(dd, -1, GETDATE())) order by ID desc ))  and DuePaidAmount<>0),0)  + ((select sum(PayableAmount+PayableAmount2) as FinalAmount from FAInvoice where MembershipCode='" + MembershipCode + "' and InvoiceID=(select Top 1 InvoiceID from FAInvoice where InvoiceID=(select top 1 InvoiceID from CCRMMembership where MembershipCode='" + MembershipCode + "' and StartDate <= DATEADD(dd, 0, DATEDIFF(dd, -1, GETDATE())) order by ID desc ))))) + (select (IsNull((select top 1 RemainingAmount as FinalAmount from FAInvoice where MemberShipCode='" + MembershipCode + "' order by ID desc),0))) + (select (IsNull((select top 1 Wallet as FinalAmount from FAInvoice where MemberShipCode='" + MembershipCode + "' order by ID desc),0))) as FinalAmount";
                //    query = "";
                //}
                //else
                //{

                //}

                using (SqlCommand cmd_LatestAmount = new SqlCommand(query, cnn))
                {
                    cnn.Open();
                    SqlDataReader LatestAmount_DR = cmd_LatestAmount.ExecuteReader();
                    if (LatestAmount_DR.Read())
                    {
                        Amount.Add(LatestAmount_DR[0].ToString());

                    }
                    cnn.Close();
                }

                //string query1 = "select sum(IGST) as IGST from FAInvoice where MembershipCode='" + MembershipCode + "' and InvoiceID=(select top 1 InvoiceID from CCRMMembership where MembershipCode='" + MembershipCode + "' order by ID desc)";
                string query1 = "select sum(IGST) as IGST from FAInvoice where InvoiceID='" + InvoiceId + "' ";
                using (SqlCommand cmd_LatestAmount1 = new SqlCommand(query1, cnn))
                {
                    cnn.Open();
                    SqlDataReader LatestAmount_DR2 = cmd_LatestAmount1.ExecuteReader();
                    if (LatestAmount_DR2.Read())
                    {
                        Amount.Add(LatestAmount_DR2[0].ToString());

                    }
                    cnn.Close();
                }
            }
            catch (Exception ec)
            {

            }
            finally
            {
                cnn.Close();
            }
            return Amount;
        }

        //Get server side details coding
        public Object Transfer([FromBody]Transfer tfr)
        {
            // ToBranchCode , FromPackageCode , FromPackageName   add in insert statements
            //{"MembershipExpireDate":"","BranchCode":"","PlanCode":"","SlotCode":"","PackageCode":"","InvoiceID":"","PlanCostCode":"","DurationCode":"","FromMembership":"2010005102","FromMobileNo":"9676600302","FromInvoice":"N7416065155","ToMembership":"","ToMobileNo":"9676600301","PackageStartDate":"","CreatedBy":"Dilip","Comments":"New Registration"}
            double FinalAmount = 0.0f;
            double FinalAmount2 = 0.0f;
            double IGST = 0.0f;
            double FinalAmount1 = 0.0f;
            double TransferfeePercentage = 0.0f;
            double Transferfee = 0.0f;
            double AmountPerDay = 0.0f;
            double LeftOutDays = 0.0d;
            double TransferLeftOutDays = 0.0d;
            double CompleteDays = 0.0d;
            double TotalDays = 0.0d;
            double RemainingAmount = 0.0d;


            FacilityOutputStatus ffopt = new FacilityOutputStatus();

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int Year = DateTime.Now.Year - 2000;
            int Month = DateTime.Now.Month;
            string r = "";
            // Clients Unique COde
            ArrayList al = new ArrayList();


            //TrainerCode,MFIDID,RemainingAmount,EnquireTypeIncentive
            //MFDID,LeftOutDays,LeftOutAmount,NewMembershipCode,NewInvoiceID

            DataTable dt_PEDetails = new DataTable();
            dt_PEDetails = getdata(string.Format("select BranchCode,EnquirePersonFirstName as FirstName,EnquirePersonLastName as LastName,Gender,Email,D_O_B,Address from CCRMMEnquireForm where MobileNo = '{0}'", tfr.ToMobileNo));

            DataTable dt_MDetails = new DataTable();
            dt_MDetails = getdata(string.Format("select top 1 FAI.MembershipCode,FAI.InvoiceID,CCRMM.BranchCode,CMSSS.SessionCode,CMSS.SessionName,CCRMM.SlotCode,CMSSS.SlotName,CCRMM.PackageCode,CMSPAC.PackageName,CCRMM.PlanCode,CMSP.PlanName,CMSPAC.PlanCostCode,CCRMM.DurationId,CCRMM.EnquireTypeNo,CCRMM.MembershipExpireDate  from FAInvoice FAI, CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,CMSPACKAGESCOST CMSPAC,CMSSESSIONTIMESETTING CMSS,CMSPLAN CMSP where CCRMM.PlanCode=CMSP.PlanCode and CMSS.SessionCode=CMSSS.SessionCode and CCRMM.SlotCode=CMSSS.SlotCode and CCRMM.PackageCode=CMSPAC.PackageCode and  FAI.InvoiceID ='{0}' ", tfr.FromInvoice));

            DataTable dt_Invoice = new DataTable();
            dt_Invoice = getdata(string.Format("select Top 1 GSTCode,FAPaymentModes,FAPaymentModes2,PayableAmount,PayableAmount2,GSTCode,IGST,CGST,SGST,GymFees,FinalAmount,DiscountAmount,TrainerCode,Wallet,PaymentDate,AmountDue,TrainerFees,IGSTableAmount,PromoCode,SlotPrice,PlanCost from  FAInvoice where InvoiceID='{0}' order by ID desc", tfr.FromInvoice));

            DataTable dt_PackageName = new DataTable();
            dt_PackageName = getdata(string.Format("select PackageName from CMSPACKAGES where PackageCode='{0}' order by ID desc ", dt_MDetails.Rows[0]["PackageCode"].ToString()));

            DataTable dt_CMSPC = new DataTable();
            dt_CMSPC = getdata(string.Format("select  CMSPC.PackageCode,CMSPC.PackageName,CMSPC.PlanCode,CMSPC.PlanCostCode from CMSPACKAGESCOST CMSPC,CMSPACKAGES CMSP where CMSPC.PackageCode=CMSP.PackageCode and  CMSPC.PackageName='{0}' and CMSPC.DurationId='{1}' and CMSPC.BranchCode='{2}' and CMSPC.SlotCode='{3}'", dt_PackageName.Rows[0]["PackageName"].ToString(), dt_MDetails.Rows[0]["DurationId"].ToString(), tfr.ToBranchCode, tfr.ToSlotCode));

            

            int SerialNo = 0;
            string MembershipCode = GetMembersExistance(tfr.ToMobileNo);

            if (MembershipCode == null || MembershipCode == "" || MembershipCode == string.Empty)
            {
                al = MCode(dt_MDetails.Rows[0]["BranchCode"].ToString());
                r = (String)al[1].ToString();
                SerialNo = Convert.ToInt32(al[0].ToString());
            }
            else
            {
                r = MembershipCode;
            }

            TransferBit(tfr.FromMemberShipCode, Convert.ToInt32(tfr.FromMFDID));

            string Invoice = "N" + UniqueGeneration2();

            if (InvoiceCheck(Invoice) != 0)
            {
                Invoice = "N" + UniqueGeneration2();
            }

            int generation = ReceiptGeneration();

            string Invoice2 = GetMembersInvoiceNumberForFacilityPosts(dt_MDetails.Rows[0]["BranchCode"].ToString());
            string ReceiptNo = Convert.ToString(ReceiptGeneration());

            string PhotoUrl = GetPhotoUrl(tfr.Photo, r);
            DataTable mdetails = new DataTable();

            ArrayList dates = MembersLatestDatesByInvoice(tfr.FromInvoice);
            DateTime MembersStartDate = Convert.ToDateTime(dates[0].ToString());
            DateTime MembersEndDate = Convert.ToDateTime(dates[1].ToString());
            DateTime MembersEndDate2 = Convert.ToDateTime(dates[1].ToString());
            double Duration = Convert.ToInt32(dates[2].ToString());
            ArrayList Amount = MembersLatestAmountByInvoice(dt_MDetails.Rows[0]["InvoiceID"].ToString());

            DateTime CurrentDate = DateTime.Now;

            if (MembersEndDate > CurrentDate)
            {
                CompleteDays = (CurrentDate - MembersStartDate).TotalDays;
                TransferLeftOutDays = (MembersEndDate2 - CurrentDate).TotalDays;
                LeftOutDays = (MembersEndDate - CurrentDate).TotalDays;
                TotalDays = Duration * 30;
            }

            FinalAmount = Convert.ToDouble(Amount[0].ToString());
            IGST = Convert.ToDouble(Amount[1].ToString());


            if (LeftOutDays < 1)
                LeftOutDays = 0;
            if (LeftOutDays > TotalDays)
                LeftOutDays = TransferLeftOutDays;
            if (CompleteDays < 1)
                CompleteDays = 0;



            if (FinalAmount > 0 && IGST > 0)
            {
                //10.5.2019
                //FinalAmount1 = Convert.ToDouble(FinalAmount) - Convert.ToDouble(IGST);
                FinalAmount1 = Convert.ToDouble(FinalAmount);
                TransferfeePercentage = TransferPresentPercentage(tfr.MembershipCode, CompleteDays);
                //Transferfee = Convert.ToDouble(FinalAmount) * (TransferfeePercentage / 100);
                Transferfee = 0;
                FinalAmount2 = FinalAmount1 - Transferfee;
                if (TotalDays > 0)
                {
                    AmountPerDay = Math.Round(FinalAmount2 / TotalDays);
                    RemainingAmount = Math.Round((AmountPerDay * LeftOutDays));
                }
            }



            cnn.Close();
            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            // Start a local transaction.
            transaction = cnn.BeginTransaction("SampleTransaction");
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = cnn;
            command.Transaction = transaction;
            
            try
            {
                //
                if (MembershipCode == null || MembershipCode == "" || MembershipCode == string.Empty)
                {
                    command.CommandText = "insert into Users(UCode,BranchCode,UserName,Firstname,Lastname,DateOfBirth,Gender,MaritialStatus,Address,Area,City,State,Country,PinCode,MobileNo,CreatedBy,CreatedOn,IsDeleted,IsActive,Photo,Email) values('" + r + "','" + dt_PEDetails.Rows[0]["BranchCode"].ToString() + "','" + dt_PEDetails.Rows[0]["FirstName"].ToString() + "','" + dt_PEDetails.Rows[0]["FirstName"].ToString() + "','" + dt_PEDetails.Rows[0]["LastName"].ToString() + "','" + dt_PEDetails.Rows[0]["D_O_B"].ToString() + "','" + dt_PEDetails.Rows[0]["Gender"].ToString() + "','','" + dt_PEDetails.Rows[0]["Address"].ToString() + "','','','','','','" + tfr.MobileNo + "','" + tfr.CreatedBy + "','" + ServerDateTime + "',0,1,'','" + dt_PEDetails.Rows[0]["Email"].ToString() + "')";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into Login(UCode,UserName,PaswordSalt,PasswordHash,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "','" + dt_PEDetails.Rows[0]["FirstName"].ToString() + "','','','" + tfr.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into CCRMMembershipCodification(SerialNo,MembershipCode,BranchCode,Year,Month) values('" + al[0].ToString() + "','" + r + "','" + dt_PEDetails.Rows[0]["FirstName"].ToString() + "','" + Year + "','" + Month + "')";
                    command.ExecuteNonQuery();
                }
                //To Information
                command.CommandText = "insert into CCRMMembership(MembershipCode,BranchCode,PlanCode,MembershipExpireDate,CreatedBy,CreatedOn,IsDeleted,IsActive,SlotCode,PackageCode,StartDate,TrainerCode,InvoiceID,Receipt,PlanCostCode,DurationId,EnquireTypeNo,EnquireTypeIncentives) values('" + r + "','" + tfr.ToBranchCode + "','" + dt_CMSPC.Rows[0]["PlanCode"].ToString() + "','" + MembersEndDate + "','" + tfr.CreatedBy + "','" + ServerDateTime + "',0,1,'" + tfr.ToSlotCode + "','" + dt_CMSPC.Rows[0]["PackageCode"].ToString() + "','" + tfr.PackageStartDate + "','" + tfr.ToTrainerCode + "','" + Invoice + "','" + generation + "','" + dt_CMSPC.Rows[0]["PlanCostCode"].ToString() + "','" + dt_MDetails.Rows[0]["DurationId"].ToString() + "','" + dt_MDetails.Rows[0]["EnquireTypeNo"].ToString() + "','" + dt_MDetails.Rows[0]["EnquireTypeNo"].ToString() + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,FacilityStartDate,FacilityExpireDate,PackageCode,PackageName,SessionCode,SessionName,PlanCode,PlanName,PlanAmount,SlotCode,SlotName,LeftOutDays,LeftOutAmount,CreatedBy,CreatedOn,IsDeleted,IsActive,BranchCode,IsTransfer,NewMembersCode,InvoiceID,NewInvoiceID) values('" + r + "','" + tfr.FromMFDID + "','','','" + dt_MDetails.Rows[0]["PackageCode"].ToString() + "','" + dt_PackageName.Rows[0]["PackageName"].ToString() + "','" + dt_MDetails.Rows[0]["SessionCode"].ToString() + "','" + dt_MDetails.Rows[0]["SessionName"].ToString() + "','" + dt_MDetails.Rows[0]["PlanCode"].ToString() + "','" + dt_MDetails.Rows[0]["PlanName"].ToString() + "','" + dt_Invoice.Rows[0]["PlanCost"].ToString() + "','" + dt_MDetails.Rows[0]["SlotCode"].ToString() + "','" + dt_MDetails.Rows[0]["SlotName"].ToString() + "','" + Math.Round(LeftOutDays) + "','" + RemainingAmount + "','" + tfr.CreatedBy + "','" + ServerDateTime + "',0,1,'" + dt_MDetails.Rows[0]["BranchCode"].ToString() + "','1','','" + Invoice + "','" + Invoice2 + "')";
                command.ExecuteNonQuery();
                //To Information
                command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,FacilityStartDate,FacilityExpireDate,PackageCode,PackageName,SessionCode,SessionName,PlanCode,PlanName,PlanAmount,SlotCode,SlotName,LeftOutDays,LeftOutAmount,CreatedBy,CreatedOn,IsDeleted,IsActive,BranchCode,IsTransfer,NewMembersCode,InvoiceID,NewInvoiceID,PreviousExpiryDate) values('" + dt_MDetails.Rows[0]["MembershipCode"].ToString() + "','" + tfr.FromMFDID + "','','','" + dt_CMSPC.Rows[0]["PackageCode"].ToString() + "','" + dt_PackageName.Rows[0]["PackageName"].ToString() + "','" + dt_MDetails.Rows[0]["SessionCode"].ToString() + "','" + dt_MDetails.Rows[0]["SessionName"].ToString() + "','" + dt_CMSPC.Rows[0]["PlanCode"].ToString() + "','" + dt_MDetails.Rows[0]["PlanName"].ToString() + "','" + dt_Invoice.Rows[0]["PlanCost"].ToString() + "','" + dt_MDetails.Rows[0]["SlotCode"].ToString() + "','" + dt_MDetails.Rows[0]["SlotName"].ToString() + "','" + Math.Round(LeftOutDays) + "','" + RemainingAmount + "','" + tfr.CreatedBy + "','" + ServerDateTime + "',0,1,'" + dt_MDetails.Rows[0]["BranchCode"].ToString() + "','1','','" + Invoice + "','" + Invoice2 + "','"+ dt_MDetails.Rows[0]["MembershipExpireDate"].ToString() + "')";
                command.ExecuteNonQuery();
                //To Information
                command.CommandText = "insert into FAInvoice(InvoiceID,GSTCode,FAPaymentModes,MembershipCode,PayableAmount,AmountDue,GymFees,TrainerFees,IGSTableAmount,IGST,CGST,SGST,CreatedBy,CreatedOn,IsDeleted,IsActive,FinalAmount,FAPaymentModes2,PayableAmount2,PromoCode,DiscountAmount,TrainerCode,Receipt,EnquireTypeNo,SlotPrice,PlanCost,PaymentDate,RemainingAmount,Wallet,Recouncelled,Matchedid,SMFMID,OrderId,Currency) values('" + Invoice + "','" + dt_Invoice.Rows[0]["GSTCode"].ToString() + "','" + dt_Invoice.Rows[0]["FAPaymentModes"].ToString() + "','" + r + "','0','0','0','0','0','0','0','0','" + tfr.CreatedBy + "','" + ServerDateTime + "',0,1,'" + dt_Invoice.Rows[0]["FinalAmount"].ToString() + "','','" + dt_Invoice.Rows[0]["FAPaymentModes2"].ToString() + "','" + dt_Invoice.Rows[0]["PromoCode"].ToString() + "','" + dt_Invoice.Rows[0]["DiscountAmount"].ToString() + "','" + tfr.ToTrainerCode + "','" + generation + "','" + dt_MDetails.Rows[0]["EnquireTypeNo"].ToString() + "','" + dt_Invoice.Rows[0]["SlotPrice"].ToString() + "','" + dt_Invoice.Rows[0]["PlanCost"].ToString() + "','" + dt_Invoice.Rows[0]["PaymentDate"].ToString() + "','" + RemainingAmount + "','" + dt_Invoice.Rows[0]["Wallet"].ToString() + "','','','"+tfr.FromMFDID+"','"+Invoice+"','INR')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into Remainders(MembershipCode,RemainderCode,RemainderName,FAInvoice,Amount,Date,CreatedBy,CreatedOn) values('" + dt_PEDetails.Rows[0]["BranchCode"].ToString() + "','','','" + Invoice + "','" + FinalAmount + "','" + tfr.ToDueDate + "','" + tfr.CreatedBy + "','" + ServerDateTime + "')";
                command.ExecuteNonQuery();
                //From Information
                command.CommandText = "update CCRMMembership set MembershipExpireDate='" + ServerDateTime + "' where InvoiceID='" + tfr.FromInvoice + "' ";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMEnquireStatus(MobileNo,EnquireTypeNo,CreatedBy,CreatedOn,IsDeleted,IsActive)  values('" + tfr.ToMobileNo + "',3,'" + tfr.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                if (tfr.Comments != "")
                {
                    command.CommandText = "insert into CommentsTracker(UCode,MasterID,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',0,'" + tfr.Comments + "','" + tfr.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
                cnn.Close();
                ffopt.status = "success";
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (Exception ex2)
                {

                }
                ffopt.status = "fail";
            }
            string sJSONResponse = JsonConvert.SerializeObject(ffopt);

            return sJSONResponse;
        }
        public Object Hold([FromBody]HoldInput hi)
        {
            FacilityOutputStatus ffopt = new FacilityOutputStatus();
            int SNO = 0;
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet = new DataSet();
            ArrayList arl = new ArrayList();
            ArrayList arlMembers = new ArrayList();
            arl = MembersLatestDates(hi.MemberShipCode);

            DateTime startdate = Convert.ToDateTime(arl[0].ToString());

            DataTable dt_GetMembersPlanDetails = new DataTable();
            DataTable dt_GetMembersPersonalDetails = new DataTable();
            DataTable dt_GetWalletCalculation = new DataTable();
            DataTable dt_FAInvoiceLatestRecord = new DataTable();

            dt_GetWalletCalculation.Clear();

            arlMembers = GetMembersBasicPlanDetails(hi.MemberShipCode, startdate.ToString("MM/dd/yyyy"));
            dt_GetMembersPlanDetails = GetMembersPlanDetails(hi.MemberShipCode, hi.BranchCode, arlMembers);
            dt_GetMembersPersonalDetails = GetMembersPersonalDetails(hi.MemberShipCode, startdate.ToString("MM/dd/yyyy"));
            dt_GetWalletCalculation = GetWalletCalculation(hi.MemberShipCode);
            dt_FAInvoiceLatestRecord = GetFAInvoiceLatestRecord(hi.MemberShipCode);
            string RemainingAmount = "";
            try
            {
                RemainingAmount = dt_GetWalletCalculation.Rows[0]["RemainingAmount"].ToString();
            }
            catch (Exception ec)
            {

            }

            string Invoice2 = GetMembersInvoiceNumberForFacilityPosts(hi.MemberShipCode);


            string SendText = string.Empty;

            string SMFMID = dt_FAInvoiceLatestRecord.Rows[0]["SMFMID"].ToString();

            if (SMFMID == "" || SMFMID == "0")
            {
                SMFMID = "8";
            }

            string ReceiptNo = Convert.ToString(ReceiptGeneration());

            if (hi.Mode == 0)
            {
                cnn.Open();
                SqlCommand command = cnn.CreateCommand();
                SqlTransaction transaction;
                // Start a local transaction.
                transaction = cnn.BeginTransaction("SampleTransaction");
                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,PackageCode,PackageName,SessionCode,SessionName,PlanCode,PlanName,PlanAmount,SlotCode,SlotName,DurationCode,DurationName,RemainingAmount,LeftOutDays,Days,CreatedBy,CreatedOn,IsDeleted,IsActive,BranchCode,IsChange,NewMembersCode,InvoiceID,FacilityStartDate,FacilityExpireDate) values('" + hi.MemberShipCode + "','" + hi.MFDID + "','" + dt_GetMembersPlanDetails.Rows[0]["PackageCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PackageName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SessionCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SessionName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanCost"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SlotCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SlotName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["DurationID"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["DurationName"].ToString() + "','" + RemainingAmount + "','" + Convert.ToInt32(dt_GetWalletCalculation.Rows[0]["LeftOutDays"].ToString()) + "','" + Convert.ToInt32(dt_GetWalletCalculation.Rows[0]["TotalDays"].ToString()) + "','" + hi.CreatedBy + "','" + ServerDateTime + "',0,1,'" + hi.BranchCode + "',1,'" + hi.MemberShipCode + "','" + Invoice2 + "','" + hi.FacilityStartDate + "','" + hi.FacilityExpireDate + "')";
                    command.ExecuteNonQuery();
                    command.CommandText = "update CCRMMembership set MembershipExpireDate='" + hi.FacilityExpireDate + "'  where  InvoiceID='" + Invoice2 + "'";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into Remainders(MembershipCode,RemainderCode,RemainderName,FAInvoice,Amount,Date,CreatedBy,CreatedOn) values('" + hi.MemberShipCode + "','" + hi.RemainderCode + "','" + hi.RemainderName + "','" + Invoice2 + "','" + dt_FAInvoiceLatestRecord.Rows[0]["AmountDue"].ToString() + "','" + dt_FAInvoiceLatestRecord.Rows[0]["DueDate"].ToString() + "','" + hi.CreatedBy + "','" + ServerDateTime + "')";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into CCRMMembershipHoldFacility(MemberShipCode,MFDID,SMFMID,FromDate,ToDate,NoOfDays,CreatedBy,CreatedOn,IsDeleted,IsActive) VALUES('" + hi.MemberShipCode + "','" + SMFMID + "','" + SMFMID + "','" + hi.FacilityStartDate + "','" + hi.FacilityExpireDate + "','" + Convert.ToInt32(dt_GetWalletCalculation.Rows[0]["TotalDays"].ToString()) + "','" + hi.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();

                    if (hi.Comments != "")
                    {
                        command.CommandText = "insert into CommentsTracker(UCode,MasterID,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + hi.MemberShipCode + "','0','" + hi.Comments + "','" + hi.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    cnn.Close();
                    ffopt.status = "success";

                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();

                    }
                    catch (Exception ex2)
                    {

                    }
                    ffopt.status = "fail";
                }

            }
            else if (hi.Mode == 1)
            {
                SNO = GetFacilityLatest(hi.MemberShipCode);

                cnn.Open();
                SqlCommand command = cnn.CreateCommand();
                SqlTransaction transaction;
                // Start a local transaction.
                transaction = cnn.BeginTransaction("SampleTransaction");
                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,PackageCode,PackageName,SessionCode,SessionName,PlanCode,PlanName,PlanAmount,SlotCode,SlotName,DurationCode,DurationName,RemainingAmount,LeftOutDays,Days,CreatedBy,CreatedOn,IsDeleted,IsActive,BranchCode,IsChange,NewMembersCode,InvoiceID,FacilityStartDate,FacilityExpireDate) values('" + hi.MemberShipCode + "','" + hi.MFDID + "','" + dt_GetMembersPlanDetails.Rows[0]["PackageCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PackageName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SessionCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SessionName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanCost"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SlotCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SlotName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["DurationID"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["DurationName"].ToString() + "','" + RemainingAmount + "','" + Convert.ToInt32(dt_GetWalletCalculation.Rows[0]["LeftOutDays"].ToString()) + "','" + Convert.ToInt32(dt_GetWalletCalculation.Rows[0]["TotalDays"].ToString()) + "','" + hi.CreatedBy + "','" + ServerDateTime + "',0,1,'" + hi.BranchCode + "',1,'" + hi.MemberShipCode + "','" + Invoice2 + "','" + hi.FacilityStartDate + "','" + hi.FacilityExpireDate + "')";
                    command.ExecuteNonQuery();
                    command.CommandText = "update CCRMMembership set MembershipExpireDate='" + ServerDateTime + "'  where  InvoiceID='" + Invoice2 + "'";
                    command.ExecuteNonQuery();
                    command.CommandText = "insert into Remainders(MembershipCode,RemainderCode,RemainderName,FAInvoice,Amount,Date,CreatedBy,CreatedOn) values('" + hi.MemberShipCode + "','" + hi.RemainderCode + "','" + hi.RemainderName + "','" + Invoice2 + "','" + dt_FAInvoiceLatestRecord.Rows[0]["AmountDue"].ToString() + "','" + dt_FAInvoiceLatestRecord.Rows[0]["DueDate"].ToString() + "','" + hi.CreatedBy + "','" + ServerDateTime + "')";
                    command.ExecuteNonQuery();
                    command.CommandText = "update CCRMMembershipFacility set FacilityExpireDate='" + ServerDateTime + "',SMFMID=8 where SNO='" + SNO + "'";
                    command.ExecuteNonQuery();

                    if (hi.Comments != "")
                    {
                        command.CommandText = "insert into CommentsTracker(UCode,MasterID,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + hi.MemberShipCode + "','0','" + hi.Comments + "','" + hi.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    cnn.Close();
                    ffopt.status = "success";
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {

                    }
                    ffopt.status = "fail";
                }

            }

            string sJSONResponse = JsonConvert.SerializeObject(ffopt);
            return sJSONResponse;
        }
        public Object ConvertToWallet([FromBody] ConvertInput ci)
        {
            FacilityOutputStatus ffopt = new FacilityOutputStatus();

            string BranchCode = ci.BranchCode;
            string MemberShipCode = ci.MemberShipCode;
            string CreatedBy = ci.CreatedBy;

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet = new DataSet();
            ArrayList arl = new ArrayList();
            ArrayList arlMembers = new ArrayList();
            arl = MembersLatestDates(MemberShipCode);

            DateTime startdate = Convert.ToDateTime(arl[0].ToString());

            DataTable dt_GetMembersPlanDetails = new DataTable();
            DataTable dt_GetMembersPersonalDetails = new DataTable();
            DataTable dt_GetWalletCalculation = new DataTable();
            DataTable dt_FAInvoiceLatestRecord = new DataTable();

            dt_GetWalletCalculation.Clear();
            arlMembers = GetMembersBasicPlanDetails(MemberShipCode, startdate.ToString("MM/dd/yyyy"));
            dt_GetMembersPlanDetails = GetMembersPlanDetails(MemberShipCode, BranchCode, arlMembers);
            dt_GetMembersPersonalDetails = GetMembersPersonalDetails(MemberShipCode, startdate.ToString("MM/dd/yyyy"));
            dt_GetWalletCalculation = GetWalletCalculation(MemberShipCode);
            dt_FAInvoiceLatestRecord = GetFAInvoiceLatestRecord(MemberShipCode);
            string RemainingAmount = "";

            try
            {
                RemainingAmount = dt_GetWalletCalculation.Rows[0]["RemainingAmount"].ToString();
            }
            catch (Exception ec)
            {

            }

            string Invoice2 = GetMembersInvoiceNumberForFacilityPosts(ci.MemberShipCode);


            string SendText = string.Empty;
            string SMFMID = "";
            try
            {
                SMFMID = dt_FAInvoiceLatestRecord.Rows[0]["SMFMID"].ToString();
            }
            catch (Exception ec)
            {

            }


            if (SMFMID == "" || SMFMID == "0")
            {
                SMFMID = "8";
            }

            string Comments = "";
            try
            {
                Comments = ci.Comments;
            }
            catch
            {
                Comments = "";
            }


            float Wallet = 0.0f;

            try
            {
                Wallet = float.Parse(ci.Wallet);
            }
            catch
            {
                Wallet = 0.0f;
            }

            int ReceiptNo = ReceiptGeneration();

            cnn.Open();
            SqlCommand command = cnn.CreateCommand();
            SqlTransaction transaction;
            // Start a local transaction.
            transaction = cnn.BeginTransaction("SampleTransaction");
            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = cnn;
            command.Transaction = transaction;

            try
            {
                command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,PackageCode,PackageName,SessionCode,SessionName,PlanCode,PlanName,PlanAmount,SlotCode,SlotName,DurationCode,DurationName,RemainingAmount,LeftOutDays,Days,CreatedBy,CreatedOn,IsDeleted,IsActive,BranchCode,IsChange,NewMembersCode,InvoiceID,FacilityStartDate,FacilityExpireDate) values('" + ci.MemberShipCode + "','8','" + dt_GetMembersPlanDetails.Rows[0]["PackageCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PackageName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SessionCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SessionName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanCost"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SlotCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SlotName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["DurationID"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["DurationName"].ToString() + "','','','','" + ci.CreatedBy + "','" + ServerDateTime + "',0,1,'" + ci.BranchCode + "',1,'" + ci.MemberShipCode + "','" + Invoice2 + "','" + ci.FacilityStartDate + "','" + ci.FacilityExpireDate + "')";
                command.ExecuteNonQuery();
                command.CommandText = "update CCRMMembership set MembershipExpireDate='" + ServerDateTime + "'  where  InvoiceID='" + Invoice2 + "'";
                command.ExecuteNonQuery();
                command.CommandText = "insert into Remainders(MembershipCode,RemainderCode,RemainderName,FAInvoice,Amount,Date,CreatedBy,CreatedOn) values('" + ci.MemberShipCode + "','" + ci.RemainderCode + "','" + ci.RemainderName + "','" + Invoice2 + "','" + dt_FAInvoiceLatestRecord.Rows[0]["AmountDue"].ToString() + "','" + dt_FAInvoiceLatestRecord.Rows[0]["DueDate"].ToString() + "','" + ci.CreatedBy + "','" + ServerDateTime + "')";
                command.ExecuteNonQuery();
                command.CommandText = "insert into CCRMMembershipHoldFacility(MemberShipCode,MFDID,SMFMID,FromDate,ToDate,NoOfDays,CreatedBy,CreatedOn,IsDeleted,IsActive) VALUES('" + ci.MemberShipCode + "','8','8','" + ci.FacilityStartDate + "','" + ci.FacilityExpireDate + "','" + ci.NoOfDays + "','" + ci.CreatedBy + "','" + ServerDateTime + "',0,1)";
                command.ExecuteNonQuery();
                command.CommandText = "insert into WalletTransactions(MembershipCode,NewMembershipCode,InvoiceId,TransactionId,TransactionName,MobileNo,Credit,Debit,CreatedOn,CreatedBy,IsDeleted,IsActive) values('" + ci.MemberShipCode + "','','" + Invoice2 + "','1101','Convert','" + ci.MobileNo + "','" + ci.Credit + "','0','" + ServerDateTime + "','" + ci.CreatedBy + "',0,1) ";
                command.ExecuteNonQuery();
                if (ci.Comments != "")
                {
                    command.CommandText = "insert into CommentsTracker(UCode,MasterID,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + ci.MemberShipCode + "','0','" + ci.Comments + "','" + ci.CreatedBy + "','" + ServerDateTime + "',0,1)";
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
                cnn.Close();

                ffopt.status = "success";


            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();

                }
                catch (Exception ex2)
                {

                }
                ffopt.status = "fail";
            }
            string sJSONResponse = JsonConvert.SerializeObject(ffopt);
            return sJSONResponse;

        }
        public DataTable GetMembersDetails(string InvoiceId, DateTime StartDate)
        {
            DataTable dt_GetMembersDetails = new DataTable();
            dt_GetMembersDetails = getdata(string.Format("select top 1 FAI.MembershipCode,FAI.InvoiceID,CCRMM.BranchCode,CMSSS.SessionCode,CMSS.SessionName,CCRMM.SlotCode,CMSSS.SlotName,CCRMM.PackageCode,CMSPAC.PackageName,CCRMM.PlanCode,CMSP.PlanName,LeftOutDays=DATEDIFF(d, GETDATE(), '{1}'),LeftOutAmount=(select top 1 TotalAmountPaied=(SUM(FAI.PayableAmount)+SUM(FAI.PayableAmount2))  from FAInvoice FAI where   FAI.InvoiceID = '{0}' )  from FAInvoice FAI, CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,CMSPACKAGESCOST CMSPAC,CMSSESSIONTIMESETTING CMSS,CMSPLAN CMSP where CCRMM.PlanCode=CMSP.PlanCode and CMSS.SessionCode=CMSSS.SessionCode and CCRMM.SlotCode=CMSSS.SlotCode and CCRMM.PackageCode=CMSPAC.PackageCode and  FAI.InvoiceID = '{0}' ", InvoiceId, StartDate));
            return dt_GetMembersDetails;
        }
        public string ReceiptNo()
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
        public int UIDNew(string BranchCode)
        {
            int UIDNew = 0;
            string query = "select isnull((Max(UID)+1), 3001) as UID from LoginID where BranchCode='" + BranchCode + "'";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_Login = cmd_SubEXT.ExecuteReader();
                if (DR_Login.Read())
                {
                    UIDNew = Convert.ToInt32(DR_Login[0].ToString());
                }
                cnn.Close();
            }
            return UIDNew;
        }
        public ArrayList GetGSTCode()
        {
            //-- GYMFEES','Trainer Fees','IGSTableAmount','IGST','CGST','SGST
            //IGSTtableamount - IGST
            //payableamount* Personaltrainer%
            //payableamount * Gymfee %
            //IGSTtableamount * IGST
            //IGSTtableamount* CGST
            //IGSTtableamount * SGST

            ArrayList arl_gst = new ArrayList();
            string GSTCode = "";
            int IGSTPercentage = 0;
            int CGSTPercentage = 0;
            int SGSTPercentage = 0;
            int GYMFeePercentage = 0;
            int PersonalTrainerPercentage = 0;

            string query = "select GSTCode,IGSTPercentage,CGSTPercentage,SGSTPercentage,GYMFeePercentage,PersonalTrainerPercentage from FAGST ";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_GST = cmd_SubEXT.ExecuteReader();
                if (DR_GST.Read())
                {
                    GSTCode = DR_GST[0].ToString();
                    IGSTPercentage = Convert.ToInt32(DR_GST[1].ToString());
                    CGSTPercentage = Convert.ToInt32(DR_GST[2].ToString());
                    SGSTPercentage = Convert.ToInt32(DR_GST[3].ToString());
                    GYMFeePercentage = Convert.ToInt32(DR_GST[4].ToString());
                    PersonalTrainerPercentage = Convert.ToInt32(DR_GST[5].ToString());
                }
                cnn.Close();
            }

            arl_gst.Add(GSTCode);
            arl_gst.Add(IGSTPercentage);
            arl_gst.Add(CGSTPercentage);
            arl_gst.Add(SGSTPercentage);
            arl_gst.Add(GYMFeePercentage);
            arl_gst.Add(PersonalTrainerPercentage);

            return arl_gst;

        }
        public int CheckExistency(string MembershipCode, string PlanCode, string SlotCode, string PackageCode, DateTime StartDate, DateTime EndDate)
        {
            //string query = "select count(*) as Count from CCRMMembership where MembershipCode='" + MembershipCode + "' and PlanCode='" + PlanCode + "' and SlotCode='" + SlotCode + "' and PackageCode='" + PackageCode + "' and DATEADD(dd, DATEDIFF(dd, 0, '" + StartDate + "'), 0)='' and DATEADD(dd, DATEDIFF(dd, 0, '" + EndDate + "'), 0)='' ";
            string query = "select count(*) as Count from CCRMMembership where MembershipCode='" + MembershipCode + "' and PlanCode='" + PlanCode + "' and SlotCode='" + SlotCode + "' and PackageCode='" + PackageCode + "'  ";
            int Exentency = 0;
            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Close();
                cnn.Open();
                SqlDataReader DR_Exist = cmd_SubEXT.ExecuteReader();
                if (DR_Exist.Read())
                {
                    Exentency = Convert.ToInt32(DR_Exist[0].ToString());
                }
                cnn.Close();
            }

            return Exentency;

        }
        public Object ConvertPackage([FromBody]FacConvert c)
        {
            double FinalAmount = 0.0f;
            double FinalAmount2 = 0.0f;
            double IGST = 0.0f;
            double FinalAmount1 = 0.0f;
            double TransferfeePercentage = 0.0f;
            double Transferfee = 0.0f;
            double AmountPerDay = 0.0f;
            double LeftOutDays = 0.0d;
            double CompleteDays = 0.0d;
            double TotalDays = 0.0d;
            double RemainingAmount = 0.0d;

            string sJSONResponse = "";
            int Year = DateTime.Now.Year - 2000;
            int Month = DateTime.Now.Month;
            FacilityOutputStatus ffopt = new FacilityOutputStatus();

            string BranchCode = c.BranchCode;
            string MemberShipCode = c.MembershipCode;
            string CreatedBy = c.CreatedBy;

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet = new DataSet();
            ArrayList arl = new ArrayList();
            ArrayList arlMembers = new ArrayList();

            string r = "";
            DataSet ds_PaymentDetailsPost = new DataSet();

            DataTable dt_AppType = new DataTable();
            ArrayList arl_pdetails = GetPersonDetailsByMobileNo(c.MobileNo);

            arl = MembersLatestDates(MemberShipCode);

            DateTime startdate = Convert.ToDateTime(arl[0].ToString());
            DataTable dt_GetMembersPlanDetails = new DataTable();
            DataTable dt_GetMembersPersonalDetails = new DataTable();
            DataTable dt_GetWalletCalculation = new DataTable();
            DataTable dt_FAInvoiceLatestRecord = new DataTable();

            //dt_GetWalletCalculation.Clear();
            arlMembers = GetMembersBasicPlanDetails(MemberShipCode, startdate.ToString("MM/dd/yyyy"));
            dt_GetMembersPlanDetails = GetMembersPlanDetails(MemberShipCode, BranchCode, arlMembers);
            dt_GetMembersPersonalDetails = GetMembersPersonalDetails(MemberShipCode, startdate.ToString("MM/dd/yyyy"));
            dt_GetWalletCalculation = GetWalletCalculation(MemberShipCode);
            dt_FAInvoiceLatestRecord = GetFAInvoiceLatestRecord(MemberShipCode);


            try
            {
                RemainingAmount = Convert.ToDouble(dt_GetWalletCalculation.Rows[0]["RemainingAmount"].ToString());
            }
            catch (Exception ec)
            {

            }

            string Invoice2 = GetMembersInvoiceNumberForFacilityPosts(c.MembershipCode);


            string SendText = string.Empty;
            string SMFMID = "";
            try
            {
                SMFMID = dt_FAInvoiceLatestRecord.Rows[0]["SMFMID"].ToString();
            }
            catch (Exception ec)
            {

            }


            if (SMFMID == "" || SMFMID == "0")
            {
                SMFMID = "8";
            }

            string Comments = "";
            try
            {
                Comments = c.Comments;
            }
            catch
            {
                Comments = "";
            }


            float Wallet = 0.0f;

            try
            {
                Wallet = float.Parse(c.Wallet);
            }
            catch
            {
                Wallet = 0.0f;
            }

            int ReceiptNo = ReceiptGeneration();

            string MembershipCode = GetMembersExistance(c.MobileNo);

            // Order.EnquireTypeNo == 2
            ArrayList al = new ArrayList();

            if (c.Mode == 2)
            {
                r = "";
            }
            else
            {
                if (MembershipCode == null || MembershipCode == "" || MembershipCode == string.Empty)
                {
                    al = MCode("1104");
                    r = (String)al[1].ToString();
                }
                else
                {
                    r = MembershipCode;
                }

            }

            // Invoice Generation
            string Invoice = "N" + UniqueGeneration2();
            int UID = UIDNew("1104");

            if (InvoiceCheck(Invoice) != 0)
            {
                Invoice = "N" + UniqueGeneration2();
            }
            //  ReceiptNo();
            int ReceiptNos = 101;

            ArrayList dates = MembersLatestDatesByInvoice(dt_FAInvoiceLatestRecord.Rows[0]["InvoiceID"].ToString());
            DateTime MembersStartDate = Convert.ToDateTime(dates[0].ToString());
            DateTime MembersEndDate = Convert.ToDateTime(dates[1].ToString());
            double Duration = Convert.ToInt32(dates[2].ToString());
            ArrayList Amount = MembersLatestAmountByInvoice(dt_FAInvoiceLatestRecord.Rows[0]["InvoiceID"].ToString());

            DateTime CurrentDate = DateTime.Now;

            if (MembersEndDate > CurrentDate)
            {
                CompleteDays = (CurrentDate - MembersStartDate).TotalDays;
                LeftOutDays = (MembersEndDate - CurrentDate).TotalDays;
                TotalDays = Duration * 30;
            }

            FinalAmount = Convert.ToDouble(Amount[0].ToString());
            IGST = Convert.ToDouble(Amount[1].ToString());

            if (LeftOutDays < 1)
                LeftOutDays = 0;
            if (LeftOutDays > TotalDays)
                LeftOutDays = TotalDays;
            if (CompleteDays < 1)
                CompleteDays = 0;



            if (FinalAmount > 0 && IGST > 0)
            {
                //10.5.2019
                //FinalAmount1 = Convert.ToDouble(FinalAmount) - Convert.ToDouble(IGST);
                FinalAmount1 = Convert.ToDouble(FinalAmount);
                TransferfeePercentage = TransferPresentPercentage(c.MembershipCode, CompleteDays);
                //Transferfee = Convert.ToDouble(FinalAmount) * (TransferfeePercentage / 100);
                Transferfee = 0;
                FinalAmount2 = FinalAmount1 - Transferfee;
                if (TotalDays > 0)
                {
                    AmountPerDay = Math.Round(FinalAmount2 / TotalDays);
                    RemainingAmount = Math.Round((AmountPerDay * LeftOutDays));
                }
            }




            if (c.Mode == 0)
            {
                cnn.Open();
                SqlCommand command = cnn.CreateCommand();
                SqlTransaction transaction;
                // Start a local transaction.
                transaction = cnn.BeginTransaction("SampleTransaction");
                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = cnn;
                command.Transaction = transaction;

                try
                {
                    //EnquireTypeNo
                    if (c.EnquireTypeNo != 2)
                    {
                        if (MembershipCode == null || MembershipCode == "" || MembershipCode == string.Empty)
                        {
                            command.CommandText = "insert into Users(UCode,BranchCode,UserName,Firstname,Lastname,DateOfBirth,Gender,MaritialStatus,Address,Area,City,State,Country,PinCode,MobileNo,CreatedBy,CreatedOn,IsDeleted,IsActive,Email,Photo,Address2,PhotoUrl) values('" + r + "','1104','" + arl_pdetails[0].ToString() + "','" + arl_pdetails[0].ToString() + "','','" + arl_pdetails[1].ToString() + "','" + arl_pdetails[2].ToString() + "','','','','','','','','" + c.MobileNo + "','" + c.MobileNo + "','" + ServerDateTime + "',0,1,'','','','')";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into CCRMMembershipCodification(SerialNo,MembershipCode,BranchCode,Year,Month) values(" + al[0].ToString() + ",'" + r + "','1104','" + Convert.ToString(Year) + "','" + Convert.ToString(Month) + "')";
                            command.ExecuteNonQuery();
                        }
                        command.CommandText = "insert into CCRMMEnquireStatus(MobileNo,EnquireTypeNo,MobileDeviceID,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + c.MobileNo + "','" + c.EnquireTypeNo + "','" + c.MobileDeviceID + "','" + c.MobileNo + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into PaymentGateway(GatewayProviderName,OrderId,BranchCode,MemberShipCode,Invoice,MobileNo,ModeOfPayment,payment_capture,Amount,Currency,Discount,receipt,TransactionDate,CreatedOn,CreatedBy,IsDeleted,IsActive) values('GPN001','" + c.OrderId + "','1104','" + r + "','" + Invoice + "','" + c.MobileNo + "','" + c.ModeOfPayment + "','" + c.PaymentId + "','" + c.Amount + "','INR','" + c.DiscountAmount + "','" + ReceiptNos + "','" + c.TransactionDate + "','" + ServerDateTime + "','" + c.CreatedBy + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into OnlinePackagePurchase(BranchCode,MembershipCode,MobileNo,EnquireTypeNo,PackageID,StartDate,EndDate,ActualPrice,NumberOfSession,NumberOfDaysValidity,MobileDeviceID,Invoice,Discount,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('1104','" + r + "','" + c.MobileNo + "','" + c.EnquireTypeNo + "','" + c.PackageCode + "','" + c.MembershipStartDate + "','" + c.EndDate + "','" + c.ActualPrice + "','" + c.NumberOfSession + "','" + c.NumberOfDaysValidity + "','" + c.MobileDeviceID + "','" + Invoice + "','" + c.DiscountAmount + "','','" + c.MobileNo + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',1,'FREEZING','" + c.Freezing + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',3,'UPGRADATION','" + c.Upgrade + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',4,'CHANGE','" + c.Change + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',5,'TRANSFER TRANSFER','" + c.Transfer + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',6,'PERSON TRANSFER','" + c.Transfer + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',8,'HOLD','" + c.Transfer + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',9,'Paused','" + c.Paused + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',10,'Convert','" + c.Convert + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,PackageCode,PackageName,SessionCode,SessionName,PlanCode,PlanName,PlanAmount,SlotCode,SlotName,DurationCode,DurationName,LeftOutDays,LeftOutAmount,CreatedBy,CreatedOn,IsDeleted,IsActive,BranchCode,IsChange,InvoiceID,RemainingAmount,NewInvoiceID) values('" + c.MembershipCode + "','" + SMFMID + "','" + c.PackageCode + "','" + dt_GetMembersPlanDetails.Rows[0]["PackageName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SessionCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SessionName"].ToString() + "','" + c.PlanCode + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanCost"].ToString() + "','" + c.SlotCode + "','" + dt_GetMembersPlanDetails.Rows[0]["SlotName"].ToString() + "','" + c.DurationId + "','" + dt_GetMembersPlanDetails.Rows[0]["DurationName"].ToString() + "','" + Math.Round(LeftOutDays) + "','0','" + c.CreatedBy + "','" + ServerDateTime + "',0,1,'" + c.BranchCode + "',1,'" + Invoice + "','" + Math.Round(RemainingAmount) + "','" + Invoice2 + "')";
                        command.ExecuteNonQuery();
                    }
                    else
                    {

                        command.CommandText = "insert into CCRMMEnquireStatus(MobileNo,EnquireTypeNo,MobileDeviceID,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + c.MobileNo + "','" + c.EnquireTypeNo + "','" + c.MobileDeviceID + "','" + c.MobileNo + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into PaymentGateway(GatewayProviderName,OrderId,BranchCode,MemberShipCode,Invoice,MobileNo,ModeOfPayment,payment_capture,Amount,Currency,Discount,receipt,TransactionDate,CreatedOn,CreatedBy,IsDeleted,IsActive) values('GPN001','" + c.OrderId + "','1104','" + r + "','" + Invoice + "','" + c.MobileNo + "','" + c.ModeOfPayment + "','" + c.PaymentId + "','" + c.Amount + "','INR','" + c.DiscountAmount + "','" + ReceiptNos + "','" + c.TransactionDate + "','" + ServerDateTime + "','" + c.CreatedBy + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into OnlinePackagePurchase(BranchCode,MembershipCode,MobileNo,EnquireTypeNo,PackageID,StartDate,EndDate,ActualPrice,NumberOfSession,NumberOfDaysValidity,MobileDeviceID,Invoice,Discount,Comment,CreatedBy,CreatedOn,IsDeleted,IsActive) values('1104','" + r + "','" + c.MobileNo + "','" + c.EnquireTypeNo + "','" + c.PackageCode + "','" + c.MembershipStartDate + "','" + c.EndDate + "','" + c.ActualPrice + "','" + c.NumberOfSession + "','" + c.NumberOfDaysValidity + "','" + c.MobileDeviceID + "','" + Invoice + "','" + c.DiscountAmount + "','','" + c.MobileNo + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',1,'FREEZING','" + c.Freezing + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',3,'UPGRADATION','" + c.Upgrade + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',4,'CHANGE','" + c.Change + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',5,'TRANSFER TRANSFER','" + c.Transfer + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',6,'PERSON TRANSFER','" + c.Transfer + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',8,'HOLD','" + c.Transfer + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',9,'Paused','" + c.Paused + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',10,'Convert','" + c.Convert + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                        command.ExecuteNonQuery();
                        command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,PackageCode,PackageName,SessionCode,SessionName,PlanCode,PlanName,PlanAmount,SlotCode,SlotName,DurationCode,DurationName,LeftOutDays,LeftOutAmount,CreatedBy,CreatedOn,IsDeleted,IsActive,BranchCode,IsChange,InvoiceID,RemainingAmount,NewInvoiceID) values('" + c.MembershipCode + "','" + SMFMID + "','" + c.PackageCode + "','" + dt_GetMembersPlanDetails.Rows[0]["PackageName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SessionCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SessionName"].ToString() + "','" + c.PlanCode + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanCost"].ToString() + "','" + c.SlotCode + "','" + dt_GetMembersPlanDetails.Rows[0]["SlotName"].ToString() + "','" + c.DurationId + "','" + dt_GetMembersPlanDetails.Rows[0]["DurationName"].ToString() + "','" + Math.Round(LeftOutDays) + "','0','" + c.CreatedBy + "','" + ServerDateTime + "',0,1,'" + c.BranchCode + "',1,'" + Invoice + "','" + Math.Round(RemainingAmount) + "','" + Invoice2 + "')";
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    cnn.Close();


                    ffopt.status = "success";


                }
                catch (Exception ex)
                {

                    try
                    {
                        transaction.Rollback();

                    }
                    catch (Exception ex2)
                    {

                    }
                    ffopt.status = "fail";
                }
            }
            else if (c.Mode == 1)
            {
                ArrayList ary_GST = new ArrayList();
                ary_GST = GetGSTCode();
                // Clients Unique COde

                string GSTCode = "";
                try
                {
                    GSTCode = ary_GST[0].ToString();
                }
                catch (Exception ec)
                {
                }

                float IGSTPercentage = float.Parse(ary_GST[1].ToString());
                float CGSTPercentage = float.Parse(ary_GST[2].ToString());
                float SGSTPercentage = float.Parse(ary_GST[3].ToString());
                float GYMFeePercentage = float.Parse(ary_GST[4].ToString());
                float PersonalTrainerPercentage = float.Parse(ary_GST[5].ToString());

                float PayableAmount = Convert.ToInt32(c.PayableAmount);

                float PersonalTrainer = PayableAmount * (PersonalTrainerPercentage / 100);
                float IGSTableAmount = PayableAmount * (GYMFeePercentage / 100);
                float IGST1 = PayableAmount * (IGSTPercentage / 100);
                float CGST = PayableAmount * (CGSTPercentage / 100);
                float SGST = PayableAmount * (SGSTPercentage / 100);
                float GYMFEE = IGSTableAmount - IGST1;
                int SerialNo = 0;

                MembershipCode = GetMembersExistance(c.MobileNo);

                if (MembershipCode == null || MembershipCode == "" || MembershipCode == string.Empty)
                {
                    al = MCode(c.BranchCode);
                    r = (String)al[1].ToString();
                    SerialNo = Convert.ToInt32(al[0].ToString());
                }
                else
                {
                    r = MembershipCode;

                }

                // Invoice Generation
                string InvoiceM1 = "N" + UniqueGeneration2();
                UID = UIDNew(c.BranchCode);

                if (InvoiceCheck(Invoice) != 0)
                {
                    Invoice = "N" + UniqueGeneration2();
                }



                ReceiptNos = ReceiptGeneration();
                if (CheckExistency(c.MembershipCode, c.PlanCode, c.SlotCode, c.PackageCode, Convert.ToDateTime(c.MembershipStartDate), Convert.ToDateTime(c.MembershipExpireDate)) == 0)
                {
                    cnn.Open();
                    SqlCommand command = cnn.CreateCommand();
                    SqlTransaction transaction;
                    // Start a local transaction.
                    transaction = cnn.BeginTransaction("SampleTransaction");
                    // Must assign both transaction object and connection
                    // to Command object for a pending local transaction
                    command.Connection = cnn;
                    command.Transaction = transaction;

                    try
                    {
                        {

                            if (MembershipCode == null || MembershipCode == "" || MembershipCode == string.Empty)
                            {

                                command.CommandText = "insert into Users(UCode,BranchCode,UserName,Firstname,Lastname,DateOfBirth,Gender,MaritialStatus,Address,Area,City,State,Country,PinCode,MobileNo,CreatedBy,CreatedOn,IsDeleted,IsActive,Email,Photo,Address2,PhotoUrl) values('" + r + "','" + c.BranchCode + "','" + arl_pdetails[0].ToString() + "','" + arl_pdetails[0].ToString() + "','','" + arl_pdetails[1].ToString() + "','" + arl_pdetails[2].ToString() + "','','','','1','1','1','','" + c.MobileNo + "','" + c.MobileNo + "','" + ServerDateTime + "',0,1,'','','','')";
                                command.ExecuteNonQuery();
                                command.CommandText = "insert into CCRMMembershipCodification(SerialNo,MembershipCode,BranchCode,Year,Month) values(" + al[0].ToString() + ",'" + r + "','" + c.BranchCode + "','" + Convert.ToString(Year) + "','" + Convert.ToString(Month) + "')";
                                command.ExecuteNonQuery();
                            }
                            command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',1,'FREEZING','" + c.Freezing + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',3,'UPGRADATION','" + c.Upgrade + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',4,'CHANGE','" + c.Change + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',5,'TRANSFER TRANSFER','" + c.Transfer + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',6,'PERSON TRANSFER','" + c.Transfer + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',8,'HOLD','" + c.Transfer + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',9,'Paused','" + c.Paused + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into FacilityOpted(MembershipCode,SMFDID,FacilityName,Opted,Invoice,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + r + "',10,'Convert','" + c.Convert + "','" + Invoice + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1)";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into CCRMMEnquireStatus(MobileNo,EnquireTypeNo,MobileDeviceID,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + c.MobileNo + "','" + c.EnquireTypeNo + "','" + c.MobileDeviceID + "','" + c.MobileNo + "','" + ServerDateTime + "',0,1)";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into CCRMMembership(MembershipCode,BranchCode,PlanCode,MembershipExpireDate,CreatedBy,CreatedOn,IsDeleted,IsActive,SlotCode,PackageCode,StartDate,TrainerCode,InvoiceID,Receipt,SerialNo,Year,Month,PlanCostCode,DurationId,EnquireTypeNo,EnquireTypeIncentives) values('" + r + "','" + c.BranchCode + "','" + c.PlanCode + "','" + c.MembershipExpireDate + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1,'" + c.SlotCode + "','" + c.PackageCode + "','" + c.MembershipStartDate + "','" + c.TrainerCode + "','" + Invoice + "','" + ReceiptNo + "'," + SerialNo + ",'" + Year + "','" + Month + "','" + c.PlanCostCode + "','" + c.DurationId + "','" + c.EnquireTypeNo + "','" + c.EnquireTypeNo + "')";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into FAInvoice(InvoiceID,GSTCode,FAPaymentModes,MembershipCode,PayableAmount,AmountDue,GymFees,TrainerFees,IGSTableAmount,IGST,CGST,SGST,DueDate,CreatedBy,CreatedOn,IsDeleted,IsActive,FinalAmount,FAPaymentModes2,PayableAmount2,PromoCode,ReferenceID,DiscountAmount,TrainerCode,Receipt,EnquireTypeNo,SlotPrice,PlanCost,PaymentDate,RemainingAmount,Wallet,OrderId,Currency) values('" + Invoice + "','" + GSTCode + "','PY6','" + r + "','" + c.PayableAmount + "','" + c.AmountDue + "','" + GYMFEE + "','" + PersonalTrainer + "','" + IGSTableAmount + "','" + IGST + "','" + CGST + "','" + SGST + "','" + c.DueDate + "','" + c.CreatedBy + "','" + ServerDateTime + "',0,1,'" + c.FinalAmount + "','0','" + c.PayableAmount + "','" + c.PromoCode + "','','" + c.DiscountAmount + "','" + c.TrainerCode + "','" + ReceiptNo + "','" + c.EnquireTypeNo + "','" + c.SlotPrice + "','" + c.PlanCost + "','" + c.TransactionDate + "','0.0','" + c.Wallet + "','" + c.OrderId + "','" + c.Currency + "')";
                            command.ExecuteNonQuery();
                            command.CommandText = "insert into CCRMMembershipFacility(MemberShipCode,MFDID,PackageCode,PackageName,SessionCode,SessionName,PlanCode,PlanName,PlanAmount,SlotCode,SlotName,DurationCode,DurationName,LeftOutDays,LeftOutAmount,CreatedBy,CreatedOn,IsDeleted,IsActive,BranchCode,IsChange,InvoiceID,RemainingAmount,NewInvoiceID) values('" + c.MembershipCode + "','" + SMFMID + "','" + c.PackageCode + "','" + dt_GetMembersPlanDetails.Rows[0]["PackageName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SessionCode"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["SessionName"].ToString() + "','" + c.PlanCode + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanName"].ToString() + "','" + dt_GetMembersPlanDetails.Rows[0]["PlanCost"].ToString() + "','" + c.SlotCode + "','" + dt_GetMembersPlanDetails.Rows[0]["SlotName"].ToString() + "','" + c.DurationId + "','" + dt_GetMembersPlanDetails.Rows[0]["DurationName"].ToString() + "','" + Math.Round(LeftOutDays) + "','0','" + c.CreatedBy + "','" + ServerDateTime + "',0,1,'" + c.BranchCode + "',1,'" + Invoice + "','" + Math.Round(RemainingAmount) + "','" + Invoice2 + "')";
                            command.ExecuteNonQuery();

                            transaction.Commit();
                            cnn.Close();

                            ffopt.status = "success";

                        }
                    }
                    catch (Exception ex)
                    {
                        ffopt.status = "fail";
                        try
                        {
                            transaction.Rollback();

                        }
                        catch (Exception ex2)
                        {

                        }
                    }

                }



            }

            sJSONResponse = JsonConvert.SerializeObject(ffopt);
            return sJSONResponse;
        }
        public ArrayList GetPersonDetailsByMobileNo(string MobileNo)
        {
            ArrayList arl_pd = new ArrayList();

            string query = "select EnquirePersonFirstName as UserName,D_O_B,Gender  from CCRMMEnquireForm  where MobileNo='" + MobileNo + "'";

            using (SqlCommand cmd_SubEXT = new SqlCommand(query, cnn))
            {
                cnn.Open();
                SqlDataReader DR_pdetails = cmd_SubEXT.ExecuteReader();
                if (DR_pdetails.Read())
                {
                    arl_pd.Add(DR_pdetails["UserName"].ToString());
                    arl_pd.Add(DR_pdetails["D_O_B"].ToString());
                    arl_pd.Add(DR_pdetails["Gender"].ToString());
                }
                cnn.Close();
            }
            return arl_pd;
        }

        // Function : ChangeSot  same like GetAllSlots in SlotController
        public string ChangeSlot([FromBody]OffLineCMSSlotsAvailability PackagePrices)
        {

            string sJSONResponse = "";

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_slots = new DataSet();
            string result = string.Empty;

            int AvailableSlots = 0;
            int FilledSlots = 0;
            int EnquiretypeNo = 0;
            string UniqueId = "";

            DataTable dt_IsPackage = new DataTable();
            string MobileNo = PackagePrices.MobileNo;
            string MembershipCode = PackagePrices.MembershipCode;

            DataTable dt_DailyPricePercentage = getdata(string.Format("select Days,DaysDuration,Percentage from DailyPricePercentage", ""));
            DataTable dt_Slotavailable = new DataTable();
            DataTable dt_Slotused = new DataTable();
            DataTable dt_Slot = new DataTable();

            string BranchCode = PackagePrices.BranchCode;
            string PackageCode = PackagePrices.PackageCode;

            ArrayList arl_Mobile = MobileCheck(PackagePrices.MobileNo);
            int EnquireformCount = Convert.ToInt32(arl_Mobile[0].ToString());
            int UserCount = Convert.ToInt32(arl_Mobile[1].ToString());

            try
            {

                if (MobileNo != null && MembershipCode != "")
                {
                    UniqueId = MobileNo;
                }
                else if (MobileNo != "" && MembershipCode == "")
                {
                    UniqueId = MobileNo;
                }
                else
                {
                    UniqueId = "";
                }


                DataTable dt = getdata(string.Format("select ID,DisplayName from DashboardDisplay", ""));

                if (UniqueId != "")
                {

                    dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM CCRMMembership AS T1,Users T2   WHERE  T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode)  SELECT (SELECT top 1 T1.EnquireTypeNo FROM CCRMMembership AS T1,Users T2  WHERE T2.MobileNo='{0}' and T1.MembershipCode=T2.UCode order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", PackagePrices.MobileNo));
                    EnquiretypeNo = Convert.ToInt32(dt_IsPackage.Rows[0]["EnquireTypeNo"].ToString());
                }
                else
                {
                    // dt_IsPackage = getdata(string.Format("IF EXISTS (SELECT EnquireTypeNo FROM CCRMMembership AS T1,Users T2   WHERE   T1.MembershipCode=T2.UCode)  SELECT (SELECT top 1 T1.EnquireTypeNo FROM CCRMMembership AS T1,Users T2  WHERE T1.MembershipCode=T2.UCode order by T1.ID desc ) as EnquireTypeNo  ELSE SELECT 0 as EnquireTypeNo", ""));
                    EnquiretypeNo = 0;
                }

                List<OffLinePackagePriceList> dbPackage = new List<OffLinePackagePriceList>();
                try
                {
                    if (EnquiretypeNo == 0)
                    {
                        EnquiretypeNo = 1;
                    }
                    else if (EnquiretypeNo > 0)
                    {
                        EnquiretypeNo = 0;

                    }
                    else
                    {
                        EnquiretypeNo = 1;
                    }
                }
                catch (Exception ec)
                {


                }
            }
            catch (Exception ec)
            {

            }

            //CreateDatatable
            DataTable dt_temptable = new DataTable();

            // days calculation

            DateTime currentday = DateTime.Now;
            DateTime firstDayThisMonth = new DateTime(currentday.Year, currentday.Month, 1);
            DateTime firstDayPlusTwoMonths = firstDayThisMonth.AddMonths(2);
            DateTime lastDayNextMonth = firstDayPlusTwoMonths.AddDays(-1);
            DateTime endOfLastDayNextMonth = firstDayPlusTwoMonths.AddTicks(-1);

            dt_Slotused = GetAllSlotsByPackageCodeUsedAUTOPRICE(PackagePrices.BranchCode, PackagePrices.PackageCode, Convert.ToString(PackagePrices.StartDate));

            if (dt_Slotused.Rows.Count > 0)
            {
                dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICE(PackagePrices.BranchCode, PackagePrices.PackageCode);
                foreach (DataRow rrow in dt_Slotavailable.Rows)
                {
                    foreach (DataRow trow in dt_Slotused.Rows)
                    {
                        rrow["Date"] = PackagePrices.StartDate;
                        AvailableSlots = Convert.ToInt32(rrow["RegularMembersPerSlot"]);
                        FilledSlots = Convert.ToInt32(trow["FilledSlots"].ToString());

                        if (rrow["SlotCode"].ToString() == trow["SlotCode"].ToString())
                        {

                            string val1 = rrow["SlotCode"].ToString();
                            string val2 = trow["SlotCode"].ToString();

                            if (trow["SNO"].ToString() == "1")
                            {

                                if (FilledSlots < AvailableSlots)
                                    rrow["IsRegularSlotAvailable"] = 1;
                                else if (FilledSlots == AvailableSlots)
                                    rrow["IsRegularSlotAvailable"] = 0;
                                else
                                    rrow["IsRegularSlotAvailable"] = 0;
                            }
                        }
                        else
                        {


                        }
                    }
                }
            }
            else
            {
                dt_Slotavailable = GetAllSlotsByPackageCodeAvailableAUTOPRICEDefault(BranchCode, PackageCode);
            }

            dt_temptable.Merge(dt_Slotavailable);

            int sloutcount = dt_temptable.Rows.Count;

            DataTable st_newSlotavailable = new DataTable();
            st_newSlotavailable = dt_temptable.DefaultView.ToTable();
            DataTable dt_FilteredSlotsAvailable = new DataTable();

            List<OffLineEnquireInfo> olei = new List<OffLineEnquireInfo>();
            olei.Add(new OffLineEnquireInfo { isFreeTrial = EnquiretypeNo, enquireTypeNo = 2 });

            DataTable dt_Sessions = getdata(string.Format("select distinct CMSSS.SessionCode,CMSSS.SessionName,convert(varchar(10),CMSSS.SessionStartTime, 108) as SessionStartTime,convert(varchar(10),  CMSSS.SessionEndTime, 108) as SessionEndTime  from  CMSSESSIONTIMESETTING CMSSS  order by SessionCode asc", ""));
            List<OffLineFreeTrialSessions> OnlineFreeTrialSessions = new List<OffLineFreeTrialSessions>();

            List<OffLineonlySlotDetails> dt_SlotPackageDopt = new List<OffLineonlySlotDetails>();

            for (int i = 0; i < dt_Sessions.Rows.Count - 1; i++)
            {

                OffLineonlySlotDetails dashboardlistpackage = new OffLineonlySlotDetails { sessionId = dt_Sessions.Rows[i]["SessionCode"].ToString(), sessionName = dt_Sessions.Rows[i]["SessionName"].ToString(), sessionStartTime = dt_Sessions.Rows[i]["SessionStartTime"].ToString(), sessionEndTime = dt_Sessions.Rows[i]["SessionEndTime"].ToString(), info = olei, slots = GetOnlySlotsDetails(st_newSlotavailable, PackagePrices.StartDate, BranchCode, dt_Sessions.Rows[i]["SessionCode"].ToString()) };
                dt_SlotPackageDopt.Add(dashboardlistpackage);

            }

            OffLineOnlySlotOutPut olppFOP = new OffLineOnlySlotOutPut();

            olppFOP.status = "Success";
            olppFOP.value = dt_SlotPackageDopt;

            sJSONResponse = JsonConvert.SerializeObject(olppFOP);


            return sJSONResponse;

        }
        public List<OffLineSessionPackage> GetSessionDetails(string BranchCode, string PackageCode)
        {

            List<OffLineSessionPackage> sessions = new List<OffLineSessionPackage>();
            DataTable dt_PackageSessions = getdata(string.Format("select SessionCode,SessionName,convert(varchar(10), SessionStartTime, 108) as SessionStartTime,convert(varchar(10), SessionEndTime, 108) as SessionEndTime from CMSSESSIONTIMESETTING", ""));

            DataTable dt_Sessions = GetAllSlotsByPackageCodeAvailableAUTOPRICE(BranchCode, PackageCode);

            DataView view = new DataView(dt_Sessions);
            dt_Sessions = view.ToTable(true, "SessionCode");


            //DataTable dt_filter = dt_Sessions.AsEnumerable().GroupBy(r => new { Col1 = r["SessionCode"] }).Select(g => g.OrderBy(r => r["SessionCode"]).First()).CopyToDataTable();

            for (int i = 0; i <= dt_PackageSessions.Rows.Count - 1; i++)
            {
                DataRow[] SessionSearch = dt_Sessions.Select("SessionCode = '" + dt_PackageSessions.Rows[i]["SessionCode"].ToString() + "'");
                if (SessionSearch.Length != 0)
                {
                    sessions.Add(new OffLineSessionPackage
                    {
                        sessionId = dt_PackageSessions.Rows[i]["SessionCode"].ToString()
                                    ,
                        sessionName = dt_PackageSessions.Rows[i]["SessionName"].ToString()
                                    ,
                        sessionStartTime = dt_PackageSessions.Rows[i]["SessionStartTime"].ToString()
                                   ,
                        sessionEndTime = dt_PackageSessions.Rows[i]["SessionEndTime"].ToString()

                    });
                }


            }
            return sessions;

        }
        public List<OffLineOnlySlots> GetOnlySlotsDetails(DataTable dt_Slot, string Date, string BranchCode, string SessionCode)
        {
            DataTable dt_temptable = new DataTable();
            DataTable dt_filter = new DataTable();

            DataRow[] rows = dt_Slot.Select("SessionCode = '" + SessionCode + "' ");
            List<OffLineOnlySlots> offLineSlot = new List<OffLineOnlySlots>();

            try
            {
                dt_filter = rows.CopyToDataTable();

                for (int i = 0; i < dt_filter.Rows.Count; i++)
                {
                    Boolean IsRegularSlotAvailable = false;
                    int a = Convert.ToInt32(dt_filter.Rows[i]["IsRegularSlotAvailable"]);
                    if (a == 0)
                    { IsRegularSlotAvailable = false; }
                    else
                    { IsRegularSlotAvailable = true; }

                    offLineSlot.Add(new OffLineOnlySlots
                    {
                        sNo = Convert.ToInt32(dt_filter.Rows[i]["SNO"])
                        ,
                        slotCode = Convert.ToString(dt_filter.Rows[i]["SlotCode"])
                         ,
                        isSlotAvailable = IsRegularSlotAvailable
                         ,
                        slotStartTime = Convert.ToString(dt_filter.Rows[i]["SlotStartTime"])
                         ,
                        slotEndTime = Convert.ToString(dt_filter.Rows[i]["SlotEndTime"])

                    });

                }

            }
            catch (Exception ec)
            {

            }

            return offLineSlot;
        }
        public List<OffLineDurationWiseColums> GetSevenDaysDurationColumns(DataTable st_newSlotavailable)
        {

            //        dt.AsEnumerable()
            //.GroupBy(r => new { Col1 = r["Col1"], Col2 = r["Col2"] })
            //.Select(g =>
            //{
            //    var row = dt.NewRow();

            //    row["PK"] = g.Min(r => r.Field<int>("PK"));
            //    row["Col1"] = g.Key.Col1;
            //    row["Col2"] = g.Key.Col2;

            //    return row;

            //})
            //.CopyToDataTable();

            // get minimum price / duration : Logic
            DataTable dt_duratuonfilter = new DataTable();

            // dt_duratuonfilter = st_newSlotavailable.AsEnumerable().GroupBy(r => new { Col1 = r["SlotCode"], Col2 = r["DurationCode"] }).Select(g => g.OrderBy(r => r["SlotPrice"]).First()).CopyToDataTable();



            st_newSlotavailable = st_newSlotavailable.AsEnumerable().GroupBy(r => new { Col1 = r["DurationCode"] }).Select(g => { var row = st_newSlotavailable.NewRow(); row["SlotPrice"] = g.Min(r => r.Field<double>("SlotPrice")); row["DurationCode"] = g.Key.Col1; return row; }).CopyToDataTable();



            // var GroupBy = st_newSlotavailable.AsEnumerable().GroupBy(e => e.Field<string>("DurationCode")).Select(d => new { d.Key, Count = d.Count() });
            //DataTable dtTemp = GroupBy.cop
            //DataTable dtObj2 = new DataTable("tableName2"); // Set table name    
            //                                                //merging first data table into second data table      
            //dtObj2.Merge(dtObj1);

            List<OffLineDurationWiseColums> dc = new List<OffLineDurationWiseColums>();


            for (int i = 0; i <= st_newSlotavailable.Rows.Count - 1; i++)
            {
                dc.Add(new OffLineDurationWiseColums
                {
                    sno = i + 1
                                    ,

                    durationCode = st_newSlotavailable.Rows[i]["DurationCode"].ToString()
                                    ,
                    slotPrice = Convert.ToDouble(st_newSlotavailable.Rows[i]["SlotPrice"].ToString())

                });
            }


            return dc;

        }
        public DataTable GetAllSlotsByPackageCodeAvailableAUTOPRICEDefault(string BranchCode, string PackageCode)
        {
            DataSet ds_custdet1 = new DataSet();
            DataTable dt_Slotav = new DataTable();
            string result = string.Empty;

            int l = 1;

            DataRow row;
            DataColumn col1 = new DataColumn("SNO", typeof(int));
            DataColumn col2 = new DataColumn("SessionCode", typeof(string));
            DataColumn col3 = new DataColumn("SlotCode", typeof(string));
            DataColumn col4 = new DataColumn("SlotStartTime", typeof(string));
            DataColumn col5 = new DataColumn("SlotEndTime", typeof(string));
            DataColumn col6 = new DataColumn("RegularMembersPerSlot", typeof(int));
            DataColumn col7 = new DataColumn("IsRegularSlotAvailable", typeof(string));
            DataColumn col8 = new DataColumn("Date", typeof(DateTime));



            dt_Slotav.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8 });
            string query1 = "";
            if (PackageCode == "P0")
            {
                query1 = "select CMSSS.SlotCode,CMSSS.SessionCode,SG.FreeMembersPerSlot,PG.PackageWiseSlot as RegularMembersPerSlot,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime  from CMSSLOTTIMESETTING CMSSS,SlotGroups SG,PackageGroup PG  where   CMSSS.SLTGID=PG.SLTGID and CMSSS.SLTGID=SG.SLTGID and PG.SLTGID=SG.SLTGID   and CMSSS.SLTGID = SG.SLTGID  and SG.BranchCode = '" + BranchCode + "'  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSSS.SlotCode,PG.PackageWiseSlot,CMSSS.SlotStartTime,CMSSS.SlotEndTime,CMSSS.SessionCode";
            }
            else
            {
                query1 = "select CMSSS.SlotCode,CMSSS.SessionCode,FreeMembersPerSlot=1,CMSSWA.AllocatedCount as RegularMembersPerSlot,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING CMSSS,CMSSlotWiseAllocation CMSSWA where CMSSS.SlotCode=CMSSWA.SlotCode and  CMSSWA.PackageCode = '" + PackageCode + "'   and CMSSWA.BranchCode='" + BranchCode + "'   group by CMSSS.SlotName,CMSSS.SlotCode,CMSSWA.AllocatedCount,CMSSS.SlotStartTime,CMSSS.SlotEndTime,CMSSS.SessionCode union all select distinct SlotCode,SessionCode,FreeMembersPerSlot=0,RegularMembersPerSlot=0,convert(varchar(10),SlotStartTime, 108) as SlotStartTime,convert(varchar(10),SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING  where SlotCode not in(select SlotCode from CMSSlotWiseAllocation  where PackageCode = '" + PackageCode + "'   and BranchCode='" + BranchCode + "')";
            }

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            for (int j = 0; j <= ds_custdet1.Tables[0].Rows.Count - 1; j++)
            {
                row = dt_Slotav.NewRow();
                row["SNO"] = l;
                row["SlotCode"] = ds_custdet1.Tables[0].Rows[j]["SlotCode"].ToString();
                row["SessionCode"] = ds_custdet1.Tables[0].Rows[j]["SessionCode"].ToString();
                row["SlotStartTime"] = 0.0f;
                row["SlotEndTime"] = 0.0f;
                row["RegularMembersPerSlot"] = ds_custdet1.Tables[0].Rows[j]["RegularMembersPerSlot"].ToString();
                row["IsRegularSlotAvailable"] = "0";
                row["Date"] = DBNull.Value;
                dt_Slotav.Rows.Add(row);
                l = l + 1;
            }

            return dt_Slotav;
        }
        public DataTable GetAllSlotsByPackageCodeAvailableAUTOPRICE(string BranchCode, string PackageCode)
        {
            DataTable dt1 = new DataTable();
            DataSet ds_custdet1 = new DataSet();
            DataTable dt_Slotav = new DataTable();
            string result = string.Empty;

            int l = 1;
            DataRow row;
            DataColumn col1 = new DataColumn("SNO", typeof(int));
            DataColumn col2 = new DataColumn("SlotCode", typeof(string));
            DataColumn col3 = new DataColumn("SessionCode", typeof(string));
            DataColumn col4 = new DataColumn("SlotStartTime", typeof(string));
            DataColumn col5 = new DataColumn("SlotEndTime", typeof(string));
            DataColumn col6 = new DataColumn("RegularMembersPerSlot", typeof(int));
            DataColumn col7 = new DataColumn("IsRegularSlotAvailable", typeof(string));
            DataColumn col8 = new DataColumn("Date", typeof(DateTime));

            dt_Slotav.Columns.AddRange(new DataColumn[] { col1, col2, col3, col4, col5, col6, col7, col8 });
            string query1 = "";
            if (PackageCode == "P0")
            {

                query1 = "select CMSSS.SlotCode,CMSSS.SessionCode,SG.FreeMembersPerSlot,PG.PackageWiseSlot as RegularMembersPerSlot,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime  from CMSSLOTTIMESETTING CMSSS,SlotGroups SG,PackageGroup PG  where   CMSSS.SLTGID=PG.SLTGID and CMSSS.SLTGID=SG.SLTGID and PG.SLTGID=SG.SLTGID   and CMSSS.SLTGID = SG.SLTGID  and SG.BranchCode = '" + BranchCode + "'  group by SG.FreeMembersPerSlot,RegularMembersPerSlot,CMSSS.SlotName,CMSSS.SlotCode,PG.PackageWiseSlot,CMSSS.SlotStartTime,CMSSS.SlotEndTime,CMSSS.SessionCode";
            }
            else
            {

                query1 = "select CMSSS.SlotCode,CMSSS.SessionCode,FreeMembersPerSlot=1,CMSSWA.AllocatedCount as RegularMembersPerSlot,convert(varchar(10),CMSSS.SlotStartTime, 108) as SlotStartTime,convert(varchar(10),CMSSS.SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING CMSSS,CMSSlotWiseAllocation CMSSWA where CMSSS.SlotCode=CMSSWA.SlotCode and  CMSSWA.PackageCode = '" + PackageCode + "'   and CMSSWA.BranchCode='" + BranchCode + "'   group by CMSSS.SlotName,CMSSS.SlotCode,CMSSWA.AllocatedCount,CMSSS.SlotStartTime,CMSSS.SlotEndTime,CMSSS.SessionCode union all select distinct SlotCode,SessionCode,FreeMembersPerSlot=0,RegularMembersPerSlot=0,convert(varchar(10),SlotStartTime, 108) as SlotStartTime,convert(varchar(10),SlotEndTime, 108) as SlotEndTime from CMSSLOTTIMESETTING  where SlotCode not in(select SlotCode from CMSSlotWiseAllocation  where PackageCode = '" + PackageCode + "'   and BranchCode='" + BranchCode + "')";
            }

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            for (int j = 0; j <= ds_custdet1.Tables[0].Rows.Count - 1; j++)
            {

                row = dt_Slotav.NewRow();

                row["SNO"] = l;
                row["SlotCode"] = ds_custdet1.Tables[0].Rows[j]["SlotCode"].ToString();
                row["SessionCode"] = ds_custdet1.Tables[0].Rows[j]["SessionCode"].ToString();
                row["SlotStartTime"] = ds_custdet1.Tables[0].Rows[j]["SlotStartTime"].ToString();
                row["SlotEndTime"] = ds_custdet1.Tables[0].Rows[j]["SlotEndTime"].ToString();
                row["RegularMembersPerSlot"] = ds_custdet1.Tables[0].Rows[j]["RegularMembersPerSlot"].ToString();
                row["Date"] = DBNull.Value;
                if (Convert.ToInt32(ds_custdet1.Tables[0].Rows[j]["RegularMembersPerSlot"].ToString()) == 0)
                    row["IsRegularSlotAvailable"] = "0";
                else
                    row["IsRegularSlotAvailable"] = "1";

                dt_Slotav.Rows.Add(row);
                l = l + 1;
            }

            return dt_Slotav;
        }
        public DataTable GetAllSlotsByPackageCodeUsedAUTOPRICE(string BranchCode, string PackageCode, string StartDate)
        {
            DataSet ds_custdet1 = new DataSet();
            DataTable dt_Slotav = new DataTable();

            string query1 = "select SNO=1,Name='Regular',CCRMM.SlotCode,SG.RegularMembersPerSlot as AvailableSlots,COUNT(*) as FilledSlots,CCRMM.PackageCode from CCRMMembership CCRMM,CMSSLOTTIMESETTING CMSSS,SlotGroups SG where CCRMM.SlotCode=CMSSS.SlotCode and CMSSS.SLTGID=SG.SLTGID and CCRMM.BranchCode = '" + BranchCode + "'  and CCRMM.IsActive = 1 and CCRMM.StartDate <= Dateadd(DAY, Datediff(DAY, -1, '" + StartDate + "'), 0) and  CCRMM.MembershipExpireDate >= Dateadd(DAY, Datediff(DAY, -1,'" + StartDate + "'), 0) group by CCRMM.SlotCode,SG.RegularMembersPerSlot,CCRMM.PackageCode ";


            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            dt_Slotav = ds_custdet1.Tables[0];
            return dt_Slotav;
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
        public DataTable GetTrainersListBySlots(string SlotCode, string BranchCode, string PackageCode)
        {
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DataSet ds_custdet1 = new DataSet();

            DataTable dt_TrainersList = new DataTable();

            string result = string.Empty;

            DataRow row;
            DataColumn col1 = new DataColumn("TrainerCode", typeof(string));
            DataColumn col2 = new DataColumn("TrainerName", typeof(string));

            dt_TrainersList.Columns.AddRange(new DataColumn[] { col1, col2 });

            string query1 = "select TrainerCode,TrainerName from TrainerSlotSpecializationMapping where SlotCode='" + SlotCode + "' and BranchCode='" + BranchCode + "' and PackageCode='" + PackageCode + "' and IsActive=1 ";

            using (SqlDataAdapter da_custdet = new SqlDataAdapter(query1, cnn))
            {
                da_custdet.Fill(ds_custdet1);
            }

            if (ds_custdet1.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i <= ds_custdet1.Tables[0].Rows.Count - 1; i++)
                {
                    row = dt_TrainersList.NewRow();
                    row["TrainerCode"] = ds_custdet1.Tables[0].Rows[i][0].ToString();
                    row["TrainerName"] = ds_custdet1.Tables[0].Rows[i][1].ToString();

                    dt_TrainersList.Rows.Add(row);
                }
            }
            return dt_TrainersList;

        }
        public string Last2digits(int num)
        {
            string x = Convert.ToString(num);
            for (int i = 0; i < x.Length; ++i)
            {
                if (i == 0)
                    x = x.Substring(x.Length - i - 2, 2).PadRight(i + 2, '0');

            }

            return x;
        }

        // GetFreezingDaysInfo
        //   use NARENGYMP
        //-- Final Response : TotalFreezingDays : Freezed Days : LeftOutFreezingDays
        //-- Request : Invoice 
        //-- Tales : FAInvoice Table, CMSPlanCost Table.
        //--TotalFreezingDays
        //--PlanCodeCost
        //select* from FAInvoice 
        //--PlanCodeCost
        //select* from CMSPLANCOST 
        //--FreezingID
        //--sub
        //select* from FreezingFacility 
        //--Respons : Table(FreezingFacility) Column(NoOfDays)

        //--Freezed Days
        //select* from CCRMMembershipFacility where MFDID=1
        //-- Where MFID = 1 :  Days count By : where(Invoice) = SUM(Days)
        //--LeftOutFreezingDays
        //--TotalFreezingDays - FreezedDays

        public Object GetFeezingDetails([FromBody]FreezingDaysPlan fdp)
        {

            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();

            DataTable dt_noofdays = getdata(string.Format("select top 1 NoOfDays from CCRMMembership CCRMM,CMSPLANCOST CMSPC,FreezingFacility FF where CCRMM.PlanCostCode=CMSPC.PlanCostCode and CMSPC.FreezingID=FF.FreezingID and CCRMM.InvoiceID='{0}' ", fdp.Invoice));
            DataTable dt_count = getdata(string.Format("select ISNULL(SUM(Days),0) as SumofDays from CCRMMembershipFacility where MFDID=1 and InvoiceID='{0}' ", fdp.Invoice));

            FreezingOutPut fop = new FreezingOutPut();

            try
            {
                List<FreezingValues> facilityvalue = new List<FreezingValues>();

                if (dt_noofdays.Rows.Count > 0)
                {
                    facilityvalue.Add(new FreezingValues
                    {

                        TotalFreezingDays = Convert.ToInt32(dt_noofdays.Rows[0]["NoOfDays"].ToString())
                    ,
                        FreezedDays = Convert.ToInt32(dt_count.Rows[0]["SumofDays"].ToString())
                    ,
                        LeftOutFreezingDays = Convert.ToInt32(dt_noofdays.Rows[0]["NoOfDays"].ToString()) - Convert.ToInt32(dt_count.Rows[0]["SumofDays"].ToString())
                        ,
                        Note = "If freezing days are more than 15 your slot will be released and need to book slot again at time of resume service."


                    });
                }
                else
                {
                    facilityvalue.Add(new FreezingValues
                    {

                        TotalFreezingDays = 0
                   ,
                        FreezedDays = 0
                   ,
                        LeftOutFreezingDays = 0
                         ,
                        Note = "If freezing days are more than 15 your slot will be released and need to book slot again at time of resume service."

                    });
                }

                fop.status = "Success";
                fop.value = facilityvalue;
                sJSONResponse = JsonConvert.SerializeObject(fop);

            }
            catch (Exception ec)
            {
                fop.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(fop);

            }
            return sJSONResponse;

        }

        // CCRMMembershipFacility


        public Object GetFacilityUsedDetails([FromBody]FacilityUsedInput fui)
        {

            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();
            //
            DataTable dt_facilityused = getdata(string.Format("select SMFM.SMFMID as MFDID,SMFM.SubSchemName as FacilityName from CCRMMembershipFacility CCRMF,SubMembersFacilityMaster SMFM where CCRMF.InvoiceID='{0}' and CCRMF.MemberShipCode='{1}' and CCRMF.MFDID=SMFM.MFMID ", fui.InvoiceId, fui.MembershipCode));

            FacilityUsedOutput fs = new FacilityUsedOutput();

            try
            {
                List<Facilities> facilityvalue = new List<Facilities>();

                for (int i = 0; i <= dt_facilityused.Rows.Count - 1; i++)
                {
                    facilityvalue.Add(new Facilities
                    {

                        FacilityId = Convert.ToInt32(dt_facilityused.Rows[i]["MFDID"].ToString())
                        ,
                        FacilityName = dt_facilityused.Rows[i]["FacilityName"].ToString()
                        ,
                        FacilityUsed = GetMembersFacilityDetails(Convert.ToInt32(dt_facilityused.Rows[i]["MFDID"].ToString()), fui.InvoiceId, fui.MembershipCode)


                    });

                }

                fs.status = "Success";
                fs.value = facilityvalue;
                sJSONResponse = JsonConvert.SerializeObject(fs);

            }
            catch (Exception ec)
            {
                fs.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(fs);

            }
            return sJSONResponse;

        }


        public List<FacilityUsedMembersDetails> GetMembersFacilityDetails(int Mfdid, string InvoiceId, string MembershipCode)
        {
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            List<FacilityUsedMembersDetails> facilityMembers = new List<FacilityUsedMembersDetails>();
            DataTable dt_FacilityDetails = getdata(string.Format("select BranchCode,MFDID,InvoiceID,RemainingAmount,PreviousExpiryDate,FacilityStartDate,FacilityExpireDate as FacilityEndDate,LeftOutDays,LeftOutAmount,FreezingDays,Days,NewMembersCode,NewInvoiceId,NewInvoiceId,CreatedOn,CreatedBy from CCRMMembershipFacility where InvoiceID='{0}' and MemberShipCode='{1}' ", InvoiceId, MembershipCode));

            for (int i = 0; i <= dt_FacilityDetails.Rows.Count - 1; i++)
            {

                facilityMembers.Add(new FacilityUsedMembersDetails
                {
                    Days = dt_FacilityDetails.Rows[i]["Days"].ToString()
                                    ,
                    FacilityExpireDate = dt_FacilityDetails.Rows[i]["FacilityEndDate"].ToString()
                    ,
                    FacilityStartDate = dt_FacilityDetails.Rows[i]["FacilityStartDate"].ToString()
                    ,
                    FreezingDays = dt_FacilityDetails.Rows[i]["FreezingDays"].ToString()
                                    ,
                    InvoiceID = dt_FacilityDetails.Rows[i]["InvoiceID"].ToString()
                    ,
                    LeftOutAmount = dt_FacilityDetails.Rows[i]["LeftOutAmount"].ToString()
                    ,
                    LeftOutDays = dt_FacilityDetails.Rows[i]["LeftOutDays"].ToString()
                                    ,
                    NewInvoiceId = dt_FacilityDetails.Rows[i]["NewInvoiceId"].ToString()
                    ,
                    NewMembersCode = dt_FacilityDetails.Rows[i]["NewMembersCode"].ToString()
                    ,
                    PreviousExpiryDate = dt_FacilityDetails.Rows[i]["PreviousExpiryDate"].ToString()
                                    ,
                    RemainingAmount = dt_FacilityDetails.Rows[i]["RemainingAmount"].ToString()
                    ,
                    MFDID = dt_FacilityDetails.Rows[i]["MFDID"].ToString()
                    ,
                    BranchCode = dt_FacilityDetails.Rows[i]["BranchCode"].ToString()
                    ,
                    CreatedBy = dt_FacilityDetails.Rows[i]["CreatedBy"].ToString()
                    ,
                    CreatedOn = ServerDateTime

                });



            }
            return facilityMembers;

        }
    }
}