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
using NarenFitnessUsers.Models.Trainer;
using NarenFitnessUsers.Models.Branch;
using NarenFitnessUsers.Models.Branch.BranchDetails;
using NarenFitnessUsers.Models.Branch.Place;
using NarenFitnessUsers.Models.Branch.BranchVideos;
using NarenFitnessUsers.Models.Branch.BranchImage;
using NarenFitnessUsers.Class;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Globalization;
using System.Web.Helpers;
using System.Collections;


namespace NarenFitnessUsers.Controllers
{
    public class BranchController : Controller
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

        public Object GetBranchList([FromBody]BranchInput Trainer)
        {

            DataTable dt_Trainer = new DataTable();
            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();

            BranchOutput topt = new BranchOutput();
            List<BranchFields> Branchfields = new List<BranchFields>();
            try
            {
                dt_Trainer = getdata(string.Format("select BranchCode, BranchName, Address, Latitude, Longitude, Imageurl from Branch where IsActive = 1 and IsDeleted = 0 and BranchCode not in (1104) and BranchCode not in (1106)", ""));

                for (int i = 0; i <= dt_Trainer.Rows.Count - 1; i++)
                {
                    BranchFields BranchDetails = new BranchFields { branchCode = dt_Trainer.Rows[i]["BranchCode"].ToString(), branchName = dt_Trainer.Rows[i]["BranchName"].ToString(), address = dt_Trainer.Rows[i]["Address"].ToString(), latitude = dt_Trainer.Rows[i]["Latitude"].ToString(), longitude = dt_Trainer.Rows[i]["Longitude"].ToString(), branchImage = dt_Trainer.Rows[i]["Imageurl"].ToString() };
                    Branchfields.Add(BranchDetails);
                }

                topt.status = "success";
                topt.value = Branchfields;
                sJSONResponse = JsonConvert.SerializeObject(topt);

            }
            catch (Exception ec)
            {
                topt.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(topt);

            }

            return sJSONResponse;

        }

        //Trainers Details
        public Object GetBranchDetails([FromBody]BranchInput Branch)
        {

            string sJSONResponse = "";
            DataSet dt_Packageresposne = new DataSet();

            GetBranchOutput tdopt = new GetBranchOutput();
            List<GetBranchFields> BranchAttribute = new List<GetBranchFields>();

            DataTable dt = getdata(string.Format("select Address,Latitude,Longitude,BranchTimming,AboutBranch from Branch where BranchCode='{0}' ", Branch.BranchCode));
            try
            {

                GetBranchFields BranchDetails = new GetBranchFields { aboutBranch = dt.Rows[0]["AboutBranch"].ToString(), branchTiming = dt.Rows[0]["BranchTimming"].ToString(), branchAddress = dt.Rows[0]["Address"].ToString(), latitude = dt.Rows[0]["Latitude"].ToString(), longitude = dt.Rows[0]["Longitude"].ToString(), branchImages = GetBranchImageDetails(Branch.BranchCode), branchVideos = GetBranchVideoDetails(Branch.BranchCode) };
                BranchAttribute.Add(BranchDetails);

                tdopt.status = "success";
                tdopt.value = BranchAttribute;
                sJSONResponse = JsonConvert.SerializeObject(tdopt);

            }
            catch (Exception ec)
            {
                tdopt.status = "success";
                sJSONResponse = JsonConvert.SerializeObject(tdopt);

            }




            return sJSONResponse;

        }
        public List<GetBranchImages> GetBranchImageDetails(string BranchCode)
        {
            List<GetBranchImages> _Images = new List<GetBranchImages>();

            DataTable dt = getdata(string.Format("select Imagename,ImageUrl from BranchImages where BranchCode='{0}'", BranchCode));
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                _Images.Add(new GetBranchImages
                {
                    imageName = Convert.ToString(dt.Rows[i]["Imagename"].ToString())
                                                     ,
                    imageUrl = Convert.ToString(dt.Rows[i]["ImageUrl"].ToString())

                });
            }
            return _Images;
        }
        public List<GetBranchVideos> GetBranchVideoDetails(string BranchCode)
        {
            List<GetBranchVideos> _videos = new List<GetBranchVideos>();

            DataTable dt = getdata(string.Format("select Videoname,VideoUrl from BranchVideos where BranchCode='{0}' ", BranchCode));

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                _videos.Add(new GetBranchVideos
                {
                    videoName = Convert.ToString(dt.Rows[i]["Videoname"].ToString())
                                                  ,
                    videoUrl = Convert.ToString(dt.Rows[i]["VideoUrl"].ToString())

                });
            }

            return _videos;
        }

        // GET: ApplicationTypes
        public Object CountryPost([FromBody]PlaceInput pi)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string country_Query = "";
            PlanOutPut Pcountry = new PlanOutPut();

            try
            {

                cnn.Open();
                country_Query = "insert into Country(CountryName,IsDeleted,IsActive) values('" + pi.CountryName + "',0,1) SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                Pcountry.status = "Success";

            }
            catch (Exception ex)
            {
                Pcountry.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(Pcountry);

            return sJSONResponse;
        }
        public Object CountryUpdate([FromBody]PlaceInput pi)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            PlanOutPut Pcountry = new PlanOutPut();
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
                    command.CommandText = "update Country set CountryName='" + pi.CountryName + "'  where CountryId=" + pi.CountryId + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    Pcountry.status = "Success";
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
                Pcountry.status = "Fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(Pcountry);

            return sJSONResponse;
        }
        public Object CountryDelete([FromBody]PlaceInput pi)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            PlanOutPut Pcountry = new PlanOutPut();
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
                    command.CommandText = "delete from Country where CountryId=" + pi.CountryId + " ";
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    Pcountry.status = "Success";
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
                Pcountry.status = "Fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(Pcountry);

            return sJSONResponse;
        }
        public Object GetCountry([FromBody]PlaceInput pi)
        {
            CountryGet daOP = new CountryGet();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Sessions = new DataTable();

            List<Country> country = new List<Country>();

            try
            {
                dt_Sessions = getdata(string.Format("select CountryId,CountryName from Country", ""));

                for (int i = 0; i < dt_Sessions.Rows.Count; i++)
                {
                    Country cntry = new Country { CountryId = dt_Sessions.Rows[i]["CountryId"].ToString(), CountryName = dt_Sessions.Rows[i]["CountryName"].ToString() };
                    country.Add(cntry);
                }

                daOP.status = "success";
                daOP.value = country;
                sJSONResponse = JsonConvert.SerializeObject(daOP);


            }
            catch (Exception ec)
            {
                daOP.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(daOP);
            }


            return sJSONResponse;
        }

        public Object StatePost([FromBody]PlaceInput pi)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string country_Query = "";
            PlanOutPut Pstate = new PlanOutPut();

            try
            {

                cnn.Open();
                country_Query = "insert into States(StatesName,CountryId,IsDeleted,IsActive) values('" + pi.StatesName + "','" + pi.CountryId + "',0,1) SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                Pstate.status = "Success";

            }
            catch (Exception ex)
            {
                Pstate.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(Pstate);

            return sJSONResponse;
        }
        public Object StateUpdate([FromBody]PlaceInput pi)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            PlanOutPut Pstate = new PlanOutPut();
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
                    command.CommandText = "update States set StatesName='" + pi.StatesName + "'  where StatesId=" + pi.StatesId + " ";
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    Pstate.status = "Success";
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
                Pstate.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(Pstate);

            return sJSONResponse;
        }
        public Object StateDelete([FromBody]PlaceInput pi)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            PlanOutPut Pstate = new PlanOutPut();
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
                    command.CommandText = "delete from States where StatesId=" + pi.StatesId + " ";
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    Pstate.status = "Success";
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
                Pstate.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(Pstate);

            return sJSONResponse;
        }
        public Object GetState([FromBody]PlaceInput pi)
        {
            StateGet daOP = new StateGet();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Sessions = new DataTable();

            List<State> state = new List<State>();

            try
            {
                dt_Sessions = getdata(string.Format("select StatesId,StatesName from States", ""));

                for (int i = 0; i < dt_Sessions.Rows.Count; i++)
                {
                    State stt = new State { StatesId = dt_Sessions.Rows[i]["StatesId"].ToString(), StatesName = dt_Sessions.Rows[i]["StatesName"].ToString() };
                    state.Add(stt);
                }

                daOP.status = "success";
                daOP.value = state;
                sJSONResponse = JsonConvert.SerializeObject(daOP);


            }
            catch (Exception ec)
            {
                daOP.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(daOP);
            }


            return sJSONResponse;
        }

        public Object CityPost([FromBody]PlaceInput pi)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string country_Query = "";
            PlanOutPut Pcity = new PlanOutPut();

            try
            {

                cnn.Open();
                country_Query = "insert into city(CityName,StatesId,IsDeleted,IsActive) values('" + pi.CityName + "'," + pi.StatesId + ",0,1) SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                Pcity.status = "Success";

            }
            catch (Exception ex)
            {
                Pcity.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(Pcity);

            return sJSONResponse;
        }
        public Object CityUpdate([FromBody]PlaceInput pi)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            PlanOutPut Pcity = new PlanOutPut();
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

                    command.CommandText = "update city set CityName='" + pi.CityName + "'  where City=" + pi.cityId + "  ";
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    Pcity.status = "Success";
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
                Pcity.status = "Fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(Pcity);

            return sJSONResponse;
        }
        public Object CityDelete([FromBody]PlaceInput pi)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            PlanOutPut Pcity = new PlanOutPut();
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
                    command.CommandText = "delete from City where City=0 ";
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    Pcity.status = "Success";
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
                Pcity.status = "Fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(Pcity);

            return sJSONResponse;
        }
        public Object GetCity([FromBody]PlaceInput pi)
        {
            CityGet daOP = new CityGet();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Sessions = new DataTable();

            List<City> city = new List<City>();

            try
            {
                dt_Sessions = getdata(string.Format("select city,CityName from city", ""));

                for (int i = 0; i < dt_Sessions.Rows.Count; i++)
                {
                    City stt = new City { cityId = dt_Sessions.Rows[i]["city"].ToString(), CityName = dt_Sessions.Rows[i]["CityName"].ToString() };
                    city.Add(stt);
                }

                daOP.status = "success";
                daOP.value = city;
                sJSONResponse = JsonConvert.SerializeObject(daOP);


            }
            catch (Exception ec)
            {
                daOP.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(daOP);
            }


            return sJSONResponse;
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
                string filepath = @"C:\inetpub\wwwroot\GYMUI\BranchImages\\" + endpath;
                using (var imageFile = new FileStream(filepath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                //urlform = "http://137.59.201.211/GYMUI/BranchImages/" + endpath;
                urlform = "http://202.143.96.72/GYMUI/BranchImages/" + endpath;
            }
            catch (Exception ec)
            {
                urlform = "";
            }

            return urlform;
        }
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
        public Object BranchImagePost([FromBody]BranchImageDetails bid)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string country_Query = "";
            PlanOutPut Pout = new PlanOutPut();


            string b = bid.Image;
            b = b.Substring(b.IndexOf(",") + 1);
            string PhotoUrl = GetPhotoUrl2(b, imgname() + 10001);

            try
            {

                cnn.Open();
                country_Query = "insert into BranchImages(BranchCode,Imagename,Image,ImageUrl,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + bid.BranchCode + "','" + bid.Imagename + "','" + bid.Image + "','" + PhotoUrl + "','" + bid.CreatedBy + "','" + ServerDateTime + "',0,1) SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                Pout.status = "Success";

            }
            catch (Exception ex)
            {
                Pout.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(Pout);

            return sJSONResponse;
        }
        public Object BranchImageUpdate([FromBody]BranchImageDetails bid)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();

            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            PlanOutPut Pout = new PlanOutPut();
            string b = bid.Image;
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

                    command.CommandText = "update BranchImages set ImageUrl='" + PhotoUrl + "',Image='" + bid.Image + "'  where SNO=" + bid.ID + " ";
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    Pout.status = "Success";
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
                Pout.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(Pout);

            return sJSONResponse;
        }
        public Object BranchImageDelete([FromBody]BranchImageDetails bid)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            PlanOutPut Pout = new PlanOutPut();
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
                    command.CommandText = "delete from BranchImages where SNO=" + bid.ID + " ";
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    Pout.status = "success";
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
                Pout.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(Pout);

            return sJSONResponse;
        }
        public Object GetBranchImage([FromBody]BranchInput bid)
        {
            BranchImageList bil = new BranchImageList();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Branch = new DataTable();

            List<BranchImageDetails2> BranchImage = new List<BranchImageDetails2>();

            try
            {
                dt_Branch = getdata(string.Format("select SNO,ImageUrl,CreatedBy from BranchImages where BranchCode='{0}'  ", bid.BranchCode));

                for (int i = 0; i < dt_Branch.Rows.Count; i++)
                {
                    BranchImageDetails2 bi = new BranchImageDetails2 { ID = Convert.ToInt32(dt_Branch.Rows[i]["SNO"].ToString()), ImageUrl = dt_Branch.Rows[i]["ImageUrl"].ToString(), CreatedBy = dt_Branch.Rows[i]["CreatedBy"].ToString() };
                    BranchImage.Add(bi);
                }

                bil.status = "success";
                bil.value = BranchImage;
                sJSONResponse = JsonConvert.SerializeObject(bil);


            }
            catch (Exception ec)
            {
                bil.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(bil);
            }


            return sJSONResponse;
        }

        public Object BranchVideosPost([FromBody]BranchVideoInput bvv)
        {
            string sJSONResponse = "";
            DataTable dt_AppType = new DataTable();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            int a = 0;
            string country_Query = "";
            PlanOutPut Pout = new PlanOutPut();

            try
            {

                cnn.Open();
                country_Query = "insert into BranchVideos(BranchCode,Videoname,VideoUrl,Video,CreatedBy,CreatedOn,IsDeleted,IsActive) values('" + bvv.BranchCode + "','" + bvv.Videoname + "','" + bvv.VideoUrl + "','" + bvv.Video + "','" + bvv.CreatedBy + "','" + ServerDateTime + "',0,1) SELECT @@IDENTITY;";
                SqlCommand tm_cmd = new SqlCommand(country_Query, cnn);
                a = Convert.ToInt32(tm_cmd.ExecuteScalar());
                Pout.status = "Success";

            }
            catch (Exception ex)
            {
                Pout.status = "Fail";
            }
            finally
            {
                cnn.Close();
            }

            sJSONResponse = JsonConvert.SerializeObject(Pout);

            return sJSONResponse;
        }
        public Object BranchVideosUpdate([FromBody]BranchVideosVideos bvv)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            string ServerDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            PlanOutPut Pout = new PlanOutPut();
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

                    command.CommandText = "update BranchVideos set Videoname='" + bvv.Videoname + "',VideoUrl='" + bvv.VideoUrl + "',Video='" + bvv.Video + "'  where SNO=" + bvv.SNO + " ";
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    Pout.status = "Success";
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
                Pout.status = "fail";
            }

            sJSONResponse = JsonConvert.SerializeObject(Pout);

            return sJSONResponse;
        }
        public Object BranchVideosDelete([FromBody]BranchVideosVideos bvv)
        {
            string sJSONResponse = "";
            SqlCommand command = cnn.CreateCommand();
            PlanOutPut Pout = new PlanOutPut();
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
                    command.CommandText = "delete from BranchVideos where SNO=" + bvv.SNO + " ";
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    Pout.status = "Success";
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
                Pout.status = "Success";
            }

            sJSONResponse = JsonConvert.SerializeObject(Pout);

            return sJSONResponse;
        }
        public Object GetBranchVideos([FromBody]BranchVideoInput bvv)
        {
            BranchVideoList bvl = new BranchVideoList();
            DataSet SelectedSlots = new DataSet();
            string sJSONResponse = "";

            DataTable dt_Branch = new DataTable();

            List<BranchVideosVideos> BranchImage = new List<BranchVideosVideos>();

            try
            {
                dt_Branch = getdata(string.Format("select SNO,Videoname,Video,VideoUrl from BranchVideos where BranchCode='{0}' ", bvv.BranchCode));

                for (int i = 0; i < dt_Branch.Rows.Count; i++)
                {
                    BranchVideosVideos bi = new BranchVideosVideos { SNO = Convert.ToInt32(dt_Branch.Rows[i]["SNO"].ToString()), Video = dt_Branch.Rows[i]["Video"].ToString(), Videoname = dt_Branch.Rows[i]["Videoname"].ToString(), VideoUrl = dt_Branch.Rows[i]["VideoUrl"].ToString() };
                    BranchImage.Add(bi);
                }

                bvl.status = "success";
                bvl.value = BranchImage;
                sJSONResponse = JsonConvert.SerializeObject(bvl);


            }
            catch (Exception ec)
            {
                bvl.status = "fail";
                sJSONResponse = JsonConvert.SerializeObject(bvl);
            }


            return sJSONResponse;
        }
    }
}